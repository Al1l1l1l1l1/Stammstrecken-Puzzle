using System;
using System.Collections.Generic;
using System.Text;
using STP.Application;

namespace STP.Infrastructure.Persistence
{
    /// <summary>Exact pre-WAGON catalog provenance. These passive bytes never grant or reprice ownership.</summary>
    internal static class RetainedCosmeticsHistory
    {
        // Original revision-1 items captured from HEAD 29599ba before the owner-requested catalog extension.
        private const string Items = @"[{""acquisition"":""DEFAULT"",""assetAddress"":""stp/cosmetics/train-stammrot-default"",""id"":""train-stammrot-default"",""status"":""DRAFT"",""titleKey"":""cosmetic.train-stammrot-default.title"",""type"":""TRAIN_VARIANT""},{""acquisition"":""PATIENCE_PURCHASE"",""assetAddress"":""stp/cosmetics/obj-wartebank-ohne-anschluss"",""id"":""obj-wartebank-ohne-anschluss"",""pricePatience"":120,""status"":""DRAFT"",""titleKey"":""cosmetic.obj-wartebank-ohne-anschluss.title"",""type"":""OPERATING_OBJECT""},{""acquisition"":""PATIENCE_PURCHASE"",""assetAddress"":""stp/cosmetics/obj-wegweiser-vielleicht"",""id"":""obj-wegweiser-vielleicht"",""pricePatience"":120,""status"":""DRAFT"",""titleKey"":""cosmetic.obj-wegweiser-vielleicht.title"",""type"":""OPERATING_OBJECT""},{""acquisition"":""PATIENCE_PURCHASE"",""assetAddress"":""stp/cosmetics/obj-absperrung-laufzeit-offen"",""id"":""obj-absperrung-laufzeit-offen"",""pricePatience"":120,""status"":""DRAFT"",""titleKey"":""cosmetic.obj-absperrung-laufzeit-offen.title"",""type"":""OPERATING_OBJECT""},{""acquisition"":""PATIENCE_PURCHASE"",""assetAddress"":""stp/cosmetics/obj-anzeiger-in-pruefung"",""id"":""obj-anzeiger-in-pruefung"",""pricePatience"":120,""status"":""DRAFT"",""titleKey"":""cosmetic.obj-anzeiger-in-pruefung.title"",""type"":""OPERATING_OBJECT""},{""acquisition"":""PATIENCE_PURCHASE"",""assetAddress"":""stp/cosmetics/livery-betriebsrot"",""id"":""livery-betriebsrot"",""pricePatience"":300,""status"":""DRAFT"",""titleKey"":""cosmetic.livery-betriebsrot.title"",""type"":""LIVERY""},{""acquisition"":""PATIENCE_PURCHASE"",""assetAddress"":""stp/cosmetics/livery-baustellengelb"",""id"":""livery-baustellengelb"",""pricePatience"":300,""status"":""DRAFT"",""titleKey"":""cosmetic.livery-baustellengelb.title"",""type"":""LIVERY""},{""acquisition"":""PATIENCE_PURCHASE"",""assetAddress"":""stp/cosmetics/obj-bahnsteig-provisorisch"",""id"":""obj-bahnsteig-provisorisch"",""pricePatience"":300,""status"":""DRAFT"",""titleKey"":""cosmetic.obj-bahnsteig-provisorisch.title"",""type"":""OPERATING_OBJECT""},{""acquisition"":""PATIENCE_PURCHASE"",""assetAddress"":""stp/cosmetics/obj-tunnelportal-wartung"",""id"":""obj-tunnelportal-wartung"",""pricePatience"":650,""status"":""DRAFT"",""titleKey"":""cosmetic.obj-tunnelportal-wartung.title"",""type"":""OPERATING_OBJECT""},{""acquisition"":""PATIENCE_PURCHASE"",""assetAddress"":""stp/cosmetics/obj-sev-bucht-geplant"",""id"":""obj-sev-bucht-geplant"",""pricePatience"":650,""status"":""DRAFT"",""titleKey"":""cosmetic.obj-sev-bucht-geplant.title"",""type"":""OPERATING_OBJECT""},{""acquisition"":""PATIENCE_PURCHASE"",""assetAddress"":""stp/cosmetics/train-ersatzgarnitur"",""id"":""train-ersatzgarnitur"",""pricePatience"":1200,""status"":""DRAFT"",""titleKey"":""cosmetic.train-ersatzgarnitur.title"",""type"":""TRAIN_VARIANT""},{""acquisition"":""PATIENCE_PURCHASE"",""assetAddress"":""stp/cosmetics/train-anschlusswunder"",""id"":""train-anschlusswunder"",""pricePatience"":2500,""status"":""DRAFT"",""titleKey"":""cosmetic.train-anschlusswunder.title"",""type"":""TRAIN_VARIANT""},{""acquisition"":""MILESTONE_GRANT"",""assetAddress"":""stp/cosmetics/obj-signal-freigegeben"",""id"":""obj-signal-freigegeben"",""milestoneEligibility"":{""contractVersion"":1,""kind"":""ALL_FIRST_CLEARS_OF_CAMPAIGN_SUBJECTS"",""requiredCampaignSubjectIds"":[""S1-01""]},""status"":""DRAFT"",""titleKey"":""cosmetic.obj-signal-freigegeben.title"",""type"":""OPERATING_OBJECT""},{""acquisition"":""MILESTONE_GRANT"",""assetAddress"":""stp/cosmetics/obj-tafel-trotzdem-weiter"",""id"":""obj-tafel-trotzdem-weiter"",""milestoneEligibility"":{""contractVersion"":1,""kind"":""ALL_FIRST_CLEARS_OF_CAMPAIGN_SUBJECTS"",""requiredCampaignSubjectIds"":[""S1""]},""status"":""DRAFT"",""titleKey"":""cosmetic.obj-tafel-trotzdem-weiter.title"",""type"":""OPERATING_OBJECT""}]";

