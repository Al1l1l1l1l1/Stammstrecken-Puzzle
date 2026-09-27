using NUnit.Framework;
using STP.Puzzle.Domain;

namespace STP.Tests.Domain.EditMode
{
    /// <summary>
    /// Sichtbare objektive Diagnosen gemäß PUZZLE_ENGINE.md Abschnitt 5. Geprüft
    /// werden alle sechs dokumentierten Bedingungen sowie die Abwesenheit falscher
    /// Meldungen bei unbestimmten Nachbarn.
    /// </summary>
    public sealed class BoardEvaluatorTests
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

        private static CellContent[] Cells(GridSize grid, params (int X, int Y, CellContent Content)[] entries)
        {
            var cells = new CellContent[grid.Width * grid.Height];
            foreach (var (x, y, content) in entries)
            {
                cells[grid.IndexOf(new CellCoordinate(x, y))] = content;
            }
            return cells;
        }

        /// <summary>Zeilenüberschreitung zählt konkrete Gleise und graue Annahmen, nicht jedoch X-Markierungen; ein Gleis in einer Null-Spalte übersteigt die Spaltenzahl.</summary>
        [Test]
        public void RowExceeded_CountsTracksAndOccupiedMarkersOnly()
        {
            var definition = SingleCellDefinition();
            var cells = Cells(
                definition.Grid,
                (0, 0, CellContent.MarkOccupied),
                (1, 0, CellContent.TrackEW),
                (0, 1, CellContent.MarkEmpty));

            var diagnostics = BoardEvaluator.Evaluate(definition, cells);

            Assert.AreEqual(new[] { 0 }, diagnostics.ExceededRows);
            Assert.AreEqual(new[] { 1 }, diagnostics.ExceededColumns);
        }

        /// <summary>Spaltenüberschreitung wird analog gemeldet.</summary>
        [Test]
        public void ColumnExceeded_IsReportedPerColumn()
        {
            var definition = SingleCellDefinition();
            var cells = Cells(
                definition.Grid,
                (0, 0, CellContent.TrackNS),
                (0, 1, CellContent.MarkOccupied));

            var diagnostics = BoardEvaluator.Evaluate(definition, cells);

            Assert.AreEqual(new[] { 0 }, diagnostics.ExceededColumns);
        }

        /// <summary>Ein konkretes Gleis, das ohne Endpoint aus dem Raster führt, verletzt TRACK_EXITS_GRID.</summary>
        [Test]
        public void TrackExitsGrid_WithoutEndpoint_IsReported()
        {
            var definition = SingleCellDefinition();
            // (0,1) liegt am unteren Rand; eine NS-Form führt nach Süden ohne Endpoint aus dem Raster.
            var cells = Cells(definition.Grid, (0, 1, CellContent.TrackNS));

            var diagnostics = BoardEvaluator.Evaluate(definition, cells);

            Assert.Contains(
                new CellDiagnostic(ObjectiveDiagnosticCodes.TrackExitsGrid, new CellCoordinate(0, 1)),
                (System.Collections.Generic.List<CellDiagnostic>)System.Linq.Enumerable.ToList(diagnostics.CellDiagnostics));
        }

        /// <summary>Ein Port zum Endpoint an exakt dieser Kante ist kein Rasteraustritt.</summary>
        [Test]
        public void PortTowardsEndpoint_IsNoExitViolation()
        {
            var definition = SingleCellDefinition();
            var cells = Cells(definition.Grid, (0, 0, CellContent.TrackWN));

            var diagnostics = BoardEvaluator.Evaluate(definition, cells);

            Assert.IsFalse(diagnostics.HasViolations);
        }

        /// <summary>Widersprüchliche konkrete Nachbaranschlüsse werden gemeldet, unbestimmte Nachbarn nicht.</summary>
        [Test]
        public void ConnectionMismatch_OnlyAgainstConcreteNeighbor()
        {
            var definition = SingleCellDefinition();
            var mismatch = Cells(
                definition.Grid,
                (0, 0, CellContent.TrackEW),
                (1, 0, CellContent.TrackNS));
            Assert.Contains(
                new CellDiagnostic(ObjectiveDiagnosticCodes.TrackConnectionMismatch, new CellCoordinate(0, 0)),
                (System.Collections.Generic.List<CellDiagnostic>)System.Linq.Enumerable.ToList(
                    BoardEvaluator.Evaluate(definition, mismatch).CellDiagnostics));

            var unsetNeighbor = Cells(definition.Grid, (0, 0, CellContent.TrackEW));
            foreach (var diagnostic in BoardEvaluator.Evaluate(definition, unsetNeighbor).CellDiagnostics)
            {
                Assert.AreNotEqual(ObjectiveDiagnosticCodes.TrackConnectionMismatch, diagnostic.Code);
            }

            var markedNeighbor = Cells(
                definition.Grid,
                (0, 0, CellContent.TrackEW),
                (1, 0, CellContent.MarkEmpty));
            foreach (var diagnostic in BoardEvaluator.Evaluate(definition, markedNeighbor).CellDiagnostics)
            {
                Assert.AreNotEqual(ObjectiveDiagnosticCodes.TrackConnectionMismatch, diagnostic.Code);
            }
        }

        /// <summary>Eine konkrete Endpointzelle ohne passenden Außenport verletzt ENDPOINT_MISMATCH.</summary>
        [Test]
        public void EndpointMismatch_IsReportedForUnservingTrack()
        {
            var definition = SingleCellDefinition();
            // (0,0) mit ES kann weder A (N) noch B (W) bedienen.
            var cells = Cells(definition.Grid, (0, 0, CellContent.TrackES));

            var diagnostics = BoardEvaluator.Evaluate(definition, cells);

            Assert.Contains(
                new CellDiagnostic(ObjectiveDiagnosticCodes.EndpointMismatch, new CellCoordinate(0, 0)),
                (System.Collections.Generic.List<CellDiagnostic>)System.Linq.Enumerable.ToList(diagnostics.CellDiagnostics));
        }

        /// <summary>Vier konkrete Kurven im 2×2-Ring bilden eine vorzeitige Schleife.</summary>
        [Test]
        public void PrematureConcreteLoop_IsDetected()
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

            var diagnostics = BoardEvaluator.Evaluate(definition, cells);

            Assert.Contains(
                new CellDiagnostic(ObjectiveDiagnosticCodes.PrematureConcreteLoop, new CellCoordinate(0, 0)),
                (System.Collections.Generic.List<CellDiagnostic>)System.Linq.Enumerable.ToList(diagnostics.CellDiagnostics));
        }

        /// <summary>Ein leerer Stand erzeugt keine Verletzungen.</summary>
        [Test]
        public void EmptyBoard_HasNoViolations()
        {
            var diagnostics = BoardEvaluator.Evaluate(
                SingleCellDefinition(), Cells(new GridSize(2, 2)));
            Assert.IsFalse(diagnostics.HasViolations);
        }
    }
}
