using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Solver.EditMode
{
    public sealed class PuzzleSolverTests
    {
        private static PuzzleDefinition Define(int w, int h, SmallPathOracle.Edge a, SmallPathOracle.Edge b, int[] rows, int[] cols)
        {
            var size = GridSize.Create(w, h).Value!;
            return PuzzleDefinition.Create(size, Endpoint.Create(size, (Direction)a.Side, a.Index).Value!, Endpoint.Create(size, (Direction)b.Side, b.Index).Value!, rows, cols, "train-track-v1").Value!;
        }
        private static SolverClassification Class(int count) => count == 0 ? SolverClassification.UNSATISFIABLE : count == 1 ? SolverClassification.UNIQUE : SolverClassification.MULTIPLE_OR_MORE;

        private static bool Valid(int w, int h, SmallPathOracle.Edge a, SmallPathOracle.Edge b, int[] r, int[] c)
        {
            int start = a.Cell(w, h), end = b.Cell(w, h);
            return r.Sum() > 0 && r.Sum() == c.Sum() && r[start / w] > 0 && c[start % w] > 0 && r[end / w] > 0 && c[end % w] > 0;
        }

        private static void Check(PuzzleDefinition def, int expected, string label)
        {
            var result = PuzzleSolver.Solve(def);
            Assert.That(result.Classification, Is.EqualTo(Class(expected)), label);
            Assert.That(result.SolutionCount, Is.EqualTo(expected), label);
            Assert.That(result.SolverVersion, Is.EqualTo("solver-v1"));
            if (expected > 0)
            {
                Assert.That(result.Path[0].X, Is.EqualTo(def.A.Cell.X)); Assert.That(result.Path[0].Y, Is.EqualTo(def.A.Cell.Y));
                Assert.That(result.Path.Last().X, Is.EqualTo(def.B.Cell.X)); Assert.That(result.Path.Last().Y, Is.EqualTo(def.B.Cell.Y));
                Assert.That(result.Path.Select(p => (p.X, p.Y)).Distinct().Count(), Is.EqualTo(result.Path.Count));
                var rows = new int[def.Grid.Height]; var cols = new int[def.Grid.Width];
                for (int i = 0; i < result.Path.Count; i++)
                {
                    var p = result.Path[i]; rows[p.Y]++; cols[p.X]++;
                    if (i > 0) Assert.That(Math.Abs(p.X - result.Path[i - 1].X) + Math.Abs(p.Y - result.Path[i - 1].Y), Is.EqualTo(1));
                }
                Assert.That(rows, Is.EqualTo(def.RowCounts)); Assert.That(cols, Is.EqualTo(def.ColumnCounts));
            }
        }

        private static PuzzleDefinition Rotate(PuzzleDefinition p)
        {
            int w = p.Grid.Width, h = p.Grid.Height;
            SmallPathOracle.Edge Edge(Endpoint e) => new SmallPathOracle.Edge(((int)e.Side + 1) % 4, e.Side == Direction.E || e.Side == Direction.W ? h - 1 - e.Index : e.Index);
            return Define(h, w, Edge(p.A), Edge(p.B), p.ColumnCounts.ToArray(), p.RowCounts.Reverse().ToArray());
        }
        private static PuzzleDefinition Mirror(PuzzleDefinition p)
        {
            SmallPathOracle.Edge Edge(Endpoint e) => new SmallPathOracle.Edge(e.Side == Direction.E ? 3 : e.Side == Direction.W ? 1 : (int)e.Side, e.Side == Direction.N || e.Side == Direction.S ? p.Grid.Width - 1 - e.Index : e.Index);
            return Define(p.Grid.Width, p.Grid.Height, Edge(p.A), Edge(p.B), p.RowCounts.ToArray(), p.ColumnCounts.Reverse().ToArray());
        }

        [TestCase(2, 2)] [TestCase(2, 3)]
        public void Exhaustive_AllOrderedEndpointAndCountCombinations_WithAllDihedralTransforms(int w, int h)
        {
            int checkedCases = 0, invalidCases = 0;
            var edges = SmallPathOracle.Edges(w, h);
            foreach (var a in edges) foreach (var b in edges)
            {
                if (a.Side == b.Side && a.Index == b.Index) continue;
                var oracle = SmallPathOracle.Counts(w, h, a, b);
                foreach (var r in SmallPathOracle.Vectors(h, w)) foreach (var c in SmallPathOracle.Vectors(w, h))
                {
                    var key = SmallPathOracle.Key(r, c);
                    if (!Valid(w, h, a, b, r, c))
                    {
                        var size = GridSize.Create(w, h).Value!;
                        Assert.That(PuzzleDefinition.Create(size, Endpoint.Create(size, (Direction)a.Side, a.Index).Value!, Endpoint.Create(size, (Direction)b.Side, b.Index).Value!, r, c, "train-track-v1").Succeeded, Is.False, key);
                        invalidCases++; continue;
                    }
                    int expected = oracle.TryGetValue(key, out var n) ? n : 0;
                    var p = Define(w, h, a, b, r, c);
                    string label = $"FIXTURE_ONLY {w}x{h} A={a.Side}/{a.Index} B={b.Side}/{b.Index} counts={key}";
                    for (int rot = 0; rot < 4; rot++)
                    {
                        Check(p, expected, label + " rotation=" + rot);
                        Check(Mirror(p), expected, label + " mirror rotation=" + rot);
                        p = Rotate(p);
                    }
                    // Ordered endpoint enumeration already checks the A/B swap separately.
                    checkedCases++;
                }
            }
            TestContext.WriteLine($"ORACLE exhaustive {w}x{h}: {checkedCases} valid definitions x 8 transforms; {invalidCases} invalid definition combinations; all ordered endpoint pairs, transposed 3x2 included.");
            Assert.That(checkedCases, Is.GreaterThan(0));
        }

        [Test]
        public void ThreeByThree_FixedSeed22022_RecordedSubsetAndSwapMetamorphoses()
        {
            var random = new Random(22022); var edges = SmallPathOracle.Edges(3, 3); int checkedCases = 0;
            for (int draw = 0; checkedCases < 256; draw++)
            {
                var a = edges[random.Next(edges.Count)]; var b = edges[random.Next(edges.Count)];
                if (a.Side == b.Side && a.Index == b.Index) continue;
                var r = Enumerable.Range(0, 3).Select(_ => random.Next(4)).ToArray(); var c = Enumerable.Range(0, 3).Select(_ => random.Next(4)).ToArray();
                if (!Valid(3, 3, a, b, r, c)) continue;
                var key = SmallPathOracle.Key(r, c); var oracle = SmallPathOracle.Counts(3, 3, a, b); int expected = oracle.TryGetValue(key, out var n) ? n : 0;
                string label = $"seed=22022; accepted={checkedCases}; draw={draw}; A={a.Side}/{a.Index}; B={b.Side}/{b.Index}; {key}";
                TestContext.WriteLine(label); var p = Define(3, 3, a, b, r, c);
                for (int rot = 0; rot < 4; rot++) { Check(p, expected, label); Check(Mirror(p), expected, label); p = Rotate(p); }
                Check(Define(3, 3, b, a, r, c), expected, label + " swap"); checkedCases++;
            }
        }

        private static PuzzleDefinition Multiple()
        {
            // Discover the first minimal 3x3 ambiguity independently, in a fixed
            // enumeration order, instead of deriving an expected answer from SUT.
            foreach (var a in SmallPathOracle.Edges(3, 3)) foreach (var b in SmallPathOracle.Edges(3, 3))
            {
                if (a.Side == b.Side && a.Index == b.Index) continue;
                foreach (var pair in SmallPathOracle.Counts(3, 3, a, b).OrderBy(p => p.Key, StringComparer.Ordinal))
                {
                    if (pair.Value < 2) continue;
                    var split = pair.Key.Split(';');
                    return Define(3, 3, a, b, split[0].Split(',').Select(int.Parse).ToArray(), split[1].Split(',').Select(int.Parse).ToArray());
                }
            }
            throw new InvalidOperationException("Independent oracle found no 3x3 ambiguity");
        }

        [Test]
        public void Determinism_LimitTwo_ReductionsAndMRVTieBreak()
        {
            var p = Multiple(); var first = PuzzleSolver.Solve(p); Check(p, 2, "independent ambiguity");
            // Independently known Hamiltonian 3x3: N/0 to N/2, all counts 3.
            // The first unresolved y/x cell is (0,0); NS precedes NE, so the
            // canonical first path goes south. Reversed DFS order must fail.
            Assert.That((p.A.Side, p.A.Index, p.B.Side, p.B.Index), Is.EqualTo((Direction.N, 0, Direction.N, 2)));
            Assert.That(first.Path.Select(c => (c.Y, c.X)), Is.EqualTo(new[] { (0, 0), (1, 0), (2, 0), (2, 1), (2, 2), (1, 2), (1, 1), (0, 1), (0, 2) }));
            Assert.That(first.Reductions.Select(r => r.ToString()).Distinct().Count(), Is.EqualTo(first.Reductions.Count));
            var single = Define(2, 2, new SmallPathOracle.Edge(0, 0), new SmallPathOracle.Edge(3, 0), new[] { 1, 0 }, new[] { 1, 0 });
            var ordered = PuzzleSolver.Solve(single).Reductions;
            Assert.That(ordered.Take(4).Select(r => (r.Cell.Y, r.Cell.X, r.Before, r.After)), Is.EqualTo(new[] { (0, 0, (byte)127, (byte)64), (0, 1, (byte)127, (byte)33), (1, 0, (byte)127, (byte)9), (1, 1, (byte)127, (byte)65) }));
            Assert.That(ordered.Skip(4).Select(r => r.RuleCode), Is.All.EqualTo("NEIGHBOR_PORT_REQUIRED"));
            for (int i = 0; i < 10; i++)
            {
                var next = PuzzleSolver.Solve(p);
                Assert.That(next.Path.Select(c => (c.X, c.Y)), Is.EqualTo(first.Path.Select(c => (c.X, c.Y))));
                Assert.That(next.Reductions.Select(r => r.ToString()), Is.EqualTo(first.Reductions.Select(r => r.ToString())));
            }
            // The selector is internal implementation detail, inspected only by tests.
            var select = typeof(PuzzleSolver).GetMethod("SelectMrv", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;
            Assert.That(select.Invoke(null, new object[] { new byte[] { 127, 3, 6, 1 } }), Is.EqualTo(1));
            Assert.That(select.Invoke(null, new object[] { new byte[] { 1, 1 } }), Is.EqualTo(-1));
        }

        [Test]
        public void CancellationTimeoutAndNodeBudget_AreIndeterminateAlsoAfterFirstSolution()
        {
            var p = Multiple(); using var cancelled = new CancellationTokenSource(); cancelled.Cancel();
            Assert.That(PuzzleSolver.Solve(p, new SolverLimits(cancellation: cancelled.Token)).Classification, Is.EqualTo(SolverClassification.INDETERMINATE));
            Assert.That(PuzzleSolver.Solve(p, new SolverLimits(maxStates: 0)).Classification, Is.EqualTo(SolverClassification.INDETERMINATE));
            Assert.That(PuzzleSolver.Solve(p, new SolverLimits(timeBudgetMs: 0)).Classification, Is.EqualTo(SolverClassification.INDETERMINATE));
            bool afterFirst = false;
            for (int limit = 1; limit < 128; limit++)
            {
                var result = PuzzleSolver.Solve(p, new SolverLimits(maxStates: limit));
                if (result.Classification == SolverClassification.INDETERMINATE && result.SolutionCount == 1) { afterFirst = true; break; }
            }
            Assert.That(afterFirst, Is.True, "A first solution must never imply UNIQUE before the rest of search completes");
            bool timedAfterFirst = false, cancelledAfterFirst = false;
            for (int limit = 1; limit < 4096 && (!timedAfterFirst || !cancelledAfterFirst); limit++)
            {
                long ticks = 0;
                var timed = PuzzleSolver.Solve(p, new SolverLimits(timeBudgetMs: limit, monotonicNow: () => ticks++));
                timedAfterFirst |= timed.Classification == SolverClassification.INDETERMINATE && timed.SolutionCount == 1;
                using var token = new CancellationTokenSource(); int calls = 0;
                var stopped = PuzzleSolver.Solve(p, new SolverLimits(cancellation: token.Token, monotonicNow: () => { if (++calls == limit) token.Cancel(); return 0; }));
                cancelledAfterFirst |= stopped.Classification == SolverClassification.INDETERMINATE && stopped.SolutionCount == 1;
            }
            Assert.That(timedAfterFirst && cancelledAfterFirst, Is.True, "Timeout and cancellation after first solution remain INDETERMINATE");
            long tick = 0;
            Assert.That(PuzzleSolver.Solve(p, new SolverLimits(timeBudgetMs: 1, monotonicNow: () => tick++)).Classification, Is.EqualTo(SolverClassification.INDETERMINATE));
            Assert.That(PuzzleSolver.Solve(null).Classification, Is.EqualTo(SolverClassification.INDETERMINATE));
        }

        private static IEnumerable<PuzzleDefinition> BudgetCorpus()
        {
            // FIXTURE_ONLY, not Season-1 content. Three 10-cell straight paths,
            // one 54-cell forced serpentine, and one vertical 10-cell chain.
            foreach (int y in new[] { 0, 4, 9 }) { var r = new int[10]; r[y] = 10; yield return Define(10, 10, new SmallPathOracle.Edge(3, y), new SmallPathOracle.Edge(1, y), r, Enumerable.Repeat(1, 10).ToArray()); }
            var rows = new[] { 10, 1, 10, 1, 10, 1, 10, 1, 10, 0 }; var cols = Enumerable.Repeat(5, 10).ToArray(); cols[0] = 7; cols[9] = 7;
            yield return Define(10, 10, new SmallPathOracle.Edge(3, 0), new SmallPathOracle.Edge(1, 8), rows, cols);
            var columns = new int[10]; columns[5] = 10; yield return Define(10, 10, new SmallPathOracle.Edge(0, 5), new SmallPathOracle.Edge(2, 5), Enumerable.Repeat(1, 10).ToArray(), columns);
        }

        [Test]
        public void Completion_AllTwoByTwoConcreteBoardsAgainstIndependentPaths()
        {
            int evaluated = 0, accepted = 0; var size = GridSize.Create(2, 2).Value!;
            foreach (var a in SmallPathOracle.Edges(2, 2)) foreach (var b in SmallPathOracle.Edges(2, 2))
            {
                if (a.Side == b.Side && a.Index == b.Index) continue;
                var boards = SmallPathOracle.Boards(2, 2, a, b);
                foreach (var values in SmallPathOracle.Vectors(4, 6))
                {
                    var rows = new int[2]; var cols = new int[2];
                    for (int i = 0; i < 4; i++) if (values[i] != 0) { rows[i / 2]++; cols[i % 2]++; }
                    var definition = PuzzleDefinition.Create(size, Endpoint.Create(size, (Direction)a.Side, a.Index).Value!, Endpoint.Create(size, (Direction)b.Side, b.Index).Value!, rows, cols, "train-track-v1");
                    bool expected = boards.Contains(string.Join("", values));
                    bool actual = definition.Succeeded && PuzzleEvaluator.Evaluate(definition.Value, values.Select(v => v == 0 ? CellContent.UNSET : (CellContent)(v + 2)).ToArray()).Completed;
                    Assert.That(actual, Is.EqualTo(expected), $"FIXTURE_ONLY completion board={string.Join("", values)} A={a.Side}/{a.Index} B={b.Side}/{b.Index}");
                    evaluated++; if (actual) accepted++;
                }
            }
            Assert.That(evaluated, Is.EqualTo(134456)); Assert.That(accepted, Is.GreaterThan(0));
            TestContext.WriteLine($"COMPLETION_ORACLE exhaustive 2x2: boards={evaluated}; accepted={accepted}");
        }

        [Test]
        public void TenByTen_FixedCorpus_WarmupAndCIReferenceBudget()
        {
            var corpus = BudgetCorpus().ToArray();
            foreach (var p in corpus) { Check(p, 1, "10x10 known forced path"); for (int i = 0; i < 5; i++) PuzzleSolver.Solve(p); }
            var samples = new List<double>(); var sw = new Stopwatch();
            for (int i = 0; i < 50; i++) foreach (var p in corpus) { sw.Restart(); var result = PuzzleSolver.Solve(p); sw.Stop(); Assert.That(result.Classification, Is.EqualTo(SolverClassification.UNIQUE)); samples.Add(sw.Elapsed.TotalMilliseconds); }
            samples.Sort(); double p95 = samples[(int)Math.Ceiling(samples.Count * .95) - 1], max = samples.Last();
            TestContext.WriteLine($"WP022_BUDGET corpus=10x10-5-forced-paths-v1 warmup=5 repeats=50 samples={samples.Count} p95_ms={p95:F3} max_ms={max:F3} machine={Environment.MachineName} os={Environment.OSVersion} runtime={Environment.Version} commit={Environment.GetEnvironmentVariable("GITHUB_SHA") ?? "LOCAL_WORKTREE"}");
            Assert.That(p95, Is.LessThan(250)); Assert.That(max, Is.LessThan(2000));
        }
    }
}
