using System;
using System.Threading;
using System.Threading.Tasks;

namespace STP.Application
{
    /// <summary>Observed local writer boundary; never gameplay or deletion consent.</summary>
    public enum ProfileDeletionState { Ready, Pending, Invalid, Failed }
    /// <summary>Explicit deletion outcome; Pending never authorizes a fresh profile.</summary>
    public enum ProfileDeletionStatus { Confirmed, Cancelled, Pending, Conflict, Failed }
    /// <summary>Immutable exact profile observation. Invalid/Failed never invent a writer epoch.</summary>
    public sealed class ProfileDeletionObservation
    {
        /// <summary>The historical epoch is a transient sentinel, never a persisted token.</summary>
        public const string HistoricalEpoch="historical";
        /// <summary>Creates facts supplied by the physical adapter; this is not user confirmation.</summary>
        public ProfileDeletionObservation(SaveProfileIdentity profile,CatalogIdentity catalog,ProfileDeletionState state,string? writerEpoch,string observationToken,string? errorCode=null)
        {
            Profile=profile??throw new ArgumentNullException(nameof(profile));Catalog=catalog??throw new ArgumentNullException(nameof(catalog));
            if(profile.Use!=catalog.Use||!Enum.IsDefined(typeof(ProfileDeletionState),state))throw new ArgumentException("SAVE_DELETE_BINDING_INVALID");
            if(state==ProfileDeletionState.Ready||state==ProfileDeletionState.Pending){if(writerEpoch!=HistoricalEpoch)Epoch(writerEpoch!);if(state==ProfileDeletionState.Pending&&writerEpoch==HistoricalEpoch)throw new ArgumentException("SAVE_DELETE_EPOCH_INVALID");}
            else if(writerEpoch!=null)throw new ArgumentException("SAVE_DELETE_INVALID_HAS_EPOCH");
            ObservationToken=ContractGuard.Hash(observationToken,nameof(observationToken));State=state;WriterEpoch=writerEpoch;ErrorCode=errorCode;
        }
        /// <summary>Exact target profile.</summary>
        public SaveProfileIdentity Profile{get;}
        /// <summary>Exact target catalog including consumer isolation.</summary>
        public CatalogIdentity Catalog{get;}
        /// <summary>Whether normal use, pending continuation or fresh explicit repair is possible.</summary>
        public ProfileDeletionState State{get;}
        /// <summary>Null on invalid or unreadable metadata.</summary>
        public string? WriterEpoch{get;}
        /// <summary>Opaque CAS proof of actual lifecycle/stage bytes; invalid/pending observations additionally bind exact scoped files.</summary>
        public string ObservationToken{get;}
        /// <summary>Stable code without paths or lifecycle bytes.</summary>
        public string? ErrorCode{get;}
        /// <summary>Validates the exact lowercase, nonempty GUID-N epoch grammar.</summary>
        public static string Epoch(string value)
        {
            if(value==null||value.Length!=32||!Guid.TryParseExact(value,"N",out var g)||g==Guid.Empty)throw new ArgumentException("SAVE_DELETE_EPOCH_INVALID");
            foreach(char c in value)if(!(c>='0'&&c<='9'||c>='a'&&c<='f'))throw new ArgumentException("SAVE_DELETE_EPOCH_INVALID");return value;
        }
    }
    /// <summary>One volatile explicit confirmation. Keep this exact object across uncertain ACK/retry.</summary>
    public sealed class ProfileDeletionIntent
    {
        /// <summary>Creates exactly one target epoch for this confirmation, never on dialog opening.</summary>
        public ProfileDeletionIntent(ProfileDeletionObservation observation)
        {
            Observation=observation??throw new ArgumentNullException(nameof(observation));
            if(observation.State==ProfileDeletionState.Failed||observation.State==ProfileDeletionState.Pending)throw new ArgumentException("SAVE_DELETE_CONFIRMATION_OBSERVATION_INVALID");
            TargetEpoch=Guid.NewGuid().ToString("N");
        }
        /// <summary>Exact original profile/catalog/epoch and original invalid-stage proof when applicable.</summary>
        public ProfileDeletionObservation Observation{get;}
        /// <summary>Unchanged receipt for retries; never logged, exported or sent to a provider.</summary>
        public string TargetEpoch{get;}
        /// <summary>Observed prior epoch; null only for explicitly observed invalid metadata.</summary>
        public string? ExpectedEpoch=>Observation.WriterEpoch;
    }
    /// <summary>Deletion result. Only Confirmed permits a new repository/load and explicit fresh profile.</summary>
    public sealed class ProfileDeletionResult
    {
        /// <summary>Creates an immutable local status; no file or user data is included.</summary>
        public ProfileDeletionResult(ProfileDeletionStatus status,string? errorCode,string? targetEpoch)
        {if(!Enum.IsDefined(typeof(ProfileDeletionStatus),status))throw new ArgumentOutOfRangeException(nameof(status));if(status==ProfileDeletionStatus.Confirmed&&targetEpoch==null)throw new ArgumentException("SAVE_DELETE_RECEIPT_REQUIRED");if(targetEpoch!=null)ProfileDeletionObservation.Epoch(targetEpoch);Status=status;ErrorCode=errorCode;TargetEpoch=targetEpoch;}
        /// <summary>Closed completion/uncertainty state.</summary>
        public ProfileDeletionStatus Status{get;}
        /// <summary>Only a fully reread confirmed state is successful.</summary>
        public bool Succeeded=>Status==ProfileDeletionStatus.Confirmed;
        /// <summary>Stable redacted error code.</summary>
        public string? ErrorCode{get;}
        /// <summary>Local idempotent receipt; never gameplay truth.</summary>
        public string? TargetEpoch{get;}
    }
    /// <summary>Root quiesces session/autosave/providers and drains the old diagnostic ring before Confirm.</summary>
    public interface ILocalProfileDeletionRepository
    {
        /// <summary>Read only; never binds an unbound normal repository writer.</summary>
        Task<ProfileDeletionObservation> InspectDeletionAsync(CancellationToken cancellationToken);
        /// <summary>Call exclusively upon actual user confirmation, after observing this exact profile.</summary>
        ProfileDeletionIntent CreateDeletionIntent(ProfileDeletionObservation observation);
        /// <summary>Retires this repository permanently and executes/retries only the same confirmation.</summary>
        Task<ProfileDeletionResult> ConfirmDeleteAsync(ProfileDeletionIntent intent,CancellationToken cancellationToken);
        /// <summary>After restart, completes only an exact valid already-confirmed pending marker.</summary>
        Task<ProfileDeletionResult> ResumePendingDeletionAsync(ProfileDeletionObservation observation,CancellationToken cancellationToken);
    }
}
