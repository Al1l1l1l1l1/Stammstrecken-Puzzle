#if STP_UNITY_IAP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;
using UnityEngine;
using UnityEngine.Purchasing;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Purchasing.Security;
#endif

namespace STP.MobileServices.Store
{
    /// <summary>Actual Unity IAP5.4.3 native-store calls; constructor does not create a StoreController.</summary>
    public sealed class UnityIapRuntime : IUnityStoreRuntime, IUnityStoreNativeUiState
    {
        private readonly StoreConfiguration configuration;
        private readonly Dictionary<string, Action<bool>> confirmations = new Dictionary<string, Action<bool>>(StringComparer.Ordinal);
        private StoreController? controller;
        private Action<StoreProduct?>? preparing;
        private Action<bool>? restoring;
        private volatile bool disposed;
        private volatile bool nativePurchaseUiPossible;
        private object? nativeRestorePresentation;
        /// <summary>Read-only possible native presentation, not connection, catalog fetch, receipt verification or deferred payment.</summary>
        public bool IsNativeUiBusy => !disposed && (nativePurchaseUiPossible || Volatile.Read(ref nativeRestorePresentation) != null);
        /// <summary>Captures only immutable configuration; does not initialize any UGS service.</summary>
        public UnityIapRuntime(StoreConfiguration configuration) { this.configuration = configuration; }
        /// <summary>Never uses the fake editor store or a different installed app identity.</summary>
        public bool Supported => UnityEngine.Application.identifier == configuration.ApplicationId &&
            ((UnityEngine.Application.platform == RuntimePlatform.Android && configuration.Store == "GooglePlay") ||
             (UnityEngine.Application.platform == RuntimePlatform.IPhonePlayer && configuration.Store == "AppleAppStore"));
        /// <summary>Native candidates still require Verify before Application sees them.</summary>
        public event Action<StoreTransaction>? TransactionReceived;
        /// <summary>Redacted purchase outcome.</summary>
        public event Action<MobileServiceStatus>? PurchaseStopped;

        /// <summary>Constructs the controller only inside the readiness-gated call chain.</summary>
        public void ConnectAndFetch(Action<StoreProduct?> completed)
        {
            if (!Supported || disposed) { completed(null); return; }
            preparing = completed;
            if (controller == null)
            {
                controller = new StoreController(configuration.Store);
                controller.SetStoreReconnectionRetryPolicyOnDisconnection(null);
                controller.OnProductsFetched += ProductsFetched;
                controller.OnProductsFetchFailed += ProductsFailed;
                controller.OnStoreDisconnected += Disconnected;
                controller.OnPurchasePending += Pending;
                controller.OnPurchaseFailed += PurchaseFailed;
                controller.OnPurchaseDeferred += Deferred;
                controller.OnPurchasesFetched += PurchasesFetched;
                controller.OnPurchasesFetchFailed += PurchasesFailed;
                controller.OnPurchaseConfirmed += Confirmed;
                var apple = controller.AppleStoreExtendedPurchaseService;
                // IAP5.4.3 AppleStoreImpl defaults m_RefreshAppReceipt=false; StoreKit2 needs no legacy setter.
                if (apple != null) apple.OnEntitlementRevoked += AppleRevoked;
            }
            _ = ConnectCore();
        }
        private async Task ConnectCore()
        {
            try
            {
                var store = controller;
                if (store == null) { FinishPrepare(null); return; }
                await store.Connect();
                if (disposed) return;
                store.FetchProductsWithNoRetries(new List<ProductDefinition> { new ProductDefinition("remove_ads", configuration.StoreProductId, ProductType.NonConsumable) });
            }
            catch { FinishPrepare(null); }
        }
        private void ProductsFetched(List<Product> products)
        {
            var product = products.SingleOrDefault(p => p.definition.id == "remove_ads" && p.definition.storeSpecificId == configuration.StoreProductId && p.definition.type == ProductType.NonConsumable);
            if (product == null || !product.availableToPurchase) { FinishPrepare(null); return; }
            FinishPrepare(new StoreProduct(configuration.StoreProductId, product.metadata.localizedPriceString, product.metadata.localizedTitle));
            controller?.FetchPurchases();
        }
        private void ProductsFailed(ProductFetchFailed _) => FinishPrepare(null);
        private void Disconnected(StoreConnectionFailureDescription _) { FinishPrepare(null); FinishRestore(false); }
        private void FinishPrepare(StoreProduct? product) { var callback = preparing; preparing = null; callback?.Invoke(product); }
        /// <summary>Only one configured non-consumable is offered.</summary>
        public void Purchase() { if (controller == null || disposed) throw new InvalidOperationException("IAP_NOT_CONNECTED"); nativePurchaseUiPossible = true; controller.PurchaseProduct("remove_ads"); }
        private void Pending(PendingOrder order) { nativePurchaseUiPossible = false; Emit(order, false); }
        private void PurchaseFailed(FailedOrder order) { nativePurchaseUiPossible = false; PurchaseStopped?.Invoke(order.FailureReason == PurchaseFailureReason.UserCancelled ? MobileServiceStatus.Cancelled : MobileServiceStatus.Failed); }
        private void Deferred(DeferredOrder _) { nativePurchaseUiPossible = false; PurchaseStopped?.Invoke(MobileServiceStatus.Pending); }
        private void Emit(Order order, bool finalized)
        {
            if (disposed) return;
            var items = order.CartOrdered.Items().ToList();
            if (items.Count != 1) return;
            var product = items[0].Product;
            if (product.definition.type != ProductType.NonConsumable || product.definition.id != "remove_ads" || product.definition.storeSpecificId != configuration.StoreProductId || string.IsNullOrWhiteSpace(order.Info.TransactionID)) return;
            TransactionReceived?.Invoke(new StoreTransaction(order.Info.TransactionID, configuration.StoreProductId, finalized, order));
        }
        private void PurchasesFetched(Orders orders)
        {
            foreach (var order in orders.ConfirmedOrders) Emit(order, true);
            foreach (var order in orders.PendingOrders) Emit(order, false);
            FinishRestore(true);
        }
        private void PurchasesFailed(PurchasesFetchFailureDescription _) => FinishRestore(false);
        /// <summary>Real RestoreTransactions followed by FetchPurchases; empty result never revokes.</summary>
        public void Restore(Action<bool> completed)
        {
            if (controller == null || disposed) { completed(false); return; }
            restoring = completed;
            // Google Play RestoreTransactions only queries purchases; Apple can present store authentication UI.
            var presentation = configuration.Store == "AppleAppStore" ? new object() : null;
            Interlocked.Exchange(ref nativeRestorePresentation, presentation);
            controller.RestoreTransactions((ok, _) =>
            {
                if (presentation != null) Interlocked.CompareExchange(ref nativeRestorePresentation, null, presentation);
                if (!ok || disposed) { FinishRestore(false); return; }
                controller?.FetchPurchases();
            });
        }
        private void FinishRestore(bool ok) { var callback = restoring; restoring = null; callback?.Invoke(ok); }

