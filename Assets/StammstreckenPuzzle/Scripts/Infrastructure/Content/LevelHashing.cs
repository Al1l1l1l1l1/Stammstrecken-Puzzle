using System;
using System.Security.Cryptography;
using System.Text;

namespace STP.Infrastructure.Content
{
    /// <summary>Outcome of a profile-dispatched hash computation.</summary>
    public sealed class ProfiledHashResult
    {
        public ProfiledHash? Hash { get; }
        public LevelDiagnostic? Error { get; }
        public bool Succeeded => Hash != null;
        private ProfiledHashResult(ProfiledHash? hash, LevelDiagnostic? error) { Hash = hash; Error = error; }
        internal static ProfiledHashResult Ok(ProfiledHash hash) => new ProfiledHashResult(hash, null);
        internal static ProfiledHashResult Fail(LevelDiagnostic error) => new ProfiledHashResult(null, error);
    }

    /// <summary>
    /// The hashes of one Level-v2 document or exactly one hash-stage diagnostic (never both). Internal: the public
    /// pipeline results expose the individual values.
    /// </summary>
    internal sealed class LevelDocumentHashes
    {
        internal string? ContentHash { get; }
        internal ProfiledHash? PublicPuzzleHash { get; }
        internal ProfiledHash? SolutionHash { get; }
        internal LevelDiagnostic? Error { get; }

        private LevelDocumentHashes(string? content, ProfiledHash? publicPuzzle, ProfiledHash? solution, LevelDiagnostic? error)
        {
            ContentHash = content; PublicPuzzleHash = publicPuzzle; SolutionHash = solution; Error = error;
        }

        internal static LevelDocumentHashes Ok(string content, ProfiledHash publicPuzzle, ProfiledHash solution) => new LevelDocumentHashes(content, publicPuzzle, solution, null);
        internal static LevelDocumentHashes Fail(LevelDiagnostic error) => new LevelDocumentHashes(null, null, null, error);
    }

    /// <summary>
    /// The binding hash contracts (LEVEL_DATA_FORMAT.md Abschnitt 6, ADR-021). Every hash is
    /// <c>SHA-256(JCS(projection))</c>; the projections are strictly separated:
    /// <list type="bullet">
    /// <item><description><b>Content-Hash</b> (<c>documentSha256</c>): the complete Level document, bare lowercase hex, no profile.</description></item>
    /// <item><description><b>Public-Puzzle-Hash</b> (<c>STP-PUZZLE-SEMANTIC-JCS-1</c>): <c>{puzzleId, rulesetVersion, grid, endpoints, rowCounts, columnCounts}</c>.</description></item>
    /// <item><description><b>Solution-Hash</b> (<c>STP-SOLUTION-JCS-1</c>): <c>{puzzleId, publicPuzzleHash, path}</c>.</description></item>
    /// <item><description><b>Proof-Hash</b> (<c>STP-PROOF-JCS-1</c>): a complete proof-v1 object without <c>proofHash</c>; only the projection hash is provided, no proof artifact is created here.</description></item>
    /// <item><description><b>Legacy v1</b> (<c>STP-LEVEL-V1-PUZZLE-JCS-1</c> and the two unprofiled v1 hashes): the unchanged historical v1 projections.</description></item>
    /// </list>
    /// Formatting, key order, <c>documentSchemaVersion</c>, <c>contentRevision</c>, texts, assets and proof regeneration do not
    /// change the Public-Puzzle-Hash; a change of puzzle id, ruleset, grid, endpoints or counts does.
    /// </summary>
    public static class LevelHashing
    {
        public static string Sha256Hex(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            using (var sha = SHA256.Create())
            {
                byte[] digest = sha.ComputeHash(bytes);
                var builder = new StringBuilder(digest.Length * 2);
                foreach (byte value in digest) builder.Append(HexDigit(value >> 4)).Append(HexDigit(value & 0xF));
                return builder.ToString();
            }
        }

        private static char HexDigit(int value) => (char)(value < 10 ? '0' + value : 'a' + (value - 10));

        /// <summary>
        /// SHA-256 over the JCS bytes of an arbitrary projection (lowercase hex).
        /// Throws <see cref="InvalidOperationException"/> when the projection cannot be canonicalised (for example an unpaired
        /// surrogate, <c>LVL-JCS-SURROGATE</c>): use it for projections that are known to be valid, and use
        /// <see cref="Compute"/> or <see cref="JcsSerializer.Serialize"/> for any projection that may come from untrusted input.
        /// </summary>
        public static string HashProjection(JsonValue projection) => Sha256Hex(JcsSerializer.SerializeOrThrow(projection));

