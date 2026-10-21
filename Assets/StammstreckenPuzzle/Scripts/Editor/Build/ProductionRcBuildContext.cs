using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using STP.Bootstrap;
using UnityEditor;
using UnityEditor.Build;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace STP.Editor.Build
{
    internal sealed class ProductionRcBuildContext
    {
        internal readonly ReleaseBuildIdentity Identity;
        internal readonly byte[] OriginalIdentity;
        internal readonly string OriginalPath,OriginalSha;
        internal readonly string OutputRoot,BuildInfoPath,BuildInfoSha;
        internal readonly byte[] BuildInfo;
        private ProductionRcBuildContext(ReleaseBuildIdentity identity,byte[] raw,string path,string hash,string output,string infoPath,string infoSha,byte[] info)
        {Identity=identity;OriginalIdentity=raw;OriginalPath=path;OriginalSha=hash;OutputRoot=output;BuildInfoPath=infoPath;BuildInfoSha=infoSha;BuildInfo=info;}
        internal static ProductionRcBuildContext Read(BuildTarget target)
        {
            string Required(string key)=>Environment.GetEnvironmentVariable(key) is string v&&!string.IsNullOrWhiteSpace(v)?v:throw new BuildFailedException("RC_INPUT_REQUIRED:"+key);
            if(Required("GITHUB_ACTIONS")!="true"||Required("GITHUB_REF_TYPE")!="tag")throw new BuildFailedException("RC_PROTECTED_TAG_JOB_REQUIRED");
            string path=Required("STP_RC_IDENTITY"),hash=Required("STP_RC_IDENTITY_RAW_SHA256");
            if(!Path.IsPathRooted(path))throw new BuildFailedException("RC_IDENTITY_ABSOLUTE_PATH_REQUIRED");
            ProductionNativeBuild.RequireNoLinks(path);if(!File.Exists(path)||new FileInfo(path).Length>8192)throw new BuildFailedException("RC_IDENTITY_FILE_INVALID");
            if(!long.TryParse(Required("GITHUB_RUN_NUMBER"),out long run)||!int.TryParse(Required("GITHUB_RUN_ATTEMPT"),out int attempt))throw new BuildFailedException("RC_CI_NUMBER_INVALID");
            byte[] bytes=File.ReadAllBytes(path);var identity=ReleaseBuildIdentity.Read(bytes,hash,target==BuildTarget.Android?"ANDROID":"IOS",run,attempt);
            if(Required("GITHUB_REF_NAME")!=identity.CandidateTag||Required("GITHUB_SHA")!=identity.GitCommit||
                Required("GITHUB_WORKFLOW_SHA")!=identity.Workflow.Split('@')[1])throw new BuildFailedException("RC_CI_SOURCE_MISMATCH");
            string outputInput=Required("STP_RC_NATIVE_OUTPUT_ROOT");if(!Path.IsPathRooted(outputInput))throw new BuildFailedException("RC_OUTPUT_ABSOLUTE_PATH_REQUIRED");
            string output=Path.GetFullPath(outputInput),temp=Path.GetFullPath(Required("RUNNER_TEMP")).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;
            if(!output.StartsWith(temp,Path.DirectorySeparatorChar=='\\'?StringComparison.OrdinalIgnoreCase:StringComparison.Ordinal)||Directory.Exists(output)||File.Exists(output))throw new BuildFailedException("RC_FRESH_RUNNER_OUTPUT_REQUIRED");
            ProductionNativeBuild.RequireNoLinks(output);
            string infoPath=Required("STP_RC_BUILD_INFO"),infoSha=Required("STP_RC_BUILD_INFO_RAW_SHA256");
            if(!Path.IsPathRooted(infoPath))throw new BuildFailedException("RC_BUILD_INFO_ABSOLUTE_PATH_REQUIRED");
            ProductionNativeBuild.RequireNoLinks(infoPath);if(!File.Exists(infoPath)||new FileInfo(infoPath).Length>8192)throw new BuildFailedException("RC_BUILD_INFO_FILE_INVALID");
            byte[] info=File.ReadAllBytes(infoPath);var result=new ProductionRcBuildContext(identity,bytes,path,hash,output,infoPath,infoSha,info);result.ValidateBuildInfo(null);return result;
        }
        internal void VerifyStillCurrent()
        {ProductionNativeBuild.RequireNoLinks(OriginalPath);ProductionNativeBuild.RequireNoLinks(BuildInfoPath);if(!File.ReadAllBytes(OriginalPath).SequenceEqual(OriginalIdentity)||!File.ReadAllBytes(BuildInfoPath).SequenceEqual(BuildInfo))throw new BuildFailedException("RC_IDENTITY_OR_BUILD_INFO_CHANGED");}
        private void ValidateBuildInfo(string? contentHash)
        {
            string package=Path.Combine(UnityEngine.Application.dataPath,"..","Packages","packages-lock.json");ProductionNativeBuild.RequireNoLinks(package);
            ReleaseBuildIdentity.ValidateBuildInfo(BuildInfo,BuildInfoSha,Identity,UnityEngine.Application.unityVersion,ProductionImportOutputVerifier.RawHash(File.ReadAllBytes(package)),contentHash,Environment.GetEnvironmentVariable("GITHUB_RUN_ID")??"");
        }
        internal void ValidatePreparedFacts(ProductionImportOutputFacts facts){VerifyStillCurrent();ValidateBuildInfo(facts.ReleaseLockJcsSha256);}
        internal void VerifySettings(BuildTarget target)
        {
            VerifyStillCurrent();var named=target==BuildTarget.Android?NamedBuildTarget.Android:NamedBuildTarget.iOS;
            ValidateReleaseFlags(EditorUserBuildSettings.development,EditorUserBuildSettings.allowDebugging,EditorUserBuildSettings.connectProfiler,EditorUserBuildSettings.buildWithDeepProfilingSupport);
            if(target==BuildTarget.Android&&PlayerSettings.Android.minifyRelease)throw new BuildFailedException("RC_R8_REQUIRES_APPROVED_SMOKE_AND_MAPPING");
            if(PlayerSettings.bundleVersion!=Identity.ReleaseVersion||PlayerSettings.GetApplicationIdentifier(named)!=Identity.ApplicationIdentifier||
                (target==BuildTarget.Android?PlayerSettings.Android.bundleVersionCode.ToString(System.Globalization.CultureInfo.InvariantCulture):PlayerSettings.iOS.buildNumber)!=Identity.BuildNumber.ToString(System.Globalization.CultureInfo.InvariantCulture))
                throw new BuildFailedException("RC_NATIVE_VERSION_MISMATCH");
            if(target==BuildTarget.Android&&(!EditorUserBuildSettings.buildAppBundle||EditorUserBuildSettings.exportAsGoogleAndroidProject||PlayerSettings.Android.useCustomKeystore||UnityEditor.Android.UserBuildSettings.DebugSymbols.level!=Unity.Android.Types.DebugSymbolLevel.Full||UnityEditor.Android.UserBuildSettings.DebugSymbols.format!=Unity.Android.Types.DebugSymbolFormat.Zip))
                throw new BuildFailedException("RC_ANDROID_UNSIGNED_BUNDLE_SETTINGS_REQUIRED");
            if(PlayerSettings.GetScriptingDefineSymbols(named).Split(';').Any(d=>d=="STP_TEST_CRASH"||d=="DEVELOPMENT_BUILD"||d=="STP_FAKE_PROVIDERS"))throw new BuildFailedException("RC_DEBUG_DEFINE_FORBIDDEN");
        }
        internal static void ValidateReleaseFlags(bool development,bool debugging,bool profiler,bool deep)
        {if(development||debugging||profiler||deep)throw new BuildFailedException("RC_DEBUG_FLAGS_FORBIDDEN");}
        internal static void VerifyUnsignedAab(string path)
        {
            ProductionNativeBuild.RequireNoLinks(path);
            using var zip=ZipFile.OpenRead(path);var names=zip.Entries.Select(e=>e.FullName).ToArray();
            if(names.Length==0||names.Distinct(StringComparer.Ordinal).Count()!=names.Length||!names.Contains("BundleConfig.pb")||!names.Contains("base/manifest/AndroidManifest.xml")||
                names.Any(n=>n.StartsWith("META-INF/",StringComparison.OrdinalIgnoreCase)&&(n.EndsWith(".RSA",StringComparison.OrdinalIgnoreCase)||n.EndsWith(".DSA",StringComparison.OrdinalIgnoreCase)||n.EndsWith(".EC",StringComparison.OrdinalIgnoreCase)||n.EndsWith(".SF",StringComparison.OrdinalIgnoreCase))))
                throw new BuildFailedException("RC_AAB_MISSING_OR_ALREADY_SIGNED");
        }
        internal void WriteNativeReceipt(string root,string artifact,string nativeAdMobAppId,string? androidRoot,string nonce)
        {
            VerifyStillCurrent();
            // This is a private native-build receipt. No store reference or promotion authority is generated.
            string Escape(string s)=>"\""+s.Replace("\\","\\\\").Replace("\"","\\\"")+"\"";
            string text="{\n\"nativeReceiptVersion\":1,\n\"stage\":"+Escape(Identity.Platform=="ANDROID"?"UNSIGNED_AAB":"IOS_SOURCE_EXPORT")+",\n\"identity\":"+Encoding.UTF8.GetString(OriginalIdentity)+",\n\"identityRawSha256\":"+Escape(OriginalSha)+",\n\"artifact\":"+Escape(Path.GetFullPath(artifact))+",\n\"adMobAppId\":"+Escape(nativeAdMobAppId)+",\n\"androidRoot\":"+(androidRoot==null?"null":Escape(androidRoot))+",\n\"scope\":"+Escape(nonce)+",\n\"debugFlags\":{\"developmentBuild\":false,\"profiler\":false,\"testCrash\":false,\"sandboxProviders\":false}\n}\n";
            text=text.Replace("\"nativeReceiptVersion\":1,","\"nativeReceiptVersion\":1,\n\"androidMinifyRelease\":"+(Identity.Platform=="ANDROID"?(PlayerSettings.Android.minifyRelease?"true":"false"):"null")+",");
            text=text.Replace("\"nativeReceiptVersion\":1,","\"nativeReceiptVersion\":1,\n\"buildInfoRawSha256\":"+Escape(BuildInfoSha)+",");
            var index=new StringBuilder("{\"nativeInputIndexVersion\":1,\"artifactKind\":").Append(Escape(Identity.Platform=="ANDROID"?"AAB":"IOS_SOURCE_EXPORT")).Append(",\"files\":[");
            string[] inputs;
            if(Identity.Platform=="ANDROID")
            {
                string[] symbols=Directory.GetFiles(root,"*.symbols.zip",SearchOption.TopDirectoryOnly).OrderBy(p=>p,StringComparer.Ordinal).ToArray();
                if(symbols.Length==0)throw new BuildFailedException("RC_ANDROID_DEBUGGING_SYMBOLS_REQUIRED");
                if(androidRoot==null)throw new BuildFailedException("RC_NATIVE_ORIGINAL_ROOT_REQUIRED");
                inputs=new[]{artifact}.Concat(symbols).Concat(CaptureAndroidOriginals(root,androidRoot,nonce)).OrderBy(p=>p,StringComparer.Ordinal).ToArray();
            }
            else
            {
                var files=new System.Collections.Generic.List<string>();var folders=new System.Collections.Generic.Stack<string>();folders.Push(artifact);
                while(folders.Count!=0){string folder=folders.Pop();ProductionNativeBuild.RequireNoLinks(folder);foreach(string entry in Directory.GetFileSystemEntries(folder)){ProductionNativeBuild.RequireNoLinks(entry);if(Directory.Exists(entry))folders.Push(entry);else files.Add(entry);if(files.Count>20000)throw new BuildFailedException("RC_NATIVE_INPUT_COUNT_INVALID");}}
                inputs=files.OrderBy(p=>p,StringComparer.Ordinal).ToArray();
            }
            for(int i=0;i<inputs.Length;i++)
            {
                string input=inputs[i];ProductionNativeBuild.RequireNoLinks(input);string relative=input.Substring((Identity.Platform=="ANDROID"?root:artifact).TrimEnd(Path.DirectorySeparatorChar).Length+1).Replace('\\','/');
                if(relative.Any(char.IsControl)||relative.StartsWith("/",StringComparison.Ordinal)||relative.Split('/').Any(p=>p==""||p=="."||p==".."))throw new BuildFailedException("RC_NATIVE_INPUT_PATH_INVALID");
                if(i!=0)index.Append(',');index.Append("{\"path\":").Append(Escape(relative)).Append(",\"rawSha256\":").Append(Escape(ShaFile(input))).Append('}');
            }
            byte[] indexBytes=new UTF8Encoding(false).GetBytes(index.Append("]}\n").ToString());
            using(var file=new FileStream(Path.Combine(root,"native-inputs.json"),FileMode.CreateNew,FileAccess.Write,FileShare.None))file.Write(indexBytes,0,indexBytes.Length);
            text=text.Replace("\"nativeReceiptVersion\":1,","\"nativeReceiptVersion\":1,\n\"nativeInputIndexRawSha256\":"+Escape(ProductionImportOutputVerifier.RawHash(indexBytes))+",");
            using(var info=new FileStream(Path.Combine(root,"build-info.original.json"),FileMode.CreateNew,FileAccess.Write,FileShare.None))info.Write(BuildInfo,0,BuildInfo.Length);
            using var output=new FileStream(Path.Combine(root,"native-rc-receipt.json"),FileMode.CreateNew,FileAccess.Write,FileShare.None);byte[] bytes=new UTF8Encoding(false).GetBytes(text);output.Write(bytes,0,bytes.Length);
        }
        private static string ShaFile(string path){using var hash=System.Security.Cryptography.SHA256.Create();using var stream=File.OpenRead(path);return string.Concat(hash.ComputeHash(stream).Select(b=>b.ToString("x2",System.Globalization.CultureInfo.InvariantCulture)));}
        internal static string[] CaptureAndroidOriginals(string outputRoot,string gradleRoot,string nonce)
        {
            if(!System.Text.RegularExpressions.Regex.IsMatch(nonce??"",@"\A[0-9a-f]{32}\z"))throw new BuildFailedException("RC_NATIVE_SCOPE_INVALID");
            ProductionNativeBuild.RequireNoLinks(outputRoot);ProductionNativeBuild.RequireNoLinks(gradleRoot);
            string origin=Path.GetFullPath(gradleRoot).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;
            string reports=Path.Combine(origin,"launcher","build","reports","stp-production-native",nonce);
            ProductionNativeBuild.RequireNoLinks(reports);if(!Directory.Exists(reports))throw new BuildFailedException("RC_BOTH_NATIVE_MERGE_REPORTS_REQUIRED");
            var planned=new System.Collections.Generic.Dictionary<string,string>(StringComparer.Ordinal);var hashes=new System.Collections.Generic.Dictionary<string,string>(StringComparer.Ordinal);var kinds=new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            void Add(string source)
            {
                source=Path.GetFullPath(source);ProductionNativeBuild.RequireNoLinks(source);
                if(!source.StartsWith(origin,Path.DirectorySeparatorChar=='\\'?StringComparison.OrdinalIgnoreCase:StringComparison.Ordinal)||!File.Exists(source))throw new BuildFailedException("RC_NATIVE_ORIGINAL_PATH_INVALID");
                string relative=source.Substring(origin.Length).Replace('\\','/');if(relative.Any(char.IsControl)||relative.Split('/').Any(p=>p==""||p=="."||p==".."))throw new BuildFailedException("RC_NATIVE_ORIGINAL_PATH_INVALID");
                string hash=ShaFile(source);if(hashes.TryGetValue(relative,out string previous)&&previous!=hash)throw new BuildFailedException("RC_NATIVE_ORIGINAL_CHANGED");planned[relative]=source;hashes[relative]=hash;
            }
            foreach(string report in Directory.GetFiles(reports,"*.txt",SearchOption.TopDirectoryOnly))
            {
                ProductionNativeBuild.RequireNoLinks(report);if(new FileInfo(report).Length>16384)throw new BuildFailedException("RC_NATIVE_REPORT_INVALID");
                var fields=new System.Collections.Generic.Dictionary<string,string>(StringComparer.Ordinal);
                foreach(string line in File.ReadAllLines(report,new UTF8Encoding(false,true)))
                {int split=line.IndexOf('=');if(split<=0||!fields.TryAdd(line.Substring(0,split),line.Substring(split+1)))throw new BuildFailedException("RC_NATIVE_REPORT_INVALID");}
                if(fields.Count!=6||!new[]{"scope","task","manifest","sha256","applicationId","adMobAppId"}.All(fields.ContainsKey)||fields["scope"]!=nonce||fields["task"]!="processReleaseMainManifest"&&fields["task"]!="processReleaseManifestForPackage")throw new BuildFailedException("RC_NATIVE_REPORT_INVALID");
                Add(report);Add(fields["manifest"]);if(ShaFile(fields["manifest"])!=fields["sha256"])throw new BuildFailedException("RC_NATIVE_REPORT_STALE");kinds.Add(fields["task"]);
            }
            if(kinds.Count!=2)throw new BuildFailedException("RC_BOTH_NATIVE_MERGE_REPORTS_REQUIRED");
            // Plan/validate the entire fixed original set before any copy. Raw reports keep their original paths.
            foreach(var row in planned)
            {string target=Path.Combine(outputRoot,"android-native-originals",row.Key.Replace('/',Path.DirectorySeparatorChar));ProductionNativeBuild.RequireNoLinks(target);if(File.Exists(target)||Directory.Exists(target))throw new BuildFailedException("RC_NATIVE_ORIGINAL_ALREADY_EXISTS");}
            var result=new System.Collections.Generic.List<string>();foreach(var row in planned.OrderBy(r=>r.Key,StringComparer.Ordinal))
            {string target=Path.Combine(outputRoot,"android-native-originals",row.Key.Replace('/',Path.DirectorySeparatorChar));if(ShaFile(row.Value)!=hashes[row.Key])throw new BuildFailedException("RC_NATIVE_ORIGINAL_CHANGED");Directory.CreateDirectory(Path.GetDirectoryName(target)!);File.Copy(row.Value,target,false);if(ShaFile(target)!=hashes[row.Key]||ShaFile(row.Value)!=hashes[row.Key])throw new BuildFailedException("RC_NATIVE_ORIGINAL_CHANGED");result.Add(target);}
            return result.ToArray();
        }
        internal void InjectAndroidBuildInfo(string root)
        {
            VerifyStillCurrent();CleanupGeneratedBuildInfo(root);
            string asset=Path.Combine(root,"unityLibrary","src","main","assets","build-info.json"),marker=Path.Combine(root,"stp-rc-build-info.scope");
            ProductionNativeBuild.RequireNoLinks(asset);ProductionNativeBuild.RequireNoLinks(marker);Directory.CreateDirectory(Path.GetDirectoryName(asset)!);
            string own="STP_RC_BUILD_INFO_V1\n"+BuildInfoSha+"\n"+Convert.ToBase64String(BuildInfo)+"\n";
            using(var file=new FileStream(marker,FileMode.CreateNew,FileAccess.Write,FileShare.None)){byte[] raw=new UTF8Encoding(false).GetBytes(own);file.Write(raw,0,raw.Length);}
            using(var file=new FileStream(asset,FileMode.CreateNew,FileAccess.Write,FileShare.None))file.Write(BuildInfo,0,BuildInfo.Length);
        }
        internal static void CleanupGeneratedBuildInfo(string root)
        {
            ProductionNativeBuild.RequireNoLinks(root);string asset=Path.Combine(root,"unityLibrary","src","main","assets","build-info.json"),marker=Path.Combine(root,"stp-rc-build-info.scope");
            ProductionNativeBuild.RequireNoLinks(asset);ProductionNativeBuild.RequireNoLinks(marker);
            if(Directory.Exists(asset)||Directory.Exists(marker))throw new BuildFailedException("RC_GENERATED_BUILD_INFO_UNKNOWN");
            if(!File.Exists(asset)&&!File.Exists(marker))return;
            if(!File.Exists(marker)||new FileInfo(marker).Length>32768||Directory.Exists(asset)||Directory.Exists(marker))throw new BuildFailedException("RC_GENERATED_BUILD_INFO_UNKNOWN");
            var lines=File.ReadAllText(marker,new UTF8Encoding(false,true)).Split('\n');if(lines.Length!=4||lines[0]!="STP_RC_BUILD_INFO_V1"||lines[3]!="")throw new BuildFailedException("RC_GENERATED_BUILD_INFO_UNKNOWN");
            byte[] original;try{original=Convert.FromBase64String(lines[2]);}catch(FormatException){throw new BuildFailedException("RC_GENERATED_BUILD_INFO_UNKNOWN");}
            ReleaseBuildIdentity.ValidateRetainedBuildInfo(original);
            if(ProductionImportOutputVerifier.RawHash(original)!=lines[1]||(File.Exists(asset)&&(new FileInfo(asset).Length!=original.Length||!File.ReadAllBytes(asset).SequenceEqual(original))))throw new BuildFailedException("RC_GENERATED_BUILD_INFO_UNKNOWN");
            // Exact owned marker + bound original are validated before mutation; foreign files are preserved on failure.
            if(File.Exists(asset))File.Delete(asset);File.Delete(marker);
        }
        internal void InjectIosBuildInfo(string export)
        {
#if UNITY_IOS
            VerifyStillCurrent();string path=Path.Combine(export,"STPBuildInfo","build-info.json");ProductionNativeBuild.RequireNoLinks(path);
            if(File.Exists(path)||Directory.Exists(path))throw new BuildFailedException("RC_IOS_BUILD_INFO_OUTPUT_EXISTS");Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            using(var file=new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.None))file.Write(BuildInfo,0,BuildInfo.Length);
            var project=new PBXProject();string projectPath=PBXProject.GetPBXProjectPath(export);project.ReadFromFile(projectPath);
            const string relative="STPBuildInfo/build-info.json";if(project.ContainsFileByProjectPath(relative))throw new BuildFailedException("RC_IOS_BUILD_INFO_RESOURCE_DUPLICATE");
            string guid=project.AddFile(relative,relative,PBXSourceTree.Source);project.AddFileToBuild(project.GetUnityMainTargetGuid(),guid);project.WriteToFile(projectPath);
#else
            throw new BuildFailedException("RC_IOS_HOOK_NOT_COMPILED");
#endif
        }
    }
}
