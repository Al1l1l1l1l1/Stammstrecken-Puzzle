using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;

namespace STP.Infrastructure.Persistence
{
    /// <summary>One profile-bound worker repository with complete three-source provenance and explicit recovery.</summary>
    public sealed class AtomicSaveRepository : ILocalProfileRepository
    {
        private const long MaxSafe = 9007199254740991L;
        private readonly SaveFileSystem files;
        private readonly LocalSaveCodec codec;
        private readonly ILocalSessionClock clock;
        private readonly string appVersion;
        private readonly string buildId;
        private RecoveryLease? lease;
        /// <summary>Binds immutable codec/profile/catalog configuration and physical worker-only I/O.</summary>
        public AtomicSaveRepository(SaveFileSystem files, LocalSaveCodec codec, ILocalSessionClock clock, string appVersion, string buildId)
        {
            this.files = files ?? throw new ArgumentNullException(nameof(files));
            this.codec = codec ?? throw new ArgumentNullException(nameof(codec));
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
            if (!files.Profile.Equals(codec.Profile) || !files.Catalog.Equals(codec.Catalog)) throw new ArgumentException("SAVE_PROFILE_BINDING_MISMATCH");
            if (string.IsNullOrWhiteSpace(appVersion) || string.IsNullOrWhiteSpace(buildId)) throw new ArgumentException("SAVE_BUILD_DIAGNOSIS_REQUIRED");
            this.appVersion = appVersion; this.buildId = buildId;
        }
        private sealed class Candidate
        {
            internal Candidate(SaveCandidateProvenance provenance, byte[]? bytes, LocalSaveDecodeResult? decoded, string? error)
            { Provenance = provenance; Bytes = bytes; Decoded = decoded; Error = error; }
            internal SaveCandidateProvenance Provenance { get; }
            internal byte[]? Bytes { get; }
            internal LocalSaveDecodeResult? Decoded { get; }
            internal string? Error { get; }
        }
        private sealed class Observed
        {
            internal Observed(SaveRepositoryObservation observation, Candidate[] candidates)
            { Observation = observation; Candidates = candidates; }
            internal SaveRepositoryObservation Observation { get; }
            internal Candidate[] Candidates { get; }
            internal Candidate? Highest => Candidates.Where(x => x.Provenance.Status == SaveCandidateStatus.Valid)
                .OrderByDescending(x => x.Provenance.Generation).ThenBy(x => x.Provenance.Kind).FirstOrDefault();
            internal long HighestGeneration => Highest?.Provenance.Generation ?? 0;
            internal bool AllMissing => Candidates.All(x => x.Provenance.Status == SaveCandidateStatus.Missing);
            internal bool Has(SaveCandidateStatus status) => Candidates.Any(x => x.Provenance.Status == status);
            internal bool SplitBrain => Highest != null && Candidates.Any(x => x.Provenance.Status == SaveCandidateStatus.Valid &&
                x.Provenance.Generation == HighestGeneration && x.Provenance.PayloadSha256 != Highest.Provenance.PayloadSha256);
        }
        private sealed class RecoveryLease
        {
            internal RecoveryLease(SaveRepositoryObservation observation, string targetPayloadHash, long generation)
            { Observation = observation; TargetPayloadHash = targetPayloadHash; Generation = generation; }
            internal SaveRepositoryObservation Observation { get; }
            internal string TargetPayloadHash { get; }
            internal long Generation { get; }
        }
        private Observed Observe(CancellationToken ct)
        {
            var candidates = new Candidate[3];
            foreach (SaveCandidateKind kind in Enum.GetValues(typeof(SaveCandidateKind)))
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    byte[]? bytes = files.ReadCandidate(kind, LocalSaveHash.Limits.MaxBytes);
                    if (bytes == null) candidates[(int)kind] = new Candidate(new SaveCandidateProvenance(kind, SaveCandidateStatus.Missing, null, null, null), null, null, null);
                    else
                    {
                        var result = codec.Decode(bytes);
                        var status = result.Status == LocalSaveDecodeStatus.Valid ? SaveCandidateStatus.Valid :
                            result.Status == LocalSaveDecodeStatus.Unsupported ? SaveCandidateStatus.Unsupported : SaveCandidateStatus.Corrupt;
                        candidates[(int)kind] = new Candidate(new SaveCandidateProvenance(kind, status, result.Generation, result.SourcePayloadSha256,
                            LocalSaveHash.Bytes(bytes)), bytes, result, result.ErrorCode);
                    }
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
                {
                    candidates[(int)kind] = new Candidate(new SaveCandidateProvenance(kind, SaveCandidateStatus.Failed, null, null, null), null, null, FileError(e));
                }
            }
            ct.ThrowIfCancellationRequested();
            return new Observed(new SaveRepositoryObservation(codec.Profile, codec.Catalog, candidates.Select(x => x.Provenance).ToArray()), candidates);
        }
        private SaveLoadResult<LocalProfileState> LoadObserved(Observed observed)
        {
            if (observed.Has(SaveCandidateStatus.Failed)) return SaveLoadResult<LocalProfileState>.Failure(SaveLoadStatus.Failed,
                observed.Candidates.First(x => x.Provenance.Status == SaveCandidateStatus.Failed).Error ?? "SAVE_READ_FAILED", observed.Observation);
            if (observed.Has(SaveCandidateStatus.Unsupported)) return SaveLoadResult<LocalProfileState>.Failure(SaveLoadStatus.Unsupported,
                observed.Candidates.First(x => x.Provenance.Status == SaveCandidateStatus.Unsupported).Error ?? "SAVE_SOURCE_UNSUPPORTED", observed.Observation);
            if (observed.AllMissing) return SaveLoadResult<LocalProfileState>.NeedsInitialization();
            if (observed.Highest == null) return SaveLoadResult<LocalProfileState>.Failure(SaveLoadStatus.Corrupt, "SAVE_NO_VALID_CANDIDATE", observed.Observation);
            if (observed.SplitBrain)
            {
                var main = observed.Candidates[(int)SaveCandidateKind.Main];
                if (main.Provenance.Status == SaveCandidateStatus.Valid && main.Provenance.Generation == observed.HighestGeneration)
                    return SaveLoadResult<LocalProfileState>.SplitBrain(main.Decoded!.Snapshot!, observed.Observation);
                return SaveLoadResult<LocalProfileState>.Failure(SaveLoadStatus.SplitBrain, "SAVE_SPLIT_BRAIN", observed.Observation);
            }
            return SaveLoadResult<LocalProfileState>.Success(observed.Highest.Decoded!.Snapshot!, observed.Observation);
        }
        /// <summary>Reads all three sources on a worker without file creation, silent initialization or repairs.</summary>
        public Task<SaveLoadResult<LocalProfileState>> LoadAsync(CancellationToken cancellationToken) => Task.Run(() =>
        {
            try
            {
                using (files.AcquireExclusive(cancellationToken)) { lease = null; return LoadObserved(Observe(cancellationToken)); }
            }
            catch (OperationCanceledException) { return SaveLoadResult<LocalProfileState>.Failure(SaveLoadStatus.Cancelled, "SAVE_CANCELLED"); }
            catch (Exception e) when (KnownFailure(e)) { return SaveLoadResult<LocalProfileState>.Failure(SaveLoadStatus.Failed, FileError(e)); }
        });
        /// <summary>Pure read-only explicit selection; its one-shot promotion permission binds every source and the exact returned payload.</summary>
        public Task<SaveLoadResult<LocalProfileState>> RecoverAsync(SaveRecoveryRequest request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            return Task.Run(() =>
            {
                try
                {
                    using (files.AcquireExclusive(cancellationToken))
                    {
                        lease = null; var observed = Observe(cancellationToken);
                        if (!request.Matches(observed.Observation)) return SaveLoadResult<LocalProfileState>.Failure(SaveLoadStatus.Conflict, "SAVE_OBSERVATION_CONFLICT", observed.Observation);
                        if (observed.Has(SaveCandidateStatus.Unsupported) || observed.Has(SaveCandidateStatus.Failed)) return LoadObserved(observed);
                        var selected = observed.Candidates[(int)request.SelectedKind];
                        if (selected.Provenance.Status != SaveCandidateStatus.Valid || selected.Decoded?.Snapshot == null)
                            return SaveLoadResult<LocalProfileState>.Failure(SaveLoadStatus.Corrupt, "SAVE_RECOVERY_SELECTION_NOT_VALID", observed.Observation);
                        if (observed.HighestGeneration == MaxSafe) return SaveLoadResult<LocalProfileState>.Failure(SaveLoadStatus.Failed, "SAVE_GENERATION_EXHAUSTED", observed.Observation);
                        string targetHash = LocalSaveHash.Bytes(codec.PayloadBytes(selected.Decoded.Snapshot.Payload));
                        cancellationToken.ThrowIfCancellationRequested();
                        lease = new RecoveryLease(observed.Observation, targetHash, observed.HighestGeneration + 1);
                        return SaveLoadResult<LocalProfileState>.Success(selected.Decoded.Snapshot, observed.Observation);
                    }
                }
                catch (OperationCanceledException) { return SaveLoadResult<LocalProfileState>.Failure(SaveLoadStatus.Cancelled, "SAVE_CANCELLED"); }
                catch (Exception e) when (KnownFailure(e)) { return SaveLoadResult<LocalProfileState>.Failure(SaveLoadStatus.Failed, FileError(e)); }
            });
        }
        /// <summary>Normal writes never initialize missing profiles or bypass a recovery decision.</summary>
        public Task<SaveCommitResult<LocalProfileState>> CommitAsync(SaveSnapshot<LocalProfileState> snapshot, long expectedGeneration, CancellationToken cancellationToken)
            => WriteAsync(snapshot, expectedGeneration, WriteKind.Normal, null, cancellationToken);
        /// <summary>Explicit fileless-profile creation, distinct from normal Commit.</summary>
        public Task<SaveCommitResult<LocalProfileState>> CreateProfileAsync(LocalProfileState state, CancellationToken cancellationToken)
            => WriteAsync(new SaveSnapshot<LocalProfileState>(codec.Profile, codec.Catalog, 1, state), 0, WriteKind.Initialize, null, cancellationToken);
        /// <summary>Separate confirmed corrupt restart; exact original sources are preserved, unsupported/unreadable sources block it.</summary>
        public Task<SaveCommitResult<LocalProfileState>> ConfirmedResetCorruptProfileAsync(SaveRepositoryObservation observation, LocalProfileState state, CancellationToken cancellationToken)
        {
            if (observation == null) throw new ArgumentNullException(nameof(observation));
            long maximum = observation.Candidates.Max(x => x.Generation ?? 0);
            if (maximum == MaxSafe) return Task.FromResult(SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed, "SAVE_GENERATION_EXHAUSTED"));
            return WriteAsync(new SaveSnapshot<LocalProfileState>(codec.Profile, codec.Catalog, maximum + 1, state), maximum, WriteKind.ConfirmedCorruptReset, observation, cancellationToken);
        }
        private enum WriteKind { Normal, Initialize, ConfirmedCorruptReset }
        // Connected after actual 43/43 fixed Codec/Node-byte goldens (original XML494dedeb..., 2026-10-19T10:31:18Z).
        private Task<SaveCommitResult<LocalProfileState>> WriteAsync(SaveSnapshot<LocalProfileState> snapshot, long expectedGeneration, WriteKind kind,
            SaveRepositoryObservation? observation, CancellationToken ct) => Task.Run(() =>
        {
            bool effects = false;
            try
            {
                using (files.AcquireExclusive(ct))
                {
                    ct.ThrowIfCancellationRequested();
                    if (snapshot == null || !snapshot.Profile.Equals(codec.Profile) || !snapshot.Catalog.Equals(codec.Catalog))
                        return Failure(SaveCommitStatus.Failed, "SAVE_PROFILE_BINDING_MISMATCH");
                    if (expectedGeneration < 0 || expectedGeneration >= MaxSafe || snapshot.Generation != expectedGeneration + 1)
                        return Failure(SaveCommitStatus.Failed, expectedGeneration == MaxSafe ? "SAVE_GENERATION_EXHAUSTED" : "SAVE_GENERATION_INVALID");
                    var observed = Observe(ct);
                    if (observed.Has(SaveCandidateStatus.Failed)) { lease = null; return Failure(SaveCommitStatus.RecoveryRequired, "SAVE_CANDIDATE_UNREADABLE"); }
                    if (observed.Has(SaveCandidateStatus.Unsupported)) { lease = null; return Failure(SaveCommitStatus.Unsupported, "SAVE_SOURCE_UNSUPPORTED"); }
                    bool promotion = false;
                    if (kind == WriteKind.Initialize)
                    {
                        lease = null;
                        if (!observed.AllMissing || expectedGeneration != 0) return Failure(SaveCommitStatus.Conflict, "SAVE_INITIALIZATION_CONFLICT");
                    }
                    else if (kind == WriteKind.ConfirmedCorruptReset)
                    {
                        lease = null;
                        if (observation == null || !observation.Equals(observed.Observation)) return Failure(SaveCommitStatus.Conflict, "SAVE_OBSERVATION_CONFLICT");
                        if (!observed.Has(SaveCandidateStatus.Corrupt)) return Failure(SaveCommitStatus.Failed, "SAVE_CORRUPT_RESET_NOT_REQUIRED");
                        if (expectedGeneration != observed.Candidates.Max(x => x.Provenance.Generation ?? 0)) return Failure(SaveCommitStatus.Conflict, "SAVE_GENERATION_CONFLICT");
                    }
                    else
                    {
                        if (observed.AllMissing) { lease = null; return Failure(SaveCommitStatus.Failed, "SAVE_EXPLICIT_INITIALIZATION_REQUIRED"); }
                        if (lease != null)
                        {
                            if (!lease.Observation.Equals(observed.Observation)) { lease = null; return Failure(SaveCommitStatus.Conflict, "SAVE_OBSERVATION_CONFLICT"); }
                            if (snapshot.Generation != lease.Generation || expectedGeneration != observed.HighestGeneration)
                                return Failure(SaveCommitStatus.Conflict, "SAVE_RECOVERY_GENERATION_MISMATCH");
                            promotion = true;
                        }
                        else
                        {
                            if (observed.SplitBrain) return Failure(SaveCommitStatus.SplitBrain, "SAVE_SPLIT_BRAIN");
                            if (observed.Highest == null) return Failure(SaveCommitStatus.Corrupt, "SAVE_NO_VALID_CANDIDATE");
                            if (expectedGeneration != observed.HighestGeneration) return Failure(SaveCommitStatus.Conflict, "SAVE_GENERATION_CONFLICT");
                        }
                    }
                    byte[] targetPayload = codec.PayloadBytes(snapshot.Payload);
                    string targetHash = LocalSaveHash.Bytes(targetPayload);
                    if (promotion && targetHash != lease!.TargetPayloadHash)
                        return Failure(SaveCommitStatus.Conflict, "SAVE_RECOVERY_PAYLOAD_MISMATCH");
                    Point("step.1.validated", ct);
                    byte[] target = codec.Encode(snapshot, clock.UtcNow, appVersion, buildId);
                    Point("step.2.encoded", ct);
                    // Last cancellation point that can truthfully guarantee no possible storage effect.
                    ct.ThrowIfCancellationRequested();
                    if (promotion) lease = null;
                    effects = true;
                    files.EnsureWriteDirectory();
                    if (promotion || kind == WriteKind.ConfirmedCorruptReset)
                    {
                        foreach (var candidate in observed.Candidates)
                        {
                            ct.ThrowIfCancellationRequested();
                            if (candidate.Bytes != null) files.PreserveSource("recovery", candidate.Provenance.Kind, candidate.Bytes);
                        }
                    }
                    foreach (var candidate in observed.Candidates.Where(x => x.Provenance.Status == SaveCandidateStatus.Valid))
                    {
                        ct.ThrowIfCancellationRequested();
                        bool migrated = candidate.Decoded!.SourceSchemaVersion != 2 || candidate.Decoded.SourcePayloadSha256 !=
                            LocalSaveHash.Bytes(codec.PayloadBytes(candidate.Decoded.Snapshot!.Payload));
                        if (migrated) files.PreserveSource("migration-source", candidate.Provenance.Kind, candidate.Bytes!);
                    }
                    var previous = observed.Highest;
                    if (previous != null)
                    {
                        // Keep the untouched highest candidate until a second regular candidate has been flushed and fully verified.
                        SaveCandidateKind safetyKind = previous.Provenance.Kind == SaveCandidateKind.Backup ? SaveCandidateKind.Main : SaveCandidateKind.Backup;
                        ct.ThrowIfCancellationRequested();
                        files.WriteCandidate(safetyKind, previous.Bytes!, "preparation.highest");
                        VerifyExact(safetyKind, previous.Bytes!, previous.Decoded!.Snapshot!.Generation, previous.Provenance.PayloadSha256!, ct);
                        Point("preparation.highest.verified", ct);
                    }
                    // This is the sole destructive Pending-open site, after the regular highest-copy invariant above.
                    ct.ThrowIfCancellationRequested();
                    files.WriteCandidate(SaveCandidateKind.Pending, target, "pending");
                    VerifyExact(SaveCandidateKind.Pending, target, snapshot.Generation, targetHash, ct);
                    Point("step.5.pending-verified", ct);
                    if (previous != null)
                    {
                        ct.ThrowIfCancellationRequested();
                        // The new pending is now independently valid; never rotate a corrupt or lower main over the previous highest.
                        files.WriteCandidate(SaveCandidateKind.Backup, previous.Bytes!, "backup.rotate");
                        VerifyExact(SaveCandidateKind.Backup, previous.Bytes!, previous.Decoded!.Snapshot!.Generation, previous.Provenance.PayloadSha256!, ct);
                        Point("backup.rotate.verified", ct);
                    }
                    Point("step.6.backup-preserved", ct);
                    files.PromotePending(target);
                    Point("step.7.promoted", ct);
                    VerifyExact(SaveCandidateKind.Main, target, snapshot.Generation, targetHash, ct);
                    Point("step.8.main-verified", ct);
                    Point("step.9.before-ack", ct);
                    return SaveCommitResult<LocalProfileState>.Success(snapshot);
                }
            }
            catch (OperationCanceledException)
            {
                if (effects) lease = null;
                return Failure(effects ? SaveCommitStatus.RecoveryRequired : SaveCommitStatus.Cancelled, effects ? "SAVE_CANCELLED_AFTER_POSSIBLE_EFFECT" : "SAVE_CANCELLED");
            }
            catch (SaveCodecFailure e)
            {
                if (effects) lease = null;
                return Failure(effects ? SaveCommitStatus.RecoveryRequired : e.Unsupported ? SaveCommitStatus.Unsupported : SaveCommitStatus.Corrupt, FileError(e));
            }
            catch (Exception e) when (KnownFailure(e))
            {
                if (effects) lease = null;
                return Failure(effects ? SaveCommitStatus.RecoveryRequired : SaveCommitStatus.Failed, FileError(e));
            }
        });
        private void Point(string point, CancellationToken ct) { ct.ThrowIfCancellationRequested(); files.Checkpoint(point); ct.ThrowIfCancellationRequested(); }
        private void VerifyExact(SaveCandidateKind kind, byte[] expected, long generation, string payloadHash, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            byte[]? actual = files.ReadCandidate(kind, LocalSaveHash.Limits.MaxBytes);
            if (actual == null || !actual.SequenceEqual(expected)) throw new IOException("SAVE_REREAD_BYTES_MISMATCH");
            var decoded = codec.Decode(actual);
            if (decoded.Status != LocalSaveDecodeStatus.Valid || decoded.Snapshot?.Generation != generation || decoded.SourcePayloadSha256 != payloadHash)
                throw new IOException("SAVE_REREAD_VALIDATION_FAILED");
            ct.ThrowIfCancellationRequested();
        }
        private static SaveCommitResult<LocalProfileState> Failure(SaveCommitStatus status, string code) => SaveCommitResult<LocalProfileState>.Failure(status, code);
        private static bool KnownFailure(Exception e) => e is IOException || e is UnauthorizedAccessException || e is ArgumentException ||
            e is InvalidOperationException || e is NotSupportedException || e is TimeoutException || e is SaveCodecFailure;
        private static string FileError(Exception e)
        {
            if (e is UnauthorizedAccessException) return "SAVE_PERMISSION_DENIED";
            if (e is TimeoutException) return "SAVE_WRITER_BUSY";
            if (e is PathTooLongException) return "SAVE_PATH_TOO_LONG";
            if (e is IOException && ((e.HResult & 65535) == 112 || (e.HResult & 65535) == 39 || (e.HResult & 65535) == 28)) return "SAVE_LOW_DISK";
            string code = e.Message;
            if (code.StartsWith("SAVE_", StringComparison.Ordinal) && code.All(c => c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == '_')) return code;
            return e is IOException ? "SAVE_IO_FAILED" : "SAVE_OPERATION_FAILED";
        }
    }
}
