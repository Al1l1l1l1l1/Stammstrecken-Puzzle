using System;
using System.IO;
using STP.Bootstrap;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

namespace STP.Editor.Build
{
    /// <summary>
    /// WP-021-Buildentrypoints gemäß der Verantwortung von STP.Editor.Build
    /// (reproduzierbare Buildentrypoints und Preflight, MODULE_BOUNDARIES.md
    /// Abschnitt 3) sowie den Plattformbaselines aus ARCHITECTURE/TECH_STACK.md
    /// Abschnitt 2 und ARCHITECTURE/BUILD_AND_RELEASE.md Abschnitte 7 und 8.
    /// Alle Methoden sind für den headless CI-Aufruf per -executeMethod ausgelegt
    /// und schlagen bei Abweichung mit einer Ausnahme fehl (Batchmode-Exitcode
    /// ungleich 0).
    /// </summary>
    public static class StpBuildEntrypoints
    {
        /// <summary>Autoritative Editor-Baseline gemäß ADR-001 und ProjectVersion.txt.</summary>
        public const string ExpectedEditorVersion = "6000.3.23f1";

        /// <summary>Dedizierte QA-Szene des Bootstrap-Composition-Smoke.</summary>
        public const string QaScenePath = "Assets/StammstreckenPuzzle/Scenes/QA/BootstrapComposition.unity";

        /// <summary>
        /// Preflight: prüft, dass die laufende Editorversion und
        /// ProjectSettings/ProjectVersion.txt exakt der autoritativen Baseline
        /// entsprechen (ADR-001). Ein Ausweichen auf einen anderen Patch ist
        /// ein harter Fehler.
        /// </summary>
        public static void VerifyProjectVersion()
        {
            VerifyScaffoldBuildIdentity();
            var versionFile = Path.Combine("ProjectSettings", "ProjectVersion.txt");
            var content = File.ReadAllText(versionFile);
            var expectedLine = $"m_EditorVersion: {ExpectedEditorVersion}";
            if (!content.Contains(expectedLine, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"ProjectVersion.txt enthaelt nicht '{expectedLine}'. Inhalt: {content}");
            }

            if (!string.Equals(Application.unityVersion, ExpectedEditorVersion, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Editorversion {Application.unityVersion} weicht von der autoritativen Baseline {ExpectedEditorVersion} ab.");
            }

            if (!File.Exists(QaScenePath))
            {
                throw new InvalidOperationException($"QA-Szene fehlt: {QaScenePath}");
            }

            VerifyRenderingAndStripping();
            Debug.Log($"STP Preflight PASS: Unity {Application.unityVersion}, ProjectVersion.txt konsistent, QA-Szene vorhanden.");
        }

        /// <summary>
        /// Erzeugt den Android-Development-IL2CPP-Build (ARM64) aus der QA-Szene.
        /// minSdk 26 und targetSdk 36 folgen ADR-012 beziehungsweise dem aktuellen
        /// Google-Play-Mandat (TECH_STACK.md Abschnitt 2).
        /// </summary>
        public static void BuildAndroidDevelopmentIl2Cpp()
        {
            VerifyScaffoldBuildIdentity();
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            // API 36 entspricht dem Google-Play-Mandat zum Architekturstand (2026-08-31).
            PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)36;

            BuildPlayerOrThrow(
                locationPathName: "Builds/Android/stp-qa-development.apk",
                target: BuildTarget.Android,
                options: BuildOptions.Development);
        }

        /// <summary>
        /// Exportiert das iOS-Xcodeprojekt (IL2CPP, ARM64, Deployment Target iOS 15.0)
        /// aus der QA-Szene. Der eigentliche xcodebuild-Lauf erfolgt headless auf dem
        /// macOS-Runner (BUILD_AND_RELEASE.md Abschnitt 8).
        /// </summary>
        public static void ExportIosXcodeProjectIl2Cpp()
        {
            VerifyScaffoldBuildIdentity();
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.iOS.targetOSVersionString = "15.0";

            BuildPlayerOrThrow(
                locationPathName: "Builds/iOS/Xcode",
                target: BuildTarget.iOS,
                options: BuildOptions.None);
        }

