namespace STP.Bootstrap
{
    /// <summary>
    /// Unveränderliche Konfiguration des WP-021-QA-Scaffolds. Kein Productionprofil,
    /// keine Dienst-IDs oder Secrets; die finale Storeidentität wird hier nicht festgelegt.
    /// </summary>
    public sealed class BootstrapConfiguration
    {
        /// <summary>Explizites Nicht-Production-Profil gemäß BUILD_AND_RELEASE.md Abschnitt 2.</summary>
        public string Profile { get; } = "qa";

        /// <summary>Getrennte interne Android-/iOS-Identität des QA-Scaffolds.</summary>
        public string ApplicationIdentifier { get; } = "com.STP.StammstreckenPuzzle.qa";
    }
}
