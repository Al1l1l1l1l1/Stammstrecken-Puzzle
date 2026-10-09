using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using STP.Puzzle.Domain;

namespace STP.Tests.Domain.EditMode
{
    internal static class Fixtures
    {
        internal static GridSize Size(int w = 2, int h = 2) => GridSize.Create(w, h).Value!;
        internal static CellCoordinate Cell(int x, int y, GridSize? size = null) => CellCoordinate.Create(size ?? Size(), x, y).Value!;
        internal static Endpoint Edge(Direction side, int index, GridSize? size = null) => Endpoint.Create(size ?? Size(), side, index).Value!;
        internal static PuzzleDefinition Horizontal() => PuzzleDefinition.Create(Size(), Edge(Direction.W, 0), Edge(Direction.E, 0), new[] { 2, 0 }, new[] { 1, 1 }, "train-track-v1").Value!;
        internal static PuzzleDefinition Single() => PuzzleDefinition.Create(Size(), Edge(Direction.N, 0), Edge(Direction.W, 0), new[] { 1, 0 }, new[] { 1, 0 }, "train-track-v1").Value!;
        internal static Guid Id(int i) => new Guid(i, 0, 0, new byte[8]);
        internal static PuzzleSessionState Session(PuzzleDefinition? definition = null, int hints = 0) => PuzzleSessionState.Create(definition ?? Horizontal(), "FIXTURE_ONLY", "STP-PUZZLE-SEMANTIC-JCS-1", new string('a', 64), Id(22), SessionMode.FIRST_RUN, hints, new DateTimeOffset(2026, 10, 7, 0, 0, 0, TimeSpan.Zero), null).Value!;
    }

    public sealed class PuzzleDomainTests
    {
        [TestCase(1, 2)] [TestCase(2, 1)] [TestCase(33, 2)] [TestCase(2, 33)] [TestCase(-1, -1)]
        public void Grid_RejectsEachInvalidAxis(int w, int h) => Assert.That(GridSize.Create(w, h).Error, Is.EqualTo(DomainError.INVALID_DEFINITION));

        [Test]
        public void Values_AreClosedAndRelativeToTheirRaster()
        {
            Assert.That(Fixtures.Size(32, 32).CellCount, Is.EqualTo(1024));
            var size = Fixtures.Size(2, 3);
            Assert.That(CellCoordinate.Create(size, 2, 0).Error, Is.EqualTo(DomainError.OUT_OF_BOUNDS));
            Assert.That(CellCoordinate.Create(size, 0, -1).Succeeded, Is.False);
            Assert.That(CellCoordinate.Create(size, -1, 0).Succeeded, Is.False);
            Assert.That(CellCoordinate.Create(size, 0, 3).Succeeded, Is.False);
            Assert.That(CellCoordinate.Create(null, 0, 0).Succeeded, Is.False);
            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                int length = d == Direction.N || d == Direction.S ? 2 : 3;
                Assert.That(Endpoint.Create(size, d, length).Succeeded, Is.False);
                Assert.That(Endpoint.Create(size, d, -1).Succeeded, Is.False);
                Assert.That(TrackGeometry.Opposite(TrackGeometry.Opposite(d)), Is.EqualTo(d));
            }
            Assert.That(Endpoint.Create(size, (Direction)99, 0).Succeeded, Is.False);
            Assert.That(Endpoint.Create(size, (Direction)(-1), 0).Succeeded, Is.False);
            Assert.That(Endpoint.Create(null, Direction.N, 0).Succeeded, Is.False);
            Assert.That(Enum.GetValues(typeof(CellContent)).Length, Is.EqualTo(9));
            Assert.That(Enum.GetValues(typeof(TrackShape)).Length, Is.EqualTo(6));
            int[] masks = { 5, 10, 3, 6, 12, 9 };
            for (int i = 0; i < 6; i++)
            {
                Assert.That(TrackGeometry.Ports((CellContent)(i + 3)), Is.EqualTo(masks[i]));
                Assert.That(TrackGeometry.Ports((TrackShape)i), Is.EqualTo(masks[i]));
            }
            Assert.That(TrackGeometry.Ports((CellContent)99), Is.Zero);
            Assert.That(TrackGeometry.IsContent((CellContent)(-1)), Is.False);
            Assert.That(Fixtures.Horizontal().Index(null), Is.EqualTo(-1));
            Assert.That(Fixtures.Horizontal().Index(Fixtures.Cell(2, 0, Fixtures.Size(3, 2))), Is.EqualTo(-1));
        }

