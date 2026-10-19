using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;
using STP.Bootstrap;
using STP.Infrastructure.Content;
using STP.Infrastructure.Persistence;
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
    /// <summary>Actual saved scene, real isolated files and real UI commands; no fake acknowledgement or alternate catalog.</summary>
    public sealed class SavedPublic240ReviewSmokeTests
    {
        private const string ScenePath = "Assets/StammstreckenPuzzle/Scenes/QA/SavedPublic240Review.unity";
        private SavedPublic240ReviewInstaller installer = null!;
        private GameObject? host;
        private PanelSettings? panel;
        private IDisposable? scope;
        private string root = string.Empty;
        private string originalRoot = string.Empty;
        private string originalRootParent = string.Empty;
        private string originalRootLeaf = string.Empty;
        private bool ownsRoot;
        private LockOwner? lockOwner;
        private static bool RequireGraphics => Environment.GetEnvironmentVariable("STP_REQUIRE_GRAPHICS") == "1";

        [UnitySetUp]
        public IEnumerator ActualSavedSceneInPrivateScope()
        {
            if (RequireGraphics) Assert.That(SystemInfo.graphicsDeviceType, Is.Not.EqualTo(GraphicsDeviceType.Null));
            ownsRoot = false;
            originalRootLeaf = "stpi-" + Guid.NewGuid().ToString("N").Substring(0, 6);
            originalRootParent = Path.GetFullPath(Environment.GetEnvironmentVariable("STP_TEST_TEMP_ROOT") ?? Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            root = Path.GetFullPath(Path.Combine(originalRootParent, originalRootLeaf));
            originalRoot = root;
            if (Directory.Exists(root) || File.Exists(root)) throw new InvalidOperationException("SAVED_TEST_ROOT_ALREADY_EXISTS");
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
            // This guard performs physical Temp/AppData disjointness BEFORE its own worker provisioning.
            // A rejected environment root must cause neither provisioning nor later cleanup deletion.
            scope = SavedPublic240ReviewInstaller.UseTestPrivateRoot(root);
            ownsRoot = true;
#else
            Assert.Ignore("Saved scene filesystem probes require the guarded Editor test-root API.");
#endif
            // Scope precedes the actual scene's Awake, never sceneLoaded or a player entry assumption.
            yield return LoadActualScene();
        }

        private IEnumerator LoadActualScene()
        {
#if UNITY_EDITOR
            InvokeBuilder("VerifyArtifacts");
            EditorSceneManager.LoadSceneInPlayMode(ScenePath, new LoadSceneParameters(LoadSceneMode.Single));
#else
            SceneManager.LoadScene(ScenePath, LoadSceneMode.Single);
#endif
            yield return null;
            installer = UnityEngine.Object.FindFirstObjectByType<SavedPublic240ReviewInstaller>()!;
            Assert.That(installer, Is.Not.Null); host = installer.gameObject;
            panel = UnityEngine.Object.Instantiate(installer.Document.panelSettings);
            panel.targetTexture = null; installer.Document.panelSettings = panel;
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
                yield return BindHeadlessDocumentToActualPanel();
            yield return Await(installer.InitializeTask);
            installer.SetApplicationFocus(true); installer.SetApplicationPaused(false);
            yield return Idle();
            Assert.That(installer.Composition.PrivateRoot, Is.EqualTo(root));
            Assert.That(installer.Composition.Profile.ProfileId, Is.EqualTo("r"));
            Assert.That(installer.Composition.Profile.Use, Is.EqualTo(CatalogUse.IsolatedReview));
            Assert.That(installer.Composition.WorkerThreadId, Is.Not.EqualTo(Thread.CurrentThread.ManagedThreadId));
        }

        private IEnumerator BindHeadlessDocumentToActualPanel()
        {
            var documentRoot = installer.Document.rootVisualElement;
            double deadline = Time.realtimeSinceStartupAsDouble + 5;
            Rect? boundPanel = null;
            Rect lastPanel = default;
            int settledFrames = 0;
            while (Time.realtimeSinceStartupAsDouble < deadline)
            {
                if (documentRoot.panel == null) { yield return null; continue; }
                var actualPanel = documentRoot.panel.visualTree.worldBound;
                lastPanel = actualPanel;
                if (!FinitePositiveBounds(actualPanel)) { settledFrames = 0; yield return null; continue; }
                if (!boundPanel.HasValue || !boundPanel.Value.Equals(actualPanel))
                {
                    documentRoot.style.width = actualPanel.width;
                    documentRoot.style.height = actualPanel.height;
                    boundPanel = actualPanel; settledFrames = 0;
                    TestContext.WriteLine("SAVED_HEADLESS_PANEL_BIND frame=" + Time.frameCount + " actualPanel=" + actualPanel);
                    yield return null; continue;
                }
                // Require two layout frames after binding; a changed panel restarts binding within the same deadline.
                if (++settledFrames < 2) { yield return null; continue; }
                var scroll = Find<ScrollView>("saved-scroll");
                TestContext.WriteLine("SAVED_HEADLESS_PANEL_CONTAINMENT frame=" + Time.frameCount + " panel=" + actualPanel
                    + " root=" + documentRoot.worldBound + " scroll=" + scroll.worldBound + " viewport=" + scroll.contentViewport.worldBound);
                AssertBoundsWithinActualPanel(actualPanel, documentRoot.worldBound, "document root");
                AssertBoundsWithinActualPanel(actualPanel, scroll.worldBound, "saved scroll");
                AssertBoundsWithinActualPanel(actualPanel, scroll.contentViewport.worldBound, "content viewport");
                yield break;
            }
            Assert.Fail("SAVED_HEADLESS_PANEL_BOUNDS_NOT_STABLE_OR_VALID: lastPanel=" + lastPanel);
        }
        private static bool FinitePositiveBounds(Rect bounds)
        {
            return Finite(bounds.x) && Finite(bounds.y) && Finite(bounds.width) && Finite(bounds.height)
                && Finite(bounds.xMax) && Finite(bounds.yMax) && bounds.width > 0 && bounds.height > 0;
        }
        private static bool Finite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
        private static void AssertBoundsWithinActualPanel(Rect actualPanel, Rect bounds, string label)
        {
            Assert.That(FinitePositiveBounds(actualPanel), Is.True, "actual panel finite positive bounds");
            Assert.That(FinitePositiveBounds(bounds), Is.True, label + " finite positive bounds");
            Assert.That(actualPanel.Contains(bounds.min), Is.True, label + " top-left inside actual panel");
            Assert.That(actualPanel.Contains(bounds.max - new Vector2(.1f, .1f)), Is.True, label + " bottom-right inside actual panel");
        }

        [UnityTearDown]
        public IEnumerator DisposeAndWaitBeforeRootCleanup()
        {
            if (lockOwner != null) { lockOwner.Release(); yield return WaitOwner(lockOwner); lockOwner.Dispose(); lockOwner = null; }
            if (host != null) yield return DestroyInstaller();
            scope?.Dispose(); scope = null;
            if (ownsRoot && Directory.Exists(root))
            {
                ValidateOwnCleanupRoot();
                Directory.Delete(root, true);
            }
            ownsRoot = false;
            yield return null;
        }

        private IEnumerator DestroyInstaller()
        {
            var old = installer; if (host != null) UnityEngine.Object.Destroy(host); host = null;
            yield return null;
            yield return Await(old.ShutdownTask);
            if (panel != null) UnityEngine.Object.Destroy(panel); panel = null;
            yield return null;
        }

        private void ValidateOwnCleanupRoot()
        {
            var comparison = Path.DirectorySeparatorChar == '\\' ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (!ownsRoot || !Path.IsPathRooted(root) || !string.Equals(Path.GetFullPath(root), originalRoot, comparison)
                || !string.Equals(Path.GetDirectoryName(originalRoot), originalRootParent, comparison)
                || Path.GetFileName(originalRoot) != originalRootLeaf || !originalRootLeaf.StartsWith("stpi-", StringComparison.Ordinal)
                || originalRootLeaf.Length != 11 || originalRootLeaf.Substring(5).Any(c => !(c >= '0' && c <= '9' || c >= 'a' && c <= 'f')))
                throw new InvalidOperationException("SAVED_TEST_CLEANUP_ROOT_CHANGED");
            // Refuse deletion if an ancestor or any owned descendant acquired a link after the setup guard.
            for (string? path = originalRoot; !string.IsNullOrEmpty(path); path = Path.GetDirectoryName(path))
                if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
                    throw new InvalidOperationException("SAVED_TEST_CLEANUP_REPARSE_REJECTED");
            var pending = new Stack<string>(); pending.Push(originalRoot);
            while (pending.Count > 0)
                foreach (string path in Directory.GetFileSystemEntries(pending.Pop()))
                {
                    var attributes = File.GetAttributes(path);
                    if ((attributes & FileAttributes.ReparsePoint) != 0) throw new InvalidOperationException("SAVED_TEST_CLEANUP_REPARSE_REJECTED");
                    if ((attributes & FileAttributes.Directory) != 0) pending.Push(path);
                }
        }

        [UnityTest]
        public IEnumerator ColdLoad_ThreeMissing_NoProfileWrite_UntilExplicitCreate()
        {
            Assert.That(installer.Model.State.Status, Is.EqualTo(SavedReviewStatus.NeedsInitialization));
            Assert.That(installer.Model.PuzzleView, Is.Null); Assert.That(installer.Presenter.Core, Is.Null);
            Assert.That(Directory.GetFiles(root, "*", SearchOption.AllDirectories), Is.Empty);
            Assert.That(installer.Model.State.CanCreateProfile, Is.True);
            Submit("saved-create"); yield return Idle();
            Assert.That(installer.Model.State.Status, Is.EqualTo(SavedReviewStatus.Ready));
            Assert.That(installer.Model.State.Progress!.Generation, Is.EqualTo(1));
            Assert.That(installer.Model.State.Progress.FirstClearCount, Is.Zero);
            Assert.That(installer.Model.State.Progress.BalancePatience, Is.Zero);
            Assert.That(installer.Model.State.SelectedIndex, Is.EqualTo(-1));
            Assert.That(installer.Model.PuzzleView, Is.Null);
            Assert.That(File.Exists(MainPath), Is.True);
        }

#if UNITY_EDITOR
        [UnityTest]
        public IEnumerator ActualWorkerCompositionFailure_ShowsLocalizedStartupBeforeAnyBridgeOrProfile()
        {
            yield return DestroyInstaller();
            var broken = new TextAsset("{}");
            var go = new GameObject("SavedBrokenComposition"); go.SetActive(false); host = go;
            installer = go.AddComponent<SavedPublic240ReviewInstaller>();
            string u = "Assets/StammstreckenPuzzle/UI/";
            installer.ConfigureForQa(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(u + "SavedPublic240Review.uxml"),
                AssetDatabase.LoadAssetAtPath<StyleSheet>(u + "SavedPublic240Review.uss"),
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(u + "Public240Review.uxml"),
                AssetDatabase.LoadAssetAtPath<StyleSheet>(u + "Public240Review.uss"),
                AssetDatabase.LoadAssetAtPath<TextAsset>(u + "SavedPublic240ReviewLocale.json"),
                AssetDatabase.LoadAssetAtPath<PanelSettings>(u + "Public240ReviewPanelSettings.asset"),
                AssetDatabase.LoadAssetAtPath<Material>("Assets/StammstreckenPuzzle/Art/Development/DevelopmentTrain.mat"), broken,
                AssetDatabase.LoadAssetAtPath<TextAsset>(u + "ReviewCampaign.catalog.json"), AssetDatabase.LoadAssetAtPath<TextAsset>(u + "ReviewCompletion.catalog.json"));
            try
            {
                go.SetActive(true); yield return Await(installer.InitializeTask); yield return null;
                Assert.That(installer.ModelOrNull, Is.Null); Assert.That(installer.Presenter.HasBoundModel, Is.False);
                Assert.That(installer.Presenter.StartupPhase, Is.EqualTo(SavedStartupPhase.Failed));
                Assert.That(Find<Label>("saved-status").text, Does.Contain("Quellen sind ungültig"));
                Assert.That(Find<Button>("saved-startup-retry").enabledSelf, Is.True);
                Assert.That(Find<VisualElement>("saved-game-host").childCount, Is.Zero);
                Assert.That(Directory.GetFiles(root, "*", SearchOption.AllDirectories), Is.Empty);
                Submit("saved-startup-retry"); yield return Await(installer.InitializeTask); yield return null;
                Assert.That(installer.ModelOrNull, Is.Null); Assert.That(installer.Presenter.StartupPhase, Is.EqualTo(SavedStartupPhase.Failed));
            }
            finally { UnityEngine.Object.Destroy(broken); }
        }
#endif

        [UnityTest]
        public IEnumerator DropdownIsTentative_All240_ExplicitNewAndResumePreserveExactDraft()
        {
            yield return CreateAndOpen();
            Submit("qa-tool-empty"); Submit("qa-cell-1-1");
            installer.SetApplicationPaused(true); yield return Idle(); installer.SetApplicationPaused(false); yield return Idle();
            var view = installer.Model.PuzzleView!; var attempt = view.AttemptId;
            var capturedBeforeNavigation = view.CaptureCommand();
            Assert.That(capturedBeforeNavigation.AttemptId, Is.EqualTo(attempt));
            long generation = installer.Model.State.Progress!.Generation;
            byte[] durable = File.ReadAllBytes(MainPath);
            var picker = Find<DropdownField>("saved-picker"); Assert.That(picker.choices.Count, Is.EqualTo(240));
            int large = LargeIndex;
            picker.index = large; installer.Presenter.Refresh();
            Assert.That(installer.Model.State.SelectedIndex, Is.Zero);
            Assert.That(installer.Model.PuzzleView, Is.SameAs(view));
            Assert.That(installer.Model.State.Progress.Generation, Is.EqualTo(generation));
            Assert.That(File.ReadAllBytes(MainPath), Is.EqualTo(durable));
            picker.index = 0; installer.Presenter.Refresh();
            Assert.That(installer.Model.HasDraftForPuzzle(0), Is.True);
            Assert.That(Find<Button>("saved-resume").enabledSelf, Is.True);
            Submit("saved-resume"); yield return Idle();
            Assert.That(installer.Model.PuzzleView!.AttemptId, Is.EqualTo(attempt));
            Assert.That(installer.Model.PuzzleView.CellAt(1, 1), Is.EqualTo(DevelopmentTool.EmptyMarker));
            picker.index = large; Submit("saved-open"); yield return Idle();
            Assert.That(installer.Model.State.SelectedIndex, Is.EqualTo(large));
            Assert.That(installer.Presenter.Core!.Grid.GridWidth, Is.EqualTo(10));
            var staleCapture = Assert.Throws<InvalidOperationException>(() => view.CaptureCommand());
            Assert.That(staleCapture!.Message, Is.EqualTo("STALE_ATTEMPT"));
            Assert.That(view.AttemptId, Is.EqualTo(attempt));
            Assert.That(capturedBeforeNavigation.AttemptId, Is.EqualTo(attempt));
            var currentView = installer.Model.PuzzleView!;
            Assert.That(currentView.AttemptId, Is.Not.EqualTo(attempt));
            long currentRevision = currentView.Revision;
            var currentCell = currentView.CellAt(0, 0);
            Assert.That(view.Apply(0, 0, DevelopmentTool.EmptyMarker, installer.NowMilliseconds, capturedBeforeNavigation), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(view.LastErrorCode, Is.EqualTo("STALE_ATTEMPT"));
            Assert.That(currentView.Apply(0, 0, DevelopmentTool.EmptyMarker, installer.NowMilliseconds, capturedBeforeNavigation), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(currentView.LastErrorCode, Is.EqualTo("STALE_ATTEMPT"));
            Assert.That(currentView.AttemptId, Is.EqualTo(installer.Model.PuzzleView!.AttemptId));
            Assert.That(currentView.Revision, Is.EqualTo(currentRevision));
            Assert.That(currentView.CellAt(0, 0), Is.EqualTo(currentCell));
            Assert.That(view.CompleteTrainRide(attempt), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            picker.index = 0; Submit("saved-open"); yield return Idle();
            Assert.That(installer.Model.PuzzleView!.AttemptId, Is.Not.EqualTo(attempt));
            Assert.That(installer.Model.PuzzleView.CellAt(1, 1), Is.EqualTo(DevelopmentTool.Clear));
        }

        [UnityTest]
        public IEnumerator RealMarkerClearUndoPause_ColdRestorePreservesStickyRevisionDedupAndUndo()
        {
            yield return CreateAndOpen();
            Submit("qa-tool-empty"); Submit("qa-cell-1-1");
            Submit("qa-tool-clear"); Submit("qa-cell-1-1");
            Submit("qa-tool-occupied"); Submit("qa-cell-1-2"); Submit("qa-undo");
            Submit("qa-tool-empty"); Submit("qa-cell-2-3");
            installer.SetApplicationFocus(false); yield return Idle();
            var old = installer.Model.PuzzleView!; var attempt = old.AttemptId; long revision = old.Revision;
            var session = Property(installer.Composition.Persisted, "Session");
            var before = CaptureSessionFacts(session);
            long time = old.ElapsedAt(installer.NowMilliseconds);
            yield return DestroyInstaller();
            for (int i = 0; i < 3; i++) yield return null;
            yield return LoadActualScene();
            Assert.That(installer.Model.PuzzleView, Is.Null);
            Assert.That(installer.Model.HasDraftForPuzzle(0), Is.True);
            Submit("saved-resume"); yield return Idle();
            var restored = installer.Model.PuzzleView!;
            Assert.That(restored.AttemptId, Is.EqualTo(attempt));
            Assert.That(restored.Revision, Is.GreaterThanOrEqualTo(revision));
            Assert.That(restored.CellAt(2, 3), Is.EqualTo(DevelopmentTool.EmptyMarker));
            Assert.That(restored.CellAt(1, 2), Is.EqualTo(DevelopmentTool.Clear));
            Assert.That(restored.CanUndo, Is.True);
            var after = CaptureSessionFacts(Property(installer.Composition.Persisted, "Session"));
            foreach (var key in before.Keys.Where(key => key != "Revision" && key != "AppliedCommandIds")) Assert.That(after[key], Is.EqualTo(before[key]), key);
            Assert.That(before["AppliedCommandIds"].Split('|').All(id => after["AppliedCommandIds"].Split('|').Contains(id)), Is.True, "Every prior dedup GUID survives; new lifecycle commands may append IDs.");
            Assert.That(restored.ElapsedAt(installer.NowMilliseconds), Is.LessThan(time + 1500));
            Submit("qa-undo");
            Assert.That(restored.CellAt(2, 3), Is.EqualTo(DevelopmentTool.Clear), "Restored actual undo stack must affect the real session.");
            Assert.That(Property(Property(installer.Composition.Persisted, "Session"), "UsedEmptyMarker"), Is.EqualTo(true));
            Assert.That(Property(Property(installer.Composition.Persisted, "Session"), "UsedOccupiedMarker"), Is.EqualTo(true));
        }

        [UnityTest]
        public IEnumerator ActualBatchPointerNoopAndOrSuspension_UseUnchangedCoreAndRealAutosave()
        {
            yield return CreateAndOpen();
            var view = installer.Model.PuzzleView!; long revision = view.Revision;
            Submit("qa-tool-clear"); Submit("qa-cell-0-3");
            Assert.That(view.Revision, Is.EqualTo(revision)); Assert.That(view.TimerStarted, Is.False);
            Submit("qa-multiselect"); Submit("qa-cell-0-3"); Submit("qa-cell-1-3");
            Assert.That(view.Revision, Is.EqualTo(revision));
            Submit("qa-tool-occupied"); Submit("qa-apply-selection");
            Assert.That(view.RowVisible(3), Is.EqualTo(2)); Assert.That(view.Revision, Is.EqualTo(revision + 1));
            Submit("qa-undo"); Submit("qa-multiselect"); Submit("qa-tool-empty");
            yield return null; PointerClick(installer.Presenter.Core!.Grid.CellButton(1, 1)); yield return null;
            Assert.That(view.CellAt(1, 1), Is.EqualTo(DevelopmentTool.EmptyMarker));
            installer.SetApplicationPaused(true); installer.SetApplicationFocus(false); yield return Idle();
            long paused = view.ElapsedAt(installer.NowMilliseconds);
            installer.SetApplicationFocus(true); yield return Idle();
            Assert.That(installer.IsSuspended, Is.True);
            Assert.That(view.ElapsedAt(installer.NowMilliseconds + 10000), Is.EqualTo(paused));
            installer.SetApplicationPaused(false); yield return Idle();
            Assert.That(installer.IsSuspended, Is.False);
            Assert.That(installer.Model.State.Progress!.Generation, Is.GreaterThan(2));
        }

        [UnityTest]
        public IEnumerator RealWriterOwnerThread_HoldsAck_MainKeepsPumping_NoRideOrResultsBeforeCommit()
        {
            yield return CreateAndOpen();
            byte[] before = File.ReadAllBytes(MainPath); long generation = installer.Model.State.Progress!.Generation;
            lockOwner = new LockOwner(installer.Composition.Files); lockOwner.Start();
            yield return WaitReady(lockOwner);
            SolveFirst();
            int frame = Time.frameCount;
            for (int i = 0; i < 8; i++) yield return null;
            Assert.That(Time.frameCount, Is.GreaterThan(frame)); Assert.That(lockOwner.Held, Is.True);
            Assert.That(installer.Model.State.CanStartRide, Is.False); Assert.That(installer.Ride, Is.Null);
            Assert.That(installer.Model.State.Progress!.Generation, Is.EqualTo(generation));
            Assert.That(installer.Model.State.Progress.BalancePatience, Is.Zero);
            Assert.That(Find<VisualElement>("qa-result").resolvedStyle.display, Is.EqualTo(DisplayStyle.None));
            Assert.That(File.ReadAllBytes(MainPath), Is.EqualTo(before));
            Assert.That(Find<Button>("qa-tool-empty").enabledInHierarchy, Is.False);
            lockOwner.Release(); yield return WaitOwner(lockOwner); lockOwner.Dispose(); lockOwner = null;
            yield return Results();
            Assert.That(installer.Model.State.Progress!.BalancePatience, Is.EqualTo(15));
            Assert.That(installer.Model.State.Progress.SelectedStars, Is.EqualTo(1));
            Assert.That(installer.Model.State.Progress.FirstClearCount, Is.EqualTo(1));
            Assert.That(Find<Label>("saved-stars").text, Does.Contain("1/3"));
        }

        [UnityTest]
        public IEnumerator PendingDirectoryObstacle_RealWriterFailure_NoFalseResult_ExplicitRetryPaysOnce()
        {
            yield return CreateAndOpen();
            string pending = Path.Combine(installer.Composition.Files.ProfileDirectory, "save.pending.json");
            byte[] before = File.ReadAllBytes(MainPath);
            byte[]? originalPending = PreparePendingDirectoryObstacle(pending);
            SolveFirst(); yield return Idle();
            Assert.That(installer.Ride, Is.Null); Assert.That(installer.Model.State.CanStartRide, Is.False);
            Assert.That(installer.Model.State.Status, Is.Not.EqualTo(SavedReviewStatus.Results));
            Assert.That(File.ReadAllBytes(MainPath), Is.EqualTo(before));
            Assert.That(Find<Label>("saved-error").text, Is.Not.Empty);
            Assert.That(installer.Model.PuzzleView, Is.Not.Null);
            Directory.Delete(pending);
            if (originalPending != null) File.WriteAllBytes(pending, originalPending);
            Assert.That(installer.Model.State.CanRetrySave, Is.False, "A Failed read candidate requires a fresh load, never a direct save retry.");
            Submit("saved-reload"); yield return Idle();
            Assert.That(installer.Model.State.Status, Is.EqualTo(SavedReviewStatus.Ready));
            Assert.That(installer.Model.PuzzleView, Is.Null);
            Submit("saved-open"); yield return Idle(); SolveFirst(); yield return Results();
            Assert.That(installer.Model.State.Progress!.BalancePatience, Is.EqualTo(15));
            Submit("saved-restart"); yield return Idle(); SolveFirst(); yield return Results();
            Assert.That(installer.Model.State.Progress!.BalancePatience, Is.EqualTo(15));
            Assert.That(installer.Model.State.Progress.FirstClearCount, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator SplitBrain_VisibleThreeCandidates_NoEffectsUntilConfirm_ExactPromotionPreservesSources()
        {
            Submit("saved-create"); yield return Idle();
            var snapshot = installer.Composition.Persisted.Snapshot!;
            byte[] original = File.ReadAllBytes(MainPath);
            byte[] backup = AlternativeBytes(snapshot);
            File.WriteAllBytes(Path.Combine(installer.Composition.Files.ProfileDirectory, "save.backup.json"), backup);
            Submit("saved-reload"); yield return Idle();
            Assert.That(installer.Model.State.LoadStatus, Is.EqualTo(SaveLoadStatus.SplitBrain));
            Assert.That(installer.Model.State.Progress, Is.Null); Assert.That(installer.Model.State.CanMutate, Is.False);
            Assert.That(Find<VisualElement>("saved-candidates").childCount, Is.EqualTo(3));
            var tree = TreeBytes();
            Submit("saved-candidate-Backup"); Submit("saved-recover"); Submit("saved-cancel");
            AssertTreeEqual(tree, TreeBytes());
            Submit("saved-recover"); Submit("saved-confirm"); yield return Idle();
            Assert.That(installer.Model.State.Status, Is.EqualTo(SavedReviewStatus.Ready));
            Assert.That(installer.Model.State.Progress!.Generation, Is.EqualTo(snapshot.Generation + 1));
            Assert.That(installer.Composition.Persisted.Snapshot!.Payload.Settings.HapticsEnabled, Is.EqualTo(!(snapshot.Payload.Settings.HapticsEnabled ?? false)));
            Assert.That(installer.Composition.Persisted.Snapshot.Payload.Settings.Locale, Is.EqualTo(snapshot.Payload.Settings.Locale));
            var files = Directory.GetFiles(installer.Composition.Files.ProfileDirectory, "*.json", SearchOption.AllDirectories).Select(File.ReadAllBytes).ToArray();
            Assert.That(files.Any(bytes => bytes.SequenceEqual(original)), Is.True);
            Assert.That(files.Any(bytes => bytes.SequenceEqual(backup)), Is.True);
            Assert.That(installer.Model.PuzzleView, Is.Null);
        }

        [UnityTest]
        public IEnumerator CorruptReset_RequiresSeparateConfirmation_PreservesExactOriginals()
        {
            Submit("saved-create"); yield return Idle();
            var corruptCandidates = new List<byte[]>();
            foreach (string filename in new[] { "save.json", "save.backup.json", "save.pending.json" })
            {
                string path = Path.Combine(installer.Composition.Files.ProfileDirectory, filename);
                PreserveFixtureOriginal(path);
                byte[] corrupt = System.Text.Encoding.UTF8.GetBytes("{broken-source-" + filename);
                File.WriteAllBytes(path, corrupt); corruptCandidates.Add(corrupt);
                Assert.That(Codec().Decode(File.ReadAllBytes(path)).Status, Is.EqualTo(LocalSaveDecodeStatus.Corrupt), filename);
            }
            Submit("saved-reload"); yield return Idle();
            Assert.That(installer.Model.State.LoadStatus, Is.EqualTo(SaveLoadStatus.Corrupt));
            Assert.That(installer.Model.State.CanReset, Is.True); Assert.That(installer.Model.State.CanCreateProfile, Is.False);
            var before = TreeBytes(); Submit("saved-reset"); Submit("saved-cancel"); AssertTreeEqual(before, TreeBytes());
            Submit("saved-reset"); Submit("saved-confirm"); yield return Idle();
            Assert.That(installer.Model.State.Status, Is.EqualTo(SavedReviewStatus.Ready));
            Assert.That(installer.Model.State.Progress!.BalancePatience, Is.Zero);
            var preserved = Directory.GetFiles(installer.Composition.Files.ProfileDirectory, "*.json", SearchOption.AllDirectories).Select(File.ReadAllBytes).ToArray();
            foreach (byte[] corrupt in corruptCandidates) Assert.That(preserved.Any(bytes => bytes.SequenceEqual(corrupt)), Is.True);
        }

        [UnityTest]
        public IEnumerator RecoveryChoice_BindsNonselectedSourceBytes_StaleConfirmCannotPromote()
        {
            Submit("saved-create"); yield return Idle();
            byte[] original = File.ReadAllBytes(MainPath);
            string backup = Path.Combine(installer.Composition.Files.ProfileDirectory, "save.backup.json");
            byte[] alternative = AlternativeBytes(installer.Composition.Persisted.Snapshot!);
            File.WriteAllBytes(backup, alternative);
            Submit("saved-reload"); yield return Idle();
            Assert.That(installer.Model.State.CanRecover, Is.True);
            Submit("saved-candidate-Main"); Submit("saved-recover");
            // Change only a NONselected candidate's source bytes after the actual visible decision.
            byte[] changed = alternative.Concat(new byte[] { 32 }).ToArray(); File.WriteAllBytes(backup, changed);
            Submit("saved-confirm"); yield return Idle();
            Assert.That(installer.Model.State.LoadStatus, Is.EqualTo(SaveLoadStatus.Conflict));
            Assert.That(installer.Model.State.CanMutate, Is.False);
            Assert.That(File.ReadAllBytes(MainPath), Is.EqualTo(original));
            Assert.That(File.ReadAllBytes(backup), Is.EqualTo(changed));
            Assert.That(installer.Ride, Is.Null);
        }

        [UnityTest]
        public IEnumerator UnsupportedOrFailedCandidate_VisibleAndProtected_NoResetCreateOrGameplay()
        {
            Submit("saved-create"); yield return Idle();
            byte[] good = File.ReadAllBytes(MainPath);
            byte[] newer = System.Text.Encoding.UTF8.GetBytes(System.Text.Encoding.UTF8.GetString(good).Replace("\"saveSchemaVersion\":2", "\"saveSchemaVersion\":99"));
            Assert.That(newer, Is.Not.EqualTo(good)); File.WriteAllBytes(MainPath, newer);
            Submit("saved-reload"); yield return Idle();
            Assert.That(installer.Model.State.Status, Is.EqualTo(SavedReviewStatus.Unsupported));
            Assert.That(installer.Model.State.CanReset, Is.False); Assert.That(installer.Model.State.CanCreateProfile, Is.False);
            Assert.That(installer.Model.State.CanMutate, Is.False); Assert.That(File.ReadAllBytes(MainPath), Is.EqualTo(newer));
            File.WriteAllBytes(MainPath, good);
            string pending = Path.Combine(installer.Composition.Files.ProfileDirectory, "save.pending.json");
            byte[]? originalPending = PreparePendingDirectoryObstacle(pending);
            Submit("saved-reload"); yield return Idle();
            Assert.That(installer.Model.State.CanRecover, Is.False); Assert.That(installer.Model.State.CanReset, Is.False);
            Assert.That(installer.Model.State.CanMutate, Is.False); Directory.Delete(pending);
            if (originalPending != null) File.WriteAllBytes(pending, originalPending);
        }

        [UnityTest]
        public IEnumerator BoundRequestFaultCancelAndSynchronousThrow_KeepOuterReloadReachable_CompletionIsNotAck()
        {
            yield return CreateAndOpen();
#if UNITY_EDITOR
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/StammstreckenPuzzle/UI/SavedPublic240Review.uxml").CloneTree();
            installer.Document.rootVisualElement.Add(tree);
            Task next = Task.FromException(new InvalidOperationException("RAW_EXCEPTION_MUST_NOT_RENDER")); int calls = 0; bool throwSynchronously = false;
            using var own = new SavedPublic240ReviewPresenter(tree,
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/StammstreckenPuzzle/UI/Public240Review.uxml"),
                AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/StammstreckenPuzzle/UI/SavedPublic240ReviewLocale.json"),
                () => installer.NowMilliseconds, _ => { calls++; if (throwSynchronously) throw new InvalidOperationException("RAW_SYNC_EXCEPTION"); return next; });
            own.BindModel(installer.Model);
            Assert.Throws<InvalidOperationException>(() => own.BindModel(installer.Model));
            Assert.Throws<InvalidOperationException>(() => own.ShowStartupFailure(SavedStartupFailure.CompositionFailed));
            SubmitElement(tree.Q<Button>("saved-reload")); own.Refresh();
            Assert.That(own.HasRequestFault, Is.True); Assert.That(calls, Is.EqualTo(1));
            Assert.That(tree.Q<Label>("saved-error").text, Does.Not.Contain("RAW_EXCEPTION"));
            Assert.That(tree.Q<VisualElement>("saved-game-host").enabledInHierarchy, Is.False);
            Assert.That(tree.Q<Button>("saved-reload").enabledSelf, Is.True);
            next = Task.CompletedTask; SubmitElement(tree.Q<Button>("saved-reload")); own.Refresh();
            Assert.That(own.HasRequestFault, Is.True, "Task success alone is no fresh load or save ACK.");
            var cancel = new CancellationTokenSource(); cancel.Cancel(); next = Task.FromCanceled(cancel.Token);
            SubmitElement(tree.Q<Button>("saved-reload")); own.Refresh();
            Assert.That(tree.Q<Label>("saved-error").text, Does.Contain("abgebrochen"));
            Assert.That(tree.Q<Button>("saved-reload").enabledSelf, Is.True);
            throwSynchronously = true; SubmitElement(tree.Q<Button>("saved-reload")); own.Refresh();
            Assert.That(tree.Q<Label>("saved-error").text, Does.Not.Contain("RAW_SYNC"));
            Assert.That(own.HasRequestFault, Is.True); Assert.That(tree.Q<Button>("saved-reload").enabledSelf, Is.True);
            throwSynchronously = false; var late = new TaskCompletionSource<bool>(); next = late.Task;
            SubmitElement(tree.Q<Button>("saved-reload"));
            Assert.That(own.PendingRequest, Is.SameAs(late.Task));
            own.Dispose(); late.SetException(new InvalidOperationException("LATE_OBSERVED"));
            yield return null; Assert.That(own.StartupPhase, Is.EqualTo(SavedStartupPhase.Disposed));
            tree.RemoveFromHierarchy(); cancel.Dispose();
#endif
            installer.Presenter.ShowRequestFailure(false);
            Assert.That(Find<Button>("saved-reload").enabledSelf, Is.True);
            Submit("saved-reload"); yield return Idle();
            Assert.That(installer.Presenter.HasRequestFault, Is.False);
            Assert.That(installer.Model.PuzzleView, Is.Null);
        }

        [UnityTest]
        public IEnumerator InstallerOnlyDestroyAndDisabledDocument_ReleaseResourcesAndPendingWork()
        {
            yield return CreateAndOpen();
            var document = installer.Document; var camera = installer.ScreenCamera; var kept = installer.gameObject;
            document.enabled = false; yield return null;
            Assert.That(document.rootVisualElement, Is.Null);
            UnityEngine.Object.Destroy(installer); yield return null;
            yield return Await(installer.ShutdownTask);
            Assert.That(kept != null, Is.True); Assert.That(document == null, Is.True); Assert.That(camera == null, Is.True);
        }

        [UnityTest]
        public IEnumerator ResultsRestartAndSolve_UsesNewAttempt_NewRide_AndNeverPaysTwice()
        {
            yield return CreateAndOpen(); SolveFirst(); yield return Results();
            var old = installer.Model.PuzzleView!; var attempt = old.AttemptId;
            Submit("saved-restart"); yield return Idle();
            Assert.That(installer.Model.PuzzleView!.AttemptId, Is.Not.EqualTo(attempt));
            Assert.That(installer.Model.PuzzleView.TimerStarted, Is.False);
            SolveFirst(); yield return Results();
            Assert.That(old.CompleteTrainRide(attempt), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(installer.Model.State.Progress!.BalancePatience, Is.EqualTo(15));
        }

        [UnityTest]
        public IEnumerator SavedFrame_586x543Viewport_ScrollReachesEveryVisibleControl_LargeCountTextFits()
        {
            yield return CreateAndOpen();
            Find<DropdownField>("saved-picker").index = LargeIndex; Submit("saved-open"); yield return Idle();
            var scroll = Find<ScrollView>("saved-scroll");
            var oldWidth = scroll.style.width; var oldHeight = scroll.style.height; var oldGrow = scroll.style.flexGrow;
            scroll.style.width = 586; scroll.style.height = 543; scroll.style.flexGrow = 0;
            try
            {
                for (int i = 0; i < 3; i++) yield return null;
                TestContext.WriteLine("SAVED_LAYOUT_NOMINAL requested=586x543 actualLayout=" + scroll.layout + " viewport=" + scroll.contentViewport.worldBound);
                // Panel scaling quantizes CSS lengths to physical pixels; allow at most one logical pixel only here.
                Assert.That(scroll.layout.width, Is.EqualTo(586).Within(1f));
                Assert.That(scroll.layout.height, Is.EqualTo(543).Within(1f));
                Assert.That(scroll.contentContainer.worldBound.height, Is.GreaterThan(scroll.contentViewport.worldBound.height));
                Assert.That(scroll.verticalScroller.highValue, Is.GreaterThan(0));
                Assert.That(scroll.mode, Is.EqualTo(ScrollViewMode.Vertical));
                Assert.That(scroll.horizontalScrollerVisibility, Is.EqualTo(ScrollerVisibility.Hidden));
                Assert.That(scroll.contentContainer.worldBound.width, Is.LessThanOrEqualTo(scroll.contentViewport.worldBound.width + .1f));
                AssertLargeCountGeometry();
                var controls = scroll.contentContainer.Query<Button>().ToList().Where(button => VisibleWithinScroll(button, scroll)).ToArray();
                Assert.That(controls.Length, Is.GreaterThan(20));
                foreach (var control in controls)
                {
                    scroll.ScrollTo(control); yield return null; yield return null;
                    var viewport = scroll.contentViewport.worldBound; var bounds = control.worldBound;
                    TestContext.WriteLine("SAVED_CONTROL_GEOMETRY name=" + control.name + " bounds=" + bounds + " viewport=" + viewport + " scrollOffset=" + scroll.scrollOffset + " controlLayout=" + control.layout + " contentLayout=" + scroll.contentContainer.layout);
                    Assert.That(viewport.Contains(bounds.min + new Vector2(.2f, .2f)), Is.True, control.name + " top-left");
                    Assert.That(viewport.Contains(bounds.max - new Vector2(.2f, .2f)), Is.True, control.name + " bottom-right");
                }
                scroll.ScrollTo(installer.Presenter.Core!.Grid); yield return null; yield return null;
                AssertReadableGrid();
                TestContext.WriteLine("SAVED_LAYOUT_VIEWPORT " + scroll.contentViewport.worldBound + " GRID " + installer.Presenter.Core.Grid.Board.worldBound);
            }
            finally { scroll.style.width = oldWidth; scroll.style.height = oldHeight; scroll.style.flexGrow = oldGrow; scroll.scrollOffset = Vector2.zero; }
        }
        private static bool VisibleWithinScroll(VisualElement target, ScrollView scroll)
        {
            for (VisualElement? current = target; current != null && current != scroll; current = current.parent)
                if (current.resolvedStyle.display == DisplayStyle.None || current.resolvedStyle.visibility == Visibility.Hidden) return false;
            return true;
        }

        [UnityTest]
        public IEnumerator NormalScreen_ActualSaved4x4_10x10_Ride_Results()
        {
            yield return CreateAndOpen();
            Submit("qa-tool-empty"); Submit("qa-cell-1-1"); Submit("qa-tool-occupied"); Submit("qa-cell-1-2");
            Submit("qa-multiselect"); Submit("qa-cell-2-2");
            for (int i = 0; i < 3; i++) yield return null;
            yield return ScrollActualTo(installer.Presenter.Core!.Grid);
            AssertReadableGrid(); yield return CaptureScreen("saved-public240-4x4.png");
            Find<DropdownField>("saved-picker").index = LargeIndex; Submit("saved-open"); yield return Idle();
            Submit("qa-tool-empty"); Submit("qa-cell-1-1"); Submit("qa-tool-occupied"); Submit("qa-cell-1-2");
            Submit("qa-multiselect"); Submit("qa-cell-2-2");
            for (int i = 0; i < 3; i++) yield return null;
            yield return ScrollActualTo(installer.Presenter.Core!.Grid);
            AssertReadableGrid(); yield return CaptureScreen("saved-public240-10x10.png");
            Find<DropdownField>("saved-picker").index = 0; Submit("saved-open"); yield return Idle(); SolveFirst();
            double deadline = Time.realtimeSinceStartupAsDouble + 15;
            while (installer.Ride == null && Time.realtimeSinceStartupAsDouble < deadline) yield return null;
            Assert.That(installer.Ride, Is.Not.Null);
            for (int i = 0; i < 3; i++) yield return null;
            installer.SetApplicationPaused(true); yield return Idle();
            Assert.That(installer.Model.State.Status, Is.EqualTo(SavedReviewStatus.TrainRide));
            Assert.That(installer.Model.State.Progress!.BalancePatience, Is.EqualTo(15));
            yield return ScrollActualTo(installer.Presenter.Core!.Grid);
            AssertReadableGrid(); yield return CaptureScreen("saved-public240-ride.png");
            installer.SetApplicationPaused(false); yield return Results();
            for (int i = 0; i < 3; i++) yield return null;
            yield return ScrollActualTo(Find<VisualElement>("qa-result"));
            yield return CaptureScreen("saved-public240-results.png");
        }

#if UNITY_EDITOR
        [Test]
        public void Builder_ReproducesOwnSceneTwice_LeavesAllExistingAAndSAssetsUnchanged()
        {
            string[] own = { ScenePath, ScenePath + ".meta" };
            string[] protectedPaths = { "Assets/StammstreckenPuzzle/Scenes/QA/Public240Review.unity", "Assets/StammstreckenPuzzle/Scenes/QA/DevelopmentPuzzle.unity",
                "Assets/StammstreckenPuzzle/UI/Public240ReviewPanelSettings.asset", "Assets/StammstreckenPuzzle/UI/Public240.public.json",
                "Assets/StammstreckenPuzzle/UI/ReviewCampaign.catalog.json", "Assets/StammstreckenPuzzle/UI/ReviewCompletion.catalog.json", "Assets/StammstreckenPuzzle/Art/Development/DevelopmentTrain.mat" };
            var before = protectedPaths.Select(File.ReadAllBytes).ToArray();
            InvokeBuilder("EnsureArtifacts"); var first = own.Select(File.ReadAllBytes).ToArray(); InvokeBuilder("EnsureArtifacts");
            AssetDatabase.ImportAsset(ScenePath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            InvokeBuilder("VerifyArtifacts");
            for (int i = 0; i < own.Length; i++) Assert.That(File.ReadAllBytes(own[i]), Is.EqualTo(first[i]), own[i]);
            for (int i = 0; i < protectedPaths.Length; i++) Assert.That(File.ReadAllBytes(protectedPaths[i]), Is.EqualTo(before[i]), protectedPaths[i]);
            var dependencies = AssetDatabase.GetDependencies(ScenePath, true);
            Assert.That(dependencies.All(path => !path.Contains("/Tests/") && !path.Contains("Content/Levels") && !path.Contains("Content/Proofs") && !path.Contains("A_SOURCE_LOCK")), Is.True);
        }
#endif

        private string MainPath => Path.Combine(installer.Composition.Files.ProfileDirectory, "save.json");
        private int LargeIndex => installer.Model.Puzzles.ToList().FindIndex(p => p.Width == 10 && p.Height == 10);
        private LocalSaveCodec Codec()
        {
            var c = installer.Composition;
            return new LocalSaveCodec(new CanonicalJsonAdapter(), c.Profile, c.ProgressCatalog.CatalogSet, c.Catalog, c.ProgressCatalog.Campaign, c.ProgressCatalog.Completion);
        }
        private byte[] AlternativeBytes(SaveSnapshot<LocalProfileState> snapshot)
        {
            var p = snapshot.Payload;
            var alternative = LocalProfileState.Create(p.Profile, p.Levels, p.Endless, p.LedgerCheckpoint, p.EconomyJournal, p.Inventory,
                p.CosmeticClaimReservations, p.SeasonProgressCache, p.Drafts, p.RewardClaims, p.DailyStatus, p.Entitlements, p.PendingOperations,
                LocalSettings.Create(p.Settings.Locale, p.Settings.AudioGroups, !(p.Settings.HapticsEnabled ?? false), p.Settings.Accessibility), p.DataLifecycle, p.HintEntitlement);
            byte[] encoded = Codec().Encode(new SaveSnapshot<LocalProfileState>(snapshot.Profile, snapshot.Catalog, snapshot.Generation, alternative),
                "2026-10-19T00:00:00Z", "qa-test", "qa-test");
            var decoded = Codec().Decode(encoded);
            Assert.That(decoded.Status, Is.EqualTo(LocalSaveDecodeStatus.Valid), decoded.ErrorCode);
            Assert.That(decoded.Snapshot!.Payload.Settings.HapticsEnabled, Is.EqualTo(!(p.Settings.HapticsEnabled ?? false)));
            Assert.That(decoded.Snapshot.Payload.Settings.Locale, Is.EqualTo(p.Settings.Locale));
            return encoded;
        }
        private static byte[]? PreserveFixtureOriginal(string path)
        {
            if (!File.Exists(path)) { TestContext.WriteLine("FIXTURE_ORIGINAL_MISSING " + Path.GetFileName(path)); return null; }
            byte[] original = File.ReadAllBytes(path);
            TestContext.WriteLine("FIXTURE_ORIGINAL_BASE64 " + Path.GetFileName(path) + " " + Convert.ToBase64String(original));
            return original;
        }
        private static byte[]? PreparePendingDirectoryObstacle(string path)
        {
            byte[]? original = PreserveFixtureOriginal(path);
            if (original != null) File.Delete(path);
            Assert.That(File.Exists(path), Is.False);
            Assert.That(Directory.Exists(path), Is.False);
            Directory.CreateDirectory(path);
            Assert.That(Directory.Exists(path), Is.True);
            return original;
        }
        private IEnumerator ScrollActualTo(VisualElement target)
        {
            var scroll = Find<ScrollView>("saved-scroll"); scroll.ScrollTo(target);
            for (int i = 0; i < 3; i++) yield return null;
        }

        private IEnumerator CreateAndOpen()
        {
            Submit("saved-create"); yield return Idle(); Submit("saved-open"); yield return Idle();
            Assert.That(installer.Model.PuzzleView!.Puzzle.Identity.PuzzleId, Is.EqualTo("S1-01-01-01"));
        }
        private IEnumerator Idle()
        {
            double deadline = Time.realtimeSinceStartupAsDouble + 20; int stable = 0;
            while (stable < 3 && Time.realtimeSinceStartupAsDouble < deadline)
            {
                yield return null; installer.Presenter.Refresh();
                stable = installer.PendingOperation.IsCompleted && installer.Presenter.PendingRequest == null ? stable + 1 : 0;
            }
            Assert.That(stable, Is.EqualTo(3), "Actual request did not settle while Main continued pumping.");
            yield return Await(installer.PendingOperation);
        }
        private static IEnumerator Await(Task task)
        {
            double deadline = Time.realtimeSinceStartupAsDouble + 20;
            while (!task.IsCompleted && Time.realtimeSinceStartupAsDouble < deadline) yield return null;
            Assert.That(task.IsCompleted, Is.True, "Observed task did not terminate.");
            if (task.IsFaulted) throw task.Exception!;
            Assert.That(task.IsCanceled, Is.False, "Unexpected cancellation in the successful flow.");
        }
        private IEnumerator Results()
        {
            double deadline = Time.realtimeSinceStartupAsDouble + 20;
            while (installer.Model.State.Status != SavedReviewStatus.Results && Time.realtimeSinceStartupAsDouble < deadline) yield return null;
            Assert.That(installer.Model.State.Status, Is.EqualTo(SavedReviewStatus.Results)); yield return Idle();
            Assert.That(installer.Model.State.Progress!.SelectedStars, Is.EqualTo(1));
            Assert.That(Find<Label>("qa-result-boundary").text, Does.Contain("QA-Profil gespeichert"));
        }
        private void SolveFirst()
        {
            Assert.That(installer.Model.PuzzleView!.Puzzle.Identity.HashSha256, Is.EqualTo("5f7aa091e1fe38d06673878eb1b2f58d4731dea895c855fbbf9f080b1222307d"));
            foreach (var step in ReferenceRoute) { Submit("qa-tool-" + step.Tool); Submit(Public240ReviewGrid.CellName(step.X, step.Y)); }
        }
        // Existing bound A public reference route, tests only; never a runtime solution source.
        private static readonly (int X, int Y, string Tool)[] ReferenceRoute = {
            (3,2,"ne"),(3,1,"ns"),(3,0,"sw"),(2,0,"ew"),(1,0,"es"),(1,1,"wn"),(0,1,"ne"),(0,0,"ns") };
        private T Find<T>(string name) where T : VisualElement => installer.Document.rootVisualElement.Q<T>(name) ?? throw new InvalidOperationException(name);
        private void Submit(string id) { var button = Find<Button>(id); Assert.That(button.enabledInHierarchy, Is.True, id); SubmitElement(button); installer.Presenter.Refresh(); }
        private static void SubmitElement(Button button) { using var submit = NavigationSubmitEvent.GetPooled(); submit.target = button; button.SendEvent(submit); }
        private static void PointerClick(Button button)
        {
            var position = button.worldBound.center; Assert.That(button.panel.Pick(position), Is.SameAs(button));
            using var down = PointerDownEvent.GetPooled(new Event { type = EventType.MouseDown, button = 0, mousePosition = position }); down.target = button; button.SendEvent(down);
            using var up = PointerUpEvent.GetPooled(new Event { type = EventType.MouseUp, button = 0, mousePosition = position }); up.target = button; button.SendEvent(up);
        }
        private static object Property(object value, string name) => value.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(value)!;
        private static Dictionary<string, string> CaptureSessionFacts(object session)
        {
            var result = new Dictionary<string, string>();
            foreach (var name in new[] { "Cells", "UsedEmptyMarker", "UsedOccupiedMarker", "CorrectionCount", "Revision", "AppliedCommandIds", "PreviouslySet" })
            { object value = Property(session, name); result[name] = value is IEnumerable list && !(value is string) ? string.Join("|", list.Cast<object>().Select(x => x.ToString())) : value.ToString()!; }
            result["UndoStack"] = string.Join(";", ((IEnumerable)Property(session, "UndoStack")).Cast<object>().Select(diff =>
                string.Join("|", ((IEnumerable)Property(diff, "Changes")).Cast<object>().Select(change =>
                    Property(Property(change, "Coordinate"), "X") + "," + Property(Property(change, "Coordinate"), "Y") + ":" + Property(change, "Before") + ">" + Property(change, "After")))));
            return result;
        }
        private Dictionary<string, byte[]> TreeBytes() => Directory.GetFiles(root, "*", SearchOption.AllDirectories).ToDictionary(path => path.Substring(root.Length), File.ReadAllBytes);
        private static void AssertTreeEqual(Dictionary<string, byte[]> first, Dictionary<string, byte[]> second)
        { Assert.That(second.Keys, Is.EquivalentTo(first.Keys)); foreach (var key in first.Keys) Assert.That(second[key], Is.EqualTo(first[key]), key); }
        private void AssertReadableGrid()
        {
            var grid = installer.Presenter.Core!.Grid; var board = grid.Board.worldBound;
            var documentRoot = installer.Document.rootVisualElement;
            var scroll = Find<ScrollView>("saved-scroll");
            TestContext.WriteLine("SAVED_GRID_BOUNDS frame=" + Time.frameCount + " graphics=" + SystemInfo.graphicsDeviceType + " batch=" + UnityEngine.Application.isBatchMode + " screenPixels=" + Screen.width + "x" + Screen.height
                + " boardWorld=" + board + " gridLayout=" + grid.layout + " gridWorld=" + grid.worldBound
                + " documentRootLayout=" + documentRoot.layout + " documentRootWorld=" + documentRoot.worldBound
                + " panelVisualTreeLayout=" + documentRoot.panel.visualTree.layout + " panelVisualTreeWorld=" + documentRoot.panel.visualTree.worldBound
                + " scrollLayout=" + scroll.layout + " scrollWorld=" + scroll.worldBound + " scrollOffset=" + scroll.scrollOffset
                + " contentViewportLayout=" + scroll.contentViewport.layout + " contentViewportWorld=" + scroll.contentViewport.worldBound
                + " contentContainerLayout=" + scroll.contentContainer.layout + " contentContainerWorld=" + scroll.contentContainer.worldBound);
            Assert.That(board.width, Is.GreaterThan(100)); Assert.That(board.height, Is.GreaterThan(100));
            var screen = installer.Document.rootVisualElement.panel.visualTree.worldBound;
            Assert.That(screen.Contains(board.min), Is.True); Assert.That(screen.Contains(board.max - new Vector2(.1f, .1f)), Is.True);
            for (int y = 0; y < grid.GridHeight; y++) for (int x = 0; x < grid.GridWidth; x++) Assert.That(grid.CellButton(x, y).worldBound.width, Is.GreaterThan(20));
            AssertLargeCountGeometry();
        }
        private void AssertLargeCountGeometry()
        {
            var grid = installer.Presenter.Core!.Grid;
            for (int x = 0; x < grid.GridWidth; x++)
            {
                var label = Find<Label>("qa-column-count-" + x);
                float textWidth = label.MeasureTextSize("10/10", 0, VisualElement.MeasureMode.Undefined, 0, VisualElement.MeasureMode.Undefined).x;
                TestContext.WriteLine("SAVED_COLUMN_SLOT " + x + " bounds=" + label.worldBound + " longest10/10=" + textWidth);
                Assert.That(label.worldBound.width, Is.GreaterThanOrEqualTo(textWidth), label.name);
                if (x + 1 < grid.GridWidth) Assert.That(label.worldBound.xMax, Is.LessThanOrEqualTo(Find<Label>("qa-column-count-" + (x + 1)).worldBound.xMin + .1f));
            }
        }
        private static void InvokeBuilder(string method)
        {
            var type = Type.GetType("STP.Editor.Build.SavedPublic240ReviewBuild, STP.Editor.Build", true)!;
            try { type.GetMethod(method, BindingFlags.Public | BindingFlags.Static)!.Invoke(null, null); }
            catch (TargetInvocationException error) when (error.InnerException != null) { throw error.InnerException; }
        }

        private sealed class LockOwner : IDisposable
        {
            private readonly SaveFileSystem files;
            private readonly ManualResetEventSlim ready = new ManualResetEventSlim(false);
            private readonly ManualResetEventSlim release = new ManualResetEventSlim(false);
            private readonly ManualResetEventSlim done = new ManualResetEventSlim(false);
            private Thread? thread;
            internal Exception? Error;
            internal volatile bool Held;
            internal bool Ready => ready.IsSet;
            internal bool Done => done.IsSet;
            internal LockOwner(SaveFileSystem files) { this.files = files; }
            internal void Start()
            {
                thread = new Thread(() =>
                {
                    try
                    {
                        using (files.AcquireExclusive(CancellationToken.None))
                        {
                            Held = true; ready.Set();
                            if (!release.Wait(25000)) throw new TimeoutException("TEST_OWNER_RELEASE_NOT_SIGNALED");
                        }
                    }
                    catch (Exception error) { Error = error; }
                    finally { Held = false; ready.Set(); done.Set(); }
                }); thread.IsBackground = true; thread.Start();
            }
            internal void Release() => release.Set();
            public void Dispose()
            {
                Release();
                if (thread != null && !thread.Join(2000)) throw new InvalidOperationException("TEST_LOCK_OWNER_DID_NOT_TERMINATE");
                ready.Dispose(); release.Dispose(); done.Dispose();
            }
        }
        private static IEnumerator WaitReady(LockOwner owner)
        {
            double deadline = Time.realtimeSinceStartupAsDouble + 10;
            while (!owner.Ready && Time.realtimeSinceStartupAsDouble < deadline) yield return null;
            Assert.That(owner.Ready, Is.True); if (owner.Error != null) throw owner.Error; Assert.That(owner.Held, Is.True);
        }
        private static IEnumerator WaitOwner(LockOwner owner)
        {
            double deadline = Time.realtimeSinceStartupAsDouble + 10;
            while (!owner.Done && Time.realtimeSinceStartupAsDouble < deadline) yield return null;
            Assert.That(owner.Done, Is.True); if (owner.Error != null) throw owner.Error;
        }

        private IEnumerator CaptureScreen(string filename)
        {
            Assert.That(panel!.targetTexture,Is.Null);Assert.That(installer.ScreenCamera.targetTexture,Is.Null);
            if(SystemInfo.graphicsDeviceType==GraphicsDeviceType.Null || UnityEngine.Application.isBatchMode)
            {
                Assert.That(RequireGraphics,Is.False,"Native required means nonbatch real normal screen, never a headless graphics pass.");
                Debug.Log("SavedPublic240 SCREEN GRAPHICS NOT_EXECUTED: headless/batch; only UI semantics executed: "+filename);yield break;
            }
            int requested=Time.frameCount,rendered=-1;Texture2D? image=null;Exception? failure=null;
            Action<ScriptableRenderContext,List<Camera>> observed=(context,cameras)=>{if(cameras.Contains(installer.ScreenCamera))rendered=Time.frameCount;};
            IEnumerator AtEndOfFrame()
            {
                while(image==null && failure==null)
                {
                    yield return new WaitForEndOfFrame();
                    if(Time.frameCount<=requested || rendered!=Time.frameCount)continue;
                    try {image=ScreenCapture.CaptureScreenshotAsTexture(1);if(image==null)throw new InvalidOperationException("No actual screen image");}
                    catch(Exception error){failure=error;}
                }
            }
            RenderPipelineManager.endContextRendering+=observed;
            var routine=installer.StartCoroutine(AtEndOfFrame());
            try
            {
                double deadline=Time.realtimeSinceStartupAsDouble+8;
                while(image==null && failure==null && Time.realtimeSinceStartupAsDouble<deadline)yield return null;
                if(failure!=null)throw failure;
                Assert.That(image,Is.Not.Null,"No completed normal Game view frame");
                Assert.That(image!.width,Is.EqualTo(Screen.width));Assert.That(image.height,Is.EqualTo(Screen.height));
                var pixels=image.GetPixels32();var first=pixels[0];
                Assert.That(pixels.Count(pixel=>Math.Abs(pixel.r-first.r)+Math.Abs(pixel.g-first.g)+Math.Abs(pixel.b-first.b)>30),Is.GreaterThan(1500),"Actual screen must contain UI, not a flat clear buffer");
                string folder=Environment.GetEnvironmentVariable("STP_GRAPHICS_OUTPUT")??"TestResults/SavedPublic240Graphics";
                Directory.CreateDirectory(folder);File.WriteAllBytes(Path.Combine(folder,filename),image.EncodeToPNG());
                Debug.Log("SavedPublic240 actual normal ScreenCapture "+filename+" frame="+Time.frameCount+" size="+image.width+"x"+image.height);
            }
            finally
            {
                installer.StopCoroutine(routine);RenderPipelineManager.endContextRendering-=observed;
                if(image!=null)UnityEngine.Object.Destroy(image);
            }
        }
    }
}
