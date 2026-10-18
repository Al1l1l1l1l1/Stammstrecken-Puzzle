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
    /// <summary>Explicit scene-local composition of the ADR033 transient, unsaved QA puzzle loop.</summary>
    [DisallowMultipleComponent]
    public sealed class DevelopmentPuzzleInstaller : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset? uiTree = null;
        [SerializeField] private StyleSheet? uiStyles = null;
        [SerializeField] private TextAsset? qaLocale = null;
        [SerializeField] private PanelSettings? panelSettings = null;
        [SerializeField] private Material? trainMaterial = null;
        [SerializeField] private bool largeScenario = false;
        [SerializeField] private float rideDurationSeconds = 3f;
        private DevelopmentClock? clock;
        private DevelopmentPuzzleSession? model;
        private DevelopmentPuzzlePresenter? presenter;
        private DevelopmentTrainRide? ride;
        private Guid rideAttempt;
        private bool applicationPaused;
        private bool hasFocus = true;
        private bool suspended;
        private bool disposed;

        /// <summary>Actual transient application session; no save or reward service is composed.</summary>
        public DevelopmentPuzzleSession Model => model ?? throw new InvalidOperationException("QA session not initialized.");
        /// <summary>Actual scene-owned UI presenter.</summary>
        public DevelopmentPuzzlePresenter Presenter => presenter ?? throw new InvalidOperationException("QA view not initialized.");
        /// <summary>Actual scene-owned runtime UI document.</summary>
        public UIDocument Document { get; private set; } = null!;
        /// <summary>Current presentation, absent before the real PuzzleSolved path exists.</summary>
        public DevelopmentTrainRide? Ride => ride;
        /// <summary>Current monotonic time supplied to the reducer and view.</summary>
        public long NowMilliseconds => clock?.Milliseconds ?? 0;

        /// <summary>Rejects every profile or identifier except the existing explicit internal QA identity.</summary>
        public static void ValidateQaProfile(string profile, string identifier)
        {
            var configuration = new BootstrapConfiguration();
            if (!string.Equals(profile, configuration.Profile, StringComparison.Ordinal) ||
                !string.Equals(identifier, configuration.ApplicationIdentifier, StringComparison.Ordinal))
                throw new InvalidOperationException("DevelopmentPuzzle requires the explicit internal QA profile and identity.");
        }

        /// <summary>Validates all serialized boundaries before composition or build mutation.</summary>
        public static void ValidateReferences(VisualTreeAsset? tree, StyleSheet? styles, TextAsset? locale,
            PanelSettings? panel, Material? material)
        {
            if (tree == null || styles == null || locale == null || panel == null || material == null || material.shader == null)
                throw new InvalidOperationException("DevelopmentPuzzle requires its explicit UXML, USS, locale, panel and train material.");
            if (panel.themeStyleSheet == null)
                throw new InvalidOperationException("DevelopmentPuzzle requires its runtime theme.");
        }

        /// <summary>Configures an inactive installer through an explicit editor/test composition boundary.</summary>
        public void ConfigureForQa(VisualTreeAsset tree, StyleSheet styles, TextAsset locale,
            PanelSettings panel, Material material, bool large = false, float durationSeconds = 3f)
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
            largeScenario = large;
            rideDurationSeconds = durationSeconds;
        }

        private void Awake()
        {
            var configuration = new BootstrapConfiguration();
            ValidateQaProfile(configuration.Profile, UnityEngine.Application.isEditor ? configuration.ApplicationIdentifier : UnityEngine.Application.identifier);
            ValidateReferences(uiTree, uiStyles, qaLocale, panelSettings, trainMaterial);
            if (float.IsNaN(rideDurationSeconds) || float.IsInfinity(rideDurationSeconds) || rideDurationSeconds <= 0)
                throw new InvalidOperationException("QA ride duration must be finite and positive.");
            clock = new DevelopmentClock();
            model = DevelopmentPuzzleData.CreateSession(largeScenario);
            Document = gameObject.AddComponent<UIDocument>();
            Document.panelSettings = panelSettings;
            uiTree!.CloneTree(Document.rootVisualElement);
            Document.rootVisualElement.styleSheets.Add(uiStyles!);
            try
            {
                presenter = new DevelopmentPuzzlePresenter(Document.rootVisualElement, model, qaLocale!, () => clock.Milliseconds);
                presenter.Refresh();
            }
            catch
            {
                DisposeComposition();
                enabled = false;
                throw;
            }
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
                    ride = new DevelopmentTrainRide(model.BuiltPath, model.A, model.B, model.Width, model.Height, trainMaterial!, rideDurationSeconds);
                    presenter.SetTrainTexture(ride.Texture);
                }
                if (!suspended) ride.Tick(Time.unscaledDeltaTime);
                if (ride.IsFinished)
                {
                    model.CompleteTrainRide(rideAttempt);
                    presenter.SetTrainTexture(null);
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
            presenter = null;
            ride = null;
        }
    }
}
