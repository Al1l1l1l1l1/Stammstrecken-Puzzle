using System;
using System.Collections.Generic;
using System.Linq;

namespace STP.Application
{
    /// <summary>Uniform integer source for the documented 60/30/10 daily table.</summary>
    public interface ILocalDailyRandom { /// <summary>Returns a uniformly distributed integer 0 through 99.</summary>
        int Next100(); }

    /// <summary>Pure ADR037 hint-credit and daily policies; callers must commit the complete returned payload before SDK or success UI.</summary>
    public sealed class LocalMetagamePolicy
    {
        private readonly IPublicPuzzleCatalog puzzles;
        private readonly CatalogIdentity dailyCatalog;
        private readonly ILocalLedger ledger;
        /// <summary>Uses exact locked profile/catalog provenance. Review catalogs never acquire production metagame.</summary>
        public LocalMetagamePolicy(IPublicPuzzleCatalog puzzles, CatalogIdentity dailyCatalog, ILocalLedger ledger)
        {
            this.puzzles = puzzles ?? throw new ArgumentNullException(nameof(puzzles)); this.dailyCatalog = dailyCatalog ?? throw new ArgumentNullException(nameof(dailyCatalog)); this.ledger = ledger ?? throw new ArgumentNullException(nameof(ledger));
            LocalModel.Require(puzzles.Identity.Use != CatalogUse.IsolatedReview && puzzles.Identity.Use == dailyCatalog.Use, "METAGAME_CATALOG_USE_INVALID");
        }
        private LocalMetagameState Binding(LocalProfileState state)
        {
            LocalModel.Require(state.Profile.CatalogSet.PublicCatalog.Equals(puzzles.Identity) && state.Profile.CatalogSet.CompletionCatalog.Equals(dailyCatalog), "METAGAME_PROFILE_BINDING_MISMATCH");
            LocalModel.Require(state.HintEntitlement == null && state.DailyStatus == null, "SAVE_MIGRATION_NEEDS_DECISION");
            ledger.Validate(state); return state.Metagame ?? LocalMetagameState.Empty;
        }
        private void Identity(LocalProfileState state, PublicPuzzleIdentity identity)
        {
            LocalModel.Require(puzzles.TryGet(identity, out _) || state.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Any(d => d.PublicPuzzleInput.Identity.Equals(identity)), "HINT_IDENTITY_NOT_ACTIVE");
        }
        private bool ActiveIdentity(LocalProfileState state,PublicPuzzleIdentity identity)=>puzzles.TryGet(identity,out _)||state.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Any(d=>d.PublicPuzzleInput.Identity.Equals(identity));
        private static LocalProfileState Hints(LocalProfileState state, LocalMetagameState data, LocalHintRecord hint)
            => state.WithMetagame(new LocalMetagameState(data.Hints.Where(h => !h.Identity.Equals(hint.Identity)).Concat(new[] { hint }).OrderBy(h => h.Identity.PuzzleId + ":" + h.Identity.HashSha256, StringComparer.Ordinal).ToArray(), data.Daily));
        private static LocalProfileState Daily(LocalProfileState state, LocalMetagameState data, LocalDailyClaim claim)
            => state.WithMetagame(new LocalMetagameState(data.Hints, data.Daily.Where(c => c.ClaimId != claim.ClaimId).Concat(new[] { claim }).OrderBy(c => c.ClaimId, StringComparer.Ordinal).ToArray()));
        private static LocalHintRecord Hint(LocalMetagameState data, PublicPuzzleIdentity identity) => data.Hints.SingleOrDefault(h => h.Identity.Equals(identity)) ?? new LocalHintRecord(identity, 0, 0, null, Array.Empty<string>());
        /// <summary>Returns three regular identity-bound credits plus only an actually confirmed video credit.</summary>
        public int HintCredits(LocalProfileState state, PublicPuzzleIdentity identity) { var data = Binding(state); Identity(state, identity); return Hint(data, identity).AvailableCredits; }
        /// <summary>Reserves the only video grant only after all existing credits have been used.</summary>
        public LocalProfileState ReserveHintVideo(LocalProfileState state, PublicPuzzleIdentity identity, string operationId, long targetGeneration)
        {
            var data = Binding(state); Identity(state, identity); var hint = Hint(data, identity);
            LocalModel.Require(hint.AvailableCredits == 0 && (hint.Reward == null || hint.Reward.State == "CLOSED_NO_REWARD"), "HINT_VIDEO_NOT_ELIGIBLE");
            LocalModel.Require(hint.Reward?.OperationId != operationId, "HINT_OPERATION_ID_REUSED");
            var reward = new LocalMetagameReward(LocalHintRecord.ClaimId(identity), operationId, targetGeneration, "RESERVED");
            return Hints(state, data, new LocalHintRecord(identity, hint.RegularConsumed, hint.RewardedConsumed, reward, hint.DisplayedIds));
        }
        private static bool Matches(LocalMetagameReward? record, AdRewardEvidence evidence)
            => record != null && record.ClaimId == evidence.Reservation.ClaimId && record.OperationId == evidence.Reservation.OperationId && record.OperationId == evidence.ProviderOperationId && record.ReservationGeneration == evidence.Reservation.SaveGeneration;
        /// <summary>Consumes no credit: one matching late or current provider callback confirms one persistent extra credit.</summary>
        public LocalProfileState ConfirmHintVideo(LocalProfileState state, PublicPuzzleIdentity identity, AdRewardEvidence evidence)
        {
            var data = Binding(state); if(!ActiveIdentity(state,identity))return state; var hint = Hint(data, identity);
            if (!Matches(hint.Reward, evidence) || hint.Reward!.State == "CLOSED_NO_REWARD" || hint.Reward.State == "CONFIRMED") return state;
            var old = hint.Reward; var reward = new LocalMetagameReward(old.ClaimId, old.OperationId, old.ReservationGeneration, "CONFIRMED");
            return Hints(state, data, new LocalHintRecord(identity, hint.RegularConsumed, hint.RewardedConsumed, reward, hint.DisplayedIds));
        }
        /// <summary>Marks an unresolved show or a definitive no-reward result without granting or consuming a credit.</summary>
        public LocalProfileState ResolveHintVideo(LocalProfileState state, PublicPuzzleIdentity identity, AdRewardReservation reservation, bool definitelyNoReward)
        {
            var data = Binding(state); if(!ActiveIdentity(state,identity))return state; var hint = Hint(data, identity); var old = hint.Reward;
            if (old == null || old.ClaimId != reservation.ClaimId || old.OperationId != reservation.OperationId || old.ReservationGeneration != reservation.SaveGeneration || old.State == "CONFIRMED" || old.State == "CLOSED_NO_REWARD") return state;
            var reward = new LocalMetagameReward(old.ClaimId, old.OperationId, old.ReservationGeneration, definitelyNoReward ? "CLOSED_NO_REWARD" : "RECONCILIATION_REQUIRED");
            return Hints(state, data, new LocalHintRecord(identity, hint.RegularConsumed, hint.RewardedConsumed, reward, hint.DisplayedIds));
        }
        /// <summary>Charges only a real acknowledged safe display and deduplicates its immutable ID.</summary>
        public LocalProfileState ConsumeHintDisplay(LocalProfileState state, SafeHintDisplayReceipt receipt)
        {
            if (receipt == null) throw new ArgumentNullException(nameof(receipt));
            var data = Binding(state); Identity(state, receipt.Identity); var hint = Hint(data, receipt.Identity);
            if(!puzzles.TryGet(receipt.Identity,out var bound))bound=state.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single(d=>d.PublicPuzzleInput.Identity.Equals(receipt.Identity)).PublicPuzzleInput;
            var offered=receipt.Puzzle;
            LocalModel.Require(bound!=null&&bound.Width==offered.Width&&bound.Height==offered.Height&&bound.RulesetVersion==offered.RulesetVersion&&bound.A.Side==offered.A.Side&&bound.A.Index==offered.A.Index&&bound.B.Side==offered.B.Side&&bound.B.Index==offered.B.Index&&bound.RowCounts.SequenceEqual(offered.RowCounts)&&bound.ColumnCounts.SequenceEqual(offered.ColumnCounts),"HINT_PUBLIC_INPUT_MISMATCH");
            if (hint.DisplayedIds.Contains(receipt.DisplayId.ToString("D"))) return state;
            LocalModel.Require(hint.AvailableCredits > 0, "HINT_NO_CREDIT");
            var draft = state.Drafts.SingleOrDefault(d => d.Identity.Equals(receipt.Identity));
            var active=state.Endless.OpenEndless.OfType<ActiveEndlessDraft>().SingleOrDefault(a=>a.PublicPuzzleInput.Identity.Equals(receipt.Identity));var sourceCheckpoint=draft?.Checkpoint??active?.Checkpoint;
            LocalModel.Require(sourceCheckpoint!=null&&sourceCheckpoint.AttemptId==receipt.AttemptId&&sourceCheckpoint.Revision==receipt.ExpectedRevision,"HINT_DISPLAY_ATTEMPT_MISMATCH");
            var restored = sourceCheckpoint!.Restore(receipt.Puzzle.ToDomainDefinition(), receipt.Identity.PuzzleId, receipt.Identity.HashProfile, receipt.Identity.HashSha256);
            LocalModel.Require(restored.Succeeded && restored.Value != null, "HINT_DISPLAY_CHECKPOINT_INVALID");
            var result = STP.Puzzle.Domain.PuzzleCommandHandler.Handle(restored.Value, new STP.Puzzle.Domain.RecordHintDisplayed(receipt.DisplayId, receipt.ExpectedRevision, receipt.AttemptId), 0);
            LocalModel.Require(result.Error == STP.Puzzle.Domain.DomainError.NONE && result.State != null, "HINT_DISPLAY_SESSION_INVALID");
            int regular = hint.RegularConsumed + (hint.RegularConsumed < 3 ? 1 : 0); int rewarded = hint.RewardedConsumed + (hint.RegularConsumed < 3 ? 0 : 1);
            var updated = new LocalHintRecord(hint.Identity, regular, rewarded, hint.Reward, hint.DisplayedIds.Concat(new[] { receipt.DisplayId.ToString("D") }).ToArray());
            var checkpoint = STP.Puzzle.Domain.PuzzleSessionCheckpoint.Export(result.State!, 0);
            var withDraft = state.WithProgress(state.Levels, state.LedgerCheckpoint, state.EconomyJournal, state.SeasonProgressCache, state.Drafts.Select(d => d.Identity.Equals(receipt.Identity) ? LocalDraft.Create(d.Identity, d.SavedAtUtc, checkpoint) : d).ToArray(), state.RewardClaims);
            if(active!=null)
            {
                var next=ActiveEndlessDraft.Create(active.GenerationOrdinal,active.Id,active.Descriptor,active.PublicPuzzleInput,active.PublicPuzzleHash,active.SolutionHash,active.ProofHash,checkpoint);
                withDraft=LocalEndlessEconomy.WithEndless(state,LocalEndlessState.Runtime(state.Endless.HighestReservedOrdinal,state.Endless.OpenEndless.Select(o=>o.Id==active.Id?next:o).ToArray(),state.Endless.RecentPublicInputs,state.Endless.EconomyReceipts));
            }
            return Hints(withDraft, data, updated);
        }
        private LocalProfileState PruneDaily(LocalProfileState state, long generation)
        {
            var data = Binding(state); if (data.Daily.Count < 767) return state;
            state = ledger.Compact(state, generation); data = state.Metagame!;
            var referenced = new HashSet<string>(state.EconomyJournal.Select(e => e.TransactionId).Concat(state.LedgerCheckpoint.JournalDiagnosticTail.Select(e => e.TransactionId)), StringComparer.Ordinal);
            var recent = new HashSet<string>(data.Daily.Where(c => c.Terminal != null || c.Reward?.State == "CLOSED_NO_REWARD").OrderByDescending(c => c.Terminal?.CommittedGeneration ?? c.Reward!.ReservationGeneration).ThenBy(c => c.ClaimId, StringComparer.Ordinal).Take(64).Select(c => c.ClaimId), StringComparer.Ordinal);
            var kept = data.Daily.Where(c => c.Terminal == null && c.Reward?.State != "CLOSED_NO_REWARD" || referenced.Contains(c.ClaimId) || recent.Contains(c.ClaimId)).ToArray();
            return state.WithMetagame(new LocalMetagameState(data.Hints, kept));
        }
        /// <summary>Fixes the outcome once using the local date supplied at claim, never server time or a future-date lock.</summary>
        public LocalProfileState ReserveDaily(LocalProfileState state, string localDate, bool rewarded, string operationId, long targetGeneration, ILocalDailyRandom random)
        {
            if (random == null) throw new ArgumentNullException(nameof(random));
            LocalModel.Positive(targetGeneration);
            var data = Binding(state); string slot = rewarded ? "REWARDED" : "FREE"; string id = "daily:v1:" + localDate + ":" + slot;
            var existing = data.Daily.SingleOrDefault(c => c.ClaimId == id);
            if (existing != null && (existing.Terminal != null || !rewarded || existing.Reward!.State != "CLOSED_NO_REWARD")) return state;
            state = PruneDaily(state, targetGeneration); data = Binding(state);
            int roll = existing?.Roll ?? random.Next100();
            if (existing?.Reward != null) LocalModel.Require(existing.Reward.OperationId != operationId, "DAILY_OPERATION_ID_REUSED");
            var reward = rewarded ? new LocalMetagameReward(id, operationId, targetGeneration, "RESERVED") : null;
            return Daily(state, data, new LocalDailyClaim(localDate, slot, roll, reward, null));
        }
        private LocalProfileState GrantDaily(LocalProfileState state, LocalDailyClaim claim, long targetGeneration, string utc)
        {
            LocalModel.Positive(targetGeneration);LocalModel.Utc(utc);
            if (claim.Terminal != null) return state;
            state = ledger.Compact(state, targetGeneration); var data = Binding(state);
            var terminal = EconomyTerminal.Create(claim.ClaimId, "DAILY_PATIENCE", claim.Date + ":" + claim.Slot, dailyCatalog, claim.ClaimId, claim.AmountPatience, targetGeneration, utc);
            var journal = state.EconomyJournal.Concat(new[] { ledger.Next(state.LedgerCheckpoint, state.EconomyJournal, terminal) }).ToArray();
            var updated = new LocalDailyClaim(claim.Date, claim.Slot, claim.Roll, claim.Reward, terminal);
            var result = Daily(state, data, updated);
            result = result.WithProgress(result.Levels, result.LedgerCheckpoint, journal, result.SeasonProgressCache, result.Drafts, result.RewardClaims);
            ledger.Validate(result); return ledger.Compact(result, targetGeneration);
        }
        /// <summary>Commits the already fixed free outcome once; a missing reservation never rerolls.</summary>
        public LocalProfileState CommitFreeDaily(LocalProfileState state, string date, long targetGeneration, string utc)
        {
            var data = Binding(state); var claim = data.Daily.SingleOrDefault(c => c.Date == date && c.Slot == "FREE");
            LocalModel.Require(claim != null, "DAILY_RESERVATION_REQUIRED"); return GrantDaily(state, claim!, targetGeneration, utc);
        }
        /// <summary>Atomically confirms the exact reserved video operation and grants its fixed outcome once.</summary>
        public LocalProfileState ConfirmDailyVideo(LocalProfileState state, AdRewardEvidence evidence, long targetGeneration, string utc)
        {
            var data = Binding(state); var claim = data.Daily.SingleOrDefault(c => c.ClaimId == evidence.Reservation.ClaimId);
            if (claim == null || claim.Terminal != null || !Matches(claim.Reward, evidence) || claim.Reward!.State == "CLOSED_NO_REWARD") return state;
            var old = claim.Reward!; var confirmed = new LocalMetagameReward(old.ClaimId, old.OperationId, old.ReservationGeneration, "CONFIRMED");
            return GrantDaily(state, new LocalDailyClaim(claim.Date, claim.Slot, claim.Roll, confirmed, null), targetGeneration, utc);
        }
        /// <summary>Closes only an exact known show or holds uncertainty without changing its selected outcome.</summary>
        public LocalProfileState ResolveDailyVideo(LocalProfileState state, AdRewardReservation reservation, bool definitelyNoReward)
        {
            var data = Binding(state); var claim = data.Daily.SingleOrDefault(c => c.ClaimId == reservation.ClaimId); var old = claim?.Reward;
            if (claim == null || claim.Terminal != null || old == null || old.OperationId != reservation.OperationId || old.ReservationGeneration != reservation.SaveGeneration || old.State == "CONFIRMED" || old.State == "CLOSED_NO_REWARD") return state;
            return Daily(state, data, new LocalDailyClaim(claim.Date, claim.Slot, claim.Roll, new LocalMetagameReward(old.ClaimId, old.OperationId, old.ReservationGeneration, definitelyNoReward ? "CLOSED_NO_REWARD" : "RECONCILIATION_REQUIRED"), null));
        }
    }
}
