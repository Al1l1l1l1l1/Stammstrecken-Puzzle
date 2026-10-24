using System;
using System.Collections;
using System.Collections.Generic;
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
    /// <summary>Explicit publisher actions over Root readiness; no URL, network or browser is opened by these fixtures.</summary>
    public sealed class PublisherContactsPresentationTests
    {
        [Test]
        public void ContactReadiness_OmittedFlagsStayClosed_AndEachFlagIsIndependent()
        {
            var legacy=new ProductionServiceState(true,true,false,false,null,true);
            Assert.That(legacy.CanShowLegal,Is.False);Assert.That(legacy.CanContactSupport,Is.False);
            var legal=new ProductionServiceState(false,false,false,false,null,canShowLegal:true);
            Assert.That(legal.CanShowLegal,Is.True);Assert.That(legal.CanContactSupport,Is.False);
            var support=new ProductionServiceState(false,false,false,false,null,canContactSupport:true);
            Assert.That(support.CanShowLegal,Is.False);Assert.That(support.CanContactSupport,Is.True);
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
            internal Phone()
            {
                Frame=AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Ui+"ProductionGame.uxml").CloneTree();Frame.style.width=390;Frame.style.height=844;Frame.style.flexShrink=0;
                Frame.Q("startup-overlay").style.display=DisplayStyle.None;Frame.Q("home-screen").style.display=DisplayStyle.None;Frame.Q("profile-screen").style.display=DisplayStyle.Flex;
                Locale=new ProductionGameLocalization(AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"ProductionGameLocale.json"));Panels=new ProductionProductPanels(Frame,Locale);
                panel=ScriptableObject.CreateInstance<PanelSettings>();panel.themeStyleSheet=AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(Ui+"DevelopmentPuzzleTheme.tss");panel.scaleMode=PanelScaleMode.ConstantPixelSize;
                texture=new RenderTexture(390,844,24);texture.Create();panel.targetTexture=texture;
                owner=new GameObject("PublisherContactsExplicitActions");var doc=owner.AddComponent<UIDocument>();doc.panelSettings=panel;
                doc.rootVisualElement.style.width=390;doc.rootVisualElement.style.height=844;doc.rootVisualElement.Add(Frame);
            }
            internal Button Button(string name)=>Frame.Q<Button>(name);
            internal Label Status=>Frame.Q<Label>("settings-contact-state");
            public void Dispose(){Panels.Dispose();Locale.Dispose();UnityEngine.Object.Destroy(owner);UnityEngine.Object.Destroy(panel);texture.Release();UnityEngine.Object.Destroy(texture);}
        }
        private static ProductionServiceState State(bool legal=false,bool support=false)=>new ProductionServiceState(false,false,false,false,null,canShowLegal:legal,canContactSupport:support);
        private static void Submit(Button button){using var e=NavigationSubmitEvent.GetPooled();e.target=button;button.SendEvent(e);}

        [UnityTest]
        public IEnumerator Contacts_OpeningAndRefreshNeverCall_IndependentReadinessAndReachableStatus()
        {
            using var phone=new Phone();var calls=new List<ProductionServiceAction>();var state=State();
            phone.Panels.BindServices(()=>state,a=>{calls.Add(a);return Task.CompletedTask;});yield return null;
            Submit(phone.Button("profile-settings"));phone.Panels.Refresh();yield return null;
            Assert.That(phone.Panels.IsModalOpen,Is.True);Assert.That(calls,Is.Empty);
            var legal=phone.Button("settings-legal");var support=phone.Button("settings-support");
            Assert.That(legal.enabledSelf,Is.False);Assert.That(support.enabledSelf,Is.False);
            Assert.That(phone.Status.text,Is.EqualTo(phone.Locale.Resolve("settings.contacts.unavailable")));
            state=State(true);phone.Panels.Refresh();Assert.That(legal.enabledSelf,Is.True);Assert.That(support.enabledSelf,Is.False);
            Assert.That(phone.Status.text,Is.EqualTo(phone.Locale.Resolve("settings.support.unavailable")));
            state=State(false,true);phone.Panels.Refresh();Assert.That(legal.enabledSelf,Is.False);Assert.That(support.enabledSelf,Is.True);
            Assert.That(phone.Status.text,Is.EqualTo(phone.Locale.Resolve("settings.legal.unavailable")));
            state=State(true,true);phone.Panels.Refresh();Assert.That(phone.Status.style.display.value,Is.EqualTo(DisplayStyle.None));Assert.That(calls,Is.Empty);
            var scroll=phone.Frame.Q("settings-overlay").Q<ScrollView>();scroll.ScrollTo(support);yield return null;
            Assert.That(support.worldBound.height,Is.GreaterThanOrEqualTo(44));Assert.That(support.worldBound.yMin,Is.GreaterThanOrEqualTo(scroll.contentViewport.worldBound.yMin-1));
            Assert.That(support.worldBound.yMax,Is.LessThanOrEqualTo(scroll.contentViewport.worldBound.yMax+1));Assert.That(support.text,Is.Not.Empty);
        }

        [UnityTest]
        public IEnumerator Contacts_ExplicitTapRechecksReadiness_AndPendingBlocksDuplicateOtherContactAndClose()
        {
            using var phone=new Phone();var calls=new List<ProductionServiceAction>();var state=State(true,true);var pending=new TaskCompletionSource<bool>();
            phone.Panels.BindServices(()=>state,a=>{calls.Add(a);return a==ProductionServiceAction.ShowLegal?pending.Task:Task.CompletedTask;});yield return null;
            Submit(phone.Button("profile-settings"));var legal=phone.Button("settings-legal");var support=phone.Button("settings-support");
            state=State(false,true);Submit(legal);Assert.That(calls,Is.Empty,"A stale enabled contact cannot bypass the fresh Root read model.");Assert.That(legal.enabledSelf,Is.False);
            state=State(true,true);phone.Panels.Refresh();Submit(legal);Submit(legal);Submit(support);Submit(phone.Button("settings-close"));
            Assert.That(calls,Is.EqualTo(new[]{ProductionServiceAction.ShowLegal}));Assert.That(phone.Panels.IsModalOpen,Is.True);
            Assert.That(legal.enabledSelf,Is.False);Assert.That(support.enabledSelf,Is.False);Assert.That(phone.Button("settings-close").enabledSelf,Is.False);
            pending.SetResult(true);phone.Panels.Refresh();Assert.That(calls.Count,Is.EqualTo(1),"Completion cannot open another page or synthesize browser success.");
            Assert.That(phone.Frame.Q<Label>("settings-service-state").text,Is.EqualTo(phone.Locale.Resolve("settings.service.quiet")));
            Submit(support);Assert.That(calls,Is.EqualTo(new[]{ProductionServiceAction.ShowLegal,ProductionServiceAction.ContactSupport}));
            Submit(phone.Button("settings-close"));Assert.That(phone.Panels.IsModalOpen,Is.False);Submit(legal);Assert.That(calls.Count,Is.EqualTo(2),"A closed settings panel owns no contact intent.");
        }

        [UnityTest]
        public IEnumerator Contacts_ReadFailureAndActionFailureStayNeutral_NoRetryAndGameCanClose()
        {
            using var phone=new Phone();var readFailure=false;var calls=0;
            phone.Panels.BindServices(()=>readFailure?throw new InvalidOperationException("CONTROLLED_CONTACT_READ"):State(true,true),_=>{calls++;throw new InvalidOperationException("CONTROLLED_BROWSER_UNAVAILABLE");});
            yield return null;Submit(phone.Button("profile-settings"));readFailure=true;Submit(phone.Button("settings-legal"));
            Assert.That(calls,Is.Zero);Assert.That(phone.Button("settings-legal").enabledSelf,Is.False);Assert.That(phone.Button("settings-support").enabledSelf,Is.False);
            readFailure=false;phone.Panels.Refresh();Submit(phone.Button("settings-support"));phone.Panels.Refresh();
            Assert.That(calls,Is.EqualTo(1));Assert.That(phone.Frame.Q<Label>("settings-service-state").text,Is.EqualTo(phone.Locale.Resolve("settings.service.failed")));
            Assert.That(phone.Button("settings-close").enabledSelf,Is.True);Submit(phone.Button("settings-close"));Assert.That(phone.Panels.IsModalOpen,Is.False);Assert.That(calls,Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator Contacts_DisposedPanelCannotAct_AndLateFailureDoesNotChangeDetachedStatus()
        {
            using var phone=new Phone();var calls=0;var pending=new TaskCompletionSource<bool>();
            phone.Panels.BindServices(()=>State(true,true),_=>{calls++;return pending.Task;});yield return null;Submit(phone.Button("profile-settings"));Submit(phone.Button("settings-legal"));
            var status=phone.Frame.Q<Label>("settings-service-state");var before=status.text;phone.Panels.Dispose();pending.SetException(new InvalidOperationException("CONTROLLED_LATE_CONTACT_FAILURE"));
            phone.Panels.Refresh();Submit(phone.Button("settings-support"));yield return null;
            Assert.That(calls,Is.EqualTo(1));Assert.That(status.text,Is.EqualTo(before));
        }
#endif
    }
}
