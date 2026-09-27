using NUnit.Framework;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Solver.EditMode
{
    /// <summary>
    /// Metamorphose-Prüfungen gemäß SOLVER_ARCHITECTURE.md Abschnitt 11 und
    /// ADR-007: Spiegelung des Puzzles samt Endpoints und Randzahlen lässt die
    /// Lösungsklasse invariant.
    /// </summary>
    public sealed class MetamorphicTests
    {
        /// <summary>Spiegelt eine Definition horizontal (x → width-1-x; W↔E; Spaltenzahlen umgekehrt).</summary>
        private static PuzzleDefinition MirrorHorizontal(PuzzleDefinition source)
        {
            var grid = source.Grid;
            var columns = new int[grid.Width];
            for (var x = 0; x < grid.Width; x++)
            {
                columns[x] = source.ColumnCounts[grid.Width - 1 - x];
            }

            return new PuzzleDefinition(
                grid,
                MirrorEndpoint(source.A, grid),
                MirrorEndpoint(source.B, grid),
                source.RowCounts,
                columns,
                source.RulesetVersion);
        }

        private static Endpoint MirrorEndpoint(Endpoint endpoint, GridSize grid)
        {
            return endpoint.Side switch
            {
                Direction.N or Direction.S => new Endpoint(endpoint.Side, grid.Width - 1 - endpoint.Index),
                Direction.W => new Endpoint(Direction.E, endpoint.Index),
                Direction.E => new Endpoint(Direction.W, endpoint.Index),
                _ => endpoint,
            };
        }

        private static CellContent MirrorContent(CellContent content)
        {
            return content switch
            {
                CellContent.TrackNE => CellContent.TrackWN,
                CellContent.TrackWN => CellContent.TrackNE,
                CellContent.TrackES => CellContent.TrackSW,
                CellContent.TrackSW => CellContent.TrackES,
                _ => content,
            };
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

        /// <summary>Die gespiegelte Ein-Zellen-Fixture bleibt eindeutig und liefert die gespiegelte Form.</summary>
        [Test]
        public void MirroredSingleCell_StaysUniqueWithMirroredShape()
        {
            var original = new PuzzleDefinition(
                new GridSize(2, 2),
                new Endpoint(Direction.N, 0),
                new Endpoint(Direction.W, 0),
                new[] { 1, 0 },
                new[] { 1, 0 },
                PuzzleDefinition.RulesetVersionV1);
            var mirrored = MirrorHorizontal(original);

            var result = PuzzleSolver.Solve(mirrored);

            Assert.AreEqual(SolutionClassification.Unique, result.Classification);
            Assert.NotNull(result.UniqueSolution);
            var grid = mirrored.Grid;
            Assert.AreEqual(
                CellContent.TrackNE,
                result.UniqueSolution![grid.IndexOf(new CellCoordinate(1, 0))]);
            Assert.IsTrue(CompletionEvaluator.IsSolved(mirrored, result.UniqueSolution!));
        }

        /// <summary>Die gespiegelte 3×3-Schlange behält ihre Lösungsklasse und die gespiegelte Lösung.</summary>
        [Test]
        public void MirroredSnake_KeepsClassificationAndMirroredSolution()
        {
            var original = SnakeDefinition();
            var mirrored = MirrorHorizontal(original);

            var originalResult = PuzzleSolver.Solve(original);
            var mirroredResult = PuzzleSolver.Solve(mirrored);

            Assert.AreEqual(originalResult.Classification, mirroredResult.Classification);
            Assert.AreEqual(SolutionClassification.Unique, mirroredResult.Classification);

            var grid = original.Grid;
            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    var expected = MirrorContent(originalResult.UniqueSolution![grid.IndexOf(new CellCoordinate(x, y))]);
                    var actual = mirroredResult.UniqueSolution![grid.IndexOf(new CellCoordinate(grid.Width - 1 - x, y))];
                    Assert.AreEqual(expected, actual, $"Gespiegelte Zelle ({x},{y})");
                }
            }
            Assert.IsTrue(CompletionEvaluator.IsSolved(mirrored, mirroredResult.UniqueSolution!));
        }
    }
}
