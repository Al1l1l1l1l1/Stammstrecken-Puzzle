using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Infrastructure.Persistence;

namespace STP.Tests.Persistence.EditMode
{
    /// <summary>Real private temporary files and real strict codec; reflection exists only to preserve existing test assembly edges.</summary>
    public sealed class AtomicSaveRepositoryTests
    {
        private string root = null!;
        private LocalSaveCodec codec = null!;
        private SaveFileSystem files = null!;
        private AtomicSaveRepository repository = null!;
        private readonly List<string> points = new List<string>();
        private readonly HashSet<AtomicSaveRepository> explicitlyLoaded = new HashSet<AtomicSaveRepository>();
        private static readonly CancellationToken None = CancellationToken.None;
        private static Type App(string name) => typeof(LocalSaveCodec).GetProperty("Profile")!.PropertyType.Assembly.GetType("STP.Application." + name, true)!;
        private static object EnumApp(string name, string value) => Enum.Parse(App(name), value);
        private static object? P(object value, string name) => LocalSaveContractFixtures.Property(value, name);
        private static object Call(object target, string method, params object?[] args)
        {
            try { return target.GetType().GetMethods().Single(m => m.Name == method && m.GetParameters().Length == args.Length).Invoke(target, args)!; }
            catch (TargetInvocationException e) when (e.InnerException != null) { throw e.InnerException; }
        }
        private static object Await(object task) { ((Task)task).GetAwaiter().GetResult(); return P(task, "Result")!; }
        private static string Status(object result) => P(result, "Status")!.ToString()!;
        private static long Generation(object snapshot) => (long)P(snapshot, "Generation")!;
        private static string Hash(byte[] bytes) { using (var sha = SHA256.Create()) return string.Concat(sha.ComputeHash(bytes).Select(b => b.ToString("x2"))); }
        private SaveFileSystem FileSystem(Action<string>? fault = null) => (SaveFileSystem)Activator.CreateInstance(typeof(SaveFileSystem), root, P(codec, "Profile"), P(codec, "Catalog"), fault)!;
        private AtomicSaveRepository Repo(SaveFileSystem fs)
        {
            return (AtomicSaveRepository)Activator.CreateInstance(typeof(AtomicSaveRepository), fs, codec, LocalSaveContractFixtures.Clock(), "contract-1", "contract-node-reference")!;
        }
        [SetUp] public void Setup()
        {
            // Hosts with long redirected OS temp paths can explicitly choose a short real scratch base; no filesystem is mocked.
            string temporaryBase = Environment.GetEnvironmentVariable("STP_TEST_TEMP_ROOT") ?? Path.GetTempPath();
            do { root = Path.GetFullPath(Path.Combine(temporaryBase, Guid.NewGuid().ToString("N").Substring(0, 6))); } while (Directory.Exists(root));
            SaveFileSystem.ProvisionPrivateRoot(root); // Explicit infrastructure setup happens before the checked constructor/read operations.
            codec = LocalSaveContractFixtures.Codec(); files = FileSystem(p => points.Add(p)); repository = Repo(files); points.Clear();
        }
        [TearDown] public void Cleanup()
        {
            string tempPrefix = Path.GetFullPath(Environment.GetEnvironmentVariable("STP_TEST_TEMP_ROOT") ?? Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            Assert.That(Path.GetFullPath(root).StartsWith(tempPrefix, StringComparison.OrdinalIgnoreCase), Is.True);
            Assert.That(Path.GetFileName(root).Length, Is.EqualTo(6));
            if (Directory.Exists(root)) Directory.Delete(root, true);
            points.Clear();
            explicitlyLoaded.Clear();
        }
        private string PathFor(string kind) => Path.Combine(files.ProfileDirectory, kind == "Main" ? "save.json" : kind == "Backup" ? "save.backup.json" : "save.pending.json");
        private byte[] Bytes(long generation, bool alternative = false) => LocalSaveContractFixtures.Encode(codec, LocalSaveContractFixtures.Snapshot(generation, alternative));
        private void Seed(string kind, byte[] bytes) { Directory.CreateDirectory(files.ProfileDirectory); File.WriteAllBytes(PathFor(kind), bytes); }
        private object Load(AtomicSaveRepository? repo = null, CancellationToken? ct = null)
        {
            var target=repo??repository;var result=Await(Call(target,"LoadAsync",ct??None));
            if(P(result,"Observation")!=null||Status(result)=="NeedsInitialization")lock(explicitlyLoaded)explicitlyLoaded.Add(target);
            return result;
        }
        private void LoadBeforeMutation(AtomicSaveRepository target,CancellationToken ct)
        {
            // ADR046: file seeding alone never authorizes a cold writer. Preserve any already obtained recovery lease.
            lock(explicitlyLoaded)if(explicitlyLoaded.Contains(target))return;
            Load(target,ct);
        }
        private object Commit(long generation, long expected, bool alternative = false, AtomicSaveRepository? repo = null, CancellationToken? ct = null)
        {var target=repo??repository;LoadBeforeMutation(target,ct??None);return Await(Call(target,"CommitAsync",LocalSaveContractFixtures.Snapshot(generation,alternative),expected,ct??None));}
        private object Request(object load, string selected) => Activator.CreateInstance(App("SaveRecoveryRequest"), P(load, "Observation"), EnumApp("SaveCandidateKind", selected))!;
        private object Recover(object request, AtomicSaveRepository? repo = null, CancellationToken? ct = null) => Await(Call(repo ?? repository, "RecoverAsync", request, ct ?? None));
        private static object[] Candidates(object load) => ((IEnumerable)P(P(load, "Observation")!, "Candidates")!).Cast<object>().ToArray();
        private SortedDictionary<string, string> Inventory()
        {
            var result = new SortedDictionary<string, string>(StringComparer.Ordinal);
            if (!Directory.Exists(root)) return result;
            foreach (string dir in Directory.GetDirectories(root, "*", SearchOption.AllDirectories)) result["D:" + dir.Substring(root.Length)] = "directory";
            foreach (string file in Directory.GetFiles(root, "*", SearchOption.AllDirectories)) result["F:" + file.Substring(root.Length)] = Convert.ToBase64String(File.ReadAllBytes(file));
            return result;
        }
        private void AssertFreshTruth(long minimumGeneration)
        {
            var result = Load(Repo(FileSystem())); Assert.That(Status(result), Is.EqualTo("Loaded"), P(result, "ErrorCode")?.ToString());
            Assert.That(Generation(P(result, "Snapshot")!), Is.GreaterThanOrEqualTo(minimumGeneration));
        }
        private static void AssertNotCommitted(object result) { Assert.That(Status(result), Is.Not.EqualTo("Committed")); Assert.That(P(result, "Snapshot"), Is.Null); }

        [Test, Category("ReadOnly")]
        public void ThreeMissingNeverCreatesEvenDirectoryOrDefaultPayload()
        {
            Assert.That(Status(Load()), Is.EqualTo("NeedsInitialization")); Assert.That(Status(Load()), Is.EqualTo("NeedsInitialization"));
            Assert.That(Directory.Exists(root), Is.True); Assert.That(Directory.Exists(files.ProfileDirectory), Is.False); Assert.That(Inventory(), Is.Empty);
        }
        [TestCase("Main"), TestCase("Backup"), TestCase("Pending"), Category("ReadOnly")]
        public void SoleValidCandidateReturnsExactCompleteSourceHash(string kind)
        {
            var bytes = Bytes(2); Seed(kind, bytes); var before = Inventory(); var result = Load();
            Assert.That(Status(result), Is.EqualTo("Loaded")); Assert.That(Generation(P(result, "Snapshot")!), Is.EqualTo(2));
            var candidate = Candidates(result).Single(c => P(c, "Kind")!.ToString() == kind);
            Assert.That(P(candidate, "SourceBytesSha256"), Is.EqualTo(Hash(bytes))); Assert.That(Inventory(), Is.EqualTo(before));
        }
        [TestCase("Main"), TestCase("Backup"), TestCase("Pending"), Category("ReadOnly")]
        public void HighestGenerationWinsIndependentOfCandidateRole(string highest)
        {
            foreach (string kind in new[] { "Main", "Backup", "Pending" }) Seed(kind, Bytes(kind == highest ? 3 : 1));
            var before = Inventory(); Assert.That(Generation(P(Load(), "Snapshot")!), Is.EqualTo(3)); Assert.That(Inventory(), Is.EqualTo(before));
        }
        [Test, Category("ReadOnly")]
        public void EqualPayloadTieAcceptsDifferentEnvelopeButRetainsDistinctRawHashes()
        {
            var bytes = Bytes(2); var other = Encoding.UTF8.GetBytes(" \n" + Encoding.UTF8.GetString(bytes) + "\n");
            Seed("Main", bytes); Seed("Backup", other); var result = Load(); Assert.That(Status(result), Is.EqualTo("Loaded"));
            var c = Candidates(result); Assert.That(P(c[0], "PayloadSha256"), Is.EqualTo(P(c[1], "PayloadSha256")));
            Assert.That(P(c[0], "SourceBytesSha256"), Is.Not.EqualTo(P(c[1], "SourceBytesSha256")));
        }
        [TestCase(true), TestCase(false), Category("ReadOnly")]
        public void SplitBrainNeverExposesAuthoritativeSnapshotAndConservativeMainMustBeHighest(bool validMain)
        {
            Seed("Backup", Bytes(2)); Seed("Pending", Bytes(2, true)); Seed("Main", validMain ? Bytes(2) : Bytes(1));
            var before = Inventory(); var result = Load(); Assert.That(Status(result), Is.EqualTo("SplitBrain"));
            Assert.That(P(result, "Snapshot"), Is.Null); Assert.That(P(result, "Succeeded"), Is.False);
            Assert.That(P(result, "ConservativeSnapshot"), validMain ? Is.Not.Null : Is.Null); Assert.That(Inventory(), Is.EqualTo(before));
        }
        [TestCase("corrupt", "Corrupt"), TestCase("schema", "Unsupported"), TestCase("profile", "Unsupported"), Category("ReadOnly")]
        public void BadOrUnknownSourceIsPreservedWithFullRawHash(string mutation, string expected)
        {
            string text = Encoding.UTF8.GetString(Bytes(2));
            text = mutation == "corrupt" ? "{broken" : mutation == "schema" ? text.Replace("\"saveSchemaVersion\":2", "\"saveSchemaVersion\":3") : text.Replace("STP-SAVE-JCS-1", "STP-SAVE-UNKNOWN-1");
            var bytes = Encoding.UTF8.GetBytes(text); Seed("Main", bytes); var before = Inventory(); var result = Load();
            Assert.That(Status(result), Is.EqualTo(expected)); Assert.That(P(Candidates(result)[0], "SourceBytesSha256"), Is.EqualTo(Hash(bytes))); Assert.That(Inventory(), Is.EqualTo(before));
        }
        [Test, Category("ReadOnly")]
        public void UnreadableCandidateIsFailedNotMissingOrPrefixHashed()
        {
            Seed("Main", Bytes(1)); var before = Inventory();
            var repo = Repo(FileSystem(p => { if (p == "read.Backup.before") throw new UnauthorizedAccessException(); }));
            var result = Load(repo); Assert.That(Status(result), Is.EqualTo("Failed"));
            var failed = Candidates(result)[1]; Assert.That(P(failed, "Status")!.ToString(), Is.EqualTo("Failed")); Assert.That(P(failed, "SourceBytesSha256"), Is.Null); Assert.That(Inventory(), Is.EqualTo(before));
        }
        [Test, Category("ReadOnly")]
        public void OversizedRealCandidateIsFailedWithoutInventedPrefixHash()
        {
            Directory.CreateDirectory(files.ProfileDirectory); using (var stream = new FileStream(PathFor("Main"), FileMode.Create)) stream.SetLength(67108865L);
            var result = Load(); Assert.That(Status(result), Is.EqualTo("Failed")); var candidate = Candidates(result)[0];
            Assert.That(P(candidate, "Status")!.ToString(), Is.EqualTo("Failed")); Assert.That(P(candidate, "SourceBytesSha256"), Is.Null);
            Assert.That(new FileInfo(PathFor("Main")).Length, Is.EqualTo(67108865L));
        }
        [TestCase("Main"), TestCase("Backup"), TestCase("Pending"), Category("ReadOnly")]
        public void ExplicitRecoveryIsReadOnlyAndKeepsFullWriterObservation(string selected)
        {
            Seed("Main", Bytes(3)); Seed("Backup", Bytes(1)); Seed("Pending", Bytes(2)); var load = Load(); var before = Inventory();
            var result = Recover(Request(load, selected)); Assert.That(Status(result), Is.EqualTo("Loaded"));
            Assert.That(Generation(P(result, "Snapshot")!), Is.EqualTo(selected == "Main" ? 3 : selected == "Backup" ? 1 : 2));
            Assert.That(Candidates(result).Max(c => (long?)P(c, "Generation") ?? 0), Is.EqualTo(3)); Assert.That(Inventory(), Is.EqualTo(before));
        }
        [TestCase("Main", "envelope"), TestCase("Backup", "payload"), TestCase("Pending", "appeared"), TestCase("Main", "generation"), TestCase("Backup", "missing"), Category("ReadOnly")]
        public void RecoveryRejectsEveryChangedSourceIncludingUnselectedEnvelope(string kind, string mutation)
        {
            Seed("Main", Bytes(2)); Seed("Backup", Bytes(1)); var request = Request(Load(), "Backup");
            if (mutation == "missing") File.Delete(PathFor(kind));
            else if (mutation == "envelope") Seed(kind, Encoding.UTF8.GetBytes("\n" + Encoding.UTF8.GetString(Bytes(2))));
            else Seed(kind, Bytes(mutation == "generation" ? 3 : 1, mutation == "payload"));
            var before = Inventory(); var result = Recover(request); Assert.That(Status(result), Is.EqualTo("Conflict"));
            Assert.That(P(result, "Observation"), Is.Not.Null); Assert.That(Inventory(), Is.EqualTo(before));
        }
        [Test, Category("ReadOnly")]
        public void PreCancelledReadAndRecoveryHaveNoStorageEffect()
        {
            Seed("Main", Bytes(1)); var request = Request(Load(), "Main"); var before = Inventory(); using (var cts = new CancellationTokenSource())
            { cts.Cancel(); Assert.That(Status(Load(ct: cts.Token)), Is.EqualTo("Cancelled")); Assert.That(Status(Recover(request, ct: cts.Token)), Is.EqualTo("Cancelled")); }
            Assert.That(Inventory(), Is.EqualTo(before));
        }
        [Test]
        public void OrdinaryCommitDoesNotImplicitlyInitializeButExplicitCreateDoes()
        {
            AssertNotCommitted(Commit(1, 0)); Assert.That(Directory.Exists(files.ProfileDirectory), Is.False); Assert.That(Inventory(), Is.Empty);
            var result = Await(Call(repository, "CreateProfileAsync", LocalSaveContractFixtures.EmptyState(), None));
            Assert.That(Status(result), Is.EqualTo("Committed")); AssertFreshTruth(1);
            var before = Inventory(); AssertNotCommitted(Await(Call(repository, "CreateProfileAsync", LocalSaveContractFixtures.EmptyState(), None))); Assert.That(Inventory(), Is.EqualTo(before));
        }
        [TestCase(2L, 0L), TestCase(2L, 2L), TestCase(1L, 1L), TestCase(3L, 1L)]
        public void WrongExpectedOrOfferedGenerationNeverWrites(long offered, long expected)
        { Seed("Main", Bytes(1)); var before = Inventory(); AssertNotCommitted(Commit(offered, expected)); Assert.That(Inventory(), Is.EqualTo(before)); }
        [Test]
        public void LastSafeGenerationCanCommitAndExhaustedGenerationCannotWrite()
        {
            Seed("Main", Bytes(9007199254740990L)); Assert.That(Status(Commit(9007199254740991L, 9007199254740990L)), Is.EqualTo("Committed"));
            var before = Inventory(); AssertNotCommitted(Commit(9007199254740991L, 9007199254740991L)); Assert.That(Inventory(), Is.EqualTo(before));
        }
        [Test]
        public void IndependentRepositoryInstancesRaceWithExactlyOneCasWinner()
        {
            Seed("Main", Bytes(1)); var other = Repo(FileSystem()); var start = new ManualResetEventSlim(false);
            var a = Task.Run(() => { start.Wait(); return Commit(2, 1); }); var b = Task.Run(() => { start.Wait(); return Commit(2, 1, true, other); }); start.Set();
            Task.WaitAll(a, b); Assert.That(new[] { Status(a.Result), Status(b.Result) }.OrderBy(x => x), Is.EqualTo(new[] { "Committed", "Conflict" })); AssertFreshTruth(2); start.Dispose();
        }
        public static IEnumerable<string> CoreWindows() => Enumerable.Range(1, 9).Select(i => new[] { "", "validated", "encoded", "pending-written", "pending-flushed", "pending-verified", "backup-preserved", "promoted", "main-verified", "before-ack" }[i]).Select((s, i) => "step." + (i + 1) + "." + s);
        public static IEnumerable<string> PreparationWindows() => new[] { "open.before", "open.after", "partial", "written", "flush.before", "flush.after", "closed", "verified" }.Select(s => "preparation.highest." + s);
        public static IEnumerable<string> BackupWindows() => new[] { "open.before", "open.after", "partial", "written", "flush.before", "flush.after", "closed", "verified" }.Select(s => "backup.rotate." + s);
        private AtomicSaveRepository Crashing(string window, out Action requireReached)
        {
            bool reached = false; var repo = Repo(FileSystem(p => { points.Add(p); if (p == window) { reached = true; throw new IOException("SAVE_INJECTED_CRASH"); } }));
            requireReached = () => Assert.That(reached, Is.True, "Required actual IO checkpoint not reached: " + window + "; got " + string.Join(",", points)); return repo;
        }
        [TestCaseSource(nameof(CoreWindows))]
        public void EveryCoreCrashWindowPreservesNormallyLoadableHighestTruth(string window)
        {
            Seed("Main", Bytes(1)); var repo = Crashing(window, out var reached); var result = Commit(2, 1, true, repo); reached(); AssertNotCommitted(result); AssertFreshTruth(1);
        }
        [TestCaseSource(nameof(PreparationWindows))]
        public void PendingOnlyHighestPreparationFailureNeverDestroysOriginalPending(string window)
        {
            var highest = Bytes(2); Seed("Pending", highest); Seed("Main", Bytes(1));
            var repo = Crashing(window, out var reached); AssertNotCommitted(Commit(3, 2, true, repo)); reached();
            Assert.That(File.ReadAllBytes(PathFor("Pending")), Is.EqualTo(highest)); AssertFreshTruth(2);
        }
        [TestCaseSource(nameof(BackupWindows))]
        public void EveryBackupRotationWindowPreservesHighestRegularCopy(string window)
        {
            Seed("Main", Bytes(2)); Seed("Backup", Bytes(1)); var repo = Crashing(window, out var reached);
            AssertNotCommitted(Commit(3, 2, true, repo)); reached(); AssertFreshTruth(2);
        }
        [TestCase("pending.open.after"), TestCase("pending.partial"), TestCase("pending.flush.before")]
        public void AfterPendingOnlyPreparationNewPendingFailureKeepsOldHighestInMainOrBackup(string window)
        {
            Seed("Pending", Bytes(2)); Seed("Main", Bytes(1)); var repo = Crashing(window, out var reached);
            AssertNotCommitted(Commit(3, 2, true, repo)); reached(); AssertFreshTruth(2);
        }
        [Test]
        public void ExistingMainUnsupportedReplaceExercisesRealCopyFallback()
        {
            Seed("Main", Bytes(1)); bool forced = false; bool fallback = false;
            var repo = Repo(FileSystem(p => { if (p == "promote.replace.operation") { forced = true; throw new PlatformNotSupportedException(); } if (p == "promote.fallback.after") fallback = true; }));
            Assert.That(Status(Commit(2, 1, true, repo)), Is.EqualTo("Committed")); Assert.That(forced && fallback, Is.True); AssertFreshTruth(2);
        }
        [Test]
        public void PreCancelledCommitHasNoEffect()
        {
            Seed("Main", Bytes(1)); var before = Inventory(); using (var cts = new CancellationTokenSource()) { cts.Cancel(); Assert.That(Status(Commit(2, 1, ct: cts.Token)), Is.EqualTo("Cancelled")); } Assert.That(Inventory(), Is.EqualTo(before));
        }
        [Test]
        public void CancellationWhileAnotherInstanceOwnsWriterLockHasNoEffect()
        {
            Seed("Main", Bytes(1)); var before = Inventory(); using (var cts = new CancellationTokenSource()) using (files.AcquireExclusive(None))
            {
                var task = (Task)Call(Repo(FileSystem()), "CommitAsync", LocalSaveContractFixtures.Snapshot(2), 1L, cts.Token);
                cts.Cancel(); Assert.That(task.Wait(10000), Is.True, "Cancellation must unblock waiting writer");
                Assert.That(Status(P(task, "Result")!), Is.EqualTo("Cancelled"));
            }
            Assert.That(Inventory(), Is.EqualTo(before));
        }
        [TestCaseSource(nameof(CoreWindows))]
        public void CancellationAtCoreTransitionNeverClaimsRollbackAfterPossibleEffect(string window)
        {
            Seed("Main", Bytes(1)); var before = Inventory(); bool reached = false; using (var cts = new CancellationTokenSource())
            {
                var repo = Repo(FileSystem(p => { if (p == window) { reached = true; cts.Cancel(); cts.Token.ThrowIfCancellationRequested(); } }));
                var result = Commit(2, 1, true, repo, cts.Token); Assert.That(reached, Is.True, window);
                Assert.That(Status(result), Is.EqualTo(window.StartsWith("step.1.", StringComparison.Ordinal) || window.StartsWith("step.2.", StringComparison.Ordinal) ? "Cancelled" : "RecoveryRequired"));
                if (Status(result) == "Cancelled") Assert.That(Inventory(), Is.EqualTo(before)); AssertFreshTruth(1);
            }
        }
        [Test]
        public void RecoveryPromotionBindsExactPayloadAndMaxObservedGeneration()
        {
            Seed("Main", Bytes(3)); Seed("Backup", Bytes(1, true)); var load = Load(); Assert.That(Status(Recover(Request(load, "Backup"))), Is.EqualTo("Loaded"));
            Assert.That(Status(Commit(4, 3, true)), Is.EqualTo("Committed")); AssertFreshTruth(4);
            foreach (var raw in new[] { Bytes(3), Bytes(1, true) }) Assert.That(Directory.GetFiles(root, "*.json", SearchOption.AllDirectories).Any(f => File.ReadAllBytes(f).SequenceEqual(raw)), Is.True, "Recovery original not retained");
        }
        [Test]
        public void RecoveryLeaseCannotAuthorizeAnotherPayloadOrNewProcess()
        {
            Seed("Main", Bytes(2)); Seed("Backup", Bytes(2, true)); var load = Load(); Assert.That(Status(Recover(Request(load, "Backup"))), Is.EqualTo("Loaded"));
            var before = Inventory(); AssertNotCommitted(Commit(3, 2)); Assert.That(Inventory(), Is.EqualTo(before));
            AssertNotCommitted(Commit(3, 2, true, Repo(FileSystem()))); Assert.That(Inventory(), Is.EqualTo(before));
        }
        [Test]
        public void RecoveryPromotionRejectsChangedUnselectedEnvelopeWithoutWrite()
        {
            Seed("Main", Bytes(2)); Seed("Backup", Bytes(2, true)); Assert.That(Status(Recover(Request(Load(), "Backup"))), Is.EqualTo("Loaded"));
            Seed("Main", Encoding.UTF8.GetBytes("\n" + Encoding.UTF8.GetString(Bytes(2)))); var before = Inventory();
            Assert.That(Status(Commit(3, 2, true)), Is.EqualTo("Conflict")); Assert.That(Inventory(), Is.EqualTo(before));
        }
        [TestCase("schema"), TestCase("failed"), TestCase("splitbrain")]
        public void NormalCommitDoesNotAutomaticallyRepairProtectedCandidates(string mode)
        {
            Seed("Main", Bytes(2)); Seed("Backup", mode == "schema" ? Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(Bytes(1)).Replace("\"saveSchemaVersion\":2", "\"saveSchemaVersion\":3")) : Bytes(2, true));
            var before = Inventory(); var repo = mode == "failed" ? Repo(FileSystem(p => { if (p == "read.Backup.before") throw new UnauthorizedAccessException(); })) : repository;
            AssertNotCommitted(Commit(3, 2, false, repo)); Assert.That(Inventory(), Is.EqualTo(before));
        }
        [TestCase("pending.partial"), TestCase("preparation.highest.partial"), TestCase("backup.rotate.partial")]
        public void PermissionFailureAfterPossibleEffectIsUncertainAndPreservesTruth(string window)
        {
            Seed("Main", Bytes(2)); bool reached = false; var repo = Repo(FileSystem(p => { if (p == window) { reached = true; throw new UnauthorizedAccessException(); } }));
            var result = Commit(3, 2, true, repo); Assert.That(reached, Is.True); Assert.That(Status(result), Is.EqualTo("RecoveryRequired")); Assert.That(P(result, "ErrorCode"), Is.Not.Null); AssertFreshTruth(2);
        }
        [Test]
        public void ExistingMainFallbackPartialFailureRetainsNormallyLoadableTruth()
        {
            Seed("Main", Bytes(2)); bool partial = false; var repo = Repo(FileSystem(p =>
            { if (p == "promote.replace.operation") throw new PlatformNotSupportedException(); if (p == "promote.fallback.partial") { partial = true; throw new IOException("SAVE_INJECTED_LOW_DISK"); } }));
            var result = Commit(3, 2, true, repo); Assert.That(partial, Is.True); Assert.That(Status(result), Is.EqualTo("RecoveryRequired")); AssertFreshTruth(2);
        }
        [Test]
        public void FinalMainRereadMutationCannotBeAcknowledgedCommitted()
        {
            Seed("Main", Bytes(1)); bool promoted = false; var repo = Repo(FileSystem(p =>
            { if (p == "step.7.promoted") promoted = true; if (p == "read.Main.before" && promoted) File.WriteAllText(PathFor("Main"), "{broken"); }));
            AssertNotCommitted(Commit(2, 1, true, repo)); Assert.That(promoted, Is.True); AssertFreshTruth(1);
        }
        [Test]
        public void ConfirmedCorruptResetPreservesEveryOriginalAndStaleResetDoesNothing()
        {
            byte[] a = Encoding.UTF8.GetBytes("{broken main"), b = Encoding.UTF8.GetBytes("{broken backup"); Seed("Main", a); Seed("Backup", b);
            var load = Load(); var result = Await(Call(repository, "ConfirmedResetCorruptProfileAsync", P(load, "Observation"), LocalSaveContractFixtures.EmptyState(), None));
            Assert.That(Status(result), Is.EqualTo("Committed")); AssertFreshTruth(1);
            foreach (var raw in new[] { a, b }) Assert.That(Directory.GetFiles(Path.Combine(files.ProfileDirectory, "recovery")).Any(f => File.ReadAllBytes(f).SequenceEqual(raw)), Is.True);
            var before = Inventory(); AssertNotCommitted(Await(Call(repository, "ConfirmedResetCorruptProfileAsync", P(load, "Observation"), LocalSaveContractFixtures.EmptyState(), None))); Assert.That(Inventory(), Is.EqualTo(before));
        }
        [TestCase("Main"), TestCase("Pending")]
        public void CorruptMainNeverOverwritesSoleHighestBackupDuringNormalCommit(string corruptKind)
        {
            Seed("Backup", Bytes(2)); Seed(corruptKind, Encoding.UTF8.GetBytes("{broken")); var result = Commit(3, 2, true);
            Assert.That(Status(result), Is.EqualTo("Committed")); AssertFreshTruth(3);
            var old = codec.Decode(File.ReadAllBytes(PathFor("Backup"))); Assert.That(old.Status, Is.EqualTo(LocalSaveDecodeStatus.Valid)); Assert.That(P(old, "Generation"), Is.EqualTo(2));
        }
        [Test, Category("ReadOnly")]
        public void ProfileCatalogAndConsumerNamespacesArePhysicallyDisjoint()
        {
            var paths = new List<string>(); foreach (string use in new[] { "ContractTest", "IsolatedReview", "Production" })
            {
                object identity = Activator.CreateInstance(App("SaveProfileIdentity"), use == "IsolatedReview" ? "qa-public240-v1" : "contract-save-v1", "profile-one", EnumApp("CatalogUse", use))!;
                foreach (char hash in new[] { 'a', 'b' })
                {
                    object catalog = Activator.CreateInstance(App("CatalogIdentity"), "contract-catalog", 1, new string(hash, 64), EnumApp("CatalogUse", use), EnumApp("CatalogApprovalStatus", use == "ContractTest" ? "FixtureOnly" : use == "IsolatedReview" ? "Draft" : "ProductApproved"))!;
                    var fs = (SaveFileSystem)Activator.CreateInstance(typeof(SaveFileSystem), root, identity, catalog, null)!; paths.Add(fs.ProfileDirectory);
                }
            }
            Assert.That(paths.Distinct(StringComparer.OrdinalIgnoreCase).Count(), Is.EqualTo(6)); Assert.That(Inventory(), Is.Empty);
        }
        [TestCase("CON"), TestCase("NUL.txt"), TestCase("profile."), TestCase("../escape"), TestCase("folder/escape"), TestCase("C:\\escape"), Category("ReadOnly")]
        public void InvalidLogicalPathSegmentsNeverBecomeSanitizedFilesystemPaths(string id)
        {
            Assert.Throws<Exception>(() =>
            {
                try { var profile = Activator.CreateInstance(App("SaveProfileIdentity"), "contract-save-v1", id, EnumApp("CatalogUse", "ContractTest")); Activator.CreateInstance(typeof(SaveFileSystem), root, profile, P(codec, "Catalog"), null); }
                catch (Exception e) { throw new Exception("Rejected invalid input", e); }
            });
            Assert.That(Inventory(), Is.Empty);
        }
        [Test, Category("ReadOnly")]
        public void RealReparseRootIsRejectedAndOutsideTargetIsUnchanged()
        {
            Directory.Delete(root, false); // Replace only the empty explicitly provisioned test root, never a profile or candidate.
            string outside = root + "-outside"; Directory.CreateDirectory(outside); string sentinel = Path.Combine(outside, "sentinel.txt"); File.WriteAllText(sentinel, "original-outside");
            bool windows = Path.DirectorySeparatorChar == '\\';
            var info = new ProcessStartInfo
            {
                FileName = windows ? "cmd.exe" : "/bin/ln",
                Arguments = windows ? "/d /c mklink /J \"" + root + "\" \"" + outside + "\"" : "-s \"" + outside + "\" \"" + root + "\"",
                UseShellExecute = false, CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true, RedirectStandardError = true
            };
            try
            {
                using (var process = Process.Start(info)!) { string output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd(); Assert.That(process.WaitForExit(10000), Is.True); Assert.That(process.ExitCode, Is.EqualTo(0), output); }
                Assert.That((File.GetAttributes(root) & FileAttributes.ReparsePoint) != 0, Is.True, "Actual filesystem link was not established");
                var error = Assert.Catch(() => FileSystem()); Assert.That(error!.ToString(), Does.Contain("SAVE_REPARSE_PATH_REJECTED"));
                Assert.That(File.ReadAllText(sentinel), Is.EqualTo("original-outside")); Assert.That(Directory.GetFiles(outside).Length, Is.EqualTo(1));
            }
            finally
            {
                // Unlink only this exact junction/symlink before recursive cleanup; never traverse the outside target.
                if (Directory.Exists(root)) Directory.Delete(root, false);
                if (Directory.Exists(outside)) Directory.Delete(outside, true);
            }
        }
        [Test, Category("ReadOnly")]
        public void ConstructorDoesNotProvisionMissingPrivateRootAndExplicitProvisionCreatesNoProfile()
        {
            Directory.Delete(root, false); var error = Assert.Catch(() => FileSystem()); Assert.That(error!.ToString(), Does.Contain("SAVE_PRIVATE_ROOT_MISSING"));
            Assert.That(Directory.Exists(root), Is.False); SaveFileSystem.ProvisionPrivateRoot(root);
            var before = Inventory(); var fs = FileSystem(); var repo = Repo(fs); Assert.That(Status(Load(repo)), Is.EqualTo("NeedsInitialization"));
            Assert.That(Inventory(), Is.EqualTo(before)); Assert.That(Directory.Exists(fs.ProfileDirectory), Is.False);
        }
        public static IEnumerable<TestCaseData> MutationFaultCases()
        {
            var windows = PreparationWindows().Concat(BackupWindows()).Concat(new[] { "pending.open.before", "pending.open.after", "pending.partial", "pending.written", "pending.flush.before", "pending.flush.after", "pending.closed", "promote.replace.before", "promote.replace.after" });
            foreach (string point in windows) foreach (string failure in new[] { "lowdisk", "permission", "cancel" }) yield return new TestCaseData(point, failure);
        }
        [TestCaseSource(nameof(MutationFaultCases))]
        public void EveryMutationFileWindowHasLowdiskPermissionAndCancellationTruthOracle(string point, string failure)
        {
            Seed("Main", Bytes(2)); bool reached = false; using (var cts = new CancellationTokenSource())
            {
                var repo = Repo(FileSystem(p => { if (p != point) return; reached = true; if (failure == "cancel") { cts.Cancel(); cts.Token.ThrowIfCancellationRequested(); } if (failure == "permission") throw new UnauthorizedAccessException(); throw new IOException("SAVE_INJECTED_LOW_DISK"); }));
                var result = Commit(3, 2, true, repo, cts.Token); Assert.That(reached, Is.True, point); Assert.That(Status(result), Is.EqualTo("RecoveryRequired")); AssertFreshTruth(2);
            }
        }
        public static IEnumerable<TestCaseData> ArchiveFaultCases()
        {
            foreach (string area in new[] { "recovery", "migration-source" }) foreach (string suffix in ArchiveWindows()) foreach (string failure in new[] { "lowdisk", "permission", "cancel" }) yield return new TestCaseData(area, suffix, failure);
        }
        [TestCaseSource(nameof(ArchiveFaultCases))]
        public void EverySourceArchiveWindowFailurePreventsDestructiveCandidateWrite(string area, string suffix, string failure)
        {
            byte[] original = area == "migration-source" ? LocalSaveContractFixtures.V1Source() : Bytes(2); Seed("Main", original);
            if (area == "recovery") Seed("Backup", Bytes(2, true)); var candidatesBefore = new[] { "Main", "Backup", "Pending" }.Select(k => File.Exists(PathFor(k)) ? File.ReadAllBytes(PathFor(k)) : null).ToArray();
            bool reached = false; using (var cts = new CancellationTokenSource())
            {
                var repo = Repo(FileSystem(p => { if (p != area + "." + suffix) return; reached = true; if (failure == "cancel") { cts.Cancel(); cts.Token.ThrowIfCancellationRequested(); } if (failure == "permission") throw new UnauthorizedAccessException(); throw new IOException("SAVE_INJECTED_LOW_DISK"); }));
                var loaded = Load(repo); object offered; long expected;
                if (area == "recovery") { Assert.That(Status(Recover(Request(loaded, "Backup"), repo)), Is.EqualTo("Loaded")); offered = LocalSaveContractFixtures.Snapshot(3, true); expected = 2; }
                else { var snapshot = P(loaded, "Snapshot")!; expected = Generation(snapshot); offered = LocalSaveContractFixtures.Snapshot(P(snapshot, "Payload")!, expected + 1); }
                var result = Await(Call(repo, "CommitAsync", offered, expected, cts.Token)); Assert.That(reached, Is.True, area + "." + suffix); Assert.That(Status(result), Is.EqualTo("RecoveryRequired"));
            }
            int i = 0; foreach (string kind in new[] { "Main", "Backup", "Pending" }) { var after = File.Exists(PathFor(kind)) ? File.ReadAllBytes(PathFor(kind)) : null; Assert.That(after, Is.EqualTo(candidatesBefore[i++]), kind); }
        }
        [TestCase("profile"), TestCase("catalog"), Category("ReadOnly")]
        public void RecoveryRequestForAnotherBindingCannotSelectLocalCandidate(string change)
        {
            Seed("Main", Bytes(1)); var load = Load(); var observation = P(load, "Observation")!; object profile = P(observation, "Profile")!, catalog = P(observation, "Catalog")!;
            if (change == "profile") profile = Activator.CreateInstance(App("SaveProfileIdentity"), "contract-save-v1", "other-profile", EnumApp("CatalogUse", "ContractTest"))!;
            else catalog = Activator.CreateInstance(App("CatalogIdentity"), "other-catalog", 1, new string('f', 64), EnumApp("CatalogUse", "ContractTest"), EnumApp("CatalogApprovalStatus", "FixtureOnly"))!;
            object foreign = Activator.CreateInstance(App("SaveRepositoryObservation"), profile, catalog, P(observation, "Candidates"))!;
            object request = Activator.CreateInstance(App("SaveRecoveryRequest"), foreign, EnumApp("SaveCandidateKind", "Main"))!;
            var before = Inventory(); var result = Recover(request); Assert.That(Status(result), Is.EqualTo("Conflict")); Assert.That(Inventory(), Is.EqualTo(before));
        }
        [Test]
        public void MigrationWritePreservesExactOriginalV1Source()
        {
            byte[] source = LocalSaveContractFixtures.V1Source(); Seed("Main", source); var load = Load(); Assert.That(Status(load), Is.EqualTo("Loaded"));
            var selected = P(load, "Snapshot")!; object offered = Activator.CreateInstance(selected.GetType(), P(selected, "Profile"), P(selected, "Catalog"), Generation(selected) + 1, P(selected, "Payload"))!;
            var result = Await(Call(repository, "CommitAsync", offered, Generation(selected), None)); Assert.That(Status(result), Is.EqualTo("Committed"));
            string archive = Path.Combine(files.ProfileDirectory, "migration-source"); Assert.That(Directory.Exists(archive), Is.True);
            Assert.That(Directory.GetFiles(archive).Any(f => File.ReadAllBytes(f).SequenceEqual(source)), Is.True); AssertFreshTruth(Generation(selected) + 1);
        }
        public static IEnumerable<string> ArchiveWindows() => new[] { "directory.before", "directory.after", "Main.open.before", "Main.open.after", "Main.partial", "Main.written", "Main.flush.before", "Main.flush.after", "Main.closed", "Main.staging-verified", "Main.finalize.before", "Main.finalize.after", "Main.verified" };
        [TestCaseSource(nameof(ArchiveWindows))]
        public void RecoveryArchiveFailureKeepsEveryOriginalCandidateUnchanged(string suffix)
        {
            var main = Bytes(2); var backup = Bytes(2, true); Seed("Main", main); Seed("Backup", backup);
            var repo = Crashing("recovery." + suffix, out var reached); var result = Recover(Request(Load(repo), "Backup"), repo); Assert.That(Status(result), Is.EqualTo("Loaded"));
            AssertNotCommitted(Commit(3, 2, true, repo)); reached();
            Assert.That(File.ReadAllBytes(PathFor("Main")), Is.EqualTo(main)); Assert.That(File.ReadAllBytes(PathFor("Backup")), Is.EqualTo(backup));
            Assert.That(Status(Load(Repo(FileSystem()))), Is.EqualTo("SplitBrain"));
        }
        [TestCaseSource(nameof(ArchiveWindows))]
        public void MigrationArchiveFailureCannotWriteTargetOrChangeOriginal(string suffix)
        {
            var source = LocalSaveContractFixtures.V1Source(); Seed("Main", source); var repo = Crashing("migration-source." + suffix, out var reached); var load = Load(repo);
            var selected = P(load, "Snapshot")!; object offered = LocalSaveContractFixtures.Snapshot(P(selected, "Payload")!, Generation(selected) + 1);
            AssertNotCommitted(Await(Call(repo, "CommitAsync", offered, Generation(selected), None))); reached();
            Assert.That(File.ReadAllBytes(PathFor("Main")), Is.EqualTo(source)); Assert.That(File.Exists(PathFor("Pending")), Is.False); AssertFreshTruth(Generation(selected));
        }
        [TestCaseSource(nameof(PreparationWindows))]
        public void PendingOnlyMigrationPreparationFailureRetainsRegularHighestSource(string window)
        {
            var source = LocalSaveContractFixtures.V1Source(); Seed("Pending", source); Seed("Main", Bytes(1)); var repo = Crashing(window, out var reached); var load = Load(repo);
            var selected = P(load, "Snapshot")!; object offered = LocalSaveContractFixtures.Snapshot(P(selected, "Payload")!, Generation(selected) + 1);
            AssertNotCommitted(Await(Call(repo, "CommitAsync", offered, Generation(selected), None))); reached();
            Assert.That(File.ReadAllBytes(PathFor("Pending")), Is.EqualTo(source)); AssertFreshTruth(Generation(selected));
        }
        [TestCaseSource(nameof(PreparationWindows))]
        public void PendingOnlyRecoveryPreparationFailureKeepsHighestRegularTruth(string window)
        {
            var highest = Bytes(2); Seed("Pending", highest); Seed("Main", Bytes(1, true)); var repo = Crashing(window, out var reached);
            var load = Load(repo); Assert.That(Status(Recover(Request(load, "Main"), repo)), Is.EqualTo("Loaded"));
            AssertNotCommitted(Commit(3, 2, true, repo)); reached(); Assert.That(File.ReadAllBytes(PathFor("Pending")), Is.EqualTo(highest)); AssertFreshTruth(2);
        }
        [Test]
        public void RawArchiveWithMismatchingBytesIsNeverOverwrittenOrAccepted()
        {
            var main = Bytes(2); Seed("Main", main); Seed("Backup", Bytes(2, true)); var load = Load(); Assert.That(Status(Recover(Request(load, "Backup"))), Is.EqualTo("Loaded"));
            string archive = Path.Combine(files.ProfileDirectory, "recovery"); Directory.CreateDirectory(archive); var wrong = Encoding.UTF8.GetBytes("foreign-content");
            string boundPath = Path.Combine(archive, "Main-" + Hash(main) + ".json"); File.WriteAllBytes(boundPath, wrong);
            AssertNotCommitted(Commit(3, 2, true)); Assert.That(File.ReadAllBytes(boundPath), Is.EqualTo(wrong)); Assert.That(File.ReadAllBytes(PathFor("Main")), Is.EqualTo(main));
        }
        [Test]
        public void NonselectedReadFailurePreventsRecoveryEvenWhenSelectedValid()
        {
            Seed("Main", Bytes(2)); Seed("Backup", Bytes(1)); var request = Request(Load(), "Main");
            var repo = Repo(FileSystem(p => { if (p == "read.Pending.before") throw new UnauthorizedAccessException(); })); var before = Inventory();
            AssertNotCommittedLikeLoad(Recover(request, repo)); Assert.That(Inventory(), Is.EqualTo(before));
        }
        private static void AssertNotCommittedLikeLoad(object result) { Assert.That(P(result, "Succeeded"), Is.False); Assert.That(P(result, "Snapshot"), Is.Null); }
        [TestCase("recovery"), TestCase("migration-source"), Category("ArchiveRetryRegression")]
        public void PartialArchiveCrashAllowsFreshExplicitRecoveryOrMigrationRetry(string area)
        {
            byte[] original = area == "recovery" ? Bytes(2) : LocalSaveContractFixtures.V1Source(); Seed("Main", original);
            if (area == "recovery") Seed("Backup", Bytes(2, true));
            var interrupted = Crashing(area + ".Main.partial", out var reached); var firstLoad = Load(interrupted);
            object offered; long expected;
            if (area == "recovery") { Assert.That(Status(Recover(Request(firstLoad, "Backup"), interrupted)), Is.EqualTo("Loaded")); offered = LocalSaveContractFixtures.Snapshot(3, true); expected = 2; }
            else { var snapshot = P(firstLoad, "Snapshot")!; expected = Generation(snapshot); offered = LocalSaveContractFixtures.Snapshot(P(snapshot, "Payload")!, expected + 1); }
            Assert.That(Status(Await(Call(interrupted, "CommitAsync", offered, expected, None))), Is.EqualTo("RecoveryRequired")); reached();
            Assert.That(File.ReadAllBytes(PathFor("Main")), Is.EqualTo(original));
            // Discard both repository and lease. The retry is based on a genuinely fresh normal load and explicit selection.
            var restarted = Repo(FileSystem()); var fresh = Load(restarted);
            if (area == "recovery") Assert.That(Status(Recover(Request(fresh, "Backup"), restarted)), Is.EqualTo("Loaded"));
            else Assert.That(Status(fresh), Is.EqualTo("Loaded"));
            var retry = Await(Call(restarted, "CommitAsync", offered, expected, None));
            Assert.That(Status(retry), Is.EqualTo("Committed"), P(retry, "ErrorCode")?.ToString()); AssertFreshTruth(expected + 1);
            string archive = Path.Combine(files.ProfileDirectory, area);
            Assert.That(Directory.GetFiles(archive, "*.json").Any(f => File.ReadAllBytes(f).SequenceEqual(original)), Is.True, "Final source archive must contain complete original bytes after retry");
        }
        [TestCase("recovery"), TestCase("migration-source"), Category("ArchiveFinalizedRetry")]
        public void AfterArchiveMoveCrashFreshExplicitRecoveryOrMigrationRetryUsesCompleteFinalSource(string area)
        {
            byte[] original = area == "recovery" ? Bytes(2) : LocalSaveContractFixtures.V1Source(); Seed("Main", original);
            if (area == "recovery") Seed("Backup", Bytes(2, true));
            var interrupted = Crashing(area + ".Main.finalize.after", out var reached); var firstLoad = Load(interrupted);
            object offered; long expected;
            if (area == "recovery") { Assert.That(Status(Recover(Request(firstLoad, "Backup"), interrupted)), Is.EqualTo("Loaded")); offered = LocalSaveContractFixtures.Snapshot(3, true); expected = 2; }
            else { var snapshot = P(firstLoad, "Snapshot")!; expected = Generation(snapshot); offered = LocalSaveContractFixtures.Snapshot(P(snapshot, "Payload")!, expected + 1); }
            Assert.That(Status(Await(Call(interrupted, "CommitAsync", offered, expected, None))), Is.EqualTo("RecoveryRequired")); reached();
            Assert.That(File.ReadAllBytes(PathFor("Main")), Is.EqualTo(original));
            string archive = Path.Combine(files.ProfileDirectory, area); string final = Path.Combine(archive, "Main-" + Hash(original) + ".json");
            Assert.That(File.ReadAllBytes(final), Is.EqualTo(original), "Crash after Move must leave the complete final content-addressed source");
            var restarted = Repo(FileSystem()); var fresh = Load(restarted);
            if (area == "recovery") Assert.That(Status(Recover(Request(fresh, "Backup"), restarted)), Is.EqualTo("Loaded"));
            else Assert.That(Status(fresh), Is.EqualTo("Loaded"));
            var retry = Await(Call(restarted, "CommitAsync", offered, expected, None));
            Assert.That(Status(retry), Is.EqualTo("Committed"), P(retry, "ErrorCode")?.ToString()); AssertFreshTruth(expected + 1);
            Assert.That(File.ReadAllBytes(final), Is.EqualTo(original), "Fresh retry must preserve the already complete original archive");
        }
        private sealed class DiskFullIOException : IOException { internal DiskFullIOException(int code) : base("Injected operating-system disk-full code") { HResult = code; } }
        [TestCase(unchecked((int)0x80070070)), TestCase(unchecked((int)0x80070027)), TestCase(28)]
        public void ActualIOExceptionHResultMapsDiskFullWithoutDependingOnMessage(int code)
        {
            Seed("Main", Bytes(2)); bool reached = false; var repo = Repo(FileSystem(p => { if (p == "pending.partial") { reached = true; throw new DiskFullIOException(code); } }));
            var result = Commit(3, 2, true, repo); Assert.That(reached, Is.True); Assert.That(Status(result), Is.EqualTo("RecoveryRequired")); Assert.That(P(result, "ErrorCode"), Is.EqualTo("SAVE_LOW_DISK")); AssertFreshTruth(2);
        }
    }
}
