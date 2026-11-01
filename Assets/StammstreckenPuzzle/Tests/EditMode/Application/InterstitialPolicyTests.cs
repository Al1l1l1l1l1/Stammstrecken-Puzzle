using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;

namespace STP.Tests.Application.EditMode
{
    // Controlled ephemeral policy inputs, never provider, persisted-clear, Sandbox or presentation evidence.
    public sealed class InterstitialPolicyTests
    {
        private readonly SaveProfileIdentity profile = new SaveProfileIdentity("interstitial-contract", "test", CatalogUse.ContractTest);
        private readonly CatalogIdentity catalog = new CatalogIdentity("interstitial-contract", 1, new string('a', 64), CatalogUse.ContractTest, CatalogApprovalStatus.FixtureOnly);
        private int sequence;
        private InterstitialPolicy Policy() => new InterstitialPolicy(profile, catalog);
        private InterstitialResultReceipt Input(int clears, InterstitialCompletionMode mode = InterstitialCompletionMode.CampaignFirstClear, Guid? attempt = null, int? generation = null, string? identity = null)
        {
            int n = ++sequence;
            return (InterstitialResultReceipt)Activator.CreateInstance(typeof(InterstitialResultReceipt), BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { profile, catalog, (long)(generation ?? n), new PublicPuzzleIdentity(identity ?? "case-" + n, PublicPuzzleIdentity.SemanticHashProfile, new string('b', 64)), attempt ?? Guid.NewGuid(), mode, clears }, null)!;
        }
        private static bool Take(InterstitialPolicy policy, InterstitialResultReceipt input, out InterstitialPermit? permit, bool consent = true, bool owned = false)
            => policy.TryTake(input, true, true, consent, owned, out permit, out _);

