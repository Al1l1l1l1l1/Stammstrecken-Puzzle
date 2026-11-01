using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace STP.Application
{
    /// <summary>One raw-locked ADR045 manifest. Original bytes are copied; construction grants no human or product approval.</summary>
    public sealed class HumanRoutingManifest
    {
        private readonly byte[] original;
        public HumanRoutingManifest(byte[] originalBytes, IHumanAssignmentCatalog catalog)
        {
            if (originalBytes == null) throw new ArgumentNullException(nameof(originalBytes));
            Catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            HumanGuard.Require(originalBytes.Length > 0 && originalBytes.Length <= 8388608, "HUMAN_ROUTING_MANIFEST_SIZE");
            original = (byte[])originalBytes.Clone();
            HumanGuard.Require(LocalSaveHash.Bytes(original) == Catalog.ManifestBytesSha256, "HUMAN_ROUTING_RAW_HASH_MISMATCH");
        }
        public IHumanAssignmentCatalog Catalog { get; }
        public byte[] GetOriginalBytes() => (byte[])original.Clone();
    }

    /// <summary>Root supplies exactly the 17 raw/UI/Public-validated packs. Resolving is read-only and cannot manufacture a replacement source.</summary>
    public interface IHumanManifestResolver
    {
        IReadOnlyList<string> ManifestBytesSha256 { get; }
        HumanRoutingManifest Resolve(string manifestBytesSha256);
    }

    /// <summary>The closed nonpersistent ADR045 CAS union. Values are expectations, never an attestation or independent write authority.</summary>
    public sealed class HumanRoutingAuthority
    {
        private HumanRoutingAuthority(string observation, string? manifest, string? token, long? generation, string? record)
        {
            GlobalObservationToken = LocalModel.Hash(observation);
            ManifestBytesSha256 = manifest;
            SessionToken = token;
            Generation = generation;
            RecordBytesSha256 = record;
        }
        public bool IsMissing => ManifestBytesSha256 == null;
        public string GlobalObservationToken { get; }
        public string? ManifestBytesSha256 { get; }
        public string? SessionToken { get; }
        public long? Generation { get; }
        public string? RecordBytesSha256 { get; }
        public static HumanRoutingAuthority Missing(string globalObservationToken) => new HumanRoutingAuthority(globalObservationToken, null, null, null, null);
        public static HumanRoutingAuthority Confirmed(string manifestBytesSha256, string sessionToken, long generation, string recordBytesSha256, string globalObservationToken)
        {
            HumanGuard.Require(Guid.TryParseExact(sessionToken, "N", out var token) && token != Guid.Empty, "HUMAN_RUN_TOKEN_INVALID");
            LocalModel.Positive(generation);
            return new HumanRoutingAuthority(globalObservationToken, LocalModel.Hash(manifestBytesSha256), sessionToken, generation, LocalModel.Hash(recordBytesSha256));
        }
        public bool Matches(HumanRoutingAuthority other) => other != null && IsMissing == other.IsMissing &&
            GlobalObservationToken == other.GlobalObservationToken && ManifestBytesSha256 == other.ManifestBytesSha256 &&
            SessionToken == other.SessionToken && Generation == other.Generation && RecordBytesSha256 == other.RecordBytesSha256;
    }

    /// <summary>Discovery does not reveal a puzzle or permit a start in recovery. Root binds gameplay only after a confirmed catalog/snapshot decision.</summary>
    public sealed class HumanRoutingInspection
    {
        public HumanRoutingInspection(HumanStoreStatus status, string errorCode, string globalObservationToken,
            IHumanAssignmentCatalog? catalog = null, HumanObservationSnapshot? snapshot = null, HumanRoutingAuthority? authority = null)
        {
            Status = HumanGuard.Enum(status); ErrorCode = LocalModel.Text(errorCode);
            GlobalObservationToken = LocalModel.Hash(globalObservationToken); Catalog = catalog; Snapshot = snapshot; Authority = authority;
            bool ready = status == HumanStoreStatus.Ready || status == HumanStoreStatus.Committed;
            HumanGuard.Require(!ready || catalog != null && snapshot != null && authority != null && !authority.IsMissing &&
                authority.ManifestBytesSha256 == catalog.ManifestBytesSha256 && snapshot.Run.ManifestBytesSha256 == catalog.ManifestBytesSha256 &&
                authority.SessionToken == snapshot.Run.SessionToken && authority.Generation == snapshot.Generation, "HUMAN_ROUTING_INSPECTION_BINDING");
            HumanGuard.Require(status != HumanStoreStatus.Missing || catalog == null && snapshot == null && authority?.IsMissing == true, "HUMAN_ROUTING_MISSING_BINDING");
            HumanGuard.Require(ready || status == HumanStoreStatus.Missing || authority == null, "HUMAN_ROUTING_UNCONFIRMED_AUTHORITY");
            HumanGuard.Require(authority == null || authority.GlobalObservationToken == globalObservationToken, "HUMAN_ROUTING_OBSERVATION_BINDING");
        }
        public HumanStoreStatus Status { get; }
        public string ErrorCode { get; }
        public string GlobalObservationToken { get; }
        public IHumanAssignmentCatalog? Catalog { get; }
        public HumanObservationSnapshot? Snapshot { get; }
        public HumanRoutingAuthority? Authority { get; }
        public bool IsConfirmed => Status == HumanStoreStatus.Ready || Status == HumanStoreStatus.Committed;
        public bool CanStartParticipant => Status == HumanStoreStatus.Missing || IsConfirmed && Snapshot!.Run.NextAssignmentIndex == Catalog!.Assignments.Count;
    }

    public interface IHumanRoutingRepository
    {
        Task<HumanRoutingInspection> InspectAsync(CancellationToken cancellation);
        IHumanCalibrationRepository OpenConfirmedRepository(HumanRoutingAuthority confirmedAuthority);
        Task<HumanRoutingInspection> StartNextParticipantAsync(HumanRoutingAuthority expectedAuthority, string targetManifestBytesSha256,
            HumanRunOrigin origin, string createdAtUtc, CancellationToken cancellation);
        Task<HumanRoutingInspection> RecoverRoutingAsync(string globalObservationToken, CancellationToken cancellation);
    }
}
