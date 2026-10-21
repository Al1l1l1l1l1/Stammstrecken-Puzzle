using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using STP.Infrastructure.Content;

namespace STP.Tools.ProductionImport;

internal static class UiTransfer
{
    private const string Prefix="Assets/StammstreckenPuzzle/";
    internal static void CheckTargetFileSet(Sources sources,JsonValue ui)
    {
        var expected=new[]{"Scripts/Presentation/UI","Scripts/Presentation/World"}.SelectMany(p=>Directory.GetFiles(sources.Inside(Prefix+p),"*.cs",SearchOption.AllDirectories))
            .Concat(Directory.GetFiles(sources.Inside(Prefix+"Art"),"*",SearchOption.AllDirectories).Where(p=>new[]{".png",".svg",".ttf",".otf"}.Contains(Path.GetExtension(p).ToLowerInvariant())))
            .Select(p=>Path.GetRelativePath(sources.Root,p).Replace('\\','/')).Concat(new[]{"ProductionGame.uxml","ProductionGame.uss","ProductionGameLocale.json","ProductionCampaignText.json","RailwayAssets.json"}.Select(p=>Prefix+"UI/"+p)).OrderBy(p=>p,StringComparer.Ordinal).ToArray();
        J.Need(J.A(ui,"files").Select(f=>J.S(f,"path")).SequenceEqual(expected),"IMPORT_TARGET_UI_COMPLETE_FILESET");
    }
    internal static void Validate(Sources sources,string targetLockPath,string measuredLockPath,string measuredPrefix,string expectedMeasuredHash,string targetTextSourceRawHash)
    {
        byte[] measured=sources.Read(measuredLockPath),target=sources.Read(targetLockPath);J.Need(J.Raw(measured)==expectedMeasuredHash,"IMPORT_MEASURED_UI_RAW_LOCK_MISMATCH");
        var old=J.Parse(measured);var next=J.Parse(target);J.Fields(old,"schemaVersion","kind","files");J.Fields(next,"schemaVersion","kind","files");J.Need(J.N(old,"schemaVersion")==1&&J.N(next,"schemaVersion")==1&&J.S(old,"kind")=="HUMAN_UI_SOURCE_LOCK"&&J.S(next,"kind")=="HUMAN_UI_SOURCE_LOCK","IMPORT_UI_LOCK_KIND");
        var oldFiles=J.A(old,"files");var newFiles=J.A(next,"files");J.Need(oldFiles.Select(f=>J.S(f,"path")).SequenceEqual(newFiles.Select(f=>J.S(f,"path"))),"IMPORT_UI_TRANSFER_FILESET_CHANGED");
        for(int i=0;i<oldFiles.Count;i++)
        {
            J.Fields(oldFiles[i],"path","sha256");J.Fields(newFiles[i],"path","sha256");string path=J.S(oldFiles[i],"path");byte[] a=sources.Read(measuredPrefix+"/"+path),b=sources.Read(path);
            J.Need(J.Raw(a)==J.H(oldFiles[i],"sha256")&&J.Raw(b)==J.H(newFiles[i],"sha256"),"IMPORT_UI_TRANSFER_ORIGINAL_BYTES");
            if(a.SequenceEqual(b))continue;
            J.Need(oldFiles.Count==75,"IMPORT_UI_METADATA_TRANSFER_REQUIRES_COMPLETE_75_SOURCE_SET");
            if(path==Prefix+"UI/RailwayAssets.json")CompareMetadataOnly(a,b,false,null);
            else if(path==Prefix+"UI/ProductionCampaignText.json")CompareMetadataOnly(a,b,true,targetTextSourceRawHash);
            else throw new ArgumentException("IMPORT_MEASURED_UI_RENDER_SOURCE_CHANGED:"+path);
        }
    }
    internal static void CompareMetadataOnly(byte[] measured,byte[] target,bool text,string? targetTextSourceRawHash)
    {
        J.Need(measured.Length<=4194304&&target.Length<=4194304,"IMPORT_UI_METADATA_SOURCE_SIZE");using var a=JsonDocument.Parse(measured,new JsonDocumentOptions{MaxDepth=32});using var b=JsonDocument.Parse(target,new JsonDocumentOptions{MaxDepth=32});Unique(a.RootElement);Unique(b.RootElement);
        J.Need(a.RootElement.GetProperty("approvalStatus").GetString()=="DRAFT"&&b.RootElement.GetProperty("approvalStatus").GetString()=="PRODUCT_APPROVED","IMPORT_UI_METADATA_APPROVAL_TRANSITION");
        if(text)J.Need(b.RootElement.GetProperty("sourceSha256").GetString()==targetTextSourceRawHash,"IMPORT_UI_METADATA_TARGET_TEXT_SOURCE");
        var ignored=text?new[]{"approvalStatus","sourceSha256"}:new[]{"approvalStatus"};
        var left=a.RootElement.EnumerateObject().Where(p=>!ignored.Contains(p.Name)).OrderBy(p=>p.Name,StringComparer.Ordinal).ToArray();var right=b.RootElement.EnumerateObject().Where(p=>!ignored.Contains(p.Name)).OrderBy(p=>p.Name,StringComparer.Ordinal).ToArray();
        J.Need(a.RootElement.EnumerateObject().Count()==b.RootElement.EnumerateObject().Count()&&left.Length==right.Length&&left.Select(p=>p.Name).SequenceEqual(right.Select(p=>p.Name))&&left.Zip(right).All(p=>Same(p.First.Value,p.Second.Value)),"IMPORT_UI_RENDER_VALUES_CHANGED");
    }
    private static bool Same(JsonElement a,JsonElement b)
    {
        if(a.ValueKind!=b.ValueKind)return false;
        if(a.ValueKind==JsonValueKind.Object){var x=a.EnumerateObject().OrderBy(p=>p.Name,StringComparer.Ordinal).ToArray();var y=b.EnumerateObject().OrderBy(p=>p.Name,StringComparer.Ordinal).ToArray();return x.Length==y.Length&&x.Select(p=>p.Name).SequenceEqual(y.Select(p=>p.Name))&&x.Zip(y).All(p=>Same(p.First.Value,p.Second.Value));}
        if(a.ValueKind==JsonValueKind.Array){var x=a.EnumerateArray().ToArray();var y=b.EnumerateArray().ToArray();return x.Length==y.Length&&x.Zip(y).All(p=>Same(p.First,p.Second));}
        if(a.ValueKind==JsonValueKind.String)return a.GetString()==b.GetString();
        // In particular, preserve exact fractional tokens. No double rounding and no modification of integer-JCS.
        return a.GetRawText()==b.GetRawText();
    }
    private static void Unique(JsonElement v){if(v.ValueKind==JsonValueKind.Object){var names=new HashSet<string>(StringComparer.Ordinal);foreach(var p in v.EnumerateObject()){J.Need(names.Add(p.Name),"IMPORT_UI_METADATA_DUPLICATE");Unique(p.Value);}}else if(v.ValueKind==JsonValueKind.Array){J.Need(v.GetArrayLength()<=4096,"IMPORT_UI_METADATA_ARRAY_LIMIT");foreach(var x in v.EnumerateArray())Unique(x);}else if(v.ValueKind==JsonValueKind.Number)J.Need(double.IsFinite(v.GetDouble()),"IMPORT_UI_METADATA_NONFINITE");}
}
