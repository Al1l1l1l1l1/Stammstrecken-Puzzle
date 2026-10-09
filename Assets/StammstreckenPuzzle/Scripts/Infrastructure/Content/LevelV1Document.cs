using System;
using System.Collections.Generic;

namespace STP.Infrastructure.Content
{
    /// <summary>Recorded Level-v1 validation block. Values are legacy facts; they are never recomputed by a solver here.</summary>
    public sealed class LevelV1ValidationDto
    {
        public string PuzzleHashSha256 { get; }
        public string SolutionHashSha256 { get; }
        public string SolverVersion { get; }
        public int SolutionCount { get; }
        public long SearchNodes { get; }
        public long DeductionSteps { get; }
        public long MaxDeductionDepth { get; }
        public long RequiredGuessDepth { get; }
        public string ProofHashSha256 { get; }

        internal LevelV1ValidationDto(string puzzleHash, string solutionHash, string solverVersion, int solutionCount, long searchNodes, long deductionSteps,
            long maxDeductionDepth, long requiredGuessDepth, string proofHash)
        {
            PuzzleHashSha256 = puzzleHash; SolutionHashSha256 = solutionHash; SolverVersion = solverVersion; SolutionCount = solutionCount;
            SearchNodes = searchNodes; DeductionSteps = deductionSteps; MaxDeductionDepth = maxDeductionDepth; RequiredGuessDepth = requiredGuessDepth; ProofHashSha256 = proofHash;
        }
    }

    /// <summary>Typed, immutable, schema-valid Level-v1 source (legacy reader and migration source only).</summary>
    public sealed class LevelV1Document
    {
        public int SchemaVersion { get; }
        public string RulesetVersion { get; }
        public string Id { get; }
        public long ContentRevision { get; }
        public LevelGridDto Grid { get; }
        public LevelEndpointDto EndpointA { get; }
        public LevelEndpointDto EndpointB { get; }
        public IReadOnlyList<int> RowCounts { get; }
        public IReadOnlyList<int> ColumnCounts { get; }
        public LevelContentDto Content { get; }
        public LevelProductionDto Production { get; }
        public LevelCompletionDto Completion { get; }
        public IReadOnlyList<LevelPathCellDto> SolutionPath { get; }
        public LevelV1ValidationDto Validation { get; }

        internal LevelV1Document(int schemaVersion, string rulesetVersion, string id, long contentRevision, LevelGridDto grid, LevelEndpointDto endpointA, LevelEndpointDto endpointB,
            IReadOnlyList<int> rowCounts, IReadOnlyList<int> columnCounts, LevelContentDto content, LevelProductionDto production, LevelCompletionDto completion,
            IReadOnlyList<LevelPathCellDto> solutionPath, LevelV1ValidationDto validation)
        {
            SchemaVersion = schemaVersion; RulesetVersion = rulesetVersion; Id = id; ContentRevision = contentRevision; Grid = grid; EndpointA = endpointA; EndpointB = endpointB;
            RowCounts = rowCounts; ColumnCounts = columnCounts; Content = content; Production = production; Completion = completion; SolutionPath = solutionPath; Validation = validation;
        }

        /// <summary>The full v1 document as JSON.</summary>
        public JsonValue ToJson() => LevelJson.Obj(
            ("schemaVersion", LevelJson.Int(SchemaVersion)),
            ("rulesetVersion", LevelJson.Str(RulesetVersion)),
            ("id", LevelJson.Str(Id)),
            ("contentRevision", LevelJson.Int(ContentRevision)),
            ("grid", Grid.ToJson()),
            ("endpoints", LevelJson.Obj(("a", EndpointA.ToJson()), ("b", EndpointB.ToJson()))),
            ("rowCounts", LevelJson.IntArray(RowCounts)),
            ("columnCounts", LevelJson.IntArray(ColumnCounts)),
            ("content", Content.ToJson()),
            ("production", Production.ToJson()),
            ("completion", Completion.ToJson()),
            ("solution", LevelJson.Obj(("path", LevelPathCellDto.PathToJson(SolutionPath)))),
            ("validation", LevelJson.Obj(
                ("puzzleHashSha256", LevelJson.Str(Validation.PuzzleHashSha256)),
                ("solutionHashSha256", LevelJson.Str(Validation.SolutionHashSha256)),
                ("solverVersion", LevelJson.Str(Validation.SolverVersion)),
                ("solutionCount", LevelJson.Int(Validation.SolutionCount)),
                ("proof", LevelJson.Obj(
                    ("searchNodes", LevelJson.Int(Validation.SearchNodes)),
                    ("deductionSteps", LevelJson.Int(Validation.DeductionSteps)),
                    ("maxDeductionDepth", LevelJson.Int(Validation.MaxDeductionDepth)),
                    ("requiredGuessDepth", LevelJson.Int(Validation.RequiredGuessDepth)),
                    ("proofHashSha256", LevelJson.Str(Validation.ProofHashSha256)))))));
    }

    /// <summary>Result of the Level-v1 schema stage.</summary>
    public sealed class LevelV1ReadResult
    {
        public LevelV1Document? Document { get; }
        public IReadOnlyList<LevelDiagnostic> Diagnostics { get; }
        public bool Succeeded => Document != null && Diagnostics.Count == 0;
        internal LevelV1ReadResult(LevelV1Document? document, IReadOnlyList<LevelDiagnostic> diagnostics) { Document = document; Diagnostics = diagnostics; }
    }
}
