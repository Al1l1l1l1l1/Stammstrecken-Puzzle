using System;
using System.Collections.Generic;
using NUnit.Framework;
using STP.Application;

namespace STP.Tests.Application.EditMode
{
    public sealed class CatalogSetIdentityTests
    {
        [Test]
        public void ExactIndependentNodeAggregateAndAllRoleBindingsArePreserved()
        {var s=new LocalSaveContractFixtures.Setup();Assert.That(s.Catalogs.AggregateCatalog.HashSha256,Is.EqualTo(LocalSaveContractFixtures.Goldens.Get("catalogs").Get("catalogSet").Get("aggregateCatalog").Get("hashSha256").Text));Assert.That(s.Catalogs.PublicCatalog,Is.EqualTo(s.PublicId));Assert.That(s.Catalogs.CampaignCatalog,Is.EqualTo(s.CampaignId));Assert.That(s.Catalogs.CompletionCatalog,Is.EqualTo(s.CompletionId));}
        [Test]
        public void SameCatalogInDifferentRolesIsRejectedDespiteItsCorrectRecomputedAggregate()
        {
            var s=new LocalSaveContractFixtures.Setup();var p=s.PublicId;string part=LocalSaveHash.CatalogProjection(p);string source="{\"publicCatalog\":"+part+",\"campaignCatalog\":"+part+",\"completionCatalog\":"+part+"}";var aggregate=new CatalogIdentity("contract-duplicate-roles",1,LocalSaveHash.Bytes(LocalSaveHash.Canonical(LocalSaveContractFixtures.Json,source)),CatalogUse.ContractTest,CatalogApprovalStatus.FixtureOnly);
            Assert.Throws<ArgumentException>(()=>CatalogSetIdentity.Create(p,p,p,aggregate,LocalSaveContractFixtures.Json));
        }
        [Test]
        public void MutationOfAnyPartOrCrossUseBreaksAggregateBinding()
        {var s=new LocalSaveContractFixtures.Setup();var changed=new CatalogIdentity(s.PublicId.CatalogId,2,s.PublicId.HashSha256,CatalogUse.ContractTest,CatalogApprovalStatus.FixtureOnly);Assert.Throws<ArgumentException>(()=>CatalogSetIdentity.Create(changed,s.CampaignId,s.CompletionId,s.Aggregate,LocalSaveContractFixtures.Json));var review=new CatalogIdentity("qa-public240-v1-test",1,new string('a',64),CatalogUse.IsolatedReview,CatalogApprovalStatus.Draft);Assert.Throws<ArgumentException>(()=>CatalogSetIdentity.Create(review,s.CampaignId,s.CompletionId,s.Aggregate,LocalSaveContractFixtures.Json));}
    }
    public sealed class LocalProfileStateTests
    {
        [Test]
        public void CompleteTypedPayloadCopiesAndProtectsEveryAuthoritativeCollection()
        {var s=new LocalSaveContractFixtures.Setup();var p=s.Empty;var levels=new LocalLevelProgress[0];var journal=new LocalStoredEntry[0];var state=LocalProfileState.Create(p.Profile,levels,p.Endless,p.LedgerCheckpoint,journal,p.Inventory,p.CosmeticClaimReservations,p.SeasonProgressCache,p.Drafts,p.RewardClaims,p.DailyStatus,p.Entitlements,p.PendingOperations,p.Settings,p.DataLifecycle,p.HintEntitlement);Assert.That(state.Levels,Is.Not.SameAs(levels));Assert.That(state.EconomyJournal,Is.Not.SameAs(journal));Assert.Throws<NotSupportedException>(()=>((IList<LocalLevelProgress>)state.Levels).Add(null!));Assert.Throws<NotSupportedException>(()=>((IList<string>)state.Inventory.SelectedItemIds).Add("injected"));Assert.That(state.Profile,Is.SameAs(p.Profile));Assert.That(state.HintEntitlement,Is.Null);}
        [Test]
        public void ClosedRecordFactoriesRejectContradictoryOrInventedFacts()
        {var s=new LocalSaveContractFixtures.Setup();var id=s.Puzzles.Puzzles[0].Identity;Assert.Throws<ArgumentException>(()=>LocalLevelProgress.Create(id,null,0,null,null,true,null,null));Assert.Throws<ArgumentException>(()=>LocalInventory.Create(Array.Empty<LocalOwnership>(),new[]{"unowned"}));Assert.Throws<ArgumentException>(()=>LocalAudioPreference.Create("master",2,1));Assert.Throws<ArgumentException>(()=>LocalEntitlement.Create("remove_ads","store","txn",true,false,true,null,null,null));Assert.Throws<ArgumentException>(()=>LocalStoredEntry.Create(1,"t","r",0,"s",null,1,null,null,"2026-10-19T00:00:00Z",new string('a',64)));}
    }
}
