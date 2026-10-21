using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#if UNITY_ANDROID
using UnityEditor.Android;
#endif
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace STP.Editor.Build
{
    /// <summary>Native measurement delay and final generated manifest guards; never a network/consent approval.</summary>
    public sealed class MobilePrivacyBuildGuard : IPreprocessBuildWithReport, IPostprocessBuildWithReport
#if UNITY_ANDROID
        , IPostGenerateGradleAndroidProject
#endif
    {
        /// <summary>Runs after provider configuration processors.</summary>
        public int callbackOrder => int.MaxValue;
        /// <summary>Target-specific native hooks must already exist in the loaded editor assembly.</summary>
        public void OnPreprocessBuild(BuildReport report) => RequireBuildTarget(report.summary.platform);

        /// <summary>Select the mobile target before editor assembly compilation, for example with -buildTarget Android.</summary>
        public static void RequireBuildTarget(BuildTarget requested) =>
            ValidateBuildTarget(requested, EditorUserBuildSettings.activeBuildTarget, CompiledMobileTarget);

        private static BuildTarget CompiledMobileTarget
        {
            get
            {
#if UNITY_ANDROID
                return BuildTarget.Android;
#elif UNITY_IOS
                return BuildTarget.iOS;
#else
                return BuildTarget.NoTarget;
#endif
            }
        }

        internal static void ValidateBuildTarget(BuildTarget requested, BuildTarget active, BuildTarget compiled)
        {
            if (requested != BuildTarget.Android && requested != BuildTarget.iOS) return;
            if (requested != active || requested != compiled)
                throw new BuildFailedException("PRIVACY_BUILD_TARGET_NOT_PRESELECTED: requested=" + requested +
                    "; active=" + active + "; compiled=" + compiled + "; start Unity with -buildTarget " + requested);
        }

        /// <summary>Delays iOS measurement in the actual exported plist and verifies its final values.</summary>
        public void OnPostprocessBuild(BuildReport report)
        {
#if UNITY_IOS
            if (report.summary.platform != BuildTarget.iOS) return;
            var path = Path.Combine(report.summary.outputPath, "Info.plist");
            var plist = new PlistDocument(); plist.ReadFromFile(path);
            plist.root.SetBoolean("GADDelayAppMeasurementInit", true);
            plist.WriteToFile(path);
            var reread = new PlistDocument(); reread.ReadFromFile(path);
            if (!reread.root.values.TryGetValue("GADDelayAppMeasurementInit", out var delay) || !delay.AsBoolean()) throw new BuildFailedException("PRIVACY_IOS_MEASUREMENT_DELAY_REQUIRED");
            if (reread.root.values.ContainsKey("NSUserTrackingUsageDescription")) throw new BuildFailedException("PRIVACY_IOS_UNEXPECTED_ATT_CONFIGURATION");
            var verifierSource = ConfigureAppleVerification(report.summary.outputPath);
            File.WriteAllText(Path.Combine(report.summary.outputPath, "stp-native-privacy-source.txt"), "GADDelayAppMeasurementInit=true\nInfoPlistSha256=" + Sha(path) +
                "\nSwiftSourceSha256=" + Sha(verifierSource) + "\nPBXProjectSha256=" + Sha(PBXProject.GetPBXProjectPath(report.summary.outputPath)) +
                "\nEvidence=EXPORTED_SOURCE_ONLY;Xcode_compile_link_and_physical_network_capture_required\n", new UTF8Encoding(false));
#endif
        }
#if UNITY_IOS
        private static string ConfigureAppleVerification(string exportRoot)
        {
            const string filename = "StpStoreKitVerifier.swift";
            var original = Path.Combine(UnityEngine.Application.dataPath, "Plugins", "iOS", filename);
            if (!File.Exists(original)) throw new BuildFailedException("IAP_IOS_VERIFIER_SOURCE_MISSING");
            var copies = Directory.GetFiles(exportRoot, filename, SearchOption.AllDirectories);
            if (copies.Length > 1) throw new BuildFailedException("IAP_IOS_VERIFIER_SOURCE_AMBIGUOUS");
            string source;
            if (copies.Length == 1) source = copies[0];
            else
            {
                source = Path.Combine(exportRoot, "Libraries", "STP", filename);
                Directory.CreateDirectory(Path.GetDirectoryName(source)!);
                File.Copy(original, source, false);
            }
            if (Sha(source) != Sha(original)) throw new BuildFailedException("IAP_IOS_VERIFIER_SOURCE_MISMATCH");
            var projectPath = PBXProject.GetPBXProjectPath(exportRoot);
            var project = new PBXProject(); project.ReadFromFile(projectPath);
            var framework = project.GetUnityFrameworkTargetGuid(); var main = project.GetUnityMainTargetGuid();
            if (string.IsNullOrWhiteSpace(framework) || string.IsNullOrWhiteSpace(main)) throw new BuildFailedException("IAP_IOS_NATIVE_TARGET_MISSING");
            var relative = Path.GetRelativePath(exportRoot, source).Replace('\\', '/');
            var guid = project.FindFileGuidByProjectPath(relative);
            if (string.IsNullOrEmpty(guid)) guid = project.AddFile(relative, relative, PBXSourceTree.Source);
            // Exactly one compiler owner; C#'s __Internal import resolves the exported C symbol in UnityFramework.
            project.RemoveFileFromBuild(main, guid); project.RemoveFileFromBuild(framework, guid); project.AddFileToBuild(framework, guid);
            project.SetBuildProperty(framework, "SWIFT_VERSION", "5.0");
            project.SetBuildProperty(framework, "CLANG_ENABLE_MODULES", "YES");
            project.SetBuildProperty(framework, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
            project.SetBuildProperty(main, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            project.AddBuildProperty(main, "LD_RUNPATH_SEARCH_PATHS", "$(inherited)");
            project.AddBuildProperty(main, "LD_RUNPATH_SEARCH_PATHS", "/usr/lib/swift");
            project.AddFrameworkToProject(framework, "StoreKit.framework", false);
            project.WriteToFile(projectPath);
            var reread = new PBXProject(); reread.ReadFromFile(projectPath);
            if (reread.GetBuildPropertyForAnyConfig(framework, "SWIFT_VERSION") != "5.0" ||
                reread.GetBuildPropertyForAnyConfig(main, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES") != "YES" ||
                !reread.ContainsFramework(framework, "StoreKit.framework")) throw new BuildFailedException("IAP_IOS_VERIFIER_LINK_CONFIGURATION_REQUIRED");
            return source;
        }
#endif
#if UNITY_ANDROID
        /// <summary>Adds a final launcher merge gate, rather than claiming a library input is the merged app manifest.</summary>
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var root = Directory.GetParent(path)?.FullName ?? throw new BuildFailedException("PRIVACY_ANDROID_PROJECT_ROOT_MISSING");
            var launcher = Path.Combine(root, "launcher", "build.gradle");
            if (!File.Exists(launcher)) throw new BuildFailedException("PRIVACY_ANDROID_LAUNCHER_MISSING");
            ConfigureAndroidLauncherManifest(Path.Combine(root, "launcher", "src", "main", "AndroidManifest.xml"));
            File.WriteAllText(Path.Combine(root, "stp-privacy.gradle"), GradleGate, new UTF8Encoding(false));
            const string apply = "apply from: rootProject.file('stp-privacy.gradle')";
            var source = File.ReadAllText(launcher);
            if (!source.Contains(apply)) File.AppendAllText(launcher, "\n" + apply + "\n", new UTF8Encoding(false));
        }
        private const string GradleGate = @"
// Generated by STP.MobilePrivacyBuildGuard. Inspects actual app merge output before packaging.
plugins.withId('com.android.application') {
    tasks.configureEach { task ->
        if (task.name ==~ /process.*MainManifest/ || task.name ==~ /process.*ManifestForPackage/) {
            task.doLast {
                def manifests = [] as Set
                task.outputs.files.files.each { output ->
                    if (output.isDirectory()) {
                        manifests.addAll(fileTree(output).matching { include '**/AndroidManifest.xml' }.files)
                    } else if (output.name == 'AndroidManifest.xml') {
                        manifests.add(output)
                    }
                }
                if (manifests.isEmpty()) throw new GradleException('PRIVACY_FINAL_MERGED_MANIFEST_MISSING')
                manifests.each { manifest ->
                    def xml = new groovy.xml.XmlSlurper(false, false).parse(manifest)
                    def delay = xml.application.'meta-data'.find { it.attributes()['android:name'] == 'com.google.android.gms.ads.DELAY_APP_MEASUREMENT_INIT' }
                    if (delay.size() == 0 || delay.attributes()['android:value'] != 'true') throw new GradleException('PRIVACY_ANDROID_MEASUREMENT_DELAY_REQUIRED')
                    if (xml.'uses-permission'.any { it.attributes()['android:name'] == 'com.google.android.gms.permission.AD_ID' }) throw new GradleException('PRIVACY_ANDROID_AD_ID_PERMISSION_FORBIDDEN')
                    if (xml.application.provider.any { (it.attributes()['android:name'] ?: '').contains('Firebase') }) throw new GradleException('PRIVACY_ANDROID_FIREBASE_PROVIDER_FORBIDDEN')
                    def hash = java.security.MessageDigest.getInstance('SHA-256').digest(manifest.bytes).encodeHex().toString()
                    def report = new File(buildDir, 'reports/stp-privacy/' + task.name + '-' + hash + '.txt')
                    report.parentFile.mkdirs()
                    report.text = 'MergedManifestSha256=' + hash + '\nDELAY_APP_MEASUREMENT_INIT=true\nAD_ID=absent\nEvidence=MERGED_MANIFEST_ONLY;physical_network_capture_required\n'
                }
            }
        }
    }
}
";
#endif
        /// <summary>Application-main merge priority is required: a sibling library's remove marker cannot override SDK library manifests.</summary>
        public static void ConfigureAndroidLauncherManifest(string path)
        {
            if (!File.Exists(path)) throw new BuildFailedException("PRIVACY_ANDROID_LAUNCHER_MANIFEST_MISSING");
            var document = XDocument.Load(path); var root = document.Root;
            if (root == null || root.Name != "manifest") throw new BuildFailedException("PRIVACY_ANDROID_LAUNCHER_MANIFEST_INVALID");
            XNamespace android = "http://schemas.android.com/apk/res/android", tools = "http://schemas.android.com/tools";
            root.SetAttributeValue(XNamespace.Xmlns + "tools", tools.NamespaceName);
            // Remove any input duplicate, then retain one high-priority instruction for every lower SDK manifest.
            root.Elements("uses-permission").Where(item => (string?)item.Attribute(android + "name") == "com.google.android.gms.permission.AD_ID").Remove();
            root.Add(new XElement("uses-permission", new XAttribute(android + "name", "com.google.android.gms.permission.AD_ID"), new XAttribute(tools + "node", "remove")));
            var app = root.Element("application") ?? throw new BuildFailedException("PRIVACY_ANDROID_LAUNCHER_APPLICATION_MISSING");
            app.Elements("meta-data").Where(item => (string?)item.Attribute(android + "name") == "com.google.android.gms.ads.DELAY_APP_MEASUREMENT_INIT").Remove();
            app.Add(new XElement("meta-data", new XAttribute(android + "name", "com.google.android.gms.ads.DELAY_APP_MEASUREMENT_INIT"),
                new XAttribute(android + "value", "true"), new XAttribute(tools + "replace", "android:value")));
            document.Save(path);
        }
        /// <summary>Deterministic plaintext merged-manifest check, also callable on bundletool dump output.</summary>
        public static void ValidateMergedAndroidManifest(string path)
        {
            var document = XDocument.Load(path); XNamespace android = "http://schemas.android.com/apk/res/android";
            var app = document.Root?.Element("application");
            if (app == null || !app.Elements("meta-data").Any(item => (string?)item.Attribute(android + "name") == "com.google.android.gms.ads.DELAY_APP_MEASUREMENT_INIT" && (string?)item.Attribute(android + "value") == "true"))
                throw new BuildFailedException("PRIVACY_ANDROID_MEASUREMENT_DELAY_REQUIRED");
            if (document.Root!.Elements("uses-permission").Any(item => (string?)item.Attribute(android + "name") == "com.google.android.gms.permission.AD_ID")) throw new BuildFailedException("PRIVACY_ANDROID_AD_ID_PERMISSION_FORBIDDEN");
            if (app.Elements("provider").Any(item => ((string?)item.Attribute(android + "name") ?? "").IndexOf("Firebase", StringComparison.OrdinalIgnoreCase) >= 0)) throw new BuildFailedException("PRIVACY_ANDROID_FIREBASE_PROVIDER_FORBIDDEN");
        }
        private static string Sha(string path) { using var sha = SHA256.Create(); return string.Concat(sha.ComputeHash(File.ReadAllBytes(path)).Select(value => value.ToString("x2", System.Globalization.CultureInfo.InvariantCulture))); }
    }
}
