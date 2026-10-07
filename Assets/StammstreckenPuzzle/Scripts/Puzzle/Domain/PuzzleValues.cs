using System;
using System.Collections.Generic;

namespace STP.Puzzle.Domain
{
    /// <summary>Stable input and command failures; NONE denotes a successful operation.</summary>
    public enum DomainError { NONE, INVALID_DEFINITION, OUT_OF_BOUNDS, STALE_COMMAND, DUPLICATE_COMMAND, SESSION_CLOSED, UNSUPPORTED_RULESET, INVALID_COMMAND, INVALID_TIME, INTERNAL_INVARIANT_BROKEN }
    /// <summary>A total factory result. Value is absent on failure.</summary>
    public sealed class DomainResult<T> where T : class
    {
        public T? Value { get; }
        public DomainError Error { get; }
        public bool Succeeded => Error == DomainError.NONE;
        private DomainResult(T? value, DomainError error) { Value = value; Error = error; }
        internal static DomainResult<T> Ok(T value) => new DomainResult<T>(value, DomainError.NONE);
        internal static DomainResult<T> Fail(DomainError error) => new DomainResult<T>(null, error);
    }
    /// <summary>Clockwise raster directions, with y growing downwards.</summary>
    public enum Direction { N, E, S, W }
    /// <summary>The six two-port shapes, in the fixed recovery search order.</summary>
    public enum TrackShape { NS, EW, NE, ES, SW, WN }
    /// <summary>Visible cell content. Markers have no concrete ports.</summary>
    public enum CellContent { UNSET, MARK_EMPTY, MARK_OCCUPIED, TRACK_NS, TRACK_EW, TRACK_NE, TRACK_ES, TRACK_SW, TRACK_WN }
    /// <summary>Validated technical grid dimensions, each in 2..32.</summary>
    public sealed class GridSize
    {
        public int Width { get; }
        public int Height { get; }
        public int CellCount => Width * Height;
        private GridSize(int width, int height) { Width = width; Height = height; }
        public static DomainResult<GridSize> Create(int width, int height) => width < 2 || width > 32 || height < 2 || height > 32
            ? DomainResult<GridSize>.Fail(DomainError.INVALID_DEFINITION) : DomainResult<GridSize>.Ok(new GridSize(width, height));
        public bool Contains(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;
    }
    /// <summary>A coordinate validated relative to a grid, never a global board location.</summary>
    public sealed class CellCoordinate
    {
        public int X { get; }
        public int Y { get; }
        internal CellCoordinate(int x, int y) { X = x; Y = y; }
        public static DomainResult<CellCoordinate> Create(GridSize? grid, int x, int y) => grid == null || !grid.Contains(x, y)
            ? DomainResult<CellCoordinate>.Fail(DomainError.OUT_OF_BOUNDS) : DomainResult<CellCoordinate>.Ok(new CellCoordinate(x, y));
    }
    /// <summary>A validated exterior edge and its adjacent interior cell.</summary>
    public sealed class Endpoint
    {
        public Direction Side { get; }
        public int Index { get; }
        public CellCoordinate Cell { get; }
        private Endpoint(GridSize grid, Direction side, int index)
        {
            Side = side; Index = index;
            Cell = new CellCoordinate(side == Direction.W ? 0 : side == Direction.E ? grid.Width - 1 : index,
                side == Direction.N ? 0 : side == Direction.S ? grid.Height - 1 : index);
        }
        public static DomainResult<Endpoint> Create(GridSize? grid, Direction side, int index)
        {
            if (grid == null || (int)side < 0 || (int)side > 3 || index < 0 || index >= (side == Direction.N || side == Direction.S ? grid.Width : grid.Height))
                return DomainResult<Endpoint>.Fail(DomainError.INVALID_DEFINITION);
            return DomainResult<Endpoint>.Ok(new Endpoint(grid, side, index));
        }
        internal bool Matches(int x, int y, Direction side) => side == Side && x == Cell.X && y == Cell.Y;
    }
    /// <summary>Closed shape geometry. Invalid enum values produce no ports.</summary>
    public static class TrackGeometry
    {
        public static Direction Opposite(Direction direction) => (Direction)(((int)direction + 2) & 3);
        public static bool IsTrack(CellContent content) => content >= CellContent.TRACK_NS && content <= CellContent.TRACK_WN;
        public static bool IsContent(CellContent content) => content >= CellContent.UNSET && content <= CellContent.TRACK_WN;
        public static int Ports(TrackShape shape) => shape switch { TrackShape.NS => 5, TrackShape.EW => 10, TrackShape.NE => 3, TrackShape.ES => 6, TrackShape.SW => 12, TrackShape.WN => 9, _ => 0 };
        public static int Ports(CellContent content) => IsTrack(content) ? Ports((TrackShape)((int)content - 3)) : 0;
        public static int DeltaX(Direction direction) => direction == Direction.E ? 1 : direction == Direction.W ? -1 : 0;
        public static int DeltaY(Direction direction) => direction == Direction.S ? 1 : direction == Direction.N ? -1 : 0;
    }
    /// <summary>Immutable public puzzle input; contains no authoring solution or proof.</summary>
    public sealed class PuzzleDefinition
    {
        public GridSize Grid { get; }
        public Endpoint A { get; }
        public Endpoint B { get; }
        public IReadOnlyList<int> RowCounts { get; }
        public IReadOnlyList<int> ColumnCounts { get; }
        public string RulesetVersion { get; }
        private PuzzleDefinition(GridSize grid, Endpoint a, Endpoint b, int[] rows, int[] columns, string ruleset)
        { Grid = grid; A = a; B = b; RowCounts = Array.AsReadOnly(rows); ColumnCounts = Array.AsReadOnly(columns); RulesetVersion = ruleset; }
        public static DomainResult<PuzzleDefinition> Create(GridSize? grid, Endpoint? a, Endpoint? b, IReadOnlyList<int>? rows, IReadOnlyList<int>? columns, string? ruleset)
        {
            if (ruleset != "train-track-v1") return DomainResult<PuzzleDefinition>.Fail(DomainError.UNSUPPORTED_RULESET);
            if (grid == null || a == null || b == null || rows == null || columns == null || rows.Count != grid.Height || columns.Count != grid.Width)
                return DomainResult<PuzzleDefinition>.Fail(DomainError.INVALID_DEFINITION);
            var aa = Endpoint.Create(grid, a.Side, a.Index).Value; var bb = Endpoint.Create(grid, b.Side, b.Index).Value;
            if (aa == null || bb == null || (a.Side == b.Side && a.Index == b.Index)) return DomainResult<PuzzleDefinition>.Fail(DomainError.INVALID_DEFINITION);
            int sumRows = 0, sumCols = 0; var r = new int[rows.Count]; var c = new int[columns.Count];
            for (int i = 0; i < r.Length; i++) { r[i] = rows[i]; if (r[i] < 0 || r[i] > grid.Width) return DomainResult<PuzzleDefinition>.Fail(DomainError.INVALID_DEFINITION); sumRows += r[i]; }
            for (int i = 0; i < c.Length; i++) { c[i] = columns[i]; if (c[i] < 0 || c[i] > grid.Height) return DomainResult<PuzzleDefinition>.Fail(DomainError.INVALID_DEFINITION); sumCols += c[i]; }
            if (sumRows < 1 || sumRows != sumCols || r[aa.Cell.Y] == 0 || c[aa.Cell.X] == 0 || r[bb.Cell.Y] == 0 || c[bb.Cell.X] == 0)
                return DomainResult<PuzzleDefinition>.Fail(DomainError.INVALID_DEFINITION);
            return DomainResult<PuzzleDefinition>.Ok(new PuzzleDefinition(grid, aa, bb, r, c, ruleset));
        }
        public bool IsEndpointPort(int x, int y, Direction direction) => A.Matches(x, y, direction) || B.Matches(x, y, direction);
        public int Index(CellCoordinate? coordinate) => coordinate != null && Grid.Contains(coordinate.X, coordinate.Y) ? coordinate.Y * Grid.Width + coordinate.X : -1;
    }
}
