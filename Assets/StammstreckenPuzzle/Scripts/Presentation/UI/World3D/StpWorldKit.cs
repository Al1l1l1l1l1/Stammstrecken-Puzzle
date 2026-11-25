using System;
using System.Collections.Generic;
using UnityEngine;

namespace STP.Presentation.UI
{
    /// <summary>Resolved vehicle: locomotive type, paint, and up to three wagons.</summary>
    internal readonly struct StpVehicle
    {
        internal readonly int Type;
        internal readonly Color Body;
        internal readonly int[] Wagons;
        internal StpVehicle(int type, Color body, int[] wagons) { Type = type; Body = body; Wagons = wagons; }
    }

    /// <summary>Original bubbly railway construction kit. Local frame: x forward, y lateral, z up, unit = one board cell.</summary>
    internal static class StpKit
    {
        internal const float Rail = 0.09f;
        internal static Color H(int rgb, float a = 1f) => new Color(((rgb >> 16) & 255) / 255f, ((rgb >> 8) & 255) / 255f, (rgb & 255) / 255f, a);
        internal static Color Mix(Color a, Color b, float t) => Color.Lerp(a, b, t);
        internal static Color Dark(Color c, float f) => new Color(c.r * f, c.g * f, c.b * f, c.a);

        private static readonly Color Glass = H(0x2A3B5C);
        private static readonly Color Metal = H(0xD9DEE6);
        private static readonly Color Wheel = H(0x2B2A33);
        private static readonly Color Light = H(0xFFF2B0);

        // ---------------------------------------------------------------- vehicles

        internal static readonly int[] TypeCount = { 7 };

        internal static Color DefaultBody(int type)
        {
            switch (type)
            {
                case 1: return H(0xB9C3C9);
                case 2: return H(0xF2F2EE);
                case 3: return H(0xEAF0F8);
                case 4: return H(0xF0E2B6);
                case 5: return H(0x2F7A57);
                case 6: return H(0xF28A24);
                default: return H(0xD6213A);
            }
        }

        internal static Color Accent(int type)
        {
            switch (type)
            {
                case 1: return H(0xE9A23B);
                case 2: return H(0xD6213A);
                case 3: return H(0x1D62C0);
                case 4: return H(0xD6213A);
                case 5: return H(0xA4252F);
                case 6: return H(0x2B2A33);
                default: return H(0xF2F2EE);
            }
        }

        internal static float Length(int type)
        {
            switch (type)
            {
                case 1: return 0.86f; case 2: return 1.08f; case 3: return 0.9f; case 4: return 0.86f; case 5: return 1.12f; case 6: return 0.62f;
                default: return 0.96f;
            }
        }

        internal const float WagonLength = 0.8f;

        /// <summary>Resolves the selected canonical items into locomotive, paint and wagons. Unknown IDs are ignored here.</summary>
        internal static StpVehicle Resolve(IReadOnlyList<string> ids)
        {
            var type = 0; int? paint = null; var wagons = new List<int>();
            foreach (var id in ids)
            {
                switch (id)
                {
                    case "train-ersatzgarnitur": type = 1; break;
                    case "train-anschlusswunder": type = 2; break;
                    case "train-u-bahn-muenchen": type = 3; break;
                    case "train-doppelstock-pendler": type = 4; break;
                    case "train-dampflok-sonderfahrt": type = 5; break;
                    case "train-rangierlok-notbetrieb": type = 6; break;
                    case "livery-betriebsrot": paint = 0x7C1F31; break;
                    case "livery-baustellengelb": paint = 0xF0C21A; break;
                    case "livery-pendlerblau": paint = 0x2F72D6; break;
                    case "livery-ersatzgruen": paint = 0x32A85A; break;
                    case "livery-umleitungsorange": paint = 0xF47B1F; break;
                    case "livery-sonderviolett": paint = 0x7C4DCB; break;
                    case "livery-bummlertuerkis": paint = 0x19B7B3; break;
                    case "livery-nachtanthrazit": paint = 0x3B424C; break;
                    case "livery-kulanzrosa": paint = 0xF48DBA; break;
                    case "livery-frischweiss": paint = 0xF6F6F2; break;
                    case "wagon-grossraum-sitzplatz-optional": wagons.Add(0); break;
                    case "wagon-bordbistro-kaffeemaschine-defekt": wagons.Add(1); break;
                    case "wagon-fahrradabteil-ueberfuellt": wagons.Add(2); break;
                }
            }
            wagons.Sort();
            return new StpVehicle(type, paint.HasValue ? H(paint.Value) : DefaultBody(type), wagons.ToArray());
        }

        private static void Wheels(StpBuilder b, float x, float half, float radius, bool sideP, bool sideN)
        {
            if (sideP) b.Cylinder(new Vector3(x, half, Rail + radius), radius, 0.016f, 1, Wheel, -1, true, 10);
            if (sideN) b.Cylinder(new Vector3(x, -half, Rail + radius), radius, 0.016f, 1, Wheel, -1, true, 10);
        }

        private static void Bogie(StpBuilder b, float x, bool sideP, bool sideN)
        {
            b.Box(new Vector3(x, 0, Rail + 0.035f), new Vector3(0.11f, 0.11f, 0.03f), Wheel, 0, false);
            Wheels(b, x - 0.055f, 0.125f, 0.05f, sideP, sideN);
            Wheels(b, x + 0.055f, 0.125f, 0.05f, sideP, sideN);
        }

