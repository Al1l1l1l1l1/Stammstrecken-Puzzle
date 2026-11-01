using System;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace STP.MobileServices.Store
{
    /// <summary>Explicit local StoreKit2 verification; a JWSstring alone is never trusted.</summary>
    public static class AppleStoreKitVerifier
    {
        /// <summary>Verifies exact signed identity, environment and revocation using StoreKit2.</summary>
        public static void Verify(string nativeId, string bundleId, string productId, bool production, Action<VerifiedStoreTransaction?> completed)
        {
#if UNITY_IOS && !UNITY_EDITOR
            var handle = GCHandle.Alloc(completed);
            try { stp_verify_apple_transaction(nativeId, bundleId, productId, production ? 1 : 0, Callback, GCHandle.ToIntPtr(handle)); }
            catch { handle.Free(); completed(null); }
#else
            completed(null);
#endif
        }
#if UNITY_IOS && !UNITY_EDITOR
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void NativeCallback(IntPtr context, int state, [MarshalAs(UnmanagedType.LPUTF8Str)] string transactionId, int sandbox);
        private static readonly NativeCallback Callback = VerificationCompleted;
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern void stp_verify_apple_transaction([MarshalAs(UnmanagedType.LPUTF8Str)] string transactionId, [MarshalAs(UnmanagedType.LPUTF8Str)] string bundleId, [MarshalAs(UnmanagedType.LPUTF8Str)] string productId, int production, NativeCallback completed, IntPtr context);
        [AOT.MonoPInvokeCallback(typeof(NativeCallback))]
        private static void VerificationCompleted(IntPtr context, int state, string transactionId, int sandbox)
        {
            var handle = GCHandle.FromIntPtr(context);
            var completed = handle.Target as Action<VerifiedStoreTransaction?>;
            handle.Free();
            completed?.Invoke(state == 1 || state == 2 ? new VerifiedStoreTransaction(transactionId, sandbox == 1 ? "Sandbox" : "Production", state == 2) : null);
        }
#endif
    }
}
