using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
using STP.Puzzle.Solver;

namespace STP.Tests.Application.EditMode
{
    public sealed class EndlessRuntimeSaveTests
    {
        private const string Utc = "2026-10-21T00:00:00Z";
        private sealed class RuntimeData
        {
            internal readonly ICanonicalJson Json = LocalSaveContractFixtures.Json;
            internal readonly IPublicPuzzleCatalog Public;
            internal readonly ILockedCampaignCatalog Campaign;
            internal readonly ILockedCompletionCatalog Completion;
            internal readonly CatalogSetIdentity Catalogs;
            internal readonly SaveProfileIdentity Profile = new SaveProfileIdentity(SaveProfileIdentity.MobileCandidateNamespace, "codec-runtime", CatalogUse.InternalPreview);
            internal readonly ILocalLedger Ledger;
            internal readonly LocalProgressPolicy Progress;
            internal readonly LocalEndlessPolicy Endless;
            internal readonly EndlessSearchProfile Search;
            internal readonly object Codec;
            internal readonly LocalProfileState Cleared;
            internal RuntimeData()
            {
                byte[] Bytes(string name) => File.ReadAllBytes(Path.Combine(Environment.GetEnvironmentVariable("STP_SOURCE_ROOT") ?? Directory.GetCurrentDirectory(), "Assets/StammstreckenPuzzle/UI", name));
                string bindings = Encoding.UTF8.GetString(Bytes("MobileCandidate.bindings.json"));
                string Bound(string name) => Regex.Match(bindings, "\"" + name + "\"\\s*:\\s*\"([a-f0-9]{64})\"").Groups[1].Value;
                var campaignBytes = Bytes("MobileCandidate.campaign.json");
                var completionBytes = Bytes("MobileCandidate.completion.json");
                var cosmeticBytes = Bytes("MobileCandidate.cosmetics.json");
                var endlessBytes = Bytes("MobileCandidate.endless-completion.json");
                Public = LocalSaveContractFixtures.Construct<IPublicPuzzleCatalog>("STP.Infrastructure.Content", "STP.Infrastructure.Content.MobileCandidateData", Bytes("MobileCandidate.public.json"), Bound("publicCatalogHashSha256"));
                var progress = LocalSaveContractFixtures.Construct<object>("STP.Infrastructure.Content", "STP.Infrastructure.Content.MobileCandidateProgressCatalog", Public, campaignBytes, completionBytes, Json, Bound("campaignHashSha256"), Bound("completionHashSha256"));
                Campaign = (ILockedCampaignCatalog)LocalSaveContractFixtures.Get(progress, "Campaign")!;
                Completion = (ILockedCompletionCatalog)LocalSaveContractFixtures.Get(progress, "Completion")!;
                Catalogs = (CatalogSetIdentity)LocalSaveContractFixtures.Get(progress, "CatalogSet")!;
                var cosmetics = LocalSaveContractFixtures.Construct<ILockedCosmeticsCatalog>("STP.Infrastructure.Content", "STP.Infrastructure.Content.LockedCosmeticsCatalog", cosmeticBytes, Bound("cosmeticsHashSha256"), CatalogUse.InternalPreview, Json);
                var profiles = LocalSaveContractFixtures.Construct<object>("STP.Infrastructure.Content", "STP.Infrastructure.Content.LockedEndlessProfiles", Bytes("MobileCandidate.endless-profiles.json"), Public);
                Search = (EndlessSearchProfile)LocalSaveContractFixtures.Call(profiles, "Get", "dienstbeginn")!;
                var endless = LocalSaveContractFixtures.Construct<object>("STP.Infrastructure.Content", "STP.Infrastructure.Content.LockedEndlessCompletion", endlessBytes, LocalSaveHash.Bytes(LocalSaveHash.Canonical(Json, Encoding.UTF8.GetString(endlessBytes))), Json);
                var assembly = Assembly.Load("STP.Infrastructure.Persistence");
                var bindingType = assembly.GetType("STP.Infrastructure.Persistence.RetainedCompletionRewardBinding", true)!;
                var rewards = Array.CreateInstance(bindingType, 2);
                rewards.SetValue(bindingType.GetMethod("FromWrapper")!.Invoke(null, new object[] { LocalPostClearPolicy.PolicyVersion, completionBytes, Json, Completion.Identity }), 0);
                rewards.SetValue(bindingType.GetMethod("FromDocument")!.Invoke(null, new object[] { LocalPostClearPolicy.PolicyVersion, endlessBytes, Json, CatalogUse.InternalPreview }), 1);
                var retainedType = assembly.GetType("STP.Infrastructure.Persistence.RetainedContractBindings", true)!;
                var cosmeticBindings = retainedType.GetMethod("ForCosmetics")!.Invoke(null, new object[] { cosmetics.Identity, Campaign.Identity, cosmeticBytes, campaignBytes, Public, Json });
                var retained = retainedType.GetMethod("ForRuntimeWithRewards")!.Invoke(null, new object?[] { CatalogUse.InternalPreview, cosmeticBindings, rewards })!;
                Codec = assembly.GetType("STP.Infrastructure.Persistence.LocalSaveCodec", true)!.GetMethod("ForMetagame")!.Invoke(null, new object[] { Json, Profile, Catalogs, Public, Campaign, Completion, retained })!;
                Ledger = LocalSaveContractFixtures.Construct<ILocalLedger>("STP.Infrastructure.Persistence", "STP.Infrastructure.Persistence.SaveLedgerCodec", Json);
                Progress = new LocalProgressPolicy(Public, Campaign, Completion, Json, Ledger);
                Endless = new LocalEndlessPolicy(Public, (CatalogIdentity)LocalSaveContractFixtures.Get(endless, "Identity")!, Json, Ledger);
                // Controlled test precondition: all exact current first-clear identities, values, subjects and ledger evidence.
                // This fixture is not a claim that a human played or approved the campaign.
                var empty = Progress.NewProfile(Profile, Catalogs, Utc).WithMetagame(LocalMetagameState.Empty);
                var levels = new List<LocalLevelProgress>();
                var journal = new List<LocalStoredEntry>();
                var claims = new List<LocalRewardClaim>();
                EconomyTerminal Grant(string transaction, string reason, string subject, int amount)
                {
                    var terminal = EconomyTerminal.Create(transaction, reason, subject, Completion.Identity, "reward:" + reason + ":" + subject, amount, 1, Utc);
                    journal.Add(Ledger.Next(empty.LedgerCheckpoint, journal, terminal));
                    return terminal;
                }
                foreach (var puzzle in Public.Puzzles)
                {
                    Assert.That(Completion.TryGet(puzzle.Identity, out var values), Is.True);
                    var attempt = Guid.NewGuid();
                    var terminal = Grant("level:" + puzzle.Identity.PuzzleId + ":first-clear", "LEVEL_FIRST_CLEAR", puzzle.Identity.PuzzleId, values!.FirstClear);
                    levels.Add(LocalLevelProgress.Create(puzzle.Identity, LocalFirstClear.Create(attempt, "puzzle-solved:" + attempt.ToString("D"), Utc, 10, terminal), 1, 10, null, true, null, null));
                }
                var cache = Progress.Cache(levels);
                foreach (var subject in cache.Subjects.Where(s => s.Kind == "ROUTE" || s.Kind == "SECTION"))
                {
                    var member = Campaign.Entries.First(e => (subject.Kind == "ROUTE" ? e.RouteId : e.SectionId) == subject.SubjectId);
                    Completion.TryGet(member.Puzzle, out var values);
                    bool route = subject.Kind == "ROUTE";
                    var terminal = Grant((route ? "route:" : "section:") + subject.SubjectId + ":clear", route ? "ROUTE_CLEAR" : "SECTION_CLEAR", subject.SubjectId, route ? values!.RouteClear : values!.SectionClear);
                    claims.Add(CampaignProgressTerminal.Create(subject.Kind, subject.SubjectId, terminal));
                }
                Cleared = empty.WithProgress(levels, empty.LedgerCheckpoint, journal, cache, empty.Drafts, claims);
                Progress.Validate(Cleared);
                Assert.That(Endless.IsUnlocked(Cleared), Is.True);
            }
            internal SaveSnapshot<LocalProfileState> Snapshot(LocalProfileState state, long generation) => new SaveSnapshot<LocalProfileState>(Profile, Catalogs.AggregateCatalog, generation, state);
            internal byte[] Encode(LocalProfileState state, long generation) => (byte[])LocalSaveContractFixtures.Call(Codec, "Encode", Snapshot(state, generation), Utc, "test", "endless-runtime-codec")!;
            internal object Decode(byte[] bytes) => LocalSaveContractFixtures.Call(Codec, "Decode", bytes)!;
            internal SaveSnapshot<LocalProfileState> RoundTrip(LocalProfileState state, long generation)
            {
                var decoded = Decode(Encode(state, generation));
                Assert.That(LocalSaveContractFixtures.Get(decoded, "Status")!.ToString(), Is.EqualTo("Valid"), LocalSaveContractFixtures.Get(decoded, "ErrorCode")?.ToString());
                return (SaveSnapshot<LocalProfileState>)LocalSaveContractFixtures.Get(decoded, "Snapshot")!;
            }
            internal LocalProfileState RetiredFirst()
            {
                var reserved = RoundTrip(Endless.Reserve(Cleared, Search, "1"), 2);
                var candidate = Endless.GenerateAsync(reserved, reserved.Payload.Endless.OpenEndless.Single().Id, CancellationToken.None).GetAwaiter().GetResult()!;
                Assert.That(candidate, Is.Not.Null);
                var active = RoundTrip(Endless.Promote(reserved.Payload, candidate, Guid.NewGuid()), 3).Payload;
                var draft = active.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single();
                var completed = RoundTrip(Endless.Complete(active, Solved(draft), 4, Utc), 4).Payload;
                return RoundTrip(Endless.SkipReward(completed, draft.Id), 5).Payload;
            }
        }

