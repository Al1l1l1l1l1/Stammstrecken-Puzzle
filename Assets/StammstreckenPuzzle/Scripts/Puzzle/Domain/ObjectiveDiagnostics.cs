using System;
using System.Collections.Generic;

namespace STP.Puzzle.Domain
{
    /// <summary>
    /// Stabile Codes der sichtbaren objektiven Diagnosen gemäß PUZZLE_ENGINE.md
    /// Abschnitt 5. Diese Fakten folgen ausschließlich aus dem sichtbaren Stand
    /// und enthalten kein Lösungswissen.
    /// </summary>
    public static class ObjectiveDiagnosticCodes
    {
        /// <summary>Konkrete Tracks plus graue Belegungsannahmen übersteigen eine Zeilenzahl.</summary>
        public const string RowCountExceeded = "ROW_COUNT_EXCEEDED";

        /// <summary>Konkrete Tracks plus graue Belegungsannahmen übersteigen eine Spaltenzahl.</summary>
        public const string ColumnCountExceeded = "COLUMN_COUNT_EXCEEDED";

        /// <summary>Konkrete Form hat einen Außenanschluss ohne A/B an exakt dieser Kante.</summary>
        public const string TrackExitsGrid = "TRACK_EXITS_GRID";

        /// <summary>Zwei benachbarte konkrete Gleise stimmen an ihrer gemeinsamen Kante nicht überein.</summary>
        public const string TrackConnectionMismatch = "TRACK_CONNECTION_MISMATCH";

        /// <summary>Konkrete Gleise bilden bereits eine geschlossene Komponente.</summary>
        public const string PrematureConcreteLoop = "PREMATURE_CONCRETE_LOOP";

        /// <summary>Konkrete Endpointzelle kann den Außenanschluss nicht bedienen.</summary>
        public const string EndpointMismatch = "ENDPOINT_MISMATCH";
    }

    /// <summary>
    /// Eine zellbezogene objektive Diagnose. Sortierung erfolgt deterministisch
    /// nach Koordinate (zeilenweise), dann Code.
    /// </summary>
    public readonly struct CellDiagnostic : IEquatable<CellDiagnostic>, IComparable<CellDiagnostic>
    {
        /// <summary>Stabiler Diagnosecode aus <see cref="ObjectiveDiagnosticCodes"/>.</summary>
        public string Code { get; }

        /// <summary>Betroffene Rasterzelle.</summary>
        public CellCoordinate Coordinate { get; }

        /// <summary>Erstellt eine zellbezogene Diagnose.</summary>
        public CellDiagnostic(string code, CellCoordinate coordinate)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Coordinate = coordinate;
        }

        /// <inheritdoc/>
        public bool Equals(CellDiagnostic other) => Code == other.Code && Coordinate == other.Coordinate;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is CellDiagnostic other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => (Code.GetHashCode() << 8) ^ Coordinate.GetHashCode();

        /// <summary>Deterministische Ordnung: Koordinate (y, dann x), dann Code.</summary>
        public int CompareTo(CellDiagnostic other)
        {
            var byCoordinate = Coordinate.CompareTo(other.Coordinate);
            return byCoordinate != 0 ? byCoordinate : string.CompareOrdinal(Code, other.Code);
        }
    }

    /// <summary>
    /// Vollständige, deterministisch sortierte objektive Diagnoselage eines
    /// sichtbaren Stands. Zeilen-/Spaltenüberschreitungen sind als Indexlisten
    /// ausgewiesen; zellbezogene Verletzungen tragen Code und Koordinate.
    /// </summary>
    public sealed class ObjectiveDiagnostics : IEquatable<ObjectiveDiagnostics>
    {
        /// <summary>Leere Diagnoselage.</summary>
        public static readonly ObjectiveDiagnostics Empty =
            new ObjectiveDiagnostics(Array.Empty<int>(), Array.Empty<int>(), Array.Empty<CellDiagnostic>());

        /// <summary>Zeilen mit <c>ROW_COUNT_EXCEEDED</c> (aufsteigend sortiert).</summary>
        public IReadOnlyList<int> ExceededRows { get; }

        /// <summary>Spalten mit <c>COLUMN_COUNT_EXCEEDED</c> (aufsteigend sortiert).</summary>
        public IReadOnlyList<int> ExceededColumns { get; }

        /// <summary>Zellbezogene Verletzungen, sortiert nach Koordinate, dann Code.</summary>
        public IReadOnlyList<CellDiagnostic> CellDiagnostics { get; }

        /// <summary>Wahr, wenn mindestens eine objektive Verletzung vorliegt.</summary>
        public bool HasViolations => ExceededRows.Count > 0 || ExceededColumns.Count > 0 || CellDiagnostics.Count > 0;

        /// <summary>Erstellt eine Diagnoselage aus bereits sortierten Auflistungen.</summary>
        public ObjectiveDiagnostics(
            IReadOnlyList<int> exceededRows,
            IReadOnlyList<int> exceededColumns,
            IReadOnlyList<CellDiagnostic> cellDiagnostics)
        {
            ExceededRows = exceededRows ?? throw new ArgumentNullException(nameof(exceededRows));
            ExceededColumns = exceededColumns ?? throw new ArgumentNullException(nameof(exceededColumns));
            CellDiagnostics = cellDiagnostics ?? throw new ArgumentNullException(nameof(cellDiagnostics));
        }

        /// <inheritdoc/>
        public bool Equals(ObjectiveDiagnostics? other)
        {
            if (other is null)
            {
                return false;
            }
            return SequenceEqual(ExceededRows, other.ExceededRows)
                && SequenceEqual(ExceededColumns, other.ExceededColumns)
                && SequenceEqual(CellDiagnostics, other.CellDiagnostics);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as ObjectiveDiagnostics);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hash = 17;
            foreach (var row in ExceededRows)
            {
                hash = (hash * 31) + row;
            }
            foreach (var column in ExceededColumns)
            {
                hash = (hash * 31) + column;
            }
            foreach (var diagnostic in CellDiagnostics)
            {
                hash = (hash * 31) + diagnostic.GetHashCode();
            }
            return hash;
        }

        private static bool SequenceEqual<T>(IReadOnlyList<T> first, IReadOnlyList<T> second)
        {
            if (first.Count != second.Count)
            {
                return false;
            }
            for (var index = 0; index < first.Count; index++)
            {
                if (!EqualityComparer<T>.Default.Equals(first[index], second[index]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
