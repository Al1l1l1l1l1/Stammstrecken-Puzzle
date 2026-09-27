using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using STP.Puzzle.Domain;

namespace STP.Tests.Domain.EditMode
{
    /// <summary>
    /// Command-Verarbeitung gemäß ADR-005, GAME_STATE_MODEL.md Abschnitte 5–9
    /// und PUZZLE_ENGINE.md Abschnitte 4 und 8: Atomarität, No-op, Sticky-Flags,
    /// Timer, Revisionen, Undo und Abschlussübergang.
    /// </summary>
    public sealed class PuzzleSessionTests
    {
        private static PuzzleDefinition SingleCellDefinition()
        {
            return new PuzzleDefinition(
                new GridSize(2, 2),
                new Endpoint(Direction.N, 0),
                new Endpoint(Direction.W, 0),
                new[] { 1, 0 },
                new[] { 1, 0 },
                PuzzleDefinition.RulesetVersionV1);
        }

        private static PuzzleSessionState NewSession()
        {
            return PuzzleSessionState.StartNew(SingleCellDefinition(), "S99-01-01-01", "attempt-1");
        }

        private static ApplyCellContentCommand Apply(
            PuzzleSessionState state, int x, int y, CellContent content, string id = "cmd-1")
        {
            return new ApplyCellContentCommand(id, state.Revision, new CellCoordinate(x, y), content);
        }

        /// <summary>Eine echte Änderung erzeugt Snapshot, Ereignis, Revision und Timerstart.</summary>
        [Test]
        public void Apply_RealChange_AdvancesStateAndStartsTimer()
        {
            var state = NewSession();
            var result = PuzzleSession.Handle(state, Apply(state, 0, 0, CellContent.TrackNS), monotonicMs: 1_000);

            Assert.IsTrue(result.Accepted);
            Assert.AreEqual(1, result.State.Revision);
            Assert.AreEqual(SessionPhase.Active, result.State.Phase);
            Assert.AreEqual(CellContent.TrackNS, result.State.Cells[0]);
            Assert.IsTrue(result.State.TimerStarted);
            Assert.AreEqual(0, result.State.ActiveElapsedMs);
            Assert.AreEqual(1_000, result.State.LastMonotonicMs);
            Assert.AreEqual(1, result.State.UndoStack.Count);
            Assert.IsTrue(result.Events.Any(e => e.Type == DomainEventType.CellContentChanged));
        }

        /// <summary>Ein No-op erzeugt weder Ereignis, Revision, Undo-Eintrag noch Timerstart.</summary>
        [Test]
        public void Apply_NoOp_LeavesEverythingUntouched()
        {
            var state = NewSession();
            var result = PuzzleSession.Handle(state, Apply(state, 1, 1, CellContent.Unset), monotonicMs: 5_000);

            Assert.IsTrue(result.Accepted);
            Assert.AreSame(state, result.State);
            Assert.IsEmpty(result.Events);
            Assert.IsFalse(result.State.TimerStarted);
            Assert.AreEqual(0, result.State.UndoStack.Count);
        }

        /// <summary>Der Timer akkumuliert monotone Deltas erst ab dem ersten echten Command.</summary>
        [Test]
        public void Timer_AccumulatesMonotonicDeltas()
        {
            var state = NewSession();
            var first = PuzzleSession.Handle(state, Apply(state, 0, 0, CellContent.TrackNS), 1_000).State;
            var second = PuzzleSession.Handle(first, Apply(first, 0, 0, CellContent.TrackWN), 3_500).State;

            Assert.AreEqual(2_500, second.ActiveElapsedMs);
            Assert.AreEqual(1, second.CorrectionCount);
        }

        /// <summary>Marker setzen sticky Flags; Undo nimmt sie nicht zurück.</summary>
        [Test]
        public void Markers_SetStickyFlags_AndUndoKeepsThem()
        {
            var state = NewSession();
            var marked = PuzzleSession.Handle(
                state, Apply(state, 1, 0, CellContent.MarkEmpty), 100).State;
            Assert.IsTrue(marked.UsedEmptyMarker);

            var undone = PuzzleSession.Handle(
                marked, new UndoLastActionCommand("undo-1", marked.Revision), 200);
            Assert.IsTrue(undone.Accepted);
            Assert.AreEqual(CellContent.Unset, undone.State.Cells[undone.State.Definition.Grid.IndexOf(new CellCoordinate(1, 0))]);
            Assert.IsTrue(undone.State.UsedEmptyMarker);
            Assert.AreEqual(2, undone.State.Revision);
            Assert.AreEqual(100, undone.State.ActiveElapsedMs);
        }

        /// <summary>Ein Batch mit einer ungültigen Koordinate verändert den gesamten Zustand nicht.</summary>
        [Test]
        public void Batch_WithInvalidCoordinate_IsFullyRejected()
        {
            var state = NewSession();
            var command = new ApplyCellContentBatchCommand(
                "batch-1",
                state.Revision,
                new[]
                {
                    new CellChange(new CellCoordinate(0, 0), CellContent.TrackWN),
                    new CellChange(new CellCoordinate(9, 9), CellContent.TrackNS),
                });

            var result = PuzzleSession.Handle(state, command, 100);

            Assert.IsFalse(result.Accepted);
            Assert.AreEqual(new[] { PuzzleSession.OutOfBounds }, result.Diagnostics);
            Assert.AreSame(state, result.State);
        }

