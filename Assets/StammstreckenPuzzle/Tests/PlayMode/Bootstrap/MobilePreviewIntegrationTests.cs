using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// <summary>Actual installer, actual public catalog, actual atomic saves and actual UI commands.</summary>
    public sealed class MobilePreviewIntegrationTests
    {
        private GameObject? host;
        private MobilePreviewInstaller installer = null!;
        private PanelSettings panel = null!;
        private RenderTexture? texture;
        private string ownRoot = "";
        private bool ownsRoot;
        private CancellationTokenSource? seasonSetupCancellation;
        private Task? seasonSetupWork;
        private const string U = "Assets/StammstreckenPuzzle/UI/";

        [UnitySetUp]
        public IEnumerator SetUp()
        {
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
            ownRoot = Path.Combine(Path.GetTempPath(), "s46-" + Guid.NewGuid().ToString("N").Substring(0, 6));
            Assert.That(Directory.Exists(ownRoot), Is.False);
            host = new GameObject("WP046 actual mobile preview integration");
            host.SetActive(false);
            installer = host.AddComponent<MobilePreviewInstaller>();
            installer.ConfigureTestRoot(ownRoot);
            ownsRoot = true;
            panel = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<PanelSettings>(U + "Public240ReviewPanelSettings.asset"));
            panel.referenceResolution = new Vector2Int(390, 844); panel.match = 0;
            if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null)
            {
                texture = new RenderTexture(390, 844, 24, RenderTextureFormat.ARGB32);
                Assert.That(texture.Create(), Is.True); panel.targetTexture = texture;
            }
            installer.Configure(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(U + "ProductionGame.uxml"),
                AssetDatabase.LoadAssetAtPath<StyleSheet>(U + "ProductionGame.uss"),
                AssetDatabase.LoadAssetAtPath<TextAsset>(U + "ProductionGameLocale.json"), panel,
                AssetDatabase.LoadAssetAtPath<TextAsset>(U + "Public240.public.json"),
                AssetDatabase.LoadAssetAtPath<TextAsset>(U + "ReviewCampaign.catalog.json"),
                AssetDatabase.LoadAssetAtPath<TextAsset>(U + "ReviewCompletion.catalog.json"),
                AssetDatabase.LoadAssetAtPath<TextAsset>(U + "RailwayAssets.json"));
            host.SetActive(true);
            installer.Document.rootVisualElement.style.width = 390;
            installer.Document.rootVisualElement.style.height = 844;
            yield return Await(installer.InitializeTask);
            yield return null;
            Assert.That(installer.Model, Is.Not.Null);
            Assert.That(installer.Model!.State.Status, Is.EqualTo(SavedReviewStatus.NeedsInitialization));
#else
            Assert.Ignore("Guarded private filesystem injection is available only in Editor tests.");
            yield break;
