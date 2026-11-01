using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace STP.Application
{
    /// <summary>Existing one-writer repository and CAS coordinate credit/session/daily transactions.</summary>
    public sealed class LocalMetagameTransactions
    {
        private readonly ILocalProfileRepository repository;
        private readonly LocalMetagamePolicy policy;
        private readonly ILocalDailyRandom random;
        /// <summary>No SDK starts here; provider work receives only acknowledged reservations.</summary>
        public LocalMetagameTransactions(ILocalProfileRepository repository, LocalMetagamePolicy policy, ILocalDailyRandom random)
        { this.repository = repository ?? throw new ArgumentNullException(nameof(repository)); this.policy = policy ?? throw new ArgumentNullException(nameof(policy)); this.random = random ?? throw new ArgumentNullException(nameof(random)); }
        private async Task<SaveCommitResult<LocalProfileState>> Execute(long expectedGeneration, Func<LocalProfileState, long, LocalProfileState> mutation, CancellationToken cancellation)
        {
            var loaded = await repository.LoadAsync(cancellation).ConfigureAwait(false);
            if (!loaded.Succeeded || loaded.Snapshot == null) return SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed, "METAGAME_PROFILE_LOAD_REQUIRED");
            var source = loaded.Snapshot;
            if (source.Generation != expectedGeneration) return SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Conflict, "METAGAME_SAVE_GENERATION_CONFLICT");
            LocalModel.Require(expectedGeneration < LocalModel.MaxSafe, "SAVE_GENERATION_EXHAUSTED");
            var result = await Task.Run(() => mutation(source.Payload, expectedGeneration + 1), cancellation).ConfigureAwait(false);
            if (ReferenceEquals(result, source.Payload)) return SaveCommitResult<LocalProfileState>.Success(source);
            return await repository.CommitAsync(new SaveSnapshot<LocalProfileState>(source.Profile, source.Catalog, expectedGeneration + 1, result), expectedGeneration, cancellation).ConfigureAwait(false);
        }
        /// <summary>Atomically records the actual displayed hint and consumes its identity-bound credit.</summary>
        public Task<SaveCommitResult<LocalProfileState>> ConsumeHintAsync(SafeHintDisplayReceipt receipt, long expectedGeneration, CancellationToken cancellation)
            => Execute(expectedGeneration, (state, generation) => policy.ConsumeHintDisplay(state, receipt), cancellation);
        /// <summary>Persists before SDK show; no additional video is allowed while a credit exists.</summary>
        public Task<SaveCommitResult<LocalProfileState>> ReserveHintAsync(PublicPuzzleIdentity identity, string operationId, long expectedGeneration, CancellationToken cancellation)
            => Execute(expectedGeneration, (state, generation) => policy.ReserveHintVideo(state, identity, operationId, generation), cancellation);
        /// <summary>Confirms a callback without spending its new credit.</summary>
        public Task<SaveCommitResult<LocalProfileState>> ConfirmHintAsync(PublicPuzzleIdentity identity, AdRewardEvidence evidence, long expectedGeneration, CancellationToken cancellation)
            => Execute(expectedGeneration, (state, generation) => policy.ConfirmHintVideo(state, identity, evidence), cancellation);
        /// <summary>Holds an uncertain result or records definitive no reward.</summary>
        public Task<SaveCommitResult<LocalProfileState>> ResolveHintAsync(PublicPuzzleIdentity identity, AdRewardReservation reservation, bool definitelyNoReward, long expectedGeneration, CancellationToken cancellation)
            => Execute(expectedGeneration, (state, generation) => policy.ResolveHintVideo(state, identity, reservation, definitelyNoReward), cancellation);
        /// <summary>Captures local date and outcome in the durable reservation; subsequent calls reuse that outcome.</summary>
        public Task<SaveCommitResult<LocalProfileState>> ReserveDailyAsync(string localDate, bool rewarded, string operationId, long expectedGeneration, CancellationToken cancellation)
            => Execute(expectedGeneration, (state, generation) => policy.ReserveDaily(state, localDate, rewarded, operationId, generation, random), cancellation);
        /// <summary>Commits only the previously selected free outcome.</summary>
        public Task<SaveCommitResult<LocalProfileState>> CommitDailyAsync(string localDate, long expectedGeneration, string utc, CancellationToken cancellation)
            => Execute(expectedGeneration, (state, generation) => policy.CommitFreeDaily(state, localDate, generation, utc), cancellation);
        /// <summary>Atomically confirms a matching rewarded daily callback and its fixed ledger grant.</summary>
        public Task<SaveCommitResult<LocalProfileState>> ConfirmDailyAsync(AdRewardEvidence evidence, long expectedGeneration, string utc, CancellationToken cancellation)
            => Execute(expectedGeneration, (state, generation) => policy.ConfirmDailyVideo(state, evidence, generation, utc), cancellation);
        /// <summary>Holds uncertainty without reroll or releases only a definitive no-reward operation.</summary>
        public Task<SaveCommitResult<LocalProfileState>> ResolveDailyAsync(AdRewardReservation reservation, bool definitelyNoReward, long expectedGeneration, CancellationToken cancellation)
            => Execute(expectedGeneration, (state, generation) => policy.ResolveDailyVideo(state, reservation, definitelyNoReward), cancellation);
        /// <summary>Extracts an SDK reservation only from a successfully committed repository result.</summary>
        public static AdRewardReservation RewardReservation(SaveCommitResult<LocalProfileState> committed, string claimId)
        {
            LocalModel.Require(committed.Succeeded && committed.Snapshot != null, "METAGAME_RESERVATION_NOT_COMMITTED");
            var snapshot = committed.Snapshot!; var data = snapshot.Payload.Metagame ?? throw new ArgumentException("METAGAME_RESERVATION_MISSING");
            var rewards = data.Hints.Select(h => h.Reward).Concat(data.Daily.Select(c => c.Reward)).Where(r => r != null && r.ClaimId == claimId).ToArray();
            LocalModel.Require(rewards.Length == 1 && rewards[0]!.State == "RESERVED" && rewards[0]!.ReservationGeneration <= snapshot.Generation, "METAGAME_RESERVATION_MISSING");
            var reward = rewards[0]!; return new AdRewardReservation(reward.ClaimId, reward.OperationId, reward.ReservationGeneration);
        }
    }
}
