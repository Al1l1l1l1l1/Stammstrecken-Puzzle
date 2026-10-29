using System;
using System.Threading.Tasks;

namespace STP.Application.Ports
{
    /// <summary>
    /// Normativer Application-Port gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 6.
    /// Minimale Verantwortung: Produkte, Kaufbeleg, lokale Prüfung, Acknowledge/Finish, Pending, Restore und Revocation abbilden.
    /// Verbotene Verantwortung: Entitlement direkt vergeben.
    /// Implementierendes Modul: <c>STP.MobileServices.Store</c>.
    /// WP-021-Skelett ohne Mitglieder; fachliche Mitglieder folgen mit den zuständigen Work Packages.
    /// </summary>
    public interface IPurchasePort
    {
        /// <summary>Verified new or replayed evidence, including late completion after timeout.</summary>
        event Action<PurchaseEvidence>? EvidenceReceived;
        /// <summary>Explicit verified revocation key; empty restore never triggers this event.</summary>
        event Action<PurchaseRevocationEvidence>? EntitlementRevoked;
        /// <summary>Local proof check BEFORE grant: only an exact currently issued verified adapter evidence is accepted.</summary>
        bool OwnsVerifiedEvidence(PurchaseEvidence evidence);
        /// <summary>Local proof check BEFORE revoke: a DTO name or transaction string never attests verification.</summary>
        bool OwnsVerifiedRevocation(PurchaseRevocationEvidence evidence);
        /// <summary>Lazy connection and sole-product fetch after intent AND reviewed readiness.</summary>
        Task<MobileResult<StoreProduct>> PrepareAsync(PurchaseIntent intent, IapPrivacyReadiness? readiness, MobileOperation operation);
        /// <summary>Starts the sole product purchase after preparation.</summary>
        Task<MobileResult<PurchaseEvidence>> PurchaseAsync(MobileOperation operation);
        /// <summary>Explicit store restore; recovered evidence uses the same verification pipeline.</summary>
        Task<MobileResult<PurchaseAcknowledgement>> RestoreAsync(MobileOperation operation);
        /// <summary>Store finish/acknowledge only after matching durable entitlement grant.</summary>
        Task<MobileResult<PurchaseAcknowledgement>> FinalizeAsync(DurablePurchaseGrant grant, MobileOperation operation);
    }
}
