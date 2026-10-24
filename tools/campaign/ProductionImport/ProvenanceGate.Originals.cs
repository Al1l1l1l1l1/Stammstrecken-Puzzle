using System;
using System.Collections.Generic;
using System.Linq;
using STP.Application;
using STP.Infrastructure.Content;
using STP.Tools.HumanReadback;

namespace STP.Tools.ProductionImport;

internal static partial class ProvenanceGate
{
    private static OriginalSet[] ReadOriginalSets(Sources sources,JsonValue evidence,string uiHash)
    {
        var result=new List<OriginalSet>();var ids=new HashSet<string>(StringComparer.Ordinal);
        foreach(var v in J.A(evidence,"observations"))
        {
            J.Fields(v,"id","sourceRoot","packs","publicCatalog","publicBindings","sourceBindings","exports","includedExposureIds");var set=new OriginalSet{Id=J.S(v,"id"),Prefix=J.S(v,"sourceRoot"),Packs=J.S(v,"packs")};J.Need(ids.Add(set.Id),"IMPORT_OBSERVATION_SET_DUPLICATE");
            string Relative(string p)=>set.Prefix+"/"+p;string Full(string p)=>sources.Inside(Relative(p));
            // Capture all bounded originals before the existing readers revisit them. VerifyUnchanged at the final
            // gate forbids readback against sources that changed between this snapshot and validation.
            TrackBundle(sources,set.Prefix,J.S(v,"packs"),J.S(v,"publicCatalog"),J.S(v,"publicBindings"),J.S(v,"sourceBindings"));
            var bundle=new SourceBundle(sources.Inside(set.Prefix),Full(J.S(v,"packs")),Full(J.S(v,"publicCatalog")),Full(J.S(v,"publicBindings")),Full(J.S(v,"sourceBindings")));set.Bundle=bundle;set.SourceBindings=J.Parse(sources.Read(Relative(J.S(v,"sourceBindings"))));set.Readback=new Readback(bundle);
            foreach(var reference in J.A(v,"exports")){byte[] bytes=sources.Reference(reference);set.Readback.Add(sources.Inside(J.S(reference,"path")),bytes);}set.Readback.ClassifyDuplicates();J.Need(set.Readback.Inputs.Count>0&&set.Readback.Inputs.All(x=>x.Rejection==null&&x.DuplicateOf==null),"IMPORT_ORIGINAL_EXPORT_REJECTED_OR_DUPLICATE");
            foreach(var exposure in J.A(v,"includedExposureIds")){string id=J.Text(exposure);J.Need(Guid.TryParseExact(id,"D",out _)&&set.Included.Add(id)&&set.Readback.Rows.Count(r=>r.ExposureId.ToString("D")==id)==1,"IMPORT_INCLUDED_EXPOSURE_UNKNOWN_OR_DUPLICATE");}
            J.Need(bundle.Catalogs.Values.All(c=>c.UiSourceSha256==uiHash),"IMPORT_MEASURED_UI_VERSION_CHANGED");result.Add(set);
        }
        J.Need(result.Count>0,"IMPORT_ACTUAL_OBSERVATIONS_MISSING");return result.ToArray();
    }
    private static void TrackBundle(Sources sources,string prefix,string packs,string publicPath,string bindingsPath,string producerPath)
    {
        byte[] Read(string p)=>sources.Read(prefix+"/"+p);Read(publicPath);Read(bindingsPath);var producer=J.Parse(Read(producerPath));var raw=J.Parse(Read(packs+"/raw-sha256-lock.json"));var ui=J.Parse(Read(packs+"/ui-source-lock.json"));foreach(var f in J.A(ui,"files"))Read(J.S(f,"path"));foreach(var m in J.A(raw,"manifests"))Read(packs+"/"+J.S(m,"path"));Read("Content/Quality/s1/difficulty-and-human-sample.json");Read("Assets/StammstreckenPuzzle/UI/MobileCandidate.endless-profiles.json");
        foreach(var pack in J.A(producer,"packs"))foreach(var row in J.A(pack,"sources"))
        {
            string p=J.S(row,"sourcePath");Read(p);if(p.EndsWith("/source-audit.internal.json",StringComparison.Ordinal)){string folder=p.Substring(0,p.LastIndexOf('/'));foreach(string name in new[]{"public.json","parameters.json","descriptor.internal.json","proof.internal.json","solution.internal.json"})Read(folder+"/"+name);string pool=folder.Substring(0,folder.LastIndexOf('/'));pool=pool.Substring(0,pool.LastIndexOf('/'));Read(pool+"/sample-audit.internal.json");}
        }
    }
    private static void CheckRequiredCampaignSample(Sources sources,OriginalSet[] sets,(OriginalSet Set,Observation Row)[] included,RuntimePublicPuzzleCatalog target)
    {
        var required=new Dictionary<string,(string Semantic,string Raw)>(StringComparer.Ordinal);
        foreach(var set in sets)
        {
            var quality=J.Parse(sources.Read(set.Prefix+"/Content/Quality/s1/difficulty-and-human-sample.json"));J.Fields(quality,"schemaVersion","policy","approvalStatus","levelCount","targetedLateCount","initialHumanSampleCount","humanAcceptance","starTimesReleased","entries");
            var needed=J.A(quality,"entries").Where(e=>J.Get(e,"humanSampleRequired").Kind==JsonKind.Boolean&&J.Get(e,"humanSampleRequired").BooleanValue).ToArray();J.Need(J.N(quality,"schemaVersion")==1&&J.N(quality,"levelCount")==240&&J.A(quality,"entries").Count==240&&J.N(quality,"initialHumanSampleCount")==54&&needed.Length==54,"IMPORT_CAMPAIGN_REQUIRED_SAMPLE_CONTRACT");
            var manifests=set.Bundle.Catalogs.Values.Where(c=>c.Assignments.All(a=>a.Purpose==HumanAssignmentPurpose.CAMPAIGN_TIME)).ToArray();J.Need(manifests.Length==9&&manifests.All(c=>c.Assignments.Count==6),"IMPORT_REQUIRED_NINE_CAMPAIGN_PACKS");var assigned=manifests.SelectMany(c=>c.Assignments).ToArray();J.Need(assigned.Length==54&&assigned.Select(a=>a.Puzzle.Identity.PuzzleId).Distinct().Count()==54&&assigned.Select(a=>a.Puzzle.Identity.PuzzleId).ToHashSet(StringComparer.Ordinal).SetEquals(needed.Select(e=>J.S(e,"puzzleId"))),"IMPORT_REQUIRED_CAMPAIGN_MANIFEST_SET");
            foreach(var row in needed)
            {
                string id=J.S(row,"puzzleId");var a=assigned.Single(a=>a.Puzzle.Identity.PuzzleId==id);J.Need(a.SourceSha256==J.H(row,"sourceSha256")&&target.Puzzles.Any(p=>p.Identity.Equals(a.Puzzle.Identity)),"IMPORT_REQUIRED_CAMPAIGN_SOURCE_BINDING");if(required.TryGetValue(id,out var old))J.Need(old.Semantic==a.Puzzle.Identity.HashSha256&&old.Raw==a.SourceSha256,"IMPORT_REQUIRED_CAMPAIGN_SNAPSHOT_CONFLICT");else required.Add(id,(a.Puzzle.Identity.HashSha256,a.SourceSha256));
            }
        }
        J.Need(required.Count==54&&required.All(r=>included.Any(x=>x.Row.ProfileId==null&&x.Row.PuzzleId==r.Key&&x.Row.SemanticSha256==r.Value.Semantic&&x.Row.SourceSha256==r.Value.Raw)),"IMPORT_CAMPAIGN_54_ACTUAL_OBSERVATIONS_REQUIRED");
    }
}
