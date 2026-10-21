using System;
using System.Collections.Generic;
using System.Linq;
using STP.Application;

namespace STP.Infrastructure.Content
{
    /// <summary>Closed ADR044 complete runtime wrapper; its entire canonical bytes are the one completion identity.</summary>
    public sealed class RuntimeCompletionCatalog:ILockedCompletionCatalog
    {
        private readonly IReadOnlyList<CompletionValues> values;
        public RuntimeCompletionCatalog(byte[] wrapper,IPublicPuzzleCatalog corpus,CatalogIdentity expected,ICanonicalJson json)
        {
            if(corpus==null||expected==null||json==null)throw new ArgumentNullException();StarSource.Require(expected.Use==corpus.Identity.Use&&(expected.Use==CatalogUse.Production||expected.Use==CatalogUse.ContractTest),"STAR_CATALOG_USE_MISMATCH");var root=StarSource.Parse(wrapper);StarSource.Fields(root,"runtimeCompletionContractVersion","completionCatalog","calibrationCatalog");StarSource.Require(StarSource.N(root,"runtimeCompletionContractVersion")==1&&StarSource.Hash(root)==expected.HashSha256,"STAR_COMPLETION_LOCK_MISMATCH");var c=StarSource.Get(root,"completionCatalog");StarSource.Fields(c,"schemaVersion","catalogId","catalogRevision","approvalStatus","rewardDefinitions","completionMoments");StarSource.Require(StarSource.N(c,"schemaVersion")==1&&StarSource.Identity(c,expected.Use,expected.HashSha256).Equals(expected),"STAR_COMPLETION_IDENTITY_MISMATCH");Identity=expected;
            var calibration=StarSource.Get(root,"calibrationCatalog");Calibration=new StarCalibrationCatalog(StarSource.Bytes(calibration),corpus,StarSource.Identity(calibration,expected.Use,StarSource.Hash(calibration)),json);
            var definitions=StarSource.A(c,"rewardDefinitions");var rewards=new Dictionary<string,int>(StringComparer.Ordinal);SubjectKind=definitions.Count==4?"ENDLESS_INSTANCE":definitions.Count==6?"LEVEL":throw new ArgumentException("STAR_REWARDS_INVALID");var reasons=SubjectKind=="LEVEL"?new[]{"LEVEL_FIRST_CLEAR","LEVEL_SECOND_STAR","LEVEL_THIRD_STAR","ROUTE_CLEAR","SECTION_CLEAR","POST_CLEAR_PATIENCE"}:new[]{"LEVEL_FIRST_CLEAR","LEVEL_SECOND_STAR","LEVEL_THIRD_STAR","POST_CLEAR_PATIENCE"};int[] amounts=SubjectKind=="LEVEL"?new[]{15,5,10,60,150,10}:new[]{15,5,10,10};foreach(var d in definitions){StarSource.Fields(d,"reasonCode","subjectKind","amountPatience","claimCardinality");string reason=StarSource.S(d,"reasonCode");int index=Array.IndexOf(reasons,reason);StarSource.Require(index>=0&&!rewards.ContainsKey(reason)&&StarSource.N(d,"amountPatience")==amounts[index]&&StarSource.S(d,"claimCardinality")=="ONCE_PER_SUBJECT","STAR_REWARDS_INVALID");string kind=SubjectKind=="ENDLESS_INSTANCE"?SubjectKind:reason=="ROUTE_CLEAR"?"ROUTE":reason=="SECTION_CLEAR"?"SECTION":"LEVEL";StarSource.Require(StarSource.S(d,"subjectKind")==kind,"STAR_REWARD_SCOPE_INVALID");rewards.Add(reason,amounts[index]);}StarSource.Require(rewards.Count==reasons.Length,"STAR_REWARDS_INVALID");RewardAmounts=new System.Collections.ObjectModel.ReadOnlyDictionary<string,int>(rewards);
            var moments=StarSource.A(c,"completionMoments");StarSource.Require(moments.Count==(SubjectKind=="LEVEL"?240:1),"STAR_MOMENT_MEMBERSHIP_INVALID");foreach(var m in moments){StarSource.Fields(m,"id","resultTextKey","mapSegmentId","trainMomentId");foreach(string name in new[]{"id","mapSegmentId","trainMomentId"})StarSource.Stable(StarSource.S(m,name));StarSource.Key(StarSource.S(m,"resultTextKey"));}foreach(string name in new[]{"id","mapSegmentId","trainMomentId"})StarSource.Require(moments.Select(m=>StarSource.S(m,name)).Distinct(StringComparer.Ordinal).Count()==moments.Count,"STAR_MOMENT_REFERENCE_DUPLICATE");
            values=Array.AsReadOnly(SubjectKind=="LEVEL"?Calibration.Campaign.Select(e=>new CompletionValues(expected,e.Identity,15,5,10,60,150,e.Times.TwoStarMs,e.Times.ThreeStarMs)).ToArray():Array.Empty<CompletionValues>());
        }
        public CatalogIdentity Identity{get;}public StarCalibrationCatalog Calibration{get;}public string SubjectKind{get;}public IReadOnlyDictionary<string,int> RewardAmounts{get;}
        public bool TryGet(PublicPuzzleIdentity identity,out CompletionValues? value){value=values.SingleOrDefault(v=>v.Puzzle.Equals(identity));return value!=null;}
        public EndlessCompletionValues GetEndlessValues(){StarSource.Require(SubjectKind=="ENDLESS_INSTANCE","STAR_REWARD_SCOPE_INVALID");return new EndlessCompletionValues(Identity,RewardAmounts["LEVEL_FIRST_CLEAR"],RewardAmounts["LEVEL_SECOND_STAR"],RewardAmounts["LEVEL_THIRD_STAR"],RewardAmounts["POST_CLEAR_PATIENCE"]);}
        public EndlessStarCalibration GetEndlessCalibration(string profileId,string profileHash){StarSource.Require(SubjectKind=="ENDLESS_INSTANCE","STAR_REWARD_SCOPE_INVALID");var row=Calibration.GetEndless(profileId,profileHash);return new EndlessStarCalibration(Identity,Calibration.Identity,row.Times.CalibrationVersion,row.Times.TwoStarMs,row.Times.ThreeStarMs);}
    }
}
