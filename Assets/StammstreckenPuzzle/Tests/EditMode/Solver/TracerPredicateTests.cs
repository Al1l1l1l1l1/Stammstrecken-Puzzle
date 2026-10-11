using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Solver.EditMode
{
    // WP-024 Phase B: the per-rule predicates of the trace verifier are checked against the real stage code on the
    // crafted constraint states of ConstraintContractTests. These states do not occur in any root propagation of the
    // fixtures (stage ordering removes the candidates earlier), so only a white-box test can pin the predicates.
    public sealed class TracerPredicateTests
    {
        private static PuzzleDefinition Single(int size)
        {
            var grid = GridSize.Create(size, size).Value!; var counts = new int[size]; counts[0] = 1;
            return PuzzleDefinition.Create(grid, Endpoint.Create(grid, Direction.N, 0).Value!, Endpoint.Create(grid, Direction.W, 0).Value!, counts, counts, "train-track-v1").Value!;
        }
        private static MethodInfo Method(string name) => typeof(PuzzleSolver).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)!;
        private static bool Stage(PuzzleDefinition p, byte[] domains, List<RecoveryReduction> trace, int stage) => (bool)Method("RunStage").Invoke(null, new object[] { p, domains, trace, stage })!;
        private static object NewTracer(PuzzleDefinition def) => Activator.CreateInstance(TraceAudit.Tracer, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { def }, null)!;
        private static Type EntryType => TraceAudit.Tracer.GetNestedType("Entry", BindingFlags.NonPublic)!;
        private static object NewEntry(PuzzleDefinition def, string rule, int cell, int removedMask, string? axis = null, int line = -1)
        {
            var entry = Activator.CreateInstance(EntryType, true)!;
            void Set(string name, object? value) => EntryType.GetField(name)!.SetValue(entry, value);
            Set("Cell", cell); Set("Rule", rule); Set("Removed", (byte)removedMask); Set("After", (byte)(127 & ~removedMask)); Set("Axis", axis); Set("Line", line);
            return entry;
        }
        private static object EntryOf(PuzzleDefinition def, RecoveryReduction r)
            => NewEntry(def, r.RuleCode, r.Cell.Y * def.Grid.Width + r.Cell.X, r.Before & ~r.After, r.LineAxis, r.LineIndex);
        private static bool Holds(PuzzleDefinition def, object entry, byte[] view)
            => (bool)TraceAudit.Tracer.GetMethod("Holds", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(NewTracer(def), new[] { entry, view })!;
        private static byte[] Open(int cells) => Enumerable.Repeat((byte)127, cells).ToArray();

        [Test]
        public void LoopPredicate_AgreesWithTheRealLoopStage()
        {
            var p = Single(3); var masks = new byte[] { 64, 1, 1, 1, 17, 32, 1, 8, 64 }; var view = (byte[])masks.Clone();
            var trace = new List<RecoveryReduction>();
            Assert.That(Stage(p, masks, trace, 4), Is.True);
            Assert.That(trace.Count, Is.EqualTo(1)); Assert.That(trace[0].RuleCode, Is.EqualTo("LOOP_PREVENTION"));
            var entry = EntryOf(p, trace[0]);
            Assert.That(Holds(p, entry, view), Is.True, "the stage's own removal is re-derived from the pre-stage view");

            Assert.That(Holds(p, NewEntry(p, "LOOP_PREVENTION", 4, 1 << 0), view), Is.False, "EMPTY never closes a loop");
            Assert.That(Holds(p, NewEntry(p, "LOOP_PREVENTION", 4, 1 << 1), view), Is.False, "NS has only one available neighbor port here");
            Assert.That(Holds(p, NewEntry(p, "LOOP_PREVENTION", 4, 1 << 4), Open(9)), Is.False, "without a mandatory chain nothing closes a loop");
            var loop = new byte[] { 64, 1, 1, 1, 16, 32, 1, 8, 64 };
            Assert.That(Stage(p, (byte[])loop.Clone(), new List<RecoveryReduction>(), 4), Is.False, "a mandatory loop contradicts the stage");
            Assert.That(Holds(p, entry, loop), Is.False, "a view that already contains a loop is a contradiction, not a derivation");
        }

        [Test]
        public void DegreePredicate_AgreesWithTheRealDegreeStage_IncludingExteriorPorts()
        {
            var p = Single(3); var masks = Open(9); masks[4] = 3; masks[1] = 1; masks[7] = 1; var view = (byte[])masks.Clone();
            var trace = new List<RecoveryReduction>();
            Assert.That(Stage(p, masks, trace, 3), Is.True);
            var first = trace.First(r => r.RuleCode == "LOCAL_DEGREE_REQUIRED" && r.Cell.X == 1 && r.Cell.Y == 1);
            Assert.That(Holds(p, EntryOf(p, first), view), Is.True);

            Assert.That(Holds(p, NewEntry(p, "LOCAL_DEGREE_REQUIRED", 4, 1 << 0), view), Is.False, "EMPTY is not a degree violation");
            Assert.That(Holds(p, NewEntry(p, "LOCAL_DEGREE_REQUIRED", 4, 1 << 1), Open(9)), Is.False, "two supported ports are a valid degree");
            // Exterior ports: only an endpoint port counts as support.
            var endpointCell = Open(9); endpointCell[3] = 1;
            Assert.That(Holds(p, NewEntry(p, "LOCAL_DEGREE_REQUIRED", 0, 1 << 1), endpointCell), Is.True, "N is the endpoint port of the corner, S is unavailable");
            Assert.That(Holds(p, NewEntry(p, "LOCAL_DEGREE_REQUIRED", 0, 1 << 1), Open(9)), Is.False, "N endpoint port plus an open S neighbor give degree two");
            Assert.That(Holds(p, NewEntry(p, "LOCAL_DEGREE_REQUIRED", 1, 1 << 1), Open(9)), Is.True, "N of (1,0) is a plain border port, so only S supports it");
        }

        [Test]
        public void BorderPredicate_RejectsOnlyExteriorViolations()
        {
            var p = Single(3);
            Assert.That(Holds(p, NewEntry(p, "BORDER_PORT_FORBIDDEN", 4, 1 << 1), Open(9)), Is.False, "an interior cell has no exterior port to forbid");
            Assert.That(Holds(p, NewEntry(p, "BORDER_PORT_FORBIDDEN", 1, 1 << 1), Open(9)), Is.True, "NS at the top border uses the exterior N port");
            Assert.That(Holds(p, NewEntry(p, "ENDPOINT_ENTRY_REQUIRED", 0, 1 << 0), Open(9)), Is.True, "an endpoint cell cannot be empty");
        }

        [Test]
        public void ConnectivityStep_IsBuiltWithItsThreePublicFacts_AndAUnjustifiedStateIsAFault()
        {
            var p = Single(2); var masks = new byte[] { 64, 1, 1, 127 }; var view = (byte[])masks.Clone();
            var tracer = NewTracer(p); var trace = new List<RecoveryReduction>();
            Assert.That((bool)Method("RunStageCore").Invoke(null, new object?[] { p, masks, trace, 5, tracer })!, Is.True);
            var steps = (IReadOnlyList<DeductionStep>)TraceAudit.Tracer.GetMethod("Build", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(tracer, null)!;
            Assert.That(steps.Count, Is.EqualTo(1));
            Assert.That(steps[0].RuleCode, Is.EqualTo("CONNECTIVITY_PRESERVATION")); Assert.That((steps[0].Cell.X, steps[0].Cell.Y), Is.EqualTo((1, 1)));
            Assert.That(steps[0].Premises.Select(q => q.PublicFact), Is.EqualTo(new PublicFactKind?[] { PublicFactKind.GRID_SIZE, PublicFactKind.ENDPOINT_A, PublicFactKind.ENDPOINT_B }));
            Assert.That(steps[0].Premises.All(q => q.IsPublicFact), Is.True);
            Assert.That(steps[0].Depth, Is.EqualTo(1));
            Assert.That((bool)TraceAudit.Tracer.GetProperty("Faulted", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(tracer)!, Is.True, "the crafted state was not produced by any recorded entry");
            Assert.That(Holds(p, EntryOf(p, trace[0]), view), Is.True, "the connectivity predicate re-derives the stage removal");
        }

        [Test]
        public void TraceAudit_RejectsMalformedValueSetsAndLineFacts()
        {
            var def = SolverV2Fixtures.TopPair(); var trace = PuzzleSolver.Solve(def).DeductionTrace.ToArray();
            var s0 = trace[0]; var premises = s0.Premises.ToArray();
            DeductionStep[] Replace(DeductionStep step) { var copy = (DeductionStep[])trace.Clone(); copy[0] = step; return copy; }
            DeductionStep Make(string rule, CandidateValue[] removed, CandidateValue[] remaining, DeductionPremise[] facts)
                => TraceAudit.Step(0, rule, def, s0.Cell.X, s0.Cell.Y, removed, remaining, facts, 1);
            Assert.That(TraceAudit.Audit(def, Replace(Make(s0.RuleCode, Array.Empty<CandidateValue>(), s0.RemainingValues.ToArray(), premises)), false), Does.Contain("removed/remaining"));
            Assert.That(TraceAudit.Audit(def, Replace(Make(s0.RuleCode, s0.RemovedValues.ToArray(), Array.Empty<CandidateValue>(), premises)), false), Does.Contain("removed/remaining"));
            Assert.That(TraceAudit.Audit(def, Replace(Make(s0.RuleCode, s0.RemovedValues.ToArray(), s0.RemovedValues.ToArray(), premises)), false), Does.Contain("removed/remaining"));
            var grid = new[] { TraceAudit.Fact(PublicFactKind.GRID_SIZE) };
            Assert.That(TraceAudit.Audit(def, Replace(Make("LINE_ZERO", s0.RemovedValues.ToArray(), s0.RemainingValues.ToArray(), grid)), false), Does.Contain("line fact"), "a line rule needs its row or column fact");
            Assert.That(TraceAudit.Audit(def, Replace(Make("LINE_ZERO", s0.RemovedValues.ToArray(), s0.RemainingValues.ToArray(), new[] { TraceAudit.Fact(PublicFactKind.ROW_COUNT, 9) })), false), Does.Contain("line fact"), "row index out of range");
            Assert.That(TraceAudit.Audit(def, Replace(Make("LINE_ZERO", s0.RemovedValues.ToArray(), s0.RemainingValues.ToArray(), new[] { TraceAudit.Fact(PublicFactKind.COLUMN_COUNT, 2) })), false), Does.Contain("line fact"), "column index out of range");
        }
    }
}
