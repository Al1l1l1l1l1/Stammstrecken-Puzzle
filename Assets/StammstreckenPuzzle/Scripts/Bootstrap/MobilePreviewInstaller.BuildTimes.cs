#if DEVELOPMENT_BUILD
using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Bootstrap
{
    public sealed partial class MobilePreviewInstaller
    {
        private const string TimingFileName = "star-times-debug.jsonl";
        private static string TimingPath => Path.Combine(UnityEngine.Application.persistentDataPath, TimingFileName);

        [Serializable]
        private sealed class BuildTime
        {
            public string puzzleId = string.Empty;
            public string size = string.Empty;
            public double seconds;
            public bool notesUsed;
            public int hintsUsed;
        }

        private void RecordBuildTime(int width, int height)
        {
            var record = new BuildTime { size = width + "x" + height };
            if (fullGame == null || !fullGame.Persisted.TryGetSolvedTiming(out record.puzzleId,
                out record.seconds, out record.notesUsed, out record.hintsUsed)) return;
            try { File.AppendAllText(TimingPath, JsonUtility.ToJson(record) + "\n", new UTF8Encoding(false)); }
            catch (IOException) { Debug.LogWarning("Bauzeit konnte nicht gespeichert werden."); }
            catch (UnauthorizedAccessException) { Debug.LogWarning("Kein Zugriff auf lokale Bauzeiten."); }
        }

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void StpShareBuildTimes(string path);
#endif
        private void AddBuildTimeExport(VisualElement root)
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            var export = new Button { name = "settings-export-times", text = "Bauzeiten exportieren" };
            export.AddToClassList("secondary-button");
            export.clicked += () => {
                if (localDataInputFence || presenter?.IsProductModalOpen != true) return;
                if (!File.Exists(TimingPath)) { export.text = "Zuerst ein Rätsel lösen"; return; }
#if UNITY_ANDROID
                using var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                using var share = new AndroidJavaClass("com.stp.railway.PunctualityProof");
                share.CallStatic("shareDocument", activity, TimingPath, TimingFileName);
#elif UNITY_IOS
                StpShareBuildTimes(TimingPath);
#endif
            };
            root.Q<Button>("settings-motion").parent.Add(export);
#endif
        }

        private static void DeleteBuildTimes()
        {
            File.Delete(TimingPath);
#if UNITY_ANDROID && !UNITY_EDITOR
            using var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            using var cache = activity.Call<AndroidJavaObject>("getCacheDir");
            File.Delete(Path.Combine(cache.Call<string>("getAbsolutePath"), "punctuality", TimingFileName));
#endif
        }
    }
}
#endif
