using System;
using System.Collections.Generic;
using System.Linq;
using STP.Application;

namespace STP.Infrastructure.Content
{
    internal static class StarSource
    {
        internal static void Require(bool condition,string code){if(!condition)throw new ArgumentException(code);}
        internal static JsonValue Parse(byte[] source){var p=StrictJsonParser.Parse(source,new JsonParserLimits(8388608,32,65536,1048576,128));Require(p.Succeeded,"STAR_JSON_INVALID");return p.Value!;}
        internal static JsonValue Get(JsonValue v,string key)=>v.Get(key)??throw new ArgumentException("STAR_FIELDS_INVALID");
        internal static void Fields(JsonValue v,params string[] fields)=>Require(v.Kind==JsonKind.Object&&v.Members.Count==fields.Length&&v.Members.All(m=>fields.Contains(m.Name,StringComparer.Ordinal)),"STAR_FIELDS_INVALID");
        internal static string S(JsonValue v){Require(v.Kind==JsonKind.String&&!string.IsNullOrWhiteSpace(v.StringValue),"STAR_STRING_INVALID");return v.StringValue;}
        internal static string S(JsonValue v,string key)=>S(Get(v,key));
        internal static long N(JsonValue v){Require(v.Kind==JsonKind.Integer&&v.IntegerValue>=0&&v.IntegerValue<=9007199254740991L,"STAR_INTEGER_INVALID");return v.IntegerValue;}
        internal static long N(JsonValue v,string key)=>N(Get(v,key));
        internal static int I(JsonValue v,string key){long n=N(v,key);Require(n<=int.MaxValue,"STAR_INTEGER_INVALID");return(int)n;}
        internal static IReadOnlyList<JsonValue> A(JsonValue v,string key){var a=Get(v,key);Require(a.Kind==JsonKind.Array,"STAR_ARRAY_INVALID");return a.Items;}
        internal static string Hash(JsonValue v)=>LevelHashing.HashProjection(v);
        internal static byte[] Bytes(JsonValue v)=>JcsSerializer.SerializeOrThrow(v);
        internal static string H(JsonValue v,string key){string hash=S(v,key);Require(System.Text.RegularExpressions.Regex.IsMatch(hash,"^[a-f0-9]{64}$"),"STAR_HASH_INVALID");return hash;}
        internal static void Reference(JsonValue v,CatalogIdentity expected){Fields(v,"catalogId","revision","hashSha256");Require(S(v,"catalogId")==expected.CatalogId&&N(v,"revision")==expected.Revision&&H(v,"hashSha256")==expected.HashSha256,"STAR_CATALOG_REFERENCE_MISMATCH");}
        internal static CatalogIdentity Identity(JsonValue v,CatalogUse use,string hash)
        {string text=S(v,"approvalStatus");var status=text=="PRODUCT_APPROVED"?CatalogApprovalStatus.ProductApproved:text=="FIXTURE_ONLY"?CatalogApprovalStatus.FixtureOnly:text=="DRAFT"?CatalogApprovalStatus.Draft:throw new ArgumentException("STAR_APPROVAL_INVALID");return new CatalogIdentity(S(v,"catalogId"),I(v,"catalogRevision"),hash,use,status);}
        internal static void Expected(JsonValue v,CatalogIdentity expected){Require(Identity(v,expected.Use,Hash(v)).Equals(expected),"STAR_CATALOG_LOCK_MISMATCH");}
        internal static PublicPuzzleIdentity PuzzleIdentity(JsonValue v){Fields(v,"puzzleId","profile","sha256");return new PublicPuzzleIdentity(S(v,"puzzleId"),S(v,"profile"),H(v,"sha256"));}
        internal static void Key(string key)=>Require(System.Text.RegularExpressions.Regex.IsMatch(key,@"^[a-z][a-z0-9-]*(\.[a-z0-9-]+)+$"),"STAR_TEXT_KEY_INVALID");
        internal static void Stable(string id)=>Require(System.Text.RegularExpressions.Regex.IsMatch(id,@"^[a-z][a-z0-9-]*$"),"STAR_ID_INVALID");
    }
    public sealed class CalibrationProvenance
    {
        internal CalibrationProvenance(string version,string evidence,string ui){CalibrationVersion=version;EvidenceSha256=evidence;UiSourceSha256=ui;}
        public string CalibrationVersion{get;}public string EvidenceSha256{get;}public string UiSourceSha256{get;}
    }
    public sealed class CalibratedCampaignTimes
    {
        internal CalibratedCampaignTimes(PublicPuzzleIdentity identity,string document,CalibratedStarTimes times){Identity=identity;SourceDocumentSha256=document;Times=times;}
        public PublicPuzzleIdentity Identity{get;}public string SourceDocumentSha256{get;}public CalibratedStarTimes Times{get;}
    }
    public sealed class CalibratedProfileTimes
    {
        internal CalibratedProfileTimes(string id,string hash,CalibratedStarTimes times){ProfileId=id;ProfileHashSha256=hash;Times=times;}
        public string ProfileId{get;}public string ProfileHashSha256{get;}public CalibratedStarTimes Times{get;}
    }
    /// <summary>Strict ADR044 source reference/value reader; actual evidence/source approval is a separate locked release import.</summary>
    public sealed class StarCalibrationCatalog
    {
        public StarCalibrationCatalog(byte[] source,IPublicPuzzleCatalog corpus,CatalogIdentity expected,ICanonicalJson json)
        {
            if(corpus==null||expected==null||json==null)throw new ArgumentNullException();StarSource.Require(expected.Use==corpus.Identity.Use&&(expected.Use==CatalogUse.Production||expected.Use==CatalogUse.ContractTest),"STAR_CATALOG_USE_MISMATCH");var v=StarSource.Parse(source);StarSource.Fields(v,"schemaVersion","catalogId","catalogRevision","approvalStatus","publicCatalog","provenance","campaign","endlessProfiles");StarSource.Require(StarSource.N(v,"schemaVersion")==1,"STAR_SCHEMA_UNSUPPORTED");StarSource.Expected(v,expected);StarSource.Reference(StarSource.Get(v,"publicCatalog"),corpus.Identity);Identity=expected;
            var provenance=new List<CalibrationProvenance>();foreach(var row in StarSource.A(v,"provenance")){StarSource.Fields(row,"calibrationVersion","evidenceSha256","uiSourceSha256");provenance.Add(new CalibrationProvenance(StarSource.S(row,"calibrationVersion"),StarSource.H(row,"evidenceSha256"),StarSource.H(row,"uiSourceSha256")));}StarSource.Require(provenance.Count>0&&provenance.Select(p=>p.CalibrationVersion).Distinct(StringComparer.Ordinal).Count()==provenance.Count,"STAR_PROVENANCE_INVALID");Provenance=provenance.AsReadOnly();
            CalibratedStarTimes Times(JsonValue row){var t=new CalibratedStarTimes(StarSource.S(row,"calibrationVersion"),StarSource.N(row,"twoStarsSeconds"),StarSource.N(row,"threeStarsSeconds"));StarSource.Require(provenance.Any(p=>p.CalibrationVersion==t.CalibrationVersion),"STAR_PROVENANCE_MISSING");return t;}
            var campaign=new List<CalibratedCampaignTimes>();foreach(var row in StarSource.A(v,"campaign")){StarSource.Fields(row,"identity","sourceDocumentSha256","calibrationVersion","twoStarsSeconds","threeStarsSeconds");var identity=StarSource.PuzzleIdentity(StarSource.Get(row,"identity"));StarSource.Require(corpus.TryGet(identity,out _),"STAR_CAMPAIGN_IDENTITY_MISMATCH");campaign.Add(new CalibratedCampaignTimes(identity,StarSource.H(row,"sourceDocumentSha256"),Times(row)));}StarSource.Require(corpus.Puzzles.Count==240&&campaign.Count==240&&campaign.Select(e=>e.Identity.PuzzleId).Distinct(StringComparer.Ordinal).Count()==240&&corpus.Puzzles.All(p=>campaign.Any(e=>e.Identity.Equals(p.Identity))),"STAR_CAMPAIGN_MEMBERSHIP_INVALID");Campaign=campaign.AsReadOnly();
            var profiles=new List<CalibratedProfileTimes>();foreach(var row in StarSource.A(v,"endlessProfiles")){StarSource.Fields(row,"profileId","profileHashSha256","calibrationVersion","twoStarsSeconds","threeStarsSeconds");profiles.Add(new CalibratedProfileTimes(StarSource.S(row,"profileId"),StarSource.H(row,"profileHashSha256"),Times(row)));}var ids=new[]{"dienstbeginn","regelbetrieb","stoerungslage","grosslage"};StarSource.Require(profiles.Count==4&&ids.All(id=>profiles.Count(p=>p.ProfileId==id)==1),"STAR_PROFILE_MEMBERSHIP_INVALID");EndlessEntries=profiles.AsReadOnly();
        }
        public CatalogIdentity Identity{get;}public IReadOnlyList<CalibrationProvenance> Provenance{get;}public IReadOnlyList<CalibratedCampaignTimes> Campaign{get;}public IReadOnlyList<CalibratedProfileTimes> EndlessEntries{get;}
        public CalibratedCampaignTimes GetCampaign(PublicPuzzleIdentity identity)=>Campaign.Single(e=>e.Identity.Equals(identity));
        public CalibratedProfileTimes GetEndless(string id,string hash){var entries=EndlessEntries.Where(e=>e.ProfileId==id&&e.ProfileHashSha256==hash).ToArray();StarSource.Require(entries.Length==1,"STAR_PROFILE_SOURCE_MISSING");return entries[0];}
    }
}
