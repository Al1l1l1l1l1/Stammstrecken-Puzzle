using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using STP.Infrastructure.Content;

namespace STP.Tools.ProductionImport;

/// <summary>Existing release-lock-v1; no alternative projection and no status field is invented.</summary>
internal sealed class ReleaseLock
{
    internal JsonValue Document{get;}
    internal string RawHash{get;}
    internal string JcsHash=>J.Hash(Document);
    internal IReadOnlyDictionary<string,string> Catalogs{get;}
    internal IReadOnlyDictionary<string,JsonValue> Puzzles{get;}
    internal ReleaseLock(byte[] bytes,bool fixture)
    {
        RawHash=J.Raw(bytes);Document=J.Parse(bytes);var v=Document;
        J.Fields(v,"lockFormatVersion","releaseId","gitCommit","toolchainLockSha256","catalogHashes","puzzles");
        J.Need(J.N(v,"lockFormatVersion")==1&&Regex.IsMatch(J.S(v,"gitCommit"),"^[0-9a-f]{40}$")&&Regex.IsMatch(J.S(v,"releaseId"),fixture?"^fixture-[a-z0-9.-]+$":"^v[0-9]+\\.[0-9]+\\.[0-9]+(?:-rc\\.[1-9][0-9]*)?$"),"IMPORT_RELEASE_LOCK_HEADER");J.H(v,"toolchainLockSha256");
        var catalogs=new Dictionary<string,string>(StringComparer.Ordinal);foreach(var c in J.A(v,"catalogHashes")){J.Fields(c,"id","sha256");J.Need(catalogs.TryAdd(J.S(c,"id"),J.H(c,"sha256")),"IMPORT_LOCK_DUPLICATE_CATALOG");}J.Need(catalogs.Count>0,"IMPORT_LOCK_EMPTY_CATALOGS");Catalogs=catalogs;
        var puzzles=new Dictionary<string,JsonValue>(StringComparer.Ordinal);foreach(var p in J.A(v,"puzzles"))
        {
            J.Fields(p,"puzzleId","documentSchemaVersion","documentSha256","publicPuzzleHash","solutionHash","proofFormatVersion","solverVersion","proofHash","legacyBindings");
            J.Need(Regex.IsMatch(J.S(p,"puzzleId"),"^S[1-9][0-9]*-[0-9]{2}-[0-9]{2}-[0-9]{2}$")&&J.N(p,"documentSchemaVersion")>0&&J.N(p,"proofFormatVersion")>0,"IMPORT_LOCK_PUZZLE_FORMAT");J.H(p,"documentSha256");J.S(p,"solverVersion");
            foreach(string key in new[]{"publicPuzzleHash","solutionHash","proofHash"})Profiled(J.Get(p,key));var legacy=J.A(p,"legacyBindings");foreach(var h in legacy)Profiled(h);J.Need(legacy.Select(J.Hash).Distinct().Count()==legacy.Count&&puzzles.TryAdd(J.S(p,"puzzleId"),p),"IMPORT_LOCK_DUPLICATE_PUZZLE_OR_LEGACY");
        }
        J.Need(puzzles.Count>0,"IMPORT_LOCK_EMPTY_PUZZLES");Puzzles=puzzles;
    }
    private static void Profiled(JsonValue v){J.Fields(v,"profile","sha256");J.S(v,"profile");J.H(v,"sha256");}
    internal void CheckCatalogs(IReadOnlyDictionary<string,string> actual)=>J.Need(Catalogs.Count==actual.Count&&actual.All(x=>Catalogs.TryGetValue(x.Key,out var h)&&h==x.Value),"IMPORT_LOCK_CATALOG_SET_MISMATCH");
    internal void CheckPuzzle(LevelValidationResult r)
    {
        var d=r.Level.Document!;J.Need(Puzzles.TryGetValue(d.PuzzleId,out var p),"IMPORT_LOCK_PUZZLE_MISSING");var proof=r.Proof!.Document!;
        J.Need(J.N(p!,"documentSchemaVersion")==d.DocumentSchemaVersion&&J.H(p!,"documentSha256")==r.Level.ContentHash&&J.Hash(J.Get(p!,"publicPuzzleHash"))==J.Hash(r.Level.PublicPuzzleHash!.ToJson())&&J.Hash(J.Get(p!,"solutionHash"))==J.Hash(r.Level.SolutionHash!.ToJson())&&J.N(p!,"proofFormatVersion")==proof.ProofFormatVersion&&J.S(p!,"solverVersion")==proof.SolverVersion&&J.Hash(J.Get(p!,"proofHash"))==J.Hash(proof.ProofHash.ToJson()),"IMPORT_LOCK_PUZZLE_BINDING:"+d.PuzzleId);
    }
    internal void CheckHistory(ReleaseLock old)
    {
        J.Need(J.S(Document,"releaseId")!=J.S(old.Document,"releaseId")||JcsHash==old.JcsHash,"IMPORT_LOCK_RELEASE_ID_REUSED");
        foreach(var p in old.Puzzles)if(Puzzles.TryGetValue(p.Key,out var next))J.Need(J.Hash(J.Get(p.Value,"publicPuzzleHash"))==J.Hash(J.Get(next,"publicPuzzleHash")),"LOCK_PUBLISHED_PUZZLE_MUTATED:"+p.Key);
    }
    internal static (string Id,long Revision,string Hash) CatalogMetadata(JsonValue v)
    {
        if(v.Get("runtimeCompletionContractVersion")!=null){var c=J.Get(v,"completionCatalog");return(J.S(c,"catalogId"),J.N(c,"catalogRevision"),J.Hash(v));}
        if(v.Get("profileId")!=null)return(J.S(v,"profileId"),J.N(v,"profileVersion"),J.Hash(v));
        if(v.Get("catalogHashSha256")!=null){string hash=J.Hash(JsonValue.CreateObject(v.Members.Where(m=>m.Name!="catalogHashSha256")));J.Need(hash==J.H(v,"catalogHashSha256"),"IMPORT_HISTORICAL_PUBLIC_SELFHASH");return(J.S(v,"catalogId"),J.N(v,"revision"),hash);}
        return(J.S(v,"catalogId"),J.N(v,"catalogRevision"),J.Hash(v));
    }
    internal static void CheckCatalogRevision((string Id,long Revision,string Hash) current,(string Id,long Revision,string Hash) prior)
    {
        if(current.Id!=prior.Id)return;J.Need(current.Revision>=prior.Revision,"CAT-REVISION-REGRESSED");J.Need(current.Revision!=prior.Revision||current.Hash==prior.Hash,"CAT-REVISION-HASH-CONFLICT");
    }
}
