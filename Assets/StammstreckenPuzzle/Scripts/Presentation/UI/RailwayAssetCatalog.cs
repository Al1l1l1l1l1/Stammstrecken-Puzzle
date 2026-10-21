using System;
using System.Collections.Generic;
using UnityEngine;

namespace STP.Presentation.UI
{
    /// <summary>Closed presentation-only asset manifest. It grants no content, ownership or release approval.</summary>
    public sealed class RailwayAssetCatalog
    {
        private readonly Dictionary<string,RailwayRouteComposition> routes=new Dictionary<string,RailwayRouteComposition>(StringComparer.Ordinal);
        private readonly Dictionary<string,RailwayMoment> reusable=new Dictionary<string,RailwayMoment>(StringComparer.Ordinal);
        private readonly Dictionary<string,RailwayMoment> moments=new Dictionary<string,RailwayMoment>(StringComparer.Ordinal);
        private readonly Dictionary<string,RailwayCosmeticAsset> cosmetics=new Dictionary<string,RailwayCosmeticAsset>(StringComparer.Ordinal);
        /// <summary>Validates the complete local manifest. Draft is accepted only for an explicitly internal preview.</summary>
        public RailwayAssetCatalog(TextAsset source,bool allowInternalDraft=false)
        {
            if(source==null)throw new ArgumentNullException(nameof(source));
            var document=JsonUtility.FromJson<Manifest>(source.text);
            if(document==null||document.schemaVersion!=1||document.rendererVersion!="railway-cartoon-1"||document.routes==null||document.routes.Length!=20||document.worlds==null||document.worlds.Length!=5||document.cosmetics==null||document.cosmetics.Length!=14||document.moments==null||document.moments.Length!=480)
                throw new ArgumentException("RAILWAY_MANIFEST_STRUCTURE");
            if(document.approvalStatus!="PRODUCT_APPROVED"&&!(allowInternalDraft&&document.approvalStatus=="DRAFT"))throw new ArgumentException("RAILWAY_MANIFEST_NOT_APPROVED");
            var worlds=new HashSet<string>(StringComparer.Ordinal);
            foreach(var world in document.worlds)if(world==null||world.section<1||world.section>5||world.rendererWorld!=world.section-1||!worlds.Add(world.id))throw new ArgumentException("RAILWAY_WORLD_INVALID");
            foreach(var route in document.routes)
            {
                if(route==null||route.section<1||route.section>5||route.route<1||route.route>4||!worlds.Contains(route.worldId)||route.modules==null||route.modules.Length<3||routes.ContainsKey(route.id))throw new ArgumentException("RAILWAY_ROUTE_INVALID");
                foreach(var module in route.modules)
                {
                    if(module==null)throw new ArgumentException("RAILWAY_MODULE_INVALID");
                    if(!RailwayCosmeticElement.SupportsModule(module.kind)||module.x<0||module.x>400||module.y<0||module.y>240||module.scale<.2f||module.scale>3)throw new ArgumentException("RAILWAY_MODULE_INVALID");
                }
                routes.Add(route.id,route);
            }
            var addresses=new HashSet<string>(StringComparer.Ordinal);
            foreach(var cosmetic in document.cosmetics)
            {
                if(cosmetic==null)throw new ArgumentException("RAILWAY_COSMETIC_INVALID");
                if(cosmetics.ContainsKey(cosmetic.id)||cosmetic.assetAddress!="stp/cosmetics/"+cosmetic.id||!addresses.Add(cosmetic.assetAddress)||!RailwayCosmeticElement.SupportsItem(cosmetic.id))throw new ArgumentException("RAILWAY_COSMETIC_INVALID");
                cosmetics.Add(cosmetic.id,cosmetic);
            }
            if(document.reusableMoments==null||document.reusableMoments.Length!=2)throw new ArgumentException("RAILWAY_REUSABLE_REFERENCES_REQUIRED");
            foreach(var moment in document.reusableMoments)
            {
                if(moment==null||!(moment.referenceId=="draft-endless-map-default"&&moment.kind=="MAP"||moment.referenceId=="draft-endless-train-default"&&moment.kind=="TRAIN")||moment.puzzleId!=string.Empty||!routes.ContainsKey(moment.compositionId)||moment.stage!=0||moment.assetAddress!="stp/moments/"+moment.referenceId||!addresses.Add(moment.assetAddress)||reusable.ContainsKey(moment.referenceId))throw new ArgumentException("RAILWAY_REUSABLE_REFERENCE_INVALID");
                reusable.Add(moment.referenceId,moment);
            }
            var puzzleKinds=new HashSet<string>(StringComparer.Ordinal);
            foreach(var moment in document.moments)
            {
                if(moment==null||string.IsNullOrEmpty(moment.referenceId)||moments.ContainsKey(moment.referenceId)||!routes.ContainsKey(moment.compositionId)||moment.stage<1||moment.stage>12||(moment.kind!="MAP"&&moment.kind!="TRAIN")||!puzzleKinds.Add(moment.puzzleId+"/"+moment.kind)||moment.assetAddress!="stp/moments/"+moment.referenceId||!addresses.Add(moment.assetAddress))throw new ArgumentException("RAILWAY_MOMENT_INVALID");
                moments.Add(moment.referenceId,moment);
            }
        }
        /// <summary>Resolves the exact level completion reference; no substitute scene is returned.</summary>
        public RailwayMoment ResolveMoment(string referenceId)=>moments.TryGetValue(referenceId,out var value)?value:throw new ArgumentException("RAILWAY_MOMENT_MISSING");
        /// <summary>Resolves a separately declared generic generated-puzzle composition; no campaign identity is borrowed.</summary>
        public RailwayMoment ResolveReusableMoment(string referenceId)=>reusable.TryGetValue(referenceId,out var value)?value:throw new ArgumentException("RAILWAY_REUSABLE_MOMENT_MISSING");
        /// <summary>Resolves a validated shared route composition.</summary>
        public RailwayRouteComposition ResolveComposition(string id)=>routes.TryGetValue(id,out var value)?value:throw new ArgumentException("RAILWAY_COMPOSITION_MISSING");
        /// <summary>Resolves an existing canonical cosmetic item to its own renderer.</summary>
        public RailwayCosmeticAsset ResolveCosmetic(string id)=>cosmetics.TryGetValue(id,out var value)?value:throw new ArgumentException("RAILWAY_COSMETIC_MISSING");
        /// <summary>Copies the catalog's actual renderable cosmetic definitions in canonical order.</summary>
        public IReadOnlyList<RailwayCosmeticAsset> CosmeticAssets
        {get{var list=new List<RailwayCosmeticAsset>(cosmetics.Values);list.Sort((a,b)=>string.CompareOrdinal(a.id,b.id));return list.AsReadOnly();}}
        /// <summary>Resolves a presentation-only puzzle/kind reference from the bound manifest.</summary>
        public RailwayMoment ResolvePuzzleMoment(string puzzleId,bool train)
        {foreach(var pair in moments)if(pair.Value.puzzleId==puzzleId&&pair.Value.kind==(train?"TRAIN":"MAP"))return pair.Value;throw new ArgumentException("RAILWAY_PUZZLE_MOMENT_MISSING");}
        [Serializable]private sealed class Manifest
        {
            public int schemaVersion=0;public string rendererVersion=string.Empty;public string approvalStatus=string.Empty;
            public World[] worlds=Array.Empty<World>();public RailwayRouteComposition[] routes=Array.Empty<RailwayRouteComposition>();
            public RailwayCosmeticAsset[] cosmetics=Array.Empty<RailwayCosmeticAsset>();public RailwayMoment[] moments=Array.Empty<RailwayMoment>();public RailwayMoment[] reusableMoments=Array.Empty<RailwayMoment>();
        }
        [Serializable]private sealed class World{public string id=string.Empty;public int section=0;public int rendererWorld=0;}
    }
    /// <summary>Authored route composition interpreted by the same code-native renderer.</summary>
    [Serializable]public sealed class RailwayRouteComposition
    {public string id=string.Empty;public string worldId=string.Empty;public int section;public int route;public RailwayModule[] modules=Array.Empty<RailwayModule>();}
    /// <summary>Placed decorative world object. It has no collision or puzzle meaning.</summary>
    [Serializable]public sealed class RailwayModule
    {public string kind=string.Empty;public float x;public float y;public float scale;}
    /// <summary>One exact original level completion reference and its reusable composition/stage.</summary>
    [Serializable]public sealed class RailwayMoment
    {public string referenceId=string.Empty;public string puzzleId=string.Empty;public string kind=string.Empty;public string assetAddress=string.Empty;public string compositionId=string.Empty;public int stage;}
    /// <summary>Canonical cosmetic address mapped to one original code-native illustration.</summary>
    [Serializable]public sealed class RailwayCosmeticAsset
    {public string id=string.Empty;public string assetAddress=string.Empty;public string titleKey=string.Empty;public string rendererKey=string.Empty;}
}
