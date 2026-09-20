using System;
using STP.Application.Ports;

namespace STP.Application.Composition
{
    /// <summary>
    /// WP-008-Composition-Skelett: bindet alle fuenfzehn normativen Application-Ports
    /// (MODULE_BOUNDARIES.md Abschnitt 6) genau einmal. Kernports werden im Konstruktor
    /// gebunden; Providerports werden nachgelagert ueber <see cref="BindProviders"/>
    /// gebunden, damit die Composition Root die dokumentierte Erstellreihenfolge
    /// (externe Adapter zuletzt) einhalten kann. Kein Port ist null und kein Port
    /// besitzt einen stillen Fallback; der Zugriff auf einen noch nicht gebundenen
    /// Providerport schlaegt fail-closed fehl.
    /// </summary>
    public sealed class ApplicationComposition
    {
        private IAdsPort? _ads;
        private IPurchasePort? _purchase;
        private IConsentPort? _consent;
        private IAnalyticsPort? _analytics;
        private ICrashReportingPort? _crashReporting;

        /// <summary>
        /// Erstellt die Composition mit allen providerneutralen Kernports.
        /// </summary>
        public ApplicationComposition(
            ISaveRepository saveRepository,
            ILevelCatalog levelCatalog,
            ICampaignCatalog campaignCatalog,
            ICompletionCatalog completionCatalog,
            ICosmeticsCatalog cosmeticsCatalog,
            IClock clock,
            IAppLifecyclePort appLifecycle,
            INetworkStatusPort networkStatus,
            IHapticsPort haptics,
            IAudioPort audio)
        {
            SaveRepository = saveRepository ?? throw new ArgumentNullException(nameof(saveRepository));
            LevelCatalog = levelCatalog ?? throw new ArgumentNullException(nameof(levelCatalog));
            CampaignCatalog = campaignCatalog ?? throw new ArgumentNullException(nameof(campaignCatalog));
            CompletionCatalog = completionCatalog ?? throw new ArgumentNullException(nameof(completionCatalog));
            CosmeticsCatalog = cosmeticsCatalog ?? throw new ArgumentNullException(nameof(cosmeticsCatalog));
            Clock = clock ?? throw new ArgumentNullException(nameof(clock));
            AppLifecycle = appLifecycle ?? throw new ArgumentNullException(nameof(appLifecycle));
            NetworkStatus = networkStatus ?? throw new ArgumentNullException(nameof(networkStatus));
            Haptics = haptics ?? throw new ArgumentNullException(nameof(haptics));
            Audio = audio ?? throw new ArgumentNullException(nameof(audio));
        }

        /// <summary>Snapshot-Persistenz (STP.Infrastructure.Persistence).</summary>
        public ISaveRepository SaveRepository { get; }

        /// <summary>Levelkatalog (STP.Infrastructure.Content).</summary>
        public ILevelCatalog LevelCatalog { get; }

        /// <summary>Kampagnenkatalog (STP.Infrastructure.Content).</summary>
        public ICampaignCatalog CampaignCatalog { get; }

        /// <summary>Completionkatalog (STP.Infrastructure.Content).</summary>
        public ICompletionCatalog CompletionCatalog { get; }

        /// <summary>Kosmetikkatalog (STP.Infrastructure.Content).</summary>
        public ICosmeticsCatalog CosmeticsCatalog { get; }

        /// <summary>Uhr (STP.Platform).</summary>
        public IClock Clock { get; }

        /// <summary>App-Lifecycle (STP.Platform).</summary>
        public IAppLifecyclePort AppLifecycle { get; }

        /// <summary>Netzwerkstatus (STP.Platform).</summary>
        public INetworkStatusPort NetworkStatus { get; }

        /// <summary>Haptik (STP.Platform).</summary>
        public IHapticsPort Haptics { get; }

        /// <summary>Audio (STP.Audio).</summary>
        public IAudioPort Audio { get; }

        /// <summary>Werbung (STP.MobileServices.Google); fail-closed vor <see cref="BindProviders"/>.</summary>
        public IAdsPort Ads => _ads ?? throw Unbound(nameof(Ads));

        /// <summary>In-App-Kauf (STP.MobileServices.Store); fail-closed vor <see cref="BindProviders"/>.</summary>
        public IPurchasePort Purchase => _purchase ?? throw Unbound(nameof(Purchase));

        /// <summary>Consent (STP.MobileServices.Google); fail-closed vor <see cref="BindProviders"/>.</summary>
        public IConsentPort Consent => _consent ?? throw Unbound(nameof(Consent));

        /// <summary>Analytics (STP.MobileServices.Google); fail-closed vor <see cref="BindProviders"/>.</summary>
        public IAnalyticsPort Analytics => _analytics ?? throw Unbound(nameof(Analytics));

        /// <summary>Crashdiagnose (STP.MobileServices.Google); fail-closed vor <see cref="BindProviders"/>.</summary>
        public ICrashReportingPort CrashReporting => _crashReporting ?? throw Unbound(nameof(CrashReporting));

        /// <summary>Gibt an, ob die Providerports bereits gebunden sind.</summary>
        public bool ProvidersBound => _ads is not null;

        /// <summary>
        /// Bindet die fuenf Providerports genau einmal. Eine Doppelbindung oder ein
        /// null-Argument schlaegt fail-closed fehl. Diese Bindung ist keine
        /// Initialisierungsfreigabe: Ads, IAP, Analytics und Crashdiagnose bleiben
        /// hinter ihren Capability-, Privacy- und Recovery-Gates (ADR-018).
        /// </summary>
        public void BindProviders(
            IAdsPort ads,
            IPurchasePort purchase,
            IConsentPort consent,
            IAnalyticsPort analytics,
            ICrashReportingPort crashReporting)
        {
            if (_ads is not null)
            {
                throw new InvalidOperationException("Providerports sind bereits gebunden; eine Doppelbindung ist nicht zulaessig.");
            }

            _ads = ads ?? throw new ArgumentNullException(nameof(ads));
            _purchase = purchase ?? throw new ArgumentNullException(nameof(purchase));
            _consent = consent ?? throw new ArgumentNullException(nameof(consent));
            _analytics = analytics ?? throw new ArgumentNullException(nameof(analytics));
            _crashReporting = crashReporting ?? throw new ArgumentNullException(nameof(crashReporting));
        }

        private static InvalidOperationException Unbound(string portName)
        {
            return new InvalidOperationException(
                $"Der Port {portName} ist noch nicht gebunden. Die Composition Root muss BindProviders aufrufen, bevor Providerports gelesen werden.");
        }
    }
}
