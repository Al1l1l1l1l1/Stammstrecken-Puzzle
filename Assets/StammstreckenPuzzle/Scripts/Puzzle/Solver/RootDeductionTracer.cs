using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using STP.Puzzle.Domain;

namespace STP.Puzzle.Solver
{
    /// <summary>
    /// Records the root propagation (before the first DFS assumption) as an auditable
    /// solver-v2 trace. Each entry is the atomic fact "cell cannot take the removed values".
    /// Its premises are the public facts its derivation consults plus an irredundant set of earlier
    /// entries from which the rule alone re-derives the removal. DFS branches never reach this class.
    /// A published step is sound for every valid puzzle that agrees with it on its declared public facts:
    /// the derivation is a deterministic function of exactly the facts it reads, so a fact it does not
    /// read cannot influence the conclusion, and a fact it reads is always declared (QC finding 1 of PR #17).
    /// </summary>
    internal sealed class RootDeductionTracer
    {
        private sealed class Entry
        {
            public int Cell; public string Rule = ""; public byte Removed; public byte After; public string? Axis; public int Line; public int Depth;
            public int[] Premises = Array.Empty<int>();
            public int Facts; // bit set (FactGrid, FactA, FactB, FactLine) of the public facts the derivation consulted
        }
        // Public fact bits of Entry.Facts, in the canonical declaration order GRID_SIZE, ENDPOINT_A, ENDPOINT_B, line count.
        private const int FactGrid = 1, FactA = 2, FactB = 4, FactLine = 8;
        private readonly PuzzleDefinition p;
        private readonly List<Entry> entries = new List<Entry>();
        private readonly HashSet<string> keys = new HashSet<string>(StringComparer.Ordinal);
        /// <summary>True when a recorded reduction could not be re-derived from its rule; such a trace is never published.</summary>
        internal bool Faulted { get; private set; }
        internal int Count => entries.Count;
        internal RootDeductionTracer(PuzzleDefinition definition) { p = definition; }

        /// <summary>Records one effective reduction (before != after, after non-empty). Identical reductions are ignored.</summary>
        internal void Record(string rule, int cell, byte before, byte after, string? axis, int line)
        {
            byte removed = (byte)(before & ~after);
            if (removed == 0 || after == 0) return;
            string key = string.Join(":", rule, cell.ToString(CultureInfo.InvariantCulture), removed.ToString(CultureInfo.InvariantCulture), axis ?? "", line.ToString(CultureInfo.InvariantCulture));
            if (!keys.Add(key)) return;
            var entry = new Entry { Cell = cell, Rule = rule, Removed = removed, After = after, Axis = axis, Line = line };
            var candidates = new List<int>();
            bool[]? readSet = ReadSet(entry);
            for (int j = 0; j < entries.Count; j++) if (readSet == null || readSet[entries[j].Cell]) candidates.Add(j);
            var included = new bool[entries.Count];
            foreach (int j in candidates) included[j] = true;
            if (!Holds(entry, View(included))) Faulted = true;
            else
                for (int k = candidates.Count - 1; k >= 0; k--)
                {
                    int j = candidates[k]; included[j] = false;
                    if (!Holds(entry, View(included))) included[j] = true;
                }
            var premises = new List<int>();
            int depth = 0;
            for (int j = 0; j < included.Length; j++) if (included[j]) { premises.Add(j); depth = Math.Max(depth, entries[j].Depth); }
            entry.Premises = premises.ToArray(); entry.Depth = depth + 1;
            // The facts are what the derivation reads on exactly the chosen premises. An underivable step is a fault and
            // never published; for diagnostics it declares every fact its rule can consult.
            int consulted = Derive(entry, View(included));
            entry.Facts = consulted >= 0 ? consulted : AllFacts(rule);
            entries.Add(entry);
        }