        private static PuzzleSessionState Solved(ActiveEndlessDraft draft)
        {
            var definition = draft.PublicPuzzleInput.ToDomainDefinition();
            var verified = PuzzleSolver.Solve(definition, new SolverLimits(1000000, 5000));
            Assert.That(verified.Classification, Is.EqualTo(SolverClassification.UNIQUE));
            var state = draft.Checkpoint.Restore(definition, draft.Id, draft.PublicPuzzleHash.Profile, draft.PublicPuzzleHash.Sha256).Value!;
            Direction Toward(CellCoordinate from, CellCoordinate to) => to.Y < from.Y ? Direction.N : to.X > from.X ? Direction.E : to.Y > from.Y ? Direction.S : Direction.W;
            for (int i = 0; i < verified.Path.Count; i++)
            {
                var cell = verified.Path[i];
                var first = i == 0 ? definition.A.Side : Toward(cell, verified.Path[i - 1]);
                var second = i == verified.Path.Count - 1 ? definition.B.Side : Toward(cell, verified.Path[i + 1]);
                int ports = (1 << (int)first) | (1 << (int)second);
                var shape = Enumerable.Range(0, 6).Select(n => (TrackShape)n).Single(s => TrackGeometry.Ports(s) == ports);
                var content = (CellContent)Enum.Parse(typeof(CellContent), "TRACK_" + shape);
                state = PuzzleCommandHandler.Handle(state, new ApplyCellContent(Guid.NewGuid(), state.Revision, cell, content), i + 1).State!;
            }
            Assert.That(state.Phase, Is.EqualTo(SessionPhase.SolvedPendingCommit));
            return state;
        }

