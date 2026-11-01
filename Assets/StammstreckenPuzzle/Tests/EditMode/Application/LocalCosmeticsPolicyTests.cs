using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;

namespace STP.Tests.Application.EditMode
{
    public sealed class LocalCosmeticsPolicyTests
    {
        private const string Utc = "2026-10-21T00:00:00Z";
        private sealed class Catalog : ILockedCosmeticsCatalog
        {
            internal Catalog(string purchaseStatus = "ACTIVE", string hash = "a", int revision = 1)
            {
                Items = Array.AsReadOnly(new[] {
                    CosmeticDefinition.Create("default-train", "TRAIN_VARIANT", "ACTIVE", "cosmetic.default.title", "stp/default", "DEFAULT", null, Array.Empty<string>()),
                    CosmeticDefinition.Create("purchase-object", "OPERATING_OBJECT", purchaseStatus, "cosmetic.purchase.title", "stp/purchase", "PATIENCE_PURCHASE", 120, Array.Empty<string>()),
                    CosmeticDefinition.Create("season-object", "OPERATING_OBJECT", "ACTIVE", "cosmetic.season.title", "stp/season", "MILESTONE_GRANT", null, new[] { "S1" }) });
                string Q(string s) => LocalSaveHash.Quote(s);
                string source = "{\"schemaVersion\":2,\"catalogId\":\"test-cosmetics\",\"catalogRevision\":" + revision + ",\"approvalStatus\":\"FIXTURE_ONLY\",\"items\":[" + string.Join(",", Items.Select(i => "{\"id\":" + Q(i.Id) + ",\"type\":" + Q(i.Type) + ",\"status\":" + Q(i.Status) + ",\"titleKey\":" + Q(i.TitleKey) + ",\"assetAddress\":" + Q(i.AssetAddress) + ",\"acquisition\":" + Q(i.Acquisition) + (i.PricePatience.HasValue ? ",\"pricePatience\":" + i.PricePatience.Value : i.Acquisition == "MILESTONE_GRANT" ? ",\"milestoneEligibility\":{\"contractVersion\":1,\"kind\":\"ALL_FIRST_CLEARS_OF_CAMPAIGN_SUBJECTS\",\"requiredCampaignSubjectIds\":[\"S1\"]}" : "") + "}")) + "]}";
                Source = LocalSaveHash.Canonical(LocalSaveContractFixtures.Json, source);
                Identity = new CatalogIdentity("test-cosmetics", revision, LocalSaveHash.Bytes(Source), CatalogUse.ContractTest, CatalogApprovalStatus.FixtureOnly);
            }
            public CatalogIdentity Identity { get; }
            internal byte[] Source { get; }
            public IReadOnlyList<CosmeticDefinition> Items { get; }
            public bool TryGet(string itemId, out CosmeticDefinition? item) { item = Items.SingleOrDefault(x => x.Id == itemId); return item != null; }
        }
        private sealed class Fixture
        {
            internal readonly LocalSaveContractFixtures.Setup Saves = new LocalSaveContractFixtures.Setup();
            internal readonly Catalog Catalog;
            internal readonly LocalCosmeticsPolicy Policy;
            internal Fixture(string status = "ACTIVE") { Catalog = new Catalog(status); Policy = new LocalCosmeticsPolicy(Catalog, Saves.Campaign, LocalSaveContractFixtures.Json, Saves.Ledger); }
            internal string Hash => Catalog.Identity.HashSha256;
            internal LocalProfileState Earned => Saves.Complete(Saves.Complete(Saves.Empty, Saves.Solved(0), 2).State, Saves.Solved(1), 3).State;
        }
        [Test]
        public void PurchaseCommitsOneExactDebitAndOwnershipWithoutMutatingSourceAndReplayCostsNothing()
        {
            var f = new Fixture(); var source = f.Earned; long balance = f.Saves.Ledger.Validate(source);
            var result = f.Policy.Purchase(source, "purchase-object", f.Hash, 4, Utc);
            Assert.That(result.Code, Is.EqualTo("PURCHASED"));
            Assert.That(f.Saves.Ledger.Validate(result.State), Is.EqualTo(balance - 120));
            var debit = result.State.EconomyJournal.Single(x => x.ReasonCode == "COSMETIC_PURCHASE");
            Assert.That(debit.Amount, Is.EqualTo(-120)); Assert.That(debit.ClaimId, Is.Null);
            var owned = result.State.Inventory.Ownership.OfType<PurchasedOwnership>().Single();
            Assert.That(debit.TransactionId, Is.EqualTo(owned.TransactionId)); Assert.That(debit.CatalogHashSha256, Is.EqualTo(f.Hash));
            Assert.That(source.Inventory.Ownership, Is.Empty); Assert.That(f.Saves.Ledger.Validate(source), Is.EqualTo(balance));
            Assert.That(f.Policy.Purchase(result.State, "purchase-object", f.Hash, 5, Utc).State, Is.SameAs(result.State));
        }
        [TestCase("DRAFT")][TestCase("HIDDEN")][TestCase("TOMBSTONE")]
        public void InactiveItemsCannotBeBought(string status)
        { var f = new Fixture(status); Assert.That(Assert.Throws<ArgumentException>(() => f.Policy.Purchase(f.Earned, "purchase-object", f.Hash, 4, Utc))!.Message, Is.EqualTo("COSMETIC_NOT_RUNTIME_ELIGIBLE")); }
        [Test]
        public void InsufficientBalanceAndWrongCatalogStopBeforeMutation()
        {
            var f = new Fixture(); Assert.That(Assert.Throws<ArgumentException>(() => f.Policy.Purchase(f.Saves.Empty, "purchase-object", f.Hash, 2, Utc))!.Message, Is.EqualTo("COSMETIC_INSUFFICIENT_PATIENCE"));
            Assert.That(Assert.Throws<ArgumentException>(() => f.Policy.Purchase(f.Earned, "purchase-object", new string('b', 64), 4, Utc))!.Message, Is.EqualTo("COSMETICS_CATALOG_HASH_MISMATCH"));
        }
        [Test]
        public void DefaultIsDurableOwnershipAndSelectionNeverInventsOwnership()
        {
            var f = new Fixture(); var result = f.Policy.GrantDefaults(f.Saves.Empty, f.Hash, Utc);
            Assert.That(result.State.Inventory.Ownership.Single(), Is.TypeOf<DefaultOwnership>());
            Assert.That(result.State.Inventory.SelectedItemIds, Is.EqualTo(new[] { "default-train" }));
            Assert.That(f.Policy.GrantDefaults(result.State, f.Hash, Utc).State, Is.SameAs(result.State));
            Assert.Throws<ArgumentException>(() => f.Policy.Select(result.State, new[] { "purchase-object" }, f.Hash));
            Assert.That(f.Saves.Ledger.Validate(result.State), Is.Zero);
        }
        [Test]
        public void MilestoneNeedsEveryFirstClearAndItsPersistedReservationBeforeOwnership()
        {
            var f = new Fixture(); Assert.Throws<ArgumentException>(() => f.Policy.ReserveMilestone(f.Saves.Empty, "season-object", "op", f.Hash, 2));
            var source = f.Earned;
            Assert.That(Assert.Throws<ArgumentException>(() => f.Policy.CommitMilestone(source, "season-object", "op", 4, f.Hash, Utc))!.Message, Is.EqualTo("COS_MILESTONE_RESERVATION_MISMATCH"));
            var reserve = f.Policy.ReserveMilestone(source, "season-object", "op", f.Hash, 4);
            Assert.That(reserve.State.Inventory.Ownership, Is.Empty); Assert.That(reserve.State.CosmeticClaimReservations.Count, Is.EqualTo(1));
            Assert.That(f.Policy.ReserveMilestone(reserve.State, "season-object", "op", f.Hash, 5).State, Is.SameAs(reserve.State));
            Assert.Throws<ArgumentException>(() => f.Policy.CommitMilestone(reserve.State, "season-object", "other", 4, f.Hash, Utc));
            Assert.Throws<ArgumentException>(() => f.Policy.CommitMilestone(reserve.State, "season-object", "op", 3, f.Hash, Utc));
            var commit = f.Policy.CommitMilestone(reserve.State, "season-object", "op", 4, f.Hash, Utc);
            Assert.That(commit.State.CosmeticClaimReservations, Is.Empty); Assert.That(commit.State.Inventory.Ownership.Single(), Is.TypeOf<MilestoneOwnership>());
            Assert.That(f.Saves.Ledger.Validate(commit.State), Is.EqualTo(f.Saves.Ledger.Validate(source)));
            Assert.That(f.Policy.CommitMilestone(commit.State, "season-object", "op", 4, f.Hash, Utc).State, Is.SameAs(commit.State));
            var changed = new Catalog(hash: "b", revision: 2); var other = new LocalCosmeticsPolicy(changed, f.Saves.Campaign, LocalSaveContractFixtures.Json, f.Saves.Ledger);
            Assert.Throws<ArgumentException>(() => other.CommitMilestone(reserve.State, "season-object", "op", 4, changed.Identity.HashSha256, Utc));
        }
        [Test]
        public void PurchasedAndReservedStatesRoundTripThroughUnchangedSaveV2Codec()
        {
            var f = new Fixture(); var bought = f.Policy.Purchase(f.Earned, "purchase-object", f.Hash, 4, Utc).State;
            var reserve = f.Policy.ReserveMilestone(bought, "season-object", "op", f.Hash, 5).State;
            var campaignJson = LocalSaveContractFixtures.Goldens.Get("catalogs").Get("campaignCatalog");
            var dom = campaignJson.GetType().GetField("value", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(campaignJson)!;
            var serializer = Assembly.Load("STP.Infrastructure.Content").GetType("STP.Infrastructure.Content.JcsSerializer", true)!;
            byte[] campaignBytes = (byte[])LocalSaveContractFixtures.Get(serializer.GetMethod("Serialize")!.Invoke(null, new[] { dom })!, "Utf8")!;
            var retainedType = Assembly.Load("STP.Infrastructure.Persistence").GetType("STP.Infrastructure.Persistence.RetainedContractBindings", true)!;
            var retained = retainedType.GetMethod("ForCosmetics")!.Invoke(null, new object[] { f.Catalog.Identity, f.Saves.CampaignId, f.Catalog.Source, campaignBytes, f.Saves.Puzzles, LocalSaveContractFixtures.Json })!;
            var wrongHash = new CatalogIdentity(f.Catalog.Identity.CatalogId, 1, new string('b', 64), CatalogUse.ContractTest, CatalogApprovalStatus.FixtureOnly);
            Assert.Throws<TargetInvocationException>(() => retainedType.GetMethod("ForCosmetics")!.Invoke(null, new object[] { wrongHash, f.Saves.CampaignId, f.Catalog.Source, campaignBytes, f.Saves.Puzzles, LocalSaveContractFixtures.Json }));
            var production = new CatalogIdentity(f.Catalog.Identity.CatalogId, 1, f.Hash, CatalogUse.Production, CatalogApprovalStatus.ProductApproved);
            Assert.Throws<TargetInvocationException>(() => retainedType.GetMethod("ForCosmetics")!.Invoke(null, new object[] { production, f.Saves.CampaignId, f.Catalog.Source, campaignBytes, f.Saves.Puzzles, LocalSaveContractFixtures.Json }));
            var codec = LocalSaveContractFixtures.Construct<object>("STP.Infrastructure.Persistence", "STP.Infrastructure.Persistence.LocalSaveCodec", LocalSaveContractFixtures.Json, f.Saves.Profile, f.Saves.Catalogs, f.Saves.Puzzles, f.Saves.Campaign, f.Saves.Completion, retained, null!, null!);
            var snapshot = new SaveSnapshot<LocalProfileState>(f.Saves.Profile, f.Saves.Aggregate, 5, reserve);
            var encoded = (byte[])LocalSaveContractFixtures.Call(codec, "Encode", snapshot, Utc, "test", "test")!;
            var decoded = LocalSaveContractFixtures.Call(codec, "Decode", encoded)!;
            Assert.That(LocalSaveContractFixtures.Get(decoded, "Status")!.ToString(), Is.EqualTo("Valid"));
            Assert.That(LocalSaveContractFixtures.Get(decoded, "SourceSchemaVersion"), Is.EqualTo(2));
            var recovered = ((SaveSnapshot<LocalProfileState>)LocalSaveContractFixtures.Get(decoded, "Snapshot")!).Payload;
            Assert.That(recovered.Inventory.Ownership.OfType<PurchasedOwnership>().Single().ItemId, Is.EqualTo("purchase-object"));
            Assert.That(f.Policy.CommitMilestone(recovered, "season-object", "op", 5, f.Hash, Utc).State.Inventory.Ownership.OfType<MilestoneOwnership>().Count(), Is.EqualTo(1));
        }
        [Test]
        public async Task FailedWriteReturnsNoSuccessAndGenerationConflictCannotDebitTwice()
        {
            var f = new Fixture(); var source = f.Earned; var snapshot = new SaveSnapshot<LocalProfileState>(f.Saves.Profile, f.Saves.Aggregate, 3, source);
            var repository = new LocalSaveContractFixtures.Repository { Loaded = SaveLoadResult<LocalProfileState>.Success(snapshot), OnCommit = (s, g, c) => Task.FromResult(SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed, "TEST_DISK_FULL")) };
            var service = new LocalCosmeticsTransactions(repository, f.Policy);
            Assert.That((await service.PurchaseAsync("purchase-object", 3, f.Hash, Utc, CancellationToken.None)).Succeeded, Is.False);
            Assert.That(repository.Loaded.Snapshot!.Payload.Inventory.Ownership, Is.Empty);
            Assert.That(repository.LastOffered!.Payload.Inventory.Ownership.Count, Is.EqualTo(1));
            repository.OnCommit = (s, g, c) => { repository.Loaded = SaveLoadResult<LocalProfileState>.Success(s); return Task.FromResult(SaveCommitResult<LocalProfileState>.Success(s)); };
            Assert.That((await service.PurchaseAsync("purchase-object", 3, f.Hash, Utc, CancellationToken.None)).Succeeded, Is.True);
            int writes = repository.Commits;
            Assert.That((await service.PurchaseAsync("purchase-object", 3, f.Hash, Utc, CancellationToken.None)).Succeeded, Is.False);
            Assert.That(repository.Commits, Is.EqualTo(writes)); Assert.That(f.Saves.Ledger.Validate(repository.Loaded.Snapshot!.Payload), Is.EqualTo(f.Saves.Ledger.Validate(source) - 120));
        }
    }
}
