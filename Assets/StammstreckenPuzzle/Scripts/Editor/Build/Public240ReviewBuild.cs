using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using STP.Bootstrap;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace STP.Editor.Build
{
    /// <summary>Reproduces only the explicitly scoped ADR034 QA scene, panel and neutral train material.</summary>
    public static class Public240ReviewBuild
    {
        /// <summary>Internal playable QA scene; it is never a production entry.</summary>
        public const string ScenePath = "Assets/StammstreckenPuzzle/Scenes/QA/Public240Review.unity";
        private const string UiPath = "Assets/StammstreckenPuzzle/UI/";
        private const string MaterialPath = "Assets/StammstreckenPuzzle/Art/Development/DevelopmentTrain.mat";
        private const string PanelPath = UiPath + "Public240ReviewPanelSettings.asset";
        private const string InstallerPath = "Assets/StammstreckenPuzzle/Scripts/Bootstrap/Public240ReviewInstaller.cs";
        private const string TrainShader = "Universal Render Pipeline/2D/Sprite-Unlit-Default";

        /// <summary>Explicit reproducible editor command; only its own named artifacts can be created or regenerated.</summary>
        public static void EnsureArtifacts()
        {
            StpBuildEntrypoints.VerifyScaffoldBuildIdentity();
            Directory.CreateDirectory(UiPath);
            var theme = Required<ThemeStyleSheet>(UiPath + "DevelopmentPuzzleTheme.tss");
            var panel = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelPath);
            if (panel == null)
            {
                panel = ScriptableObject.CreateInstance<PanelSettings>();
                panel.name = "Public240ReviewPanelSettings";
                panel.themeStyleSheet = theme;
                panel.scaleMode = PanelScaleMode.ScaleWithScreenSize;
                panel.referenceResolution = new Vector2Int(960, 840);
                panel.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
                panel.match = .5f;
                AssetDatabase.CreateAsset(panel, PanelPath);
                AssetDatabase.SaveAssetIfDirty(panel);
                BindNewAssetGuid(PanelPath, "8fd2c857cfac7bdf0748472d8b0402eb");
                panel = Required<PanelSettings>(PanelPath);
            }
            var material = Required<Material>(MaterialPath);
            ValidateAssets(panel, material, theme);
            Public240ReviewInstaller.ValidatePublicData(Required<TextAsset>(UiPath + "Public240.public.json"));
            var scene = SceneText(panel, material);
            WriteMeta(ScenePath, "9942898d28f5332ff27c217a9d2cb12f", "DefaultImporter", 0);
            if (!File.Exists(ScenePath) || !string.Equals(File.ReadAllText(ScenePath), scene, StringComparison.Ordinal))
                File.WriteAllText(ScenePath, scene, new UTF8Encoding(false));
            AssetDatabase.ImportAsset(ScenePath, ImportAssetOptions.ForceSynchronousImport);
            VerifyArtifacts();
            Debug.Log("STP Public240 QA artifacts reproduced and verified.");
        }

        /// <summary>Read-only preflight: every checked-in QA source must already reproduce exactly.</summary>
        public static void VerifyArtifacts()
        {
            StpBuildEntrypoints.VerifyScaffoldBuildIdentity();
            VerifyGuid(PanelPath, "8fd2c857cfac7bdf0748472d8b0402eb");
            VerifyGuid(ScenePath, "9942898d28f5332ff27c217a9d2cb12f");
            var panel = Required<PanelSettings>(PanelPath);
            var material = Required<Material>(MaterialPath);
            var theme = Required<ThemeStyleSheet>(UiPath + "DevelopmentPuzzleTheme.tss");
            ValidateAssets(panel, material, theme);
            if (!File.Exists(ScenePath) || !string.Equals(File.ReadAllText(ScenePath).Replace("\r\n", "\n"), SceneText(panel, material), StringComparison.Ordinal))
                throw new InvalidOperationException("QA scene does not match its deterministic source. Run Public240ReviewBuild.EnsureArtifacts explicitly.");
            Debug.Log("STP Public240 QA scene/panel/material preflight PASS; production profile remains rejected.");
        }

        private static void ValidateAssets(PanelSettings panel, Material material, ThemeStyleSheet theme)
        {
            Public240ReviewInstaller.ValidatePublicData(Required<TextAsset>(UiPath + "Public240.public.json"));
            Public240ReviewInstaller.ValidateReferences(Required<VisualTreeAsset>(UiPath + "Public240Review.uxml"),
                Required<StyleSheet>(UiPath + "Public240Review.uss"), Required<TextAsset>(UiPath + "Public240ReviewLocale.json"), panel, material);
            if (panel.themeStyleSheet != theme || panel.targetTexture != null ||
                panel.scaleMode != PanelScaleMode.ScaleWithScreenSize || panel.referenceResolution != new Vector2Int(960, 840) ||
                panel.screenMatchMode != PanelScreenMatchMode.MatchWidthOrHeight || !Mathf.Approximately(panel.match, .5f) ||
                material.shader.name != TrainShader)
                throw new InvalidOperationException("QA panel/material differs from its declared reproducible technical configuration.");
        }

        /// <summary>Concrete internal Editor entry. This does not change regular platform build scenes.</summary>
        [MenuItem("Stammstrecken-Puzzle/Public240/Open review")]
        public static void OpenReview()
        {
            EnsureArtifacts();
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(ScenePath);
        }

        private static T Required<T>(string assetPath) where T : UnityEngine.Object =>
            AssetDatabase.LoadAssetAtPath<T>(assetPath) ?? throw new InvalidOperationException("Missing own QA asset: " + assetPath);

        private static string Reference(UnityEngine.Object asset)
        {
            if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out long fileId))
                throw new InvalidOperationException("QA asset has no stable GUID/local identifier.");
            var type = asset is Material || asset is PanelSettings ? 2 : 3;
            return "{fileID: " + fileId.ToString(CultureInfo.InvariantCulture) + ", guid: " + guid + ", type: " + type + "}";
        }

        private static void BindNewAssetGuid(string assetPath, string guid)
        {
            // CreateAsset writes its own meta, so bind the declared identity only AFTER creation.
            // This branch is limited to an asset created by this exact builder invocation.
            var meta = File.ReadAllText(assetPath + ".meta");
            var pattern = new Regex("(?m)^guid: [0-9a-f]{32}\\r?$");
            if (pattern.Matches(meta).Count != 1) throw new InvalidOperationException("New QA asset has no single meta GUID: " + assetPath);
            var boundMeta = pattern.Replace(meta, "guid: " + guid);
            File.WriteAllText(assetPath + ".meta", CanonicalOwnYaml(boundMeta), new UTF8Encoding(false));
            File.WriteAllText(assetPath, CanonicalOwnYaml(File.ReadAllText(assetPath)), new UTF8Encoding(false));
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            VerifyGuid(assetPath, guid);
        }

        private static string CanonicalOwnYaml(string text) =>
            Regex.Replace(text.Replace("\r\n", "\n"), "[ \\t]+$", "", RegexOptions.Multiline);

        private static void VerifyGuid(string assetPath, string guid)
        {
            if (!File.Exists(assetPath + ".meta") ||
                !Regex.IsMatch(File.ReadAllText(assetPath + ".meta"), "(?m)^guid: " + guid + "\\r?$") ||
                !string.Equals(AssetDatabase.AssetPathToGUID(assetPath), guid, StringComparison.Ordinal))
                throw new InvalidOperationException("QA artifact does not have its declared fixed GUID: " + assetPath);
        }

        private static void WriteMeta(string assetPath, string guid, string importer, long fileId)
        {
            var text = "fileFormatVersion: 2\nguid: " + guid + "\n" + importer + ":\n  externalObjects: {}\n" +
                (fileId == 0 ? "" : "  mainObjectFileID: " + fileId.ToString(CultureInfo.InvariantCulture) + "\n") +
                "  userData:\n  assetBundleName:\n  assetBundleVariant:\n";
            if (File.Exists(assetPath + ".meta"))
            {
                if (!File.ReadAllText(assetPath + ".meta").Contains("guid: " + guid, StringComparison.Ordinal))
                    throw new InvalidOperationException("QA artifact GUID differs from its fixed identity: " + assetPath);
                return;
            }
            File.WriteAllText(assetPath + ".meta", text, new UTF8Encoding(false));
        }

        private static string SceneText(PanelSettings panel, Material material)
        {
            var script = Required<MonoScript>(InstallerPath);
            var tree = Required<VisualTreeAsset>(UiPath + "Public240Review.uxml");
            var styles = Required<StyleSheet>(UiPath + "Public240Review.uss");
            var locale = Required<TextAsset>(UiPath + "Public240ReviewLocale.json");
            var data = Required<TextAsset>(UiPath + "Public240.public.json");
            return "%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n" +
                "--- !u!1 &1000\nGameObject:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  serializedVersion: 6\n  m_Component:\n  - component: {fileID: 1001}\n  - component: {fileID: 1002}\n  m_Layer: 0\n  m_Name: Public240Review\n  m_TagString: Untagged\n  m_Icon: {fileID: 0}\n  m_NavMeshLayer: 0\n  m_StaticEditorFlags: 0\n  m_IsActive: 1\n" +
                "--- !u!4 &1001\nTransform:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: 1000}\n  serializedVersion: 2\n  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}\n  m_LocalPosition: {x: 0, y: 0, z: 0}\n  m_LocalScale: {x: 1, y: 1, z: 1}\n  m_ConstrainProportionsScale: 0\n  m_Children: []\n  m_Father: {fileID: 0}\n  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}\n" +
                "--- !u!114 &1002\nMonoBehaviour:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: 1000}\n  m_Enabled: 1\n  m_EditorHideFlags: 0\n  m_Script: " + Reference(script) +
                "\n  m_Name:\n  m_EditorClassIdentifier: STP.Bootstrap::STP.Bootstrap.Public240ReviewInstaller\n  uiTree: " + Reference(tree) +
                "\n  uiStyles: " + Reference(styles) + "\n  qaLocale: " + Reference(locale) + "\n  panelSettings: " + Reference(panel) +
                "\n  trainMaterial: " + Reference(material) + "\n  publicData: " + Reference(data) + "\n  rideDurationSeconds: 3\n" +
                "--- !u!1660057539 &9223372036854775807\nSceneRoots:\n  m_ObjectHideFlags: 0\n  m_Roots:\n  - {fileID: 1001}\n";
        }
    }

    /// <summary>Records the actual player build's processed QA scenes against its real BuildReport.</summary>
    public sealed class Public240ReviewSceneBuildCheck : IProcessSceneWithReport
    {
        /// <summary>Runs before optional later scene processors.</summary>
        public int callbackOrder => 0;

        /// <summary>Checks compiled scene roots; this is build validation, never runtime scene-search wiring.</summary>
        public void OnProcessScene(Scene scene, BuildReport report)
        {
            // Unity also processes scenes for Editor PlayMode without a player report.
            if (report == null) return;
            StpBuildEntrypoints.VerifyScaffoldBuildIdentity();
            if (scene.path != Public240ReviewBuild.ScenePath) return;
            var count = 0;
            foreach (var root in scene.GetRootGameObjects())
                count += root.GetComponentsInChildren<Public240ReviewInstaller>(true).Length;
            if (count != 1) throw new InvalidOperationException("Built QA scene must contain exactly its explicit installer: " + scene.path);
            Debug.Log($"STP actual BuildReport QA scene processed: {scene.path}; platform={report.summary.platform}; buildGuid={report.summary.guid}; installers={count}.");
        }
    }
}
