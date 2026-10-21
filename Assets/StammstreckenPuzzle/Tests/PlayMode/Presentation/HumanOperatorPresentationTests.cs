using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Presentation.UI;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace STP.Tests.Presentation.PlayMode
{
    /// <summary>Operator intent tests use display DTOs only; no observation, human identity or solver fact is invented.</summary>
    public sealed class HumanOperatorPresentationTests
    {
        [Test]
        public void OperatorReadModels_RequireClosedPackIdsSixInputsAndActualDisplayBinding()
        {
            Assert.That(Packs().Count,Is.EqualTo(17));
            Assert.Throws<ArgumentException>(()=>new HumanOperatorPack("campaign-pack-10.json",6));
            Assert.Throws<ArgumentException>(()=>new HumanOperatorPack("endless-pack-09.json",6));
            Assert.Throws<ArgumentException>(()=>new HumanOperatorPack("Campaign-pack-01.json",6));
            Assert.Throws<ArgumentException>(()=>new HumanOperatorPack("../campaign-pack-01.json",6));
            Assert.Throws<ArgumentException>(()=>new HumanOperatorPack("campaign-pack-01.json",5));
            Assert.Throws<ArgumentException>(()=>new HumanOperatorView(HumanOperatorPhase.Missing,string.Empty));
            Assert.Throws<ArgumentException>(()=>new HumanOperatorView(HumanOperatorPhase.Active,"observed",null));
            Assert.Throws<ArgumentException>(()=>new HumanOperatorView(HumanOperatorPhase.Complete,"observed","endless-pack-09.json"));
            Assert.Throws<ArgumentException>(()=>new HumanOperatorView(HumanOperatorPhase.Missing,"observed","campaign-pack-01.json"));
            Assert.Throws<ArgumentException>(()=>new HumanOperatorView(HumanOperatorPhase.Recovery,"observed",canExport:true));
            Assert.Throws<ArgumentException>(()=>new HumanOperatorView((HumanOperatorPhase)99,"observed"));
        }

#if UNITY_EDITOR
        private const string Ui="Assets/StammstreckenPuzzle/UI/";
        private static TextAsset Locale()=>AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"ProductionGameLocale.json");
        private static VisualElement Frame(){var frame=AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Ui+"ProductionGame.uxml").CloneTree();frame.style.width=390;frame.style.height=844;return frame;}

        [UnityTest]
        public IEnumerator AllSeventeenPacks_SelectionAndCancelMakeNoCalls_OnlyConfirmedStartHasOneBoundIntent()
        {
            using var phone=new Phone();var frame=Frame();phone.Root.Add(frame);var starts=0;var resumes=0;var retries=0;var exports=0;string? pack=null,key=null;
            var state=new HumanOperatorView(HumanOperatorPhase.Missing,"missing-observation");var hold=new TaskCompletionSource<bool>();
            using var controller=new ProductionHumanOperatorPanel(frame,Locale(),Packs(),()=>state,(p,k)=>{starts++;pack=p;key=k;return hold.Task;},()=>{resumes++;return Task.CompletedTask;},()=>{retries++;return Task.CompletedTask;},()=>{exports++;return Task.CompletedTask;},false);
            yield return null;controller.Refresh();
            Assert.That(frame.Query<Button>(className:"human-operator-pack").ToList().Count,Is.EqualTo(17));
            foreach(var id in new[]{"home-screen","map-screen","human-screen","puzzle-screen","tool-dock","hint-overlay","result-panel","startup-overlay","main-navigation"})
                Assert.That(Presented(frame.Q(id)),Is.False,id+" must stay behind the neutral operator boundary.");
            Assert.That(frame.Query<Button>(className:"puzzle-cell").ToList(),Is.Empty);
            foreach(var option in Packs())Submit(frame.Q<Button>("human-operator-pack-"+option.Id));
            Assert.That(starts+resumes+retries+exports,Is.Zero,"All seventeen choices are transient display intents.");
            Submit(frame.Q<Button>("human-operator-choose"));Assert.That(controller.IsConfirmationOpen,Is.True);Assert.That(starts,Is.Zero);
            Submit(frame.Q<Button>("human-operator-confirm-cancel"));Assert.That(controller.IsConfirmationOpen,Is.False);Assert.That(starts,Is.Zero);
            Submit(frame.Q<Button>("human-operator-pack-campaign-pack-09.json"));Submit(frame.Q<Button>("human-operator-choose"));Submit(frame.Q<Button>("human-operator-confirm-start"));
            Assert.That(starts,Is.EqualTo(1));Assert.That(pack,Is.EqualTo("campaign-pack-09.json"));Assert.That(key,Is.EqualTo("missing-observation"));
            Assert.That(controller.HasPendingOperation,Is.True);Submit(frame.Q<Button>("human-operator-confirm-start"));Submit(frame.Q<Button>("human-operator-confirm-cancel"));Assert.That(starts,Is.EqualTo(1));Assert.That(controller.IsConfirmationOpen,Is.True);
            controller.SetExternalSuspended(true);hold.SetResult(true);yield return null;controller.Refresh();
            Assert.That(starts,Is.EqualTo(1));Assert.That(resumes+retries+exports,Is.Zero,"Task completion does not implicitly navigate or repeat any operation.");
        }

        [UnityTest]
        public IEnumerator ActiveRecoveryUnsupportedAndChangedAuthority_CannotStart_UnknownNeedsExplicitRetry()
        {
            using var phone=new Phone();var frame=Frame();phone.Root.Add(frame);var starts=0;var resumes=0;var retries=0;
            var state=new HumanOperatorView(HumanOperatorPhase.Active,"active","campaign-pack-09.json",canExport:true);
            using var controller=new ProductionHumanOperatorPanel(frame,Locale(),Packs(),()=>state,(_,__)=>{starts++;return Task.FromException(new InvalidOperationException("unknown"));},()=>{resumes++;return Task.CompletedTask;},()=>{retries++;return Task.CompletedTask;},()=>Task.CompletedTask,false);
            yield return null;Submit(frame.Q<Button>("human-operator-pack-endless-pack-08.json"));Submit(frame.Q<Button>("human-operator-choose"));Submit(frame.Q<Button>("human-operator-confirm-start"));Assert.That(starts,Is.Zero);
            controller.SetExternalSuspended(true);Submit(frame.Q<Button>("human-operator-resume"));Assert.That(resumes,Is.Zero);controller.SetExternalSuspended(false);Submit(frame.Q<Button>("human-operator-resume"));Assert.That(resumes,Is.EqualTo(1));
            state=new HumanOperatorView(HumanOperatorPhase.Missing,"missing-a");controller.Refresh();Submit(frame.Q<Button>("human-operator-pack-campaign-pack-01.json"));Submit(frame.Q<Button>("human-operator-choose"));
            state=new HumanOperatorView(HumanOperatorPhase.Missing,"missing-b");Submit(frame.Q<Button>("human-operator-confirm-start"));Assert.That(starts,Is.Zero);Assert.That(controller.IsConfirmationOpen,Is.False);Assert.That(controller.SelectedPackId,Is.Null);
            state=new HumanOperatorView(HumanOperatorPhase.Complete,"completed","campaign-pack-09.json");controller.Refresh();Submit(frame.Q<Button>("human-operator-pack-endless-pack-08.json"));Submit(frame.Q<Button>("human-operator-choose"));
            controller.SetExternalSuspended(true);Submit(frame.Q<Button>("human-operator-confirm-start"));Submit(frame.Q<Button>("human-operator-confirm-cancel"));Assert.That(controller.IsConfirmationOpen,Is.True);Assert.That(starts,Is.Zero);
            controller.SetExternalSuspended(false);Submit(frame.Q<Button>("human-operator-confirm-start"));controller.Refresh();Assert.That(starts,Is.EqualTo(1));Assert.That(controller.HasRequestFault,Is.True);
            Submit(frame.Q<Button>("human-operator-resume"));Submit(frame.Q<Button>("human-operator-pack-campaign-pack-01.json"));Submit(frame.Q<Button>("human-operator-confirm-start"));Assert.That(starts,Is.EqualTo(1));Assert.That(resumes,Is.EqualTo(1));Assert.That(retries,Is.Zero);
            Submit(frame.Q<Button>("human-operator-retry"));controller.Refresh();Assert.That(retries,Is.EqualTo(1));Assert.That(controller.HasRequestFault,Is.False);Assert.That(starts,Is.EqualTo(1));
            foreach(var phase in new[]{HumanOperatorPhase.Recovery,HumanOperatorPhase.Unsupported})
            {state=new HumanOperatorView(phase,"blocked");controller.Refresh();Submit(frame.Q<Button>("human-operator-pack-campaign-pack-01.json"));Submit(frame.Q<Button>("human-operator-choose"));Submit(frame.Q<Button>("human-operator-confirm-start"));Assert.That(starts,Is.EqualTo(1));Assert.That(Presented(frame.Q("human-operator-export")),Is.False);}
        }

        [UnityTest]
        public IEnumerator PhoneLayoutAndAccessibility_ExposeOnlyNeutralVisibleOperatorControls_ConfirmIsTopScope()
        {
            using var phone=new Phone();var frame=Frame();phone.Root.Add(frame);var starts=0;var state=new HumanOperatorView(HumanOperatorPhase.Missing,"observed");
            using var controller=new ProductionHumanOperatorPanel(frame,Locale(),Packs(),()=>state,(_,__)=>{starts++;return Task.CompletedTask;},()=>Task.CompletedTask,()=>Task.CompletedTask,publishNative:false);
            yield return null;yield return null;
            var scroll=frame.Q<ScrollView>("human-operator-packs");var first=frame.Q<Button>("human-operator-pack-campaign-pack-01.json");var choose=frame.Q<Button>("human-operator-choose");
            Assert.That(scroll.contentViewport.worldBound.height,Is.GreaterThan(200));Assert.That(first.worldBound.height,Is.GreaterThanOrEqualTo(44));Assert.That(first.worldBound.width,Is.GreaterThan(300));
            Assert.That(choose.worldBound.yMax,Is.LessThanOrEqualTo(844));Assert.That(choose.worldBound.xMin,Is.GreaterThanOrEqualTo(0));Assert.That(choose.worldBound.xMax,Is.LessThanOrEqualTo(390));
            var previous=AssistiveSupport.activeHierarchy;controller.Accessibility.RefreshForInspection();Assert.That(AssistiveSupport.activeHierarchy,Is.SameAs(previous));
            Assert.That(Nodes(controller.Accessibility.Hierarchy).Where(n=>n.isActive).Any(n=>n.label.Contains("S1-")||n.label.Contains("E1-")||n.label.Contains("seed")||n.label.Contains("Spalte")),Is.False);
            Assert.That(controller.Accessibility.TryActivate(first),Is.True);Assert.That(controller.Accessibility.TryActivate(choose),Is.True);yield return null;controller.Accessibility.RefreshForInspection();
            Assert.That(controller.IsConfirmationOpen,Is.True);Assert.That(controller.Accessibility.TryActivate(first),Is.False);Assert.That(starts,Is.Zero);
            controller.SetExternalSuspended(true);Assert.That(Nodes(controller.Accessibility.Hierarchy).All(n=>!n.isActive),Is.True);Assert.That(controller.Accessibility.TryActivate(frame.Q<Button>("human-operator-confirm-start")),Is.False);
            controller.SetExternalSuspended(false);yield return null;Assert.That(controller.Accessibility.TryActivate(frame.Q<Button>("human-operator-confirm-cancel")),Is.True);Assert.That(starts,Is.Zero);
        }

        private static IEnumerable<AccessibilityNode> Nodes(AccessibilityHierarchy hierarchy)
        {foreach(var node in hierarchy.rootNodes)foreach(var entry in Branch(node))yield return entry;}
        private static IEnumerable<AccessibilityNode> Branch(AccessibilityNode node)
        {yield return node;foreach(var child in node.children)foreach(var entry in Branch(child))yield return entry;}
        private static bool Presented(VisualElement element)
        {for(var current=element;current!=null;current=current.parent)if(current.resolvedStyle.display==DisplayStyle.None||current.style.display.value==DisplayStyle.None)return false;return true;}
        private static void Submit(Button button){using var e=NavigationSubmitEvent.GetPooled();e.target=button;button.SendEvent(e);}
        private sealed class Phone:IDisposable
        {
            private readonly GameObject go;private readonly PanelSettings panel;private readonly RenderTexture target;
            internal Phone()
            {
                panel=ScriptableObject.CreateInstance<PanelSettings>();panel.themeStyleSheet=AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(Ui+"DevelopmentPuzzleTheme.tss");panel.scaleMode=PanelScaleMode.ConstantPixelSize;
                target=new RenderTexture(390,844,24);target.Create();panel.targetTexture=target;go=new GameObject("HumanOperatorPhone");var document=go.AddComponent<UIDocument>();document.panelSettings=panel;Root=document.rootVisualElement;
            }
            internal VisualElement Root{get;}
            public void Dispose(){panel.targetTexture=null;target.Release();UnityEngine.Object.Destroy(target);UnityEngine.Object.Destroy(go);UnityEngine.Object.Destroy(panel);}
        }
#endif
        private static IReadOnlyList<HumanOperatorPack> Packs()
        {var result=new List<HumanOperatorPack>();for(var group=0;group<2;group++)for(var i=1;i<=(group==0?9:8);i++)result.Add(new HumanOperatorPack((group==0?"campaign":"endless")+"-pack-"+i.ToString("00",System.Globalization.CultureInfo.InvariantCulture)+".json",6));return result.AsReadOnly();}
    }
}
