using System;
using STP.Application;

namespace STP.MobileServices.Store
{
    /// <summary>Availability guarded native composition; creating the factory never creates a store controller.</summary>
    public static class NativeStoreRuntimeFactory
    {
        /// <summary>Invoked only by the readiness gated adapter preparation.</summary>
        public static IUnityStoreRuntime Create(StoreConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
#if STP_UNITY_IAP
            return new UnityIapRuntime(configuration);
#else
            return new Unavailable();
#endif
        }
#if !STP_UNITY_IAP
        private sealed class Unavailable : IUnityStoreRuntime
        {
            public bool Supported => false;
            public event Action<StoreTransaction>? TransactionReceived { add { } remove { } }
            public event Action<MobileServiceStatus>? PurchaseStopped { add { } remove { } }
            public void ConnectAndFetch(Action<StoreProduct?> completed) => completed(null);
            public void Purchase() { }
            public void Restore(Action<bool> completed) => completed(false);
            public void Verify(StoreTransaction transaction, Action<VerifiedStoreTransaction?> completed) => completed(null);
            public void Confirm(StoreTransaction transaction, Action<bool> completed) => completed(false);
            public void Dispose() { }
        }
#endif
    }
}