        /// <summary>Rounded passenger body with window band and head/tail details, centered on the local origin.</summary>
        private static void Coach(StpBuilder b, float len, float halfW, float height, float round, Color body, Color accent, int rows, bool nose, bool tail)
        {
            var sideP = b.Faces(Vector3.up); var sideN = b.Faces(Vector3.down);
            var fwd = b.Faces(Vector3.right); var back = b.Faces(Vector3.left);
            var zb = Rail + 0.06f;
            Bogie(b, len * 0.3f, sideP, sideN); Bogie(b, -len * 0.3f, sideP, sideN);
            b.Ellipsoid(new Vector3(0, 0, zb + height * 0.5f), new Vector3(len * 0.5f, halfW, height * 0.5f), round, body, 0, true, 20, 10);
            foreach (var s in new[] { 1f, -1f })
            {
                if (s > 0 && !sideP) continue; if (s < 0 && !sideN) continue;
                var yy = s * (halfW * 0.985f);
                // belt stripe
                b.Box(new Vector3(0, yy, zb + height * 0.30f), new Vector3(len * 0.455f, 0.006f, 0.012f), accent, 0, false);
                for (var r = 0; r < rows; r++)
                {
                    var z = zb + height * (rows == 1 ? 0.62f : (r == 0 ? 0.80f : 0.50f));
                    var h = rows == 1 ? height * 0.13f : height * 0.085f;
                    for (var k = 0; k < 5; k++)
                        b.Box(new Vector3(-len * 0.34f + k * len * 0.17f, yy, z), new Vector3(len * 0.062f, 0.007f, h), Glass, 0, false);
                }
                b.Box(new Vector3(len * 0.04f, yy, zb + height * 0.40f), new Vector3(0.03f, 0.008f, height * 0.34f), Dark(body, 0.72f), 0, false);
            }
            if (nose && fwd) EndFace(b, len * 0.5f, halfW, zb, height, body, 1);
            if (nose && back) EndFace(b, -len * 0.5f, halfW, zb, height, body, -1);
            if (tail && fwd) EndFace(b, len * 0.5f, halfW, zb, height, body, 1);
            if (tail && back) EndFace(b, -len * 0.5f, halfW, zb, height, body, -1);
        }

        private static void EndFace(StpBuilder b, float x, float halfW, float zb, float height, Color body, int dir)
        {
            var xx = x - dir * 0.012f;
            b.Box(new Vector3(xx, 0, zb + height * 0.62f), new Vector3(0.012f, halfW * 0.72f, height * 0.22f), Glass, 0, false);
            b.Box(new Vector3(xx, -halfW * 0.6f, zb + height * 0.22f), new Vector3(0.012f, 0.025f, 0.025f), Light, 0, false);
            b.Box(new Vector3(xx, halfW * 0.6f, zb + height * 0.22f), new Vector3(0.012f, 0.025f, 0.025f), Light, 0, false);
            b.Box(new Vector3(xx, 0, zb + height * 0.20f), new Vector3(0.010f, halfW * 0.55f, 0.012f), Dark(body, 0.7f), 0, false);
        }

        private static void Roof(StpBuilder b, float x, float halfLen, float z, Color color)
        { b.Box(new Vector3(x, 0, z), new Vector3(halfLen, 0.075f, 0.022f), color, 0, true); }

        /// <summary>Builds the locomotive/head car around the local origin.</summary>
        internal static void Loco(StpBuilder b, int type, Color body)
        {
            var accent = Accent(type);
            var sideP = b.Faces(Vector3.up); var sideN = b.Faces(Vector3.down);
            var len = Length(type);
            switch (type)
            {
                case 1: // Ersatzgarnitur: boxy, unmatched door, rusty patch
                    Coach(b, len, 0.165f, 0.31f, 0.16f, body, accent, 1, true, true);
                    foreach (var s in new[] { 1f, -1f })
                    {
                        if (s > 0 && !sideP) continue; if (s < 0 && !sideN) continue;
                        b.Box(new Vector3(len * 0.18f, s * 0.162f, Rail + 0.22f), new Vector3(0.05f, 0.008f, 0.12f), accent, 0, false);
                        b.Box(new Vector3(-len * 0.30f, s * 0.162f, Rail + 0.16f), new Vector3(0.07f, 0.008f, 0.05f), H(0x9A6B4A), 0, false);
                    }
                    Roof(b, -0.1f, 0.13f, Rail + 0.385f, H(0x8E979C));
                    break;
                case 2: // Anschlusswunder: long pointed express
                    Coach(b, len, 0.150f, 0.30f, 0.62f, body, accent, 1, true, true);
                    b.Ellipsoid(new Vector3(len * 0.5f - 0.02f, 0, Rail + 0.20f), new Vector3(0.20f, 0.135f, 0.14f), 0.7f, body, 0, true, 14, 8);
                    b.Ellipsoid(new Vector3(-len * 0.5f + 0.02f, 0, Rail + 0.20f), new Vector3(0.14f, 0.13f, 0.13f), 0.7f, body, 0, true, 14, 8);
                    b.Box(new Vector3(len * 0.5f + 0.1f, 0, Rail + 0.25f), new Vector3(0.012f, 0.085f, 0.04f), Glass, 0, false);
                    break;
                case 3: // U-Bahn Muenchen: flat front, blue band, roof pods
                    Coach(b, len, 0.17f, 0.32f, 0.12f, body, accent, 1, true, true);
                    b.Box(new Vector3(0, 0, Rail + 0.385f), new Vector3(len * 0.42f, 0.1f, 0.02f), H(0xB7C1CD), 0, true);
                    foreach (var s in new[] { 1f, -1f })
                    {
                        if (s > 0 && !sideP) continue; if (s < 0 && !sideN) continue;
                        b.Box(new Vector3(0, s * 0.172f, Rail + 0.13f), new Vector3(len * 0.46f, 0.006f, 0.035f), accent, 0, false);
                    }
                    break;
                case 4: // Doppelstock-Pendler: tall, two window rows
                    Coach(b, len, 0.17f, 0.46f, 0.22f, body, accent, 2, true, true);
                    Roof(b, 0.0f, 0.2f, Rail + 0.555f, H(0x9FA7AE));
                    break;
                case 5: SteamLoco(b, len, body, accent); break;
                case 6: Shunter(b, len, body, accent); break;
                default:
                    Coach(b, len, 0.16f, 0.32f, 0.34f, body, accent, 1, true, true);
                    Roof(b, -0.12f, 0.15f, Rail + 0.405f, H(0xA9B1B8));
                    Roof(b, 0.2f, 0.07f, Rail + 0.405f, H(0xA9B1B8));
                    break;
            }
        }

