using System;
using System.Collections.Generic;
using System.Globalization;
using STP.Puzzle.Domain;

namespace STP.Infrastructure.Content
{
    /// <summary>Result of the Domain-map stage: the integrated <see cref="PuzzleDefinition"/> or stable mapping diagnostics.</summary>
    public sealed class LevelV2MapResult
    {
        public PuzzleDefinition? Definition { get; }
        public IReadOnlyList<LevelDiagnostic> Diagnostics { get; }
        public bool Succeeded => Definition != null && Diagnostics.Count == 0;

        internal LevelV2MapResult(PuzzleDefinition? definition, IReadOnlyList<LevelDiagnostic> diagnostics) { Definition = definition; Diagnostics = diagnostics; }
    }

    /// <summary>
    /// Deterministic mapping of the public part of a schema-valid Level-v2 document to the integrated Domain
    /// <see cref="PuzzleDefinition"/>. Acceptance is decided exclusively by the Domain factories
    /// (<c>GridSize.Create</c>, <c>Endpoint.Create</c>, <c>PuzzleDefinition.Create</c>); when they reject, the classification
    /// below only attributes the rejection to a stable author-facing code and never accepts or rejects on its own.
    /// The authoring solution is not part of the mapped definition.
    /// </summary>
    public static class LevelV2DomainMapper
    {
        public static LevelV2MapResult Map(LevelV2Document? document)
        {
            var diagnostics = new List<LevelDiagnostic>();
            if (document == null)
            {
                diagnostics.Add(Diagnostic(LevelDiagnosticCodes.DomainInvalidDefinition, string.Empty, "No document to map."));
                return new LevelV2MapResult(null, diagnostics);
            }

            var grid = GridSize.Create(document.Grid.Width, document.Grid.Height).Value;
            if (grid == null)
            {
                diagnostics.Add(Diagnostic(LevelDiagnosticCodes.DomainInvalidDefinition, "/grid", "The Domain rejected the grid size."));
                return new LevelV2MapResult(null, diagnostics);
            }

            var a = Endpoint.Create(grid, ToDirection(document.EndpointA.Side), document.EndpointA.Index).Value;
            var b = Endpoint.Create(grid, ToDirection(document.EndpointB.Side), document.EndpointB.Index).Value;
            if (a == null) diagnostics.Add(Diagnostic(LevelDiagnosticCodes.EndpointRange, "/endpoints/a/index", "Endpoint index is outside the side length of the grid."));
            if (b == null) diagnostics.Add(Diagnostic(LevelDiagnosticCodes.EndpointRange, "/endpoints/b/index", "Endpoint index is outside the side length of the grid."));
            if (a == null || b == null) return new LevelV2MapResult(null, diagnostics);

            var created = PuzzleDefinition.Create(grid, a, b, document.RowCounts, document.ColumnCounts, document.RulesetVersion);
            if (created.Value != null) return new LevelV2MapResult(created.Value, diagnostics);

            if (created.Error == DomainError.UNSUPPORTED_RULESET)
            {
                diagnostics.Add(Diagnostic(LevelDiagnosticCodes.DomainUnsupportedRuleset, "/rulesetVersion", "The Domain does not support this ruleset."));
                return new LevelV2MapResult(null, diagnostics);
            }
            Classify(document, a, b, diagnostics);
            if (diagnostics.Count == 0) diagnostics.Add(Diagnostic(LevelDiagnosticCodes.DomainInvalidDefinition, string.Empty, "The Domain rejected the public puzzle definition (" + created.Error.ToString() + ")."));
            return new LevelV2MapResult(null, diagnostics);
        }

        /// <summary>Attributes a Domain rejection to author-facing codes; purely diagnostic.</summary>
        private static void Classify(LevelV2Document document, Endpoint a, Endpoint b, List<LevelDiagnostic> diagnostics)
        {
            int width = document.Grid.Width, height = document.Grid.Height;
            if (document.RowCounts.Count != height) diagnostics.Add(Diagnostic(LevelDiagnosticCodes.GridRowCountLength, "/rowCounts", "rowCounts must have exactly height entries."));
            if (document.ColumnCounts.Count != width) diagnostics.Add(Diagnostic(LevelDiagnosticCodes.GridColumnCountLength, "/columnCounts", "columnCounts must have exactly width entries."));
            if (document.EndpointA.Side == document.EndpointB.Side && document.EndpointA.Index == document.EndpointB.Index)
                diagnostics.Add(Diagnostic(LevelDiagnosticCodes.EndpointIdentical, "/endpoints/b", "Endpoints A and B must be different exterior connections."));
            int sumRows = 0, sumColumns = 0;
            for (int i = 0; i < document.RowCounts.Count; i++)
            {
                sumRows += document.RowCounts[i];
                if (document.RowCounts[i] > width) diagnostics.Add(Diagnostic(LevelDiagnosticCodes.GridRowCountValue, "/rowCounts/" + i.ToString(CultureInfo.InvariantCulture), "A row count cannot exceed the grid width."));
            }
            for (int i = 0; i < document.ColumnCounts.Count; i++)
            {
                sumColumns += document.ColumnCounts[i];
                if (document.ColumnCounts[i] > height) diagnostics.Add(Diagnostic(LevelDiagnosticCodes.GridColumnCountValue, "/columnCounts/" + i.ToString(CultureInfo.InvariantCulture), "A column count cannot exceed the grid height."));
            }
            if (document.RowCounts.Count == height && document.ColumnCounts.Count == width)
            {
                if (sumRows < 1 || sumRows != sumColumns) diagnostics.Add(Diagnostic(LevelDiagnosticCodes.GridCountSum, "/rowCounts", "Row and column counts must have the same positive sum."));
                CheckEndpointCounts(document, a, "/endpoints/a", diagnostics);
                CheckEndpointCounts(document, b, "/endpoints/b", diagnostics);
            }
        }

        private static void CheckEndpointCounts(LevelV2Document document, Endpoint endpoint, string path, List<LevelDiagnostic> diagnostics)
        {
            if (document.RowCounts[endpoint.Cell.Y] == 0 || document.ColumnCounts[endpoint.Cell.X] == 0)
                diagnostics.Add(Diagnostic(LevelDiagnosticCodes.EndpointCountZero, path, "The row and column of an endpoint cell must have a positive count."));
        }

        private static LevelDiagnostic Diagnostic(string code, string path, string message) => new LevelDiagnostic(LevelDiagnosticStage.DomainMap, code, path, message);

        internal static Direction ToDirection(LevelSide side)
        {
            switch (side)
            {
                case LevelSide.N: return Direction.N;
                case LevelSide.E: return Direction.E;
                case LevelSide.S: return Direction.S;
                default: return Direction.W;
            }
        }

        internal static CellContent ToCellContent(LevelTrack track)
        {
            switch (track)
            {
                case LevelTrack.TRACK_NS: return CellContent.TRACK_NS;
                case LevelTrack.TRACK_EW: return CellContent.TRACK_EW;
                case LevelTrack.TRACK_NE: return CellContent.TRACK_NE;
                case LevelTrack.TRACK_ES: return CellContent.TRACK_ES;
                case LevelTrack.TRACK_SW: return CellContent.TRACK_SW;
                default: return CellContent.TRACK_WN;
            }
        }
    }
}
