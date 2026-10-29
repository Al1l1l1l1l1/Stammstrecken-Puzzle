using System;

namespace STP.Presentation.UI
{
    /// <summary>Confirmed local daily projection. The UI never rolls or grants an outcome.</summary>
    public sealed class DailyView
    {
        /// <summary>Creates a projection; confirmedAmount is present only after a durable grant.</summary>
        public DailyView(string dateLabel,bool freeAvailable,bool videoAvailable,int? confirmedAmount,bool pendingClaim)
        {
            DateLabel=dateLabel??throw new ArgumentNullException(nameof(dateLabel));FreeAvailable=freeAvailable;VideoAvailable=videoAvailable;
            if(confirmedAmount.HasValue&&confirmedAmount!=10&&confirmedAmount!=15&&confirmedAmount!=25)throw new ArgumentOutOfRangeException(nameof(confirmedAmount));
            ConfirmedAmount=confirmedAmount;PendingClaim=pendingClaim;
        }
        /// <summary>Root-formatted current local date. Older saved claims keep their own reservation date.</summary>
        public string DateLabel{get;}
        /// <summary>Persistent policy permits a free operation or continuation.</summary>
        public bool FreeAvailable{get;}
        /// <summary>Policy and provider permit the one optional video operation.</summary>
        public bool VideoAvailable{get;}
        /// <summary>Actual durably committed amount, absent before acknowledgement.</summary>
        public int? ConfirmedAmount{get;}
        /// <summary>Existing prepared claim must be resolved without rerolling.</summary>
        public bool PendingClaim{get;}
    }
    /// <summary>Explicit native service intent; Root retains provider and entitlement authority.</summary>
    public enum ProductionServiceAction { ReviewPrivacy, RestorePurchases, PurchaseRemoveAds, ShowLegal, ContactSupport, LoadStorePrice }
    /// <summary>Actual Store, privacy and publisher-contact readiness, including a price supplied by the Store.</summary>
    public sealed class ProductionServiceState
    {
        /// <summary>Creates an actual provider projection, never a proposed successful purchase.</summary>
        public ProductionServiceState(bool canReviewPrivacy,bool canRestore,bool removeAdsOwned,bool canPurchaseRemoveAds,string? localizedRemoveAdsPrice,bool canLoadStorePrice=false,bool canShowLegal=false,bool canContactSupport=false)
        {
            CanReviewPrivacy=canReviewPrivacy;CanRestore=canRestore;RemoveAdsOwned=removeAdsOwned;CanPurchaseRemoveAds=canPurchaseRemoveAds;LocalizedRemoveAdsPrice=localizedRemoveAdsPrice;CanLoadStorePrice=canLoadStorePrice;CanShowLegal=canShowLegal;CanContactSupport=canContactSupport;
            if(canPurchaseRemoveAds&&(removeAdsOwned||string.IsNullOrWhiteSpace(localizedRemoveAdsPrice)))throw new ArgumentException("PRODUCT_STORE_STATE_INVALID");
        }
        /// <summary>Privacy-options service is callable.</summary>
        public bool CanReviewPrivacy{get;}
        /// <summary>Restore service is callable.</summary>
        public bool CanRestore{get;}
        /// <summary>Actually validated non-consumable entitlement.</summary>
        public bool RemoveAdsOwned{get;}
        /// <summary>Store permits this explicit purchase with the shown price.</summary>
        public bool CanPurchaseRemoveAds{get;}
        /// <summary>Actual localized Store price, absent while unavailable.</summary>
        public string? LocalizedRemoveAdsPrice{get;}
        /// <summary>Root permits an explicit Store price request without authorizing a purchase.</summary>
        public bool CanLoadStorePrice{get;}
        /// <summary>Root has validated the actual legal URL and permits this explicit open action.</summary>
        public bool CanShowLegal{get;}
        /// <summary>Root has validated the actual support URL and permits this explicit open action.</summary>
        public bool CanContactSupport{get;}
    }
    /// <summary>Root-owned local profile deletion projection; no state is inferred from a completed task.</summary>
    public enum ProductionLocalDataState { Unavailable, Ready, Confirmation, Deleting, RetryRequired, Deleted }
    /// <summary>Explicit local deletion intent. Retry is the same Root-held operation, never a fresh confirmation.</summary>
    public enum ProductionLocalDataAction { Open, Cancel, Confirm, Retry, Restart }
    /// <summary>Immutable status without file paths, epochs, Store state or deletion authority.</summary>
    public sealed class ProductionLocalDataView
    {
        /// <summary>Creates a closed, typed Root read model.</summary>
        public ProductionLocalDataView(ProductionLocalDataState state)
        {if(!Enum.IsDefined(typeof(ProductionLocalDataState),state))throw new ArgumentOutOfRangeException(nameof(state));State=state;}
        /// <summary>Actual Root lifecycle state, distinct from transient UI task completion.</summary>
        public ProductionLocalDataState State{get;}
    }
    /// <summary>Localized player text for one exact canonical campaign slot.</summary>
    public sealed class ProductionPuzzleDisplay
    {
        /// <summary>Creates display-only text, with no puzzle or persistence authority.</summary>
        public ProductionPuzzleDisplay(string section,string route,string title,string status,string result)
        {Section=section;Route=route;Title=title;Status=status;Result=result;}
        /// <summary>Campaign section text.</summary>
        public string Section{get;}
        /// <summary>Campaign route text.</summary>
        public string Route{get;}
        /// <summary>Exact puzzle title.</summary>
        public string Title{get;}
        /// <summary>Contextual satire, without an additional puzzle rule.</summary>
        public string Status{get;}
        /// <summary>Completion text shown only after the actual committed ride.</summary>
        public string Result{get;}
    }
}
