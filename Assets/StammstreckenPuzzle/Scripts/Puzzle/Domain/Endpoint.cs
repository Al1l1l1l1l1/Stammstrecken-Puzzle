using System;

namespace STP.Puzzle.Domain
{
    /// <summary>
    /// Äußerer Anschlusspunkt (A oder B) gemäß LEVEL_DATA_FORMAT.md Abschnitt 2.
    /// Endpoints liegen außerhalb des Rasters und zählen nicht in Randzahlen.
    /// Bei Seite N/S ist der Index eine Spalte, bei Seite E/W eine Zeile. Ein
    /// Endpoint liefert genau eine angrenzende Innenzelle und eine
    /// Außenrichtung.
    /// </summary>
    public readonly struct Endpoint : IEquatable<Endpoint>
    {
        /// <summary>Rasterkante, an der der Anschluss liegt.</summary>
        public Direction Side { get; }

        /// <summary>Spaltenindex (N/S) beziehungsweise Zeilenindex (E/W) entlang der Kante.</summary>
        public int Index { get; }

        /// <summary>Erstellt einen Endpoint aus Kante und Index (Achsprüfung erfolgt durch <see cref="PuzzleDefinition"/>).</summary>
        public Endpoint(Direction side, int index)
        {
            Side = side;
            Index = index;
        }

        /// <summary>Prüft, ob der Index zur Achse der Kante des gegebenen Rasters passt.</summary>
        public bool IsValidFor(GridSize grid)
        {
            if (Index < 0)
            {
                return false;
            }
            return Side switch
            {
                Direction.N or Direction.S => Index < grid.Width,
                Direction.E or Direction.W => Index < grid.Height,
                _ => false,
            };
        }

        /// <summary>Liefert die einzige angrenzende Innenzelle dieses Endpoints.</summary>
        public CellCoordinate AdjacentCell(GridSize grid)
        {
            return Side switch
            {
                Direction.N => new CellCoordinate(Index, 0),
                Direction.S => new CellCoordinate(Index, grid.Height - 1),
                Direction.W => new CellCoordinate(0, Index),
                Direction.E => new CellCoordinate(grid.Width - 1, Index),
                _ => throw new ArgumentOutOfRangeException(nameof(Side)),
            };
        }

        /// <inheritdoc/>
        public bool Equals(Endpoint other) => Side == other.Side && Index == other.Index;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is Endpoint other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => ((int)Side << 16) ^ Index;

        /// <summary>Gleichheit zweier Endpoints (gleiche Kante und gleicher Index).</summary>
        public static bool operator ==(Endpoint left, Endpoint right) => left.Equals(right);

        /// <summary>Ungleichheit zweier Endpoints.</summary>
        public static bool operator !=(Endpoint left, Endpoint right) => !left.Equals(right);

        /// <inheritdoc/>
        public override string ToString() => $"{Side}{Index}";
    }
}
