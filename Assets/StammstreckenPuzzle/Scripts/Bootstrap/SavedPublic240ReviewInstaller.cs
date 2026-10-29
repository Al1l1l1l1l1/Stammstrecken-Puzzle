using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;
using STP.Infrastructure.Persistence;
using STP.Platform;
using STP.Presentation.UI;
using STP.Presentation.World;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
#endif

namespace STP.Bootstrap
{
    /// <summary>Explicit scene-local ADR036 saved QA entry; all durable authority remains the unchanged S composition.</summary>
    [DisallowMultipleComponent]
    public sealed class SavedPublic240ReviewInstaller : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset? savedTree = null;
        [SerializeField] private StyleSheet? savedStyles = null;
        [SerializeField] private VisualTreeAsset? puzzleTree = null;
        [SerializeField] private StyleSheet? puzzleStyles = null;
        [SerializeField] private TextAsset? qaLocale = null;
        [SerializeField] private PanelSettings? panelSettings = null;
        [SerializeField] private Material? trainMaterial = null;
        [SerializeField] private TextAsset? publicData = null;
        [SerializeField] private TextAsset? campaignData = null;
        [SerializeField] private TextAsset? completionData = null;
        [SerializeField] private float rideDurationSeconds = 3f;
        private readonly CancellationTokenSource lifetime = new CancellationTokenSource();
        private LocalSaveClock? clock;
        private MainThreadSaveDispatcher? dispatcher;
        private SavedPublic240Composition? composition;
        private SavedPublic240ReviewSession? model;
        private SavedPublic240ReviewPresenter? presenter;
        private Camera? screenCamera;
        private DevelopmentTrainRide? ride;
        private IReviewPuzzleSession? rideView;
        private Guid rideAttempt;
        private Guid completionSubmittedFor;
        private Task operation = Task.CompletedTask;
        private Task lifecycleOperation = Task.CompletedTask;
        private Task initializeTask = Task.CompletedTask;
        private Task shutdownTask = Task.CompletedTask;
        private int startupEpoch;
        private bool applicationPaused;
        private bool hasFocus = true;
        private bool suspended;
        private bool suspensionApplied;
        private bool autoSaveBlocked;
        private bool requestFault;
        private bool disposed;
        private bool awakeStarted;
        private string privateRoot = "";
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
        private static string? testPrivateRoot;
        private static int testRootOwnerThread;
        private static int testLiveInstances;
        private bool capturedTestRoot;
#endif

        /// <summary>The real saved application bridge; absent until successful worker composition.</summary>
        public SavedPublic240ReviewSession Model => model ?? throw new InvalidOperationException("SAVED_MODEL_NOT_BOUND");
        /// <summary>Null-safe startup projection without manufacturing a puzzle session.</summary>
        public SavedPublic240ReviewSession? ModelOrNull => model;
        /// <summary>The explicit worker result, including the real isolated repository for contract diagnostics.</summary>
        public SavedPublic240Composition Composition => composition ?? throw new InvalidOperationException("SAVED_COMPOSITION_NOT_BOUND");
        /// <summary>The scene-owned saved presenter, which exists before the bridge.</summary>
        public SavedPublic240ReviewPresenter Presenter => presenter ?? throw new InvalidOperationException("SAVED_PRESENTER_NOT_BOUND");
        /// <summary>The scene-owned runtime UI document.</summary>
        public UIDocument Document { get; private set; } = null!;
        /// <summary>The actual player-path ride, absent until this attempt's real verified commit.</summary>
        public DevelopmentTrainRide? Ride => ride;
        /// <summary>Explicit output camera for the overlay, independently of the offscreen train camera.</summary>
        public Camera ScreenCamera => screenCamera ?? throw new InvalidOperationException("SAVED_SCREEN_NOT_BOUND");
        /// <summary>Current process-local monotonic instant.</summary>
        public long NowMilliseconds => clock?.MonotonicMilliseconds ?? 0;
        /// <summary>All currently observed user and lifecycle work; tests wait by frames and continue pumping.</summary>
        public Task PendingOperation => Task.WhenAll(operation, lifecycleOperation);
        /// <summary>The actual initial composition and fresh load, never an initialization write.</summary>
        public Task InitializeTask => initializeTask;
        /// <summary>Completes after all owned asynchronous work has terminated, including after OnDestroy.</summary>
        public Task ShutdownTask => shutdownTask;
        /// <summary>Actual OR-combined application suspension.</summary>
        public bool IsSuspended => suspended;

