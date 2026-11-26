using System;
using System.Collections.Generic;
using UnityEngine;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("STP.Presentation.UI")]

namespace STP.Presentation.World
{
    /// <summary>Local-to-world frame: translation, yaw (x towards y, clockwise on screen) and uniform scale.</summary>
    internal readonly struct StpFrame
    {
        internal readonly Vector3 Origin;
        internal readonly float Scale;
        private readonly float cos;
        private readonly float sin;
        internal StpFrame(Vector3 origin, float yawRadians, float scale)
        { Origin = origin; Scale = scale; cos = Mathf.Cos(yawRadians); sin = Mathf.Sin(yawRadians); }
        internal static StpFrame Identity => new StpFrame(Vector3.zero, 0, 1);
        internal Vector3 Point(Vector3 l) => new Vector3(Origin.x + (l.x * cos - l.y * sin) * Scale, Origin.y + (l.x * sin + l.y * cos) * Scale, Origin.z + l.z * Scale);
        internal Vector3 Dir(Vector3 d) => new Vector3(d.x * cos - d.y * sin, d.x * sin + d.y * cos, d.z);
    }

    /// <summary>One convex solid with smooth or flat normals and an optional inflated outline hull of identical topology.</summary>
    internal sealed class StpPart
    {
        internal Vector3[] P = Array.Empty<Vector3>();
        internal Vector3[] N = Array.Empty<Vector3>();
        internal Vector3[]? Hull;
        internal int[] T = Array.Empty<int>();
        internal Color Color;
        internal bool Unlit;
        internal Vector3 Center;
    }

    /// <summary>Oblique game camera: x' = x - Sx*z, y' = y - Sy*z, in board cells (y grows downwards, z up).</summary>
    internal struct StpView
    {
        internal static float Sx = 0.30f;
        internal static float Sy = 0.80f;
        internal static void Use(float sx, float sy) { Sx = sx; Sy = sy; }
        internal float Scale;
        internal float Ox;
        internal float Oy;
        internal Vector2 Project(Vector3 p) => new Vector2(Ox + (p.x - Sx * p.z) * Scale, Oy + (p.y - Sy * p.z) * Scale);
    }

    /// <summary>Procedural low-poly toolkit with bubbly super-ellipsoids, boxes, cylinders and prisms.</summary>
    internal sealed class StpBuilder
    {
        internal StpFrame Frame = StpFrame.Identity;
        /// <summary>Outline thickness in world units (after frame scaling).</summary>
        internal float Outline = 0.022f;
        internal static readonly Color Ink = new Color(0.14f, 0.10f, 0.22f, 1f);
        private readonly List<StpPart> parts = new List<StpPart>();

        internal List<StpPart> Take()
        { var result = new List<StpPart>(parts); parts.Clear(); return result; }

        /// <summary>True when a face with this local normal points towards the oblique camera.</summary>
        internal bool Solid;
        internal bool Faces(Vector3 localDirection)
        { var w = Frame.Dir(localDirection); return Solid || w.x * StpView.Sx + w.y * StpView.Sy + w.z > 0.12f; }

        private float OutlineLocal => Frame.Scale > 1e-5f ? Outline / Frame.Scale : Outline;

        private static float Sp(float value, float e)
        { return value < 0 ? -Mathf.Pow(-value, e) : Mathf.Pow(value, e); }

        private void Commit(Vector3[] p, Vector3[] n, Vector3[]? hull, List<int> tris, Color color, bool unlit)
        {
            var part = new StpPart { Color = color, Unlit = unlit };
            var wp = new Vector3[p.Length];
            var wn = new Vector3[p.Length];
            var center = Vector3.zero;
            for (var i = 0; i < p.Length; i++)
            {
                wp[i] = Frame.Point(p[i]);
                wn[i] = Frame.Dir(n[i]).normalized;
                center += wp[i];
            }
            part.Center = p.Length > 0 ? center / p.Length : center;
            if (hull != null)
            {
                var wh = new Vector3[hull.Length];
                for (var i = 0; i < hull.Length; i++) wh[i] = Frame.Point(hull[i]);
                part.Hull = wh;
            }
            var list = new List<int>(tris.Count);
            for (var i = 0; i + 2 < tris.Count; i += 3)
            {
                int a = tris[i], b = tris[i + 1], c = tris[i + 2];
                var g = Vector3.Cross(wp[b] - wp[a], wp[c] - wp[a]);
                if (g.sqrMagnitude < 1e-12f) continue;
                if (Vector3.Dot(g, wn[a] + wn[b] + wn[c]) < 0) { var t = b; b = c; c = t; }
                list.Add(a); list.Add(b); list.Add(c);
            }
            part.P = wp; part.N = wn; part.T = list.ToArray();
            parts.Add(part);
        }