        internal IReadOnlyList<DeductionStep> Build()
        {
            var steps = new DeductionStep[entries.Count];
            for (int i = 0; i < steps.Length; i++)
            {
                var e = entries[i];
                var premises = FactsOf(e).Concat(e.Premises.Select(DeductionPremise.Entry));
                steps[i] = new DeductionStep(i, e.Rule, CellOf(e.Cell), Values(e.Removed), Values(e.After), premises, e.Depth);
            }
            return Array.AsReadOnly(steps);
        }

        /// <summary>
        /// Re-verifies a published trace from its own data: every step must follow from exactly its declared
        /// premises under its rule, its public facts and depth must be those of the rule, and (optionally) no
        /// trace premise may be removable. Returns null when the trace is consistent, otherwise the first defect.
        /// </summary>
        internal static string? Audit(PuzzleDefinition definition, IReadOnlyList<DeductionStep> trace, bool requireIrredundant)
        {
            var auditor = new RootDeductionTracer(definition);
            var depths = new int[trace.Count];
            for (int i = 0; i < trace.Count; i++)
            {
                var step = trace[i];
                if (step.Index != i) return "step " + i + ": index " + step.Index;
                var e = new Entry { Cell = definition.Index(step.Cell), Rule = step.RuleCode, Axis = null, Line = -1 };
                foreach (var v in step.RemovedValues) e.Removed |= (byte)(1 << (int)v);
                foreach (var v in step.RemainingValues) e.After |= (byte)(1 << (int)v);
                if (e.Removed == 0 || e.After == 0 || (e.Removed & e.After) != 0) return "step " + i + ": removed/remaining values";
                foreach (var q in step.Premises.Where(q => q.IsPublicFact && (q.PublicFact == PublicFactKind.ROW_COUNT || q.PublicFact == PublicFactKind.COLUMN_COUNT)))
                { e.Axis = q.PublicFact == PublicFactKind.ROW_COUNT ? "ROW" : "COLUMN"; e.Line = q.LineIndex; }
                if ((e.Rule.StartsWith("LINE_", StringComparison.Ordinal)) && (e.Axis == null || e.Line < 0 || e.Line >= (e.Axis == "ROW" ? definition.Grid.Height : definition.Grid.Width))) return "step " + i + ": line fact";
                var declaredFacts = step.Premises.Where(q => q.IsPublicFact).ToArray();
                var refs = step.Premises.Where(q => !q.IsPublicFact).Select(q => q.TraceIndex).ToArray();
                if (step.Premises.Take(declaredFacts.Length).Any(q => !q.IsPublicFact) || refs.Zip(refs.Skip(1), (x, y) => x >= y).Any(b => b) || refs.Any(r => r < 0 || r >= i)) return "step " + i + ": premise order or forward reference";
                int depth = 0; foreach (int r in refs) depth = Math.Max(depth, depths[r]);
                depths[i] = depth + 1;
                if (step.Depth != depths[i]) return "step " + i + ": depth " + step.Depth + " instead of " + depths[i];
                // Entries are replayed into the auditor so that Holds can build premise views from exactly their data.
                auditor.entries.Add(e);
                byte[] ViewOf(IEnumerable<int> set)
                {
                    var view = new byte[definition.Grid.CellCount];
                    for (int c = 0; c < view.Length; c++) view[c] = 127;
                    foreach (int r in set) view[auditor.entries[r].Cell] &= (byte)~auditor.entries[r].Removed;
                    return view;
                }
                int consulted = auditor.Derive(e, ViewOf(refs));
                if (consulted < 0) return "step " + i + ": not derivable from its premises";
                // The declared public facts must be exactly the facts the derivation reads: a missing one would let an
                // undeclared fact change the conclusion, an additional one is not a premise of the derivation.
                e.Facts = consulted;
                var expectedFacts = auditor.FactsOf(e).ToArray();
                if (expectedFacts.Length != declaredFacts.Length || expectedFacts.Where((q, k) => q.PublicFact != declaredFacts[k].PublicFact || q.LineIndex != declaredFacts[k].LineIndex).Any()) return "step " + i + ": public facts";
                if (requireIrredundant)
                    foreach (int dropped in refs)
                        if (auditor.Holds(e, ViewOf(refs.Where(r => r != dropped)))) return "step " + i + ": premise #" + dropped + " is redundant";
            }
            return null;
        }

