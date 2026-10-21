using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Puzzle.Domain;

namespace STP.Application
{
    /// <summary>Closed saved-QA presentation states; none substitutes for repository acknowledgement.</summary>
    public enum SavedReviewStatus
    {
        /// <summary>A fresh authoritative load is running.</summary>
        Loading,
        /// <summary>Three missing candidates require an explicit create request.</summary>
        NeedsInitialization,
        /// <summary>Confirmed profile; a puzzle may still be deliberately unselected.</summary>
        Ready,
        /// <summary>An observed draft or profile write is running.</summary>
        Saving,
        /// <summary>The actual solved attempt awaits its completion acknowledgement.</summary>
        CompletionPending,
        /// <summary>This exact attempt has a verified committed completion.</summary>
        ProgressCommitted,
        /// <summary>The explicitly acknowledged player's ride is being presented.</summary>
        TrainRide,
        /// <summary>The same acknowledged ride has finished.</summary>
        Results,
        /// <summary>Fresh observation and explicit recovery are required.</summary>
        RecoveryRequired,
        /// <summary>An unsupported source is protected from mutation.</summary>
        Unsupported,
        /// <summary>A named failure requires an explicit action.</summary>
        Failed,
        /// <summary>The view owner has ended its lifetime.</summary>
        Disposed
    }

    /// <summary>Display-only projection from one exact, confirmed and validated S snapshot.</summary>
    public sealed class SavedReviewProgress
    {
        internal SavedReviewProgress(SaveSnapshot<LocalProfileState> snapshot, IReadOnlyList<PublicPuzzleInput> puzzles, PublicPuzzleIdentity? selected,int? committedStars=null)
        {
            Profile = snapshot.Profile; Catalog = snapshot.Catalog; Generation = snapshot.Generation;
            BalancePatience = checked(snapshot.Payload.LedgerCheckpoint.BalancePatience + snapshot.Payload.EconomyJournal.Sum(x => x.Amount));
            MemberCount = puzzles.Count;
            FirstClearCount = puzzles.Count(p => snapshot.Payload.Levels.Any(l => l.Identity.Equals(p.Identity) && l.FirstClear != null));
            SelectedIdentity = selected;
            SelectedStars = committedStars ?? (selected == null ? null : snapshot.Payload.Levels.SingleOrDefault(l => l.Identity.Equals(selected))?.StarsHighWater);
        }
        /// <summary>Confirmed profile binding.</summary>
        public SaveProfileIdentity Profile { get; }
        /// <summary>Confirmed aggregate catalog binding.</summary>
        public CatalogIdentity Catalog { get; }
        /// <summary>Confirmed repository generation.</summary>
        public long Generation { get; }
        /// <summary>Confirmed QA balance, never a proposed completion balance.</summary>
        public long BalancePatience { get; }
        /// <summary>Confirmed first clears among current public members.</summary>
        public int FirstClearCount { get; }
        /// <summary>Actual public catalog size.</summary>
        public int MemberCount { get; }
        /// <summary>Actually started puzzle, or null before deliberate selection.</summary>
        public PublicPuzzleIdentity? SelectedIdentity { get; }
        /// <summary>Confirmed stars of the selected identity, or null when not yet recorded.</summary>
        public int? SelectedStars { get; }
    }

    /// <summary>Immutable UI state; controls must use these current gates rather than task completion.</summary>
    public sealed class SavedReviewState
    {
        internal SavedReviewState(long version, SavedReviewStatus status, SaveLoadStatus? load, SaveCommitStatus? commit, string error,
            bool mutate, bool navigate, bool restart, bool retryLoad, bool retrySave, bool create, bool recover, bool reset,
            bool ride, bool dirty, int index, PublicPuzzleIdentity? identity, bool hasDraft, SavedReviewProgress? progress,
            IReadOnlyList<SaveCandidateProvenance> candidates)
        {
            Version = version; Status = status; LoadStatus = load; CommitStatus = commit; ErrorCode = error;
            CanMutate = mutate; CanNavigate = navigate; CanRestart = restart; CanRetryLoad = retryLoad; CanRetrySave = retrySave;
            CanCreateProfile = create; CanRecover = recover; CanReset = reset; CanStartRide = ride; IsDirty = dirty;
            SelectedIndex = index; CurrentIdentity = identity; HasDraftForSelection = hasDraft; Progress = progress;
            RecoveryChoices = Array.AsReadOnly(candidates.ToArray());
        }
        /// <summary>Monotonic process-local display version.</summary>
        public long Version { get; }
        /// <summary>Saved presentation status.</summary>
        public SavedReviewStatus Status { get; }
        /// <summary>Most recently observed load outcome, if any.</summary>
        public SaveLoadStatus? LoadStatus { get; }
        /// <summary>Most recently observed write outcome, if any.</summary>
        public SaveCommitStatus? CommitStatus { get; }
        /// <summary>Stable technical identifier; localization belongs to the presenter.</summary>
        public string ErrorCode { get; }
        /// <summary>Current actual domain mutation gate.</summary>
        public bool CanMutate { get; }
        /// <summary>Allows explicit open/resume after required serialized flush.</summary>
        public bool CanNavigate { get; }
        /// <summary>Allows explicit asynchronous restart.</summary>
        public bool CanRestart { get; }
        /// <summary>Allows a deliberate fresh load, also after a completed request fault.</summary>
        public bool CanRetryLoad { get; }
        /// <summary>Allows explicit retry of a known noncommitting failure.</summary>
        public bool CanRetrySave { get; }
        /// <summary>Allows explicit generation-one profile creation.</summary>
        public bool CanCreateProfile { get; }
        /// <summary>Allows explicit selection among valid, fully observed candidates.</summary>
        public bool CanRecover { get; }
        /// <summary>Allows separately confirmed corrupt reset; unsupported/failed sources remain protected.</summary>
        public bool CanReset { get; }
        /// <summary>Only the exact acknowledged current attempt may begin a ride.</summary>
        public bool CanStartRide { get; }
        /// <summary>A notified mutation has not yet been acknowledged.</summary>
        public bool IsDirty { get; }
        /// <summary>Actually started catalog index; -1 before deliberate start/restore.</summary>
        public int SelectedIndex { get; }
        /// <summary>Actually started identity; never the presenter's tentative dropdown value.</summary>
        public PublicPuzzleIdentity? CurrentIdentity { get; }
        /// <summary>Confirmed exact draft availability for the actual selection.</summary>
        public bool HasDraftForSelection { get; }
        /// <summary>Last confirmed projection, absent when authority is uncertain.</summary>
        public SavedReviewProgress? Progress { get; }
        /// <summary>Defensively copied current observation, never arbitrary cached recovery choices.</summary>
        public IReadOnlyList<SaveCandidateProvenance> RecoveryChoices { get; }
    }

