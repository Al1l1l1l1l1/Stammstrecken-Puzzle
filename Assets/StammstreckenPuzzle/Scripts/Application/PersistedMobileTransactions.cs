using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Application.Ports;

namespace STP.Application
{
    /// <summary>Technical source revisions, never an assertion of legal or device approval.</summary>
    public static class MobilePersistenceContract
    {
        /// <summary>Launch privacy policy contract key.</summary>
        public const string PrivacyPolicyRevision = "privacy-launch-v1";
        /// <summary>Exact reviewed SDK source line; native exported locks remain a separate release gate.</summary>
        public const string SdkContractRevision = "gma-11.5.0-ump-android4.0.0-ios3.1.0-iap5.4.3";
        /// <summary>Only the current native UMP flow supplies advertising candidates.</summary>
        public const string ConsentSource = "GOOGLE_UMP";
        /// <summary>Single native-store non-consumable grant contract.</summary>
        public const string IapPolicyVersion = "iap-remove-ads-v1";
    }

    /// <summary>Application's exact profile-specific purchase binding, independent of SDK types.</summary>
    public sealed class MobilePurchaseBinding
    {
        /// <summary>Creates an explicit expected identity; production Apple may use signed Sandbox in TestFlight.</summary>
        public MobilePurchaseBinding(string store, string applicationId, string storeProductId, bool production)
        {
            if ((store != "GooglePlay" && store != "AppleAppStore") || string.IsNullOrWhiteSpace(applicationId) || string.IsNullOrWhiteSpace(storeProductId)) throw new ArgumentException("IAP_BINDING_INVALID");
            MobileApplicationProfile.Validate(applicationId, production);
            Store = store; ApplicationId = applicationId; StoreProductId = storeProductId; Production = production;
        }
        /// <summary>Exact native store.</summary>
        public string Store { get; }
        /// <summary>Exact installed app identity.</summary>
        public string ApplicationId { get; }
        /// <summary>Actual configured non-consumable product.</summary>
        public string StoreProductId { get; }
        /// <summary>Test profiles must only accept Sandbox evidence.</summary>
        public bool Production { get; }
        internal bool Matches(PurchaseEvidence evidence) => evidence.Store == Store && evidence.ApplicationId == ApplicationId && evidence.StoreProductId == StoreProductId &&
            (evidence.Environment == "Sandbox" || (Production && evidence.Environment == "Production")) && evidence.TransactionKey.StartsWith("iap-v1:", StringComparison.Ordinal) &&
            evidence.TransactionKey.Length == 71 && evidence.TransactionKey.Substring(7).All(c => c >= '0' && c <= '9' || c >= 'a' && c <= 'f');
        internal bool Matches(PurchaseRevocationEvidence evidence) => evidence.Store == Store && evidence.ApplicationId == ApplicationId && evidence.StoreProductId == StoreProductId &&
            (evidence.Environment == "Sandbox" || (Production && evidence.Environment == "Production")) && evidence.TransactionKey.StartsWith("iap-v1:", StringComparison.Ordinal) &&
            evidence.TransactionKey.Length == 71 && evidence.TransactionKey.Substring(7).All(c => c >= '0' && c <= '9' || c >= 'a' && c <= 'f');
    }

    /// <summary>Real atomic-profile bridge for privacy and non-consumables. No second save, no ledger fabrication.</summary>
    public sealed class PersistedMobileTransactions
    {
        private readonly ILocalProfileRepository repository;
        private readonly SaveProfileIdentity profile;
        private readonly CatalogSetIdentity catalogs;
        private readonly ILocalSaveDispatcher dispatcher;
        private readonly ILocalSessionClock clock;
        private readonly MobilePurchaseBinding? purchases;
        private readonly SemaphoreSlim writer = new SemaphoreSlim(1, 1);

