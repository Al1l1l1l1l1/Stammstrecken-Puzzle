using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using STP.Application;
using STP.Bootstrap;
using STP.Presentation.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace STP.Tests.Bootstrap.PlayMode
{
    /// <summary>Actual scene/UI/ride contracts; graphical evidence is separate from headless semantics.</summary>
    public sealed class DevelopmentPuzzleSmokeTests
    {
        private const string ScenePath = "Assets/StammstreckenPuzzle/Scenes/QA/DevelopmentPuzzle.unity";
        private const string UiPath = "Assets/StammstreckenPuzzle/UI/";
        private const string MaterialPath = "Assets/StammstreckenPuzzle/Art/Development/DevelopmentTrain.mat";
        private DevelopmentPuzzleInstaller installer = null!;
        private PanelSettings ownedPanel = null!;
        private RenderTexture? panelTexture;

        [UnitySetUp]
        public IEnumerator LoadActualQaSceneWithAnOwnedPanel()
        {
            if (RequireGraphics()) Assert.That(SystemInfo.graphicsDeviceType, Is.Not.EqualTo(GraphicsDeviceType.Null), "STP_REQUIRE_GRAPHICS=1 requires a real renderer.");
#if UNITY_EDITOR
            InvokeBuilder("VerifyArtifacts");
            EditorSceneManager.LoadSceneInPlayMode(ScenePath, new LoadSceneParameters(LoadSceneMode.Single));
#else
            SceneManager.LoadScene(ScenePath, LoadSceneMode.Single);
#endif
            yield return null;
            installer = UnityEngine.Object.FindFirstObjectByType<DevelopmentPuzzleInstaller>()!;
            Assert.That(installer, Is.Not.Null, "The actual checked-in QA scene must compose its explicit installer.");
            ownedPanel = UnityEngine.Object.Instantiate(installer.Document.panelSettings);
            ownedPanel.name = "WP038 test-owned panel clone";
            if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null)
            {
                panelTexture = new RenderTexture(960, 840, 24, RenderTextureFormat.ARGB32) { name = "WP038 actual QA panel capture" };
                Assert.That(panelTexture.Create(), Is.True);
                ownedPanel.targetTexture = panelTexture;
            }
            installer.Document.panelSettings = ownedPanel;
            PrepareVisiblePanel();
            for (var frame = 0; frame < 4; frame++) yield return null;
        }

        [UnityTearDown]
        public IEnumerator ReleaseOnlyTestOwnedRenderingResources()
        {
            if (installer != null) UnityEngine.Object.Destroy(installer.gameObject);
            yield return null;
            if (ownedPanel != null)
            {
                ownedPanel.targetTexture = null;
                UnityEngine.Object.Destroy(ownedPanel);
            }
            if (panelTexture != null)
            {
                panelTexture.Release();
                UnityEngine.Object.Destroy(panelTexture);
                panelTexture = null;
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator ActualUi_AllToolsSelectionBatchUndoAndNoOpUseTheRealDomain()
        {
            // Unity may deliver legitimate startup focus/pause commands before the probe.
            // Assert the UI's own revision delta, not an assumed platform callback count.
            long baselineRevision = installer.Model.Revision;
            Assert.That(installer.Model.Width, Is.EqualTo(4)); Assert.That(installer.Model.Height, Is.EqualTo(4));
            Assert.That(installer.Model.A.Edge, Is.EqualTo(DevelopmentEdge.W)); Assert.That(installer.Model.B.Edge, Is.EqualTo(DevelopmentEdge.S));
            Assert.That(installer.Presenter.Grid.GridWidth, Is.EqualTo(4));
            Assert.That(Find<Label>("qa-boundary").text, Is.Not.Empty);
            Assert.That(Find<Label>("qa-endpoint-a").text, Is.Not.Empty); Assert.That(Find<Label>("qa-endpoint-b").text, Is.Not.Empty);
            var toolIds = new[] { "clear", "empty", "occupied", "ns", "ew", "ne", "es", "sw", "wn" };
            for (var index = 0; index < toolIds.Length; index++)
            {
                var button = Find<Button>("qa-tool-" + toolIds[index]);
                Assert.That(button.text, Is.Not.Empty); Assert.That(button.enabledInHierarchy, Is.True);
                Submit(button);
                Assert.That(installer.Presenter.ActiveTool, Is.EqualTo((DevelopmentTool)index));
                Assert.That(button.ClassListContains("qa-tool--active"), Is.True);
            }
            Assert.That(installer.Model.Revision, Is.EqualTo(baselineRevision)); Assert.That(installer.Model.TimerStarted, Is.False);
            Submit("qa-tool-clear"); Submit("qa-cell-0-1");
            Assert.That(installer.Model.Revision, Is.EqualTo(baselineRevision)); Assert.That(installer.Model.TimerStarted, Is.False);
            Submit("qa-multiselect"); Submit("qa-cell-0-1"); Submit("qa-cell-1-1");
            Assert.That(installer.Presenter.SelectionCount, Is.EqualTo(2));
            Assert.That(installer.Presenter.Grid.IsCellSelected(0, 1), Is.True);
            Assert.That(Find<Button>("qa-cell-0-1").ClassListContains("qa-cell--selected"), Is.True);
            Assert.That(installer.Model.CellAt(0, 1), Is.EqualTo(DevelopmentTool.Clear)); Assert.That(installer.Model.TimerStarted, Is.False);
            Submit("qa-tool-occupied"); Submit("qa-apply-selection");
            Assert.That(installer.Model.Revision, Is.EqualTo(baselineRevision + 1)); Assert.That(installer.Model.RowVisible(1), Is.EqualTo(2));
            Assert.That(Find<Label>("qa-row-count-1").ClassListContains("qa-count--exceeded"), Is.True);
            Assert.That(installer.Presenter.SelectionCount, Is.Zero); Assert.That(installer.Model.TimerStarted, Is.True);
            Submit("qa-undo");
            Assert.That(installer.Model.CellAt(0, 1), Is.EqualTo(DevelopmentTool.Clear));
            Assert.That(installer.Model.CellAt(1, 1), Is.EqualTo(DevelopmentTool.Clear));
            Assert.That(installer.Model.RowVisible(1), Is.Zero); Assert.That(installer.Model.CanUndo, Is.False);
            Assert.That(installer.Model.UsedOccupiedMarker, Is.True);
            Submit("qa-multiselect"); Submit("qa-tool-empty"); Submit("qa-cell-0-1");
            Assert.That(installer.Model.CellAt(0, 1), Is.EqualTo(DevelopmentTool.EmptyMarker));
            Assert.That(installer.Model.RowVisible(1), Is.Zero); Assert.That(installer.Model.HasObjectiveViolations, Is.False);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ActualUi_SolvedPathStartsOneRideBeforeUnsavedResults_AndRestarts()
        {
            var attempt = installer.Model.AttemptId;
            SolveUsingActualUi();
            Assert.That(installer.Model.Phase, Is.EqualTo(DevelopmentPreviewPhase.TrainRide));
            Assert.That(installer.Model.IsAwaitingSaveCommit, Is.True);
            Assert.That(Find<VisualElement>("qa-result").style.display.value, Is.EqualTo(DisplayStyle.None));
            var expected = ExpectedLPath(4);
            Assert.That(Project(installer.Model.BuiltPath), Is.EqualTo(Project(expected)));
            yield return null;
            var ride = installer.Ride!;
            Assert.That(ride, Is.Not.Null); Assert.That(Project(ride.Path), Is.EqualTo(Project(expected)));
            var revision = installer.Model.Revision;
            Assert.That(Find<Button>("qa-restart").enabledInHierarchy, Is.False);
            Submit("qa-restart"); Submit("qa-cell-0-0"); Submit("qa-tool-empty");
            Assert.That(installer.Model.AttemptId, Is.EqualTo(attempt)); Assert.That(installer.Model.Revision, Is.EqualTo(revision));
            Assert.That(installer.Model.Phase, Is.EqualTo(DevelopmentPreviewPhase.TrainRide));
            double deadline = Time.realtimeSinceStartupAsDouble + 12;
            while (installer.Model.Phase == DevelopmentPreviewPhase.TrainRide && Time.realtimeSinceStartupAsDouble < deadline) yield return null;
            Assert.That(installer.Model.Phase, Is.EqualTo(DevelopmentPreviewPhase.Results));
            Assert.That(ride.IsFinished, Is.True); Assert.That(Project(ride.VisitedCells), Is.EqualTo(Project(expected)));
            Assert.That(ride.Position.x, Is.EqualTo(3.5f).Within(.001f)); Assert.That(ride.Position.y, Is.EqualTo(0f).Within(.001f));
            Assert.That(installer.Model.IsAwaitingSaveCommit, Is.True);
            Assert.That(Find<VisualElement>("qa-result").style.display.value, Is.EqualTo(DisplayStyle.Flex));
            Assert.That(Find<Label>("qa-result-boundary").text, Is.Not.Empty);
            Assert.That(installer.Model.CompleteTrainRide(attempt), Is.EqualTo(DevelopmentCommandStatus.Unchanged));
            Submit("qa-restart");
            Assert.That(installer.Model.AttemptId, Is.Not.EqualTo(attempt)); Assert.That(installer.Model.TimerStarted, Is.False);
            Assert.That(installer.Model.Phase, Is.EqualTo(DevelopmentPreviewPhase.Editing)); Assert.That(installer.Model.BuiltPath, Is.Empty);
            Assert.That(installer.Model.CompleteTrainRide(attempt), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            yield return null;
            Assert.That(ride.IsDisposed, Is.True); Assert.That(installer.Ride, Is.Null);
        }

        [UnityTest]
        public IEnumerator ActualLifecycle_PauseAndFocusExcludeBackgroundTime_AndDisposedCallbacksDoNothing()
        {
            Submit("qa-tool-empty"); Submit("qa-cell-1-1");
            installer.SetApplicationPaused(true);
            long pausedElapsed = installer.Model.ElapsedAt(installer.NowMilliseconds);
            Assert.That(installer.Model.IsPaused, Is.True);
            for (var frame = 0; frame < 5; frame++) yield return null;
            Assert.That(installer.Model.ElapsedAt(installer.NowMilliseconds), Is.EqualTo(pausedElapsed));
            installer.SetApplicationFocus(false); installer.SetApplicationPaused(false);
            Assert.That(installer.Model.IsPaused, Is.True, "Loss of focus remains a separate pause cause.");
            installer.SetApplicationFocus(true);
            Assert.That(installer.Model.IsPaused, Is.False);
            var model = installer.Model; var presenter = installer.Presenter; var cell = Find<Button>("qa-cell-2-2");
            long revision = model.Revision;
            presenter.Dispose(); presenter.Dispose();
            Submit(cell); Submit(Find<Button>("qa-restart"));
            Assert.That(model.Revision, Is.EqualTo(revision));
            UnityEngine.Object.Destroy(installer.gameObject);
            yield return null;
            presenter.Dispose();
            Submit(cell);
            Assert.That(model.Revision, Is.EqualTo(revision));
        }

        [Test]
        public void QaProfileMissingReferencesAndMissingLocaleKeys_FailClosed()
        {
            var configuration = new BootstrapConfiguration();
            DevelopmentPuzzleInstaller.ValidateQaProfile(configuration.Profile, configuration.ApplicationIdentifier);
            Assert.Throws<InvalidOperationException>(() => DevelopmentPuzzleInstaller.ValidateQaProfile("production", configuration.ApplicationIdentifier));
            Assert.Throws<InvalidOperationException>(() => DevelopmentPuzzleInstaller.ValidateQaProfile("qa", "com.STP.StammstreckenPuzzle"));
#if UNITY_EDITOR
            var tree = OwnAsset<VisualTreeAsset>(UiPath + "DevelopmentPuzzle.uxml");
            var styles = OwnAsset<StyleSheet>(UiPath + "DevelopmentPuzzle.uss");
            var locale = OwnAsset<TextAsset>(UiPath + "DevelopmentPuzzleLocale.json");
            var material = OwnAsset<Material>(MaterialPath);
            DevelopmentPuzzleInstaller.ValidateReferences(tree, styles, locale, ownedPanel, material);
            Assert.Throws<InvalidOperationException>(() => DevelopmentPuzzleInstaller.ValidateReferences(null, styles, locale, ownedPanel, material));
            Assert.Throws<InvalidOperationException>(() => DevelopmentPuzzleInstaller.ValidateReferences(tree, null, locale, ownedPanel, material));
            Assert.Throws<InvalidOperationException>(() => DevelopmentPuzzleInstaller.ValidateReferences(tree, styles, null, ownedPanel, material));
            Assert.Throws<InvalidOperationException>(() => DevelopmentPuzzleInstaller.ValidateReferences(tree, styles, locale, null, material));
            Assert.Throws<InvalidOperationException>(() => DevelopmentPuzzleInstaller.ValidateReferences(tree, styles, locale, ownedPanel, null));
            var themeLess = UnityEngine.Object.Instantiate(ownedPanel);
            try
            {
                themeLess.themeStyleSheet = null;
                Assert.Throws<InvalidOperationException>(() => DevelopmentPuzzleInstaller.ValidateReferences(tree, styles, locale, themeLess, material));
            }
            finally { UnityEngine.Object.Destroy(themeLess); }
            var incomplete = new TextAsset("{\"locale\":\"de-DE\",\"entries\":[{\"key\":\"qa.wp038.unused\",\"value\":\"QA\",\"argumentTypes\":[]}]}");
            try
            {
                using var localization = new DevelopmentQaLocalization(incomplete);
                Assert.Throws<InvalidOperationException>(() => localization.Resolve("qa.wp038.missing"));
                Assert.Throws<InvalidOperationException>(() => new DevelopmentPuzzlePresenter(
                    tree.CloneTree(), installer.Model, incomplete, () => installer.NowMilliseconds));
            }
            finally { UnityEngine.Object.Destroy(incomplete); }
#endif
        }

#if UNITY_EDITOR
        [Test]
        public void Builder_TwoReproductionsPreserveBytesGuidsAndOriginalPanel()
        {
            Assert.That(AssetDatabase.AssetPathToGUID(UiPath + "DevelopmentPuzzlePanelSettings.asset"), Is.EqualTo("b281a1bc613c4544b3dc4c9e6a038001"));
            Assert.That(AssetDatabase.AssetPathToGUID(MaterialPath), Is.EqualTo("b281a1bc613c4544b3dc4c9e6a038002"));
            Assert.That(AssetDatabase.AssetPathToGUID(ScenePath), Is.EqualTo("b281a1bc613c4544b3dc4c9e6a038003"));
            var paths = new[] { ScenePath, ScenePath + ".meta", UiPath + "DevelopmentPuzzlePanelSettings.asset",
                UiPath + "DevelopmentPuzzlePanelSettings.asset.meta", MaterialPath, MaterialPath + ".meta" };
            var before = paths.Select(File.ReadAllBytes).ToArray();
            InvokeBuilder("EnsureArtifacts");
            for (var index = 0; index < paths.Length; index++) Assert.That(File.ReadAllBytes(paths[index]), Is.EqualTo(before[index]), paths[index]);
            foreach (var path in new[] { ScenePath, UiPath + "DevelopmentPuzzlePanelSettings.asset", MaterialPath })
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            for (var index = 0; index < paths.Length; index++) Assert.That(File.ReadAllBytes(paths[index]), Is.EqualTo(before[index]), paths[index]);
            InvokeBuilder("EnsureArtifacts");
            for (var index = 0; index < paths.Length; index++) Assert.That(File.ReadAllBytes(paths[index]), Is.EqualTo(before[index]), paths[index]);
            var original = OwnAsset<PanelSettings>(UiPath + "DevelopmentPuzzlePanelSettings.asset");
            Assert.That(original.targetTexture, Is.Null); Assert.That(EditorUtility.IsDirty(original), Is.False);
            InvokeBuilder("VerifyArtifacts");
        }
#endif

        [UnityTest]
        public IEnumerator GraphicsProbe_4x4_ActualPanelAndSelection()
        {
            PrepareColoredUi();
            for (var frame = 0; frame < 4; frame++) yield return null;
            CapturePanel("wp038-4x4-editing.png");
        }

        [UnityTest]
        public IEnumerator GraphicsProbe_10x10_ActualPanelAndSelection()
        {
#if UNITY_EDITOR
            UnityEngine.Object.Destroy(installer.gameObject);
            yield return null;
            var host = new GameObject("WP038 large QA composition test");
            host.SetActive(false);
            installer = host.AddComponent<DevelopmentPuzzleInstaller>();
            installer.ConfigureForQa(OwnAsset<VisualTreeAsset>(UiPath + "DevelopmentPuzzle.uxml"),
                OwnAsset<StyleSheet>(UiPath + "DevelopmentPuzzle.uss"), OwnAsset<TextAsset>(UiPath + "DevelopmentPuzzleLocale.json"),
                ownedPanel, OwnAsset<Material>(MaterialPath), true);
            host.SetActive(true);
            PrepareVisiblePanel();
            for (var frame = 0; frame < 4; frame++) yield return null;
            Assert.That(installer.Model.Width, Is.EqualTo(10)); Assert.That(installer.Model.Height, Is.EqualTo(10));
            Assert.That(installer.Presenter.Grid.GridWidth, Is.EqualTo(10));
            Assert.That(Find<Button>("qa-cell-9-9"), Is.Not.Null);
            PrepareColoredUi();
            for (var frame = 0; frame < 4; frame++) yield return null;
            CapturePanel("wp038-10x10-editing.png");
#else
            Assert.Fail("The large QA scene composition probe requires the controlled Editor PlayMode route.");
            yield return null;
#endif
        }

        [UnityTest]
        public IEnumerator GraphicsProbe_4x4_ActualBuiltPathTrainOverlayBeforeResult()
        {
            SolveUsingActualUi();
            double deadline = Time.realtimeSinceStartupAsDouble + 8;
            while ((installer.Ride == null || installer.Ride.VisitedCells.Count < 1) && Time.realtimeSinceStartupAsDouble < deadline) yield return null;
            var ride = installer.Ride!;
            Assert.That(ride, Is.Not.Null); Assert.That(installer.Model.Phase, Is.EqualTo(DevelopmentPreviewPhase.TrainRide));
            installer.SetApplicationPaused(true);
            var position = ride.Position;
            for (var frame = 0; frame < 4; frame++) yield return null;
            Assert.That(ride.Position, Is.EqualTo(position));
            Assert.That(Find<VisualElement>("qa-result").style.display.value, Is.EqualTo(DisplayStyle.None));
            if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null)
            {
                Assert.That(ride.Texture, Is.TypeOf<RenderTexture>());
                Assert.That(((RenderTexture)ride.Texture!).IsCreated(), Is.True);
                Assert.That(Find<Image>("qa-train-overlay").image, Is.SameAs(ride.Texture));
                AssertTrainIsActuallyRendered((RenderTexture)ride.Texture!);
            }
            CapturePanel("wp038-4x4-ride.png");
            installer.SetApplicationPaused(false);
        }

        [UnityTest]
        public IEnumerator GraphicsProbe_NormalScreen_EditingRideResultsAndRestartRemainVisible()
        {
            // The panel-target probes above deliberately test another output boundary.
            // This probe keeps the normal screen panel unredirected for the entire UI loop.
            ownedPanel.targetTexture = null;
            var documentRoot = installer.Document.rootVisualElement;
            documentRoot.style.width = StyleKeyword.Null;
            documentRoot.style.height = StyleKeyword.Null;
            installer.SetApplicationPaused(false);
            installer.SetApplicationFocus(true);
            var screenCamera = installer.ScreenCamera;
            Assert.That(screenCamera.targetTexture, Is.Null);
            Assert.That(screenCamera.cullingMask, Is.Zero);
            Assert.That(screenCamera.depth, Is.GreaterThan(0));
            Assert.That(screenCamera.rect, Is.EqualTo(new Rect(0, 0, 1, 1)));
            Assert.That(screenCamera.targetDisplay, Is.Zero);
            Assert.That(screenCamera.enabled, Is.EqualTo(SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null));
            for (var frame = 0; frame < 4; frame++) yield return null;

            PrepareColoredUi();
            for (var frame = 0; frame < 2; frame++) yield return null;
            ScreenFrame? editing = null;
            yield return CaptureScreen("wp038-screen-editing.png", -1, value => editing = value);
            if (editing != null)
            {
                AssertScreenText(editing, "qa-boundary");
                AssertScreenText(editing, "qa-title");
                AssertOnScreen(editing, Find<Button>("qa-tool-ew"));
                AssertOnScreen(editing, Find<Button>("qa-tool-clear"));
                AssertOnScreen(editing, Find<Button>("qa-restart"));
                AssertOnScreen(editing, installer.Presenter.Grid.Board);
                AssertEditingScreenOrientation(editing);
            }

            var markedAttempt = installer.Model.AttemptId;
            Submit("qa-restart");
            Assert.That(installer.Model.AttemptId, Is.Not.EqualTo(markedAttempt));
            Assert.That(installer.Model.CellAt(1, 1), Is.EqualTo(DevelopmentTool.Clear));
            if (installer.Presenter.MultipleSelection) Submit("qa-multiselect");
            var rideAttempt = installer.Model.AttemptId;
            SolveUsingActualUi();
            double rideDeadline = Time.realtimeSinceStartupAsDouble + 8;
            while ((installer.Ride == null || installer.Ride.VisitedCells.Count < 1) && Time.realtimeSinceStartupAsDouble < rideDeadline) yield return null;
            var ride = installer.Ride!;
            Assert.That(ride, Is.Not.Null);
            Assert.That(installer.Model.Phase, Is.EqualTo(DevelopmentPreviewPhase.TrainRide));
            Assert.That(installer.Model.IsAwaitingSaveCommit, Is.True);
            Assert.That(Find<VisualElement>("qa-result").style.display.value, Is.EqualTo(DisplayStyle.None));
            installer.SetApplicationPaused(true);
            var pausedPosition = ride.Position;
            for (var frame = 0; frame < 4; frame++) yield return null;
            ScreenFrame? riding = null;
            yield return CaptureScreen("wp038-screen-ride.png", editing?.Frame ?? -1, value => riding = value);
            Assert.That(ride.Position, Is.EqualTo(pausedPosition));
            if (riding != null)
            {
                Assert.That(Find<Image>("qa-train-overlay").image, Is.SameAs(ride.Texture));
                AssertScreenText(riding, "qa-boundary");
                int trainPixels = CountScreenPixels(riding, installer.Presenter.Grid.Board,
                    color => color.r > 180 && color.g > 90 && color.g < 220 && color.b < 150 && color.r > color.g + 20 && color.g > color.b + 30);
                Assert.That(trainPixels, Is.GreaterThan(30), "The actual screen's inner board must show the orange technical train; allocation or World/Panel RT pixels alone cannot satisfy this assertion.");
            }
            installer.SetApplicationPaused(false);
            installer.SetApplicationFocus(true);
            double resultDeadline = Time.realtimeSinceStartupAsDouble + 12;
            while (installer.Model.Phase == DevelopmentPreviewPhase.TrainRide && Time.realtimeSinceStartupAsDouble < resultDeadline) yield return null;
            Assert.That(installer.Model.Phase, Is.EqualTo(DevelopmentPreviewPhase.Results));
            Assert.That(installer.Model.IsAwaitingSaveCommit, Is.True);
            Assert.That(Find<VisualElement>("qa-result").style.display.value, Is.EqualTo(DisplayStyle.Flex));
            for (var frame = 0; frame < 2; frame++) yield return null;
            ScreenFrame? results = null;
            yield return CaptureScreen("wp038-screen-results.png", riding?.Frame ?? -1, value => results = value);
            if (results != null)
            {
                AssertScreenText(results, "qa-boundary");
                AssertScreenText(results, "qa-result-title");
                AssertScreenText(results, "qa-result-boundary");
                AssertOnScreen(results, Find<Button>("qa-restart"));
            }
            Submit("qa-restart");
            Assert.That(installer.Model.AttemptId, Is.Not.EqualTo(rideAttempt));
            Assert.That(installer.Model.Phase, Is.EqualTo(DevelopmentPreviewPhase.Editing));
            Assert.That(installer.Model.TimerStarted, Is.False);
            Assert.That(installer.Model.BuiltPath, Is.Empty);
            Assert.That(installer.Model.CompleteTrainRide(rideAttempt), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            yield return null;
            Assert.That(installer.Ride, Is.Null);
            Assert.That(ride.IsDisposed, Is.True);
            Assert.That(ownedPanel.targetTexture, Is.Null);
        }

        private void PrepareVisiblePanel()
        {
            installer.SetApplicationPaused(false);
            installer.SetApplicationFocus(true);
            installer.Document.rootVisualElement.style.width = 960;
            installer.Document.rootVisualElement.style.height = 840;
            installer.Presenter.Refresh();
        }

        private void PrepareColoredUi()
        {
            Submit("qa-tool-empty"); Submit("qa-cell-1-1");
            Submit("qa-tool-occupied"); Submit("qa-cell-1-2");
            Submit("qa-multiselect"); Submit("qa-cell-2-2");
            Assert.That(installer.Model.CellAt(1, 1), Is.EqualTo(DevelopmentTool.EmptyMarker));
            Assert.That(installer.Model.CellAt(1, 2), Is.EqualTo(DevelopmentTool.OccupiedMarker));
            Assert.That(installer.Presenter.Grid.IsCellSelected(2, 2), Is.True);
        }

        private void SolveUsingActualUi()
        {
            int size = installer.Model.Width;
            Submit("qa-tool-ew");
            for (var x = 0; x < size - 1; x++) Submit(PuzzleGridElement.CellName(x, 0));
            Submit("qa-tool-sw"); Submit(PuzzleGridElement.CellName(size - 1, 0));
            Submit("qa-tool-ns");
            for (var y = 1; y < size; y++) Submit(PuzzleGridElement.CellName(size - 1, y));
            Assert.That(installer.Model.Phase, Is.EqualTo(DevelopmentPreviewPhase.TrainRide));
        }

        private T Find<T>(string id) where T : VisualElement => installer.Document.rootVisualElement.Q<T>(id)
            ?? throw new InvalidOperationException("Missing actual QA control: " + id);

        private void Submit(string id) => Submit(Find<Button>(id));
        private static void Submit(Button button)
        {
            using var action = NavigationSubmitEvent.GetPooled();
            action.target = button;
            button.SendEvent(action);
        }

        private static IReadOnlyList<DevelopmentCell> ExpectedLPath(int size)
        {
            var cells = new List<DevelopmentCell>();
            for (var x = 0; x < size; x++) cells.Add(new DevelopmentCell(x, 0));
            for (var y = 1; y < size; y++) cells.Add(new DevelopmentCell(size - 1, y));
            return cells.AsReadOnly();
        }

        private static IEnumerable<(int X, int Y)> Project(IReadOnlyList<DevelopmentCell> cells) => cells.Select(cell => (cell.X, cell.Y));
        private static bool RequireGraphics() => string.Equals(Environment.GetEnvironmentVariable("STP_REQUIRE_GRAPHICS"), "1", StringComparison.Ordinal);

        private static void AssertTrainIsActuallyRendered(RenderTexture texture)
        {
            var previous = RenderTexture.active;
            var pixels = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            try
            {
                RenderTexture.active = texture;
                pixels.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
                pixels.Apply(false, false);
                int visiblePixels = pixels.GetPixels32().Count(color => color.a > 32 && color.r + color.g + color.b > 64);
                Assert.That(visiblePixels, Is.GreaterThan(30), "The real World target must contain the visible technical train, not only an allocated empty texture.");
            }
            finally
            {
                RenderTexture.active = previous;
                UnityEngine.Object.Destroy(pixels);
            }
        }

        private sealed class ScreenFrame
        {
            public readonly int Width;
            public readonly int Height;
            public readonly int Frame;
            public readonly Rect PanelBounds;
            public readonly Color32[] Pixels;
            public ScreenFrame(int width, int height, int frame, Rect panelBounds, Color32[] pixels)
            {
                Width = width; Height = height; Frame = frame; PanelBounds = panelBounds; Pixels = pixels;
            }
        }

        private IEnumerator CaptureScreen(string filename, int precedingFrame, Action<ScreenFrame> receive)
        {
            Assert.That(installer.Document.panelSettings, Is.SameAs(ownedPanel));
            Assert.That(ownedPanel.targetTexture, Is.Null, "Normal screen evidence must never redirect the UI panel into a texture.");
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            {
                Assert.That(RequireGraphics(), Is.False, "STP_REQUIRE_GRAPHICS=1 cannot report a normal-screen graphics pass with the null renderer.");
                Debug.Log("WP038 SCREEN GRAPHICS NOT_EXECUTED: null renderer; only real UI-loop semantics ran for " + filename);
                yield break;
            }
            Assert.That(UnityEngine.Application.isBatchMode, Is.False,
                "The actual normal-screen graphics route requires a non-batch Editor/Player with a rendering Game view. Headless CI still runs the UI semantics without claiming screen pixels.");
            Assert.That(Screen.width, Is.GreaterThan(0)); Assert.That(Screen.height, Is.GreaterThan(0));
            var camera = installer.ScreenCamera;
            Texture2D? pixels = null;
            Coroutine? screenshotRoutine = null;
            Exception? captureError = null;
            int requestedFrame = Time.frameCount;
            int actualFrame = -1;
            int renderedScreenFrame = -1;
            Rect panelBounds = default;
            bool captured = false;
            Action<ScriptableRenderContext, List<Camera>> onRendered = (context, cameras) =>
            {
                if (cameras.Contains(camera)) renderedScreenFrame = Time.frameCount;
            };
            IEnumerator CaptureAtEndOfFrame()
            {
                while (!captured && captureError == null)
                {
                    // Unity's documented screenshot route runs after the complete frame.
                    // The separate routine is stoppable if the Game view never renders;
                    // the test's normal-frame wait below still has an eight-second bound.
                    yield return new WaitForEndOfFrame();
                    if (Time.frameCount <= requestedFrame || renderedScreenFrame != Time.frameCount) continue;
                    try
                    {
                        if (ownedPanel.targetTexture != null || camera.targetTexture != null || !camera.enabled)
                            throw new InvalidOperationException("The actual normal-screen output boundary changed before capture.");
                        pixels = ScreenCapture.CaptureScreenshotAsTexture(1);
                        if (pixels == null || pixels.width != Screen.width || pixels.height != Screen.height)
                            throw new InvalidOperationException("The completed normal-screen screenshot does not match the actual Game view size.");
                        actualFrame = Time.frameCount;
                        panelBounds = installer.Document.rootVisualElement.panel.visualTree.worldBound;
                        captured = true;
                    }
                    catch (Exception error) { captureError = error; }
                }
            }
            try
            {
                RenderPipelineManager.endContextRendering += onRendered;
                screenshotRoutine = installer.StartCoroutine(CaptureAtEndOfFrame());
                double deadline = Time.realtimeSinceStartupAsDouble + 8;
                while (!captured && captureError == null && Time.realtimeSinceStartupAsDouble < deadline) yield return null;
                if (captureError != null) throw new InvalidOperationException("Actual screen capture failed.", captureError);
                Assert.That(captured, Is.True, "No completed Game view frame containing the actual screen camera reached the official end-of-frame screenshot route within 8 seconds.");
                Assert.That(actualFrame, Is.GreaterThan(requestedFrame));
                Assert.That(actualFrame, Is.GreaterThan(precedingFrame));
                Assert.That(ownedPanel.targetTexture, Is.Null);
                // CaptureScreenshotAsTexture owns the native screen-to-texture orientation.
                // No intermediate camera/panel target, RT ReadPixels or ad-hoc Y flip exists.
                var colors = pixels!.GetPixels32();
                var first = colors[0]; int different = 0;
                for (var index = 0; index < colors.Length; index++)
                    if (Math.Abs(colors[index].r - first.r) + Math.Abs(colors[index].g - first.g) + Math.Abs(colors[index].b - first.b) > 30) different++;
                Assert.That(different, Is.GreaterThan(5000), "The actual ScreenCapture image must show rendered UI, not a flat/null screen.");
                Assert.That(panelBounds.width, Is.GreaterThan(0)); Assert.That(panelBounds.height, Is.GreaterThan(0));
                string folder = Environment.GetEnvironmentVariable("STP_GRAPHICS_OUTPUT") ?? "TestResults/DevelopmentGraphics";
                Directory.CreateDirectory(folder);
                string output = Path.Combine(folder, filename);
                File.WriteAllBytes(output, pixels!.EncodeToPNG());
                Assert.That(new FileInfo(output).Length, Is.GreaterThan(5000));
                Debug.Log("WP038 actual normal-screen PNG via CaptureScreenshotAsTexture after WaitForEndOfFrame, rendered frame " + actualFrame + ", " + pixels.width + "x" + pixels.height + ": " + Path.GetFullPath(output));
                receive(new ScreenFrame(pixels.width, pixels.height, actualFrame, panelBounds, colors));
            }
            finally
            {
                RenderPipelineManager.endContextRendering -= onRendered;
                if (screenshotRoutine != null && installer != null) installer.StopCoroutine(screenshotRoutine);
                if (pixels != null) UnityEngine.Object.Destroy(pixels);
            }
        }

        private static void AssertOnScreen(ScreenFrame frame, VisualElement element)
        {
            var bounds = element.worldBound;
            Assert.That(bounds.width, Is.GreaterThan(0), element.name); Assert.That(bounds.height, Is.GreaterThan(0), element.name);
            Assert.That(bounds.xMin, Is.GreaterThanOrEqualTo(frame.PanelBounds.xMin - 1), element.name);
            Assert.That(bounds.yMin, Is.GreaterThanOrEqualTo(frame.PanelBounds.yMin - 1), element.name);
            Assert.That(bounds.xMax, Is.LessThanOrEqualTo(frame.PanelBounds.xMax + 1), element.name);
            Assert.That(bounds.yMax, Is.LessThanOrEqualTo(frame.PanelBounds.yMax + 1), element.name);
        }

        private void AssertScreenText(ScreenFrame frame, string id)
        {
            var label = Find<Label>(id);
            Assert.That(label.text, Is.Not.Empty);
            AssertOnScreen(frame, label);
            int glyphPixels = CountScreenPixels(frame, label, color => color.r < 125 && color.g < 125 && color.b < 125);
            int lightPixels = CountScreenPixels(frame, label, color => color.r > 150 && color.g > 150 && color.b > 150);
            Assert.That(glyphPixels, Is.GreaterThan(15), id + " must contain actual visible dark glyph pixels in the screen buffer.");
            Assert.That(lightPixels, Is.GreaterThan(50), id + " must contain actual visible light background pixels in the screen buffer.");
        }

        private void AssertEditingScreenOrientation(ScreenFrame frame)
        {
            Func<Color32, bool> redNote = color => color.r > 150 && color.g < 130 && color.b < 130 && color.r > color.g + 70 && color.r > color.b + 70;
            int redAtEmptyNote = CountScreenPixels(frame, Find<Button>("qa-cell-1-1"), redNote);
            int redAtOccupiedNote = CountScreenPixels(frame, Find<Button>("qa-cell-1-2"), redNote);
            Assert.That(redAtEmptyNote, Is.GreaterThan(15), "The actual screen must locate the red X inside cell (1,1); an upside-down screenshot must fail.");
            Assert.That(redAtOccupiedNote, Is.LessThanOrEqualTo(2), "Cell (1,2) holds the gray note, not the vertically reflected red X.");
            int selectionPixels = CountScreenPixels(frame, Find<Button>("qa-cell-2-2"),
                color => color.r > 235 && color.g > 190 && color.g < 240 && color.b > 50 && color.b < 150);
            Assert.That(selectionPixels, Is.GreaterThan(50), "The actual screen must locate yellow selection in cell (2,2).");
            var boundary = Find<Label>("qa-boundary");
            Color32 background = boundary.resolvedStyle.backgroundColor;
            int boundaryPixels = CountScreenPixels(frame, boundary, color =>
                Math.Abs(color.r - background.r) + Math.Abs(color.g - background.g) + Math.Abs(color.b - background.b) < 10);
            Assert.That(boundaryPixels, Is.GreaterThan(200), "The QA boundary's distinct background must be at its actual top-of-screen location.");
        }

        private static int CountScreenPixels(ScreenFrame frame, VisualElement element, Func<Color32, bool> predicate)
        {
            AssertOnScreen(frame, element);
            var bounds = element.worldBound;
            int xMin = Mathf.Clamp(Mathf.FloorToInt((bounds.xMin - frame.PanelBounds.xMin) / frame.PanelBounds.width * frame.Width), 0, frame.Width);
            int xMax = Mathf.Clamp(Mathf.CeilToInt((bounds.xMax - frame.PanelBounds.xMin) / frame.PanelBounds.width * frame.Width), 0, frame.Width);
            // UI Toolkit uses top-left panel coordinates; Texture2D.GetPixels32 stores the bottom row first.
            int yMin = Mathf.Clamp(Mathf.FloorToInt((frame.PanelBounds.yMax - bounds.yMax) / frame.PanelBounds.height * frame.Height), 0, frame.Height);
            int yMax = Mathf.Clamp(Mathf.CeilToInt((frame.PanelBounds.yMax - bounds.yMin) / frame.PanelBounds.height * frame.Height), 0, frame.Height);
            int count = 0;
            for (var y = yMin; y < yMax; y++)
                for (var x = xMin; x < xMax; x++)
                    if (predicate(frame.Pixels[y * frame.Width + x])) count++;
            return count;
        }

        private void CapturePanel(string filename)
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            {
                Assert.That(RequireGraphics(), Is.False, "A required graphics run must fail on the null renderer.");
                Debug.Log("WP038 GRAPHICS NOT_EXECUTED: null renderer; only actual UI semantics ran for " + filename);
                return;
            }
            Assert.That(panelTexture, Is.Not.Null); Assert.That(panelTexture!.IsCreated(), Is.True);
            Assert.That(installer.Document.panelSettings.targetTexture, Is.SameAs(panelTexture));
            Assert.That(installer.Presenter.Grid.Board.resolvedStyle.width, Is.GreaterThan(0));
            var previous = RenderTexture.active;
            var pixels = new Texture2D(960, 840, TextureFormat.RGBA32, false);
            try
            {
                RenderTexture.active = panelTexture;
                pixels.ReadPixels(new Rect(0, 0, 960, 840), 0, 0);
                pixels.Apply(false, false);
                var colors = pixels.GetPixels32();
                var first = colors[0]; int different = 0;
                for (var index = 0; index < colors.Length; index++)
                    if (Math.Abs(colors[index].r - first.r) + Math.Abs(colors[index].g - first.g) + Math.Abs(colors[index].b - first.b) > 30) different++;
                Assert.That(different, Is.GreaterThan(5000), "The actual panel image must contain rendered UI rather than a flat/null target.");
                string folder = Environment.GetEnvironmentVariable("STP_GRAPHICS_OUTPUT") ?? "TestResults/DevelopmentGraphics";
                Directory.CreateDirectory(folder);
                string output = Path.Combine(folder, filename);
                File.WriteAllBytes(output, pixels.EncodeToPNG());
                Assert.That(new FileInfo(output).Length, Is.GreaterThan(5000));
                Debug.Log("WP038 actual rendered QA panel PNG: " + Path.GetFullPath(output));
            }
            finally
            {
                RenderTexture.active = previous;
                UnityEngine.Object.Destroy(pixels);
            }
        }

#if UNITY_EDITOR
        private static T OwnAsset<T>(string path) where T : UnityEngine.Object => AssetDatabase.LoadAssetAtPath<T>(path)
            ?? throw new InvalidOperationException("Missing own QA asset: " + path);

        private static void InvokeBuilder(string methodName)
        {
            var type = Type.GetType("STP.Editor.Build.DevelopmentPuzzleBuild, STP.Editor.Build", true)!;
            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)!;
            try { method.Invoke(null, null); }
            catch (TargetInvocationException error) when (error.InnerException != null) { throw error.InnerException; }
        }
#endif
    }
}
