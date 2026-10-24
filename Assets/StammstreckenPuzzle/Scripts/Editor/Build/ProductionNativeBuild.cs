using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using STP.Bootstrap;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace STP.Editor.Build
{
    /// <summary>Explicit local development APK/source export only. Source approval is Root's actual importer preflight;
    /// this entry never signs a store release, uploads, obtains credentials or starts a runtime SDK.</summary>
    public static class ProductionNativeBuild
    {
        internal const string GmaSettingsPath="Assets/GoogleMobileAds/Resources/GoogleMobileAdsSettings.asset";
        private static Scope? active;
        internal enum ScopeMode{AndroidDevelopmentApk,IosSourceExport,AndroidReleaseBundle,IosReleaseSourceExport}
        internal sealed class Scope
        {
            internal readonly ProductionMobileServiceProfile Profile;
            internal readonly ProductionRcBuildContext? Release;
            internal readonly ScopeMode Mode;
            internal readonly BuildTarget Target;
            internal readonly string ApplicationId,AdMobAppId,Nonce;
            internal readonly string[] Scenes;
            internal string? AndroidRoot;
            internal int ActualSceneCount;
            internal Scope(ProductionMobileServiceProfile profile,BuildTarget target,ProductionRcBuildContext? release=null)
            {Profile=profile;Release=release;Mode=release==null?(target==BuildTarget.Android?ScopeMode.AndroidDevelopmentApk:ScopeMode.IosSourceExport):(target==BuildTarget.Android?ScopeMode.AndroidReleaseBundle:ScopeMode.IosReleaseSourceExport);Target=target;ApplicationId=profile.ApplicationId;AdMobAppId=profile.NativeAdMobAppId;Nonce=Guid.NewGuid().ToString("N");Scenes=new[]{ProductionGameBuild.ScenePath};}
        }

        [MenuItem("Stammstrecken-Puzzle/Production/Build local Android development APK")]
        public static void BuildAndroidDevelopment()=>Build(BuildTarget.Android);
        [MenuItem("Stammstrecken-Puzzle/Production/Export iOS sources")]
        public static void ExportIosSources()=>Build(BuildTarget.iOS);

        public static void BuildAndroidReleaseBundle()=>Build(BuildTarget.Android,true);
        public static void ExportIosReleaseSources()=>Build(BuildTarget.iOS,true);

        private static void Build(BuildTarget target,bool release=false)
        {
            if(active!=null)throw new BuildFailedException("PRODUCTION_NATIVE_BUILD_ALREADY_ACTIVE");
            RequireAndroidSymbolModule(target);
            // Closed, source-bound CI identity is checked before PrepareForNativeBuild mutates any assets/settings.
            var rc=release?ProductionRcBuildContext.Read(target):null;
            MobilePrivacyBuildGuard.RequireBuildTarget(target);MobileIconAssets.VerifyForActiveTarget();
            // This must complete before the first application ID, signing or GMA setting mutation.
            var profile=rc==null?ProductionGameBuild.PrepareForNativeBuild():ProductionGameBuild.PrepareForNativeBuildWithFacts(rc.ValidatePreparedFacts);
            if(profile==null||profile.IsIos!=(target==BuildTarget.iOS))throw new BuildFailedException("PRODUCTION_NATIVE_PRESET_TARGET_MISMATCH");
            ProductionGameBuild.VerifyForNativeBuild(profile);
            if(rc!=null&&rc.Identity.ApplicationIdentifier!=profile.ApplicationId)throw new BuildFailedException("RC_PRESET_APPLICATION_MISMATCH");
            var gma=new GmaSettingsOriginal();
            var named=target==BuildTarget.iOS?NamedBuildTarget.iOS:NamedBuildTarget.Android;
            string priorId=PlayerSettings.GetApplicationIdentifier(named);
            var priorBackend=PlayerSettings.GetScriptingBackend(named);var priorOrientation=PlayerSettings.defaultInterfaceOrientation;
            string priorVersion=PlayerSettings.bundleVersion,priorIosNumber=PlayerSettings.iOS.buildNumber;int priorAndroidNumber=PlayerSettings.Android.bundleVersionCode;
            bool priorDevelopment=EditorUserBuildSettings.development,priorDebugging=EditorUserBuildSettings.allowDebugging,priorProfiler=EditorUserBuildSettings.connectProfiler,priorDeep=EditorUserBuildSettings.buildWithDeepProfilingSupport;
            var restore=new List<Action>{()=>PlayerSettings.SetApplicationIdentifier(named,priorId),()=>PlayerSettings.SetScriptingBackend(named,priorBackend),()=>PlayerSettings.defaultInterfaceOrientation=priorOrientation,gma.Restore};
            restore.AddRange(new Action[]{()=>PlayerSettings.bundleVersion=priorVersion,()=>PlayerSettings.iOS.buildNumber=priorIosNumber,()=>PlayerSettings.Android.bundleVersionCode=priorAndroidNumber,
                ()=>EditorUserBuildSettings.development=priorDevelopment,()=>EditorUserBuildSettings.allowDebugging=priorDebugging,()=>EditorUserBuildSettings.connectProfiler=priorProfiler,
                ()=>EditorUserBuildSettings.buildWithDeepProfilingSupport=priorDeep});
            restore.Add(CaptureAndroidSymbolSettings(target));
            if(target==BuildTarget.Android)
            {
                bool bundle=EditorUserBuildSettings.buildAppBundle,export=EditorUserBuildSettings.exportAsGoogleAndroidProject,custom=PlayerSettings.Android.useCustomKeystore;
                var abi=PlayerSettings.Android.targetArchitectures;var min=PlayerSettings.Android.minSdkVersion;var sdk=PlayerSettings.Android.targetSdkVersion;
                restore.AddRange(new Action[]{()=>EditorUserBuildSettings.buildAppBundle=bundle,()=>EditorUserBuildSettings.exportAsGoogleAndroidProject=export,()=>PlayerSettings.Android.useCustomKeystore=custom,
                    ()=>PlayerSettings.Android.targetArchitectures=abi,()=>PlayerSettings.Android.minSdkVersion=min,()=>PlayerSettings.Android.targetSdkVersion=sdk});
            }
            else
            {
                string os=PlayerSettings.iOS.targetOSVersionString,team=PlayerSettings.iOS.appleDeveloperTeamID,provision=PlayerSettings.iOS.iOSManualProvisioningProfileID;
                bool automatic=PlayerSettings.iOS.appleEnableAutomaticSigning;var provisionType=PlayerSettings.iOS.iOSManualProvisioningProfileType;
                restore.AddRange(new Action[]{()=>PlayerSettings.iOS.targetOSVersionString=os,()=>PlayerSettings.iOS.appleDeveloperTeamID=team,()=>PlayerSettings.iOS.appleEnableAutomaticSigning=automatic,
                    ()=>PlayerSettings.iOS.iOSManualProvisioningProfileID=provision,()=>PlayerSettings.iOS.iOSManualProvisioningProfileType=provisionType});
            }
            try
            {
                PlayerSettings.SetApplicationIdentifier(named,profile.ApplicationId);PlayerSettings.SetScriptingBackend(named,ScriptingImplementation.IL2CPP);
                PlayerSettings.defaultInterfaceOrientation=UIOrientation.Portrait;
                if(rc!=null)
                {
                    PlayerSettings.bundleVersion=rc.Identity.ReleaseVersion;PlayerSettings.Android.bundleVersionCode=rc.Identity.BuildNumber;PlayerSettings.iOS.buildNumber=rc.Identity.BuildNumber.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    EditorUserBuildSettings.development=false;EditorUserBuildSettings.allowDebugging=false;EditorUserBuildSettings.connectProfiler=false;EditorUserBuildSettings.buildWithDeepProfilingSupport=false;
                }
                if(target==BuildTarget.Android)
                {
                    EditorUserBuildSettings.buildAppBundle=release;EditorUserBuildSettings.exportAsGoogleAndroidProject=false;
                    PlayerSettings.Android.useCustomKeystore=false; // Development uses debug signing. RC additionally clears release signingConfig in its guarded Gradle scope.
                    if(release)ConfigureAndroidReleaseSymbols();
                    PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
                    PlayerSettings.Android.minSdkVersion=(AndroidSdkVersions)26;PlayerSettings.Android.targetSdkVersion=(AndroidSdkVersions)36;
                }
                else
                {
                    PlayerSettings.iOS.targetOSVersionString="15.0";PlayerSettings.iOS.appleEnableAutomaticSigning=false;
                    PlayerSettings.iOS.appleDeveloperTeamID=string.Empty;PlayerSettings.iOS.iOSManualProvisioningProfileID=string.Empty;
                }
                gma.SetForTarget(profile.IsIos,profile.NativeAdMobAppId);
                active=new Scope(profile,target,rc);RequireScope(target);
                // Every attempt has a fresh output root. Never merge a prior native export or overwrite an APK.
                string root=rc!=null?rc.OutputRoot:Path.Combine("Builds",release?"ReleaseCandidate":"ProductionDevelopment",target==BuildTarget.Android?"Android":"iOS",DateTime.UtcNow.ToString("yyyyMMddTHHmmssfff",System.Globalization.CultureInfo.InvariantCulture)+"-"+active.Nonce);
                RequireNoLinks(Path.GetFullPath(root));Directory.CreateDirectory(root);
                string destination=target==BuildTarget.Android?Path.Combine(root,release?"stp-production-unsigned.aab":"stp-production-development.apk"):Path.Combine(root,"Xcode");
                var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=(string[])active.Scenes.Clone(),locationPathName=destination,target=target,
                    options=target==BuildTarget.Android&&!release?BuildOptions.Development:BuildOptions.None});
                if(report.summary.result!=BuildResult.Succeeded)throw new BuildFailedException("PRODUCTION_NATIVE_BUILD_FAILED:"+target+":"+report.summary.totalErrors);
                if(target==BuildTarget.Android&&(!File.Exists(destination)||new FileInfo(destination).Length==0))throw new BuildFailedException("PRODUCTION_NATIVE_APK_MISSING");
                if(release)ProductionRcBuildContext.ValidateReleaseFlags((report.summary.options&BuildOptions.Development)!=0,(report.summary.options&BuildOptions.AllowDebugging)!=0,(report.summary.options&BuildOptions.ConnectWithProfiler)!=0,(report.summary.options&BuildOptions.EnableDeepProfilingSupport)!=0);
                RequireScope(target);if(active.ActualSceneCount!=1)throw new BuildFailedException("PRODUCTION_NATIVE_ACTUAL_SCENE_COUNT_INVALID");
                ProductionNativeBuildGuard.VerifyFinalOutput(report);
                if(rc!=null)
                {
                    if(target==BuildTarget.iOS)rc.InjectIosBuildInfo(destination);
                    if(target==BuildTarget.Android)ProductionRcBuildContext.VerifyUnsignedAab(destination);
                    rc.WriteNativeReceipt(root,destination,profile.NativeAdMobAppId,active.AndroidRoot,active.Nonce);
                }
                File.WriteAllText(Path.Combine(root,"stp-production-native-scope.txt"),"scope="+(release?"RC_UNSIGNED_NATIVE_PACKAGING":target==BuildTarget.Android?"LOCAL_DEVELOPMENT_SIGNED_APK":"IOS_EXPORTED_SOURCE_ONLY")+"\nreleaseApproval=false\nstoreSigningOrUpload=false\n",new UTF8Encoding(false));
                UnityEngine.Debug.Log("Production local native artifact created; no signed release, device, privacy/legal, Human or store acceptance.");
            }
            finally{active=null;RestoreAll(restore);}
        }

        /// <summary>Read facts only; the active scope is private and can only be opened after actual source/preset verification.</summary>
        internal static Scope RequireScope(BuildTarget actual)
        {
            var scope=active??throw new BuildFailedException("PRODUCTION_NATIVE_EXPLICIT_ENTRY_REQUIRED");
            var named=actual==BuildTarget.iOS?NamedBuildTarget.iOS:NamedBuildTarget.Android;
            ValidateScopeFacts(scope.Target,actual,scope.ApplicationId,PlayerSettings.GetApplicationIdentifier(named),scope.Scenes);
            if((scope.Release!=null)!=(scope.Mode==ScopeMode.AndroidReleaseBundle||scope.Mode==ScopeMode.IosReleaseSourceExport)||
                (actual==BuildTarget.Android)!=(scope.Mode==ScopeMode.AndroidDevelopmentApk||scope.Mode==ScopeMode.AndroidReleaseBundle))throw new BuildFailedException("RC_SCOPE_MODE_MISMATCH");
            ProductionGameBuild.VerifyForNativeBuild(scope.Profile);
            GmaSettingsOriginal.VerifyCurrent(actual==BuildTarget.iOS,scope.AdMobAppId);
            scope.Release?.VerifySettings(actual);
            return scope;
        }
        internal static bool HasScope=>active!=null;
        internal static void RequireAndroidSymbolModule(BuildTarget target)
        {
#if !UNITY_ANDROID
            if(target==BuildTarget.Android)throw new BuildFailedException("RC_ANDROID_SYMBOL_MODULE_REQUIRED");
#endif
        }
        internal static Action CaptureAndroidSymbolSettings(BuildTarget target)
        {
            RequireAndroidSymbolModule(target);
            if(target!=BuildTarget.Android)return ()=>{};
#if UNITY_ANDROID
            var level=UnityEditor.Android.UserBuildSettings.DebugSymbols.level;var format=UnityEditor.Android.UserBuildSettings.DebugSymbols.format;
            return ()=>RestoreAll(new Action[]{()=>UnityEditor.Android.UserBuildSettings.DebugSymbols.level=level,()=>UnityEditor.Android.UserBuildSettings.DebugSymbols.format=format});
#else
            throw new BuildFailedException("RC_ANDROID_SYMBOL_MODULE_REQUIRED");
#endif
        }
        internal static void ConfigureAndroidReleaseSymbols()
        {
#if UNITY_ANDROID
            UnityEditor.Android.UserBuildSettings.DebugSymbols.level=Unity.Android.Types.DebugSymbolLevel.Full;
            UnityEditor.Android.UserBuildSettings.DebugSymbols.format=Unity.Android.Types.DebugSymbolFormat.Zip;
#else
            throw new BuildFailedException("RC_ANDROID_SYMBOL_MODULE_REQUIRED");
#endif
        }
        internal static void RequireAndroidReleaseSymbols()
        {
#if UNITY_ANDROID
            if(UnityEditor.Android.UserBuildSettings.DebugSymbols.level!=Unity.Android.Types.DebugSymbolLevel.Full||UnityEditor.Android.UserBuildSettings.DebugSymbols.format!=Unity.Android.Types.DebugSymbolFormat.Zip)
                throw new BuildFailedException("RC_ANDROID_UNSIGNED_BUNDLE_SETTINGS_REQUIRED");
#else
            throw new BuildFailedException("RC_ANDROID_SYMBOL_MODULE_REQUIRED");
#endif
        }
        internal static void ValidateScopeFacts(BuildTarget expected,BuildTarget actual,string expectedId,string actualId,string[] scenes)
        {
            if((actual!=BuildTarget.Android&&actual!=BuildTarget.iOS)||actual!=expected||!string.Equals(expectedId,actualId,StringComparison.Ordinal))throw new BuildFailedException("PRODUCTION_NATIVE_SCOPE_PROFILE_MISMATCH");
            if(scenes==null||scenes.Length!=1||scenes[0]!=ProductionGameBuild.ScenePath)throw new BuildFailedException("PRODUCTION_NATIVE_EXCLUSIVE_SCENE_REQUIRED");
        }
        internal static void RestoreAll(IEnumerable<Action> actions)
        {int failures=0;foreach(var action in actions)try{action();}catch{failures++;}if(failures!=0)throw new BuildFailedException("PRODUCTION_NATIVE_SETTINGS_RESTORE_FAILED:"+failures);}
        internal static void RequireNoLinks(string path)
        {for(string? current=Path.GetFullPath(path);current!=null;current=Path.GetDirectoryName(current))if((File.Exists(current)||Directory.Exists(current))&&(File.GetAttributes(current)&FileAttributes.ReparsePoint)!=0)throw new BuildFailedException("PRODUCTION_NATIVE_REPARSE_FORBIDDEN");}

        private sealed class GmaSettingsOriginal
        {
            private readonly byte[] bytes,meta;
            private readonly string editorJson;
            internal GmaSettingsOriginal()
            {
                RequireNoLinks(GmaSettingsPath);RequireNoLinks(GmaSettingsPath+".meta");
                if(!File.Exists(GmaSettingsPath)||!File.Exists(GmaSettingsPath+".meta"))throw new BuildFailedException("PRODUCTION_NATIVE_GMA_SETTINGS_MISSING");
                if(new FileInfo(GmaSettingsPath).Length>1048576||new FileInfo(GmaSettingsPath+".meta").Length>16384)throw new BuildFailedException("PRODUCTION_NATIVE_GMA_SETTINGS_LARGE");
                var asset=AssetDatabase.LoadMainAssetAtPath(GmaSettingsPath);
                if(asset==null||asset.GetType().FullName!="GoogleMobileAds.Editor.GoogleMobileAdsSettings"||EditorUtility.IsDirty(asset))throw new BuildFailedException("PRODUCTION_NATIVE_GMA_SETTINGS_NOT_CLEAN");
                bytes=File.ReadAllBytes(GmaSettingsPath);meta=File.ReadAllBytes(GmaSettingsPath+".meta");editorJson=EditorJsonUtility.ToJson(asset);
            }
            internal void SetForTarget(bool ios,string appId)
            {
                var asset=AssetDatabase.LoadMainAssetAtPath(GmaSettingsPath);var value=new SerializedObject(asset);
                var target=value.FindProperty(ios?"adMobIOSAppId":"adMobAndroidAppId");
                if(target==null||target.propertyType!=SerializedPropertyType.String)throw new BuildFailedException("PRODUCTION_NATIVE_GMA_SERIALIZATION_CHANGED");
                target.stringValue=appId;value.ApplyModifiedPropertiesWithoutUndo();EditorUtility.SetDirty(asset);AssetDatabase.SaveAssetIfDirty(asset);
                VerifyCurrent(ios,appId);
            }
            internal static void VerifyCurrent(bool ios,string expected)
            {
                var asset=AssetDatabase.LoadMainAssetAtPath(GmaSettingsPath);if(asset==null||asset.GetType().FullName!="GoogleMobileAds.Editor.GoogleMobileAdsSettings")throw new BuildFailedException("PRODUCTION_NATIVE_GMA_SETTINGS_MISSING");
                var value=new SerializedObject(asset);value.Update();var target=value.FindProperty(ios?"adMobIOSAppId":"adMobAndroidAppId");
                if(target==null||target.propertyType!=SerializedPropertyType.String||target.stringValue!=expected)throw new BuildFailedException("PRODUCTION_NATIVE_GMA_SETTINGS_MISMATCH");
            }
            internal void Restore()
            {
                RequireNoLinks(GmaSettingsPath);RequireNoLinks(GmaSettingsPath+".meta");
                var asset=AssetDatabase.LoadMainAssetAtPath(GmaSettingsPath);
                if(asset==null)throw new BuildFailedException("PRODUCTION_NATIVE_GMA_RESTORE_MISSING");
                // Clear any partially applied serialized edit as well as its dirty state before restoring exact bytes.
                EditorJsonUtility.FromJsonOverwrite(editorJson,asset);EditorUtility.SetDirty(asset);AssetDatabase.SaveAssetIfDirty(asset);
                File.WriteAllBytes(GmaSettingsPath,bytes);File.WriteAllBytes(GmaSettingsPath+".meta",meta);
                AssetDatabase.ImportAsset(GmaSettingsPath,ImportAssetOptions.ForceSynchronousImport|ImportAssetOptions.ForceUpdate);
                if(!File.ReadAllBytes(GmaSettingsPath).SequenceEqual(bytes)||!File.ReadAllBytes(GmaSettingsPath+".meta").SequenceEqual(meta)||
                    EditorJsonUtility.ToJson(AssetDatabase.LoadMainAssetAtPath(GmaSettingsPath))!=editorJson)throw new BuildFailedException("PRODUCTION_NATIVE_GMA_RESTORE_MISMATCH");
            }
        }
    }
}
