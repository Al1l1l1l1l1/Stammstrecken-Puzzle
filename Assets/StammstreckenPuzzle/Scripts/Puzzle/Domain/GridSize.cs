using System;

namespace STP.Puzzle.Domain
{
    /// <summary>
    /// Rastergröße gemäß PUZZLE_ENGINE.md Abschnitt 2: Breite und Höhe mindestens
    /// 2, Runtime-Sicherheitsmaximum 32. Ungültige Größen können nicht
    /// konstruiert werden.
    /// </summary>
    public readonly struct GridSize : IEquatable<GridSize>
    {
        /// <summary>Sicherheitsmaximum je Achse.</summary>
        public const int Maximum = 32;

        /// <summary>Breite des Rasters (Anzahl Spalten).</summary>
        public int Width { get; }

        /// <summary>Höhe des Rasters (Anzahl Zeilen).</summary>
        public int Height { get; }

        /// <summary>Erstellt eine validierte Rastergröße.</summary>
        public GridSize(int width, int height)
        {
            if (width < 2 || height < 2 || width > Maximum || height > Maximum)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(width),
                    $"Rastergröße {width}x{height} außerhalb von 2..{Maximum} je Achse.");
            }
            Width = width;
            Height = height;
        }

        /// <summary>Prüft, ob eine Koordinate innerhalb des Rasters liegt.</summary>
        public bool Contains(CellCoordinate coordinate)
        {
            return coordinate.X >= 0 && coordinate.X < Width && coordinate.Y >= 0 && coordinate.Y < Height;
        }

        /// <summary>Liefert den zeilenweisen Zellindex einer Koordinate (y * Width + x).</summary>
        public int IndexOf(CellCoordinate coordinate)
        {
            return coordinate.Y * Width + coordinate.X;
        }

        /// <inheritdoc/>
        public bool Equals(GridSize other) => Width == other.Width && Height == other.Height;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is GridSize other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => (Width << 16) ^ Height;

        /// <summary>Gleichheit zweier Rastergrößen.</summary>
        public static bool operator ==(GridSize left, GridSize right) => left.Equals(right);

        /// <summary>Ungleichheit zweier Rastergrößen.</summary>
        public static bool operator !=(GridSize left, GridSize right) => !left.Equals(right);

        /// <inheritdoc/>
        public override string ToString() => $"{Width}x{Height}";
    }
}
