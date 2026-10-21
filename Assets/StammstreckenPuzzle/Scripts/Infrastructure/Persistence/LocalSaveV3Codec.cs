using System;
using System.Linq;
using STP.Application;

namespace STP.Infrastructure.Persistence
{
    public sealed partial class LocalSaveCodec
    {
        /// <summary>Explicit writer line; existing isolated QA composition remains on its historical v2 line.</summary>
        public int WriteSchemaVersion { get; private set; } = 2;
        /// <summary>Activates accepted ADR037 without changing the existing QA constructor or promoting its profile.</summary>
        public static LocalSaveCodec ForMetagame(ICanonicalJson json, SaveProfileIdentity profile, CatalogSetIdentity catalogs, IPublicPuzzleCatalog puzzles,
            ILockedCampaignCatalog campaign, ILockedCompletionCatalog completion, RetainedContractBindings? retained = null)
        {
            if (profile.Use == CatalogUse.IsolatedReview) throw new ArgumentException("SAVE_REVIEW_METAGAME_PROMOTION_FORBIDDEN");
            var result = new LocalSaveCodec(json, profile, catalogs, puzzles, campaign, completion, retained); result.WriteSchemaVersion = 3; return result;
        }
        private static LocalProfileState NormalizeV3(LocalProfileState state)
        {
            Require(state.HintEntitlement == null && (state.DailyStatus == null || state.DailyStatus.ConfirmedClaimIds.Count == 0), "SAVE_MIGRATION_NEEDS_DECISION", true);
            if(state.DailyStatus!=null)state=LocalProfileState.Create(state.Profile,state.Levels,state.Endless,state.LedgerCheckpoint,state.EconomyJournal,state.Inventory,state.CosmeticClaimReservations,state.SeasonProgressCache,state.Drafts,state.RewardClaims,null,state.Entitlements,state.PendingOperations,state.Settings,state.DataLifecycle,null).WithMetagame(state.Metagame);
            return state.Metagame == null ? state.WithMetagame(LocalMetagameState.Empty) : state;
        }
        private void ValidateMetagameV3(LocalProfileState state, long? generation)
        {
            var metagame = state.Metagame ?? throw new SaveCodecFailure("SAVE_METAGAME_REQUIRED");
            foreach (var hint in metagame.Hints)
            {
                bool known = puzzles.TryGet(hint.Identity, out _) || state.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Any(d => d.PublicPuzzleInput.Identity.Equals(hint.Identity)) || retained.HistoricalProgress.Any(h => h.Campaign.Entries.Any(e => e.Puzzle.Equals(hint.Identity)));
                Require(known, "HINT_IDENTITY_UNRESOLVED", true);
                if (generation.HasValue && hint.Reward != null) Require(hint.Reward.ReservationGeneration <= generation.Value, "SAVE_RESERVATION_GENERATION_INVALID");
            }
            foreach (var daily in metagame.Daily)
            {
                if (generation.HasValue && daily.Reward != null) Require(daily.Reward.ReservationGeneration <= generation.Value, "SAVE_RESERVATION_GENERATION_INVALID");
                if (daily.Terminal != null)
                {
                    Require(daily.Terminal.Catalog.Equals(completion.Identity), "DAILY_CATALOG_BINDING_INVALID", true);
                    if (generation.HasValue) Require(daily.Terminal.CommittedGeneration <= generation.Value, "SAVE_TERMINAL_GENERATION_INVALID");
                }
            }
        }
        private LocalProfileState ReadStateV3(CanonicalDtoCursor v)
        {
            var f = v.Fields("profile", "levels", "endless", "ledgerCheckpoint", "economyJournal", "inventory", "cosmeticClaimReservations", "seasonProgressCache", "drafts", "rewardClaims", "entitlements", "pendingOperations", "settings", "dataLifecycle", "hintState", "dailyState");
            var hints = f[14].Fields("policyVersion", "records"); hints[0].RequireString("hint-policy-v1");
            var daily = f[15].Fields("policyVersion", "records"); daily[0].RequireString("daily-local-v1");
            var state = LocalProfileState.Create(ReadLocalProfileHeader(f[0]), f[1].Array().Select(ReadLocalLevelProgress).ToArray(), ReadEndless(f[2]), ReadLocalLedgerCheckpoint(f[3]), f[4].Array().Select(ReadLocalStoredEntry).ToArray(), ReadLocalInventory(f[5]), f[6].Array().Select(ReadLocalCosmeticReservation).ToArray(), ReadLocalSeasonProgressCache(f[7]), f[8].Array().Select(ReadLocalDraft).ToArray(), f[9].Array().Select(ReadLocalRewardClaim).ToArray(), null, ReadLocalEntitlements(f[10]), f[11].Array().Select(ReadLocalPendingOperation).ToArray(), ReadLocalSettings(f[12]), ReadLocalDataLifecycle(f[13]), null);
            return state.WithMetagame(new LocalMetagameState(hints[1].Array().Select(ReadHintV3).ToArray(), daily[1].Array().Select(ReadDailyV3).ToArray()));
        }
        private LocalMetagameReward ReadRewardV3(CanonicalDtoCursor value)
        {
            var f = value.Fields("recordVersion", "claimId", "operationId", "providerOperationId", "reservationGeneration", "state"); f[0].RequireInteger(1);
            Require(f[2].String() == f[3].String(), "SAVE_PROVIDER_OPERATION_MISMATCH");
            return new LocalMetagameReward(f[1].String(), f[2].String(), f[4].Integer(), f[5].String());
        }
        private LocalHintRecord ReadHintV3(CanonicalDtoCursor value)
        {
            var f = value.Fields("recordVersion", "identity", "regularConsumed", "rewardedConsumed", "reward", "displayedIds"); f[0].RequireInteger(1);
            return new LocalHintRecord(ReadPublicPuzzleIdentity(f[1]), f[2].Int32(), f[3].Int32(), f[4].IsNull ? null : ReadRewardV3(f[4]), f[5].Array().Select(x => x.String()).ToArray());
        }
        private LocalDailyClaim ReadDailyV3(CanonicalDtoCursor value)
        {
            var f = value.Fields("recordVersion", "date", "slot", "roll", "reward", "terminal"); f[0].RequireInteger(1);
            return new LocalDailyClaim(f[1].String(), f[2].String(), f[3].Int32(), f[4].IsNull ? null : ReadRewardV3(f[4]), f[5].IsNull ? null : ReadEconomyTerminal(f[5]));
        }
        private static string Write(LocalMetagameReward value) => Object(("recordVersion", "1"), ("claimId", Q(value.ClaimId)), ("operationId", Q(value.OperationId)), ("providerOperationId", Q(value.OperationId)), ("reservationGeneration", Write(value.ReservationGeneration)), ("state", Q(value.State)));
        private static string Write(LocalHintRecord value) => Object(("recordVersion", "1"), ("identity", Write(value.Identity)), ("regularConsumed", Write(value.RegularConsumed)), ("rewardedConsumed", Write(value.RewardedConsumed)), ("reward", value.Reward == null ? "null" : Write(value.Reward)), ("displayedIds", "[" + string.Join(",", value.DisplayedIds.Select(Q)) + "]"));
        private string Write(LocalDailyClaim value) => Object(("recordVersion", "1"), ("date", Q(value.Date)), ("slot", Q(value.Slot)), ("roll", Write(value.Roll)), ("reward", value.Reward == null ? "null" : Write(value.Reward)), ("terminal", value.Terminal == null ? "null" : Write(value.Terminal)));
        private string WriteStateV3(LocalProfileState v) => Object(("profile", Write(v.Profile)), ("levels", "[" + string.Join(",", v.Levels.Select(Write)) + "]"), ("endless", Write(v.Endless)), ("ledgerCheckpoint", Write(v.LedgerCheckpoint)), ("economyJournal", "[" + string.Join(",", v.EconomyJournal.Select(Write)) + "]"), ("inventory", Write(v.Inventory)), ("cosmeticClaimReservations", "[" + string.Join(",", v.CosmeticClaimReservations.Select(Write)) + "]"), ("seasonProgressCache", Write(v.SeasonProgressCache)), ("drafts", "[" + string.Join(",", v.Drafts.Select(Write)) + "]"), ("rewardClaims", "[" + string.Join(",", v.RewardClaims.Select(Write)) + "]"), ("entitlements", Write(v.Entitlements)), ("pendingOperations", "[" + string.Join(",", v.PendingOperations.Select(Write)) + "]"), ("settings", Write(v.Settings)), ("dataLifecycle", Write(v.DataLifecycle)), ("hintState", Object(("policyVersion", Q("hint-policy-v1")), ("records", "[" + string.Join(",", v.Metagame!.Hints.Select(Write)) + "]"))), ("dailyState", Object(("policyVersion", Q("daily-local-v1")), ("records", "[" + string.Join(",", v.Metagame!.Daily.Select(Write)) + "]"))));
    }
}
