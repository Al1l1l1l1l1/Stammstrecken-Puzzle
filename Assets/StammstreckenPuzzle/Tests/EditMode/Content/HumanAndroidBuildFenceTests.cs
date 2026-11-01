using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;

namespace STP.Tests.Content.EditMode
{
    public sealed class HumanAndroidBuildFenceTests
    {
        private const string App = "com.STP.StammstreckenPuzzle.human";
        private const string Scene = "Assets/StammstreckenPuzzle/Scenes/QA/HumanCalibration.unity";
        private static readonly XNamespace A = "http://schemas.android.com/apk/res/android";
        private static readonly XNamespace T = "http://schemas.android.com/tools";
        private static string DirectoryForCase()
        {
            var root = Environment.GetEnvironmentVariable("STP_SOURCE_ROOT") ?? Directory.GetCurrentDirectory();
            var path = Path.Combine(root, ".utmp", "human-fence-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(path); return path;
        }
        private static void Call(string method, params object[] args)
        {
            var type = Assembly.Load(Environment.GetEnvironmentVariable("STP_HUMAN_FENCE_ASSEMBLY") ?? "STP.Editor.Build").GetType("STP.Editor.Build.HumanAndroidBuildFence", true)!;
            try { type.GetMethod(method, BindingFlags.Static | BindingFlags.Public)!.Invoke(null, args); }
            catch (TargetInvocationException error) { throw error.InnerException!; }
        }
        private static XDocument Valid() => XDocument.Parse("<manifest xmlns:android='http://schemas.android.com/apk/res/android' package='" + App + "'><application android:allowBackup='false' android:fullBackupContent='false'><activity android:name='com.unity3d.player.UnityPlayerGameActivity' android:exported='true'><intent-filter><action android:name='android.intent.action.MAIN'/><category android:name='android.intent.category.LAUNCHER'/></intent-filter></activity></application></manifest>");
        private static string Save(XDocument value) { var path = Path.Combine(DirectoryForCase(), "AndroidManifest.xml"); value.Save(path); return path; }

        [Test]
        public void ScopeAndActualGradleIdMustAgreeBeforeAnyGeneratedFileChanges()
        {
            var root = DirectoryForCase(); var source = Path.Combine(root, "launcher", "src", "main"); Directory.CreateDirectory(source);
            var manifest = Path.Combine(source, "AndroidManifest.xml"); File.WriteAllText(manifest, "<manifest><application/></manifest>");
            var gradle = Path.Combine(root, "launcher", "build.gradle"); File.WriteAllText(gradle, "applicationId \"com.STP.StammstreckenPuzzle.qa\"\n");
            var before = File.ReadAllText(manifest);
            Assert.Catch(() => Call("ConfigureGeneratedProject", root, "com.STP.StammstreckenPuzzle.qa", new[] { Scene }));
            Assert.Catch(() => Call("ConfigureGeneratedProject", root, App, new[] { "Assets/StammstreckenPuzzle/Scenes/QA/MobilePreview.unity" }));
            Assert.Catch(() => Call("ConfigureGeneratedProject", root, App, new[] { Scene }));
            Assert.That(File.ReadAllText(manifest), Is.EqualTo(before)); Assert.That(File.Exists(Path.Combine(root, "stp-human-privacy.gradle")), Is.False);
        }

        [Test]
        public void GeneratedKnownRemovalMarkersAreIdempotentAndUnknownComponentsStayVisibleToTheOutputGate()
        {
            var root = DirectoryForCase(); var source = Path.Combine(root, "launcher", "src", "main"); Directory.CreateDirectory(source);
            var manifest = Path.Combine(source, "AndroidManifest.xml"); File.WriteAllText(manifest, "<manifest xmlns:android='http://schemas.android.com/apk/res/android'><application><provider android:name='example.UnknownProvider'/></application></manifest>");
            var gradle = Path.Combine(root, "launcher", "build.gradle"); File.WriteAllText(gradle, "applicationId \"" + App + "\"\n");
            Call("ConfigureGeneratedProject", root, App, new[] { Scene }); var before = File.ReadAllText(manifest);
            Call("ConfigureGeneratedProject", root, App, new[] { Scene }); Assert.That(File.ReadAllText(manifest), Is.EqualTo(before));
            var document = XDocument.Load(manifest); var app = document.Root!.Element("application")!;
            Assert.That(document.Root.Elements("uses-permission").Count(x => (string?)x.Attribute(A + "name") == "android.permission.INTERNET" && (string?)x.Attribute(T + "node") == "remove"), Is.EqualTo(1));
            Assert.That(app.Elements("provider").Count(x => (string?)x.Attribute(A + "name") == "com.google.android.gms.ads.MobileAdsInitProvider" && (string?)x.Attribute(T + "node") == "remove"), Is.EqualTo(1));
            Assert.That(app.Elements("provider").Any(x => (string?)x.Attribute(A + "name") == "example.UnknownProvider" && x.Attribute(T + "node") == null), Is.True);
            Assert.That(File.ReadAllText(gradle).Split(new[] { "apply from: rootProject.file('stp-human-privacy.gradle')" }, StringSplitOptions.None).Length - 1, Is.EqualTo(1));
            var gate = File.ReadAllText(Path.Combine(root, "stp-human-privacy.gradle"));
            Assert.That(gate, Does.Contain("outputs.upToDateWhen { false }")); Assert.That(gate, Does.Contain("packaging.dependsOn(check)"));
            var java=File.ReadAllText(Path.Combine(root,"launcher","src","main","java","com","stp","human","StpHumanFileProvider.java"));
            Assert.That(java,Is.EqualTo("package com.stp.human;\n\n/** Generated only for the isolated ADR042 Human launcher. */\npublic final class StpHumanFileProvider extends androidx.core.content.FileProvider { }\n"));
            Assert.That(File.ReadAllText(gradle).Split(new[]{"dependencies { implementation 'androidx.core:core:1.15.0' }"},StringSplitOptions.None).Length-1,Is.EqualTo(1));
            Assert.DoesNotThrow(()=>Call("ValidateExportPaths",Path.Combine(root,"launcher","src","main","res","xml","stp_human_export_paths.xml")));
        }

        [Test]
        public void ActualMergedOutputAllowsOnlyTheUnityEntryAndOptionalLocalSignaturePermission()
        {
            Assert.DoesNotThrow(() => Call("ValidateMergedManifest", Save(Valid())));
            var value = Valid(); string own = App + ".DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION";
            value.Root!.AddFirst(new XElement("permission", new XAttribute(A + "name", own), new XAttribute(A + "protectionLevel", "signature")), new XElement("uses-permission", new XAttribute(A + "name", own)));
            Assert.DoesNotThrow(() => Call("ValidateMergedManifest", Save(value)));
        }

        private static XElement LocalProvider()=>new XElement("provider", new XAttribute(A+"name","com.stp.human.StpHumanFileProvider"),new XAttribute(A+"authorities",App+".humanexports"),new XAttribute(A+"exported","false"),new XAttribute(A+"grantUriPermissions","true"),new XElement("meta-data",new XAttribute(A+"name","android.support.FILE_PROVIDER_PATHS"),new XAttribute(A+"resource","@xml/stp_human_export_paths")));
        [Test]
        public void OptionalLocalProviderAndExactInternalExportResourceAreAllowed()
        {
            var value=Valid();value.Root!.Element("application")!.Add(LocalProvider());
            Assert.DoesNotThrow(()=>Call("ValidateMergedManifest",Save(value)));
            Assert.DoesNotThrow(()=>Call("ValidateExportPaths",Save(XDocument.Parse("<paths><files-path name='human_exports' path='human-exports/'/></paths>"))));
        }
        [TestCase("authority")][TestCase("exported")][TestCase("grant")][TestCase("class")][TestCase("permission")][TestCase("resource")][TestCase("extra-child")][TestCase("duplicate")]
        public void LocalProviderCannotExpandItsAuthorityOrUriGrantContract(string mutation)
        {
            var value=Valid();var app=value.Root!.Element("application")!;var provider=LocalProvider();app.Add(provider);
            if(mutation=="authority")provider.SetAttributeValue(A+"authorities",App+".other");
            else if(mutation=="exported")provider.SetAttributeValue(A+"exported","true");
            else if(mutation=="grant")provider.SetAttributeValue(A+"grantUriPermissions","false");
            else if(mutation=="class")provider.SetAttributeValue(A+"name","example.Provider");
            else if(mutation=="permission")provider.SetAttributeValue(A+"writePermission","example.permission");
            else if(mutation=="resource")provider.Element("meta-data")!.SetAttributeValue(A+"resource","@xml/other_paths");
            else if(mutation=="extra-child")provider.Add(new XElement("grant-uri-permission",new XAttribute(A+"pathPrefix","/")));
            else app.Add(LocalProvider());
            Assert.Catch(()=>Call("ValidateMergedManifest",Save(value)));
        }
        [TestCase(".")][TestCase("/")][TestCase("../")][TestCase("human-exports/../")][TestCase("human-exports/sub/")]
        public void ExportResourceCannotExposeBroadOrDifferentPaths(string path)
        {Assert.Catch(()=>Call("ValidateExportPaths",Save(new XDocument(new XElement("paths",new XElement("files-path",new XAttribute("name","human_exports"),new XAttribute("path",path)))))));}
        [Test]
        public void ExportResourceCannotUseExternalRootOrAdditionalAliases()
        {
            foreach(var xml in new[]{"<paths><external-path name='human_exports' path='human-exports/'/></paths>","<paths><root-path name='human_exports' path='human-exports/'/></paths>","<paths><files-path name='human_exports' path='human-exports/'/><files-path name='other' path='.'/></paths>"})
                Assert.Catch(()=>Call("ValidateExportPaths",Save(XDocument.Parse(xml))));
        }

        [TestCase("network")]
        [TestCase("billing")]
        [TestCase("sdk23-local")]
        [TestCase("provider")]
        [TestCase("service")]
        [TestCase("receiver")]
        [TestCase("activity")]
        [TestCase("queries")]
        [TestCase("backup")]
        [TestCase("sdk-metadata")]
        [TestCase("wrong-package")]
        [TestCase("custom-application")]
        [TestCase("instrumentation")]
        public void UnknownOrProviderOutputCannotBeSilentlyStrippedAndApproved(string mutation)
        {
            var value = Valid(); var root = value.Root!; var app = root.Element("application")!;
            if (mutation == "network" || mutation == "billing") root.Add(new XElement("uses-permission", new XAttribute(A + "name", mutation == "network" ? "android.permission.INTERNET" : "com.android.vending.BILLING")));
            else if (mutation == "sdk23-local") root.Add(new XElement("uses-permission-sdk-23", new XAttribute(A + "name", App + ".DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION")));
            else if (new[] { "provider", "service", "receiver", "activity" }.Contains(mutation)) app.Add(new XElement(mutation, new XAttribute(A + "name", "example.UnreviewedComponent")));
            else if (mutation == "queries") root.Add(new XElement("queries"));
            else if (mutation == "backup") app.SetAttributeValue(A + "allowBackup", "true");
            else if (mutation == "sdk-metadata") app.Add(new XElement("meta-data", new XAttribute(A + "name", "com.google.android.gms.ads.APPLICATION_ID"), new XAttribute(A + "value", "unexpected")));
            else if (mutation == "wrong-package") root.SetAttributeValue("package", "com.STP.StammstreckenPuzzle.qa");
            else if (mutation == "custom-application") app.SetAttributeValue(A + "name", "example.ForeignApplication");
            else if (mutation == "instrumentation") root.Add(new XElement("instrumentation", new XAttribute(A + "name", "example.Instrumentation")));
            var path = Save(value); var before = File.ReadAllBytes(path); Assert.Catch(() => Call("ValidateMergedManifest", path)); Assert.That(File.ReadAllBytes(path), Is.EqualTo(before));
        }
    }
}
