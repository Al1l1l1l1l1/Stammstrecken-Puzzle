using NUnit.Framework;
using STP.Puzzle.Domain;

namespace STP.Tests.Domain.EditMode
{
    /// <summary>
    /// Regressionstests zur Command-Validierung: Malformed Commands (fehlende
    /// Hülle, fehlende Änderungsliste, Zellinhalte außerhalb des Enum-Bereichs)
    /// werden stabil und deterministisch mit Diagnosecode abgelehnt statt mit
    /// einer ungefangenen Ausnahme abzubrechen.
    /// </summary>
    public sealed class CommandValidationTests
    {
        private static PuzzleSessionState NewSession()
        {
            return PuzzleSessionState.StartNew(
                new PuzzleDefinition(
                    new GridSize(2, 2),
                    new Endpoint(Direction.N, 0),
                    new Endpoint(Direction.W, 0),
                    new[] { 1, 0 },
                    new[] { 1, 0 },
                    PuzzleDefinition.RulesetVersionV1),
                "p",
                "a");
        }

        private static void AssertRejected(PuzzleSessionState state, CommandResult result, string code)
        {
            Assert.IsFalse(result.Accepted);
            Assert.AreEqual(new[] { code }, result.Diagnostics);
            Assert.AreSame(state, result.State);
            Assert.IsEmpty(result.Events);
        }

        /// <summary>Ein per default erzeugter Batch (CommandId und Changes null) wird abgelehnt, nicht mit NullReferenceException abgebrochen.</summary>
        [Test]
        public void DefaultBatchCommand_IsRejectedDeterministically()
        {
            var state = NewSession();
            var result = PuzzleSession.Handle(state, default(ApplyCellContentBatchCommand), 0);
            AssertRejected(state, result, PuzzleSession.InvalidCommand);

            var repeated = PuzzleSession.Handle(state, default(ApplyCellContentBatchCommand), 0);
            Assert.AreEqual(result.Diagnostics, repeated.Diagnostics);
        }

        /// <summary>Ein per default erzeugter Einzel-Command (CommandId null) wird abgelehnt.</summary>
        [Test]
        public void DefaultApplyCommand_IsRejected()
        {
            var state = NewSession();
            AssertRejected(state, PuzzleSession.Handle(state, default(ApplyCellContentCommand), 0), PuzzleSession.InvalidCommand);
        }

        /// <summary>Ein per default erzeugter Undo-Command (CommandId null) wird abgelehnt.</summary>
        [Test]
        public void DefaultUndoCommand_IsRejected()
        {
            var state = NewSession();
            AssertRejected(state, PuzzleSession.Handle(state, default(UndoLastActionCommand), 0), PuzzleSession.InvalidCommand);
        }

        /// <summary>Eine leere Batch-Änderungsliste ist ein malformed Command.</summary>
        [Test]
        public void EmptyBatchChanges_IsRejected()
        {
            var state = NewSession();
            var command = new ApplyCellContentBatchCommand(
                "batch-empty", state.Revision, System.Array.Empty<CellChange>());
            AssertRejected(state, PuzzleSession.Handle(state, command, 0), PuzzleSession.InvalidCommand);
        }

        /// <summary>Ein Zellinhalt außerhalb des Enum-Bereichs wird im Einzel-Command abgelehnt.</summary>
        [Test]
        public void OutOfRangeCellContent_Single_IsRejected()
        {
            var state = NewSession();
            var command = new ApplyCellContentCommand(
                "cmd-bad", state.Revision, new CellCoordinate(0, 0), (CellContent)255);
            AssertRejected(state, PuzzleSession.Handle(state, command, 0), PuzzleSession.InvalidCommand);
        }

        /// <summary>Ein Zellinhalt außerhalb des Enum-Bereichs wird im Batch abgelehnt.</summary>
        [Test]
        public void OutOfRangeCellContent_Batch_IsRejected()
        {
            var state = NewSession();
            var command = new ApplyCellContentBatchCommand(
                "batch-bad",
                state.Revision,
                new[] { new CellChange(new CellCoordinate(0, 0), (CellContent)255) });
            AssertRejected(state, PuzzleSession.Handle(state, command, 0), PuzzleSession.InvalidCommand);
        }
    }
}
