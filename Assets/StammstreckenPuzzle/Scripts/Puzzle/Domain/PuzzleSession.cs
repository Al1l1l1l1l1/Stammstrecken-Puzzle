using System;
using System.Collections.Generic;

namespace STP.Puzzle.Domain
{
    /// <summary>
    /// Command-Verarbeitung des Domainkerns gemäß PUZZLE_ENGINE.md Abschnitt 4
    /// und ADR-005. Jede Handlung erzeugt entweder einen neuen immutable
    /// Snapshot plus geordnete Ereignisse oder eine unveränderte Ablehnung mit
    /// Diagnosecode. Befehle werden nicht teilweise angewendet; Zeit ist ein
    /// monotoner Command-Input, keine Wanduhr.
    /// </summary>
    public static class PuzzleSession
    {
        /// <summary>Diagnosecode für eine abweichende erwartete Revision.</summary>
        public const string StaleCommand = "STALE_COMMAND";

        /// <summary>Diagnosecode für Commands außerhalb der Phasen Ready/Active.</summary>
        public const string SessionClosed = "SESSION_CLOSED";

        /// <summary>Diagnosecode für eine Koordinate außerhalb des Rasters.</summary>
        public const string OutOfBounds = "OUT_OF_BOUNDS";

        /// <summary>Diagnosecode für eine doppelte Koordinate innerhalb eines Batches.</summary>
        public const string DuplicateCoordinate = "DUPLICATE_COORDINATE";

        /// <summary>Diagnosecode für Undo ohne vorhandenen Verlauf.</summary>
        public const string NothingToUndo = "NOTHING_TO_UNDO";

        /// <summary>
        /// Diagnosecode für eine malformed Command-Hülle: fehlende Command-ID,
        /// fehlende oder leere Batch-Änderungsliste oder ein Zellinhalt
        /// außerhalb des geschlossenen Enum-Bereichs. Solche Eingaben werden
        /// stabil und deterministisch abgelehnt, niemals mit einer ungefangenen
        /// Ausnahme abgebrochen.
        /// </summary>
        public const string InvalidCommand = "INVALID_COMMAND";

