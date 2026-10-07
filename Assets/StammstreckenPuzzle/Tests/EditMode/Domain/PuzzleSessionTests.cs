using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NUnit.Framework;
using STP.Puzzle.Domain;

namespace STP.Tests.Domain.EditMode
{
    public sealed class PuzzleSessionTests
    {
        private static CommandResult Apply(PuzzleSessionState s, int id, int x, int y, CellContent content, long time = 0) => PuzzleCommandHandler.Handle(s, new ApplyCellContent(Fixtures.Id(id), s.Revision, Fixtures.Cell(x, y), content), time);

        [Test]
        public void NoOp_IsIdenticalWithoutTimerRevisionUndoOrEvents()
        {
            var s = Fixtures.Session(); var r = Apply(s, 1, 0, 0, CellContent.UNSET, 50);
            Assert.That(r.State!, Is.SameAs(s)); Assert.That(r.Events, Is.Empty); Assert.That(r.Error, Is.EqualTo(DomainError.NONE));
            Assert.That(s.TimerStarted, Is.False); Assert.That(s.Revision, Is.Zero); Assert.That(s.UndoStack, Is.Empty);
            var emptyUndo = PuzzleCommandHandler.Handle(s, new UndoLastAction(Fixtures.Id(2), 0), 50);
            Assert.That(emptyUndo.State!, Is.SameAs(s)); Assert.That(emptyUndo.Events, Is.Empty);
            r = PuzzleCommandHandler.Handle(s, new ApplyCellContentBatch(Fixtures.Id(3), 0, new[] { Fixtures.Cell(1, 0), Fixtures.Cell(0, 0) }, CellContent.UNSET), 50);
            Assert.That(r.State!, Is.SameAs(s)); Assert.That(r.Events, Is.Empty);
        }

        [Test]
        public void Batch_IsAtomicSortedImmutableAndHasOneUndoStep()
        {
            var s = Apply(Fixtures.Session(), 1, 1, 0, CellContent.MARK_EMPTY).State!;
            var coords = new[] { Fixtures.Cell(1, 0), Fixtures.Cell(0, 0) };
            var command = new ApplyCellContentBatch(Fixtures.Id(2), 1, coords, CellContent.MARK_EMPTY);
            coords[0] = Fixtures.Cell(1, 1);
            var r = PuzzleCommandHandler.Handle(s, command, 10);
            Assert.That(r.State!.Cells, Is.EqualTo(new[] { CellContent.MARK_EMPTY, CellContent.MARK_EMPTY, CellContent.UNSET, CellContent.UNSET }));
            Assert.That(s.Cells[0], Is.EqualTo(CellContent.UNSET));
            Assert.That(r.State!.UndoStack.Count, Is.EqualTo(2)); Assert.That(r.State!.UndoStack.Last().Changes.Count, Is.EqualTo(1));
            Assert.That(r.Events[0].Kind, Is.EqualTo(PuzzleEventKind.CellBatchChanged));
            Assert.That(r.Events[0].Changes[0].Coordinate.X, Is.Zero);
            Assert.Throws<NotSupportedException>(() => ((IList<CellContent>)r.State!.Cells)[0] = CellContent.UNSET);
            var undone = PuzzleCommandHandler.Handle(r.State!, new UndoLastAction(Fixtures.Id(3), 2), 20).State!;
            Assert.That(undone.Cells, Is.EqualTo(s.Cells)); Assert.That(undone.UsedEmptyMarker, Is.True); Assert.That(undone.TimerStarted, Is.True);
            Assert.That(undone.Revision, Is.EqualTo(3)); Assert.That(undone.ActiveElapsedMs, Is.EqualTo(20));
            foreach (var invalid in new IReadOnlyList<CellCoordinate>?[] { null, Array.Empty<CellCoordinate>(), new[] { Fixtures.Cell(0, 0), Fixtures.Cell(0, 0) }, new[] { Fixtures.Cell(0, 0), Fixtures.Cell(2, 0, Fixtures.Size(3, 2)) } })
            {
                var bad = PuzzleCommandHandler.Handle(s, new ApplyCellContentBatch(Fixtures.Id(4), 1, invalid, CellContent.MARK_OCCUPIED), 10);
                Assert.That(bad.Error, Is.Not.EqualTo(DomainError.NONE)); Assert.That(bad.State!, Is.SameAs(s)); Assert.That(bad.Events, Is.Empty);
            }
            var full = PuzzleCommandHandler.Handle(Fixtures.Session(), new ApplyCellContentBatch(Fixtures.Id(8), 0, new[] { Fixtures.Cell(1, 1), Fixtures.Cell(0, 0), Fixtures.Cell(1, 0), Fixtures.Cell(0, 1) }, CellContent.MARK_OCCUPIED), 0);
            Assert.That(full.State!.UndoStack[0].Changes.Select(d => (d.Coordinate.Y, d.Coordinate.X)), Is.Ordered);
        }

