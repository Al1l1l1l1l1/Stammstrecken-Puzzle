using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace STP.Editor.Build
{
    /// <summary>
    /// WP-008-Buildentrypoints gemäß der Verantwortung von STP.Editor.Build
    /// (reproduzierbare Buildentrypoints und Preflight, MODULE_BOUNDARIES.md
    /// Abschnitt 3). Alle Methoden sind für den headless CI-Aufruf per
    /// -executeMethod ausgelegt und schlagen bei Abweichung mit einer
    /// Ausnahme fehl (Batchmode-Exitcode ungleich 0).
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

            Debug.Log($"STP Preflight PASS: Unity {Application.unityVersion}, ProjectVersion.txt konsistent, QA-Szene vorhanden.");
        }

        /// <summary>
        /// Erzeugt den Android-Development-IL2CPP-Build (ARM64) aus der QA-Szene.
        /// minSdk 26 und targetSdk 36 folgen ADR-012 beziehungsweise dem aktuellen
        /// Google-Play-Mandat (Architecture v1.0, TECH_STACK.md Abschnitt 2).
        /// </summary>
        public static void BuildAndroidDevelopmentIl2Cpp()
        {
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
        /// aus der QA-Szene. Der eigentliche xcodebuild-Archive-/Exportlauf erfolgt
        /// headless auf dem macOS-Runner (BUILD_AND_RELEASE.md Abschnitt 8).
        /// </summary>
        public static void ExportIosXcodeProjectIl2Cpp()
        {
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.iOS.targetOSVersionString = "15.0";

            BuildPlayerOrThrow(
                locationPathName: "Builds/iOS/Xcode",
                target: BuildTarget.iOS,
                options: BuildOptions.None);
        }

        private static void BuildPlayerOrThrow(string locationPathName, BuildTarget target, BuildOptions options)
        {
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
    }
}
