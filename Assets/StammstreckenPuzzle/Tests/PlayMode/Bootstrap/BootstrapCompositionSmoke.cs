using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using STP.Application.Composition;
using STP.Bootstrap;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace STP.Tests.Bootstrap.PlayMode
{
    /// <summary>
    /// WP-008-Compile-/Composition-Smoke gemäß MODULE_BOUNDARIES.md Abschnitt 7.
    /// Belegt fuer das Produktionsscaffold ohne Fachlogik: vollstaendige Composition
    /// des Application-/UI-/World-Graphen, genau ein Binding je normativem
    /// Application-Port, referenzgleiche Bindung bei wiederholter Abfrage, keine
    /// undokumentierten Null-/Fallback-Ports (fail-closed), Fehlschlag einer
    /// Doppel- oder Nullbindung sowie — als Laufzeitbeobachtung des realen
    /// Composition-Pfads — dass waehrend des untersuchten Ausfuehrungszeitraums
    /// keine zusaetzlich erkannten verwalteten Provider-SDK-Assemblies geladen
    /// werden. Nutzt die dedizierte QA-Szene als PlayMode-Basis.
    /// </summary>
    public sealed class BootstrapCompositionSmoke
    {
        private const string QaScenePath = "Assets/StammstreckenPuzzle/Scenes/QA/BootstrapComposition.unity";

        /// <summary>
        /// Der Composition Graph wird vollstaendig aufgebaut: Application, UI und
        /// World sind vorhanden und beide Praesentationsmodule verdrahten
        /// referenzgleich dieselbe Application-Wurzel.
        /// </summary>
        [Test]
        public void Compose_BuildsCompleteApplicationUiWorldGraph()
        {
            var result = BootstrapComposition.Compose();

            AssertCompleteGraph(result);
        }

        /// <summary>
        /// Jeder der fuenfzehn normativen Application-Ports ist an genau eine
        /// Adapterinstanz des Composition-Ergebnisses gebunden. Referenzgleichheit
        /// gegen die bekannten Adapter schliesst undokumentierte Null- oder
        /// Fallback-Ports aus; Mehrport-Adapter liefern ueberall dieselbe Instanz.
        /// </summary>
        [Test]
        public void Compose_BindsExactlyOneAdapterPerApplicationPort()
        {
            var result = BootstrapComposition.Compose();

            AssertPortBindings(result);
        }

        /// <summary>
        /// Wiederholte Abfrage desselben Ports liefert referenzgleich dieselbe
        /// Instanz; es existiert keine austauschende oder neu erzeugende
        /// Zugriffslogik.
        /// </summary>
        [Test]
        public void Compose_PortQueriesReturnReferenceEqualInstances()
        {
            var composition = BootstrapComposition.Compose().Application.Composition;

            Assert.AreSame(composition.SaveRepository, composition.SaveRepository);
            Assert.AreSame(composition.LevelCatalog, composition.LevelCatalog);
            Assert.AreSame(composition.CampaignCatalog, composition.CampaignCatalog);
            Assert.AreSame(composition.CompletionCatalog, composition.CompletionCatalog);
            Assert.AreSame(composition.CosmeticsCatalog, composition.CosmeticsCatalog);
            Assert.AreSame(composition.Clock, composition.Clock);
            Assert.AreSame(composition.AppLifecycle, composition.AppLifecycle);
            Assert.AreSame(composition.NetworkStatus, composition.NetworkStatus);
            Assert.AreSame(composition.Haptics, composition.Haptics);
            Assert.AreSame(composition.Audio, composition.Audio);
            Assert.AreSame(composition.Ads, composition.Ads);
            Assert.AreSame(composition.Purchase, composition.Purchase);
            Assert.AreSame(composition.Consent, composition.Consent);
            Assert.AreSame(composition.Analytics, composition.Analytics);
            Assert.AreSame(composition.CrashReporting, composition.CrashReporting);
        }

        /// <summary>
        /// Der Zugriff auf einen noch nicht gebundenen Providerport schlaegt
        /// fail-closed fehl, statt einen undokumentierten Null- oder Fallbackport
        /// zu liefern.
        /// </summary>
        [Test]
        public void Composition_WithoutBoundProviders_FailsClosedOnProviderPorts()
        {
            var seed = BootstrapComposition.Compose();
            var composition = NewCoreComposition(seed);

            Assert.IsFalse(composition.ProvidersBound);
            Assert.Throws<InvalidOperationException>(() => _ = composition.Ads);
            Assert.Throws<InvalidOperationException>(() => _ = composition.Purchase);
            Assert.Throws<InvalidOperationException>(() => _ = composition.Consent);
            Assert.Throws<InvalidOperationException>(() => _ = composition.Analytics);
            Assert.Throws<InvalidOperationException>(() => _ = composition.CrashReporting);
        }

        /// <summary>
        /// Eine Doppelbindung der Providerports und jede Nullbindung schlagen
        /// fail-closed fehl; eine einmalige Bindung ist zulaessig und wird
        /// referenzgleich sichtbar.
        /// </summary>
        [Test]
        public void BindProviders_DoubleOrNullBinding_Fails()
        {
            var seed = BootstrapComposition.Compose();
            var composition = NewCoreComposition(seed);

            composition.BindProviders(seed.Google, seed.Store, seed.Google, seed.Google, seed.Google);

            Assert.IsTrue(composition.ProvidersBound);
            Assert.AreSame(seed.Google, composition.Ads);
            Assert.AreSame(seed.Store, composition.Purchase);
            Assert.Throws<InvalidOperationException>(() =>
                composition.BindProviders(seed.Google, seed.Store, seed.Google, seed.Google, seed.Google));

            var fresh = NewCoreComposition(seed);
            Assert.Throws<ArgumentNullException>(() =>
                fresh.BindProviders(null!, seed.Store, seed.Google, seed.Google, seed.Google));
            Assert.Throws<ArgumentNullException>(() => new ApplicationComposition(
                null!, seed.Content, seed.Content, seed.Content, seed.Content,
                seed.Platform, seed.Platform, seed.Platform, seed.Platform, seed.AudioAdapter));
        }

        /// <summary>
        /// Statische Pruefung: Keine Adapter-Assembly referenziert ein Provider-SDK
        /// (Ads, IAP, Analytics, Crashdiagnose). Dies belegt ausschliesslich das
        /// Fehlen von Compile-Zeit-Referenzen und ist ausdruecklich KEIN Nachweis
        /// ueber das Laufzeitverhalten von Compose(); die Laufzeitbeobachtung
        /// erfolgt in <see cref="Compose_LoadsNoAdditionalProviderSdkAssembliesDuringComposition"/>.
        /// </summary>
        [Test]
        public void Compose_CreatedAdaptersReferenceNoProviderSdks()
        {
            var result = BootstrapComposition.Compose();
            string[] bannedFragments =
            {
                "GoogleMobileAds", "Firebase", "UnityEngine.Purchasing", "Unity.Services", "Unity.Advertisement",
            };
            object[] adapters =
            {
                result.Content, result.Persistence, result.Platform, result.AudioAdapter, result.Google, result.Store,
            };

            foreach (var adapter in adapters)
            {
                foreach (var reference in adapter.GetType().Assembly.GetReferencedAssemblies())
                {
                    foreach (var fragment in bannedFragments)
                    {
                        Assert.IsFalse(
                            reference.Name.Contains(fragment, StringComparison.OrdinalIgnoreCase),
                            $"{adapter.GetType().Name} referenziert unerwartet {reference.Name}.");
                    }
                }
            }
        }

        /// <summary>
        /// Laufzeitbeobachtung des realen Composition-Pfads: Vergleicht die erkannten
        /// geladenen verwalteten Provider-SDK-Assemblies unmittelbar vor und nach
        /// Compose() und schlaegt nur fehl, wenn waehrend dieses Zeitraums zusaetzlich
        /// erkannte Provider-SDK-Assemblies hinzugekommen sind. Bereits vorher
        /// geladene Assemblies schlagen nicht allein wegen ihrer Anwesenheit fehl.
        /// Dieser Vergleich beweist ausdruecklich NICHT: die allgemeine Abwesenheit
        /// von Initialize- oder Startaufrufen, eine Aussage ueber native Starts ohne
        /// verwaltete Assembly, die Kausalitaet eines etwaigen Ladevorgangs durch
        /// Compose() selbst oder eine Eigenschaft zukuenftiger echter
        /// SDK-Integrationen. Er gilt ausschliesslich fuer diesen Scaffold und den
        /// untersuchten Ausfuehrungszeitraum.
        /// Separate Scaffold-Feststellung aus dem vorhandenen Code (nicht aus diesem
        /// Vergleich): Packages/manifest.json enthaelt keinerlei
        /// Provider-SDK-Pakete; BootstrapComposition.Compose() ruft ausschliesslich
        /// parameterlose Konstruktoren zustandsloser Skelett-Adapter sowie
        /// ApplicationComposition.BindProviders auf; Start- oder Initialize-Methoden
        /// existieren im Scaffold nicht. Ein konkreter Providerstartpfad ist im
        /// vorhandenen Code damit nicht vorhanden.
        /// </summary>
        [Test]
        public void Compose_LoadsNoAdditionalProviderSdkAssembliesDuringComposition()
        {
            var before = LoadedProviderSdkAssemblies();

            var result = BootstrapComposition.Compose();

            AssertCompleteGraph(result);
            AssertPortBindings(result);
            Assert.IsEmpty(
                NewSince(before),
                "Waehrend des untersuchten Composition-Pfads wurden zusaetzlich erkannte verwaltete Provider-SDK-Assemblies geladen.");
        }

        /// <summary>
        /// Die dedizierte QA-Szene baut ueber ihren dokumentierten Entry-Installer
        /// den Composition Graph auf; der Graph erfuellt dieselben Invarianten wie
        /// die direkte Composition. Zusaetzlich gilt dieselbe Vorher-/Nachher-
        /// Beobachtung: Der Snapshot erfolgt unmittelbar vor dem Laden der QA-Szene,
        /// der Vergleich nach Abschluss des Installer-/Bootstrap-Pfads; nur
        /// zusaetzlich erkannte verwaltete Provider-SDK-Assemblies schlagen fehl.
        /// </summary>
        [UnityTest]
        public IEnumerator QaScene_BootstrapInstallerBuildsComposition()
        {
            var before = LoadedProviderSdkAssemblies();

            yield return SceneManager.LoadSceneAsync(QaScenePath, LoadSceneMode.Single);

            var scene = SceneManager.GetActiveScene();
            Assert.AreEqual(QaScenePath, scene.path);

            BootstrapInstaller? installer = null;
            foreach (var root in scene.GetRootGameObjects())
            {
                installer = root.GetComponentInChildren<BootstrapInstaller>(true);
                if (installer is not null)
                {
                    break;
                }
            }

            Assert.NotNull(installer, "Die QA-Szene enthaelt keinen BootstrapInstaller.");
            var composition = installer!.Composition;
            Assert.NotNull(composition, "Der BootstrapInstaller hat keinen Composition Graph aufgebaut.");

            AssertCompleteGraph(composition!);
            AssertPortBindings(composition!);
            Assert.IsEmpty(
                NewSince(before),
                "Waehrend des untersuchten QA-Szenenpfads wurden zusaetzlich erkannte verwaltete Provider-SDK-Assemblies geladen.");
        }

        /// <summary>
        /// Listet die Namen aller aktuell geladenen verwalteten Assemblies auf, die
        /// einem bekannten Provider-SDK (Google Mobile Ads inklusive UMP, Firebase,
        /// Unity IAP, Unity Services, Unity Ads) zugehoeren. Die Praefixe benennen
        /// ausschliesslich Paket-Assemblies; Engine-eigene Module werden durch den
        /// Segmentvergleich nicht erfasst.
        /// </summary>
        private static List<string> LoadedProviderSdkAssemblies()
        {
            var loaded = new List<string>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = assembly.GetName().Name ?? string.Empty;
                if (IsProviderSdkAssembly(name))
                {
                    loaded.Add(name);
                }
            }
            return loaded;
        }

        /// <summary>
        /// Liefert die Namen der Provider-SDK-Assemblies, die seit dem uebergebenen
        /// Snapshot zusaetzlich geladen wurden. Bereits im Snapshot enthaltene
        /// Assemblies werden ignoriert.
        /// </summary>
        private static List<string> NewSince(IReadOnlyCollection<string> beforeSnapshot)
        {
            var before = new HashSet<string>(beforeSnapshot, StringComparer.OrdinalIgnoreCase);
            var added = new List<string>();
            foreach (var name in LoadedProviderSdkAssemblies())
            {
                if (!before.Contains(name))
                {
                    added.Add(name);
                }
            }
            return added;
        }

        /// <summary>
        /// Prueft einen Assemblynamen gegen die bekannten Provider-SDK-Praefixe.
        /// Ein Praefix ohne abschliessenden Punkt trifft nur auf die Assembly selbst
        /// oder auf Unterassemblies mit Punktgrenze zu, damit keine Engine-Module
        /// mit aehnlichem Namensanfang faelschlich erfasst werden.
        /// </summary>
        private static bool IsProviderSdkAssembly(string name)
        {
            string[] providerSdkPrefixes =
            {
                "GoogleMobileAds", "Firebase.", "UnityEngine.Purchasing", "Unity.Services.",
                "UnityEngine.Advertisements", "UnityEngine.Monetization",
            };
            foreach (var prefix in providerSdkPrefixes)
            {
                if (prefix.EndsWith(".", StringComparison.Ordinal))
                {
                    if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                else if (
                    name.Equals(prefix, StringComparison.OrdinalIgnoreCase) ||
                    name.StartsWith(prefix + ".", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Erstellt eine Kerncomposition ohne Providerbindung aus den bereits
        /// bekannten Skelett-Adaptern eines Composition-Ergebnisses.
        /// </summary>
        private static ApplicationComposition NewCoreComposition(BootstrapCompositionResult seed)
        {
            return new ApplicationComposition(
                seed.Persistence, seed.Content, seed.Content, seed.Content, seed.Content,
                seed.Platform, seed.Platform, seed.Platform, seed.Platform, seed.AudioAdapter);
        }

        /// <summary>
        /// Prueft den vollstaendig aufgebauten Application-/UI-/World-Graphen.
        /// </summary>
        private static void AssertCompleteGraph(BootstrapCompositionResult result)
        {
            Assert.NotNull(result.Application);
            Assert.NotNull(result.Ui);
            Assert.NotNull(result.World);
            Assert.NotNull(result.Application.Composition);
            Assert.AreSame(result.Application, result.Ui.Application);
            Assert.AreSame(result.Application, result.World.Application);
        }

        /// <summary>
        /// Prueft jede der fuenfzehn normativen Portbindungen referenzgleich gegen
        /// genau eine Adapterinstanz des Composition-Ergebnisses.
        /// </summary>
        private static void AssertPortBindings(BootstrapCompositionResult result)
        {
            var composition = result.Application.Composition;

            Assert.IsTrue(composition.ProvidersBound);
            Assert.AreSame(result.Persistence, composition.SaveRepository);
            Assert.AreSame(result.Content, composition.LevelCatalog);
            Assert.AreSame(result.Content, composition.CampaignCatalog);
            Assert.AreSame(result.Content, composition.CompletionCatalog);
            Assert.AreSame(result.Content, composition.CosmeticsCatalog);
            Assert.AreSame(result.Platform, composition.Clock);
            Assert.AreSame(result.Platform, composition.AppLifecycle);
            Assert.AreSame(result.Platform, composition.NetworkStatus);
            Assert.AreSame(result.Platform, composition.Haptics);
            Assert.AreSame(result.AudioAdapter, composition.Audio);
            Assert.AreSame(result.Google, composition.Ads);
            Assert.AreSame(result.Google, composition.Consent);
            Assert.AreSame(result.Google, composition.Analytics);
            Assert.AreSame(result.Google, composition.CrashReporting);
            Assert.AreSame(result.Store, composition.Purchase);
        }
    }
}
