using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using STP.Bootstrap;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;
#if UNITY_ANDROID
using UnityEditor.Android;
#endif
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace STP.Editor.Build
{
    /// <summary>Additive exclusive-source/identity checks. Existing Human and privacy processors remain active.</summary>
    public sealed class ProductionNativeBuildGuard:IPreprocessBuildWithReport,IProcessSceneWithReport,IPostprocessBuildWithReport
#if UNITY_ANDROID
        ,IPostGenerateGradleAndroidProject
#endif
    {
        public int callbackOrder=>int.MaxValue-1;
        public void OnPreprocessBuild(BuildReport report)
        {
            var target=report.summary.platform;
            if(ProductionNativeBuild.HasScope){ProductionNativeBuild.RequireScope(target);return;}
            if(target!=BuildTarget.Android&&target!=BuildTarget.iOS)return;
            var named=target==BuildTarget.iOS?NamedBuildTarget.iOS:NamedBuildTarget.Android;
            // Only the existing explicitly isolated lines are allowed without this private native scope.
            ValidateUnscopedIdentity(PlayerSettings.GetApplicationIdentifier(named));
        }
        public void OnProcessScene(Scene scene,BuildReport report)
        {
            if(report==null)return;
            if(!ProductionNativeBuild.HasScope)
            {if(scene.path==ProductionGameBuild.ScenePath)throw new BuildFailedException("PRODUCTION_NATIVE_EXPLICIT_ENTRY_REQUIRED");return;}
            var scope=ProductionNativeBuild.RequireScope(report.summary.platform);
            if(scene.path!=ProductionGameBuild.ScenePath)throw new BuildFailedException("PRODUCTION_NATIVE_FOREIGN_SCENE_FORBIDDEN");
            scope.ActualSceneCount++;
        }
        public void OnPostprocessBuild(BuildReport report)
        {if(ProductionNativeBuild.HasScope)VerifyFinalOutput(report);}
        internal static void ValidateUnscopedIdentity(string id)
        {if(id!=new BootstrapConfiguration().ApplicationIdentifier&&id!=HumanAndroidBuildFence.ApplicationId)throw new BuildFailedException("PRODUCTION_NATIVE_EXPLICIT_ENTRY_REQUIRED");}

#if UNITY_ANDROID
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            string root=Directory.GetParent(Path.GetFullPath(path))?.FullName??throw new BuildFailedException("PRODUCTION_NATIVE_GRADLE_ROOT_MISSING");
            if(!ProductionNativeBuild.HasScope)
            {CleanupGeneratedNativeFiles(root,PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android));return;}
            var scope=ProductionNativeBuild.RequireScope(BuildTarget.Android);
            ProductionNativeBuild.RequireNoLinks(root);
            if(scope.AndroidRoot!=null&&scope.AndroidRoot!=root)throw new BuildFailedException("PRODUCTION_NATIVE_GRADLE_ROOT_CHANGED");
            string launcher=Path.Combine(root,"launcher","build.gradle");
            if(!File.Exists(launcher))throw new BuildFailedException("PRODUCTION_NATIVE_LAUNCHER_MISSING");
            string source=ReadGeneratedState(root);scope.AndroidRoot=root;
            string gate=Path.Combine(root,"stp-production-native.gradle");ProductionNativeBuild.RequireNoLinks(gate);
            File.WriteAllText(gate,RenderAndroidGate(scope.ApplicationId,scope.AdMobAppId,scope.Nonce),new UTF8Encoding(false));
            if(!HookMatches(source).Any())
            {string newline=source.Contains("\r\n")?"\r\n":"\n";File.WriteAllText(launcher,source+(source.EndsWith("\n",StringComparison.Ordinal)?"":newline)+NativeApply+newline,new UTF8Encoding(false));}
        }
