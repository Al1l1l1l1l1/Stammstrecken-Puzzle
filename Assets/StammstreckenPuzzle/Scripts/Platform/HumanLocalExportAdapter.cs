using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace STP.Platform
{
    /// <summary>ADR042 explicit local Human export. A requested chooser is never a delivery or human-attestation fact.</summary>
    public sealed class HumanLocalExportAdapter
    {
        public const string HumanApplicationId = "com.STP.StammstreckenPuzzle.human";
        public const string Authority = HumanApplicationId + ".humanexports";
        private readonly int owner = Thread.CurrentThread.ManagedThreadId;
        private readonly string applicationId;
        public HumanLocalExportAdapter(string applicationId)
        { RequireApplicationId(applicationId); this.applicationId = applicationId; }

        /// <summary>Returns only a direct JSON child of the app's private internal export directory. The caller writes the original repository bytes.</summary>
        public string PrepareFilePath(string fileName)
        {
            RequireOwnerAndScope(); ValidateFileName(fileName);
            string directory = ExportDirectory(); CheckAncestors(directory); Directory.CreateDirectory(directory); CheckAncestors(directory);
            return ValidateChildPath(directory, Path.Combine(directory, fileName), false);
        }

        /// <summary>True means Android accepted the explicit chooser start request; false is the Editor's local-file-only path. No automatic destination is selected.</summary>
        public Task<bool> ShareAsync(string path, string title, CancellationToken cancellation)
        {
            RequireOwnerAndScope(); cancellation.ThrowIfCancellationRequested();
            string checkedPath = ValidateChildPath(ExportDirectory(), path, true);
            if (string.IsNullOrWhiteSpace(title) || title.Length > 160) throw new ArgumentException("HUMAN_EXPORT_TITLE_INVALID");
#if UNITY_ANDROID && !UNITY_EDITOR
            var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            if (activity == null) throw new InvalidOperationException("HUMAN_EXPORT_ACTIVITY_UNAVAILABLE");
            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                if (cancellation.IsCancellationRequested) { completion.TrySetCanceled(); return; }
                try
                {
                    using var liveUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    using var liveActivity = liveUnity.GetStatic<AndroidJavaObject>("currentActivity");
                    if (liveActivity == null || liveActivity.Call<string>("getPackageName") != HumanApplicationId)
                        throw new InvalidOperationException("HUMAN_EXPORT_APPLICATION_MISMATCH");
                    using var javaFile = new AndroidJavaObject("java.io.File", checkedPath);
                    using var files = liveActivity.Call<AndroidJavaObject>("getFilesDir");
                    string internalDirectory = Path.Combine(files.Call<string>("getCanonicalPath"), "human-exports");
                    ValidateChildPath(internalDirectory, javaFile.Call<string>("getCanonicalPath"), true);
                    using var provider = new AndroidJavaClass("androidx.core.content.FileProvider");
                    using var uri = provider.CallStatic<AndroidJavaObject>("getUriForFile", liveActivity, Authority, javaFile);
                    using var intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.SEND");
                    using var typed = intent.Call<AndroidJavaObject>("setType", "application/json");
                    using var stream = intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.STREAM", uri);
                    using var clip = new AndroidJavaClass("android.content.ClipData");
                    using var data = clip.CallStatic<AndroidJavaObject>("newRawUri", "Human observation JSON", uri);
                    intent.Call("setClipData", data);
                    using var readGrant = intent.Call<AndroidJavaObject>("addFlags", 1); // FLAG_GRANT_READ_URI_PERMISSION only, no write or persistable grant.
                    using var intentClass = new AndroidJavaClass("android.content.Intent");
                    using var chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intent, title);
                    using var chooserReadGrant = chooser.Call<AndroidJavaObject>("addFlags", 1);
                    liveActivity.Call("startActivity", chooser);
                    completion.TrySetResult(true);
                }
                catch (Exception error) { completion.TrySetException(error); }
            }));
            return completion.Task;
#elif UNITY_EDITOR
            return Task.FromResult(false);
#else
            throw new PlatformNotSupportedException("HUMAN_ANDROID_EXPORT_ONLY");
#endif
        }

        private void RequireOwnerAndScope()
        {
            if (Thread.CurrentThread.ManagedThreadId != owner) throw new InvalidOperationException("HUMAN_EXPORT_OWNER_THREAD_REQUIRED");
            RequireApplicationId(applicationId);
#if !UNITY_EDITOR
            RequireApplicationId(UnityEngine.Application.identifier);
#endif
        }
        private string ExportDirectory()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            if (activity == null || activity.Call<string>("getPackageName") != HumanApplicationId) throw new InvalidOperationException("HUMAN_EXPORT_APPLICATION_MISMATCH");
            using var files = activity.Call<AndroidJavaObject>("getFilesDir");
            return Path.Combine(files.Call<string>("getCanonicalPath"), "human-exports");
#elif UNITY_EDITOR
            return Path.Combine(UnityEngine.Application.persistentDataPath, "human-exports");
#else
            throw new PlatformNotSupportedException("HUMAN_ANDROID_EXPORT_ONLY");
#endif
        }
        internal static void RequireApplicationId(string value)
        { if (value != HumanApplicationId) throw new ArgumentException("HUMAN_EXPORT_APPLICATION_MISMATCH"); }
        internal static void ValidateFileName(string value)
        {
            if (value == null || !Regex.IsMatch(value, "^[A-Za-z0-9][A-Za-z0-9_-]{0,110}\\.json$", RegexOptions.CultureInvariant))
                throw new ArgumentException("HUMAN_EXPORT_FILE_NAME_INVALID");
        }
        internal static string ValidateChildPath(string directory, string path, bool requireExisting)
        {
            if (!Path.IsPathRooted(directory) || !Path.IsPathRooted(path)) throw new ArgumentException("HUMAN_EXPORT_ABSOLUTE_PATH_REQUIRED");
            directory = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string result = Path.GetFullPath(path); ValidateFileName(Path.GetFileName(result));
            if (!string.Equals(Path.GetDirectoryName(result), directory, StringComparison.Ordinal)) throw new ArgumentException("HUMAN_EXPORT_DIRECT_CHILD_REQUIRED");
            CheckAncestors(result);
            if (requireExisting && (!File.Exists(result) || new FileInfo(result).Length <= 0 || new FileInfo(result).Length > 8388608))
                throw new ArgumentException("HUMAN_EXPORT_FILE_REQUIRED");
            return result;
        }
        private static void CheckAncestors(string path)
        {
            for (string? node = Path.GetFullPath(path); node != null; node = Path.GetDirectoryName(node))
                if ((File.Exists(node) || Directory.Exists(node)) && (File.GetAttributes(node) & FileAttributes.ReparsePoint) != 0)
                    throw new IOException("HUMAN_EXPORT_REPARSE_REJECTED");
        }
    }
}
