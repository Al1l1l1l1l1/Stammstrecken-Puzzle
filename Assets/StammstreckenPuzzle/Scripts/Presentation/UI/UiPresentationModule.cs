using System;
using STP.Application;

namespace STP.Presentation.UI
{
    /// <summary>
    /// WP-008-Praesentations-Skelett des UI-Moduls. UI-Toolkit-Screens, Presenter,
    /// Custom Grid und Navigation folgen mit WP-009; dieses Skelett belegt nur die
    /// vollstaendige Verdrahtung des Application-Graphen in der Praesentation.
    /// </summary>
    public sealed class UiPresentationModule
    {
        /// <summary>
        /// Erstellt das UI-Modul mit der Application-Wurzel.
        /// </summary>
        public UiPresentationModule(ApplicationRoot application)
        {
            Application = application ?? throw new ArgumentNullException(nameof(application));
        }

        /// <summary>Die verdrahtete Application-Wurzel.</summary>
        public ApplicationRoot Application { get; }
    }
}