        private static void SteamLoco(StpBuilder b, float len, Color body, Color accent)
        {
            var sideP = b.Faces(Vector3.up); var sideN = b.Faces(Vector3.down);
            var black = H(0x2A2B33);
            b.Box(new Vector3(0, 0, Rail + 0.09f), new Vector3(len * 0.5f, 0.12f, 0.03f), black, 0, true);
            b.Cylinder(new Vector3(0.14f, 0, Rail + 0.23f), 0.12f, 0.24f, 0, black, -1, true, 14);
            b.Cylinder(new Vector3(0.40f, 0, Rail + 0.23f), 0.125f, 0.025f, 0, H(0x555867), -1, true, 14);
            b.Cylinder(new Vector3(0.33f, 0, Rail + 0.40f), 0.045f, 0.075f, 2, black, 0.075f, true, 10);
            b.Cylinder(new Vector3(0.10f, 0, Rail + 0.375f), 0.05f, 0.03f, 2, H(0xC7A24B), -1, true, 10);
            b.Box(new Vector3(-0.28f, 0, Rail + 0.28f), new Vector3(0.17f, 0.15f, 0.20f), body, 0, true);
            b.Box(new Vector3(-0.28f, 0, Rail + 0.485f), new Vector3(0.20f, 0.17f, 0.018f), accent, 0, true);
            b.Box(new Vector3(-0.50f, 0, Rail + 0.18f), new Vector3(0.14f, 0.13f, 0.09f), Dark(body, 0.85f), 0, true);
            b.Box(new Vector3(-0.58f, 0, Rail + 0.27f), new Vector3(0.10f, 0.115f, 0.015f), black, 0, true);
            foreach (var s in new[] { 1f, -1f })
            {
                if (s > 0 && !sideP) continue; if (s < 0 && !sideN) continue;
                b.Box(new Vector3(-0.28f, s * 0.152f, Rail + 0.34f), new Vector3(0.07f, 0.008f, 0.07f), Glass, 0, false);
                foreach (var wx in new[] { -0.30f, -0.04f, 0.22f })
                    b.Cylinder(new Vector3(wx, s * 0.14f, Rail + 0.075f), 0.075f, 0.018f, 1, accent, -1, true, 12);
                b.Box(new Vector3(-0.04f, s * 0.162f, Rail + 0.075f), new Vector3(0.27f, 0.007f, 0.012f), Metal, 0, false);
            }
            b.Box(new Vector3(0.50f, 0, Rail + 0.06f), new Vector3(0.03f, 0.13f, 0.04f), accent, 0, true);
            if (len > 0) b.Box(new Vector3(0.43f, 0, Rail + 0.34f), new Vector3(0.012f, 0.03f, 0.03f), Light, 0, false);
        }

        private static void Shunter(StpBuilder b, float len, Color body, Color accent)
        {
            var sideP = b.Faces(Vector3.up); var sideN = b.Faces(Vector3.down);
            Bogie(b, 0.18f, sideP, sideN); Bogie(b, -0.18f, sideP, sideN);
            b.Ellipsoid(new Vector3(0.12f, 0, Rail + 0.17f), new Vector3(0.19f, 0.16f, 0.11f), 0.28f, body, 0, true, 16, 8);
            b.Ellipsoid(new Vector3(-0.1f, 0, Rail + 0.26f), new Vector3(0.17f, 0.16f, 0.2f), 0.26f, body, 0, true, 16, 8);
            b.Box(new Vector3(-0.1f, 0, Rail + 0.46f), new Vector3(0.19f, 0.17f, 0.014f), accent, 0, true);
            foreach (var s in new[] { 1f, -1f })
            {
                if (s > 0 && !sideP) continue; if (s < 0 && !sideN) continue;
                b.Box(new Vector3(-0.1f, s * 0.158f, Rail + 0.30f), new Vector3(0.09f, 0.007f, 0.07f), Glass, 0, false);
                for (var k = 0; k < 4; k++)
                    b.Box(new Vector3(-0.2f + k * 0.12f, s * 0.145f, Rail + 0.11f), new Vector3(0.04f, 0.007f, 0.02f), k % 2 == 0 ? accent : H(0xFFE16B), 0, false);
            }
            b.Cylinder(new Vector3(0.22f, 0, Rail + 0.34f), 0.025f, 0.05f, 2, accent, -1, true, 8);
            b.Box(new Vector3(0.31f, 0, Rail + 0.17f), new Vector3(0.012f, 0.12f, 0.05f), Dark(body, 0.8f), 0, false);
        }