        private CellCoordinate CellOf(int cell) => CellCoordinate.Create(p.Grid, cell % p.Grid.Width, cell / p.Grid.Width).Value!;
        private static IEnumerable<CandidateValue> Values(byte mask)
        {
            for (int v = 0; v < 7; v++) if ((mask & (1 << v)) != 0) yield return (CandidateValue)v;
        }

        private IEnumerable<DeductionPremise> FactsOf(Entry e)
        {
            if ((e.Facts & FactGrid) != 0) yield return DeductionPremise.Fact(PublicFactKind.GRID_SIZE);
            if ((e.Facts & FactA) != 0) yield return DeductionPremise.Fact(PublicFactKind.ENDPOINT_A);
            if ((e.Facts & FactB) != 0) yield return DeductionPremise.Fact(PublicFactKind.ENDPOINT_B);
            if ((e.Facts & FactLine) != 0) yield return DeductionPremise.Fact(e.Axis == "ROW" ? PublicFactKind.ROW_COUNT : PublicFactKind.COLUMN_COUNT, e.Line);
        }

        // The cells whose earlier entries the rule can read; null means the whole grid.
        private bool[]? ReadSet(Entry e)
        {
            var set = new bool[p.Grid.CellCount];
            switch (e.Rule)
            {
                case "BORDER_PORT_FORBIDDEN":
                case "ENDPOINT_ENTRY_REQUIRED":
                    return set;
                case "NEIGHBOR_PORT_REQUIRED":
                case "LOCAL_DEGREE_REQUIRED":
                    set[e.Cell] = true;
                    for (int d = 0; d < 4; d++) { int n = PuzzleSolver.Neighbor(p, e.Cell, d); if (n >= 0) set[n] = true; }
                    return set;
                case "LINE_ZERO":
                case "LINE_TARGET_REACHED":
                case "LINE_ALL_REMAINING_OCCUPIED":
                    foreach (int cell in LineCells(e)) set[cell] = true;
                    return set;
                default:
                    return null;
            }
        }

        private IEnumerable<int> LineCells(Entry e)
        {
            bool row = e.Axis == "ROW"; int length = row ? p.Grid.Width : p.Grid.Height;
            return Enumerable.Range(0, length).Select(i => row ? e.Line * p.Grid.Width + i : i * p.Grid.Width + e.Line);
        }

        // View of the grid that knows only the facts of the included entries: every other value stays possible.
        private byte[] View(bool[] included)
        {
            var view = new byte[p.Grid.CellCount];
            for (int i = 0; i < view.Length; i++) view[i] = 127;
            for (int j = 0; j < included.Length; j++) if (included[j]) view[entries[j].Cell] &= (byte)~entries[j].Removed;
            return view;
        }

        // True when the entry's rule, applied to the view alone, rejects every value the entry removed.
        private bool Holds(Entry e, byte[] view) => Derive(e, view) >= 0;

        private static int AllFacts(string rule)
        {
            switch (rule)
            {
                case "BORDER_PORT_FORBIDDEN":
                case "ENDPOINT_ENTRY_REQUIRED":
                case "LOCAL_DEGREE_REQUIRED":
                case "CONNECTIVITY_PRESERVATION": return FactGrid | FactA | FactB;
                case "NEIGHBOR_PORT_REQUIRED":
                case "LOOP_PREVENTION": return FactGrid;
                case "LINE_ALL_REMAINING_OCCUPIED": return FactGrid | FactLine;
                case "LINE_ZERO":
                case "LINE_TARGET_REACHED": return FactLine;
                default: return 0;
            }
        }

