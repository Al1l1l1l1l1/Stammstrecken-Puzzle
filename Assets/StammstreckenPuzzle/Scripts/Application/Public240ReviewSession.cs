using System;
using System.Collections.Generic;
using System.Linq;
using STP.Puzzle.Domain;

namespace STP.Application
{
    /// <summary>Provider-free read/command boundary; I may adapt its persisted session without changing the view.</summary>
    public interface IReviewPuzzleSession
    {
        /// <summary>Verified public puzzle input.</summary>
        PublicPuzzleInput Puzzle { get; }
        /// <summary>Grid dimensions.</summary>
        int Width { get; }
        /// <summary>Grid dimensions.</summary>
        int Height { get; }
        /// <summary>Validated A exterior projection.</summary>
        DevelopmentEndpoint EndpointA { get; }
        /// <summary>Validated B exterior projection.</summary>
        DevelopmentEndpoint EndpointB { get; }
        /// <summary>Presentation phase, separate from domain save truth.</summary>
        DevelopmentPreviewPhase Phase { get; }
        /// <summary>Last command result.</summary>
        DevelopmentCommandStatus LastCommand { get; }
        /// <summary>Stable error identifier.</summary>
        string LastErrorCode { get; }
        /// <summary>Attempt identity.</summary>
        Guid AttemptId { get; }
        /// <summary>Domain revision.</summary>
        long Revision { get; }
        /// <summary>Timer has started from an actual mutation.</summary>
        bool TimerStarted { get; }
        /// <summary>Lifecycle timer is suspended.</summary>
        bool IsPaused { get; }
        /// <summary>Undo availability.</summary>
        bool CanUndo { get; }
        /// <summary>Objective diagnostics only.</summary>
        bool HasObjectiveViolations { get; }
        /// <summary>Copied actual PuzzleSolved path.</summary>
        IReadOnlyList<DevelopmentCell> BuiltPath { get; }
        /// <summary>Visible cell read.</summary>
        DevelopmentTool CellAt(int x, int y);
        /// <summary>Public row target.</summary>
        int RowTarget(int y);
        /// <summary>Public column target.</summary>
        int ColumnTarget(int x);
        /// <summary>Current visible row count.</summary>
        int RowVisible(int y);
        /// <summary>Current visible column count.</summary>
        int ColumnVisible(int x);
        /// <summary>Read-only active-time projection.</summary>
        long ElapsedAt(long monotonicMilliseconds);
        /// <summary>Captures immutable attempt/command/revision binding.</summary>
        DevelopmentCommandEnvelope CaptureCommand();
        /// <summary>Single user command.</summary>
        DevelopmentCommandStatus Apply(int x,int y,DevelopmentTool tool,long time);
        /// <summary>Explicitly bound single command.</summary>
        DevelopmentCommandStatus Apply(int x,int y,DevelopmentTool tool,long time,DevelopmentCommandEnvelope envelope);
        /// <summary>One atomic selection command.</summary>
        DevelopmentCommandStatus ApplyBatch(IReadOnlyList<DevelopmentCell>? cells,DevelopmentTool tool,long time);
        /// <summary>Explicitly bound atomic selection.</summary>
        DevelopmentCommandStatus ApplyBatch(IReadOnlyList<DevelopmentCell>? cells,DevelopmentTool tool,long time,DevelopmentCommandEnvelope envelope);
        /// <summary>One atomic undo.</summary>
        DevelopmentCommandStatus Undo(long time);
        /// <summary>Lifecycle pause.</summary>
        DevelopmentCommandStatus Pause(long time);
        /// <summary>Lifecycle resume.</summary>
        DevelopmentCommandStatus Resume(long time);
        /// <summary>New attempt on the same puzzle.</summary>
        DevelopmentCommandStatus Restart();
        /// <summary>Finishes only an actual ride of the expected attempt.</summary>
        DevelopmentCommandStatus CompleteTrainRide(Guid expectedAttempt);
    }

    /// <summary>
    /// Pure, in-memory QA orchestration under ADR034. Every cell/lifecycle change goes
    /// through the existing domain reducer. Its solved snapshot stays SolvedPendingCommit;
    /// only the separate QA preview advances to an unsaved ride/result. This type has no
    /// save, reward, catalog, provider, authoring-solution or solver dependency.
    /// </summary>
    public sealed class Public240ReviewSession : IReviewPuzzleSession
    {
        private readonly PuzzleDefinition definition;
        private PuzzleSessionState state;
        private PuzzleEvaluation evaluation;
        private IReadOnlyList<DevelopmentCell> builtPath = Array.AsReadOnly(Array.Empty<DevelopmentCell>());
        private long lastMonotonicMilliseconds;
        private DevelopmentPreviewPhase overviewReturnPhase;

