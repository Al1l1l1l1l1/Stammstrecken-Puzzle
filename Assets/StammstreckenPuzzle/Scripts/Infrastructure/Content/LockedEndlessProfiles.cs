using System;
using System.Collections.Generic;
using System.Linq;
using STP.Application;

namespace STP.Infrastructure.Content
{
    /// <summary>Strict actual DRAFT profiles for ADR040; this reader never creates production approval.</summary>
    public sealed class LockedEndlessProfiles
    {
        private readonly IReadOnlyList<EndlessSearchProfile> profiles;
        public LockedEndlessProfiles(byte[] source,IPublicPuzzleCatalog corpus)
        {
            if(corpus==null||corpus.Identity.Use!=CatalogUse.InternalPreview||corpus.Identity.ApprovalStatus!=CatalogApprovalStatus.Draft)throw new ArgumentException("GEN_CANDIDATE_REQUIRED");
            var parsed=StrictJsonParser.Parse(source,new JsonParserLimits(65536,8,1024,128,32));if(!parsed.Succeeded)throw new ArgumentException("GEN_PROFILE_JSON");var root=parsed.Value!;
            Fields(root,"schemaVersion","use","approvalStatus","productApproved","humanCasesRequiredPerProfile","profiles");Int(root,"schemaVersion",1);Text(root,"use","InternalPreview");Text(root,"approvalStatus","DRAFT");Require(Get(root,"productApproved").Kind==JsonKind.Boolean&&!Get(root,"productApproved").BooleanValue,"GEN_PROFILE_PROMOTION_FORBIDDEN");Int(root,"humanCasesRequiredPerProfile",12);
            var list=Get(root,"profiles");Require(list.Kind==JsonKind.Array&&list.Items.Count==4,"GEN_PROFILE_COUNT");var result=new List<EndlessSearchProfile>();string[] ids={"dienstbeginn","regelbetrieb","stoerungslage","grosslage"};
            for(int i=0;i<4;i++)
            {
                var item=list.Items[i];Fields(item,"profileId","label","approvalStatus","humanStatus","generatorVersion","solverVersion","noveltyVersion","publicCatalogHashSha256","parameters","profileHashSha256");Text(item,"profileId",ids[i]);Require(Get(item,"label").Kind==JsonKind.String&&!string.IsNullOrWhiteSpace(Get(item,"label").StringValue),"GEN_PROFILE_LABEL");Text(item,"approvalStatus","DRAFT");Text(item,"humanStatus","NOT_EXECUTED");Text(item,"generatorVersion",PuzzleCandidateSearch.GeneratorVersion);Text(item,"solverVersion","solver-v2");Text(item,"noveltyVersion",PuzzleCandidateSearch.NoveltyVersion);Text(item,"publicCatalogHashSha256",corpus.Identity.HashSha256);
                string hash=Get(item,"profileHashSha256").StringValue;Require(LevelHashing.HashProjection(JsonValue.CreateObject(item.Members.Where(m=>m.Name!="profileHashSha256")))==hash,"GEN_PROFILE_HASH_MISMATCH");
                result.Add(new EndlessSearchProfile(ids[i],hash,corpus.Identity.HashSha256,ReadSearch(Get(item,"parameters"))));
            }
            profiles=result.AsReadOnly();
        }
        public IReadOnlyList<EndlessSearchProfile> Profiles=>profiles;
        public EndlessSearchProfile Get(string id)=>profiles.Single(p=>p.Id==id);
        private static PuzzleCandidateParameters ReadSearch(JsonValue v)
        {
            string[] names={"minSize","maxSize","minDensityPercent","maxDensityPercent","minDeductionSteps","minDeductionDepth","maxAttempts","pathStateBudget","totalBudgetMs","solverBudgetMs","maxTrivialLines","recentSimilarityPercent"};Fields(v,names);int N(string key){var x=Get(v,key);Require(x.Kind==JsonKind.Integer&&x.IntegerValue>=0&&x.IntegerValue<=int.MaxValue,"GEN_PROFILE_PARAMETER_RANGE");return(int)x.IntegerValue;}
            return new PuzzleCandidateParameters(N(names[0]),N(names[1]),N(names[2]),N(names[3]),N(names[4]),N(names[5]),N(names[6]),N(names[7]),N(names[8]),N(names[9]),N(names[10]),N(names[11]));
        }
        private static void Require(bool b,string code){if(!b)throw new ArgumentException(code);}
        private static JsonValue Get(JsonValue v,string name)=>v.Get(name)??throw new ArgumentException("GEN_PROFILE_FIELDS");
        private static void Fields(JsonValue v,params string[] fields)=>Require(v.Kind==JsonKind.Object&&v.Members.Count==fields.Length&&v.Members.All(m=>fields.Contains(m.Name)),"GEN_PROFILE_FIELDS");
        private static void Text(JsonValue v,string key,string expected)=>Require(Get(v,key).Kind==JsonKind.String&&Get(v,key).StringValue==expected,"GEN_PROFILE_BINDING");
        private static void Int(JsonValue v,string key,long expected)=>Require(Get(v,key).Kind==JsonKind.Integer&&Get(v,key).IntegerValue==expected,"GEN_PROFILE_BINDING");
    }
}
