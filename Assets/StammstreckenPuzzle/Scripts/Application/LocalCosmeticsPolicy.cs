using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace STP.Application
{
    /// <summary>Pure copy-on-write cosmetic result; only repository acknowledgement makes it durable.</summary>
    public sealed class LocalCosmeticsProjection
    {
        internal LocalCosmeticsProjection(LocalProfileState state, string code) { State = state; Code = code; }
        /// <summary>Complete proposed immutable payload.</summary>
        public LocalProfileState State { get; }
        /// <summary>Projected outcome, never an acknowledgement.</summary>
        public string Code { get; }
    }

    /// <summary>Existing cosmetics-v2/ADR028 rules implemented without changing save-v2 or any hash projection.</summary>
    public sealed class LocalCosmeticsPolicy
    {
        private readonly ILockedCosmeticsCatalog cosmetics;
        private readonly ILockedCampaignCatalog campaign;
        private readonly ICanonicalJson json;
        private readonly ILocalLedger ledger;
        /// <summary>Requires an exact runtime-eligible catalog and an equally isolated campaign graph.</summary>
        public LocalCosmeticsPolicy(ILockedCosmeticsCatalog cosmetics, ILockedCampaignCatalog campaign, ICanonicalJson json, ILocalLedger ledger)
        {
            this.cosmetics = cosmetics ?? throw new ArgumentNullException(nameof(cosmetics));
            this.campaign = campaign ?? throw new ArgumentNullException(nameof(campaign));
            this.json = json ?? throw new ArgumentNullException(nameof(json));
            this.ledger = ledger ?? throw new ArgumentNullException(nameof(ledger));
            LocalModel.Require(cosmetics.Identity.Use != CatalogUse.IsolatedReview && cosmetics.Identity.Use == campaign.Identity.Use, "COSMETICS_RUNTIME_CATALOG_REQUIRED");
            LocalModel.Unique(cosmetics.Items.Select(i => i.Id));
            foreach (var item in cosmetics.Items) foreach (string subject in item.RequiredCampaignSubjectIds)
                LocalModel.Require(Members(subject).Length > 0, "COSMETICS_CAMPAIGN_SUBJECT_MISSING");
        }
        private LockedCampaignEntry[] Members(string subject) => campaign.Entries.Where(e => e.SeasonId == subject || e.SectionId == subject || e.RouteId == subject).ToArray();
        private void Binding(LocalProfileState state, string expectedHash)
        {
            LocalModel.Require(expectedHash == cosmetics.Identity.HashSha256, "COSMETICS_CATALOG_HASH_MISMATCH");
            LocalModel.Require(state.Profile.Identity.Use == cosmetics.Identity.Use && state.Profile.CatalogSet.CampaignCatalog.Equals(campaign.Identity), "COSMETICS_PROFILE_BINDING_MISMATCH");
            ledger.Validate(state);
        }
        private CosmeticDefinition Item(string id, string? acquisition = null)
        {
            LocalModel.Require(cosmetics.TryGet(id, out var value) && value != null, "COSMETIC_NOT_FOUND");
            LocalModel.Require(Eligible(value!), "COSMETIC_NOT_RUNTIME_ELIGIBLE");
            if (acquisition != null) LocalModel.Require(value!.Acquisition == acquisition, "COSMETIC_ACQUISITION_MISMATCH");
            return value!;
        }
        private bool Eligible(CosmeticDefinition item)=>item.Status=="ACTIVE"||(cosmetics.Identity.Use==CatalogUse.InternalPreview&&item.Status=="DRAFT");
        private static LocalProfileState Copy(LocalProfileState s, LocalInventory inventory, IReadOnlyList<LocalCosmeticReservation> reservations, IReadOnlyList<LocalStoredEntry>? journal = null)
            => LocalProfileState.Create(s.Profile, s.Levels, s.Endless, s.LedgerCheckpoint, journal ?? s.EconomyJournal, inventory, reservations,
                s.SeasonProgressCache, s.Drafts, s.RewardClaims, s.DailyStatus, s.Entitlements, s.PendingOperations, s.Settings, s.DataLifecycle, s.HintEntitlement).WithMetagame(s.Metagame);
        /// <summary>Grants only ACTIVE DEFAULT items, preserving all existing ownership and choices.</summary>
        public LocalCosmeticsProjection GrantDefaults(LocalProfileState state, string expectedHash, string acquiredAtUtc)
        {
            Binding(state, expectedHash); LocalModel.Utc(acquiredAtUtc);
            var ownership = state.Inventory.Ownership.ToList();
            foreach (var item in cosmetics.Items.Where(i => Eligible(i) && i.Acquisition == "DEFAULT"))
                if (!ownership.Any(o => o.ItemId == item.Id)) ownership.Add(DefaultOwnership.Create(item.Id, item.Type, cosmetics.Identity, acquiredAtUtc));
            if (ownership.Count == state.Inventory.Ownership.Count) return new LocalCosmeticsProjection(state, "ALREADY_OWNED");
            var selected = state.Inventory.SelectedItemIds.ToList();
            foreach (var item in cosmetics.Items.Where(i => Eligible(i) && i.Acquisition == "DEFAULT"))
                if (!selected.Any(id => ownership.Single(o => o.ItemId == id).ItemType == item.Type)) selected.Add(item.Id);
            return new LocalCosmeticsProjection(Copy(state, LocalInventory.Create(ownership.OrderBy(o => o.ItemId, StringComparer.Ordinal).ToArray(), selected.ToArray()), state.CosmeticClaimReservations), "DEFAULTS_GRANTED");
        }
        /// <summary>Projects one exact catalog-priced debit and ownership in one immutable snapshot.</summary>
        public LocalCosmeticsProjection Purchase(LocalProfileState state, string itemId, string expectedHash, long targetGeneration, string acquiredAtUtc)
        {
            Binding(state, expectedHash); LocalModel.Positive(targetGeneration); LocalModel.Utc(acquiredAtUtc);
            var item = Item(itemId, "PATIENCE_PURCHASE");
            if (state.Inventory.Ownership.Any(o => o.ItemId == item.Id)) return new LocalCosmeticsProjection(state, "ALREADY_OWNED");
            LocalModel.Require(ledger.Validate(state) >= item.PricePatience!.Value, "COSMETIC_INSUFFICIENT_PATIENCE");
            state = ledger.Compact(state, targetGeneration);
            var purchase = PurchasedOwnership.Create(item.Id, item.Type, cosmetics.Identity, "cosmetic-purchase:" + item.Id, item.PricePatience.Value, acquiredAtUtc);
            var journal = state.EconomyJournal.Concat(new[] { ledger.NextPurchase(state.LedgerCheckpoint, state.EconomyJournal, purchase) }).ToArray();
            var inventory = LocalInventory.Create(state.Inventory.Ownership.Concat(new[] { purchase }).OrderBy(o => o.ItemId, StringComparer.Ordinal).ToArray(), state.Inventory.SelectedItemIds);
            var result = Copy(state, inventory, state.CosmeticClaimReservations, journal);
            ledger.Validate(result);
            return new LocalCosmeticsProjection(ledger.Compact(result, targetGeneration), "PURCHASED");
        }
        private string Eligibility(LocalProfileState state, CosmeticDefinition item)
        {
            var members = item.RequiredCampaignSubjectIds.SelectMany(Members).Select(e => e.Puzzle).Distinct().OrderBy(p => p.PuzzleId, StringComparer.Ordinal).ToArray();
            LocalModel.Require(members.Length > 0 && members.All(p => state.Levels.Any(l => l.Identity.Equals(p) && l.FirstClear != null)), "COSMETIC_MILESTONE_NOT_ELIGIBLE");
            // Preserve the exact existing ADR028/save-v2 eligibility projection.
            string identities = "[" + string.Join(",", members.Select(p => LocalSaveHash.Quote(p.PuzzleId))) + "]";
            string subjects = "[" + string.Join(",", item.RequiredCampaignSubjectIds.OrderBy(x => x, StringComparer.Ordinal).Select(LocalSaveHash.Quote)) + "]";
            string projection = "{\"catalogId\":" + LocalSaveHash.Quote(cosmetics.Identity.CatalogId) + ",\"catalogRevision\":" + cosmetics.Identity.Revision.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",\"catalogHashSha256\":" + LocalSaveHash.Quote(cosmetics.Identity.HashSha256) + ",\"itemId\":" + LocalSaveHash.Quote(item.Id) + ",\"acquisition\":\"MILESTONE_GRANT\",\"eligibilityContractVersion\":1,\"kind\":\"ALL_FIRST_CLEARS_OF_CAMPAIGN_SUBJECTS\",\"requiredCampaignSubjectIds\":" + subjects + ",\"requiredPuzzleIds\":" + identities + ",\"validatedFirstClearPuzzleIds\":" + identities + "}";
            return LocalSaveHash.Bytes(LocalSaveHash.Canonical(json, projection));
        }
        /// <summary>Persists this projection before commit is requested; grants no ownership or currency.</summary>
        public LocalCosmeticsProjection ReserveMilestone(LocalProfileState state, string itemId, string operationId, string expectedHash, long targetGeneration)
        {
            Binding(state, expectedHash); LocalModel.Text(operationId); LocalModel.Positive(targetGeneration);
            var item = Item(itemId, "MILESTONE_GRANT");
            if (state.Inventory.Ownership.Any(o => o.ItemId == item.Id)) return new LocalCosmeticsProjection(state, "ALREADY_OWNED");
            string hash = Eligibility(state, item); string claim = "cosmetic-milestone-claim:v1:" + cosmetics.Identity.CatalogId + ":" + item.Id;
            var existing = state.CosmeticClaimReservations.SingleOrDefault(r => r.ItemId == item.Id);
            if (existing != null)
            {
                LocalModel.Require(existing.ClaimId == claim && existing.OperationId == operationId && existing.Catalog.Equals(cosmetics.Identity) && existing.EligibilityProjectionHashSha256 == hash, "COS_MILESTONE_CLAIM_COLLISION");
                return new LocalCosmeticsProjection(state, "ALREADY_RESERVED");
            }
            LocalModel.Require(!state.CosmeticClaimReservations.Any(r => r.OperationId == operationId), "COS_MILESTONE_CLAIM_COLLISION");
            var reservation = LocalCosmeticReservation.Create(claim, operationId, targetGeneration, item.Id, cosmetics.Identity, hash);
            return new LocalCosmeticsProjection(Copy(state, state.Inventory, state.CosmeticClaimReservations.Concat(new[] { reservation }).OrderBy(r => r.ClaimId, StringComparer.Ordinal).ToArray()), "RESERVED");
        }
        /// <summary>Consumes the loaded reservation only after every binding and current eligibility have been rechecked.</summary>
        public LocalCosmeticsProjection CommitMilestone(LocalProfileState state, string itemId, string operationId, long claimGeneration, string expectedHash, string acquiredAtUtc)
        {
            Binding(state, expectedHash); LocalModel.Utc(acquiredAtUtc);
            var item = Item(itemId, "MILESTONE_GRANT"); string hash = Eligibility(state, item);
            var owned = state.Inventory.Ownership.SingleOrDefault(o => o.ItemId == item.Id);
            if (owned != null)
            {
                LocalModel.Require(owned is MilestoneOwnership m && m.OperationId == operationId && m.ClaimGeneration == claimGeneration && m.Catalog.Equals(cosmetics.Identity) && m.EligibilityProjectionHashSha256 == hash, "COS_MILESTONE_CLAIM_COLLISION");
                return new LocalCosmeticsProjection(state, "ALREADY_OWNED");
            }
            var reservation = state.CosmeticClaimReservations.SingleOrDefault(r => r.ItemId == item.Id);
            LocalModel.Require(reservation != null && reservation.OperationId == operationId && reservation.ClaimGeneration == claimGeneration && reservation.Catalog.Equals(cosmetics.Identity) && reservation.EligibilityProjectionHashSha256 == hash, "COS_MILESTONE_RESERVATION_MISMATCH");
            var r = reservation!;
            var ownership = MilestoneOwnership.Create(item.Id, item.Type, cosmetics.Identity, r.ClaimId, r.OperationId, r.ClaimGeneration, hash, acquiredAtUtc);
            return new LocalCosmeticsProjection(Copy(state, LocalInventory.Create(state.Inventory.Ownership.Concat(new[] { ownership }).OrderBy(o => o.ItemId, StringComparer.Ordinal).ToArray(), state.Inventory.SelectedItemIds), state.CosmeticClaimReservations.Where(x => x.ClaimId != r.ClaimId).ToArray()), "MILESTONE_GRANTED");
        }
        /// <summary>Selection uses owned IDs, including retained hidden/tombstoned ownership; at most one train and livery.</summary>
        public LocalCosmeticsProjection Select(LocalProfileState state, IReadOnlyList<string> itemIds, string expectedHash)
        {
            Binding(state, expectedHash); var inventory = LocalInventory.Create(state.Inventory.Ownership, itemIds);
            foreach (string type in new[] { "TRAIN_VARIANT", "LIVERY" }) LocalModel.Require(itemIds.Count(id => state.Inventory.Ownership.Single(o => o.ItemId == id).ItemType == type) <= 1, "COSMETIC_SELECTION_CONFLICT");
            return new LocalCosmeticsProjection(Copy(state, inventory, state.CosmeticClaimReservations), "SELECTION_SAVED");
        }
    }

    /// <summary>Loads the authoritative profile for each command and uses repository CAS before returning committed success.</summary>
    public sealed class LocalCosmeticsTransactions
    {
        private readonly ILocalProfileRepository repository;
        private readonly LocalCosmeticsPolicy policy;
        /// <summary>Uses the existing one-writer repository, never a separate cosmetic save.</summary>
        public LocalCosmeticsTransactions(ILocalProfileRepository repository, LocalCosmeticsPolicy policy) { this.repository = repository ?? throw new ArgumentNullException(nameof(repository)); this.policy = policy ?? throw new ArgumentNullException(nameof(policy)); }
        private async Task<SaveCommitResult<LocalProfileState>> Execute(long expectedGeneration, Func<LocalProfileState, long, LocalCosmeticsProjection> project, CancellationToken cancellation)
        {
            var loaded = await repository.LoadAsync(cancellation).ConfigureAwait(false);
            if (!loaded.Succeeded || loaded.Snapshot == null) return SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed, "COSMETICS_PROFILE_LOAD_REQUIRED");
            var snapshot = loaded.Snapshot;
            if (snapshot.Generation != expectedGeneration) return SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Conflict, "COSMETICS_SAVE_GENERATION_CONFLICT");
            LocalModel.Require(expectedGeneration < LocalModel.MaxSafe, "SAVE_GENERATION_EXHAUSTED");
            var proposal = await Task.Run(() => project(snapshot.Payload, expectedGeneration + 1), cancellation).ConfigureAwait(false);
            if (ReferenceEquals(proposal.State, snapshot.Payload)) return SaveCommitResult<LocalProfileState>.Success(snapshot);
            return await repository.CommitAsync(new SaveSnapshot<LocalProfileState>(snapshot.Profile, snapshot.Catalog, expectedGeneration + 1, proposal.State), expectedGeneration, cancellation).ConfigureAwait(false);
        }
        /// <summary>Commits the simultaneous catalog-priced debit and ownership.</summary>
        public Task<SaveCommitResult<LocalProfileState>> PurchaseAsync(string itemId, long expectedGeneration, string expectedHash, string acquiredAtUtc, CancellationToken cancellation)
            => Execute(expectedGeneration, (state, generation) => policy.Purchase(state, itemId, expectedHash, generation, acquiredAtUtc), cancellation);
        /// <summary>Commits the independently recoverable milestone reservation.</summary>
        public Task<SaveCommitResult<LocalProfileState>> ReserveMilestoneAsync(string itemId, string operationId, long expectedGeneration, string expectedHash, CancellationToken cancellation)
            => Execute(expectedGeneration, (state, generation) => policy.ReserveMilestone(state, itemId, operationId, expectedHash, generation), cancellation);
        /// <summary>Reloads the persisted reservation, validates provenance, then commits ownership without a ledger delta.</summary>
        public Task<SaveCommitResult<LocalProfileState>> CommitMilestoneAsync(string itemId, string operationId, long claimGeneration, long expectedGeneration, string expectedHash, string acquiredAtUtc, CancellationToken cancellation)
            => Execute(expectedGeneration, (state, generation) => { LocalModel.Require(claimGeneration <= expectedGeneration, "COS_MILESTONE_RESERVATION_MISMATCH"); return policy.CommitMilestone(state, itemId, operationId, claimGeneration, expectedHash, acquiredAtUtc); }, cancellation);
        /// <summary>Persists default ownership before presentation uses it.</summary>
        public Task<SaveCommitResult<LocalProfileState>> GrantDefaultsAsync(long expectedGeneration, string expectedHash, string acquiredAtUtc, CancellationToken cancellation)
            => Execute(expectedGeneration, (state, generation) => policy.GrantDefaults(state, expectedHash, acquiredAtUtc), cancellation);
        /// <summary>Persists cosmetic presentation choices.</summary>
        public Task<SaveCommitResult<LocalProfileState>> SelectAsync(IReadOnlyList<string> itemIds, long expectedGeneration, string expectedHash, CancellationToken cancellation)
            => Execute(expectedGeneration, (state, generation) => policy.Select(state, itemIds, expectedHash), cancellation);
    }
}
