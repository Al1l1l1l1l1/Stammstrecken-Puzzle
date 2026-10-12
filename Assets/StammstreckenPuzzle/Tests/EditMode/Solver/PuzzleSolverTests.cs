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
            Assert.That(result.SolverVersion, Is.EqualTo("solver-v2"));
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

        [TestCase(0, 8, 1, "timeout")] [TestCase(1, 11, 1, "timeout")] [TestCase(2, 51, 5, "timeout")]
        [TestCase(0, 8, 1, "cancellation")] [TestCase(1, 11, 1, "cancellation")] [TestCase(2, 51, 5, "cancellation")]
        [TestCase(0, 8, 1, "clock-backwards")] [TestCase(1, 11, 1, "clock-backwards")] [TestCase(2, 51, 5, "clock-backwards")]
        public void FinalClassification_RechecksTimeoutCancellationAndMonotonicity(int expectedCount, int searchClockReads, int exactStates, string interruption)
        {
            // FIXTURE_ONLY: no path can connect N/0 to W/0 with counts
            // (2,0)/(1,1); (1,0)/(1,0) is the single WN cell; the 3x3
            // Hamiltonian fixture has two paths. The fixed clock scripts leave
            // every search/propagation probe at 1 ms, and change only the next
            // read after search and completion (8/11/51 reads including start).
            // These scripts fail on the old solver, which omits that final read.
            var p = expectedCount == 2 ? Multiple() : Define(2, 2,
                new SmallPathOracle.Edge(0, 0), new SmallPathOracle.Edge(3, 0),
                new[] { expectedCount == 0 ? 2 : 1, 0 }, new[] { 1, expectedCount == 0 ? 1 : 0 });
            var baseline = PuzzleSolver.Solve(p, new SolverLimits(maxStates: exactStates, monotonicNow: () => 1));
            Assert.That(baseline.Classification, Is.EqualTo(Class(expectedCount)), "Exactly consumed state allowance still permits a completed search");
            Assert.That(baseline.SolutionCount, Is.EqualTo(expectedCount));
            Assert.That(PuzzleSolver.Solve(p, new SolverLimits(maxStates: exactStates - 1, monotonicNow: () => 1)).Classification, Is.EqualTo(SolverClassification.INDETERMINATE));
            {
                using var token = new CancellationTokenSource(); int reads = 0;
                long Now()
                {
                    if (++reads <= searchClockReads) return 1;
                    Assert.That(reads, Is.EqualTo(searchClockReads + 1), "Only the final classification probe may observe this interruption");
                    if (interruption == "cancellation") token.Cancel();
                    return interruption == "timeout" ? 11 : interruption == "clock-backwards" ? 0 : 1;
                }
                var result = PuzzleSolver.Solve(p, new SolverLimits(maxStates: exactStates, timeBudgetMs: 10, cancellation: token.Token, monotonicNow: Now));
                Assert.That(result.Classification, Is.EqualTo(SolverClassification.INDETERMINATE), interruption);
                Assert.That(reads, Is.EqualTo(searchClockReads + 1), interruption);
                Assert.That(result.SolutionCount, Is.EqualTo(expectedCount), "Interruption must occur after all completion processing");
                Assert.That(result.Path.Select(c => (c.X, c.Y)), Is.EqualTo(baseline.Path.Select(c => (c.X, c.Y))));
                Assert.That(result.Reductions.Select(r => r.ToString()), Is.EqualTo(baseline.Reductions.Select(r => r.ToString())));
            }
        }

        // FIXTURE_ONLY (QC finding 2 of PR #17). Forced serpentine in an n x n grid: the full rows 0, 2, .., 2m are joined by one
        // connector cell on every odd row, alternating right and left. The root propagation alone solves it, so the single
        // solution carries a long root trace (1639 steps for 32x32, m = 14) whose construction is the post-search work.
        private static PuzzleDefinition Serpentine(int n, int m)
        {
            var rows = new int[n]; var cols = new int[n];
            for (int j = 0; j <= m; j++) { rows[2 * j] = n; for (int x = 0; x < n; x++) cols[x]++; }
            for (int j = 0; j < m; j++) { rows[2 * j + 1] = 1; cols[j % 2 == 0 ? n - 1 : 0]++; }
            return Define(n, n, new SmallPathOracle.Edge(3, 0), new SmallPathOracle.Edge(m % 2 == 0 ? 1 : 3, 2 * m), rows, cols);
        }

        [TestCase(10, 4)] [TestCase(32, 14)]
        public void HardBudget_UniqueNeedsAProbeAfterTheWholeTraceBuild_AtEveryInterruptionKind(int n, int m)
        {
            // The scripted clock stays at 1 ms for every probe but the last and flips only the last one. The last probe is the
            // one after dedup, trace build, metrics and result construction; a flip there must never end as UNIQUE or eligible.
            var def = Serpentine(n, m); int total = 0;
            var baseline = PuzzleSolver.Solve(def, new SolverLimits(maxStates: 5000000, timeBudgetMs: 10, monotonicNow: () => { total++; return 1; }));
            Assert.That(baseline.Classification, Is.EqualTo(SolverClassification.UNIQUE));
            Assert.That(baseline.DeductionTrace.Count, Is.GreaterThan(n), "the trace must be large enough to be real post-search work");
            Assert.That(CampaignGate.Evaluate(baseline), Is.EqualTo(CampaignGateOutcome.ELIGIBLE));
            Assert.That(total, Is.GreaterThan(2));
            foreach (var interruption in new[] { "timeout", "cancellation", "clock-backwards" })
            {
                using var token = new CancellationTokenSource(); int reads = 0;
                long Now()
                {
                    if (++reads < total) return 1;
                    Assert.That(reads, Is.EqualTo(total), "no probe may follow the last one");
                    if (interruption == "cancellation") token.Cancel();
                    return interruption == "timeout" ? 11 : interruption == "clock-backwards" ? 0 : 1;
                }
                var result = PuzzleSolver.Solve(def, new SolverLimits(maxStates: 5000000, timeBudgetMs: 10, cancellation: token.Token, monotonicNow: Now));
                Assert.That(reads, Is.EqualTo(total), interruption);
                Assert.That(result.Classification, Is.EqualTo(SolverClassification.INDETERMINATE), interruption);
                Assert.That(CampaignGate.Evaluate(result), Is.EqualTo(CampaignGateOutcome.INDETERMINATE), interruption);
                Assert.That(result.Metrics, Is.Null, interruption); Assert.That(result.DeductionTrace, Is.Empty, interruption);
                Assert.That(result.SolutionCount, Is.EqualTo(1), "the search itself was complete");
            }
            {
                // The limit is exclusive at the budget: the last probe sitting exactly on it is exhausted, one millisecond earlier is not.
                int reads = 0;
                long FinalAt(long finalNow) { return ++reads < total ? 1 : finalNow; }
                Assert.That(PuzzleSolver.Solve(def, new SolverLimits(maxStates: 5000000, timeBudgetMs: 10, monotonicNow: () => FinalAt(11))).Classification, Is.EqualTo(SolverClassification.INDETERMINATE));
                reads = 0;
                var inside = PuzzleSolver.Solve(def, new SolverLimits(maxStates: 5000000, timeBudgetMs: 10, monotonicNow: () => FinalAt(10)));
                Assert.That(inside.Classification, Is.EqualTo(SolverClassification.UNIQUE));
                Assert.That(inside.DeductionTrace.Count, Is.EqualTo(baseline.DeductionTrace.Count));
            }
            // The state limit shares the fail-closed rule: it is checked before work starts, never after the fact.
            Assert.That(PuzzleSolver.Solve(def, new SolverLimits(maxStates: 1, monotonicNow: () => 1)).Classification, Is.EqualTo(SolverClassification.UNIQUE), "the root alone fits one state");
            Assert.That(PuzzleSolver.Solve(def, new SolverLimits(maxStates: 0, monotonicNow: () => 1)).Classification, Is.EqualTo(SolverClassification.INDETERMINATE));
        }

        [Test]
        public void HardBudget_NoBudgetedWorkRunsBetweenTheLastProbeAndTheReturn()
        {
            // Before the correction the trace build, the metrics and the result object were created after the last probe
            // (about 1.3 ms of 50 ms on the 32x32 serpentine). Wall-clock based, therefore robust by construction: the
            // minimum over repeated runs of (return - last probe) is compared with the minimum of the whole call, so a
            // scheduler hiccup can only raise single samples and a slow machine scales both sides alike.
            var def = Serpentine(32, 14); var sw = new Stopwatch(); long lastProbe = 0;
            long Now() { lastProbe = sw.ElapsedTicks; return lastProbe * 1000 / Stopwatch.Frequency; }
            var limits = new SolverLimits(maxStates: 5000000, timeBudgetMs: 600000, monotonicNow: Now);
            for (int i = 0; i < 3; i++) Assert.That(PuzzleSolver.Solve(def, limits).Classification, Is.EqualTo(SolverClassification.UNIQUE));
            double minTail = double.MaxValue, minTotal = double.MaxValue;
            for (int i = 0; i < 25; i++)
            {
                sw.Restart(); lastProbe = 0;
                var result = PuzzleSolver.Solve(def, limits); long end = sw.ElapsedTicks;
                Assert.That(result.Classification, Is.EqualTo(SolverClassification.UNIQUE));
                minTail = Math.Min(minTail, (end - lastProbe) * 1000.0 / Stopwatch.Frequency); minTotal = Math.Min(minTotal, end * 1000.0 / Stopwatch.Frequency);
            }
            TestContext.WriteLine($"WP024_HARD_BUDGET tail_after_last_probe_ms={minTail:F4} total_ms={minTotal:F3} ratio={minTail / minTotal:P3}");
            Assert.That(minTail, Is.LessThan(minTotal * 0.005), "work after the last budget probe is not covered by the hard budget");
        }

        // FIXTURE_ONLY (second QC of PR #17, finding 1). The 3x3 ambiguity of Multiple() (N/0 to N/2, all counts 3) in the
        // top left corner of a 32x32 grid whose other rows and columns are 0, as reported: MULTIPLE_OR_MORE with a recovery
        // protocol of about 1,200 reductions, all of which were copied into the result after the last budget probe.
        private static PuzzleDefinition AmbiguousCorner32()
        {
            var rows = new int[32]; var cols = new int[32];
            for (int i = 0; i < 3; i++) { rows[i] = 3; cols[i] = 3; }
            return Define(32, 32, new SmallPathOracle.Edge(0, 0), new SmallPathOracle.Edge(0, 2), rows, cols);
        }

        // FIXTURE_ONLY. A long first path with an ambiguous start: A = N/0 enters a 3x3 block that has two Hamiltonian
        // routes from (0,0) to (2,0) (via (1,0)/(1,1)/(0,1) or via (0,1)/(0,2)/../(2,1)), both leaving east along row 0;
        // the path then runs down column 31 to row 4 and continues as a serpentine of full rows 4, 6, .., 28 joined by
        // single connector cells, ending at the W or E border of row 28. Same counts for both block routes, so the result
        // is MULTIPLE_OR_MORE with a first path of several hundred cells.
        private static PuzzleDefinition AmbiguousLongPath32()
        {
            var cells = new List<(int X, int Y)>();
            for (int x = 0; x < 32; x++) cells.Add((x, 0));
            foreach (var c in new[] { (0, 1), (1, 1), (2, 1), (0, 2), (1, 2), (2, 2) }) cells.Add(c);
            for (int y = 1; y <= 3; y++) cells.Add((31, y));
            int last = 4; bool leftward = true;
            for (int y = 4; y <= 28; y += 2)
            {
                for (int x = 0; x < 32; x++) cells.Add((x, y));
                if (y + 2 <= 28) cells.Add((leftward ? 0 : 31, y + 1));
                last = y; leftward = !leftward;
            }
            var rows = new int[32]; var cols = new int[32];
            foreach (var (x, y) in cells) { rows[y]++; cols[x]++; }
            // After the last full row the serpentine leaves at the side opposite to its entry connector.
            int exitSide = leftward ? 1 : 3;
            return Define(32, 32, new SmallPathOracle.Edge(0, 0), new SmallPathOracle.Edge(exitSide, last), rows, cols);
        }

        // FIXTURE_ONLY. The forced 32x32 serpentine with one track moved from column 31 to column 30: the root propagation
        // records about 1,700 reductions before it runs into the contradiction, so the result is a large UNSATISFIABLE one.
        private static PuzzleDefinition UnsatisfiableSerpentine32()
        {
            var def = Serpentine(32, 14); var cols = def.ColumnCounts.ToArray(); cols[31]--; cols[30]++;
            return Define(32, 32, new SmallPathOracle.Edge((int)def.A.Side, def.A.Index), new SmallPathOracle.Edge((int)def.B.Side, def.B.Index), def.RowCounts.ToArray(), cols);
        }

        [Test]
        public void HardBudget_NoResultIsBuiltAfterTheLastProbe_ForEveryResultKind()
        {
            // Second QC of PR #17, finding 1: the first correction built the UNIQUE result before the last probe, but an
            // UNSATISFIABLE, MULTIPLE_OR_MORE or INDETERMINATE result was still constructed after it, copying path and recovery
            // protocol. A definitive verdict could therefore be returned after the budget had run out. Wall clock, because the
            // public API exposes nothing that runs after the last probe: for each result kind with a large result, the time from
            // the last probe to the return, minus the same time for a tiny result (the constant cost of returning, measured in the
            // same run), must stay below a quarter of what building such a result object costs (the copy of path and protocol the
            // old code made after the probe, measured on the same machine). Minima over repeated runs, so scheduler noise only
            // raises single samples.
            var cases = new (string Name, PuzzleDefinition Def, long MaxStates, SolverClassification Expected)[]
            {
                ("tiny unsatisfiable 2x2 (baseline)", Define(2, 2, new SmallPathOracle.Edge(0, 0), new SmallPathOracle.Edge(3, 0), new[] { 2, 0 }, new[] { 1, 1 }), 5000000, SolverClassification.UNSATISFIABLE),
                ("unsatisfiable serpentine 32x32", UnsatisfiableSerpentine32(), 5000000, SolverClassification.UNSATISFIABLE),
                ("ambiguous corner 32x32", AmbiguousCorner32(), 5000000, SolverClassification.MULTIPLE_OR_MORE),
                ("ambiguous long path 32x32", AmbiguousLongPath32(), 5000000, SolverClassification.MULTIPLE_OR_MORE),
                ("interrupted long path 32x32", AmbiguousLongPath32(), 0, SolverClassification.INDETERMINATE),
                ("unique serpentine 32x32", Serpentine(32, 14), 5000000, SolverClassification.UNIQUE)
            };
            var late = new List<string>(); double baseline = double.NaN;
            foreach (var c in cases)
            {
                long maxStates = c.MaxStates;
                if (c.Expected == SolverClassification.INDETERMINATE)
                {
                    // Smallest state allowance that has found the first solution but not finished the search.
                    for (maxStates = 1; maxStates < 10000; maxStates++)
                    {
                        var probe = PuzzleSolver.Solve(c.Def, new SolverLimits(maxStates: maxStates));
                        if (probe.Classification == SolverClassification.INDETERMINATE && probe.SolutionCount == 1) break;
                    }
                    Assert.That(maxStates, Is.LessThan(10000), c.Name);
                }
                var sw = new Stopwatch(); long lastProbe = 0;
                long Now() { lastProbe = sw.ElapsedTicks; return lastProbe * 1000 / Stopwatch.Frequency; }
                var limits = new SolverLimits(maxStates: maxStates, timeBudgetMs: 600000, monotonicNow: Now);
                for (int i = 0; i < 2; i++) Assert.That(PuzzleSolver.Solve(c.Def, limits).Classification, Is.EqualTo(c.Expected), c.Name);
                double minTail = double.MaxValue, minBuild = double.MaxValue; int size = 0;
                for (int i = 0; i < 25; i++)
                {
                    sw.Restart(); lastProbe = 0;
                    var result = PuzzleSolver.Solve(c.Def, limits); long end = sw.ElapsedTicks;
                    Assert.That(result.Classification, Is.EqualTo(c.Expected), c.Name);
                    minTail = Math.Min(minTail, (end - lastProbe) * 1e6 / Stopwatch.Frequency);
                    size = result.Path.Count + result.Reductions.Count;
                    // The work the result constructor does: copy path and protocol into fresh read-only collections.
                    long before = sw.ElapsedTicks;
                    var rebuilt = (Array.AsReadOnly(result.Path.ToArray()), Array.AsReadOnly(result.Reductions.ToArray()));
                    long after = sw.ElapsedTicks; GC.KeepAlive(rebuilt.Item1); GC.KeepAlive(rebuilt.Item2);
                    minBuild = Math.Min(minBuild, (after - before) * 1e6 / Stopwatch.Frequency);
                }
                if (double.IsNaN(baseline))
                {
                    baseline = minTail; Assert.That(size, Is.LessThan(20), c.Name);
                    TestContext.WriteLine($"WP024_HARD_BUDGET_KIND {c.Name}: tail_after_last_probe_us={minTail:F3}");
                    continue;
                }
                double excess = minTail - baseline;
                TestContext.WriteLine($"WP024_HARD_BUDGET_KIND {c.Name}: class={c.Expected} maxStates={maxStates} path+protocol={size} tail_after_last_probe_us={minTail:F3} excess_over_baseline_us={excess:F3} result_build_us={minBuild:F3}");
                Assert.That(size, Is.GreaterThan(1000), c.Name + ": the result must be large enough to make its construction measurable");
                if (!(excess < minBuild * 0.25)) late.Add($"{c.Name}: {excess:F3} us more after the last probe than for a tiny result, building the result takes {minBuild:F3} us");
            }
            Assert.That(late, Is.Empty, "a result object is built after the last budget probe");
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
