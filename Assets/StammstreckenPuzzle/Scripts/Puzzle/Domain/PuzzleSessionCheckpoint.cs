using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace STP.Puzzle.Domain
{
    /// <summary>One immutable serialized cell transition; coordinates are checked against the definition on restore.</summary>
    public sealed class CheckpointCellChange
    {
        /// <summary>Copies a real transition, rejecting unsupported values and technically invalid coordinates.</summary>
        public CheckpointCellChange(int x, int y, CellContent before, CellContent after)
        {
            if (x < 0 || x >= 32 || y < 0 || y >= 32 || !TrackGeometry.IsContent(before) ||
                !TrackGeometry.IsContent(after) || before == after) throw new ArgumentException("CHECKPOINT_DIFF_INVALID");
            X = x; Y = y; Before = before; After = after;
        }
        /// <summary>Zero-based column.</summary>
        public int X { get; }
        /// <summary>Zero-based row.</summary>
        public int Y { get; }
        /// <summary>Content before this action.</summary>
        public CellContent Before { get; }
        /// <summary>Content after this action.</summary>
        public CellContent After { get; }
    }

    /// <summary>A copied, sorted, unique and nonempty atomic undo action.</summary>
    public sealed class CheckpointCellDiff
    {
        /// <summary>Requires existing row/column order rather than silently repairing malformed saved history.</summary>
        public CheckpointCellDiff(IReadOnlyList<CheckpointCellChange> changes)
        {
            if (changes == null || changes.Count == 0 || changes.Count > 1024) throw new ArgumentException("CHECKPOINT_DIFF_INVALID");
            var copy = new CheckpointCellChange[changes.Count]; int previous = -1;
            for (int i = 0; i < copy.Length; i++)
            {
                var c = changes[i] ?? throw new ArgumentException("CHECKPOINT_DIFF_INVALID");
                int key = c.Y * 32 + c.X;
                if (key <= previous) throw new ArgumentException("CHECKPOINT_DIFF_ORDER");
                previous = key; copy[i] = c;
            }
            Changes = Array.AsReadOnly(copy);
        }
        /// <summary>Immutable row-major changes.</summary>
        public IReadOnlyList<CheckpointCellChange> Changes { get; }
    }

    /// <summary>A complete immutable attempt checkpoint, without clock anchors, view state or authoring solutions.</summary>
    public sealed class PuzzleSessionCheckpoint
    {
        private const long MaxSafe = 9007199254740991L;
        private const string SemanticProfile = "STP-PUZZLE-SEMANTIC-JCS-1";

        /// <summary>Copies all attempt truth. Definition-dependent invariants are checked by Restore before state construction.</summary>
        public PuzzleSessionCheckpoint(string puzzleId, string publicPuzzleHashProfile, string publicPuzzleHashSha256,
            string rulesetVersion, Guid attemptId, SessionMode mode, SessionPhase phase,
            IReadOnlyList<CellContent> cells, IReadOnlyList<bool> previouslySet, long revision,
            bool timerStarted, long activeElapsedMs, bool usedEmptyMarker, bool usedOccupiedMarker,
            int hintCount, long correctionCount, IReadOnlyList<Guid> appliedCommandIds,
            IReadOnlyList<CheckpointCellDiff> undoStack, string? solvedEventId,
            DateTimeOffset? startedAtUtc, DateTimeOffset? lastSavedAtUtc)
        {
            if (string.IsNullOrWhiteSpace(puzzleId) || publicPuzzleHashProfile != SemanticProfile || !IsHash(publicPuzzleHashSha256) ||
                rulesetVersion != "train-track-v1" || attemptId == Guid.Empty || !Enum.IsDefined(typeof(SessionMode), mode) ||
                (phase != SessionPhase.Ready && phase != SessionPhase.Active && phase != SessionPhase.Suspended && phase != SessionPhase.SolvedPendingCommit))
                throw new ArgumentException("CHECKPOINT_IDENTITY_OR_PHASE_INVALID");
            Safe(revision); Safe(activeElapsedMs); Safe(correctionCount);
            if (hintCount < 0 || cells == null || cells.Count < 4 || cells.Count > 1024 || previouslySet == null || previouslySet.Count != cells.Count ||
                appliedCommandIds == null || appliedCommandIds.Count != revision || undoStack == null || undoStack.Count > 256 || undoStack.Count > revision || correctionCount > revision * cells.Count)
                throw new ArgumentException("CHECKPOINT_LENGTH_OR_COUNTER_INVALID");
            var cc = cells.ToArray(); var pp = previouslySet.ToArray();
            if (cc.Any(c => !TrackGeometry.IsContent(c))) throw new ArgumentException("CHECKPOINT_CELL_INVALID");
            for (int i = 0; i < cc.Length; i++)
                if (cc[i] != CellContent.UNSET && !pp[i]) throw new ArgumentException("CHECKPOINT_HISTORY_INVALID");
            var ids = appliedCommandIds.ToArray();
            for (int i = 0; i < ids.Length; i++)
                if (ids[i] == Guid.Empty || (i > 0 && ids[i - 1].CompareTo(ids[i]) >= 0)) throw new ArgumentException("CHECKPOINT_COMMAND_IDS_INVALID");
            var undo = undoStack.ToArray();
            if (undo.Any(d => d == null)) throw new ArgumentException("CHECKPOINT_DIFF_INVALID");
            bool markerEmpty = cc.Contains(CellContent.MARK_EMPTY) || undo.Any(d => d.Changes.Any(c => c.Before == CellContent.MARK_EMPTY || c.After == CellContent.MARK_EMPTY));
            bool markerOccupied = cc.Contains(CellContent.MARK_OCCUPIED) || undo.Any(d => d.Changes.Any(c => c.Before == CellContent.MARK_OCCUPIED || c.After == CellContent.MARK_OCCUPIED));
            if ((markerEmpty && !usedEmptyMarker) || (markerOccupied && !usedOccupiedMarker) ||
                correctionCount < undo.Sum(d => (long)d.Changes.Count(c => c.Before != CellContent.UNSET)))
                throw new ArgumentException("CHECKPOINT_STICKY_HISTORY_INVALID");
            // Historical inputs may carry an initial opaque HintCount; new displayed hints are real commands.
            // Accept the original lifecycle parity or the parity after those hint commands, preserving v2 validity.
            bool lifecycleParity = phase == SessionPhase.Ready ? revision % 2 == 0 : phase == SessionPhase.Suspended && revision % 2 == 1;
            bool hintLifecycleParity = hintCount <= revision && (phase == SessionPhase.Ready ? (revision-hintCount)%2==0 : phase == SessionPhase.Suspended && (revision-hintCount)%2==1);
            if ((timerStarted && (revision == 0 || !pp.Any(p => p))) || (!timerStarted && !lifecycleParity && !hintLifecycleParity) || (!timerStarted && (activeElapsedMs != 0 || cc.Any(c => c != CellContent.UNSET) || pp.Any(p => p) || undo.Length > 0 || usedEmptyMarker || usedOccupiedMarker || correctionCount != 0)) ||
                (phase == SessionPhase.Ready && timerStarted) || (phase == SessionPhase.Active && !timerStarted))
                throw new ArgumentException("CHECKPOINT_TIMER_INVALID");
            if ((phase == SessionPhase.SolvedPendingCommit) != (solvedEventId != null) ||
                (solvedEventId != null && (solvedEventId != "puzzle-solved:" + attemptId.ToString("D") || !timerStarted)))
                throw new ArgumentException("CHECKPOINT_SOLVED_ID_INVALID");
            if ((startedAtUtc.HasValue && startedAtUtc.Value.Offset != TimeSpan.Zero) || (lastSavedAtUtc.HasValue && lastSavedAtUtc.Value.Offset != TimeSpan.Zero))
                throw new ArgumentException("CHECKPOINT_UTC_INVALID");
            PuzzleId = puzzleId; PublicPuzzleHashProfile = publicPuzzleHashProfile; PublicPuzzleHashSha256 = publicPuzzleHashSha256;
            RulesetVersion = rulesetVersion; AttemptId = attemptId; Mode = mode; Phase = phase; Cells = Array.AsReadOnly(cc); PreviouslySet = Array.AsReadOnly(pp);
            Revision = revision; TimerStarted = timerStarted; ActiveElapsedMs = activeElapsedMs; UsedEmptyMarker = usedEmptyMarker; UsedOccupiedMarker = usedOccupiedMarker;
            HintCount = hintCount; CorrectionCount = correctionCount; AppliedCommandIds = Array.AsReadOnly(ids); UndoStack = Array.AsReadOnly(undo);
            SolvedEventId = solvedEventId; StartedAtUtc = startedAtUtc; LastSavedAtUtc = lastSavedAtUtc; StartedAtUtcText = Diagnostic(startedAtUtc); LastSavedAtUtcText = Diagnostic(lastSavedAtUtc);
        }

        /// <summary>Bound puzzle ID.</summary>
        public string PuzzleId { get; }
        /// <summary>Existing semantic hash profile.</summary>
        public string PublicPuzzleHashProfile { get; }
        /// <summary>Bound semantic puzzle hash.</summary>
        public string PublicPuzzleHashSha256 { get; }
        /// <summary>Bound public ruleset.</summary>
        public string RulesetVersion { get; }
        /// <summary>Unchanged attempt UUID.</summary>
        public Guid AttemptId { get; }
        /// <summary>Original attempt mode.</summary>
        public SessionMode Mode { get; }
        /// <summary>Saved domain phase, independent of presentation.</summary>
        public SessionPhase Phase { get; }
        /// <summary>Complete copied row-major content.</summary>
        public IReadOnlyList<CellContent> Cells { get; }
        /// <summary>Sticky per-cell historical occupancy.</summary>
        public IReadOnlyList<bool> PreviouslySet { get; }
        /// <summary>Unchanged domain revision.</summary>
        public long Revision { get; }
        /// <summary>Whether a real state-changing cell action began the timer.</summary>
        public bool TimerStarted { get; }
        /// <summary>Materialized active elapsed time, excluding process downtime.</summary>
        public long ActiveElapsedMs { get; }
        /// <summary>Sticky empty-marker history.</summary>
        public bool UsedEmptyMarker { get; }
        /// <summary>Sticky occupied-marker history.</summary>
        public bool UsedOccupiedMarker { get; }
        /// <summary>Preserved displayed-hint count, without enabling hint policy.</summary>
        public int HintCount { get; }
        /// <summary>Preserved genuine correction count.</summary>
        public long CorrectionCount { get; }
        /// <summary>Complete sorted unique successful-command deduplication truth.</summary>
        public IReadOnlyList<Guid> AppliedCommandIds { get; }
        /// <summary>Complete retained atomic undo stack, oldest first.</summary>
        public IReadOnlyList<CheckpointCellDiff> UndoStack { get; }
        /// <summary>Deterministic solved ID, only while awaiting commit.</summary>
        public string? SolvedEventId { get; }
        /// <summary>Original optional UTC diagnostic.</summary>
        public DateTimeOffset? StartedAtUtc { get; }
        /// <summary>Original optional UTC diagnostic.</summary>
        public DateTimeOffset? LastSavedAtUtc { get; }

        /// <summary>Original strict UTC spelling retained by a save reader, or a normalized diagnostic for a new domain export.</summary>
        public string? StartedAtUtcText {get;private set;}
        /// <summary>Original strict UTC spelling retained by a save reader, or a normalized diagnostic for a new domain export.</summary>
        public string? LastSavedAtUtcText {get;private set;}
        private static string? Diagnostic(DateTimeOffset? value)=>value?.ToString("yyyy-MM-dd'T'HH:mm:ss.FFFFFFF'Z'",CultureInfo.InvariantCulture);
        /// <summary>Returns a new checkpoint retaining exact validated UTC diagnostic spellings for the same instants; never mutates this checkpoint.</summary>
        public PuzzleSessionCheckpoint WithDiagnosticText(string? startedAtUtcText,string? lastSavedAtUtcText)
        {
            void Check(string? text,DateTimeOffset? instant)
            {
                if(text==null){if(instant.HasValue)throw new ArgumentException("CHECKPOINT_UTC_INVALID");return;}
                if(!instant.HasValue||!System.Text.RegularExpressions.Regex.IsMatch(text,@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d{1,7})?Z$")||!DateTimeOffset.TryParse(text,CultureInfo.InvariantCulture,DateTimeStyles.None,out var parsed)||parsed.Offset!=TimeSpan.Zero||parsed!=instant.Value)throw new ArgumentException("CHECKPOINT_UTC_INVALID");
            }
            Check(startedAtUtcText,StartedAtUtc);Check(lastSavedAtUtcText,LastSavedAtUtc);
            var copy=new PuzzleSessionCheckpoint(PuzzleId,PublicPuzzleHashProfile,PublicPuzzleHashSha256,RulesetVersion,AttemptId,Mode,Phase,Cells,PreviouslySet,Revision,TimerStarted,ActiveElapsedMs,UsedEmptyMarker,UsedOccupiedMarker,HintCount,CorrectionCount,AppliedCommandIds,UndoStack,SolvedEventId,StartedAtUtc,LastSavedAtUtc);
            copy.StartedAtUtcText=startedAtUtcText;copy.LastSavedAtUtcText=lastSavedAtUtcText;return copy;
        }
        /// <summary>Materializes running time at an explicit monotonic instant without mutating revision or producing a command.</summary>
        public static PuzzleSessionCheckpoint Export(PuzzleSessionState state, long monotonicMilliseconds)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (monotonicMilliseconds < 0 || monotonicMilliseconds < state.LastMonotonicMs) throw new ArgumentException("CHECKPOINT_CLOCK_INVALID");
            long elapsed = state.ActiveElapsedMs;
            if (state.Phase == SessionPhase.Active && state.TimerStarted)
            {
                long delta = monotonicMilliseconds - state.LastMonotonicMs;
                if (delta > MaxSafe - elapsed) throw new ArgumentException("CHECKPOINT_TIME_OVERFLOW");
                elapsed += delta;
            }
            return new PuzzleSessionCheckpoint(state.PuzzleId, state.PublicPuzzleHashProfile, state.PublicPuzzleHashSha256, state.Definition.RulesetVersion,
                state.AttemptId, state.Mode, state.Phase, state.Cells, state.PreviouslySet, state.Revision, state.TimerStarted, elapsed,
                state.UsedEmptyMarker, state.UsedOccupiedMarker, state.HintCount, state.CorrectionCount, state.AppliedCommandIds,
                state.UndoStack.Select(d => new CheckpointCellDiff(d.Changes.Select(c => new CheckpointCellChange(c.Coordinate.X,c.Coordinate.Y,c.Before,c.After)).ToArray())).ToArray(),
                state.SolvedEventId, state.StartedAtUtc, state.LastSavedAtUtc);
        }

        /// <summary>Validates exact expected identity, definition, reverse undo replay and real completion before constructing a suspended/restored state.</summary>
        public DomainResult<PuzzleSessionState> Restore(PuzzleDefinition definition, string expectedPuzzleId, string expectedProfile, string expectedHash)
        {
            if (definition == null || PuzzleId != expectedPuzzleId || PublicPuzzleHashProfile != expectedProfile || PublicPuzzleHashSha256 != expectedHash ||
                RulesetVersion != definition.RulesetVersion || Cells.Count != definition.Grid.CellCount)
                return DomainResult<PuzzleSessionState>.Fail(DomainError.INVALID_DEFINITION);
            var replay = Cells.ToArray(); var domainDiffs = new List<CellDiff>();
            foreach (var diff in UndoStack)
            {
                var changes = new List<CellChange>();
                foreach (var change in diff.Changes)
                {
                    if (!definition.Grid.Contains(change.X, change.Y) || !PreviouslySet[change.Y * definition.Grid.Width + change.X])
                        return DomainResult<PuzzleSessionState>.Fail(DomainError.INVALID_DEFINITION);
                    changes.Add(new CellChange(CellCoordinate.Create(definition.Grid,change.X,change.Y).Value!,change.Before,change.After));
                }
                domainDiffs.Add(new CellDiff(changes));
            }
            for (int i = UndoStack.Count - 1; i >= 0; i--)
                foreach (var change in UndoStack[i].Changes)
                {
                    int index = change.Y * definition.Grid.Width + change.X;
                    if (replay[index] != change.After) return DomainResult<PuzzleSessionState>.Fail(DomainError.INVALID_DEFINITION);
                    replay[index] = change.Before;
                }
            bool completed = PuzzleEvaluator.Evaluate(definition, Cells).Completed;
            if (completed != (Phase == SessionPhase.SolvedPendingCommit)) return DomainResult<PuzzleSessionState>.Fail(DomainError.INVALID_DEFINITION);
            var restoredPhase = Phase == SessionPhase.Active ? SessionPhase.Suspended : Phase;
            return DomainResult<PuzzleSessionState>.Ok(new PuzzleSessionState(definition, PuzzleId, PublicPuzzleHashProfile, PublicPuzzleHashSha256,
                AttemptId, Mode, HintCount, StartedAtUtc, LastSavedAtUtc, Cells.ToArray(), PreviouslySet.ToArray(), domainDiffs.ToArray(), AppliedCommandIds.ToArray(),
                restoredPhase, Revision, TimerStarted, ActiveElapsedMs, 0, UsedEmptyMarker, UsedOccupiedMarker, CorrectionCount, SolvedEventId));
        }

        private static void Safe(long value) { if (value < 0 || value > MaxSafe) throw new ArgumentException("CHECKPOINT_INTEGER_INVALID"); }
        private static bool IsHash(string value) => value != null && value.Length == 64 && value.All(c => c >= '0' && c <= '9' || c >= 'a' && c <= 'f');
    }
}
