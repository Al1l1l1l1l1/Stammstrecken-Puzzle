using System;
using System.Collections.Generic;

namespace STP.Infrastructure.Content
{
    /// <summary>Outcome of a migration (or a pass-through of an already current document).</summary>
    public sealed class LevelMigrationResult
    {
        public LevelV2Document? Document { get; }
        public JsonValue? Json { get; }
        /// <summary>Content-Hash (<c>documentSha256</c>) of the resulting Level-v2 document.</summary>
        public string? ContentHash { get; }
        public ProfiledHash? PublicPuzzleHash { get; }
        public ProfiledHash? SolutionHash { get; }
        /// <summary>False when the input already was a valid Level-v2 document (idempotent pass-through).</summary>
        public bool WasMigrated { get; }
        public IReadOnlyList<LevelDiagnostic> Diagnostics { get; }
        public bool Succeeded => Document != null && Diagnostics.Count == 0;

        internal LevelMigrationResult(LevelV2Document? document, bool wasMigrated, IReadOnlyList<LevelDiagnostic> diagnostics)
        {
            Document = document; WasMigrated = wasMigrated; Diagnostics = diagnostics;
            if (document != null && diagnostics.Count == 0)
            {
                Json = document.ToJson();
                ContentHash = LevelHashing.ComputeContentHash(Json);
                PublicPuzzleHash = LevelHashing.ComputePublicPuzzleHash(document);
                SolutionHash = LevelHashing.ComputeSolutionHash(document);
            }
        }
    }

    /// <summary>
    /// Level-v1 to Level-v2 migration as far as the binding contract (LEVEL_DATA_FORMAT.md Abschnitt 9) provides: it works on
    /// immutable inputs (so always on a copy), is deterministic and idempotent, maps <c>id</c> to <c>puzzleId</c>, keeps all
    /// public inputs, solution, texts, completion and production fields unchanged, does not raise <c>contentRevision</c>, and
    /// derives <c>proofRef</c> only from the recorded v1 facts. The v1 hashes are verified first. Anything that cannot be
    /// carried over neutrally and schema-valid yields <c>LVL_MIGRATION_NEEDS_EDITORIAL_DECISION</c>; no value is invented.
    /// No solver runs and no proof artifact is generated.
    /// </summary>
    public static class LevelV1ToV2Migrator
    {
        public static LevelMigrationResult Migrate(LevelV1Document? source)
        {
            if (source == null) return Fail(new LevelDiagnostic(LevelDiagnosticStage.Migration, LevelDiagnosticCodes.SchemaType, string.Empty, "No Level-v1 document."));

            var hashErrors = VerifyLegacyHashes(source);
            if (hashErrors.Count > 0) return new LevelMigrationResult(null, false, hashErrors);

            var proofErrors = CheckProofMetrics(source);
            if (proofErrors.Count > 0) return new LevelMigrationResult(null, false, proofErrors);

            var json = BuildV2(source);
            var read = LevelV2Reader.Read(json);
            if (read.Document == null)
            {
                var editorial = new List<LevelDiagnostic>();
                foreach (var diagnostic in read.Diagnostics) editorial.Add(Editorial(diagnostic.Path, "The v1 value cannot be carried over neutrally into a valid Level-v2 document (" + diagnostic.Code + ": " + diagnostic.Message + ")"));
                return new LevelMigrationResult(null, false, editorial);
            }

            var mapped = LevelV2DomainMapper.Map(read.Document);
            if (mapped.Definition == null) return new LevelMigrationResult(null, false, mapped.Diagnostics);
            var semantic = LevelV2Semantics.Validate(read.Document, mapped.Definition);
            if (semantic.Count > 0) return new LevelMigrationResult(null, false, semantic);
            return new LevelMigrationResult(read.Document, true, new LevelDiagnostic[0]);
        }

        public static LevelMigrationResult Migrate(JsonValue? json)
        {
            var read = LevelV1Reader.Read(json);
            return read.Document == null ? new LevelMigrationResult(null, false, read.Diagnostics) : Migrate(read.Document);
        }

