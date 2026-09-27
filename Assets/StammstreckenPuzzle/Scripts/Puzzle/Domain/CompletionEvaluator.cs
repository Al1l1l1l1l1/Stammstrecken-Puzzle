using System;
using System.Collections.Generic;

namespace STP.Puzzle.Domain
{
    /// <summary>
    /// Exakte Completion-Prüfung gemäß PUZZLE_ENGINE.md Abschnitt 6. Ein Puzzle
    /// ist genau dann gelöst, wenn die konkreten Gleise exakt die Randzahlen
    /// erfüllen und zusammen mit A und B einen einzigen einfachen Pfad bilden.
    /// Hilfsmarkierungen außerhalb der Strecke entwerten den Abschluss nicht;
    /// ein Vergleich mit einer Authoringlösung findet niemals statt.
    /// </summary>
    public static class CompletionEvaluator
    {
        /// <summary>
        /// Prüft den vollständigen Zielzustand: exakte Randzahlen aus konkreten
        /// Tracks, alle Anschlüsse geschlossen (Grad zwei je Trackzelle, Grad
        /// eins je Endpoint), Traversierung von A nach B besucht jede konkrete
        /// Trackzelle genau einmal; keine zweite Komponente, Schleife oder
        /// Verbindung zu einer falschen Außenkante.
        /// </summary>
        public static bool IsSolved(PuzzleDefinition definition, IReadOnlyList<CellContent> cells)
        {
            if (definition is null)
            {
                throw new ArgumentNullException(nameof(definition));
            }
            if (cells is null || cells.Count != definition.Grid.Width * definition.Grid.Height)
            {
                return false;
            }

            var grid = definition.Grid;
            var trackCount = 0;

            for (var y = 0; y < grid.Height; y++)
            {
                var rowTracks = 0;
                for (var x = 0; x < grid.Width; x++)
                {
                    if (CellContentSemantics.IsConcreteTrack(cells[grid.IndexOf(new CellCoordinate(x, y))]))
                    {
                        rowTracks++;
                    }
                }
                if (rowTracks != definition.RowCounts[y])
                {
                    return false;
                }
                trackCount += rowTracks;
            }

            for (var x = 0; x < grid.Width; x++)
            {
                var columnTracks = 0;
                for (var y = 0; y < grid.Height; y++)
                {
                    if (CellContentSemantics.IsConcreteTrack(cells[grid.IndexOf(new CellCoordinate(x, y))]))
                    {
                        columnTracks++;
                    }
                }
                if (columnTracks != definition.ColumnCounts[x])
                {
                    return false;
                }
            }

            if (trackCount == 0)
            {
                return false;
            }

            // Jeder Anschluss jeder konkreten Zelle muss geschlossen sein:
            // innen ein konkreter Gegenport, außen ein Endpoint an exakt dieser
            // Kante. Damit hat jede Trackzelle Grad zwei im passenden Graphen.
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
                    var (first, second) = TrackShapeGeometry.Ports(CellContentSemantics.ToTrackShape(content));
                    if (!IsConnectionClosed(definition, cells, coordinate, first)
                        || !IsConnectionClosed(definition, cells, coordinate, second))
                    {
                        return false;
                    }
                }
            }

            // A und B haben jeweils Grad eins und verbinden mit ihrer Zelle.
            var startA = definition.A.AdjacentCell(grid);
            var startB = definition.B.AdjacentCell(grid);
            if (!ConnectsToEndpoint(definition, cells, startA, definition.A)
                || !ConnectsToEndpoint(definition, cells, startB, definition.B))
            {
                return false;
            }

            // Traversierung ab A muss B erreichen und jede konkrete Zelle
            // genau einmal besuchen. Da alle Grade geschlossen sind, schliesst
            // das Zählen zweite Komponenten und Schleifen aus.
            var visited = new HashSet<CellCoordinate> { startA };
            var current = startA;
            var incoming = definition.A.Side;
            var reachedB = startA == startB
                && TrackShapeGeometry.HasPort(CellContentSemantics.ToTrackShape(cells[grid.IndexOf(startA)]), definition.B.Side);

            while (!reachedB)
            {
                var content = cells[grid.IndexOf(current)];
                var (first, second) = TrackShapeGeometry.Ports(CellContentSemantics.ToTrackShape(content));
                var outgoing = first == incoming ? second : first;
                var next = current.Neighbor(outgoing);

                if (!grid.Contains(next))
                {
                    // Einzige erlaubte Außenkante ist B an exakt dieser Kante.
                    if (definition.IsEndpointAt(definition.B, current, outgoing))
                    {
                        reachedB = true;
                        continue;
                    }
                    return false;
                }

                if (!visited.Add(next))
                {
                    return false;
                }
                current = next;
                incoming = DirectionGeometry.Opposite(outgoing);

                if (current == startB
                    && TrackShapeGeometry.HasPort(
                        CellContentSemantics.ToTrackShape(cells[grid.IndexOf(current)]), definition.B.Side))
                {
                    reachedB = true;
                }
                if (visited.Count > trackCount)
                {
                    return false;
                }
            }

            return visited.Count == trackCount;
        }

        private static bool IsConnectionClosed(
            PuzzleDefinition definition,
            IReadOnlyList<CellContent> cells,
            CellCoordinate coordinate,
            Direction port)
        {
            var grid = definition.Grid;
            var neighbor = coordinate.Neighbor(port);
            if (!grid.Contains(neighbor))
            {
                return definition.TryGetEndpointAt(coordinate, port, out _);
            }
            var neighborContent = cells[grid.IndexOf(neighbor)];
            return CellContentSemantics.IsConcreteTrack(neighborContent)
                && CellContentSemantics.HasPort(neighborContent, DirectionGeometry.Opposite(port));
        }

        private static bool ConnectsToEndpoint(
            PuzzleDefinition definition,
            IReadOnlyList<CellContent> cells,
            CellCoordinate adjacentCell,
            Endpoint endpoint)
        {
            var content = cells[definition.Grid.IndexOf(adjacentCell)];
            return CellContentSemantics.IsConcreteTrack(content)
                && CellContentSemantics.HasPort(content, endpoint.Side);
        }
    }
}
