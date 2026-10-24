using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
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
    /// <summary>Actual assigned assets, physical observation store, production grid and Root-owned ride.</summary>
    public sealed class HumanCalibrationIntegrationTests
    {
        private GameObject? host;
        private HumanCalibrationInstaller installer=null!;
        private PanelSettings? panel;
        private RenderTexture? texture;
        private string ownRoot=string.Empty;
        private const string Ui="Assets/StammstreckenPuzzle/UI/";
        [UnitySetUp]
        public IEnumerator SetUp()
        {
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
            ownRoot=Path.Combine(Path.GetTempPath(),"h46-"+Guid.NewGuid().ToString("N").Substring(0,8));
            Assert.That(Directory.Exists(ownRoot),Is.False);
            host=new GameObject("Actual assigned observation integration");host.SetActive(false);
            installer=host.AddComponent<HumanCalibrationInstaller>();installer.ConfigureTestRoot(ownRoot);
            panel=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<PanelSettings>(Ui+"Public240ReviewPanelSettings.asset"));
            panel.referenceResolution=new Vector2Int(390,844);panel.match=0;
            if(SystemInfo.graphicsDeviceType!=GraphicsDeviceType.Null)
            {texture=new RenderTexture(390,844,24,RenderTextureFormat.ARGB32);Assert.That(texture.Create(),Is.True);panel.targetTexture=texture;}
            var raw=JsonUtility.FromJson<RawLock>(AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"HumanAssignments/raw-sha256-lock.json").text);
            var packs=HumanManifestRegistry.ExpectedPackPaths().Select(p=>AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"HumanAssignments/"+p)).ToArray();
            var bindings=JsonUtility.FromJson<Bindings>(AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"MobileCandidate.bindings.json").text);
            installer.Configure(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Ui+"ProductionGame.uxml"),AssetDatabase.LoadAssetAtPath<StyleSheet>(Ui+"ProductionGame.uss"),
                AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"ProductionGameLocale.json"),panel,AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"MobileCandidate.public.json"),
                bindings.publicCatalogHashSha256,packs,AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"HumanAssignments/raw-sha256-lock.json"),AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"HumanAssignments/ui-source-lock.json"),
                AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"RailwayAssets.json"),AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"ProductionCampaignText.json"));
            host.SetActive(true);installer.Document.rootVisualElement.style.width=390;installer.Document.rootVisualElement.style.height=844;
            yield return Await(installer.InitializeTask);yield return Idle();
            Assert.That(installer.StartupDiagnostic,Is.EqualTo("NONE"));Assert.That(installer.Session,Is.Null);
            Assert.That(installer.IsOperatorVisible,Is.True);Assert.That(installer.RoutingInspection!.Status,Is.EqualTo(HumanStoreStatus.Missing));
#else
            Assert.Ignore("Only the guarded Editor fixture may inject private storage.");yield break;
