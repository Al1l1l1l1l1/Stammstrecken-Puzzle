using System;
using System.Linq;
using System.Threading;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Application
{
    /// <summary>One safe public-root conclusion, never an authoring path, proof blob or overall solution.</summary>
    public sealed class SafePuzzleHint
    {
        private readonly PublicPuzzleInput puzzle;
        private readonly Guid attempt;
        private readonly long revision;
        private readonly Guid displayId = Guid.NewGuid();
        internal SafePuzzleHint(PublicPuzzleInput puzzle, PuzzleSessionCheckpoint checkpoint, DeductionStep step)
        { this.puzzle = puzzle; attempt = checkpoint.AttemptId; revision = checkpoint.Revision; X = step.Cell.X; Y = step.Cell.Y; RuleCode = step.RuleCode; Content = step.RemainingValues[0] == CandidateValue.EMPTY ? CellContent.MARK_EMPTY : (CellContent)((int)step.RemainingValues[0] + 2); }
        /// <summary>Conclusion column.</summary>
        public int X { get; }
        /// <summary>Conclusion row.</summary>
        public int Y { get; }
        /// <summary>Audited solver rule code for a localized single-step explanation.</summary>
        public string RuleCode { get; }
        /// <summary>One objectively safe cell fact; displaying it does not fill the board.</summary>
        public CellContent Content { get; }
        /// <summary>Internal before-display reservation identity, without acknowledging a visible hint.</summary>
        internal Guid ReservedDisplayId=>displayId;
        internal long ReservedRevision=>revision;
        /// <summary>Call only after the help view actually displayed this conclusion successfully.</summary>
        public SafeHintDisplayReceipt AcknowledgeDisplayed() => new SafeHintDisplayReceipt(puzzle, attempt, revision, displayId);
    }
    /// <summary>Immutable display acknowledgement bound to exact puzzle, attempt and revision.</summary>
    public sealed class SafeHintDisplayReceipt
    {
        internal SafeHintDisplayReceipt(PublicPuzzleInput puzzle, Guid attempt, long revision, Guid display) { Puzzle = puzzle; AttemptId = attempt; ExpectedRevision = revision; DisplayId = display; }
        /// <summary>Verified public input used to derive the one conclusion.</summary>
        public PublicPuzzleInput Puzzle { get; }
        /// <summary>Complete immutable puzzle identity.</summary>
        public PublicPuzzleIdentity Identity => Puzzle.Identity;
        /// <summary>Original help attempt.</summary>
        public Guid AttemptId { get; }
        /// <summary>Original immutable session revision.</summary>
        public long ExpectedRevision { get; }
        /// <summary>Once-only display/command ID.</summary>
        public Guid DisplayId { get; }
    }
    /// <summary>Bounded worker-side hint projection from public solver root deductions only.</summary>
    public static class SafePuzzleHints
    {
        /// <summary>Returns null without spending a credit when no safe nonredundant conclusion is available.</summary>
        public static SafePuzzleHint? Prepare(PublicPuzzleInput puzzle, PuzzleSessionCheckpoint checkpoint, CancellationToken cancellation = default)
        {
            if (puzzle == null || checkpoint == null) throw new ArgumentNullException();
            var restored = checkpoint.Restore(puzzle.ToDomainDefinition(), puzzle.Identity.PuzzleId, puzzle.Identity.HashProfile, puzzle.Identity.HashSha256);
            if (!restored.Succeeded || restored.Value == null || restored.Value.Phase == SessionPhase.SolvedPendingCommit) return null;
            var result = PuzzleSolver.Solve(puzzle.ToDomainDefinition(), new SolverLimits(100000, 100, cancellation));
            if (result.Classification != SolverClassification.UNIQUE || result.Metrics?.RequiredGuessDepth != 0) return null;
            var step = result.DeductionTrace.FirstOrDefault(s => s.RemainingValues.Count == 1 && checkpoint.Cells[s.Cell.Y * puzzle.Width + s.Cell.X] != (s.RemainingValues[0] == CandidateValue.EMPTY ? CellContent.MARK_EMPTY : (CellContent)((int)s.RemainingValues[0] + 2)));
            return step == null ? null : new SafePuzzleHint(puzzle, checkpoint, step);
        }
    }
}
