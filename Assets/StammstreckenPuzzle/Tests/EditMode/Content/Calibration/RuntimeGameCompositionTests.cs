using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;
using STP.Bootstrap;
using STP.Infrastructure.Content;
using STP.Infrastructure.Persistence;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Calibration.EditMode
{
    /// <summary>All positive documents are controlled FIXTURE_ONLY data; no approval source or Production document is fabricated.</summary>
    public sealed class RuntimeGameCompositionTests
    {
        private const string Utc="2026-10-21T00:00:00Z";
        private static readonly string H1=new string('1',64),H2=new string('2',64),H3=new string('3',64);
        private static JsonValue Parse(byte[] b){var p=StrictJsonParser.Parse(b,new JsonParserLimits(8388608,64,65536,1048576,256));Assert.That(p.Succeeded,Is.True);return p.Value!;}
        private static byte[] Bytes(JsonValue v)=>JcsSerializer.Serialize(v).Utf8!;
        private static JsonValue S(string s)=>JsonValue.CreateString(s);
        private static JsonValue N(long n)=>JsonValue.CreateInteger(n);
        private static JsonValue Put(JsonValue v,string k,JsonValue x)=>JsonValue.CreateObject(v.Members.Where(m=>m.Name!=k).Concat(new[]{new JsonMember(k,x)}));
        private static JsonValue Ref(CatalogIdentity c)=>Parse(Encoding.UTF8.GetBytes(LocalSaveHash.CatalogProjection(c)));
        private static object OriginalFixture(){var lazy=typeof(CalibratedEndlessTests).GetField("Shared",BindingFlags.Static|BindingFlags.NonPublic)!.GetValue(null)!;return lazy.GetType().GetProperty("Value")!.GetValue(lazy)!;}
        private static T Field<T>(object f,string name)=>(T)f.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(f)!;
        private sealed class Data
        {
            internal readonly Dictionary<string,byte[]> Files=new Dictionary<string,byte[]>(StringComparer.Ordinal);internal JsonValue Binding;internal byte[] BindingBytes=>Bytes(Binding);
            internal Data()
            {
                object f=OriginalFixture();var corpus=Field<RuntimePublicPuzzleCatalog>(f,"Corpus");var progress=Field<RuntimeProgressCatalog>(f,"ProgressCatalog");var campaignCompletion=Field<RuntimeCompletionCatalog>(f,"CampaignCompletion");var endlessCompletion=Field<RuntimeCompletionCatalog>(f,"EndlessCompletion");var profileBytes=Field<byte[][]>(f,"ProfileBytes");
                JsonValue Read(string p)=>Parse(File.ReadAllBytes(Path.Combine(Environment.GetEnvironmentVariable("STP_SOURCE_ROOT")??Directory.GetCurrentDirectory(),p)));
                var publicDoc=Parse(Encoding.UTF8.GetBytes("{\"schemaVersion\":1,\"catalogId\":\"fixture-calibrated-public\",\"revision\":1,\"use\":\"ContractTest\",\"status\":\"FIXTURE_ONLY\",\"puzzles\":["+string.Join(",",corpus.Puzzles.Select(p=>EndlessProjection.PublicInput(p,true)))+"]}"));Files["Public.json"]=Bytes(Put(publicDoc,"catalogHashSha256",S(corpus.Identity.HashSha256)));
                Files["Campaign.json"]=Bytes(Put(Put(Read("Assets/StammstreckenPuzzle/UI/MobileCandidate.campaign.json"),"catalogId",S("campaign-fixture-calibrated")),"approvalStatus",S("FIXTURE_ONLY")));
                var originalCampaign=Read("Assets/StammstreckenPuzzle/UI/MobileCandidate.completion.json").Get("completionCatalog")!;originalCampaign=Put(Put(originalCampaign,"catalogId",S(campaignCompletion.Identity.CatalogId)),"approvalStatus",S("FIXTURE_ONLY"));var calibration=Field<JsonValue>(f,"Calibration");
                Files["Calibration.json"]=Bytes(calibration);Files["CampaignCompletion.json"]=Bytes(JsonValue.CreateObject(new[]{new JsonMember("runtimeCompletionContractVersion",N(1)),new JsonMember("completionCatalog",originalCampaign),new JsonMember("calibrationCatalog",calibration)}));Files["EndlessCompletion.json"]=Bytes(Field<JsonValue>(f,"EndlessWrapper"));
                var cosmetics=Read("Assets/StammstreckenPuzzle/UI/MobileCandidate.cosmetics.json");cosmetics=Put(Put(cosmetics,"catalogId",S("cosmetics-fixture-composition")),"approvalStatus",S("FIXTURE_ONLY"));cosmetics=Put(cosmetics,"items",JsonValue.CreateArray(cosmetics.Get("items")!.Items.Select(i=>Put(i,"status",S("ACTIVE")))));Files["Cosmetics.json"]=Bytes(cosmetics);
                foreach(var b in profileBytes){var p=Parse(b);Files["Profiles/"+p.Get("profileId")!.StringValue+".json"]=(byte[])b.Clone();}
                // Preserve existing float lexemes; these copied fixture bytes are never canonicalized through integer-JCS.
                string root=Environment.GetEnvironmentVariable("STP_SOURCE_ROOT")??Directory.GetCurrentDirectory();Files["RailwayAssets.json"]=Encoding.UTF8.GetBytes(File.ReadAllText(Path.Combine(root,"Assets/StammstreckenPuzzle/UI/RailwayAssets.json")).Replace("\"approvalStatus\": \"DRAFT\"","\"approvalStatus\": \"FIXTURE_ONLY\""));
                Files["CampaignText.json"]=Bytes(Put(Read("Assets/StammstreckenPuzzle/UI/ProductionCampaignText.json"),"approvalStatus",S("FIXTURE_ONLY")));
                var cosmeticId=new CatalogIdentity("cosmetics-fixture-composition",1,LevelHashing.HashProjection(cosmetics),CatalogUse.ContractTest,CatalogApprovalStatus.FixtureOnly);
                Binding=JsonValue.CreateObject(new[]{new JsonMember("runtimeImportVersion",N(1)),new JsonMember("use",S("ContractTest")),new JsonMember("status",S("FIXTURE_ONLY")),new JsonMember("releaseLockRawSha256",S(H1)),new JsonMember("releaseLockJcsSha256",S(H2)),new JsonMember("approvalLockRawSha256",JsonValue.Null),new JsonMember("uiSourceLockRawSha256",S(H3)),new JsonMember("publicCatalog",Ref(corpus.Identity)),new JsonMember("campaignCatalog",Ref(progress.Campaign.Identity)),new JsonMember("campaignCompletionCatalog",Ref(campaignCompletion.Identity)),new JsonMember("endlessCompletionCatalog",Ref(endlessCompletion.Identity)),new JsonMember("calibrationCatalog",Ref(campaignCompletion.Calibration.Identity)),new JsonMember("cosmeticsCatalog",Ref(cosmeticId)),new JsonMember("catalogSet",Ref(progress.CatalogSet.AggregateCatalog)),new JsonMember("qualityProfiles",JsonValue.CreateArray(profileBytes.Select(Parse).OrderBy(p=>p.Get("profileId")!.StringValue,StringComparer.Ordinal).Select(p=>JsonValue.CreateObject(new[]{new JsonMember("profileId",p.Get("profileId")!),new JsonMember("hashSha256",S(LevelHashing.HashProjection(p)))})))),new JsonMember("railwayAssetsRawSha256",S(LocalSaveHash.Bytes(Files["RailwayAssets.json"]))),new JsonMember("campaignTextRawSha256",S(LocalSaveHash.Bytes(Files["CampaignText.json"]))),new JsonMember("playerFiles",JsonValue.Null)});RebindRaw();
            }
            internal void RebindRaw(){Binding=Put(Binding,"playerFiles",JsonValue.CreateArray(Files.OrderBy(p=>p.Key,StringComparer.Ordinal).Select(p=>JsonValue.CreateObject(new[]{new JsonMember("path",S(p.Key)),new JsonMember("rawSha256",S(LocalSaveHash.Bytes(p.Value)))}))));}
            internal RuntimeGameSources Read()=>RuntimeGameSources.ReadContractTest(Files,BindingBytes,LocalSaveHash.Bytes(BindingBytes),H1,H2,H3);
        }
        private sealed class Clock:ILocalSessionClock{public long Now;public long MonotonicMilliseconds=>Now;public string UtcNow=>Utc;}
        private sealed class Owner:ILocalSaveDispatcher
        {
            private readonly int owner=Thread.CurrentThread.ManagedThreadId;private readonly ConcurrentQueue<(Action,TaskCompletionSource<bool>)> q=new ConcurrentQueue<(Action,TaskCompletionSource<bool>)>();
            public Task InvokeAsync(Action a){if(Thread.CurrentThread.ManagedThreadId==owner){a();return Task.CompletedTask;}var t=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);q.Enqueue((a,t));return t.Task;}
            public void Pump(Task task){var timer=Stopwatch.StartNew();while(!task.IsCompleted){while(q.TryDequeue(out var p)){try{p.Item1();p.Item2.TrySetResult(true);}catch(Exception e){p.Item2.TrySetException(e);}}if(timer.ElapsedMilliseconds>60000)Assert.Fail("Fixture owner timeout");Thread.Sleep(1);}task.GetAwaiter().GetResult();}
            public T Pump<T>(Task<T> task){Pump((Task)task);return task.GetAwaiter().GetResult();}
        }
        private static string NewRoot()=>Path.GetFullPath(Path.Combine(Environment.GetEnvironmentVariable("STP_SOURCE_ROOT")??Directory.GetCurrentDirectory(),".utmp","c"+Guid.NewGuid().ToString("N").Substring(0,6)));
        [Test]public void CompleteLockedSourcesDefensivelyCloneOriginalAndReturnedBytes()
        {
            var d=new Data();byte[] binding=d.BindingBytes;var source=RuntimeGameSources.ReadContractTest(d.Files,binding,LocalSaveHash.Bytes(binding),H1,H2,H3);byte[] originalBinding=(byte[])binding.Clone();byte[] before=source.GetFileBytes("Public.json");d.Files["Public.json"][0]^=1;binding[0]^=1;var exposed=source.GetFileBytes("Public.json");exposed[0]^=1;Assert.That(source.GetFileBytes("Public.json"),Is.EqualTo(before));Assert.That(source.GetBindingBytes(),Is.EqualTo(originalBinding));Assert.That(source.CalibratedProfiles.Count,Is.EqualTo(4));Assert.That(source.Catalog.Puzzles.Count,Is.EqualTo(240));Assert.That(source.ApprovalLockRawSha256,Is.Null);Assert.That(source.GetFileBytes("RailwayAssets.json"),Is.EqualTo(d.Files["RailwayAssets.json"]));
        }
        [TestCase("raw")][TestCase("bindingRaw")][TestCase("releaseRaw")][TestCase("releaseJcs")][TestCase("uiRaw")][TestCase("extraField")][TestCase("missingField")][TestCase("duplicateField")][TestCase("draft")][TestCase("approval")][TestCase("extraFile")][TestCase("missingFile")][TestCase("profileOrder")][TestCase("catalogSet")][TestCase("semanticHash")][TestCase("wrapperCalibration")][TestCase("missingHash")][TestCase("unknownVersion")][TestCase("textIdentity")][TestCase("rawOrder")][TestCase("cosmeticReference")]
        public void InvalidBindingsAndCrossSourcesNeverCreatePrivateRoot(string mutation)
        {
            var d=new Data();string root=NewRoot();string br=H1,rr=H1,rj=H2,ui=H3;
            if(mutation=="raw")d.Files["Public.json"][0]^=1;
            if(mutation=="releaseRaw")rr=new string('4',64);if(mutation=="releaseJcs")rj=new string('4',64);if(mutation=="uiRaw")ui=new string('4',64);
            if(mutation=="extraField")d.Binding=Put(d.Binding,"EXTRA",N(1));if(mutation=="missingField")d.Binding=JsonValue.CreateObject(d.Binding.Members.Where(m=>m.Name!="catalogSet"));
            if(mutation=="draft")d.Binding=Put(Put(d.Binding,"use",S("InternalPreview")),"status",S("DRAFT"));if(mutation=="approval")d.Binding=Put(d.Binding,"approvalLockRawSha256",S(H1));
            if(mutation=="extraFile")d.Files.Add("../extra.json",new byte[]{1});if(mutation=="missingFile")d.Files.Remove("Calibration.json");
            if(mutation=="profileOrder")d.Binding=Put(d.Binding,"qualityProfiles",JsonValue.CreateArray(d.Binding.Get("qualityProfiles")!.Items.Reverse()));
            if(mutation=="missingHash")d.Binding=Put(d.Binding,"publicCatalog",JsonValue.CreateObject(d.Binding.Get("publicCatalog")!.Members.Where(m=>m.Name!="hashSha256")));if(mutation=="unknownVersion")d.Binding=Put(d.Binding,"runtimeImportVersion",N(2));
            if(mutation=="rawOrder")d.Binding=Put(d.Binding,"playerFiles",JsonValue.CreateArray(d.Binding.Get("playerFiles")!.Items.Reverse()));
            if(mutation=="textIdentity"){var text=Parse(d.Files["CampaignText.json"]);text=Put(text,"entries",JsonValue.CreateArray(text.Get("entries")!.Items.Select((row,i)=>i==0?Put(row,"puzzleId",S("S1-99-99-99")):row)));d.Files["CampaignText.json"]=Bytes(text);d.RebindRaw();d.Binding=Put(d.Binding,"campaignTextRawSha256",S(LocalSaveHash.Bytes(d.Files["CampaignText.json"])));}
            if(mutation=="catalogSet")d.Binding=Put(d.Binding,"catalogSet",Put(d.Binding.Get("catalogSet")!,"hashSha256",S(new string('4',64))));
            if(mutation=="semanticHash"){var doc=Parse(d.Files["Public.json"]);d.Files["Public.json"]=Bytes(Put(doc,"catalogHashSha256",S(new string('4',64))));d.RebindRaw();}
            if(mutation=="wrapperCalibration"){var doc=Parse(d.Files["CampaignCompletion.json"]);doc=Put(doc,"calibrationCatalog",Put(doc.Get("calibrationCatalog")!,"catalogRevision",N(2)));d.Files["CampaignCompletion.json"]=Bytes(doc);d.RebindRaw();d.Binding=Put(d.Binding,"campaignCompletionCatalog",Put(d.Binding.Get("campaignCompletionCatalog")!,"hashSha256",S(LevelHashing.HashProjection(doc))));}
            if(mutation=="cosmeticReference"){var doc=Parse(d.Files["Cosmetics.json"]);bool changed=false;doc=Put(doc,"items",JsonValue.CreateArray(doc.Get("items")!.Items.Select(item=>{if(changed||item.Get("acquisition")!.StringValue!="MILESTONE_GRANT")return item;changed=true;return Put(item,"milestoneEligibility",Put(item.Get("milestoneEligibility")!,"requiredCampaignSubjectIds",JsonValue.CreateArray(new[]{S("S1-99")})));})));Assert.That(changed,Is.True);d.Files["Cosmetics.json"]=Bytes(doc);d.RebindRaw();d.Binding=Put(d.Binding,"cosmeticsCatalog",Put(d.Binding.Get("cosmeticsCatalog")!,"hashSha256",S(LevelHashing.HashProjection(doc))));}
            byte[] bytes=d.BindingBytes;if(mutation=="duplicateField"){string s=Encoding.UTF8.GetString(bytes);bytes=Encoding.UTF8.GetBytes("{\"runtimeImportVersion\":1,"+s.Substring(1));}if(mutation!="bindingRaw")br=LocalSaveHash.Bytes(bytes);
            bool read=false;Assert.Catch(()=>{var sources=RuntimeGameSources.ReadContractTest(d.Files,bytes,br,rr,rj,ui);read=true;ProductionGameComposition.CreateAsync(sources,root,new Clock(),new Owner(),"fixture","fixture",CancellationToken.None).GetAwaiter().GetResult();},mutation);Assert.That(Directory.Exists(root),Is.False,mutation);if(mutation=="cosmeticReference")Assert.That(read,Is.True,"This case reaches composition's pure policy checks before any storage effect");
        }
        [Test]public void FixtureAndQaSourcesCannotEnterProductionReader()
        {var d=new Data();Assert.Throws<ArgumentException>(()=>RuntimeGameSources.ReadProduction(d.Files,d.BindingBytes,LocalSaveHash.Bytes(d.BindingBytes),H1,H2,new string('4',64),H3));d.Binding=Put(Put(d.Binding,"use",S("InternalPreview")),"status",S("DRAFT"));Assert.Throws<ArgumentException>(()=>RuntimeGameSources.ReadProduction(d.Files,d.BindingBytes,LocalSaveHash.Bytes(d.BindingBytes),H1,H2,new string('4',64),H3));}
        private static PuzzleSessionState Solve(PublicPuzzleInput p,Guid attempt,long elapsed)=> (PuzzleSessionState)typeof(CalibratedEndlessTests).GetMethod("Solve",BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,new object[]{p,attempt,elapsed,SessionMode.FIRST_RUN,false})!;
        private static void SolveView(SavedPublic240ReviewSession saved,Clock clock)
        {
            long begin=clock.Now;var view=saved.PuzzleView!;var puzzle=saved.Puzzles.SingleOrDefault(p=>p.Identity.Equals(saved.State.CurrentIdentity));if(puzzle==null){var active=saved.ConfirmedSnapshot!.Payload.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single();puzzle=active.PublicPuzzleInput;}var d=puzzle.ToDomainDefinition();var solved=PuzzleSolver.Solve(d,new SolverLimits(1000000,5000));Assert.That(solved.Classification,Is.EqualTo(SolverClassification.UNIQUE));Direction To(CellCoordinate a,CellCoordinate b)=>b.Y<a.Y?Direction.N:b.X>a.X?Direction.E:b.Y>a.Y?Direction.S:Direction.W;for(int i=0;i<solved.Path.Count;i++){var c=solved.Path[i];int ports=(1<<(int)(i==0?d.A.Side:To(c,solved.Path[i-1])))|(1<<(int)(i==solved.Path.Count-1?d.B.Side:To(c,solved.Path[i+1])));int shape=Enumerable.Range(0,6).Single(n=>TrackGeometry.Ports((TrackShape)n)==ports);clock.Now=begin+1000*i/(solved.Path.Count-1);Assert.That(view.Apply(c.X,c.Y,(DevelopmentTool)(shape+3),clock.Now),Is.EqualTo(DevelopmentCommandStatus.Applied));}
        }
        [Test]public void ActualCompositionInitializesEmptyThenPhysicalCampaignHintRestartAndEndlessResultRoundTrip()
        {
            var data=new Data();var sources=data.Read();string root=NewRoot();var owner=new Owner();var clock=new Clock();var c=owner.Pump(ProductionGameComposition.CreateAsync(sources,root,clock,owner,"fixture","fixture",CancellationToken.None));Assert.That(c.Profile.Namespace,Is.EqualTo("fixture-production-composition"));Assert.That(c.Profile.Use,Is.EqualTo(CatalogUse.ContractTest));using var saved=SavedPublic240ReviewSession.ForLockedRuntime(c.Persisted,c.Autosave,c.Catalog,clock,owner,c.Profile);owner.Pump(saved.InitializeAsync(CancellationToken.None));Assert.That(saved.State.Status,Is.EqualTo(SavedReviewStatus.NeedsInitialization));owner.Pump(saved.CreateProfileAsync(CancellationToken.None));Assert.That(saved.State.Progress!.FirstClearCount,Is.Zero);
            owner.Pump(saved.SelectPuzzleAsync(0,false,CancellationToken.None));var hint=owner.Pump(saved.PrepareHintAsync(c.Metagame,CancellationToken.None));Assert.That(hint,Is.Not.Null);Assert.That(owner.Pump(saved.CommitHintDisplayAsync(c.Metagame,hint!.AcknowledgeDisplayed(),CancellationToken.None)).Succeeded,Is.True);owner.Pump(saved.SetSuspendedAsync(false,CancellationToken.None));owner.Pump(saved.RestartAsync(CancellationToken.None));Assert.That(c.Metagame.HintCredits(saved.ConfirmedSnapshot!.Payload,saved.State.CurrentIdentity!),Is.EqualTo(2));SolveView(saved,clock);owner.Pump(saved.TickAutosaveAsync(CancellationToken.None));Assert.That(saved.State.Progress!.SelectedStars,Is.EqualTo(3));var attempt=saved.PuzzleView!.AttemptId;saved.BeginTrainRide(attempt);saved.PuzzleView.CompleteTrainRide(attempt);
            // Complete the remaining 239 controlled cases through actual Domain solves and the actual policy; no synthetic first-clear records.
            var loaded=owner.Pump(c.Repository.LoadAsync(CancellationToken.None));var state=loaded.Snapshot!.Payload;var policy=new LocalProgressPolicy(c.Catalog,c.Progress.Campaign,sources.CampaignCompletion,new CanonicalJsonAdapter(),new SaveLedgerCodec(new CanonicalJsonAdapter()));long generation=loaded.Snapshot.Generation+1;
            foreach(var input in c.Catalog.Puzzles.Skip(1))state=policy.Complete(state,Solve(input,Guid.NewGuid(),3000),generation,Utc).State;
            Assert.That(owner.Pump(c.Repository.CommitAsync(new SaveSnapshot<LocalProfileState>(c.Profile,c.Progress.CatalogSet.AggregateCatalog,generation,state),generation-1,CancellationToken.None)).Succeeded,Is.True);owner.Pump(saved.ReloadAsync(CancellationToken.None));Assert.That(saved.State.Progress!.FirstClearCount,Is.EqualTo(240));
            var target=c.CalibratedProfiles.Single(p=>p.Id=="dienstbeginn").Profile;Assert.That(owner.Pump(saved.StartEndlessAsync(c.EndlessPolicy,target,"2",CancellationToken.None)).Succeeded,Is.True);string id=saved.State.CurrentIdentity!.PuzzleId;Assert.That(id,Does.StartWith("E1-"));hint=owner.Pump(saved.PrepareHintAsync(c.Metagame,CancellationToken.None));Assert.That(hint,Is.Not.Null);owner.Pump(saved.CommitHintDisplayAsync(c.Metagame,hint!.AcknowledgeDisplayed(),CancellationToken.None));owner.Pump(saved.SetSuspendedAsync(false,CancellationToken.None));owner.Pump(saved.RestartAsync(CancellationToken.None));owner.Pump(saved.ReloadAsync(CancellationToken.None));Assert.That(owner.Pump(saved.ResumeEndlessAsync(c.EndlessPolicy,id,CancellationToken.None)).Succeeded,Is.True);SolveView(saved,clock);owner.Pump(saved.TickAutosaveAsync(CancellationToken.None));Assert.That(saved.State.Progress!.SelectedStars,Is.EqualTo(3));var final=owner.Pump(c.Repository.LoadAsync(CancellationToken.None));Assert.That(final.Succeeded,Is.True,final.ErrorCode);Assert.That(final.Snapshot!.Payload.Endless.OpenEndless.OfType<OpenEndlessCompletion>().Single().Result!.Stars,Is.EqualTo(3));
        }
    }
}
