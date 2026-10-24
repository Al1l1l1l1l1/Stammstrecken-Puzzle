#nullable enable
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using STP.Bootstrap;

namespace STP.Tests.RuntimeComposition.EditMode
{
    public sealed class ProductionReleasePackagingTests
    {
        private const string Identity="{\"releaseVersion\":\"1.2.3\",\"candidateTag\":\"v1.2.3-rc.4\",\"candidateOrdinal\":4,\"gitCommit\":\"1111111111111111111111111111111111111111\",\"workflow\":\"release-candidate.yml@2222222222222222222222222222222222222222\",\"platform\":\"ANDROID\",\"buildNumber\":\"12003\",\"buildProfile\":\"production\",\"applicationIdentifier\":\"contract.stp.player\"}";
        private static ReleaseBuildIdentity Read(string text,string? hash=null,long run=12,int attempt=3)=>ReleaseBuildIdentity.Read(Encoding.UTF8.GetBytes(text),hash??ProductionImportOutputVerifier.RawHash(Encoding.UTF8.GetBytes(text)),"ANDROID",run,attempt);
        private static string BuildInfo=>"{\"buildInfoVersion\":1,\"releaseVersion\":\"1.2.3\",\"buildNumber\":\"12003\",\"gitCommit\":\"1111111111111111111111111111111111111111\",\"candidateTag\":\"v1.2.3-rc.4\",\"builtAtUtc\":\"2026-10-22T00:01:02Z\",\"unityVersion\":\"6000.3.23f1\",\"packageLockSha256\":\""+new string('a',64)+"\",\"contentLockSha256\":\""+new string('b',64)+"\",\"saveSchemaVersion\":3,\"levelSchemaVersion\":2,\"solverVersion\":\"solver-v2\",\"workflow\":\"release-candidate.yml@2222222222222222222222222222222222222222\",\"workflowRunId\":\"123\"}";
        [Test]public void ControlledMetadataParsesWithoutSourceApprovalNativeScopeOrSigning()
        {var value=Read(Identity);Assert.That(value.BuildNumber,Is.EqualTo(12003));Assert.That(value.ReleaseVersion,Is.EqualTo("1.2.3"));Assert.That(value.CandidateOrdinal,Is.EqualTo(4));}
        [TestCase("\"candidateOrdinal\":4","\"candidateOrdinal\":true")]
        [TestCase("\"buildProfile\":\"production\"","\"buildProfile\":\"qa\"")]
        [TestCase("contract.stp.player","contract.stp.qa")]
        [TestCase("contract.stp.player","contract.stp.human")]
        [TestCase("v1.2.3-rc.4","v1.2.3-rc.5")]
        [TestCase("1.2.3\",\"candidateTag","01.2.3\",\"candidateTag")]
        [TestCase("\"buildNumber\":\"12003\"","\"buildNumber\":\"12004\"")]
        [TestCase("\"buildNumber\":\"12003\"","\"buildNumber\":\"2100000001\"")]
        [TestCase("\"platform\":\"ANDROID\"","\"platform\":\"IOS\"")]
        public void WrongIdentityIsRejected(string from,string to)=>Assert.Throws<ArgumentException>(()=>Read(Identity.Replace(from,to)));
        [Test]public void DuplicateAndExtraFieldsAreRejected()
        {Assert.Throws<ArgumentException>(()=>Read(Identity.Replace("{","{\"releaseVersion\":\"1.2.3\",")));Assert.Throws<ArgumentException>(()=>Read(Identity.Replace("{","{\"approved\":true,")));}
        [Test]public void RawHashIsNotAFreeApprovalBoolean()=>Assert.Throws<ArgumentException>(()=>Read(Identity,new string('a',64)));
        [Test]public void ActualClosedBuildInfoMustMatchIdentityAndContentBeforeNativeMutation()
        {byte[] raw=Encoding.UTF8.GetBytes(BuildInfo);ReleaseBuildIdentity.ValidateBuildInfo(raw,ProductionImportOutputVerifier.RawHash(raw),Read(Identity),"6000.3.23f1",new string('a',64),new string('b',64),"123");Assert.Throws<ArgumentException>(()=>ReleaseBuildIdentity.ValidateBuildInfo(raw,ProductionImportOutputVerifier.RawHash(raw),Read(Identity),"6000.3.23f1",new string('a',64),new string('c',64),"123"));}
        [TestCase("2026-10-22T00:01:02Z","2026-02-31T00:01:02Z")]
        [TestCase("\"saveSchemaVersion\":3","\"saveSchemaVersion\":true")]
        [TestCase("\"buildInfoVersion\":1","\"buildInfoVersion\":2")]
        public void ClosedMetadataIsNotAnArbitraryJsonSidecar(string a,string b)=>Assert.Throws<ArgumentException>(()=>ReleaseBuildIdentity.ValidateRetainedBuildInfo(Encoding.UTF8.GetBytes(BuildInfo.Replace(a,b))));
        [TestCase(0,3)][TestCase(12,0)][TestCase(12,1000)][TestCase(2100000,1)]
        public void CiBoundsNeverWrap(long run,int attempt)=>Assert.Throws<ArgumentException>(()=>Read(Identity,null,run,attempt));
        private static object? Call(string type,string method,params object[] arguments)
        {
            var target=System.AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("STP.Editor.Build."+type)).SingleOrDefault(t=>t!=null)??Assembly.Load("STP.Editor.Build").GetType("STP.Editor.Build."+type)!;
            try{return target.GetMethod(method,BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,arguments);}catch(TargetInvocationException error){throw error.InnerException!;}
        }
        [Test]public void IosSymbolSnapshotNeedsNoAndroidModuleAndChangesNothing()
        {
            Assert.That(()=>Call("ProductionNativeBuild","RequireAndroidSymbolModule",UnityEditor.BuildTarget.iOS),Throws.Nothing);
            var restore=(Action)Call("ProductionNativeBuild","CaptureAndroidSymbolSettings",UnityEditor.BuildTarget.iOS)!;
            Assert.That(()=>restore(),Throws.Nothing);
        }
#if !UNITY_ANDROID
        [TestCase("ConfigureAndroidReleaseSymbols")][TestCase("RequireAndroidReleaseSymbols")]
        public void UnavailableAndroidSymbolModuleNeverSuppliesSuccess(string method)
        {Assert.That(()=>Call("ProductionNativeBuild",method),Throws.Exception.With.Message.EqualTo("RC_ANDROID_SYMBOL_MODULE_REQUIRED"));}
        [Test]public void UnavailableAndroidModuleStopsRequestedTargetBeforeAnyCapture()
        {
            Assert.That(()=>Call("ProductionNativeBuild","RequireAndroidSymbolModule",UnityEditor.BuildTarget.Android),Throws.Exception.With.Message.EqualTo("RC_ANDROID_SYMBOL_MODULE_REQUIRED"));
            Assert.That(()=>Call("ProductionNativeBuild","CaptureAndroidSymbolSettings",UnityEditor.BuildTarget.Android),Throws.Exception.With.Message.EqualTo("RC_ANDROID_SYMBOL_MODULE_REQUIRED"));
        }
#else
        [Test]public void CompiledAndroidPassesOnlyModuleAvailabilityWithoutStartingBuildOrTouchingSettings()
        {Assert.That(()=>Call("ProductionNativeBuild","RequireAndroidSymbolModule",UnityEditor.BuildTarget.Android),Throws.Nothing);}
#endif
        [TestCase(true,false,false,false)][TestCase(false,true,false,false)][TestCase(false,false,true,false)][TestCase(false,false,false,true)]
        public void EveryReleaseDebugFlagFails(bool a,bool b,bool c,bool d)=>Assert.That(()=>Call("ProductionRcBuildContext","ValidateReleaseFlags",a,b,c,d),Throws.Exception);
        [Test]public void RcSigningOverrideIsExplicitAndDevelopmentRendererHasNoOverride()
        {
            string rc=(string)Call("ProductionNativeBuildGuard","RenderAndroidGateForMode","contract.stp.player","ca-app-pub-1234567890123456~1234567890",new string('a',32),true)!;
            string dev=(string)Call("ProductionNativeBuildGuard","RenderAndroidGate","contract.stp.player","ca-app-pub-1234567890123456~1234567890",new string('a',32))!;
            Assert.That(rc,Does.Contain("signingConfig = null"));Assert.That(rc,Does.Contain("RC_RELEASE_SIGNING_CONFIG_MUST_BE_NULL"));Assert.That(rc,Does.Contain("packageReleaseBundle"));Assert.That(dev,Does.Not.Contain("signingConfig = null"));
        }
        [TestCase("META-INF/DEBUG.RSA")][TestCase("META-INF/UPLOAD.SF")][TestCase("META-INF/OLD.EC")]
        public void DebugOrOldSignatureIsNeverCalledUnsigned(string signature)
        {
            string fixtureBase=Path.GetFullPath(Environment.GetEnvironmentVariable("STP_TEST_TEMP_ROOT")??Path.GetTempPath());
            string root=Path.Combine(fixtureBase,"stp-rc-test-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);
            try
            {
                string aab=Path.Combine(root,"controlled.aab");using(var zip=ZipFile.Open(aab,ZipArchiveMode.Create))foreach(string n in new[]{"BundleConfig.pb","base/manifest/AndroidManifest.xml",signature})using(var stream=zip.CreateEntry(n).Open())stream.WriteByte(1);
                Assert.That(()=>Call("ProductionRcBuildContext","VerifyUnsignedAab",aab),Throws.Exception);
            }
            finally{if(Path.GetDirectoryName(Path.GetFullPath(root))==fixtureBase&&Path.GetFileName(root).StartsWith("stp-rc-test-",StringComparison.Ordinal))Directory.Delete(root,true);}
        }
        [TestCase(false)][TestCase(true)]
        public void ExactOwnedGeneratedMetadataIsRemovedAndNeighborPreserved(bool markerOnly)
        {
            using var temp=new OwnedTemp();string asset=temp.Write("unityLibrary/src/main/assets/build-info.json",BuildInfo);byte[] raw=Encoding.UTF8.GetBytes(BuildInfo);string own="STP_RC_BUILD_INFO_V1\n"+ProductionImportOutputVerifier.RawHash(raw)+"\n"+Convert.ToBase64String(raw)+"\n";
            string marker=temp.Write("stp-rc-build-info.scope",own),neighbor=temp.Write("unityLibrary/src/main/assets/other.txt","literal neighbor");if(markerOnly)File.Delete(asset);
            Call("ProductionRcBuildContext","CleanupGeneratedBuildInfo",temp.Root);Assert.That(File.Exists(asset),Is.False);Assert.That(File.Exists(marker),Is.False);Assert.That(File.ReadAllText(neighbor),Is.EqualTo("literal neighbor"));Call("ProductionRcBuildContext","CleanupGeneratedBuildInfo",temp.Root);
        }
        [TestCase("no-marker")][TestCase("changed-payload")][TestCase("foreign-marker")]
        public void UnknownGeneratedMetadataFailsBeforeAnyDelete(string change)
        {
            using var temp=new OwnedTemp();string asset=temp.Write("unityLibrary/src/main/assets/build-info.json",BuildInfo);byte[] raw=Encoding.UTF8.GetBytes(BuildInfo);
            if(change!="no-marker")temp.Write("stp-rc-build-info.scope",change=="foreign-marker"?"other owner":"STP_RC_BUILD_INFO_V1\n"+ProductionImportOutputVerifier.RawHash(raw)+"\n"+Convert.ToBase64String(raw)+"\n");
            if(change=="changed-payload")File.AppendAllText(asset," ");byte[] original=File.ReadAllBytes(asset);
            Assert.That(()=>Call("ProductionRcBuildContext","CleanupGeneratedBuildInfo",temp.Root),Throws.Exception);Assert.That(File.ReadAllBytes(asset),Is.EqualTo(original));
        }
        [TestCase("valid")][TestCase("missing-kind")][TestCase("foreign-manifest")][TestCase("stale-manifest")]
        public void NativeCaptureKeepsRawPathsAndValidatesAllBeforeAnyCopy(string mode)
        {
            using var temp=new OwnedTemp();string nonce=new string('a',32),output=Path.Combine(temp.Root,"native"),gradle=Path.Combine(temp.Root,"gradle");Directory.CreateDirectory(output);
            byte[] original=Encoding.UTF8.GetBytes("actual controlled manifest\r\n");string sha=ProductionImportOutputVerifier.RawHash(original);
            var reports=new System.Collections.Generic.List<string>();
            foreach(string task in new[]{"processReleaseMainManifest","processReleaseManifestForPackage"})
            {
                string manifest=temp.Write("gradle/launcher/build/intermediates/"+task+"/AndroidManifest.xml",Encoding.UTF8.GetString(original));
                if(mode=="foreign-manifest"&&task.EndsWith("ForPackage",StringComparison.Ordinal))manifest=temp.Write("foreign/AndroidManifest.xml",Encoding.UTF8.GetString(original));
                if(mode=="stale-manifest"&&task.EndsWith("ForPackage",StringComparison.Ordinal))File.AppendAllText(manifest,"changed");
                string report=temp.Write("gradle/launcher/build/reports/stp-production-native/"+nonce+"/"+task+"-controlled.txt","scope="+nonce+"\r\ntask="+task+"\r\nmanifest="+manifest+"\r\nsha256="+sha+"\r\napplicationId=contract.stp.player\r\nadMobAppId=ca-app-pub-1234567890123456~1234567890\r\n");reports.Add(report);
            }
            if(mode=="missing-kind")File.Delete(reports[1]);
            if(mode!="valid")
            {Assert.That(()=>Call("ProductionRcBuildContext","CaptureAndroidOriginals",output,gradle,nonce),Throws.Exception);Assert.That(Directory.GetFileSystemEntries(output),Is.Empty);return;}
            string[] copied=(string[])Call("ProductionRcBuildContext","CaptureAndroidOriginals",output,gradle,nonce)!;Assert.That(copied.Length,Is.EqualTo(4));
            foreach(string report in reports){string relative=report.Substring(gradle.Length+1);Assert.That(File.ReadAllBytes(Path.Combine(output,"android-native-originals",relative)),Is.EqualTo(File.ReadAllBytes(report)));}
            Assert.That(()=>Call("ProductionRcBuildContext","CaptureAndroidOriginals",output,gradle,nonce),Throws.Exception);
        }
        private sealed class OwnedTemp:IDisposable
        {
            private readonly string parent=Path.GetFullPath(Environment.GetEnvironmentVariable("STP_TEST_TEMP_ROOT")??Path.GetTempPath());
            internal string Root{get;}
            internal OwnedTemp(){Root=Path.Combine(parent,"stp-rc-owned-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(Root);}
            internal string Write(string relative,string text){string path=Path.Combine(Root,relative.Replace('/',Path.DirectorySeparatorChar));Directory.CreateDirectory(Path.GetDirectoryName(path)!);File.WriteAllText(path,text,new UTF8Encoding(false));return path;}
            public void Dispose(){if(Path.GetDirectoryName(Path.GetFullPath(Root))==parent&&Path.GetFileName(Root).StartsWith("stp-rc-owned-",StringComparison.Ordinal))Directory.Delete(Root,true);}
        }
    }
}
