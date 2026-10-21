using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace STP.Application
{
    public sealed partial class SavedPublic240ReviewSession
    {
        public Task<SaveCommitResult<LocalProfileState>> StartEndlessAsync(LocalEndlessPolicy policy,EndlessSearchProfile profile,string seed,CancellationToken ct)
            =>RunProfileActionAsync(async(use,token)=>
            {
                var reserved=await use.ReserveEndlessAsync(policy,profile,seed,token).ConfigureAwait(false);if(!reserved.Succeeded||reserved.Snapshot==null)return reserved;
                string id=reserved.Snapshot.Payload.Endless.OpenEndless.Last().Id;var generated=await use.GenerateEndlessAsync(policy,id,token).ConfigureAwait(false);
                if(generated.Succeeded)await OnOwner(()=>BindEndlessView(use,id)).ConfigureAwait(false);return AcknowledgedGenerationOutcome(generated,reserved.Snapshot);
            },ct);
        public Task<SaveCommitResult<LocalProfileState>> ResumeEndlessAsync(LocalEndlessPolicy policy,string id,CancellationToken ct)
            =>RunProfileActionAsync(async(use,token)=>
            {
                var snapshot=await use.CaptureCurrentEndlessAsync(token).ConfigureAwait(false);
                if(snapshot.Payload.Endless.OpenEndless.Any(r=>r.Id==id&&r is ReservedEndless))
                {var generated=await use.GenerateEndlessAsync(policy,id,token).ConfigureAwait(false);if(!generated.Succeeded)return AcknowledgedGenerationOutcome(generated,snapshot);snapshot=generated.Snapshot!;}
                if(snapshot.Payload.Endless.OpenEndless.Any(r=>r.Id==id&&r is ActiveEndlessDraft))await OnOwner(()=>BindEndlessView(use,id)).ConfigureAwait(false);
                else if(!snapshot.Payload.Endless.OpenEndless.Any(r=>r.Id==id&&r is OpenEndlessCompletion))throw new ArgumentException("ENDLESS_OPEN_RECORD_REQUIRED");
                return SaveCommitResult<LocalProfileState>.Success(snapshot);
            },ct);
        private SaveCommitResult<LocalProfileState> AcknowledgedGenerationOutcome(SaveCommitResult<LocalProfileState> result,SaveSnapshot<LocalProfileState> reserved)
        {
            if(result.ErrorCode=="ENDLESS_GENERATION_RETRY_REQUIRED"||result.ErrorCode=="ENDLESS_GENERATION_CANCELLED"||result.ErrorCode=="ENDLESS_PROMOTION_NOVELTY_CONFLICT")
            {endlessGenerationStatus=result.ErrorCode=="ENDLESS_GENERATION_CANCELLED"?"ready":result.ErrorCode=="ENDLESS_PROMOTION_NOVELTY_CONFLICT"?"unavailable":"budget";return SaveCommitResult<LocalProfileState>.Success(reserved);}
            endlessGenerationStatus=result.Succeeded?"ready":"failed";return result;
        }
        private string endlessGenerationStatus="ready";
        public string EndlessGenerationStatus{get{RequireOwner();return endlessGenerationStatus;}}
        public Task<SaveCommitResult<LocalProfileState>> AbandonEndlessAsync(LocalEndlessPolicy policy,string id,CancellationToken ct)
            =>RunProfileActionAsync(async(use,token)=>
            {
                var result=await use.AbandonEndlessAsync(policy,id,token).ConfigureAwait(false);
                if(result.Succeeded)await OnOwner(()=>{if(view?.Puzzle.Identity.PuzzleId==id){use.RetireEndless(id);ClearView();}}).ConfigureAwait(false);return result;
            },ct);
        private Task<SaveCommitResult<LocalProfileState>> RestartCurrentEndlessAsync(CancellationToken ct)
        {
            RequireOwner();if(!State.CanRestart){error="SAVE_REVIEW_ACTION_NOT_ALLOWED";Publish();return Task.FromResult(SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed,error));}string id=view!.Puzzle.Identity.PuzzleId;var policy=persisted.ConfiguredEndlessPolicy??throw new ArgumentException("ENDLESS_COMPOSITION_REQUIRED");
            return RunProfileActionAsync(async(use,token)=>{var result=await use.RestartEndlessAsync(policy,id,token).ConfigureAwait(false);if(result.Succeeded)await OnOwner(()=>BindEndlessView(use,id)).ConfigureAwait(false);return result;},ct);
        }
        private void BindEndlessView(PersistedPuzzleSession use,string id)
        {
            var active=use.Snapshot!.Payload.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single(a=>a.Id==id);var state=use.OpenEndless(id);ClearView();
            view=new BoundView(this,active.PublicPuzzleInput,state,clock.MonotonicMilliseconds);completionAttempted=false;writeFailure=false;requestFault=false;
            if(suspended||presentationPaused)view.Lifecycle(true,clock.MonotonicMilliseconds);else if(view.IsPaused)view.Lifecycle(false,clock.MonotonicMilliseconds);
            status=SavedReviewStatus.Ready;SyncView();Publish();
        }
    }
    public sealed partial class PersistedPuzzleSession
    {
        internal Task<SaveSnapshot<LocalProfileState>> CaptureCurrentEndlessAsync(CancellationToken ct)
        {ct.ThrowIfCancellationRequested();return Capture(()=>{LocalModel.Require(!blocked&&!needsPromotion&&durable!=null,"ENDLESS_SESSION_NOT_READY");return durable!;});}
    }
}
