using System;
using System.IO;
using System.Globalization;
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
        private const string RawLockPath=AssignmentRoot+"raw-sha256-lock.json";
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
                Required<TextAsset>(Root+"UI/MobileCandidate.public.json"),publicHash,
                HumanManifestRegistry.ExpectedPackPaths().Select(p=>Required<TextAsset>(AssignmentRoot+p)).ToArray(),Required<TextAsset>(RawLockPath),
                Required<TextAsset>(UiLockPath),Required<TextAsset>(Root+"UI/RailwayAssets.json"),Required<TextAsset>(Root+"UI/ProductionCampaignText.json"));
            if(!EditorSceneManager.SaveScene(scene,ScenePath))throw new BuildFailedException("HUMAN_SCENE_SAVE_FAILED");
            AssetDatabase.SaveAssets(); VerifyArtifacts();
            Debug.Log("Assigned Human scene created; no observations, timing, device or product approval produced.");
        }
        public static void VerifyArtifacts()
        {
            VerifySourceLock();
            if(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath)==null)throw new BuildFailedException("HUMAN_SCENE_MISSING");
            string[] fields={"tree","styles","locale","panel","publicData","rawAssignmentLock","uiSourceLock","railwayAssets","campaignText"};
            string[] paths={Root+"UI/ProductionGame.uxml",Root+"UI/ProductionGame.uss",Root+"UI/ProductionGameLocale.json",PanelPath,
                Root+"UI/MobileCandidate.public.json",RawLockPath,UiLockPath,Root+"UI/RailwayAssets.json",Root+"UI/ProductionCampaignText.json"};
            var guids=new string[paths.Length];var ids=new long[paths.Length];
            for(int i=0;i<paths.Length;i++)
                if(!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Required<UnityEngine.Object>(paths[i]),out guids[i],out ids[i]))
                    throw new BuildFailedException("HUMAN_SCENE_ASSET_ID_MISSING");
            string scene=File.ReadAllText(ScenePath);
            MobilePreviewBuild.ValidateTypedSceneReferences(scene,AssetDatabase.AssetPathToGUID(Root+"Scripts/Bootstrap/HumanCalibrationInstaller.cs"),fields,guids,ids,new[]{3,3,3,2,3,3,3,3,3});
            var packPaths=HumanManifestRegistry.ExpectedPackPaths().Select(p=>AssignmentRoot+p).ToArray();
            var packGuids=new string[packPaths.Length];var packIds=new long[packPaths.Length];
            for(int i=0;i<packPaths.Length;i++)
                if(!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Required<TextAsset>(packPaths[i]),out packGuids[i],out packIds[i]))
                    throw new BuildFailedException("HUMAN_SCENE_PACK_ASSET_ID_MISSING");
            ValidatePackSceneReferences(scene,AssetDatabase.AssetPathToGUID(Root+"Scripts/Bootstrap/HumanCalibrationInstaller.cs"),packGuids,packIds);
            string currentPublicHash=JsonUtility.FromJson<PublicBinding>(Required<TextAsset>(Root+"UI/MobileCandidate.bindings.json").text).publicCatalogHashSha256;
            ValidatePublicCatalogHashValue(scene,AssetDatabase.AssetPathToGUID(Root+"Scripts/Bootstrap/HumanCalibrationInstaller.cs"),currentPublicHash);
        }
        // Exact saved component/array bindings, independent of incidental dependency or GUID text elsewhere in the scene.
        internal static void ValidatePackSceneReferences(string text,string installerGuid,string[] guids,long[] ids)
        {
            if(!text.StartsWith("%YAML 1.1",StringComparison.Ordinal)||!Regex.IsMatch(installerGuid,"^[a-f0-9]{32}$")||
                guids.Length!=17||ids.Length!=17||guids.Any(g=>!Regex.IsMatch(g,"^[a-f0-9]{32}$"))||
                guids.Distinct(StringComparer.Ordinal).Count()!=17||ids.Any(id=>id!=4900000))throw new InvalidOperationException("HUMAN_SCENE_PACK_ARGUMENTS");
            var blocks=Regex.Split(text,@"(?m)^--- !u!").Where(block=>Regex.IsMatch(block,@"\A114 &-?\d+\r?\nMonoBehaviour:\r?\n")&&
                Regex.IsMatch(block,@"(?m)^  m_Script: \{fileID: 11500000, guid: "+installerGuid+@", type: 3\}\r?$")).ToArray();
            if(blocks.Length!=1||Regex.Matches(blocks[0],@"(?m)^  m_Script:").Count!=1||
                Regex.Matches(blocks[0],@"(?m)^  assignmentPacks:").Count!=1||
                Regex.IsMatch(blocks[0],@"(?m)^  (assignmentData|assignmentBytesSha256):"))
                throw new InvalidOperationException("HUMAN_SCENE_PACK_COMPONENT");
            var array=Regex.Match(blocks[0],@"(?m)^  assignmentPacks:\r?\n(?<items>(?:  - [^\r\n]*\r?\n)*)(?=  [A-Za-z_][A-Za-z0-9_]*:|\z)");
            if(!array.Success)throw new InvalidOperationException("HUMAN_SCENE_PACK_ARRAY");
            var lines=array.Groups["items"].Value.Replace("\r\n","\n").Split(new[]{'\n'},StringSplitOptions.RemoveEmptyEntries);
            if(lines.Length!=17)throw new InvalidOperationException("HUMAN_SCENE_PACK_COUNT");
            for(int i=0;i<17;i++)
                if(lines[i]!="  - {fileID: "+ids[i].ToString(CultureInfo.InvariantCulture)+", guid: "+guids[i]+", type: 3}")
                    throw new InvalidOperationException("HUMAN_SCENE_PACK_REFERENCE:"+i.ToString(CultureInfo.InvariantCulture));
        }
        // Inspect the actual installer value; an unrelated component cannot satisfy its binding.
        internal static void ValidatePublicCatalogHashValue(string text,string installerGuid,string expectedHash)
        {
            if(text==null||!text.StartsWith("%YAML 1.1",StringComparison.Ordinal)||installerGuid==null||expectedHash==null||
                !Regex.IsMatch(installerGuid,@"\A[a-f0-9]{32}\z")||!Regex.IsMatch(expectedHash,@"\A[a-f0-9]{64}\z"))
                throw new InvalidOperationException("HUMAN_SCENE_PUBLIC_HASH_ARGUMENTS");
            var blocks=Regex.Split(text,@"(?m)^--- !u!").Where(block=>Regex.IsMatch(block,@"\A114 &-?\d+\r?\nMonoBehaviour:\r?\n")&&
                Regex.IsMatch(block,@"(?m)^  m_Script: \{fileID: 11500000, guid: "+installerGuid+@", type: 3\}\r?$")).ToArray();
            if(blocks.Length!=1||Regex.Matches(blocks[0],@"(?m)^  m_Script:").Count!=1||
                Regex.Matches(blocks[0],@"(?m)^  publicCatalogHashSha256:").Count!=1)
                throw new InvalidOperationException("HUMAN_SCENE_PUBLIC_HASH_COMPONENT");
            var value=Regex.Match(blocks[0],@"(?m)^  publicCatalogHashSha256: (?<hash>[a-f0-9]{64})\r?$");
            if(!value.Success||!string.Equals(value.Groups["hash"].Value,expectedHash,StringComparison.Ordinal))
                throw new InvalidOperationException("HUMAN_SCENE_PUBLIC_HASH_MISMATCH");
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
            string publicHash=JsonUtility.FromJson<PublicBinding>(Required<TextAsset>(Root+"UI/MobileCandidate.bindings.json").text).publicCatalogHashSha256;
            HumanCalibrationInstaller.ValidateRoutingAssets(HumanManifestRegistry.ExpectedPackPaths().Select(p=>Required<TextAsset>(AssignmentRoot+p)).ToArray(),
                Required<TextAsset>(RawLockPath),source,Required<TextAsset>(Root+"UI/MobileCandidate.public.json"),publicHash);
        }
        private static RawLock ReadRawLock()
        {
            var value=JsonUtility.FromJson<RawLock>(Required<TextAsset>(RawLockPath).text);
            if(value==null||value.schemaVersion!=1||value.kind!="HUMAN_ASSIGNMENT_RAW_LOCK"||value.manifests==null||
                value.manifests.Length!=17||!Regex.IsMatch(value.defaultManifest,@"^campaign-pack-0[1-9]\.json$")||
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
