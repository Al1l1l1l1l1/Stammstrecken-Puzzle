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
    /// <summary>Explicit UI lifecycle over controlled Root facts; no profile, Store, file or scene operation occurs here.</summary>
    public sealed class LocalDataDeletionPresentationTests
    {
        [Test]
        public void ViewAndActionMatrix_RejectUnknown_AndOnlyPermitStateSpecificIntent()
        {
            Assert.Throws<ArgumentOutOfRangeException>(()=>new ProductionLocalDataView((ProductionLocalDataState)99));
            var gate=typeof(ProductionProductPanels).GetMethod("AllowsLocalDataAction",BindingFlags.NonPublic|BindingFlags.Static)!;
            foreach(ProductionLocalDataState state in Enum.GetValues(typeof(ProductionLocalDataState)))
            foreach(ProductionLocalDataAction action in Enum.GetValues(typeof(ProductionLocalDataAction)))
            {
                var expected=state==ProductionLocalDataState.Ready&&action==ProductionLocalDataAction.Open||
                    state==ProductionLocalDataState.Confirmation&&(action==ProductionLocalDataAction.Cancel||action==ProductionLocalDataAction.Confirm)||
                    state==ProductionLocalDataState.RetryRequired&&(action==ProductionLocalDataAction.Retry||action==ProductionLocalDataAction.Restart)||state==ProductionLocalDataState.Deleted&&action==ProductionLocalDataAction.Restart;
                Assert.That(gate.Invoke(null,new object[]{state,false,action}),Is.EqualTo(expected),state+"/"+action);
                if(action==ProductionLocalDataAction.Open||action==ProductionLocalDataAction.Confirm||action==ProductionLocalDataAction.Cancel)
                    Assert.That(gate.Invoke(null,new object[]{state,true,action}),Is.False,"An attempted confirmation cannot be opened, cancelled or reconfirmed: "+state+"/"+action);
            }
            Assert.That(gate.Invoke(null,new object[]{ProductionLocalDataState.Ready,false,(ProductionLocalDataAction)99}),Is.False);
        }
#if UNITY_EDITOR
        private const string Ui="Assets/StammstreckenPuzzle/UI/";
        private sealed class Phone:IDisposable
        {
            internal readonly VisualElement Frame;
            internal readonly ProductionGameLocalization Locale;
            internal readonly ProductionProductPanels Panels;
            private readonly GameObject owner;
            private readonly PanelSettings panel;
            private readonly RenderTexture texture;
            internal Phone(int width=390,int height=844)
            {
                Frame=AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Ui+"ProductionGame.uxml").CloneTree();Frame.style.width=width;Frame.style.height=height;Frame.style.flexShrink=0;
                Frame.Q("startup-overlay").style.display=DisplayStyle.None;Frame.Q("home-screen").style.display=DisplayStyle.None;Frame.Q("profile-screen").style.display=DisplayStyle.Flex;
                Locale=new ProductionGameLocalization(AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"ProductionGameLocale.json"));Panels=new ProductionProductPanels(Frame,Locale);
                panel=ScriptableObject.CreateInstance<PanelSettings>();panel.themeStyleSheet=AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(Ui+"DevelopmentPuzzleTheme.tss");panel.scaleMode=PanelScaleMode.ConstantPixelSize;
                texture=new RenderTexture(width,height,24);texture.Create();panel.targetTexture=texture;
                owner=new GameObject("ExplicitProfileDeletionUi");var doc=owner.AddComponent<UIDocument>();doc.panelSettings=panel;
                doc.rootVisualElement.style.width=width;doc.rootVisualElement.style.height=height;doc.rootVisualElement.Add(Frame);
            }
            internal Button Button(string name)=>Frame.Q<Button>(name);
            internal VisualElement Overlay=>Frame.Q("local-data-overlay");
            internal Label Status=>Frame.Q<Label>("local-data-state");
            public void Dispose(){Panels.Dispose();Locale.Dispose();UnityEngine.Object.Destroy(owner);UnityEngine.Object.Destroy(panel);texture.Release();UnityEngine.Object.Destroy(texture);}
        }
        private static void Submit(Button button){using var e=NavigationSubmitEvent.GetPooled();e.target=button;button.SendEvent(e);}
        private static void Shown(VisualElement element,bool expected)=>Assert.That(element.style.display.value,Is.EqualTo(expected?DisplayStyle.Flex:DisplayStyle.None),element.name);

        [UnityTest]
        public IEnumerator OpenAndCancel_NeverConfirm_BothSettingsAndStartupErrorReturnToPreviousScreen()
        {
            using var phone=new Phone();var state=ProductionLocalDataState.Ready;var calls=new List<ProductionLocalDataAction>();
            phone.Panels.BindLocalDataDeletion(()=>new ProductionLocalDataView(state),a=>{calls.Add(a);state=a==ProductionLocalDataAction.Open?ProductionLocalDataState.Confirmation:ProductionLocalDataState.Ready;return Task.CompletedTask;});
            yield return null;phone.Panels.Refresh();Assert.That(calls,Is.Empty);Shown(phone.Overlay,false);
            Submit(phone.Button("profile-settings"));Submit(phone.Button("settings-delete-local"));
            Assert.That(calls,Is.EqualTo(new[]{ProductionLocalDataAction.Open}));Shown(phone.Overlay,true);Assert.That(phone.Panels.IsLocalDataBlocking,Is.True);
            Assert.That(phone.Frame.Q<Label>("local-data-body").text,Does.Contain("unwiderruflich"));Assert.That(phone.Frame.Q<Label>("local-data-body").text,Does.Contain("Storekäufe bleiben bestehen"));
            Submit(phone.Button("local-data-cancel"));Assert.That(calls,Is.EqualTo(new[]{ProductionLocalDataAction.Open,ProductionLocalDataAction.Cancel}));Shown(phone.Overlay,false);Shown(phone.Frame.Q("settings-overlay"),true);
            Submit(phone.Button("settings-close"));Assert.That(phone.Panels.IsModalOpen,Is.False);
            foreach(var origin in new[]{"startup","error"})
            {
                var previous=phone.Frame.Q(origin+"-overlay");previous.style.display=DisplayStyle.Flex;Submit(phone.Button(origin+"-delete-local"));
                Shown(phone.Overlay,true);Assert.That(previous.style.display.value,Is.EqualTo(DisplayStyle.Flex));
                Assert.That(phone.Overlay.parent.IndexOf(phone.Overlay),Is.GreaterThan(previous.parent.IndexOf(previous)),"Deletion must be above the real startup/save-error overlay.");
                Submit(phone.Button("local-data-cancel"));Shown(phone.Overlay,false);Shown(previous,true);Assert.That(phone.Panels.IsModalOpen,Is.False);
                previous.style.display=DisplayStyle.None;
            }
            Assert.That(calls.Count(a=>a==ProductionLocalDataAction.Confirm),Is.Zero);
        }

        [UnityTest]
        public IEnumerator DeliberateConfirm_ReentrantAndDuplicateTapsCannotRepeat_OnlyRootDeletedAllowsRestart()
        {
            using var phone=new Phone();phone.Frame.Q("startup-overlay").style.display=DisplayStyle.Flex;var state=ProductionLocalDataState.Ready;var calls=new List<ProductionLocalDataAction>();var pending=new TaskCompletionSource<bool>();var otherCalls=0;
            phone.Panels.BindDaily(()=>new DailyView("Datum",true,true,null,false),_=>{otherCalls++;return Task.CompletedTask;});
            phone.Panels.BindServices(()=>new ProductionServiceState(true,true,false,false,null,true,true,true),_=>{otherCalls++;return Task.CompletedTask;});
            phone.Panels.BindLocalDataDeletion(()=>new ProductionLocalDataView(state),a=>
            {
                calls.Add(a);if(a==ProductionLocalDataAction.Open){state=ProductionLocalDataState.Confirmation;return Task.CompletedTask;}
                if(a==ProductionLocalDataAction.Confirm){state=ProductionLocalDataState.Deleting;phone.Panels.Refresh();Submit(phone.Button("local-data-confirm"));return pending.Task;}
                return Task.CompletedTask;
            });
            yield return null;Submit(phone.Button("startup-delete-local"));Submit(phone.Button("local-data-confirm"));Submit(phone.Button("local-data-confirm"));Submit(phone.Button("local-data-cancel"));Submit(phone.Button("local-data-restart"));
            Submit(phone.Button("settings-legal"));Submit(phone.Button("home-daily"));Submit(phone.Button("daily-free"));Submit(phone.Button("settings-close"));
            Assert.That(calls,Is.EqualTo(new[]{ProductionLocalDataAction.Open,ProductionLocalDataAction.Confirm}));Assert.That(otherCalls,Is.Zero);Assert.That(phone.Panels.IsModalOpen,Is.True);
            Assert.That(phone.Status.text,Is.EqualTo(phone.Locale.Resolve("local-data.deleting")));Assert.That(phone.Button("local-data-cancel").enabledSelf,Is.False);
            pending.SetResult(true);phone.Panels.Refresh();Assert.That(phone.Button("local-data-restart").enabledSelf,Is.False,"A successful task is not the Root Deleted fact.");
            state=ProductionLocalDataState.Deleted;phone.Panels.Refresh();Assert.That(phone.Button("local-data-restart").enabledSelf,Is.True);
            Assert.That(phone.Frame.Q<Label>("local-data-title").text,Is.EqualTo(phone.Locale.Resolve("local-data.deleted.title")));
            Submit(phone.Button("local-data-restart"));Assert.That(calls.Last(),Is.EqualTo(ProductionLocalDataAction.Restart));Assert.That(phone.Panels.IsLocalDataBlocking,Is.True,"The old binder remains closed until Root actually replaces it.");
        }

        [UnityTest]
        public IEnumerator UnknownOutcome_OnlyExactRetry_NoNewConfirmCancelOrPrematureReturnToGame()
        {
            using var phone=new Phone();phone.Frame.Q("startup-overlay").style.display=DisplayStyle.Flex;var state=ProductionLocalDataState.Ready;var calls=new List<ProductionLocalDataAction>();var retry=new TaskCompletionSource<bool>();
            phone.Panels.BindLocalDataDeletion(()=>new ProductionLocalDataView(state),a=>
            {
                calls.Add(a);if(a==ProductionLocalDataAction.Open){state=ProductionLocalDataState.Confirmation;return Task.CompletedTask;}
                if(a==ProductionLocalDataAction.Confirm){state=ProductionLocalDataState.RetryRequired;return Task.FromException(new InvalidOperationException("CONTROLLED_DELETE_ACK_UNKNOWN"));}
                return retry.Task;
            });
            yield return null;Submit(phone.Button("startup-delete-local"));Submit(phone.Button("local-data-confirm"));
            Assert.That(phone.Button("local-data-retry").enabledSelf,Is.True);Submit(phone.Button("local-data-cancel"));Submit(phone.Button("local-data-confirm"));
            Assert.That(phone.Button("local-data-restart").enabledSelf,Is.True);Assert.That(phone.Button("local-data-restart").text,Is.EqualTo(phone.Locale.Resolve("local-data.reload")));
            Assert.That(calls.Count,Is.EqualTo(2));Shown(phone.Overlay,true);
            state=ProductionLocalDataState.Ready;phone.Panels.Refresh();Submit(phone.Button("startup-delete-local"));Submit(phone.Button("profile-settings"));
            Assert.That(phone.Panels.IsLocalDataBlocking,Is.True,"A completed/failed attempt cannot become a fresh intent merely because a stale Root projection says Ready.");Assert.That(calls.Count,Is.EqualTo(2));
            state=ProductionLocalDataState.RetryRequired;phone.Panels.Refresh();Submit(phone.Button("local-data-retry"));Submit(phone.Button("local-data-retry"));
            Assert.That(calls,Is.EqualTo(new[]{ProductionLocalDataAction.Open,ProductionLocalDataAction.Confirm,ProductionLocalDataAction.Retry}));Assert.That(phone.Button("local-data-retry").enabledSelf,Is.False);
            state=ProductionLocalDataState.Deleted;retry.SetResult(true);phone.Panels.Refresh();Assert.That(phone.Button("local-data-restart").enabledSelf,Is.True);
        }

        [UnityTest]
        public IEnumerator FreshReadAndExternalFocus_CloseAllDeleteActions_AndAReadFailureCannotCancelAnUnknownAttempt()
        {
            using var phone=new Phone();phone.Frame.Q("startup-overlay").style.display=DisplayStyle.Flex;var state=ProductionLocalDataState.Ready;var failRead=false;var calls=new List<ProductionLocalDataAction>();
            phone.Panels.BindLocalDataDeletion(()=>failRead?throw new InvalidOperationException("CONTROLLED_LIFECYCLE_READ"):new ProductionLocalDataView(state),a=>{calls.Add(a);state=a==ProductionLocalDataAction.Open?ProductionLocalDataState.Confirmation:ProductionLocalDataState.Ready;return Task.CompletedTask;});
            yield return null;state=ProductionLocalDataState.Unavailable;Submit(phone.Button("startup-delete-local"));Assert.That(calls,Is.Empty,"A stale enabled trigger must re-read the current Root state.");
            state=ProductionLocalDataState.Ready;phone.Panels.Refresh();Submit(phone.Button("startup-delete-local"));phone.Panels.SetExternalSuspended(true);
            Submit(phone.Button("local-data-confirm"));Submit(phone.Button("local-data-cancel"));Assert.That(calls.Count,Is.EqualTo(1));Assert.That(phone.Button("local-data-confirm").enabledSelf,Is.False);
            phone.Panels.SetExternalSuspended(false);Assert.That(phone.Button("local-data-cancel").enabledSelf,Is.True);
            failRead=true;Submit(phone.Button("local-data-confirm"));Assert.That(calls.Count,Is.EqualTo(1));Assert.That(phone.Panels.IsModalOpen,Is.True);Assert.That(phone.Button("local-data-cancel").enabledSelf,Is.False);
            failRead=false;phone.Panels.Refresh();Submit(phone.Button("local-data-cancel"));Assert.That(calls.Last(),Is.EqualTo(ProductionLocalDataAction.Cancel));
        }

        [UnityTest]
        public IEnumerator BootFactsAndPhoneGeometry_AreTopmostScrollableReadable_WithoutPlayableModelOrHiddenInput()
        {
            foreach(var size in new[]{new Vector2Int(320,568),new Vector2Int(390,844)})
            {
                using var phone=new Phone(size.x,size.y);var state=ProductionLocalDataState.Confirmation;var calls=0;
                phone.Frame.Q("startup-overlay").style.display=DisplayStyle.Flex;phone.Frame.Q("error-overlay").style.display=DisplayStyle.Flex;
                phone.Panels.BindLocalDataDeletion(()=>new ProductionLocalDataView(state),_=>{calls++;return Task.CompletedTask;});yield return null;yield return null;
                var scroll=phone.Frame.Q<ScrollView>("local-data-scroll");var cancel=phone.Button("local-data-cancel");var confirm=phone.Button("local-data-confirm");
                Assert.That(scroll.contentViewport.worldBound.height,Is.GreaterThan(44),"Phone="+size);Assert.That(confirm.worldBound.height,Is.GreaterThanOrEqualTo(48));Assert.That(cancel.worldBound.height,Is.GreaterThanOrEqualTo(44));
                Assert.That(cancel.worldBound.yMax,Is.LessThanOrEqualTo(size.y-21),"Phone="+size+" Modal="+phone.Overlay.worldBound+" Cancel="+cancel.worldBound);Assert.That(confirm.worldBound.xMin,Is.GreaterThanOrEqualTo(21));Assert.That(confirm.worldBound.xMax,Is.LessThanOrEqualTo(size.x-21));
                scroll.ScrollTo(phone.Status);yield return null;Assert.That(phone.Status.worldBound.yMax,Is.LessThanOrEqualTo(scroll.contentViewport.worldBound.yMax+1));
                using var semantics=new ProductionGameAccessibility(phone.Frame,phone.Locale.Resolve,false);semantics.RefreshForInspection();
                Assert.That(semantics.TryActivate(phone.Button("profile-settings")),Is.False,"The topmost delete modal owns accessibility, even over actual Startup/Error overlays.");
                Assert.That(calls,Is.Zero);Assert.That(phone.Panels.IsModalOpen,Is.True);
                foreach(var closed in new[]{ProductionLocalDataState.Deleting,ProductionLocalDataState.RetryRequired,ProductionLocalDataState.Deleted})
                {state=closed;phone.Panels.Refresh();Shown(phone.Overlay,true);Assert.That(phone.Panels.IsLocalDataBlocking,Is.True);}
                state=ProductionLocalDataState.Ready;phone.Panels.Refresh();Shown(phone.Overlay,true);Assert.That(phone.Panels.IsLocalDataBlocking,Is.True,"A boot-observed confirmed deletion also fences the old binder permanently.");
                Assert.That(calls,Is.Zero,"Reading boot lifecycle facts is not confirmation or automatic restart.");
            }
        }

        [UnityTest]
        public IEnumerator RecoveryRestart_IsExplicitReload_NotDeletionSuccess_AndBlocksDuplicatesWhilePending()
        {
            using var phone=new Phone();var calls=new List<ProductionLocalDataAction>();var pending=new TaskCompletionSource<bool>();
            phone.Panels.BindLocalDataDeletion(()=>new ProductionLocalDataView(ProductionLocalDataState.RetryRequired),a=>{calls.Add(a);return pending.Task;});yield return null;
            Assert.That(calls,Is.Empty);Assert.That(phone.Button("local-data-restart").text,Is.EqualTo(phone.Locale.Resolve("local-data.reload")));
            Assert.That(phone.Frame.Q<Label>("local-data-title").text,Is.Not.EqualTo(phone.Locale.Resolve("local-data.deleted.title")));
            Assert.That(phone.Frame.Q<Label>("local-data-body").text,Is.Not.EqualTo(phone.Locale.Resolve("local-data.deleted.body")));
            Submit(phone.Button("local-data-restart"));Submit(phone.Button("local-data-restart"));Submit(phone.Button("local-data-retry"));Submit(phone.Button("local-data-cancel"));
            Assert.That(calls,Is.EqualTo(new[]{ProductionLocalDataAction.Restart}));Assert.That(phone.Panels.IsModalOpen,Is.True);Assert.That(phone.Button("local-data-restart").enabledSelf,Is.False);
            pending.SetResult(true);phone.Panels.Refresh();Assert.That(phone.Frame.Q<Label>("local-data-title").text,Is.Not.EqualTo(phone.Locale.Resolve("local-data.deleted.title")),"A completed reload task cannot synthesize deletion success.");
        }

        [UnityTest]
        public IEnumerator DisposeDuringPending_ObservesLateFailureWithoutRenderingOrFurtherAction()
        {
            using var phone=new Phone();var state=ProductionLocalDataState.Confirmation;var calls=0;var pending=new TaskCompletionSource<bool>();
            phone.Panels.BindLocalDataDeletion(()=>new ProductionLocalDataView(state),_=>{calls++;state=ProductionLocalDataState.Deleting;return pending.Task;});yield return null;
            Submit(phone.Button("local-data-confirm"));var before=phone.Status.text;phone.Panels.Dispose();pending.SetException(new InvalidOperationException("CONTROLLED_LATE_DELETE_FAILURE"));
            phone.Panels.Refresh();Submit(phone.Button("local-data-retry"));yield return null;Assert.That(calls,Is.EqualTo(1));Assert.That(phone.Status.text,Is.EqualTo(before));
        }
#endif
    }
}
