using STP.Application;
using STP.Application.Composition;
using STP.Audio;
using STP.Infrastructure.Content;
using STP.Infrastructure.Persistence;
using STP.MobileServices.Google;
using STP.MobileServices.Store;
using STP.Platform;
using STP.Presentation.UI;
using STP.Presentation.World;

namespace STP.Bootstrap
{
    /// <summary>
    /// WP-008-Composition-Root-Skelett gemäß ADR-018 und MODULE_BOUNDARIES.md
    /// Abschnitt 7. Manuelle Konstruktorinjektion ohne Service Locator, ohne
    /// veraenderliche Singleton-Registry, ohne Reflexions-Wiring und ohne
    /// Szenensuche. Die Erstellreihenfolge folgt dem Architekturvertrag:
    /// Content, Persistenz, Clock/Lifecycle, Audio, Application, Presentation
    /// und erst danach die erlaubten externen Adapter.
    /// </summary>
    public static class BootstrapComposition
    {
        /// <summary>
        /// Erstellt den vollstaendigen Composition Graph des WP-008-Skeletts.
        /// Jeder der fuenfzehn normativen Application-Ports wird genau einmal
        /// gebunden. Kein Adapter wird gestartet: die Bindung ist keine
        /// Initialisierungsfreigabe; Ads, IAP, Analytics und Crashdiagnose
        /// bleiben hinter ihren Capability-, Privacy- und Recovery-Gates.
        /// </summary>
        public static BootstrapCompositionResult Compose()
        {
            var content = new ContentCatalogAdapter();
            var persistence = new SaveRepositoryAdapter();
            var platform = new PlatformServicesAdapter();
            var audio = new AudioAdapter();

            var composition = new ApplicationComposition(
                saveRepository: persistence,
                levelCatalog: content,
                campaignCatalog: content,
                completionCatalog: content,
                cosmeticsCatalog: content,
                clock: platform,
                appLifecycle: platform,
                networkStatus: platform,
                haptics: platform,
                audio: audio);

            var application = new ApplicationRoot(composition);
            var ui = new UiPresentationModule(application);
            var world = new WorldPresentationModule(application);

            var google = new GoogleMobileServicesAdapter();
            var store = new StorePurchaseAdapter();

            composition.BindProviders(
                ads: google,
                purchase: store,
                consent: google,
                analytics: google,
                crashReporting: google);

            return new BootstrapCompositionResult(
                application: application,
                ui: ui,
                world: world,
                content: content,
                persistence: persistence,
                platform: platform,
                audio: audio,
                google: google,
                store: store);
        }
    }
}
