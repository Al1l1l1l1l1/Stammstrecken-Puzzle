using System;
using STP.Application;

namespace STP.Presentation.UI
{
    /// <summary>
    /// WP-021-Skelett der UI-Präsentation gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 3
    /// (UI Toolkit Screens, Presenter, Custom Grid und Navigation). Ohne Screens, Presenter
    /// oder Fachlogik; dient ausschließlich dem vollständigen Composition Graph.
    /// </summary>
    public sealed class UiPresentationSkeleton
    {
        /// <summary>Verdrahtet die UI mit der zuvor erstellten ApplicationRoot.</summary>
        public UiPresentationSkeleton(ApplicationRoot application)
        {
            this.Application = application ?? throw new ArgumentNullException(nameof(application));
        }

        /// <summary>Die von Bootstrap explizit injizierte ApplicationRoot.</summary>
        public ApplicationRoot Application { get; }
    }
}