        private static Vector3 Rot(Vector3 v, float cy, float sy) => new Vector3(v.x * cy - v.y * sy, v.x * sy + v.y * cy, v.z);

        /// <summary>Axis-aligned (before yaw) box with sharp edges and flat normals.</summary>
        internal void Box(Vector3 center, Vector3 half, Color color, float yaw = 0, bool outline = true, bool unlit = false)
        {
            var cy = Mathf.Cos(yaw); var sy = Mathf.Sin(yaw);
            var p = new List<Vector3>(); var n = new List<Vector3>(); var t = new List<int>();
            List<Vector3>? hull = outline ? new List<Vector3>() : null;
            var grow = outline ? OutlineLocal : 0;
            for (var axis = 0; axis < 3; axis++)
                for (var sign = -1; sign <= 1; sign += 2)
                {
                    var ua = (axis + 1) % 3; var va = (axis + 2) % 3;
                    var nrm = Vector3.zero; nrm[axis] = sign;
                    var baseIndex = p.Count;
                    for (var k = 0; k < 4; k++)
                    {
                        var su = (k & 1) == 0 ? -1f : 1f; var sv = (k & 2) == 0 ? -1f : 1f;
                        var local = Vector3.zero; local[axis] = sign * half[axis]; local[ua] = su * half[ua]; local[va] = sv * half[va];
                        p.Add(center + Rot(local, cy, sy)); n.Add(Rot(nrm, cy, sy));
                        if (hull != null)
                        {
                            var big = Vector3.zero; big[axis] = sign * (half[axis] + grow); big[ua] = su * (half[ua] + grow); big[va] = sv * (half[va] + grow);
                            hull.Add(center + Rot(big, cy, sy));
                        }
                    }
                    t.Add(baseIndex); t.Add(baseIndex + 1); t.Add(baseIndex + 2); t.Add(baseIndex + 1); t.Add(baseIndex + 3); t.Add(baseIndex + 2);
                }
            Commit(p.ToArray(), n.ToArray(), hull?.ToArray(), t, color, unlit);
        }

        /// <summary>Bubbly super-ellipsoid; exponent 1 is an ellipsoid, lower values approach a rounded box.</summary>
        internal void Ellipsoid(Vector3 center, Vector3 half, float exponent, Color color, float yaw = 0, bool outline = true, int nu = 14, int nv = 7, bool unlit = false)
        {
            var cy = Mathf.Cos(yaw); var sy = Mathf.Sin(yaw);
            var count = (nu + 1) * (nv + 1);
            var p = new Vector3[count]; var n = new Vector3[count];
            var hull = outline ? new Vector3[count] : null;
            var grow = outline ? OutlineLocal : 0;
            var e2 = 2f - exponent;
            for (var j = 0; j <= nv; j++)
            {
                var v = -Mathf.PI * .5f + Mathf.PI * j / nv; var cv = Mathf.Cos(v); var sv = Mathf.Sin(v);
                for (var i = 0; i <= nu; i++)
                {
                    var u = 2 * Mathf.PI * i / nu; var cu = Mathf.Cos(u); var su = Mathf.Sin(u);
                    var k = j * (nu + 1) + i;
                    var fx = Sp(cv, exponent) * Sp(cu, exponent); var fy = Sp(cv, exponent) * Sp(su, exponent); var fz = Sp(sv, exponent);
                    p[k] = center + Rot(new Vector3(half.x * fx, half.y * fy, half.z * fz), cy, sy);
                    if (hull != null) hull[k] = center + Rot(new Vector3((half.x + grow) * fx, (half.y + grow) * fy, (half.z + grow) * fz), cy, sy);
                    var nn = new Vector3(Sp(cv, e2) * Sp(cu, e2) / Mathf.Max(half.x, 1e-4f), Sp(cv, e2) * Sp(su, e2) / Mathf.Max(half.y, 1e-4f), Sp(sv, e2) / Mathf.Max(half.z, 1e-4f));
                    if (nn.sqrMagnitude < 1e-12f) nn = Vector3.up;
                    n[k] = Rot(nn.normalized, cy, sy);
                }
            }
            var t = new List<int>();
            for (var j = 0; j < nv; j++)
                for (var i = 0; i < nu; i++)
                {
                    var a = j * (nu + 1) + i; var b = a + 1; var c = a + nu + 1; var d = c + 1;
                    t.Add(a); t.Add(b); t.Add(c); t.Add(b); t.Add(d); t.Add(c);
                }
            Commit(p, n, hull, t, color, unlit);
        }

