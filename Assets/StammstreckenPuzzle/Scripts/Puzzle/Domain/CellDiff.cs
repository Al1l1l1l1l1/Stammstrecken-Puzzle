using System;
using System.Collections.Generic;

namespace STP.Puzzle.Domain
{
    /// <summary>
    /// Eine einzelne beabsichtigte Zelländerung eines Batch-Commands.
    /// </summary>
    public readonly struct CellChange : IEquatable<CellChange>
    {
        /// <summary>Zielkoordinate im Raster.</summary>
        public CellCoordinate Coordinate { get; }

        /// <summary>Neuer Zellinhalt.</summary>
        public CellContent Content { get; }

        /// <summary>Erstellt eine Zelländerung.</summary>
        public CellChange(CellCoordinate coordinate, CellContent content)
        {
            Coordinate = coordinate;
            Content = content;
        }

        /// <inheritdoc/>
        public bool Equals(CellChange other) => Coordinate == other.Coordinate && Content == other.Content;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is CellChange other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => (Coordinate.GetHashCode() << 4) ^ (int)Content;
    }

    /// <summary>
    /// Ein Tupel (Koordinate, vorher, nachher) eines atomaren Diffs.
    /// </summary>
    public readonly struct CellDiffEntry : IEquatable<CellDiffEntry>
    {
        /// <summary>Betroffene Koordinate.</summary>
        public CellCoordinate Coordinate { get; }

        /// <summary>Inhalt vor der Handlung.</summary>
        public CellContent Before { get; }

        /// <summary>Inhalt nach der Handlung.</summary>
        public CellContent After { get; }

        /// <summary>Erstellt einen Diff-Eintrag.</summary>
        public CellDiffEntry(CellCoordinate coordinate, CellContent before, CellContent after)
        {
            Coordinate = coordinate;
            Before = before;
            After = after;
        }

        /// <inheritdoc/>
        public bool Equals(CellDiffEntry other)
        {
            return Coordinate == other.Coordinate && Before == other.Before && After == other.After;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is CellDiffEntry other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return (Coordinate.GetHashCode() << 8) ^ ((int)Before << 4) ^ (int)After;
        }
    }

    /// <summary>
    /// Atomarer, nach Koordinate sortierter Zell-Diff einer Nutzerhandlung gemäß
    /// PUZZLE_ENGINE.md Abschnitt 8. Der Undo-Stack enthält maximal 256 solcher
    /// Diffs; beim Überschreiten wird der älteste verworfen.
    /// </summary>
    public sealed class CellDiff
    {
        /// <summary>Maximale Stacktiefe des Undo-Verlaufs.</summary>
        public const int MaximumUndoDepth = 256;

        /// <summary>Sortierte Einträge (Koordinate, vorher, nachher); niemals leer.</summary>
        public IReadOnlyList<CellDiffEntry> Entries { get; }

        /// <summary>Erstellt einen Diff aus unsortierten Einträgen und sortiert sie zeilenweise.</summary>
        public CellDiff(IReadOnlyList<CellDiffEntry> entries)
        {
            if (entries is null || entries.Count == 0)
            {
                throw new ArgumentException("Ein CellDiff benötigt mindestens einen Eintrag.", nameof(entries));
            }
            var sorted = new List<CellDiffEntry>(entries);
            sorted.Sort((left, right) => left.Coordinate.CompareTo(right.Coordinate));
            Entries = sorted;
        }
    }
}