        // Re-derives the removal from the view alone. The result is -1 when the rule does not reject every removed value,
        // otherwise the bit set of the public facts that derivation consulted. A fact is consulted exactly when a different
        // value of it could change the outcome of the chosen derivation, so the conclusion holds for every valid puzzle that
        // agrees on these facts. The rule is evaluated as its stage defines it. Where a rule rejects a value by any one of
        // several violated conditions (border, neighbor), the violated condition consulting the fewest facts is named
        // (the first on a tie); a counting rule (degree) consults everything it counts.
        private int Derive(Entry e, byte[] view)
        {
            switch (e.Rule)
            {
                case "BORDER_PORT_FORBIDDEN":
                case "ENDPOINT_ENTRY_REQUIRED":
                    return Each(e, v => BorderReason(e.Cell, v));
                case "NEIGHBOR_PORT_REQUIRED":
                    return Each(e, v => NeighborReason(view, e.Cell, v));
                case "LOCAL_DEGREE_REQUIRED":
                    return Each(e, v => DegreeReason(view, e.Cell, v));
                case "LINE_ALL_REMAINING_OCCUPIED":
                {
                    // The number of cells that can still be occupied depends on the line length, hence on the grid size.
                    int max = 0, target = e.Axis == "ROW" ? p.RowCounts[e.Line] : p.ColumnCounts[e.Line];
                    foreach (int cell in LineCells(e)) if ((view[cell] & 126) != 0) max++;
                    return max == target && (view[e.Cell] & 126) != 0 ? Each(e, v => v == 0 ? FactGrid | FactLine : -1) : -1;
                }
                case "LINE_ZERO":
                case "LINE_TARGET_REACHED":
                {
                    // Only cells already known to be occupied are counted, so the length of the line does not matter.
                    int min = 0, target = e.Axis == "ROW" ? p.RowCounts[e.Line] : p.ColumnCounts[e.Line];
                    foreach (int cell in LineCells(e)) if ((view[cell] & 1) == 0) min++;
                    return min == target && (view[e.Cell] & 1) != 0 && (e.Rule == "LINE_ZERO") == (target == 0) ? Each(e, v => v != 0 ? FactLine : -1) : -1;
                }
                case "LOOP_PREVENTION":
                    return LoopReason(e, view);
                case "CONNECTIVITY_PRESERVATION":
                    return ConnectivityReason(e, view);
                default:
                    return -1;
            }
        }

        // Union of the facts of every removed value; -1 as soon as one removed value is not rejected.
        private static int Each(Entry e, Func<int, int> reason)
        {
            int facts = 0;
            for (int v = 0; v < 7; v++)
            {
                if ((e.Removed & (1 << v)) == 0) continue;
                int r = reason(v);
                if (r < 0) return -1;
                facts |= r;
            }
            return facts;
        }

        private static int FactCount(int facts) { int n = 0; for (; facts != 0; facts &= facts - 1) n++; return n; }
        private static int Cheaper(int best, int offer) => offer >= 0 && (best < 0 || FactCount(offer) < FactCount(best)) ? offer : best;

        private bool EndpointMatches(Endpoint endpoint, int cell, int direction) => endpoint.Side == (Direction)direction && p.Index(endpoint.Cell) == cell;

        // Public endpoint port of the puzzle at the exterior side of a cell. A matching endpoint settles it alone; a
        // negative answer needs both endpoints, because either could be moved onto the port.
        private bool EndpointPort(int cell, int direction, out int facts)
        {
            if (EndpointMatches(p.A, cell, direction)) { facts = FactA; return true; }
            if (EndpointMatches(p.B, cell, direction)) { facts = FactB; return true; }
            facts = FactA | FactB; return false;
        }

