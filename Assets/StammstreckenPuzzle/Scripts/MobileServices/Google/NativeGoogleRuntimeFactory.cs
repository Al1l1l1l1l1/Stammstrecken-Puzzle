using System;
using STP.Application;

namespace STP.MobileServices.Google
{
    /// <summary>Stable composition entry while the pinned SDK implementation remains availability guarded.</summary>
    public static class NativeGoogleRuntimeFactory
    {
        /// <summary>Construction never initializes UMP, advertisements or another service.</summary>
        public static IGoogleMobileRuntime Create(GoogleAdConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
#if STP_GOOGLE_MOBILE_ADS
            return new GoogleMobileAdsRuntime(configuration);
#else
            return new Unavailable();
#endif
        }
#if !STP_GOOGLE_MOBILE_ADS
        private sealed class Unavailable : IGoogleMobileRuntime
        {
            public void RefreshConsent(Action<bool, bool, bool> completed) => completed(false, false, false);
            public void ShowPrivacyOptions(Action<bool, bool, bool> completed) => completed(false, false, false);
            public void InitializeAds(Action<bool> completed) => completed(false);
            public void Load(MobileAdKind kind, Action<IGoogleLoadedAd?> completed) => completed(null);
        }
#endif
    }
}
