using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;

namespace STP.Tests.Application.EditMode
{
    public sealed class SavedPublic240ReviewSessionTests
    {
        private sealed class QueueDispatcher : ILocalSaveDispatcher
        {
            private readonly int owner = Thread.CurrentThread.ManagedThreadId;
            private readonly ConcurrentQueue<(Action Action, TaskCompletionSource<bool> Done)> pending = new ConcurrentQueue<(Action, TaskCompletionSource<bool>)>();
            public Task InvokeAsync(Action action)
            {
                if (Thread.CurrentThread.ManagedThreadId == owner) { action(); return Task.CompletedTask; }
                var done = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously); pending.Enqueue((action, done)); return done.Task;
            }
            internal void Drain()
            {
                Assert.That(Thread.CurrentThread.ManagedThreadId, Is.EqualTo(owner));
                while (pending.TryDequeue(out var item))
                { try { item.Action(); item.Done.TrySetResult(true); } catch (Exception error) { item.Done.TrySetException(error); } }
            }
        }
        private sealed class Repository : ILocalProfileRepository
        {
            internal SaveSnapshot<LocalProfileState>? Stored;
            internal SaveLoadResult<LocalProfileState>? OverrideLoad;
            internal Func<SaveSnapshot<LocalProfileState>, SaveCommitResult<LocalProfileState>?>? Result;
            internal TaskCompletionSource<SaveCommitResult<LocalProfileState>>? Hold;
            internal TaskCompletionSource<SaveLoadResult<LocalProfileState>>? HoldLoad;
            internal bool FailLoad;
            internal SaveSnapshot<LocalProfileState>? Offered;
            internal int Loads, Commits, Creates, Recovers, Resets;
            private readonly SaveProfileIdentity profile;
            private readonly CatalogIdentity catalog;
            internal Repository(SaveProfileIdentity profile, CatalogIdentity catalog) { this.profile = profile; this.catalog = catalog; }
            internal SaveRepositoryObservation Observation(SaveCandidateStatus main = SaveCandidateStatus.Valid)
            {
                return new SaveRepositoryObservation(profile, catalog, new[] {
                    new SaveCandidateProvenance(SaveCandidateKind.Main, main, main == SaveCandidateStatus.Valid ? Stored!.Generation : (long?)null,
                        main == SaveCandidateStatus.Valid ? new string('a',64) : null, main == SaveCandidateStatus.Missing ? null : new string('b',64)),
                    new SaveCandidateProvenance(SaveCandidateKind.Backup, SaveCandidateStatus.Missing, null, null, null),
                    new SaveCandidateProvenance(SaveCandidateKind.Pending, SaveCandidateStatus.Missing, null, null, null) });
            }
            public Task<SaveLoadResult<LocalProfileState>> LoadAsync(CancellationToken ct)
            { Loads++; ct.ThrowIfCancellationRequested(); if (FailLoad) return Task.FromException<SaveLoadResult<LocalProfileState>>(new InvalidOperationException("TEST_UNEXPECTED_LOAD_FAULT")); if (HoldLoad != null) return HoldLoad.Task; return Task.FromResult(OverrideLoad ?? (Stored == null ? SaveLoadResult<LocalProfileState>.NeedsInitialization() : SaveLoadResult<LocalProfileState>.Success(Stored, Observation()))); }
            public Task<SaveLoadResult<LocalProfileState>> RecoverAsync(SaveRecoveryRequest request, CancellationToken ct)
            { Recovers++; return Task.FromResult(SaveLoadResult<LocalProfileState>.Success(Stored!, request.Observation)); }
            public Task<SaveCommitResult<LocalProfileState>> CommitAsync(SaveSnapshot<LocalProfileState> snapshot, long expected, CancellationToken ct)
            {
                Commits++; Offered = snapshot;
                var result = Result?.Invoke(snapshot);
                if (result != null) return Task.FromResult(result);
                if (Hold != null) return Hold.Task;
                Stored = snapshot; return Task.FromResult(SaveCommitResult<LocalProfileState>.Success(snapshot));
            }
            public Task<SaveCommitResult<LocalProfileState>> CreateProfileAsync(LocalProfileState state, CancellationToken ct)
            { Creates++; return CommitAsync(new SaveSnapshot<LocalProfileState>(profile, catalog, 1, state), 0, ct); }
            public Task<SaveCommitResult<LocalProfileState>> ConfirmedResetCorruptProfileAsync(SaveRepositoryObservation observation, LocalProfileState state, CancellationToken ct)
            { Resets++; OverrideLoad = null; return CommitAsync(new SaveSnapshot<LocalProfileState>(profile, catalog, (Stored?.Generation ?? 1) + 1, state), Stored?.Generation ?? 1, ct); }
            internal void Release()
            { var held = Hold!; Hold = null; Stored = Offered; held.SetResult(SaveCommitResult<LocalProfileState>.Success(Offered!)); }
        }
        private sealed class Fixture : IDisposable
        {
            internal readonly QueueDispatcher Dispatcher = new QueueDispatcher();
            internal readonly LocalSaveContractFixtures.Clock Clock = new LocalSaveContractFixtures.Clock();
            internal readonly Repository Repo;
            internal readonly PersistedPuzzleSession Use;
            internal readonly SavedPublic240ReviewSession Bridge;
            internal readonly LocalSaveContractFixtures.PublicCatalog Puzzles;
            internal readonly SaveProfileIdentity Profile;
            internal readonly CatalogSetIdentity Catalogs;
            internal Fixture(bool missing = false)
            {
                var json = LocalSaveContractFixtures.Json;
                CatalogIdentity Id(string id, char hash) => new CatalogIdentity("qa-public240-v1-" + id, 1, new string(hash, 64), CatalogUse.IsolatedReview, CatalogApprovalStatus.Draft);
                var publicId = Id("test-public", 'a'); var campaignId = Id("test-campaign", 'b'); var completionId = Id("test-completion", 'c');
                string projection = "{\"publicCatalog\":" + LocalSaveHash.CatalogProjection(publicId) + ",\"campaignCatalog\":" + LocalSaveHash.CatalogProjection(campaignId) + ",\"completionCatalog\":" + LocalSaveHash.CatalogProjection(completionId) + "}";
                var aggregate = new CatalogIdentity("qa-public240-v1-test-aggregate", 1, LocalSaveHash.Bytes(LocalSaveHash.Canonical(json, projection)), CatalogUse.IsolatedReview, CatalogApprovalStatus.Draft);
                Catalogs = CatalogSetIdentity.Create(publicId, campaignId, completionId, aggregate, json);
                Profile = new SaveProfileIdentity("qa-public240-v1", "r", CatalogUse.IsolatedReview);
                var inputs = new[] { Input("S1-01-01-01", 2, 2), Input("S1-01-01-02", 3, 2), Input("S1-01-01-03", 10, 10) };
                Puzzles = new LocalSaveContractFixtures.PublicCatalog(publicId, inputs);
                var campaign = new LocalSaveContractFixtures.CampaignCatalog(campaignId, inputs.Select((p, i) => new LockedCampaignEntry(campaignId, p.Identity, "S1", "S1-01", "S1-01-01", i + 1, Array.Empty<string>())).ToArray());
                var completion = new LocalSaveContractFixtures.CompletionCatalog(completionId, inputs.Select(p => new CompletionValues(completionId, p.Identity, 15, 5, 10, 60, 150, null, null)).ToArray());
                var ledger = LocalSaveContractFixtures.Construct<ILocalLedger>("STP.Infrastructure.Persistence", "STP.Infrastructure.Persistence.SaveLedgerCodec", json);
                var policy = new LocalProgressPolicy(Puzzles, campaign, completion, json, ledger);
                Repo = new Repository(Profile, aggregate);
                if (!missing) Repo.Stored = new SaveSnapshot<LocalProfileState>(Profile, aggregate, 1, policy.NewProfile(Profile, Catalogs, Clock.UtcNow));
                Use = new PersistedPuzzleSession(Repo, Puzzles, policy, Profile, Catalogs, Clock, Dispatcher);
                Bridge = new SavedPublic240ReviewSession(Use, new AutosaveCoordinator(Use), Puzzles, Clock, Dispatcher);
            }
            private static PublicPuzzleInput Input(string id, int width, int height)
            {
                var rows = new int[height]; rows[0] = width; var cols = Enumerable.Repeat(1, width).ToArray();
                string source = "{\"puzzleId\":" + LocalSaveHash.Quote(id) + ",\"rulesetVersion\":\"train-track-v1\",\"grid\":{\"width\":" + width + ",\"height\":" + height + "},\"endpoints\":{\"a\":{\"side\":\"W\",\"index\":0},\"b\":{\"side\":\"E\",\"index\":0}},\"rowCounts\":[" + string.Join(",", rows) + "],\"columnCounts\":[" + string.Join(",", cols) + "]}";
                return new PublicPuzzleInput(new PublicPuzzleIdentity(id, "STP-PUZZLE-SEMANTIC-JCS-1", LocalSaveHash.Bytes(LocalSaveHash.Canonical(LocalSaveContractFixtures.Json, source))), "train-track-v1", width, height, PublicPuzzleSide.W, 0, PublicPuzzleSide.E, 0, rows, cols);
            }
            internal void Pump(Task task)
            {
                Until(() => task.IsCompleted); task.GetAwaiter().GetResult();
            }
            internal void Until(Func<bool> condition)
            {
                var timeout = Stopwatch.StartNew();
                while (!condition()) { Dispatcher.Drain(); if (timeout.ElapsedMilliseconds > 10000) Assert.Fail("Observed worker did not finish while owner dispatcher was pumped"); Thread.Sleep(1); }
                Dispatcher.Drain();
            }
            internal void Open(int index = 0) { Pump(Bridge.InitializeAsync(CancellationToken.None)); Pump(Bridge.SelectPuzzleAsync(index, false, CancellationToken.None)); }
            internal void Solve()
            { var v = Bridge.PuzzleView!; for (int x = 0; x < v.Width; x++) Assert.That(v.Apply(x, 0, DevelopmentTool.TrackEW, Clock.MonotonicMilliseconds++), Is.EqualTo(DevelopmentCommandStatus.Applied)); }
            public void Dispose() { Bridge.Dispose(); Dispatcher.Drain(); }
        }

