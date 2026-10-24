using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;
using STP.Application.Ports;

namespace STP.MobileServices.Store
{
    /// <summary>Explicit immutable profile. Its public Play key is not a credential.</summary>
    public sealed class StoreConfiguration
    {
        /// <summary>Creates the sole product binding, without connecting to any service.</summary>
        public StoreConfiguration(string applicationId, string storeProductId, string store, bool production, string policyRevision, string googlePublicKey = "")
        {
            if (string.IsNullOrWhiteSpace(applicationId) || string.IsNullOrWhiteSpace(storeProductId) || string.IsNullOrWhiteSpace(policyRevision) || (store != "GooglePlay" && store != "AppleAppStore")) throw new ArgumentException("STORE_CONFIGURATION_INVALID");
            MobileApplicationProfile.Validate(applicationId, production);
            ApplicationId = applicationId; StoreProductId = storeProductId; Store = store; Production = production; PolicyRevision = policyRevision; GooglePublicKey = googlePublicKey;
        }
        /// <summary>Actual package/bundle.</summary>
        public string ApplicationId { get; }
        /// <summary>Explicit native product ID.</summary>
        public string StoreProductId { get; }
        /// <summary>Exactly GooglePlay or AppleAppStore.</summary>
        public string Store { get; }
        /// <summary>Production identity; Apple still uses Sandbox through TestFlight.</summary>
        public bool Production { get; }
        /// <summary>Reviewed policy record revision.</summary>
        public string PolicyRevision { get; }
        /// <summary>Official non-secret Google Play licensing public key for local cryptographic validation.</summary>
        public string GooglePublicKey { get; }
        /// <summary>Exact installed SDK contract.</summary>
        public string SdkRevision => "unity-iap-5.4.3";
    }

    /// <summary>Adapter-local transient native transaction. Never passed to Application.</summary>
    public sealed class StoreTransaction
    {
        /// <summary>Creates an untrusted native candidate, requiring Verify before any evidence.</summary>
        public StoreTransaction(string nativeId, string productId, bool finalized, object? nativeHandle)
        { NativeId = nativeId; ProductId = productId; Finalized = finalized; NativeHandle = nativeHandle; }
        /// <summary>Transient token/transaction ID, never logged or persisted by this module.</summary>
        public string NativeId { get; }
        /// <summary>Native candidate product.</summary>
        public string ProductId { get; }
        /// <summary>Native order was already finalized.</summary>
        public bool Finalized { get; }
        /// <summary>Native SDK object remains within this module.</summary>
        public object? NativeHandle { get; }
    }
    /// <summary>Result from actual cryptographic/native verification.</summary>
    public sealed class VerifiedStoreTransaction
    {
        /// <summary>Creates the verified identity after matching signature, app, product and environment.</summary>
        public VerifiedStoreTransaction(string nativeId, string environment, bool revoked) { NativeId = nativeId; Environment = environment; Revoked = revoked; }
        /// <summary>Verified native transaction; never passed to Application.</summary>
        public string NativeId { get; }
        /// <summary>Verified Apple environment or explicitly isolated Google profile.</summary>
        public string Environment { get; }
        /// <summary>Explicit verified revocation, never inferred from missing purchases.</summary>
        public bool Revoked { get; }
    }
    /// <summary>Native IAP boundary, owned only by the Store adapter assembly.</summary>
    public interface IUnityStoreRuntime : IDisposable
    {
        /// <summary>True only for the configured installed app/platform.</summary>
        bool Supported { get; }
        /// <summary>Untrusted native candidates, including replay and explicit revocation.</summary>
        event Action<StoreTransaction>? TransactionReceived;
        /// <summary>Pending/cancelled/failed user purchase status, with no SDK text.</summary>
        event Action<MobileServiceStatus>? PurchaseStopped;
        /// <summary>Connects and fetches a single configured non-consumable.</summary>
        void ConnectAndFetch(Action<StoreProduct?> completed);
        /// <summary>Starts only the configured product purchase.</summary>
        void Purchase();
        /// <summary>Runs explicit store restore and fetches verified candidates.</summary>
        void Restore(Action<bool> completed);
        /// <summary>Cryptographic/native validation; null is never a grant.</summary>
        void Verify(StoreTransaction transaction, Action<VerifiedStoreTransaction?> completed);
        /// <summary>Finishes a pending native transaction after the Application durable grant.</summary>
        void Confirm(StoreTransaction transaction, Action<bool> completed);
    }

