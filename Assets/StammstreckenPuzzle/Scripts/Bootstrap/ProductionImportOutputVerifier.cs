using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using STP.Infrastructure.Content;

namespace STP.Bootstrap
{
    /// <summary>Primitive facts from actual importer bytes. This helper grants no source, Human or native approval.</summary>
    public sealed class ProductionImportOutputFacts
    {
        internal ProductionImportOutputFacts(string bindings,string releaseRaw,string releaseJcs,string? approval,string ui,Dictionary<string,string> originals)
        {BindingsRawSha256=bindings;ReleaseLockRawSha256=releaseRaw;ReleaseLockJcsSha256=releaseJcs;ApprovalLockRawSha256=approval;UiSourceLockRawSha256=ui;OriginalRawHashes=new ReadOnlyDictionary<string,string>(originals);}
        public string BindingsRawSha256{get;}public string ReleaseLockRawSha256{get;}public string ReleaseLockJcsSha256{get;}public string? ApprovalLockRawSha256{get;}public string UiSourceLockRawSha256{get;}
        public IReadOnlyDictionary<string,string> OriginalRawHashes{get;}
    }

    /// <summary>Existing strict JSON/JCS and RuntimeGameSources behind Bootstrap's existing Content edge.</summary>
    public static class ProductionImportOutputVerifier
    {
        private static readonly string[] RequestFields={"importRequestVersion","use","publicCatalog","campaign","campaignCompletion","endlessCompletion","calibration","cosmetics","profiles","levels","localizations","railwayAssets","releaseLock","toolchainLock","uiSourceLock","evidence","publishedLocks"};
        private static readonly string[] PlayerNames={"Calibration.json","Campaign.json","CampaignCompletion.json","CampaignText.json","Cosmetics.json","EndlessCompletion.json","Profiles/dienstbeginn.json","Profiles/grosslage.json","Profiles/regelbetrieb.json","Profiles/stoerungslage.json","Public.json","RailwayAssets.json","Runtime.bindings.json"};
        public static IReadOnlyList<string> PlayerPaths=>System.Array.AsReadOnly(PlayerNames);
        public static string RawHash(byte[] bytes)=>LevelHashing.Sha256Hex(bytes??throw new ArgumentNullException(nameof(bytes)));
        public static void RequireHash(string value)=>Need(value!=null&&Regex.IsMatch(value,@"\A[0-9a-f]{64}\z"),"PREFLIGHT_HASH_INVALID");
        public static void RequireRelativePath(string value)=>Need(!string.IsNullOrWhiteSpace(value)&&value.Length<=512&&!value.Contains('\\')&&!value.Contains(':')&&!value.StartsWith("/",StringComparison.Ordinal)&&!value.Split('/').Any(s=>s==""||s=="."||s=="..")&&!value.Any(char.IsControl),"PREFLIGHT_RELATIVE_PATH_INVALID");
        public static IReadOnlyList<string> CheckProductionRequest(byte[] bytes)
        {var request=ReadRequest(bytes,"Production");return System.Array.AsReadOnly(new[]{Path(request,"releaseLock"),Path(request,"uiSourceLock")});}
        public static void CheckProductionApproval(byte[] bytes,string expectedRawSha256)
        {
            RequireHash(expectedRawSha256);Need(RawHash(bytes)==expectedRawSha256,"PREFLIGHT_APPROVAL_RAW_MISMATCH");var value=Parse(bytes);
            Fields(value,"approvalLockVersion","kind","use","releaseLock","evidence","sources","acceptedBy","decisionRef","acceptance");
            Need(Number(Get(value,"approvalLockVersion"))==1&&Text(value,"kind")=="PRODUCTION_SOURCE_APPROVAL_LOCK"&&Text(value,"use")=="Production","PREFLIGHT_APPROVAL_KIND_INVALID");
        }
        public static ProductionImportOutputFacts VerifyProduction(IReadOnlyDictionary<string,byte[]> player,byte[] sourceBindings,byte[] rawLock,byte[] release,byte[] ui,byte[] approvalOriginal,string approvalRelativePath,string expectedApprovalRaw)
        {
            CheckProductionApproval(approvalOriginal,expectedApprovalRaw);RequireRelativePath(approvalRelativePath);
            var facts=Verify(player,sourceBindings,rawLock,release,ui,expectedApprovalRaw,true);
            var trusted=Parse(approvalOriginal);var expected=new Dictionary<string,string>(StringComparer.Ordinal);
            foreach(var row in Array(trusted,"sources")){Fields(row,"path","rawSha256");expected.Add(Path(row,"path"),Hash(row,"rawSha256"));}
            Need(!expected.ContainsKey(approvalRelativePath),"PREFLIGHT_APPROVAL_SELF_SOURCE_FORBIDDEN");expected.Add(approvalRelativePath,expectedApprovalRaw);
            Need(expected.Count==facts.OriginalRawHashes.Count&&expected.All(e=>facts.OriginalRawHashes.TryGetValue(e.Key,out var hash)&&hash==e.Value),"PREFLIGHT_APPROVAL_ORIGINAL_SET_MISMATCH");
            var releaseRef=Get(trusted,"releaseLock");Fields(releaseRef,"path","rawSha256");Need(expected.TryGetValue(Path(releaseRef,"path"),out var releaseHash)&&releaseHash==facts.ReleaseLockRawSha256&&Hash(releaseRef,"rawSha256")==facts.ReleaseLockRawSha256,"PREFLIGHT_APPROVAL_RELEASE_MISMATCH");
            return facts;
        }
#if UNITY_INCLUDE_TESTS
        /// <summary>Dedicated technical entry, never a Production preflight result or approval.</summary>
        public static IReadOnlyList<string> CheckContractTestRequest(byte[] bytes)
        {var request=ReadRequest(bytes,"ContractTest");return System.Array.AsReadOnly(new[]{Path(request,"releaseLock"),Path(request,"uiSourceLock")});}
        public static ProductionImportOutputFacts VerifyContractTest(IReadOnlyDictionary<string,byte[]> player,byte[] sourceBindings,byte[] rawLock,byte[] release,byte[] ui)
            =>Verify(player,sourceBindings,rawLock,release,ui,null,false);
#endif
        private static ProductionImportOutputFacts Verify(IReadOnlyDictionary<string,byte[]> player,byte[] sourceBindings,byte[] rawLock,byte[] release,byte[] ui,string? expectedApprovalRaw,bool production)
        {
            Need(player!=null&&player.Count==13&&player.Keys.OrderBy(k=>k,StringComparer.Ordinal).SequenceEqual(PlayerNames),"PREFLIGHT_PLAYER_FILE_SET_INVALID");
            long sum=0;foreach(var value in player!.Values){Need(value!=null&&value.Length>0&&value.Length<=8388608,"PREFLIGHT_PLAYER_SIZE_INVALID");sum+=value!.Length;}Need(sum<=33554432,"PREFLIGHT_PLAYER_SIZE_INVALID");
            string use=production?"Production":"ContractTest";var binding=Parse(player["Runtime.bindings.json"]);Need(Text(binding,"use")==use,"PREFLIGHT_OUTPUT_USE_MISMATCH");
            string br=RawHash(player["Runtime.bindings.json"]),rr=RawHash(release),rj=LevelHashing.HashProjection(Parse(release)),ur=RawHash(ui);
            var payload=player.Where(p=>p.Key!="Runtime.bindings.json").ToDictionary(p=>p.Key,p=>p.Value,StringComparer.Ordinal);
            var checkedSources=production?RuntimeGameSources.ReadProduction(payload,player["Runtime.bindings.json"],br,rr,rj,expectedApprovalRaw!,ur):RuntimeGameSources.ReadContractTest(payload,player["Runtime.bindings.json"],br,rr,rj,ur);
            var authoring=Parse(sourceBindings);Fields(authoring,"sourceBindingsVersion","use","releaseLockRawSha256","releaseLockJcsSha256","catalogSet","railwayAssetsRawSha256","sources","humanSourceBindings","puzzles");
            Need(Number(Get(authoring,"sourceBindingsVersion"))==1&&Text(authoring,"use")==use&&Hash(authoring,"releaseLockRawSha256")==rr&&Hash(authoring,"releaseLockJcsSha256")==rj,"PREFLIGHT_AUTHORING_BINDING_INVALID");
            Need(LevelHashing.HashProjection(Get(authoring,"catalogSet"))==LevelHashing.HashProjection(Get(binding,"catalogSet"))&&Hash(authoring,"railwayAssetsRawSha256")==RawHash(player["RailwayAssets.json"]),"PREFLIGHT_AUTHORING_BINDING_INVALID");
            var originals=new Dictionary<string,string>(StringComparer.Ordinal);string? previous=null;
            foreach(var row in Array(authoring,"sources")){Fields(row,"path","rawSha256");string path=Path(row,"path");Need(previous==null||StringComparer.Ordinal.Compare(previous,path)<0,"PREFLIGHT_SOURCE_ORDER_INVALID");previous=path;originals.Add(path,Hash(row,"rawSha256"));}
            Need(originals.Count>0&&originals.Count<=8192&&Array(authoring,"puzzles").Count==240,"PREFLIGHT_SOURCE_COUNT_INVALID");
            var releaseDocument=Parse(release);Fields(releaseDocument,"lockFormatVersion","releaseId","gitCommit","toolchainLockSha256","catalogHashes","puzzles");Need(Number(Get(releaseDocument,"lockFormatVersion"))==1,"PREFLIGHT_RELEASE_VERSION_INVALID");
            var actualCatalogs=new Dictionary<string,string>(StringComparer.Ordinal);
            foreach(var c in new[]{checkedSources.Catalog.Identity,checkedSources.Progress.Campaign.Identity,checkedSources.CampaignCompletion.Identity,checkedSources.EndlessCompletion.Identity,checkedSources.Calibration.Identity,checkedSources.Cosmetics.Identity})actualCatalogs.Add(c.CatalogId,c.HashSha256);
            foreach(var p in checkedSources.CalibratedProfiles)actualCatalogs.Add(p.Id,p.HashSha256);
            var releaseCatalogs=Array(releaseDocument,"catalogHashes");Need(releaseCatalogs.Count==10,"PREFLIGHT_RELEASE_OUTPUT_CATALOG_MISMATCH");var names=new HashSet<string>(StringComparer.Ordinal);
            foreach(var row in releaseCatalogs){Fields(row,"id","sha256");string id=Text(row,"id");Need(names.Add(id)&&actualCatalogs.TryGetValue(id,out var hash)&&hash==Hash(row,"sha256"),"PREFLIGHT_RELEASE_OUTPUT_CATALOG_MISMATCH");}
            var releasePuzzles=Array(releaseDocument,"puzzles");Need(releasePuzzles.Count==240,"PREFLIGHT_RELEASE_OUTPUT_PUZZLE_MISMATCH");var authoringPuzzles=Array(authoring,"puzzles");var seen=new HashSet<string>(StringComparer.Ordinal);
            foreach(var row in releasePuzzles)
            {
                string id=Text(row,"puzzleId");Need(seen.Add(id),"PREFLIGHT_RELEASE_OUTPUT_PUZZLE_MISMATCH");var puzzle=checkedSources.Catalog.Puzzles.SingleOrDefault(p=>p.Identity.PuzzleId==id);var original=authoringPuzzles.SingleOrDefault(p=>Text(p,"puzzleId")==id);Need(puzzle!=null&&original!=null,"PREFLIGHT_RELEASE_OUTPUT_PUZZLE_MISMATCH");
                Fields(original!,"puzzleId","targetDocumentSha256","publicPuzzleHash","proofHash");var semantic=Get(row,"publicPuzzleHash");Fields(semantic,"profile","sha256");
                Need(Text(semantic,"profile")==puzzle!.Identity.HashProfile&&Hash(semantic,"sha256")==puzzle.Identity.HashSha256&&Hash(original!,"targetDocumentSha256")==Hash(row,"documentSha256")&&LevelHashing.HashProjection(Get(original!,"publicPuzzleHash"))==LevelHashing.HashProjection(semantic)&&LevelHashing.HashProjection(Get(original!,"proofHash"))==LevelHashing.HashProjection(Get(row,"proofHash")),"PREFLIGHT_RELEASE_OUTPUT_PUZZLE_MISMATCH");
            }
            var output=Parse(rawLock);Fields(output,"schemaVersion","use","files");Need(Number(Get(output,"schemaVersion"))==1&&Text(output,"use")==use,"PREFLIGHT_RAW_LOCK_INVALID");var rows=Array(output,"files");Need(rows.Count==13,"PREFLIGHT_RAW_LOCK_INVALID");
            for(int i=0;i<13;i++){Fields(rows[i],"path","rawSha256");Need(Text(rows[i],"path")==PlayerNames[i]&&Hash(rows[i],"rawSha256")==RawHash(player[PlayerNames[i]]),"PREFLIGHT_RAW_LOCK_INVALID");}
            return new ProductionImportOutputFacts(br,rr,rj,expectedApprovalRaw,ur,originals);
        }
        private static JsonValue ReadRequest(byte[] bytes,string use)
        {var v=Parse(bytes);Fields(v,RequestFields);Need(Number(Get(v,"importRequestVersion"))==1&&Text(v,"use")==use,"PREFLIGHT_REQUEST_USE_INVALID");return v;}
        private static JsonValue Parse(byte[] bytes){var p=StrictJsonParser.Parse(bytes,new JsonParserLimits(16777216,64,1048576,65536,256));Need(p.Succeeded,"PREFLIGHT_JSON_INVALID");return p.Value!;}
        private static void Need(bool value,string code){if(!value)throw new InvalidOperationException(code);}
        private static void Fields(JsonValue v,params string[] names)=>Need(v.Kind==JsonKind.Object&&v.Members.Count==names.Length&&v.Members.All(m=>names.Contains(m.Name,StringComparer.Ordinal)),"PREFLIGHT_FIELDS_INVALID");
        private static JsonValue Get(JsonValue v,string key)=>v.Get(key)??throw new InvalidOperationException("PREFLIGHT_FIELDS_INVALID");
        private static string Text(JsonValue v,string key){var x=Get(v,key);Need(x.Kind==JsonKind.String&&!string.IsNullOrWhiteSpace(x.StringValue),"PREFLIGHT_STRING_INVALID");return x.StringValue;}
        private static string Path(JsonValue v,string key){string x=Text(v,key);RequireRelativePath(x);return x;}
        private static string Hash(JsonValue v,string key){string x=Text(v,key);RequireHash(x);return x;}
        private static long Number(JsonValue v){Need(v.Kind==JsonKind.Integer,"PREFLIGHT_INTEGER_INVALID");return v.IntegerValue;}
        private static IReadOnlyList<JsonValue> Array(JsonValue v,string key){var x=Get(v,key);Need(x.Kind==JsonKind.Array,"PREFLIGHT_ARRAY_INVALID");return x.Items;}
    }
}
