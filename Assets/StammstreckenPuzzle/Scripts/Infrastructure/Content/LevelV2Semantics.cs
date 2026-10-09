using System;
using System.Collections.Generic;
using System.Globalization;
using STP.Puzzle.Domain;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Semantic stage (LEVEL_DATA_FORMAT.md Abschnitt 8): cross-field rules of a schema-valid document whose public part
    /// was already mapped to the Domain. It checks the ordered authoring path (document semantics the Domain does not
    /// know) and finally requires the Domain's own <see cref="PuzzleEvaluator"/> to accept the solution as completed,
    /// so the Domain stays the authority for the puzzle rules. Diagnostics are deterministic and ordered by rule family.
    /// </summary>
    public static class LevelV2Semantics
    {
        public static IReadOnlyList<LevelDiagnostic> Validate(LevelV2Document? document, PuzzleDefinition? definition)
        {
            var diagnostics = new List<LevelDiagnostic>();
            if (document == null || definition == null)
            {
                diagnostics.Add(Diagnostic(LevelDiagnosticCodes.DomainInvalidDefinition, string.Empty, "Semantic validation requires a document and its mapped definition."));
                return diagnostics;
            }
            CheckIdentity(document, diagnostics);
            CheckTimeOrder(document, diagnostics);
            CheckProofReference(document, diagnostics);
            CheckSolution(document, definition, diagnostics);
            return diagnostics;
        }

        private static void CheckIdentity(LevelV2Document document, List<LevelDiagnostic> diagnostics)
        {
            var content = document.Content;
            if (!LevelPatterns.TryParsePuzzleId(document.PuzzleId, out long season, out long section, out long route, out long position)
                || season != content.Season || section != content.NetworkSection || route != content.Route || position != content.Position)
                diagnostics.Add(Diagnostic(LevelDiagnosticCodes.IdContentMismatch, "/puzzleId", "The four puzzleId segments must equal content.season, networkSection, route and position."));
            if (content.Season == 1)
            {
                if (content.NetworkSection < 1 || content.NetworkSection > 5) diagnostics.Add(Diagnostic(LevelDiagnosticCodes.IdSeasonOneSection, "/content/networkSection", "Season 1 has network sections 1..5."));
                if (content.Route < 1 || content.Route > 4) diagnostics.Add(Diagnostic(LevelDiagnosticCodes.IdSeasonOneRoute, "/content/route", "Season 1 has routes 1..4."));
                if (content.Position < 1 || content.Position > 12) diagnostics.Add(Diagnostic(LevelDiagnosticCodes.IdSeasonOnePosition, "/content/position", "Season 1 has positions 1..12."));
            }
        }

        private static void CheckTimeOrder(LevelV2Document document, List<LevelDiagnostic> diagnostics)
        {
            var thresholds = document.Production.StarThresholdsSeconds;
            if (thresholds != null && thresholds.ThreeStars >= thresholds.TwoStars)
                diagnostics.Add(Diagnostic(LevelDiagnosticCodes.TimeOrder, "/production/starThresholdsSeconds", "threeStars must be strictly less than twoStars."));
        }

        private static void CheckProofReference(LevelV2Document document, List<LevelDiagnostic> diagnostics)
        {
            if (!string.Equals(document.ProofRef.ProofHash.Profile, LevelHashProfiles.Proof, StringComparison.Ordinal))
                diagnostics.Add(new LevelDiagnostic(LevelDiagnosticStage.Hash, LevelDiagnosticCodes.HashProfileUnknown, "/proofRef/proofHash/profile",
                    "proofRef.proofHash requires the hash profile " + LevelHashProfiles.Proof + "; unknown profiles are never interpreted heuristically."));
        }

        private static void CheckSolution(LevelV2Document document, PuzzleDefinition definition, List<LevelDiagnostic> diagnostics)
        {
            var path = document.SolutionPath;
            int width = definition.Grid.Width, height = definition.Grid.Height, count = path.Count;
            int before = diagnostics.Count;
            var firstIndex = new Dictionary<int, int>();
            bool buildable = true;
            var rows = new int[height];
            var columns = new int[width];

            for (int i = 0; i < count; i++)
            {
                var cell = path[i];
                string cellPath = CellPath(i);
                if (cell.X >= width || cell.Y >= height)
                {
                    diagnostics.Add(Diagnostic(LevelDiagnosticCodes.PathOutOfBounds, cellPath, "The path cell lies outside the grid."));
                    buildable = false;
                    continue;
                }
                rows[cell.Y]++;
                columns[cell.X]++;
                int key = cell.Y * 16 + cell.X;
                if (firstIndex.ContainsKey(key))
                {
                    diagnostics.Add(Diagnostic(LevelDiagnosticCodes.PathDuplicate, cellPath, "The path visits a coordinate more than once."));
                    buildable = false;
                }
                else firstIndex[key] = i;
            }

            for (int y = 0; y < height; y++)
                if (rows[y] != definition.RowCounts[y]) diagnostics.Add(Diagnostic(LevelDiagnosticCodes.CountRowMismatch, "/rowCounts/" + y.ToString(CultureInfo.InvariantCulture), "rowCounts must equal the number of solution cells in the row."));
            for (int x = 0; x < width; x++)
                if (columns[x] != definition.ColumnCounts[x]) diagnostics.Add(Diagnostic(LevelDiagnosticCodes.CountColumnMismatch, "/columnCounts/" + x.ToString(CultureInfo.InvariantCulture), "columnCounts must equal the number of solution cells in the column."));

            for (int i = 0; i + 1 < count; i++)
            {
                var left = path[i];
                var right = path[i + 1];
                int dx = right.X - left.X, dy = right.Y - left.Y;
                if (Math.Abs(dx) + Math.Abs(dy) != 1)
                {
                    diagnostics.Add(Diagnostic(LevelDiagnosticCodes.PathNotAdjacent, CellPath(i + 1), "Consecutive path cells must be orthogonally adjacent."));
                    continue;
                }
                Direction toRight = dx == 1 ? Direction.E : dx == -1 ? Direction.W : dy == 1 ? Direction.S : Direction.N;
                if (!HasPort(left, toRight) || !HasPort(right, TrackGeometry.Opposite(toRight)))
                    diagnostics.Add(Diagnostic(LevelDiagnosticCodes.PathPortMismatch, CellPath(i + 1), "The track shapes of consecutive path cells must connect to each other."));
            }

            if (count > 0)
            {
                var first = path[0];
                var last = path[count - 1];
                if (first.X != definition.A.Cell.X || first.Y != definition.A.Cell.Y) diagnostics.Add(Diagnostic(LevelDiagnosticCodes.EndpointACell, CellPath(0), "The first path cell must be the cell adjacent to endpoint A."));
                else if (!HasPort(first, definition.A.Side)) diagnostics.Add(Diagnostic(LevelDiagnosticCodes.EndpointAShape, CellPath(0), "The first track shape must open towards endpoint A."));
                if (last.X != definition.B.Cell.X || last.Y != definition.B.Cell.Y) diagnostics.Add(Diagnostic(LevelDiagnosticCodes.EndpointBCell, CellPath(count - 1), "The last path cell must be the cell adjacent to endpoint B."));
                else if (!HasPort(last, definition.B.Side)) diagnostics.Add(Diagnostic(LevelDiagnosticCodes.EndpointBShape, CellPath(count - 1), "The last track shape must open towards endpoint B."));
            }

            for (int i = 0; i < count; i++)
            {
                var cell = path[i];
                int ports = TrackGeometry.Ports(LevelV2DomainMapper.ToCellContent(cell.Track));
                for (int d = 0; d < 4; d++)
                {
                    if ((ports & (1 << d)) == 0) continue;
                    var direction = (Direction)d;
                    if ((i == 0 && direction == definition.A.Side) || (i == count - 1 && direction == definition.B.Side)) continue;
                    int nx = cell.X + TrackGeometry.DeltaX(direction), ny = cell.Y + TrackGeometry.DeltaY(direction);
                    if (nx >= 0 && ny >= 0 && firstIndex.TryGetValue(ny * 16 + nx, out int neighbor))
                    {
                        if (neighbor != i - 1 && neighbor != i + 1)
                            diagnostics.Add(Diagnostic(LevelDiagnosticCodes.RuleLoop, CellPath(i), "A track port leads into a non-consecutive path cell (loop)."));
                    }
                    else diagnostics.Add(Diagnostic(LevelDiagnosticCodes.RuleOpenConnection, CellPath(i), "A track port leads into an empty cell or out of the grid without being endpoint A or B."));
                }
            }

            if (buildable && diagnostics.Count == before)
            {
                var cells = new CellContent[definition.Grid.CellCount];
                foreach (var cell in path) cells[cell.Y * width + cell.X] = LevelV2DomainMapper.ToCellContent(cell.Track);
                if (!PuzzleEvaluator.Evaluate(definition, cells).Completed)
                    diagnostics.Add(Diagnostic(LevelDiagnosticCodes.DomainSolutionNotCompleted, "/solution/path", "The Domain evaluator does not accept the authoring solution as a completed puzzle."));
            }
        }

        private static bool HasPort(LevelPathCellDto cell, Direction direction) =>
            (TrackGeometry.Ports(LevelV2DomainMapper.ToCellContent(cell.Track)) & (1 << (int)direction)) != 0;

        private static string CellPath(int index) => "/solution/path/" + index.ToString(CultureInfo.InvariantCulture);

        private static LevelDiagnostic Diagnostic(string code, string path, string message) => new LevelDiagnostic(LevelDiagnosticStage.Semantic, code, path, message);
    }
}
