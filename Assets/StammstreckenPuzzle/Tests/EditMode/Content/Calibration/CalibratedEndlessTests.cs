using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using STP.Application;
using STP.Infrastructure.Content;
using STP.Infrastructure.Persistence;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Calibration.EditMode
{
    /// <summary>Entirely synthetic FIXTURE_ONLY sources and controlled Domain clocks; no Human/source approval evidence.</summary>
    public sealed class CalibratedEndlessTests
    {
        private const string Utc="2026-10-21T00:00:00Z";
        private static JsonValue Parse(string value){var parsed=StrictJsonParser.Parse(Encoding.UTF8.GetBytes(value),new JsonParserLimits(8388608,64,65536,1048576,256));Assert.That(parsed.Succeeded,Is.True,parsed.Error?.Code);return parsed.Value!;}
        private static JsonValue Read(string relative)=>Parse(File.ReadAllText(Path.Combine(Environment.GetEnvironmentVariable("STP_SOURCE_ROOT")??Directory.GetCurrentDirectory(),relative)));
        private static byte[] Bytes(JsonValue value)=>JcsSerializer.Serialize(value).Utf8!;
        private static JsonValue Put(JsonValue value,string key,JsonValue replacement)=>JsonValue.CreateObject(value.Members.Where(m=>m.Name!=key).Concat(new[]{new JsonMember(key,replacement)}));
        private static JsonValue S(string value)=>JsonValue.CreateString(value);
        private static JsonValue N(long value)=>JsonValue.CreateInteger(value);
        private static string Q(string value)=>LocalSaveHash.Quote(value);
        private sealed class Fixture
        {
            internal readonly ICanonicalJson Json=new CanonicalJsonAdapter();
            internal readonly RuntimePublicPuzzleCatalog Corpus;internal readonly RuntimeCompletionCatalog CampaignCompletion,EndlessCompletion;
            internal readonly RuntimeProgressCatalog ProgressCatalog;internal readonly LocalProgressPolicy Progress;internal readonly SaveLedgerCodec Ledger;
            internal readonly CalibratedEndlessProfile[] Profiles;internal readonly RetainedContractBindings Retained;internal readonly LocalSaveCodec Codec;
            internal readonly LocalEndlessPolicy Endless;internal readonly SaveProfileIdentity Profile=new SaveProfileIdentity("fixture-calibrated","person",CatalogUse.ContractTest);
            internal readonly LocalProfileState Empty,Cleared;internal readonly JsonValue Calibration,EndlessWrapper;internal readonly byte[][] ProfileBytes;
            internal Fixture()
            {
                var inputs=Enumerable.Range(0,240).Select(i=>Input("S1-"+(i/48+1).ToString("D2")+"-"+(i%48/12+1).ToString("D2")+"-"+(i%12+1).ToString("D2"))).ToArray();
                var publicDoc=Parse("{\"schemaVersion\":1,\"catalogId\":\"fixture-calibrated-public\",\"revision\":1,\"use\":\"ContractTest\",\"status\":\"FIXTURE_ONLY\",\"puzzles\":["+string.Join(",",inputs.Select(p=>EndlessProjection.PublicInput(p,true)))+"]}");string ph=LevelHashing.HashProjection(publicDoc);publicDoc=Put(publicDoc,"catalogHashSha256",S(ph));Corpus=new RuntimePublicPuzzleCatalog(Bytes(publicDoc),Id("fixture-calibrated-public",ph));
                string[] names={"dienstbeginn","regelbetrieb","stoerungslage","grosslage"};int[] min={4,5,6,8},max={5,6,8,10};ProfileBytes=names.Select((id,i)=>Bytes(Parse("{\"schemaVersion\":1,\"profileId\":"+Q(id)+",\"profileVersion\":1,\"rulesetVersion\":\"train-track-v1\",\"approvalStatus\":\"FIXTURE_ONLY\",\"difficultyBand\":"+Q(id)+",\"generatorVersion\":\"path-search-v1\",\"solverVersion\":\"solver-v2\",\"gridRanges\":{\"minSize\":"+min[i]+",\"maxSize\":"+max[i]+",\"minDensityPercent\":25,\"maxDensityPercent\":75},\"metricRanges\":{\"searchNodes\":{\"min\":0,\"max\":1000000},\"deductionSteps\":{\"min\":0,\"max\":1000000},\"maxDeductionDepth\":{\"min\":0,\"max\":1000000},\"requiredGuessDepth\":{\"min\":0,\"max\":0},\"trivialLines\":{\"min\":0,\"max\":20}},\"runtimeBudgets\":{\"maxAttempts\":1000,\"pathStateBudget\":20000,\"totalBudgetMs\":10000,\"solverBudgetMs\":2000,\"solverStateBudget\":1000000,\"hintBudgetMs\":100,\"hintStateBudget\":100000,\"timeoutPolicy\":\"ABORT_ATTEMPT_KEEP_RESERVATION\"},\"novelty\":{\"algorithmVersion\":\"public-d4-l1-v1\",\"referenceCorpus\":"+LocalSaveHash.CatalogProjection(Corpus.Identity)+",\"recentWindow\":50,\"recentSimilarityPercent\":0},\"humanReview\":{\"calibrationVersion\":\"fixture-no-human-v1\",\"evidenceSha256\":"+Q(new string('e',64))+",\"uiSourceSha256\":"+Q(new string('f',64))+",\"sampleCount\":12,\"criteriaVersion\":\"SYNTHETIC_TEST_NO_HUMAN_EVIDENCE\"},\"approvedBy\":\"fixture-only\",\"approvedDecisionRef\":\"fixture-only\"}"))).ToArray();
                string campaignRows=string.Join(",",inputs.Select(p=>"{\"identity\":{\"puzzleId\":"+Q(p.Identity.PuzzleId)+",\"profile\":"+Q(p.Identity.HashProfile)+",\"sha256\":"+Q(p.Identity.HashSha256)+"},\"sourceDocumentSha256\":"+Q(new string('a',64))+",\"calibrationVersion\":\"fixture-no-human-v1\",\"twoStarsSeconds\":2,\"threeStarsSeconds\":1}"));string profileRows=string.Join(",",names.Select((id,i)=>"{\"profileId\":"+Q(id)+",\"profileHashSha256\":"+Q(LocalSaveHash.Bytes(ProfileBytes[i]))+",\"calibrationVersion\":\"fixture-no-human-v1\",\"twoStarsSeconds\":2,\"threeStarsSeconds\":1}"));
                Calibration=Parse("{\"schemaVersion\":1,\"catalogId\":\"fixture-star-calibration\",\"catalogRevision\":1,\"approvalStatus\":\"FIXTURE_ONLY\",\"publicCatalog\":"+LocalSaveHash.CatalogProjection(Corpus.Identity)+",\"provenance\":[{\"calibrationVersion\":\"fixture-no-human-v1\",\"evidenceSha256\":"+Q(new string('e',64))+",\"uiSourceSha256\":"+Q(new string('f',64))+"}],\"campaign\":["+campaignRows+"],\"endlessProfiles\":["+profileRows+"]}");
                JsonValue Wrap(JsonValue c,string id){c=Put(Put(c,"catalogId",S(id)),"approvalStatus",S("FIXTURE_ONLY"));return Parse("{\"runtimeCompletionContractVersion\":1,\"completionCatalog\":"+Encoding.UTF8.GetString(Bytes(c))+",\"calibrationCatalog\":"+Encoding.UTF8.GetString(Bytes(Calibration))+"}");}
                var campaignWrapper=Wrap(Read("Assets/StammstreckenPuzzle/UI/MobileCandidate.completion.json").Get("completionCatalog")!,"completion-fixture-calibrated");EndlessWrapper=Wrap(Read("Content/Catalogs/endless/completion-v1.json"),"completion-fixture-calibrated-endless");CampaignCompletion=new RuntimeCompletionCatalog(Bytes(campaignWrapper),Corpus,Id("completion-fixture-calibrated",LevelHashing.HashProjection(campaignWrapper)),Json);EndlessCompletion=new RuntimeCompletionCatalog(Bytes(EndlessWrapper),Corpus,Id("completion-fixture-calibrated-endless",LevelHashing.HashProjection(EndlessWrapper)),Json);
                var campaign=Put(Put(Read("Assets/StammstreckenPuzzle/UI/MobileCandidate.campaign.json"),"catalogId",S("campaign-fixture-calibrated")),"approvalStatus",S("FIXTURE_ONLY"));ProgressCatalog=new RuntimeProgressCatalog(Corpus,Bytes(campaign),Id("campaign-fixture-calibrated",LevelHashing.HashProjection(campaign)),CampaignCompletion,Json);Ledger=new SaveLedgerCodec(Json);Progress=new LocalProgressPolicy(Corpus,ProgressCatalog.Campaign,CampaignCompletion,Json,Ledger);Empty=Progress.NewProfile(Profile,ProgressCatalog.CatalogSet,Utc).WithMetagame(LocalMetagameState.Empty);
                var state=Empty;foreach(var p in Corpus.Puzzles)state=Progress.Complete(state,Solve(p,Guid.NewGuid(),3000,SessionMode.FIRST_RUN),1,Utc).State;Cleared=state;
                var bindings=ProfileBytes.Select(b=>RetainedCalibratedEndlessBinding.FromDocuments(Bytes(EndlessWrapper),b,Corpus,EndlessCompletion.Identity,LocalSaveHash.Bytes(b),Json)).ToArray();Profiles=bindings.Select(b=>b.Profile).ToArray();var rewards=new[]{RetainedCompletionRewardBinding.FromRuntimeWrapper(LocalPostClearPolicy.PolicyVersion,Bytes(campaignWrapper),Json,CampaignCompletion.Identity,Corpus),RetainedCompletionRewardBinding.FromRuntimeWrapper(LocalPostClearPolicy.PolicyVersion,Bytes(EndlessWrapper),Json,EndlessCompletion.Identity,Corpus)};Retained=RetainedContractBindings.ForRuntimeWithCalibratedEndless(CatalogUse.ContractTest,null,rewards,bindings);Codec=LocalSaveCodec.ForMetagame(Json,Profile,ProgressCatalog.CatalogSet,Corpus,ProgressCatalog.Campaign,CampaignCompletion,Retained);Endless=new LocalEndlessPolicy(Corpus,new[]{EndlessCompletion.GetEndlessValues()},Json,Ledger,Profiles);
            }
            internal CatalogIdentity Id(string id,string hash)=>new CatalogIdentity(id,1,hash,CatalogUse.ContractTest,CatalogApprovalStatus.FixtureOnly);
            internal SaveSnapshot<LocalProfileState> Snapshot(LocalProfileState state,long generation=10)=>new SaveSnapshot<LocalProfileState>(Profile,ProgressCatalog.CatalogSet.AggregateCatalog,generation,state);
            internal LocalProfileState Round(LocalProfileState state,long generation=10){var decoded=Codec.Decode(Codec.Encode(Snapshot(state,generation),Utc,"fixture","fixture"));Assert.That(decoded.Status,Is.EqualTo(LocalSaveDecodeStatus.Valid),decoded.ErrorCode);return decoded.Snapshot!.Payload;}
            internal LocalProfileState Active(out ActiveEndlessDraft active){var reserved=Endless.Reserve(Cleared,Profiles[0].Profile,"2");reserved=Round(reserved);var candidate=Endless.GenerateAsync(Snapshot(reserved),reserved.Endless.OpenEndless.Single().Id,CancellationToken.None).GetAwaiter().GetResult();Assert.That(candidate,Is.Not.Null);var state=Endless.Promote(reserved,candidate!,Guid.NewGuid());active=state.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single();return Round(state);}
        }
        private static readonly Lazy<Fixture> Shared=new Lazy<Fixture>(()=>new Fixture());
        private static PublicPuzzleInput Input(string id){var temp=new PublicPuzzleInput(new PublicPuzzleIdentity(id,PublicPuzzleIdentity.SemanticHashProfile,new string('0',64)),"train-track-v1",4,4,PublicPuzzleSide.W,0,PublicPuzzleSide.E,0,new[]{4,0,0,0},new[]{1,1,1,1});string hash=LevelHashing.HashProjection(Parse(EndlessProjection.PublicInput(temp,false)));return new PublicPuzzleInput(new PublicPuzzleIdentity(id,PublicPuzzleIdentity.SemanticHashProfile,hash),"train-track-v1",4,4,PublicPuzzleSide.W,0,PublicPuzzleSide.E,0,temp.RowCounts,temp.ColumnCounts);}
        private static PuzzleSessionState Solve(PublicPuzzleInput input,Guid attempt,long elapsed,SessionMode mode,bool hint=false)
        {
            var definition=input.ToDomainDefinition();var proof=PuzzleSolver.Solve(definition,new SolverLimits(1000000,5000));Assert.That(proof.Classification,Is.EqualTo(SolverClassification.UNIQUE));var s=PuzzleSessionState.Create(definition,input.Identity.PuzzleId,input.Identity.HashProfile,input.Identity.HashSha256,attempt,mode,0,null,null).Value!;if(hint)s=PuzzleCommandHandler.Handle(s,new RecordHintDisplayed(Guid.NewGuid(),s.Revision,attempt),0).State!;
            Direction Toward(CellCoordinate a,CellCoordinate b)=>b.Y<a.Y?Direction.N:b.X>a.X?Direction.E:b.Y>a.Y?Direction.S:Direction.W;for(int i=0;i<proof.Path.Count;i++){var cell=proof.Path[i];var before=i==0?definition.A.Side:Toward(cell,proof.Path[i-1]);var after=i==proof.Path.Count-1?definition.B.Side:Toward(cell,proof.Path[i+1]);int ports=(1<<(int)before)|(1<<(int)after);var shape=Enumerable.Range(0,6).Single(k=>TrackGeometry.Ports((TrackShape)k)==ports);long now=i==proof.Path.Count-1?elapsed:elapsed*i/(proof.Path.Count-1);s=PuzzleCommandHandler.Handle(s,new ApplyCellContent(Guid.NewGuid(),s.Revision,cell,(CellContent)(shape+3)),now).State!;}Assert.That(s.Phase,Is.EqualTo(SessionPhase.SolvedPendingCommit));Assert.That(s.ActiveElapsedMs,Is.EqualTo(elapsed));return s;
        }
        [TestCase(1000,3,30)][TestCase(2000,2,20)][TestCase(2001,1,15)]
        public void ActualDomainEndlessCompletionPaysExactStarsAndRoundTrips(long elapsed,int stars,int amount)
        {var f=Shared.Value;var state=f.Active(out var active);var solved=Solve(active.PublicPuzzleInput,active.Checkpoint.AttemptId,elapsed,SessionMode.ENDLESS,true);long balance=f.Ledger.Validate(state);var done=f.Endless.Complete(state,solved,11,Utc);var record=done.Endless.OpenEndless.OfType<OpenEndlessCompletion>().Single();Assert.That(record.Result!.Stars,Is.EqualTo(stars));Assert.That(record.Result.ActiveElapsedMs,Is.EqualTo(elapsed));Assert.That(f.Ledger.Validate(done)-balance,Is.EqualTo(amount));Assert.That(record.Result.Terminals.Count,Is.EqualTo(stars));Assert.That(f.Endless.Complete(done,solved,12,Utc),Is.SameAs(done));var loaded=f.Round(done,11);Assert.That(loaded.Endless.OpenEndless.OfType<OpenEndlessCompletion>().Single().Result!.Stars,Is.EqualTo(stars));Assert.That(f.Endless.Complete(loaded,solved,12,Utc),Is.SameAs(loaded));var skipped=f.Endless.SkipReward(loaded,active.Id);Assert.That(skipped.Endless.OpenEndless,Is.Empty);Assert.That(f.Ledger.Validate(f.Round(skipped,12))-balance,Is.EqualTo(amount));}
        [TestCase(0,1)][TestCase(1,1)][TestCase(3,2)][TestCase(9007199254740L,1)]
        public void InvalidSecondsNeverRoundOrOverflow(long two,long three){if(two>three&&three>0){var times=new CalibratedStarTimes("fixture",two,three);Assert.That(times.TwoStarMs,Is.EqualTo(two*1000));}else Assert.Throws<ArgumentException>(()=>new CalibratedStarTimes("fixture",two,three));}
        [Test]public void SecondsBeyondSafeMillisecondRangeAreRejected()=>Assert.Throws<ArgumentException>(()=>new CalibratedStarTimes("fixture",CalibratedStarTimes.MaxSeconds+1,1));
        [Test]public void ClosedProfileExtraFieldAndChangedMetricVersionAreRejected(){var f=Shared.Value;var source=Put(Parse(Encoding.UTF8.GetString(f.ProfileBytes[0])),"ignored",N(1));Assert.Throws<ArgumentException>(()=>new LockedCalibratedEndlessProfile(Bytes(source),f.Corpus,f.EndlessCompletion,LevelHashing.HashProjection(source)));source=Put(Parse(Encoding.UTF8.GetString(f.ProfileBytes[0])),"solverVersion",S("other"));Assert.Throws<ArgumentException>(()=>new LockedCalibratedEndlessProfile(Bytes(source),f.Corpus,f.EndlessCompletion,LevelHashing.HashProjection(source)));}
        [Test]public void ProfileHashAndCalibrationProvenanceAreExact(){var f=Shared.Value;Assert.Throws<ArgumentException>(()=>new LockedCalibratedEndlessProfile(f.ProfileBytes[0],f.Corpus,f.EndlessCompletion,new string('a',64)));var source=Parse(Encoding.UTF8.GetString(f.ProfileBytes[0]));var human=Put(source.Get("humanReview")!,"evidenceSha256",S(new string('b',64)));source=Put(source,"humanReview",human);Assert.Throws<ArgumentException>(()=>new LockedCalibratedEndlessProfile(Bytes(source),f.Corpus,f.EndlessCompletion,LevelHashing.HashProjection(source)));}
        [Test]public void DraftAndFixturePublicSourcesCannotBeConsumedAsProduction(){var f=Shared.Value;var doc=Read("Assets/StammstreckenPuzzle/UI/MobileCandidate.public.json");Assert.Throws<ArgumentException>(()=>new RuntimePublicPuzzleCatalog(Bytes(doc),new CatalogIdentity("production-test-public",1,f.Corpus.Identity.HashSha256,CatalogUse.Production,CatalogApprovalStatus.ProductApproved)));}
        [Test]public void IndependentContentAndRetainedReadersAgreeOnAllFourFullSources()
        {
            var f=Shared.Value;
            for(int i=0;i<4;i++)
            {
                var actual=new LockedCalibratedEndlessProfile(f.ProfileBytes[i],f.Corpus,f.EndlessCompletion,LocalSaveHash.Bytes(f.ProfileBytes[i])).Profile;var retained=f.Profiles[i];
                Assert.That(actual.Id,Is.EqualTo(retained.Id));Assert.That(actual.Version,Is.EqualTo(retained.Version));Assert.That(actual.HashSha256,Is.EqualTo(retained.HashSha256));
                Assert.That(EndlessProjection.SearchFields(actual.Search),Is.EqualTo(EndlessProjection.SearchFields(retained.Search)));Assert.That(actual.StarCalibration.EqualsBinding(retained.StarCalibration),Is.True);
                var a=new[]{actual.Quality.SearchNodes,actual.Quality.DeductionSteps,actual.Quality.MaxDeductionDepth,actual.Quality.RequiredGuessDepth,actual.Quality.TrivialLines};var b=new[]{retained.Quality.SearchNodes,retained.Quality.DeductionSteps,retained.Quality.MaxDeductionDepth,retained.Quality.RequiredGuessDepth,retained.Quality.TrivialLines};
                for(int n=0;n<a.Length;n++){Assert.That(a[n].Min,Is.EqualTo(b[n].Min));Assert.That(a[n].Max,Is.EqualTo(b[n].Max));}
            }
        }
        [Test]public void NewCalibrationAndRuntimeWrapperConsumersRejectTheActualDraftCandidateLine()
        {
            var f=Shared.Value;var doc=Read("Assets/StammstreckenPuzzle/UI/MobileCandidate.public.json");var candidate=new MobileCandidateData(Bytes(doc),doc.Get("catalogHashSha256")!.StringValue);
            var calibration=new CatalogIdentity("qa-mobile-candidate-calibration",1,LevelHashing.HashProjection(f.Calibration),CatalogUse.InternalPreview,CatalogApprovalStatus.Draft);
            var completion=new CatalogIdentity("completion-qa-mobile-candidate-endless",1,LevelHashing.HashProjection(f.EndlessWrapper),CatalogUse.InternalPreview,CatalogApprovalStatus.Draft);
            Assert.That(Assert.Throws<ArgumentException>(()=>new StarCalibrationCatalog(Bytes(f.Calibration),candidate,calibration,f.Json))!.Message,Is.EqualTo("STAR_CATALOG_USE_MISMATCH"));
            Assert.That(Assert.Throws<ArgumentException>(()=>new RuntimeCompletionCatalog(Bytes(f.EndlessWrapper),candidate,completion,f.Json))!.Message,Is.EqualTo("STAR_CATALOG_USE_MISMATCH"));
            Assert.That(Assert.Catch(()=>RetainedCompletionRewardBinding.FromRuntimeWrapper(LocalPostClearPolicy.PolicyVersion,Bytes(f.EndlessWrapper),f.Json,completion,candidate))!.Message,Is.EqualTo("SAVE_CATALOG_USE_MISMATCH"));
        }
        [TestCase("solverStateBudget")][TestCase("hintBudgetMs")][TestCase("hintStateBudget")][TestCase("wrongGrid")]
        [TestCase("guessRequired")][TestCase("reverseInterval")][TestCase("provenance")][TestCase("sampleCount")]
        public void UnsupportedProfileRequirementsFailAfterExactSourceHashRebinding(string mutation)
        {
            var f=Shared.Value;var source=Parse(Encoding.UTF8.GetString(f.ProfileBytes[0]));
            if(mutation=="solverStateBudget"||mutation=="hintBudgetMs"||mutation=="hintStateBudget")source=Put(source,"runtimeBudgets",Put(source.Get("runtimeBudgets")!,mutation,N(1)));
            if(mutation=="wrongGrid")source=Put(source,"gridRanges",Put(source.Get("gridRanges")!,"minSize",N(5)));
            if(mutation=="guessRequired")source=Put(source,"metricRanges",Put(source.Get("metricRanges")!,"requiredGuessDepth",Parse("{\"min\":0,\"max\":1}")));
            if(mutation=="reverseInterval")source=Put(source,"metricRanges",Put(source.Get("metricRanges")!,"searchNodes",Parse("{\"min\":2,\"max\":1}")));
            if(mutation=="provenance")source=Put(source,"humanReview",Put(source.Get("humanReview")!,"evidenceSha256",S(new string('b',64))));
            if(mutation=="sampleCount")source=Put(source,"humanReview",Put(source.Get("humanReview")!,"sampleCount",N(11)));
            string hash=LevelHashing.HashProjection(source);var calibration=Put(f.Calibration,"endlessProfiles",JsonValue.CreateArray(f.Calibration.Get("endlessProfiles")!.Items.Select(p=>p.Get("profileId")!.StringValue=="dienstbeginn"?Put(p,"profileHashSha256",S(hash)):p)));
            var wrapper=Put(f.EndlessWrapper,"calibrationCatalog",calibration);var completion=new RuntimeCompletionCatalog(Bytes(wrapper),f.Corpus,f.Id("completion-fixture-calibrated-endless",LevelHashing.HashProjection(wrapper)),f.Json);
            Assert.Throws<ArgumentException>(()=>new LockedCalibratedEndlessProfile(Bytes(source),f.Corpus,completion,hash),mutation);
        }
        [Test]public void PinnedParametersAndResultRejectRehashedMutations()
        {var f=Shared.Value;var state=f.Active(out var active);var solved=Solve(active.PublicPuzzleInput,active.Checkpoint.AttemptId,1000,SessionMode.ENDLESS);state=f.Endless.Complete(state,solved,11,Utc);var envelope=Parse(Encoding.UTF8.GetString(f.Codec.Encode(f.Snapshot(state,11),Utc,"fixture","fixture")));foreach(string mutation in new[]{"stars","time","attempt","generation","source"}){var payload=envelope.Get("payload")!;var endless=payload.Get("endless")!;var open=endless.Get("openEndless")!.Items[0];var result=open.Get("result")!;if(mutation=="stars")result=Put(result,"stars",N(2));if(mutation=="time")result=Put(result,"activeElapsedMs",N(3000));if(mutation=="attempt")result=Put(result,"attemptId",S(Guid.NewGuid().ToString("D")));if(mutation=="generation")result=Put(result,"firstClearTerminal",Put(result.Get("firstClearTerminal")!,"committedGeneration",N(12)));if(mutation=="source")result=Put(result,"firstClearTerminal",Put(result.Get("firstClearTerminal")!,"catalog",Put(result.Get("firstClearTerminal")!.Get("catalog")!,"hashSha256",S(new string('b',64)))));open=Put(open,"result",result);endless=Put(endless,"openEndless",JsonValue.CreateArray(new[]{open}));payload=Put(payload,"endless",endless);var changed=Put(Put(envelope,"payload",payload),"payloadSha256",S(LevelHashing.HashProjection(payload)));var decoded=f.Codec.Decode(Bytes(changed));Assert.That(decoded.Status,Is.Not.EqualTo(LocalSaveDecodeStatus.Valid),mutation);}}
        [Test]public void StaleSolvedAttemptCannotCompleteRestartedDraftOrMovePoints()
        {
            var f=Shared.Value;var state=f.Active(out var active);var old=Solve(active.PublicPuzzleInput,active.Checkpoint.AttemptId,1000,SessionMode.ENDLESS);
            state=f.Endless.RestartDraft(state,active.Id,Guid.NewGuid());long balance=f.Ledger.Validate(state);
            Assert.Throws<ArgumentException>(()=>f.Endless.Complete(state,old,11,Utc));
            Assert.That(f.Ledger.Validate(state),Is.EqualTo(balance));Assert.That(state.Endless.OpenEndless.Single(),Is.TypeOf<ActiveEndlessDraft>());
        }
        [Test]public void ResultWitnessAndPinnedContractMutationsCannotPassARehashedEnvelope()
        {
            var f=Shared.Value;var state=f.Active(out var active);state=f.Endless.Complete(state,Solve(active.PublicPuzzleInput,active.Checkpoint.AttemptId,1000,SessionMode.ENDLESS),11,Utc);
            var envelope=Parse(Encoding.UTF8.GetString(f.Codec.Encode(f.Snapshot(state,11),Utc,"fixture","fixture")));
            foreach(string mutation in new[]{"receiptMissing","secondMissing","resultMissing","unknownResultVersion","unknownParametersVersion","pinnedMilliseconds"})
            {
                var payload=envelope.Get("payload")!;var endless=payload.Get("endless")!;var open=endless.Get("openEndless")!.Items[0];
                if(mutation=="receiptMissing")endless=Put(endless,"economyReceipts",JsonValue.CreateArray(endless.Get("economyReceipts")!.Items.Skip(1)));
                if(mutation=="secondMissing")open=Put(open,"result",Put(open.Get("result")!,"secondStarPaid",JsonValue.Null));
                if(mutation=="resultMissing")open=JsonValue.CreateObject(open.Members.Where(m=>m.Name!="result"));
                if(mutation=="unknownResultVersion")open=Put(open,"recordVersion",N(3));
                if(mutation=="unknownParametersVersion"||mutation=="pinnedMilliseconds")
                {
                    var descriptor=open.Get("descriptor")!;var parameters=descriptor.Get("parameters")!;
                    parameters=mutation=="unknownParametersVersion"?Put(parameters,"contractVersion",N(3)):Put(parameters,"starCalibration",Put(parameters.Get("starCalibration")!,"twoStarMs",N(3000)));
                    descriptor=Put(Put(descriptor,"parameters",parameters),"parameterHashSha256",S(LevelHashing.HashProjection(parameters)));open=Put(open,"descriptor",descriptor);
                }
                endless=Put(endless,"openEndless",JsonValue.CreateArray(new[]{open}));payload=Put(payload,"endless",endless);
                var changed=Put(Put(envelope,"payload",payload),"payloadSha256",S(LevelHashing.HashProjection(payload)));var decoded=f.Codec.Decode(Bytes(changed));
                Assert.That(decoded.Status,Is.Not.EqualTo(LocalSaveDecodeStatus.Valid),mutation);Assert.That(decoded.ErrorCode,Is.Not.Null.And.Not.Empty,mutation);
            }
        }
        private sealed class Clock:ILocalSessionClock{public string UtcNow=>Utc;public long MonotonicMilliseconds=>0;}
        [Test]public void PhysicalAtomicRepositoryRereadsCalibratedResultAndRejectsStaleCommit()
        {
            var f=Shared.Value;string root=Path.Combine(Environment.GetEnvironmentVariable("STP_SOURCE_ROOT")??Directory.GetCurrentDirectory(),".utmp","star-atomic-"+Guid.NewGuid().ToString("N").Substring(0,10));Directory.CreateDirectory(root);var files=new SaveFileSystem(Path.GetFullPath(root),f.Profile,f.ProgressCatalog.CatalogSet.AggregateCatalog);var repo=new AtomicSaveRepository(files,f.Codec,new Clock(),"fixture","fixture");Assert.That(repo.CreateProfileAsync(f.Cleared,CancellationToken.None).GetAwaiter().GetResult().Succeeded,Is.True);var reserved=f.Endless.Reserve(f.Cleared,f.Profiles[0].Profile,"2");var reservation=repo.CommitAsync(f.Snapshot(reserved,2),1,CancellationToken.None).GetAwaiter().GetResult();Assert.That(reservation.Succeeded,Is.True,reservation.ErrorCode);var loaded=repo.LoadAsync(CancellationToken.None).GetAwaiter().GetResult();Assert.That(loaded.Snapshot!.Payload.Endless.OpenEndless.OfType<ReservedEndless>().Single().Descriptor.Parameters,Is.TypeOf<SearchGeneratorParameters>());var candidate=f.Endless.GenerateAsync(loaded.Snapshot,loaded.Snapshot.Payload.Endless.OpenEndless.Single().Id,CancellationToken.None).GetAwaiter().GetResult()!;var promoted=f.Endless.Promote(loaded.Snapshot.Payload,candidate,Guid.NewGuid());Assert.That(repo.CommitAsync(f.Snapshot(promoted,3),2,CancellationToken.None).GetAwaiter().GetResult().Succeeded,Is.True);var active=promoted.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single();var solved=Solve(active.PublicPuzzleInput,active.Checkpoint.AttemptId,1000,SessionMode.ENDLESS);var done=f.Endless.Complete(promoted,solved,4,Utc);Assert.That(repo.CommitAsync(f.Snapshot(done,4),3,CancellationToken.None).GetAwaiter().GetResult().Succeeded,Is.True);var reconstructed=new AtomicSaveRepository(new SaveFileSystem(Path.GetFullPath(root),f.Profile,f.ProgressCatalog.CatalogSet.AggregateCatalog),f.Codec,new Clock(),"fixture","fixture");var round=reconstructed.LoadAsync(CancellationToken.None).GetAwaiter().GetResult();Assert.That(round.Snapshot!.Payload.Endless.OpenEndless.OfType<OpenEndlessCompletion>().Single().Result!.Stars,Is.EqualTo(3));Assert.That(reconstructed.CommitAsync(f.Snapshot(done,4),3,CancellationToken.None).GetAwaiter().GetResult().Succeeded,Is.False);Assert.That(f.Endless.Complete(round.Snapshot.Payload,solved,5,Utc),Is.SameAs(round.Snapshot.Payload));
        }
        [Test,Timeout(600000)]public void OpenResultSurvivesActualCompactionWhileReceiptsPruneAndTenThousandOrdinalsStayBounded()
        {
            var f=Shared.Value;var state=f.Active(out var active);state=f.Endless.Complete(state,Solve(active.PublicPuzzleInput,active.Checkpoint.AttemptId,1000,SessionMode.ENDLESS),11,Utc);LocalProfileState? compacted=null;foreach(var input in f.Corpus.Puzzles){state=f.Progress.Complete(state,Solve(input,Guid.NewGuid(),1000,SessionMode.REVISION),20,Utc).State;if(state.EconomyJournal.Count==0)compacted=state;}
            Assert.That(state.Endless.OpenEndless.OfType<OpenEndlessCompletion>().Single().Result!.Stars,Is.EqualTo(3));Assert.That(state.Endless.EconomyReceipts,Is.Empty);Assert.That(f.Round(state,20).Endless.OpenEndless.OfType<OpenEndlessCompletion>().Single().Result!.Terminals.Count,Is.EqualTo(3));Assert.That(compacted,Is.Not.Null);state=compacted!;string retired="";for(int i=0;i<10000;i++){state=f.Endless.Reserve(state,f.Profiles[0].Profile,"2");retired=state.Endless.OpenEndless.OfType<ReservedEndless>().Single().Id;state=f.Endless.Abandon(state,retired);}Assert.That(state.Endless.HighestReservedOrdinal,Is.EqualTo("10001"));Assert.That(state.Endless.OpenEndless.Count,Is.EqualTo(1));Assert.That(state.Endless.EconomyReceipts.Count,Is.LessThanOrEqualTo(576));Assert.That(state.Endless.RecentPublicInputs.Count,Is.LessThanOrEqualTo(50));Assert.That(state.Endless.OpenEndless.OfType<OpenEndlessCompletion>().Single().Result!.Terminals.Count,Is.EqualTo(3));Assert.That(f.Endless.SkipReward(state,retired),Is.SameAs(state));f.Round(state,10050);
        }
    }
}
