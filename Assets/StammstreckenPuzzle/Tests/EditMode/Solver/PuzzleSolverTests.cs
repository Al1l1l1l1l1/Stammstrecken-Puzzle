using System.Collections.Generic;
using NUnit.Framework;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Solver.EditMode
{
    /// <summary>
    /// Klassifikation, Determinismus und Grenzfälle des Constraint-Solvers gemäß
    /// SOLVER_ARCHITECTURE.md Abschnitte 4, 5 und 11 sowie ADR-007. Ein
    /// unabhängiger Brute-Force-Enumerator (nur Testcode) prüft sehr kleine
    /// Raster gegen den Produktionssolver.
    /// </summary>
    public sealed class PuzzleSolverTests
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

        private static PuzzleDefinition SnakeDefinition()
        {
            return new PuzzleDefinition(
                new GridSize(3, 3),
                new Endpoint(Direction.N, 0),
                new Endpoint(Direction.S, 2),
                new[] { 1, 1, 3 },
                new[] { 3, 1, 1 },
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

        private static PuzzleDefinition LongChainDefinition()
        {
            return new PuzzleDefinition(
                new GridSize(4, 4),
                new Endpoint(Direction.N, 0),
                new Endpoint(Direction.S, 3),
                new[] { 1, 1, 1, 4 },
                new[] { 4, 1, 1, 1 },
                PuzzleDefinition.RulesetVersionV1);
        }

        /// <summary>Die Ein-Zellen-Fixture ist rein deduktiv eindeutig lösbar (Annahmetiefe 0).</summary>
        [Test]
        public void SingleCell_IsUniqueWithoutGuessing()
        {
            var result = PuzzleSolver.Solve(SingleCellDefinition());

            Assert.AreEqual(SolutionClassification.Unique, result.Classification);
            Assert.AreEqual(1, result.SolutionCount);
            Assert.NotNull(result.UniqueSolution);
            var grid = new GridSize(2, 2);
            Assert.AreEqual(CellContent.TrackWN, result.UniqueSolution![grid.IndexOf(new CellCoordinate(0, 0))]);
            Assert.AreEqual(CellContent.Unset, result.UniqueSolution[grid.IndexOf(new CellCoordinate(1, 1))]);
            Assert.AreEqual(1, result.Metrics.PathLength);
            Assert.AreEqual(0, result.Metrics.RequiredGuessDepth);
            Assert.IsTrue(CompletionEvaluator.IsSolved(SingleCellDefinition(), result.UniqueSolution));
        }

        /// <summary>Die 3×3-Schlange ist eindeutig; die gefundene Lösung trägt die erwarteten Formen.</summary>
        [Test]
        public void Snake_IsUniqueWithExpectedShapes()
        {
            var definition = SnakeDefinition();
            var result = PuzzleSolver.Solve(definition);

            Assert.AreEqual(SolutionClassification.Unique, result.Classification);
            var grid = definition.Grid;
            Assert.AreEqual(CellContent.TrackNS, result.UniqueSolution![grid.IndexOf(new CellCoordinate(0, 0))]);
            Assert.AreEqual(CellContent.TrackNS, result.UniqueSolution[grid.IndexOf(new CellCoordinate(0, 1))]);
            Assert.AreEqual(CellContent.TrackNE, result.UniqueSolution[grid.IndexOf(new CellCoordinate(0, 2))]);
            Assert.AreEqual(CellContent.TrackEW, result.UniqueSolution[grid.IndexOf(new CellCoordinate(1, 2))]);
            Assert.AreEqual(CellContent.TrackSW, result.UniqueSolution[grid.IndexOf(new CellCoordinate(2, 2))]);
            Assert.AreEqual(5, result.Metrics.PathLength);
            Assert.IsTrue(CompletionEvaluator.IsSolved(definition, result.UniqueSolution!));
        }

        /// <summary>Die lange eindeutige 4×4-Kette ist eindeutig lösbar.</summary>
        [Test]
        public void LongChain_IsUnique()
        {
            var definition = LongChainDefinition();
            var result = PuzzleSolver.Solve(definition);

            Assert.AreEqual(SolutionClassification.Unique, result.Classification);
            var grid = definition.Grid;
            Assert.AreEqual(CellContent.TrackNS, result.UniqueSolution![grid.IndexOf(new CellCoordinate(0, 2))]);
            Assert.AreEqual(CellContent.TrackNE, result.UniqueSolution[grid.IndexOf(new CellCoordinate(0, 3))]);
            Assert.AreEqual(CellContent.TrackEW, result.UniqueSolution[grid.IndexOf(new CellCoordinate(1, 3))]);
            Assert.AreEqual(CellContent.TrackSW, result.UniqueSolution[grid.IndexOf(new CellCoordinate(3, 3))]);
            Assert.AreEqual(7, result.Metrics.PathLength);
            Assert.IsTrue(CompletionEvaluator.IsSolved(definition, result.UniqueSolution!));
        }

        /// <summary>Eine widersprüchliche Endpoint-/Zahlenkonstellation ist unlösbar.</summary>
        [Test]
        public void ContradictoryPuzzle_IsUnsatisfiable()
        {
            var result = PuzzleSolver.Solve(UnsatisfiableDefinition());

            Assert.AreEqual(SolutionClassification.Unsatisfiable, result.Classification);
            Assert.AreEqual(0, result.SolutionCount);
            Assert.IsNull(result.UniqueSolution);
        }

        /// <summary>Die volle 4×4-Fläche mit Mittelraute hat zwei oder mehr Lösungen; die Zählung stoppt bei zwei.</summary>
        [Test]
        public void FullBoardWithMiddleDiamond_IsMultipleOrMore()
        {
            var result = PuzzleSolver.Solve(MultipleDefinition());

            Assert.AreEqual(SolutionClassification.MultipleOrMore, result.Classification);
            Assert.AreEqual(2, result.SolutionCount);
            Assert.IsNull(result.UniqueSolution);
        }

        /// <summary>Ein erschöpftes Ressourcenlimit ergibt INDETERMINATE, niemals UNIQUE.</summary>
        [Test]
        public void ExhaustedBudget_IsIndeterminate_NeverUnique()
        {
            var result = PuzzleSolver.Solve(MultipleDefinition(), maxSearchNodes: 0);

            Assert.AreEqual(SolutionClassification.Indeterminate, result.Classification);
            Assert.IsNull(result.UniqueSolution);
        }

        /// <summary>Identische Eingaben liefern identische Ergebnisse, Lösungen und Metriken.</summary>
        [Test]
        public void Solve_IsDeterministic()
        {
            var definition = SnakeDefinition();
            var first = PuzzleSolver.Solve(definition);
            var second = PuzzleSolver.Solve(definition);

            Assert.AreEqual(first.Classification, second.Classification);
            Assert.AreEqual(first.Metrics.SearchNodes, second.Metrics.SearchNodes);
            Assert.AreEqual(first.Metrics.DeductionSteps, second.Metrics.DeductionSteps);
            Assert.AreEqual(first.Metrics.RequiredGuessDepth, second.Metrics.RequiredGuessDepth);
            CollectionAssert.AreEqual(first.UniqueSolution!, second.UniqueSolution!);
        }

        /// <summary>
        /// Unabhängiger Gegenbeweis auf sehr kleinen Rastern: Ein einfacher
        /// Brute-Force-Enumerator (kein Produktionscode) zählt Lösungen über die
        /// Domain-Completion und muss dieselbe Klassifikation liefern wie der
        /// Produktionssolver.
        /// </summary>
        [Test]
        public void ExhaustiveEnumerator_AgreesOnTinyGrids()
        {
            var definitions = new[]
            {
                SingleCellDefinition(),
                UnsatisfiableDefinition(),
                SnakeDefinition(),
                new PuzzleDefinition(
                    new GridSize(3, 3),
                    new Endpoint(Direction.N, 0),
                    new Endpoint(Direction.N, 1),
                    new[] { 2, 2, 0 },
                    new[] { 2, 2, 0 },
                    PuzzleDefinition.RulesetVersionV1),
            };

            foreach (var definition in definitions)
            {
                var expected = BruteForceCount(definition, cap: 2);
                var result = PuzzleSolver.Solve(definition);
                Assert.AreEqual(
                    expected,
                    result.SolutionCount,
                    $"Lösungszahl abweichend für Raster {definition.Grid} mit A={definition.A}, B={definition.B}");
            }
        }

        private static int BruteForceCount(PuzzleDefinition definition, int cap)
        {
            var grid = definition.Grid;
            var total = grid.Width * grid.Height;
            var cells = new CellContent[total];
            var rowUsed = new int[grid.Height];
            var columnUsed = new int[grid.Width];
            var count = 0;

            void Recurse(int index)
            {
                if (count >= cap)
                {
                    return;
                }
                if (index == total)
                {
                    for (var y = 0; y < grid.Height; y++)
                    {
                        if (rowUsed[y] != definition.RowCounts[y])
                        {
                            return;
                        }
                    }
                    for (var x = 0; x < grid.Width; x++)
                    {
                        if (columnUsed[x] != definition.ColumnCounts[x])
                        {
                            return;
                        }
                    }
                    if (CompletionEvaluator.IsSolved(definition, cells))
                    {
                        count++;
                    }
                    return;
                }

                var coordinate = new CellCoordinate(index % grid.Width, index / grid.Width);
                cells[index] = CellContent.Unset;
                Recurse(index + 1);
                if (rowUsed[coordinate.Y] >= definition.RowCounts[coordinate.Y]
                    || columnUsed[coordinate.X] >= definition.ColumnCounts[coordinate.X])
                {
                    cells[index] = CellContent.Unset;
                    return;
                }
                foreach (var shape in TrackShapeGeometry.All)
                {
                    cells[index] = CellContentSemantics.FromTrackShape(shape);
                    rowUsed[coordinate.Y]++;
                    columnUsed[coordinate.X]++;
                    Recurse(index + 1);
                    rowUsed[coordinate.Y]--;
                    columnUsed[coordinate.X]--;
                }
                cells[index] = CellContent.Unset;
            }

            Recurse(0);
            return count;
        }
    }
}
