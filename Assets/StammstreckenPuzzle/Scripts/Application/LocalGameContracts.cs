using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using STP.Application.Ports;
using STP.Puzzle.Domain;

namespace STP.Application
{
    /// <summary>Explicit catalog consumers; review and contract data never become production data.</summary>
    public enum CatalogUse
    {
        /// <summary>Production consumer, subject to all independent import and release gates.</summary>
        Production,
        /// <summary>Isolated contract tests only.</summary>
        ContractTest,
        /// <summary>The separately authorized, nonproductive Public240 review.</summary>
        IsolatedReview
    }

    /// <summary>Source approval metadata, never an application decision to promote content.</summary>
    public enum CatalogApprovalStatus
    {
        /// <summary>Review source with no product approval.</summary>
        Draft,
        /// <summary>Contract-test source forbidden in runtime production.</summary>
        FixtureOnly,
        /// <summary>Product approval metadata; remaining import and release checks still apply.</summary>
        ProductApproved
    }

    /// <summary>Clockwise exterior sides for public puzzle inputs.</summary>
    public enum PublicPuzzleSide
    {
        /// <summary>North exterior edge.</summary>
        N,
        /// <summary>East exterior edge.</summary>
        E,
        /// <summary>South exterior edge.</summary>
        S,
        /// <summary>West exterior edge.</summary>
        W
    }

    /// <summary>A syntactically validated public identity triplet; its loader must verify the hash against public bytes.</summary>
    public sealed class PublicPuzzleIdentity : IEquatable<PublicPuzzleIdentity>
    {
        /// <summary>The existing public semantic hash profile, without an alternate projection.</summary>
        public const string SemanticHashProfile = "STP-PUZZLE-SEMANTIC-JCS-1";

        /// <summary>Creates a stable ID, supported hash profile and lowercase SHA-256 identity.</summary>
        public PublicPuzzleIdentity(string puzzleId, string hashProfile, string hashSha256)
        {
            PuzzleId = ContractGuard.Identifier(puzzleId, nameof(puzzleId));
            if (hashProfile != SemanticHashProfile) throw new ArgumentException("PUZZLE_HASH_PROFILE_UNSUPPORTED", nameof(hashProfile));
            HashProfile = hashProfile;
            HashSha256 = ContractGuard.Hash(hashSha256, nameof(hashSha256));
        }

        /// <summary>Stable puzzle ID; it carries no solution information.</summary>
        public string PuzzleId { get; }
        /// <summary>Explicit public semantic hash profile.</summary>
        public string HashProfile { get; }
        /// <summary>Exactly 64 lowercase hexadecimal characters.</summary>
        public string HashSha256 { get; }
        /// <summary>Compares the complete ordinal identity triplet.</summary>
        public bool Equals(PublicPuzzleIdentity? other) => other != null && PuzzleId == other.PuzzleId && HashProfile == other.HashProfile && HashSha256 == other.HashSha256;
        /// <summary>Compares the complete ordinal identity triplet.</summary>
        public override bool Equals(object? obj) => obj is PublicPuzzleIdentity other && Equals(other);
        /// <summary>Returns a process-local value hash; this is never a content hash.</summary>
        public override int GetHashCode() => ContractGuard.CombineHash(PuzzleId, HashProfile, HashSha256);
    }

    /// <summary>Immutable exterior endpoint projected from a validated domain definition.</summary>
    public sealed class PublicPuzzleEndpoint
    {
        internal PublicPuzzleEndpoint(Endpoint endpoint)
        {
            Side = (PublicPuzzleSide)endpoint.Side;
            Index = endpoint.Index;
            X = endpoint.Cell.X;
            Y = endpoint.Cell.Y;
        }

        /// <summary>Exterior side.</summary>
        public PublicPuzzleSide Side { get; }
        /// <summary>Zero-based position along that side.</summary>
        public int Index { get; }
        /// <summary>Adjacent interior column.</summary>
        public int X { get; }
        /// <summary>Adjacent interior row.</summary>
        public int Y { get; }
    }

    /// <summary>Defensively copied, domain-validated public inputs without solution, proof, hints or authoring metadata.</summary>
    public sealed class PublicPuzzleInput
    {
        private readonly PuzzleDefinition definition;

        /// <summary>Maps primitive public inputs through the existing domain factories; invalid definitions are rejected.</summary>
        public PublicPuzzleInput(PublicPuzzleIdentity identity, string rulesetVersion, int width, int height,
            PublicPuzzleSide aSide, int aIndex, PublicPuzzleSide bSide, int bIndex,
            IReadOnlyList<int> rows, IReadOnlyList<int> columns)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            var gridResult = GridSize.Create(width, height);
            var grid = gridResult.Value ?? throw new ArgumentException("INVALID_DEFINITION");
            var a = Endpoint.Create(grid, (Direction)aSide, aIndex).Value;
            var b = Endpoint.Create(grid, (Direction)bSide, bIndex).Value;
            var mapped = PuzzleDefinition.Create(grid, a, b, rows, columns, rulesetVersion);
            definition = mapped.Value ?? throw new ArgumentException(mapped.Error.ToString());
            A = new PublicPuzzleEndpoint(definition.A);
            B = new PublicPuzzleEndpoint(definition.B);
        }

