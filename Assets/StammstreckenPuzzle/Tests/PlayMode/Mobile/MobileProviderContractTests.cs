using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using STP.Application;
using STP.MobileServices.Google;
using STP.MobileServices.Store;

namespace STP.Tests.Mobile.PlayMode
{
    // These are adapter contract tests with in-process fakes, never Sandbox/SDK/device evidence.
    public sealed class MobileProviderContractTests
    {
        private static MobileOperation Op(string id = "op", int milliseconds = 1000) => new MobileOperation(id, TimeSpan.FromMilliseconds(milliseconds));
        private static GoogleMobileServices Google(FakeGoogle sdk) => new GoogleMobileServices(sdk, action => action());
        private static void Enable(GoogleMobileServices service)
        {
            var snapshot = service.RefreshAsync(Op("consent:" + Guid.NewGuid().ToString("N"))).GetAwaiter().GetResult().Value!;
            Assert.That(service.ConfirmDecisionPersisted(snapshot, 1), Is.True);
        }
        private static StoreConfiguration Config() => new StoreConfiguration("example.stp.qa", "remove_ads_qa", "GooglePlay", false, "privacy-v1");
        private static IapPrivacyReadiness Readiness() => new IapPrivacyReadiness("privacy-v1", "unity-iap-5.4.3", new string('a', 64));
        private static UnityStorePurchaseAdapter Store(FakeStore sdk, Action? factoryCalled = null) => new UnityStorePurchaseAdapter(Config(), () => { factoryCalled?.Invoke(); return sdk; }, action => action());
        private static void Prepare(UnityStorePurchaseAdapter adapter) => Assert.That(adapter.PrepareAsync(PurchaseIntent.Purchase, Readiness(), Op("prepare")).GetAwaiter().GetResult().Status, Is.EqualTo(MobileServiceStatus.Succeeded));

        [Test]
        public void NativeUiFactExcludesBackgroundConsentAndAdLoadButRetainsAnUnclosedFormAfterTimeout()
        {
            var sdk = new FakeGoogle { DeferConsent = true }; using var service = Google(sdk);
            Assert.That(service.IsNativeUiBusy, Is.False); Assert.That(sdk.Initializations, Is.Zero);
            var refresh = service.RefreshAsync(Op("background-update", 30));
            Assert.That(service.IsNativeUiBusy, Is.False, "ConsentInfo.Update is a background query");
            sdk.NativeFormActive = true;
            Assert.That(service.IsNativeUiBusy, Is.True);
            Assert.That(refresh.Wait(1000), Is.True); Assert.That(refresh.Result.Status, Is.EqualTo(MobileServiceStatus.TimedOut));
            Assert.That(service.IsNativeUiBusy, Is.True, "Caller timeout does not confirm form dismissal");
            sdk.ConsentCallback!(true, true, true); Assert.That(service.IsNativeUiBusy, Is.False);
            sdk.DeferConsent = false; Enable(service); sdk.DeferLoad = true;
            var loading = service.LoadAsync(MobileAdKind.Interstitial, Op("background-load"));
            Assert.That(service.IsNativeUiBusy, Is.False); sdk.LoadCallback!(sdk.Ad);
            Assert.That(loading.Result.Status, Is.EqualTo(MobileServiceStatus.Succeeded));
        }

        [Test]
        public void UncertainAdShowThrowKeepsNativeUiUntilTheSameNativeCloseOrDispose()
        {
            var sdk = new FakeGoogle(); sdk.Ad.ThrowAfterShow = true; using var service = Google(sdk); Enable(service);
            var ad = service.LoadAsync(MobileAdKind.Interstitial, Op("load-ui")).Result.Value!;
            Assert.That(service.ShowAsync(ad, null, Op("show-ui")).Result.Status, Is.EqualTo(MobileServiceStatus.Unavailable));
            Assert.That(service.IsNativeUiBusy, Is.True);
            Assert.That(service.ShowPrivacyOptionsAsync(Op("overlap-ui")).Result.Status, Is.EqualTo(MobileServiceStatus.Pending));
            sdk.Ad.Close!(); Assert.That(service.IsNativeUiBusy, Is.False);
            sdk.NativeFormActive = true; Assert.That(service.IsNativeUiBusy, Is.True);
            service.Dispose(); Assert.That(service.IsNativeUiBusy, Is.False);
        }

