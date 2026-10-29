using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace STP.Editor.Build
{
    /// <summary>Process-local opt-out for the pinned EDM editor measurement implementation, without a generated project setting.</summary>
    [InitializeOnLoad]
    public sealed class MobileEditorPrivacyDefaults : IPreprocessBuildWithReport
    {
        private const string ImplementationAssembly = "Google.VersionHandlerImpl";
        private const string MeasurementType = "Google.EditorMeasurement";
        static MobileEditorPrivacyDefaults()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) Disable(assembly);
            AppDomain.CurrentDomain.AssemblyLoad += (_, args) => Disable(args.LoadedAssembly);
            ScheduleBillingModeNormalization();
        }

        private static void Disable(Assembly assembly)
        {
            if (assembly.GetName().Name != ImplementationAssembly) return;
            var field = assembly.GetType(MeasurementType, false)?.GetField("GloballyEnabled", BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null && field.FieldType == typeof(bool)) field.SetValue(null, false);
        }

        /// <summary>Runs before other build processors; a changed EDM contract cannot silently re-enable measurement.</summary>
        public int callbackOrder => int.MinValue;
        /// <summary>Requires the actual pinned loaded implementation to remain disabled.</summary>
        public void OnPreprocessBuild(BuildReport report)
        {
            NormalizeGeneratedBillingMode();
            bool found = false;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name != ImplementationAssembly) continue;
                Disable(assembly);
                var field = assembly.GetType(MeasurementType, false)?.GetField("GloballyEnabled", BindingFlags.NonPublic | BindingFlags.Static);
                if (field == null || field.FieldType != typeof(bool) || !Equals(field.GetValue(null), false))
                    throw new BuildFailedException("PRIVACY_EDM_MEASUREMENT_OPTOUT_UNAVAILABLE");
                found = true;
            }
            if (!found) throw new BuildFailedException("PRIVACY_EDM_MEASUREMENT_IMPLEMENTATION_MISSING");
        }

        internal static void ScheduleBillingModeNormalization()
        {
            // Every InitializeOnLoad constructor queues before these nested rounds; never race the emitter's startup write.
            EditorApplication.delayCall += () => EditorApplication.delayCall += NormalizeGeneratedBillingMode;
        }

        /// <summary>IAP5.4.3 hardcodes a generated root resource; retain the canonical source and reject changed store configuration.</summary>
        public static void NormalizeGeneratedBillingMode()
        {
            const string canonical = "Assets/StammstreckenPuzzle/Settings/Resources/BillingMode.json";
            const string generated = "Assets/Resources/BillingMode.json";
            const string googlePlayOnly = "^\\s*\\{\\s*\"androidStore\"\\s*:\\s*\"GooglePlay\"\\s*\\}\\s*$";
            if (!File.Exists(canonical) || !Regex.IsMatch(File.ReadAllText(canonical), googlePlayOnly))
                throw new BuildFailedException("IAP_CANONICAL_GOOGLEPLAY_MODE_REQUIRED");
            if (File.Exists(generated))
            {
                if (!Regex.IsMatch(File.ReadAllText(generated), googlePlayOnly)) throw new BuildFailedException("IAP_GENERATED_STORE_MODE_MISMATCH");
                if (!AssetDatabase.DeleteAsset(generated)) throw new BuildFailedException("IAP_GENERATED_MODE_CLEANUP_FAILED");
            }
            // IAP's queued private SaveConfig assumes its MigrateBillingMode-created directory still exists.
            // Keep that generated editor work directory throughout this process, including repeated emissions.
            // Its untracked folder metadata is archived after the editor exits; it is not a source asset.
        }
    }

    internal sealed class GeneratedBillingModeImporter : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] previous)
        {
            foreach (var path in imported)
                if (path == "Assets/Resources/BillingMode.json") { MobileEditorPrivacyDefaults.ScheduleBillingModeNormalization(); break; }
        }
    }
}
