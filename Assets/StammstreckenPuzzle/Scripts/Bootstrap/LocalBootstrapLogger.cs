using System;
using UnityEngine;

namespace STP.Bootstrap
{
    /// <summary>
    /// Lokale Bootstrapdiagnose über den Plattformlog. Akzeptiert ausschließlich
    /// feste Composition-Meldungen ohne Nutzerdaten; kein Crash-/Analytics-Port,
    /// Dateizugriff, Provider oder allgemeines Loggingframework.
    /// </summary>
    public sealed class LocalBootstrapLogger
    {
        /// <summary>Bindet die zuvor erstellte Scaffoldkonfiguration ohne Fallback.</summary>
        public LocalBootstrapLogger(BootstrapConfiguration configuration)
        {
            this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>Die tatsächlich injizierte Konfiguration der Composition Root.</summary>
        public BootstrapConfiguration Configuration { get; }

        /// <summary>Lokale Startdiagnose vor Konstruktion des Contentmoduls.</summary>
        public void LogCompositionStarted() => Debug.Log($"STP Bootstrap [{this.Configuration.Profile}]: composition started.");

        /// <summary>Lokale Abschlussdiagnose nach vollständiger Portbindung.</summary>
        public void LogCompositionCompleted() => Debug.Log($"STP Bootstrap [{this.Configuration.Profile}]: composition completed; providers not initialized.");
    }
}
