using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;
using STP.Audio;
using STP.Platform;
using STP.MobileServices.Store;
using STP.Presentation.UI;
using STP.Presentation.World;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

namespace STP.Bootstrap
{
    /// <summary>New mobile presentation over the isolated, durable review composition. No production promotion.</summary>
    [DisallowMultipleComponent]
    public sealed class MobilePreviewInstaller : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset? tree;
        [SerializeField] private StyleSheet? styles;
        [SerializeField] private TextAsset? locale;
        [SerializeField] private PanelSettings? panel;
        [SerializeField] private TextAsset? publicData;
        [SerializeField] private TextAsset? campaignData;
        [SerializeField] private TextAsset? completionData;
        [SerializeField] private TextAsset? railwayAssets;
        [SerializeField] private TextAsset? cosmeticsData;
        [SerializeField] private TextAsset? candidateBindings;
        [SerializeField] private TextAsset? campaignText;
        [SerializeField] private TextAsset? endlessProfilesData;
        [SerializeField] private TextAsset? endlessCompletionData;
        [SerializeField] private TextAsset? endlessBindingsData;
        [SerializeField] private AudioMixer? audioMixer;
        [SerializeField] private AudioClip[] audioClips=Array.Empty<AudioClip>();
        private ProductionGameAudio? gameAudio;
        private LocalSettings? appliedAudioSettings;
        private NativeHapticsAdapter? haptics;
        private MobileCandidateComposition? candidate;
        private MobileCandidateExperience? experience;
        private StoreConfiguration? storeConfiguration;
        private IapPrivacyReadiness? storeReadiness;
        private readonly CancellationTokenSource lifetime = new CancellationTokenSource();
        private LocalSaveClock? clock;
        private MainThreadSaveDispatcher? dispatcher;
        private SavedPublic240ReviewSession? model;
        private ProductionGamePresenter? presenter;
        private ProductionTrainRide? ride;
        private IReviewPuzzleSession? rideView;
        private Guid rideAttempt;
        private Task operation = Task.CompletedTask, lifecycle = Task.CompletedTask;
        private bool paused, focused = true, suspended, hasSuspension, disposed, started, fault, saveBlocked;
        private Rect lastSafeArea;
        private int lastScreenWidth, lastScreenHeight;
        private int startupEpoch;
        private Camera? output;
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
        private string? testRoot;
        private Func<string>? testEndlessSeed;
        public string StartupDiagnostic {get;private set;}="NONE";
#endif
        /// <summary>Actual durable bridge; null while loading.</summary>
        public SavedPublic240ReviewSession? Model => model;
        public MobileCandidateComposition? Candidate => candidate;
        /// <summary>Actual scene presenter.</summary>
        public ProductionGamePresenter Presenter => presenter ?? throw new InvalidOperationException("PREVIEW_NOT_READY");
        /// <summary>Actual overlay document.</summary>
        public UIDocument Document { get; private set; } = null!;
        /// <summary>Observed user, save and lifecycle work.</summary>
        public Task PendingOperation => Task.WhenAll(operation, lifecycle,experience?.Pending??Task.CompletedTask);
        /// <summary>Completion of startup composition and initial load.</summary>
        public Task InitializeTask { get; private set; } = Task.CompletedTask;
        /// <summary>Completion of all owned work after destruction.</summary>
        public Task ShutdownTask { get; private set; } = Task.CompletedTask;

