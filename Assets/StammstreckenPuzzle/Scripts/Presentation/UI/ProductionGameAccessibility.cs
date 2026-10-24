using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>Own native screen-reader hierarchy over actual presented controls, without a second gameplay input path.</summary>
    public sealed class ProductionGameAccessibility:IDisposable
    {
        private readonly int ownerThread=Thread.CurrentThread.ManagedThreadId;
        private readonly VisualElement root;
        private readonly Func<string,object[],string> localize;
        private readonly bool publishNative;
        private readonly Dictionary<VisualElement,AccessibilityNode> nodes=new Dictionary<VisualElement,AccessibilityNode>();
        private readonly IVisualElementScheduledItem scheduled;
        private VisualElement? scope;
        private bool suspended,disposed;
        /// <summary>Creates an isolated hierarchy. It becomes native-active only while the system screen reader is enabled.</summary>
        public ProductionGameAccessibility(VisualElement root,Func<string,object[],string> localize,bool publishNative=true)
        {
            this.root=root??throw new ArgumentNullException(nameof(root));this.localize=localize??throw new ArgumentNullException(nameof(localize));
            this.publishNative=publishNative;
            Hierarchy=new AccessibilityHierarchy();AssistiveSupport.screenReaderStatusChanged+=OnReaderChanged;
            root.RegisterCallback<AttachToPanelEvent>(OnAttached);root.RegisterCallback<DetachFromPanelEvent>(OnDetached);
            scheduled=root.schedule.Execute(Tick).Every(150);
        }
        /// <summary>Own semantic hierarchy; inspecting it does not enable or override the operating system screen reader.</summary>
        public AccessibilityHierarchy Hierarchy{get;}
        /// <summary>Root supplies native-provider and lifecycle suspension independently of in-game modal attention.</summary>
        public void SetSuspended(bool value)
        {
            CheckThread();if(disposed||suspended==value)return;suspended=value;
            if(value){foreach(var node in nodes.Values)node.isActive=false;ReleaseNative();}else Tick();
        }
        /// <summary>Refreshes semantic geometry without claiming a native-device or spoken-output check.</summary>
        public void RefreshForInspection(){CheckThread();if(!disposed)Refresh(false);}
        /// <summary>Routes the native activation through the same guarded UI submit event as touch and keyboard.</summary>
        public bool TryActivate(Button button)
        {
            if(Thread.CurrentThread.ManagedThreadId!=ownerThread||disposed||suspended||button==null||!button.enabledInHierarchy||!IsPresented(button)||!InCurrentScope(button))return false;
            using var submit=NavigationSubmitEvent.GetPooled();submit.target=button;button.SendEvent(submit);return true;
        }
        /// <summary>Removes only the hierarchy currently owned by this presenter; foreign/native views are left alone.</summary>
        public void Dispose()
        {
            CheckThread();if(disposed)return;disposed=true;scheduled.Pause();AssistiveSupport.screenReaderStatusChanged-=OnReaderChanged;root.UnregisterCallback<AttachToPanelEvent>(OnAttached);root.UnregisterCallback<DetachFromPanelEvent>(OnDetached);ReleaseNative();Hierarchy.Clear();nodes.Clear();scope=null;
        }
        private void Tick()
        {
            if(Thread.CurrentThread.ManagedThreadId!=ownerThread||disposed||!publishNative||!AssistiveSupport.isScreenReaderEnabled)return;
            try{Refresh(true);}catch{ /* Optional native semantics cannot stop offline gameplay or a save. */ }
        }
        private void OnReaderChanged(bool enabled){if(enabled)Tick();}
        private void OnAttached(AttachToPanelEvent change)=>Tick();
        private void OnDetached(DetachFromPanelEvent change){foreach(var node in nodes.Values)node.isActive=false;ReleaseNative();}
        private void Refresh(bool publish)
        {
            if(suspended||root.panel==null||!IsDisplayed(root)){foreach(var node in nodes.Values)node.isActive=false;ReleaseNative();return;}
            var previous=scope;scope=FindScope();var visited=new HashSet<VisualElement>();var changed=false;
            Visit(scope,null,visited,ref changed);
            var detached=new List<VisualElement>();
            foreach(var pair in nodes)
            {
                if(!visited.Contains(pair.Key)&&pair.Value.isActive){pair.Value.isActive=false;changed=true;}
                if(pair.Key!=root&&!root.Contains(pair.Key))detached.Add(pair.Key);
            }
            foreach(var element in detached){Hierarchy.RemoveNode(nodes[element],false);nodes.Remove(element);changed=true;}
            if(publish&&AssistiveSupport.isScreenReaderEnabled)
            {
                if(AssistiveSupport.activeHierarchy!=null&&!ReferenceEquals(AssistiveSupport.activeHierarchy,Hierarchy))return;
                var activated=!ReferenceEquals(AssistiveSupport.activeHierarchy,Hierarchy);
                if(activated)AssistiveSupport.activeHierarchy=Hierarchy;
                if(activated||previous!=scope)AssistiveSupport.notificationDispatcher.SendScreenChanged();
                else if(changed)AssistiveSupport.notificationDispatcher.SendLayoutChanged();
            }
        }
        private void Visit(VisualElement element,AccessibilityNode? parent,HashSet<VisualElement> visited,ref bool changed)
        {
            if(!IsDisplayed(element))return;
            if(element is Button button)
            {
                var label=string.IsNullOrWhiteSpace(button.text)?button.tooltip:button.text;
                if(!string.IsNullOrWhiteSpace(button.tooltip)&&(button.ClassListContains("puzzle-cell")||button.ClassListContains("tool-button")||button.text=="‹"))label=button.tooltip;
                if(!string.IsNullOrWhiteSpace(label))Update(element,label,AccessibilityRole.Button,parent,visited,ref changed);
                return;
            }
            if(element is ScrollView scroll)
            {
                var node=Update(element,localize("accessibility.scroll",Array.Empty<object>()),AccessibilityRole.ScrollView,parent,visited,ref changed);
                foreach(var child in scroll.contentContainer.Children())Visit(child,node,visited,ref changed);return;
            }
            if(element is Label text&&!string.IsNullOrWhiteSpace(text.text))
            {
                var label=(text.ClassListContains("puzzle-count")||text.ClassListContains("puzzle-endpoint"))&&!string.IsNullOrWhiteSpace(text.tooltip)?text.tooltip:text.text;
                var heading=text.ClassListContains("page-title")||text.ClassListContains("hero-title")||text.ClassListContains("result-title")||text.ClassListContains("settings-heading");
                Update(text,label,heading?AccessibilityRole.Header:AccessibilityRole.StaticText,parent,visited,ref changed);return;
            }
            foreach(var child in element.Children())Visit(child,parent,visited,ref changed);
        }
        private AccessibilityNode Update(VisualElement element,string label,AccessibilityRole role,AccessibilityNode? parent,HashSet<VisualElement> visited,ref bool changed)
        {
            visited.Add(element);
            if(!nodes.TryGetValue(element,out var node))
            {
                node=Hierarchy.AddNode(label,parent);nodes.Add(element,node);node.frameGetter=()=>ScreenFrame(element);changed=true;
                if(element is Button button)node.invoked+=()=>TryActivate(button);
                if(element is ScrollView scroll)node.scrolled+=direction=>Scroll(scroll,direction);
            }
            if(node.parent!=parent){Hierarchy.MoveNode(node,parent);changed=true;}
            var active=IsPresented(element);var state=element.enabledInHierarchy?AccessibilityState.None:AccessibilityState.Disabled;
            if(element.ClassListContains("tool--active")||element.ClassListContains("settings-choice--active")||element.ClassListContains("nav--active"))state|=AccessibilityState.Selected;
            var frame=ScreenFrame(element);
            if(node.label!=label||node.role!=role||node.state!=state||node.isActive!=active||node.frame!=frame)changed=true;
            node.label=label;node.role=role;node.state=state;node.isActive=active;node.frame=frame;return node;
        }
        private bool Scroll(ScrollView scroll,AccessibilityScrollDirection direction)
        {
            if(Thread.CurrentThread.ManagedThreadId!=ownerThread||disposed||suspended||!IsPresented(scroll)||!InCurrentScope(scroll))return false;
            var offset=scroll.scrollOffset;var before=offset;var page=Mathf.Max(44,scroll.contentViewport.contentRect.height*.75f);
            switch(direction)
            {
                case AccessibilityScrollDirection.Down:case AccessibilityScrollDirection.Forward:offset.y=Mathf.Min(scroll.verticalScroller.highValue,offset.y+page);break;
                case AccessibilityScrollDirection.Up:case AccessibilityScrollDirection.Backward:offset.y=Mathf.Max(scroll.verticalScroller.lowValue,offset.y-page);break;
                case AccessibilityScrollDirection.Right:offset.x=Mathf.Min(scroll.horizontalScroller.highValue,offset.x+page);break;
                case AccessibilityScrollDirection.Left:offset.x=Mathf.Max(scroll.horizontalScroller.lowValue,offset.x-page);break;
                default:return false;
            }
            if(offset==before)return false;scroll.scrollOffset=offset;Tick();return true;
        }
        private VisualElement FindScope()
        {
            var found=root;
            foreach(var candidate in root.Query<VisualElement>(className:"overlay").ToList())if(IsDisplayed(candidate))found=candidate;
            if(found==root){var result=root.Q<ScrollView>("result-panel");if(result!=null&&IsDisplayed(result))found=result;}
            return found;
        }
        private bool InCurrentScope(VisualElement element){var actual=FindScope();return actual==element||actual.Contains(element);}
        private static bool IsDisplayed(VisualElement element)
        {
            if(element.panel==null)return false;
            for(var current=element;current!=null;current=current.parent)if(current.resolvedStyle.display==DisplayStyle.None||current.resolvedStyle.visibility==Visibility.Hidden)return false;
            return true;
        }
        private static bool IsPresented(VisualElement element)=>IsDisplayed(element)&&VisibleFrame(element).width>0&&VisibleFrame(element).height>0;
        private static Rect VisibleFrame(VisualElement element)
        {
            var rect=element is ScrollView scroll?scroll.contentViewport.worldBound:element.worldBound;
            for(var current=element.parent;current!=null;current=current.parent)
                if(current.ClassListContains("unity-scroll-view__content-viewport")||current.ClassListContains("production-root")||current.style.overflow.value==Overflow.Hidden)rect=Intersect(rect,current.worldBound);
            return rect;
        }
        private static Rect ScreenFrame(VisualElement element)
        {if(element.panel==null)return Rect.zero;var rect=VisibleFrame(element);var scale=element.panel.scaledPixelsPerPoint;return new Rect(rect.position*scale,rect.size*scale);}
        private static Rect Intersect(Rect a,Rect b)
        {var x=Mathf.Max(a.xMin,b.xMin);var y=Mathf.Max(a.yMin,b.yMin);return new Rect(x,y,Mathf.Max(0,Mathf.Min(a.xMax,b.xMax)-x),Mathf.Max(0,Mathf.Min(a.yMax,b.yMax)-y));}
        private void ReleaseNative(){if(ReferenceEquals(AssistiveSupport.activeHierarchy,Hierarchy))AssistiveSupport.activeHierarchy=null;}
        private void CheckThread(){if(Thread.CurrentThread.ManagedThreadId!=ownerThread)throw new InvalidOperationException("PRODUCT_ACCESSIBILITY_WRONG_THREAD");}
    }
}