        /// <summary>Profile registry: unknown profile names are a hard error; the projection must already match the profile.</summary>
        public static ProfiledHashResult Compute(string? profile, JsonValue? projection)
        {
            if (!LevelHashProfiles.IsKnown(profile))
                return ProfiledHashResult.Fail(new LevelDiagnostic(LevelDiagnosticStage.Hash, LevelDiagnosticCodes.HashProfileUnknown, string.Empty, "Unknown hash profile '" + (profile ?? "<null>") + "'."));
            var canonical = JcsSerializer.Serialize(projection);
            if (canonical.Utf8 == null) return ProfiledHashResult.Fail(canonical.Error!);
            return ProfiledHashResult.Ok(new ProfiledHash(profile!, Sha256Hex(canonical.Utf8)));
        }

        // ---- Content-Hash --------------------------------------------------------------------------------------

        /// <summary>
        /// Content-Hash of a complete document: SHA-256 of its JCS bytes. Independent of formatting and member order.
        /// Throws <see cref="InvalidOperationException"/> when the document cannot be canonicalised (see <see cref="HashProjection"/>);
        /// the pipeline results (<see cref="LevelV2Loader"/>, <see cref="LevelV1ToV2Migrator"/>) report that case as a diagnostic instead.
        /// </summary>
        public static string ComputeContentHash(JsonValue document) => HashProjection(document ?? throw new ArgumentNullException(nameof(document)));

        public static string ComputeContentHash(LevelV2Document document) => ComputeContentHash((document ?? throw new ArgumentNullException(nameof(document))).ToJson());

        // ---- Public-Puzzle-Hash --------------------------------------------------------------------------------

        public static JsonValue PublicPuzzleProjection(LevelV2Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            return PublicPuzzleProjection(document.PuzzleId, document.RulesetVersion, document.Grid, document.EndpointA, document.EndpointB, document.RowCounts, document.ColumnCounts);
        }

        internal static JsonValue PublicPuzzleProjection(string puzzleId, string rulesetVersion, LevelGridDto grid, LevelEndpointDto a, LevelEndpointDto b,
            System.Collections.Generic.IReadOnlyList<int> rowCounts, System.Collections.Generic.IReadOnlyList<int> columnCounts) =>
            LevelJson.Obj(
                ("puzzleId", LevelJson.Str(puzzleId)),
                ("rulesetVersion", LevelJson.Str(rulesetVersion)),
                ("grid", grid.ToJson()),
                ("endpoints", LevelJson.Obj(("a", a.ToJson()), ("b", b.ToJson()))),
                ("rowCounts", LevelJson.IntArray(rowCounts)),
                ("columnCounts", LevelJson.IntArray(columnCounts)));

        /// <summary>Throws <see cref="InvalidOperationException"/> when the projection cannot be canonicalised (see <see cref="HashProjection"/>).</summary>
        public static ProfiledHash ComputePublicPuzzleHash(LevelV2Document document) =>
            new ProfiledHash(LevelHashProfiles.PuzzleSemantic, HashProjection(PublicPuzzleProjection(document)));

        // ---- Solution-Hash -------------------------------------------------------------------------------------

        public static JsonValue SolutionProjection(string puzzleId, ProfiledHash publicPuzzleHash, System.Collections.Generic.IReadOnlyList<LevelPathCellDto> path)
        {
            if (puzzleId == null) throw new ArgumentNullException(nameof(puzzleId));
            if (publicPuzzleHash == null) throw new ArgumentNullException(nameof(publicPuzzleHash));
            if (path == null) throw new ArgumentNullException(nameof(path));
            return LevelJson.Obj(
                ("puzzleId", LevelJson.Str(puzzleId)),
                ("publicPuzzleHash", publicPuzzleHash.ToJson()),
                ("path", LevelPathCellDto.PathToJson(path)));
        }

        /// <summary>Throws <see cref="InvalidOperationException"/> when the projection cannot be canonicalised (see <see cref="HashProjection"/>).</summary>
        public static ProfiledHash ComputeSolutionHash(LevelV2Document document) =>
            new ProfiledHash(LevelHashProfiles.Solution, HashProjection(SolutionProjection(document.PuzzleId, ComputePublicPuzzleHash(document), document.SolutionPath)));

        // ---- All hashes of a document, without exceptions -----------------------------------------------------

        /// <summary>
        /// Computes the Content-Hash, the Public-Puzzle-Hash and the Solution-Hash of a schema-valid document through the
        /// non-throwing canonicalisation. A document the schema stage accepted can still hold a .NET string that is not
        /// canonicalisable (a DOM built programmatically may carry unpaired surrogates in free text); that is reported as
        /// the single hash-stage diagnostic of the canonicaliser (<c>LVL-JCS-SURROGATE</c>), never as an exception.
        /// </summary>
        internal static LevelDocumentHashes TryComputeDocumentHashes(LevelV2Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            var content = JcsSerializer.Serialize(document.ToJson());
            if (content.Utf8 == null) return LevelDocumentHashes.Fail(content.Error!);
            var publicPuzzle = Compute(LevelHashProfiles.PuzzleSemantic, PublicPuzzleProjection(document));
            if (publicPuzzle.Hash == null) return LevelDocumentHashes.Fail(publicPuzzle.Error!);
            var solution = Compute(LevelHashProfiles.Solution, SolutionProjection(document.PuzzleId, publicPuzzle.Hash, document.SolutionPath));
            if (solution.Hash == null) return LevelDocumentHashes.Fail(solution.Error!);
            return LevelDocumentHashes.Ok(Sha256Hex(content.Utf8), publicPuzzle.Hash, solution.Hash);
        }

