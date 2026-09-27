using System;

namespace STP.Puzzle.Domain
{
    /// <summary>
    /// Fachliche Sessionphase gemäß GAME_STATE_MODEL.md Abschnitt 6, begrenzt
    /// auf den Domainkern von WP-009. Save-Commit, Zugfahrt, Ergebnis und
    /// Lifecycle-Suspension sind Application-/Persistenzbelange späterer
    /// Work Packages.
    /// </summary>
    public enum SessionPhase : byte
    {
        /// <summary>Session geladen, noch kein zustandsändernder Command.</summary>
        Ready = 0,

        /// <summary>Mindestens ein zustandsändernder Command wurde angewendet.</summary>
        Active = 1,

        /// <summary>PuzzleSolved wurde emittiert; der Save-Commit ist Application-Sache.</summary>
        SolvedPendingCommit = 2,
    }

    /// <summary>
    /// Typ eines lokalen In-Process-Domainereignisses gemäß GAME_STATE_MODEL.md
    /// Abschnitt 8. Ereignisse werden nicht als dauerhaftes Event-Sourcing-Log
    /// verwendet.
    /// </summary>
    public enum DomainEventType : byte
    {
        /// <summary>Echte Einzeländerung einer Zelle.</summary>
        CellContentChanged = 0,

        /// <summary>Atomare Mehrfachänderung (Batch oder Undo über mehrere Zellen).</summary>
        CellBatchChanged = 1,

        /// <summary>Der sichtbare objektive Diagnosestatus hat sich geändert.</summary>
        ObjectiveViolationChanged = 2,

        /// <summary>Alle Gültigkeitsregeln sind erstmals in diesem Versuch erfüllt.</summary>
        PuzzleSolved = 3,
    }

    /// <summary>
    /// Ein lokales Domainereignis. Zellbezogene Ereignisse tragen die erste
    /// betroffene Koordinate des zugrunde liegenden Diffs.
    /// </summary>
    public readonly struct DomainEvent : IEquatable<DomainEvent>
    {
        /// <summary>Art des Ereignisses.</summary>
        public DomainEventType Type { get; }

        /// <summary>Erste betroffene Koordinate oder <c>null</c> bei nicht zellbezogenen Ereignissen.</summary>
        public CellCoordinate? Coordinate { get; }

        /// <summary>Erstellt ein Domainereignis.</summary>
        public DomainEvent(DomainEventType type, CellCoordinate? coordinate)
        {
            Type = type;
            Coordinate = coordinate;
        }

        /// <inheritdoc/>
        public bool Equals(DomainEvent other) => Type == other.Type && Coordinate == other.Coordinate;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is DomainEvent other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => ((int)Type << 8) ^ Coordinate.GetHashCode();
    }
}
