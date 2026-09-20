using STP.Application.Ports;

namespace STP.MobileServices.Google
{
    /// <summary>
    /// WP-008-Adapter-Skelett fuer die Google-gebundenen Ports. Dieses Skelett
    /// referenziert kein SDK und startet keinen Provider: Ads, UMP/Consent,
    /// Analytics und Crashdiagnose bleiben hinter ihren Capability-, Privacy-
    /// und Recovery-Gates (ADR-018, ADR-020, ADR-024). Firebase Analytics und
    /// Crashlytics sind im aktuellen Productionprofil ausgeschlossen.
    /// </summary>
    public sealed class GoogleMobileServicesAdapter : IAdsPort, IConsentPort, IAnalyticsPort, ICrashReportingPort
    {
    }
}
