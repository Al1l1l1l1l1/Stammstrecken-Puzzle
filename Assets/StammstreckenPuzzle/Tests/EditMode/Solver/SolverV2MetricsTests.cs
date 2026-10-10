using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Solver.EditMode
{
    // WP-024 Phase B: solver-v2 identity, root deduction trace, ADR-031 metrics and the campaign gate.
    public sealed class SolverV2MetricsTests
    {
        private static string Fmt(DeductionStep s)
        {
            string V(IEnumerable<CandidateValue> v) => string.Join("+", v.Select(x => x.ToString().Replace("TRACK_", "")));
            string P(DeductionPremise q) => q.IsPublicFact
                ? (q.PublicFact == PublicFactKind.GRID_SIZE ? "grid" : q.PublicFact == PublicFactKind.ENDPOINT_A ? "A" : q.PublicFact == PublicFactKind.ENDPOINT_B ? "B" : q.PublicFact == PublicFactKind.ROW_COUNT ? "row" + q.LineIndex : "col" + q.LineIndex)
                : "#" + q.TraceIndex;
            return $"{s.RuleCode}@{s.Cell.X},{s.Cell.Y} -{V(s.RemovedValues)} ={V(s.RemainingValues)} <{string.Join(" ", s.Premises.Select(P))}> d{s.Depth}";
        }

        private static int OracleCount(PuzzleDefinition def)
        {
            var oracle = SmallPathOracle.Counts(def.Grid.Width, def.Grid.Height, new SmallPathOracle.Edge((int)def.A.Side, def.A.Index), new SmallPathOracle.Edge((int)def.B.Side, def.B.Index));
            return oracle.TryGetValue(SmallPathOracle.Key(def.RowCounts.ToArray(), def.ColumnCounts.ToArray()), out var n) ? n : 0;
        }

        private static PuzzleDefinition Ambiguous()
        {
            // First minimal 3x3 ambiguity from the independent oracle in a fixed enumeration order (the Hamiltonian N/0 to N/2 fixture).
            foreach (var a in SmallPathOracle.Edges(3, 3)) foreach (var b in SmallPathOracle.Edges(3, 3))
            {
                if (a.Side == b.Side && a.Index == b.Index) continue;
                foreach (var pair in SmallPathOracle.Counts(3, 3, a, b).OrderBy(p => p.Key, StringComparer.Ordinal))
                {
                    if (pair.Value < 2) continue;
                    var split = pair.Key.Split(';');
                    return TraceAudit.Define(3, 3, a, b, split[0].Split(',').Select(int.Parse).ToArray(), split[1].Split(',').Select(int.Parse).ToArray());
                }
            }
            throw new InvalidOperationException("Independent oracle found no 3x3 ambiguity");
        }

        private static IEnumerable<PuzzleDefinition> TenByTenCorpus()
        {
            // FIXTURE_ONLY, same shapes as the WP-022 budget corpus: forced straight paths, a 54-cell serpentine and a vertical chain.
            foreach (int y in new[] { 0, 4, 9 }) { var r = new int[10]; r[y] = 10; yield return SolverV2Fixtures.D(10, 10, 3, y, 1, y, r, Enumerable.Repeat(1, 10).ToArray()); }
            var rows = new[] { 10, 1, 10, 1, 10, 1, 10, 1, 10, 0 }; var cols = Enumerable.Repeat(5, 10).ToArray(); cols[0] = 7; cols[9] = 7;
            yield return SolverV2Fixtures.D(10, 10, 3, 0, 1, 8, rows, cols);
            var columns = new int[10]; columns[5] = 10; yield return SolverV2Fixtures.D(10, 10, 0, 5, 2, 5, Enumerable.Repeat(1, 10).ToArray(), columns);
        }

        private static (int Nodes, int Steps, int Depth, int Guess) Quad(SolverMetrics m) => (m.SearchNodes, m.DeductionSteps, m.MaxDeductionDepth, m.RequiredGuessDepth);

        // AK-02
        [Test]
        public void SolverVersion_IsSolverV2_ForTheConstantAndEveryResultKind()
        {
            Assert.That(PuzzleSolver.SolverVersion, Is.EqualTo("solver-v2"));
            var results = new[]
            {
                PuzzleSolver.Solve(SolverV2Fixtures.TopPair()), PuzzleSolver.Solve(SolverV2Fixtures.Unsatisfiable()), PuzzleSolver.Solve(Ambiguous()),
                PuzzleSolver.Solve(SolverV2Fixtures.TopPair(), new SolverLimits(maxStates: 0)), PuzzleSolver.Solve(null)
            };
            Assert.That(results.Select(r => r.Classification), Is.EqualTo(new[] { SolverClassification.UNIQUE, SolverClassification.UNSATISFIABLE, SolverClassification.MULTIPLE_OR_MORE, SolverClassification.INDETERMINATE, SolverClassification.INDETERMINATE }));
            Assert.That(results.Select(r => r.SolverVersion), Is.All.EqualTo("solver-v2"));
        }

        // AK-03, AK-04, AK-07: hand-derived literal traces of two purely deductive references.
        [Test]
        public void PureDeductive_SingleCell_HasExactTraceZeroSearchAndRecomputableMetrics()
        {
            // A=N/0 and B=W/0 are the same corner cell. Hand derivation: that cell must open to N and W (WN) [#0];
            // the three border cells keep only the shapes that do not leave the grid [#1..#3]; (1,0) and (0,1) then
            // lose the shape that would need an E/S port of the WN cell [#4,#5]; (1,1) loses WN because its N
            // neighbour (1,0) is empty, which needs both of its entries (#1 removes the S shapes NS/ES, #4 SW).
            var def = SolverV2Fixtures.SingleCell(); var result = PuzzleSolver.Solve(def);
            Assert.That(result.Classification, Is.EqualTo(SolverClassification.UNIQUE)); Assert.That(OracleCount(def), Is.EqualTo(1));
            Assert.That(Quad(result.Metrics!), Is.EqualTo((0, 7, 3, 0)));
            Assert.That(result.DeductionTrace.Select(Fmt), Is.EqualTo(new[]
            {
                "ENDPOINT_ENTRY_REQUIRED@0,0 -EMPTY+NS+EW+NE+ES+SW =WN <grid A B> d1",
                "BORDER_PORT_FORBIDDEN@1,0 -NS+EW+NE+ES+WN =EMPTY+SW <grid> d1",
                "BORDER_PORT_FORBIDDEN@0,1 -NS+EW+ES+SW+WN =EMPTY+NE <grid> d1",
                "BORDER_PORT_FORBIDDEN@1,1 -NS+EW+NE+ES+SW =EMPTY+WN <grid> d1",
                "NEIGHBOR_PORT_REQUIRED@1,0 -SW =EMPTY <grid #0> d2",
                "NEIGHBOR_PORT_REQUIRED@0,1 -NE =EMPTY <grid #0> d2",
                "NEIGHBOR_PORT_REQUIRED@1,1 -WN =EMPTY <grid #1 #4> d3"
            }));
            Assert.That(result.Reductions.Count, Is.EqualTo(7), "A purely deductive run has no branch reductions");
            Assert.That(result.Path.Select(c => (c.X, c.Y)), Is.EqualTo(new[] { (0, 0) }));
            TraceAudit.AssertWellFormedAndSound(def, result, "single cell");
        }

        [Test]
        public void PureDeductive_TopPair_HasExactTraceZeroSearchAndRecomputableMetrics()
        {
            // N/0 -> N/1 on 2x2, rows (2,0): both top cells must open to the north [#0,#1], the bottom cells keep
            // EMPTY or their border-legal shape [#2,#3], row 1 has target 0 [#4,#5] and with it the south-facing
            // top shapes disappear [#6,#7]. The root alone solves the puzzle.
            var def = SolverV2Fixtures.TopPair(); var result = PuzzleSolver.Solve(def);
            Assert.That(result.Classification, Is.EqualTo(SolverClassification.UNIQUE)); Assert.That(OracleCount(def), Is.EqualTo(1));
            Assert.That(Quad(result.Metrics!), Is.EqualTo((0, 8, 2, 0)));
            Assert.That(result.DeductionTrace.Select(Fmt), Is.EqualTo(new[]
            {
                "ENDPOINT_ENTRY_REQUIRED@0,0 -EMPTY+EW+ES+SW+WN =NS+NE <grid A> d1",
                "ENDPOINT_ENTRY_REQUIRED@1,0 -EMPTY+EW+NE+ES+SW =NS+WN <grid B> d1",
                "BORDER_PORT_FORBIDDEN@0,1 -NS+EW+ES+SW+WN =EMPTY+NE <grid> d1",
                "BORDER_PORT_FORBIDDEN@1,1 -NS+EW+NE+ES+SW =EMPTY+WN <grid> d1",
                "LINE_ZERO@0,1 -NE =EMPTY <row1> d1",
                "LINE_ZERO@1,1 -WN =EMPTY <row1> d1",
                "NEIGHBOR_PORT_REQUIRED@0,0 -NS =NE <grid #2 #4> d2",
                "NEIGHBOR_PORT_REQUIRED@1,0 -NS =WN <grid #3 #5> d2"
            }));
            Assert.That(result.Reductions.Count, Is.EqualTo(8));
            Assert.That(result.Path.Select(c => (c.X, c.Y)), Is.EqualTo(new[] { (0, 0), (1, 0) }));
            TraceAudit.AssertWellFormedAndSound(def, result, "top pair");
        }

        // AK-04: without any root step the maximum depth is 0 (a real puzzle always has border steps, so the metric
        // contract is exercised on the internal factory that builds the metrics from the trace alone).
        [Test]
        public void NoRootStep_GivesMaxDeductionDepthZero_AndSearchMetricsStayIndependent()
        {
            var factory = typeof(SolverMetrics).GetMethod("FromTrace", BindingFlags.NonPublic | BindingFlags.Static)!;
            var metrics = (SolverMetrics)factory.Invoke(null, new object[] { 3, Array.Empty<DeductionStep>(), 1 })!;
            Assert.That(Quad(metrics), Is.EqualTo((3, 0, 0, 1)));
            Assert.That(TraceAudit.Recompute(Array.Empty<DeductionStep>()), Is.EqualTo((0, 0)));
        }

        // AK-05, AK-06: reference cases whose search discards branches.
        [Test]
        public void SearchCases_CountEveryAssumption_ButOnlyRootDeductionsAndOnlyActivePathDepth()
        {
            // (puzzle, searchNodes, deductionSteps, maxDeductionDepth, requiredGuessDepth, recovery-protocol length)
            var cases = new (string Name, PuzzleDefinition Def, int Nodes, int Steps, int Depth, int Guess, int Recovery)[]
            {
                // 2x3 snake: the first open cell (0,0) {NS,NE}: NS leads to the solution (one assumption), NE is then discarded.
                ("snake2x3", SolverV2Fixtures.Snake2x3(), 2, 12, 2, 1, 18),
                // First open cell (1,0) {EW,SW}: the EW subtree (8 nested assumptions, five levels deep) is discarded,
                // SW is the solution without further assumption. Nodes 10 = 9 in/after EW + 1; the path depth stays 1.
                ("deep-discard", SolverV2Fixtures.DeepDiscard4x4(), 10, 20, 2, 1, 70),
                // (1,0)=NE then (2,1)=SW: two active assumptions; NS and EW were discarded siblings.
                ("guess2", SolverV2Fixtures.Guess2(), 4, 36, 5, 2, 49),
                // (0,0)=NS, (1,0)=NE, (0,1)=NE: three active assumptions; two discarded siblings and the trailing NE of (0,0).
                ("guess3", SolverV2Fixtures.Guess3(), 6, 34, 4, 3, 54)
            };
            foreach (var c in cases)
            {
                var result = PuzzleSolver.Solve(c.Def);
                Assert.That(OracleCount(c.Def), Is.EqualTo(1), c.Name);
                Assert.That(result.Classification, Is.EqualTo(SolverClassification.UNIQUE), c.Name);
                Assert.That(Quad(result.Metrics!), Is.EqualTo((c.Nodes, c.Steps, c.Depth, c.Guess)), c.Name);
                Assert.That(result.Metrics!.SearchNodes, Is.GreaterThan(result.Metrics.RequiredGuessDepth), c.Name + ": discarded assumptions are counted as nodes but not as depth");
                Assert.That(result.Reductions.Count, Is.EqualTo(c.Recovery), c.Name);
                Assert.That(result.Reductions.Count, Is.GreaterThan(result.DeductionTrace.Count), c.Name + ": branch reductions exist but never become deduction steps");
                TraceAudit.AssertWellFormedAndSound(c.Def, result, c.Name);

                // Independent root-only recomputation (no DFS at all) reproduces exactly the published trace.
                Assert.That(RootOnlyTrace(c.Def).Select(Fmt), Is.EqualTo(result.DeductionTrace.Select(Fmt)), c.Name);

                // The state budget counts the root plus every begun assumption: the smallest sufficient budget is nodes + 1.
                int minimal = -1;
                for (int limit = 1; limit <= c.Nodes + 2 && minimal < 0; limit++)
                    if (PuzzleSolver.Solve(c.Def, new SolverLimits(maxStates: limit)).Classification == SolverClassification.UNIQUE) minimal = limit;
                Assert.That(minimal, Is.EqualTo(c.Nodes + 1), c.Name);
            }
        }

        private static IReadOnlyList<DeductionStep> RootOnlyTrace(PuzzleDefinition def)
        {
            var tracerType = TraceAudit.Tracer;
            var tracer = Activator.CreateInstance(tracerType, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { def }, null)!;
            var runStage = typeof(PuzzleSolver).GetMethod("RunStageCore", BindingFlags.NonPublic | BindingFlags.Static)!;
            var domains = Enumerable.Repeat((byte)127, def.Grid.CellCount).ToArray(); var sink = new List<RecoveryReduction>();
            var work = new SortedSet<int>(Enumerable.Range(0, 6));
            while (work.Count > 0)
            {
                int stage = work.Min; work.Remove(stage); int before = sink.Count;
                Assert.That((bool)runStage.Invoke(null, new object[] { def, domains, sink, stage, tracer })!, Is.True, "root propagation of a UNIQUE puzzle never contradicts");
                if (sink.Count != before) foreach (int dirty in Enumerable.Range(0, 6)) work.Add(dirty);
            }
            return (IReadOnlyList<DeductionStep>)tracerType.GetMethod("Build", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(tracer, null)!;
        }

        // AK-03/AK-07/AK-09: every UNIQUE result of all small definitions carries a sound, re-derivable, irredundant trace.
        [Test]
        public void Exhaustive_SmallShapes_TraceIsSoundAndMetricsFollowFromIt_OnlyUniqueExposesMetrics()
        {
            int total = 0, unique = 0, rootOnly = 0, oneAssumption = 0;
            foreach (var (w, h) in new[] { (2, 2), (2, 3), (3, 2), (3, 3) })
            {
                var edges = SmallPathOracle.Edges(w, h);
                foreach (var a in edges) foreach (var b in edges)
                {
                    if (a.Side == b.Side && a.Index == b.Index) continue;
                    var oracle = SmallPathOracle.Counts(w, h, a, b);
                    foreach (var r in SmallPathOracle.Vectors(h, w)) foreach (var c in SmallPathOracle.Vectors(w, h))
                    {
                        int start = a.Cell(w, h), end = b.Cell(w, h);
                        if (!(r.Sum() > 0 && r.Sum() == c.Sum() && r[start / w] > 0 && c[start % w] > 0 && r[end / w] > 0 && c[end % w] > 0)) continue;
                        var key = SmallPathOracle.Key(r, c); int expected = oracle.TryGetValue(key, out var n) ? n : 0;
                        var def = TraceAudit.Define(w, h, a, b, r, c); var result = PuzzleSolver.Solve(def); total++;
                        string label = $"{w}x{h} A={a.Side}/{a.Index} B={b.Side}/{b.Index} {key}";
                        Assert.That(result.SolverVersion, Is.EqualTo("solver-v2"), label);
                        if (expected != 1)
                        {
                            Assert.That(result.Metrics, Is.Null, label); Assert.That(result.DeductionTrace, Is.Empty, label);
                            Assert.That(CampaignGate.Evaluate(result), Is.EqualTo(CampaignGateOutcome.NOT_UNIQUE), label);
                            continue;
                        }
                        unique++;
                        TraceAudit.AssertWellFormedAndSound(def, result, label);
                        if (result.Metrics!.RequiredGuessDepth == 0) { rootOnly++; Assert.That(CampaignGate.IsEligible(result), Is.True, label); }
                        else { oneAssumption++; Assert.That(result.Metrics.RequiredGuessDepth, Is.EqualTo(1), label); Assert.That(CampaignGate.Evaluate(result), Is.EqualTo(CampaignGateOutcome.GUESS_DEPTH_REQUIRED), label); }
                    }
                }
            }
            TestContext.WriteLine($"SOLVER_V2_EXHAUSTIVE definitions={total} unique={unique} rootOnly={rootOnly} oneAssumption={oneAssumption}");
            Assert.That((total, unique, rootOnly, oneAssumption), Is.EqualTo((39664, 1800, 1600, 200)));
        }

        // AK-03/AK-09: deterministic 4x4 sample with deeper search; all classification results are checked against the oracle.
        [Test]
        public void SeededFourByFour_Sample_TraceIsSoundAndSearchMetricsAreConsistent()
        {
            var random = new Random(24024); var edges = SmallPathOracle.Edges(4, 4); var cache = new Dictionary<(int, int, int, int), Dictionary<string, int>>();
            int checkedCases = 0, deepest = 0;
            for (int draw = 0; checkedCases < 400 && draw < 100000; draw++)
            {
                var a = edges[random.Next(edges.Count)]; var b = edges[random.Next(edges.Count)];
                if (a.Side == b.Side && a.Index == b.Index) continue;
                var pair = (a.Side, a.Index, b.Side, b.Index);
                if (!cache.TryGetValue(pair, out var oracle)) cache[pair] = oracle = SmallPathOracle.Counts(4, 4, a, b);
                var keys = oracle.Keys.OrderBy(k => k, StringComparer.Ordinal).ToArray(); var key = keys[random.Next(keys.Length)];
                var parts = key.Split(';'); var rows = parts[0].Split(',').Select(int.Parse).ToArray(); var cols = parts[1].Split(',').Select(int.Parse).ToArray();
                var def = TraceAudit.Define(4, 4, a, b, rows, cols); var result = PuzzleSolver.Solve(def);
                string label = $"seed=24024 draw={draw} A={a.Side}/{a.Index} B={b.Side}/{b.Index} {key}";
                Assert.That(result.Classification, Is.EqualTo(oracle[key] == 1 ? SolverClassification.UNIQUE : SolverClassification.MULTIPLE_OR_MORE), label);
                if (oracle[key] == 1) { TraceAudit.AssertWellFormedAndSound(def, result, label); deepest = Math.Max(deepest, result.Metrics!.RequiredGuessDepth); }
                else { Assert.That(result.Metrics, Is.Null, label); Assert.That(result.DeductionTrace, Is.Empty, label); }
                checkedCases++;
            }
            TestContext.WriteLine($"SOLVER_V2_SEEDED_4X4 cases={checkedCases} deepestRequiredGuessDepth={deepest}");
            Assert.That(checkedCases, Is.EqualTo(400)); Assert.That(deepest, Is.GreaterThanOrEqualTo(2), "The sample must contain nested assumptions");
        }

        // AK-08/AK-09: no metric or trace exists for any non-UNIQUE result, in particular not after a limit hit.
        [Test]
        public void NonUniqueAndLimitedResults_ExposeNeitherMetricsNorTrace_AndNeverPassTheGate()
        {
            Assert.That(PuzzleSolver.Solve(null).Metrics, Is.Null);
            foreach (var def in new[] { SolverV2Fixtures.Snake2x3(), SolverV2Fixtures.Guess3(), Ambiguous(), SolverV2Fixtures.Unsatisfiable() })
            {
                var full = PuzzleSolver.Solve(def); int needed = full.Metrics == null ? 0 : full.Metrics.SearchNodes + 1;
                bool anyAfterFirst = false;
                for (int limit = 0; limit <= needed + 1; limit++)
                {
                    var r = PuzzleSolver.Solve(def, new SolverLimits(maxStates: limit));
                    if (r.Classification == SolverClassification.INDETERMINATE)
                    {
                        Assert.That(r.Metrics, Is.Null); Assert.That(r.DeductionTrace, Is.Empty);
                        Assert.That(CampaignGate.Evaluate(r), Is.EqualTo(CampaignGateOutcome.INDETERMINATE)); anyAfterFirst |= r.SolutionCount == 1;
                    }
                    else { Assert.That(r.Metrics == null, Is.EqualTo(r.Classification != SolverClassification.UNIQUE)); }
                }
                if (full.Classification == SolverClassification.UNIQUE) Assert.That(anyAfterFirst, Is.True, "a found solution with unfinished search must stay INDETERMINATE without metrics");
                using var cancelled = new CancellationTokenSource(); cancelled.Cancel();
                var stopped = PuzzleSolver.Solve(def, new SolverLimits(cancellation: cancelled.Token));
                Assert.That((stopped.Classification, stopped.Metrics, stopped.DeductionTrace.Count), Is.EqualTo((SolverClassification.INDETERMINATE, (SolverMetrics?)null, 0)));
                Assert.That(CampaignGate.Evaluate(stopped), Is.EqualTo(CampaignGateOutcome.INDETERMINATE));
                var timed = PuzzleSolver.Solve(def, new SolverLimits(timeBudgetMs: 0));
                Assert.That((timed.Classification, timed.Metrics), Is.EqualTo((SolverClassification.INDETERMINATE, (SolverMetrics?)null)));
                for (int budget = 1; budget < 200; budget++)
                {
                    long tick = 0; var r = PuzzleSolver.Solve(def, new SolverLimits(timeBudgetMs: budget, monotonicNow: () => tick++));
                    if (r.Classification == SolverClassification.INDETERMINATE) { Assert.That(r.Metrics, Is.Null); Assert.That(CampaignGate.IsEligible(r), Is.False); }
                }
            }
        }

        // AK-08
        [Test]
        public void CampaignGate_AcceptsOnlyUniqueRootOnlySolutions()
        {
            foreach (var def in new[] { SolverV2Fixtures.SingleCell(), SolverV2Fixtures.TopPair() }.Concat(TenByTenCorpus()))
                Assert.That(CampaignGate.Evaluate(PuzzleSolver.Solve(def)), Is.EqualTo(CampaignGateOutcome.ELIGIBLE));
            foreach (var def in new[] { SolverV2Fixtures.Snake2x3(), SolverV2Fixtures.DeepDiscard4x4(), SolverV2Fixtures.Guess2(), SolverV2Fixtures.Guess3() })
            {
                var result = PuzzleSolver.Solve(def);
                Assert.That(result.Classification, Is.EqualTo(SolverClassification.UNIQUE));
                Assert.That(CampaignGate.Evaluate(result), Is.EqualTo(CampaignGateOutcome.GUESS_DEPTH_REQUIRED)); Assert.That(CampaignGate.IsEligible(result), Is.False);
            }
            Assert.That(CampaignGate.Evaluate(PuzzleSolver.Solve(SolverV2Fixtures.Unsatisfiable())), Is.EqualTo(CampaignGateOutcome.NOT_UNIQUE));
            Assert.That(CampaignGate.Evaluate(PuzzleSolver.Solve(Ambiguous())), Is.EqualTo(CampaignGateOutcome.NOT_UNIQUE));
            Assert.That(CampaignGate.Evaluate(PuzzleSolver.Solve(SolverV2Fixtures.TopPair(), new SolverLimits(maxStates: 0))), Is.EqualTo(CampaignGateOutcome.INDETERMINATE));
            Assert.That(CampaignGate.Evaluate(null), Is.EqualTo(CampaignGateOutcome.INDETERMINATE)); Assert.That(CampaignGate.IsEligible(null), Is.False);
        }

        private static SolverResult Forge(SolverClassification classification, int count, SolverMetrics? metrics)
            => (SolverResult)typeof(SolverResult).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0].Invoke(new object?[] { classification, count, Array.Empty<CellCoordinate>(), Array.Empty<RecoveryReduction>(), metrics, null });
        private static SolverMetrics Metrics(int nodes, int guess)
            => (SolverMetrics)typeof(SolverMetrics).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0].Invoke(new object[] { nodes, 0, 0, guess });

        [Test]
        public void CampaignGate_IsFailClosedForEveryClassificationAndMetricCombination()
        {
            foreach (SolverClassification classification in Enum.GetValues(typeof(SolverClassification)))
                foreach (var metrics in new SolverMetrics?[] { null, Metrics(0, 0), Metrics(2, 1) })
                    foreach (int count in new[] { 0, 1, 2 })
                    {
                        var outcome = CampaignGate.Evaluate(Forge(classification, count, metrics));
                        bool eligible = classification == SolverClassification.UNIQUE && count == 1 && metrics != null && metrics.RequiredGuessDepth == 0;
                        Assert.That(outcome == CampaignGateOutcome.ELIGIBLE, Is.EqualTo(eligible), $"{classification} count={count} metrics={(metrics == null ? "none" : metrics.RequiredGuessDepth.ToString())}");
                        if (classification == SolverClassification.UNIQUE && count == 1 && metrics != null && metrics.RequiredGuessDepth > 0) Assert.That(outcome, Is.EqualTo(CampaignGateOutcome.GUESS_DEPTH_REQUIRED));
                        if (classification == SolverClassification.UNSATISFIABLE || classification == SolverClassification.MULTIPLE_OR_MORE) Assert.That(outcome, Is.EqualTo(CampaignGateOutcome.NOT_UNIQUE));
                        if (classification == SolverClassification.INDETERMINATE) Assert.That(outcome, Is.EqualTo(CampaignGateOutcome.INDETERMINATE));
                    }
        }

        // AK-10
        [Test]
        public void RepeatedRuns_AreByteIdenticalForTraceAndMetrics_AlsoAcrossLimitVariants()
        {
            foreach (var def in new[] { SolverV2Fixtures.SingleCell(), SolverV2Fixtures.Snake2x3(), SolverV2Fixtures.DeepDiscard4x4(), SolverV2Fixtures.Guess3() }.Concat(TenByTenCorpus()))
            {
                var first = PuzzleSolver.Solve(def); var baseline = first.DeductionTrace.Select(Fmt).ToArray();
                for (int i = 0; i < 5; i++)
                {
                    var next = PuzzleSolver.Solve(def, i % 2 == 0 ? null : new SolverLimits(maxStates: 5000000, timeBudgetMs: 60000));
                    Assert.That(next.DeductionTrace.Select(Fmt), Is.EqualTo(baseline)); Assert.That(Quad(next.Metrics!), Is.EqualTo(Quad(first.Metrics!)));
                    Assert.That(next.Path.Select(c => (c.X, c.Y)), Is.EqualTo(first.Path.Select(c => (c.X, c.Y))));
                }
            }
        }

        // AK-04/AK-10: the 10x10 forced paths stay purely deductive, fully traced, audited and inside the documented budget.
        [Test]
        public void TenByTen_ForcedPaths_AreDeductive_FullyAudited_AndWithinTheTraceInclusiveBudget()
        {
            var corpus = TenByTenCorpus().ToArray(); var samples = new List<double>(); var sw = new System.Diagnostics.Stopwatch();
            foreach (var def in corpus)
            {
                var result = PuzzleSolver.Solve(def);
                Assert.That(result.Metrics!.SearchNodes, Is.EqualTo(0)); Assert.That(result.Metrics.RequiredGuessDepth, Is.EqualTo(0));
                Assert.That(result.Metrics.DeductionSteps, Is.GreaterThan(0)); Assert.That(result.Metrics.MaxDeductionDepth, Is.GreaterThan(0));
                Assert.That(result.Reductions.Count, Is.EqualTo(result.DeductionTrace.Count));
                TraceAudit.AssertWellFormedAndSound(def, result, "10x10");
            }
            foreach (var def in corpus) for (int i = 0; i < 5; i++) PuzzleSolver.Solve(def);
            for (int i = 0; i < 50; i++) foreach (var def in corpus) { sw.Restart(); var r = PuzzleSolver.Solve(def); sw.Stop(); Assert.That(r.Classification, Is.EqualTo(SolverClassification.UNIQUE)); samples.Add(sw.Elapsed.TotalMilliseconds); }
            samples.Sort(); double p95 = samples[(int)Math.Ceiling(samples.Count * .95) - 1], max = samples.Last();
            TestContext.WriteLine($"WP024_BUDGET solver=solver-v2 corpus=10x10-5-forced-paths-v1 warmup=5 repeats=50 samples={samples.Count} p95_ms={p95:F3} max_ms={max:F3} machine={Environment.MachineName} os={Environment.OSVersion} runtime={Environment.Version} commit={Environment.GetEnvironmentVariable("GITHUB_SHA") ?? "LOCAL_WORKTREE"}");
            Assert.That(p95, Is.LessThan(250)); Assert.That(max, Is.LessThan(2000));
        }

        // AK-03: immutable value types without engine, UI, persistence or asset data.
        [Test]
        public void TraceTypes_AreImmutable_AndReferenceNoEngineUiOrPersistenceTypes()
        {
            var result = PuzzleSolver.Solve(SolverV2Fixtures.TopPair());
            foreach (var step in result.DeductionTrace)
            {
                Assert.That(((ICollection<CandidateValue>)step.RemovedValues).IsReadOnly, Is.True); Assert.That(((ICollection<CandidateValue>)step.RemainingValues).IsReadOnly, Is.True);
                Assert.That(((ICollection<DeductionPremise>)step.Premises).IsReadOnly, Is.True);
            }
            Assert.That(((ICollection<DeductionStep>)result.DeductionTrace).IsReadOnly, Is.True);
            foreach (var type in new[] { typeof(DeductionStep), typeof(DeductionPremise), typeof(SolverMetrics), typeof(SolverResult) })
            {
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    Assert.That(property.SetMethod == null || !property.SetMethod.IsPublic, Is.True, type.Name + "." + property.Name + " must not be settable");
                    var argument = property.PropertyType.IsGenericType ? property.PropertyType.GetGenericArguments()[0] : property.PropertyType;
                    string ns = argument.Namespace ?? "";
                    Assert.That(ns == "System" || ns.StartsWith("System.", StringComparison.Ordinal) || ns.StartsWith("STP.Puzzle.", StringComparison.Ordinal), Is.True, type.Name + "." + property.Name + " exposes " + argument.FullName);
                }
                Assert.That(type.GetConstructors(BindingFlags.Public | BindingFlags.Instance), Is.Empty, type.Name + " is created by the solver only");
            }
            Assert.That(typeof(DeductionStep).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(m => !m.IsSpecialName), Is.Empty);
        }

        // Tracer white-box contract (internal collaborator, inspected by reflection like the stage selector).
        private static object NewTracer(PuzzleDefinition def) => Activator.CreateInstance(TraceAudit.Tracer, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { def }, null)!;
        private static void Record(object tracer, PuzzleDefinition def, DeductionStep s)
        {
            byte removed = 0, remaining = 0; string? axis = null; int line = -1;
            foreach (var v in s.RemovedValues) removed |= (byte)(1 << (int)v);
            foreach (var v in s.RemainingValues) remaining |= (byte)(1 << (int)v);
            foreach (var q in s.Premises.Where(q => q.IsPublicFact && (q.PublicFact == PublicFactKind.ROW_COUNT || q.PublicFact == PublicFactKind.COLUMN_COUNT)))
            { axis = q.PublicFact == PublicFactKind.ROW_COUNT ? "ROW" : "COLUMN"; line = q.LineIndex; }
            TraceAudit.Tracer.GetMethod("Record", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(tracer, new object?[] { s.RuleCode, s.Cell.Y * def.Grid.Width + s.Cell.X, (byte)(removed | remaining), remaining, axis, line });
        }
        private static bool Faulted(object tracer) => (bool)TraceAudit.Tracer.GetProperty("Faulted", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(tracer)!;
        private static int Count(object tracer) => (int)TraceAudit.Tracer.GetProperty("Count", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(tracer)!;

        [Test]
        public void Tracer_ReplaysReductionsToTheSamePremises_IgnoresDuplicatesAndNoOps_AndFlagsUnjustifiedReductions()
        {
            foreach (var def in new[] { SolverV2Fixtures.SingleCell(), SolverV2Fixtures.TopPair(), SolverV2Fixtures.DeepDiscard4x4() })
            {
                var original = PuzzleSolver.Solve(def).DeductionTrace; var tracer = NewTracer(def);
                foreach (var step in original) Record(tracer, def, step);
                foreach (var step in original) Record(tracer, def, step); // identical reductions are deduplicated
                Assert.That(Faulted(tracer), Is.False); Assert.That(Count(tracer), Is.EqualTo(original.Count));
                var rebuilt = (IReadOnlyList<DeductionStep>)TraceAudit.Tracer.GetMethod("Build", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(tracer, null)!;
                Assert.That(rebuilt.Select(Fmt), Is.EqualTo(original.Select(Fmt)));
            }
            var d = SolverV2Fixtures.TopPair(); var noop = NewTracer(d);
            var record = TraceAudit.Tracer.GetMethod("Record", BindingFlags.NonPublic | BindingFlags.Instance)!;
            record.Invoke(noop, new object?[] { "BORDER_PORT_FORBIDDEN", 2, (byte)33, (byte)33, null, -1 });   // nothing removed
            record.Invoke(noop, new object?[] { "BORDER_PORT_FORBIDDEN", 2, (byte)33, (byte)0, null, -1 });    // contradiction: not a deduction
            Assert.That(Count(noop), Is.EqualTo(0)); Assert.That(Faulted(noop), Is.False);

            // A removal that no rule justifies is a fault; so is a step recorded without the entries it needs.
            var bogus = NewTracer(d); record.Invoke(bogus, new object?[] { "NEIGHBOR_PORT_REQUIRED", 0, (byte)127, (byte)126, null, -1 });
            Assert.That(Faulted(bogus), Is.True);
            var unknownRule = NewTracer(d); record.Invoke(unknownRule, new object?[] { "MADE_UP_RULE", 0, (byte)127, (byte)126, null, -1 });
            Assert.That(Faulted(unknownRule), Is.True);
            var original2 = PuzzleSolver.Solve(d).DeductionTrace; var skipped = NewTracer(d);
            foreach (int i in new[] { 0, 1, 6 }) Record(skipped, d, original2[i]);
            Assert.That(Faulted(skipped), Is.True, "step #6 needs #2 and #4");
        }

        [Test]
        public void TraceAudit_RejectsForgedTraces_AndAcceptsRedundantPremisesOnlyWhenIrredundancyIsNotRequired()
        {
            var def = SolverV2Fixtures.TopPair(); var trace = PuzzleSolver.Solve(def).DeductionTrace.ToArray();
            Assert.That(TraceAudit.Audit(def, trace, true), Is.Null);
            DeductionStep[] Replace(int index, DeductionStep step) { var copy = (DeductionStep[])trace.Clone(); copy[index] = step; return copy; }
            var s6 = trace[6]; var facts6 = s6.Premises.Where(q => q.IsPublicFact).ToArray();
            DeductionPremise[] Premises(params int[] entries) => facts6.Concat(entries.Select(TraceAudit.Entry)).ToArray();

            Assert.That(TraceAudit.Audit(def, Replace(6, TraceAudit.With(s6, def, Premises(2))), true), Is.Not.Null, "missing premise #4");
            Assert.That(TraceAudit.Audit(def, Replace(6, TraceAudit.With(s6, def, Premises(4))), true), Is.Not.Null, "missing premise #2");
            Assert.That(TraceAudit.Audit(def, Replace(6, TraceAudit.With(s6, def, Premises())), false), Is.Not.Null, "no premise at all");
            var redundant = TraceAudit.With(s6, def, Premises(0, 2, 4));
            Assert.That(TraceAudit.Audit(def, Replace(6, redundant), true), Does.Contain("redundant"));
            Assert.That(TraceAudit.Audit(def, Replace(6, redundant), false), Is.Null, "a redundant premise is still a valid derivation");
            Assert.That(TraceAudit.Audit(def, Replace(6, TraceAudit.With(s6, def, depth: 3)), true), Does.Contain("depth"), "depth must follow from the premises");
            Assert.That(TraceAudit.Audit(def, Replace(6, TraceAudit.With(s6, def, Premises(2, 7))), true), Is.Not.Null, "forward reference");
            Assert.That(TraceAudit.Audit(def, Replace(6, TraceAudit.With(s6, def, Premises(4, 2))), true), Is.Not.Null, "premises must be in ascending order");
            Assert.That(TraceAudit.Audit(def, Replace(6, TraceAudit.With(s6, def, facts6.Skip(1).Concat(new[] { TraceAudit.Entry(2), TraceAudit.Entry(4) }).ToArray())), true), Is.Not.Null, "public fact missing");
            Assert.That(TraceAudit.Audit(def, Replace(6, TraceAudit.With(s6, def, index: 5)), true), Is.Not.Null, "index must equal position");
            Assert.That(TraceAudit.Audit(def, Replace(6, TraceAudit.With(s6, def, rule: "LOCAL_DEGREE_REQUIRED")), true), Is.Not.Null, "wrong rule code");
            Assert.That(TraceAudit.Audit(def, Replace(6, TraceAudit.With(s6, def, rule: "MADE_UP_RULE")), true), Is.Not.Null, "unknown rule");
            // A public-fact-only step cannot be replaced by an invented conclusion.
            var s4 = trace[4];
            Assert.That(TraceAudit.Audit(def, Replace(4, TraceAudit.Step(4, "LINE_ZERO", def, 0, 1, new[] { CandidateValue.TRACK_NS }, new[] { CandidateValue.EMPTY }, s4.Premises.ToArray(), 1)), true), Is.Not.Null, "NS at (0,1) was already removed by the border rule and row 1 does not make it impossible by itself");
        }

        [Test]
        public void Classify_KeepsTheClassificationTable_AndNeverReportsUniqueWithAFaultedTrace()
        {
            var classify = typeof(PuzzleSolver).GetMethod("Classify", BindingFlags.NonPublic | BindingFlags.Static)!;
            SolverClassification C(bool interrupted, int found, bool faulted) => (SolverClassification)classify.Invoke(null, new object[] { interrupted, found, faulted })!;
            Assert.That(C(false, 0, false), Is.EqualTo(SolverClassification.UNSATISFIABLE));
            Assert.That(C(false, 1, false), Is.EqualTo(SolverClassification.UNIQUE));
            Assert.That(C(false, 2, false), Is.EqualTo(SolverClassification.MULTIPLE_OR_MORE));
            Assert.That(C(false, 1, true), Is.EqualTo(SolverClassification.INDETERMINATE));
            Assert.That(C(false, 2, true), Is.EqualTo(SolverClassification.MULTIPLE_OR_MORE), "a trace fault cannot change a non-unique class");
            foreach (int found in new[] { 0, 1, 2 }) foreach (bool faulted in new[] { false, true }) Assert.That(C(true, found, faulted), Is.EqualTo(SolverClassification.INDETERMINATE));
        }
    }
}
