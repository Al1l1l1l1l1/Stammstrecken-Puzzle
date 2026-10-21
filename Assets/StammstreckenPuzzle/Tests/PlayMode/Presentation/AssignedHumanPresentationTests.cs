using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Presentation.UI;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace STP.Tests.Presentation.PlayMode
{
    /// <summary>Assigned UI contracts use the actual Application adapter, controlled observation acknowledgements and the product tree.</summary>
    public sealed class AssignedHumanPresentationTests
    {
#if UNITY_EDITOR
        private const string Ui="Assets/StammstreckenPuzzle/UI/";
        private static TextAsset Locale()=>AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"ProductionGameLocale.json");
        private static VisualElement Frame(){var frame=AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Ui+"ProductionGame.uxml").CloneTree();frame.style.width=390;frame.style.height=844;return frame;}

        [UnityTest]
        public IEnumerator ExposureGate_RequiresActualVisibleGeometryAndExternalPermission_AndAcknowledgesOnce()
        {
            using var phone=new Phone();var screen=new VisualElement();screen.style.width=390;screen.style.height=844;phone.Root.Add(screen);
            var board=new VisualElement();board.style.width=240;board.style.height=240;screen.Add(board);var allowed=false;var calls=0;
            using var gate=new ProductionExposureGate(screen,board,()=>allowed);gate.Arm(()=>calls++);
            yield return new WaitForSecondsRealtime(.08f);Assert.That(calls,Is.Zero,"Geometry alone does not grant exposure while the external presentation fence is closed.");
            screen.style.display=DisplayStyle.None;allowed=true;yield return new WaitForSecondsRealtime(.05f);Assert.That(calls,Is.Zero);
            screen.style.display=DisplayStyle.Flex;yield return new WaitForSecondsRealtime(.08f);Assert.That(calls,Is.EqualTo(1));
            board.style.width=220;yield return new WaitForSecondsRealtime(.05f);Assert.That(calls,Is.EqualTo(1));
            gate.Arm(()=>calls++);gate.Cancel();yield return new WaitForSecondsRealtime(.05f);Assert.That(calls,Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator HintGate_TransparentOutsideClippedAndZeroGeometryCannotAcknowledge()
        {
            using var phone=new Phone();var overlay=new VisualElement();overlay.style.width=300;overlay.style.height=200;phone.Root.Add(overlay);
            var clip=new VisualElement();clip.style.width=240;clip.style.height=120;clip.style.overflow=Overflow.Hidden;overlay.Add(clip);
            var fact=new Label("Ein einzelner vorbereiteter Schluss.");fact.style.position=Position.Absolute;fact.style.left=0;fact.style.top=0;fact.style.width=200;fact.style.height=40;clip.Add(fact);
            using var gate=new HintDisplayGate(overlay,fact);var calls=0;fact.style.opacity=0;gate.Arm(()=>calls++);yield return new WaitForSecondsRealtime(.06f);Assert.That(calls,Is.Zero);
            fact.style.opacity=1;overlay.style.opacity=0;gate.Recheck();yield return new WaitForSecondsRealtime(.05f);Assert.That(calls,Is.Zero,"Transparent parents cannot confirm a visible fact.");
            overlay.style.opacity=1;fact.style.left=400;gate.Recheck();yield return new WaitForSecondsRealtime(.05f);Assert.That(calls,Is.Zero,"A fact outside the actual panel cannot confirm display.");
            fact.style.left=250;gate.Recheck();yield return new WaitForSecondsRealtime(.05f);Assert.That(calls,Is.Zero,"A fact within the panel but entirely outside its clip parent cannot confirm display.");
            fact.style.paddingLeft=12;fact.style.paddingRight=12;fact.style.paddingTop=8;fact.style.paddingBottom=8;
            fact.style.left=235;gate.Recheck();yield return new WaitForSecondsRealtime(.05f);
            Assert.That(fact.worldBound.Overlaps(clip.worldBound),Is.True,MessageGeometry(fact));Assert.That(fact.LocalToWorld(fact.contentRect).Overlaps(clip.worldBound),Is.False,MessageGeometry(fact));
            Assert.That(calls,Is.Zero,"Visible padding cannot acknowledge a text content box that is completely clipped.");
            fact.style.left=0;fact.style.width=0;fact.style.height=0;gate.Recheck();yield return new WaitForSecondsRealtime(.05f);
            Assert.That(fact.worldBound.width,Is.GreaterThan(0),MessageGeometry(fact));Assert.That(fact.worldBound.height,Is.GreaterThan(0),MessageGeometry(fact));
            Assert.That(fact.contentRect.width,Is.LessThanOrEqualTo(0),MessageGeometry(fact));Assert.That(fact.contentRect.height,Is.LessThanOrEqualTo(0),MessageGeometry(fact));
            Assert.That(calls,Is.Zero,"A positive padding/glyph world box cannot acknowledge zero text content area.");
            fact.style.height=40;gate.Recheck();yield return new WaitForSecondsRealtime(.05f);Assert.That(calls,Is.Zero,"Zero content width cannot acknowledge display. "+MessageGeometry(fact));
            fact.style.width=200;fact.style.height=0;gate.Recheck();yield return new WaitForSecondsRealtime(.05f);Assert.That(calls,Is.Zero,"Zero content height cannot acknowledge display. "+MessageGeometry(fact));
            fact.style.width=200;fact.style.height=40;gate.Recheck();yield return new WaitForSecondsRealtime(.06f);Assert.That(calls,Is.EqualTo(1));
            gate.Recheck();yield return new WaitForSecondsRealtime(.04f);Assert.That(calls,Is.EqualTo(1));
        }

        private static string MessageGeometry(Label message)=>"layout="+message.layout+"; contentRect="+message.contentRect+"; worldBound="+message.worldBound+"; worldContent="+message.LocalToWorld(message.contentRect);

        [UnityTest]
        public IEnumerator AssignedReview_DurableExposureThenVisibleReceiptThenContact_UsesSameCellsToolsUndoAndExternalPause()
        {
            using var test=new HumanFixture();using var phone=new Phone();var frame=Frame();phone.Root.Add(frame);
            using var presenter=new ProductionGamePresenter(frame,Locale(),()=>test.Now);
            test.Bind(presenter);Assert.That(presenter.HasAssignedReview,Is.True);Assert.That(presenter.HasBoundModel,Is.False);Assert.That(presenter.Grid,Is.Null);
            yield return test.Action("InitializeAsync");presenter.Refresh();
            Submit(frame.Q<Button>("human-begin"));yield return Finish(presenter);Assert.That(presenter.Grid,Is.Null);
            test.HoldNextCommit=true;Submit(frame.Q<Button>("human-show"));yield return null;presenter.Refresh();
            Assert.That(test.HasHeldWrite,Is.True);Assert.That(presenter.Grid,Is.Null,"No targets, geometry, endpoint or accessibility cell exists before confirmed exposure reservation.");
            Assert.That(frame.Q("puzzle-screen").style.display.value,Is.EqualTo(DisplayStyle.None));
            presenter.SetAccessibilitySuspended(true);test.ReleaseWrite();yield return Finish(presenter);yield return new WaitForSecondsRealtime(.06f);
            Assert.That(presenter.Grid,Is.Not.Null);Assert.That(test.TrialExposure,Is.EqualTo("EXPOSURE_PENDING"),"An attached board must not acknowledge visibility while the external UI fence is closed.");
            Assert.That(presenter.Grid!.CellButton(0,0).enabledSelf,Is.False);
            test.HoldNextCommit=true;presenter.SetAccessibilitySuspended(false);yield return new WaitForSecondsRealtime(.08f);presenter.Refresh();
            Assert.That(test.HasHeldWrite,Is.True,"Actual board geometry must submit one receipt to the observation writer.");
            Assert.That(presenter.Grid.CellButton(0,0).enabledSelf,Is.False);Assert.That(frame.Q<Button>("human-contact-confirm").enabledSelf,Is.False);
            test.ReleaseWrite();yield return Finish(presenter);Assert.That(test.TrialExposure,Is.EqualTo("EXPOSURE_CONFIRMED"));
            Submit(frame.Q<Button>("human-never"));test.HoldNextCommit=true;Submit(frame.Q<Button>("human-contact-confirm"));yield return null;presenter.Refresh();
            Assert.That(presenter.Grid.CellButton(0,0).enabledSelf,Is.False,"A selected but unconfirmed answer cannot open the command path.");
            test.ReleaseWrite();yield return Finish(presenter);presenter.Refresh();yield return null;presenter.Refresh();
            Assert.That(test.BoolState("FirstContactRecorded"),Is.True);var cell=presenter.Grid.CellButton(1,1);Assert.That(cell.enabledSelf,Is.True);
            var board=presenter.Grid.Board.worldBound;Submit(frame.Q<Button>("tool-4"));test.Now+=100;Submit(cell);presenter.Refresh();
            Assert.That(test.Cell(1,1),Is.EqualTo(4));Assert.That(presenter.Grid.CellButton(1,1),Is.SameAs(cell));
            test.Now+=100;Submit(cell);Assert.That(test.Cell(1,1),Is.Zero,"The existing direct tool toggles the same content off.");
            Submit(frame.Q<Button>("puzzle-undo"));Assert.That(test.Cell(1,1),Is.EqualTo(4));
            Submit(frame.Q<Button>("puzzle-zoom"));yield return null;Submit(frame.Q<Button>("puzzle-zoom"));yield return null;
            Assert.That(presenter.Grid.Board.worldBound.width,Is.EqualTo(board.width).Within(.2f));Assert.That(presenter.Grid.CellButton(1,1),Is.SameAs(cell));
            presenter.SetAccessibilitySuspended(true);presenter.Refresh();var elapsed=test.Elapsed;test.Now+=5000;test.Call("Resume",test.Now);presenter.Refresh();
            Assert.That(test.Elapsed,Is.EqualTo(elapsed),"A shared Resume cannot overrule the external pause reason.");Assert.That(cell.enabledSelf,Is.False);
            presenter.SetAccessibilitySuspended(false);presenter.Refresh();
            foreach(var id in new[]{"home-screen","main-navigation","depot-screen","profile-screen","settings-overlay","daily-overlay","endless-overlay"})Assert.That(frame.Q(id).style.display.value,Is.EqualTo(DisplayStyle.None),id);
            Assert.Throws<InvalidOperationException>(()=>presenter.BindServices(()=>new ProductionServiceState(false,false,false,false,null),_=>Task.CompletedTask));
            Assert.Throws<InvalidOperationException>(()=>presenter.BindHintVideo(()=>true,()=>Task.CompletedTask));
            Assert.Throws<InvalidOperationException>(()=>test.Bind(presenter));
        }

        [UnityTest]
        public IEnumerator AssignedHint_DurablePendingBeforeFact_ActualVisibleAckBeforeClose_NoVideoAndNoDuplicateCredit()
        {
            using var test=new HumanFixture();using var phone=new Phone();var frame=Frame();phone.Root.Add(frame);
            using var presenter=new ProductionGamePresenter(frame,Locale(),()=>test.Now);test.Bind(presenter);yield return StartReady(test,presenter,frame);
            test.HoldNextCommit=true;Submit(frame.Q<Button>("puzzle-hint"));
            yield return Until(()=>test.HasHeldWrite,presenter,"The real safe conclusion did not reach its pending reservation.");
            using var locale=new ProductionGameLocalization(Locale());Assert.That(frame.Q<Label>("hint-fact").text,Is.EqualTo(locale.Resolve("hint.preparing")),"A computed but not durably reserved fact must remain invisible.");
            Assert.That(test.HintCount,Is.Zero);Assert.That(test.Credits,Is.EqualTo(3));
            presenter.SetAccessibilitySuspended(true);test.ReleaseWrite();yield return Until(()=>test.Credits==2&&frame.Q<Label>("hint-fact").text!=locale.Resolve("hint.preparing"),presenter,"The confirmed pending offer was not rendered.");
            Assert.That(test.HintCount,Is.Zero);Assert.That(frame.Q<Button>("hint-close").enabledSelf,Is.False);Assert.That(frame.Q<Button>("hint-video").style.display.value,Is.EqualTo(DisplayStyle.None));
            test.HoldNextCommit=true;presenter.SetAccessibilitySuspended(false);yield return Until(()=>test.HasHeldWrite,presenter,"Visible hint geometry did not submit its exact receipt.");
            Assert.That(test.HintCount,Is.Zero,"An unacknowledged observation is not a confirmed hint display.");Assert.That(frame.Q<Button>("hint-close").enabledSelf,Is.False);
            test.ReleaseWrite();yield return Until(()=>test.HintCount==1&&frame.Q<Button>("hint-close").enabledSelf,presenter,"Display confirmation did not become visible before resume.");
            var receipt=typeof(ProductionGamePresenter).GetField("hintReceipt",BindingFlags.NonPublic|BindingFlags.Instance)!.GetValue(presenter)!;
            var duplicate=(Task)test.Call("CommitHintDisplayAsync",receipt,System.Threading.CancellationToken.None)!;yield return WaitTask(duplicate);
            Assert.That(test.HintCount,Is.EqualTo(1));Assert.That(test.Credits,Is.EqualTo(2));
            Submit(frame.Q<Button>("hint-close"));presenter.Refresh();Assert.That(presenter.IsHelpOpen,Is.False);Assert.That(presenter.Grid!.CellButton(1,1).enabledSelf,Is.True);
        }

        [UnityTest]
        public IEnumerator AssignedResult_WaitsForObservationAndActualRide_NextKeepsProductMetagameAbsent()
        {
            using var test=new HumanFixture();using var phone=new Phone();var frame=Frame();phone.Root.Add(frame);
            using var presenter=new ProductionGamePresenter(frame,Locale(),()=>test.Now);test.Bind(presenter);yield return StartReady(test,presenter,frame);
            using var locale=new ProductionGameLocalization(Locale());Assert.That(frame.Q<Label>("human-result-observation").text,Does.Contain(locale.Resolve("human.result.unassisted")));
            Submit(frame.Q<Button>("puzzle-hint"));yield return Until(()=>test.HintCount==1&&frame.Q<Button>("hint-close").enabledSelf,presenter,"The earlier attempt's hint must be visibly acknowledged before restart.");
            Submit(frame.Q<Button>("hint-close"));Submit(frame.Q<Button>("puzzle-back"));Submit(frame.Q<Button>("human-restart"));yield return Finish(presenter);
            yield return Until(()=>test.TrialExposure=="EXPOSURE_CONFIRMED"&&presenter.PendingRequest==null,presenter,"The restarted attempt must receive its own confirmed exposure.");
            Submit(frame.Q<Button>("human-seen"));Submit(frame.Q<Button>("human-contact-confirm"));yield return Finish(presenter);
            Assert.That(test.HintCount,Is.Zero,"The restarted trial has no local hint record.");Assert.That(test.Credits,Is.EqualTo(2),"The same identity retains the earlier attempt's hint history.");
            Submit(frame.Q<Button>("tool-4"));for(var x=0;x<4;x++){test.Now+=100;Submit(presenter.Grid!.CellButton(x,1));}
            presenter.Refresh();Assert.That(test.BoolState("CanStartRide"),Is.False);Assert.That(frame.Q<Button>("result-next").enabledSelf,Is.False);
            test.HoldNextCommit=true;var save=(Task)test.Call("TickAsync",System.Threading.CancellationToken.None)!;yield return Until(()=>test.HasHeldWrite,presenter,"The actual solved checkpoint was not proposed.");
            Assert.That(test.BoolState("CanStartRide"),Is.False);test.ReleaseWrite();yield return WaitTask(save);presenter.Refresh();Assert.That(test.BoolState("CanStartRide"),Is.True);
            var attempt=test.CallGet("AttemptId");Assert.That(test.Call("BeginTrainRide",attempt)!.ToString(),Is.EqualTo("Applied"));presenter.Refresh();Assert.That(frame.Q<Button>("result-next").enabledSelf,Is.False);
            Assert.That(test.Call("CompleteTrainRide",Guid.NewGuid())!.ToString(),Is.EqualTo("Rejected"));Assert.That(test.BoolState("CanAdvance"),Is.False);
            Assert.That(test.Call("CompleteTrainRide",attempt)!.ToString(),Is.EqualTo("Applied"));presenter.Refresh();yield return null;presenter.Refresh();
            Assert.That(test.TrialOutcome,Is.EqualTo("SOLVED_UNREVIEWED"));Assert.That(frame.Q("result-panel").style.display.value,Is.EqualTo(DisplayStyle.Flex));Assert.That(frame.Q<Button>("result-next").enabledSelf,Is.True);
            Assert.That(frame.Q<Label>("human-result-observation").text,Does.Contain(locale.Resolve("human.result.supported")),"A hint from an earlier attempt must not become an unassisted result after restart.");
            foreach(var id in new[]{"result-stars","result-progress","result-balance","result-bonus-state","result-video","result-skip-bonus","result-home"})Assert.That(frame.Q(id).style.display.value,Is.EqualTo(DisplayStyle.None),id);
            Assert.That(frame.Q<Label>("human-result-observation").text,Does.Contain("noch nicht fachlich beurteilt"));
            var original=test.AssignmentId;test.HoldNextCommit=true;Submit(frame.Q<Button>("result-next"));yield return null;presenter.Refresh();Assert.That(test.AssignmentId,Is.EqualTo(original));Assert.That(frame.Q<Button>("result-next").enabledSelf,Is.False);
            test.ReleaseWrite();yield return Finish(presenter);Assert.That(test.AssignmentId,Is.Not.EqualTo(original));Assert.That(presenter.Grid,Is.Null,"The next input needs its own durable exposure before reveal.");
        }

        [UnityTest]
        public IEnumerator NextParticipant_OnlyCompletedRunAndExplicitConfirmationCanRotateToken_CancelRefreshAndPendingCannot()
        {
            using var test=new HumanFixture();using var phone=new Phone();var frame=Frame();phone.Root.Add(frame);
            using var presenter=new ProductionGamePresenter(frame,Locale(),()=>test.Now);test.Bind(presenter);yield return StartReady(test,presenter,frame);
            var firstToken=test.SessionToken;Assert.That(test.StartRunCalls,Is.EqualTo(1));Submit(frame.Q<Button>("human-next-participant"));presenter.Refresh();Assert.That(test.StartRunCalls,Is.EqualTo(1));Assert.That(test.SessionToken,Is.EqualTo(firstToken));
            for(var assignment=0;assignment<2;assignment++)
            {
                if(assignment>0)
                {
                    Submit(frame.Q<Button>("human-show"));yield return Finish(presenter);yield return Until(()=>test.TrialExposure=="EXPOSURE_CONFIRMED",presenter,"The next assigned input was not acknowledged.");
                    Submit(frame.Q<Button>("human-never"));Submit(frame.Q<Button>("human-contact-confirm"));yield return Finish(presenter);presenter.Refresh();
                }
                Submit(frame.Q<Button>("puzzle-back"));Submit(frame.Q<Button>("human-abort"));Submit(frame.Q<Button>("human-abort-confirm"));yield return Finish(presenter);
                Submit(frame.Q<Button>("human-advance"));yield return Finish(presenter);
            }
            Assert.That(test.BoolState("IsFinished"),Is.True);Assert.That(test.BoolState("CanBeginNextParticipant"),Is.True);Assert.That(test.BoolState("ExposureReady"),Is.False);Assert.That(presenter.IsAssignedInstructionOpen,Is.True,"The visible completed-run instruction/export screen must remain an instruction even when the previous contact answer is retained.");
            var previous=test.ConfirmedSnapshot;Submit(frame.Q<Button>("human-next-participant"));yield return null;presenter.Refresh();
            Assert.That(frame.Q("human-participant-overlay").style.display.value,Is.EqualTo(DisplayStyle.Flex));Assert.That(test.StartRunCalls,Is.EqualTo(1));Assert.That(test.SessionToken,Is.EqualTo(firstToken));
            Submit(frame.Q<Button>("human-next-participant-cancel"));presenter.Refresh();presenter.Refresh();Assert.That(test.StartRunCalls,Is.EqualTo(1));Assert.That(test.SessionToken,Is.EqualTo(firstToken));
            presenter.SetAccessibilitySuspended(true);Submit(frame.Q<Button>("human-next-participant"));presenter.Refresh();Assert.That(frame.Q("human-participant-overlay").style.display.value,Is.EqualTo(DisplayStyle.None));Assert.That(test.StartRunCalls,Is.EqualTo(1));
            presenter.SetAccessibilitySuspended(false);presenter.Refresh();Submit(frame.Q<Button>("human-next-participant"));test.HoldNextStart=true;Submit(frame.Q<Button>("human-next-participant-confirm"));yield return null;presenter.Refresh();
            Assert.That(test.HasHeldWrite,Is.True);Assert.That(test.StartRunCalls,Is.EqualTo(2));Assert.That(test.SessionToken,Is.EqualTo(firstToken));
            Submit(frame.Q<Button>("human-next-participant-confirm"));Submit(frame.Q<Button>("human-next-participant-cancel"));presenter.Refresh();Assert.That(test.StartRunCalls,Is.EqualTo(2));Assert.That(frame.Q<Button>("human-next-participant-confirm").enabledSelf,Is.False);
            test.ReleaseWrite();yield return Finish(presenter);presenter.Refresh();Assert.That(test.SessionToken,Is.Not.EqualTo(firstToken));Assert.That(test.StartRunCalls,Is.EqualTo(2));Assert.That(test.RetainedRuns.Single(),Is.SameAs(previous));
            Assert.That(test.BoolState("FirstContactRecorded"),Is.False);Assert.That(presenter.Grid,Is.Null);Assert.That(frame.Q("human-participant-overlay").style.display.value,Is.EqualTo(DisplayStyle.None));
            presenter.Refresh();presenter.Refresh();Assert.That(test.StartRunCalls,Is.EqualTo(2),"Refresh and opening the next instruction never manufacture another token.");
        }

        private static IEnumerator StartReady(HumanFixture test,ProductionGamePresenter presenter,VisualElement frame)
        {
            yield return test.Action("InitializeAsync");presenter.Refresh();Submit(frame.Q<Button>("human-begin"));yield return Finish(presenter);
            Submit(frame.Q<Button>("human-show"));yield return Finish(presenter);
            yield return Until(()=>test.TrialExposure=="EXPOSURE_CONFIRMED",presenter,"The first actual board receipt was not confirmed.");
            Submit(frame.Q<Button>("human-never"));Submit(frame.Q<Button>("human-contact-confirm"));yield return Finish(presenter);presenter.Refresh();yield return null;presenter.Refresh();
            Assert.That(presenter.Grid!.CellButton(1,1).enabledSelf,Is.True);
        }
        private static IEnumerator Until(Func<bool> predicate,ProductionGamePresenter presenter,string error)
        {for(var i=0;i<300&&!predicate();i++){presenter.Refresh();yield return null;}presenter.Refresh();Assert.That(predicate(),Is.True,error);}
        private static IEnumerator WaitTask(Task task)
        {for(var i=0;i<300&&!task.IsCompleted;i++)yield return null;Assert.That(task.IsCompleted,Is.True);task.GetAwaiter().GetResult();}

        private static IEnumerator Finish(ProductionGamePresenter presenter)
        {
            for(var i=0;i<180&&presenter.PendingRequest!=null;i++){presenter.Refresh();yield return null;}
            presenter.Refresh();Assert.That(presenter.PendingRequest,Is.Null,"A controlled UI operation did not complete.");Assert.That(presenter.HasRequestFault,Is.False);
        }
        private static void Submit(Button button){using var e=NavigationSubmitEvent.GetPooled();e.target=button;button.SendEvent(e);}

        private sealed class Phone:IDisposable
        {
            private readonly GameObject go;private readonly PanelSettings panel;private readonly RenderTexture target;
            internal Phone()
            {
                panel=ScriptableObject.CreateInstance<PanelSettings>();panel.themeStyleSheet=AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(Ui+"DevelopmentPuzzleTheme.tss");panel.scaleMode=PanelScaleMode.ConstantPixelSize;
                target=new RenderTexture(390,844,24);target.Create();panel.targetTexture=target;go=new GameObject("AssignedReviewProductPhone");var document=go.AddComponent<UIDocument>();document.panelSettings=panel;Root=document.rootVisualElement;
            }
            internal VisualElement Root{get;}
            public void Dispose(){panel.targetTexture=null;target.Release();UnityEngine.Object.Destroy(target);UnityEngine.Object.Destroy(go);UnityEngine.Object.Destroy(panel);}
        }

        // Reflection preserves the established test assembly boundary; the actual adapter and contracts are never replaced.
        private sealed class HumanFixture:IDisposable
        {
            private readonly Assembly app=AppDomain.CurrentDomain.GetAssemblies().Single(a=>a.GetName().Name=="STP.Application");
            private readonly object repository,catalog;private object? snapshot,heldSnapshot,heldCompletion;
            internal readonly object Session;
            internal long Now;
            internal bool HoldNextCommit;
            internal bool HoldNextStart;
            internal int StartRunCalls;
            internal readonly List<object> RetainedRuns=new List<object>();
            internal bool HasHeldWrite=>heldCompletion!=null;
            internal HumanFixture()
            {
                var assignments=Array.CreateInstance(Type("HumanAssignment"),2);
                for(var i=0;i<2;i++)
                {
                    var identity=New("PublicPuzzleIdentity","S1-01-01-0"+(i+1),"STP-PUZZLE-SEMANTIC-JCS-1",new string((char)('a'+i),64));
                    var puzzle=New("PublicPuzzleInput",identity,"train-track-v1",4,4,Enum.Parse(Type("PublicPuzzleSide"),"W"),1,Enum.Parse(Type("PublicPuzzleSide"),"E"),1,new[]{0,4,0,0},new[]{1,1,1,1});
                    assignments.SetValue(New("HumanAssignment","assigned-ui-"+(i+1),Enum.Parse(Type("HumanAssignmentPurpose"),"CAMPAIGN_TIME"),puzzle,new string('c',64),new string('d',64),1,null,null),i);
                }
                catalog=HumanContractProxy.Create(Type("IHumanAssignmentCatalog"),(method,args)=>method.Name switch
                {
                    "get_Assignments"=>assignments,"get_ManifestBytesSha256"=>new string('e',64),"get_StudyId"=>"ui-observation-test","get_Revision"=>1,"get_BuildId"=>"automated-ui-test","get_UiSourceSha256"=>new string('f',64),"get_PublicCatalogHashSha256"=>new string('1',64),_=>throw new NotSupportedException(method.Name)
                });
                repository=HumanContractProxy.Create(Type("IHumanCalibrationRepository"),RepositoryCall);
                // This constructor is deliberately bound to B's real API, never an invented SavedReviewState.
                var clock=HumanContractProxy.Create(Type("ILocalSessionClock"),(method,args)=>method.Name=="get_MonotonicMilliseconds"?(object)Now:"2026-10-21T12:00:00Z");
                var context=System.Threading.SynchronizationContext.Current??throw new InvalidOperationException("UNITY_TEST_OWNER_CONTEXT_REQUIRED");var owner=System.Threading.Thread.CurrentThread.ManagedThreadId;
                var dispatcher=HumanContractProxy.Create(Type("ILocalSaveDispatcher"),(method,args)=>
                {
                    var action=(Action)args[0]!;if(System.Threading.Thread.CurrentThread.ManagedThreadId==owner){action();return Task.CompletedTask;}
                    var completion=new TaskCompletionSource<bool>();context.Post(_=>{try{action();completion.SetResult(true);}catch(Exception e){completion.SetException(e);}},null);return completion.Task;
                });
                Session=New("AssignedHumanSession",catalog,repository,clock,dispatcher,Enum.Parse(Type("HumanRunOrigin"),"AUTOMATED_TEST"));
            }
            internal string TrialExposure=>Get(Get(State,"Trial"),"Exposure").ToString()!;
            internal string SessionToken=>(string)Get(Get(snapshot!,"Run"),"SessionToken");
            internal object ConfirmedSnapshot=>snapshot!;
            internal string TrialOutcome=>Get(Get(State,"Trial"),"Outcome").ToString()!;
            internal string AssignmentId=>(string)Get(Get(State,"Assignment"),"Id");
            internal int HintCount=>(int)Get(Get(Get(State,"Trial"),"Checkpoint"),"HintCount");
            internal int Credits=>(int)Get(State,"HintCredits");
            private object State=>Get(Session,"State");
            internal bool BoolState(string name)=>(bool)Get(State,name);
            internal long Elapsed=>(long)Call("ElapsedAt",Now)!;
            internal int Cell(int x,int y)=>Convert.ToInt32(Call("CellAt",x,y));
            internal object? Call(string name,params object?[] args)=>Session.GetType().GetMethods().Single(m=>m.Name==name&&m.GetParameters().Length==args.Length).Invoke(Session,args);
            internal object CallGet(string property)=>Get(Session,property);
            internal IEnumerator Action(string name)
            {var task=(Task)Call(name,System.Threading.CancellationToken.None)!;for(var i=0;i<180&&!task.IsCompleted;i++)yield return null;Assert.That(task.IsCompleted,Is.True);task.GetAwaiter().GetResult();}
            internal void Bind(ProductionGamePresenter presenter)
            {
                var assignment=Type("HumanAssignment");var callback=typeof(HumanFixture).GetMethod(nameof(Display),BindingFlags.NonPublic|BindingFlags.Static)!;
                var display=Delegate.CreateDelegate(typeof(Func<,>).MakeGenericType(assignment,typeof(ProductionPuzzleDisplay)),callback);
                try{typeof(ProductionGamePresenter).GetMethod("BindAssignedReview")!.Invoke(presenter,new object?[]{Session,display,null});}
                catch(TargetInvocationException e)when(e.InnerException!=null){throw e.InnerException;}
            }
            private static ProductionPuzzleDisplay Display(object assignment)=>new ProductionPuzzleDisplay("Prüfung","Zugewiesen","Eine zugewiesene Verbindung","Verbinde A und B.","Die Verbindung steht.");
            internal void ReleaseWrite()
            {Assert.That(heldCompletion,Is.Not.Null);snapshot=heldSnapshot;var result=Result("Committed",snapshot);heldCompletion!.GetType().GetMethod("SetResult")!.Invoke(heldCompletion,new[]{result});heldCompletion=null;heldSnapshot=null;}
            private object? RepositoryCall(MethodInfo method,object?[] args)
            {
                switch(method.Name)
                {
                    case "LoadAsync":return Completed(Result(snapshot==null?"Missing":"Ready",snapshot));
                    case "StartRunAsync":
                        StartRunCalls++;if(snapshot!=null){Assert.That(args[1],Is.EqualTo(SessionToken));RetainedRuns.Add(snapshot);}
                        var next=New("HumanObservationSnapshot",1L,args[0]);
                        if(HoldNextStart){HoldNextStart=false;heldSnapshot=next;heldCompletion=Activator.CreateInstance(typeof(TaskCompletionSource<>).MakeGenericType(Type("HumanStoreResult")))!;return Get(heldCompletion,"Task");}
                        snapshot=next;return Completed(Result("Committed",snapshot));
                    case "CommitAsync":
                        Assert.That((long)Get(snapshot!,"Generation"),Is.EqualTo((long)args[1]!));
                        if(HoldNextCommit){HoldNextCommit=false;heldSnapshot=args[0];heldCompletion=Activator.CreateInstance(typeof(TaskCompletionSource<>).MakeGenericType(Type("HumanStoreResult")))!;return Get(heldCompletion,"Task");}
                        snapshot=args[0];return Completed(Result("Committed",snapshot));
                    case "RecoverAsync":return Completed(Result(snapshot==null?"Missing":"Ready",snapshot));
                    case "ExportAsync":return Task.FromResult(Array.Empty<byte>());
                    default:throw new NotSupportedException(method.Name);
                }
            }
            private object Result(string status,object? value)=>New("HumanStoreResult",Enum.Parse(Type("HumanStoreStatus"),status),"NONE",value,"ui-observed");
            private object Completed(object result)=>typeof(Task).GetMethod(nameof(Task.FromResult))!.MakeGenericMethod(Type("HumanStoreResult")).Invoke(null,new[]{result})!;
            private Type Type(string name)=>app.GetType("STP.Application."+name,true)!;
            private object New(string name,params object?[] args)=>Activator.CreateInstance(Type(name),args)!;
            private static object Get(object value,string property)=>value.GetType().GetProperty(property)!.GetValue(value)!;
            public void Dispose(){if(Session is IDisposable disposable)disposable.Dispose();}
        }
#endif
    }

    public class HumanContractProxy:DispatchProxy
    {
        private Func<MethodInfo,object?[],object?>? call;
        public static object Create(Type contract,Func<MethodInfo,object?[],object?> invoke)
        {
            var method=typeof(DispatchProxy).GetMethods().Single(m=>m.Name=="Create"&&m.IsGenericMethodDefinition);
            var value=method.MakeGenericMethod(contract,typeof(HumanContractProxy)).Invoke(null,null)!;((HumanContractProxy)value).call=invoke;return value;
        }
        protected override object? Invoke(MethodInfo? targetMethod,object?[]? args)=>call!(targetMethod!,args??Array.Empty<object?>());
    }
}
