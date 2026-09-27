using System;

namespace STP.Puzzle.Domain
{
    /// <summary>
    /// Ganzzahlige Rasterkoordinate gemäß LEVEL_DATA_FORMAT.md Abschnitt 2:
    /// Ursprung links oben (0, 0), x wächst nach rechts, y nach unten. Die
    /// Existenz einer Koordinate ist relativ zu einer <see cref="GridSize"/> zu
    /// prüfen. Die deterministische Sortierung erfolgt zeilenweise (y, dann x).
    /// </summary>
    public readonly struct CellCoordinate : IEquatable<CellCoordinate>, IComparable<CellCoordinate>
    {
        /// <summary>Spaltenindex (wächst nach rechts).</summary>
        public int X { get; }

        /// <summary>Zeilenindex (wächst nach unten).</summary>
        public int Y { get; }

        /// <summary>Erstellt eine Koordinate aus Spalten- und Zeilenindex.</summary>
        public CellCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>Liefert die orthogonale Nachbarkoordinate in der gegebenen Richtung (ohne Rasterprüfung).</summary>
        public CellCoordinate Neighbor(Direction direction)
        {
            return new CellCoordinate(
                X + DirectionGeometry.DeltaX(direction),
                Y + DirectionGeometry.DeltaY(direction));
        }

        /// <inheritdoc/>
        public bool Equals(CellCoordinate other) => X == other.X && Y == other.Y;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is CellCoordinate other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => (Y << 16) ^ X;

        /// <summary>Zeilenweise Ordnung: erst y, dann x.</summary>
        public int CompareTo(CellCoordinate other)
        {
            var byY = Y.CompareTo(other.Y);
            return byY != 0 ? byY : X.CompareTo(other.X);
        }

        /// <summary>Gleichheit zweier Koordinaten.</summary>
        public static bool operator ==(CellCoordinate left, CellCoordinate right) => left.Equals(right);

        /// <summary>Ungleichheit zweier Koordinaten.</summary>
        public static bool operator !=(CellCoordinate left, CellCoordinate right) => !left.Equals(right);

        /// <inheritdoc/>
        public override string ToString() => $"({X},{Y})";
    }
}