    /// <summary>Optional thread-safe native presentation fact, independent of a deferred payment or background product query.</summary>
    public interface IUnityStoreNativeUiState
    {
        /// <summary>True from a possible native purchase/restore UI call until an actual outcome callback or disposal.</summary>
        bool IsNativeUiBusy { get; }
    }

    /// <summary>Lazy IAP adapter. Entitlements and persisted operations remain exclusively in Application.</summary>
    public sealed class UnityStorePurchaseAdapter : IPurchasePort, IDisposable
    {
        private sealed class Issued
        {
            internal Issued(PurchaseEvidence evidence, StoreTransaction transaction) { Evidence = evidence; Transaction = transaction; }
            internal readonly PurchaseEvidence Evidence;
            internal readonly StoreTransaction Transaction;
            internal bool Confirming;
            internal bool Finalized;
            internal bool Revoked;
        }
        private readonly StoreConfiguration configuration;
        private readonly Func<IUnityStoreRuntime> createRuntime;
        private readonly Action<Action> post;
        private readonly int ownerThread;
        private readonly Dictionary<string, Issued> issued = new Dictionary<string, Issued>(StringComparer.Ordinal);
        private readonly Dictionary<string, PurchaseRevocationEvidence> revocations = new Dictionary<string, PurchaseRevocationEvidence>(StringComparer.Ordinal);
        private readonly HashSet<string> verifying = new HashSet<string>(StringComparer.Ordinal);
        private volatile IUnityStoreRuntime? sdk;
        private StoreProduct? product;
        private bool preparing;
        private volatile bool disposed;
        private bool restoreNativeDone;
        private bool restoreFailed;
        private MobileOperationCompletion<PurchaseEvidence>? purchase;
        private MobileOperationCompletion<PurchaseAcknowledgement>? restore;

        /// <summary>Construction and factory capture perform no IAP initialization.</summary>
        public UnityStorePurchaseAdapter(StoreConfiguration configuration, Func<IUnityStoreRuntime> createRuntime, Action<Action> post)
        { this.configuration = configuration; this.createRuntime = createRuntime; this.post = post; ownerThread = Thread.CurrentThread.ManagedThreadId; }
        /// <summary>Idempotent Application grant source, also used by late callbacks and restore.</summary>
        public event Action<PurchaseEvidence>? EvidenceReceived;
        /// <summary>Reading presentation state never constructs or starts the IAP SDK.</summary>
        public bool IsNativeUiBusy => !disposed && sdk is IUnityStoreNativeUiState state && state.IsNativeUiBusy;
        /// <summary>Only a positively verified revocation produces this exact transaction key.</summary>
        public event Action<PurchaseRevocationEvidence>? EntitlementRevoked;
        /// <summary>Checks adapter provenance before Application is permitted to grant anything.</summary>
        public bool OwnsVerifiedEvidence(PurchaseEvidence evidence) => Thread.CurrentThread.ManagedThreadId == ownerThread && !disposed && evidence != null && issued.TryGetValue(evidence.Handle, out var entry) && ReferenceEquals(entry.Evidence, evidence) && !entry.Revoked;
        /// <summary>Checks exact verified native revocation provenance before any persistent revoke.</summary>
        public bool OwnsVerifiedRevocation(PurchaseRevocationEvidence evidence) => Thread.CurrentThread.ManagedThreadId == ownerThread && !disposed && evidence != null && revocations.TryGetValue(evidence.Handle, out var proof) && ReferenceEquals(proof, evidence);

