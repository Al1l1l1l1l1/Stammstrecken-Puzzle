using System;
using System.Collections.Generic;

namespace STP.Tests.Solver.EditMode
{
    // FIXTURE_ONLY. Independent simple-path enumeration: no Domain completion,
    // track ports, candidate propagation or production solver calls.
    internal static class SmallPathOracle
    {
        internal readonly struct Edge
        {
            public readonly int Side, Index;
            public Edge(int side, int index) { Side = side; Index = index; }
            public int Cell(int w, int h) => Side == 0 ? Index : Side == 1 ? Index * w + w - 1 : Side == 2 ? (h - 1) * w + Index : Index * w;
        }

        internal static List<Edge> Edges(int w, int h)
        {
            var edges = new List<Edge>();
            for (int side = 0; side < 4; side++)
                for (int index = 0; index < (side % 2 == 0 ? w : h); index++) edges.Add(new Edge(side, index));
            return edges;
        }

        internal static Dictionary<string, int> Counts(int w, int h, Edge a, Edge b)
        {
            var counts = new Dictionary<string, int>(StringComparer.Ordinal);
            int start = a.Cell(w, h), end = b.Cell(w, h);
            var visited = new bool[w * h];
            var rows = new int[h]; var columns = new int[w];
            void Walk(int cell)
            {
                visited[cell] = true; rows[cell / w]++; columns[cell % w]++;
                if (cell == end)
                {
                    var key = Key(rows, columns);
                    counts[key] = Math.Min(2, counts.TryGetValue(key, out var n) ? n + 1 : 1);
                }
                else
                {
                    int x = cell % w, y = cell / w;
                    if (y > 0 && !visited[cell - w]) Walk(cell - w);
                    if (x + 1 < w && !visited[cell + 1]) Walk(cell + 1);
                    if (y + 1 < h && !visited[cell + w]) Walk(cell + w);
                    if (x > 0 && !visited[cell - 1]) Walk(cell - 1);
                }
                visited[cell] = false; rows[cell / w]--; columns[cell % w]--;
            }
            Walk(start);
            return counts;
        }

        internal static string Key(int[] rows, int[] columns) => string.Join(",", rows) + ";" + string.Join(",", columns);
        // Exact boards produced by independent simple paths, including exterior
        // entry/exit directions. Used to test Completion, not just solution count.
        internal static HashSet<string> Boards(int w, int h, Edge a, Edge b)
        {
            var boards = new HashSet<string>(StringComparer.Ordinal); var path = new List<int>(); var seen = new bool[w * h];
            int DirectionTo(int from, int to) => to == from - w ? 0 : to == from + 1 ? 1 : to == from + w ? 2 : 3;
            void Walk(int cell)
            {
                seen[cell] = true; path.Add(cell);
                if (cell == b.Cell(w, h))
                {
                    var board = new int[w * h];
                    for (int i = 0; i < path.Count; i++)
                    {
                        int incoming = i == 0 ? a.Side : DirectionTo(path[i], path[i - 1]);
                        int outgoing = i == path.Count - 1 ? b.Side : DirectionTo(path[i], path[i + 1]);
                        int mask = (1 << incoming) | (1 << outgoing);
                        // Closed six-shape contract, independent of TrackGeometry.
                        board[path[i]] = mask == 5 ? 1 : mask == 10 ? 2 : mask == 3 ? 3 : mask == 6 ? 4 : mask == 12 ? 5 : mask == 9 ? 6 : -1;
                    }
                    boards.Add(string.Join("", board));
                }
                else
                {
                    int x = cell % w, y = cell / w;
                    if (y > 0 && !seen[cell - w]) Walk(cell - w);
                    if (x + 1 < w && !seen[cell + 1]) Walk(cell + 1);
                    if (y + 1 < h && !seen[cell + w]) Walk(cell + w);
                    if (x > 0 && !seen[cell - 1]) Walk(cell - 1);
                }
                path.RemoveAt(path.Count - 1); seen[cell] = false;
            }
            Walk(a.Cell(w, h)); return boards;
        }
        internal static IEnumerable<int[]> Vectors(int length, int max)
        {
            var vector = new int[length];
            while (true)
            {
                yield return (int[])vector.Clone();
                int i = 0;
                while (i < length && vector[i] == max) { vector[i] = 0; i++; }
                if (i == length) yield break;
                vector[i]++;
            }
        }
    }
}
