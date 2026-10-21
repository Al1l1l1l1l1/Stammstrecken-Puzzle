using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using STP.Bootstrap;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Editor.Build
{
    /// <summary>Explicit isolated Android observation entry; no product save or provider composition.</summary>
    public static class HumanCalibrationBuild
    {
        public const string ScenePath="Assets/StammstreckenPuzzle/Scenes/QA/HumanCalibration.unity";
        private const string Root="Assets/StammstreckenPuzzle/";
        private const string AssignmentRoot=Root+"UI/HumanAssignments/";
        private static string DefaultManifest=>AssignmentRoot+ReadRawLock().defaultManifest;
        private const string UiLockPath=AssignmentRoot+"ui-source-lock.json";
        private const string PanelPath=Root+"Settings/MobilePreviewPanelSettings.asset";
        private static string[]? buildingScenes;

        [MenuItem("Stammstrecken-Puzzle/Human Calibration/Rebuild assigned scene")]
        public static void EnsureArtifacts()
        {
            StpBuildEntrypoints.VerifyScaffoldBuildIdentity();
            VerifySourceLock();
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var root=new GameObject("Assigned Human calibration");
            var installer=root.AddComponent<HumanCalibrationInstaller>();
            string publicHash=JsonUtility.FromJson<PublicBinding>(Required<TextAsset>(Root+"UI/MobileCandidate.bindings.json").text).publicCatalogHashSha256;
            installer.Configure(Required<VisualTreeAsset>(Root+"UI/ProductionGame.uxml"),Required<StyleSheet>(Root+"UI/ProductionGame.uss"),
                Required<TextAsset>(Root+"UI/ProductionGameLocale.json"),Required<PanelSettings>(PanelPath),
                Required<TextAsset>(Root+"UI/MobileCandidate.public.json"),publicHash,Required<TextAsset>(DefaultManifest),Sha(DefaultManifest),
                Required<TextAsset>(UiLockPath),Required<TextAsset>(Root+"UI/RailwayAssets.json"),Required<TextAsset>(Root+"UI/ProductionCampaignText.json"));
            if(!EditorSceneManager.SaveScene(scene,ScenePath))throw new BuildFailedException("HUMAN_SCENE_SAVE_FAILED");
            AssetDatabase.SaveAssets(); VerifyArtifacts();
            Debug.Log("Assigned Human scene created; no observations, timing, device or product approval produced.");
        }
        public static void VerifyArtifacts()
        {
            VerifySourceLock();
            if(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath)==null)throw new BuildFailedException("HUMAN_SCENE_MISSING");
            string[] fields={"tree","styles","locale","panel","publicData","assignmentData","uiSourceLock","railwayAssets","campaignText"};
            string[] paths={Root+"UI/ProductionGame.uxml",Root+"UI/ProductionGame.uss",Root+"UI/ProductionGameLocale.json",PanelPath,
                Root+"UI/MobileCandidate.public.json",DefaultManifest,UiLockPath,Root+"UI/RailwayAssets.json",Root+"UI/ProductionCampaignText.json"};
            var guids=new string[paths.Length];var ids=new long[paths.Length];
            for(int i=0;i<paths.Length;i++)
                if(!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Required<UnityEngine.Object>(paths[i]),out guids[i],out ids[i]))
                    throw new BuildFailedException("HUMAN_SCENE_ASSET_ID_MISSING");
            string scene=File.ReadAllText(ScenePath);
            MobilePreviewBuild.ValidateTypedSceneReferences(scene,AssetDatabase.AssetPathToGUID(Root+"Scripts/Bootstrap/HumanCalibrationInstaller.cs"),fields,guids,ids,new[]{3,3,3,2,3,3,3,3,3});
            if(!Regex.IsMatch(scene,@"(?m)^  assignmentBytesSha256: "+Sha(DefaultManifest)+@"\r?$"))
                throw new BuildFailedException("HUMAN_SCENE_ASSIGNMENT_HASH_STALE");
        }
        private static string[] ExpectedUiSources()=>Directory.GetFiles(Root+"Scripts/Presentation","*.cs",SearchOption.AllDirectories)
            .Concat(Directory.GetFiles(Root+"Art","*",SearchOption.AllDirectories).Where(p=>new[]{".png",".svg",".ttf",".otf"}.Contains(Path.GetExtension(p).ToLowerInvariant())))
            .Concat(new[]{Root+"UI/ProductionGame.uxml",Root+"UI/ProductionGame.uss",Root+"UI/ProductionGameLocale.json",Root+"UI/ProductionCampaignText.json",Root+"UI/RailwayAssets.json"})
            .Select(p=>p.Replace('\\','/')).OrderBy(p=>p,StringComparer.Ordinal).ToArray();
        /// <summary>Actual source verification, never a repair or acceptance of a stale UI lock.</summary>
        public static void VerifySourceLock()
        {
            var source=Required<TextAsset>(UiLockPath);
            var document=JsonUtility.FromJson<UiLock>(source.text);
            if(document==null||document.schemaVersion!=1||document.kind!="HUMAN_UI_SOURCE_LOCK"||document.files==null)
                throw new BuildFailedException("HUMAN_UI_LOCK_INVALID");
            var expected=ExpectedUiSources();
            if(!document.files.Select(f=>f.path).SequenceEqual(expected))throw new BuildFailedException("HUMAN_UI_LOCK_FILESET_MISMATCH");
            foreach(var file in document.files)
                if(!Regex.IsMatch(file.sha256,"^[a-f0-9]{64}$")||Sha(file.path)!=file.sha256)throw new BuildFailedException("HUMAN_UI_SOURCE_CHANGED:"+file.path);
            var raw=ReadRawLock();
            if(raw.uiSourceSha256!=Sha(UiLockPath))throw new BuildFailedException("HUMAN_UI_RAW_LOCK_MISMATCH");
            foreach(var entry in raw.manifests)
                if(!Regex.IsMatch(entry.path,@"^(campaign-pack-0[1-9]|endless-pack-0[1-8])\.json$")||
                    entry.assignmentCount!=6||Sha(AssignmentRoot+entry.path)!=entry.sha256)
                    throw new BuildFailedException("HUMAN_MANIFEST_RAW_LOCK_MISMATCH");
        }
        private static RawLock ReadRawLock()
        {
            var value=JsonUtility.FromJson<RawLock>(Required<TextAsset>(AssignmentRoot+"raw-sha256-lock.json").text);
            if(value==null||value.schemaVersion!=1||value.kind!="HUMAN_ASSIGNMENT_RAW_LOCK"||value.manifests==null||
                value.manifests.Length<9||value.manifests.Length>17||!Regex.IsMatch(value.defaultManifest,@"^campaign-pack-0[1-9]\.json$")||
                value.manifests.Count(m=>m.path==value.defaultManifest)!=1||value.manifests.Select(m=>m.path).Distinct().Count()!=value.manifests.Length)
                throw new BuildFailedException("HUMAN_RAW_LOCK_INVALID");
            return value;
        }
        internal static void ValidateBuildScope(BuildTarget target)
        {
            var named=target==BuildTarget.iOS?NamedBuildTarget.iOS:NamedBuildTarget.Android;
            string id=PlayerSettings.GetApplicationIdentifier(named);
            bool human=id.EndsWith(".human",StringComparison.OrdinalIgnoreCase);
            if(!human){if(buildingScenes!=null)throw new BuildFailedException("HUMAN_BUILD_ID_CHANGED");return;}
            if(target!=BuildTarget.Android||buildingScenes==null)throw new BuildFailedException("HUMAN_EXPLICIT_ANDROID_BUILD_SCOPE_REQUIRED");
            HumanAndroidBuildFence.RequireScope(id,buildingScenes);
        }
        internal static string[] RequireAndroidScope()
        {ValidateBuildScope(BuildTarget.Android);return (string[])(buildingScenes??throw new BuildFailedException("HUMAN_BUILD_SCOPE_MISSING")).Clone();}
        [MenuItem("Stammstrecken-Puzzle/Human Calibration/Build Android")]
        public static void BuildAndroid()
        {
            MobilePrivacyBuildGuard.RequireBuildTarget(BuildTarget.Android); MobileIconAssets.VerifyForActiveTarget();
            StpBuildEntrypoints.VerifyScaffoldBuildIdentity(); VerifyArtifacts();
            if(buildingScenes!=null)throw new BuildFailedException("HUMAN_BUILD_ALREADY_ACTIVE");
            string prior=PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android);bool priorBundle=EditorUserBuildSettings.buildAppBundle;
            var priorBackend=PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android);var priorAbi=PlayerSettings.Android.targetArchitectures;
            var priorMin=PlayerSettings.Android.minSdkVersion;var priorTarget=PlayerSettings.Android.targetSdkVersion;
            var priorOrientation=PlayerSettings.defaultInterfaceOrientation;
            string priorProductName=PlayerSettings.productName;
            try
            {
                PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android,HumanAndroidBuildFence.ApplicationId);
                EditorUserBuildSettings.buildAppBundle=false;
                PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);
                PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
                PlayerSettings.Android.minSdkVersion=(AndroidSdkVersions)26;PlayerSettings.Android.targetSdkVersion=(AndroidSdkVersions)36;
                PlayerSettings.defaultInterfaceOrientation=UIOrientation.Portrait;
                PlayerSettings.productName="Stammstrecken-Puzzle Spielprüfung";
                var options=new BuildPlayerOptions{scenes=new[]{ScenePath},locationPathName="Builds/HumanCalibration/Android/stp-human-calibration.apk",target=BuildTarget.Android,options=BuildOptions.Development};
                buildingScenes=(string[])options.scenes.Clone();ValidateBuildScope(options.target);
                Directory.CreateDirectory(Path.GetDirectoryName(options.locationPathName)!);
                var report=BuildPipeline.BuildPlayer(options);
                if(report.summary.result!=BuildResult.Succeeded)throw new BuildFailedException("HUMAN_ANDROID_BUILD_FAILED:"+report.summary.totalErrors);
                Debug.Log("Assigned Human APK created. Final packaged APK audit is still mandatory before distribution; no device/network/Human approval.");
            }
            finally
            {
                buildingScenes=null;EditorUserBuildSettings.buildAppBundle=priorBundle;PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android,prior);
                PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android,priorBackend);PlayerSettings.Android.targetArchitectures=priorAbi;
                PlayerSettings.Android.minSdkVersion=priorMin;PlayerSettings.Android.targetSdkVersion=priorTarget;
                PlayerSettings.defaultInterfaceOrientation=priorOrientation;
                PlayerSettings.productName=priorProductName;
            }
        }
        private static string Sha(string path){using var hash=SHA256.Create();return string.Concat(hash.ComputeHash(File.ReadAllBytes(path)).Select(b=>b.ToString("x2",System.Globalization.CultureInfo.InvariantCulture)));}
        private static T Required<T>(string path) where T:UnityEngine.Object=>AssetDatabase.LoadAssetAtPath<T>(path)??throw new BuildFailedException("HUMAN_ASSET_MISSING:"+path);
        [Serializable]private sealed class PublicBinding{public string publicCatalogHashSha256=string.Empty;}
        [Serializable]private sealed class UiLock{public int schemaVersion=0;public string kind=string.Empty;public UiFile[] files=Array.Empty<UiFile>();}
        [Serializable]private sealed class UiFile{public string path=string.Empty;public string sha256=string.Empty;}
        [Serializable]private sealed class RawLock{public int schemaVersion=0;public string kind=string.Empty;public string uiSourceSha256=string.Empty;public string defaultManifest=string.Empty;public RawManifest[] manifests=Array.Empty<RawManifest>();}
        [Serializable]private sealed class RawManifest{public string path=string.Empty;public string sha256=string.Empty;public string studyId=string.Empty;public int assignmentCount=0;}
    }
}
