using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;
using STP.Bootstrap;
using STP.Presentation.UI;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace STP.Tests.Bootstrap.PlayMode
{
    /// <summary>Real common installer/UI/atomic repository, with immutable imported FIXTURE_ONLY bytes.
    /// These controlled play sequences and synthetic thresholds are never human or production evidence.</summary>
    public sealed class RuntimeGameIntegrationTests
    {
        private const string Ui = "Assets/StammstreckenPuzzle/UI/";
        private const string Fixtures = "Assets/StammstreckenPuzzle/Tests/PlayMode/Bootstrap/RuntimeFixtures/";
        private const string BindingSha = "3baf99433918a2aa35e9edd7bdab988f9daaaf7c1e57ede5268c624e3935d9a1";
        private const string ReleaseSha = "3174e9d24f8ac6be040032601ba156ab51463df596f094adf04ac0c4b8dccf06";
        private const string UiSha = "fc0c2a3f02fcd00445a5c5a1ab1d1e8017657329c84447b619ff76c77b15a8ee";
        private static readonly string[] Paths = { "Calibration.json", "Campaign.json", "CampaignCompletion.json", "CampaignText.json", "Cosmetics.json", "EndlessCompletion.json",
            "Profiles/dienstbeginn.json", "Profiles/grosslage.json", "Profiles/regelbetrieb.json", "Profiles/stoerungslage.json", "Public.json", "RailwayAssets.json" };
        private GameObject? host;
        private MobilePreviewInstaller installer = null!;
        private PanelSettings panel = null!;
        private RenderTexture? texture;
        private string ownRoot = string.Empty;
        private bool ownsRoot;
        private ProductionGameComposition Runtime => installer.RuntimeComposition ?? throw new InvalidOperationException("RUNTIME_COMPOSITION_NOT_READY");
        private SavedPublic240ReviewSession Model => installer.Model ?? throw new InvalidOperationException("RUNTIME_MODEL_NOT_READY");

        [UnitySetUp]
        public IEnumerator SetUp()
        {
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
            // Short private path also exercises the real Windows archive-length guards.
            ownRoot = Path.Combine(Path.GetTempPath(), "r" + Guid.NewGuid().ToString("N").Substring(0, 6));
            Assert.That(Directory.Exists(ownRoot), Is.False);
            host = new GameObject("WP046 locked fixture runtime integration"); host.SetActive(false);
            installer = host.AddComponent<MobilePreviewInstaller>(); installer.ConfigureTestRoot(ownRoot); ownsRoot = true;
            // Original fixture budgets stay unchanged: seed1 exhausts ten attempts; the independently
            // probed seeds2/3 each verify on attempt1, the latter against the actual first Recent.
            installer.ConfigureTestEndlessSeeds(2, 3);
            panel = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<PanelSettings>(Ui + "Public240ReviewPanelSettings.asset"));
            panel.referenceResolution = new Vector2Int(390, 844); panel.match = 0;
            if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null)
            {
                texture = new RenderTexture(390, 844, 24, RenderTextureFormat.ARGB32);
                Assert.That(texture.Create(), Is.True); panel.targetTexture = texture;
            }
            var files = Paths.ToDictionary(path => path, path => AssetDatabase.LoadAssetAtPath<TextAsset>(Fixtures + path), StringComparer.Ordinal);
            Assert.That(files.Values.All(value => value != null), Is.True, "All twelve independently imported fixture payloads must exist.");
            installer.ConfigureContractTestRuntime(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Ui + "ProductionGame.uxml"),
                AssetDatabase.LoadAssetAtPath<StyleSheet>(Ui + "ProductionGame.uss"), AssetDatabase.LoadAssetAtPath<TextAsset>(Ui + "ProductionGameLocale.json"), panel,
                files, AssetDatabase.LoadAssetAtPath<TextAsset>(Fixtures + "Runtime.bindings.json"), BindingSha, ReleaseSha, ReleaseSha, UiSha);
            host.SetActive(true); installer.Document.rootVisualElement.style.width = 390; installer.Document.rootVisualElement.style.height = 844;
            yield return Await(installer.InitializeTask); yield return Idle();
            Assert.That(installer.Model, Is.Not.Null, installer.StartupDiagnostic);
            Assert.That(Model.State.Status, Is.EqualTo(SavedReviewStatus.NeedsInitialization), installer.StartupDiagnostic);
            Assert.That(Runtime.Profile.Use, Is.EqualTo(CatalogUse.ContractTest));
            Assert.That(Runtime.Profile.Namespace, Is.EqualTo("fixture-production-composition"));
            Assert.That(Runtime.Catalog.Puzzles.Count, Is.EqualTo(240)); Assert.That(installer.Candidate, Is.Null);