#endif
        }
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if(host!=null)UnityEngine.Object.Destroy(host);yield return null;
            if(!ReferenceEquals(installer,null))yield return Await(installer.ShutdownTask);
            if(texture!=null){texture.Release();UnityEngine.Object.Destroy(texture);}if(panel!=null)UnityEngine.Object.Destroy(panel);
            if(ownRoot.Length>0&&Directory.Exists(ownRoot))
            {
                string prefix=Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;
                Assert.That(Path.GetFullPath(ownRoot).StartsWith(prefix,StringComparison.OrdinalIgnoreCase),Is.True);
                Assert.That(Path.GetFileName(ownRoot),Does.StartWith("h46-"));Directory.Delete(ownRoot,true);
            }
        }
        [UnityTest]
        public IEnumerator ActualAssignedInputs_ContactHintRestart_PersistBeforeRealRide_ExportOriginalsWithoutGameProgress()
        {
            yield return SelectPack("campaign-pack-09.json");
            var session=installer.Session!;
            Assert.That(session.State.ObservationStatus,Is.EqualTo(HumanObservationStatus.NeedsExposure));Assert.That(installer.Presenter.Grid,Is.Null);
            yield return Capture("human-introduction");
            Submit("human-show");yield return Idle();
            yield return Until(()=>session.State.Trial?.Exposure==HumanExposureState.EXPOSURE_CONFIRMED);
            yield return Until(()=>installer.Document.rootVisualElement.Q<Button>("human-never").enabledInHierarchy);
            Assert.That(session.State.FirstContactRecorded,Is.False);Assert.That(session.State.CanInput,Is.False);
            Submit("human-never");Submit("human-contact-confirm");yield return Idle();
            Assert.That(session.State.CanInput,Is.True);Assert.That(session.TimerStarted,Is.False);
            var cell=installer.Presenter.Grid!.CellButton(0,0);var bounds=cell.worldBound;
            Submit("tool-1");Submit(cell.name);Assert.That(session.CellAt(0,0),Is.EqualTo(DevelopmentTool.EmptyMarker));
            Submit(cell.name);Assert.That(session.CellAt(0,0),Is.EqualTo(DevelopmentTool.Clear));yield return Idle();
            Assert.That(cell.worldBound,Is.EqualTo(bounds));Assert.That(session.TimerStarted,Is.True);
            installer.SetPaused(true);installer.SetFocused(false);installer.SetFocused(true);
            Assert.That(session.IsPaused,Is.True,"Focus cannot clear an active OS pause.");
            long frozen=session.ElapsedAt(long.MaxValue);yield return new WaitForSecondsRealtime(.05f);
            Assert.That(session.ElapsedAt(long.MaxValue),Is.EqualTo(frozen));installer.SetPaused(false);yield return Idle();
            // Read the presenter's own locale without creating another table or mutating product state.
            var hintLocale=(ProductionGameLocalization)typeof(ProductionGamePresenter)
                .GetField("locale",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)!.GetValue(installer.Presenter)!;
            var terminalHintTexts=new[]{hintLocale.Resolve("hint.no-safe-conclusion"),hintLocale.Resolve("hint.failed"),hintLocale.Resolve("hint.changed")};
            Submit("puzzle-hint");yield return Until(()=>
            {
                if(session.State.Trial!.Hints.Any(h=>h.State==HumanHintState.HINT_DISPLAY_CONFIRMED))return true;
                string text=installer.Document.rootVisualElement.Q<Label>("hint-fact").text;
                if(terminalHintTexts.Any(value=>string.Equals(value,text,StringComparison.Ordinal)))
                    Assert.Fail("HUMAN_HINT_CONFIRMATION_UNAVAILABLE: phase="+session.Phase+" label="+text
                        +" error="+session.State.ErrorCode+" attempt="+session.AttemptId+" revision="+session.Revision
                        +" hints="+session.State.Trial.Hints.Count);
                return false;
            });
            yield return null;installer.Presenter.Refresh();Assert.That(session.State.HintCredits,Is.EqualTo(2));
            yield return Until(()=>installer.Document.rootVisualElement.Q<Button>("hint-close").enabledInHierarchy);
            Submit("hint-close");yield return Idle();Submit("puzzle-back");yield return null;Submit("human-restart");yield return Idle();
            yield return Until(()=>session.State.Trial?.Exposure==HumanExposureState.EXPOSURE_CONFIRMED);
            yield return Until(()=>installer.Document.rootVisualElement.Q<Button>("human-never").enabledInHierarchy);
            Assert.That(session.State.PriorExposure,Is.True);Assert.That(session.State.HintCredits,Is.EqualTo(2));
            Submit("human-never");Submit("human-contact-confirm");yield return Idle();
            Assert.That(session.State.Trial!.EligibleFirstContact,Is.False,"Restart cannot manufacture a second first contact.");
            yield return Capture("human-puzzle");
            var view=session.PuzzleView!;var identity=view.Puzzle.Identity;var solved=PuzzleSolver.Solve(view.Puzzle.ToDomainDefinition(),new SolverLimits(1000000,10000));
            Assert.That(solved.Classification,Is.EqualTo(SolverClassification.UNIQUE));
            for(int i=0;i<solved.Path.Count;i++)
            {
                var current=solved.Path[i];int a=i==0?(int)view.EndpointA.Edge:Toward(current,solved.Path[i-1]);
                int b=i==solved.Path.Count-1?(int)view.EndpointB.Edge:Toward(current,solved.Path[i+1]);
                Submit("tool-"+(int)Shape((1<<a)|(1<<b)));Submit(ProductionPuzzleGrid.CellName(current.X,current.Y));yield return Idle();
            }
            bool sawRide=false;float deadline=Time.realtimeSinceStartup+30;
            while(session.Phase!=DevelopmentPreviewPhase.Results&&Time.realtimeSinceStartup<deadline)
            {
                if(session.Phase==DevelopmentPreviewPhase.TrainRide)
                {
                    sawRide=true;Assert.That(session.State.Trial!.Outcome,Is.EqualTo(HumanTrialOutcome.SOLVED_UNREVIEWED));
                    Assert.That(Directory.GetFiles(ownRoot,"observations.json",SearchOption.AllDirectories).Length,Is.EqualTo(1));
                }
                yield return null;
            }
            Assert.That(sawRide,Is.True);Assert.That(session.Phase,Is.EqualTo(DevelopmentPreviewPhase.Results));
            yield return Capture("human-result");Submit("result-next");yield return Idle();
            Assert.That(session.State.Assignment!.Puzzle.Identity,Is.Not.EqualTo(identity));Assert.That(installer.Presenter.Grid,Is.Null);
            Assert.That(installer.Presenter.IsAssignedInstructionOpen,Is.True);Submit("human-export");
            yield return Until(()=>Directory.Exists(Path.Combine(ownRoot,"human-exports"))&&Directory.GetFiles(Path.Combine(ownRoot,"human-exports")).Length==1);
            var exported=Directory.GetFiles(Path.Combine(ownRoot,"human-exports")).Single();
            var record=Directory.GetFiles(ownRoot,"observations.json",SearchOption.AllDirectories).Single();
            CollectionAssert.AreEqual(File.ReadAllBytes(record),File.ReadAllBytes(exported));
            Assert.That(Encoding.UTF8.GetString(File.ReadAllBytes(exported)),Does.Contain("\"origin\":\"AUTOMATED_TEST\""));
            Assert.That(Directory.GetFiles(ownRoot,"save.json",SearchOption.AllDirectories),Is.Empty);
        }
        [UnityTest]
        public IEnumerator ActualOperator_StartRequiresExplicitConfirmation_AndAllPacksRemainNeutralBeforeAck()
        {
            Assert.That(installer.Session,Is.Null);Assert.That(installer.OperatorPanel,Is.Not.Null);
            Assert.That(installer.Document.rootVisualElement.Q<ProductionPuzzleGrid>(),Is.Null);
            Assert.That(installer.Document.rootVisualElement.Q<Button>("human-operator-confirm-start").enabledInHierarchy,Is.False);
            Assert.That(Directory.Exists(ownRoot),Is.False,"Read-only discovery must not create a participant repository.");
            foreach(var path in HumanManifestRegistry.ExpectedPackPaths())
            {
                var choice=installer.Document.rootVisualElement.Q<Button>("human-operator-pack-"+path);
                Assert.That(choice,Is.Not.Null);Assert.That(choice.text,Does.Not.Contain("S1-"));Assert.That(choice.text,Does.Not.Contain("E1-"));
            }
            Submit("human-operator-pack-endless-pack-08.json");Submit("human-operator-choose");yield return null;
            Assert.That(installer.Session,Is.Null);Assert.That(Directory.Exists(ownRoot),Is.False);
            installer.SetPaused(true);yield return null;
            Assert.That(installer.Document.rootVisualElement.Q<Button>("human-operator-confirm-start").enabledInHierarchy,Is.False);
            installer.SetPaused(false);yield return null;
            Submit("human-operator-confirm-cancel");yield return null;
            Assert.That(installer.Session,Is.Null);Assert.That(Directory.Exists(ownRoot),Is.False);
            yield return SelectPack("endless-pack-08.json");
            Assert.That(installer.Session!.State.Assignment!.Purpose,Is.EqualTo(HumanAssignmentPurpose.ENDLESS_PROFILE));
            Assert.That(installer.Presenter.Grid,Is.Null,"Confirmed run creation alone does not reserve an exposure.");
            Assert.That(installer.RoutingInspection!.Catalog!.StudyId,Is.EqualTo("wp046-human-endless-08"));
            Assert.That(Directory.GetFiles(ownRoot,"observations.json",SearchOption.AllDirectories).Length,Is.EqualTo(1));
            yield return Capture("human-operator-endless-ready");
        }
        [UnityTest]
        public IEnumerator ActualCompletedHook_OpensNeutralOperator_ThenConfirmedCrossPackCreatesOneNewToken()
        {
            yield return SelectPack("campaign-pack-01.json");
            var oldSession=installer.Session!;
            string oldToken=installer.RoutingInspection!.Snapshot!.Run.SessionToken;
            for(int i=0;i<6;i++)
            {
                Submit("human-show");yield return Idle();
                yield return Until(()=>oldSession.State.Trial?.Exposure==HumanExposureState.EXPOSURE_CONFIRMED);
                yield return Until(()=>installer.Document.rootVisualElement.Q<Button>("human-never").enabledInHierarchy);
                Submit("human-never");Submit("human-contact-confirm");yield return Idle();
                Submit("puzzle-back");yield return null;Submit("human-abort");yield return null;
                Submit("human-abort-confirm");yield return Idle();Submit("human-advance");yield return Idle();
            }
            Assert.That(oldSession.State.IsFinished,Is.True);
            Submit("human-next-participant");yield return Idle();
            Assert.That(installer.IsOperatorVisible,Is.True);Assert.That(installer.Session,Is.SameAs(oldSession));
            Assert.That(installer.RoutingInspection!.Snapshot!.Run.SessionToken,Is.EqualTo(oldToken));
            yield return Capture("human-operator-next-participant");
            Submit("human-operator-pack-endless-pack-01.json");Submit("human-operator-choose");yield return null;
            Assert.That(installer.RoutingInspection.Snapshot.Run.SessionToken,Is.EqualTo(oldToken),"Dialog opening grants no participant write.");
            Submit("human-operator-confirm-start");yield return Idle();
            Assert.That(installer.IsOperatorVisible,Is.False);Assert.That(installer.Session,Is.Not.SameAs(oldSession));
            Assert.That(installer.RoutingInspection!.Snapshot!.Run.SessionToken,Is.Not.EqualTo(oldToken));
            Assert.That(installer.RoutingInspection.Catalog!.StudyId,Is.EqualTo("wp046-human-endless-01"));
            Assert.That(installer.Session!.State.ObservationStatus,Is.EqualTo(HumanObservationStatus.NeedsExposure));
            Assert.That(installer.Presenter.Grid,Is.Null);
            string activeIndex=Directory.GetFiles(ownRoot,"active-run.json",SearchOption.AllDirectories).Single();
            byte[] indexBefore=File.ReadAllBytes(activeIndex);
            var stale=oldSession.BeginNextParticipantAsync(System.Threading.CancellationToken.None);
            yield return Until(()=>stale.IsCompleted);
            Assert.That(stale.IsFaulted,Is.True);Assert.That(stale.Exception!.GetBaseException(),Is.TypeOf<ObjectDisposedException>());
            CollectionAssert.AreEqual(indexBefore,File.ReadAllBytes(activeIndex),"A late old-session callback must not write the new participant index.");
            Assert.That(Directory.GetFiles(ownRoot,"save.json",SearchOption.AllDirectories),Is.Empty);
        }
        private IEnumerator SelectPack(string path)
        {
            Assert.That(installer.IsOperatorVisible,Is.True);
            Submit("human-operator-pack-"+path);Submit("human-operator-choose");yield return null;
            Assert.That(installer.OperatorPanel!.IsConfirmationOpen,Is.True);
            Submit("human-operator-confirm-start");yield return Idle();
            Assert.That(installer.IsOperatorVisible,Is.False);Assert.That(installer.Session,Is.Not.Null);
        }
        private IEnumerator Idle()
        {
            yield return null;yield return Until(()=>installer.PendingOperation.IsCompleted&&(installer.IsOperatorVisible||installer.Presenter.PendingRequest==null));
            if(!installer.IsOperatorVisible)Assert.That(installer.Presenter.HasRequestFault,Is.False);
            Assert.That(installer.OperatorPanel?.HasRequestFault??false,Is.False);Assert.That(installer.PendingOperation.IsFaulted,Is.False);
        }
        private static IEnumerator Await(Task task)
        {yield return Until(()=>task.IsCompleted);Assert.That(task.IsFaulted,Is.False,task.Exception?.GetBaseException().ToString());Assert.That(task.IsCanceled,Is.False);}
        private static IEnumerator Until(Func<bool> condition)
        {float deadline=Time.realtimeSinceStartup+30;while(!condition()&&Time.realtimeSinceStartup<deadline)yield return null;Assert.That(condition(),Is.True);}
        private void Submit(string name)
        {var button=installer.Document.rootVisualElement.Q<Button>(name);Assert.That(button,Is.Not.Null,name);Assert.That(button.enabledInHierarchy,Is.True,name);using var e=NavigationSubmitEvent.GetPooled();e.target=button;button.SendEvent(e);}
        private static int Toward(CellCoordinate from,CellCoordinate to)=>to.X>from.X?1:to.X<from.X?3:to.Y>from.Y?2:0;
        private static DevelopmentTool Shape(int ports)=>ports switch{5=>DevelopmentTool.TrackNS,10=>DevelopmentTool.TrackEW,3=>DevelopmentTool.TrackNE,6=>DevelopmentTool.TrackES,12=>DevelopmentTool.TrackSW,9=>DevelopmentTool.TrackWN,_=>throw new InvalidOperationException("Invalid path ports")};
        private IEnumerator Capture(string name)
        {
            if(texture==null)yield break;yield return null;yield return null;
            var prior=RenderTexture.active;var image=new Texture2D(texture.width,texture.height,TextureFormat.RGBA32,false);
            try{RenderTexture.active=texture;image.ReadPixels(new Rect(0,0,texture.width,texture.height),0,0);image.Apply();
                string output=Path.Combine(Directory.GetCurrentDirectory(),".utmp","wp046-runtime",name+".png");File.WriteAllBytes(output,image.EncodeToPNG());}
            finally{RenderTexture.active=prior;UnityEngine.Object.Destroy(image);}
        }
        [Serializable]private sealed class RawLock{public string defaultManifest=string.Empty;}
        [Serializable]private sealed class Bindings{public string publicCatalogHashSha256=string.Empty;}
    }
}
