using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using STP.Application;
using STP.Infrastructure.Content;

namespace STP.Tools.ProductionImport;

internal sealed class TextSources
{
    internal Dictionary<string,string> Player{get;}=new(StringComparer.Ordinal);
    internal Dictionary<string,string> Internal{get;}=new(StringComparer.Ordinal);
    internal Dictionary<string,string> PlayerOrigin{get;}=new(StringComparer.Ordinal);
    internal TextSources(Sources sources,IReadOnlyList<JsonValue> paths,string status)
    {
        J.Need(paths.Count>0,"IMPORT_LOCALIZATION_MISSING");
        foreach(var path in paths)
        {
            var v=sources.ReadJson(J.Text(path));bool runtime=v.Get("schemaVersion")==null;
            if(runtime)J.Fields(v,"locale","entries");else{J.Fields(v,"schemaVersion","locale","status","entries");J.Need(J.N(v,"schemaVersion")==1&&J.S(v,"status")==status,"IMPORT_LOCALIZATION_STATUS");}
            J.Need(J.S(v,"locale")=="de-DE","IMPORT_LOCALE_UNSUPPORTED");
            foreach(var entry in J.A(v,"entries"))
            {
                string key=J.S(entry,"key"),text;bool player;
                if(runtime){J.Fields(entry,"key","value","argumentTypes");text=J.S(entry,"value");J.A(entry,"argumentTypes");player=true;}
                else
                {
                    string[] common={"key","text","description","charLimit","placeholders","status","audience","availability","blockedBy","charLimitStatus"};J.Fields(entry,entry.Get("shortText")==null?common:common.Concat(new[]{"shortText"}).ToArray());
                    J.Need(J.S(entry,"status")==status&&J.S(entry,"availability")=="DECIDED"&&J.A(entry,"blockedBy").Count==0,"IMPORT_TEXT_UNAPPROVED_OR_BLOCKED:"+key);
                    text=J.S(entry,"text");J.Need(J.N(entry,"charLimit")>0&&text.Length<=J.N(entry,"charLimit"),"IMPORT_TEXT_LIMIT:"+key);J.Need(J.Get(entry,"placeholders").Kind==JsonKind.Object,"IMPORT_TEXT_PLACEHOLDERS");
                    string audience=J.S(entry,"audience");J.Need(audience=="PLAYER"||audience=="INTERNAL","IMPORT_TEXT_AUDIENCE");player=audience=="PLAYER";
                }
                var table=player?Player:Internal;
                J.Need(!table.ContainsKey(key),"IMPORT_DUPLICATE_LOCALIZATION:"+key);table.Add(key,text);if(player)PlayerOrigin.Add(key,J.Text(path));
            }
        }
    }
    internal void RequirePlayer(string key)=>J.Need(Player.ContainsKey(key),"IMPORT_PLAYER_TEXT_UNRESOLVED:"+key);
    internal void RequireInternal(string key)=>J.Need(Internal.ContainsKey(key),"IMPORT_INTERNAL_TEXT_UNRESOLVED:"+key);
}

