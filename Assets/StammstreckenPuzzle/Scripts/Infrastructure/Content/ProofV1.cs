using System;
using System.Collections.Generic;

namespace STP.Infrastructure.Content
{
    /// <summary>Stable proof rejection codes; no stored cache is a production proof by itself.</summary>
    public static class ProofDiagnosticCodes
    {
        public const string Missing = "PRF-MISSING";
        public const string Format = "PRF-FORMAT-UNKNOWN";
        public const string Profile = "PRF-HASH-PROFILE-UNKNOWN";
        public const string Hash = "PRF-HASH-MISMATCH";
        public const string Puzzle = "PRF-PUZZLE-MISMATCH";
        public const string Solution = "PRF-SOLUTION-MISMATCH";
        public const string Artifact = "PRF-ARTIFACT-ID-MISMATCH";
        public const string SolverUnknown = "PRF-SOLVER-UNKNOWN";
        public const string SolverVersion = "PRF-SOLVER-VERSION-MISMATCH";
        public const string NotUnique = "PRF-NOT-UNIQUE";
        public const string Indeterminate = "PRF-INDETERMINATE";
        public const string MetricsMissing = "PRF-METRICS-MISSING";
        public const string GuessDepth = "PRF-CAMPAIGN-GUESS-DEPTH";
        public const string Nondeterministic = "PRF-NONDETERMINISTIC";
    }

    /// <summary>The four nonnegative ADR-031 metrics, recorded exactly as delivered by the solver.</summary>
    public sealed class ProofMetrics
    {
        public long SearchNodes { get; }
        public long DeductionSteps { get; }
        public long MaxDeductionDepth { get; }
        public long RequiredGuessDepth { get; }
        internal ProofMetrics(long search, long steps, long depth, long guess)
        { SearchNodes = search; DeductionSteps = steps; MaxDeductionDepth = depth; RequiredGuessDepth = guess; }
        internal JsonValue ToJson() => LevelJson.Obj(("searchNodes", LevelJson.Int(SearchNodes)),
            ("deductionSteps", LevelJson.Int(DeductionSteps)), ("maxDeductionDepth", LevelJson.Int(MaxDeductionDepth)),
            ("requiredGuessDepth", LevelJson.Int(RequiredGuessDepth)));
    }

    /// <summary>Immutable closed proof-v1 DTO; construction goes through schema reading and self-validation.</summary>
    public sealed class ProofV1Document
    {
        public int ProofFormatVersion => 1;
        public string PuzzleId { get; }
        public ProfiledHash PublicPuzzleHash { get; }
        public ProfiledHash SolutionHash { get; }
        public string SolverVersion { get; }
        public int SolutionCount => 1;
        public ProofMetrics Metrics { get; }
        public ProfiledHash ProofHash { get; }
        internal ProofV1Document(string id, ProfiledHash puzzle, ProfiledHash solution, string solver, ProofMetrics metrics, ProfiledHash hash)
        { PuzzleId = id; PublicPuzzleHash = puzzle; SolutionHash = solution; SolverVersion = solver; Metrics = metrics; ProofHash = hash; }
        /// <summary>STP-PROOF-JCS-1: every proof field except proofHash.</summary>
        public JsonValue Projection() => LevelJson.Obj(("proofFormatVersion", LevelJson.Int(1)), ("puzzleId", LevelJson.Str(PuzzleId)),
            ("publicPuzzleHash", PublicPuzzleHash.ToJson()), ("solutionHash", SolutionHash.ToJson()),
            ("solverVersion", LevelJson.Str(SolverVersion)), ("solutionCount", LevelJson.Int(1)), ("metrics", Metrics.ToJson()));
        public JsonValue ToJson()
        {
            var members = new List<JsonMember>(Projection().Members) { new JsonMember("proofHash", ProofHash.ToJson()) };
            return JsonValue.CreateObject(members);
        }
    }