        /// <summary>Explicit scene references; configure only while inactive, before Awake.</summary>
        public void Configure(VisualTreeAsset view, StyleSheet stylesheet, TextAsset translations,
            PanelSettings settings, TextAsset puzzles, TextAsset campaign, TextAsset completion, TextAsset assets)
        {
            if (started || disposed) throw new InvalidOperationException("PREVIEW_ALREADY_STARTED");
            tree = view; styles = stylesheet; locale = translations; panel = settings;
            publicData = puzzles; campaignData = campaign; completionData = completion;
            railwayAssets = assets;
            ValidateReferences();
        }
        /// <summary>Opt-in full feature candidate; historical review construction keeps its original catalogs and save namespace.</summary>
        public void ConfigureCandidate(VisualTreeAsset view, StyleSheet stylesheet, TextAsset translations,
            PanelSettings settings, TextAsset puzzles, TextAsset campaign, TextAsset completion, TextAsset assets,
            TextAsset cosmetics, TextAsset bindings, TextAsset? playerTexts=null)
        {
            if(started||disposed)throw new InvalidOperationException("PREVIEW_ALREADY_STARTED");
            cosmeticsData=cosmetics; candidateBindings=bindings;campaignText=playerTexts;
            Configure(view,stylesheet,translations,settings,puzzles,campaign,completion,assets);
        }
        public void ConfigureAudio(AudioMixer mixer,AudioClip[] clips)
        {
            if(started||disposed)throw new InvalidOperationException("PREVIEW_ALREADY_STARTED");
            audioMixer=mixer;audioClips=(AudioClip[])clips.Clone();
        }
        /// <summary>Explicit extra DRAFT sources for the actual post-season generator, before the component starts.</summary>
        public void ConfigureEndless(TextAsset profiles,TextAsset completion,TextAsset bindings)
        {
            if(started||disposed||candidateBindings==null)throw new InvalidOperationException("ENDLESS_CONFIGURATION_NOT_ALLOWED");
            endlessProfilesData=profiles??throw new ArgumentNullException(nameof(profiles));
            endlessCompletionData=completion??throw new ArgumentNullException(nameof(completion));
            endlessBindingsData=bindings??throw new ArgumentNullException(nameof(bindings));
        }

