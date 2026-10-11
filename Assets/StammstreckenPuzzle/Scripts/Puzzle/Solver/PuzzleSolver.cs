using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using STP.Puzzle.Domain;

namespace STP.Puzzle.Solver
{
    /// <summary>Complete search classification; limits never imply uniqueness.</summary>
    public enum SolverClassification { UNSATISFIABLE, UNIQUE, MULTIPLE_OR_MORE, INDETERMINATE }
    /// <summary>Explicit recovery search limits, with an optional injected monotonic millisecond source.</summary>
    public sealed class SolverLimits
    {
        public long MaxStates { get; }
        public long TimeBudgetMs { get; }
        public CancellationToken Cancellation { get; }
        internal Func<long> MonotonicNow { get; }
        public SolverLimits(long maxStates = 1000000, long timeBudgetMs = 2000, CancellationToken cancellation = default, Func<long>? monotonicNow = null)
        { MaxStates = maxStates; TimeBudgetMs = timeBudgetMs; Cancellation = cancellation; MonotonicNow = monotonicNow ?? (() => (long)(Stopwatch.GetTimestamp() * (1000.0 / Stopwatch.Frequency))); }
    }
    /// <summary>A recovery candidate reduction, not a proof, hint or auditable solver-v2 root trace.</summary>
    public sealed class RecoveryReduction
    {
        public string RuleCode { get; }
        public CellCoordinate Cell { get; }
        public byte Before { get; }
        public byte After { get; }
        public IReadOnlyList<CellCoordinate> ReferencedCells { get; }
        public string? LineAxis { get; }
        public int LineIndex { get; }
        internal RecoveryReduction(PuzzleDefinition p, string code, int cell, byte before, byte after, IEnumerable<int> references, string? axis, int line)
        {
            RuleCode = code; Cell = CellCoordinate.Create(p.Grid, cell % p.Grid.Width, cell / p.Grid.Width).Value!; Before = before; After = after;
            ReferencedCells = Array.AsReadOnly(references.Distinct().OrderBy(i => i).Select(i => CellCoordinate.Create(p.Grid, i % p.Grid.Width, i / p.Grid.Width).Value!).ToArray()); LineAxis = axis; LineIndex = line;
        }
        public override string ToString() => string.Join(":", RuleCode, Cell.Y.ToString(CultureInfo.InvariantCulture), Cell.X.ToString(CultureInfo.InvariantCulture), Before.ToString(CultureInfo.InvariantCulture), After.ToString(CultureInfo.InvariantCulture), LineAxis ?? "", LineIndex.ToString(CultureInfo.InvariantCulture), string.Join(",", ReferencedCells.Select(c => c.Y.ToString(CultureInfo.InvariantCulture) + "/" + c.X.ToString(CultureInfo.InvariantCulture))));
    }
    /// <summary>Bounded classification, first deterministic A-B path and, for UNIQUE only, the root deduction trace and ADR-031 metrics. No production proof is generated.</summary>
    public sealed class SolverResult
    {
        public string SolverVersion => PuzzleSolver.SolverVersion;
        public SolverClassification Classification { get; }
        public int SolutionCount { get; }
        public IReadOnlyList<CellCoordinate> Path { get; }
        /// <summary>Recovery protocol over root and search branches. Neither a proof nor the auditable root trace; use <see cref="DeductionTrace"/>.</summary>
        public IReadOnlyList<RecoveryReduction> Reductions { get; }
        /// <summary>Auditable root deductions in application order. Empty unless <see cref="Classification"/> is UNIQUE.</summary>
        public IReadOnlyList<DeductionStep> DeductionTrace { get; }
        /// <summary>The four ADR-031 metrics. Null unless <see cref="Classification"/> is UNIQUE; no metric is valid for any other result.</summary>
        public SolverMetrics? Metrics { get; }
        internal SolverResult(SolverClassification classification, int count, IEnumerable<CellCoordinate> path, IEnumerable<RecoveryReduction> reductions, SolverMetrics? metrics = null, IEnumerable<DeductionStep>? deductionTrace = null)
        {
            Classification = classification; SolutionCount = count; Path = Array.AsReadOnly(path.ToArray()); Reductions = Array.AsReadOnly(reductions.ToArray());
            Metrics = metrics; DeductionTrace = Array.AsReadOnly((deductionTrace ?? Array.Empty<DeductionStep>()).ToArray());
        }
    }
    /// <summary>Production solver-v2: sorted propagation, MRV y/x and EMPTY/NS/EW/NE/ES/SW/WN DFS with a root deduction trace and ADR-031 metrics.</summary>
    public static class PuzzleSolver
    {
        public const string SolverVersion = "solver-v2";
        internal static int Ports(int value) => value == 0 ? 0 : TrackGeometry.Ports((TrackShape)(value - 1));
        internal static bool Has(byte domain, int direction, bool port)
        {
            for (int value = 0; value < 7; value++) if ((domain & (1 << value)) != 0 && (((Ports(value) & (1 << direction)) != 0) == port)) return true;
            return false;
        }
        internal static int Neighbor(PuzzleDefinition p, int cell, int direction)
        {
            int x = cell % p.Grid.Width + TrackGeometry.DeltaX((Direction)direction), y = cell / p.Grid.Width + TrackGeometry.DeltaY((Direction)direction);
            return p.Grid.Contains(x, y) ? y * p.Grid.Width + x : -1;
        }
        private static bool Reduce(PuzzleDefinition p, byte[] domains, List<RecoveryReduction> trace, int cell, int allowed, string code, IEnumerable<int>? references = null, string? axis = null, int line = -1, RootDeductionTracer? tracer = null)
        {
            byte before = domains[cell], after = (byte)(before & allowed);
            if (before != after) { trace.Add(new RecoveryReduction(p, code, cell, before, after, references ?? new[] { cell }, axis, line)); tracer?.Record(code, cell, before, after, axis, line); domains[cell] = after; }
            return after != 0;
        }
        // Isolated stages are also exercised by contract/mutation tests. This is
        // an internal candidate model, not a public assumption or hint API.
        private static bool RunStage(PuzzleDefinition p, byte[] domains, List<RecoveryReduction> trace, int stage) => RunStageCore(p, domains, trace, stage, null);
        private static bool RunStageCore(PuzzleDefinition p, byte[] domains, List<RecoveryReduction> trace, int stage, RootDeductionTracer? tracer)
        {
            bool Reduce(int cell, int allowed, string code, IEnumerable<int>? references = null, string? axis = null, int line = -1) => PuzzleSolver.Reduce(p, domains, trace, cell, allowed, code, references, axis, line, tracer);
            if (stage == 0)
            {
                for (int cell = 0; cell < domains.Length; cell++)
                {
                    int x = cell % p.Grid.Width, y = cell / p.Grid.Width, allowed = 0; bool endpoint = cell == p.Index(p.A.Cell) || cell == p.Index(p.B.Cell);
                    for (int value = 0; value < 7; value++)
                    {
                        bool valid = value != 0 || !endpoint; int ports = Ports(value);
                        for (int d = 0; d < 4 && valid; d++)
                        {
                            bool has = (ports & (1 << d)) != 0;
                            if (Neighbor(p, cell, d) == -1 && has != p.IsEndpointPort(x, y, (Direction)d)) valid = false;
                        }
                        if (valid) allowed |= 1 << value;
                    }
                    if (!Reduce(cell, allowed, endpoint ? "ENDPOINT_ENTRY_REQUIRED" : "BORDER_PORT_FORBIDDEN")) return false;
                }
            }
            else if (stage == 1 || stage == 3)
            {
                for (int cell = 0; cell < domains.Length; cell++)
                {
                    int allowed = 0; var refs = new List<int> { cell };
                    for (int value = 0; value < 7; value++)
                    {
                        if ((domains[cell] & (1 << value)) == 0) continue;
                        bool valid = true; int supportedPorts = 0;
                        for (int d = 0; d < 4; d++)
                        {
                            int neighbor = Neighbor(p, cell, d); bool has = (Ports(value) & (1 << d)) != 0;
                            if (neighbor >= 0)
                            {
                                refs.Add(neighbor);
                                if (has && Has(domains[neighbor], (d + 2) % 4, true)) supportedPorts++;
                                if (stage == 1 && !Has(domains[neighbor], (d + 2) % 4, has)) valid = false;
                            }
                            else if (has && p.IsEndpointPort(cell % p.Grid.Width, cell / p.Grid.Width, (Direction)d)) supportedPorts++;
                        }
                        if (stage == 3 && value != 0 && supportedPorts != 2) valid = false;
                        if (valid) allowed |= 1 << value;
                    }
                    if (!Reduce(cell, allowed, stage == 1 ? "NEIGHBOR_PORT_REQUIRED" : "LOCAL_DEGREE_REQUIRED", refs)) return false;
                }
            }
            else if (stage == 2)
            {
                for (int axis = 0; axis < 2; axis++)
                {
                    int lineCount = axis == 0 ? p.Grid.Height : p.Grid.Width, length = axis == 0 ? p.Grid.Width : p.Grid.Height;
                    for (int line = 0; line < lineCount; line++)
                    {
                        var refs = Enumerable.Range(0, length).Select(i => axis == 0 ? line * p.Grid.Width + i : i * p.Grid.Width + line).ToArray();
                        int target = axis == 0 ? p.RowCounts[line] : p.ColumnCounts[line], min = 0, max = 0;
                        foreach (int cell in refs) { if ((domains[cell] & 1) == 0) min++; if ((domains[cell] & 126) != 0) max++; }
                        if (min > target || max < target) return false;
                        foreach (int cell in refs)
                        {
                            if (max == target && (domains[cell] & 126) != 0 && !Reduce(cell, 126, "LINE_ALL_REMAINING_OCCUPIED", refs, axis == 0 ? "ROW" : "COLUMN", line)) return false;
                            if (min == target && (domains[cell] & 1) != 0 && !Reduce(cell, 1, target == 0 ? "LINE_ZERO" : "LINE_TARGET_REACHED", refs, axis == 0 ? "ROW" : "COLUMN", line)) return false;
                        }
                    }
                }
            }
            else if (stage == 4)
            {
                var parents = Enumerable.Range(0, domains.Length).ToArray();
                int Root(int cell) { while (parents[cell] != cell) cell = parents[cell]; return cell; }
                for (int cell = 0; cell < domains.Length; cell++)
                for (int d = 0; d < 4; d++)
                {
                    int neighbor = Neighbor(p, cell, d);
                    if (neighbor <= cell || Has(domains[cell], d, false) || Has(domains[neighbor], (d + 2) % 4, false)) continue;
                    int left = Root(cell), right = Root(neighbor);
                    if (left == right) return false;
                    parents[left] = right;
                }
                for (int cell = 0; cell < domains.Length; cell++)
                {
                    int allowed = domains[cell];
                    for (int value = 1; value < 7; value++)
                    {
                        if ((allowed & (1 << value)) == 0) continue;
                        var adjacent = new List<int>(2);
                        for (int d = 0; d < 4; d++) if ((Ports(value) & (1 << d)) != 0)
                        {
                            int n = Neighbor(p, cell, d); if (n >= 0 && !Has(domains[n], (d + 2) % 4, false)) adjacent.Add(n);
                        }
                        // A mandatory component containing this cell would make
                        // its existing edges self-referential in this candidate test.
                        // Such a cycle is detected above; prune only two components
                        // whose joining path does not already go through this cell.
                        if (adjacent.Count == 2 && Root(adjacent[0]) == Root(adjacent[1]) && Root(cell) != Root(adjacent[0])) allowed &= ~(1 << value);
                    }
                    if (!Reduce(cell, allowed, "LOOP_PREVENTION", Enumerable.Range(0, domains.Length).Where(i => (domains[i] & 1) == 0))) return false;
                }
            }
            else if (stage == 5)
            {
                var reached = new bool[domains.Length]; var queue = new Queue<int>(); int start = p.Index(p.A.Cell); reached[start] = true; queue.Enqueue(start);
                while (queue.Count > 0)
                {
                    int cell = queue.Dequeue();
                    for (int d = 0; d < 4; d++)
                    {
                        int n = Neighbor(p, cell, d);
                        if (n >= 0 && !reached[n] && Has(domains[cell], d, true) && Has(domains[n], (d + 2) % 4, true)) { reached[n] = true; queue.Enqueue(n); }
                    }
                }
                if (!reached[p.Index(p.B.Cell)]) return false;
                for (int cell = 0; cell < domains.Length; cell++) if (!reached[cell] && !Reduce(cell, 1, "CONNECTIVITY_PRESERVATION", Enumerable.Range(0, domains.Length).Where(i => reached[i]))) return false;
            }
            return domains.All(d => d != 0);
        }
        private static int SelectMrv(byte[] domains)
        {
            int best = -1, count = 8;
            for (int i = 0; i < domains.Length; i++) { int size = 0; for (int v = 0; v < 7; v++) if ((domains[i] & (1 << v)) != 0) size++; if (size > 1 && size < count) { best = i; count = size; } }
            return best;
        }
        // A UNIQUE verdict needs a verified trace: an untraceable root reduction is INDETERMINATE, never a silent UNIQUE.
        private static SolverClassification Classify(bool interrupted, int found, bool traceFaulted)
            => interrupted ? SolverClassification.INDETERMINATE : found == 0 ? SolverClassification.UNSATISFIABLE : found == 1 ? (traceFaulted ? SolverClassification.INDETERMINATE : SolverClassification.UNIQUE) : SolverClassification.MULTIPLE_OR_MORE;
        public static SolverResult Solve(PuzzleDefinition? definition, SolverLimits? limits = null)
        {
            var trace = new List<RecoveryReduction>(); var path = new List<CellCoordinate>(); int found = 0, searchNodes = 0, guessDepth = 0;
            if (definition == null) return new SolverResult(SolverClassification.INDETERMINATE, 0, path, trace);
            var p = definition; var tracer = new RootDeductionTracer(definition); var budget = limits ?? new SolverLimits(); long startTime = budget.MonotonicNow(), states = 0; bool interrupted = false; long lastTime = startTime;
            bool CheckBudget()
            {
                long now = budget.MonotonicNow();
                if (budget.Cancellation.IsCancellationRequested || budget.TimeBudgetMs < 0 || startTime < 0 || now < lastTime || now - startTime >= budget.TimeBudgetMs) { interrupted = true; return false; }
                lastTime = now; return true;
            }
            // depth 0 is the root: only its propagation is traced; DFS assumptions deepen the path.
            void Search(byte[] domains, int depth)
            {
                if (found >= 2 || interrupted) return;
                if (!CheckBudget()) return;
                if (states >= budget.MaxStates) { interrupted = true; return; } states++;
                var work = new SortedSet<int>(Enumerable.Range(0, 6));
                while (work.Count > 0)
                {
                    if (!CheckBudget()) return;
                    int stage = work.Min; work.Remove(stage); int before = trace.Count;
                    if (!RunStageCore(p, domains, trace, stage, depth == 0 ? tracer : null)) return;
                    if (trace.Count != before) foreach (int dirty in Enumerable.Range(0, 6)) work.Add(dirty);
                }
                int selected = SelectMrv(domains);
                if (selected == -1)
                {
                    var cells = new CellContent[domains.Length];
                    for (int i = 0; i < cells.Length; i++) for (int v = 0; v < 7; v++) if (domains[i] == (1 << v)) cells[i] = v == 0 ? CellContent.UNSET : (CellContent)(v + 2);
                    var complete = PuzzleEvaluator.Evaluate(p, cells);
                    if (complete.Completed) { found++; if (found == 1) { path.AddRange(complete.Path); guessDepth = depth; } }
                    return;
                }
                for (int value = 0; value < 7 && found < 2 && !interrupted; value++)
                {
                    if ((domains[selected] & (1 << value)) == 0) continue;
                    var child = (byte[])domains.Clone(); child[selected] = (byte)(1 << value); searchNodes++; Search(child, depth + 1);
                }
            }
            Search(Enumerable.Repeat((byte)127, p.Grid.CellCount).ToArray(), 0);
            // Everything below up to the single final CheckBudget() is budgeted work: reduction dedup, the trace build,
            // the metrics and the construction of the UNIQUE result. A UNIQUE verdict is only handed out when the last
            // budget probe, taken after all of this work, still sits inside the budget (ADR-031: a timeout is never UNIQUE).
            var uniqueTrace = new HashSet<string>(StringComparer.Ordinal);
            var reductions = trace.Where(r => uniqueTrace.Add(r.ToString())).ToArray();
            var candidate = Classify(interrupted, found, tracer.Faulted);
            SolverResult? unique = null;
            if (candidate == SolverClassification.UNIQUE)
            {
                var steps = tracer.Build();
                unique = new SolverResult(candidate, found, path, reductions, SolverMetrics.FromTrace(searchNodes, steps, guessDepth), steps);
            }
            // The probe is the last operation that takes time; only returning the finished object follows it.
            CheckBudget();
            if (interrupted) return new SolverResult(SolverClassification.INDETERMINATE, found, path, reductions);
            return unique ?? new SolverResult(candidate, found, path, reductions);
        }
    }
}