        /// <summary>Rejects every profile/identifier except the existing explicit internal QA identity.</summary>
        public static void ValidateQaProfile(string profile, string identifier) => Public240ReviewInstaller.ValidateQaProfile(profile, identifier);

        /// <summary>Validates only the explicitly injected visual references, without creating files or a fake model.</summary>
        public static void ValidateReferences(VisualTreeAsset? frame, StyleSheet? frameStyles,
            VisualTreeAsset? coreTree, StyleSheet? coreStyles, TextAsset? locale, PanelSettings? panel, Material? material)
        {
            Public240ReviewInstaller.ValidateReferences(coreTree, coreStyles, locale, panel, material);
            if (frame == null || frameStyles == null) throw new InvalidOperationException("SAVED_FRAME_ASSETS_REQUIRED");
            SavedPublic240ReviewPresenter.ValidateLocale(locale!);
        }

        /// <summary>Copies TextAsset sources on main, then performs read-only catalog validation on a worker for editor/build preflight.</summary>
        public static void ValidateCatalogData(TextAsset data, TextAsset campaign, TextAsset completion)
        {
            if (data == null || campaign == null || completion == null) throw new InvalidOperationException("SAVED_CATALOG_ASSETS_REQUIRED");
            byte[] p = data.bytes, c = campaign.bytes, v = completion.bytes;
            Task.Run(() => SavedPublic240Composition.ValidateCatalogBytes(p, c, v)).GetAwaiter().GetResult();
        }

        /// <summary>Read-only build guard over this exact installer's serialized references and QA identity.</summary>
        public void ValidateConfiguredReferences()
        {
            var configuration = new BootstrapConfiguration();
            ValidateQaProfile(configuration.Profile, configuration.ApplicationIdentifier);
            ValidateReferences(savedTree, savedStyles, puzzleTree, puzzleStyles, qaLocale, panelSettings, trainMaterial);
            ValidateDuration(rideDurationSeconds);
            ValidateCatalogData(publicData!, campaignData!, completionData!);
        }

        /// <summary>Configures an inactive scene/test instance through the same explicit references as the deterministic builder.</summary>
        public void ConfigureForQa(VisualTreeAsset frame, StyleSheet frameStyles, VisualTreeAsset coreTree,
            StyleSheet coreStyles, TextAsset locale, PanelSettings panel, Material material,
            TextAsset data, TextAsset campaign, TextAsset completion, float durationSeconds = 3f)
        {
            if (awakeStarted || disposed) throw new InvalidOperationException("SAVED_ALREADY_STARTED");
            ValidateReferences(frame, frameStyles, coreTree, coreStyles, locale, panel, material);
            ValidateDuration(durationSeconds);
            savedTree = frame; savedStyles = frameStyles; puzzleTree = coreTree; puzzleStyles = coreStyles;
            qaLocale = locale; panelSettings = panel; trainMaterial = material;
            publicData = data ?? throw new ArgumentNullException(nameof(data));
            campaignData = campaign ?? throw new ArgumentNullException(nameof(campaign));
            completionData = completion ?? throw new ArgumentNullException(nameof(completion));
            rideDurationSeconds = durationSeconds;
        }

        private static void ValidateDuration(float duration)
        {
            if (float.IsNaN(duration) || float.IsInfinity(duration) || duration <= 0)
                throw new ArgumentOutOfRangeException(nameof(duration));
        }