        /// <summary>Builds one wagon (0 Grossraum, 1 Bordbistro, 2 Fahrradabteil) around the local origin.</summary>
        internal static void Wagon(StpBuilder b, int kind, Color body)
        {
            var sideP = b.Faces(Vector3.up); var sideN = b.Faces(Vector3.down);
            var len = WagonLength;
            var accent = kind == 1 ? H(0x8A5A3C) : kind == 2 ? H(0x3CB371) : H(0xE9EDF2);
            Coach(b, len, 0.16f, 0.32f, 0.3f, body, accent, 1, false, false);
            Roof(b, 0, 0.2f, Rail + 0.405f, H(0xA9B1B8));
            foreach (var s in new[] { 1f, -1f })
            {
                if (s > 0 && !sideP) continue; if (s < 0 && !sideN) continue;
                var yy = s * 0.163f;
                if (kind == 1)
                {
                    b.Box(new Vector3(0.0f, yy + s * 0.03f, Rail + 0.27f), new Vector3(0.20f, 0.03f, 0.016f), accent, 0, true);
                    b.Box(new Vector3(0.0f, yy, Rail + 0.17f), new Vector3(0.20f, 0.008f, 0.04f), H(0xF2E6C9), 0, false);
                }
                else if (kind == 2)
                {
                    foreach (var bx in new[] { -0.16f, 0.16f })
                    {
                        b.Cylinder(new Vector3(bx - 0.045f, yy + s * 0.012f, Rail + 0.21f), 0.04f, 0.008f, 1, H(0x20232B), 0.04f, true, 10);
                        b.Cylinder(new Vector3(bx + 0.045f, yy + s * 0.012f, Rail + 0.21f), 0.04f, 0.008f, 1, H(0x20232B), 0.04f, true, 10);
                        b.Box(new Vector3(bx, yy + s * 0.016f, Rail + 0.25f), new Vector3(0.05f, 0.006f, 0.008f), H(0xE04B4B), 0, false);
                    }
                }
            }
            if (kind == 1)
            {
                b.Cylinder(new Vector3(0.0f, 0, Rail + 0.46f), 0.04f, 0.035f, 2, H(0xFFFFFF), -1, true, 10);
                b.Ellipsoid(new Vector3(0.0f, 0, Rail + 0.53f), new Vector3(0.03f, 0.03f, 0.03f), 1f, H(0xF4F6FA), 0, true, 8, 5);
                b.Ellipsoid(new Vector3(0.0f, 0.02f, Rail + 0.60f), new Vector3(0.022f, 0.022f, 0.022f), 1f, H(0xE6EAF0), 0, true, 8, 5);
                b.Box(new Vector3(-0.12f, 0, Rail + 0.43f), new Vector3(0.018f, 0.018f, 0.018f), H(0xE53935), 0, true);
            }
            else if (kind == 2)
            {
                b.Box(new Vector3(0.0f, 0, Rail + 0.43f), new Vector3(0.14f, 0.06f, 0.022f), accent, 0, true);
            }
            else
            {
                b.Box(new Vector3(0.0f, 0, Rail + 0.43f), new Vector3(0.10f, 0.05f, 0.018f), H(0x6FA8E8), 0, true);
            }
        }

        // ---------------------------------------------------------------- tracks

        /// <summary>Straight and curved track cell content. Code: 0 N-S, 1 E-W, 2 N-E, 3 E-S, 4 S-W, 5 W-N.</summary>
        internal static void TrackBallast(StpBuilder b, int code, int cx, int cy)
        {
            b.Frame = StpFrame.Identity;
            var ballast = H(0xBBAE95);
            foreach (var seg in Centerline(code, cx, cy, 5))
                b.Box(new Vector3(seg.c.x, seg.c.y, 0.03f), new Vector3(seg.half + 0.022f, 0.31f, 0.03f), ballast, seg.yaw, true);
        }

        internal static void TrackRails(StpBuilder b, int code, int cx, int cy)
        {
            b.Frame = StpFrame.Identity;
            var tie = H(0x8A5A3A); var tieDark = H(0x6E4528);
            var segs = Centerline(code, cx, cy, 5);
            var tieCount = code < 2 ? 6 : 5;
            for (var i = 0; i < tieCount; i++)
            {
                var p = Along(code, cx, cy, (i + 0.5f) / tieCount, out var yaw);
                b.Box(new Vector3(p.x, p.y, 0.068f), new Vector3(0.036f, 0.235f, 0.018f), (i & 1) == 0 ? tie : tieDark, yaw, false);
            }
            foreach (var seg in segs)
                foreach (var off in new[] { -0.145f, 0.145f })
                {
                    var nrm = new Vector2(-Mathf.Sin(seg.yaw), Mathf.Cos(seg.yaw)) * off;
                    var c = seg.c + nrm;
                    b.Box(new Vector3(c.x, c.y, 0.083f), new Vector3(seg.half + 0.03f, 0.036f, 0.012f), H(0x4B4A55), seg.yaw, false);
                    b.Box(new Vector3(c.x, c.y, 0.098f), new Vector3(seg.half + 0.03f, 0.022f, 0.014f), Metal, seg.yaw, false);
                }
        }

