using System;
using System.Threading.Tasks;
using STP.Application;
using STP.Application.Ports;

namespace STP.MobileServices.Google
{
    /// <summary>
    /// WP-021-Skelett des Google-Adapters gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 3
    /// (Mobile Ads, UMP und Firebase Analytics; Crashdiagnose bleibt lokal). Implementiert die
    /// vier Google-Ports aus Abschnitt 6 ohne SDK-Referenz und ohne Fachlogik. Der Adapter wird
    /// von der Composition Root erstellt, aber gemäß Abschnitt 7 nicht initialisiert: Ads,
    /// Analytics und Crashdiagnose bleiben hinter ihren Capability-, Privacy- und Recovery-Gates.
    /// </summary>
    public sealed class GoogleModuleSkeleton : IAdsPort, IConsentPort, IAnalyticsPort, ICrashReportingPort
    {
        /// <summary>Legacy QA skeleton never grants provider capability.</summary>
        public MobileCapabilities Capabilities => new MobileCapabilities(false, false);
        /// <summary>No SDK exists in this historical composition.</summary>
        public event Action<AdRewardEvidence>? RewardEarned { add { } remove { } }
        /// <summary>Fail-closed legacy method.</summary>
        public Task<MobileResult<ConsentSnapshot>> RefreshAsync(MobileOperation operation) => Task.FromResult(MobileResult<ConsentSnapshot>.Stop(operation, MobileServiceStatus.Unavailable, "MOBILE_LEGACY_SKELETON"));
        /// <summary>Fail-closed legacy method.</summary>
        public Task<MobileResult<ConsentSnapshot>> ShowPrivacyOptionsAsync(MobileOperation operation) => RefreshAsync(operation);
        /// <summary>The legacy skeleton never applies consent.</summary>
        public bool ConfirmDecisionPersisted(ConsentSnapshot snapshot, long committedGeneration) => false;
        /// <summary>No SDK exists to revoke.</summary>
        public void Revoke() { }
        /// <summary>Fail-closed legacy method.</summary>
        public Task<MobileResult<LoadedAd>> LoadAsync(MobileAdKind kind, MobileOperation operation) => Task.FromResult(MobileResult<LoadedAd>.Stop(operation, MobileServiceStatus.Unavailable, "MOBILE_LEGACY_SKELETON"));
        /// <summary>Fail-closed legacy method.</summary>
        public Task<MobileResult<AdPresentation>> ShowAsync(LoadedAd ad, AdRewardReservation? reservation, MobileOperation operation) => Task.FromResult(MobileResult<AdPresentation>.Stop(operation, MobileServiceStatus.Unavailable, "MOBILE_LEGACY_SKELETON"));
        /// <summary>No cached ads exist.</summary>
        public void DiscardLoadedAds() { }
        /// <summary>Gibt an, ob der Adapter gestartet wurde; nach der Composition immer <c>false</c>.</summary>
        public bool IsInitialized { get; private set; }

        /// <summary>Markiert den Adapter als gestartet (ohne jede SDK- oder Netzwirkung im Skelett).</summary>
        public void Initialize()
        {
            this.IsInitialized = true;
        }
    }
}