        [Test]
        public void Definition_RejectsEveryInvariantAndDefendsCollections()
        {
            var s = Fixtures.Size(); var a = Fixtures.Edge(Direction.W, 0); var b = Fixtures.Edge(Direction.E, 0);
            DomainError Create(GridSize? grid, Endpoint? start, Endpoint? end, int[]? rows, int[]? cols, string rules = "train-track-v1") => PuzzleDefinition.Create(grid, start, end, rows, cols, rules).Error;
            Assert.That(Create(null, a, b, new[] { 2, 0 }, new[] { 1, 1 }), Is.EqualTo(DomainError.INVALID_DEFINITION));
            Assert.That(Create(s, null, b, new[] { 2, 0 }, new[] { 1, 1 }), Is.EqualTo(DomainError.INVALID_DEFINITION));
            Assert.That(Create(s, a, null, new[] { 2, 0 }, new[] { 1, 1 }), Is.EqualTo(DomainError.INVALID_DEFINITION));
            Assert.That(Create(s, a, a, new[] { 2, 0 }, new[] { 1, 1 }), Is.EqualTo(DomainError.INVALID_DEFINITION));
            Assert.That(Create(s, Fixtures.Edge(Direction.N, 2, Fixtures.Size(3, 2)), b, new[] { 2, 0 }, new[] { 1, 1 }), Is.EqualTo(DomainError.INVALID_DEFINITION));
            foreach (var rows in new int[]?[] { null, new[] { 2 }, new[] { -1, 2 }, new[] { 3, 0 }, new[] { 1, 0 }, new[] { 0, 0 }, new[] { 0, 2 } })
                Assert.That(Create(s, a, b, rows, new[] { 1, 1 }), Is.EqualTo(DomainError.INVALID_DEFINITION));
            foreach (var cols in new int[]?[] { null, new[] { 1 }, new[] { -1, 3 }, new[] { 3, -1 }, new[] { 1, 0 }, new[] { 0, 2 } })
                Assert.That(Create(s, a, b, new[] { 2, 0 }, cols), Is.EqualTo(DomainError.INVALID_DEFINITION));
            Assert.That(Create(s, a, b, new[] { 0, 0 }, new[] { 0, 0 }), Is.EqualTo(DomainError.INVALID_DEFINITION));
            Assert.That(Create(s, a, b, new[] { 2, 0 }, new[] { 1, 1 }, "unknown"), Is.EqualTo(DomainError.UNSUPPORTED_RULESET));
            Assert.That(Create(s, a, b, new[] { 2, 0 }, new[] { 1, 1, 0 }), Is.EqualTo(DomainError.INVALID_DEFINITION));
            Assert.That(Create(s, a, b, new[] { 3, 0 }, new[] { 2, 1 }), Is.EqualTo(DomainError.INVALID_DEFINITION));
            Assert.That(Create(s, Fixtures.Edge(Direction.N, 0), Fixtures.Edge(Direction.W, 0), new[] { 2, 1 }, new[] { 3, 0 }), Is.EqualTo(DomainError.INVALID_DEFINITION));
            var three = Fixtures.Size(3, 3); var na = Fixtures.Edge(Direction.N, 0, three); var wa = Fixtures.Edge(Direction.W, 0, three);
            Assert.That(Create(three, na, wa, new[] { 1, -1, 1 }, new[] { 1, 0, 0 }), Is.EqualTo(DomainError.INVALID_DEFINITION));
            Assert.That(Create(three, na, wa, new[] { 1, 0, 0 }, new[] { 1, -1, 1 }), Is.EqualTo(DomainError.INVALID_DEFINITION));
            var bottom = Fixtures.Edge(Direction.E, 1);
            Assert.That(Create(s, a, bottom, new[] { 0, 2 }, new[] { 1, 1 }), Is.EqualTo(DomainError.INVALID_DEFINITION), "A row independently unoccupiable");
            Assert.That(Create(s, a, bottom, new[] { 2, 0 }, new[] { 1, 1 }), Is.EqualTo(DomainError.INVALID_DEFINITION), "B row independently unoccupiable");
            Assert.That(Create(s, Fixtures.Edge(Direction.N, 0), Fixtures.Edge(Direction.S, 1), new[] { 1, 1 }, new[] { 2, 0 }), Is.EqualTo(DomainError.INVALID_DEFINITION), "B column independently unoccupiable");
            var r = new[] { 2, 0 }; var c = new[] { 1, 1 };
            var def = PuzzleDefinition.Create(s, a, b, r, c, "train-track-v1").Value!;
            r[0] = 0; c[0] = 0;
            Assert.That(def.RowCounts[0], Is.EqualTo(2)); Assert.That(def.ColumnCounts[0], Is.EqualTo(1));
            Assert.Throws<NotSupportedException>(() => ((IList<int>)def.RowCounts)[0] = 0);
        }

