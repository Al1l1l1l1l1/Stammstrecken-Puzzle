using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEditor.Build;

namespace STP.Editor.Build
{
    /// <summary>ADR042 Android-only generated Human variant. No package, global template or Candidate source mutation.</summary>
    public static class HumanAndroidBuildFence
    {
        /// <summary>Only the explicit isolated observation app may receive this generated policy.</summary>
        public const string ApplicationId = "com.STP.StammstreckenPuzzle.human";
        /// <summary>Root must build this actual exclusive scene, not infer scope from a label.</summary>
        public const string ScenePath = "Assets/StammstreckenPuzzle/Scenes/QA/HumanCalibration.unity";
        private const string ExportProvider = "com.stp.human.StpHumanFileProvider";
        private const string ExportAuthority = ApplicationId + ".humanexports";
        private const string ExportResource = "@xml/stp_human_export_paths";
        private const string MainActivity = "com.unity3d.player.UnityPlayerGameActivity";
        private static readonly XNamespace Android = "http://schemas.android.com/apk/res/android";
        private static readonly XNamespace Tools = "http://schemas.android.com/tools";
        private static readonly string[] RemovedPermissions = { "android.permission.INTERNET", "android.permission.ACCESS_NETWORK_STATE", "com.android.vending.BILLING", "com.google.android.gms.permission.AD_ID", "android.permission.ACCESS_ADSERVICES_AD_ID", "android.permission.ACCESS_ADSERVICES_ATTRIBUTION", "android.permission.ACCESS_ADSERVICES_TOPICS", "android.permission.WAKE_LOCK", "android.permission.FOREGROUND_SERVICE" };
        private static readonly string[] RemovedMetadata = { "com.google.android.gms.ads.flag.OPTIMIZE_AD_LOADING", "com.google.android.gms.ads.flag.OPTIMIZE_INITIALIZATION", "com.google.android.play.billingclient.version", "com.google.android.gms.version", "com.google.android.gms.ads.APPLICATION_ID", "com.google.unity.ads.UNITY_VERSION", "com.google.android.gms.ads.DELAY_APP_MEASUREMENT_INIT" };
        private static readonly string[] Removed_activity = { "com.google.android.gms.ads.AdActivity", "com.google.android.gms.ads.OutOfContextTestingActivity", "com.google.android.gms.ads.NotificationHandlerActivity", "com.google.android.play.core.hsdp.service.HsdpShimActivity", "com.android.billingclient.api.ProxyBillingActivity", "com.android.billingclient.api.ProxyBillingActivityV2", "com.google.android.gms.common.api.GoogleApiActivity" };
        private static readonly string[] Removed_provider = { "com.google.android.gms.ads.MobileAdsInitProvider", "androidx.startup.InitializationProvider" };
        private static readonly string[] Removed_service = { "com.google.android.gms.ads.AdService", "androidx.work.impl.background.systemalarm.SystemAlarmService", "androidx.work.impl.background.systemjob.SystemJobService", "androidx.work.impl.foreground.SystemForegroundService", "com.google.android.datatransport.runtime.backends.TransportBackendDiscovery", "com.google.android.datatransport.runtime.scheduling.jobscheduling.JobInfoSchedulerService", "androidx.room.MultiInstanceInvalidationService" };
        private static readonly string[] Removed_receiver = { "androidx.work.impl.utils.ForceStopRunnable$BroadcastReceiver", "androidx.work.impl.background.systemalarm.ConstraintProxy$BatteryChargingProxy", "androidx.work.impl.background.systemalarm.ConstraintProxy$BatteryNotLowProxy", "androidx.work.impl.background.systemalarm.ConstraintProxy$StorageNotLowProxy", "androidx.work.impl.background.systemalarm.ConstraintProxy$NetworkStateProxy", "androidx.work.impl.background.systemalarm.RescheduleReceiver", "androidx.work.impl.background.systemalarm.ConstraintProxyUpdateReceiver", "androidx.work.impl.diagnostics.DiagnosticsReceiver", "androidx.profileinstaller.ProfileInstallReceiver", "com.google.android.datatransport.runtime.scheduling.jobscheduling.AlarmManagerSchedulerBroadcastReceiver" };
        private static readonly string[] Removed_uses_library = { "android.ext.adservices", "org.apache.http.legacy" };
        /// <summary>Root supplies the actual BuildPlayer scene selection and actual player identifier before generation.</summary>
        public static void RequireScope(string applicationId, string[] scenePaths)
        {
            Require(applicationId == ApplicationId && scenePaths != null && scenePaths.Length == 1 && scenePaths[0] == ScenePath, "HUMAN_BUILD_SCOPE_REQUIRED");
        }
        /// <summary>After Unity regenerated the launcher for a non-Human app, removes only exact owned Human inputs. Validates the entire plan before changing any file.</summary>
        public static void CleanupGeneratedHumanFiles(string generatedRoot, string applicationId)
        {
            Require(!string.IsNullOrWhiteSpace(applicationId) && !applicationId.EndsWith(".human", StringComparison.OrdinalIgnoreCase), "HUMAN_CLEANUP_NONHUMAN_REQUIRED");
            var root = Path.GetFullPath(generatedRoot);
            var comparison = Path.DirectorySeparatorChar == '\\' ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            Require(Path.GetDirectoryName(root) != null, "HUMAN_CLEANUP_ROOT_INVALID");
            string Bound(string relative)
            {
                var path = Path.GetFullPath(Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar)));
                Require(path.StartsWith(root.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, comparison), "HUMAN_CLEANUP_PATH_ESCAPE");
                for (string? part = path; part != null; part = Path.GetDirectoryName(part))
                    if (File.Exists(part) || Directory.Exists(part)) Require((File.GetAttributes(part) & FileAttributes.ReparsePoint) == 0, "HUMAN_CLEANUP_REPARSE_FORBIDDEN");
                return path;
            }
            var gradle = Bound("launcher/build.gradle"); var manifest = Bound("launcher/src/main/AndroidManifest.xml");
            Require(File.Exists(gradle) && File.Exists(manifest), "HUMAN_CLEANUP_GENERATED_LAUNCHER_MISSING");
            var script = File.ReadAllText(gradle);
            var ids = Regex.Matches(script, "(?m)^\\s*applicationId\\s+[\\\"']([^\\\"']+)[\\\"']\\s*\\r?$");
            Require(ids.Count == 1 && ids[0].Groups[1].Value == applicationId, "HUMAN_CLEANUP_APPLICATION_ID_MISMATCH");
            var document = XDocument.Load(manifest); var element = document.Root;
            Require(element != null && element.Name == "manifest", "HUMAN_CLEANUP_MANIFEST_INVALID");
            Require((string?)element!.Attribute("package") != ApplicationId, "HUMAN_CLEANUP_STALE_MANIFEST");
            var humanOnlyNames = RemovedPermissions.Where(x => x != "com.google.android.gms.permission.AD_ID").Concat(RemovedMetadata).Concat(Removed_activity).Concat(Removed_provider).Concat(Removed_service).Concat(Removed_receiver).Concat(Removed_uses_library).ToArray();
            Require(!element.Descendants().Any(x => (string?)x.Attribute(Android + "name") == ExportProvider ||
                (string?)x.Attribute(Android + "authorities") == ExportAuthority ||
                x.Attribute(Tools + "node") != null && (x.Name == "queries" || humanOnlyNames.Contains((string?)x.Attribute(Android + "name"))) ||
                ((string?)x.Attribute(Tools + "replace") ?? "").Split(',').Any(v => v.Trim() == "android:allowBackup" || v.Trim() == "android:fullBackupContent")), "HUMAN_CLEANUP_STALE_MANIFEST");
            const string javaRelative = "launcher/src/main/java/com/stp/human/StpHumanFileProvider.java";
            const string pathsRelative = "launcher/src/main/res/xml/stp_human_export_paths.xml";
            const string javaText = "package com.stp.human;\n\n/** Generated only for the isolated ADR042 Human launcher. */\npublic final class StpHumanFileProvider extends androidx.core.content.FileProvider { }\n";
            const string pathsText = "<paths><files-path name=\"human_exports\" path=\"human-exports/\" /></paths>\n";
            const string core = "dependencies { implementation 'androidx.core:core:1.15.0' }";
            const string apply = "apply from: rootProject.file('stp-human-privacy.gradle')";
            var deletes = new List<string>();
            void Owned(string relative, string expected)
            {
                var path = Bound(relative); if (!File.Exists(path)) return;
                Require(File.ReadAllText(path) == expected, "HUMAN_CLEANUP_OWNED_FILE_CHANGED:" + relative); deletes.Add(path);
            }
            Owned(javaRelative, javaText); Owned(pathsRelative, pathsText); Owned("stp-human-privacy.gradle", GradleGate);
            var scope = Bound("stp-human-generated-scope.txt");
            if (File.Exists(scope))
            {
                var lines = File.ReadAllLines(scope);
                Require(lines.Length == 7 && lines[0] == "ApplicationId=" + ApplicationId && lines[1] == "Scene=" + ScenePath &&
                    lines[6] == "Evidence=GENERATED_HUMAN_INPUT_ONLY;merged_manifest_final_APK_and_physical_capture_required", "HUMAN_CLEANUP_SCOPE_CHANGED");
                var names = new[] { "LauncherSourceSha256", "ExportPathsSha256", "FileProviderJavaSha256", "GradleGateSha256" };
                for (int i = 0; i < names.Length; i++) Require(Regex.IsMatch(lines[i + 2], "^" + names[i] + "=[0-9a-f]{64}$"), "HUMAN_CLEANUP_SCOPE_CHANGED");
                var ownedPaths = new[] { pathsRelative, javaRelative, "stp-human-privacy.gradle" };
                for (int i = 0; i < ownedPaths.Length; i++) { var path = Bound(ownedPaths[i]); if (File.Exists(path)) Require(lines[i + 3].EndsWith("=" + Sha(path), StringComparison.Ordinal), "HUMAN_CLEANUP_SCOPE_HASH_MISMATCH"); }
                deletes.Add(scope);
            }
            var reportDirectory = Bound("launcher/build/reports/stp-human");
            if (Directory.Exists(reportDirectory))
            {
                Require(!Directory.EnumerateDirectories(reportDirectory).Any(), "HUMAN_CLEANUP_UNKNOWN_REPORT_CHILD");
                foreach (var report in Directory.GetFiles(reportDirectory))
                {
                    Bound(Path.GetRelativePath(root, report));
                    var match = Regex.Match(Path.GetFileName(report), "^process(?:Debug|Release)(?:MainManifest|ManifestForPackage)-([0-9a-f]{64})\\.txt$");
                    Require(match.Success, "HUMAN_CLEANUP_UNKNOWN_REPORT");
                    var expected = "HumanManifestSha256=" + match.Groups[1].Value + "\nNetworkBillingAdsPermissions=absent\nForeignComponents=absent\nEvidence=ACTUAL_MERGED_MANIFEST_ONLY;final_APK_and_physical_capture_still_required\n";
                    Require(File.ReadAllText(report) == expected, "HUMAN_CLEANUP_REPORT_CHANGED"); deletes.Add(report);
                }
            }
            var linesBefore = script.Replace("\r\n", "\n").Split('\n');
            Require(linesBefore.Count(x => x.Trim() == apply) <= 1 && linesBefore.Count(x => x.Trim() == core) <= 1, "HUMAN_CLEANUP_GRADLE_DUPLICATE");
            Require(linesBefore.All(x => !x.Contains("stp-human") || x.Trim() == apply), "HUMAN_CLEANUP_UNKNOWN_GRADLE_HOOK");
            var hasHumanProvenance = deletes.Count > 0 || linesBefore.Any(x => x.Trim() == apply);
            var cleaned = string.Join("\n", linesBefore.Where(x => x.Trim() != apply && (!hasHumanProvenance || x.Trim() != core)));
            // No recursive deletion: every input has its exact identity checked above; unrelated neighbours and directories remain.
            foreach (var path in deletes) File.Delete(path);
            if (cleaned != script.Replace("\r\n", "\n")) File.WriteAllText(gradle, cleaned, new UTF8Encoding(false));
        }
        /// <summary>Runs only after native SDK processors, against the generated Human launcher, never against Assets or Packages.</summary>
        public static void ConfigureGeneratedProject(string generatedRoot, string applicationId, string[] actualScenes)
        {
            RequireScope(applicationId, actualScenes);
            var root = Path.GetFullPath(generatedRoot);
            var gradle = Path.Combine(root, "launcher", "build.gradle");
            var manifest = Path.Combine(root, "launcher", "src", "main", "AndroidManifest.xml");
            Require(File.Exists(gradle) && File.Exists(manifest), "HUMAN_GENERATED_LAUNCHER_MISSING");
            var script = File.ReadAllText(gradle);
            var ids = Regex.Matches(script, "(?m)^\\s*applicationId\\s+[\\\"']([^\\\"']+)[\\\"']\\s*\\r?$");
            Require(ids.Count == 1 && ids[0].Groups[1].Value == ApplicationId, "HUMAN_GENERATED_APPLICATION_ID_MISMATCH");
            ConfigureLauncherManifest(manifest);
            var exportPaths = Path.Combine(root, "launcher", "src", "main", "res", "xml", "stp_human_export_paths.xml");
            Directory.CreateDirectory(Path.GetDirectoryName(exportPaths)!);
            File.WriteAllText(exportPaths, "<paths><files-path name=\"human_exports\" path=\"human-exports/\" /></paths>\n", new UTF8Encoding(false));
            ValidateExportPaths(exportPaths);
            var javaSource = Path.Combine(root, "launcher", "src", "main", "java", "com", "stp", "human", "StpHumanFileProvider.java");
            Directory.CreateDirectory(Path.GetDirectoryName(javaSource)!);
            File.WriteAllText(javaSource, "package com.stp.human;\n\n/** Generated only for the isolated ADR042 Human launcher. */\npublic final class StpHumanFileProvider extends androidx.core.content.FileProvider { }\n", new UTF8Encoding(false));
            const string core = "dependencies { implementation 'androidx.core:core:1.15.0' }";
            if (!script.Contains(core)) File.AppendAllText(gradle, "\n" + core + "\n", new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(root, "stp-human-privacy.gradle"), GradleGate, new UTF8Encoding(false));
            const string apply = "apply from: rootProject.file('stp-human-privacy.gradle')";
            if (!script.Contains(apply)) File.AppendAllText(gradle, "\n" + apply + "\n", new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(root, "stp-human-generated-scope.txt"), "ApplicationId=" + ApplicationId + "\nScene=" + ScenePath + "\nLauncherSourceSha256=" + Sha(manifest) + "\nExportPathsSha256=" + Sha(exportPaths) + "\nFileProviderJavaSha256=" + Sha(javaSource) + "\nGradleGateSha256=" + Sha(Path.Combine(root, "stp-human-privacy.gradle")) + "\nEvidence=GENERATED_HUMAN_INPUT_ONLY;merged_manifest_final_APK_and_physical_capture_required\n", new UTF8Encoding(false));
        }
        /// <summary>High-priority removal markers apply to known pinned SDK entries; unknown future entries remain and fail the closed output gate.</summary>
        public static void ConfigureLauncherManifest(string path)
        {
            var document = XDocument.Load(path);
            var root = document.Root;
            Require(root != null && root.Name == "manifest", "HUMAN_MANIFEST_INVALID");
            root!.SetAttributeValue(XNamespace.Xmlns + "tools", Tools.NamespaceName);
            var declaredPackage = (string?)root.Attribute("package");
            Require(declaredPackage == null || declaredPackage == ApplicationId, "HUMAN_MANIFEST_PACKAGE_MISMATCH");
            foreach (var permission in RemovedPermissions)
            {
                RemoveMarker(root, "uses-permission", permission);
                RemoveMarker(root, "uses-permission-sdk-23", permission);
            }
            root.Elements("queries").Remove(); root.Add(new XElement("queries", new XAttribute(Tools + "node", "removeAll")));
            var app = root.Element("application"); Require(app != null, "HUMAN_APPLICATION_MISSING");
            foreach (var name in RemovedMetadata) RemoveMarker(app!, "meta-data", name);
            foreach (var name in Removed_activity) RemoveMarker(app!, "activity", name);
            foreach (var name in Removed_provider) RemoveMarker(app!, "provider", name);
            foreach (var name in Removed_service) RemoveMarker(app!, "service", name);
            foreach (var name in Removed_receiver) RemoveMarker(app!, "receiver", name);
            foreach (var name in Removed_uses_library) RemoveMarker(app!, "uses-library", name);
            app!.Elements("provider").Where(x => (string?)x.Attribute(Android + "name") == ExportProvider).Remove();
            app.Add(new XElement("provider", new XAttribute(Android + "name", ExportProvider), new XAttribute(Android + "authorities", ExportAuthority),
                new XAttribute(Android + "exported", "false"), new XAttribute(Android + "grantUriPermissions", "true"),
                new XElement("meta-data", new XAttribute(Android + "name", "android.support.FILE_PROVIDER_PATHS"), new XAttribute(Android + "resource", ExportResource))));
            app!.SetAttributeValue(Android + "allowBackup", "false");
            app.SetAttributeValue(Android + "fullBackupContent", "false");
            var replaced = ((string?)app.Attribute(Tools + "replace") ?? string.Empty).Split(',').Select(x => x.Trim()).Where(x => x.Length > 0).Concat(new[] { "android:allowBackup", "android:fullBackupContent" }).Distinct();
            app.SetAttributeValue(Tools + "replace", string.Join(",", replaced));
            document.Save(path);
        }
        private static void RemoveMarker(XElement parent, string element, string name)
        {
            parent.Elements(element).Where(x => (string?)x.Attribute(Android + "name") == name).Remove();
            parent.Add(new XElement(element, new XAttribute(Android + "name", name), new XAttribute(Tools + "node", "remove")));
        }
        /// <summary>Closed actual merged-output check. This never repairs or strips a failed output.</summary>
        public static void ValidateMergedManifest(string path)
        {
            var root = XDocument.Load(path).Root; Require(root != null && root.Name == "manifest" && (string?)root.Attribute("package") == ApplicationId, "HUMAN_PACKAGE_INVALID");
            var apps = root!.Elements("application").ToArray(); Require(apps.Length == 1, "HUMAN_APPLICATION_COUNT_INVALID"); var app = apps[0];
            string own = ApplicationId + ".DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION";
            var permissions = root.Elements().Where(x => x.Name.LocalName.StartsWith("uses-permission", StringComparison.Ordinal)).ToArray();
            Require(permissions.All(x => x.Name == "uses-permission" && (string?)x.Attribute(Android + "name") == own), "HUMAN_PERMISSION_FORBIDDEN");
            Require(!root.Elements().Any(x => new[] { "permission-tree", "permission-group", "instrumentation", "queries" }.Contains(x.Name.LocalName)), "HUMAN_EXTERNAL_ENTRY_FORBIDDEN");
            var declared = root.Elements("permission").ToArray();
            Require(declared.Length <= 1 && declared.All(x => (string?)x.Attribute(Android + "name") == own && new[] { "signature", "0x2" }.Contains((string?)x.Attribute(Android + "protectionLevel"))), "HUMAN_CUSTOM_PERMISSION_FORBIDDEN");
            Require(permissions.Length == declared.Length, "HUMAN_LOCAL_PERMISSION_BINDING_INVALID");
            Require((string?)app.Attribute(Android + "allowBackup") == "false" && (string?)app.Attribute(Android + "fullBackupContent") == "false", "HUMAN_BACKUP_MUST_BE_OFF");
            Require(!new[] { "name", "backupAgent", "manageSpaceActivity", "process" }.Any(name => !string.IsNullOrEmpty((string?)app.Attribute(Android + name))), "HUMAN_CUSTOM_APPLICATION_FORBIDDEN");
            var factory = (string?)app.Attribute(Android + "appComponentFactory"); Require(string.IsNullOrEmpty(factory) || factory == "androidx.core.app.CoreComponentFactory", "HUMAN_COMPONENT_FACTORY_FORBIDDEN");
            Require(app.Elements().All(x => x.Name == "activity" || x.Name == "meta-data" || x.Name == "provider"), "HUMAN_COMPONENT_FORBIDDEN");
            var providers = app.Elements("provider").ToArray();
            Require(providers.Length <= 1, "HUMAN_PROVIDER_COUNT_INVALID");
            foreach (var provider in providers)
            {
                Require((string?)provider.Attribute(Android + "name") == ExportProvider && (string?)provider.Attribute(Android + "authorities") == ExportAuthority &&
                    (string?)provider.Attribute(Android + "exported") == "false" && (string?)provider.Attribute(Android + "grantUriPermissions") == "true" &&
                    provider.Attributes().All(x => new[] { Android + "name", Android + "authorities", Android + "exported", Android + "grantUriPermissions" }.Contains(x.Name)), "HUMAN_LOCAL_PROVIDER_INVALID");
                var details = provider.Elements().ToArray();
                Require(details.Length == 1 && details[0].Name == "meta-data" && (string?)details[0].Attribute(Android + "name") == "android.support.FILE_PROVIDER_PATHS" &&
                    (string?)details[0].Attribute(Android + "resource") == ExportResource && !details[0].Elements().Any() && details[0].Attributes().Count() == 2, "HUMAN_LOCAL_PROVIDER_PATH_BINDING_INVALID");
            }
            var activities = app.Elements("activity").ToArray(); Require(activities.Length == 1 && (string?)activities[0].Attribute(Android + "name") == MainActivity && (string?)activities[0].Attribute(Android + "exported") == "true", "HUMAN_MAIN_ACTIVITY_INVALID");
            var intents = activities[0].Elements("intent-filter").ToArray();
            Require(intents.Length == 1 && intents[0].Elements("action").Count() == 1 && (string?)intents[0].Element("action")?.Attribute(Android + "name") == "android.intent.action.MAIN" && intents[0].Elements("category").Count() == 1 && (string?)intents[0].Element("category")?.Attribute(Android + "name") == "android.intent.category.LAUNCHER" && !intents[0].Elements("data").Any(), "HUMAN_LAUNCH_INTENT_INVALID");
            var metadata = app.Elements("meta-data").Select(x => (string?)x.Attribute(Android + "name")).ToArray();
            Require(metadata.Distinct().Count() == metadata.Length && metadata.All(name => name != null && (name.StartsWith("unity.", StringComparison.Ordinal) || name == "notch.config")), "HUMAN_PROVIDER_METADATA_FORBIDDEN");
        }
        /// <summary>The one local provider may expose only the dedicated internal files child, never a broad/root/external path.</summary>
        public static void ValidateExportPaths(string path)
        {
            var root = XDocument.Load(path).Root;
            Require(root != null && root.Name == "paths" && !root.Attributes().Any(), "HUMAN_EXPORT_PATHS_INVALID");
            var children = root!.Elements().ToArray();
            Require(children.Length == 1 && children[0].Name == "files-path" && (string?)children[0].Attribute("name") == "human_exports" &&
                (string?)children[0].Attribute("path") == "human-exports/" && children[0].Attributes().Count() == 2 && !children[0].Elements().Any(), "HUMAN_EXPORT_PATHS_INVALID");
        }
        private static void Require(bool condition, string code) { if (!condition) throw new BuildFailedException(code); }
        private static string Sha(string path) { using var sha = SHA256.Create(); return string.Concat(sha.ComputeHash(File.ReadAllBytes(path)).Select(x => x.ToString("x2", System.Globalization.CultureInfo.InvariantCulture))); }
        private const string GradleGate = @"// Generated Human variant only. No source-package edits. Always-run validation precedes APK packaging.
