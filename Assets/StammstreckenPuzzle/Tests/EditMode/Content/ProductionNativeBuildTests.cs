using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace STP.Tests.Content.EditMode
{
    /// <summary>Controlled native-output grammar and failure probes only. Positives use official test SDK IDs,
    /// never an approved Production source/preset, a real signing key, Unity BuildPipeline, AGP or Xcode.</summary>
    public sealed class ProductionNativeBuildTests
    {
        private const string App="contract.stp.qa",AdMob="ca-app-pub-3940256099942544~3347511713",Nonce="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        private static Type Entry=>Assembly.Load("STP.Editor.Build").GetType("STP.Editor.Build.ProductionNativeBuild",true)!;
        private static Type Guard=>Assembly.Load("STP.Editor.Build").GetType("STP.Editor.Build.ProductionNativeBuildGuard",true)!;
        private static Type Target=>Assembly.Load("UnityEditor.CoreModule").GetType("UnityEditor.BuildTarget",true)!;
        private static object Platform(string name)=>Enum.Parse(Target,name);
        private static object? Call(Type type,string method,params object[] args)
        {try{return type.GetMethod(method,BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic)!.Invoke(null,args);}catch(TargetInvocationException error){throw error.InnerException!;}}
        private static void Reject(string code,Action action){var error=Assert.Catch(()=>action());Assert.That(error!.Message,Does.Contain(code));}
        private sealed class Temp:IDisposable
        {
            internal readonly string Root;private readonly string boundary;
            internal Temp(){boundary=Path.GetFullPath(Path.Combine(Environment.GetEnvironmentVariable("STP_TEST_TEMP_ROOT")??Path.GetTempPath(),"stp-native-grammar"));Root=Path.Combine(boundary,Guid.NewGuid().ToString("N"));Assert.That(Directory.Exists(Root),Is.False);Directory.CreateDirectory(Root);}
            internal string Write(string path,string text){string full=Path.Combine(Root,path.Replace('/',Path.DirectorySeparatorChar));Directory.CreateDirectory(Path.GetDirectoryName(full)!);File.WriteAllText(full,text,new UTF8Encoding(false));return full;}
            public void Dispose(){string full=Path.GetFullPath(Root);Assert.That(full.StartsWith(boundary+Path.DirectorySeparatorChar,StringComparison.Ordinal),Is.True);if(Directory.Exists(full))Directory.Delete(full,true);}
        }
        private static string Manifest(string package=App,string id=AdMob)=>"<manifest xmlns:android=\"http://schemas.android.com/apk/res/android\" package=\""+package+"\"><application><meta-data android:name=\"com.google.android.gms.ads.APPLICATION_ID\" android:value=\""+id+"\"/><meta-data android:name=\"com.google.android.gms.ads.DELAY_APP_MEASUREMENT_INIT\" android:value=\"true\"/></application></manifest>";
        private static string Plist(string bundle="${PRODUCT_BUNDLE_IDENTIFIER}",string id=AdMob)=>"<?xml version=\"1.0\"?><!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\"><plist version=\"1.0\"><dict><key>CFBundleIdentifier</key><string>"+bundle+"</string><key>GADApplicationIdentifier</key><string>"+id+"</string></dict></plist>";
        [Test]
        public void ExactControlledFactsDoNotOpenThePrivateNativeAuthorityScope()
        {
            string scene=(string)Assembly.Load("STP.Editor.Build").GetType("STP.Editor.Build.ProductionGameBuild",true)!.GetField("ScenePath")!.GetRawConstantValue()!;
            Assert.DoesNotThrow(()=>Call(Entry,"ValidateScopeFacts",Platform("Android"),Platform("Android"),App,App,new[]{scene}));
            Assert.That((bool)Entry.GetProperty("HasScope",BindingFlags.Static|BindingFlags.NonPublic)!.GetValue(null)!,Is.False);
            Reject("PRODUCTION_NATIVE_EXPLICIT_ENTRY_REQUIRED",()=>Call(Entry,"RequireScope",Platform("Android")));
        }
        [TestCase("iOS")][TestCase("StandaloneWindows64")]
        public void DifferentOrNonMobileTargetCannotUseCapturedScopeFacts(string target)
            =>Reject("PRODUCTION_NATIVE_SCOPE_PROFILE_MISMATCH",()=>Call(Entry,"ValidateScopeFacts",Platform("Android"),Platform(target),App,App,new[]{"Assets/StammstreckenPuzzle/Scenes/ProductionGame.unity"}));
        [Test]
        public void ChangedIdentityAndExtraOrQaScenesAreRejected()
        {
            const string scene="Assets/StammstreckenPuzzle/Scenes/ProductionGame.unity";
            Reject("PRODUCTION_NATIVE_SCOPE_PROFILE_MISMATCH",()=>Call(Entry,"ValidateScopeFacts",Platform("Android"),Platform("Android"),App,"other.stp.qa",new[]{scene}));
            foreach(var scenes in new[]{Array.Empty<string>(),new[]{scene,scene},new[]{"Assets/StammstreckenPuzzle/Scenes/QA/MobilePreview.unity"}})
                Reject("PRODUCTION_NATIVE_EXCLUSIVE_SCENE_REQUIRED",()=>Call(Entry,"ValidateScopeFacts",Platform("Android"),Platform("Android"),App,App,scenes));
        }
        [TestCase("")][TestCase("fixture.production.blocked")][TestCase("other.stp.human")]
        public void UnscopedUnknownIdentitiesNeverFallBackToTheProductionScene(string id)
            =>Reject("PRODUCTION_NATIVE_EXPLICIT_ENTRY_REQUIRED",()=>Call(Guard,"ValidateUnscopedIdentity",id));
        [Test]
        public void ExistingExactQaAndHumanIdentitiesKeepTheirOwnBuildHooks()
        {
            var bootstrap=Assembly.Load("STP.Bootstrap").GetType("STP.Bootstrap.BootstrapConfiguration",true)!;var value=Activator.CreateInstance(bootstrap)!;
            string qa=(string)bootstrap.GetProperty("ApplicationIdentifier")!.GetValue(value)!;
            var human=Assembly.Load("STP.Editor.Build").GetType("STP.Editor.Build.HumanAndroidBuildFence",true)!;string id=(string)human.GetField("ApplicationId")!.GetRawConstantValue()!;
            Assert.DoesNotThrow(()=>Call(Guard,"ValidateUnscopedIdentity",qa));Assert.DoesNotThrow(()=>Call(Guard,"ValidateUnscopedIdentity",id));
        }
        [Test]
        public void ControlledGateBindsBothActualMergeClassesAndContainsNoReplacementSeedOrTestFallback()
        {
            string gate=(string)Call(Guard,"RenderAndroidGate",App,AdMob,Nonce)!;
            Assert.That(gate,Does.Contain("process.*MainManifest"));Assert.That(gate,Does.Contain("process.*ManifestForPackage"));
            Assert.That(gate,Does.Contain(App));Assert.That(gate,Does.Contain(AdMob));Assert.That(gate,Does.Contain(Nonce));Assert.That(gate,Does.Not.Contain("__STP_"));
            Assert.That(gate,Does.Contain("ids.size()!=1"));Assert.That(gate,Does.Contain("PRODUCTION_NATIVE_ANDROID_PACKAGE_MISMATCH"));
        }
        [TestCase("\n")][TestCase(" ")][TestCase("'")]
        public void RenderRejectsUnboundedPackageAdIdOrNonceSuffixes(string suffix)
        {
            Reject("PRODUCTION_NATIVE_GATE_INPUT_INVALID",()=>Call(Guard,"RenderAndroidGate",App+suffix,AdMob,Nonce));
            Reject("PRODUCTION_NATIVE_GATE_INPUT_INVALID",()=>Call(Guard,"RenderAndroidGate",App,AdMob+suffix,Nonce));
            Reject("PRODUCTION_NATIVE_GATE_INPUT_INVALID",()=>Call(Guard,"RenderAndroidGate",App,AdMob,Nonce+suffix));
        }
        [Test]
        public void ActualControlledAndroidXmlRequiresOneExactPackageAndOneExactNativeId()
        {
            using var temp=new Temp();string file=temp.Write("AndroidManifest.xml",Manifest());
            Assert.DoesNotThrow(()=>Call(Guard,"ValidateMergedAndroidManifest",file,App,AdMob));
            File.WriteAllText(file,Manifest("other.stp.qa"));Reject("PRODUCTION_NATIVE_ANDROID_PACKAGE_MISMATCH",()=>Call(Guard,"ValidateMergedAndroidManifest",file,App,AdMob));
            File.WriteAllText(file,Manifest(App,AdMob+"0"));Reject("PRODUCTION_NATIVE_ANDROID_ADMOB_MISMATCH",()=>Call(Guard,"ValidateMergedAndroidManifest",file,App,AdMob));
            File.WriteAllText(file,Manifest().Replace("</application>","<meta-data android:name=\"com.google.android.gms.ads.APPLICATION_ID\" android:value=\""+AdMob+"\"/></application>"));Reject("PRODUCTION_NATIVE_ANDROID_ADMOB_MISMATCH",()=>Call(Guard,"ValidateMergedAndroidManifest",file,App,AdMob));
            File.WriteAllText(file,Manifest().Replace("</application>","</application><application/>"));Reject("PRODUCTION_NATIVE_ANDROID_ADMOB_MISMATCH",()=>Call(Guard,"ValidateMergedAndroidManifest",file,App,AdMob));
        }
        [TestCase("${PRODUCT_BUNDLE_IDENTIFIER}")][TestCase("$(PRODUCT_BUNDLE_IDENTIFIER)")][TestCase(App)]
        public void StandardActualExportBundleMacroOrExactLiteralHasSeparatePbxResolutionRequirement(string bundle)
        {using var temp=new Temp();string file=temp.Write("Info.plist",Plist(bundle));Assert.DoesNotThrow(()=>Call(Guard,"ValidateExportedPlist",file,App,AdMob));}
        [TestCase("${OTHER_IDENTIFIER}")][TestCase("other.stp.qa")]
        public void UnknownBundleMacrosOrDifferentLiteralCannotHideIdentity(string bundle)
        {using var temp=new Temp();string file=temp.Write("Info.plist",Plist(bundle));Reject("PRODUCTION_NATIVE_IOS_BUNDLE_MISMATCH",()=>Call(Guard,"ValidateExportedPlist",file,App,AdMob));}
        [Test]
        public void ChangedOrDuplicateNativePlistIdsAreRejected()
        {
            using var temp=new Temp();string file=temp.Write("Info.plist",Plist(id:AdMob+"0"));Reject("PRODUCTION_NATIVE_IOS_ADMOB_MISMATCH",()=>Call(Guard,"ValidateExportedPlist",file,App,AdMob));
            File.WriteAllText(file,Plist().Replace("</dict>","<key>GADApplicationIdentifier</key><string>"+AdMob+"</string></dict>"));Reject("PRODUCTION_NATIVE_IOS_PLIST_INVALID",()=>Call(Guard,"ValidateExportedPlist",file,App,AdMob));
        }
        private static string Sha(string file){using var hash=SHA256.Create();return string.Concat(hash.ComputeHash(File.ReadAllBytes(file)).Select(x=>x.ToString("x2")));}
        private static void Report(Temp temp,string task,string manifest,string? hash=null)
        {temp.Write("launcher/build/reports/stp-production-native/"+Nonce+"/"+task+".txt","scope="+Nonce+"\ntask="+task+"\nmanifest="+manifest+"\nsha256="+(hash??Sha(manifest))+"\napplicationId="+App+"\nadMobAppId="+AdMob+"\n");}
        [Test]
        public void PhysicalTwoMergeReportsMustRemainExactCurrentAndPrivacyValid()
        {
            using var temp=new Temp();string first=temp.Write("launcher/build/intermediates/main/AndroidManifest.xml",Manifest()),second=temp.Write("launcher/build/intermediates/package/AndroidManifest.xml",Manifest());
            Report(temp,"processReleaseMainManifest",first);Report(temp,"processReleaseManifestForPackage",second);
            Assert.DoesNotThrow(()=>Call(Guard,"ValidateAndroidReports",temp.Root,App,AdMob,Nonce));
            File.AppendAllText(second," ");Reject("PRODUCTION_NATIVE_MERGE_REPORT_STALE",()=>Call(Guard,"ValidateAndroidReports",temp.Root,App,AdMob,Nonce));
            Report(temp,"processReleaseManifestForPackage",second);Assert.DoesNotThrow(()=>Call(Guard,"ValidateAndroidReports",temp.Root,App,AdMob,Nonce));
            File.WriteAllText(second,Manifest().Replace("</manifest>","<uses-permission android:name=\"com.google.android.gms.permission.AD_ID\"/></manifest>"));Report(temp,"processReleaseManifestForPackage",second);
            Reject("PRIVACY_ANDROID_AD_ID_PERMISSION_FORBIDDEN",()=>Call(Guard,"ValidateAndroidReports",temp.Root,App,AdMob,Nonce));
        }
        [Test]
        public void TwoMainReportsCannotStandInForMissingPackagingOutput()
        {
            using var temp=new Temp();string file=temp.Write("merged/AndroidManifest.xml",Manifest());
            Report(temp,"processDebugMainManifest",file);Report(temp,"processReleaseMainManifest",file);
            Reject("PRODUCTION_NATIVE_BOTH_MERGE_OUTPUTS_REQUIRED",()=>Call(Guard,"ValidateAndroidReports",temp.Root,App,AdMob,Nonce));
        }
        [Test]
        public void AFailureRestoringOneSettingStillRestoresEveryOtherCapturedSetting()
        {
            int applied=0;Action[] restore={()=>applied++,()=>throw new IOException("controlled restore failure"),()=>applied++};
            Reject("PRODUCTION_NATIVE_SETTINGS_RESTORE_FAILED:1",()=>Call(Entry,"RestoreAll",(object)restore));Assert.That(applied,Is.EqualTo(2));
        }
        private static string QaIdentity()
        {var type=Assembly.Load("STP.Bootstrap").GetType("STP.Bootstrap.BootstrapConfiguration",true)!;return(string)type.GetProperty("ApplicationIdentifier")!.GetValue(Activator.CreateInstance(type))!;}
        private const string Apply="apply from: rootProject.file('stp-production-native.gradle')";
        [Test]
        public void OwnedNativeGateCleanupIsIdempotentAndRetainsOtherGradleBytesAndNeighbors()
        {
            using var temp=new Temp();string original="// keep exact CRLF\r\ndependencies { implementation 'androidx.core:core:1.15.0' }\r\n";
            string launcher=temp.Write("launcher/build.gradle",original+Apply+"\r\n");
            string gate=temp.Write("stp-production-native.gradle",(string)Call(Guard,"RenderAndroidGate",App,AdMob,Nonce)!);
            string neighbor=temp.Write("stp-privacy.gradle","OTHER_OWNERS_BYTES\n");
            Call(Guard,"CleanupGeneratedNativeFiles",temp.Root,QaIdentity());Assert.That(File.ReadAllText(launcher),Is.EqualTo(original));Assert.That(File.Exists(gate),Is.False);Assert.That(File.ReadAllText(neighbor),Is.EqualTo("OTHER_OWNERS_BYTES\n"));
            Call(Guard,"CleanupGeneratedNativeFiles",temp.Root,QaIdentity());Assert.That(File.ReadAllText(launcher),Is.EqualTo(original));
        }
        [Test]
        public void CleanupNeverTouchesASeparateLegitimateCoreLineWithoutNativeProvenance()
        {
            using var temp=new Temp();byte[] original=Encoding.UTF8.GetBytes("dependencies { implementation 'androidx.core:core:1.15.0' }\r\n");string launcher=temp.Write("launcher/build.gradle","");File.WriteAllBytes(launcher,original);
            Call(Guard,"CleanupGeneratedNativeFiles",temp.Root,QaIdentity());CollectionAssert.AreEqual(original,File.ReadAllBytes(launcher));
        }
        [Test]
        public void UnknownScriptStopsBeforeRemovingTheExactOwnedHookOrChangingAnyOtherByte()
        {
            using var temp=new Temp();string launcher=temp.Write("launcher/build.gradle","// prefix\n"+Apply+"\n// suffix\n"),gate=temp.Write("stp-production-native.gradle",(string)Call(Guard,"RenderAndroidGate",App,AdMob,Nonce)!+"\n// unknown mutation\n");
            byte[] before=File.ReadAllBytes(launcher),script=File.ReadAllBytes(gate);
            Reject("PRODUCTION_NATIVE_GENERATED_SCRIPT_UNKNOWN",()=>Call(Guard,"CleanupGeneratedNativeFiles",temp.Root,QaIdentity()));
            CollectionAssert.AreEqual(before,File.ReadAllBytes(launcher));CollectionAssert.AreEqual(script,File.ReadAllBytes(gate));
            Reject("PRODUCTION_NATIVE_GENERATED_SCRIPT_UNKNOWN",()=>Call(Guard,"ReadGeneratedState",temp.Root));
        }
        [Test]
        public void UnknownHookShapeStopsBeforeDeletingEvenTheKnownGeneratedScript()
        {
            using var temp=new Temp();string launcher=temp.Write("launcher/build.gradle","apply from: 'stp-production-native.gradle'\n"),gate=temp.Write("stp-production-native.gradle",(string)Call(Guard,"RenderAndroidGate",App,AdMob,Nonce)!);
            string before=File.ReadAllText(launcher);Reject("PRODUCTION_NATIVE_GENERATED_HOOK_UNKNOWN",()=>Call(Guard,"CleanupGeneratedNativeFiles",temp.Root,QaIdentity()));Assert.That(File.Exists(gate),Is.True);Assert.That(File.ReadAllText(launcher),Is.EqualTo(before));
        }
        [Test]
        public void ArbitraryUnscopedApplicationCannotTriggerGeneratedCleanup()
        {
            using var temp=new Temp();string launcher=temp.Write("launcher/build.gradle",Apply+"\n"),gate=temp.Write("stp-production-native.gradle",(string)Call(Guard,"RenderAndroidGate",App,AdMob,Nonce)!);
            Reject("PRODUCTION_NATIVE_EXPLICIT_ENTRY_REQUIRED",()=>Call(Guard,"CleanupGeneratedNativeFiles",temp.Root,"other.stp.production"));Assert.That(File.Exists(gate),Is.True);Assert.That(File.ReadAllText(launcher),Is.EqualTo(Apply+"\n"));
        }
        [Test]
        public void KnownOrphanHookAndKnownOrphanScriptCanEachBeCleanedWithoutGuessingOtherInputs()
        {
            using var temp=new Temp();string launcher=temp.Write("launcher/build.gradle","// exact neighbor\n"+Apply+"\n");
            Call(Guard,"CleanupGeneratedNativeFiles",temp.Root,QaIdentity());Assert.That(File.ReadAllText(launcher),Is.EqualTo("// exact neighbor\n"));
            string gate=temp.Write("stp-production-native.gradle",(string)Call(Guard,"RenderAndroidGate",App,AdMob,Nonce)!);
            Call(Guard,"CleanupGeneratedNativeFiles",temp.Root,QaIdentity());Assert.That(File.Exists(gate),Is.False);Assert.That(File.ReadAllText(launcher),Is.EqualTo("// exact neighbor\n"));
        }
    }
}