        [Test]
        public void Completion_UsesOnlyTracksAndTraversesAllOfThem()
        {
            var cells = new[] { CellContent.TRACK_EW, CellContent.TRACK_EW, CellContent.MARK_OCCUPIED, CellContent.MARK_EMPTY };
            var eval = PuzzleEvaluator.Evaluate(Fixtures.Horizontal(), cells);
            Assert.That(eval.Completed, Is.True); Assert.That(eval.Path.Select(p => p.X), Is.EqualTo(new[] { 0, 1 }));
            Assert.That(eval.VisibleRows, Is.EqualTo(new[] { 2, 1 }));
            Assert.That(eval.Diagnostics.Any(d => d.Code == DiagnosticCode.ROW_COUNT_EXCEEDED), Is.True);
            cells[0] = CellContent.MARK_OCCUPIED;
            Assert.That(PuzzleEvaluator.Evaluate(Fixtures.Horizontal(), cells).Completed, Is.False);
            Assert.That(PuzzleEvaluator.Evaluate(Fixtures.Horizontal(), new[] { CellContent.TRACK_EW, CellContent.UNSET, CellContent.UNSET, CellContent.UNSET }).Completed, Is.False);
            Assert.That(PuzzleEvaluator.Evaluate(Fixtures.Horizontal(), new[] { CellContent.TRACK_NE, CellContent.TRACK_EW, CellContent.UNSET, CellContent.UNSET }).Completed, Is.False);
            Assert.That(PuzzleEvaluator.Evaluate(Fixtures.Single(), new[] { CellContent.TRACK_WN, CellContent.UNSET, CellContent.UNSET, CellContent.UNSET }).Completed, Is.True);
            Assert.That(PuzzleEvaluator.Evaluate(Fixtures.Single(), new[] { CellContent.TRACK_NS, CellContent.UNSET, CellContent.UNSET, CellContent.UNSET }).Completed, Is.False);
            Assert.That(PuzzleEvaluator.Evaluate(null, cells).Error, Is.EqualTo(DomainError.INVALID_DEFINITION));
            Assert.That(PuzzleEvaluator.Evaluate(Fixtures.Horizontal(), null).Error, Is.EqualTo(DomainError.INVALID_DEFINITION));
            Assert.That(PuzzleEvaluator.Evaluate(Fixtures.Horizontal(), new CellContent[1]).Error, Is.EqualTo(DomainError.INVALID_DEFINITION));
            Assert.That(PuzzleEvaluator.Evaluate(Fixtures.Horizontal(), new[] { (CellContent)99, CellContent.UNSET, CellContent.UNSET, CellContent.UNSET }).Error, Is.EqualTo(DomainError.INVALID_DEFINITION));
        }

