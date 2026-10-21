using System;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Presentation.UI;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace STP.Tests.Presentation.PlayMode
{
    /// <summary>Explicit consumer separation; temporary JSON values are test memory, never promoted content assets.</summary>
    public sealed class ContractTestPresentationSourceTests
    {
#if UNITY_EDITOR
        private const string Ui="Assets/StammstreckenPuzzle/UI/";
        private static TextAsset Read(string name)=>AssetDatabase.LoadAssetAtPath<TextAsset>(Ui+name);
        private static TextAsset Status(TextAsset source,string status)=>new TextAsset(source.text.Replace("\"approvalStatus\": \"DRAFT\"","\"approvalStatus\": \""+status+"\""));
        [Test]
        public void CampaignText_FixtureFactoryAcceptsOnlyFixture_ExistingProductAndDraftConsumersStayNarrow()
        {
            var draft=Read("ProductionCampaignText.json");var fixture=Status(draft,"FIXTURE_ONLY");var product=Status(draft,"PRODUCT_APPROVED");
            try
            {
                Assert.Throws<ArgumentException>(()=>new ProductionCampaignText(draft));
                using(var text=new ProductionCampaignText(draft,true))Assert.That(text.Resolve(0,"section","route").Title,Is.Not.Empty);
                Assert.Throws<ArgumentException>(()=>new ProductionCampaignText(fixture));Assert.Throws<ArgumentException>(()=>new ProductionCampaignText(fixture,true));
                using(var text=ProductionCampaignText.ForContractTest(fixture))Assert.That(text.Resolve(239,"section","route").Result,Is.Not.Empty);
                Assert.Throws<ArgumentException>(()=>ProductionCampaignText.ForContractTest(draft));Assert.Throws<ArgumentException>(()=>ProductionCampaignText.ForContractTest(product));
                using(var text=new ProductionCampaignText(product))Assert.That(text.Resolve(0,"section","route").Title,Is.Not.Empty);
            }
            finally{UnityEngine.Object.DestroyImmediate(fixture);UnityEngine.Object.DestroyImmediate(product);}
        }
        [Test]
        public void RailwayAssets_FixtureFactoryNeverBroadensDefaultOrDraftOptIn()
        {
            var draft=Read("RailwayAssets.json");var fixture=Status(draft,"FIXTURE_ONLY");var product=Status(draft,"PRODUCT_APPROVED");
            try
            {
                Assert.Throws<ArgumentException>(()=>new RailwayAssetCatalog(draft));Assert.That(new RailwayAssetCatalog(draft,true).CosmeticAssets.Count,Is.EqualTo(14));
                Assert.Throws<ArgumentException>(()=>new RailwayAssetCatalog(fixture));Assert.Throws<ArgumentException>(()=>new RailwayAssetCatalog(fixture,true));
                Assert.That(RailwayAssetCatalog.ForContractTest(fixture).ResolvePuzzleMoment("S1-05-04-12",true).stage,Is.EqualTo(12));
                Assert.Throws<ArgumentException>(()=>RailwayAssetCatalog.ForContractTest(draft));Assert.Throws<ArgumentException>(()=>RailwayAssetCatalog.ForContractTest(product));
                Assert.That(new RailwayAssetCatalog(product).CosmeticAssets.Count,Is.EqualTo(14));
            }
            finally{UnityEngine.Object.DestroyImmediate(fixture);UnityEngine.Object.DestroyImmediate(product);}
        }
        [Test]
        public void Presenter_FixtureIntentUsesSameGallery_RejectsProductCrossingAndSecondBind()
        {
            var source=Read("RailwayAssets.json");var fixture=Status(source,"FIXTURE_ONLY");var product=Status(source,"PRODUCT_APPROVED");var textSource=Read("ProductionCampaignText.json");var fixtureText=Status(textSource,"FIXTURE_ONLY");var frame=AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Ui+"ProductionGame.uxml").CloneTree();
            try
            {
                using var presenter=new ProductionGamePresenter(frame,Read("ProductionGameLocale.json"),()=>0,_=>Task.CompletedTask);
                Assert.Throws<ArgumentException>(()=>presenter.BindContractTestAssetCatalog(source));Assert.Throws<ArgumentException>(()=>presenter.BindContractTestAssetCatalog(product));Assert.That(frame.Q("depot-catalog").childCount,Is.Zero);
                presenter.BindContractTestAssetCatalog(fixture);Assert.That(frame.Q("depot-catalog").childCount,Is.EqualTo(14));Assert.Throws<InvalidOperationException>(()=>presenter.BindContractTestAssetCatalog(fixture));Assert.Throws<InvalidOperationException>(()=>presenter.BindAssetCatalog(product));
                Assert.Throws<ArgumentException>(()=>presenter.BindContractTestCampaignText(textSource));presenter.BindContractTestCampaignText(fixtureText);Assert.Throws<InvalidOperationException>(()=>presenter.BindContractTestCampaignText(fixtureText));
            }
            finally{UnityEngine.Object.DestroyImmediate(fixture);UnityEngine.Object.DestroyImmediate(product);UnityEngine.Object.DestroyImmediate(fixtureText);}
        }
#endif
    }
}
