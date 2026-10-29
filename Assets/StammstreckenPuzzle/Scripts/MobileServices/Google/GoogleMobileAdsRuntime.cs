using System;
using System.Threading;
#if STP_GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.Ump.Api;
#endif
using STP.Application;
using UnityEngine;

namespace STP.MobileServices.Google
{
    /// <summary>Explicit ad configuration. Native AdMob app IDs are supplied by the build profile.</summary>
    public sealed class GoogleAdConfiguration
    {
        private const string TestPublisher = "ca-app-pub-3940256099942544/";
        /// <summary>Creates a profile-bound pair of ad units; production/test mixing fails immediately.</summary>
        public GoogleAdConfiguration(string applicationId, string interstitialId, string rewardedId, bool production)
        {
            if (string.IsNullOrWhiteSpace(applicationId) || string.IsNullOrWhiteSpace(interstitialId) || string.IsNullOrWhiteSpace(rewardedId)) throw new ArgumentException("ADS_CONFIGURATION_MISSING");
            if (!interstitialId.StartsWith("ca-app-pub-", StringComparison.Ordinal) || !rewardedId.StartsWith("ca-app-pub-", StringComparison.Ordinal)) throw new ArgumentException("ADS_CONFIGURATION_INVALID");
            if (interstitialId.StartsWith(TestPublisher, StringComparison.Ordinal) == production || rewardedId.StartsWith(TestPublisher, StringComparison.Ordinal) == production) throw new ArgumentException("ADS_PROFILE_ID_MISMATCH");
            MobileApplicationProfile.Validate(applicationId, production);
            ApplicationId = applicationId; InterstitialId = interstitialId; RewardedId = rewardedId; Production = production;
        }
        /// <summary>Immutable production/test identity; reading it starts no SDK.</summary>
        public bool Production { get; }
        /// <summary>Expected package/bundle identity.</summary>
        public string ApplicationId { get; }
        /// <summary>Explicit interstitial unit.</summary>
        public string InterstitialId { get; }
        /// <summary>Explicit rewarded unit.</summary>
        public string RewardedId { get; }
        /// <summary>Official test IDs from the pinned Google sample; no production traffic.</summary>
        public static GoogleAdConfiguration TestAds(string applicationId, bool ios) => new GoogleAdConfiguration(applicationId,
            ios ? "ca-app-pub-3940256099942544/4411468910" : "ca-app-pub-3940256099942544/1033173712",
            ios ? "ca-app-pub-3940256099942544/1712485313" : "ca-app-pub-3940256099942544/5224354917", false);
    }

#if STP_GOOGLE_MOBILE_ADS
    /// <summary>Actual pinned Google UMP/GMA SDK calls. Construction has no SDK/network effect.</summary>
    public sealed class GoogleMobileAdsRuntime : IGoogleMobileRuntime, IGoogleNativeUiState
    {
        private readonly GoogleAdConfiguration configuration;
        private object? nativeConsentPresentation;
        /// <summary>Only a possible UMP form phase; querying this does not call any SDK API.</summary>
        public bool IsNativeUiBusy => Volatile.Read(ref nativeConsentPresentation) != null;
        /// <summary>Constructs without SDK initialization.</summary>
        public GoogleMobileAdsRuntime(GoogleAdConfiguration configuration) { this.configuration = configuration; }
        private bool Supported => (UnityEngine.Application.platform == RuntimePlatform.Android || UnityEngine.Application.platform == RuntimePlatform.IPhonePlayer) && UnityEngine.Application.identifier == configuration.ApplicationId;

        /// <summary>UMP current launch update, then required form. Errors never inherit old consent.</summary>
        public void RefreshConsent(Action<bool, bool, bool> completed)
        {
            if (!Supported) { completed(false, false, false); return; }
            ConsentInformation.Update(new ConsentRequestParameters(), error =>
            {
                if (error != null) { completed(false, false, IsPrivacyEntryRequired()); return; }
                var presentation = new object();
                Interlocked.Exchange(ref nativeConsentPresentation, presentation);
                ConsentForm.LoadAndShowConsentFormIfRequired(formError =>
                {
                    Interlocked.CompareExchange(ref nativeConsentPresentation, null, presentation);
                    completed(formError == null, formError == null && ConsentInformation.CanRequestAds(), IsPrivacyEntryRequired());
                });
            });
        }
        /// <summary>Runs the real UMP privacy UI, with no AdMob request.</summary>
        public void ShowPrivacyOptions(Action<bool, bool, bool> completed)
        {
            if (!Supported) { completed(false, false, false); return; }
            var presentation = new object();
            Interlocked.Exchange(ref nativeConsentPresentation, presentation);
            ConsentForm.ShowPrivacyOptionsForm(error =>
            {
                Interlocked.CompareExchange(ref nativeConsentPresentation, null, presentation);
                completed(error == null, error == null && ConsentInformation.CanRequestAds(), IsPrivacyEntryRequired());
            });
        }
        private static bool IsPrivacyEntryRequired() => ConsentInformation.PrivacyOptionsRequirementStatus == PrivacyOptionsRequirementStatus.Required;

        /// <summary>Only the coordinator's effective-consent branch invokes initialization.</summary>
        public void InitializeAds(Action<bool> completed)
        {
            if (!Supported) { completed(false); return; }
            MobileAds.SetRequestConfiguration(new RequestConfiguration
            {
                PublisherFirstPartyIdEnabled = false,
                PublisherPrivacyPersonalizationState = PublisherPrivacyPersonalizationState.Disabled
            });
            MobileAds.Initialize(status => MobileAdsEventExecutor.ExecuteInUpdate(() => completed(status != null)));
        }
        /// <summary>Requests a single ad with personalization explicitly disabled.</summary>
        public void Load(MobileAdKind kind, Action<IGoogleLoadedAd?> completed)
        {
            if (!Supported) { completed(null); return; }
            var request = new AdRequest();
            request.Extras.Add("npa", "1");
            if (kind == MobileAdKind.Interstitial)
                InterstitialAd.Load(configuration.InterstitialId, request, (ad, error) =>
                {
                    if (error != null || ad == null) { ad?.Destroy(); completed(null); }
                    else completed(new Interstitial(ad));
                });
            else
                RewardedAd.Load(configuration.RewardedId, request, (ad, error) =>
                {
                    if (error != null || ad == null) { ad?.Destroy(); completed(null); }
                    else completed(new Rewarded(ad));
                });
        }
        private sealed class Interstitial : IGoogleLoadedAd
        {
            private readonly InterstitialAd ad;
            internal Interstitial(InterstitialAd ad) { this.ad = ad; }
            public bool CanShow => ad.CanShowAd();
            public void Show(Action opened, Action closed, Action failed, Action rewarded)
            { ad.OnAdFullScreenContentOpened += opened; ad.OnAdFullScreenContentClosed += closed; ad.OnAdFullScreenContentFailed += _ => failed(); ad.Show(); }
            public void Destroy() => ad.Destroy();
        }
        private sealed class Rewarded : IGoogleLoadedAd
        {
            private readonly RewardedAd ad;
            internal Rewarded(RewardedAd ad) { this.ad = ad; }
            public bool CanShow => ad.CanShowAd();
            public void Show(Action opened, Action closed, Action failed, Action rewarded)
            { ad.OnAdFullScreenContentOpened += opened; ad.OnAdFullScreenContentClosed += closed; ad.OnAdFullScreenContentFailed += _ => failed(); ad.Show(_ => rewarded()); }
            public void Destroy() => ad.Destroy();
        }
    }
#endif
}
