using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Solver.EditMode
{
    public sealed class ConstraintContractTests
    {
        private static PuzzleDefinition Single(int size = 2)
        {
            var grid = GridSize.Create(size, size).Value!; var counts = new int[size]; counts[0] = 1;
            return PuzzleDefinition.Create(grid, Endpoint.Create(grid, Direction.N, 0).Value!, Endpoint.Create(grid, Direction.W, 0).Value!, counts, counts, "train-track-v1").Value!;
        }
        private static bool Stage(PuzzleDefinition p, byte[] domains, List<RecoveryReduction> trace, int stage) => (bool)typeof(PuzzleSolver).GetMethod("RunStage", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, new object[] { p, domains, trace, stage })!;

        [Test]
        public void BorderAndEndpointConstraint_RequiresOnlyTheExactExteriorPorts()
        {
            var p = Single(); var masks = Enumerable.Repeat((byte)127, 4).ToArray(); var trace = new List<RecoveryReduction>();
            Assert.That(Stage(p, masks, trace, 0), Is.True);
            Assert.That(masks[0], Is.EqualTo(64)); // only WN connects the two external edges
            Assert.That(masks[3], Is.EqualTo(1 | 64)); // EMPTY or WN: east and south are exterior
            Assert.That(trace.Any(r => r.RuleCode == "ENDPOINT_ENTRY_REQUIRED"), Is.True);
            Assert.That(trace.Any(r => r.RuleCode == "BORDER_PORT_FORBIDDEN"), Is.True);
        }
        [Test]
        public void NeighborConstraint_IsBiconditionalIncludingAbsentPorts()
        {
            var trace = new List<RecoveryReduction>(); var masks = new byte[] { 4, 2, 1, 1 }; // EW versus NS
            Assert.That(Stage(Single(), masks, trace, 1), Is.False);
            masks = new byte[] { 1, 4, 1, 1 }; // EMPTY also cannot face a concrete incoming port
            Assert.That(Stage(Single(), masks, trace, 1), Is.False);
            Assert.That(Stage(Single(), new byte[] { 1, 1, 1, 1 }, trace, 1), Is.True, "absent ports on both cells agree");
        }
        [Test]
        public void Lines_EnforceBothBoundsAndAllThreeDeductionForms()
        {
            var p = Single(); var trace = new List<RecoveryReduction>();
            Assert.That(Stage(p, new byte[] { 64, 4, 1, 1 }, trace, 2), Is.False); // min > target
            Assert.That(Stage(p, new byte[] { 1, 1, 1, 1 }, trace, 2), Is.False); // max < target
            var masks = new byte[] { 65, 1, 127, 127 }; trace.Clear();
            Assert.That(Stage(p, masks, trace, 2), Is.True);
            Assert.That(masks, Is.EqualTo(new byte[] { 64, 1, 1, 1 }));
            Assert.That(trace.Any(r => r.RuleCode == "LINE_ZERO" && r.LineAxis == "ROW" && r.LineIndex == 1), Is.True);
            Assert.That(trace.Any(r => r.RuleCode == "LINE_ALL_REMAINING_OCCUPIED"), Is.True);
            masks = new byte[] { 64, 127, 1, 1 }; trace.Clear();
            Assert.That(Stage(p, masks, trace, 2), Is.True);
            Assert.That(masks[1], Is.EqualTo(1)); Assert.That(trace.Any(r => r.RuleCode == "LINE_TARGET_REACHED"), Is.True);
        }
        [Test]
        public void LocalDegree_RejectsAConcreteShapeWithAnUnavailableSecondPort()
        {
            var masks = Enumerable.Repeat((byte)127, 9).ToArray(); masks[4] = 3; masks[1] = 1; masks[7] = 1;
            var trace = new List<RecoveryReduction>(); Assert.That(Stage(Single(3), masks, trace, 3), Is.True);
            Assert.That(masks[4], Is.EqualTo(1)); Assert.That(trace.Any(r => r.RuleCode == "LOCAL_DEGREE_REQUIRED" && r.Cell.X == 1 && r.Cell.Y == 1), Is.True);
        }
        [Test]
        public void Subtour_RejectsMandatoryLoopsAndPrunesTheClosingCandidate()
        {
            var masks = new byte[] { 64, 1, 1, 1, 16, 32, 1, 8, 64 }; var trace = new List<RecoveryReduction>();
            Assert.That(Stage(Single(3), masks, trace, 4), Is.False);
            masks[4] = 17; trace.Clear(); Assert.That(Stage(Single(3), masks, trace, 4), Is.True);
            Assert.That(masks[4], Is.EqualTo(1)); Assert.That(trace.Any(r => r.RuleCode == "LOOP_PREVENTION"), Is.True);
        }
        [Test]
        public void GlobalReachability_RemovesOptionalIslandsAndRejectsDisconnectedMandatoryCells()
        {
            var masks = new byte[] { 64, 1, 1, 127 }; var trace = new List<RecoveryReduction>();
            Assert.That(Stage(Single(), masks, trace, 5), Is.True); Assert.That(masks[3], Is.EqualTo(1));
            Assert.That(trace.Any(r => r.RuleCode == "CONNECTIVITY_PRESERVATION"), Is.True);
            masks[3] = 8; Assert.That(Stage(Single(), masks, trace, 5), Is.False);
            var p = Single(); var horizontal = PuzzleDefinition.Create(p.Grid, Endpoint.Create(p.Grid, Direction.W, 0).Value!, Endpoint.Create(p.Grid, Direction.E, 0).Value!, new[] { 2, 0 }, new[] { 1, 1 }, "train-track-v1").Value!;
            Assert.That(Stage(horizontal, new byte[] { 64, 4, 1, 1 }, trace, 5), Is.False);
        }
    }
}
