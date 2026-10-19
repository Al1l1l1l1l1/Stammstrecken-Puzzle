using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using STP.Application;

namespace STP.Tests.Application.EditMode
{
    public sealed class Public240ReviewSessionTests
    {
        // These two small definitions are tests-only, never a runtime fallback or public-catalog entry.
        private static Public240ReviewSession Create(string id,int w,int h,DevelopmentEdge a,int ai,DevelopmentEdge b,int bi,int[] rows,int[] columns)
            => new Public240ReviewSession(new PublicPuzzleInput(new PublicPuzzleIdentity(id, PublicPuzzleIdentity.SemanticHashProfile,new string('a',64)),
                "train-track-v1",w,h,(PublicPuzzleSide)a,ai,(PublicPuzzleSide)b,bi,rows,columns));

        private static Public240ReviewSession Horizontal() => Create(
            "qa-app-horizontal", 2, 2, DevelopmentEdge.W, 0, DevelopmentEdge.E, 0, new[] { 2, 0 }, new[] { 1, 1 });

        [Test]
        public void Interface_UsesFullActualPublicIdentity_AndNullIsRejected()
        {
            var input=new PublicPuzzleInput(new PublicPuzzleIdentity("S1-01-01-01",PublicPuzzleIdentity.SemanticHashProfile,"5f7aa091e1fe38d06673878eb1b2f58d4731dea895c855fbbf9f080b1222307d"),
                "train-track-v1",4,4,PublicPuzzleSide.E,2,PublicPuzzleSide.N,0,new[]{4,3,1,0},new[]{2,2,1,3});
            IReviewPuzzleSession session=new Public240ReviewSession(input);
            Assert.That(session.Puzzle.Identity,Is.EqualTo(input.Identity));
            Assert.That(session.Puzzle,Is.SameAs(input));
            Assert.That(session.Width,Is.EqualTo(4));
            Assert.Throws<ArgumentNullException>(()=>new Public240ReviewSession(null!));
        }

        [Test]
        public void PublicInput_IsCopied_AndInitialReadsDoNotStartTimer()
        {
            var rows = new[] { 2, 0 }; var columns = new[] { 1, 1 };
            var session = Create("qa-copy", 2, 2, DevelopmentEdge.W, 0, DevelopmentEdge.E, 0, rows, columns);
            rows[0] = 0; columns[0] = 0;
            Assert.That(session.RowTarget(0), Is.EqualTo(2)); Assert.That(session.ColumnTarget(0), Is.EqualTo(1));
            Assert.That(session.EndpointA.Edge, Is.EqualTo(DevelopmentEdge.W)); Assert.That(session.EndpointB.X, Is.EqualTo(1));
            Assert.That(session.CellAt(0, 0), Is.EqualTo(DevelopmentTool.Clear)); Assert.That(session.Width, Is.EqualTo(2)); Assert.That(session.Height, Is.EqualTo(2));
            Assert.That(session.Phase, Is.EqualTo(DevelopmentPreviewPhase.Editing)); Assert.That(session.CaptureCommand().AttemptId, Is.EqualTo(session.AttemptId));
            Assert.That(session.Revision, Is.Zero); Assert.That(session.ElapsedAt(100000), Is.Zero); Assert.That(session.TimerStarted, Is.False);
            Assert.That(session.BuiltPath, Is.Empty); Assert.That(session.IsAwaitingSaveCommit, Is.False);
            Assert.That(session.Puzzle.Identity.HashProfile, Is.EqualTo(PublicPuzzleIdentity.SemanticHashProfile));
            Assert.That(session.Puzzle.Identity.PuzzleId, Is.EqualTo("qa-copy"));
        }

        [TestCase(DevelopmentTool.Clear)] [TestCase(DevelopmentTool.EmptyMarker)] [TestCase(DevelopmentTool.OccupiedMarker)]
        [TestCase(DevelopmentTool.TrackNS)] [TestCase(DevelopmentTool.TrackEW)] [TestCase(DevelopmentTool.TrackNE)]
        [TestCase(DevelopmentTool.TrackES)] [TestCase(DevelopmentTool.TrackSW)] [TestCase(DevelopmentTool.TrackWN)]
        public void AllNineToolContents_MapExactlyWithoutCycling(DevelopmentTool tool)
        {
            var session = Horizontal();
            session.Apply(0, 1, tool, 10);
            Assert.That(session.CellAt(0, 1), Is.EqualTo(tool));
            long revision = session.Revision;
            Assert.That(session.Apply(0, 1, tool, 20), Is.EqualTo(DevelopmentCommandStatus.Unchanged));
            Assert.That(session.Revision, Is.EqualTo(revision));
        }

