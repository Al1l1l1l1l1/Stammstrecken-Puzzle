using System;
using STP.Application;

namespace STP.MobileServices.Store
{
    /// <summary>Explicit immutable production metadata over the existing store contract; no defaults, SDK call or approval is created.</summary>
    public sealed class ProductionStoreConfiguration
    {
        public ProductionStoreConfiguration(StoreConfiguration actualConfiguration)
        {
            Configuration = actualConfiguration ?? throw new ArgumentNullException(nameof(actualConfiguration));
            if (!actualConfiguration.Production) throw new ArgumentException("IAP_PRODUCTION_CONFIGURATION_REQUIRED");
            if (actualConfiguration.Store == "GooglePlay")
            {
                if (string.IsNullOrWhiteSpace(actualConfiguration.GooglePublicKey)) throw new ArgumentException("IAP_GOOGLE_PUBLIC_KEY_REQUIRED");
                if (actualConfiguration.GooglePublicKey.Length > 8192) throw new ArgumentException("IAP_GOOGLE_PUBLIC_KEY_INVALID");
                byte[] key;
                try { key = Convert.FromBase64String(actualConfiguration.GooglePublicKey); }
                catch (FormatException) { throw new ArgumentException("IAP_GOOGLE_PUBLIC_KEY_INVALID"); }
                if (key.Length < 128 || key.Length > 4096) throw new ArgumentException("IAP_GOOGLE_PUBLIC_KEY_INVALID");
                // This is a bounded format check. Actual signature/key verification still belongs to the pinned native verification path.
            }
            else if (!string.IsNullOrEmpty(actualConfiguration.GooglePublicKey)) throw new ArgumentException("IAP_APPLE_GOOGLE_KEY_MISMATCH");
        }
        public StoreConfiguration Configuration { get; }
        /// <summary>Matches the existing reviewed policy/SDK record; this CLR type is never a legal attestation.</summary>
        public void RequireReadiness(IapPrivacyReadiness reviewed)
        {
            if (reviewed == null) throw new ArgumentException("IAP_PRIVACY_READINESS_REQUIRED");
            if (reviewed.PolicyRevision != Configuration.PolicyRevision || reviewed.SdkRevision != Configuration.SdkRevision)
                throw new ArgumentException("IAP_REVIEWED_INPUTS_MISMATCH");
        }
    }
}