        private struct Seg { internal Vector2 c; internal float half; internal float yaw; }

        private static List<Seg> Centerline(int code, int cx, int cy, int n)
        {
            var list = new List<Seg>();
            if (code == 0) { list.Add(new Seg { c = new Vector2(cx + 0.5f, cy + 0.5f), half = 0.5f, yaw = Mathf.PI * 0.5f }); return list; }
            if (code == 1) { list.Add(new Seg { c = new Vector2(cx + 0.5f, cy + 0.5f), half = 0.5f, yaw = 0 }); return list; }
            Arc(code, cx, cy, out var center, out var a0, out var da);
            for (var i = 0; i < n; i++)
            {
                var t0 = a0 + da * i / n; var t1 = a0 + da * (i + 1) / n;
                var p0 = center + new Vector2(Mathf.Cos(t0), Mathf.Sin(t0)) * 0.5f; var p1 = center + new Vector2(Mathf.Cos(t1), Mathf.Sin(t1)) * 0.5f;
                var d = p1 - p0;
                list.Add(new Seg { c = (p0 + p1) * 0.5f, half = d.magnitude * 0.5f, yaw = Mathf.Atan2(d.y, d.x) });
            }
            return list;
        }

        private static void Arc(int code, int cx, int cy, out Vector2 center, out float a0, out float da)
        {
            var q = Mathf.PI * 0.5f;
            switch (code)
            {
                case 2: center = new Vector2(cx + 1, cy); a0 = Mathf.PI; da = -q; break;       // N -> E
                case 3: center = new Vector2(cx + 1, cy + 1); a0 = -q; da = -q; break;         // E -> S
                case 4: center = new Vector2(cx, cy + 1); a0 = 0; da = -q; break;              // S -> W
                default: center = new Vector2(cx, cy); a0 = q; da = -q; break;                 // W -> N
            }
        }

        private static Vector2 Along(int code, int cx, int cy, float t, out float yaw)
        {
            if (code == 0) { yaw = Mathf.PI * 0.5f; return new Vector2(cx + 0.5f, cy + t); }
            if (code == 1) { yaw = 0; return new Vector2(cx + t, cy + 0.5f); }
            Arc(code, cx, cy, out var center, out var a0, out var da);
            var a = a0 + da * t;
            yaw = a + (da < 0 ? -Mathf.PI * 0.5f : Mathf.PI * 0.5f);
            return center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * 0.5f;
        }

        /// <summary>Plain straight stub entering the board from an endpoint edge (N/E/S/W as 0..3).</summary>
        internal static void Stub(StpBuilder b, int edge, int cx, int cy)
        {
            var vertical = edge == 0 || edge == 2;
            var dx = edge == 1 ? 1 : edge == 3 ? -1 : 0; var dy = edge == 2 ? 1 : edge == 0 ? -1 : 0;
            var mid = new Vector2(cx + 0.5f + dx * 0.78f, cy + 0.5f + dy * 0.78f);
            var yaw = vertical ? Mathf.PI * 0.5f : 0;
            b.Frame = StpFrame.Identity;
            b.Box(new Vector3(mid.x, mid.y, 0.03f), new Vector3(vertical ? 0.31f : 0.28f, vertical ? 0.28f : 0.31f, 0.03f), H(0xBBAE95), 0, true);
            for (var i = 0; i < 3; i++)
            {
                var p = mid + new Vector2(dx, dy) * ((i - 1) * 0.18f);
                b.Box(new Vector3(p.x, p.y, 0.068f), new Vector3(0.036f, 0.235f, 0.018f), H(0x8A5A3A), yaw, false);
            }
            foreach (var off in new[] { -0.145f, 0.145f })
            {
                var n = vertical ? new Vector2(off, 0) : new Vector2(0, off);
                b.Box(new Vector3(mid.x + n.x, mid.y + n.y, 0.083f), new Vector3(0.28f, 0.036f, 0.012f), H(0x4B4A55), yaw, false);
                b.Box(new Vector3(mid.x + n.x, mid.y + n.y, 0.098f), new Vector3(0.28f, 0.022f, 0.014f), Metal, yaw, false);
            }
        }

        // ---------------------------------------------------------------- scenery

        internal static void Tree(StpBuilder b, float x, float y, float s, int kind)
        {
            b.Frame = new StpFrame(new Vector3(x, y, 0), 0, s);
            b.Shadow(new Vector3(0.04f, 0.05f, 0.004f), 0.24f, 0.16f, 0.25f);
            b.Cylinder(new Vector3(0, 0, 0.1f), 0.05f, 0.1f, 2, H(0x8A5A3A), 0.04f, true, 8);
            if (kind % 3 == 2)
            {
                b.Cylinder(new Vector3(0, 0, 0.30f), 0.23f, 0.13f, 2, H(0x2F8F57), 0.03f, true, 12);
                b.Cylinder(new Vector3(0, 0, 0.47f), 0.16f, 0.12f, 2, H(0x3AA466), 0.02f, true, 12);
            }
            else
            {
                b.Ellipsoid(new Vector3(0, 0, 0.36f), new Vector3(0.23f, 0.23f, 0.24f), 1f, kind % 3 == 0 ? H(0x4CB35A) : H(0x62C25C), 0, true, 12, 7);
                b.Ellipsoid(new Vector3(-0.07f, -0.06f, 0.5f), new Vector3(0.12f, 0.12f, 0.12f), 1f, H(0x7ED06A), 0, true, 10, 6);
            }
        }