    /// <summary>Owner-thread saved public-QA bridge over the existing S use case; it owns no second domain or ledger truth.</summary>
    public sealed partial class SavedPublic240ReviewSession : IDisposable
    {
        private enum Request { Load, Create, Select, Restart, RetrySave, Complete, Recover, Reset, Tick, Suspend }
        private readonly PersistedPuzzleSession persisted;
        private AutosaveCoordinator autosave;
        private readonly ILocalSessionClock clock;
        private readonly ILocalSaveDispatcher dispatcher;
        private readonly int owner = Thread.CurrentThread.ManagedThreadId;
        private readonly CancellationTokenSource lifetime = new CancellationTokenSource();
        private readonly IReadOnlyList<PublicPuzzleInput> puzzles;
        private readonly CatalogIdentity publicCatalogIdentity;
        private readonly SaveProfileIdentity expectedProfile;
        private BoundView? view;
        private bool presentationPaused,presentationReconcilePending;
        private long? presentationPauseAt;
        private bool disposed, busy, exclusive, suspended, writeFailure, requestFault, completionAttempted;
        private bool authority;
        private TaskCompletionSource<bool>? requestIdle;
        private int selected = -1;
        private long version;
        private SaveLoadStatus? lastLoad;
        private SaveCommitStatus? lastCommit;
        private string error = "NONE";
        private SavedReviewStatus status = SavedReviewStatus.Loading;
        private SaveSnapshot<LocalProfileState>? projectedSnapshot;
        private PublicPuzzleIdentity? projectedIdentity;
        private SavedReviewProgress? progress;