    /// <summary>A schema/hash-validated proof and canonical bytes, or immutable rejection diagnostics.</summary>
    public sealed class ProofReadResult
    {
        private readonly byte[]? _bytes;
        public ProofV1Document? Document { get; }
        /// <summary>A defensive copy; callers cannot mutate the validated artifact.</summary>
        public byte[]? CanonicalBytes => _bytes == null ? null : (byte[])_bytes.Clone();
        public IReadOnlyList<LevelDiagnostic> Diagnostics { get; }
        public bool Succeeded => Document != null && Diagnostics.Count == 0;
        internal ProofReadResult(ProofV1Document? document, byte[]? bytes, IReadOnlyList<LevelDiagnostic> diagnostics)
        { Document = document; _bytes = bytes == null ? null : (byte[])bytes.Clone(); Diagnostics = LevelDiagnosticList.Freeze(diagnostics); }
    }

    /// <summary>Strict parser, closed schema reader, exact profile validation and canonical serializer for proof-v1.</summary>
    internal static class ProofV1Core
    {
        private enum IdentitySpace { Campaign, Endless }
        internal static ProofReadResult ReadCampaign(byte[]? bytes)=>Read(bytes,IdentitySpace.Campaign);
        internal static ProofReadResult ReadCampaign(JsonValue? json)=>Read(json,IdentitySpace.Campaign);
        internal static ProofReadResult ReadEndless(byte[]? bytes)=>Read(bytes,IdentitySpace.Endless);
        internal static ProofReadResult ReadEndless(JsonValue? json)=>Read(json,IdentitySpace.Endless);
        internal static ProofReadResult CreateCampaign(string id,ProfiledHash puzzle,ProfiledHash solution,string solver,long search,long steps,long depth,long guess)=>Create(id,puzzle,solution,solver,search,steps,depth,guess,IdentitySpace.Campaign);
        internal static ProofReadResult CreateEndless(string id,ProfiledHash puzzle,ProfiledHash solution,string solver,long search,long steps,long depth,long guess)=>Create(id,puzzle,solution,solver,search,steps,depth,guess,IdentitySpace.Endless);
        private static bool IsEndlessId(string value)
        {
            if(value.Length!=67||!value.StartsWith("E1-",StringComparison.Ordinal))return false;
            for(var i=3;i<value.Length;i++)if(!(value[i]>='0'&&value[i]<='9'||value[i]>='a'&&value[i]<='f'))return false;
            return true;
        }
        internal static LevelDiagnostic Error(string code, string path, string message) => new LevelDiagnostic(LevelDiagnosticStage.Proof, code, path, message);
        internal static ProofReadResult Fail(LevelDiagnostic error) => new ProofReadResult(null, null, new[] { error });
        private static LevelDiagnostic Translate(LevelDiagnostic error) => Error(error.Code.Replace("LVL-", "PRF-"), error.Path, error.Message);

        private static ProofReadResult Read(byte[]? bytes,IdentitySpace space)
        {
            if (bytes == null) return Fail(Error(ProofDiagnosticCodes.Missing, string.Empty, "The proof artifact is missing."));
            var parsed = StrictJsonParser.Parse(bytes);
            return parsed.Value == null ? Fail(Translate(parsed.Error!)) : Read(parsed.Value,space);
        }