        /// <summary>Connects only after explicit intent and exact reviewed policy/SDK readiness.</summary>
        public async Task<MobileResult<StoreProduct>> PrepareAsync(PurchaseIntent intent, IapPrivacyReadiness? readiness, MobileOperation operation)
        {
            using var completion = new MobileOperationCompletion<StoreProduct>(operation);
            Dispatch(() =>
            {
                if (completion.IsCompleted) return;
                if (disposed || readiness == null || readiness.PolicyRevision != configuration.PolicyRevision || readiness.SdkRevision != configuration.SdkRevision || !Enum.IsDefined(typeof(PurchaseIntent), intent))
                { completion.Stop(MobileServiceStatus.NotAllowed, "IAP_PRIVACY_READINESS_REQUIRED"); return; }
                if (purchase != null || restore != null || IsNativeUiBusy) { completion.Stop(MobileServiceStatus.Pending, "IAP_MODAL_PENDING"); return; }
                if (preparing) { completion.Stop(MobileServiceStatus.Pending, "IAP_PREPARE_PENDING"); return; }
                // An explicit new intent reconnects/refetches after a disconnect; a cached price is not store readiness.
                product = null;
                try
                {
                    if (sdk == null)
                    {
                        sdk = createRuntime();
                        sdk.TransactionReceived += OnTransaction;
                        sdk.PurchaseStopped += OnPurchaseStopped;
                    }
                    if (!sdk.Supported) { completion.Stop(MobileServiceStatus.Unavailable, "IAP_PLATFORM_UNAVAILABLE"); return; }
                    preparing = true;
                    sdk.ConnectAndFetch(fetched => Dispatch(() =>
                    {
                        preparing = false;
                        if (disposed) return;
                        if (fetched == null || fetched.StoreProductId != configuration.StoreProductId || string.IsNullOrWhiteSpace(fetched.LocalizedPrice))
                        { completion.Stop(MobileServiceStatus.Unavailable, "IAP_PRODUCT_UNAVAILABLE"); return; }
                        product = fetched;
                        completion.Succeed(fetched);
                    }));
                }
                catch { preparing = false; completion.Stop(MobileServiceStatus.Failed, "IAP_PREPARE_EXCEPTION"); }
            });
            return await completion.Task.ConfigureAwait(false);
        }
        /// <summary>Exact transient product currently prepared by this adapter. A copied DTO or prior fetch is not the displayed offer.</summary>
        public bool IsPreparedProduct(StoreProduct? displayedProduct) => Thread.CurrentThread.ManagedThreadId == ownerThread && !disposed && displayedProduct != null && ReferenceEquals(product, displayedProduct) && !preparing && purchase == null && restore == null && !IsNativeUiBusy;
        /// <summary>Consumes only the exact previously displayed native product, without refetching or starting another purchase.</summary>
        public Task<MobileResult<PurchaseEvidence>> PurchasePreparedAsync(StoreProduct? displayedProduct, MobileOperation operation) => PurchaseCoreAsync(displayedProduct, true, operation);
        /// <summary>Starts purchase with a single active platform transaction. Timeout does not reset it.</summary>
        public Task<MobileResult<PurchaseEvidence>> PurchaseAsync(MobileOperation operation) => PurchaseCoreAsync(null, false, operation);
        private async Task<MobileResult<PurchaseEvidence>> PurchaseCoreAsync(StoreProduct? displayedProduct, bool requireDisplayedProduct, MobileOperation operation)
        {
            using var completion = new MobileOperationCompletion<PurchaseEvidence>(operation);
            Dispatch(() =>
            {
                if (completion.IsCompleted) return;
                if (disposed || sdk == null || product == null) { completion.Stop(MobileServiceStatus.NotAllowed, "IAP_NOT_PREPARED"); return; }
                if (purchase != null || restore != null || IsNativeUiBusy) { completion.Stop(MobileServiceStatus.Pending, "IAP_MODAL_PENDING"); return; }
                if (requireDisplayedProduct && !ReferenceEquals(product, displayedProduct)) { completion.Stop(MobileServiceStatus.NotAllowed, "IAP_DISPLAYED_OFFER_REQUIRED"); return; }
                if (requireDisplayedProduct) product = null;
                purchase = completion;
                try { sdk.Purchase(); }
                catch { purchase = null; completion.Stop(MobileServiceStatus.Failed, "IAP_PURCHASE_EXCEPTION"); }
            });
            return await completion.Task.ConfigureAwait(false);
        }
        /// <summary>Explicit restore. Empty success does not revoke a cached entitlement.</summary>
        public async Task<MobileResult<PurchaseAcknowledgement>> RestoreAsync(MobileOperation operation)
        {
            using var completion = new MobileOperationCompletion<PurchaseAcknowledgement>(operation);
            Dispatch(() =>
            {
                if (completion.IsCompleted) return;
                if (disposed || sdk == null || product == null) { completion.Stop(MobileServiceStatus.NotAllowed, "IAP_NOT_PREPARED"); return; }
                if (purchase != null || restore != null || IsNativeUiBusy) { completion.Stop(MobileServiceStatus.Pending, "IAP_MODAL_PENDING"); return; }
                restore = completion; restoreNativeDone = false; restoreFailed = false;
                try { sdk.Restore(ok => Dispatch(() => { restoreNativeDone = true; restoreFailed |= !ok; CompleteRestore(); })); }
                catch { restore = null; completion.Stop(MobileServiceStatus.Failed, "IAP_RESTORE_EXCEPTION"); }
            });
            return await completion.Task.ConfigureAwait(false);
        }

