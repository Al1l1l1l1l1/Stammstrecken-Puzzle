using System;
using STP.Puzzle.Domain;
using System.Collections.Generic;

namespace STP.Puzzle.Solver
{
    /// <summary>
    /// Propagation des Constraint-Solvers gemäß SOLVER_ARCHITECTURE.md
    /// Abschnitte 2 und 3. Eine deterministische Fixpunkt-Schleife verarbeitet
    /// Nachbarkonsistenz, Zeilen-/Spaltenzahlen, Subtourverbot und globale
    /// A-B-Erreichbarkeit; Raster- und Endpointports werden einmalig bei der
    /// Initialisierung angewendet. Jede Reduktion wird dedupliziert als
    /// Deduktionsschritt gezählt; eine leere Domäne ist ein Widerspruch.
    /// Rein additiv zur Metrik „maximale Deduktionskettentiefe“
    /// (SOLVER_ARCHITECTURE.md Abschnitte 5 und 7) wird je Reduktion die
    /// Prämissentiefe protokolliert: statische Raster-/Endpointreduktionen
    /// haben Tiefe 1; eine Reduktion durch eine Regel hat Tiefe eins plus der
    /// höchsten aktuellen Tiefe der herangezogenen Prämissenzellen. Diese
    /// Buchführung verändert weder Constraintsemantik noch Tiebreaker,
    /// Wertreihenfolge oder bestehende Metrikwerte.
    /// </summary>
    internal sealed class SolverCore
    {
        private const byte RuleBorder = 1;
        private const byte RuleEndpoint = 2;
        private const byte RuleNeighbor = 3;
        private const byte RuleLine = 4;

        private readonly PuzzleDefinition _definition;
        private readonly GridSize _grid;
        private readonly int _cellCount;
        private readonly HashSet<long> _seenReductions;
        private readonly ReductionCounter _counter;
        private readonly DepthTracker _depthTracker;

        private byte[] _domains;
        private int[] _depths;
        private bool _anyReduction;

        private sealed class ReductionCounter
        {
            internal long Steps;
        }

        private sealed class DepthTracker
        {
            internal long Max;
        }

        private SolverCore(PuzzleDefinition definition, byte[] domains, int[] depths, HashSet<long> seenReductions, ReductionCounter counter, DepthTracker depthTracker)
        {
            _definition = definition;
            _grid = definition.Grid;
            _cellCount = domains.Length;
            _domains = domains;
            _depths = depths;
            _seenReductions = seenReductions;
            _counter = counter;
            _depthTracker = depthTracker;
        }

        /// <summary>Aktuelle Zell-Domänen (interne Repräsentation).</summary>
        internal byte[] Domains => _domains;

        /// <summary>Zahl deduplizierter Reduktionen über den gesamten Suchbaum.</summary>
        internal long DeductionSteps => _counter.Steps;

        /// <summary>Maximale beobachtete Deduktionskettentiefe über den gesamten Suchbaum (0 ohne Reduktion).</summary>
        internal long MaxDeductionDepth => _depthTracker.Max;

        /// <summary>
        /// Erstellt den initialen Solverstand aus dem öffentlichen Puzzleinput,
        /// wendet Raster- und Endpointconstraints an und propagiert bis zum
        /// Fixpunkt. Liefert <c>null</c> bei sofortigem Widerspruch; die bis
        /// dahin angefallenen Deduktionsschritte und die maximale
        /// Deduktionskettentiefe werden ausgegeben.
        /// </summary>
        internal static SolverCore? CreateInitial(PuzzleDefinition definition, out long deductionSteps, out long maxDeductionDepth)
        {
            var cellCount = definition.Grid.Width * definition.Grid.Height;
            var domains = new byte[cellCount];
            for (var index = 0; index < cellCount; index++)
            {
                domains[index] = CellDomain.AllBits;
            }

            var core = new SolverCore(definition, domains, new int[cellCount], new HashSet<long>(), new ReductionCounter(), new DepthTracker());
            var consistent = core.ApplyStaticConstraints() && core.Propagate();
            deductionSteps = core.DeductionSteps;
            maxDeductionDepth = core.MaxDeductionDepth;
            return consistent ? core : null;
        }

        /// <summary>Erstellt eine tiefe Kopie für die Tiefensuche; Reduktions- und Tiefenzählung werden geteilt.</summary>
        internal SolverCore Clone()
        {
            var domains = new byte[_cellCount];
            Array.Copy(_domains, domains, _cellCount);
            var depths = new int[_cellCount];
            Array.Copy(_depths, depths, _cellCount);
            return new SolverCore(_definition, domains, depths, _seenReductions, _counter, _depthTracker);
        }

