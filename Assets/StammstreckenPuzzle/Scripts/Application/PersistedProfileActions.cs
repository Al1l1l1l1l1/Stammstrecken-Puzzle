using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace STP.Application
{
    public sealed partial class PersistedPuzzleSession
    {
        private async Task<SaveCommitResult<LocalProfileState>> ProfileMutationAsync(Func<LocalProfileState,long,string,LocalProfileState> project,CancellationToken ct)
        {
            await writer.WaitAsync(ct).ConfigureAwait(false);
            try { return await ProfileMutationUnderLock(project,ct).ConfigureAwait(false); }
            finally { writer.Release(); }
        }
        private async Task<SaveCommitResult<LocalProfileState>> ProfileMutationUnderLock(Func<LocalProfileState,long,string,LocalProfileState> project,CancellationToken ct)
        {
            var context=await Capture(()=>
            {
                LocalModel.Require(profile.Use!=CatalogUse.IsolatedReview&&!blocked&&!needsPromotion&&durable!=null&&(session==null||session.Phase!=STP.Puzzle.Domain.SessionPhase.SolvedPendingCommit||completionCommitted),"METAGAME_SESSION_NOT_READY");
                LocalModel.Require(durable!.Generation<LocalModel.MaxSafe,"SAVE_GENERATION_EXHAUSTED");
                var prior=CanPresentResults;blocked=true;
                return(Snapshot:durable,Session:session,Presented:prior,Utc:clock.UtcNow);
            }).ConfigureAwait(false);
            LocalProfileState payload;
            try { payload=await Task.Run(()=>project(context.Snapshot.Payload,context.Snapshot.Generation+1,context.Utc),ct).ConfigureAwait(false); }
            catch { await dispatcher.InvokeAsync(()=>blocked=false).ConfigureAwait(false);throw; }
            if(ReferenceEquals(payload,context.Snapshot.Payload))
            { await dispatcher.InvokeAsync(()=>blocked=false).ConfigureAwait(false);return SaveCommitResult<LocalProfileState>.Success(context.Snapshot); }
            var result=await CommitUnderLock(payload,false,ct).ConfigureAwait(false);
            await dispatcher.InvokeAsync(()=>
            {
                if(result.Succeeded&&context.Presented&&ReferenceEquals(session,context.Session)&&!blocked)
                {completionCommitted=true;Status=PersistedSessionStatus.ProgressCommitted;}
                else if(result.Status==SaveCommitStatus.Failed||result.Status==SaveCommitStatus.Cancelled){blocked=false;if(context.Presented){completionCommitted=true;Status=PersistedSessionStatus.ProgressCommitted;}}
            }).ConfigureAwait(false);
            return result;
        }
        /// <summary>Commits catalog defaults on the same writer as draft/completion saves.</summary>
        public Task<SaveCommitResult<LocalProfileState>> GrantCosmeticDefaultsAsync(LocalCosmeticsPolicy cosmetics,string hash,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>cosmetics.GrantDefaults(s,hash,utc).State,ct);
        /// <summary>Atomic catalog-priced debit and ownership.</summary>
        public Task<SaveCommitResult<LocalProfileState>> PurchaseCosmeticAsync(LocalCosmeticsPolicy cosmetics,string itemId,string hash,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>cosmetics.Purchase(s,itemId,hash,g,utc).State,ct);
        /// <summary>Persists selection without a separate inventory writer.</summary>
        public Task<SaveCommitResult<LocalProfileState>> SelectCosmeticsAsync(LocalCosmeticsPolicy cosmetics,IReadOnlyList<string> itemIds,string hash,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>cosmetics.Select(s,itemIds,hash).State,ct);
        /// <summary>Resumes an acknowledged milestone reservation or writes reserve then grant under one writer lock.</summary>
        public async Task<SaveCommitResult<LocalProfileState>> ClaimMilestoneCosmeticAsync(LocalCosmeticsPolicy cosmetics,string itemId,string hash,CancellationToken ct)
        {
            await writer.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                var initial=await Capture(()=>durable).ConfigureAwait(false);
                if(initial==null)return SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed,"METAGAME_SESSION_NOT_READY");
                if(initial.Payload.Inventory.Ownership.Any(o=>o.ItemId==itemId))return SaveCommitResult<LocalProfileState>.Success(initial);
                var reservation=initial.Payload.CosmeticClaimReservations.SingleOrDefault(r=>r.ItemId==itemId);
                if(reservation==null)
                {
                    var operation=Guid.NewGuid().ToString("D");
                    var reserved=await ProfileMutationUnderLock((s,g,utc)=>cosmetics.ReserveMilestone(s,itemId,operation,hash,g).State,ct).ConfigureAwait(false);
                    if(!reserved.Succeeded||reserved.Snapshot==null)return reserved;
                    reservation=reserved.Snapshot.Payload.CosmeticClaimReservations.Single(r=>r.ItemId==itemId);
                }
                var bound=reservation;
                return await ProfileMutationUnderLock((s,g,utc)=>cosmetics.CommitMilestone(s,itemId,bound.OperationId,bound.ClaimGeneration,hash,utc).State,ct).ConfigureAwait(false);
            }
            finally { writer.Release(); }
        }
        /// <summary>Captures the fixed local-date roll, then commits its free grant; crash retry reuses the roll.</summary>
        public async Task<SaveCommitResult<LocalProfileState>> ClaimFreeDailyAsync(LocalMetagamePolicy metagame,ILocalDailyRandom random,string date,CancellationToken ct)
        {
            await writer.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                var reserved=await ProfileMutationUnderLock((s,g,utc)=>metagame.ReserveDaily(s,date,false,Guid.NewGuid().ToString("D"),g,random),ct).ConfigureAwait(false);
                if(!reserved.Succeeded)return reserved;
                return await ProfileMutationUnderLock((s,g,utc)=>metagame.CommitFreeDaily(s,date,g,utc),ct).ConfigureAwait(false);
            }
            finally { writer.Release(); }
        }
        /// <summary>Returns only an acknowledged video reservation; SDK show belongs after this task.</summary>
        public Task<SaveCommitResult<LocalProfileState>> ReserveDailyRewardAsync(LocalMetagamePolicy metagame,ILocalDailyRandom random,string date,string operation,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>metagame.ReserveDaily(s,date,true,operation,g,random),ct);
        /// <summary>Commits exact callback evidence and its fixed daily grant atomically.</summary>
        public Task<SaveCommitResult<LocalProfileState>> ConfirmDailyRewardAsync(LocalMetagamePolicy metagame,AdRewardEvidence evidence,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>metagame.ConfirmDailyVideo(s,evidence,g,utc),ct);
        /// <summary>Retains uncertainty or closes a definitive no-reward show without a new roll.</summary>
        public Task<SaveCommitResult<LocalProfileState>> ResolveDailyRewardAsync(LocalMetagamePolicy metagame,AdRewardReservation reservation,bool definitelyNoReward,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>metagame.ResolveDailyVideo(s,reservation,definitelyNoReward),ct);
        /// <summary>Persists the only extra hint credit reservation before SDK show.</summary>
        public Task<SaveCommitResult<LocalProfileState>> ReserveHintRewardAsync(LocalMetagamePolicy metagame,PublicPuzzleIdentity identity,string operation,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>metagame.ReserveHintVideo(s,identity,operation,g),ct);
        /// <summary>Confirms the extra hint credit without consuming it.</summary>
        public Task<SaveCommitResult<LocalProfileState>> ConfirmHintRewardAsync(LocalMetagamePolicy metagame,PublicPuzzleIdentity identity,AdRewardEvidence evidence,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>metagame.ConfirmHintVideo(s,identity,evidence),ct);
        /// <summary>Closes or reconciles only the exact rewarded hint reservation.</summary>
        public Task<SaveCommitResult<LocalProfileState>> ResolveHintRewardAsync(LocalMetagamePolicy metagame,PublicPuzzleIdentity identity,AdRewardReservation reservation,bool definitelyNoReward,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>metagame.ResolveHintVideo(s,identity,reservation,definitelyNoReward),ct);
        /// <summary>Commits the existing closed preference model before UI/audio applies saved choices.</summary>
        public Task<SaveCommitResult<LocalProfileState>> SaveSettingsAsync(LocalSettings settings,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>LocalSettingsPolicy.Apply(s,settings),ct);

        /// <summary>Serializes a trusted provider transaction on this same profile writer, then reloads its actual complete committed profile without retiring the live attempt.</summary>
        public async Task<SaveCommitResult<LocalProfileState>> RunExternalProfileActionAsync(Func<CancellationToken,Task<SaveCommitResult<LocalProfileState>>> action,CancellationToken ct)
        {
            if(action==null)throw new ArgumentNullException(nameof(action));
            await writer.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                var before=await Capture(()=>
                {
                    LocalModel.Require(profile.Use!=CatalogUse.IsolatedReview&&!blocked&&!needsPromotion&&durable!=null&&(session==null||session.Phase!=STP.Puzzle.Domain.SessionPhase.SolvedPendingCommit||completionCommitted),"METAGAME_SESSION_NOT_READY");
                    var presented=CanPresentResults;blocked=true;Status=PersistedSessionStatus.Writing;return(Snapshot:durable!,Session:session,Presented:presented);
                }).ConfigureAwait(false);
                SaveCommitResult<LocalProfileState> result;
                try {result=await action(ct).ConfigureAwait(false);}
                catch {result=SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.RecoveryRequired,"MOBILE_ACTION_OUTCOME_UNKNOWN");}
                SaveLoadResult<LocalProfileState> loaded;
                try {loaded=await repository.LoadAsync(ct).ConfigureAwait(false);}
                catch {await dispatcher.InvokeAsync(()=>{blocked=true;requiresExplicitRecovery=true;Status=PersistedSessionStatus.RecoveryRequired;}).ConfigureAwait(false);return SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.RecoveryRequired,"MOBILE_PROFILE_RELOAD_REQUIRED");}
                bool valid=await ValidateLoaded(loaded,ct).ConfigureAwait(false);
                if(!valid||loaded.Snapshot==null||loaded.Snapshot.Generation<before.Snapshot.Generation||
                    result.Succeeded&&(result.Snapshot==null||result.Snapshot.Generation!=loaded.Snapshot.Generation||!Bound(result.Snapshot)))
                { await dispatcher.InvokeAsync(()=>{blocked=true;requiresExplicitRecovery=true;Status=PersistedSessionStatus.RecoveryRequired;}).ConfigureAwait(false);return SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.RecoveryRequired,"MOBILE_COMMIT_BINDING_UNCONFIRMED"); }
                var confirmed=loaded.Snapshot;
                await dispatcher.InvokeAsync(()=>
                {
                    if(!ReferenceEquals(session,before.Session)){Status=PersistedSessionStatus.RecoveryRequired;return;}
                    durable=confirmed;blocked=!result.Succeeded&&result.Status!=SaveCommitStatus.Failed&&result.Status!=SaveCommitStatus.Cancelled;
                    if(blocked){requiresExplicitRecovery=true;recoveryObservation=loaded.Observation;}
                    completionCommitted=before.Presented&&!blocked;Status=blocked?PersistedSessionStatus.RecoveryRequired:before.Presented?PersistedSessionStatus.ProgressCommitted:PersistedSessionStatus.Ready;
                }).ConfigureAwait(false);
                return result.Succeeded?SaveCommitResult<LocalProfileState>.Success(confirmed):result;
            }
            finally {writer.Release();}
        }
    }
}



