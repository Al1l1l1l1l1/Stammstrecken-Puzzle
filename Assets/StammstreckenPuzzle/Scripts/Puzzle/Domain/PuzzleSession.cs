using System;
using System.Collections.Generic;
using System.Linq;

namespace STP.Puzzle.Domain
{
    /// <summary>Mode is identity data here, without progress or reward policy.</summary>
    public enum SessionMode { FIRST_RUN, PRACTICE, REVISION, ENDLESS }
    /// <summary>Only Ready, Active, Suspended and SolvedPendingCommit transitions are implemented.</summary>
    public enum SessionPhase { Loading, Ready, Active, SolvedPendingCommit, RecoveryRequired, TrainRide, Results, Closed, Suspended }
    /// <summary>One immutable coordinate change.</summary>
    public sealed class CellChange
    {
        public CellCoordinate Coordinate { get; }
        public CellContent Before { get; }
        public CellContent After { get; }
        internal CellChange(CellCoordinate coordinate, CellContent before, CellContent after) { Coordinate = coordinate; Before = before; After = after; }
    }
    /// <summary>One atomic user action, with changes ordered by y then x.</summary>
    public sealed class CellDiff
    {
        public IReadOnlyList<CellChange> Changes { get; }
        internal CellDiff(IEnumerable<CellChange> changes) { Changes = Array.AsReadOnly(changes.ToArray()); }
    }
    /// <summary>Immutable attempt snapshot. Hash/UTC inputs are opaque external data.</summary>
    public sealed class PuzzleSessionState
    {
        public PuzzleDefinition Definition { get; }
        public string PuzzleId { get; }
        public string PublicPuzzleHashProfile { get; }
        public string PublicPuzzleHashSha256 { get; }
        public Guid AttemptId { get; }
        public SessionMode Mode { get; }
        public IReadOnlyList<CellContent> Cells { get; }
        public SessionPhase Phase { get; }
        public long Revision { get; }
        public bool TimerStarted { get; }
        public long ActiveElapsedMs { get; }
        public bool UsedEmptyMarker { get; }
        public bool UsedOccupiedMarker { get; }
        public int HintCount { get; }
        public long CorrectionCount { get; }
        public IReadOnlyList<CellDiff> UndoStack { get; }
        public string? SolvedEventId { get; }
        public DateTimeOffset? StartedAtUtc { get; }
        public DateTimeOffset? LastSavedAtUtc { get; }
        internal IReadOnlyList<bool> PreviouslySet { get; }
        internal IReadOnlyList<Guid> AppliedCommandIds { get; }
        internal long LastMonotonicMs { get; }
        internal PuzzleSessionState(PuzzleDefinition definition, string puzzleId, string profile, string hash, Guid attempt, SessionMode mode, int hints, DateTimeOffset? started, DateTimeOffset? saved,
            CellContent[] cells, bool[] previouslySet, CellDiff[] undo, Guid[] commandIds, SessionPhase phase, long revision, bool timerStarted, long elapsed, long time, bool empty, bool occupied, long corrections, string? solved)
        {
            Definition = definition; PuzzleId = puzzleId; PublicPuzzleHashProfile = profile; PublicPuzzleHashSha256 = hash; AttemptId = attempt; Mode = mode; HintCount = hints; StartedAtUtc = started; LastSavedAtUtc = saved;
            Cells = Array.AsReadOnly(cells); PreviouslySet = Array.AsReadOnly(previouslySet); UndoStack = Array.AsReadOnly(undo); AppliedCommandIds = Array.AsReadOnly(commandIds);
            Phase = phase; Revision = revision; TimerStarted = timerStarted; ActiveElapsedMs = elapsed; LastMonotonicMs = time; UsedEmptyMarker = empty; UsedOccupiedMarker = occupied; CorrectionCount = corrections; SolvedEventId = solved;
        }
        public static DomainResult<PuzzleSessionState> Create(PuzzleDefinition? definition, string? puzzleId, string? publicPuzzleHashProfile, string? publicPuzzleHashSha256, Guid attemptId, SessionMode mode, int hintCount, DateTimeOffset? startedAtUtc, DateTimeOffset? lastSavedAtUtc)
        {
            if (definition == null || string.IsNullOrWhiteSpace(puzzleId) || string.IsNullOrWhiteSpace(publicPuzzleHashProfile) || string.IsNullOrWhiteSpace(publicPuzzleHashSha256) || attemptId == Guid.Empty || (int)mode < 0 || (int)mode > 3 || hintCount < 0)
                return DomainResult<PuzzleSessionState>.Fail(DomainError.INVALID_DEFINITION);
            // Profile dispatch and hash verification belong to the future adapter.
            return DomainResult<PuzzleSessionState>.Ok(new PuzzleSessionState(definition, puzzleId!, publicPuzzleHashProfile!, publicPuzzleHashSha256!, attemptId, mode, hintCount, startedAtUtc, lastSavedAtUtc,
                new CellContent[definition.Grid.CellCount], new bool[definition.Grid.CellCount], Array.Empty<CellDiff>(), Array.Empty<Guid>(), SessionPhase.Ready, 0, false, 0, 0, false, false, 0, null));
        }
    }
    /// <summary>Closed command envelope. IDs and revisions come from the caller.</summary>
    public abstract class PuzzleCommand
    {
        public Guid CommandId { get; }
        public long ExpectedRevision { get; }
        private protected PuzzleCommand(Guid commandId, long expectedRevision) { CommandId = commandId; ExpectedRevision = expectedRevision; }
    }
    /// <summary>Apply one visible cell content.</summary>
    public sealed class ApplyCellContent : PuzzleCommand
    {
        public CellCoordinate? Coordinate { get; }
        public CellContent Content { get; }
        public ApplyCellContent(Guid id, long revision, CellCoordinate? coordinate, CellContent content) : base(id, revision) { Coordinate = coordinate; Content = content; }
    }
    /// <summary>Apply one tool to a defensively copied, unique coordinate batch.</summary>
    public sealed class ApplyCellContentBatch : PuzzleCommand
    {
        public IReadOnlyList<CellCoordinate>? Coordinates { get; }
        public CellContent Content { get; }
        public ApplyCellContentBatch(Guid id, long revision, IReadOnlyList<CellCoordinate>? coordinates, CellContent content) : base(id, revision) { Coordinates = coordinates == null ? null : Array.AsReadOnly(coordinates.ToArray()); Content = content; }
    }
    /// <summary>Undo the most recent atomic cell action; history flags remain sticky.</summary>
    public sealed class UndoLastAction : PuzzleCommand { public UndoLastAction(Guid id, long revision) : base(id, revision) { } }
    /// <summary>Accumulate active time and suspend; performs no save operation.</summary>
    public sealed class PauseForLifecycle : PuzzleCommand { public PauseForLifecycle(Guid id, long revision) : base(id, revision) { } }
    /// <summary>Resume a suspended timer at a new monotonic interval.</summary>
    public sealed class ResumeFromLifecycle : PuzzleCommand { public ResumeFromLifecycle(Guid id, long revision) : base(id, revision) { } }
    /// <summary>Domain facts emitted in cell, diagnostic, completion order.</summary>
    public enum PuzzleEventKind { CellContentChanged, CellBatchChanged, ObjectiveViolationChanged, PuzzleSolved, SessionSuspended }
    /// <summary>Immutable local fact, with completion path only for PuzzleSolved.</summary>
    public sealed class PuzzleEvent
    {
        public PuzzleEventKind Kind { get; }
        public string? Id { get; }
        public IReadOnlyList<CellChange> Changes { get; }
        public IReadOnlyList<CellCoordinate> Path { get; }
        internal PuzzleEvent(PuzzleEventKind kind, IEnumerable<CellChange>? changes = null, string? id = null, IEnumerable<CellCoordinate>? path = null)
        { Kind = kind; Id = id; Changes = Array.AsReadOnly(changes?.ToArray() ?? Array.Empty<CellChange>()); Path = Array.AsReadOnly(path?.ToArray() ?? Array.Empty<CellCoordinate>()); }
    }
    /// <summary>A new snapshot or the unchanged rejected/no-op snapshot, with diagnostics.</summary>
    public sealed class CommandResult
    {
        public PuzzleSessionState? State { get; }
        public DomainError Error { get; }
        public IReadOnlyList<PuzzleEvent> Events { get; }
        public IReadOnlyList<PuzzleDiagnostic> Diagnostics { get; }
        internal CommandResult(PuzzleSessionState? state, DomainError error, IEnumerable<PuzzleEvent>? events = null)
        { State = state; Error = error; Events = Array.AsReadOnly(events?.ToArray() ?? Array.Empty<PuzzleEvent>()); Diagnostics = PuzzleEvaluator.Evaluate(state?.Definition, state?.Cells).Diagnostics; }
    }
    /// <summary>Pure serial command reducer; all time is explicitly injected in milliseconds.</summary>
    public static class PuzzleCommandHandler
    {
        public static CommandResult Handle(PuzzleSessionState? state, PuzzleCommand? command, long monotonicTime)
        {
            CommandResult Fail(DomainError error) => new CommandResult(state, error);
            if (state == null) return Fail(DomainError.INVALID_DEFINITION);
            if (command == null || command.CommandId == Guid.Empty) return Fail(DomainError.INVALID_COMMAND);
            var s = state;
            if (command.ExpectedRevision != s.Revision) return Fail(DomainError.STALE_COMMAND);
            if (s.AppliedCommandIds.Contains(command.CommandId)) return Fail(DomainError.DUPLICATE_COMMAND);
            bool resume = command is ResumeFromLifecycle;
            if (resume ? s.Phase != SessionPhase.Suspended : s.Phase != SessionPhase.Ready && s.Phase != SessionPhase.Active) return Fail(DomainError.SESSION_CLOSED);
            if (monotonicTime < 0 || monotonicTime < s.LastMonotonicMs) return Fail(DomainError.INVALID_TIME);
            if (s.Revision == long.MaxValue) return Fail(DomainError.INTERNAL_INVARIANT_BROKEN);
            bool lifecycle = resume || command is PauseForLifecycle;
            var changes = new List<CellChange>(); var cells = s.Cells.ToArray(); var history = s.PreviouslySet.ToArray(); var undo = s.UndoStack.ToList(); bool isUndo = command is UndoLastAction;
            if (isUndo)
            {
                if (undo.Count == 0) return Fail(DomainError.NONE);
                foreach (var change in undo[undo.Count - 1].Changes) changes.Add(new CellChange(change.Coordinate, change.After, change.Before));
                undo.RemoveAt(undo.Count - 1);
            }
            else if (!lifecycle)
            {
                IReadOnlyList<CellCoordinate?>? coordinates; CellContent content;
                if (command is ApplyCellContent single) { coordinates = new[] { single.Coordinate }; content = single.Content; }
                else if (command is ApplyCellContentBatch batch) { coordinates = batch.Coordinates; content = batch.Content; }
                else return Fail(DomainError.INVALID_COMMAND);
                if (!TrackGeometry.IsContent(content) || coordinates == null || coordinates.Count < 1 || coordinates.Count > s.Definition.Grid.CellCount) return Fail(DomainError.INVALID_COMMAND);
                var unique = new HashSet<int>();
                foreach (var coordinate in coordinates)
                {
                    if (coordinate == null || !s.Definition.Grid.Contains(coordinate.X, coordinate.Y)) return Fail(DomainError.OUT_OF_BOUNDS);
                    int index = s.Definition.Index(coordinate);
                    if (!unique.Add(index)) return Fail(DomainError.INVALID_COMMAND);
                    if (cells[index] != content) changes.Add(new CellChange(coordinate, cells[index], content));
                }
                if (changes.Count == 0) return Fail(DomainError.NONE);
                changes = changes.OrderBy(c => c.Coordinate.Y).ThenBy(c => c.Coordinate.X).ToList();
                undo.Add(new CellDiff(changes)); if (undo.Count > 256) undo.RemoveAt(0);
            }
            bool empty = s.UsedEmptyMarker, occupied = s.UsedOccupiedMarker; long corrections = s.CorrectionCount;
            foreach (var change in changes)
            {
                int index = s.Definition.Index(change.Coordinate); cells[index] = change.After;
                if (!isUndo)
                {
                    if (history[index]) corrections++;
                    history[index] |= change.After != CellContent.UNSET;
                    empty |= change.After == CellContent.MARK_EMPTY; occupied |= change.After == CellContent.MARK_OCCUPIED;
                }
            }
            bool started = s.TimerStarted || changes.Count > 0; long elapsed = s.ActiveElapsedMs;
            if (s.Phase == SessionPhase.Active && s.TimerStarted)
            {
                long delta = monotonicTime - s.LastMonotonicMs;
                if (delta > long.MaxValue - elapsed) return Fail(DomainError.INVALID_TIME);
                elapsed += delta;
            }
            var evaluation = PuzzleEvaluator.Evaluate(s.Definition, cells);
            var oldEvaluation = PuzzleEvaluator.Evaluate(s.Definition, s.Cells);
            var events = new List<PuzzleEvent>(); string? solvedId = s.SolvedEventId;
            SessionPhase phase = resume ? (started ? SessionPhase.Active : SessionPhase.Ready) : command is PauseForLifecycle ? SessionPhase.Suspended : started ? SessionPhase.Active : SessionPhase.Ready;
            if (changes.Count > 0)
            {
                events.Add(new PuzzleEvent(command is ApplyCellContent ? PuzzleEventKind.CellContentChanged : PuzzleEventKind.CellBatchChanged, changes));
                if (!evaluation.Diagnostics.Select(d => (d.Coordinate.Y, d.Coordinate.X, d.Code)).SequenceEqual(oldEvaluation.Diagnostics.Select(d => (d.Coordinate.Y, d.Coordinate.X, d.Code)))) events.Add(new PuzzleEvent(PuzzleEventKind.ObjectiveViolationChanged));
                if (evaluation.Completed && solvedId == null) { solvedId = "puzzle-solved:" + s.AttemptId.ToString("D"); phase = SessionPhase.SolvedPendingCommit; events.Add(new PuzzleEvent(PuzzleEventKind.PuzzleSolved, id: solvedId, path: evaluation.Path)); }
            }
            if (command is PauseForLifecycle) events.Add(new PuzzleEvent(PuzzleEventKind.SessionSuspended));
            var ids = s.AppliedCommandIds.Concat(new[] { command.CommandId }).OrderBy(id => id).ToArray();
            var next = new PuzzleSessionState(s.Definition, s.PuzzleId, s.PublicPuzzleHashProfile, s.PublicPuzzleHashSha256, s.AttemptId, s.Mode, s.HintCount, s.StartedAtUtc, s.LastSavedAtUtc,
                cells, history, undo.ToArray(), ids, phase, s.Revision + 1, started, elapsed, monotonicTime, empty, occupied, corrections, solvedId);
            return new CommandResult(next, DomainError.NONE, events);
        }
    }
}
