using System;
using System.Collections.Generic;
using STP.Application;
using UnityEngine;

namespace STP.Presentation.World
{
    /// <summary>Arc-length presentation of the committed player's path; never computes a solution.</summary>
    public sealed class ProductionTrainRide : IDisposable
    {
        private readonly List<Segment> segments = new List<Segment>();
        private readonly DevelopmentCell[] path;
        private readonly float[] cellEnds;
        private readonly List<DevelopmentCell> visited = new List<DevelopmentCell>();
        private readonly IReadOnlyList<DevelopmentCell> visitedView;
        private readonly float speedTime;
        private float length;
        private float elapsed;

        /// <summary>Validates and copies an already built A-to-B path. Coordinates use top-left cell units.</summary>
        public ProductionTrainRide(IReadOnlyList<DevelopmentCell> builtPath, DevelopmentEndpoint a,
            DevelopmentEndpoint b, int width, int height, float secondsPerCell = .13f)
        {
            if (builtPath == null || builtPath.Count == 0) throw new ArgumentException("RIDE_PATH_EMPTY", nameof(builtPath));
            if (width < 2 || height < 2 || width > 32 || height > 32) throw new ArgumentOutOfRangeException(nameof(width));
            if (float.IsNaN(secondsPerCell) || float.IsInfinity(secondsPerCell) || secondsPerCell <= 0)
                throw new ArgumentOutOfRangeException(nameof(secondsPerCell));
            speedTime = secondsPerCell;
            path = new DevelopmentCell[builtPath.Count];
            cellEnds = new float[path.Length];
            var seen = new HashSet<int>();
            for (var i = 0; i < path.Length; i++)
            {
                var cell = builtPath[i];
                if (cell.X < 0 || cell.Y < 0 || cell.X >= width || cell.Y >= height || !seen.Add(cell.Y * width + cell.X))
                    throw new ArgumentException("RIDE_PATH_BOUNDS_OR_DUPLICATE", nameof(builtPath));
                if (i > 0 && Math.Abs(cell.X - path[i - 1].X) + Math.Abs(cell.Y - path[i - 1].Y) != 1)
                    throw new ArgumentException("RIDE_PATH_DISCONNECTED", nameof(builtPath));
                path[i] = cell;
            }
            if (path[0].X != a.X || path[0].Y != a.Y || path[path.Length - 1].X != b.X || path[path.Length - 1].Y != b.Y)
                throw new ArgumentException("RIDE_ENDPOINT_MISMATCH");
            CheckExterior(a, width, height); CheckExterior(b, width, height);
            var start = Center(path[0]) + Normal(a.Edge) * .5f;
            AddLine(start + Normal(a.Edge) * .55f, start);
            for (var i = 0; i < path.Length; i++)
            {
                var center = Center(path[i]);
                var entry = i == 0 ? Normal(a.Edge) : Center(path[i - 1]) - center;
                var exit = i == path.Length - 1 ? Normal(b.Edge) : Center(path[i + 1]) - center;
                if (entry == exit) throw new ArgumentException("RIDE_REVERSED_CELL");
                var from = center + entry * .5f;
                var to = center + exit * .5f;
                if (Vector2.Dot(entry, exit) < -.5f) AddLine(from, to);
                else
                {
                    var corner = center + (entry + exit) * .5f;
                    var angle = Mathf.Atan2(from.y - corner.y, from.x - corner.x);
                    var end = Mathf.Atan2(to.y - corner.y, to.x - corner.x);
                    var delta = Mathf.DeltaAngle(angle * Mathf.Rad2Deg, end * Mathf.Rad2Deg) * Mathf.Deg2Rad;
                    segments.Add(new Segment(corner, angle, delta));
                    length += Mathf.Abs(delta) * .5f;
                }
                cellEnds[i] = length;
            }
            var finish = Center(path[path.Length - 1]) + Normal(b.Edge) * .5f;
            AddLine(finish, finish + Normal(b.Edge) * .55f);
            visitedView = visited.AsReadOnly();
            UpdatePose(0);
        }

