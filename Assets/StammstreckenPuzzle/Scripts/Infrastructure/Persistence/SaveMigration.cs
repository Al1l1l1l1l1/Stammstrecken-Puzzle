using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using STP.Application;
using STP.Puzzle.Domain;

namespace STP.Infrastructure.Persistence
{
    /// <summary>One immutable explicitly known privacy reader tuple; it enables no capability.</summary>
    public sealed class RetainedPrivacyBinding
    {
        /// <summary>Binds the exact policy, SDK and source strings.</summary>
        public RetainedPrivacyBinding(string policyRevision,string sdkContractRevision,string source){PolicyRevision=policyRevision??throw new ArgumentNullException(nameof(policyRevision));SdkContractRevision=sdkContractRevision??throw new ArgumentNullException(nameof(sdkContractRevision));Source=source??throw new ArgumentNullException(nameof(source));}
        /// <summary>Exact retained policy revision.</summary>
        public string PolicyRevision{get;}
        /// <summary>Exact retained SDK contract.</summary>
        public string SdkContractRevision{get;}
        /// <summary>Exact persisted source semantics.</summary>
        public string Source{get;}
    }
    /// <summary>Known passive store/product reader tuple, without receipts or provider execution.</summary>
    public sealed class RetainedIapBinding
    {
        /// <summary>Binds a documented passive reader policy to one store and logical product.</summary>
        public RetainedIapBinding(string policyVersion,string store,string logicalProductKey){PolicyVersion=policyVersion??throw new ArgumentNullException(nameof(policyVersion));Store=store??throw new ArgumentNullException(nameof(store));LogicalProductKey=logicalProductKey??throw new ArgumentNullException(nameof(logicalProductKey));}
        /// <summary>Known passive reader version.</summary>
        public string PolicyVersion{get;}
        /// <summary>Known store namespace.</summary>
        public string Store{get;}
        /// <summary>Known product namespace.</summary>
        public string LogicalProductKey{get;}
    }
    /// <summary>Closed explicit legacy-ownership provenance reader. This is no current cosmetic grant policy.</summary>
    public sealed class RetainedLegacyOwnershipReader
    {
        /// <summary>Binds one exact supported source version and closed item-type set.</summary>
        public RetainedLegacyOwnershipReader(string readerId,int sourceSaveSchemaVersion,IReadOnlyList<string> itemTypes){ReaderId=readerId??throw new ArgumentNullException(nameof(readerId));if(sourceSaveSchemaVersion!=1||itemTypes==null||itemTypes.Count==0||itemTypes.Any(t=>t!="OPERATING_OBJECT"&&t!="LIVERY"&&t!="TRAIN_VARIANT")||itemTypes.Distinct(StringComparer.Ordinal).Count()!=itemTypes.Count)throw new ArgumentException("SAVE_LEGACY_READER_UNSUPPORTED");SourceSaveSchemaVersion=sourceSaveSchemaVersion;ItemTypes=Array.AsReadOnly(itemTypes.ToArray());}
        /// <summary>Known closed source reader ID.</summary>
        public string ReaderId{get;}
        /// <summary>Original supported source schema.</summary>
        public int SourceSaveSchemaVersion{get;}
        /// <summary>Copied supported item types.</summary>
        public IReadOnlyList<string> ItemTypes{get;}
    }
    /// <summary>One schema-validated immutable cosmetic item retained without a purchase or claim command.</summary>
    public sealed class RetainedCosmeticItem
    {
        internal RetainedCosmeticItem(string id,string type,string acquisition,long? price,string[] subjects){ItemId=id;ItemType=type;Acquisition=acquisition;PricePatience=price;RequiredCampaignSubjectIds=Array.AsReadOnly(subjects);}
        /// <summary>Stable historical item ID.</summary>
        public string ItemId{get;}
        /// <summary>Bound type.</summary>
        public string ItemType{get;}
        /// <summary>Exact single acquisition mode.</summary>
        public string Acquisition{get;}
        /// <summary>Historical positive purchase price, or null for the other modes.</summary>
        public long? PricePatience{get;}
        /// <summary>Exact closed milestone subject set.</summary>
        public IReadOnlyList<string> RequiredCampaignSubjectIds{get;}
    }
    /// <summary>Verified immutable catalog provenance for passive cosmetic records.</summary>
    public sealed class RetainedCosmeticsBinding
    {
        private RetainedCosmeticsBinding(CatalogIdentity identity,ILockedCampaignCatalog campaign,RetainedCosmeticItem[] items){Identity=identity;Campaign=campaign;Items=Array.AsReadOnly(items);}
        /// <summary>Verifies complete canonical document identities and parses closed source grammars before binding items.</summary>
        public static RetainedCosmeticsBinding FromDocuments(byte[] cosmeticDocument,byte[] campaignDocument,ICanonicalJson json,CatalogUse use,IPublicPuzzleCatalog puzzles)
        {
            var c=BindingReaders.Document(cosmeticDocument,json);var f=c.Fields("schemaVersion","catalogId","catalogRevision","approvalStatus","items");f[0].RequireInteger(2);var identity=BindingReaders.Catalog(c,json,use);var campaign=BindingReaders.Campaign(campaignDocument,json,use,puzzles,null);var list=new List<RetainedCosmeticItem>();
            foreach(var item in f[4].Array())
            {
                string mode=item.Find("acquisition").String();string[] baseFields={"id","type","status","titleKey","assetAddress","acquisition"};var fields=baseFields.Concat(mode=="PATIENCE_PURCHASE"?new[]{"pricePatience"}:mode=="MILESTONE_GRANT"?new[]{"milestoneEligibility"}:Array.Empty<string>()).ToArray();item.Fields(fields);string id=item.Find("id").String();BindingReaders.Stable(id);string type=item.Find("type").String();LocalSaveCodec.Require(type=="OPERATING_OBJECT"||type=="LIVERY"||type=="TRAIN_VARIANT","SAVE_COSMETIC_TYPE_UNSUPPORTED",true);string status=item.Find("status").String();LocalSaveCodec.Require(new[]{"DRAFT","ACTIVE","HIDDEN","TOMBSTONE"}.Contains(status),"SAVE_COSMETIC_STATUS_INVALID");BindingReaders.Key(item.Find("titleKey").String());LocalSaveCodec.Text(item.Find("assetAddress").String());long? price=null;string[] subjects=Array.Empty<string>();
                if(mode=="PATIENCE_PURCHASE"){price=item.Find("pricePatience").Integer();LocalSaveCodec.Require(price>0,"SAVE_COSMETIC_PRICE_INVALID");}
                else if(mode=="MILESTONE_GRANT"){var e=item.Find("milestoneEligibility").Fields("contractVersion","kind","requiredCampaignSubjectIds");e[0].RequireInteger(1);e[1].RequireString("ALL_FIRST_CLEARS_OF_CAMPAIGN_SUBJECTS");subjects=e[2].Array().Select(x=>x.String()).ToArray();LocalSaveCodec.Require(subjects.Length>0&&subjects.Distinct(StringComparer.Ordinal).Count()==subjects.Length&&subjects.All(id2=>BindingReaders.Expand(campaign,id2).Count>0),"SAVE_COSMETIC_SUBJECT_INVALID");}
                else LocalSaveCodec.Require(mode=="DEFAULT","SAVE_COSMETIC_ACQUISITION_UNSUPPORTED",true);
                list.Add(new RetainedCosmeticItem(id,type,mode,price,subjects));
            }
            LocalSaveCodec.Require(list.Count>0&&list.Select(x=>x.ItemId).Distinct(StringComparer.Ordinal).Count()==list.Count,"SAVE_DUPLICATE_ID");return new RetainedCosmeticsBinding(identity,campaign,list.ToArray());
        }
        /// <summary>Exact full source catalog identity.</summary>
        public CatalogIdentity Identity{get;}
        /// <summary>Exact fully bound campaign source used for milestone expansion.</summary>
        public ILockedCampaignCatalog Campaign{get;}
        /// <summary>Copied closed item records.</summary>
        public IReadOnlyList<RetainedCosmeticItem> Items{get;}
    }
    /// <summary>Actual immutable POST_CLEAR_PATIENCE definition from one complete bound completion document; it never executes the reward.</summary>
    public sealed class RetainedCompletionRewardBinding
    {
        private RetainedCompletionRewardBinding(string policy,CatalogIdentity catalog,string subjectKind,long amount,IReadOnlyDictionary<string,long>? definitions=null){PolicyVersion=policy;Catalog=catalog;SubjectKind=subjectKind;AmountPatience=amount;RewardAmounts=new System.Collections.ObjectModel.ReadOnlyDictionary<string,long>(definitions==null?new Dictionary<string,long>(StringComparer.Ordinal){{"POST_CLEAR_PATIENCE",amount}}:definitions.ToDictionary(p=>p.Key,p=>p.Value,StringComparer.Ordinal));}
        public IReadOnlyDictionary<string,long> RewardAmounts{get;}
        /// <summary>Parses and hashes the complete closed completion-v1 source and selects exactly one actual POST_CLEAR_PATIENCE definition.</summary>
        public static RetainedCompletionRewardBinding FromDocument(string policyVersion,byte[] source,ICanonicalJson json,CatalogUse use)
        {
            var v=BindingReaders.Document(source,json);var f=v.Fields("schemaVersion","catalogId","catalogRevision","approvalStatus","rewardDefinitions","completionMoments");f[0].RequireInteger(1);var catalog=BindingReaders.Catalog(v,json,use);var definitions=f[4].Array();foreach(var d in definitions){var a=d.Fields("reasonCode","subjectKind","amountPatience","claimCardinality");LocalSaveCodec.Require(System.Text.RegularExpressions.Regex.IsMatch(a[0].String(),@"^[A-Z][A-Z0-9_]*$")&&new[]{"LEVEL","ROUTE","SECTION","SEASON","ENDLESS_INSTANCE"}.Contains(a[1].String())&&a[2].Integer()>=0&&a[2].Integer()<=1000000&&new[]{"ONCE_PER_SUBJECT","ONCE_PER_ACHIEVEMENT"}.Contains(a[3].String()),"SAVE_COMPLETION_BINDING_INVALID");}var selected=definitions.Where(d=>d.Find("reasonCode").String()=="POST_CLEAR_PATIENCE").ToArray();LocalSaveCodec.Require(selected.Length==1,"SAVE_COMPLETION_BINDING_UNSUPPORTED",true);var reward=selected[0];if(reward.Find("subjectKind").String()=="ENDLESS_INSTANCE"){string[] reasons={"LEVEL_FIRST_CLEAR","LEVEL_SECOND_STAR","LEVEL_THIRD_STAR","POST_CLEAR_PATIENCE"};long[] amounts={15,5,10,10};LocalSaveCodec.Require(use==CatalogUse.InternalPreview&&catalog.CatalogId=="completion-qa-mobile-candidate-endless"&&definitions.Count==4,"SAVE_ENDLESS_COMPLETION_BINDING_UNSUPPORTED",true);for(int index=0;index<4;index++){var exact=definitions.Where(d=>d.Find("reasonCode").String()==reasons[index]).ToArray();LocalSaveCodec.Require(exact.Length==1&&exact[0].Find("subjectKind").String()=="ENDLESS_INSTANCE"&&exact[0].Find("amountPatience").Integer()==amounts[index]&&exact[0].Find("claimCardinality").String()=="ONCE_PER_SUBJECT","SAVE_ENDLESS_COMPLETION_BINDING_UNSUPPORTED",true);}}reward.Find("claimCardinality").RequireString("ONCE_PER_SUBJECT");foreach(var moment in f[5].Array()){var m=moment.Fields("id","resultTextKey","mapSegmentId","trainMomentId");BindingReaders.Stable(m[0].String());BindingReaders.Key(m[1].String());BindingReaders.Stable(m[2].String());BindingReaders.Stable(m[3].String());}LocalSaveCodec.Require(f[5].Array().Count>0,"SAVE_COMPLETION_BINDING_INVALID");return new RetainedCompletionRewardBinding(LocalSaveCodec.Text(policyVersion),catalog,reward.Find("subjectKind").String(),reward.Find("amountPatience").Integer(),definitions.ToDictionary(d=>d.Find("reasonCode").String(),d=>d.Find("amountPatience").Integer(),StringComparer.Ordinal));
        }
        /// <summary>Reads the existing full candidate completion wrapper; its complete canonical bytes define the same current catalog identity.</summary>
        public static RetainedCompletionRewardBinding FromWrapper(string policyVersion,byte[] source,ICanonicalJson json,CatalogIdentity expected)
        {
            var wrapper=BindingReaders.Document(source,json,expected.HashSha256);var f=wrapper.Fields("reviewCompletionContractVersion","completionCatalog","starTimes");f[0].RequireInteger(1);
            var inner=FromDocument(policyVersion,f[1].Bytes,json,expected.Use);LocalSaveCodec.Require(inner.Catalog.CatalogId==expected.CatalogId&&inner.Catalog.Revision==expected.Revision&&inner.Catalog.ApprovalStatus==expected.ApprovalStatus,"SAVE_COMPLETION_BINDING_MISMATCH",true);
            var ids=new HashSet<string>(StringComparer.Ordinal);foreach(var time in f[2].Array()){var s=time.Fields("puzzleId","twoStarMs","threeStarMs");LocalSaveCodec.Require(ids.Add(s[0].String())&&s[1].IsNull==s[2].IsNull,"SAVE_COMPLETION_BINDING_INVALID");if(!s[1].IsNull)LocalSaveCodec.Require(s[1].Integer()>0&&s[2].Integer()>0&&s[2].Integer()<=s[1].Integer(),"SAVE_COMPLETION_BINDING_INVALID");}
            return new RetainedCompletionRewardBinding(policyVersion,expected,inner.SubjectKind,inner.AmountPatience,inner.RewardAmounts);
        }
        /// <summary>Reads the complete ADR044 runtime wrapper with exact calibration and completion source binding.</summary>
        public static RetainedCompletionRewardBinding FromRuntimeWrapper(string policyVersion,byte[] source,ICanonicalJson json,CatalogIdentity expected,IPublicPuzzleCatalog corpus)
        {
            LocalSaveCodec.Require(expected.Use==corpus.Identity.Use&&(expected.Use==CatalogUse.Production||expected.Use==CatalogUse.ContractTest),"SAVE_CATALOG_USE_MISMATCH",true);var root=BindingReaders.Document(source,json,expected.HashSha256);var w=root.Fields("runtimeCompletionContractVersion","completionCatalog","calibrationCatalog");w[0].RequireInteger(1);var c=w[1].Fields("schemaVersion","catalogId","catalogRevision","approvalStatus","rewardDefinitions","completionMoments");c[0].RequireInteger(1);var inner=BindingReaders.Catalog(w[1],json,expected.Use);LocalSaveCodec.Require(inner.CatalogId==expected.CatalogId&&inner.Revision==expected.Revision&&inner.ApprovalStatus==expected.ApprovalStatus,"SAVE_COMPLETION_BINDING_MISMATCH",true);RetainedCalibratedEndlessBinding.ValidateCalibration(w[2],corpus,json);
            var definitions=c[4].Array();string kind=definitions.Count==4?"ENDLESS_INSTANCE":definitions.Count==6?"LEVEL":throw new SaveCodecFailure("SAVE_COMPLETION_BINDING_INVALID");string[] reasons=kind=="LEVEL"?new[]{"LEVEL_FIRST_CLEAR","LEVEL_SECOND_STAR","LEVEL_THIRD_STAR","ROUTE_CLEAR","SECTION_CLEAR","POST_CLEAR_PATIENCE"}:new[]{"LEVEL_FIRST_CLEAR","LEVEL_SECOND_STAR","LEVEL_THIRD_STAR","POST_CLEAR_PATIENCE"};long[] amounts=kind=="LEVEL"?new long[]{15,5,10,60,150,10}:new long[]{15,5,10,10};var rewards=new Dictionary<string,long>(StringComparer.Ordinal);foreach(var d in definitions){var a=d.Fields("reasonCode","subjectKind","amountPatience","claimCardinality");string reason=a[0].String();int index=Array.IndexOf(reasons,reason);LocalSaveCodec.Require(index>=0&&!rewards.ContainsKey(reason)&&a[2].Integer()==amounts[index],"SAVE_REWARD_AMOUNT_MISMATCH");a[1].RequireString(kind=="ENDLESS_INSTANCE"?kind:reason=="ROUTE_CLEAR"?"ROUTE":reason=="SECTION_CLEAR"?"SECTION":"LEVEL");a[3].RequireString("ONCE_PER_SUBJECT");rewards.Add(reason,amounts[index]);}
            var moments=c[5].Array();LocalSaveCodec.Require(moments.Count==(kind=="LEVEL"?240:1),"SAVE_COMPLETION_BINDING_INVALID");var ids=new HashSet<string>(StringComparer.Ordinal);var maps=new HashSet<string>(StringComparer.Ordinal);var trains=new HashSet<string>(StringComparer.Ordinal);foreach(var moment in moments){var m=moment.Fields("id","resultTextKey","mapSegmentId","trainMomentId");BindingReaders.Stable(m[0].String());BindingReaders.Key(m[1].String());BindingReaders.Stable(m[2].String());BindingReaders.Stable(m[3].String());LocalSaveCodec.Require(ids.Add(m[0].String())&&maps.Add(m[2].String())&&trains.Add(m[3].String()),"SAVE_COMPLETION_BINDING_INVALID");}return new RetainedCompletionRewardBinding(policyVersion,expected,kind,10,rewards);
        }
        /// <summary>Known passive reader version.</summary>
        public string PolicyVersion{get;}
        /// <summary>Exact full completion document identity.</summary>
        public CatalogIdentity Catalog{get;}
        /// <summary>Actual persisted definition scope.</summary>
        public string SubjectKind{get;}
        /// <summary>Actual bound amount, never from a provider callback.</summary>
        public long AmountPatience{get;}
    }
    /// <summary>Explicit pair of immutable verified historical progress catalogs, with no fallback to today's values.</summary>
    public sealed class RetainedProgressCatalogBinding
    {
        /// <summary>Binds exact locked campaign membership and completion values under the same isolated use.</summary>
        public RetainedProgressCatalogBinding(ILockedCampaignCatalog campaign,ILockedCompletionCatalog completion)
        {Campaign=campaign??throw new ArgumentNullException(nameof(campaign));Completion=completion??throw new ArgumentNullException(nameof(completion));if(campaign.Identity.Use!=completion.Identity.Use)throw new ArgumentException("SAVE_CATALOG_USE_MISMATCH");}
        /// <summary>Immutable actual historical membership.</summary>
        public ILockedCampaignCatalog Campaign{get;}
        /// <summary>Immutable actual historical completion values and provenance.</summary>
        public ILockedCompletionCatalog Completion{get;}
    }
    /// <summary>Explicit immutable passive reader registration. Empty runtime entries invent no historical or product policy.</summary>
    public sealed class RetainedContractBindings
    {
        /// <summary>Copies narrowly enumerated known test reader descriptors; nonempty test registrations are forbidden in production and review entries.</summary>
        public RetainedContractBindings(CatalogUse use,IReadOnlyList<string> hintPolicies,IReadOnlyList<string> dailyPolicies,IReadOnlyList<string> accessibilityRevisions,IReadOnlyList<RetainedPrivacyBinding> privacy,IReadOnlyList<RetainedIapBinding> iap,IReadOnlyList<RetainedLegacyOwnershipReader> legacy,IReadOnlyList<RetainedCosmeticsBinding> cosmetics,IReadOnlyList<string> rewardPolicies,IReadOnlyList<string> generatorGrammars,IReadOnlyList<string> locales,IReadOnlyList<string> audioGroups,IReadOnlyList<RetainedCompletionRewardBinding>? completionRewards=null,IReadOnlyList<RetainedProgressCatalogBinding>? historicalProgress=null)
        {
            if(hintPolicies==null||dailyPolicies==null||accessibilityRevisions==null||privacy==null||iap==null||legacy==null||cosmetics==null||rewardPolicies==null||generatorGrammars==null||locales==null||audioGroups==null)throw new ArgumentNullException();Use=use;HintPolicies=Copy(hintPolicies);DailyPolicies=Copy(dailyPolicies);AccessibilityRevisions=Copy(accessibilityRevisions);Privacy=Copy(privacy);Iap=Copy(iap);Legacy=Copy(legacy);Cosmetics=Copy(cosmetics);RewardPolicies=Copy(rewardPolicies);GeneratorGrammars=Copy(generatorGrammars);Locales=Copy(locales);AudioGroups=Copy(audioGroups);CompletionRewards=Copy(completionRewards??Array.Empty<RetainedCompletionRewardBinding>());HistoricalProgress=Copy(historicalProgress??Array.Empty<RetainedProgressCatalogBinding>());if(HistoricalProgress.Any(h=>h.Campaign.Identity.Use!=use||h.Completion.Identity.Use!=use))throw new ArgumentException("SAVE_CATALOG_USE_MISMATCH");
            if(use!=CatalogUse.ContractTest&&hintPolicies.Count+dailyPolicies.Count+accessibilityRevisions.Count+privacy.Count+iap.Count+legacy.Count+cosmetics.Count+rewardPolicies.Count+generatorGrammars.Count+locales.Count+audioGroups.Count+CompletionRewards.Count+HistoricalProgress.Count>0)throw new ArgumentException("SAVE_TEST_READER_PROMOTION_FORBIDDEN");
        }
        private static IReadOnlyList<T> Copy<T>(IReadOnlyList<T> list){if(list.Any(x=>x==null))throw new ArgumentException("SAVE_NULL_ITEM");return Array.AsReadOnly(list.ToArray());}
        /// <summary>Creates no registered passive policies for a real runtime entry.</summary>
        public static RetainedContractBindings Empty(CatalogUse use)=>new RetainedContractBindings(use,Array.Empty<string>(),Array.Empty<string>(),Array.Empty<string>(),Array.Empty<RetainedPrivacyBinding>(),Array.Empty<RetainedIapBinding>(),Array.Empty<RetainedLegacyOwnershipReader>(),Array.Empty<RetainedCosmeticsBinding>(),Array.Empty<string>(),Array.Empty<string>(),Array.Empty<string>(),Array.Empty<string>());
        /// <summary>Registers only complete cosmetics/campaign source bytes matching the exact injected locks; test-reader promotion remains forbidden.</summary>
        public static RetainedContractBindings ForCosmetics(CatalogIdentity expectedCosmetics,CatalogIdentity expectedCampaign,byte[] cosmeticsSource,byte[] campaignSource,IPublicPuzzleCatalog puzzles,ICanonicalJson json)
        {
            if(expectedCosmetics==null||expectedCampaign==null||puzzles==null||json==null)throw new ArgumentNullException();
            LocalSaveCodec.Require(expectedCosmetics.Use!=CatalogUse.IsolatedReview&&expectedCosmetics.Use==expectedCampaign.Use&&expectedCampaign.Use==puzzles.Identity.Use,"SAVE_CATALOG_USE_MISMATCH",true);
            var binding=RetainedCosmeticsBinding.FromDocuments(cosmeticsSource,campaignSource,json,expectedCosmetics.Use,puzzles);
            LocalSaveCodec.Require(binding.Identity.Equals(expectedCosmetics)&&binding.Campaign.Identity.Equals(expectedCampaign),"SAVE_COSMETIC_CATALOG_BINDING_MISMATCH",true);
            var result=Empty(expectedCosmetics.Use);result.Cosmetics=Array.AsReadOnly(new[]{binding});return result;
        }
        /// <summary>Registers the exact current launch privacy/IAP contracts, preserving optional separately verified cosmetic locks.</summary>
        public static RetainedContractBindings ForRuntime(CatalogUse use,RetainedContractBindings? cosmetics=null)
        {
            LocalSaveCodec.Require(use==CatalogUse.Production||use==CatalogUse.ContractTest||use==CatalogUse.InternalPreview,"SAVE_REVIEW_METAGAME_PROMOTION_FORBIDDEN",true);
            if(cosmetics!=null)LocalSaveCodec.Require(cosmetics.Use==use&&cosmetics.HintPolicies.Count+cosmetics.DailyPolicies.Count+cosmetics.AccessibilityRevisions.Count+cosmetics.Privacy.Count+cosmetics.Iap.Count+cosmetics.Legacy.Count+cosmetics.RewardPolicies.Count+cosmetics.GeneratorGrammars.Count+cosmetics.Locales.Count+cosmetics.AudioGroups.Count+cosmetics.CompletionRewards.Count+cosmetics.HistoricalProgress.Count+cosmetics.CalibratedEndless.Count==0,"SAVE_TEST_READER_PROMOTION_FORBIDDEN",true);
            if(cosmetics!=null)LocalSaveCodec.Require(cosmetics.Cosmetics.All(c=>c.Identity.Use==use&&c.Campaign.Identity.Use==use),"SAVE_CATALOG_USE_MISMATCH",true);
            var result=Empty(use);
            result.Cosmetics=cosmetics?.Cosmetics??Array.Empty<RetainedCosmeticsBinding>();
            result.Privacy=Array.AsReadOnly(new[]{new RetainedPrivacyBinding(MobilePersistenceContract.PrivacyPolicyRevision,MobilePersistenceContract.SdkContractRevision,MobilePersistenceContract.ConsentSource)});
            result.Iap=Array.AsReadOnly(new[]{new RetainedIapBinding(MobilePersistenceContract.IapPolicyVersion,"GooglePlay","remove_ads"),new RetainedIapBinding(MobilePersistenceContract.IapPolicyVersion,"AppleAppStore","remove_ads")});
            result.Locales=Array.AsReadOnly(new[]{"de-DE"});
            result.AudioGroups=Array.AsReadOnly(new[]{"Master","Music","SFX","Ambience","UI"});
            result.AccessibilityRevisions=Array.AsReadOnly(new[]{LocalSettingsPolicy.AccessibilityRevision});
            return result;
        }
        /// <summary>Composes only independently verified exact completion reward sources; no generic test-policy promotion.</summary>
        public static RetainedContractBindings ForRuntimeWithRewards(CatalogUse use,RetainedContractBindings? cosmetics,IReadOnlyList<RetainedCompletionRewardBinding> rewards)
        {
            var result=ForRuntime(use,cosmetics);LocalSaveCodec.Require(rewards!=null&&rewards.All(r=>r.Catalog.Use==use&&r.PolicyVersion==LocalPostClearPolicy.PolicyVersion&&r.AmountPatience==10)&&rewards.Select(r=>r.Catalog.CatalogId+":"+r.Catalog.Revision.ToString(CultureInfo.InvariantCulture)).Distinct(StringComparer.Ordinal).Count()==rewards.Count,"SAVE_COMPLETION_BINDING_MISMATCH",true);
            result.CompletionRewards=Copy(rewards!);result.RewardPolicies=Array.AsReadOnly(new[]{LocalPostClearPolicy.PolicyVersion});return result;
        }
        public static RetainedContractBindings ForRuntimeWithCalibratedEndless(CatalogUse use,RetainedContractBindings? cosmetics,IReadOnlyList<RetainedCompletionRewardBinding> rewards,IReadOnlyList<RetainedCalibratedEndlessBinding> profiles)
        {var result=ForRuntimeWithRewards(use,cosmetics,rewards);LocalSaveCodec.Require((use==CatalogUse.Production||use==CatalogUse.ContractTest)&&profiles!=null&&profiles.Count>0&&profiles.All(p=>p.CompletionCatalog.Use==use&&rewards.Any(r=>r.Catalog.Equals(p.CompletionCatalog)&&r.SubjectKind=="ENDLESS_INSTANCE"))&&profiles.Select(p=>p.Profile.Id+":"+p.Profile.HashSha256).Distinct(StringComparer.Ordinal).Count()==profiles.Count,"SAVE_CALIBRATION_BINDING_MISMATCH",true);result.CalibratedEndless=Copy(profiles!);return result;}
        /// <summary>Exact isolated consumer use.</summary>
        public CatalogUse Use{get;}
        /// <summary>Known passive hint policy IDs.</summary>
        public IReadOnlyList<string> HintPolicies{get;}
        /// <summary>Known passive daily policy IDs.</summary>
        public IReadOnlyList<string> DailyPolicies{get;}
        /// <summary>Known accessibility source revisions.</summary>
        public IReadOnlyList<string> AccessibilityRevisions{get;private set;}
        /// <summary>Known privacy tuples.</summary>
        public IReadOnlyList<RetainedPrivacyBinding> Privacy{get;private set;}
        /// <summary>Known passive store/product tuples.</summary>
        public IReadOnlyList<RetainedIapBinding> Iap{get;private set;}
        /// <summary>Known explicit legacy provenance readers.</summary>
        public IReadOnlyList<RetainedLegacyOwnershipReader> Legacy{get;}
        /// <summary>Verified immutable cosmetic document bindings.</summary>
        public IReadOnlyList<RetainedCosmeticsBinding> Cosmetics{get;private set;}
        /// <summary>Known retained reward reader versions.</summary>
        public IReadOnlyList<string> RewardPolicies{get;private set;}
        /// <summary>Known complete closed generator parameter grammars.</summary>
        public IReadOnlyList<string> GeneratorGrammars{get;}
        /// <summary>Known retained locale identifiers.</summary>
        public IReadOnlyList<string> Locales{get;private set;}
        /// <summary>Known rational audio preference group identifiers.</summary>
        public IReadOnlyList<string> AudioGroups{get;private set;}
        /// <summary>Exact complete retained POST_CLEAR reward definitions.</summary>
        public IReadOnlyList<RetainedCompletionRewardBinding> CompletionRewards{get;private set;}
        /// <summary>Explicit exact historical campaign/completion pairs; absence is unsupported rather than an invented reward value.</summary>
        public IReadOnlyList<RetainedProgressCatalogBinding> HistoricalProgress{get;}
        public IReadOnlyList<RetainedCalibratedEndlessBinding> CalibratedEndless{get;private set;}=Array.Empty<RetainedCalibratedEndlessBinding>();
        internal void RequireGenerator(string grammar){LocalSaveCodec.Require((Use==CatalogUse.InternalPreview||CalibratedEndless.Count>0)&&grammar==PuzzleCandidateSearch.GeneratorVersion||Use==CatalogUse.ContractTest&&grammar=="contract-preservation-parameters-v1"&&GeneratorGrammars.Contains(grammar),"SAVE_GENERATOR_PARAMETERS_UNSUPPORTED",true);}
        internal void Validate(LocalProfileState state,ICanonicalJson json)
        {
            void Known(bool b){LocalSaveCodec.Require(b,"SAVE_UNSUPPORTED_POLICY",true);}
            if(state.HintEntitlement!=null)Known(HintPolicies.Contains(state.HintEntitlement.PolicyVersion));if(state.DailyStatus!=null)Known(DailyPolicies.Contains(state.DailyStatus.PolicyVersion));if(state.Settings.Accessibility!=null)Known(AccessibilityRevisions.Contains(state.Settings.Accessibility.ContractRevision));if(state.Settings.Locale!=null)Known(Locales.Contains(state.Settings.Locale));Known(state.Settings.AudioGroups.All(a=>AudioGroups.Contains(a.GroupId)));
            var privacy=state.DataLifecycle.PrivacyDecision;if(privacy!=null)Known(Privacy.Any(p=>p.PolicyRevision==privacy.PolicyRevision&&p.SdkContractRevision==privacy.SdkContractRevision&&p.Source==privacy.Source));
            foreach(var o in state.PendingOperations.OfType<LocalIapOperation>())Known(Iap.Any(p=>p.Store==o.Store&&p.LogicalProductKey==o.LogicalProductKey));foreach(var e in state.Entitlements.Records)Known(Iap.Any(p=>p.Store==e.Store&&p.LogicalProductKey==e.ProductKey));if(state.PendingOperations.OfType<LocalRewardOperation>().Any()||state.RewardClaims.OfType<PostClearRewardClaim>().Any())Known(RewardPolicies.Count>0);
            foreach(var claim in state.RewardClaims.OfType<PostClearRewardClaim>())
            {
                var definitions=CompletionRewards.Where(d=>d.Catalog.Equals(claim.CompletionCatalog)&&RewardPolicies.Contains(d.PolicyVersion)).ToArray();Known(definitions.Length==1);var definition=definitions[0];LocalSaveCodec.Require(definition.SubjectKind==(claim.PuzzleIdentity.PuzzleId.StartsWith("E1-",StringComparison.Ordinal)?"ENDLESS_INSTANCE":"LEVEL"),"SAVE_REWARD_SCOPE_MISMATCH");if(claim.Terminal!=null)LocalSaveCodec.Require(claim.Terminal.Amount==definition.AmountPatience&&claim.Terminal.ReasonCode=="POST_CLEAR_PATIENCE","SAVE_REWARD_AMOUNT_MISMATCH");
            }
            foreach(var ownership in state.Inventory.Ownership)
            {
                if(ownership is LegacyOwnership legacy){Known(Legacy.Any(r=>r.SourceSaveSchemaVersion==1&&r.ItemTypes.Contains(legacy.ItemType)));continue;}
                CatalogIdentity catalog=ownership is DefaultOwnership d?d.Catalog:ownership is PurchasedOwnership p?p.Catalog:((MilestoneOwnership)ownership).Catalog;var binding=Cosmetics.SingleOrDefault(c=>c.Identity.Equals(catalog));Known(binding!=null);var item=binding!.Items.SingleOrDefault(i=>i.ItemId==ownership.ItemId);Known(item!=null);LocalSaveCodec.Require(item!.ItemType==ownership.ItemType,"SAVE_COSMETIC_TYPE_MISMATCH");
                if(ownership is DefaultOwnership)LocalSaveCodec.Require(item.Acquisition=="DEFAULT","SAVE_COSMETIC_ACQUISITION_MISMATCH");else if(ownership is PurchasedOwnership purchase)LocalSaveCodec.Require(item.Acquisition=="PATIENCE_PURCHASE"&&item.PricePatience==purchase.PricePatience,"SAVE_COSMETIC_PRICE_MISMATCH");else if(ownership is MilestoneOwnership milestone)Eligibility(binding,item,milestone.EligibilityProjectionHashSha256,state,json);
            }
            foreach(var reservation in state.CosmeticClaimReservations){var binding=Cosmetics.SingleOrDefault(c=>c.Identity.Equals(reservation.Catalog));Known(binding!=null);var item=binding!.Items.SingleOrDefault(i=>i.ItemId==reservation.ItemId);Known(item!=null);Eligibility(binding,item!,reservation.EligibilityProjectionHashSha256,state,json);}
            foreach(var e in state.Endless.OpenEndless){RequireGenerator(e.Descriptor.GeneratorVersion);var d=e.Descriptor;if(d.Parameters is SearchGeneratorParameters search){ValidateSearchDescriptor(state,e,search,json);continue;}LocalSaveCodec.Require(d.Parameters is ContractGeneratorParameters,"SAVE_GENERATOR_PARAMETERS_UNSUPPORTED",true);var p=(ContractGeneratorParameters)d.Parameters;string parameters=LocalSaveCodec.Object(("gridWidth",LocalSaveCodec.Write(p.GridWidth)),("gridHeight",LocalSaveCodec.Write(p.GridHeight)));LocalSaveCodec.Require(LocalSaveHash.Bytes(LocalSaveHash.Canonical(json,parameters))==d.ParameterHashSha256,"SAVE_PARAMETER_HASH_MISMATCH");string projection=LocalSaveCodec.Object(("endlessContractVersion","1"),("rulesetVersion",LocalSaveCodec.Q(d.RulesetVersion)),("generatorVersion",LocalSaveCodec.Q(d.GeneratorVersion)),("seed",LocalSaveCodec.Q(d.Seed)),("generationOrdinal",LocalSaveCodec.Q(d.GenerationOrdinal)),("parameterHashSha256",LocalSaveCodec.Q(d.ParameterHashSha256)));LocalSaveCodec.Require(e.Id=="E1-"+LocalSaveHash.Bytes(LocalSaveHash.Canonical(json,projection)),"SAVE_ENDLESS_ID_MISMATCH");if(e is OpenEndlessCompletion c&&c.Claim.ClaimStatus!="LOCAL_DECISION_PENDING")Known(RewardPolicies.Count>0);}
            foreach(var operation in state.PendingOperations.OfType<LocalRewardOperation>()){var open=state.Endless.OpenEndless.OfType<OpenEndlessCompletion>().SingleOrDefault(c=>c.Claim.LocalOperationId==operation.OperationId);if(open!=null)LocalSaveCodec.Require(operation.State==open.Claim.ClaimStatus||operation.State=="RESERVED"&&open.Claim.ClaimStatus=="PROVIDER_RESERVED","SAVE_REWARD_OPERATION_STATE_MISMATCH");else{var claim=state.RewardClaims.OfType<PostClearRewardClaim>().Single(c=>c.LocalOperationId==operation.OperationId);LocalSaveCodec.Require(claim.ClaimStatus==operation.State,"SAVE_REWARD_OPERATION_STATE_MISMATCH");}}
        }
        private void ValidateSearchDescriptor(LocalProfileState state,LocalOpenEndless open,SearchGeneratorParameters search,ICanonicalJson json)
        {
            LocalSaveCodec.Require((search.StarCalibration==null?Use==CatalogUse.InternalPreview:CalibratedEndless.Count(p=>p.Matches(search))==1)&&open.Descriptor.GeneratorVersion==PuzzleCandidateSearch.GeneratorVersion&&search.CampaignCatalog.Equals(state.Profile.CatalogSet.PublicCatalog),"SAVE_GENERATOR_REFERENCE_UNSUPPORTED",true);
            LocalSaveCodec.Require(LocalSaveHash.Bytes(LocalSaveHash.Canonical(json,search.Projection()))==open.Descriptor.ParameterHashSha256&&EndlessProjection.DescriptorId(open.Descriptor,json)==open.Id,"SAVE_ENDLESS_ID_MISMATCH");
            foreach(var p in search.RecentPublicInputs)EndlessProjection.ValidatePublic(p,json);
            if(open is OpenEndlessCompletion done)
            {
                LocalSaveCodec.Require((search.StarCalibration==null)==(done.Result==null),"SAVE_ENDLESS_RESULT_REQUIRED");
                string hash;try{hash=LocalEndlessPolicy.CompletionSourceHash(done.Claim);}catch(ArgumentException){throw new SaveCodecFailure("SAVE_ENDLESS_COMPLETION_SOURCE_UNSUPPORTED",true);}
                var sources=CompletionRewards.Where(r=>r.PolicyVersion==LocalPostClearPolicy.PolicyVersion&&r.SubjectKind=="ENDLESS_INSTANCE"&&r.Catalog.Use==Use&&(search.StarCalibration==null?r.Catalog.CatalogId=="completion-qa-mobile-candidate-endless":r.Catalog.Equals(search.StarCalibration.CompletionCatalog))&&r.Catalog.HashSha256==hash).ToArray();
                LocalSaveCodec.Require(sources.Length==1,"SAVE_ENDLESS_COMPLETION_SOURCE_UNSUPPORTED",true);
                LocalSaveCodec.Require(done.CompletionCommitId.StartsWith("puzzle-solved:",StringComparison.Ordinal)&&Guid.TryParseExact(done.CompletionCommitId.Substring("puzzle-solved:".Length),"D",out _),"SAVE_ENDLESS_COMPLETION_ATTEMPT_INVALID");
                if(done.Claim.ProviderOperationId!=null)
                {var f=done.Claim.ProviderOperationId.Split(':');LocalSaveCodec.Require(f.Length==3&&f[0]=="endless-provider-v1"&&long.TryParse(f[1],NumberStyles.None,CultureInfo.InvariantCulture,out long gen)&&gen>0&&Guid.TryParseExact(f[2],"D",out _),"SAVE_ENDLESS_PROVIDER_BINDING_INVALID");}
            }
        }
        private static void Eligibility(RetainedCosmeticsBinding binding,RetainedCosmeticItem item,string expected,LocalProfileState state,ICanonicalJson json)
        {LocalSaveCodec.Require(item.Acquisition=="MILESTONE_GRANT","SAVE_COSMETIC_ACQUISITION_MISMATCH");var subjects=item.RequiredCampaignSubjectIds.OrderBy(s=>s,StringComparer.Ordinal).ToArray();var required=subjects.SelectMany(s=>BindingReaders.Expand(binding.Campaign,s)).Distinct().OrderBy(p=>p.PuzzleId,StringComparer.Ordinal).ToArray();var valid=required.Where(id=>state.Levels.Any(l=>l.Identity.Equals(id)&&l.FirstClear!=null)).Select(i=>i.PuzzleId).ToArray();LocalSaveCodec.Require(required.Length>0&&required.Length==valid.Length,"SAVE_COSMETIC_ELIGIBILITY_INVALID");string Strings(IEnumerable<string> s)=>"["+string.Join(",",s.Select(LocalSaveCodec.Q))+"]";string projection=LocalSaveCodec.Object(("catalogId",LocalSaveCodec.Q(binding.Identity.CatalogId)),("catalogRevision",LocalSaveCodec.Write(binding.Identity.Revision)),("catalogHashSha256",LocalSaveCodec.Q(binding.Identity.HashSha256)),("itemId",LocalSaveCodec.Q(item.ItemId)),("acquisition",LocalSaveCodec.Q(item.Acquisition)),("eligibilityContractVersion","1"),("kind",LocalSaveCodec.Q("ALL_FIRST_CLEARS_OF_CAMPAIGN_SUBJECTS")),("requiredCampaignSubjectIds",Strings(subjects)),("requiredPuzzleIds",Strings(required.Select(i=>i.PuzzleId))),("validatedFirstClearPuzzleIds",Strings(valid)));LocalSaveCodec.Require(LocalSaveHash.Bytes(LocalSaveHash.Canonical(json,projection))==expected,"SAVE_COSMETIC_ELIGIBILITY_HASH_MISMATCH");}
    }
    internal static class BindingReaders
    {
        internal static CanonicalDtoCursor Document(byte[] source,ICanonicalJson json,string? expected=null)
        {var result=json.Canonicalize(source,LocalSaveHash.Limits);LocalSaveCodec.Require(result.Succeeded,result.ErrorCode??"SAVE_BINDING_JSON_INVALID",true);var bytes=result.GetUtf8()!;if(expected!=null)LocalSaveCodec.Require(LocalSaveHash.Bytes(bytes)==expected,"SAVE_LEVEL_IDENTITY_UNRESOLVED",true);return CanonicalDtoCursor.FromCanonical(bytes);}
        internal static CatalogIdentity Catalog(CanonicalDtoCursor v,ICanonicalJson json,CatalogUse use)
        {string status=v.Find("approvalStatus").String();var approval=status=="DRAFT"?CatalogApprovalStatus.Draft:status=="FIXTURE_ONLY"?CatalogApprovalStatus.FixtureOnly:status=="PRODUCT_APPROVED"?CatalogApprovalStatus.ProductApproved:throw new SaveCodecFailure("SAVE_CATALOG_STATUS_UNSUPPORTED",true);return new CatalogIdentity(v.Find("catalogId").String(),v.Find("catalogRevision").Int32(),LocalSaveHash.Bytes(v.Bytes),use,approval);}
        internal static void Stable(string value){LocalSaveCodec.Require(System.Text.RegularExpressions.Regex.IsMatch(value,@"^[a-z][a-z0-9-]*$"),"SAVE_BINDING_ID_INVALID");}
        internal static void Key(string value){LocalSaveCodec.Require(System.Text.RegularExpressions.Regex.IsMatch(value,@"^[a-z][a-z0-9-]*(\.[a-z0-9-]+)+$"),"SAVE_BINDING_KEY_INVALID");}
        private sealed class CampaignSource : ILockedCampaignCatalog
        {
            internal CampaignSource(CatalogIdentity identity,LockedCampaignEntry[] entries){Identity=identity;Entries=Array.AsReadOnly(entries);}
            public CatalogIdentity Identity{get;}
            public IReadOnlyList<LockedCampaignEntry> Entries{get;}
            public bool TryGet(PublicPuzzleIdentity identity,out LockedCampaignEntry? value){value=Entries.SingleOrDefault(e=>e.Puzzle.Equals(identity));return value!=null;}
        }
        internal static IReadOnlyList<PublicPuzzleIdentity> Expand(ILockedCampaignCatalog c,string subject)=>c.Entries.Where(e=>e.SeasonId==subject||e.SectionId==subject||e.RouteId==subject||e.Puzzle.PuzzleId==subject).Select(e=>e.Puzzle).ToArray();
        internal static ILockedCampaignCatalog Campaign(byte[] document,ICanonicalJson json,CatalogUse use,IPublicPuzzleCatalog puzzles,string? expectedHash)
        {
            var root=Document(document,json,expectedHash);var f=root.Fields("schemaVersion","catalogId","catalogRevision","approvalStatus","seasons");int version=f[0].Int32();LocalSaveCodec.Require(version==1||version==2,"SAVE_CAMPAIGN_READER_UNSUPPORTED",true);var catalog=Catalog(root,json,use);var rows=new List<(PublicPuzzleIdentity Id,string Season,string Section,string Route,int Ordinal,string[] Required)>();var subjects=new HashSet<string>(StringComparer.Ordinal);var orders=new HashSet<string>(StringComparer.Ordinal);
            string[] Unlock(CanonicalDtoCursor v){var a=v.Fields("kind","requiredSubjectIds");var kind=a[0].String();var ids=a[1].Array().Select(x=>x.String()).ToArray();LocalSaveCodec.Require(kind=="ALWAYS"?ids.Length==0:kind=="ALL_FIRST_CLEARS","SAVE_CAMPAIGN_UNLOCK_INVALID");LocalSaveCodec.Require(ids.Distinct(StringComparer.Ordinal).Count()==ids.Length,"SAVE_DUPLICATE_ID");return ids;}
            foreach(var season in f[4].Array())
            {
                var s=season.Fields("id","order","titleKey","unlock","sections");string sid=s[0].String();LocalSaveCodec.Require(System.Text.RegularExpressions.Regex.IsMatch(sid,@"^S[1-9][0-9]*$")&&subjects.Add(sid)&&s[1].Integer()>0&&orders.Add("s:"+s[1].Raw),"SAVE_CAMPAIGN_BINDING_INVALID");Key(s[2].String());var sr=Unlock(s[3]);
                foreach(var section in s[4].Array())
                {
                    var n=section.Fields("id","order","titleKey","unlock","routes");string nid=n[0].String();LocalSaveCodec.Require(System.Text.RegularExpressions.Regex.IsMatch(nid,"^"+sid+@"-[0-9]{2}$")&&subjects.Add(nid)&&n[1].Integer()>0&&orders.Add(sid+":"+n[1].Raw),"SAVE_CAMPAIGN_BINDING_INVALID");Key(n[2].String());var nr=sr.Concat(Unlock(n[3])).ToArray();
                    foreach(var route in n[4].Array())
                    {
                        var r=route.Fields("id","order","titleKey","unlock","levels");string rid=r[0].String();LocalSaveCodec.Require(System.Text.RegularExpressions.Regex.IsMatch(rid,"^"+nid+@"-[0-9]{2}$")&&subjects.Add(rid)&&r[1].Integer()>0&&orders.Add(nid+":"+r[1].Raw),"SAVE_CAMPAIGN_BINDING_INVALID");Key(r[2].String());var rr=nr.Concat(Unlock(r[3])).ToArray();
                        foreach(var level in r[4].Array()){var l=level.Fields(version==1?"levelId":"puzzleId","order","unlock");string id=l[0].String();LocalSaveCodec.Require(System.Text.RegularExpressions.Regex.IsMatch(id,"^"+rid+@"-[0-9]{2}$")&&subjects.Add(id)&&l[1].Integer()>0&&orders.Add(rid+":"+l[1].Raw),"SAVE_CAMPAIGN_BINDING_INVALID");var item=puzzles.Puzzles.SingleOrDefault(p=>p.Identity.PuzzleId==id);LocalSaveCodec.Require(item!=null,"SAVE_LEVEL_IDENTITY_UNRESOLVED",true);rows.Add((item!.Identity,sid,nid,rid,l[1].Int32(),rr.Concat(Unlock(l[2])).Distinct(StringComparer.Ordinal).ToArray()));}
                    }
                }
            }
            LocalSaveCodec.Require(rows.Count>0,"SAVE_CAMPAIGN_BINDING_INVALID");var entries=rows.Select(row=>new LockedCampaignEntry(catalog,row.Id,row.Season,row.Section,row.Route,row.Ordinal,row.Required.SelectMany(subject=>{var expanded=rows.Where(x=>x.Season==subject||x.Section==subject||x.Route==subject||x.Id.PuzzleId==subject).Select(x=>x.Id.PuzzleId).ToArray();LocalSaveCodec.Require(expanded.Length>0,"SAVE_CAMPAIGN_REQUIREMENT_MISSING");return expanded;}).Distinct(StringComparer.Ordinal).OrderBy(id=>id,StringComparer.Ordinal).ToArray())).ToArray();return new CampaignSource(catalog,entries);
        }
        internal static PublicPuzzleInput PublicInput(CanonicalDtoCursor document,ICanonicalJson json,bool legacy)
        {
            string id=document.Find(legacy?"id":"puzzleId").String();string raw=LocalSaveCodec.Object(("puzzleId",LocalSaveCodec.Q(id)),("rulesetVersion",document.Find("rulesetVersion").Raw),("grid",document.Find("grid").Raw),("endpoints",document.Find("endpoints").Raw),("rowCounts",document.Find("rowCounts").Raw),("columnCounts",document.Find("columnCounts").Raw));var canonical=CanonicalDtoCursor.FromCanonical(LocalSaveHash.Canonical(json,raw));var f=canonical.Fields("puzzleId","rulesetVersion","grid","endpoints","rowCounts","columnCounts");var g=f[2].Fields("width","height");var ep=f[3].Fields("a","b");var a=ep[0].Fields("side","index");var b=ep[1].Fields("side","index");return new PublicPuzzleInput(new PublicPuzzleIdentity(id,"STP-PUZZLE-SEMANTIC-JCS-1",LocalSaveHash.Bytes(canonical.Bytes)),f[1].String(),g[0].Int32(),g[1].Int32(),a[0].EnumValue<PublicPuzzleSide>(),a[1].Int32(),b[0].EnumValue<PublicPuzzleSide>(),b[1].Int32(),f[4].Array().Select(x=>x.Int32()).ToArray(),f[5].Array().Select(x=>x.Int32()).ToArray());
        }
    }
    /// <summary>One verified neutral legacy-to-semantic document/lock binding, with no authoring bytes retained in the payload.</summary>
    public sealed class HistoricalLevelBinding
    {
        private HistoricalLevelBinding(string legacyId,string legacyHash,PublicPuzzleInput target,string sourceHash,string targetHash,string lockHash){LegacyLevelId=legacyId;LegacyPuzzleHashSha256=legacyHash;Target=target;SourceDocumentSha256=sourceHash;TargetDocumentSha256=targetHash;ReleaseLockSha256=lockHash;}
        /// <summary>Validates exact canonical document hashes, semantic neutrality and exactly one matching legacy lock entry.</summary>
        public static HistoricalLevelBinding Create(byte[] sourceDocument,string expectedSourceDocumentSha256,byte[] targetDocument,string expectedTargetDocumentSha256,byte[] releaseLock,string expectedReleaseLockSha256,ICanonicalJson json)
        {
            var source=BindingReaders.Document(sourceDocument,json,expectedSourceDocumentSha256);var target=BindingReaders.Document(targetDocument,json,expectedTargetDocumentSha256);var locked=BindingReaders.Document(releaseLock,json,expectedReleaseLockSha256);
            source.Fields("schemaVersion","rulesetVersion","id","contentRevision","grid","endpoints","rowCounts","columnCounts","content","production","completion","solution","validation");source.Find("schemaVersion").RequireInteger(1);target.Fields("documentSchemaVersion","rulesetVersion","puzzleId","contentRevision","grid","endpoints","rowCounts","columnCounts","content","production","completion","solution","proofRef");target.Find("documentSchemaVersion").RequireInteger(2);
            var oldPublic=BindingReaders.PublicInput(source,json,true);var current=BindingReaders.PublicInput(target,json,false);LocalSaveCodec.Require(oldPublic.Identity.Equals(current.Identity),"SAVE_LEVEL_IDENTITY_UNRESOLVED",true);string legacyProjection=LocalSaveCodec.Object(("schemaVersion","1"),("rulesetVersion",source.Find("rulesetVersion").Raw),("id",source.Find("id").Raw),("grid",source.Find("grid").Raw),("endpoints",source.Find("endpoints").Raw),("rowCounts",source.Find("rowCounts").Raw),("columnCounts",source.Find("columnCounts").Raw));string legacyHash=LocalSaveHash.Bytes(LocalSaveHash.Canonical(json,legacyProjection));LocalSaveCodec.Require(source.Find("validation").Find("puzzleHashSha256").String()==legacyHash,"SAVE_LEVEL_IDENTITY_UNRESOLVED",true);
            var lockFields=locked.Fields("lockFormatVersion","releaseId","gitCommit","toolchainLockSha256","catalogHashes","puzzles");lockFields[0].RequireInteger(1);LocalSaveCodec.Require(System.Text.RegularExpressions.Regex.IsMatch(lockFields[1].String(),@"^(fixture-.+|v[0-9]+\.[0-9]+\.[0-9]+(?:[-+].+)?)$"),"SAVE_LEVEL_IDENTITY_UNRESOLVED",true);var matching=new List<CanonicalDtoCursor>();
            foreach(var puzzle in lockFields[5].Array())foreach(var binding in puzzle.Find("legacyBindings").Array()){var b=binding.Fields("profile","sha256");if(b[0].String()=="STP-LEVEL-V1-PUZZLE-JCS-1"&&b[1].String()==legacyHash)matching.Add(puzzle);}
            LocalSaveCodec.Require(matching.Count==1,"SAVE_LEVEL_IDENTITY_UNRESOLVED",true);var entry=matching[0];LocalSaveCodec.Require(entry.Find("puzzleId").String()==current.Identity.PuzzleId&&entry.Find("documentSchemaVersion").Integer()==2&&entry.Find("documentSha256").String()==expectedTargetDocumentSha256,"SAVE_LEVEL_IDENTITY_UNRESOLVED",true);var ph=entry.Find("publicPuzzleHash").Fields("profile","sha256");LocalSaveCodec.Require(ph[0].String()==current.Identity.HashProfile&&ph[1].String()==current.Identity.HashSha256,"SAVE_LEVEL_IDENTITY_UNRESOLVED",true);return new HistoricalLevelBinding(oldPublic.Identity.PuzzleId,legacyHash,current,expectedSourceDocumentSha256,expectedTargetDocumentSha256,expectedReleaseLockSha256);
        }
        /// <summary>Original stable legacy ID.</summary>
        public string LegacyLevelId{get;}
        /// <summary>Original profiled legacy puzzle hash.</summary>
        public string LegacyPuzzleHashSha256{get;}
        /// <summary>Complete immutable neutral target public input.</summary>
        public PublicPuzzleInput Target{get;}
        /// <summary>Exact trusted source document canonical hash.</summary>
        public string SourceDocumentSha256{get;}
        /// <summary>Exact trusted target document canonical hash.</summary>
        public string TargetDocumentSha256{get;}
        /// <summary>Exact trusted historical release-lock canonical hash.</summary>
        public string ReleaseLockSha256{get;}
    }
    /// <summary>Immutable known full-v1 reader provenance for one historical profile/catalog binding.</summary>
    public sealed class HistoricalProfileBinding
    {
        /// <summary>Copies explicit closed source metadata; no current profile, policy, ledger algorithm or level mapping is guessed.</summary>
        public HistoricalProfileBinding(string sourceLocalProfileId,string sourceContentCatalogHash,SaveProfileIdentity destinationProfileIdentity,CatalogSetIdentity destinationCatalogSet,IReadOnlyList<HistoricalLevelBinding> levels,ILockedCampaignCatalog sourceCampaign,string ledgerAlgorithm)
        {SourceLocalProfileId=sourceLocalProfileId??throw new ArgumentNullException(nameof(sourceLocalProfileId));SourceContentCatalogHash=LocalSaveCodec.Hash(sourceContentCatalogHash);DestinationProfileIdentity=destinationProfileIdentity??throw new ArgumentNullException(nameof(destinationProfileIdentity));DestinationCatalogSet=destinationCatalogSet??throw new ArgumentNullException(nameof(destinationCatalogSet));if(levels==null||levels.Any(x=>x==null))throw new ArgumentNullException(nameof(levels));Levels=Array.AsReadOnly(levels.ToArray());SourceCampaign=sourceCampaign??throw new ArgumentNullException(nameof(sourceCampaign));LedgerAlgorithm=ledgerAlgorithm??throw new ArgumentNullException(nameof(ledgerAlgorithm));LocalSaveCodec.Require(SourceCampaign.Identity.HashSha256==SourceContentCatalogHash&&SourceCampaign.Identity.Use==destinationProfileIdentity.Use,"SAVE_LEVEL_IDENTITY_UNRESOLVED",true);}
        /// <summary>Creates a fully parsed source campaign from exact bytes and expected historical hash.</summary>
        public static ILockedCampaignCatalog ReadSourceCampaign(byte[] source,string expectedHash,ICanonicalJson json,CatalogUse use,IPublicPuzzleCatalog targetPublic)=>BindingReaders.Campaign(source,json,use,targetPublic,expectedHash);
        /// <summary>Original logical source profile ID.</summary>
        public string SourceLocalProfileId{get;}
        /// <summary>Original historical envelope catalog hash.</summary>
        public string SourceContentCatalogHash{get;}
        /// <summary>Explicit destination profile, preserving isolation.</summary>
        public SaveProfileIdentity DestinationProfileIdentity{get;}
        /// <summary>Explicit exact target catalog set.</summary>
        public CatalogSetIdentity DestinationCatalogSet{get;}
        /// <summary>Complete copied source-to-target level bindings.</summary>
        public IReadOnlyList<HistoricalLevelBinding> Levels{get;}
        /// <summary>Fully parsed exact historical membership.</summary>
        public ILockedCampaignCatalog SourceCampaign{get;}
        /// <summary>Explicit known source chain algorithm identifier.</summary>
        public string LedgerAlgorithm{get;}
    }
    /// <summary>Exact immutable old-v2 reader fingerprint and known absence of other open claim states.</summary>
    public sealed class LegacyV2ReaderBinding
    {
        /// <summary>Binds the known closed ACTIVE_DRAFTS-only source form to exact original envelope and payload hashes.</summary>
        public LegacyV2ReaderBinding(string readerId,string sourceEnvelopeSha256,string sourcePayloadSha256,string sourceContentCatalogHash,CatalogUse use)
        {ReaderId=readerId??throw new ArgumentNullException(nameof(readerId));SourceEnvelopeSha256=LocalSaveCodec.Hash(sourceEnvelopeSha256);SourcePayloadSha256=LocalSaveCodec.Hash(sourcePayloadSha256);SourceContentCatalogHash=LocalSaveCodec.Hash(sourceContentCatalogHash);Use=use;if(use!=CatalogUse.ContractTest||readerId!="contract-save-v2-active-drafts-closed-1")throw new ArgumentException("SAVE_MIGRATION_NEEDS_DECISION");}
        /// <summary>Exact known reader lineage.</summary>
        public string ReaderId{get;}
        /// <summary>Exact complete original source byte hash.</summary>
        public string SourceEnvelopeSha256{get;}
        /// <summary>Exact original canonical payload hash.</summary>
        public string SourcePayloadSha256{get;}
        /// <summary>Exact original aggregate catalog hash.</summary>
        public string SourceContentCatalogHash{get;}
        /// <summary>Explicit isolated fixture use.</summary>
        public CatalogUse Use{get;}
    }
    internal sealed class LegacyLevelDto
    {
        internal LegacyLevelDto(string id,string profile,string hash,bool completed,int stars,long? best,long? revision,long? firstElapsed,string? firstUtc,Guid? attempt,string? solved,bool direct,EconomyTerminal? terminal,EconomyTerminal? second,EconomyTerminal? third,string[] claims){LevelId=id;HashProfile=profile;HashSha256=hash;Completed=completed;Stars=stars;Best=best;RevisionBest=revision;FirstElapsed=firstElapsed;FirstUtc=firstUtc;Attempt=attempt;Solved=solved;Direct=direct;Terminal=terminal;Second=second;Third=third;TerminalClaims=Array.AsReadOnly(claims);}
        internal string LevelId{get;}internal string HashProfile{get;}internal string HashSha256{get;}internal bool Completed{get;}internal int Stars{get;}internal long? Best{get;}internal long? RevisionBest{get;}internal long? FirstElapsed{get;}internal string? FirstUtc{get;}internal Guid? Attempt{get;}internal string? Solved{get;}internal bool Direct{get;}internal EconomyTerminal? Terminal{get;}internal EconomyTerminal? Second{get;}internal EconomyTerminal? Third{get;}internal IReadOnlyList<string> TerminalClaims{get;}
    }
    internal sealed class LegacyDraftDto
    {
        internal LegacyDraftDto(string id,string profile,string hash,string saved,PuzzleSessionCheckpoint checkpoint){LevelId=id;HashProfile=profile;HashSha256=hash;SavedAtUtc=saved;Checkpoint=checkpoint;}
        internal string LevelId{get;}internal string HashProfile{get;}internal string HashSha256{get;}internal string SavedAtUtc{get;}internal PuzzleSessionCheckpoint Checkpoint{get;}
    }
    internal sealed class LegacyIntervalDto
    {
        internal LegacyIntervalDto(ulong first,ulong last,string status){First=first;Last=last;Status=status;}
        internal ulong First{get;}internal ulong Last{get;}internal string Status{get;}
    }
    internal sealed class LegacyEndlessDto
    {
        internal LegacyEndlessDto(ulong next,ActiveEndlessDraft[] active,LegacyIntervalDto[] intervals,OpenEndlessCompletion[] claims){NextOrdinal=next;ActiveDrafts=Array.AsReadOnly(active);Intervals=Array.AsReadOnly(intervals);OpenClaims=Array.AsReadOnly(claims);}
        internal ulong NextOrdinal{get;}internal IReadOnlyList<ActiveEndlessDraft> ActiveDrafts{get;}internal IReadOnlyList<LegacyIntervalDto> Intervals{get;}internal IReadOnlyList<OpenEndlessCompletion> OpenClaims{get;}
    }
    // Complete typed source DTO. No arbitrary JSON cursor, mutable dictionary or default-filled partial save survives parsing.
    internal sealed class SaveV1Dto
    {
        internal SaveV1Dto(string id,string firstUse,LegacyLevelDto[] levels,LegacyEndlessDto endless,LocalLedgerCheckpoint ledger,LocalStoredEntry[] journal,LocalInventory inventory,LocalCosmeticReservation[] reservations,LocalSeasonProgressCache cache,LegacyDraftDto[] drafts,LocalRewardClaim[] claims,LocalDailyStatus? daily,LocalEntitlements entitlements,LocalPendingOperation[] operations,LocalSettings settings,LocalDataLifecycle lifecycle,LocalHintEntitlement? hints){LocalProfileId=id;FirstUseUtc=firstUse;Levels=Array.AsReadOnly(levels);Endless=endless;LedgerCheckpoint=ledger;EconomyJournal=Array.AsReadOnly(journal);Inventory=inventory;Reservations=Array.AsReadOnly(reservations);Cache=cache;Drafts=Array.AsReadOnly(drafts);Claims=Array.AsReadOnly(claims);Daily=daily;Entitlements=entitlements;Operations=Array.AsReadOnly(operations);Settings=settings;Lifecycle=lifecycle;Hints=hints;}
        internal string LocalProfileId{get;}internal string FirstUseUtc{get;}internal IReadOnlyList<LegacyLevelDto> Levels{get;}internal LegacyEndlessDto Endless{get;}internal LocalLedgerCheckpoint LedgerCheckpoint{get;}internal IReadOnlyList<LocalStoredEntry> EconomyJournal{get;}internal LocalInventory Inventory{get;}internal IReadOnlyList<LocalCosmeticReservation> Reservations{get;}internal LocalSeasonProgressCache Cache{get;}internal IReadOnlyList<LegacyDraftDto> Drafts{get;}internal IReadOnlyList<LocalRewardClaim> Claims{get;}internal LocalDailyStatus? Daily{get;}internal LocalEntitlements Entitlements{get;}internal IReadOnlyList<LocalPendingOperation> Operations{get;}internal LocalSettings Settings{get;}internal LocalDataLifecycle Lifecycle{get;}internal LocalHintEntitlement? Hints{get;}
    }
    /// <summary>Pure versioned migration with explicit original source, historical document/lock and complete prefix bindings.</summary>
    public sealed class SaveMigration
    {
        private readonly LocalSaveCodec codec;private readonly ICanonicalJson json;private readonly SaveProfileIdentity profile;private readonly CatalogSetIdentity catalogs;private readonly IReadOnlyList<HistoricalProfileBinding> histories;private readonly IReadOnlyList<LegacyV2ReaderBinding> legacyV2;
        internal SaveMigration(LocalSaveCodec codec,ICanonicalJson json,SaveProfileIdentity profile,CatalogSetIdentity catalogs,IReadOnlyList<HistoricalProfileBinding> histories,IReadOnlyList<LegacyV2ReaderBinding> legacyV2){this.codec=codec;this.json=json;this.profile=profile;this.catalogs=catalogs;this.histories=Array.AsReadOnly(histories.ToArray());this.legacyV2=Array.AsReadOnly(legacyV2.ToArray());}
        internal void RequireLegacyV2(string sourceEnvelope,string sourcePayload,string sourceCatalog){LocalSaveCodec.Require(legacyV2.Count(b=>b.Use==profile.Use&&b.SourceEnvelopeSha256==sourceEnvelope&&b.SourcePayloadSha256==sourcePayload&&b.SourceContentCatalogHash==sourceCatalog)==1,"SAVE_MIGRATION_NEEDS_DECISION",true);}
        private static ulong Ordinal(CanonicalDtoCursor v){string value=v.String();LocalSaveCodec.Require(value.Length>0&&(value=="0"||value[0]!='0')&&value.All(c=>c>='0'&&c<='9')&&ulong.TryParse(value,NumberStyles.None,CultureInfo.InvariantCulture,out _),"SAVE_ORDINAL_INVALID");return ulong.Parse(value,CultureInfo.InvariantCulture);}
        private static long? Time(CanonicalDtoCursor v)=>v.IsNull?(long?)null:v.Integer();
        private static string? TextOrNull(CanonicalDtoCursor v)=>v.IsNull?null:v.String();
        private HistoricalLevelBinding Resolve(HistoricalProfileBinding binding,string id,string hashProfile,string hash)
        {var found=binding.Levels.Where(l=>l.LegacyLevelId==id&&hashProfile=="STP-LEVEL-V1-PUZZLE-JCS-1"&&l.LegacyPuzzleHashSha256==hash).ToArray();LocalSaveCodec.Require(found.Length==1,"SAVE_LEVEL_IDENTITY_UNRESOLVED",true);return found[0];}
        private LegacyLevelDto ReadLevel(string id,CanonicalDtoCursor v)
        {
            var f=v.Fields("puzzleHashProfile","puzzleHashSha256","completed","starsHighWater","bestElapsedMilliseconds","revisionBestElapsedMilliseconds","firstClearElapsedMilliseconds","firstCompletedAtUtc","firstClearAttemptId","firstClearSolvedEventId","directSolved","firstClearTerminal","secondStarPaid","thirdStarPaid","terminalClaimIds");return new LegacyLevelDto(id,f[0].String(),f[1].String(),f[2].Boolean(),f[3].Int32(),Time(f[4]),Time(f[5]),Time(f[6]),TextOrNull(f[7]),f[8].IsNull?(Guid?)null:f[8].Guid(),TextOrNull(f[9]),f[10].Boolean(),f[11].IsNull?null:codec.ReadEconomyTerminal(f[11]),f[12].IsNull?null:codec.ReadEconomyTerminal(f[12]),f[13].IsNull?null:codec.ReadEconomyTerminal(f[13]),f[14].Array().Select(x=>x.String()).ToArray());
        }
        private LegacyDraftDto ReadDraft(CanonicalDtoCursor v,HistoricalProfileBinding binding)
        {
            var f=v.Fields("levelId","legacyPuzzleHash","savedAtUtc","checkpoint");var h=f[1].Fields("profile","sha256");var resolved=Resolve(binding,f[0].String(),h[0].String(),h[1].String());var cp=f[3];cp.Version("recordVersion",1,"SAVE_RECORD_VERSION_UNSUPPORTED");foreach(string name in new[]{"recordVersion","identity","rulesetVersion","attemptId","mode","phase","cells","previouslySet","revision","timerStarted","activeElapsedMs","usedEmptyMarker","usedOccupiedMarker","hintCount","correctionCount","appliedCommandIds","undo","solvedEventId","startedAtUtc","lastSavedAtUtc"})if(!cp.Has(name))throw new SaveCodecFailure("SAVE_MIGRATION_CHECKPOINT_INCOMPLETE",true);var i=cp.Find("identity").Fields("levelId","profile","sha256");LocalSaveCodec.Require(i[0].String()==resolved.LegacyLevelId&&i[1].String()=="STP-LEVEL-V1-PUZZLE-JCS-1"&&i[2].String()==resolved.LegacyPuzzleHashSha256,"SAVE_LEVEL_IDENTITY_UNRESOLVED",true);var checkpoint=codec.ReadCheckpoint(cp,resolved.Target.Identity);LocalSaveCodec.Require(checkpoint.Restore(resolved.Target.ToDomainDefinition(),resolved.Target.Identity.PuzzleId,resolved.Target.Identity.HashProfile,resolved.Target.Identity.HashSha256).Succeeded,"CHECKPOINT_UNDO_REPLAY_INVALID");return new LegacyDraftDto(f[0].String(),h[0].String(),h[1].String(),LocalSaveCodec.Utc(f[2].String()),checkpoint);
        }
        private LegacyEndlessDto ReadEndless(CanonicalDtoCursor v)
        {
            var f=v.Fields("nextGenerationOrdinal","activeDrafts","terminalIntervals","openRewardClaims");ulong next=Ordinal(f[0]);LocalSaveCodec.Require(next>0,"SAVE_MIGRATION_NEEDS_DECISION",true);var active=f[1].Array().Select(codec.ReadActiveEndlessDraft).ToArray();var intervals=f[2].Array().Select(x=>{var q=x.Fields("firstOrdinal","lastOrdinal","status");ulong first=Ordinal(q[0]),last=Ordinal(q[1]);string status=q[2].String();LocalSaveCodec.Require(first>0&&last>=first&&last<next&&(status=="COMPLETED"||status=="ABANDONED"),"SAVE_MIGRATION_NEEDS_DECISION",true);return new LegacyIntervalDto(first,last,status);}).ToArray();
            var claims=f[3].Array().Select(x=>{var q=x.Fields("generationOrdinal","id","descriptor","puzzleId","completionCommitId","claim");var claim=q[5].Find("claimStatus").String();LocalSaveCodec.Require(new[]{"LOCAL_DECISION_PENDING","PROVIDER_RESERVED","RECONCILIATION_REQUIRED","REWARD_CONFIRMED","NO_REWARD_CONFIRMED"}.Contains(claim),"SAVE_MIGRATION_NEEDS_DECISION",true);return OpenEndlessCompletion.Create(q[0].String(),q[1].String(),codec.ReadLocalEndlessDescriptor(q[2]),q[3].String(),q[4].String(),codec.ReadLocalEndlessClaim(q[5]));}).ToArray();return new LegacyEndlessDto(next,active,intervals,claims);
        }
        private SaveV1Dto ReadDto(CanonicalDtoCursor v,HistoricalProfileBinding binding)
        {
            var f=v.FieldsOptional(new[]{"profile","levels","endless","ledgerCheckpoint","economyJournal","inventory","cosmeticClaimReservations","seasonProgressCache","drafts","rewardClaims","dailyStatus","entitlements","pendingOperations","settings","dataLifecycle"},new[]{"hintEntitlement"});var header=f[0].Fields("localProfileId","firstUseUtc");var cp=f[3];cp.Version("ledgerContractVersion",1,"SAVE_LEDGER_ALGORITHM_UNSUPPORTED");var checkpoint=cp.Fields("ledgerContractVersion","throughSequence","balancePatience","entryChainHashSha256","sourceSaveGeneration");LocalSaveCodec.Require(checkpoint[1].Integer()==0,"SAVE_LEDGER_ANCESTRY_UNPROVEN",true);var ledger=LocalLedgerCheckpoint.Create(checkpoint[1].Integer(),checkpoint[2].Integer(),checkpoint[3].String(),checkpoint[4].Integer(),Array.Empty<LocalStoredEntry>());var inventory=codec.ReadLocalInventory(f[5]);LocalSaveCodec.Require(inventory.Ownership.All(o=>o is LegacyOwnership),"SAVE_LEGACY_READER_UNSUPPORTED",true);
            return new SaveV1Dto(header[0].String(),LocalSaveCodec.Utc(header[1].String()),f[1].Members().Select(x=>ReadLevel(x.Key,x.Value)).ToArray(),ReadEndless(f[2]),ledger,f[4].Array().Select(codec.ReadLocalStoredEntry).ToArray(),inventory,f[6].Array().Select(codec.ReadLocalCosmeticReservation).ToArray(),codec.ReadLocalSeasonProgressCache(f[7]),f[8].Array().Select(d=>ReadDraft(d,binding)).ToArray(),f[9].Array().Select(codec.ReadLocalRewardClaim).ToArray(),f[10].IsNull?null:codec.ReadLocalDailyStatus(f[10]),codec.ReadLocalEntitlements(f[11]),f[12].Array().Select(codec.ReadLocalPendingOperation).ToArray(),codec.ReadLocalSettings(f[13]),codec.ReadLocalDataLifecycle(f[14]),f[15].Missing||f[15].IsNull?null:codec.ReadLocalHintEntitlement(f[15]));
        }
        private static LocalEndlessState ConvertEndless(LegacyEndlessDto e)
        {
            ulong high=e.NextOrdinal-1;var open=e.ActiveDrafts.Cast<LocalOpenEndless>().Concat(e.OpenClaims).OrderBy(x=>ulong.Parse(x.GenerationOrdinal,CultureInfo.InvariantCulture)).ToArray();LocalSaveCodec.Require(open.Length<=20&&open.Select(x=>x.GenerationOrdinal).Distinct(StringComparer.Ordinal).Count()==open.Length,"SAVE_MIGRATION_NEEDS_DECISION",true);
            var ranges=e.Intervals.Select(i=>(First:i.First,Last:i.Last)).Concat(e.ActiveDrafts.Select(d=>{ulong n=ulong.Parse(d.GenerationOrdinal,CultureInfo.InvariantCulture);return(First:n,Last:n);})).OrderBy(i=>i.First).ToArray();ulong position=0;foreach(var range in ranges){LocalSaveCodec.Require(position<ulong.MaxValue&&range.First==position+1&&range.Last>=range.First&&range.Last<=high,"SAVE_MIGRATION_NEEDS_DECISION",true);position=range.Last;}LocalSaveCodec.Require(position==high,"SAVE_MIGRATION_NEEDS_DECISION",true);
            foreach(var interval in e.Intervals.Where(i=>i.Status=="COMPLETED")){var claims=e.OpenClaims.Where(c=>{ulong n=ulong.Parse(c.GenerationOrdinal,CultureInfo.InvariantCulture);return n>=interval.First&&n<=interval.Last;}).OrderBy(c=>ulong.Parse(c.GenerationOrdinal,CultureInfo.InvariantCulture)).ToArray();LocalSaveCodec.Require(interval.Last-interval.First<(ulong)claims.Length&&claims.Length>0,"SAVE_MIGRATION_NEEDS_DECISION",true);for(int i=0;i<claims.Length;i++)LocalSaveCodec.Require(ulong.Parse(claims[i].GenerationOrdinal,CultureInfo.InvariantCulture)==interval.First+(ulong)i,"SAVE_MIGRATION_NEEDS_DECISION",true);}
            foreach(var claim in e.OpenClaims){ulong n=ulong.Parse(claim.GenerationOrdinal,CultureInfo.InvariantCulture);LocalSaveCodec.Require(e.Intervals.Any(i=>i.Status=="COMPLETED"&&n>=i.First&&n<=i.Last),"SAVE_MIGRATION_NEEDS_DECISION",true);}
            return LocalEndlessState.Create(high.ToString(CultureInfo.InvariantCulture),open);
        }
        internal LocalProfileState ReadAndMigrateV1(CanonicalDtoCursor payload,string sourceCatalog,long generation)
        {
            var header=payload.Find("profile").Fields("localProfileId","firstUseUtc");string id=header[0].String();var found=histories.Where(h=>h.SourceLocalProfileId==id&&h.SourceContentCatalogHash==sourceCatalog&&h.DestinationProfileIdentity.Equals(profile)&&h.DestinationCatalogSet.Equals(catalogs)).ToArray();LocalSaveCodec.Require(found.Length==1,"SAVE_LEVEL_IDENTITY_UNRESOLVED",true);var binding=found[0];LocalSaveCodec.Require(binding.LedgerAlgorithm=="CONTRACT1_RAW32_PREVIOUS_THEN_ENTRY_JCS","SAVE_LEDGER_ALGORITHM_UNSUPPORTED",true);var dto=ReadDto(payload,binding);var levels=new List<LocalLevelProgress>();var claims=new HashSet<string>(StringComparer.Ordinal);
            foreach(var l in dto.Levels)
            {
                var resolution=Resolve(binding,l.LevelId,l.HashProfile,l.HashSha256);LocalFirstClear? first=null;
                if(l.Completed){LocalSaveCodec.Require(l.FirstElapsed.HasValue&&l.FirstUtc!=null&&l.Attempt.HasValue&&l.Solved!=null&&l.Terminal!=null&&l.Best.HasValue,"SAVE_MIGRATION_NEEDS_DECISION",true);first=LocalFirstClear.Create(l.Attempt!.Value,l.Solved!,LocalSaveCodec.Utc(l.FirstUtc!),l.FirstElapsed!.Value,l.Terminal!);}
                else LocalSaveCodec.Require(l.Stars==0&&l.Best==null&&l.RevisionBest==null&&l.FirstElapsed==null&&l.FirstUtc==null&&l.Attempt==null&&l.Solved==null&&l.Terminal==null&&l.Second==null&&l.Third==null&&!l.Direct,"SAVE_MIGRATION_NEEDS_DECISION",true);
                levels.Add(LocalLevelProgress.Create(resolution.Target.Identity,first,l.Stars,l.Best,l.RevisionBest,l.Direct,l.Second,l.Third));foreach(string claim in l.TerminalClaims)LocalSaveCodec.Require(claims.Add(claim),"SAVE_DUPLICATE_ID");
            }
            var ordered=levels.OrderBy(l=>l.Identity.PuzzleId,StringComparer.Ordinal).ToArray();var actualTerminals=ordered.SelectMany(l=>new[]{l.FirstClear?.Terminal,l.SecondStarPaid,l.ThirdStarPaid}).Concat(dto.Claims.Select(c=>c is CampaignProgressTerminal p?p.Terminal:c is PostClearRewardClaim r?r.Terminal:null)).Where(t=>t!=null).Select(t=>t!.ClaimId).ToArray();LocalSaveCodec.Require(claims.All(c=>actualTerminals.Contains(c)),"SAVE_MIGRATION_NEEDS_DECISION",true);
            var sourceCache=codec.CacheFor(binding.SourceCampaign,ordered);LocalSaveCodec.Require(CacheEqual(sourceCache,dto.Cache),"SAVE_CACHE_INVALID");var cache=codec.CacheCurrent(ordered);LocalSaveCodec.Require(SameMembership(binding.SourceCampaign,codec.TargetCampaign),"SAVE_MIGRATION_NEEDS_DECISION",true);
            var drafts=dto.Drafts.Select(d=>{var r=Resolve(binding,d.LevelId,d.HashProfile,d.HashSha256);return LocalDraft.Create(r.Target.Identity,d.SavedAtUtc,d.Checkpoint);}).ToArray();var result=LocalProfileState.Create(LocalProfileHeader.Create(dto.LocalProfileId,dto.FirstUseUtc,profile,catalogs),ordered,ConvertEndless(dto.Endless),dto.LedgerCheckpoint,dto.EconomyJournal,dto.Inventory,dto.Reservations,cache,drafts,dto.Claims,dto.Daily,dto.Entitlements,dto.Operations,dto.Settings,dto.Lifecycle,dto.Hints);codec.Validate(result,generation);return result;
        }
        private static bool CacheEqual(LocalSeasonProgressCache a,LocalSeasonProgressCache b){if(!a.CampaignCatalog.Equals(b.CampaignCatalog)||a.SourceProjectionHashSha256!=b.SourceProjectionHashSha256||a.Subjects.Count!=b.Subjects.Count)return false;for(int i=0;i<a.Subjects.Count;i++){var x=a.Subjects[i];var y=b.Subjects[i];if(x.SubjectId!=y.SubjectId||x.Kind!=y.Kind||x.FirstClearCount!=y.FirstClearCount||x.MemberCount!=y.MemberCount||x.Completed!=y.Completed||!x.FirstClearPuzzleIds.SequenceEqual(y.FirstClearPuzzleIds))return false;}return true;}
        private static bool SameMembership(ILockedCampaignCatalog a,ILockedCampaignCatalog b)=>a.Entries.Count==b.Entries.Count&&a.Entries.All(x=>b.Entries.Any(y=>x.Puzzle.Equals(y.Puzzle)&&x.SeasonId==y.SeasonId&&x.SectionId==y.SectionId&&x.RouteId==y.RouteId&&x.Ordinal==y.Ordinal&&x.RequiredFirstClears.OrderBy(s=>s,StringComparer.Ordinal).SequenceEqual(y.RequiredFirstClears.OrderBy(s=>s,StringComparer.Ordinal))));
    }
}
