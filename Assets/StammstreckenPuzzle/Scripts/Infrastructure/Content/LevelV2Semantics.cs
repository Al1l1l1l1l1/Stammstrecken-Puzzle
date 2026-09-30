using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using STP.Puzzle.Domain;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Semantische Leveldiagnosen des einzelnen level-v2-Datensatzes
    /// einschließlich seiner Authoringlösung gemäß LEVEL_DATA_FORMAT.md
    /// Abschnitt 8: die dokumentierten Codefamilien <c>LVL-ID-*</c> (Segmente
    /// exakt gegen <c>content.season</c>, <c>networkSection</c>, <c>route</c>,
    /// <c>position</c>; Season-1-Bereiche 5×4×12), <c>LVL-PATH-*</c>,
    /// <c>LVL-RULE-*</c>, <c>LVL-COUNT-*</c> (aus der Lösung abgeleitete
    /// Randzahlen), <c>LVL-TIME-ORDER</c> und <c>LVL-HASH-*</c> (bekannte
    /// Profile). Die Familien <c>LVL-GRID-*</c> und <c>LVL-ENDPOINT-*</c>
    /// werden bereits durch die Definitionsinvarianten der Domain-Mapping-Stufe
    /// (<see cref="LevelV2DomainMapper"/>) abgedeckt. Katalogübergreifende
    /// ID-/Hierarchieprüfungen bleiben ausdrücklich der späteren
    /// Cross-reference-Stufe vorbehalten.
    /// </summary>
    public static class LevelV2Semantics
    {
        private static readonly Regex PuzzleIdSegments = new Regex(
            @"^S([1-9][0-9]*)-([0-9]{2})-([0-9]{2})-([0-9]{2})$",
            RegexOptions.CultureInvariant);

        /// <summary>
        /// Prüft den Datensatz semantisch gegen die dokumentierten Familien
        /// und sammelt alle Befunde als Diagnosen.
        /// </summary>
        public static void Validate(LevelV2Document document, PuzzleDefinition definition, List<LevelDiagnostic> diagnostics)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }
            if (definition is null)
            {
                throw new ArgumentNullException(nameof(definition));
            }
            if (diagnostics is null)
            {
                throw new ArgumentNullException(nameof(diagnostics));
            }

            ValidateIdSegments(document, diagnostics);
            ValidatePath(document, definition, diagnostics);
            ValidateCounts(document, diagnostics);
            ValidateTimeOrder(document, diagnostics);
            ValidateHashProfiles(document, diagnostics);
        }

        private static void ValidateIdSegments(LevelV2Document document, List<LevelDiagnostic> diagnostics)
        {
            var match = PuzzleIdSegments.Match(document.PuzzleId);
            if (!match.Success)
            {
                diagnostics.Add(LevelDiagnostic.Error("LVL-ID-FORMAT", "$.puzzleId", $"puzzleId '{document.PuzzleId}' hat nicht vier auswertbare Segmente."));
                return;
            }
            var season = long.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
            var section = long.Parse(match.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);
            var route = long.Parse(match.Groups[3].Value, System.Globalization.CultureInfo.InvariantCulture);
            var position = long.Parse(match.Groups[4].Value, System.Globalization.CultureInfo.InvariantCulture);

            if (season != document.Content.Season
                || section != document.Content.NetworkSection
                || route != document.Content.Route
                || position != document.Content.Position)
            {
                diagnostics.Add(LevelDiagnostic.Error(
                    "LVL-ID-SEGMENT-MISMATCH",
                    "$.puzzleId",
                    $"Die ID-Segmente S{season}-{section:00}-{route:00}-{position:00} weichen von content "
                    + $"(season {document.Content.Season}, networkSection {document.Content.NetworkSection}, "
                    + $"route {document.Content.Route}, position {document.Content.Position}) ab."));
            }

            if (document.Content.Season == 1)
            {
                if (document.Content.NetworkSection < 1 || document.Content.NetworkSection > 5)
                {
                    diagnostics.Add(LevelDiagnostic.Error("LVL-ID-SEASON1-SECTION-RANGE", "$.content.networkSection", $"Season 1 verlangt networkSection 1..5, nicht {document.Content.NetworkSection}."));
                }
                if (document.Content.Route < 1 || document.Content.Route > 4)
                {
                    diagnostics.Add(LevelDiagnostic.Error("LVL-ID-SEASON1-ROUTE-RANGE", "$.content.route", $"Season 1 verlangt route 1..4, nicht {document.Content.Route}."));
                }
                if (document.Content.Position < 1 || document.Content.Position > 12)
                {
                    diagnostics.Add(LevelDiagnostic.Error("LVL-ID-SEASON1-POSITION-RANGE", "$.content.position", $"Season 1 verlangt position 1..12, nicht {document.Content.Position}."));
                }
            }
        }

        private static void ValidatePath(LevelV2Document document, PuzzleDefinition definition, List<LevelDiagnostic> diagnostics)
        {
            var grid = definition.Grid;
            var path = document.Solution.Path;
            var coordinates = new HashSet<CellCoordinate>();

            for (var i = 0; i < path.Count; i++)
            {
                var cell = path[i];
                var coordinate = new CellCoordinate(cell.X, cell.Y);
                if (!grid.Contains(coordinate))
                {
                    diagnostics.Add(LevelDiagnostic.Error("LVL-PATH-CELL-OUT-OF-BOUNDS", $"$.solution.path[{i}]", $"Pfadzelle {coordinate} liegt außerhalb des Rasters {grid}."));
                    continue;
                }
                if (!coordinates.Add(coordinate))
                {
                    diagnostics.Add(LevelDiagnostic.Error("LVL-PATH-DUPLICATE-CELL", $"$.solution.path[{i}]", $"Pfadzelle {coordinate} kommt mehrfach vor."));
                }
            }

            for (var i = 0; i + 1 < path.Count; i++)
            {
                var current = new CellCoordinate(path[i].X, path[i].Y);
                var next = new CellCoordinate(path[i + 1].X, path[i + 1].Y);
                var deltaX = next.X - current.X;
                var deltaY = next.Y - current.Y;
                var orthogonal = (deltaX == 0 && (deltaY == 1 || deltaY == -1)) || (deltaY == 0 && (deltaX == 1 || deltaX == -1));
                if (!orthogonal)
                {
                    diagnostics.Add(LevelDiagnostic.Error("LVL-PATH-NOT-ADJACENT", $"$.solution.path[{i + 1}]", $"Pfadzellen {current} und {next} sind nicht orthogonal benachbart."));
                    continue;
                }
                var direction = DirectionOf(deltaX, deltaY);
                if (!TrackShapeGeometry.HasPort(path[i].Track, direction) || !TrackShapeGeometry.HasPort(path[i + 1].Track, DirectionGeometry.Opposite(direction)))
                {
                    diagnostics.Add(LevelDiagnostic.Error("LVL-PATH-CONNECTION-MISMATCH", $"$.solution.path[{i}]", $"Die Anschlüsse von {current} ({path[i].Track}) und {next} ({path[i + 1].Track}) passen an ihrer gemeinsamen Kante nicht zusammen."));
                }
            }

            var first = path[0];
            var firstCoordinate = new CellCoordinate(first.X, first.Y);
            if (firstCoordinate != definition.A.AdjacentCell(grid) || !TrackShapeGeometry.HasPort(first.Track, definition.A.Side))
            {
                diagnostics.Add(LevelDiagnostic.Error("LVL-PATH-ENDPOINT-A-MISMATCH", "$.solution.path[0]", $"Die erste Pfadzelle {firstCoordinate} ({first.Track}) bedient den Außenanschluss A ({definition.A}) nicht."));
            }
            var last = path[path.Count - 1];
            var lastCoordinate = new CellCoordinate(last.X, last.Y);
            if (lastCoordinate != definition.B.AdjacentCell(grid) || !TrackShapeGeometry.HasPort(last.Track, definition.B.Side))
            {
                diagnostics.Add(LevelDiagnostic.Error("LVL-PATH-ENDPOINT-B-MISMATCH", $"$.solution.path[{path.Count - 1}]", $"Die letzte Pfadzelle {lastCoordinate} ({last.Track}) bedient den Außenanschluss B ({definition.B}) nicht."));
            }

            ValidatePathRules(document, definition, coordinates, diagnostics);
        }

        private static void ValidatePathRules(LevelV2Document document, PuzzleDefinition definition, HashSet<CellCoordinate> coordinates, List<LevelDiagnostic> diagnostics)
        {
            var grid = definition.Grid;
            var path = document.Solution.Path;
            for (var i = 0; i < path.Count; i++)
            {
                var cell = path[i];
                var coordinate = new CellCoordinate(cell.X, cell.Y);
                var (first, second) = TrackShapeGeometry.Ports(cell.Track);
                foreach (var port in new[] { first, second })
                {
                    var neighbor = coordinate.Neighbor(port);
                    var isSequenceNeighbor = (i > 0 && neighbor == new CellCoordinate(path[i - 1].X, path[i - 1].Y))
                        || (i + 1 < path.Count && neighbor == new CellCoordinate(path[i + 1].X, path[i + 1].Y));
                    if (isSequenceNeighbor)
                    {
                        continue;
                    }
                    if (!grid.Contains(neighbor))
                    {
                        if (definition.TryGetEndpointAt(coordinate, port, out _))
                        {
                            continue;
                        }
                        diagnostics.Add(LevelDiagnostic.Error("LVL-RULE-OPEN-END", $"$.solution.path[{i}]", $"Die Gleisform {cell.Track} auf {coordinate} endet an einer Außenkante ohne passenden Anschluss ({port})."));
                        continue;
                    }
                    if (coordinates.Contains(neighbor))
                    {
                        diagnostics.Add(LevelDiagnostic.Error("LVL-RULE-LOOP", $"$.solution.path[{i}]", $"Die Gleisform {cell.Track} auf {coordinate} verbindet mit {neighbor} außerhalb der Pfadfolge (Schleife oder Abzweigung)."));
                        continue;
                    }
                    diagnostics.Add(LevelDiagnostic.Error("LVL-RULE-OPEN-END", $"$.solution.path[{i}]", $"Die Gleisform {cell.Track} auf {coordinate} endet an der nicht belegten Zelle {neighbor} ({port})."));
                }
            }
        }

        private static void ValidateCounts(LevelV2Document document, List<LevelDiagnostic> diagnostics)
        {
            var derivedRows = new int[document.Grid.Height];
            var derivedColumns = new int[document.Grid.Width];
            foreach (var cell in document.Solution.Path)
            {
                if (cell.Y >= 0 && cell.Y < document.Grid.Height)
                {
                    derivedRows[cell.Y]++;
                }
                if (cell.X >= 0 && cell.X < document.Grid.Width)
                {
                    derivedColumns[cell.X]++;
                }
            }
            for (var y = 0; y < document.Grid.Height && y < document.RowCounts.Count; y++)
            {
                if (derivedRows[y] != document.RowCounts[y])
                {
                    diagnostics.Add(LevelDiagnostic.Error("LVL-COUNT-ROW-MISMATCH", $"$.rowCounts[{y}]", $"Aus der Lösung abgeleitete Zeilenzahl {derivedRows[y]} weicht vom deklarierten Wert {document.RowCounts[y]} ab."));
                }
            }
            for (var x = 0; x < document.Grid.Width && x < document.ColumnCounts.Count; x++)
            {
                if (derivedColumns[x] != document.ColumnCounts[x])
                {
                    diagnostics.Add(LevelDiagnostic.Error("LVL-COUNT-COLUMN-MISMATCH", $"$.columnCounts[{x}]", $"Aus der Lösung abgeleitete Spaltenzahl {derivedColumns[x]} weicht vom deklarierten Wert {document.ColumnCounts[x]} ab."));
                }
            }
        }

        private static void ValidateTimeOrder(LevelV2Document document, List<LevelDiagnostic> diagnostics)
        {
            var thresholds = document.Production.StarThresholdsSeconds;
            if (thresholds is not null && thresholds.ThreeStars >= thresholds.TwoStars)
            {
                diagnostics.Add(LevelDiagnostic.Error("LVL-TIME-ORDER", "$.production.starThresholdsSeconds", $"Erforderlich ist threeStars ({thresholds.ThreeStars}) strikt kleiner als twoStars ({thresholds.TwoStars}) oder vollständig null."));
            }
        }

        private static void ValidateHashProfiles(LevelV2Document document, List<LevelDiagnostic> diagnostics)
        {
            if (!HashProfiles.IsKnown(document.ProofRef.ProofHash.Profile))
            {
                diagnostics.Add(LevelDiagnostic.Error("LVL-HASH-UNKNOWN-PROFILE", "$.proofRef.proofHash.profile", $"Unbekanntes Hashprofil '{document.ProofRef.ProofHash.Profile}'."));
            }
        }

        private static Direction DirectionOf(int deltaX, int deltaY)
        {
            if (deltaX == 1)
            {
                return Direction.E;
            }
            if (deltaX == -1)
            {
                return Direction.W;
            }
            if (deltaY == 1)
            {
                return Direction.S;
            }
            return Direction.N;
        }
    }
}
