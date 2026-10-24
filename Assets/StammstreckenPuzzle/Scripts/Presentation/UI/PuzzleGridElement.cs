using System;
using STP.Application;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>QA grid presentation of visible cells only; it owns no puzzle rules or solution.</summary>
    public sealed class PuzzleGridElement : VisualElement, IDisposable
    {
        private readonly Button[] cells;
        private readonly TrackGlyph[] glyphs;
        private readonly DevelopmentTool[] displayed;
        private readonly bool[] selected;
        private readonly Label[] rowCounts;
        private readonly Label[] columnCounts;
        private readonly int[] previousRows;
        private readonly int[] previousColumns;
        private readonly VisualElement board;
        private readonly Image trainImage;
        private readonly Label endpointA;
        private readonly Label endpointB;
        private readonly string[,] rowTexts;
        private readonly string[,] columnTexts;
        private readonly string[,] cellDescriptions;
        private DevelopmentEndpoint portA;
        private DevelopmentEndpoint portB;
        private bool portsAssigned;
        private Action<int, int>? activation;

        /// <summary>Creates stable, focusable semantic cell targets without changing any domain state.</summary>
        public PuzzleGridElement(int width, int height, Action<int, int>? cellActivated = null)
        {
            if (width < 2 || width > 32 || height < 2 || height > 32)
                throw new ArgumentOutOfRangeException(nameof(width));
            GridWidth = width;
            GridHeight = height;
            activation = cellActivated;
            name = "qa-grid";
            AddToClassList("qa-grid");
            board = new VisualElement { name = "qa-grid-board" };
            board.AddToClassList("qa-grid-board");
            Add(board);
            cells = new Button[width * height];
            glyphs = new TrackGlyph[cells.Length];
            displayed = new DevelopmentTool[cells.Length];
            selected = new bool[cells.Length];
            cellDescriptions = new string[cells.Length, 9];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var cellX = x;
                    var cellY = y;
                    var index = y * width + x;
                    var button = new Button(() => activation?.Invoke(cellX, cellY))
                    {
                        name = CellName(x, y),
                        focusable = true,
                        tabIndex = index,
                    };
                    button.AddToClassList("qa-cell");
                    button.style.position = Position.Absolute;
                    var glyph = new TrackGlyph();
                    button.Add(glyph);
                    board.Add(button);
                    cells[index] = button;
                    glyphs[index] = glyph;
                }
            }

            trainImage = new Image { name = "qa-train-overlay", pickingMode = PickingMode.Ignore };
            trainImage.AddToClassList("qa-train-overlay");
            trainImage.scaleMode = ScaleMode.StretchToFill;
            trainImage.style.display = DisplayStyle.None;
            board.Add(trainImage);
            rowCounts = new Label[height];
            columnCounts = new Label[width];
            previousRows = new int[height];
            previousColumns = new int[width];
            rowTexts = new string[height, width + 1];
            columnTexts = new string[width, height + 1];
            for (var y = 0; y < height; y++)
            {
                previousRows[y] = -1;
                rowCounts[y] = MakeCount("qa-row-count-" + y);
                Add(rowCounts[y]);
            }
            for (var x = 0; x < width; x++)
            {
                previousColumns[x] = -1;
                columnCounts[x] = MakeCount("qa-column-count-" + x);
                Add(columnCounts[x]);
            }
            endpointA = MakePort("qa-endpoint-a");
            endpointB = MakePort("qa-endpoint-b");
            Add(endpointA);
            Add(endpointB);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        /// <summary>Width of the displayed public grid.</summary>
        public int GridWidth { get; }
        /// <summary>Height of the displayed public grid.</summary>
        public int GridHeight { get; }
        /// <summary>The inner board excludes all count and endpoint gutters.</summary>
        public VisualElement Board => board;

        /// <summary>Returns the stable cell control for semantic pointer/focus automation.</summary>
        public Button CellButton(int x, int y)
        {
            if (x < 0 || x >= GridWidth || y < 0 || y >= GridHeight)
                throw new ArgumentOutOfRangeException(nameof(x));
            return cells[y * GridWidth + x];
        }

        /// <summary>Returns whether a cell is selected; selection never changes its contents.</summary>
        public bool IsCellSelected(int x, int y) => selected[y * GridWidth + x];

        /// <summary>Stable technical identifier, never used as visible copy.</summary>
        public static string CellName(int x, int y) => "qa-cell-" + x + "-" + y;

        /// <summary>Prepares localized labels once for the injected public scenario.</summary>
        public void Configure(DevelopmentPuzzleSession model, Func<string, object[], string> localize)
        {
            if (model == null || model.Width != GridWidth || model.Height != GridHeight)
                throw new ArgumentException("QA_GRID_DIMENSIONS");
            if (localize == null) throw new ArgumentNullException(nameof(localize));
            for (var y = 0; y < GridHeight; y++)
                for (var value = 0; value <= GridWidth; value++)
                    rowTexts[y, value] = localize("qa.wp038.count", new object[] { value, model.RowTarget(y) });
            for (var x = 0; x < GridWidth; x++)
                for (var value = 0; value <= GridHeight; value++)
                    columnTexts[x, value] = localize("qa.wp038.count", new object[] { value, model.ColumnTarget(x) });
            for (var y = 0; y < GridHeight; y++)
                for (var x = 0; x < GridWidth; x++)
                    for (var tool = 0; tool < 9; tool++)
                        cellDescriptions[y * GridWidth + x, tool] = localize("qa.wp038.cell", new object[]
                        {
                            x + 1, y + 1, localize(DevelopmentPuzzlePresenter.ToolKey((DevelopmentTool)tool), Array.Empty<object>()),
                        });
            endpointA.text = localize("qa.wp038.endpoint.a", Array.Empty<object>());
            endpointB.text = localize("qa.wp038.endpoint.b", Array.Empty<object>());
            portA = model.EndpointA;
            portB = model.EndpointB;
            portsAssigned = true;
            LayoutBoard();
        }

        /// <summary>Updates changed visible glyphs/counts without rebuilding the element tree.</summary>
        public void Refresh(DevelopmentPuzzleSession model, bool[] selection, bool interactive)
        {
            if (selection == null || selection.Length != cells.Length)
                throw new ArgumentException("QA_GRID_SELECTION");
            for (var y = 0; y < GridHeight; y++)
            {
                for (var x = 0; x < GridWidth; x++)
                {
                    var index = y * GridWidth + x;
                    var tool = model.CellAt(x, y);
                    if (tool != displayed[index] || string.IsNullOrEmpty(cells[index].tooltip))
                    {
                        displayed[index] = tool;
                        glyphs[index].SetContent(tool);
                        cells[index].tooltip = cellDescriptions[index, (int)tool];
                    }
                    if (selected[index] != selection[index])
                    {
                        selected[index] = selection[index];
                        cells[index].EnableInClassList("qa-cell--selected", selected[index]);
                    }
                    if (cells[index].enabledSelf != interactive) cells[index].SetEnabled(interactive);
                }
                var visible = model.RowVisible(y);
                if (visible != previousRows[y])
                {
                    previousRows[y] = visible;
                    rowCounts[y].text = rowTexts[y, visible];
                    SetCountState(rowCounts[y], visible, model.RowTarget(y));
                }
            }
            for (var x = 0; x < GridWidth; x++)
            {
                var visible = model.ColumnVisible(x);
                if (visible != previousColumns[x])
                {
                    previousColumns[x] = visible;
                    columnCounts[x].text = columnTexts[x, visible];
                    SetCountState(columnCounts[x], visible, model.ColumnTarget(x));
                }
            }
        }

        /// <summary>Sets an explicitly supplied transparent train texture over the inner board only.</summary>
        public void SetTrainTexture(Texture? texture)
        {
            trainImage.image = texture;
            trainImage.style.display = texture == null ? DisplayStyle.None : DisplayStyle.Flex;
        }

        /// <summary>Removes the lifetime-bound activation delegate and external texture reference.</summary>
        public void Dispose()
        {
            activation = null;
            SetTrainTexture(null);
            SetEnabled(false);
            UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private static Label MakeCount(string id)
        {
            var result = new Label { name = id, pickingMode = PickingMode.Ignore };
            result.AddToClassList("qa-count");
            result.style.position = Position.Absolute;
            return result;
        }

        private static Label MakePort(string id)
        {
            var result = new Label { name = id, pickingMode = PickingMode.Ignore };
            result.AddToClassList("qa-endpoint");
            result.style.position = Position.Absolute;
            return result;
        }

        private static void SetCountState(Label label, int visible, int target)
        {
            label.EnableInClassList("qa-count--exact", visible == target);
            label.EnableInClassList("qa-count--exceeded", visible > target);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt) => LayoutBoard();

        private void LayoutBoard()
        {
            const float left = 72;
            const float top = 48;
            const float right = 30;
            const float bottom = 30;
            var availableWidth = contentRect.width - left - right;
            var availableHeight = contentRect.height - top - bottom;
            if (availableWidth <= 0 || availableHeight <= 0) return;
            var size = Mathf.Min(availableWidth / GridWidth, availableHeight / GridHeight);
            var boardWidth = size * GridWidth;
            var boardHeight = size * GridHeight;
            var originX = left + (availableWidth - boardWidth) * 0.5f;
            var originY = top + (availableHeight - boardHeight) * 0.5f;
            SetRect(board, originX, originY, boardWidth, boardHeight);
            for (var y = 0; y < GridHeight; y++)
            {
                SetRect(rowCounts[y], originX - 72, originY + y * size, 42, size);
                for (var x = 0; x < GridWidth; x++)
                    SetRect(cells[y * GridWidth + x], x * size, y * size, size, size);
            }
            for (var x = 0; x < GridWidth; x++)
                SetRect(columnCounts[x], originX + x * size, originY - 47, size, 24);
            SetRect(trainImage, 0, 0, boardWidth, boardHeight);
            if (portsAssigned)
            {
                LayoutPort(endpointA, portA, originX, originY, size);
                LayoutPort(endpointB, portB, originX, originY, size);
            }
        }

        private static void LayoutPort(Label label, DevelopmentEndpoint port, float ox, float oy, float size)
        {
            var x = ox + (port.X + 0.5f) * size - 11;
            var y = oy + (port.Y + 0.5f) * size - 11;
            switch (port.Edge)
            {
                case DevelopmentEdge.N: y = oy - 23; break;
                case DevelopmentEdge.S: y = oy + size * (port.Y + 1) + 1; break;
                case DevelopmentEdge.W: x = ox - 23; break;
                case DevelopmentEdge.E: x = ox + size * (port.X + 1) + 1; break;
            }
            SetRect(label, x, y, 22, 22);
        }

        private static void SetRect(VisualElement element, float x, float y, float width, float height)
        {
            element.style.left = x;
            element.style.top = y;
            element.style.width = width;
            element.style.height = height;
        }

        private sealed class TrackGlyph : VisualElement
        {
            private static readonly Color Track = new Color(0.36f, 0.22f, 0.13f);
            private static readonly Color Note = new Color(0.48f, 0.51f, 0.53f);
            private static readonly Color Empty = new Color(0.74f, 0.15f, 0.13f);
            private DevelopmentTool content;

            public TrackGlyph()
            {
                pickingMode = PickingMode.Ignore;
                style.position = Position.Absolute;
                style.left = 0;
                style.top = 0;
                style.right = 0;
                style.bottom = 0;
                generateVisualContent += Draw;
            }

            public void SetContent(DevelopmentTool value)
            {
                if (content == value) return;
                content = value;
                MarkDirtyRepaint();
            }

            private void Draw(MeshGenerationContext context)
            {
                var width = contentRect.width;
                var height = contentRect.height;
                if (width <= 0 || height <= 0 || content == DevelopmentTool.Clear) return;
                var painter = context.painter2D;
                painter.lineWidth = Mathf.Max(3, Mathf.Min(width, height) * 0.12f);
                painter.lineCap = LineCap.Round;
                painter.lineJoin = LineJoin.Round;
                if (content == DevelopmentTool.EmptyMarker)
                {
                    painter.strokeColor = Empty;
                    Stroke(painter, new Vector2(width * 0.33f, height * 0.33f), new Vector2(width * 0.67f, height * 0.67f));
                    Stroke(painter, new Vector2(width * 0.67f, height * 0.33f), new Vector2(width * 0.33f, height * 0.67f));
                    return;
                }
                if (content == DevelopmentTool.OccupiedMarker)
                {
                    painter.strokeColor = Note;
                    painter.lineWidth = Mathf.Max(2, width * 0.055f);
                    Stroke(painter, new Vector2(width * 0.3f, height * 0.43f), new Vector2(width * 0.7f, height * 0.43f));
                    Stroke(painter, new Vector2(width * 0.3f, height * 0.57f), new Vector2(width * 0.7f, height * 0.57f));
                    for (var i = 0; i < 3; i++)
                    {
                        var x = width * (0.36f + i * 0.14f);
                        Stroke(painter, new Vector2(x, height * 0.35f), new Vector2(x, height * 0.65f));
                    }
                    return;
                }
                painter.strokeColor = Track;
                var center = new Vector2(width * 0.5f, height * 0.5f);
                var north = new Vector2(center.x, 0);
                var east = new Vector2(width, center.y);
                var south = new Vector2(center.x, height);
                var west = new Vector2(0, center.y);
                switch (content)
                {
                    case DevelopmentTool.TrackNS: Stroke(painter, north, south); break;
                    case DevelopmentTool.TrackEW: Stroke(painter, west, east); break;
                    case DevelopmentTool.TrackNE: Curve(painter, north, center, east); break;
                    case DevelopmentTool.TrackES: Curve(painter, east, center, south); break;
                    case DevelopmentTool.TrackSW: Curve(painter, south, center, west); break;
                    case DevelopmentTool.TrackWN: Curve(painter, west, center, north); break;
                }
            }

            private static void Stroke(Painter2D painter, Vector2 from, Vector2 to)
            {
                painter.BeginPath();
                painter.MoveTo(from);
                painter.LineTo(to);
                painter.Stroke();
            }

            private static void Curve(Painter2D painter, Vector2 from, Vector2 center, Vector2 to)
            {
                painter.BeginPath();
                painter.MoveTo(from);
                painter.BezierCurveTo(Vector2.Lerp(from, center, 0.75f), Vector2.Lerp(to, center, 0.75f), to);
                painter.Stroke();
            }
        }
    }
}