        /// <summary>Captures Unity main as logical owner after worker composition has returned these provider-free objects.</summary>
        public SavedPublic240ReviewSession(PersistedPuzzleSession persisted, AutosaveCoordinator autosave, IPublicPuzzleCatalog catalog,
            ILocalSessionClock clock, ILocalSaveDispatcher dispatcher)
            : this(persisted,autosave,catalog,clock,dispatcher,new SaveProfileIdentity(SaveProfileIdentity.Public240ReviewNamespace,"r",CatalogUse.IsolatedReview)) { }
        private SavedPublic240ReviewSession(PersistedPuzzleSession persisted, AutosaveCoordinator autosave, IPublicPuzzleCatalog catalog,
            ILocalSessionClock clock, ILocalSaveDispatcher dispatcher,SaveProfileIdentity expectedProfile)
        {
            this.expectedProfile=expectedProfile;
            this.persisted = persisted ?? throw new ArgumentNullException(nameof(persisted));
            this.autosave = autosave ?? throw new ArgumentNullException(nameof(autosave));
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (catalog.Identity.Use != expectedProfile.Use || catalog.Identity.ApprovalStatus != CatalogApprovalStatus.Draft)
                throw new ArgumentException("SAVE_REVIEW_CATALOG_REQUIRED", nameof(catalog));
            puzzles = Array.AsReadOnly(catalog.Puzzles.ToArray());
            publicCatalogIdentity = catalog.Identity;
            if (puzzles.Count == 0 || puzzles.Select(x => x.Identity.PuzzleId).Distinct(StringComparer.Ordinal).Count() != puzzles.Count)
                throw new ArgumentException("SAVE_REVIEW_CATALOG_INVALID", nameof(catalog));
            State = null!; Publish();
        }
        /// <summary>Explicit ADR039 entry; the historical public constructor remains isolated to Public240 v2.</summary>
        public static SavedPublic240ReviewSession ForMobileCandidate(PersistedPuzzleSession persisted,AutosaveCoordinator autosave,IPublicPuzzleCatalog catalog,ILocalSessionClock clock,ILocalSaveDispatcher dispatcher,SaveProfileIdentity profile)
        {
            if(profile==null||profile.Use!=CatalogUse.InternalPreview||profile.Namespace!=SaveProfileIdentity.MobileCandidateNamespace)throw new ArgumentException("CANDIDATE_PROFILE_REQUIRED");
            return new SavedPublic240ReviewSession(persisted,autosave,catalog,clock,dispatcher,profile);
        }
        /// <summary>Attempt-bound read/command view; null until the user explicitly opens or resumes a puzzle.</summary>
        public IReviewPuzzleSession? PuzzleView => view;
        /// <summary>Copied immutable public inputs in catalog order.</summary>
        public IReadOnlyList<PublicPuzzleInput> Puzzles => puzzles;
        /// <summary>Current immutable display snapshot, published on the captured owner.</summary>
        public SavedReviewState State { get; private set; }
        /// <summary>Current first-clear-based map access, including the separate candidate's linear graph.</summary>
        public bool CanEnterPuzzle(int index){RequireOwner();return !disposed&&authority&&!requestFault&&index>=0&&index<puzzles.Count&&persisted.CanEnter(puzzles[index].Identity);}
        /// <summary>Pauses and flushes before deriving one safe clue; opening help never starts the timer or spends a credit.</summary>
        public async Task<SafePuzzleHint?> PrepareHintAsync(LocalMetagamePolicy hints,CancellationToken cancellationToken)
        {
            RequireOwner();if(expectedProfile.Use!=CatalogUse.InternalPreview||view==null||busy||!authority)throw new ArgumentException("HINT_CANDIDATE_NOT_READY");
            await SetSuspendedAsync(true,cancellationToken).ConfigureAwait(false);
            return await persisted.PrepareHintAsync(hints,cancellationToken).ConfigureAwait(false);
        }
        /// <summary>Commits and refreshes the cached command view before a later Resume captures its next revision.</summary>
        public Task<SaveCommitResult<LocalProfileState>> CommitHintDisplayAsync(LocalMetagamePolicy hints,SafeHintDisplayReceipt receipt,CancellationToken cancellationToken)
        {
            RequireOwner();if(expectedProfile.Use!=CatalogUse.InternalPreview||!suspended||view==null||!authority)throw new ArgumentException("HINT_CANDIDATE_NOT_READY");
            return RunProfileActionAsync((use,token)=>use.CommitHintDisplayAsync(hints,receipt,token),cancellationToken);
        }
        /// <summary>Read-only exact identity lookup from the confirmed validated snapshot; never creates a draft.</summary>
        public bool HasDraftForPuzzle(int index)
        {
            RequireOwner();
            return !disposed && authority && !requestFault && index >= 0 && index < puzzles.Count && persisted.Snapshot != null &&
                persisted.Snapshot.Payload.Drafts.Any(d => d.Identity.Equals(puzzles[index].Identity));
        }
        /// <summary>Performs the initial fresh load without selecting a puzzle or initializing missing files.</summary>
        public Task InitializeAsync(CancellationToken cancellationToken) => RunAsync(Request.Load, -1, false, SaveCandidateKind.Main, cancellationToken);
        /// <summary>Deliberately creates a fileless QA profile.</summary>
        public Task CreateProfileAsync(CancellationToken cancellationToken) => RunAsync(Request.Create, -1, false, SaveCandidateKind.Main, cancellationToken);
        /// <summary>Flushes the prior attempt, then explicitly starts or resumes the captured index; no missing-draft fallback.</summary>
        public Task SelectPuzzleAsync(int index, bool restoreExistingDraft, CancellationToken cancellationToken) => RunAsync(Request.Select, index, restoreExistingDraft, SaveCandidateKind.Main, cancellationToken);
        /// <summary>Creates and immediately saves a fresh Ready attempt of the actual current puzzle.</summary>
        public Task RestartAsync(CancellationToken cancellationToken) => persisted.Session?.Mode==STP.Puzzle.Domain.SessionMode.ENDLESS?RestartCurrentEndlessAsync(cancellationToken):RunAsync(Request.Restart, selected, false, SaveCandidateKind.Main, cancellationToken);
        /// <summary>Retries a known noncommitting draft or completion failure exactly once.</summary>
        public Task RetrySaveAsync(CancellationToken cancellationToken) => RunAsync(Request.RetrySave, -1, false, SaveCandidateKind.Main, cancellationToken);
        /// <summary>Explicitly retries the retained solved attempt; never starts a frame-based retry loop.</summary>
        public Task RetryCompletionAsync(CancellationToken cancellationToken) => RunAsync(Request.Complete, -1, false, SaveCandidateKind.Main, cancellationToken);
        /// <summary>Replaces authority only from a full fresh load; unconfirmed old attempts cannot release results.</summary>
        public Task ReloadAsync(CancellationToken cancellationToken) => RunAsync(Request.Load, -1, false, SaveCandidateKind.Main, cancellationToken);
        /// <summary>Uses the current full observation for read-only recovery followed by exact explicit promotion.</summary>
        public Task RecoverAndPromoteAsync(SaveCandidateKind selectedKind, CancellationToken cancellationToken) => RunAsync(Request.Recover, -1, false, selectedKind, cancellationToken);
        /// <summary>Invokes only the separately confirmed corrupt reset path.</summary>
        public Task ConfirmedResetAsync(CancellationToken cancellationToken) => RunAsync(Request.Reset, -1, false, SaveCandidateKind.Main, cancellationToken);
        /// <summary>Observed host tick; commits a newly solved attempt once or delegates existing S debounce scheduling.</summary>
        public Task TickAutosaveAsync(CancellationToken cancellationToken)
        {
            RequireOwner();
            if (disposed || busy || writeFailure || requestFault || !authority || suspended) return Task.CompletedTask;
            if (Solved && !persisted.CanPresentResults)
                return completionAttempted ? Task.CompletedTask : RunAsync(Request.Complete, -1, false, SaveCandidateKind.Main, cancellationToken);
            return !autosave.IsDirty || Solved ? Task.CompletedTask : RunAsync(Request.Tick, -1, false, SaveCandidateKind.Main, cancellationToken);
        }
        /// <summary>Pauses the hidden puzzle without disabling campaign/menu navigation. Defaults false for the historical host.</summary>
        public void SetPresentationPaused(bool paused)
        {
            RequireOwner();if(disposed)return;bool changed=presentationPaused!=paused;
            if(changed){presentationPaused=paused;presentationReconcilePending=true;presentationPauseAt=paused?clock.MonotonicMilliseconds:(long?)null;}
            ReconcilePresentationPause();if(changed)Publish();
        }
        private void ReconcilePresentationPause()
        {
            if(!presentationReconcilePending||view==null||!persisted.CanMutate)return;
            bool pause=suspended||presentationPaused;
            if(!pause&&(busy||writeFailure||requestFault||!authority))return;
            if(view.IsPaused!=pause)view.Lifecycle(pause,pause&&presentationPauseAt.HasValue?presentationPauseAt.Value:clock.MonotonicMilliseconds);
            presentationReconcilePending=false;presentationPauseAt=null;
        }
        /// <summary>Applies the host's OR-combined suspension, pauses actual time, and immediately requests a flush.</summary>
        public Task SetSuspendedAsync(bool paused, CancellationToken cancellationToken)
        {
            RequireOwner();
            if (disposed) throw new ObjectDisposedException(nameof(SavedPublic240ReviewSession));
            if (suspended == paused) return Task.CompletedTask;
            suspended = paused;
            // Capture active time now, even when a previous draft write has not acknowledged yet.
            if (view != null && persisted.CanMutate)
                view.Lifecycle(paused || presentationPaused, clock.MonotonicMilliseconds);
            Publish();
            return AwaitLifecycleAsync(paused, cancellationToken);
        }
        private async Task AwaitLifecycleAsync(bool paused, CancellationToken cancellationToken)
        {
            using (var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, lifetime.Token))
            {
                while (true)
                {
                    Task? pending = null; Task? flush = null; bool obsolete = false;
                    await OnOwner(() =>
                    {
                        obsolete = suspended != paused;
                        if (obsolete) return;
                        if (busy) pending = requestIdle!.Task;
                        else flush = RunAsync(Request.Suspend, -1, paused, SaveCandidateKind.Main, linked.Token);
                    }).ConfigureAwait(false);
                    linked.Token.ThrowIfCancellationRequested();
                    if (obsolete || disposed) return;
                    if (flush != null) { await flush.ConfigureAwait(false); return; }
                    var cancelled = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    using (linked.Token.Register(() => cancelled.TrySetCanceled()))
                    { var winner = await Task.WhenAny(pending!, cancelled.Task).ConfigureAwait(false); await winner.ConfigureAwait(false); }
                }
            }
        }
        /// <summary>Begins only this current attempt's acknowledged player-path ride.</summary>
        public DevelopmentCommandStatus BeginTrainRide(Guid expectedAttempt)
        {
            RequireOwner();
            if (view == null || expectedAttempt != view.AttemptId) return DevelopmentCommandStatus.Rejected;
            if (!State.CanStartRide) return view.Reject("SAVE_REVIEW_RIDE_NOT_COMMITTED");
            status = SavedReviewStatus.TrainRide; view.Preview = DevelopmentPreviewPhase.TrainRide; Publish(); return view.Applied();
        }
        private bool Solved => view != null && persisted.Session?.Phase == SessionPhase.SolvedPendingCommit;
        private bool Editable => !disposed && authority && !requestFault && !writeFailure && !exclusive && !suspended && !presentationPaused && view?.IsPaused == false && persisted.CanMutate;
        private bool Navigable => !disposed && !busy && !suspended && authority && !requestFault && !writeFailure &&
            status != SavedReviewStatus.TrainRide && (!Solved || status == SavedReviewStatus.Results);
        private bool ProtectedObservation => persisted.RecoveryObservation == null || persisted.RecoveryObservation.Candidates.Any(c => c.Status == SaveCandidateStatus.Unsupported || c.Status == SaveCandidateStatus.Failed);
        private void RequireOwner()
        { if (Thread.CurrentThread.ManagedThreadId != owner) throw new InvalidOperationException("SAVE_REVIEW_OWNER_REQUIRED"); }
        private Task OnOwner(Action action) => dispatcher.InvokeAsync(() => { RequireOwner(); if (!disposed) action(); });