        [Test]
        public void MissingColdLoadDoesNotCreateProfileOrFakeAttempt()
        {
            using var f = new Fixture(true); f.Pump(f.Bridge.InitializeAsync(CancellationToken.None));
            Assert.That(f.Bridge.State.Status, Is.EqualTo(SavedReviewStatus.NeedsInitialization));
            Assert.That(f.Bridge.State.CanCreateProfile, Is.True); Assert.That(f.Repo.Commits, Is.Zero);
            Assert.That(f.Bridge.PuzzleView, Is.Null); Assert.That(f.Bridge.State.SelectedIndex, Is.EqualTo(-1));
            Assert.That(f.Bridge.HasDraftForPuzzle(0), Is.False); Assert.That(f.Bridge.State.Progress, Is.Null);
            f.Pump(f.Bridge.CreateProfileAsync(CancellationToken.None));
            Assert.That(f.Repo.Creates, Is.EqualTo(1)); Assert.That(f.Use.Snapshot!.Generation, Is.EqualTo(1));
            Assert.That(f.Bridge.PuzzleView, Is.Null); Assert.That(f.Bridge.State.Progress!.SelectedIdentity, Is.Null);
        }
        [Test]
        public void ExplicitOpenConfirmsReadyCheckpointAndReadOnlyDraftQueriesDoNotSelect()
        {
            using var f = new Fixture(); f.Open(); var view = f.Bridge.PuzzleView!;
            Assert.That(view.TimerStarted, Is.False); Assert.That(view.ElapsedAt(100), Is.Zero);
            Assert.That(f.Use.Snapshot!.Generation, Is.EqualTo(2)); Assert.That(f.Bridge.HasDraftForPuzzle(0), Is.True);
            int writes = f.Repo.Commits; var snapshot = f.Use.Snapshot;
            for (int i = -1; i <= f.Puzzles.Puzzles.Count; i++) f.Bridge.HasDraftForPuzzle(i);
            Assert.That(f.Repo.Commits, Is.EqualTo(writes)); Assert.That(f.Use.Snapshot, Is.SameAs(snapshot));
            Assert.That(f.Bridge.State.SelectedIndex, Is.Zero); Assert.That(f.Bridge.PuzzleView, Is.SameAs(view));
        }
        [Test]
        public void NoOpDoesNotNotifyButActualUndoAndMarkerHistoryRoundtripThroughS()
        {
            using var f = new Fixture(); f.Open(); var view = f.Bridge.PuzzleView!;
            Assert.That(view.Apply(0, 1, DevelopmentTool.Clear, 0), Is.EqualTo(DevelopmentCommandStatus.Unchanged));
            Assert.That(f.Bridge.State.IsDirty || view.TimerStarted, Is.False);
            Assert.That(view.Apply(0, 1, DevelopmentTool.EmptyMarker, 10), Is.EqualTo(DevelopmentCommandStatus.Applied));
            Assert.That(view.Apply(0, 1, DevelopmentTool.Clear, 20), Is.EqualTo(DevelopmentCommandStatus.Applied));
            Assert.That(view.Apply(0, 1, DevelopmentTool.OccupiedMarker, 30), Is.EqualTo(DevelopmentCommandStatus.Applied));
            Assert.That(view.Undo(40), Is.EqualTo(DevelopmentCommandStatus.Applied)); f.Clock.MonotonicMilliseconds = 50;
            f.Pump(f.Bridge.SetSuspendedAsync(true, CancellationToken.None));
            var cp = LocalSaveContractFixtures.Get(f.Use.Snapshot!.Payload.Drafts[0], "Checkpoint")!;
            Assert.That(LocalSaveContractFixtures.Get(cp, "UsedEmptyMarker"), Is.True);
            Assert.That(LocalSaveContractFixtures.Get(cp, "UsedOccupiedMarker"), Is.True);
            Assert.That(LocalSaveContractFixtures.Get(cp, "CorrectionCount"), Is.EqualTo(2));
            var attempt = view.AttemptId; f.Pump(f.Bridge.ReloadAsync(CancellationToken.None));
            Assert.That(f.Bridge.PuzzleView, Is.Null); f.Clock.MonotonicMilliseconds = 10000;
            f.Pump(f.Bridge.SetSuspendedAsync(false, CancellationToken.None));
            f.Pump(f.Bridge.SelectPuzzleAsync(0, true, CancellationToken.None));
            Assert.That(f.Bridge.PuzzleView!.AttemptId, Is.EqualTo(attempt)); Assert.That(f.Bridge.PuzzleView.CellAt(0, 1), Is.EqualTo(DevelopmentTool.Clear));
            Assert.That(f.Bridge.PuzzleView.ElapsedAt(10000), Is.EqualTo(40)); Assert.That(f.Bridge.PuzzleView.CanUndo, Is.True);
        }
        [Test]
        public void SafeDraftFailureStopsFrameRetriesAndExplicitRetryAloneConfirms()
        {
            using var f = new Fixture(); f.Open(); f.Bridge.PuzzleView!.Apply(0, 1, DevelopmentTool.EmptyMarker, 0);
            var durable = f.Use.Snapshot; f.Repo.Result = _ => SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed, "SAVE_FLUSH_FAILED");
            f.Clock.MonotonicMilliseconds = 499; f.Pump(f.Bridge.TickAutosaveAsync(CancellationToken.None)); Assert.That(f.Repo.Commits, Is.EqualTo(1));
            f.Clock.MonotonicMilliseconds = 500; f.Pump(f.Bridge.TickAutosaveAsync(CancellationToken.None));
            Assert.That(f.Bridge.State.Status, Is.EqualTo(SavedReviewStatus.Failed)); Assert.That(f.Use.Snapshot, Is.SameAs(durable));
            Assert.That(f.Bridge.State.CanRetrySave, Is.True); int writes = f.Repo.Commits;
            for (int i = 0; i < 5; i++) { f.Clock.MonotonicMilliseconds += 2000; f.Pump(f.Bridge.TickAutosaveAsync(CancellationToken.None)); }
            Assert.That(f.Repo.Commits, Is.EqualTo(writes)); f.Repo.Result = null; f.Pump(f.Bridge.RetrySaveAsync(CancellationToken.None));
            Assert.That(f.Bridge.State.IsDirty, Is.False); Assert.That(f.Use.Snapshot!.Generation, Is.EqualTo(3));
        }
        [Test]
        public void SolveDoesNotStartUnobservedCommitAndHeldAcknowledgementCannotReleaseRide()
        {
            using var f = new Fixture(); f.Open(); var original = f.Bridge.PuzzleView!; f.Solve();
            Assert.That(f.Repo.Commits, Is.EqualTo(1)); Assert.That(f.Bridge.State.Status, Is.EqualTo(SavedReviewStatus.CompletionPending));
            Assert.That(f.Bridge.State.Progress!.BalancePatience, Is.Zero); Assert.That(f.Bridge.State.CanStartRide, Is.False);
            f.Repo.Hold = new TaskCompletionSource<SaveCommitResult<LocalProfileState>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var completing = f.Bridge.TickAutosaveAsync(CancellationToken.None); f.Until(() => f.Repo.Commits == 2);
            Assert.That(completing.IsCompleted, Is.False); Assert.That(f.Bridge.BeginTrainRide(original.AttemptId), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(original.Apply(0, 1, DevelopmentTool.EmptyMarker, 100), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(f.Bridge.State.Progress!.BalancePatience, Is.Zero); f.Repo.Release(); f.Pump(completing);
            Assert.That(f.Bridge.State.Status, Is.EqualTo(SavedReviewStatus.ProgressCommitted)); Assert.That(f.Bridge.State.CanStartRide, Is.True);
            Assert.That(f.Bridge.State.Progress!.BalancePatience, Is.EqualTo(15)); Assert.That(f.Bridge.State.Progress.SelectedStars, Is.EqualTo(1));
            Assert.That(original.BuiltPath.Count, Is.EqualTo(2)); Assert.That(f.Bridge.BeginTrainRide(original.AttemptId), Is.EqualTo(DevelopmentCommandStatus.Applied));
            Assert.That(original.CompleteTrainRide(original.AttemptId), Is.EqualTo(DevelopmentCommandStatus.Applied));
            Assert.That(f.Bridge.State.Status, Is.EqualTo(SavedReviewStatus.Results)); f.Pump(f.Bridge.TickAutosaveAsync(CancellationToken.None));
            Assert.That(f.Repo.Commits, Is.EqualTo(2)); f.Pump(f.Bridge.RestartAsync(CancellationToken.None));
            Assert.That(f.Bridge.PuzzleView!.AttemptId, Is.Not.EqualTo(original.AttemptId)); Assert.That(f.Bridge.PuzzleView.TimerStarted, Is.False);
            f.Solve(); f.Pump(f.Bridge.TickAutosaveAsync(CancellationToken.None)); Assert.That(f.Bridge.State.Progress!.BalancePatience, Is.EqualTo(15));
        }
        [Test]
        public void FailedCompletionRequiresExplicitRetryAndNeverDoublePays()
        {
            using var f = new Fixture(); f.Open(); f.Solve(); f.Repo.Result = _ => SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed, "SAVE_FLUSH_FAILED");
            f.Pump(f.Bridge.TickAutosaveAsync(CancellationToken.None)); int writes = f.Repo.Commits;
            Assert.That(f.Bridge.State.CanStartRide, Is.False); Assert.That(f.Bridge.State.Progress!.BalancePatience, Is.Zero);
            f.Pump(f.Bridge.TickAutosaveAsync(CancellationToken.None)); Assert.That(f.Repo.Commits, Is.EqualTo(writes));
            f.Repo.Result = null; f.Pump(f.Bridge.RetryCompletionAsync(CancellationToken.None));
            Assert.That(f.Bridge.State.CanStartRide, Is.True); Assert.That(f.Bridge.State.Progress!.BalancePatience, Is.EqualTo(15));
            f.Pump(f.Bridge.RetryCompletionAsync(CancellationToken.None)); Assert.That(f.Repo.Commits, Is.EqualTo(writes + 1));
        }
        [Test]
        public void OldViewsCannotRetargetNewAttemptDimensionsCommandsOrRide()
        {
            using var f = new Fixture(); f.Open(); var old = f.Bridge.PuzzleView!; var envelope = old.CaptureCommand();
            old.Apply(0, 1, DevelopmentTool.EmptyMarker, 0); f.Clock.MonotonicMilliseconds = 20;
            f.Pump(f.Bridge.SelectPuzzleAsync(2, false, CancellationToken.None)); var current = f.Bridge.PuzzleView!; var revision = current.Revision;
            Assert.That(current.Width, Is.EqualTo(10)); Assert.That(old.Width, Is.EqualTo(2));
            Assert.That(old.Apply(0, 0, DevelopmentTool.TrackEW, 30, envelope), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(old.LastErrorCode, Is.EqualTo("STALE_ATTEMPT")); Assert.That(old.Undo(30), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(old.Pause(30), Is.EqualTo(DevelopmentCommandStatus.Rejected)); Assert.That(old.Resume(30), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(old.CompleteTrainRide(old.AttemptId), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.Throws<InvalidOperationException>(() => old.CaptureCommand()); Assert.That(current.Revision, Is.EqualTo(revision));
            Assert.That(current.Restart(), Is.EqualTo(DevelopmentCommandStatus.Rejected)); Assert.That(current.LastErrorCode, Is.EqualTo("SAVE_REVIEW_ASYNC_RESTART_REQUIRED"));
        }
        [Test]
        public void NavigationWaitsForActualFlushAndFailureKeepsOldAttemptAndDurableDraft()
        {
            using var f = new Fixture(); f.Open(); var old = f.Bridge.PuzzleView!; old.Apply(0, 1, DevelopmentTool.EmptyMarker, 0);
            f.Clock.MonotonicMilliseconds = 100; var durable = f.Use.Snapshot;
            f.Repo.Result = _ => SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed, "SAVE_FLUSH_FAILED");
            f.Pump(f.Bridge.SelectPuzzleAsync(1, false, CancellationToken.None));
            Assert.That(f.Bridge.PuzzleView, Is.SameAs(old)); Assert.That(f.Bridge.State.SelectedIndex, Is.Zero); Assert.That(f.Use.Snapshot, Is.SameAs(durable));
            Assert.That(old.IsPaused, Is.True); f.Clock.MonotonicMilliseconds = 10000; f.Repo.Result = null;
            f.Pump(f.Bridge.RetrySaveAsync(CancellationToken.None)); Assert.That(old.IsPaused, Is.False);
            Assert.That(old.ElapsedAt(10000), Is.EqualTo(100));
        }
        [Test]
        public void LifecyclePausesAtCallbackInstantWhileEarlierWriteIsHeld()
        {
            using var f = new Fixture(); f.Open(); var v = f.Bridge.PuzzleView!; v.Apply(0, 1, DevelopmentTool.EmptyMarker, 0);
            f.Repo.Hold = new TaskCompletionSource<SaveCommitResult<LocalProfileState>>(TaskCreationOptions.RunContinuationsAsynchronously);
            f.Clock.MonotonicMilliseconds = 500; var writing = f.Bridge.TickAutosaveAsync(CancellationToken.None); f.Until(() => f.Repo.Commits == 2);
            f.Clock.MonotonicMilliseconds = 550; var pause = f.Bridge.SetSuspendedAsync(true, CancellationToken.None);
            Assert.That(v.IsPaused, Is.True); Assert.That(v.ElapsedAt(10000), Is.EqualTo(550)); Assert.That(pause.IsCompleted, Is.False);
            f.Clock.MonotonicMilliseconds = 10000; f.Repo.Release(); f.Pump(writing); f.Pump(pause);
            var cp = LocalSaveContractFixtures.Get(f.Use.Snapshot!.Payload.Drafts[0], "Checkpoint")!;
            Assert.That(LocalSaveContractFixtures.Get(cp, "ActiveElapsedMs"), Is.EqualTo(550));
            int writes = f.Repo.Commits; f.Pump(f.Bridge.SetSuspendedAsync(true, CancellationToken.None)); Assert.That(f.Repo.Commits, Is.EqualTo(writes));
            f.Pump(f.Bridge.SetSuspendedAsync(false, CancellationToken.None)); Assert.That(v.ElapsedAt(10000), Is.EqualTo(550));
        }
        [Test]
        public void NewMutationDuringDraftWriteRemainsDirtyAfterEarlierAcknowledgement()
        {
            using var f = new Fixture(); f.Open(); var v = f.Bridge.PuzzleView!; v.Apply(0, 1, DevelopmentTool.EmptyMarker, 0);
            f.Repo.Hold = new TaskCompletionSource<SaveCommitResult<LocalProfileState>>(TaskCreationOptions.RunContinuationsAsynchronously);
            f.Clock.MonotonicMilliseconds = 500; var writing = f.Bridge.TickAutosaveAsync(CancellationToken.None); f.Until(() => f.Repo.Commits == 2);
            Assert.That(v.Apply(1, 1, DevelopmentTool.OccupiedMarker, 550), Is.EqualTo(DevelopmentCommandStatus.Applied));
            f.Clock.MonotonicMilliseconds = 550; f.Repo.Release(); f.Pump(writing); Assert.That(f.Bridge.State.IsDirty, Is.True);
            f.Clock.MonotonicMilliseconds = 1049; f.Pump(f.Bridge.TickAutosaveAsync(CancellationToken.None)); Assert.That(f.Bridge.State.IsDirty, Is.True);
            f.Clock.MonotonicMilliseconds = 1050; f.Pump(f.Bridge.TickAutosaveAsync(CancellationToken.None)); Assert.That(f.Bridge.State.IsDirty, Is.False);
        }
        [Test]
        public void UnknownCompletionRequiresFreshLoadAndCannotReuseOldSolvedView()
        {
            using var f = new Fixture(); f.Open(); f.Solve(); var old = f.Bridge.PuzzleView!;
            f.Repo.Result = snapshot => { f.Repo.Stored = snapshot; return SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.RecoveryRequired, "SAVE_COMMIT_OUTCOME_UNKNOWN"); };
            f.Pump(f.Bridge.TickAutosaveAsync(CancellationToken.None)); Assert.That(f.Bridge.State.Progress, Is.Null); Assert.That(f.Bridge.State.CanStartRide || f.Bridge.State.CanRetrySave, Is.False);
            f.Repo.Result = null; f.Pump(f.Bridge.ReloadAsync(CancellationToken.None)); Assert.That(f.Bridge.PuzzleView, Is.Null);
            Assert.That(f.Bridge.State.Progress!.BalancePatience, Is.EqualTo(15)); Assert.That(f.Bridge.State.CanStartRide, Is.False);
            f.Pump(f.Bridge.SelectPuzzleAsync(1, false, CancellationToken.None)); Assert.That(old.Undo(100), Is.EqualTo(DevelopmentCommandStatus.Rejected));
        }
        [Test]
        public void ConflictUsesFreshObservationAndExplicitRecoveryPromotionClearsUnconfirmedAttempt()
        {
            using var f = new Fixture(); f.Open(); f.Solve(); f.Repo.Result = _ => SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Conflict, "SAVE_GENERATION_CONFLICT");
            int loads = f.Repo.Loads; f.Pump(f.Bridge.TickAutosaveAsync(CancellationToken.None));
            Assert.That(f.Repo.Loads, Is.EqualTo(loads + 1)); Assert.That(f.Bridge.State.CanRecover, Is.True);
            Assert.That(f.Bridge.State.RecoveryChoices.Count, Is.EqualTo(3)); Assert.That(f.Bridge.State.Progress, Is.Null);
            f.Repo.Result = null; f.Pump(f.Bridge.RecoverAndPromoteAsync(SaveCandidateKind.Main, CancellationToken.None));
            Assert.That(f.Repo.Recovers, Is.EqualTo(1)); Assert.That(f.Bridge.PuzzleView, Is.Null); Assert.That(f.Bridge.State.CanNavigate, Is.True);
            f.Pump(f.Bridge.SelectPuzzleAsync(1, false, CancellationToken.None)); Assert.That(f.Bridge.State.SelectedIndex, Is.EqualTo(1));
        }
        [TestCase(SaveCandidateStatus.Unsupported, SaveLoadStatus.Unsupported)]
        [TestCase(SaveCandidateStatus.Failed, SaveLoadStatus.Failed)]
        public void UnsupportedOrFailedCandidateNeverOffersCreateRecoveryOrReset(SaveCandidateStatus candidate, SaveLoadStatus outcome)
        {
            using var f = new Fixture(); f.Repo.OverrideLoad = SaveLoadResult<LocalProfileState>.Failure(outcome, "SAVE_SOURCE_PROTECTED", f.Repo.Observation(candidate));
            f.Pump(f.Bridge.InitializeAsync(CancellationToken.None)); var s = f.Bridge.State;
            Assert.That(s.CanCreateProfile || s.CanReset || s.CanRecover || s.CanNavigate || s.CanStartRide, Is.False);
            f.Pump(f.Bridge.ConfirmedResetAsync(CancellationToken.None)); Assert.That(f.Repo.Resets, Is.Zero); Assert.That(f.Repo.Commits, Is.Zero);
        }
        [Test]
        public void CorruptResetIsOnlyAnExplicitObservedActionAndClearsOldView()
        {
            using var f = new Fixture(); f.Open(); f.Repo.OverrideLoad = SaveLoadResult<LocalProfileState>.Failure(SaveLoadStatus.Corrupt, "SAVE_NO_VALID_CANDIDATE", f.Repo.Observation(SaveCandidateStatus.Corrupt));
            f.Pump(f.Bridge.ReloadAsync(CancellationToken.None)); Assert.That(f.Bridge.State.CanReset, Is.True); Assert.That(f.Repo.Resets, Is.Zero);
            f.Pump(f.Bridge.ConfirmedResetAsync(CancellationToken.None)); Assert.That(f.Repo.Resets, Is.EqualTo(1)); Assert.That(f.Bridge.PuzzleView, Is.Null);
            Assert.That(f.Bridge.State.Progress!.BalancePatience, Is.Zero); Assert.That(f.Bridge.State.CanNavigate, Is.True);
        }
        [Test]
        public void FailedNewReadyWriteDoesNotInventConfirmedDraftAvailability()
        {
            using var f = new Fixture(); f.Open(); int previousWrites = f.Repo.Commits;
            // The old Ready attempt must first pause and flush successfully. Only the newly offered Ready attempt fails.
            f.Repo.Result = _ => f.Repo.Commits == previousWrites + 2 ? SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed, "SAVE_FLUSH_FAILED") : null;
            f.Pump(f.Bridge.SelectPuzzleAsync(1, false, CancellationToken.None)); Assert.That(f.Bridge.State.SelectedIndex, Is.EqualTo(1));
            Assert.That(f.Bridge.HasDraftForPuzzle(1), Is.False); Assert.That(f.Bridge.HasDraftForPuzzle(0), Is.True);
            Assert.That(f.Repo.Commits, Is.EqualTo(previousWrites + 2)); Assert.That(f.Use.Snapshot!.Generation, Is.EqualTo(3));
            Assert.That(f.Use.Snapshot.Payload.Drafts.Single().Identity, Is.EqualTo(f.Puzzles.Puzzles[0].Identity));
            Assert.That(f.Bridge.State.IsDirty, Is.True); Assert.That(f.Bridge.State.Status, Is.EqualTo(SavedReviewStatus.Failed));
        }
        [Test]
        public void LifecycleDuringHeldLoadPausesKnownSessionAtImmediateCallbackInstant()
        {
            using var f = new Fixture(); f.Open(); var old = f.Bridge.PuzzleView!; old.Apply(0, 1, DevelopmentTool.EmptyMarker, 0);
            f.Repo.HoldLoad = new TaskCompletionSource<SaveLoadResult<LocalProfileState>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var loading = f.Bridge.ReloadAsync(CancellationToken.None); Assert.That(f.Bridge.State.Status, Is.EqualTo(SavedReviewStatus.Loading));
            f.Clock.MonotonicMilliseconds = 175; var pausing = f.Bridge.SetSuspendedAsync(true, CancellationToken.None);
            Assert.That(old.IsPaused, Is.True); Assert.That(old.ElapsedAt(10000), Is.EqualTo(175));
            Assert.That(f.Bridge.State.CanMutate, Is.False); Assert.That(old.Apply(1, 1, DevelopmentTool.EmptyMarker, 176), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            int writes = f.Repo.Commits;
            f.Repo.HoldLoad.SetResult(SaveLoadResult<LocalProfileState>.Success(f.Repo.Stored!, f.Repo.Observation())); f.Repo.HoldLoad = null;
            f.Pump(loading); f.Pump(pausing); Assert.That(f.Bridge.PuzzleView, Is.Null); Assert.That(f.Bridge.State.IsDirty, Is.False);
            Assert.That(f.Repo.Commits, Is.EqualTo(writes)); Assert.That(f.Bridge.State.Status, Is.EqualTo(SavedReviewStatus.Ready));
        }
        [Test]
        public void SuspensionPreservesAcknowledgedRideAndResultWithoutReplayOrExtraWrite()
        {
            using var f = new Fixture(); f.Open(); f.Solve(); f.Pump(f.Bridge.TickAutosaveAsync(CancellationToken.None));
            var view = f.Bridge.PuzzleView!; Assert.That(f.Bridge.BeginTrainRide(view.AttemptId), Is.EqualTo(DevelopmentCommandStatus.Applied));
            int writes = f.Repo.Commits;
            f.Pump(f.Bridge.SetSuspendedAsync(true, CancellationToken.None)); Assert.That(f.Bridge.State.Status, Is.EqualTo(SavedReviewStatus.TrainRide));
            Assert.That(view.CompleteTrainRide(view.AttemptId), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            f.Pump(f.Bridge.SetSuspendedAsync(false, CancellationToken.None)); Assert.That(f.Bridge.State.Status, Is.EqualTo(SavedReviewStatus.TrainRide));
            Assert.That(view.CompleteTrainRide(view.AttemptId), Is.EqualTo(DevelopmentCommandStatus.Applied));
            f.Pump(f.Bridge.SetSuspendedAsync(true, CancellationToken.None)); f.Pump(f.Bridge.SetSuspendedAsync(false, CancellationToken.None));
            Assert.That(f.Bridge.State.Status, Is.EqualTo(SavedReviewStatus.Results)); Assert.That(f.Bridge.State.CanStartRide, Is.False);
            Assert.That(f.Repo.Commits, Is.EqualTo(writes)); Assert.That(f.Bridge.State.Progress!.BalancePatience, Is.EqualTo(15));
        }
        [Test]
        public void ConfirmedFreshLoadRetiresDirtySchedulerBeforeExplicitResumeOrNewStart()
        {
            using var f = new Fixture(); f.Open(); var old = f.Bridge.PuzzleView!; old.Apply(0, 1, DevelopmentTool.EmptyMarker, 0);
            Assert.That(f.Bridge.State.IsDirty, Is.True); f.Pump(f.Bridge.ReloadAsync(CancellationToken.None));
            Assert.That(f.Bridge.State.IsDirty, Is.False); Assert.That(f.Bridge.PuzzleView, Is.Null);
            int writes = f.Repo.Commits; f.Clock.MonotonicMilliseconds = 5000; f.Pump(f.Bridge.TickAutosaveAsync(CancellationToken.None));
            Assert.That(f.Repo.Commits, Is.EqualTo(writes)); f.Pump(f.Bridge.SelectPuzzleAsync(0, true, CancellationToken.None));
            Assert.That(f.Repo.Commits, Is.EqualTo(writes)); Assert.That(f.Bridge.PuzzleView!.CellAt(0, 1), Is.EqualTo(DevelopmentTool.Clear));
            Assert.That(f.Bridge.State.SelectedIndex, Is.Zero);
        }
        [Test]
        public void CompletionDuringDraftWriteWaitsForItsOwnSeparateAcknowledgement()
        {
            using var f = new Fixture(); f.Open(); f.Bridge.PuzzleView!.Apply(0, 1, DevelopmentTool.EmptyMarker, 0);
            f.Repo.Hold = new TaskCompletionSource<SaveCommitResult<LocalProfileState>>(TaskCreationOptions.RunContinuationsAsynchronously);
            f.Clock.MonotonicMilliseconds = 500; var draft = f.Bridge.TickAutosaveAsync(CancellationToken.None); f.Until(() => f.Repo.Commits == 2);
            f.Solve(); Assert.That(f.Bridge.State.Status, Is.EqualTo(SavedReviewStatus.CompletionPending));
            Assert.That(f.Bridge.State.CanStartRide, Is.False); f.Repo.Release(); f.Pump(draft);
            Assert.That(f.Bridge.State.CanStartRide, Is.False); Assert.That(f.Bridge.State.Progress!.BalancePatience, Is.Zero);
            f.Pump(f.Bridge.TickAutosaveAsync(CancellationToken.None)); Assert.That(f.Repo.Commits, Is.EqualTo(3));
            Assert.That(f.Bridge.State.CanStartRide, Is.True); Assert.That(f.Bridge.State.Progress!.BalancePatience, Is.EqualTo(15));
        }
        [Test]
        public void CancellingQueuedLifecycleFlushDoesNotCancelEarlierPhysicalWrite()
        {
            using var f = new Fixture(); f.Open(); var view = f.Bridge.PuzzleView!; view.Apply(0, 1, DevelopmentTool.EmptyMarker, 0);
            f.Repo.Hold = new TaskCompletionSource<SaveCommitResult<LocalProfileState>>(TaskCreationOptions.RunContinuationsAsynchronously);
            f.Clock.MonotonicMilliseconds = 500; var writing = f.Bridge.TickAutosaveAsync(CancellationToken.None); f.Until(() => f.Repo.Commits == 2);
            using var cancellation = new CancellationTokenSource(); f.Clock.MonotonicMilliseconds = 550;
            var pausing = f.Bridge.SetSuspendedAsync(true, cancellation.Token); Assert.That(view.IsPaused, Is.True); cancellation.Cancel();
            Assert.Catch<OperationCanceledException>(() => f.Pump(pausing)); Assert.That(writing.IsCompleted, Is.False);
            f.Repo.Release(); f.Pump(writing); Assert.That(f.Repo.Commits, Is.EqualTo(2)); Assert.That(f.Bridge.State.IsDirty, Is.True);
            Assert.That(f.Bridge.State.CanMutate, Is.False); Assert.That(view.ElapsedAt(10000), Is.EqualTo(550));
        }
        [Test]
        public void UnexpectedRequestFaultKeepsExplicitReloadReachableAndCannotReuseStaleRecovery()
        {
            using var f = new Fixture(); f.Open(); var old = f.Bridge.PuzzleView!; f.Repo.FailLoad = true;
            Assert.Throws<InvalidOperationException>(() => f.Pump(f.Bridge.ReloadAsync(CancellationToken.None)));
            Assert.That(f.Bridge.State.Status, Is.EqualTo(SavedReviewStatus.Failed)); Assert.That(f.Bridge.State.CanRetryLoad, Is.True);
            Assert.That(f.Bridge.State.CanCreateProfile || f.Bridge.State.CanRecover || f.Bridge.State.CanReset || f.Bridge.State.CanNavigate || f.Bridge.State.CanStartRide, Is.False);
            Assert.That(old.Apply(0, 1, DevelopmentTool.EmptyMarker, 0), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            f.Repo.FailLoad = false; f.Pump(f.Bridge.ReloadAsync(CancellationToken.None)); Assert.That(f.Bridge.PuzzleView, Is.Null);
            Assert.That(f.Bridge.State.CanNavigate, Is.True); Assert.That(f.Bridge.State.Status, Is.EqualTo(SavedReviewStatus.Ready));
        }
        [Test]
        public void BridgeWorkerEntryIsRejectedAndPublishedStatesRemainImmutable()
        {
            using var f = new Fixture(); f.Open(); var before = f.Bridge.State;
            Assert.Throws<InvalidOperationException>(() => Task.Run(() => f.Bridge.TickAutosaveAsync(CancellationToken.None)).GetAwaiter().GetResult());
            f.Bridge.PuzzleView!.Apply(0, 1, DevelopmentTool.EmptyMarker, 0);
            Assert.That(before.IsDirty, Is.False); Assert.That(f.Bridge.State.IsDirty, Is.True);
            Assert.That(f.Bridge.State.Version, Is.GreaterThan(before.Version)); Assert.That(before.Progress!.Generation, Is.EqualTo(2));
        }
        [Test]
        public void MissingResumeNeverFallsBackToStartAndNoStartupAttemptIsFabricated()
        {
            using var f = new Fixture(); f.Pump(f.Bridge.InitializeAsync(CancellationToken.None));
            f.Pump(f.Bridge.SelectPuzzleAsync(1, true, CancellationToken.None)); Assert.That(f.Bridge.PuzzleView, Is.Null); Assert.That(f.Repo.Commits, Is.Zero);
            Assert.That(f.Bridge.State.SelectedIndex, Is.EqualTo(-1));
        }
        [Test]
        public void DisposeRetiresViewsAndLateAcknowledgementCannotPublishResults()
        {
            using var f = new Fixture(); f.Open(); f.Solve(); var old = f.Bridge.PuzzleView!;
            f.Repo.Hold = new TaskCompletionSource<SaveCommitResult<LocalProfileState>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var saving = f.Bridge.TickAutosaveAsync(CancellationToken.None); f.Until(() => f.Repo.Commits == 2);
            f.Bridge.Dispose(); long version = f.Bridge.State.Version; f.Repo.Release(); f.Pump(saving);
            Assert.That(f.Bridge.State.Status, Is.EqualTo(SavedReviewStatus.Disposed)); Assert.That(f.Bridge.State.Version, Is.EqualTo(version));
            Assert.That(old.Apply(0, 1, DevelopmentTool.EmptyMarker, 100), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(f.Bridge.BeginTrainRide(old.AttemptId), Is.EqualTo(DevelopmentCommandStatus.Rejected));
        }
        [Test]
        public void MainThreadDispatcherQueuesWorkersAndRejectsWorkerDrainWithoutExecutingPublications()
        {
            object dispatcher = Activator.CreateInstance(Assembly.Load("STP.Platform").GetType("STP.Platform.MainThreadSaveDispatcher", true)!)!;
            var port = (ILocalSaveDispatcher)dispatcher; int owner = Thread.CurrentThread.ManagedThreadId, actual = 0;
            using var queued = new ManualResetEventSlim(); Task? publication = null;
            var worker = Task.Run(() => { publication = port.InvokeAsync(() => actual = Thread.CurrentThread.ManagedThreadId); queued.Set(); });
            Assert.That(queued.Wait(3000), Is.True); worker.GetAwaiter().GetResult(); Assert.That(actual, Is.Zero); Assert.That(publication!.IsCompleted, Is.False);
            var failure = Task.Run(() => Assert.Throws<TargetInvocationException>(() => dispatcher.GetType().GetMethod("Drain")!.Invoke(dispatcher, null))!.InnerException!.Message);
            Assert.That(failure.GetAwaiter().GetResult(), Is.EqualTo("SAVE_DISPATCH_OWNER_REQUIRED"));
            dispatcher.GetType().GetMethod("Drain")!.Invoke(dispatcher, null); publication.GetAwaiter().GetResult(); Assert.That(actual, Is.EqualTo(owner));
            ((IDisposable)dispatcher).Dispose();
        }
        [Test]
        public void DispatcherDisposeCompletesWaitingAndFuturePublicationsWithoutTheirActions()
        {
            object dispatcher = Activator.CreateInstance(Assembly.Load("STP.Platform").GetType("STP.Platform.MainThreadSaveDispatcher", true)!)!;
            var port = (ILocalSaveDispatcher)dispatcher; int calls = 0; Task? publication = null;
            Task.Run(() => { publication = port.InvokeAsync(() => calls++); }).GetAwaiter().GetResult();
            ((IDisposable)dispatcher).Dispose(); Assert.Throws<ObjectDisposedException>(() => publication!.GetAwaiter().GetResult());
            Assert.Throws<ObjectDisposedException>(() => port.InvokeAsync(() => calls++).GetAwaiter().GetResult()); Assert.That(calls, Is.Zero);
            ((IDisposable)dispatcher).Dispose();
        }
        [Test]
        public void LocalSaveClockUsesProcessMonotonicAndInvariantUtc()
        {
            var clock = (ILocalSessionClock)Activator.CreateInstance(Assembly.Load("STP.Platform").GetType("STP.Platform.LocalSaveClock", true)!)!;
            long before = clock.MonotonicMilliseconds; Thread.Sleep(2); Assert.That(clock.MonotonicMilliseconds, Is.GreaterThanOrEqualTo(before));
            Assert.That(clock.UtcNow, Does.Match(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{7}Z$"));
        }
    }
}
