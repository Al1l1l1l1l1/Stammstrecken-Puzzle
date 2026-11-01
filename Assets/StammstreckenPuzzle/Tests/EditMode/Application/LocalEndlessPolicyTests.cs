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
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Application.EditMode
{
    public sealed class LocalEndlessPolicyTests
    {
        private const string Utc="2026-10-21T00:00:00Z";
        private const string RootSource="Content/Catalogs/endless/completion-v1.json";
        private sealed class Fixture
        {
            internal readonly ICanonicalJson Json=LocalSaveContractFixtures.Json;
            internal readonly IPublicPuzzleCatalog Corpus;
            internal readonly ILockedCampaignCatalog Campaign;
            internal readonly ILockedCompletionCatalog Completion;
            internal readonly CatalogSetIdentity Catalogs;
            internal readonly SaveProfileIdentity Profile=new SaveProfileIdentity(SaveProfileIdentity.MobileCandidateNamespace,"endless-test",CatalogUse.InternalPreview);
            internal readonly ILocalLedger Ledger;
            internal readonly LocalProgressPolicy Progress;
            internal readonly LocalEndlessPolicy Endless;
            internal readonly CatalogIdentity EndlessCompletion;
            internal readonly LocalProfileState Empty,Cleared;
            internal readonly byte[] CompletionBytes;
            internal Fixture()
            {
                CatalogIdentity Id(string id,char hash)=>new CatalogIdentity(id,1,new string(hash,64),CatalogUse.InternalPreview,CatalogApprovalStatus.Draft);
                var publicId=Id("qa-mobile-candidate-endless-test-public",'a');var campaignId=Id("campaign-qa-mobile-candidate-endless-test",'b');var completionId=Id("completion-qa-mobile-candidate-endless-test",'c');
                var inputs=Enumerable.Range(1,240).Select(i=>Input("S1-test-"+i,2,2)).ToArray();Corpus=new LocalSaveContractFixtures.PublicCatalog(publicId,inputs);
                Campaign=new LocalSaveContractFixtures.CampaignCatalog(campaignId,inputs.Select((p,i)=>new LockedCampaignEntry(campaignId,p.Identity,"S1","S1-01","S1-01-01",i+1,Array.Empty<string>())).ToArray());
                Completion=new LocalSaveContractFixtures.CompletionCatalog(completionId,inputs.Select(p=>new CompletionValues(completionId,p.Identity,15,5,10,60,150,null,null)).ToArray());
                string projection="{\"publicCatalog\":"+LocalSaveHash.CatalogProjection(publicId)+",\"campaignCatalog\":"+LocalSaveHash.CatalogProjection(campaignId)+",\"completionCatalog\":"+LocalSaveHash.CatalogProjection(completionId)+"}";
                Catalogs=CatalogSetIdentity.Create(publicId,campaignId,completionId,new CatalogIdentity("qa-mobile-candidate-endless-test-set",1,LocalSaveHash.Bytes(LocalSaveHash.Canonical(Json,projection)),CatalogUse.InternalPreview,CatalogApprovalStatus.Draft),Json);
                Ledger=LocalSaveContractFixtures.Construct<ILocalLedger>("STP.Infrastructure.Persistence","STP.Infrastructure.Persistence.SaveLedgerCodec",Json);
                Progress=new LocalProgressPolicy(Corpus,Campaign,Completion,Json,Ledger);Empty=Progress.NewProfile(Profile,Catalogs,Utc).WithMetagame(LocalMetagameState.Empty);
                var levels=new List<LocalLevelProgress>();var journal=new List<LocalStoredEntry>();var claims=new List<LocalRewardClaim>();
                EconomyTerminal Grant(string tx,string reason,string subject,int amount){var t=EconomyTerminal.Create(tx,reason,subject,Completion.Identity,"reward:"+reason+":"+subject,amount,1,Utc);journal.Add(Ledger.Next(Empty.LedgerCheckpoint,journal,t));return t;}
                foreach(var p in Corpus.Puzzles){var a=Guid.NewGuid();var t=Grant("level:"+p.Identity.PuzzleId+":first-clear","LEVEL_FIRST_CLEAR",p.Identity.PuzzleId,15);levels.Add(LocalLevelProgress.Create(p.Identity,LocalFirstClear.Create(a,"puzzle-solved:"+a.ToString("D"),Utc,10,t),1,10,null,true,null,null));}
                claims.Add(CampaignProgressTerminal.Create("ROUTE","S1-01-01",Grant("route:S1-01-01:clear","ROUTE_CLEAR","S1-01-01",60)));
                claims.Add(CampaignProgressTerminal.Create("SECTION","S1-01",Grant("section:S1-01:clear","SECTION_CLEAR","S1-01",150)));
                Cleared=Empty.WithProgress(levels,Empty.LedgerCheckpoint,journal,Progress.Cache(levels),Empty.Drafts,claims);Progress.Validate(Cleared);
                string root=Environment.GetEnvironmentVariable("STP_SOURCE_ROOT")??Directory.GetCurrentDirectory();CompletionBytes=File.ReadAllBytes(Path.Combine(root,RootSource));
                var reader=LocalSaveContractFixtures.Construct<object>("STP.Infrastructure.Content","STP.Infrastructure.Content.LockedEndlessCompletion",CompletionBytes,LocalSaveHash.Bytes(LocalSaveHash.Canonical(Json,Encoding.UTF8.GetString(CompletionBytes))),Json);
                EndlessCompletion=(CatalogIdentity)LocalSaveContractFixtures.Get(reader,"Identity")!;Endless=new LocalEndlessPolicy(Corpus,EndlessCompletion,Json,Ledger);
            }
            internal static Fixture Create()=>new Fixture();
            internal EndlessSearchProfile Search(bool impossible=false)=>new EndlessSearchProfile("dienstbeginn",new string('e',64),Corpus.Identity.HashSha256,new PuzzleCandidateParameters(4,5,35,75,impossible?1000000:10,1,impossible?1:1000,20000,10000,2000,4,85));
            internal SaveSnapshot<LocalProfileState> Snapshot(LocalProfileState s,long generation=1)=>new SaveSnapshot<LocalProfileState>(Profile,Catalogs.AggregateCatalog,generation,s);
            internal LocalProfileState Active(out LocalEndlessCandidate candidate,string seed="1")
            {var state=Endless.Reserve(Cleared,Search(),seed);candidate=Endless.GenerateAsync(Snapshot(state,2),state.Endless.OpenEndless.Single().Id,CancellationToken.None).GetAwaiter().GetResult()!;Assert.That(candidate,Is.Not.Null);return Endless.Promote(state,candidate,Guid.NewGuid());}
        }
        private static PublicPuzzleInput Input(string id,int width,int height)
        {
            var rows=new int[height];rows[0]=width;var cols=Enumerable.Repeat(1,width).ToArray();string projection="{\"puzzleId\":"+LocalSaveHash.Quote(id)+",\"rulesetVersion\":\"train-track-v1\",\"grid\":{\"width\":"+width+",\"height\":"+height+"},\"endpoints\":{\"a\":{\"side\":\"W\",\"index\":0},\"b\":{\"side\":\"E\",\"index\":0}},\"rowCounts\":["+string.Join(",",rows)+"],\"columnCounts\":["+string.Join(",",cols)+"]}";
            return new PublicPuzzleInput(new PublicPuzzleIdentity(id,PublicPuzzleIdentity.SemanticHashProfile,LocalSaveHash.Bytes(LocalSaveHash.Canonical(LocalSaveContractFixtures.Json,projection))),"train-track-v1",width,height,PublicPuzzleSide.W,0,PublicPuzzleSide.E,0,rows,cols);
        }
        private static PuzzleSessionState Solve(ActiveEndlessDraft active)
        {
            var definition=active.PublicPuzzleInput.ToDomainDefinition();var solved=PuzzleSolver.Solve(definition,new SolverLimits(1000000,5000));Assert.That(solved.Classification,Is.EqualTo(SolverClassification.UNIQUE));var s=active.Checkpoint.Restore(definition,active.Id,active.PublicPuzzleHash.Profile,active.PublicPuzzleHash.Sha256).Value!;long time=0;if(s.Phase==SessionPhase.Suspended)s=PuzzleCommandHandler.Handle(s,new ResumeFromLifecycle(Guid.NewGuid(),s.Revision),0).State!;
            string Side(CellCoordinate a,CellCoordinate b)=>a.X<b.X?"E":a.X>b.X?"W":a.Y<b.Y?"S":"N";
            for(int i=0;i<solved.Path.Count;i++)
            {
                var cell=solved.Path[i];string first=i==0?definition.A.Side.ToString():Side(cell,solved.Path[i-1]);string second=i==solved.Path.Count-1?definition.B.Side.ToString():Side(cell,solved.Path[i+1]);string pair=first+second;
                CellContent track=pair.Contains("N")&&pair.Contains("S")?CellContent.TRACK_NS:pair.Contains("E")&&pair.Contains("W")?CellContent.TRACK_EW:pair.Contains("N")&&pair.Contains("E")?CellContent.TRACK_NE:pair.Contains("E")&&pair.Contains("S")?CellContent.TRACK_ES:pair.Contains("S")&&pair.Contains("W")?CellContent.TRACK_SW:CellContent.TRACK_WN;
                s=PuzzleCommandHandler.Handle(s,new ApplyCellContent(Guid.NewGuid(),s.Revision,cell,track),++time).State!;
            }
            Assert.That(s.Phase,Is.EqualTo(SessionPhase.SolvedPendingCommit));return s;
        }
        [Test]
        public void SeasonFirstClearsGateReserveAndWatermarkKeepsTenThousandRetiredOrdinalsTerminal()
        {
            var f=Fixture.Create();Assert.That(f.Endless.IsUnlocked(f.Empty),Is.False);Assert.Throws<ArgumentException>(()=>f.Endless.Reserve(f.Empty,f.Search(),"1"));var s=f.Cleared;
            // Already verified compact checkpoint isolates the ordinal bound from unrelated campaign journal hashing.
            var last=s.EconomyJournal.Last();s=s.WithProgress(s.Levels,LocalLedgerCheckpoint.Create(last.Sequence,f.Ledger.Validate(s),last.EntryChainHashSha256,1,Array.Empty<LocalStoredEntry>()),Array.Empty<LocalStoredEntry>(),s.SeasonProgressCache,s.Drafts,s.RewardClaims);
            for(int i=0;i<10000;i++){s=f.Endless.Reserve(s,f.Search(),"1");s=f.Endless.Abandon(s,s.Endless.OpenEndless.Single().Id);}
            Assert.That(s.Endless.HighestReservedOrdinal,Is.EqualTo("10000"));Assert.That(s.Endless.OpenEndless,Is.Empty);Assert.That(s.RewardClaims.Count,Is.EqualTo(f.Cleared.RewardClaims.Count));Assert.That(f.Ledger.Validate(s),Is.EqualTo(f.Ledger.Validate(f.Cleared)));
        }
        [Test]
        public void ActualGeneratedInputPromotesResumesAndOnlyActualSolvedAttemptCreatesBaseTerminal()
        {
            var f=Fixture.Create();var active=f.Active(out var generated);var draft=active.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single();Assert.That(draft.PublicPuzzleInput.Identity.PuzzleId,Does.StartWith("E1-"));Assert.That(draft.Checkpoint.Mode,Is.EqualTo(SessionMode.ENDLESS));Assert.That(active.Endless.RecentPublicInputs.Single().Identity,Is.EqualTo(generated.Puzzle.Identity));
            Assert.Throws<ArgumentException>(()=>f.Endless.Complete(active,draft.Checkpoint.Restore(draft.PublicPuzzleInput.ToDomainDefinition(),draft.Id,draft.PublicPuzzleHash.Profile,draft.PublicPuzzleHash.Sha256).Value!,3,Utc));
            var completed=f.Endless.Complete(active,Solve(draft),3,Utc);var claim=completed.Endless.OpenEndless.OfType<OpenEndlessCompletion>().Single();Assert.That(LocalEndlessPolicy.CompletionSourceHash(claim.Claim),Is.EqualTo(f.EndlessCompletion.HashSha256));Assert.That(f.Ledger.Validate(completed),Is.EqualTo(f.Ledger.Validate(active)+15));
            var skipped=f.Endless.SkipReward(completed,claim.Id);Assert.That(skipped.Endless.OpenEndless,Is.Empty);Assert.That(f.Endless.Abandon(skipped,claim.Id),Is.SameAs(skipped));
            var hints=new LocalMetagamePolicy(f.Corpus,f.Completion.Identity,f.Ledger);Assert.Throws<ArgumentException>(()=>hints.HintCredits(skipped,draft.PublicPuzzleInput.Identity));var late=new AdRewardReservation(LocalHintRecord.ClaimId(draft.PublicPuzzleInput.Identity),"old",1);Assert.That(hints.ConfirmHintVideo(skipped,draft.PublicPuzzleInput.Identity,new AdRewardEvidence(late,"old")),Is.SameAs(skipped));
        }
        [Test]
        public void GeneratedSolutionAndProofHashesEqualExistingTypedContentContractsIncludingTrackForms()
        {
            var f=Fixture.Create();var shapes=new HashSet<string>();var endpointSides=new HashSet<string>();
            for(int seed=1;seed<=12;seed++)
            {
                var state=f.Active(out var candidate,seed.ToString(System.Globalization.CultureInfo.InvariantCulture));var active=state.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single();var actual=PuzzleSolver.Solve(candidate.Puzzle.ToDomainDefinition(),new SolverLimits(1000000,5000));var solved=Solve(active);var assembly=Assembly.Load("STP.Infrastructure.Content");var cellType=assembly.GetType("STP.Infrastructure.Content.LevelPathCellDto",true)!;var trackType=assembly.GetType("STP.Infrastructure.Content.LevelTrack",true)!;var path=Array.CreateInstance(cellType,actual.Path.Count);endpointSides.Add(candidate.Puzzle.A.Side.ToString());endpointSides.Add(candidate.Puzzle.B.Side.ToString());
                for(int i=0;i<actual.Path.Count;i++){var c=actual.Path[i];string form=solved.Cells[c.Y*candidate.Puzzle.Width+c.X].ToString();shapes.Add(form);var track=Enum.Parse(trackType,form);var dto=Activator.CreateInstance(cellType,BindingFlags.Instance|BindingFlags.NonPublic,null,new object[]{c.X,c.Y,track},null)!;path.SetValue(dto,i);}
                var hashType=assembly.GetType("STP.Infrastructure.Content.ProfiledHash",true)!;var publicHash=Activator.CreateInstance(hashType,new object[]{candidate.Puzzle.Identity.HashProfile,candidate.Puzzle.Identity.HashSha256})!;var hashing=assembly.GetType("STP.Infrastructure.Content.LevelHashing",true)!;var projection=hashing.GetMethod("SolutionProjection")!.Invoke(null,new object[]{active.Id,publicHash,path})!;var computed=hashing.GetMethod("Compute")!.Invoke(null,new object[]{"STP-SOLUTION-JCS-1",projection})!;var solution=LocalSaveContractFixtures.Get(computed,"Hash")!;Assert.That(LocalSaveContractFixtures.Get(solution,"Sha256"),Is.EqualTo(candidate.SolutionHash.Sha256));
                var m=actual.Metrics!;var metrics=Activator.CreateInstance(assembly.GetType("STP.Infrastructure.Content.ProofMetrics",true)!,BindingFlags.Instance|BindingFlags.NonPublic,null,new object[]{m.SearchNodes,m.DeductionSteps,(long)m.MaxDeductionDepth,(long)m.RequiredGuessDepth},null)!;var placeholder=Activator.CreateInstance(hashType,new object[]{"STP-PROOF-JCS-1",new string('0',64)})!;var document=Activator.CreateInstance(assembly.GetType("STP.Infrastructure.Content.ProofV1Document",true)!,BindingFlags.Instance|BindingFlags.NonPublic,null,new object[]{active.Id,publicHash,solution,"solver-v2",metrics,placeholder},null)!;
                var proofProjection=LocalSaveContractFixtures.Call(document,"Projection")!;var proofComputed=hashing.GetMethod("Compute")!.Invoke(null,new object[]{"STP-PROOF-JCS-1",proofProjection})!;Assert.That(LocalSaveContractFixtures.Get(LocalSaveContractFixtures.Get(proofComputed,"Hash")!,"Sha256"),Is.EqualTo(candidate.ProofHash.Sha256));
            }
            Assert.That(shapes.Count,Is.EqualTo(6));Assert.That(endpointSides.Count,Is.EqualTo(4));
        }
        [Test]
        public void ExactRewardReservationGrantsTenOnceAndDefinitiveSkipNeverManufacturesProviderEvidence()
        {
            var f=Fixture.Create();var active=f.Active(out _);var draft=active.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single();var completed=f.Endless.Complete(active,Solve(draft),3,Utc);var reserved=f.Endless.ReserveReward(completed,draft.Id,4);var reservation=LocalEndlessPolicy.RewardReservation(SaveCommitResult<LocalProfileState>.Success(f.Snapshot(reserved,4)),draft.Id);
            Assert.Throws<ArgumentException>(()=>f.Endless.SkipReward(reserved,draft.Id));Assert.That(f.Endless.ConfirmReward(reserved,new AdRewardEvidence(reservation,"wrong"),5,Utc),Is.SameAs(reserved));var wrong=new AdRewardReservation(reservation.ClaimId,reservation.OperationId,5);Assert.That(f.Endless.ConfirmReward(reserved,new AdRewardEvidence(wrong,wrong.OperationId),5,Utc),Is.SameAs(reserved));
            var pending=f.Endless.ResolveReward(reserved,reservation,false);Assert.That(pending.Endless.OpenEndless.OfType<OpenEndlessCompletion>().Single().Claim.ClaimStatus,Is.EqualTo("RECONCILIATION_REQUIRED"));var paid=f.Endless.ConfirmReward(pending,new AdRewardEvidence(reservation,reservation.OperationId),5,Utc);Assert.That(f.Ledger.Validate(paid),Is.EqualTo(f.Ledger.Validate(completed)+10));Assert.That(paid.Endless.OpenEndless,Is.Empty);Assert.That(f.Endless.ConfirmReward(paid,new AdRewardEvidence(reservation,reservation.OperationId),6,Utc),Is.SameAs(paid));Assert.That(paid.PendingOperations,Is.Empty);
        }
        [Test]
        public void ExactBoundSourceIsRequiredForCompletionAndReceiptSaveRoundTrips()
        {
            var f=Fixture.Create();var active=f.Active(out _);var done=f.Endless.Complete(active,Solve(active.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single()),3,Utc);var codec=Codec(f,true);var bytes=(byte[])LocalSaveContractFixtures.Call(codec,"Encode",f.Snapshot(done,3),Utc,"test","endless")!;var decoded=LocalSaveContractFixtures.Call(codec,"Decode",bytes)!;Assert.That(LocalSaveContractFixtures.Get(decoded,"Status")!.ToString(),Is.EqualTo("Valid"));
            var missing=LocalSaveContractFixtures.Call(Codec(f,false),"Decode",bytes)!;Assert.That(LocalSaveContractFixtures.Get(missing,"Status")!.ToString(),Is.EqualTo("Unsupported"));
            var reserved=f.Endless.ReserveReward(done,done.Endless.OpenEndless.Single().Id,9);Assert.Throws<TargetInvocationException>(()=>LocalSaveContractFixtures.Call(codec,"Encode",f.Snapshot(reserved,4),Utc,"test","future-provider"));
            string id=done.Endless.OpenEndless.Single().Id;var retired=f.Endless.SkipReward(done,id);var retiredBytes=(byte[])LocalSaveContractFixtures.Call(codec,"Encode",f.Snapshot(retired,4),Utc,"test","endless")!;Assert.That(LocalSaveContractFixtures.Get(LocalSaveContractFixtures.Call(Codec(f,false),"Decode",retiredBytes)!,"Status")!.ToString(),Is.EqualTo("Unsupported"));
        }
        [Test]
        public void CapacityOrdinalAndDescriptorMutationRejectWithoutReplacementReservations()
        {
            var f=Fixture.Create();var s=f.Cleared;for(int i=0;i<20;i++)s=f.Endless.Reserve(s,f.Search(),"1");Assert.That(s.Endless.OpenEndless.Count,Is.EqualTo(20));Assert.Throws<ArgumentException>(()=>f.Endless.Reserve(s,f.Search(),"1"));var first=s.Endless.OpenEndless.First();s=f.Endless.Abandon(s,first.Id);s=f.Endless.Reserve(s,f.Search(),"1");Assert.That(s.Endless.HighestReservedOrdinal,Is.EqualTo("21"));Assert.That(s.Endless.OpenEndless.Any(r=>r.Id==first.Id),Is.False);
            var maximum=LocalEndlessEconomy.WithEndless(f.Cleared,LocalEndlessState.Runtime(ulong.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture),Array.Empty<LocalOpenEndless>(),Array.Empty<PublicPuzzleInput>(),Array.Empty<EconomyTerminal>()));Assert.Throws<ArgumentException>(()=>f.Endless.Reserve(maximum,f.Search(),"1"));
            var original=s.Endless.OpenEndless.First();var descriptor=LocalEndlessDescriptor.Create(original.Descriptor.RulesetVersion,original.Descriptor.GeneratorVersion,"2",original.GenerationOrdinal,original.Descriptor.Parameters,original.Descriptor.ParameterHashSha256);var mutated=ReservedEndless.Create(original.GenerationOrdinal,original.Id,descriptor);var changed=LocalEndlessEconomy.WithEndless(s,LocalEndlessState.Runtime(s.Endless.HighestReservedOrdinal,s.Endless.OpenEndless.Select(r=>r.Id==original.Id?mutated:r).ToArray(),s.Endless.RecentPublicInputs,s.Endless.EconomyReceipts));Assert.Throws<TargetInvocationException>(()=>LocalSaveContractFixtures.Call(Codec(f,true),"Encode",f.Snapshot(changed,2),Utc,"test","descriptor-mutated"));
        }
        [Test]
        public void NoveltyWindowPinsFiftyPublicInputsThenEvictsOnlyOldestAndRejectsForgedHashes()
        {
            var f=Fixture.Create();var recent=Enumerable.Range(1,50).Select(i=>Input("E1-"+i.ToString("x64"),2,2)).ToArray();var state=LocalEndlessEconomy.WithEndless(f.Cleared,LocalEndlessState.Runtime("50",Array.Empty<LocalOpenEndless>(),recent,Array.Empty<EconomyTerminal>()));var reserved=f.Endless.Reserve(state,f.Search(),"1");var record=reserved.Endless.OpenEndless.Single();var parameters=(SearchGeneratorParameters)record.Descriptor.Parameters;Assert.That(parameters.RecentPublicInputs.Select(p=>p.Identity),Is.EqualTo(recent.Select(p=>p.Identity)));
            var candidate=f.Endless.GenerateAsync(f.Snapshot(reserved,2),record.Id,CancellationToken.None).GetAwaiter().GetResult()!;Assert.That(candidate,Is.Not.Null);var promoted=f.Endless.Promote(reserved,candidate,Guid.NewGuid());Assert.That(promoted.Endless.RecentPublicInputs.Count,Is.EqualTo(50));Assert.That(promoted.Endless.RecentPublicInputs.First().Identity,Is.EqualTo(recent[1].Identity));Assert.That(promoted.Endless.RecentPublicInputs.Last().Identity,Is.EqualTo(candidate.Puzzle.Identity));Assert.That(parameters.RecentPublicInputs.First().Identity,Is.EqualTo(recent[0].Identity));
            var p=recent[0];var forged=new PublicPuzzleInput(new PublicPuzzleIdentity(p.Identity.PuzzleId,p.Identity.HashProfile,new string('f',64)),p.RulesetVersion,p.Width,p.Height,p.A.Side,p.A.Index,p.B.Side,p.B.Index,p.RowCounts,p.ColumnCounts);var invalid=LocalEndlessEconomy.WithEndless(state,LocalEndlessState.Runtime("50",Array.Empty<LocalOpenEndless>(),new[]{forged},Array.Empty<EconomyTerminal>()));Assert.Throws<TargetInvocationException>(()=>LocalSaveContractFixtures.Call(Codec(f,true),"Encode",f.Snapshot(invalid,2),Utc,"test","recent-hash-mutated"));Assert.Throws<ArgumentException>(()=>LocalEndlessState.Runtime("51",Array.Empty<LocalOpenEndless>(),recent.Concat(new[]{candidate.Puzzle}).ToArray(),Array.Empty<EconomyTerminal>()));
        }
        private static object Codec(Fixture f,bool source)
        {
            var a=Assembly.Load("STP.Infrastructure.Persistence");var b=a.GetType("STP.Infrastructure.Persistence.RetainedCompletionRewardBinding",true)!;var values=Array.CreateInstance(b,source?1:0);if(source)values.SetValue(b.GetMethod("FromDocument")!.Invoke(null,new object[]{LocalPostClearPolicy.PolicyVersion,f.CompletionBytes,f.Json,CatalogUse.InternalPreview}),0);
            var retained=a.GetType("STP.Infrastructure.Persistence.RetainedContractBindings",true)!.GetMethod("ForRuntimeWithRewards")!.Invoke(null,new object?[]{CatalogUse.InternalPreview,null,values})!;return a.GetType("STP.Infrastructure.Persistence.LocalSaveCodec",true)!.GetMethod("ForMetagame")!.Invoke(null,new object[]{f.Json,f.Profile,f.Catalogs,f.Corpus,f.Campaign,f.Completion,retained})!;
        }
        [Test]
        public void TenThousandTerminalReceiptsStayBoundedToActualJournalAndTailAndMutationsFailClosed()
        {
            var f=Fixture.Create();var s=f.Cleared;long initial=f.Ledger.Validate(s);
            for(int i=1;i<=10000;i++)
            {
                if(s.EconomyJournal.Count>=384)s=f.Ledger.Compact(s,i+1);
                string id="E1-"+i.ToString("x64");var receipt=EconomyTerminal.Create("level:"+id+":first-clear","LEVEL_FIRST_CLEAR",id,f.EndlessCompletion,"reward:LEVEL_FIRST_CLEAR:"+id,15,i+1,Utc);var entry=f.Ledger.Next(s.LedgerCheckpoint,s.EconomyJournal,receipt);
                var endless=LocalEndlessState.Runtime(i.ToString(System.Globalization.CultureInfo.InvariantCulture),Array.Empty<LocalOpenEndless>(),s.Endless.RecentPublicInputs,s.Endless.EconomyReceipts.Concat(new[]{receipt}).ToArray());s=LocalEndlessEconomy.WithEndless(s,endless);s=s.WithProgress(s.Levels,s.LedgerCheckpoint,s.EconomyJournal.Concat(new[]{entry}).ToArray(),s.SeasonProgressCache,s.Drafts,s.RewardClaims);
                Assert.That(s.Endless.EconomyReceipts.Count,Is.LessThanOrEqualTo(576));
            }
            Assert.That(f.Ledger.Validate(s),Is.EqualTo(initial+150000));Assert.That(s.Endless.HighestReservedOrdinal,Is.EqualTo("10000"));Assert.That(s.Endless.OpenEndless,Is.Empty);Assert.That(s.Endless.EconomyReceipts.Count,Is.EqualTo(s.EconomyJournal.Count+s.LedgerCheckpoint.JournalDiagnosticTail.Count));Assert.That(s.Endless.EconomyReceipts.Any(r=>r.SubjectId=="E1-"+1.ToString("x64")),Is.False);
            var codec=Codec(f,true);var encoded=(byte[])LocalSaveContractFixtures.Call(codec,"Encode",f.Snapshot(s,10001),Utc,"test","10k-receipts")!;Assert.That(LocalSaveContractFixtures.Get(LocalSaveContractFixtures.Call(codec,"Decode",encoded)!,"Status")!.ToString(),Is.EqualTo("Valid"));
            LocalProfileState Receipts(EconomyTerminal[] values)=>LocalEndlessEconomy.WithEndless(s,LocalEndlessState.Runtime(s.Endless.HighestReservedOrdinal,s.Endless.OpenEndless,s.Endless.RecentPublicInputs,values));
            var original=s.Endless.EconomyReceipts.ToArray();Assert.Throws<TargetInvocationException>(()=>LocalSaveContractFixtures.Call(codec,"Encode",f.Snapshot(Receipts(original.Skip(1).ToArray()),10001),Utc,"test","missing-receipt"));
            var old=original[0];original[0]=EconomyTerminal.Create(old.TransactionId,old.ReasonCode,old.SubjectId,old.Catalog,old.ClaimId,old.Amount+1,old.CommittedGeneration,old.CreatedAtUtc);Assert.Throws<TargetInvocationException>(()=>LocalSaveContractFixtures.Call(codec,"Encode",f.Snapshot(Receipts(original),10001),Utc,"test","wrong-amount"));
        }
        private sealed class QueueDispatcher:ILocalSaveDispatcher
        {
            private readonly int owner=Thread.CurrentThread.ManagedThreadId;private readonly ConcurrentQueue<(Action Action,TaskCompletionSource<bool> Done)> queue=new ConcurrentQueue<(Action,TaskCompletionSource<bool>)>();
            public Task InvokeAsync(Action action){if(Thread.CurrentThread.ManagedThreadId==owner){action();return Task.CompletedTask;}var done=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);queue.Enqueue((action,done));return done.Task;}
            internal void Drain(){while(queue.TryDequeue(out var next)){try{next.Action();next.Done.TrySetResult(true);}catch(Exception e){next.Done.TrySetException(e);}}}
        }
        private sealed class Live:IDisposable
        {
            internal readonly Fixture Data=Fixture.Create();internal readonly QueueDispatcher Dispatcher=new QueueDispatcher();internal readonly LocalSaveContractFixtures.Clock Clock=new LocalSaveContractFixtures.Clock();internal readonly LocalSaveContractFixtures.Repository Repo=new LocalSaveContractFixtures.Repository();internal readonly PersistedPuzzleSession Use;internal readonly SavedPublic240ReviewSession Saved;
            internal Live(LocalProfileState? initial=null)
            {
                var snapshot=Data.Snapshot(initial??Data.Cleared);var observation=new SaveRepositoryObservation(Data.Profile,Data.Catalogs.AggregateCatalog,new[]{new SaveCandidateProvenance(SaveCandidateKind.Main,SaveCandidateStatus.Valid,1,new string('a',64),new string('b',64)),new SaveCandidateProvenance(SaveCandidateKind.Backup,SaveCandidateStatus.Missing,null,null,null),new SaveCandidateProvenance(SaveCandidateKind.Pending,SaveCandidateStatus.Missing,null,null,null)});
                Repo.Loaded=SaveLoadResult<LocalProfileState>.Success(snapshot,observation);Use=new PersistedPuzzleSession(Repo,Data.Corpus,Data.Progress,Data.Profile,Data.Catalogs,Clock,Dispatcher);Use.ConfigureEndless(Data.Endless);Saved=SavedPublic240ReviewSession.ForMobileCandidate(Use,new AutosaveCoordinator(Use),Data.Corpus,Clock,Dispatcher,Data.Profile);Pump(Saved.InitializeAsync(CancellationToken.None));
            }
            internal void Pump(Task task){var watch=Stopwatch.StartNew();while(!task.IsCompleted){Dispatcher.Drain();if(watch.ElapsedMilliseconds>30000)Assert.Fail("Endless worker did not finish while the owner dispatcher was pumped");Thread.Sleep(1);}Dispatcher.Drain();task.GetAwaiter().GetResult();}
            internal T Pump<T>(Task<T> task){Pump((Task)task);return task.GetAwaiter().GetResult();}
            public void Dispose(){Saved.Dispose();Dispatcher.Drain();}
        }
        [Test]
        public void AcknowledgedReservationRemainsReadyAndRetryableAfterDeterministicGenerationFailure()
        {
            using var f=new Live();var result=f.Pump(f.Saved.StartEndlessAsync(f.Data.Endless,f.Data.Search(true),"1",CancellationToken.None));Assert.That(result.Succeeded,Is.True);Assert.That(f.Saved.State.Status,Is.EqualTo(SavedReviewStatus.Ready));Assert.That(f.Saved.EndlessGenerationStatus,Is.EqualTo("budget"));Assert.That(f.Saved.ConfirmedSnapshot!.Payload.Endless.OpenEndless.Single(),Is.TypeOf<ReservedEndless>());Assert.That(f.Saved.CanEnterPuzzle(0),Is.True);Assert.That(f.Saved.PuzzleView,Is.Null);
            var descriptor=f.Saved.ConfirmedSnapshot.Payload.Endless.OpenEndless.Single();long generation=f.Saved.ConfirmedSnapshot.Generation;var retry=f.Pump(f.Saved.ResumeEndlessAsync(f.Data.Endless,descriptor.Id,CancellationToken.None));Assert.That(retry.Succeeded,Is.True);Assert.That(retry.Snapshot!.Generation,Is.EqualTo(generation));Assert.That(retry.Snapshot.Payload.Endless.OpenEndless.Single().Id,Is.EqualTo(descriptor.Id));Assert.That(f.Saved.State.Status,Is.EqualTo(SavedReviewStatus.Ready));
        }
        [Test]
        public void OlderReservationCannotPromoteACloneAfterAnotherReservationWasActuallyIssued()
        {
            var f=Fixture.Create();var first=f.Endless.Reserve(f.Cleared,f.Search(),"1");string firstId=first.Endless.OpenEndless.Single().Id;var second=f.Endless.Reserve(first,f.Search(),"1");string secondId=second.Endless.OpenEndless.Last().Id;var firstCandidate=f.Endless.GenerateAsync(f.Snapshot(second,2),firstId,CancellationToken.None).GetAwaiter().GetResult()!;var secondCandidate=f.Endless.GenerateAsync(f.Snapshot(second,2),secondId,CancellationToken.None).GetAwaiter().GetResult()!;
            Assert.That(PuzzleCandidateSearch.CanonicalSignature(firstCandidate.Puzzle.ToDomainDefinition()),Is.EqualTo(PuzzleCandidateSearch.CanonicalSignature(secondCandidate.Puzzle.ToDomainDefinition())));var issued=f.Endless.Promote(second,secondCandidate,Guid.NewGuid());var oldDescriptor=issued.Endless.OpenEndless.Single(e=>e.Id==firstId).Descriptor;
            Assert.Throws<ArgumentException>(()=>f.Endless.Promote(issued,firstCandidate,Guid.NewGuid()));using var live=new Live(issued);var resumed=live.Pump(live.Saved.ResumeEndlessAsync(live.Data.Endless,firstId,CancellationToken.None));Assert.That(resumed.Succeeded,Is.True);Assert.That(live.Saved.State.Status,Is.EqualTo(SavedReviewStatus.Ready));Assert.That(live.Saved.EndlessGenerationStatus,Is.EqualTo("unavailable"));var stillReserved=live.Saved.ConfirmedSnapshot!.Payload.Endless.OpenEndless.Single(e=>e.Id==firstId);Assert.That(stillReserved,Is.TypeOf<ReservedEndless>());Assert.That(stillReserved.Descriptor.ParameterHashSha256,Is.EqualTo(oldDescriptor.ParameterHashSha256));Assert.That(stillReserved.Descriptor.Seed,Is.EqualTo(oldDescriptor.Seed));Assert.That(live.Saved.ConfirmedSnapshot.Payload.Endless.RecentPublicInputs.Single().Identity,Is.EqualTo(secondCandidate.Puzzle.Identity));Assert.That(live.Saved.CanRunProfileAction,Is.True);
            Assert.That(live.Pump(live.Saved.AbandonEndlessAsync(live.Data.Endless,firstId,CancellationToken.None)).Succeeded,Is.True);Assert.That(live.Saved.ConfirmedSnapshot!.Payload.Endless.OpenEndless.Any(e=>e.Id==firstId),Is.False);
        }
        [Test]
        public void CurrentRecentSimilarityAlsoRejectsANonidenticalPreviouslyReservedCandidate()
        {
            var f=Fixture.Create();var first=f.Endless.Reserve(f.Cleared,f.Search(),"1");var firstCandidate=f.Endless.GenerateAsync(f.Snapshot(first,2),first.Endless.OpenEndless.Single().Id,CancellationToken.None).GetAwaiter().GetResult()!;bool checkedPair=false;
            for(int seed=2;seed<=12&&!checkedPair;seed++)
            {
                var second=f.Endless.Reserve(first,f.Search(),seed.ToString(System.Globalization.CultureInfo.InvariantCulture));var secondCandidate=f.Endless.GenerateAsync(f.Snapshot(second,2),second.Endless.OpenEndless.Last().Id,CancellationToken.None).GetAwaiter().GetResult()!;if(secondCandidate==null)continue;var a=firstCandidate.Puzzle.ToDomainDefinition();var b=secondCandidate.Puzzle.ToDomainDefinition();if(PuzzleCandidateSearch.CanonicalSignature(a)==PuzzleCandidateSearch.CanonicalSignature(b)||PuzzleCandidateSearch.SimilarityDistancePercent(a,b)>f.Search().Search.RecentSimilarityPercent)continue;
                var issued=f.Endless.Promote(second,secondCandidate,Guid.NewGuid());var error=Assert.Throws<ArgumentException>(()=>f.Endless.Promote(issued,firstCandidate,Guid.NewGuid()));Assert.That(error!.Message,Is.EqualTo("ENDLESS_PROMOTION_NOVELTY_CONFLICT"));Assert.That(issued.Endless.OpenEndless.OfType<ReservedEndless>().Single().Id,Is.EqualTo(firstCandidate.Reservation.Id));checkedPair=true;
            }
            Assert.That(checkedPair,Is.True,"The controlled technical seed corpus must include a nonidentical recent-similarity case");
        }
        [Test]
        public void ReservationCasConflictNeverOpensAnUnacknowledgedPuzzleAndRequiresActualRecovery()
        {
            using var f=new Live();f.Repo.OnCommit=(s,g,ct)=>Task.FromResult(SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Conflict,"TEST_CONCURRENT_WRITE"));var result=f.Pump(f.Saved.StartEndlessAsync(f.Data.Endless,f.Data.Search(),"1",CancellationToken.None));Assert.That(result.Status,Is.EqualTo(SaveCommitStatus.Conflict));Assert.That(f.Saved.ConfirmedSnapshot,Is.Null);Assert.That(f.Saved.PuzzleView,Is.Null);Assert.That(f.Use.Snapshot!.Payload.Endless.OpenEndless,Is.Empty);Assert.That(f.Saved.State.Status,Is.EqualTo(SavedReviewStatus.RecoveryRequired));
        }
        [Test]
        public void ActualActivePublicInputAndHintCheckpointResumeAfterFullCodecRoundTrip()
        {
            LocalProfileState restored;string id;Guid attempt;long revision;
            using(var f=new Live())
            {
                Assert.That(f.Pump(f.Saved.StartEndlessAsync(f.Data.Endless,f.Data.Search(),"1",CancellationToken.None)).Succeeded,Is.True);var hints=new LocalMetagamePolicy(f.Data.Corpus,f.Data.Completion.Identity,f.Data.Ledger);var hint=f.Pump(f.Saved.PrepareHintAsync(hints,CancellationToken.None));Assert.That(f.Pump(f.Saved.CommitHintDisplayAsync(hints,hint!.AcknowledgeDisplayed(),CancellationToken.None)).Succeeded,Is.True);var snapshot=f.Saved.ConfirmedSnapshot!;var active=snapshot.Payload.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single();id=active.Id;attempt=active.Checkpoint.AttemptId;revision=active.Checkpoint.Revision;
                var codec=Codec(f.Data,true);var encoded=(byte[])LocalSaveContractFixtures.Call(codec,"Encode",snapshot,Utc,"test","active-resume")!;var decoded=LocalSaveContractFixtures.Call(codec,"Decode",encoded)!;Assert.That(LocalSaveContractFixtures.Get(decoded,"Status")!.ToString(),Is.EqualTo("Valid"));restored=((SaveSnapshot<LocalProfileState>)LocalSaveContractFixtures.Get(decoded,"Snapshot")!).Payload;
            }
            using var reload=new Live(restored);Assert.That(reload.Pump(reload.Saved.ResumeEndlessAsync(reload.Data.Endless,id,CancellationToken.None)).Succeeded,Is.True);Assert.That(reload.Use.Session!.AttemptId,Is.EqualTo(attempt));Assert.That(reload.Use.Session.Revision,Is.GreaterThanOrEqualTo(revision));Assert.That(reload.Use.Session.HintCount,Is.EqualTo(1));Assert.That(reload.Use.Session.TimerStarted,Is.False);Assert.That(reload.Saved.PuzzleView!.Puzzle.Identity.PuzzleId,Is.EqualTo(id));Assert.That(reload.Saved.CurrentModeName,Is.EqualTo("ENDLESS"));
            var meta=new LocalMetagamePolicy(reload.Data.Corpus,reload.Data.Completion.Identity,reload.Data.Ledger);var identity=reload.Saved.PuzzleView.Puzzle.Identity;Assert.That(meta.HintCredits(reload.Saved.ConfirmedSnapshot!.Payload,identity),Is.EqualTo(2));reload.Pump(reload.Saved.RestartAsync(CancellationToken.None));Assert.That(reload.Use.Session.AttemptId,Is.Not.EqualTo(attempt));Assert.That(reload.Saved.PuzzleView!.Puzzle.Identity,Is.EqualTo(identity));Assert.That(meta.HintCredits(reload.Saved.ConfirmedSnapshot!.Payload,identity),Is.EqualTo(2));Assert.That(reload.Use.Session.TimerStarted,Is.False);
        }
        [TestCase("1")]
        [TestCase("2")]
        public void ActualEndlessResultSkipThenSecondStartReplacesRetiredViewWhileModalIsPaused(string secondSeed)
        {
            using var f = new Live();
            Assert.That(f.Pump(f.Saved.StartEndlessAsync(f.Data.Endless, f.Data.Search(), "1", CancellationToken.None)).Succeeded, Is.True);
            var firstView = f.Saved.PuzzleView!;
            string firstId = firstView.Puzzle.Identity.PuzzleId;
            var firstDraft = f.Saved.ConfirmedSnapshot!.Payload.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single();
            var solved = Solve(firstDraft);
            for (int y = 0; y < firstView.Height; y++)
                for (int x = 0; x < firstView.Width; x++)
                {
                    var content = solved.Cells[y * firstView.Width + x];
                    if (content < CellContent.TRACK_NS) continue;
                    var tool = (DevelopmentTool)Enum.Parse(typeof(DevelopmentTool), "Track" + content.ToString().Substring(6));
                    Assert.That(firstView.Apply(x, y, tool, ++f.Clock.MonotonicMilliseconds), Is.EqualTo(DevelopmentCommandStatus.Applied));
                }
            f.Pump(f.Saved.TickAutosaveAsync(CancellationToken.None));
            Assert.That(f.Use.CanPresentResults, Is.True);
            Assert.That(f.Saved.BeginTrainRide(firstView.AttemptId), Is.EqualTo(DevelopmentCommandStatus.Applied));
            Assert.That(firstView.CompleteTrainRide(firstView.AttemptId), Is.EqualTo(DevelopmentCommandStatus.Applied));
            Assert.That(f.Saved.State.Status, Is.EqualTo(SavedReviewStatus.Results));
            Assert.That(f.Pump(f.Saved.RunProfileActionAsync((use, ct) => use.SkipEndlessRewardAsync(f.Data.Endless, firstId, ct), CancellationToken.None)).Succeeded, Is.True);
            Assert.That(f.Saved.ConfirmedSnapshot!.Payload.Endless.OpenEndless, Is.Empty);
            Assert.That(f.Use.CanPresentResults, Is.True);
            Assert.That(f.Saved.PuzzleView, Is.SameAs(firstView));
            f.Saved.SetPresentationPaused(true);
            f.Pump(f.Saved.SetSuspendedAsync(true, CancellationToken.None));
            Assert.That(f.Saved.State.CanNavigate, Is.False);
            Assert.That(f.Saved.CanRunProfileAction, Is.True);
            var second = f.Pump(f.Saved.StartEndlessAsync(f.Data.Endless, f.Data.Search(), secondSeed, CancellationToken.None));
            Assert.That(second.Succeeded, Is.True, second.ErrorCode);
            Assert.That(second.Snapshot!.Payload.Endless.HighestReservedOrdinal, Is.EqualTo("2"));
            var secondDraft = second.Snapshot.Payload.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single();
            Assert.That(secondDraft.Id, Is.Not.EqualTo(firstId));
            Assert.That(f.Saved.PuzzleView, Is.Not.SameAs(firstView));
            Assert.That(f.Saved.PuzzleView!.Puzzle.Identity, Is.EqualTo(secondDraft.PublicPuzzleInput.Identity));
            Assert.That(f.Use.Session!.PuzzleId, Is.EqualTo(secondDraft.Id));
            Assert.That(f.Use.CanPresentResults, Is.False);
            Assert.That(f.Saved.State.Status, Is.EqualTo(SavedReviewStatus.Ready));
            Assert.That(f.Saved.EndlessGenerationStatus, Is.EqualTo("ready"));
            Assert.That(f.Saved.PuzzleView.IsPaused, Is.True);
            Assert.That(f.Saved.State.CanMutate, Is.False);
            f.Pump(f.Saved.SetSuspendedAsync(false, CancellationToken.None));
            f.Saved.SetPresentationPaused(false);
            Assert.That(f.Saved.State.CanMutate, Is.True);
            Assert.That(firstView.Apply(0, 0, DevelopmentTool.TrackNS, ++f.Clock.MonotonicMilliseconds), Is.EqualTo(DevelopmentCommandStatus.Rejected));
        }


        [Test]
        public void GenericSavedHintThenActualEndlessCompletionHasOneStarAndRoundTripsWithoutDanglingHint()
        {
            using var f=new Live();Assert.That(f.Pump(f.Saved.StartEndlessAsync(f.Data.Endless,f.Data.Search(),"1",CancellationToken.None)).Succeeded,Is.True);Assert.That(f.Saved.CurrentModeName,Is.EqualTo("ENDLESS"));var view=f.Saved.PuzzleView!;var identity=view.Puzzle.Identity;Assert.That(f.Saved.State.SelectedIndex,Is.EqualTo(-1));
            var meta=new LocalMetagamePolicy(f.Data.Corpus,f.Data.Completion.Identity,f.Data.Ledger);var hint=f.Pump(f.Saved.PrepareHintAsync(meta,CancellationToken.None));Assert.That(hint,Is.Not.Null);Assert.That(f.Use.Session!.TimerStarted,Is.False);var receipt=hint!.AcknowledgeDisplayed();Assert.That(f.Pump(f.Saved.CommitHintDisplayAsync(meta,receipt,CancellationToken.None)).Succeeded,Is.True);Assert.That(f.Use.Session.HintCount,Is.EqualTo(1));Assert.That(f.Pump(f.Saved.CommitHintDisplayAsync(meta,receipt,CancellationToken.None)).Succeeded,Is.True);Assert.That(f.Use.Session.HintCount,Is.EqualTo(1));
            f.Pump(f.Saved.SetSuspendedAsync(false,CancellationToken.None));view.Resume(f.Clock.MonotonicMilliseconds);var draft=f.Saved.ConfirmedSnapshot!.Payload.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single();var solved=Solve(draft);
            for(int y=0;y<view.Height;y++)for(int x=0;x<view.Width;x++){var content=solved.Cells[y*view.Width+x];if(content>=CellContent.TRACK_NS){var tool=(DevelopmentTool)Enum.Parse(typeof(DevelopmentTool),"Track"+content.ToString().Substring(6));Assert.That(view.Apply(x,y,tool,++f.Clock.MonotonicMilliseconds),Is.EqualTo(DevelopmentCommandStatus.Applied));}}
            f.Pump(f.Saved.TickAutosaveAsync(CancellationToken.None));Assert.That(f.Use.CanPresentResults,Is.True);Assert.That(f.Use.CurrentEndlessStars,Is.EqualTo(1));Assert.That(f.Saved.State.Progress!.SelectedStars,Is.EqualTo(1));var snapshot=f.Saved.ConfirmedSnapshot!;Assert.That(snapshot.Payload.Metagame!.Hints.Any(h=>h.Identity.Equals(identity)),Is.False);
            var codec=Codec(f.Data,true);var encoded=(byte[])LocalSaveContractFixtures.Call(codec,"Encode",snapshot,Utc,"test","generic")!;Assert.That(LocalSaveContractFixtures.Get(LocalSaveContractFixtures.Call(codec,"Decode",encoded)!,"Status")!.ToString(),Is.EqualTo("Valid"));
            string id=snapshot.Payload.Endless.OpenEndless.OfType<OpenEndlessCompletion>().Single().Id;Assert.That(f.Pump(f.Saved.ResumeEndlessAsync(f.Data.Endless,id,CancellationToken.None)).Succeeded,Is.True);Assert.That(f.Saved.ConfirmedSnapshot!.Generation,Is.EqualTo(snapshot.Generation));
        }
    }
}
