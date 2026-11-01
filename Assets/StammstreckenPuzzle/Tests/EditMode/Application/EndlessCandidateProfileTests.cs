using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using STP.Application;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Application.EditMode
{
    public sealed class EndlessCandidateProfileTests
    {
        private static string Source(string path) => File.ReadAllText(Path.Combine(
            Environment.GetEnvironmentVariable("STP_SOURCE_ROOT") ?? Directory.GetCurrentDirectory(), path));
        private static PuzzleDefinition[] Campaign()
        {
            var bindings = ContractJson.Parse(Source("Assets/StammstreckenPuzzle/UI/MobileCandidate.bindings.json"));
            var corpus = LocalSaveContractFixtures.Construct<IPublicPuzzleCatalog>("STP.Infrastructure.Content",
                "STP.Infrastructure.Content.MobileCandidateData",
                Encoding.UTF8.GetBytes(Source("Assets/StammstreckenPuzzle/UI/MobileCandidate.public.json")),
                bindings.Get("publicCatalogHashSha256").Text);
            return corpus.Puzzles.Select(p => p.ToDomainDefinition()).ToArray();
        }
        private static PuzzleCandidateParameters Profile(string id)
        {
            var row = ContractJson.Parse(Source("Content/Generator/path-search-v1.profiles.json"))
                .Get("profiles").Items.Single(p => p.Get("profileId").Text == id).Get("parameters");
            int N(string key) => checked((int)row.Get(key).Integer);
            return new PuzzleCandidateParameters(N("minSize"), N("maxSize"), N("minDensityPercent"),
                N("maxDensityPercent"), N("minDeductionSteps"), N("minDeductionDepth"), N("maxAttempts"),
                N("pathStateBudget"), N("totalBudgetMs"), N("solverBudgetMs"), N("maxTrivialLines"), N("recentSimilarityPercent"));
        }

        [TestCase("dienstbeginn")]
        [TestCase("regelbetrieb")]
        [TestCase("stoerungslage")]
        [TestCase("grosslage")]
        public void ActualDraftProfilesGenerateDeterministicVerifiedNovelPuzzles(string id)
        {
            var corpus = Campaign(); var profile = Profile(id);
            PuzzleCandidateResult Generate(PuzzleDefinition[] recent) => PuzzleCandidateSearch.GenerateAsync(1,
                profile, corpus, recent, CancellationToken.None).GetAwaiter().GetResult();
            var first = Generate(Array.Empty<PuzzleDefinition>()); var repeat = Generate(Array.Empty<PuzzleDefinition>());
            Assert.That(first.Succeeded, Is.True, id + ":" + first.Status);
            Assert.That(repeat.Succeeded, Is.True, id + ":" + repeat.Status);
            Assert.That(repeat.Attempts, Is.EqualTo(first.Attempts));
            string signature = PuzzleCandidateSearch.CanonicalSignature(first.Puzzle!);
            Assert.That(PuzzleCandidateSearch.CanonicalSignature(repeat.Puzzle!), Is.EqualTo(signature));
            Assert.That(corpus.Select(PuzzleCandidateSearch.CanonicalSignature), Does.Not.Contain(signature));
            var independent = PuzzleSolver.Solve(first.Puzzle, new SolverLimits(1000000, 10000));
            Assert.That(independent.Classification, Is.EqualTo(SolverClassification.UNIQUE));
            Assert.That(independent.Metrics!.RequiredGuessDepth, Is.Zero);
            Assert.That(independent.Metrics.DeductionSteps, Is.GreaterThanOrEqualTo(profile.MinDeductionSteps));
            Assert.That(independent.Metrics.MaxDeductionDepth, Is.GreaterThanOrEqualTo(profile.MinDeductionDepth));
            // A new identity, reversed A/B or a D4 clone must not evade the persisted recent-public guard.
            var q = first.Puzzle!;
            var reversed = PuzzleDefinition.Create(q.Grid, q.B, q.A, q.RowCounts, q.ColumnCounts, q.RulesetVersion).Value!;
            var subsequent = Generate(new[] { reversed });
            if (subsequent.Succeeded)
                Assert.That(PuzzleCandidateSearch.SimilarityDistancePercent(subsequent.Puzzle!, reversed),
                    Is.GreaterThan(profile.RecentSimilarityPercent));
            else Assert.That(subsequent.Status, Is.EqualTo("GEN_NO_ACCEPTABLE_CANDIDATE")
                .Or.EqualTo("GEN_BUDGET_EXHAUSTED").Or.EqualTo("GEN_VERIFICATION_INDETERMINATE"));
        }

        [Test]
        public void LargeProfilePublicTemplateTimeoutCannotSkipToAnotherProposal()
        {
            var profile = new PuzzleCandidateParameters(8, 10, 30, 65, 130, 12, 4096, 50000, 60000, 1, 10, 12);
            long clock = 0;
            var result = PuzzleCandidateSearch.GenerateAsync(1, profile, Campaign(), Array.Empty<PuzzleDefinition>(),
                CancellationToken.None, () => ++clock).GetAwaiter().GetResult();
            Assert.That(result.Succeeded, Is.False); Assert.That(result.Puzzle, Is.Null);
            Assert.That(result.Status, Is.EqualTo("GEN_VERIFICATION_INDETERMINATE"));
            Assert.That(result.Attempts, Is.EqualTo(1));
        }

        [Test]
        public void RootScreenCancellationAndNonMonotonicClockAreInterrupted()
        {
            var publicInput = Campaign()[0];
            using (var cancellation = new CancellationTokenSource())
            {
                cancellation.Cancel();
                Assert.That(PuzzleSolver.ScreenForRootSolution(publicInput,
                    new SolverLimits(1000, 1000, cancellation.Token)), Is.EqualTo(RootSolutionScreen.Interrupted));
            }
            long clock = 10;
            Assert.That(PuzzleSolver.ScreenForRootSolution(publicInput,
                new SolverLimits(1000, 1000, CancellationToken.None, () => clock--)), Is.EqualTo(RootSolutionScreen.Interrupted));
        }
    }
}
