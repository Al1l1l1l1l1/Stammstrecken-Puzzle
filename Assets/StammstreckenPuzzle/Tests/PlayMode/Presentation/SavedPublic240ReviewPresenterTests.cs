using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Presentation.UI;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace STP.Tests.Presentation.PlayMode
{
    /// <summary>Model-free startup and immutable intent contracts without adding an Application assembly edge.</summary>
    public sealed class SavedPublic240ReviewPresenterTests
    {
        private const string UiPath = "Assets/StammstreckenPuzzle/UI/";
#if UNITY_EDITOR
        private static VisualElement Frame() => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UiPath + "SavedPublic240Review.uxml").CloneTree();
        private static VisualTreeAsset PuzzleTree() => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UiPath + "Public240Review.uxml");
        private static TextAsset Locale() => AssetDatabase.LoadAssetAtPath<TextAsset>(UiPath + "SavedPublic240ReviewLocale.json");

        [Test]
        public void ModelFreeStartup_ShowsLocalizedLoadingAndAllClosedFailureReasons_WithoutGridOrProgress()
        {
            var frame = Frame();
            using var presenter = new SavedPublic240ReviewPresenter(frame, PuzzleTree(), Locale(), () => 0, _ => Task.CompletedTask);
            Assert.That(presenter.StartupPhase, Is.EqualTo(SavedStartupPhase.Loading));
            Assert.That(presenter.HasBoundModel, Is.False); Assert.That(presenter.Core, Is.Null);
            Assert.That(frame.Q<Label>("saved-status").text, Does.Contain("vorbereiten"));
            Assert.That(frame.Q<Label>("saved-progress").text, Does.Contain("Kein bestätigter"));
            Assert.That(frame.Q<VisualElement>("saved-game-host").childCount, Is.Zero);
            foreach (SavedStartupFailure failure in Enum.GetValues(typeof(SavedStartupFailure)))
            {
                presenter.ShowStartupFailure(failure);
                Assert.That(presenter.StartupPhase, Is.EqualTo(SavedStartupPhase.Failed));
                Assert.That(frame.Q<Label>("saved-status").text, Does.Not.StartWith("qa."));
                Assert.That(frame.Q<Button>("saved-startup-retry").enabledSelf, Is.True);
                Assert.That(frame.Q<Button>("saved-create").enabledSelf, Is.False);
                Assert.That(frame.Q<Button>("saved-reset").enabledSelf, Is.False);
            }
            presenter.ShowStartupLoading(); Assert.That(presenter.StartupPhase, Is.EqualTo(SavedStartupPhase.Loading));
            Assert.Throws<ArgumentOutOfRangeException>(() => presenter.ShowStartupFailure((SavedStartupFailure)99));
        }

        [UnityTest]
        public IEnumerator StartupRetry_IsObserved_DuplicateClickBlocked_TaskCompletionIsNotModelSuccess()
        {
            var frame = Frame(); var source = new TaskCompletionSource<bool>(); int calls = 0;
            SavedReviewUiRequest? captured = null;
            using var presenter = new SavedPublic240ReviewPresenter(frame, PuzzleTree(), Locale(), () => 0, intent => { calls++; captured = intent; return source.Task; });
            var panel = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<PanelSettings>(UiPath + "Public240ReviewPanelSettings.asset")); var go = new GameObject("SavedStartupTest");
            var document = go.AddComponent<UIDocument>(); document.panelSettings = panel; document.rootVisualElement.Add(frame);
            try
            {
                presenter.ShowStartupFailure(SavedStartupFailure.CompositionFailed);
                Submit(frame.Q<Button>("saved-startup-retry"));
                Assert.That(calls, Is.EqualTo(1)); Assert.That(captured!.Kind, Is.EqualTo(SavedReviewUiRequestKind.RetryStartup));
                Assert.That(captured.ExpectedStateVersion, Is.Null); Assert.That(captured.PuzzleIndex, Is.Null);
                Assert.That(presenter.PendingRequest, Is.SameAs(source.Task));
                Assert.That(frame.Q<Button>("saved-startup-retry").enabledSelf, Is.False);
                source.SetResult(true); presenter.Refresh();
                Assert.That(presenter.PendingRequest, Is.Null); Assert.That(presenter.HasBoundModel, Is.False);
                Assert.That(presenter.Core, Is.Null); Assert.That(presenter.StartupPhase, Is.EqualTo(SavedStartupPhase.Failed));
                Assert.That(frame.Q<Button>("saved-create").enabledSelf, Is.False);
                yield return null;
            }
            finally { UnityEngine.Object.Destroy(go); UnityEngine.Object.Destroy(panel); }
        }

        [Test]
        public void RootFailureReporting_UsesClosedLocalizedBanner_AndClearCannotBindOrGrant()
        {
            var frame = Frame();
            using var presenter = new SavedPublic240ReviewPresenter(frame, PuzzleTree(), Locale(), () => 0, _ => Task.CompletedTask);
            presenter.ShowRequestFailure(false);
            Assert.That(presenter.HasRequestFault, Is.True);
            Assert.That(frame.Q<Label>("saved-error").text, Does.Contain("fehlgeschlagen"));
            presenter.ShowRequestFailure(true);
            Assert.That(frame.Q<Label>("saved-error").text, Does.Contain("abgebrochen"));
            presenter.ClearRequestFailure();
            Assert.That(presenter.HasRequestFault, Is.False); Assert.That(presenter.HasBoundModel, Is.False);
            Assert.That(frame.Q<Button>("saved-open").enabledSelf, Is.False);
        }

        [UnityTest]
        public IEnumerator ConstructorFailureAndDispose_ReleaseExactlyOwnLocalization_NoGlobalMutation()
        {
            int before = LocaleCount();
            Assert.Throws<InvalidOperationException>(() => new SavedPublic240ReviewPresenter(new VisualElement(), PuzzleTree(), Locale(), () => 0, _ => Task.CompletedTask));
            yield return null;
            Assert.That(LocaleCount(), Is.EqualTo(before));
            var frame = Frame();
            var presenter = new SavedPublic240ReviewPresenter(frame, PuzzleTree(), Locale(), () => 0, _ => Task.CompletedTask);
            Assert.That(LocaleCount(), Is.EqualTo(before + 3));
            presenter.Dispose(); presenter.Dispose();
            Assert.That(presenter.StartupPhase, Is.EqualTo(SavedStartupPhase.Disposed));
            Assert.Throws<ObjectDisposedException>(() => presenter.ShowStartupLoading());
            yield return null;
            Assert.That(LocaleCount(), Is.EqualTo(before));
        }

        [Test]
        public void WrongThread_RejectsBeforeUnityOrLocaleAccess()
        {
            using var presenter = new SavedPublic240ReviewPresenter(Frame(), PuzzleTree(), Locale(), () => 0, _ => Task.CompletedTask);
            Exception? observed = null;
            var thread = new System.Threading.Thread(() => { try { presenter.Refresh(); } catch (Exception error) { observed = error; } });
            thread.Start(); Assert.That(thread.Join(2000), Is.True);
            Assert.That(observed, Is.TypeOf<InvalidOperationException>());
            Assert.That(observed!.Message, Is.EqualTo("SAVED_UI_WRONG_THREAD"));
        }

        [Test]
        public void CompleteSavedLocale_ContainsAllUnchangedCoreKeys_AndNoUnsavedResultClaim()
        {
            SavedPublic240ReviewPresenter.ValidateLocale(Locale());
            using var saved = new Public240ReviewLocalization(Locale());
            var original = JsonUtility.FromJson<LocaleDocument>(AssetDatabase.LoadAssetAtPath<TextAsset>(UiPath + "Public240ReviewLocale.json").text);
            foreach (var entry in original.entries)
            {
                var arguments = entry.argumentTypes.Select(type => type == "string" ? (object)"QA" : type == "integer" ? (object)1 : 1.0).ToArray();
                Assert.That(saved.Resolve(entry.key, arguments), Is.Not.Empty, entry.key);
            }
            Assert.That(saved.Resolve("qa.public240.result.boundary"), Does.Contain("QA-Profil gespeichert"));
            Assert.That(saved.Resolve("qa.public240.result.boundary"), Does.Not.Contain("keinen Abschluss"));
            foreach (var suffix in new[] { "startup.loading", "startup.failed.assets", "startup.failed.profile", "startup.failed.storage", "startup.failed.composition", "request.failed", "request.cancelled", "open", "resume", "confirm.reset", "confirm.recovery" })
                Assert.That(saved.Resolve("qa.public240.saved." + suffix), Is.Not.Empty);
        }

        [TestCase("qa.public240.saved.status.RecoveryRequired")]
        [TestCase("qa.public240.saved.candidate.status.Unsupported")]
        [TestCase("qa.public240.saved.stars")]
        [TestCase("qa.public240.tool.ne")]
        public void MissingClosedLocaleKey_IsErrorBeforeBindingOrWorker(string key)
        {
            var broken = new TextAsset(Locale().text.Replace("\"" + key + "\"", "\"qa.public240.saved.missing-required-key\""));
            try { Assert.Throws<InvalidOperationException>(() => SavedPublic240ReviewPresenter.ValidateLocale(broken)); }
            finally { UnityEngine.Object.DestroyImmediate(broken); }
        }

        [Test]
        public void SavedLocale_ResolvesExactGermanUnicode_AndRejectsDoubleDecodedUtf8Evidence()
        {
            var source = Locale();
            var strictUtf8 = new System.Text.UTF8Encoding(false, true);
            Assert.That(strictUtf8.GetString(source.bytes), Is.EqualTo(source.text));
            using var saved = new Public240ReviewLocalization(source);
            Assert.That(saved.Resolve("qa.public240.undo"), Is.EqualTo("R\u00fcckg\u00e4ngig"));
            Assert.That(saved.Resolve("qa.public240.picker.label"), Is.EqualTo("R\u00e4tsel"));
            Assert.That(saved.Resolve("qa.public240.picker.open"), Is.EqualTo("\u00d6ffnen"));
            Assert.That(saved.Resolve("qa.public240.title", "QA", 4, 10), Is.EqualTo("QA \u00b7 4 \u00d7 10"));
            Assert.That(saved.Resolve("qa.public240.tool.ne"), Is.EqualTo("Kurve oben\u2013rechts"));
            Assert.That(saved.Resolve("qa.public240.status.objective"), Does.Contain("Regelversto\u00df"));
            Assert.That(saved.Resolve("qa.public240.boundary"), Does.Contain("Pr\u00fcffassung"));
            Assert.That(saved.Resolve("qa.public240.saved.startup.loading"), Is.EqualTo("Lokalen QA-Speicher vorbereiten \u2026"));
            Assert.That(saved.Resolve("qa.public240.saved.status.Saving"), Is.EqualTo("Lokale \u00c4nderungen werden gespeichert \u2026"));
            Assert.That(saved.Resolve("qa.public240.saved.stars", "QA", 1), Is.EqualTo("Best\u00e4tigte QA-Sterne f\u00fcr QA: 1/3"));
            var document = JsonUtility.FromJson<LocaleDocument>(source.text);
            Assert.That(document.entries.Length, Is.EqualTo(93));
            foreach (var entry in document.entries)
                foreach (var invalid in new[] { "\u00c3", "\u00c2", "\u00e2\u20ac", "\ufffd" })
                    Assert.That(entry.value.IndexOf(invalid, StringComparison.Ordinal), Is.EqualTo(-1), entry.key);
        }

        private static int LocaleCount() => Resources.FindObjectsOfTypeAll<ScriptableObject>().Count(value =>
            value.GetType().FullName == "UnityEngine.Localization.Locale" || value.GetType().FullName == "UnityEngine.Localization.Tables.SharedTableData" || value.GetType().FullName == "UnityEngine.Localization.Tables.StringTable");
        private static void Submit(Button button)
        {
            using var submit = NavigationSubmitEvent.GetPooled(); submit.target = button; button.SendEvent(submit);
        }
        [Serializable] private sealed class LocaleDocument { public LocaleEntry[] entries = Array.Empty<LocaleEntry>(); }
        [Serializable] private sealed class LocaleEntry { public string key = string.Empty; public string value = string.Empty; public string[] argumentTypes = Array.Empty<string>(); }