        /// <summary>Binds the existing repository/CAS authority and explicit Application-loop dispatcher.</summary>
        public PersistedMobileTransactions(ILocalProfileRepository repository, SaveProfileIdentity profile, CatalogSetIdentity catalogs,
            ILocalSaveDispatcher dispatcher, ILocalSessionClock clock, MobilePurchaseBinding purchases)
            : this(repository, profile, catalogs, dispatcher, clock, purchases ?? throw new ArgumentNullException(nameof(purchases)), false) { }

        /// <summary>Consent-only bridge: no product or store identity is invented, and both purchase mutations stay unavailable.</summary>
        public static PersistedMobileTransactions ForConsent(ILocalProfileRepository repository, SaveProfileIdentity profile, CatalogSetIdentity catalogs,
            ILocalSaveDispatcher dispatcher, ILocalSessionClock clock) => new PersistedMobileTransactions(repository, profile, catalogs, dispatcher, clock, null, true);

        private PersistedMobileTransactions(ILocalProfileRepository repository, SaveProfileIdentity profile, CatalogSetIdentity catalogs,
            ILocalSaveDispatcher dispatcher, ILocalSessionClock clock, MobilePurchaseBinding? purchases, bool consentOnly)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository)); this.profile = profile ?? throw new ArgumentNullException(nameof(profile));
            this.catalogs = catalogs ?? throw new ArgumentNullException(nameof(catalogs)); this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock)); this.purchases = purchases;
            if (profile.Use != catalogs.AggregateCatalog.Use || profile.Use == CatalogUse.IsolatedReview ||
                (!consentOnly && ((profile.Use == CatalogUse.Production) != purchases!.Production ||
                (profile.Use == CatalogUse.InternalPreview && (profile.Namespace != "qa-mobile-candidate" || purchases!.ApplicationId != MobileApplicationProfile.CandidateApplicationId)))))
                throw new ArgumentException("MOBILE_PROFILE_BINDING_INVALID");
        }
        private bool Bound(SaveSnapshot<LocalProfileState> snapshot) => snapshot.Generation >= 1 && snapshot.Profile.Equals(profile) && snapshot.Catalog.Equals(catalogs.AggregateCatalog) && snapshot.Payload.Profile.Identity.Equals(profile) && snapshot.Payload.Profile.CatalogSet.Equals(catalogs);
        private async Task<string> Utc()
        { string utc = string.Empty; await dispatcher.InvokeAsync(() => utc = clock.UtcNow).ConfigureAwait(false); return LocalModel.Utc(utc); }
        private static LocalProfileState Copy(LocalProfileState source, LocalEntitlements? entitlements = null, IReadOnlyList<LocalPendingOperation>? operations = null, LocalDataLifecycle? lifecycle = null)
            => LocalProfileState.Create(source.Profile, source.Levels, source.Endless, source.LedgerCheckpoint, source.EconomyJournal, source.Inventory, source.CosmeticClaimReservations,
                source.SeasonProgressCache, source.Drafts, source.RewardClaims, source.DailyStatus, entitlements ?? source.Entitlements, operations ?? source.PendingOperations,
                source.Settings, lifecycle ?? source.DataLifecycle, source.HintEntitlement).WithMetagame(source.Metagame);

        // Only guaranteed no-write CAS conflicts are retried, always from a fresh complete snapshot.
        private async Task<SaveCommitResult<LocalProfileState>> Commit(Func<LocalProfileState, LocalProfileState> project, CancellationToken cancellation)
        {
            for (int attempt = 0; attempt < 3; attempt++)
            {
                SaveLoadResult<LocalProfileState> loaded;
                try { loaded = await repository.LoadAsync(cancellation).ConfigureAwait(false); }
                catch (OperationCanceledException) { return Failure(SaveCommitStatus.Cancelled, "MOBILE_LOAD_CANCELLED"); }
                catch { return Failure(SaveCommitStatus.RecoveryRequired, "MOBILE_LOAD_REQUIRED"); }
                var snapshot = loaded.Snapshot;
                if (!loaded.Succeeded || snapshot == null || !Bound(snapshot)) return Failure(SaveCommitStatus.RecoveryRequired, "MOBILE_BOUND_PROFILE_REQUIRED");
                if (snapshot.Generation >= LocalModel.MaxSafe) return Failure(SaveCommitStatus.Failed, "SAVE_GENERATION_EXHAUSTED");
                LocalProfileState payload;
                try { payload = project(snapshot.Payload); }
                catch (ArgumentException) { return Failure(SaveCommitStatus.Failed, "MOBILE_PROJECTION_REJECTED"); }
                if (ReferenceEquals(payload, snapshot.Payload)) return SaveCommitResult<LocalProfileState>.Success(snapshot);
                var offered = new SaveSnapshot<LocalProfileState>(profile, catalogs.AggregateCatalog, snapshot.Generation + 1, payload);
                SaveCommitResult<LocalProfileState> result;
                try { result = await repository.CommitAsync(offered, snapshot.Generation, cancellation).ConfigureAwait(false); }
                catch { return Failure(SaveCommitStatus.RecoveryRequired, "MOBILE_COMMIT_OUTCOME_UNKNOWN"); }
                if (result.Status == SaveCommitStatus.Conflict) continue;
                if (!result.Succeeded) return result;
                if (result.Snapshot == null || !Bound(result.Snapshot) || result.Snapshot.Generation != offered.Generation || !ReferenceEquals(result.Snapshot.Payload, offered.Payload))
                    return Failure(SaveCommitStatus.RecoveryRequired, "MOBILE_COMMIT_BINDING_UNCONFIRMED");
                return result;
            }
            return Failure(SaveCommitStatus.Conflict, "MOBILE_SAVE_GENERATION_CONFLICT");
        }
        private static SaveCommitResult<LocalProfileState> Failure(SaveCommitStatus status, string code) => SaveCommitResult<LocalProfileState>.Failure(status, code);

        /// <summary>Commits ENABLE/REVOKE_PENDING, applies only that exact candidate, then confirms the applied decision.</summary>
        public async Task<SaveCommitResult<LocalProfileState>> PersistConsentAsync(IConsentPort consent, ConsentSnapshot candidate, CancellationToken cancellation)
        {
            if (consent == null || candidate == null) throw new ArgumentNullException();
            await writer.WaitAsync(cancellation).ConfigureAwait(false);
            try
            {
                var utc = await Utc().ConfigureAwait(false);
                var pending = await Commit(state =>
                {
                    var previous = state.DataLifecycle.PrivacyDecision;
                    var revision = checked((previous?.DecisionRevision ?? 0) + 1);
                    var decision = LocalPrivacyDecision.Create(MobilePersistenceContract.PrivacyPolicyRevision, MobilePersistenceContract.SdkContractRevision, revision, utc,
                        candidate.CanRequestAds, false, false, "VALID", MobilePersistenceContract.ConsentSource, candidate.CanRequestAds ? "ENABLE_PENDING" : "REVOKE_PENDING");
                    return Copy(state, lifecycle: LocalDataLifecycle.Create(decision, state.DataLifecycle.LastReconciledAtUtc));
                }, cancellation).ConfigureAwait(false);
                if (!pending.Succeeded || pending.Snapshot == null) { await dispatcher.InvokeAsync(consent.Revoke).ConfigureAwait(false); return pending; }
                var committedDecision = pending.Snapshot.Payload.DataLifecycle.PrivacyDecision!;
                bool applied = false;
                await dispatcher.InvokeAsync(() => applied = consent.ConfirmDecisionPersisted(candidate, pending.Snapshot.Generation)).ConfigureAwait(false);
                if (!applied) { await dispatcher.InvokeAsync(consent.Revoke).ConfigureAwait(false); return Failure(SaveCommitStatus.RecoveryRequired, "CONSENT_CURRENT_CANDIDATE_REQUIRED"); }
                var confirmed = await Commit(state =>
                {
                    var current = state.DataLifecycle.PrivacyDecision;
                    LocalModel.Require(current != null && current.DecisionRevision == committedDecision.DecisionRevision && current.PolicyRevision == committedDecision.PolicyRevision &&
                        current.SdkContractRevision == committedDecision.SdkContractRevision && current.Ads == committedDecision.Ads && current.NativeSyncState == committedDecision.NativeSyncState,
                        "CONSENT_COMMIT_BINDING_CHANGED");
                    var decision = LocalPrivacyDecision.Create(committedDecision.PolicyRevision, committedDecision.SdkContractRevision, committedDecision.DecisionRevision,
                        committedDecision.CapturedAtUtc, committedDecision.Ads, false, false, "VALID", MobilePersistenceContract.ConsentSource, committedDecision.Ads ? "ENABLED_CONFIRMED" : "REVOKED_CONFIRMED");
                    return Copy(state, lifecycle: LocalDataLifecycle.Create(decision, utc));
                }, cancellation).ConfigureAwait(false);
                if (!confirmed.Succeeded) await dispatcher.InvokeAsync(consent.Revoke).ConfigureAwait(false);
                return confirmed;
            }
            finally { writer.Release(); }
        }

        /// <summary>Immediate effective revoke, durable pending phase, idempotent native discard, then confirmed phase.</summary>
        public async Task<SaveCommitResult<LocalProfileState>> RevokeConsentAsync(IConsentPort consent, CancellationToken cancellation)
        {
            await dispatcher.InvokeAsync(consent.Revoke).ConfigureAwait(false);
            await writer.WaitAsync(cancellation).ConfigureAwait(false);
            try
            {
                var utc = await Utc().ConfigureAwait(false);
                long revision = 0;
                var pending = await Commit(state =>
                {
                    revision = checked((state.DataLifecycle.PrivacyDecision?.DecisionRevision ?? 0) + 1);
                    var decision = LocalPrivacyDecision.Create(MobilePersistenceContract.PrivacyPolicyRevision, MobilePersistenceContract.SdkContractRevision, revision, utc, false, false, false,
                        "VALID", MobilePersistenceContract.ConsentSource, "REVOKE_PENDING");
                    return Copy(state, lifecycle: LocalDataLifecycle.Create(decision, state.DataLifecycle.LastReconciledAtUtc));
                }, cancellation).ConfigureAwait(false);
                if (!pending.Succeeded) return pending;
                await dispatcher.InvokeAsync(consent.Revoke).ConfigureAwait(false);
                return await Commit(state =>
                {
                    var old = state.DataLifecycle.PrivacyDecision;
                    LocalModel.Require(old != null && old.DecisionRevision == revision && !old.Ads && old.NativeSyncState == "REVOKE_PENDING", "CONSENT_REVOKE_BINDING_CHANGED");
                    return Copy(state, lifecycle: LocalDataLifecycle.Create(LocalPrivacyDecision.Create(old!.PolicyRevision, old.SdkContractRevision, revision, utc,
                        false, false, false, "VALID", MobilePersistenceContract.ConsentSource, "REVOKED_CONFIRMED"), utc));
                }, cancellation).ConfigureAwait(false);
            }
            finally { writer.Release(); }
        }

        private LocalIapOperation? Existing(LocalProfileState state, PurchaseEvidence evidence)
        {
            var matches = state.PendingOperations.OfType<LocalIapOperation>().Where(operation => operation.Store == purchases!.Store && operation.LogicalProductKey == "remove_ads" && operation.TransactionReference == evidence.TransactionKey).ToArray();
            LocalModel.Require(matches.Length <= 1, "IAP_DUPLICATE_TRANSACTION_OPERATION");
            return matches.FirstOrDefault();
        }
        private LocalProfileState Stage(LocalProfileState state, PurchaseEvidence evidence, string target, string utc)
        {
            var existing = Existing(state, evidence);
            var entitlement = state.Entitlements.Records.SingleOrDefault(item => item.Store == purchases!.Store && item.ProductKey == "remove_ads" && item.TransactionReference == evidence.TransactionKey);
            LocalModel.Require(entitlement == null || !entitlement.RevocationConfirmed, "IAP_VERIFIED_REVOCATION_CONFLICT");
            if (existing?.State == "FINALIZED") return state;
            var order = new Dictionary<string, int>(StringComparer.Ordinal) { ["EVIDENCE_RECEIVED"] = 1, ["VERIFIED"] = 2, ["GRANTED_NOT_FINALIZED"] = 3, ["FINALIZED"] = 4 };
            if (existing != null && order.TryGetValue(existing.State, out var prior) && prior >= order[target]) return state;
            LocalModel.Require(state.PendingOperations.OfType<LocalIapOperation>().Count() < 256 || existing != null, "IAP_OPERATION_CAPACITY");
            var operationId = existing?.OperationId ?? "iap-grant:" + evidence.TransactionKey;
            LocalModel.Require(!state.PendingOperations.Any(operation => operation.OperationId == operationId && !ReferenceEquals(operation, existing)), "IAP_OPERATION_COLLISION");
            var operation = LocalIapOperation.Create(operationId, "remove_ads", purchases!.Store, evidence.TransactionKey, target);
            var operations = state.PendingOperations.Where(item => !ReferenceEquals(item, existing)).Concat(new[] { operation }).OrderBy(item => item.OperationId, StringComparer.Ordinal).ToArray();
            var entitlements = state.Entitlements;
            if (target == "GRANTED_NOT_FINALIZED" || target == "FINALIZED")
            {
                var granted = LocalEntitlement.Create("remove_ads", purchases!.Store, evidence.TransactionKey, true, false, false, utc, operationId, target == "FINALIZED" ? operationId : null);
                entitlements = LocalEntitlements.Create(state.Entitlements.Records.Where(item => !ReferenceEquals(item, entitlement)).Concat(new[] { granted }).OrderBy(item => item.Store + ":" + item.TransactionReference, StringComparer.Ordinal).ToArray());
            }
            return Copy(state, entitlements, operations);
        }

        /// <summary>Persisted evidence/verification, atomic entitlement+grant, native finalization, then final status commit.</summary>
        public async Task<SaveCommitResult<LocalProfileState>> ApplyPurchaseAsync(PurchaseEvidence evidence, IPurchasePort store, MobileOperation finalization, CancellationToken cancellation)
        {
            if (evidence == null || store == null || finalization == null) throw new ArgumentNullException();
            if (purchases == null || !purchases.Matches(evidence)) return Failure(SaveCommitStatus.Failed, "IAP_EVIDENCE_PROFILE_MISMATCH");
            bool issued = false;
            await dispatcher.InvokeAsync(() => issued = store.OwnsVerifiedEvidence(evidence)).ConfigureAwait(false);
            if (!issued) return Failure(SaveCommitStatus.Failed, "IAP_ADAPTER_EVIDENCE_REQUIRED");
            await writer.WaitAsync(cancellation).ConfigureAwait(false);
            try
            {
                var utc = await Utc().ConfigureAwait(false);
                SaveCommitResult<LocalProfileState>? result = null;
                foreach (var stage in new[] { "EVIDENCE_RECEIVED", "VERIFIED", "GRANTED_NOT_FINALIZED" })
                {
                    // A callback can revoke this proof while waiting for the writer or an earlier CAS phase.
                    await dispatcher.InvokeAsync(() => issued = store.OwnsVerifiedEvidence(evidence)).ConfigureAwait(false);
                    if (!issued) return Failure(SaveCommitStatus.Failed, "IAP_ADAPTER_EVIDENCE_REVOKED");
                    result = await Commit(state => Stage(state, evidence, stage, utc), cancellation).ConfigureAwait(false);
                    if (!result.Succeeded || result.Snapshot == null) return result;
                }
                var durable = result!.Snapshot!;
                var entitlement = durable.Payload.Entitlements.Records.SingleOrDefault(item => item.Store == evidence.Store && item.ProductKey == "remove_ads" && item.TransactionReference == evidence.TransactionKey);
                var operation = Existing(durable.Payload, evidence);
                if (entitlement == null || !entitlement.Owned || entitlement.RevocationConfirmed || operation == null || (operation.State != "GRANTED_NOT_FINALIZED" && operation.State != "FINALIZED") || entitlement.GrantOperationId != operation.OperationId)
                    return Failure(SaveCommitStatus.RecoveryRequired, "IAP_DURABLE_GRANT_UNCONFIRMED");
                if (operation.State == "FINALIZED") return result;
                try
                {
                    Task<MobileResult<PurchaseAcknowledgement>>? native = null;
                    await dispatcher.InvokeAsync(() => native = store.FinalizeAsync(new DurablePurchaseGrant(evidence, durable.Generation), finalization)).ConfigureAwait(false);
                    var acknowledged = await native!.ConfigureAwait(false);
                    if (acknowledged.Status != MobileServiceStatus.Succeeded || acknowledged.Value?.TransactionKey != evidence.TransactionKey)
                        return SaveCommitResult<LocalProfileState>.Success(durable);
                }
                catch
                {
                    // This catch surrounds only native work after the authoritatively committed grant.
                    // The independent FINALIZED save below retains its full uncertain-outcome semantics.
                    return SaveCommitResult<LocalProfileState>.Success(durable);
                }
                return await Commit(state => Stage(state, evidence, "FINALIZED", utc), cancellation).ConfigureAwait(false);
            }
            finally { writer.Release(); }
        }

        /// <summary>Only an explicitly verified store revocation key changes that one entitlement; no absence-based revoke.</summary>
        public async Task<SaveCommitResult<LocalProfileState>> ApplyVerifiedRevocationAsync(PurchaseRevocationEvidence evidence, IPurchasePort store, CancellationToken cancellation)
        {
            if (evidence == null || store == null || purchases == null || !purchases.Matches(evidence)) return Failure(SaveCommitStatus.Failed, "IAP_REVOCATION_BINDING_REQUIRED");
            bool issued = false;
            await dispatcher.InvokeAsync(() => issued = store.OwnsVerifiedRevocation(evidence)).ConfigureAwait(false);
            if (!issued) return Failure(SaveCommitStatus.Failed, "IAP_ADAPTER_REVOCATION_REQUIRED");
            await writer.WaitAsync(cancellation).ConfigureAwait(false);
            try
            {
                var utc = await Utc().ConfigureAwait(false);
                return await Commit(state =>
                {
                    var current = state.Entitlements.Records.SingleOrDefault(item => item.Store == purchases.Store && item.ProductKey == "remove_ads" && item.TransactionReference == evidence.TransactionKey);
                    if (current?.RevocationConfirmed == true) return state;
                    // Preserve verified revocation even before the first grant or after adapter recreation.
                    var revoked = LocalEntitlement.Create("remove_ads", purchases.Store, evidence.TransactionKey, false, false, true, utc, current?.GrantOperationId, current?.FinalizationOperationId);
                    return Copy(state, LocalEntitlements.Create(state.Entitlements.Records.Where(item => !ReferenceEquals(item, current)).Concat(new[] { revoked }).OrderBy(item => item.Store + ":" + item.TransactionReference, StringComparer.Ordinal).ToArray()));
                }, cancellation).ConfigureAwait(false);
            }
            finally { writer.Release(); }
        }
        /// <summary>Local confirmed cache remains effective offline and while finalization is pending.</summary>
        public static bool HasRemoveAds(LocalProfileState state) => state.Entitlements.Records.Any(item => item.ProductKey == "remove_ads" && item.Owned && !item.RevocationConfirmed);
    }
}
