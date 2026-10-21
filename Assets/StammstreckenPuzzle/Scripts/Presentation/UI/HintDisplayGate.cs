using System;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>Runs one display acknowledgement only after an attached, visible help panel has real message geometry.</summary>
    public sealed class HintDisplayGate:IDisposable
    {
        private readonly VisualElement panel;
        private readonly Label message;
        private IVisualElementScheduledItem? scheduled;
        private Action? acknowledge;
        private bool disposed;
        /// <summary>Observes only the supplied panel/message; it owns no save or gameplay authority.</summary>
        public HintDisplayGate(VisualElement panel,Label message)
        {
            this.panel=panel??throw new ArgumentNullException(nameof(panel));this.message=message??throw new ArgumentNullException(nameof(message));
            panel.RegisterCallback<AttachToPanelEvent>(OnAttach);panel.RegisterCallback<GeometryChangedEvent>(OnGeometry);message.RegisterCallback<GeometryChangedEvent>(OnGeometry);
        }
        /// <summary>Arms a single acknowledgement for the currently displayed help offer.</summary>
        public void Arm(Action displayed)
        {if(disposed)throw new ObjectDisposedException(nameof(HintDisplayGate));Cancel();acknowledge=displayed??throw new ArgumentNullException(nameof(displayed));Schedule();}
        /// <summary>Cancels an undisplayed offer without acknowledging it.</summary>
        public void Cancel(){scheduled?.Pause();scheduled=null;acknowledge=null;}
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
            if(panel.panel==null||string.IsNullOrWhiteSpace(message.text)||message.worldBound.width<=0||message.worldBound.height<=0)return;
            for(VisualElement? ancestor=message;ancestor!=null;ancestor=ancestor.parent)
                if(ancestor.resolvedStyle.display==DisplayStyle.None||ancestor.resolvedStyle.visibility==Visibility.Hidden)return;
            var action=acknowledge;acknowledge=null;action();
        }
    }
}
