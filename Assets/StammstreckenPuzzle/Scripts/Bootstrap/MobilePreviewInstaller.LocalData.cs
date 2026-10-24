using System;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;
using STP.Presentation.UI;
using UnityEngine.SceneManagement;

namespace STP.Bootstrap
{
    public sealed partial class MobilePreviewInstaller
    {
        private ProductionLocalDataState localDataState=ProductionLocalDataState.Unavailable;
        private ProfileDeletionObservation? deletionObservation;
        private ProfileDeletionIntent? deletionIntent;
        private Task localDataOperation=Task.CompletedTask;
        private Task hintWork=Task.CompletedTask;
        private Task? deletionDrain;
        private bool localDataInputFence,localDataDispatching,deletionOwnersClosed;
        private bool localDataStartupBlocked;

        private void BindLocalDataDeletion()
        {
            if(fullGame==null||presenter==null)return;
            presenter.BindLocalDataDeletion(()=>new ProductionLocalDataView(localDataState),RunLocalDataActionAsync);
        }
        private bool LocalDataExternalBusy=>paused||!focused||experience?.IsNativeUiBusy==true;
        private Task RunLocalDataActionAsync(ProductionLocalDataAction action)
        {
            if(disposed||localDataDispatching||!localDataOperation.IsCompleted||fullGame==null||LocalDataExternalBusy)
                return Task.FromException(new InvalidOperationException("LOCAL_DATA_ACTION_UNAVAILABLE"));
            bool allowed=action==ProductionLocalDataAction.Open&&localDataState==ProductionLocalDataState.Ready||
                (action==ProductionLocalDataAction.Confirm||action==ProductionLocalDataAction.Cancel)&&localDataState==ProductionLocalDataState.Confirmation||
                action==ProductionLocalDataAction.Retry&&localDataState==ProductionLocalDataState.RetryRequired||
                action==ProductionLocalDataAction.Restart&&(localDataState==ProductionLocalDataState.Deleted||localDataState==ProductionLocalDataState.RetryRequired);
            if(!allowed)return Task.FromException(new InvalidOperationException("LOCAL_DATA_STATE_CONFLICT"));
            // Snapshot only prior owned work. The current UI/Root deletion task is deliberately absent.
            Task prior=Task.WhenAll(operation,lifecycle,hintWork,experience?.Pending??Task.CompletedTask);
            localDataDispatching=true;
            try{return localDataOperation=RunLocalDataObservedAsync(action,prior);}
            finally{localDataDispatching=false;}
        }
        private async Task RunLocalDataObservedAsync(ProductionLocalDataAction action,Task prior)
        {
            try
            {
                switch(action)
                {
                    case ProductionLocalDataAction.Open:
                        localDataInputFence=true;
                        experience?.SetExternalInputSuspended(true);
                        await SettledAsync(prior);
                        if(disposed)return;
                        if(model?.ConfirmedSnapshot!=null)await model.SetSuspendedAsync(true,lifetime.Token);
                        deletionObservation=await fullGame!.Repository.InspectDeletionAsync(lifetime.Token);
                        if(disposed)return;
                        if(deletionObservation.State==ProfileDeletionState.Invalid||deletionObservation.State==ProfileDeletionState.Pending)
                        {
                            // These proofs include ring bytes. Seal our writer before taking the held proof.
                            await CloseLocalDiagnosticsAsync();
                            deletionObservation=await fullGame.Repository.InspectDeletionAsync(lifetime.Token);
                            if(disposed)return;
                        }
                        if(deletionObservation.State==ProfileDeletionState.Pending)
                        {await ResumeBootDeletionAsync(deletionObservation,Task.CompletedTask);return;}
                        if(deletionObservation.State==ProfileDeletionState.Invalid||deletionObservation.State==ProfileDeletionState.Failed)
                        {
                            localDataStartupBlocked=true;
                            ShowLocalDataUnavailable();
                        }
                        if(deletionObservation.State==ProfileDeletionState.Failed)throw new InvalidOperationException("LOCAL_DATA_OBSERVATION_UNAVAILABLE");
                        localDataState=ProductionLocalDataState.Confirmation;break;
                    case ProductionLocalDataAction.Cancel:
                        deletionObservation=null;localDataInputFence=localDataStartupBlocked;localDataState=ProductionLocalDataState.Ready;hasSuspension=false;break;
                    case ProductionLocalDataAction.Confirm:
                        if(deletionObservation==null||deletionIntent!=null||deletionOwnersClosed)throw new InvalidOperationException("LOCAL_DATA_CONFIRMATION_UNAVAILABLE");
                        deletionIntent=fullGame!.Repository.CreateDeletionIntent(deletionObservation);
                        await ExecuteConfirmedDeletionAsync(prior);break;
                    case ProductionLocalDataAction.Retry:
                        if(deletionIntent!=null)await ExecuteConfirmedDeletionAsync(prior);
                        else if(deletionObservation?.State==ProfileDeletionState.Pending)await ResumeBootDeletionAsync(deletionObservation,prior);
                        else throw new InvalidOperationException("LOCAL_DATA_EXISTING_INTENT_REQUIRED");
                        break;
                    case ProductionLocalDataAction.Restart:
                        RestartLocalDataScene();break;
                    default:throw new InvalidOperationException("LOCAL_DATA_ACTION_INVALID");
                }
            }
            catch
            {
                if(disposed)return;
                if(deletionIntent!=null||deletionOwnersClosed||deletionObservation?.State==ProfileDeletionState.Pending)
                {localDataInputFence=true;localDataState=ProductionLocalDataState.RetryRequired;}
                else{localDataInputFence=localDataStartupBlocked;localDataState=ProductionLocalDataState.Ready;hasSuspension=false;}
                throw;
            }
            finally{if(!disposed)presenter?.Refresh();}
        }
        private async Task<bool> InspectLocalDataBeforeLoadAsync()
        {
            if(fullGame==null)return true;
            localDataInputFence=true;
            var observed=await fullGame.Repository.InspectDeletionAsync(lifetime.Token);
            if(disposed)return false;
            if(observed.State==ProfileDeletionState.Pending)
            {await ResumeBootDeletionAsync(observed,lifecycle);return false;}
            if(observed.State==ProfileDeletionState.Invalid||observed.State==ProfileDeletionState.Failed)
            {
                localDataStartupBlocked=true;
                localDataState=ProductionLocalDataState.Ready;
                ShowLocalDataUnavailable();
                return false;
            }
            localDataStartupBlocked=false;
            localDataInputFence=false;localDataState=ProductionLocalDataState.Ready;
            return true;
        }
        private async Task RetryLocalDataStartupAsync()
        {
            if(!presenter!.HasBoundModel)presenter.ShowStartupLoading();
            try
            {
                if(!await InspectLocalDataBeforeLoadAsync())return;
                if(!presenter.HasBoundModel)presenter.BindModel(model!);
                await model!.InitializeAsync(lifetime.Token);
                if(disposed)return;
                TryStartLocalDiagnostics();
                if(experience!=null)await experience.EnsureReadyAsync();
                fault=false;presenter.ClearRequestFailure();
                ApplySuspension();presenter.Refresh();
            }
            catch(Exception error)
            {
                if(disposed)return;
                RecordLocalDiagnosticException(error);
                localDataStartupBlocked=true;localDataInputFence=true;
                ShowLocalDataUnavailable();
            }
        }
        private void ShowLocalDataUnavailable()
        {
            if(presenter!.HasBoundModel)presenter.ShowRequestFailure(false);
            else presenter.ShowStartupFailure(SavedStartupFailure.LocalStorageUnavailable);
        }
        private async Task ExecuteConfirmedDeletionAsync(Task prior)
        {
            localDataInputFence=true;localDataState=ProductionLocalDataState.Deleting;
            if(!await CloseOwnersForDeletionAsync(prior)){localDataState=ProductionLocalDataState.RetryRequired;return;}
            if(disposed)return;
            var result=await fullGame!.Repository.ConfirmDeleteAsync(deletionIntent!,CancellationToken.None);
            if(!disposed)localDataState=result.Succeeded?ProductionLocalDataState.Deleted:ProductionLocalDataState.RetryRequired;
        }
        private async Task ResumeBootDeletionAsync(ProfileDeletionObservation observed,Task prior)
        {
            deletionObservation=observed;localDataInputFence=true;localDataState=ProductionLocalDataState.Deleting;
            if(!await CloseOwnersForDeletionAsync(prior)){localDataState=ProductionLocalDataState.RetryRequired;return;}
            if(disposed)return;
            var result=await fullGame!.Repository.ResumePendingDeletionAsync(observed,CancellationToken.None);
            if(!disposed)localDataState=result.Succeeded?ProductionLocalDataState.Deleted:ProductionLocalDataState.RetryRequired;
        }
        private async Task<bool> CloseOwnersForDeletionAsync(Task prior)
        {
            if(!deletionOwnersClosed)
            {
                deletionOwnersClosed=true;lifetime.Cancel();
                experience?.Dispose();model?.Dispose();DisposeRide();
                Task lateServices=experience?.Pending??Task.CompletedTask;
                Task testDrain=Task.CompletedTask;
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
                testDrain=testLocalDataDrain??Task.CompletedTask;
#endif
                deletionDrain=Task.WhenAll(DrainClosedExperienceAsync(Task.WhenAll(prior,lateServices)),CloseLocalDiagnosticsAsync(),testDrain);
            }
            // A timeout is only UI uncertainty. It never authorizes deleting alongside a live writer.
            Task drain=deletionDrain??Task.CompletedTask;
            if(await Task.WhenAny(drain,Task.Delay(5000))!=drain)return false;
            await drain;return true;
        }
        private void RestartLocalDataScene()
        {
            if(disposed||LocalDataExternalBusy)throw new InvalidOperationException("LOCAL_DATA_RESTART_UNAVAILABLE");
            int index=gameObject.scene.buildIndex;
            if(index<0||index>=SceneManager.sceneCountInBuildSettings)throw new InvalidOperationException("LOCAL_DATA_RESTART_SCENE_UNAVAILABLE");
            _=SceneManager.LoadSceneAsync(index,LoadSceneMode.Single)??throw new InvalidOperationException("LOCAL_DATA_RESTART_FAILED");
        }
        private static async Task SettledAsync(Task work)
        {try{await work;}catch(Exception){/* Completion, including failure, is observed; no rollback is inferred. */}}
        private async Task DrainClosedExperienceAsync(Task prior)
        {
            await SettledAsync(prior);
            // A finishing provider callback can enqueue its own acknowledgement/notice.
            // Re-read after its parent has settled; the disposed service cannot accept new actions.
            while(experience!=null)
            {
                await SettledAsync(experience.Pending);
                if(experience.Pending.IsCompleted)return;
            }
        }
        private Task<T> TrackHintWork<T>(Task<T> task)
        {hintWork=Task.WhenAll(hintWork,task);return task;}
        private Task TrackHintWork(Task task)
        {hintWork=Task.WhenAll(hintWork,task);return task;}
        private async Task CommitRootHintAsync(SafeHintDisplayReceipt receipt)
        {
            var result=await model!.CommitHintDisplayAsync(fullGame!.Metagame,receipt,lifetime.Token);
            if(!result.Succeeded)throw new System.IO.IOException("HINT_DISPLAY_NOT_COMMITTED");
        }
    }
}
