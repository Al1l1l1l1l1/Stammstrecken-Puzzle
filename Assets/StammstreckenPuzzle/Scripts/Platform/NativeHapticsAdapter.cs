using System;
using System.Diagnostics;
using System.Threading;
using STP.Application.Ports;
using UnityEngine;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace STP.Platform
{
    /// <summary>Optional neutral interaction haptics; never inspects a puzzle, validity, reward or correctness.</summary>
    public sealed class NativeHapticsAdapter : IHapticsPort
    {
        private readonly int ownerThread = Thread.CurrentThread.ManagedThreadId;
        private volatile bool enabled;
        private volatile bool suspended = true;
        private bool nativeUnavailable = false;
        private long previous = 0;

        /// <summary>The composition supplies saved user preference and its combined lifecycle/modal pause.</summary>
        public void SetState(bool enabled, bool suspended)
        {
            if (Thread.CurrentThread.ManagedThreadId != ownerThread) throw new InvalidOperationException("HAPTICS_OWNER_THREAD_REQUIRED");
            this.enabled = enabled; this.suspended = suspended;
        }

        /// <summary>Requests one debounced selection tick. True means requested, never proof of physical motor output.</summary>
        public bool PulseInteraction()
        {
            if (Thread.CurrentThread.ManagedThreadId != ownerThread || !enabled || suspended || nativeUnavailable) return false;
            long now = Stopwatch.GetTimestamp();
            if (previous != 0 && (now - previous) / (double)Stopwatch.Frequency < .08) return false;
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                if (activity == null) return false;
                activity.Call("runOnUiThread", new AndroidJavaRunnable(AndroidInteraction));
                previous = now; return true;
            }
            catch { nativeUnavailable = true; return false; }
#elif UNITY_IOS && !UNITY_EDITOR
            try { bool requested = stp_haptic_interaction() == 1; if (requested) previous = now; return requested; }
            catch { nativeUnavailable = true; return false; }
#else
            return false;
#endif
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        private void AndroidInteraction()
        {
            if (!enabled || suspended) return;
            try
            {
                using var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                using var window = activity?.Call<AndroidJavaObject>("getWindow");
                using var view = window?.Call<AndroidJavaObject>("getDecorView");
                using var feedback = new AndroidJavaClass("android.view.HapticFeedbackConstants");
                // No IGNORE_GLOBAL_SETTING flag and no additional vibration permission.
                view?.Call<bool>("performHapticFeedback", feedback.GetStatic<int>("CLOCK_TICK"));
            }
            catch { /* Optional feedback never interrupts or changes the user's action. */ }
        }
#endif
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern int stp_haptic_interaction();
#endif
    }
}