        /// <summary>
        /// Validates public QA inputs through domain factories. Invalid configuration is
        /// a programmer error; user commands instead return stable rejection codes.
        /// </summary>
        public Public240ReviewSession(PublicPuzzleInput puzzle)
        {
            Puzzle = puzzle ?? throw new ArgumentNullException(nameof(puzzle));
            definition = puzzle.ToDomainDefinition();
            state = CreateAttempt();
            evaluation = PuzzleEvaluator.Evaluate(definition, state.Cells);
        }

        /// <summary>Genuine public identity and validated input supplied by the strict catalog.</summary>
        public PublicPuzzleInput Puzzle { get; }

        /// <summary>Technical QA scenario identity, never a campaign catalog reference.</summary>
        public string ScenarioId => Puzzle.Identity.PuzzleId;
        /// <summary>Grid width.</summary>
        public int Width => definition.Grid.Width;
        /// <summary>Grid height.</summary>
        public int Height => definition.Grid.Height;
        /// <summary>Entry endpoint from public input.</summary>
        public DevelopmentEndpoint EndpointA => new DevelopmentEndpoint(definition.A);
        /// <summary>Exit endpoint from public input.</summary>
        public DevelopmentEndpoint EndpointB => new DevelopmentEndpoint(definition.B);
        /// <summary>Entry endpoint alias used by the QA world renderer.</summary>
        public DevelopmentEndpoint A => EndpointA;
        /// <summary>Exit endpoint alias used by the QA world renderer.</summary>
        public DevelopmentEndpoint B => EndpointB;
        /// <summary>Current transient QA phase, separate from domain and saved progress.</summary>
        public DevelopmentPreviewPhase Phase { get; private set; }
        /// <summary>Most recent QA action result.</summary>
        public DevelopmentCommandStatus LastCommand { get; private set; } = DevelopmentCommandStatus.Unchanged;
        /// <summary>Stable technical rejection code; never a localized user message.</summary>
        public string LastErrorCode { get; private set; } = "NONE";
        /// <summary>Fresh per-attempt identity, also guarding delayed callbacks after restart.</summary>
        public Guid AttemptId => state.AttemptId;
        /// <summary>Current domain revision.</summary>
        public long Revision => state.Revision;
        /// <summary>Whether a real state-changing input has started the domain timer.</summary>
        public bool TimerStarted => state.TimerStarted;
        /// <summary>Whether the current domain attempt is suspended.</summary>
        public bool IsPaused => state.Phase == SessionPhase.Suspended;
        /// <summary>Whether undo is available for the current editable attempt.</summary>
        public bool CanUndo => Phase == DevelopmentPreviewPhase.Editing && !IsPaused && state.UndoStack.Count > 0;
        /// <summary>Sticky domain marker history; undo does not erase it.</summary>
        public bool UsedEmptyMarker => state.UsedEmptyMarker;
        /// <summary>Sticky domain marker history; undo does not erase it.</summary>
        public bool UsedOccupiedMarker => state.UsedOccupiedMarker;
        /// <summary>Domain correction history.</summary>
        public long CorrectionCount => state.CorrectionCount;
        /// <summary>Only objective, currently visible domain diagnostics.</summary>
        public bool HasObjectiveViolations => evaluation.Diagnostics.Count > 0;
        /// <summary>True after completion: QA preview never commits or advances the domain save phase.</summary>
        public bool IsAwaitingSaveCommit => state.Phase == SessionPhase.SolvedPendingCommit;
        /// <summary>Copied immutable A-to-B path from the actual PuzzleSolved event; empty before it.</summary>
        public IReadOnlyList<DevelopmentCell> BuiltPath => builtPath;

        /// <summary>Reads current visible content without supplying a solution oracle.</summary>
        public DevelopmentTool CellAt(int x, int y)
        {
            if (!definition.Grid.Contains(x, y)) throw new ArgumentOutOfRangeException(nameof(x));
            return (DevelopmentTool)state.Cells[y * Width + x];
        }
        /// <summary>Public target count for a row.</summary>
        public int RowTarget(int y) => definition.RowCounts[y];
        /// <summary>Public target count for a column.</summary>
        public int ColumnTarget(int x) => definition.ColumnCounts[x];
        /// <summary>Visible tracks plus gray assumptions in a row, without a truth claim.</summary>
        public int RowVisible(int y) => evaluation.VisibleRows[y];
        /// <summary>Visible tracks plus gray assumptions in a column, without a truth claim.</summary>
        public int ColumnVisible(int x) => evaluation.VisibleColumns[x];

