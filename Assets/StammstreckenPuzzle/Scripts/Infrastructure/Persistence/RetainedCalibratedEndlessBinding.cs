using System;
using System.Collections.Generic;
using System.Linq;
using STP.Application;

namespace STP.Infrastructure.Persistence
{
    /// <summary>Exact source-backed passive interpretation through the existing canonical port/cursor; no Content assembly dependency.</summary>
    public sealed class RetainedCalibratedEndlessBinding
    {
        private RetainedCalibratedEndlessBinding(CalibratedEndlessProfile profile,EndlessCompletionValues rewards){Profile=profile;Rewards=rewards;}
        public CalibratedEndlessProfile Profile{get;}
        public EndlessCompletionValues Rewards{get;}
        public CatalogIdentity CompletionCatalog=>Rewards.Catalog;
        internal bool Matches(SearchGeneratorParameters search)=>search.StarCalibration!=null&&search.ProfileId==Profile.Id&&search.ProfileHashSha256==Profile.HashSha256&&search.CampaignCatalog.HashSha256==Profile.Profile.PublicCatalogHashSha256&&search.StarCalibration.EqualsBinding(Profile.StarCalibration)&&EndlessProjection.SearchFields(search.Search)==EndlessProjection.SearchFields(Profile.Search);
        internal sealed class CalibrationData
        {
            internal CatalogIdentity Catalog=null!;
            internal readonly Dictionary<string,(string Evidence,string Ui)> Provenance=new Dictionary<string,(string,string)>(StringComparer.Ordinal);
            internal readonly Dictionary<string,(string Hash,CalibratedStarTimes Times)> Profiles=new Dictionary<string,(string,CalibratedStarTimes)>(StringComparer.Ordinal);
        }
        internal static void Reference(CanonicalDtoCursor cursor,CatalogIdentity expected)
        {var f=cursor.Fields("catalogId","revision","hashSha256");LocalSaveCodec.Require(f[0].String()==expected.CatalogId&&f[1].Integer()==expected.Revision&&f[2].String()==expected.HashSha256,"SAVE_CALIBRATION_REFERENCE_MISMATCH",true);}
        internal static CalibrationData ValidateCalibration(CanonicalDtoCursor v,IPublicPuzzleCatalog corpus,ICanonicalJson json)
        {
            var f=v.Fields("schemaVersion","catalogId","catalogRevision","approvalStatus","publicCatalog","provenance","campaign","endlessProfiles");f[0].RequireInteger(1);var result=new CalibrationData{Catalog=BindingReaders.Catalog(v,json,corpus.Identity.Use)};Reference(f[4],corpus.Identity);
            foreach(var row in f[5].Array()){var p=row.Fields("calibrationVersion","evidenceSha256","uiSourceSha256");string version=LocalSaveCodec.Text(p[0].String());LocalSaveCodec.Require(!result.Provenance.ContainsKey(version),"SAVE_CALIBRATION_PROVENANCE_INVALID");result.Provenance.Add(version,(LocalSaveCodec.Hash(p[1].String()),LocalSaveCodec.Hash(p[2].String())));}LocalSaveCodec.Require(result.Provenance.Count>0,"SAVE_CALIBRATION_PROVENANCE_INVALID");
            CalibratedStarTimes Times(CanonicalDtoCursor version,CanonicalDtoCursor two,CanonicalDtoCursor three){var value=new CalibratedStarTimes(version.String(),two.Integer(),three.Integer());LocalSaveCodec.Require(result.Provenance.ContainsKey(value.CalibrationVersion),"SAVE_CALIBRATION_PROVENANCE_MISSING");return value;}
            var ids=new HashSet<string>(StringComparer.Ordinal);foreach(var row in f[6].Array()){var c=row.Fields("identity","sourceDocumentSha256","calibrationVersion","twoStarsSeconds","threeStarsSeconds");var id=c[0].Fields("puzzleId","profile","sha256");var identity=new PublicPuzzleIdentity(id[0].String(),id[1].String(),id[2].String());LocalSaveCodec.Require(ids.Add(identity.PuzzleId)&&corpus.TryGet(identity,out _),"SAVE_CALIBRATION_MEMBERSHIP_INVALID");LocalSaveCodec.Hash(c[1].String());Times(c[2],c[3],c[4]);}LocalSaveCodec.Require(corpus.Puzzles.Count==240&&ids.Count==240&&corpus.Puzzles.All(p=>ids.Contains(p.Identity.PuzzleId)),"SAVE_CALIBRATION_MEMBERSHIP_INVALID");
            foreach(var row in f[7].Array()){var p=row.Fields("profileId","profileHashSha256","calibrationVersion","twoStarsSeconds","threeStarsSeconds");string id=p[0].String();LocalSaveCodec.Require(!result.Profiles.ContainsKey(id),"SAVE_CALIBRATION_MEMBERSHIP_INVALID");result.Profiles.Add(id,(LocalSaveCodec.Hash(p[1].String()),Times(p[2],p[3],p[4])));}string[] expected={"dienstbeginn","regelbetrieb","stoerungslage","grosslage"};LocalSaveCodec.Require(result.Profiles.Count==4&&expected.All(result.Profiles.ContainsKey),"SAVE_CALIBRATION_MEMBERSHIP_INVALID");return result;
        }
        public static RetainedCalibratedEndlessBinding FromDocuments(byte[] wrapper,byte[] qualityProfile,IPublicPuzzleCatalog corpus,CatalogIdentity expectedCompletion,string expectedProfileHashSha256,ICanonicalJson json)
        {
            if(corpus==null||expectedCompletion==null||json==null)throw new ArgumentNullException();LocalSaveCodec.Require(corpus.Identity.Use==expectedCompletion.Use&&(corpus.Identity.Use==CatalogUse.Production||corpus.Identity.Use==CatalogUse.ContractTest),"SAVE_CALIBRATION_USE_MISMATCH",true);
            var completion=RetainedCompletionRewardBinding.FromRuntimeWrapper(LocalPostClearPolicy.PolicyVersion,wrapper,json,expectedCompletion,corpus);LocalSaveCodec.Require(completion.SubjectKind=="ENDLESS_INSTANCE","SAVE_REWARD_SCOPE_MISMATCH");var wrapped=BindingReaders.Document(wrapper,json,expectedCompletion.HashSha256);var calibration=ValidateCalibration(wrapped.Find("calibrationCatalog"),corpus,json);
            var q=BindingReaders.Document(qualityProfile,json,expectedProfileHashSha256);var f=q.Fields("schemaVersion","profileId","profileVersion","rulesetVersion","approvalStatus","difficultyBand","generatorVersion","solverVersion","gridRanges","metricRanges","runtimeBudgets","novelty","humanReview","approvedBy","approvedDecisionRef");f[0].RequireInteger(1);string id=f[1].String();f[3].RequireString("train-track-v1");f[4].RequireString(corpus.Identity.Use==CatalogUse.Production?"PRODUCT_APPROVED":"FIXTURE_ONLY");f[5].RequireString(id);f[6].RequireString(PuzzleCandidateSearch.GeneratorVersion);f[7].RequireString("solver-v2");LocalSaveCodec.Require(calibration.Profiles.TryGetValue(id,out var row)&&row.Hash==expectedProfileHashSha256,"SAVE_CALIBRATION_PROFILE_MISMATCH",true);
            var g=f[8].Fields("minSize","maxSize","minDensityPercent","maxDensityPercent");var m=f[9].Fields("searchNodes","deductionSteps","maxDeductionDepth","requiredGuessDepth","trivialLines");QualityInterval Range(CanonicalDtoCursor r){var a=r.Fields("min","max");return new QualityInterval(a[0].Integer(),a[1].Integer());}var bounds=new EndlessQualityBounds(Range(m[0]),Range(m[1]),Range(m[2]),Range(m[3]),Range(m[4]));
            var b=f[10].Fields("maxAttempts","pathStateBudget","totalBudgetMs","solverBudgetMs","solverStateBudget","hintBudgetMs","hintStateBudget","timeoutPolicy");b[4].RequireInteger(1000000);b[5].RequireInteger(100);b[6].RequireInteger(100000);b[7].RequireString("ABORT_ATTEMPT_KEEP_RESERVATION");var n=f[11].Fields("algorithmVersion","referenceCorpus","recentWindow","recentSimilarityPercent");n[0].RequireString(PuzzleCandidateSearch.NoveltyVersion);Reference(n[1],corpus.Identity);n[2].RequireInteger(50);
            int Small(long value){LocalSaveCodec.Require(value<=int.MaxValue,"SAVE_CALIBRATION_PARAMETER_UNSUPPORTED",true);return(int)value;}var search=new PuzzleCandidateParameters(g[0].Int32(),g[1].Int32(),g[2].Int32(),g[3].Int32(),Small(bounds.DeductionSteps.Min),Small(bounds.MaxDeductionDepth.Min),b[0].Int32(),b[1].Int32(),b[2].Int32(),b[3].Int32(),Small(bounds.TrivialLines.Max),n[3].Int32());
            var h=f[12].Fields("calibrationVersion","evidenceSha256","uiSourceSha256","sampleCount","criteriaVersion");var provenance=calibration.Provenance[row.Times.CalibrationVersion];LocalSaveCodec.Require(h[0].String()==row.Times.CalibrationVersion&&h[1].String()==provenance.Evidence&&h[2].String()==provenance.Ui&&h[3].Integer()>=12,"SAVE_CALIBRATION_PROVENANCE_MISMATCH",true);LocalSaveCodec.Text(h[4].String());LocalSaveCodec.Text(f[13].String());LocalSaveCodec.Text(f[14].String());LocalSaveCodec.Require(f[2].Integer()>0,"SAVE_CALIBRATION_PROFILE_MISMATCH");
            var stars=new EndlessStarCalibration(expectedCompletion,calibration.Catalog,row.Times.CalibrationVersion,row.Times.TwoStarMs,row.Times.ThreeStarMs);var profile=new CalibratedEndlessProfile(new EndlessSearchProfile(id,expectedProfileHashSha256,corpus.Identity.HashSha256,search),f[2].Int32(),bounds,stars);return new RetainedCalibratedEndlessBinding(profile,new EndlessCompletionValues(expectedCompletion,(int)completion.RewardAmounts["LEVEL_FIRST_CLEAR"],(int)completion.RewardAmounts["LEVEL_SECOND_STAR"],(int)completion.RewardAmounts["LEVEL_THIRD_STAR"],(int)completion.RewardAmounts["POST_CLEAR_PATIENCE"]));
        }
    }
}