        internal static void Bush(StpBuilder b, float x, float y, float s)
        {
            b.Frame = new StpFrame(new Vector3(x, y, 0), 0, s);
            b.Ellipsoid(new Vector3(0, 0, 0.09f), new Vector3(0.15f, 0.12f, 0.1f), 1f, H(0x58BD58), 0, true, 10, 6);
            b.Ellipsoid(new Vector3(0.1f, 0.03f, 0.07f), new Vector3(0.09f, 0.08f, 0.07f), 1f, H(0x6BCB63), 0, true, 8, 5);
        }

        internal static void Cone(StpBuilder b, float x, float y)
        {
            b.Frame = new StpFrame(new Vector3(x, y, 0), 0, 1);
            b.Box(new Vector3(0, 0, 0.012f), new Vector3(0.07f, 0.07f, 0.012f), H(0x3A3A44), 0, true);
            b.Cylinder(new Vector3(0, 0, 0.11f), 0.055f, 0.095f, 2, H(0xFF7A1A), 0.016f, true, 10);
            b.Box(new Vector3(0, 0.0f, 0.12f), new Vector3(0.05f, 0.05f, 0.012f), H(0xFFFFFF), 0, false);
        }

        internal static void House(StpBuilder b, float x, float y, float w, float d, float h, Color wall, Color roof)
        {
            b.Frame = new StpFrame(new Vector3(x, y, 0), 0, 1);
            b.Box(new Vector3(0, 0, h * 0.5f), new Vector3(w * 0.5f, d * 0.5f, h * 0.5f), wall, 0, true);
            b.Prism(new Vector3(0, 0, h), w * 0.53f, d * 0.58f, h * 0.45f, roof, 0, true);
            var cols = Mathf.Max(2, (int)(w / 0.34f));
            for (var i = 0; i < cols; i++)
            {
                var wx = -w * 0.5f + w * (i + 0.5f) / cols;
                b.Box(new Vector3(wx, d * 0.5f + 0.004f, h * 0.58f), new Vector3(0.05f, 0.006f, 0.065f), H(0x6FA8DC), 0, false);
                b.Box(new Vector3(wx, d * 0.5f + 0.004f, h * 0.2f), new Vector3(0.05f, 0.006f, 0.065f), i == cols / 2 ? H(0x7A4B2F) : H(0x6FA8DC), 0, false);
            }
            b.Box(new Vector3(w * 0.5f + 0.004f, 0, h * 0.58f), new Vector3(0.006f, 0.05f, 0.065f), H(0x6FA8DC), 0, false);
        }

        internal static void Bench(StpBuilder b, float x, float y, float s)
        {
            b.Frame = new StpFrame(new Vector3(x, y, 0), 0, s);
            b.Shadow(new Vector3(0, 0.03f, 0.004f), 0.3f, 0.12f, 0.22f);
            b.Box(new Vector3(0, 0, 0.1f), new Vector3(0.27f, 0.07f, 0.014f), H(0xC98A4B), 0, true);
            b.Box(new Vector3(0, -0.06f, 0.2f), new Vector3(0.27f, 0.012f, 0.06f), H(0xD79C5A), 0, true);
            b.Box(new Vector3(-0.22f, 0, 0.05f), new Vector3(0.018f, 0.06f, 0.05f), H(0x4B4A55), 0, false);
            b.Box(new Vector3(0.22f, 0, 0.05f), new Vector3(0.018f, 0.06f, 0.05f), H(0x4B4A55), 0, false);
        }

        internal static void Signal(StpBuilder b, float x, float y, float s, bool green)
        {
            b.Frame = new StpFrame(new Vector3(x, y, 0), 0, s);
            b.Shadow(new Vector3(0.03f, 0.03f, 0.004f), 0.1f, 0.06f, 0.25f);
            b.Cylinder(new Vector3(0, 0, 0.25f), 0.022f, 0.25f, 2, H(0x6A6F7A), -1, true, 8);
            b.Box(new Vector3(0, 0, 0.56f), new Vector3(0.06f, 0.05f, 0.11f), H(0x2A2B33), 0, true);
            b.Cylinder(new Vector3(0, 0.056f, 0.60f), 0.032f, 0.008f, 1, green ? H(0x3A4A44) : H(0xFF3B3B), -1, false, 10);
            b.Cylinder(new Vector3(0, 0.056f, 0.52f), 0.032f, 0.008f, 1, green ? H(0x4CE07A) : H(0x4A3A3A), -1, false, 10);
        }

