using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;
using STP.Puzzle.Domain;

namespace STP.Tests.Application.EditMode
{
    /// <summary>Physical authoring fixtures over all 17 actual raw-locked packs. No human/device attestation.</summary>
    public sealed class HumanRoutingPersistenceTests
    {
        private static readonly ICanonicalJson Json = LocalSaveContractFixtures.Json;
        private const string Utc = "2026-10-21T00:00:00Z";
        private Resolver sources = null!;
        private sealed class Resolver : IHumanManifestResolver
        {
            internal readonly Dictionary<string, HumanRoutingManifest> Values = new Dictionary<string, HumanRoutingManifest>(StringComparer.Ordinal);
            public IReadOnlyList<string> ManifestBytesSha256 => Values.Keys.OrderBy(x => x, StringComparer.Ordinal).ToArray();
            public HumanRoutingManifest Resolve(string hash) => Values[hash];
        }
        [OneTimeSetUp]
        public void ReadAllActualRawLockedManifestsThroughTheExistingReader()
        {
            string root = Environment.GetEnvironmentVariable("STP_SOURCE_ROOT") ?? Directory.GetCurrentDirectory();
            string ui = Path.Combine(root, "Assets", "StammstreckenPuzzle", "UI");
            string publicHash = Regex.Match(File.ReadAllText(Path.Combine(ui, "MobileCandidate.bindings.json")), "\\\"publicCatalogHashSha256\\\"\\s*:\\s*\\\"([a-f0-9]{64})\\\"").Groups[1].Value;
            var corpus = LocalSaveContractFixtures.Construct<IPublicPuzzleCatalog>("STP.Infrastructure.Content", "STP.Infrastructure.Content.MobileCandidateData", File.ReadAllBytes(Path.Combine(ui, "MobileCandidate.public.json")), publicHash);
            string packs = Path.Combine(ui, "HumanAssignments"); var raw = File.ReadAllText(Path.Combine(packs, "raw-sha256-lock.json")); sources = new Resolver();
            var entries = Regex.Matches(raw, "\\\"path\\\"\\s*:\\s*\\\"([^\\\"]+)\\\"\\s*,\\s*\\\"sha256\\\"\\s*:\\s*\\\"([a-f0-9]{64})\\\"");
            Assert.That(entries.Count, Is.EqualTo(17));
            foreach (Match entry in entries)
            {
                byte[] bytes = File.ReadAllBytes(Path.Combine(packs, entry.Groups[1].Value)); string hash = entry.Groups[2].Value;
                var catalog = LocalSaveContractFixtures.Construct<IHumanAssignmentCatalog>("STP.Infrastructure.Content", "STP.Infrastructure.Content.HumanAssignmentCatalog", bytes, hash, corpus, Json);
                Assert.That(catalog.Assignments.Count, Is.EqualTo(6)); sources.Values.Add(hash, new HumanRoutingManifest(bytes, catalog));
            }
        }
        private static string Root() => Path.Combine(Environment.GetEnvironmentVariable("STP_TEST_TEMP_ROOT") ?? Path.Combine(Directory.GetCurrentDirectory(), ".utmp"), "hr-" + Guid.NewGuid().ToString("N").Substring(0, 10));
        private IHumanRoutingRepository Router(string root, Action<string>? fault = null) => LocalSaveContractFixtures.Construct<IHumanRoutingRepository>("STP.Infrastructure.Persistence", "STP.Infrastructure.Persistence.HumanCalibrationStore", root, sources.Values.First().Value.Catalog, sources, Json, fault!);
        private static string Namespace(string root) => Path.Combine(root, "human-calibration-v1");
        private static string Record(string root, HumanRoutingAuthority authority) => Path.Combine(Namespace(root), authority.ManifestBytesSha256!, authority.SessionToken!, "observations.json");
        private static Dictionary<string, byte[]> Files(string root) => Directory.Exists(root) ? Directory.GetFiles(root, "*", SearchOption.AllDirectories).ToDictionary(p => p, File.ReadAllBytes) : new Dictionary<string, byte[]>();
        private static void EqualFiles(Dictionary<string, byte[]> before, string root)
        {
            var after = Files(root); Assert.That(after.Keys.OrderBy(x => x), Is.EqualTo(before.Keys.OrderBy(x => x)));
            foreach (var item in before) Assert.That(after[item.Key], Is.EqualTo(item.Value), item.Key);
        }
        private HumanRoutingInspection Begin(IHumanRoutingRepository router, string? target = null)
        {
            var missing = router.InspectAsync(CancellationToken.None).GetAwaiter().GetResult(); Assert.That(missing.Status, Is.EqualTo(HumanStoreStatus.Missing));
            var result = router.StartNextParticipantAsync(missing.Authority!, target ?? sources.ManifestBytesSha256[0], HumanRunOrigin.AUTOMATED_TEST, Utc, CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(result.Status, Is.EqualTo(HumanStoreStatus.Committed), result.ErrorCode); return result;
        }
        private HumanRoutingInspection Complete(IHumanRoutingRepository router, HumanRoutingInspection prior)
        {
            var run = prior.Snapshot!.Run; var trials = new List<HumanTrialRecord>();
            foreach (var assignment in prior.Catalog!.Assignments)
            {
                var view = new Public240ReviewSession(assignment.Puzzle);
                var checkpoint = (PuzzleSessionCheckpoint)typeof(Public240ReviewSession).GetMethod("CaptureHumanCheckpoint", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(view, new object[] { 0L })!;
                trials.Add(new HumanTrialRecord(assignment.Id, assignment.Puzzle.Identity, assignment.SourceSha256, Guid.NewGuid(), HumanExposureState.EXPOSURE_PENDING,
                    null, false, false, false, Guid.NewGuid(), null, null, true, HumanTrialOutcome.ABORTED, checkpoint, Array.Empty<HumanHintObservation>()));
            }
            var completed = new HumanCalibrationRun(run.ManifestBytesSha256, run.SessionToken, run.Origin, run.CreatedAtUtc, prior.Catalog.Assignments.Count, trials);
            var committed = router.OpenConfirmedRepository(prior.Authority!).CommitAsync(new HumanObservationSnapshot(prior.Snapshot.Generation + 1, completed), prior.Snapshot.Generation, CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(committed.Succeeded, Is.True, committed.ErrorCode); return router.InspectAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
        [Test]
        public void MissingReadOnlyDiscoveryLoadAndRejectedExportNeverCreateDirectories()
        {
            string root = Root(); var router = Router(root); var legacy = (IHumanCalibrationRepository)router;
            Assert.That(Directory.Exists(root), Is.False);
            var first = router.InspectAsync(CancellationToken.None).GetAwaiter().GetResult();
            for (int i = 0; i < 3; i++)
            {
                var inspected = router.InspectAsync(CancellationToken.None).GetAwaiter().GetResult();
                Assert.That(inspected.Status, Is.EqualTo(HumanStoreStatus.Missing)); Assert.That(inspected.Snapshot, Is.Null);
                Assert.That(inspected.GlobalObservationToken, Is.EqualTo(first.GlobalObservationToken));
                Assert.That(legacy.LoadAsync(CancellationToken.None).GetAwaiter().GetResult().Status, Is.EqualTo(HumanStoreStatus.Missing));
                Assert.Throws<IOException>(() => legacy.ExportAsync(CancellationToken.None).GetAwaiter().GetResult());
                Assert.That(Directory.Exists(root), Is.False, "Reads and a rejected export cannot create the private root.");
            }
            var started = router.StartNextParticipantAsync(first.Authority!, sources.ManifestBytesSha256[0], HumanRunOrigin.AUTOMATED_TEST, Utc, CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(started.Status, Is.EqualTo(HumanStoreStatus.Committed));
            Assert.That(Directory.GetFiles(root, "observations.json", SearchOption.AllDirectories).Length, Is.EqualTo(1));
        }
        [Test]
        public void ExistingEmptyNamespaceUsesTheSameMissingObservationWithoutChangingDirectories()
        {
            string root = Root(); var router = Router(root); var absent = router.InspectAsync(CancellationToken.None).GetAwaiter().GetResult();
            Directory.CreateDirectory(Namespace(root)); var directories = Directory.GetDirectories(root, "*", SearchOption.AllDirectories);
            var existing = router.InspectAsync(CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(existing.Status, Is.EqualTo(HumanStoreStatus.Missing));
            Assert.That(existing.GlobalObservationToken, Is.EqualTo(absent.GlobalObservationToken), "The existing inventory hash projection is unchanged.");
            Assert.That(Directory.GetDirectories(root, "*", SearchOption.AllDirectories), Is.EqualTo(directories)); Assert.That(Files(root), Is.Empty);
        }
        [Test]
        public void ExistingNamespaceFileIsUnknownRatherThanAnAuthorizedMissingRepository()
        {
            string root = Root(); Directory.CreateDirectory(root); string path = Namespace(root); byte[] original = Encoding.UTF8.GetBytes("existing non-directory authority"); File.WriteAllBytes(path, original);
            var router = Router(root); var observed = router.InspectAsync(CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(observed.Status, Is.EqualTo(HumanStoreStatus.RecoveryRequired)); Assert.That(observed.Authority, Is.Null);
            Assert.That(((IHumanCalibrationRepository)router).LoadAsync(CancellationToken.None).GetAwaiter().GetResult().Status, Is.EqualTo(HumanStoreStatus.RecoveryRequired));
            Assert.Throws<IOException>(() => ((IHumanCalibrationRepository)router).ExportAsync(CancellationToken.None).GetAwaiter().GetResult());
            Assert.That(File.ReadAllBytes(path), Is.EqualTo(original)); Assert.That(Directory.Exists(path), Is.False);
        }
        [Test]
        public void ExistingAncestorFileCannotBecomeMissingRoutingAuthority()
        {
            string root = Root(); Directory.CreateDirectory(Path.GetDirectoryName(root)!); byte[] original = Encoding.UTF8.GetBytes("private-root file remains untouched"); File.WriteAllBytes(root, original);
            var router = Router(root); var observed = router.InspectAsync(CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(observed.Status, Is.EqualTo(HumanStoreStatus.RecoveryRequired)); Assert.That(observed.Authority, Is.Null);
            Assert.That(File.ReadAllBytes(root), Is.EqualTo(original)); Assert.That(Directory.Exists(root), Is.False);
        }
        [Test]
        public void MissingBoundRepositoryStillRejectsTheStaleParticipantWithoutRecreatingItsNamespace()
        {
            string root = Root(); var router = Router(root); var started = Begin(router); var bound = router.OpenConfirmedRepository(started.Authority!);
            string source = Path.GetFullPath(Namespace(root)), preserved = source + "-fixture-preserved";
            Assert.That(Path.GetDirectoryName(source), Is.EqualTo(Path.GetFullPath(root))); Assert.That(Path.GetDirectoryName(preserved), Is.EqualTo(Path.GetFullPath(root)));
            Directory.Move(source, preserved); var originals = Files(root);
            var loaded = bound.LoadAsync(CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(loaded.Status, Is.EqualTo(HumanStoreStatus.Conflict)); Assert.That(loaded.ErrorCode, Is.EqualTo("HUMAN_BOUND_SESSION_CHANGED"));
            Assert.Throws<IOException>(() => bound.ExportAsync(CancellationToken.None).GetAwaiter().GetResult());
            Assert.That(Directory.Exists(source), Is.False); EqualFiles(originals, root);
        }
        [Test]
        public void ExistingNamespaceReadWaitsForTheUnchangedPhysicalWriterLock()
        {
            string root = Root(); using var entered = new ManualResetEventSlim(); using var release = new ManualResetEventSlim();
            var writer = Router(root, point => { if (point == "RECORD_PENDING_FLUSHED") { entered.Set(); if (!release.Wait(15000)) throw new IOException("Held physical write timed out"); } });
            var reader = Router(root); var missing = writer.InspectAsync(CancellationToken.None).GetAwaiter().GetResult();
            var pending = writer.StartNextParticipantAsync(missing.Authority!, sources.ManifestBytesSha256[0], HumanRunOrigin.AUTOMATED_TEST, Utc, CancellationToken.None);
            Assert.That(entered.Wait(15000), Is.True); var read = reader.InspectAsync(CancellationToken.None);
            try { Assert.That(read.Wait(60), Is.False, "An existing namespace is read under the unchanged physical writer lock."); }
            finally { release.Set(); }
            var committed = pending.GetAwaiter().GetResult(); Assert.That(committed.Status, Is.EqualTo(HumanStoreStatus.Committed));
            var observed = read.GetAwaiter().GetResult(); Assert.That(observed.Status, Is.EqualTo(HumanStoreStatus.Ready));
            Assert.That(observed.Authority!.SessionToken, Is.EqualTo(committed.Authority!.SessionToken));
        }
        [Test]
        public void DiscoveryAndAllSeventeenPackTransitionsUseOriginalCatalogsWithoutRevealingOrChangingPriorBytes()
        {
            string root = Root(); var router = Router(root); var before = Files(root); var empty = router.InspectAsync(CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(empty.Status, Is.EqualTo(HumanStoreStatus.Missing)); Assert.That(empty.Catalog, Is.Null); Assert.That(empty.Snapshot, Is.Null); EqualFiles(before, root);
            HumanRoutingInspection current = Begin(router); var originals = new Dictionary<string, byte[]>();
            foreach (string hash in sources.ManifestBytesSha256)
            {
                if (current.Catalog!.ManifestBytesSha256 != hash)
                {
                    current = router.StartNextParticipantAsync(current.Authority!, hash, HumanRunOrigin.AUTOMATED_TEST, Utc, CancellationToken.None).GetAwaiter().GetResult();
                    Assert.That(current.Status, Is.EqualTo(HumanStoreStatus.Committed), current.ErrorCode);
                }
                Assert.That(current.Catalog!.ManifestBytesSha256, Is.EqualTo(hash)); Assert.That(current.Catalog.Assignments.Count, Is.EqualTo(6)); Assert.That(current.Snapshot!.Run.Trials, Is.Empty);
                Assert.That(current.CanStartParticipant, Is.False); var unstarted = Files(root); router.InspectAsync(CancellationToken.None).GetAwaiter().GetResult(); EqualFiles(unstarted, root);
                current = Complete(router, current); originals.Add(Record(root, current.Authority!), File.ReadAllBytes(Record(root, current.Authority!)));
                foreach (var original in originals) Assert.That(File.ReadAllBytes(original.Key), Is.EqualTo(original.Value));
            }
            Assert.That(originals.Count, Is.EqualTo(17));
            string index = File.ReadAllText(Path.Combine(Namespace(root), "active-run.json")); Assert.That(index, Does.Contain("\"humanIndexVersion\":1"));
            Assert.That(Regex.Matches(index, ":").Count, Is.EqualTo(5)); Assert.That(Directory.GetFiles(Namespace(root), "*transition*", SearchOption.AllDirectories), Is.Empty);
        }
        [TestCase("observation")][TestCase("manifest")][TestCase("token")][TestCase("generation")][TestCase("record")]
        public void EveryClosedExpectedAuthorityFieldIsComparedBeforeAnyFileChanges(string field)
        {
            string root = Root(); var router = Router(root); var current = Complete(router, Begin(router)); var a = current.Authority!; var before = Files(root);
            var stale = HumanRoutingAuthority.Confirmed(field == "manifest" ? sources.ManifestBytesSha256.First(h => h != a.ManifestBytesSha256) : a.ManifestBytesSha256!, field == "token" ? Guid.NewGuid().ToString("N") : a.SessionToken!, field == "generation" ? a.Generation!.Value + 1 : a.Generation!.Value, field == "record" ? new string('0', 64) : a.RecordBytesSha256!, field == "observation" ? new string('0', 64) : a.GlobalObservationToken);
            var result = router.StartNextParticipantAsync(stale, sources.ManifestBytesSha256.Last(), HumanRunOrigin.AUTOMATED_TEST, Utc, CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(result.Status, Is.EqualTo(HumanStoreStatus.Conflict)); Assert.That(result.Authority, Is.Null); EqualFiles(before, root);
        }
        [Test]
        public void ResolverEnabledRouterCannotBypassTheGlobalCompareAndSwapThroughTheLegacyStartApi()
        {
            string root = Root(); var router = Router(root); var legacy = (IHumanCalibrationRepository)router;
            var target = sources.Values.First().Value.Catalog;
            HumanCalibrationRun EmptyRun() => new HumanCalibrationRun(target.ManifestBytesSha256, Guid.NewGuid().ToString("N"), HumanRunOrigin.AUTOMATED_TEST, Utc, 0, Array.Empty<HumanTrialRecord>());
            var before = Files(root); var rejected = legacy.StartRunAsync(EmptyRun(), null, CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(rejected.Status, Is.EqualTo(HumanStoreStatus.Conflict)); Assert.That(rejected.ErrorCode, Is.EqualTo("HUMAN_ROUTING_USE_GLOBAL_START")); EqualFiles(before, root);
            var completed = Complete(router, Begin(router)); before = Files(root);
            rejected = legacy.StartRunAsync(EmptyRun(), completed.Authority!.SessionToken, CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(rejected.Status, Is.EqualTo(HumanStoreStatus.Conflict)); Assert.That(rejected.ErrorCode, Is.EqualTo("HUMAN_ROUTING_USE_GLOBAL_START")); EqualFiles(before, root);
        }
        [Test]
        public void InitialMissingCompareAndSwapSerializesTwoRealWritersAndOnlyCreatesOneToken()
        {
            string root = Root(); using var entered = new ManualResetEventSlim(); using var release = new ManualResetEventSlim();
            var first = Router(root, point => { if (point == "RECORD_PENDING_FLUSHED") { entered.Set(); if (!release.Wait(15000)) throw new IOException("Held physical write timed out"); } });
            var second = Router(root); var missing = first.InspectAsync(CancellationToken.None).GetAwaiter().GetResult();
            var pending = first.StartNextParticipantAsync(missing.Authority!, sources.ManifestBytesSha256[0], HumanRunOrigin.AUTOMATED_TEST, Utc, CancellationToken.None);
            Assert.That(entered.Wait(15000), Is.True); var race = second.StartNextParticipantAsync(missing.Authority!, sources.ManifestBytesSha256[1], HumanRunOrigin.AUTOMATED_TEST, Utc, CancellationToken.None);
            Assert.That(race.Wait(60), Is.False); release.Set(); Assert.That(pending.GetAwaiter().GetResult().Status, Is.EqualTo(HumanStoreStatus.Committed)); Assert.That(race.GetAwaiter().GetResult().Status, Is.EqualTo(HumanStoreStatus.Conflict));
            var actual = second.InspectAsync(CancellationToken.None).GetAwaiter().GetResult(); Assert.That(actual.Catalog!.ManifestBytesSha256, Is.EqualTo(sources.ManifestBytesSha256[0]));
            Assert.That(Directory.GetFiles(Namespace(root), "observations.json", SearchOption.AllDirectories).Length, Is.EqualTo(1));
        }
        [TestCase(false)][TestCase(true)]
        public void OldSameOrDifferentManifestRepositoryCannotLoadCommitRecoverOrExportTheNextParticipant(bool samePack)
        {
            string root = Root(); var router = Router(root); var initial = Begin(router); var old = router.OpenConfirmedRepository(initial.Authority!); var completed = Complete(router, initial);
            using var owner = new HumanCalibrationTestFactory();
            using var oldSession = new AssignedHumanSession(initial.Catalog!, old, owner.Clock, owner.Dispatcher, HumanRunOrigin.AUTOMATED_TEST);
            owner.Pump(oldSession.InitializeAsync(CancellationToken.None));
            string target = samePack ? initial.Catalog!.ManifestBytesSha256 : sources.ManifestBytesSha256.First(h => h != initial.Catalog!.ManifestBytesSha256);
            var next = router.StartNextParticipantAsync(completed.Authority!, target, HumanRunOrigin.AUTOMATED_TEST, Utc, CancellationToken.None).GetAwaiter().GetResult(); Assert.That(next.Status, Is.EqualTo(HumanStoreStatus.Committed));
            var before = Files(root); Assert.That(old.LoadAsync(CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.False);
            Assert.That(old.CommitAsync(new HumanObservationSnapshot(3, completed.Snapshot!.Run), 2, CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.False);
            Assert.That(old.RecoverAsync(completed.GlobalObservationToken, CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.False);
            Assert.Throws<IOException>(() => old.ExportAsync(CancellationToken.None).GetAwaiter().GetResult());
            owner.Pump(oldSession.RetryAsync(CancellationToken.None)); Assert.That(oldSession.State.IsRecoveryRequired, Is.True); Assert.That(oldSession.PuzzleView, Is.Null);
            Assert.That(router.StartNextParticipantAsync(completed.Authority!, target, HumanRunOrigin.AUTOMATED_TEST, Utc, CancellationToken.None).GetAwaiter().GetResult().Status, Is.EqualTo(HumanStoreStatus.Conflict)); EqualFiles(before, root);
        }
        [TestCase("PARTICIPANT_ARCHIVE_PENDING_FLUSHED")][TestCase("PARTICIPANT_ARCHIVE_MAIN_REPLACED")]
        [TestCase("RECORD_PENDING_FLUSHED")][TestCase("RECORD_MAIN_REPLACED")][TestCase("INDEX_PENDING_FLUSHED")][TestCase("INDEX_MAIN_REPLACED")]
        public void CrossManifestSwitchCrashRequiresExplicitObservationAndPreservesCompletedOriginal(string point)
        {
            string root = Root(); string? armed = null; var router = Router(root, p => { if (p == armed) { armed = null; throw new IOException("Controlled switch crash " + p); } });
            var completed = Complete(router, Begin(router)); string originalPath = Record(root, completed.Authority!); byte[] original = File.ReadAllBytes(originalPath); string target = sources.ManifestBytesSha256.Last(); armed = point;
            var uncertain = router.StartNextParticipantAsync(completed.Authority!, target, HumanRunOrigin.AUTOMATED_TEST, Utc, CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(uncertain.IsConfirmed, Is.False); Assert.That(uncertain.Authority, Is.Null); var reload = Router(root); var observed = reload.InspectAsync(CancellationToken.None).GetAwaiter().GetResult(); Assert.That(observed.Status, Is.Not.EqualTo(HumanStoreStatus.Missing));
            if (!observed.IsConfirmed)
            {
                Assert.That(observed.CanStartParticipant, Is.False); Assert.That(observed.Authority, Is.Null);
                observed = reload.RecoverRoutingAsync(observed.GlobalObservationToken, CancellationToken.None).GetAwaiter().GetResult();
            }
            Assert.That(observed.IsConfirmed, Is.True, observed.ErrorCode); Assert.That(File.ReadAllBytes(originalPath), Is.EqualTo(original));
            Assert.That(observed.Catalog!.ManifestBytesSha256, Is.EqualTo(point == "INDEX_MAIN_REPLACED" ? target : completed.Catalog!.ManifestBytesSha256));
        }
        [TestCase("RECOVERY_ANCHOR_COMMITTED")][TestCase("RECOVERY_INDEX_ANCHOR_COMMITTED")][TestCase("RECOVERY_ROOT_MAIN_DELETED")][TestCase("RECOVERY_ROLES_CLEARED")]
        public void CrossManifestRecoveryCrashNeverFallsBackToHistoricalParticipantOrLosesOrphanOriginals(string point)
        {
            string root = Root(); string? armed = null; var router = Router(root, p => { if (p == armed) { armed = null; throw new IOException("Controlled recovery crash " + p); } });
            var first = Complete(router, Begin(router)); string firstPath = Record(root, first.Authority!); byte[] firstBytes = File.ReadAllBytes(firstPath);
            string target = sources.ManifestBytesSha256.Last(); var second = router.StartNextParticipantAsync(first.Authority!, target, HumanRunOrigin.AUTOMATED_TEST, Utc, CancellationToken.None).GetAwaiter().GetResult(); string secondPath = Record(root, second.Authority!);
            byte[] broken = Encoding.UTF8.GetBytes("exact unknown second-participant backup bytes"); File.WriteAllBytes(secondPath + ".bak", broken); var observed = router.InspectAsync(CancellationToken.None).GetAwaiter().GetResult(); Assert.That(observed.Status, Is.EqualTo(HumanStoreStatus.RecoveryRequired));
            armed = point; Assert.That(router.RecoverRoutingAsync(observed.GlobalObservationToken, CancellationToken.None).GetAwaiter().GetResult().IsConfirmed, Is.False);
            var reload = Router(root); observed = reload.InspectAsync(CancellationToken.None).GetAwaiter().GetResult(); Assert.That(observed.Status, Is.Not.EqualTo(HumanStoreStatus.Missing));
            var recovered = reload.RecoverRoutingAsync(observed.GlobalObservationToken, CancellationToken.None).GetAwaiter().GetResult(); Assert.That(recovered.IsConfirmed, Is.True, recovered.ErrorCode);
            Assert.That(recovered.Authority!.SessionToken, Is.EqualTo(second.Authority!.SessionToken)); Assert.That(recovered.Catalog!.ManifestBytesSha256, Is.EqualTo(target)); Assert.That(File.ReadAllBytes(firstPath), Is.EqualTo(firstBytes));
            Assert.That(Directory.GetFiles(Path.Combine(Namespace(root), target, "_recovery"), "*", SearchOption.AllDirectories).Any(p => File.ReadAllBytes(p).SequenceEqual(broken)), Is.True);
        }
        [TestCase("unknown-source")][TestCase("index-version")][TestCase("record-version")]
        public void UnknownSourcesAndFutureVersionsRemainUnsupportedAndNeverDowngradeDuringRetry(string mutation)
        {
            string root = Root(); var router = Router(root); var initial = Begin(router);
            if (mutation == "unknown-source") Directory.CreateDirectory(Path.Combine(Namespace(root), new string('0', 64), Guid.NewGuid().ToString("N")));
            if (mutation == "index-version") { string path = Path.Combine(Namespace(root), "active-run.json"); File.WriteAllText(path, File.ReadAllText(path).Replace("\"humanIndexVersion\":1", "\"humanIndexVersion\":2")); }
            if (mutation == "record-version") { string path = Record(root, initial.Authority!); File.WriteAllText(path, File.ReadAllText(path).Replace("\"humanRecordVersion\":1", "\"humanRecordVersion\":2")); }
            var before = Files(root); var observed = router.InspectAsync(CancellationToken.None).GetAwaiter().GetResult(); Assert.That(observed.Status, Is.EqualTo(HumanStoreStatus.Unsupported)); Assert.That(observed.Authority, Is.Null);
            Assert.That(router.RecoverRoutingAsync(observed.GlobalObservationToken, CancellationToken.None).GetAwaiter().GetResult().Status, Is.EqualTo(HumanStoreStatus.Unsupported));
            Assert.That(router.StartNextParticipantAsync(initial.Authority!, sources.ManifestBytesSha256[1], HumanRunOrigin.AUTOMATED_TEST, Utc, CancellationToken.None).GetAwaiter().GetResult().IsConfirmed, Is.False); EqualFiles(before, root);
        }
        [Test]
        public void OriginalManifestByteCopiesCannotBeChangedThroughEitherInputOrGetter()
        {
            var original = sources.Values.First().Value; var bytes = original.GetOriginalBytes(); var copied = new HumanRoutingManifest(bytes, original.Catalog); bytes[0] ^= 1;
            var fromGetter = copied.GetOriginalBytes(); fromGetter[0] ^= 1; Assert.That(copied.GetOriginalBytes(), Is.EqualTo(original.GetOriginalBytes()));
            Assert.Throws<ArgumentException>(() => new HumanRoutingManifest(bytes, original.Catalog));
        }
        [Test]
        public void AChangedHistoricalArchiveInvalidatesTheGlobalExpectedAuthorityEvenThoughActiveRecordIsUnchanged()
        {
            string root = Root(); var router = Router(root); var first = Complete(router, Begin(router)); var next = router.StartNextParticipantAsync(first.Authority!, sources.ManifestBytesSha256.Last(), HumanRunOrigin.AUTOMATED_TEST, Utc, CancellationToken.None).GetAwaiter().GetResult(); var completed = Complete(router, next);
            string archive = Path.Combine(Namespace(root), first.Catalog!.ManifestBytesSha256, "_recovery", "controlled-original-evidence"); Directory.CreateDirectory(archive); File.WriteAllText(Path.Combine(archive, "original.txt"), "newly observed original bytes");
            var before = Files(root); var stale = router.StartNextParticipantAsync(completed.Authority!, sources.ManifestBytesSha256[0], HumanRunOrigin.AUTOMATED_TEST, Utc, CancellationToken.None).GetAwaiter().GetResult(); Assert.That(stale.Status, Is.EqualTo(HumanStoreStatus.Conflict)); EqualFiles(before, root);
            var fresh = router.InspectAsync(CancellationToken.None).GetAwaiter().GetResult(); Assert.That(fresh.IsConfirmed, Is.True); Assert.That(fresh.Authority!.RecordBytesSha256, Is.EqualTo(completed.Authority!.RecordBytesSha256)); Assert.That(fresh.GlobalObservationToken, Is.Not.EqualTo(completed.GlobalObservationToken));
        }
        [Test]
        public void UnconfirmedOrphanNeverBecomesAuthorityAndExplicitRecoveryArchivesItsExactOriginalBytes()
        {
            string root = Root(); var router = Router(root); var completed = Complete(router, Begin(router)); var target = sources.Resolve(sources.ManifestBytesSha256.Last()).Catalog; string token = Guid.NewGuid().ToString("N");
            var run = new HumanCalibrationRun(target.ManifestBytesSha256, token, HumanRunOrigin.AUTOMATED_TEST, Utc, 0, Array.Empty<HumanTrialRecord>());
            var codec = LocalSaveContractFixtures.Construct<object>("STP.Infrastructure.Persistence", "STP.Infrastructure.Persistence.HumanCalibrationCodec", Json, target);
            var original = (byte[])LocalSaveContractFixtures.Call(codec, "Encode", new HumanObservationSnapshot(1, run))!;
            string path = Path.Combine(Namespace(root), target.ManifestBytesSha256, token, "observations.json"); Directory.CreateDirectory(Path.GetDirectoryName(path)!); File.WriteAllBytes(path, original);
            var observed = router.InspectAsync(CancellationToken.None).GetAwaiter().GetResult(); Assert.That(observed.Status, Is.EqualTo(HumanStoreStatus.RecoveryRequired)); Assert.That(observed.Authority, Is.Null); Assert.That(File.ReadAllBytes(path), Is.EqualTo(original));
            var recovered = router.RecoverRoutingAsync(observed.GlobalObservationToken, CancellationToken.None).GetAwaiter().GetResult(); Assert.That(recovered.IsConfirmed, Is.True, recovered.ErrorCode); Assert.That(recovered.Authority!.SessionToken, Is.EqualTo(completed.Authority!.SessionToken)); Assert.That(File.Exists(path), Is.False);
            Assert.That(Directory.GetFiles(Path.Combine(Namespace(root), completed.Catalog!.ManifestBytesSha256, "_recovery"), "*", SearchOption.AllDirectories).Any(p => File.ReadAllBytes(p).SequenceEqual(original)), Is.True);
        }
        [Test]
        public void AHistoricalFutureWriteCannotBeIgnoredOrRetakeTheActiveParticipant()
        {
            string root = Root(); var router = Router(root); var first = Complete(router, Begin(router)); string oldPath = Record(root, first.Authority!); byte[] oldOriginal = File.ReadAllBytes(oldPath);
            var next = router.StartNextParticipantAsync(first.Authority!, sources.ManifestBytesSha256.Last(), HumanRunOrigin.AUTOMATED_TEST, Utc, CancellationToken.None).GetAwaiter().GetResult();
            var codec = LocalSaveContractFixtures.Construct<object>("STP.Infrastructure.Persistence", "STP.Infrastructure.Persistence.HumanCalibrationCodec", Json, first.Catalog!);
            byte[] future = (byte[])LocalSaveContractFixtures.Call(codec, "Encode", new HumanObservationSnapshot(first.Snapshot!.Generation + 1, first.Snapshot.Run))!; File.WriteAllBytes(oldPath + ".tmp", future);
            var observed = router.InspectAsync(CancellationToken.None).GetAwaiter().GetResult(); Assert.That(observed.Status, Is.EqualTo(HumanStoreStatus.RecoveryRequired)); Assert.That(observed.Authority, Is.Null);
            var recovered = router.RecoverRoutingAsync(observed.GlobalObservationToken, CancellationToken.None).GetAwaiter().GetResult(); Assert.That(recovered.IsConfirmed, Is.True, recovered.ErrorCode); Assert.That(recovered.Authority!.SessionToken, Is.EqualTo(next.Authority!.SessionToken));
            Assert.That(File.ReadAllBytes(oldPath), Is.EqualTo(oldOriginal)); Assert.That(File.Exists(oldPath + ".tmp"), Is.False); Assert.That(Directory.GetFiles(Path.Combine(Namespace(root), next.Catalog!.ManifestBytesSha256, "_recovery"), "*", SearchOption.AllDirectories).Any(p => File.ReadAllBytes(p).SequenceEqual(future)), Is.True);
        }
    }
}
