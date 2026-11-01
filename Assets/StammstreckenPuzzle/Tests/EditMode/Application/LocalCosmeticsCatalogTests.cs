using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using STP.Application;

namespace STP.Tests.Application.EditMode
{
    public sealed class LocalCosmeticsCatalogTests
    {
        private const string Item = "{\"id\":\"object-one\",\"type\":\"OPERATING_OBJECT\",\"status\":\"ACTIVE\",\"titleKey\":\"cosmetic.one.title\",\"assetAddress\":\"stp/one\",\"acquisition\":\"PATIENCE_PURCHASE\",\"pricePatience\":120}";
        private static string Source(string items) => "{\"schemaVersion\":2,\"catalogId\":\"test-cosmetics\",\"catalogRevision\":1,\"approvalStatus\":\"FIXTURE_ONLY\",\"items\":[" + items + "]}";
        private static ILockedCosmeticsCatalog Read(string text, CatalogUse use = CatalogUse.ContractTest, string? forcedHash = null)
        {
            var bytes = Encoding.UTF8.GetBytes(text); var json = LocalSaveContractFixtures.Json;
            var canonical = json.Canonicalize(bytes, LocalSaveHash.Limits);
            string hash = forcedHash ?? (canonical.Succeeded ? LocalSaveHash.Bytes(canonical.GetUtf8()!) : new string('a', 64));
            return LocalSaveContractFixtures.Construct<ILockedCosmeticsCatalog>("STP.Infrastructure.Content", "STP.Infrastructure.Content.LockedCosmeticsCatalog", bytes, hash, use, json);
        }
        [Test]
        public void LockedPriceIsStrictAndFormattingDoesNotChangeHash()
        {
            var first = Read(Source(Item)); var second = Read("\n" + Source(Item).Replace(",", ",\n") + "\n");
            Assert.That(first.Identity.HashSha256, Is.EqualTo(second.Identity.HashSha256)); Assert.That(first.Items.Single().PricePatience, Is.EqualTo(120));
            Assert.That(first.TryGet("absent", out var absent), Is.False); Assert.That(absent, Is.Null);
            Assert.Throws<TargetInvocationException>(() => Read(Source(Item), forcedHash: new string('b', 64)));
        }
        [TestCase("duplicate-id")][TestCase("duplicate-key")][TestCase("unknown-field")][TestCase("mixed-acquisition")][TestCase("wrong-type")]
        public void MalformedOrAmbiguousCatalogIsRejected(string mutation)
        {
            string text = Source(Item);
            if (mutation == "duplicate-id") text = Source(Item + "," + Item);
            if (mutation == "duplicate-key") text = text.Replace("\"pricePatience\":120", "\"pricePatience\":120,\"pricePatience\":120");
            if (mutation == "unknown-field") text = text.Replace("\"pricePatience\":120", "\"pricePatience\":120,\"unexpected\":1");
            if (mutation == "mixed-acquisition") text = text.Replace("\"PATIENCE_PURCHASE\"", "\"DEFAULT\"");
            if (mutation == "wrong-type") text = text.Replace("\"OPERATING_OBJECT\"", "\"GAMEPLAY_UPGRADE\"");
            Assert.Throws<TargetInvocationException>(() => Read(text));
        }
        [Test]
        public void DraftCannotBeImportedAsProductionAndFourteenItemCatalogPreservesP06PricesAndMilestones()
        {
            string root = Environment.GetEnvironmentVariable("STP_SOURCE_ROOT") ?? Directory.GetCurrentDirectory();
            string text = File.ReadAllText(Path.Combine(root, "Content", "Catalogs", "s1", "cosmetics-v2.json"));
            Assert.Throws<TargetInvocationException>(() => Read(text, CatalogUse.Production));
            var catalog = Read(text, CatalogUse.IsolatedReview);
            Assert.That(catalog.Items.Count, Is.EqualTo(14)); Assert.That(catalog.Items.All(i => i.Status == "DRAFT"), Is.True);
            Assert.That(catalog.Items.Count(i => i.Acquisition == "DEFAULT"), Is.EqualTo(1));
            Assert.That(catalog.Items.Where(i => i.Acquisition == "PATIENCE_PURCHASE").Select(i => i.PricePatience).OrderBy(p => p), Is.EqualTo(new long?[] { 120,120,120,120,300,300,300,650,650,1200,2500 }));
            Assert.That(catalog.Items.Single(i => i.Id == "obj-signal-freigegeben").RequiredCampaignSubjectIds, Is.EqualTo(new[] { "S1-01" }));
            Assert.That(catalog.Items.Single(i => i.Id == "obj-tafel-trotzdem-weiter").RequiredCampaignSubjectIds, Is.EqualTo(new[] { "S1" }));
        }
    }
}
