using System;
using System.Collections;
using System.Collections.Generic;
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
using STP.Presentation.UI;
using STP.Presentation.World;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine.Rendering;
#endif

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

#if UNITY_EDITOR
        [Test]
        public void Composition_CompiledConstructorOrder_IncludesConfigurationAndLoggerBeforeContent()
        {
            // Prüft die echten newobj-Instruktionen der kompilierten Root, keine
            // separat gepflegte Reihenfolgenliste im Produktionscode.
            var method = typeof(BootstrapComposition).GetMethod(nameof(BootstrapComposition.Compose))!;
            var code = method.GetMethodBody()!.GetILAsByteArray();
            var opcodes = new Dictionary<short, OpCode>();
            foreach (var field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.GetValue(null) is OpCode opcode)
                {
                    opcodes[opcode.Value] = opcode;
                }
            }
            var constructors = new List<string>();
            for (var offset = 0; offset < code.Length;)
            {
                short value = code[offset++];
                if (value == 0xfe)
                {
                    value = (short)(0xfe00 | code[offset++]);
                }
                var opcode = opcodes[value];
                if (opcode == OpCodes.Newobj)
                {
                    constructors.Add(method.Module.ResolveMethod(BitConverter.ToInt32(code, offset))!.DeclaringType!.FullName!);
                }
                switch (opcode.OperandType)
                {
                    case OperandType.InlineNone: break;
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineVar: offset += 1; break;
                    case OperandType.InlineVar: offset += 2; break;
                    case OperandType.InlineI8:
                    case OperandType.InlineR: offset += 8; break;
                    case OperandType.InlineSwitch:
                        offset += 4 + (4 * BitConverter.ToInt32(code, offset)); break;
                    default: offset += 4; break;
                }
            }
            CollectionAssert.AreEqual(new[]
            {
                "STP.Bootstrap.BootstrapConfiguration", "STP.Bootstrap.LocalBootstrapLogger",
                typeof(ContentModuleSkeleton).FullName, typeof(PersistenceModuleSkeleton).FullName,
                typeof(PlatformModuleSkeleton).FullName, typeof(AudioModuleSkeleton).FullName, typeof(ApplicationRoot).FullName,
                typeof(UiPresentationSkeleton).FullName, typeof(WorldPresentationSkeleton).FullName,
                typeof(GoogleModuleSkeleton).FullName, typeof(StoreModuleSkeleton).FullName, typeof(BootstrapCompositionResult).FullName,
            }, constructors);
        }

        [Test]
        public void Project_ScaffoldIdentity_IsExplicitQaOnAndroidAndIos()
        {
            Assert.AreEqual("com.STP.StammstreckenPuzzle.qa", PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android));
            Assert.AreEqual("com.STP.StammstreckenPuzzle.qa", PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.iOS));
            BuildEntrypoint("VerifyProjectVersion").Invoke(null, null);
        }

        [TestCase("Android", "com.STP.StammstreckenPuzzle")]
        [TestCase("iOS", "com.STP.StammstreckenPuzzle")]
        [TestCase("Android", "")]
        [TestCase("iOS", "")]
        [TestCase("Android", "com.STP.StammstreckenPuzzle.dev")]
        [TestCase("iOS", "com.STP.StammstreckenPuzzle.dev")]
        [TestCase("Android", "com.STP.StammstreckenPuzzle.staging")]
        [TestCase("iOS", "com.STP.StammstreckenPuzzle.staging")]
        [TestCase("Android", "com.other.product.qa")]
        [TestCase("iOS", "com.other.product.qa")]
        public void BuildEntrypoints_RejectWrongIdentityBeforeAnyBuild(string platform, string identifier)
        {
            var target = platform == "Android" ? NamedBuildTarget.Android : NamedBuildTarget.iOS;
            var original = PlayerSettings.GetApplicationIdentifier(target);
            try
            {
                PlayerSettings.SetApplicationIdentifier(target, identifier);
                foreach (var entrypoint in new[] { "VerifyProjectVersion", "BuildAndroidDevelopmentIl2Cpp", "ExportIosXcodeProjectIl2Cpp" })
                {
                    var failure = Assert.Throws<TargetInvocationException>(() => BuildEntrypoint(entrypoint).Invoke(null, null));
                    Assert.That(failure!.InnerException, Is.TypeOf<InvalidOperationException>());
                    StringAssert.Contains("Scaffold identity", failure.InnerException!.Message, entrypoint);
                    Assert.AreEqual(identifier, PlayerSettings.GetApplicationIdentifier(target), "Fehlkonfiguration darf nicht still korrigiert werden.");
                }
            }
            finally
            {
                PlayerSettings.SetApplicationIdentifier(target, original);
            }
        }

        // Nur Testaufruf: keine Editor-Abhängigkeit in der playerfähigen Testassembly.
        private static MethodInfo BuildEntrypoint(string name) =>
            Type.GetType("STP.Editor.Build.StpBuildEntrypoints, STP.Editor.Build", true)!.GetMethod(name)!;

        [Test]
        public void Project_UrpAssetAndRenderer_AreAssignedForEveryQualityLevel()
        {
            var pipeline = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(
                "Assets/StammstreckenPuzzle/Settings/StpUniversalRP.asset");
            Assert.NotNull(pipeline);
            Assert.AreEqual("UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset", pipeline.GetType().FullName);
            var renderer = AssetDatabase.LoadAssetAtPath<ScriptableObject>(
                "Assets/StammstreckenPuzzle/Settings/StpRenderer2D.asset");
            Assert.NotNull(renderer);
            Assert.AreEqual("UnityEngine.Rendering.Universal.Renderer2DData", renderer.GetType().FullName);
            var data = new SerializedObject(pipeline);
            var renderers = data.FindProperty("m_RendererDataList");
            Assert.AreEqual(1, renderers.arraySize);
            Assert.AreSame(renderer, renderers.GetArrayElementAtIndex(0).objectReferenceValue);
            Assert.AreEqual(0, data.FindProperty("m_DefaultRendererIndex").intValue);
            Assert.AreSame(pipeline, GraphicsSettings.defaultRenderPipeline);
            var previous = QualitySettings.GetQualityLevel();
            try
            {
                for (var quality = 0; quality < QualitySettings.names.Length; quality++)
                {
                    QualitySettings.SetQualityLevel(quality, false);
                    Assert.AreSame(pipeline, QualitySettings.renderPipeline, QualitySettings.names[quality]);
                    Assert.AreSame(pipeline, GraphicsSettings.currentRenderPipeline);
                }
            }
            finally
            {
                QualitySettings.SetQualityLevel(previous, false);
            }
        }

        [Test]
        public void Project_AndroidAndIos_ExplicitlyUseMediumManagedStripping()
        {
            Assert.AreEqual(ManagedStrippingLevel.Medium, PlayerSettings.GetManagedStrippingLevel(NamedBuildTarget.Android));
            Assert.AreEqual(ManagedStrippingLevel.Medium, PlayerSettings.GetManagedStrippingLevel(NamedBuildTarget.iOS));
        }
