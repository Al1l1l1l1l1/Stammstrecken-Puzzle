using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Puzzle.Domain;

namespace STP.Application
{
    /// <summary>ADR042 observation owner. Every common UI command passes through this adapter, never the inner QA session.</summary>
    public sealed class AssignedHumanSession:IReviewPuzzleSession,IDisposable
    {
        private readonly IHumanAssignmentCatalog catalog;private readonly IHumanCalibrationRepository repository;private readonly ILocalSessionClock clock;private readonly ILocalSaveDispatcher dispatcher;private readonly HumanRunOrigin origin;private readonly int owner=Thread.CurrentThread.ManagedThreadId;private readonly SemaphoreSlim writer=new SemaphoreSlim(1,1);private readonly Guid epoch=Guid.NewGuid();private readonly SemaphoreSlim operations=new SemaphoreSlim(1,1);private bool operationBusy;
        private HumanObservationSnapshot? snapshot;private Public240ReviewSession? inner;private HumanExposurePresentation? exposure;private HumanPauseReason reasons;private bool busy,blocked=true,disposed,dirty,rideStarted;private string error="NONE";private string? observationToken;private long version,lastNow,firstDirty,lastDirty,previewAccumulated;private long? previewAnchor;private HumanObservationStatus status=HumanObservationStatus.Loading;
        public AssignedHumanSession(IHumanAssignmentCatalog catalog,IHumanCalibrationRepository repository,ILocalSessionClock clock,ILocalSaveDispatcher dispatcher,HumanRunOrigin origin)
        {this.catalog=catalog??throw new ArgumentNullException(nameof(catalog));this.repository=repository??throw new ArgumentNullException(nameof(repository));this.clock=clock??throw new ArgumentNullException(nameof(clock));this.dispatcher=dispatcher??throw new ArgumentNullException(nameof(dispatcher));this.origin=HumanGuard.Enum(origin);State=null!;Publish();}
        public AssignedHumanState State{get;private set;}
        public event Action<AssignedHumanState>? Changed;
        public IReviewPuzzleSession? PuzzleView{get{Owner();return exposure!=null&&inner!=null&&!blocked&&!disposed?this:null;}}
        public int HintCredits{get{Owner();var identity=Current?.Identity;return snapshot==null||identity==null?0:Math.Max(0,3-snapshot.Run.Trials.Where(t=>t.Identity.Equals(identity)).Sum(t=>t.Hints.Count));}}
        private HumanTrialRecord? Current=>snapshot?.Run.Trials.LastOrDefault(t=>catalog.Assignments[snapshot.Run.NextAssignmentIndex<catalog.Assignments.Count?snapshot.Run.NextAssignmentIndex:catalog.Assignments.Count-1].Id==t.AssignmentId);
        private HumanAssignment? Assignment=>snapshot==null||snapshot.Run.NextAssignmentIndex>=catalog.Assignments.Count?null:catalog.Assignments[snapshot.Run.NextAssignmentIndex];
        private bool Solved=>inner?.IsAwaitingSaveCommit==true;
        private bool ShouldPause=>reasons!=HumanPauseReason.None||busy||operationBusy||blocked||Current?.Contact==null||Current?.Exposure!=HumanExposureState.EXPOSURE_CONFIRMED;
        private void Owner(){if(Thread.CurrentThread.ManagedThreadId!=owner)throw new InvalidOperationException("HUMAN_OWNER_THREAD_REQUIRED");if(disposed)throw new ObjectDisposedException(nameof(AssignedHumanSession));}
        private Task OnOwner(Action action)=>dispatcher.InvokeAsync(action);
        private void ObservePreview(long now)
        {if(now<lastNow)throw new ArgumentException("HUMAN_CLOCK_REVERSED");lastNow=now;if(previewAnchor.HasValue){previewAccumulated=checked(previewAccumulated+now-previewAnchor.Value);previewAnchor=now;}}
        private void Reconcile()
        {
            if(inner==null)return;long now=clock.MonotonicMilliseconds;ObservePreview(now);bool pause=ShouldPause;
            if(!Solved){var prior=inner.Revision;if(pause&&!inner.IsPaused)inner.Pause(now);else if(!pause&&inner.IsPaused)inner.Resume(now);if(inner.Revision!=prior)MarkDirty(now);}
            previewAnchor=!pause&&Current?.Exposure==HumanExposureState.EXPOSURE_CONFIRMED&&!inner.TimerStarted?now:(long?)null;
        }
        public void SetSuspension(HumanPauseReason reason,bool paused)
        {Owner();HumanGuard.Require(reason!=HumanPauseReason.None&&(reason&~(HumanPauseReason.External|HumanPauseReason.Presentation|HumanPauseReason.Instruction|HumanPauseReason.Hint))==0,"HUMAN_PAUSE_REASON_INVALID");reasons=paused?reasons|reason:reasons&~reason;Reconcile();Publish();}
        private void Publish()
        {
            var trial=Current;bool ready=!disposed&&!blocked&&!busy&&!operationBusy&&snapshot!=null;bool input=ready&&exposure!=null&&!ShouldPause&&!Solved&&trial?.Outcome==HumanTrialOutcome.IN_PROGRESS;
            bool terminal=trial?.Outcome==HumanTrialOutcome.ABORTED||trial?.Outcome==HumanTrialOutcome.SOLVED_UNREVIEWED&&(inner==null||inner.Phase==DevelopmentPreviewPhase.Results);
            if(!blocked&&!busy&&snapshot!=null)status=Assignment==null?HumanObservationStatus.Complete:exposure==null?HumanObservationStatus.NeedsExposure:trial?.Contact==null||trial.Exposure!=HumanExposureState.EXPOSURE_CONFIRMED?HumanObservationStatus.WaitingFirstContact:HumanObservationStatus.Ready;
            State=new AssignedHumanState(++version,Assignment,!blocked&&!disposed?exposure:null,trial,disposed?HumanObservationStatus.Disposed:status,error,input,ready&&Assignment!=null&&trial!=null&&snapshot!.Run.Trials.Count<256&&!Solved,ready&&exposure!=null&&trial?.Outcome==HumanTrialOutcome.SOLVED_UNREVIEWED&&!rideStarted&&(reasons&HumanPauseReason.External)==0,ready&&Assignment!=null&&terminal,HintCreditsUnsafe(),snapshot!=null,ready&&Assignment!=null&&trial?.Outcome==HumanTrialOutcome.IN_PROGRESS&&!Solved,ready&&Assignment==null&&(reasons&HumanPauseReason.External)==0);
            Changed?.Invoke(State);
        }
        private int HintCreditsUnsafe(){var identity=Current?.Identity;return snapshot==null||identity==null?0:Math.Max(0,3-snapshot.Run.Trials.Where(t=>t.Identity.Equals(identity)).Sum(t=>t.Hints.Count));}
        private void MarkDirty(long now){if(!dirty)firstDirty=now;dirty=true;lastDirty=now;}
        private HumanCalibrationRun ReplaceCurrent(HumanCalibrationRun run,HumanTrialRecord replacement)=>run.With(run.Trials.Select(t=>t.Checkpoint.AttemptId==replacement.Checkpoint.AttemptId?replacement:t).ToArray());
        private HumanTrialRecord CaptureCurrent()
        {var current=Current??throw new ArgumentException("HUMAN_TRIAL_REQUIRED");if(inner==null)return current;ObservePreview(clock.MonotonicMilliseconds);return current.Change(checkpoint:inner.CaptureHumanCheckpoint(clock.MonotonicMilliseconds),previewActiveMs:current.TimingComplete?previewAccumulated:(long?)null);}
        private async Task CommitAsync(Func<HumanCalibrationRun,HumanCalibrationRun> project,CancellationToken ct,Action? acknowledged=null)
        {
            await writer.WaitAsync(ct).ConfigureAwait(false);bool attempted=false;
            try
            {
                HumanObservationSnapshot? offer=null;long expected=0;
                await OnOwner(()=>{Owner();HumanGuard.Require(!blocked&&snapshot!=null,"HUMAN_OBSERVATION_NOT_READY");busy=true;status=HumanObservationStatus.Saving;Reconcile();expected=snapshot!.Generation;offer=new HumanObservationSnapshot(checked(expected+1),project(snapshot.Run));Publish();}).ConfigureAwait(false);
                attempted=true;var result=await repository.CommitAsync(offer!,expected,ct).ConfigureAwait(false);bool accepted=false;
                await OnOwner(()=>
                {
                    observationToken=result.ObservationToken;
                    if(result.Succeeded&&result.Snapshot!=null&&result.Snapshot.Generation==offer!.Generation&&ReferenceEquals(result.Snapshot.Run,offer.Run))
                    {snapshot=result.Snapshot;blocked=false;dirty=false;error="NONE";accepted=true;acknowledged?.Invoke();}
                    else{blocked=true;error=result.ErrorCode;status=HumanObservationStatus.RecoveryRequired;exposure=null;}
                    Publish();
                }).ConfigureAwait(false);
                if(!accepted)throw new InvalidOperationException(result.Succeeded?"HUMAN_COMMIT_BINDING_UNCONFIRMED":result.ErrorCode);
            }
            catch when(attempted){await OnOwner(()=>{blocked=true;status=HumanObservationStatus.RecoveryRequired;error="HUMAN_WRITE_OUTCOME_UNKNOWN";exposure=null;}).ConfigureAwait(false);throw;}
            finally{await OnOwner(()=>{busy=false;if(!disposed){Reconcile();Publish();}}).ConfigureAwait(false);writer.Release();}
        }
        private async Task InitializeCoreAsync(CancellationToken ct)
        {
            Owner();HumanGuard.Require(status==HumanObservationStatus.Loading&&snapshot==null,"HUMAN_ALREADY_INITIALIZED");HumanStoreResult loaded;
            try{loaded=await repository.LoadAsync(ct).ConfigureAwait(false);}catch{await Freeze("HUMAN_LOAD_OUTCOME_UNKNOWN").ConfigureAwait(false);throw;}
            await OnOwner(()=>{observationToken=loaded.ObservationToken;error=loaded.ErrorCode;if(loaded.Status==HumanStoreStatus.Missing){blocked=true;status=HumanObservationStatus.NeedsRun;}else if(loaded.Succeeded){snapshot=loaded.Snapshot;blocked=false;status=HumanObservationStatus.NeedsExposure;}else{blocked=true;status=loaded.Status==HumanStoreStatus.Unsupported?HumanObservationStatus.Unsupported:HumanObservationStatus.RecoveryRequired;}Publish();}).ConfigureAwait(false);
            if(loaded.Succeeded&&loaded.Snapshot!.Run.Trials.LastOrDefault()?.Outcome==HumanTrialOutcome.IN_PROGRESS)
                await CommitAsync(run=>ReplaceCurrent(run,Current!.AsInterrupted()),ct).ConfigureAwait(false);
        }
        private Task BeginRunCoreAsync(CancellationToken ct)
        {Owner();HumanGuard.Require(status==HumanObservationStatus.NeedsRun&&!busy,"HUMAN_NEW_RUN_NOT_ALLOWED");return StartRunCoreAsync(null,ct);}
        private Task BeginNextParticipantCoreAsync(CancellationToken ct)
        {Owner();HumanGuard.Require(State.CanBeginNextParticipant&&snapshot!=null&&snapshot.Run.NextAssignmentIndex==catalog.Assignments.Count,"HUMAN_NEXT_PARTICIPANT_NOT_ALLOWED");return StartRunCoreAsync(snapshot!.Run.SessionToken,ct);}
        private async Task StartRunCoreAsync(string? expectedPriorToken,CancellationToken ct)
        {
            busy=true;status=HumanObservationStatus.Saving;Publish();
            try
            {
                var run=new HumanCalibrationRun(catalog.ManifestBytesSha256,Guid.NewGuid().ToString("N"),origin,clock.UtcNow,0,Array.Empty<HumanTrialRecord>());var result=await repository.StartRunAsync(run,expectedPriorToken,ct).ConfigureAwait(false);bool accepted=false;
                await OnOwner(()=>{observationToken=result.ObservationToken;if(result.Status==HumanStoreStatus.Committed&&result.Snapshot?.Generation==1&&ReferenceEquals(result.Snapshot.Run,run)){snapshot=result.Snapshot;blocked=false;error="NONE";inner=null;exposure=null;rideStarted=false;dirty=false;previewAnchor=null;previewAccumulated=0;accepted=true;}else{blocked=true;status=HumanObservationStatus.RecoveryRequired;error=result.ErrorCode;}}).ConfigureAwait(false);
                if(!accepted)throw new InvalidOperationException(result.Succeeded?"HUMAN_RUN_START_BINDING_UNCONFIRMED":result.ErrorCode);
            }
            catch{await Freeze("HUMAN_RUN_START_OUTCOME_UNKNOWN").ConfigureAwait(false);throw;}
            finally{await OnOwner(()=>{busy=false;Publish();}).ConfigureAwait(false);}
        }
        private Task ReserveExposureCoreAsync(CancellationToken ct)
        {
            Owner();HumanGuard.Require(!blocked&&!busy&&Assignment!=null&&exposure==null,"HUMAN_EXPOSURE_NOT_ALLOWED");var assignment=Assignment!;var current=Current;Public240ReviewSession prepared;HumanTrialRecord trial;
            if(current?.Outcome==HumanTrialOutcome.INTERRUPTED)
            {prepared=new Public240ReviewSession(assignment.Puzzle,current.Checkpoint);trial=current.Change(exposure:HumanExposureState.EXPOSURE_PENDING,priorExposure:true,interrupted:true,clockEpoch:epoch,timingComplete:false,outcome:HumanTrialOutcome.IN_PROGRESS,clearTimes:true);}
            else
            {HumanGuard.Require(current==null,"HUMAN_EXISTING_TRIAL_REQUIRED");prepared=new Public240ReviewSession(assignment.Puzzle);trial=new HumanTrialRecord(assignment.Id,assignment.Puzzle.Identity,assignment.SourceSha256,Guid.NewGuid(),HumanExposureState.EXPOSURE_PENDING,null,false,snapshot!.Run.Trials.Any(t=>t.Identity.Equals(assignment.Puzzle.Identity)),false,epoch,null,null,true,HumanTrialOutcome.IN_PROGRESS,prepared.CaptureHumanCheckpoint(clock.MonotonicMilliseconds),Array.Empty<HumanHintObservation>());}
            return CommitAsync(run=>current==null?run.With(run.Trials.Concat(new[]{trial}).ToArray()):ReplaceCurrent(run,trial),ct,()=>{inner=prepared;rideStarted=false;previewAccumulated=0;previewAnchor=null;exposure=new HumanExposurePresentation(snapshot!.Run.SessionToken,catalog.ManifestBytesSha256,assignment.Id,assignment.Puzzle.Identity,trial.Checkpoint.AttemptId,trial.ExposureId,snapshot.Generation,epoch);});
        }
        private Task ConfirmExposureVisibleCoreAsync(HumanExposureReceipt receipt,CancellationToken ct)
        {
            Owner();var current=Current??throw new ArgumentException("HUMAN_TRIAL_REQUIRED");var p=receipt.Presentation;
            HumanGuard.Require(snapshot!=null&&p.SessionToken==snapshot.Run.SessionToken&&p.ManifestBytesSha256==catalog.ManifestBytesSha256&&p.AssignmentId==current.AssignmentId&&p.Identity.Equals(current.Identity)&&p.AttemptId==current.Checkpoint.AttemptId&&p.ExposureId==current.ExposureId&&p.Generation<=snapshot.Generation&&current.ClockEpoch==epoch&&p.ClockEpoch==epoch&&receipt.MonotonicMilliseconds<=clock.MonotonicMilliseconds,"HUMAN_VISIBLE_RECEIPT_STALE");
            if(current.Exposure==HumanExposureState.EXPOSURE_CONFIRMED)return Task.CompletedTask;
            return CommitAsync(run=>ReplaceCurrent(run,Current!.Change(exposure:HumanExposureState.EXPOSURE_CONFIRMED,visibleAtMs:receipt.MonotonicMilliseconds,previewActiveMs:Current.TimingComplete?0:(long?)null)),ct,()=>{lastNow=clock.MonotonicMilliseconds;previewAccumulated=0;});
        }
        private Task RecordFirstContactCoreAsync(HumanFirstContact contact,bool priorVersion,CancellationToken ct)
        {Owner();HumanGuard.Enum(contact);HumanGuard.Require(Current?.Exposure==HumanExposureState.EXPOSURE_CONFIRMED&&Current.Contact==null,"HUMAN_CONTACT_NOT_ALLOWED");return CommitAsync(run=>ReplaceCurrent(run,Current!.Change(contact:contact,priorVersion:priorVersion)),ct);}
        private async Task TickCoreAsync(CancellationToken ct)
        {
            Owner();if(blocked||busy||inner==null||snapshot==null||!dirty)return;long now=clock.MonotonicMilliseconds;
            if(!Solved&&now-lastDirty<500&&now-firstDirty<2000)return;
            await CommitAsync(run=>{var trial=CaptureCurrent();if(Solved)trial=trial.Change(outcome:HumanTrialOutcome.SOLVED_UNREVIEWED);return ReplaceCurrent(run,trial);},ct).ConfigureAwait(false);
        }
        private Task AbortCoreAsync(CancellationToken ct)
        {Owner();HumanGuard.Require(!blocked&&!busy&&Current?.Outcome==HumanTrialOutcome.IN_PROGRESS&&!Solved,"HUMAN_ABORT_NOT_ALLOWED");return CommitAsync(run=>ReplaceCurrent(run,CaptureCurrent().Change(outcome:HumanTrialOutcome.ABORTED)),ct,()=>{exposure=null;});}
        private Task NextCoreAsync(CancellationToken ct)
        {Owner();HumanGuard.Require(State.CanAdvance,"HUMAN_NEXT_NOT_ALLOWED");return CommitAsync(run=>run.With(run.Trials,run.NextAssignmentIndex+1),ct,()=>{inner=null;exposure=null;rideStarted=false;previewAnchor=null;});}
        private Task RestartCoreAsync(CancellationToken ct)
        {
            Owner();HumanGuard.Require(State.CanRestart&&Assignment!=null,"HUMAN_RESTART_NOT_ALLOWED");var assignment=Assignment!;var prepared=new Public240ReviewSession(assignment.Puzzle);HumanTrialRecord? replacement=null;
            return CommitAsync(run=>{var previous=CaptureCurrent().Change(outcome:HumanTrialOutcome.ABORTED);var trials=ReplaceCurrent(run,previous).Trials;replacement=new HumanTrialRecord(assignment.Id,assignment.Puzzle.Identity,assignment.SourceSha256,Guid.NewGuid(),HumanExposureState.EXPOSURE_PENDING,null,false,true,false,epoch,null,null,true,HumanTrialOutcome.IN_PROGRESS,prepared.CaptureHumanCheckpoint(clock.MonotonicMilliseconds),Array.Empty<HumanHintObservation>());return run.With(trials.Concat(new[]{replacement}).ToArray());},ct,()=>{inner=prepared;rideStarted=false;previewAccumulated=0;previewAnchor=null;exposure=new HumanExposurePresentation(snapshot!.Run.SessionToken,catalog.ManifestBytesSha256,assignment.Id,assignment.Puzzle.Identity,replacement!.Checkpoint.AttemptId,replacement.ExposureId,snapshot.Generation,epoch);});
        }
        private async Task RetryCoreAsync(CancellationToken ct)
        {
            Owner();HumanGuard.Require(!busy,"HUMAN_WRITER_PENDING");Reconcile();HumanStoreResult loaded;
            try{loaded=await repository.LoadAsync(ct).ConfigureAwait(false);if(!loaded.Succeeded&&loaded.ObservationToken!=null)loaded=await repository.RecoverAsync(loaded.ObservationToken,ct).ConfigureAwait(false);}catch{await Freeze("HUMAN_RELOAD_OUTCOME_UNKNOWN").ConfigureAwait(false);throw;}
            await OnOwner(()=>{observationToken=loaded.ObservationToken;exposure=null;inner=null;previewAnchor=null;error=loaded.ErrorCode;blocked=!loaded.Succeeded;if(loaded.Succeeded)snapshot=loaded.Snapshot;status=blocked?HumanObservationStatus.RecoveryRequired:HumanObservationStatus.NeedsExposure;Publish();}).ConfigureAwait(false);
            if(loaded.Succeeded&&Current?.Outcome==HumanTrialOutcome.IN_PROGRESS)await CommitAsync(run=>ReplaceCurrent(run,Current!.AsInterrupted()),ct).ConfigureAwait(false);
        }
        private async Task<SafePuzzleHint?> PrepareHintCoreAsync(CancellationToken ct)
        {
            Owner();HumanGuard.Require(!blocked&&!busy&&exposure!=null&&Current?.Exposure==HumanExposureState.EXPOSURE_CONFIRMED&&Current.Contact!=null&&!Current.Hints.Any(h=>h.State==HumanHintState.HINT_DISPLAY_PENDING)&&!Solved&&inner!=null&&(reasons&(HumanPauseReason.External|HumanPauseReason.Instruction))==0&&HintCredits>0,"HUMAN_HINT_NOT_ALLOWED");SetSuspension(HumanPauseReason.Hint,true);var checkpoint=inner!.CaptureHumanCheckpoint(clock.MonotonicMilliseconds);var puzzle=inner.Puzzle;
            var hint=await Task.Run(()=>SafePuzzleHints.Prepare(puzzle,checkpoint,ct),ct).ConfigureAwait(false);if(hint==null)return null;
            await CommitAsync(run=>{HumanGuard.Require(inner!.AttemptId==checkpoint.AttemptId&&inner.Revision==checkpoint.Revision,"HUMAN_HINT_CHANGED");var item=new HumanHintObservation(hint.ReservedDisplayId,hint.ReservedRevision,hint.X,hint.Y,hint.RuleCode,hint.Content,HumanHintState.HINT_DISPLAY_PENDING);return ReplaceCurrent(run,CaptureCurrent().Change(hints:Current!.Hints.Concat(new[]{item}).ToArray()));},ct).ConfigureAwait(false);return hint;
        }
        private Task CommitHintDisplayCoreAsync(SafeHintDisplayReceipt receipt,CancellationToken ct)
        {
            Owner();var current=Current??throw new ArgumentException("HUMAN_TRIAL_REQUIRED");var hint=current.Hints.SingleOrDefault(h=>h.DisplayId==receipt.DisplayId);
            HumanGuard.Require(hint!=null&&receipt.Identity.Equals(current.Identity)&&receipt.AttemptId==current.Checkpoint.AttemptId&&hint.ExpectedRevision==receipt.ExpectedRevision,"HUMAN_HINT_RECEIPT_STALE");
            if(hint!.State==HumanHintState.HINT_DISPLAY_CONFIRMED)return Task.CompletedTask;
            HumanGuard.Require(hint.State==HumanHintState.HINT_DISPLAY_PENDING&&inner!=null&&inner.AttemptId==receipt.AttemptId&&inner.Revision==receipt.ExpectedRevision,"HUMAN_HINT_RECEIPT_STALE");
            return CommitAsync(run=>{HumanGuard.Require(inner!.RecordHumanHint(receipt,clock.MonotonicMilliseconds)==DevelopmentCommandStatus.Applied,"HUMAN_HINT_REJECTED");var cp=inner.CaptureHumanCheckpoint(clock.MonotonicMilliseconds);return ReplaceCurrent(run,Current!.Change(checkpoint:cp,hints:Current.Hints.Select(h=>h.DisplayId==receipt.DisplayId?h.WithState(HumanHintState.HINT_DISPLAY_CONFIRMED):h).ToArray()));},ct);
        }
        private Task Freeze(string code)=>OnOwner(()=>{blocked=true;status=HumanObservationStatus.RecoveryRequired;error=code;exposure=null;previewAnchor=null;Publish();});
        private async Task<T> Serialized<T>(Func<Task<T>> action,CancellationToken ct)
        {
            Owner();await operations.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                Task<T>? pending=null;
                await OnOwner(()=>{Owner();operationBusy=true;pending=action();Publish();}).ConfigureAwait(false);
                return await pending!.ConfigureAwait(false);
            }
            finally{await OnOwner(()=>{operationBusy=false;if(!disposed){Reconcile();Publish();}}).ConfigureAwait(false);operations.Release();}
        }
        private Task Serialized(Func<Task> action,CancellationToken ct)=>Serialized(async()=>{await action().ConfigureAwait(false);return true;},ct);
        public Task InitializeAsync(CancellationToken ct)=>Serialized(()=>InitializeCoreAsync(ct),ct);
        public Task BeginRunAsync(CancellationToken ct)=>Serialized(()=>BeginRunCoreAsync(ct),ct);
        /// <summary>Only a separately confirmed UI action may request a new participant after this complete run.</summary>
        public Task BeginNextParticipantAsync(CancellationToken ct)=>Serialized(()=>BeginNextParticipantCoreAsync(ct),ct);
        public Task ReserveExposureAsync(CancellationToken ct)=>Serialized(()=>ReserveExposureCoreAsync(ct),ct);
        public Task ConfirmExposureVisibleAsync(HumanExposureReceipt receipt,CancellationToken ct)=>Serialized(()=>ConfirmExposureVisibleCoreAsync(receipt,ct),ct);
        public Task RecordFirstContactAsync(HumanFirstContact contact,bool priorVersion,CancellationToken ct)=>Serialized(()=>RecordFirstContactCoreAsync(contact,priorVersion,ct),ct);
        public Task TickAsync(CancellationToken ct)=>Serialized(()=>TickCoreAsync(ct),ct);
        public Task AbortAsync(CancellationToken ct)=>Serialized(()=>AbortCoreAsync(ct),ct);
        public Task NextAsync(CancellationToken ct)=>Serialized(()=>NextCoreAsync(ct),ct);
        public Task RestartAsync(CancellationToken ct)=>Serialized(()=>RestartCoreAsync(ct),ct);
        public Task RetryAsync(CancellationToken ct)=>Serialized(()=>RetryCoreAsync(ct),ct);
        public Task<SafePuzzleHint?> PrepareHintAsync(CancellationToken ct)=>Serialized(()=>PrepareHintCoreAsync(ct),ct);
        public Task CommitHintDisplayAsync(SafeHintDisplayReceipt receipt,CancellationToken ct)=>Serialized(()=>CommitHintDisplayCoreAsync(receipt,ct),ct);

