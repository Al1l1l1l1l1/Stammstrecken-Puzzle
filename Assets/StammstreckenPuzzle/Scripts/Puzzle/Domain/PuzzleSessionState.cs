using System;
using System.Collections.Generic;

namespace STP.Puzzle.Domain
{
    /// <summary>
    /// Unveränderlicher Session-Snapshot gemäß GAME_STATE_MODEL.md Abschnitt 5
    /// und ADR-005, begrenzt auf den Domainkern von WP-009. Commands sind die
    /// einzige Mutationsgrenze; jede Handlung erzeugt einen neuen Snapshot mit
    /// monoton steigender Revision. Persistenzfelder wie publicPuzzleHash,
    /// Modus und UTC-Diagnosezeiten sind Verträge späterer Work Packages
    /// (Content-/Persistenzadapter).
    /// </summary>
    public sealed class PuzzleSessionState
    {
        /// <summary>Stabile fachliche Puzzle-ID.</summary>
        public string PuzzleId { get; }

        /// <summary>Lokal eindeutige Versuchs-ID.</summary>
        public string AttemptId { get; }

        /// <summary>Die validierte Definition dieses Versuchs.</summary>
        public PuzzleDefinition Definition { get; }

        /// <summary>Zeilenweiser Zellstand, exakt <c>width * height</c> Einträge.</summary>
        public IReadOnlyList<CellContent> Cells { get; }

        /// <summary>Fachliche Sessionphase.</summary>
        public SessionPhase Phase { get; }

        /// <summary>Monotone Revision; Commands tragen ihre erwartete Revision.</summary>
        public long Revision { get; }

        /// <summary>Wahr nach dem ersten Command, der mindestens eine Zelle wirklich verändert hat.</summary>
        public bool TimerStarted { get; }

        /// <summary>Akkumulierte aktive Zeit in Millisekunden (monotone Quelle als Command-Input).</summary>
        public long ActiveElapsedMs { get; }

        /// <summary>Letzter bekannter monotone Zeitwert oder <c>null</c> vor Timerstart.</summary>
        public long? LastMonotonicMs { get; }

        /// <summary>Sticky: eine Leer-Markierung wurde mindestens einmal gesetzt.</summary>
        public bool UsedEmptyMarker { get; }

        /// <summary>Sticky: eine Belegungsmarkierung wurde mindestens einmal gesetzt.</summary>
        public bool UsedOccupiedMarker { get; }

        /// <summary>Zahl tatsächlich dargestellter Hinweise in diesem Versuch; bleibt in WP-009 bei 0 (BLOCKER-PROD-001).</summary>
        public int HintCount { get; }

        /// <summary>Zahl echter Inhaltsänderungen bereits belegter Zellen (Korrekturen).</summary>
        public int CorrectionCount { get; }

        /// <summary>Undo-Diffs, älteste zuerst verworfen, maximal <see cref="CellDiff.MaximumUndoDepth"/> Einträge.</summary>
        public IReadOnlyList<CellDiff> UndoStack { get; }

        /// <summary>Deterministische Abschluss-ID oder <c>null</c>, solange nicht gelöst.</summary>
        public string? SolvedEventId { get; }

        private PuzzleSessionState(
            string puzzleId,
            string attemptId,
            PuzzleDefinition definition,
            IReadOnlyList<CellContent> cells,
            SessionPhase phase,
            long revision,
            bool timerStarted,
            long activeElapsedMs,
            long? lastMonotonicMs,
            bool usedEmptyMarker,
            bool usedOccupiedMarker,
            int hintCount,
            int correctionCount,
            IReadOnlyList<CellDiff> undoStack,
            string? solvedEventId)
        {
            PuzzleId = puzzleId;
            AttemptId = attemptId;
            Definition = definition;
            Cells = cells;
            Phase = phase;
            Revision = revision;
            TimerStarted = timerStarted;
            ActiveElapsedMs = activeElapsedMs;
            LastMonotonicMs = lastMonotonicMs;
            UsedEmptyMarker = usedEmptyMarker;
            UsedOccupiedMarker = usedOccupiedMarker;
            HintCount = hintCount;
            CorrectionCount = correctionCount;
            UndoStack = undoStack;
            SolvedEventId = solvedEventId;
        }

        /// <summary>
        /// Startet einen neuen Versuch in Phase <see cref="SessionPhase.Ready"/>
        /// mit leerem Raster und leerem Undo-Verlauf.
        /// </summary>
        public static PuzzleSessionState StartNew(
            PuzzleDefinition definition,
            string puzzleId,
            string attemptId)
        {
            if (definition is null)
            {
                throw new ArgumentNullException(nameof(definition));
            }
            if (string.IsNullOrEmpty(puzzleId))
            {
                throw new ArgumentException("puzzleId ist erforderlich.", nameof(puzzleId));
            }
            if (string.IsNullOrEmpty(attemptId))
            {
                throw new ArgumentException("attemptId ist erforderlich.", nameof(attemptId));
            }

            var cells = new CellContent[definition.Grid.Width * definition.Grid.Height];
            return new PuzzleSessionState(
                puzzleId,
                attemptId,
                definition,
                cells,
                SessionPhase.Ready,
                revision: 0,
                timerStarted: false,
                activeElapsedMs: 0,
                lastMonotonicMs: null,
                usedEmptyMarker: false,
                usedOccupiedMarker: false,
                hintCount: 0,
                correctionCount: 0,
                undoStack: Array.Empty<CellDiff>(),
                solvedEventId: null);
        }

        /// <summary>Erzeugt einen Nachfolgesnapshot; nicht genannte Felder bleiben erhalten.</summary>
        public PuzzleSessionState With(
            IReadOnlyList<CellContent>? cells = null,
            SessionPhase? phase = null,
            long? revision = null,
            bool? timerStarted = null,
            long? activeElapsedMs = null,
            long? lastMonotonicMs = null,
            bool? usedEmptyMarker = null,
            bool? usedOccupiedMarker = null,
            int? hintCount = null,
            int? correctionCount = null,
            IReadOnlyList<CellDiff>? undoStack = null,
            string? solvedEventId = null)
        {
            return new PuzzleSessionState(
                PuzzleId,
                AttemptId,
                Definition,
                cells ?? Cells,
                phase ?? Phase,
                revision ?? Revision,
                timerStarted ?? TimerStarted,
                activeElapsedMs ?? ActiveElapsedMs,
                lastMonotonicMs ?? LastMonotonicMs,
                usedEmptyMarker ?? UsedEmptyMarker,
                usedOccupiedMarker ?? UsedOccupiedMarker,
                hintCount ?? HintCount,
                correctionCount ?? CorrectionCount,
                undoStack ?? UndoStack,
                solvedEventId ?? SolvedEventId);
        }
    }
}
