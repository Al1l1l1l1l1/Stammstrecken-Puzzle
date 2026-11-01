using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;
using STP.Puzzle.Domain;

namespace STP.Tests.Application.EditMode
{
    public sealed class LocalMetagamePolicyTests
    {
        private const string Utc = "2026-10-21T00:00:00Z";
        private sealed class Random : ILocalDailyRandom { internal int Value; internal int Calls; public int Next100() { Calls++; return Value; } }
        private sealed class Fixture
        {
            internal readonly LocalSaveContractFixtures.Setup Saves;
            internal readonly LocalMetagamePolicy Policy;
            internal Fixture(bool times=false) { Saves=times?new LocalSaveContractFixtures.Setup(10000,5000):new LocalSaveContractFixtures.Setup();Policy = new LocalMetagamePolicy(Saves.Puzzles, Saves.CompletionId, Saves.Ledger); }
            internal PublicPuzzleInput Input => Saves.Puzzles.Puzzles[0];
            internal LocalProfileState Empty => Saves.Empty.WithMetagame(LocalMetagameState.Empty);
            internal LocalProfileState WithDraft()
            {
                var state = Empty; var input = Input; var session = PuzzleSessionState.Create(input.ToDomainDefinition(), input.Identity.PuzzleId, input.Identity.HashProfile, input.Identity.HashSha256, Guid.NewGuid(), SessionMode.FIRST_RUN, 0, null, null).Value!;
                return state.WithProgress(state.Levels, state.LedgerCheckpoint, state.EconomyJournal, state.SeasonProgressCache, new[] { LocalDraft.Create(input.Identity, Utc, PuzzleSessionCheckpoint.Export(session, 0)) }, state.RewardClaims);
            }
            internal SafeHintDisplayReceipt Receipt(LocalProfileState state)
            {
                var hint = SafePuzzleHints.Prepare(Input, state.Drafts.Single().Checkpoint);
                Assert.That(hint, Is.Not.Null, "fixture must yield an objective single-cell root conclusion"); return hint!.AcknowledgeDisplayed();
            }
            internal object Codec(bool v3, bool legacyLocale = false)
            {
                var type = Assembly.Load("STP.Infrastructure.Persistence").GetType("STP.Infrastructure.Persistence.LocalSaveCodec", true)!;
                object? retained = null;
                if(legacyLocale)
                {
                    var retainedType=type.Assembly.GetType("STP.Infrastructure.Persistence.RetainedContractBindings",true)!;
                    var empty=retainedType.GetMethod("Empty")!.Invoke(null,new object[]{CatalogUse.ContractTest})!;
                    object Get(string name)=>LocalSaveContractFixtures.Get(empty,name)!;
                    retained=Activator.CreateInstance(retainedType,new object[]{CatalogUse.ContractTest,Get("HintPolicies"),Get("DailyPolicies"),Get("AccessibilityRevisions"),Get("Privacy"),Get("Iap"),Get("Legacy"),Get("Cosmetics"),Get("RewardPolicies"),Get("GeneratorGrammars"),new[]{"de-DE"},Get("AudioGroups"),Get("CompletionRewards"),Get("HistoricalProgress")})!;
                }
                if (v3) return type.GetMethod("ForMetagame")!.Invoke(null, new object?[] { LocalSaveContractFixtures.Json, Saves.Profile, Saves.Catalogs, Saves.Puzzles, Saves.Campaign, Saves.Completion, retained })!;
                return Activator.CreateInstance(type, new object?[] { LocalSaveContractFixtures.Json, Saves.Profile, Saves.Catalogs, Saves.Puzzles, Saves.Campaign, Saves.Completion, null, null, null })!;
            }
            internal LocalProfileState RoundTrip(LocalProfileState state, long generation = 5)
            {
                var codec = Codec(true); var snapshot = new SaveSnapshot<LocalProfileState>(Saves.Profile, Saves.Aggregate, generation, state);
                var bytes = (byte[])LocalSaveContractFixtures.Call(codec, "Encode", snapshot, Utc, "test", "test")!;
                var result = LocalSaveContractFixtures.Call(codec, "Decode", bytes)!;
                Assert.That(LocalSaveContractFixtures.Get(result, "Status")!.ToString(), Is.EqualTo("Valid"));
                return ((SaveSnapshot<LocalProfileState>)LocalSaveContractFixtures.Get(result, "Snapshot")!).Payload;
            }
        }
        private static AdRewardEvidence Evidence(LocalMetagameReward reward, string? operation = null)
        { string id = operation ?? reward.OperationId; return new AdRewardEvidence(new AdRewardReservation(reward.ClaimId, id, reward.ReservationGeneration), id); }
        [Test]
        public void OnlyAcknowledgedSafeDisplaySpendsOneCreditAndSameReceiptIsIdempotent()
        {
            var f = new Fixture(); var source = f.WithDraft(); var receipt = f.Receipt(source); var staleOther = f.Receipt(source);
            Assert.That(f.Policy.HintCredits(source, f.Input.Identity), Is.EqualTo(3));
            var result = f.Policy.ConsumeHintDisplay(source, receipt);
            Assert.That(f.Policy.HintCredits(result, f.Input.Identity), Is.EqualTo(2));
            Assert.That(result.Drafts.Single().Checkpoint.HintCount, Is.EqualTo(1)); Assert.That(result.Drafts.Single().Checkpoint.TimerStarted, Is.False);
            Assert.That(result.Drafts.Single().Checkpoint.Cells, Is.EqualTo(source.Drafts.Single().Checkpoint.Cells));
            Assert.That(f.Policy.ConsumeHintDisplay(result, receipt), Is.SameAs(result));
            Assert.Throws<ArgumentException>(() => f.Policy.ConsumeHintDisplay(result, staleOther));
            Assert.Throws<ArgumentException>(() => f.Policy.ConsumeHintDisplay(source, f.Receipt(f.WithDraft())));
            Assert.That(f.RoundTrip(result).Drafts.Single().Checkpoint.HintCount, Is.EqualTo(1));
        }
        [Test]
        public void ThreeRegularUsesAreIdentityBoundAndVideoCreditRequiresExactConfirmedCallback()
        {
            var f = new Fixture(); var source = f.WithDraft();
            Assert.Throws<ArgumentException>(() => f.Policy.ReserveHintVideo(source, f.Input.Identity, "video", 2));
            var exhausted = new LocalHintRecord(f.Input.Identity, 3, 0, null, Enumerable.Range(0, 3).Select(_ => Guid.NewGuid().ToString("D")).ToArray());
            source = source.WithMetagame(new LocalMetagameState(new[] { exhausted }, Array.Empty<LocalDailyClaim>()));
            var reserved = f.Policy.ReserveHintVideo(source, f.Input.Identity, "video", 2); var reward = reserved.Metagame!.Hints.Single().Reward!;
            Assert.That(f.Policy.HintCredits(reserved, f.Input.Identity), Is.Zero);
            Assert.That(f.Policy.ConfirmHintVideo(reserved, f.Input.Identity, Evidence(reward, "wrong")), Is.SameAs(reserved));
            var confirmed = f.Policy.ConfirmHintVideo(reserved, f.Input.Identity, Evidence(reward));
            Assert.That(f.Policy.HintCredits(confirmed, f.Input.Identity), Is.EqualTo(1)); Assert.That(confirmed.Metagame!.Hints.Single().RewardedConsumed, Is.Zero);
            Assert.That(f.Policy.ConfirmHintVideo(confirmed, f.Input.Identity, Evidence(reward)), Is.SameAs(confirmed));
            var used = f.Policy.ConsumeHintDisplay(confirmed, f.Receipt(confirmed));
            Assert.That(used.Metagame!.Hints.Single().RewardedConsumed, Is.EqualTo(1)); Assert.That(f.Policy.HintCredits(used, f.Input.Identity), Is.Zero);
            Assert.Throws<ArgumentException>(() => f.Policy.ReserveHintVideo(used, f.Input.Identity, "second-video", 3));
            var terminal = new PublicPuzzleIdentity("E1-" + new string('a', 64), PublicPuzzleIdentity.SemanticHashProfile, new string('b', 64));
            Assert.Throws<ArgumentException>(() => f.Policy.HintCredits(f.Empty, terminal));
        }
        [TestCase(0,10)][TestCase(59,10)][TestCase(60,15)][TestCase(89,15)][TestCase(90,25)][TestCase(99,25)]
        public void DailyOutcomePersistsBeforeGrantAndOneFreeSlotCannotRerollOrGrantTwice(int roll, int amount)
        {
            var f = new Fixture(); var random = new Random { Value = roll }; var source = f.Empty;
            var reserved = f.Policy.ReserveDaily(source, "2026-10-21", false, "unused-free", 2, random);
            Assert.That(random.Calls, Is.EqualTo(1)); Assert.That(f.Saves.Ledger.Validate(reserved), Is.Zero);
            var recovered = f.RoundTrip(reserved); var otherRandom = new Random { Value = (roll + 51) % 100 };
            Assert.That(f.Policy.ReserveDaily(recovered, "2026-10-21", false, "unused", 6, otherRandom), Is.SameAs(recovered)); Assert.That(otherRandom.Calls, Is.Zero);
            var granted = f.Policy.CommitFreeDaily(recovered, "2026-10-21", 6, Utc);
            Assert.That(f.Saves.Ledger.Validate(granted), Is.EqualTo(amount));
            Assert.That(f.Policy.CommitFreeDaily(granted, "2026-10-21", 7, Utc), Is.SameAs(granted));
            Assert.That(f.RoundTrip(granted, 6).Metagame!.Daily.Single().Terminal!.Amount, Is.EqualTo(amount));
        }
        [Test]
        public void RewardedDailyKeepsOutcomeAcrossNoRewardRetryAndIgnoresReleasedLateCallbacks()
        {
            var f = new Fixture(); var random = new Random { Value = 95 };
            var first = f.Policy.ReserveDaily(f.Empty, "2026-10-21", true, "one", 2, random); var old = first.Metagame!.Daily.Single().Reward!;
            var closed = f.Policy.ResolveDailyVideo(first, Evidence(old).Reservation, true);
            Assert.That(f.Policy.ConfirmDailyVideo(closed, Evidence(old), 3, Utc), Is.SameAs(closed));
            random.Value = 0; var second = f.Policy.ReserveDaily(closed, "2026-10-21", true, "two", 3, random); var current = second.Metagame!.Daily.Single().Reward!;
            Assert.That(random.Calls, Is.EqualTo(1)); Assert.That(second.Metagame.Daily.Single().Roll, Is.EqualTo(95));
            Assert.That(f.Policy.ConfirmDailyVideo(second, Evidence(old), 4, Utc), Is.SameAs(second));
            var uncertain = f.Policy.ResolveDailyVideo(second, Evidence(current).Reservation, false);
            Assert.That(uncertain.Metagame!.Daily.Single().Reward!.State, Is.EqualTo("RECONCILIATION_REQUIRED"));
            var granted = f.Policy.ConfirmDailyVideo(uncertain, Evidence(current), 4, Utc);
            Assert.That(f.Saves.Ledger.Validate(granted), Is.EqualTo(25));
            Assert.That(f.Policy.ConfirmDailyVideo(granted, Evidence(current), 5, Utc), Is.SameAs(granted));
            Assert.That(f.RoundTrip(granted, 4).Metagame!.Daily.Single().Terminal!.Amount, Is.EqualTo(25));
        }
        [Test]
        public void ClockRollbackAllowsAnotherLocalDateWithoutLosingExistingClaimsAndTwentyOpenSlotsAreBounded()
        {
            var f = new Fixture(); var random = new Random(); var state = f.Policy.ReserveDaily(f.Empty, "2026-10-22", false, "unused", 2, random);
            state = f.Policy.ReserveDaily(state, "2026-10-20", false, "unused", 3, random);
            Assert.That(state.Metagame!.Daily.Select(c => c.Date), Is.EquivalentTo(new[] { "2026-10-22", "2026-10-20" }));
            state = f.Empty;
            for (int i = 0; i < 20; i++) state = f.Policy.ReserveDaily(state, new DateTime(2026, 1, 1).AddDays(i).ToString("yyyy-MM-dd"), false, "unused", i + 2, random);
            Assert.Throws<ArgumentException>(() => f.Policy.ReserveDaily(state, "2026-02-01", false, "unused", 22, random));
            Assert.That(state.Metagame!.Daily.Count, Is.EqualTo(20)); Assert.That(f.Saves.Ledger.Validate(state), Is.Zero);
        }
        [Test]
        public async Task FailedHintCommitLeavesBothSessionAndCreditsUntouchedAndCasRejectsStaleRetry()
        {
            var f = new Fixture(); var state = f.WithDraft(); var receipt = f.Receipt(state); var snapshot = new SaveSnapshot<LocalProfileState>(f.Saves.Profile, f.Saves.Aggregate, 1, state);
            var repository = new LocalSaveContractFixtures.Repository { Loaded = SaveLoadResult<LocalProfileState>.Success(snapshot), OnCommit = (s, g, c) => Task.FromResult(SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed, "TEST_DISK_FULL")) };
            var service = new LocalMetagameTransactions(repository, f.Policy, new Random());
            Assert.That((await service.ConsumeHintAsync(receipt, 1, CancellationToken.None)).Succeeded, Is.False);
            Assert.That(repository.Loaded.Snapshot!.Payload.Drafts.Single().Checkpoint.HintCount, Is.Zero); Assert.That(f.Policy.HintCredits(repository.Loaded.Snapshot.Payload, f.Input.Identity), Is.EqualTo(3));
            Assert.That(repository.LastOffered!.Payload.Drafts.Single().Checkpoint.HintCount, Is.EqualTo(1)); Assert.That(f.Policy.HintCredits(repository.LastOffered.Payload, f.Input.Identity), Is.EqualTo(2));
            repository.OnCommit = (s, g, c) => { repository.Loaded = SaveLoadResult<LocalProfileState>.Success(s); return Task.FromResult(SaveCommitResult<LocalProfileState>.Success(s)); };
            Assert.That((await service.ConsumeHintAsync(receipt, 1, CancellationToken.None)).Succeeded, Is.True);
            Assert.That((await service.ConsumeHintAsync(receipt, 1, CancellationToken.None)).Status, Is.EqualTo(SaveCommitStatus.Conflict));
            int writes = repository.Commits; Assert.That((await service.ConsumeHintAsync(receipt, 2, CancellationToken.None)).Succeeded, Is.True); Assert.That(repository.Commits, Is.EqualTo(writes));
        }
        [Test]
        public void V2NeutralMigrationMatchesIndependentNodeGoldenAndOldWriterRejectsV3()
        {
            var f = new Fixture(); string root = Environment.GetEnvironmentVariable("STP_SOURCE_ROOT") ?? Directory.GetCurrentDirectory();
            var golden = ContractJson.Parse(File.ReadAllText(Path.Combine(root, "Assets/StammstreckenPuzzle/Tests/EditMode/Persistence/save-v3-neutral-golden.json")));
            var codec = f.Codec(true, true); var result = LocalSaveContractFixtures.Call(codec, "Decode", Encoding.UTF8.GetBytes(golden.Get("sourceEnvelopeUtf8").Text))!;
            Assert.That(LocalSaveContractFixtures.Get(result, "Status")!.ToString(), Is.EqualTo("Valid"));
            var snapshot = (SaveSnapshot<LocalProfileState>)LocalSaveContractFixtures.Get(result, "Snapshot")!;
            var payload = (byte[])LocalSaveContractFixtures.Call(codec, "PayloadBytes", snapshot.Payload)!;
            Assert.That(Encoding.UTF8.GetString(payload), Is.EqualTo(golden.Get("expectedPayloadUtf8").Text));
            Assert.That(LocalSaveHash.Bytes(payload), Is.EqualTo("893c3a5070d262be18af379613ad539c72b5d1df60ad2d0c6bb7c9d99414bc48"));
            var v3 = (byte[])LocalSaveContractFixtures.Call(codec, "Encode", snapshot, Utc, "test", "test")!;
            var legacy = LocalSaveContractFixtures.Call(f.Codec(false), "Decode", v3)!;
            Assert.That(LocalSaveContractFixtures.Get(legacy, "Status")!.ToString(), Is.EqualTo("Unsupported"));
            var blocked = LocalSaveContractFixtures.Call(codec, "Decode", Encoding.UTF8.GetBytes(golden.Get("blockedSourceEnvelopeUtf8").Text))!;
            Assert.That(LocalSaveContractFixtures.Get(blocked, "Status")!.ToString(), Is.EqualTo("Unsupported")); Assert.That(LocalSaveContractFixtures.Get(blocked, "ErrorCode"), Is.EqualTo("SAVE_MIGRATION_NEEDS_DECISION"));
        }
        [Test]
        public void BoundedDailyRetentionKeepsBalanceAndLatestClaimsAfterCheckpointCompaction()
        {
            var f=new Fixture();var state=f.Empty;
            var claims=Enumerable.Range(0,768).Select(i=>
            {
                string date=new DateTime(2020,1,1).AddDays(i).ToString("yyyy-MM-dd");string id="daily:v1:"+date+":FREE";
                return new LocalDailyClaim(date,"FREE",0,null,EconomyTerminal.Create(id,"DAILY_PATIENCE",date+":FREE",f.Saves.CompletionId,id,10,i+1,Utc));
            }).ToArray();
            state=state.WithProgress(state.Levels,LocalLedgerCheckpoint.Create(768,7680,new string('a',64),768,Array.Empty<LocalStoredEntry>()),Array.Empty<LocalStoredEntry>(),state.SeasonProgressCache,state.Drafts,state.RewardClaims).WithMetagame(new LocalMetagameState(Array.Empty<LocalHintRecord>(),claims));
            Assert.That(f.Saves.Ledger.Validate(state),Is.EqualTo(7680));
            var reserved=f.Policy.ReserveDaily(state,"2026-10-21",false,"unused",769,new Random());
            Assert.That(reserved.Metagame!.Daily.Count,Is.EqualTo(65));Assert.That(reserved.Metagame.Daily.Any(c=>c.Date==claims.Last().Date),Is.True);
            Assert.That(state.Metagame!.Daily.Count,Is.EqualTo(768));
            var granted=f.Policy.CommitFreeDaily(reserved,"2026-10-21",769,Utc);
            Assert.That(f.Saves.Ledger.Validate(granted),Is.EqualTo(7690));Assert.That(f.RoundTrip(granted,769).Metagame!.Daily.Count,Is.EqualTo(65));
        }
        [Test]
        public async Task LiveAttemptHintCommitUpdatesSessionAndCreditTogetherAndAllowsKnownWriteFailureRetry()
        {
            var f=new Fixture();var initial=new SaveSnapshot<LocalProfileState>(f.Saves.Profile,f.Saves.Aggregate,1,f.Empty);
            var repository=new LocalSaveContractFixtures.Repository{Loaded=SaveLoadResult<LocalProfileState>.Success(initial,f.Saves.Observe())};
            var clock=new LocalSaveContractFixtures.Clock();var use=new PersistedPuzzleSession(repository,f.Saves.Puzzles,f.Saves.Policy,f.Saves.Profile,f.Saves.Catalogs,clock,new LocalSaveContractFixtures.Dispatcher());
            await use.LoadAsync(CancellationToken.None);use.Start(f.Input.Identity,Guid.NewGuid());
            var offer=await use.PrepareHintAsync(f.Policy,CancellationToken.None);Assert.That(offer,Is.Not.Null);var receipt=offer!.AcknowledgeDisplayed();
            Assert.That(offer.AcknowledgeDisplayed().DisplayId,Is.EqualTo(receipt.DisplayId));
            repository.OnCommit=(s,g,c)=>Task.FromResult(SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed,"TEST_DISK_FULL"));
            Assert.That((await use.CommitHintDisplayAsync(f.Policy,receipt,CancellationToken.None)).Succeeded,Is.False);
            Assert.That(use.Session!.HintCount,Is.Zero);Assert.That(f.Policy.HintCredits(use.Snapshot!.Payload,f.Input.Identity),Is.EqualTo(3));Assert.That(use.CanMutate,Is.True);
            repository.OnCommit=(s,g,c)=>Task.FromResult(SaveCommitResult<LocalProfileState>.Success(s));
            Assert.That((await use.CommitHintDisplayAsync(f.Policy,receipt,CancellationToken.None)).Succeeded,Is.True);
            Assert.That(use.Session.HintCount,Is.EqualTo(1));Assert.That(use.Session.TimerStarted,Is.False);Assert.That(f.Policy.HintCredits(use.Snapshot!.Payload,f.Input.Identity),Is.EqualTo(2));
            var cell=CellCoordinate.Create(use.Session.Definition.Grid,offer.X,offer.Y).Value!;
            use.Handle(new ApplyCellContent(Guid.NewGuid(),use.Session.Revision,cell,offer.Content),0);
            long revision=use.Session.Revision;int writes=repository.Commits;
            Assert.That((await use.CommitHintDisplayAsync(f.Policy,receipt,CancellationToken.None)).Succeeded,Is.True);
            Assert.That(repository.Commits,Is.EqualTo(writes));Assert.That(use.Session.Revision,Is.EqualTo(revision));Assert.That(use.Session.HintCount,Is.EqualTo(1));
        }
        [Test]
        public void EarlierHintSurvivesRestartForDirectSolvedButNeverChangesTimeStarsOrReward()
        {
            var f=new Fixture(true);var hint=new LocalHintRecord(f.Input.Identity,1,0,null,new[]{Guid.NewGuid().ToString("D")});
            var source=f.Empty.WithMetagame(new LocalMetagameState(new[]{hint},Array.Empty<LocalDailyClaim>()));
            var result=f.Saves.Complete(source,f.Saves.Solved(0),2);
            Assert.That(result.State.Levels.Single().DirectSolved,Is.False);
            Assert.That(result.State.Levels.Single().StarsHighWater,Is.EqualTo(3));Assert.That(result.RewardPatience,Is.EqualTo(30));
            Assert.That(result.State.Metagame!.Hints.Single().RegularConsumed,Is.EqualTo(1));
        }
    }
}