        public static LevelMigrationResult Migrate(byte[]? utf8)
        {
            var parsed = StrictJsonParser.Parse(utf8);
            return parsed.Value == null ? Fail(parsed.Error!) : Migrate(parsed.Value);
        }

        /// <summary>
        /// Brings any supported document to the current Level-v2 form: <c>schemaVersion 1</c> is migrated, <c>documentSchemaVersion 2</c>
        /// is validated and passed through unchanged (idempotence), everything else is <c>LVL-VERSION-UNKNOWN</c>.
        /// </summary>
        public static LevelMigrationResult MigrateToCurrent(JsonValue? json)
        {
            if (json == null || json.Kind != JsonKind.Object)
                return Fail(new LevelDiagnostic(LevelDiagnosticStage.Schema, LevelDiagnosticCodes.SchemaType, string.Empty, "Expected an object."));
            var legacy = json.Get("schemaVersion");
            var current = json.Get("documentSchemaVersion");
            if (legacy != null && current != null)
                return Fail(new LevelDiagnostic(LevelDiagnosticStage.Schema, LevelDiagnosticCodes.VersionUnknown, string.Empty, "A document cannot carry both schemaVersion and documentSchemaVersion."));
            if (legacy != null && legacy.Kind == JsonKind.Integer && legacy.IntegerValue == LevelV2Contract.LegacySchemaVersion) return Migrate(json);
            if (current != null && current.Kind == JsonKind.Integer && current.IntegerValue == LevelV2Contract.DocumentSchemaVersion)
            {
                var load = LevelV2Loader.Load(json);
                return new LevelMigrationResult(load.IsValid ? load.Document : null, false, load.Diagnostics);
            }
            return Fail(new LevelDiagnostic(LevelDiagnosticStage.Schema, LevelDiagnosticCodes.VersionUnknown, legacy != null ? "/schemaVersion" : current != null ? "/documentSchemaVersion" : string.Empty,
                "The document version is missing or not supported (supported: schemaVersion 1, documentSchemaVersion 2)."));
        }

        public static LevelMigrationResult MigrateToCurrent(byte[]? utf8)
        {
            var parsed = StrictJsonParser.Parse(utf8);
            return parsed.Value == null ? Fail(parsed.Error!) : MigrateToCurrent(parsed.Value);
        }

        private static IReadOnlyList<LevelDiagnostic> VerifyLegacyHashes(LevelV1Document source)
        {
            var errors = new List<LevelDiagnostic>();
            var validation = source.Validation;
            if (!string.Equals(LevelHashing.ComputeLegacyV1PuzzleHash(source).Sha256, validation.PuzzleHashSha256, StringComparison.Ordinal))
                errors.Add(new LevelDiagnostic(LevelDiagnosticStage.Hash, LevelDiagnosticCodes.HashMismatch, "/validation/puzzleHashSha256", "The recorded " + LevelHashProfiles.LevelV1Puzzle + " hash differs from the recomputed legacy projection."));
            if (!string.Equals(LevelHashing.ComputeLegacyV1SolutionHashHex(source), validation.SolutionHashSha256, StringComparison.Ordinal))
                errors.Add(new LevelDiagnostic(LevelDiagnosticStage.Hash, LevelDiagnosticCodes.HashMismatch, "/validation/solutionHashSha256", "The recorded legacy solution hash differs from the recomputed projection."));
            if (!string.Equals(LevelHashing.ComputeLegacyV1ProofHashHex(source), validation.ProofHashSha256, StringComparison.Ordinal))
                errors.Add(new LevelDiagnostic(LevelDiagnosticStage.Hash, LevelDiagnosticCodes.HashMismatch, "/validation/proof/proofHashSha256", "The recorded legacy proof hash differs from the recomputed projection."));
            return errors;
        }