        /// <summary>Projects the running domain timer; read-only calls cannot start or change it.</summary>
        public long ElapsedAt(long monotonicMilliseconds)
        {
            long elapsed = state.ActiveElapsedMs;
            if (state.Phase != SessionPhase.Active || !state.TimerStarted || monotonicMilliseconds < lastMonotonicMilliseconds) return elapsed;
            long delta = monotonicMilliseconds - lastMonotonicMilliseconds;
            return delta > long.MaxValue - elapsed ? long.MaxValue : elapsed + delta;
        }

        /// <summary>Captures a fresh command identity and the current attempt/revision for later delivery.</summary>
        public DevelopmentCommandEnvelope CaptureCommand() => new DevelopmentCommandEnvelope(AttemptId, Guid.NewGuid(), Revision);

        /// <summary>Applies one tool immediately using a fresh domain command envelope.</summary>
        public DevelopmentCommandStatus Apply(int x, int y, DevelopmentTool tool, long monotonicMilliseconds)
            => Apply(x, y, tool, monotonicMilliseconds, CaptureCommand());
        /// <summary>Applies one tool with explicit original callback identity/revision.</summary>
        public DevelopmentCommandStatus Apply(int x, int y, DevelopmentTool tool, long monotonicMilliseconds, DevelopmentCommandEnvelope envelope)
            => Execute(new ApplyCellContent(envelope.CommandId, envelope.ExpectedRevision,
                CellCoordinate.Create(definition.Grid, x, y).Value, (CellContent)tool), monotonicMilliseconds, envelope);

        /// <summary>Applies an entire selection as exactly one atomic domain action.</summary>
        public DevelopmentCommandStatus ApplyBatch(IReadOnlyList<DevelopmentCell>? cells, DevelopmentTool tool, long monotonicMilliseconds)
            => ApplyBatch(cells, tool, monotonicMilliseconds, CaptureCommand());
        /// <summary>Applies an entire selection with explicit original callback identity/revision.</summary>
        public DevelopmentCommandStatus ApplyBatch(IReadOnlyList<DevelopmentCell>? cells, DevelopmentTool tool, long monotonicMilliseconds, DevelopmentCommandEnvelope envelope)
        {
            var coordinates = cells?.Select(cell => CellCoordinate.Create(definition.Grid, cell.X, cell.Y).Value!).ToArray();
            return Execute(new ApplyCellContentBatch(envelope.CommandId, envelope.ExpectedRevision, coordinates, (CellContent)tool), monotonicMilliseconds, envelope);
        }

        /// <summary>Undoes exactly one atomic domain action.</summary>
        public DevelopmentCommandStatus Undo(long monotonicMilliseconds) => Undo(monotonicMilliseconds, CaptureCommand());
        /// <summary>Undoes with explicit callback identity/revision.</summary>
        public DevelopmentCommandStatus Undo(long monotonicMilliseconds, DevelopmentCommandEnvelope envelope)
            => Execute(new UndoLastAction(envelope.CommandId, envelope.ExpectedRevision), monotonicMilliseconds, envelope);

        /// <summary>Suspends the real domain timer; repeated lifecycle notifications are harmless.</summary>
        public DevelopmentCommandStatus Pause(long monotonicMilliseconds) => Pause(monotonicMilliseconds, CaptureCommand());
        /// <summary>Suspends using an explicitly bound lifecycle callback.</summary>
        public DevelopmentCommandStatus Pause(long monotonicMilliseconds, DevelopmentCommandEnvelope envelope)
        {
            if (envelope.AttemptId != AttemptId) return Reject("STALE_ATTEMPT");
            if (Phase != DevelopmentPreviewPhase.Editing || IsPaused) return Unchanged();
            return Execute(new PauseForLifecycle(envelope.CommandId, envelope.ExpectedRevision), monotonicMilliseconds, envelope);
        }

        /// <summary>Resumes the real domain timer only while the puzzle is shown.</summary>
        public DevelopmentCommandStatus Resume(long monotonicMilliseconds) => Resume(monotonicMilliseconds, CaptureCommand());
        /// <summary>Resumes using an explicitly bound lifecycle callback.</summary>
        public DevelopmentCommandStatus Resume(long monotonicMilliseconds, DevelopmentCommandEnvelope envelope)
        {
            if (envelope.AttemptId != AttemptId) return Reject("STALE_ATTEMPT");
            if (Phase != DevelopmentPreviewPhase.Editing || !IsPaused) return Unchanged();
            return Execute(new ResumeFromLifecycle(envelope.CommandId, envelope.ExpectedRevision), monotonicMilliseconds, envelope);
        }

