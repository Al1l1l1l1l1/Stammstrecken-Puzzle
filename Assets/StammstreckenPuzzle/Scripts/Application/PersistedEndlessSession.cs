using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Puzzle.Domain;

namespace STP.Application
{
    public sealed partial class PersistedPuzzleSession
    {
        private LocalEndlessPolicy? endlessPolicy;
        private int? committedEndlessStars;
        internal LocalEndlessPolicy? ConfiguredEndlessPolicy=>endlessPolicy;
        /// <summary>Binds the actual locked internal endless policy once during composition, before loading a profile.</summary>
        public void ConfigureEndless(LocalEndlessPolicy value)
        {LocalModel.Require(profile.Use==CatalogUse.InternalPreview&&Status==PersistedSessionStatus.Unloaded&&endlessPolicy==null,"ENDLESS_COMPOSITION_NOT_ALLOWED");endlessPolicy=value??throw new ArgumentNullException(nameof(value));}
        /// <summary>Actual acknowledged result stars; current DRAFT profiles have no invented time thresholds.</summary>
        public int? CurrentEndlessStars=>CanPresentResults&&session?.Mode==SessionMode.ENDLESS?committedEndlessStars:null;
        public Task<SaveCommitResult<LocalProfileState>> ReserveEndlessAsync(LocalEndlessPolicy endless,EndlessSearchProfile target,string seed,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>endless.Reserve(s,target,seed),ct);
        public async Task<SaveCommitResult<LocalProfileState>> GenerateEndlessAsync(LocalEndlessPolicy endless,string id,CancellationToken ct)
        {
            var source=await Capture(()=>{LocalModel.Require(!blocked&&!needsPromotion&&durable!=null&&ReferenceEquals(endlessPolicy,endless),"ENDLESS_SESSION_NOT_READY");return durable!;}).ConfigureAwait(false);
            LocalEndlessCandidate? candidate;
            try{candidate=await endless.GenerateAsync(source,id,ct).ConfigureAwait(false);}
            catch(OperationCanceledException){return SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Cancelled,"ENDLESS_GENERATION_CANCELLED");}
            if(candidate==null)return SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed,"ENDLESS_GENERATION_RETRY_REQUIRED");
            try{return await ProfileMutationAsync((s,g,utc)=>endless.Promote(s,candidate,Guid.NewGuid()),ct).ConfigureAwait(false);}
            catch(ArgumentException e)when(e.Message=="ENDLESS_PROMOTION_NOVELTY_CONFLICT"){return SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed,"ENDLESS_PROMOTION_NOVELTY_CONFLICT");}
        }
        public Task<SaveCommitResult<LocalProfileState>> AbandonEndlessAsync(LocalEndlessPolicy endless,string id,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>endless.Abandon(s,id),ct);
        public Task<SaveCommitResult<LocalProfileState>> RestartEndlessAsync(LocalEndlessPolicy endless,string id,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>endless.RestartDraft(s,id,Guid.NewGuid()),ct);
        public Task<SaveCommitResult<LocalProfileState>> SkipEndlessRewardAsync(LocalEndlessPolicy endless,string id,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>endless.SkipReward(s,id),ct);
        public Task<SaveCommitResult<LocalProfileState>> ReserveEndlessRewardAsync(LocalEndlessPolicy endless,string id,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>endless.ReserveReward(s,id,g),ct);
        public Task<SaveCommitResult<LocalProfileState>> ConfirmEndlessRewardAsync(LocalEndlessPolicy endless,AdRewardEvidence evidence,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>endless.ConfirmReward(s,evidence,g,utc),ct);
        public Task<SaveCommitResult<LocalProfileState>> ResolveEndlessRewardAsync(LocalEndlessPolicy endless,AdRewardReservation reservation,bool definitelyNoReward,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>endless.ResolveReward(s,reservation,definitelyNoReward),ct);
        /// <summary>Restores only one actually issued active descriptor; no campaign catalog membership is manufactured.</summary>
        public PuzzleSessionState OpenEndless(string id)
        {
            LocalModel.Require(!blocked&&!needsPromotion&&durable!=null&&endlessPolicy!=null&&(session==null||session.Phase!=SessionPhase.SolvedPendingCommit||completionCommitted),"ENDLESS_SESSION_NOT_READY");
            var active=durable!.Payload.Endless.OpenEndless.OfType<ActiveEndlessDraft>().SingleOrDefault(a=>a.Id==id)??throw new ArgumentException("ENDLESS_DRAFT_REQUIRED");
            var restored=active.Checkpoint.Restore(active.PublicPuzzleInput.ToDomainDefinition(),id,active.PublicPuzzleHash.Profile,active.PublicPuzzleHash.Sha256);
            session=restored.Value??throw new ArgumentException("ENDLESS_DRAFT_INVALID");restoredCheckpoint=active.Checkpoint;completionCommitted=false;committedEndlessStars=null;Status=PersistedSessionStatus.Ready;return session;
        }
        internal void RetireEndless(string id)
        {if(session?.Mode==SessionMode.ENDLESS&&session.PuzzleId==id){session=null;restoredCheckpoint=null;completionCommitted=false;committedEndlessStars=null;committedAttempt=null;committedPuzzle=null;Status=PersistedSessionStatus.Ready;}}
        private async Task<SaveCommitResult<LocalProfileState>> CompleteEndlessUnderLock(LocalProfileState source,PuzzleSessionState solved,string utc,CancellationToken ct)
        {
            LocalModel.Require(endlessPolicy!=null,"ENDLESS_COMPOSITION_REQUIRED");
            if(source.Endless.OpenEndless.OfType<OpenEndlessCompletion>().Any(c=>c.Id==solved.PuzzleId&&c.CompletionCommitId==solved.SolvedEventId)&&CanPresentResults)return SaveCommitResult<LocalProfileState>.Success(durable!);
            var payload=await Task.Run(()=>endlessPolicy!.Complete(source,solved,checked(durable!.Generation+1),utc),ct).ConfigureAwait(false);
            var result=await CommitUnderLock(payload,true,ct,null,solved).ConfigureAwait(false);
            if(result.Succeeded)await dispatcher.InvokeAsync(()=>committedEndlessStars=1).ConfigureAwait(false);
            return result;
        }
    }
}
