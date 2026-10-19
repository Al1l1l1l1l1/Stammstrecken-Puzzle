using System;
using STP.Application;
using STP.Infrastructure.Content;
using STP.Platform;
using STP.Presentation.UI;
using STP.Presentation.World;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Bootstrap
{
    /// <summary>Explicit scene-local composition of the ADR034 transient, unsaved QA puzzle loop.</summary>
    [DisallowMultipleComponent]
    public sealed class Public240ReviewInstaller : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset? uiTree = null;
        [SerializeField] private StyleSheet? uiStyles = null;
        [SerializeField] private TextAsset? qaLocale = null;
        [SerializeField] private PanelSettings? panelSettings = null;
        [SerializeField] private Material? trainMaterial = null;
        [SerializeField] private TextAsset? publicData = null;
        [SerializeField] private float rideDurationSeconds = 3f;
        private DevelopmentClock? clock;
        private IReviewPuzzleSession? model;
        private Public240ReviewData? catalog;
        private readonly System.Collections.Generic.Dictionary<int,IReviewPuzzleSession> sessions = new System.Collections.Generic.Dictionary<int,IReviewPuzzleSession>();
        private Public240ReviewPresenter? presenter;
        private DevelopmentTrainRide? ride;
        private Camera? screenCamera;
        private Guid rideAttempt;
        private bool applicationPaused;
        private bool hasFocus = true;
        private bool suspended;
        private bool disposed;

        /// <summary>Actual transient application session; no save or reward service is composed.</summary>
        public IReviewPuzzleSession Model => model ?? throw new InvalidOperationException("QA session not initialized.");
        /// <summary>Exact count of separately validated public entries.</summary>
        public int PuzzleCount => catalog?.Puzzles.Count ?? 0;
        /// <summary>The public immutable catalog supplied only by the explicitly serialized TextAsset.</summary>
        public IPublicPuzzleCatalog Catalog => catalog ?? throw new InvalidOperationException("PUBLIC240_NOT_INITIALIZED");
        /// <summary>Actual scene-owned UI presenter.</summary>
        public Public240ReviewPresenter Presenter => presenter ?? throw new InvalidOperationException("QA view not initialized.");
        /// <summary>Actual scene-owned runtime UI document.</summary>
        public UIDocument Document { get; private set; } = null!;
        /// <summary>Current presentation, absent before the real PuzzleSolved path exists.</summary>
        public DevelopmentTrainRide? Ride => ride;
        /// <summary>Explicit screen output keeps URP overlay UI visible while the separate train camera renders offscreen.</summary>
        public Camera ScreenCamera => screenCamera ?? throw new InvalidOperationException("QA screen output not initialized.");
        /// <summary>Current monotonic time supplied to the reducer and view.</summary>
        public long NowMilliseconds => clock?.Milliseconds ?? 0;

        /// <summary>Rejects every profile or identifier except the existing explicit internal QA identity.</summary>
        public static void ValidateQaProfile(string profile, string identifier)
        {
            var configuration = new BootstrapConfiguration();
            if (!string.Equals(profile, configuration.Profile, StringComparison.Ordinal) ||
                !string.Equals(identifier, configuration.ApplicationIdentifier, StringComparison.Ordinal))
                throw new InvalidOperationException("Public240Review requires the explicit internal QA profile and identity.");
        }

        /// <summary>Validates the explicitly serialized public TextAsset without shipping a repository audit lock.</summary>
        public static void ValidatePublicData(TextAsset data)
        {
            if(data==null) throw new ArgumentNullException(nameof(data));
            Public240ReviewData.VerifyAssetBytes(data.bytes);
            _ = new Public240ReviewData(data.bytes);
        }

        /// <summary>Validates all serialized boundaries before composition or build mutation.</summary>
        public static void ValidateReferences(VisualTreeAsset? tree, StyleSheet? styles, TextAsset? locale,
            PanelSettings? panel, Material? material)
        {
            if (tree == null || styles == null || locale == null || panel == null || material == null || material.shader == null)
                throw new InvalidOperationException("Public240Review requires its explicit UXML, USS, locale, panel and train material.");
            if (panel.themeStyleSheet == null)
                throw new InvalidOperationException("Public240Review requires its runtime theme.");
        }

        /// <summary>Configures an inactive installer through an explicit editor/test composition boundary.</summary>
        public void ConfigureForQa(VisualTreeAsset tree, StyleSheet styles, TextAsset locale,
            PanelSettings panel, Material material, TextAsset data, float durationSeconds = 3f)
        {
            if (model != null || disposed) throw new InvalidOperationException("QA composition is already initialized or disposed.");
            ValidateReferences(tree, styles, locale, panel, material);
            if (float.IsNaN(durationSeconds) || float.IsInfinity(durationSeconds) || durationSeconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(durationSeconds));
            uiTree = tree;
            uiStyles = styles;
            qaLocale = locale;
            panelSettings = panel;
            trainMaterial = material;
            publicData = data ?? throw new ArgumentNullException(nameof(data));
            rideDurationSeconds = durationSeconds;
        }

        private void Awake()
        {
            var configuration = new BootstrapConfiguration();
            ValidateQaProfile(configuration.Profile, UnityEngine.Application.isEditor ? configuration.ApplicationIdentifier : UnityEngine.Application.identifier);
            ValidateReferences(uiTree, uiStyles, qaLocale, panelSettings, trainMaterial);
            if (float.IsNaN(rideDurationSeconds) || float.IsInfinity(rideDurationSeconds) || rideDurationSeconds <= 0)
                throw new InvalidOperationException("QA ride duration must be finite and positive.");
            if(publicData==null) throw new InvalidOperationException("PUBLIC240_DATA_MISSING");
            Public240ReviewData.VerifyAssetBytes(publicData.bytes);
            catalog = new Public240ReviewData(publicData.bytes);
            var screenOutput = new GameObject("Public240 explicit QA screen output");
            screenOutput.transform.SetParent(transform, false);
            screenOutput.transform.localPosition = new Vector3(0, 0, -10f);
            screenCamera = screenOutput.AddComponent<Camera>();
            screenCamera.orthographic = true;
            screenCamera.cullingMask = 0;
            screenCamera.clearFlags = CameraClearFlags.SolidColor;
            screenCamera.backgroundColor = new Color(240f / 255f, 241f / 255f, 237f / 255f, 1f);
            screenCamera.targetTexture = null;
            screenCamera.depth = 100f;
            screenCamera.nearClipPlane = .1f;
            screenCamera.farClipPlane = 30f;
            screenCamera.allowHDR = false;
            screenCamera.allowMSAA = false;
            screenCamera.enabled = SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null;
            clock = new DevelopmentClock();
            model = new Public240ReviewSession(catalog.Puzzles[0]); sessions.Add(0,model);
            Document = gameObject.AddComponent<UIDocument>();
            Document.panelSettings = panelSettings;
            uiTree!.CloneTree(Document.rootVisualElement);
            Document.rootVisualElement.styleSheets.Add(uiStyles!);
            try
            {
                presenter = new Public240ReviewPresenter(Document.rootVisualElement, model, qaLocale!, () => clock.Milliseconds, catalog.Puzzles, SelectPuzzle);
                presenter.Refresh();
            }
            catch
            {
                DisposeComposition();
                enabled = false;
                throw;
            }
        }

        /// <summary>Explicit selection command from the real picker; retained attempts remain transient.</summary>
        public void SelectPuzzle(int index)
        {
            if(disposed || catalog==null || clock==null || model==null || presenter==null) throw new InvalidOperationException("PUBLIC240_NOT_INITIALIZED");
            if(index<0 || index>=catalog.Puzzles.Count) throw new ArgumentOutOfRangeException(nameof(index));
            if(model.Phase==DevelopmentPreviewPhase.TrainRide) throw new InvalidOperationException("TRAIN_RIDE_ACTIVE");
            model.Pause(clock.Milliseconds);
            if(!sessions.TryGetValue(index,out var next)) {next=new Public240ReviewSession(catalog.Puzzles[index]);sessions.Add(index,next);}
            model=next;
            if(!suspended) model.Resume(clock.Milliseconds);
            presenter.BindSession(model);
        }

        private void Update()
        {
            if (disposed || model == null || presenter == null) return;
            if (suspended && model.Phase == DevelopmentPreviewPhase.Editing && !model.IsPaused)
                model.Pause(NowMilliseconds);
            if (model.Phase == DevelopmentPreviewPhase.TrainRide)
            {
                if (ride == null)
                {
                    rideAttempt = model.AttemptId;
                    ride = new DevelopmentTrainRide(model.BuiltPath, model.EndpointA, model.EndpointB, model.Width, model.Height, trainMaterial!, rideDurationSeconds);
                    presenter.SetTrainTexture(ride.Texture);
                }
                if (!suspended) ride.Tick(Time.unscaledDeltaTime);
                if (ride.IsFinished)
                {
                    model.CompleteTrainRide(rideAttempt);
                    presenter.SetTrainTexture(null);
                    ride.Dispose();
                    ride = null;
                }
            }
            else if (ride != null)
            {
                ride.Dispose();
                ride = null;
                presenter.SetTrainTexture(null);
            }
            presenter.Refresh();
        }

        /// <summary>Explicit lifecycle adapter used by the real Unity pause callback and its contract tests.</summary>
        public void SetApplicationPaused(bool paused)
        {
            applicationPaused = paused;
            RefreshSuspension();
        }

        /// <summary>Explicit lifecycle adapter used by the real Unity focus callback and its contract tests.</summary>
        public void SetApplicationFocus(bool focused)
        {
            hasFocus = focused;
            RefreshSuspension();
        }

        private void OnApplicationPause(bool paused) => SetApplicationPaused(paused);
        private void OnApplicationFocus(bool focused) => SetApplicationFocus(focused);
        private void RefreshSuspension()
        {
            if (disposed || model == null || clock == null) return;
            var next = applicationPaused || !hasFocus;
            if (next == suspended) return;
            suspended = next;
            if (suspended) model.Pause(clock.Milliseconds);
            else model.Resume(clock.Milliseconds);
            presenter?.Refresh();
        }

        private void OnDestroy() => DisposeComposition();
        private void DisposeComposition()
        {
            if (disposed) return;
            disposed = true;
            presenter?.Dispose();
            ride?.Dispose();
            if (screenCamera != null) UnityEngine.Object.Destroy(screenCamera.gameObject);
            presenter = null;
            ride = null;
            screenCamera = null;
            sessions.Clear(); model=null; catalog=null;
            // UIDocument.OnDisable may already have released the root when the host is destroyed.
            // When only this installer is removed, its separately added document is still ours to destroy.
            if(Document!=null) {Document.rootVisualElement?.Clear(); UnityEngine.Object.Destroy(Document);}
        }
    }
}
