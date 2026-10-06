using STP.Application.Ports;

namespace STP.MobileServices.Store
{
    /// <summary>
    /// WP-021-Skelett des Store-Adapters gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 3
    /// (Unity-IAP-Adapter, lokale Belegprüfung und Storetransaktionsmapping). Implementiert den
    /// <see cref="IPurchasePort"/>-Port aus Abschnitt 6 ohne SDK-Referenz und ohne Fachlogik.
    /// Der Adapter wird erstellt, aber gemäß Abschnitt 7 nicht initialisiert.
    /// </summary>
    public sealed class StoreModuleSkeleton : IPurchasePort
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
