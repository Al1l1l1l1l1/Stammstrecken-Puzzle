using System;
using System.Collections.Generic;

namespace STP.Puzzle.Domain
{
    /// <summary>
    /// Command zum Setzen eines Zellinhalts auf genau eine Koordinate gemäß
    /// GAME_STATE_MODEL.md Abschnitt 7.
    /// </summary>
    public readonly struct ApplyCellContentCommand
    {
        /// <summary>Eindeutige Command-ID (Deduplikation von SDK-/UI-Callbacks).</summary>
        public string CommandId { get; }

        /// <summary>Erwartete Sessionrevision; Abweichung wird als <c>STALE_COMMAND</c> abgelehnt.</summary>
        public long ExpectedRevision { get; }

        /// <summary>Zielkoordinate.</summary>
        public CellCoordinate Coordinate { get; }

        /// <summary>Zu setzender Inhalt.</summary>
        public CellContent Content { get; }

        /// <summary>Erstellt den Einzel-Command.</summary>
        public ApplyCellContentCommand(
            string commandId,
            long expectedRevision,
            CellCoordinate coordinate,
            CellContent content)
        {
            CommandId = commandId ?? throw new ArgumentNullException(nameof(commandId));
            ExpectedRevision = expectedRevision;
            Coordinate = coordinate;
            Content = content;
        }
    }

    /// <summary>
    /// Atomarer Batch-Command für die bestätigte Mehrfachauswahl: alle
    /// Änderungen oder keine, ein einziger Undo-Diff.
    /// </summary>
    public readonly struct ApplyCellContentBatchCommand
    {
        /// <summary>Eindeutige Command-ID.</summary>
        public string CommandId { get; }

        /// <summary>Erwartete Sessionrevision.</summary>
        public long ExpectedRevision { get; }

        /// <summary>1..N eindeutige Zelländerungen.</summary>
        public IReadOnlyList<CellChange> Changes { get; }

        /// <summary>Erstellt den Batch-Command.</summary>
        public ApplyCellContentBatchCommand(
            string commandId,
            long expectedRevision,
            IReadOnlyList<CellChange> changes)
        {
            CommandId = commandId ?? throw new ArgumentNullException(nameof(commandId));
            ExpectedRevision = expectedRevision;
            Changes = changes ?? throw new ArgumentNullException(nameof(changes));
        }
    }

    /// <summary>
    /// Command zum Rücknehmen der letzten atomaren Nutzerhandlung. Timer und
    /// sticky Meisterschaftsflags bleiben erhalten.
    /// </summary>
    public readonly struct UndoLastActionCommand
    {
        /// <summary>Eindeutige Command-ID.</summary>
        public string CommandId { get; }

        /// <summary>Erwartete Sessionrevision.</summary>
        public long ExpectedRevision { get; }

        /// <summary>Erstellt den Undo-Command.</summary>
        public UndoLastActionCommand(string commandId, long expectedRevision)
        {
            CommandId = commandId ?? throw new ArgumentNullException(nameof(commandId));
            ExpectedRevision = expectedRevision;
        }
    }
}