#endif
        private const string NativeApply="apply from: rootProject.file('stp-production-native.gradle')";
        private static Match[] HookMatches(string source)=>Regex.Matches(source,@"(?m)^"+Regex.Escape(NativeApply)+@"\r?(?:\n|\z)").Cast<Match>().ToArray();
        /// <summary>Only our fully recognized generated script/hook is normalized. Unknown files stop before any mutation.</summary>
        internal static void CleanupGeneratedNativeFiles(string root,string actualNonProductionId)
        {
            ValidateUnscopedIdentity(actualNonProductionId);string source=ReadGeneratedState(root);
            string launcher=Path.Combine(root,"launcher","build.gradle"),gate=Path.Combine(root,"stp-production-native.gradle");
            var hooks=HookMatches(source);string changed=source;
            foreach(var hook in hooks.OrderByDescending(m=>m.Index))changed=changed.Remove(hook.Index,hook.Length);
            // All paths, bytes and ownership have been checked above; no other manifest/script/report is removed.
            if(changed!=source)File.WriteAllText(launcher,changed,new UTF8Encoding(false));
            if(File.Exists(gate))File.Delete(gate);
        }
        private static string ReadGeneratedState(string root)
        {
            if(!Path.IsPathRooted(root)||!Directory.Exists(root))throw new BuildFailedException("PRODUCTION_NATIVE_GRADLE_ROOT_MISSING");
            ProductionNativeBuild.RequireNoLinks(root);
            string launcher=Path.Combine(root,"launcher","build.gradle"),gate=Path.Combine(root,"stp-production-native.gradle");
            ProductionNativeBuild.RequireNoLinks(launcher);ProductionNativeBuild.RequireNoLinks(gate);
            if(Directory.Exists(gate))throw new BuildFailedException("PRODUCTION_NATIVE_GENERATED_SCRIPT_UNKNOWN");
            if(!File.Exists(launcher)||new FileInfo(launcher).Length>4194304)throw new BuildFailedException("PRODUCTION_NATIVE_LAUNCHER_MISSING");
            var utf8=new UTF8Encoding(false,true);string source=utf8.GetString(File.ReadAllBytes(launcher));var hooks=HookMatches(source);
            if(hooks.Length>1||source.Split(new[]{"stp-production-native.gradle"},StringSplitOptions.None).Length-1!=hooks.Length)
                throw new BuildFailedException("PRODUCTION_NATIVE_GENERATED_HOOK_UNKNOWN");
            if(File.Exists(gate))
            {
                if(new FileInfo(gate).Length>65536)throw new BuildFailedException("PRODUCTION_NATIVE_GENERATED_SCRIPT_UNKNOWN");
                string script=utf8.GetString(File.ReadAllBytes(gate));
                var app=Regex.Match(script,@"if\(xml\.attributes\(\)\['package'\]!='([^']+)'\)");
                var ad=Regex.Match(script,@"ids\[0\]\.attributes\(\)\['android:value'\]!='([^']+)'");
                var nonce=Regex.Match(script,@"reports/stp-production-native/([0-9a-f]{32})/");
                if(!app.Success||!ad.Success||!nonce.Success||script!=RenderAndroidGate(app.Groups[1].Value,ad.Groups[1].Value,nonce.Groups[1].Value))
                    throw new BuildFailedException("PRODUCTION_NATIVE_GENERATED_SCRIPT_UNKNOWN");
            }
            return source;
        }
        /// <summary>Rechecks the final generated merged inputs/export after all build hooks have returned.</summary>
        internal static void VerifyFinalOutput(BuildReport report)
        {
            var scope=ProductionNativeBuild.RequireScope(report.summary.platform);
            if(report.summary.platform==BuildTarget.Android)
            {
                if(scope.AndroidRoot==null)throw new BuildFailedException("PRODUCTION_NATIVE_GRADLE_HOOK_REQUIRED");
                ValidateAndroidReports(scope.AndroidRoot,scope.ApplicationId,scope.AdMobAppId,scope.Nonce);
            }
            else
            {
#if UNITY_IOS
                string export=Path.GetFullPath(report.summary.outputPath);ProductionNativeBuild.RequireNoLinks(export);
                ValidateExportedPlist(Path.Combine(export,"Info.plist"),scope.ApplicationId,scope.AdMobAppId);
                var project=new PBXProject();project.ReadFromFile(PBXProject.GetPBXProjectPath(export));
                string main=project.GetUnityMainTargetGuid();var names=project.BuildConfigNames().Distinct(StringComparer.Ordinal).ToArray();
                if(string.IsNullOrEmpty(main)||names.Length==0)throw new BuildFailedException("PRODUCTION_NATIVE_IOS_MAIN_TARGET_MISSING");
                foreach(string name in names)
                {string config=project.BuildConfigByName(main,name);if(string.IsNullOrEmpty(config)||project.GetBuildPropertyForConfig(config,"PRODUCT_BUNDLE_IDENTIFIER")!=scope.ApplicationId)throw new BuildFailedException("PRODUCTION_NATIVE_IOS_BUNDLE_CONFIGURATION_MISMATCH");}
#else
                throw new BuildFailedException("PRODUCTION_NATIVE_IOS_HOOK_NOT_COMPILED");
#endif
            }
        }

        internal static string RenderAndroidGate(string applicationId,string nativeAdMobAppId,string nonce)
        {
            if(!Regex.IsMatch(applicationId??"",@"\A[A-Za-z][A-Za-z0-9_]*(?:\.[A-Za-z][A-Za-z0-9_]*)+\z")||
                !Regex.IsMatch(nativeAdMobAppId??"",@"\Aca-app-pub-[0-9]{16}~[0-9]{10}\z")||!Regex.IsMatch(nonce??"",@"\A[0-9a-f]{32}\z"))throw new BuildFailedException("PRODUCTION_NATIVE_GATE_INPUT_INVALID");
            return AndroidGate.Replace("__STP_APP__",applicationId).Replace("__STP_ADMOB__",nativeAdMobAppId).Replace("__STP_NONCE__",nonce);
        }
        private const string AndroidGate=@"
