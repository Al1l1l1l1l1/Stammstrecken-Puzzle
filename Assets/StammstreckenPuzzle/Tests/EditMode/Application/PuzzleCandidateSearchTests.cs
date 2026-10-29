using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Application.EditMode
{
    public sealed class PuzzleCandidateSearchTests
    {
        private static PuzzleCandidateParameters Parameters() => new PuzzleCandidateParameters(4, 5, 25, 65, 0, 0, 256, 4000, 6000, 200, 20, 0);

        [Test]
        public void SameSeed_IsReproducible_AndPublicOnlyVerificationIsUniqueWithoutGuessing()
        {
            var a = PuzzleCandidateSearch.GenerateAsync(2, Parameters(), Array.Empty<PuzzleDefinition>(), Array.Empty<PuzzleDefinition>(), CancellationToken.None).GetAwaiter().GetResult();
            var b = PuzzleCandidateSearch.GenerateAsync(2, Parameters(), Array.Empty<PuzzleDefinition>(), Array.Empty<PuzzleDefinition>(), CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(a.Succeeded, Is.True, a.Status); Assert.That(b.Succeeded, Is.True, b.Status);
            Assert.That(PuzzleCandidateSearch.CanonicalSignature(a.Puzzle!), Is.EqualTo(PuzzleCandidateSearch.CanonicalSignature(b.Puzzle!)));
            Assert.That(a.Attempts, Is.EqualTo(b.Attempts));
            var independent = PuzzleSolver.Solve(a.Puzzle, new SolverLimits(1000000, 10000));
            Assert.That(independent.Classification, Is.EqualTo(SolverClassification.UNIQUE));
            Assert.That(independent.Metrics!.RequiredGuessDepth, Is.Zero);
        }

        [Test]
        public void NoveltyRejectsAllRotationsMirrorsAndReversedEndpoints_IncludingRectangularGrids()
        {
            var result = PuzzleCandidateSearch.GenerateAsync(4, Parameters(), Array.Empty<PuzzleDefinition>(), Array.Empty<PuzzleDefinition>(), CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(result.Succeeded, Is.True, result.Status);
            var source = result.Puzzle!; var current = source;
            for (int rotation = 0; rotation < 4; rotation++)
            {
                foreach (var variant in new[] { current, Mirror(current), Reverse(current), Reverse(Mirror(current)) })
                {
                    Assert.That(PuzzleCandidateSearch.CanonicalSignature(variant), Is.EqualTo(PuzzleCandidateSearch.CanonicalSignature(source)));
                    Assert.That(PuzzleCandidateSearch.SimilarityDistancePercent(source, variant), Is.Zero);
                }
                current = Rotate(current);
            }
        }

        [Test]
        public void CampaignAndRecentWindowCannotReissueSamePublicPuzzleUnderANewIdentity()
        {
            var first = PuzzleCandidateSearch.GenerateAsync(1, Parameters(), Array.Empty<PuzzleDefinition>(), Array.Empty<PuzzleDefinition>(), CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(first.Succeeded, Is.True, first.Status);
            foreach (bool campaign in new[] { true, false })
            {
                var result = PuzzleCandidateSearch.GenerateAsync(1, Parameters(), campaign ? new[] { first.Puzzle! } : Array.Empty<PuzzleDefinition>(),
                    campaign ? Array.Empty<PuzzleDefinition>() : new[] { Mirror(first.Puzzle!) }, CancellationToken.None).GetAwaiter().GetResult();
                if (result.Succeeded) Assert.That(PuzzleCandidateSearch.CanonicalSignature(result.Puzzle!), Is.Not.EqualTo(PuzzleCandidateSearch.CanonicalSignature(first.Puzzle!)));
                else Assert.That(result.Status, Is.EqualTo("GEN_BUDGET_EXHAUSTED").Or.EqualTo("GEN_NO_ACCEPTABLE_CANDIDATE").Or.EqualTo("GEN_VERIFICATION_INDETERMINATE"));
            }
        }

        [Test]
        public void CancelledAndExhaustedSearchNeverClaimAPlayableCandidate()
        {
            using (var cancellation = new CancellationTokenSource())
            {
                cancellation.Cancel();
                Assert.Throws<TaskCanceledException>(() => PuzzleCandidateSearch.GenerateAsync(1, Parameters(),
                    Array.Empty<PuzzleDefinition>(), Array.Empty<PuzzleDefinition>(), cancellation.Token).GetAwaiter().GetResult());
            }
        }

        [Test]
        public void SlowVerificationClockStopsTheWholeAttemptInsteadOfChoosingALaterCandidate()
        {
            var bounded=new PuzzleCandidateParameters(4,5,25,65,0,0,256,4000,60000,1,20,0);
            long ticks=0;
            var result=PuzzleCandidateSearch.GenerateAsync(1,bounded,Array.Empty<PuzzleDefinition>(),Array.Empty<PuzzleDefinition>(),
                CancellationToken.None,()=>++ticks).GetAwaiter().GetResult();
            Assert.That(result.Succeeded,Is.False);Assert.That(result.Puzzle,Is.Null);
            Assert.That(result.Status,Is.EqualTo("GEN_VERIFICATION_INDETERMINATE"));
            Assert.That(result.Attempts,Is.EqualTo(1),"The first verifier timeout must not advance to another candidate.");
        }

        [Test]
        public void ImpossibleQualityBudgetTerminatesWithExplicitNonSuccess()
        {
            var strict = new PuzzleCandidateParameters(4, 4, 25, 65, 100000, 100000, 2, 64, 100, 10, 20, 0);
            var result = PuzzleCandidateSearch.GenerateAsync(5, strict, Array.Empty<PuzzleDefinition>(), Array.Empty<PuzzleDefinition>(), CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(result.Succeeded, Is.False); Assert.That(result.Puzzle, Is.Null); Assert.That(result.Verification, Is.Null);
            Assert.That(result.Attempts, Is.LessThanOrEqualTo(2));
        }

        private static PuzzleDefinition Reverse(PuzzleDefinition p) => PuzzleDefinition.Create(p.Grid, p.B, p.A, p.RowCounts, p.ColumnCounts, p.RulesetVersion).Value!;
        private static PuzzleDefinition Rotate(PuzzleDefinition p)
        {
            var grid = GridSize.Create(p.Grid.Height, p.Grid.Width).Value!;
            Endpoint Transform(Endpoint e)
            {
                int x = p.Grid.Height - 1 - e.Cell.Y, y = e.Cell.X; var side = (Direction)(((int)e.Side + 1) & 3);
                return Endpoint.Create(grid, side, side == Direction.N || side == Direction.S ? x : y).Value!;
            }
            return PuzzleDefinition.Create(grid, Transform(p.A), Transform(p.B), p.ColumnCounts, p.RowCounts.Reverse().ToArray(), p.RulesetVersion).Value!;
        }
        private static PuzzleDefinition Mirror(PuzzleDefinition p)
        {
            Endpoint Transform(Endpoint e)
            {
                var side = e.Side == Direction.E ? Direction.W : e.Side == Direction.W ? Direction.E : e.Side;
                int index = side == Direction.N || side == Direction.S ? p.Grid.Width - 1 - e.Cell.X : e.Cell.Y;
                return Endpoint.Create(p.Grid, side, index).Value!;
            }
            return PuzzleDefinition.Create(p.Grid, Transform(p.A), Transform(p.B), p.RowCounts, p.ColumnCounts.Reverse().ToArray(), p.RulesetVersion).Value!;
        }
    }
}