        private void Awake()
        {
            awakeStarted = true;
            ValidateReferences(savedTree, savedStyles, puzzleTree, puzzleStyles, qaLocale, panelSettings, trainMaterial);
            ValidateDuration(rideDurationSeconds);
            try
            {
                clock = new LocalSaveClock();
                dispatcher = new MainThreadSaveDispatcher();
                privateRoot = CapturePrivateRoot();
                Document = gameObject.AddComponent<UIDocument>();
                Document.panelSettings = panelSettings;
                savedTree!.CloneTree(Document.rootVisualElement);
                Document.rootVisualElement.styleSheets.Add(puzzleStyles!);
                Document.rootVisualElement.styleSheets.Add(savedStyles!);
                presenter = new SavedPublic240ReviewPresenter(Document.rootVisualElement, puzzleTree!, qaLocale!,
                    () => NowMilliseconds, RunRequestAsync);
                CreateScreenOutput();
                initializeTask = StartStartupAsync(++startupEpoch);
                operation = initializeTask;
            }
            catch
            {
                DisposeComposition();
                enabled = false;
                throw;
            }
        }

        private async Task StartStartupAsync(int epoch)
        {
            requestFault = false; autoSaveBlocked = false;
            presenter!.ShowStartupLoading();
            try
            {
                var configuration = new BootstrapConfiguration();
                try { ValidateQaProfile(configuration.Profile, UnityEngine.Application.isEditor ? configuration.ApplicationIdentifier : UnityEngine.Application.identifier); }
                catch (Exception) { presenter.ShowStartupFailure(SavedStartupFailure.ProfileMismatch); return; }
                if (publicData == null || campaignData == null || completionData == null)
                { presenter.ShowStartupFailure(SavedStartupFailure.InvalidAssets); return; }
                byte[] p = publicData.bytes, c = campaignData.bytes, v = completionData.bytes;
                string version = UnityEngine.Application.version;
                string build = UnityEngine.Application.isEditor ? "editor-public240-review" : UnityEngine.Application.buildGUID;
                var built = await SavedPublic240Composition.CreateAsync(p, c, v, privateRoot, clock!, dispatcher!, version, build, lifetime.Token);
                if (disposed || epoch != startupEpoch) return;
                composition = built;
                // This constructor and BindModel deliberately occur AFTER the worker await on the Unity main context.
                model = new SavedPublic240ReviewSession(built.Persisted, built.Autosave, built.Catalog, clock!, dispatcher!);
                suspensionApplied = false;
                presenter.BindModel(model);
                await model.InitializeAsync(lifetime.Token);
                if (disposed || epoch != startupEpoch) return;
                ApplySuspension();
                presenter.Refresh();
            }
            catch (OperationCanceledException)
            {
                if (!disposed && epoch == startupEpoch) ShowObservedFailure(true, model == null);
            }
            catch (Exception error)
            {
                if (disposed || epoch != startupEpoch) return;
                if (model != null) ShowObservedFailure(false, false);
                else presenter.ShowStartupFailure(error is IOException || error is UnauthorizedAccessException
                    ? SavedStartupFailure.LocalStorageUnavailable : error is ArgumentException
                    ? SavedStartupFailure.InvalidAssets : SavedStartupFailure.CompositionFailed);
            }
        }

        /// <summary>One observed explicit saved action; the bridge checks its own current immutable gates again.</summary>
        public Task RunRequestAsync(SavedReviewUiRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (disposed) return Task.FromException(new ObjectDisposedException(nameof(SavedPublic240ReviewInstaller)));
            if (!operation.IsCompleted || !lifecycleOperation.IsCompleted)
                return Task.FromException(new InvalidOperationException("SAVED_REQUEST_BUSY"));
            if (request.Kind == SavedReviewUiRequestKind.RetryStartup)
            {
                if (model != null) return Task.FromException(new InvalidOperationException("SAVED_STARTUP_ALREADY_BOUND"));
                initializeTask = StartStartupAsync(++startupEpoch); operation = initializeTask; return operation;
            }
            if (model == null) return Task.FromException(new InvalidOperationException("SAVED_MODEL_NOT_BOUND"));
            operation = RunBoundRequestObservedAsync(request);
            return operation;
        }

