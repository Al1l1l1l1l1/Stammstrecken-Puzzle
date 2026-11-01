using System;
using System.Collections.Generic;
using System.Linq;
using STP.Application;

namespace STP.Infrastructure.Content
{
    /// <summary>Isolated provider-free review graph and completion adapter; never consumes A-specific types or authoring fixtures.</summary>
    public sealed class ReviewProgressCatalog
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
        public ReviewProgressCatalog(IPublicPuzzleCatalog puzzles,byte[] campaignUtf8,byte[] completionUtf8,ICanonicalJson canonicalJson)
        {
            if(puzzles==null||canonicalJson==null)throw new ArgumentNullException();Require(puzzles.Identity.Use==CatalogUse.IsolatedReview&&puzzles.Identity.ApprovalStatus==CatalogApprovalStatus.Draft,"REVIEW_CATALOG_USE_INVALID");
            var cb=Canonical(canonicalJson,campaignUtf8);var vb=Canonical(canonicalJson,completionUtf8);var c=Parse(cb);var wrapper=Parse(vb);
            Fields(c,"schemaVersion","catalogId","catalogRevision","approvalStatus","seasons");Integer(c,"schemaVersion",2);Text(c,"catalogId","qa-public240-v1-campaign");Text(c,"approvalStatus","DRAFT");Integer(c,"catalogRevision",1);
            Fields(wrapper,"reviewCompletionContractVersion","completionCatalog","starTimes");Integer(wrapper,"reviewCompletionContractVersion",1);var v=Need(wrapper,"completionCatalog");Fields(v,"schemaVersion","catalogId","catalogRevision","approvalStatus","rewardDefinitions","completionMoments");Integer(v,"schemaVersion",1);Text(v,"catalogId","qa-public240-v1-completion");Integer(v,"catalogRevision",1);Text(v,"approvalStatus","DRAFT");
            var campaignId=new CatalogIdentity("qa-public240-v1-campaign",1,LocalSaveHash.Bytes(cb),CatalogUse.IsolatedReview,CatalogApprovalStatus.Draft);var completionId=new CatalogIdentity("qa-public240-v1-completion",1,LocalSaveHash.Bytes(vb),CatalogUse.IsolatedReview,CatalogApprovalStatus.Draft);
            var publicItems=puzzles.Puzzles.ToArray();Require(publicItems.Length==240&&publicItems.Select(p=>p.Identity.PuzzleId).Distinct(StringComparer.Ordinal).Count()==240,"REVIEW_PUBLIC_MEMBERSHIP_INVALID");var entries=new List<LockedCampaignEntry>();var seasons=Array(Need(c,"seasons"));Require(seasons.Count==1,"REVIEW_MEMBERSHIP_INVALID");var season=seasons[0];Fields(season,"id","order","titleKey","unlock","sections");Text(season,"id","S1");Integer(season,"order",1);Key(season,"titleKey");Always(Need(season,"unlock"));var sections=Array(Need(season,"sections"));Require(sections.Count==5,"REVIEW_MEMBERSHIP_INVALID");
            for(int si=0;si<5;si++){var section=sections[si];string sid="S1-"+(si+1).ToString("D2");Fields(section,"id","order","titleKey","unlock","routes");Text(section,"id",sid);Integer(section,"order",si+1);Key(section,"titleKey");Always(Need(section,"unlock"));var routes=Array(Need(section,"routes"));Require(routes.Count==4,"REVIEW_MEMBERSHIP_INVALID");for(int ri=0;ri<4;ri++){var route=routes[ri];string rid=sid+"-"+(ri+1).ToString("D2");Fields(route,"id","order","titleKey","unlock","levels");Text(route,"id",rid);Integer(route,"order",ri+1);Key(route,"titleKey");Always(Need(route,"unlock"));var levels=Array(Need(route,"levels"));Require(levels.Count==12,"REVIEW_MEMBERSHIP_INVALID");for(int li=0;li<12;li++){var level=levels[li];string id=rid+"-"+(li+1).ToString("D2");Fields(level,"puzzleId","order","unlock");Text(level,"puzzleId",id);Integer(level,"order",li+1);Always(Need(level,"unlock"));var item=publicItems.SingleOrDefault(p=>p.Identity.PuzzleId==id);Require(item!=null&&puzzles.TryGet(item.Identity,out var exact)&&exact!=null&&exact.Identity.Equals(item.Identity),"REVIEW_PUBLIC_MEMBERSHIP_INVALID");entries.Add(new LockedCampaignEntry(campaignId,item!.Identity,"S1",sid,rid,li+1,System.Array.Empty<string>()));}}}
            var rewards=Array(Need(v,"rewardDefinitions"));Require(rewards.Count==5,"REVIEW_COMPLETION_INVALID");string[] reasons={"LEVEL_FIRST_CLEAR","LEVEL_SECOND_STAR","LEVEL_THIRD_STAR","ROUTE_CLEAR","SECTION_CLEAR"};string[] kinds={"LEVEL","LEVEL","LEVEL","ROUTE","SECTION"};int[] amounts={15,5,10,60,150};var actualAmounts=new int[5];for(int i=0;i<5;i++){var reward=rewards.SingleOrDefault(r=>Need(r,"reasonCode").Kind==JsonKind.String&&Need(r,"reasonCode").StringValue==reasons[i]);Require(reward!=null,"REVIEW_COMPLETION_INVALID");Fields(reward!,"reasonCode","subjectKind","amountPatience","claimCardinality");Text(reward!,"subjectKind",kinds[i]);Text(reward!,"claimCardinality","ONCE_PER_SUBJECT");Integer(reward!,"amountPatience",amounts[i]);actualAmounts[i]=(int)Need(reward!,"amountPatience").IntegerValue;}
            var moments=Array(Need(v,"completionMoments"));Require(moments.Count>0,"REVIEW_COMPLETION_INVALID");var momentIds=new HashSet<string>(StringComparer.Ordinal);foreach(var moment in moments){Fields(moment,"id","resultTextKey","mapSegmentId","trainMomentId");foreach(string key in new[]{"id","mapSegmentId","trainMomentId"})Stable(moment,key);Key(moment,"resultTextKey");Require(momentIds.Add(Need(moment,"id").StringValue),"REVIEW_DUPLICATE_ID");}
            var times=Array(Need(wrapper,"starTimes"));Require(times.Count==240,"REVIEW_MEMBERSHIP_INVALID");var values=new CompletionValues[240];for(int i=0;i<240;i++){var time=times[i];Fields(time,"puzzleId","twoStarMs","threeStarMs");Text(time,"puzzleId",entries[i].Puzzle.PuzzleId);Require(Need(time,"twoStarMs").Kind==JsonKind.Null&&Need(time,"threeStarMs").Kind==JsonKind.Null,"REVIEW_STAR_TIMES_MUST_BE_NULL");values[i]=new CompletionValues(completionId,entries[i].Puzzle,actualAmounts[0],actualAmounts[1],actualAmounts[2],actualAmounts[3],actualAmounts[4],null,null);}
            Campaign=new CampaignView(campaignId,entries.ToArray());Completion=new CompletionView(completionId,values);
            string projection="{\"publicCatalog\":"+LocalSaveHash.CatalogProjection(puzzles.Identity)+",\"campaignCatalog\":"+LocalSaveHash.CatalogProjection(campaignId)+",\"completionCatalog\":"+LocalSaveHash.CatalogProjection(completionId)+"}";var aggregate=new CatalogIdentity("qa-public240-v1-catalog-set",1,LocalSaveHash.Bytes(LocalSaveHash.Canonical(canonicalJson,projection)),CatalogUse.IsolatedReview,CatalogApprovalStatus.Draft);CatalogSet=CatalogSetIdentity.Create(puzzles.Identity,campaignId,completionId,aggregate,canonicalJson);
        }
        /// <summary>Immutable all-open technical review membership.</summary>
        public ILockedCampaignCatalog Campaign {get;}
        /// <summary>Immutable identity-bound base values with explicitly null timing thresholds.</summary>
        public ILockedCompletionCatalog Completion {get;}
        /// <summary>Combined profile/envelope binding covering both complete review sources and injected public catalog.</summary>
        public CatalogSetIdentity CatalogSet {get;}
        private static byte[] Canonical(ICanonicalJson j,byte[] source){var r=j.Canonicalize(source,LocalSaveHash.Limits);Require(r.Succeeded,r.ErrorCode??"REVIEW_JSON_INVALID");return r.GetUtf8()!;}
        private static JsonValue Parse(byte[] b){var r=StrictJsonParser.Parse(b,new JsonParserLimits(67108864,64,65536,1048576,256));Require(r.Value!=null,r.Error?.Code??"REVIEW_JSON_INVALID");return r.Value!;}
        private static void Require(bool condition,string code){if(!condition)throw new ArgumentException(code);}
        private static JsonValue Need(JsonValue v,string name)=>v.Get(name)??throw new ArgumentException("REVIEW_FIELDS_INVALID");
        private static IReadOnlyList<JsonValue> Array(JsonValue v){Require(v.Kind==JsonKind.Array,"REVIEW_TYPE_INVALID");return v.Items;}
        private static void Fields(JsonValue v,params string[] fields){Require(v.Kind==JsonKind.Object&&v.Members.Count==fields.Length&&v.Members.All(m=>fields.Contains(m.Name)),"REVIEW_FIELDS_INVALID");}
        private static void Text(JsonValue v,string key,string expected){var n=Need(v,key);Require(n.Kind==JsonKind.String&&n.StringValue==expected,"REVIEW_VALUE_INVALID");}
        private static void Integer(JsonValue v,string key,long expected){var n=Need(v,key);Require(n.Kind==JsonKind.Integer&&n.IntegerValue==expected,"REVIEW_VALUE_INVALID");}
        private static void Key(JsonValue v,string key){var n=Need(v,key);Require(n.Kind==JsonKind.String&&System.Text.RegularExpressions.Regex.IsMatch(n.StringValue,@"^[a-z][a-z0-9-]*(\.[a-z0-9-]+)+$"),"REVIEW_KEY_INVALID");}
        private static void Stable(JsonValue v,string key){var n=Need(v,key);Require(n.Kind==JsonKind.String&&System.Text.RegularExpressions.Regex.IsMatch(n.StringValue,@"^[a-z][a-z0-9]*(?:-[a-z0-9]+)*$"),"REVIEW_ID_INVALID");}
        private static void Always(JsonValue v){Fields(v,"kind","requiredSubjectIds");Text(v,"kind","ALWAYS");Require(Array(Need(v,"requiredSubjectIds")).Count==0,"REVIEW_TECHNICAL_ACCESS_INVALID");}
    }
}
