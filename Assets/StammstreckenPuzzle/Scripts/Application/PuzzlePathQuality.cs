using System;
using System.Collections.Generic;
using System.Linq;
using STP.Puzzle.Domain;

namespace STP.Application
{
    /// <summary>Deterministic structural authoring filters, not puzzle rules or human difficulty approval.</summary>
    public static class PuzzlePathQuality
    {
        public const string Version = "path-geometry-v1";

        /// <summary>Zero/full counts are separate from one-lines, which do not decide a whole line.</summary>
        public static bool AcceptsCounts(PuzzleDefinition puzzle)
        {
            if (puzzle == null) throw new ArgumentNullException(nameof(puzzle));
            int width = puzzle.Grid.Width, height = puzzle.Grid.Height;
            bool small = Math.Min(width, height) < 6;
            int zero = puzzle.RowCounts.Count(n => n == 0) + puzzle.ColumnCounts.Count(n => n == 0);
            int full = puzzle.RowCounts.Count(n => n == width) + puzzle.ColumnCounts.Count(n => n == height);
            int rows = puzzle.RowCounts.Count(n => n == 0 || n == width);
            int columns = puzzle.ColumnCounts.Count(n => n == 0 || n == height);
            // Count the union, not the sum: intersections must not inflate the decided area.
            int fixedCells = rows * width + columns * height - rows * columns;
            return zero <= (small ? 2 : 1) && full <= (small ? 2 : 1) &&
                fixedCells * 100 <= puzzle.Grid.CellCount * (small ? 50 : 30);
        }

        /// <summary>At least 3/4/5/... internal turns, spread across the board; extra A-B travel, no four-arm sweep.</summary>
        public static bool Accepts(PuzzleDefinition puzzle, IReadOnlyList<CellCoordinate> path)
        {
            if (puzzle == null || path == null) throw new ArgumentNullException();
            if (!AcceptsCounts(puzzle) || path.Count < 2) return false;
            int size = Math.Min(puzzle.Grid.Width, puzzle.Grid.Height);
            int distance = Math.Abs(puzzle.A.Cell.X - puzzle.B.Cell.X) + Math.Abs(puzzle.A.Cell.Y - puzzle.B.Cell.Y);
            int edges = path.Count - 1;
            if (edges - distance < (size >= 8 ? 6 : size >= 6 ? 4 : 2) || edges * 4 < distance * 5) return false;
            var runs = new List<(int X, int Y, int Length)>();
            var turnRows = new HashSet<int>(); var turnColumns = new HashSet<int>();
            for (int i = 1; i < path.Count; i++)
            {
                int x = path[i].X - path[i - 1].X, y = path[i].Y - path[i - 1].Y;
                if (Math.Abs(x) + Math.Abs(y) != 1) return false;
                if (runs.Count > 0 && runs[runs.Count - 1].X == x && runs[runs.Count - 1].Y == y)
                {
                    var previous = runs[runs.Count - 1];
                    runs[runs.Count - 1] = (x, y, previous.Length + 1);
                }
                else
                {
                    if (runs.Count > 0) { turnRows.Add(path[i - 1].Y); turnColumns.Add(path[i - 1].X); }
                    runs.Add((x, y, 1));
                }
            }
            if (runs.Count - 1 < size - 1 || (size >= 6 && (turnRows.Count < 2 || turnColumns.Count < 2))) return false;
            return MaximumParallelArms(runs) <= 3;
        }

        /// <summary>Longest alternating parallel-arm series joined by identically directed connectors, regardless of lengths.</summary>
        private static int MaximumParallelArms(IReadOnlyList<(int X, int Y, int Length)> runs)
        {
            int maximum = 1;
            for (int start = 0; start + 2 < runs.Count; start++)
            {
                if (runs[start].Length < 2) continue;
                var connector = runs[start + 1]; int arms = 1;
                for (int i = start + 2; i < runs.Count; i += 2)
                {
                    var previous = runs[i - 2]; var arm = runs[i]; var join = runs[i - 1];
                    if (arm.Length < 2 || arm.X != -previous.X || arm.Y != -previous.Y ||
                        join.X != connector.X || join.Y != connector.Y) break;
                    arms++; maximum = Math.Max(maximum, arms);
                }
            }
            return maximum;
        }
    }
}
