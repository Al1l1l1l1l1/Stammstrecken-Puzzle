using STP.Application;
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
    /// Composition Root gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 7 und ADR-018.
    /// Erstellt den vollständigen Graphen per manueller Konstruktorinjektion in der
    /// dokumentierten Reihenfolge (Konfiguration/Logger, Content, Persistenz, Clock/Lifecycle,
    /// Audio, Application, Presentation und erst danach die erlaubten externen Adapter).
    /// Kein Service Locator, keine veränderliche Registry, kein Reflexions-Wiring, keine
    /// Szenensuche, kein Providerstart: Die Google-/Store-Adapter werden erstellt, aber
    /// nicht initialisiert. Die lokale Portbindung und ApplicationRoot entstehen vor
    /// der Presentation; beide Presentation-Skelette erhalten dieselbe Root.
    /// </summary>
    public static class BootstrapComposition
    {
        /// <summary>Erstellt den vollständigen Composition Graph ohne jede Providerinitialisierung.</summary>
        public static BootstrapCompositionResult Compose()
        {
            var configuration = new BootstrapConfiguration();
            var logger = new LocalBootstrapLogger(configuration);
            logger.LogCompositionStarted();
            var content = new ContentModuleSkeleton();
            var persistence = new PersistenceModuleSkeleton();
            var platform = new PlatformModuleSkeleton();
            var audio = new AudioModuleSkeleton();
            var composition = ApplicationComposition.FromPorts(
                saves: persistence,
                levels: content,
                campaigns: content,
                completions: content,
                cosmetics: content,
                clock: platform,
                audio: audio,
                lifecycle: platform,
                network: platform,
                haptics: platform);

            var root = new ApplicationRoot(composition);
            var ui = new UiPresentationSkeleton(root);
            var world = new WorldPresentationSkeleton(root);

            var google = new GoogleModuleSkeleton();
            var store = new StoreModuleSkeleton();

            // Erst danach die erlaubten externen Adapter binden (Abschnitt 7).
            composition.BindProviders(
                ads: google,
                purchases: store,
                consent: google,
                analytics: google,
                crashes: google);

            logger.LogCompositionCompleted();
            return new BootstrapCompositionResult(
                configuration, logger, content, persistence, platform, audio, ui, world, google, store, root);
        }
    }
}