        [Test]
        public void Envelopes_RejectStaleDuplicatesInvalidContentAndCoordinates()
        {
            var initial = Fixtures.Session(); var s = Apply(initial, 1, 0, 0, CellContent.MARK_OCCUPIED).State!;
            var duplicate = PuzzleCommandHandler.Handle(s, new ApplyCellContent(Fixtures.Id(1), 1, Fixtures.Cell(0, 0), CellContent.TRACK_NS), 1);
            Assert.That(duplicate.Error, Is.EqualTo(DomainError.DUPLICATE_COMMAND)); Assert.That(duplicate.State!, Is.SameAs(s));
            var stale = PuzzleCommandHandler.Handle(s, new ApplyCellContent(Fixtures.Id(2), 0, Fixtures.Cell(0, 0), CellContent.TRACK_NS), 1);
            Assert.That(stale.Error, Is.EqualTo(DomainError.STALE_COMMAND)); Assert.That(stale.State!, Is.SameAs(s));
            foreach (var cmd in new PuzzleCommand?[] { null, new ApplyCellContent(Guid.Empty, 1, Fixtures.Cell(0, 0), CellContent.UNSET), new ApplyCellContent(Fixtures.Id(3), 1, null, CellContent.UNSET), new ApplyCellContent(Fixtures.Id(3), 1, Fixtures.Cell(0, 0), (CellContent)99) })
                Assert.That(PuzzleCommandHandler.Handle(s, cmd, 1).State!, Is.SameAs(s));
            Assert.That(Apply(s, 4, 0, 0, CellContent.UNSET, -1).Error, Is.EqualTo(DomainError.INVALID_TIME));
            Assert.That(PuzzleCommandHandler.Handle(null, null, 0).Error, Is.EqualTo(DomainError.INVALID_DEFINITION));
        }

        [Test]
        public void ClearCorrectionsStickyFlagsAndUndoCap_AreHistorical()
        {
            var s = Fixtures.Session(hints: 3);
            s = Apply(s, 1, 0, 0, CellContent.MARK_EMPTY).State!;
            s = Apply(s, 2, 0, 0, CellContent.UNSET, 1).State!;
            s = Apply(s, 3, 0, 0, CellContent.MARK_OCCUPIED, 2).State!;
            Assert.That(s.CorrectionCount, Is.EqualTo(2)); Assert.That(s.UsedEmptyMarker && s.UsedOccupiedMarker, Is.True);
            var cleared = Apply(s, 90, 0, 0, CellContent.UNSET, 3).State!;
            Assert.That(cleared.UsedOccupiedMarker && cleared.UsedEmptyMarker, Is.True);
            Assert.That(cleared.PuzzleId, Is.EqualTo("FIXTURE_ONLY"));
            s = PuzzleCommandHandler.Handle(s, new UndoLastAction(Fixtures.Id(4), 3), 3).State!;
            Assert.That(s.HintCount, Is.EqualTo(3)); Assert.That(s.CorrectionCount, Is.EqualTo(2)); Assert.That(s.UsedOccupiedMarker, Is.True);
            s = Fixtures.Session();
            for (int i = 1; i <= 257; i++) s = Apply(s, i, 0, 0, i % 2 == 1 ? CellContent.MARK_EMPTY : CellContent.MARK_OCCUPIED, i).State!;
            Assert.That(s.UndoStack.Count, Is.EqualTo(256)); Assert.That(s.CorrectionCount, Is.EqualTo(256));
            for (int i = 258; i <= 513; i++) s = PuzzleCommandHandler.Handle(s, new UndoLastAction(Fixtures.Id(i), s.Revision), i).State!;
            Assert.That(s.UndoStack, Is.Empty); Assert.That(s.Cells[0], Is.EqualTo(CellContent.MARK_EMPTY)); Assert.That(s.Revision, Is.EqualTo(513));
        }

