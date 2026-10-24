using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using STP.Puzzle.Domain;

namespace STP.Application
{
    /// <summary>Pure ledger operations supplied by the persistence ledger codec; no file effect is permitted.</summary>
    public interface ILocalLedger
    {
        /// <summary>Verifies checkpoint, suffix, terminal deduplication and returns the authoritative nonnegative balance.</summary>
        long Validate(LocalProfileState state);
        /// <summary>Builds the next exact chained entry from one already validated terminal.</summary>
        LocalStoredEntry Next(LocalLedgerCheckpoint checkpoint,IReadOnlyList<LocalStoredEntry> journal,EconomyTerminal terminal);
        /// <summary>Builds the existing COSMETIC_PURCHASE debit with null claim-ID from its simultaneous ownership record.</summary>
        LocalStoredEntry NextPurchase(LocalLedgerCheckpoint checkpoint,IReadOnlyList<LocalStoredEntry> journal,PurchasedOwnership ownership);
        /// <summary>Compacts only terminal-backed entries and preserves all authoritative deduplication truth.</summary>
        LocalProfileState Compact(LocalProfileState state,long sourceSaveGeneration);
    }

    /// <summary>Immutable result of a pure completion projection; it has no committed success or presentation authority.</summary>
    public sealed class LocalCompletionProjection
    {
        internal LocalCompletionProjection(LocalProfileState state,bool alreadyCommitted,int reward){State=state;AlreadyCommitted=alreadyCommitted;RewardPatience=reward;}
        /// <summary>New copy-on-write payload, or the unchanged payload on exact terminal replay.</summary>
        public LocalProfileState State {get;}
        /// <summary>Whether this exact attempt already owns the durable first-clear terminal.</summary>
        public bool AlreadyCommitted {get;}
        /// <summary>Projected ledger delta; visible reward success still requires repository acknowledgement.</summary>
        public int RewardPatience {get;}
    }

