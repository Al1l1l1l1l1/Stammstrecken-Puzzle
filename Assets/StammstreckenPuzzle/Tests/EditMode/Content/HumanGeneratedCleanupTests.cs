using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace STP.Tests.Content.EditMode
{
    public sealed class HumanGeneratedCleanupTests
    {
        private const string App = "com.STP.StammstreckenPuzzle.human";
        private const string Main = "com.STP.StammstreckenPuzzle.qa";
        private const string Scene = "Assets/StammstreckenPuzzle/Scenes/QA/HumanCalibration.unity";
        private static void Call(string method,params object[] args)
        {
            var type=Assembly.Load(Environment.GetEnvironmentVariable("STP_HUMAN_FENCE_ASSEMBLY")??"STP.Editor.Build").GetType("STP.Editor.Build.HumanAndroidBuildFence",true)!;
            try{type.GetMethod(method,BindingFlags.Public|BindingFlags.Static)!.Invoke(null,args);}catch(TargetInvocationException error){throw error.InnerException!;}
        }
        private static string Fixture()
        {
            var root=Path.Combine(Environment.GetEnvironmentVariable("STP_SOURCE_ROOT")??Directory.GetCurrentDirectory(),".utmp","human-cleanup-"+Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(root,"launcher","src","main"));
            File.WriteAllText(Path.Combine(root,"launcher","src","main","AndroidManifest.xml"),"<manifest xmlns:android='http://schemas.android.com/apk/res/android'><application/></manifest>");
            File.WriteAllText(Path.Combine(root,"launcher","build.gradle"),"applicationId '"+App+"'\n");
            Call("ConfigureGeneratedProject",root,App,new[]{Scene});return root;
        }
        private static void RegeneratedMain(string root)
        {
            var gradle=Path.Combine(root,"launcher","build.gradle");File.WriteAllText(gradle,File.ReadAllText(gradle).Replace(App,Main));
            File.WriteAllText(Path.Combine(root,"launcher","src","main","AndroidManifest.xml"),"<manifest xmlns:android='http://schemas.android.com/apk/res/android'><application android:icon='@mipmap/app_icon'/></manifest>");
        }
        private static string Java(string root)=>Path.Combine(root,"launcher","src","main","java","com","stp","human","StpHumanFileProvider.java");
        private static string Report(string root)
        {
            var dir=Path.Combine(root,"launcher","build","reports","stp-human");Directory.CreateDirectory(dir);var digest=new string('a',64);var path=Path.Combine(dir,"processDebugMainManifest-"+digest+".txt");
            File.WriteAllText(path,"HumanManifestSha256="+digest+"\nNetworkBillingAdsPermissions=absent\nForeignComponents=absent\nEvidence=ACTUAL_MERGED_MANIFEST_ONLY;final_APK_and_physical_capture_still_required\n");return path;
        }
        [Test]
        public void HumanToMainRemovesOnlyOwnedInputsAndHooksAndIsIdempotent()
        {
            var root=Fixture();var report=Report(root);RegeneratedMain(root);var neighbour=Path.Combine(Path.GetDirectoryName(Java(root))!,"Other.java");File.WriteAllText(neighbour,"unknown original stays");
            var manifest=File.ReadAllBytes(Path.Combine(root,"launcher","src","main","AndroidManifest.xml"));
            Call("CleanupGeneratedHumanFiles",root,Main);
            foreach(var relative in new[]{"launcher/src/main/java/com/stp/human/StpHumanFileProvider.java","launcher/src/main/res/xml/stp_human_export_paths.xml","stp-human-privacy.gradle","stp-human-generated-scope.txt"})Assert.That(File.Exists(Path.Combine(root,relative.Replace('/',Path.DirectorySeparatorChar))),Is.False,relative);
            Assert.That(File.Exists(report),Is.False);Assert.That(File.ReadAllText(neighbour),Is.EqualTo("unknown original stays"));Assert.That(File.ReadAllBytes(Path.Combine(root,"launcher","src","main","AndroidManifest.xml")),Is.EqualTo(manifest));
            var script=File.ReadAllText(Path.Combine(root,"launcher","build.gradle"));Assert.That(script,Does.Not.Contain("stp-human"));Assert.That(script,Does.Not.Contain("androidx.core:core:1.15.0"));
            Assert.DoesNotThrow(()=>Call("CleanupGeneratedHumanFiles",root,Main));Assert.That(File.ReadAllText(Path.Combine(root,"launcher","build.gradle")),Is.EqualTo(script));
        }
        [TestCase("java")][TestCase("paths")][TestCase("gate")][TestCase("scope")][TestCase("report")][TestCase("unknown-report")][TestCase("unknown-hook")][TestCase("stale-manifest")]
        public void UnknownOrModifiedHumanArtefactStopsBeforeAnyDeletion(string mutation)
        {
            var root=Fixture();var report=Report(root);RegeneratedMain(root);
            if(mutation=="java")File.AppendAllText(Java(root),"// unknown");
            if(mutation=="paths")File.AppendAllText(Path.Combine(root,"launcher","src","main","res","xml","stp_human_export_paths.xml"),"<!-- unknown -->");
            if(mutation=="gate")File.AppendAllText(Path.Combine(root,"stp-human-privacy.gradle"),"// unknown");
            if(mutation=="scope")File.AppendAllText(Path.Combine(root,"stp-human-generated-scope.txt"),"Unknown=1\n");
            if(mutation=="report")File.AppendAllText(report,"unknown\n");
            if(mutation=="unknown-report")File.WriteAllText(Path.Combine(Path.GetDirectoryName(report)!,"Unknown.txt"),"unknown");
            if(mutation=="unknown-hook")File.AppendAllText(Path.Combine(root,"launcher","build.gradle"),"apply from: 'stp-human-unknown.gradle'\n");
            if(mutation=="stale-manifest")File.WriteAllText(Path.Combine(root,"launcher","src","main","AndroidManifest.xml"),"<manifest xmlns:android='http://schemas.android.com/apk/res/android'><application><provider android:name='com.stp.human.StpHumanFileProvider'/></application></manifest>");
            var originals=Directory.GetFiles(root,"*",SearchOption.AllDirectories).ToDictionary(p=>p,File.ReadAllBytes);
            Assert.Catch(()=>Call("CleanupGeneratedHumanFiles",root,Main));
            foreach(var original in originals)Assert.That(File.ReadAllBytes(original.Key),Is.EqualTo(original.Value),original.Key);
        }
        [Test]
        public void LegitimateIdenticalCoreDependencyWithoutHumanProvenanceRemainsByteIdentical()
        {
            var root=Fixture();RegeneratedMain(root);Call("CleanupGeneratedHumanFiles",root,Main);
            var gradle=Path.Combine(root,"launcher","build.gradle");
            File.WriteAllText(gradle,"applicationId '"+Main+"'\r\ndependencies { implementation 'androidx.core:core:1.15.0' }\r\n");
            var before=File.ReadAllBytes(gradle);Assert.DoesNotThrow(()=>Call("CleanupGeneratedHumanFiles",root,Main));Assert.That(File.ReadAllBytes(gradle),Is.EqualTo(before));
        }
        [TestCase("human")][TestCase("mismatch")][TestCase("missing-root")][TestCase("filesystem-root")]
        public void ActualNonHumanLauncherAndBoundedGeneratedRootAreRequired(string invalid)
        {
            var root=Fixture();RegeneratedMain(root);var original=File.ReadAllBytes(Java(root));
            string suppliedRoot=invalid=="missing-root"?Path.Combine(root,"not-the-generated-root"):invalid=="filesystem-root"?Path.GetPathRoot(root)!:root;
            string id=invalid=="human"?App:invalid=="mismatch"?Main+".other":Main;
            Assert.Catch(()=>Call("CleanupGeneratedHumanFiles",suppliedRoot,id));Assert.That(File.ReadAllBytes(Java(root)),Is.EqualTo(original));
        }
    }
}
