using System;
using System.Collections.Generic;
using System.Linq;

namespace STP.Puzzle.Domain
{
    /// <summary>Only objective facts visible in the current player state.</summary>
    public enum DiagnosticCode { ROW_COUNT_EXCEEDED, COLUMN_COUNT_EXCEEDED, TRACK_EXITS_GRID, TRACK_CONNECTION_MISMATCH, PREMATURE_CONCRETE_LOOP, ENDPOINT_MISMATCH }
    /// <summary>A stable, coordinate-bound visible violation.</summary>
    public sealed class PuzzleDiagnostic
    {
        public CellCoordinate Coordinate { get; }
        public DiagnosticCode Code { get; }
        internal PuzzleDiagnostic(CellCoordinate coordinate, DiagnosticCode code) { Coordinate = coordinate; Code = code; }
    }
    /// <summary>Visible counts and authoritative completion, without a solution oracle.</summary>
    public sealed class PuzzleEvaluation
    {
        public DomainError Error { get; }
        public bool Completed { get; }
        public IReadOnlyList<int> VisibleRows { get; }
        public IReadOnlyList<int> VisibleColumns { get; }
        public IReadOnlyList<PuzzleDiagnostic> Diagnostics { get; }
        public IReadOnlyList<CellCoordinate> Path { get; }
        internal PuzzleEvaluation(DomainError error, int[] rows, int[] columns, List<PuzzleDiagnostic> diagnostics, List<CellCoordinate> path, bool completed)
        { Error = error; VisibleRows = Array.AsReadOnly(rows); VisibleColumns = Array.AsReadOnly(columns); Diagnostics = diagnostics.AsReadOnly(); Path = path.AsReadOnly(); Completed = completed; }
    }
    /// <summary>Pure visible diagnostics and exact A-B graph traversal.</summary>
    public static class PuzzleEvaluator
    {
        public static PuzzleEvaluation Evaluate(PuzzleDefinition? definition, IReadOnlyList<CellContent>? cells)
        {
            if (definition == null || cells == null || cells.Count != definition.Grid.CellCount || cells.Any(c => !TrackGeometry.IsContent(c)))
                return new PuzzleEvaluation(DomainError.INVALID_DEFINITION, Array.Empty<int>(), Array.Empty<int>(), new List<PuzzleDiagnostic>(), new List<CellCoordinate>(), false);
            var p = definition; int w = p.Grid.Width, h = p.Grid.Height;
            var rows = new int[h]; var columns = new int[w]; var concreteRows = new int[h]; var concreteCols = new int[w];
            var diagnoses = new List<PuzzleDiagnostic>(); var neighbors = new List<int>[cells.Count]; var external = new int[cells.Count]; int trackCount = 0;
            void Add(int x, int y, DiagnosticCode code) => diagnoses.Add(new PuzzleDiagnostic(new CellCoordinate(x, y), code));
            for (int i = 0; i < cells.Count; i++)
            {
                neighbors[i] = new List<int>(2); int x = i % w, y = i / w;
                bool track = TrackGeometry.IsTrack(cells[i]);
                if (track || cells[i] == CellContent.MARK_OCCUPIED) { rows[y]++; columns[x]++; }
                if (!track) continue;
                trackCount++; concreteRows[y]++; concreteCols[x]++;
                int ports = TrackGeometry.Ports(cells[i]);
                for (int d = 0; d < 4; d++)
                {
                    var direction = (Direction)d; int nx = x + TrackGeometry.DeltaX(direction), ny = y + TrackGeometry.DeltaY(direction); bool has = (ports & (1 << d)) != 0;
                    if (!p.Grid.Contains(nx, ny))
                    {
                        if (has) { if (p.IsEndpointPort(x, y, direction)) external[i]++; else Add(x, y, DiagnosticCode.TRACK_EXITS_GRID); }
                    }
                    else
                    {
                        int ni = ny * w + nx;
                        if (!TrackGeometry.IsTrack(cells[ni])) continue;
                        bool other = (TrackGeometry.Ports(cells[ni]) & (1 << (int)TrackGeometry.Opposite(direction))) != 0;
                        if (has != other) Add(x, y, DiagnosticCode.TRACK_CONNECTION_MISMATCH);
                        if (has && other) neighbors[i].Add(ni);
                    }
                }
                if ((p.A.Cell.X == x && p.A.Cell.Y == y && (ports & (1 << (int)p.A.Side)) == 0) ||
                    (p.B.Cell.X == x && p.B.Cell.Y == y && (ports & (1 << (int)p.B.Side)) == 0)) Add(x, y, DiagnosticCode.ENDPOINT_MISMATCH);
            }
            for (int y = 0; y < h; y++) if (rows[y] > p.RowCounts[y]) Add(0, y, DiagnosticCode.ROW_COUNT_EXCEEDED);
            for (int x = 0; x < w; x++) if (columns[x] > p.ColumnCounts[x]) Add(x, 0, DiagnosticCode.COLUMN_COUNT_EXCEEDED);
            var seen = new bool[cells.Count];
            for (int i = 0; i < cells.Count; i++)
            {
                if (seen[i] || !TrackGeometry.IsTrack(cells[i])) continue;
                var component = new List<int>(); var queue = new Queue<int>(); queue.Enqueue(i); seen[i] = true; bool closed = true;
                while (queue.Count > 0) { int node = queue.Dequeue(); component.Add(node); closed &= neighbors[node].Count == 2 && external[node] == 0; foreach (int next in neighbors[node]) if (!seen[next]) { seen[next] = true; queue.Enqueue(next); } }
                if (closed) foreach (int node in component) Add(node % w, node / w, DiagnosticCode.PREMATURE_CONCRETE_LOOP);
            }
            diagnoses = diagnoses.GroupBy(d => (d.Coordinate.Y, d.Coordinate.X, d.Code)).Select(g => g.First()).OrderBy(d => d.Coordinate.Y).ThenBy(d => d.Coordinate.X).ThenBy(d => d.Code).ToList();
            bool valid = trackCount > 0 && concreteRows.SequenceEqual(p.RowCounts) && concreteCols.SequenceEqual(p.ColumnCounts);
            for (int i = 0; i < cells.Count; i++) if (TrackGeometry.IsTrack(cells[i]) && neighbors[i].Count + external[i] != 2) valid = false;
            int a = p.Index(p.A.Cell), b = p.Index(p.B.Cell);
            valid &= (TrackGeometry.Ports(cells[a]) & (1 << (int)p.A.Side)) != 0 && (TrackGeometry.Ports(cells[b]) & (1 << (int)p.B.Side)) != 0;
            // Boundary diagnostics are authoritative even if an accidental graph
            // degree happens to fit after ignoring a forbidden exterior edge.
            valid &= !diagnoses.Any(d => d.Code == DiagnosticCode.TRACK_EXITS_GRID || d.Code == DiagnosticCode.ENDPOINT_MISMATCH || d.Code == DiagnosticCode.TRACK_CONNECTION_MISMATCH || d.Code == DiagnosticCode.PREMATURE_CONCRETE_LOOP);
            var path = new List<CellCoordinate>(); Array.Clear(seen, 0, seen.Length); int current = a, previous = -1;
            while (valid)
            {
                if (seen[current]) { valid = false; break; }
                seen[current] = true; path.Add(new CellCoordinate(current % w, current / w));
                if (current == b) { valid = path.Count == trackCount; break; }
                int next = -1;
                foreach (int neighbor in neighbors[current]) if (neighbor != previous) { if (next != -1) { valid = false; break; } next = neighbor; }
                if (!valid || next == -1) { valid = false; break; } previous = current; current = next;
            }
            if (!valid) path.Clear();
            return new PuzzleEvaluation(DomainError.NONE, rows, columns, diagnoses, path, valid);
        }
    }
}
