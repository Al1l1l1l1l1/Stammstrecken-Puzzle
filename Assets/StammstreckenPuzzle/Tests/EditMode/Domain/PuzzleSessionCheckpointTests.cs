using System;
using System.Linq;
using NUnit.Framework;
using STP.Puzzle.Domain;

namespace STP.Tests.Domain.EditMode
{
    public sealed class PuzzleSessionCheckpointTests
    {
        [TestCase(2)][TestCase(4)][TestCase(10)]
        public void FullRasterAnd256UndoKeepEverySuccessfulCommandIdAcrossRestore(int side)
        {
            var grid=Fixtures.Size(side,side);var rows=Enumerable.Range(0,side).Select(y=>y==0?side:0).ToArray();var definition=PuzzleDefinition.Create(grid,Fixtures.Edge(Direction.W,0,grid),Fixtures.Edge(Direction.E,0,grid),rows,Enumerable.Repeat(1,side).ToArray(),"train-track-v1").Value!;var state=Fixtures.Session(definition);var coordinates=Enumerable.Range(0,side*side).Select(i=>Fixtures.Cell(i%side,i/side,grid)).ToArray();for(int i=1;i<=300;i++)state=PuzzleCommandHandler.Handle(state,new ApplyCellContentBatch(Fixtures.Id(i),state.Revision,coordinates,i%2==0?CellContent.MARK_OCCUPIED:CellContent.MARK_EMPTY),i).State!;var cp=PuzzleSessionCheckpoint.Export(state,301);Assert.That(cp.Cells.Count,Is.EqualTo(side*side));Assert.That(cp.UndoStack.Count,Is.EqualTo(256));Assert.That(cp.UndoStack.All(d=>d.Changes.Count==side*side),Is.True);Assert.That(cp.AppliedCommandIds.Count,Is.EqualTo(300));Assert.That(cp.CorrectionCount,Is.EqualTo(299*side*side));var restored=cp.Restore(definition,cp.PuzzleId,cp.PublicPuzzleHashProfile,cp.PublicPuzzleHashSha256).Value!;restored=PuzzleCommandHandler.Handle(restored,new ResumeFromLifecycle(Fixtures.Id(301),restored.Revision),100000).State!;Assert.That(PuzzleCommandHandler.Handle(restored,new ApplyCellContent(Fixtures.Id(1),restored.Revision,coordinates[0],CellContent.UNSET),100001).Error,Is.EqualTo(DomainError.DUPLICATE_COMMAND));for(int i=302;i<558;i++)restored=PuzzleCommandHandler.Handle(restored,new UndoLastAction(Fixtures.Id(i),restored.Revision),100000+i).State!;Assert.That(restored.UndoStack,Is.Empty);Assert.That(restored.Revision,Is.EqualTo(557));Assert.That(PuzzleSessionCheckpoint.Export(restored,101000).AppliedCommandIds.Count,Is.EqualTo(557));
        }
        [Test]
        public void CheckpointAndNestedDiffInputsAreDefensivelyCopiedAndCannotExposeWritableLists()
        {
            var original=PuzzleSessionCheckpoint.Export(Apply(Fixtures.Session(),1,CellContent.MARK_EMPTY,0),1);var cells=original.Cells.ToArray();var history=original.PreviouslySet.ToArray();var ids=original.AppliedCommandIds.ToArray();var changes=new[]{new CheckpointCellChange(0,0,CellContent.UNSET,CellContent.MARK_EMPTY)};var diff=new CheckpointCellDiff(changes);var undo=new[]{diff};var cp=new PuzzleSessionCheckpoint(original.PuzzleId,original.PublicPuzzleHashProfile,original.PublicPuzzleHashSha256,original.RulesetVersion,original.AttemptId,original.Mode,original.Phase,cells,history,original.Revision,original.TimerStarted,original.ActiveElapsedMs,true,false,0,0,ids,undo,null,original.StartedAtUtc,original.LastSavedAtUtc);cells[0]=CellContent.TRACK_NS;history[0]=false;ids[0]=Guid.NewGuid();changes[0]=new CheckpointCellChange(1,1,CellContent.UNSET,CellContent.TRACK_EW);undo[0]=new CheckpointCellDiff(changes);Assert.That(cp.Cells[0],Is.EqualTo(CellContent.MARK_EMPTY));Assert.That(cp.PreviouslySet[0],Is.True);Assert.That(cp.AppliedCommandIds[0],Is.EqualTo(Fixtures.Id(1)));Assert.That(cp.UndoStack[0].Changes[0].X,Is.Zero);Assert.Throws<NotSupportedException>(()=>((System.Collections.Generic.IList<CellContent>)cp.Cells)[0]=CellContent.UNSET);Assert.Throws<NotSupportedException>(()=>((System.Collections.Generic.IList<CheckpointCellChange>)cp.UndoStack[0].Changes).Clear());
        }
        [Test]
        public void SolvedCheckpointMustMatchActualEvaluatorAndExactSolvedEventIdentity()
        {
            var s=Fixtures.Session();s=PuzzleCommandHandler.Handle(s,new ApplyCellContentBatch(Fixtures.Id(1),0,new[]{Fixtures.Cell(0,0),Fixtures.Cell(1,0)},CellContent.TRACK_EW),100).State!;var cp=PuzzleSessionCheckpoint.Export(s,100);Assert.That(cp.Phase,Is.EqualTo(SessionPhase.SolvedPendingCommit));Assert.That(cp.Restore(s.Definition,s.PuzzleId,s.PublicPuzzleHashProfile,s.PublicPuzzleHashSha256).Value!.SolvedEventId,Is.EqualTo("puzzle-solved:"+s.AttemptId.ToString("D")));Assert.Throws<ArgumentException>(()=>Copy(cp,SessionPhase.Active,true,cp.Revision,cp.AppliedCommandIds,cp.UndoStack,0));var active=new PuzzleSessionCheckpoint(cp.PuzzleId,cp.PublicPuzzleHashProfile,cp.PublicPuzzleHashSha256,cp.RulesetVersion,cp.AttemptId,cp.Mode,SessionPhase.Active,cp.Cells,cp.PreviouslySet,cp.Revision,true,cp.ActiveElapsedMs,false,false,0,0,cp.AppliedCommandIds,cp.UndoStack,null,cp.StartedAtUtc,cp.LastSavedAtUtc);Assert.That(active.Restore(s.Definition,s.PuzzleId,s.PublicPuzzleHashProfile,s.PublicPuzzleHashSha256).Succeeded,Is.False);
        }
        private static PuzzleSessionState Apply(PuzzleSessionState s,int id,CellContent content,long time)=>PuzzleCommandHandler.Handle(s,new ApplyCellContent(Fixtures.Id(id),s.Revision,Fixtures.Cell(0,0),content),time).State!;
        [Test]
        public void CompleteCheckpoint_RestoresStickyHistoryAndUsesNewClockAnchor()
        {
            var s=Apply(Fixtures.Session(),1,CellContent.MARK_EMPTY,100);
            s=Apply(s,2,CellContent.UNSET,200);s=Apply(s,3,CellContent.MARK_OCCUPIED,300);
            s=PuzzleCommandHandler.Handle(s,new UndoLastAction(Fixtures.Id(4),s.Revision),400).State!;
            var cp=PuzzleSessionCheckpoint.Export(s,500);
            Assert.That(cp.ActiveElapsedMs,Is.EqualTo(400));Assert.That(s.ActiveElapsedMs,Is.EqualTo(300));
            var restored=cp.Restore(s.Definition,s.PuzzleId,s.PublicPuzzleHashProfile,s.PublicPuzzleHashSha256).Value!;
            Assert.That(restored.Phase,Is.EqualTo(SessionPhase.Suspended));Assert.That(restored.Cells,Is.EqualTo(s.Cells));
            Assert.That(restored.Revision,Is.EqualTo(4));Assert.That(restored.CorrectionCount,Is.EqualTo(2));
            Assert.That(restored.UsedEmptyMarker&&restored.UsedOccupiedMarker,Is.True);Assert.That(restored.UndoStack.Count,Is.EqualTo(2));
            restored=PuzzleCommandHandler.Handle(restored,new ResumeFromLifecycle(Fixtures.Id(5),restored.Revision),100000).State!;
            restored=Apply(restored,6,CellContent.MARK_EMPTY,100050);
            Assert.That(restored.ActiveElapsedMs,Is.EqualTo(450));
            Assert.That(PuzzleCommandHandler.Handle(restored,new ApplyCellContent(Fixtures.Id(1),restored.Revision,Fixtures.Cell(0,0),CellContent.UNSET),100060).Error,Is.EqualTo(DomainError.DUPLICATE_COMMAND));
        }
        [Test]
        public void Constructor_RejectsImpossibleTimerUndoAndCorrectionHistories()
        {
            var s=Fixtures.Session();var cp=PuzzleSessionCheckpoint.Export(s,0);
            Assert.Throws<ArgumentException>(()=>Copy(cp,SessionPhase.Active,true,0,Array.Empty<Guid>(),Array.Empty<CheckpointCellDiff>(),0));
            var diff=new CheckpointCellDiff(new[]{new CheckpointCellChange(0,0,CellContent.UNSET,CellContent.MARK_EMPTY)});
            Assert.Throws<ArgumentException>(()=>Copy(cp,SessionPhase.Ready,false,0,Array.Empty<Guid>(),new[]{diff},0));
            var c=PuzzleSessionCheckpoint.Export(Apply(s,1,CellContent.MARK_EMPTY,0),1);
            Assert.Throws<ArgumentException>(()=>Copy(c,c.Phase,true,1,c.AppliedCommandIds,c.UndoStack,5));
            Assert.Throws<ArgumentException>(()=>Copy(cp,SessionPhase.Suspended,false,0,Array.Empty<Guid>(),Array.Empty<CheckpointCellDiff>(),0));
            Assert.Throws<ArgumentException>(()=>Copy(cp,SessionPhase.Ready,false,1,new[]{Fixtures.Id(1)},Array.Empty<CheckpointCellDiff>(),0));
        }
        [Test]
        public void Restore_RejectsWrongIdentityDefinitionAndUndoReplay()
        {
            var s=Apply(Fixtures.Session(),1,CellContent.MARK_EMPTY,0);var cp=PuzzleSessionCheckpoint.Export(s,1);
            Assert.That(cp.Restore(s.Definition,s.PuzzleId,s.PublicPuzzleHashProfile,new string('b',64)).Succeeded,Is.False);
            var bad=new CheckpointCellDiff(new[]{new CheckpointCellChange(0,0,CellContent.UNSET,CellContent.TRACK_EW)});
            var malformed=Copy(cp,cp.Phase,true,1,cp.AppliedCommandIds,new[]{bad},0);
            Assert.That(malformed.Restore(s.Definition,s.PuzzleId,s.PublicPuzzleHashProfile,s.PublicPuzzleHashSha256).Succeeded,Is.False);
        }
        [Test]
        public void LifecycleBeforeInput_PreservesRealParityWithoutStartingTimer()
        {
            var s=Fixtures.Session();s=PuzzleCommandHandler.Handle(s,new PauseForLifecycle(Fixtures.Id(1),0),10).State!;
            var cp=PuzzleSessionCheckpoint.Export(s,20);Assert.That(cp.TimerStarted,Is.False);
            Assert.That(cp.Restore(s.Definition,s.PuzzleId,s.PublicPuzzleHashProfile,s.PublicPuzzleHashSha256).Succeeded,Is.True);
            s=PuzzleCommandHandler.Handle(s,new ResumeFromLifecycle(Fixtures.Id(2),1),30).State!;
            Assert.That(PuzzleSessionCheckpoint.Export(s,40).Phase,Is.EqualTo(SessionPhase.Ready));
        }
        private static PuzzleSessionCheckpoint Copy(PuzzleSessionCheckpoint c,SessionPhase phase,bool started,long revision,System.Collections.Generic.IReadOnlyList<Guid> ids,System.Collections.Generic.IReadOnlyList<CheckpointCellDiff> undo,long corrections)=>new PuzzleSessionCheckpoint(c.PuzzleId,c.PublicPuzzleHashProfile,c.PublicPuzzleHashSha256,c.RulesetVersion,c.AttemptId,c.Mode,phase,c.Cells,c.PreviouslySet,revision,started,c.ActiveElapsedMs,c.UsedEmptyMarker,c.UsedOccupiedMarker,c.HintCount,corrections,ids,undo,c.SolvedEventId,c.StartedAtUtc,c.LastSavedAtUtc);
    }
}