        /// <summary>Read-only reference and public-catalog preflight.</summary>
        public void ValidateReferences()
        {
            if (tree == null || styles == null || locale == null || panel == null ||
                publicData == null || campaignData == null || completionData == null || railwayAssets == null)
                throw new InvalidOperationException("PREVIEW_REQUIRED_REFERENCE_MISSING:" +
                    (tree == null ? "tree," : "") + (styles == null ? "styles," : "") +
                    (locale == null ? "locale," : "") + (panel == null ? "panel," : "") +
                    (publicData == null ? "public," : "") + (campaignData == null ? "campaign," : "") +
                    (completionData == null ? "completion," : "") + (railwayAssets == null ? "assets" : ""));
            if(candidateBindings!=null)
            {
                if(cosmeticsData==null)throw new InvalidOperationException("CANDIDATE_COSMETICS_MISSING");
                MobileCandidateComposition.ValidateCatalogBytes(publicData.bytes,campaignData.bytes,completionData.bytes,cosmeticsData.bytes,candidateBindings.bytes);
            }
            else SavedPublic240ReviewInstaller.ValidateCatalogData(publicData, campaignData, completionData);
        }
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
        /// <summary>Editor tests use the same existing physical-path boundary, never the real player save.</summary>
        public void ConfigureTestRoot(string root)
        {
            if (started) throw new InvalidOperationException("PREVIEW_ALREADY_STARTED");
            string real = UnityEngine.Application.persistentDataPath;
            string temp = Path.GetTempPath();
            string? configured = Environment.GetEnvironmentVariable("STP_TEST_TEMP_ROOT");
            testRoot = Task.Run(() => SavedPublic240ReviewInstaller.ValidateTestRootBoundary(root, real, temp, configured)).GetAwaiter().GetResult();
        }
        /// <summary>Deterministic test input for the same actual generator; never serialized or compiled into a player.</summary>
        public void ConfigureTestEndlessSeeds(params ulong[] seeds)
        {
            if(started)throw new InvalidOperationException("PREVIEW_ALREADY_STARTED");
            if(seeds==null||seeds.Length==0)throw new ArgumentException("PREVIEW_TEST_SEEDS_REQUIRED",nameof(seeds));
            var values=(ulong[])seeds.Clone();int position=0;
            testEndlessSeed=()=>position<values.Length?values[position++].ToString(System.Globalization.CultureInfo.InvariantCulture):
                throw new InvalidOperationException("PREVIEW_TEST_SEEDS_EXHAUSTED");
        }
#endif
        /// <summary>Optional actual build-profile metadata, supplied before Awake. No SDK starts or review authority is inferred.</summary>
        public void ConfigureCandidateStore(StoreConfiguration configuration,IapPrivacyReadiness? reviewedReadiness)
        {
            if(started||disposed||storeConfiguration!=null)throw new InvalidOperationException("PREVIEW_STORE_INPUT_WINDOW_CLOSED");
            if(candidateBindings==null)throw new InvalidOperationException("PREVIEW_CANDIDATE_CONFIGURATION_REQUIRED");
            storeConfiguration=configuration??throw new ArgumentNullException(nameof(configuration));
            storeReadiness=reviewedReadiness;
        }
        private void Awake()
        {
            started = true;
            ValidateReferences();
            clock = new LocalSaveClock();
            dispatcher = new MainThreadSaveDispatcher();
            Document = gameObject.AddComponent<UIDocument>();
            Document.panelSettings = panel;
            tree!.CloneTree(Document.rootVisualElement);
            Document.rootVisualElement.styleSheets.Add(styles!);
            presenter = new ProductionGamePresenter(Document.rootVisualElement, locale!, () => clock.MonotonicMilliseconds, RunRequestAsync);
            presenter.BindAssetCatalog(railwayAssets!, true);
            var screen = new GameObject("Stammstrecken Puzzle screen");
            screen.transform.SetParent(transform, false);
            output = screen.AddComponent<Camera>();
            screen.AddComponent<AudioListener>();
            output.cullingMask = 0; output.clearFlags = CameraClearFlags.SolidColor;
            output.backgroundColor = new Color(.96f, .92f, .81f); output.depth = 100;
            output.enabled = SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null;
            haptics=new NativeHapticsAdapter();
            if(audioMixer!=null)
            {
                gameAudio=gameObject.AddComponent<ProductionGameAudio>();gameAudio.Configure(audioMixer,audioClips);
            }
            presenter.BindFeedback(cue=>
            {
                gameAudio?.Play(cue.ToString());
                if(cue==ProductionFeedbackCue.ToolFocus||cue==ProductionFeedbackCue.TrackSeat||cue==ProductionFeedbackCue.EmptyMark||
                    cue==ProductionFeedbackCue.OccupancyNote||cue==ProductionFeedbackCue.Clear||cue==ProductionFeedbackCue.Undo)haptics?.PulseInteraction();
            });
            string root = StorageRoot;
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
            if (testRoot != null) root = testRoot;
#endif
            InitializeTask = StartAsync(root, ++startupEpoch);
            operation = InitializeTask;
        }

