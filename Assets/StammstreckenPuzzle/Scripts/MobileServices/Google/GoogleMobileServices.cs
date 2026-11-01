using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;
using STP.Application.Ports;

namespace STP.MobileServices.Google
{
    /// <summary>Thin SDK boundary so the same fail-closed adapter is contract-tested without ad traffic.</summary>
    public interface IGoogleMobileRuntime
    {
        /// <summary>Current UMP update followed by any required form.</summary>
        void RefreshConsent(Action<bool, bool, bool> completed);
        /// <summary>Shows privacy options; callback reports success, CanRequestAds and required entry.</summary>
        void ShowPrivacyOptions(Action<bool, bool, bool> completed);
        /// <summary>Initializes actual GMA, called only after effective consent.</summary>
        void InitializeAds(Action<bool> completed);
        /// <summary>Loads one non-personalized ad; null indicates unavailable/no fill.</summary>
        void Load(MobileAdKind kind, Action<IGoogleLoadedAd?> completed);
    }

    /// <summary>Optional runtime presentation fact. Reading it neither starts the SDK nor grants capabilities.</summary>
    public interface IGoogleNativeUiState
    {
        /// <summary>True during a possibly native UMP form, until its actual dismissal callback.</summary>
        bool IsNativeUiBusy { get; }
    }

    /// <summary>One SDK object, never exposed to Application.</summary>
    public interface IGoogleLoadedAd
    {
        /// <summary>Whether the provider permits this once-only show.</summary>
        bool CanShow { get; }
        /// <summary>Shows once. Callback order can vary; reward is independent of caller timeout.</summary>
        void Show(Action opened, Action closed, Action failed, Action rewarded);
        /// <summary>Releases native resources.</summary>
        void Destroy();
    }

    /// <summary>Real UMP/AdMob coordinator. No automatic load, retry, economy or navigation policy.</summary>
    public sealed class GoogleMobileServices : IConsentPort, IAdsPort, IDisposable
    {
        private sealed class Entry
        {
            internal Entry(IGoogleLoadedAd ad, long created, MobileAdKind kind) { Ad = ad; Created = created; Kind = kind; }
            internal readonly IGoogleLoadedAd Ad;
            internal readonly long Created;
            internal readonly MobileAdKind Kind;
        }
        private readonly IGoogleMobileRuntime sdk;
        private readonly Action<Action> post;
        private readonly int ownerThread;
        private readonly Dictionary<string, Entry> loaded = new Dictionary<string, Entry>(StringComparer.Ordinal);
        private MobileCapabilities capabilities = new MobileCapabilities(false, false);
        private ConsentSnapshot? candidate;
        private long epoch;
        private bool consentBusy;
        private bool currentLaunchUmpFlowSucceeded;
        private bool loadBusy;
        private bool showBusy;
        private bool initialized;
        private volatile bool disposed;
        private object? nativeAdPresentation;

        /// <summary>Construct on the Unity main thread; post must dispatch to that same thread.</summary>
        public GoogleMobileServices(IGoogleMobileRuntime sdk, Action<Action> post)
        { this.sdk = sdk ?? throw new ArgumentNullException(nameof(sdk)); this.post = post ?? throw new ArgumentNullException(nameof(post)); ownerThread = Thread.CurrentThread.ManagedThreadId; }
        /// <summary>Effective false until current UMP AND durable decision confirmation.</summary>
        public MobileCapabilities Capabilities => capabilities;
        /// <summary>Thread-safe presentation fact; background consent information, ad initialization and loads do not occupy native UI.</summary>
        public bool IsNativeUiBusy => !disposed && (Volatile.Read(ref nativeAdPresentation) != null || sdk is IGoogleNativeUiState state && state.IsNativeUiBusy);
        /// <summary>Business grant uses this exact evidence, including when the caller already timed out.</summary>
        public event Action<AdRewardEvidence>? RewardEarned;

        /// <summary>Runs the start UMP flow without initializing advertisements.</summary>
        public Task<MobileResult<ConsentSnapshot>> RefreshAsync(MobileOperation operation) => ConsentAsync(operation, false);
        /// <summary>Gates off immediately, discards loads and presents privacy options.</summary>
        public Task<MobileResult<ConsentSnapshot>> ShowPrivacyOptionsAsync(MobileOperation operation) => ConsentAsync(operation, true);

