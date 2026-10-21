using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Presentation.UI;
using STP.Presentation.World;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace STP.Tests.Presentation.PlayMode
{
    /// <summary>Product presentation contracts with real domain-backed public sessions and no new assembly edges.</summary>
    public sealed class ProductionGamePresentationTests
    {
#if UNITY_EDITOR
        private const string Ui="Assets/StammstreckenPuzzle/UI/";
        private static TextAsset Locale()=>AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"ProductionGameLocale.json");
        private static VisualElement Frame()=>AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Ui+"ProductionGame.uxml").CloneTree();

        [UnityTest]
        public IEnumerator HintGate_DetachedAndHiddenOffersAreNotAcknowledged_VisibleGeometryAcknowledgesOnce()
        {
            var overlay=new VisualElement();overlay.style.width=300;overlay.style.height=200;
            var message=new Label("Eine sichere einzelne Aussage.");message.style.width=260;message.style.height=50;overlay.Add(message);
            using var gate=new HintDisplayGate(overlay,message);var acknowledgements=0;
            gate.Arm(()=>acknowledgements++);yield return null;yield return null;
            Assert.That(acknowledgements,Is.Zero,"Detached help must not spend a credit.");
            overlay.style.display=DisplayStyle.None;
            var panel=ScriptableObject.CreateInstance<PanelSettings>();var go=new GameObject("HintGeometryContract");var document=go.AddComponent<UIDocument>();document.panelSettings=panel;document.rootVisualElement.Add(overlay);
            try
            {
                yield return null;yield return new WaitForSecondsRealtime(.04f);Assert.That(acknowledgements,Is.Zero,"Hidden help must not spend a credit.");
                overlay.style.display=DisplayStyle.Flex;yield return null;yield return new WaitForSecondsRealtime(.06f);
                Assert.That(acknowledgements,Is.EqualTo(1));Assert.That(message.worldBound.width,Is.GreaterThan(0));
                message.style.width=250;yield return new WaitForSecondsRealtime(.04f);Assert.That(acknowledgements,Is.EqualTo(1),"Further layout does not mint a second receipt.");
                gate.Arm(()=>acknowledgements++);gate.Cancel();yield return new WaitForSecondsRealtime(.04f);Assert.That(acknowledgements,Is.EqualTo(1));
            }
            finally{UnityEngine.Object.Destroy(go);UnityEngine.Object.Destroy(panel);}
        }

        [Test]
        public void HintPresentation_UsesTypedSingleFact_AndDoesNotExposeRuleCodesOrSolutionText()
        {
            using var locale=new ProductionGameLocalization(Locale());
            Assert.That(locale.Resolve("hint.fact",3,4,locale.Resolve("tool.empty")),Is.EqualTo("Spalte 3, Zeile 4: Leerannahme X"));
            Assert.That(locale.Resolve("hint.unspent"),Does.Contain("tatsächlich angezeigten"));
            Assert.That(locale.Resolve("hint.save.failed"),Does.Contain("sicher gespeichert"));
            var frame=Frame();using var presenter=new ProductionGamePresenter(frame,Locale(),()=>0,_=>Task.CompletedTask);
            Assert.That(presenter.IsHelpOpen,Is.False);Assert.That(frame.Q<VisualElement>("hint-overlay").ClassListContains("hidden"),Is.True);
        }

        [Test]
        public void AssetManifest_ResolvesAll480OriginalReferences_AndRequiresExplicitInternalDraft()
        {
            var source=AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"RailwayAssets.json");
            Assert.Throws<ArgumentException>(()=>new RailwayAssetCatalog(source));
            var catalog=new RailwayAssetCatalog(source,true);
            for(var section=1;section<=5;section++)for(var route=1;route<=4;route++)for(var position=1;position<=12;position++)
            {
                var puzzle=string.Format(System.Globalization.CultureInfo.InvariantCulture,"S1-{0:00}-{1:00}-{2:00}",section,route,position);
                var map=catalog.ResolvePuzzleMoment(puzzle,false);var train=catalog.ResolvePuzzleMoment(puzzle,true);
                Assert.That(map.kind,Is.EqualTo("MAP"));Assert.That(train.kind,Is.EqualTo("TRAIN"));
                Assert.That(map.stage,Is.EqualTo(position));Assert.That(catalog.ResolveMoment(map.referenceId),Is.SameAs(map));
                var composition=catalog.ResolveComposition(map.compositionId);Assert.That(composition.section,Is.EqualTo(section));Assert.That(composition.route,Is.EqualTo(route));
                Assert.That(train.compositionId,Is.EqualTo(map.compositionId));Assert.That(composition.modules.Length,Is.GreaterThanOrEqualTo(3));
            }
            var reusableMap=catalog.ResolveReusableMoment("draft-endless-map-default");var reusableTrain=catalog.ResolveReusableMoment("draft-endless-train-default");Assert.That(reusableMap.puzzleId,Is.Empty);Assert.That(reusableTrain.puzzleId,Is.Empty);Assert.That(reusableMap.kind,Is.EqualTo("MAP"));Assert.That(reusableTrain.kind,Is.EqualTo("TRAIN"));Assert.That(reusableMap.compositionId,Is.EqualTo("s1-route-05-04"));Assert.Throws<ArgumentException>(()=>catalog.ResolveMoment(reusableMap.referenceId));Assert.That(catalog.CosmeticAssets.Count,Is.EqualTo(14));
            foreach(var asset in catalog.CosmeticAssets){Assert.That(RailwayCosmeticElement.SupportsItem(asset.id),Is.True);Assert.That(catalog.ResolveCosmetic(asset.id),Is.SameAs(asset));}
            Assert.Throws<ArgumentException>(()=>catalog.ResolveMoment("missing"));
            using var presenter=new ProductionGamePresenter(Frame(),Locale(),()=>0,_=>Task.CompletedTask);
            presenter.BindAssetCatalog(source,true);Assert.That(presenter.Root.Q("depot-catalog").childCount,Is.EqualTo(14));
        }

        [Test]
        public void ProductFrame_HasNineDirectSymbolTools_AndNoAdministrationOrBatchControls()
        {
            var frame=Frame();using var presenter=new ProductionGamePresenter(frame,Locale(),()=>0,_=>Task.CompletedTask);
            var continuationCalls=0;presenter.BindResultContinuation(()=>{continuationCalls++;return Task.CompletedTask;});presenter.Refresh();presenter.Refresh();Assert.That(continuationCalls,Is.Zero);Assert.That(presenter.IsResultContinuationPending,Is.False);Assert.That(presenter.HasBoundModel,Is.False);Assert.That(presenter.Grid,Is.Null);
            Assert.That(frame.Q<DropdownField>(),Is.Null);
            foreach(var forbidden in new[]{"qa-multiselect","qa-apply-selection","saved-status","saved-picker","qa-boundary"})Assert.That(frame.Q(forbidden),Is.Null,forbidden);
            for(var i=0;i<9;i++)
            {
                var button=frame.Q<Button>("tool-"+i);Assert.That(button,Is.Not.Null);Assert.That(button.text,Is.Empty);
                Assert.That(button.tooltip,Is.Not.Empty);Assert.That(button.childCount,Is.GreaterThanOrEqualTo(1));
            }
            Assert.That(frame.Q<Label>("startup-message").text,Does.Contain("vorbereitet"));
            Assert.That(frame.Q<VisualElement>("tool-dock").parent,Is.SameAs(frame.Q<VisualElement>("puzzle-screen")));
        }

        [Test]
        public void ProductLocale_HasRealGermanText_TypedNumbers_AndNoGlobalMutation()
        {
            using var locale=new ProductionGameLocalization(Locale());
            Assert.That(locale.Resolve("balance",1234L),Is.EqualTo("1.234 GP"));
            Assert.That(locale.Resolve("time",4L,7L),Is.EqualTo("04:07"));
            Assert.That(locale.Resolve("tool.occupied"),Does.Contain("Belegungsnotiz"));
            Assert.Throws<ArgumentException>(()=>locale.Resolve("time",4L));
            Assert.Throws<ArgumentException>(()=>locale.Resolve("balance","1234"));
            Assert.Throws<InvalidOperationException>(()=>locale.Resolve("unknown"));
            locale.Dispose();Assert.Throws<ObjectDisposedException>(()=>locale.Resolve("brand"));
        }

        [UnityTest]
        public IEnumerator StartupRetry_ObservesSingleTask_AndNeverInventsBoundModel()
        {
            var frame=Frame();var tcs=new TaskCompletionSource<bool>();var calls=0;
            using var presenter=new ProductionGamePresenter(frame,Locale(),()=>0,_=>{calls++;return tcs.Task;});
            var panel=ScriptableObject.CreateInstance<PanelSettings>();var go=new GameObject("ProductStartupContract");
            var document=go.AddComponent<UIDocument>();document.panelSettings=panel;document.rootVisualElement.Add(frame);
            try
            {
                presenter.ShowStartupFailure(SavedStartupFailure.CompositionFailed);
                Submit(frame.Q<Button>("startup-retry"));Submit(frame.Q<Button>("startup-retry"));
                Assert.That(calls,Is.EqualTo(1));Assert.That(presenter.PendingRequest,Is.SameAs(tcs.Task));
                tcs.SetResult(true);presenter.Refresh();var continuationCalls=0;presenter.BindResultContinuation(()=>{continuationCalls++;return Task.CompletedTask;});presenter.Refresh();presenter.Refresh();Assert.That(continuationCalls,Is.Zero);Assert.That(presenter.IsResultContinuationPending,Is.False);Assert.That(presenter.HasBoundModel,Is.False);Assert.That(presenter.Grid,Is.Null);
                yield return null;
            }
            finally{UnityEngine.Object.Destroy(go);UnityEngine.Object.Destroy(panel);}
        }

        [UnityTest]
        public IEnumerator Grid_6x5_And10x10_KeepExactTargetsAcrossContentsAndUndo()
        {
            foreach(var shape in new[]{new Vector2Int(6,5),new Vector2Int(10,10)})
            {
                var session=Session(shape.x,shape.y,1,1,"W","E");
                using var locale=new ProductionGameLocalization(Locale());
                var ctor=typeof(ProductionPuzzleGrid).GetConstructors().Single();
                using var grid=(ProductionPuzzleGrid)ctor.Invoke(new object[]{session,(Func<string,object[],string>)locale.Resolve,(Action<int,int>)((x,y)=>{})});
                var panel=ScriptableObject.CreateInstance<PanelSettings>();var go=new GameObject("ProductStableGrid");
                var doc=go.AddComponent<UIDocument>();doc.panelSettings=panel;doc.rootVisualElement.Add(grid);
                grid.style.width=370;grid.style.height=420;yield return null;yield return null;
                try
                {
                    var targets=new Rect[shape.x*shape.y];
                    for(var y=0;y<shape.y;y++)for(var x=0;x<shape.x;x++)targets[y*shape.x+x]=grid.CellButton(x,y).worldBound;
                    for(var tool=1;tool<=8;tool++)
                    {
                        Apply(session,2,2,tool,1000+tool*100);grid.Refresh(true);yield return null;
                        for(var y=0;y<shape.y;y++)for(var x=0;x<shape.x;x++)Assert.That(grid.CellButton(x,y).worldBound,Is.EqualTo(targets[y*shape.x+x]));
                        session.GetType().GetMethod("Undo",new[]{typeof(long)})!.Invoke(session,new object[]{2000L+tool*100});grid.Refresh(true);yield return null;
                        Assert.That(grid.CellButton(2,2).worldBound,Is.EqualTo(targets[2*shape.x+2]));
                    }
                    Assert.That(grid.CellButton(0,0).focusable,Is.True);Assert.That(grid.CellButton(0,0).worldBound.width,Is.GreaterThan(shape.x==6?45:28));
                    Assert.That(grid.Board.Contains(grid.CellButton(0,0)),Is.True);
                }
                finally{UnityEngine.Object.Destroy(go);UnityEngine.Object.Destroy(panel);}
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator Grid_TwoDigitCountsAndExteriorPortsKeepSeparateLanes_InsideSafeArea_AcrossMutationAndPause()
        {
            var session=Session(10,10,1,2,"W","N",true);using var locale=new ProductionGameLocalization(Locale());
            using var grid=(ProductionPuzzleGrid)typeof(ProductionPuzzleGrid).GetConstructors().Single().Invoke(new object[]{session,(Func<string,object[],string>)locale.Resolve,(Action<int,int>)((_,__)=>{})});
            var root=new VisualElement();root.AddToClassList("production-root");root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(Ui+"ProductionGame.uss"));root.style.width=390;root.style.height=844;
            root.style.paddingLeft=12;root.style.paddingRight=12;root.style.paddingTop=28;root.style.paddingBottom=24;root.Add(grid);grid.style.width=Length.Percent(100);grid.style.height=420;
            var panel=ScriptableObject.CreateInstance<PanelSettings>();panel.themeStyleSheet=AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(Ui+"DevelopmentPuzzleTheme.tss");panel.scaleMode=PanelScaleMode.ConstantPixelSize;
            var target=new RenderTexture(390,844,24);target.Create();panel.targetTexture=target;var go=new GameObject("CountPortSafeAreaLanes");var document=go.AddComponent<UIDocument>();document.panelSettings=panel;document.rootVisualElement.Add(root);
            try
            {
                grid.Refresh(true);yield return null;yield return null;
                var row=grid.Q<Label>("puzzle-row-1");var column=grid.Q<Label>("puzzle-column-2");var leftPort=grid.Q<Label>("puzzle-endpoint-a");var topPort=grid.Q<Label>("puzzle-endpoint-b");
                Assert.That(row.text,Is.EqualTo("10"));Assert.That(column.text,Is.EqualTo("10"));Assert.That(row.worldBound.width,Is.GreaterThanOrEqualTo(24));Assert.That(column.worldBound.height,Is.GreaterThanOrEqualTo(24));
                Assert.That(leftPort.worldBound.xMin-row.worldBound.xMax,Is.GreaterThanOrEqualTo(5.9f));Assert.That(topPort.worldBound.yMin-column.worldBound.yMax,Is.GreaterThanOrEqualTo(5.9f));
                Assert.That(row.worldBound.Overlaps(leftPort.worldBound),Is.False);Assert.That(column.worldBound.Overlaps(topPort.worldBound),Is.False);
                Assert.That(grid.worldBound.xMin,Is.GreaterThanOrEqualTo(root.worldBound.xMin+12));Assert.That(grid.worldBound.yMin,Is.GreaterThanOrEqualTo(root.worldBound.yMin+28));
                foreach(var visual in new VisualElement[]{row,column,leftPort,topPort})
                {Assert.That(visual.worldBound.xMin,Is.GreaterThanOrEqualTo(grid.worldBound.xMin));Assert.That(visual.worldBound.yMin,Is.GreaterThanOrEqualTo(grid.worldBound.yMin));Assert.That(visual.worldBound.xMax,Is.LessThanOrEqualTo(grid.worldBound.xMax));Assert.That(visual.worldBound.yMax,Is.LessThanOrEqualTo(grid.worldBound.yMax));}
                var stable=new[]{grid.Board.worldBound,row.worldBound,column.worldBound,leftPort.worldBound,topPort.worldBound,grid.CellButton(3,3).worldBound};
                for(var tool=1;tool<=8;tool++)
                {
                    Apply(session,3,3,tool,1000+tool*100);grid.Refresh(true);yield return null;grid.Refresh(false);yield return null;grid.Refresh(true);
                    CollectionAssert.AreEqual(stable,new[]{grid.Board.worldBound,row.worldBound,column.worldBound,leftPort.worldBound,topPort.worldBound,grid.CellButton(3,3).worldBound});
                }
                session.GetType().GetMethod("Undo",new[]{typeof(long)})!.Invoke(session,new object[]{3000L});grid.Refresh(true);yield return null;
                CollectionAssert.AreEqual(stable,new[]{grid.Board.worldBound,row.worldBound,column.worldBound,leftPort.worldBound,topPort.worldBound,grid.CellButton(3,3).worldBound});
            }
            finally{panel.targetTexture=null;target.Release();UnityEngine.Object.Destroy(target);UnityEngine.Object.Destroy(go);UnityEngine.Object.Destroy(panel);}
        }

        [Test]
        public void Train_UsesContinuousQuarterArc_AndVisitsOnlyActualPlayerCells()
        {
            var session=Session(3,3,1,1,"W","N");
            var app=session.GetType().Assembly;var cellType=app.GetType("STP.Application.DevelopmentCell")!;
            var cells=Array.CreateInstance(cellType,3);
            cells.SetValue(Activator.CreateInstance(cellType,0,1),0);cells.SetValue(Activator.CreateInstance(cellType,1,1),1);cells.SetValue(Activator.CreateInstance(cellType,1,0),2);
            using var ride=(ProductionTrainRide)typeof(ProductionTrainRide).GetConstructors().Single().Invoke(new[]{(object)cells,Get(session,"EndpointA"),Get(session,"EndpointB"),3,3,1f});
            Assert.That(ride.Length,Is.EqualTo(3.1f+Mathf.PI*.25f).Within(.0001));
            ride.Tick(.65f+.55f+1f+.25f*Mathf.PI*.5f);
            var expectedBogieMidpoint=1f+.5f*Mathf.Cos(.34f)/Mathf.Sqrt(2);
            Assert.That(ride.X,Is.EqualTo(expectedBogieMidpoint).Within(.0001));Assert.That(ride.Y,Is.EqualTo(expectedBogieMidpoint).Within(.0001));
            var angle=ride.AngleDegrees;ride.Tick(.005f);Assert.That(Mathf.Abs(Mathf.DeltaAngle(angle,ride.AngleDegrees)),Is.LessThan(2));
            Assert.That(ride.IsFinished,Is.False);ride.Tick(10);Assert.That(ride.IsFinished,Is.True);
            var visited=(ICollection)typeof(ProductionTrainRide).GetProperty("VisitedCells")!.GetValue(ride)!;Assert.That(visited.Count,Is.EqualTo(3));
            Assert.That(ride.X,Is.EqualTo(1.5f).Within(.001));Assert.That(ride.Y,Is.LessThan(0));
        }

        [UnityTest]
        public IEnumerator DailyPanel_TaskCompletionIsNotAGrant_AndProviderFailureRemainsLocal()
        {
            var frame=Frame();using var locale=new ProductionGameLocalization(Locale());using var panels=new ProductionProductPanels(frame,locale);
            var state=new DailyView("21.10.2026",true,false,null,false);var completion=new TaskCompletionSource<bool>();var calls=0;
            panels.BindDaily(()=>state,rewarded=>{Assert.That(rewarded,Is.False);calls++;return completion.Task;});
            var panel=ScriptableObject.CreateInstance<PanelSettings>();var go=new GameObject("DailyProductContract");var document=go.AddComponent<UIDocument>();document.panelSettings=panel;document.rootVisualElement.Add(frame);
            try
            {
                yield return null;Submit(frame.Q<Button>("home-daily"));Assert.That(panels.IsModalOpen,Is.True);
                Submit(frame.Q<Button>("daily-free"));Submit(frame.Q<Button>("daily-free"));Assert.That(calls,Is.EqualTo(1));
                Assert.That(frame.Q<Button>("daily-close").enabledSelf,Is.False);Assert.That(frame.Q<Button>("daily-video").enabledSelf,Is.False);
                completion.SetResult(true);panels.Refresh();Assert.That(frame.Q<Label>("daily-result").text,Is.EqualTo(locale.Resolve("daily.ready")),"An operation completing is not proof of a durable grant.");
                state=new DailyView("21.10.2026",false,true,15,false);panels.Refresh();Assert.That(frame.Q<Label>("daily-result").text,Is.EqualTo("Bestätigt: 15 GP"));
                panels.BindDaily(()=>state,_=>Task.FromException(new InvalidOperationException("provider unavailable")));
                Submit(frame.Q<Button>("daily-video"));panels.Refresh();Assert.That(frame.Q<Label>("daily-result").text,Is.EqualTo("Bestätigt: 15 GP"));
                Assert.That(frame.Q<Label>("daily-state").text,Is.EqualTo(locale.Resolve("daily.failed")));Assert.That(frame.Q<Button>("daily-close").enabledSelf,Is.True);
                Submit(frame.Q<Button>("daily-close"));Assert.That(panels.IsModalOpen,Is.False);
            }
            finally{UnityEngine.Object.Destroy(go);UnityEngine.Object.Destroy(panel);}
        }

        [UnityTest]
        public IEnumerator SettingsStore_OpeningNeverCallsProvider_ExplicitPriceThenSeparatePurchase_BlockPendingDuplicatesAndStalePrice()
        {
            var frame=Frame();frame.style.width=390;frame.style.height=844;using var locale=new ProductionGameLocalization(Locale());using var panels=new ProductionProductPanels(frame,locale);
            var state=new ProductionServiceState(false,false,false,false,null,true);var calls=new System.Collections.Generic.List<ProductionServiceAction>();
            var priceLoad=new TaskCompletionSource<bool>();var purchase=new TaskCompletionSource<bool>();
            panels.BindServices(()=>state,action=>{calls.Add(action);return action==ProductionServiceAction.LoadStorePrice?priceLoad.Task:purchase.Task;});
            var panel=ScriptableObject.CreateInstance<PanelSettings>();panel.themeStyleSheet=AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(Ui+"DevelopmentPuzzleTheme.tss");panel.scaleMode=PanelScaleMode.ConstantPixelSize;
            var target=new RenderTexture(390,844,24);target.Create();panel.targetTexture=target;var go=new GameObject("ExplicitStorePriceThenPurchase");var document=go.AddComponent<UIDocument>();document.panelSettings=panel;document.rootVisualElement.Add(frame);
            frame.Q("startup-overlay").style.display=DisplayStyle.None;frame.Q("home-screen").style.display=DisplayStyle.None;frame.Q("profile-screen").style.display=DisplayStyle.Flex;
            try
            {
                yield return null;Submit(frame.Q<Button>("profile-settings"));panels.Refresh();yield return null;Assert.That(panels.IsModalOpen,Is.True);Assert.That(calls,Is.Empty,"Opening settings and reading readiness cannot fetch or purchase anything.");
                var button=frame.Q<Button>("settings-remove-ads");var scroll=frame.Q("settings-overlay").Q<ScrollView>();scroll.ScrollTo(button);yield return null;
                Assert.That(button.worldBound.height,Is.GreaterThanOrEqualTo(44));Assert.That(button.worldBound.yMin,Is.GreaterThanOrEqualTo(scroll.contentViewport.worldBound.yMin));Assert.That(button.worldBound.yMax,Is.LessThanOrEqualTo(scroll.contentViewport.worldBound.yMax+1));
                Assert.That(button.text,Is.EqualTo(locale.Resolve("settings.remove-ads.load-price")));Assert.That(button.enabledSelf,Is.True);Assert.That(calls,Is.Empty,"Scrolling to the Store controls does not start an SDK operation.");
                Submit(button);Submit(button);Assert.That(calls,Is.EqualTo(new[]{ProductionServiceAction.LoadStorePrice}));Assert.That(button.enabledSelf,Is.False);Assert.That(frame.Q<Button>("settings-close").enabledSelf,Is.False);
                state=new ProductionServiceState(false,false,false,true,"SDK-Preis A",true);priceLoad.SetResult(true);panels.Refresh();Assert.That(button.text,Is.EqualTo(locale.Resolve("settings.remove-ads.price","SDK-Preis A")));Assert.That(button.enabledSelf,Is.True);Assert.That(calls.Count,Is.EqualTo(1),"A fetched price is no purchase authorization.");
                state=new ProductionServiceState(false,false,false,true,"SDK-Preis B",true);Submit(button);Assert.That(calls.Count,Is.EqualTo(1),"A changed price must be displayed before another deliberate tap.");Assert.That(button.text,Is.EqualTo(locale.Resolve("settings.remove-ads.price","SDK-Preis B")));
                Submit(button);Submit(button);Assert.That(calls,Is.EqualTo(new[]{ProductionServiceAction.LoadStorePrice,ProductionServiceAction.PurchaseRemoveAds}));Assert.That(button.enabledSelf,Is.False);
                purchase.SetResult(true);panels.Refresh();Assert.That(button.text,Is.EqualTo(locale.Resolve("settings.remove-ads.price","SDK-Preis B")),"Task completion cannot synthesize ownership.");
                state=new ProductionServiceState(false,false,true,false,null,true);panels.Refresh();Assert.That(button.text,Is.EqualTo(locale.Resolve("settings.remove-ads.owned")));Assert.That(button.enabledSelf,Is.False);
                state=new ProductionServiceState(false,false,false,false,null);panels.Refresh();Assert.That(button.enabledSelf,Is.False);Assert.That(button.text,Is.EqualTo(locale.Resolve("settings.remove-ads.unavailable")),"Missing Store inputs remain unavailable without a fabricated price.");
            }
            finally{panel.targetTexture=null;target.Release();UnityEngine.Object.Destroy(target);UnityEngine.Object.Destroy(go);UnityEngine.Object.Destroy(panel);}
        }

        [Test]
        public void SettingsCopy_PreservesNullableAndUnknownFields_ChangesOnlyRequestedAudioGroup()
        {
            var app=AppDomain.CurrentDomain.GetAssemblies().Single(a=>a.GetName().Name=="STP.Application");var audio=app.GetType("STP.Application.LocalAudioPreference")!;
            var extra=audio.GetMethod("Create")!.Invoke(null,new object[]{"FutureGroup",1L,3L})!;var groups=Array.CreateInstance(audio,1);groups.SetValue(extra,0);
            var settings=app.GetType("STP.Application.LocalSettings")!.GetMethod("Create")!.Invoke(null,new object?[]{"de-DE",groups,null,null})!;
            var copy=typeof(ProductionProductPanels).GetMethod("Copy",BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,new object?[]{settings,"UI",0,null,null,null})!;
            Assert.That(Get(copy,"Locale"),Is.EqualTo("de-DE"));Assert.That(copy.GetType().GetProperty("HapticsEnabled")!.GetValue(copy),Is.Null);
            Assert.That(copy.GetType().GetProperty("Accessibility")!.GetValue(copy),Is.Null);
            var copied=((IEnumerable)Get(copy,"AudioGroups")).Cast<object>().ToArray();Assert.That(copied.Length,Is.EqualTo(2));
            Assert.That(copied.Single(g=>(string)Get(g,"GroupId")=="FutureGroup"),Is.SameAs(extra));
            Assert.That(Get(copied.Single(g=>(string)Get(g,"GroupId")=="UI"),"Numerator"),Is.EqualTo(0L));
            Assert.That(((ICollection)Get(settings,"AudioGroups")).Count,Is.EqualTo(1),"The confirmed source remains immutable.");
        }

        [Test]
        public void CampaignPlayerText_RequiresExplicitDraft_AndResolvesAll240ExactTitlesStatusesResults()
        {
            var source=AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"ProductionCampaignText.json");Assert.That(source,Is.Not.Null);
            Assert.Throws<ArgumentException>(()=>new ProductionCampaignText(source));using var text=new ProductionCampaignText(source,true);
            Assert.That(text.Resolve(0,"Tunnel","Route").Title,Is.EqualTo("Alles planmäßig"));
            for(var i=0;i<240;i++){var value=text.Resolve(i,"Abschnitt","Route");Assert.That(value.Title,Is.Not.Empty);Assert.That(value.Status,Is.Not.Empty);Assert.That(value.Result,Is.Not.Empty);}
            Assert.Throws<ArgumentOutOfRangeException>(()=>text.Resolve(240,"a","b"));
            var duplicate=new TextAsset(source.text.Replace("S1-01-01-02","S1-01-01-01"));try{Assert.Throws<ArgumentException>(()=>new ProductionCampaignText(duplicate,true));}finally{UnityEngine.Object.DestroyImmediate(duplicate);}
        }

        [UnityTest]
        public IEnumerator PuzzleHeader_LongPlayerStatusKeepsDetailButtonInside390PixelPhone()
        {
            var frame=Frame();frame.style.width=390;frame.style.height=844;frame.Q("home-screen").style.display=DisplayStyle.None;frame.Q("puzzle-screen").style.display=DisplayStyle.Flex;frame.Q("startup-overlay").style.display=DisplayStyle.None;frame.Q("main-navigation").style.display=DisplayStyle.None;
            var status=frame.Q<Label>("puzzle-phase");status.text="Die erste Abweichung ist bereits berücksichtigt. Worin, wird noch geklärt. Die weitere Rückmeldung erfolgt nach erneuter Abstimmung.";
            frame.Q<Button>("puzzle-zoom").text="Detail +";frame.Q<Label>("puzzle-title").text="Anpassung vor Eintritt der nächsten Abweichung";
            var panel=ScriptableObject.CreateInstance<PanelSettings>();var go=new GameObject("PlayerHeaderGeometry");var document=go.AddComponent<UIDocument>();document.panelSettings=panel;document.rootVisualElement.Add(frame);
            try
            {
                yield return null;yield return null;var detail=frame.Q<Button>("puzzle-zoom");
                Assert.That(detail.worldBound.xMax,Is.LessThanOrEqualTo(390));Assert.That(detail.worldBound.width,Is.GreaterThanOrEqualTo(60));
                Assert.That(status.worldBound.xMax,Is.LessThanOrEqualTo(detail.worldBound.xMin));Assert.That(status.worldBound.width,Is.GreaterThan(200));Assert.That(status.resolvedStyle.whiteSpace,Is.EqualTo(WhiteSpace.Normal));
            }
            finally{UnityEngine.Object.Destroy(go);UnityEngine.Object.Destroy(panel);}
        }

        [UnityTest]
        public IEnumerator AudioBank_HasEightBoundedVoices_RespectsMasterUiMute_AndSuspendsOwnedSources()
        {
            var audioType=AppDomain.CurrentDomain.GetAssemblies().Single(a=>a.GetName().Name=="STP.Audio").GetType("STP.Audio.ProductionGameAudio")!;
            var editorType=AppDomain.CurrentDomain.GetAssemblies().Single(a=>a.GetName().Name=="STP.Editor.Build").GetType("STP.Editor.Build.ProductionAudioAssets")!;
            var mixer=editorType.GetMethod("LoadMixer")!.Invoke(null,null)!;var clips=editorType.GetMethod("LoadClips")!.Invoke(null,null)!;
            var host=new GameObject("BoundedProductAudio");var component=host.AddComponent(audioType);audioType.GetMethod("Configure")!.Invoke(component,new[]{mixer,clips});
            try
            {
                Assert.That(Get(component,"VoiceCount"),Is.EqualTo(8));Assert.That(((Array)clips).Length,Is.EqualTo(15));
                foreach(AudioClip clip in (Array)clips){var data=new float[clip.samples];Assert.That(clip.GetData(data,0),Is.True);Assert.That(data.Max(Mathf.Abs),Is.LessThan(.15f),"The original source balance must not be normalized to full scale during import.");}
                for(var i=0;i<100;i++)audioType.GetMethod("Play")!.Invoke(component,new object[]{"TrackSeat"});
                Assert.That(host.GetComponentsInChildren<AudioSource>().Length,Is.EqualTo(8));
                var app=AppDomain.CurrentDomain.GetAssemblies().Single(a=>a.GetName().Name=="STP.Application");var preference=app.GetType("STP.Application.LocalAudioPreference")!;
                var groups=Array.CreateInstance(preference,2);groups.SetValue(preference.GetMethod("Create")!.Invoke(null,new object[]{"Master",1L,2L}),0);groups.SetValue(preference.GetMethod("Create")!.Invoke(null,new object[]{"UI",0L,100L}),1);
                var settings=app.GetType("STP.Application.LocalSettings")!.GetMethod("Create")!.Invoke(null,new object?[]{null,groups,null,null})!;
                audioType.GetMethod("ApplySettings")!.Invoke(component,new[]{settings});
                foreach(var source in host.GetComponentsInChildren<AudioSource>())Assert.That(source.volume,Is.EqualTo(source.outputAudioMixerGroup.name=="UI"?0:.5f).Within(.00001));
                audioType.GetMethod("SetPresentationState")!.Invoke(component,new object[]{true,true});audioType.GetMethod("SetSuspended")!.Invoke(component,new object[]{true});yield return null;
                Assert.That(Get(component,"IsSuspended"),Is.True);foreach(var source in host.GetComponentsInChildren<AudioSource>())Assert.That(source.isPlaying,Is.False);
                audioType.GetMethod("Play")!.Invoke(component,new object[]{"TrainStart"});Assert.That(host.GetComponentsInChildren<AudioSource>().Length,Is.EqualTo(8));
                audioType.GetMethod("SetSuspended")!.Invoke(component,new object[]{false});audioType.GetMethod("StopAll")!.Invoke(component,null);yield return null;
                foreach(var source in host.GetComponentsInChildren<AudioSource>())Assert.That(source.isPlaying,Is.False);
            }
            finally{UnityEngine.Object.Destroy(host);}
        }

        [UnityTest]
        public IEnumerator Postgame_LockedProfilesDoNotGenerate_AndAbandonUsesExactRecordAfterConfirmation()
        {
            var frame=Frame();using var locale=new ProductionGameLocalization(Locale());using var panelUi=new ProductionPostgamePanels(frame,locale,()=>{});
            var entry=new EndlessEntryView("persisted-record","dienstbeginn",EndlessEntryPhase.Active,true,true);
            var state=new EndlessView(false,false,new[]{"dienstbeginn"},new[]{entry},"ready");var startCalls=0;string? resumed=null,abandoned=null;
            panelUi.BindEndless(()=>state,_=>{startCalls++;return Task.CompletedTask;},id=>{resumed=id;return Task.CompletedTask;},id=>{abandoned=id;return Task.CompletedTask;});
            var panel=ScriptableObject.CreateInstance<PanelSettings>();var go=new GameObject("PostgameProductContract");var doc=go.AddComponent<UIDocument>();doc.panelSettings=panel;doc.rootVisualElement.Add(frame);
            try
            {
                yield return null;Submit(frame.Q<Button>("home-endless"));Assert.That(panelUi.IsModalOpen,Is.True);
                foreach(var id in new[]{"dienstbeginn","regelbetrieb","stoerungslage","grosslage"})Assert.That(frame.Q<Button>("endless-profile-"+id).enabledSelf,Is.False);
                Assert.That(startCalls,Is.Zero);state=new EndlessView(true,false,new[]{"dienstbeginn"},new[]{entry},"ready");panelUi.Refresh();
                Assert.That(frame.Q<Button>("endless-profile-dienstbeginn").enabledSelf,Is.True);Assert.That(frame.Q<Button>("endless-profile-grosslage").enabledSelf,Is.False);
                Submit(frame.Q<Button>("endless-resume-persisted-record"));Assert.That(resumed,Is.EqualTo("persisted-record"));
                Submit(frame.Q<Button>("endless-abandon-persisted-record"));Assert.That(abandoned,Is.Null);
                Submit(frame.Q<Button>("endless-abandon-cancel"));Assert.That(abandoned,Is.Null);
                Submit(frame.Q<Button>("endless-abandon-persisted-record"));Submit(frame.Q<Button>("endless-abandon-confirm"));Assert.That(abandoned,Is.EqualTo("persisted-record"));
                Assert.That(frame.Q("endless-record-persisted-record"),Is.Not.Null,"Task completion cannot remove an unacknowledged record from the view.");
                state=new EndlessView(true,false,new[]{"dienstbeginn"},Array.Empty<EndlessEntryView>(),"ready");panelUi.Refresh();Assert.That(frame.Q("endless-record-persisted-record"),Is.Null);
            }
            finally{UnityEngine.Object.Destroy(go);UnityEngine.Object.Destroy(panel);}
        }

        [UnityTest]
        public IEnumerator CompletionBonus_RequiresExplicitOptIn_AndOnlyConfirmedSnapshotShowsTenPoints()
        {
            var frame=Frame();frame.Q("home-screen").style.display=DisplayStyle.None;frame.Q("puzzle-screen").style.display=DisplayStyle.Flex;frame.Q("result-panel").style.display=DisplayStyle.Flex;frame.Q("startup-overlay").style.display=DisplayStyle.None;
            using var locale=new ProductionGameLocalization(Locale());using var panelUi=new ProductionPostgamePanels(frame,locale,()=>{});
            var state=new CompletionBonusView(true,true,false,null);var completion=new TaskCompletionSource<bool>();var calls=0;
            panelUi.BindCompletionBonus(()=>state,video=>{Assert.That(video,Is.True);calls++;return completion.Task;});
            var panel=ScriptableObject.CreateInstance<PanelSettings>();var go=new GameObject("CompletionBonusProductContract");var document=go.AddComponent<UIDocument>();document.panelSettings=panel;document.rootVisualElement.Add(frame);
            try
            {
                yield return null;Assert.That(calls,Is.Zero);Submit(frame.Q<Button>("result-video"));Submit(frame.Q<Button>("result-video"));Assert.That(calls,Is.EqualTo(1));
                Assert.That(frame.Q<Button>("result-skip-bonus").enabledSelf,Is.False);completion.SetResult(true);panelUi.Refresh();
                Assert.That(frame.Q<Label>("result-bonus-state").text,Is.EqualTo(locale.Resolve("completion.optional")),"Task completion is not grant truth.");
                state=new CompletionBonusView(false,false,false,10);panelUi.Refresh();Assert.That(frame.Q<Label>("result-bonus-state").text,Is.EqualTo("Zusätzlich bestätigt: 10 GP"));
                Assert.That(frame.Q<Button>("result-video").style.display.value,Is.EqualTo(DisplayStyle.None));
                Assert.That(frame.Q<Label>("result-bonus-state").style.display.value,Is.EqualTo(DisplayStyle.Flex));
                state=new CompletionBonusView(false,false,true,null);panelUi.Refresh();Assert.That(frame.Q<Label>("result-bonus-state").text,Is.EqualTo(locale.Resolve("completion.pending")));Assert.That(frame.Q<Label>("result-bonus-state").style.display.value,Is.EqualTo(DisplayStyle.Flex));
                state=new CompletionBonusView(false,false,false,null);panelUi.Refresh();Assert.That(frame.Q<Label>("result-bonus-state").style.display.value,Is.EqualTo(DisplayStyle.None));Assert.That(frame.Q<Label>("result-bonus-state").text,Is.Empty,"A completed skip or absent offer must not leave phantom video copy.");
                Assert.That(frame.Q<Button>("result-video").style.display.value,Is.EqualTo(DisplayStyle.None));Assert.That(frame.Q<Button>("result-skip-bonus").style.display.value,Is.EqualTo(DisplayStyle.None));
            }
            finally{UnityEngine.Object.Destroy(go);UnityEngine.Object.Destroy(panel);}
        }

        [Test]
        public void ResultContinuation_OnlyExplicitActualResultsInvoke_AndErrorsContinueSameAttemptOnce()
        {
            using var gate=new ResultContinuationGate();var attempt=Guid.NewGuid();var tcs=new TaskCompletionSource<bool>();var calls=0;
            gate.Bind(()=>{calls++;return tcs.Task;});Assert.That(calls,Is.Zero);Assert.That(gate.TryBegin(false,attempt),Is.False);
            Assert.That(gate.Poll(false,attempt),Is.False);Assert.That(calls,Is.Zero);
            Assert.That(gate.TryBegin(true,attempt),Is.True);Assert.That(gate.TryBegin(true,attempt),Is.False);Assert.That(calls,Is.EqualTo(1));
            Assert.That(gate.Poll(true,attempt),Is.False);tcs.SetException(new InvalidOperationException("optional service unavailable"));
            Assert.That(gate.Poll(true,attempt),Is.True);Assert.That(gate.IsPending,Is.False);Assert.That(gate.Poll(true,attempt),Is.False);
            gate.Bind(()=>throw new InvalidOperationException("synchronous optional service failure"));Assert.That(gate.TryBegin(true,attempt),Is.True);Assert.That(gate.Poll(true,attempt),Is.True);
        }

        [Test]
        public void ResultContinuation_CancelledPolicyContinues_ButStaleAttemptAndDisposalNeverNavigate()
        {
            using var gate=new ResultContinuationGate();var attempt=Guid.NewGuid();
            gate.Bind(()=>Task.FromCanceled(new System.Threading.CancellationToken(true)));gate.TryBegin(true,attempt);Assert.That(gate.Poll(true,attempt),Is.True);
            gate.Bind(()=>Task.CompletedTask);gate.TryBegin(true,attempt);Assert.That(gate.Poll(true,Guid.NewGuid()),Is.False);
            var late=new TaskCompletionSource<bool>();gate.Bind(()=>late.Task);gate.TryBegin(true,attempt);gate.Dispose();late.SetException(new InvalidOperationException("late service failure"));
            Assert.That(gate.IsPending,Is.False);Assert.That(gate.Poll(true,attempt),Is.False);Assert.That(gate.TryBegin(true,attempt),Is.False);
        }

        [Test]
        public void ResultContinuation_CompletedHookWaitsForSameResultAfterTemporaryFocusOrSavePause()
        {
            using var gate=new ResultContinuationGate();var attempt=Guid.NewGuid();var calls=0;
            gate.Bind(()=>{calls++;return Task.CompletedTask;});Assert.That(gate.TryBegin(true,attempt),Is.True);
            Assert.That(gate.PollWhenReady(true,false,attempt),Is.False);Assert.That(gate.IsPending,Is.True);
            Assert.That(gate.PollWhenReady(true,false,attempt),Is.False);Assert.That(calls,Is.EqualTo(1));
            Assert.That(gate.PollWhenReady(true,true,attempt),Is.True);Assert.That(gate.IsPending,Is.False);
            gate.TryBegin(true,attempt);Assert.That(gate.PollWhenReady(true,false,Guid.NewGuid()),Is.False);Assert.That(gate.IsPending,Is.False,"Only a genuinely stale attempt discards the completed callback.");
        }

        [UnityTest]
        public IEnumerator CosmeticsProjection_ActiveLiveryAndObjectCanBeRemoved_TrainRemainsMutuallySelected()
        {
            var frame=Frame();using var presenter=new ProductionGamePresenter(frame,Locale(),()=>0,_=>Task.CompletedTask);presenter.BindAssetCatalog(AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"RailwayAssets.json"),true);
            var app=AppDomain.CurrentDomain.GetAssemblies().Single(a=>a.GetName().Name=="STP.Application");var content=AppDomain.CurrentDomain.GetAssemblies().Single(a=>a.GetName().Name=="STP.Infrastructure.Content");
            var source=AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"MobileCandidate.cosmetics.json");var binding=AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"MobileCandidate.bindings.json");
            var hash=System.Text.RegularExpressions.Regex.Match(binding.text,"\"cosmeticsHashSha256\"\\s*:\\s*\"([a-f0-9]{64})\"").Groups[1].Value;
            var catalog=Activator.CreateInstance(content.GetType("STP.Infrastructure.Content.LockedCosmeticsCatalog")!,source.bytes,hash,Enum.Parse(app.GetType("STP.Application.CatalogUse")!,"InternalPreview"),Activator.CreateInstance(content.GetType("STP.Infrastructure.Content.CanonicalJsonAdapter")!))!;
            var selected=new System.Collections.Generic.HashSet<string>(new[]{"train-stammrot-default","livery-baustellengelb","obj-wartebank-ohne-anschluss"});var owned=new System.Collections.Generic.HashSet<string>(selected);var calls=0;
            typeof(ProductionGamePresenter).GetMethod("BindCosmetics")!.Invoke(presenter,new object[]{catalog,(Func<string,bool>)owned.Contains,(Func<string,bool>)selected.Contains,(Func<string,bool>)(id=>owned.Contains(id)&&id!="train-stammrot-default"),(Func<string,Task>)(id=>{calls++;selected.Remove(id);return Task.CompletedTask;})});
            var render=typeof(ProductionGamePresenter).GetMethod("RenderCosmetics",BindingFlags.Instance|BindingFlags.NonPublic)!;render.Invoke(presenter,new object[]{true});
            frame.Q("startup-overlay").style.display=DisplayStyle.None;frame.Q("home-screen").style.display=DisplayStyle.None;frame.Q("depot-screen").style.display=DisplayStyle.Flex;
            var panel=ScriptableObject.CreateInstance<PanelSettings>();var go=new GameObject("CosmeticSelectionProjection");var document=go.AddComponent<UIDocument>();document.panelSettings=panel;document.rootVisualElement.Add(frame);
            try
            {
                yield return null;var livery=frame.Q<Button>("cosmetic-action-livery-baustellengelb");var prop=frame.Q<Button>("cosmetic-action-obj-wartebank-ohne-anschluss");
                Assert.That(livery.enabledSelf,Is.True);Assert.That(livery.text,Is.EqualTo("Ablegen"));Assert.That(prop.enabledSelf,Is.True);Assert.That(frame.Q<Button>("cosmetic-action-train-stammrot-default").enabledSelf,Is.False);
                Submit(livery);render.Invoke(presenter,new object[]{true});Assert.That(calls,Is.EqualTo(1));Assert.That(selected.Contains("livery-baustellengelb"),Is.False);Assert.That(selected.Contains("obj-wartebank-ohne-anschluss"),Is.True);
                Assert.That(frame.Q<Label>("depot-current-livery").text,Is.EqualTo("Originallackierung"));Assert.That(prop.text,Is.EqualTo("Ablegen"));
            }
            finally{UnityEngine.Object.Destroy(go);UnityEngine.Object.Destroy(panel);}
        }

        [Test]
        public void EndlessDisplay_UsesFourLocalizedProfiles_AndActualSeparateResultText()
        {
            using var presenter=new ProductionGamePresenter(Frame(),Locale(),()=>0,_=>Task.CompletedTask);using var locale=new ProductionGameLocalization(Locale());
            foreach(var id in new[]{"dienstbeginn","regelbetrieb","stoerungslage","grosslage"})
            {
                var display=presenter.ResolveEndlessDisplay(id);Assert.That(display.Section,Is.EqualTo(locale.Resolve("endless.title")));Assert.That(display.Route,Is.EqualTo(locale.Resolve("endless.profile."+id)));
                Assert.That(display.Title,Is.EqualTo(locale.Resolve("endless.puzzle.title")));Assert.That(display.Status,Is.EqualTo(locale.Resolve("puzzle.instruction")));Assert.That(display.Result,Is.EqualTo(locale.Resolve("endless.puzzle.result")));
            }
            Assert.Throws<ArgumentException>(()=>presenter.ResolveEndlessDisplay("unknown-profile"));
        }

        [Test]
        public void ShowCurrentPuzzle_RejectsUnboundStartupAndFailure_WithoutCreatingOrResumingAView()
        {
            var calls=0;using var presenter=new ProductionGamePresenter(Frame(),Locale(),()=>5000,_=>{calls++;return Task.CompletedTask;});
            var error=Assert.Throws<InvalidOperationException>(()=>presenter.ShowCurrentPuzzle());Assert.That(error!.Message,Is.EqualTo("PRODUCT_SAVED_PUZZLE_NOT_READY"));
            Assert.That(presenter.HasBoundModel,Is.False);Assert.That(presenter.IsPuzzleScreenActive,Is.False);Assert.That(presenter.Grid,Is.Null);Assert.That(calls,Is.Zero);
            presenter.ShowRequestFailure(false);Assert.Throws<InvalidOperationException>(()=>presenter.ShowCurrentPuzzle());
            Assert.That(presenter.IsPuzzleScreenActive,Is.False);Assert.That(presenter.Grid,Is.Null);Assert.That(calls,Is.Zero,"Opening a confirmed generated view cannot invent profile/create/resume requests.");
        }

        [UnityTest]
        public IEnumerator EndlessCompletion_ProviderReservationBlocksSkipAndDuplicateVideo_OnSameRecord()
        {
            var frame=Frame();using var locale=new ProductionGameLocalization(Locale());using var panelUi=new ProductionPostgamePanels(frame,locale,()=>{});
            var entry=new EndlessEntryView("completed-record","grosslage",EndlessEntryPhase.CompletionPending,false,false);
            var state=new EndlessView(true,false,Array.Empty<string>(),new[]{entry},"ready");var canSkip=true;var calls=0;
            panelUi.BindEndless(()=>state,_=>Task.CompletedTask,_=>Task.CompletedTask,_=>Task.CompletedTask);
            panelUi.BindEndlessReward(_=>true,(_,__)=>{calls++;return Task.CompletedTask;},_=>canSkip);
            var panel=ScriptableObject.CreateInstance<PanelSettings>();var go=new GameObject("EndlessCompletionRecoveryProjection");var document=go.AddComponent<UIDocument>();document.panelSettings=panel;document.rootVisualElement.Add(frame);
            try
            {
                panelUi.Open();yield return null;var skip=frame.Q<Button>("endless-skip-completed-record");var video=frame.Q<Button>("endless-video-completed-record");Assert.That(skip.enabledSelf,Is.True);Assert.That(video.enabledSelf,Is.True);Assert.That(calls,Is.Zero);
                canSkip=false;panelUi.Refresh();Assert.That(frame.Q<Button>("endless-skip-completed-record"),Is.SameAs(skip));Assert.That(skip.enabledSelf,Is.False);Assert.That(video.enabledSelf,Is.False);
                Assert.That(frame.Q("endless-record-completed-record").Query<Label>().ToList().Any(l=>l.text==locale.Resolve("endless.reward.pending")),Is.True);
                Submit(skip);Submit(video);Assert.That(calls,Is.Zero);Assert.That(frame.Q<Button>("endless-abandon-completed-record").style.display.value,Is.EqualTo(DisplayStyle.None));
            }
            finally{UnityEngine.Object.Destroy(go);UnityEngine.Object.Destroy(panel);}
        }

        [UnityTest]
        public IEnumerator ResultSheet_CompactContentDoesNotLeaveBlankTail_AndLongContentRemainsScrollable()
        {
            var frame=Frame();using var locale=new ProductionGameLocalization(Locale());
            foreach(var label in frame.Query<Label>().ToList())if(!string.IsNullOrEmpty(label.viewDataKey))label.text=locale.Resolve(label.viewDataKey);
            var art=new RailwayIllustrationElement();var catalog=new RailwayAssetCatalog(AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"RailwayAssets.json"),true);var moment=catalog.ResolvePuzzleMoment("S1-01-01-01",true);art.SetComposition(catalog.ResolveComposition(moment.compositionId),moment.stage);frame.Q("result-art").Add(art);
            frame.style.width=390;frame.style.height=844;frame.Q("home-screen").style.display=DisplayStyle.None;frame.Q("main-navigation").style.display=DisplayStyle.None;var owner=frame.Q("puzzle-screen");owner.style.display=DisplayStyle.Flex;
            var sheet=frame.Q<ScrollView>("result-panel");sheet.style.display=DisplayStyle.Flex;frame.Q("startup-overlay").style.display=DisplayStyle.None;
            frame.Q<Label>("result-copy").text="Die Verbindung steht.";frame.Q<Label>("result-mode").text="Erste Fahrt";frame.Q<Label>("result-stars").text="★ ☆ ☆";frame.Q<Label>("result-time").text="00:03";frame.Q<Label>("result-progress").text="1 / 240";frame.Q<Label>("result-balance").text="15 GP";frame.Q<Button>("result-next").text="Nächste Verbindung";frame.Q<Button>("result-home").text="Zum Bahnhof";
            using var layout=new ProductionResultSheet(sheet,owner);var panel=ScriptableObject.CreateInstance<PanelSettings>();panel.themeStyleSheet=AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(Ui+"DevelopmentPuzzleTheme.tss");panel.scaleMode=PanelScaleMode.ConstantPixelSize;
            Assert.That(panel.themeStyleSheet,Is.Not.Null,"Geometry checks require the same default theme/font metrics as the actual scene.");
            var target=new RenderTexture(390,844,24);target.Create();panel.targetTexture=target;
            var go=new GameObject("ResultSheetContentGeometry");var document=go.AddComponent<UIDocument>();document.panelSettings=panel;document.rootVisualElement.Add(frame);
            try
            {
                layout.Refresh();yield return null;yield return null;yield return null;layout.Refresh();yield return null;yield return null;
                Assert.That(frame.Q("result-art").worldBound.height,Is.InRange(99f,101f));Assert.That(art.worldBound.width,Is.GreaterThan(250));Assert.That(frame.Q<Label>("result-copy").worldBound.height,Is.GreaterThan(10));
                Assert.That(document.rootVisualElement.contentRect.width,Is.EqualTo(390).Within(1));Assert.That(document.rootVisualElement.contentRect.height,Is.EqualTo(844).Within(1));Assert.That(owner.contentRect.height,Is.EqualTo(844).Within(1),"The fixture must render an actual full phone panel, independent of the editor window.");
                Assert.That(sheet.worldBound.height,Is.LessThanOrEqualTo(owner.contentRect.height*.82f+1));Assert.That(sheet.verticalScroller.highValue,Is.LessThan(.5f),"The compact fixture must fit without scrolling; the lower inset then measures excess blank space directly.");
                Assert.That(sheet.worldBound.yMax-frame.Q<Button>("result-home").worldBound.yMax,Is.InRange(18f,55f),"The last action should finish near the sheet's lower inset.");
                frame.Q<Label>("result-copy").text=string.Join(" ",Enumerable.Repeat("Eine weitere Meldung bleibt eine weitere Meldung.",35));layout.Refresh();yield return null;yield return null;yield return null;layout.Refresh();yield return null;yield return null;
                Assert.That(sheet.worldBound.height,Is.LessThanOrEqualTo(owner.contentRect.height*.82f+1));Assert.That(sheet.verticalScroller.highValue,Is.GreaterThan(0));
                sheet.scrollOffset=new Vector2(0,sheet.verticalScroller.highValue);yield return null;yield return null;
                var lastAction=frame.Q<Button>("result-home").worldBound;Assert.That(lastAction.yMax,Is.LessThanOrEqualTo(sheet.contentViewport.worldBound.yMax+1));Assert.That(lastAction.yMin,Is.GreaterThanOrEqualTo(sheet.contentViewport.worldBound.yMin));
            }
            finally{panel.targetTexture=null;target.Release();UnityEngine.Object.Destroy(target);UnityEngine.Object.Destroy(go);UnityEngine.Object.Destroy(panel);}
        }

        [UnityTest]
        public IEnumerator Accessibility_OnlyTopModalCanActivate_NativePauseAndDetachReleaseOwnNodes()
        {
            using var locale=new ProductionGameLocalization(Locale());var root=new VisualElement();root.AddToClassList("production-root");root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(Ui+"ProductionGame.uss"));root.style.width=390;root.style.height=844;
            var calls=0;var underlying=new Button(()=>calls++){text="Spiel starten"};underlying.style.height=48;var scroll=new ScrollView();scroll.style.height=80;scroll.style.width=300;scroll.Add(underlying);root.Add(scroll);
            var modal=new VisualElement();modal.AddToClassList("overlay");modal.style.display=DisplayStyle.None;modal.style.width=300;modal.style.height=200;root.Add(modal);
            var close=new Button(()=>calls+=10){text="Schließen"};close.style.height=48;modal.Add(close);
            var panel=ScriptableObject.CreateInstance<PanelSettings>();panel.themeStyleSheet=AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(Ui+"DevelopmentPuzzleTheme.tss");panel.scaleMode=PanelScaleMode.ConstantPixelSize;
            var target=new RenderTexture(390,844,24);target.Create();panel.targetTexture=target;
            var go=new GameObject("NativeAccessibilityModalScope");var document=go.AddComponent<UIDocument>();document.panelSettings=panel;document.rootVisualElement.Add(root);
            using var semantics=new ProductionGameAccessibility(root,locale.Resolve,false);var previousNative=AssistiveSupport.activeHierarchy;
            try
            {
                yield return null;yield return null;semantics.RefreshForInspection();Assert.That(AssistiveSupport.activeHierarchy,Is.SameAs(previousNative),"Inspection must not enable or replace a native hierarchy.");
                Assert.That(semantics.TryActivate(underlying),Is.True);Assert.That(calls,Is.EqualTo(1));var originalButton=AccessibleNodes(semantics.Hierarchy).Single(n=>n.label==underlying.text);var originalScroll=AccessibleNodes(semantics.Hierarchy).Single(n=>n.role==AccessibilityRole.ScrollView);
                modal.style.display=DisplayStyle.Flex;yield return null;semantics.RefreshForInspection();Assert.That(semantics.TryActivate(underlying),Is.False);Assert.That(semantics.TryActivate(close),Is.True);Assert.That(calls,Is.EqualTo(11));
                Assert.That(AccessibleNodes(semantics.Hierarchy).Single(n=>n.label==underlying.text).isActive,Is.False);
                semantics.SetSuspended(true);Assert.That(AccessibleNodes(semantics.Hierarchy).All(n=>!n.isActive),Is.True);Assert.That(semantics.TryActivate(close),Is.False);
                semantics.SetSuspended(false);semantics.RefreshForInspection();Assert.That(semantics.TryActivate(close),Is.True);Assert.That(calls,Is.EqualTo(21));
                scroll.RemoveFromHierarchy();Assert.DoesNotThrow(()=>semantics.RefreshForInspection());Assert.That(semantics.Hierarchy.ContainsNode(originalButton),Is.False);Assert.That(semantics.Hierarchy.ContainsNode(originalScroll),Is.False,"A detached scroll parent and child must both be removed without double-removing a subtree.");
                root.RemoveFromHierarchy();Assert.That(AccessibleNodes(semantics.Hierarchy).All(n=>!n.isActive),Is.True);Assert.That(semantics.TryActivate(close),Is.False);
            }
            finally{panel.targetTexture=null;target.Release();UnityEngine.Object.Destroy(target);UnityEngine.Object.Destroy(go);UnityEngine.Object.Destroy(panel);}
        }

        [UnityTest]
        public IEnumerator Accessibility_RealGridExposesLocalizedCellCountsAndPorts_AndSameGuardedButtonInput()
        {
            var session=Session(6,5,1,1,"W","E");using var locale=new ProductionGameLocalization(Locale());var calls=0;
            using var grid=(ProductionPuzzleGrid)typeof(ProductionPuzzleGrid).GetConstructors().Single().Invoke(new object[]{session,(Func<string,object[],string>)locale.Resolve,(Action<int,int>)((_,__)=>calls++)});
            var root=new VisualElement();root.AddToClassList("production-root");root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(Ui+"ProductionGame.uss"));root.style.width=390;root.style.height=600;grid.style.width=370;grid.style.height=420;root.Add(grid);grid.Refresh(true);
            var panel=ScriptableObject.CreateInstance<PanelSettings>();panel.themeStyleSheet=AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(Ui+"DevelopmentPuzzleTheme.tss");panel.scaleMode=PanelScaleMode.ConstantPixelSize;
            var target=new RenderTexture(390,600,24);target.Create();panel.targetTexture=target;
            var go=new GameObject("NativeAccessibilityActualGrid");var document=go.AddComponent<UIDocument>();document.panelSettings=panel;document.rootVisualElement.Add(root);using var semantics=new ProductionGameAccessibility(root,locale.Resolve,false);
            try
            {
                yield return null;yield return null;semantics.RefreshForInspection();var cell=grid.CellButton(0,0);var node=AccessibleNodes(semantics.Hierarchy).Single(n=>n.label==cell.tooltip);
                Assert.That(node.label,Is.EqualTo(locale.Resolve("cell",1,1,locale.Resolve("accessibility.cell.empty"))));Assert.That(node.role,Is.EqualTo(AccessibilityRole.Button));Assert.That(node.isActive,Is.True);
                Assert.That(AccessibleNodes(semantics.Hierarchy).Count(n=>n.role==AccessibilityRole.Button&&n.isActive),Is.EqualTo(30),"All thirty actual cell rectangles must remain reachable.");
                Assert.That(node.frame.width,Is.EqualTo(cell.worldBound.width*cell.panel.scaledPixelsPerPoint).Within(.1f));Assert.That(semantics.TryActivate(cell),Is.True);Assert.That(calls,Is.EqualTo(1));
                Assert.That(AccessibleNodes(semantics.Hierarchy).Any(n=>n.label.StartsWith("Zeile 1:",StringComparison.Ordinal)),Is.True);Assert.That(AccessibleNodes(semantics.Hierarchy).Any(n=>n.label.StartsWith("Spalte 1:",StringComparison.Ordinal)),Is.True);
                Assert.That(AccessibleNodes(semantics.Hierarchy).Any(n=>n.label==locale.Resolve("accessibility.endpoint","A",1,2,locale.Resolve("accessibility.edge.W"))),Is.True);
                grid.Refresh(false);semantics.RefreshForInspection();Assert.That((node.state&AccessibilityState.Disabled)!=0,Is.True);Assert.That(semantics.TryActivate(cell),Is.False);Assert.That(calls,Is.EqualTo(1));
            }
            finally{panel.targetTexture=null;target.Release();UnityEngine.Object.Destroy(target);UnityEngine.Object.Destroy(go);UnityEngine.Object.Destroy(panel);}
        }

        private static System.Collections.Generic.IEnumerable<AccessibilityNode> AccessibleNodes(AccessibilityHierarchy hierarchy)
        {foreach(var node in hierarchy.rootNodes)foreach(var entry in AccessibleBranch(node))yield return entry;}
        private static System.Collections.Generic.IEnumerable<AccessibilityNode> AccessibleBranch(AccessibilityNode node)
        {yield return node;foreach(var child in node.children)foreach(var entry in AccessibleBranch(child))yield return entry;}

        private static object Session(int width,int height,int aIndex,int bIndex,string aSide,string bSide,bool fullTargets=false)
        {
            var app=AppDomain.CurrentDomain.GetAssemblies().Single(a=>a.GetName().Name=="STP.Application");
            var identity=Activator.CreateInstance(app.GetType("STP.Application.PublicPuzzleIdentity")!,"presentation-contract","STP-PUZZLE-SEMANTIC-JCS-1",new string('a',64))!;
            var side=app.GetType("STP.Application.PublicPuzzleSide")!;var rows=Enumerable.Repeat(fullTargets?width:1,height).ToArray();var columns=Enumerable.Repeat(fullTargets?height:1,width).ToArray();
            // Equal totals and public exterior endpoints; no authoring solution is supplied.
            if(!fullTargets){if(width>height)rows[0]+=width-height;else if(height>width)columns[0]+=height-width;}
            var puzzle=Activator.CreateInstance(app.GetType("STP.Application.PublicPuzzleInput")!,identity,"train-track-v1",width,height,Enum.Parse(side,aSide),aIndex,Enum.Parse(side,bSide),bIndex,rows,columns)!;
            return Activator.CreateInstance(app.GetType("STP.Application.Public240ReviewSession")!,puzzle)!;
        }
        private static object Get(object value,string property)=>value.GetType().GetProperty(property)!.GetValue(value)!;
        private static void Apply(object session,int x,int y,int content,long time)
        {
            var tool=session.GetType().Assembly.GetType("STP.Application.DevelopmentTool")!;
            var method=session.GetType().GetMethods().Single(m=>m.Name=="Apply"&&m.GetParameters().Length==4);
            method.Invoke(session,new[]{(object)x,y,Enum.ToObject(tool,content),time});
        }
        private static void Submit(Button button){using var e=NavigationSubmitEvent.GetPooled();e.target=button;button.SendEvent(e);}
#endif
    }
}
