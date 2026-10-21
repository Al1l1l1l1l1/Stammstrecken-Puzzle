using System;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using STP.Bootstrap;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Editor.Build
{
    /// <summary>Reproducible new mobile-quality preview. Remains the isolated QA application identity.</summary>
    public static class MobilePreviewBuild
    {
        /// <summary>The new first mobile preview scene, separate from historical QA forms.</summary>
        public const string ScenePath = "Assets/StammstreckenPuzzle/Scenes/QA/MobilePreview.unity";
        private const string Root = "Assets/StammstreckenPuzzle/";
        private const string PanelPath = Root + "Settings/MobilePreviewPanelSettings.asset";

        /// <summary>Explicitly creates or updates only this program's panel and scene.</summary>
        [MenuItem("Stammstrecken-Puzzle/Mobile Preview/Rebuild scene")]
        public static void EnsureArtifacts()
        {
            StpBuildEntrypoints.VerifyScaffoldBuildIdentity();
            ProductionAudioAssets.EnsureAssets();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var tree = Required<VisualTreeAsset>(Root + "UI/ProductionGame.uxml");
            var styles = Required<StyleSheet>(Root + "UI/ProductionGame.uss");
            var locale = Required<TextAsset>(Root + "UI/ProductionGameLocale.json");
            var puzzles = Required<TextAsset>(Root + "UI/MobileCandidate.public.json");
            var campaign = Required<TextAsset>(Root + "UI/MobileCandidate.campaign.json");
            var completion = Required<TextAsset>(Root + "UI/MobileCandidate.completion.json");
            var cosmetics = Required<TextAsset>(Root + "UI/MobileCandidate.cosmetics.json");
            var bindings = Required<TextAsset>(Root + "UI/MobileCandidate.bindings.json");
            var texts = Required<TextAsset>(Root + "UI/ProductionCampaignText.json");
            var assets = Required<TextAsset>(Root + "UI/RailwayAssets.json");
            var panel = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelPath);
            if (panel == null)
            {
                if (!AssetDatabase.CopyAsset(Root + "UI/Public240ReviewPanelSettings.asset", PanelPath))
                    throw new InvalidOperationException("PREVIEW_PANEL_COPY_FAILED");
                panel = Required<PanelSettings>(PanelPath);
            }
            panel.name = "MobilePreviewPanelSettings";
            panel.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            panel.referenceResolution = new Vector2Int(390, 844);
            panel.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            panel.match = 0;
            EditorUtility.SetDirty(panel);
            var root = new GameObject("Stammstrecken-Puzzle Mobile Preview");
            var installer = root.AddComponent<MobilePreviewInstaller>();
            installer.ConfigureCandidate(tree, styles, locale, panel, puzzles, campaign, completion, assets, cosmetics, bindings,texts);
            installer.ConfigureEndless(Required<TextAsset>(Root+"UI/MobileCandidate.endless-profiles.json"),
                Required<TextAsset>(Root+"UI/MobileCandidate.endless-completion.json"),Required<TextAsset>(Root+"UI/MobileCandidate.endless-bindings.json"));
            installer.ConfigureAudio(ProductionAudioAssets.LoadMixer(),ProductionAudioAssets.LoadClips());
            if (!EditorSceneManager.SaveScene(scene, ScenePath)) throw new InvalidOperationException("PREVIEW_SCENE_SAVE_FAILED");
            AssetDatabase.SaveAssets();
            VerifyArtifacts();
            Debug.Log("STP MobilePreview scene/panel rebuilt and validated; isolated QA profile, no production promotion.");
        }

        /// <summary>Preflight never repairs the source it is verifying.</summary>
        public static void VerifyArtifacts()
        {
            if (!File.Exists(ScenePath)) throw new InvalidOperationException("PREVIEW_SCENE_MISSING");
            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
            var settings = Required<PanelSettings>(PanelPath);
            if (scene == null || settings.referenceResolution != new Vector2Int(390, 844) ||
                settings.scaleMode != PanelScaleMode.ScaleWithScreenSize || settings.match != 0)
                throw new InvalidOperationException("PREVIEW_PANEL_OR_SCENE_INVALID");
            string[] fields={"endlessProfilesData","endlessCompletionData","endlessBindingsData"};
            string[] files={"MobileCandidate.endless-profiles.json","MobileCandidate.endless-completion.json","MobileCandidate.endless-bindings.json"};
            var guids=new string[files.Length];var ids=new long[files.Length];
            for(int i=0;i<files.Length;i++)
                if(!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Required<TextAsset>(Root+"UI/"+files[i]),out guids[i],out ids[i]))
                    throw new InvalidOperationException("PREVIEW_ENDLESS_ASSET_ID_MISSING");
            ValidateSceneReferences(File.ReadAllText(ScenePath),AssetDatabase.AssetPathToGUID(Root+"Scripts/Bootstrap/MobilePreviewInstaller.cs"),fields,guids,ids);
        }

        // Inspect the saved document, without loading/repairing a scene or relying on unrelated dependencies.
        internal static void ValidateSceneReferences(string text,string installerGuid,string[] fields,string[] guids,long[] ids)
        {
            if(!text.StartsWith("%YAML 1.1",StringComparison.Ordinal)||!Regex.IsMatch(installerGuid,"^[a-f0-9]{32}$")||
                fields.Length!=guids.Length||fields.Length!=ids.Length)throw new InvalidOperationException("PREVIEW_SCENE_TEXT_INVALID");
            var blocks=Regex.Split(text,@"(?m)^--- !u!").Where(block=>Regex.IsMatch(block,@"\A114 &-?\d+\r?\nMonoBehaviour:\r?\n")&&Regex.IsMatch(block,
                @"(?m)^  m_Script: \{fileID: 11500000, guid: "+Regex.Escape(installerGuid)+@", type: 3\}\r?$")).ToArray();
            if(blocks.Length!=1||Regex.Matches(blocks[0],@"(?m)^  m_Script:").Count!=1)throw new InvalidOperationException("PREVIEW_INSTALLER_COUNT_INVALID");
            for(int i=0;i<fields.Length;i++)
            {
                var values=Regex.Matches(blocks[0],@"(?m)^  "+Regex.Escape(fields[i])+@": \{fileID: (-?\d+), guid: ([a-f0-9]{32}), type: 3\}\r?$");
                if(Regex.Matches(blocks[0],@"(?m)^  "+Regex.Escape(fields[i])+@":").Count!=1||values.Count!=1||
                    values[0].Groups[1].Value!=ids[i].ToString(CultureInfo.InvariantCulture)||values[0].Groups[2].Value!=guids[i])
                    throw new InvalidOperationException("PREVIEW_ENDLESS_SCENE_REFERENCE_INVALID:"+fields[i]);
            }
        }

        /// <summary>Local installable Windows preview for actual visual/runtime inspection.</summary>
        [MenuItem("Stammstrecken-Puzzle/Mobile Preview/Build Windows")]
        public static void BuildWindows()
        {
            VerifyArtifacts(); StpBuildEntrypoints.VerifyScaffoldBuildIdentity();
            Build("Builds/MobilePreview/Windows/Stammstrecken-Puzzle.exe", BuildTarget.StandaloneWindows64);
        }
        /// <summary>Installable Android ARM64 IL2CPP preview, development signing only.</summary>
        public static void BuildAndroid()
        {
            MobilePrivacyBuildGuard.RequireBuildTarget(BuildTarget.Android);
            VerifyArtifacts(); StpBuildEntrypoints.VerifyScaffoldBuildIdentity();
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel36;
            EditorUserBuildSettings.buildAppBundle = false;
            Build("Builds/MobilePreview/Android/stp-mobile-preview.apk", BuildTarget.Android);
        }
        /// <summary>Reproducible iOS source export; archive/sign/store checks still require macOS and real roles.</summary>
        public static void ExportIos()
        {
            MobilePrivacyBuildGuard.RequireBuildTarget(BuildTarget.iOS);
            VerifyArtifacts(); StpBuildEntrypoints.VerifyScaffoldBuildIdentity();
            Build("Builds/MobilePreview/iOS", BuildTarget.iOS);
        }
        private static void Build(string destination, BuildTarget target)
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            var report = BuildPipeline.BuildPlayer(new[] { ScenePath }, destination, target, BuildOptions.Development);
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("PREVIEW_BUILD_FAILED:" + target + ":" + report.summary.totalErrors);
            Debug.Log("STP MobilePreview build PASS: " + target + " -> " + destination + "; not a signed production RC.");
        }
        private static T Required<T>(string path) where T : UnityEngine.Object =>
            AssetDatabase.LoadAssetAtPath<T>(path) ?? throw new InvalidOperationException("PREVIEW_ASSET_MISSING:" + path);
    }
}