        [Test]
        public void Diagnostics_ReportSixVisibleFactsWithoutJudgingOpenNeighbors()
        {
            var open = PuzzleEvaluator.Evaluate(Fixtures.Horizontal(), new[] { CellContent.TRACK_EW, CellContent.MARK_EMPTY, CellContent.UNSET, CellContent.UNSET });
            Assert.That(open.Diagnostics, Is.Empty);
            var wrong = PuzzleEvaluator.Evaluate(Fixtures.Horizontal(), new[] { CellContent.TRACK_NS, CellContent.TRACK_EW, CellContent.MARK_OCCUPIED, CellContent.MARK_OCCUPIED });
            foreach (var code in new[] { DiagnosticCode.ROW_COUNT_EXCEEDED, DiagnosticCode.COLUMN_COUNT_EXCEEDED, DiagnosticCode.TRACK_EXITS_GRID, DiagnosticCode.TRACK_CONNECTION_MISMATCH, DiagnosticCode.ENDPOINT_MISMATCH })
                Assert.That(wrong.Diagnostics.Any(d => d.Code == code), Is.True, code.ToString());
            Assert.That(wrong.Diagnostics.Select(d => (d.Coordinate.Y, d.Coordinate.X, d.Code)), Is.Ordered);
            var s = Fixtures.Size(3, 3);
            var def = PuzzleDefinition.Create(s, Fixtures.Edge(Direction.N, 0, s), Fixtures.Edge(Direction.W, 0, s), new[] { 1, 2, 2 }, new[] { 1, 2, 2 }, "train-track-v1").Value!;
            var loop = new[] { CellContent.TRACK_WN, CellContent.UNSET, CellContent.UNSET, CellContent.UNSET, CellContent.TRACK_ES, CellContent.TRACK_SW, CellContent.UNSET, CellContent.TRACK_NE, CellContent.TRACK_WN };
            var result = PuzzleEvaluator.Evaluate(def, loop);
            Assert.That(result.Completed, Is.False);
            Assert.That(result.Diagnostics.Count(d => d.Code == DiagnosticCode.PREMATURE_CONCRETE_LOOP), Is.EqualTo(4));
            Assert.Throws<NotSupportedException>(() => ((IList<int>)result.VisibleRows)[0] = 7);
        }

        [Test]
        public void Completion_RequiresRowsAndColumnsSeparatelyEvenForAValidSimplePath()
        {
            var s = Fixtures.Size(3, 3);
            var rows = PuzzleDefinition.Create(s, Fixtures.Edge(Direction.W, 1, s), Fixtures.Edge(Direction.E, 1, s), new[] { 3, 2, 0 }, new[] { 2, 1, 2 }, "train-track-v1").Value!;
            var bottom = new[] { CellContent.UNSET, CellContent.UNSET, CellContent.UNSET, CellContent.TRACK_SW, CellContent.UNSET, CellContent.TRACK_ES, CellContent.TRACK_NE, CellContent.TRACK_EW, CellContent.TRACK_WN };
            Assert.That(PuzzleEvaluator.Evaluate(rows, bottom).Completed, Is.False, "same column counts, wrong row counts");
            var cols = PuzzleDefinition.Create(s, Fixtures.Edge(Direction.N, 1, s), Fixtures.Edge(Direction.S, 1, s), new[] { 2, 1, 2 }, new[] { 3, 2, 0 }, "train-track-v1").Value!;
            var right = new[] { CellContent.UNSET, CellContent.TRACK_NE, CellContent.TRACK_SW, CellContent.UNSET, CellContent.UNSET, CellContent.TRACK_NS, CellContent.UNSET, CellContent.TRACK_ES, CellContent.TRACK_WN };
            Assert.That(PuzzleEvaluator.Evaluate(cols, right).Completed, Is.False, "same row counts, wrong column counts");
        }
    }
}