        // ---- Proof-Hash (projection only) ----------------------------------------------------------------------

        /// <summary>
        /// Hash of a proof-v1 object <b>without</b> its <c>proofHash</c> member. This computes the identity of a given projection;
        /// it neither generates a proof nor binds a proof artifact (solver/proof regeneration is a later work package).
        /// </summary>
        /// <remarks>Throws <see cref="InvalidOperationException"/> when the projection cannot be canonicalised (see <see cref="HashProjection"/>).</remarks>
        public static ProfiledHash ComputeProofHash(JsonValue proofWithoutProofHash)
        {
            if (proofWithoutProofHash == null) throw new ArgumentNullException(nameof(proofWithoutProofHash));
            return new ProfiledHash(LevelHashProfiles.Proof, HashProjection(proofWithoutProofHash));
        }

        // ---- Verification of externally stored hashes ----------------------------------------------------------

        /// <summary>
        /// Checks a stored <c>{profile, sha256}</c> against the expected profile and value. An unknown or wrong-slot profile
        /// is <c>LVL-HASH-PROFILE-UNKNOWN</c>; a different value is <c>LVL-HASH-MISMATCH</c>. Values are never interpreted heuristically.
        /// </summary>
        public static LevelDiagnostic? Verify(ProfiledHash? stored, ProfiledHash expected, string path)
        {
            if (stored == null) return new LevelDiagnostic(LevelDiagnosticStage.Hash, LevelDiagnosticCodes.HashMismatch, path, "No hash is stored.");
            if (!LevelHashProfiles.IsKnown(stored.Profile) || !string.Equals(stored.Profile, expected.Profile, StringComparison.Ordinal))
                return new LevelDiagnostic(LevelDiagnosticStage.Hash, LevelDiagnosticCodes.HashProfileUnknown, path, "Hash profile '" + stored.Profile + "' is unknown or not valid for this slot (expected " + expected.Profile + ").");
            if (!string.Equals(stored.Sha256, expected.Sha256, StringComparison.Ordinal))
                return new LevelDiagnostic(LevelDiagnosticStage.Hash, LevelDiagnosticCodes.HashMismatch, path, "The stored hash differs from the recomputed projection hash.");
            return null;
        }

        // ---- Legacy Level-v1 -----------------------------------------------------------------------------------

        public static JsonValue LegacyV1PuzzleProjection(LevelV1Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            return LevelJson.Obj(
                ("schemaVersion", LevelJson.Int(document.SchemaVersion)),
                ("rulesetVersion", LevelJson.Str(document.RulesetVersion)),
                ("id", LevelJson.Str(document.Id)),
                ("grid", document.Grid.ToJson()),
                ("endpoints", LevelJson.Obj(("a", document.EndpointA.ToJson()), ("b", document.EndpointB.ToJson()))),
                ("rowCounts", LevelJson.IntArray(document.RowCounts)),
                ("columnCounts", LevelJson.IntArray(document.ColumnCounts)));
        }

        /// <summary><c>STP-LEVEL-V1-PUZZLE-JCS-1</c>: the unchanged historical v1 puzzle projection.</summary>
        public static ProfiledHash ComputeLegacyV1PuzzleHash(LevelV1Document document) =>
            new ProfiledHash(LevelHashProfiles.LevelV1Puzzle, HashProjection(LegacyV1PuzzleProjection(document)));

        /// <summary>Legacy v1 <c>solutionHashSha256</c>: SHA-256(JCS({id, path})).</summary>
        public static string ComputeLegacyV1SolutionHashHex(LevelV1Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            return HashProjection(LevelJson.Obj(("id", LevelJson.Str(document.Id)), ("path", LevelPathCellDto.PathToJson(document.SolutionPath))));
        }

        /// <summary>Legacy v1 <c>proof.proofHashSha256</c>: SHA-256(JCS({solverVersion, solutionCount, searchNodes, deductionSteps, maxDeductionDepth, requiredGuessDepth})).</summary>
        public static string ComputeLegacyV1ProofHashHex(LevelV1Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            var validation = document.Validation;
            return HashProjection(LevelJson.Obj(
                ("solverVersion", LevelJson.Str(validation.SolverVersion)),
                ("solutionCount", LevelJson.Int(validation.SolutionCount)),
                ("searchNodes", LevelJson.Int(validation.SearchNodes)),
                ("deductionSteps", LevelJson.Int(validation.DeductionSteps)),
                ("maxDeductionDepth", LevelJson.Int(validation.MaxDeductionDepth)),
                ("requiredGuessDepth", LevelJson.Int(validation.RequiredGuessDepth))));
        }
    }
}