        internal static void Sign(StpBuilder b, float x, float y, float s, int variant)
        {
            b.Frame = new StpFrame(new Vector3(x, y, 0), 0, s);
            b.Shadow(new Vector3(0.03f, 0.03f, 0.004f), 0.12f, 0.06f, 0.25f);
            b.Cylinder(new Vector3(0, 0, 0.3f), 0.02f, 0.3f, 2, H(0x6A6F7A), -1, true, 8);
            b.Box(new Vector3(0.0f, 0, 0.58f), new Vector3(0.20f, 0.02f, 0.06f), variant == 0 ? H(0x1D62C0) : H(0xF2F2EE), 0, true);
            b.Box(new Vector3(0.0f, 0.022f, 0.58f), new Vector3(0.15f, 0.004f, 0.012f), variant == 0 ? H(0xFFFFFF) : H(0x1D62C0), 0, false);
            b.Box(new Vector3(0.05f, 0, 0.44f), new Vector3(0.17f, 0.02f, 0.05f), H(0xD6213A), 0, true);
            b.Box(new Vector3(0.05f, 0.022f, 0.44f), new Vector3(0.12f, 0.004f, 0.010f), H(0xFFFFFF), 0, false);
        }

        internal static void Barrier(StpBuilder b, float x, float y, float s)
        {
            b.Frame = new StpFrame(new Vector3(x, y, 0), 0, s);
            b.Shadow(new Vector3(0, 0.03f, 0.004f), 0.3f, 0.08f, 0.22f);
            b.Box(new Vector3(-0.24f, 0, 0.08f), new Vector3(0.02f, 0.04f, 0.08f), H(0x4B4A55), 0, true);
            b.Box(new Vector3(0.24f, 0, 0.08f), new Vector3(0.02f, 0.04f, 0.08f), H(0x4B4A55), 0, true);
            b.Box(new Vector3(0, 0, 0.17f), new Vector3(0.29f, 0.016f, 0.045f), H(0xF2F2EE), 0, true);
            for (var i = 0; i < 4; i++) b.Box(new Vector3(-0.21f + i * 0.14f, 0.018f, 0.17f), new Vector3(0.035f, 0.004f, 0.04f), H(0xD6213A), 0, false);
            b.Box(new Vector3(0, 0, 0.24f), new Vector3(0.03f, 0.03f, 0.03f), H(0xFFC62E), 0, true);
        }

        internal static void Display(StpBuilder b, float x, float y, float s)
        {
            b.Frame = new StpFrame(new Vector3(x, y, 0), 0, s);
            b.Shadow(new Vector3(0.03f, 0.03f, 0.004f), 0.14f, 0.07f, 0.25f);
            b.Cylinder(new Vector3(0, 0, 0.25f), 0.02f, 0.25f, 2, H(0x6A6F7A), -1, true, 8);
            b.Box(new Vector3(0, 0, 0.58f), new Vector3(0.20f, 0.03f, 0.08f), H(0x23262F), 0, true);
            b.Box(new Vector3(0, 0.032f, 0.64f), new Vector3(0.18f, 0.004f, 0.016f), H(0xD6213A), 0, false);
            for (var i = 0; i < 3; i++) b.Box(new Vector3(-0.04f, 0.032f, 0.58f - i * 0.03f), new Vector3(0.10f - i * 0.02f, 0.004f, 0.008f), H(0xF4E9B0), 0, false);
        }

        internal static void Platform(StpBuilder b, float x, float y, float w, float s)
        {
            b.Frame = new StpFrame(new Vector3(x, y, 0), 0, s);
            b.Box(new Vector3(0, 0, 0.07f), new Vector3(w * 0.5f, 0.2f, 0.07f), H(0xC9CDD6), 0, true);
            b.Box(new Vector3(0, 0.17f, 0.145f), new Vector3(w * 0.5f, 0.012f, 0.004f), H(0xFFD23E), 0, false);
            b.Box(new Vector3(-w * 0.3f, -0.05f, 0.34f), new Vector3(w * 0.14f, 0.15f, 0.015f), H(0x1D62C0), 0, true);
            b.Box(new Vector3(-w * 0.43f, -0.05f, 0.22f), new Vector3(0.014f, 0.014f, 0.12f), H(0x6A6F7A), 0, false);
            b.Box(new Vector3(-w * 0.17f, -0.05f, 0.22f), new Vector3(0.014f, 0.014f, 0.12f), H(0x6A6F7A), 0, false);
        }

        internal static void Tunnel(StpBuilder b, float x, float y, float s)
        {
            b.Frame = new StpFrame(new Vector3(x, y, 0), 0, s);
            b.Ellipsoid(new Vector3(0, -0.05f, 0.1f), new Vector3(0.62f, 0.5f, 0.52f), 0.9f, H(0x6DBB5C), 0, true, 18, 9);
            b.Box(new Vector3(0, 0.30f, 0.2f), new Vector3(0.30f, 0.05f, 0.2f), H(0xB6B2A8), 0, true);
            b.Ellipsoid(new Vector3(0, 0.352f, 0.18f), new Vector3(0.19f, 0.02f, 0.16f), 1f, H(0x22232B), 0, false, 14, 6);
            b.Box(new Vector3(0, 0.352f, 0.03f), new Vector3(0.19f, 0.02f, 0.03f), H(0x22232B), 0, false);
            b.Box(new Vector3(0, 0.356f, 0.02f), new Vector3(0.1f, 0.01f, 0.02f), H(0xF2F2EE), 0, false);
        }