        /// <summary>
        /// Prüft beide eingecheckten Plattformidentitäten gegen das explizite QA-Profil.
        /// Leere, Production-, fremde oder andere Profil-IDs brechen vor jeder Buildmutation
        /// ab; eine Fehlkonfiguration wird niemals durch automatisches Überschreiben verdeckt.
        /// </summary>
        public static void VerifyScaffoldBuildIdentity()
        {
            var configuration = new BootstrapConfiguration();
            foreach (var target in new[] { UnityEditor.Build.NamedBuildTarget.Android, UnityEditor.Build.NamedBuildTarget.iOS })
            {
                var identifier = PlayerSettings.GetApplicationIdentifier(target);
                if (!string.Equals(identifier, configuration.ApplicationIdentifier, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"Scaffold identity: {target.TargetName} muss fuer Profil {configuration.Profile} exakt '{configuration.ApplicationIdentifier}' verwenden; gefunden '{identifier}'.");
                }
            }
            Debug.Log($"STP Scaffold identity PASS: profile={configuration.Profile}, Android/iOS={configuration.ApplicationIdentifier}.");
        }

        private static void BuildPlayerOrThrow(string locationPathName, BuildTarget target, BuildOptions options)
        {
            VerifyProjectVersion();
            if (!File.Exists(QaScenePath))
            {
                throw new InvalidOperationException($"QA-Szene fehlt: {QaScenePath}");
            }

            var report = BuildPipeline.BuildPlayer(new[] { QaScenePath }, locationPathName, target, options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Build fuer {target} fehlgeschlagen: {report.summary.result}, Fehler: {report.summary.totalErrors}");
            }

            Debug.Log($"STP Build PASS: {target} -> {locationPathName} ({report.summary.totalSize} Bytes).");
        }

        private static void VerifyRenderingAndStripping()
        {
            var pipeline = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(
                "Assets/StammstreckenPuzzle/Settings/StpUniversalRP.asset");
            var renderer = AssetDatabase.LoadAssetAtPath<ScriptableObject>(
                "Assets/StammstreckenPuzzle/Settings/StpRenderer2D.asset");
            if (pipeline == null || renderer == null ||
                pipeline.GetType().FullName != "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset" ||
                renderer.GetType().FullName != "UnityEngine.Rendering.Universal.Renderer2DData" ||
                GraphicsSettings.defaultRenderPipeline != pipeline || GraphicsSettings.currentRenderPipeline != pipeline)
            {
                throw new InvalidOperationException("URP-Pipeline/2D-Renderer fehlt oder ist nicht aktiv zugewiesen.");
            }

            var pipelineData = new SerializedObject(pipeline);
            var renderers = pipelineData.FindProperty("m_RendererDataList");
            if (renderers == null || renderers.arraySize != 1 ||
                renderers.GetArrayElementAtIndex(0).objectReferenceValue != renderer ||
                pipelineData.FindProperty("m_DefaultRendererIndex").intValue != 0)
            {
                throw new InvalidOperationException("URP-Pipeline muss den dokumentierten 2D-Renderer verwenden.");
            }

            var quality = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/QualitySettings.asset")[0]);
            var levels = quality.FindProperty("m_QualitySettings");
            for (var index = 0; index < levels.arraySize; index++)
            {
                if (levels.GetArrayElementAtIndex(index).FindPropertyRelative("customRenderPipeline").objectReferenceValue != pipeline)
                {
                    throw new InvalidOperationException($"Quality-Level {index} besitzt nicht die dokumentierte URP-Zuweisung.");
                }
            }

            if (PlayerSettings.GetManagedStrippingLevel(UnityEditor.Build.NamedBuildTarget.Android) != ManagedStrippingLevel.Medium ||
                PlayerSettings.GetManagedStrippingLevel(UnityEditor.Build.NamedBuildTarget.iOS) != ManagedStrippingLevel.Medium)
            {
                throw new InvalidOperationException("Android/iOS Managed Stripping muss explizit Medium sein.");
            }
            Debug.Log($"STP Rendering/Stripping PASS: URP mit 2D-Renderer, {levels.arraySize} Quality-Level, Android/iOS Medium.");
        }
    }
}