        private async Task<MobileResult<ConsentSnapshot>> ConsentAsync(MobileOperation operation, bool options)
        {
            using var completion = new MobileOperationCompletion<ConsentSnapshot>(operation);
            Dispatch(() =>
            {
                if (disposed) { completion.Stop(MobileServiceStatus.Unavailable, "CONSENT_DISPOSED"); return; }
                if (completion.IsCompleted) return;
                if (consentBusy || showBusy || IsNativeUiBusy) { completion.Stop(MobileServiceStatus.Pending, "MOBILE_MODAL_BUSY"); return; }
                if (options && !currentLaunchUmpFlowSucceeded) { completion.Stop(MobileServiceStatus.NotAllowed, "CONSENT_CURRENT_UPDATE_REQUIRED"); return; }
                if (options && !capabilities.PrivacyOptionsRequired) { completion.Stop(MobileServiceStatus.NotAllowed, "CONSENT_OPTIONS_NOT_REQUIRED"); return; }
                RevokeCore();
                if (!options) currentLaunchUmpFlowSucceeded = false;
                var flow = epoch;
                consentBusy = true;
                try
                {
                    Action<bool, bool, bool> callback = (ok, ads, required) => Dispatch(() =>
                    {
                        consentBusy = false;
                        if (flow != epoch || disposed || completion.IsCompleted) return;
                        capabilities = new MobileCapabilities(false, required);
                        if (!ok) { completion.Stop(MobileServiceStatus.Unavailable, "CONSENT_PROVIDER_UNAVAILABLE"); return; }
                        if (!options) currentLaunchUmpFlowSucceeded = true;
                        candidate = new ConsentSnapshot(flow, ads, required);
                        completion.Succeed(candidate);
                    });
                    if (options) sdk.ShowPrivacyOptions(callback); else sdk.RefreshConsent(callback);
                }
                catch { consentBusy = false; completion.Stop(MobileServiceStatus.Failed, "CONSENT_PROVIDER_EXCEPTION"); }
            });
            return await completion.Task.ConfigureAwait(false);
        }

        /// <summary>Only exact current candidate + positive committed generation enables advertising.</summary>
        public bool ConfirmDecisionPersisted(ConsentSnapshot snapshot, long committedGeneration)
        {
            if (Thread.CurrentThread.ManagedThreadId != ownerThread || disposed || committedGeneration < 1 || !ReferenceEquals(candidate, snapshot) || snapshot.Revision != epoch) return false;
            capabilities = new MobileCapabilities(snapshot.CanRequestAds, snapshot.PrivacyOptionsRequired);
            candidate = null;
            return true;
        }
        /// <summary>Invalidates stale callbacks. A shown ad can still report an already earned reward.</summary>
        public void Revoke() => Dispatch(RevokeCore);
        private void RevokeCore()
        {
            epoch++; candidate = null;
            capabilities = new MobileCapabilities(false, capabilities.PrivacyOptionsRequired);
            DiscardCore();
        }

        /// <summary>Loads after current effective permission and explicit Application request.</summary>
        public async Task<MobileResult<LoadedAd>> LoadAsync(MobileAdKind kind, MobileOperation operation)
        {
            using var completion = new MobileOperationCompletion<LoadedAd>(operation);
            Dispatch(() =>
            {
                if (completion.IsCompleted) return;
                if (!Enum.IsDefined(typeof(MobileAdKind), kind)) { completion.Stop(MobileServiceStatus.NotAllowed, "ADS_FORMAT_INVALID"); return; }
                if (disposed || !capabilities.CanRequestAds) { completion.Stop(MobileServiceStatus.NotAllowed, "ADS_CAPABILITY_REQUIRED"); return; }
                if (loadBusy || showBusy || loaded.Count >= 2) { completion.Stop(MobileServiceStatus.Pending, "ADS_BUSY"); return; }
                loadBusy = true;
                var requestEpoch = epoch;
                void Load()
                {
                    if (completion.IsCompleted || disposed || requestEpoch != epoch || !capabilities.CanRequestAds)
                    { loadBusy = false; completion.Stop(MobileServiceStatus.NotAllowed, "ADS_CONSENT_CHANGED"); return; }
                    try
                    {
                        bool callbackSeen = false;
                        IGoogleLoadedAd? acceptedAd = null;
                        sdk.Load(kind, ad => Dispatch(() =>
                        {
                            if (callbackSeen) { if (!ReferenceEquals(ad, acceptedAd)) Destroy(ad); return; }
                            callbackSeen = true;
                            loadBusy = false;
                            if (disposed || completion.IsCompleted || requestEpoch != epoch || !capabilities.CanRequestAds) { Destroy(ad); return; }
                            if (ad == null) { completion.Stop(MobileServiceStatus.Unavailable, "ADS_NO_FILL"); return; }
                            var handle = Guid.NewGuid().ToString("N");
                            acceptedAd = ad;
                            loaded.Add(handle, new Entry(ad, Stopwatch.GetTimestamp(), kind));
                            completion.Succeed(new LoadedAd(handle, kind));
                        }));
                    }
                    catch { loadBusy = false; completion.Stop(MobileServiceStatus.Failed, "ADS_LOAD_EXCEPTION"); }
                }
                try
                {
                    if (initialized) Load();
                    else
                    {
                        bool initializationCallbackSeen = false;
                        sdk.InitializeAds(ok => Dispatch(() =>
                        {
                            if (initializationCallbackSeen) return;
                            initializationCallbackSeen = true;
                            initialized = ok;
                            if (!ok) { loadBusy = false; completion.Stop(MobileServiceStatus.Unavailable, "ADS_INITIALIZATION_FAILED"); }
                            else Load();
                        }));
                    }
                }
                catch { loadBusy = false; completion.Stop(MobileServiceStatus.Failed, "ADS_INITIALIZATION_EXCEPTION"); }
            });
            return await completion.Task.ConfigureAwait(false);
        }