    /// <summary>Base campaign progression using only immutable locked public, membership and completion catalogs.</summary>
    public sealed class LocalProgressPolicy
    {
        private readonly IPublicPuzzleCatalog puzzles;private readonly ILockedCampaignCatalog campaign;private readonly ILockedCompletionCatalog completion;private readonly ICanonicalJson json;private readonly ILocalLedger ledger;
        /// <summary>Validates a complete immutable campaign graph and the three catalog uses.</summary>
        public LocalProgressPolicy(IPublicPuzzleCatalog puzzles,ILockedCampaignCatalog campaign,ILockedCompletionCatalog completion,ICanonicalJson json,ILocalLedger ledger)
        {
            this.puzzles=puzzles??throw new ArgumentNullException(nameof(puzzles));this.campaign=campaign??throw new ArgumentNullException(nameof(campaign));this.completion=completion??throw new ArgumentNullException(nameof(completion));this.json=json??throw new ArgumentNullException(nameof(json));this.ledger=ledger??throw new ArgumentNullException(nameof(ledger));
            LocalModel.Require(puzzles.Identity.Use==campaign.Identity.Use&&puzzles.Identity.Use==completion.Identity.Use,"SAVE_CATALOG_USE_MISMATCH");
            LocalModel.Unique(campaign.Entries.Select(x=>x.Puzzle.PuzzleId));LocalModel.Unique(campaign.Entries.Select(x=>x.RouteId+":"+x.Ordinal.ToString(CultureInfo.InvariantCulture)));
            foreach(var e in campaign.Entries){LocalModel.Require(e.Catalog.Equals(campaign.Identity)&&puzzles.TryGet(e.Puzzle,out _)&&completion.TryGet(e.Puzzle,out var v)&&v!=null&&v.Catalog.Equals(completion.Identity),"SAVE_CAMPAIGN_BINDING_INVALID");foreach(string id in e.RequiredFirstClears)LocalModel.Require(campaign.Entries.Any(x=>x.Puzzle.PuzzleId==id),"SAVE_CAMPAIGN_REQUIREMENT_MISSING");Visit(e.Puzzle.PuzzleId,new HashSet<string>(StringComparer.Ordinal),new HashSet<string>(StringComparer.Ordinal));}
            LocalModel.Require(campaign.Entries.Count>0,"SAVE_CAMPAIGN_EMPTY");
        }
        private void Visit(string id,HashSet<string> path,HashSet<string> done){if(done.Contains(id))return;LocalModel.Require(path.Add(id),"SAVE_CAMPAIGN_CYCLE");foreach(string required in campaign.Entries.Single(e=>e.Puzzle.PuzzleId==id).RequiredFirstClears)Visit(required,path,done);path.Remove(id);done.Add(id);}
        private void Binding(LocalProfileState state){LocalModel.Require(state.Profile.CatalogSet.PublicCatalog.Equals(puzzles.Identity)&&state.Profile.CatalogSet.CampaignCatalog.Equals(campaign.Identity)&&state.Profile.CatalogSet.CompletionCatalog.Equals(completion.Identity),"SAVE_CATALOG_BINDING_MISMATCH");}
        private static bool Cleared(LocalProfileState state,PublicPuzzleIdentity id)=>state.Levels.Any(l=>l.Identity.Equals(id)&&l.FirstClear!=null);
        /// <summary>Determines access exclusively from required first clears; stars never gate access.</summary>
        public bool CanEnter(LocalProfileState state,PublicPuzzleIdentity identity){Binding(state);return campaign.TryGet(identity,out var e)&&e!=null&&e.RequiredFirstClears.All(id=>campaign.Entries.Any(m=>m.Puzzle.PuzzleId==id&&Cleared(state,m.Puzzle)));}
        /// <summary>Selects first run, practice before full-season completion, or revision after full-season completion.</summary>
        public SessionMode ModeFor(LocalProfileState state,PublicPuzzleIdentity identity){Binding(state);LocalModel.Require(CanEnter(state,identity),"SAVE_LEVEL_LOCKED");if(!Cleared(state,identity))return SessionMode.FIRST_RUN;var e=campaign.Entries.Single(m=>m.Puzzle.Equals(identity));return campaign.Entries.Where(m=>m.SeasonId==e.SeasonId).All(m=>Cleared(state,m.Puzzle))?SessionMode.REVISION:SessionMode.PRACTICE;}
        /// <summary>Creates an explicit empty profile payload. Calling it alone never writes or authorizes repository initialization.</summary>
        public LocalProfileState NewProfile(SaveProfileIdentity profile,CatalogSetIdentity catalogs,string firstUseUtc)
        {
            var header=LocalProfileHeader.Create(profile.ProfileId,firstUseUtc,profile,catalogs);var empty=Array.Empty<LocalLevelProgress>();
            var cache=Cache(empty);return LocalProfileState.Create(header,empty,LocalEndlessState.Create("0",Array.Empty<LocalOpenEndless>()),LocalLedgerCheckpoint.Create(0,0,LocalSaveHash.Bytes(Array.Empty<byte>()),0,Array.Empty<LocalStoredEntry>()),Array.Empty<LocalStoredEntry>(),LocalInventory.Create(Array.Empty<LocalOwnership>(),Array.Empty<string>()),Array.Empty<LocalCosmeticReservation>(),cache,Array.Empty<LocalDraft>(),Array.Empty<LocalRewardClaim>(),null,LocalEntitlements.Create(Array.Empty<LocalEntitlement>()),Array.Empty<LocalPendingOperation>(),LocalSettings.Create(null,Array.Empty<LocalAudioPreference>(),null,null),LocalDataLifecycle.Create(null,null),null);
        }
        /// <summary>Computes a fresh cache from exact current membership and validated identity-bound first clears.</summary>
        public LocalSeasonProgressCache Cache(IReadOnlyList<LocalLevelProgress> levels)
        {
            var entriesById=campaign.Entries.ToLookup(e=>e.Puzzle.PuzzleId,StringComparer.Ordinal);
            foreach(var l in levels){var same=entriesById[l.Identity.PuzzleId].FirstOrDefault();LocalModel.Require(same==null||same.Puzzle.Equals(l.Identity),"LVL-PUBLISHED-PUZZLE-MUTATED");}
            var catalogIdentities=new HashSet<PublicPuzzleIdentity>(campaign.Entries.Select(e=>e.Puzzle));
            var first=levels.Where(l=>l.FirstClear!=null&&catalogIdentities.Contains(l.Identity)).Select(l=>l.Identity).OrderBy(i=>i.PuzzleId,StringComparer.Ordinal).ToArray();var firstMembership=new HashSet<PublicPuzzleIdentity>(first);var subjects=new List<LocalSubjectProgress>();
            foreach(string kind in new[]{"SEASON","SECTION","ROUTE"})foreach(var group in campaign.Entries.GroupBy(e=>kind=="SEASON"?e.SeasonId:kind=="SECTION"?e.SectionId:e.RouteId).OrderBy(g=>g.Key,StringComparer.Ordinal)){var ids=group.Where(e=>firstMembership.Contains(e.Puzzle)).Select(e=>e.Puzzle.PuzzleId).OrderBy(id=>id,StringComparer.Ordinal).ToArray();subjects.Add(LocalSubjectProgress.Create(group.Key,kind,ids,ids.Length,group.Count(),ids.Length==group.Count()));}
            string projection="{\"campaignCatalog\":"+LocalSaveHash.CatalogProjection(campaign.Identity)+",\"firstClearIdentities\":["+string.Join(",",first.Select(i=>"{\"puzzleId\":"+LocalSaveHash.Quote(i.PuzzleId)+",\"profile\":"+LocalSaveHash.Quote(i.HashProfile)+",\"sha256\":"+LocalSaveHash.Quote(i.HashSha256)+"}"))+"]}";
            return LocalSeasonProgressCache.Create(campaign.Identity,LocalSaveHash.Bytes(LocalSaveHash.Canonical(json,projection)),subjects);
        }
        /// <summary>Rejects a stale or forged cache and verifies the complete authoritative ledger.</summary>
        public void Validate(LocalProfileState state)
        {
            Binding(state);var c=Cache(state.Levels);var actual=state.SeasonProgressCache;LocalModel.Require(c.CampaignCatalog.Equals(actual.CampaignCatalog)&&c.SourceProjectionHashSha256==actual.SourceProjectionHashSha256&&c.Subjects.Count==actual.Subjects.Count,"SAVE_CACHE_INVALID");
            for(int i=0;i<c.Subjects.Count;i++){var a=c.Subjects[i];var b=actual.Subjects[i];LocalModel.Require(a.SubjectId==b.SubjectId&&a.Kind==b.Kind&&a.FirstClearCount==b.FirstClearCount&&a.MemberCount==b.MemberCount&&a.Completed==b.Completed&&a.FirstClearPuzzleIds.SequenceEqual(b.FirstClearPuzzleIds),"SAVE_CACHE_INVALID");}
            foreach(var level in state.Levels.Where(l=>l.FirstClear!=null))
            {
                if(!completion.TryGet(level.Identity,out var v)||v==null)continue;
                void Terminal(EconomyTerminal t,string reason,int amount){LocalModel.Require(t.SubjectId==level.Identity.PuzzleId&&t.ReasonCode==reason&&t.Amount==amount,"SAVE_TERMINAL_VALUE_MISMATCH");}
                if(level.FirstClear!.Terminal.Catalog.Equals(completion.Identity))Terminal(level.FirstClear.Terminal,"LEVEL_FIRST_CLEAR",v.FirstClear);
                if(level.SecondStarPaid!=null&&level.SecondStarPaid.Catalog.Equals(completion.Identity))Terminal(level.SecondStarPaid,"LEVEL_SECOND_STAR",v.SecondStar);
                if(level.ThirdStarPaid!=null&&level.ThirdStarPaid.Catalog.Equals(completion.Identity))Terminal(level.ThirdStarPaid,"LEVEL_THIRD_STAR",v.ThirdStar);
            }
            foreach(var subject in c.Subjects.Where(s=>s.Kind=="ROUTE"||s.Kind=="SECTION"))
            {
                var terminals=state.RewardClaims.OfType<CampaignProgressTerminal>().Where(t=>t.SubjectKind==subject.Kind&&t.SubjectId==subject.SubjectId).ToArray();
                LocalModel.Require(terminals.Length<=1&&(!subject.Completed||terminals.Length==1),"SAVE_SUBJECT_TERMINAL_MISSING");
                if(terminals.Length==1&&terminals[0].Terminal.Catalog.Equals(completion.Identity)){var terminal=terminals[0].Terminal;var member=campaign.Entries.First(e=>(subject.Kind=="ROUTE"?e.RouteId:e.SectionId)==subject.SubjectId);completion.TryGet(member.Puzzle,out var v);LocalModel.Require(subject.Completed&&terminal.ReasonCode==(subject.Kind=="ROUTE"?"ROUTE_CLEAR":"SECTION_CLEAR")&&terminal.Amount==(subject.Kind=="ROUTE"?v!.RouteClear:v!.SectionClear),"SAVE_SUBJECT_TERMINAL_INVALID");}
            }
            ledger.Validate(state);
        }
        /// <summary>Projects a real solved domain attempt atomically with its base rewards, terminal deduplication, cache and draft removal.</summary>
        public LocalCompletionProjection Complete(LocalProfileState state,PuzzleSessionState session,long targetGeneration,string completedAtUtc)
        {
            Validate(state);LocalModel.Positive(targetGeneration);LocalModel.Utc(completedAtUtc);
            var identity=new PublicPuzzleIdentity(session.PuzzleId,session.PublicPuzzleHashProfile,session.PublicPuzzleHashSha256);
            CompletionValues? values;bool hasValues=completion.TryGet(identity,out values);LocalModel.Require(puzzles.TryGet(identity,out var input)&&input!=null&&campaign.TryGet(identity,out var entry)&&entry!=null&&hasValues&&values!=null,"SAVE_COMPLETION_BINDING_MISSING");
            LocalModel.Require(SameDefinition(session.Definition,input!.ToDomainDefinition())&&session.Phase==SessionPhase.SolvedPendingCommit&&session.SolvedEventId=="puzzle-solved:"+session.AttemptId.ToString("D")&&PuzzleEvaluator.Evaluate(session.Definition,session.Cells).Completed,"SAVE_COMPLETION_NOT_SOLVED");
            var old=state.Levels.SingleOrDefault(l=>l.Identity.Equals(identity));if(old?.FirstClear?.AttemptId==session.AttemptId)return new LocalCompletionProjection(state,true,0);
            LocalModel.Require(CanEnter(state,identity)&&session.Mode==ModeFor(state,identity),"SAVE_COMPLETION_MODE_INVALID");
            state=ledger.Compact(state,targetGeneration);
            var levels=state.Levels.Where(l=>!l.Identity.Equals(identity)).ToList();var claims=state.RewardClaims.ToList();var journal=state.EconomyJournal.ToList();int reward=0;
            EconomyTerminal Grant(string transaction,string reason,string subject,int amount){var t=EconomyTerminal.Create(transaction,reason,subject,completion.Identity,"reward:"+reason+":"+subject,amount,targetGeneration,completedAtUtc);journal.Add(ledger.Next(state.LedgerCheckpoint,journal,t));reward=checked(reward+amount);return t;}
            if(session.Mode==SessionMode.FIRST_RUN){var v=values!;int stars=Stars(v,session.ActiveElapsedMs);var t=Grant("level:"+identity.PuzzleId+":first-clear","LEVEL_FIRST_CLEAR",identity.PuzzleId,v.FirstClear);var first=LocalFirstClear.Create(session.AttemptId,session.SolvedEventId!,completedAtUtc,session.ActiveElapsedMs,t);var second=stars>=2?Grant("level:"+identity.PuzzleId+":second-star","LEVEL_SECOND_STAR",identity.PuzzleId,v.SecondStar):null;var third=stars>=3?Grant("level:"+identity.PuzzleId+":third-star","LEVEL_THIRD_STAR",identity.PuzzleId,v.ThirdStar):null;levels.Add(LocalLevelProgress.Create(identity,first,stars,session.ActiveElapsedMs,null,!session.UsedEmptyMarker&&!session.UsedOccupiedMarker&&session.HintCount==0&&!(state.Metagame?.Hints.Any(h=>h.Identity.Equals(identity)&&(h.RegularConsumed+h.RewardedConsumed)>0)??false),second,third));}
            else if(session.Mode==SessionMode.REVISION){var previous=old!;var v=values!;long best=Math.Min(previous.RevisionBestElapsedMs??session.ActiveElapsedMs,session.ActiveElapsedMs);int stars=Math.Max(previous.StarsHighWater,Stars(v,best));var second=previous.SecondStarPaid??(stars>=2?Grant("level:"+identity.PuzzleId+":second-star","LEVEL_SECOND_STAR",identity.PuzzleId,v.SecondStar):null);var third=previous.ThirdStarPaid??(stars>=3?Grant("level:"+identity.PuzzleId+":third-star","LEVEL_THIRD_STAR",identity.PuzzleId,v.ThirdStar):null);levels.Add(LocalLevelProgress.Create(identity,previous.FirstClear,stars,previous.FirstRunBestElapsedMs,best,previous.DirectSolved,second,third));}
            else levels.Add(old!);
            levels=levels.OrderBy(l=>l.Identity.PuzzleId,StringComparer.Ordinal).ToList();var cache=Cache(levels);
            foreach(var s in cache.Subjects.Where(s=>s.Completed&&(s.Kind=="ROUTE"||s.Kind=="SECTION"))){if(claims.OfType<CampaignProgressTerminal>().Any(c=>c.SubjectKind==s.Kind&&c.SubjectId==s.SubjectId))continue;var members=campaign.Entries.Where(e=>(s.Kind=="ROUTE"?e.RouteId:e.SectionId)==s.SubjectId).ToArray();var vs=members.Select(e=>{LocalModel.Require(completion.TryGet(e.Puzzle,out var v)&&v!=null,"SAVE_COMPLETION_BINDING_MISSING");return v!;}).ToArray();int amount=s.Kind=="ROUTE"?vs[0].RouteClear:vs[0].SectionClear;LocalModel.Require(vs.All(v=>(s.Kind=="ROUTE"?v.RouteClear:v.SectionClear)==amount),"SAVE_SUBJECT_REWARD_CONFLICT");string reason=s.Kind=="ROUTE"?"ROUTE_CLEAR":"SECTION_CLEAR";var t=Grant(s.Kind.ToLowerInvariant()+":"+s.SubjectId+":clear",reason,s.SubjectId,amount);claims.Add(CampaignProgressTerminal.Create(s.Kind,s.SubjectId,t));}
            var result=state.WithProgress(levels,state.LedgerCheckpoint,journal,cache,state.Drafts.Where(d=>!d.Identity.Equals(identity)).ToArray(),claims);ledger.Validate(result);result=ledger.Compact(result,targetGeneration);return new LocalCompletionProjection(result,false,reward);
        }
        /// <summary>Performs deterministic terminal-backed compaction before a normal offered snapshot is constructed; recovery promotion bypasses this operation.</summary>
        public LocalProfileState PrepareForSave(LocalProfileState state,long targetGeneration)
        {Validate(state);var prepared=ledger.Compact(state,targetGeneration);if(!ReferenceEquals(prepared,state))Validate(prepared);return prepared;}
        /// <summary>Retains the offered exact draft and all orphaned drafts, then fills the remaining20 slots with the newest known drafts by UTC instant and ordinal identity. Capacity and incompatible same-ID records stop without mutation.</summary>
        public LocalProfileState WithDraft(LocalProfileState state,LocalDraft offered)
        {
            Binding(state);if(offered==null)throw new ArgumentNullException(nameof(offered));LocalModel.Require(puzzles.TryGet(offered.Identity,out var current)&&current!=null,"SAVE_DRAFT_IDENTITY_UNRESOLVED");LocalModel.Require(!state.Drafts.Any(d=>d.Identity.PuzzleId==offered.Identity.PuzzleId&&!d.Identity.Equals(offered.Identity)),"SAVE_DRAFT_IDENTITY_COLLISION");var previous=state.Drafts.Where(d=>!d.Identity.Equals(offered.Identity)).ToArray();var orphaned=previous.Where(d=>!puzzles.TryGet(d.Identity,out _)).ToArray();LocalModel.Require(orphaned.Length<20,"SAVE_DRAFT_CAPACITY");var known=previous.Where(d=>puzzles.TryGet(d.Identity,out _)).OrderByDescending(d=>DateTimeOffset.Parse(d.SavedAtUtc,CultureInfo.InvariantCulture)).ThenBy(d=>d.Identity.PuzzleId,StringComparer.Ordinal).ThenBy(d=>d.Identity.HashProfile,StringComparer.Ordinal).ThenBy(d=>d.Identity.HashSha256,StringComparer.Ordinal).Take(19-orphaned.Length);var retained=orphaned.Concat(new[]{offered}).Concat(known).OrderBy(d=>d.Identity.PuzzleId,StringComparer.Ordinal).ThenBy(d=>d.Identity.HashProfile,StringComparer.Ordinal).ThenBy(d=>d.Identity.HashSha256,StringComparer.Ordinal).ToArray();return state.WithProgress(state.Levels,state.LedgerCheckpoint,state.EconomyJournal,state.SeasonProgressCache,retained,state.RewardClaims);
        }
        private static int Stars(CompletionValues v,long elapsed)=>!v.TwoStarMs.HasValue?1:elapsed<=v.ThreeStarMs!.Value?3:elapsed<=v.TwoStarMs.Value?2:1;
        private static bool SameDefinition(PuzzleDefinition a,PuzzleDefinition b)=>a.RulesetVersion==b.RulesetVersion&&a.Grid.Width==b.Grid.Width&&a.Grid.Height==b.Grid.Height&&a.A.Side==b.A.Side&&a.A.Index==b.A.Index&&a.B.Side==b.B.Side&&a.B.Index==b.B.Index&&a.RowCounts.SequenceEqual(b.RowCounts)&&a.ColumnCounts.SequenceEqual(b.ColumnCounts);
    }
}
