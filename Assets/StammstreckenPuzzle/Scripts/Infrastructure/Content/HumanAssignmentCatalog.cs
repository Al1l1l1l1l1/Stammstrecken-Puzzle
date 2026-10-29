using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STP.Application;

namespace STP.Infrastructure.Content
{
    /// <summary>ADR042 public-only, exact-byte-bound internal assignments. No progress or provider configuration.</summary>
    public sealed class HumanAssignmentCatalog:IHumanAssignmentCatalog
    {
        public const string ApplicationId="com.STP.StammstreckenPuzzle.human";
        public HumanAssignmentCatalog(byte[] source,string expectedManifestBytesSha256,IPublicPuzzleCatalog currentPublic,ICanonicalJson json)
        {
            Require(source!=null&&source.Length<=1048576&&LocalSaveHash.Bytes(source)==expectedManifestBytesSha256,"HUMAN_MANIFEST_BYTES_MISMATCH");
            Require(currentPublic!=null&&currentPublic.Identity.Use==CatalogUse.InternalPreview&&currentPublic.Identity.ApprovalStatus==CatalogApprovalStatus.Draft,"HUMAN_CURRENT_PUBLIC_REQUIRED");
            var parsed=StrictJsonParser.Parse(source!,new JsonParserLimits(1048576,12,1024,2048,32));Require(parsed.Succeeded,"HUMAN_MANIFEST_JSON_INVALID");var root=parsed.Value!;
            Fields(root,"schemaVersion","kind","approvalStatus","applicationId","studyId","revision","buildId","uiSourceSha256","publicCatalogHashSha256","assignments");
            Require(Integer(Get(root,"schemaVersion"))==1&&Text(Get(root,"kind"))=="HUMAN_CALIBRATION_ASSIGNMENTS"&&Text(Get(root,"approvalStatus"))=="DRAFT"&&Text(Get(root,"applicationId"))==ApplicationId,"HUMAN_MANIFEST_HEADER_INVALID");
            ManifestBytesSha256=expectedManifestBytesSha256;StudyId=Text(Get(root,"studyId"));Require(System.Text.RegularExpressions.Regex.IsMatch(StudyId,"^[a-z][a-z0-9-]{0,63}$"),"HUMAN_STUDY_ID_INVALID");Revision=Integer(Get(root,"revision"));Require(Revision>0,"HUMAN_REVISION_INVALID");BuildId=Text(Get(root,"buildId"));Require(!string.IsNullOrWhiteSpace(BuildId)&&BuildId.Length<=128,"HUMAN_BUILD_ID_INVALID");UiSourceSha256=Hash(Get(root,"uiSourceSha256"));PublicCatalogHashSha256=Hash(Get(root,"publicCatalogHashSha256"));Require(PublicCatalogHashSha256==currentPublic!.Identity.HashSha256,"HUMAN_CURRENT_PUBLIC_MISMATCH");
            var items=Get(root,"assignments");Require(items.Kind==JsonKind.Array&&items.Items.Count>0&&items.Items.Count<=128,"HUMAN_ASSIGNMENT_LIMIT");var result=new List<HumanAssignment>();
            foreach(var item in items.Items)
            {
                Fields(item,"assignmentId","purpose","publicPuzzleInput","publicSourceSha256","sourceSha256","contentRevision","profileId","profileHashSha256");
                string purpose=Text(Get(item,"purpose"));Require(purpose=="CAMPAIGN_TIME"||purpose=="ENDLESS_PROFILE","HUMAN_PURPOSE_INVALID");var p=ReadPublic(Get(item,"publicPuzzleInput"),json);
                string publicBytes=Hash(Get(item,"publicSourceSha256"));Require(publicBytes==LocalSaveHash.Bytes(LocalSaveHash.Canonical(json,EndlessProjection.PublicInput(p,true))),"HUMAN_PUBLIC_SOURCE_MISMATCH");
                if(purpose=="CAMPAIGN_TIME")Require(currentPublic.TryGet(p.Identity,out var exact)&&exact!=null,"HUMAN_PUZZLE_SOURCE_UNRESOLVED");
                var revision=Get(item,"contentRevision");var profile=Get(item,"profileId");var profileHash=Get(item,"profileHashSha256");
                result.Add(new HumanAssignment(Text(Get(item,"assignmentId")),(HumanAssignmentPurpose)Enum.Parse(typeof(HumanAssignmentPurpose),purpose),p,publicBytes,Hash(Get(item,"sourceSha256")),revision.Kind==JsonKind.Null?(int?)null:Integer(revision),profile.Kind==JsonKind.Null?null:Text(profile),profileHash.Kind==JsonKind.Null?null:Hash(profileHash)));
            }
            Require(result.Select(a=>a.Id).Distinct(StringComparer.Ordinal).Count()==result.Count&&result.Select(a=>a.Puzzle.Identity).Distinct().Count()==result.Count,"HUMAN_ASSIGNMENT_ALIAS_FORBIDDEN");Assignments=Array.AsReadOnly(result.ToArray());
        }
        public string ManifestBytesSha256{get;} public string StudyId{get;} public int Revision{get;} public string BuildId{get;} public string UiSourceSha256{get;} public string PublicCatalogHashSha256{get;} public IReadOnlyList<HumanAssignment> Assignments{get;}
        private static PublicPuzzleInput ReadPublic(JsonValue v,ICanonicalJson json)
        {
            Fields(v,"puzzleId","rulesetVersion","grid","endpoints","rowCounts","columnCounts","publicPuzzleHash");var g=Get(v,"grid");Fields(g,"width","height");var ep=Get(v,"endpoints");Fields(ep,"a","b");var a=Get(ep,"a");var b=Get(ep,"b");Fields(a,"side","index");Fields(b,"side","index");var h=Get(v,"publicPuzzleHash");Fields(h,"profile","sha256");
            PublicPuzzleSide Side(JsonValue x){string s=Text(x);Require(Enum.TryParse(s,false,out PublicPuzzleSide value)&&Enum.IsDefined(typeof(PublicPuzzleSide),value),"HUMAN_PUBLIC_SIDE_INVALID");return value;}
            int[] Counts(JsonValue x){Require(x.Kind==JsonKind.Array&&x.Items.Count<=32,"HUMAN_PUBLIC_COUNTS_INVALID");return x.Items.Select(Integer).ToArray();}
            var input=new PublicPuzzleInput(new PublicPuzzleIdentity(Text(Get(v,"puzzleId")),Text(Get(h,"profile")),Hash(Get(h,"sha256"))),Text(Get(v,"rulesetVersion")),Integer(Get(g,"width")),Integer(Get(g,"height")),Side(Get(a,"side")),Integer(Get(a,"index")),Side(Get(b,"side")),Integer(Get(b,"index")),Counts(Get(v,"rowCounts")),Counts(Get(v,"columnCounts")));
            EndlessProjection.ValidatePublic(input,json);return input;
        }
        private static JsonValue Get(JsonValue v,string key)=>v.Get(key)??throw new ArgumentException("HUMAN_FIELDS_INVALID");
        private static void Fields(JsonValue v,params string[] fields)=>Require(v.Kind==JsonKind.Object&&v.Members.Count==fields.Length&&v.Members.All(m=>fields.Contains(m.Name)),"HUMAN_FIELDS_INVALID");
        private static string Text(JsonValue v){Require(v.Kind==JsonKind.String,"HUMAN_STRING_INVALID");return v.StringValue;}
        private static int Integer(JsonValue v){Require(v.Kind==JsonKind.Integer&&v.IntegerValue>=0&&v.IntegerValue<=int.MaxValue,"HUMAN_INTEGER_INVALID");return(int)v.IntegerValue;}
        private static string Hash(JsonValue v){string s=Text(v);Require(System.Text.RegularExpressions.Regex.IsMatch(s,"^[a-f0-9]{64}$"),"HUMAN_HASH_INVALID");return s;}
        private static void Require(bool value,string code){if(!value)throw new ArgumentException(code);}
    }
}
