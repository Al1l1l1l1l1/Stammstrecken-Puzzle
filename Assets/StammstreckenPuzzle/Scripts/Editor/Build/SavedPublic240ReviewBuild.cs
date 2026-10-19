using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using STP.Bootstrap;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Editor.Build
{
    /// <summary>Reproduces only the fixed own Saved scene/meta, reading existing A/S assets without modifying them.</summary>
    public static class SavedPublic240ReviewBuild
    {
        /// <summary>First normal internal saved QA entry.</summary>
        public const string ScenePath = "Assets/StammstreckenPuzzle/Scenes/QA/SavedPublic240Review.unity";
        private const string RootPath = "Assets/StammstreckenPuzzle/";
        private const string SceneGuid = "f88de80a150920b165155795a56367f0";
        private const string SceneMeta = "fileFormatVersion: 2\nguid: " + SceneGuid + "\nDefaultImporter:\n  externalObjects: {}\n  userData:\n  assetBundleName:\n  assetBundleVariant:\n";

        /// <summary>Explicit generation; does not create or repair existing panels, materials or other scenes.</summary>
        public static void EnsureArtifacts()
        {
            StpBuildEntrypoints.VerifyScaffoldBuildIdentity();
            var text = SceneText();
            if (File.Exists(ScenePath + ".meta") && Normalize(File.ReadAllText(ScenePath + ".meta")) != SceneMeta)
                throw new InvalidOperationException("SAVED_SCENE_META_MISMATCH");
            if (!File.Exists(ScenePath + ".meta")) File.WriteAllText(ScenePath + ".meta", SceneMeta, new UTF8Encoding(false));
            if (!File.Exists(ScenePath) || Normalize(File.ReadAllText(ScenePath)) != text)
                File.WriteAllText(ScenePath, text, new UTF8Encoding(false));
            AssetDatabase.ImportAsset(ScenePath, ImportAssetOptions.ForceSynchronousImport);
            VerifyArtifacts();
            Debug.Log("STP SavedPublic240 own scene reproduced and verified.");
        }

        /// <summary>Read-only byte-profile preflight; checkout CRLF is compared under the declared Git-LF profile.</summary>
        public static void VerifyArtifacts()
        {
            StpBuildEntrypoints.VerifyScaffoldBuildIdentity();
            var text = SceneText();
            if (!File.Exists(ScenePath) || Normalize(File.ReadAllText(ScenePath)) != text
                || !File.Exists(ScenePath + ".meta") || Normalize(File.ReadAllText(ScenePath + ".meta")) != SceneMeta
                || AssetDatabase.AssetPathToGUID(ScenePath) != SceneGuid)
                throw new InvalidOperationException("SAVED_SCENE_NOT_REPRODUCIBLE");
            Debug.Log("STP SavedPublic240 scene/reference preflight PASS; QA only.");
        }

        /// <summary>Explicit editor convenience entry; normal player entry is owned by the shared build entrypoints.</summary>
        [MenuItem("Stammstrecken-Puzzle/Public240/Open saved review")]
        public static void OpenReview()
        {
            EnsureArtifacts();
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(ScenePath);
        }

        private static string Normalize(string text) => text.Replace("\r\n", "\n");
        private static T Required<T>(string path) where T : UnityEngine.Object =>
            AssetDatabase.LoadAssetAtPath<T>(path) ?? throw new InvalidOperationException("SAVED_ASSET_MISSING:" + path);
        private static string Reference(UnityEngine.Object asset)
        {
            if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out long id))
                throw new InvalidOperationException("SAVED_ASSET_ID_MISSING");
            int type = asset is Material || asset is PanelSettings ? 2 : 3;
            return "{fileID: " + id.ToString(CultureInfo.InvariantCulture) + ", guid: " + guid + ", type: " + type + "}";
        }
        private static void VerifyGuid(string path, string guid)
        {
            if (AssetDatabase.AssetPathToGUID(path) != guid || !File.Exists(path + ".meta")
                || !Regex.IsMatch(File.ReadAllText(path + ".meta"), "(?m)^guid: " + guid + "\\r?$"))
                throw new InvalidOperationException("SAVED_ASSET_GUID_MISMATCH:" + path);
        }
        private static string SceneText()
        {
            var script = Required<MonoScript>(RootPath + "Scripts/Bootstrap/SavedPublic240ReviewInstaller.cs");
            var savedTree = Required<VisualTreeAsset>(RootPath + "UI/SavedPublic240Review.uxml");
            var savedStyles = Required<StyleSheet>(RootPath + "UI/SavedPublic240Review.uss");
            var puzzleTree = Required<VisualTreeAsset>(RootPath + "UI/Public240Review.uxml");
            var puzzleStyles = Required<StyleSheet>(RootPath + "UI/Public240Review.uss");
            var locale = Required<TextAsset>(RootPath + "UI/SavedPublic240ReviewLocale.json");
            var panel = Required<PanelSettings>(RootPath + "UI/Public240ReviewPanelSettings.asset");
            var material = Required<Material>(RootPath + "Art/Development/DevelopmentTrain.mat");
            var publicData = Required<TextAsset>(RootPath + "UI/Public240.public.json");
            var campaign = Required<TextAsset>(RootPath + "UI/ReviewCampaign.catalog.json");
            var completion = Required<TextAsset>(RootPath + "UI/ReviewCompletion.catalog.json");
            SavedPublic240ReviewInstaller.ValidateReferences(savedTree, savedStyles, puzzleTree, puzzleStyles, locale, panel, material);
            SavedPublic240ReviewInstaller.ValidateCatalogData(publicData, campaign, completion);
            VerifyGuid(RootPath + "Scripts/Bootstrap/SavedPublic240ReviewInstaller.cs", "60e3dac3a62eb19df68982e3cd5b9d56");
            VerifyGuid(RootPath + "UI/SavedPublic240Review.uxml", "7666be5eabe395d838b50916622fabd3");
            VerifyGuid(RootPath + "UI/SavedPublic240Review.uss", "daa0e2a635e31a646bdf1ec11df885c6");
            VerifyGuid(RootPath + "UI/Public240Review.uxml", "65df22bf59330d3313c037f9b66a8127");
            VerifyGuid(RootPath + "UI/Public240Review.uss", "9a97e82b2ceabcd350106b19f85d0a55");
            VerifyGuid(RootPath + "UI/SavedPublic240ReviewLocale.json", "08b027e5c889c5df29f6f34f3d3a3d59");
            VerifyGuid(RootPath + "UI/Public240ReviewPanelSettings.asset", "8fd2c857cfac7bdf0748472d8b0402eb");
            VerifyGuid(RootPath + "Art/Development/DevelopmentTrain.mat", "b281a1bc613c4544b3dc4c9e6a038002");
            VerifyGuid(RootPath + "UI/Public240.public.json", "e0d44f18b99809133d6f9af9135ceea2");
            VerifyGuid(RootPath + "UI/ReviewCampaign.catalog.json", "605b0b01b790a7a031c3657fc47c71c4");
            VerifyGuid(RootPath + "UI/ReviewCompletion.catalog.json", "90f5a876c3548f396e3fab1f175669ac");
            return "%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n--- !u!1 &1000\nGameObject:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  serializedVersion: 6\n  m_Component:\n  - component: {fileID: 1001}\n  - component: {fileID: 1002}\n  m_Layer: 0\n  m_Name: SavedPublic240Review\n  m_TagString: Untagged\n  m_Icon: {fileID: 0}\n  m_NavMeshLayer: 0\n  m_StaticEditorFlags: 0\n  m_IsActive: 1\n--- !u!4 &1001\nTransform:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: 1000}\n  serializedVersion: 2\n  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}\n  m_LocalPosition: {x: 0, y: 0, z: 0}\n  m_LocalScale: {x: 1, y: 1, z: 1}\n  m_ConstrainProportionsScale: 0\n  m_Children: []\n  m_Father: {fileID: 0}\n  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}\n--- !u!114 &1002\nMonoBehaviour:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: 1000}\n  m_Enabled: 1\n  m_EditorHideFlags: 0\n  m_Script: @script@\n  m_Name:\n  m_EditorClassIdentifier: STP.Bootstrap::STP.Bootstrap.SavedPublic240ReviewInstaller\n  savedTree: @savedTree@\n  savedStyles: @savedStyles@\n  puzzleTree: @puzzleTree@\n  puzzleStyles: @puzzleStyles@\n  qaLocale: @locale@\n  panelSettings: @panel@\n  trainMaterial: @material@\n  publicData: @publicData@\n  campaignData: @campaign@\n  completionData: @completion@\n  rideDurationSeconds: 3\n--- !u!1660057539 &9223372036854775807\nSceneRoots:\n  m_ObjectHideFlags: 0\n  m_Roots:\n  - {fileID: 1001}\n"
                .Replace("@script@", Reference(script))
                .Replace("@savedTree@", Reference(savedTree))
                .Replace("@savedStyles@", Reference(savedStyles))
                .Replace("@puzzleTree@", Reference(puzzleTree))
                .Replace("@puzzleStyles@", Reference(puzzleStyles))
                .Replace("@locale@", Reference(locale))
                .Replace("@panel@", Reference(panel))
                .Replace("@material@", Reference(material))
                .Replace("@publicData@", Reference(publicData))
                .Replace("@campaign@", Reference(campaign))
                .Replace("@completion@", Reference(completion));
        }
    }
}
