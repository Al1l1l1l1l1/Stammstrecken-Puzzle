using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace STP.Puzzle.Domain
{
    /// <summary>
    /// Unveränderliche, bei Konstruktion vollständig validierte Puzzledefinition
    /// gemäß PUZZLE_ENGINE.md Abschnitt 3. Trägt Raster, die Außenanschlüsse A/B
    /// und die Zielzahlen aller Zeilen und Spalten. Eindeutigkeit ist nicht Teil
    /// der Konstruktion, sondern des Content-Gates (Solver/Validator).
    /// Randzahlen werden defensiv kopiert und ausschließlich schreibgeschützt
    /// exponiert; eine externe Mutation nach der Konstruktion ist nicht möglich.
    /// </summary>
    public sealed class PuzzleDefinition
    {
        /// <summary>Derzeit registrierte Ruleset-Version des Rätselregelvertrags.</summary>
        public const string RulesetVersionV1 = "train-track-v1";

        private readonly int[] _rowCounts;
        private readonly int[] _columnCounts;
        private readonly ReadOnlyCollection<int> _rowCountsView;
        private readonly ReadOnlyCollection<int> _columnCountsView;

        /// <summary>Rastergröße.</summary>
        public GridSize Grid { get; }

        /// <summary>Äußerer Anschlusspunkt A.</summary>
        public Endpoint A { get; }

        /// <summary>Äußerer Anschlusspunkt B.</summary>
        public Endpoint B { get; }

        /// <summary>Zielzahl belegter Zellen je Zeile (Länge <see cref="GridSize.Height"/>), schreibgeschützt.</summary>
        public IReadOnlyList<int> RowCounts => _rowCountsView;

        /// <summary>Zielzahl belegter Zellen je Spalte (Länge <see cref="GridSize.Width"/>), schreibgeschützt.</summary>
        public IReadOnlyList<int> ColumnCounts => _columnCountsView;

        /// <summary>Registrierte Ruleset-Version dieser Definition.</summary>
        public string RulesetVersion { get; }

        /// <summary>
        /// Erstellt eine validierte Definition. Jeder Verstoß gegen die
        /// Invarianten aus PUZZLE_ENGINE.md Abschnitt 3 führt zu einer
        /// <see cref="ArgumentException"/> mit dem Diagnosecode
        /// <c>INVALID_DEFINITION</c>; ungültige Definitionen können nicht
        /// konstruiert werden.
        /// </summary>
        public PuzzleDefinition(
            GridSize grid,
            Endpoint a,
            Endpoint b,
            IReadOnlyList<int> rowCounts,
            IReadOnlyList<int> columnCounts,
            string rulesetVersion)
        {
            if (rulesetVersion != RulesetVersionV1)
            {
                throw Invalid($"UNSUPPORTED_RULESET: '{rulesetVersion}' ist nicht registriert.");
            }
            if (a == b)
            {
                throw Invalid("INVALID_DEFINITION: A und B müssen unterschiedliche Außenanschlüsse sein.");
            }
            if (!a.IsValidFor(grid))
            {
                throw Invalid($"INVALID_DEFINITION: Endpoint-A-Index {a.Index} passt nicht zur Achse {a.Side} von {grid}.");
            }
            if (!b.IsValidFor(grid))
            {
                throw Invalid($"INVALID_DEFINITION: Endpoint-B-Index {b.Index} passt nicht zur Achse {b.Side} von {grid}.");
            }
            if (rowCounts is null || rowCounts.Count != grid.Height)
            {
                throw Invalid($"INVALID_DEFINITION: rowCounts benötigt Länge {grid.Height}.");
            }
            if (columnCounts is null || columnCounts.Count != grid.Width)
            {
                throw Invalid($"INVALID_DEFINITION: columnCounts benötigt Länge {grid.Width}.");
            }

            var rows = new int[grid.Height];
            var rowSum = 0;
            for (var y = 0; y < grid.Height; y++)
            {
                if (rowCounts[y] < 0 || rowCounts[y] > grid.Width)
                {
                    throw Invalid($"INVALID_DEFINITION: rowCounts[{y}]={rowCounts[y]} außerhalb von 0..{grid.Width}.");
                }
                rows[y] = rowCounts[y];
                rowSum += rowCounts[y];
            }

            var columns = new int[grid.Width];
            var columnSum = 0;
            for (var x = 0; x < grid.Width; x++)
            {
                if (columnCounts[x] < 0 || columnCounts[x] > grid.Height)
                {
                    throw Invalid($"INVALID_DEFINITION: columnCounts[{x}]={columnCounts[x]} außerhalb von 0..{grid.Height}.");
                }
                columns[x] = columnCounts[x];
                columnSum += columnCounts[x];
            }

            if (rowSum != columnSum || rowSum < 1)
            {
                throw Invalid(
                    $"INVALID_DEFINITION: Zeilensumme {rowSum} muss gleich Spaltensumme und mindestens 1 sein (Spaltensumme {columnSum}).");
            }

            RequireOccupiable(grid, rows, columns, a);
            RequireOccupiable(grid, rows, columns, b);

            Grid = grid;
            A = a;
            B = b;
            _rowCounts = rows;
            _columnCounts = columns;
            _rowCountsView = new ReadOnlyCollection<int>(_rowCounts);
            _columnCountsView = new ReadOnlyCollection<int>(_columnCounts);
            RulesetVersion = rulesetVersion;
        }

        /// <summary>
        /// Prüft, ob die Koordinate am betreffenden Rand einen Endpoint besitzt
        /// (A oder B), und liefert diesen gegebenenfalls zurück.
        /// </summary>
        public bool TryGetEndpointAt(CellCoordinate coordinate, Direction outsideDirection, out Endpoint endpoint)
        {
            if (IsEndpointAt(A, coordinate, outsideDirection))
            {
                endpoint = A;
                return true;
            }
            if (IsEndpointAt(B, coordinate, outsideDirection))
            {
                endpoint = B;
                return true;
            }
            endpoint = default;
            return false;
        }

        /// <summary>Prüft, ob der gegebene Endpoint exakt an dieser Zellkante liegt.</summary>
        public bool IsEndpointAt(Endpoint endpoint, CellCoordinate coordinate, Direction outsideDirection)
        {
            return endpoint.Side == outsideDirection && endpoint.AdjacentCell(Grid) == coordinate;
        }

        private static void RequireOccupiable(GridSize grid, int[] rows, int[] columns, Endpoint endpoint)
        {
            var cell = endpoint.AdjacentCell(grid);
            if (rows[cell.Y] == 0 || columns[cell.X] == 0)
            {
                throw Invalid(
                    $"INVALID_DEFINITION: Angrenzende Zelle {cell} des Endpoints {endpoint} kann laut Randzahlen nicht belegt sein.");
            }
        }

        private static ArgumentException Invalid(string message)
        {
            return new ArgumentException(message);
        }
    }
}