#else
            Assert.Ignore("The explicit fixture selector and private root exist only in Editor tests."); yield break;
#endif
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (host != null) UnityEngine.Object.Destroy(host);
            yield return null;
            if (installer != null) yield return Await(installer.ShutdownTask);
            if (texture != null) { texture.Release(); UnityEngine.Object.Destroy(texture); }
            if (panel != null) UnityEngine.Object.Destroy(panel);
            if (ownsRoot && Directory.Exists(ownRoot))
            {
                string prefix = Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
                Assert.That(Path.GetFullPath(ownRoot).StartsWith(prefix, StringComparison.OrdinalIgnoreCase), Is.True);
                Assert.That(Path.GetFileName(ownRoot).Length, Is.EqualTo(7)); Assert.That(Path.GetFileName(ownRoot), Does.StartWith("r"));
                Directory.Delete(ownRoot, true);
            }
            ownsRoot = false;
        }

        [UnityTest]
        public IEnumerator LockedRuntime_ProfileHintRestartPhysicalReloadAndOrPauseUseTheCommonGrid()
        {
            Assert.That(Directory.GetFiles(ownRoot, "save.json", SearchOption.AllDirectories), Is.Empty);
            Submit("home-play"); yield return Idle();
            Assert.That(Model.CanEnterPuzzle(0), Is.True); Assert.That(Model.CanEnterPuzzle(1), Is.False);
            Assert.That(Model.CurrentModeName, Is.EqualTo("FIRST_RUN"));
            Assert.That(Runtime.Persisted.Snapshot!.Payload.Levels, Is.Empty);
            Assert.That(Runtime.Persisted.Snapshot.Payload.Inventory.Ownership.Any(o => o.ItemId == "train-stammrot-default"), Is.True);
            Assert.That(Find<VisualElement>("puzzle-screen").resolvedStyle.display, Is.EqualTo(DisplayStyle.Flex));
            Assert.That(installer.Presenter.Grid!.CellButton(1, 1).worldBound.width, Is.GreaterThan(28));
#if UNITY_EDITOR
            using (var texts = ProductionCampaignText.ForContractTest(AssetDatabase.LoadAssetAtPath<TextAsset>(Fixtures + "CampaignText.json")))
                Assert.That(Find<Label>("puzzle-title").text, Is.EqualTo(texts.Resolve(0, "", "").Title));
#endif
            var identity = Model.State.CurrentIdentity!; var first = Model.PuzzleView!;
            Assert.That(Runtime.Metagame.HintCredits(Model.ConfirmedSnapshot!.Payload, identity), Is.EqualTo(3));
            Assert.That(first.TimerStarted, Is.False);
            Submit("puzzle-hint");
            yield return Until(() => Runtime.Metagame.HintCredits(Model.ConfirmedSnapshot!.Payload, identity) == 2 && Find<Button>("hint-close").enabledInHierarchy, "actual hint receipt/save acknowledgement");
            Assert.That(first.IsPaused, Is.True); Assert.That(first.TimerStarted, Is.False);
            Submit("hint-close"); yield return Idle();
            Guid oldAttempt = first.AttemptId;
            yield return Request(SavedReviewUiRequestKind.Restart);
            Assert.That(Model.PuzzleView!.AttemptId, Is.Not.EqualTo(oldAttempt));
            Assert.That(Runtime.Metagame.HintCredits(Model.ConfirmedSnapshot!.Payload, identity), Is.EqualTo(2));
            Submit("tool-1"); Submit(ProductionPuzzleGrid.CellName(1, 1)); yield return Idle();
            installer.SetPaused(true); installer.SetFocused(false); yield return Idle();
            installer.SetFocused(true); yield return Idle();
            Assert.That(Model.PuzzleView.IsPaused, Is.True, "Focus cannot cancel the actual application pause.");
            installer.SetPaused(false); yield return Idle();
            Assert.That(Model.PuzzleView.IsPaused, Is.False);
            Submit("puzzle-back"); yield return Idle(); yield return Request(SavedReviewUiRequestKind.Reload);
            Submit("home-play"); yield return Idle();
            Assert.That(Model.PuzzleView!.Puzzle.Identity, Is.EqualTo(identity));
            Assert.That(Model.PuzzleView.CellAt(1, 1), Is.EqualTo(DevelopmentTool.EmptyMarker));
            Assert.That(Runtime.Metagame.HintCredits(Model.ConfirmedSnapshot!.Payload, identity), Is.EqualTo(2));
            Assert.That(Directory.GetFiles(ownRoot, "save.json", SearchOption.AllDirectories).Length, Is.EqualTo(1));
            Assert.That(Model.ConfirmedSnapshot.Payload.Entitlements.Records, Is.Empty);
        }

        [UnityTest]
        public IEnumerator LockedRuntime_ActualUiSolveCommitsCalibratedResultBeforeRideAndContinuesOffline()
        {
            Submit("home-play"); yield return Idle();
            var view = Model.PuzzleView!; var identity = view.Puzzle.Identity; long before = Model.State.Progress!.BalancePatience;
            yield return SolveCurrentThroughUi();
            var level = Model.ConfirmedSnapshot!.Payload.Levels.Single(l => l.Identity.Equals(identity));
            Assert.That(level.FirstClear!.AttemptId, Is.EqualTo(view.AttemptId));
            Assert.That(level.FirstRunBestElapsedMs, Is.Not.Null);
            int expectedStars = SyntheticStars(level.FirstRunBestElapsedMs!.Value);
            Assert.That(level.StarsHighWater, Is.EqualTo(expectedStars));
            Assert.That(Model.State.Progress!.SelectedStars, Is.EqualTo(expectedStars));
            Assert.That(Model.State.Progress.BalancePatience - before, Is.EqualTo(15 + (expectedStars >= 2 ? 5 : 0) + (expectedStars >= 3 ? 10 : 0)));
            Assert.That(Model.CanEnterPuzzle(1), Is.True);
            Assert.That(Find<Button>("result-video").enabledInHierarchy, Is.False, "No native provider is composed for the explicit fixture runtime.");
            Submit("result-skip-bonus"); yield return Idle();
            long balance = Model.State.Progress.BalancePatience;
            Submit("result-next"); yield return Idle();
            Assert.That(Model.State.SelectedIndex, Is.EqualTo(1)); Assert.That(Model.CurrentModeName, Is.EqualTo("FIRST_RUN"));
            Assert.That(Model.PuzzleView!.TimerStarted, Is.False);
            Assert.That(Model.State.Progress.BalancePatience, Is.EqualTo(balance));
            yield return Request(SavedReviewUiRequestKind.Reload);
            Assert.That(Model.State.Progress!.FirstClearCount, Is.EqualTo(1));
            Assert.That(Model.ConfirmedSnapshot!.Payload.Levels.Single(l => l.Identity.Equals(identity)).StarsHighWater, Is.EqualTo(expectedStars));
        }

        [UnityTest]
        public IEnumerator LockedRuntime_FreeDailySettingsAndDefaultCosmeticsPersistWithoutProviderOrEntitlement()
        {
            yield return CreateProfile();
            Submit("home-daily"); yield return Idle();
            long before = Model.State.Progress!.BalancePatience;
            Submit("daily-free");
            yield return Until(() => Model.ConfirmedSnapshot?.Payload.Metagame?.Daily.Any(d => d.Slot == "FREE" && d.Terminal != null) == true && Find<Button>("daily-close").enabledInHierarchy, "free daily durable terminal");
            var daily = Model.ConfirmedSnapshot!.Payload.Metagame!.Daily.Single(d => d.Slot == "FREE");
            Assert.That(Model.State.Progress!.BalancePatience - before, Is.EqualTo(daily.AmountPatience));
            Assert.That(Find<Button>("daily-free").enabledInHierarchy, Is.False); Assert.That(Find<Button>("daily-video").enabledInHierarchy, Is.False);
            Submit("daily-close"); yield return Idle(); Submit("nav-profile"); Submit("profile-settings"); yield return Idle();
            bool previous = Model.ConfirmedSnapshot.Payload.Settings.HapticsEnabled ?? false;
            Submit("settings-haptics"); yield return Until(() => Model.ConfirmedSnapshot!.Payload.Settings.HapticsEnabled == !previous && Find<Button>("settings-close").enabledInHierarchy, "settings acknowledgement");
            Assert.That(Find<Button>("settings-remove-ads").enabledInHierarchy, Is.False); Assert.That(Find<Button>("settings-restore").enabledInHierarchy, Is.False);
            Submit("settings-close"); yield return Idle(); Submit("nav-depot"); yield return Idle();
            Assert.That(Find<Button>("cosmetic-action-train-stammrot-default").enabledInHierarchy, Is.False);
            Assert.That(Model.ConfirmedSnapshot.Payload.Inventory.SelectedItemIds, Does.Contain("train-stammrot-default"));
            yield return Request(SavedReviewUiRequestKind.Reload);
            Assert.That(Model.ConfirmedSnapshot!.Payload.Settings.HapticsEnabled, Is.EqualTo(!previous));
            Assert.That(Model.ConfirmedSnapshot.Payload.Metagame!.Daily.Single(d => d.Slot == "FREE").Terminal, Is.Not.Null);
            Assert.That(Model.ConfirmedSnapshot.Payload.Entitlements.Records, Is.Empty);
        }

        [UnityTest, Timeout(180000)]
        public IEnumerator LockedRuntime_RealSeasonUnlockCosmeticPurchaseAndV2EndlessResultRoundTrip()
        {
            yield return CreateProfile(); yield return CompleteFixtureSeasonThroughActualDomain();
            Assert.That(Model.State.Progress!.FirstClearCount, Is.EqualTo(240));
            Submit("nav-depot"); yield return Idle(); long balance = Model.State.Progress.BalancePatience;
            Submit("cosmetic-action-obj-wartebank-ohne-anschluss"); yield return Idle();
            Assert.That(Model.ConfirmedSnapshot!.Payload.Inventory.Ownership.Any(o => o.ItemId == "obj-wartebank-ohne-anschluss"), Is.True);
            Assert.That(Model.State.Progress.BalancePatience, Is.EqualTo(balance - 120));
            Submit("cosmetic-action-obj-wartebank-ohne-anschluss"); yield return Idle();
            Assert.That(Model.ConfirmedSnapshot.Payload.Inventory.SelectedItemIds, Does.Contain("obj-wartebank-ohne-anschluss"));
            Submit("cosmetic-action-obj-wartebank-ohne-anschluss"); yield return Idle();
            Assert.That(Model.ConfirmedSnapshot.Payload.Inventory.SelectedItemIds, Does.Not.Contain("obj-wartebank-ohne-anschluss"));
            Assert.That(Model.State.Progress.BalancePatience, Is.EqualTo(balance - 120));
            Submit("nav-home"); Submit("home-endless"); yield return Idle();
            yield return Until(() => Find<Button>("endless-profile-dienstbeginn").enabledInHierarchy, "settled modal profile action");
            Submit("endless-profile-dienstbeginn"); yield return Idle();
            Assert.That(Model.CurrentModeName, Is.EqualTo("ENDLESS"), Model.EndlessGenerationStatus);
            var view = Model.PuzzleView!; var identity = view.Puzzle.Identity;
            var active = Model.ConfirmedSnapshot!.Payload.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single();
            var parameters = (SearchGeneratorParameters)active.Descriptor.Parameters;
            Assert.That(parameters.StarCalibration, Is.Not.Null);
            Assert.That(installer.Presenter.IsPuzzleScreenActive, Is.True);
            Submit("puzzle-hint"); yield return Until(() => Runtime.Metagame.HintCredits(Model.ConfirmedSnapshot!.Payload, identity) == 2 && Find<Button>("hint-close").enabledInHierarchy, "endless hint acknowledgement");
            Submit("hint-close"); yield return Idle();
            yield return Request(SavedReviewUiRequestKind.Restart);
            Assert.That(Runtime.Metagame.HintCredits(Model.ConfirmedSnapshot!.Payload, identity), Is.EqualTo(2));
            long before = Model.State.Progress!.BalancePatience;
            yield return SolveCurrentThroughUi();
            var done = Model.ConfirmedSnapshot!.Payload.Endless.OpenEndless.OfType<OpenEndlessCompletion>().Single();
            Assert.That(done.RecordVersion, Is.EqualTo(2));
            Assert.That(done.Result, Is.Not.Null); Assert.That(done.Result!.AttemptId, Is.EqualTo(Model.PuzzleView!.AttemptId));
            Assert.That(done.Result.Stars, Is.EqualTo(parameters.StarCalibration!.Stars(done.Result.ActiveElapsedMs)));
            Assert.That(Model.State.Progress!.SelectedStars, Is.EqualTo(done.Result.Stars));
            Assert.That(Model.State.Progress.BalancePatience - before, Is.EqualTo(15 + (done.Result.Stars >= 2 ? 5 : 0) + (done.Result.Stars >= 3 ? 10 : 0)));
            Assert.That(Model.ConfirmedSnapshot.Payload.Levels.Any(l => l.Identity.Equals(identity)), Is.False);
            var physical = Runtime.Repository.LoadAsync(CancellationToken.None); yield return Await(physical);
            Assert.That(physical.Result.Succeeded, Is.True, physical.Result.ErrorCode);
            Assert.That(physical.Result.Snapshot!.Payload.Endless.OpenEndless.OfType<OpenEndlessCompletion>().Single().Result!.Stars, Is.EqualTo(done.Result.Stars));
            long resultBalance = Model.State.Progress.BalancePatience;
            Submit("result-skip-bonus"); yield return Idle();
            Assert.That(Model.ConfirmedSnapshot!.Payload.Endless.OpenEndless, Is.Empty);
            Assert.That(Model.State.Progress.BalancePatience, Is.EqualTo(resultBalance));
            Submit("result-next"); yield return Idle();
            yield return Until(() => Find<Button>("endless-profile-dienstbeginn").enabledInHierarchy, "second settled modal action");
            Submit("endless-profile-dienstbeginn"); yield return Idle();
            Assert.That(Model.ConfirmedSnapshot.Payload.Endless.HighestReservedOrdinal, Is.EqualTo("2"));
            Assert.That(Model.PuzzleView!.Puzzle.Identity, Is.Not.EqualTo(identity));
            Assert.That(Model.State.Progress!.FirstClearCount, Is.EqualTo(240));
        }

        private IEnumerator CompleteFixtureSeasonThroughActualDomain()
        {
            var original = Model.ConfirmedSnapshot!;
            var work = Task.Run(() =>
            {
                var json = new STP.Infrastructure.Content.CanonicalJsonAdapter();
                var ledger = new STP.Infrastructure.Persistence.SaveLedgerCodec(json);
                var progress = new LocalProgressPolicy(Runtime.Catalog, Runtime.Progress.Campaign, Runtime.Progress.Completion, json, ledger);
                var state = original.Payload;
                foreach (var input in Runtime.Catalog.Puzzles)
                {
                    var solution = PuzzleSolver.Solve(input.ToDomainDefinition(), new SolverLimits(1000000, 10000));
                    Assert.That(solution.Classification, Is.EqualTo(SolverClassification.UNIQUE));
                    var session = PuzzleSessionState.Create(input.ToDomainDefinition(), input.Identity.PuzzleId, input.Identity.HashProfile,
                        input.Identity.HashSha256, Guid.NewGuid(), SessionMode.FIRST_RUN, 0, null, null).Value!;
                    for (int i = 0; i < solution.Path.Count; i++)
                    {
                        var cell = solution.Path[i]; int a = i == 0 ? (int)input.A.Side : DirectionTo(cell, solution.Path[i - 1]);
                        int b = i == solution.Path.Count - 1 ? (int)input.B.Side : DirectionTo(cell, solution.Path[i + 1]);
                        var applied = PuzzleCommandHandler.Handle(session, new ApplyCellContent(Guid.NewGuid(), session.Revision, cell, (CellContent)Shape((1 << a) | (1 << b))), i * 10L);
                        Assert.That(applied.Error, Is.EqualTo(DomainError.NONE)); session = applied.State!;
                    }
                    Assert.That(session.Phase, Is.EqualTo(SessionPhase.SolvedPendingCommit));
                    state = progress.Complete(state, session, original.Generation + 1, "2026-10-21T00:00:00Z").State;
                }
                return progress.PrepareForSave(state, original.Generation + 1);
            });
            yield return Await(work);
            var write = Runtime.Repository.CommitAsync(new SaveSnapshot<LocalProfileState>(original.Profile, original.Catalog, original.Generation + 1, work.Result), original.Generation, CancellationToken.None);
            yield return Await(write); Assert.That(write.Result.Succeeded, Is.True, write.Result.ErrorCode);
            yield return Request(SavedReviewUiRequestKind.Reload);
        }
        private IEnumerator SolveCurrentThroughUi()
        {
            var view = Model.PuzzleView!;
            var solved = PuzzleSolver.Solve(view.Puzzle.ToDomainDefinition(), new SolverLimits(1000000, 10000));
            Assert.That(solved.Classification, Is.EqualTo(SolverClassification.UNIQUE));
            for (int i = 0; i < solved.Path.Count; i++)
            {
                var cell = solved.Path[i]; int a = i == 0 ? (int)view.EndpointA.Edge : DirectionTo(cell, solved.Path[i - 1]);
                int b = i == solved.Path.Count - 1 ? (int)view.EndpointB.Edge : DirectionTo(cell, solved.Path[i + 1]);
                Submit("tool-" + (int)Shape((1 << a) | (1 << b))); Submit(ProductionPuzzleGrid.CellName(cell.X, cell.Y)); yield return Idle();
            }
            bool sawCommittedRide = Model.State.Status == SavedReviewStatus.TrainRide;
            float deadline = Time.realtimeSinceStartup + 30;
            while (Model.State.Status != SavedReviewStatus.Results && Time.realtimeSinceStartup < deadline)
            {
                if (Model.State.Status == SavedReviewStatus.TrainRide)
                {
                    sawCommittedRide = true;
                    Assert.That(Runtime.Persisted.CanPresentResults, Is.True);
                    Assert.That(Model.ConfirmedSnapshot!.Generation, Is.GreaterThan(0));
                }
                yield return null;
            }
            Assert.That(sawCommittedRide, Is.True, "Actual UI solve must pass through the acknowledged path animation.");
            Assert.That(Model.State.Status, Is.EqualTo(SavedReviewStatus.Results)); yield return Idle();
            CollectionAssert.AreEqual(solved.Path.Select(c => new DevelopmentCell(c.X, c.Y)).ToArray(), view.BuiltPath);
        }
        private static int SyntheticStars(long elapsed) => elapsed <= 1000 ? 3 : elapsed <= 2000 ? 2 : 1;
        private IEnumerator CreateProfile() => Request(SavedReviewUiRequestKind.CreateProfile);
        private IEnumerator Request(SavedReviewUiRequestKind kind)
        { yield return Await(installer.RunRequestAsync(SavedReviewUiRequest.Bound(kind, Model.State.Version))); yield return Idle(); }
        private IEnumerator Idle()
        {
            float deadline = Time.realtimeSinceStartup + 25;
            do { yield return null; } while ((!installer.PendingOperation.IsCompleted || installer.Presenter.PendingRequest != null || installer.Presenter.IsResultContinuationPending || installer.Model?.State.IsDirty == true) && Time.realtimeSinceStartup < deadline);
            Assert.That(installer.PendingOperation.IsCompleted, Is.True, "Common installer/user/lifecycle work did not settle.");
            Assert.That(installer.PendingOperation.IsFaulted, Is.False, installer.PendingOperation.Exception?.GetBaseException().ToString());
            Assert.That(installer.Presenter.PendingRequest, Is.Null); Assert.That(installer.Presenter.IsResultContinuationPending, Is.False);
            Assert.That(installer.Presenter.HasRequestFault, Is.False);
        }
        private static IEnumerator Await(Task task)
        {
            float deadline = Time.realtimeSinceStartup + 60;
            while (!task.IsCompleted && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(task.IsCompleted, Is.True); if (task.IsFaulted) throw task.Exception!;
            Assert.That(task.IsCanceled, Is.False);
        }
        private static IEnumerator Until(Func<bool> condition, string boundary)
        {
            float deadline = Time.realtimeSinceStartup + 25;
            while (!condition() && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(condition(), Is.True, boundary);
        }
        private T Find<T>(string name) where T : VisualElement => installer.Document.rootVisualElement.Q<T>(name) ?? throw new InvalidOperationException(name);
        private void Submit(string name)
        {
            var button = Find<Button>(name); Assert.That(button.enabledInHierarchy, Is.True, name);
            using (var evt = NavigationSubmitEvent.GetPooled()) { evt.target = button; button.SendEvent(evt); }
        }
        private static int DirectionTo(CellCoordinate from, CellCoordinate to) => to.X > from.X ? 1 : to.X < from.X ? 3 : to.Y > from.Y ? 2 : 0;
        private static DevelopmentTool Shape(int ports) => ports switch
        { 5 => DevelopmentTool.TrackNS, 10 => DevelopmentTool.TrackEW, 3 => DevelopmentTool.TrackNE, 6 => DevelopmentTool.TrackES, 12 => DevelopmentTool.TrackSW, 9 => DevelopmentTool.TrackWN, _ => throw new InvalidOperationException("Invalid actual solved-path ports") };
    }
}