        /// <summary>Historical solver-v1 is readable; production binding accepts only solver-v2.</summary>
        private static ProofReadResult Read(JsonValue? json,IdentitySpace space)
        {
            var ctx = new SchemaContext();
            if (!ctx.BeginObject(json, string.Empty, "proofFormatVersion", "puzzleId", "publicPuzzleHash", "solutionHash", "solverVersion", "solutionCount", "metrics", "proofHash"))
                return SchemaFailure(ctx);
            ctx.ConstInt(json!, string.Empty, "proofFormatVersion", 1, ProofDiagnosticCodes.Format);
            string? id = ctx.Pattern(json!, string.Empty, "puzzleId", space==IdentitySpace.Campaign?(Func<string,bool>)LevelPatterns.IsPuzzleId:IsEndlessId);
            var puzzle = LevelV2Reader.ReadProfiledHash(ctx, json!, string.Empty, "publicPuzzleHash");
            var solution = LevelV2Reader.ReadProfiledHash(ctx, json!, string.Empty, "solutionHash");
            string? solver = ctx.Pattern(json!, string.Empty, "solverVersion", LevelPatterns.IsSolverVersion);
            ctx.ConstInt(json!, string.Empty, "solutionCount", 1, ProofDiagnosticCodes.NotUnique);
            var metrics = json!.Get("metrics");
            long? search = null, steps = null, depth = null, guess = null;
            if (metrics != null && ctx.BeginObject(metrics, "/metrics", "searchNodes", "deductionSteps", "maxDeductionDepth", "requiredGuessDepth"))
            {
                search = ctx.Int(metrics, "/metrics", "searchNodes", 0, JsonValue.MaxSafeInteger);
                steps = ctx.Int(metrics, "/metrics", "deductionSteps", 0, JsonValue.MaxSafeInteger);
                depth = ctx.Int(metrics, "/metrics", "maxDeductionDepth", 0, JsonValue.MaxSafeInteger);
                guess = ctx.Int(metrics, "/metrics", "requiredGuessDepth", 0, JsonValue.MaxSafeInteger);
            }
            var hash = LevelV2Reader.ReadProfiledHash(ctx, json!, string.Empty, "proofHash");
            if (ctx.HasErrors) return SchemaFailure(ctx);
            var doc = new ProofV1Document(id!, puzzle!, solution!, solver!, new ProofMetrics(search!.Value, steps!.Value, depth!.Value, guess!.Value), hash!);
            foreach (var slot in new[] { (doc.PublicPuzzleHash, LevelHashProfiles.PuzzleSemantic, "/publicPuzzleHash"),
                (doc.SolutionHash, LevelHashProfiles.Solution, "/solutionHash"), (doc.ProofHash, LevelHashProfiles.Proof, "/proofHash") })
                if (slot.Item1.Profile != slot.Item2) return Fail(Error(ProofDiagnosticCodes.Profile, slot.Item3 + "/profile", "Unknown or wrong-slot hash profile."));
            if (solver != "solver-v1" && solver != "solver-v2") return Fail(Error(ProofDiagnosticCodes.SolverUnknown, "/solverVersion", "No reader contract for this solver version."));
            var projection = LevelHashing.Compute(LevelHashProfiles.Proof, doc.Projection());
            if (!projection.Succeeded) return Fail(Translate(projection.Error!));
            if (!doc.ProofHash.Equals(projection.Hash)) return Fail(Error(ProofDiagnosticCodes.Hash, "/proofHash", "Proof self-hash does not match the exact projection."));
            var canonical = JcsSerializer.Serialize(doc.ToJson());
            return canonical.Utf8 == null ? Fail(Translate(canonical.Error!)) : new ProofReadResult(doc, canonical.Utf8, Array.Empty<LevelDiagnostic>());
        }

        private static ProofReadResult SchemaFailure(SchemaContext ctx)
        {
            var errors = new List<LevelDiagnostic>();
            foreach (var error in ctx.Diagnostics) errors.Add(Translate(error));
            return new ProofReadResult(null, null, errors);
        }

