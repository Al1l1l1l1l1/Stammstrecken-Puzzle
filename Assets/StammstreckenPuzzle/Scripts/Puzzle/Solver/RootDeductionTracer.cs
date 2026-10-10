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
    /// Its premises are the public facts of its rule plus an irredundant set of earlier
    /// entries from which the rule alone re-derives the removal. DFS branches never reach this class.
    /// </summary>
    internal sealed class RootDeductionTracer
    {
        private sealed class Entry
        {
            public int Cell; public string Rule = ""; public byte Removed; public byte After; public string? Axis; public int Line; public int Depth;
            public int[] Premises = Array.Empty<int>();
        }
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
                var expectedFacts = auditor.FactsOf(e).ToArray();
                var declaredFacts = step.Premises.Where(q => q.IsPublicFact).ToArray();
                if (expectedFacts.Length != declaredFacts.Length || expectedFacts.Where((q, k) => q.PublicFact != declaredFacts[k].PublicFact || q.LineIndex != declaredFacts[k].LineIndex).Any()) return "step " + i + ": public facts";
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
                if (!auditor.Holds(e, ViewOf(refs))) return "step " + i + ": not derivable from its premises";
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
            switch (e.Rule)
            {
                case "BORDER_PORT_FORBIDDEN":
                case "ENDPOINT_ENTRY_REQUIRED":
                case "LOCAL_DEGREE_REQUIRED":
                    yield return DeductionPremise.Fact(PublicFactKind.GRID_SIZE);
                    if (e.Cell == p.Index(p.A.Cell)) yield return DeductionPremise.Fact(PublicFactKind.ENDPOINT_A);
                    if (e.Cell == p.Index(p.B.Cell)) yield return DeductionPremise.Fact(PublicFactKind.ENDPOINT_B);
                    break;
                case "NEIGHBOR_PORT_REQUIRED":
                case "LOOP_PREVENTION":
                    yield return DeductionPremise.Fact(PublicFactKind.GRID_SIZE);
                    break;
                case "CONNECTIVITY_PRESERVATION":
                    yield return DeductionPremise.Fact(PublicFactKind.GRID_SIZE);
                    yield return DeductionPremise.Fact(PublicFactKind.ENDPOINT_A);
                    yield return DeductionPremise.Fact(PublicFactKind.ENDPOINT_B);
                    break;
                case "LINE_ZERO":
                case "LINE_TARGET_REACHED":
                case "LINE_ALL_REMAINING_OCCUPIED":
                    yield return DeductionPremise.Fact(e.Axis == "ROW" ? PublicFactKind.ROW_COUNT : PublicFactKind.COLUMN_COUNT, e.Line);
                    break;
            }
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
        private bool Holds(Entry e, byte[] view)
        {
            switch (e.Rule)
            {
                case "BORDER_PORT_FORBIDDEN":
                case "ENDPOINT_ENTRY_REQUIRED":
                    return All(e, v => RejectsBorder(e.Cell, v));
                case "NEIGHBOR_PORT_REQUIRED":
                    return All(e, v => RejectsNeighbor(view, e.Cell, v));
                case "LOCAL_DEGREE_REQUIRED":
                    return All(e, v => RejectsDegree(view, e.Cell, v));
                case "LINE_ALL_REMAINING_OCCUPIED":
                {
                    int max = 0, target = e.Axis == "ROW" ? p.RowCounts[e.Line] : p.ColumnCounts[e.Line];
                    foreach (int cell in LineCells(e)) if ((view[cell] & 126) != 0) max++;
                    return max == target && (view[e.Cell] & 126) != 0 && All(e, v => v == 0);
                }
                case "LINE_ZERO":
                case "LINE_TARGET_REACHED":
                {
                    int min = 0, target = e.Axis == "ROW" ? p.RowCounts[e.Line] : p.ColumnCounts[e.Line];
                    foreach (int cell in LineCells(e)) if ((view[cell] & 1) == 0) min++;
                    return min == target && (view[e.Cell] & 1) != 0 && (e.Rule == "LINE_ZERO") == (target == 0) && All(e, v => v != 0);
                }
                case "LOOP_PREVENTION":
                    return LoopHolds(e, view);
                case "CONNECTIVITY_PRESERVATION":
                    return ConnectivityHolds(e, view);
                default:
                    return false;
            }
        }

        private static bool All(Entry e, Func<int, bool> rejects)
        {
            for (int v = 0; v < 7; v++) if ((e.Removed & (1 << v)) != 0 && !rejects(v)) return false;
            return true;
        }

        private bool RejectsBorder(int cell, int value)
        {
            int x = cell % p.Grid.Width, y = cell / p.Grid.Width;
            bool endpoint = cell == p.Index(p.A.Cell) || cell == p.Index(p.B.Cell);
            if (value == 0 && endpoint) return true;
            for (int d = 0; d < 4; d++)
                if (PuzzleSolver.Neighbor(p, cell, d) == -1 && ((PuzzleSolver.Ports(value) & (1 << d)) != 0) != p.IsEndpointPort(x, y, (Direction)d)) return true;
            return false;
        }

        private bool RejectsNeighbor(byte[] view, int cell, int value)
        {
            for (int d = 0; d < 4; d++)
            {
                int n = PuzzleSolver.Neighbor(p, cell, d);
                if (n >= 0 && !PuzzleSolver.Has(view[n], (d + 2) % 4, (PuzzleSolver.Ports(value) & (1 << d)) != 0)) return true;
            }
            return false;
        }

        private bool RejectsDegree(byte[] view, int cell, int value)
        {
            if (value == 0) return false;
            int supported = 0;
            for (int d = 0; d < 4; d++)
            {
                if ((PuzzleSolver.Ports(value) & (1 << d)) == 0) continue;
                int n = PuzzleSolver.Neighbor(p, cell, d);
                if (n >= 0) { if (PuzzleSolver.Has(view[n], (d + 2) % 4, true)) supported++; }
                else if (p.IsEndpointPort(cell % p.Grid.Width, cell / p.Grid.Width, (Direction)d)) supported++;
            }
            return supported != 2;
        }

        private bool LoopHolds(Entry e, byte[] view)
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
                    if (left == right) return false;
                    parents[left] = right;
                }
            return All(e, value =>
            {
                if (value == 0) return false;
                var adjacent = new List<int>(2);
                for (int d = 0; d < 4; d++) if ((PuzzleSolver.Ports(value) & (1 << d)) != 0)
                {
                    int n = PuzzleSolver.Neighbor(p, e.Cell, d); if (n >= 0 && !PuzzleSolver.Has(view[n], (d + 2) % 4, false)) adjacent.Add(n);
                }
                return adjacent.Count == 2 && Root(adjacent[0]) == Root(adjacent[1]) && Root(e.Cell) != Root(adjacent[0]);
            });
        }

        private bool ConnectivityHolds(Entry e, byte[] view)
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
            return reached[p.Index(p.B.Cell)] && !reached[e.Cell] && All(e, v => v != 0);
        }
    }
}