        [Test]
        public void EmptyAndNoOpInputs_KeepTimerAndUndoOff()
        {
            var session = Horizontal();
            Assert.That(session.Apply(0, 0, DevelopmentTool.Clear, 100), Is.EqualTo(DevelopmentCommandStatus.Unchanged));
            Assert.That(session.Undo(200), Is.EqualTo(DevelopmentCommandStatus.Unchanged));
            Assert.That(session.TimerStarted, Is.False); Assert.That(session.CanUndo, Is.False); Assert.That(session.Revision, Is.Zero);
        }

        [Test]
        public void Batch_IsAtomicAndOneUndoStep_WithStickyMarkerHistory()
        {
            var session = Horizontal();
            Assert.That(session.ApplyBatch(new[] { new DevelopmentCell(1, 0), new DevelopmentCell(0, 0) }, DevelopmentTool.OccupiedMarker, 50), Is.EqualTo(DevelopmentCommandStatus.Applied));
            Assert.That(session.RowVisible(0), Is.EqualTo(2)); Assert.That(session.ColumnVisible(0), Is.EqualTo(1));
            Assert.That(session.Phase, Is.EqualTo(DevelopmentPreviewPhase.Editing)); Assert.That(session.BuiltPath, Is.Empty);
            Assert.That(session.Undo(70), Is.EqualTo(DevelopmentCommandStatus.Applied));
            Assert.That(session.CellAt(0, 0), Is.EqualTo(DevelopmentTool.Clear)); Assert.That(session.CellAt(1, 0), Is.EqualTo(DevelopmentTool.Clear));
            Assert.That(session.CanUndo, Is.False); Assert.That(session.UsedOccupiedMarker, Is.True); Assert.That(session.TimerStarted, Is.True);
            Assert.That(session.ElapsedAt(80), Is.EqualTo(30));
            foreach (var bad in new IReadOnlyList<DevelopmentCell>?[] { null, Array.Empty<DevelopmentCell>(),
                new[] { new DevelopmentCell(0, 0), new DevelopmentCell(9, 0) }, new[] { new DevelopmentCell(0, 0), new DevelopmentCell(0, 0) } })
            {
                long revision = session.Revision;
                Assert.That(session.ApplyBatch(bad, DevelopmentTool.EmptyMarker, 90), Is.EqualTo(DevelopmentCommandStatus.Rejected));
                Assert.That(session.Revision, Is.EqualTo(revision)); Assert.That(session.CellAt(0, 0), Is.EqualTo(DevelopmentTool.Clear));
            }
        }

        [Test]
        public void VisibleCounts_AreOnlyVisibleAssumptions_AndClearRestoresNeutralContent()
        {
            var session = Horizontal();
            session.Apply(0, 1, DevelopmentTool.EmptyMarker, 0);
            Assert.That(session.RowVisible(1), Is.Zero); Assert.That(session.HasObjectiveViolations, Is.False);
            session.Apply(0, 1, DevelopmentTool.OccupiedMarker, 10);
            Assert.That(session.RowVisible(1), Is.EqualTo(1)); Assert.That(session.ColumnVisible(0), Is.EqualTo(1)); Assert.That(session.HasObjectiveViolations, Is.True);
            session.Apply(0, 1, DevelopmentTool.Clear, 20);
            Assert.That(session.RowVisible(1), Is.Zero); Assert.That(session.HasObjectiveViolations, Is.False);
            Assert.That(session.UsedEmptyMarker && session.UsedOccupiedMarker, Is.True); Assert.That(session.CorrectionCount, Is.EqualTo(2));
        }

