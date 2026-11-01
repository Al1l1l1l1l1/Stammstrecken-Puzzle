using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace STP.Tests.Application.EditMode
{
    public sealed class MobileEditorSourceIntegrationTests
    {
        [Test]
        public void ActualIapPrivateEmitterCanWriteTwiceWhileCanonicalBillingModeAndGuidRemainStable()
        {
            const string canonical = "Assets/StammstreckenPuzzle/Settings/Resources/BillingMode.json";
            var original = File.ReadAllBytes(canonical); var originalMeta = File.ReadAllBytes(canonical + ".meta");
            var defaults = Assembly.Load("STP.Editor.Build").GetType("STP.Editor.Build.MobileEditorPrivacyDefaults", true)!;
            var normalize = defaults.GetMethod("NormalizeGeneratedBillingMode", BindingFlags.Public | BindingFlags.Static)!;
            var editor = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("UnityEditor.Purchasing.UnityPurchasingEditor", false)).First(t => t != null)!;
            var emitter = editor.GetMethod("SaveConfig", BindingFlags.Static | BindingFlags.NonPublic)!;
            var googlePlay = Enum.Parse(emitter.GetParameters()[0].ParameterType, "GooglePlay");
            for (int emission = 0; emission < 2; emission++)
            {
                // This invokes the actual pinned package emitter, without TargetAndroidStore's telemetry/menu action.
                Directory.CreateDirectory("Assets/Resources");
                emitter.Invoke(null, new[] { googlePlay });
                Assert.That(File.Exists("Assets/Resources/BillingMode.json"), Is.True);
                normalize.Invoke(null, null);
                Assert.That(File.Exists("Assets/Resources/BillingMode.json"), Is.False);
                Assert.That(Directory.Exists("Assets/Resources"), Is.True, "Queued and repeated package writes require their work directory");
                Assert.That(File.ReadAllBytes(canonical), Is.EqualTo(original));
                Assert.That(File.ReadAllBytes(canonical + ".meta"), Is.EqualTo(originalMeta));
            }
        }

        [Test]
        public void ActualLoadedEdmMeasurementIsDisabledAndRealPreBuildGuardAcceptsIt()
        {
            var defaults = Assembly.Load("STP.Editor.Build").GetType("STP.Editor.Build.MobileEditorPrivacyDefaults", true)!;
            var instance = Activator.CreateInstance(defaults)!;
            defaults.GetMethod("OnPreprocessBuild")!.Invoke(instance, new object?[] { null });
            var measurement = Assembly.Load("Google.VersionHandlerImpl").GetType("Google.EditorMeasurement", true)!;
            var enabled = measurement.GetField("GloballyEnabled", BindingFlags.Static | BindingFlags.NonPublic)!;
            Assert.That(enabled.GetValue(null), Is.False);
        }
    }
}
