using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using STP.Application;

namespace STP.Tests.Application.EditMode
{
    public sealed class MobileCandidateCatalogTests
    {
        [Test]
        public void GeneratedActualCandidateSourcesBindExactLocksWithoutOldQaInputs()
        {
            string bindings=Source("MobileCandidate.bindings.json");
            string Bound(string key)=>Regex.Match(bindings,"\""+key+"\"\\s*:\\s*\"([a-f0-9]{64})\"").Groups[1].Value;
            var puzzles=ReadPublic(Source("MobileCandidate.public.json"),Bound("publicCatalogHashSha256"));
            var progress=ReadProgress(puzzles,Source("MobileCandidate.campaign.json"),Source("MobileCandidate.completion.json"),Bound("campaignHashSha256"),Bound("completionHashSha256"));
            var campaign=(ILockedCampaignCatalog)LocalSaveContractFixtures.Get(progress,"Campaign")!;
            var cosmetics=LocalSaveContractFixtures.Construct<ILockedCosmeticsCatalog>("STP.Infrastructure.Content","STP.Infrastructure.Content.LockedCosmeticsCatalog",Encoding.UTF8.GetBytes(Source("MobileCandidate.cosmetics.json")),Bound("cosmeticsHashSha256"),CatalogUse.InternalPreview,LocalSaveContractFixtures.Json);
            Assert.That(puzzles.Puzzles.Count,Is.EqualTo(240));Assert.That(campaign.Entries.Count,Is.EqualTo(240));Assert.That(cosmetics.Items.Count,Is.EqualTo(14));
            var type=Assembly.Load("STP.Infrastructure.Persistence").GetType("STP.Infrastructure.Persistence.RetainedContractBindings",true)!;
            Assert.DoesNotThrow(()=>type.GetMethod("ForCosmetics")!.Invoke(null,new object[]{cosmetics.Identity,campaign.Identity,Encoding.UTF8.GetBytes(Source("MobileCandidate.cosmetics.json")),Encoding.UTF8.GetBytes(Source("MobileCandidate.campaign.json")),puzzles,LocalSaveContractFixtures.Json}));
        }
        [Test]
        public void PostClearRoundTripUsesExactWholeWrapperDefinitionAndMissingBindingIsUnsupported()
        {
            var f=new Fixture();var json=LocalSaveContractFixtures.Json;var assembly=Assembly.Load("STP.Infrastructure.Persistence");
            string completion=new Regex("\"rewardDefinitions\"\\s*:\\s*\\[").Replace(f.CompletionText,"\"rewardDefinitions\":[{\"reasonCode\":\"POST_CLEAR_PATIENCE\",\"subjectKind\":\"LEVEL\",\"amountPatience\":10,\"claimCardinality\":\"ONCE_PER_SUBJECT\"},",1);
            var progress=ReadProgress(f.Public,f.CampaignText,completion);var graph=(ILockedCampaignCatalog)LocalSaveContractFixtures.Get(progress,"Campaign")!;var values=(ILockedCompletionCatalog)LocalSaveContractFixtures.Get(progress,"Completion")!;var catalogs=(CatalogSetIdentity)LocalSaveContractFixtures.Get(progress,"CatalogSet")!;
            var ledger=LocalSaveContractFixtures.Construct<ILocalLedger>("STP.Infrastructure.Persistence","STP.Infrastructure.Persistence.SaveLedgerCodec",json);var policy=new LocalProgressPolicy(f.Public,graph,values,json,ledger);var state=policy.NewProfile(f.Profile,catalogs,Utc).WithMetagame(LocalMetagameState.Empty);var identity=f.Public.Puzzles[0].Identity;
            var attempt=Guid.NewGuid();var terminal=EconomyTerminal.Create("level:"+identity.PuzzleId+":first-clear","LEVEL_FIRST_CLEAR",identity.PuzzleId,values.Identity,"reward:LEVEL_FIRST_CLEAR:"+identity.PuzzleId,15,1,Utc);
            var levels=new[]{LocalLevelProgress.Create(identity,LocalFirstClear.Create(attempt,"puzzle-solved:"+attempt.ToString("D"),Utc,100,terminal),1,100,null,true,null,null)};
            state=state.WithProgress(levels,state.LedgerCheckpoint,new[]{ledger.Next(state.LedgerCheckpoint,state.EconomyJournal,terminal)},policy.Cache(levels),state.Drafts,state.RewardClaims);
            var post=new LocalPostClearPolicy(f.Public,values.Identity,ledger);state=post.Reserve(state,identity,2);var reservation=LocalPostClearPolicy.Reservation(SaveCommitResult<LocalProfileState>.Success(new SaveSnapshot<LocalProfileState>(f.Profile,catalogs.AggregateCatalog,2,state)),identity);state=post.Confirm(state,new AdRewardEvidence(reservation,reservation.OperationId),3,Utc);
            var bindingType=assembly.GetType("STP.Infrastructure.Persistence.RetainedCompletionRewardBinding",true)!;var binding=bindingType.GetMethod("FromWrapper")!.Invoke(null,new object[]{LocalPostClearPolicy.PolicyVersion,Encoding.UTF8.GetBytes(completion),json,values.Identity})!;
            var rewards=Array.CreateInstance(bindingType,1);rewards.SetValue(binding,0);var retainedType=assembly.GetType("STP.Infrastructure.Persistence.RetainedContractBindings",true)!;var retained=retainedType.GetMethod("ForRuntimeWithRewards")!.Invoke(null,new object?[]{CatalogUse.InternalPreview,null,rewards})!;
            var codecType=assembly.GetType("STP.Infrastructure.Persistence.LocalSaveCodec",true)!;var codec=codecType.GetMethod("ForMetagame")!.Invoke(null,new object[]{json,f.Profile,catalogs,f.Public,graph,values,retained})!;
            var bytes=(byte[])LocalSaveContractFixtures.Call(codec,"Encode",new SaveSnapshot<LocalProfileState>(f.Profile,catalogs.AggregateCatalog,3,state),Utc,"test","post-clear")!;var decoded=LocalSaveContractFixtures.Call(codec,"Decode",bytes)!;Assert.That(LocalSaveContractFixtures.Get(decoded,"Status")!.ToString(),Is.EqualTo("Valid"));Assert.That(ledger.Validate(((SaveSnapshot<LocalProfileState>)LocalSaveContractFixtures.Get(decoded,"Snapshot")!).Payload),Is.EqualTo(25));
            var missing=codecType.GetMethod("ForMetagame")!.Invoke(null,new object?[]{json,f.Profile,catalogs,f.Public,graph,values,null})!;Assert.That(LocalSaveContractFixtures.Get(LocalSaveContractFixtures.Call(missing,"Decode",bytes)!,"Status")!.ToString(),Is.EqualTo("Unsupported"));
            Assert.Throws<TargetInvocationException>(()=>bindingType.GetMethod("FromWrapper")!.Invoke(null,new object[]{LocalPostClearPolicy.PolicyVersion,Encoding.UTF8.GetBytes(completion.Replace("POST_CLEAR_PATIENCE","INVENTED_REWARD")),json,values.Identity}));
        }
        private const string Utc="2026-10-21T00:00:00Z";
        private static string Source(string file)=>File.ReadAllText(Path.Combine(Environment.GetEnvironmentVariable("STP_SOURCE_ROOT")??Directory.GetCurrentDirectory(),"Assets/StammstreckenPuzzle/UI",file));
        private static string Hash(string text)=>LocalSaveHash.Bytes(LocalSaveHash.Canonical(LocalSaveContractFixtures.Json,text));
        private sealed class Fixture
        {
            internal readonly string PublicText,CampaignText,CompletionText;
            internal readonly string PublicHash;
            internal readonly IPublicPuzzleCatalog Public;
            internal readonly object Progress;
            internal readonly ILockedCampaignCatalog Campaign;
            internal readonly ILockedCompletionCatalog Completion;
            internal readonly CatalogSetIdentity Catalogs;
            internal readonly SaveProfileIdentity Profile=new SaveProfileIdentity(SaveProfileIdentity.MobileCandidateNamespace,"candidate",CatalogUse.InternalPreview);
            internal Fixture()
            {
                string payload=Source("Public240.public.json").Replace("qa-public240-v1-public","qa-mobile-candidate-public").Replace("IsolatedReview","InternalPreview");
                payload=Regex.Replace(payload,",?\\s*\"catalogHashSha256\"\\s*:\\s*\"[a-f0-9]+\"","");PublicHash=Hash(payload);
                PublicText=payload.Substring(0,payload.LastIndexOf('}'))+",\"catalogHashSha256\":\""+PublicHash+"\"}";
                Public=ReadPublic(PublicText,PublicHash);
                string campaign=Source("ReviewCampaign.catalog.json").Replace("qa-public240-v1-campaign","campaign-qa-mobile-candidate");
                string? previous=null;int count=0;
                CampaignText=Regex.Replace(campaign,"(?s)\"puzzleId\"\\s*:\\s*\"(S1-[0-9-]+)\",\\s*\"order\"\\s*:\\s*([0-9]+),\\s*\"unlock\"\\s*:\\s*\\{\\s*\"kind\"\\s*:\\s*\"ALWAYS\",\\s*\"requiredSubjectIds\"\\s*:\\s*\\[\\s*\\]\\s*\\}",match=>
                {
                    string id=match.Groups[1].Value;string unlock=previous==null?"{\"kind\":\"ALWAYS\",\"requiredSubjectIds\":[]}":"{\"kind\":\"ALL_FIRST_CLEARS\",\"requiredSubjectIds\":[\""+previous+"\"]}";
                    previous=id;count++;return "\"puzzleId\":\""+id+"\",\"order\":"+match.Groups[2].Value+",\"unlock\":"+unlock;
                });
                Assert.That(count,Is.EqualTo(240));
                string completion=Source("ReviewCompletion.catalog.json").Replace("qa-public240-v1-completion","completion-qa-mobile-candidate");
                string moments=string.Join(",",Public.Puzzles.Select(p=>"{\"id\":\"candidate-"+p.Identity.PuzzleId.ToLowerInvariant()+"\",\"resultTextKey\":\"level."+p.Identity.PuzzleId.ToLowerInvariant().Replace('-','.')+".result\",\"mapSegmentId\":\"map-"+p.Identity.PuzzleId.ToLowerInvariant()+"\",\"trainMomentId\":\"train-"+p.Identity.PuzzleId.ToLowerInvariant()+"\"}"));
                CompletionText=Regex.Replace(completion,"(?s)\"completionMoments\"\\s*:\\s*\\[\\s*\\{.*?\\}\\s*\\]","\"completionMoments\":["+moments+"]");
                Progress=ReadProgress(Public,CampaignText,CompletionText);Campaign=(ILockedCampaignCatalog)LocalSaveContractFixtures.Get(Progress,"Campaign")!;Completion=(ILockedCompletionCatalog)LocalSaveContractFixtures.Get(Progress,"Completion")!;Catalogs=(CatalogSetIdentity)LocalSaveContractFixtures.Get(Progress,"CatalogSet")!;
            }
        }
        private static IPublicPuzzleCatalog ReadPublic(string source,string hash)=>LocalSaveContractFixtures.Construct<IPublicPuzzleCatalog>("STP.Infrastructure.Content","STP.Infrastructure.Content.MobileCandidateData",Encoding.UTF8.GetBytes(source),hash);
        private static object ReadProgress(IPublicPuzzleCatalog puzzles,string campaign,string completion,string? campaignHash=null,string? completionHash=null)=>LocalSaveContractFixtures.Construct<object>("STP.Infrastructure.Content","STP.Infrastructure.Content.MobileCandidateProgressCatalog",puzzles,Encoding.UTF8.GetBytes(campaign),Encoding.UTF8.GetBytes(completion),LocalSaveContractFixtures.Json,campaignHash??Hash(campaign),completionHash??Hash(completion));
        [Test]
        public void ActualDraft240HaveSeparateLocksFullHierarchyAndOnlyLinearFirstClearUnlocks()
        {
            var f=new Fixture();Assert.That(f.Public.Identity.Use,Is.EqualTo(CatalogUse.InternalPreview));Assert.That(f.Public.Puzzles.Count,Is.EqualTo(240));
            Assert.That(f.Campaign.Entries.Select(e=>e.SectionId).Distinct().Count(),Is.EqualTo(5));Assert.That(f.Campaign.Entries.Select(e=>e.RouteId).Distinct().Count(),Is.EqualTo(20));
            Assert.That(f.Campaign.Entries[0].RequiredFirstClears,Is.Empty);
            for(int i=1;i<240;i++)Assert.That(f.Campaign.Entries[i].RequiredFirstClears,Is.EqualTo(new[]{f.Campaign.Entries[i-1].Puzzle.PuzzleId}));
            var ledger=LocalSaveContractFixtures.Construct<ILocalLedger>("STP.Infrastructure.Persistence","STP.Infrastructure.Persistence.SaveLedgerCodec",LocalSaveContractFixtures.Json);
            var policy=new LocalProgressPolicy(f.Public,f.Campaign,f.Completion,LocalSaveContractFixtures.Json,ledger);
            var state=policy.NewProfile(f.Profile,f.Catalogs,Utc).WithMetagame(LocalMetagameState.Empty);
            Assert.That(policy.CanEnter(state,f.Public.Puzzles[0].Identity),Is.True);Assert.That(policy.CanEnter(state,f.Public.Puzzles[1].Identity),Is.False);
            Assert.That(f.Completion.TryGet(f.Public.Puzzles.Last().Identity,out var values),Is.True);Assert.That(values!.TwoStarMs,Is.Null);
        }
        [Test]
        public void CandidateNamespaceCannotMasqueradeAsProductionFixtureOrOldReviewAndLocksFailClosed()
        {
            var f=new Fixture();Assert.Throws<ArgumentException>(()=>new SaveProfileIdentity(SaveProfileIdentity.MobileCandidateNamespace,"candidate",CatalogUse.Production));
            Assert.Throws<ArgumentException>(()=>new SaveProfileIdentity(SaveProfileIdentity.Public240ReviewNamespace,"r",CatalogUse.InternalPreview));
            Assert.Throws<ArgumentException>(()=>new SaveProfileIdentity("production-v1","candidate",CatalogUse.InternalPreview));
            Assert.Throws<ArgumentException>(()=>new CatalogIdentity("qa-mobile-candidate-public",1,f.PublicHash,CatalogUse.Production,CatalogApprovalStatus.ProductApproved));
            Assert.Throws<ArgumentException>(()=>new CatalogIdentity("public-production",1,f.PublicHash,CatalogUse.InternalPreview,CatalogApprovalStatus.Draft));
            Assert.Throws<ArgumentException>(()=>new CatalogIdentity("campaign-qa-mobile-candidate",1,f.PublicHash,CatalogUse.ContractTest,CatalogApprovalStatus.FixtureOnly));
            Assert.Throws<TargetInvocationException>(()=>ReadPublic(f.PublicText,new string('b',64)));
            Assert.Throws<TargetInvocationException>(()=>ReadProgress(f.Public,f.CampaignText,f.CompletionText,campaignHash:new string('b',64)));
            string broken=f.CampaignText.Replace("\"requiredSubjectIds\":[\"S1-01-01-01\"]","\"requiredSubjectIds\":[\"S1-05-04-12\"]");
            Assert.Throws<TargetInvocationException>(()=>ReadProgress(f.Public,broken,f.CompletionText));
            Assert.Throws<TargetInvocationException>(()=>ReadProgress(f.Public,f.CampaignText,f.CompletionText.Replace("DRAFT","FIXTURE_ONLY")));
        }
        [Test]
        public void DraftCosmeticsAreUsableOnlyInsideCandidateV3AndStorageNeverUsesContractTest()
        {
            var f=new Fixture();var json=LocalSaveContractFixtures.Json;var assembly=Assembly.Load("STP.Infrastructure.Persistence");
            string root=Environment.GetEnvironmentVariable("STP_SOURCE_ROOT")??Directory.GetCurrentDirectory();
            string source=File.ReadAllText(Path.Combine(root,"Content/Catalogs/s1/cosmetics-v2.json")).Replace("season1-cosmetics","qa-mobile-candidate-cosmetics");
            var cosmetics=LocalSaveContractFixtures.Construct<ILockedCosmeticsCatalog>("STP.Infrastructure.Content","STP.Infrastructure.Content.LockedCosmeticsCatalog",Encoding.UTF8.GetBytes(source),Hash(source),CatalogUse.InternalPreview,json);
            var ledger=LocalSaveContractFixtures.Construct<ILocalLedger>("STP.Infrastructure.Persistence","STP.Infrastructure.Persistence.SaveLedgerCodec",json);
            var progress=new LocalProgressPolicy(f.Public,f.Campaign,f.Completion,json,ledger);var state=progress.NewProfile(f.Profile,f.Catalogs,Utc).WithMetagame(LocalMetagameState.Empty);
            state=new LocalCosmeticsPolicy(cosmetics,f.Campaign,json,ledger).GrantDefaults(state,cosmetics.Identity.HashSha256,Utc).State;
            Assert.That(state.Inventory.Ownership.Single().ItemId,Is.EqualTo("train-stammrot-default"));Assert.That(cosmetics.Items.All(i=>i.Status=="DRAFT"),Is.True);
            var retainedType=assembly.GetType("STP.Infrastructure.Persistence.RetainedContractBindings",true)!;
            var cosmeticBinding=retainedType.GetMethod("ForCosmetics")!.Invoke(null,new object[]{cosmetics.Identity,f.Campaign.Identity,Encoding.UTF8.GetBytes(source),Encoding.UTF8.GetBytes(f.CampaignText),f.Public,json})!;
            var retained=retainedType.GetMethod("ForRuntime")!.Invoke(null,new[]{(object)CatalogUse.InternalPreview,cosmeticBinding})!;
            var codecType=assembly.GetType("STP.Infrastructure.Persistence.LocalSaveCodec",true)!;
            var codec=codecType.GetMethod("ForMetagame")!.Invoke(null,new object[]{json,f.Profile,f.Catalogs,f.Public,f.Campaign,f.Completion,retained})!;
            state=LocalSettingsPolicy.Apply(state,LocalSettings.Create("de-DE",new[]{LocalAudioPreference.Create("Master",75,100)},true,LocalAccessibilityPreference.Create(LocalSettingsPolicy.AccessibilityRevision,true,false)));
            var snapshot=new SaveSnapshot<LocalProfileState>(f.Profile,f.Catalogs.AggregateCatalog,1,state);
            var encoded=(byte[])LocalSaveContractFixtures.Call(codec,"Encode",snapshot,Utc,"candidate","candidate")!;
            var decoded=LocalSaveContractFixtures.Call(codec,"Decode",encoded)!;Assert.That(LocalSaveContractFixtures.Get(decoded,"Status")!.ToString(),Is.EqualTo("Valid"));
            var roundTrip=((SaveSnapshot<LocalProfileState>)LocalSaveContractFixtures.Get(decoded,"Snapshot")!).Payload.Settings;Assert.That(roundTrip.AudioGroups.Single().Numerator,Is.EqualTo(75));Assert.That(roundTrip.Accessibility!.ReducedMotion,Is.True);
            var v2=Activator.CreateInstance(codecType,new object?[]{json,f.Profile,f.Catalogs,f.Public,f.Campaign,f.Completion,retained,null,null})!;
            Assert.Throws<TargetInvocationException>(()=>LocalSaveContractFixtures.Call(v2,"Encode",snapshot,Utc,"candidate","candidate"));
            var files=Activator.CreateInstance(assembly.GetType("STP.Infrastructure.Persistence.SaveFileSystem",true)!,new object?[]{Path.GetFullPath(Path.Combine(root,".utmp")),f.Profile,f.Catalogs.AggregateCatalog,null})!;
            string directory=(string)LocalSaveContractFixtures.Get(files,"ProfileDirectory")!;Assert.That(directory,Does.Contain("qa-mobile-candidate"));Assert.That(directory,Does.Not.Contain("contract-test-v1"));Assert.That(directory.Split(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar).Count(p=>p==SaveProfileIdentity.MobileCandidateNamespace),Is.EqualTo(1));
        }
    }
}