        /// <summary>Local official Google signature validation or explicit native StoreKit2 verified transaction.</summary>
        public void Verify(StoreTransaction transaction, Action<VerifiedStoreTransaction?> completed)
        {
            if (!Supported || disposed || transaction.ProductId != configuration.StoreProductId) { completed(null); return; }
            if (configuration.Store == "AppleAppStore")
            {
                AppleStoreKitVerifier.Verify(transaction.NativeId, configuration.ApplicationId, configuration.StoreProductId, configuration.Production, completed);
                return;
            }
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                if (!(transaction.NativeHandle is Order order) || string.IsNullOrWhiteSpace(configuration.GooglePublicKey)) { completed(null); return; }
                var validator = new CrossPlatformValidator(Convert.FromBase64String(configuration.GooglePublicKey), configuration.ApplicationId);
                var receipts = validator.Validate(order.Info.Receipt);
                var receipt = receipts.OfType<GooglePlayReceipt>().SingleOrDefault(r => r.productID == configuration.StoreProductId && r.packageName == configuration.ApplicationId && r.purchaseToken == transaction.NativeId);
                if (receipt == null || (receipt.purchaseState != GooglePurchaseState.Purchased && receipt.purchaseState != GooglePurchaseState.Refunded)) { completed(null); return; }
                completed(new VerifiedStoreTransaction(receipt.purchaseToken, configuration.Production ? "Production" : "Sandbox", receipt.purchaseState == GooglePurchaseState.Refunded));
            }
            catch { completed(null); }
#else
            completed(null);
#endif
        }
        private void AppleRevoked(string productId)
        {
            if (productId == configuration.StoreProductId)
                TransactionReceived?.Invoke(new StoreTransaction(string.Empty, productId, true, null));
        }
        /// <summary>Uses only the original SDK PendingOrder; FailedOrder never counts as success.</summary>
        public void Confirm(StoreTransaction transaction, Action<bool> completed)
        {
            if (controller == null || disposed || !(transaction.NativeHandle is PendingOrder pending) || confirmations.ContainsKey(transaction.NativeId)) { completed(false); return; }
            confirmations.Add(transaction.NativeId, completed);
            try { controller.ConfirmPurchase(pending); }
            catch { confirmations.Remove(transaction.NativeId); completed(false); }
        }
        private void Confirmed(Order order)
        {
            if (!confirmations.TryGetValue(order.Info.TransactionID, out var callback)) return;
            confirmations.Remove(order.Info.TransactionID);
            callback(order is ConfirmedOrder);
            // Explicit ownership reconciliation, rather than treating a FailedOrder as acknowledgement.
            if (order is FailedOrder) controller?.FetchPurchases();
        }
        /// <summary>Unsubscribes only; never auto-confirms ungranted transactions.</summary>
        public void Dispose()
        {
            disposed = true;
            if (controller == null) return;
            controller.OnProductsFetched -= ProductsFetched; controller.OnProductsFetchFailed -= ProductsFailed;
            controller.OnStoreDisconnected -= Disconnected; controller.OnPurchasePending -= Pending;
            controller.OnPurchaseFailed -= PurchaseFailed; controller.OnPurchaseDeferred -= Deferred;
            controller.OnPurchasesFetched -= PurchasesFetched; controller.OnPurchasesFetchFailed -= PurchasesFailed;
            controller.OnPurchaseConfirmed -= Confirmed;
            var apple = controller.AppleStoreExtendedPurchaseService;
            if (apple != null) apple.OnEntitlementRevoked -= AppleRevoked;
            confirmations.Clear(); preparing = null; restoring = null;
        }
    }
}
#endif
