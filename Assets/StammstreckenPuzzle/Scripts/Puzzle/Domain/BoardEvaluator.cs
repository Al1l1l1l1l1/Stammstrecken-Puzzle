using System;
using System.Collections.Generic;

namespace STP.Puzzle.Domain
{
    /// <summary>
    /// Ermittelt die sichtbaren objektiven Diagnosen gemäß PUZZLE_ENGINE.md
    /// Abschnitt 5 ausschließlich aus Definition und aktuellem Zellstand. Ein
    /// einzelnes konkretes Gleis mit Anschluss zu einer noch unbestimmten
    /// Nachbarzelle ist nicht automatisch falsch; gemeldet werden nur
    /// Widersprüche zu bereits konkretem Nachbar, Rastergrenze oder Endpoint.
    /// </summary>
    public static class BoardEvaluator
    {
        /// <summary>
        /// Berechnet die vollständige, deterministisch sortierte Diagnoselage.
        /// </summary>
        public static ObjectiveDiagnostics Evaluate(PuzzleDefinition definition, IReadOnlyList<CellContent> cells)
        {
            if (definition is null)
            {
                throw new ArgumentNullException(nameof(definition));
            }
            if (cells is null || cells.Count != definition.Grid.Width * definition.Grid.Height)
            {
                throw new ArgumentException("Zellstand passt nicht zur Rastergröße.", nameof(cells));
            }

            var grid = definition.Grid;
            var exceededRows = new List<int>();
            var exceededColumns = new List<int>();
            var cellDiagnostics = new List<CellDiagnostic>();

            for (var y = 0; y < grid.Height; y++)
            {
                var visible = 0;
                for (var x = 0; x < grid.Width; x++)
                {
                    if (CellContentSemantics.CountsAsVisibleOccupancy(cells[grid.IndexOf(new CellCoordinate(x, y))]))
                    {
                        visible++;
                    }
                }
                if (visible > definition.RowCounts[y])
                {
                    exceededRows.Add(y);
                }
            }

            for (var x = 0; x < grid.Width; x++)
            {
                var visible = 0;
                for (var y = 0; y < grid.Height; y++)
                {
                    if (CellContentSemantics.CountsAsVisibleOccupancy(cells[grid.IndexOf(new CellCoordinate(x, y))]))
                    {
                        visible++;
                    }
                }
                if (visible > definition.ColumnCounts[x])
                {
                    exceededColumns.Add(x);
                }
            }

            var mismatchReported = new HashSet<long>();
            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    var coordinate = new CellCoordinate(x, y);
                    var content = cells[grid.IndexOf(coordinate)];
                    if (!CellContentSemantics.IsConcreteTrack(content))
                    {
                        continue;
                    }

                    var shape = CellContentSemantics.ToTrackShape(content);
                    var (first, second) = TrackShapeGeometry.Ports(shape);
                    EvaluatePort(definition, cells, coordinate, first, mismatchReported, cellDiagnostics);
                    EvaluatePort(definition, cells, coordinate, second, mismatchReported, cellDiagnostics);

                    if (definition.A.AdjacentCell(grid) == coordinate && !TrackShapeGeometry.HasPort(shape, definition.A.Side))
                    {
                        cellDiagnostics.Add(new CellDiagnostic(ObjectiveDiagnosticCodes.EndpointMismatch, coordinate));
                    }
                    if (definition.B.AdjacentCell(grid) == coordinate && !TrackShapeGeometry.HasPort(shape, definition.B.Side))
                    {
                        cellDiagnostics.Add(new CellDiagnostic(ObjectiveDiagnosticCodes.EndpointMismatch, coordinate));
                    }
                }
            }

            foreach (var loopCell in FindConcreteLoopCells(definition, cells))
            {
                cellDiagnostics.Add(new CellDiagnostic(ObjectiveDiagnosticCodes.PrematureConcreteLoop, loopCell));
            }

            cellDiagnostics.Sort();
            var deduplicated = new List<CellDiagnostic>();
            foreach (var diagnostic in cellDiagnostics)
            {
                if (deduplicated.Count == 0 || !deduplicated[deduplicated.Count - 1].Equals(diagnostic))
                {
                    deduplicated.Add(diagnostic);
                }
            }