        private void OnPurchaseStopped(MobileServiceStatus status) => Dispatch(() =>
        {
            var active = purchase;
            // Deferred payments can complete later; no new user purchase is started over the pending one.
            if (status != MobileServiceStatus.Pending) purchase = null;
            active?.Stop(status, status == MobileServiceStatus.Pending ? "IAP_DEFERRED" : status == MobileServiceStatus.Cancelled ? "IAP_USER_CANCELLED" : "IAP_PURCHASE_FAILED");
        });

        private void OnTransaction(StoreTransaction transaction) => Dispatch(() =>
        {
            if (disposed || sdk == null || transaction.ProductId != configuration.StoreProductId || (string.IsNullOrWhiteSpace(transaction.NativeId) && configuration.Store != "AppleAppStore")) return;
            var candidateKey = transaction.NativeId.Length == 0 ? "apple-revocation-query" : TransactionKey(transaction.NativeId);
            if (!verifying.Add(candidateKey)) return;
            var operationId = purchase?.Operation.Id ?? restore?.Operation.Id ?? "iap-recovery:" + Guid.NewGuid().ToString("N");
            try
            {
                sdk.Verify(transaction, verified => Dispatch(() =>
                {
                    if (!verifying.Remove(candidateKey)) return;
                    if (disposed) return;
                    if (verified == null || string.IsNullOrWhiteSpace(verified.NativeId) ||
                        (transaction.NativeId.Length > 0 && transaction.NativeId != verified.NativeId) ||
                        (verified.Environment != "Production" && verified.Environment != "Sandbox") ||
                        (!configuration.Production && verified.Environment != "Sandbox"))
                    {
                        restoreFailed = true;
                        purchase?.Stop(MobileServiceStatus.Failed, "IAP_EVIDENCE_UNVERIFIED"); purchase = null;
                        CompleteRestore(); return;
                    }
                    var key = TransactionKey(verified.NativeId);
                    if (verified.Revoked)
                    {
                        foreach (var item in issued.Values) if (item.Evidence.TransactionKey == key) item.Revoked = true;
                        var proof = revocations.Values.FirstOrDefault(item => item.TransactionKey == key);
                        if (proof == null && revocations.Count < 256)
                        {
                            proof = new PurchaseRevocationEvidence(key, Guid.NewGuid().ToString("N"), configuration.Store, verified.Environment, configuration.ApplicationId, configuration.StoreProductId);
                            revocations.Add(proof.Handle, proof);
                        }
                        if (proof != null) InvokeSafely(EntitlementRevoked, proof);
                        CompleteRestore(); return;
                    }
                    Issued? existing = null;
                    foreach (var item in issued.Values) if (item.Evidence.TransactionKey == key) { existing = item; break; }
                    if (existing != null)
                    {
                        existing.Finalized |= transaction.Finalized;
                        purchase?.Succeed(existing.Evidence); purchase = null;
                        InvokeSafely(EvidenceReceived, existing.Evidence);
                        CompleteRestore(); return;
                    }
                    if (issued.Count >= 256) { purchase?.Stop(MobileServiceStatus.Unavailable, "IAP_RECOVERY_CAPACITY"); purchase = null; restoreFailed = true; CompleteRestore(); return; }
                    var evidence = new PurchaseEvidence(operationId, key, Guid.NewGuid().ToString("N"), configuration.Store, verified.Environment, configuration.ApplicationId, configuration.StoreProductId, transaction.Finalized);
                    issued.Add(evidence.Handle, new Issued(evidence, transaction) { Finalized = transaction.Finalized });
                    InvokeSafely(EvidenceReceived, evidence);
                    purchase?.Succeed(evidence); purchase = null;
                    CompleteRestore();
                }));
            }
            catch { verifying.Remove(candidateKey); restoreFailed = true; purchase?.Stop(MobileServiceStatus.Failed, "IAP_VERIFICATION_EXCEPTION"); purchase = null; CompleteRestore(); }
        });
        private void CompleteRestore()
        {
            if (restore == null || !restoreNativeDone || verifying.Count != 0) return;
            if (restoreFailed) restore.Stop(MobileServiceStatus.Unavailable, "IAP_RESTORE_UNVERIFIED");
            else restore.Succeed(new PurchaseAcknowledgement(string.Empty));
            restore = null;
        }

