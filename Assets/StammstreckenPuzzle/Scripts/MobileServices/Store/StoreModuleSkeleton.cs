using System;
using System.Threading.Tasks;
using STP.Application;
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
        /// <summary>The historical QA skeleton never emits store evidence.</summary>
        public event Action<PurchaseEvidence>? EvidenceReceived { add { } remove { } }
        /// <summary>The historical QA skeleton never revokes entitlements.</summary>
        public event Action<PurchaseRevocationEvidence>? EntitlementRevoked { add { } remove { } }
        /// <summary>A legacy skeleton never owns a verified transaction.</summary>
        public bool OwnsVerifiedEvidence(PurchaseEvidence evidence) => false;
        /// <summary>A legacy skeleton never owns a verified revocation.</summary>
        public bool OwnsVerifiedRevocation(PurchaseRevocationEvidence evidence) => false;
        /// <summary>Fail-closed historical composition.</summary>
        public Task<MobileResult<StoreProduct>> PrepareAsync(PurchaseIntent intent, IapPrivacyReadiness? readiness, MobileOperation operation) => Task.FromResult(MobileResult<StoreProduct>.Stop(operation, MobileServiceStatus.Unavailable, "MOBILE_LEGACY_SKELETON"));
        /// <summary>Fail-closed historical composition.</summary>
        public Task<MobileResult<PurchaseEvidence>> PurchaseAsync(MobileOperation operation) => Task.FromResult(MobileResult<PurchaseEvidence>.Stop(operation, MobileServiceStatus.Unavailable, "MOBILE_LEGACY_SKELETON"));
        /// <summary>Fail-closed historical composition.</summary>
        public Task<MobileResult<PurchaseAcknowledgement>> RestoreAsync(MobileOperation operation) => Task.FromResult(MobileResult<PurchaseAcknowledgement>.Stop(operation, MobileServiceStatus.Unavailable, "MOBILE_LEGACY_SKELETON"));
        /// <summary>Fail-closed historical composition.</summary>
        public Task<MobileResult<PurchaseAcknowledgement>> FinalizeAsync(DurablePurchaseGrant grant, MobileOperation operation) => RestoreAsync(operation);
        /// <summary>Gibt an, ob der Adapter gestartet wurde; nach der Composition immer <c>false</c>.</summary>
        public bool IsInitialized { get; private set; }

        /// <summary>Markiert den Adapter als gestartet (ohne jede SDK- oder Netzwirkung im Skelett).</summary>
        public void Initialize()
        {
            this.IsInitialized = true;
        }
    }
}
