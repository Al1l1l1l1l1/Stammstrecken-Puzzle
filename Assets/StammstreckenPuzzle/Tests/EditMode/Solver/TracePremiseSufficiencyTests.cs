using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Solver.EditMode
{
    // WP-024, premise contract of the root trace (ADR-031 section 3: concrete conclusion and MINIMAL premises).
    //
    // QC finding 1 of PR #17 (first QC): published steps did not follow from their published premises (facts were missing).
    // Second QC of PR #17, finding 2: published steps carried premises they do not need (GRID_SIZE on neighbor steps).
    //
    // Contract under test, both directions:
    // - sufficient: a published step is sound for EVERY valid puzzle that agrees with it on its declared public facts
    //   (GRID_SIZE, ENDPOINT_A, ENDPOINT_B, the row/column count of its line) and contains the cells it names, given its
    //   declared earlier entries as premises;
    // - minimal (irredundant, not globally shortest): leaving out any single declared public fact or any single referenced
    //   earlier entry is refuted by a valid puzzle that agrees on the rest.
    //
    // The oracle here is independent of the tracer: for every step it builds the valid puzzles that keep the declared facts
    // and change the undeclared ones (grid size, endpoint A, endpoint B, line counts), puts the step's premises into a fresh
    // candidate state and lets the REAL solver stage of the step's rule (not the tracer predicate) decide whether the
    // removal is still derived. Semantic counterexamples are cross-checked with the independent path enumerator.
    public sealed class TracePremiseSufficiencyTests
    {
        private const int FGrid = 1, FA = 2, FB = 4, FLine = 8;

        // A neutral description of one deduction, independent of the grid size it is evaluated in.
        private sealed class Spec
        {
            public string Rule = ""; public int X, Y, Removed; public string? Axis; public int Line = -1;
            public (int X, int Y, int Removed)[] Premises = Array.Empty<(int, int, int)>();
            public int[] Entries = Array.Empty<int>();
            public int Facts; public string Label = "";
            public Spec Without(int entry) => new Spec { Rule = Rule, X = X, Y = Y, Removed = Removed, Axis = Axis, Line = Line, Facts = Facts, Label = Label, Entries = Entries.Where(e => e != entry).ToArray(), Premises = Premises.Where((_, k) => Entries[k] != entry).ToArray() };
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
            spec.Entries = step.Premises.Where(q => !q.IsPublicFact).Select(q => q.TraceIndex).ToArray();
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

        // A valid puzzle of the neighbourhood that agrees on <facts>, contains the named cells and in which the real stage does
        // not derive the step from <spec>'s premises (and does not contradict). Exhaustive over grid sizes up to +1 and every
        // endpoint pair; line counts are sampled from random paths (several per size and endpoint pair).
        private static PuzzleDefinition? Refutation(PuzzleDefinition def, Spec spec, int facts, int seed)
        {
            bool line = spec.Rule.StartsWith("LINE_", StringComparison.Ordinal);
            foreach (var other in Agreeing(def, spec, facts, seed, samples: line ? 4 : 1))
                if (!Derives(other, spec, out _)) return other;
            return null;
        }

        [Test]
        public void EveryDeclaredPremise_IsNecessary_EachFactAndEachEntryIsRefutedByAValidPuzzleWhenLeftOut()
        {
            // Second QC of PR #17, finding 2 (and its negative control, QC finding 1): with one declared premise released, a
            // valid puzzle must exist in which the step is no longer derived. Facts and entries alike, every step of the sample.
            var stats = new SortedDictionary<string, int[]>(StringComparer.Ordinal); var unrefuted = new List<string>(); int index = 0;
            foreach (var (name, def) in Corpus())
            {
                if (index++ % 3 != 0) continue;
                var result = PuzzleSolver.Solve(def); if (result.Classification != SolverClassification.UNIQUE) continue;
                foreach (var step in result.DeductionTrace)
                {
                    var spec = SpecOf(step, result.DeductionTrace, name);
                    void Count(string what, PuzzleDefinition? refutation)
                    {
                        string key = step.RuleCode + " without " + what;
                        if (!stats.TryGetValue(key, out var counts)) stats[key] = counts = new int[2];
                        counts[0]++; if (refutation != null) counts[1]++; else unrefuted.Add(spec.Label + " without " + what);
                    }
                    foreach (int bit in new[] { FGrid, FA, FB, FLine })
                        if ((spec.Facts & bit) != 0) Count(Facts(bit), Refutation(def, spec, spec.Facts & ~bit, 77 + step.Index));
                    foreach (int entry in spec.Entries) Count("an entry", Refutation(def, spec.Without(entry), spec.Facts, 78 + step.Index));
                }
            }
            foreach (var kv in stats) TestContext.WriteLine($"WP024_PREMISE_NECESSITY {kv.Key}: refuted {kv.Value[1]} of {kv.Value[0]}");
            foreach (var key in new[]
            {
                "BORDER_PORT_FORBIDDEN without A", "BORDER_PORT_FORBIDDEN without B", "BORDER_PORT_FORBIDDEN without grid",
                "ENDPOINT_ENTRY_REQUIRED without A", "ENDPOINT_ENTRY_REQUIRED without B", "ENDPOINT_ENTRY_REQUIRED without grid",
                "LINE_ZERO without line", "LINE_TARGET_REACHED without line", "LINE_TARGET_REACHED without an entry",
                "LINE_ALL_REMAINING_OCCUPIED without line", "LINE_ALL_REMAINING_OCCUPIED without grid", "NEIGHBOR_PORT_REQUIRED without an entry"
            })
                Assert.That(stats.ContainsKey(key), Is.True, key + " must occur in the sample");
            Assert.That(stats.Keys.Where(k => k.StartsWith("NEIGHBOR_PORT_REQUIRED without ", StringComparison.Ordinal)), Is.EqualTo(new[] { "NEIGHBOR_PORT_REQUIRED without an entry" }), "a neighbor step declares no public fact");
            Assert.That(unrefuted, Is.Empty, "every declared premise must be necessary");
        }

        [Test]
        public void ReportedBorderCounterexample_IsRefutedByTheIndependentEnumeratorWithoutEndpointFacts_AndHoldsWithThem()
        {
            // 3x3, A=W/0, B=E/2, rows (3,1,1), cols (1,1,3). The first delivery published BORDER_PORT_FORBIDDEN at (1,0) with the grid size
            // as its only public fact. Moving A onto the N port of (1,0) keeps the grid size and every declared premise, but NS at (1,0)
            // is then a solution. SmallPathOracle never calls the solver, the tracer or any propagation stage.
            // The removal rests on the N port of (1,0), which is exterior in every grid: A and B are its premises, the grid size is not
            // (second QC of PR #17, finding 2, applied to the border rule).
            var def = TraceAudit.Define(3, 3, new SmallPathOracle.Edge(3, 0), new SmallPathOracle.Edge(1, 2), new[] { 3, 1, 1 }, new[] { 1, 1, 3 });
            var result = PuzzleSolver.Solve(def); Assert.That(result.Classification, Is.EqualTo(SolverClassification.UNIQUE));
            var step = result.DeductionTrace.First(t => t.RuleCode == "BORDER_PORT_FORBIDDEN" && t.Cell.X == 1 && t.Cell.Y == 0);
            Assert.That(step.Premises.Where(q => !q.IsPublicFact), Is.Empty, "a border step has no earlier entry");
            Assert.That(step.Premises.Select(q => q.PublicFact), Is.EqualTo(new PublicFactKind?[] { PublicFactKind.ENDPOINT_A, PublicFactKind.ENDPOINT_B }));
            int removed = Mask(step.RemovedValues); Assert.That(removed, Is.Not.EqualTo(0));
            string? Counterexample(int w, int h, bool keepA, bool keepB)
            {
                var edges = SmallPathOracle.Edges(w, h);
                foreach (var a in edges) foreach (var b in edges)
                {
                    if (a.Side == b.Side && a.Index == b.Index) continue;
                    if (keepA && (a.Side != (int)def.A.Side || a.Index != def.A.Index)) continue;
                    if (keepB && (b.Side != (int)def.B.Side || b.Index != def.B.Index)) continue;
                    foreach (var board in SmallPathOracle.Boards(w, h, a, b))
                        if ((removed & (1 << (board[1] - '0'))) != 0) return $"{w}x{h} A={a.Side}/{a.Index} B={b.Side}/{b.Index} board={board}";
                }
                return null;
            }
            string? withoutBoth = Counterexample(3, 3, false, false), withoutA = Counterexample(3, 3, false, true), withoutB = Counterexample(3, 3, true, false);
            TestContext.WriteLine($"WP024_QC_COUNTEREXAMPLE grid-size-only: {withoutBoth}; A free: {withoutA}; B free: {withoutB}");
            Assert.That(withoutBoth, Is.Not.Null, "the grid size alone does not carry the conclusion");
            Assert.That(withoutA, Is.Not.Null, "ENDPOINT_A is needed");
            Assert.That(withoutB, Is.Not.Null, "ENDPOINT_B is needed");
            // With A and B declared, no valid puzzle of any grid size 2..4 x 3..4 (B=E/2 needs three rows) has a solution that uses
            // a removed shape at (1,0): the grid size is not a premise.
            foreach (var (w, h) in new[] { (2, 3), (3, 3), (4, 3), (2, 4), (3, 4), (4, 4) })
                Assert.That(Counterexample(w, h, true, true), Is.Null, $"{w}x{h} with A and B declared");
        }

        [Test]
        public void SecondQcFinding2_NeighborStepRestsOnItsEntryAlone_InEveryGridSize()
        {
            // Second QC of PR #17, finding 2: 2x2, A=N/0, B=W/0, rows and columns (1,0). Step #0 leaves only WN at (0,0); step #4
            // (NEIGHBOR_PORT_REQUIRED) removes SW at (1,0). The earlier delivery declared GRID_SIZE next to #0. WN has no E port while
            // SW at the cell to its right needs exactly that port; the grid size does not matter.
            var def = SolverV2Fixtures.SingleCell(); var result = PuzzleSolver.Solve(def);
            var step = result.DeductionTrace[4];
            Assert.That((step.RuleCode, step.Cell.X, step.Cell.Y, Mask(step.RemovedValues)), Is.EqualTo(("NEIGHBOR_PORT_REQUIRED", 1, 0, 1 << (int)CandidateValue.TRACK_SW)));
            Assert.That(step.Premises.Select(q => (q.IsPublicFact, q.TraceIndex)), Is.EqualTo(new[] { (false, 0) }), "#0 is the only premise");
            Assert.That(result.DeductionTrace[0].RemainingValues, Is.EqualTo(new[] { CandidateValue.TRACK_WN }));
            // Independent semantics: in every solvable puzzle of 2..4 x 2..4 cells, any endpoints, any counts, a solution with WN at
            // (0,0) never has SW at (1,0).
            int withWn = 0;
            for (int w = 2; w <= 4; w++)
                for (int h = 2; h <= 4; h++)
                {
                    var edges = SmallPathOracle.Edges(w, h);
                    foreach (var a in edges) foreach (var b in edges)
                    {
                        if (a.Side == b.Side && a.Index == b.Index) continue;
                        foreach (var board in SmallPathOracle.Boards(w, h, a, b))
                        {
                            if (board[0] - '0' != (int)CandidateValue.TRACK_WN) continue;
                            withWn++;
                            Assert.That(board[1] - '0', Is.Not.EqualTo((int)CandidateValue.TRACK_SW), $"{w}x{h} A={a.Side}/{a.Index} B={b.Side}/{b.Index} board={board}");
                        }
                    }
                }
            Assert.That(withWn, Is.GreaterThan(0));
            // The real neighbor stage derives it from #0 in every agreeing puzzle (no fact declared) and not without #0.
            var spec = SpecOf(step, result.DeductionTrace, "SingleCell");
            Assert.That(spec.Facts, Is.EqualTo(0));
            int agreeing = 0;
            foreach (var other in Agreeing(def, spec, 0, 4, grow: 2)) { agreeing++; Assert.That(Derives(other, spec, out _), Is.True, $"{other.Grid.Width}x{other.Grid.Height} A={other.A.Side}/{other.A.Index} B={other.B.Side}/{other.B.Index}"); }
            Assert.That(agreeing, Is.GreaterThan(100));
            Assert.That(Refutation(def, spec.Without(0), 0, 5), Is.Not.Null, "#0 is necessary");
        }

        private static byte[] Open(int cells) => Enumerable.Repeat((byte)127, cells).ToArray();
        private static PuzzleDefinition Single(int size)
        {
            var grid = GridSize.Create(size, size).Value!; var counts = new int[size]; counts[0] = 1;
            return PuzzleDefinition.Create(grid, Endpoint.Create(grid, Direction.N, 0).Value!, Endpoint.Create(grid, Direction.W, 0).Value!, counts, counts, "train-track-v1").Value!;
        }

        private static object NewEntry(PuzzleDefinition def, string rule, int x, int y, int removed)
        {
            var entryType = TraceAudit.Tracer.GetNestedType("Entry", BindingFlags.NonPublic)!; var entry = Activator.CreateInstance(entryType, true)!;
            void Set(string name, object? value) => entryType.GetField(name)!.SetValue(entry, value);
            Set("Cell", y * def.Grid.Width + x); Set("Rule", rule); Set("Removed", (byte)removed); Set("After", (byte)(127 & ~removed)); Set("Axis", null); Set("Line", -1);
            return entry;
        }
        private static object NewTracer(PuzzleDefinition def) => Activator.CreateInstance(TraceAudit.Tracer, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { def }, null)!;
        private static int MinimalFacts(PuzzleDefinition def, object entry, byte[] view) => (int)TraceAudit.Tracer.GetMethod("MinimalFacts", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(NewTracer(def), new[] { entry, view })!;
        private static bool Holds(PuzzleDefinition def, object entry, byte[] view) => (bool)TraceAudit.Tracer.GetMethod("Holds", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(NewTracer(def), new[] { entry, view })!;

        private static Spec Crafted(PuzzleDefinition def, string rule, int x, int y, int removed, byte[] view, out int facts)
        {
            facts = MinimalFacts(def, NewEntry(def, rule, x, y, removed), view);
            var premises = new List<(int, int, int)>();
            for (int c = 0; c < view.Length; c++) if (view[c] != 127) premises.Add((c % def.Grid.Width, c / def.Grid.Width, 127 & ~view[c]));
            return new Spec { Rule = rule, X = x, Y = y, Removed = removed, Premises = premises.ToArray(), Entries = Enumerable.Range(0, premises.Count).ToArray(), Facts = facts, Label = "crafted " + rule + "@" + x + "," + y };
        }

        private static (int X, int Y, int Removed) RealReduction(PuzzleDefinition def, byte[] view, int stage)
        {
            var sink = new List<RecoveryReduction>(); var copy = (byte[])view.Clone();
            Assert.That((bool)RunStage.Invoke(null, new object[] { def, copy, sink, stage })!, Is.True);
            Assert.That(sink.Count, Is.EqualTo(1));
            return (sink[0].Cell.X, sink[0].Cell.Y, sink[0].Before & ~sink[0].After);
        }

        // Root trace of an arbitrary (also contradictory) definition: the rare rules only occur there.
        private static IReadOnlyList<DeductionStep> RootTrace(PuzzleDefinition def)
        {
            var tracer = NewTracer(def); var runStage = typeof(PuzzleSolver).GetMethod("RunStageCore", BindingFlags.NonPublic | BindingFlags.Static)!;
            var domains = Enumerable.Repeat((byte)127, def.Grid.CellCount).ToArray(); var sink = new List<RecoveryReduction>();
            var work = new SortedSet<int>(Enumerable.Range(0, 6));
            while (work.Count > 0)
            {
                int stage = work.Min; work.Remove(stage); int before = sink.Count;
                if (!(bool)runStage.Invoke(null, new object[] { def, domains, sink, stage, tracer })!) break;
                if (sink.Count != before) foreach (int dirty in Enumerable.Range(0, 6)) work.Add(dirty);
            }
            Assert.That((bool)TraceAudit.Tracer.GetProperty("Faulted", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(tracer)!, Is.False);
            return (IReadOnlyList<DeductionStep>)TraceAudit.Tracer.GetMethod("Build", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(tracer, null)!;
        }

        [Test]
        public void RareRules_DegreeLoopAndConnectivity_DeclareSufficientAndIrredundantFacts()
        {
            var open3 = Open(9); var interior = Open(9); interior[1] = 1;
            var loop = new byte[] { 64, 1, 1, 1, 17, 32, 1, 8, 64 }; var loopReduction = RealReduction(Single(3), loop, 4);
            var conn = new byte[] { 64, 1, 1, 127 }; var connReduction = RealReduction(Single(2), conn, 5);
            // A named EMPTY east neighbor: the E port of EW at (1,1) is unsupported in every grid that contains that neighbor.
            var eastEmpty = Open(9); eastEmpty[5] = 1;
            // (1,2) of 3x3 is walled in by EMPTY cells; its only exterior side is S, so at most one endpoint can enter it.
            var oneDoor = Open(9); oneDoor[4] = 1; oneDoor[6] = 1; oneDoor[8] = 1; var oneDoorReduction = RealReduction(Single(3), oneDoor, 5);
            // 3x3, A = E/0, B = W/2: (1,0) has no E port, (0,0) and (1,1) are EMPTY. Only in a grid two wide would A's cell be (1,0),
            // and then B could not be reached; B itself is needed, because A and B could otherwise both use (1,0) there.
            var eastA = SolverV2Fixtures.D(3, 3, 1, 0, 3, 2, new[] { 1, 1, 3 }, new[] { 1, 1, 3 });
            var noEast = Open(9); noEast[0] = 1; noEast[1] = 1 | 2 | 32 | 64; noEast[4] = 1; var noEastReduction = RealReduction(eastA, noEast, 5);
            // Crafted states name every cell of the view as a premise; only the real root traces have minimised entries.
            var cases = new List<(PuzzleDefinition Def, Spec Spec, int Expected)>
            {
                // NS at the border cell (1,0) of 3x3 (A=N/0, B=W/0): its N port is no endpoint port, which either endpoint could
                // change; its S port is answered by the open cell below. The N side is exterior in every grid: A and B, no grid size.
                (Single(3), Crafted(Single(3), "LOCAL_DEGREE_REQUIRED", 1, 0, 1 << 1, open3, out _), FA | FB),
                // NS at the interior cell (1,1) whose N neighbor (named by a premise) is empty: no public fact at all.
                (Single(3), Crafted(Single(3), "LOCAL_DEGREE_REQUIRED", 1, 1, 1 << 1, interior, out _), 0),
                // EMPTY removed at the cell of endpoint A (TopPair: A=N/0 at (0,0), B=N/1): A alone, the N side needs no grid size.
                (SolverV2Fixtures.TopPair(), Crafted(SolverV2Fixtures.TopPair(), "ENDPOINT_ENTRY_REQUIRED", 0, 0, 1 << 0, Open(4), out _), FA),
                (SolverV2Fixtures.TopPair(), Crafted(SolverV2Fixtures.TopPair(), "ENDPOINT_ENTRY_REQUIRED", 1, 0, 1 << 0, Open(4), out _), FB),
                // EMPTY removed at the cell of an E-side endpoint: the cell is the endpoint cell only in a grid of this width.
                (SolverV2Fixtures.DeepDiscard4x4(), Crafted(SolverV2Fixtures.DeepDiscard4x4(), "ENDPOINT_ENTRY_REQUIRED", 3, 2, 1 << 0, Open(16), out _), FGrid | FB),
                // EW at the E border cell (2,1): only its E port can fail, which is exterior only while the grid is three wide (in a
                // wider grid the unnamed cell behind it answers the port) and then no endpoint port: grid size, A and B.
                (Single(3), Crafted(Single(3), "LOCAL_DEGREE_REQUIRED", 2, 1, 1 << 2, open3, out _), FGrid | FA | FB),
                // EW at (1,1) with the named EMPTY cell (2,1) to its east: no public fact (the named cell rules out a narrower grid).
                (Single(3), Crafted(Single(3), "LOCAL_DEGREE_REQUIRED", 1, 1, 1 << 2, eastEmpty, out _), 0),
                // The loop rule compares premise cells only.
                (Single(3), Crafted(Single(3), "LOOP_PREVENTION", loopReduction.X, loopReduction.Y, loopReduction.Removed, loop, out _), 0),
                // (0,0) must be WN, has no interior port and lies outside the component of (1,1) in every grid: every agreeing puzzle
                // that reached (1,1) would contradict, so the step holds without any public fact.
                (Single(2), Crafted(Single(2), "CONNECTIVITY_PRESERVATION", connReduction.X, connReduction.Y, connReduction.Removed, conn, out _), 0),
                // One exterior port in the component of (1,2): A and B cannot both be there, the grid size (a taller grid opens S) is needed.
                (Single(3), Crafted(Single(3), "CONNECTIVITY_PRESERVATION", oneDoorReduction.X, oneDoorReduction.Y, oneDoorReduction.Removed, oneDoor, out _), FGrid),
                (eastA, Crafted(eastA, "CONNECTIVITY_PRESERVATION", noEastReduction.X, noEastReduction.Y, noEastReduction.Removed, noEast, out _), FB)
            };
            // Connectivity steps of real root propagations (the rule never fires on the UNIQUE corpus): one needing the grid size and A,
            // one needing only the grid size (the SW corner cell at the border is isolated only while the S side is exterior).
            foreach (var (def, x, y, expected) in new[]
            {
                (SolverV2Fixtures.D(4, 4, 0, 2, 1, 0, new[] { 2, 0, 1, 1 }, new[] { 0, 0, 2, 2 }), 2, 2, FGrid | FA),
                (SolverV2Fixtures.D(3, 3, 3, 2, 2, 0, new[] { 2, 1, 2 }, new[] { 2, 2, 1 }), 0, 0, FGrid)
            })
            {
                var trace = RootTrace(def); var step = trace.Single(t => t.RuleCode == "CONNECTIVITY_PRESERVATION" && t.Cell.X == x && t.Cell.Y == y);
                cases.Add((def, SpecOf(step, trace, $"root {def.Grid.Width}x{def.Grid.Height}"), expected));
                Assert.That(TraceAudit.Audit(def, trace, true), Is.Null);
            }
            foreach (var (def, spec, expected) in cases)
            {
                Assert.That(spec.Facts, Is.EqualTo(expected), $"{spec.Label} declares [{Facts(spec.Facts)}], expected [{Facts(expected)}]");
                Assert.That(Derives(def, spec, out bool vacuous) && !vacuous, Is.True, spec.Label + " in its own puzzle");
                int perturbed = 0;
                foreach (var other in Agreeing(def, spec, spec.Facts, 5150, grow: 1))
                {
                    perturbed++;
                    if (!Derives(other, spec, out _)) Assert.Fail($"{spec.Label} declares [{Facts(spec.Facts)}] but is not derived for {other.Grid.Width}x{other.Grid.Height} A={other.A.Side}/{other.A.Index} B={other.B.Side}/{other.B.Index}");
                }
                Assert.That(perturbed, Is.GreaterThan(0), spec.Label + ": the oracle must perturb");
                foreach (int bit in new[] { FGrid, FA, FB })
                    if ((spec.Facts & bit) != 0) Assert.That(Refutation(def, spec, spec.Facts & ~bit, 5151), Is.Not.Null, $"{spec.Label}: releasing {Facts(bit)} must be refuted by a valid puzzle");
                if (spec.Label.StartsWith("root", StringComparison.Ordinal))
                    foreach (int entry in spec.Entries)
                        Assert.That(Refutation(def, spec.Without(entry), spec.Facts, 5152), Is.Not.Null, $"{spec.Label}: entry {entry} must be necessary");
            }
        }

        [Test]
        public void GridSizeBound_AStepAtTheMaximumWidthNeedsNoGridSizeForItsEastSide()
        {
            // The domain admits at most 32 columns. A cell in column 31 has an exterior E side in every agreeing puzzle, so a removal
            // that rests on that side needs the endpoints but not the grid size; LINE_ALL_REMAINING_OCCUPIED on a 32-cell row needs only
            // its count (a shorter row would contradict the count). One column fewer, both need the grid size again.
            // Straight path W/0 to E/0 along row 0 of a three-row grid; the border step is the one at (width-1, 1), whose only possibly
            // exterior side besides E would be S, which it does not rely on (it removes the shapes with an E port).
            foreach (int width in new[] { 32, 31 })
            {
                var rows = new int[3]; rows[0] = width; var cols = Enumerable.Repeat(1, width).ToArray();
                var def = SolverV2Fixtures.D(width, 3, 3, 0, 1, 0, rows, cols);
                var result = PuzzleSolver.Solve(def); Assert.That(result.Classification, Is.EqualTo(SolverClassification.UNIQUE));
                var border = result.DeductionTrace.First(t => t.RuleCode == "BORDER_PORT_FORBIDDEN" && t.Cell.X == width - 1 && t.Cell.Y == 1);
                var full = result.DeductionTrace.First(t => t.RuleCode == "LINE_ALL_REMAINING_OCCUPIED");
                var expectBorder = width == 32 ? new PublicFactKind?[] { PublicFactKind.ENDPOINT_A, PublicFactKind.ENDPOINT_B } : new PublicFactKind?[] { PublicFactKind.GRID_SIZE, PublicFactKind.ENDPOINT_A, PublicFactKind.ENDPOINT_B };
                Assert.That(border.Premises.Where(q => q.IsPublicFact).Select(q => q.PublicFact), Is.EqualTo(expectBorder), "border step at x=" + (width - 1));
                Assert.That(full.Premises.Where(q => q.IsPublicFact).Select(q => q.PublicFact), Is.EqualTo(width == 32 ? new PublicFactKind?[] { PublicFactKind.ROW_COUNT } : new PublicFactKind?[] { PublicFactKind.GRID_SIZE, PublicFactKind.ROW_COUNT }));
                Assert.That(TraceAudit.Audit(def, result.DeductionTrace, true), Is.Null);
                var spec = SpecOf(border, result.DeductionTrace, width + "x2");
                int agreeing = 0;
                foreach (var other in Agreeing(def, spec, spec.Facts, 9, maxCombinations: 400)) { agreeing++; Assert.That(Derives(other, spec, out _), Is.True, $"{other.Grid.Width}x{other.Grid.Height} A={other.A.Side}/{other.A.Index} B={other.B.Side}/{other.B.Index}"); }
                Assert.That(agreeing, Is.GreaterThan(0));
                if (width == 31) Assert.That(Refutation(def, spec, spec.Facts & ~FGrid, 10), Is.Not.Null, "one column fewer than the maximum, a wider grid refutes the step without the grid size");
            }
        }

        [Test]
        public void BorderPredicate_AgreesWithTheRealStageZero_ForEveryCellValueAndEndpointPair()
        {
            // The tracer evaluates the border rule on the local situation of the cell. With every public fact declared that must be
            // exactly the real stage 0 of the solver, for every cell, every value and every endpoint pair.
            int checkedValues = 0;
            foreach (var (w, h) in new[] { (2, 2), (2, 3), (3, 2), (3, 3) })
            {
                var edges = SmallPathOracle.Edges(w, h);
                foreach (var a in edges) foreach (var b in edges)
                {
                    if (a.Side == b.Side && a.Index == b.Index) continue;
                    var counts = SmallPathOracle.Counts(w, h, a, b); if (counts.Count == 0) continue;
                    var parts = counts.Keys.OrderBy(k => k, StringComparer.Ordinal).First().Split(';');
                    var def = TraceAudit.Define(w, h, a, b, parts[0].Split(',').Select(int.Parse).ToArray(), parts[1].Split(',').Select(int.Parse).ToArray());
                    var domains = Open(w * h); Assert.That((bool)RunStage.Invoke(null, new object[] { def, domains, new List<RecoveryReduction>(), 0 })!, Is.True);
                    for (int cell = 0; cell < w * h; cell++)
                        for (int v = 0; v < 7; v++)
                        {
                            bool removedByStage = (domains[cell] & (1 << v)) == 0;
                            Assert.That(Holds(def, NewEntry(def, "BORDER_PORT_FORBIDDEN", cell % w, cell / w, 1 << v), Open(w * h)), Is.EqualTo(removedByStage), $"{w}x{h} A={a.Side}/{a.Index} B={b.Side}/{b.Index} cell {cell} value {v}");
                            checkedValues++;
                        }
                }
            }
            Assert.That(checkedValues, Is.GreaterThan(10000));
        }

        [Test]
        public void Audit_RejectsAnyPublicFactSetThatIsInsufficientOrRedundant()
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
                        // A line rule without its row/column fact has no line at all; every other missing fact is a missing premise.
                        bool lineFact = facts[drop].PublicFact == PublicFactKind.ROW_COUNT || facts[drop].PublicFact == PublicFactKind.COLUMN_COUNT;
                        Assert.That(TraceAudit.Audit(def, With(kept), false), Does.Contain(lineFact && trace[i].RuleCode.StartsWith("LINE_", StringComparison.Ordinal) ? "line fact" : "public facts"), $"step {i} {trace[i].RuleCode} without {facts[drop].PublicFact}"); mutated++;
                    }
                    foreach (var extra in new[] { TraceAudit.Fact(PublicFactKind.GRID_SIZE), TraceAudit.Fact(PublicFactKind.ENDPOINT_A), TraceAudit.Fact(PublicFactKind.ENDPOINT_B) })
                    {
                        if (facts.Any(q => q.PublicFact == extra.PublicFact)) continue;
                        // Facts stay in their canonical order: grid, A, B, then the line count. A dispensable fact is rejected even if
                        // irredundancy of the entries is not required: the public facts are part of every step's premise contract.
                        var all = facts.Concat(new[] { extra }).OrderBy(q => (int)q.PublicFact!.Value).ThenBy(q => q.LineIndex).Concat(entries).ToArray();
                        Assert.That(TraceAudit.Audit(def, With(all), false), Does.Contain("public facts"), $"step {i} {trace[i].RuleCode} plus dispensable {extra.PublicFact}"); mutated++;
                    }
                    bool isLine = trace[i].RuleCode.StartsWith("LINE_", StringComparison.Ordinal);
                    if (!isLine) { Assert.That(TraceAudit.Audit(def, With(facts.Concat(new[] { TraceAudit.Fact(PublicFactKind.ROW_COUNT, 0) }).Concat(entries).ToArray()), false), Is.Not.Null); mutated++; }
                    // Duplicated or reordered facts are not canonical.
                    if (facts.Length >= 1) { Assert.That(TraceAudit.Audit(def, With(facts.Concat(facts.Take(1)).Concat(entries).ToArray()), false), Does.Contain("public facts"), $"step {i} duplicated fact"); mutated++; }
                    if (facts.Length >= 2) { Assert.That(TraceAudit.Audit(def, With(facts.Reverse().Concat(entries).ToArray()), false), Does.Contain("public facts"), $"step {i} reordered facts"); mutated++; }
                }
            }
            // Earlier deliveries: grid size only on a border step and one endpoint on an endpoint step (first QC, insufficient), the grid
            // size on a neighbor step and on an endpoint step at the N/W corner (second QC, redundant).
            var top = SolverV2Fixtures.TopPair(); var topTrace = PuzzleSolver.Solve(top).DeductionTrace.ToArray();
            DeductionStep[] Old(int index, params DeductionPremise[] premises) { var copy = (DeductionStep[])topTrace.Clone(); copy[index] = TraceAudit.With(topTrace[index], top, premises); return copy; }
            Assert.That(TraceAudit.Audit(top, Old(2, TraceAudit.Fact(PublicFactKind.GRID_SIZE)), true), Does.Contain("insufficient"), "border step with the grid size only");
            Assert.That(TraceAudit.Audit(top, Old(0, TraceAudit.Fact(PublicFactKind.GRID_SIZE), TraceAudit.Fact(PublicFactKind.ENDPOINT_A)), true), Does.Contain("insufficient"), "endpoint step with its own endpoint only");
            Assert.That(TraceAudit.Audit(top, Old(6, TraceAudit.Fact(PublicFactKind.GRID_SIZE), TraceAudit.Entry(2), TraceAudit.Entry(4)), true), Does.Contain("public facts"), "neighbor step with the grid size");
            Assert.That(TraceAudit.Audit(top, Old(0, TraceAudit.Fact(PublicFactKind.GRID_SIZE), TraceAudit.Fact(PublicFactKind.ENDPOINT_A), TraceAudit.Fact(PublicFactKind.ENDPOINT_B)), true), Does.Contain("GRID_SIZE is redundant"), "endpoint step at the N/W corner with the grid size");
            Assert.That(mutated, Is.GreaterThan(50));
        }
    }
}
