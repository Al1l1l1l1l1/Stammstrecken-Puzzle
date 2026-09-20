using System;
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
    /// WP-008-Ergebnis der Composition Root: belegt den vollstaendig gebundenen
    /// Application-/UI-/World-Graphen sowie saemtliche Adapterinstanzen, damit
    /// der BootstrapCompositionSmoke jede Portbindung referenzgleich gegen genau
    /// eine Adapterinstanz pruefen kann.
    /// </summary>
    public sealed class BootstrapCompositionResult
    {
        /// <summary>
        /// Erstellt das Composition-Ergebnis. Alle Argumente sind zwingend;
        /// null schlaegt fail-closed fehl.
        /// </summary>
        public BootstrapCompositionResult(
            ApplicationRoot application,
            UiPresentationModule ui,
            WorldPresentationModule world,
            ContentCatalogAdapter content,
            SaveRepositoryAdapter persistence,
            PlatformServicesAdapter platform,
            AudioAdapter audio,
            GoogleMobileServicesAdapter google,
            StorePurchaseAdapter store)
        {
            Application = application ?? throw new ArgumentNullException(nameof(application));
            Ui = ui ?? throw new ArgumentNullException(nameof(ui));
            World = world ?? throw new ArgumentNullException(nameof(world));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Persistence = persistence ?? throw new ArgumentNullException(nameof(persistence));
            Platform = platform ?? throw new ArgumentNullException(nameof(platform));
            AudioAdapter = audio ?? throw new ArgumentNullException(nameof(audio));
            Google = google ?? throw new ArgumentNullException(nameof(google));
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }

        /// <summary>Die Application-Wurzel des Graphen.</summary>
        public ApplicationRoot Application { get; }

        /// <summary>Das UI-Praesentationsmodul.</summary>
        public UiPresentationModule Ui { get; }

        /// <summary>Das World-Praesentationsmodul.</summary>
        public WorldPresentationModule World { get; }

        /// <summary>Der Content-Adapter (vier Katalogports).</summary>
        public ContentCatalogAdapter Content { get; }

        /// <summary>Der Persistenz-Adapter (Save-Port).</summary>
        public SaveRepositoryAdapter Persistence { get; }

        /// <summary>Der Plattform-Adapter (Clock-, Lifecycle-, Network-, Haptics-Ports).</summary>
        public PlatformServicesAdapter Platform { get; }

        /// <summary>Der Audio-Adapter (Audio-Port).</summary>
        public AudioAdapter AudioAdapter { get; }

        /// <summary>Der Google-Adapter (Ads-, Consent-, Analytics-, Crash-Ports), erstellt aber nicht gestartet.</summary>
        public GoogleMobileServicesAdapter Google { get; }

        /// <summary>Der Store-Adapter (Purchase-Port), erstellt aber nicht initialisiert.</summary>
        public StorePurchaseAdapter Store { get; }
    }
}
