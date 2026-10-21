using System;
using System.Collections.Generic;
using System.Linq;
using STP.Application;

namespace STP.Infrastructure.Content
{
    /// <summary>Separate actual-DRAFT candidate graph with strict own source locks; no historical QA or fixture promotion.</summary>
    public sealed class MobileCandidateProgressCatalog
    {
        private sealed class CampaignView : ILockedCampaignCatalog
        {
            internal CampaignView(CatalogIdentity identity,LockedCampaignEntry[] entries){Identity=identity;Entries=System.Array.AsReadOnly(entries);}
            public CatalogIdentity Identity {get;}
            public IReadOnlyList<LockedCampaignEntry> Entries {get;}
            public bool TryGet(PublicPuzzleIdentity identity,out LockedCampaignEntry? entry){entry=Entries.SingleOrDefault(e=>e.Puzzle.Equals(identity));return entry!=null;}
        }
        private sealed class CompletionView : ILockedCompletionCatalog
        {
            private readonly IReadOnlyList<CompletionValues> values;
            internal CompletionView(CatalogIdentity identity,CompletionValues[] entries){Identity=identity;values=System.Array.AsReadOnly(entries);}
            public CatalogIdentity Identity {get;}
            public bool TryGet(PublicPuzzleIdentity identity,out CompletionValues? value){value=values.SingleOrDefault(e=>e.Puzzle.Equals(identity));return value!=null;}
        }
        /// <summary>Strictly validates both complete sources and exact injected 240-puzzle identities; completion hash covers the entire wrapper.</summary>
        public MobileCandidateProgressCatalog(IPublicPuzzleCatalog puzzles,byte[] campaignUtf8,byte[] completionUtf8,ICanonicalJson canonicalJson,string expectedCampaignHashSha256,string expectedCompletionHashSha256)
        {
            if(puzzles==null||canonicalJson==null)throw new ArgumentNullException();Require(puzzles.Identity.CatalogId==MobileCandidateData.CatalogId&&puzzles.Identity.Use==CatalogUse.InternalPreview&&puzzles.Identity.ApprovalStatus==CatalogApprovalStatus.Draft,"CANDIDATE_CATALOG_USE_INVALID");
            var cb=Canonical(canonicalJson,campaignUtf8);var vb=Canonical(canonicalJson,completionUtf8);Require(LocalSaveHash.Bytes(cb)==expectedCampaignHashSha256&&LocalSaveHash.Bytes(vb)==expectedCompletionHashSha256,"CANDIDATE_CATALOG_LOCK_MISMATCH");var c=Parse(cb);var wrapper=Parse(vb);
            Fields(c,"schemaVersion","catalogId","catalogRevision","approvalStatus","seasons");Integer(c,"schemaVersion",2);Text(c,"catalogId","campaign-qa-mobile-candidate");Text(c,"approvalStatus","DRAFT");Integer(c,"catalogRevision",1);
            Fields(wrapper,"reviewCompletionContractVersion","completionCatalog","starTimes");Integer(wrapper,"reviewCompletionContractVersion",1);var v=Need(wrapper,"completionCatalog");Fields(v,"schemaVersion","catalogId","catalogRevision","approvalStatus","rewardDefinitions","completionMoments");Integer(v,"schemaVersion",1);Text(v,"catalogId","completion-qa-mobile-candidate");Integer(v,"catalogRevision",1);Text(v,"approvalStatus","DRAFT");
            var campaignId=new CatalogIdentity("campaign-qa-mobile-candidate",1,LocalSaveHash.Bytes(cb),CatalogUse.InternalPreview,CatalogApprovalStatus.Draft);var completionId=new CatalogIdentity("completion-qa-mobile-candidate",1,LocalSaveHash.Bytes(vb),CatalogUse.InternalPreview,CatalogApprovalStatus.Draft);
            var publicItems=puzzles.Puzzles.ToArray();Require(publicItems.Length==240&&publicItems.Select(p=>p.Identity.PuzzleId).Distinct(StringComparer.Ordinal).Count()==240,"CANDIDATE_PUBLIC_MEMBERSHIP_INVALID");var entries=new List<LockedCampaignEntry>();var seasons=Array(Need(c,"seasons"));Require(seasons.Count==1,"CANDIDATE_MEMBERSHIP_INVALID");var season=seasons[0];Fields(season,"id","order","titleKey","unlock","sections");Text(season,"id","S1");Integer(season,"order",1);Key(season,"titleKey");Always(Need(season,"unlock"));var sections=Array(Need(season,"sections"));Require(sections.Count==5,"CANDIDATE_MEMBERSHIP_INVALID");
            for(int si=0;si<5;si++){var section=sections[si];string sid="S1-"+(si+1).ToString("D2");Fields(section,"id","order","titleKey","unlock","routes");Text(section,"id",sid);Integer(section,"order",si+1);Key(section,"titleKey");Always(Need(section,"unlock"));var routes=Array(Need(section,"routes"));Require(routes.Count==4,"CANDIDATE_MEMBERSHIP_INVALID");for(int ri=0;ri<4;ri++){var route=routes[ri];string rid=sid+"-"+(ri+1).ToString("D2");Fields(route,"id","order","titleKey","unlock","levels");Text(route,"id",rid);Integer(route,"order",ri+1);Key(route,"titleKey");Always(Need(route,"unlock"));var levels=Array(Need(route,"levels"));Require(levels.Count==12,"CANDIDATE_MEMBERSHIP_INVALID");for(int li=0;li<12;li++){var level=levels[li];string id=rid+"-"+(li+1).ToString("D2");Fields(level,"puzzleId","order","unlock");Text(level,"puzzleId",id);Integer(level,"order",li+1);Linear(Need(level,"unlock"),entries.Count==0?null:entries[entries.Count-1].Puzzle.PuzzleId);var item=publicItems.SingleOrDefault(p=>p.Identity.PuzzleId==id);Require(item!=null&&puzzles.TryGet(item.Identity,out var exact)&&exact!=null&&exact.Identity.Equals(item.Identity),"CANDIDATE_PUBLIC_MEMBERSHIP_INVALID");entries.Add(new LockedCampaignEntry(campaignId,item!.Identity,"S1",sid,rid,li+1,entries.Count==0?System.Array.Empty<string>():new[]{entries[entries.Count-1].Puzzle.PuzzleId}));}}}
            var rewards=Array(Need(v,"rewardDefinitions"));Require(rewards.Count==5||rewards.Count==6,"CANDIDATE_COMPLETION_INVALID");string[] reasons={"LEVEL_FIRST_CLEAR","LEVEL_SECOND_STAR","LEVEL_THIRD_STAR","ROUTE_CLEAR","SECTION_CLEAR"};string[] kinds={"LEVEL","LEVEL","LEVEL","ROUTE","SECTION"};int[] amounts={15,5,10,60,150};var actualAmounts=new int[5];for(int i=0;i<5;i++){var reward=rewards.SingleOrDefault(r=>Need(r,"reasonCode").Kind==JsonKind.String&&Need(r,"reasonCode").StringValue==reasons[i]);Require(reward!=null,"CANDIDATE_COMPLETION_INVALID");Fields(reward!,"reasonCode","subjectKind","amountPatience","claimCardinality");Text(reward!,"subjectKind",kinds[i]);Text(reward!,"claimCardinality","ONCE_PER_SUBJECT");Integer(reward!,"amountPatience",amounts[i]);actualAmounts[i]=(int)Need(reward!,"amountPatience").IntegerValue;}
            if(rewards.Count==6){var extra=rewards.SingleOrDefault(r=>Need(r,"reasonCode").Kind==JsonKind.String&&Need(r,"reasonCode").StringValue=="POST_CLEAR_PATIENCE");Require(extra!=null,"CANDIDATE_COMPLETION_INVALID");Fields(extra!,"reasonCode","subjectKind","amountPatience","claimCardinality");Text(extra!,"subjectKind","LEVEL");Text(extra!,"claimCardinality","ONCE_PER_SUBJECT");Integer(extra!,"amountPatience",10);}var moments=Array(Need(v,"completionMoments"));Require(moments.Count==240,"CANDIDATE_COMPLETION_INVALID");var momentIds=new HashSet<string>(StringComparer.Ordinal);foreach(var moment in moments){Fields(moment,"id","resultTextKey","mapSegmentId","trainMomentId");foreach(string key in new[]{"id","mapSegmentId","trainMomentId"})Stable(moment,key);Key(moment,"resultTextKey");Require(momentIds.Add(Need(moment,"id").StringValue),"CANDIDATE_DUPLICATE_ID");}
            Require(moments.Select(m=>Need(m,"mapSegmentId").StringValue).Distinct(StringComparer.Ordinal).Count()==240&&moments.Select(m=>Need(m,"trainMomentId").StringValue).Distinct(StringComparer.Ordinal).Count()==240,"CANDIDATE_MOMENT_REFERENCES_DUPLICATE");
            var times=Array(Need(wrapper,"starTimes"));Require(times.Count==240,"CANDIDATE_MEMBERSHIP_INVALID");var values=new CompletionValues[240];for(int i=0;i<240;i++){var time=times[i];Fields(time,"puzzleId","twoStarMs","threeStarMs");Text(time,"puzzleId",entries[i].Puzzle.PuzzleId);values[i]=new CompletionValues(completionId,entries[i].Puzzle,actualAmounts[0],actualAmounts[1],actualAmounts[2],actualAmounts[3],actualAmounts[4],OptionalTime(Need(time,"twoStarMs")),OptionalTime(Need(time,"threeStarMs")));}
            Campaign=new CampaignView(campaignId,entries.ToArray());Completion=new CompletionView(completionId,values);
            string projection="{\"publicCatalog\":"+LocalSaveHash.CatalogProjection(puzzles.Identity)+",\"campaignCatalog\":"+LocalSaveHash.CatalogProjection(campaignId)+",\"completionCatalog\":"+LocalSaveHash.CatalogProjection(completionId)+"}";var aggregate=new CatalogIdentity("qa-mobile-candidate-catalog-set",1,LocalSaveHash.Bytes(LocalSaveHash.Canonical(canonicalJson,projection)),CatalogUse.InternalPreview,CatalogApprovalStatus.Draft);CatalogSet=CatalogSetIdentity.Create(puzzles.Identity,campaignId,completionId,aggregate,canonicalJson);
        }
        /// <summary>Immutable linearly gated internal candidate membership.</summary>
        public ILockedCampaignCatalog Campaign {get;}
        /// <summary>Immutable identity-bound values; uncalibrated source thresholds remain explicitly null.</summary>
        public ILockedCompletionCatalog Completion {get;}
        /// <summary>Combined profile/envelope binding covering both complete review sources and injected public catalog.</summary>
        public CatalogSetIdentity CatalogSet {get;}
        private static byte[] Canonical(ICanonicalJson j,byte[] source){var r=j.Canonicalize(source,LocalSaveHash.Limits);Require(r.Succeeded,r.ErrorCode??"CANDIDATE_JSON_INVALID");return r.GetUtf8()!;}
        private static JsonValue Parse(byte[] b){var r=StrictJsonParser.Parse(b,new JsonParserLimits(67108864,64,65536,1048576,256));Require(r.Value!=null,r.Error?.Code??"CANDIDATE_JSON_INVALID");return r.Value!;}
        private static void Require(bool condition,string code){if(!condition)throw new ArgumentException(code);}
        private static JsonValue Need(JsonValue v,string name)=>v.Get(name)??throw new ArgumentException("CANDIDATE_FIELDS_INVALID");
        private static IReadOnlyList<JsonValue> Array(JsonValue v){Require(v.Kind==JsonKind.Array,"CANDIDATE_TYPE_INVALID");return v.Items;}
        private static void Fields(JsonValue v,params string[] fields){Require(v.Kind==JsonKind.Object&&v.Members.Count==fields.Length&&v.Members.All(m=>fields.Contains(m.Name)),"CANDIDATE_FIELDS_INVALID");}
        private static void Text(JsonValue v,string key,string expected){var n=Need(v,key);Require(n.Kind==JsonKind.String&&n.StringValue==expected,"CANDIDATE_VALUE_INVALID");}
        private static void Integer(JsonValue v,string key,long expected){var n=Need(v,key);Require(n.Kind==JsonKind.Integer&&n.IntegerValue==expected,"CANDIDATE_VALUE_INVALID");}
        private static void Key(JsonValue v,string key){var n=Need(v,key);Require(n.Kind==JsonKind.String&&System.Text.RegularExpressions.Regex.IsMatch(n.StringValue,@"^[a-z][a-z0-9-]*(\.[a-z0-9-]+)+$"),"CANDIDATE_KEY_INVALID");}
        private static void Stable(JsonValue v,string key){var n=Need(v,key);Require(n.Kind==JsonKind.String&&System.Text.RegularExpressions.Regex.IsMatch(n.StringValue,@"^[a-z][a-z0-9]*(?:-[a-z0-9]+)*$"),"CANDIDATE_ID_INVALID");}
        private static void Always(JsonValue v){Fields(v,"kind","requiredSubjectIds");Text(v,"kind","ALWAYS");Require(Array(Need(v,"requiredSubjectIds")).Count==0,"CANDIDATE_TECHNICAL_ACCESS_INVALID");}
        private static void Linear(JsonValue v,string? previous)
        {
            if(previous==null){Always(v);return;}Fields(v,"kind","requiredSubjectIds");Text(v,"kind","ALL_FIRST_CLEARS");var required=Array(Need(v,"requiredSubjectIds"));Require(required.Count==1&&required[0].Kind==JsonKind.String&&required[0].StringValue==previous,"CANDIDATE_LINEAR_UNLOCK_INVALID");
        }
        private static long? OptionalTime(JsonValue v)=>v.Kind==JsonKind.Null?(long?)null:v.Kind==JsonKind.Integer&&v.IntegerValue>0?v.IntegerValue:throw new ArgumentException("CANDIDATE_STAR_TIME_INVALID");
    }
}