        [Test]
        public void Lifecycle_AccumulatesOnlyActivePeriodsAndClosesAtFirstSolution()
        {
            var s = Fixtures.Session();
            s = PuzzleCommandHandler.Handle(s, new PauseForLifecycle(Fixtures.Id(1), 0), 100).State!;
            Assert.That(s.Phase, Is.EqualTo(SessionPhase.Suspended)); Assert.That(s.TimerStarted, Is.False);
            Assert.That(Apply(s, 8, 0, 0, CellContent.MARK_EMPTY, 200).Error, Is.EqualTo(DomainError.SESSION_CLOSED));
            s = PuzzleCommandHandler.Handle(s, new ResumeFromLifecycle(Fixtures.Id(2), 1), 300).State!;
            Assert.That(s.Phase, Is.EqualTo(SessionPhase.Ready));
            s = Apply(s, 3, 0, 0, CellContent.TRACK_EW, 400).State!;
            s = PuzzleCommandHandler.Handle(s, new PauseForLifecycle(Fixtures.Id(4), 3), 450).State!;
            Assert.That(s.ActiveElapsedMs, Is.EqualTo(50));
            Assert.That(PuzzleCommandHandler.Handle(s, new ResumeFromLifecycle(Fixtures.Id(5), 4), 449).Error, Is.EqualTo(DomainError.INVALID_TIME));
            s = PuzzleCommandHandler.Handle(s, new ResumeFromLifecycle(Fixtures.Id(5), 4), 1000).State!;
            var solved = Apply(s, 6, 1, 0, CellContent.TRACK_EW, 1020);
            Assert.That(solved.State!.ActiveElapsedMs, Is.EqualTo(70)); Assert.That(solved.State!.Phase, Is.EqualTo(SessionPhase.SolvedPendingCommit));
            Assert.That(solved.Events.Count(e => e.Kind == PuzzleEventKind.PuzzleSolved), Is.EqualTo(1));
            Assert.That(solved.State!.SolvedEventId, Is.EqualTo("puzzle-solved:" + Fixtures.Id(22).ToString("D")));
            Assert.That(Apply(solved.State!, 7, 0, 0, CellContent.UNSET, 1030).Error, Is.EqualTo(DomainError.SESSION_CLOSED));
            Assert.That(PuzzleCommandHandler.Handle(solved.State!, new UndoLastAction(Fixtures.Id(7), solved.State!.Revision), 1030).State!, Is.SameAs(solved.State!));
            Assert.That(PuzzleCommandHandler.Handle(solved.State!, new PauseForLifecycle(Fixtures.Id(7), solved.State!.Revision), 1030).State!, Is.SameAs(solved.State!));
            Assert.That(PuzzleCommandHandler.Handle(s, new ResumeFromLifecycle(Fixtures.Id(9), s.Revision), 1020).Error, Is.EqualTo(DomainError.SESSION_CLOSED));
            Assert.That(solved.State!.PublicPuzzleHashSha256, Is.EqualTo(new string('a', 64))); Assert.That(solved.State!.LastSavedAtUtc, Is.Null);
        }

