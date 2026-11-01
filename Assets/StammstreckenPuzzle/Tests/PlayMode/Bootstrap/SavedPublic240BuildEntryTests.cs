#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;
using STP.Bootstrap;
using STP.Platform;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace STP.Tests.Bootstrap.PlayMode
{
    /// <summary>Concrete normal saved-entry, source-dependency, worker/isolation and closed configuration tests; actual platform reports remain regular CI evidence.</summary>
    public sealed class SavedPublic240BuildEntryTests
    {
        private sealed class OwnedWork
        {
            internal readonly string Root;
            internal readonly MainThreadSaveDispatcher Dispatcher;
            internal readonly CancellationTokenSource Cancellation = new CancellationTokenSource();
            internal readonly List<Task> Tasks = new List<Task>();
            internal OwnedWork(string root, MainThreadSaveDispatcher dispatcher) { Root = root; Dispatcher = dispatcher; }
        }
        private readonly List<OwnedWork> ownedWork = new List<OwnedWork>();
        private sealed class OwnedInstaller
        {
            internal string Root = "";
            internal GameObject Host = null!;
            internal SavedPublic240ReviewInstaller Installer = null!;
            internal IDisposable? Scope;
        }
        private readonly List<OwnedInstaller> ownedInstallers = new List<OwnedInstaller>();
        private const string Root = "Assets/StammstreckenPuzzle/";
        private const string SavedScene = Root + "Scenes/QA/SavedPublic240Review.unity";
        private const string PublicScene = Root + "Scenes/QA/Public240Review.unity";
        private static readonly string[] ExpectedScenes =
        {
            SavedScene, PublicScene, Root + "Scenes/QA/DevelopmentPuzzle.unity", Root + "Scenes/QA/BootstrapComposition.unity"
        };

        [UnityTearDown]
        public IEnumerator FinishActualWorkersBeforeAnyTemporaryRootCleanup()
        {
            foreach (var item in ownedInstallers) if (item.Host != null) UnityEngine.Object.Destroy(item.Host);
            if (ownedInstallers.Count != 0) yield return null;
            float installerDeadline = Time.realtimeSinceStartup + 30f;
            while (ownedInstallers.Any(i => !i.Installer.ShutdownTask.IsCompleted) && Time.realtimeSinceStartup < installerDeadline) yield return null;
            foreach (var item in ownedInstallers)
            {
                Assert.That(item.Installer.ShutdownTask.IsCompleted, Is.True, "Keep scope/root until actual installer work stops: " + item.Root);
                item.Installer.ShutdownTask.GetAwaiter().GetResult(); item.Scope?.Dispose(); DeleteOwnRoot(item.Root);
            }
            ownedInstallers.Clear();
            foreach (var work in ownedWork) { work.Cancellation.Cancel(); work.Dispatcher.Dispose(); }
            float deadline = Time.realtimeSinceStartup + 30f;
            while (ownedWork.Any(w => w.Tasks.Any(t => !t.IsCompleted)) && Time.realtimeSinceStartup < deadline) yield return null;
            foreach (var work in ownedWork)
            {
                Assert.That(work.Tasks.All(t => t.IsCompleted), Is.True,
                    "Outstanding work did not terminate; retain the explicit temporary root rather than deleting under a live worker: " + work.Root);
                foreach (var task in work.Tasks) if (task.IsFaulted) _ = task.Exception;
                work.Cancellation.Dispose(); DeleteOwnRoot(work.Root);
            }
            ownedWork.Clear();
        }

        [Test]
        public void NormalBuildAndEditorInputsHaveFourSeparateOrderedEntriesAndStableGuids()
        {
            var method = BuildType("StpBuildEntrypoints").GetMethod("GetQaScenePaths", BindingFlags.Public | BindingFlags.Static)!;
            var first = (string[])method.Invoke(null, null)!;
            CollectionAssert.AreEqual(ExpectedScenes, first);
            first[0] = "changed caller copy";
            CollectionAssert.AreEqual(ExpectedScenes, (string[])method.Invoke(null, null)!);
            var enabled = EditorBuildSettings.scenes.Where(x => x.enabled).ToArray();
            CollectionAssert.AreEqual(ExpectedScenes, enabled.Select(x => x.path).ToArray());
            foreach (var scene in enabled)
            {
                Assert.That(AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path), Is.Not.Null);
                Assert.That(scene.guid.ToString(), Is.EqualTo(AssetDatabase.AssetPathToGUID(scene.path)));
            }
        }

        [Test]
        public void SavedBuildPreflightReadsExistingAAndSAssetsWithoutChangingThem()
        {
            var protectedPaths = new[]
            {
                PublicScene, PublicScene + ".meta", Root + "UI/Public240ReviewPanelSettings.asset",
                Root + "UI/Public240.public.json", Root + "UI/ReviewCampaign.catalog.json", Root + "UI/ReviewCompletion.catalog.json",
                Root + "Art/Development/DevelopmentTrain.mat", Root + "Scenes/QA/DevelopmentPuzzle.unity"
            };
            var originals = protectedPaths.ToDictionary(path => path, File.ReadAllBytes);
            InvokeBuild("SavedPublic240ReviewBuild", "VerifyArtifacts");
            foreach (var pair in originals) CollectionAssert.AreEqual(pair.Value, File.ReadAllBytes(pair.Key), pair.Key);
            Assert.That(BuildType("Public240ReviewSceneBuildCheck"), Is.Not.Null);
            Assert.That(BuildType("DevelopmentPuzzleSceneBuildCheck"), Is.Not.Null);
            Assert.That(Type.GetType("STP.Editor.Build.SavedPublic240ReviewSceneBuildCheck, STP.Editor.Build", false), Is.Null,
                "Saved processing belongs only to the existing Development guard, without a duplicate guard.");
        }

        [Test]
        public void SavedPlayerDependencyGraphContainsOnlyExplicitPublicAndQaRuntimeAssets()
        {
            InvokeBuild("SavedPublic240ReviewBuild", "VerifyArtifacts");
            var dependencies = AssetDatabase.GetDependencies(SavedScene, true);
            foreach (var expected in new[]
            {
                Root + "UI/Public240.public.json", Root + "UI/ReviewCampaign.catalog.json", Root + "UI/ReviewCompletion.catalog.json",
                Root + "UI/SavedPublic240ReviewLocale.json", Root + "UI/Public240Review.uxml",
                Root + "UI/SavedPublic240Review.uxml", Root + "Scripts/Bootstrap/SavedPublic240ReviewInstaller.cs"
            }) CollectionAssert.Contains(dependencies, expected);
            foreach (string path in dependencies)
            {
                Assert.That(path, Does.Not.Contain("/Tests/").And.Not.Contain("/Authoring/").And.Not.Contain("/Proofs/").And.Not.Contain("/Reviews/"));
                Assert.That(path, Does.Not.Contain("SourceLock").And.Not.Contain("source-lock").And.Not.Contain("Fixture"));
            }
            Assert.That(dependencies.Count(x => x.EndsWith(".public.json", StringComparison.Ordinal)), Is.EqualTo(1));
        }

        [TestCase("qa", "com.STP.StammstreckenPuzzle.qa", true)]
        [TestCase("production", "com.STP.StammstreckenPuzzle.qa", false)]
        [TestCase("staging", "com.STP.StammstreckenPuzzle.qa", false)]
        [TestCase("qa", "com.STP.StammstreckenPuzzle", false)]
        [TestCase("qa", "", false)]
        [TestCase("", "com.STP.StammstreckenPuzzle.qa", false)]
        public void SavedEntryUsesActualExistingClosedQaConfiguration(string profile, string identifier, bool allowed)
        {
            if (allowed) Assert.DoesNotThrow(() => SavedPublic240ReviewInstaller.ValidateQaProfile(profile, identifier));
            else Assert.Throws<InvalidOperationException>(() => SavedPublic240ReviewInstaller.ValidateQaProfile(profile, identifier));
        }

        [Test]
        public void UnsafeEnvironmentCannotPermitRealProfileRootAncestorOrDescendant()
        {
            string real = UnityEngine.Application.persistentDataPath;
            string? prior = Environment.GetEnvironmentVariable("STP_TEST_TEMP_ROOT");
            try
            {
                Environment.SetEnvironmentVariable("STP_TEST_TEMP_ROOT", Path.GetPathRoot(Path.GetFullPath(real)));
                foreach (string unsafePath in new[] { real, Path.Combine(real, "test-child"), Path.GetDirectoryName(real)! })
                    Assert.Throws<ArgumentException>(() => SavedPublic240ReviewInstaller.UseTestPrivateRoot(unsafePath), unsafePath);
            }
            finally { Environment.SetEnvironmentVariable("STP_TEST_TEMP_ROOT", prior); }
        }

        [Test]
        public void TestPrivateRootIsSingleScopedAndRejectsRelativeRoots()
        {
            Assert.Throws<ArgumentException>(() => SavedPublic240ReviewInstaller.UseTestPrivateRoot("relative-root"));
            string root = TemporaryRoot();
            try
            {
                using (SavedPublic240ReviewInstaller.UseTestPrivateRoot(root))
                    Assert.Throws<InvalidOperationException>(() => SavedPublic240ReviewInstaller.UseTestPrivateRoot(root));
                using (SavedPublic240ReviewInstaller.UseTestPrivateRoot(root)) { }
            }
            finally { DeleteOwnRoot(root); }
        }

        [Test]
        public void NonordinaryWindowsNamespaceAndTrimmedComponentInputsNeverProvisionRoots()
        {
            string real = UnityEngine.Application.persistentDataPath.Replace('/', '\\');
            foreach (string prefix in new[] { @"\\?\", @"\\.\", @"\??\" })
                Assert.Throws<ArgumentException>(() => SavedPublic240ReviewInstaller.UseTestPrivateRoot(prefix + real));
            if (Path.DirectorySeparatorChar == '\\')
            {
                string temp = Path.GetTempPath();
                foreach (string leaf in new[] { "unsafe. ", "unsafe.", "unsafe ", "unsafe:stream" })
                    Assert.Throws<ArgumentException>(() => SavedPublic240ReviewInstaller.UseTestPrivateRoot(Path.Combine(temp, leaf, "child")));
            }
        }

        [Test]
        public void PhysicalBoundaryAllowsSafeMissingSuffixWithoutProvisioningAndRejectsProtectedAncestors()
        {
            string root = TemporaryRoot();
            try
            {
                Directory.CreateDirectory(root);
                string real = Path.Combine(root, "protected"), temp = Path.Combine(root, "allowed"), safe = Path.Combine(temp, "missing", "child");
                Directory.CreateDirectory(real); Directory.CreateDirectory(temp);
                var method = typeof(SavedPublic240ReviewInstaller).GetMethod("ValidateTestRootBoundary", BindingFlags.NonPublic | BindingFlags.Static)!;
                string Invoke(string candidate, string? environment)
                {
                    try { return (string)method.Invoke(null, new object?[] { candidate, real, temp, environment })!; }
                    catch (TargetInvocationException error) { throw error.InnerException!; }
                }
                Assert.That(Invoke(safe, null), Is.EqualTo(safe));
                Assert.That(Directory.Exists(safe), Is.False, "Physical identity queries cannot create the suffix.");
                foreach (string candidate in new[] { real, Path.Combine(real, "missing"), root })
                    Assert.Throws<ArgumentException>(() => Invoke(candidate, root));
                Assert.That(Invoke(real + "-sibling", root), Is.EqualTo(real + "-sibling"));
                if (Path.DirectorySeparatorChar == '\\')
                {
                    string file = Path.Combine(temp, "file"); File.WriteAllText(file, "own fixture");
                    Assert.Throws<IOException>(() => Invoke(Path.Combine(file, "child"), null));
                }
                else
                {
                    string alias = Path.Combine(root, "real-alias"), dangling = Path.Combine(root, "dangling-real-alias");
                    void Link(string target, string link)
                    {
                        var info = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "/bin/ln", Arguments = "-s \"" + target + "\" \"" + link + "\"",
                            UseShellExecute = false, CreateNoWindow = true, WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                            RedirectStandardOutput = true, RedirectStandardError = true
                        };
                        using var process = System.Diagnostics.Process.Start(info)!;
                        string output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
                        Assert.That(process.WaitForExit(10000), Is.True); Assert.That(process.ExitCode, Is.Zero, output);
                        Assert.That((File.GetAttributes(link) & FileAttributes.ReparsePoint) != 0, Is.True, "Actual Unix link required.");
                    }
                    try
                    {
                        Link(real, alias); Link(Path.Combine(root, "absent-real-target"), dangling);
                        string candidate = Path.Combine(real, "never-provisioned"), safeMissing = Path.Combine(temp, "missing-ordinary");
                        void RejectProtectedAlias(string protectedRoot)
                        {
                            var error = Assert.Throws<TargetInvocationException>(() => method.Invoke(null,
                                new object?[] { candidate, protectedRoot, root, null }));
                            Assert.That(error!.InnerException, Is.TypeOf<ArgumentException>());
                            Assert.That(error.InnerException!.Message, Is.EqualTo("SAVED_TEST_UNIX_REPARSE_BOUNDARY_REJECTED"));
                        }
                        RejectProtectedAlias(alias); RejectProtectedAlias(Path.Combine(alias, "absent")); RejectProtectedAlias(dangling);
                        Assert.That(Directory.Exists(candidate), Is.False, "No candidate provision through another real-root name.");
                        Assert.That(Invoke(safeMissing, null), Is.EqualTo(safeMissing));
                        Assert.That(Directory.Exists(safeMissing), Is.False);
                        TestContext.WriteLine("SAVED_UNIX_REAL_ALIAS_REJECTED actual/dangling/ancestor; safe missing suffix remains fileless.");
                    }
                    finally
                    {
                        // Unlink only these exact two owned names; never recurse through a link target.
                        if ((File.GetAttributes(alias) & FileAttributes.ReparsePoint) != 0) File.Delete(alias);
                        if ((File.GetAttributes(dangling) & FileAttributes.ReparsePoint) != 0) File.Delete(dangling);
                    }
                }
            }
            finally { DeleteOwnRoot(root); }
        }

        [UnityTest]
        public IEnumerator TestRootScopeCannotEndBeforeActualInstallerShutdown()
        {
            var item = new OwnedInstaller { Root = TemporaryRoot() };
            item.Scope = SavedPublic240ReviewInstaller.UseTestPrivateRoot(item.Root);
            item.Host = new GameObject("Saved entry scope lifetime contract"); item.Host.SetActive(false);
            item.Installer = item.Host.AddComponent<SavedPublic240ReviewInstaller>(); ownedInstallers.Add(item);
            item.Installer.ConfigureForQa(
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Root + "UI/SavedPublic240Review.uxml"),
                AssetDatabase.LoadAssetAtPath<StyleSheet>(Root + "UI/SavedPublic240Review.uss"),
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Root + "UI/Public240Review.uxml"),
                AssetDatabase.LoadAssetAtPath<StyleSheet>(Root + "UI/Public240Review.uss"),
                AssetDatabase.LoadAssetAtPath<TextAsset>(Root + "UI/SavedPublic240ReviewLocale.json"),
                AssetDatabase.LoadAssetAtPath<PanelSettings>(Root + "UI/Public240ReviewPanelSettings.asset"),
                AssetDatabase.LoadAssetAtPath<Material>(Root + "Art/Development/DevelopmentTrain.mat"),
                AssetDatabase.LoadAssetAtPath<TextAsset>(Root + "UI/Public240.public.json"),
                AssetDatabase.LoadAssetAtPath<TextAsset>(Root + "UI/ReviewCampaign.catalog.json"),
                AssetDatabase.LoadAssetAtPath<TextAsset>(Root + "UI/ReviewCompletion.catalog.json"));
            item.Host.SetActive(true);
            Assert.Throws<InvalidOperationException>(() => item.Scope!.Dispose());
            UnityEngine.Object.Destroy(item.Host); yield return null;
            float deadline = Time.realtimeSinceStartup + 30f;
            while (!item.Installer.ShutdownTask.IsCompleted && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(item.Installer.ShutdownTask.IsCompleted, Is.True);
            item.Installer.ShutdownTask.GetAwaiter().GetResult();
            Assert.DoesNotThrow(() => item.Scope!.Dispose()); item.Scope = null;
        }

        [UnityTest]
        public IEnumerator ActualCompositionRunsOnWorkerAndAllMissingRemainsFilelessBeforeExplicitCreate()
        {
            string root = TemporaryRoot();
            using var dispatcher = new MainThreadSaveDispatcher();
            var owned = new OwnedWork(root, dispatcher); ownedWork.Add(owned);
            var clock = new LocalSaveClock();
            int main = Thread.CurrentThread.ManagedThreadId;
            var build = SavedPublic240Composition.CreateAsync(Bytes("UI/Public240.public.json"), Bytes("UI/ReviewCampaign.catalog.json"),
                Bytes("UI/ReviewCompletion.catalog.json"), root, clock, dispatcher, "entry-contract", "editor-public240-review", owned.Cancellation.Token);
            owned.Tasks.Add(build);
            try
            {
                yield return Wait(build, dispatcher);
                var composition = build.GetAwaiter().GetResult();
                Assert.That(composition.WorkerThreadId, Is.Not.EqualTo(main));
                Assert.That(composition.PrivateRoot, Is.EqualTo(root));
                Assert.That(composition.Catalog.Puzzles.Count, Is.EqualTo(240));
                Assert.That(composition.Profile.Use, Is.EqualTo(CatalogUse.IsolatedReview));
                Assert.That(composition.Profile.Namespace, Is.EqualTo("qa-public240-v1"));
                Assert.That(composition.Profile.ProfileId, Is.EqualTo("r"));
                string expected = Path.Combine(root, "qa-public240-v1", composition.ProgressCatalog.CatalogSet.AggregateCatalog.HashSha256, "r");
                Assert.That(composition.Files.ProfileDirectory, Is.EqualTo(expected));
                // Reuse the actual adapter's exact suffix, replacing only the test root for this read-only diagnostic.
                string suffix = composition.Files.ProfileDirectory.Substring(root.Length);
                string actualApplicationProfile = Path.GetFullPath(UnityEngine.Application.persistentDataPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + suffix;
                Assert.DoesNotThrow(() => SavedPublic240Composition.ValidateStoragePathLengthsForProfileDirectory(actualApplicationProfile));
                Debug.Log("STP Saved actual application-root length preflight PASS: rootChars=" + UnityEngine.Application.persistentDataPath.Length
                    + "; profileDirectoryChars=" + actualApplicationProfile.Length + "; profile=r; namespace=qa-public240-v1; no real-profile IO.");
                Assert.That(Directory.Exists(root), Is.True);
                Assert.That(Directory.GetFileSystemEntries(root), Is.Empty);
                var loaded = composition.Persisted.LoadAsync(owned.Cancellation.Token); owned.Tasks.Add(loaded);
                yield return Wait(loaded, dispatcher);
                Assert.That(loaded.GetAwaiter().GetResult().Status, Is.EqualTo(SaveLoadStatus.NeedsInitialization));
                Assert.That(Directory.GetFileSystemEntries(root), Is.Empty, "Read-only initial observation must create neither a profile nor a lockfile.");
            }
            finally { /* UnityTearDown terminates and observes actual workers before cleanup, including assertion failures. */ }
        }

        [UnityTest]
        public IEnumerator CorruptPublicOrReviewSourcesAreRejectedBeforePrivateRootProvisioning()
        {
            using var dispatcher = new MainThreadSaveDispatcher();
            var clock = new LocalSaveClock();
            for (int source = 0; source < 3; source++)
            {
                string root = TemporaryRoot();
                var owned = new OwnedWork(root, dispatcher); ownedWork.Add(owned);
                byte[][] sources = { Bytes("UI/Public240.public.json"), Bytes("UI/ReviewCampaign.catalog.json"), Bytes("UI/ReviewCompletion.catalog.json") };
                sources[source] = System.Text.Encoding.UTF8.GetBytes("{}");
                var build = SavedPublic240Composition.CreateAsync(sources[0], sources[1], sources[2], root, clock, dispatcher,
                    "entry-contract", "editor-public240-review", owned.Cancellation.Token);
                owned.Tasks.Add(build);
                try
                {
                    yield return Wait(build, dispatcher, false);
                    Assert.That(build.IsFaulted, Is.True);
                    Assert.Throws<ArgumentException>(() => build.GetAwaiter().GetResult());
                    Assert.That(Directory.Exists(root), Is.False, "Invalid source must be rejected before even root provisioning.");
                }
                finally { /* Teardown owns cleanup after every actual task has terminated. */ }
            }
        }

        private static IEnumerator Wait(Task task, MainThreadSaveDispatcher dispatcher, bool requireSuccess = true)
        {
            float deadline = Time.realtimeSinceStartup + 30f;
            while (!task.IsCompleted && Time.realtimeSinceStartup < deadline) { dispatcher.Drain(); yield return null; }
            dispatcher.Drain();
            Assert.That(task.IsCompleted, Is.True, "Observed composition task timed out.");
            if (requireSuccess) Assert.DoesNotThrow(() => task.GetAwaiter().GetResult());
        }

        private static byte[] Bytes(string path) => AssetDatabase.LoadAssetAtPath<TextAsset>(Root + path).bytes;
        private static string TemporaryRoot()
        {
            string root = Path.Combine(Path.GetTempPath(), "sti-" + Guid.NewGuid().ToString("N").Substring(0, 8));
            ValidateOwnRootBeforeIo(root);
            return root;
        }
        private static void ValidateOwnRootBeforeIo(string root)
        {
            // Capture Unity's root on the owner thread. Directory identity checks run read-only on a worker,
            // including for direct composition fixtures which intentionally do not provision a test scope.
            string real = UnityEngine.Application.persistentDataPath, temp = Path.GetTempPath();
            string? environment = Environment.GetEnvironmentVariable("STP_TEST_TEMP_ROOT");
            var method = typeof(SavedPublic240ReviewInstaller).GetMethod("ValidateTestRootBoundary", BindingFlags.NonPublic | BindingFlags.Static)!;
            Task.Run(() =>
            {
                try { _ = method.Invoke(null, new object?[] { root, real, temp, environment }); }
                catch (TargetInvocationException error) { throw error.InnerException!; }
            }).GetAwaiter().GetResult();
        }
        private static void DeleteOwnRoot(string root)
        {
            string full = Path.GetFullPath(root), temp = Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            if (!full.StartsWith(temp, Path.DirectorySeparatorChar == '\\' ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)
                || !Path.GetFileName(full).StartsWith("sti-", StringComparison.Ordinal)) throw new InvalidOperationException("TEST_CLEANUP_OUTSIDE_OWN_ROOT");
            ValidateOwnRootBeforeIo(full);
            if (Directory.Exists(full)) Directory.Delete(full, true);
        }

        private static Type BuildType(string name) => Type.GetType("STP.Editor.Build." + name + ", STP.Editor.Build", true)!;
        private static void InvokeBuild(string type, string method)
        {
            try { BuildType(type).GetMethod(method, BindingFlags.Public | BindingFlags.Static)!.Invoke(null, null); }
            catch (TargetInvocationException error) { throw error.InnerException!; }
        }
    }
}
#endif