        /// <summary>Creates a proof projection from production values, then validates its full serialized artifact before returning it.</summary>
        private static ProofReadResult Create(string puzzleId, ProfiledHash puzzleHash, ProfiledHash solutionHash, string solverVersion,
            long searchNodes, long deductionSteps, long maxDeductionDepth, long requiredGuessDepth,IdentitySpace space)
        {
            var doc = new ProofV1Document(puzzleId, puzzleHash, solutionHash, solverVersion,
                new ProofMetrics(searchNodes, deductionSteps, maxDeductionDepth, requiredGuessDepth), new ProfiledHash(LevelHashProfiles.Proof, new string('0', 64)));
            var hash = LevelHashing.Compute(LevelHashProfiles.Proof, doc.Projection());
            if (!hash.Succeeded) return Fail(Translate(hash.Error!));
            doc = new ProofV1Document(puzzleId, puzzleHash, solutionHash, solverVersion, doc.Metrics, hash.Hash!);
            var canonical = JcsSerializer.Serialize(doc.ToJson());
            return canonical.Utf8 == null ? Fail(Translate(canonical.Error!)) : Read(canonical.Utf8,space);
        }

    }

    /// <summary>Campaign-only proof-v1 entry. E1 identities remain rejected by this reader and its unchanged schema.</summary>
    public static class ProofV1Codec
    {
        private static LevelDiagnostic Error(string code,string path,string message)=>ProofV1Core.Error(code,path,message);
        public static ProofReadResult Read(byte[]? bytes)=>ProofV1Core.ReadCampaign(bytes);
        public static ProofReadResult Read(JsonValue? json)=>ProofV1Core.ReadCampaign(json);
        public static ProofReadResult Create(string puzzleId,ProfiledHash puzzleHash,ProfiledHash solutionHash,string solverVersion,
            long searchNodes,long deductionSteps,long maxDeductionDepth,long requiredGuessDepth)
            =>ProofV1Core.CreateCampaign(puzzleId,puzzleHash,solutionHash,solverVersion,searchNodes,deductionSteps,maxDeductionDepth,requiredGuessDepth);

        /// <summary>The artifact ID is a logical reference, never a filesystem path to open.</summary>
        public static string ArtifactId(string puzzleId, string solverVersion) => "proofs/" + puzzleId + "/" + solverVersion + ".proof-v1.json";

        /// <summary>Full level/reference/artifact binding, after the artifact has passed schema and self-hash validation.</summary>
        public static IReadOnlyList<LevelDiagnostic> Bind(LevelV2LoadResult level, string artifactId, ProofReadResult proof)
        {
            if (!proof.Succeeded) return proof.Diagnostics;
            if (!level.IsValid) return level.Diagnostics;
            var errors = new List<LevelDiagnostic>();
            var doc = proof.Document!; var reference = level.Document!.ProofRef;
            void Add(string code, string path, string message) => errors.Add(Error(code, path, message));
            if (doc.SolverVersion != "solver-v2") Add(ProofDiagnosticCodes.SolverVersion, "/solverVersion", "Production requires solver-v2.");
            if (artifactId != reference.ArtifactId || artifactId != ArtifactId(level.Document.PuzzleId, doc.SolverVersion))
                Add(ProofDiagnosticCodes.Artifact, "/proofRef/artifactId", "Artifact ID does not bind this puzzle and solver version.");
            if (reference.ProofFormatVersion != doc.ProofFormatVersion) Add(ProofDiagnosticCodes.Format, "/proofRef/proofFormatVersion", "Proof format differs.");
            if (doc.PuzzleId != level.Document.PuzzleId || !doc.PublicPuzzleHash.Equals(level.PublicPuzzleHash))
                Add(ProofDiagnosticCodes.Puzzle, "/publicPuzzleHash", "Proof belongs to a different public puzzle.");
            if (!doc.SolutionHash.Equals(level.SolutionHash)) Add(ProofDiagnosticCodes.Solution, "/solutionHash", "Proof solution is stale or belongs to another level.");
            if (reference.ProofHash.Profile != LevelHashProfiles.Proof) Add(ProofDiagnosticCodes.Profile, "/proofRef/proofHash/profile", "Reference has the wrong hash profile.");
            else if (!reference.ProofHash.Equals(doc.ProofHash)) Add(ProofDiagnosticCodes.Hash, "/proofRef/proofHash", "Reference does not identify the supplied proof.");
            return LevelDiagnosticList.Freeze(errors);
        }
    }
}