        /// <summary>Doppelte Koordinaten innerhalb eines Batches werden abgelehnt.</summary>
        [Test]
        public void Batch_WithDuplicateCoordinate_IsRejected()
        {
            var state = NewSession();
            var command = new ApplyCellContentBatchCommand(
                "batch-2",
                state.Revision,
                new[]
                {
                    new CellChange(new CellCoordinate(0, 0), CellContent.TrackWN),
                    new CellChange(new CellCoordinate(0, 0), CellContent.TrackNS),
                });

            var result = PuzzleSession.Handle(state, command, 100);

            Assert.IsFalse(result.Accepted);
            Assert.AreEqual(new[] { PuzzleSession.DuplicateCoordinate }, result.Diagnostics);
        }

        /// <summary>Ein valider Batch ist atomar und erzeugt genau einen Undo-Diff.</summary>
        [Test]
        public void Batch_Valid_IsAtomicSingleUndoStep()
        {
            var state = NewSession();
            var command = new ApplyCellContentBatchCommand(
                "batch-3",
                state.Revision,
                new[]
                {
                    new CellChange(new CellCoordinate(1, 0), CellContent.MarkEmpty),
                    new CellChange(new CellCoordinate(1, 1), CellContent.MarkOccupied),
                });

            var applied = PuzzleSession.Handle(state, command, 100).State;
            Assert.AreEqual(1, applied.UndoStack.Count);
            Assert.AreEqual(2, applied.UndoStack[0].Entries.Count);
            Assert.IsTrue(applied.UsedEmptyMarker && applied.UsedOccupiedMarker);

            var undone = PuzzleSession.Handle(
                applied, new UndoLastActionCommand("undo-2", applied.Revision), 200).State;
            Assert.AreEqual(CellContent.Unset, undone.Cells[undone.Definition.Grid.IndexOf(new CellCoordinate(1, 0))]);
            Assert.AreEqual(CellContent.Unset, undone.Cells[undone.Definition.Grid.IndexOf(new CellCoordinate(1, 1))]);
        }

        /// <summary>Eine abweichende erwartete Revision wird als STALE_COMMAND abgelehnt.</summary>
        [Test]
        public void StaleRevision_IsRejected()
        {
            var state = NewSession();
            var stale = new ApplyCellContentCommand(
                "stale-1", state.Revision + 1, new CellCoordinate(0, 0), CellContent.TrackNS);

            var result = PuzzleSession.Handle(state, stale, 100);

            Assert.IsFalse(result.Accepted);
            Assert.AreEqual(new[] { PuzzleSession.StaleCommand }, result.Diagnostics);
            Assert.AreSame(state, result.State);
        }

        /// <summary>Undo ohne Verlauf wird abgelehnt.</summary>
        [Test]
        public void Undo_WithoutHistory_IsRejected()
        {
            var state = NewSession();
            var result = PuzzleSession.Handle(
                state, new UndoLastActionCommand("undo-0", state.Revision), 100);

            Assert.IsFalse(result.Accepted);
            Assert.AreEqual(new[] { PuzzleSession.NothingToUndo }, result.Diagnostics);
        }

        /// <summary>Der Undo-Stack verwirft nach 256 Einträgen den ältesten Diff.</summary>
        [Test]
        public void UndoStack_DiscardsOldestBeyond256()
        {
            var state = NewSession();
            var current = state;
            for (var round = 0; round < 300; round++)
            {
                var content = round % 2 == 0 ? CellContent.MarkEmpty : CellContent.Unset;
                current = PuzzleSession.Handle(
                    current,
                    Apply(current, 1, 0, content, $"fill-{round}"),
                    1_000 + round).State;
            }

            Assert.AreEqual(CellDiff.MaximumUndoDepth, current.UndoStack.Count);
            Assert.AreEqual(300, current.Revision);
        }

        /// <summary>Abschluss: PuzzleSolved genau einmal, Phase SolvedPendingCommit, danach SESSION_CLOSED.</summary>
        [Test]
        public void Solve_TransitionsPhaseAndClosesSession()
        {
            var state = NewSession();
            var result = PuzzleSession.Handle(
                state, Apply(state, 0, 0, CellContent.TrackWN), 1_000);

            Assert.IsTrue(result.Accepted);
            Assert.AreEqual(SessionPhase.SolvedPendingCommit, result.State.Phase);
            Assert.AreEqual("S99-01-01-01:attempt-1:solved", result.State.SolvedEventId);
            Assert.Contains(DomainEventType.PuzzleSolved, result.Events.Select(e => e.Type).ToList());

            var after = PuzzleSession.Handle(
                result.State, Apply(result.State, 1, 1, CellContent.MarkEmpty), 2_000);
            Assert.IsFalse(after.Accepted);
            Assert.AreEqual(new[] { PuzzleSession.SessionClosed }, after.Diagnostics);
        }

        /// <summary>ObjectiveViolationChanged wird nur bei tatsächlicher Diagnoseänderung emittiert.</summary>
        [Test]
        public void ObjectiveViolationChanged_EmitsOnlyOnChange()
        {
            var state = NewSession();
            // Schritt 1: reine Markierung ohne Verletzung → kein Ereignis.
            var first = PuzzleSession.Handle(state, Apply(state, 1, 1, CellContent.MarkEmpty), 100);
            Assert.IsFalse(first.Events.Any(e => e.Type == DomainEventType.ObjectiveViolationChanged));

            // Schritt 2: Gleis in einer Null-Spalte → Diagnoseänderung → Ereignis.
            var second = PuzzleSession.Handle(
                first.State, Apply(first.State, 1, 0, CellContent.TrackEW), 200);
            Assert.IsTrue(second.Events.Any(e => e.Type == DomainEventType.ObjectiveViolationChanged));
        }
    }
}
