using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using STP.Application;
using STP.Editor.Content;
using STP.Infrastructure.Content;
using STP.Puzzle.Solver;

namespace STP.Tools.ProductionImport;

/// <summary>Authoring-only actual source pipeline. The old candidate projector and its null-threshold rule are untouched.</summary>
internal sealed class ProductionImporter
{
    internal Sources Sources{get;}
    internal ProductionImporter(string root){Sources=new Sources(root);}
    internal ImportResult Validate(string requestPath,string? approvalPath,string? trustedApprovalRawSha256)
    {
        var request=Sources.ReadJson(requestPath);
        J.Fields(request,"importRequestVersion","use","publicCatalog","campaign","campaignCompletion","endlessCompletion","calibration","cosmetics","profiles","levels","localizations","railwayAssets","releaseLock","toolchainLock","uiSourceLock","evidence","publishedLocks");
        J.Need(J.N(request,"importRequestVersion")==1,"IMPORT_REQUEST_VERSION");string useText=J.S(request,"use");J.Need(useText is "Production" or "ContractTest","IMPORT_USE_FORBIDDEN");bool fixture=useText=="ContractTest";var use=fixture?CatalogUse.ContractTest:CatalogUse.Production;string status=fixture?"FIXTURE_ONLY":"PRODUCT_APPROVED";var approval=fixture?CatalogApprovalStatus.FixtureOnly:CatalogApprovalStatus.ProductApproved;
        foreach(string list in new[]{"profiles","localizations","evidence"}){var a=J.A(request,list);J.Need(a.Count>0&&a.Count<=16&&a.Select(J.Text).Distinct(StringComparer.Ordinal).Count()==a.Count,"IMPORT_SOURCE_LIST_DUPLICATE_OR_LIMIT");}J.Need(J.A(request,"publishedLocks").Count<=256,"IMPORT_HISTORY_LIMIT");
        // The independent trust input is required before any production output or caller-supplied approval DTO can exist.
        if(!fixture)J.Need(!string.IsNullOrWhiteSpace(approvalPath)&&!string.IsNullOrWhiteSpace(trustedApprovalRawSha256),"IMPORT_TRUSTED_APPROVAL_SOURCE_REQUIRED");
        if(!fixture)ProvenanceGate.ReadTrustedApproval(Sources,approvalPath!,trustedApprovalRawSha256!);
        var lockBytes=Sources.Read(J.S(request,"releaseLock"));var release=new ReleaseLock(lockBytes,fixture);
        J.Need(J.Raw(Sources.Read(J.S(request,"toolchainLock")))==J.H(release.Document,"toolchainLockSha256"),"IMPORT_TOOLCHAIN_LOCK_MISMATCH");
        var rows=J.A(request,"levels");J.Need(rows.Count==240,"IMPORT_FULL_240_REQUIRED");var inputs=new List<LevelValidationInput>();
        foreach(var row in rows)
        {
            J.Fields(row,"level","proof");string path=J.S(row,"level");byte[] bytes=Sources.Read(path);var loaded=LevelV2Loader.Load(bytes);J.Need(loaded.IsValid,"IMPORT_LEVEL_INVALID:"+path+":"+string.Join(",",loaded.Diagnostics.Select(d=>d.Code)));
            J.Need(loaded.Document!.Production.StarThresholdsSeconds!=null,"IMPORT_LEVEL_NOT_CALIBRATED:"+loaded.Document.PuzzleId);
            inputs.Add(new LevelValidationInput(path,bytes,loaded.Document.ProofRef.ArtifactId,Sources.Read(J.S(row,"proof"))));
        }
        var validation=LevelValidationPipeline.ValidateBatch(inputs,true,new SolverLimits(1000000,60000));
        J.Need(validation.Succeeded,"IMPORT_FRESH_LEVEL_PROOF_FAILED:"+string.Join(";",validation.Results.Where(r=>!r.Succeeded).Select(r=>r.SourcePath+":"+string.Join(",",r.Diagnostics.Select(d=>d.Code)))));
        var levels=validation.Results.OrderBy(r=>r.Level.Document!.PuzzleId,StringComparer.Ordinal).ToArray();J.Need(release.Puzzles.Count==240,"IMPORT_RELEASE_PUZZLE_SCOPE");
        for(int i=0;i<240;i++){var r=levels[i];string id=$"S1-{i/48+1:D2}-{i%48/12+1:D2}-{i%12+1:D2}";J.Need(r.Level.Document!.PuzzleId==id,"IMPORT_LEVEL_MEMBERSHIP");release.CheckPuzzle(r);}
        var publicRequest=J.Get(request,"publicCatalog");J.Fields(publicRequest,"catalogId","revision");
        var publicPayload=J.Obj(("schemaVersion",J.Num(1)),("catalogId",J.Str(J.S(publicRequest,"catalogId"))),("revision",J.Num(J.N(publicRequest,"revision"))),("use",J.Str(useText)),("status",J.Str(status)),("puzzles",J.Arr(levels.Select(r=>J.Put(LevelHashing.PublicPuzzleProjection(r.Level.Document!),"publicPuzzleHash",r.Level.PublicPuzzleHash!.ToJson())))));
        string publicHash=J.Hash(publicPayload);publicPayload=J.Put(publicPayload,"catalogHashSha256",J.Str(publicHash));
        CatalogIdentity Id(JsonValue doc,bool publicDoc=false)=>new(J.S(doc,"catalogId"),checked((int)J.N(doc,publicDoc?"revision":"catalogRevision")),publicDoc?publicHash:J.Hash(doc),use,approval);
        var corpus=new RuntimePublicPuzzleCatalog(J.Bytes(publicPayload),Id(publicPayload,true));
        var calibrationDoc=Sources.ReadJson(J.S(request,"calibration"));var calibration=new StarCalibrationCatalog(J.Bytes(calibrationDoc),corpus,Id(calibrationDoc),new CanonicalJsonAdapter());
        foreach(var r in levels){var d=r.Level.Document!;var times=calibration.GetCampaign(corpus.Puzzles.Single(p=>p.Identity.PuzzleId==d.PuzzleId).Identity);var t=d.Production.StarThresholdsSeconds!;J.Need(times.SourceDocumentSha256==r.Level.ContentHash&&times.Times.CalibrationVersion==t.CalibrationVersion&&times.Times.TwoStarsSeconds==t.TwoStars&&times.Times.ThreeStarsSeconds==t.ThreeStars,"IMPORT_LEVEL_CALIBRATION_BINDING:"+d.PuzzleId);}
        var campaignWrapper=Sources.ReadJson(J.S(request,"campaignCompletion"));var endlessWrapper=Sources.ReadJson(J.S(request,"endlessCompletion"));
        J.Need(J.Hash(J.Get(campaignWrapper,"calibrationCatalog"))==J.Hash(calibrationDoc)&&J.Hash(J.Get(endlessWrapper,"calibrationCatalog"))==J.Hash(calibrationDoc),"IMPORT_WRAPPER_CALIBRATION_DIFFERENT");
        CatalogIdentity WrapperId(JsonValue v)=>new(J.S(J.Get(v,"completionCatalog"),"catalogId"),checked((int)J.N(J.Get(v,"completionCatalog"),"catalogRevision")),J.Hash(v),use,approval);
        var json=new CanonicalJsonAdapter();var campaignCompletion=new RuntimeCompletionCatalog(J.Bytes(campaignWrapper),corpus,WrapperId(campaignWrapper),json);var endlessCompletion=new RuntimeCompletionCatalog(J.Bytes(endlessWrapper),corpus,WrapperId(endlessWrapper),json);
        J.Need(campaignCompletion.SubjectKind=="LEVEL"&&endlessCompletion.SubjectKind=="ENDLESS_INSTANCE"&&!campaignCompletion.Identity.Equals(endlessCompletion.Identity),"IMPORT_COMPLETION_SUBJECT_CONFUSION");
        var campaign=Sources.ReadJson(J.S(request,"campaign"));var progress=new RuntimeProgressCatalog(corpus,J.Bytes(campaign),Id(campaign),campaignCompletion,json);
        var cosmeticsDoc=Sources.ReadJson(J.S(request,"cosmetics"));var cosmetics=new LockedCosmeticsCatalog(J.Bytes(cosmeticsDoc),J.Hash(cosmeticsDoc),use,json);J.Need(cosmetics.Identity.ApprovalStatus==approval,"IMPORT_COSMETICS_APPROVAL");
        var profiles=J.A(request,"profiles").Select(p=>Sources.ReadJson(J.Text(p))).ToArray();J.Need(profiles.Length==4,"IMPORT_FOUR_QUALITY_PROFILES_REQUIRED");var profileIds=new HashSet<string>(StringComparer.Ordinal);var lockedProfiles=new List<LockedCalibratedEndlessProfile>();
        foreach(var profile in profiles){var p=new LockedCalibratedEndlessProfile(J.Bytes(profile),corpus,endlessCompletion,J.Hash(profile));J.Need(profileIds.Add(p.Profile.Id),"IMPORT_DUPLICATE_PROFILE");lockedProfiles.Add(p);}J.Need(profileIds.SetEquals(new[]{"dienstbeginn","regelbetrieb","stoerungslage","grosslage"}),"IMPORT_PROFILE_MEMBERSHIP");
        var texts=new TextSources(Sources,J.A(request,"localizations"),status);using var assets=new RailwaySources(Sources.Read(J.S(request,"railwayAssets")),status);CrossReferences.Validate(levels,campaign,campaignWrapper,endlessWrapper,cosmetics,texts,assets);
        var catalogHashes=new Dictionary<string,string>(StringComparer.Ordinal);void Add(string id,string hash){J.Need(catalogHashes.TryAdd(id,hash),"IMPORT_CATALOG_ID_COLLISION");}
        foreach(var identity in new[]{corpus.Identity,progress.Campaign.Identity,campaignCompletion.Identity,endlessCompletion.Identity,cosmetics.Identity,calibration.Identity})Add(identity.CatalogId,identity.HashSha256);
        foreach(var p in lockedProfiles)Add(p.Profile.Id,p.Profile.HashSha256);release.CheckCatalogs(catalogHashes);
        var currentMetadata=new[]{publicPayload,campaign,campaignWrapper,endlessWrapper,cosmeticsDoc,calibrationDoc}.Concat(profiles).Select(ReleaseLock.CatalogMetadata).ToArray();
        foreach(var history in J.A(request,"publishedLocks"))
        {
            J.Fields(history,"lock","catalogs");var old=new ReleaseLock(Sources.Reference(J.Get(history,"lock")),fixture);release.CheckHistory(old);var prior=J.A(history,"catalogs").Select(reference=>ReleaseLock.CatalogMetadata(J.Parse(Sources.Reference(reference)))).ToArray();
            J.Need(prior.Length==old.Catalogs.Count&&prior.Select(p=>p.Id).Distinct().Count()==prior.Length&&prior.All(p=>old.Catalogs.TryGetValue(p.Id,out var h)&&h==p.Hash),"IMPORT_HISTORICAL_CATALOG_SOURCE_BINDING");foreach(var p in prior)foreach(var current in currentMetadata)ReleaseLock.CheckCatalogRevision(current,p);
        }
        var evidence=J.A(request,"evidence");
        var textOrigins=levels.SelectMany(r=>new[]{r.Level.Document!.Content.TitleKey,r.Level.Document.Content.StatusKey,r.Level.Document.Completion.ResultTextKey}).Select(k=>texts.PlayerOrigin[k]).Distinct(StringComparer.Ordinal).ToArray();J.Need(textOrigins.Length==1,"IMPORT_CAMPAIGN_TEXT_MULTIPLE_ORIGINS");
        string targetTextSourceRawHash=J.Raw(Sources.Read(textOrigins[0]));
        // This validator reads the accepted provenance files themselves; a caller bool or constructed catalog cannot grant it.
        ProvenanceGate.Validate(Sources,request,levels,profiles,calibration,corpus,release,evidence,fixture,approvalPath,trustedApprovalRawSha256,targetTextSourceRawHash);
        var campaignText=J.Obj(("schemaVersion",J.Num(1)),("locale",J.Str("de-DE")),("approvalStatus",J.Str(status)),("sourceSha256",J.Str(J.Raw(Sources.Read(textOrigins[0])))),("entries",J.Arr(levels.Select(r=>{var d=r.Level.Document!;return J.Obj(("puzzleId",J.Str(d.PuzzleId)),("title",J.Str(texts.Player[d.Content.TitleKey])),("status",J.Str(texts.Player[d.Content.StatusKey])),("result",J.Str(texts.Player[d.Completion.ResultTextKey])));}))));
        foreach(var entry in J.A(campaignText,"entries"))foreach(string key in new[]{"title","status","result"})J.Need(J.S(entry,key).IndexOfAny(new[]{'{','}'})<0,"IMPORT_CAMPAIGN_TEXT_NOT_PLAIN");
        if(!fixture)
        {
            const string ui="Assets/StammstreckenPuzzle/UI/";
            J.Need(Sources.Read(J.S(request,"railwayAssets")).SequenceEqual(Sources.Read(ui+"RailwayAssets.json")),"IMPORT_RENDER_MANIFEST_OUTSIDE_TARGET_UI_LOCK");
            J.Need(J.Hash(Sources.ReadJson(ui+"ProductionCampaignText.json"))==J.Hash(campaignText),"IMPORT_CAMPAIGN_TEXT_OUTSIDE_TARGET_UI_LOCK");
        }
        var outputs=new SortedDictionary<string,byte[]>(StringComparer.Ordinal){["Public.json"]=J.Bytes(publicPayload),["Campaign.json"]=J.Bytes(campaign),["CampaignCompletion.json"]=J.Bytes(campaignWrapper),["EndlessCompletion.json"]=J.Bytes(endlessWrapper),["Calibration.json"]=J.Bytes(calibrationDoc),["Cosmetics.json"]=J.Bytes(cosmeticsDoc),["RailwayAssets.json"]=Sources.Read(J.S(request,"railwayAssets")),["CampaignText.json"]=J.Bytes(campaignText)};
        foreach(var profile in profiles)outputs.Add("Profiles/"+J.S(profile,"profileId")+".json",J.Bytes(profile));
        JsonValue Ref(CatalogIdentity c)=>J.Parse(System.Text.Encoding.UTF8.GetBytes(LocalSaveHash.CatalogProjection(c)));
        var bindings=J.Obj(
            ("runtimeImportVersion",J.Num(1)),
            ("use",J.Str(useText)),
            ("status",J.Str(status)),
            ("releaseLockRawSha256",J.Str(release.RawHash)),
            ("releaseLockJcsSha256",J.Str(release.JcsHash)),
            ("approvalLockRawSha256",fixture?JsonValue.Null:J.Str(trustedApprovalRawSha256!)),
            ("publicCatalog",Ref(corpus.Identity)),
            ("campaignCatalog",Ref(progress.Campaign.Identity)),
            ("campaignCompletionCatalog",Ref(campaignCompletion.Identity)),
            ("endlessCompletionCatalog",Ref(endlessCompletion.Identity)),
            ("calibrationCatalog",Ref(calibration.Identity)),
            ("cosmeticsCatalog",Ref(cosmetics.Identity)),
            ("qualityProfiles",J.Arr(lockedProfiles.OrderBy(p=>p.Profile.Id,StringComparer.Ordinal).Select(p=>J.Obj(("profileId",J.Str(p.Profile.Id)),("hashSha256",J.Str(p.Profile.HashSha256)))))),
            ("catalogSet",Ref(progress.CatalogSet.AggregateCatalog)),
            ("railwayAssetsRawSha256",J.Str(assets.RawHash)),
            ("campaignTextRawSha256",J.Str(J.Raw(outputs["CampaignText.json"]))),
            ("uiSourceLockRawSha256",J.Str(J.Raw(Sources.Read(J.S(request,"uiSourceLock"))))),
            ("playerFiles",J.Arr(outputs.Select(p=>J.Obj(("path",J.Str(p.Key)),("rawSha256",J.Str(J.Raw(p.Value))))))));
        outputs.Add("Runtime.bindings.json",J.Bytes(bindings));
        var sourceBindings=J.Obj(("sourceBindingsVersion",J.Num(1)),("use",J.Str(useText)),("releaseLockRawSha256",J.Str(release.RawHash)),("releaseLockJcsSha256",J.Str(release.JcsHash)),("catalogSet",J.Parse(System.Text.Encoding.UTF8.GetBytes(LocalSaveHash.CatalogProjection(progress.CatalogSet.AggregateCatalog)))),("railwayAssetsRawSha256",J.Str(assets.RawHash)),("sources",Sources.Lock()),("humanSourceBindings",J.Arr(Sources.HumanSourceBindings)),("puzzles",J.Arr(levels.Select(r=>J.Obj(("puzzleId",J.Str(r.Level.Document!.PuzzleId)),("targetDocumentSha256",J.Str(r.Level.ContentHash!)),("publicPuzzleHash",r.Level.PublicPuzzleHash!.ToJson()),("proofHash",r.Proof!.Document!.ProofHash.ToJson()))))));
        Sources.VerifyUnchanged();
        return new ImportResult(useText,outputs,sourceBindings);
    }
}