        /// <summary>Rejects forged evidence or missing durable grant before any native confirm call.</summary>
        public async Task<MobileResult<PurchaseAcknowledgement>> FinalizeAsync(DurablePurchaseGrant grant, MobileOperation operation)
        {
            using var completion = new MobileOperationCompletion<PurchaseAcknowledgement>(operation);
            Dispatch(() =>
            {
                if (completion.IsCompleted) return;
                if (disposed || sdk == null || grant == null || grant.CommittedGeneration < 1 || !issued.TryGetValue(grant.Evidence.Handle, out var entry) || !ReferenceEquals(entry.Evidence, grant.Evidence) || entry.Revoked)
                { completion.Stop(MobileServiceStatus.NotAllowed, "IAP_DURABLE_GRANT_REQUIRED"); return; }
                if (entry.Finalized) { completion.Succeed(new PurchaseAcknowledgement(entry.Evidence.TransactionKey)); return; }
                if (entry.Confirming) { completion.Stop(MobileServiceStatus.Pending, "IAP_CONFIRM_PENDING"); return; }
                entry.Confirming = true;
                try
                {
                    sdk.Confirm(entry.Transaction, ok => Dispatch(() =>
                    {
                        entry.Confirming = false;
                        if (disposed) return;
                        if (!ok) { completion.Stop(MobileServiceStatus.Unavailable, "IAP_CONFIRM_FAILED"); return; }
                        entry.Finalized = true;
                        completion.Succeed(new PurchaseAcknowledgement(entry.Evidence.TransactionKey));
                    }));
                }
                catch { entry.Confirming = false; completion.Stop(MobileServiceStatus.Failed, "IAP_CONFIRM_EXCEPTION"); }
            });
            return await completion.Task.ConfigureAwait(false);
        }
        private string TransactionKey(string nativeId)
        {
            using var hash = SHA256.Create();
            var bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(configuration.Store + "\n" + configuration.ApplicationId + "\n" + configuration.StoreProductId + "\n" + nativeId));
            var text = new StringBuilder(64); foreach (var b in bytes) text.Append(b.ToString("x2", System.Globalization.CultureInfo.InvariantCulture));
            return "iap-v1:" + text;
        }
        private static void InvokeSafely<T>(Action<T>? handlers, T value) { if (handlers == null) return; foreach (Action<T> handler in handlers.GetInvocationList()) try { handler(value); } catch { } }
        private void Dispatch(Action action) { if (Thread.CurrentThread.ManagedThreadId == ownerThread) action(); else post(action); }
        /// <summary>Unsubscribes native events without finalizing or revoking a purchase.</summary>
        public void Dispose() => Dispatch(() =>
        {
            disposed = true; purchase?.Stop(MobileServiceStatus.Cancelled, "IAP_DISPOSED"); restore?.Stop(MobileServiceStatus.Cancelled, "IAP_DISPOSED");
            if (sdk != null) { sdk.TransactionReceived -= OnTransaction; sdk.PurchaseStopped -= OnPurchaseStopped; sdk.Dispose(); }
            issued.Clear(); verifying.Clear(); revocations.Clear();
        });
    }
}
