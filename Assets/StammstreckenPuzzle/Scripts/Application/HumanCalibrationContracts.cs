using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Puzzle.Domain;

namespace STP.Application
{
    public enum HumanAssignmentPurpose { CAMPAIGN_TIME, ENDLESS_PROFILE }
    public enum HumanRunOrigin { PARTICIPANT_UI, AUTOMATED_TEST }
    public enum HumanFirstContact { NEVER_SEEN, SEEN_OR_PLAYED, UNSURE }
    public enum HumanExposureState { EXPOSURE_PENDING, EXPOSURE_CONFIRMED, UNCERTAIN }
    public enum HumanHintState { HINT_DISPLAY_PENDING, HINT_DISPLAY_CONFIRMED, UNCERTAIN }
    public enum HumanTrialOutcome { IN_PROGRESS, SOLVED_UNREVIEWED, ABORTED, INTERRUPTED }
    public enum HumanObservationStatus { Loading, NeedsRun, NeedsExposure, WaitingFirstContact, Ready, Saving, RecoveryRequired, Unsupported, Complete, Disposed }
    public enum HumanStoreStatus { Ready, Missing, Committed, Conflict, RecoveryRequired, Unsupported, Corrupt, Failed, Cancelled }
    [Flags] public enum HumanPauseReason { None=0, External=1, Presentation=2, Instruction=4, Hint=8 }

