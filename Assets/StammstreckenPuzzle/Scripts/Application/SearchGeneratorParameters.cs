using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace STP.Application
{
    /// <summary>Closed ADR040 internal profile source, fully bound to its canonical source hash and public corpus.</summary>
    public sealed class EndlessSearchProfile
    {
        /// <summary>Technical DRAFT search settings; construction grants no product approval.</summary>
        public EndlessSearchProfile(string id,string hash,string publicCatalogHash,PuzzleCandidateParameters search)
        {
            LocalModel.Require(new[]{"dienstbeginn","regelbetrieb","stoerungslage","grosslage"}.Contains(id),"GEN_PROFILE_ID_INVALID");
            Id=id;HashSha256=LocalModel.Hash(hash);PublicCatalogHashSha256=LocalModel.Hash(publicCatalogHash);Search=search??throw new ArgumentNullException(nameof(search));
            int slot=Array.IndexOf(new[]{"dienstbeginn","regelbetrieb","stoerungslage","grosslage"},id);int[] min={4,5,6,8},max={5,6,8,10};
            LocalModel.Require(search.MinSize==min[slot]&&search.MaxSize==max[slot],"GEN_PROFILE_RANGE_INVALID");
        }
        /// <summary>Named P05 target without a claimed human difficulty rating.</summary>
        public string Id{get;}
        /// <summary>JCS hash of the complete profile source excluding only its own hash field.</summary>
        public string HashSha256{get;}
        /// <summary>Exact actual public corpus lock.</summary>
        public string PublicCatalogHashSha256{get;}
        /// <summary>All twelve explicit search constraints.</summary>
        public PuzzleCandidateParameters Search{get;}
    }

    /// <summary>Persisted complete search inputs, pinned before generation and guarded by the existing parameter hash.</summary>
    public sealed class SearchGeneratorParameters:RetainedGeneratorParameters
    {
        /// <summary>Copies references; unknown algorithms/corpora or unbound recent inputs fail closed.</summary>
        public SearchGeneratorParameters(string profileId,string profileHashSha256,string noveltyVersion,CatalogIdentity campaignCatalog,IReadOnlyList<PublicPuzzleInput> recentPublicInputs,PuzzleCandidateParameters search)
        {
            LocalModel.Require(noveltyVersion==PuzzleCandidateSearch.NoveltyVersion,"GEN_NOVELTY_UNSUPPORTED");
            LocalModel.Require(campaignCatalog!=null&&campaignCatalog.Use==CatalogUse.InternalPreview&&campaignCatalog.ApprovalStatus==CatalogApprovalStatus.Draft,"GEN_CANDIDATE_REQUIRED");
            var profile=new EndlessSearchProfile(profileId,profileHashSha256,campaignCatalog!.HashSha256,search);
            ProfileId=profile.Id;ProfileHashSha256=profile.HashSha256;NoveltyVersion=noveltyVersion;CampaignCatalog=campaignCatalog;Search=search;
            RecentPublicInputs=LocalModel.Copy(recentPublicInputs);LocalModel.Require(RecentPublicInputs.Count<=50,"GEN_RECENT_CAPACITY");LocalModel.Unique(RecentPublicInputs.Select(p=>p.Identity.PuzzleId));
            LocalModel.Require(RecentPublicInputs.All(p=>p.Identity.PuzzleId.StartsWith("E1-",StringComparison.Ordinal)),"GEN_RECENT_ID_INVALID");
        }
        public string ProfileId{get;}
        public string ProfileHashSha256{get;}
        public string NoveltyVersion{get;}
        public CatalogIdentity CampaignCatalog{get;}
        public IReadOnlyList<PublicPuzzleInput> RecentPublicInputs{get;}
        public PuzzleCandidateParameters Search{get;}
        /// <summary>Exact existing JCS parameters projection; no alternate hash profile.</summary>
        public string Projection()=>"{\"contractVersion\":1,\"profileId\":"+LocalSaveHash.Quote(ProfileId)+",\"profileHashSha256\":"+LocalSaveHash.Quote(ProfileHashSha256)+",\"noveltyVersion\":"+LocalSaveHash.Quote(NoveltyVersion)+",\"campaignCatalog\":"+LocalSaveHash.CatalogProjection(CampaignCatalog)+",\"recentPublicInputs\":["+string.Join(",",RecentPublicInputs.Select(p=>EndlessProjection.PublicInput(p,true)))+"],"+EndlessProjection.SearchFields(Search)+"}";
    }

    /// <summary>Existing semantic/descriptor projections used by Application without importing content or JSON parsers.</summary>
    public static class EndlessProjection
    {
        private static string N(long value)=>value.ToString(CultureInfo.InvariantCulture);
        private static string Side(PublicPuzzleSide side)=>side==PublicPuzzleSide.N?"N":side==PublicPuzzleSide.E?"E":side==PublicPuzzleSide.S?"S":"W";
        /// <summary>Existing public input shape; semantic hash excludes its own profiled hash field.</summary>
        public static string PublicInput(PublicPuzzleInput p,bool includeHash)
        {
            string value="{\"puzzleId\":"+LocalSaveHash.Quote(p.Identity.PuzzleId)+",\"rulesetVersion\":"+LocalSaveHash.Quote(p.RulesetVersion)+",\"grid\":{\"width\":"+N(p.Width)+",\"height\":"+N(p.Height)+"},\"endpoints\":{\"a\":{\"side\":"+LocalSaveHash.Quote(Side(p.A.Side))+",\"index\":"+N(p.A.Index)+"},\"b\":{\"side\":"+LocalSaveHash.Quote(Side(p.B.Side))+",\"index\":"+N(p.B.Index)+"}},\"rowCounts\":["+string.Join(",",p.RowCounts.Select(n=>N(n)))+"],\"columnCounts\":["+string.Join(",",p.ColumnCounts.Select(n=>N(n)))+"]";
            return value+(includeHash?",\"publicPuzzleHash\":{\"profile\":"+LocalSaveHash.Quote(p.Identity.HashProfile)+",\"sha256\":"+LocalSaveHash.Quote(p.Identity.HashSha256)+"}}":"}");
        }
        /// <summary>All search settings in a complete closed object member sequence.</summary>
        public static string SearchFields(PuzzleCandidateParameters p)=>string.Join(",",new[]{("minSize",p.MinSize),("maxSize",p.MaxSize),("minDensityPercent",p.MinDensityPercent),("maxDensityPercent",p.MaxDensityPercent),("minDeductionSteps",p.MinDeductionSteps),("minDeductionDepth",p.MinDeductionDepth),("maxAttempts",p.MaxAttempts),("pathStateBudget",p.PathStateBudget),("totalBudgetMs",p.TotalBudgetMs),("solverBudgetMs",p.SolverBudgetMs),("maxTrivialLines",p.MaxTrivialLines),("recentSimilarityPercent",p.RecentSimilarityPercent)}.Select(t=>LocalSaveHash.Quote(t.Item1)+":"+N(t.Item2)));
        /// <summary>Unchanged endless-v1 identity projection.</summary>
        public static string DescriptorId(LocalEndlessDescriptor d,ICanonicalJson json)
        {
            string p="{\"endlessContractVersion\":1,\"rulesetVersion\":"+LocalSaveHash.Quote(d.RulesetVersion)+",\"generatorVersion\":"+LocalSaveHash.Quote(d.GeneratorVersion)+",\"seed\":"+LocalSaveHash.Quote(d.Seed)+",\"generationOrdinal\":"+LocalSaveHash.Quote(d.GenerationOrdinal)+",\"parameterHashSha256\":"+LocalSaveHash.Quote(d.ParameterHashSha256)+"}";
            return "E1-"+LocalSaveHash.Bytes(LocalSaveHash.Canonical(json,p));
        }
        /// <summary>Rejects fabricated public-input hashes at every retained-reference boundary.</summary>
        public static void ValidatePublic(PublicPuzzleInput input,ICanonicalJson json)=>LocalModel.Require(input.Identity.HashSha256==LocalSaveHash.Bytes(LocalSaveHash.Canonical(json,PublicInput(input,false))),"GEN_PUBLIC_INPUT_HASH_MISMATCH");
    }
}