        /// <summary>The recorded v1 metrics must fit the closed proof-v1 minimums, otherwise no neutral proof projection exists.</summary>
        private static IReadOnlyList<LevelDiagnostic> CheckProofMetrics(LevelV1Document source)
        {
            var errors = new List<LevelDiagnostic>();
            var v = source.Validation;
            if (v.SearchNodes < 1) errors.Add(Editorial("/validation/proof/searchNodes", "proof-v1 requires searchNodes >= 1"));
            if (v.DeductionSteps < 1) errors.Add(Editorial("/validation/proof/deductionSteps", "proof-v1 requires deductionSteps >= 1"));
            if (v.MaxDeductionDepth < 1) errors.Add(Editorial("/validation/proof/maxDeductionDepth", "proof-v1 requires maxDeductionDepth >= 1"));
            return errors;
        }

        private static JsonValue BuildV2(LevelV1Document source)
        {
            var publicHash = new ProfiledHash(LevelHashProfiles.PuzzleSemantic, LevelHashing.HashProjection(
                LevelHashing.PublicPuzzleProjection(source.Id, source.RulesetVersion, source.Grid, source.EndpointA, source.EndpointB, source.RowCounts, source.ColumnCounts)));
            var solutionHash = new ProfiledHash(LevelHashProfiles.Solution, LevelHashing.HashProjection(LevelHashing.SolutionProjection(source.Id, publicHash, source.SolutionPath)));
            var validation = source.Validation;
            var proofProjection = LevelJson.Obj(
                ("proofFormatVersion", LevelJson.Int(LevelV2Contract.ProofFormatVersion)),
                ("puzzleId", LevelJson.Str(source.Id)),
                ("publicPuzzleHash", publicHash.ToJson()),
                ("solutionHash", solutionHash.ToJson()),
                ("solverVersion", LevelJson.Str(validation.SolverVersion)),
                ("solutionCount", LevelJson.Int(validation.SolutionCount)),
                ("metrics", LevelJson.Obj(
                    ("searchNodes", LevelJson.Int(validation.SearchNodes)),
                    ("deductionSteps", LevelJson.Int(validation.DeductionSteps)),
                    ("maxDeductionDepth", LevelJson.Int(validation.MaxDeductionDepth)),
                    ("requiredGuessDepth", LevelJson.Int(validation.RequiredGuessDepth)))));
            var proofHash = LevelHashing.ComputeProofHash(proofProjection);
            return LevelJson.Obj(
                ("documentSchemaVersion", LevelJson.Int(LevelV2Contract.DocumentSchemaVersion)),
                ("rulesetVersion", LevelJson.Str(source.RulesetVersion)),
                ("puzzleId", LevelJson.Str(source.Id)),
                ("contentRevision", LevelJson.Int(source.ContentRevision)),
                ("grid", source.Grid.ToJson()),
                ("endpoints", LevelJson.Obj(("a", source.EndpointA.ToJson()), ("b", source.EndpointB.ToJson()))),
                ("rowCounts", LevelJson.IntArray(source.RowCounts)),
                ("columnCounts", LevelJson.IntArray(source.ColumnCounts)),
                ("content", source.Content.ToJson()),
                ("production", source.Production.ToJson()),
                ("completion", source.Completion.ToJson()),
                ("solution", LevelJson.Obj(("path", LevelPathCellDto.PathToJson(source.SolutionPath)))),
                ("proofRef", LevelJson.Obj(
                    ("artifactId", LevelJson.Str("proofs/" + source.Id + "/" + validation.SolverVersion + ".proof-v1.json")),
                    ("proofFormatVersion", LevelJson.Int(LevelV2Contract.ProofFormatVersion)),
                    ("proofHash", proofHash.ToJson()))));
        }

        private static LevelDiagnostic Editorial(string path, string message) =>
            new LevelDiagnostic(LevelDiagnosticStage.Migration, LevelDiagnosticCodes.MigrationNeedsEditorialDecision, path, message);

        private static LevelMigrationResult Fail(LevelDiagnostic diagnostic) => new LevelMigrationResult(null, false, new[] { diagnostic });
    }
}
