using System;
using System.Collections.Generic;
using System.Linq;
using STP.Application;

namespace STP.Infrastructure.Content
{
    /// <summary>Strict cosmetics-v2 adapter. A supplied release hash is checked before exposing any item.</summary>
    public sealed class LockedCosmeticsCatalog : ILockedCosmeticsCatalog
    {
        private readonly IReadOnlyList<CosmeticDefinition> items;
        /// <summary>Parses bounded strict JSON, validates the closed schema and verifies the entire JCS catalog hash.</summary>
        public LockedCosmeticsCatalog(byte[] source, string expectedHashSha256, CatalogUse use, ICanonicalJson canonicalJson)
        {
            if (canonicalJson == null) throw new ArgumentNullException(nameof(canonicalJson));
            var canonical = canonicalJson.Canonicalize(source, new JsonResourceLimits(1048576, 16, 4096, 4096, 32));
            Require(canonical.Succeeded, canonical.ErrorCode ?? "COSMETICS_JSON_INVALID");
            byte[] bytes = canonical.GetUtf8()!;
            Require(LocalSaveHash.Bytes(bytes) == expectedHashSha256, "COSMETICS_CATALOG_HASH_MISMATCH");
            var parsed = StrictJsonParser.Parse(bytes, new JsonParserLimits(1048576, 16, 4096, 4096, 32));
            Require(parsed.Succeeded, parsed.Error?.Code ?? "COSMETICS_JSON_INVALID");
            var root = parsed.Value!;
            Fields(root, "schemaVersion", "catalogId", "catalogRevision", "approvalStatus", "items");
            Require(Integer(Need(root, "schemaVersion")) == 2, "COSMETICS_SCHEMA_UNSUPPORTED");
            string status = Text(Need(root, "approvalStatus"));
            CatalogApprovalStatus approval = status == "DRAFT" ? CatalogApprovalStatus.Draft : status == "FIXTURE_ONLY" ? CatalogApprovalStatus.FixtureOnly : status == "PRODUCT_APPROVED" ? CatalogApprovalStatus.ProductApproved : throw new ArgumentException("COSMETICS_APPROVAL_INVALID");
            long revision = Integer(Need(root, "catalogRevision"));
            Require(revision >= 1 && revision <= int.MaxValue, "COSMETICS_REVISION_INVALID");
            Identity = new CatalogIdentity(Text(Need(root, "catalogId")), (int)revision, expectedHashSha256, use, approval);
            var array = Array(Need(root, "items")); Require(array.Count > 0 && array.Count <= 1024, "COSMETICS_ITEM_LIMIT");
            var result = new List<CosmeticDefinition>();
            foreach (var value in array)
            {
                string acquisition = Text(Need(value, "acquisition")); long? price = null; string[] subjects = System.Array.Empty<string>();
                string[] common = { "id", "type", "status", "titleKey", "assetAddress", "acquisition" };
                if (acquisition == "DEFAULT") Fields(value, common);
                else if (acquisition == "PATIENCE_PURCHASE") { Fields(value, common.Concat(new[] { "pricePatience" }).ToArray()); price = Integer(Need(value, "pricePatience")); }
                else if (acquisition == "MILESTONE_GRANT")
                {
                    Fields(value, common.Concat(new[] { "milestoneEligibility" }).ToArray());
                    var eligibility = Need(value, "milestoneEligibility"); Fields(eligibility, "contractVersion", "kind", "requiredCampaignSubjectIds");
                    Require(Integer(Need(eligibility, "contractVersion")) == 1 && Text(Need(eligibility, "kind")) == "ALL_FIRST_CLEARS_OF_CAMPAIGN_SUBJECTS", "COSMETICS_ELIGIBILITY_UNSUPPORTED");
                    subjects = Array(Need(eligibility, "requiredCampaignSubjectIds")).Select(Text).ToArray();
                }
                else throw new ArgumentException("COSMETICS_ACQUISITION_UNSUPPORTED");
                result.Add(CosmeticDefinition.Create(Text(Need(value, "id")), Text(Need(value, "type")), Text(Need(value, "status")), Text(Need(value, "titleKey")), Text(Need(value, "assetAddress")), acquisition, price, subjects));
            }
            Require(result.Select(x => x.Id).Distinct(StringComparer.Ordinal).Count() == result.Count, "COSMETICS_DUPLICATE_ID");
            items = System.Array.AsReadOnly(result.OrderBy(x => x.Id, StringComparer.Ordinal).ToArray());
        }
        /// <summary>Hash-locked source identity.</summary>
        public CatalogIdentity Identity { get; }
        /// <summary>Read-only definitions.</summary>
        public IReadOnlyList<CosmeticDefinition> Items => items;
        /// <summary>Ordinal exact lookup.</summary>
        public bool TryGet(string itemId, out CosmeticDefinition? item) { item = items.SingleOrDefault(x => x.Id == itemId); return item != null; }
        private static void Require(bool condition, string code) { if (!condition) throw new ArgumentException(code); }
        private static JsonValue Need(JsonValue value, string name) => value.Get(name) ?? throw new ArgumentException("COSMETICS_FIELDS_INVALID");
        private static string Text(JsonValue value) => value.Kind == JsonKind.String ? value.StringValue : throw new ArgumentException("COSMETICS_TYPE_INVALID");
        private static long Integer(JsonValue value) => value.Kind == JsonKind.Integer ? value.IntegerValue : throw new ArgumentException("COSMETICS_TYPE_INVALID");
        private static IReadOnlyList<JsonValue> Array(JsonValue value) => value.Kind == JsonKind.Array ? value.Items : throw new ArgumentException("COSMETICS_TYPE_INVALID");
        private static void Fields(JsonValue value, params string[] names) => Require(value.Kind == JsonKind.Object && value.Members.Count == names.Length && value.Members.All(m => names.Contains(m.Name, StringComparer.Ordinal)), "COSMETICS_FIELDS_INVALID");
    }
}
