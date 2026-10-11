using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Solver.EditMode
{
    // FIXTURE_ONLY. Independent trace audit for AK-03/AK-07: it recomputes depth,
    // steps and soundness from the published trace and an oracle-derived solution
    // only; it never calls solver internals.
    internal static class TraceAudit
    {
        internal static readonly string[] RuleCodes =
        {
            "BORDER_PORT_FORBIDDEN", "ENDPOINT_ENTRY_REQUIRED", "NEIGHBOR_PORT_REQUIRED", "LINE_ZERO",
            "LINE_ALL_REMAINING_OCCUPIED", "LINE_TARGET_REACHED", "LOCAL_DEGREE_REQUIRED", "LOOP_PREVENTION", "CONNECTIVITY_PRESERVATION"
        };

        internal static PuzzleDefinition Define(int w, int h, SmallPathOracle.Edge a, SmallPathOracle.Edge b, int[] rows, int[] cols)
        {
            var size = GridSize.Create(w, h).Value!;
            return PuzzleDefinition.Create(size, Endpoint.Create(size, (Direction)a.Side, a.Index).Value!, Endpoint.Create(size, (Direction)b.Side, b.Index).Value!, rows, cols, "train-track-v1").Value!;
        }

        // Independent Steps/Depth recomputation from the premises alone (ADR-031: public facts 0, no derived premise 1, else 1 + max).
        internal static (int Steps, int MaxDepth) Recompute(IReadOnlyList<DeductionStep> trace)
        {
            var depth = new int[trace.Count]; int max = 0;
            for (int i = 0; i < trace.Count; i++)
            {
                int derived = 0;
                foreach (var premise in trace[i].Premises)
                {
                    if (premise.IsPublicFact) continue;
                    Assert.That(premise.TraceIndex, Is.InRange(0, i - 1), $"step {i} may only reference earlier entries");
                    derived = Math.Max(derived, depth[premise.TraceIndex]);
                }
                depth[i] = derived + 1; max = Math.Max(max, depth[i]);
            }
            return (trace.Count, max);
        }

        // The cell values of the one solution as seen by an observer who only knows the A-B path and the endpoint sides.
        internal static int[] SolutionValues(PuzzleDefinition def, IReadOnlyList<CellCoordinate> path)
        {
            var values = new int[def.Grid.CellCount];
            int Dir(CellCoordinate from, CellCoordinate to) => to.Y < from.Y ? 0 : to.X > from.X ? 1 : to.Y > from.Y ? 2 : 3;
            for (int k = 0; k < path.Count; k++)
            {
                int mask = 0;
                mask |= 1 << (k == 0 ? (int)def.A.Side : Dir(path[k], path[k - 1]));
                mask |= 1 << (k == path.Count - 1 ? (int)def.B.Side : Dir(path[k], path[k + 1]));
                int value = mask == 5 ? 1 : mask == 10 ? 2 : mask == 3 ? 3 : mask == 6 ? 4 : mask == 12 ? 5 : mask == 9 ? 6 : -1;
                Assert.That(value, Is.Not.EqualTo(-1), "path cell must have exactly two distinct ports");
                values[path[k].Y * def.Grid.Width + path[k].X] = value;
            }
            return values;
        }

        internal static void AssertWellFormedAndSound(PuzzleDefinition def, SolverResult result, string label)
        {
            Assert.That(result.Classification, Is.EqualTo(SolverClassification.UNIQUE), label);
            var trace = result.DeductionTrace; var metrics = result.Metrics!;
            Assert.That(metrics, Is.Not.Null, label);
            var solution = SolutionValues(def, result.Path);
            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < trace.Count; i++)
            {
                var step = trace[i];
                Assert.That(step.Index, Is.EqualTo(i), label);
                Assert.That(RuleCodes, Has.Member(step.RuleCode), label);
                Assert.That(step.RemovedValues, Is.Not.Empty, label); Assert.That(step.RemainingValues, Is.Not.Empty, label);
                Assert.That(step.RemovedValues.Intersect(step.RemainingValues), Is.Empty, label);
                Assert.That(step.RemovedValues.Select(v => (int)v), Is.Ordered.And.Unique, label);
                Assert.That(seen.Add(step.RuleCode + step.Cell.X + "," + step.Cell.Y + string.Join(",", step.RemovedValues)), Is.True, label + " duplicates must be removed");
                int cell = step.Cell.Y * def.Grid.Width + step.Cell.X;
                // Safe: the removed values never include the value of the one solution; the remaining ones always do.
                Assert.That(step.RemovedValues.Select(v => (int)v), Has.No.Member(solution[cell]), $"{label} step {i} {step.RuleCode} removes the solution value");
                Assert.That(step.RemainingValues.Select(v => (int)v), Has.Member(solution[cell]), label);
                var facts = step.Premises.Where(q => q.IsPublicFact).ToArray();
                Assert.That(step.Premises.Take(facts.Length).All(q => q.IsPublicFact), Is.True, label + " public facts come first");
                var refs = step.Premises.Where(q => !q.IsPublicFact).Select(q => q.TraceIndex).ToArray();
                Assert.That(refs, Is.Ordered.And.Unique, label); Assert.That(refs.All(r => r < i), Is.True, label);
                foreach (var q in facts)
                {
                    Assert.That(q.TraceIndex, Is.EqualTo(-1));
                    if (q.PublicFact == PublicFactKind.ROW_COUNT) Assert.That(q.LineIndex, Is.InRange(0, def.Grid.Height - 1));
                    else if (q.PublicFact == PublicFactKind.COLUMN_COUNT) Assert.That(q.LineIndex, Is.InRange(0, def.Grid.Width - 1));
                    else Assert.That(q.LineIndex, Is.EqualTo(-1));
                }
            }
            var recomputed = Recompute(trace);
            Assert.That(metrics.DeductionSteps, Is.EqualTo(recomputed.Steps), label);
            Assert.That(metrics.MaxDeductionDepth, Is.EqualTo(recomputed.MaxDepth), label);
            for (int i = 0; i < trace.Count; i++)
            {
                int derived = trace[i].Premises.Where(q => !q.IsPublicFact).Select(q => trace[q.TraceIndex].Depth).DefaultIfEmpty(0).Max();
                Assert.That(trace[i].Depth, Is.EqualTo(derived + 1), $"{label} step {i}");
            }
            Assert.That(metrics.SearchNodes, Is.GreaterThanOrEqualTo(metrics.RequiredGuessDepth), label);
            Assert.That(metrics.RequiredGuessDepth == 0, Is.EqualTo(metrics.SearchNodes == 0), label + " a root-only solution begins no assumption, and any assumption raises the path depth");
            // The recovery protocol starts with the root reductions of the trace; branch reductions follow and are not metrics.
            Assert.That(result.Reductions.Count, Is.GreaterThanOrEqualTo(trace.Count), label);
            for (int i = 0; i < trace.Count; i++)
            {
                var recovery = result.Reductions[i];
                Assert.That((recovery.Cell.X, recovery.Cell.Y, recovery.RuleCode), Is.EqualTo((trace[i].Cell.X, trace[i].Cell.Y, trace[i].RuleCode)), label);
                Assert.That(Enumerable.Range(0, 7).Where(v => ((recovery.Before & ~recovery.After) & (1 << v)) != 0), Is.EqualTo(trace[i].RemovedValues.Select(v => (int)v)), label);
            }
            Assert.That(Audit(def, trace, true), Is.Null, label);
        }

        // The rule-level re-derivation of every step from exactly its premises (internal verifier).
        internal static string? Audit(PuzzleDefinition def, IReadOnlyList<DeductionStep> trace, bool requireIrredundant)
            => (string?)Tracer.GetMethod("Audit", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, new object[] { def, trace, requireIrredundant });

        internal static Type Tracer => typeof(PuzzleSolver).Assembly.GetType("STP.Puzzle.Solver.RootDeductionTracer", throwOnError: true)!;

        internal static DeductionPremise Fact(PublicFactKind kind, int line = -1)
            => (DeductionPremise)typeof(DeductionPremise).GetMethod("Fact", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, new object[] { kind, line })!;
        internal static DeductionPremise Entry(int index)
            => (DeductionPremise)typeof(DeductionPremise).GetMethod("Entry", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, new object[] { index })!;
        internal static DeductionStep Step(int index, string rule, PuzzleDefinition def, int x, int y, CandidateValue[] removed, CandidateValue[] remaining, DeductionPremise[] premises, int depth)
            => (DeductionStep)typeof(DeductionStep).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0].Invoke(new object[] { index, rule, CellCoordinate.Create(def.Grid, x, y).Value!, removed, remaining, premises, depth });
        internal static DeductionStep With(DeductionStep s, PuzzleDefinition def, DeductionPremise[]? premises = null, int? depth = null, int? index = null, string? rule = null)
            => Step(index ?? s.Index, rule ?? s.RuleCode, def, s.Cell.X, s.Cell.Y, s.RemovedValues.ToArray(), s.RemainingValues.ToArray(), premises ?? s.Premises.ToArray(), depth ?? s.Depth);
    }

    // FIXTURE_ONLY reference puzzles. Every claim below was derived by hand from the public rules and is documented
    // next to the test that uses it; none of them is Season-1 content.
    internal static class SolverV2Fixtures
    {
        internal static PuzzleDefinition D(int w, int h, int aSide, int aIndex, int bSide, int bIndex, int[] rows, int[] cols)
            => TraceAudit.Define(w, h, new SmallPathOracle.Edge(aSide, aIndex), new SmallPathOracle.Edge(bSide, bIndex), rows, cols);
        // 2x2, A=N/0 and B=W/0 are the same cell: the single WN cell, everything else empty.
        internal static PuzzleDefinition SingleCell() => D(2, 2, 0, 0, 3, 0, new[] { 1, 0 }, new[] { 1, 0 });
        // 2x2, A=N/0, B=N/1: the two top cells form the whole path.
        internal static PuzzleDefinition TopPair() => D(2, 2, 0, 0, 0, 1, new[] { 2, 0 }, new[] { 1, 1 });
        // 2x3 snake from N/0 to N/1: root leaves the first cell open, search needs one assumption.
        internal static PuzzleDefinition Snake2x3() => D(2, 3, 0, 0, 0, 1, new[] { 2, 2, 2 }, new[] { 3, 3 });
        // 4x4, A=N/3 to B=E/2: the first branch (EW at the first open cell) is discarded after a nested subtree, the success path needs one assumption.
        internal static PuzzleDefinition DeepDiscard4x4() => D(4, 4, 0, 3, 1, 2, new[] { 4, 3, 3, 3 }, new[] { 3, 4, 3, 3 });
        internal static PuzzleDefinition Guess2() => D(4, 4, 0, 0, 0, 1, new[] { 4, 4, 2, 2 }, new[] { 2, 2, 4, 4 });
        internal static PuzzleDefinition Guess3() => D(4, 4, 0, 0, 0, 1, new[] { 4, 4, 2, 2 }, new[] { 4, 4, 2, 2 });
        // 2x2, N/0 to W/0 with rows (2,0) and columns (1,1): no path; the independent oracle has no entry.
        internal static PuzzleDefinition Unsatisfiable() => D(2, 2, 0, 0, 3, 0, new[] { 2, 0 }, new[] { 1, 1 });
    }
}