        private async Task RunBoundRequestObservedAsync(SavedReviewUiRequest request)
        {
            var current = model!;
            long before = current.State.Version;
            try
            {
                if (!request.ExpectedStateVersion.HasValue || request.ExpectedStateVersion.Value != before)
                    throw new InvalidOperationException("SAVED_STALE_UI_REQUEST");
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
                        if (!request.CandidateKind.HasValue) throw new ArgumentException("SAVED_CANDIDATE_REQUIRED");
                        await current.RecoverAndPromoteAsync(request.CandidateKind.Value, lifetime.Token); break;
                    case SavedReviewUiRequestKind.ConfirmReset: await current.ConfirmedResetAsync(lifetime.Token); break;
                    default: throw new ArgumentOutOfRangeException(nameof(request));
                }
                if (disposed || !ReferenceEquals(current, model)) return;
                if (request.Kind == SavedReviewUiRequestKind.Reload && current.State.Version > before)
                { requestFault = false; presenter!.ClearRequestFailure(); }
                RefreshAutomaticSaveGate();
                presenter!.Refresh();
            }
            catch (OperationCanceledException) { if (!disposed) ShowObservedFailure(true, false); }
            catch (Exception) { if (!disposed) ShowObservedFailure(false, false); }
        }

        private static int RequiredIndex(SavedReviewUiRequest request) => request.PuzzleIndex ?? throw new ArgumentException("SAVED_PUZZLE_INDEX_REQUIRED");

        private void Update()
        {
            if (disposed) return;
            // Null-model loading must still drain worker publications and refresh the localized startup frame.
            dispatcher?.Drain();
            presenter?.Refresh();
            if (model == null || presenter == null) return;
            if (requestFault) { DisposeRide(); return; }
            if (operation.IsCompleted && lifecycleOperation.IsCompleted)
            {
                var view = model.PuzzleView;
                if (model.State.Status == SavedReviewStatus.CompletionPending && view != null && !suspended && !autoSaveBlocked && completionSubmittedFor != view.AttemptId)
                {
                    completionSubmittedFor = view.AttemptId;
                    operation = ObserveAutomaticAsync(() => model.TickAutosaveAsync(lifetime.Token));
                }
                else if (model.State.Status == SavedReviewStatus.Ready && model.State.IsDirty && !autoSaveBlocked)
                    operation = ObserveAutomaticAsync(() => model.TickAutosaveAsync(lifetime.Token));
            }
            UpdateRide();
            presenter.Refresh();
        }

        private async Task ObserveAutomaticAsync(Func<Task> action)
        {
            try { await action(); if (!disposed) RefreshAutomaticSaveGate(); }
            catch (OperationCanceledException) { if (!disposed) ShowObservedFailure(true, false); }
            catch (Exception) { if (!disposed) ShowObservedFailure(false, false); }
        }

        private void RefreshAutomaticSaveGate()
        {
            var status = model!.State.CommitStatus;
            autoSaveBlocked = status.HasValue && status.Value != SaveCommitStatus.Committed;
        }

        private void ShowObservedFailure(bool cancelled, bool startup)
        {
            requestFault = true; autoSaveBlocked = true;
            DisposeRide();
            if (startup) presenter?.ShowStartupFailure(SavedStartupFailure.CompositionFailed);
            else presenter?.ShowRequestFailure(cancelled);
        }

        private void UpdateRide()
        {
            var view = model!.PuzzleView;
            if (model.State.CanStartRide && view != null && ride == null && !suspended)
            {
                Guid attempt = view.AttemptId;
                if (model.BeginTrainRide(attempt) == DevelopmentCommandStatus.Applied)
                {
                    try
                    {
                        rideAttempt = attempt; rideView = view;
                        ride = new DevelopmentTrainRide(view.BuiltPath, view.EndpointA, view.EndpointB,
                            view.Width, view.Height, trainMaterial!, rideDurationSeconds);
                        presenter!.SetTrainTexture(ride.Texture);
                    }
                    catch (Exception) { ShowObservedFailure(false, false); return; }
                }
            }
            if (ride == null) return;
            if (view == null || !ReferenceEquals(view, rideView) || view.AttemptId != rideAttempt || view.Phase != DevelopmentPreviewPhase.TrainRide)
            { DisposeRide(); return; }
            if (!suspended) ride.Tick(Time.unscaledDeltaTime);
            if (ride.IsFinished)
            {
                rideView!.CompleteTrainRide(rideAttempt);
                DisposeRide();
            }
        }

        /// <summary>Applies the actual Unity pause signal immediately; observed flush may await the existing serial writer.</summary>
        public void SetApplicationPaused(bool paused) { applicationPaused = paused; ApplySuspension(); }
        /// <summary>Combines focus with pause by OR; regaining focus alone cannot resume a paused application.</summary>
        public void SetApplicationFocus(bool focused) { hasFocus = focused; ApplySuspension(); }
        private void OnApplicationPause(bool paused) => SetApplicationPaused(paused);
        private void OnApplicationFocus(bool focused) => SetApplicationFocus(focused);
        private void OnApplicationQuit() => SetApplicationPaused(true);

        private void ApplySuspension()
        {
            bool next = applicationPaused || !hasFocus;
            if (disposed || model == null) { suspended = next; suspensionApplied = false; return; }
            if (next == suspended && suspensionApplied) return;
            suspended = next; suspensionApplied = true;
            // SetSuspendedAsync captures domain time synchronously on main before awaiting its flush guard.
            Task requested;
            try { requested = model.SetSuspendedAsync(next, lifetime.Token); }
            catch (Exception) { ShowObservedFailure(false, false); return; }
            lifecycleOperation = Task.WhenAll(lifecycleOperation, ObserveAutomaticAsync(() => requested));
        }

        private void CreateScreenOutput()
        {
            var output = new GameObject("SavedPublic240 explicit QA screen output");
            output.transform.SetParent(transform, false); output.transform.localPosition = new Vector3(0, 0, -10f);
            screenCamera = output.AddComponent<Camera>();
            screenCamera.orthographic = true; screenCamera.cullingMask = 0;
            screenCamera.clearFlags = CameraClearFlags.SolidColor;
            screenCamera.backgroundColor = new Color(240f / 255f, 241f / 255f, 237f / 255f, 1f);
            screenCamera.targetTexture = null; screenCamera.depth = 100f;
            screenCamera.nearClipPlane = .1f; screenCamera.farClipPlane = 30f;
            screenCamera.allowHDR = false; screenCamera.allowMSAA = false;
            screenCamera.enabled = SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null;
        }

        private void DisposeRide()
        {
            if (ride != null) { presenter?.SetTrainTexture(null); ride.Dispose(); }
            ride = null; rideView = null; rideAttempt = Guid.Empty;
        }

        private void OnDestroy() => DisposeComposition();
        private void DisposeComposition()
        {
            if (disposed) return;
            disposed = true; startupEpoch++; lifetime.Cancel();
            DisposeRide(); presenter?.Dispose(); model?.Dispose(); dispatcher?.Dispose();
            if (screenCamera != null) UnityEngine.Object.Destroy(screenCamera.gameObject);
            if (Document != null) { Document.rootVisualElement?.Clear(); UnityEngine.Object.Destroy(Document); }
            screenCamera = null; presenter = null;
            shutdownTask = FinishShutdownAsync(Task.WhenAll(operation, lifecycleOperation, initializeTask));
        }

        private async Task FinishShutdownAsync(Task pending)
        {
            try { await pending.ConfigureAwait(false); } catch (Exception) { /* Every late task is observed; no success or rollback is inferred. */ }
            finally
            {
                lifetime.Dispose();
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
                if (capturedTestRoot) { capturedTestRoot = false; Interlocked.Decrement(ref testLiveInstances); }
#endif
            }
        }

        private string CapturePrivateRoot()
        {
#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
            if (testPrivateRoot != null)
            {
                if (Thread.CurrentThread.ManagedThreadId != testRootOwnerThread) throw new InvalidOperationException("SAVED_TEST_ROOT_WRONG_THREAD");
                capturedTestRoot = true; Interlocked.Increment(ref testLiveInstances); return testPrivateRoot;
            }
#endif
            return UnityEngine.Application.persistentDataPath;
        }

