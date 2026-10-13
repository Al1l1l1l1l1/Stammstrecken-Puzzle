using System;
using System.Collections.Generic;
using STP.Infrastructure.Content;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Editor.Content
{
    /// <summary>Fresh generation result; solver input never includes an authoring solution or player assumptions.</summary>
    public sealed class ProofGenerationResult
    {
        public LevelV2LoadResult Level { get; }
        public SolverResult? Solver { get; }
        public ProofReadResult? Proof { get; }
        public IReadOnlyList<LevelDiagnostic> Diagnostics { get; }
        public bool Succeeded => Proof != null && Proof.Succeeded && Diagnostics.Count == 0;
        internal ProofGenerationResult(LevelV2LoadResult level, SolverResult? solver, ProofReadResult? proof, IReadOnlyList<LevelDiagnostic> diagnostics)
        { Level = level; Solver = solver; Proof = proof; var copy = new LevelDiagnostic[diagnostics.Count]; for (int i = 0; i < copy.Length; i++) copy[i] = diagnostics[i]; Diagnostics = Array.AsReadOnly(copy); }
    }

    /// <summary>Production solver-v2 proof generation, including an independent pass from the public puzzle and a solution-hash comparison.</summary>
    public static class ProofGenerator
    {
        public static ProofGenerationResult Generate(byte[]? levelBytes, SolverLimits? limits = null) => Generate(LevelV2Loader.Load(levelBytes), limits);

        public static ProofGenerationResult Generate(LevelV2LoadResult level, SolverLimits? limits = null)
        {
            if (level == null) throw new ArgumentNullException(nameof(level));
            if (!level.IsValid) return new ProofGenerationResult(level, null, null, level.Diagnostics);
            var solver = PuzzleSolver.Solve(level.Definition, limits);
            ProofGenerationResult Fail(string code, string message) => new ProofGenerationResult(level, solver, null,
                new[] { new LevelDiagnostic(LevelDiagnosticStage.Solver, code, string.Empty, message) });
            if (solver.Classification == SolverClassification.INDETERMINATE) return Fail(ProofDiagnosticCodes.Indeterminate, "Solver budget, cancellation or trace prevents a definitive proof.");
            if (solver.Classification != SolverClassification.UNIQUE || solver.SolutionCount != 1) return Fail(ProofDiagnosticCodes.NotUnique, "Public puzzle must have exactly one solution.");
            if (solver.Metrics == null) return Fail(ProofDiagnosticCodes.MetricsMissing, "A unique proof requires all four solver-v2 metrics.");
            var document = level.Document!;
            var path = new List<JsonValue>();
            for (int i = 0; i < solver.Path.Count; i++)
            {
                var cell = solver.Path[i];
                Direction incoming = i == 0 ? level.Definition!.A.Side : Toward(cell, solver.Path[i - 1]);
                Direction outgoing = i == solver.Path.Count - 1 ? level.Definition!.B.Side : Toward(cell, solver.Path[i + 1]);
                int ports = (1 << (int)incoming) | (1 << (int)outgoing);
                int shape = -1;
                for (int s = 0; s < 6; s++) if (TrackGeometry.Ports((TrackShape)s) == ports) shape = s;
                if (shape < 0) return Fail(ProofDiagnosticCodes.Solution, "Solver path has no matching track shape.");
                path.Add(Obj(("x", JsonValue.CreateInteger(cell.X)), ("y", JsonValue.CreateInteger(cell.Y)),
                    ("track", JsonValue.CreateString(((LevelTrack)shape).ToString()))));
            }
            var solutionProjection = Obj(("puzzleId", JsonValue.CreateString(document.PuzzleId)),
                ("publicPuzzleHash", level.PublicPuzzleHash!.ToJson()), ("path", JsonValue.CreateArray(path)));
            var solution = LevelHashing.Compute(LevelHashProfiles.Solution, solutionProjection);
            if (!solution.Succeeded || !solution.Hash!.Equals(level.SolutionHash))
                return Fail(ProofDiagnosticCodes.Solution, "Fresh solver solution differs from the canonical authoring solution hash.");
            var m = solver.Metrics;
            // Only the integrated ADR-031 Metrics object is consumed. Reductions and state-budget counters are never used.
            var proof = ProofV1Codec.Create(document.PuzzleId, level.PublicPuzzleHash!, solution.Hash!, solver.SolverVersion,
                m.SearchNodes, m.DeductionSteps, m.MaxDeductionDepth, m.RequiredGuessDepth);
            return new ProofGenerationResult(level, solver, proof.Succeeded ? proof : null, proof.Diagnostics);
        }

        private static Direction Toward(CellCoordinate from, CellCoordinate to) => to.Y < from.Y ? Direction.N : to.X > from.X ? Direction.E : to.Y > from.Y ? Direction.S : Direction.W;
        private static JsonValue Obj(params (string Name, JsonValue Value)[] members)
        { var list = new List<JsonMember>(); foreach (var m in members) list.Add(new JsonMember(m.Name, m.Value)); return JsonValue.CreateObject(list); }
    }
}