        /// <summary>Cylinder or cone frustum along a local axis (0=x, 1=y, 2=z); radiusEnd applies at the positive end.</summary>
        internal void Cylinder(Vector3 center, float radius, float halfLength, int axis, Color color, float radiusEnd = -1, bool outline = true, int segments = 12)
        {
            if (radiusEnd < 0) radiusEnd = radius;
            var pa = (axis + 1) % 3; var qa = (axis + 2) % 3;
            var grow = outline ? OutlineLocal : 0;
            var p = new List<Vector3>(); var n = new List<Vector3>(); var t = new List<int>();
            List<Vector3>? hull = outline ? new List<Vector3>() : null;
            Vector3 Make(float along, float r, float ang)
            { var v = Vector3.zero; v[axis] = along; v[pa] = Mathf.Cos(ang) * r; v[qa] = Mathf.Sin(ang) * r; return center + v; }
            for (var ring = 0; ring < 2; ring++)
            {
                var along = ring == 0 ? -halfLength : halfLength; var r = ring == 0 ? radius : radiusEnd;
                for (var i = 0; i <= segments; i++)
                {
                    var ang = 2 * Mathf.PI * i / segments;
                    p.Add(Make(along, r, ang));
                    var radial = Vector3.zero; radial[pa] = Mathf.Cos(ang); radial[qa] = Mathf.Sin(ang);
                    var tilt = Vector3.zero; tilt[axis] = radius - radiusEnd;
                    n.Add((radial * (2 * halfLength) + tilt).normalized);
                    hull?.Add(Make(along + (ring == 0 ? -grow : grow), Mathf.Max(0, r + grow), ang));
                }
            }
            for (var i = 0; i < segments; i++)
            { var a = i; var b = i + 1; var c = i + segments + 1; var d = c + 1; t.Add(a); t.Add(b); t.Add(c); t.Add(b); t.Add(d); t.Add(c); }
            for (var cap = 0; cap < 2; cap++)
            {
                var r = cap == 0 ? radius : radiusEnd; if (r < 0.004f) continue;
                var along = cap == 0 ? -halfLength : halfLength; var sign = cap == 0 ? -1f : 1f;
                var normal = Vector3.zero; normal[axis] = sign;
                var centerIndex = p.Count; p.Add(Make(along, 0, 0)); n.Add(normal); hull?.Add(Make(along + sign * grow, 0, 0));
                for (var i = 0; i <= segments; i++)
                { var ang = 2 * Mathf.PI * i / segments; p.Add(Make(along, r, ang)); n.Add(normal); hull?.Add(Make(along + sign * grow, r + grow, ang)); }
                for (var i = 0; i < segments; i++) { t.Add(centerIndex); t.Add(centerIndex + 1 + i); t.Add(centerIndex + 2 + i); }
            }
            Commit(p.ToArray(), n.ToArray(), hull?.ToArray(), t, color, false);
        }

