using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;
using STP.Application.Ports;
using STP.MobileServices.Google;
using STP.MobileServices.Store;
using UnityEngine;

namespace STP.Bootstrap
{
    /// <summary>Real isolated candidate providers. SDK requests stay outside the injected shared profile writer.</summary>
    public sealed class MobileCandidateServices : IDisposable
    {
        private readonly ILocalProfileRepository repository;
        private readonly SaveProfileIdentity profile;
        private readonly CatalogSetIdentity catalogs;
        private readonly ILocalSaveDispatcher dispatcher;
        private readonly ILocalSessionClock clock;
        private readonly Func<Func<CancellationToken, Task<SaveCommitResult<LocalProfileState>>>, CancellationToken, Task<SaveCommitResult<LocalProfileState>>> runProfileTransaction;
        private readonly Func<AdRewardEvidence, CancellationToken, Task<SaveCommitResult<LocalProfileState>>> applyReward;
        private readonly GoogleMobileServices google;
        private readonly PersistedMobileTransactions privacyTransactions;
        private readonly InterstitialPolicy interstitials;
        private readonly CancellationTokenSource lifetime = new CancellationTokenSource();
        private readonly object gate = new object();
        private readonly HashSet<Task> pending = new HashSet<Task>();
        private readonly Dictionary<string, Task<SaveCommitResult<LocalProfileState>>> evidenceTasks = new Dictionary<string, Task<SaveCommitResult<LocalProfileState>>>(StringComparer.Ordinal);
        private PersistedMobileTransactions? storeTransactions;
        private volatile UnityStorePurchaseAdapter? store;
        private IapPrivacyReadiness? readiness;
        private volatile bool disposed;

