using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>Observes one actual displayed board after durable exposure permission; it grants no permission itself.</summary>
    public sealed class ProductionExposureGate:IDisposable
    {
        private readonly VisualElement screen;
        private readonly VisualElement board;
        private readonly Func<bool> eligible;
        private IVisualElementScheduledItem? scheduled;
        private Action? acknowledge;
        private bool disposed;
        public ProductionExposureGate(VisualElement screen,VisualElement board,Func<bool> eligible)
        {this.screen=screen??throw new ArgumentNullException(nameof(screen));this.board=board??throw new ArgumentNullException(nameof(board));this.eligible=eligible??throw new ArgumentNullException(nameof(eligible));}
        public void Arm(Action displayed)
        {
            if(disposed)throw new ObjectDisposedException(nameof(ProductionExposureGate));Cancel();acknowledge=displayed??throw new ArgumentNullException(nameof(displayed));
            scheduled=screen.schedule.Execute(TryDisplay).StartingIn(32).Every(16);
        }
        public void Cancel(){scheduled?.Pause();scheduled=null;acknowledge=null;}
        public void Dispose(){if(disposed)return;disposed=true;Cancel();}
        private void TryDisplay()
        {
            if(disposed||acknowledge==null||!eligible()||screen.panel==null||board.panel!=screen.panel||!screen.Contains(board))return;
            for(VisualElement? element=board;element!=null;element=element.parent)
                if(element.resolvedStyle.display==DisplayStyle.None||element.resolvedStyle.visibility==Visibility.Hidden||element.resolvedStyle.opacity<=0)return;
            var rect=board.worldBound;var owner=screen.worldBound;
            if(!Finite(rect.width)||!Finite(rect.height)||rect.width<=1||rect.height<=1||!rect.Overlaps(owner))return;
            var action=acknowledge;Cancel();action();
        }
        private static bool Finite(float value)=>!float.IsNaN(value)&&!float.IsInfinity(value);
    }
}