        [Test]
        public void DuplicateOldAdCloseCannotClearTheNextNativePresentation()
        {
            var sdk = new FakeGoogle(); using var service = Google(sdk); Enable(service);
            var first = service.LoadAsync(MobileAdKind.Interstitial, Op("load-first-ui")).Result.Value!;
            var firstShow = service.ShowAsync(first, null, Op("show-first-ui"));
            var firstClose = sdk.Ad.Close!; sdk.Ad.Open!(); firstClose();
            Assert.That(firstShow.Result.Status, Is.EqualTo(MobileServiceStatus.Succeeded));
            sdk.DeferLoad = true; var secondNative = new FakeAd();
            var secondLoad = service.LoadAsync(MobileAdKind.Interstitial, Op("load-second-ui")); sdk.LoadCallback!(secondNative);
            var secondShow = service.ShowAsync(secondLoad.Result.Value!, null, Op("show-second-ui"));
            var secondClose = secondNative.Close!;
            Assert.That(service.IsNativeUiBusy, Is.True);
            firstClose(); Assert.That(service.IsNativeUiBusy, Is.True, "A previous presentation callback cannot release the active native UI");
            secondNative.Open!(); secondClose();
            Assert.That(service.IsNativeUiBusy, Is.False); Assert.That(secondShow.Result.Status, Is.EqualTo(MobileServiceStatus.Succeeded));
        }

        [Test]
        public void NativeStoreUiIsIndependentOfBackgroundPreparationAndDeferredBusinessState()
        {
            var sdk = new FakeStore(); int factories = 0; using var adapter = Store(sdk, () => factories++);
            Assert.That(adapter.IsNativeUiBusy, Is.False); Assert.That(factories, Is.Zero);
            Prepare(adapter); Assert.That(adapter.IsNativeUiBusy, Is.False);
            var purchase = adapter.PurchaseAsync(Op("purchase-ui", 30)); Assert.That(adapter.IsNativeUiBusy, Is.True);
            Assert.That(purchase.Wait(1000), Is.True); Assert.That(adapter.IsNativeUiBusy, Is.True);
            sdk.Stop(MobileServiceStatus.Pending); Assert.That(adapter.IsNativeUiBusy, Is.False, "Deferred payment persists after the native sheet closes");
            Assert.That(adapter.PurchaseAsync(Op("still-deferred")).Result.Status, Is.EqualTo(MobileServiceStatus.Pending));
        }

        [Test]
        public void PossibleRestoreUiRetainsFenceUntilItsNativeCallbackDespiteCallerTimeout()
        {
            var sdk = new FakeStore { DeferRestore = true }; using var adapter = Store(sdk); Prepare(adapter);
            var restore = adapter.RestoreAsync(Op("restore-ui", 30)); Assert.That(adapter.IsNativeUiBusy, Is.True);
            Assert.That(restore.Wait(1000), Is.True); Assert.That(adapter.IsNativeUiBusy, Is.True);
            sdk.FinishNativeRestore(); Assert.That(adapter.IsNativeUiBusy, Is.False);
            Assert.That(restore.Result.Status, Is.EqualTo(MobileServiceStatus.TimedOut));
        }

        [TestCase(false)][TestCase(true)]
        public void UnknownStoreUiThrowRemainsFencedUntilNativeOutcomeOrDisposal(bool restoring)
        {
            var sdk = new FakeStore { ThrowPurchase = !restoring, DeferRestore = restoring, ThrowRestore = restoring };
            using var adapter = Store(sdk); Prepare(adapter);
            if (restoring) Assert.That(adapter.RestoreAsync(Op("restore-throw")).Result.Status, Is.EqualTo(MobileServiceStatus.Failed));
            else Assert.That(adapter.PurchaseAsync(Op("purchase-throw")).Result.Status, Is.EqualTo(MobileServiceStatus.Failed));
            Assert.That(adapter.IsNativeUiBusy, Is.True);
            Assert.That(adapter.PurchaseAsync(Op("overlap")).Result.Status, Is.EqualTo(MobileServiceStatus.Pending));
            if (restoring) sdk.FinishNativeRestore(); else sdk.Stop(MobileServiceStatus.Cancelled);
            Assert.That(adapter.IsNativeUiBusy, Is.False);
            sdk.ThrowPurchase = false; sdk.Purchase(); Assert.That(adapter.IsNativeUiBusy, Is.True);
            adapter.Dispose(); Assert.That(adapter.IsNativeUiBusy, Is.False);
        }

