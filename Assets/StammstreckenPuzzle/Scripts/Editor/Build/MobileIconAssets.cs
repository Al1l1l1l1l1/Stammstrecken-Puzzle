using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace STP.Editor.Build
{
    /// <summary>Explicit original-art import/assignment; build verification never repairs PlayerSettings.</summary>
    public static class MobileIconAssets
    {
        private const string Root = "Assets/StammstreckenPuzzle/Art/AppIcon/";
        private static readonly string[] Files = { "stp-appicon-1024.png", "stp-appicon-adaptive-foreground-1024.png", "stp-appicon-adaptive-background-1024.png", "stp-appicon-adaptive-monochrome-1024.png" };

        [MenuItem("Stammstrecken-Puzzle/Mobile Preview/Configure original platform icons")]
        public static void ConfigureForActiveTarget()
        {
            var target = Target();
            MobilePrivacyBuildGuard.RequireBuildTarget(EditorUserBuildSettings.activeBuildTarget);
            foreach (var sourcePath in Directory.GetFiles(Root, "*.png"))
            {
                var path = sourcePath.Replace('\\', '/');
                var file = Path.GetFileName(path);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter ?? throw new InvalidOperationException("ICON_TEXTURE_IMPORTER_MISSING:" + path);
                importer.textureType = TextureImporterType.Default;
                importer.textureShape = TextureImporterShape.Texture2D;
                importer.mipmapEnabled = false;
                importer.ignoreMipmapLimit = true;
                importer.sRGBTexture = true;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.maxTextureSize = 1024;
                importer.alphaSource = TextureImporterAlphaSource.FromInput;
                importer.alphaIsTransparency = file.Contains("foreground") || file.Contains("monochrome");
                importer.SaveAndReimport();
            }
            var textures = Files.Select(Load).ToArray();
            PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new[] { textures[0] }, IconKind.Application);
            var kinds = PlayerSettings.GetSupportedIconKinds(target);
            if (kinds.Length == 0) throw new InvalidOperationException("ICON_PLATFORM_SLOTS_MISSING");
            foreach (var kind in kinds)
            {
                var slots = PlayerSettings.GetPlatformIcons(target, kind);
                foreach (var slot in slots) slot.SetTextures(Layers(target, slot, textures));
                PlayerSettings.SetPlatformIcons(target, kind, slots);
            }
            AssetDatabase.SaveAssets();
            VerifyForActiveTarget();
            Debug.Log("STP original platform icon assignment PASS: " + target.TargetName + "; no device/store acceptance implied.");
        }

        /// <summary>Checks actual persisted texture assignments and the precise layer order.</summary>
        public static void VerifyForActiveTarget()
        {
            var target = Target();
            var textures = Files.Select(Load).ToArray();
            var kinds = PlayerSettings.GetSupportedIconKinds(target);
            if (kinds.Length == 0) throw new InvalidOperationException("ICON_PLATFORM_SLOTS_MISSING");
            int count = 0;
            foreach (var kind in kinds)
            foreach (var slot in PlayerSettings.GetPlatformIcons(target, kind))
            {
                var expected = Layers(target, slot, textures);
                var actual = slot.GetTextures();
                if (actual.Length != expected.Length || !actual.SequenceEqual(expected))
                    throw new InvalidOperationException("ICON_PLATFORM_ASSIGNMENT_INVALID:" + target.TargetName + ":" + slot.width + "x" + slot.height);
                count++;
            }
            if (count == 0) throw new InvalidOperationException("ICON_PLATFORM_SLOTS_EMPTY");
        }

        private static Texture2D[] Layers(NamedBuildTarget target, PlatformIcon slot, Texture2D[] textures)
        {
            if (target == NamedBuildTarget.Android && slot.minLayerCount >= 2 && slot.maxLayerCount >= 2)
                // Unity serializes Android adaptive layers as background, foreground, monochrome.
                return slot.maxLayerCount >= 3 ? new[] { textures[2], textures[1], textures[3] } : new[] { textures[2], textures[1] };
            if (slot.minLayerCount <= 1) return new[] { textures[0] };
            throw new InvalidOperationException("ICON_PLATFORM_LAYER_LAYOUT_UNSUPPORTED");
        }
        private static Texture2D Load(string file)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(Root + file);
            if (texture == null || texture.width != 1024 || texture.height != 1024)
                throw new InvalidOperationException("ICON_MASTER_MISSING_OR_WRONG_SIZE:" + file + ":" +
                    (texture == null ? AssetDatabase.LoadMainAssetAtPath(Root + file)?.GetType().Name ?? "missing" : texture.width + "x" + texture.height));
            return texture;
        }
        private static NamedBuildTarget Target() => EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ? NamedBuildTarget.Android :
            EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS ? NamedBuildTarget.iOS : throw new InvalidOperationException("ICON_MOBILE_TARGET_REQUIRED");
    }
}