#endif

        [Test]
        public void Composition_ConfigurationAndLocalLogger_AreWiredAndUsedWithoutGlobalState()
        {
            LogAssert.Expect(LogType.Log, "STP Bootstrap [qa]: composition started.");
            LogAssert.Expect(LogType.Log, "STP Bootstrap [qa]: composition completed; providers not initialized.");
            var result = BootstrapComposition.Compose();
            Assert.AreEqual("qa", result.Configuration.Profile);
            Assert.AreEqual("com.STP.StammstreckenPuzzle.qa", result.Configuration.ApplicationIdentifier);
            Assert.AreSame(result.Configuration, result.Logger.Configuration);
            Assert.Throws<ArgumentNullException>(() => new LocalBootstrapLogger(null!));
            var second = BootstrapComposition.Compose();
            Assert.AreNotSame(result.Configuration, second.Configuration);
            Assert.AreNotSame(result.Logger, second.Logger);
            Assert.AreSame(second.Configuration, second.Logger.Configuration);
        }

        [Test]
        public void Presentation_RequiresApplication_AndCanBeWiredBeforeProviders()
        {
            var composition = CreateLocalComposition();
            var root = new ApplicationRoot(composition);
            var ui = new UiPresentationSkeleton(root);
            var world = new WorldPresentationSkeleton(root);
            Assert.AreSame(root, ui.Application);
            Assert.AreSame(root, world.Application);
            Assert.AreEqual(10, root.Composition.Bindings.Count);
            Assert.Throws<InvalidOperationException>(() => ui.Application.Composition.Get<IAdsPort>());
            Assert.Throws<InvalidOperationException>(() => world.Application.Composition.Get<IPurchasePort>());
            Assert.Throws<ArgumentNullException>(() => new UiPresentationSkeleton(null!));
            Assert.Throws<ArgumentNullException>(() => new WorldPresentationSkeleton(null!));
            var google = new GoogleModuleSkeleton();
            var store = new StoreModuleSkeleton();
            composition.BindProviders(google, store, google, google, google);
            Assert.AreSame(google, ui.Application.Composition.Get<IAdsPort>());
            Assert.AreSame(store, world.Application.Composition.Get<IPurchasePort>());
        }

        [Test]
        public void Composition_Bindings_CannotBeMutatedThroughDictionaryInterfaces()
        {
            var result = BootstrapComposition.Compose();
            var composition = result.Root.Composition;
            Assert.IsFalse(composition.Bindings is Dictionary<Type, object>);
            var dictionary = (IDictionary<Type, object>)composition.Bindings;
            Assert.IsTrue(dictionary.IsReadOnly);
            Assert.Throws<NotSupportedException>(() => dictionary.Clear());
            Assert.Throws<NotSupportedException>(() => dictionary.Remove(typeof(IAdsPort)));
            Assert.Throws<NotSupportedException>(() => dictionary[typeof(IAdsPort)] = new GoogleModuleSkeleton());
            var legacy = (IDictionary)composition.Bindings;
            Assert.Throws<NotSupportedException>(() => legacy.Clear());
            Assert.Throws<NotSupportedException>(() => legacy.Remove(typeof(IAdsPort)));
            Assert.Throws<NotSupportedException>(() => legacy[typeof(IAdsPort)] = new GoogleModuleSkeleton());
            Assert.AreEqual(15, composition.Bindings.Count);
            Assert.AreSame(result.Google, composition.Get<IAdsPort>());
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void Composition_NullProvider_LeavesAllProviderPortsUnbound_ThenAllowsRetry(int nullIndex)
        {
            var composition = CreateLocalComposition();
            var google = new GoogleModuleSkeleton();
            var store = new StoreModuleSkeleton();
            var view = composition.Bindings;
            var original = new Dictionary<Type, object>(view);
            Assert.Throws<ArgumentNullException>(() => composition.BindProviders(
                nullIndex == 0 ? null! : google,
                nullIndex == 1 ? null! : store,
                nullIndex == 2 ? null! : google,
                nullIndex == 3 ? null! : google,
                nullIndex == 4 ? null! : google));
            Assert.AreEqual(10, view.Count);
            foreach (var pair in original)
            {
                Assert.AreSame(pair.Value, view[pair.Key]);
            }
            Assert.Throws<InvalidOperationException>(() => composition.Get<IAdsPort>());
            Assert.Throws<InvalidOperationException>(() => composition.Get<IPurchasePort>());
            Assert.Throws<InvalidOperationException>(() => composition.Get<IConsentPort>());
            Assert.Throws<InvalidOperationException>(() => composition.Get<IAnalyticsPort>());
            Assert.Throws<InvalidOperationException>(() => composition.Get<ICrashReportingPort>());
            composition.BindProviders(google, store, google, google, google);
            Assert.AreEqual(15, view.Count);
            AssertBinding<IAdsPort>(composition, google);
            AssertBinding<IPurchasePort>(composition, store);
            AssertBinding<IConsentPort>(composition, google);
            AssertBinding<IAnalyticsPort>(composition, google);
            AssertBinding<ICrashReportingPort>(composition, google);
            Assert.Throws<InvalidOperationException>(() => composition.BindProviders(google, store, google, google, google));
        }

        [UnityTest]
        public IEnumerator QaScene_ComposesCompleteGraph_ExactlyOneBindingPerPort_NoProviderStart()
        {
            SceneManager.LoadScene(QaScenePath, LoadSceneMode.Single);
            yield return null;

            var installer = UnityEngine.Object.FindFirstObjectByType<BootstrapInstaller>();
            Assert.NotNull(installer, "BootstrapInstaller fehlt in der QA-Szene.");
            var result = installer.Result;
            Assert.NotNull(result, "BootstrapCompositionResult wurde nicht erstellt.");
            Assert.AreEqual("qa", result.Configuration.Profile);
            Assert.AreSame(result.Configuration, result.Logger.Configuration);

            // Vollständiger Application-/UI-/World-Graph.
            Assert.NotNull(result.Root, "ApplicationRoot fehlt.");
            Assert.NotNull(result.Root.Composition, "ApplicationComposition fehlt.");
            Assert.NotNull(result.Ui, "UI-Präsentation fehlt.");
            Assert.NotNull(result.World, "Welt-Präsentation fehlt.");
            Assert.AreSame(result.Root, result.Ui.Application, "UI muss die echte ApplicationRoot erhalten.");
            Assert.AreSame(result.Root, result.World.Application, "World muss die echte ApplicationRoot erhalten.");

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
            var composition = CreateLocalComposition();
            composition.BindProviders(google, store, google, google, google);
            return composition;
        }

        private static ApplicationComposition CreateLocalComposition()
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