        private async Task RunAsync(Request request, int index, bool flag, SaveCandidateKind candidate, CancellationToken cancellationToken)
        {
            RequireOwner();
            if (disposed) throw new ObjectDisposedException(nameof(SavedPublic240ReviewSession));
            if (busy) { error = "SAVE_REVIEW_REQUEST_BUSY"; Publish(); return; }
            if (!Allowed(request, index, flag)) { error = "SAVE_REVIEW_ACTION_NOT_ALLOWED"; Publish(); return; }
            busy = true; exclusive = request != Request.Tick; error = "NONE";
            var idle = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously); requestIdle = idle;
            if (request == Request.Load) { authority = false; status = SavedReviewStatus.Loading; }
            else if (request == Request.Complete) { completionAttempted = true; status = SavedReviewStatus.CompletionPending; }
            else if (request != Request.Tick && request != Request.Suspend) status = SavedReviewStatus.Saving;
            if (request != Request.Tick) Publish();
            using (var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, lifetime.Token))
            {
                try
                {
                    linked.Token.ThrowIfCancellationRequested();
                    if (request == Request.Load)
                    {
                        var loaded = await persisted.LoadAsync(linked.Token).ConfigureAwait(false);
                        await OnOwner(() => ApplyLoaded(loaded)).ConfigureAwait(false);
                    }
                    else if (request == Request.Create || request == Request.Reset)
                    {
                        var created = request == Request.Create ? await persisted.CreateProfileAsync(linked.Token).ConfigureAwait(false) :
                            await persisted.ConfirmedResetCorruptProfileAsync(linked.Token).ConfigureAwait(false);
                        await OnOwner(() => { ApplyCommitted(created); if (created.Succeeded && authority) ClearView(); }).ConfigureAwait(false);
                    }
                    else if (request == Request.Recover)
                    {
                        SaveRecoveryRequest recovery = null!;
                        await OnOwner(() => recovery = new SaveRecoveryRequest(persisted.RecoveryObservation!, candidate)).ConfigureAwait(false);
                        var recovered = await persisted.RecoverAsync(recovery, linked.Token).ConfigureAwait(false);
                        await OnOwner(() => { lastLoad = recovered.Status; error = recovered.ErrorCode ?? "NONE"; authority = false; }).ConfigureAwait(false);
                        if (recovered.Succeeded)
                        {
                            var promoted = await persisted.PromoteRecoveryAsync(linked.Token).ConfigureAwait(false);
                            await OnOwner(() => { ApplyCommitted(promoted); if (promoted.Succeeded && authority) ClearView(); }).ConfigureAwait(false);
                        }
                        else await OnOwner(() => status = LoadFailureStatus(recovered.Status)).ConfigureAwait(false);
                    }
                    else if (request == Request.Select || request == Request.Restart)
                    {
                        bool resume = flag;
                        await OnOwner(() => { if (view != null && persisted.CanMutate && persisted.Session!.Phase != SessionPhase.Suspended) view.Lifecycle(true, clock.MonotonicMilliseconds); }).ConfigureAwait(false);
                        var flushed = await autosave.FlushAsync(linked.Token).ConfigureAwait(false);
                        bool proceed = false;
                        await OnOwner(() => { if (flushed != null) ApplyCommitted(flushed); proceed = flushed == null || flushed.Succeeded && authority; }).ConfigureAwait(false);
                        if (proceed)
                        {
                            await OnOwner(() =>
                            {
                                var puzzle = puzzles[index];
                                if (resume && !HasDraftForPuzzle(index)) throw new InvalidOperationException("SAVE_DRAFT_MISSING");
                                var state = resume ? persisted.RestoreDraft(puzzle.Identity) : persisted.Start(puzzle.Identity, Guid.NewGuid());
                                selected = index; view = new BoundView(this, puzzle, state, clock.MonotonicMilliseconds);
                                completionAttempted = false; writeFailure = false; requestFault = false;
                                if (state.Phase == SessionPhase.Suspended && !suspended && !presentationPaused) view.Lifecycle(false, clock.MonotonicMilliseconds);
                                if (!resume)
                                {
                                    if (suspended || presentationPaused) view.Lifecycle(true, clock.MonotonicMilliseconds);
                                    autosave.NotifyMutation(clock.MonotonicMilliseconds);
                                }
                                status = state.Phase == SessionPhase.SolvedPendingCommit ? SavedReviewStatus.CompletionPending : SavedReviewStatus.Ready;
                            }).ConfigureAwait(false);
                            // A deliberate fresh attempt has its own confirmed Ready checkpoint. Resume does not overwrite its source merely by displaying it.
                            if (!resume)
                            {
                                var saved = await autosave.FlushAsync(linked.Token).ConfigureAwait(false);
                                if (saved != null) await OnOwner(() => ApplyCommitted(saved)).ConfigureAwait(false);
                            }
                        }
                    }
                    else if (request == Request.Suspend)
                    {
                        if (flag)
                        {
                            bool complete = false;
                            await OnOwner(() => { complete = Solved && !persisted.CanPresentResults; if (complete) completionAttempted = true; }).ConfigureAwait(false);
                            var flushed = complete ? await autosave.CommitCompletionAsync(linked.Token).ConfigureAwait(false) : await autosave.FlushAsync(linked.Token).ConfigureAwait(false);
                            if (flushed != null) await OnOwner(() => ApplyCommitted(flushed)).ConfigureAwait(false);
                        }
                        else await OnOwner(() => { if (!writeFailure) status = SettledStatus(); }).ConfigureAwait(false);
                    }
                    else
                    {
                        Task<SaveCommitResult<LocalProfileState>?> work;
                        bool complete = request == Request.Complete;
                        await OnOwner(() => complete = complete || request == Request.RetrySave && Solved).ConfigureAwait(false);
                        if (complete)
                        {
                            await OnOwner(() => completionAttempted = true).ConfigureAwait(false);
                            work = autosave.CommitCompletionAsync(linked.Token);
                        }
                        else if (request == Request.Tick) work = autosave.TickAsync(clock.MonotonicMilliseconds, linked.Token);
                        else work = autosave.FlushAsync(linked.Token);
                        if (request == Request.Tick && !work.IsCompleted) await OnOwner(() => { status = SavedReviewStatus.Saving; Publish(); }).ConfigureAwait(false);
                        var committed = await work.ConfigureAwait(false);
                        if (committed != null) await OnOwner(() => ApplyCommitted(committed)).ConfigureAwait(false);
                        if (request == Request.RetrySave && committed?.Succeeded == true)
                            await OnOwner(() => { if (!suspended && !presentationPaused && view?.IsPaused == true && persisted.CanMutate) view.Lifecycle(false, clock.MonotonicMilliseconds); }).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    await OnOwner(() => { error = "SAVE_REVIEW_REQUEST_CANCELLED"; requestFault = true; authority = false; status = SavedReviewStatus.Failed; }).ConfigureAwait(false);
                    throw;
                }
                catch (Exception)
                {
                    await OnOwner(() => { error = "SAVE_REVIEW_REQUEST_FAILED"; requestFault = true; authority = false; status = SavedReviewStatus.Failed; }).ConfigureAwait(false);
                    throw;
                }
                finally
                {
                    try { await OnOwner(() => { busy = false; exclusive = false; SyncView(); Publish(); }).ConfigureAwait(false); }
                    finally { idle.TrySetResult(true); }
                }
            }
        }
        private bool Allowed(Request r, int index, bool resume)
        {
            if (r == Request.Load) return true;
            if (r == Request.Create) return !requestFault && persisted.Status == PersistedSessionStatus.NeedsInitialization;
            if (r == Request.Recover) return !requestFault && !ProtectedObservation && persisted.Status == PersistedSessionStatus.RecoveryRequired && persisted.RecoveryObservation!.Candidates.Any(x => x.Status == SaveCandidateStatus.Valid);
            if (r == Request.Reset) return !requestFault && !ProtectedObservation && persisted.Status == PersistedSessionStatus.RecoveryRequired && persisted.RecoveryObservation!.Candidates.Any(x => x.Status == SaveCandidateStatus.Corrupt);
            if (r == Request.Select || r == Request.Restart) return Navigable && CanEnterPuzzle(index) && (!resume || HasDraftForPuzzle(index));
            if (r == Request.Complete) return authority && !requestFault && Solved && !persisted.CanPresentResults && persisted.Status != PersistedSessionStatus.RecoveryRequired;
            if (r == Request.RetrySave) return State.CanRetrySave;
            if (r == Request.Suspend) return authority && !requestFault;
            return authority && !writeFailure && !requestFault;
        }
        private void ApplyLoaded(SaveLoadResult<LocalProfileState> result)
        {
            lastLoad = result.Status; lastCommit = null; error = result.ErrorCode ?? "NONE";
            authority = result.Succeeded && persisted.Status == PersistedSessionStatus.Ready && BoundSnapshot();
            requestFault = false; writeFailure = false;
            if (authority) { ClearView(); status = SavedReviewStatus.Ready; }
            else
            {
                status = result.Status == SaveLoadStatus.NeedsInitialization ? SavedReviewStatus.NeedsInitialization : LoadFailureStatus(result.Status);
                if (result.Succeeded && persisted.Status == PersistedSessionStatus.Ready) { error = "SAVE_REVIEW_PROFILE_BINDING_MISMATCH"; requestFault = true; status = SavedReviewStatus.Unsupported; }
            }
        }
        private static SavedReviewStatus LoadFailureStatus(SaveLoadStatus value) => value == SaveLoadStatus.Unsupported ? SavedReviewStatus.Unsupported :
            value == SaveLoadStatus.Failed || value == SaveLoadStatus.Cancelled ? SavedReviewStatus.Failed : SavedReviewStatus.RecoveryRequired;
        private void ApplyCommitted(SaveCommitResult<LocalProfileState> result)
        {
            lastCommit = result.Status; error = result.ErrorCode ?? "NONE";
            if (result.Succeeded && BoundSnapshot() && persisted.Status != PersistedSessionStatus.RecoveryRequired)
            { authority = true; writeFailure = false; requestFault = false; status = SettledStatus(); }
            else
            {
                writeFailure = true;
                if (result.Status != SaveCommitStatus.Failed && result.Status != SaveCommitStatus.Cancelled) authority = false;
                status = result.Status == SaveCommitStatus.Unsupported ? SavedReviewStatus.Unsupported : authority ? SavedReviewStatus.Failed : SavedReviewStatus.RecoveryRequired;
            }
            SyncView();
        }
        private bool BoundSnapshot() => persisted.Snapshot != null && persisted.Snapshot.Profile.Equals(expectedProfile) &&
            persisted.Snapshot.Payload.Profile.CatalogSet.PublicCatalog.Equals(publicCatalogIdentity);
        private SavedReviewStatus SettledStatus() => persisted.CanPresentResults ?
            (view?.Preview == DevelopmentPreviewPhase.TrainRide ? SavedReviewStatus.TrainRide : view?.Preview == DevelopmentPreviewPhase.Results ? SavedReviewStatus.Results : SavedReviewStatus.ProgressCommitted) : Solved ? SavedReviewStatus.CompletionPending : SavedReviewStatus.Ready;
        private void ClearView()
        {
            // Called only after a confirmed serialized replacement, after old coordinator work completed.
            // Dirty versions belong to the explicitly retired attempt; they cannot be flushed into a null/new session.
            autosave = new AutosaveCoordinator(persisted);
            view = null; selected = -1; completionAttempted = false; projectedIdentity = null;
        }
        private void SyncView()
        {
            if (view != null && persisted.Session != null && persisted.Session.AttemptId == view.AttemptId && view.Puzzle.Identity.PuzzleId == persisted.Session.PuzzleId &&
                view.Puzzle.Identity.HashProfile == persisted.Session.PublicPuzzleHashProfile && view.Puzzle.Identity.HashSha256 == persisted.Session.PublicPuzzleHashSha256)
                view.Refresh(persisted.Session);
            ReconcilePresentationPause();
        }
        private void Publish()
        {
            RequireOwner();
            var identity = view?.Puzzle.Identity;
            var snapshot = authority && !requestFault ? persisted.Snapshot : null;
            if (snapshot == null) { progress = null; projectedSnapshot = null; }
            else if (!ReferenceEquals(projectedSnapshot, snapshot) || !Equals(projectedIdentity, identity))
            { progress = new SavedReviewProgress(snapshot, puzzles, identity,persisted.CurrentEndlessStars); projectedSnapshot = snapshot; projectedIdentity = identity; }
            var observation = persisted.RecoveryObservation;
            var candidates = !authority && observation != null ? observation.Candidates : Array.Empty<SaveCandidateProvenance>();
            bool recover = !disposed && !busy && !authority && !requestFault && persisted.Status == PersistedSessionStatus.RecoveryRequired && !ProtectedObservation && observation!.Candidates.Any(c => c.Status == SaveCandidateStatus.Valid);
            bool reset = !disposed && !busy && !authority && !requestFault && persisted.Status == PersistedSessionStatus.RecoveryRequired && !ProtectedObservation && observation!.Candidates.Any(c => c.Status == SaveCandidateStatus.Corrupt);
            State = new SavedReviewState(++version, disposed ? SavedReviewStatus.Disposed : status, lastLoad, lastCommit, error,
                Editable, Navigable, Navigable && view != null&&(persisted.Session?.Mode!=STP.Puzzle.Domain.SessionMode.ENDLESS||!persisted.CanPresentResults), !disposed && !busy,
                !disposed && !busy && authority && writeFailure && !requestFault && (lastCommit == SaveCommitStatus.Failed || lastCommit == SaveCommitStatus.Cancelled),
                !disposed && !busy && !requestFault && persisted.Status == PersistedSessionStatus.NeedsInitialization, recover, reset,
                !disposed && !busy && !suspended && !presentationPaused && authority && !requestFault && status == SavedReviewStatus.ProgressCommitted && persisted.CanPresentResults,
                autosave.IsDirty, selected, identity, HasDraftForPuzzle(selected), progress, candidates);
        }
        /// <summary>Stops future publication and commands; an already executing physical write is not claimed rolled back.</summary>
        public void Dispose()
        {
            RequireOwner(); if (disposed) return; disposed = true; lifetime.Cancel(); authority = false; Publish();
        }

        private sealed class BoundView : IReviewPuzzleSession
        {
            private readonly SavedPublic240ReviewSession host;
            private PuzzleSessionState state;
            private PuzzleEvaluation evaluation;
            private IReadOnlyList<DevelopmentCell> path;
            private readonly PuzzleDefinition definition;
            private long anchor;
            internal DevelopmentPreviewPhase Preview;
            internal BoundView(SavedPublic240ReviewSession host, PublicPuzzleInput puzzle, PuzzleSessionState state, long now)
            { this.host = host; Puzzle = puzzle; this.state = state; definition = puzzle.ToDomainDefinition(); evaluation = PuzzleEvaluator.Evaluate(definition, state.Cells); path = CopyPath(evaluation); anchor = now; }
            public PublicPuzzleInput Puzzle { get; }
            public int Width => Puzzle.Width;
            public int Height => Puzzle.Height;
            public DevelopmentEndpoint EndpointA => new DevelopmentEndpoint(definition.A);
            public DevelopmentEndpoint EndpointB => new DevelopmentEndpoint(definition.B);
            public DevelopmentPreviewPhase Phase => Preview;
            public DevelopmentCommandStatus LastCommand { get; private set; } = DevelopmentCommandStatus.Unchanged;
            public string LastErrorCode { get; private set; } = "NONE";
            public Guid AttemptId => state.AttemptId;
            public long Revision => state.Revision;
            public bool TimerStarted => state.TimerStarted;
            public bool IsPaused => state.Phase == SessionPhase.Suspended;
            public bool CanUndo => Current && host.Editable && state.UndoStack.Count > 0;
            public bool HasObjectiveViolations => evaluation.Diagnostics.Count != 0;
            public IReadOnlyList<DevelopmentCell> BuiltPath => path;
            private bool Current => !host.disposed && ReferenceEquals(host.view, this) && host.persisted.Session?.AttemptId == AttemptId;
            private static IReadOnlyList<DevelopmentCell> CopyPath(PuzzleEvaluation value) => Array.AsReadOnly(value.Path.Select(c => new DevelopmentCell(c.X, c.Y)).ToArray());
            internal void Refresh(PuzzleSessionState next)
            { if (ReferenceEquals(state, next)) return; state = next; evaluation = PuzzleEvaluator.Evaluate(definition, next.Cells); path = CopyPath(evaluation); }
            public DevelopmentTool CellAt(int x, int y)
            { if (!definition.Grid.Contains(x, y)) throw new ArgumentOutOfRangeException(nameof(x)); return (DevelopmentTool)state.Cells[y * Width + x]; }
            public int RowTarget(int y) => definition.RowCounts[y];
            public int ColumnTarget(int x) => definition.ColumnCounts[x];
            public int RowVisible(int y) => evaluation.VisibleRows[y];
            public int ColumnVisible(int x) => evaluation.VisibleColumns[x];
            public long ElapsedAt(long time) => state.Phase == SessionPhase.Active && state.TimerStarted && time >= anchor ?
                (time - anchor > long.MaxValue - state.ActiveElapsedMs ? long.MaxValue : state.ActiveElapsedMs + time - anchor) : state.ActiveElapsedMs;
            public DevelopmentCommandEnvelope CaptureCommand()
            { host.RequireOwner(); if (!Current) throw new InvalidOperationException("STALE_ATTEMPT"); return new DevelopmentCommandEnvelope(AttemptId, Guid.NewGuid(), Revision); }
            private DevelopmentCommandEnvelope Envelope() => new DevelopmentCommandEnvelope(AttemptId, Guid.NewGuid(), Revision);
            public DevelopmentCommandStatus Apply(int x, int y, DevelopmentTool tool, long time) => Apply(x, y, tool, time, Envelope());
            public DevelopmentCommandStatus Apply(int x, int y, DevelopmentTool tool, long time, DevelopmentCommandEnvelope envelope)
            {
                if (!Check(envelope, false)) return LastCommand;
                var coordinate = CellCoordinate.Create(definition.Grid, x, y);
                if (coordinate.Value == null) return Reject("INVALID_COORDINATE");
                return Execute(new ApplyCellContent(envelope.CommandId, envelope.ExpectedRevision, coordinate.Value, (CellContent)tool), time);
            }
            public DevelopmentCommandStatus ApplyBatch(IReadOnlyList<DevelopmentCell>? cells, DevelopmentTool tool, long time) => ApplyBatch(cells, tool, time, Envelope());
            public DevelopmentCommandStatus ApplyBatch(IReadOnlyList<DevelopmentCell>? cells, DevelopmentTool tool, long time, DevelopmentCommandEnvelope envelope)
            {
                if (!Check(envelope, false)) return LastCommand;
                var coordinates = new List<CellCoordinate>();
                if (cells != null) foreach (var cell in cells)
                { var c = CellCoordinate.Create(definition.Grid, cell.X, cell.Y); if (c.Value == null) return Reject("INVALID_COORDINATE"); coordinates.Add(c.Value); }
                return Execute(new ApplyCellContentBatch(envelope.CommandId, envelope.ExpectedRevision, cells == null ? null : coordinates, (CellContent)tool), time);
            }
            public DevelopmentCommandStatus Undo(long time)
            { var e = Envelope(); return Check(e, false) ? Execute(new UndoLastAction(e.CommandId, e.ExpectedRevision), time) : LastCommand; }
            public DevelopmentCommandStatus Pause(long time) => Lifecycle(true, time);
            public DevelopmentCommandStatus Resume(long time) => Lifecycle(false, time);
            internal DevelopmentCommandStatus Lifecycle(bool pause, long time)
            {
                if(!pause&&(host.suspended||host.presentationPaused))return Unchanged();
                var e = Envelope(); if (!Check(e, true)) return LastCommand;
                if (pause == IsPaused) return Unchanged();
                return Execute(pause ? (PuzzleCommand)new PauseForLifecycle(e.CommandId, e.ExpectedRevision) : new ResumeFromLifecycle(e.CommandId, e.ExpectedRevision), time);
            }
            private bool Check(DevelopmentCommandEnvelope e, bool lifecycle)
            {
                host.RequireOwner();
                if (!Current || e.AttemptId != AttemptId) { Reject("STALE_ATTEMPT"); return false; }
                if (lifecycle ? !host.persisted.CanMutate : !host.Editable)
                { Reject("SAVE_REVIEW_INPUT_BLOCKED"); return false; }
                return true;
            }
            private DevelopmentCommandStatus Execute(PuzzleCommand command, long time)
            {
                var before = state; var result = host.persisted.Handle(command, time);
                if (result.Error != DomainError.NONE) return Reject(result.Error.ToString());
                if (ReferenceEquals(before, result.State)) return Unchanged();
                Refresh(result.State!); anchor = time; host.autosave.NotifyMutation(time);
                if (state.Phase == SessionPhase.SolvedPendingCommit) { host.status = SavedReviewStatus.CompletionPending; host.completionAttempted = false; }
                host.Publish(); return Applied();
            }
            public DevelopmentCommandStatus Restart() => Reject("SAVE_REVIEW_ASYNC_RESTART_REQUIRED");
            public DevelopmentCommandStatus CompleteTrainRide(Guid expectedAttempt)
            {
                host.RequireOwner();
                if (!Current || expectedAttempt != AttemptId) return Reject("STALE_ATTEMPT");
                if (host.status == SavedReviewStatus.Results) return Unchanged();
                if (host.suspended || host.status != SavedReviewStatus.TrainRide || !host.persisted.CanPresentResults) return Reject("TRAIN_RIDE_NOT_ACTIVE");
                Preview = DevelopmentPreviewPhase.Results; host.status = SavedReviewStatus.Results; host.Publish(); return Applied();
            }
            internal DevelopmentCommandStatus Reject(string code) { LastErrorCode = code; return LastCommand = DevelopmentCommandStatus.Rejected; }
            internal DevelopmentCommandStatus Applied() { LastErrorCode = "NONE"; return LastCommand = DevelopmentCommandStatus.Applied; }
            private DevelopmentCommandStatus Unchanged() { LastErrorCode = "NONE"; return LastCommand = DevelopmentCommandStatus.Unchanged; }
        }
    }
}
