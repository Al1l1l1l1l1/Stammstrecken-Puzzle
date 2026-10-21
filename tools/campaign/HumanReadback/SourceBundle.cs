using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using STP.Application;
using STP.Infrastructure.Content;
using STP.Infrastructure.Persistence;

namespace STP.Tools.HumanReadback;

// Authoring-only input verification over the existing strict parser, JCS and Human readers.
internal sealed class SourceBundle
{
    internal readonly ICanonicalJson Json=new CanonicalJsonAdapter();
    internal readonly Dictionary<string,HumanAssignmentCatalog> Catalogs=new(StringComparer.Ordinal);
    internal readonly SortedDictionary<string,byte[]> Provenance=new(StringComparer.Ordinal);
    internal readonly string Root;
    internal readonly MobileCandidateData Public;
    internal SourceBundle(string root,string packs,string publicFile,string bindingsFile,string sourceBindings)
    {
        Root=Path.GetFullPath(root);packs=Path.GetFullPath(packs);
        byte[] publicBytes=Read(publicFile),bindingBytes=Read(bindingsFile),rawBytes=Read(Path.Combine(packs,"raw-sha256-lock.json")),uiBytes=Read(Path.Combine(packs,"ui-source-lock.json")),sourceBytes=Read(sourceBindings);
        var bindings=Parse(bindingBytes);Fields(bindings,"schemaVersion","catalogNamespace","use","approvalStatus","releaseEligible","publicCatalogHashSha256","campaignHashSha256","completionHashSha256","cosmeticsHashSha256");Need(N(bindings,"schemaVersion")==1&&S(bindings,"catalogNamespace")=="qa-mobile-candidate"&&S(bindings,"use")=="InternalPreview"&&S(bindings,"approvalStatus")=="DRAFT"&&Get(bindings,"releaseEligible").Kind==JsonKind.Boolean&&!Get(bindings,"releaseEligible").BooleanValue,"READBACK_PUBLIC_BINDINGS");
        Public=new MobileCandidateData(publicBytes,S(bindings,"publicCatalogHashSha256"));
        var raw=Parse(rawBytes);Fields(raw,"schemaVersion","kind","uiSourceSha256","defaultManifest","manifests");Need(N(raw,"schemaVersion")==1&&S(raw,"kind")=="HUMAN_ASSIGNMENT_RAW_LOCK"&&S(raw,"uiSourceSha256")==Hash(uiBytes),"READBACK_RAW_LOCK");
        var ui=Parse(uiBytes);Fields(ui,"schemaVersion","kind","files");Need(N(ui,"schemaVersion")==1&&S(ui,"kind")=="HUMAN_UI_SOURCE_LOCK","READBACK_UI_LOCK");
        var uiPaths=new List<string>();foreach(var file in A(ui,"files")){Fields(file,"path","sha256");string path=S(file,"path");uiPaths.Add(path);Need(Hash(Read(Inside(path)))==S(file,"sha256"),"READBACK_UI_SOURCE_CHANGED:"+path);}
        string prefix="Assets/StammstreckenPuzzle/";var expected=Directory.GetFiles(Inside(prefix+"Scripts/Presentation"),"*.cs",SearchOption.AllDirectories).Concat(Directory.GetFiles(Inside(prefix+"Art"),"*",SearchOption.AllDirectories).Where(p=>new[]{".png",".svg",".ttf",".otf"}.Contains(Path.GetExtension(p).ToLowerInvariant()))).Select(p=>Path.GetRelativePath(Root,p).Replace('\\','/')).Concat(new[]{"ProductionGame.uxml","ProductionGame.uss","ProductionGameLocale.json","ProductionCampaignText.json","RailwayAssets.json"}.Select(p=>prefix+"UI/"+p)).OrderBy(p=>p,StringComparer.Ordinal).ToArray();
        Need(uiPaths.SequenceEqual(expected),"READBACK_UI_FILESET");
        var sources=Parse(sourceBytes);Fields(sources,"schemaVersion","status","humanStatus","buildId","sourceUiOverlay","uiSourceSha256","campaignPublicRawSha256","campaignQualityRawSha256","endlessSampleRawSha256","packs");Need(N(sources,"schemaVersion")==1&&S(sources,"status")=="DRAFT_UNISSUED_ASSIGNED_INPUTS"&&S(sources,"humanStatus")=="NOT_EXECUTED"&&S(sources,"uiSourceSha256")==Hash(uiBytes)&&S(sources,"campaignPublicRawSha256")==Hash(publicBytes),"READBACK_PRODUCER_SOURCE_BINDING");
        Need(S(sources,"campaignQualityRawSha256")==Hash(Read(Inside("Content/Quality/s1/difficulty-and-human-sample.json"))),"READBACK_CAMPAIGN_QUALITY_SOURCE");
        var names=new HashSet<string>(StringComparer.Ordinal);var producerPacks=A(sources,"packs");var manifests=A(raw,"manifests");Need(manifests.Count>=9&&manifests.Count<=17&&producerPacks.Count==manifests.Count,"READBACK_PACK_COUNT");
        foreach(var entry in manifests)
        {
            Fields(entry,"path","sha256","studyId","assignmentCount");string name=S(entry,"path");Need(Regex.IsMatch(name,@"^(campaign-pack-0[1-9]|endless-pack-0[1-8])\.json$")&&names.Add(name)&&N(entry,"assignmentCount")==6,"READBACK_PACK_ID");
            var bytes=Read(Path.Combine(packs,name));var catalog=new HumanAssignmentCatalog(bytes,S(entry,"sha256"),Public,Json);Need(catalog.StudyId==S(entry,"studyId")&&catalog.BuildId==S(sources,"buildId")&&catalog.UiSourceSha256==Hash(uiBytes)&&catalog.Assignments.Count==6,"READBACK_PACK_BINDING");Need(Catalogs.TryAdd(catalog.ManifestBytesSha256,catalog),"READBACK_MANIFEST_ALIAS");
            var matches=producerPacks.Where(p=>S(p,"manifest")==name).ToArray();Need(matches.Length==1,"READBACK_SOURCE_PACK");Fields(matches[0],"manifest","sources");var sourceRows=A(matches[0],"sources");Need(sourceRows.Count==6,"READBACK_SOURCE_COUNT");
            foreach(var assignment in catalog.Assignments)
            {
                var matched=sourceRows.Where(p=>S(p,"assignmentId")==assignment.Id).ToArray();Need(matched.Length==1,"READBACK_SOURCE_ASSIGNMENT");var source=matched[0];Need(S(source,"puzzleId")==assignment.Puzzle.Identity.PuzzleId&&S(source,"sourceSha256")==assignment.SourceSha256,"READBACK_SOURCE_IDENTITY");string relative;
                if(assignment.Purpose==HumanAssignmentPurpose.CAMPAIGN_TIME)
                {
                    Fields(source,"assignmentId","puzzleId","sourcePath","sourceSha256");var id=assignment.Puzzle.Identity.PuzzleId.Split('-');relative=$"Content/Levels/s1/{id[1]}/{id[2]}/{assignment.Puzzle.Identity.PuzzleId}.level-v2.json";Need(S(source,"sourcePath")==relative,"READBACK_CAMPAIGN_SOURCE_PATH");var bytesSource=Read(Inside(relative));Need(Hash(bytesSource)==assignment.SourceSha256,"READBACK_AUTHORING_BYTES");var level=Parse(bytesSource);Need(N(level,"contentRevision")==assignment.ContentRevision,"READBACK_AUTHORING_REVISION");var semantic=JsonValue.CreateObject(level.Members.Where(m=>new[]{"puzzleId","rulesetVersion","grid","endpoints","rowCounts","columnCounts"}.Contains(m.Name)));Need(LevelHashing.HashProjection(semantic)==assignment.Puzzle.Identity.HashSha256,"READBACK_AUTHORING_PUBLIC");
                }
                else
                {
                    Fields(source,"assignmentId","puzzleId","sourcePath","sourceSha256","descriptorSha256","parametersSha256");var original=S(source,"sourcePath").Replace('\\','/');var match=Regex.Match(original,@"(?:^|/)(Content/Quality/endless/[^/]+/"+Regex.Escape(assignment.ProfileId!)+"/"+Regex.Escape(assignment.Puzzle.Identity.PuzzleId)+@"/source-audit\.internal\.json)$");Need(match.Success,"READBACK_ENDLESS_SOURCE_PATH");relative=match.Groups[1].Value;var auditBytes=Read(Inside(relative));Need(Hash(auditBytes)==assignment.SourceSha256,"READBACK_ENDLESS_AUDIT_BYTES");var audit=Parse(auditBytes);Need(S(audit,"puzzleId")==assignment.Puzzle.Identity.PuzzleId&&S(audit,"profileId")==assignment.ProfileId&&S(audit,"profileHashSha256")==assignment.ProfileHashSha256&&S(audit,"publicSourceSha256")==assignment.PublicSourceSha256,"READBACK_ENDLESS_AUDIT_BINDING");
                    string folder=Path.GetDirectoryName(Inside(relative))!;var publicSource=Read(Path.Combine(folder,"public.json"));Need(Hash(publicSource)==assignment.PublicSourceSha256,"READBACK_ENDLESS_PUBLIC_BYTES");var descriptorBytes=Read(Path.Combine(folder,"descriptor.internal.json"));var parameterBytes=Read(Path.Combine(folder,"parameters.json"));Need(Hash(descriptorBytes)==S(source,"descriptorSha256")&&Hash(parameterBytes)==S(source,"parametersSha256"),"READBACK_ENDLESS_DESCRIPTOR_BYTES");
                    var descriptor=Parse(descriptorBytes);Fields(descriptor,"endlessContractVersion","rulesetVersion","generatorVersion","seed","generationOrdinal","parameterHashSha256");var parameters=Parse(parameterBytes);Need(N(descriptor,"endlessContractVersion")==1&&S(descriptor,"rulesetVersion")==assignment.Puzzle.RulesetVersion&&S(descriptor,"generatorVersion")==PuzzleCandidateSearch.GeneratorVersion&&"E1-"+LevelHashing.HashProjection(descriptor)==assignment.Puzzle.Identity.PuzzleId&&S(descriptor,"parameterHashSha256")==LevelHashing.HashProjection(parameters)&&S(parameters,"profileId")==assignment.ProfileId&&S(parameters,"profileHashSha256")==assignment.ProfileHashSha256&&S(Get(parameters,"campaignCatalog"),"hashSha256")==Public.Identity.HashSha256,"READBACK_ENDLESS_DESCRIPTOR_BINDING");
                    string pool=Directory.GetParent(Directory.GetParent(folder)!.FullName)!.FullName;Need(Hash(Read(Path.Combine(pool,"sample-audit.internal.json")))==S(sources,"endlessSampleRawSha256"),"READBACK_ENDLESS_SAMPLE_BYTES");
                    var profiles=new LockedEndlessProfiles(Read(Inside(prefix+"UI/MobileCandidate.endless-profiles.json")),Public);Need(profiles.Get(assignment.ProfileId!).HashSha256==assignment.ProfileHashSha256,"READBACK_ENDLESS_PROFILE_BINDING");
                }
            }
            Add("manifest-"+name,bytes);
        }
        Need(names.Contains(S(raw,"defaultManifest")),"READBACK_DEFAULT_MANIFEST");
        Add("raw-lock",rawBytes);Add("ui-source-lock",uiBytes);Add("public-catalog",publicBytes);Add("public-bindings",bindingBytes);Add("producer-source-bindings",sourceBytes);
    }
    internal string Inside(string relative){Need(!Path.IsPathRooted(relative)&&!relative.Contains('\\')&&!relative.Split('/').Any(p=>p is "" or "." or ".."),"READBACK_SOURCE_PATH");return Path.Combine(Root,relative.Replace('/',Path.DirectorySeparatorChar));}
    internal static byte[] Read(string path){CheckPath(path);var f=new FileInfo(path);Need(f.Exists&&f.Length<=8388608,"READBACK_FILE_SIZE_OR_MISSING:"+path);return File.ReadAllBytes(path);}
    internal static void CheckPath(string path){for(string? node=Path.GetFullPath(path);node!=null;node=Path.GetDirectoryName(node))if((File.Exists(node)||Directory.Exists(node))&&(File.GetAttributes(node)&FileAttributes.ReparsePoint)!=0)throw new ArgumentException("READBACK_REPARSE_REJECTED");}
    internal void Add(string name,byte[] bytes)=>Provenance.Add(name,bytes);
    internal static string Hash(byte[] b)=>LocalSaveHash.Bytes(b);
    internal static JsonValue Parse(byte[] b){var p=StrictJsonParser.Parse(b,new JsonParserLimits(8388608,64,65536,1048576,256));Need(p.Succeeded,"READBACK_JSON:"+p.Error?.Code);return p.Value!;}
    internal static JsonValue Get(JsonValue v,string key)=>v.Get(key)??throw new ArgumentException("READBACK_MISSING_FIELD:"+key);
    internal static string S(JsonValue v,string key){var x=Get(v,key);Need(x.Kind==JsonKind.String,"READBACK_STRING:"+key);return x.StringValue;}
    internal static long N(JsonValue v,string key){var x=Get(v,key);Need(x.Kind==JsonKind.Integer,"READBACK_INTEGER:"+key);return x.IntegerValue;}
    internal static IReadOnlyList<JsonValue> A(JsonValue v,string key){var x=Get(v,key);Need(x.Kind==JsonKind.Array,"READBACK_ARRAY:"+key);return x.Items;}
    internal static void Fields(JsonValue v,params string[] names)=>Need(v.Kind==JsonKind.Object&&v.Members.Count==names.Length&&v.Members.All(m=>names.Contains(m.Name)),"READBACK_FIELDS");
    internal static void Need(bool ok,string code){if(!ok)throw new ArgumentException(code);}
}