        public DevelopmentCommandStatus BeginTrainRide(Guid expectedAttempt)
        {Owner();if(!State.CanStartRide||inner?.AttemptId!=expectedAttempt)return Reject("HUMAN_RIDE_NOT_READY");rideStarted=true;Publish();return Applied();}
        private Public240ReviewSession Visible{get{Owner();if(exposure==null||inner==null||blocked)throw new InvalidOperationException("HUMAN_INPUT_NOT_EXPOSED");return inner;}}
        public PublicPuzzleInput Puzzle=>Visible.Puzzle;public int Width=>Visible.Width;public int Height=>Visible.Height;public DevelopmentEndpoint EndpointA=>Visible.EndpointA;public DevelopmentEndpoint EndpointB=>Visible.EndpointB;
        public DevelopmentPreviewPhase Phase{get{Owner();if(inner==null||Current?.Outcome==HumanTrialOutcome.ABORTED)return DevelopmentPreviewPhase.Overview;if(Solved&&!rideStarted)return DevelopmentPreviewPhase.Editing;return inner.Phase;}}
        public DevelopmentCommandStatus LastCommand{get;private set;} public string LastErrorCode{get;private set;}="NONE";
        public Guid AttemptId=>Visible.AttemptId;public long Revision=>Visible.Revision;public bool TimerStarted=>Visible.TimerStarted;public bool IsPaused=>Visible.IsPaused||ShouldPause;public bool CanUndo=>State.CanInput&&Visible.CanUndo;public bool HasObjectiveViolations=>Visible.HasObjectiveViolations;public IReadOnlyList<DevelopmentCell> BuiltPath=>Visible.BuiltPath;
        public DevelopmentTool CellAt(int x,int y)=>Visible.CellAt(x,y);public int RowTarget(int y)=>Visible.RowTarget(y);public int ColumnTarget(int x)=>Visible.ColumnTarget(x);public int RowVisible(int y)=>Visible.RowVisible(y);public int ColumnVisible(int x)=>Visible.ColumnVisible(x);public long ElapsedAt(long time)=>Visible.ElapsedAt(time);public DevelopmentCommandEnvelope CaptureCommand()=>Visible.CaptureCommand();
        private DevelopmentCommandStatus Command(Func<Public240ReviewSession,DevelopmentCommandStatus> apply,long time)
        {Owner();if(!State.CanInput)return Reject("HUMAN_INPUT_BLOCKED");if(time<lastNow||time>clock.MonotonicMilliseconds)return Reject("HUMAN_CLOCK_INVALID");ObservePreview(time);var result=apply(Visible);if(result==DevelopmentCommandStatus.Applied){MarkDirty(time);if(inner!.TimerStarted)previewAnchor=null;Publish();}LastCommand=result;LastErrorCode=inner!.LastErrorCode;return result;}
        public DevelopmentCommandStatus Apply(int x,int y,DevelopmentTool tool,long time)=>Apply(x,y,tool,time,CaptureCommand());
        public DevelopmentCommandStatus Apply(int x,int y,DevelopmentTool tool,long time,DevelopmentCommandEnvelope envelope)=>Command(s=>s.Apply(x,y,tool,time,envelope),time);
        public DevelopmentCommandStatus ApplyBatch(IReadOnlyList<DevelopmentCell>? cells,DevelopmentTool tool,long time)=>ApplyBatch(cells,tool,time,CaptureCommand());
        public DevelopmentCommandStatus ApplyBatch(IReadOnlyList<DevelopmentCell>? cells,DevelopmentTool tool,long time,DevelopmentCommandEnvelope envelope)=>Command(s=>s.ApplyBatch(cells,tool,time,envelope),time);
        public DevelopmentCommandStatus Undo(long time)=>Command(s=>s.Undo(time),time);
        public DevelopmentCommandStatus Pause(long time){Owner();SetSuspension(HumanPauseReason.Presentation,true);return Applied();}
        public DevelopmentCommandStatus Resume(long time){Owner();SetSuspension(HumanPauseReason.Presentation,false);return ShouldPause?Reject("HUMAN_OTHER_SUSPENSION_ACTIVE"):Applied();}
        public DevelopmentCommandStatus Restart()=>Reject("HUMAN_ASYNC_RESTART_REQUIRED");
        public DevelopmentCommandStatus CompleteTrainRide(Guid expectedAttempt){Owner();if(!rideStarted||busy||blocked||(reasons&HumanPauseReason.External)!=0||inner?.AttemptId!=expectedAttempt)return Reject("HUMAN_RIDE_NOT_ACTIVE");var result=inner.CompleteTrainRide(expectedAttempt);Publish();return result;}
        private DevelopmentCommandStatus Reject(string code){LastErrorCode=code;return LastCommand=DevelopmentCommandStatus.Rejected;}
        private DevelopmentCommandStatus Applied(){LastErrorCode="NONE";return LastCommand=DevelopmentCommandStatus.Applied;}
        public void Dispose(){if(disposed)return;Owner();disposed=true;blocked=true;previewAnchor=null;Publish();}
    }
}
