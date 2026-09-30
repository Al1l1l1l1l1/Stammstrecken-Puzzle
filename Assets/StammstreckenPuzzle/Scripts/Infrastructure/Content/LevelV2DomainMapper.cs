using System;
using System.Collections.Generic;
using STP.Puzzle.Domain;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Domain-Mapping eines strukturell gültigen level-v2-DTOs auf die
    /// vollständig validierte <see cref="PuzzleDefinition"/> gemäß
    /// PUZZLE_ENGINE.md Abschnitt 3, einschließlich des Ein-Zellen-Sonderfalls
    /// (A und B mit derselben angrenzenden Zelle). Verletzungen der
    /// Definitionsinvarianten werden als Mappingcodes der Familien
    /// <c>LVL-GRID-*</c> und <c>LVL-ENDPOINT-*</c> gemeldet; die Konstruktion
    /// der Definition bleibt die abschließende fail-closed Instanz.
    /// </summary>
    public static class LevelV2DomainMapper
    {
        /// <summary>
        /// Bildet das DTO auf eine <see cref="PuzzleDefinition"/> ab. Liefert
        /// <c>null</c>, wenn eine Definitionsinvariante verletzt ist; jede
        /// Verletzung wird als Diagnose gemeldet.
        /// </summary>
        public static PuzzleDefinition? Map(LevelV2Document document, List<LevelDiagnostic> diagnostics)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }
            if (diagnostics is null)
            {
                throw new ArgumentNullException(nameof(diagnostics));
            }

            var valid = true;

            GridSize grid;
            try
            {
                grid = new GridSize(document.Grid.Width, document.Grid.Height);
            }
            catch (ArgumentOutOfRangeException)
            {
                diagnostics.Add(LevelDiagnostic.Error("LVL-GRID-SIZE", "$.grid", $"Rastergröße {document.Grid.Width}x{document.Grid.Height} außerhalb des Domainbereichs."));
                return null;
            }

            var a = new Endpoint(document.EndpointA.Side, document.EndpointA.Index);
            var b = new Endpoint(document.EndpointB.Side, document.EndpointB.Index);

            if (a == b)
            {
                diagnostics.Add(LevelDiagnostic.Error("LVL-ENDPOINT-IDENTICAL", "$.endpoints", "A und B müssen unterschiedliche Außenanschlüsse sein."));
                valid = false;
            }
            if (!a.IsValidFor(grid))
            {
                diagnostics.Add(LevelDiagnostic.Error("LVL-ENDPOINT-INDEX-OUT-OF-RANGE", "$.endpoints.a", $"Endpoint-A-Index {a.Index} passt nicht zur Achse {a.Side} des Rasters {grid}."));
                valid = false;
            }
            if (!b.IsValidFor(grid))
            {
                diagnostics.Add(LevelDiagnostic.Error("LVL-ENDPOINT-INDEX-OUT-OF-RANGE", "$.endpoints.b", $"Endpoint-B-Index {b.Index} passt nicht zur Achse {b.Side} des Rasters {grid}."));
                valid = false;
            }

            if (document.RowCounts.Count != grid.Height)
            {
                diagnostics.Add(LevelDiagnostic.Error("LVL-GRID-ROWCOUNT-LENGTH", "$.rowCounts", $"rowCounts benötigt Länge {grid.Height}, hat aber {document.RowCounts.Count}."));
                valid = false;
            }
            if (document.ColumnCounts.Count != grid.Width)
            {
                diagnostics.Add(LevelDiagnostic.Error("LVL-GRID-COLUMNCOUNT-LENGTH", "$.columnCounts", $"columnCounts benötigt Länge {grid.Width}, hat aber {document.ColumnCounts.Count}."));
                valid = false;
            }

            var rowSum = 0;
            var rowsWithinBounds = document.RowCounts.Count == grid.Height;
            for (var y = 0; y < document.RowCounts.Count; y++)
            {
                var count = document.RowCounts[y];
                if (count < 0 || count > grid.Width)
                {
                    diagnostics.Add(LevelDiagnostic.Error("LVL-GRID-ROWCOUNT-RANGE", $"$.rowCounts[{y}]", $"Zeilenzahl {count} außerhalb von 0..{grid.Width}."));
                    rowsWithinBounds = false;
                    valid = false;
                }
                rowSum += count;
            }

            var columnSum = 0;
            var columnsWithinBounds = document.ColumnCounts.Count == grid.Width;
            for (var x = 0; x < document.ColumnCounts.Count; x++)
            {
                var count = document.ColumnCounts[x];
                if (count < 0 || count > grid.Height)
                {
                    diagnostics.Add(LevelDiagnostic.Error("LVL-GRID-COLUMNCOUNT-RANGE", $"$.columnCounts[{x}]", $"Spaltenzahl {count} außerhalb von 0..{grid.Height}."));
                    columnsWithinBounds = false;
                    valid = false;
                }
                columnSum += count;
            }

            if (rowsWithinBounds && columnsWithinBounds)
            {
                if (rowSum != columnSum)
                {
                    diagnostics.Add(LevelDiagnostic.Error("LVL-GRID-SUM-MISMATCH", "$.rowCounts", $"Zeilensumme {rowSum} muss gleich der Spaltensumme {columnSum} sein."));
                    valid = false;
                }
                else if (rowSum < 1)
                {
                    diagnostics.Add(LevelDiagnostic.Error("LVL-GRID-SUM-EMPTY", "$.rowCounts", "Die Summe der Randzahlen muss mindestens 1 sein."));
                    valid = false;
                }

                if (a.IsValidFor(grid) && document.RowCounts.Count == grid.Height && document.ColumnCounts.Count == grid.Width)
                {
                    var cellA = a.AdjacentCell(grid);
                    if (document.RowCounts[cellA.Y] == 0 || document.ColumnCounts[cellA.X] == 0)
                    {
                        diagnostics.Add(LevelDiagnostic.Error("LVL-ENDPOINT-ADJACENT-CELL-EMPTY", "$.endpoints.a", $"Die angrenzende Zelle {cellA} des Endpoints {a} kann laut Randzahlen nicht belegt sein."));
                        valid = false;
                    }
                }
                if (b.IsValidFor(grid) && document.RowCounts.Count == grid.Height && document.ColumnCounts.Count == grid.Width)
                {
                    var cellB = b.AdjacentCell(grid);
                    if (document.RowCounts[cellB.Y] == 0 || document.ColumnCounts[cellB.X] == 0)
                    {
                        diagnostics.Add(LevelDiagnostic.Error("LVL-ENDPOINT-ADJACENT-CELL-EMPTY", "$.endpoints.b", $"Die angrenzende Zelle {cellB} des Endpoints {b} kann laut Randzahlen nicht belegt sein."));
                        valid = false;
                    }
                }
            }

            if (!valid)
            {
                return null;
            }

            try
            {
                return new PuzzleDefinition(grid, a, b, document.RowCounts, document.ColumnCounts, document.RulesetVersion);
            }
            catch (ArgumentException ex)
            {
                diagnostics.Add(LevelDiagnostic.Error("LVL-DOMAIN-INVALID-DEFINITION", "$", $"Die Definition verletzt eine Domaininvariante: {ex.Message}"));
                return null;
            }
        }
    }
}