        [Test]
        public void ExplicitPriceFetchDoesNotPurchaseAndOnlyItsExactDisplayedProductCanBeConsumed()
        {
            var sdk = new FakeStore(); using var adapter = Store(sdk);
            var fetched = adapter.PrepareAsync(PurchaseIntent.Purchase, Readiness(), Op("price-only")).Result.Value!;
            Assert.That(sdk.Purchases, Is.Zero); Assert.That(adapter.IsNativeUiBusy, Is.False);
            Assert.That(adapter.IsPreparedProduct(fetched), Is.True);
            var copied = new StoreProduct(fetched.StoreProductId, fetched.LocalizedPrice, fetched.Title);
            Assert.That(adapter.IsPreparedProduct(copied), Is.False);
            Assert.That(adapter.PurchasePreparedAsync(copied, Op("copied-offer")).Result.Code, Is.EqualTo("IAP_DISPLAYED_OFFER_REQUIRED"));
            Assert.That(sdk.Purchases, Is.Zero);
            var purchase = adapter.PurchasePreparedAsync(fetched, Op("shown-offer"));
            Assert.That(sdk.Purchases, Is.EqualTo(1)); Assert.That(adapter.IsPreparedProduct(fetched), Is.False);
            sdk.Stop(MobileServiceStatus.Cancelled); Assert.That(purchase.Result.Status, Is.EqualTo(MobileServiceStatus.Cancelled));
            Assert.That(adapter.PurchasePreparedAsync(fetched, Op("consumed-offer")).Result.Status, Is.EqualTo(MobileServiceStatus.NotAllowed));
            Assert.That(sdk.Purchases, Is.EqualTo(1));
        }

        [Test]
        public void RefetchInvalidatesTheEarlierDisplayedProductWithoutPurchasing()
        {
            var sdk = new FakeStore(); using var adapter = Store(sdk);
            var first = adapter.PrepareAsync(PurchaseIntent.Purchase, Readiness(), Op("first-price")).Result.Value!;
            var second = adapter.PrepareAsync(PurchaseIntent.Purchase, Readiness(), Op("refreshed-price")).Result.Value!;
            Assert.That(second, Is.Not.SameAs(first)); Assert.That(adapter.IsPreparedProduct(first), Is.False);
            Assert.That(adapter.PurchasePreparedAsync(first, Op("stale-offer")).Result.Code, Is.EqualTo("IAP_DISPLAYED_OFFER_REQUIRED"));
            Assert.That(sdk.Purchases, Is.Zero); Assert.That(adapter.IsPreparedProduct(second), Is.True);
            using var foreign = Store(new FakeStore()); Prepare(foreign);
            Assert.That(foreign.PurchasePreparedAsync(second, Op("foreign-offer")).Result.Code, Is.EqualTo("IAP_DISPLAYED_OFFER_REQUIRED"));
        }

