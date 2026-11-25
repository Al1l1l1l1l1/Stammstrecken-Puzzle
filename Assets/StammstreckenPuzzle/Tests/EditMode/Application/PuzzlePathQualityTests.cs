using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using STP.Application;
using STP.Puzzle.Domain;

namespace STP.Tests.Application.EditMode
{
    public sealed class PuzzlePathQualityTests
    {
        private static PuzzleDefinition Definition(int size, IReadOnlyList<CellCoordinate> path)
        {
            var grid = GridSize.Create(size, size).Value!;
            Endpoint Port(CellCoordinate c) => Endpoint.Create(grid, c.X == 0 ? Direction.W : c.X == size - 1 ? Direction.E : c.Y == 0 ? Direction.N : Direction.S,
                c.X == 0 || c.X == size - 1 ? c.Y : c.X).Value!;
            return PuzzleDefinition.Create(grid, Port(path[0]), Port(path[path.Count - 1]),
                Enumerable.Range(0, size).Select(y => path.Count(c => c.Y == y)).ToArray(),
                Enumerable.Range(0, size).Select(x => path.Count(c => c.X == x)).ToArray(), "train-track-v1").Value!;
        }
        private static CellCoordinate Cell(int size, int x, int y) => CellCoordinate.Create(GridSize.Create(size, size).Value!, x, y).Value!;

        [Test]
        public void FullEmptyStripsAndManhattanPathsAreRejected()
        {
            var path = Enumerable.Range(0, 8).Select(x => Cell(8, x, 0)).ToArray();
            Assert.That(PuzzlePathQuality.AcceptsCounts(Definition(8, path)), Is.False);
            Assert.That(PuzzlePathQuality.Accepts(Definition(8, path), path), Is.False);
        }

        [Test]
        public void FiveParallelArmsAreRejectedEvenWhenArmLengthsVary()
        {
            // Actual audited former S1-05-02-06: unequal arms, one zero line, no full line.
            // Counts and detour pass, so rejection specifically exercises the arm-series gate.
            var coordinates = new[] { (0,0),(0,1),(0,2),(0,3),(0,4),(0,5),(0,6),(0,7),(1,7),(2,7),
                (2,6),(2,5),(2,4),(2,3),(3,3),(4,3),(4,4),(4,5),(4,6),(5,6),(6,6),(6,5),
                (6,4),(6,3),(6,2),(6,1),(7,1),(8,1),(8,2),(8,3),(8,4),(8,5),(8,6),(8,7) };
            var path = coordinates.Select(c => Cell(9, c.Item1, c.Item2)).ToArray();
            Assert.That(PuzzlePathQuality.AcceptsCounts(Definition(9, path)), Is.True);
            Assert.That(PuzzlePathQuality.Accepts(Definition(9, path), path), Is.False);
        }

        [TestCase(4, 5)][TestCase(6, 8)][TestCase(8, 10)]
        public void ActualGeneratedSolverPathAlwaysPassesTheSameStructuralGates(int minimum, int maximum)
        {
            var result = PuzzleCandidateSearch.GenerateAsync(minimum >= 8 ? 1UL : 2UL,
                new PuzzleCandidateParameters(minimum, maximum, 30, 65, 0, 0, 10000, 50000, 60000, 1000, 12, 0),
                Array.Empty<PuzzleDefinition>(), Array.Empty<PuzzleDefinition>(), CancellationToken.None, () => 0).GetAwaiter().GetResult();
            Assert.That(result.Succeeded, Is.True, result.Status);
            Assert.That(PuzzlePathQuality.Accepts(result.Puzzle!, result.Verification!.Path), Is.True);
        }
    }
}
