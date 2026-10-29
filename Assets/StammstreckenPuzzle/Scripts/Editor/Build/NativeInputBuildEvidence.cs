#nullable enable
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#if UNITY_ANDROID
using UnityEditor.Android;
#endif

namespace STP.Editor.Build
{
    /// <summary>Private prototype. Requires explicit build-entrypoint integration; grants no release authority.</summary>
    public sealed class NativeInputBuildEvidence
#if UNITY_ANDROID
        : IPostGenerateGradleAndroidProject
#endif
    {
        private const string EnvironmentNonce="STP_NATIVE_BUILD_NONCE";
        private const string HookFile="stp-native-build-inputs.gradle";
#if UNITY_ANDROID
        private static Context? active;
#endif
        private sealed class Context
        {
            internal string Repository=string.Empty,Nonce=string.Empty,Variant=string.Empty,Kind=string.Empty;
            internal string EvidenceRoot=string.Empty,SourceWitness=string.Empty,RequestHash=string.Empty,HookHash=string.Empty;
            internal string GeneratedRoot=string.Empty,ExpectedOutput=string.Empty;
        }
        [Serializable] private sealed class Request
        {
            public int schemaVersion=1;
            public string nonce=string.Empty,variant=string.Empty,artifactKind=string.Empty,sourceWitness=string.Empty,evidenceRoot=string.Empty;
        }
        [Serializable] private sealed class FileEvidence
        {
            public string path=string.Empty,file=string.Empty,sha256=string.Empty;
            public long sizeBytes=0;
        }
        [Serializable] private sealed class Before
        {
            public string nonce=string.Empty,invocationId=string.Empty,requestRawSha256=string.Empty;
            public FileEvidence[] generatedBuildInputs=Array.Empty<FileEvidence>();
        }
        [Serializable] private sealed class After
        {
            public int schemaVersion=0;
            public string nonce=string.Empty,invocationId=string.Empty,variant=string.Empty,artifactKind=string.Empty;
            public string sourceWitness=string.Empty,requestRawSha256=string.Empty,phase=string.Empty,status=string.Empty;
            public string gradleVersion=string.Empty,agpVersion=string.Empty;
            public bool currentPackageExecuted=false,runtimeArtifactsUnchanged=false,packagingTaskInputsUnchanged=false;
            public bool terminalGradleAndUnitySuccessVerified=false;
            public FileEvidence beforeOriginal=new FileEvidence(),packagingTaskInputsOriginal=new FileEvidence();
            public FileEvidence[] nativeOutputs=Array.Empty<FileEvidence>();
        }
        [Serializable] private sealed class Terminal
        {
            public int schemaVersion=1;
            public string nonce=string.Empty,invocationId=string.Empty,sourceWitness=string.Empty,variant=string.Empty,artifactKind=string.Empty;
            public string afterOriginalRawSha256=string.Empty,actualUnityOutput=string.Empty,actualUnityOutputSha256=string.Empty;
            public string unityBuildGuid=string.Empty,verifiedAtUtc=string.Empty,hookRawSha256=string.Empty;
            public bool unityBuildSucceeded=true,gradlePackagingReportAndActualOutputBound=true;
            public bool independentCleanSourceDeviceLegalOrStoreAcceptance=false;
        }
        /// <summary>Future Android entrypoints call this instead of BuildPipeline.BuildPlayer, within the same source-approved scope.</summary>
        public static BuildReport BuildPlayerWithEvidence(BuildPlayerOptions options,string callerDeclaredCheckout)
        {
#if !UNITY_ANDROID
            throw new BuildFailedException("NATIVE_INPUTS_ANDROID_EDITOR_TARGET_REQUIRED");
#else
            if(options.target!=BuildTarget.Android||EditorUserBuildSettings.exportAsGoogleAndroidProject)
                throw new BuildFailedException("NATIVE_INPUTS_ACTUAL_ANDROID_PACKAGING_REQUIRED");
            if(active!=null)throw new BuildFailedException("NATIVE_INPUTS_BUILD_ALREADY_ACTIVE");
            if(!Regex.IsMatch(callerDeclaredCheckout??string.Empty,"^[a-f0-9]{40}$"))
                throw new BuildFailedException("NATIVE_INPUTS_DECLARED_CHECKOUT_REQUIRED");
            string repository=Path.GetFullPath(Path.Combine(UnityEngine.Application.dataPath,".."));
            string output=Path.GetFullPath(options.locationPathName),extension=Path.GetExtension(output).ToLowerInvariant();
            if(extension!=".apk"&&extension!=".aab")throw new BuildFailedException("NATIVE_INPUTS_APK_OR_AAB_REQUIRED");
            var context=new Context{Repository=repository,Nonce=Guid.NewGuid().ToString("N"),
                Variant=(options.options&BuildOptions.Development)!=0?"debug":"release",Kind=extension.Substring(1),
                EvidenceRoot=Path.Combine(repository,"Builds","NativeInputEvidence"),ExpectedOutput=output,
                SourceWitness="CALLER_DECLARED_CHECKOUT:"+callerDeclaredCheckout};
            if(Directory.Exists(Path.Combine(context.EvidenceRoot,context.Nonce)))throw new BuildFailedException("NATIVE_INPUTS_FRESH_NONCE_REQUIRED");
            string? previous=Environment.GetEnvironmentVariable(EnvironmentNonce);active=context;
            Environment.SetEnvironmentVariable(EnvironmentNonce,context.Nonce);
            try
            {
                var report=BuildPipeline.BuildPlayer(options);
                if(report==null||report.summary.result!=BuildResult.Succeeded)throw new BuildFailedException("NATIVE_INPUTS_PLAYER_BUILD_FAILED");
                VerifyAndPreserveTerminal(context,report);
                return report;
            }
            finally {active=null;Environment.SetEnvironmentVariable(EnvironmentNonce,previous);}
#endif
        }
#if UNITY_ANDROID
        /// <summary>Other own generated-project privacy hooks may still run afterwards; their final bytes are captured by Gradle.</summary>
        public int callbackOrder=>int.MaxValue-1;
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var context=active??throw new BuildFailedException("NATIVE_INPUTS_EXPLICIT_BUILD_SCOPE_REQUIRED");
            string root=Directory.GetParent(Path.GetFullPath(path))?.FullName??throw new BuildFailedException("NATIVE_INPUTS_GENERATED_ROOT_REQUIRED");
            string launcher=Path.Combine(root,"launcher","build.gradle");
            if(!File.Exists(launcher))throw new BuildFailedException("NATIVE_INPUTS_LAUNCHER_REQUIRED");
            string original=Path.Combine(context.Repository,"tools","release",HookFile);
            byte[] bytes=File.ReadAllBytes(original);context.HookHash=RawHash(bytes);
            File.WriteAllBytes(Path.Combine(root,HookFile),bytes);
            var request=new Request{nonce=context.Nonce,variant=context.Variant,artifactKind=context.Kind,
                sourceWitness=context.SourceWitness,evidenceRoot=context.EvidenceRoot};
            string requestPath=Path.Combine(root,"stp-native-build-request.json");
            File.WriteAllText(requestPath,UnityEngine.JsonUtility.ToJson(request,true)+"\n",new UTF8Encoding(false));
            context.RequestHash=RawHash(File.ReadAllBytes(requestPath));context.GeneratedRoot=root;
            const string apply="apply from: rootProject.file('stp-native-build-inputs.gradle')";
            string text=File.ReadAllText(launcher);int count=text.Split(new[]{apply},StringSplitOptions.None).Length-1;
            if(count>1)throw new BuildFailedException("NATIVE_INPUTS_DUPLICATE_HOOK");
            if(count==0)File.AppendAllText(launcher,"\n"+apply+"\n",new UTF8Encoding(false));
        }
#endif
        private static void VerifyAndPreserveTerminal(Context context,BuildReport report)
        {
            if(context.GeneratedRoot.Length==0||context.RequestHash.Length==0)throw new BuildFailedException("NATIVE_INPUTS_GENERATION_RECEIPT_REQUIRED");
            string directory=Path.Combine(context.EvidenceRoot,context.Nonce),afterPath=Path.Combine(directory,"inputs-after.json");
            byte[] afterRaw=ReadBounded(afterPath);var after=UnityEngine.JsonUtility.FromJson<After>(Encoding.UTF8.GetString(afterRaw));
            if(after==null||after.schemaVersion!=1||after.nonce!=context.Nonce||after.variant!=context.Variant||after.artifactKind!=context.Kind||
                after.sourceWitness!=context.SourceWitness||after.requestRawSha256!=context.RequestHash||
                after.phase!="AFTER_NATIVE_PACKAGING"||after.status!="PACKAGING_ATTESTED_TERMINAL_BUILD_SUCCESS_STILL_REQUIRED"||
                !after.currentPackageExecuted||!after.runtimeArtifactsUnchanged||!after.packagingTaskInputsUnchanged||
                after.terminalGradleAndUnitySuccessVerified||after.gradleVersion!="9.1.0"||after.agpVersion!="9.0.0"||
                !Guid.TryParse(after.invocationId,out _))throw new BuildFailedException("NATIVE_INPUTS_AFTER_IDENTITY_OR_STATE_MISMATCH");
            byte[] beforeRaw=VerifyOriginal(directory,after.beforeOriginal,"inputs-before.json");
            VerifyOriginal(directory,after.packagingTaskInputsOriginal,"packaging-task-inputs-before.json");
            var before=UnityEngine.JsonUtility.FromJson<Before>(Encoding.UTF8.GetString(beforeRaw));
            if(before==null||before.nonce!=context.Nonce||before.invocationId!=after.invocationId||before.requestRawSha256!=context.RequestHash)
                throw new BuildFailedException("NATIVE_INPUTS_BEFORE_IDENTITY_MISMATCH");
            string hook=Path.Combine(context.GeneratedRoot,HookFile);
            if(RawHash(File.ReadAllBytes(hook))!=context.HookHash||before.generatedBuildInputs.Count(x=>
                SamePath(x.path,hook)&&x.sha256==context.HookHash)!=1)throw new BuildFailedException("NATIVE_INPUTS_HOOK_SOURCE_MISMATCH");
            string finalHash=RawHash(File.ReadAllBytes(context.ExpectedOutput));
            if(after.nativeOutputs.Length==0||after.nativeOutputs.Count(x=>x.sha256==finalHash&&x.sizeBytes==new FileInfo(context.ExpectedOutput).Length)!=1)
                throw new BuildFailedException("NATIVE_INPUTS_FINAL_PLAYER_OUTPUT_MISMATCH");
            var terminal=new Terminal{nonce=context.Nonce,invocationId=after.invocationId,sourceWitness=context.SourceWitness,
                variant=context.Variant,artifactKind=context.Kind,afterOriginalRawSha256=RawHash(afterRaw),
                actualUnityOutput=context.ExpectedOutput,actualUnityOutputSha256=finalHash,unityBuildGuid=report.summary.guid.ToString(),
                hookRawSha256=context.HookHash,verifiedAtUtc=DateTime.UtcNow.ToString("O",CultureInfo.InvariantCulture)};
            byte[] terminalBytes=new UTF8Encoding(false).GetBytes(UnityEngine.JsonUtility.ToJson(terminal,true)+"\n");
            using var stream=new FileStream(Path.Combine(directory,"terminal-unity-build.json"),FileMode.CreateNew,FileAccess.Write,FileShare.None);
            stream.Write(terminalBytes,0,terminalBytes.Length);
        }
        private static byte[] VerifyOriginal(string directory,FileEvidence record,string requiredName)
        {
            if(record==null||record.file!=requiredName||!Regex.IsMatch(record.sha256??string.Empty,"^[a-f0-9]{64}$"))
                throw new BuildFailedException("NATIVE_INPUTS_ORIGINAL_NAME_OR_HASH_MISMATCH");
            byte[] bytes=ReadBounded(Path.Combine(directory,requiredName));
            if(bytes.LongLength!=record.sizeBytes||RawHash(bytes)!=record.sha256)throw new BuildFailedException("NATIVE_INPUTS_ORIGINAL_BYTES_CHANGED");
            return bytes;
        }
        private static byte[] ReadBounded(string path)
        {var info=new FileInfo(path);if(!info.Exists||info.Length<=0||info.Length>67108864)throw new BuildFailedException("NATIVE_INPUTS_ORIGINAL_SIZE_INVALID");return File.ReadAllBytes(path);}
        private static bool SamePath(string first,string second)=>string.Equals(Path.GetFullPath(first),Path.GetFullPath(second),
            Path.DirectorySeparatorChar=='\\'?StringComparison.OrdinalIgnoreCase:StringComparison.Ordinal);
        private static string RawHash(byte[] bytes){using var hash=SHA256.Create();return string.Concat(hash.ComputeHash(bytes).Select(b=>b.ToString("x2",CultureInfo.InvariantCulture)));}
    }
}