        [Test]
        public void AdsRequireCurrentUmpAndExactPersistedCandidate()
        {
            var sdk = new FakeGoogle(); using var service = Google(sdk);
            Assert.That(service.LoadAsync(MobileAdKind.Interstitial, Op()).Result.Status, Is.EqualTo(MobileServiceStatus.NotAllowed));
            var candidate = service.RefreshAsync(Op("refresh")).Result.Value!;
            Assert.That(service.Capabilities.CanRequestAds, Is.False);
            Assert.That(service.ConfirmDecisionPersisted(new ConsentSnapshot(candidate.Revision, true, true), 1), Is.False);
            Assert.That(service.ConfirmDecisionPersisted(candidate, 0), Is.False);
            Assert.That(service.ConfirmDecisionPersisted(candidate, 2), Is.True);
            Assert.That(service.Capabilities.CanSendAnalytics || service.Capabilities.CanSendCrashReports, Is.False);
            Assert.That(service.LoadAsync(MobileAdKind.Interstitial, Op("load")).Result.Status, Is.EqualTo(MobileServiceStatus.Succeeded));
            Assert.That(sdk.Initializations, Is.EqualTo(1));
        }

        [Test]
        public void RevocationInvalidatesStaleConsentAndNeverStartsAds()
        {
            var sdk = new FakeGoogle { DeferConsent = true }; using var service = Google(sdk);
            var pending = service.RefreshAsync(Op("refresh", 30));
            service.Revoke(); sdk.ConsentCallback!(true, true, true);
            Assert.That(pending.Wait(1000), Is.True);
            Assert.That(pending.Result.Status, Is.EqualTo(MobileServiceStatus.TimedOut));
            Assert.That(service.Capabilities.CanRequestAds, Is.False);
            Assert.That(sdk.Initializations, Is.Zero);
        }

        [Test]
        public void ConsentErrorDoesNotReusePreviousPermission()
        {
            var sdk = new FakeGoogle(); using var service = Google(sdk); Enable(service);
            sdk.ConsentSuccess = false;
            Assert.That(service.RefreshAsync(Op("failure")).Result.Status, Is.EqualTo(MobileServiceStatus.Unavailable));
            Assert.That(service.Capabilities.CanRequestAds, Is.False);
        }

        [Test]
        public void FailedCurrentStartupCannotBeBypassedWithCachedPrivacyOptions()
        {
            var sdk = new FakeGoogle(); using var service = Google(sdk); Enable(service);
            sdk.ConsentSuccess = false;
            Assert.That(service.RefreshAsync(Op("failed-startup")).Result.Status, Is.EqualTo(MobileServiceStatus.Unavailable));
            sdk.ConsentSuccess = true;
            var options = service.ShowPrivacyOptionsAsync(Op("cached-options")).Result;
            Assert.That(options.Code, Is.EqualTo("CONSENT_CURRENT_UPDATE_REQUIRED"));
            Assert.That(sdk.OptionsCalls, Is.Zero); Assert.That(service.Capabilities.CanRequestAds, Is.False);
            Assert.That(sdk.Initializations, Is.Zero);
            Enable(service);
            var candidate = service.ShowPrivacyOptionsAsync(Op("fresh-options")).Result.Value!;
            Assert.That(sdk.OptionsCalls, Is.EqualTo(1)); Assert.That(service.ConfirmDecisionPersisted(candidate, 3), Is.True);
        }

        [TestCase("example.stp")][TestCase("example.stp.qa")][TestCase("example.stp.dev")][TestCase("example.stp.staging")]
        public void ProviderProfilesRejectCrossedProductionAndTestIdentities(string app)
        {
            bool isolated = app != "example.stp";
            Assert.Throws<ArgumentException>(() => new StoreConfiguration(app, "remove_ads", "GooglePlay", isolated, "privacy-v1"));
            Assert.Throws<ArgumentException>(() => new MobilePurchaseBinding("GooglePlay", app, "remove_ads", isolated));
            if (isolated)
                Assert.Throws<ArgumentException>(() => new GoogleAdConfiguration(app, "ca-app-pub-1111111111111111/2222222222", "ca-app-pub-1111111111111111/3333333333", true));
            else Assert.Throws<ArgumentException>(() => GoogleAdConfiguration.TestAds(app, false));
        }

