using System.Threading;
using System.Threading.Tasks;

namespace STP.Application
{
    public sealed partial class PersistedPuzzleSession
    {
        public Task<SaveCommitResult<LocalProfileState>> ReservePostClearAsync(LocalPostClearPolicy postClear,PublicPuzzleIdentity identity,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>postClear.Reserve(s,identity,g),ct);
        public Task<SaveCommitResult<LocalProfileState>> ConfirmPostClearAsync(LocalPostClearPolicy postClear,AdRewardEvidence evidence,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>postClear.Confirm(s,evidence,g,utc),ct);
        public Task<SaveCommitResult<LocalProfileState>> ResolvePostClearAsync(LocalPostClearPolicy postClear,AdRewardReservation reservation,bool definitelyNoReward,CancellationToken ct)
            =>ProfileMutationAsync((s,g,utc)=>postClear.Resolve(s,reservation,definitelyNoReward),ct);
    }
}