/// <summary>The established renderer manifest contains fractional coordinates. Parse bounded JSON solely for
/// typed cross-references; identity is SHA256 of exact original bytes, never integer-JCS of rounded floats.</summary>
internal sealed class RailwaySources:IDisposable
{
    private readonly JsonDocument doc;
    internal string RawHash{get;}
    internal readonly Dictionary<string,(string Puzzle,string Kind,string Composition,int Stage)> Moments=new(StringComparer.Ordinal);
    internal readonly Dictionary<string,(string Address,string Title,string Renderer)> Cosmetics=new(StringComparer.Ordinal);
    private readonly HashSet<string> routes=new(StringComparer.Ordinal);
    private static readonly HashSet<string> modules=new(new[]{"barrier","bench","bridge","depot","indicator","noticeboard","platform","sev-stop","signal","tunnel","wayfinding"},StringComparer.Ordinal);
    internal RailwaySources(byte[] source,string status)
    {
        J.Need(source.Length<=4194304,"IMPORT_ASSET_SIZE");RawHash=J.Raw(source);doc=JsonDocument.Parse(source,new JsonDocumentOptions{AllowTrailingCommas=false,CommentHandling=JsonCommentHandling.Disallow,MaxDepth=16});
        Unique(doc.RootElement);var v=doc.RootElement;Fields(v,"schemaVersion","rendererVersion","approvalStatus","referenceSourceCommit","styleSourceCommit","origin","runtimeContract","worlds","routes","cosmetics","moments","reusableMoments");
        J.Need(v.GetProperty("schemaVersion").GetInt32()==1&&S(v,"rendererVersion")=="railway-cartoon-1"&&S(v,"approvalStatus")==status,"IMPORT_ASSET_VERSION_OR_STATUS");S(v,"referenceSourceCommit");S(v,"styleSourceCommit");S(v,"origin");S(v,"runtimeContract");
        var worlds=new HashSet<string>(StringComparer.Ordinal);int section=0;foreach(var w in v.GetProperty("worlds").EnumerateArray()){Fields(w,"id","section","rendererWorld");section++;J.Need(S(w,"id")=="s1-world-"+section.ToString("D2")&&w.GetProperty("section").GetInt32()==section&&w.GetProperty("rendererWorld").GetInt32()==section-1&&worlds.Add(S(w,"id")),"IMPORT_ASSET_WORLD");}J.Need(section==5,"IMPORT_ASSET_WORLD_COUNT");
        foreach(var r in v.GetProperty("routes").EnumerateArray())
        {
            Fields(r,"id","worldId","section","route","modules");int si=r.GetProperty("section").GetInt32(),ri=r.GetProperty("route").GetInt32();
            J.Need(si>=1&&si<=5&&ri>=1&&ri<=4&&S(r,"id")==$"s1-route-{si:D2}-{ri:D2}"&&S(r,"worldId")==$"s1-world-{si:D2}"&&worlds.Contains(S(r,"worldId"))&&routes.Add(S(r,"id")),"IMPORT_ASSET_ROUTE");
            J.Need(r.GetProperty("modules").GetArrayLength()>=3,"IMPORT_ASSET_ROUTE_MODULE_COUNT");
            foreach(var m in r.GetProperty("modules").EnumerateArray()){Fields(m,"kind","x","y","scale");double x=m.GetProperty("x").GetDouble(),y=m.GetProperty("y").GetDouble(),scale=m.GetProperty("scale").GetDouble();J.Need(modules.Contains(S(m,"kind"))&&double.IsFinite(x)&&double.IsFinite(y)&&double.IsFinite(scale)&&x>=0&&x<=400&&y>=0&&y<=240&&scale>=.2&&scale<=3,"IMPORT_ASSET_MODULE");}
        }
        J.Need(routes.Count==20,"IMPORT_ASSET_ROUTE_COUNT");
        foreach(var c in v.GetProperty("cosmetics").EnumerateArray()){Fields(c,"id","assetAddress","titleKey","rendererKey");string id=S(c,"id");J.Need(Cosmetics.TryAdd(id,(S(c,"assetAddress"),S(c,"titleKey"),S(c,"rendererKey")))&&S(c,"rendererKey")==id,"IMPORT_ASSET_COSMETIC");}J.Need(Cosmetics.Count==14,"IMPORT_ASSET_COSMETIC_COUNT");
        void Add(JsonElement m,bool reusable){Fields(m,"referenceId","puzzleId","kind","assetAddress","compositionId","stage");string kind=S(m,"kind"),id=S(m,"referenceId"),p=m.GetProperty("puzzleId").GetString()!;int stage=m.GetProperty("stage").GetInt32();J.Need((kind=="MAP"||kind=="TRAIN")&&S(m,"assetAddress")=="stp/moments/"+id&&routes.Contains(S(m,"compositionId"))&&(reusable?p.Length==0&&stage==0:p.Length>0&&stage>=1&&stage<=12)&&Moments.TryAdd(id,(p,kind,S(m,"compositionId"),stage)),"IMPORT_ASSET_MOMENT");}
        int count=0;foreach(var m in v.GetProperty("moments").EnumerateArray()){Add(m,false);count++;}J.Need(count==480,"IMPORT_ASSET_MOMENT_COUNT");var reusable=v.GetProperty("reusableMoments").EnumerateArray().ToArray();J.Need(reusable.Length==2&&reusable.All(m=>S(m,"referenceId")=="draft-endless-map-default"&&S(m,"kind")=="MAP"||S(m,"referenceId")=="draft-endless-train-default"&&S(m,"kind")=="TRAIN"),"IMPORT_ASSET_REUSABLE_NAMEDSET");foreach(var m in reusable)Add(m,true);
    }
    internal void Moment(string id,string kind,string puzzle,int section,int route,int stage)
    {J.Need(Moments.TryGetValue(id,out var m)&&m.Kind==kind&&m.Puzzle==puzzle&&(puzzle.Length==0||m.Composition==$"s1-route-{section:D2}-{route:D2}"&&m.Stage==stage),"IMPORT_TYPED_MOMENT_UNRESOLVED:"+id);}
    private static string S(JsonElement v,string key){string? s=v.GetProperty(key).GetString();J.Need(!string.IsNullOrWhiteSpace(s),"IMPORT_ASSET_STRING");return s!;}
    private static void Fields(JsonElement v,params string[] keys)=>J.Need(v.ValueKind==JsonValueKind.Object&&v.EnumerateObject().Count()==keys.Length&&v.EnumerateObject().All(p=>keys.Contains(p.Name,StringComparer.Ordinal)),"IMPORT_ASSET_CLOSED_FIELDS");
    private static void Unique(JsonElement v){if(v.ValueKind==JsonValueKind.Object){var names=new HashSet<string>(StringComparer.Ordinal);foreach(var p in v.EnumerateObject()){J.Need(names.Add(p.Name),"IMPORT_ASSET_DUPLICATE_KEY");Unique(p.Value);}}else if(v.ValueKind==JsonValueKind.Array){J.Need(v.GetArrayLength()<=2048,"IMPORT_ASSET_ARRAY_LIMIT");foreach(var x in v.EnumerateArray())Unique(x);}else if(v.ValueKind==JsonValueKind.Number)J.Need(double.IsFinite(v.GetDouble()),"IMPORT_ASSET_NONFINITE");}
    public void Dispose()=>doc.Dispose();
}

