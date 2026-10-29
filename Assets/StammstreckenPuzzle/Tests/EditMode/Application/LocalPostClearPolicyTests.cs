using System;
using System.Linq;
using NUnit.Framework;
using STP.Application;
using STP.Puzzle.Domain;

namespace STP.Tests.Application.EditMode
{
    public sealed class LocalPostClearPolicyTests
    {
        private const string Utc="2026-10-21T00:00:00Z";
        private sealed class Fixture
        {
            internal readonly LocalSaveContractFixtures.Setup Data=new LocalSaveContractFixtures.Setup();
            internal LocalPostClearPolicy Policy=>new LocalPostClearPolicy(Data.Puzzles,Data.CompletionId,Data.Ledger);
            internal PublicPuzzleIdentity Identity=>Data.Puzzles.Puzzles[0].Identity;
            internal LocalProfileState Cleared=>Data.Policy.Complete(Data.Empty,(PuzzleSessionState)Data.Solved(0),2,Utc).State;
            internal SaveCommitResult<LocalProfileState> Ack(LocalProfileState state,long generation)=>SaveCommitResult<LocalProfileState>.Success(new SaveSnapshot<LocalProfileState>(Data.Profile,Data.Aggregate,generation,state));
        }
        [Test]
        public void FirstClearIsRequiredAndOnlyExactAcknowledgedReservationCanGrantOneTenPointTerminal()
        {
            var f=new Fixture();Assert.Throws<ArgumentException>(()=>f.Policy.Reserve(f.Data.Empty,f.Identity,3));var initial=f.Cleared;long before=f.Data.Ledger.Validate(initial);
            var reserved=f.Policy.Reserve(initial,f.Identity,3);Assert.That(f.Data.Ledger.Validate(reserved),Is.EqualTo(before));var reservation=LocalPostClearPolicy.Reservation(f.Ack(reserved,3),f.Identity);
            var wrong=new AdRewardEvidence(new AdRewardReservation(reservation.ClaimId,reservation.OperationId,reservation.SaveGeneration+1),reservation.OperationId);
            Assert.That(f.Policy.Confirm(reserved,wrong,4,Utc),Is.SameAs(reserved));Assert.That(f.Policy.Confirm(reserved,new AdRewardEvidence(reservation,"foreign"),4,Utc),Is.SameAs(reserved));
            var evidence=new AdRewardEvidence(reservation,reservation.OperationId);var committed=f.Policy.Confirm(reserved,evidence,4,Utc);
            Assert.That(f.Data.Ledger.Validate(committed),Is.EqualTo(before+10));var claim=committed.RewardClaims.OfType<PostClearRewardClaim>().Single();Assert.That(claim.ClaimStatus,Is.EqualTo("COMMITTED"));Assert.That(claim.Terminal!.Amount,Is.EqualTo(10));
            Assert.That(committed.PendingOperations.OfType<LocalRewardOperation>().Single().State,Is.EqualTo("COMMITTED"));Assert.That(f.Policy.Confirm(committed,evidence,5,Utc),Is.SameAs(committed));Assert.That(f.Policy.Reserve(committed,f.Identity,5),Is.SameAs(committed));
            Assert.That(f.Policy.CanReserve(committed,f.Identity),Is.False);Assert.Throws<ArgumentException>(()=>LocalPostClearPolicy.Reservation(f.Ack(committed,4),f.Identity));
        }
        [Test]
        public void FailedShowRetryUsesNewBindingAndLateReleasedOrTerminalCallbacksAreNoOps()
        {
            var f=new Fixture();var first=f.Policy.Reserve(f.Cleared,f.Identity,3);var old=LocalPostClearPolicy.Reservation(f.Ack(first,3),f.Identity);
            var reconciliation=f.Policy.Resolve(first,old,false);Assert.That(reconciliation.RewardClaims.OfType<PostClearRewardClaim>().Single().ClaimStatus,Is.EqualTo("RECONCILIATION_REQUIRED"));Assert.That(f.Policy.Reserve(reconciliation,f.Identity,4),Is.SameAs(reconciliation));
            var closed=f.Policy.Resolve(reconciliation,old,true);var second=f.Policy.Reserve(closed,f.Identity,5);var current=LocalPostClearPolicy.Reservation(f.Ack(second,5),f.Identity);
            Assert.That(current.OperationId,Is.Not.EqualTo(old.OperationId));Assert.That(f.Policy.Confirm(second,new AdRewardEvidence(old,old.OperationId),6,Utc),Is.SameAs(second));
            var granted=f.Policy.Confirm(second,new AdRewardEvidence(current,current.OperationId),6,Utc);Assert.That(f.Policy.Resolve(granted,current,true),Is.SameAs(granted));Assert.That(granted.EconomyJournal.Count(e=>e.ReasonCode=="POST_CLEAR_PATIENCE"),Is.EqualTo(1));
        }
        [Test]
        public void RetainedForeignCompletionClaimCannotBeReboundOrResolvedByCurrentCatalog()
        {
            var f=new Fixture();var reserved=f.Policy.Reserve(f.Cleared,f.Identity,3);var reservation=LocalPostClearPolicy.Reservation(f.Ack(reserved,3),f.Identity);var claim=reserved.RewardClaims.OfType<PostClearRewardClaim>().Single();
            var foreign=new CatalogIdentity("retained-other-completion",2,new string('a',64),CatalogUse.ContractTest,CatalogApprovalStatus.FixtureOnly);
            var changed=PostClearRewardClaim.Create(claim.PuzzleIdentity,claim.ClaimId,claim.LocalOperationId,claim.ProviderOperationId,claim.ClaimStatus,foreign,null);
            var state=reserved.WithProgress(reserved.Levels,reserved.LedgerCheckpoint,reserved.EconomyJournal,reserved.SeasonProgressCache,reserved.Drafts,reserved.RewardClaims.Where(c=>!ReferenceEquals(c,claim)).Concat(new[]{changed}).ToArray());
            Assert.Throws<ArgumentException>(()=>f.Policy.Reserve(state,f.Identity,4));Assert.Throws<ArgumentException>(()=>LocalPostClearPolicy.Reservation(f.Ack(state,3),f.Identity));
            Assert.That(f.Policy.Confirm(state,new AdRewardEvidence(reservation,reservation.OperationId),4,Utc),Is.SameAs(state));Assert.That(f.Policy.Resolve(state,reservation,true),Is.SameAs(state));
        }
        [Test]
        public void UnacknowledgedOrFutureReservationCannotCrossProviderBoundary()
        {
            var f=new Fixture();var reserved=f.Policy.Reserve(f.Cleared,f.Identity,9);
            Assert.Throws<ArgumentException>(()=>LocalPostClearPolicy.Reservation(SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed,"TEST_WRITE_FAILED"),f.Identity));
            Assert.Throws<ArgumentException>(()=>LocalPostClearPolicy.Reservation(f.Ack(reserved,8),f.Identity));
        }
    }
}
