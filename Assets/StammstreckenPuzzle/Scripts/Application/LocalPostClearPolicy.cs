using System;
using System.Globalization;
using System.Linq;

namespace STP.Application
{
    /// <summary>Executable existing campaign POST_CLEAR_PATIENCE contract, using one acknowledged profile and one ledger.</summary>
    public sealed class LocalPostClearPolicy
    {
        public const string PolicyVersion="post-clear-v1";
        private readonly IPublicPuzzleCatalog puzzles;private readonly CatalogIdentity completion;private readonly ILocalLedger ledger;
        public LocalPostClearPolicy(IPublicPuzzleCatalog puzzles,CatalogIdentity completion,ILocalLedger ledger)
        {this.puzzles=puzzles??throw new ArgumentNullException(nameof(puzzles));this.completion=completion??throw new ArgumentNullException(nameof(completion));this.ledger=ledger??throw new ArgumentNullException(nameof(ledger));LocalModel.Require(puzzles.Identity.Use==completion.Use&&puzzles.Identity.Use!=CatalogUse.IsolatedReview,"POST_CLEAR_CATALOG_USE");}
        private void Bound(LocalProfileState state,PublicPuzzleIdentity identity)
        {LocalModel.Require(state.Profile.CatalogSet.PublicCatalog.Equals(puzzles.Identity)&&state.Profile.CatalogSet.CompletionCatalog.Equals(completion)&&puzzles.TryGet(identity,out _)&&state.Levels.Any(l=>l.Identity.Equals(identity)&&l.FirstClear!=null),"POST_CLEAR_FIRST_CLEAR_REQUIRED");ledger.Validate(state);}
        public bool CanReserve(LocalProfileState state,PublicPuzzleIdentity identity)
        {Bound(state,identity);var claim=state.RewardClaims.OfType<PostClearRewardClaim>().SingleOrDefault(c=>c.PuzzleIdentity.Equals(identity));return claim==null||claim.CompletionCatalog.Equals(completion)&&(claim.ClaimStatus=="CLOSED_NO_REWARD"||claim.ClaimStatus=="FAILED");}
        private static long ReservationGeneration(string operation)
        {
            var fields=operation.Split(':');LocalModel.Require(fields.Length==3&&fields[0]=="postclear-v1"&&long.TryParse(fields[1],NumberStyles.None,CultureInfo.InvariantCulture,out _)&&Guid.TryParseExact(fields[2],"D",out _),"POST_CLEAR_OPERATION_INVALID");
            return LocalModel.Positive(long.Parse(fields[1],CultureInfo.InvariantCulture));
        }
        private static LocalProfileState Replace(LocalProfileState state,PostClearRewardClaim claim,LocalRewardOperation operation)
        {
            var previous=state.RewardClaims.OfType<PostClearRewardClaim>().SingleOrDefault(c=>c.ClaimId==claim.ClaimId);
            var claims=state.RewardClaims.Where(c=>!ReferenceEquals(c,previous)).Concat(new[]{claim}).ToArray();
            var operations=state.PendingOperations.Where(o=>!(o is LocalRewardOperation r&&r.ClaimId==claim.ClaimId)).Concat(new[]{operation}).OrderBy(o=>o.OperationId,StringComparer.Ordinal).ToArray();
            return LocalProfileState.Create(state.Profile,state.Levels,state.Endless,state.LedgerCheckpoint,state.EconomyJournal,state.Inventory,state.CosmeticClaimReservations,state.SeasonProgressCache,state.Drafts,claims,state.DailyStatus,state.Entitlements,operations,state.Settings,state.DataLifecycle,state.HintEntitlement).WithMetagame(state.Metagame);
        }
        /// <summary>Durably binds claim/local provider key/generation before any SDK start.</summary>
        public LocalProfileState Reserve(LocalProfileState state,PublicPuzzleIdentity identity,long generation)
        {
            Bound(state,identity);var previous=state.RewardClaims.OfType<PostClearRewardClaim>().SingleOrDefault(c=>c.PuzzleIdentity.Equals(identity));
            LocalModel.Require(previous==null||previous.CompletionCatalog.Equals(completion),"POST_CLEAR_CATALOG_MISMATCH");
            if(previous!=null&&previous.ClaimStatus!="CLOSED_NO_REWARD"&&previous.ClaimStatus!="FAILED")return state;
            string claimId="reward-claim:POST_CLEAR_PATIENCE:"+identity.PuzzleId;string operation="postclear-v1:"+LocalModel.Positive(generation).ToString(CultureInfo.InvariantCulture)+":"+Guid.NewGuid().ToString("D");
            var claim=PostClearRewardClaim.Create(identity,claimId,operation,operation,"RESERVED",completion,null);
            return Replace(state,claim,LocalRewardOperation.Create(operation,claimId,identity,operation,"RESERVED"));
        }
        /// <summary>Only acknowledged reservation snapshots may cross the provider boundary.</summary>
        public static AdRewardReservation Reservation(SaveCommitResult<LocalProfileState> committed,PublicPuzzleIdentity identity)
        {
            LocalModel.Require(committed.Succeeded&&committed.Snapshot!=null,"POST_CLEAR_NOT_COMMITTED");var claim=committed.Snapshot!.Payload.RewardClaims.OfType<PostClearRewardClaim>().SingleOrDefault(c=>c.PuzzleIdentity.Equals(identity));
            LocalModel.Require(claim!=null&&claim.CompletionCatalog.Equals(committed.Snapshot.Payload.Profile.CatalogSet.CompletionCatalog)&&claim.ClaimStatus=="RESERVED"&&claim.LocalOperationId==claim.ProviderOperationId,"POST_CLEAR_RESERVATION_REQUIRED");long generation=ReservationGeneration(claim!.LocalOperationId);LocalModel.Require(generation<=committed.Snapshot.Generation,"POST_CLEAR_GENERATION_INVALID");return new AdRewardReservation(claim.ClaimId,claim.LocalOperationId,generation);
        }
        private static bool Matches(PostClearRewardClaim claim,AdRewardReservation reservation)=>claim.ClaimId==reservation.ClaimId&&claim.LocalOperationId==reservation.OperationId&&claim.ProviderOperationId==reservation.OperationId&&ReservationGeneration(claim.LocalOperationId)==reservation.SaveGeneration;
        public LocalProfileState Confirm(LocalProfileState state,AdRewardEvidence evidence,long generation,string utc)
        {
            var claim=state.RewardClaims.OfType<PostClearRewardClaim>().SingleOrDefault(c=>c.ClaimId==evidence.Reservation.ClaimId);if(claim==null||!claim.CompletionCatalog.Equals(completion)||claim.ClaimStatus=="COMMITTED"||claim.ClaimStatus=="CLOSED_NO_REWARD"||evidence.ProviderOperationId!=evidence.Reservation.OperationId||!Matches(claim,evidence.Reservation))return state;
            Bound(state,claim.PuzzleIdentity);state=ledger.Compact(state,generation);
            var terminal=EconomyTerminal.Create(claim.ClaimId,"POST_CLEAR_PATIENCE",claim.PuzzleIdentity.PuzzleId,completion,claim.ClaimId,10,generation,utc);
            var updated=PostClearRewardClaim.Create(claim.PuzzleIdentity,claim.ClaimId,claim.LocalOperationId,claim.ProviderOperationId,"COMMITTED",completion,terminal);
            var next=Replace(state,updated,LocalRewardOperation.Create(claim.LocalOperationId,claim.ClaimId,claim.PuzzleIdentity,claim.ProviderOperationId,"COMMITTED"));
            next=next.WithProgress(next.Levels,next.LedgerCheckpoint,next.EconomyJournal.Concat(new[]{ledger.Next(next.LedgerCheckpoint,next.EconomyJournal,terminal)}).ToArray(),next.SeasonProgressCache,next.Drafts,next.RewardClaims);return ledger.Compact(next,generation);
        }
        public LocalProfileState Resolve(LocalProfileState state,AdRewardReservation reservation,bool definitelyNoReward)
        {
            var claim=state.RewardClaims.OfType<PostClearRewardClaim>().SingleOrDefault(c=>c.ClaimId==reservation.ClaimId);if(claim==null||!claim.CompletionCatalog.Equals(completion)||claim.ClaimStatus=="COMMITTED"||claim.ClaimStatus=="CLOSED_NO_REWARD"||!Matches(claim,reservation))return state;
            Bound(state,claim.PuzzleIdentity);string status=definitelyNoReward?"CLOSED_NO_REWARD":"RECONCILIATION_REQUIRED";
            return Replace(state,PostClearRewardClaim.Create(claim.PuzzleIdentity,claim.ClaimId,claim.LocalOperationId,claim.ProviderOperationId,status,completion,null),LocalRewardOperation.Create(claim.LocalOperationId,claim.ClaimId,claim.PuzzleIdentity,claim.ProviderOperationId,status));
        }
    }
}
