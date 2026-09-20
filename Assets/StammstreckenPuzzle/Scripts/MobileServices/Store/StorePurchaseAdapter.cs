using STP.Application.Ports;

namespace STP.MobileServices.Store
{
    /// <summary>
    /// WP-008-Adapter-Skelett fuer den Kauf-Port. Dieses Skelett referenziert kein
    /// SDK und initialisiert IAP nicht: Unity IAP wird erst bei Nutzeraktion oder
    /// persistenter Recovery plus Release-Readiness lazy initialisiert
    /// (ARCHITECTURE.md Abschnitt 5.1). Lokale Belegpruefung und
    /// Storetransaktionsmapping folgen mit WP-009.
    /// </summary>
    public sealed class StorePurchaseAdapter : IPurchasePort
    {
    }
}
