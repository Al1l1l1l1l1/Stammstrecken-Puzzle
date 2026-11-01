using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>Fits the result sheet to its visible content, retaining a bounded scrollable area on short screens.</summary>
    public sealed class ProductionResultSheet:IDisposable
    {
        private readonly ScrollView sheet;
        private readonly VisualElement owner;
        private IVisualElementScheduledItem? pending;
        private bool disposed;
        /// <summary>Observes only the sheet's own geometry; the puzzle board keeps its independent fixed viewport.</summary>
        public ProductionResultSheet(ScrollView sheet,VisualElement owner)
        {
            this.sheet=sheet??throw new ArgumentNullException(nameof(sheet));this.owner=owner??throw new ArgumentNullException(nameof(owner));
            owner.RegisterCallback<GeometryChangedEvent>(OnGeometry);sheet.contentContainer.RegisterCallback<GeometryChangedEvent>(OnGeometry);sheet.RegisterCallback<GeometryChangedEvent>(OnGeometry);
        }
        /// <summary>Measures after layout has applied current labels and optional controls.</summary>
        public void Refresh(){if(disposed||pending!=null)return;pending=sheet.schedule.Execute(()=>{pending=null;Measure();});}
        /// <summary>Releases only owned callbacks and scheduled measurement.</summary>
        public void Dispose(){if(disposed)return;disposed=true;pending?.Pause();pending=null;owner.UnregisterCallback<GeometryChangedEvent>(OnGeometry);sheet.contentContainer.UnregisterCallback<GeometryChangedEvent>(OnGeometry);sheet.UnregisterCallback<GeometryChangedEvent>(OnGeometry);}
        private void OnGeometry(GeometryChangedEvent change)=>Refresh();
        private void Measure()
        {
            if(disposed||sheet.panel==null||sheet.resolvedStyle.display==DisplayStyle.None||owner.resolvedStyle.display==DisplayStyle.None)return;
            var limit=Mathf.Min(720,owner.contentRect.height*.82f);if(!Valid(limit)||limit<=0)return;
            var bottom=0f;
            foreach(var child in sheet.contentContainer.Children())if(child.resolvedStyle.display!=DisplayStyle.None)
            {var end=child.layout.yMax+child.resolvedStyle.marginBottom;if(Valid(end))bottom=Mathf.Max(bottom,end);}
            if(bottom<=0)return;
            var style=sheet.resolvedStyle;var chrome=style.paddingTop+style.paddingBottom+style.borderTopWidth+style.borderBottomWidth;
            var desired=Mathf.Min(limit,bottom+chrome);if(!Valid(desired)||desired<=0)return;
            if(!Valid(sheet.resolvedStyle.height)||Mathf.Abs(sheet.resolvedStyle.height-desired)>.5f)sheet.style.height=desired;
        }
        private static bool Valid(float value)=>!float.IsNaN(value)&&!float.IsInfinity(value);
    }
}
