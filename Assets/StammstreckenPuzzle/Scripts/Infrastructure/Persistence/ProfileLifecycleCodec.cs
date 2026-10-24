using System;
using System.Text;
using STP.Application;

namespace STP.Infrastructure.Persistence
{
    /// <summary>Closed technical lifecycle v1, independent of all saved gameplay bytes and hashes.</summary>
    public sealed class ProfileLifecycleCodec
    {
        private readonly ICanonicalJson json;
        /// <summary>Uses the existing strict canonical JSON authority.</summary>
        public ProfileLifecycleCodec(ICanonicalJson json){this.json=json??throw new ArgumentNullException(nameof(json));}
        /// <summary>Canonical closed record; epoch is a nonempty lowercase GUID-N.</summary>
        public byte[] Encode(string writerEpoch,bool deletePending)
        {ProfileDeletionObservation.Epoch(writerEpoch);return LocalSaveHash.Canonical(json,"{\"profileLifecycleVersion\":1,\"writerEpoch\":\""+writerEpoch+"\",\"deletePending\":"+(deletePending?"true":"false")+"}");}
        /// <summary>Strictly reads v1; missing/extra/duplicate/unsupported/partial values are errors.</summary>
        public ProfileLifecycleRecord Decode(byte[] bytes)
        {
            if(bytes==null||bytes.Length==0||bytes.Length>1024)throw new ArgumentException("SAVE_LIFECYCLE_INVALID");
            try
            {
                var c=json.Canonicalize(bytes,new JsonResourceLimits(1024,4,128,16,8));if(!c.Succeeded)throw new ArgumentException("SAVE_LIFECYCLE_INVALID");
                var f=CanonicalDtoCursor.FromCanonical(c.GetUtf8()!).Fields("profileLifecycleVersion","writerEpoch","deletePending");
                if(f[0].Integer()!=1)throw new ArgumentException("SAVE_LIFECYCLE_VERSION_UNSUPPORTED");
                return new ProfileLifecycleRecord(ProfileDeletionObservation.Epoch(f[1].String()),f[2].Boolean());
            }
            catch(SaveCodecFailure){throw new ArgumentException("SAVE_LIFECYCLE_INVALID");}
        }
    }
    /// <summary>Immutable infrastructure record, never serialized into gameplay save.</summary>
    public sealed class ProfileLifecycleRecord
    {
        internal ProfileLifecycleRecord(string epoch,bool pending){WriterEpoch=epoch;DeletePending=pending;}
        /// <summary>Locally generated writer boundary.</summary>
        public string WriterEpoch{get;}
        /// <summary>Requires finishing already confirmed deletion before any profile usage.</summary>
        public bool DeletePending{get;}
    }
    public sealed partial class LocalSaveCodec
    {
        internal ProfileLifecycleCodec LifecycleCodec=>new ProfileLifecycleCodec(json);
    }
}
