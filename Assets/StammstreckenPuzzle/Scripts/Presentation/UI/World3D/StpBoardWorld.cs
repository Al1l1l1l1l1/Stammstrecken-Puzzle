using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>Uploads a software-projected, depth-sorted batch to the UI Toolkit mesh pipeline. No camera or render texture is required.</summary>
    internal static class StpPaint
    {
        internal static void Submit(MeshGenerationContext context, StpSink sink)
        {
            foreach (var batch in sink.Batches)
            {
                var vertexCount = batch.Pos.Count; var indexCount = batch.Idx.Count;
                if (vertexCount == 0 || indexCount == 0 || vertexCount > 65000) continue;
                var data = context.Allocate(vertexCount, indexCount);
                var vertices = new Vertex[vertexCount];
                for (var i = 0; i < vertexCount; i++)
                    vertices[i] = new Vertex { position = new Vector3(batch.Pos[i].x, batch.Pos[i].y, Vertex.nearZ), tint = batch.Col[i] };
                var indices = new ushort[indexCount];
                for (var i = 0; i < indexCount; i++) indices[i] = (ushort)batch.Idx[i];
                data.SetAllVertices(vertices);
                data.SetAllIndices(indices);
            }
        }
    }

    /// <summary>Recent path of the locomotive centre, used to place wagons on the genuine track behind it.</summary>
    internal sealed class StpTrail
    {
        private readonly List<Vector2> points = new List<Vector2>();
        private readonly List<float> cumulative = new List<float>();
        private Vector2 startDirection = Vector2.right;

        internal void Clear() { points.Clear(); cumulative.Clear(); }

        internal void Add(Vector2 position, float yawDegrees)
        {
            if (points.Count == 0)
            {
                startDirection = new Vector2(Mathf.Cos(yawDegrees * Mathf.Deg2Rad), Mathf.Sin(yawDegrees * Mathf.Deg2Rad));
                points.Add(position); cumulative.Add(0); return;
            }
            var step = Vector2.Distance(points[points.Count - 1], position);
            if (step < 0.004f) return;
            if (step > 1.5f) { Clear(); Add(position, yawDegrees); return; }
            points.Add(position); cumulative.Add(cumulative[cumulative.Count - 1] + step);
            if (points.Count > 400) { points.RemoveRange(0, 100); var shift = cumulative[99]; cumulative.RemoveRange(0, 100); for (var i = 0; i < cumulative.Count; i++) cumulative[i] -= shift; }
        }

        /// <summary>Position at a given path distance behind the newest sample; extrapolates straight behind the first sample.</summary>
        internal Vector2 Behind(float distance)
        {
            if (points.Count == 0) return Vector2.zero;
            var end = cumulative[cumulative.Count - 1];
            var target = end - distance;
            if (target <= 0) return points[0] - startDirection * (-target);
            var lo = 0; var hi = cumulative.Count - 1;
            while (hi - lo > 1) { var mid = (lo + hi) >> 1; if (cumulative[mid] <= target) lo = mid; else hi = mid; }
            var span = cumulative[hi] - cumulative[lo];
            return span < 1e-5f ? points[lo] : Vector2.Lerp(points[lo], points[hi], (target - cumulative[lo]) / span);
        }
    }

    /// <summary>Board-space 3D world: ground tiles, rim, rails, endpoints and the moving vehicle.</summary>
    internal static class StpBoardWorld
    {
        private static readonly Color GrassA = StpKit.H(0x8DD35F);
        private static readonly Color GrassB = StpKit.H(0x7FC954);
        private static readonly Color Rim = StpKit.H(0xE9DFC8);

        /// <summary>Code per cell: 0 N-S, 1 E-W, 2 N-E, 3 E-S, 4 S-W, 5 W-N, -1 none.</summary>
        internal static void PaintBoard(MeshGenerationContext context, Rect board, int width, int height, Func<int, int, int> codeAt, int edgeA, int ax, int ay, int edgeB, int bx, int by)
            => StpPaint.Submit(context, BuildBoard(board, width, height, codeAt, edgeA, ax, ay, edgeB, bx, by));

        internal static StpSink BuildBoard(Rect board, int width, int height, Func<int, int, int> codeAt, int edgeA, int ax, int ay, int edgeB, int bx, int by)
        {
            var sink = new StpSink();
            var scale = board.width / width;
            var view = new StpView { Scale = scale, Ox = board.x, Oy = board.y };
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    sink.Polygon(new[] { view.Project(new Vector3(x, y, 0)), view.Project(new Vector3(x + 1, y, 0)), view.Project(new Vector3(x + 1, y + 1, 0)), view.Project(new Vector3(x, y + 1, 0)) }, ((x + y) & 1) == 0 ? GrassA : GrassB);
            var scene = new StpScene(); var b = new StpBuilder();
            StpKit.Stub(b, edgeA, ax, ay); StpKit.Stub(b, edgeB, bx, by); scene.Add(b.Take(), 1, -100);
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                {
                    var code = codeAt(x, y);
                    if (code < 0) continue;
                    StpKit.TrackBallast(b, code, x, y); StpKit.TrackRails(b, code, x, y);
                    scene.Add(b.Take(), 1, 0, true);
                }
            // low bubbly rim around the tile
            const float t = 0.07f;
            b.Frame = StpFrame.Identity;
            b.Box(new Vector3(width * .5f, -t, 0.04f), new Vector3(width * .5f + t * 2, t, 0.04f), Rim, 0, true);
            b.Box(new Vector3(width * .5f, height + t, 0.04f), new Vector3(width * .5f + t * 2, t, 0.04f), Rim, 0, true);
            b.Box(new Vector3(-t, height * .5f, 0.04f), new Vector3(t, height * .5f, 0.04f), Rim, 0, true);
            b.Box(new Vector3(width + t, height * .5f, 0.04f), new Vector3(t, height * .5f, 0.04f), Rim, 0, true);
            scene.Add(b.Take(), 0, 0);
            scene.Emit(view, sink);
            return sink;
        }

        /// <summary>Top-down single track tile (tool dock icons), fitted into the rectangle.</summary>
        internal static void PaintTile(MeshGenerationContext context, Rect rect, int code)
            => StpPaint.Submit(context, BuildTile(rect, code));

        internal static StpSink BuildTile(Rect rect, int code)
        {
            var sink = new StpSink();
            if (code < 0 || rect.width < 2 || rect.height < 2) return sink;
            var sx = StpView.Sx; var sy = StpView.Sy;
            StpView.Use(0, 0);
            try
            {
                var s = Mathf.Min(rect.width, rect.height);
                var view = new StpView { Scale = s, Ox = rect.center.x - s * .5f, Oy = rect.center.y - s * .5f };
                var scene = new StpScene(); var b = new StpBuilder();
                StpKit.TrackBallast(b, code, 0, 0); StpKit.TrackRails(b, code, 0, 0);
                scene.Add(b.Take(), 1, 0, true);
                scene.Emit(view, sink);
            }
            finally { StpView.Use(sx, sy); }
            return sink;
        }

        /// <summary>Paints the locomotive and wagons; the pose is in board cell units with y growing downwards.</summary>
        internal static void PaintTrain(MeshGenerationContext context, Rect board, float cellSize, float x, float y, float yawDegrees, StpVehicle vehicle, StpTrail trail)
            => StpPaint.Submit(context, BuildTrain(cellSize, x, y, yawDegrees, vehicle, trail));

        internal static StpSink BuildTrain(float cellSize, float x, float y, float yawDegrees, StpVehicle vehicle, StpTrail trail)
        {
            if (cellSize <= 0) return new StpSink();
            var view = new StpView { Scale = cellSize, Ox = 0, Oy = 0 };
            var sink = new StpSink(); var scene = new StpScene(); var b = new StpBuilder();
            var cars = new List<Vector4>(); // x,y,yaw radians,kind (-1 loco)
            cars.Add(new Vector4(x, y, yawDegrees * Mathf.Deg2Rad, -1));
            var d = StpKit.Length(vehicle.Type) * 0.5f + 0.05f;
            foreach (var kind in vehicle.Wagons)
            {
                d += StpKit.WagonLength * 0.5f;
                var p = trail.Behind(d); var q0 = trail.Behind(d - 0.2f); var q1 = trail.Behind(d + 0.2f);
                var dir = q0 - q1;
                cars.Add(new Vector4(p.x, p.y, dir.sqrMagnitude < 1e-6f ? yawDegrees * Mathf.Deg2Rad : Mathf.Atan2(dir.y, dir.x), kind));
                d += StpKit.WagonLength * 0.5f + 0.05f;
            }
            // far cars first so the nearer ones overlap them
            for (var i = cars.Count - 1; i >= 0; i--)
            {
                var c = cars[i];
                b.Frame = new StpFrame(new Vector3(c.x, c.y, 0), c.z, 1);
                b.Shadow(new Vector3(0.02f, 0.03f, 0.006f), 0.58f, 0.2f, 0.28f);
                if (c.w < 0) StpKit.Loco(b, vehicle.Type, vehicle.Body); else StpKit.Wagon(b, (int)c.w, vehicle.Body);
                scene.Add(b.Take(), 2, i * -0.001f);
            }
            scene.Emit(view, sink);
            return sink;
        }

        internal static int CodeFor(STP.Application.DevelopmentTool tool)
        {
            switch (tool)
            {
                case STP.Application.DevelopmentTool.TrackNS: return 0;
                case STP.Application.DevelopmentTool.TrackEW: return 1;
                case STP.Application.DevelopmentTool.TrackNE: return 2;
                case STP.Application.DevelopmentTool.TrackES: return 3;
                case STP.Application.DevelopmentTool.TrackSW: return 4;
                case STP.Application.DevelopmentTool.TrackWN: return 5;
                default: return -1;
            }
        }

        internal static int EdgeIndex(STP.Application.DevelopmentEdge edge)
        {
            switch (edge)
            {
                case STP.Application.DevelopmentEdge.N: return 0;
                case STP.Application.DevelopmentEdge.E: return 1;
                case STP.Application.DevelopmentEdge.S: return 2;
                default: return 3;
            }
        }
    }

    /// <summary>Hand-composed railway dioramas for the five campaign worlds. All geometry is original and procedural.</summary>
    internal static class StpDiorama
    {
        internal static readonly Color[] SkyTop = { StpKit.H(0x7EC8F2), StpKit.H(0xFFC27A), StpKit.H(0x8FB8F5), StpKit.H(0x9AD3E8), StpKit.H(0xB49BE8) };
        internal static readonly Color[] SkyBottom = { StpKit.H(0xDDF3FF), StpKit.H(0xFFEBC8), StpKit.H(0xE6F0FF), StpKit.H(0xF2FBF4), StpKit.H(0xF5E5F8) };
        internal static readonly Color[] Ground = { StpKit.H(0x7BCB62), StpKit.H(0xC9B07A), StpKit.H(0x9CCB86), StpKit.H(0x6FBF7E), StpKit.H(0x8DA3B8) };

        /// <summary>Paints a complete diorama into the rectangle: sky, hills, ground, scenery, train and props.</summary>
        internal static void Paint(MeshGenerationContext context, Rect rect, int world, StpVehicle vehicle, IReadOnlyList<string> propKinds, RailwayRouteComposition? composition, int clearedStage)
            => StpPaint.Submit(context, Build(rect, world, vehicle, propKinds, composition, clearedStage));

        internal static StpSink Build(Rect rect, int world, StpVehicle vehicle, IReadOnlyList<string> propKinds, RailwayRouteComposition? composition, int clearedStage)
        {
            if (rect.width < 4 || rect.height < 4) return new StpSink();
            world = Mathf.Clamp(world, 0, 4);
            var sink = new StpSink();
            var camX = StpView.Sx; var camY = StpView.Sy;
            StpView.Use(0.16f, 0.68f);
            try { BuildInto(sink, rect, world, vehicle, propKinds, composition, clearedStage); }
            finally { StpView.Use(camX, camY); }
            return sink;
        }

        private static void BuildInto(StpSink sink, Rect rect, int world, StpVehicle vehicle, IReadOnlyList<string> propKinds, RailwayRouteComposition? composition, int clearedStage)
        {
            sink.Gradient(rect, SkyTop[world], SkyBottom[world]);
            var scale = rect.height / 4.3f;
            var view = new StpView { Scale = scale, Ox = rect.center.x, Oy = rect.yMin + 1.15f * scale };
            var wcells = rect.width / scale;
            var left = -wcells * .5f - 1f; var right = wcells * .5f + 1f;
            // soft clouds and hills in screen space
            sink.Disc(new Vector2(rect.x + rect.width * .2f, rect.y + rect.height * .16f), rect.height * .07f, new Color(1, 1, 1, .85f));
            sink.Disc(new Vector2(rect.x + rect.width * .2f + rect.height * .08f, rect.y + rect.height * .18f), rect.height * .06f, new Color(1, 1, 1, .85f));
            sink.Disc(new Vector2(rect.x + rect.width * .72f, rect.y + rect.height * .22f), rect.height * .06f, new Color(1, 1, 1, .8f));
            var horizon = view.Project(new Vector3(0, 0.15f, 0)).y;
            var hill = Color.Lerp(Ground[world], Color.white, .25f);
            for (var i = 0; i < 4; i++)
            {
                var cx = rect.x + rect.width * (0.1f + 0.28f * i + (world % 2) * .05f); var rx = rect.width * .26f; var ry = rect.height * (.12f + .03f * ((i + world) % 3));
                var pts = new List<Vector2>();
                for (var k = 0; k <= 14; k++) { var a = Mathf.PI * k / 14; pts.Add(new Vector2(cx - Mathf.Cos(a) * rx, horizon - Mathf.Sin(a) * ry)); }
                sink.Polygon(pts, Color.Lerp(hill, SkyBottom[world], .2f * (i % 2)));
            }
            sink.Gradient(Rect.MinMaxRect(rect.xMin, horizon, rect.xMax, rect.yMax), Ground[world], Color.Lerp(Ground[world], StpKit.H(0x4E9A4A), .45f));
            var scene = new StpScene(); var b = new StpBuilder();
            b.Outline = 0.03f;
            // ballast bed and rails across the full width
            const float railY = 2.15f;
            b.Frame = StpFrame.Identity;
            b.Box(new Vector3(0, railY, 0.03f), new Vector3(wcells * .5f + 1.2f, 0.32f, 0.03f), StpKit.H(0xBBAE95), 0, true);
            scene.Add(b.Take(), 0, 0);
            var tieCount = (int)((right - left) / 0.19f);
            for (var i = 0; i < tieCount; i++)
                b.Box(new Vector3(left + i * 0.19f, railY, 0.068f), new Vector3(0.04f, 0.235f, 0.018f), (i & 1) == 0 ? StpKit.H(0x8A5A3A) : StpKit.H(0x6E4528), 0, false);
            foreach (var off in new[] { -0.145f, 0.145f })
            {
                b.Box(new Vector3(0, railY + off, 0.083f), new Vector3(wcells * .5f + 1.2f, 0.036f, 0.012f), StpKit.H(0x4B4A55), 0, false);
                b.Box(new Vector3(0, railY + off, 0.098f), new Vector3(wcells * .5f + 1.2f, 0.022f, 0.014f), StpKit.H(0xD9DEE6), 0, false);
            }
            scene.Add(b.Take(), 0, 1);
            Scenery(scene, b, world, wcells);
            Modules(scene, b, composition, wcells);
            var propsX = -wcells * .5f + 0.8f; var pi = 0;
            foreach (var kind in propKinds)
            {
                StpKit.Module(b, kind, propsX + (pi % 5) * 1.25f, 2.95f - (pi / 5) * 0.45f, 0.9f);
                scene.Add(b.Take(), 1, 5 + pi * 0.01f); pi++;
            }
            // train, large and facing right, seen from a flatter side-on camera
            var baseTotal = StpKit.Length(vehicle.Type) + vehicle.Wagons.Length * (StpKit.WagonLength + 0.05f) + 0.05f;
            var tk = Mathf.Clamp((wcells - 1.4f) / baseTotal, 0.7f, 1.7f);
            var total = baseTotal * tk;
            var headX = Mathf.Min(wcells * .5f - 0.6f, -wcells * .5f + 0.5f + total);
            headX = Mathf.Max(headX, -wcells * .5f + total * .5f + 0.6f);
            var dx = headX - StpKit.Length(vehicle.Type) * .5f * tk;
            var lc = new List<Vector4>();
            lc.Add(new Vector4(dx, railY, 0, -1));
            var d = (StpKit.Length(vehicle.Type) * .5f + 0.05f) * tk;
            foreach (var kind in vehicle.Wagons) { d += StpKit.WagonLength * .5f * tk; lc.Add(new Vector4(dx - d, railY, 0, kind)); d += (StpKit.WagonLength * .5f + .05f) * tk; }
            for (var i = lc.Count - 1; i >= 0; i--)
            {
                var c = lc[i];
                b.Frame = new StpFrame(new Vector3(c.x, c.y, 0), 0, tk);
                b.Shadow(new Vector3(0.0f, 0.02f, 0.006f), 0.5f, 0.15f, 0.25f);
                if (c.w < 0) StpKit.Loco(b, vehicle.Type, vehicle.Body); else StpKit.Wagon(b, (int)c.w, vehicle.Body);
                scene.Add(b.Take(), 2, i * -0.001f);
            }
            scene.Emit(view, sink);
            if (composition != null)
                for (var i = 0; i < 12; i++)
                    sink.Disc(new Vector2(rect.x + rect.width * (0.5f + (i - 5.5f) * 0.06f), rect.yMax - Mathf.Max(5, rect.height * .035f)), Mathf.Max(2.2f, rect.height * .014f), i == clearedStage - 1 ? StpKit.H(0xE02A3F) : new Color(1, 1, 1, .75f));
        }

        private static void Scenery(StpScene scene, StpBuilder b, int world, float wcells)
        {
            var half = wcells * .5f;
            switch (world)
            {
                case 0: // Stammstrecke: city blocks and tunnel mouth
                    StpKit.House(b, -half + 0.9f, 0.9f, 1.1f, 0.7f, 1.0f, StpKit.H(0xF4E3C1), StpKit.H(0xC0392B)); scene.Add(b.Take(), 1, 0);
                    StpKit.House(b, -half + 2.3f, 0.7f, 0.9f, 0.7f, 1.35f, StpKit.H(0xD9E7F5), StpKit.H(0x5B6C8F)); scene.Add(b.Take(), 1, 0);
                    StpKit.Tunnel(b, half - 1.1f, 1.15f, 1.5f); scene.Add(b.Take(), 1, 0);
                    StpKit.Tree(b, -half + 3.6f, 1.2f, 1f, 0); scene.Add(b.Take(), 1, 0);
                    StpKit.Sign(b, half - 2.7f, 1.7f, 1f, 0); scene.Add(b.Take(), 1, 0);
                    break;
                case 1: // Baustelle: cones, barrier, shed
                    StpKit.Shed(b, -half + 1.2f, 0.85f, 1f); scene.Add(b.Take(), 1, 0);
                    StpKit.Barrier(b, half - 1.4f, 1.6f, 1.2f); scene.Add(b.Take(), 1, 0);
                    for (var i = 0; i < 5; i++) { StpKit.Cone(b, -half + 2.8f + i * 0.45f, 1.55f + (i % 2) * 0.05f); scene.Add(b.Take(), 1, 0); }
                    StpKit.StopSign(b, half - 2.7f, 3.0f, 1f); scene.Add(b.Take(), 1, 6);
                    break;
                case 2: // Bahnhof: platform, roofed hall, benches
                    StpKit.Platform(b, -half + 3.0f, 1.45f, 5.4f, 1f); scene.Add(b.Take(), 1, 0);
                    StpKit.House(b, half - 1.5f, 0.85f, 1.8f, 0.8f, 1.15f, StpKit.H(0xE5D4B0), StpKit.H(0x8F5B49)); scene.Add(b.Take(), 1, 0);
                    StpKit.Bench(b, -half + 2.2f, 1.62f, 1f); scene.Add(b.Take(), 1, 1);
                    StpKit.Display(b, -half + 3.8f, 1.6f, 1f); scene.Add(b.Take(), 1, 1);
                    StpKit.Signal(b, half - 0.8f, 1.7f, 1f, false); scene.Add(b.Take(), 1, 1);
                    break;
                case 3: // Viadukt: bridge, trees
                    StpKit.Bridge(b, -half + 2.2f, 1.1f, 1f); scene.Add(b.Take(), 1, 0);
                    StpKit.Tree(b, half - 1.2f, 0.9f, 1.2f, 1); scene.Add(b.Take(), 1, 0);
                    StpKit.Tree(b, half - 2.2f, 1.2f, 1f, 2); scene.Add(b.Take(), 1, 0);
                    StpKit.Tree(b, -half + 0.6f, 1.2f, 1.1f, 0); scene.Add(b.Take(), 1, 0);
                    StpKit.Signal(b, half - 3.2f, 1.7f, 1f, true); scene.Add(b.Take(), 1, 1);
                    break;
                default: // Betriebshof: depot shed, stop board, cones
                    StpKit.Shed(b, -half + 1.5f, 0.9f, 1.3f); scene.Add(b.Take(), 1, 0);
                    StpKit.Shed(b, half - 1.7f, 0.9f, 1.1f); scene.Add(b.Take(), 1, 0);
                    StpKit.Noticeboard(b, -half + 3.6f, 1.55f, 1f); scene.Add(b.Take(), 1, 1);
                    StpKit.Signal(b, half - 3.3f, 1.7f, 1f, false); scene.Add(b.Take(), 1, 1);
                    break;
            }
            for (var i = 0; i < 4; i++) { StpKit.Bush(b, -half + 0.5f + i * (wcells / 3.4f), 3.1f, 1f + (i % 2) * .2f); scene.Add(b.Take(), 1, 7); }
        }

        private static void Modules(StpScene scene, StpBuilder b, RailwayRouteComposition? composition, float wcells)
        {
            if (composition == null) return;
            foreach (var module in composition.modules)
            {
                var wx = (module.x / 400f - .5f) * (wcells - 1f);
                var wy = Mathf.Lerp(0.55f, 1.55f, Mathf.Clamp01(module.y / 240f));
                StpKit.Module(b, module.kind, wx, wy, Mathf.Clamp(module.scale * 0.9f, 0.5f, 1.4f));
                scene.Add(b.Take(), 1, 2);
            }
        }
    }

    /// <summary>Card renderer for the cosmetic catalogue: one vehicle, wagon or object on a small stage.</summary>
    internal static class StpShowcase
    {
        internal static void Paint(MeshGenerationContext context, Rect rect, string itemId)
            => StpPaint.Submit(context, Build(rect, itemId));

        internal static StpSink Build(Rect rect, string itemId)
        {
            var sink = new StpSink();
            if (rect.width < 4 || rect.height < 4) return sink;
            var vehicleCard = itemId.StartsWith("train-", StringComparison.Ordinal) || itemId.StartsWith("livery-", StringComparison.Ordinal) || itemId.StartsWith("wagon-", StringComparison.Ordinal);
            var world = (itemId.Length + itemId[itemId.Length - 1]) % 5;
            sink.Gradient(rect, StpDiorama.SkyTop[world], StpDiorama.SkyBottom[world]);
            var ground = Rect.MinMaxRect(rect.xMin, rect.y + rect.height * .66f, rect.xMax, rect.yMax);
            sink.Gradient(ground, StpDiorama.Ground[world], Color.Lerp(StpDiorama.Ground[world], StpKit.H(0x4E9A4A), .4f));
            var sx = StpView.Sx; var sy = StpView.Sy;
            if (vehicleCard) StpView.Use(0.16f, 0.68f); else StpView.Use(0.26f, 0.74f);
            try
            {
                var scene = new StpScene(); var track = new StpScene(); var b = new StpBuilder();
                b.Outline = 0.03f;
                if (vehicleCard)
                {
                    var vehicle = StpKit.Resolve(new List<string> { itemId });
                    for (var x = -3; x < 8; x++) { StpKit.TrackBallast(b, 1, x, 0); StpKit.TrackRails(b, 1, x, 0); track.Add(b.Take(), 1, 0, true); }
                    if (itemId.StartsWith("wagon-", StringComparison.Ordinal))
                    {
                        b.Frame = new StpFrame(new Vector3(2f, .5f, 0), 0, 1f);
                        b.Shadow(new Vector3(0, .02f, .006f), .5f, .15f, .25f);
                        StpKit.Wagon(b, vehicle.Wagons.Length > 0 ? vehicle.Wagons[0] : 0, StpKit.DefaultBody(0));
                    }
                    else
                    {
                        b.Frame = new StpFrame(new Vector3(2f, .5f, 0), 0, 1f);
                        b.Shadow(new Vector3(0, .02f, .006f), .5f, .15f, .25f);
                        StpKit.Loco(b, vehicle.Type, vehicle.Body);
                    }
                    scene.Add(b.Take(), 2, 0);
                }
                else
                {
                    StpKit.Module(b, ModuleFor(itemId), 0, 0, 1f);
                    scene.Add(b.Take(), 2, 0);
                }
                var view = scene.Fit(new Rect(rect.x, rect.y + rect.height * .06f, rect.width, rect.height * .86f), vehicleCard ? 0.1f : 0.12f);
                track.Emit(view, sink);
                scene.Emit(view, sink);
            }
            finally { StpView.Use(sx, sy); }
            return sink;
        }

        internal static string ModuleFor(string id)
        {
            switch (id)
            {
                case "obj-wartebank-ohne-anschluss": return "bench";
                case "obj-wegweiser-vielleicht": return "wayfinding";
                case "obj-absperrung-laufzeit-offen": return "barrier";
                case "obj-anzeiger-in-pruefung": return "indicator";
                case "obj-bahnsteig-provisorisch": return "platform";
                case "obj-tunnelportal-wartung": return "tunnel";
                case "obj-sev-bucht-geplant": return "sev-stop";
                case "obj-signal-freigegeben": return "signal";
                default: return "noticeboard";
            }
        }
    }
}
