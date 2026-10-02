using System;
using System.Collections;
using NUnit.Framework;
using STP.Application;
using STP.Application.Ports;
using STP.Audio;
using STP.Bootstrap;
using STP.Infrastructure.Content;
using STP.Infrastructure.Persistence;
using STP.MobileServices.Google;
using STP.MobileServices.Store;
using STP.Platform;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace STP.Tests.Bootstrap.PlayMode
{
    /// <summary>
    /// Compile-/Composition-Smoke gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 7:
    /// kompiliert die echte .asmdef-Kante, startet die Root in der dedizierten QA-Szene
    /// und belegt genau ein Binding pro Application-Port (referenzgleich gegen die
    /// Adapterinstanzen aus <see cref="BootstrapCompositionResult"/>), den vollständigen
    /// Application-/UI-/World-Graphen, keine nicht dokumentierten Null-/Fallback-Ports,
    /// keinen optionalen Providerstart sowie den Fehlschlag einer Doppelbindung.
    /// </summary>
    [TestFixture]
    public class BootstrapCompositionSmoke
    {
        private const string QaScenePath = "Assets/StammstreckenPuzzle/Scenes/QA/BootstrapComposition.unity";

        [UnityTest]
        public IEnumerator QaScene_ComposesCompleteGraph_ExactlyOneBindingPerPort_NoProviderStart()
        {
            SceneManager.LoadScene(QaScenePath, LoadSceneMode.Single);
            yield return null;

            var installer = UnityEngine.Object.FindFirstObjectByType<BootstrapInstaller>();
            Assert.NotNull(installer, "BootstrapInstaller fehlt in der QA-Szene.");
            var result = installer.Result;
            Assert.NotNull(result, "BootstrapCompositionResult wurde nicht erstellt.");

            // Vollständiger Application-/UI-/World-Graph.
            Assert.NotNull(result.Root, "ApplicationRoot fehlt.");
            Assert.NotNull(result.Root.Composition, "ApplicationComposition fehlt.");
            Assert.NotNull(result.Ui, "UI-Präsentation fehlt.");
            Assert.NotNull(result.World, "Welt-Präsentation fehlt.");

            // Genau ein Binding pro Application-Port, referenzgleich gegen die erstellten Instanzen.
            var bindings = result.Root.Composition.Bindings;
            Assert.AreEqual(15, bindings.Count, "Es müssen exakt fünfzehn Portbindungen existieren.");

            AssertBinding<ISaveRepository>(result.Root.Composition, result.Persistence);
            AssertBinding<ILevelCatalog>(result.Root.Composition, result.Content);
            AssertBinding<ICampaignCatalog>(result.Root.Composition, result.Content);
            AssertBinding<ICompletionCatalog>(result.Root.Composition, result.Content);
            AssertBinding<ICosmeticsCatalog>(result.Root.Composition, result.Content);
            AssertBinding<IClock>(result.Root.Composition, result.Platform);
            AssertBinding<IAudioPort>(result.Root.Composition, result.Audio);
            AssertBinding<IAppLifecyclePort>(result.Root.Composition, result.Platform);
            AssertBinding<INetworkStatusPort>(result.Root.Composition, result.Platform);
            AssertBinding<IHapticsPort>(result.Root.Composition, result.Platform);
            AssertBinding<IAdsPort>(result.Root.Composition, result.Google);
            AssertBinding<IPurchasePort>(result.Root.Composition, result.Store);
            AssertBinding<IConsentPort>(result.Root.Composition, result.Google);
            AssertBinding<IAnalyticsPort>(result.Root.Composition, result.Google);
            AssertBinding<ICrashReportingPort>(result.Root.Composition, result.Google);

            // Kein optionaler Providerstart: Adapter sind erstellt, aber nicht initialisiert.
            Assert.IsFalse(result.Google.IsInitialized, "Google-Adapter darf nicht initialisiert sein.");
            Assert.IsFalse(result.Store.IsInitialized, "Store-Adapter darf nicht initialisiert sein.");
        }

        [Test]
        public void Composition_RejectsNullPort_FailClosed()
        {
            Assert.Throws<ArgumentNullException>(() => ApplicationComposition.FromPorts(
                saves: null!,
                levels: new ContentModuleSkeleton(),
                campaigns: new ContentModuleSkeleton(),
                completions: new ContentModuleSkeleton(),
                cosmetics: new ContentModuleSkeleton(),
                clock: new PlatformModuleSkeleton(),
                audio: new AudioModuleSkeleton(),
                lifecycle: new PlatformModuleSkeleton(),
                network: new PlatformModuleSkeleton(),
                haptics: new PlatformModuleSkeleton()));
        }

        [Test]
        public void Composition_SecondBindProviders_FailsAsDoubleBinding()
        {
            var google = new GoogleModuleSkeleton();
            var store = new StoreModuleSkeleton();
            var composition = CreateCompleteComposition(google, store);

            Assert.Throws<InvalidOperationException>(() => composition.BindProviders(
                ads: google,
                purchases: store,
                consent: google,
                analytics: google,
                crashes: google));
        }

        [Test]
        public void Composition_Get_UnboundPort_FailsWithoutFallback()
        {
            var google = new GoogleModuleSkeleton();
            var store = new StoreModuleSkeleton();
            var composition = CreateCompleteComposition(google, store);

            Assert.Throws<InvalidOperationException>(() => composition.Get<IDisposable>());
        }

        [Test]
        public void Composition_Get_ProviderPort_BeforeBindProviders_FailsWithoutFallback()
        {
            var composition = ApplicationComposition.FromPorts(
                saves: new PersistenceModuleSkeleton(),
                levels: new ContentModuleSkeleton(),
                campaigns: new ContentModuleSkeleton(),
                completions: new ContentModuleSkeleton(),
                cosmetics: new ContentModuleSkeleton(),
                clock: new PlatformModuleSkeleton(),
                audio: new AudioModuleSkeleton(),
                lifecycle: new PlatformModuleSkeleton(),
                network: new PlatformModuleSkeleton(),
                haptics: new PlatformModuleSkeleton());

            Assert.Throws<InvalidOperationException>(() => composition.Get<IAdsPort>());
        }

        private static ApplicationComposition CreateCompleteComposition(GoogleModuleSkeleton google, StoreModuleSkeleton store)
        {
            var content = new ContentModuleSkeleton();
            var persistence = new PersistenceModuleSkeleton();
            var platform = new PlatformModuleSkeleton();
            var composition = ApplicationComposition.FromPorts(
                saves: persistence,
                levels: content,
                campaigns: content,
                completions: content,
                cosmetics: content,
                clock: platform,
                audio: new AudioModuleSkeleton(),
                lifecycle: platform,
                network: platform,
                haptics: platform);

            composition.BindProviders(
                ads: google,
                purchases: store,
                consent: google,
                analytics: google,
                crashes: google);

            return composition;
        }

        private static void AssertBinding<TPort>(ApplicationComposition composition, object expectedInstance) where TPort : class
        {
            var port = composition.Get<TPort>();
            Assert.NotNull(port, $"Port {typeof(TPort).Name} ist nicht gebunden.");
            Assert.AreSame(expectedInstance, port, $"Port {typeof(TPort).Name} ist nicht referenzgleich mit der Adapterinstanz.");
        }
    }
}