        /// <summary>Full public puzzle identity, to be hash-verified by the source adapter.</summary>
        public PublicPuzzleIdentity Identity { get; }
        /// <summary>Ruleset accepted by the domain.</summary>
        public string RulesetVersion => definition.RulesetVersion;
        /// <summary>Validated grid width.</summary>
        public int Width => definition.Grid.Width;
        /// <summary>Validated grid height.</summary>
        public int Height => definition.Grid.Height;
        /// <summary>Entry endpoint.</summary>
        public PublicPuzzleEndpoint A { get; }
        /// <summary>Exit endpoint.</summary>
        public PublicPuzzleEndpoint B { get; }
        /// <summary>Read-only defensive copy of row targets.</summary>
        public IReadOnlyList<int> RowCounts => definition.RowCounts;
        /// <summary>Read-only defensive copy of column targets.</summary>
        public IReadOnlyList<int> ColumnCounts => definition.ColumnCounts;
        /// <summary>Returns the already validated immutable domain input; it has no solution or mutable session state.</summary>
        public PuzzleDefinition ToDomainDefinition() => definition;
    }

    /// <summary>Exact locked catalog provenance plus its isolated consumer and source status.</summary>
    public sealed class CatalogIdentity : IEquatable<CatalogIdentity>
    {
        /// <summary>Creates a catalog reference; status/use mismatch is rejected without inferring product approval.</summary>
        public CatalogIdentity(string catalogId, int revision, string hashSha256, CatalogUse use, CatalogApprovalStatus approvalStatus)
        {
            CatalogId = ContractGuard.Identifier(catalogId, nameof(catalogId));
            if (revision < 1) throw new ArgumentOutOfRangeException(nameof(revision), "INVALID_CATALOG_REVISION");
            ContractGuard.Use(use);
            if (!Enum.IsDefined(typeof(CatalogApprovalStatus), approvalStatus)) throw new ArgumentOutOfRangeException(nameof(approvalStatus));
            if ((use == CatalogUse.Production && approvalStatus != CatalogApprovalStatus.ProductApproved) ||
                (use == CatalogUse.ContractTest && approvalStatus != CatalogApprovalStatus.FixtureOnly) ||
                (use == CatalogUse.IsolatedReview && approvalStatus != CatalogApprovalStatus.Draft))
                throw new ArgumentException("CATALOG_USE_STATUS_MISMATCH");
            if (use != CatalogUse.IsolatedReview && (catalogId == SaveProfileIdentity.Public240ReviewNamespace ||
                catalogId.StartsWith(SaveProfileIdentity.Public240ReviewNamespace + "-", StringComparison.Ordinal)))
                throw new ArgumentException("REVIEW_CATALOG_PROMOTION_FORBIDDEN");
            Revision = revision;
            HashSha256 = ContractGuard.Hash(hashSha256, nameof(hashSha256));
            Use = use;
            ApprovalStatus = approvalStatus;
        }

        /// <summary>Stable catalog line.</summary>
        public string CatalogId { get; }
        /// <summary>Positive catalog revision.</summary>
        public int Revision { get; }
        /// <summary>Lowercase hash of the exact locked catalog bytes under its existing contract.</summary>
        public string HashSha256 { get; }
        /// <summary>Explicit consumer isolation.</summary>
        public CatalogUse Use { get; }
        /// <summary>Source approval metadata; this property does not grant import or release eligibility.</summary>
        public CatalogApprovalStatus ApprovalStatus { get; }
        /// <summary>Compares all provenance and isolation fields.</summary>
        public bool Equals(CatalogIdentity? other) => other != null && CatalogId == other.CatalogId && Revision == other.Revision &&
            HashSha256 == other.HashSha256 && Use == other.Use && ApprovalStatus == other.ApprovalStatus;
        /// <summary>Compares all provenance and isolation fields.</summary>
        public override bool Equals(object? obj) => obj is CatalogIdentity other && Equals(other);
        /// <summary>Returns a process-local value hash, never a catalog content hash.</summary>
        public override int GetHashCode() => unchecked(ContractGuard.CombineHash(CatalogId, HashSha256, Revision.ToString(System.Globalization.CultureInfo.InvariantCulture)) * 31 + (int)Use * 7 + (int)ApprovalStatus);
    }

    /// <summary>An immutable flattened campaign membership with explicit first-clear requirements.</summary>
    public sealed class LockedCampaignEntry
    {
        /// <summary>Creates locked membership; the adapter validates the complete graph, including implicit parent requirements.</summary>
        public LockedCampaignEntry(CatalogIdentity catalog, PublicPuzzleIdentity puzzle, string seasonId, string sectionId,
            string routeId, int ordinal, IReadOnlyList<string> requiredFirstClears)
        {
            Catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            Puzzle = puzzle ?? throw new ArgumentNullException(nameof(puzzle));
            SeasonId = ContractGuard.Identifier(seasonId, nameof(seasonId));
            SectionId = ContractGuard.Identifier(sectionId, nameof(sectionId));
            RouteId = ContractGuard.Identifier(routeId, nameof(routeId));
            if (ordinal < 1) throw new ArgumentOutOfRangeException(nameof(ordinal), "INVALID_CAMPAIGN_ORDINAL");
            Ordinal = ordinal;
            if (requiredFirstClears == null) throw new ArgumentNullException(nameof(requiredFirstClears));
            var required = new string[requiredFirstClears.Count];
            var unique = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < required.Length; i++)
            {
                string subject = ContractGuard.Identifier(requiredFirstClears[i], nameof(requiredFirstClears));
                if (subject == Puzzle.PuzzleId || !unique.Add(subject)) throw new ArgumentException("INVALID_FIRST_CLEAR_REQUIREMENT", nameof(requiredFirstClears));
                required[i] = subject;
            }
            RequiredFirstClears = Array.AsReadOnly(required);
        }

        /// <summary>Exact campaign catalog provenance.</summary>
        public CatalogIdentity Catalog { get; }
        /// <summary>Exact public puzzle identity.</summary>
        public PublicPuzzleIdentity Puzzle { get; }
        /// <summary>Parent season subject ID.</summary>
        public string SeasonId { get; }
        /// <summary>Parent section subject ID.</summary>
        public string SectionId { get; }
        /// <summary>Parent route subject ID.</summary>
        public string RouteId { get; }
        /// <summary>Positive deterministic puzzle order within this locked catalog.</summary>
        public int Ordinal { get; }
        /// <summary>Copied subject IDs whose complete first-clears are required; empty means no explicit requirement.</summary>
        public IReadOnlyList<string> RequiredFirstClears { get; }
    }

    /// <summary>Explicit confirmed base rewards and optional per-puzzle star times from one locked completion catalog.</summary>
    public sealed class CompletionValues
    {
        /// <summary>Validates supplied confirmed values; missing times remain null and no timing default is invented.</summary>
        public CompletionValues(CatalogIdentity catalog, PublicPuzzleIdentity puzzle, int firstClear, int secondStar,
            int thirdStar, int routeClear, int sectionClear, long? twoStarMilliseconds, long? threeStarMilliseconds)
        {
            Catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            Puzzle = puzzle ?? throw new ArgumentNullException(nameof(puzzle));
            if (firstClear != 15 || secondStar != 5 || thirdStar != 10 || routeClear != 60 || sectionClear != 150)
                throw new ArgumentException("UNCONFIRMED_COMPLETION_VALUE");
            if (twoStarMilliseconds.HasValue != threeStarMilliseconds.HasValue)
                throw new ArgumentException("INCOMPLETE_STAR_THRESHOLDS");
            if (twoStarMilliseconds.HasValue)
            {
                ContractGuard.PositiveSafeInteger(twoStarMilliseconds.Value, nameof(twoStarMilliseconds));
                ContractGuard.PositiveSafeInteger(threeStarMilliseconds!.Value, nameof(threeStarMilliseconds));
                if (threeStarMilliseconds.Value >= twoStarMilliseconds.Value) throw new ArgumentException("INVALID_STAR_THRESHOLD_ORDER");
            }
            FirstClear = firstClear;
            SecondStar = secondStar;
            ThirdStar = thirdStar;
            RouteClear = routeClear;
            SectionClear = sectionClear;
            TwoStarMs = twoStarMilliseconds;
            ThreeStarMs = threeStarMilliseconds;
        }

        /// <summary>Exact completion catalog provenance.</summary>
        public CatalogIdentity Catalog { get; }
        /// <summary>Puzzle to which these level-specific thresholds belong.</summary>
        public PublicPuzzleIdentity Puzzle { get; }
        /// <summary>Confirmed first-clear Geduldspunkte.</summary>
        public int FirstClear { get; }
        /// <summary>Confirmed additional second-star Geduldspunkte.</summary>
        public int SecondStar { get; }
        /// <summary>Confirmed additional third-star Geduldspunkte.</summary>
        public int ThirdStar { get; }
        /// <summary>Confirmed complete-route Geduldspunkte.</summary>
        public int RouteClear { get; }
        /// <summary>Confirmed complete-section Geduldspunkte.</summary>
        public int SectionClear { get; }
        /// <summary>Explicit two-star upper time bound, or null when no threshold is supplied.</summary>
        public long? TwoStarMs { get; }
        /// <summary>Explicit three-star upper time bound, or null when no threshold is supplied.</summary>
        public long? ThreeStarMs { get; }
    }

    /// <summary>Typed public-only catalog; adapters verify identities and return immutable snapshots.</summary>
    public interface IPublicPuzzleCatalog : ILevelCatalog
    {
        /// <summary>Exact source lock and consumer isolation.</summary>
        CatalogIdentity Identity { get; }
        /// <summary>Immutable catalog-ordered public puzzles, never authoring documents.</summary>
        IReadOnlyList<PublicPuzzleInput> Puzzles { get; }
        /// <summary>Finds an exact identity; false requires a null output and never falls back to another hash or fixture.</summary>
        bool TryGet(PublicPuzzleIdentity identity, out PublicPuzzleInput? puzzle);
    }

    /// <summary>Locked campaign graph read port; no mutable progress or star-based unlock requirement.</summary>
    public interface ILockedCampaignCatalog : ICampaignCatalog
    {
        /// <summary>Exact campaign lock and consumer isolation.</summary>
        CatalogIdentity Identity { get; }
        /// <summary>Immutable ordered entries from a fully validated, reachable and acyclic campaign graph.</summary>
        IReadOnlyList<LockedCampaignEntry> Entries { get; }
        /// <summary>Finds exact membership; false requires null and never substitutes another puzzle identity.</summary>
        bool TryGet(PublicPuzzleIdentity identity, out LockedCampaignEntry? entry);
    }

    /// <summary>Locked completion read port; supplied values are definitions, never reward eligibility.</summary>
    public interface ILockedCompletionCatalog : ICompletionCatalog
    {
        /// <summary>Exact completion lock and consumer isolation.</summary>
        CatalogIdentity Identity { get; }
        /// <summary>Finds exact level-bound values; absent definitions remain errors rather than defaults.</summary>
        bool TryGet(PublicPuzzleIdentity identity, out CompletionValues? values);
    }

    /// <summary>Logical profile identity; the persistence adapter alone maps it to storage.</summary>
    public sealed class SaveProfileIdentity : IEquatable<SaveProfileIdentity>
    {
        /// <summary>Exclusively isolated Public240 review namespace under ADR034.</summary>
        public const string Public240ReviewNamespace = "qa-public240-v1";

        /// <summary>Creates a profile identity; the Public240 namespace and IsolatedReview use must occur together.</summary>
        public SaveProfileIdentity(string profileNamespace, string profileId, CatalogUse use)
        {
            Namespace = ContractGuard.Identifier(profileNamespace, nameof(profileNamespace));
            ProfileId = ContractGuard.Identifier(profileId, nameof(profileId));
            ContractGuard.Use(use);
            if ((profileNamespace == Public240ReviewNamespace) != (use == CatalogUse.IsolatedReview))
                throw new ArgumentException("PROFILE_USE_NAMESPACE_MISMATCH");
            Use = use;
        }

        /// <summary>Logical namespace, never a filesystem path.</summary>
        public string Namespace { get; }
        /// <summary>Stable logical profile ID.</summary>
        public string ProfileId { get; }
        /// <summary>Mandatory profile/catalog consumer isolation.</summary>
        public CatalogUse Use { get; }
        /// <summary>Compares namespace, ID and consumer isolation.</summary>
        public bool Equals(SaveProfileIdentity? other) => other != null && Namespace == other.Namespace && ProfileId == other.ProfileId && Use == other.Use;
        /// <summary>Compares namespace, ID and consumer isolation.</summary>
        public override bool Equals(object? obj) => obj is SaveProfileIdentity other && Equals(other);
        /// <summary>Returns a process-local hash, never a persistent profile hash.</summary>
        public override int GetHashCode() => unchecked(ContractGuard.CombineHash(Namespace, ProfileId, string.Empty) * 31 + (int)Use);
    }

    /// <summary>
    /// Ownership contract for deeply immutable, validated save payload DTOs. Implementations must defensively copy
    /// all mutable inputs, expose no writable collections or provider objects, and preserve this guarantee recursively.
    /// A marker alone does not validate a payload; each concrete codec and its contract tests remain responsible.
    /// </summary>
    public interface IImmutableSavePayload { }

    /// <summary>Immutable, typed profile/catalog/generation binding; no filesystem or serializer implementation is exposed.</summary>
    public sealed class SaveSnapshot<T> where T : class, IImmutableSavePayload
    {
        /// <summary>Creates a validated binding to an already deeply immutable payload; profile and catalog uses must match.</summary>
        public SaveSnapshot(SaveProfileIdentity profile, CatalogIdentity catalog, long generation, T payload)
        {
            Profile = profile ?? throw new ArgumentNullException(nameof(profile));
            Catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            if (profile.Use != catalog.Use) throw new ArgumentException("SAVE_PROFILE_CATALOG_MISMATCH");
            ContractGuard.NonnegativeSafeInteger(generation, nameof(generation));
            Generation = generation;
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
        }

        /// <summary>Logical profile and isolation identity.</summary>
        public SaveProfileIdentity Profile { get; }
        /// <summary>Exact catalog against which the immutable payload was validated.</summary>
        public CatalogIdentity Catalog { get; }
        /// <summary>Generation in the existing save-safe integer range; each commit must increment the expected generation by one.</summary>
        public long Generation { get; }
        /// <summary>Validated deeply immutable payload whose ownership contract is enforced by the concrete DTO and codec.</summary>
        public T Payload { get; }
    }

    /// <summary>Closed identities of the three persistence candidates; these are logical roles, never paths.</summary>
    public enum SaveCandidateKind
    {
        /// <summary>The authoritative main candidate.</summary>
        Main,
        /// <summary>The retained previous candidate.</summary>
        Backup,
        /// <summary>The pending-write candidate.</summary>
        Pending
    }

    /// <summary>What the repository established about one candidate during a complete observation.</summary>
    public enum SaveCandidateStatus
    {
        /// <summary>The candidate was verified to be absent.</summary>
        Missing,
        /// <summary>The candidate's bytes, hash profile and payload were fully validated.</summary>
        Valid,
        /// <summary>Exact bytes were read but failed corruption or semantic checks.</summary>
        Corrupt,
        /// <summary>Exact bytes were read but their schema or profile is unsupported.</summary>
        Unsupported,
        /// <summary>The repository could not establish the candidate's complete state.</summary>
        Failed
    }

    /// <summary>Immutable provenance of exactly observed bytes, including invalid candidates; contains no filesystem address.</summary>
    public sealed class SaveCandidateProvenance : IEquatable<SaveCandidateProvenance>
    {
        /// <summary>
        /// Creates observed provenance. Valid requires a safe generation and both hashes; Missing permits no metadata.
        /// Corrupt and Unsupported require the exact source-byte hash. Failed may lack hashes and blocks recovery selection.
        /// Hash and generation metadata, when supplied, must always be syntactically valid.
        /// </summary>
        public SaveCandidateProvenance(SaveCandidateKind kind, SaveCandidateStatus status, long? generation,
            string? payloadSha256, string? sourceBytesSha256)
        {
            if (!Enum.IsDefined(typeof(SaveCandidateKind), kind)) throw new ArgumentOutOfRangeException(nameof(kind));
            if (!Enum.IsDefined(typeof(SaveCandidateStatus), status)) throw new ArgumentOutOfRangeException(nameof(status));
            if (generation.HasValue) ContractGuard.NonnegativeSafeInteger(generation.Value, nameof(generation));
            if (payloadSha256 != null) ContractGuard.Hash(payloadSha256, nameof(payloadSha256));
            if (sourceBytesSha256 != null) ContractGuard.Hash(sourceBytesSha256, nameof(sourceBytesSha256));
            if (status == SaveCandidateStatus.Valid && (!generation.HasValue || payloadSha256 == null || sourceBytesSha256 == null))
                throw new ArgumentException("INCOMPLETE_VALID_SAVE_CANDIDATE");
            if (status == SaveCandidateStatus.Missing && (generation.HasValue || payloadSha256 != null || sourceBytesSha256 != null))
                throw new ArgumentException("MISSING_SAVE_CANDIDATE_HAS_METADATA");
            if ((status == SaveCandidateStatus.Corrupt || status == SaveCandidateStatus.Unsupported) && sourceBytesSha256 == null)
                throw new ArgumentException("SAVE_CANDIDATE_SOURCE_HASH_REQUIRED");
            Kind = kind;
            Status = status;
            Generation = generation;
            PayloadSha256 = payloadSha256;
            SourceBytesSha256 = sourceBytesSha256;
        }

        /// <summary>Logical Main, Backup or Pending role.</summary>
        public SaveCandidateKind Kind { get; }
        /// <summary>Established read/validation status.</summary>
        public SaveCandidateStatus Status { get; }
        /// <summary>Observed safe generation when known; not an instruction to reset a writer generation.</summary>
        public long? Generation { get; }
        /// <summary>Validated lowercase payload hash when known.</summary>
        public string? PayloadSha256 { get; }
        /// <summary>Lowercase SHA-256 of the exact complete observed source bytes, including their envelope.</summary>
        public string? SourceBytesSha256 { get; }
        /// <summary>Compares every observed field, including exact source-byte identity, ordinally.</summary>
        public bool Equals(SaveCandidateProvenance? other) => other != null && Kind == other.Kind && Status == other.Status &&
            Generation == other.Generation && PayloadSha256 == other.PayloadSha256 && SourceBytesSha256 == other.SourceBytesSha256;
        /// <summary>Compares every observed field, including exact source-byte identity, ordinally.</summary>
        public override bool Equals(object? obj) => obj is SaveCandidateProvenance other && Equals(other);
        /// <summary>Returns a process-local equality hash, never an observed file or payload hash.</summary>
        public override int GetHashCode() => unchecked(ContractGuard.CombineHash(PayloadSha256 ?? string.Empty,
            SourceBytesSha256 ?? string.Empty, Generation?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty) * 31 + (int)Kind * 7 + (int)Status);
    }

    /// <summary>A complete immutable observation binding all three candidates to one exact profile and catalog.</summary>
    public sealed class SaveRepositoryObservation : IEquatable<SaveRepositoryObservation>
    {
        /// <summary>Requires exactly one candidate of each kind and copies them into deterministic Main/Backup/Pending order.</summary>
        public SaveRepositoryObservation(SaveProfileIdentity profile, CatalogIdentity catalog, IReadOnlyList<SaveCandidateProvenance> candidates)
        {
            Profile = profile ?? throw new ArgumentNullException(nameof(profile));
            Catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            if (profile.Use != catalog.Use) throw new ArgumentException("SAVE_PROFILE_CATALOG_MISMATCH");
            if (candidates == null) throw new ArgumentNullException(nameof(candidates));
            if (candidates.Count != 3) throw new ArgumentException("INCOMPLETE_SAVE_OBSERVATION", nameof(candidates));
            var ordered = new SaveCandidateProvenance[3];
            for (int i = 0; i < candidates.Count; i++)
            {
                var candidate = candidates[i] ?? throw new ArgumentException("NULL_SAVE_CANDIDATE", nameof(candidates));
                int index = (int)candidate.Kind;
                if (ordered[index] != null) throw new ArgumentException("DUPLICATE_SAVE_CANDIDATE", nameof(candidates));
                ordered[index] = candidate;
            }
            Candidates = Array.AsReadOnly(ordered);
        }

        /// <summary>Exact logical profile, including its isolation.</summary>
        public SaveProfileIdentity Profile { get; }
        /// <summary>Exact locked catalog, including its isolation and status.</summary>
        public CatalogIdentity Catalog { get; }
        /// <summary>Defensively copied, read-only candidate provenance in Main/Backup/Pending order.</summary>
        public IReadOnlyList<SaveCandidateProvenance> Candidates { get; }
        /// <summary>Compares profile, catalog and every field of every candidate; no same-generation shortcut is allowed.</summary>
        public bool Equals(SaveRepositoryObservation? other)
        {
            if (other == null || !Profile.Equals(other.Profile) || !Catalog.Equals(other.Catalog)) return false;
            for (int i = 0; i < Candidates.Count; i++) if (!Candidates[i].Equals(other.Candidates[i])) return false;
            return true;
        }
        /// <summary>Compares the complete profile/catalog/candidate observation ordinally.</summary>
        public override bool Equals(object? obj) => obj is SaveRepositoryObservation other && Equals(other);
        /// <summary>Returns a process-local equality hash over the complete observation.</summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Profile.GetHashCode() * 397 ^ Catalog.GetHashCode();
                for (int i = 0; i < Candidates.Count; i++) hash = hash * 397 ^ Candidates[i].GetHashCode();
                return hash;
            }
        }
    }

    /// <summary>An explicit read-only recovery selection bound to the complete original repository observation.</summary>
    public sealed class SaveRecoveryRequest
    {
        /// <summary>Requires a Valid selection and rejects every Unsupported or Failed candidate, whose state cannot authorize recovery.</summary>
        public SaveRecoveryRequest(SaveRepositoryObservation observation, SaveCandidateKind selectedKind)
        {
            Observation = observation ?? throw new ArgumentNullException(nameof(observation));
            if (!Enum.IsDefined(typeof(SaveCandidateKind), selectedKind)) throw new ArgumentOutOfRangeException(nameof(selectedKind));
            foreach (var candidate in observation.Candidates)
                if (candidate.Status == SaveCandidateStatus.Unsupported || candidate.Status == SaveCandidateStatus.Failed)
                    throw new ArgumentException("SAVE_RECOVERY_CANDIDATE_UNRESOLVED", nameof(observation));
            if (observation.Candidates[(int)selectedKind].Status != SaveCandidateStatus.Valid)
                throw new ArgumentException("SAVE_RECOVERY_SELECTION_NOT_VALID", nameof(selectedKind));
            SelectedKind = selectedKind;
        }

        /// <summary>Complete state to reobserve before reading the selected candidate; this is not permission to write.</summary>
        public SaveRepositoryObservation Observation { get; }
        /// <summary>Explicitly selected Valid candidate.</summary>
        public SaveCandidateKind SelectedKind { get; }
        /// <summary>True only for the same profile, catalog and exact bytes/status of all three newly observed candidates.</summary>
        public bool Matches(SaveRepositoryObservation observed) => Observation.Equals(observed);
    }

    /// <summary>Closed load outcomes; only Loaded carries a usable snapshot.</summary>
    public enum SaveLoadStatus
    {
        /// <summary>A validated supported snapshot was loaded.</summary>
        Loaded,
        /// <summary>The profile is completely fileless and requires an explicit initialization action.</summary>
        NeedsInitialization,
        /// <summary>A schema, profile or supported-version boundary prevents loading.</summary>
        Unsupported,
        /// <summary>Candidate bytes or their validated semantic content are corrupt.</summary>
        Corrupt,
        /// <summary>Recovery and a fresh authoritative load are required before mutation.</summary>
        RecoveryRequired,
        /// <summary>Different payload hashes share one generation; preserve all candidates and do not mutate automatically.</summary>
        SplitBrain,
        /// <summary>The repository changed since an explicit recovery selection was observed; the stale selection is rejected.</summary>
        Conflict,
        /// <summary>Cancellation completed without storage mutation.</summary>
        Cancelled,
        /// <summary>A known noncommitting failure occurred.</summary>
        Failed
    }

    /// <summary>Closed commit outcomes; only Committed confirms a re-read authoritative snapshot.</summary>
    public enum SaveCommitStatus
    {
        /// <summary>Full atomic write and authoritative main reread succeeded.</summary>
        Committed,
        /// <summary>Unsupported existing schema or profile must remain unchanged.</summary>
        Unsupported,
        /// <summary>Invalid candidate content prevents a confirmed commit.</summary>
        Corrupt,
        /// <summary>Storage effects may have occurred; reload before retry rather than asserting rollback.</summary>
        RecoveryRequired,
        /// <summary>Conflicting same-generation payloads block automatic writes.</summary>
        SplitBrain,
        /// <summary>The expected generation lost the compare-and-swap race.</summary>
        Conflict,
        /// <summary>Cancellation is guaranteed to have occurred before any possible storage effect.</summary>
        Cancelled,
        /// <summary>A known failure with no storage effect occurred.</summary>
        Failed
    }

    /// <summary>Validated load union: success contains exactly one snapshot; all other outcomes contain none.</summary>
    public sealed class SaveLoadResult<T> where T : class, IImmutableSavePayload
    {
        private SaveLoadResult(SaveLoadStatus status, SaveSnapshot<T>? snapshot, string? errorCode,
            SaveSnapshot<T>? conservativeSnapshot = null, SaveRepositoryObservation? observation = null)
        { Status = status; Snapshot = snapshot; ErrorCode = errorCode; ConservativeSnapshot = conservativeSnapshot; Observation = observation; }

        /// <summary>Closed load outcome.</summary>
        public SaveLoadStatus Status { get; }
        /// <summary>True only for a fully validated supported snapshot.</summary>
        public bool Succeeded => Status == SaveLoadStatus.Loaded;
        /// <summary>Loaded immutable snapshot, absent for every non-success outcome.</summary>
        public SaveSnapshot<T>? Snapshot { get; }
        /// <summary>Stable nonlocalized diagnostic code on failure; absent on load or fresh-profile initialization.</summary>
        public string? ErrorCode { get; }
        /// <summary>Non-authoritative conservative main candidate only on a fully observed SplitBrain; it never denotes successful recovery.</summary>
        public SaveSnapshot<T>? ConservativeSnapshot { get; }
        /// <summary>Complete observed candidate provenance when supplied, including recoverable failures and explicit read-only selections.</summary>
        public SaveRepositoryObservation? Observation { get; }
        /// <summary>Creates a successful result without an error code.</summary>
        public static SaveLoadResult<T> Success(SaveSnapshot<T> snapshot) => new SaveLoadResult<T>(SaveLoadStatus.Loaded,
            snapshot ?? throw new ArgumentNullException(nameof(snapshot)), null);
        /// <summary>Reports an observed validated read, preserving the full writer-generation/candidate context after an explicit selection.</summary>
        public static SaveLoadResult<T> Success(SaveSnapshot<T> snapshot, SaveRepositoryObservation observation)
        {
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));
            if (observation == null) throw new ArgumentNullException(nameof(observation));
            if (!snapshot.Profile.Equals(observation.Profile) || !snapshot.Catalog.Equals(observation.Catalog))
                throw new ArgumentException("SAVE_PROFILE_CATALOG_MISMATCH");
            bool matched = false;
            foreach (var candidate in observation.Candidates)
                if (candidate.Status == SaveCandidateStatus.Valid && candidate.Generation == snapshot.Generation) matched = true;
            if (!matched) throw new ArgumentException("SAVE_READ_NOT_OBSERVED");
            // The codec binds the selected payload hash. This read never resets the separately observed writer generation.
            return new SaveLoadResult<T>(SaveLoadStatus.Loaded, snapshot, null, observation: observation);
        }
        /// <summary>Reports a completely fileless profile without silently constructing a new save.</summary>
        public static SaveLoadResult<T> NeedsInitialization() => new SaveLoadResult<T>(SaveLoadStatus.NeedsInitialization, null, null);
        /// <summary>
        /// Reports an actual highest-generation hash conflict with a fully validated conservative main candidate.
        /// Profile, catalog and generation must match the observation; the codec must also bind the payload to its observed hash.
        /// The normal Snapshot remains null and Succeeded remains false, so no reward or writable authoritative state is inferred.
        /// </summary>
        public static SaveLoadResult<T> SplitBrain(SaveSnapshot<T> conservativeSnapshot, SaveRepositoryObservation observation)
        {
            if (conservativeSnapshot == null) throw new ArgumentNullException(nameof(conservativeSnapshot));
            if (observation == null) throw new ArgumentNullException(nameof(observation));
            var main = observation.Candidates[(int)SaveCandidateKind.Main];
            if (!conservativeSnapshot.Profile.Equals(observation.Profile) || !conservativeSnapshot.Catalog.Equals(observation.Catalog) ||
                main.Status != SaveCandidateStatus.Valid || main.Generation != conservativeSnapshot.Generation)
                throw new ArgumentException("INVALID_CONSERVATIVE_SAVE_CANDIDATE");
            bool conflict = false;
            foreach (var candidate in observation.Candidates)
            {
                if (candidate.Status != SaveCandidateStatus.Valid) continue;
                if (candidate.Generation > main.Generation) throw new ArgumentException("CONSERVATIVE_SAVE_CANDIDATE_NOT_HIGHEST");
                if (candidate.Generation == main.Generation && candidate.PayloadSha256 != main.PayloadSha256) conflict = true;
            }
            if (!conflict) throw new ArgumentException("SAVE_SPLIT_BRAIN_NOT_OBSERVED");
            return new SaveLoadResult<T>(SaveLoadStatus.SplitBrain, null, "SAVE_SPLIT_BRAIN", conservativeSnapshot, observation);
        }
        /// <summary>Creates one named failure; Loaded, NeedsInitialization and undefined values are rejected.</summary>
        public static SaveLoadResult<T> Failure(SaveLoadStatus status, string errorCode)
        {
            if (!Enum.IsDefined(typeof(SaveLoadStatus), status) || status == SaveLoadStatus.Loaded || status == SaveLoadStatus.NeedsInitialization)
                throw new ArgumentOutOfRangeException(nameof(status), "INVALID_LOAD_RESULT_STATE");
            return new SaveLoadResult<T>(status, null, ContractGuard.Error(errorCode));
        }
        /// <summary>Reports a failed or uncertain load while preserving complete read-only candidate provenance for explicit recovery.</summary>
        public static SaveLoadResult<T> Failure(SaveLoadStatus status, string errorCode, SaveRepositoryObservation observation)
        {
            if (observation == null) throw new ArgumentNullException(nameof(observation));
            var failure = Failure(status, errorCode);
            return new SaveLoadResult<T>(failure.Status, null, failure.ErrorCode, observation: observation);
        }
    }

    /// <summary>Validated commit union: only confirmed success carries the committed snapshot.</summary>
    public sealed class SaveCommitResult<T> where T : class, IImmutableSavePayload
    {
        private SaveCommitResult(SaveCommitStatus status, SaveSnapshot<T>? snapshot, string? errorCode)
        { Status = status; Snapshot = snapshot; ErrorCode = errorCode; }

        /// <summary>Closed commit outcome.</summary>
        public SaveCommitStatus Status { get; }
        /// <summary>True only after the authoritative committed snapshot has been re-read and validated.</summary>
        public bool Succeeded => Status == SaveCommitStatus.Committed;
        /// <summary>Authoritative committed snapshot; absent on every failed or uncertain result.</summary>
        public SaveSnapshot<T>? Snapshot { get; }
        /// <summary>Stable nonlocalized failure code; absent on success.</summary>
        public string? ErrorCode { get; }
        /// <summary>Creates a confirmed-success result; the adapter must have completed the full write/reread protocol.</summary>
        public static SaveCommitResult<T> Success(SaveSnapshot<T> snapshot) => new SaveCommitResult<T>(SaveCommitStatus.Committed,
            snapshot ?? throw new ArgumentNullException(nameof(snapshot)), null);
        /// <summary>Creates a non-success result without a success snapshot; Committed and undefined states are rejected.</summary>
        public static SaveCommitResult<T> Failure(SaveCommitStatus status, string errorCode)
        {
            if (!Enum.IsDefined(typeof(SaveCommitStatus), status) || status == SaveCommitStatus.Committed)
                throw new ArgumentOutOfRangeException(nameof(status), "INVALID_COMMIT_RESULT_STATE");
            return new SaveCommitResult<T>(status, null, ContractGuard.Error(errorCode));
        }
    }

    /// <summary>Typed provider-neutral repository for one bound profile/catalog; it never computes progress.</summary>
    public interface ISaveRepository<TSnapshot> : ISaveRepository where TSnapshot : class, IImmutableSavePayload
    {
        /// <summary>Loads all candidates without silent initialization or automatic destructive recovery.</summary>
        Task<SaveLoadResult<TSnapshot>> LoadAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Reobserves the exact bound profile/catalog and all candidate bytes, returns Conflict on any stale observation,
        /// then revalidates and reads the selected candidate without changing any candidate or generation.
        /// It never overwrites a newer profile, resets a writer generation or silently treats a selection as a committed save.
        /// Unsupported or unreadable candidates prevent recovery; cancellation has no storage effect on this read-only path.
        /// </summary>
        Task<SaveLoadResult<TSnapshot>> RecoverAsync(SaveRecoveryRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Serializes a CAS write to this repository's exact profile/catalog with snapshot.Generation == expectedGeneration + 1.
        /// Initialization uses expectedGeneration zero only after explicit user action and a verified fileless profile.
        /// Committed requires full main reread; Cancelled guarantees no possible storage effect, otherwise return RecoveryRequired.
        /// </summary>
        Task<SaveCommitResult<TSnapshot>> CommitAsync(SaveSnapshot<TSnapshot> snapshot, long expectedGeneration, CancellationToken cancellationToken);
    }

    /// <summary>Positive, explicitly supplied resource bounds for strict JSON parsing before DTO construction.</summary>
    public sealed class JsonResourceLimits
    {
        /// <summary>Requires a positive value for every independent resource limit; there are no implicit defaults.</summary>
        public JsonResourceLimits(int maxBytes, int maxDepth, int maxStringLength, int maxArrayLength, int maxObjectMembers)
        {
            if (maxBytes <= 0) throw new ArgumentOutOfRangeException(nameof(maxBytes));
            if (maxDepth <= 0) throw new ArgumentOutOfRangeException(nameof(maxDepth));
            if (maxStringLength <= 0) throw new ArgumentOutOfRangeException(nameof(maxStringLength));
            if (maxArrayLength <= 0) throw new ArgumentOutOfRangeException(nameof(maxArrayLength));
            if (maxObjectMembers <= 0) throw new ArgumentOutOfRangeException(nameof(maxObjectMembers));
            MaxBytes = maxBytes;
            MaxDepth = maxDepth;
            MaxStringLength = maxStringLength;
            MaxArrayLength = maxArrayLength;
            MaxObjectMembers = maxObjectMembers;
        }

        /// <summary>Maximum input byte count.</summary>
        public int MaxBytes { get; }
        /// <summary>Maximum strict-parser nesting depth.</summary>
        public int MaxDepth { get; }
        /// <summary>Maximum UTF-16 code units per decoded string, including object keys.</summary>
        public int MaxStringLength { get; }
        /// <summary>Maximum elements in an individual array.</summary>
        public int MaxArrayLength { get; }
        /// <summary>Maximum properties in an individual object.</summary>
        public int MaxObjectMembers { get; }
    }

    /// <summary>A strict canonicalization union with defensive byte ownership and stable failure codes.</summary>
    public sealed class CanonicalJsonResult
    {
        private readonly byte[]? utf8;
        private CanonicalJsonResult(byte[]? utf8, string? errorCode)
        { this.utf8 = utf8; ErrorCode = errorCode; }

        /// <summary>True exactly when copied canonical bytes exist and no error exists.</summary>
        public bool Succeeded => utf8 != null;
        /// <summary>Stable nonlocalized code on failure, otherwise null.</summary>
        public string? ErrorCode { get; }
        /// <summary>Returns a fresh copy of canonical bytes, or null on failure.</summary>
        public byte[]? GetUtf8() => utf8 == null ? null : (byte[])utf8.Clone();
        /// <summary>Copies nonempty adapter-verified canonical bytes; null and empty inputs are rejected.</summary>
        public static CanonicalJsonResult Success(byte[] utf8)
        {
            if (utf8 == null) throw new ArgumentNullException(nameof(utf8));
            if (utf8.Length == 0) throw new ArgumentException("EMPTY_CANONICAL_JSON", nameof(utf8));
            return new CanonicalJsonResult((byte[])utf8.Clone(), null);
        }
        /// <summary>Creates a failure without bytes; blank or non-code diagnostics are rejected.</summary>
        public static CanonicalJsonResult Failure(string errorCode) => new CanonicalJsonResult(null, ContractGuard.Error(errorCode));
    }

    /// <summary>Existing strict JCS semantics behind an injected byte port, without hashing or a second projection.</summary>
    public interface ICanonicalJson
    {
        /// <summary>Strictly parses bounded UTF-8 JSON and returns copied canonical bytes or a stable failure; never mutates input bytes.</summary>
        CanonicalJsonResult Canonicalize(byte[] utf8, JsonResourceLimits limits);
    }

    internal static class ContractGuard
    {
        private const long MaxSafeInteger = 9007199254740991L;

        internal static string Identifier(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 160 || value == "." || value == "..") throw new ArgumentException("INVALID_IDENTIFIER", parameterName);
            foreach (char c in value)
                if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '-' || c == '_' || c == '.' || c == ':'))
                    throw new ArgumentException("INVALID_IDENTIFIER", parameterName);
            return value;
        }

        internal static string Hash(string value, string parameterName)
        {
            if (value == null || value.Length != 64) throw new ArgumentException("INVALID_SHA256", parameterName);
            foreach (char c in value)
                if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f'))) throw new ArgumentException("INVALID_SHA256", parameterName);
            return value;
        }

        internal static void Use(CatalogUse value)
        {
            if (!Enum.IsDefined(typeof(CatalogUse), value)) throw new ArgumentOutOfRangeException(nameof(value), "INVALID_CATALOG_USE");
        }

        internal static void PositiveSafeInteger(long value, string parameterName)
        {
            if (value < 1 || value > MaxSafeInteger) throw new ArgumentOutOfRangeException(parameterName, "INVALID_POSITIVE_SAFE_INTEGER");
        }

        internal static void NonnegativeSafeInteger(long value, string parameterName)
        {
            if (value < 0 || value > MaxSafeInteger) throw new ArgumentOutOfRangeException(parameterName, "INVALID_NONNEGATIVE_SAFE_INTEGER");
        }

        internal static string Error(string errorCode)
        {
            if (string.IsNullOrEmpty(errorCode) || errorCode.Length > 100 || errorCode[0] < 'A' || errorCode[0] > 'Z')
                throw new ArgumentException("INVALID_ERROR_CODE", nameof(errorCode));
            foreach (char c in errorCode)
                if (!((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '_' || c == '-'))
                    throw new ArgumentException("INVALID_ERROR_CODE", nameof(errorCode));
            return errorCode;
        }

        internal static int CombineHash(string first, string second, string third)
            => unchecked(((StringComparer.Ordinal.GetHashCode(first) * 397) ^ StringComparer.Ordinal.GetHashCode(second)) * 397 ^ StringComparer.Ordinal.GetHashCode(third));
    }
}