        [Test]
        public void CurrentRuntimeCodecRoundTripsRetiredFirstThenSecondReservedActiveAndCompletedDescriptors()
        {
            var data = new RuntimeData();
            var retired = data.RetiredFirst();
            Assert.That(retired.Endless.OpenEndless, Is.Empty);
            var first = retired.Endless.RecentPublicInputs.Single();
            var reserved = data.RoundTrip(data.Endless.Reserve(retired, data.Search, "2"), 6);
            var record = reserved.Payload.Endless.OpenEndless.OfType<ReservedEndless>().Single();
            Assert.That(((SearchGeneratorParameters)record.Descriptor.Parameters).RecentPublicInputs.Single().Identity, Is.EqualTo(first.Identity));
            var candidate = data.Endless.GenerateAsync(reserved, record.Id, CancellationToken.None).GetAwaiter().GetResult()!;
            Assert.That(candidate, Is.Not.Null);
            var active = data.RoundTrip(data.Endless.Promote(reserved.Payload, candidate, Guid.NewGuid()), 7).Payload;
            var draft = active.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single();
            var completed = data.RoundTrip(data.Endless.Complete(active, Solved(draft), 8, Utc), 8).Payload;
            var retiredSecond = data.RoundTrip(data.Endless.SkipReward(completed, draft.Id), 9).Payload;
            Assert.That(retiredSecond.Endless.HighestReservedOrdinal, Is.EqualTo("2"));
            Assert.That(retiredSecond.Endless.OpenEndless, Is.Empty);
            Assert.That(retiredSecond.Endless.RecentPublicInputs.Count, Is.EqualTo(2));
            Assert.That(data.Ledger.Validate(retiredSecond), Is.EqualTo(data.Ledger.Validate(data.Cleared) + 30));
        }

