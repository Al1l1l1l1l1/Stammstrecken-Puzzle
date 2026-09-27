using NUnit.Framework;
using STP.Puzzle.Domain;

namespace STP.Tests.Domain.EditMode
{
    /// <summary>
    /// Exakte Completion-Prüfung gemäß PUZZLE_ENGINE.md Abschnitt 6: positiv bei
    /// vollständigem einfachen A-B-Pfad, negativ bei jeder dokumentierten
    /// Verletzung; Hilfsmarkierungen außerhalb der Strecke entwerten den
    /// Abschluss nicht.
    /// </summary>
    public sealed class CompletionEvaluatorTests
    {
        private static PuzzleDefinition SingleCellDefinition()
        {
            return new PuzzleDefinition(
                new GridSize(2, 2),
                new Endpoint(Direction.N, 0),
                new Endpoint(Direction.W, 0),
                new[] { 1, 0 },
                new[] { 1, 0 },
                PuzzleDefinition.RulesetVersionV1);
        }

        /// <summary>Die verankerte 3×3-Schlange A=N0 nach B=S2 mit ihrer eindeutigen Lösung.</summary>
        private static PuzzleDefinition SnakeDefinition()
        {
            return new PuzzleDefinition(
                new GridSize(3, 3),
                new Endpoint(Direction.N, 0),
                new Endpoint(Direction.S, 2),
                new[] { 1, 1, 3 },
                new[] { 3, 1, 1 },
                PuzzleDefinition.RulesetVersionV1);
        }

        private static CellContent[] Cells(GridSize grid, params (int X, int Y, CellContent Content)[] entries)
        {
            var cells = new CellContent[grid.Width * grid.Height];
            foreach (var (x, y, content) in entries)
            {
                cells[grid.IndexOf(new CellCoordinate(x, y))] = content;
            }
            return cells;
        }

        private static CellContent[] SnakeSolution()
        {
            return Cells(
                new GridSize(3, 3),
                (0, 0, CellContent.TrackNS),
                (0, 1, CellContent.TrackNS),
                (0, 2, CellContent.TrackNE),
                (1, 2, CellContent.TrackEW),
                (2, 2, CellContent.TrackSW));
        }

        /// <summary>Der technisch gültige Ein-Zellen-A-B-Pfad (TRACK_WN) ist gelöst.</summary>
        [Test]
        public void SingleCellPath_IsSolved()
        {
            var definition = SingleCellDefinition();
            var cells = Cells(definition.Grid, (0, 0, CellContent.TrackWN));
            Assert.IsTrue(CompletionEvaluator.IsSolved(definition, cells));
        }

        /// <summary>Die vollständige 3×3-Schlange ist gelöst.</summary>
        [Test]
        public void SnakeSolution_IsSolved()
        {
            Assert.IsTrue(CompletionEvaluator.IsSolved(SnakeDefinition(), SnakeSolution()));
        }

        /// <summary>Hilfsmarkierungen außerhalb der Strecke entwerten den Abschluss nicht.</summary>
        [Test]
        public void MarkersOutsideTheTrack_DoNotInvalidate()
        {
            var definition = SingleCellDefinition();
            var cells = Cells(
                definition.Grid,
                (0, 0, CellContent.TrackWN),
                (1, 0, CellContent.MarkOccupied),
                (0, 1, CellContent.MarkEmpty),
                (1, 1, CellContent.MarkOccupied));
            Assert.IsTrue(CompletionEvaluator.IsSolved(definition, cells));
        }

        /// <summary>Falsche Form in der Ein-Zellen-Aufgabe (kein WN) ist nicht gelöst.</summary>
        [Test]
        public void WrongShapeOnSingleCell_IsNotSolved()
        {
            var definition = SingleCellDefinition();
            Assert.IsFalse(CompletionEvaluator.IsSolved(
                definition, Cells(definition.Grid, (0, 0, CellContent.TrackNS))));
        }

