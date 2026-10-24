using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using STP.Application;
using STP.Infrastructure.Content;

namespace STP.Bootstrap
{
    /// <summary>Immutable checked runtime bytes. This is a consumer grammar/lock boundary, never source approval or Human/Proof authority.</summary>
    public sealed class RuntimeGameSources
    {
        private static readonly string[] ProfileNames={"dienstbeginn","grosslage","regelbetrieb","stoerungslage"};
        private static readonly string[] Names=new[]{"Public.json","Campaign.json","CampaignCompletion.json","EndlessCompletion.json","Calibration.json","Cosmetics.json","Profiles/dienstbeginn.json","Profiles/regelbetrieb.json","Profiles/stoerungslage.json","Profiles/grosslage.json","RailwayAssets.json","CampaignText.json"}.OrderBy(x=>x,StringComparer.Ordinal).ToArray();
        private readonly Dictionary<string,byte[]> files;
        private readonly byte[] bindings;
        private RuntimeGameSources(IReadOnlyDictionary<string,byte[]> input,byte[] bindingBytes,CatalogUse use,string expectedBindingRaw,
            string expectedReleaseRaw,string expectedReleaseJcs,string? expectedApprovalRaw,string expectedUiRaw)
        {
            if(input==null||bindingBytes==null)throw new ArgumentNullException();Need(bindingBytes.Length>0&&bindingBytes.Length<=65536,"RUNTIME_BINDINGS_SIZE_INVALID");
            foreach(string hash in new[]{expectedBindingRaw,expectedReleaseRaw,expectedReleaseJcs,expectedUiRaw})Hash(hash);
            if(use==CatalogUse.Production)Hash(expectedApprovalRaw!);else Need(expectedApprovalRaw==null,"RUNTIME_FIXTURE_APPROVAL_FORBIDDEN");
            Need(input.Count==Names.Length&&input.Keys.OrderBy(x=>x,StringComparer.Ordinal).SequenceEqual(Names),"RUNTIME_FILE_SET_INVALID");
            files=new Dictionary<string,byte[]>(StringComparer.Ordinal);long total=0;
            foreach(string name in Names){var value=input[name]??throw new ArgumentException("RUNTIME_FILE_SIZE_INVALID");Need(value.Length>0&&value.Length<=8388608,"RUNTIME_FILE_SIZE_INVALID");total+=value.Length;Need(total<=33554432,"RUNTIME_SOURCE_SIZE_LIMIT");files.Add(name,(byte[])value.Clone());}
            bindings=(byte[])bindingBytes.Clone();Need(LocalSaveHash.Bytes(bindings)==expectedBindingRaw,"RUNTIME_BINDINGS_RAW_MISMATCH");
            var root=Parse(bindings,65536);Fields(root,"runtimeImportVersion","use","status","releaseLockRawSha256","releaseLockJcsSha256","approvalLockRawSha256","publicCatalog","campaignCatalog","campaignCompletionCatalog","endlessCompletionCatalog","calibrationCatalog","cosmeticsCatalog","qualityProfiles","catalogSet","railwayAssetsRawSha256","campaignTextRawSha256","uiSourceLockRawSha256","playerFiles");
            string useText=use==CatalogUse.Production?"Production":"ContractTest",status=use==CatalogUse.Production?"PRODUCT_APPROVED":"FIXTURE_ONLY";
            Need(Integer(Get(root,"runtimeImportVersion"))==1&&Text(root,"use")==useText&&Text(root,"status")==status,"RUNTIME_BINDINGS_USE_INVALID");
            Need(H(root,"releaseLockRawSha256")==expectedReleaseRaw&&H(root,"releaseLockJcsSha256")==expectedReleaseJcs&&H(root,"uiSourceLockRawSha256")==expectedUiRaw,"RUNTIME_EXTERNAL_LOCK_MISMATCH");
            Need(use==CatalogUse.Production?H(root,"approvalLockRawSha256")==expectedApprovalRaw:Get(root,"approvalLockRawSha256").Kind==JsonKind.Null,"RUNTIME_APPROVAL_LOCK_MISMATCH");
            var list=Array(root,"playerFiles");Need(list.Count==Names.Length,"RUNTIME_FILE_SET_INVALID");
            for(int i=0;i<Names.Length;i++){Fields(list[i],"path","rawSha256");Need(Text(list[i],"path")==Names[i]&&H(list[i],"rawSha256")==LocalSaveHash.Bytes(files[Names[i]]),"RUNTIME_FILE_RAW_MISMATCH");}
            Need(H(root,"railwayAssetsRawSha256")==LocalSaveHash.Bytes(files["RailwayAssets.json"])&&H(root,"campaignTextRawSha256")==LocalSaveHash.Bytes(files["CampaignText.json"]),"RUNTIME_PRESENTATION_RAW_MISMATCH");
            Use=use;var approval=use==CatalogUse.Production?CatalogApprovalStatus.ProductApproved:CatalogApprovalStatus.FixtureOnly;
            CatalogIdentity Ref(string key){var v=Get(root,key);Fields(v,"catalogId","revision","hashSha256");long revision=Integer(Get(v,"revision"));Need(revision>=1&&revision<=int.MaxValue,"RUNTIME_CATALOG_REVISION_INVALID");return new CatalogIdentity(Text(v,"catalogId"),(int)revision,H(v,"hashSha256"),use,approval);}
            ICanonicalJson json=new CanonicalJsonAdapter();
            Catalog=new RuntimePublicPuzzleCatalog(files["Public.json"],Ref("publicCatalog"));
            Calibration=new StarCalibrationCatalog(files["Calibration.json"],Catalog,Ref("calibrationCatalog"),json);
            CampaignCompletion=new RuntimeCompletionCatalog(files["CampaignCompletion.json"],Catalog,Ref("campaignCompletionCatalog"),json);
            EndlessCompletion=new RuntimeCompletionCatalog(files["EndlessCompletion.json"],Catalog,Ref("endlessCompletionCatalog"),json);
            Need(CampaignCompletion.SubjectKind=="LEVEL"&&EndlessCompletion.SubjectKind=="ENDLESS_INSTANCE"&&!CampaignCompletion.Identity.Equals(EndlessCompletion.Identity),"RUNTIME_COMPLETION_SCOPE_INVALID");
            Need(CampaignCompletion.Calibration.Identity.Equals(Calibration.Identity)&&EndlessCompletion.Calibration.Identity.Equals(Calibration.Identity),"RUNTIME_CALIBRATION_WRAPPER_MISMATCH");
            Progress=new RuntimeProgressCatalog(Catalog,files["Campaign.json"],Ref("campaignCatalog"),CampaignCompletion,json);
            Need(Progress.CatalogSet.AggregateCatalog.Equals(Ref("catalogSet")),"RUNTIME_CATALOG_SET_MISMATCH");
            var cosmeticRef=Ref("cosmeticsCatalog");Cosmetics=new LockedCosmeticsCatalog(files["Cosmetics.json"],cosmeticRef.HashSha256,use,json);Need(Cosmetics.Identity.Equals(cosmeticRef),"RUNTIME_COSMETICS_REFERENCE_MISMATCH");
            var profiles=Array(root,"qualityProfiles");Need(profiles.Count==4,"RUNTIME_QUALITY_NAMED_SET_INVALID");var parsed=new List<CalibratedEndlessProfile>();
            for(int i=0;i<4;i++){var row=profiles[i];Fields(row,"profileId","hashSha256");Need(Text(row,"profileId")==ProfileNames[i],"RUNTIME_QUALITY_NAMED_SET_INVALID");var profile=new LockedCalibratedEndlessProfile(files["Profiles/"+ProfileNames[i]+".json"],Catalog,EndlessCompletion,H(row,"hashSha256")).Profile;Need(profile.Id==ProfileNames[i],"RUNTIME_QUALITY_NAMED_SET_INVALID");parsed.Add(profile);}
            CalibratedProfiles=parsed.AsReadOnly();
            var ids=new[]{Catalog.Identity,Progress.Campaign.Identity,CampaignCompletion.Identity,EndlessCompletion.Identity,Calibration.Identity,Cosmetics.Identity};Need(ids.Select(c=>c.CatalogId).Concat(parsed.Select(p=>p.Id)).Distinct(StringComparer.Ordinal).Count()==10,"RUNTIME_CATALOG_ID_COLLISION");
            ValidateCampaignText(files["CampaignText.json"],status,Catalog);
            // Float-bearing RailwayAssets is copied and raw-bound, never altered or sent through the integer-JCS parser.
            // Its actual renderer/source crossreferences are checked by the producer/build before these injected locks exist.
            BindingRawSha256=expectedBindingRaw;ReleaseLockRawSha256=expectedReleaseRaw;ReleaseLockJcsSha256=expectedReleaseJcs;ApprovalLockRawSha256=expectedApprovalRaw;UiSourceLockRawSha256=expectedUiRaw;
        }
        public static RuntimeGameSources ReadProduction(IReadOnlyDictionary<string,byte[]> files,byte[] bindingsBytes,string expectedBindingRawSha256,string expectedReleaseLockRawSha256,string expectedReleaseLockJcsSha256,string expectedApprovalLockRawSha256,string expectedUiSourceLockRawSha256)
            =>new RuntimeGameSources(files,bindingsBytes,CatalogUse.Production,expectedBindingRawSha256,expectedReleaseLockRawSha256,expectedReleaseLockJcsSha256,expectedApprovalLockRawSha256,expectedUiSourceLockRawSha256);
        public static RuntimeGameSources ReadContractTest(IReadOnlyDictionary<string,byte[]> files,byte[] bindingsBytes,string expectedBindingRawSha256,string expectedReleaseLockRawSha256,string expectedReleaseLockJcsSha256,string expectedUiSourceLockRawSha256)
            =>new RuntimeGameSources(files,bindingsBytes,CatalogUse.ContractTest,expectedBindingRawSha256,expectedReleaseLockRawSha256,expectedReleaseLockJcsSha256,null,expectedUiSourceLockRawSha256);
        public CatalogUse Use{get;}public RuntimePublicPuzzleCatalog Catalog{get;}public RuntimeProgressCatalog Progress{get;}public LockedCosmeticsCatalog Cosmetics{get;}
        public StarCalibrationCatalog Calibration{get;}public RuntimeCompletionCatalog CampaignCompletion{get;}public RuntimeCompletionCatalog EndlessCompletion{get;}public IReadOnlyList<CalibratedEndlessProfile> CalibratedProfiles{get;}
        public string BindingRawSha256{get;}public string ReleaseLockRawSha256{get;}public string ReleaseLockJcsSha256{get;}public string? ApprovalLockRawSha256{get;}public string UiSourceLockRawSha256{get;}
        public IReadOnlyList<string> PlayerPaths=>System.Array.AsReadOnly(Names);
        public byte[] GetFileBytes(string path)=>files.TryGetValue(path,out var value)?(byte[])value.Clone():throw new ArgumentException("RUNTIME_FILE_NOT_BOUND");
        public byte[] GetBindingBytes()=>(byte[])bindings.Clone();
        private static void ValidateCampaignText(byte[] bytes,string status,IPublicPuzzleCatalog catalog)
        {
            var root=Parse(bytes,8388608);Fields(root,"schemaVersion","locale","approvalStatus","sourceSha256","entries");Need(Integer(Get(root,"schemaVersion"))==1&&Text(root,"locale")=="de-DE"&&Text(root,"approvalStatus")==status,"RUNTIME_CAMPAIGN_TEXT_HEADER");H(root,"sourceSha256");var rows=Array(root,"entries");Need(rows.Count==240,"RUNTIME_CAMPAIGN_TEXT_MEMBERSHIP");
            for(int i=0;i<240;i++){var row=rows[i];Fields(row,"puzzleId","title","status","result");Need(Text(row,"puzzleId")==catalog.Puzzles[i].Identity.PuzzleId,"RUNTIME_CAMPAIGN_TEXT_MEMBERSHIP");foreach(string key in new[]{"title","status","result"}){string text=Text(row,key);Need(!string.IsNullOrWhiteSpace(text)&&text.IndexOf('{')<0&&text.IndexOf('}')<0,"RUNTIME_CAMPAIGN_TEXT_INVALID");}}
        }
        private static void Need(bool value,string code){if(!value)throw new ArgumentException(code);}
        private static void Hash(string value)=>Need(value!=null&&value.Length==64&&Regex.IsMatch(value,"^[0-9a-f]{64}$",RegexOptions.CultureInvariant),"RUNTIME_HASH_INVALID");
        private static string H(JsonValue v,string key){string value=Text(v,key);Hash(value);return value;}
        private static JsonValue Parse(byte[] bytes,int limit){var value=StrictJsonParser.Parse(bytes,new JsonParserLimits(limit,64,65536,1048576,256));Need(value.Succeeded,"RUNTIME_JSON_INVALID");return value.Value!;}
        private static JsonValue Get(JsonValue v,string key)=>v.Get(key)??throw new ArgumentException("RUNTIME_FIELDS_INVALID");
        private static string Text(JsonValue v,string key){var value=Get(v,key);Need(value.Kind==JsonKind.String,"RUNTIME_STRING_INVALID");return value.StringValue;}
        private static long Integer(JsonValue v){Need(v.Kind==JsonKind.Integer&&v.IntegerValue>=0&&v.IntegerValue<=9007199254740991L,"RUNTIME_INTEGER_INVALID");return v.IntegerValue;}
        private static IReadOnlyList<JsonValue> Array(JsonValue v,string key){var a=Get(v,key);Need(a.Kind==JsonKind.Array,"RUNTIME_ARRAY_INVALID");return a.Items;}
        private static void Fields(JsonValue v,params string[] fields)=>Need(v.Kind==JsonKind.Object&&v.Members.Count==fields.Length&&v.Members.All(m=>fields.Contains(m.Name,StringComparer.Ordinal)),"RUNTIME_FIELDS_INVALID");
    }
}