        [TestCase("com.STP.StammstreckenPuzzle.human", false)]
        [TestCase("com.STP.StammstreckenPuzzle.human", true)]
        [TestCase("com.STP.StammstreckenPuzzle.HUMAN", false)]
        [TestCase("com.STP.StammstreckenPuzzle.HUMAN", true)]
        public void HumanObservationIdentityIsNeverAnAdvertisingOrStoreProfile(string applicationId, bool production)
        {
            var unit = production ? "ca-app-pub-1111111111111111/2222222222" : "ca-app-pub-3940256099942544/1033173712";
            Assert.That(Assert.Throws<ArgumentException>(() => new GoogleAdConfiguration(applicationId, unit, unit, production))!.Message, Is.EqualTo("MOBILE_HUMAN_PROVIDER_FORBIDDEN"));
            Assert.That(Assert.Throws<ArgumentException>(() => new StoreConfiguration(applicationId, "controlled-product", "GooglePlay", production, "controlled-policy"))!.Message, Is.EqualTo("MOBILE_HUMAN_PROVIDER_FORBIDDEN"));
            Assert.That(Assert.Throws<ArgumentException>(() => new MobilePurchaseBinding("GooglePlay", applicationId, "controlled-product", production))!.Message, Is.EqualTo("MOBILE_HUMAN_PROVIDER_FORBIDDEN"));
        }

        [Test]
        public void DuplicateLoadDoesNotDestroyTheAcceptedNativeAd()
        {
            var sdk = new FakeGoogle { DuplicateLoad = true }; using var service = Google(sdk); Enable(service);
            var ad = service.LoadAsync(MobileAdKind.Interstitial, Op("load")).Result.Value!;
            Assert.That(sdk.Ad.Destroyed, Is.False);
            var show = service.ShowAsync(ad, null, Op("show"));
            sdk.Ad.Open!(); sdk.Ad.Close!(); sdk.Ad.Close!();
            Assert.That(show.Result.Status, Is.EqualTo(MobileServiceStatus.Succeeded));
            Assert.That(service.ShowAsync(ad, null, Op("show-again")).Result.Status, Is.EqualTo(MobileServiceStatus.Unavailable));
        }

        [Test]
        public void TimedOutRewardKeepsReservationAndBlocksASecondModalUntilClose()
        {
            var sdk = new FakeGoogle(); using var service = Google(sdk); Enable(service);
            var ad = service.LoadAsync(MobileAdKind.Rewarded, Op("load")).Result.Value!;
            var reservation = new AdRewardReservation("daily:v1:2026-10-21:REWARDED", "reward-operation", 7);
            var evidence = new List<AdRewardEvidence>(); service.RewardEarned += evidence.Add;
            var show = service.ShowAsync(ad, reservation, Op("reward-operation", 30));
            Assert.That(show.Wait(1000), Is.True); Assert.That(show.Result.Status, Is.EqualTo(MobileServiceStatus.TimedOut));
            Assert.That(service.IsNativeUiBusy, Is.True);
            Assert.That(service.ShowAsync(ad, reservation, Op("reward-operation")).Result.Status, Is.EqualTo(MobileServiceStatus.Pending));
            sdk.Ad.Open!(); sdk.Ad.Reward!(); sdk.Ad.Reward!(); sdk.Ad.Close!();
            Assert.That(evidence.Count, Is.EqualTo(1)); Assert.That(evidence[0].Reservation, Is.SameAs(reservation));
            Assert.That(evidence[0].ProviderOperationId, Is.EqualTo(reservation.OperationId));
            Assert.That(show.Result.Status, Is.EqualTo(MobileServiceStatus.TimedOut));
        }

        [Test]
        public void RewardedShowRejectsMissingOrOtherOperationReservation()
        {
            var sdk = new FakeGoogle(); using var service = Google(sdk); Enable(service);
            var ad = service.LoadAsync(MobileAdKind.Rewarded, Op("load")).Result.Value!;
            Assert.That(service.ShowAsync(ad, null, Op("show")).Result.Status, Is.EqualTo(MobileServiceStatus.NotAllowed));
            Assert.That(service.ShowAsync(ad, new AdRewardReservation("claim", "other", 1), Op("show")).Result.Status, Is.EqualTo(MobileServiceStatus.NotAllowed));
            Assert.That(sdk.Ad.Shows, Is.Zero);
        }