        [Test]
        public void SessionFactory_RejectsInvalidExternalIdentityAndRetainsModes()
        {
            foreach (SessionMode mode in Enum.GetValues(typeof(SessionMode)))
            {
                var r = PuzzleSessionState.Create(Fixtures.Horizontal(), "p", "externally-supplied", "opaque-value", Fixtures.Id(1), mode, 2, null, null);
                Assert.That(r.Succeeded, Is.True); Assert.That(r.Value!.Mode, Is.EqualTo(mode));
            }
            foreach (var input in new[] { ("", "hash", "value", Fixtures.Id(1), SessionMode.FIRST_RUN, 0), ("p", "", "value", Fixtures.Id(1), SessionMode.FIRST_RUN, 0), ("p", "hash", "", Fixtures.Id(1), SessionMode.FIRST_RUN, 0), ("p", "hash", "value", Guid.Empty, SessionMode.FIRST_RUN, 0), ("p", "hash", "value", Fixtures.Id(1), (SessionMode)99, 0), ("p", "hash", "value", Fixtures.Id(1), SessionMode.FIRST_RUN, -1) })
                Assert.That(PuzzleSessionState.Create(Fixtures.Horizontal(), input.Item1, input.Item2, input.Item3, input.Item4, input.Item5, input.Item6, null, null).Succeeded, Is.False);
        }

        private static byte[] Project(CommandResult r)
        {
            // Explicit FIXTURE_ONLY projection, neither a save format nor JCS.
            var s = r.State!; var b = new StringBuilder();
            b.Append(s.PuzzleId).Append('|').Append(s.PublicPuzzleHashProfile).Append('|').Append(s.PublicPuzzleHashSha256).Append('|').Append(s.AttemptId.ToString("D"));
            foreach (var n in new long[] { s.Revision, s.ActiveElapsedMs, s.HintCount, s.CorrectionCount }) b.Append('|').Append(n.ToString(CultureInfo.InvariantCulture));
            b.Append('|').Append(s.Mode).Append('|').Append(s.Phase).Append('|').Append(s.TimerStarted).Append('|').Append(s.UsedEmptyMarker).Append('|').Append(s.UsedOccupiedMarker).Append('|').Append(s.SolvedEventId);
            foreach (var c in s.Cells) b.Append('|').Append(c);
            foreach (var diff in s.UndoStack) foreach (var c in diff.Changes) b.Append('|').Append(c.Coordinate.X).Append(',').Append(c.Coordinate.Y).Append(':').Append(c.Before).Append('>').Append(c.After);
            foreach (var d in r.Diagnostics) b.Append('|').Append(d.Coordinate.X).Append(',').Append(d.Coordinate.Y).Append(':').Append(d.Code);
            foreach (var e in r.Events) { b.Append('|').Append(e.Kind).Append(':').Append(e.Id); foreach (var c in e.Changes) b.Append(':').Append(c.Coordinate.X).Append(',').Append(c.Coordinate.Y).Append(':').Append(c.Before).Append('>').Append(c.After); }
            return Encoding.UTF8.GetBytes(b.ToString());
        }

        [TestCase(2201)] [TestCase(2202)] [TestCase(2203)]
        public void FixedSeedProperty_ReplaysIdenticalSnapshotsEventsAndRevisions(int seed)
        {
            var random = new Random(seed); var a = Fixtures.Session(); var b = Fixtures.Session();
            for (int i = 1; i <= 500; i++)
            {
                PuzzleCommand cmd = i % 7 == 0 ? new UndoLastAction(Fixtures.Id(i), a.Revision) : new ApplyCellContent(Fixtures.Id(i), a.Revision, Fixtures.Cell(random.Next(2), random.Next(2)), (CellContent)random.Next(3));
                var ra = PuzzleCommandHandler.Handle(a, cmd, i * 5); var rb = PuzzleCommandHandler.Handle(b, cmd, i * 5);
                Assert.That(Project(ra), Is.EqualTo(Project(rb)), $"seed={seed}; first failing/minimal replay prefix={i}; command={cmd.GetType().Name}");
                Assert.That(ra.State!.UndoStack.Count, Is.LessThanOrEqualTo(256));
                Assert.That(ra.State!.Revision, Is.InRange(a.Revision, a.Revision + 1)); a = ra.State!; b = rb.State!;
            }
        }
    }
}
