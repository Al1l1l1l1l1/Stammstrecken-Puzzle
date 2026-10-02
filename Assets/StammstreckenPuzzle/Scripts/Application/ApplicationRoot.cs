namespace STP.Application
{
    /// <summary>
    /// Einstiegspunkt der Application gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 7.
    /// Kapselt die fail-closed Portbindung; ohne Fachlogik (WP-021-Skelett).
    /// </summary>
    public sealed class ApplicationRoot
    {
        /// <summary>Erstellt die Root mit einer bereits vollständig geprüften Portbindung.</summary>
        public ApplicationRoot(ApplicationComposition composition)
        {
            this.Composition = composition ?? throw new System.ArgumentNullException(nameof(composition));
        }

        /// <summary>Die vollständige, fail-closed geprüfte Portbindung der Anwendung.</summary>
        public ApplicationComposition Composition { get; }
    }
}