        /// <summary>Shows once. Application owns eligibility and the already committed reward reservation.</summary>
        public async Task<MobileResult<AdPresentation>> ShowAsync(LoadedAd ad, AdRewardReservation? reservation, MobileOperation operation)
        {
            using var completion = new MobileOperationCompletion<AdPresentation>(operation);
            Dispatch(() =>
            {
                if (completion.IsCompleted) return;
                if (disposed || !capabilities.CanRequestAds) { completion.Stop(MobileServiceStatus.NotAllowed, "ADS_CAPABILITY_REQUIRED"); return; }
                if (showBusy || consentBusy || IsNativeUiBusy) { completion.Stop(MobileServiceStatus.Pending, "MOBILE_MODAL_BUSY"); return; }
                if ((ad.Kind == MobileAdKind.Rewarded && (reservation == null || reservation.OperationId != operation.Id)) || (ad.Kind == MobileAdKind.Interstitial && reservation != null))
                { completion.Stop(MobileServiceStatus.NotAllowed, "ADS_RESERVATION_REQUIRED"); return; }
                if (!loaded.TryGetValue(ad.Handle, out var entry) || entry.Kind != ad.Kind) { completion.Stop(MobileServiceStatus.Unavailable, "ADS_HANDLE_UNKNOWN"); return; }
                loaded.Remove(ad.Handle);
                var ageSeconds = (Stopwatch.GetTimestamp() - entry.Created) / (double)Stopwatch.Frequency;
                if (ageSeconds >= 3500 || !entry.Ad.CanShow) { Destroy(entry.Ad); completion.Stop(MobileServiceStatus.Unavailable, "ADS_EXPIRED"); return; }
                showBusy = true;
                var presentation = new object();
                Interlocked.Exchange(ref nativeAdPresentation, presentation);
                bool opened = false, rewarded = false, terminal = false;
                var providerOperation = operation.Id;
                void Close(bool failed, bool nativeCloseConfirmed)
                {
                    if (nativeCloseConfirmed) Interlocked.CompareExchange(ref nativeAdPresentation, null, presentation);
                    if (terminal) return;
                    terminal = true; showBusy = false;
                    if (failed) completion.Stop(MobileServiceStatus.Unavailable, "ADS_SHOW_FAILED");
                    else completion.Succeed(new AdPresentation(opened, rewarded));
                    Destroy(entry.Ad);
                }
                try
                {
                    entry.Ad.Show(
                        () => Dispatch(() => opened = true),
                        () => Dispatch(() => Close(false, true)),
                        () => Dispatch(() => Close(true, true)),
                        () => Dispatch(() =>
                        {
                            if (rewarded || reservation == null) return;
                            rewarded = true;
                            // Native AdMob guarantees reward-before-close; retain binding for late/duplicate callbacks too.
                            var evidence = new AdRewardEvidence(reservation, providerOperation);
                            var handlers = RewardEarned;
                            if (handlers == null) return;
                            foreach (Action<AdRewardEvidence> handler in handlers.GetInvocationList())
                                try { handler(evidence); } catch { /* Reservation remains recoverable; never leak callback exceptions into SDK. */ }
                        }));
                }
                catch { Close(true, false); }
            });
            return await completion.Task.ConfigureAwait(false);
        }
        /// <summary>Destroys cached ads; never grants or revokes any business reward.</summary>
        public void DiscardLoadedAds() => Dispatch(RevokeCore);
        /// <summary>Releases one abandoned load without changing a persisted consent decision or another load.</summary>
        public void ReleaseLoadedAd(LoadedAd ad) => Dispatch(() =>
        {
            if (ad != null && loaded.TryGetValue(ad.Handle, out var entry) && entry.Kind == ad.Kind) { loaded.Remove(ad.Handle); Destroy(entry.Ad); }
        });
        private void DiscardCore() { foreach (var entry in loaded.Values) Destroy(entry.Ad); loaded.Clear(); }
        private static void Destroy(IGoogleLoadedAd? ad) { try { ad?.Destroy(); } catch { } }
        private void Dispatch(Action action) { if (Thread.CurrentThread.ManagedThreadId == ownerThread) action(); else post(action); }
        /// <summary>Releases loads and gates off. A provider-owned modal is not falsely reported cancelled.</summary>
        public void Dispose() => Dispatch(() => { RevokeCore(); disposed = true; });
    }
}