        // The border rule: the removed value either leaves an endpoint cell empty or disagrees with the exterior port
        // of a border side. Which cells and sides are border or endpoint is a property of the grid and the endpoints.
        private int BorderReason(int cell, int value)
        {
            int best = -1;
            if (value == 0)
            {
                if (cell == p.Index(p.A.Cell)) best = Cheaper(best, FactGrid | FactA);
                else if (cell == p.Index(p.B.Cell)) best = Cheaper(best, FactGrid | FactB);
            }
            for (int d = 0; d < 4; d++)
            {
                if (PuzzleSolver.Neighbor(p, cell, d) != -1) continue;
                bool has = (PuzzleSolver.Ports(value) & (1 << d)) != 0;
                if (has != EndpointPort(cell, d, out int facts)) best = Cheaper(best, FactGrid | facts);
            }
            return best;
        }

        private int NeighborReason(byte[] view, int cell, int value)
        {
            for (int d = 0; d < 4; d++)
            {
                int n = PuzzleSolver.Neighbor(p, cell, d);
                if (n >= 0 && !PuzzleSolver.Has(view[n], (d + 2) % 4, (PuzzleSolver.Ports(value) & (1 << d)) != 0)) return FactGrid;
            }
            return -1;
        }

        // The degree rule counts the supported ports of the value (stage 3), so it consults the support of every port:
        // the neighbor of an interior port, the public endpoint port of an exterior one.
        private int DegreeReason(byte[] view, int cell, int value)
        {
            if (value == 0) return -1;
            int supported = 0, facts = 0;
            for (int d = 0; d < 4; d++)
            {
                if ((PuzzleSolver.Ports(value) & (1 << d)) == 0) continue;
                int n = PuzzleSolver.Neighbor(p, cell, d); facts |= FactGrid;
                if (n >= 0) { if (PuzzleSolver.Has(view[n], (d + 2) % 4, true)) supported++; }
                else { if (EndpointPort(cell, d, out int portFacts)) supported++; facts |= portFacts; }
            }
            return supported != 2 ? facts : -1;
        }

        private int LoopReason(Entry e, byte[] view)
        {
            int count = view.Length;
            var parents = Enumerable.Range(0, count).ToArray();
            int Root(int cell) { while (parents[cell] != cell) cell = parents[cell]; return cell; }
            for (int cell = 0; cell < count; cell++)
                for (int d = 0; d < 4; d++)
                {
                    int neighbor = PuzzleSolver.Neighbor(p, cell, d);
                    if (neighbor <= cell || PuzzleSolver.Has(view[cell], d, false) || PuzzleSolver.Has(view[neighbor], (d + 2) % 4, false)) continue;
                    int left = Root(cell), right = Root(neighbor);
                    if (left == right) return -1;
                    parents[left] = right;
                }
            return Each(e, value =>
            {
                if (value == 0) return -1;
                var adjacent = new List<int>(2);
                for (int d = 0; d < 4; d++) if ((PuzzleSolver.Ports(value) & (1 << d)) != 0)
                {
                    int n = PuzzleSolver.Neighbor(p, e.Cell, d); if (n >= 0 && !PuzzleSolver.Has(view[n], (d + 2) % 4, false)) adjacent.Add(n);
                }
                return adjacent.Count == 2 && Root(adjacent[0]) == Root(adjacent[1]) && Root(e.Cell) != Root(adjacent[0]) ? FactGrid : -1;
            });
        }

        // Connectivity starts at endpoint A, requires endpoint B to be reached and walks the grid.
        private int ConnectivityReason(Entry e, byte[] view)
        {
            var reached = new bool[view.Length]; var queue = new Queue<int>(); int start = p.Index(p.A.Cell); reached[start] = true; queue.Enqueue(start);
            while (queue.Count > 0)
            {
                int cell = queue.Dequeue();
                for (int d = 0; d < 4; d++)
                {
                    int n = PuzzleSolver.Neighbor(p, cell, d);
                    if (n >= 0 && !reached[n] && PuzzleSolver.Has(view[cell], d, true) && PuzzleSolver.Has(view[n], (d + 2) % 4, true)) { reached[n] = true; queue.Enqueue(n); }
                }
            }
            return reached[p.Index(p.B.Cell)] && !reached[e.Cell] ? Each(e, v => v != 0 ? FactGrid | FactA | FactB : -1) : -1;
        }
    }
}