        [TestCase("forged-hash", "SAVE_ENDLESS_REFERENCE_HASH_MISMATCH")]
        [TestCase("missing-hash", "SAVE_FIELDS_INVALID")]
        [TestCase("unexpected-field", "SAVE_FIELDS_INVALID")]
        [TestCase("wrong-profile", "SAVE_ENDLESS_REFERENCE_HASH_PROFILE_UNSUPPORTED")]
        [TestCase("wrong-parameter-hash", "SAVE_ENDLESS_ID_MISMATCH")]
        public void SecondDescriptorRejectsMutatedCapturedRecentOrParameterHash(string mutation, string error)
        {
            var data = new RuntimeData();
            var retired = data.RetiredFirst();
            var state = data.Endless.Reserve(retired, data.Search, "2");
            var record = state.Endless.OpenEndless.Single();
            var parameters = (SearchGeneratorParameters)record.Descriptor.Parameters;
            var recent = parameters.RecentPublicInputs.Single();
            string Canonical(string text) => Encoding.UTF8.GetString(LocalSaveHash.Canonical(data.Json, text));
            string parameterText = Canonical(parameters.Projection());
            string originalRecent = Canonical(EndlessProjection.PublicInput(recent, true));
            string changed = mutation == "forged-hash" ? originalRecent.Replace(recent.Identity.HashSha256, new string('f', 64)) :
                mutation == "missing-hash" ? Canonical(EndlessProjection.PublicInput(recent, false)) :
                mutation == "unexpected-field" ? Canonical(originalRecent.Insert(1, "\"unexpected\":true,")) :
                originalRecent.Replace(recent.Identity.HashProfile, "STP-SOLUTION-JCS-1");
            string payload = Encoding.UTF8.GetString((byte[])LocalSaveContractFixtures.Call(data.Codec, "PayloadBytes", state)!);
            string mutated = mutation == "wrong-parameter-hash" ? payload.Replace("\"parameterHashSha256\":\"" + record.Descriptor.ParameterHashSha256 + "\"", "\"parameterHashSha256\":\"" + new string('f', 64) + "\"") :
                payload.Replace(parameterText, parameterText.Replace(originalRecent, changed));
            Assert.That(mutated, Is.Not.EqualTo(payload));
            string envelope = Encoding.UTF8.GetString(data.Encode(state, 6));
            string before = LocalSaveHash.Bytes(Encoding.UTF8.GetBytes(payload));
            string after = LocalSaveHash.Bytes(Encoding.UTF8.GetBytes(mutated));
            var decoded = data.Decode(Encoding.UTF8.GetBytes(envelope.Replace(payload, mutated).Replace("\"payloadSha256\":\"" + before + "\"", "\"payloadSha256\":\"" + after + "\"")));
            Assert.That(LocalSaveContractFixtures.Get(decoded, "Status")!.ToString(), Is.Not.EqualTo("Valid"));
            Assert.That(LocalSaveContractFixtures.Get(decoded, "ErrorCode"), Is.EqualTo(error));
            Assert.That(LocalSaveContractFixtures.Get(decoded, "Snapshot"), Is.Null);
        }

        private sealed class QueueDispatcher : ILocalSaveDispatcher
        {
            private readonly int owner = Thread.CurrentThread.ManagedThreadId;
            private readonly ConcurrentQueue<(Action Action, TaskCompletionSource<bool> Done)> queue = new ConcurrentQueue<(Action, TaskCompletionSource<bool>)>();
            public Task InvokeAsync(Action action)
            {
                if (Thread.CurrentThread.ManagedThreadId == owner) { action(); return Task.CompletedTask; }
                var done = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                queue.Enqueue((action, done)); return done.Task;
            }
            internal void Drain() { while (queue.TryDequeue(out var next)) { try { next.Action(); next.Done.TrySetResult(true); } catch (Exception e) { next.Done.TrySetException(e); } } }
            internal void Pump(Task task)
            {
                var watch = Stopwatch.StartNew();
                while (!task.IsCompleted) { Drain(); if (watch.ElapsedMilliseconds > 60000) Assert.Fail("Physical endless save did not finish while owner dispatcher was pumped"); Thread.Sleep(1); }
                Drain(); task.GetAwaiter().GetResult();
            }
            internal T Pump<T>(Task<T> task) { Pump((Task)task); return task.GetAwaiter().GetResult(); }
        }