        internal static void StopSign(StpBuilder b, float x, float y, float s)
        {
            b.Frame = new StpFrame(new Vector3(x, y, 0), 0, s);
            b.Shadow(new Vector3(0.1f, 0.05f, 0.004f), 0.4f, 0.14f, 0.22f);
            b.Cylinder(new Vector3(-0.2f, 0, 0.3f), 0.02f, 0.3f, 2, H(0x6A6F7A), -1, true, 8);
            b.Cylinder(new Vector3(-0.2f, 0.03f, 0.58f), 0.07f, 0.012f, 1, H(0xFFD23E), -1, true, 12);
            b.Ellipsoid(new Vector3(0.12f, 0, 0.17f), new Vector3(0.24f, 0.1f, 0.12f), 0.3f, H(0xFFD23E), 0, true, 14, 7);
            b.Box(new Vector3(0.12f, 0.098f, 0.2f), new Vector3(0.17f, 0.004f, 0.04f), H(0x2A3B5C), 0, false);
            b.Cylinder(new Vector3(0.0f, 0.06f, 0.055f), 0.045f, 0.015f, 1, Wheel, -1, true, 10);
            b.Cylinder(new Vector3(0.24f, 0.06f, 0.055f), 0.045f, 0.015f, 1, Wheel, -1, true, 10);
        }

        internal static void Shed(StpBuilder b, float x, float y, float s)
        {
            b.Frame = new StpFrame(new Vector3(x, y, 0), 0, s);
            b.Box(new Vector3(0, 0, 0.26f), new Vector3(0.55f, 0.4f, 0.26f), H(0xC7B79A), 0, true);
            b.Prism(new Vector3(0, 0, 0.52f), 0.58f, 0.45f, 0.2f, H(0xC0392B), 0, true);
            b.Box(new Vector3(0, 0.402f, 0.18f), new Vector3(0.16f, 0.006f, 0.18f), H(0x5C5F6B), 0, true);
            b.Box(new Vector3(0, 0.41f, 0.18f), new Vector3(0.012f, 0.004f, 0.17f), H(0x3C3F4B), 0, false);
            b.Box(new Vector3(0.38f, 0.402f, 0.3f), new Vector3(0.07f, 0.006f, 0.06f), H(0x6FA8DC), 0, false);
            b.Box(new Vector3(-0.38f, 0.402f, 0.3f), new Vector3(0.07f, 0.006f, 0.06f), H(0x6FA8DC), 0, false);
        }

        internal static void Bridge(StpBuilder b, float x, float y, float s)
        {
            b.Frame = new StpFrame(new Vector3(x, y, 0), 0, s);
            b.Box(new Vector3(0, 0, 0.18f), new Vector3(0.9f, 0.3f, 0.04f), H(0xA7ABB5), 0, true);
            b.Box(new Vector3(-0.8f, 0, 0.09f), new Vector3(0.1f, 0.28f, 0.09f), H(0x9199A6), 0, true);
            b.Box(new Vector3(0.8f, 0, 0.09f), new Vector3(0.1f, 0.28f, 0.09f), H(0x9199A6), 0, true);
            for (var i = 0; i < 6; i++) b.Box(new Vector3(-0.75f + i * 0.3f, 0.3f, 0.3f), new Vector3(0.015f, 0.012f, 0.1f), H(0x4B4A55), 0, false);
            b.Box(new Vector3(0, 0.3f, 0.4f), new Vector3(0.9f, 0.014f, 0.014f), H(0x4B4A55), 0, true);
        }

        internal static void Noticeboard(StpBuilder b, float x, float y, float s)
        {
            b.Frame = new StpFrame(new Vector3(x, y, 0), 0, s);
            b.Shadow(new Vector3(0.03f, 0.03f, 0.004f), 0.2f, 0.07f, 0.25f);
            b.Box(new Vector3(-0.15f, 0, 0.14f), new Vector3(0.016f, 0.016f, 0.14f), H(0x6A6F7A), 0, false);
            b.Box(new Vector3(0.15f, 0, 0.14f), new Vector3(0.016f, 0.016f, 0.14f), H(0x6A6F7A), 0, false);
            b.Box(new Vector3(0, 0, 0.36f), new Vector3(0.22f, 0.02f, 0.16f), H(0xF2F2EE), 0, true);
            b.Box(new Vector3(0, 0.022f, 0.47f), new Vector3(0.2f, 0.004f, 0.03f), H(0xD6213A), 0, false);
            for (var i = 0; i < 3; i++) b.Box(new Vector3(-0.02f - i * 0.02f, 0.022f, 0.36f - i * 0.045f), new Vector3(0.14f - i * 0.03f, 0.004f, 0.01f), H(0x2A3B5C), 0, false);
        }

        /// <summary>Draws a module name from the closed manifest vocabulary; unknown names are ignored.</summary>
        internal static void Module(StpBuilder b, string kind, float x, float y, float s)
        {
            switch (kind)
            {
                case "bench": Bench(b, x, y, s); break;
                case "wayfinding": Sign(b, x, y, s, 0); break;
                case "barrier": Barrier(b, x, y, s); break;
                case "indicator": Display(b, x, y, s); break;
                case "platform": Platform(b, x, y, 1.1f, s); break;
                case "tunnel": Tunnel(b, x, y, s); break;
                case "sev-stop": StopSign(b, x, y, s); break;
                case "signal": Signal(b, x, y, s, false); break;
                case "noticeboard": Noticeboard(b, x, y, s); break;
                case "bridge": Bridge(b, x, y, s); break;
                case "depot": Shed(b, x, y, s); break;
            }
        }

        /// <summary>Wraps current builder output in a scene layer.</summary>
        internal static void Flush(StpScene scene, StpBuilder b, int layer, bool hullsFirst = false)
        { scene.Add(b.Take(), layer, 0, hullsFirst); }
    }
}