    public sealed class HumanAssignment
    {
        public HumanAssignment(string id,HumanAssignmentPurpose purpose,PublicPuzzleInput puzzle,string publicSourceSha256,string sourceSha256,int? contentRevision,string? profileId,string? profileHashSha256)
        {
            Id=HumanGuard.Id(id);Purpose=HumanGuard.Enum(purpose);Puzzle=puzzle??throw new ArgumentNullException(nameof(puzzle));PublicSourceSha256=LocalModel.Hash(publicSourceSha256);SourceSha256=LocalModel.Hash(sourceSha256);
            if(purpose==HumanAssignmentPurpose.CAMPAIGN_TIME)HumanGuard.Require(contentRevision>0&&profileId==null&&profileHashSha256==null,"HUMAN_CAMPAIGN_SOURCE_INVALID");
            else HumanGuard.Require(contentRevision==null&&new[]{"dienstbeginn","regelbetrieb","stoerungslage","grosslage"}.Contains(profileId)&&profileHashSha256!=null&&puzzle.Identity.PuzzleId.StartsWith("E1-",StringComparison.Ordinal),"HUMAN_ENDLESS_SOURCE_INVALID");
            ContentRevision=contentRevision;ProfileId=profileId;ProfileHashSha256=profileHashSha256==null?null:LocalModel.Hash(profileHashSha256);
        }
        public string Id{get;} public HumanAssignmentPurpose Purpose{get;} public PublicPuzzleInput Puzzle{get;} public string PublicSourceSha256{get;} public string SourceSha256{get;} public int? ContentRevision{get;} public string? ProfileId{get;} public string? ProfileHashSha256{get;}
    }
    public interface IHumanAssignmentCatalog
    {
        string ManifestBytesSha256{get;} string StudyId{get;} int Revision{get;} string BuildId{get;} string UiSourceSha256{get;} string PublicCatalogHashSha256{get;} IReadOnlyList<HumanAssignment> Assignments{get;}
    }
    public sealed class HumanHintObservation
    {
        public HumanHintObservation(Guid displayId,long expectedRevision,int x,int y,string ruleCode,CellContent content,HumanHintState state)
        {HumanGuard.Require(displayId!=Guid.Empty&&x>=0&&x<32&&y>=0&&y<32&&TrackGeometry.IsContent(content),"HUMAN_HINT_INVALID");LocalModel.Safe(expectedRevision);DisplayId=displayId;ExpectedRevision=expectedRevision;X=x;Y=y;RuleCode=LocalModel.Text(ruleCode);Content=content;State=HumanGuard.Enum(state);}
        public Guid DisplayId{get;} public long ExpectedRevision{get;} public int X{get;} public int Y{get;} public string RuleCode{get;} public CellContent Content{get;} public HumanHintState State{get;}
        internal HumanHintObservation WithState(HumanHintState value)=>new HumanHintObservation(DisplayId,ExpectedRevision,X,Y,RuleCode,Content,value);
    }
    public sealed class HumanTrialRecord
    {
        public HumanTrialRecord(string assignmentId,PublicPuzzleIdentity identity,string sourceSha256,Guid exposureId,HumanExposureState exposure,HumanFirstContact? contact,bool priorVersion,bool priorExposure,bool interrupted,Guid clockEpoch,long? visibleAtMs,long? previewActiveMs,bool timingComplete,HumanTrialOutcome outcome,PuzzleSessionCheckpoint checkpoint,IReadOnlyList<HumanHintObservation> hints)
        {
            AssignmentId=HumanGuard.Id(assignmentId);Identity=identity??throw new ArgumentNullException(nameof(identity));SourceSha256=LocalModel.Hash(sourceSha256);HumanGuard.Require(exposureId!=Guid.Empty&&clockEpoch!=Guid.Empty,"HUMAN_EXPOSURE_INVALID");ExposureId=exposureId;Exposure=HumanGuard.Enum(exposure);if(contact.HasValue)HumanGuard.Enum(contact.Value);Contact=contact;PriorVersion=priorVersion;PriorExposure=priorExposure;Interrupted=interrupted;ClockEpoch=clockEpoch;
            if(visibleAtMs.HasValue)LocalModel.Safe(visibleAtMs.Value);if(previewActiveMs.HasValue)LocalModel.Safe(previewActiveMs.Value);VisibleAtMs=visibleAtMs;PreviewActiveMs=previewActiveMs;TimingComplete=timingComplete;
            Outcome=HumanGuard.Enum(outcome);Checkpoint=checkpoint??throw new ArgumentNullException(nameof(checkpoint));HumanGuard.Require(checkpoint.PuzzleId==identity.PuzzleId&&checkpoint.PublicPuzzleHashProfile==identity.HashProfile&&checkpoint.PublicPuzzleHashSha256==identity.HashSha256&&checkpoint.Mode==SessionMode.PRACTICE,"HUMAN_CHECKPOINT_BINDING");
            Hints=LocalModel.Copy(hints);HumanGuard.Require(Hints.Count<=3,"HUMAN_HINT_LIMIT");LocalModel.Unique(Hints.Select(h=>h.DisplayId.ToString("D")));HumanGuard.Require(checkpoint.HintCount==Hints.Count(h=>h.State==HumanHintState.HINT_DISPLAY_CONFIRMED),"HUMAN_HINT_COUNT_INVALID");
            HumanGuard.Require(outcome!=HumanTrialOutcome.SOLVED_UNREVIEWED||checkpoint.Phase==SessionPhase.SolvedPendingCommit,"HUMAN_COMPLETION_NOT_SOLVED");HumanGuard.Require(!timingComplete||!interrupted,"HUMAN_TIME_COMPLETENESS_INVALID");
        }
        public string AssignmentId{get;} public PublicPuzzleIdentity Identity{get;} public string SourceSha256{get;} public Guid ExposureId{get;} public HumanExposureState Exposure{get;} public HumanFirstContact? Contact{get;} public bool PriorVersion{get;} public bool PriorExposure{get;} public bool Interrupted{get;} public Guid ClockEpoch{get;} public long? VisibleAtMs{get;} public long? PreviewActiveMs{get;} public bool TimingComplete{get;} public HumanTrialOutcome Outcome{get;} public PuzzleSessionCheckpoint Checkpoint{get;} public IReadOnlyList<HumanHintObservation> Hints{get;}
        public bool EligibleFirstContact=>Contact==HumanFirstContact.NEVER_SEEN&&!PriorVersion&&!PriorExposure&&!Interrupted;
        public bool PossiblyAssisted=>Hints.Count>0;
        internal HumanTrialRecord Change(PuzzleSessionCheckpoint? checkpoint=null,HumanExposureState? exposure=null,HumanFirstContact? contact=null,bool? priorVersion=null,bool? priorExposure=null,bool? interrupted=null,Guid? clockEpoch=null,long? visibleAtMs=null,long? previewActiveMs=null,bool? timingComplete=null,HumanTrialOutcome? outcome=null,IReadOnlyList<HumanHintObservation>? hints=null,bool clearTimes=false)
            =>new HumanTrialRecord(AssignmentId,Identity,SourceSha256,ExposureId,exposure??Exposure,contact??Contact,priorVersion??PriorVersion,priorExposure??PriorExposure,interrupted??Interrupted,clockEpoch??ClockEpoch,clearTimes?null:visibleAtMs??VisibleAtMs,clearTimes?null:previewActiveMs??PreviewActiveMs,timingComplete??TimingComplete,outcome??Outcome,checkpoint??Checkpoint,hints??Hints);
        internal HumanTrialRecord AsInterrupted()=>Change(exposure:Exposure==HumanExposureState.EXPOSURE_PENDING?HumanExposureState.UNCERTAIN:Exposure,priorExposure:true,interrupted:true,timingComplete:false,outcome:HumanTrialOutcome.INTERRUPTED,hints:Hints.Select(h=>h.State==HumanHintState.HINT_DISPLAY_PENDING?h.WithState(HumanHintState.UNCERTAIN):h).ToArray(),clearTimes:true);
    }
    public sealed class HumanCalibrationRun
    {
        public HumanCalibrationRun(string manifestBytesSha256,string sessionToken,HumanRunOrigin origin,string createdAtUtc,int nextAssignmentIndex,IReadOnlyList<HumanTrialRecord> trials)
        {ManifestBytesSha256=LocalModel.Hash(manifestBytesSha256);HumanGuard.Require(Guid.TryParseExact(sessionToken,"N",out var token)&&token!=Guid.Empty,"HUMAN_RUN_TOKEN_INVALID");SessionToken=sessionToken;Origin=HumanGuard.Enum(origin);CreatedAtUtc=LocalModel.Utc(createdAtUtc);HumanGuard.Require(nextAssignmentIndex>=0&&nextAssignmentIndex<=128,"HUMAN_ASSIGNMENT_INDEX_INVALID");NextAssignmentIndex=nextAssignmentIndex;Trials=LocalModel.Copy(trials);HumanGuard.Require(Trials.Count<=256&&Trials.Select(t=>t.AssignmentId).Distinct().Count()<=128&&Trials.Count(t=>t.Outcome==HumanTrialOutcome.IN_PROGRESS)<=1,"HUMAN_TRIAL_LIMIT");LocalModel.Unique(Trials.Select(t=>t.Checkpoint.AttemptId.ToString("D")));}
        public string ManifestBytesSha256{get;} public string SessionToken{get;} public HumanRunOrigin Origin{get;} public string CreatedAtUtc{get;} public int NextAssignmentIndex{get;} public IReadOnlyList<HumanTrialRecord> Trials{get;}
        internal HumanCalibrationRun With(IReadOnlyList<HumanTrialRecord> trials,int? index=null)=>new HumanCalibrationRun(ManifestBytesSha256,SessionToken,Origin,CreatedAtUtc,index??NextAssignmentIndex,trials);
    }
    public sealed class HumanObservationSnapshot
    {
        public HumanObservationSnapshot(long generation,HumanCalibrationRun run){LocalModel.Positive(generation);Generation=generation;Run=run??throw new ArgumentNullException(nameof(run));}
        public long Generation{get;} public HumanCalibrationRun Run{get;}
    }
    public sealed class HumanStoreResult
    {
        public HumanStoreResult(HumanStoreStatus status,string errorCode,HumanObservationSnapshot? snapshot=null,string? observationToken=null){Status=HumanGuard.Enum(status);ErrorCode=LocalModel.Text(errorCode);Snapshot=snapshot;ObservationToken=observationToken;HumanGuard.Require(status!=HumanStoreStatus.Ready&&status!=HumanStoreStatus.Committed||snapshot!=null,"HUMAN_SNAPSHOT_REQUIRED");}
        public HumanStoreStatus Status{get;} public string ErrorCode{get;} public HumanObservationSnapshot? Snapshot{get;} public string? ObservationToken{get;} public bool Succeeded=>Status==HumanStoreStatus.Ready||Status==HumanStoreStatus.Committed;
    }
    public interface IHumanCalibrationRepository
    {
        Task<HumanStoreResult> LoadAsync(CancellationToken cancellation);
        Task<HumanStoreResult> StartRunAsync(HumanCalibrationRun run,string? expectedPriorToken,CancellationToken cancellation);
        Task<HumanStoreResult> CommitAsync(HumanObservationSnapshot snapshot,long expectedGeneration,CancellationToken cancellation);
        Task<HumanStoreResult> RecoverAsync(string observationToken,CancellationToken cancellation);
        Task<byte[]> ExportAsync(CancellationToken cancellation);
    }
    public sealed class HumanExposurePresentation
    {
        internal HumanExposurePresentation(string token,string manifest,string assignment,PublicPuzzleIdentity puzzle,Guid attempt,Guid exposure,long generation,Guid clockEpoch)
        {SessionToken=token;ManifestBytesSha256=manifest;AssignmentId=assignment;Identity=puzzle;AttemptId=attempt;ExposureId=exposure;Generation=generation;ClockEpoch=clockEpoch;}
        public string SessionToken{get;} public string ManifestBytesSha256{get;} public string AssignmentId{get;} public PublicPuzzleIdentity Identity{get;} public Guid AttemptId{get;} public Guid ExposureId{get;} public long Generation{get;} public Guid ClockEpoch{get;}
        public HumanExposureReceipt AcknowledgeVisible(long monotonicMilliseconds)=>new HumanExposureReceipt(this,monotonicMilliseconds);
    }
    public sealed class HumanExposureReceipt
    {
        internal HumanExposureReceipt(HumanExposurePresentation presentation,long now){LocalModel.Safe(now);Presentation=presentation;MonotonicMilliseconds=now;}
        public HumanExposurePresentation Presentation{get;} public long MonotonicMilliseconds{get;}
    }
    public sealed class AssignedHumanState
    {
        internal AssignedHumanState(long version,HumanAssignment? assignment,HumanExposurePresentation? exposure,HumanTrialRecord? trial,HumanObservationStatus status,string error,bool input,bool restart,bool ride,bool advance,int credits,bool hasRun,bool abort,bool nextParticipant)
        {Version=version;Assignment=assignment;Exposure=exposure;Trial=trial;ObservationStatus=status;ErrorCode=error;CanInput=input;CanRestart=restart;CanStartRide=ride;CanAdvance=advance;HintCredits=credits;HasRun=hasRun;CanAbort=abort;CanBeginNextParticipant=nextParticipant;}
        public long Version{get;} public HumanAssignment? Assignment{get;} public HumanExposurePresentation? Exposure{get;} public HumanTrialRecord? Trial{get;} public bool ExposureReady=>Exposure!=null; public bool PriorExposure=>Trial?.PriorExposure??false; public bool FirstContactRecorded=>Trial?.Contact!=null; public bool CanInput{get;} public bool CanRestart{get;} public bool CanStartRide{get;} public bool CanAdvance{get;} public bool CanAbort{get;} public bool CanBeginNextParticipant{get;} public int HintCredits{get;} public HumanObservationStatus ObservationStatus{get;} public string ErrorCode{get;} public long ActiveElapsedMs=>Trial?.Checkpoint.ActiveElapsedMs??0; public long? PreviewActiveMs=>Trial?.PreviewActiveMs; public bool TimingComplete=>Trial?.TimingComplete??false; public bool HasRun{get;} public bool IsFinished=>ObservationStatus==HumanObservationStatus.Complete; public bool IsRecoveryRequired=>ObservationStatus==HumanObservationStatus.RecoveryRequired;
    }
    internal static class HumanGuard
    {
        internal static void Require(bool condition,string code){if(!condition)throw new ArgumentException(code);}
        internal static string Id(string value){Require(value!=null&&System.Text.RegularExpressions.Regex.IsMatch(value,"^[a-z][a-z0-9-]{0,63}$"),"HUMAN_ID_INVALID");return value!;}
        internal static T Enum<T>(T value)where T:struct{Require(System.Enum.IsDefined(typeof(T),value),"HUMAN_ENUM_INVALID");return value;}
    }
}
