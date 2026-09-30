using System;
using System.Collections.Generic;
using STP.Infrastructure.Content;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Editor.Content
{
    /// <summary>Stufe der LevelValidationPipeline.</summary>
    public enum PipelineStage : byte
    {
        /// <summary>Parse: Bytes → JSON-Dokument oder Parsecodes.</summary>
        Parse = 0,

        /// <summary>Schema: Strukturdiagnosen gegen den level-v2-Vertrag.</summary>
        Schema = 1,

        /// <summary>Domain map: gültiges Domainobjekt oder Mappingcodes.</summary>
        DomainMap = 2,

        /// <summary>Semantic: semantische Leveldiagnosen (LVL-*).</summary>
        Semantic = 3,

        /// <summary>Solver/Proof: Lösungszählung, Lösungshashvergleich, Proofbindung und Proofgenerierung.</summary>
        SolverProof = 4,
    }

    /// <summary>
    /// Eingabeeinheit des Batchvalidators: ein Leveldokument plus optional
    /// zugeordnetem proof-v1-Artefakt (benannt über seine Artefakt-ID).
    /// </summary>
    public sealed class LevelValidationInput
    {
        /// <summary>Logischer Quellenname des Leveldokuments (für Berichte und Sortierung).</summary>
        public string SourceName { get; }

        /// <summary>Rohe Bytes des level-v2-Dokuments.</summary>
        public byte[] LevelBytes { get; }

        /// <summary>Artefakt-ID des bereitgestellten Proofs oder <c>null</c>.</summary>
        public string? ProofArtifactId { get; }

        /// <summary>Rohe Bytes des proof-v1-Artefakts oder <c>null</c>.</summary>
        public byte[]? ProofBytes { get; }

        /// <summary>Erstellt die Eingabeeinheit.</summary>
        public LevelValidationInput(string sourceName, byte[] levelBytes, string? proofArtifactId = null, byte[]? proofBytes = null)
        {
            SourceName = sourceName ?? throw new ArgumentNullException(nameof(sourceName));
            LevelBytes = levelBytes ?? throw new ArgumentNullException(nameof(levelBytes));
            ProofArtifactId = proofArtifactId;
            ProofBytes = proofBytes;
        }
    }

    /// <summary>Ergebnis eines einzelnen Leveldatensatzes.</summary>
    public sealed class LevelValidationResult
    {
        /// <summary>Logischer Quellenname.</summary>
        public string SourceName { get; }

        /// <summary>Stufe, bei der die Prüfung wegen eines Fehlers gestoppt wurde, oder <c>null</c>, wenn alle Stufen ohne Fehler liefen.</summary>
        public PipelineStage? FailedStage { get; }

        /// <summary>Sortierte Diagnosen (stabile Codes; Fehler und Warnungen).</summary>
        public IReadOnlyList<LevelDiagnostic> Diagnostics { get; }

        /// <summary>Kein Fehler in dieser Stufenkette (Warnungen sind zulässig).</summary>
        public bool Succeeded { get; }

        /// <summary>Berechneter semantischer Puzzlehash (ab Solver/Proof-Stufe), sonst <c>null</c>.</summary>
        public ProfiledHash? PublicPuzzleHash { get; }

        /// <summary>Berechneter Lösungshash der Authoringlösung (ab Solver/Proof-Stufe), sonst <c>null</c>.</summary>
        public ProfiledHash? SolutionHash { get; }

        /// <summary>Generierter Proof aus dem Solverlauf (kanonische Bytes, bei eindeutigem Solverlauf), sonst <c>null</c>.</summary>
        public byte[]? GeneratedProofBytes { get; }

        /// <summary>Erstellt das Ergebnis.</summary>
        public LevelValidationResult(
            string sourceName,
            PipelineStage? failedStage,
            IReadOnlyList<LevelDiagnostic> diagnostics,
            bool succeeded,
            ProfiledHash? publicPuzzleHash,
            ProfiledHash? solutionHash,
            byte[]? generatedProofBytes)
        {
            SourceName = sourceName;
            FailedStage = failedStage;
            Diagnostics = diagnostics;
            Succeeded = succeeded;
            PublicPuzzleHash = publicPuzzleHash;
            SolutionHash = solutionHash;
            GeneratedProofBytes = generatedProofBytes;
        }
    }

    /// <summary>Gesamtbericht eines Batchlaufs.</summary>
    public sealed class LevelValidationReport
    {
        /// <summary>Ergebnisse je Datensatz, nach Quellenname ordinal sortiert.</summary>
        public IReadOnlyList<LevelValidationResult> Levels { get; }

        /// <summary>Zahl der Fehler über alle Datensätze.</summary>
        public int ErrorCount { get; }

        /// <summary>Zahl der Warnungen über alle Datensätze.</summary>
        public int WarningCount { get; }

        /// <summary>Aktiver <c>--strict</c>-Modus.</summary>
        public bool Strict { get; }

        /// <summary>Gesamtergebnis: kein Fehler; im <c>--strict</c>-Modus zusätzlich keine Warnung.</summary>
        public bool Succeeded { get; }

        /// <summary>Erstellt den Bericht.</summary>
        public LevelValidationReport(IReadOnlyList<LevelValidationResult> levels, int errorCount, int warningCount, bool strict, bool succeeded)
        {
            Levels = levels;
            ErrorCount = errorCount;
            WarningCount = warningCount;
            Strict = strict;
            Succeeded = succeeded;
        }
    }

    /// <summary>
    /// LevelValidationPipeline als gemeinsamer Batchvalidator gemäß
    /// CONTENT_PIPELINE.md Abschnitt 5 mit den Stufen Parse, Schema,
    /// Domain map, Semantic und Solver/Proof: sortierte Fehler/Warnungen mit
    /// stabilen Diagnosecodes, ein <c>--strict</c>-Modus, der Warnungen als
    /// Fehler behandelt, und die Regel, dass ein späterer Schritt einen
    /// früheren Fehler nicht durch Fallback verdeckt (bei einem Stufenfehler
    /// werden abhängige Folgestufen für diesen Datensatz nicht mehr
    /// ausgeführt; frühere Diagnosen bleiben vollständig erhalten). Die
    /// Stufen Cross-reference, Quality und Import sind nicht Teil von WP-013.
    /// Diese Klasse enthält keine GUI und kein davon abweichendes
    /// Validierungsverhalten.
    /// </summary>
    public static class LevelValidationPipeline
    {
        /// <summary>Validiert einen einzelnen Datensatz durch alle fünf Stufen.</summary>
        public static LevelValidationResult ValidateOne(LevelValidationInput input, bool strict)
        {
            return Validate(new[] { input }, strict).Levels[0];
        }

        /// <summary>Validiert einen Batch von Datensätzen; der Bericht ist deterministisch sortiert.</summary>
        public static LevelValidationReport Validate(IReadOnlyList<LevelValidationInput> inputs, bool strict)
        {
            if (inputs is null)
            {
                throw new ArgumentNullException(nameof(inputs));
            }

            var results = new List<LevelValidationResult>();
            foreach (var input in inputs)
            {
                results.Add(ValidateLevel(input));
            }
            results.Sort((left, right) => string.CompareOrdinal(left.SourceName, right.SourceName));

            var errors = 0;
            var warnings = 0;
            foreach (var result in results)
            {
                foreach (var diagnostic in result.Diagnostics)
                {
                    if (diagnostic.Severity == DiagnosticSeverity.Error)
                    {
                        errors++;
                    }
                    else
                    {
                        warnings++;
                    }
                }
            }
            var succeeded = errors == 0 && (!strict || warnings == 0);
            return new LevelValidationReport(results, errors, warnings, strict, succeeded);
        }

        private static LevelValidationResult ValidateLevel(LevelValidationInput input)
        {
            var diagnostics = new List<LevelDiagnostic>();
            var limits = JsonParseLimits.Default;

            JsonValue parsed;
            try
            {
                parsed = StrictJsonParser.ParseUtf8(input.LevelBytes, limits);
            }
            catch (StrictJsonException ex)
            {
                diagnostics.Add(LevelDiagnostic.Error(ex.Code, "$", ex.Message));
                return Result(input, PipelineStage.Parse, diagnostics, null, null, null);
            }

            var document = LevelV2SchemaValidator.Validate(parsed, diagnostics);
            if (document is null || HasError(diagnostics))
            {
                return Result(input, PipelineStage.Schema, diagnostics, null, null, null);
            }

            var definition = LevelV2DomainMapper.Map(document, diagnostics);
            if (definition is null || HasError(diagnostics))
            {
                return Result(input, PipelineStage.DomainMap, diagnostics, null, null, null);
            }

            LevelV2Semantics.Validate(document, definition, diagnostics);
            if (HasError(diagnostics))
            {
                return Result(input, PipelineStage.Semantic, diagnostics, null, null, null);
            }

            return RunSolverProofStage(input, document, definition, diagnostics);
        }

        private static LevelValidationResult RunSolverProofStage(
            LevelValidationInput input,
            LevelV2Document document,
            PuzzleDefinition definition,
            List<LevelDiagnostic> diagnostics)
        {
            var run = PuzzleSolver.SolveForProof(definition);
            if (run.Classification != SolutionClassification.Unique || run.UniqueSolutionPath is null)
            {
                diagnostics.Add(LevelDiagnostic.Error(
                    "LVL-SOLVER-NOT-UNIQUE",
                    "$",
                    $"Der Solver klassifiziert den öffentlichen Puzzleinput als {run.Classification} (Lösungszahl {run.SolutionCount}); verbindlich ist genau eine Lösung."));
                return Result(input, PipelineStage.SolverProof, diagnostics, null, null, null);
            }

            var publicPuzzleHash = PuzzleHashContracts.ComputePublicPuzzleHash(document.Source);
            document.Source.TryGet("puzzleId", out var puzzleIdValue);
            var authoringPath = AuthoringPath(document);
            var solutionHash = PuzzleHashContracts.ComputeSolutionHash(puzzleIdValue, authoringPath, publicPuzzleHash);

            var foundSolutionHash = PuzzleHashContracts.ComputeSolutionHash(
                puzzleIdValue,
                SolverPath(run.UniqueSolutionPath),
                publicPuzzleHash);
            if (foundSolutionHash != solutionHash)
            {
                diagnostics.Add(LevelDiagnostic.Error(
                    "LVL-SOLVER-SOLUTION-MISMATCH",
                    "$.solution",
                    "Die einzige gefundene Lösung weicht über den kanonischen Lösungshash von der Authoringlösung ab."));
            }

            if (run.Metrics.RequiredGuessDepth > 0)
            {
                diagnostics.Add(LevelDiagnostic.Warning(
                    "LVL-SOLVER-GUESS-REQUIRED",
                    "$",
                    $"Die eindeutige Lösung erfordert Annahmetiefe {run.Metrics.RequiredGuessDepth}; deduktive Standardlevel verlangen 0 (SOLVER_ARCHITECTURE.md Abschnitt 6)."));
            }

            var generatedProofBytes = ProofGenerator.GenerateCanonicalBytes(run, document.PuzzleId, publicPuzzleHash, solutionHash);

            if (input.ProofBytes is null)
            {
                diagnostics.Add(LevelDiagnostic.Error("PRF-ARTIFACT-MISSING", "$.proofRef", "Zum Datensatz wurde kein proof-v1-Artefakt bereitgestellt."));
            }
            else
            {
                var proofDiagnostics = new List<LevelDiagnostic>();
                var proof = ProofV1Serializer.Deserialize(input.ProofBytes, JsonParseLimits.Default, proofDiagnostics);
                diagnostics.AddRange(proofDiagnostics);
                if (proof is not null)
                {
                    ProofV1Binding.Validate(
                        document,
                        proof,
                        publicPuzzleHash,
                        solutionHash,
                        PuzzleSolver.SolverVersion,
                        input.ProofArtifactId ?? string.Empty,
                        diagnostics);
                }
            }

            return Result(input, PipelineStage.SolverProof, diagnostics, publicPuzzleHash, solutionHash, generatedProofBytes);
        }

        private static JsonValue.Array AuthoringPath(LevelV2Document document)
        {
            var items = new List<JsonValue>();
            foreach (var cell in document.Solution.Path)
            {
                items.Add(PathCell(cell.X, cell.Y, cell.Track));
            }
            return new JsonValue.Array(items);
        }

        private static JsonValue.Array SolverPath(IReadOnlyList<SolverPathCell> path)
        {
            var items = new List<JsonValue>();
            foreach (var cell in path)
            {
                items.Add(PathCell(cell.Coordinate.X, cell.Coordinate.Y, cell.Track));
            }
            return new JsonValue.Array(items);
        }

        private static JsonValue.Object PathCell(int x, int y, TrackShape track)
        {
            return new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
            {
                new KeyValuePair<string, JsonValue>("x", new JsonValue.Integer(x)),
                new KeyValuePair<string, JsonValue>("y", new JsonValue.Integer(y)),
                new KeyValuePair<string, JsonValue>("track", new JsonValue.String(TrackName(track))),
            });
        }

        private static string TrackName(TrackShape track)
        {
            return track switch
            {
                TrackShape.NS => "TRACK_NS",
                TrackShape.EW => "TRACK_EW",
                TrackShape.NE => "TRACK_NE",
                TrackShape.ES => "TRACK_ES",
                TrackShape.SW => "TRACK_SW",
                TrackShape.WN => "TRACK_WN",
                _ => throw new ArgumentOutOfRangeException(nameof(track)),
            };
        }

        private static bool HasError(List<LevelDiagnostic> diagnostics)
        {
            foreach (var diagnostic in diagnostics)
            {
                if (diagnostic.Severity == DiagnosticSeverity.Error)
                {
                    return true;
                }
            }
            return false;
        }

        private static LevelValidationResult Result(
            LevelValidationInput input,
            PipelineStage? failedStage,
            List<LevelDiagnostic> diagnostics,
            ProfiledHash? publicPuzzleHash,
            ProfiledHash? solutionHash,
            byte[]? generatedProofBytes)
        {
            var sorted = LevelDiagnostic.Sorted(diagnostics);
            return new LevelValidationResult(
                input.SourceName,
                HasError(diagnostics) ? failedStage : null,
                sorted,
                !HasError(diagnostics),
                publicPuzzleHash,
                solutionHash,
                generatedProofBytes);
        }
    }
}