#endif
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (seasonSetupCancellation != null)
            {
                seasonSetupCancellation.Cancel();
                var pending = seasonSetupWork;
                if (pending != null)
                {
                    float cleanupDeadline = Time.realtimeSinceStartup + 25;
                    while (!pending.IsCompleted && Time.realtimeSinceStartup < cleanupDeadline) yield return null;
                    Assert.That(pending.IsCompleted, Is.True, "Owned season fixture worker must stop before host/root teardown.");
                    if (pending.IsFaulted) _ = pending.Exception;
                }
                seasonSetupCancellation.Dispose(); seasonSetupCancellation = null; seasonSetupWork = null;
            }
            var retainedInstaller = installer;
            if (host != null) UnityEngine.Object.Destroy(host);
            yield return null;
            // Unity's destroyed-object equality must not skip the still-owned managed shutdown task.
            if (retainedInstaller is not null) yield return Await(retainedInstaller.ShutdownTask);
            if (texture != null) { texture.Release(); UnityEngine.Object.Destroy(texture); }
            if (panel != null) UnityEngine.Object.Destroy(panel);
            if (ownsRoot && Directory.Exists(ownRoot))
            {
                string prefix = Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
                Assert.That(Path.GetFullPath(ownRoot).StartsWith(prefix, StringComparison.OrdinalIgnoreCase), Is.True);
                Assert.That(Path.GetFileName(ownRoot), Does.StartWith("s46-"));
                Directory.Delete(ownRoot, true);
            }
            ownsRoot = false;
        }

        [UnityTest]
        public IEnumerator StartupCreatesOnlyOnAction_AndFirstPuzzleUsesDirectTools()
        {
            Assert.That(Directory.Exists(ownRoot) ? Directory.GetFiles(ownRoot, "*", SearchOption.AllDirectories) : Array.Empty<string>(), Is.Empty);
            yield return Capture("home");
            Submit("home-play"); yield return Idle(); yield return null;
            Assert.That(installer.Model!.PuzzleView, Is.Not.Null);
            var view = installer.Model.PuzzleView!;
            Assert.That(view.TimerStarted, Is.False);
            Assert.That(Find<VisualElement>("puzzle-screen").resolvedStyle.display, Is.EqualTo(DisplayStyle.Flex));
            Submit("tool-1"); yield return null;
            Assert.That(view.TimerStarted, Is.False, "Choosing a tool is not a cell mutation.");
            Submit(ProductionPuzzleGrid.CellName(1, 1));
            Assert.That(view.CellAt(1, 1), Is.EqualTo(DevelopmentTool.EmptyMarker));
            Assert.That(view.TimerStarted, Is.True);
            yield return Idle();
            Submit(ProductionPuzzleGrid.CellName(1, 1));
            Assert.That(view.CellAt(1, 1), Is.EqualTo(DevelopmentTool.Clear));
            Assert.That(installer.Presenter.ActiveTool, Is.EqualTo(DevelopmentTool.EmptyMarker));
            yield return Idle();
            Assert.That(Find<VisualElement>("production-root").Query<Button>().ToList().Any(x => x.name.Contains("batch") || x.name.Contains("multi")), Is.False);
        }

        [UnityTest]
        public IEnumerator TenByTen_AllEightToggleTools_ReplaceUndoAutosave_DoNotMoveTargets()
        {
            yield return CreateProfile();
            int index = installer.Model!.Puzzles.ToList().FindIndex(p => p.Width == 10 && p.Height == 10);
            Assert.That(index, Is.GreaterThanOrEqualTo(0));
            yield return Select(index);
            // Enter the already selected editing view through the normal resume action.
            Submit("home-play"); yield return Idle(); yield return null;
            var grid = installer.Presenter.Grid!; var cell = grid.CellButton(1, 1);
            Rect beforeCell = cell.worldBound, beforeDock = Find<VisualElement>("tool-dock").worldBound;
            Assert.That(beforeCell.width, Is.GreaterThan(28), "10x10 must occupy the actual phone viewport, not collapse to its minimum height.");
            foreach (DevelopmentTool tool in new[] { DevelopmentTool.EmptyMarker, DevelopmentTool.OccupiedMarker,
                DevelopmentTool.TrackNS, DevelopmentTool.TrackEW, DevelopmentTool.TrackNE, DevelopmentTool.TrackES, DevelopmentTool.TrackSW, DevelopmentTool.TrackWN })
            {
                Submit("tool-" + (int)tool); Submit(cell.name);
                Assert.That(installer.Model.PuzzleView!.CellAt(1, 1), Is.EqualTo(tool));
                yield return StableIdle(cell, beforeCell, beforeDock);
                Assert.That(installer.Presenter.Grid!.CellButton(1, 1), Is.SameAs(cell));
                Submit(cell.name);
                Assert.That(installer.Model.PuzzleView.CellAt(1, 1), Is.EqualTo(DevelopmentTool.Clear));
                yield return StableIdle(cell, beforeCell, beforeDock);
                Assert.That(installer.Presenter.ActiveTool, Is.EqualTo(tool));
            }
            Submit("tool-3"); Submit(cell.name); yield return StableIdle(cell, beforeCell, beforeDock);
            Submit("tool-1"); Submit(cell.name); yield return StableIdle(cell, beforeCell, beforeDock);
            Assert.That(installer.Model.PuzzleView!.CellAt(1, 1), Is.EqualTo(DevelopmentTool.EmptyMarker));
            Submit("puzzle-undo"); yield return StableIdle(cell, beforeCell, beforeDock);
            Assert.That(installer.Model.PuzzleView.CellAt(1, 1), Is.EqualTo(DevelopmentTool.TrackNS));
            yield return Capture("puzzle-10x10");
        }

        [UnityTest]
        public IEnumerator SixByFive_RealSolvedPath_IsCommittedBeforeRide_ThenResultAndNext()
        {
            yield return CreateProfile();
            int index = installer.Model!.Puzzles.ToList().FindIndex(p => p.Width == 6 && p.Height == 5);
            Assert.That(index, Is.GreaterThanOrEqualTo(0));
            yield return Select(index); Submit("home-play"); yield return Idle(); yield return null;
            var view = installer.Model.PuzzleView!;
            Assert.That(installer.Presenter.Grid!.CellButton(1, 1).worldBound.width, Is.GreaterThan(45), "6x5 must be legible and directly touchable.");
            yield return Capture("puzzle-6x5");
            // Solver is used only by the test to feed actual UI commands, never by the product presenter.
            var solved = PuzzleSolver.Solve(view.Puzzle.ToDomainDefinition(), new SolverLimits(1000000, 10000));
            Assert.That(solved.Classification, Is.EqualTo(SolverClassification.UNIQUE));
            var path = solved.Path;
            for (int i = 0; i < path.Count; i++)
            {
                var cell = path[i];
                int inPort = i == 0 ? (int)view.EndpointA.Edge : DirectionTo(cell, path[i - 1]);
                int outPort = i == path.Count - 1 ? (int)view.EndpointB.Edge : DirectionTo(cell, path[i + 1]);
                DevelopmentTool tool = Shape((1 << inPort) | (1 << outPort));
                Submit("tool-" + (int)tool); Submit(ProductionPuzzleGrid.CellName(cell.X, cell.Y));
                yield return Idle();
            }
            bool sawRide = false;
            float deadline = Time.realtimeSinceStartup + 25;
            while (installer.Model.State.Status != SavedReviewStatus.Results && Time.realtimeSinceStartup < deadline)
            {
                if (installer.Model.State.Status == SavedReviewStatus.TrainRide)
                {
                    sawRide = true;
                    Assert.That(installer.Model.State.Progress!.FirstClearCount, Is.EqualTo(1));
                    Assert.That(Directory.GetFiles(ownRoot, "save.json", SearchOption.AllDirectories).Length, Is.EqualTo(1));
                }
                yield return null;
            }
            Assert.That(sawRide, Is.True, "The actual host must animate the acknowledged player's path.");
            Assert.That(installer.Model.State.Status, Is.EqualTo(SavedReviewStatus.Results));
            CollectionAssert.AreEqual(path.Select(p => new DevelopmentCell(p.X, p.Y)).ToArray(), view.BuiltPath);
            Assert.That(installer.Model.State.Progress!.FirstClearCount, Is.EqualTo(1));
            yield return Capture("result");
            Submit("result-next"); yield return Idle(); yield return null;
            Assert.That(installer.Model.PuzzleView!.AttemptId, Is.Not.EqualTo(view.AttemptId));
            Assert.That(installer.Model.State.Status, Is.EqualTo(SavedReviewStatus.Ready));
            Assert.That(installer.Model.PuzzleView.TimerStarted, Is.False);
        }

        private IEnumerator CreateProfile()
        { yield return Await(installer.RunRequestAsync(SavedReviewUiRequest.Bound(SavedReviewUiRequestKind.CreateProfile, installer.Model!.State.Version))); yield return Idle(); }

        [UnityTest]
        public IEnumerator Candidate_HintDisplayCommitsOnce_AndCloseRestoresAutosaveWithSequentialUnlocks()
        {
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
            yield return ConfigureCandidate();
            Submit("home-play");yield return Idle();yield return null;
            Assert.That(installer.Model!.CanEnterPuzzle(0),Is.True);Assert.That(installer.Model.CanEnterPuzzle(1),Is.False);
            var candidate=installer.Candidate!;var view=installer.Model.PuzzleView!;
            yield return Capture("candidate-puzzle");
            var identity=installer.Model.State.CurrentIdentity!;
            Assert.That(candidate.Metagame.HintCredits(candidate.Persisted.Snapshot!.Payload,identity),Is.EqualTo(3));
            Submit("puzzle-hint");
            float deadline=Time.realtimeSinceStartup+15;
            while(candidate.Metagame.HintCredits(candidate.Persisted.Snapshot!.Payload,identity)==3&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(installer.Presenter.IsHelpOpen,Is.True);
            Assert.That(view.IsPaused,Is.True);Assert.That(view.TimerStarted,Is.False);
            Assert.That(candidate.Metagame.HintCredits(candidate.Persisted.Snapshot!.Payload,identity),Is.EqualTo(2),"A real visible clue must consume one durable credit.");
            while(!Find<Button>("hint-close").enabledInHierarchy&&Time.realtimeSinceStartup<deadline)yield return null;
            yield return Capture("candidate-hint");
            Submit("hint-close");yield return Idle();yield return null;
            Assert.That(view.IsPaused,Is.False);
            long generation=candidate.Persisted.Snapshot.Generation;
            Submit("tool-1");Submit(ProductionPuzzleGrid.CellName(1,1));yield return Idle();
            Assert.That(view.CellAt(1,1),Is.EqualTo(DevelopmentTool.EmptyMarker));
            Assert.That(candidate.Persisted.Snapshot.Generation,Is.GreaterThan(generation),"Closing hint must restore the host's autosave gate.");
            Assert.That(candidate.Metagame.HintCredits(candidate.Persisted.Snapshot.Payload,identity),Is.EqualTo(2));
            yield return Await(installer.RunRequestAsync(SavedReviewUiRequest.Bound(SavedReviewUiRequestKind.Restart,installer.Model.State.Version)));yield return Idle();
            Assert.That(candidate.Metagame.HintCredits(candidate.Persisted.Snapshot.Payload,identity),Is.EqualTo(2),"Restart cannot refill identity-bound credits.");
            Submit("puzzle-back");yield return Idle();Submit("home-daily");yield return Idle();
            long beforeDaily=installer.Model.State.Progress!.BalancePatience;
            Submit("daily-free");deadline=Time.realtimeSinceStartup+15;
            while(candidate.Persisted.Snapshot.Payload.Metagame!.Daily.All(d=>d.Terminal==null)&&Time.realtimeSinceStartup<deadline)yield return null;
            var claim=candidate.Persisted.Snapshot.Payload.Metagame!.Daily.Single(d=>d.Slot=="FREE");
            Assert.That(claim.Terminal,Is.Not.Null);yield return null;
            Assert.That(Find<Button>("daily-free").enabledInHierarchy,Is.False,"A confirmed daily claim cannot be claimed twice.");
            while(!Find<Button>("daily-close").enabledInHierarchy&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(Find<Button>("daily-close").enabledInHierarchy,Is.True,"Wait for the actual application acknowledgement before reading the UI projection.");
            Assert.That(installer.Model.State.Progress!.BalancePatience-beforeDaily,Is.EqualTo(claim.AmountPatience));
            yield return Capture("candidate-daily");Submit("daily-close");yield return Idle();
            Submit("nav-profile");Submit("profile-settings");yield return Idle();
            bool previousHaptics=candidate.Persisted.Snapshot.Payload.Settings.HapticsEnabled??false;
            Submit("settings-haptics");deadline=Time.realtimeSinceStartup+15;
            while((candidate.Persisted.Snapshot.Payload.Settings.HapticsEnabled??false)==previousHaptics&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(candidate.Persisted.Snapshot.Payload.Settings.HapticsEnabled,Is.EqualTo(!previousHaptics));
            while(!Find<Button>("settings-close").enabledInHierarchy&&Time.realtimeSinceStartup<deadline)yield return null;
            yield return Capture("candidate-settings");Submit("settings-close");yield return Idle();
            Assert.That(installer.Model.PuzzleView!.IsPaused,Is.True,"Closing a settings modal on the profile must not resume the background puzzle timer.");
            Submit("nav-depot");yield return null;yield return Capture("candidate-collection");
            Assert.That(candidate.Persisted.Snapshot.Payload.Inventory.Ownership.Any(o=>o.ItemId=="train-stammrot-default"),Is.True);
#else
            yield break;
#endif
        }

        [UnityTest]
        public IEnumerator Candidate_FirstClearRecordsAndOptionalBonus_ContinueOfflineIntoNextPuzzle()
        {
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
            yield return ConfigureCandidate();Submit("home-play");yield return Idle();yield return null;
            var view=installer.Model!.PuzzleView!;var identity=view.Puzzle.Identity;
            Assert.That(installer.Model.CurrentModeName,Is.EqualTo("FIRST_RUN"));
            var solved=PuzzleSolver.Solve(view.Puzzle.ToDomainDefinition(),new SolverLimits(1000000,10000));
            Assert.That(solved.Classification,Is.EqualTo(SolverClassification.UNIQUE));
            for(int i=0;i<solved.Path.Count;i++)
            {
                var cell=solved.Path[i];int a=i==0?(int)view.EndpointA.Edge:DirectionTo(cell,solved.Path[i-1]);
                int b=i==solved.Path.Count-1?(int)view.EndpointB.Edge:DirectionTo(cell,solved.Path[i+1]);
                Submit("tool-"+(int)Shape((1<<a)|(1<<b)));Submit(ProductionPuzzleGrid.CellName(cell.X,cell.Y));yield return Idle();
            }
            float deadline=Time.realtimeSinceStartup+25;
            while(installer.Model.State.Status!=SavedReviewStatus.Results&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(installer.Model.State.Status,Is.EqualTo(SavedReviewStatus.Results));
            var state=installer.Model.ConfirmedSnapshot!.Payload;var level=state.Levels.Single(l=>l.Identity.Equals(identity));
            Assert.That(level.FirstClear!.AttemptId,Is.EqualTo(view.AttemptId));
            Assert.That(level.StarsHighWater,Is.EqualTo(1),"Uncalibrated DRAFT time limits must not award invented stars.");
            Assert.That(level.FirstRunBestElapsedMs,Is.Not.Null);Assert.That(level.RevisionBestElapsedMs,Is.Null);
            Assert.That(installer.Model.CanEnterPuzzle(1),Is.True);
            Assert.That(Find<Button>("result-video").enabledInHierarchy,Is.False,"Editor unavailable ads cannot enable a rewarded request.");
            Assert.That(Find<Button>("result-skip-bonus").enabledInHierarchy,Is.True);
            long balance=installer.Model.State.Progress!.BalancePatience;
            Submit("result-skip-bonus");yield return null;yield return null;
            Assert.That(installer.Model.State.Progress.BalancePatience,Is.EqualTo(balance));
            Assert.That(installer.Model.ConfirmedSnapshot.Payload.RewardClaims.OfType<PostClearRewardClaim>(),Is.Empty,
                "Declining an unreserved campaign video must not invent a provider operation or reward terminal.");
            yield return Capture("candidate-result");Submit("result-next");yield return Idle();yield return null;
            Assert.That(installer.Model.State.SelectedIndex,Is.EqualTo(1));
            Assert.That(installer.Model.PuzzleView!.TimerStarted,Is.False);
            yield return Select(0);
            Assert.That(installer.Model.CurrentModeName,Is.EqualTo("PRACTICE"),"Revision must remain locked until the full season is cleared.");
#else
            yield break;
#endif
        }
        [UnityTest, Timeout(180000)]
        public IEnumerator Candidate_AllTwentyMapRoutes_KeepTheirWorldAndCanvasAcrossHomeAndRefresh()
        {
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
            yield return ConfigureCandidate(); yield return CreateProfile();
            Assert.That(installer.Model!.State.Progress!.FirstClearCount, Is.Zero);
            Submit("home-map"); yield return Idle(); yield return null; yield return null;
            string output = UiQualityDirectory("world-routes");
            var records = new System.Collections.Generic.List<string> {
                "routeIndex\trouteId\texpectedWorld\tbeforeWorld\tafterWorld\tstage\tcanvasEvidence\tbeforePngSha256\tafterPngSha256\tregionWidth\tregionHeight"
            };
            for (int routeIndex = 0; routeIndex < 20; routeIndex++)
            {
                if (routeIndex > 0) { Submit("map-next"); yield return Idle(); yield return null; yield return null; }
                var art = Find<VisualElement>("map-art").Q<RailwayIllustrationElement>();
                Assert.That(art, Is.Not.Null);
                var route = UiQualityField<RailwayRouteComposition>(art!, "composition");
                int expectedWorld = routeIndex / 4;
                Assert.That(route.id, Is.EqualTo(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "s1-route-{0:00}-{1:00}", expectedWorld + 1, routeIndex % 4 + 1)));
                Assert.That(route.section, Is.EqualTo(expectedWorld + 1));
                int beforeWorld = UiQualityField<int>(art!, "world");
                Assert.That(beforeWorld, Is.EqualTo(expectedWorld));
                int stage = UiQualityField<int>(art!, "clearedStage");
                string[] props = UiQualityProps(route);
                Rect beforeBounds = art!.worldBound;
                UiQualityRegion? before = texture == null ? null : UiQualityReadRegion(art);
                if (before != null) Assert.That(before.Pixels.Distinct().Take(5).Count(), Is.GreaterThanOrEqualTo(4), "A blank Canvas is not a rendered map proof.");

                // These are the actual bound UI navigation actions, not a direct renderer imitation.
                Submit("map-back"); yield return Idle();
                Assert.That(Find<VisualElement>("home-screen").resolvedStyle.display, Is.EqualTo(DisplayStyle.Flex));
                Submit("home-map"); yield return Idle();
                installer.Presenter.Refresh(); yield return null; yield return null;
                Assert.That(Find<VisualElement>("map-screen").resolvedStyle.display, Is.EqualTo(DisplayStyle.Flex));
                var returned = Find<VisualElement>("map-art").Q<RailwayIllustrationElement>();
                Assert.That(returned, Is.SameAs(art));
                Assert.That(UiQualityField<RailwayRouteComposition>(returned!, "composition"), Is.SameAs(route));
                Assert.That(UiQualityField<int>(returned!, "world"), Is.EqualTo(expectedWorld), "Home/Refresh must not replace a browsed route's base world with the progress world.");
                Assert.That(UiQualityField<int>(returned!, "clearedStage"), Is.EqualTo(stage));
                CollectionAssert.AreEqual(props, UiQualityProps(route)); AssertRect(returned!.worldBound, beforeBounds);
                UiQualityRegion? after = texture == null ? null : UiQualityReadRegion(returned);
                if (before != null && after != null)
                {
                    Assert.That(after.Width, Is.EqualTo(before.Width)); Assert.That(after.Height, Is.EqualTo(before.Height));
                    CollectionAssert.AreEqual(before.Pixels, after.Pixels, "Compare the original decorative Canvas region, excluding changing focus/buttons outside map-art.");
                    if (routeIndex % 4 == 0)
                    {
                        string name = "map-world-" + (expectedWorld + 1).ToString("00", System.Globalization.CultureInfo.InvariantCulture);
                        File.WriteAllBytes(Path.Combine(output, name + "-before-region.png"), before.Png);
                        File.WriteAllBytes(Path.Combine(output, name + "-after-region.png"), after.Png);
                        UiQualityWriteCanvas(Path.Combine(output, name + "-after-navigation.png"));
                    }
                }
                records.Add(string.Join("\t", routeIndex, route.id, expectedWorld, beforeWorld,
                    UiQualityField<int>(returned!, "world"), stage, texture != null,
                    before == null ? "NOT_CAPTURED_GRAPHICS_NULL" : UiQualityHash(before.Png),
                    after == null ? "NOT_CAPTURED_GRAPHICS_NULL" : UiQualityHash(after.Png),
                    before?.Width ?? 0, before?.Height ?? 0));
            }
            Assert.That(installer.Model.State.Progress!.FirstClearCount, Is.Zero, "Browsing all routes must not mint campaign progress or require a synthetic season.");
            File.WriteAllLines(Path.Combine(output, "map-evidence.tsv"), records);
#else
            yield break;
#endif
        }

        [UnityTest, Timeout(120000)]
        public IEnumerator TenByTen_ExplicitRetapCaptionFits_AndAllCellAndToolRectsStayFixed()
        {
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
            // Existing isolated public-review path: real installer/domain/save/UI, no production unlock bypass.
            yield return CreateProfile();
            int index = installer.Model!.Puzzles.ToList().FindIndex(p => p.Width == 10 && p.Height == 10);
            Assert.That(index, Is.GreaterThanOrEqualTo(0)); yield return Select(index);
            Submit("home-play"); yield return Idle(); yield return null; yield return null;
            var caption = installer.Document.rootVisualElement.Q<Label>(className: "tool-caption");
            Assert.That(caption, Is.Not.Null);
            const string expected = "Mit gleichem Werkzeug dieselbe Zelle erneut tippen: leeren.";
            Assert.That(caption!.text, Is.EqualTo(expected));
            Assert.That(caption.resolvedStyle.whiteSpace, Is.EqualTo(WhiteSpace.Normal));
            Assert.That(caption.resolvedStyle.fontSize, Is.GreaterThanOrEqualTo(10), "Do not make the clarification fit by shrinking the agreed caption.");
            Assert.That(caption.contentRect.width, Is.GreaterThan(0));
            Vector2 measured = caption.MeasureTextSize(expected, caption.contentRect.width,
                VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
            Assert.That(measured.y, Is.LessThanOrEqualTo(caption.contentRect.height + .5f), "All wrapped lines must fit the actual allocated caption box.");
            Assert.That(caption.isElided, Is.False);
            Rect row = caption.parent.worldBound, box = caption.worldBound;
            Assert.That(box.xMin, Is.GreaterThanOrEqualTo(row.xMin - .5f)); Assert.That(box.xMax, Is.LessThanOrEqualTo(row.xMax + .5f));
            Assert.That(box.yMin, Is.GreaterThanOrEqualTo(row.yMin - .5f)); Assert.That(box.yMax, Is.LessThanOrEqualTo(row.yMax + .5f));
            var grid = installer.Presenter.Grid!; var cell = grid.CellButton(1, 1);
            Rect cellBefore = cell.worldBound, dockBefore = Find<VisualElement>("tool-dock").worldBound;
            Assert.That(cellBefore.width, Is.GreaterThan(28));
            var rects = UiQualityRects();
            Submit("tool-1"); Submit(cell.name); yield return StableIdle(cell, cellBefore, dockBefore); UiQualityAssertRects(rects);
            Assert.That(installer.Model.PuzzleView!.CellAt(1, 1), Is.EqualTo(DevelopmentTool.EmptyMarker));
            Submit("tool-1"); yield return null; UiQualityAssertRects(rects);
            Assert.That(installer.Model.PuzzleView.CellAt(1, 1), Is.EqualTo(DevelopmentTool.EmptyMarker), "Repeating the tool button does not clear a cell; the instruction must name the cell tap.");
            Submit(cell.name); yield return StableIdle(cell, cellBefore, dockBefore); UiQualityAssertRects(rects);
            Assert.That(installer.Model.PuzzleView.CellAt(1, 1), Is.EqualTo(DevelopmentTool.Clear));
            Assert.That(installer.Presenter.ActiveTool, Is.EqualTo(DevelopmentTool.EmptyMarker));
            Submit("puzzle-undo"); yield return StableIdle(cell, cellBefore, dockBefore); UiQualityAssertRects(rects);
            Assert.That(installer.Model.PuzzleView.CellAt(1, 1), Is.EqualTo(DevelopmentTool.EmptyMarker));
            Assert.That(installer.Presenter.ActiveTool, Is.EqualTo(DevelopmentTool.EmptyMarker));
            Assert.That(caption.text, Is.EqualTo(expected)); AssertRect(caption.worldBound, box);
            string output = UiQualityDirectory("caption-10x10");
            File.WriteAllLines(Path.Combine(output, "caption-evidence.tsv"), new[] {
                "canvasEvidence\tcaptionWidth\tcaptionHeight\tmeasuredTextHeight\tfontSize\tcellAndToolRectCount\ttext",
                string.Join("\t", texture != null, caption.contentRect.width.ToString("F3", System.Globalization.CultureInfo.InvariantCulture),
                    caption.contentRect.height.ToString("F3", System.Globalization.CultureInfo.InvariantCulture),
                    measured.y.ToString("F3", System.Globalization.CultureInfo.InvariantCulture), caption.resolvedStyle.fontSize,
                    rects.Count, expected)
            });
            if (texture != null)
            {
                yield return null; yield return null;
                var region = UiQualityReadRegion(caption);
                Assert.That(region.Pixels.Distinct().Take(3).Count(), Is.GreaterThanOrEqualTo(2), "A blank Canvas cannot demonstrate rendered caption text.");
                File.WriteAllBytes(Path.Combine(output, "caption-region.png"), region.Png);
                UiQualityWriteCanvas(Path.Combine(output, "caption-10x10-after-retap-undo.png"));
            }
#else
            yield break;
#endif
        }

        private static T UiQualityField<T>(object owner, string name)
        {
            var field = owner.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, name); return (T)field!.GetValue(owner)!;
        }
        private static string[] UiQualityProps(RailwayRouteComposition route) => route.modules.Select(m =>
            m.kind + ":" + m.x.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + ":" +
            m.y.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + ":" + m.scale.ToString("R", System.Globalization.CultureInfo.InvariantCulture)).ToArray();
        private System.Collections.Generic.Dictionary<string, Rect> UiQualityRects()
        {
            var result = new System.Collections.Generic.Dictionary<string, Rect>(StringComparer.Ordinal);
            var grid = installer.Presenter.Grid!;
            for (int y = 0; y < 10; y++) for (int x = 0; x < 10; x++)
            { var cell = grid.CellButton(x, y); result.Add(cell.name, cell.worldBound); }
            var counts = grid.Query<Label>(className: "puzzle-count").ToList();
            var endpoints = grid.Query<Label>(className: "puzzle-endpoint").ToList();
            Assert.That(counts.Count, Is.EqualTo(20)); Assert.That(endpoints.Count, Is.EqualTo(2));
            foreach (var label in counts.Concat(endpoints)) result.Add(label.name, label.worldBound);
            for (int i = 0; i < 9; i++) result.Add("tool-" + i, Find<Button>("tool-" + i).worldBound);
            foreach (string name in new[] { "tool-dock", "puzzle-viewport", "puzzle-undo", "puzzle-hint", "puzzle-zoom" }) result.Add(name, Find<VisualElement>(name).worldBound);
            return result;
        }
        private void UiQualityAssertRects(System.Collections.Generic.Dictionary<string, Rect> expected)
        { var actual = UiQualityRects(); foreach (var pair in expected) AssertRect(actual[pair.Key], pair.Value); }
        private static string UiQualityDirectory(string label)
        {
            string output = Path.Combine(Directory.GetCurrentDirectory(), ".utmp", "wp046-runtime", "ui-quality-" + label + "-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(output); return output;
        }
        private static string UiQualityHash(byte[] bytes)
        { using var hash = System.Security.Cryptography.SHA256.Create(); return BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-", string.Empty).ToLowerInvariant(); }
        private sealed class UiQualityRegion
        {
            internal UiQualityRegion(Color32[] pixels, byte[] png, int width, int height) { Pixels = pixels; Png = png; Width = width; Height = height; }
            internal readonly Color32[] Pixels; internal readonly byte[] Png; internal readonly int Width, Height;
        }
        private UiQualityRegion UiQualityReadRegion(VisualElement element)
        {
            Assert.That(texture, Is.Not.Null); Rect root = installer.Document.rootVisualElement.worldBound, box = element.worldBound;
            Assert.That(root.width, Is.GreaterThan(0)); Assert.That(root.height, Is.GreaterThan(0));
            float sx = texture!.width / root.width, sy = texture.height / root.height;
            int left = Mathf.Clamp(Mathf.FloorToInt((box.xMin - root.xMin) * sx), 0, texture.width - 1);
            int right = Mathf.Clamp(Mathf.CeilToInt((box.xMax - root.xMin) * sx), left + 1, texture.width);
            int bottom = Mathf.Clamp(Mathf.FloorToInt(texture.height - (box.yMax - root.yMin) * sy), 0, texture.height - 1);
            int top = Mathf.Clamp(Mathf.CeilToInt(texture.height - (box.yMin - root.yMin) * sy), bottom + 1, texture.height);
            var prior = RenderTexture.active; var image = new Texture2D(right - left, top - bottom, TextureFormat.RGBA32, false);
            try
            {
                RenderTexture.active = texture; image.ReadPixels(new Rect(left, bottom, image.width, image.height), 0, 0); image.Apply();
                return new UiQualityRegion(image.GetPixels32(), image.EncodeToPNG(), image.width, image.height);
            }
            finally { RenderTexture.active = prior; UnityEngine.Object.Destroy(image); }
        }
        private void UiQualityWriteCanvas(string path)
        {
            Assert.That(texture, Is.Not.Null); var prior = RenderTexture.active;
            var image = new Texture2D(texture!.width, texture.height, TextureFormat.RGBA32, false);
            try { RenderTexture.active = texture; image.ReadPixels(new Rect(0, 0, image.width, image.height), 0, 0); image.Apply(); File.WriteAllBytes(path, image.EncodeToPNG()); }
            finally { RenderTexture.active = prior; UnityEngine.Object.Destroy(image); }
        }

        private IEnumerator ConfigureCandidate(bool unreviewedStoreMetadata=false)
        {
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
            UnityEngine.Object.Destroy(host);yield return null;yield return Await(installer.ShutdownTask);
            host=new GameObject("WP046 full internal candidate integration");host.SetActive(false);
            installer=host.AddComponent<MobilePreviewInstaller>();installer.ConfigureTestRoot(ownRoot);
            installer.ConfigureTestEndlessSeeds(1,2);
            installer.ConfigureCandidate(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(U+"ProductionGame.uxml"),
                AssetDatabase.LoadAssetAtPath<StyleSheet>(U+"ProductionGame.uss"),AssetDatabase.LoadAssetAtPath<TextAsset>(U+"ProductionGameLocale.json"),panel,
                AssetDatabase.LoadAssetAtPath<TextAsset>(U+"MobileCandidate.public.json"),AssetDatabase.LoadAssetAtPath<TextAsset>(U+"MobileCandidate.campaign.json"),
                AssetDatabase.LoadAssetAtPath<TextAsset>(U+"MobileCandidate.completion.json"),AssetDatabase.LoadAssetAtPath<TextAsset>(U+"RailwayAssets.json"),
                AssetDatabase.LoadAssetAtPath<TextAsset>(U+"MobileCandidate.cosmetics.json"),AssetDatabase.LoadAssetAtPath<TextAsset>(U+"MobileCandidate.bindings.json"),
                AssetDatabase.LoadAssetAtPath<TextAsset>(U+"ProductionCampaignText.json"));
            installer.ConfigureEndless(AssetDatabase.LoadAssetAtPath<TextAsset>(U+"MobileCandidate.endless-profiles.json"),
                AssetDatabase.LoadAssetAtPath<TextAsset>(U+"MobileCandidate.endless-completion.json"),AssetDatabase.LoadAssetAtPath<TextAsset>(U+"MobileCandidate.endless-bindings.json"));
            if(unreviewedStoreMetadata)installer.ConfigureCandidateStore(new STP.MobileServices.Store.StoreConfiguration(
                MobileApplicationProfile.CandidateApplicationId,"remove_ads_synthetic_test","GooglePlay",false,"synthetic-unreviewed-test"),null);
            host.SetActive(true);installer.Document.rootVisualElement.style.width=390;installer.Document.rootVisualElement.style.height=844;
            yield return Await(installer.InitializeTask);yield return null;
            Assert.That(installer.Candidate,Is.Not.Null,installer.StartupDiagnostic);
#else
            yield break;
#endif
        }

        [UnityTest]
        public IEnumerator Candidate_UnreviewedStoreMetadataShowsNoOffer_AndOfflinePuzzleStillSaves()
        {
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
            yield return ConfigureCandidate(true);yield return CreateProfile();
            Submit("nav-profile");Submit("profile-settings");yield return Idle();
            var purchase=Find<Button>("settings-remove-ads");
            Assert.That(purchase.enabledInHierarchy,Is.False);
            Assert.That(purchase.text,Does.Not.Contain("€"));
            Assert.That(Find<Button>("settings-restore").enabledInHierarchy,Is.False);
            Submit("settings-close");yield return Idle();Submit("nav-home");Submit("home-play");yield return Idle();
            var view=installer.Model!.PuzzleView!;long generation=installer.Candidate!.Persisted.Snapshot!.Generation;
            Submit("tool-1");Submit(ProductionPuzzleGrid.CellName(1,1));yield return Idle();
            Assert.That(view.CellAt(1,1),Is.EqualTo(DevelopmentTool.EmptyMarker));
            Assert.That(installer.Candidate.Persisted.Snapshot!.Generation,Is.GreaterThan(generation));
            Assert.That(installer.Model.State.CanMutate,Is.True);
#else
            yield break;
#endif
        }

        [UnityTest, Timeout(180000)]
        public IEnumerator Candidate_Endless_ActualGeneratedPuzzleHintRestartResumeCompletionSkipAndNextReservation()
        {
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
            yield return ConfigureCandidate();yield return CreateProfile();
            // Test-only setup: solve all actual public inputs through real domain commands and progress policy.
            // These synthetic commands and their times are never human acceptance or calibration evidence.
            var original=installer.Model!.ConfirmedSnapshot!;var candidate=installer.Candidate!;
            var corpus=Port<IPublicPuzzleCatalog>(candidate,"Catalog");var progress=Port<object>(candidate,"Progress");
            var campaign=Port<ILockedCampaignCatalog>(progress,"Campaign");var completion=Port<ILockedCompletionCatalog>(progress,"Completion");
            var json=(ICanonicalJson)Activator.CreateInstance(Assembly.Load("STP.Infrastructure.Content").GetType("STP.Infrastructure.Content.CanonicalJsonAdapter",true)!)!;
            var ledger=(ILocalLedger)Activator.CreateInstance(Assembly.Load("STP.Infrastructure.Persistence").GetType("STP.Infrastructure.Persistence.SaveLedgerCodec",true)!,json)!;
            var policy=new LocalProgressPolicy(corpus,campaign,completion,json,ledger);
            var routes = campaign.Entries.GroupBy(e => e.RouteId, StringComparer.Ordinal)
                .Select(g => new { Id = g.Key, Inputs = g.Select(e => corpus.Puzzles.Single(p => p.Identity.Equals(e.Puzzle))).ToArray() }).ToArray();
            Assert.That(routes.Length, Is.EqualTo(20)); Assert.That(routes.All(r => r.Inputs.Length == 12), Is.True);
            CollectionAssert.AreEqual(corpus.Puzzles.Select(p => p.Identity), routes.SelectMany(r => r.Inputs).Select(p => p.Identity), "The actual twenty campaign routes must preserve all 240 source identities and their order.");
            seasonSetupCancellation = new CancellationTokenSource(); var token = seasonSetupCancellation.Token;
            var seasonState = original.Payload; int completed = 0;
            Assert.That(seasonState.Levels.Count(l => l.FirstClear != null), Is.Zero);
            foreach (var route in routes)
            {
                var routeInputs = route.Inputs; var prior = seasonState;
                var routeWork = Task.Run(() =>
                {
                    var state = prior;
                foreach(var input in routeInputs)
                {
                    token.ThrowIfCancellationRequested();
                    var solution=PuzzleSolver.Solve(input.ToDomainDefinition(),new SolverLimits(1000000,10000,token));
                    Assert.That(solution.Classification,Is.EqualTo(SolverClassification.UNIQUE));
                    var session=PuzzleSessionState.Create(input.ToDomainDefinition(),input.Identity.PuzzleId,input.Identity.HashProfile,
                        input.Identity.HashSha256,Guid.NewGuid(),SessionMode.FIRST_RUN,0,null,null).Value!;
                    for(int i=0;i<solution.Path.Count;i++)
                    {
                        var cell=solution.Path[i];int a=i==0?(int)input.A.Side:DirectionTo(cell,solution.Path[i-1]);
                        int b=i==solution.Path.Count-1?(int)input.B.Side:DirectionTo(cell,solution.Path[i+1]);
                        var command=new ApplyCellContent(Guid.NewGuid(),session.Revision,cell,(CellContent)Shape((1<<a)|(1<<b)));
                        var applied=PuzzleCommandHandler.Handle(session,command,i*10L);
                        Assert.That(applied.Error,Is.EqualTo(DomainError.NONE));
                        session=applied.State??throw new InvalidOperationException("Controlled campaign command returned no state.");
                    }
                    Assert.That(session.Phase,Is.EqualTo(SessionPhase.SolvedPendingCommit));
                    state=policy.Complete(state,session,original.Generation+1,DateTimeOffset.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'")).State;
                }
                    return state;
                }, token);
                seasonSetupWork = routeWork; yield return Await(routeWork);
                seasonState = routeWork.GetAwaiter().GetResult(); completed += routeInputs.Length;
                Assert.That(seasonState.Levels.Count(l => l.FirstClear != null), Is.EqualTo(completed), route.Id);
            }
            var prepared = Task.Run(() => policy.PrepareForSave(seasonState, original.Generation + 1), token);
            seasonSetupWork = prepared; yield return Await(prepared);
            var repository=Port<ILocalProfileRepository>(candidate,"Repository");
            var write=repository.CommitAsync(new SaveSnapshot<LocalProfileState>(original.Profile,original.Catalog,original.Generation+1,prepared.Result),original.Generation,CancellationToken.None);
            seasonSetupWork = write; yield return Await(write);Assert.That(write.Result.Succeeded,Is.True,write.Result.ErrorCode);
            yield return Await(installer.RunRequestAsync(SavedReviewUiRequest.Bound(SavedReviewUiRequestKind.Reload,installer.Model.State.Version)));yield return Idle();
            Assert.That(installer.Model.State.Progress!.FirstClearCount,Is.EqualTo(240));
            Submit("home-endless");yield return null;yield return Capture("candidate-endless-menu");
            foreach(string profile in new[]{"dienstbeginn","regelbetrieb","stoerungslage","grosslage"})Assert.That(Find<Button>("endless-profile-"+profile).enabledInHierarchy,Is.True);
            Submit("endless-profile-dienstbeginn");yield return Idle();yield return null;
            var view=installer.Model.PuzzleView!;var identity=view.Puzzle.Identity;
            Assert.That(installer.Model.CurrentModeName,Is.EqualTo("ENDLESS"));Assert.That(identity.PuzzleId,Does.Not.StartWith("S1-"));
            var issued=candidate.Persisted.Snapshot!.Payload.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single();
            Assert.That(issued.PublicPuzzleInput.Identity,Is.EqualTo(identity));Assert.That(issued.GenerationOrdinal,Is.EqualTo("1"));
            Assert.That(installer.Presenter.IsPuzzleScreenActive,Is.True,"Issued puzzle must replace its own paused menu.");
            Assert.That(installer.Model.State.CanMutate,Is.True,"Closing the menu must release the host input pause.");
            Submit("puzzle-hint");float deadline=Time.realtimeSinceStartup+15;
            while(candidate.Metagame.HintCredits(candidate.Persisted.Snapshot.Payload,identity)==3&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(candidate.Metagame.HintCredits(candidate.Persisted.Snapshot.Payload,identity),Is.EqualTo(2),"Wait for the actual visible clue's durable acknowledgement.");
            while(!Find<Button>("hint-close").enabledInHierarchy&&Time.realtimeSinceStartup<deadline)yield return null;
            Submit("hint-close");yield return Idle();
            Guid firstAttempt=view.AttemptId;
            yield return Await(installer.RunRequestAsync(SavedReviewUiRequest.Bound(SavedReviewUiRequestKind.Restart,installer.Model.State.Version)));yield return Idle();
            Assert.That(installer.Model.PuzzleView!.AttemptId,Is.Not.EqualTo(firstAttempt));
            Assert.That(candidate.Metagame.HintCredits(candidate.Persisted.Snapshot.Payload,identity),Is.EqualTo(2));
            Submit("tool-1");Submit(ProductionPuzzleGrid.CellName(1,1));yield return Idle();Submit("puzzle-back");yield return Idle();
            yield return Await(installer.RunRequestAsync(SavedReviewUiRequest.Bound(SavedReviewUiRequestKind.Reload,installer.Model.State.Version)));yield return Idle();
            Submit("home-endless");yield return null;Submit("endless-resume-"+identity.PuzzleId);yield return Idle();yield return null;
            view=installer.Model.PuzzleView!;Assert.That(view.Puzzle.Identity,Is.EqualTo(identity));Assert.That(view.CellAt(1,1),Is.EqualTo(DevelopmentTool.EmptyMarker));
            yield return Capture("candidate-endless-puzzle");
            var solved=PuzzleSolver.Solve(view.Puzzle.ToDomainDefinition(),new SolverLimits(1000000,10000));
            long displayedBefore=installer.Model.State.Progress!.BalancePatience;
            for(int i=0;i<solved.Path.Count;i++)
            {
                var cell=solved.Path[i];int a=i==0?(int)view.EndpointA.Edge:DirectionTo(cell,solved.Path[i-1]);
                int b=i==solved.Path.Count-1?(int)view.EndpointB.Edge:DirectionTo(cell,solved.Path[i+1]);
                Submit("tool-"+(int)Shape((1<<a)|(1<<b)));Submit(ProductionPuzzleGrid.CellName(cell.X,cell.Y));yield return Idle();
            }
            deadline=Time.realtimeSinceStartup+25;while(installer.Model.State.Status!=SavedReviewStatus.Results&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(installer.Model.State.Status,Is.EqualTo(SavedReviewStatus.Results));
            Assert.That(installer.Model.State.Progress!.FirstClearCount,Is.EqualTo(240));Assert.That(installer.Model.State.Progress.SelectedStars,Is.EqualTo(1));
            Assert.That(installer.Model.State.Progress.BalancePatience-displayedBefore,Is.EqualTo(15));
            Assert.That(candidate.Persisted.Snapshot!.Payload.Levels.Any(l=>l.Identity.Equals(identity)),Is.False);
            Assert.That(candidate.Persisted.Snapshot.Payload.Endless.OpenEndless.OfType<OpenEndlessCompletion>().Single().Id,Is.EqualTo(identity.PuzzleId));
            Submit("result-skip-bonus");yield return Idle();yield return null;
            Assert.That(candidate.Persisted.Snapshot.Payload.Endless.OpenEndless,Is.Empty);yield return Capture("candidate-endless-result");
            Submit("result-next");yield return Idle();yield return null;
            Assert.That(Find<VisualElement>("endless-overlay").resolvedStyle.display,Is.EqualTo(DisplayStyle.Flex));
            yield return Idle();deadline=Time.realtimeSinceStartup+10;
            while((!installer.Model.CanRunProfileAction||!installer.PendingOperation.IsCompleted||!Find<Button>("endless-profile-dienstbeginn").enabledInHierarchy)&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(installer.Model.CanRunProfileAction,Is.True,"Next menu must settle its host attention pause: "+installer.Model.State.Status+":"+installer.Model.State.ErrorCode);
            Submit("endless-profile-dienstbeginn");yield return Idle();yield return null;
            Assert.That(candidate.Persisted.Snapshot.Payload.Endless.HighestReservedOrdinal,Is.EqualTo("2"),
                installer.Model.State.Status+":"+installer.Model.State.ErrorCode+":"+installer.Model.EndlessGenerationStatus+":pending="+installer.PendingOperation.Status);
            Assert.That(installer.Model.PuzzleView!.Puzzle.Identity.PuzzleId,Is.Not.EqualTo(identity.PuzzleId),
                installer.Model.EndlessGenerationStatus+":"+string.Join(",",candidate.Persisted.Snapshot.Payload.Endless.OpenEndless.Select(e=>e.GetType().Name+":"+e.Id)));
            Assert.That(installer.Presenter.IsPuzzleScreenActive,Is.True);
#else
            yield break;
#endif
        }
        private static T Port<T>(object owner,string name)=>(T)owner.GetType().GetProperty(name)!.GetValue(owner)!;
        private IEnumerator Select(int index)
        { yield return Await(installer.RunRequestAsync(SavedReviewUiRequest.Puzzle(SavedReviewUiRequestKind.OpenPuzzle, installer.Model!.State.Version, index))); yield return Idle(); }
        private IEnumerator StableIdle(Button cell, Rect cellRect, Rect dockRect)
        {
            float deadline = Time.realtimeSinceStartup + 15;
            do
            {
                yield return null; AssertRect(cell.worldBound, cellRect); AssertRect(Find<VisualElement>("tool-dock").worldBound, dockRect);
            } while ((!installer.PendingOperation.IsCompleted || installer.Model!.State.IsDirty || installer.Model.State.Status == SavedReviewStatus.Saving) && Time.realtimeSinceStartup < deadline);
            Assert.That(installer.Model!.State.IsDirty, Is.False);
            Assert.That(installer.Presenter.HasRequestFault, Is.False);
        }
        private IEnumerator Idle()
        {
            float deadline = Time.realtimeSinceStartup + 20;
            do { yield return null; } while ((!installer.PendingOperation.IsCompleted || installer.Presenter.PendingRequest != null || installer.Presenter.IsResultContinuationPending || installer.Model?.State.IsDirty == true) && Time.realtimeSinceStartup < deadline);
            Assert.That(installer.PendingOperation.IsCompleted, Is.True);
            Assert.That(installer.PendingOperation.IsFaulted,Is.False,installer.PendingOperation.Exception?.GetBaseException().ToString());
            Assert.That(installer.Presenter.PendingRequest, Is.Null);
            Assert.That(installer.Presenter.IsResultContinuationPending, Is.False);
            Assert.That(installer.Presenter.HasRequestFault, Is.False);
        }
        private static IEnumerator Await(Task task)
        {
            float deadline = Time.realtimeSinceStartup + 25;
            while (!task.IsCompleted && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(task.IsCompleted, Is.True); if (task.IsFaulted) throw task.Exception!;
            Assert.That(task.IsCanceled, Is.False);
        }
        private T Find<T>(string name) where T : VisualElement => installer.Document.rootVisualElement.Q<T>(name) ?? throw new InvalidOperationException(name);
        private void Submit(string name)
        {
            var button = Find<Button>(name); Assert.That(button.enabledInHierarchy, Is.True, name);
            using (var evt = NavigationSubmitEvent.GetPooled()) { evt.target = button; button.SendEvent(evt); }
        }
        private static void AssertRect(Rect actual, Rect expected)
        { Assert.That(actual.x, Is.EqualTo(expected.x).Within(.05f)); Assert.That(actual.y, Is.EqualTo(expected.y).Within(.05f)); Assert.That(actual.width, Is.EqualTo(expected.width).Within(.05f)); Assert.That(actual.height, Is.EqualTo(expected.height).Within(.05f)); }
        private static int DirectionTo(CellCoordinate from, CellCoordinate to) => to.X > from.X ? 1 : to.X < from.X ? 3 : to.Y > from.Y ? 2 : 0;
        private static DevelopmentTool Shape(int ports) => ports switch
        { 5 => DevelopmentTool.TrackNS, 10 => DevelopmentTool.TrackEW, 3 => DevelopmentTool.TrackNE, 6 => DevelopmentTool.TrackES, 12 => DevelopmentTool.TrackSW, 9 => DevelopmentTool.TrackWN, _ => throw new InvalidOperationException("Invalid path ports") };
        private IEnumerator Capture(string name)
        {
            if (texture == null) yield break;
            yield return null; yield return null;
            var prior = RenderTexture.active;
            var image = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            try
            {
                RenderTexture.active = texture; image.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0); image.Apply();
                string path = Path.Combine(Directory.GetCurrentDirectory(), ".utmp", "wp046-runtime"); Directory.CreateDirectory(path);
                File.WriteAllBytes(Path.Combine(path, name + ".png"), image.EncodeToPNG());
            }
            finally { RenderTexture.active = prior; UnityEngine.Object.Destroy(image); }
        }
    }
}
