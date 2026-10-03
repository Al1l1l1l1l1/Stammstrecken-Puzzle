using System;
using STP.Application;

namespace STP.Presentation.World
{
    /// <summary>
    /// WP-021-Skelett der Welt-Präsentation gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 3
    /// (Zugfahrt, Kartenwelt, 2D-/2.5D-Renderer und Animation). Ohne Renderer, Weltobjekte
    /// oder Fachlogik; dient ausschließlich dem vollständigen Composition Graph.
    /// </summary>
    public sealed class WorldPresentationSkeleton
    {
        /// <summary>Verdrahtet die Welt mit der zuvor erstellten ApplicationRoot.</summary>
        public WorldPresentationSkeleton(ApplicationRoot application)
        {
            this.Application = application ?? throw new ArgumentNullException(nameof(application));
        }

        /// <summary>Die von Bootstrap explizit injizierte ApplicationRoot.</summary>
        public ApplicationRoot Application { get; }
    }
}