        /// <summary>Abweichende Zeilen- oder Spaltenzahlen sind nicht gelöst.</summary>
        [Test]
        public void CountMismatch_IsNotSolved()
        {
            var definition = SingleCellDefinition();
            var cells = Cells(
                definition.Grid,
                (0, 0, CellContent.TrackWN),
                (1, 1, CellContent.TrackES));
            Assert.IsFalse(CompletionEvaluator.IsSolved(definition, cells));
        }

        /// <summary>Ein offenes Anschlussende (fehlender Gegenport) ist nicht gelöst.</summary>
        [Test]
        public void DanglingConnection_IsNotSolved()
        {
            var definition = SnakeDefinition();
            var cells = SnakeSolution();
            cells[definition.Grid.IndexOf(new CellCoordinate(1, 2))] = CellContent.TrackNS;
            Assert.IsFalse(CompletionEvaluator.IsSolved(definition, cells));
        }

        /// <summary>Eine zweite getrennte Komponente ist nicht gelöst, selbst bei erfüllten Zahlen.</summary>
        [Test]
        public void SecondComponent_IsNotSolved()
        {
            var definition = new PuzzleDefinition(
                new GridSize(4, 4),
                new Endpoint(Direction.N, 0),
                new Endpoint(Direction.W, 0),
                new[] { 1, 0, 2, 2 },
                new[] { 1, 0, 2, 2 },
                PuzzleDefinition.RulesetVersionV1);
            // Lösende Einzelzelle (0,0)WN plus ein getrennter 2×2-Ring unten rechts:
            // alle Randzahlen erfüllt, aber zwei Komponenten.
            var cells = Cells(
                definition.Grid,
                (0, 0, CellContent.TrackWN),
                (2, 2, CellContent.TrackES),
                (3, 2, CellContent.TrackSW),
                (2, 3, CellContent.TrackNE),
                (3, 3, CellContent.TrackWN));
            Assert.IsFalse(CompletionEvaluator.IsSolved(definition, cells));
        }

        /// <summary>Eine geschlossene Schleife ohne A/B-Bedeckung ist nicht gelöst.</summary>
        [Test]
        public void ClosedLoop_IsNotSolved()
        {
            var definition = new PuzzleDefinition(
                new GridSize(3, 3),
                new Endpoint(Direction.N, 0),
                new Endpoint(Direction.N, 1),
                new[] { 2, 2, 0 },
                new[] { 2, 2, 0 },
                PuzzleDefinition.RulesetVersionV1);
            var cells = Cells(
                definition.Grid,
                (0, 0, CellContent.TrackES),
                (1, 0, CellContent.TrackSW),
                (0, 1, CellContent.TrackNE),
                (1, 1, CellContent.TrackWN));
            Assert.IsFalse(CompletionEvaluator.IsSolved(definition, cells));
        }

        /// <summary>Ein Gleis zu einer falschen Außenkante ist nicht gelöst.</summary>
        [Test]
        public void ExitToWrongEdge_IsNotSolved()
        {
            var definition = new PuzzleDefinition(
                new GridSize(2, 2),
                new Endpoint(Direction.N, 0),
                new Endpoint(Direction.N, 1),
                new[] { 2, 0 },
                new[] { 1, 1 },
                PuzzleDefinition.RulesetVersionV1);
            // Beide Gleise führen nach Norden zu A und B, bleiben aber untereinander offen;
            // (0,0) zusätzlich mit unzulässigem Westport aus dem Raster.
            var cells = Cells(
                definition.Grid,
                (0, 0, CellContent.TrackWN),
                (1, 0, CellContent.TrackNE));
            Assert.IsFalse(CompletionEvaluator.IsSolved(definition, cells));
        }

        /// <summary>Ein leerer Stand ist nicht gelöst.</summary>
        [Test]
        public void EmptyBoard_IsNotSolved()
        {
            Assert.IsFalse(CompletionEvaluator.IsSolved(
                SingleCellDefinition(), Cells(new GridSize(2, 2))));
        }
    }
}