        [Test]
        public void LateLoadAfterRevokeIsDestroyedAndNotCached()
        {
            var sdk = new FakeGoogle { DeferLoad = true }; using var service = Google(sdk); Enable(service);
            var load = service.LoadAsync(MobileAdKind.Interstitial, Op("load", 30));
            service.Revoke(); sdk.LoadCallback!(sdk.Ad);
            Assert.That(load.Wait(1000), Is.True); Assert.That(sdk.Ad.Destroyed, Is.True);
            Assert.That(load.Result.Status, Is.Not.EqualTo(MobileServiceStatus.Succeeded));
        }

        [Test]
        public void NoFillDoesNotRetryAndCancellationDoesNotInitialize()
        {
            var sdk = new FakeGoogle { NoFill = true }; using var service = Google(sdk); Enable(service);
            Assert.That(service.LoadAsync(MobileAdKind.Interstitial, Op("load")).Result.Status, Is.EqualTo(MobileServiceStatus.Unavailable));
            Assert.That(sdk.Loads, Is.EqualTo(1));
            using var cancelled = new CancellationTokenSource(); cancelled.Cancel();
            Assert.That(service.LoadAsync(MobileAdKind.Interstitial, new MobileOperation("cancelled", TimeSpan.FromSeconds(1), cancelled.Token)).Result.Status, Is.EqualTo(MobileServiceStatus.Cancelled));
            Assert.That(sdk.Loads, Is.EqualTo(1));
        }

        [Test]
        public void ConstructionAndInvalidReadinessNeverCreateIapRuntime()
        {
            var sdk = new FakeStore(); int factories = 0; using var adapter = Store(sdk, () => factories++);
            Assert.That(factories, Is.Zero);
            Assert.That(adapter.PrepareAsync(PurchaseIntent.Purchase, null, Op()).Result.Status, Is.EqualTo(MobileServiceStatus.NotAllowed));
            Assert.That(adapter.PrepareAsync(PurchaseIntent.Restore, new IapPrivacyReadiness("other", "unity-iap-5.4.3", new string('a', 64)), Op()).Result.Status, Is.EqualTo(MobileServiceStatus.NotAllowed));
            Assert.That(factories, Is.Zero); Assert.That(sdk.Connects, Is.Zero);
        }

        [Test]
        public void PurchaseNeverConfirmsBeforeAnExactDurableGrant()
        {
            var sdk = new FakeStore(); using var adapter = Store(sdk); Prepare(adapter);
            var evidenceEvents = new List<PurchaseEvidence>(); adapter.EvidenceReceived += evidenceEvents.Add;
            var purchase = adapter.PurchaseAsync(Op("purchase")); sdk.Emit("native-token");
            var evidence = purchase.Result.Value!;
            Assert.That(sdk.Confirms, Is.Zero); Assert.That(evidenceEvents.Count, Is.EqualTo(1));
            Assert.That(evidence.TransactionKey, Does.StartWith("iap-v1:")); Assert.That(evidence.TransactionKey, Does.Not.Contain("native-token"));
            var forged = new PurchaseEvidence(evidence.OperationId, evidence.TransactionKey, evidence.Handle, evidence.Store, evidence.Environment, evidence.ApplicationId, evidence.StoreProductId, false);
            Assert.That(adapter.FinalizeAsync(new DurablePurchaseGrant(forged, 4), Op("forged")).Result.Status, Is.EqualTo(MobileServiceStatus.NotAllowed));
            Assert.That(sdk.Confirms, Is.Zero);
            Assert.That(adapter.FinalizeAsync(new DurablePurchaseGrant(evidence, 4), Op("confirm")).Result.Status, Is.EqualTo(MobileServiceStatus.Succeeded));
            Assert.That(sdk.Confirms, Is.EqualTo(1));
            Assert.That(adapter.FinalizeAsync(new DurablePurchaseGrant(evidence, 4), Op("repeat")).Result.Status, Is.EqualTo(MobileServiceStatus.Succeeded));
            Assert.That(sdk.Confirms, Is.EqualTo(1));
        }