plugins.withId('com.android.application') {
    gradle.projectsEvaluated {
        ['Debug', 'Release'].each { variant ->
            def packaging = tasks.findByName('package' + variant)
            if (packaging != null) {
                def main = tasks.findByName('process' + variant + 'MainManifest')
                def packaged = tasks.findByName('process' + variant + 'ManifestForPackage')
                if (main == null || packaged == null) throw new GradleException('HUMAN_MANIFEST_TASKS_MISSING:' + variant)
                def check = tasks.register('stpHumanCheck' + variant + 'Manifest') {
                    dependsOn(main, packaged)
                    outputs.upToDateWhen { false }
                    doLast {
                        [main, packaged].each { sourceTask ->
                            def manifests = [] as Set
                            sourceTask.outputs.files.files.each { output ->
                                if (output.isDirectory()) manifests.addAll(fileTree(output).matching { include '**/AndroidManifest.xml' }.files)
                                else if (output.name == 'AndroidManifest.xml') manifests.add(output)
                            }
                            if (manifests.isEmpty()) throw new GradleException('HUMAN_FINAL_MANIFEST_MISSING:' + sourceTask.name)
                            manifests.each { manifest ->
                                def x = new groovy.xml.XmlSlurper(false, false).parse(manifest)
                                def fail = { String code -> throw new GradleException(code + ':' + manifest) }
                                def app = x.application[0]
                                def own = 'com.STP.StammstreckenPuzzle.human.DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION'
                                if (x.@package.text() != 'com.STP.StammstreckenPuzzle.human' || app.size() != 1) fail('HUMAN_PACKAGE_OR_APPLICATION_INVALID')
                                if (x.children().any { it.name().startsWith('uses-permission') && (it.name() != 'uses-permission' || it.attributes()['android:name'] != own) }) fail('HUMAN_PERMISSION_FORBIDDEN')
                                if (x.children().any { it.name() in ['permission-tree', 'permission-group', 'instrumentation'] }) fail('HUMAN_EXTERNAL_ENTRY_FORBIDDEN')
                                def declared = x.permission
                                if (declared.size() > 1 || declared.any { it.attributes()['android:name'] != own || !(it.attributes()['android:protectionLevel'] in ['signature', '0x2']) }) fail('HUMAN_CUSTOM_PERMISSION_FORBIDDEN')
                                def used = x.'uses-permission'.findAll { it.attributes()['android:name'] == own }
                                if (used.size() > 1 || used.size() != declared.size()) fail('HUMAN_LOCAL_PERMISSION_BINDING_INVALID')
                                if (x.queries.size() != 0) fail('HUMAN_QUERIES_FORBIDDEN')
                                if (app.attributes()['android:allowBackup'] != 'false' || app.attributes()['android:fullBackupContent'] != 'false') fail('HUMAN_BACKUP_MUST_BE_OFF')
                                if (['android:name', 'android:backupAgent', 'android:manageSpaceActivity', 'android:process'].any { app.attributes()[it] }) fail('HUMAN_CUSTOM_APPLICATION_FORBIDDEN')
                                if (app.attributes()['android:appComponentFactory'] && app.attributes()['android:appComponentFactory'] != 'androidx.core.app.CoreComponentFactory') fail('HUMAN_COMPONENT_FACTORY_FORBIDDEN')
                                if (app.children().any { !(it.name() in ['activity', 'meta-data', 'provider']) }) fail('HUMAN_COMPONENT_FORBIDDEN')
                                if (app.provider.size() > 1) fail('HUMAN_PROVIDER_COUNT_INVALID')
                                app.provider.each { provider ->
                                    def a = provider.attributes()
                                    if (a['android:name'] != 'com.stp.human.StpHumanFileProvider' || a['android:authorities'] != 'com.STP.StammstreckenPuzzle.human.humanexports' || a['android:exported'] != 'false' || a['android:grantUriPermissions'] != 'true' || a.keySet() != ['android:name', 'android:authorities', 'android:exported', 'android:grantUriPermissions'].toSet()) fail('HUMAN_LOCAL_PROVIDER_INVALID')
                                    def details = provider.children()
                                    if (details.size() != 1 || details[0].name() != 'meta-data' || details[0].attributes()['android:name'] != 'android.support.FILE_PROVIDER_PATHS' || details[0].attributes()['android:resource'] != '@xml/stp_human_export_paths' || details[0].attributes().size() != 2 || details[0].children().size() != 0) fail('HUMAN_LOCAL_PROVIDER_PATH_BINDING_INVALID')
                                    def pathsFile = rootProject.file('launcher/src/main/res/xml/stp_human_export_paths.xml')
                                    if (!pathsFile.isFile()) fail('HUMAN_EXPORT_PATHS_MISSING')
                                    def paths = new groovy.xml.XmlSlurper(false, false).parse(pathsFile)
                                    def children = paths.children()
                                    if (paths.name() != 'paths' || paths.attributes().size() != 0 || children.size() != 1 || children[0].name() != 'files-path' || children[0].attributes()['name'] != 'human_exports' || children[0].attributes()['path'] != 'human-exports/' || children[0].attributes().size() != 2 || children[0].children().size() != 0) fail('HUMAN_EXPORT_PATHS_INVALID')
                                }
                                if (app.activity.size() != 1 || app.activity[0].attributes()['android:name'] != 'com.unity3d.player.UnityPlayerGameActivity' || app.activity[0].attributes()['android:exported'] != 'true') fail('HUMAN_MAIN_ACTIVITY_INVALID')
                                def intents = app.activity[0].'intent-filter'
                                if (intents.size() != 1 || intents.action.size() != 1 || intents.action[0].attributes()['android:name'] != 'android.intent.action.MAIN' || intents.category.size() != 1 || intents.category[0].attributes()['android:name'] != 'android.intent.category.LAUNCHER' || intents.data.size() != 0) fail('HUMAN_LAUNCH_INTENT_INVALID')
                                def names = app.'meta-data'.collect { it.attributes()['android:name'] }
                                if (names.size() != names.toSet().size() || names.any { !(it && (it.startsWith('unity.') || it == 'notch.config')) }) fail('HUMAN_PROVIDER_METADATA_FORBIDDEN')
                                def digest = java.security.MessageDigest.getInstance('SHA-256').digest(manifest.bytes).encodeHex().toString()
                                def report = new File(buildDir, 'reports/stp-human/' + sourceTask.name + '-' + digest + '.txt')
                                report.parentFile.mkdirs()
                                report.text = 'HumanManifestSha256=' + digest + '\nNetworkBillingAdsPermissions=absent\nForeignComponents=absent\nEvidence=ACTUAL_MERGED_MANIFEST_ONLY;final_APK_and_physical_capture_still_required\n'
                            }
                        }
                    }
                }
                packaging.dependsOn(check)
                def bundle = tasks.findByName('bundle' + variant)
                if (bundle != null) bundle.doFirst { throw new GradleException('HUMAN_AAB_NOT_SUPPORTED_USE_APK') }
            }
        }
    }
}
";
    }
}
