using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using STP.Puzzle.Domain;

namespace STP.Puzzle.Solver
{
    /// <summary>
    /// Records the root propagation (before the first DFS assumption) as an auditable solver-v2 trace. Each entry is
    /// the atomic fact "cell cannot take the removed values". Its premises are minimal in the sense of irredundant
    /// (ADR-031 section 3): no declared public fact and no referenced earlier entry can be left out without the rule
    /// losing the derivation. A published step is sound for every valid puzzle that agrees with it on its declared
    /// public facts and contains the cells it names; a public fact is declared exactly when some valid puzzle that
    /// differs only in that fact refutes the derivation (QC finding 1 and the second QC finding 2 of PR #17).
    /// DFS branches never reach this class.
    /// </summary>
    internal sealed class RootDeductionTracer
    {
        private sealed class Entry
        {
            public int Cell; public string Rule = ""; public byte Removed; public byte After; public string? Axis; public int Line; public int Depth;
            public int[] Premises = Array.Empty<int>();
            public int Facts; // bit set (FactGrid, FactA, FactB, FactLine) of the declared public facts
        }
        // Public fact bits of Entry.Facts, in the canonical declaration order GRID_SIZE, ENDPOINT_A, ENDPOINT_B, line count.
        private const int FactGrid = 1, FactA = 2, FactB = 4, FactLine = 8;
        // Fixed order in which public facts are tried for removal: the grid size first, then endpoint B before endpoint A
        // (where either endpoint alone suffices, A is kept), the line count last. Part of solver-v2, like the entry order.
        private static readonly int[] FactDeletionOrder = { FactGrid, FactB, FactA, FactLine };
        // GridSize bounds of the domain: another valid puzzle can only have a grid inside them.
        private const int MinGrid = 2, MaxGrid = 32;
        // Position of an endpoint relative to the traced cell: one of its sides (Direction) or elsewhere.
        private const int Elsewhere = 4;
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
            // One premise view per reduction. Dropping an entry can change only its own cell, so replay just that
            // cell's remaining candidates instead of allocating and rebuilding the entire grid for every trial.
            // The candidate order and newest-first deletion order remain exactly the solver-v2 contract.
            var view = View(included);
            bool derivable = Holds(entry, view);
            if (!derivable) Faulted = true;
            else
                for (int k = candidates.Count - 1; k >= 0; k--)
                {
                    int j = candidates[k]; included[j] = false;
                    int changedCell = entries[j].Cell;
                    byte saved = view[changedCell], remaining = 127;
                    foreach (int prior in candidates)
                        if (included[prior] && entries[prior].Cell == changedCell) remaining &= (byte)~entries[prior].Removed;
                    view[changedCell] = remaining;
                    if (!Holds(entry, view)) { included[j] = true; view[changedCell] = saved; }
                }
            var premises = new List<int>();
            int depth = 0;
            for (int j = 0; j < included.Length; j++) if (included[j]) { premises.Add(j); depth = Math.Max(depth, entries[j].Depth); }
            entry.Premises = premises.ToArray(); entry.Depth = depth + 1;
            // The public facts are minimised on exactly the chosen entries. An underivable step is a fault and never
            // published; for diagnostics it declares every fact its rule can consult.
            entry.Facts = derivable ? MinimalFacts(entry, view) : Candidates(rule);
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
        /// Re-verifies a published trace from its own data: every step must follow from exactly its declared premises under
        /// its rule, its declared public facts must be sufficient and each of them necessary, its depth must follow from
        /// its entries, and (optionally) no referenced earlier entry may be removable. Returns null when the trace is
        /// consistent, otherwise the first defect.
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
                bool lineRule = e.Rule.StartsWith("LINE_", StringComparison.Ordinal);
                if (e.Axis != null && (e.Line < 0 || e.Line >= (e.Axis == "ROW" ? definition.Grid.Height : definition.Grid.Width))) return "step " + i + ": line fact";
                // A line rule needs the row or column fact of the line through its own cell.
                if (lineRule && (e.Axis == null || e.Line != (e.Axis == "ROW" ? step.Cell.Y : step.Cell.X))) return "step " + i + ": line fact";
                var declaredFacts = step.Premises.Where(q => q.IsPublicFact).ToArray();
                var refs = step.Premises.Where(q => !q.IsPublicFact).Select(q => q.TraceIndex).ToArray();
                if (step.Premises.Take(declaredFacts.Length).Any(q => !q.IsPublicFact) || refs.Zip(refs.Skip(1), (x, y) => x >= y).Any(b => b) || refs.Any(r => r < 0 || r >= i)) return "step " + i + ": premise order or forward reference";
                int depth = 0; foreach (int r in refs) depth = Math.Max(depth, depths[r]);
                depths[i] = depth + 1;
                if (step.Depth != depths[i]) return "step " + i + ": depth " + step.Depth + " instead of " + depths[i];
                // Entries are replayed into the auditor so that premise views can be built from exactly their data.
                auditor.entries.Add(e);
                byte[] ViewOf(IEnumerable<int> set)
                {
                    var view = new byte[definition.Grid.CellCount];
                    for (int c = 0; c < view.Length; c++) view[c] = 127;
                    foreach (int r in set) view[auditor.entries[r].Cell] &= (byte)~auditor.entries[r].Removed;
                    return view;
                }
                var premiseView = ViewOf(refs);
                if (!auditor.Holds(e, premiseView)) return "step " + i + ": not derivable from its premises";
                // The declared public facts: canonical (each kind once, in order), only facts the rule can consult at all,
                // sufficient together, and none of them dispensable.
                int declared = 0;
                foreach (var q in declaredFacts) declared |= Bit(q.PublicFact!.Value);
                e.Facts = declared;
                var canonical = auditor.FactsOf(e).ToArray();
                if (canonical.Length != declaredFacts.Length || canonical.Where((q, k) => q.PublicFact != declaredFacts[k].PublicFact || q.LineIndex != declaredFacts[k].LineIndex).Any()) return "step " + i + ": public facts not in canonical form";
                if ((declared & ~Candidates(e.Rule)) != 0) return "step " + i + ": public facts include a fact the rule never consults";
                if (!auditor.Sufficient(e, premiseView, declared)) return "step " + i + ": public facts are insufficient";
                foreach (int fact in FactDeletionOrder)
                    if ((declared & fact) != 0 && auditor.Sufficient(e, premiseView, declared & ~fact)) return "step " + i + ": public facts: " + Name(fact) + " is redundant";
                if (requireIrredundant)
                    foreach (int dropped in refs)
                        if (auditor.Sufficient(e, ViewOf(refs.Where(r => r != dropped)), declared)) return "step " + i + ": premise #" + dropped + " is redundant";
            }
            return null;
        }

        private static int Bit(PublicFactKind kind) => kind == PublicFactKind.GRID_SIZE ? FactGrid : kind == PublicFactKind.ENDPOINT_A ? FactA : kind == PublicFactKind.ENDPOINT_B ? FactB : FactLine;
        private static string Name(int fact) => fact == FactGrid ? "GRID_SIZE" : fact == FactA ? "ENDPOINT_A" : fact == FactB ? "ENDPOINT_B" : "line count";

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

        // The public facts a rule can consult at all. Neighbor and loop rules only compare cells that the premises name.
        private static int Candidates(string rule)
        {
            switch (rule)
            {
                case "BORDER_PORT_FORBIDDEN":
                case "ENDPOINT_ENTRY_REQUIRED":
                case "LOCAL_DEGREE_REQUIRED":
                case "CONNECTIVITY_PRESERVATION": return FactGrid | FactA | FactB;
                case "LINE_ALL_REMAINING_OCCUPIED": return FactGrid | FactLine;
                case "LINE_ZERO":
                case "LINE_TARGET_REACHED": return FactLine;
                default: return 0;
            }
        }

        // Irredundant public facts on a fixed view: start with every fact the rule can consult and drop each one, in the
        // fixed deletion order, whose absence keeps the derivation valid. Sufficiency is monotone in the facts, so every
        // fact that survives the single pass is necessary for the final set.
        private int MinimalFacts(Entry e, byte[] view)
        {
            int facts = Candidates(e.Rule);
            foreach (int fact in FactDeletionOrder)
                if ((facts & fact) != 0 && Sufficient(e, view, facts & ~fact)) facts &= ~fact;
            return facts;
        }

        // True when the entry's rule, applied to the view of this puzzle alone, rejects every value the entry removed.
        private bool Holds(Entry e, byte[] view)
        {
            switch (e.Rule)
            {
                case "BORDER_PORT_FORBIDDEN":
                case "ENDPOINT_ENTRY_REQUIRED":
                    return Local(e, view, FactGrid | FactA | FactB, false);
                case "LOCAL_DEGREE_REQUIRED":
                    return Local(e, view, FactGrid | FactA | FactB, true);
                case "NEIGHBOR_PORT_REQUIRED":
                    return All(e, v => NeighborRejects(view, e.Cell, v));
                case "LINE_ALL_REMAINING_OCCUPIED":
                {
                    int max = 0, target = e.Axis == "ROW" ? p.RowCounts[e.Line] : p.ColumnCounts[e.Line];
                    foreach (int cell in LineCells(e)) if ((view[cell] & 126) != 0) max++;
                    return LineCells(e).Contains(e.Cell) && max == target && (view[e.Cell] & 126) != 0 && All(e, v => v == 0);
                }
                case "LINE_ZERO":
                case "LINE_TARGET_REACHED":
                {
                    int min = 0, target = e.Axis == "ROW" ? p.RowCounts[e.Line] : p.ColumnCounts[e.Line];
                    foreach (int cell in LineCells(e)) if ((view[cell] & 1) == 0) min++;
                    return LineCells(e).Contains(e.Cell) && min == target && (view[e.Cell] & 1) != 0 && (e.Rule == "LINE_ZERO") == (target == 0) && All(e, v => v != 0);
                }
                case "LOOP_PREVENTION":
                    return LoopRejects(e, view);
                case "CONNECTIVITY_PRESERVATION":
                    return ConnectivityRejects(e, view);
                default:
                    return false;
            }
        }

        // True when the rule derives the entry from the view for EVERY valid puzzle that agrees with this one on the given
        // public facts and contains every cell the entry or its premises name (other grid size within 2..32, other endpoint
        // positions, other line counts). The check is exact for each rule; see SOLVER_ARCHITECTURE.md 13.3.
        private bool Sufficient(Entry e, byte[] view, int facts)
        {
            if (!Holds(e, view)) return false;
            switch (e.Rule)
            {
                case "BORDER_PORT_FORBIDDEN":
                case "ENDPOINT_ENTRY_REQUIRED":
                    return Local(e, view, facts, false);
                case "LOCAL_DEGREE_REQUIRED":
                    return Local(e, view, facts, true);
                case "NEIGHBOR_PORT_REQUIRED":
                case "LOOP_PREVENTION":
                    // Only cells named by the premises can contradict a port or close a chain, and they keep their
                    // coordinates and neighbors in every puzzle that contains them: no public fact is consulted.
                    return true;
                case "LINE_ZERO":
                case "LINE_TARGET_REACHED":
                    // The count of cells already known to be occupied does not depend on the line length; another
                    // count (one above that number, at most the number of cells that can still be occupied) breaks it.
                    return (facts & FactLine) != 0;
                case "LINE_ALL_REMAINING_OCCUPIED":
                    // A longer line adds unnamed cells that can be occupied, so the line length is needed, unless the line
                    // already has the maximum length 32: every shorter line drops such a cell and contradicts the count.
                    return (facts & FactLine) != 0 && ((facts & FactGrid) != 0 || (e.Axis == "ROW" ? p.Grid.Width : p.Grid.Height) == MaxGrid);
                case "CONNECTIVITY_PRESERVATION":
                    return ConnectivitySufficient(e, view, facts);
                default:
                    return false;
            }
        }

        private static bool All(Entry e, Func<int, bool> rejects)
        {
            for (int v = 0; v < 7; v++) if ((e.Removed & (1 << v)) != 0 && !rejects(v)) return false;
            return true;
        }

        private bool NeighborRejects(byte[] view, int cell, int value)
        {
            for (int d = 0; d < 4; d++)
            {
                int n = PuzzleSolver.Neighbor(p, cell, d);
                if (n >= 0 && !PuzzleSolver.Has(view[n], (d + 2) % 4, (PuzzleSolver.Ports(value) & (1 << d)) != 0)) return true;
            }
            return false;
        }

        // Smallest grid extent every agreeing puzzle must have: all named cells and, if declared, the endpoint indices.
        private void Extent(byte[] view, int cell, int facts, out int needW, out int needH)
        {
            int w = p.Grid.Width;
            needW = cell % w + 1; needH = cell / w + 1;
            for (int i = 0; i < view.Length; i++) if (view[i] != 127) { needW = Math.Max(needW, i % w + 1); needH = Math.Max(needH, i / w + 1); }
            void Fit(Endpoint endpoint, ref int width, ref int height)
            {
                if (endpoint.Side == Direction.N || endpoint.Side == Direction.S) width = Math.Max(width, endpoint.Index + 1);
                else height = Math.Max(height, endpoint.Index + 1);
            }
            if ((facts & FactA) != 0) Fit(p.A, ref needW, ref needH);
            if ((facts & FactB) != 0) Fit(p.B, ref needW, ref needH);
        }

        // Whether the far border (E for x, S for y) can lie directly behind the coordinate in some agreeing puzzle.
        private static IEnumerable<bool> FarBorder(bool sizeKnown, int coordinate, int size, int need)
        {
            if (sizeKnown) { yield return coordinate == size - 1; yield break; }
            if (coordinate + 1 >= Math.Max(MinGrid, need) && coordinate + 1 <= MaxGrid) yield return true;
            if (Math.Max(Math.Max(MinGrid, need), coordinate + 2) <= MaxGrid) yield return false;
        }

        // Where a declared endpoint lies relative to the cell (x, y) in a puzzle with the given far borders.
        private static int Position(Endpoint endpoint, int x, int y, bool east, bool south)
        {
            switch (endpoint.Side)
            {
                case Direction.N: return y == 0 && endpoint.Index == x ? (int)Direction.N : Elsewhere;
                case Direction.W: return x == 0 && endpoint.Index == y ? (int)Direction.W : Elsewhere;
                case Direction.E: return east && endpoint.Index == y ? (int)Direction.E : Elsewhere;
                default: return south && endpoint.Index == x ? (int)Direction.S : Elsewhere;
            }
        }

        // The border and degree rules only depend on the cell's own exterior sides, the endpoints on them and the named
        // neighbors. Every agreeing puzzle falls into one of these finitely many local situations (far borders present or
        // not, each undeclared endpoint on one of the exterior sides or elsewhere); the step holds iff every situation
        // rejects every removed value. With all facts declared exactly this puzzle's situation remains.
        private bool Local(Entry e, byte[] view, int facts, bool degree)
        {
            int w = p.Grid.Width, h = p.Grid.Height, x = e.Cell % w, y = e.Cell / w;
            bool gridKnown = (facts & FactGrid) != 0;
            Extent(view, e.Cell, facts, out int needW, out int needH);
            foreach (bool east in FarBorder(gridKnown, x, w, needW))
                foreach (bool south in FarBorder(gridKnown, y, h, needH))
                {
                    bool Border(int d) => d == 0 ? y == 0 : d == 1 ? east : d == 2 ? south : x == 0;
                    var open = Enumerable.Range(0, 4).Where(Border).Append(Elsewhere).ToArray();
                    var aOptions = (facts & FactA) != 0 ? new[] { Position(p.A, x, y, east, south) } : open;
                    var bOptions = (facts & FactB) != 0 ? new[] { Position(p.B, x, y, east, south) } : open;
                    foreach (int a in aOptions)
                        foreach (int b in bOptions)
                        {
                            if (a == b && a != Elsewhere) continue; // two endpoints never share an exterior port
                            bool Port(int d) => a == d || b == d;
                            for (int v = 0; v < 7; v++)
                            {
                                if ((e.Removed & (1 << v)) == 0) continue;
                                int ports = PuzzleSolver.Ports(v); bool rejected = false;
                                if (!degree)
                                {
                                    // Stage 0: an endpoint cell is never empty, and an exterior side carries a port iff it is an endpoint port.
                                    if (v == 0 && (a != Elsewhere || b != Elsewhere)) rejected = true;
                                    for (int d = 0; d < 4; d++) if (Border(d) && ((ports & (1 << d)) != 0) != Port(d)) rejected = true;
                                }
                                else if (v != 0)
                                {
                                    // Stage 3: a port is supported by an endpoint port outside or a neighbor that can answer it.
                                    for (int d = 0; d < 4; d++)
                                    {
                                        if ((ports & (1 << d)) == 0) continue;
                                        bool supported;
                                        if (Border(d)) supported = Port(d);
                                        else
                                        {
                                            int nx = x + TrackGeometry.DeltaX((Direction)d), ny = y + TrackGeometry.DeltaY((Direction)d);
                                            supported = nx >= w || ny >= h || PuzzleSolver.Has(view[ny * w + nx], (d + 2) % 4, true);
                                        }
                                        if (!supported) rejected = true;
                                    }
                                }
                                if (!rejected) return false;
                            }
                        }
                }
            return true;
        }

        private bool LoopRejects(Entry e, byte[] view)
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

        // Connectivity starts at endpoint A, requires endpoint B to be reached and empties every unreached cell.
        private bool ConnectivityRejects(Entry e, byte[] view)
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

        // In an agreeing puzzle the connectivity stage keeps the cell's tracks exactly when A and B sit in the cell's
        // component of possible connections and every cell that must be occupied lies in it too (otherwise the stage
        // contradicts, which proves nothing). Grid sizes beyond one more column and row than this puzzle add only free
        // cells behind the band of that extra column and row and behave alike, so the sizes up to +1 decide the check.
        private bool ConnectivitySufficient(Entry e, byte[] view, int facts)
        {
            bool gridKnown = (facts & FactGrid) != 0;
            Extent(view, e.Cell, facts, out int needW, out int needH);
            int w = p.Grid.Width, h = p.Grid.Height;
            int wMax = gridKnown ? w : Math.Min(MaxGrid, w + 1), wMin = gridKnown ? w : Math.Max(MinGrid, needW);
            int hMax = gridKnown ? h : Math.Min(MaxGrid, h + 1), hMin = gridKnown ? h : Math.Max(MinGrid, needH);
            for (int qw = wMax; qw >= wMin; qw--)
                for (int qh = hMax; qh >= hMin; qh--)
                    if (Reaches(e, view, facts, qw, qh)) return false;
            return true;
        }

        private bool Reaches(Entry e, byte[] view, int facts, int qw, int qh)
        {
            int w = p.Grid.Width, h = p.Grid.Height;
            byte At(int x, int y) => x < w && y < h ? view[y * w + x] : (byte)127;
            var parents = Enumerable.Range(0, qw * qh).ToArray();
            int Root(int cell) { while (parents[cell] != cell) cell = parents[cell] = parents[parents[cell]]; return cell; }
            void Join(int a, int b) { a = Root(a); b = Root(b); if (a != b) parents[a] = b; }
            for (int y = 0; y < qh; y++)
                for (int x = 0; x < qw; x++)
                {
                    if (x + 1 < qw && PuzzleSolver.Has(At(x, y), (int)Direction.E, true) && PuzzleSolver.Has(At(x + 1, y), (int)Direction.W, true)) Join(y * qw + x, y * qw + x + 1);
                    if (y + 1 < qh && PuzzleSolver.Has(At(x, y), (int)Direction.S, true) && PuzzleSolver.Has(At(x, y + 1), (int)Direction.N, true)) Join(y * qw + x, (y + 1) * qw + x);
                }
            int component = Root(e.Cell / w * qw + e.Cell % w);
            for (int i = 0; i < view.Length; i++)
                if (view[i] != 127 && (view[i] & 1) == 0 && Root(i / w * qw + i % w) != component) return false;
            bool aKnown = (facts & FactA) != 0, bKnown = (facts & FactB) != 0, aIn = !aKnown, bIn = !bKnown; int free = 0;
            for (int side = 0; side < 4; side++)
                for (int index = 0; index < (side % 2 == 0 ? qw : qh); index++)
                {
                    int x = side == 1 ? qw - 1 : side == 3 ? 0 : index, y = side == 0 ? 0 : side == 2 ? qh - 1 : index;
                    if (Root(y * qw + x) != component) continue;
                    if (aKnown && (int)p.A.Side == side && p.A.Index == index) aIn = true;
                    else if (bKnown && (int)p.B.Side == side && p.B.Index == index) bIn = true;
                    else free++;
                }
            return aIn && bIn && free >= (aKnown ? 0 : 1) + (bKnown ? 0 : 1);
        }
    }
}
