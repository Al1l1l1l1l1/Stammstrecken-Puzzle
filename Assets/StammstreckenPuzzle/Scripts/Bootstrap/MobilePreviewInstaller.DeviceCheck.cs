#if DEVELOPMENT_BUILD && UNITY_ANDROID && !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Bootstrap
{
    public sealed partial class MobilePreviewInstaller
    {
        [Serializable] private sealed class VisibleTarget { public string name = "", text = ""; public int x, y, width, height; public bool enabled; }
        [Serializable] private sealed class DevicePage
        {
            public string status = "", phase = "", puzzle = "", mode = "", storage = "";
            public int clears, width, height, ax, ay, ae, bx, by, be, frameTarget;
            public int[] rows = Array.Empty<int>(), columns = Array.Empty<int>();
            public string[] equipped = Array.Empty<string>(), owned = Array.Empty<string>();
            public List<VisibleTarget> targets = new List<VisibleTarget>();
            public double measuredFps;
        }

        private string? probeDir;
        private float nextProbe, sampleStart;
        private int sampleFrames;

        private void EnableDeviceCheck()
        {
            using var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            using var launch = activity.Call<AndroidJavaObject>("getIntent");
            if (!launch.Call<bool>("getBooleanExtra", "stpDeviceCheck", false)) return;
            using var files = activity.Call<AndroidJavaObject>("getFilesDir");
            probeDir = files.Call<string>("getAbsolutePath");
        }

        private void PollDeviceCheck()
        {
            if (probeDir == null) return;
            float now = Time.realtimeSinceStartup;
            if (sampleFrames > 0) sampleFrames++;
            if (now < nextProbe) return;
            nextProbe = now + .25f;
            string request = Path.Combine(probeDir, "screen.request");
            if (!File.Exists(request)) return;
            string operation = File.ReadAllText(request).Trim();
            if (operation == "sample" && sampleFrames == 0) { sampleStart = now; sampleFrames = 1; nextProbe = now + 4; return; }
            var root = Document.rootVisualElement;
            var panel = root.panel;
            if (panel == null) return;
            var page = new DevicePage { storage = StorageRoot, status = model?.State.Status.ToString() ?? "Starting",
                mode = model?.CurrentModeName ?? "", frameTarget = UnityEngine.Application.targetFrameRate,
                clears = model?.State.Progress?.FirstClearCount ?? 0 };
            if (sampleFrames > 0) { page.measuredFps = (sampleFrames - 1) / (now - sampleStart); sampleFrames = 0; }
            var inventory = model?.ConfirmedSnapshot?.Payload.Inventory;
            if (inventory != null) {
                page.equipped = inventory.SelectedItemIds.ToArray();
                page.owned = inventory.Ownership.Select(i => i.ItemId).ToArray();
            }
            var view = model?.PuzzleView;
            if (view != null) {
                page.puzzle = view.Puzzle.Identity.PuzzleId; page.phase = view.Phase.ToString();
                page.width = view.Width; page.height = view.Height;
                page.ax = view.EndpointA.X; page.ay = view.EndpointA.Y; page.ae = (int)view.EndpointA.Edge;
                page.bx = view.EndpointB.X; page.by = view.EndpointB.Y; page.be = (int)view.EndpointB.Edge;
                page.rows = Enumerable.Range(0,view.Height).Select(view.RowTarget).ToArray();
                page.columns = Enumerable.Range(0,view.Width).Select(view.ColumnTarget).ToArray();
            }
            var bounds = panel.visualTree.worldBound;
            float sx = Screen.width / bounds.width, sy = Screen.height / bounds.height;
            root.Query<VisualElement>().ForEach(element => {
                if (string.IsNullOrEmpty(element.name) || !(element is Button || element.name.StartsWith("puzzle-cell-", StringComparison.Ordinal))) return;
                Rect box = element.worldBound;
                if (box.width <= 0 || box.height <= 0 || !bounds.Contains(box.center)) return;
                var hit = panel.Pick(box.center);
                if (hit == null || (hit != element && !element.Contains(hit))) return;
                page.targets.Add(new VisibleTarget { name = element.name, text = (element as Button)?.text ?? "",
                    enabled = element.enabledInHierarchy, x = Mathf.RoundToInt((box.center.x-bounds.x)*sx),
                    y = Mathf.RoundToInt((box.center.y-bounds.y)*sy), width = Mathf.RoundToInt(box.width*sx), height = Mathf.RoundToInt(box.height*sy) });
            });
            string output = Path.Combine(probeDir, "screen.json");
            File.WriteAllText(output, JsonUtility.ToJson(page));
            File.Delete(request);
        }
    }
}
#endif
