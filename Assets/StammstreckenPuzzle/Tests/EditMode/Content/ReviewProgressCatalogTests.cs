using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using STP.Infrastructure.Content;

namespace STP.Tests.Content.EditMode
{
    public sealed class ReviewProgressCatalogTests
    {
        private static byte[] Source(string file)=>File.ReadAllBytes(Path.Combine(Environment.GetEnvironmentVariable("STP_SOURCE_ROOT")??Directory.GetCurrentDirectory(),"Assets","StammstreckenPuzzle","UI",file));
        private static object Public(string use)=>Assembly.Load("STP.Tests.Application.EditMode").GetType("STP.Tests.Application.EditMode.LocalSaveContractFixtures",true)!.GetMethod("CreateReviewPublicCatalog")!.Invoke(null,new object[]{use})!;
        private static object? P(object o,string name)=>o.GetType().GetProperty(name)!.GetValue(o);
        private static ReviewProgressCatalog Build(string use="IsolatedReview",byte[]? campaign=null,byte[]? completion=null)=>(ReviewProgressCatalog)Activator.CreateInstance(typeof(ReviewProgressCatalog),Public(use),campaign??Source("ReviewCampaign.catalog.json"),completion??Source("ReviewCompletion.catalog.json"),new CanonicalJsonAdapter())!;
        private static string ShaCanonical(byte[] bytes){var json=StrictJsonParser.Parse(bytes).Value!;return LevelHashing.Sha256Hex(JcsSerializer.Serialize(json).Utf8!);}
        [Test]
        public void Exactly240InjectedIdentitiesHaveAllOpenMembershipLockedBaseRewardsAndExplicitNullTimes()
        {
            var a=Build();var campaign=P(a,"Campaign")!;var entries=((IEnumerable)P(campaign,"Entries")!).Cast<object>().ToArray();Assert.That(entries.Length,Is.EqualTo(240));Assert.That(entries.Select(e=>(string)P(P(e,"Puzzle")!,"PuzzleId")!).Distinct().Count(),Is.EqualTo(240));Assert.That(entries.Select(e=>P(e,"SectionId")).Distinct().Count(),Is.EqualTo(5));Assert.That(entries.Select(e=>P(e,"RouteId")).Distinct().Count(),Is.EqualTo(20));var publicCatalog=Public("IsolatedReview");var puzzles=((IEnumerable)P(publicCatalog,"Puzzles")!).Cast<object>().ToArray();var completion=P(a,"Completion")!;foreach(var e in entries){Assert.That(((IEnumerable)P(e,"RequiredFirstClears")!).Cast<object>(),Is.Empty);var identity=P(e,"Puzzle")!;Assert.That(puzzles.Any(p=>P(p,"Identity")!.Equals(identity)),Is.True);object?[] args={identity,null};Assert.That(completion.GetType().GetMethod("TryGet")!.Invoke(completion,args),Is.EqualTo(true));var v=args[1]!;Assert.That(P(v,"Puzzle"),Is.EqualTo(identity));Assert.That(P(v,"Catalog"),Is.EqualTo(P(completion,"Identity")));Assert.That(P(v,"FirstClear"),Is.EqualTo(15));Assert.That(P(v,"SecondStar"),Is.EqualTo(5));Assert.That(P(v,"ThirdStar"),Is.EqualTo(10));Assert.That(P(v,"RouteClear"),Is.EqualTo(60));Assert.That(P(v,"SectionClear"),Is.EqualTo(150));Assert.That(P(v,"TwoStarMs"),Is.Null);Assert.That(P(v,"ThreeStarMs"),Is.Null);}Assert.That(P(P(campaign,"Identity")!,"Use")!.ToString(),Is.EqualTo("IsolatedReview"));Assert.That(P(P(campaign,"Identity")!,"ApprovalStatus")!.ToString(),Is.EqualTo("Draft"));
        }
        [Test]
        public void CompletionIdentityCoversEntireWrapperAndValidSourceChangesWithoutMutatingInput()
        {
            var bytes=Source("ReviewCompletion.catalog.json");var original=(byte[])bytes.Clone();var a=Build(completion:bytes);Assert.That(bytes,Is.EqualTo(original));var completion=P(a,"Completion")!;var id=P(completion,"Identity")!;Assert.That(P(id,"HashSha256"),Is.EqualTo(ShaCanonical(bytes)));var inner=StrictJsonParser.Parse(bytes).Value!.Get("completionCatalog")!;Assert.That(P(id,"HashSha256"),Is.Not.EqualTo(LevelHashing.Sha256Hex(JcsSerializer.Serialize(inner).Utf8!)));var text=Encoding.UTF8.GetString(bytes);var changed=Encoding.UTF8.GetBytes(text.Replace("review.result.first-clear","review.result.alternative"));Assert.That(changed,Is.Not.EqualTo(bytes),"Known result key must be changed by this input mutation");var b=Build(completion:changed);Assert.That(P(P(P(b,"Completion")!,"Identity")!,"HashSha256"),Is.Not.EqualTo(P(id,"HashSha256")));Assert.That(P(a,"CatalogSet"),Is.Not.EqualTo(P(b,"CatalogSet")));
        }
        [TestCase("Production")][TestCase("ContractTest")]
        public void ReviewAdapterCannotPromoteProductionOrFixturePublicCatalog(string use)
        {var e=Assert.Throws<TargetInvocationException>(()=>Build(use));Assert.That(e!.InnerException!.Message,Is.EqualTo("REVIEW_CATALOG_USE_INVALID"));}
        [TestCase("version")][TestCase("product")][TestCase("fixture")][TestCase("unknown")][TestCase("duplicate")][TestCase("membership")][TestCase("stars-zero")][TestCase("reward-command")]
        public void ClosedCompletionSourceRejectsVersionPromotionUnknownDuplicateMembershipAndInventedDefaults(string mutation)
        {
            string s=Encoding.UTF8.GetString(Source("ReviewCompletion.catalog.json"));switch(mutation){case "version":s=s.Replace("\"reviewCompletionContractVersion\": 1","\"reviewCompletionContractVersion\": 2");break;case "product":s=s.Replace("DRAFT","PRODUCT_APPROVED");break;case "fixture":s=s.Replace("DRAFT","FIXTURE_ONLY");break;case "unknown":s="{\"extra\":1,"+s.Substring(s.IndexOf('{')+1);break;case "duplicate":s="{\"reviewCompletionContractVersion\":1,"+s.Substring(s.IndexOf('{')+1);break;case "membership":s=s.Replace("S1-01-01-02","S1-01-01-01");break;case "stars-zero":s=s.Replace("\"twoStarMs\": null","\"twoStarMs\": 0");break;case "reward-command":s=s.Replace("\"amountPatience\": 15","\"amountPatience\": 999");break;}Assert.Throws<TargetInvocationException>(()=>Build(completion:Encoding.UTF8.GetBytes(s)));
        }
        [TestCase("unknown")][TestCase("duplicate")][TestCase("membership")][TestCase("gated")]
        public void ClosedCampaignSourceRejectsUnknownDuplicateIncorrectMembershipAndProductGates(string mutation)
        {
            string s=Encoding.UTF8.GetString(Source("ReviewCampaign.catalog.json"));switch(mutation){case "unknown":s="{\"extra\":1,"+s.Substring(s.IndexOf('{')+1);break;case "duplicate":s="{\"schemaVersion\":2,"+s.Substring(s.IndexOf('{')+1);break;case "membership":s=s.Replace("S1-01-01-02","S1-01-01-01");break;case "gated":s=s.Replace("ALWAYS","ALL_FIRST_CLEARED");break;}Assert.Throws<TargetInvocationException>(()=>Build(campaign:Encoding.UTF8.GetBytes(s)));
        }
    }
}