        [Test]
        public void StaleDuplicateAndInvalidCommands_UseReducerIdentityAndNeverMutateAgain()
        {
            var session = Horizontal(); var envelope = session.CaptureCommand();
            Assert.That(session.Apply(0, 0, DevelopmentTool.EmptyMarker, 10, envelope), Is.EqualTo(DevelopmentCommandStatus.Applied));
            Assert.That(session.Apply(1, 0, DevelopmentTool.EmptyMarker, 20, envelope), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(session.LastErrorCode, Is.EqualTo("STALE_COMMAND"));
            var duplicate = new DevelopmentCommandEnvelope(session.AttemptId, envelope.CommandId, session.Revision);
            Assert.That(session.Apply(1, 0, DevelopmentTool.EmptyMarker, 20, duplicate), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(session.LastErrorCode, Is.EqualTo("DUPLICATE_COMMAND"));
            Assert.That(session.CellAt(1, 0), Is.EqualTo(DevelopmentTool.Clear)); Assert.That(session.Revision, Is.EqualTo(1));
            Assert.That(session.Apply(-1, 0, DevelopmentTool.EmptyMarker, 30), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(session.LastErrorCode, Is.EqualTo("OUT_OF_BOUNDS"));
            Assert.That(session.Apply(0, 0, (DevelopmentTool)99, 30), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(session.LastErrorCode, Is.EqualTo("INVALID_COMMAND"));
            Assert.That(session.Apply(0, 0, DevelopmentTool.Clear, 9), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(session.LastErrorCode, Is.EqualTo("INVALID_TIME"));
        }

        [Test]
        public void LifecycleAndOverview_ExcludeBackgroundTimeAndRetainTheSameAttempt()
        {
            var session = Horizontal(); Guid attempt = session.AttemptId;
            session.ShowOverview(100); Assert.That(session.IsPaused, Is.True); Assert.That(session.TimerStarted, Is.False);
            session.ReturnToPuzzle(300); Assert.That(session.IsPaused, Is.False); Assert.That(session.TimerStarted, Is.False);
            session.Apply(0, 0, DevelopmentTool.EmptyMarker, 400);
            Assert.That(session.ElapsedAt(420), Is.EqualTo(20));
            session.Pause(450); Assert.That(session.ElapsedAt(900), Is.EqualTo(50));
            Assert.That(session.Apply(1, 0, DevelopmentTool.EmptyMarker, 900), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(session.Pause(950), Is.EqualTo(DevelopmentCommandStatus.Unchanged));
            session.Resume(1000); Assert.That(session.ElapsedAt(1020), Is.EqualTo(70));
            session.ShowOverview(1050); Assert.That(session.Phase, Is.EqualTo(DevelopmentPreviewPhase.Overview));
            Assert.That(session.Resume(2000), Is.EqualTo(DevelopmentCommandStatus.Unchanged)); Assert.That(session.IsPaused, Is.True);
            Assert.That(session.ReturnToPuzzle(1049), Is.EqualTo(DevelopmentCommandStatus.Rejected)); Assert.That(session.Phase, Is.EqualTo(DevelopmentPreviewPhase.Overview));
            session.ReturnToPuzzle(2050); Assert.That(session.ElapsedAt(2070), Is.EqualTo(120));
            Assert.That(session.AttemptId, Is.EqualTo(attempt)); Assert.That(session.CellAt(0, 0), Is.EqualTo(DevelopmentTool.EmptyMarker)); Assert.That(session.CanUndo, Is.True);
        }

        [Test]
        public void Completion_UsesActualPlayerPath_LocksInputs_AndResultsNeverCommitDomain()
        {
            var session = Horizontal(); Guid attempt = session.AttemptId;
            Assert.That(session.CompleteTrainRide(), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            session.Apply(0, 0, DevelopmentTool.TrackEW, 100);
            Assert.That(session.BuiltPath, Is.Empty); Assert.That(session.Phase, Is.EqualTo(DevelopmentPreviewPhase.Editing));
            session.Apply(1, 0, DevelopmentTool.TrackEW, 150);
            Assert.That(session.Phase, Is.EqualTo(DevelopmentPreviewPhase.TrainRide)); Assert.That(session.IsAwaitingSaveCommit, Is.True);
            Assert.That(session.BuiltPath.Select(cell => (cell.X, cell.Y)), Is.EqualTo(new[] { (0, 0), (1, 0) }));
            Assert.Throws<NotSupportedException>(() => ((IList<DevelopmentCell>)session.BuiltPath)[0] = new DevelopmentCell(1, 1));
            Assert.That(session.Apply(0, 0, DevelopmentTool.Clear, 200), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(session.Undo(200), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(session.Restart(), Is.EqualTo(DevelopmentCommandStatus.Rejected)); Assert.That(session.ShowOverview(200), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(session.ElapsedAt(10000), Is.EqualTo(50));
            Assert.That(session.CompleteTrainRide(attempt), Is.EqualTo(DevelopmentCommandStatus.Applied));
            Assert.That(session.CompleteTrainRide(attempt), Is.EqualTo(DevelopmentCommandStatus.Unchanged));
            Assert.That(session.Phase, Is.EqualTo(DevelopmentPreviewPhase.Results)); Assert.That(session.IsAwaitingSaveCommit, Is.True);
            session.ShowOverview(10000); session.ReturnToPuzzle(20000);
            Assert.That(session.Phase, Is.EqualTo(DevelopmentPreviewPhase.Results)); Assert.That(session.ElapsedAt(30000), Is.EqualTo(50));
        }

        [Test]
        public void Restart_CreatesFreshIdentity_AndRejectsOldInputAndRideCallbacks()
        {
            var session = Horizontal(); var oldEnvelope = session.CaptureCommand(); Guid oldAttempt = session.AttemptId;
            session.ApplyBatch(new[] { new DevelopmentCell(0, 0), new DevelopmentCell(1, 0) }, DevelopmentTool.TrackEW, 100);
            session.CompleteTrainRide(oldAttempt); var oldPath = session.BuiltPath;
            Assert.That(session.Restart(), Is.EqualTo(DevelopmentCommandStatus.Applied));
            Assert.That(session.AttemptId, Is.Not.EqualTo(oldAttempt)); Assert.That(session.Phase, Is.EqualTo(DevelopmentPreviewPhase.Editing));
            Assert.That(session.TimerStarted, Is.False); Assert.That(session.Revision, Is.Zero); Assert.That(session.BuiltPath, Is.Empty); Assert.That(oldPath.Count, Is.EqualTo(2));
            Assert.That(session.Apply(0, 0, DevelopmentTool.EmptyMarker, 200, oldEnvelope), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(session.LastErrorCode, Is.EqualTo("STALE_ATTEMPT")); Assert.That(session.Revision, Is.Zero);
            session.ApplyBatch(new[] { new DevelopmentCell(0, 0), new DevelopmentCell(1, 0) }, DevelopmentTool.TrackEW, 200);
            Assert.That(session.CompleteTrainRide(oldAttempt), Is.EqualTo(DevelopmentCommandStatus.Rejected));
            Assert.That(session.Phase, Is.EqualTo(DevelopmentPreviewPhase.TrainRide));
            Assert.That(session.CompleteTrainRide(session.AttemptId), Is.EqualTo(DevelopmentCommandStatus.Applied));
        }

        [Test]
        public void CurvedCompletionPath_IsCopiedFromAThroughThePlayersEntireRoute()
        {
            var session = Create("qa-app-turns", 3, 3, DevelopmentEdge.W, 0, DevelopmentEdge.E, 2, new[] { 3, 3, 3 }, new[] { 3, 3, 3 });
            var cells = new[] { new DevelopmentCell(0, 0), new DevelopmentCell(1, 0), new DevelopmentCell(2, 0),
                new DevelopmentCell(2, 1), new DevelopmentCell(1, 1), new DevelopmentCell(0, 1),
                new DevelopmentCell(0, 2), new DevelopmentCell(1, 2), new DevelopmentCell(2, 2) };
            var tools = new[] { DevelopmentTool.TrackEW, DevelopmentTool.TrackEW, DevelopmentTool.TrackSW,
                DevelopmentTool.TrackWN, DevelopmentTool.TrackEW, DevelopmentTool.TrackES,
                DevelopmentTool.TrackNE, DevelopmentTool.TrackEW, DevelopmentTool.TrackEW };
            for (int i = 0; i < cells.Length; i++) session.Apply(cells[i].X, cells[i].Y, tools[i], i * 10);
            Assert.That(session.Phase, Is.EqualTo(DevelopmentPreviewPhase.TrainRide));
            Assert.That(session.BuiltPath.Select(cell => (cell.X, cell.Y)), Is.EqualTo(cells.Select(cell => (cell.X, cell.Y))));
            Assert.That(session.ElapsedAt(1000), Is.EqualTo(80));
        }

        [Test]
        public void InvalidPublicConfiguration_FailsThroughDomainValidation()
        {
            Assert.Throws<ArgumentException>(() => Create("qa-invalid", 1, 2, DevelopmentEdge.W, 0, DevelopmentEdge.E, 0, new[] { 2, 0 }, new[] { 1, 1 }));
            Assert.Throws<ArgumentException>(() => Create("qa-invalid", 2, 2, DevelopmentEdge.W, 0, DevelopmentEdge.W, 0, new[] { 2, 0 }, new[] { 1, 1 }));
            Assert.Throws<ArgumentException>(() => Create("qa-invalid", 2, 2, DevelopmentEdge.W, 0, DevelopmentEdge.E, 0, new[] { 2, 0 }, new[] { 0, 0 }));
            Assert.Throws<ArgumentException>(() => Create("", 2, 2, DevelopmentEdge.W, 0, DevelopmentEdge.E, 0, new[] { 2, 0 }, new[] { 1, 1 }));
        }
    }
}