        /// <summary>
        /// Weist einer Zelle einen Wert zu (Leere oder konkrete Form) und
        /// propagiert. Liefert <c>false</c> bei Widerspruch. Eine Suchannahme
        /// ist keine Deduktion; sie setzt die Kettentiefe der Zelle zurück.
        /// </summary>
        internal bool Assign(int cellIndex, TrackShape? shape)
        {
            var mask = shape is null ? CellDomain.EmptyBit : CellDomain.OnlyShape(shape.Value);
            if ((_domains[cellIndex] & mask) == 0)
            {
                return false;
            }
            _domains[cellIndex] = mask;
            _depths[cellIndex] = 0;
            return Propagate();
        }

        /// <summary>Prüft, ob alle Domänen Singletons sind.</summary>
        internal bool IsCompleteAssignment()
        {
            for (var index = 0; index < _cellCount; index++)
            {
                if (!CellDomain.IsSingleton(_domains[index]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Liefert den MRV-Kandidaten: kleinste Domäne größer eins; Gleichstände
        /// zeilenweise (y, dann x). Liefert -1 bei vollständiger Zuweisung.
        /// </summary>
        internal int MostConstrainedCell()
        {
            var best = -1;
            var bestCount = int.MaxValue;
            for (var index = 0; index < _cellCount; index++)
            {
                var count = CellDomain.Count(_domains[index]);
                if (count > 1 && count < bestCount)
                {
                    bestCount = count;
                    best = index;
                }
            }
            return best;
        }

        /// <summary>Übersetzt die Zuweisung in einen Zellstand (konkrete Gleise oder Unset).</summary>
        internal CellContent[] ExtractCells()
        {
            var cells = new CellContent[_cellCount];
            for (var index = 0; index < _cellCount; index++)
            {
                if (_domains[index] == CellDomain.EmptyBit)
                {
                    cells[index] = CellContent.Unset;
                }
                else
                {
                    cells[index] = CellContentSemantics.FromTrackShape(CellDomain.SingletonShape(_domains[index]));
                }
            }
            return cells;
        }

        /// <summary>Propagiert alle Regeln bis zum Fixpunkt; <c>false</c> bei Widerspruch.</summary>
        internal bool Propagate()
        {
            while (true)
            {
                _anyReduction = false;
                for (var y = 0; y < _grid.Height; y++)
                {
                    for (var x = 0; x < _grid.Width; x++)
                    {
                        if (!ApplyNeighborRule(x, y))
                        {
                            return false;
                        }
                    }
                }
                for (var y = 0; y < _grid.Height; y++)
                {
                    if (!ApplyLineRule(isRow: true, y, _grid.Width, _definition.RowCounts[y]))
                    {
                        return false;
                    }
                }
                for (var x = 0; x < _grid.Width; x++)
                {
                    if (!ApplyLineRule(isRow: false, x, _grid.Height, _definition.ColumnCounts[x]))
                    {
                        return false;
                    }
                }
                if (!ApplySubtourRule())
                {
                    return false;
                }
                if (!ApplyReachabilityRule())
                {
                    return false;
                }
                if (!_anyReduction)
                {
                    return true;
                }
            }
        }

        private bool ApplyStaticConstraints()
        {
            for (var y = 0; y < _grid.Height; y++)
            {
                for (var x = 0; x < _grid.Width; x++)
                {
                    var coordinate = new CellCoordinate(x, y);
                    foreach (var direction in new[] { Direction.N, Direction.E, Direction.S, Direction.W })
                    {
                        if (!_grid.Contains(coordinate.Neighbor(direction))
                            && !_definition.TryGetEndpointAt(coordinate, direction, out _))
                        {
                            if (!Reduce(
                                _grid.IndexOf(coordinate),
                                CellDomain.RemoveTracksWithPort(_domains[_grid.IndexOf(coordinate)], direction),
                                RuleBorder,
                                premiseDepth: 0))
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            if (!ApplyEndpointConstraint(_definition.A) || !ApplyEndpointConstraint(_definition.B))
            {
                return false;
            }
            return true;
        }

        private bool ApplyEndpointConstraint(Endpoint endpoint)
        {
            var cell = endpoint.AdjacentCell(_grid);
            var index = _grid.IndexOf(cell);
            return Reduce(index, CellDomain.KeepOnlyTracksWithPort(_domains[index], endpoint.Side), RuleEndpoint, premiseDepth: 0);
        }

        private bool ApplyNeighborRule(int x, int y)
        {
            var coordinate = new CellCoordinate(x, y);
            var index = _grid.IndexOf(coordinate);
            foreach (var direction in new[] { Direction.N, Direction.E, Direction.S, Direction.W })
            {
                var neighbor = coordinate.Neighbor(direction);
                if (!_grid.Contains(neighbor))
                {
                    continue;
                }
                var neighborIndex = _grid.IndexOf(neighbor);
                var neighborDomain = _domains[neighborIndex];
                var opposite = DirectionGeometry.Opposite(direction);

                if (!CellDomain.AllowsTrackWithPort(neighborDomain, opposite))
                {
                    if (!Reduce(index, CellDomain.RemoveTracksWithPort(_domains[index], direction), RuleNeighbor, _depths[neighborIndex]))
                    {
                        return false;
                    }
                }
                if (CellDomain.AllTracksHavePort(neighborDomain, opposite))
                {
                    if (!Reduce(index, CellDomain.KeepOnlyTracksWithPort(_domains[index], direction), RuleNeighbor, _depths[neighborIndex]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool ApplyLineRule(bool isRow, int lineIndex, int length, int target)
        {
            var mandatory = 0;
            var possible = 0;
            var premiseDepth = 0;
            for (var offset = 0; offset < length; offset++)
            {
                var index = isRow
                    ? _grid.IndexOf(new CellCoordinate(offset, lineIndex))
                    : _grid.IndexOf(new CellCoordinate(lineIndex, offset));
                if (!CellDomain.AllowsEmpty(_domains[index]))
                {
                    mandatory++;
                }
                if (CellDomain.AllowsAnyTrack(_domains[index]))
                {
                    possible++;
                }
                if (_depths[index] > premiseDepth)
                {
                    premiseDepth = _depths[index];
                }
            }

            if (mandatory > target || possible < target)
            {
                return false;
            }

            for (var offset = 0; offset < length; offset++)
            {
                var index = isRow
                    ? _grid.IndexOf(new CellCoordinate(offset, lineIndex))
                    : _grid.IndexOf(new CellCoordinate(lineIndex, offset));
                if (possible == target && CellDomain.AllowsAnyTrack(_domains[index]))
                {
                    if (!Reduce(index, CellDomain.RemoveEmpty(_domains[index]), RuleLine, premiseDepth))
                    {
                        return false;
                    }
                }
                if (mandatory == target && CellDomain.AllowsEmpty(_domains[index]))
                {
                    if (!Reduce(index, CellDomain.OnlyEmpty(_domains[index]), RuleLine, premiseDepth))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool ApplySubtourRule()
        {
            var nodeCount = _cellCount + 2;
            var parent = new int[nodeCount];
            for (var node = 0; node < nodeCount; node++)
            {
                parent[node] = node;
            }
            var nodeA = _cellCount;
            var nodeB = _cellCount + 1;

            int Find(int node)
            {
                while (parent[node] != node)
                {
                    parent[node] = parent[parent[node]];
                    node = parent[node];
                }
                return node;
            }

            bool Union(int first, int second)
            {
                var rootFirst = Find(first);
                var rootSecond = Find(second);
                if (rootFirst == rootSecond)
                {
                    return false;
                }
                parent[rootSecond] = rootFirst;
                return true;
            }

            for (var y = 0; y < _grid.Height; y++)
            {
                for (var x = 0; x < _grid.Width; x++)
                {
                    var coordinate = new CellCoordinate(x, y);
                    var index = _grid.IndexOf(coordinate);
                    var domain = _domains[index];
                    if (domain == CellDomain.EmptyBit || !CellDomain.IsSingleton(domain))
                    {
                        continue;
                    }
                    var shape = CellDomain.SingletonShape(domain);
                    var (first, second) = TrackShapeGeometry.Ports(shape);
                    foreach (var port in new[] { first, second })
                    {
                        var neighbor = coordinate.Neighbor(port);
                        if (!_grid.Contains(neighbor))
                        {
                            if (_definition.TryGetEndpointAt(coordinate, port, out var endpoint))
                            {
                                var endpointNode = endpoint == _definition.A ? nodeA : nodeB;
                                if (!Union(index, endpointNode) && !IsCompletedLoop(nodeA, nodeB, Find))
                                {
                                    return false;
                                }
                            }
                            continue;
                        }

                        if (coordinate.CompareTo(neighbor) >= 0)
                        {
                            continue;
                        }
                        var neighborDomain = _domains[_grid.IndexOf(neighbor)];
                        if (neighborDomain == CellDomain.EmptyBit || !CellDomain.IsSingleton(neighborDomain))
                        {
                            continue;
                        }
                        if (!CellDomain.AllowsTrackWithPort(neighborDomain, DirectionGeometry.Opposite(port)))
                        {
                            continue;
                        }
                        if (!Union(index, _grid.IndexOf(neighbor)) && !IsCompletedLoop(nodeA, nodeB, Find))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Eine geschlossene Runde ist nur dann zulässig, wenn sie die
        /// vollständige Lösung ist: beide Endpoints enthalten und alle
        /// Randzahlen bereits exakt durch zugewiesene Gleise erfüllt.
        /// </summary>
        private bool IsCompletedLoop(int nodeA, int nodeB, Func<int, int> find)
        {
            if (find(nodeA) != find(nodeB))
            {
                return false;
            }
            for (var y = 0; y < _grid.Height; y++)
            {
                var assigned = 0;
                for (var x = 0; x < _grid.Width; x++)
                {
                    var domain = _domains[_grid.IndexOf(new CellCoordinate(x, y))];
                    if (domain != CellDomain.EmptyBit && CellDomain.IsSingleton(domain))
                    {
                        assigned++;
                    }
                }
                if (assigned != _definition.RowCounts[y])
                {
                    return false;
                }
            }
            for (var x = 0; x < _grid.Width; x++)
            {
                var assigned = 0;
                for (var y = 0; y < _grid.Height; y++)
                {
                    var domain = _domains[_grid.IndexOf(new CellCoordinate(x, y))];
                    if (domain != CellDomain.EmptyBit && CellDomain.IsSingleton(domain))
                    {
                        assigned++;
                    }
                }
                if (assigned != _definition.ColumnCounts[x])
                {
                    return false;
                }
            }
            return true;
        }

        private bool ApplyReachabilityRule()
        {
            var start = _definition.A.AdjacentCell(_grid);
            var target = _definition.B.AdjacentCell(_grid);
            var startIndex = _grid.IndexOf(start);
            var targetIndex = _grid.IndexOf(target);

            if (!CellDomain.AllowsTrackWithPort(_domains[startIndex], _definition.A.Side)
                || !CellDomain.AllowsTrackWithPort(_domains[targetIndex], _definition.B.Side))
            {
                return false;
            }
            if (start == target)
            {
                return true;
            }

            var visited = new bool[_cellCount];
            var queue = new Queue<int>();
            visited[startIndex] = true;
            queue.Enqueue(startIndex);
            while (queue.Count > 0)
            {
                var index = queue.Dequeue();
                if (index == targetIndex)
                {
                    return true;
                }
                var coordinate = new CellCoordinate(index % _grid.Width, index / _grid.Width);
                foreach (var direction in new[] { Direction.N, Direction.E, Direction.S, Direction.W })
                {
                    var neighbor = coordinate.Neighbor(direction);
                    if (!_grid.Contains(neighbor))
                    {
                        continue;
                    }
                    var neighborIndex = _grid.IndexOf(neighbor);
                    if (visited[neighborIndex])
                    {
                        continue;
                    }
                    if (!CellDomain.AllowsTrackWithPort(_domains[index], direction)
                        || !CellDomain.AllowsTrackWithPort(_domains[neighborIndex], DirectionGeometry.Opposite(direction)))
                    {
                        continue;
                    }
                    visited[neighborIndex] = true;
                    queue.Enqueue(neighborIndex);
                }
            }
            return false;
        }

        private bool Reduce(int cellIndex, byte reduced, byte rule, int premiseDepth)
        {
            var current = _domains[cellIndex];
            if (reduced == current)
            {
                return true;
            }
            if (CellDomain.IsEmpty(reduced))
            {
                return false;
            }
            _domains[cellIndex] = reduced;
            var depth = premiseDepth + 1;
            _depths[cellIndex] = depth;
            if (depth > _depthTracker.Max)
            {
                _depthTracker.Max = depth;
            }
            _anyReduction = true;
            var key = ((long)rule << 48) | ((long)cellIndex << 8) | reduced;
            if (_seenReductions.Add(key))
            {
                _counter.Steps++;
            }
            return true;
        }
    }
}