#endif

        [Test]
        public void UiIntentFactories_AreClosedAndImmutable_NoContradictoryArgumentShapes()
        {
            var value = SavedReviewUiRequest.Puzzle(SavedReviewUiRequestKind.ResumeDraft, 7, 239);
            Assert.That(value.Kind, Is.EqualTo(SavedReviewUiRequestKind.ResumeDraft));
            Assert.That(value.ExpectedStateVersion, Is.EqualTo(7)); Assert.That(value.PuzzleIndex, Is.EqualTo(239));
            Assert.That(typeof(SavedReviewUiRequest).GetProperties().All(property => property.SetMethod == null), Is.True);
            Assert.That(typeof(SavedReviewUiRequest).GetConstructors(), Is.Empty);
            Assert.Throws<ArgumentOutOfRangeException>(() => SavedReviewUiRequest.Bound(SavedReviewUiRequestKind.OpenPuzzle, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => SavedReviewUiRequest.Bound(SavedReviewUiRequestKind.ConfirmRecovery, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => SavedReviewUiRequest.Bound((SavedReviewUiRequestKind)99, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => SavedReviewUiRequest.Bound(SavedReviewUiRequestKind.Reload, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => SavedReviewUiRequest.Puzzle(SavedReviewUiRequestKind.Restart, 1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => SavedReviewUiRequest.Puzzle(SavedReviewUiRequestKind.OpenPuzzle, 1, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => SavedReviewUiRequest.Puzzle(SavedReviewUiRequestKind.OpenPuzzle, 1, 240));
            // Candidate enum remains an Application type; inspect it without a new test assembly reference.
            var recovery = typeof(SavedReviewUiRequest).GetMethod("Recovery")!;
            var kind = recovery.GetParameters()[1].ParameterType;
            foreach (int index in new[] { 0, 1, 2 })
                Assert.That(recovery.Invoke(null, new[] { (object)1L, Enum.ToObject(kind, index) }), Is.Not.Null);
            var failure = Assert.Throws<TargetInvocationException>(() => recovery.Invoke(null, new[] { (object)1L, Enum.ToObject(kind, 99) }));
            Assert.That(failure!.InnerException, Is.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