        [Test]
        public void UnverifiedEvidenceNeverGrantsOrConfirms()
        {
            var sdk = new FakeStore { Valid = false }; using var adapter = Store(sdk); Prepare(adapter);
            int evidenceEvents = 0; adapter.EvidenceReceived += _ => evidenceEvents++;
            var purchase = adapter.PurchaseAsync(Op("purchase")); sdk.Emit("native-token");
            Assert.That(purchase.Result.Status, Is.EqualTo(MobileServiceStatus.Failed));
            Assert.That(evidenceEvents, Is.Zero); Assert.That(sdk.Confirms, Is.Zero);
        }

        [Test]
        public void TimedOutPurchaseDeliversLateVerifiedRecoveryAndNeverDoubleConfirms()
        {
            var sdk = new FakeStore(); using var adapter = Store(sdk); Prepare(adapter);
            var events = new List<PurchaseEvidence>(); adapter.EvidenceReceived += events.Add;
            var purchase = adapter.PurchaseAsync(Op("purchase", 30));
            Assert.That(purchase.Wait(1000), Is.True);
            Assert.That(adapter.PurchaseAsync(Op("second")).Result.Status, Is.EqualTo(MobileServiceStatus.Pending));
            sdk.Emit("same-token"); sdk.Emit("same-token");
            Assert.That(events.Count, Is.EqualTo(2)); Assert.That(events[0].TransactionKey, Is.EqualTo(events[1].TransactionKey));
            Assert.That(events[0], Is.SameAs(events[1])); Assert.That(events[0].OperationId, Is.EqualTo("purchase"));
            Assert.That(purchase.Result.Status, Is.EqualTo(MobileServiceStatus.TimedOut)); Assert.That(sdk.Confirms, Is.Zero);
        }

        [Test]
        public void FailedConfirmationKeepsGrantRecoverableAndAllowsRetry()
        {
            var sdk = new FakeStore { ConfirmSuccess = false }; using var adapter = Store(sdk); Prepare(adapter);
            var purchase = adapter.PurchaseAsync(Op("purchase")); sdk.Emit("token"); var grant = new DurablePurchaseGrant(purchase.Result.Value!, 5);
            Assert.That(adapter.FinalizeAsync(grant, Op("confirm")).Result.Status, Is.EqualTo(MobileServiceStatus.Unavailable));
            sdk.ConfirmSuccess = true;
            Assert.That(adapter.FinalizeAsync(grant, Op("retry")).Result.Status, Is.EqualTo(MobileServiceStatus.Succeeded));
            Assert.That(sdk.Confirms, Is.EqualTo(2));
        }

        [Test]
        public void EmptyOrFailedRestoreNeverRevokesAnyEntitlement()
        {
            var sdk = new FakeStore(); using var adapter = Store(sdk); Prepare(adapter);
            int revoked = 0; adapter.EntitlementRevoked += _ => revoked++;
            Assert.That(adapter.RestoreAsync(Op("restore")).Result.Status, Is.EqualTo(MobileServiceStatus.Succeeded));
            sdk.RestoreSuccess = false;
            Assert.That(adapter.RestoreAsync(Op("offline")).Result.Status, Is.EqualTo(MobileServiceStatus.Unavailable));
            Assert.That(revoked, Is.Zero);
        }

        [Test]
        public void EachExplicitNewStoreIntentReconnectsAndFetchesWithoutAutomaticRetry()
        {
            var sdk = new FakeStore(); using var adapter = Store(sdk); Prepare(adapter);
            Assert.That(sdk.Connects, Is.EqualTo(1));
            Assert.That(adapter.PrepareAsync(PurchaseIntent.Restore, Readiness(), Op("prepare-explicit-restore")).Result.Status, Is.EqualTo(MobileServiceStatus.Succeeded));
            Assert.That(sdk.Connects, Is.EqualTo(2));
        }

        [Test]
        public void CallerCompletionCannotChangeAfterSuccessOrTimeout()
        {
            using var completion = new MobileOperationCompletion<StoreProduct>(Op("completion", 30));
            Assert.That(completion.Succeed(new StoreProduct("id", "price", "title")), Is.True);
            Assert.That(completion.Stop(MobileServiceStatus.Failed, "later"), Is.False);
            Assert.That(completion.Task.Result.Status, Is.EqualTo(MobileServiceStatus.Succeeded));
        }

