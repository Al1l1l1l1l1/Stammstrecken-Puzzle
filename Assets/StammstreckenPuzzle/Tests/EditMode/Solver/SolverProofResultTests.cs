using NUnit.Framework;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Solver.EditMode
{
    /// <summary>
    /// Proof-Ergebnismodell des Solvers gemäß WP-013 (Scopepunkt 3):
    /// deterministisch für bekannte 0-/1-/2+-Fälle, gefundener Lösungspfad
    /// bei eindeutiger Lösung, dokumentierte Metriken einschließlich
    /// MaxDeductionDepth, unveränderte <c>solver-v1</c>-Versionskonstante.
    /// </summary>
    public sealed class SolverProofResultTests
    {
        private static PuzzleDefinition SingleCellDefinition()
        {
            return new PuzzleDefinition(
                new GridSize(2, 2),
                new Endpoint(Direction.N, 0),
                new Endpoint(Direction.W, 0),
                new[] { 1, 0 },
                new[] { 1, 0 },
                PuzzleDefinition.RulesetVersionV1);
        }

        private static PuzzleDefinition UnsatisfiableDefinition()
        {
            return new PuzzleDefinition(
                new GridSize(2, 2),
                new Endpoint(Direction.N, 0),
                new Endpoint(Direction.E, 1),
                new[] { 1, 1 },
                new[] { 1, 1 },
                PuzzleDefinition.RulesetVersionV1);
        }

        private static PuzzleDefinition MultipleDefinition()
        {
            return new PuzzleDefinition(
                new GridSize(4, 4),
                new Endpoint(Direction.N, 0),
                new Endpoint(Direction.W, 1),
                new[] { 4, 4, 4, 4 },
                new[] { 4, 4, 4, 4 },
                PuzzleDefinition.RulesetVersionV1);
        }

        private static PuzzleDefinition GuessRequiredDefinition()
        {
            return new PuzzleDefinition(
                new GridSize(5, 5),
                new Endpoint(Direction.N, 2),
                new Endpoint(Direction.W, 3),
                new[] { 2, 2, 3, 3, 2 },
                new[] { 2, 2, 3, 3, 2 },
                PuzzleDefinition.RulesetVersionV1);
        }

        private static void AssertDeterministic(PuzzleDefinition definition)
        {
            var first = PuzzleSolver.SolveForProof(definition);
            var second = PuzzleSolver.SolveForProof(definition);

            Assert.AreEqual(first.Classification, second.Classification);
            Assert.AreEqual(first.SolutionCount, second.SolutionCount);
            Assert.AreEqual(first.Metrics.SearchNodes, second.Metrics.SearchNodes);
            Assert.AreEqual(first.Metrics.DeductionSteps, second.Metrics.DeductionSteps);
            Assert.AreEqual(first.Metrics.RequiredGuessDepth, second.Metrics.RequiredGuessDepth);
            Assert.AreEqual(first.Metrics.PathLength, second.Metrics.PathLength);
            Assert.AreEqual(first.MaxDeductionDepth, second.MaxDeductionDepth);
            Assert.AreEqual(first.SolverVersion, second.SolverVersion);
            if (first.UniqueSolutionPath is null)
            {
                Assert.IsNull(second.UniqueSolutionPath);
            }
            else
            {
                Assert.NotNull(second.UniqueSolutionPath);
                Assert.AreEqual(first.UniqueSolutionPath.Count, second.UniqueSolutionPath!.Count);
                for (var i = 0; i < first.UniqueSolutionPath.Count; i++)
                {
                    Assert.AreEqual(first.UniqueSolutionPath[i].Coordinate, second.UniqueSolutionPath[i].Coordinate);
                    Assert.AreEqual(first.UniqueSolutionPath[i].Track, second.UniqueSolutionPath[i].Track);
                }
            }
        }

        /// <summary>Der 0-Lösungs-Fall ist deterministisch und trägt keinen Lösungspfad.</summary>
        [Test]
        public void UnsatisfiableCase_IsDeterministicWithoutPath()
        {
            var result = PuzzleSolver.SolveForProof(UnsatisfiableDefinition());
            Assert.AreEqual(SolutionClassification.Unsatisfiable, result.Classification);
            Assert.AreEqual(0, result.SolutionCount);
            Assert.IsNull(result.UniqueSolutionPath);
            AssertDeterministic(UnsatisfiableDefinition());
        }

        /// <summary>Der 1-Lösungs-Fall ist deterministisch und trägt den vollständigen Lösungspfad.</summary>
        [Test]
        public void UniqueCase_IsDeterministicWithPath()
        {
            var result = PuzzleSolver.SolveForProof(SingleCellDefinition());
            Assert.AreEqual(SolutionClassification.Unique, result.Classification);
            Assert.AreEqual(1, result.SolutionCount);
            Assert.NotNull(result.UniqueSolutionPath);
            Assert.AreEqual(1, result.UniqueSolutionPath!.Count);
            Assert.AreEqual(new CellCoordinate(0, 0), result.UniqueSolutionPath[0].Coordinate);
            Assert.AreEqual(TrackShape.WN, result.UniqueSolutionPath[0].Track);
            Assert.GreaterOrEqual(result.MaxDeductionDepth, 1);
            AssertDeterministic(SingleCellDefinition());
        }

        /// <summary>Der gefundene Lösungspfad einer größeren eindeutigen Lösung stimmt mit der Referenzlösung überein.</summary>
        [Test]
        public void UniqueCase_PathMatchesReferenceSolution()
        {
            var definition = new PuzzleDefinition(
                new GridSize(4, 4),
                new Endpoint(Direction.N, 0),
                new Endpoint(Direction.S, 3),
                new[] { 1, 3, 4, 1 },
                new[] { 3, 1, 2, 3 },
                PuzzleDefinition.RulesetVersionV1);
            var result = PuzzleSolver.SolveForProof(definition);

            Assert.AreEqual(SolutionClassification.Unique, result.Classification);
            var expected = new[]
            {
                (0, 0, TrackShape.NS), (0, 1, TrackShape.NS), (0, 2, TrackShape.NE),
                (1, 2, TrackShape.EW), (2, 2, TrackShape.WN), (2, 1, TrackShape.ES),
                (3, 1, TrackShape.SW), (3, 2, TrackShape.NS), (3, 3, TrackShape.NS),
            };
            Assert.NotNull(result.UniqueSolutionPath);
            Assert.AreEqual(expected.Length, result.UniqueSolutionPath!.Count);
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(new CellCoordinate(expected[i].Item1, expected[i].Item2), result.UniqueSolutionPath[i].Coordinate, $"Zelle {i}");
                Assert.AreEqual(expected[i].Item3, result.UniqueSolutionPath[i].Track, $"Zelle {i}");
            }
            Assert.AreEqual(expected.Length, result.Metrics.PathLength);
        }

        /// <summary>Der 2+-Lösungs-Fall ist deterministisch und trägt keinen Lösungspfad.</summary>
        [Test]
        public void MultipleCase_IsDeterministicWithoutPath()
        {
            var result = PuzzleSolver.SolveForProof(MultipleDefinition());
            Assert.AreEqual(SolutionClassification.MultipleOrMore, result.Classification);
            Assert.AreEqual(2, result.SolutionCount);
            Assert.IsNull(result.UniqueSolutionPath);
            AssertDeterministic(MultipleDefinition());
        }

        /// <summary>Ein Suchbudget von null ergibt INDETERMINATE, niemals UNIQUE.</summary>
        [Test]
        public void ExhaustedBudget_IsIndeterminate()
        {
            var result = PuzzleSolver.SolveForProof(GuessRequiredDefinition(), maxSearchNodes: 0);
            Assert.AreEqual(SolutionClassification.Indeterminate, result.Classification);
            Assert.IsNull(result.UniqueSolutionPath);
        }

        /// <summary>Ein annahmepflichtiger eindeutiger Fall meldet Annahmetiefe größer null und bleibt deterministisch.</summary>
        [Test]
        public void GuessRequiredCase_ReportsGuessDepth()
        {
            var result = PuzzleSolver.SolveForProof(GuessRequiredDefinition());
            Assert.AreEqual(SolutionClassification.Unique, result.Classification);
            Assert.GreaterOrEqual(result.Metrics.RequiredGuessDepth, 1);
            Assert.NotNull(result.UniqueSolutionPath);
            AssertDeterministic(GuessRequiredDefinition());
        }

        /// <summary>Die solver-v1-Versionskonstante ist unverändert und wird im Modell gebündelt.</summary>
        [Test]
        public void SolverVersionConstant_IsUnchanged()
        {
            Assert.AreEqual("solver-v1", PuzzleSolver.SolverVersion);
            Assert.AreEqual("solver-v1", PuzzleSolver.SolveForProof(SingleCellDefinition()).SolverVersion);
        }
    }
}