            return new ObjectiveDiagnostics(exceededRows, exceededColumns, deduplicated);
        }

        private static void EvaluatePort(
            PuzzleDefinition definition,
            IReadOnlyList<CellContent> cells,
            CellCoordinate coordinate,
            Direction port,
            HashSet<long> mismatchReported,
            List<CellDiagnostic> cellDiagnostics)
        {
            var grid = definition.Grid;
            var neighbor = coordinate.Neighbor(port);
            if (!grid.Contains(neighbor))
            {
                if (!definition.TryGetEndpointAt(coordinate, port, out _))
                {
                    cellDiagnostics.Add(new CellDiagnostic(ObjectiveDiagnosticCodes.TrackExitsGrid, coordinate));
                }
                return;
            }

            var neighborContent = cells[grid.IndexOf(neighbor)];
            if (!CellContentSemantics.IsConcreteTrack(neighborContent))
            {
                return;
            }
            if (CellContentSemantics.HasPort(neighborContent, DirectionGeometry.Opposite(port)))
            {
                return;
            }

            var first = coordinate.CompareTo(neighbor) <= 0 ? coordinate : neighbor;
            var edgeKey = ((long)first.Y << 32) | (uint)first.X | ((long)(port == Direction.E || port == Direction.W ? 1 : 0) << 62);
            if (mismatchReported.Add(edgeKey))
            {
                cellDiagnostics.Add(new CellDiagnostic(ObjectiveDiagnosticCodes.TrackConnectionMismatch, first));
            }
        }

        /// <summary>
        /// Findet geschlossene Komponenten konkreter Gleise über Union-Find und
        /// liefert je Schleife die zeilenweise kleinste beteiligte Zelle.
        /// </summary>
        private static IReadOnlyList<CellCoordinate> FindConcreteLoopCells(
            PuzzleDefinition definition,
            IReadOnlyList<CellContent> cells)
        {
            var grid = definition.Grid;
            var total = grid.Width * grid.Height;
            var parent = new int[total];
            for (var index = 0; index < total; index++)
            {
                parent[index] = index;
            }

            int Find(int node)
            {
                while (parent[node] != node)
                {
                    parent[node] = parent[parent[node]];
                    node = parent[node];
                }
                return node;
            }

            var loopRoots = new HashSet<int>();
            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    var coordinate = new CellCoordinate(x, y);
                    var content = cells[grid.IndexOf(coordinate)];
                    if (!CellContentSemantics.IsConcreteTrack(content))
                    {
                        continue;
                    }
                    var shape = CellContentSemantics.ToTrackShape(content);
                    var (first, second) = TrackShapeGeometry.Ports(shape);
                    foreach (var port in new[] { first, second })
                    {
                        var neighbor = coordinate.Neighbor(port);
                        if (!grid.Contains(neighbor))
                        {
                            continue;
                        }
                        var neighborContent = cells[grid.IndexOf(neighbor)];
                        if (!CellContentSemantics.IsConcreteTrack(neighborContent)
                            || !CellContentSemantics.HasPort(neighborContent, DirectionGeometry.Opposite(port)))
                        {
                            continue;
                        }
                        // Jede Kante wird nur von der zeilenweise kleineren Zelle
                        // aus verarbeitet, sonst würde eine einzelne Kante beim
                        // zweiten Durchlauf faelschlich als Schleife gemeldet.
                        if (coordinate.CompareTo(neighbor) >= 0)
                        {
                            continue;
                        }
                        var rootA = Find(grid.IndexOf(coordinate));
                        var rootB = Find(grid.IndexOf(neighbor));
                        if (rootA == rootB)
                        {
                            loopRoots.Add(rootA);
                        }
                        else
                        {
                            parent[rootB] = rootA;
                        }
                    }
                }
            }

            var lowestByRoot = new Dictionary<int, CellCoordinate>();
            if (loopRoots.Count > 0)
            {
                for (var y = 0; y < grid.Height; y++)
                {
                    for (var x = 0; x < grid.Width; x++)
                    {
                        var coordinate = new CellCoordinate(x, y);
                        if (!CellContentSemantics.IsConcreteTrack(cells[grid.IndexOf(coordinate)]))
                        {
                            continue;
                        }
                        var root = Find(grid.IndexOf(coordinate));
                        if (!loopRoots.Contains(root))
                        {
                            continue;
                        }
                        if (!lowestByRoot.TryGetValue(root, out var lowest) || coordinate.CompareTo(lowest) < 0)
                        {
                            lowestByRoot[root] = coordinate;
                        }
                    }
                }
            }

            var result = new List<CellCoordinate>(lowestByRoot.Values);
            result.Sort();
            return result;
        }
    }
}
