using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;
using UnityEngine;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>Startup lifetime only; it contains no save or attempt truth.</summary>
    public enum SavedStartupPhase { Loading, Failed, Bound, Disposed }
    /// <summary>Closed localized failures before an Application bridge exists.</summary>
    public enum SavedStartupFailure { InvalidAssets, ProfileMismatch, LocalStorageUnavailable, CompositionFailed }
    /// <summary>Explicit user intents, interpreted by the Bootstrap task owner.</summary>
    public enum SavedReviewUiRequestKind
    {
        RetryStartup, CreateProfile, OpenPuzzle, ResumeDraft, Restart, RetrySave,
        RetryCompletion, Reload, ConfirmRecovery, ConfirmReset
    }

    /// <summary>Immutable UI intent; never a repository observation or save acknowledgement.</summary>
    public sealed class SavedReviewUiRequest
    {
        private SavedReviewUiRequest(SavedReviewUiRequestKind kind, long? version, int? index, SaveCandidateKind? candidate)
        { Kind = kind; ExpectedStateVersion = version; PuzzleIndex = index; CandidateKind = candidate; }
        /// <summary>Closed explicit action.</summary>
        public SavedReviewUiRequestKind Kind { get; }
        /// <summary>Application version actually shown when the user acted; absent only before binding.</summary>
        public long? ExpectedStateVersion { get; }
        /// <summary>Captured proposed catalog index for OpenPuzzle or ResumeDraft.</summary>
        public int? PuzzleIndex { get; }
        /// <summary>Explicit valid candidate choice; full provenance stays inside Application.</summary>
        public SaveCandidateKind? CandidateKind { get; }
        /// <summary>Creates the only model-free action.</summary>
        public static SavedReviewUiRequest StartupRetry() => new SavedReviewUiRequest(SavedReviewUiRequestKind.RetryStartup, null, null, null);
        /// <summary>Creates a bound action with no puzzle or candidate argument.</summary>
        public static SavedReviewUiRequest Bound(SavedReviewUiRequestKind kind, long version)
        {
            if (!Enum.IsDefined(typeof(SavedReviewUiRequestKind), kind) || kind == SavedReviewUiRequestKind.RetryStartup
                || kind == SavedReviewUiRequestKind.OpenPuzzle || kind == SavedReviewUiRequestKind.ResumeDraft
                || kind == SavedReviewUiRequestKind.ConfirmRecovery) throw new ArgumentOutOfRangeException(nameof(kind));
            ValidateVersion(version);
            return new SavedReviewUiRequest(kind, version, null, null);
        }
        /// <summary>Creates an explicit new attempt or restore request, never a dropdown-change request.</summary>
        public static SavedReviewUiRequest Puzzle(SavedReviewUiRequestKind kind, long version, int index)
        {
            if (kind != SavedReviewUiRequestKind.OpenPuzzle && kind != SavedReviewUiRequestKind.ResumeDraft) throw new ArgumentOutOfRangeException(nameof(kind));
            if (index < 0 || index >= 240) throw new ArgumentOutOfRangeException(nameof(index));
            ValidateVersion(version);
            return new SavedReviewUiRequest(kind, version, index, null);
        }
        /// <summary>Creates a deliberate candidate promotion request bound to the displayed version.</summary>
        public static SavedReviewUiRequest Recovery(long version, SaveCandidateKind candidate)
        {
            if (!Enum.IsDefined(typeof(SaveCandidateKind), candidate)) throw new ArgumentOutOfRangeException(nameof(candidate));
            ValidateVersion(version);
            return new SavedReviewUiRequest(SavedReviewUiRequestKind.ConfirmRecovery, version, null, candidate);
        }
        private static void ValidateVersion(long version) { if (version < 0) throw new ArgumentOutOfRangeException(nameof(version)); }
    }

    /// <summary>Saved QA frame around the unchanged public grid, with explicit observed asynchronous actions.</summary>
    public sealed class SavedPublic240ReviewPresenter : IDisposable
    {
        private const string Prefix = "qa.public240.saved.";
        private readonly int ownerThread = Thread.CurrentThread.ManagedThreadId;
        private readonly VisualTreeAsset puzzleTree;
        private readonly TextAsset localeSource;
        private readonly Public240ReviewLocalization locale;
        private readonly Func<long> clock;
        private readonly Func<SavedReviewUiRequest, Task> runRequest;
        private readonly VisualElement gameHost;
        private readonly VisualElement candidates;
        private readonly VisualElement confirmation;
        private readonly Label status;
        private readonly Label error;
        private readonly Label progress;
        private readonly Label stars;
        private readonly Label current;
        private readonly Label confirmText;
        private readonly DropdownField picker;
        private readonly Dictionary<string, Button> buttons = new Dictionary<string, Button>(StringComparer.Ordinal);
        private readonly Dictionary<string, Action> actions = new Dictionary<string, Action>(StringComparer.Ordinal);
        private SavedPublic240ReviewSession? model;
        private IReviewPuzzleSession? boundView;
        private Task? request;
        private SavedReviewUiRequestKind requestKind;
        private bool requestFault;
        private bool requestCancelled;
        private long candidateVersion = -1;
        private long confirmationVersion = -1;
        private SaveCandidateKind? selectedCandidate;
        private bool resetConfirmation;
        private bool disposed;

        /// <summary>Creates a localized visible loading frame without any catalog, bridge or fake session.</summary>
        public SavedPublic240ReviewPresenter(VisualElement savedRoot, VisualTreeAsset puzzleTree, TextAsset savedLocale,
            Func<long> monotonicMilliseconds, Func<SavedReviewUiRequest, Task> runRequest)
        {
            Root = savedRoot ?? throw new ArgumentNullException(nameof(savedRoot));
            this.puzzleTree = puzzleTree ?? throw new ArgumentNullException(nameof(puzzleTree));
            localeSource = savedLocale ?? throw new ArgumentNullException(nameof(savedLocale));
            clock = monotonicMilliseconds ?? throw new ArgumentNullException(nameof(monotonicMilliseconds));
            this.runRequest = runRequest ?? throw new ArgumentNullException(nameof(runRequest));
            locale = new Public240ReviewLocalization(savedLocale);
            try
            {
                ValidateLocaleTable(locale);
                gameHost = Require<VisualElement>("saved-game-host");
                candidates = Require<VisualElement>("saved-candidates");
                confirmation = Require<VisualElement>("saved-confirmation");
                status = Require<Label>("saved-status"); error = Require<Label>("saved-error");
                progress = Require<Label>("saved-progress"); current = Require<Label>("saved-current");
                stars = Require<Label>("saved-stars");
                confirmText = Require<Label>("saved-confirm-text");
                picker = Require<DropdownField>("saved-picker");
                picker.label = L("picker.label");
                picker.RegisterValueChangedCallback(OnPickerChanged);
                Require<Label>("saved-boundary").text = L("boundary");
                AddButton("saved-startup-retry", "startup.retry", () => Send(SavedReviewUiRequest.StartupRetry()));
                AddButton("saved-create", "create", () => SendBound(SavedReviewUiRequestKind.CreateProfile));
                AddButton("saved-open", "open", () => SendPuzzle(false));
                AddButton("saved-resume", "resume", () => SendPuzzle(true));
                AddButton("saved-restart", "restart", () => SendBound(SavedReviewUiRequestKind.Restart));
                AddButton("saved-retry-save", "retry.save", () => SendBound(SavedReviewUiRequestKind.RetrySave));
                AddButton("saved-retry-completion", "retry.completion", () => SendBound(SavedReviewUiRequestKind.RetryCompletion));
                AddButton("saved-reload", "reload", () => SendBound(SavedReviewUiRequestKind.Reload));
                AddButton("saved-recover", "recover", ConfirmRecovery);
                AddButton("saved-reset", "reset", ShowResetConfirmation);
                AddButton("saved-confirm", "confirm", Confirm);
                AddButton("saved-cancel", "cancel", ClearConfirmation);
                ShowStartupLoading();
            }
            catch
            {
                Unsubscribe(); locale.Dispose(); throw;
            }
        }

        /// <summary>Explicitly injected root; no runtime scene lookup.</summary>
        public VisualElement Root { get; }
        /// <summary>True after the sole successful bridge binding.</summary>
        public bool HasBoundModel => model != null;
        /// <summary>Model-free startup lifetime only.</summary>
        public SavedStartupPhase StartupPhase { get; private set; }
        /// <summary>Unchanged core, absent until an actual session exists.</summary>
        public Public240ReviewPresenter? Core { get; private set; }
        /// <summary>Pending observed UI request, independently of the Root task owner.</summary>
        public Task? PendingRequest => request;
        /// <summary>Ephemeral proposed index, distinct from Application.State.SelectedIndex.</summary>
        public int ProposedIndex => picker.index;
        /// <summary>Conservative local failure latch; cannot grant a save or gameplay success.</summary>
        public bool HasRequestFault => requestFault;

        /// <summary>Checks every closed saved/core key and its typed format contract before any runtime composition.</summary>
        public static void ValidateLocale(TextAsset source)
        {
            using var table = new Public240ReviewLocalization(source); ValidateLocaleTable(table);
        }

        /// <summary>Reports an observed Root-owned request failure on Main; explicit reload remains accessible.</summary>
        public void ShowRequestFailure(bool cancelled)
        {
            CheckThread(); if (disposed) return;
            requestFault = true; requestCancelled = cancelled; Refresh();
        }
        /// <summary>Clears only the UI latch; Root calls after an explicit fully answered fresh reload with a new version.</summary>
        public void ClearRequestFailure()
        {
            CheckThread(); if (disposed) return;
            requestFault = false; requestCancelled = false; Refresh();
        }

        /// <summary>Shows localized loading before binding; never replaces a bound save state.</summary>
        public void ShowStartupLoading()
        {
            CheckStartup(); StartupPhase = SavedStartupPhase.Loading; status.text = L("startup.loading");
            RenderStartup();
        }
        /// <summary>Shows a closed composition error without exposing raw exceptions.</summary>
        public void ShowStartupFailure(SavedStartupFailure failure)
        {
            CheckStartup();
            if (!Enum.IsDefined(typeof(SavedStartupFailure), failure)) throw new ArgumentOutOfRangeException(nameof(failure));
            StartupPhase = SavedStartupPhase.Failed;
            var suffix = failure == SavedStartupFailure.InvalidAssets ? "assets" : failure == SavedStartupFailure.ProfileMismatch ? "profile"
                : failure == SavedStartupFailure.LocalStorageUnavailable ? "storage" : "composition";
            status.text = L("startup.failed." + suffix); RenderStartup();
        }
        /// <summary>Binds a real main-thread Application bridge exactly once.</summary>
        public void BindModel(SavedPublic240ReviewSession value)
        {
            CheckStartup();
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Puzzles.Count != 240) throw new ArgumentException("SAVED_PICKER_COUNT", nameof(value));
            var labels = new List<string>(240);
            foreach (var puzzle in value.Puzzles) labels.Add(locale.Resolve("qa.public240.picker.item", puzzle.Identity.PuzzleId, puzzle.Width, puzzle.Height));
            picker.choices = labels; picker.index = 0;
            model = value; StartupPhase = SavedStartupPhase.Bound; requestFault = false; requestCancelled = false;
            Refresh();
        }

        /// <summary>Processes observed task outcomes and renders only the current immutable App state.</summary>
        public void Refresh()
        {
            CheckThread(); if (disposed) return;
            PollRequest();
            if (model == null) { RenderStartup(); return; }
            var state = model.State;
            if (confirmationVersion >= 0 && confirmationVersion != state.Version) ClearConfirmation();
            var busy = request != null;
            bool clear = !busy && !requestFault;
            status.text = L("status." + state.Status);
            error.text = requestCancelled ? L("request.cancelled") : requestFault ? L("request.failed")
                : string.IsNullOrEmpty(state.ErrorCode) || state.ErrorCode == "NONE" ? string.Empty : locale.Resolve(Prefix + "error", state.ErrorCode);
            current.text = state.CurrentIdentity == null ? L("current.none") : locale.Resolve(Prefix + "current", state.CurrentIdentity.PuzzleId);
            var confirmed = state.Progress;
            progress.text = confirmed == null ? L("progress.none") : locale.Resolve(Prefix + "progress", confirmed.Generation,
                confirmed.BalancePatience, confirmed.FirstClearCount, confirmed.MemberCount);
            stars.text = confirmed?.SelectedIdentity == null || !confirmed.SelectedStars.HasValue ? L("stars.none")
                : locale.Resolve(Prefix + "stars", confirmed.SelectedIdentity.PuzzleId, confirmed.SelectedStars.Value);
            SetButton("saved-startup-retry", false);
            SetButton("saved-create", clear && state.CanCreateProfile);
            picker.SetEnabled(clear && state.CanNavigate);
            SetButton("saved-open", clear && state.CanNavigate && picker.index >= 0);
            SetButton("saved-resume", clear && state.CanNavigate && picker.index >= 0 && model.HasDraftForPuzzle(picker.index));
            SetButton("saved-restart", clear && state.CanRestart);
            SetButton("saved-retry-save", clear && state.CanRetrySave);
            SetButton("saved-retry-completion", clear && state.CanRetrySave && state.Status == SavedReviewStatus.CompletionPending);
            // A request exception must not make the only explicit fresh observation unreachable.
            SetButton("saved-reload", !busy && (state.CanRetryLoad || requestFault));
            SetButton("saved-reset", clear && state.CanReset);
            RenderCandidates(state, clear);
            SetButton("saved-recover", clear && state.CanRecover && selectedCandidate.HasValue && candidateVersion == state.Version);
            confirmation.style.display = confirmationVersion >= 0 ? DisplayStyle.Flex : DisplayStyle.None;
            buttons["saved-confirm"].SetEnabled(clear && confirmationVersion == state.Version);
            buttons["saved-cancel"].SetEnabled(!busy);
            var view = model.PuzzleView;
            if (!ReferenceEquals(view, boundView))
            {
                if (view == null) { Core?.Dispose(); Core = null; boundView = null; gameHost.Clear(); }
                else if (Core == null)
                {
                    var tree = puzzleTree.CloneTree(); gameHost.Add(tree);
                    try { Core = new Public240ReviewPresenter(tree, view, localeSource, clock, model.Puzzles, _ => { }); boundView = view; }
                    catch { gameHost.Clear(); throw; }
                }
                else { Core.BindSession(view); boundView = view; }
            }
            if (Core != null)
            {
                Core.Refresh();
                Core.Root.Q<VisualElement>("qa-picker-bar").style.display = DisplayStyle.None;
                Core.Root.Q<Button>("qa-restart").style.display = DisplayStyle.None;
                Core.Root.Q<VisualElement>("qa-result").style.display = clear && state.Status == SavedReviewStatus.Results ? DisplayStyle.Flex : DisplayStyle.None;
            }
            gameHost.SetEnabled(clear && state.CanMutate);
            gameHost.style.display = view == null ? DisplayStyle.None : DisplayStyle.Flex;
        }

        /// <summary>Only projects the Root-owned actual ride texture.</summary>
        public void SetTrainTexture(Texture? texture) { CheckThread(); if (!disposed) Core?.SetTrainTexture(texture); }
        /// <summary>Unsubscribes and releases own locale/core; late task faults stay observed without touching Unity.</summary>
        public void Dispose()
        {
            CheckThread(); if (disposed) return; disposed = true; StartupPhase = SavedStartupPhase.Disposed;
            Unsubscribe(); Core?.Dispose(); Core = null; boundView = null; gameHost.Clear(); locale.Dispose();
        }

        private string L(string suffix) => locale.Resolve(Prefix + suffix);
        private T Require<T>(string id) where T : VisualElement => Root.Q<T>(id) ?? throw new InvalidOperationException("SAVED_UI_ELEMENT_MISSING:" + id);
        private void CheckThread() { if (Thread.CurrentThread.ManagedThreadId != ownerThread) throw new InvalidOperationException("SAVED_UI_WRONG_THREAD"); }
        private void CheckStartup() { CheckThread(); if (disposed) throw new ObjectDisposedException(nameof(SavedPublic240ReviewPresenter)); if (model != null) throw new InvalidOperationException("SAVED_UI_ALREADY_BOUND"); }
        private void AddButton(string id, string key, Action callback)
        {
            var button = Require<Button>(id); button.text = L(key); buttons.Add(id, button); actions.Add(id, callback); button.clicked += callback;
        }
        private void Unsubscribe()
        {
            foreach (var pair in actions) buttons[pair.Key].clicked -= pair.Value;
            if (picker != null) picker.UnregisterValueChangedCallback(OnPickerChanged);
        }
        private void SetButton(string id, bool enabled)
        {
            var button = buttons[id]; button.SetEnabled(enabled); button.style.display = enabled || (request != null && button.resolvedStyle.display != DisplayStyle.None) ? DisplayStyle.Flex : DisplayStyle.None;
        }
        private void RenderStartup()
        {
            gameHost.style.display = DisplayStyle.None; gameHost.SetEnabled(false); picker.SetEnabled(false);
            candidates.Clear(); confirmation.style.display = DisplayStyle.None; progress.text = L("progress.none"); current.text = L("current.none"); stars.text = L("stars.none");
            foreach (var pair in buttons) { pair.Value.SetEnabled(false); pair.Value.style.display = DisplayStyle.None; }
            SetButton("saved-startup-retry", StartupPhase == SavedStartupPhase.Failed && request == null);
            error.text = requestCancelled ? L("request.cancelled") : requestFault ? L("request.failed") : string.Empty;
        }
        private void OnPickerChanged(ChangeEvent<string> change) { if (!disposed) Refresh(); }
        private void SendPuzzle(bool restore)
        {
            if (model == null || !model.State.CanNavigate || picker.index < 0 || (restore && !model.HasDraftForPuzzle(picker.index))) return;
            Send(SavedReviewUiRequest.Puzzle(restore ? SavedReviewUiRequestKind.ResumeDraft : SavedReviewUiRequestKind.OpenPuzzle, model.State.Version, picker.index));
        }
        private void SendBound(SavedReviewUiRequestKind kind)
        {
            if (model == null) return;
            var state = model.State;
            bool allowed = kind == SavedReviewUiRequestKind.Reload ? state.CanRetryLoad || requestFault
                : kind == SavedReviewUiRequestKind.CreateProfile ? state.CanCreateProfile
                : kind == SavedReviewUiRequestKind.Restart ? state.CanRestart : state.CanRetrySave;
            if (allowed) Send(SavedReviewUiRequest.Bound(kind, state.Version));
        }
        private void Send(SavedReviewUiRequest intent)
        {
            CheckThread();
            if (disposed || request != null || (requestFault && intent.Kind != SavedReviewUiRequestKind.Reload && intent.Kind != SavedReviewUiRequestKind.RetryStartup)) return;
            if (intent.Kind == SavedReviewUiRequestKind.RetryStartup && (model != null || StartupPhase != SavedStartupPhase.Failed)) return;
            requestCancelled = false; requestKind = intent.Kind;
            try { request = runRequest(intent) ?? throw new InvalidOperationException("SAVED_UI_NULL_REQUEST_TASK"); }
            catch { requestFault = true; Refresh(); return; }
            // Read Exception even if destroyed before the next frame. This continuation has no Unity access.
            _ = request.ContinueWith(t => { if (t.IsFaulted) { var observed = t.Exception; } }, TaskScheduler.Default);
            Refresh();
        }
        private void PollRequest()
        {
            if (request == null || !request.IsCompleted) return;
            if (request.IsFaulted) { var observed = request.Exception; requestFault = true; }
            else if (request.IsCanceled) { requestCancelled = true; requestFault = true; }
            else if (requestKind == SavedReviewUiRequestKind.RetryStartup && model == null) requestFault = false;
            request = null;
        }
        private void RenderCandidates(SavedReviewState state, bool clear)
        {
            if (candidateVersion == state.Version)
            {
                candidates.SetEnabled(clear && state.CanRecover); return;
            }
            candidateVersion = state.Version; selectedCandidate = null; candidates.Clear();
            foreach (var candidate in state.RecoveryChoices)
            {
                var kind = candidate.Kind;
                var displayedVersion = state.Version;
                var source = candidate.SourceBytesSha256;
                var text = locale.Resolve(Prefix + "candidate", L("candidate.kind." + kind), L("candidate.status." + candidate.Status),
                    candidate.Generation.HasValue ? locale.Resolve(Prefix + "generation", candidate.Generation.Value) : L("generation.none"),
                    source == null ? L("hash.none") : source.Substring(0, 12));
                var button = new Button(() => { if (disposed || model == null || model.State.Version != displayedVersion || !model.State.CanRecover || candidate.Status != SaveCandidateStatus.Valid) return;
                    selectedCandidate = kind; foreach (var child in candidates.Children()) child.EnableInClassList("saved-candidate--selected", child.name == "saved-candidate-" + kind); Refresh(); });
                button.name = "saved-candidate-" + kind; button.text = text;
                button.SetEnabled(candidate.Status == SaveCandidateStatus.Valid && state.CanRecover);
                button.AddToClassList("saved-candidate"); candidates.Add(button);
            }
            candidates.SetEnabled(clear && state.CanRecover);
        }
        private void ConfirmRecovery()
        {
            if (model == null || !model.State.CanRecover || !selectedCandidate.HasValue || candidateVersion != model.State.Version) return;
            resetConfirmation = false; confirmationVersion = model.State.Version; confirmText.text = L("confirm.recovery"); Refresh();
        }
        private void ShowResetConfirmation()
        {
            if (model == null || !model.State.CanReset) return;
            resetConfirmation = true; confirmationVersion = model.State.Version; confirmText.text = L("confirm.reset"); Refresh();
        }
        private void ClearConfirmation() { confirmationVersion = -1; if (confirmation != null) confirmation.style.display = DisplayStyle.None; }
        private void Confirm()
        {
            if (model == null || model.State.Version != confirmationVersion) { ClearConfirmation(); return; }
            var state = model.State;
            SavedReviewUiRequest? intent = resetConfirmation ? state.CanReset ? SavedReviewUiRequest.Bound(SavedReviewUiRequestKind.ConfirmReset, state.Version) : null
                : state.CanRecover && selectedCandidate.HasValue ? SavedReviewUiRequest.Recovery(state.Version, selectedCandidate.Value) : null;
            ClearConfirmation(); if (intent != null) Send(intent);
        }
        private static void ValidateLocaleTable(Public240ReviewLocalization table)
        {
            foreach (var key in new[] { "boundary", "startup.loading", "startup.failed.assets", "startup.failed.profile", "startup.failed.storage", "startup.failed.composition",
                "startup.retry", "request.failed", "request.cancelled", "picker.label", "open", "resume", "restart", "create", "retry.save", "retry.completion",
                "reload", "recover", "reset", "confirm", "cancel", "confirm.recovery", "confirm.reset", "current.none", "progress.none", "stars.none", "generation.none", "hash.none" }) table.Resolve(Prefix + key);
            foreach (SavedReviewStatus value in Enum.GetValues(typeof(SavedReviewStatus))) table.Resolve(Prefix + "status." + value);
            foreach (SaveCandidateKind value in Enum.GetValues(typeof(SaveCandidateKind))) table.Resolve(Prefix + "candidate.kind." + value);
            foreach (SaveCandidateStatus value in Enum.GetValues(typeof(SaveCandidateStatus))) table.Resolve(Prefix + "candidate.status." + value);
            table.Resolve(Prefix + "error", "SAVE_STATUS"); table.Resolve(Prefix + "current", "QA");
            table.Resolve(Prefix + "progress", 1L, 0L, 0, 240); table.Resolve(Prefix + "stars", "QA", 1);
            table.Resolve(Prefix + "candidate", "QA", "QA", "QA", "QA"); table.Resolve(Prefix + "generation", 1L);
            foreach (var key in new[] { "boundary", "phase.editing", "phase.ride", "phase.results", "phase.overview", "count.legend", "endpoint.a", "endpoint.b",
                "multi.off", "multi.on", "apply", "undo", "restart", "selection.empty", "status.ready", "status.objective", "result.title", "result.boundary",
                "picker.label", "picker.previous", "picker.next", "picker.open" }) table.Resolve("qa.public240." + key);
            foreach (DevelopmentTool value in Enum.GetValues(typeof(DevelopmentTool))) table.Resolve(Public240ReviewPresenter.ToolKey(value));
            table.Resolve("qa.public240.title", "QA", 4, 4); table.Resolve("qa.public240.timer", 0L, 0L);
            table.Resolve("qa.public240.count", 0, 1); table.Resolve("qa.public240.cell", 1, 1, "QA");
            table.Resolve("qa.public240.selection", 0); table.Resolve("qa.public240.status.rejected", "QA");
            table.Resolve("qa.public240.result.time", 0L, 0L); table.Resolve("qa.public240.picker.item", "QA", 4, 4);
        }
    }
}
