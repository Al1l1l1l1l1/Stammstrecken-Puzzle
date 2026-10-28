using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        // The following input is a layout/format-preservation fixture, never license or legal material for shipping.
        private static string NoticeTestInput()=>"PRIVATE UI TEST INPUT ONLY\n"+new string('x',4095-"PRIVATE UI TEST INPUT ONLY\n".Length)+"🚂\n{Copyright Holder} <b>literal text</b>\n"+
            string.Join("\n\n",System.Linq.Enumerable.Repeat("Dieser Absatz ist ausschließlich ein UI-Testtext. Zeichen bleiben wörtlich erhalten; das Lesen löst keine externe Aktion aus.",240));

        [Test]
        public void OfflineNotices_OneLiteralBindingPreservesEveryCharacter_AndAnUnboundViewCannotOpen()
        {
            using var phone=new Phone();var entry=phone.Button("settings-notices");Submit(phone.Button("profile-settings"));
            Assert.That(entry.enabledSelf,Is.False);Submit(entry);Assert.That(phone.Panels.IsOfflineNoticesOpen,Is.False);
            string original=NoticeTestInput();phone.Panels.BindOfflineNotices(original);
            Submit(entry);var rendered=new List<string>();int pages=0;
            do
            {
                var blocks=phone.Frame.Q("notices-content").Query<Label>(className:"notices-copy").ToList();
                Assert.That(blocks.Count,Is.InRange(1,6));pages++;
                foreach(var block in blocks)
                {
                    Assert.That(block.text.Length,Is.LessThanOrEqualTo(4096));Assert.That(block.enableRichText,Is.False);
                    Assert.That(char.IsLowSurrogate(block.text[0]),Is.False);Assert.That(char.IsHighSurrogate(block.text[block.text.Length-1]),Is.False);rendered.Add(block.text);
                }
                if(!phone.Button("notices-next").enabledSelf)break;Submit(phone.Button("notices-next"));
            }while(pages<1000);
            Assert.That(pages,Is.GreaterThan(1));Assert.That(string.Concat(rendered),Is.EqualTo(original));
            Assert.Throws<InvalidOperationException>(()=>phone.Panels.BindOfflineNotices("replacement"));
            Assert.That(phone.Button("notices-next").enabledSelf,Is.False);Submit(phone.Button("notices-close"));
            Assert.That(phone.Frame.Q("notices-content").childCount,Is.Zero,"Closing releases the currently rendered text blocks.");
            AssertPackagedFullNoticePagesAreExact();
        }

        [UnityTest]
        public IEnumerator OfflineNotices_OpenReadCloseAndLargeFontNeverRequestServicesOrProfileActions()
        {
            using var phone=new Phone();var services=new List<ProductionServiceAction>();var daily=0;var local=0;
            phone.Panels.BindServices(()=>new ProductionServiceState(true,true,false,true,"1,99 €",true,true,true),a=>{services.Add(a);return Task.CompletedTask;});
            phone.Panels.BindDaily(()=>new DailyView("Datum",true,true,null,false),_=>{daily++;return Task.CompletedTask;});
            phone.Panels.BindLocalDataDeletion(()=>new ProductionLocalDataView(ProductionLocalDataState.Ready),_=>{local++;return Task.CompletedTask;});
            phone.Panels.BindOfflineNotices(NoticeTestInput());yield return null;Submit(phone.Button("profile-settings"));
            var settings=phone.Frame.Q("settings-overlay");settings.Q<ScrollView>().ScrollTo(phone.Button("settings-notices"));yield return null;
            Assert.That(phone.Button("settings-notices").worldBound.height,Is.GreaterThanOrEqualTo(44));
            Submit(phone.Button("settings-notices"));yield return null;yield return null;
            Assert.That(phone.Panels.IsOfflineNoticesOpen,Is.True);Assert.That(phone.Panels.IsModalOpen,Is.True);
            Assert.That(settings.style.display.value,Is.EqualTo(DisplayStyle.None));
            Assert.That(phone.Frame.Q("notices-overlay").style.display.value,Is.EqualTo(DisplayStyle.Flex));
            foreach(string name in new[]{"settings-legal","settings-support","settings-privacy","settings-restore","settings-remove-ads","settings-delete-local","daily-free","daily-video","home-daily","settings-close"})Submit(phone.Button(name));
            Submit(phone.Button("notices-font"));Submit(phone.Button("notices-next"));Submit(phone.Button("notices-previous"));phone.Frame.Q<ScrollView>("notices-scroll").scrollOffset=new Vector2(0,100);yield return null;
            Assert.That(phone.Panels.IsOfflineNoticesOpen,Is.True);Assert.That(services,Is.Empty);Assert.That(daily,Is.Zero);Assert.That(local,Is.Zero);
            Submit(phone.Button("notices-close"));yield return null;
            Assert.That(phone.Panels.IsOfflineNoticesOpen,Is.False);Assert.That(phone.Panels.IsModalOpen,Is.True,"The settings parent still owns the existing gameplay pause.");
            Assert.That(settings.style.display.value,Is.EqualTo(DisplayStyle.Flex));Submit(phone.Button("settings-close"));
            Assert.That(phone.Panels.IsModalOpen,Is.False);Submit(phone.Button("settings-notices"));Assert.That(phone.Panels.IsOfflineNoticesOpen,Is.False,"A stale hidden entry has no settings intent.");
            Assert.That(services,Is.Empty);Assert.That(daily+local,Is.Zero);
        }

        [UnityTest]
        public IEnumerator OfflineNotices_PendingNativeFocusAndFreshLocalDataFactsFenceEveryIntent()
        {
            using var phone=new Phone();var state=ProductionLocalDataState.Ready;var failedRead=false;var calls=0;var provider=new TaskCompletionSource<bool>();
            phone.Panels.BindLocalDataDeletion(()=>failedRead?throw new InvalidOperationException("TEST_LOCAL_DATA_READ"):new ProductionLocalDataView(state),_=>{calls++;return Task.CompletedTask;});
            phone.Panels.BindServices(()=>State(true,true),_=>provider.Task);phone.Panels.BindOfflineNotices(NoticeTestInput());yield return null;
            Submit(phone.Button("profile-settings"));Submit(phone.Button("settings-legal"));Submit(phone.Button("settings-notices"));
            Assert.That(phone.Panels.IsOfflineNoticesOpen,Is.False,"A pending external contact retains its existing fence.");
            provider.SetResult(true);phone.Panels.Refresh();phone.Panels.SetExternalSuspended(true);Submit(phone.Button("settings-notices"));Assert.That(phone.Panels.IsOfflineNoticesOpen,Is.False);
            phone.Panels.SetExternalSuspended(false);Submit(phone.Button("settings-notices"));yield return null;
            string font=phone.Button("notices-font").text;phone.Panels.SetExternalSuspended(true);Submit(phone.Button("notices-close"));Submit(phone.Button("notices-font"));
            Assert.That(phone.Panels.IsOfflineNoticesOpen,Is.True);Assert.That(phone.Button("notices-font").text,Is.EqualTo(font));
            phone.Panels.SetExternalSuspended(false);state=ProductionLocalDataState.Confirmation;phone.Panels.Refresh();
            Assert.That(phone.Panels.IsOfflineNoticesOpen,Is.False);Assert.That(phone.Frame.Q("local-data-overlay").style.display.value,Is.EqualTo(DisplayStyle.Flex));
            Submit(phone.Button("settings-notices"));Submit(phone.Button("notices-close"));Submit(phone.Button("notices-font"));Assert.That(calls,Is.Zero);
            state=ProductionLocalDataState.Ready;phone.Panels.Refresh();Submit(phone.Button("settings-notices"));failedRead=true;Submit(phone.Button("notices-close"));
            Assert.That(phone.Panels.IsOfflineNoticesOpen,Is.True);Assert.That(calls,Is.Zero);failedRead=false;Submit(phone.Button("notices-close"));Assert.That(phone.Panels.IsOfflineNoticesOpen,Is.False);
            phone.Panels.Dispose();Submit(phone.Button("settings-notices"));Assert.That(phone.Panels.IsOfflineNoticesOpen,Is.False);
        }

        [UnityTest]
        public IEnumerator OfflineNotices_390By844_AllTextRemainsReachableAt22Px_AndNativeScopeExcludesTheGame()
        {
            using var phone=new Phone();phone.Panels.BindOfflineNotices(NoticeTestInput());yield return null;
            Submit(phone.Button("profile-settings"));Submit(phone.Button("settings-notices"));yield return null;yield return null;
            var scroll=phone.Frame.Q<ScrollView>("notices-scroll");var close=phone.Button("notices-close");var font=phone.Button("notices-font");
            var closeBefore=close.worldBound;var fontBefore=font.worldBound;
            AssertNoticeControlInsidePhone(close);AssertNoticeControlInsidePhone(font);
            Assert.That(scroll.contentViewport.worldBound.height,Is.GreaterThan(100));Assert.That(scroll.verticalScroller.highValue,Is.GreaterThan(0));
            while(phone.Button("notices-next").enabledSelf){Submit(phone.Button("notices-next"));yield return null;}
            yield return null;yield return null;
            var last=phone.Frame.Q("notices-content").Query<Label>(className:"notices-copy").ToList().Last();
            scroll.scrollOffset=new Vector2(0,scroll.verticalScroller.highValue);yield return null;yield return null;
            Assert.That(last.worldBound.yMax,Is.LessThanOrEqualTo(scroll.contentViewport.worldBound.yMax+1));
            using var semantics=new ProductionGameAccessibility(phone.Frame,(key,args)=>phone.Locale.Resolve(key,args),false);
            semantics.RefreshForInspection();Assert.That(semantics.TryActivate(phone.Button("profile-settings")),Is.False);
            Assert.That(semantics.TryActivate(font),Is.True);yield return null;yield return null;
            Assert.That(last.resolvedStyle.fontSize,Is.EqualTo(22));Assert.That(close.worldBound,Is.EqualTo(closeBefore));Assert.That(font.worldBound,Is.EqualTo(fontBefore));
            AssertNoticeControlInsidePhone(close);AssertNoticeControlInsidePhone(font);
            scroll.scrollOffset=new Vector2(0,scroll.verticalScroller.highValue);yield return null;yield return null;
            Assert.That(last.worldBound.yMax,Is.LessThanOrEqualTo(scroll.contentViewport.worldBound.yMax+1));
            foreach(var block in phone.Frame.Q("notices-content").Query<Label>(className:"notices-copy").ToList())
            {Assert.That(block.resolvedStyle.whiteSpace,Is.EqualTo(WhiteSpace.Normal));Assert.That(block.isElided,Is.False);}
            semantics.SetSuspended(true);Assert.That(semantics.TryActivate(close),Is.False);semantics.SetSuspended(false);
            Assert.That(semantics.TryActivate(close),Is.True);Assert.That(phone.Panels.IsOfflineNoticesOpen,Is.False);
        }
        [UnityTest]
        public IEnumerator OfflineNotices_KeyboardFocusStaysWithinReader_AndEscapeReturnsToTheSettingsEntry()
        {
            using var phone=new Phone();phone.Panels.BindOfflineNotices(NoticeTestInput());yield return null;
            Submit(phone.Button("profile-settings"));phone.Frame.Q("settings-overlay").Q<ScrollView>().ScrollTo(phone.Button("settings-notices"));yield return null;
            Submit(phone.Button("settings-notices"));yield return null;yield return null;
            var close=phone.Button("notices-close");var font=phone.Button("notices-font");var scroll=phone.Frame.Q<ScrollView>("notices-scroll");
            Assert.That(phone.Frame.panel.focusController.focusedElement,Is.SameAs(close));
            NoticeKey(close,KeyCode.Tab);Assert.That(phone.Frame.panel.focusController.focusedElement,Is.SameAs(scroll));
            var next=phone.Button("notices-next");Assert.That(next.enabledSelf,Is.True);
            NoticeKey(scroll,KeyCode.Tab);Assert.That(phone.Frame.panel.focusController.focusedElement,Is.SameAs(next));
            NoticeKey(next,KeyCode.Tab);Assert.That(phone.Frame.panel.focusController.focusedElement,Is.SameAs(font));
            NoticeKey(font,KeyCode.Tab);Assert.That(phone.Frame.panel.focusController.focusedElement,Is.SameAs(close));
            NoticeKey(close,KeyCode.Tab,true);Assert.That(phone.Frame.panel.focusController.focusedElement,Is.SameAs(font));
            NoticeKey(font,KeyCode.Escape);yield return null;yield return null;
            Assert.That(phone.Panels.IsOfflineNoticesOpen,Is.False);Assert.That(phone.Panels.IsModalOpen,Is.True);
            Assert.That(phone.Frame.panel.focusController.focusedElement,Is.SameAs(phone.Button("settings-notices")));
            yield return AssertStaleCosmeticCallbackUsesTheFreshUiFence();
            yield return AssertActualSavedRetrySupersedesAnOpenReader();
        }
        private static void AssertPackagedFullNoticePagesAreExact()
        {
            var original=AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"ThirdPartyNotices.txt");Assert.That(original,Is.Not.Null,"The actual shipped notice source is required, never a shorter substitute.");
            using var phone=new Phone();phone.Panels.BindOfflineNotices(original!.text);Submit(phone.Button("profile-settings"));Submit(phone.Button("settings-notices"));
            var pages=new List<string>();
            do
            {
                var blocks=phone.Frame.Q("notices-content").Query<Label>(className:"notices-copy").ToList();Assert.That(blocks.Count,Is.InRange(1,6));
                pages.Add(string.Concat(blocks.Select(x=>x.text)));
                foreach(var block in blocks){Assert.That(block.text.Length,Is.LessThanOrEqualTo(4096));Assert.That(block.enableRichText,Is.False);}
                if(!phone.Button("notices-next").enabledSelf)break;Submit(phone.Button("notices-next"));
                Assert.That(pages.Count,Is.LessThan(10000));
            }while(true);
            Assert.That(string.Concat(pages),Is.EqualTo(original.text));
            for(int index=pages.Count-1;index>0;index--)
            {
                Submit(phone.Button("notices-previous"));Assert.That(string.Concat(phone.Frame.Q("notices-content").Query<Label>(className:"notices-copy").ToList().Select(x=>x.text)),Is.EqualTo(pages[index-1]));
            }
            Assert.That(phone.Button("notices-previous").enabledSelf,Is.False);
        }
        // Controlled still-true Root readiness isolates the immediate UI callback fence. This is not a normal-device defect reproduction or a purchase test.
        private static IEnumerator AssertStaleCosmeticCallbackUsesTheFreshUiFence()
        {
            var frame=AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Ui+"ProductionGame.uxml").CloneTree();
            using var presenter=new ProductionGamePresenter(frame,AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"ProductionGameLocale.json"),()=>0);
            presenter.BindAssetCatalog(AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"RailwayAssets.json"),true);
            var app=AppDomain.CurrentDomain.GetAssemblies().Single(a=>a.GetName().Name=="STP.Application");var content=AppDomain.CurrentDomain.GetAssemblies().Single(a=>a.GetName().Name=="STP.Infrastructure.Content");
            var source=AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"MobileCandidate.cosmetics.json");var binding=AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"MobileCandidate.bindings.json");
            var hash=System.Text.RegularExpressions.Regex.Match(binding.text,"\"cosmeticsHashSha256\"\\s*:\\s*\"([a-f0-9]{64})\"").Groups[1].Value;
            var catalog=Activator.CreateInstance(content.GetType("STP.Infrastructure.Content.LockedCosmeticsCatalog")!,source.bytes,hash,Enum.Parse(app.GetType("STP.Application.CatalogUse")!,"InternalPreview"),Activator.CreateInstance(content.GetType("STP.Infrastructure.Content.CanonicalJsonAdapter")!))!;
            int calls=0;
            typeof(ProductionGamePresenter).GetMethod("BindCosmetics")!.Invoke(presenter,new object[]{catalog,(Func<string,bool>)(_=>false),(Func<string,bool>)(_=>false),(Func<string,bool>)(_=>true),(Func<string,Task>)(_=>{calls++;return Task.CompletedTask;})});
            presenter.BindOfflineNotices("PRIVATE UI INTENT TEST ONLY");
            var method=typeof(ProductionGamePresenter).GetMethod("RequestCosmetic",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)!;
            void StaleCallback(){try{method.Invoke(presenter,new object[]{"obj-wartebank-ohne-anschluss"});}catch(System.Reflection.TargetInvocationException error){throw error.InnerException!;}}
            var owner=new GameObject("FreshUnderlyingIntentFence");var panel=ScriptableObject.CreateInstance<PanelSettings>();owner.AddComponent<UIDocument>().panelSettings=panel;owner.GetComponent<UIDocument>().rootVisualElement.Add(frame);
            try
            {
                yield return null;
                StaleCallback();Assert.That(calls,Is.EqualTo(1),"Unbound historical construction is not closed merely because no LocalData port exists.");
                var state=ProductionLocalDataState.Ready;bool failRead=false;
                presenter.BindLocalDataDeletion(()=>failRead?throw new InvalidOperationException("CONTROLLED_FRESH_ROOT_READ"):new ProductionLocalDataView(state),_=>Task.CompletedTask);
                Submit(frame.Q<Button>("profile-settings"));Submit(frame.Q<Button>("settings-notices"));
                StaleCallback();Assert.That(calls,Is.EqualTo(1),"An immediate open-reader epoch must reject an old dynamic callback even while controlled Root readiness is still true.");
                Submit(frame.Q<Button>("notices-close"));StaleCallback();Assert.That(calls,Is.EqualTo(1),"The still-open settings parent owns attention.");
                Submit(frame.Q<Button>("settings-close"));StaleCallback();Assert.That(calls,Is.EqualTo(2),"Ordinary readiness after close remains usable.");
                presenter.SetAccessibilitySuspended(true);StaleCallback();Assert.That(calls,Is.EqualTo(2));presenter.SetAccessibilitySuspended(false);
                state=ProductionLocalDataState.Confirmation;StaleCallback();Assert.That(calls,Is.EqualTo(2),"The callback reads fresh Root lifecycle facts without waiting for a presenter refresh.");
                state=ProductionLocalDataState.Ready;StaleCallback();Assert.That(calls,Is.EqualTo(3));
                failRead=true;StaleCallback();Assert.That(calls,Is.EqualTo(3));failRead=false;StaleCallback();Assert.That(calls,Is.EqualTo(4));
            }
            finally{UnityEngine.Object.Destroy(owner);UnityEngine.Object.Destroy(panel);}
        }

        // Actual Saved bridge and actual candidate catalogs over a controlled in-memory repository.
        // No SavedReviewState is constructed or patched; the retry flag is produced by a known noncommitting save failure.
        private sealed class RecoveryUiFixture:IDisposable
        {
            private readonly System.Reflection.Assembly app=AppDomain.CurrentDomain.GetAssemblies().Single(a=>a.GetName().Name=="STP.Application");
            private readonly System.Reflection.Assembly content=AppDomain.CurrentDomain.GetAssemblies().Single(a=>a.GetName().Name=="STP.Infrastructure.Content");
            private readonly System.Reflection.Assembly persistence=AppDomain.CurrentDomain.GetAssemblies().Single(a=>a.GetName().Name=="STP.Infrastructure.Persistence");
            internal readonly object Session;
            internal bool FailCommit;
            internal int RetryCalls,OtherRequests;
            internal Task LastRequest=Task.CompletedTask;
            internal readonly object Cosmetics;
            internal long Now=100;
            private object snapshot=null!;
            private readonly Type payload,loadResult,commitResult;
            private Type T(string name)=>app.GetType("STP.Application."+name)!;
            private object E(string type,string value)=>Enum.Parse(T(type),value);
            private object New(string name,params object?[] args)=>Activator.CreateInstance(T(name),args)!;
            internal static object Get(object value,string key)=>value.GetType().GetProperty(key)!.GetValue(value)!;
            private static object Static(Type type,string method,params object?[] args)=>type.GetMethods().Single(m=>m.Name==method&&m.GetParameters().Length==args.Length).Invoke(null,args)!;
            private static object Ready(object result)=>typeof(Task).GetMethod("FromResult")!.MakeGenericMethod(result.GetType()).Invoke(null,new[]{result})!;
            internal Task Call(string method,params object?[] args)=>(Task)Session.GetType().GetMethod(method)!.Invoke(Session,args)!;
            internal bool CanRetry=>(bool)Get(Get(Session,"State"),"CanRetrySave");
            internal RecoveryUiFixture()
            {
                payload=T("LocalProfileState");loadResult=T("SaveLoadResult`1").MakeGenericType(payload);commitResult=T("SaveCommitResult`1").MakeGenericType(payload);
                var binding=AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"MobileCandidate.bindings.json").text;
                string Hash(string key)=>System.Text.RegularExpressions.Regex.Match(binding,"\""+key+"\"\\s*:\\s*\"([a-f0-9]{64})\"").Groups[1].Value;
                byte[] Bytes(string file)=>AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+file).bytes;
                var json=Activator.CreateInstance(content.GetType("STP.Infrastructure.Content.CanonicalJsonAdapter")!)!;
                Cosmetics=Activator.CreateInstance(content.GetType("STP.Infrastructure.Content.LockedCosmeticsCatalog")!,Bytes("MobileCandidate.cosmetics.json"),Hash("cosmeticsHashSha256"),E("CatalogUse","InternalPreview"),json)!;
                var catalog=Activator.CreateInstance(content.GetType("STP.Infrastructure.Content.MobileCandidateData")!,Bytes("MobileCandidate.public.json"),Hash("publicCatalogHashSha256"))!;
                var progress=Activator.CreateInstance(content.GetType("STP.Infrastructure.Content.MobileCandidateProgressCatalog")!,catalog,Bytes("MobileCandidate.campaign.json"),Bytes("MobileCandidate.completion.json"),json,Hash("campaignHashSha256"),Hash("completionHashSha256"))!;
                var profile=New("SaveProfileIdentity","qa-mobile-candidate","candidate",E("CatalogUse","InternalPreview"));var catalogs=Get(progress,"CatalogSet");var aggregate=Get(catalogs,"AggregateCatalog");
                var ledger=Activator.CreateInstance(persistence.GetType("STP.Infrastructure.Persistence.SaveLedgerCodec")!,json)!;
                var policy=New("LocalProgressPolicy",catalog,Get(progress,"Campaign"),Get(progress,"Completion"),json,ledger);
                var initial=policy.GetType().GetMethod("NewProfile")!.Invoke(policy,new[]{profile,catalogs,"2026-10-27T12:00:00Z"})!;
                snapshot=Activator.CreateInstance(T("SaveSnapshot`1").MakeGenericType(payload),profile,aggregate,1L,initial)!;
                var repo=HumanContractProxy.Create(T("ILocalProfileRepository"),(method,args)=>
                {
                    if(method.Name=="LoadAsync")
                    {
                        var candidates=Array.CreateInstance(T("SaveCandidateProvenance"),3);
                        candidates.SetValue(New("SaveCandidateProvenance",E("SaveCandidateKind","Main"),E("SaveCandidateStatus","Valid"),(long)Get(snapshot,"Generation"),new string('a',64),new string('b',64)),0);
                        candidates.SetValue(New("SaveCandidateProvenance",E("SaveCandidateKind","Backup"),E("SaveCandidateStatus","Missing"),null,null,null),1);
                        candidates.SetValue(New("SaveCandidateProvenance",E("SaveCandidateKind","Pending"),E("SaveCandidateStatus","Missing"),null,null,null),2);
                        return Ready(Static(loadResult,"Success",snapshot,New("SaveRepositoryObservation",profile,aggregate,candidates)));
                    }
                    if(method.Name!="CommitAsync")throw new NotSupportedException(method.Name);
                    if(FailCommit)return Ready(Static(commitResult,"Failure",E("SaveCommitStatus","Failed"),"CONTROLLED_NONCOMMITTING_UI_RECOVERY"));
                    snapshot=args[0]!;return Ready(Static(commitResult,"Success",snapshot));
                });
                var clock=HumanContractProxy.Create(T("ILocalSessionClock"),(method,args)=>method.Name=="get_MonotonicMilliseconds"?(object)Now:"2026-10-27T12:00:00Z");
                var context=System.Threading.SynchronizationContext.Current??throw new InvalidOperationException("UNITY_TEST_OWNER_CONTEXT_REQUIRED");var owner=System.Threading.Thread.CurrentThread.ManagedThreadId;
                var dispatcher=HumanContractProxy.Create(T("ILocalSaveDispatcher"),(method,args)=>
                {
                    var action=(Action)args[0]!;if(System.Threading.Thread.CurrentThread.ManagedThreadId==owner){action();return Task.CompletedTask;}
                    var done=new TaskCompletionSource<bool>();context.Post(_=>{try{action();done.SetResult(true);}catch(Exception e){done.SetException(e);}},null);return done.Task;
                });
                var use=New("PersistedPuzzleSession",repo,catalog,policy,profile,catalogs,clock,dispatcher);
                Session=T("SavedPublic240ReviewSession").GetMethod("ForMobileCandidate")!.Invoke(null,new[]{use,New("AutosaveCoordinator",use),catalog,clock,dispatcher,profile})!;
            }
            internal void Dirty()
            {
                var view=Get(Session,"PuzzleView");var apply=T("IReviewPuzzleSession").GetMethods().Single(m=>m.Name=="Apply"&&m.GetParameters().Length==4);
                Assert.That(apply.Invoke(view,new[]{(object)0,0,E("DevelopmentTool","EmptyMarker"),++Now})!.ToString(),Is.EqualTo("Applied"));
            }
            internal ProductionGamePresenter Presenter(VisualElement frame)
            {
                var ctor=typeof(ProductionGamePresenter).GetConstructors().Single(c=>c.GetParameters().Length==4);var requestType=ctor.GetParameters()[3].ParameterType;
                var callback=Delegate.CreateDelegate(requestType,this,GetType().GetMethod(nameof(Request),System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)!.MakeGenericMethod(requestType.GetGenericArguments()[0]));
                var presenter=(ProductionGamePresenter)ctor.Invoke(new object[]{frame,AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"ProductionGameLocale.json"),(Func<long>)(()=>Now),callback});
                typeof(ProductionGamePresenter).GetMethod("BindModel")!.Invoke(presenter,new[]{Session});return presenter;
            }
            private Task Request<TRequest>(TRequest request)
            {
                var kind=Get(request!,"Kind").ToString();if(kind!="RetrySave"){OtherRequests++;return Task.CompletedTask;}
                RetryCalls++;return LastRequest=Call("RetrySaveAsync",System.Threading.CancellationToken.None);
            }
            public void Dispose()=>((IDisposable)Session).Dispose();
        }
        private static IEnumerator AwaitRecoveryUi(Task task)
        {
            var until=Time.realtimeSinceStartup+10;while(!task.IsCompleted){Assert.That(Time.realtimeSinceStartup,Is.LessThan(until),"Actual saved bridge owner dispatch timed out.");yield return null;}task.GetAwaiter().GetResult();
        }
        private static IEnumerator AssertActualSavedRetrySupersedesAnOpenReader()
        {
            using var saved=new RecoveryUiFixture();yield return AwaitRecoveryUi(saved.Call("InitializeAsync",System.Threading.CancellationToken.None));yield return AwaitRecoveryUi(saved.Call("SelectPuzzleAsync",0,false,System.Threading.CancellationToken.None));
            var frame=AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Ui+"ProductionGame.uxml").CloneTree();using var presenter=saved.Presenter(frame);presenter.BindOfflineNotices("PRIVATE RECOVERY UI TEST ONLY");
            int cosmetics=0;typeof(ProductionGamePresenter).GetMethod("BindCosmetics")!.Invoke(presenter,new object[]{saved.Cosmetics,(Func<string,bool>)(_=>false),(Func<string,bool>)(_=>false),(Func<string,bool>)(_=>true),(Func<string,Task>)(_=>{cosmetics++;return Task.CompletedTask;})});
            var cosmeticMethod=typeof(ProductionGamePresenter).GetMethod("RequestCosmetic",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)!;
            void StaleCosmetic()=>cosmeticMethod.Invoke(presenter,new object[]{"obj-wartebank-ohne-anschluss"});
            var state=ProductionLocalDataState.Ready;bool failRead=false;presenter.BindLocalDataDeletion(()=>failRead?throw new InvalidOperationException("CONTROLLED_FRESH_ROOT_READ"):new ProductionLocalDataView(state),_=>Task.CompletedTask);
            var host=new GameObject("ActualSavedRecoveryOverReader");var panel=ScriptableObject.CreateInstance<PanelSettings>();host.AddComponent<UIDocument>().panelSettings=panel;host.GetComponent<UIDocument>().rootVisualElement.Add(frame);
            try
            {
                yield return null;StaleCosmetic();Assert.That(cosmetics,Is.EqualTo(1));Submit(frame.Q<Button>("profile-settings"));Submit(frame.Q<Button>("settings-notices"));Assert.That(presenter.IsProductModalOpen,Is.True);
                saved.Dirty();saved.FailCommit=true;yield return AwaitRecoveryUi(saved.Call("SetSuspendedAsync",true,System.Threading.CancellationToken.None));Assert.That(saved.CanRetry,Is.True);
                presenter.Refresh();yield return null;Assert.That(frame.Q("error-overlay").resolvedStyle.display,Is.EqualTo(DisplayStyle.Flex));Assert.That(frame.Q("settings-overlay").resolvedStyle.display,Is.EqualTo(DisplayStyle.None));Assert.That(frame.Q("notices-overlay").resolvedStyle.display,Is.EqualTo(DisplayStyle.None));Assert.That(frame.Q("notices-content").childCount,Is.Zero);
                var retry=frame.Q<Button>("error-retry");Assert.That(retry.resolvedStyle.display,Is.EqualTo(DisplayStyle.Flex));Assert.That(retry.enabledInHierarchy,Is.True);yield return null;Assert.That(frame.panel.focusController.focusedElement,Is.SameAs(retry));
                Submit(frame.Q<Button>("profile-settings"));Assert.That(presenter.IsProductModalOpen,Is.False,"A lower ordinary panel cannot reclaim attention from a saved-state error.");Submit(frame.Q<Button>("home-play"));Assert.That(saved.OtherRequests,Is.Zero);
                StaleCosmetic();Assert.That(cosmetics,Is.EqualTo(1),"The higher error still rejects background cosmetic callbacks even after the reader has relinquished attention.");
                presenter.SetAccessibilitySuspended(true);Submit(retry);Assert.That(saved.RetryCalls,Is.Zero);presenter.SetAccessibilitySuspended(false);
                state=ProductionLocalDataState.Confirmation;Submit(retry);Assert.That(saved.RetryCalls,Is.Zero);state=ProductionLocalDataState.Ready;
                failRead=true;Submit(retry);Assert.That(saved.RetryCalls,Is.Zero);failRead=false;
                saved.FailCommit=false;Submit(retry);Assert.That(saved.RetryCalls,Is.EqualTo(1),"The actual visible Retry callback must reach the saved bridge even after the reader owned attention.");
                yield return AwaitRecoveryUi(saved.LastRequest);presenter.Refresh();yield return null;Assert.That(saved.CanRetry,Is.False);Assert.That(frame.Q("error-overlay").resolvedStyle.display,Is.EqualTo(DisplayStyle.None));
                Submit(retry);Assert.That(saved.RetryCalls,Is.EqualTo(1),"A stale recovery callback cannot act after its higher error has cleared.");Submit(frame.Q<Button>("profile-settings"));Assert.That(presenter.IsProductModalOpen,Is.True,"Ordinary settings remain available after the successful explicit retry.");
            }
            finally{UnityEngine.Object.Destroy(host);UnityEngine.Object.Destroy(panel);}
        }

        private static void NoticeKey(VisualElement target,KeyCode key,bool shift=false)
        {
            using var change=KeyDownEvent.GetPooled(new Event{type=EventType.KeyDown,keyCode=key,modifiers=shift?EventModifiers.Shift:EventModifiers.None});
            change.target=target;target.SendEvent(change);
        }
        private static void AssertNoticeControlInsidePhone(Button button)
        {
            var bounds=button.worldBound;Assert.That(bounds.height,Is.GreaterThanOrEqualTo(44));Assert.That(bounds.xMin,Is.GreaterThanOrEqualTo(0));Assert.That(bounds.xMax,Is.LessThanOrEqualTo(390));
            Assert.That(bounds.yMin,Is.GreaterThanOrEqualTo(0));Assert.That(bounds.yMax,Is.LessThanOrEqualTo(844));
        }

#endif
    }
}
