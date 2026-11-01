using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;

namespace STP.Tests.Application.EditMode
{
    public sealed class LocalProgressPolicyTests
    {
        private sealed class DraftFixture
        {
            internal readonly LocalSaveContractFixtures.Setup Base=new LocalSaveContractFixtures.Setup();internal readonly LocalSaveContractFixtures.PublicCatalog Public;internal readonly LocalProgressPolicy Policy;internal readonly LocalProfileState State;internal readonly LocalDraft Current;
            internal DraftFixture(int known,int orphans,bool future=false,bool equalInstants=false,bool collision=false)
            {
                var items=Enumerable.Range(0,21).Select(i=>new PublicPuzzleInput(new PublicPuzzleIdentity("known-"+i.ToString("D2"),"STP-PUZZLE-SEMANTIC-JCS-1",new string('a',64)),"train-track-v1",2,2,PublicPuzzleSide.W,0,PublicPuzzleSide.E,0,new[]{2,0},new[]{1,1})).ToArray();Public=new LocalSaveContractFixtures.PublicCatalog(Base.PublicId,items);var campaign=new LocalSaveContractFixtures.CampaignCatalog(Base.CampaignId,items.Select((p,i)=>new LockedCampaignEntry(Base.CampaignId,p.Identity,"S1","S1-01","S1-01-01",i+1,Array.Empty<string>())).ToArray());var completion=new LocalSaveContractFixtures.CompletionCatalog(Base.CompletionId,items.Select(p=>new CompletionValues(Base.CompletionId,p.Identity,15,5,10,60,150,null,null)).ToArray());Policy=new LocalProgressPolicy(Public,campaign,completion,LocalSaveContractFixtures.Json,Base.Ledger);var p=Policy.NewProfile(Base.Profile,Base.Catalogs,"2026-10-19T00:00:00Z");var drafts=items.Take(known).Select((v,i)=>Draft(v.Identity,future?"2027-01-01T00:00:00Z":equalInstants&&i%2==0?"2026-10-19T00:00:00.000Z":"2026-10-19T00:00:00Z")).Concat(Enumerable.Range(0,orphans).Select(i=>Draft(new PublicPuzzleIdentity(collision&&i==0?items[20].Identity.PuzzleId:"orphan-"+i.ToString("D2"),"STP-PUZZLE-SEMANTIC-JCS-1",new string('b',64)),"2026-10-19T00:00:00Z"))).ToArray();State=p.WithProgress(p.Levels,p.LedgerCheckpoint,p.EconomyJournal,p.SeasonProgressCache,drafts,p.RewardClaims);Current=Draft(items[20].Identity,future?"2025-01-01T00:00:00Z":"2026-10-19T00:00:00Z");
            }
            private static LocalDraft Draft(PublicPuzzleIdentity identity,string utc)
            {
                var input=new PublicPuzzleInput(identity,"train-track-v1",2,2,PublicPuzzleSide.W,0,PublicPuzzleSide.E,0,new[]{2,0},new[]{1,1});var assembly=Assembly.Load("STP.Puzzle.Domain");var definition=LocalSaveContractFixtures.Call(input,"ToDomainDefinition")!;var result=assembly.GetType("STP.Puzzle.Domain.PuzzleSessionState",true)!.GetMethod("Create")!.Invoke(null,new object?[]{definition,identity.PuzzleId,identity.HashProfile,identity.HashSha256,Guid.NewGuid(),LocalSaveContractFixtures.DomainEnum("SessionMode","FIRST_RUN"),0,null,null})!;var state=LocalSaveContractFixtures.Get(result,"Value")!;var checkpoint=assembly.GetType("STP.Puzzle.Domain.PuzzleSessionCheckpoint",true)!.GetMethod("Export")!.Invoke(null,new[]{state,(object)0L})!;return(LocalDraft)typeof(LocalDraft).GetMethod("Create")!.Invoke(null,new[]{(object)identity,utc,checkpoint})!;
            }
        }
        [TestCase(false,false)][TestCase(true,false)][TestCase(false,true)]
        public void TwentyFirstKnownDraftAlwaysPersistsCurrentDespiteClockRollbackAndEqualInstantSpellings(bool future,bool equal)
        {
            var f=new DraftFixture(20,0,future,equal);var result=f.Policy.WithDraft(f.State,f.Current);Assert.That(result.Drafts.Count,Is.EqualTo(20));Assert.That(result.Drafts.Single(d=>d.Identity.Equals(f.Current.Identity)),Is.SameAs(f.Current));Assert.That(result.Drafts.Any(d=>d.Identity.PuzzleId=="known-19"),Is.False,"Same UTC instant uses the full ordinal identity, regardless of fractional spelling");Assert.That(result.Drafts.Any(d=>d.Identity.PuzzleId=="known-00"),Is.True);Assert.That(f.State.Drafts.Count,Is.EqualTo(20));Assert.That(f.State.Drafts.Any(d=>d.Identity.Equals(f.Current.Identity)),Is.False);
        }
        [Test]
        public void NineteenOrphansRemainIdenticalAndDisplaceOnlyKnownOlderDraft()
        {var f=new DraftFixture(1,19);var result=f.Policy.WithDraft(f.State,f.Current);Assert.That(result.Drafts.Count,Is.EqualTo(20));foreach(var orphan in f.State.Drafts.Where(d=>d.Identity.PuzzleId.StartsWith("orphan-",StringComparison.Ordinal)))Assert.That(result.Drafts.Single(d=>d.Identity.Equals(orphan.Identity)),Is.SameAs(orphan));Assert.That(result.Drafts.Any(d=>d.Identity.PuzzleId=="known-00"),Is.False);Assert.That(result.Drafts.Single(d=>d.Identity.Equals(f.Current.Identity)),Is.SameAs(f.Current));}
        [Test]
        public void TwentyOrphansAndChangedIdentityStopBeforeAnyRetentionLoss()
        {var full=new DraftFixture(0,20);var e=Assert.Throws<ArgumentException>(()=>full.Policy.WithDraft(full.State,full.Current));Assert.That(e!.Message,Is.EqualTo("SAVE_DRAFT_CAPACITY"));Assert.That(full.State.Drafts.Count,Is.EqualTo(20));var collision=new DraftFixture(19,1,collision:true);e=Assert.Throws<ArgumentException>(()=>collision.Policy.WithDraft(collision.State,collision.Current));Assert.That(e!.Message,Is.EqualTo("SAVE_DRAFT_IDENTITY_COLLISION"));Assert.That(collision.State.Drafts.Count,Is.EqualTo(20));}
        [Test]
        public async Task FailedTwentyFirstDraftWriteKeepsDurableTwentyAndCurrentUnsavedAttempt()
        {
            var f=new DraftFixture(20,0,true);var observation=new SaveRepositoryObservation(f.Base.Profile,f.Base.Aggregate,new[]{new SaveCandidateProvenance(SaveCandidateKind.Main,SaveCandidateStatus.Valid,1,new string('a',64),new string('b',64)),new SaveCandidateProvenance(SaveCandidateKind.Backup,SaveCandidateStatus.Missing,null,null,null),new SaveCandidateProvenance(SaveCandidateKind.Pending,SaveCandidateStatus.Missing,null,null,null)});var snapshot=new SaveSnapshot<LocalProfileState>(f.Base.Profile,f.Base.Aggregate,1,f.State);var repo=new LocalSaveContractFixtures.Repository{Loaded=SaveLoadResult<LocalProfileState>.Success(snapshot,observation),OnCommit=(s,g,ct)=>Task.FromResult(SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed,"SAVE_TEST_FLUSH"))};var clock=new LocalSaveContractFixtures.Clock{UtcNow="2025-01-01T00:00:00Z"};var use=new PersistedPuzzleSession(repo,f.Public,f.Policy,f.Base.Profile,f.Base.Catalogs,clock,new LocalSaveContractFixtures.Dispatcher());await use.LoadAsync(CancellationToken.None);LocalSaveContractFixtures.Call(use,"Start",f.Current.Identity,Guid.NewGuid());LocalSaveContractFixtures.Mark(use,clock,100,"MARK_EMPTY",0);var current=LocalSaveContractFixtures.Get(use,"Session");Assert.That((await use.SaveDraftAsync(CancellationToken.None)).Succeeded,Is.False);Assert.That(use.Snapshot,Is.SameAs(snapshot));Assert.That(use.Snapshot!.Payload.Drafts.Count,Is.EqualTo(20));Assert.That(LocalSaveContractFixtures.Get(use,"Session"),Is.SameAs(current));Assert.That(repo.LastOffered!.Payload.Drafts.Any(d=>d.Identity.Equals(f.Current.Identity)),Is.True);Assert.That(repo.LastOffered.Payload.Drafts.Count,Is.EqualTo(20));
        }
        [TestCase(511,false)][TestCase(512,false)][TestCase(383,true)]
        public void LegalFullSuffixCompactsBeforeFirstClearRouteAndSectionGrants(int count,bool bytes)
        {
            var f=LocalSaveContractFixtures.CreateLedgerBoundary(count,bytes);var policy=(LocalProgressPolicy)f[0];var source=(LocalProfileState)f[1];var ledger=(ILocalLedger)f[3];int originalBytes=((byte[])LocalSaveContractFixtures.Call(ledger,"JournalBytes",source.EconomyJournal)!).Length;if(bytes)Assert.That(originalBytes,Is.EqualTo(262140));Assert.That(ledger.Validate(source),Is.EqualTo(count*15));var result=(LocalCompletionProjection)LocalSaveContractFixtures.Call(policy,"Complete",source,f[2],2L,"2026-10-19T00:00:00Z")!;Assert.That(result.RewardPatience,Is.EqualTo(225));Assert.That(result.State.LedgerCheckpoint.ThroughSequence,Is.EqualTo(count));Assert.That(result.State.EconomyJournal.Count,Is.EqualTo(3));Assert.That(result.State.RewardClaims.OfType<CampaignProgressTerminal>().Count(),Is.EqualTo(2));Assert.That(ledger.Validate(result.State),Is.EqualTo(count*15+225));Assert.That(result.State.LedgerCheckpoint.JournalDiagnosticTail.Count,Is.EqualTo(64));Assert.That(source.EconomyJournal.Count,Is.EqualTo(count));Assert.That(source.LedgerCheckpoint.ThroughSequence,Is.Zero);Assert.That(result.State.EconomyJournal.All(e=>result.State.Levels.Any(l=>l.FirstClear?.Terminal.TransactionId==e.TransactionId)||result.State.RewardClaims.OfType<CampaignProgressTerminal>().Any(c=>c.Terminal.TransactionId==e.TransactionId)),Is.True);
        }
        [Test]
        public void NullTimesAndBothMarkersNeverInventStarsOrGateTheNextFirstClear()
        {
            var s=new LocalSaveContractFixtures.Setup();var p=s.Empty;Assert.That(s.Policy.CanEnter(p,s.Puzzles.Puzzles[0].Identity),Is.True);Assert.That(s.Policy.CanEnter(p,s.Puzzles.Puzzles[1].Identity),Is.False);var first=s.Complete(p,s.Solved(0),2);Assert.That(first.RewardPatience,Is.EqualTo(15));Assert.That(first.State.Levels.Single().StarsHighWater,Is.EqualTo(1));Assert.That(first.State.Levels.Single().DirectSolved,Is.True);Assert.That(s.Policy.CanEnter(first.State,s.Puzzles.Puzzles[1].Identity),Is.True);Assert.That(s.Ledger.Validate(first.State),Is.EqualTo(15));Assert.That(LocalSaveContractFixtures.Call(s.Policy,"ModeFor",first.State,s.Puzzles.Puzzles[0].Identity)!.ToString(),Is.EqualTo("PRACTICE"));
        }
        [Test]
        public void FirstClearRouteSectionAndTerminalReplayAreOneCopyOnWriteTransaction()
        {
            var s=new LocalSaveContractFixtures.Setup();var empty=s.Empty;var attempt=s.Solved(0);var a=s.Complete(empty,attempt,2);var replay=s.Complete(a.State,attempt,3);Assert.That(replay.AlreadyCommitted,Is.True);Assert.That(replay.State,Is.SameAs(a.State));Assert.That(replay.RewardPatience,Is.Zero);var b=s.Complete(a.State,s.Solved(1,"FIRST_RUN",2),3);Assert.That(b.RewardPatience,Is.EqualTo(225));Assert.That(s.Ledger.Validate(b.State),Is.EqualTo(240));Assert.That(b.State.RewardClaims.OfType<CampaignProgressTerminal>().Count(),Is.EqualTo(2));Assert.That(empty.Levels,Is.Empty);Assert.That(empty.EconomyJournal,Is.Empty);Assert.That(LocalSaveContractFixtures.Call(s.Policy,"ModeFor",b.State,s.Puzzles.Puzzles[0].Identity)!.ToString(),Is.EqualTo("REVISION"));
        }
        [Test]
        public void PracticeCannotImproveOfficialFactsAndRevisionOnlyPaysEachMissingStarOnce()
        {
            var s=new LocalSaveContractFixtures.Setup(100,50);var a=s.Complete(s.Empty,s.Solved(0),2);var first=a.State.Levels.Single();Assert.That(first.StarsHighWater,Is.EqualTo(2));var practice=s.Complete(a.State,s.Solved(0,"PRACTICE",2,1),3);Assert.That(practice.RewardPatience,Is.Zero);Assert.That(practice.State.Levels.Single().FirstClear,Is.SameAs(first.FirstClear));Assert.That(practice.State.Levels.Single().RevisionBestElapsedMs,Is.Null);Assert.That(practice.State.Levels.Single().StarsHighWater,Is.EqualTo(2));var b=s.Complete(practice.State,s.Solved(1,"FIRST_RUN",3),4);var rev=s.Complete(b.State,s.Solved(0,"REVISION",4,1),5);var improved=rev.State.Levels.Single(l=>l.Identity.Equals(s.Puzzles.Puzzles[0].Identity));Assert.That(rev.RewardPatience,Is.EqualTo(10));Assert.That(improved.StarsHighWater,Is.EqualTo(3));Assert.That(improved.RevisionBestElapsedMs,Is.EqualTo(8));Assert.That(improved.FirstRunBestElapsedMs,Is.EqualTo(first.FirstRunBestElapsedMs));var repeat=s.Complete(rev.State,s.Solved(0,"REVISION",5,2),6);Assert.That(repeat.RewardPatience,Is.Zero);Assert.That(repeat.State.Levels.Single(l=>l.Identity.Equals(improved.Identity)).StarsHighWater,Is.EqualTo(3));Assert.That(repeat.State.Levels.Single(l=>l.Identity.Equals(improved.Identity)).RevisionBestElapsedMs,Is.EqualTo(8));
        }
        [Test]
        public void SameIdChangedSemanticHashAndUnprovenRevisionAreRejected()
        {
            var s=new LocalSaveContractFixtures.Setup();var bad=LocalLevelProgress.Create(new PublicPuzzleIdentity(s.Puzzles.Puzzles[0].Identity.PuzzleId,"STP-PUZZLE-SEMANTIC-JCS-1",new string('b',64)),null,0,null,null,false,null,null);Assert.Throws<ArgumentException>(()=>s.Policy.Cache(new[]{bad}));Assert.Throws<TargetInvocationException>(()=>s.Complete(s.Empty,s.Solved(0,"REVISION"),2));
        }
    }
}
