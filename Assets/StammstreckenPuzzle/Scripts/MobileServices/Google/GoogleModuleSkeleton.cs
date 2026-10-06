using STP.Application.Ports;

namespace STP.MobileServices.Google
{
    /// <summary>
    /// WP-021-Skelett des Google-Adapters gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 3
    /// (Mobile Ads, UMP und Firebase Analytics; Crashdiagnose bleibt lokal). Implementiert die
    /// vier Google-Ports aus Abschnitt 6 ohne SDK-Referenz und ohne Fachlogik. Der Adapter wird
    /// von der Composition Root erstellt, aber gemäß Abschnitt 7 nicht initialisiert: Ads,
    /// Analytics und Crashdiagnose bleiben hinter ihren Capability-, Privacy- und Recovery-Gates.
    /// </summary>
    public sealed class GoogleModuleSkeleton : IAdsPort, IConsentPort, IAnalyticsPort, ICrashReportingPort
    {
        /// <summary>Gibt an, ob der Adapter gestartet wurde; nach der Composition immer <c>false</c>.</summary>
        public bool IsInitialized { get; private set; }

        /// <summary>Markiert den Adapter als gestartet (ohne jede SDK- oder Netzwirkung im Skelett).</summary>
        public void Initialize()
        {
            this.IsInitialized = true;
        }
    }
}
