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
    /// Offengelegtes Ergebnis der Composition Root, damit der Composition-Smoke jede
    /// Portbindung referenzgleich gegen die erstellten Adapterinstanzen prüfen kann
    /// (ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 7).
    /// </summary>
    public sealed class BootstrapCompositionResult
    {
        /// <summary>Erstellt das Ergebnis mit allen Modulinstanzen und der Application-Root.</summary>
        public BootstrapCompositionResult(
            ContentModuleSkeleton content,
            PersistenceModuleSkeleton persistence,
            PlatformModuleSkeleton platform,
            AudioModuleSkeleton audio,
            UiPresentationSkeleton ui,
            WorldPresentationSkeleton world,
            GoogleModuleSkeleton google,
            StoreModuleSkeleton store,
            ApplicationRoot root)
        {
            this.Content = content;
            this.Persistence = persistence;
            this.Platform = platform;
            this.Audio = audio;
            this.Ui = ui;
            this.World = world;
            this.Google = google;
            this.Store = store;
            this.Root = root;
        }

        /// <summary>Content-Modul (vier Katalogports).</summary>
        public ContentModuleSkeleton Content { get; }

        /// <summary>Persistenz-Modul (Save-Port).</summary>
        public PersistenceModuleSkeleton Persistence { get; }

        /// <summary>Plattform-Modul (Clock, Lifecycle, Network, Haptics).</summary>
        public PlatformModuleSkeleton Platform { get; }

        /// <summary>Audio-Modul (Audio-Port).</summary>
        public AudioModuleSkeleton Audio { get; }

        /// <summary>UI-Präsentation.</summary>
        public UiPresentationSkeleton Ui { get; }

        /// <summary>Welt-Präsentation.</summary>
        public WorldPresentationSkeleton World { get; }

        /// <summary>Google-Adapter (erstellt, nicht initialisiert).</summary>
        public GoogleModuleSkeleton Google { get; }

        /// <summary>Store-Adapter (erstellt, nicht initialisiert).</summary>
        public StoreModuleSkeleton Store { get; }

        /// <summary>Application-Root mit der fail-closed geprüften Portbindung.</summary>
        public ApplicationRoot Root { get; }
    }
}