        /// <summary>Construct on Unity main, using the same repository, profile and dispatcher as the live session.</summary>
        public MobileCandidateServices(ILocalProfileRepository repository, SaveProfileIdentity profile, CatalogSetIdentity catalogs,
            ILocalSaveDispatcher dispatcher, ILocalSessionClock clock,
            Func<Func<CancellationToken, Task<SaveCommitResult<LocalProfileState>>>, CancellationToken, Task<SaveCommitResult<LocalProfileState>>> runProfileTransaction,
            Func<AdRewardEvidence, CancellationToken, Task<SaveCommitResult<LocalProfileState>>> applyReward)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.profile = profile ?? throw new ArgumentNullException(nameof(profile)); this.catalogs = catalogs ?? throw new ArgumentNullException(nameof(catalogs));
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher)); this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this.runProfileTransaction = runProfileTransaction ?? throw new ArgumentNullException(nameof(runProfileTransaction)); this.applyReward = applyReward ?? throw new ArgumentNullException(nameof(applyReward));
            if (profile.Use != CatalogUse.InternalPreview || profile.Namespace != SaveProfileIdentity.MobileCandidateNamespace || catalogs.AggregateCatalog.Use != CatalogUse.InternalPreview)
                throw new ArgumentException("MOBILE_CANDIDATE_PROFILE_REQUIRED");
            google = new GoogleMobileServices(NativeGoogleRuntimeFactory.Create(GoogleAdConfiguration.TestAds(MobileApplicationProfile.CandidateApplicationId,
                UnityEngine.Application.platform == RuntimePlatform.IPhonePlayer)), Post);
            privacyTransactions = PersistedMobileTransactions.ForConsent(repository, profile, catalogs, dispatcher, clock);
            interstitials = new InterstitialPolicy(profile, catalogs.AggregateCatalog);
            google.RewardEarned += RewardReceived;
        }

        /// <summary>Current effective capabilities; analytics and remote crash reporting always remain false.</summary>
        public MobileCapabilities Capabilities => google.Capabilities;
        /// <summary>Actual provider presentation only; reading this never initializes an SDK.</summary>
        public bool IsNativeUiBusy => !disposed && (google.IsNativeUiBusy || store?.IsNativeUiBusy == true);
        /// <summary>Exact real ports, without a fake initialization claim.</summary>
        public IConsentPort Consent => google;
        /// <summary>Exact real advertising port. Application owns when an ad is eligible.</summary>
        public IAdsPort Ads => google;
        /// <summary>Absent until actual sandbox product configuration is supplied.</summary>
        public IPurchasePort? Purchases => store;
        /// <summary>Actual localized native product, absent until an explicit readiness gated fetch succeeds.</summary>
        public StoreProduct? Product { get; private set; }
        /// <summary>Only configured and explicitly reviewed inputs permit the user's price-load action; this getter starts nothing.</summary>
        public bool CanLoadStorePrice => !disposed && store != null && readiness != null && !IsNativeUiBusy;
        /// <summary>The exact fetched product remains available for a separate deliberate purchase.</summary>
        public bool CanPurchaseDisplayedProduct => store?.IsPreparedProduct(Product) == true;
        /// <summary>Stable redacted callback/persistence diagnostics, published on the Application dispatcher.</summary>
        public event Action<string>? ServiceNotice;
        /// <summary>All currently observed late callback work; the owning scene continues pumping its dispatcher until shutdown completes.</summary>
        public Task PendingCallbacks { get { lock (gate) return Task.WhenAll(pending.ToArray()); } }

        private static MobileOperation Op(string purpose, CancellationToken cancellation, int seconds = 30) => new MobileOperation("candidate:" + purpose + ":" + Guid.NewGuid().ToString("N"), TimeSpan.FromSeconds(seconds), cancellation);
        private static SaveCommitResult<LocalProfileState> Failure(string code) => SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.RecoveryRequired, code);
        private void Post(Action action) { try { Observe(dispatcher.InvokeAsync(action)); } catch { } }
        private void Notice(string code) => Post(() => ServiceNotice?.Invoke(code));
        private void Observe(Task task)
        {
            lock (gate) pending.Add(task);
            _ = CompleteObserved(task);
        }
        private async Task CompleteObserved(Task task)
        {
            try { await task.ConfigureAwait(false); }
            catch { /* Caller-facing operations and persisted callbacks report stable codes separately. */ }
            finally { lock (gate) pending.Remove(task); }
        }

        /// <summary>Explicit current-start UMP flow, followed by both durable consent phases.</summary>
        public Task<MobileResult<ConsentSnapshot>> RefreshConsentAsync(CancellationToken cancellation) => ConsentAsync(false, cancellation);
        /// <summary>Explicit native privacy options, with immediate effective revocation and durable reapplication.</summary>
        public Task<MobileResult<ConsentSnapshot>> ShowPrivacyOptionsAsync(CancellationToken cancellation) => ConsentAsync(true, cancellation);
        private async Task<MobileResult<ConsentSnapshot>> ConsentAsync(bool options, CancellationToken cancellation)
        {
            var operation = Op(options ? "privacy-options" : "consent", cancellation);
            if (disposed) return MobileResult<ConsentSnapshot>.Stop(operation, MobileServiceStatus.Unavailable, "MOBILE_CANDIDATE_DISPOSED");
            var result = await (options ? google.ShowPrivacyOptionsAsync(operation) : google.RefreshAsync(operation)).ConfigureAwait(false);
            if (result.Status != MobileServiceStatus.Succeeded || result.Value == null) return result;
            var committed = await runProfileTransaction(ct => privacyTransactions.PersistConsentAsync(google, result.Value, ct), cancellation).ConfigureAwait(false);
            if (!committed.Succeeded) return MobileResult<ConsentSnapshot>.Stop(operation, MobileServiceStatus.Pending, "CONSENT_DURABLE_SYNC_REQUIRED");
            return result;
        }
        /// <summary>Immediate effective off, then the same atomically persisted revocation protocol.</summary>
        public Task<SaveCommitResult<LocalProfileState>> RevokeConsentAsync(CancellationToken cancellation)
        {
            google.Revoke();
            return runProfileTransaction(ct => privacyTransactions.RevokeConsentAsync(google, ct), cancellation);
        }

        /// <summary>Supply only actual QA-store metadata and qualified readiness; construction itself still does not start IAP.</summary>
        public void ConfigureStore(StoreConfiguration configuration, IapPrivacyReadiness? reviewedReadiness)
        {
            if (disposed || store != null) throw new InvalidOperationException("MOBILE_STORE_ALREADY_BOUND");
            if (configuration == null || configuration.Production || configuration.ApplicationId != MobileApplicationProfile.CandidateApplicationId)
                throw new ArgumentException("MOBILE_CANDIDATE_STORE_PROFILE_REQUIRED");
            if (reviewedReadiness != null && (reviewedReadiness.PolicyRevision != configuration.PolicyRevision || reviewedReadiness.SdkRevision != configuration.SdkRevision || (configuration.Store == "GooglePlay" && string.IsNullOrWhiteSpace(configuration.GooglePublicKey))))
                throw new ArgumentException("MOBILE_STORE_REVIEWED_INPUTS_MISMATCH");
            storeTransactions = new PersistedMobileTransactions(repository, profile, catalogs, dispatcher, clock,
                new MobilePurchaseBinding(configuration.Store, configuration.ApplicationId, configuration.StoreProductId, false));
            readiness = reviewedReadiness;
            store = new UnityStorePurchaseAdapter(configuration, () => NativeStoreRuntimeFactory.Create(configuration), Post);
            store.EvidenceReceived += EvidenceReceived; store.EntitlementRevoked += RevocationReceived;
        }
        /// <summary>Fetches the real localized product only for an explicit purchase/restore intent and actual readiness.</summary>
        public async Task<MobileResult<StoreProduct>> PrepareStoreAsync(PurchaseIntent intent, CancellationToken cancellation)
        {
            var operation = Op("store-prepare", cancellation);
            var adapter = store;
            if (disposed || adapter == null) return MobileResult<StoreProduct>.Stop(operation, MobileServiceStatus.NotAllowed, "IAP_ACTUAL_CONFIGURATION_REQUIRED");
            await dispatcher.InvokeAsync(() => Product = null).ConfigureAwait(false);
            var result = await adapter.PrepareAsync(intent, readiness, operation).ConfigureAwait(false);
            if (result.Status == MobileServiceStatus.Succeeded) await dispatcher.InvokeAsync(() => Product = result.Value).ConfigureAwait(false);
            return result;
        }
        /// <summary>Explicitly loads the native price for display; never buys anything.</summary>
        public Task<MobileResult<StoreProduct>> LoadStorePriceAsync(CancellationToken cancellation) => PrepareStoreAsync(PurchaseIntent.Purchase, cancellation);
        /// <summary>Actual separate native purchase of the displayed product; no implicit refetch or changed unseen offer.</summary>
        public async Task<MobileResult<PurchaseEvidence>> PurchaseRemoveAdsAsync(StoreProduct displayedProduct, CancellationToken cancellation)
        {
            var operation = Op("purchase", cancellation, 120);
            var adapter = store;
            if (disposed || adapter == null || displayedProduct == null || !ReferenceEquals(Product, displayedProduct))
                return MobileResult<PurchaseEvidence>.Stop(operation, MobileServiceStatus.NotAllowed, "IAP_DISPLAYED_OFFER_REQUIRED");
            await dispatcher.InvokeAsync(() => Product = null).ConfigureAwait(false);
            var result = await adapter.PurchasePreparedAsync(displayedProduct, operation).ConfigureAwait(false);
            if (result.Status != MobileServiceStatus.Succeeded || result.Value == null) return result;
            var committed = await ApplyEvidence(result.Value, adapter).ConfigureAwait(false);
            return GrantIsFinalized(committed, result.Value.TransactionKey) ? result : MobileResult<PurchaseEvidence>.Stop(operation, MobileServiceStatus.Pending, "IAP_DURABLE_GRANT_OR_FINALIZATION_REQUIRED");
        }
        /// <summary>Explicit native restore. Empty restore never revokes; each recovered transaction uses the durable shared writer.</summary>
        public async Task<MobileResult<PurchaseAcknowledgement>> RestoreAsync(CancellationToken cancellation)
        {
            var operation = Op("restore", cancellation, 120);
            var prepared = await PrepareStoreAsync(PurchaseIntent.Restore, cancellation).ConfigureAwait(false);
            if (prepared.Status != MobileServiceStatus.Succeeded) return MobileResult<PurchaseAcknowledgement>.Stop(operation, prepared.Status, prepared.Code);
            lock (gate)
                foreach (var key in evidenceTasks.Where(pair => pair.Value.Status == TaskStatus.RanToCompletion && !GrantIsFinalized(pair.Value.Result, pair.Key)).Select(pair => pair.Key).ToArray()) evidenceTasks.Remove(key);
            var restored = await store!.RestoreAsync(operation).ConfigureAwait(false);
            await PendingCallbacks.ConfigureAwait(false);
            lock (gate)
                if (evidenceTasks.Any(pair => pair.Value.Status != TaskStatus.RanToCompletion || !GrantIsFinalized(pair.Value.Result, pair.Key)))
                    return MobileResult<PurchaseAcknowledgement>.Stop(operation, MobileServiceStatus.Pending, "IAP_RESTORE_SAVE_RECOVERY_REQUIRED");
            return restored;
        }
        /// <summary>Only an already committed Application reservation is shown; late earned evidence still enters the shared writer.</summary>
        public async Task<MobileResult<AdPresentation>> ShowRewardedAsync(AdRewardReservation committedReservation, CancellationToken cancellation)
        {
            if (committedReservation == null) throw new ArgumentNullException(nameof(committedReservation));
            var operation = new MobileOperation(committedReservation.OperationId, TimeSpan.FromSeconds(120), cancellation);
            if (disposed) return MobileResult<AdPresentation>.Stop(operation, MobileServiceStatus.Unavailable, "MOBILE_CANDIDATE_DISPOSED");
            var loaded = await google.LoadAsync(MobileAdKind.Rewarded, Op("reward-load", cancellation)).ConfigureAwait(false);
            if (loaded.Status != MobileServiceStatus.Succeeded || loaded.Value == null) return MobileResult<AdPresentation>.Stop(operation, loaded.Status, loaded.Code);
            return await google.ShowAsync(loaded.Value, committedReservation, operation).ConfigureAwait(false);
        }
        /// <summary>Only explicit Continue after a complete result may offer an interstitial. Every skipped/no-fill opportunity ends here.</summary>
        public async Task<MobileResult<AdPresentation>> TryInterstitialAfterResultAsync(InterstitialResultReceipt receipt,
            Func<bool> stillAtCompletedResult, bool explicitContinue, CancellationToken cancellation)
        {
            if (receipt == null || stillAtCompletedResult == null) throw new ArgumentNullException();
            var operation = Op("interstitial", cancellation, 120);
            InterstitialPermit? permit = null; LoadedAd? ad = null;
            try
            {
                if (disposed) return MobileResult<AdPresentation>.Stop(operation, MobileServiceStatus.Unavailable, "MOBILE_CANDIDATE_DISPOSED");
                var current = await repository.LoadAsync(cancellation).ConfigureAwait(false);
                if (!CurrentProfile(current, receipt)) return MobileResult<AdPresentation>.Stop(operation, MobileServiceStatus.NotAllowed, "INTERSTITIAL_CURRENT_PROFILE_REQUIRED");
                bool boundary = false;
                await dispatcher.InvokeAsync(() => boundary = stillAtCompletedResult()).ConfigureAwait(false);
                if (!interstitials.TryTake(receipt, boundary, explicitContinue, google.Capabilities.CanRequestAds,
                    PersistedMobileTransactions.HasRemoveAds(current.Snapshot!.Payload), out permit, out var code))
                    return MobileResult<AdPresentation>.Stop(operation, MobileServiceStatus.NotAllowed, code);
                var loaded = await google.LoadAsync(MobileAdKind.Interstitial, Op("interstitial-load", cancellation)).ConfigureAwait(false);
                if (loaded.Status != MobileServiceStatus.Succeeded || loaded.Value == null) return MobileResult<AdPresentation>.Stop(operation, loaded.Status, loaded.Code);
                ad = loaded.Value;
                current = await repository.LoadAsync(cancellation).ConfigureAwait(false);
                if (!CurrentProfile(current, receipt)) return MobileResult<AdPresentation>.Stop(operation, MobileServiceStatus.NotAllowed, "INTERSTITIAL_CURRENT_PROFILE_REQUIRED");
                bool owned = PersistedMobileTransactions.HasRemoveAds(current.Snapshot!.Payload);
                Task<MobileResult<AdPresentation>>? native = null;
                await dispatcher.InvokeAsync(() =>
                {
                    if (interstitials.BeginShow(permit!, stillAtCompletedResult(), google.Capabilities.CanRequestAds, owned))
                        native = google.ShowAsync(ad, null, operation);
                }).ConfigureAwait(false);
                return native == null ? MobileResult<AdPresentation>.Stop(operation, MobileServiceStatus.NotAllowed, "INTERSTITIAL_BOUNDARY_OR_CAPABILITY_CHANGED") : await native.ConfigureAwait(false);
            }
            catch (OperationCanceledException) { return MobileResult<AdPresentation>.Stop(operation, MobileServiceStatus.Cancelled, "INTERSTITIAL_CALLER_CANCELLED"); }
            catch { return MobileResult<AdPresentation>.Stop(operation, MobileServiceStatus.Unavailable, "INTERSTITIAL_UNAVAILABLE"); }
            finally
            {
                if (ad != null) google.ReleaseLoadedAd(ad);
                if (permit != null) interstitials.Release(permit);
            }
        }
        private bool CurrentProfile(SaveLoadResult<LocalProfileState> loaded, InterstitialResultReceipt receipt)
            => loaded.Succeeded && loaded.Snapshot != null && loaded.Snapshot.Profile.Equals(profile) && loaded.Snapshot.Catalog.Equals(catalogs.AggregateCatalog) &&
                loaded.Snapshot.Generation >= receipt.Generation && loaded.Snapshot.Payload.Profile.Identity.Equals(profile) && loaded.Snapshot.Payload.Profile.CatalogSet.Equals(catalogs);

        private void RewardReceived(AdRewardEvidence evidence) => Observe(ApplyCallback(() => applyReward(evidence, lifetime.Token), "ADS_REWARD_SAVE_RECOVERY_REQUIRED"));
        private void EvidenceReceived(PurchaseEvidence evidence) { var adapter = store; if (adapter != null) Observe(ApplyEvidence(evidence, adapter)); }
        private void RevocationReceived(PurchaseRevocationEvidence evidence)
        {
            var adapter = store; var transactions = storeTransactions;
            if (adapter != null && transactions != null) Observe(ApplyCallback(() => runProfileTransaction(ct => transactions.ApplyVerifiedRevocationAsync(evidence, adapter, ct), lifetime.Token), "IAP_REVOCATION_SAVE_RECOVERY_REQUIRED"));
        }
        private static bool GrantIsFinalized(SaveCommitResult<LocalProfileState> committed, string transactionKey)
            => committed.Succeeded && committed.Snapshot != null && committed.Snapshot.Payload.PendingOperations.OfType<LocalIapOperation>()
                .Any(operation => operation.LogicalProductKey == "remove_ads" && operation.TransactionReference == transactionKey && operation.State == "FINALIZED");

        private Task<SaveCommitResult<LocalProfileState>> ApplyEvidence(PurchaseEvidence evidence, UnityStorePurchaseAdapter adapter)
        {
            TaskCompletionSource<SaveCommitResult<LocalProfileState>> completion;
            lock (gate)
            {
                if (evidenceTasks.TryGetValue(evidence.TransactionKey, out var existing)) return existing;
                completion = new TaskCompletionSource<SaveCommitResult<LocalProfileState>>(TaskCreationOptions.RunContinuationsAsynchronously);
                evidenceTasks.Add(evidence.TransactionKey, completion.Task);
            }
            _ = CompleteEvidence(evidence, adapter, completion);
            return completion.Task;
        }
        private async Task CompleteEvidence(PurchaseEvidence evidence, UnityStorePurchaseAdapter adapter, TaskCompletionSource<SaveCommitResult<LocalProfileState>> completion)
        {
            var transactions = storeTransactions;
            var result = transactions == null ? Failure("IAP_ACTUAL_CONFIGURATION_REQUIRED") : await ApplyCallback(() => runProfileTransaction(
                ct => transactions.ApplyPurchaseAsync(evidence, adapter, Op("iap-finalize", lifetime.Token), ct), lifetime.Token), "IAP_GRANT_SAVE_RECOVERY_REQUIRED").ConfigureAwait(false);
            completion.TrySetResult(result);
        }
        private async Task<SaveCommitResult<LocalProfileState>> ApplyCallback(Func<Task<SaveCommitResult<LocalProfileState>>> action, string failureCode)
        {
            try
            {
                var result = await action().ConfigureAwait(false);
                Notice(result.Succeeded ? "MOBILE_PROFILE_SYNCED" : failureCode);
                return result;
            }
            catch { Notice(failureCode); return Failure(failureCode); }
        }
        /// <summary>Gates off and cancels owned work; persisted uncertain transactions remain recoverable, never silently finalized.</summary>
        public void Dispose()
        {
            if (disposed) return; disposed = true; lifetime.Cancel();
            google.RewardEarned -= RewardReceived; google.Dispose();
            if (store != null) { store.EvidenceReceived -= EvidenceReceived; store.EntitlementRevoked -= RevocationReceived; store.Dispose(); }
        }
    }
}
