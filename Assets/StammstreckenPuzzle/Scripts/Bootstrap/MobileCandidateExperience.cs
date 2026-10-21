using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;
using STP.Infrastructure.Content;
using STP.Presentation.UI;
using STP.MobileServices.Store;
using UnityEngine;

namespace STP.Bootstrap
{
    /// <summary>Product callbacks over one confirmed profile and the same serialized puzzle writer.</summary>
    public sealed partial class MobileCandidateExperience : IDisposable
    {
        private readonly MobileCandidateComposition composition;
        private readonly SavedPublic240ReviewSession saved;
        private readonly ProductionGamePresenter presenter;
        private readonly ILocalSaveDispatcher dispatcher;
        private readonly CancellationToken cancellation;
        private readonly LocalDailyRandom random=new LocalDailyRandom();
        private readonly MobileCandidateServices services;
        private Task consent=Task.CompletedTask;
        private Task userAction=Task.CompletedTask;
        private Guid skippedCompletionBonusAttempt;
        private InterstitialResultReceipt? resultReceipt;
        private bool consentStarted,disposed;
        private bool externalInputSuspended;

        public MobileCandidateExperience(MobileCandidateComposition composition,SavedPublic240ReviewSession saved,
            ProductionGamePresenter presenter,ILocalSaveDispatcher dispatcher,ILocalSessionClock clock,CancellationToken cancellation,Func<string>? endlessSeedSource=null)
        {
            this.composition=composition;this.saved=saved;this.presenter=presenter;this.dispatcher=dispatcher;this.cancellation=cancellation;
            nextEndlessSeed=endlessSeedSource??FreshEndlessSeed;
            services=new MobileCandidateServices(composition.Repository,composition.Profile,composition.Progress.CatalogSet,dispatcher,clock,
                (action,ct)=>OnOwnerAsync(()=>saved.RunProfileActionAsync((use,inner)=>use.RunExternalProfileActionAsync(action,inner),ct)),
                ApplyRewardAsync);
            presenter.BindCampaignProgress(IsCleared,saved.CanEnterPuzzle,RecommendedIndex);
            presenter.BindCosmetics(composition.Cosmetics,Owns,Equipped,CanActCosmetic,id=>TrackUserAction(ActCosmeticAsync(id)));
            presenter.BindDaily(ReadDaily,video=>TrackUserAction(ClaimDailyAsync(video)));
            presenter.BindHintVideo(CanRequestHintVideo,()=>TrackUserAction(RequestHintVideoAsync()));
            presenter.BindSettings(()=>saved.ConfirmedSnapshot?.Payload.Settings??null!,settings=>TrackUserAction(CommitAsync((use,ct)=>use.SaveSettingsAsync(settings,ct))));
            presenter.BindServices(ReadServices,action=>TrackUserAction(ActServiceAsync(action)));
            presenter.BindCampaignRecords(ReadCampaignRecord);
            presenter.BindActivePuzzleMode(()=>saved.CurrentModeName);
            presenter.BindCompletionBonus(ReadCompletionBonus,video=>TrackUserAction(RequestCompletionBonusAsync(video)));
            presenter.BindResultContinuation(()=>TrackUserAction(ContinueResultAsync()));
            BindEndless();
        }
        /// <summary>Explicit build-profile input handoff before readiness. This stores metadata only and starts no SDK.</summary>
        public void ConfigureStore(StoreConfiguration configuration,IapPrivacyReadiness? reviewedReadiness)
        {
            if(disposed||consentStarted)throw new InvalidOperationException("CANDIDATE_STORE_INPUT_WINDOW_CLOSED");
            services.ConfigureStore(configuration,reviewedReadiness);
        }
        public Task Pending=>Task.WhenAll(consent,services.PendingCallbacks,userAction);
        /// <summary>Actual native provider presentation, excluding background requests and in-game modals.</summary>
        public bool IsNativeUiBusy=>services.IsNativeUiBusy;
        /// <summary>Actual OS/focus/provider input boundary, separate from an in-game modal's attention pause.</summary>
        public void SetExternalInputSuspended(bool value){externalInputSuspended=value;}
        private Task TrackUserAction(Task task)
        {userAction=userAction.IsCompleted?task:Task.WhenAll(userAction,task);return task;}
        private LocalProfileState? State=>saved.ConfirmedSnapshot?.Payload;
        private bool IsCleared(int index)=>index>=0&&index<saved.Puzzles.Count&&State?.Levels.Any(l=>l.Identity.Equals(saved.Puzzles[index].Identity)&&l.FirstClear!=null)==true;
        private int RecommendedIndex()
        {for(int i=0;i<saved.Puzzles.Count;i++)if(!IsCleared(i)&&saved.CanEnterPuzzle(i))return i;return Math.Max(0,saved.State.SelectedIndex);}
        private bool Owns(string id)=>State?.Inventory.Ownership.Any(o=>o.ItemId==id)==true;
        private bool Equipped(string id)=>State?.Inventory.SelectedItemIds.Contains(id)==true;
        private CampaignPuzzleRecord ReadCampaignRecord(int index)
        {
            var identity=index>=0&&index<saved.Puzzles.Count?saved.Puzzles[index].Identity:null;
            var level=identity==null?null:State?.Levels.SingleOrDefault(l=>l.Identity.Equals(identity));
            return new CampaignPuzzleRecord(level?.StarsHighWater??0,level?.FirstRunBestElapsedMs,
                level?.RevisionBestElapsedMs,level?.DirectSolved??false);
        }
        private CompletionBonusView ReadCompletionBonus()
        {
            var identity=saved.State.CurrentIdentity;var state=State;var view=saved.PuzzleView;
            if(identity!=null&&view?.Phase==DevelopmentPreviewPhase.Results&&saved.CurrentModeName=="ENDLESS")return ReadEndlessBonus(identity.PuzzleId);
            if(identity==null||state==null||view==null||view.Phase!=DevelopmentPreviewPhase.Results||
                !composition.Catalog.TryGet(identity,out _))return new CompletionBonusView(false,false,false,null);
            var claim=state.RewardClaims.OfType<PostClearRewardClaim>().SingleOrDefault(c=>c.PuzzleIdentity.Equals(identity));
            bool pending=claim?.ClaimStatus=="RESERVED"||claim?.ClaimStatus=="RECONCILIATION_REQUIRED";
            bool eligible=skippedCompletionBonusAttempt!=view.AttemptId&&composition.PostClear.CanReserve(state,identity);
            return new CompletionBonusView(eligible&&services.Capabilities.CanRequestAds,eligible,pending,
                claim?.ClaimStatus=="COMMITTED"?(int?)claim.Terminal!.Amount:null);
        }
        private async Task RequestCompletionBonusAsync(bool video)
        {
            var view=saved.PuzzleView;var identity=saved.State.CurrentIdentity;
            if(view==null||identity==null||view.Phase!=DevelopmentPreviewPhase.Results)throw new InvalidOperationException("POST_CLEAR_RESULT_REQUIRED");
            if(saved.CurrentModeName=="ENDLESS"){await RequestEndlessRewardAsync(identity.PuzzleId,video);return;}
            if(!video){if(ReadCompletionBonus().CanSkip)skippedCompletionBonusAttempt=view.AttemptId;return;}
            if(!ReadCompletionBonus().CanRequestVideo)throw new InvalidOperationException("POST_CLEAR_VIDEO_UNAVAILABLE");
            var reserved=await saved.RunProfileActionAsync((use,ct)=>use.ReservePostClearAsync(composition.PostClear,identity,ct),cancellation);
            RequireCommit(reserved);
            var reservation=LocalPostClearPolicy.Reservation(reserved,identity);
            var shown=await services.ShowRewardedAsync(reservation,cancellation);
            await services.PendingCallbacks;
            await CommitAsync((use,ct)=>use.ResolvePostClearAsync(composition.PostClear,reservation,DefinitelyBeforeShow(shown.Code),ct));
        }
        private async Task ContinueResultAsync()
        {
            var view=saved.PuzzleView;
            if(disposed||view==null||view.Phase!=DevelopmentPreviewPhase.Results||!composition.Persisted.CanPresentResults)return;
            InterstitialResultReceipt receipt;
            try{receipt=resultReceipt?.AttemptId==view.AttemptId&&resultReceipt.Identity.Equals(view.Puzzle.Identity)?resultReceipt:InterstitialResultReceipt.FromCommittedResult(composition.Persisted);}
            catch(ArgumentException){return;}
            Guid attempt=view.AttemptId;
            await services.TryInterstitialAfterResultAsync(receipt,()=>!disposed&&presenter.IsPuzzleScreenActive&&
                !presenter.IsHelpOpen&&!presenter.IsProductModalOpen&&composition.Persisted.CanPresentResults&&saved.State.CanNavigate&&State!=null&&
                !PersistedMobileTransactions.HasRemoveAds(State)&&saved.State.CurrentIdentity?.Equals(receipt.Identity)==true&&saved.PuzzleView?.AttemptId==attempt&&
                saved.PuzzleView.Phase==DevelopmentPreviewPhase.Results,true,cancellation);
        }
        /// <summary>Captures the acknowledged attempt before optional completion-claim closure retires its open record.</summary>
        public void CaptureCommittedResult()
        {
            resultReceipt=null;
            try{resultReceipt=InterstitialResultReceipt.FromCommittedResult(composition.Persisted);}
            catch(ArgumentException){/* Practice or an unavailable completion has no eligible receipt. */}
        }
        private bool CanActCosmetic(string id)
        {
            var state=State;if(state==null||!saved.State.CanNavigate||!composition.Cosmetics.TryGet(id,out var item)||item==null)return false;
            if(Owns(id))return !Equipped(id)||item.Type=="LIVERY"||item.Type=="OPERATING_OBJECT";
            if(item.Acquisition=="PATIENCE_PURCHASE")return saved.State.Progress?.BalancePatience>=item.PricePatience;
            if(item.Acquisition=="DEFAULT")return true;
            return item.RequiredCampaignSubjectIds.All(subject=>composition.Progress.Campaign.Entries
                .Where(e=>e.SeasonId==subject||e.SectionId==subject||e.RouteId==subject)
                .All(e=>state.Levels.Any(l=>l.Identity.Equals(e.Puzzle)&&l.FirstClear!=null)));
        }
        private async Task ActCosmeticAsync(string id)
        {
            if(!CanActCosmetic(id)||!composition.Cosmetics.TryGet(id,out var item)||item==null)throw new InvalidOperationException("COSMETIC_ACTION_UNAVAILABLE");
            string hash=composition.Cosmetics.Identity.HashSha256;
            if(Owns(id))
            {
                var inventory=State!.Inventory;
                bool remove=Equipped(id)&&(item.Type=="LIVERY"||item.Type=="OPERATING_OBJECT");
                var selected=remove?inventory.SelectedItemIds.Where(existing=>existing!=id).ToArray():
                    inventory.SelectedItemIds.Where(existing=>existing!=id&&(item.Type=="OPERATING_OBJECT"||inventory.Ownership.Single(o=>o.ItemId==existing).ItemType!=item.Type)).Concat(new[]{id}).ToArray();
                await CommitAsync((use,ct)=>use.SelectCosmeticsAsync(composition.CosmeticPolicy,selected,hash,ct));
            }
            else if(item.Acquisition=="PATIENCE_PURCHASE")await CommitAsync((use,ct)=>use.PurchaseCosmeticAsync(composition.CosmeticPolicy,id,hash,ct));
            else if(item.Acquisition=="MILESTONE_GRANT")await CommitAsync((use,ct)=>use.ClaimMilestoneCosmeticAsync(composition.CosmeticPolicy,id,hash,ct));
            else await CommitAsync((use,ct)=>use.GrantCosmeticDefaultsAsync(composition.CosmeticPolicy,hash,ct));
        }
        public async Task EnsureReadyAsync()
        {
            if(disposed||State==null)return;
            if(composition.Cosmetics.Items.Any(i=>i.Acquisition=="DEFAULT"&&!Owns(i.Id)))
                await CommitAsync((use,ct)=>use.GrantCosmeticDefaultsAsync(composition.CosmeticPolicy,composition.Cosmetics.Identity.HashSha256,ct));
            if(!consentStarted){consentStarted=true;consent=RefreshConsentObservedAsync();}
        }
        private async Task RefreshConsentObservedAsync()
        {try{await services.RefreshConsentAsync(cancellation);}catch(Exception){/* Service unavailability leaves the offline game usable. */}}
        private string DailyDate()
        {
            var pending=State?.Metagame?.Daily.Where(d=>d.Terminal==null&&d.Reward?.State!="CLOSED_NO_REWARD")
                .OrderBy(d=>d.Date,StringComparer.Ordinal).FirstOrDefault();
            return pending?.Date??DateTime.Now.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture);
        }
        private DailyView ReadDaily()
        {
            string date=DailyDate();var records=State?.Metagame?.Daily.Where(d=>d.Date==date).ToArray()??Array.Empty<LocalDailyClaim>();
            var free=records.SingleOrDefault(d=>d.Slot=="FREE");var video=records.SingleOrDefault(d=>d.Slot=="REWARDED");
            var terminal=video?.Terminal??free?.Terminal;
            bool pending=records.Any(d=>d.Terminal==null&&d.Reward?.State!="CLOSED_NO_REWARD");
            return new DailyView(date,State!=null&&free?.Terminal==null,
                State!=null&&free?.Terminal!=null&&video?.Terminal==null&&services.Capabilities.CanRequestAds&&
                (video==null||video.Reward?.State=="CLOSED_NO_REWARD"),terminal==null?(int?)null:(int)terminal.Amount,pending);
        }
        private async Task ClaimDailyAsync(bool video)
        {
            string date=DailyDate();
            if(!video){await CommitAsync((use,ct)=>use.ClaimFreeDailyAsync(composition.Metagame,random,date,ct));return;}
            var reserved=await saved.RunProfileActionAsync((use,ct)=>use.ReserveDailyRewardAsync(composition.Metagame,random,date,Guid.NewGuid().ToString("D"),ct),cancellation);
            RequireCommit(reserved);
            var reservation=LocalMetagameTransactions.RewardReservation(reserved,"daily:v1:"+date+":REWARDED");
            var shown=await services.ShowRewardedAsync(reservation,cancellation);
            bool definitelyNoReward=DefinitelyBeforeShow(shown.Code);
            await CommitAsync((use,ct)=>use.ResolveDailyRewardAsync(composition.Metagame,reservation,definitelyNoReward,ct));
        }
        private bool CanRequestHintVideo()
        {
            var identity=saved.State.CurrentIdentity;var state=State;
            if(identity==null||state==null||!services.Capabilities.CanRequestAds||!presenter.IsHelpOpen)return false;
            var hint=state.Metagame?.Hints.SingleOrDefault(h=>h.Identity.Equals(identity));
            return composition.Metagame.HintCredits(state,identity)==0&&(hint?.Reward==null||hint.Reward.State=="CLOSED_NO_REWARD");
        }
        private async Task RequestHintVideoAsync()
        {
            if(!CanRequestHintVideo())throw new InvalidOperationException("HINT_VIDEO_NOT_AVAILABLE");
            var identity=saved.State.CurrentIdentity!;
            var reserved=await saved.RunProfileActionAsync((use,ct)=>use.ReserveHintRewardAsync(composition.Metagame,identity,Guid.NewGuid().ToString("D"),ct),cancellation);
            RequireCommit(reserved);
            var reservation=LocalMetagameTransactions.RewardReservation(reserved,LocalHintRecord.ClaimId(identity));
            var shown=await services.ShowRewardedAsync(reservation,cancellation);
            await services.PendingCallbacks;
            await CommitAsync((use,ct)=>use.ResolveHintRewardAsync(composition.Metagame,identity,reservation,DefinitelyBeforeShow(shown.Code),ct));
        }
        private Task<SaveCommitResult<LocalProfileState>> ApplyRewardAsync(AdRewardEvidence evidence,CancellationToken ct)
            =>OnOwnerAsync(()=>
            {
                if(evidence.Reservation.ClaimId.StartsWith("daily:v1:",StringComparison.Ordinal))
                    return saved.RunProfileActionAsync((use,inner)=>use.ConfirmDailyRewardAsync(composition.Metagame,evidence,inner),ct);
                if(evidence.Reservation.OperationId.StartsWith("endless-provider-v1:",StringComparison.Ordinal)&&composition.EndlessPolicy!=null)
                    return saved.RunProfileActionAsync((use,inner)=>use.ConfirmEndlessRewardAsync(composition.EndlessPolicy,evidence,inner),ct);
                if(evidence.Reservation.ClaimId.StartsWith("reward-claim:POST_CLEAR_PATIENCE:",StringComparison.Ordinal))
                    return saved.RunProfileActionAsync((use,inner)=>use.ConfirmPostClearAsync(composition.PostClear,evidence,inner),ct);
                var hint=State?.Metagame?.Hints.SingleOrDefault(h=>h.Reward?.ClaimId==evidence.Reservation.ClaimId);
                if(hint==null)return Task.FromResult(SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed,"ADS_CLAIM_NOT_ACTIVE"));
                return saved.RunProfileActionAsync((use,inner)=>use.ConfirmHintRewardAsync(composition.Metagame,hint.Identity,evidence,inner),ct);
            });
        private ProductionServiceState ReadServices()
        {
            bool owned=State?.Entitlements.Records.Any(e=>e.ProductKey=="remove_ads"&&e.Owned&&!e.RevocationConfirmed)==true;
            bool ready=State!=null&&!externalInputSuspended&&saved.CanRunProfileAction;
            return new ProductionServiceState(ready,ready&&services.CanLoadStorePrice,
                owned,ready&&!owned&&services.CanPurchaseDisplayedProduct,services.Product?.LocalizedPrice,ready&&!owned&&services.CanLoadStorePrice);
        }
        private async Task ActServiceAsync(ProductionServiceAction action)
        {
            switch(action)
            {
                case ProductionServiceAction.ReviewPrivacy:
                    var privacy=await (services.Capabilities.PrivacyOptionsRequired?services.ShowPrivacyOptionsAsync(cancellation):services.RefreshConsentAsync(cancellation));
                    if(privacy.Code=="CONSENT_CURRENT_UPDATE_REQUIRED")privacy=await services.RefreshConsentAsync(cancellation);
                    if(privacy.Status!=MobileServiceStatus.Succeeded)throw new InvalidOperationException(privacy.Code);break;
                case ProductionServiceAction.RestorePurchases:
                    var restored=await services.RestoreAsync(cancellation);
                    if(restored.Status!=MobileServiceStatus.Succeeded)throw new InvalidOperationException(restored.Code);break;
                case ProductionServiceAction.LoadStorePrice:
                    var price=await services.LoadStorePriceAsync(cancellation);
                    if(price.Status!=MobileServiceStatus.Succeeded)throw new InvalidOperationException(price.Code);break;
                case ProductionServiceAction.PurchaseRemoveAds:
                    var displayed=services.Product;
                    if(displayed==null||!ReadServices().CanPurchaseRemoveAds)throw new InvalidOperationException("IAP_DISPLAYED_OFFER_REQUIRED");
                    var purchased=await services.PurchaseRemoveAdsAsync(displayed,cancellation);
                    if(purchased.Status!=MobileServiceStatus.Succeeded)throw new InvalidOperationException(purchased.Code);break;
                default:throw new InvalidOperationException("PUBLISHER_SUPPORT_PRIVACY_CONTACT_REQUIRED");
            }
        }
        private async Task CommitAsync(Func<PersistedPuzzleSession,CancellationToken,Task<SaveCommitResult<LocalProfileState>>> action)
        {RequireCommit(await saved.RunProfileActionAsync(action,cancellation));presenter.Refresh();}
        private static void RequireCommit(SaveCommitResult<LocalProfileState> result)
        {if(!result.Succeeded)throw new IOException(result.ErrorCode??"METAGAME_SAVE_NOT_COMMITTED");}
        private static bool DefinitelyBeforeShow(string code)=>new[]{"ADS_NO_FILL","ADS_INITIALIZATION_FAILED","ADS_INITIALIZATION_EXCEPTION",
            "ADS_LOAD_EXCEPTION","ADS_CAPABILITY_REQUIRED","ADS_EXPIRED","ADS_HANDLE_UNKNOWN","ADS_CONSENT_CHANGED","ADS_BUSY"}.Contains(code);
        private async Task<SaveCommitResult<LocalProfileState>> OnOwnerAsync(Func<Task<SaveCommitResult<LocalProfileState>>> action)
        {
            Task<SaveCommitResult<LocalProfileState>>? task=null;
            await dispatcher.InvokeAsync(()=>task=action()).ConfigureAwait(false);
            return await task!.ConfigureAwait(false);
        }
        public void Dispose(){if(disposed)return;disposed=true;services.Dispose();}
    }
}
