using System;
using System.Collections.Generic;
using STP.Application;
using UnityEngine;
using UnityEngine.Rendering;

namespace STP.Presentation.World
{
    /// <summary>Disposable QA train presentation consuming only a copied, actually built A-to-B path.</summary>
    public sealed class DevelopmentTrainRide : IDisposable
    {
        private const int QaLayer = 30;
        private readonly GameObject root;
        private readonly Transform train;
        private readonly Vector3[] points;
        private readonly IReadOnlyList<DevelopmentCell> path;
        private readonly List<DevelopmentCell> visited = new List<DevelopmentCell>();
        private readonly IReadOnlyList<DevelopmentCell> visitedView;
        private readonly Texture2D spriteTexture;
        private readonly Sprite sprite;
        private readonly Material material;
        private readonly RenderTexture? texture;
        private readonly float duration;
        private float elapsed;
        private int visitedCount;
        private Vector3 position;

        /// <summary>Creates a neutral world train and isolated transparent board camera; the input never contains a solution source.</summary>
        public DevelopmentTrainRide(IReadOnlyList<DevelopmentCell> builtPath, DevelopmentEndpoint a,
            DevelopmentEndpoint b, int width, int height, Material trainMaterial, float durationSeconds = 3f)
        {
            if (builtPath == null || builtPath.Count == 0) throw new ArgumentException("A built path is required.", nameof(builtPath));
            if (width < 2 || height < 2 || width > 32 || height > 32) throw new ArgumentOutOfRangeException(nameof(width));
            if (trainMaterial == null || trainMaterial.shader == null) throw new ArgumentNullException(nameof(trainMaterial));
            if (float.IsNaN(durationSeconds) || float.IsInfinity(durationSeconds) || durationSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(durationSeconds));
            var copy = new DevelopmentCell[builtPath.Count];
            for (var index = 0; index < copy.Length; index++)
            {
                var cell = builtPath[index];
                if (cell.X < 0 || cell.Y < 0 || cell.X >= width || cell.Y >= height) throw new ArgumentException("Path leaves board.", nameof(builtPath));
                if (index > 0 && Math.Abs(cell.X - copy[index - 1].X) + Math.Abs(cell.Y - copy[index - 1].Y) != 1)
                    throw new ArgumentException("Path is not contiguous.", nameof(builtPath));
                copy[index] = cell;
            }
            if (copy[0].X != a.X || copy[0].Y != a.Y || copy[copy.Length - 1].X != b.X || copy[copy.Length - 1].Y != b.Y)
                throw new ArgumentException("Path must run from A to B.", nameof(builtPath));
            path = Array.AsReadOnly(copy);
            visitedView = visited.AsReadOnly();
            duration = durationSeconds;
            points = new Vector3[copy.Length + 2];
            points[0] = Center(copy[0], height) + Exterior(a.Edge) * .5f;
            for (var index = 0; index < copy.Length; index++) points[index + 1] = Center(copy[index], height);
            points[points.Length - 1] = Center(copy[copy.Length - 1], height) + Exterior(b.Edge) * .5f;

            root = new GameObject("WP038 QA train presentation");
            root.layer = QaLayer;
            var body = new GameObject("WP038 QA train");
            body.layer = QaLayer;
            body.transform.SetParent(root.transform, false);
            train = body.transform;
            train.position = position = points[0];
            spriteTexture = new Texture2D(24, 16, TextureFormat.RGBA32, false);
            spriteTexture.name = "WP038 neutral technical train";
            var pixels = new Color[24 * 16];
            for (var y = 0; y < 16; y++)
                for (var x = 0; x < 24; x++)
                    pixels[y * 24 + x] = x < 2 || x > 21 || y < 2 || y > 13 ? Color.clear :
                        (x > 15 && y > 5 && y < 11 ? new Color(.05f, .11f, .18f, 1f) : new Color(1f, .61f, .12f, 1f));
            spriteTexture.SetPixels(pixels);
            spriteTexture.Apply(false, false);
            sprite = Sprite.Create(spriteTexture, new Rect(0, 0, 24, 16), new Vector2(.5f, .5f), 30f);
            material = new Material(trainMaterial);
            var renderer = body.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sharedMaterial = material;
            renderer.sortingOrder = 100;
            var cameraObject = new GameObject("WP038 isolated board camera");
            cameraObject.layer = QaLayer;
            cameraObject.transform.SetParent(root.transform, false);
            cameraObject.transform.position = new Vector3(width * .5f, height * .5f, -10f);
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = height * .5f;
            camera.aspect = (float)width / height;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.clear;
            camera.cullingMask = 1 << QaLayer;
            camera.nearClipPlane = .1f;
            camera.farClipPlane = 30f;
            camera.allowHDR = false;
            camera.allowMSAA = false;
            camera.enabled = SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null;
            if (camera.enabled)
            {
                texture = new RenderTexture(768, 768, 24, RenderTextureFormat.ARGB32) { name = "WP038 transparent train board" };
                if (!texture.Create())
                {
                    Dispose();
                    throw new InvalidOperationException("QA train render target could not be created.");
                }
                camera.targetTexture = texture;
            }
        }

        /// <summary>Transparent inner-board image, absent in explicitly headless semantic tests.</summary>
        public Texture? Texture => texture;
        /// <summary>Copied immutable built path.</summary>
        public IReadOnlyList<DevelopmentCell> Path => path;
        /// <summary>Actual visited cell centers in movement order.</summary>
        public IReadOnlyList<DevelopmentCell> VisitedCells => visitedView;
        /// <summary>Current world-space train position.</summary>
        public Vector3 Position => position;
        /// <summary>Whether the train has reached the exterior B port.</summary>
        public bool IsFinished { get; private set; }
        /// <summary>Whether owned Unity resources have been released.</summary>
        public bool IsDisposed { get; private set; }

        /// <summary>Advances presentation time; suspended sessions provide no ticks.</summary>
        public void Tick(float seconds)
        {
            if (IsDisposed || IsFinished) return;
            if (float.IsNaN(seconds) || float.IsInfinity(seconds) || seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));
            elapsed = Mathf.Min(duration, elapsed + seconds);
            var step = elapsed / duration * (points.Length - 1);
            var segment = Mathf.Min(points.Length - 2, Mathf.FloorToInt(step));
            while (visitedCount < path.Count && step >= visitedCount + 1)
            {
                visited.Add(path[visitedCount]);
                visitedCount++;
            }
            train.position = position = Vector3.Lerp(points[segment], points[segment + 1], step - segment);
            var direction = points[segment + 1] - points[segment];
            train.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            IsFinished = elapsed >= duration;
        }

        /// <summary>Idempotently destroys all instance-owned rendering resources.</summary>
        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            if (texture != null) { texture.Release(); Destroy(texture); }
            Destroy(root);
            Destroy(sprite);
            Destroy(spriteTexture);
            Destroy(material);
        }

        private static Vector3 Center(DevelopmentCell cell, int height) => new Vector3(cell.X + .5f, height - cell.Y - .5f, 0);
        private static Vector3 Exterior(DevelopmentEdge edge) => edge switch
        {
            DevelopmentEdge.N => Vector3.up, DevelopmentEdge.E => Vector3.right,
            DevelopmentEdge.S => Vector3.down, DevelopmentEdge.W => Vector3.left,
            _ => throw new ArgumentOutOfRangeException(nameof(edge))
        };
        private static void Destroy(UnityEngine.Object value)
        {
            if (UnityEngine.Application.isPlaying) UnityEngine.Object.Destroy(value);
            else UnityEngine.Object.DestroyImmediate(value);
        }
    }
}