        private async Task StartAsync(string root, int epoch)
        {
            fault = false; saveBlocked = false;
            presenter!.ShowStartupLoading();
            try
            {
                var configuration = new BootstrapConfiguration();
                SavedPublic240ReviewInstaller.ValidateQaProfile(configuration.Profile,
                    UnityEngine.Application.isEditor ? configuration.ApplicationIdentifier : UnityEngine.Application.identifier);
                byte[] p = publicData!.bytes, c = campaignData!.bytes, v = completionData!.bytes;
                string build = UnityEngine.Application.isEditor ? "editor-mobile-preview" : UnityEngine.Application.buildGUID;
                if(candidateBindings!=null)
                {
                    candidate=await MobileCandidateComposition.CreateAsync(p,c,v,cosmeticsData!.bytes,candidateBindings.bytes,root,
                        clock!,dispatcher!,UnityEngine.Application.version,build,lifetime.Token,
                        endlessProfilesData?.bytes,endlessCompletionData?.bytes,endlessBindingsData?.bytes);
                    if(disposed||epoch!=startupEpoch)return;
                    model=SavedPublic240ReviewSession.ForMobileCandidate(candidate.Persisted,candidate.Autosave,candidate.Catalog,clock!,dispatcher!,candidate.Profile);
                }
                else
                {
                    var composition = await SavedPublic240Composition.CreateAsync(p, c, v, root, clock!, dispatcher!,
                        UnityEngine.Application.version, build, lifetime.Token);
                    if (disposed || epoch != startupEpoch) return;
                    model = new SavedPublic240ReviewSession(composition.Persisted, composition.Autosave, composition.Catalog, clock!, dispatcher!);
                }
                presenter.BindModel(model);
                if(campaignText!=null)presenter.BindCampaignText(campaignText,true);
                if(candidate!=null)
                {
                    Func<string>? seedSource=null;
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
                    seedSource=testEndlessSeed;
#endif
                    experience=new MobileCandidateExperience(candidate,model,presenter,dispatcher!,clock!,lifetime.Token,seedSource);
                    if(storeConfiguration!=null)experience.ConfigureStore(storeConfiguration,storeReadiness);
                    presenter.BindHints(()=>model.PrepareHintAsync(candidate.Metagame,lifetime.Token),async receipt=>
                    {
                        var result=await model.CommitHintDisplayAsync(candidate.Metagame,receipt,lifetime.Token);
                        if(!result.Succeeded)throw new IOException("HINT_DISPLAY_NOT_COMMITTED");
                    },()=>candidate.Persisted.Snapshot==null||model.State.CurrentIdentity==null||model.PuzzleView?.Phase!=DevelopmentPreviewPhase.Editing?0:
                        candidate.Metagame.HintCredits(candidate.Persisted.Snapshot.Payload,model.State.CurrentIdentity));
                }
                await model.InitializeAsync(lifetime.Token);
                if (disposed || epoch != startupEpoch) return;
                if(experience!=null)await experience.EnsureReadyAsync();
                ApplySuspension(); presenter.Refresh();
            }
            catch (OperationCanceledException) { if (!disposed) ShowFailure(true); }
            catch (Exception error)
            {
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
                StartupDiagnostic=error.GetType().Name+":"+error.Message;
#endif
                if (disposed || epoch != startupEpoch) return;
                fault = true;
                presenter.ShowStartupFailure(error is IOException || error is UnauthorizedAccessException
                    ? SavedStartupFailure.LocalStorageUnavailable : error is ArgumentException
                    ? SavedStartupFailure.InvalidAssets : SavedStartupFailure.CompositionFailed);
            }
        }

