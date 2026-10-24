using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;

namespace STP.Infrastructure.Persistence
{
    public sealed partial class AtomicSaveRepository
    {
        /// <summary>Infrastructure-only original scoped location for the profile diagnostic ring; no IO or path derivation.</summary>
        public string ProfileDirectory=>files.ProfileDirectory;
        private string? boundWriterEpoch;
        private int deletionRetired;
        private sealed class LifecycleView
        {
            internal ProfileLifecycleRecord? Main,Stage,Previous;
            internal ProfileDeletionState State;
            internal string? Epoch;
            internal bool HasStages;
        }
        private LifecycleView ReadLifecycle()
        {
            var view=new LifecycleView();byte[]?[] bytes={files.ReadLifecycle(SaveFileSystem.LifecycleMain),files.ReadLifecycle(SaveFileSystem.LifecycleStage),files.ReadLifecycle(SaveFileSystem.LifecyclePrevious)};
            var records=new ProfileLifecycleRecord?[3];bool invalid=false;
            for(int i=0;i<3;i++)if(bytes[i]!=null)try{records[i]=codec.LifecycleCodec.Decode(bytes[i]!);}catch(ArgumentException){invalid=true;}
            view.Main=records[0];view.Stage=records[1];view.Previous=records[2];view.HasStages=bytes[1]!=null||bytes[2]!=null;
            if(bytes.All(b=>b==null)){view.State=ProfileDeletionState.Ready;view.Epoch=ProfileDeletionObservation.HistoricalEpoch;return view;}
            if(invalid||view.Main==null){view.State=ProfileDeletionState.Invalid;return view;}
            bool stageFits=view.Stage==null||view.Stage.WriterEpoch==view.Main.WriterEpoch;
            bool completing=!view.Main.DeletePending&&view.Previous?.DeletePending==true&&view.Previous.WriterEpoch==view.Main.WriterEpoch&&stageFits;
            if(stageFits&&(view.Main.DeletePending||completing)){view.State=ProfileDeletionState.Pending;view.Epoch=view.Main.WriterEpoch;return view;}
            if(!view.HasStages&&!view.Main.DeletePending){view.State=ProfileDeletionState.Ready;view.Epoch=view.Main.WriterEpoch;return view;}
            view.State=ProfileDeletionState.Invalid;return view;
        }
        private string? CheckWriterEpoch(bool explicitLoad,bool historicalCreate)
        {
            if(Volatile.Read(ref deletionRetired)!=0)return "SAVE_PROFILE_REPOSITORY_RETIRED";
            var lifecycle=ReadLifecycle();
            if(lifecycle.State==ProfileDeletionState.Pending)return "SAVE_PROFILE_DELETE_PENDING";
            if(lifecycle.State!=ProfileDeletionState.Ready)return "SAVE_PROFILE_LIFECYCLE_INVALID";
            if(boundWriterEpoch==null)
            {
                if(!explicitLoad)
                    return historicalCreate&&lifecycle.Epoch==ProfileDeletionObservation.HistoricalEpoch?null:"SAVE_PROFILE_EPOCH_LOAD_REQUIRED";
                boundWriterEpoch=lifecycle.Epoch;
            }
            return boundWriterEpoch==lifecycle.Epoch?null:"SAVE_PROFILE_EPOCH_CONFLICT";
        }
        private ProfileDeletionObservation InspectDeletion(CancellationToken ct)
        {
            var plan=files.PlanDeletion(ct);var lifecycle=ReadLifecycle();string token=files.DeletionObservationToken(plan,ct,lifecycle.State==ProfileDeletionState.Ready);
            return new ProfileDeletionObservation(codec.Profile,codec.Catalog,lifecycle.State,lifecycle.Epoch,token,lifecycle.State==ProfileDeletionState.Invalid?"SAVE_PROFILE_LIFECYCLE_INVALID":null);
        }
        /// <summary>Pure scoped observation, including invalid lifecycle/stage bytes; never binds a cold normal writer.</summary>
        public Task<ProfileDeletionObservation> InspectDeletionAsync(CancellationToken cancellationToken)=>Task.Run(()=>
        {
            try{using(files.AcquireExclusive(cancellationToken))return InspectDeletion(cancellationToken);}
            catch(Exception e)when(KnownFailure(e)||e is OperationCanceledException)
            {return new ProfileDeletionObservation(codec.Profile,codec.Catalog,ProfileDeletionState.Failed,null,new string('0',64),e is OperationCanceledException?"SAVE_CANCELLED":FileError(e));}
        });
        /// <summary>Called only for a concrete explicit confirmation; no effect on disk or normal writer binding.</summary>
        public ProfileDeletionIntent CreateDeletionIntent(ProfileDeletionObservation observation)
        {
            if(observation==null||!observation.Profile.Equals(codec.Profile)||!observation.Catalog.Equals(codec.Catalog))throw new ArgumentException("SAVE_DELETE_PROFILE_MISMATCH");
            return new ProfileDeletionIntent(observation);
        }
        /// <summary>Retires this normal writer permanently; retries retain this exact target epoch.</summary>
        public Task<ProfileDeletionResult> ConfirmDeleteAsync(ProfileDeletionIntent intent,CancellationToken cancellationToken)
        {
            if(intent==null)throw new ArgumentNullException(nameof(intent));
            if(!intent.Observation.Profile.Equals(codec.Profile)||!intent.Observation.Catalog.Equals(codec.Catalog))return Task.FromResult(new ProfileDeletionResult(ProfileDeletionStatus.Conflict,"SAVE_DELETE_PROFILE_MISMATCH",intent.TargetEpoch));
            Interlocked.Exchange(ref deletionRetired,1);return DeleteAsync(intent.Observation,intent.TargetEpoch,false,cancellationToken);
        }
        /// <summary>Only the exact observed valid pending receipt can resume after a process restart.</summary>
        public Task<ProfileDeletionResult> ResumePendingDeletionAsync(ProfileDeletionObservation observation,CancellationToken cancellationToken)
        {
            if(observation==null)throw new ArgumentNullException(nameof(observation));
            if(!observation.Profile.Equals(codec.Profile)||!observation.Catalog.Equals(codec.Catalog)||observation.State!=ProfileDeletionState.Pending||observation.WriterEpoch==null)
                return Task.FromResult(new ProfileDeletionResult(ProfileDeletionStatus.Conflict,"SAVE_DELETE_PENDING_OBSERVATION_REQUIRED",null));
            Interlocked.Exchange(ref deletionRetired,1);return DeleteAsync(observation,observation.WriterEpoch,true,cancellationToken);
        }
        private Task<ProfileDeletionResult> DeleteAsync(ProfileDeletionObservation expected,string target,bool resume,CancellationToken ct)=>Task.Run(()=>
        {
            bool effects=false;
            ProfileDeletionResult Result(ProfileDeletionStatus status,string? error=null)=>new ProfileDeletionResult(status,error,target);
            try
            {
                using(files.AcquireExclusive(ct))
                {
                    ct.ThrowIfCancellationRequested();lease=null;var current=ReadLifecycle();
                    // Completed same-operation ACK never enumerates/deletes newly initialized gameplay data.
                    if(current.Main?.WriterEpoch==target&&!current.Main.DeletePending)
                    {
                        if(current.State!=ProfileDeletionState.Ready&&current.State!=ProfileDeletionState.Pending)return Result(ProfileDeletionStatus.Conflict,"SAVE_DELETE_STAGE_CONFLICT");
                        if(current.HasStages){effects=true;files.ClearLifecycleStages();}
                        var ack=ReadLifecycle();if(ack.State!=ProfileDeletionState.Ready||ack.Epoch!=target)throw new IOException("SAVE_DELETE_ACK_REREAD_FAILED");
                        files.Checkpoint("delete.before-ack");return Result(ProfileDeletionStatus.Confirmed);
                    }
                    bool ownPending=current.Main?.WriterEpoch==target&&current.Main.DeletePending&&current.State==ProfileDeletionState.Pending;
                    if(!ownPending)
                    {
                        if(resume)return Result(ProfileDeletionStatus.Conflict,"SAVE_DELETE_PENDING_CHANGED");
                        var observed=InspectDeletion(ct);
                        if(observed.State!=expected.State||observed.WriterEpoch!=expected.WriterEpoch||observed.ObservationToken!=expected.ObservationToken)return Result(ProfileDeletionStatus.Conflict,"SAVE_DELETE_OBSERVATION_CONFLICT");
                        if(observed.State!=ProfileDeletionState.Ready&&observed.State!=ProfileDeletionState.Invalid)return Result(ProfileDeletionStatus.Conflict,"SAVE_DELETE_CONFIRMATION_OBSERVATION_INVALID");
                        ct.ThrowIfCancellationRequested();effects=true;files.WriteLifecycle(codec.LifecycleCodec.Encode(target,true),"pending");
                    }
                    else
                    {
                        if(resume)
                        {
                            var observed=InspectDeletion(ct);if(observed.ObservationToken!=expected.ObservationToken)return Result(ProfileDeletionStatus.Conflict,"SAVE_DELETE_OBSERVATION_CONFLICT");
                        }
                        effects=true;files.ClearLifecycleStages();
                    }
                    var pending=ReadLifecycle();if(pending.State!=ProfileDeletionState.Pending||pending.Main?.WriterEpoch!=target||!pending.Main.DeletePending||pending.HasStages)throw new IOException("SAVE_DELETE_PENDING_REREAD_FAILED");
                    files.Checkpoint("delete.pending-verified");ct.ThrowIfCancellationRequested();
                    var plan=files.PlanDeletion(ct);files.Checkpoint("delete.plan-verified");files.DeleteProfileData(plan,ct);
                    ct.ThrowIfCancellationRequested();files.WriteLifecycle(codec.LifecycleCodec.Encode(target,false),"complete");
                    var completed=ReadLifecycle();if(completed.State!=ProfileDeletionState.Ready||completed.Epoch!=target||completed.HasStages)throw new IOException("SAVE_DELETE_ACK_REREAD_FAILED");
                    files.Checkpoint("delete.before-ack");ct.ThrowIfCancellationRequested();return Result(ProfileDeletionStatus.Confirmed);
                }
            }
            catch(OperationCanceledException){return Result(effects?ProfileDeletionStatus.Pending:ProfileDeletionStatus.Cancelled,effects?"SAVE_DELETE_CANCELLED_AFTER_POSSIBLE_EFFECT":"SAVE_CANCELLED");}
            catch(Exception e)when(KnownFailure(e)){return Result(effects?ProfileDeletionStatus.Pending:ProfileDeletionStatus.Failed,FileError(e));}
        });
    }
}