#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
        /// <summary>Single editor-test-only root override, captured before Awake and strictly disjoint from the real AppData tree.</summary>
        public static IDisposable UseTestPrivateRoot(string root)
        {
            if (testPrivateRoot != null) throw new InvalidOperationException("SAVED_TEST_ROOT_SCOPE_NESTED");
            if (root == null || !Path.IsPathRooted(root)) throw new ArgumentException("SAVED_TEST_ROOT_NOT_ABSOLUTE");
            string realRoot = UnityEngine.Application.persistentDataPath, tempRoot = Path.GetTempPath();
            string? configured = Environment.GetEnvironmentVariable("STP_TEST_TEMP_ROOT");
            // Setup-only, read-only physical identity queries precede every provisioning operation.
            // The normalized NT identity is NEVER used as the SaveFS root.
            string full = Task.Run(() => ValidateTestRootBoundary(root, realRoot, tempRoot, configured)).GetAwaiter().GetResult();
            Task.Run(() => SaveFileSystem.ProvisionPrivateRoot(full)).GetAwaiter().GetResult();
            testRootOwnerThread = Thread.CurrentThread.ManagedThreadId; testPrivateRoot = full;
            return new TestRootLease(full, testRootOwnerThread);
        }

        private static string CanonicalDirectory(string path)
        {
            string full = Path.GetFullPath(path), trimmed = full.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string volume = Path.GetPathRoot(full)!;
            return trimmed.Length < volume.Length ? volume : trimmed;
        }

        internal static string ValidateTestRootBoundary(string root, string realRoot, string tempRoot, string? configured)
        {
            string full = CheckedTestDirectory(root), real = CheckedTestDirectory(realRoot), temp = CheckedTestDirectory(tempRoot);
            string? environmentRoot = string.IsNullOrWhiteSpace(configured) ? null : CheckedTestDirectory(configured!);
            string physical = ComparisonDirectory(full), physicalReal = ComparisonDirectory(real), physicalTemp = ComparisonDirectory(temp);
            if (SameOrDescendant(physical, physicalReal) || SameOrDescendant(physicalReal, physical))
                throw new ArgumentException("SAVED_TEST_ROOT_TOUCHES_REAL_PROFILE");
            bool allowed = IsStrictDescendant(physical, physicalTemp);
            if (environmentRoot != null) allowed |= IsStrictDescendant(physical, ComparisonDirectory(environmentRoot));
            if (!allowed) throw new ArgumentException("SAVED_TEST_ROOT_OUTSIDE_TEMP");
            return full;
        }

        private static string CheckedTestDirectory(string path)
        {
            if (!Path.IsPathRooted(path)) throw new ArgumentException("SAVED_TEST_ROOT_NOT_ABSOLUTE");
            if (Path.DirectorySeparatorChar == '\\')
            {
                // Only ordinary fully qualified DOS drive paths. Namespaces, ADS and Win32-trimmed components are ambiguous.
                if (path.Length < 3 || !char.IsLetter(path[0]) || path[1] != ':' || (path[2] != '\\' && path[2] != '/'))
                    throw new ArgumentException("SAVED_TEST_WINDOWS_NAMESPACE_REJECTED");
                foreach (string component in path.Substring(3).Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries))
                    if (component.EndsWith(".", StringComparison.Ordinal) || component.EndsWith(" ", StringComparison.Ordinal) || component.Contains(":", StringComparison.Ordinal))
                        throw new ArgumentException("SAVED_TEST_WINDOWS_AMBIGUOUS_COMPONENT");
            }
            return CanonicalDirectory(path);
        }

        private static string ComparisonDirectory(string path)
        {
            if (Path.DirectorySeparatorChar == '\\') return WindowsTestPathIdentity.ResolveExistingDirectoryPrefix(path);
            // Unix test bounds are accepted only without links in ANY name, including the protected
            // real-root name. SaveFS cannot discover an alias in a different, never-passed root name.
            for (string? current = path; current != null; current = Path.GetDirectoryName(current))
            {
                try
                {
                    var attributes = File.GetAttributes(current);
                    if ((attributes & FileAttributes.ReparsePoint) != 0)
                        throw new ArgumentException("SAVED_TEST_UNIX_REPARSE_BOUNDARY_REJECTED");
                    if ((attributes & FileAttributes.Directory) == 0)
                        throw new IOException("SAVED_TEST_BOUNDARY_NOT_DIRECTORY");
                }
                catch (FileNotFoundException) { }
                catch (DirectoryNotFoundException) { }
                if (Path.GetDirectoryName(current) == current) break;
            }
            return path;
        }

        private static class WindowsTestPathIdentity
        {
            [StructLayout(LayoutKind.Sequential)]
            private struct AttributeTag { internal uint Attributes; internal uint Tag; }
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
            private static extern IntPtr CreateFileW(string name, uint access, uint share, IntPtr security, uint creation, uint flags, IntPtr template);
            [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool GetFileInformationByHandleEx(IntPtr handle, int informationClass, out AttributeTag attributes, uint bytes);
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
            private static extern uint GetFinalPathNameByHandleW(IntPtr handle, StringBuilder path, uint capacity, uint flags);
            [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool CloseHandle(IntPtr handle);

            internal static string ResolveExistingDirectoryPrefix(string path)
            {
                string current = path;
                var suffix = new List<string>();
                while (true)
                {
                    // access0: metadata only; OPEN_EXISTING never creates a directory or opens a save payload.
                    IntPtr handle = CreateFileW(current, 0, 7, IntPtr.Zero, 3, 0x02000000, IntPtr.Zero);
                    if (handle == new IntPtr(-1) || handle == IntPtr.Zero)
                    {
                        int error = Marshal.GetLastWin32Error();
                        if (error != 2 && error != 3) throw new IOException("SAVED_TEST_PATH_IDENTITY_OPEN_" + error);
                        string? parent = Path.GetDirectoryName(current);
                        if (string.IsNullOrEmpty(parent) || parent == current) throw new IOException("SAVED_TEST_PATH_IDENTITY_ROOT_MISSING");
                        suffix.Add(Path.GetFileName(current)); current = parent; continue;
                    }
                    try
                    {
                        if (!GetFileInformationByHandleEx(handle, 9, out var attributes, 8))
                            throw new IOException("SAVED_TEST_PATH_ATTRIBUTES_" + Marshal.GetLastWin32Error());
                        if ((attributes.Attributes & 0x10) == 0) throw new IOException("SAVED_TEST_PATH_PREFIX_NOT_DIRECTORY");
                        int capacity = 512;
                        while (capacity <= 32768)
                        {
                            var value = new StringBuilder(capacity);
                            uint count = GetFinalPathNameByHandleW(handle, value, (uint)capacity, 2);
                            if (count == 0) throw new IOException("SAVED_TEST_PATH_IDENTITY_QUERY_" + Marshal.GetLastWin32Error());
                            if (count >= capacity) { if (count >= 32768) break; capacity = checked((int)count + 1); continue; }
                            string normalized = value.ToString();
                            if (!normalized.StartsWith(@"\Device\", StringComparison.OrdinalIgnoreCase) || normalized.Length != count)
                                throw new IOException("SAVED_TEST_PATH_IDENTITY_INVALID");
                            normalized = normalized.TrimEnd('\\');
                            for (int i = suffix.Count - 1; i >= 0; i--) normalized += "\\" + suffix[i];
                            return normalized;
                        }
                        throw new IOException("SAVED_TEST_PATH_IDENTITY_BUFFER_LIMIT");
                    }
                    finally { if (!CloseHandle(handle)) throw new IOException("SAVED_TEST_PATH_IDENTITY_CLOSE_" + Marshal.GetLastWin32Error()); }
                }
            }
        }
        private static bool IsStrictDescendant(string path, string parent) => path.StartsWith(parent.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar,
            Path.DirectorySeparatorChar == '\\' ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        private static bool SameOrDescendant(string path, string parent) => string.Equals(path, parent,
            Path.DirectorySeparatorChar == '\\' ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) || IsStrictDescendant(path, parent);

        private sealed class TestRootLease : IDisposable
        {
            private readonly string root;
            private readonly int thread;
            private bool released;
            internal TestRootLease(string root, int thread) { this.root = root; this.thread = thread; }
            public void Dispose()
            {
                if (released) return;
                if (Thread.CurrentThread.ManagedThreadId != thread || testPrivateRoot != root)
                    throw new InvalidOperationException("SAVED_TEST_ROOT_WRONG_OWNER");
                if (Volatile.Read(ref testLiveInstances) != 0) throw new InvalidOperationException("SAVED_TEST_ROOT_INSTANCE_STILL_RUNNING");
                testPrivateRoot = null; testRootOwnerThread = 0; released = true;
            }
        }
#endif
    }
}