        /// <summary>Uses version-bound explicit actions; only the application grants durable authority.</summary>
        public Task RunRequestAsync(SavedReviewUiRequest request)
        {
            if (disposed) return Task.FromException(new ObjectDisposedException(nameof(MobilePreviewInstaller)));
            if (!operation.IsCompleted || !lifecycle.IsCompleted) return Task.FromException(new InvalidOperationException("PREVIEW_BUSY"));
            if (request.Kind == SavedReviewUiRequestKind.RetryStartup && model == null)
            {
                string root = StorageRoot;
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
                if (testRoot != null) root = testRoot;
#endif
                InitializeTask = StartAsync(root, ++startupEpoch); return operation = InitializeTask;
            }
            if (model == null) return Task.FromException(new InvalidOperationException("PREVIEW_MODEL_MISSING"));
            return operation = ObserveRequestAsync(request);
        }
        private async Task ObserveRequestAsync(SavedReviewUiRequest request)
        {
            var current = model!;
            try
            {
                if (request.ExpectedStateVersion != current.State.Version) throw new InvalidOperationException("PREVIEW_STALE_REQUEST");
                switch (request.Kind)
                {
                    case SavedReviewUiRequestKind.CreateProfile: await current.CreateProfileAsync(lifetime.Token); break;
                    case SavedReviewUiRequestKind.OpenPuzzle: await current.SelectPuzzleAsync(RequiredIndex(request), false, lifetime.Token); break;
                    case SavedReviewUiRequestKind.ResumeDraft: await current.SelectPuzzleAsync(RequiredIndex(request), true, lifetime.Token); break;
                    case SavedReviewUiRequestKind.Restart: await current.RestartAsync(lifetime.Token); break;
                    case SavedReviewUiRequestKind.RetrySave: await current.RetrySaveAsync(lifetime.Token); break;
                    case SavedReviewUiRequestKind.RetryCompletion: await current.RetryCompletionAsync(lifetime.Token); break;
                    case SavedReviewUiRequestKind.Reload: await current.ReloadAsync(lifetime.Token); break;
                    case SavedReviewUiRequestKind.ConfirmRecovery:
                        await current.RecoverAndPromoteAsync(request.CandidateKind ?? throw new ArgumentException("PREVIEW_CANDIDATE_MISSING"), lifetime.Token); break;
                    case SavedReviewUiRequestKind.ConfirmReset: await current.ConfirmedResetAsync(lifetime.Token); break;
                    default: throw new ArgumentOutOfRangeException(nameof(request));
                }
                if (disposed) return;
                if (request.Kind == SavedReviewUiRequestKind.Reload) { fault = false; presenter!.ClearRequestFailure(); }
                if(experience!=null)await experience.EnsureReadyAsync();
                RefreshSaveGate(); presenter!.Refresh();
            }
            catch (OperationCanceledException) { if (!disposed) ShowFailure(true); }
            catch (Exception) { if (!disposed) ShowFailure(false); }
        }
        private static int RequiredIndex(SavedReviewUiRequest request) => request.PuzzleIndex ?? throw new ArgumentException("PREVIEW_PUZZLE_MISSING");
        private void Update()
        {
            if (disposed) return;
            dispatcher?.Drain(); ApplySafeArea(); presenter?.Refresh();
            model?.SetPresentationPaused(presenter?.IsPuzzleScreenActive!=true);ApplySuspension();
            if (model == null || presenter == null || fault) return;
            if (operation.IsCompleted && lifecycle.IsCompleted && !saveBlocked && !suspended)
            {
                if (model.State.Status == SavedReviewStatus.CompletionPending ||
                    (model.State.Status == SavedReviewStatus.Ready && model.State.IsDirty))
                    operation = ObserveAsync(() => model.TickAutosaveAsync(lifetime.Token));
            }
            UpdateRide();RefreshAudio(); presenter.Refresh();
        }
        private async Task ObserveAsync(Func<Task> action)
        {
            try { await action(); if (!disposed) RefreshSaveGate(); }
            catch (OperationCanceledException) { if (!disposed) ShowFailure(true); }
            catch (Exception) { if (!disposed) ShowFailure(false); }
        }
        private void RefreshSaveGate() => saveBlocked = model!.State.CommitStatus.HasValue && model.State.CommitStatus != SaveCommitStatus.Committed;
        private void ShowFailure(bool cancelled)
        { fault = true; saveBlocked = true; DisposeRide(); presenter?.ShowRequestFailure(cancelled); }
        private void UpdateRide()
        {
            var view = model!.PuzzleView;
            if (model.State.CanStartRide && view != null && ride == null && !suspended)
            {
                Guid attempt = view.AttemptId;
                experience?.CaptureCommittedResult();
                if (model.BeginTrainRide(attempt) == DevelopmentCommandStatus.Applied)
                {
                    try
                    {
                        ride = new ProductionTrainRide(view.BuiltPath, view.EndpointA, view.EndpointB, view.Width, view.Height);
                        rideAttempt = attempt; rideView = view;
                        gameAudio?.Play("CompletionClearance");gameAudio?.Play("TrainStart");
                        var motion=model.ConfirmedSnapshot?.Payload.Settings.Accessibility;
                        if(motion?.TrainRideSkip==true||motion?.ReducedMotion==true)ride.Tick(1000);
                    }
                    catch (Exception) { ShowFailure(false); return; }
                }
            }
            if (ride == null) return;
            if (view == null || !ReferenceEquals(view, rideView) || view.AttemptId != rideAttempt || view.Phase != DevelopmentPreviewPhase.TrainRide)
            { DisposeRide(); return; }
            if (!suspended) ride.Tick(Time.unscaledDeltaTime);
            presenter!.SetTrainPose(ride.X, ride.Y, ride.AngleDegrees, true);
            if (ride.IsFinished) { rideView!.CompleteTrainRide(rideAttempt);gameAudio?.Play("TrainArrive");gameAudio?.Play("ResultsOpen"); DisposeRide(); }
        }
        private void RefreshAudio()
        {
            var settings=model?.ConfirmedSnapshot?.Payload.Settings;
            if(settings!=null&&!ReferenceEquals(settings,appliedAudioSettings))
            {appliedAudioSettings=settings;gameAudio?.ApplySettings(settings);}
            gameAudio?.SetSuspended(suspended);
            gameAudio?.SetPresentationState(presenter?.IsPuzzleScreenActive==true,ride!=null);
            haptics?.SetState(settings?.HapticsEnabled==true,suspended);
        }
        private void DisposeRide()
        { ride?.Dispose(); ride = null; rideView = null; rideAttempt = Guid.Empty; presenter?.SetTrainPose(0, 0, 0, false); }
        private void ApplySafeArea()
        {
            if (Document == null || Screen.width <= 0 || Screen.height <= 0) return;
            Rect safe = Screen.safeArea;
            if (safe == lastSafeArea && lastScreenWidth == Screen.width && lastScreenHeight == Screen.height) return;
            var root = Document.rootVisualElement;
            float scale = root.resolvedStyle.width / Screen.width;
            if (!float.IsFinite(scale) || scale <= 0) return;
            lastSafeArea = safe; lastScreenWidth = Screen.width; lastScreenHeight = Screen.height;
            root.style.paddingLeft = safe.xMin * scale; root.style.paddingRight = (Screen.width - safe.xMax) * scale;
            root.style.paddingTop = (Screen.height - safe.yMax) * scale; root.style.paddingBottom = safe.yMin * scale;
        }
        /// <summary>Actual OR-combined suspension; captures puzzle time before the serialized flush.</summary>
        public void SetPaused(bool value) { paused = value; ApplySuspension(); }
        /// <summary>Focus never cancels an active pause signal.</summary>
        public void SetFocused(bool value) { focused = value; ApplySuspension(); }
        private void OnApplicationPause(bool value) => SetPaused(value);
        private void OnApplicationFocus(bool value) => SetFocused(value);
        private void OnApplicationQuit() => SetPaused(true);
        private string StorageRoot=>candidateBindings!=null?UnityEngine.Application.persistentDataPath:
            Path.Combine(UnityEngine.Application.persistentDataPath,"mobile-preview");
        private void ApplySuspension()
        {
            bool externalInput=paused||!focused||experience?.IsNativeUiBusy==true;
            experience?.SetExternalInputSuspended(externalInput);
            presenter?.SetAccessibilitySuspended(externalInput);
            bool next = externalInput || presenter?.IsHelpOpen==true || presenter?.IsProductModalOpen==true;
            if (disposed || model == null) { suspended = next; hasSuspension = false; return; }
            if (hasSuspension && next == suspended) return;
            suspended = next; hasSuspension = true;
            RefreshAudio();
            try
            {
                Task flush = model.SetSuspendedAsync(next, lifetime.Token);
                lifecycle = Task.WhenAll(lifecycle, ObserveAsync(() => flush));
            }
            catch (Exception) { ShowFailure(false); }
        }
        private void OnDestroy()
        {
            if (disposed) return;
            disposed = true; startupEpoch++; lifetime.Cancel();
            Task serviceWork=experience?.Pending??Task.CompletedTask;
            experience?.Dispose();DisposeRide(); presenter?.Dispose(); model?.Dispose(); dispatcher?.Dispose();
            if (output != null) UnityEngine.Object.Destroy(output.gameObject);
            if (Document != null) Document.rootVisualElement?.Clear();
            ShutdownTask = FinishShutdownAsync(Task.WhenAll(operation, lifecycle, InitializeTask,serviceWork));
        }
        private async Task FinishShutdownAsync(Task pending)
        { try { await pending.ConfigureAwait(false); } catch (Exception) { } finally { lifetime.Dispose(); } }
    }
}