        /// <summary>Triangular prism along local x (gabled roof): base half-width hy at z=0, ridge at z=height.</summary>
        internal void Prism(Vector3 center, float halfLength, float halfWidth, float height, Color color, float yaw = 0, bool outline = true)
        {
            var cy = Mathf.Cos(yaw); var sy = Mathf.Sin(yaw);
            var grow = outline ? OutlineLocal : 0;
            var p = new List<Vector3>(); var n = new List<Vector3>(); var t = new List<int>();
            List<Vector3>? hull = outline ? new List<Vector3>() : null;
            var slope = new Vector3(0, height, halfWidth).normalized;
            void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 nrm, Vector3 ha, Vector3 hb, Vector3 hc, Vector3 hd)
            {
                var i = p.Count; p.Add(center + Rot(a, cy, sy)); p.Add(center + Rot(b, cy, sy)); p.Add(center + Rot(c, cy, sy)); p.Add(center + Rot(d, cy, sy));
                for (var k = 0; k < 4; k++) n.Add(Rot(nrm, cy, sy));
                if (hull != null) { hull.Add(center + Rot(ha, cy, sy)); hull.Add(center + Rot(hb, cy, sy)); hull.Add(center + Rot(hc, cy, sy)); hull.Add(center + Rot(hd, cy, sy)); }
                t.Add(i); t.Add(i + 1); t.Add(i + 2); t.Add(i + 1); t.Add(i + 3); t.Add(i + 2);
            }
            float hx = halfLength, hw = halfWidth, hz = height, g = grow;
            Quad(new Vector3(-hx, -hw, 0), new Vector3(hx, -hw, 0), new Vector3(-hx, 0, hz), new Vector3(hx, 0, hz), new Vector3(0, -slope.z, slope.y).normalized,
                new Vector3(-hx - g, -hw - g, -g), new Vector3(hx + g, -hw - g, -g), new Vector3(-hx - g, 0, hz + g), new Vector3(hx + g, 0, hz + g));
            Quad(new Vector3(-hx, 0, hz), new Vector3(hx, 0, hz), new Vector3(-hx, hw, 0), new Vector3(hx, hw, 0), new Vector3(0, slope.z, slope.y).normalized,
                new Vector3(-hx - g, 0, hz + g), new Vector3(hx + g, 0, hz + g), new Vector3(-hx - g, hw + g, -g), new Vector3(hx + g, hw + g, -g));
            Quad(new Vector3(-hx, -hw, 0), new Vector3(-hx, hw, 0), new Vector3(hx, -hw, 0), new Vector3(hx, hw, 0), Vector3.back,
                new Vector3(-hx - g, -hw - g, -g), new Vector3(-hx - g, hw + g, -g), new Vector3(hx + g, -hw - g, -g), new Vector3(hx + g, hw + g, -g));
            foreach (var side in new[] { -1f, 1f })
            {
                var i = p.Count; var nrm = new Vector3(side, 0, 0);
                p.Add(center + Rot(new Vector3(side * hx, -hw, 0), cy, sy)); p.Add(center + Rot(new Vector3(side * hx, hw, 0), cy, sy)); p.Add(center + Rot(new Vector3(side * hx, 0, hz), cy, sy));
                for (var k = 0; k < 3; k++) n.Add(Rot(nrm, cy, sy));
                if (hull != null)
                {
                    hull.Add(center + Rot(new Vector3(side * (hx + g), -hw - g, -g), cy, sy)); hull.Add(center + Rot(new Vector3(side * (hx + g), hw + g, -g), cy, sy)); hull.Add(center + Rot(new Vector3(side * (hx + g), 0, hz + g), cy, sy));
                }
                t.Add(i); t.Add(i + 1); t.Add(i + 2);
            }
            Commit(p.ToArray(), n.ToArray(), hull?.ToArray(), t, color, false);
        }

