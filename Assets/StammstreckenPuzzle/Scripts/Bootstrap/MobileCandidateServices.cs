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
    /// <summary>Explicit candidate or production composition over one shared provider/persistence pipeline. SDK requests stay outside the injected shared profile writer.</summary>
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
        private readonly bool production;
        private readonly int ownerThread;
        private readonly GoogleAdConfiguration adConfiguration;
        private readonly RuntimePlatform? productionPlatform;
        private readonly Func<StoreConfiguration, IUnityStoreRuntime> createStoreRuntime;
        private volatile bool productionInputWindowClosed;
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

        /// <summary>Construct the unchanged isolated QA provider profile on Unity main.</summary>
        public MobileCandidateServices(ILocalProfileRepository repository, SaveProfileIdentity profile, CatalogSetIdentity catalogs,
            ILocalSaveDispatcher dispatcher, ILocalSessionClock clock,
            Func<Func<CancellationToken, Task<SaveCommitResult<LocalProfileState>>>, CancellationToken, Task<SaveCommitResult<LocalProfileState>>> runProfileTransaction,
            Func<AdRewardEvidence, CancellationToken, Task<SaveCommitResult<LocalProfileState>>> applyReward)
            : this(repository, profile, catalogs, dispatcher, clock, runProfileTransaction, applyReward,
                GoogleAdConfiguration.TestAds(MobileApplicationProfile.CandidateApplicationId, UnityEngine.Application.platform == RuntimePlatform.IPhonePlayer),
                CatalogUse.InternalPreview, null, NativeGoogleRuntimeFactory.Create, NativeStoreRuntimeFactory.Create) { }

        /// <summary>Root supplies its exact acknowledged locked profile. Missing real inputs keep composition offline; this method has no fallback IDs.</summary>
        public static MobileCandidateServices CreateProduction(ILocalProfileRepository repository, SaveSnapshot<LocalProfileState> confirmedProfile,
            ILocalSaveDispatcher dispatcher, ILocalSessionClock clock,
            Func<Func<CancellationToken, Task<SaveCommitResult<LocalProfileState>>>, CancellationToken, Task<SaveCommitResult<LocalProfileState>>> runProfileTransaction,
            Func<AdRewardEvidence, CancellationToken, Task<SaveCommitResult<LocalProfileState>>> applyReward,
            GoogleAdConfiguration ads, RuntimePlatform expectedPlatform)
        {
            RequireSnapshot(confirmedProfile, CatalogUse.Production);
            if (ads == null) throw new ArgumentException("MOBILE_PRODUCTION_AD_UNITS_REQUIRED");
            var problems = ProductionConfigurationIssues(ads.ApplicationId, expectedPlatform, ads, null, null, false);
            if (problems.Count != 0) throw new ArgumentException(problems[0]);
            return new MobileCandidateServices(repository, confirmedProfile.Profile, confirmedProfile.Payload.Profile.CatalogSet,
                dispatcher, clock, runProfileTransaction, applyReward, ads, CatalogUse.Production, expectedPlatform,
                NativeGoogleRuntimeFactory.Create, NativeStoreRuntimeFactory.Create);
        }

        private static void RequireSnapshot(SaveSnapshot<LocalProfileState> snapshot, CatalogUse use)
        {
            if (snapshot == null || snapshot.Generation < 1 || snapshot.Profile.Use != use ||
                !snapshot.Payload.Profile.Identity.Equals(snapshot.Profile) || !snapshot.Payload.Profile.CatalogSet.AggregateCatalog.Equals(snapshot.Catalog))
                throw new ArgumentException("MOBILE_CONFIRMED_PROFILE_BINDING_REQUIRED");
            RequireCatalogs(snapshot.Profile, snapshot.Payload.Profile.CatalogSet, use);
        }
        private static void RequireCatalogs(SaveProfileIdentity profile, CatalogSetIdentity catalogs, CatalogUse use)
        {
            var approval = use == CatalogUse.Production ? CatalogApprovalStatus.ProductApproved : use == CatalogUse.ContractTest ? CatalogApprovalStatus.FixtureOnly : CatalogApprovalStatus.Draft;
            if (profile.Use != use || new[] { catalogs.PublicCatalog, catalogs.CampaignCatalog, catalogs.CompletionCatalog, catalogs.AggregateCatalog }
                .Any(c => c.Use != use || c.ApprovalStatus != approval)) throw new ArgumentException("MOBILE_PROFILE_CATALOG_BINDING_REQUIRED");
            if (use == CatalogUse.InternalPreview && profile.Namespace != SaveProfileIdentity.MobileCandidateNamespace)
                throw new ArgumentException("MOBILE_CANDIDATE_PROFILE_REQUIRED");
        }
        /// <summary>Pure build-profile diagnostics, independent of the current Editor target. No SDK, network or readiness attestation.</summary>
        public static IReadOnlyList<string> ProductionConfigurationIssues(string applicationId, RuntimePlatform expectedPlatform,
            GoogleAdConfiguration? ads, ProductionStoreConfiguration? purchases, IapPrivacyReadiness? reviewed, bool requirePurchases = true)
        {
            var result = new List<string>();
            try { MobileApplicationProfile.Validate(applicationId, true); } catch { result.Add("MOBILE_PRODUCTION_APPLICATION_ID_REQUIRED"); }
            if (expectedPlatform != RuntimePlatform.Android && expectedPlatform != RuntimePlatform.IPhonePlayer) result.Add("MOBILE_PRODUCTION_PLATFORM_REQUIRED");
            if (ads == null) result.Add("MOBILE_PRODUCTION_AD_UNITS_REQUIRED");
            else
            {
                if (!ads.Production || ads.ApplicationId != applicationId) result.Add("MOBILE_PRODUCTION_AD_PROFILE_MISMATCH");
                if (!System.Text.RegularExpressions.Regex.IsMatch(ads.InterstitialId, @"\Aca-app-pub-[0-9]{16}/[0-9]{10}\z") ||
                    !System.Text.RegularExpressions.Regex.IsMatch(ads.RewardedId, @"\Aca-app-pub-[0-9]{16}/[0-9]{10}\z")) result.Add("MOBILE_PRODUCTION_AD_UNITS_INVALID");
            }
            if (requirePurchases)
            {
                if (purchases == null) result.Add("IAP_PRODUCTION_CONFIGURATION_REQUIRED");
                else
                {
                    var c = purchases.Configuration;
                    if (c.ApplicationId != applicationId || (expectedPlatform == RuntimePlatform.Android ? c.Store != "GooglePlay" : c.Store != "AppleAppStore")) result.Add("IAP_PRODUCTION_PROFILE_MISMATCH");
                    try { purchases.RequireReadiness(reviewed!); } catch (ArgumentException error) { result.Add(error.Message); }
                }
                if (reviewed == null && purchases == null) result.Add("IAP_PRIVACY_READINESS_REQUIRED");
            }
            return result.AsReadOnly();
        }
        private MobileCandidateServices(ILocalProfileRepository repository, SaveProfileIdentity profile, CatalogSetIdentity catalogs,
            ILocalSaveDispatcher dispatcher, ILocalSessionClock clock,
            Func<Func<CancellationToken, Task<SaveCommitResult<LocalProfileState>>>, CancellationToken, Task<SaveCommitResult<LocalProfileState>>> runProfileTransaction,
            Func<AdRewardEvidence, CancellationToken, Task<SaveCommitResult<LocalProfileState>>> applyReward,
            GoogleAdConfiguration ads, CatalogUse use, RuntimePlatform? expectedPlatform,
            Func<GoogleAdConfiguration, IGoogleMobileRuntime> createGoogle, Func<StoreConfiguration, IUnityStoreRuntime> createStore)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.profile = profile ?? throw new ArgumentNullException(nameof(profile)); this.catalogs = catalogs ?? throw new ArgumentNullException(nameof(catalogs));
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher)); this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this.runProfileTransaction = runProfileTransaction ?? throw new ArgumentNullException(nameof(runProfileTransaction)); this.applyReward = applyReward ?? throw new ArgumentNullException(nameof(applyReward));
            adConfiguration = ads ?? throw new ArgumentNullException(nameof(ads)); createStoreRuntime = createStore ?? throw new ArgumentNullException(nameof(createStore));
            if (use == CatalogUse.InternalPreview && (profile.Use != CatalogUse.InternalPreview || profile.Namespace != SaveProfileIdentity.MobileCandidateNamespace || catalogs.AggregateCatalog.Use != CatalogUse.InternalPreview))
                throw new ArgumentException("MOBILE_CANDIDATE_PROFILE_REQUIRED");
            RequireCatalogs(profile, catalogs, use); ownerThread = Thread.CurrentThread.ManagedThreadId;
            RequireInlineOwner(dispatcher, ownerThread);
            production = use == CatalogUse.Production; productionPlatform = expectedPlatform;
            if (production) RequireProductionRuntime();
            else if (ads.Production || use == CatalogUse.InternalPreview && ads.ApplicationId != MobileApplicationProfile.CandidateApplicationId)
                throw new ArgumentException("MOBILE_CANDIDATE_AD_PROFILE_REQUIRED");
            // Both runtime wrappers have inert constructors. Native initialization remains in the existing consent/readiness-gated operations.
            google = new GoogleMobileServices(createGoogle(ads), Post);
            privacyTransactions = PersistedMobileTransactions.ForConsent(repository, profile, catalogs, dispatcher, clock);
            interstitials = new InterstitialPolicy(profile, catalogs.AggregateCatalog);
            google.RewardEarned += RewardReceived;
        }
        private static void RequireInlineOwner(ILocalSaveDispatcher dispatcher, int caller)
        {
            bool matched = false;
            var confirmation = dispatcher.InvokeAsync(() => matched = Thread.CurrentThread.ManagedThreadId == caller);
            if (!confirmation.IsCompleted || confirmation.IsCanceled || confirmation.IsFaulted || !matched)
            {
                // A queued marker has no provider or mutation effect; observe a disposed-dispatcher failure without blocking its owner.
                _ = confirmation.ContinueWith(t => { _ = t.Exception; }, CancellationToken.None,
                    TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
                throw new InvalidOperationException("MOBILE_DISPATCH_OWNER_REQUIRED");
            }
        }
        // Pure exact boundary, also used by controlled fixture negatives without calling Unity or a native SDK.
        internal static bool RuntimeProfileMatches(string expectedApplicationId, RuntimePlatform expectedPlatform, string actualApplicationId, RuntimePlatform actualPlatform)
            => (expectedPlatform == RuntimePlatform.Android || expectedPlatform == RuntimePlatform.IPhonePlayer) &&
                actualPlatform == expectedPlatform && string.Equals(actualApplicationId, expectedApplicationId, StringComparison.Ordinal);
        private void RequireProductionRuntime()
        {
            if (Thread.CurrentThread.ManagedThreadId != ownerThread) throw new InvalidOperationException("MOBILE_PRODUCTION_OWNER_REQUIRED");
            if (!productionPlatform.HasValue || !RuntimeProfileMatches(adConfiguration.ApplicationId, productionPlatform.Value,
                UnityEngine.Application.identifier, UnityEngine.Application.platform))
                throw new ArgumentException("MOBILE_PRODUCTION_RUNTIME_PROFILE_REQUIRED");
        }
        private async Task<bool> RuntimeAllowed()
        {
            if (!production) return !disposed;
            bool allowed = false;
            await dispatcher.InvokeAsync(() =>
            {
                productionInputWindowClosed = true;
                if (disposed) return;
                try { RequireProductionRuntime(); allowed = true; } catch (ArgumentException) { }
            }).ConfigureAwait(false);
            return allowed;
        }
#if UNITY_INCLUDE_TESTS
        /// <summary>Separate controlled fixture composition. It cannot consume Production, run native factories or appear in a player without tests.</summary>
        internal static MobileCandidateServices CreateForContractTests(ILocalProfileRepository repository, SaveSnapshot<LocalProfileState> confirmedProfile,
            ILocalSaveDispatcher dispatcher, ILocalSessionClock clock,
            Func<Func<CancellationToken, Task<SaveCommitResult<LocalProfileState>>>, CancellationToken, Task<SaveCommitResult<LocalProfileState>>> runProfileTransaction,
            Func<AdRewardEvidence, CancellationToken, Task<SaveCommitResult<LocalProfileState>>> applyReward,
            Func<GoogleAdConfiguration, IGoogleMobileRuntime> google, Func<StoreConfiguration, IUnityStoreRuntime> store)
        {
            RequireSnapshot(confirmedProfile, CatalogUse.ContractTest);
            return new MobileCandidateServices(repository, confirmedProfile.Profile, confirmedProfile.Payload.Profile.CatalogSet, dispatcher, clock,
                runProfileTransaction, applyReward, GoogleAdConfiguration.TestAds("contract.stp.qa", false), CatalogUse.ContractTest, null, google, store);
        }
        internal void ConfigureContractTestStore(StoreConfiguration configuration, IapPrivacyReadiness reviewed)
        {
            if (profile.Use != CatalogUse.ContractTest || configuration == null || configuration.Production || configuration.ApplicationId != adConfiguration.ApplicationId)
                throw new ArgumentException("IAP_CONTRACT_TEST_PROFILE_REQUIRED");
            if (reviewed == null || reviewed.PolicyRevision != configuration.PolicyRevision || reviewed.SdkRevision != configuration.SdkRevision)
                throw new ArgumentException("IAP_REVIEWED_INPUTS_MISMATCH");
            BindStore(configuration, reviewed);
        }
#endif

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

        private MobileOperation Op(string purpose, CancellationToken cancellation, int seconds = 30) => new MobileOperation((production ? "production:" : "candidate:") + purpose + ":" + Guid.NewGuid().ToString("N"), TimeSpan.FromSeconds(seconds), cancellation);
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
            if (!await RuntimeAllowed().ConfigureAwait(false)) return MobileResult<ConsentSnapshot>.Stop(operation, MobileServiceStatus.Unavailable, "MOBILE_RUNTIME_PROFILE_UNAVAILABLE");
            var result = await (options ? google.ShowPrivacyOptionsAsync(operation) : google.RefreshAsync(operation)).ConfigureAwait(false);
            if (result.Status != MobileServiceStatus.Succeeded || result.Value == null) return result;
            var committed = await runProfileTransaction(ct => privacyTransactions.PersistConsentAsync(google, result.Value, ct), cancellation).ConfigureAwait(false);
            if (!committed.Succeeded) return MobileResult<ConsentSnapshot>.Stop(operation, MobileServiceStatus.Pending, "CONSENT_DURABLE_SYNC_REQUIRED");
            return result;
        }
        /// <summary>Immediate effective off, then the same atomically persisted revocation protocol.</summary>
        public Task<SaveCommitResult<LocalProfileState>> RevokeConsentAsync(CancellationToken cancellation)
        {
            if (production) productionInputWindowClosed = true;
            google.Revoke();
            return runProfileTransaction(ct => privacyTransactions.RevokeConsentAsync(google, ct), cancellation);
        }

        /// <summary>Supply only actual QA-store metadata and qualified readiness; construction itself still does not start IAP.</summary>
        public void ConfigureStore(StoreConfiguration configuration, IapPrivacyReadiness? reviewedReadiness)
        {
            if (production || disposed || store != null) throw new InvalidOperationException("MOBILE_STORE_ALREADY_BOUND");
            if (configuration == null || configuration.Production || configuration.ApplicationId != MobileApplicationProfile.CandidateApplicationId)
                throw new ArgumentException("MOBILE_CANDIDATE_STORE_PROFILE_REQUIRED");
            if (reviewedReadiness != null && (reviewedReadiness.PolicyRevision != configuration.PolicyRevision || reviewedReadiness.SdkRevision != configuration.SdkRevision || (configuration.Store == "GooglePlay" && string.IsNullOrWhiteSpace(configuration.GooglePublicKey))))
                throw new ArgumentException("MOBILE_STORE_REVIEWED_INPUTS_MISMATCH");
            BindStore(configuration, reviewedReadiness);
        }
        /// <summary>Capture explicit reviewed production inputs once before the first provider action; no SDK is constructed here.</summary>
        public void ConfigureProductionStore(ProductionStoreConfiguration actualConfiguration, IapPrivacyReadiness reviewedReadiness)
        {
            if (Thread.CurrentThread.ManagedThreadId != ownerThread) throw new InvalidOperationException("MOBILE_PRODUCTION_OWNER_REQUIRED");
            if (!production || disposed || store != null || productionInputWindowClosed) throw new InvalidOperationException("MOBILE_PRODUCTION_STORE_INPUT_WINDOW_CLOSED");
            if (actualConfiguration == null) throw new ArgumentException("IAP_PRODUCTION_CONFIGURATION_REQUIRED");
            actualConfiguration.RequireReadiness(reviewedReadiness);
            var configuration = actualConfiguration.Configuration;
            if (configuration.ApplicationId != adConfiguration.ApplicationId ||
                (productionPlatform == RuntimePlatform.Android ? configuration.Store != "GooglePlay" : configuration.Store != "AppleAppStore"))
                throw new ArgumentException("IAP_PRODUCTION_PROFILE_MISMATCH");
            RequireProductionRuntime(); BindStore(configuration, reviewedReadiness);
        }
        private void BindStore(StoreConfiguration configuration, IapPrivacyReadiness? reviewedReadiness)
        {
            if (disposed || store != null) throw new InvalidOperationException("MOBILE_STORE_ALREADY_BOUND");
            storeTransactions = new PersistedMobileTransactions(repository, profile, catalogs, dispatcher, clock,
                new MobilePurchaseBinding(configuration.Store, configuration.ApplicationId, configuration.StoreProductId, configuration.Production));
            readiness = reviewedReadiness;
            store = new UnityStorePurchaseAdapter(configuration, () =>
            {
                if (production) RequireProductionRuntime();
                return createStoreRuntime(configuration);
            }, Post);
            store.EvidenceReceived += EvidenceReceived; store.EntitlementRevoked += RevocationReceived;
        }
        /// <summary>Fetches the real localized product only for an explicit purchase/restore intent and actual readiness.</summary>
        public async Task<MobileResult<StoreProduct>> PrepareStoreAsync(PurchaseIntent intent, CancellationToken cancellation)
        {
            var operation = Op("store-prepare", cancellation);
            var adapter = store;
            if (disposed || adapter == null) return MobileResult<StoreProduct>.Stop(operation, MobileServiceStatus.NotAllowed, "IAP_ACTUAL_CONFIGURATION_REQUIRED");
            if (!await RuntimeAllowed().ConfigureAwait(false)) return MobileResult<StoreProduct>.Stop(operation, MobileServiceStatus.Unavailable, "MOBILE_RUNTIME_PROFILE_UNAVAILABLE");
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
            if (!await RuntimeAllowed().ConfigureAwait(false)) return MobileResult<PurchaseEvidence>.Stop(operation, MobileServiceStatus.Unavailable, "MOBILE_RUNTIME_PROFILE_UNAVAILABLE");
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
            if (!await RuntimeAllowed().ConfigureAwait(false)) return MobileResult<AdPresentation>.Stop(operation, MobileServiceStatus.Unavailable, "MOBILE_RUNTIME_PROFILE_UNAVAILABLE");
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
                if (!await RuntimeAllowed().ConfigureAwait(false)) return MobileResult<AdPresentation>.Stop(operation, MobileServiceStatus.Unavailable, "MOBILE_RUNTIME_PROFILE_UNAVAILABLE");
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
