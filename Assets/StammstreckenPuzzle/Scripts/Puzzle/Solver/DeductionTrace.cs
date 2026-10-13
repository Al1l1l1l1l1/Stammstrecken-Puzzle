using System;
using System.Collections.Generic;
using System.Linq;
using STP.Puzzle.Domain;

namespace STP.Puzzle.Solver
{
    /// <summary>One of the seven solver candidate values of a cell, in the fixed solver-v2 value order.</summary>
    public enum CandidateValue { EMPTY, TRACK_NS, TRACK_EW, TRACK_NE, TRACK_ES, TRACK_SW, TRACK_WN }

    /// <summary>Public puzzle facts a root deduction may reference. No other input exists for the solver.</summary>
    public enum PublicFactKind { GRID_SIZE, ENDPOINT_A, ENDPOINT_B, ROW_COUNT, COLUMN_COUNT }

    /// <summary>
    /// A premise of a root deduction: either a public puzzle fact or an earlier root trace entry.
    /// Immutable; never carries UI, persistence, asset or engine data.
    /// </summary>
    public sealed class DeductionPremise
    {
        /// <summary>The public fact, or null when this premise is an earlier trace entry.</summary>
        public PublicFactKind? PublicFact { get; }
        /// <summary>Row or column index for <see cref="PublicFactKind.ROW_COUNT"/> / <see cref="PublicFactKind.COLUMN_COUNT"/>, otherwise -1.</summary>
        public int LineIndex { get; }
        /// <summary>Index of an earlier <see cref="DeductionStep"/> in the same trace, or -1 for a public fact.</summary>
        public int TraceIndex { get; }
        public bool IsPublicFact => PublicFact.HasValue;
        private DeductionPremise(PublicFactKind? fact, int line, int trace) { PublicFact = fact; LineIndex = line; TraceIndex = trace; }
        internal static DeductionPremise Fact(PublicFactKind kind, int line = -1) => new DeductionPremise(kind, line, -1);
        internal static DeductionPremise Entry(int traceIndex) => new DeductionPremise(null, -1, traceIndex);
    }

    /// <summary>
    /// One explainable, safe root reduction: a rule code, the concrete conclusion
    /// "the cell can no longer take <see cref="RemovedValues"/>", and its minimal premises.
    /// </summary>
    public sealed class DeductionStep
    {
        /// <summary>Position in the root trace; premises only ever reference smaller indices.</summary>
        public int Index { get; }
        public string RuleCode { get; }
        public CellCoordinate Cell { get; }
        /// <summary>Candidate values this step removed from the cell (non-empty, ascending value order).</summary>
        public IReadOnlyList<CandidateValue> RemovedValues { get; }
        /// <summary>Candidate values the cell has left after this step (non-empty, ascending value order).</summary>
        public IReadOnlyList<CandidateValue> RemainingValues { get; }
        /// <summary>Public facts first, then earlier trace entries in ascending index order.</summary>
        public IReadOnlyList<DeductionPremise> Premises { get; }
        /// <summary>1 without a derived premise, otherwise 1 + the largest depth of the referenced earlier entries.</summary>
        public int Depth { get; }
        internal DeductionStep(int index, string ruleCode, CellCoordinate cell, IEnumerable<CandidateValue> removed, IEnumerable<CandidateValue> remaining, IEnumerable<DeductionPremise> premises, int depth)
        {
            Index = index; RuleCode = ruleCode; Cell = cell; Depth = depth;
            RemovedValues = Array.AsReadOnly(removed.ToArray()); RemainingValues = Array.AsReadOnly(remaining.ToArray()); Premises = Array.AsReadOnly(premises.ToArray());
        }
    }

    /// <summary>The four solver-v2 metrics (ADR-031). Only produced for a UNIQUE result.</summary>
    public sealed class SolverMetrics
    {
        /// <summary>Begun DFS value assumptions up to the solution limit of two; root propagation is excluded.</summary>
        public int SearchNodes { get; }
        /// <summary>Deduplicated root reductions; all search branches are excluded.</summary>
        public int DeductionSteps { get; }
        /// <summary>Longest premise chain of the root trace; 0 without a root step.</summary>
        public int MaxDeductionDepth { get; }
        /// <summary>Simultaneously active assumptions on the deterministic path to the single solution.</summary>
        public int RequiredGuessDepth { get; }
        internal SolverMetrics(int searchNodes, int deductionSteps, int maxDeductionDepth, int requiredGuessDepth)
        { SearchNodes = searchNodes; DeductionSteps = deductionSteps; MaxDeductionDepth = maxDeductionDepth; RequiredGuessDepth = requiredGuessDepth; }
        /// <summary>Derives the root metrics from the trace alone; search metrics come from the DFS.</summary>
        internal static SolverMetrics FromTrace(int searchNodes, IReadOnlyList<DeductionStep> trace, int requiredGuessDepth)
            => new SolverMetrics(searchNodes, trace.Count, trace.Count == 0 ? 0 : trace.Max(step => step.Depth), requiredGuessDepth);
    }
}
