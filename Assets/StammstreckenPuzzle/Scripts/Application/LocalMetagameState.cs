using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace STP.Application
{
    /// <summary>Save-v3 binding reserved durably before a rewarded show. Provider key is the same locally allocated operation.</summary>
    public sealed class LocalMetagameReward
    {
        /// <summary>Copies a closed operation record; provider tokens are never accepted.</summary>
        public LocalMetagameReward(string claimId, string operationId, long reservationGeneration, string state)
        {
            ClaimId = LocalModel.Text(claimId); OperationId = LocalModel.Text(operationId);
            ReservationGeneration = LocalModel.Positive(reservationGeneration);
            State = LocalModel.One(state, "RESERVED|RECONCILIATION_REQUIRED|CONFIRMED|CLOSED_NO_REWARD");
        }
        /// <summary>Stable business claim.</summary>
        public string ClaimId { get; }
        /// <summary>Local and provider audit key, fixed before SDK show.</summary>
        public string OperationId { get; }
        /// <summary>Generation that originally committed the reservation.</summary>
        public long ReservationGeneration { get; }
        /// <summary>Closed lifecycle state.</summary>
        public string State { get; }
    }

    /// <summary>Exactly three regular uses and at most one confirmed video use over the whole immutable puzzle identity.</summary>
    public sealed class LocalHintRecord
    {
        /// <summary>Validates credit accounting and the bounded display deduplication set.</summary>
        public LocalHintRecord(PublicPuzzleIdentity identity, int regularConsumed, int rewardedConsumed, LocalMetagameReward? reward, IReadOnlyList<string> displayedIds)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            LocalModel.Require(regularConsumed >= 0 && regularConsumed <= 3 && rewardedConsumed >= 0 && rewardedConsumed <= 1, "HINT_CREDIT_RANGE");
            RegularConsumed = regularConsumed; RewardedConsumed = rewardedConsumed; Reward = reward;
            DisplayedIds = LocalModel.Copy(displayedIds); LocalModel.Unique(DisplayedIds);
            LocalModel.Require(DisplayedIds.Count == regularConsumed + rewardedConsumed, "HINT_DISPLAY_ACCOUNTING_INVALID");
            foreach (string id in DisplayedIds) LocalModel.Require(Guid.TryParseExact(id, "D", out var parsed) && parsed != Guid.Empty, "HINT_DISPLAY_ID_INVALID");
            if (reward != null) LocalModel.Require(reward.ClaimId == ClaimId(identity), "HINT_REWARD_IDENTITY_MISMATCH");
            LocalModel.Require(rewardedConsumed == 0 || reward?.State == "CONFIRMED", "HINT_REWARD_NOT_CONFIRMED");
        }
        /// <summary>Canonical identity-bound hint-video claim.</summary>
        public static string ClaimId(PublicPuzzleIdentity identity) => "hint-reward:v1:" + identity.PuzzleId + ":" + identity.HashSha256;
        /// <summary>Full semantic puzzle identity.</summary>
        public PublicPuzzleIdentity Identity { get; }
        /// <summary>Regular credits already used.</summary>
        public int RegularConsumed { get; }
        /// <summary>Confirmed video credits already used.</summary>
        public int RewardedConsumed { get; }
        /// <summary>Current optional video operation.</summary>
        public LocalMetagameReward? Reward { get; }
        /// <summary>At most four acknowledged display IDs, never solution data.</summary>
        public IReadOnlyList<string> DisplayedIds { get; }
        /// <summary>Remaining regular or confirmed video credits.</summary>
        public int AvailableCredits => 3 - RegularConsumed + (Reward?.State == "CONFIRMED" ? 1 - RewardedConsumed : 0);
    }

    /// <summary>One local-date daily slot with its immutable roll and optional provider reservation.</summary>
    public sealed class LocalDailyClaim
    {
        /// <summary>Validates date/slot/result/operation and terminal consistency.</summary>
        public LocalDailyClaim(string date, string slot, int roll, LocalMetagameReward? reward, EconomyTerminal? terminal)
        {
            LocalModel.Require(DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _), "DAILY_DATE_INVALID");
            Date = date; Slot = LocalModel.One(slot, "FREE|REWARDED");
            LocalModel.Require(roll >= 0 && roll < 100, "DAILY_ROLL_INVALID"); Roll = roll;
            LocalModel.Require((slot == "REWARDED") == (reward != null), "DAILY_REWARD_BINDING_INVALID");
            Reward = reward; Terminal = terminal;
            if (reward != null) LocalModel.Require(reward.ClaimId == ClaimId, "DAILY_REWARD_BINDING_INVALID");
            if (terminal != null) LocalModel.Require(terminal.ClaimId == ClaimId && terminal.TransactionId == ClaimId && terminal.SubjectId == Date + ":" + Slot && terminal.ReasonCode == "DAILY_PATIENCE" && terminal.Amount == AmountPatience && (reward == null || reward.State == "CONFIRMED"), "DAILY_TERMINAL_INVALID");
        }
        /// <summary>Local device date captured at reservation.</summary>
        public string Date { get; }
        /// <summary>FREE or REWARDED.</summary>
        public string Slot { get; }
        /// <summary>Immutable uniformly selected integer 0–99.</summary>
        public int Roll { get; }
        /// <summary>Authoritative daily-local-v1 outcome: 60/30/10 percent.</summary>
        public int AmountPatience => Roll < 60 ? 10 : Roll < 90 ? 15 : 25;
        /// <summary>Stable local-date deduplication key.</summary>
        public string ClaimId => "daily:v1:" + Date + ":" + Slot;
        /// <summary>Optional rewarded operation.</summary>
        public LocalMetagameReward? Reward { get; }
        /// <summary>Durable grant terminal, or null before grant.</summary>
        public EconomyTerminal? Terminal { get; }
    }

    /// <summary>Bounded immutable save-v3 extensions; legacy placeholder records remain separate reader inputs.</summary>
    public sealed class LocalMetagameState
    {
        /// <summary>Copies sorted hint/daily collections and rejects all duplicates and overflow.</summary>
        public LocalMetagameState(IReadOnlyList<LocalHintRecord> hints, IReadOnlyList<LocalDailyClaim> daily)
        {
            Hints = LocalModel.Copy(hints); Daily = LocalModel.Copy(daily);
            LocalModel.Require(Hints.Count <= 4096 && Daily.Count <= 768 && Daily.Count(c => c.Terminal == null && c.Reward?.State != "CLOSED_NO_REWARD") <= 20, "METAGAME_RECORD_CAPACITY");
            LocalModel.Unique(Hints.Select(h => h.Identity.PuzzleId + ":" + h.Identity.HashSha256)); LocalModel.Unique(Daily.Select(c => c.ClaimId));
            LocalModel.Require(Hints.Select(h => h.Identity.PuzzleId + ":" + h.Identity.HashSha256).SequenceEqual(Hints.Select(h => h.Identity.PuzzleId + ":" + h.Identity.HashSha256).OrderBy(x => x, StringComparer.Ordinal)), "HINT_RECORD_ORDER");
            LocalModel.Require(Daily.Select(c => c.ClaimId).SequenceEqual(Daily.Select(c => c.ClaimId).OrderBy(x => x, StringComparer.Ordinal)), "DAILY_RECORD_ORDER");
            LocalModel.Unique(Hints.Where(h => h.Reward != null).Select(h => h.Reward!.OperationId).Concat(Daily.Where(c => c.Reward != null).Select(c => c.Reward!.OperationId)));
        }
        /// <summary>Empty neutral migration target.</summary>
        public static LocalMetagameState Empty { get; } = new LocalMetagameState(Array.Empty<LocalHintRecord>(), Array.Empty<LocalDailyClaim>());
        /// <summary>Identity-bound credit records.</summary>
        public IReadOnlyList<LocalHintRecord> Hints { get; }
        /// <summary>Bounded local-date claims.</summary>
        public IReadOnlyList<LocalDailyClaim> Daily { get; }
    }
}
