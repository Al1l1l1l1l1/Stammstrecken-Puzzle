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
    /// <summary>Real checked-in scene and real UI input. Reference route below is tests-only and never read by runtime.</summary>
    public sealed class Public240ReviewSmokeTests
    {
        private const string ScenePath = "Assets/StammstreckenPuzzle/Scenes/QA/Public240Review.unity";
        private Public240ReviewInstaller installer = null!;
        private PanelSettings ownedPanel = null!;
        private GameObject? ownedHost;
        private static bool RequireGraphics => Environment.GetEnvironmentVariable("STP_REQUIRE_GRAPHICS") == "1";

        [UnitySetUp]
        public IEnumerator ActualScene()
        {
            if (RequireGraphics) Assert.That(SystemInfo.graphicsDeviceType, Is.Not.EqualTo(GraphicsDeviceType.Null));
#if UNITY_EDITOR
            InvokeBuilder("VerifyArtifacts");
            EditorSceneManager.LoadSceneInPlayMode(ScenePath, new LoadSceneParameters(LoadSceneMode.Single));
#else
            SceneManager.LoadScene(ScenePath, LoadSceneMode.Single);
#endif
            yield return null;
            // Test-only scene inspection; production composition uses only serialized bindings.
            installer = UnityEngine.Object.FindFirstObjectByType<Public240ReviewInstaller>()!;
            Assert.That(installer, Is.Not.Null);
            ownedHost = installer.gameObject;
            ownedPanel = UnityEngine.Object.Instantiate(installer.Document.panelSettings);
            ownedPanel.targetTexture = null;
            installer.Document.panelSettings = ownedPanel;
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            {
                installer.Document.rootVisualElement.style.width = 960;
                installer.Document.rootVisualElement.style.height = 840;
            }
            for (int frame = 0; frame < 4; frame++) yield return null;
            installer.SetApplicationFocus(true); installer.SetApplicationPaused(false);
        }

        [UnityTearDown]
        public IEnumerator ReleaseOwnResources()
        {
            if (ownedHost != null) UnityEngine.Object.Destroy(ownedHost);
            yield return null;
            if (ownedPanel != null) UnityEngine.Object.Destroy(ownedPanel);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ActualPicker_All240OpenViaRealNextButtons_WithExactIdentities()
        {
            Assert.That(installer.PuzzleCount, Is.EqualTo(240));
            Assert.That(installer.Catalog.Identity.Use, Is.EqualTo(CatalogUse.IsolatedReview));
            Assert.That(installer.Catalog.Identity.ApprovalStatus, Is.EqualTo(CatalogApprovalStatus.Draft));
            Assert.Throws<NotSupportedException>(()=>((IList<PublicPuzzleInput>)installer.Catalog.Puzzles)[0]=installer.Catalog.Puzzles[0]);
            Assert.That(installer.Catalog.TryGet(null!,out _),Is.False);
            var picker = Find<DropdownField>("qa-picker");
            Assert.That(picker.choices.Count, Is.EqualTo(240));
            for (int index = 0; index < 240; index++)
            {
                var expected = installer.Catalog.Puzzles[index];
                Assert.That(installer.Model.Puzzle.Identity, Is.EqualTo(expected.Identity));
                Assert.That(installer.Model.Puzzle.Identity.HashProfile, Is.EqualTo(PublicPuzzleIdentity.SemanticHashProfile));
                Assert.That(installer.Presenter.Grid.GridWidth, Is.EqualTo(expected.Width));
                Assert.That(installer.Presenter.Grid.GridHeight, Is.EqualTo(expected.Height));
                Assert.That(Find<Label>("qa-title").text, Does.Contain(expected.Identity.PuzzleId));
                Assert.That(installer.Catalog.TryGet(expected.Identity,out var found),Is.True);
                Assert.That(found,Is.SameAs(expected));
                if (index < 239) Submit("qa-picker-next");
                if(index%24==0) yield return null;
            }
            Assert.That(Find<Button>("qa-picker-next").enabledSelf, Is.False);
            var last = installer.Catalog.Puzzles[239];
            Assert.That(installer.Catalog.TryGet(new PublicPuzzleIdentity(last.Identity.PuzzleId,last.Identity.HashProfile,new string('a',64)),out _),Is.False);
            Submit("qa-picker-previous");
            Assert.That(installer.Model.Puzzle.Identity,Is.EqualTo(installer.Catalog.Puzzles[238].Identity));
            Assert.That(installer.Model.TimerStarted,Is.False);
        }

        [UnityTest]
        public IEnumerator ActualToolsPointerSelectionBatchUndoNoOpAndNavigationRetainAttempt()
        {
            long revision = installer.Model.Revision;
            var toolIds = new[]{"clear","empty","occupied","ns","ew","ne","es","sw","wn"};
            for(int i=0;i<toolIds.Length;i++)
            {
                Submit("qa-tool-"+toolIds[i]);
                Assert.That(installer.Presenter.ActiveTool,Is.EqualTo((DevelopmentTool)i));
            }
            Assert.That(installer.Model.Revision,Is.EqualTo(revision));
            Submit("qa-tool-clear"); Submit("qa-cell-0-3");
            Assert.That(installer.Model.Revision,Is.EqualTo(revision));
            Assert.That(installer.Model.TimerStarted,Is.False);
            Submit("qa-multiselect"); Submit("qa-cell-0-3"); Submit("qa-cell-1-3");
            Assert.That(installer.Presenter.SelectionCount,Is.EqualTo(2));
            Assert.That(installer.Model.Revision,Is.EqualTo(revision));
            Submit("qa-tool-occupied"); Submit("qa-apply-selection");
            Assert.That(installer.Model.Revision,Is.EqualTo(revision+1));
            Assert.That(installer.Model.RowVisible(3),Is.EqualTo(2));
            Assert.That(Find<Label>("qa-row-count-3").ClassListContains("qa-count--exceeded"),Is.True);
            Assert.That(installer.Presenter.SelectionCount,Is.Zero);
            Submit("qa-undo");
            Assert.That(installer.Model.CellAt(0,3),Is.EqualTo(DevelopmentTool.Clear));
            Assert.That(installer.Model.CellAt(1,3),Is.EqualTo(DevelopmentTool.Clear));
            Submit("qa-multiselect"); Submit("qa-tool-empty");
            yield return null;
            PointerClick(installer.Presenter.Grid.CellButton(1,1));
            yield return null;
            Assert.That(installer.Model.CellAt(1,1),Is.EqualTo(DevelopmentTool.EmptyMarker),"Actual pointer click must reach the real Application command.");
            var attempt=installer.Model.AttemptId;
            installer.SetApplicationPaused(true); long paused=installer.Model.ElapsedAt(installer.NowMilliseconds);
            yield return null;
            Assert.That(installer.Model.ElapsedAt(installer.NowMilliseconds+10000),Is.EqualTo(paused));
            installer.SetApplicationPaused(false);
            Submit("qa-picker-next"); Submit("qa-picker-previous");
            Assert.That(installer.Model.AttemptId,Is.EqualTo(attempt));
            Assert.That(installer.Model.CellAt(1,1),Is.EqualTo(DevelopmentTool.EmptyMarker));
            Assert.That(installer.Model.CanUndo,Is.True);
        }

        [UnityTest]
        public IEnumerator ActualPublicPuzzle_PlayerCommandsProduceOwnPathThenRideThenUnsavedResult()
        {
            AssertFirstIdentity(); SolveFirstViaUi();
            Assert.That(installer.Model.Phase,Is.EqualTo(DevelopmentPreviewPhase.TrainRide));
            Assert.That(Find<VisualElement>("qa-result").resolvedStyle.display,Is.EqualTo(DisplayStyle.None));
            Assert.That(Find<Button>("qa-picker-next").enabledSelf,Is.False);
            var attempt=installer.Model.AttemptId;
            Assert.That(installer.Model.BuiltPath.Select(cell=>(cell.X,cell.Y)),Is.EqualTo(ReferenceRoute.Select(cell=>(cell.X,cell.Y))));
            double deadline=Time.realtimeSinceStartupAsDouble+8;
            while(installer.Model.Phase==DevelopmentPreviewPhase.TrainRide && Time.realtimeSinceStartupAsDouble<deadline) yield return null;
            Assert.That(installer.Model.Phase,Is.EqualTo(DevelopmentPreviewPhase.Results));
            Assert.That(Find<VisualElement>("qa-result").resolvedStyle.display,Is.EqualTo(DisplayStyle.Flex));
            Assert.That(Find<Label>("qa-result-boundary").text,Does.Contain("keinen Abschluss"));
            Submit("qa-restart");
            Assert.That(installer.Model.AttemptId,Is.Not.EqualTo(attempt));
            Assert.That(installer.Model.CompleteTrainRide(attempt),Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(installer.Model.Phase,Is.EqualTo(DevelopmentPreviewPhase.Editing));
        }

        [UnityTest]
        public IEnumerator NormalScreen_Actual4x4And10x10PickerRideAndResults()
        {
            Assert.That(ownedPanel.targetTexture,Is.Null); Assert.That(installer.ScreenCamera.targetTexture,Is.Null);
            AssertFirstIdentity();
            Submit("qa-tool-empty"); Submit("qa-cell-1-1");
            Submit("qa-tool-occupied"); Submit("qa-cell-1-2");
            Submit("qa-multiselect"); Submit("qa-cell-2-2");
            for(int frame=0;frame<3;frame++) yield return null;
            AssertReadableGrid();
            yield return CaptureScreen("public240-4x4.png");
            // Every selection changes through real navigation buttons, not direct session replacement.
            int largeIndex=installer.Catalog.Puzzles.ToList().FindIndex(input=>input.Width==10 && input.Height==10);
            Assert.That(largeIndex,Is.GreaterThan(0));
            for(int i=0;i<largeIndex;i++) Submit("qa-picker-next");
            for(int frame=0;frame<3;frame++) yield return null;
            Assert.That(installer.Model.Width,Is.EqualTo(10)); AssertReadableGrid();
            Submit("qa-tool-empty"); Submit("qa-cell-1-1");
            Submit("qa-tool-occupied"); Submit("qa-cell-1-2");
            Submit("qa-multiselect"); Submit("qa-cell-2-2");
            for(int frame=0;frame<2;frame++) yield return null;
            yield return CaptureScreen("public240-10x10.png");
            yield return CaptureScreen("public240-picker.png");
            for(int i=0;i<largeIndex;i++) Submit("qa-picker-previous");
            Submit("qa-restart"); SolveFirstViaUi();
            double rideDeadline=Time.realtimeSinceStartupAsDouble+5;
            while((installer.Ride==null || installer.Ride.VisitedCells.Count<1) && Time.realtimeSinceStartupAsDouble<rideDeadline) yield return null;
            Assert.That(installer.Ride,Is.Not.Null); Assert.That(installer.Ride!.VisitedCells.Count,Is.GreaterThan(0));
            installer.SetApplicationPaused(true);
            Assert.That(installer.Model.Phase,Is.EqualTo(DevelopmentPreviewPhase.TrainRide));
            for(int frame=0;frame<2;frame++) yield return null;
            yield return CaptureScreen("public240-ride.png");
            installer.SetApplicationPaused(false);
            double resultDeadline=Time.realtimeSinceStartupAsDouble+8;
            while(installer.Model.Phase==DevelopmentPreviewPhase.TrainRide && Time.realtimeSinceStartupAsDouble<resultDeadline) yield return null;
            Assert.That(installer.Model.Phase,Is.EqualTo(DevelopmentPreviewPhase.Results));
            for(int frame=0;frame<2;frame++) yield return null;
            yield return CaptureScreen("public240-results.png");
        }

        [Test]
        public void ExplicitProfileAndReferencesRejectProductionMissingDataAndForeignBytes()
        {
            Public240ReviewInstaller.ValidateQaProfile("qa","com.STP.StammstreckenPuzzle.qa");
            Assert.Throws<InvalidOperationException>(()=>Public240ReviewInstaller.ValidateQaProfile("production","com.STP.StammstreckenPuzzle"));
            Assert.Throws<InvalidOperationException>(()=>Public240ReviewInstaller.ValidateQaProfile("staging","com.STP.StammstreckenPuzzle.qa"));
            Assert.Throws<InvalidOperationException>(()=>Public240ReviewInstaller.ValidateReferences(null,null,null,null,null));
            Assert.Throws<ArgumentNullException>(()=>Public240ReviewInstaller.ValidatePublicData(null!));
            var foreign=new TextAsset("{}");
            try {Assert.Throws<ArgumentException>(()=>Public240ReviewInstaller.ValidatePublicData(foreign));}
            finally {UnityEngine.Object.DestroyImmediate(foreign);}
        }

        [UnityTest]
        public IEnumerator ConstructorFailure_ReleasesOwnLocalizationAndRegistersNoPickerCallbacks()
        {
            int before=OwnedLocaleObjects();
            var partialLocale=new TextAsset("{\"locale\":\"de-DE\",\"entries\":[{\"key\":\"qa.public240.test\",\"value\":\"Test\",\"argumentTypes\":[]}]}");
            try
            {
                Assert.Throws<InvalidOperationException>(()=>new Public240ReviewPresenter(new VisualElement(),installer.Model,partialLocale,()=>installer.NowMilliseconds,installer.Catalog.Puzzles,_=>{}));
                yield return null;
                Assert.That(OwnedLocaleObjects(),Is.EqualTo(before),"A rejected composition must release the Locale/SharedTableData/StringTable it allocated.");
            }
            finally {UnityEngine.Object.Destroy(partialLocale);}
        }
        private static int OwnedLocaleObjects() => Resources.FindObjectsOfTypeAll<ScriptableObject>().Count(value=>
            value.GetType().FullName=="UnityEngine.Localization.Locale" || value.GetType().FullName=="UnityEngine.Localization.Tables.SharedTableData" || value.GetType().FullName=="UnityEngine.Localization.Tables.StringTable");

        [UnityTest]
        public IEnumerator DestroyInstallerComponent_KeepsHostButReleasesDocumentLocaleTrainAndCameras()
        {
            SolveFirstViaUi();
            double deadline=Time.realtimeSinceStartupAsDouble+5;
            while(installer.Ride==null && Time.realtimeSinceStartupAsDouble<deadline)yield return null;
            var ride=installer.Ride; Assert.That(ride,Is.Not.Null);
            var document=installer.Document; var camera=installer.ScreenCamera; var host=installer.gameObject;
            int localeCount=OwnedLocaleObjects();
            UnityEngine.Object.Destroy(installer);
            yield return null; yield return null;
            Assert.That(host!=null,Is.True,"Destroying the installer must not destroy its caller-owned host.");
            Assert.That(document==null,Is.True,"The separately added UIDocument belongs to the installer.");
            Assert.That(camera==null,Is.True);
            Assert.That(ride!.IsDisposed,Is.True);
            Assert.That(OwnedLocaleObjects(),Is.EqualTo(localeCount-3));
        }

        [UnityTest]
        public IEnumerator DisabledDocumentThenDestroyedHost_ReleasesResourcesWithoutAssumingRootExists()
        {
            var document=installer.Document;var camera=installer.ScreenCamera;
            document.enabled=false;
            yield return null;
            Assert.That(document.rootVisualElement,Is.Null,"This exercises Unity's actual disabled-document ordering.");
            UnityEngine.Object.Destroy(installer.gameObject);
            yield return null; yield return null;
            Assert.That(document==null,Is.True);Assert.That(camera==null,Is.True);
        }

        [UnityTest]
        public IEnumerator ResultsThenRestartAndSolveWithoutYield_UsesFreshRideAndReachesSecondResult()
        {
            SolveFirstViaUi();
            installer.SetApplicationPaused(true);
            yield return null;
            var firstRide=installer.Ride; Assert.That(firstRide,Is.Not.Null);
            var firstAttempt=installer.Model.AttemptId;
            installer.SetApplicationPaused(false);
            double firstDeadline=Time.realtimeSinceStartupAsDouble+8;
            while(installer.Model.Phase==DevelopmentPreviewPhase.TrainRide && Time.realtimeSinceStartupAsDouble<firstDeadline)yield return null;
            Assert.That(installer.Model.Phase,Is.EqualTo(DevelopmentPreviewPhase.Results));
            // Deliberately no yield or Update between Results -> Restart -> all actual UI solution commands.
            Submit("qa-restart"); SolveFirstViaUi();
            installer.SetApplicationPaused(true);
            Assert.That(installer.Model.AttemptId,Is.Not.EqualTo(firstAttempt));
            yield return null;
            Assert.That(installer.Ride,Is.Not.Null);
            Assert.That(installer.Ride,Is.Not.SameAs(firstRide),"The old finished World ride must never retain ownership of a new attempt.");
            Assert.That(firstRide!.IsDisposed,Is.True);
            Assert.That(installer.Model.LastErrorCode,Is.Not.EqualTo("STALE_ATTEMPT"));
            Assert.That(installer.Model.Phase,Is.EqualTo(DevelopmentPreviewPhase.TrainRide));
            installer.SetApplicationPaused(false);
            double secondDeadline=Time.realtimeSinceStartupAsDouble+8;
            while(installer.Model.Phase==DevelopmentPreviewPhase.TrainRide && Time.realtimeSinceStartupAsDouble<secondDeadline)yield return null;
            Assert.That(installer.Model.Phase,Is.EqualTo(DevelopmentPreviewPhase.Results));
        }

#if UNITY_EDITOR
        [Test]
        public void OwnBuilder_TwoByteIdenticalGenerationsAndCleanReimport_AndNoAuditLockReference()
        {
            string[] paths={ScenePath,ScenePath+".meta","Assets/StammstreckenPuzzle/UI/Public240ReviewPanelSettings.asset","Assets/StammstreckenPuzzle/UI/Public240ReviewPanelSettings.asset.meta"};
            InvokeBuilder("EnsureArtifacts"); var first=paths.Select(File.ReadAllBytes).ToArray();
            InvokeBuilder("EnsureArtifacts");
            for(int i=0;i<paths.Length;i++) Assert.That(File.ReadAllBytes(paths[i]),Is.EqualTo(first[i]),paths[i]);
            foreach(string path in paths.Where(path=>!path.EndsWith(".meta",StringComparison.Ordinal))) AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport|ImportAssetOptions.ForceUpdate);
            InvokeBuilder("VerifyArtifacts");
            for(int i=0;i<paths.Length;i++) Assert.That(File.ReadAllBytes(paths[i]),Is.EqualTo(first[i]),paths[i]);
            var yaml=File.ReadAllText(ScenePath);
            Assert.That(yaml,Does.Not.Contain("A_SOURCE_LOCK")); Assert.That(yaml,Does.Not.Contain("Content/Levels"));
            Assert.That(yaml,Does.Not.Contain("solution")); Assert.That(yaml,Does.Not.Contain("proofRef"));
        }
#endif

        // Exact actual source route bound by source blob ad6da74e404d3b2b3926feb3e1f16fce7ac0681b, tests only.
        private static readonly (int X,int Y,string Tool)[] ReferenceRoute={
            (3,2,"ne"),(3,1,"ns"),(3,0,"sw"),(2,0,"ew"),(1,0,"es"),(1,1,"wn"),(0,1,"ne"),(0,0,"ns")};
        private void AssertFirstIdentity()
        {
            Assert.That(installer.Model.Puzzle.Identity.PuzzleId,Is.EqualTo("S1-01-01-01"));
            Assert.That(installer.Model.Puzzle.Identity.HashSha256,Is.EqualTo("5f7aa091e1fe38d06673878eb1b2f58d4731dea895c855fbbf9f080b1222307d"));
        }
        private void SolveFirstViaUi()
        {
            AssertFirstIdentity();
            foreach(var step in ReferenceRoute){Submit("qa-tool-"+step.Tool);Submit(Public240ReviewGrid.CellName(step.X,step.Y));}
        }
        private T Find<T>(string id) where T:VisualElement => installer.Document.rootVisualElement.Q<T>(id) ?? throw new InvalidOperationException(id);
        private void Submit(string id)
        {
            var button=Find<Button>(id); Assert.That(button.enabledInHierarchy,Is.True,id);
            using var action=NavigationSubmitEvent.GetPooled();action.target=button;button.SendEvent(action);
        }
        private static void PointerClick(Button button)
        {
            Assert.That(button.panel,Is.Not.Null);
            var position=button.worldBound.center;
            Assert.That(button.panel.Pick(position),Is.SameAs(button),"A real pointer must pick the actual visible cell.");
            using var down=PointerDownEvent.GetPooled(new Event{type=EventType.MouseDown,button=0,mousePosition=position});
            down.target=button;button.SendEvent(down);
            using var up=PointerUpEvent.GetPooled(new Event{type=EventType.MouseUp,button=0,mousePosition=position});
            up.target=button;button.SendEvent(up);
        }
        private void AssertReadableGrid()
        {
            var grid=installer.Presenter.Grid; var board=grid.Board.worldBound;
            Assert.That(board.width,Is.GreaterThan(100)); Assert.That(board.height,Is.GreaterThan(100));
            var bounds=installer.Document.rootVisualElement.panel.visualTree.worldBound;
            Assert.That(bounds.Contains(board.min),Is.True);Assert.That(bounds.Contains(board.max-new Vector2(.1f,.1f)),Is.True);
            for(int y=0;y<grid.GridHeight;y++) for(int x=0;x<grid.GridWidth;x++)
            {
                var button=grid.CellButton(x,y); Assert.That(button.worldBound.width,Is.GreaterThan(10));
                Assert.That(button.tooltip,Is.Not.Empty);
            }
        }

        private IEnumerator CaptureScreen(string filename)
        {
            Assert.That(ownedPanel.targetTexture,Is.Null);Assert.That(installer.ScreenCamera.targetTexture,Is.Null);
            if(SystemInfo.graphicsDeviceType==GraphicsDeviceType.Null || UnityEngine.Application.isBatchMode)
            {
                Assert.That(RequireGraphics,Is.False,"Native required means nonbatch real normal screen, never a headless graphics pass.");
                Debug.Log("Public240 SCREEN GRAPHICS NOT_EXECUTED: headless/batch; only UI semantics executed: "+filename);yield break;
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
                string folder=Environment.GetEnvironmentVariable("STP_GRAPHICS_OUTPUT")??"TestResults/Public240Graphics";
                Directory.CreateDirectory(folder);File.WriteAllBytes(Path.Combine(folder,filename),image.EncodeToPNG());
                Debug.Log("Public240 actual normal ScreenCapture "+filename+" frame="+Time.frameCount+" size="+image.width+"x"+image.height);
            }
            finally
            {
                installer.StopCoroutine(routine);RenderPipelineManager.endContextRendering-=observed;
                if(image!=null)UnityEngine.Object.Destroy(image);
            }
        }
        private static void InvokeBuilder(string method)
        {
            var type=Type.GetType("STP.Editor.Build.Public240ReviewBuild, STP.Editor.Build",true)!;
            try {type.GetMethod(method,BindingFlags.Public|BindingFlags.Static)!.Invoke(null,null);}
            catch(TargetInvocationException error) when(error.InnerException!=null){throw error.InnerException;}
        }
    }
}