        /// <summary>Wendet einen Einzel-Command an.</summary>
        public static CommandResult Handle(
            PuzzleSessionState state,
            ApplyCellContentCommand command,
            long monotonicMs)
        {
            if (state is null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            var envelope = ValidateEnvelope(state, command.ExpectedRevision);
            if (envelope is not null)
            {
                return CommandResult.Rejected(state, envelope);
            }
            if (string.IsNullOrEmpty(command.CommandId) || !Enum.IsDefined(typeof(CellContent), command.Content))
            {
                return CommandResult.Rejected(state, InvalidCommand);
            }
            if (!state.Definition.Grid.Contains(command.Coordinate))
            {
                return CommandResult.Rejected(state, OutOfBounds);
            }

            return ApplyChanges(
                state,
                new[] { new CellChange(command.Coordinate, command.Content) },
                isBatch: false,
                monotonicMs);
        }

        /// <summary>Wendet einen atomaren Batch-Command an; bei einer ungültigen Koordinate bleibt der gesamte Zustand unverändert.</summary>
        public static CommandResult Handle(
            PuzzleSessionState state,
            ApplyCellContentBatchCommand command,
            long monotonicMs)
        {
            if (state is null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            var envelope = ValidateEnvelope(state, command.ExpectedRevision);
            if (envelope is not null)
            {
                return CommandResult.Rejected(state, envelope);
            }
            if (string.IsNullOrEmpty(command.CommandId) || command.Changes is null || command.Changes.Count == 0)
            {
                return CommandResult.Rejected(state, InvalidCommand);
            }

            var seen = new HashSet<CellCoordinate>();
            foreach (var change in command.Changes)
            {
                if (!Enum.IsDefined(typeof(CellContent), change.Content))
                {
                    return CommandResult.Rejected(state, InvalidCommand);
                }
                if (!state.Definition.Grid.Contains(change.Coordinate))
                {
                    return CommandResult.Rejected(state, OutOfBounds);
                }
                if (!seen.Add(change.Coordinate))
                {
                    return CommandResult.Rejected(state, DuplicateCoordinate);
                }
            }

            return ApplyChanges(state, command.Changes, isBatch: true, monotonicMs);
        }

        /// <summary>
        /// Nimmt die letzte atomare Nutzerhandlung zurück. Zellen werden
        /// wiederhergestellt; Timer, sticky Flags, Korrekturzähler und ein
        /// bereits emittiertes PuzzleSolved bleiben erhalten.
        /// </summary>
        public static CommandResult Handle(
            PuzzleSessionState state,
            UndoLastActionCommand command,
            long monotonicMs)
        {
            if (state is null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            var envelope = ValidateEnvelope(state, command.ExpectedRevision);
            if (envelope is not null)
            {
                return CommandResult.Rejected(state, envelope);
            }
            if (string.IsNullOrEmpty(command.CommandId))
            {
                return CommandResult.Rejected(state, InvalidCommand);
            }
            if (state.UndoStack.Count == 0)
            {
                return CommandResult.Rejected(state, NothingToUndo);
            }

            var grid = state.Definition.Grid;
            var diff = state.UndoStack[state.UndoStack.Count - 1];
            var cells = CopyCells(state);
            foreach (var entry in diff.Entries)
            {
                cells[grid.IndexOf(entry.Coordinate)] = entry.Before;
            }

            var undoStack = new List<CellDiff>(state.UndoStack);
            undoStack.RemoveAt(undoStack.Count - 1);

            var (timerStarted, activeElapsedMs, lastMonotonicMs) = AccumulateTimer(state, monotonicMs);
            var next = state.With(
                cells: cells,
                revision: state.Revision + 1,
                timerStarted: timerStarted,
                activeElapsedMs: activeElapsedMs,
                lastMonotonicMs: lastMonotonicMs,
                undoStack: undoStack);

            var events = new List<DomainEvent>
            {
                new DomainEvent(
                    diff.Entries.Count == 1 ? DomainEventType.CellContentChanged : DomainEventType.CellBatchChanged,
                    diff.Entries[0].Coordinate),
            };
            if (ViolationsChanged(state, next))
            {
                events.Add(new DomainEvent(DomainEventType.ObjectiveViolationChanged, null));
            }

            return CommandResult.AcceptedWith(next, events);
        }

        private static string? ValidateEnvelope(PuzzleSessionState state, long expectedRevision)
        {
            if (expectedRevision != state.Revision)
            {
                return StaleCommand;
            }
            if (state.Phase != SessionPhase.Ready && state.Phase != SessionPhase.Active)
            {
                return SessionClosed;
            }
            return null;
        }

        private static CommandResult ApplyChanges(
            PuzzleSessionState state,
            IReadOnlyList<CellChange> changes,
            bool isBatch,
            long monotonicMs)
        {
            var grid = state.Definition.Grid;
            var diffEntries = new List<CellDiffEntry>();
            foreach (var change in changes)
            {
                var before = state.Cells[grid.IndexOf(change.Coordinate)];
                if (before != change.Content)
                {
                    diffEntries.Add(new CellDiffEntry(change.Coordinate, before, change.Content));
                }
            }

            // No-op: kein Undo-Eintrag, keine Ereignisse, keine Revisionsänderung.
            if (diffEntries.Count == 0)
            {
                return CommandResult.AcceptedWith(state, Array.Empty<DomainEvent>());
            }

            var cells = CopyCells(state);
            var corrections = state.CorrectionCount;
            var usedEmptyMarker = state.UsedEmptyMarker;
            var usedOccupiedMarker = state.UsedOccupiedMarker;
            foreach (var entry in diffEntries)
            {
                cells[grid.IndexOf(entry.Coordinate)] = entry.After;
                if (entry.Before != CellContent.Unset)
                {
                    corrections++;
                }
                usedEmptyMarker |= entry.After == CellContent.MarkEmpty;
                usedOccupiedMarker |= entry.After == CellContent.MarkOccupied;
            }

            var diff = new CellDiff(diffEntries);
            var undoStack = new List<CellDiff>(state.UndoStack) { diff };
            if (undoStack.Count > CellDiff.MaximumUndoDepth)
            {
                undoStack.RemoveAt(0);
            }

            var (timerStarted, activeElapsedMs, lastMonotonicMs) = AccumulateTimer(state, monotonicMs);

            var phase = state.Phase == SessionPhase.Ready ? SessionPhase.Active : state.Phase;
            var solvedEventId = state.SolvedEventId;
            var solvedNow = false;
            if (CompletionEvaluator.IsSolved(state.Definition, cells))
            {
                phase = SessionPhase.SolvedPendingCommit;
                if (solvedEventId is null)
                {
                    solvedEventId = $"{state.PuzzleId}:{state.AttemptId}:solved";
                    solvedNow = true;
                }
            }

            var next = state.With(
                cells: cells,
                phase: phase,
                revision: state.Revision + 1,
                timerStarted: timerStarted,
                activeElapsedMs: activeElapsedMs,
                lastMonotonicMs: lastMonotonicMs,
                usedEmptyMarker: usedEmptyMarker,
                usedOccupiedMarker: usedOccupiedMarker,
                correctionCount: corrections,
                undoStack: undoStack,
                solvedEventId: solvedEventId);

            var events = new List<DomainEvent>
            {
                new DomainEvent(
                    isBatch ? DomainEventType.CellBatchChanged : DomainEventType.CellContentChanged,
                    diff.Entries[0].Coordinate),
            };
            if (ViolationsChanged(state, next))
            {
                events.Add(new DomainEvent(DomainEventType.ObjectiveViolationChanged, null));
            }
            if (solvedNow)
            {
                events.Add(new DomainEvent(DomainEventType.PuzzleSolved, null));
            }

            return CommandResult.AcceptedWith(next, events);
        }

        private static (bool TimerStarted, long ActiveElapsedMs, long LastMonotonicMs) AccumulateTimer(
            PuzzleSessionState state,
            long monotonicMs)
        {
            if (!state.TimerStarted)
            {
                return (true, state.ActiveElapsedMs, monotonicMs);
            }
            var previous = state.LastMonotonicMs ?? monotonicMs;
            var delta = monotonicMs - previous;
            if (delta < 0)
            {
                delta = 0;
            }
            return (true, state.ActiveElapsedMs + delta, monotonicMs);
        }

        private static bool ViolationsChanged(PuzzleSessionState before, PuzzleSessionState after)
        {
            var oldDiagnostics = BoardEvaluator.Evaluate(before.Definition, before.Cells);
            var newDiagnostics = BoardEvaluator.Evaluate(after.Definition, after.Cells);
            return !oldDiagnostics.Equals(newDiagnostics);
        }

        private static CellContent[] CopyCells(PuzzleSessionState state)
        {
            var cells = new CellContent[state.Cells.Count];
            for (var index = 0; index < cells.Length; index++)
            {
                cells[index] = state.Cells[index];
            }
            return cells;
        }
    }
}
