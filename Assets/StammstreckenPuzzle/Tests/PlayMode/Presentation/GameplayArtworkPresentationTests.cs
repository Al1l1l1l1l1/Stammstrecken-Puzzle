using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// <summary>Actual panel geometry and semantic controls; these fixtures make no native-device or Human observation claim.</summary>
    public sealed class GameplayArtworkPresentationTests
    {
#if UNITY_EDITOR
        private const string Ui="Assets/StammstreckenPuzzle/UI/";
        private sealed class Phone:IDisposable
        {
            internal readonly VisualElement Frame;
            internal readonly ProductionPuzzleGrid Grid;
            internal readonly object Session;
            internal readonly ProductionGameLocalization Locale;
            internal readonly PanelSettings Panel;
            internal readonly RenderTexture Texture;
            internal readonly GameObject Owner;
            internal readonly ScrollView Viewport;
            internal readonly VisualElement DocumentRoot;
            private readonly Vector2Int phoneSize;
            private readonly Vector2Int shape;
            internal Phone(int width,int height,int columns,int rows)
            {
                phoneSize=new Vector2Int(width,height);shape=new Vector2Int(columns,rows);
                Locale=new ProductionGameLocalization(AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+"ProductionGameLocale.json"));
                Frame=AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Ui+"ProductionGame.uxml").CloneTree();
                Frame.style.width=width;Frame.style.height=height;Frame.style.flexShrink=0;
                Frame.Q("home-screen").style.display=DisplayStyle.None;Frame.Q("main-navigation").style.display=DisplayStyle.None;
                Frame.Q("startup-overlay").style.display=DisplayStyle.None;Frame.Q("puzzle-screen").style.display=DisplayStyle.Flex;
                foreach(var label in Frame.Query<Label>().ToList())if(!string.IsNullOrEmpty(label.viewDataKey))label.text=Locale.Resolve(label.viewDataKey);
                for(int i=0;i<9;i++)Frame.Q<Button>("tool-"+i).tooltip=Locale.Resolve(new[]{"tool.clear","tool.empty","tool.occupied","tool.ns","tool.ew","tool.ne","tool.es","tool.sw","tool.wn"}[i]);
                Session=CreateSession(columns,rows);
                Grid=(ProductionPuzzleGrid)typeof(ProductionPuzzleGrid).GetConstructors().Single().Invoke(new object[]{Session,(Func<string,object[],string>)Locale.Resolve,(Action<int,int>)((_,__)=>{})});
                Viewport=Frame.Q<ScrollView>("puzzle-viewport");
                // Match the actual presenter: hidden scrollers and measured outer viewport, not the scroll child's transient area.
                Viewport.horizontalScrollerVisibility=ScrollerVisibility.Hidden;Viewport.verticalScrollerVisibility=ScrollerVisibility.Hidden;
                Viewport.Add(Grid);Grid.Refresh(true);
                Panel=ScriptableObject.CreateInstance<PanelSettings>();Panel.themeStyleSheet=AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(Ui+"DevelopmentPuzzleTheme.tss");Panel.scaleMode=PanelScaleMode.ConstantPixelSize;
                Panel.referenceResolution=new Vector2Int(width,height);
                Texture=new RenderTexture(width,height,24);Texture.Create();Panel.targetTexture=Texture;
                Owner=new GameObject("ActualGameplayArtworkGeometry");var document=Owner.AddComponent<UIDocument>();document.panelSettings=Panel;
                DocumentRoot=document.rootVisualElement;DocumentRoot.style.width=width;DocumentRoot.style.height=height;DocumentRoot.Add(Frame);
            }
            internal void Fit()
            {
                var r=Viewport.contentRect;Assert.That(r.width,Is.GreaterThan(200),Diagnostics);Assert.That(r.height,Is.GreaterThanOrEqualTo(120),Diagnostics);
                Viewport.contentContainer.style.width=r.width;Viewport.contentContainer.style.height=r.height;Grid.style.width=r.width;Grid.style.height=r.height;
            }
            internal string Diagnostics=>"Phone="+phoneSize+" Shape="+shape+" Document="+DocumentRoot.contentRect+" Frame="+Frame.contentRect+
                " ProductionRoot="+Frame.Q("production-root").contentRect+" PuzzleScreen="+Frame.Q("puzzle-screen").contentRect+
                " Viewport="+Viewport.contentRect+" ContentViewport="+Viewport.contentViewport.contentRect+" Grid="+Grid.contentRect+
                " Board="+Grid.Board.layout+" Artwork="+Grid.WorksiteIllustrationBounds;
            internal void AssertActualPhoneGeometry()
            {
                Assert.That(DocumentRoot.contentRect.width,Is.EqualTo(phoneSize.x).Within(.5f),Diagnostics);
                Assert.That(DocumentRoot.contentRect.height,Is.EqualTo(phoneSize.y).Within(.5f),Diagnostics);
                Assert.That(Frame.Q("puzzle-screen").contentRect.height,Is.EqualTo(phoneSize.y).Within(.5f),Diagnostics);
                Assert.That(Grid.contentRect.width,Is.EqualTo(Viewport.contentRect.width).Within(.5f),Diagnostics);
                Assert.That(Grid.contentRect.height,Is.EqualTo(Viewport.contentRect.height).Within(.5f),Diagnostics);
                TestContext.WriteLine(Diagnostics);
            }
            public void Dispose(){Grid.Dispose();Locale.Dispose();Panel.targetTexture=null;Texture.Release();UnityEngine.Object.Destroy(Texture);UnityEngine.Object.Destroy(Owner);UnityEngine.Object.Destroy(Panel);}
        }

        [UnityTest]
        public IEnumerator Worksite_4x4_6x5_10x10_OnlyUsesFreeSpace_AndNeverMovesSemanticTargets()
        {
            foreach(var size in new[]{new Vector2Int(390,844),new Vector2Int(320,568)})
            foreach(var shape in new[]{new Vector2Int(4,4),new Vector2Int(6,5),new Vector2Int(10,10)})
            {
                using var phone=new Phone(size.x,size.y,shape.x,shape.y);
                yield return null;yield return null;phone.Fit();yield return null;yield return null;
                phone.AssertActualPhoneGeometry();
                var grid=phone.Grid;var initial=AllTargetBounds(grid);var tools=ToolBounds(phone.Frame);var art=grid.WorksiteIllustrationBounds;
                Assert.That(grid.Query<Button>().ToList().Count,Is.EqualTo(shape.x*shape.y),"Paint must add no input target. "+phone.Diagnostics);
                if(size.x==390){Assert.That(art.height,Is.GreaterThanOrEqualTo(48),phone.Diagnostics);Assert.That(grid.CellButton(0,0).worldBound.width,Is.GreaterThan(shape.x==10?28:45),phone.Diagnostics);}
                if(art.height>0)
                {
                    var world=grid.LocalToWorld(art);Assert.That(world.yMin-grid.Board.worldBound.yMax,Is.GreaterThanOrEqualTo(33.9f));
                    Assert.That(world.xMin,Is.GreaterThanOrEqualTo(grid.worldBound.xMin));Assert.That(world.xMax,Is.LessThanOrEqualTo(grid.worldBound.xMax));Assert.That(world.yMax,Is.LessThanOrEqualTo(grid.worldBound.yMax-7.9f));
                    foreach(var target in grid.Query<Button>().ToList().Cast<VisualElement>().Concat(grid.Query<Label>().ToList()))Assert.That(world.Overlaps(target.worldBound),Is.False,target.name);
                }
                Apply(phone.Session,1,1,3);grid.Refresh(true);yield return null;grid.Refresh(false);yield return null;grid.Refresh(true);yield return null;
                CollectionAssert.AreEqual(initial,AllTargetBounds(grid));CollectionAssert.AreEqual(tools,ToolBounds(phone.Frame));Assert.That(grid.WorksiteIllustrationBounds,Is.EqualTo(art));
                phone.Session.GetType().GetMethod("Undo",new[]{typeof(long)})!.Invoke(phone.Session,new object[]{2000L});grid.Refresh(true);yield return null;
                CollectionAssert.AreEqual(initial,AllTargetBounds(grid));
                grid.style.height=120;yield return null;yield return null;Assert.That(grid.WorksiteIllustrationBounds,Is.EqualTo(Rect.zero),"Tight layouts must hide decoration rather than shrink cells to make room. "+phone.Diagnostics);
            }
        }

        [UnityTest]
        public IEnumerator Artwork_HintHelpResultAndSettings_DoNotMoveTools_OrAddSemanticControls()
        {
            using var phone=new Phone(390,844,6,5);yield return null;yield return null;phone.Fit();yield return null;yield return null;
            phone.AssertActualPhoneGeometry();
            var grid=phone.Grid;var targets=AllTargetBounds(grid);var tools=ToolBounds(phone.Frame);var art=grid.WorksiteIllustrationBounds;
            using var semantics=new ProductionGameAccessibility(phone.Frame,phone.Locale.Resolve,false);semantics.RefreshForInspection();
            Assert.That(Nodes(semantics.Hierarchy).Any(n=>n.label==grid.CellButton(0,0).tooltip),Is.True);
            Assert.That(Nodes(semantics.Hierarchy).Count(n=>n.role==AccessibilityRole.Button&&n.label.StartsWith("Spalte ",StringComparison.Ordinal)),Is.EqualTo(30));
            foreach(string name in new[]{"hint-overlay","notice-overlay","settings-overlay","result-panel"})
            {
                var modal=phone.Frame.Q(name);grid.Refresh(false);modal.style.display=DisplayStyle.Flex;yield return null;yield return null;semantics.RefreshForInspection();
                CollectionAssert.AreEqual(targets,AllTargetBounds(grid),name);CollectionAssert.AreEqual(tools,ToolBounds(phone.Frame),name);Assert.That(grid.WorksiteIllustrationBounds,Is.EqualTo(art));
                Assert.That(semantics.TryActivate(grid.CellButton(0,0)),Is.False,name);
                modal.style.display=DisplayStyle.None;grid.Refresh(true);yield return null;
            }
            var mark=phone.Frame.Q("puzzle-operating-mark");Assert.That(mark.pickingMode,Is.EqualTo(PickingMode.Ignore));Assert.That(mark.focusable,Is.False);
            Assert.That(phone.Frame.Q<Button>("human-unsure").ClassListContains("human-contact-option--last"),Is.True);
            Assert.That(phone.Frame.Q<Button>("human-seen").ClassListContains("human-contact-option--last"),Is.False);
        }

        private static Rect[] AllTargetBounds(ProductionPuzzleGrid grid)=>grid.Query<Button>().ToList().Cast<VisualElement>().Concat(grid.Query<Label>().ToList()).Select(e=>e.worldBound).ToArray();
        private static Rect[] ToolBounds(VisualElement frame)=>Enumerable.Range(0,9).Select(i=>frame.Q<Button>("tool-"+i).worldBound).ToArray();
        private static IEnumerable<AccessibilityNode> Nodes(AccessibilityHierarchy hierarchy){foreach(var root in hierarchy.rootNodes)foreach(var node in Branch(root))yield return node;}
        private static IEnumerable<AccessibilityNode> Branch(AccessibilityNode node){yield return node;foreach(var child in node.children)foreach(var entry in Branch(child))yield return entry;}
        private static object CreateSession(int width,int height)
        {
            var app=AppDomain.CurrentDomain.GetAssemblies().Single(a=>a.GetName().Name=="STP.Application");
            var identity=Activator.CreateInstance(app.GetType("STP.Application.PublicPuzzleIdentity")!,"artwork-geometry","STP-PUZZLE-SEMANTIC-JCS-1",new string('a',64))!;
            var side=app.GetType("STP.Application.PublicPuzzleSide")!;var rows=Enumerable.Repeat(1,height).ToArray();var columns=Enumerable.Repeat(1,width).ToArray();if(width>height)rows[0]+=width-height;
            var puzzle=Activator.CreateInstance(app.GetType("STP.Application.PublicPuzzleInput")!,identity,"train-track-v1",width,height,Enum.Parse(side,"W"),1,Enum.Parse(side,"S"),1,rows,columns)!;
            return Activator.CreateInstance(app.GetType("STP.Application.Public240ReviewSession")!,puzzle)!;
        }
        private static void Apply(object session,int x,int y,int content)
        {var tool=session.GetType().Assembly.GetType("STP.Application.DevelopmentTool")!;session.GetType().GetMethods().Single(m=>m.Name=="Apply"&&m.GetParameters().Length==4).Invoke(session,new[]{(object)x,y,Enum.ToObject(tool,content),1000L});}
#endif
    }
}