        /// <summary>Shows only the local QA overview and retains the unsaved attempt in memory.</summary>
        public DevelopmentCommandStatus ShowOverview(long monotonicMilliseconds)
        {
            if (Phase == DevelopmentPreviewPhase.TrainRide) return Reject("TRAIN_RIDE_ACTIVE");
            if (Phase == DevelopmentPreviewPhase.Overview) return Unchanged();
            if (Phase == DevelopmentPreviewPhase.Editing && Pause(monotonicMilliseconds) == DevelopmentCommandStatus.Rejected) return LastCommand;
            overviewReturnPhase = Phase;
            Phase = DevelopmentPreviewPhase.Overview;
            return Applied();
        }

        /// <summary>Returns to the retained QA editing/result view, excluding overview time.</summary>
        public DevelopmentCommandStatus ReturnToPuzzle(long monotonicMilliseconds)
        {
            if (Phase != DevelopmentPreviewPhase.Overview) return Unchanged();
            if (overviewReturnPhase == DevelopmentPreviewPhase.Editing && IsPaused)
            {
                var envelope = CaptureCommand();
                if (Execute(new ResumeFromLifecycle(envelope.CommandId, envelope.ExpectedRevision), monotonicMilliseconds, envelope) == DevelopmentCommandStatus.Rejected) return LastCommand;
            }
            Phase = overviewReturnPhase;
            return Applied();
        }

        /// <summary>Completes the current QA-only ride; no save or reward is asserted.</summary>
        public DevelopmentCommandStatus CompleteTrainRide() => CompleteTrainRide(AttemptId);
        /// <summary>Completes only the ride belonging to the captured original attempt.</summary>
        public DevelopmentCommandStatus CompleteTrainRide(Guid expectedAttemptId)
        {
            if (expectedAttemptId != AttemptId) return Reject("STALE_ATTEMPT");
            if (Phase == DevelopmentPreviewPhase.Results) return Unchanged();
            if (Phase != DevelopmentPreviewPhase.TrainRide) return Reject("TRAIN_RIDE_NOT_ACTIVE");
            Phase = DevelopmentPreviewPhase.Results;
            return Applied();
        }

        /// <summary>Starts a fresh unsaved QA attempt; an active ride cannot be interrupted.</summary>
        public DevelopmentCommandStatus Restart()
        {
            if (Phase == DevelopmentPreviewPhase.TrainRide) return Reject("TRAIN_RIDE_ACTIVE");
            state = CreateAttempt();
            evaluation = PuzzleEvaluator.Evaluate(definition, state.Cells);
            builtPath = Array.AsReadOnly(Array.Empty<DevelopmentCell>());
            lastMonotonicMilliseconds = 0;
            Phase = overviewReturnPhase = DevelopmentPreviewPhase.Editing;
            return Applied();
        }

        private PuzzleSessionState CreateAttempt()
        {
            return PuzzleSessionState.Create(definition, Puzzle.Identity.PuzzleId, Puzzle.Identity.HashProfile,
                Puzzle.Identity.HashSha256, Guid.NewGuid(), SessionMode.PRACTICE, 0, null, null).Value
                ?? throw new InvalidOperationException("INVALID_QA_ATTEMPT");
        }

        private DevelopmentCommandStatus Execute(PuzzleCommand command, long time, DevelopmentCommandEnvelope envelope)
        {
            if (envelope.AttemptId != AttemptId) return Reject("STALE_ATTEMPT");
            var result = PuzzleCommandHandler.Handle(state, command, time);
            if (result.Error != DomainError.NONE) return Reject(result.Error.ToString());
            if (ReferenceEquals(state, result.State)) return Unchanged();
            state = result.State ?? throw new InvalidOperationException("INTERNAL_INVARIANT_BROKEN");
            lastMonotonicMilliseconds = time;
            evaluation = PuzzleEvaluator.Evaluate(definition, state.Cells);
            foreach (var fact in result.Events)
            {
                if (fact.Kind != PuzzleEventKind.PuzzleSolved) continue;
                builtPath = Array.AsReadOnly(fact.Path.Select(cell => new DevelopmentCell(cell.X, cell.Y)).ToArray());
                Phase = DevelopmentPreviewPhase.TrainRide;
            }
            return Applied();
        }

        private DevelopmentCommandStatus Applied() { LastErrorCode = "NONE"; return LastCommand = DevelopmentCommandStatus.Applied; }
        private DevelopmentCommandStatus Unchanged() { LastErrorCode = "NONE"; return LastCommand = DevelopmentCommandStatus.Unchanged; }
        private DevelopmentCommandStatus Reject(string code) { LastErrorCode = code; return LastCommand = DevelopmentCommandStatus.Rejected; }
    }
}
