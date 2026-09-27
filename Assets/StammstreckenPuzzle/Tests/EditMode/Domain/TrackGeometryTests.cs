using NUnit.Framework;
using STP.Puzzle.Domain;

namespace STP.Tests.Domain.EditMode
{
    /// <summary>
    /// Geometrie- und Semantikverträge der sechs Gleisformen und Zellinhalte
    /// (Konzeptbaustein 03 Abschnitt 3, GAME_STATE_MODEL.md Abschnitt 3).
    /// </summary>
    public sealed class TrackGeometryTests
    {
        /// <summary>Jede der sechs Formen besitzt exakt die dokumentierten zwei Anschlüsse.</summary>
        [Test]
        public void AllShapes_HaveExactlyTheirTwoDocumentedPorts()
        {
            Assert.AreEqual((Direction.N, Direction.S), TrackShapeGeometry.Ports(TrackShape.NS));
            Assert.AreEqual((Direction.E, Direction.W), TrackShapeGeometry.Ports(TrackShape.EW));
            Assert.AreEqual((Direction.N, Direction.E), TrackShapeGeometry.Ports(TrackShape.NE));
            Assert.AreEqual((Direction.E, Direction.S), TrackShapeGeometry.Ports(TrackShape.ES));
            Assert.AreEqual((Direction.S, Direction.W), TrackShapeGeometry.Ports(TrackShape.SW));
            Assert.AreEqual((Direction.W, Direction.N), TrackShapeGeometry.Ports(TrackShape.WN));
            Assert.AreEqual(6, TrackShapeGeometry.All.Length);
        }

        /// <summary>Jedes ungeordnete Richtungspaar wird von genau einer Form abgedeckt.</summary>
        [Test]
        public void FromPorts_CoversEveryDistinctDirectionPairExactlyOnce()
        {
            var directions = new[] { Direction.N, Direction.E, Direction.S, Direction.W };
            var covered = 0;
            foreach (var first in directions)
            {
                foreach (var second in directions)
                {
                    if (first == second)
                    {
                        continue;
                    }
                    var shape = TrackShapeGeometry.FromPorts(first, second);
                    Assert.IsTrue(TrackShapeGeometry.HasPort(shape, first));
                    Assert.IsTrue(TrackShapeGeometry.HasPort(shape, second));
                    covered++;
                }
            }
            Assert.AreEqual(12, covered);
        }

        /// <summary>Die Gegenrichtung ist symmetrisch und vertauscht die Achsen korrekt.</summary>
        [Test]
        public void Opposite_IsSymmetricAndDeltasMatchConcept()
        {
            Assert.AreEqual(Direction.S, DirectionGeometry.Opposite(Direction.N));
            Assert.AreEqual(Direction.N, DirectionGeometry.Opposite(Direction.S));
            Assert.AreEqual(Direction.W, DirectionGeometry.Opposite(Direction.E));
            Assert.AreEqual(Direction.E, DirectionGeometry.Opposite(Direction.W));
            Assert.AreEqual(new CellCoordinate(3, 1), new CellCoordinate(3, 2).Neighbor(Direction.N));
            Assert.AreEqual(new CellCoordinate(3, 3), new CellCoordinate(3, 2).Neighbor(Direction.S));
            Assert.AreEqual(new CellCoordinate(4, 2), new CellCoordinate(3, 2).Neighbor(Direction.E));
            Assert.AreEqual(new CellCoordinate(2, 2), new CellCoordinate(3, 2).Neighbor(Direction.W));
        }

        /// <summary>Zellinhalte und Gleisformen bilden bijektiv aufeinander ab.</summary>
        [Test]
        public void CellContent_TrackMapping_IsBijective()
        {
            foreach (var shape in TrackShapeGeometry.All)
            {
                var content = CellContentSemantics.FromTrackShape(shape);
                Assert.IsTrue(CellContentSemantics.IsConcreteTrack(content));
                Assert.AreEqual(shape, CellContentSemantics.ToTrackShape(content));
            }
            Assert.IsFalse(CellContentSemantics.IsConcreteTrack(CellContent.Unset));
            Assert.IsFalse(CellContentSemantics.IsConcreteTrack(CellContent.MarkEmpty));
            Assert.IsFalse(CellContentSemantics.IsConcreteTrack(CellContent.MarkOccupied));
        }

        /// <summary>Nur graue Belegungsannahme und konkrete Gleise zählen für die sichtbare Mengenrückmeldung.</summary>
        [Test]
        public void VisibleOccupancy_CountsOnlyMarkersAndConcreteTracks()
        {
            Assert.IsFalse(CellContentSemantics.CountsAsVisibleOccupancy(CellContent.Unset));
            Assert.IsFalse(CellContentSemantics.CountsAsVisibleOccupancy(CellContent.MarkEmpty));
            Assert.IsTrue(CellContentSemantics.CountsAsVisibleOccupancy(CellContent.MarkOccupied));
            Assert.IsTrue(CellContentSemantics.CountsAsVisibleOccupancy(CellContent.TrackNS));
        }
    }
}