// Generated by STP.ProductionNativeBuildGuard; exact protected scope, actual two AGP merge-output classes.
plugins.withId('com.android.application') {
 tasks.configureEach { task ->
  if(task.name=='packageDebug' || task.name=='packageRelease') {
   def variant=task.name.substring('package'.length())
   task.dependsOn('process'+variant+'MainManifest','process'+variant+'ManifestForPackage')
   task.doFirst {
    def reports=fileTree(new File(buildDir,'reports/stp-production-native/__STP_NONCE__')).matching { include '*.txt' }.files
    ['MainManifest','ManifestForPackage'].each { kind ->
     def selected=reports.findAll { it.name.startsWith('process'+variant+kind+'-') }
     if(selected.isEmpty()) throw new GradleException('PRODUCTION_NATIVE_BOTH_MERGE_OUTPUTS_REQUIRED')
     selected.each { report ->
      def fields=report.readLines().collectEntries { line -> def pair=line.split('=',2); if(pair.length!=2) throw new GradleException('PRODUCTION_NATIVE_MERGE_REPORT_INVALID'); [(pair[0]):pair[1]] }
      if(fields.scope!='__STP_NONCE__' || fields.applicationId!='__STP_APP__' || fields.adMobAppId!='__STP_ADMOB__') throw new GradleException('PRODUCTION_NATIVE_MERGE_REPORT_INVALID')
      def actual=new File(fields.manifest)
      if(!actual.isFile() || java.security.MessageDigest.getInstance('SHA-256').digest(actual.bytes).encodeHex().toString()!=fields.sha256) throw new GradleException('PRODUCTION_NATIVE_MERGE_REPORT_STALE')
     }
    }
   }
  }
  if (task.name ==~ /process.*MainManifest/ || task.name ==~ /process.*ManifestForPackage/) {
   // A fresh private scope must observe actual output even on an incremental/cache-enabled project.
   task.outputs.upToDateWhen { false }
   task.outputs.cacheIf { false }
   task.doLast {
    def manifests=[] as Set
    task.outputs.files.files.each { output ->
     if(output.isDirectory()) manifests.addAll(fileTree(output).matching { include '**/AndroidManifest.xml' }.files)
     else if(output.name=='AndroidManifest.xml') manifests.add(output)
    }
    if(manifests.isEmpty()) throw new GradleException('PRODUCTION_NATIVE_MERGED_MANIFEST_MISSING')
    manifests.each { manifest ->
     def xml=new groovy.xml.XmlSlurper(false,false).parse(manifest)
     if(xml.attributes()['package']!='__STP_APP__') throw new GradleException('PRODUCTION_NATIVE_ANDROID_PACKAGE_MISMATCH')
     def ids=xml.application.'meta-data'.findAll { it.attributes()['android:name']=='com.google.android.gms.ads.APPLICATION_ID' }
     if(ids.size()!=1 || ids[0].attributes()['android:value']!='__STP_ADMOB__') throw new GradleException('PRODUCTION_NATIVE_ANDROID_ADMOB_MISMATCH')
     def hash=java.security.MessageDigest.getInstance('SHA-256').digest(manifest.bytes).encodeHex().toString()
     def report=new File(buildDir,'reports/stp-production-native/__STP_NONCE__/'+task.name+'-'+hash+'.txt')
     report.parentFile.mkdirs()
     report.text='scope=__STP_NONCE__\ntask='+task.name+'\nmanifest='+manifest.canonicalPath+'\nsha256='+hash+'\napplicationId=__STP_APP__\nadMobAppId=__STP_ADMOB__\n'
    }
   }
  }
 }
}
";
        internal static void ValidateAndroidReports(string root,string expectedApp,string expectedAdMob,string nonce)
        {
            string reports=Path.Combine(root,"launcher","build","reports","stp-production-native",nonce);
            ProductionNativeBuild.RequireNoLinks(reports);if(!Directory.Exists(reports))throw new BuildFailedException("PRODUCTION_NATIVE_MERGE_REPORTS_REQUIRED");
            var files=Directory.GetFiles(reports,"*.txt",SearchOption.TopDirectoryOnly);if(files.Length<2||files.Length>64)throw new BuildFailedException("PRODUCTION_NATIVE_MERGE_REPORTS_REQUIRED");
            bool main=false,package=false;
            foreach(string file in files)
            {
                ProductionNativeBuild.RequireNoLinks(file);if(new FileInfo(file).Length>16384)throw new BuildFailedException("PRODUCTION_NATIVE_MERGE_REPORT_INVALID");
                var lines=File.ReadAllLines(file).Select(s=>s.Split(new[]{'='},2)).ToArray();
                if(lines.Length!=6||lines.Any(p=>p.Length!=2)||lines.Select(p=>p[0]).Distinct().Count()!=6)throw new BuildFailedException("PRODUCTION_NATIVE_MERGE_REPORT_INVALID");
                var values=lines.ToDictionary(p=>p[0],p=>p[1],StringComparer.Ordinal);
                if(!new[]{"scope","task","manifest","sha256","applicationId","adMobAppId"}.All(values.ContainsKey)||values["scope"]!=nonce||values["applicationId"]!=expectedApp||values["adMobAppId"]!=expectedAdMob)throw new BuildFailedException("PRODUCTION_NATIVE_MERGE_REPORT_INVALID");
                string manifest=Path.GetFullPath(values["manifest"]),boundary=Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;
                if(!manifest.StartsWith(boundary,Path.DirectorySeparatorChar=='\\'?StringComparison.OrdinalIgnoreCase:StringComparison.Ordinal))throw new BuildFailedException("PRODUCTION_NATIVE_MERGE_REPORT_PATH_INVALID");
                ProductionNativeBuild.RequireNoLinks(manifest);
                if(!Regex.IsMatch(values["sha256"],@"\A[0-9a-f]{64}\z")||Sha(manifest)!=values["sha256"])throw new BuildFailedException("PRODUCTION_NATIVE_MERGE_REPORT_STALE");
                ValidateMergedAndroidManifest(manifest,expectedApp,expectedAdMob);MobilePrivacyBuildGuard.ValidateMergedAndroidManifest(manifest);
                main|=Regex.IsMatch(values["task"],@"\Aprocess.*MainManifest\z");package|=Regex.IsMatch(values["task"],@"\Aprocess.*ManifestForPackage\z");
                if(!Regex.IsMatch(values["task"],@"\Aprocess.*(?:MainManifest|ManifestForPackage)\z"))throw new BuildFailedException("PRODUCTION_NATIVE_MERGE_REPORT_INVALID");
            }
            if(!main||!package)throw new BuildFailedException("PRODUCTION_NATIVE_BOTH_MERGE_OUTPUTS_REQUIRED");
        }
        internal static void ValidateMergedAndroidManifest(string path,string expectedApp,string expectedAdMob)
        {
            var xml=ReadXml(path);XNamespace android="http://schemas.android.com/apk/res/android";
            var root=xml.Root;if(root==null||root.Name!="manifest"||(string?)root.Attribute("package")!=expectedApp)throw new BuildFailedException("PRODUCTION_NATIVE_ANDROID_PACKAGE_MISMATCH");
            var apps=root.Elements("application").ToArray();var ids=apps.SelectMany(a=>a.Elements("meta-data")).Where(e=>(string?)e.Attribute(android+"name")=="com.google.android.gms.ads.APPLICATION_ID").ToArray();
            if(apps.Length!=1||ids.Length!=1||(string?)ids[0].Attribute(android+"value")!=expectedAdMob)throw new BuildFailedException("PRODUCTION_NATIVE_ANDROID_ADMOB_MISMATCH");
        }
        internal static void ValidateExportedPlist(string path,string expectedApp,string expectedAdMob)
        {
            var xml=ReadXml(path);var dict=xml.Root?.Elements("dict").SingleOrDefault();if(dict==null)throw new BuildFailedException("PRODUCTION_NATIVE_IOS_PLIST_INVALID");
            string Value(string key)
            {var keys=dict.Elements("key").Where(k=>k.Value==key).ToArray();var next=keys.Length==1?keys[0].ElementsAfterSelf().FirstOrDefault():null;if(next==null||next.Name!="string")throw new BuildFailedException("PRODUCTION_NATIVE_IOS_PLIST_INVALID");return next.Value;}
            if(Value("GADApplicationIdentifier")!=expectedAdMob)throw new BuildFailedException("PRODUCTION_NATIVE_IOS_ADMOB_MISMATCH");
            string id=Value("CFBundleIdentifier");if(id!=expectedApp&&id!="${PRODUCT_BUNDLE_IDENTIFIER}"&&id!="$(PRODUCT_BUNDLE_IDENTIFIER)")throw new BuildFailedException("PRODUCTION_NATIVE_IOS_BUNDLE_MISMATCH");
        }
        private static XDocument ReadXml(string path)
        {
            ProductionNativeBuild.RequireNoLinks(path);if(!File.Exists(path)||new FileInfo(path).Length>4194304)throw new BuildFailedException("PRODUCTION_NATIVE_XML_MISSING_OR_LARGE");
            using var reader=XmlReader.Create(path,new XmlReaderSettings{DtdProcessing=DtdProcessing.Ignore,XmlResolver=null,MaxCharactersInDocument=4194304});return XDocument.Load(reader);
        }
        private static string Sha(string path){using var hash=SHA256.Create();using var input=File.OpenRead(path);return string.Concat(hash.ComputeHash(input).Select(b=>b.ToString("x2",System.Globalization.CultureInfo.InvariantCulture)));}
    }
}
