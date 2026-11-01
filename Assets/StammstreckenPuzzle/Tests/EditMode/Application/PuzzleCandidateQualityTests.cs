using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using STP.Application;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Application.EditMode
{
    public sealed class PuzzleCandidateQualityTests
    {
        private const long MaxSafe = 9007199254740991L;
        private static PuzzleCandidateResult Generate(int attempts = 256, EndlessQualityBounds? quality = null)
            => PuzzleCandidateSearch.GenerateAsync(2, new PuzzleCandidateParameters(4, 5, 25, 65, 0, 0, attempts, 4000, 6000, 200, 20, 0),
                Array.Empty<PuzzleDefinition>(), Array.Empty<PuzzleDefinition>(), CancellationToken.None, () => 0, quality).GetAwaiter().GetResult();
        private static int Trivial(PuzzleDefinition p) => p.RowCounts.Count(n => n <= 1 || n == p.Grid.Width) + p.ColumnCounts.Count(n => n <= 1 || n == p.Grid.Height);
        private static EndlessQualityBounds Bounds(QualityInterval[] values) => new EndlessQualityBounds(values[0], values[1], values[2], new QualityInterval(0, 0), values[3]);
        private static QualityInterval[] Wide() => Enumerable.Range(0, 4).Select(_ => new QualityInterval(0, MaxSafe)).ToArray();
        private static PuzzleCandidateResult First()
        {
            var first = Generate(); Assert.That(first.Succeeded, Is.True, first.Status);
            var fresh = PuzzleSolver.Solve(first.Puzzle!, new SolverLimits(1000000, 10000));
            Assert.That(fresh.Classification, Is.EqualTo(SolverClassification.UNIQUE));
            Assert.That(fresh.Metrics!.RequiredGuessDepth, Is.Zero);
            Assert.That(fresh.Metrics.SearchNodes, Is.EqualTo(first.Verification!.Metrics!.SearchNodes));
            Assert.That(fresh.Metrics.DeductionSteps, Is.EqualTo(first.Verification.Metrics.DeductionSteps));
            Assert.That(fresh.Metrics.MaxDeductionDepth, Is.EqualTo(first.Verification.Metrics.MaxDeductionDepth));
            return first;
        }
        [Test]
        public void NullWideAndExactInclusiveRangesPreserveTheActualLegacyCandidate()
        {
            var first = First(); var m = first.Verification!.Metrics!;
            var exact = new[] { new QualityInterval(m.SearchNodes, m.SearchNodes), new QualityInterval(m.DeductionSteps, m.DeductionSteps), new QualityInterval(m.MaxDeductionDepth, m.MaxDeductionDepth), new QualityInterval(Trivial(first.Puzzle!), Trivial(first.Puzzle!)) };
            foreach (var quality in new[] { Bounds(Wide()), Bounds(exact) })
            {
                var bounded = Generate(256, quality);
                Assert.That(bounded.Succeeded, Is.True, bounded.Status);
                Assert.That(bounded.Attempts, Is.EqualTo(first.Attempts));
                Assert.That(PuzzleCandidateSearch.CanonicalSignature(bounded.Puzzle!), Is.EqualTo(PuzzleCandidateSearch.CanonicalSignature(first.Puzzle!)));
            }
        }
        [TestCase(0, false)][TestCase(1, false)][TestCase(1, true)]
        [TestCase(2, false)][TestCase(2, true)][TestCase(3, false)][TestCase(3, true)]
        public void EveryMetricMinimumAndMaximumRejectsARealOtherwiseValidCandidate(int metric, bool maximum)
        {
            var first = First(); var m = first.Verification!.Metrics!;
            long[] observed = { m.SearchNodes, m.DeductionSteps, m.MaxDeductionDepth, Trivial(first.Puzzle!) };
            if (maximum) Assert.That(observed[metric], Is.GreaterThan(0), "This fixture must exercise a real positive upper bound.");
            var intervals = Wide(); intervals[metric] = maximum ? new QualityInterval(0, observed[metric] - 1) : new QualityInterval(observed[metric] + 1, MaxSafe);
            var rejected = Generate(first.Attempts, Bounds(intervals));
            Assert.That(rejected.Succeeded, Is.False); Assert.That(rejected.Puzzle, Is.Null); Assert.That(rejected.Verification, Is.Null);
            Assert.That(rejected.Attempts, Is.EqualTo(first.Attempts)); Assert.That(rejected.Status, Is.EqualTo("GEN_NO_ACCEPTABLE_CANDIDATE"));
        }
        [Test]
        public void ActualZeroSearchNodesAcceptZeroMaximumAndNoNegativeMaximumCanBeConstructed()
        {
            var first = First(); Assert.That(first.Verification!.Metrics!.SearchNodes, Is.Zero);
            var intervals = Wide(); intervals[0] = new QualityInterval(0, 0);
            var accepted = Generate(first.Attempts, Bounds(intervals));
            Assert.That(accepted.Succeeded, Is.True, accepted.Status);
            Assert.That(accepted.Attempts, Is.EqualTo(first.Attempts));
            Assert.That(PuzzleCandidateSearch.CanonicalSignature(accepted.Puzzle!), Is.EqualTo(PuzzleCandidateSearch.CanonicalSignature(first.Puzzle!)));
            Assert.Catch<ArgumentException>(() => new QualityInterval(0, -1));
        }
        [Test]
        public void QualityBoundsCannotTurnAnInterruptedVerifierIntoASeedSkip()
        {
            long clock = 0;
            var result = PuzzleCandidateSearch.GenerateAsync(1, new PuzzleCandidateParameters(4, 5, 25, 65, 0, 0, 256, 4000, 60000, 1, 20, 0),
                Array.Empty<PuzzleDefinition>(), Array.Empty<PuzzleDefinition>(), CancellationToken.None, () => ++clock, Bounds(Wide())).GetAwaiter().GetResult();
            Assert.That(result.Succeeded, Is.False); Assert.That(result.Status, Is.EqualTo("GEN_VERIFICATION_INDETERMINATE")); Assert.That(result.Attempts, Is.EqualTo(1));
        }
        [Test]
        public void QualityBoundsNeverAllowGuessDepthAboveZero()
        {
            var wide = new QualityInterval(0, MaxSafe);
            Assert.Throws<ArgumentException>(() => new EndlessQualityBounds(wide, wide, wide, new QualityInterval(0, 1), wide));
            Assert.Throws<ArgumentException>(() => new EndlessQualityBounds(wide, wide, wide, new QualityInterval(1, 1), wide));
        }
    }
}
