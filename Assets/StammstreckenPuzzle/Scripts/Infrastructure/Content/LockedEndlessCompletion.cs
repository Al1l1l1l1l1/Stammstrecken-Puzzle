using System;
using System.Linq;
using STP.Application;

namespace STP.Infrastructure.Content
{
    /// <summary>Actual separately locked ENDLESS_INSTANCE completion source, with no campaign subject conversion.</summary>
    public sealed class LockedEndlessCompletion
    {
        public LockedEndlessCompletion(byte[] source,string expectedHash,ICanonicalJson json)
        {
            var parsed=StrictJsonParser.Parse(source,new JsonParserLimits(65536,8,1024,128,16));if(!parsed.Succeeded)throw new ArgumentException("ENDLESS_COMPLETION_JSON");var root=parsed.Value!;
            Fields(root,"schemaVersion","catalogId","catalogRevision","approvalStatus","rewardDefinitions","completionMoments");Int(root,"schemaVersion",1);Int(root,"catalogRevision",1);Text(root,"catalogId","completion-qa-mobile-candidate-endless");Text(root,"approvalStatus","DRAFT");
            string hash=LevelHashing.HashProjection(root);Require(hash==expectedHash,"ENDLESS_COMPLETION_LOCK_MISMATCH");var definitions=Get(root,"rewardDefinitions");Require(definitions.Kind==JsonKind.Array&&definitions.Items.Count==4,"ENDLESS_COMPLETION_REWARDS");
            string[] reasons={"LEVEL_FIRST_CLEAR","LEVEL_SECOND_STAR","LEVEL_THIRD_STAR","POST_CLEAR_PATIENCE"};long[] amounts={15,5,10,10};
            for(int i=0;i<4;i++){var d=definitions.Items.SingleOrDefault(v=>Get(v,"reasonCode").StringValue==reasons[i])??throw new ArgumentException("ENDLESS_COMPLETION_REWARDS");Fields(d,"reasonCode","subjectKind","amountPatience","claimCardinality");Text(d,"subjectKind","ENDLESS_INSTANCE");Text(d,"claimCardinality","ONCE_PER_SUBJECT");Int(d,"amountPatience",amounts[i]);}
            var moments=Get(root,"completionMoments");Require(moments.Kind==JsonKind.Array&&moments.Items.Count==1,"ENDLESS_COMPLETION_MOMENT");var moment=moments.Items[0];Fields(moment,"id","resultTextKey","mapSegmentId","trainMomentId");foreach(var key in new[]{"id","mapSegmentId","trainMomentId"})Require(System.Text.RegularExpressions.Regex.IsMatch(Get(moment,key).StringValue,@"^[a-z][a-z0-9]*(?:-[a-z0-9]+)*$"),"ENDLESS_COMPLETION_MOMENT");Require(System.Text.RegularExpressions.Regex.IsMatch(Get(moment,"resultTextKey").StringValue,@"^[a-z][a-z0-9-]*(\.[a-z0-9-]+)+$"),"ENDLESS_COMPLETION_MOMENT");
            Identity=new CatalogIdentity("completion-qa-mobile-candidate-endless",1,hash,CatalogUse.InternalPreview,CatalogApprovalStatus.Draft);
        }
        public CatalogIdentity Identity{get;}
        private static void Require(bool b,string code){if(!b)throw new ArgumentException(code);}
        private static JsonValue Get(JsonValue v,string name)=>v.Get(name)??throw new ArgumentException("ENDLESS_COMPLETION_FIELDS");
        private static void Fields(JsonValue v,params string[] f)=>Require(v.Kind==JsonKind.Object&&v.Members.Count==f.Length&&v.Members.All(m=>f.Contains(m.Name)),"ENDLESS_COMPLETION_FIELDS");
        private static void Text(JsonValue v,string key,string expected)=>Require(Get(v,key).Kind==JsonKind.String&&Get(v,key).StringValue==expected,"ENDLESS_COMPLETION_BINDING");
        private static void Int(JsonValue v,string key,long expected)=>Require(Get(v,key).Kind==JsonKind.Integer&&Get(v,key).IntegerValue==expected,"ENDLESS_COMPLETION_BINDING");
    }
}