        [Test]
        public void CampaignPacingPreservesTwoInitialClearsAndSlowsAfterEightWithoutCatchup()
        {
            var policy = Policy(); var slots = new List<int>();
            for (int clear = 1; clear <= 17; clear++)
                if (Take(policy, Input(clear), out var permit)) { slots.Add(clear); policy.Release(permit!); }
            Assert.That(slots, Is.EqualTo(new[] { 2, 4, 6, 8, 11, 14, 17 }));
            Assert.That(policy.SessionShowBudgetConsumed, Is.Zero, "No-fill opportunities do not claim a native show");
        }
        [Test]
        public void NoFillAndUnavailableConsentConsumeOnlyTheirOpportunityAndNeverQueueIt()
        {
            var policy = Policy(); var second = Input(2);
            Assert.That(Take(policy, second, out _, consent: false), Is.False);
            Assert.That(Take(policy, second, out _), Is.False);
            Assert.That(Take(policy, Input(3), out _), Is.False);
            Assert.That(Take(policy, Input(4), out var fourth), Is.True); policy.Release(fourth!);
            Assert.That(Take(policy, Input(5), out _), Is.False);
            Assert.That(Take(policy, Input(6), out _), Is.True);
        }
        [Test]
        public void TheSameAttemptCannotBeReofferedAfterNewSavesOrDuplicateCallbacks()
        {
            var policy = Policy(); var attempt = Guid.NewGuid();
            Assert.That(Take(policy, Input(2, attempt: attempt), out var permit), Is.True); policy.Release(permit!);
            Assert.That(Take(policy, Input(2, attempt: attempt), out _), Is.False);
        }
        [Test]
        public void ThinkingRideAndResultAssemblyNeverConsumeTheLaterExplicitContinue()
        {
            var policy = Policy(); var result = Input(2);
            Assert.That(policy.TryTake(result, false, true, true, false, out _, out _), Is.False);
            Assert.That(policy.TryTake(result, true, false, true, false, out _, out _), Is.False);
            Assert.That(Take(policy, result, out _), Is.True);
        }
        [TestCase(InterstitialCompletionMode.CampaignFirstClear)][TestCase(InterstitialCompletionMode.Revision)][TestCase(InterstitialCompletionMode.Endless)]
        public void RemoveAdsPreventsEveryMode(InterstitialCompletionMode mode)
        {
            var policy = Policy();
            for (int n = 1; n <= 15; n++) Assert.That(Take(policy, Input(mode == InterstitialCompletionMode.CampaignFirstClear ? n : 2, mode), out _, owned: true), Is.False);
            Assert.That(policy.SessionShowBudgetConsumed, Is.Zero);
        }
        [Test]
        public void RevisionAndNewEndlessUseIndependentFiveAndThreeCompletionCadences()
        {
            var policy = Policy(); var revisions = new List<int>(); var endless = new List<int>();
            for (int n = 1; n <= 10; n++)
            {
                if (Take(policy, Input(2, InterstitialCompletionMode.Revision), out var revision)) { revisions.Add(n); policy.Release(revision!); }
                if (Take(policy, Input(2, InterstitialCompletionMode.Endless), out var fresh)) { endless.Add(n); policy.Release(fresh!); }
            }
            Assert.That(revisions, Is.EqualTo(new[] { 5, 10 })); Assert.That(endless, Is.EqualTo(new[] { 3, 6, 9 }));
        }
        [Test]
        public void ThreeNativeShowStartsCapTheWholeSessionAcrossModesAndTimeouts()
        {
            var policy = Policy();
            foreach (int clear in new[] { 2, 4, 6 })
            {
                Assert.That(Take(policy, Input(clear), out var permit), Is.True);
                Assert.That(policy.BeginShow(permit!, true, true, false), Is.True);
                Assert.That(policy.BeginShow(permit!, true, true, false), Is.False); policy.Release(permit!);
            }
            Assert.That(Take(policy, Input(8), out _), Is.False);
            for (int n = 0; n < 6; n++) Assert.That(Take(policy, Input(8, InterstitialCompletionMode.Endless), out _), Is.False);
            Assert.That(policy.SessionShowBudgetConsumed, Is.EqualTo(3));
        }
        [Test]
        public void ChangedBoundaryConsentOrOwnershipAfterLoadingPreventsShow()
        {
            var policy = Policy(); Assert.That(Take(policy, Input(2), out var permit), Is.True);
            Assert.That(policy.BeginShow(permit!, false, true, false), Is.False);
            Assert.That(policy.BeginShow(permit!, true, false, false), Is.False);
            Assert.That(policy.BeginShow(permit!, true, true, true), Is.False);
            Assert.That(policy.SessionShowBudgetConsumed, Is.Zero); policy.Release(permit!);
        }
        [Test]
        public void ReceiptFactoryRejectsAnUnacknowledgedSession()
        {
            var data = new LocalSaveContractFixtures.Setup();
            var use = new PersistedPuzzleSession(new LocalSaveContractFixtures.Repository(), data.Puzzles, data.Policy, data.Profile, data.Catalogs,
                new LocalSaveContractFixtures.Clock(), new LocalSaveContractFixtures.Dispatcher());
            Assert.Throws<ArgumentException>(() => InterstitialResultReceipt.FromCommittedResult(use));
        }
        [Test]
        public void RetainedOrphanClearCannotAdvanceTheCurrentCampaignAdvertisementCadence()
        {
            Task.Run(async () =>
            {
                var data = new LocalSaveContractFixtures.Setup(); var clock = new LocalSaveContractFixtures.Clock();
                var oldClear = data.Complete(data.Empty, data.Solved(0), 2).State;
                // Controlled FixtureOnly catalog projection: the earlier genuine clear is now retained outside current membership.
                var currentInput = data.Puzzles.Puzzles[1];
                var puzzles = new LocalSaveContractFixtures.PublicCatalog(data.PublicId, new[] { currentInput });
                var campaign = new LocalSaveContractFixtures.CampaignCatalog(data.CampaignId, new[] { new LockedCampaignEntry(data.CampaignId, currentInput.Identity, "S1", "S1-01", "S1-01-01", 1, Array.Empty<string>()) });
                var policy = new LocalProgressPolicy(puzzles, campaign, data.Completion, LocalSaveContractFixtures.Json, data.Ledger);
                var retained = oldClear.WithProgress(oldClear.Levels, oldClear.LedgerCheckpoint, oldClear.EconomyJournal, policy.Cache(oldClear.Levels), oldClear.Drafts, oldClear.RewardClaims);
                policy.Validate(retained);
                var repository = new LocalSaveContractFixtures.Repository { Loaded = SaveLoadResult<LocalProfileState>.Success(new SaveSnapshot<LocalProfileState>(data.Profile, data.Aggregate, 2, retained), data.Observe(2)) };
                var use = new PersistedPuzzleSession(repository, puzzles, policy, data.Profile, data.Catalogs, clock, new LocalSaveContractFixtures.Dispatcher());
                await use.LoadAsync(CancellationToken.None).ConfigureAwait(false); use.Start(currentInput.Identity, Guid.NewGuid());
                LocalSaveContractFixtures.SolveCurrent(use, clock);
                Assert.That((await use.CommitCompletionAsync(CancellationToken.None).ConfigureAwait(false)).Succeeded, Is.True);
                Assert.That(use.Snapshot!.Payload.Levels.Count(item => item.FirstClear != null), Is.EqualTo(2));
                var receipt = InterstitialResultReceipt.FromCommittedResult(use);
                Assert.That(receipt.CampaignFirstClearCount, Is.EqualTo(1));
                var policyForAds = new InterstitialPolicy(data.Profile, data.Aggregate);
                Assert.That(Take(policyForAds, receipt, out _), Is.False);
            }).GetAwaiter().GetResult();
        }
        [Test]
        public void ReceiptFactoryBindsTheActualAcknowledgedFirstClearAndRevisionAttempts()
        {
            // Real domain commands and session commit acknowledgement; repository remains an explicit test double.
            Task.Run(async () =>
            {
                var data = new LocalSaveContractFixtures.Setup(); var clock = new LocalSaveContractFixtures.Clock();
                var repository = new LocalSaveContractFixtures.Repository
                {
                    Loaded = SaveLoadResult<LocalProfileState>.Success(new SaveSnapshot<LocalProfileState>(data.Profile, data.Aggregate, 1, data.Empty), data.Observe())
                };
                var use = new PersistedPuzzleSession(repository, data.Puzzles, data.Policy, data.Profile, data.Catalogs, clock, new LocalSaveContractFixtures.Dispatcher());
                await use.LoadAsync(CancellationToken.None).ConfigureAwait(false);
                var identity = data.Puzzles.Puzzles[0].Identity; var firstAttempt = Guid.NewGuid(); use.Start(identity, firstAttempt);
                LocalSaveContractFixtures.SolveCurrent(use, clock);
                Assert.That(use.CanPresentResults, Is.False);
                Assert.Throws<ArgumentException>(() => InterstitialResultReceipt.FromCommittedResult(use));
                var committed = await use.CommitCompletionAsync(CancellationToken.None).ConfigureAwait(false);
                Assert.That(committed.Succeeded, Is.True); Assert.That(use.CanPresentResults, Is.True);
                var first = InterstitialResultReceipt.FromCommittedResult(use);
                Assert.That(first.Profile, Is.EqualTo(data.Profile)); Assert.That(first.Catalog, Is.EqualTo(data.Aggregate));
                Assert.That(first.Identity, Is.EqualTo(identity)); Assert.That(first.AttemptId, Is.EqualTo(firstAttempt));
                Assert.That(first.Generation, Is.EqualTo(committed.Snapshot!.Generation));
                Assert.That(first.Mode, Is.EqualTo(InterstitialCompletionMode.CampaignFirstClear)); Assert.That(first.CampaignFirstClearCount, Is.EqualTo(1));
                // Revision is eligible only after the actual complete season; an early replay is practice.
                foreach (var remaining in data.Puzzles.Puzzles.Skip(1))
                {
                    use.Start(remaining.Identity, Guid.NewGuid()); LocalSaveContractFixtures.SolveCurrent(use, clock);
                    Assert.That((await use.CommitCompletionAsync(CancellationToken.None).ConfigureAwait(false)).Succeeded, Is.True);
                }
                var revisionAttempt = Guid.NewGuid(); use.Start(identity, revisionAttempt);
                Assert.Throws<ArgumentException>(() => InterstitialResultReceipt.FromCommittedResult(use));
                LocalSaveContractFixtures.SolveCurrent(use, clock);
                var revisionCommit = await use.CommitCompletionAsync(CancellationToken.None).ConfigureAwait(false);
                Assert.That(revisionCommit.Succeeded, Is.True);
                var revision = InterstitialResultReceipt.FromCommittedResult(use);
                Assert.That(revision.AttemptId, Is.EqualTo(revisionAttempt)); Assert.That(revision.Mode, Is.EqualTo(InterstitialCompletionMode.Revision));
                Assert.That(revision.CampaignFirstClearCount, Is.EqualTo(data.Puzzles.Puzzles.Count)); Assert.That(revision.Generation, Is.EqualTo(revisionCommit.Snapshot!.Generation));
            }).GetAwaiter().GetResult();
        }
    }
}
