using System;
using STP.Application.Composition;

namespace STP.Application
{
    /// <summary>
    /// WP-008-Composition-Skelett des Application-Einstiegs. Fachliche Use Cases,
    /// Policies, Read Models und Orchestrierung folgen mit WP-009; diese Wurzel
    /// traegt ausschliesslich die gebundene Port-Composition.
    /// </summary>
    public sealed class ApplicationRoot
    {
        /// <summary>
        /// Erstellt die Application-Wurzel mit der vollstaendig gebundenen Composition.
        /// </summary>
        public ApplicationRoot(ApplicationComposition composition)
        {
            Composition = composition ?? throw new ArgumentNullException(nameof(composition));
        }

        /// <summary>Die gebundene Port-Composition dieser Anwendung.</summary>
        public ApplicationComposition Composition { get; }
    }
}
