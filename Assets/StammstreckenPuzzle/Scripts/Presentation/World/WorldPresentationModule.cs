using System;
using STP.Application;

namespace STP.Presentation.World
{
    /// <summary>
    /// WP-008-Praesentations-Skelett des World-Moduls. Zugfahrt, Kartenwelt,
    /// 2D-/2.5D-Renderer und Animation folgen mit WP-009; dieses Skelett belegt nur
    /// die vollstaendige Verdrahtung des Application-Graphen in der Praesentation.
    /// </summary>
    public sealed class WorldPresentationModule
    {
        /// <summary>
        /// Erstellt das World-Modul mit der Application-Wurzel.
        /// </summary>
        public WorldPresentationModule(ApplicationRoot application)
        {
            Application = application ?? throw new ArgumentNullException(nameof(application));
        }

        /// <summary>Die verdrahtete Application-Wurzel.</summary>
        public ApplicationRoot Application { get; }
    }
}