internal sealed class ImportResult
{
    internal string Use{get;}internal IReadOnlyDictionary<string,byte[]> Player{get;}internal JsonValue SourceBindings{get;}
    internal ImportResult(string use,IReadOnlyDictionary<string,byte[]> player,JsonValue sourceBindings){Use=use;Player=player;SourceBindings=sourceBindings;}
    internal void Write(string output)
    {
        output=Path.GetFullPath(output);Sources.CheckNoLinks(output);J.Need(!Directory.Exists(output)&&!File.Exists(output),"IMPORT_OUTPUT_ALREADY_EXISTS");
        Directory.CreateDirectory(output);foreach(var file in Player){string path=Path.Combine(output,"player",file.Key.Replace('/',Path.DirectorySeparatorChar));Directory.CreateDirectory(Path.GetDirectoryName(path)!);File.WriteAllBytes(path,file.Value);}
        Directory.CreateDirectory(Path.Combine(output,"authoring"));File.WriteAllBytes(Path.Combine(output,"authoring","source-bindings.json"),J.Bytes(SourceBindings));
        File.WriteAllBytes(Path.Combine(output,"authoring","player-raw-lock.json"),J.Bytes(J.Obj(("schemaVersion",J.Num(1)),("use",J.Str(Use)),("files",J.Arr(Player.Select(p=>J.Obj(("path",J.Str(p.Key)),("rawSha256",J.Str(J.Raw(p.Value))))))))));
    }
}