        private sealed class FakeGoogle : IGoogleMobileRuntime, IGoogleNativeUiState
        {
            internal bool DeferConsent, DeferLoad, DuplicateLoad, NoFill;
            internal bool ConsentSuccess = true;
            internal bool NativeFormActive;
            public bool IsNativeUiBusy => NativeFormActive;
            internal int Initializations, Loads, OptionsCalls;
            internal Action<bool, bool, bool>? ConsentCallback;
            internal Action<IGoogleLoadedAd?>? LoadCallback;
            internal readonly FakeAd Ad = new FakeAd();
            public void RefreshConsent(Action<bool, bool, bool> completed) { ConsentCallback = (ok, ads, required) => { NativeFormActive = false; completed(ok, ads, required); }; if (!DeferConsent) ConsentCallback(ConsentSuccess, true, true); }
            public void ShowPrivacyOptions(Action<bool, bool, bool> completed) { OptionsCalls++; NativeFormActive = true; RefreshConsent(completed); }
            public void InitializeAds(Action<bool> completed) { Initializations++; completed(true); }
            public void Load(MobileAdKind kind, Action<IGoogleLoadedAd?> completed) { Loads++; LoadCallback = completed; if (DeferLoad) return; completed(NoFill ? null : Ad); if (DuplicateLoad) completed(Ad); }
        }
        private sealed class FakeAd : IGoogleLoadedAd
        {
            internal Action? Open, Close, Fail, Reward;
            internal int Shows;
            internal bool Destroyed;
            internal bool ThrowAfterShow;
            public bool CanShow => !Destroyed;
            public void Show(Action opened, Action closed, Action failed, Action rewarded) { Shows++; Open = opened; Close = closed; Fail = failed; Reward = rewarded; if (ThrowAfterShow) throw new InvalidOperationException("controlled-show-throw"); }
            public void Destroy() { Destroyed = true; }
        }
        private sealed class FakeStore : IUnityStoreRuntime, IUnityStoreNativeUiState
        {
            internal int Connects, Confirms, Purchases;
            internal bool Valid = true, ConfirmSuccess = true, RestoreSuccess = true;
            internal bool DeferRestore;
            internal bool ThrowPurchase, ThrowRestore;
            private bool purchaseUi, restoreUi;
            private Action<bool>? restoreCallback;
            public bool IsNativeUiBusy => purchaseUi || restoreUi;
            public bool Supported => true;
            public event Action<StoreTransaction>? TransactionReceived;
            public event Action<MobileServiceStatus>? PurchaseStopped;
            public void ConnectAndFetch(Action<StoreProduct?> completed) { Connects++; completed(new StoreProduct("remove_ads_qa", "3,99 €", "Werbefrei")); }
            public void Purchase() { Purchases++; purchaseUi = true; if (ThrowPurchase) throw new InvalidOperationException("controlled-purchase-throw"); }
            public void Restore(Action<bool> completed) { if (DeferRestore) { restoreUi = true; restoreCallback = completed; if (ThrowRestore) throw new InvalidOperationException("controlled-restore-throw"); } else completed(RestoreSuccess); }
            public void Verify(StoreTransaction transaction, Action<VerifiedStoreTransaction?> completed) => completed(Valid ? new VerifiedStoreTransaction(transaction.NativeId, "Sandbox", false) : null);
            public void Confirm(StoreTransaction transaction, Action<bool> completed) { Confirms++; completed(ConfirmSuccess); }
            public void Dispose() { }
            internal void Emit(string id) { purchaseUi = false; TransactionReceived?.Invoke(new StoreTransaction(id, "remove_ads_qa", false, null)); }
            internal void Stop(MobileServiceStatus status) { purchaseUi = false; PurchaseStopped?.Invoke(status); }
            internal void FinishNativeRestore() { restoreUi = false; var completed = restoreCallback; restoreCallback = null; completed?.Invoke(RestoreSuccess); }
        }
    }
}
