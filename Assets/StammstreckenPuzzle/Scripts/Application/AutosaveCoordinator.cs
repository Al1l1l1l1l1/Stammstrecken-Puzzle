using System;
using System.Threading;
using System.Threading.Tasks;

namespace STP.Application
{
    /// <summary>Caller-driven autosave scheduling with one serial use-case writer and explicit process-local monotonic time.</summary>
    public sealed class AutosaveCoordinator
    {
        private readonly PersistedPuzzleSession session;private readonly object gate=new object();private long version;private long savedVersion;private long firstDirty;private long lastDirty;private long? nextFirstDirty;private bool writing;private long lastNow;private TaskCompletionSource<bool>? idle;
        /// <summary>Binds the persistence use case; TickAsync is called by the host application loop.</summary>
        public AutosaveCoordinator(PersistedPuzzleSession session){this.session=session??throw new ArgumentNullException(nameof(session));}
        /// <summary>Whether any notified mutation remains unacknowledged.</summary>
        public bool IsDirty {get{lock(gate)return version!=savedVersion;}}
        /// <summary>Records a state-changing domain/preferences mutation; selection and no-op commands must not call this method.</summary>
        public void NotifyMutation(long monotonicMilliseconds)
        {lock(gate){Time(monotonicMilliseconds);if(version==long.MaxValue)throw new InvalidOperationException("AUTOSAVE_REVISION_EXHAUSTED");if(version==savedVersion)firstDirty=monotonicMilliseconds;if(writing&&!nextFirstDirty.HasValue)nextFirstDirty=monotonicMilliseconds;lastDirty=monotonicMilliseconds;version++;}}
        private void Time(long now){if(now<0||now<lastNow)throw new ArgumentException("AUTOSAVE_CLOCK_INVALID");lastNow=now;}
        /// <summary>Writes after 500ms without a mutation or 2000ms since the earliest unpersisted mutation; returns null if not due or already writing.</summary>
        public Task<SaveCommitResult<LocalProfileState>?> TickAsync(long monotonicMilliseconds,CancellationToken cancellationToken)
        {lock(gate){Time(monotonicMilliseconds);if(writing||version==savedVersion||monotonicMilliseconds-lastDirty<500&&monotonicMilliseconds-firstDirty<2000)return Task.FromResult<SaveCommitResult<LocalProfileState>?>(null);writing=true;idle=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);nextFirstDirty=null;return WriteAsync(version,cancellationToken,false);}}
        private async Task<SaveCommitResult<LocalProfileState>?> WriteAsync(long captured,CancellationToken ct,bool completion)
        {
            SaveCommitResult<LocalProfileState>? result=null;try{result=completion?await session.CommitCompletionAsync(ct).ConfigureAwait(false):await session.SaveDraftAsync(ct).ConfigureAwait(false);return result;}
            finally{lock(gate){if(result?.Succeeded==true){savedVersion=captured;if(version!=captured&&nextFirstDirty.HasValue)firstDirty=nextFirstDirty.Value;}writing=false;nextFirstDirty=null;idle?.TrySetResult(true);}}
        }
        /// <summary>Immediately requests a lifecycle checkpoint; host lifecycle pause must first be applied to the domain at its real monotonic instant.</summary>
        public async Task<SaveCommitResult<LocalProfileState>?> FlushAsync(CancellationToken cancellationToken)
        {
            while(true){Task<SaveCommitResult<LocalProfileState>?>? task=null;Task? wait=null;lock(gate){if(!writing){if(version==savedVersion)return null;writing=true;idle=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);nextFirstDirty=null;task=WriteAsync(version,cancellationToken,false);}else wait=idle!.Task;}if(task!=null)return await task.ConfigureAwait(false);await WaitIdle(wait!,cancellationToken).ConfigureAwait(false);}
        }
        /// <summary>Queues completion after earlier draft writes; a draft response can never release results or overwrite the later completion.</summary>
        public async Task<SaveCommitResult<LocalProfileState>?> CommitCompletionAsync(CancellationToken cancellationToken)
        {
            while(true){Task<SaveCommitResult<LocalProfileState>?>? task=null;Task? wait=null;lock(gate){if(!writing){writing=true;idle=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);nextFirstDirty=null;task=WriteAsync(version,cancellationToken,true);}else wait=idle!.Task;}if(task!=null)return await task.ConfigureAwait(false);await WaitIdle(wait!,cancellationToken).ConfigureAwait(false);}
        }
        private static async Task WaitIdle(Task pending,CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();if(!ct.CanBeCanceled){await pending.ConfigureAwait(false);return;}
            var cancelled=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using(ct.Register(()=>cancelled.TrySetResult(true))){if(await Task.WhenAny(pending,cancelled.Task).ConfigureAwait(false)!=pending)ct.ThrowIfCancellationRequested();await pending.ConfigureAwait(false);}
        }
    }
}
