using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;
using STP.Application.Ports;

namespace STP.Tests.Application.EditMode
{
    // These verify real Application transitions against a controlled CAS repository, never a store/legal PASS.
    public sealed class PersistedMobileTransactionsTests
    {
        private sealed class Fixture
        {
            internal readonly LocalSaveContractFixtures.Setup Data = new LocalSaveContractFixtures.Setup();
            internal readonly Repository Repo;
            internal readonly PersistedMobileTransactions Transactions;
            internal Fixture()
            {
                Repo = new Repository(Data);
                Transactions = new PersistedMobileTransactions(Repo, Data.Profile, Data.Catalogs, new LocalSaveContractFixtures.Dispatcher(), new LocalSaveContractFixtures.Clock(),
                    new MobilePurchaseBinding("GooglePlay", "example.stp.qa", "remove_ads_qa", false));
            }
        }
        private sealed class Repository : ILocalProfileRepository
        {
            private readonly LocalSaveContractFixtures.Setup data;
            internal SaveSnapshot<LocalProfileState> Current;
            internal int CommitAttempts, FailCommit, Conflicts;
            internal Repository(LocalSaveContractFixtures.Setup data) { this.data = data; Current = new SaveSnapshot<LocalProfileState>(data.Profile, data.Aggregate, 1, data.Empty); }
            public Task<SaveLoadResult<LocalProfileState>> LoadAsync(CancellationToken cancellation) => Task.FromResult(SaveLoadResult<LocalProfileState>.Success(Current, data.Observe(Current.Generation)));
            public Task<SaveCommitResult<LocalProfileState>> CommitAsync(SaveSnapshot<LocalProfileState> offered, long expected, CancellationToken cancellation)
            {
                CommitAttempts++;
                if (FailCommit == CommitAttempts) return Task.FromResult(SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.RecoveryRequired, "TEST_UNCERTAIN_COMMIT"));
                if (Conflicts > 0) { Conflicts--; return Task.FromResult(SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Conflict, "TEST_CONFLICT")); }
                if (Current.Generation != expected || offered.Generation != expected + 1) return Task.FromResult(SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Conflict, "TEST_CONFLICT"));
                Current = offered;
                return Task.FromResult(SaveCommitResult<LocalProfileState>.Success(Current));
            }
            public Task<SaveLoadResult<LocalProfileState>> RecoverAsync(SaveRecoveryRequest request, CancellationToken cancellation) => LoadAsync(cancellation);
            public Task<SaveCommitResult<LocalProfileState>> CreateProfileAsync(LocalProfileState state, CancellationToken cancellation) => throw new NotSupportedException();
            public Task<SaveCommitResult<LocalProfileState>> ConfirmedResetCorruptProfileAsync(SaveRepositoryObservation observation, LocalProfileState state, CancellationToken cancellation) => throw new NotSupportedException();
        }
        private sealed class Store : IPurchasePort
        {
            internal readonly PurchaseEvidence Evidence = new PurchaseEvidence("purchase", "iap-v1:" + new string('a', 64), "native-proof", "GooglePlay", "Sandbox", "example.stp.qa", "remove_ads_qa", false);
            internal readonly PurchaseRevocationEvidence Revocation = new PurchaseRevocationEvidence("iap-v1:" + new string('a', 64), "revoked-proof", "GooglePlay", "Sandbox", "example.stp.qa", "remove_ads_qa");
            private readonly Repository repository;
            internal int NativeConfirms, EvidenceChecks, RevokeAfterCheck;
            internal bool ProofActive = true, NativeConfirmSuccess = true, NativeConfirmThrows;
            private bool finalized;
            internal Store(Repository repository) { this.repository = repository; }
            public event Action<PurchaseEvidence>? EvidenceReceived { add { } remove { } }
            public event Action<PurchaseRevocationEvidence>? EntitlementRevoked { add { } remove { } }
            public bool OwnsVerifiedEvidence(PurchaseEvidence evidence)
            {
                bool verified = ProofActive && ReferenceEquals(evidence, Evidence);
                EvidenceChecks++; if (EvidenceChecks == RevokeAfterCheck) ProofActive = false;
                return verified;
            }
            public bool OwnsVerifiedRevocation(PurchaseRevocationEvidence evidence) => ReferenceEquals(evidence, Revocation);
            public Task<MobileResult<StoreProduct>> PrepareAsync(PurchaseIntent intent, IapPrivacyReadiness? readiness, MobileOperation operation) => Task.FromResult(MobileResult<StoreProduct>.Stop(operation, MobileServiceStatus.Unavailable, "TEST_NOT_USED"));
            public Task<MobileResult<PurchaseEvidence>> PurchaseAsync(MobileOperation operation) => Task.FromResult(MobileResult<PurchaseEvidence>.Success(operation, Evidence));
            public Task<MobileResult<PurchaseAcknowledgement>> RestoreAsync(MobileOperation operation) => Task.FromResult(MobileResult<PurchaseAcknowledgement>.Success(operation, new PurchaseAcknowledgement(string.Empty)));
            public Task<MobileResult<PurchaseAcknowledgement>> FinalizeAsync(DurablePurchaseGrant grant, MobileOperation operation)
            {
                var entitlement = repository.Current.Payload.Entitlements.Records.Single();
                Assert.That(entitlement.Owned, Is.True, "Native finish ran before durable entitlement");
                Assert.That(entitlement.TransactionReference, Is.EqualTo(grant.Evidence.TransactionKey));
                Assert.That(repository.Current.Generation, Is.EqualTo(grant.CommittedGeneration));
                Assert.That(repository.Current.Payload.PendingOperations.OfType<LocalIapOperation>().Single().State, Is.EqualTo("GRANTED_NOT_FINALIZED"));
                if (NativeConfirmThrows) throw new InvalidOperationException("TEST_NATIVE_ACK_EXCEPTION");
                if (!NativeConfirmSuccess) return Task.FromResult(MobileResult<PurchaseAcknowledgement>.Stop(operation, MobileServiceStatus.Unavailable, "TEST_NATIVE_ACK_UNAVAILABLE"));
                if (!finalized) { NativeConfirms++; finalized = true; }
                return Task.FromResult(MobileResult<PurchaseAcknowledgement>.Success(operation, new PurchaseAcknowledgement(grant.Evidence.TransactionKey)));
            }
        }
        private sealed class Consent : IConsentPort
        {
            internal readonly ConsentSnapshot Candidate = new ConsentSnapshot(1, true, true);
            internal bool Effective;
            internal int Applies, Revokes;
            public MobileCapabilities Capabilities => new MobileCapabilities(Effective, true);
            public Task<MobileResult<ConsentSnapshot>> RefreshAsync(MobileOperation operation) => Task.FromResult(MobileResult<ConsentSnapshot>.Success(operation, Candidate));
            public Task<MobileResult<ConsentSnapshot>> ShowPrivacyOptionsAsync(MobileOperation operation) => RefreshAsync(operation);
            public bool ConfirmDecisionPersisted(ConsentSnapshot snapshot, long committedGeneration) { Applies++; Effective = ReferenceEquals(snapshot, Candidate) && committedGeneration > 1; return Effective; }
            public void Revoke() { Revokes++; Effective = false; }
        }
        private static MobileOperation Finalization() => new MobileOperation("finalize:" + Guid.NewGuid().ToString("N"), TimeSpan.FromSeconds(1));

        private static PersistedMobileTransactions BoundBridge(CatalogUse use, string applicationId, bool production)
        {
            var f = new Fixture();
            string ns = use == CatalogUse.InternalPreview ? "qa-mobile-candidate" : use == CatalogUse.IsolatedReview ? "qa-public240-v1" : "mobile-profile-test";
            var status = use == CatalogUse.Production ? CatalogApprovalStatus.ProductApproved : use == CatalogUse.ContractTest ? CatalogApprovalStatus.FixtureOnly : CatalogApprovalStatus.Draft;
            CatalogIdentity Id(string id, char hash) => new CatalogIdentity(id, 1, new string(hash, 64), use, status);
            var p = Id(ns + "-public", 'a');
            var c = Id(use == CatalogUse.InternalPreview ? "campaign-qa-mobile-candidate" : ns + "-campaign", 'b');
            var v = Id(use == CatalogUse.InternalPreview ? "completion-qa-mobile-candidate" : ns + "-completion", 'c');
            string projection = "{\"publicCatalog\":" + LocalSaveHash.CatalogProjection(p) + ",\"campaignCatalog\":" + LocalSaveHash.CatalogProjection(c) + ",\"completionCatalog\":" + LocalSaveHash.CatalogProjection(v) + "}";
            var a = new CatalogIdentity(ns + "-aggregate", 1, LocalSaveHash.Bytes(LocalSaveHash.Canonical(LocalSaveContractFixtures.Json, projection)), use, status);
            var catalogs = CatalogSetIdentity.Create(p, c, v, a, LocalSaveContractFixtures.Json);
            return new PersistedMobileTransactions(f.Repo, new SaveProfileIdentity(ns, "provider-fence", use), catalogs,
                new LocalSaveContractFixtures.Dispatcher(), new LocalSaveContractFixtures.Clock(), new MobilePurchaseBinding("GooglePlay", applicationId, "remove_ads", production));
        }
        [Test]
        public void CandidateAndProductionProviderBindingsCannotCrossOrPromote()
        {
            Assert.DoesNotThrow(() => BoundBridge(CatalogUse.InternalPreview, MobileApplicationProfile.CandidateApplicationId, false));
            Assert.DoesNotThrow(() => BoundBridge(CatalogUse.Production, "example.stp", true));
            Assert.Throws<ArgumentException>(() => BoundBridge(CatalogUse.InternalPreview, "example.stp", true));
            Assert.Throws<ArgumentException>(() => BoundBridge(CatalogUse.InternalPreview, "example.stp.dev", false));
            Assert.Throws<ArgumentException>(() => BoundBridge(CatalogUse.InternalPreview, "other.stp.qa", false));
            Assert.Throws<ArgumentException>(() => BoundBridge(CatalogUse.Production, MobileApplicationProfile.CandidateApplicationId, false));
            Assert.Throws<ArgumentException>(() => BoundBridge(CatalogUse.ContractTest, "example.stp", true));
            Assert.Throws<ArgumentException>(() => BoundBridge(CatalogUse.IsolatedReview, "example.stp.qa", false));
        }

        [Test]
        public void ConsentOnlyBridgePersistsPrivacyButCannotMutatePurchasesWithoutActualStoreBinding()
        {
            var f = new Fixture(); var store = new Store(f.Repo); var consent = new Consent();
            var transactions = PersistedMobileTransactions.ForConsent(f.Repo, f.Data.Profile, f.Data.Catalogs, new LocalSaveContractFixtures.Dispatcher(), new LocalSaveContractFixtures.Clock());
            Assert.That(transactions.PersistConsentAsync(consent, consent.Candidate, CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.True);
            int committed = f.Repo.CommitAttempts;
            Assert.That(transactions.ApplyPurchaseAsync(store.Evidence, store, Finalization(), CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.False);
            Assert.That(transactions.ApplyVerifiedRevocationAsync(store.Revocation, store, CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.False);
            Assert.That(f.Repo.CommitAttempts, Is.EqualTo(committed)); Assert.That(store.NativeConfirms, Is.Zero); Assert.That(store.EvidenceChecks, Is.Zero);
        }

        [Test]
        public void ForgedPurchaseDtoCannotWriteOrGrantBeforeTheAdapterProvenanceCheck()
        {
            var f = new Fixture(); var store = new Store(f.Repo); var e = store.Evidence;
            var forged = new PurchaseEvidence(e.OperationId, e.TransactionKey, e.Handle, e.Store, e.Environment, e.ApplicationId, e.StoreProductId, false);
            var result = f.Transactions.ApplyPurchaseAsync(forged, store, Finalization(), CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(result.Succeeded, Is.False); Assert.That(f.Repo.CommitAttempts, Is.Zero);
            Assert.That(PersistedMobileTransactions.HasRemoveAds(f.Repo.Current.Payload), Is.False); Assert.That(store.NativeConfirms, Is.Zero);
        }
        [Test]
        public void GrantAndOperationAreDurableTogetherBeforeNativeFinishAndReplayIsNoOp()
        {
            var f = new Fixture(); var store = new Store(f.Repo); var original = f.Repo.Current.Payload;
            var result = f.Transactions.ApplyPurchaseAsync(store.Evidence, store, Finalization(), CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(result.Succeeded, Is.True); Assert.That(store.NativeConfirms, Is.EqualTo(1)); Assert.That(f.Repo.CommitAttempts, Is.EqualTo(4));
            Assert.That(PersistedMobileTransactions.HasRemoveAds(f.Repo.Current.Payload), Is.True);
            Assert.That(f.Repo.Current.Payload.PendingOperations.OfType<LocalIapOperation>().Single().State, Is.EqualTo("FINALIZED"));
            Assert.That(f.Repo.Current.Payload.EconomyJournal.Count, Is.EqualTo(original.EconomyJournal.Count)); Assert.That(f.Repo.Current.Payload.LedgerCheckpoint, Is.SameAs(original.LedgerCheckpoint));
            Assert.That(f.Repo.Current.Payload.Metagame, Is.SameAs(original.Metagame));
            Assert.That(f.Transactions.ApplyPurchaseAsync(store.Evidence, store, Finalization(), CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.True);
            Assert.That(f.Repo.CommitAttempts, Is.EqualTo(4)); Assert.That(store.NativeConfirms, Is.EqualTo(1));
        }
        [TestCase(1)][TestCase(2)][TestCase(3)][TestCase(4)]
        public void EveryCommitCrashPointPreservesGrantBeforeFinalizeAndReplayRecovers(int failCommit)
        {
            var f = new Fixture(); var store = new Store(f.Repo); f.Repo.FailCommit = failCommit;
            Assert.That(f.Transactions.ApplyPurchaseAsync(store.Evidence, store, Finalization(), CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.False);
            Assert.That(store.NativeConfirms, Is.EqualTo(failCommit == 4 ? 1 : 0));
            Assert.That(PersistedMobileTransactions.HasRemoveAds(f.Repo.Current.Payload), Is.EqualTo(failCommit == 4));
            f.Repo.FailCommit = 0;
            Assert.That(f.Transactions.ApplyPurchaseAsync(store.Evidence, store, Finalization(), CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.True);
            Assert.That(PersistedMobileTransactions.HasRemoveAds(f.Repo.Current.Payload), Is.True); Assert.That(store.NativeConfirms, Is.EqualTo(1));
        }
        [Test]
        public void ConsentCapabilityRequiresBothDurablePhasesAndSecondPhaseFailureRevokes()
        {
            var f = new Fixture(); var consent = new Consent(); f.Repo.FailCommit = 2;
            Assert.That(f.Transactions.PersistConsentAsync(consent, consent.Candidate, CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.False);
            Assert.That(consent.Effective, Is.False); Assert.That(consent.Revokes, Is.EqualTo(1));
            Assert.That(f.Repo.Current.Payload.DataLifecycle.PrivacyDecision!.NativeSyncState, Is.EqualTo("ENABLE_PENDING"));
            f.Repo.FailCommit = 0;
            Assert.That(f.Transactions.PersistConsentAsync(consent, consent.Candidate, CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.True);
            Assert.That(consent.Effective, Is.True); Assert.That(f.Repo.Current.Payload.DataLifecycle.PrivacyDecision!.NativeSyncState, Is.EqualTo("ENABLED_CONFIRMED"));
            Assert.That(f.Transactions.RevokeConsentAsync(consent, CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.True);
            Assert.That(consent.Effective, Is.False); Assert.That(f.Repo.Current.Payload.DataLifecycle.PrivacyDecision!.NativeSyncState, Is.EqualTo("REVOKED_CONFIRMED"));
        }
        [Test]
        public void ForgedRevocationCannotRemoveAnOwnedEntitlement()
        {
            var f = new Fixture(); var store = new Store(f.Repo);
            f.Transactions.ApplyPurchaseAsync(store.Evidence, store, Finalization(), CancellationToken.None).GetAwaiter().GetResult();
            int committed = f.Repo.CommitAttempts; var e = store.Revocation;
            var forged = new PurchaseRevocationEvidence(e.TransactionKey, e.Handle, e.Store, e.Environment, e.ApplicationId, e.StoreProductId);
            Assert.That(f.Transactions.ApplyVerifiedRevocationAsync(forged, store, CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.False);
            Assert.That(f.Repo.CommitAttempts, Is.EqualTo(committed)); Assert.That(PersistedMobileTransactions.HasRemoveAds(f.Repo.Current.Payload), Is.True);
            Assert.That(f.Transactions.ApplyVerifiedRevocationAsync(e, store, CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.True);
            Assert.That(PersistedMobileTransactions.HasRemoveAds(f.Repo.Current.Payload), Is.False);
            Assert.That(f.Transactions.ApplyPurchaseAsync(store.Evidence, store, Finalization(), CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.False);
        }
        [Test]
        public void VerifiedRevocationBeforeFirstGrantPersistsAndBlocksReplayAfterAdapterRecreation()
        {
            var f = new Fixture(); var store = new Store(f.Repo);
            Assert.That(f.Transactions.ApplyVerifiedRevocationAsync(store.Revocation, store, CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.True);
            Assert.That(f.Repo.Current.Payload.Entitlements.Records.Single().RevocationConfirmed, Is.True);
            Assert.That(PersistedMobileTransactions.HasRemoveAds(f.Repo.Current.Payload), Is.False);
            var recreated = new Store(f.Repo);
            Assert.That(f.Transactions.ApplyPurchaseAsync(recreated.Evidence, recreated, Finalization(), CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.False);
            Assert.That(f.Repo.CommitAttempts, Is.EqualTo(1)); Assert.That(recreated.NativeConfirms, Is.Zero);
            Assert.That(f.Transactions.ApplyVerifiedRevocationAsync(store.Revocation, store, CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.True);
            Assert.That(f.Repo.CommitAttempts, Is.EqualTo(1));
        }
        [TestCase(1)][TestCase(2)][TestCase(3)]
        public void EvidenceRevocationAroundWriterAndCommitWaitsCannotGrantOrFinish(int revokeAfterCheck)
        {
            var f = new Fixture(); var store = new Store(f.Repo) { RevokeAfterCheck = revokeAfterCheck };
            var result = f.Transactions.ApplyPurchaseAsync(store.Evidence, store, Finalization(), CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(result.Succeeded, Is.False); Assert.That(result.ErrorCode, Is.EqualTo("IAP_ADAPTER_EVIDENCE_REVOKED"));
            Assert.That(PersistedMobileTransactions.HasRemoveAds(f.Repo.Current.Payload), Is.False);
            Assert.That(store.NativeConfirms, Is.Zero); Assert.That(f.Repo.CommitAttempts, Is.EqualTo(revokeAfterCheck - 1));
        }

        [TestCase(false)][TestCase(true)]
        public void KnownNativeAckPendingKeepsTheConfirmedGrantPlayableAndSameEvidenceCanFinalizeOnExplicitRetry(bool nativeThrows)
        {
            var f = new Fixture(); var store = new Store(f.Repo) { NativeConfirmSuccess = false, NativeConfirmThrows = nativeThrows };
            var use = new PersistedPuzzleSession(f.Repo, f.Data.Puzzles, f.Data.Policy, f.Data.Profile, f.Data.Catalogs, new LocalSaveContractFixtures.Clock(), new LocalSaveContractFixtures.Dispatcher());
            Assert.That(use.LoadAsync(CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.True);
            var result = use.RunExternalProfileActionAsync(ct => f.Transactions.ApplyPurchaseAsync(store.Evidence, store, Finalization(), ct), CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(result.Succeeded, Is.True); Assert.That(f.Repo.CommitAttempts, Is.EqualTo(3));
            Assert.That(PersistedMobileTransactions.HasRemoveAds(result.Snapshot!.Payload), Is.True);
            Assert.That(result.Snapshot.Payload.PendingOperations.OfType<LocalIapOperation>().Single().State, Is.EqualTo("GRANTED_NOT_FINALIZED"));
            Assert.That(use.Status, Is.EqualTo(PersistedSessionStatus.Ready)); Assert.That(use.CanEnter(f.Data.Puzzles.Puzzles[0].Identity), Is.True);
            Assert.That(store.NativeConfirms, Is.Zero);
            store.NativeConfirmSuccess = true; store.NativeConfirmThrows = false;
            var retried = use.RunExternalProfileActionAsync(ct => f.Transactions.ApplyPurchaseAsync(store.Evidence, store, Finalization(), ct), CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(retried.Succeeded, Is.True); Assert.That(store.NativeConfirms, Is.EqualTo(1));
            Assert.That(retried.Snapshot!.Payload.PendingOperations.OfType<LocalIapOperation>().Single().State, Is.EqualTo("FINALIZED"));
        }

        [Test]
        public void GuaranteedCasConflictReloadsFreshStateAndNeverFinishesAnUncommittedGrant()
        {
            var f = new Fixture(); var store = new Store(f.Repo); f.Repo.Conflicts = 1;
            Assert.That(f.Transactions.ApplyPurchaseAsync(store.Evidence, store, Finalization(), CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.True);
            Assert.That(f.Repo.CommitAttempts, Is.EqualTo(5)); Assert.That(store.NativeConfirms, Is.EqualTo(1));
        }
    }
}
