using System;
using System.Threading;
using System.Threading.Tasks;

namespace STP.Application
{
    /// <summary>Shared application identity fence; a test service never uses a production package.</summary>
    public static class MobileApplicationProfile
    {
        /// <summary>The separately composed internal candidate, never a production identity.</summary>
        public const string CandidateApplicationId = "com.STP.StammstreckenPuzzle.qa";
        /// <summary>Checks both directions of the production/isolated suffix binding before any SDK exists.</summary>
        public static void Validate(string applicationId, bool production)
        {
            if (string.IsNullOrWhiteSpace(applicationId)) throw new ArgumentException("MOBILE_APPLICATION_ID_REQUIRED");
            if (applicationId.EndsWith(".human", StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("MOBILE_HUMAN_PROVIDER_FORBIDDEN");
            bool isolated = applicationId.EndsWith(".qa", StringComparison.OrdinalIgnoreCase) || applicationId.EndsWith(".dev", StringComparison.OrdinalIgnoreCase) || applicationId.EndsWith(".staging", StringComparison.OrdinalIgnoreCase);
            if (isolated == production) throw new ArgumentException("MOBILE_APPLICATION_PROFILE_MISMATCH");
        }
    }

    /// <summary>Closed provider-neutral operation outcome. A timeout never means provider cancellation.</summary>
    public enum MobileServiceStatus { Succeeded, Unavailable, NotAllowed, Cancelled, TimedOut, Pending, Failed }

    /// <summary>Locally allocated immutable request identity and bounded caller lifetime.</summary>
    public sealed class MobileOperation
    {
        /// <summary>Creates an operation. A caller must never reuse an ID for a different request.</summary>
        public MobileOperation(string id, TimeSpan timeout, CancellationToken cancellation = default)
        {
            if (string.IsNullOrWhiteSpace(id) || id.Length > 160) throw new ArgumentException("MOBILE_OPERATION_ID", nameof(id));
            if (timeout <= TimeSpan.Zero || timeout > TimeSpan.FromMinutes(3)) throw new ArgumentOutOfRangeException(nameof(timeout));
            Id = id; Timeout = timeout; Cancellation = cancellation;
        }
        /// <summary>Opaque local ID; never an advertising or store token.</summary>
        public string Id { get; }
        /// <summary>Maximum wait for the local result.</summary>
        public TimeSpan Timeout { get; }
        /// <summary>Local cancellation; the provider can still complete a started transaction.</summary>
        public CancellationToken Cancellation { get; }
    }

    /// <summary>Immutable result without SDK types, exception text or credentials.</summary>
    public sealed class MobileResult<T> where T : class
    {
        private MobileResult(string operationId, MobileServiceStatus status, string code, T? value)
        { OperationId = operationId; Status = status; Code = code; Value = value; }
        /// <summary>Original local request ID.</summary>
        public string OperationId { get; }
        /// <summary>Discriminated outcome.</summary>
        public MobileServiceStatus Status { get; }
        /// <summary>Stable redacted diagnostic code.</summary>
        public string Code { get; }
        /// <summary>Payload for success; absent for all other outcomes.</summary>
        public T? Value { get; }
        /// <summary>Creates a successful result with a non-null payload.</summary>
        public static MobileResult<T> Success(MobileOperation operation, T value) => new MobileResult<T>(operation.Id, MobileServiceStatus.Succeeded, "MOBILE_OK", value ?? throw new ArgumentNullException(nameof(value)));
        /// <summary>Creates a non-successful result with no payload.</summary>
        public static MobileResult<T> Stop(MobileOperation operation, MobileServiceStatus status, string code)
        {
            if (status == MobileServiceStatus.Succeeded) throw new ArgumentException("MOBILE_SUCCESS_REQUIRES_VALUE", nameof(status));
            return new MobileResult<T>(operation.Id, status, code, null);
        }
    }

    /// <summary>One caller result with cancellation/timeout races resolved atomically.</summary>
    public sealed class MobileOperationCompletion<T> : IDisposable where T : class
    {
        private readonly TaskCompletionSource<MobileResult<T>> source = new TaskCompletionSource<MobileResult<T>>(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly Timer timer;
        private readonly CancellationTokenRegistration registration;
        /// <summary>Starts the caller deadline without initiating a provider operation.</summary>
        public MobileOperationCompletion(MobileOperation operation)
        {
            Operation = operation;
            timer = new Timer(_ => Stop(MobileServiceStatus.TimedOut, "MOBILE_TIMEOUT"), null, operation.Timeout, Timeout.InfiniteTimeSpan);
            registration = operation.Cancellation.Register(() => Stop(MobileServiceStatus.Cancelled, "MOBILE_CANCELLED"));
        }
        /// <summary>Original request.</summary>
        public MobileOperation Operation { get; }
        /// <summary>Exactly one result even when callbacks arrive repeatedly or late.</summary>
        public Task<MobileResult<T>> Task => source.Task;
        /// <summary>Whether the local result has already resolved.</summary>
        public bool IsCompleted => source.Task.IsCompleted;
        /// <summary>Attempts success; false means an earlier result already won.</summary>
        public bool Succeed(T value) => source.TrySetResult(MobileResult<T>.Success(Operation, value));
        /// <summary>Attempts a non-success result.</summary>
        public bool Stop(MobileServiceStatus status, string code) => source.TrySetResult(MobileResult<T>.Stop(Operation, status, code));
        /// <summary>Releases only the caller timer/subscription, without cancelling a native purchase.</summary>
        public void Dispose() { timer.Dispose(); registration.Dispose(); }
    }

    /// <summary>UMP candidate bound to a single successful current flow. Persist before applying it.</summary>
    public sealed class ConsentSnapshot
    {
        /// <summary>Creates an immutable candidate; no advertising identifier is included.</summary>
        public ConsentSnapshot(long revision, bool canRequestAds, bool privacyOptionsRequired)
        { Revision = revision; CanRequestAds = canRequestAds; PrivacyOptionsRequired = privacyOptionsRequired; }
        /// <summary>Process-local monotonic flow revision.</summary>
        public long Revision { get; }
        /// <summary>Current UMP candidate, not an effective permission until persisted.</summary>
        public bool CanRequestAds { get; }
        /// <summary>Whether the app must expose privacy options.</summary>
        public bool PrivacyOptionsRequired { get; }
    }

    /// <summary>Explicit effective capabilities; analytics/crash are absent from launch.</summary>
    public sealed class MobileCapabilities
    {
        /// <summary>Creates the effective ads state.</summary>
        public MobileCapabilities(bool ads, bool privacyOptionsRequired) { CanRequestAds = ads; PrivacyOptionsRequired = privacyOptionsRequired; }
        /// <summary>Effective permission after current UMP and durable Application decision.</summary>
        public bool CanRequestAds { get; }
        /// <summary>Required visible privacy entry.</summary>
        public bool PrivacyOptionsRequired { get; }
        /// <summary>Launch always excludes analytics.</summary>
        public bool CanSendAnalytics => false;
        /// <summary>Launch always excludes external crash collection.</summary>
        public bool CanSendCrashReports => false;
    }

    /// <summary>Supported ad formats.</summary>
    public enum MobileAdKind { Interstitial, Rewarded }
    /// <summary>Once-only loaded object reference, opaque outside its adapter.</summary>
    public sealed class LoadedAd
    {
        /// <summary>Creates an adapter handle.</summary>
        public LoadedAd(string handle, MobileAdKind kind) { Handle = handle; Kind = kind; }
        /// <summary>Opaque process handle.</summary>
        public string Handle { get; }
        /// <summary>Loaded format.</summary>
        public MobileAdKind Kind { get; }
    }
    /// <summary>Application's already persisted reward reservation; contains no reward amount.</summary>
    public sealed class AdRewardReservation
    {
        /// <summary>Creates the binding after a successful reservation commit.</summary>
        public AdRewardReservation(string claimId, string operationId, long saveGeneration)
        {
            if (string.IsNullOrWhiteSpace(claimId) || string.IsNullOrWhiteSpace(operationId) || saveGeneration < 1) throw new ArgumentException("AD_RESERVATION_INVALID");
            ClaimId = claimId; OperationId = operationId; SaveGeneration = saveGeneration;
        }
        /// <summary>Persistent business deduplication key.</summary>
        public string ClaimId { get; }
        /// <summary>Operation committed before SDK show.</summary>
        public string OperationId { get; }
        /// <summary>Successfully committed reservation generation.</summary>
        public long SaveGeneration { get; }
    }
    /// <summary>Verified provider reward callback, including callbacks after a local timeout.</summary>
    public sealed class AdRewardEvidence
    {
        /// <summary>Creates evidence; Application must still perform its idempotent atomic grant.</summary>
        public AdRewardEvidence(AdRewardReservation reservation, string providerOperationId) { Reservation = reservation; ProviderOperationId = providerOperationId; }
        /// <summary>Exact original business reservation.</summary>
        public AdRewardReservation Reservation { get; }
        /// <summary>Opaque local adapter audit handle, never an advertising identifier.</summary>
        public string ProviderOperationId { get; }
    }
    /// <summary>Completed full-screen presentation; rewards arrive only through RewardEarned.</summary>
    public sealed class AdPresentation
    {
        /// <summary>Creates a completed presentation.</summary>
        public AdPresentation(bool wasShown, bool rewardObserved) { WasShown = wasShown; RewardObserved = rewardObserved; }
        /// <summary>A full screen open callback was observed.</summary>
        public bool WasShown { get; }
        /// <summary>Informational only; do not grant a reward from this property.</summary>
        public bool RewardObserved { get; }
    }

    /// <summary>Explicit SDK/legal readiness bound to the current policy and SDK revision.</summary>
    public sealed class IapPrivacyReadiness
    {
        /// <summary>Construct only from a reviewed release record; this type is not legal approval.</summary>
        public IapPrivacyReadiness(string policyRevision, string sdkRevision, string evidenceSha256)
        {
            if (string.IsNullOrWhiteSpace(policyRevision) || string.IsNullOrWhiteSpace(sdkRevision) || evidenceSha256 == null || evidenceSha256.Length != 64) throw new ArgumentException("IAP_READINESS_INVALID");
            foreach (var c in evidenceSha256) if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f'))) throw new ArgumentException("IAP_READINESS_HASH_INVALID");
            PolicyRevision = policyRevision; SdkRevision = sdkRevision; EvidenceSha256 = evidenceSha256;
        }
        /// <summary>Reviewed policy revision.</summary>
        public string PolicyRevision { get; }
        /// <summary>Exact reviewed SDK contract.</summary>
        public string SdkRevision { get; }
        /// <summary>Release evidence reference, not a secret.</summary>
        public string EvidenceSha256 { get; }
    }
    /// <summary>Visible user intent or previously persisted recovery, never bootstrap.</summary>
    public enum PurchaseIntent { Purchase, Restore, PersistentRecovery }
    /// <summary>Metadata fetched from the actual store for the sole non-consumable.</summary>
    public sealed class StoreProduct
    {
        /// <summary>Creates store-owned display metadata.</summary>
        public StoreProduct(string storeProductId, string localizedPrice, string title) { StoreProductId = storeProductId; LocalizedPrice = localizedPrice; Title = title; }
        /// <summary>Only supported logical product.</summary>
        public string LogicalProductId => "remove_ads";
        /// <summary>Configured platform-specific ID.</summary>
        public string StoreProductId { get; }
        /// <summary>Actual localized store price; never replace with a hardcoded price.</summary>
        public string LocalizedPrice { get; }
        /// <summary>Localized store title.</summary>
        public string Title { get; }
    }
    /// <summary>Locally verified purchase; no raw receipt/token crosses the adapter.</summary>
    public sealed class PurchaseEvidence
    {
        /// <summary>Creates adapter evidence after cryptographic/native verification.</summary>
        public PurchaseEvidence(string operationId, string transactionKey, string handle, string store, string environment, string applicationId, string storeProductId, bool alreadyFinalized)
        { OperationId = operationId; TransactionKey = transactionKey; Handle = handle; Store = store; Environment = environment; ApplicationId = applicationId; StoreProductId = storeProductId; AlreadyFinalized = alreadyFinalized; }
        /// <summary>Original caller or recovery operation.</summary>
        public string OperationId { get; }
        /// <summary>Namespaced SHA256 of the store transaction/token, persistent deduplication key.</summary>
        public string TransactionKey { get; }
        /// <summary>Opaque process-local handle for finalization.</summary>
        public string Handle { get; }
        /// <summary>GooglePlay or AppleAppStore.</summary>
        public string Store { get; }
        /// <summary>Explicit configured store environment.</summary>
        public string Environment { get; }
        /// <summary>Verified package/bundle identity.</summary>
        public string ApplicationId { get; }
        /// <summary>Verified non-consumable product identity.</summary>
        public string StoreProductId { get; }
        /// <summary>Store fetched a confirmed order; no further native finish is needed.</summary>
        public bool AlreadyFinalized { get; }
    }
    /// <summary>Explicit native verified revocation, with a process-local adapter provenance handle.</summary>
    public sealed class PurchaseRevocationEvidence
    {
        /// <summary>Only the adapter's ownership predicate makes this DTO authoritative.</summary>
        public PurchaseRevocationEvidence(string transactionKey, string handle, string store, string environment, string applicationId, string storeProductId)
        { TransactionKey = transactionKey; Handle = handle; Store = store; Environment = environment; ApplicationId = applicationId; StoreProductId = storeProductId; }
        /// <summary>Exact revoked transaction, never absence of an owned purchase.</summary>
        public string TransactionKey { get; }
        /// <summary>Adapter-owned opaque proof handle.</summary>
        public string Handle { get; }
        /// <summary>Verified native store.</summary>
        public string Store { get; }
        /// <summary>Verified native environment or explicitly isolated Google profile.</summary>
        public string Environment { get; }
        /// <summary>Verified app identity.</summary>
        public string ApplicationId { get; }
        /// <summary>Verified sole product identity.</summary>
        public string StoreProductId { get; }
    }

    /// <summary>Application attestation issued only after durable entitlement+operation commit.</summary>
    public sealed class DurablePurchaseGrant
    {
        /// <summary>Binds a successful commit to its original evidence.</summary>
        public DurablePurchaseGrant(PurchaseEvidence evidence, long committedGeneration)
        {
            if (committedGeneration < 1) throw new ArgumentOutOfRangeException(nameof(committedGeneration));
            Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence)); CommittedGeneration = committedGeneration;
        }
        /// <summary>Original adapter evidence.</summary>
        public PurchaseEvidence Evidence { get; }
        /// <summary>Actual committed save generation; not a pending save request.</summary>
        public long CommittedGeneration { get; }
    }
    /// <summary>Positive finalization/restore response; never conveys a new entitlement.</summary>
    public sealed class PurchaseAcknowledgement
    {
        /// <summary>Creates an acknowledgement for a transaction or restore operation.</summary>
        public PurchaseAcknowledgement(string transactionKey) { TransactionKey = transactionKey; }
        /// <summary>Bound key, empty only for a successful empty restore.</summary>
        public string TransactionKey { get; }
    }
}
