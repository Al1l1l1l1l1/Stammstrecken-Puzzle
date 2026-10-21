using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>Runs one display acknowledgement only after an attached, visible help panel has real message geometry.</summary>
    public sealed class HintDisplayGate:IDisposable
    {
        private readonly VisualElement panel;
        private readonly Label message;
        private readonly Func<bool>? eligible;
        private IVisualElementScheduledItem? scheduled;
        private Action? acknowledge;
        private bool disposed;
        /// <summary>Observes only the supplied panel/message; it owns no save or gameplay authority.</summary>
        public HintDisplayGate(VisualElement panel,Label message,Func<bool>? eligible=null)
        {
            this.panel=panel??throw new ArgumentNullException(nameof(panel));this.message=message??throw new ArgumentNullException(nameof(message));
            this.eligible=eligible;
            panel.RegisterCallback<AttachToPanelEvent>(OnAttach);panel.RegisterCallback<GeometryChangedEvent>(OnGeometry);message.RegisterCallback<GeometryChangedEvent>(OnGeometry);
        }
        /// <summary>Arms a single acknowledgement for the currently displayed help offer.</summary>
        public void Arm(Action displayed)
        {if(disposed)throw new ObjectDisposedException(nameof(HintDisplayGate));Cancel();acknowledge=displayed??throw new ArgumentNullException(nameof(displayed));Schedule();}
        /// <summary>Cancels an undisplayed offer without acknowledging it.</summary>
        public void Cancel(){scheduled?.Pause();scheduled=null;acknowledge=null;}
        /// <summary>Rechecks an armed offer when its external presentation fence changes.</summary>
        public void Recheck()=>Schedule();
        /// <summary>Releases callbacks and prevents any late display acknowledgement.</summary>
        public void Dispose()
        {if(disposed)return;disposed=true;Cancel();panel.UnregisterCallback<AttachToPanelEvent>(OnAttach);panel.UnregisterCallback<GeometryChangedEvent>(OnGeometry);message.UnregisterCallback<GeometryChangedEvent>(OnGeometry);}
        private void OnAttach(AttachToPanelEvent e)=>Schedule();
        private void OnGeometry(GeometryChangedEvent e)=>Schedule();
        private void Schedule()
        {if(disposed||acknowledge==null||scheduled!=null)return;scheduled=panel.schedule.Execute(TryDisplay).StartingIn(16);}
        private void TryDisplay()
        {
            scheduled=null;if(disposed||acknowledge==null)return;
            if(eligible?.Invoke()==false||panel.panel==null||message.panel!=panel.panel||!panel.Contains(message)||string.IsNullOrWhiteSpace(message.text)||!Positive(message.worldBound)||!Positive(panel.worldBound))return;
            // Padding and glyph bounds can remain nonzero when the actual text content box has no area.
            var content=message.contentRect;if(!Positive(content))return;
            var worldContent=message.LocalToWorld(content);if(!Positive(worldContent))return;
            var visible=Intersect(worldContent,panel.worldBound);
            for(VisualElement? ancestor=message;ancestor!=null;ancestor=ancestor.parent)
            {
                if(ancestor.resolvedStyle.display==DisplayStyle.None||ancestor.resolvedStyle.visibility==Visibility.Hidden||ancestor.resolvedStyle.opacity<=0)return;
                if(ancestor.style.overflow.value==Overflow.Hidden||ancestor.ClassListContains("unity-scroll-view__content-viewport")||ancestor.ClassListContains("production-root"))visible=Intersect(visible,ancestor.worldBound);
            }
            if(!Positive(visible))return;
            var action=acknowledge;acknowledge=null;action();
        }
        private static bool Positive(Rect rect)=>Finite(rect.x)&&Finite(rect.y)&&Finite(rect.width)&&Finite(rect.height)&&rect.width>0&&rect.height>0;
        private static bool Finite(float value)=>!float.IsNaN(value)&&!float.IsInfinity(value);
        private static Rect Intersect(Rect a,Rect b)
        {var x=Mathf.Max(a.xMin,b.xMin);var y=Mathf.Max(a.yMin,b.yMin);return new Rect(x,y,Mathf.Max(0,Mathf.Min(a.xMax,b.xMax)-x),Mathf.Max(0,Mathf.Min(a.yMax,b.yMax)-y));}
    }
}