        [Test]
        public void ActualAtomicRepositoryCompletesTwoEndlessGamesThroughSameRuntimeCodecAndReloadsThem()
        {
            var data = new RuntimeData();
            string privateRoot = Path.Combine(Environment.GetEnvironmentVariable("STP_SOURCE_ROOT") ?? Directory.GetCurrentDirectory(), ".utmp", "e1c-" + Guid.NewGuid().ToString("N").Substring(0, 8));
            var assembly = Assembly.Load("STP.Infrastructure.Persistence");
            var filesType = assembly.GetType("STP.Infrastructure.Persistence.SaveFileSystem", true)!;
            filesType.GetMethod("ProvisionPrivateRoot")!.Invoke(null, new object[] { privateRoot });
            var files = Activator.CreateInstance(filesType, new object?[] { privateRoot, data.Profile, data.Catalogs.AggregateCatalog, null })!;
            var clock = new LocalSaveContractFixtures.Clock();
            var repository = LocalSaveContractFixtures.Construct<ILocalProfileRepository>("STP.Infrastructure.Persistence", "STP.Infrastructure.Persistence.AtomicSaveRepository", files, data.Codec, clock, "test", "actual-endless-codec");
            Assert.That(repository.CreateProfileAsync(data.Cleared, CancellationToken.None).GetAwaiter().GetResult().Succeeded, Is.True);
            var dispatcher = new QueueDispatcher();
            var use = new PersistedPuzzleSession(repository, data.Public, data.Progress, data.Profile, data.Catalogs, clock, dispatcher);
            use.ConfigureEndless(data.Endless);
            using var saved = SavedPublic240ReviewSession.ForMobileCandidate(use, new AutosaveCoordinator(use), data.Public, clock, dispatcher, data.Profile);
            dispatcher.Pump(saved.InitializeAsync(CancellationToken.None));
            string? prior = null;
            for (int ordinal = 1; ordinal <= 2; ordinal++)
            {
                var start = dispatcher.Pump(saved.StartEndlessAsync(data.Endless, data.Search, ordinal.ToString(System.Globalization.CultureInfo.InvariantCulture), CancellationToken.None));
                Assert.That(start.Succeeded, Is.True, start.ErrorCode + " at ordinal " + ordinal);
                var draft = start.Snapshot!.Payload.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single();
                if (prior != null) Assert.That(((SearchGeneratorParameters)draft.Descriptor.Parameters).RecentPublicInputs.Single().Identity.PuzzleId, Is.EqualTo(prior));
                var solved = Solved(draft); var view = saved.PuzzleView!;
                for (int y = 0; y < view.Height; y++) for (int x = 0; x < view.Width; x++)
                {
                    var content = solved.Cells[y * view.Width + x]; if (content < CellContent.TRACK_NS) continue;
                    var tool = (DevelopmentTool)Enum.Parse(typeof(DevelopmentTool), "Track" + content.ToString().Substring(6));
                    Assert.That(view.Apply(x, y, tool, ++clock.MonotonicMilliseconds), Is.EqualTo(DevelopmentCommandStatus.Applied));
                }
                dispatcher.Pump(saved.TickAutosaveAsync(CancellationToken.None));
                Assert.That(use.CanPresentResults, Is.True);
                Assert.That(saved.BeginTrainRide(view.AttemptId), Is.EqualTo(DevelopmentCommandStatus.Applied));
                Assert.That(view.CompleteTrainRide(view.AttemptId), Is.EqualTo(DevelopmentCommandStatus.Applied));
                var skipped = dispatcher.Pump(saved.RunProfileActionAsync((session, ct) => session.SkipEndlessRewardAsync(data.Endless, draft.Id, ct), CancellationToken.None));
                Assert.That(skipped.Succeeded, Is.True, skipped.ErrorCode);
                Assert.That(saved.ConfirmedSnapshot!.Payload.Endless.OpenEndless, Is.Empty);
                prior = draft.Id;
            }
            var loaded = repository.LoadAsync(CancellationToken.None).GetAwaiter().GetResult();
            Assert.That(loaded.Succeeded, Is.True, loaded.ErrorCode);
            Assert.That(loaded.Snapshot!.Payload.Endless.HighestReservedOrdinal, Is.EqualTo("2"));
            Assert.That(loaded.Snapshot.Payload.Endless.RecentPublicInputs.Count, Is.EqualTo(2));
            Assert.That(data.Ledger.Validate(loaded.Snapshot.Payload), Is.EqualTo(data.Ledger.Validate(data.Cleared) + 30));
            TestContext.WriteLine("Physical runtime codec evidence: " + privateRoot + "; public corpus " + data.Public.Identity.HashSha256);
        }
    }
}
