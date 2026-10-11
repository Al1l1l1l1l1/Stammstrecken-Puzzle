using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Solver.EditMode
{
    // WP-024, QC finding 1 of PR #17 ("published root deductions do not follow completely from their published premises").
    //
    // Contract under test: a published step is sound for EVERY valid puzzle that agrees with it on its declared public facts
    // (GRID_SIZE, ENDPOINT_A, ENDPOINT_B, the row/column count of its line), given its declared earlier entries as premises.
    // A public fact that the derivation reads must therefore be declared, and a declared fact must be one the derivation reads.
    //
    // The oracle here is independent of the tracer: for every step it builds the perturbed valid puzzles that keep the declared
    // facts and change the undeclared ones (grid size, endpoint A, endpoint B, line counts), puts the step's premises into a
    // fresh candidate state and lets the REAL solver stage of the step's rule (not the tracer predicate) decide whether the
    // removal is still derived. Semantic counterexamples are cross-checked with the independent path enumerator.
    public sealed class TracePremiseSufficiencyTests
    {
        private const int FGrid = 1, FA = 2, FB = 4, FLine = 8;

        // A neutral description of one deduction, independent of the grid size it is evaluated in.
        private sealed class Spec
        {
            public string Rule = ""; public int X, Y, Removed; public string? Axis; public int Line = -1;
            public (int X, int Y, int Removed)[] Premises = Array.Empty<(int, int, int)>();
            public int Facts; public string Label = "";
        }

        private static int Mask(IEnumerable<CandidateValue> values) { int m = 0; foreach (var v in values) m |= 1 << (int)v; return m; }

        private static Spec SpecOf(DeductionStep step, IReadOnlyList<DeductionStep> trace, string label)
        {
            var spec = new Spec { Rule = step.RuleCode, X = step.Cell.X, Y = step.Cell.Y, Removed = Mask(step.RemovedValues), Label = label + " step#" + step.Index + " " + step.RuleCode };
            foreach (var q in step.Premises.Where(q => q.IsPublicFact))
            {
                switch (q.PublicFact!.Value)
                {
                    case PublicFactKind.GRID_SIZE: spec.Facts |= FGrid; break;
                    case PublicFactKind.ENDPOINT_A: spec.Facts |= FA; break;
                    case PublicFactKind.ENDPOINT_B: spec.Facts |= FB; break;
                    case PublicFactKind.ROW_COUNT: spec.Facts |= FLine; spec.Axis = "ROW"; spec.Line = q.LineIndex; break;
                    default: spec.Facts |= FLine; spec.Axis = "COLUMN"; spec.Line = q.LineIndex; break;
                }
            }
            spec.Premises = step.Premises.Where(q => !q.IsPublicFact).Select(q => trace[q.TraceIndex]).Select(t => (t.Cell.X, t.Cell.Y, Mask(t.RemovedValues))).ToArray();
            return spec;
        }

        private static int StageOf(string rule)
        {
            switch (rule)
            {
                case "BORDER_PORT_FORBIDDEN": case "ENDPOINT_ENTRY_REQUIRED": return 0;
                case "NEIGHBOR_PORT_REQUIRED": return 1;
                case "LINE_ZERO": case "LINE_TARGET_REACHED": case "LINE_ALL_REMAINING_OCCUPIED": return 2;
                case "LOCAL_DEGREE_REQUIRED": return 3;
                case "LOOP_PREVENTION": return 4;
                default: return 5;
            }
        }

        private static readonly MethodInfo RunStage = typeof(PuzzleSolver).GetMethod("RunStage", BindingFlags.NonPublic | BindingFlags.Static)!;

        // The removal is derived when the real stage of the rule, run on the premise state alone, no longer admits the removed
        // values. A premise state that contradicts the perturbed puzzle (the stage reports a contradiction, for example because
        // another line's count cannot hold the premises) has no solution at all, so every conclusion holds vacuously there.
        private static bool Derives(PuzzleDefinition p, Spec s, out bool vacuous)
        {
            var domains = Enumerable.Repeat((byte)127, p.Grid.CellCount).ToArray();
            foreach (var q in s.Premises) domains[q.Y * p.Grid.Width + q.X] &= (byte)~q.Removed;
            bool consistent = (bool)RunStage.Invoke(null, new object[] { p, domains, new List<RecoveryReduction>(), StageOf(s.Rule) })!;
            vacuous = !consistent;
            return !consistent || (domains[s.Y * p.Grid.Width + s.X] & s.Removed) == 0;
        }

        private static bool RandomCounts(Random rnd, int w, int h, Endpoint a, Endpoint b, out int[] rows, out int[] cols)
        {
            var r = new int[h]; var c = new int[w]; var visited = new bool[w * h]; int budget = 4000, end = b.Cell.Y * w + b.Cell.X;
            bool Walk(int cell)
            {
                if (--budget < 0) return false;
                visited[cell] = true; r[cell / w]++; c[cell % w]++;
                if (cell == end) return true;
                var order = new[] { 0, 1, 2, 3 };
                for (int i = 3; i > 0; i--) { int j = rnd.Next(i + 1); var t = order[i]; order[i] = order[j]; order[j] = t; }
                foreach (int d in order)
                {
                    int x = cell % w + (d == 1 ? 1 : d == 3 ? -1 : 0), y = cell / w + (d == 0 ? -1 : d == 2 ? 1 : 0);
                    if (x < 0 || y < 0 || x >= w || y >= h || visited[y * w + x]) continue;
                    if (Walk(y * w + x)) return true;
                }
                visited[cell] = false; r[cell / w]--; c[cell % w]--; return false;
            }
            bool found = Walk(a.Cell.Y * w + a.Cell.X);
            rows = r; cols = c; return found;
        }

        // All valid puzzles of the neighbourhood (grids up to <grow> larger or smaller down to the cells the step names, every
        // endpoint position, path-derived counts) that agree with <basis> on the facts in <agree> and are free elsewhere.
        private static IEnumerable<PuzzleDefinition> Agreeing(PuzzleDefinition basis, Spec s, int agree, int seed, int maxCombinations = int.MaxValue, int grow = 1, int samples = 2)
        {
            int needW = s.Premises.Select(q => q.X).Append(s.X).Max() + 1, needH = s.Premises.Select(q => q.Y).Append(s.Y).Max() + 1;
            bool fixedGrid = (agree & FGrid) != 0; var rnd = new Random(seed);
            int w0 = fixedGrid ? basis.Grid.Width : needW, w1 = fixedGrid ? basis.Grid.Width : Math.Max(needW, basis.Grid.Width) + grow;
            int h0 = fixedGrid ? basis.Grid.Height : needH, h1 = fixedGrid ? basis.Grid.Height : Math.Max(needH, basis.Grid.Height) + grow;
            int declaredLine = -1;
            if ((agree & FLine) != 0) declaredLine = s.Axis == "ROW" ? basis.RowCounts[s.Line] : basis.ColumnCounts[s.Line];
            // Combination = (grid size, endpoint A, endpoint B). A stride over the ordered combinations bounds the work per step
            // deterministically; the offset depends on the seed so that different steps look at different combinations.
            long total = 0;
            for (int w = w0; w <= w1; w++) for (int h = h0; h <= h1; h++)
            {
                int e = SmallPathOracle.Edges(w, h).Count; total += ((agree & FA) != 0 ? 1L : e) * ((agree & FB) != 0 ? 1L : e);
            }
            long stride = Math.Max(1, (total + maxCombinations - 1) / maxCombinations), position = seed % stride;
            for (int w = w0; w <= w1; w++)
                for (int h = h0; h <= h1; h++)
                {
                    var grid = GridSize.Create(w, h).Value; if (grid == null) continue;
                    var edges = SmallPathOracle.Edges(w, h);
                    var aList = (agree & FA) != 0 ? new[] { new SmallPathOracle.Edge((int)basis.A.Side, basis.A.Index) } : edges.ToArray();
                    var bList = (agree & FB) != 0 ? new[] { new SmallPathOracle.Edge((int)basis.B.Side, basis.B.Index) } : edges.ToArray();
                    foreach (var ea in aList) foreach (var eb in bList)
                    {
                        if (position++ % stride != 0) continue;
                        if ((agree & FLine) != 0 && s.Line >= (s.Axis == "ROW" ? h : w)) continue;
                        if (ea.Side == eb.Side && ea.Index == eb.Index) continue;
                        var a = Endpoint.Create(grid, (Direction)ea.Side, ea.Index).Value; var b = Endpoint.Create(grid, (Direction)eb.Side, eb.Index).Value;
                        if (a == null || b == null) continue;
                        for (int k = 0; k < samples; k++)
                        {
                            int[] rows = Array.Empty<int>(), cols = Array.Empty<int>(); bool ok = false;
                            for (int attempt = 0; attempt < 40 && !ok; attempt++)
                            {
                                if (!RandomCounts(rnd, w, h, a, b, out rows, out cols)) break;
                                ok = declaredLine < 0 || (s.Axis == "ROW" ? rows[s.Line] : cols[s.Line]) == declaredLine;
                            }
                            if (!ok) continue;
                            var def = PuzzleDefinition.Create(grid, a, b, rows, cols, "train-track-v1").Value;
                            if (def != null) yield return def;
                        }
                    }
                }
        }

        private static IEnumerable<(string Name, PuzzleDefinition Def)> Corpus()
        {
            yield return ("SingleCell", SolverV2Fixtures.SingleCell()); yield return ("TopPair", SolverV2Fixtures.TopPair());
            yield return ("Snake2x3", SolverV2Fixtures.Snake2x3()); yield return ("DeepDiscard4x4", SolverV2Fixtures.DeepDiscard4x4());
            yield return ("Guess2", SolverV2Fixtures.Guess2()); yield return ("Guess3", SolverV2Fixtures.Guess3());
            int n = 0;
            foreach (var (w, h) in new[] { (2, 2), (2, 3), (3, 2), (3, 3) })
                foreach (var a in SmallPathOracle.Edges(w, h)) foreach (var b in SmallPathOracle.Edges(w, h))
                {
                    if (a.Side == b.Side && a.Index == b.Index) continue;
                    foreach (var kv in SmallPathOracle.Counts(w, h, a, b).Where(k => k.Value == 1).OrderBy(k => k.Key, StringComparer.Ordinal))
                    {
                        // 3x3 is sampled (every 27th puzzle) to keep the test fast; the 2x2/2x3/3x2 puzzles are complete.
                        if (w == 3 && h == 3 && n++ % 27 != 0) continue;
                        var parts = kv.Key.Split(';');
                        yield return ($"{w}x{h}-A{a.Side}/{a.Index}-B{b.Side}/{b.Index}-{kv.Key}", TraceAudit.Define(w, h, a, b, parts[0].Split(',').Select(int.Parse).ToArray(), parts[1].Split(',').Select(int.Parse).ToArray()));
                    }
                }
        }

        private static string Facts(int facts) => string.Join("+", new[] { (FGrid, "grid"), (FA, "A"), (FB, "B"), (FLine, "line") }.Where(f => (facts & f.Item1) != 0).Select(f => f.Item2));

        [Test]
        public void EveryPublishedStep_IsDerivedByTheRealStage_ForEveryValidPuzzleAgreeingOnItsDeclaredFacts()
        {
            int puzzles = 0, steps = 0, perturbed = 0, vacuousCount = 0; var perRule = new SortedDictionary<string, (int Steps, int Perturbations)>(StringComparer.Ordinal);
            foreach (var (name, def) in Corpus())
            {
                var result = PuzzleSolver.Solve(def); if (result.Classification != SolverClassification.UNIQUE) continue;
                puzzles++;
                foreach (var step in result.DeductionTrace)
                {
                    var spec = SpecOf(step, result.DeductionTrace, name); steps++;
                    Assert.That(Derives(def, spec, out bool ownVacuous) && !ownVacuous, Is.True, spec.Label + ": the published step is not derived (non-vacuously) in its own puzzle");
                    int count = 0;
                    foreach (var other in Agreeing(def, spec, spec.Facts, 24024 + steps, maxCombinations: 12))
                    {
                        count++;
                        bool derived = Derives(other, spec, out bool vacuous); if (vacuous) vacuousCount++;
                        if (!derived)
                            Assert.Fail($"{spec.Label} declares [{Facts(spec.Facts)}] but is not derived for {other.Grid.Width}x{other.Grid.Height} A={other.A.Side}/{other.A.Index} B={other.B.Side}/{other.B.Index} rows({string.Join(",", other.RowCounts)}) cols({string.Join(",", other.ColumnCounts)}) in {name}");
                    }
                    perturbed += count; perRule.TryGetValue(step.RuleCode, out var cur); perRule[step.RuleCode] = (cur.Steps + 1, cur.Perturbations + count);
                }
            }
            TestContext.WriteLine($"WP024_PREMISE_SUFFICIENCY puzzles={puzzles} steps={steps} perturbedPuzzles={perturbed} vacuous={vacuousCount} " + string.Join(" ", perRule.Select(kv => $"{kv.Key}={kv.Value.Steps}/{kv.Value.Perturbations}")));
            Assert.That(puzzles, Is.GreaterThan(100));
            foreach (var rule in new[] { "BORDER_PORT_FORBIDDEN", "ENDPOINT_ENTRY_REQUIRED", "NEIGHBOR_PORT_REQUIRED", "LINE_ZERO", "LINE_ALL_REMAINING_OCCUPIED", "LINE_TARGET_REACHED" })
            {
                Assert.That(perRule.ContainsKey(rule), Is.True, rule + " must occur in the corpus");
                Assert.That(perRule[rule].Perturbations, Is.GreaterThan(perRule[rule].Steps), rule + ": the oracle must actually perturb");
            }
        }

        [Test]
        public void DeclaredEndpointLineAndSizeFacts_AreNecessary_EachOneIsRefutedByAValidPuzzleWhenDropped()
        {
            // Negative control of the oracle: with one declared fact released, a valid puzzle must exist in which the step is
            // not derived. This is the defect of the first delivery (border steps declared the grid size only) turned into a test.
            var stats = new SortedDictionary<string, int[]>(StringComparer.Ordinal); int index = 0;
            foreach (var (name, def) in Corpus())
            {
                if (index++ % 5 != 0) continue;
                var result = PuzzleSolver.Solve(def); if (result.Classification != SolverClassification.UNIQUE) continue;
                foreach (var step in result.DeductionTrace)
                {
                    var spec = SpecOf(step, result.DeductionTrace, name);
                    foreach (int bit in new[] { FGrid, FA, FB, FLine })
                    {
                        if ((spec.Facts & bit) == 0) continue;
                        bool refuted = false;
                        foreach (var other in Agreeing(def, spec, spec.Facts & ~bit, 77 + step.Index, maxCombinations: 150))
                            if (!Derives(other, spec, out _)) { refuted = true; break; }
                        string key = step.RuleCode + " without " + Facts(bit);
                        if (!stats.TryGetValue(key, out var counts)) stats[key] = counts = new int[2];
                        counts[0]++; if (refuted) counts[1]++;
                    }
                }
            }
            foreach (var kv in stats) TestContext.WriteLine($"WP024_PREMISE_NECESSITY {kv.Key}: refuted {kv.Value[1]} of {kv.Value[0]}");
            foreach (var key in new[]
            {
                "BORDER_PORT_FORBIDDEN without A", "BORDER_PORT_FORBIDDEN without B", "ENDPOINT_ENTRY_REQUIRED without A", "ENDPOINT_ENTRY_REQUIRED without B",
                "LINE_ZERO without line", "LINE_TARGET_REACHED without line", "LINE_ALL_REMAINING_OCCUPIED without line", "LINE_ALL_REMAINING_OCCUPIED without grid"
            })
            {
                Assert.That(stats.ContainsKey(key), Is.True, key + " must occur in the corpus");
                Assert.That(stats[key][1], Is.EqualTo(stats[key][0]), key + ": every step needs this fact, so releasing it must be refuted by a valid puzzle");
            }
            // The grid size is declared wherever a rule reads the grid geometry. It is necessary where the border side is the S or E side or the
            // line length counts; for N/W sides and for neighbors named by a premise it is a conservative, still sound declaration.
            Assert.That(stats["BORDER_PORT_FORBIDDEN without grid"][1], Is.GreaterThan(0));
        }

        [Test]
        public void ReportedBorderCounterexample_IsRefutedByTheIndependentEnumeratorWithoutEndpointFacts_AndHoldsWithThem()
        {
            // 3x3, A=W/0, B=E/2, rows (3,1,1), cols (1,1,3). The first delivery published BORDER_PORT_FORBIDDEN at (1,0) with the grid size
            // as its only public fact. Moving A onto the N port of (1,0) keeps the grid size and every declared premise, but NS at (1,0)
            // is then a solution. SmallPathOracle never calls the solver, the tracer or any propagation stage.
            var def = TraceAudit.Define(3, 3, new SmallPathOracle.Edge(3, 0), new SmallPathOracle.Edge(1, 2), new[] { 3, 1, 1 }, new[] { 1, 1, 3 });
            var result = PuzzleSolver.Solve(def); Assert.That(result.Classification, Is.EqualTo(SolverClassification.UNIQUE));
            var step = result.DeductionTrace.First(t => t.RuleCode == "BORDER_PORT_FORBIDDEN" && t.Cell.X == 1 && t.Cell.Y == 0);
            Assert.That(step.Premises.Where(q => !q.IsPublicFact), Is.Empty, "a border step has no earlier entry");
            Assert.That(step.Premises.Select(q => q.PublicFact), Is.EqualTo(new PublicFactKind?[] { PublicFactKind.GRID_SIZE, PublicFactKind.ENDPOINT_A, PublicFactKind.ENDPOINT_B }));
            int removed = Mask(step.RemovedValues); Assert.That(removed, Is.Not.EqualTo(0));
            var edges = SmallPathOracle.Edges(3, 3);
            string? Counterexample(bool keepA, bool keepB)
            {
                foreach (var a in edges) foreach (var b in edges)
                {
                    if (a.Side == b.Side && a.Index == b.Index) continue;
                    if (keepA && (a.Side != (int)def.A.Side || a.Index != def.A.Index)) continue;
                    if (keepB && (b.Side != (int)def.B.Side || b.Index != def.B.Index)) continue;
                    foreach (var board in SmallPathOracle.Boards(3, 3, a, b))
                        if ((removed & (1 << (board[1] - '0'))) != 0) return $"A={a.Side}/{a.Index} B={b.Side}/{b.Index} board={board}";
                }
                return null;
            }
            string? withoutBoth = Counterexample(false, false), withoutA = Counterexample(false, true), withoutB = Counterexample(true, false);
            TestContext.WriteLine($"WP024_QC_COUNTEREXAMPLE grid-size-only: {withoutBoth}; A free: {withoutA}; B free: {withoutB}");
            Assert.That(withoutBoth, Is.Not.Null, "the grid size alone does not carry the conclusion");
            Assert.That(withoutA, Is.Not.Null, "ENDPOINT_A is needed");
            Assert.That(withoutB, Is.Not.Null, "ENDPOINT_B is needed");
            Assert.That(Counterexample(true, true), Is.Null, "with grid size, A and B declared, no valid 3x3 puzzle has a solution that violates the conclusion");
        }

        [Test]
        public void TrackedEndpointPort_AgreesWithThePuzzleDefinition_AndNamesTheEndpointThatSettlesIt()
        {
            var method = TraceAudit.Tracer.GetMethod("EndpointPort", BindingFlags.NonPublic | BindingFlags.Instance)!; int checkedPorts = 0;
            foreach (var (w, h) in new[] { (2, 2), (2, 3), (3, 3) })
            {
                var edges = SmallPathOracle.Edges(w, h);
                foreach (var a in edges) foreach (var b in edges)
                {
                    if (a.Side == b.Side && a.Index == b.Index) continue;
                    var counts = SmallPathOracle.Counts(w, h, a, b); if (counts.Count == 0) continue;
                    var parts = counts.Keys.OrderBy(k => k, StringComparer.Ordinal).First().Split(';');
                    var def = TraceAudit.Define(w, h, a, b, parts[0].Split(',').Select(int.Parse).ToArray(), parts[1].Split(',').Select(int.Parse).ToArray());
                    var tracer = Activator.CreateInstance(TraceAudit.Tracer, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { def }, null)!;
                    for (int cell = 0; cell < w * h; cell++)
                        for (int d = 0; d < 4; d++)
                        {
                            var args = new object[] { cell, d, 0 }; bool port = (bool)method.Invoke(tracer, args)!; int facts = (int)args[2];
                            Assert.That(port, Is.EqualTo(def.IsEndpointPort(cell % w, cell / w, (Direction)d)), $"{w}x{h} cell {cell} side {d}");
                            bool aMatches = def.A.Side == (Direction)d && def.A.Cell.X == cell % w && def.A.Cell.Y == cell / w;
                            Assert.That(facts, Is.EqualTo(port ? (aMatches ? FA : FB) : FA | FB), "a matching endpoint settles the port alone, a miss needs both endpoints");
                            checkedPorts++;
                        }
                }
            }
            Assert.That(checkedPorts, Is.GreaterThan(1000));
        }

        // The rules that no root propagation of the corpus reaches are checked on the crafted constraint states of the stage contracts.
        private static byte[] Open(int cells) => Enumerable.Repeat((byte)127, cells).ToArray();
        private static PuzzleDefinition Single(int size)
        {
            var grid = GridSize.Create(size, size).Value!; var counts = new int[size]; counts[0] = 1;
            return PuzzleDefinition.Create(grid, Endpoint.Create(grid, Direction.N, 0).Value!, Endpoint.Create(grid, Direction.W, 0).Value!, counts, counts, "train-track-v1").Value!;
        }

        private static Spec Crafted(PuzzleDefinition def, string rule, int x, int y, int removed, byte[] view, out int facts)
        {
            var entryType = TraceAudit.Tracer.GetNestedType("Entry", BindingFlags.NonPublic)!; var entry = Activator.CreateInstance(entryType, true)!;
            void Set(string name, object? value) => entryType.GetField(name)!.SetValue(entry, value);
            Set("Cell", y * def.Grid.Width + x); Set("Rule", rule); Set("Removed", (byte)removed); Set("After", (byte)(127 & ~removed)); Set("Axis", null); Set("Line", -1);
            var tracer = Activator.CreateInstance(TraceAudit.Tracer, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { def }, null)!;
            facts = (int)TraceAudit.Tracer.GetMethod("Derive", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(tracer, new[] { entry, view })!;
            var premises = new List<(int, int, int)>();
            for (int c = 0; c < view.Length; c++) if (view[c] != 127) premises.Add((c % def.Grid.Width, c / def.Grid.Width, 127 & ~view[c]));
            return new Spec { Rule = rule, X = x, Y = y, Removed = removed, Premises = premises.ToArray(), Facts = facts, Label = "crafted " + rule };
        }

        private static (int X, int Y, int Removed) RealReduction(PuzzleDefinition def, byte[] view, int stage)
        {
            var sink = new List<RecoveryReduction>(); var copy = (byte[])view.Clone();
            Assert.That((bool)RunStage.Invoke(null, new object[] { def, copy, sink, stage })!, Is.True);
            Assert.That(sink.Count, Is.EqualTo(1));
            return (sink[0].Cell.X, sink[0].Cell.Y, sink[0].Before & ~sink[0].After);
        }

        [Test]
        public void CraftedStates_DegreeLoopAndConnectivity_DeclareExactlyTheFactsTheyReadAndAreDerivedForAllPuzzlesAgreeingOnThem()
        {
            var open3 = Open(9); var interior = Open(9); interior[1] = 1;
            var loop = new byte[] { 64, 1, 1, 1, 17, 32, 1, 8, 64 }; var loopReduction = RealReduction(Single(3), loop, 4);
            var conn = new byte[] { 64, 1, 1, 127 }; var connReduction = RealReduction(Single(2), conn, 5);
            // The endpoint facts are shown to be necessary (a valid puzzle refutes the step without them) for the degree rule; the loop and
            // connectivity states contradict every perturbed endpoint position (no solution at all), so there they are declared because the
            // rule reads them, and the oracle can only confirm sufficiency.
            var cases = new[]
            {
                // NS at the border cell (1,0): its N port is no endpoint port of this puzzle, which depends on A and B (QC finding 1, rule LOCAL_DEGREE_REQUIRED).
                (Single(3), Crafted(Single(3), "LOCAL_DEGREE_REQUIRED", 1, 0, 1 << 1, open3, out int degreeBorder), degreeBorder, FGrid | FA | FB, true),
                // NS at the interior cell (1,1) whose N neighbor is empty: both ports are interior, no endpoint is read.
                (Single(3), Crafted(Single(3), "LOCAL_DEGREE_REQUIRED", 1, 1, 1 << 1, interior, out int degreeInterior), degreeInterior, FGrid, false),
                // EMPTY alone removed at the cell of endpoint A: needs the grid (cell position) and A, not B (TopPair: A=N/0 at (0,0), B=N/1).
                (SolverV2Fixtures.TopPair(), Crafted(SolverV2Fixtures.TopPair(), "ENDPOINT_ENTRY_REQUIRED", 0, 0, 1 << 0, Open(4), out int emptyAtA), emptyAtA, FGrid | FA, true),
                (SolverV2Fixtures.TopPair(), Crafted(SolverV2Fixtures.TopPair(), "ENDPOINT_ENTRY_REQUIRED", 1, 0, 1 << 0, Open(4), out int emptyAtB), emptyAtB, FGrid | FB, true),
                (Single(3), Crafted(Single(3), "LOOP_PREVENTION", loopReduction.X, loopReduction.Y, loopReduction.Removed, loop, out int loopFacts), loopFacts, FGrid, false),
                (Single(2), Crafted(Single(2), "CONNECTIVITY_PRESERVATION", connReduction.X, connReduction.Y, connReduction.Removed, conn, out int connFacts), connFacts, FGrid | FA | FB, false)
            };
            foreach (var (def, spec, facts, expected, endpointsNecessary) in cases)
            {
                Assert.That(facts, Is.EqualTo(expected), $"{spec.Label} declares [{Facts(facts)}], expected [{Facts(expected)}]");
                Assert.That(Derives(def, spec, out bool vacuous) && !vacuous, Is.True, spec.Label + " in its own puzzle");
                int perturbed = 0;
                foreach (var other in Agreeing(def, spec, spec.Facts, 5150, maxCombinations: 400))
                {
                    perturbed++;
                    if (!Derives(other, spec, out _)) Assert.Fail($"{spec.Label} declares [{Facts(facts)}] but is not derived for {other.Grid.Width}x{other.Grid.Height} A={other.A.Side}/{other.A.Index} B={other.B.Side}/{other.B.Index}");
                }
                Assert.That(perturbed, Is.GreaterThan(0), spec.Label + ": the oracle must perturb");
                foreach (int bit in new[] { FA, FB })
                {
                    if ((facts & bit) == 0 || !endpointsNecessary) continue;
                    bool refuted = Agreeing(def, spec, facts & ~bit, 5151, maxCombinations: 400).Any(other => !Derives(other, spec, out _));
                    Assert.That(refuted, Is.True, $"{spec.Label}: releasing {Facts(bit)} must be refuted by a valid puzzle");
                }
            }
        }

        [Test]
        public void Audit_RejectsAnyPublicFactSetThatIsNotExactlyTheFactsTheDerivationReads()
        {
            int mutated = 0;
            foreach (var def in new[] { SolverV2Fixtures.SingleCell(), SolverV2Fixtures.TopPair(), SolverV2Fixtures.Snake2x3(), SolverV2Fixtures.DeepDiscard4x4() })
            {
                var trace = PuzzleSolver.Solve(def).DeductionTrace.ToArray(); Assert.That(TraceAudit.Audit(def, trace, true), Is.Null);
                for (int i = 0; i < trace.Length; i++)
                {
                    var facts = trace[i].Premises.Where(q => q.IsPublicFact).ToArray(); var entries = trace[i].Premises.Where(q => !q.IsPublicFact).ToArray();
                    DeductionStep[] With(DeductionPremise[] premises) { var copy = (DeductionStep[])trace.Clone(); copy[i] = TraceAudit.With(trace[i], def, premises); return copy; }
                    for (int drop = 0; drop < facts.Length; drop++)
                    {
                        var kept = facts.Where((_, k) => k != drop).Concat(entries).ToArray();
                        // A line rule without its row/column fact has no line at all; every other missing fact is a missing premise of the derivation.
                        bool lineFact = facts[drop].PublicFact == PublicFactKind.ROW_COUNT || facts[drop].PublicFact == PublicFactKind.COLUMN_COUNT;
                        Assert.That(TraceAudit.Audit(def, With(kept), false), Does.Contain(lineFact && trace[i].RuleCode.StartsWith("LINE_", StringComparison.Ordinal) ? "line fact" : "public facts"), $"step {i} {trace[i].RuleCode} without {facts[drop].PublicFact}"); mutated++;
                    }
                    bool isLine = trace[i].RuleCode.StartsWith("LINE_", StringComparison.Ordinal);
                    foreach (var extra in new[] { TraceAudit.Fact(PublicFactKind.GRID_SIZE), TraceAudit.Fact(PublicFactKind.ENDPOINT_A), TraceAudit.Fact(PublicFactKind.ENDPOINT_B) })
                    {
                        if (facts.Any(q => q.PublicFact == extra.PublicFact)) continue;
                        // Facts stay in their canonical order: grid, A, B, then the line count.
                        var all = facts.Concat(new[] { extra }).OrderBy(q => (int)q.PublicFact!.Value).ThenBy(q => q.LineIndex).Concat(entries).ToArray();
                        Assert.That(TraceAudit.Audit(def, With(all), false), Does.Contain("public facts"), $"step {i} {trace[i].RuleCode} plus unread {extra.PublicFact}"); mutated++;
                    }
                    if (!isLine) { Assert.That(TraceAudit.Audit(def, With(facts.Concat(new[] { TraceAudit.Fact(PublicFactKind.ROW_COUNT, 0) }).Concat(entries).ToArray()), false), Is.Not.Null); mutated++; }
                }
            }
            // The first delivery's declarations: grid size only on a border step, one endpoint on an endpoint-entry step.
            var top = SolverV2Fixtures.TopPair(); var topTrace = PuzzleSolver.Solve(top).DeductionTrace.ToArray();
            DeductionStep[] Old(int index, params DeductionPremise[] facts) { var copy = (DeductionStep[])topTrace.Clone(); copy[index] = TraceAudit.With(topTrace[index], top, facts); return copy; }
            Assert.That(TraceAudit.Audit(top, Old(2, TraceAudit.Fact(PublicFactKind.GRID_SIZE)), true), Does.Contain("public facts"), "border step with the grid size only");
            Assert.That(TraceAudit.Audit(top, Old(0, TraceAudit.Fact(PublicFactKind.GRID_SIZE), TraceAudit.Fact(PublicFactKind.ENDPOINT_A)), true), Does.Contain("public facts"), "endpoint step with its own endpoint only");
            Assert.That(mutated, Is.GreaterThan(50));
        }
    }
}
