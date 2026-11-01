using System;
using System.Threading;
using System.Threading.Tasks;

namespace STP.Application
{
    public sealed partial class SavedPublic240ReviewSession
    {
        /// <summary>The confirmed whole-profile read model; absent while authority is uncertain.</summary>
        public SaveSnapshot<LocalProfileState>? ConfirmedSnapshot
        { get {RequireOwner();return !disposed&&authority&&!requestFault?persisted.Snapshot:null;} }
        /// <summary>Allows a deliberate internal profile action while a product modal pauses the puzzle; uncertain writes, active writers and unfinished result presentation still block it.</summary>
        public bool CanRunProfileAction
        {get{RequireOwner();return SupportsMetagame&&!disposed&&!busy&&authority&&!requestFault&&!writeFailure&&status!=SavedReviewStatus.TrainRide&&(!Solved||status==SavedReviewStatus.Results);}}

        /// <summary>Serializes one composed metagame action with puzzle flushes, command gates and the displayed snapshot.</summary>
        public async Task<SaveCommitResult<LocalProfileState>> RunProfileActionAsync(Func<PersistedPuzzleSession,CancellationToken,Task<SaveCommitResult<LocalProfileState>>> action,CancellationToken ct)
        {
            RequireOwner();if(disposed)throw new ObjectDisposedException(nameof(SavedPublic240ReviewSession));if(action==null)throw new ArgumentNullException(nameof(action));
            using(var linked=CancellationTokenSource.CreateLinkedTokenSource(ct,lifetime.Token))
            {
                TaskCompletionSource<bool>? idle=null;
                while(idle==null)
                {
                    Task? pending=null;
                    await OnOwner(()=>
                    {
                        if(!SupportsMetagame||!authority||requestFault)throw new ArgumentException("METAGAME_CANDIDATE_NOT_READY");
                        if(busy){pending=requestIdle!.Task;return;}
                        busy=true;exclusive=true;error="NONE";idle=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);requestIdle=idle;
                        if(view!=null&&persisted.CanMutate&&!view.IsPaused)view.Lifecycle(true,clock.MonotonicMilliseconds);
                        status=SavedReviewStatus.Saving;Publish();
                    }).ConfigureAwait(false);
                    if(idle==null)linked.Token.ThrowIfCancellationRequested();
                    if(pending!=null)
                    {
                        var cancelled=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                        using(linked.Token.Register(()=>cancelled.TrySetCanceled()))
                        {var winner=await Task.WhenAny(pending,cancelled.Task).ConfigureAwait(false);await winner.ConfigureAwait(false);}
                    }
                }
                try
                {
                    var flushed=await autosave.FlushAsync(linked.Token).ConfigureAwait(false);
                    if(flushed!=null)
                    {
                        await OnOwner(()=>ApplyCommitted(flushed)).ConfigureAwait(false);
                        if(!flushed.Succeeded)return flushed;
                    }
                    var result=await action(persisted,linked.Token).ConfigureAwait(false);
                    await OnOwner(()=>
                    {
                        if(result.Succeeded&&(result.Snapshot==null||!ReferenceEquals(result.Snapshot,persisted.Snapshot)))
                        {authority=false;requestFault=true;status=SavedReviewStatus.RecoveryRequired;error="METAGAME_COMMIT_BINDING_UNCONFIRMED";}
                        else ApplyCommitted(result);
                        SyncView();
                    }).ConfigureAwait(false);
                    return result;
                }
                catch(ArgumentException)
                {
                    await OnOwner(()=>{error="METAGAME_ACTION_REJECTED";status=SettledStatus();}).ConfigureAwait(false);
                    return SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed,"METAGAME_ACTION_REJECTED");
                }
                catch
                {
                    await OnOwner(()=>{authority=false;requestFault=true;error="METAGAME_ACTION_OUTCOME_UNKNOWN";status=SavedReviewStatus.RecoveryRequired;}).ConfigureAwait(false);throw;
                }
                finally
                {
                    try
                    {
                        await OnOwner(()=>
                        {
                            busy=false;exclusive=false;SyncView();
                            if(authority&&!writeFailure&&!requestFault&&!suspended&&!presentationPaused&&view?.IsPaused==true&&persisted.CanMutate)view.Lifecycle(false,clock.MonotonicMilliseconds);
                            Publish();
                        }).ConfigureAwait(false);
                    }
                    finally {idle.TrySetResult(true);}
                }
            }
        }
    }
}
