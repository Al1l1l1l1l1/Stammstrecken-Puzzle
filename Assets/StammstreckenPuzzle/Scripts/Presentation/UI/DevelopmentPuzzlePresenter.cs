using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using STP.Application;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;

namespace STP.Presentation.UI
{
    /// <summary>Explicitly isolated, in-memory QA StringTable; it never changes global localization settings.</summary>
    public sealed class DevelopmentQaLocalization : IDisposable
    {
        private readonly Locale locale;
        private readonly SharedTableData shared;
        private readonly StringTable table;
        private readonly CultureInfo culture = CultureInfo.GetCultureInfo("de-DE");
        private readonly Dictionary<string, string[]> argumentContracts = new Dictionary<string, string[]>(StringComparer.Ordinal);
        private bool disposed;

        /// <summary>Validates the own QA source and creates a real Unity Localization table.</summary>
        public DevelopmentQaLocalization(TextAsset source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var document = JsonUtility.FromJson<LocaleDocument>(source.text);
            if (document == null || document.locale != "de-DE" || document.entries == null || document.entries.Length == 0)
                throw new ArgumentException("QA_LOCALE_INVALID", nameof(source));
            var keys = new HashSet<string>(StringComparer.Ordinal);
            foreach (var entry in document.entries)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.key) || !entry.key.StartsWith("qa.wp038.", StringComparison.Ordinal)
                    || string.IsNullOrWhiteSpace(entry.value) || !keys.Add(entry.key))
                    throw new ArgumentException("QA_LOCALE_ENTRY_INVALID", nameof(source));
                ValidateFormat(entry);
                argumentContracts.Add(entry.key, entry.argumentTypes);
            }
            locale = Locale.CreateLocale("de-DE");
            shared = ScriptableObject.CreateInstance<SharedTableData>();
            shared.TableCollectionName = "qa.wp038";
            table = ScriptableObject.CreateInstance<StringTable>();
            table.SharedData = shared;
            table.LocaleIdentifier = locale.Identifier;
            for (var i = 0; i < document.entries.Length; i++)
            {
                var entry = document.entries[i];
                shared.AddKey(entry.key, i + 1);
                table.AddEntry(i + 1, entry.value);
            }
        }

        /// <summary>Resolves a key with explicit de-DE formatting and without a global SelectedLocale lookup.</summary>
        public string Resolve(string key, params object[] arguments)
        {
            if (disposed) throw new ObjectDisposedException(nameof(DevelopmentQaLocalization));
            var entry = table.GetEntry(key);
            if (entry == null) throw new InvalidOperationException("QA_LOCALE_KEY_MISSING:" + key);
            var contract = argumentContracts[key];
            if (arguments == null || arguments.Length != contract.Length)
                throw new ArgumentException("QA_LOCALE_ARGUMENT_COUNT:" + key, nameof(arguments));
            for (var i = 0; i < contract.Length; i++)
                if (!ArgumentMatches(contract[i], arguments[i]))
                    throw new ArgumentException("QA_LOCALE_ARGUMENT_TYPE:" + key, nameof(arguments));
            return entry.GetLocalizedString(culture, arguments, null);
        }

        /// <summary>Releases the own transient ScriptableObjects; no global handles are retained.</summary>
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            Destroy(table);
            Destroy(shared);
            Destroy(locale);
        }

        private static void Destroy(UnityEngine.Object value)
        {
            if (UnityEngine.Application.isPlaying) UnityEngine.Object.Destroy(value);
            else UnityEngine.Object.DestroyImmediate(value);
        }

        private static void ValidateFormat(LocaleEntry entry)
        {
            if (entry.argumentTypes == null) throw new ArgumentException("QA_LOCALE_FORMAT_INVALID");
            var used = new bool[entry.argumentTypes.Length];
            for (var i = 0; i < entry.argumentTypes.Length; i++)
                if (entry.argumentTypes[i] != "integer" && entry.argumentTypes[i] != "number" && entry.argumentTypes[i] != "string")
                    throw new ArgumentException("QA_LOCALE_FORMAT_TYPE_INVALID");
            const string pattern = @"\{([0-9]+)(?::(00|N1))?\}";
            foreach (Match match in Regex.Matches(entry.value, pattern, RegexOptions.CultureInvariant))
            {
                if (!int.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var index)
                    || index < 0 || index >= entry.argumentTypes.Length
                    || (match.Groups[2].Success && entry.argumentTypes[index] == "string"))
                    throw new ArgumentException("QA_LOCALE_FORMAT_INDEX_INVALID");
                used[index] = true;
            }
            var unformatted = Regex.Replace(entry.value, pattern, string.Empty, RegexOptions.CultureInvariant);
            if (unformatted.IndexOf('{') >= 0 || unformatted.IndexOf('}') >= 0)
                throw new ArgumentException("QA_LOCALE_FORMAT_INVALID");
            for (var i = 0; i < used.Length; i++)
                if (!used[i]) throw new ArgumentException("QA_LOCALE_FORMAT_ARGUMENT_UNUSED");
        }

        private static bool ArgumentMatches(string expected, object? value)
        {
            if (expected == "string") return value is string;
            var integer = value is byte || value is sbyte || value is short || value is ushort
                || value is int || value is uint || value is long || value is ulong;
            if (expected == "integer" || integer) return integer;
            if (value is decimal) return true;
            if (value is double floating) return !double.IsNaN(floating) && !double.IsInfinity(floating);
            if (value is float single) return !float.IsNaN(single) && !float.IsInfinity(single);
            return false;
        }

        [Serializable]
        private sealed class LocaleDocument
        {
            public string locale = string.Empty;
            public LocaleEntry[] entries = Array.Empty<LocaleEntry>();
        }

        [Serializable]
        private sealed class LocaleEntry
        {
            public string key = string.Empty;
            public string value = string.Empty;
            public string[] argumentTypes = Array.Empty<string>();
        }
    }

    /// <summary>QA presenter: transitory tool/selection state, actual Application commands, no save or reward claim.</summary>
    public sealed class DevelopmentPuzzlePresenter : IDisposable
    {
        private static readonly string[] ToolIds =
        {
            "qa-tool-clear", "qa-tool-empty", "qa-tool-occupied", "qa-tool-ns", "qa-tool-ew",
            "qa-tool-ne", "qa-tool-es", "qa-tool-sw", "qa-tool-wn",
        };
        private readonly DevelopmentPuzzleSession model;
        private readonly Func<long> monotonicMilliseconds;
        private readonly DevelopmentQaLocalization localization;
        private readonly Button[] toolButtons = new Button[9];
        private readonly Action[] toolActions = new Action[9];
        private readonly Button multiButton;
        private readonly Button applyButton;
        private readonly Button undoButton;
        private readonly Button restartButton;
        private readonly Label timer;
        private readonly Label phase;
        private readonly Label status;
        private readonly Label resultTime;
        private readonly VisualElement result;
        private readonly bool[] selection;
        private readonly List<DevelopmentCell> selectedCells;
        private readonly string[] selectionMessages;
        private readonly string[] phaseMessages;
        private readonly string readyMessage;
        private readonly string violationsMessage;
        private readonly string multiOff;
        private readonly string multiOn;
        private readonly string emptySelectionMessage;
        private DevelopmentTool tool = DevelopmentTool.TrackEW;
        private bool multiple;
        private int selectionCount;
        private long uiRevision;
        private long renderedUiRevision = -1;
        private long renderedRevision = -1;
        private long renderedSecond = -1;
        private Guid renderedAttempt;
        private DevelopmentPreviewPhase renderedPhase = (DevelopmentPreviewPhase)(-1);
        private bool renderedPaused;
        private DevelopmentCommandStatus renderedCommand = (DevelopmentCommandStatus)(-1);
        private string renderedError = string.Empty;
        private bool disposed;

        /// <summary>Creates the presenter on an already cloned UXML root with an explicit own QA locale.</summary>
        public DevelopmentPuzzlePresenter(VisualElement root, DevelopmentPuzzleSession model, TextAsset qaLocale, Func<long> monotonicMilliseconds)
        {
            Root = root ?? throw new ArgumentNullException(nameof(root));
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            this.monotonicMilliseconds = monotonicMilliseconds ?? throw new ArgumentNullException(nameof(monotonicMilliseconds));
            localization = new DevelopmentQaLocalization(qaLocale);
            try
            {
                selection = new bool[model.Width * model.Height];
                selectedCells = new List<DevelopmentCell>(selection.Length);
                selectionMessages = new string[selection.Length + 1];
                for (var i = 0; i < selectionMessages.Length; i++) selectionMessages[i] = localization.Resolve("qa.wp038.selection", i);
                phaseMessages = new[]
                {
                    localization.Resolve("qa.wp038.phase.editing"), localization.Resolve("qa.wp038.phase.ride"),
                    localization.Resolve("qa.wp038.phase.results"), localization.Resolve("qa.wp038.phase.overview"),
                };
                readyMessage = localization.Resolve("qa.wp038.status.ready");
                violationsMessage = localization.Resolve("qa.wp038.status.objective");
                multiOff = localization.Resolve("qa.wp038.multi.off");
                multiOn = localization.Resolve("qa.wp038.multi.on");
                emptySelectionMessage = localization.Resolve("qa.wp038.selection.empty");
                Require<Label>("qa-boundary").text = localization.Resolve("qa.wp038.boundary");
                Require<Label>("qa-title").text = localization.Resolve("qa.wp038.title", model.Width, model.Height);
                Require<Label>("qa-count-legend").text = localization.Resolve("qa.wp038.count.legend");
                timer = Require<Label>("qa-timer");
                phase = Require<Label>("qa-phase");
                status = Require<Label>("qa-status");
                resultTime = Require<Label>("qa-result-time");
                result = Require<VisualElement>("qa-result");
                Require<Label>("qa-result-title").text = localization.Resolve("qa.wp038.result.title");
                Require<Label>("qa-result-boundary").text = localization.Resolve("qa.wp038.result.boundary");
                for (var i = 0; i < toolButtons.Length; i++)
                {
                    var current = (DevelopmentTool)i;
                    toolButtons[i] = Require<Button>(ToolIds[i]);
                    toolButtons[i].text = localization.Resolve(ToolKey(current));
                    toolActions[i] = () => ChooseTool(current);
                    toolButtons[i].clicked += toolActions[i];
                }
                multiButton = Require<Button>("qa-multiselect");
                applyButton = Require<Button>("qa-apply-selection");
                undoButton = Require<Button>("qa-undo");
                restartButton = Require<Button>("qa-restart");
                applyButton.text = localization.Resolve("qa.wp038.apply");
                undoButton.text = localization.Resolve("qa.wp038.undo");
                restartButton.text = localization.Resolve("qa.wp038.restart");
                multiButton.clicked += ToggleMultiple;
                applyButton.clicked += ApplySelection;
                undoButton.clicked += Undo;
                restartButton.clicked += Restart;
                Grid = new PuzzleGridElement(model.Width, model.Height, OnCellActivated);
                Grid.Configure(model, localization.Resolve);
                Require<VisualElement>("qa-grid-host").Add(Grid);
                Refresh();
            }
            catch
            {
                disposed = true;
                for (var i = 0; i < toolButtons.Length; i++)
                    if (toolButtons[i] != null && toolActions[i] != null) toolButtons[i].clicked -= toolActions[i];
                if (multiButton != null) multiButton.clicked -= ToggleMultiple;
                if (applyButton != null) applyButton.clicked -= ApplySelection;
                if (undoButton != null) undoButton.clicked -= Undo;
                if (restartButton != null) restartButton.clicked -= Restart;
                if (Grid != null)
                {
                    Grid.Dispose();
                    Grid.RemoveFromHierarchy();
                }
                localization.Dispose();
                throw;
            }
        }

        /// <summary>The explicitly supplied view tree, usable through stable semantic IDs.</summary>
        public VisualElement Root { get; }
        /// <summary>The custom grid; it excludes count gutters from train overlays.</summary>
        public PuzzleGridElement Grid { get; } = null!;
        /// <summary>Current UI tool, independent of every cell value.</summary>
        public DevelopmentTool ActiveTool => tool;
        /// <summary>Whether cell activations collect a transient batch selection.</summary>
        public bool MultipleSelection => multiple;
        /// <summary>Number of selected cells, independent of cell contents.</summary>
        public int SelectionCount => selectionCount;

        /// <summary>Refreshes changed state only; unchanged frames create no controls or localized strings.</summary>
        public void Refresh()
        {
            if (disposed) return;
            var second = model.ElapsedAt(monotonicMilliseconds()) / 1000;
            var changed = renderedRevision != model.Revision || renderedAttempt != model.AttemptId
                || renderedPhase != model.Phase || renderedUiRevision != uiRevision || renderedPaused != model.IsPaused
                || renderedCommand != model.LastCommand || !string.Equals(renderedError, model.LastErrorCode, StringComparison.Ordinal);
            if (!changed && renderedSecond == second) return;
            if (renderedSecond != second || renderedAttempt != model.AttemptId)
            {
                timer.text = localization.Resolve("qa.wp038.timer", second / 60, second % 60);
                if (model.Phase == DevelopmentPreviewPhase.Results)
                    resultTime.text = localization.Resolve("qa.wp038.result.time", second / 60, second % 60);
                renderedSecond = second;
            }
            if (!changed) return;
            var editing = model.Phase == DevelopmentPreviewPhase.Editing && !model.IsPaused;
            if (model.Phase != DevelopmentPreviewPhase.Editing && selectionCount > 0) ClearSelection();
            Grid.Refresh(model, selection, editing);
            phase.text = phaseMessages[(int)model.Phase];
            result.style.display = model.Phase == DevelopmentPreviewPhase.Results ? DisplayStyle.Flex : DisplayStyle.None;
            if (model.Phase == DevelopmentPreviewPhase.Results)
                resultTime.text = localization.Resolve("qa.wp038.result.time", second / 60, second % 60);
            for (var i = 0; i < toolButtons.Length; i++)
            {
                toolButtons[i].SetEnabled(editing);
                toolButtons[i].EnableInClassList("qa-tool--active", i == (int)tool);
            }
            multiButton.text = multiple ? multiOn : multiOff;
            multiButton.EnableInClassList("qa-tool--active", multiple);
            multiButton.SetEnabled(editing);
            applyButton.SetEnabled(editing && multiple && selectionCount > 0);
            undoButton.SetEnabled(editing && model.CanUndo);
            restartButton.SetEnabled(model.Phase != DevelopmentPreviewPhase.TrainRide);
            if (model.LastCommand == DevelopmentCommandStatus.Rejected && !string.IsNullOrEmpty(model.LastErrorCode))
                status.text = localization.Resolve("qa.wp038.status.rejected", model.LastErrorCode);
            else if (multiple) status.text = selectionCount == 0 ? emptySelectionMessage : selectionMessages[selectionCount];
            else status.text = model.HasObjectiveViolations ? violationsMessage : readyMessage;
            renderedAttempt = model.AttemptId;
            renderedRevision = model.Revision;
            renderedPhase = model.Phase;
            renderedUiRevision = uiRevision;
            renderedPaused = model.IsPaused;
            renderedCommand = model.LastCommand;
            renderedError = model.LastErrorCode;
        }

        /// <summary>Supplies or clears the World adapter's explicitly injected transparent render texture.</summary>
        public void SetTrainTexture(Texture? texture)
        {
            if (!disposed) Grid.SetTrainTexture(texture);
        }

        /// <summary>Removes owned callbacks and localized objects. Repeat disposal is harmless.</summary>
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            for (var i = 0; i < toolButtons.Length; i++) toolButtons[i].clicked -= toolActions[i];
            multiButton.clicked -= ToggleMultiple;
            applyButton.clicked -= ApplySelection;
            undoButton.clicked -= Undo;
            restartButton.clicked -= Restart;
            Grid.Dispose();
            Grid.RemoveFromHierarchy();
            localization.Dispose();
        }

        /// <summary>Stable localization key for each closed QA tool; not a new rule mapping.</summary>
        public static string ToolKey(DevelopmentTool value)
        {
            switch (value)
            {
                case DevelopmentTool.Clear: return "qa.wp038.tool.clear";
                case DevelopmentTool.EmptyMarker: return "qa.wp038.tool.empty";
                case DevelopmentTool.OccupiedMarker: return "qa.wp038.tool.occupied";
                case DevelopmentTool.TrackNS: return "qa.wp038.tool.ns";
                case DevelopmentTool.TrackEW: return "qa.wp038.tool.ew";
                case DevelopmentTool.TrackNE: return "qa.wp038.tool.ne";
                case DevelopmentTool.TrackES: return "qa.wp038.tool.es";
                case DevelopmentTool.TrackSW: return "qa.wp038.tool.sw";
                case DevelopmentTool.TrackWN: return "qa.wp038.tool.wn";
                default: throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        private T Require<T>(string id) where T : VisualElement => Root.Q<T>(id) ?? throw new InvalidOperationException("QA_UI_ELEMENT_MISSING:" + id);

        private bool CanEdit => !disposed && model.Phase == DevelopmentPreviewPhase.Editing && !model.IsPaused;

        private void ChooseTool(DevelopmentTool value)
        {
            if (!CanEdit) return;
            tool = value;
            uiRevision++;
            Refresh();
        }

        private void ToggleMultiple()
        {
            if (!CanEdit) return;
            multiple = !multiple;
            ClearSelection();
            uiRevision++;
            Refresh();
        }

        private void OnCellActivated(int x, int y)
        {
            if (!CanEdit) return;
            if (multiple)
            {
                var index = y * model.Width + x;
                selection[index] = !selection[index];
                selectionCount += selection[index] ? 1 : -1;
                uiRevision++;
            }
            else model.Apply(x, y, tool, monotonicMilliseconds());
            Refresh();
        }

        private void ApplySelection()
        {
            if (!CanEdit || !multiple || selectionCount == 0) return;
            selectedCells.Clear();
            for (var y = 0; y < model.Height; y++)
                for (var x = 0; x < model.Width; x++)
                    if (selection[y * model.Width + x]) selectedCells.Add(new DevelopmentCell(x, y));
            model.ApplyBatch(selectedCells, tool, monotonicMilliseconds());
            ClearSelection();
            Refresh();
        }

        private void Undo()
        {
            if (!CanEdit) return;
            model.Undo(monotonicMilliseconds());
            Refresh();
        }

        private void Restart()
        {
            if (disposed || model.Phase == DevelopmentPreviewPhase.TrainRide) return;
            model.Restart();
            ClearSelection();
            SetTrainTexture(null);
            Refresh();
        }

        private void ClearSelection()
        {
            Array.Clear(selection, 0, selection.Length);
            selectionCount = 0;
            uiRevision++;
        }
    }
}