        /// <summary>Flat translucent contact shadow disc, drawn unlit without outline.</summary>
        internal void Shadow(Vector3 center, float rx, float ry, float alpha, float yaw = 0)
        {
            var cy = Mathf.Cos(yaw); var sy = Mathf.Sin(yaw);
            var p = new List<Vector3>(); var n = new List<Vector3>(); var t = new List<int>();
            p.Add(center); n.Add(Vector3.forward);
            const int segments = 18;
            for (var i = 0; i <= segments; i++)
            { var a = 2 * Mathf.PI * i / segments; p.Add(center + Rot(new Vector3(Mathf.Cos(a) * rx, Mathf.Sin(a) * ry, 0), cy, sy)); n.Add(Vector3.forward); }
            for (var i = 0; i < segments; i++) { t.Add(0); t.Add(1 + i); t.Add(2 + i); }
            Commit(p.ToArray(), n.ToArray(), null, t, new Color(0.08f, 0.06f, 0.16f, alpha), true);
        }
    }

    /// <summary>Output vertex batches below the UI mesh limit; also consumed by the offline rasterizer test.</summary>
    internal sealed class StpBatch
    {
        internal readonly List<Vector2> Pos = new List<Vector2>();
        internal readonly List<Color32> Col = new List<Color32>();
        internal readonly List<int> Idx = new List<int>();
    }

    internal sealed class StpSink
    {
        private const int Limit = 60000;
        internal readonly List<StpBatch> Batches = new List<StpBatch>();
        private StpBatch current = new StpBatch();
        internal StpSink() { Batches.Add(current); }
        internal void Reserve(int vertices)
        { if (current.Pos.Count + vertices > Limit && current.Pos.Count > 0) { current = new StpBatch(); Batches.Add(current); } }
        internal int Vertex(Vector2 position, Color32 color) { current.Pos.Add(position); current.Col.Add(color); return current.Pos.Count - 1; }
        internal void Triangle(int a, int b, int c) { current.Idx.Add(a); current.Idx.Add(b); current.Idx.Add(c); }
        /// <summary>Screen-space vertical gradient rectangle (clockwise).</summary>
        internal void Gradient(Rect r, Color top, Color bottom)
        {
            Reserve(4);
            var a = Vertex(new Vector2(r.xMin, r.yMin), top); var b = Vertex(new Vector2(r.xMax, r.yMin), top);
            var c = Vertex(new Vector2(r.xMin, r.yMax), bottom); var d = Vertex(new Vector2(r.xMax, r.yMax), bottom);
            Triangle(a, b, c); Triangle(b, d, c);
        }
        /// <summary>Screen-space convex polygon fan (any winding).</summary>
        internal void Polygon(IList<Vector2> points, Color color)
        {
            if (points.Count < 3) return;
            Reserve(points.Count);
            var first = -1; var cross = 0f;
            for (var i = 1; i + 1 < points.Count; i++) cross += (points[i].x - points[0].x) * (points[i + 1].y - points[0].y) - (points[i].y - points[0].y) * (points[i + 1].x - points[0].x);
            var ids = new int[points.Count];
            for (var i = 0; i < points.Count; i++) { ids[i] = Vertex(points[i], color); if (first < 0) first = ids[i]; }
            for (var i = 1; i + 1 < points.Count; i++) { if (cross >= 0) Triangle(ids[0], ids[i], ids[i + 1]); else Triangle(ids[0], ids[i + 1], ids[i]); }
        }
        internal void Disc(Vector2 center, float radius, Color color, int segments = 20)
        {
            var pts = new Vector2[segments];
            for (var i = 0; i < segments; i++) { var a = 2 * Mathf.PI * i / segments; pts[i] = center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius; }
            Polygon(pts, color);
        }
    }

    /// <summary>Depth-sorted scene of part groups (painter's algorithm over convex parts).</summary>
    internal sealed class StpScene
    {
        private sealed class Group { internal List<StpPart> Parts = new List<StpPart>(); internal int Layer; internal float Key; internal bool HullsFirst; }
        private readonly List<Group> groups = new List<Group>();
        private int[] map = new int[256];
        private static readonly Vector3 Light = new Vector3(-0.35f, 0.45f, 0.82f).normalized;

        internal void Clear() => groups.Clear();

        /// <summary>Adds parts that must keep their relative order (a vehicle, a prop). Layers separate ground from objects.</summary>
        internal void Add(List<StpPart> parts, int layer = 1, float keyBias = 0, bool hullsFirst = false)
        {
            if (parts.Count == 0) return;
            var center = Vector3.zero;
            foreach (var part in parts) center += part.Center;
            center /= parts.Count;
            groups.Add(new Group { Parts = parts, Layer = layer, Key = StpView.Sx * center.x + StpView.Sy * center.y + center.z * 0.25f + keyBias, HullsFirst = hullsFirst });
        }

        /// <summary>Projected bounds at unit scale, including outlines.</summary>
        internal Rect Bounds()
        {
            var min = new Vector2(float.MaxValue, float.MaxValue); var max = new Vector2(float.MinValue, float.MinValue);
            var view = new StpView { Scale = 1 };
            foreach (var g in groups)
                foreach (var part in g.Parts)
                {
                    var src = part.Hull ?? part.P;
                    foreach (var p in src) { var s = view.Project(p); min = Vector2.Min(min, s); max = Vector2.Max(max, s); }
                }
            if (min.x > max.x) return new Rect(0, 0, 1, 1);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        /// <summary>Fits the scene into a rectangle, preserving aspect and centering it.</summary>
        internal StpView Fit(Rect target, float margin = 0.04f)
        {
            var b = Bounds();
            var scale = Mathf.Min(target.width * (1 - 2 * margin) / Mathf.Max(b.width, 1e-4f), target.height * (1 - 2 * margin) / Mathf.Max(b.height, 1e-4f));
            return new StpView { Scale = scale, Ox = target.center.x - b.center.x * scale, Oy = target.center.y - b.center.y * scale };
        }

        internal static Color Shade(Color baseColor, Vector3 normal)
        {
            var ndl = Vector3.Dot(normal, Light);
            var t = Mathf.Clamp01((ndl + 0.25f) / 1.0f); t = t * t * (3 - 2 * t);
            var r = baseColor.r * Mathf.Lerp(0.56f, 1.10f, t);
            var g = baseColor.g * Mathf.Lerp(0.62f, 1.05f, t);
            var b = baseColor.b * Mathf.Lerp(0.84f, 0.95f, t);
            return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), baseColor.a);
        }

        internal void Emit(StpView view, StpSink sink)
        {
            groups.Sort((a, b) => a.Layer != b.Layer ? a.Layer.CompareTo(b.Layer) : a.Key.CompareTo(b.Key));
            var ink = (Color32)StpBuilder.Ink;
            foreach (var g in groups)
            {
                if (g.HullsFirst)
                {
                    foreach (var part in g.Parts) if (part.Hull != null && part.T.Length > 0) EmitOne(view, sink, part, ink, true);
                    foreach (var part in g.Parts) if (part.T.Length > 0) EmitOne(view, sink, part, ink, false);
                }
                else
                    foreach (var part in g.Parts)
                    {
                        if (part.T.Length == 0) continue;
                        if (part.Hull != null) EmitOne(view, sink, part, ink, true);
                        EmitOne(view, sink, part, ink, false);
                    }
            }
        }

        private void EmitOne(StpView view, StpSink sink, StpPart part, Color32 ink, bool hull)
        {
            var count = part.P.Length;
            if (map.Length < count) map = new int[count * 2];
            sink.Reserve(count);
            if (hull) EmitMesh(view, sink, part.Hull!, null, part, ink, true);
            else EmitMesh(view, sink, part.P, part.N, part, default, false);
        }

        private void EmitMesh(StpView view, StpSink sink, Vector3[] positions, Vector3[]? normals, StpPart part, Color32 flat, bool hull)
        {
            var count = positions.Length;
            for (var i = 0; i < count; i++) map[i] = -1;
            var projected = new Vector2[count];
            for (var i = 0; i < count; i++) projected[i] = view.Project(positions[i]);
            var t = part.T;
            for (var i = 0; i + 2 < t.Length; i += 3)
            {
                int a = t[i], b = t[i + 1], c = t[i + 2];
                var cross = (projected[b].x - projected[a].x) * (projected[c].y - projected[a].y) - (projected[b].y - projected[a].y) * (projected[c].x - projected[a].x);
                if (cross <= 0.0005f) continue;
                var ids = new[] { a, b, c };
                for (var k = 0; k < 3; k++)
                {
                    var v = ids[k];
                    if (map[v] < 0)
                    {
                        Color32 color = hull ? flat : (Color32)(part.Unlit ? part.Color : Shade(part.Color, normals![v]));
                        map[v] = sink.Vertex(projected[v], color);
                    }
                }
                sink.Triangle(map[a], map[b], map[c]);
            }
        }
    }
}
