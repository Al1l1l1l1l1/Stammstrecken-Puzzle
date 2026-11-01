using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using STP.Application.Ports;

namespace STP.Application
{
    /// <summary>Immutable cosmetics-v2 read model. Acquisition fields are mutually exclusive.</summary>
    public sealed class CosmeticDefinition
    {
        private CosmeticDefinition(string id, string type, string status, string titleKey, string assetAddress,
            string acquisition, long? price, IReadOnlyList<string> subjects)
        {
            if (id == null || !Regex.IsMatch(id, @"^[a-z][a-z0-9-]*$")) throw new ArgumentException("COSMETIC_ID_INVALID");
            Id = id; Type = LocalModel.One(type, "OPERATING_OBJECT|LIVERY|TRAIN_VARIANT");
            Status = LocalModel.One(status, "DRAFT|ACTIVE|HIDDEN|TOMBSTONE");
            if (titleKey == null || !Regex.IsMatch(titleKey, @"^[a-z][a-z0-9]*(?:[.-][a-z0-9]+)+$")) throw new ArgumentException("COSMETIC_TITLE_KEY_INVALID");
            TitleKey = titleKey; AssetAddress = LocalModel.Text(assetAddress);
            Acquisition = LocalModel.One(acquisition, "DEFAULT|PATIENCE_PURCHASE|MILESTONE_GRANT");
            PricePatience = price; RequiredCampaignSubjectIds = LocalModel.Copy(subjects);
            LocalModel.Unique(RequiredCampaignSubjectIds);
            if (acquisition == "PATIENCE_PURCHASE") LocalModel.Require(price.HasValue && LocalModel.Positive(price.Value) > 0 && subjects.Count == 0, "COSMETIC_ACQUISITION_INVALID");
            else LocalModel.Require(!price.HasValue && (acquisition == "MILESTONE_GRANT" ? subjects.Count > 0 : subjects.Count == 0), "COSMETIC_ACQUISITION_INVALID");
            foreach (string subject in subjects) LocalModel.Require(Regex.IsMatch(subject, @"^S[1-9][0-9]*(?:-[0-9]{2}){0,2}$"), "COSMETIC_SUBJECT_INVALID");
        }
        /// <summary>Creates a schema-equivalent discriminated definition.</summary>
        public static CosmeticDefinition Create(string id, string type, string status, string titleKey, string assetAddress,
            string acquisition, long? pricePatience, IReadOnlyList<string> requiredCampaignSubjectIds)
            => new CosmeticDefinition(id, type, status, titleKey, assetAddress, acquisition, pricePatience, requiredCampaignSubjectIds);
        /// <summary>Stable canonical item ID.</summary>
        public string Id { get; }
        /// <summary>OPERATING_OBJECT, LIVERY or TRAIN_VARIANT.</summary>
        public string Type { get; }
        /// <summary>Closed authoring/runtime status.</summary>
        public string Status { get; }
        /// <summary>Localization key resolved by presentation.</summary>
        public string TitleKey { get; }
        /// <summary>Typed asset-manifest address.</summary>
        public string AssetAddress { get; }
        /// <summary>Exactly one acquisition mode.</summary>
        public string Acquisition { get; }
        /// <summary>Authoritative locked price, present only for purchases.</summary>
        public long? PricePatience { get; }
        /// <summary>Version-1 ALL_FIRST_CLEARS_OF_CAMPAIGN_SUBJECTS prerequisites.</summary>
        public IReadOnlyList<string> RequiredCampaignSubjectIds { get; }
    }

    /// <summary>Application-owned read port over a strict, hash-locked cosmetics-v2 source.</summary>
    public interface ILockedCosmeticsCatalog : ICosmeticsCatalog
    {
        /// <summary>Exact catalog provenance and consumer isolation.</summary>
        CatalogIdentity Identity { get; }
        /// <summary>Immutable canonical-order items.</summary>
        IReadOnlyList<CosmeticDefinition> Items { get; }
        /// <summary>Exact stable-ID lookup; absence never substitutes an item.</summary>
        bool TryGet(string itemId, out CosmeticDefinition? item);
    }
}