        /// <summary>Board-space x coordinate in cell units.</summary>
        public float X { get; private set; }
        /// <summary>Board-space y coordinate, increasing downwards.</summary>
        public float Y { get; private set; }
        /// <summary>Clockwise orientation in the board's screen coordinates.</summary>
        public float AngleDegrees { get; private set; }
        /// <summary>True only after the actual path and arrival hold have completed.</summary>
        public bool IsFinished { get; private set; }
        /// <summary>Whether this presentation lifetime has ended.</summary>
        public bool IsDisposed { get; private set; }
        /// <summary>Copied path cells passed in their genuine traversal order.</summary>
        public IReadOnlyList<DevelopmentCell> VisitedCells => visitedView;
        /// <summary>Total centerline length, including short exterior entry and exit.</summary>
        public float Length => length;

        /// <summary>Advances only presentation. The root grants no result until IsFinished.</summary>
        public void Tick(float seconds)
        {
            if (IsDisposed || IsFinished) return;
            if (float.IsNaN(seconds) || float.IsInfinity(seconds) || seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));
            elapsed += seconds;
            var distance = Mathf.Clamp((elapsed - .65f) / speedTime, 0, length);
            UpdatePose(distance);
            while (visited.Count < path.Length && distance >= cellEnds[visited.Count]) visited.Add(path[visited.Count]);
            IsFinished = elapsed >= .65f + length * speedTime + .35f;
        }

        /// <summary>Stops this presentation; it does not mutate any gameplay state.</summary>
        public void Dispose() => IsDisposed = true;

        private void AddLine(Vector2 from, Vector2 to)
        { segments.Add(new Segment(from, to)); length += Vector2.Distance(from, to); }
        private void UpdatePose(float distance)
        {
            var rear = Sample(Mathf.Max(0, distance - .17f));
            var front = Sample(Mathf.Min(length, distance + .17f));
            var point = (rear + front) * .5f;
            X = point.x; Y = point.y;
            AngleDegrees = Mathf.Atan2(front.y - rear.y, front.x - rear.x) * Mathf.Rad2Deg;
        }
        private Vector2 Sample(float distance)
        {
            foreach (var segment in segments)
            {
                if (distance <= segment.Length) return segment.At(distance / segment.Length);
                distance -= segment.Length;
            }
            return segments[segments.Count - 1].At(1);
        }
        private static Vector2 Center(DevelopmentCell cell) => new Vector2(cell.X + .5f, cell.Y + .5f);
        private static Vector2 Normal(DevelopmentEdge edge) => edge switch
        {
            DevelopmentEdge.N => Vector2.up * -1, DevelopmentEdge.E => Vector2.right,
            DevelopmentEdge.S => Vector2.up, DevelopmentEdge.W => Vector2.left,
            _ => throw new ArgumentOutOfRangeException(nameof(edge))
        };
        private static void CheckExterior(DevelopmentEndpoint port, int width, int height)
        {
            var valid = port.Edge == DevelopmentEdge.N ? port.Y == 0 : port.Edge == DevelopmentEdge.S ? port.Y == height - 1
                : port.Edge == DevelopmentEdge.W ? port.X == 0 : port.Edge == DevelopmentEdge.E && port.X == width - 1;
            if (!valid) throw new ArgumentException("RIDE_ENDPOINT_NOT_EXTERIOR");
        }
        private readonly struct Segment
        {
            private readonly Vector2 from;
            private readonly Vector2 to;
            private readonly float angle;
            private readonly float delta;
            private readonly bool arc;
            public Segment(Vector2 from, Vector2 to)
            { this.from = from; this.to = to; angle = delta = 0; arc = false; Length = Vector2.Distance(from, to); }
            public Segment(Vector2 center, float angle, float delta)
            { from = center; to = default; this.angle = angle; this.delta = delta; arc = true; Length = Mathf.Abs(delta) * .5f; }
            public float Length { get; }
            public Vector2 At(float t) => arc ? from + new Vector2(Mathf.Cos(angle + delta * t), Mathf.Sin(angle + delta * t)) * .5f : Vector2.Lerp(from, to, t);
        }
    }
}