internal static class CrossReferences
{
    internal static void Validate(IReadOnlyList<LevelValidationResult> levels,JsonValue campaign,JsonValue campaignWrapper,JsonValue endlessWrapper,LockedCosmeticsCatalog cosmetics,TextSources texts,RailwaySources assets)
    {
        void Titles(JsonValue node){if(node.Kind==JsonKind.Object){if(node.Get("titleKey")!=null)texts.RequirePlayer(J.S(node,"titleKey"));foreach(var m in node.Members)Titles(m.Value);}else if(node.Kind==JsonKind.Array)foreach(var child in node.Items)Titles(child);}Titles(campaign);
        var subjects=new HashSet<string>(StringComparer.Ordinal){"S1"};var moments=J.A(J.Get(campaignWrapper,"completionCatalog"),"completionMoments");
        foreach(var result in levels)
        {
            var d=result.Level.Document!;int s=(int)d.Content.NetworkSection,r=(int)d.Content.Route,pos=(int)d.Content.Position;
            J.Need(d.Content.Season==1&&d.PuzzleId==$"S1-{s:D2}-{r:D2}-{pos:D2}","IMPORT_LEVEL_CONTENT_SLOT");subjects.Add($"S1-{s:D2}");subjects.Add($"S1-{s:D2}-{r:D2}");
            texts.RequirePlayer(d.Content.TitleKey);texts.RequirePlayer(d.Content.StatusKey);texts.RequirePlayer(d.Completion.ResultTextKey);texts.RequireInternal(d.Production.EntryDeductionKey);
            var matches=moments.Where(m=>J.S(m,"mapSegmentId")==d.Completion.MapSegmentId&&J.S(m,"trainMomentId")==d.Completion.TrainMomentId&&J.S(m,"resultTextKey")==d.Completion.ResultTextKey).ToArray();J.Need(matches.Length==1,"IMPORT_LEVEL_COMPLETION_UNRESOLVED:"+d.PuzzleId);
            assets.Moment(d.Completion.MapSegmentId,"MAP",d.PuzzleId,s,r,pos);assets.Moment(d.Completion.TrainMomentId,"TRAIN",d.PuzzleId,s,r,pos);
        }
        foreach(var m in J.A(J.Get(endlessWrapper,"completionCatalog"),"completionMoments")){texts.RequirePlayer(J.S(m,"resultTextKey"));assets.Moment(J.S(m,"mapSegmentId"),"MAP","",0,0,0);assets.Moment(J.S(m,"trainMomentId"),"TRAIN","",0,0,0);}
        J.Need(cosmetics.Items.Count==14,"IMPORT_COSMETIC_SCOPE");foreach(var c in cosmetics.Items){J.Need(c.Status!="DRAFT"&&assets.Cosmetics.TryGetValue(c.Id,out var a)&&a.Address==c.AssetAddress&&a.Title==c.TitleKey,"IMPORT_COSMETIC_ASSET_OR_STATUS:"+c.Id);texts.RequirePlayer(c.TitleKey);foreach(string subject in c.RequiredCampaignSubjectIds)J.Need(subjects.Contains(subject),"IMPORT_COSMETIC_SUBJECT_UNRESOLVED");}
        var expected=new Dictionary<string,(string Type,string Acquisition,long? Price,string[] Subjects)>(StringComparer.Ordinal);
        void Item(string id,string type,long? price,string? subject=null)=>expected.Add(id,(type,subject!=null?"MILESTONE_GRANT":price.HasValue?"PATIENCE_PURCHASE":"DEFAULT",price,subject==null?Array.Empty<string>():new[]{subject}));
        Item("train-stammrot-default","TRAIN_VARIANT",null);foreach(string id in new[]{"obj-wartebank-ohne-anschluss","obj-wegweiser-vielleicht","obj-absperrung-laufzeit-offen","obj-anzeiger-in-pruefung"})Item(id,"OPERATING_OBJECT",120);
        Item("livery-betriebsrot","LIVERY",300);Item("livery-baustellengelb","LIVERY",300);Item("obj-bahnsteig-provisorisch","OPERATING_OBJECT",300);Item("obj-tunnelportal-wartung","OPERATING_OBJECT",650);Item("obj-sev-bucht-geplant","OPERATING_OBJECT",650);Item("train-ersatzgarnitur","TRAIN_VARIANT",1200);Item("train-anschlusswunder","TRAIN_VARIANT",2500);Item("obj-signal-freigegeben","OPERATING_OBJECT",null,"S1-01");Item("obj-tafel-trotzdem-weiter","OPERATING_OBJECT",null,"S1");
        J.Need(cosmetics.Items.All(c=>expected.TryGetValue(c.Id,out var p)&&c.Type==p.Type&&c.Acquisition==p.Acquisition&&c.PricePatience==p.Price&&c.RequiredCampaignSubjectIds.SequenceEqual(p.Subjects)),"IMPORT_P06_COSMETIC_VALUES_CHANGED");
    }
}