        internal static IReadOnlyList<RetainedCosmeticsBinding> WithKnownRevisionOne(RetainedCosmeticsBinding current,
            byte[] campaignSource, IPublicPuzzleCatalog puzzles, ICanonicalJson json)
        {
            var identity = current.Identity;
            if (identity.Revision < 2) return Array.AsReadOnly(new[] { current });
            string? hash = null; string status = "";
            // These existing lock values are retained, never recalculated from today's catalog.
            if (identity.Use == CatalogUse.InternalPreview && identity.CatalogId == "qa-mobile-candidate-cosmetics")
            { hash = "87af7d75d155851d5ca6e7602b9459a4d20ca68c55a78756d6e552deec5e4592"; status = "DRAFT"; }
            else if (identity.Use == CatalogUse.ContractTest && identity.CatalogId == "fixture-import-cosmetics")
            { hash = "2ce2e6b6ba7e4ac5327887784e2ff63aa105f8bc8a33bd2fa34c1f13f08339ba"; status = "FIXTURE_ONLY"; }
            if (hash == null) return Array.AsReadOnly(new[] { current });
            string items = status == "FIXTURE_ONLY" ? Items.Replace("\"status\":\"DRAFT\"", "\"status\":\"ACTIVE\"") : Items;
            string source = "{\"approvalStatus\":\"" + status + "\",\"catalogId\":\"" + identity.CatalogId + "\",\"catalogRevision\":1,\"items\":" + items + ",\"schemaVersion\":2}";
            var historical = RetainedCosmeticsBinding.FromDocuments(Encoding.UTF8.GetBytes(source), campaignSource, json, identity.Use, puzzles);
            LocalSaveCodec.Require(historical.Identity.HashSha256 == hash, "SAVE_COSMETIC_HISTORY_HASH_MISMATCH", true);
            return Array.AsReadOnly(new[] { current, historical });
        }
    }
}
