using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using STP.Application;
using STP.Infrastructure.Content;
using STP.Presentation.UI;
using UnityEngine;

namespace STP.Bootstrap
{
    /// <summary>The complete raw/UI/public-bound pack set. Resolving never creates a run or exposes a puzzle.</summary>
    public sealed class HumanManifestRegistry : IHumanManifestResolver
    {
        private readonly Dictionary<string, HumanRoutingManifest> byHash = new Dictionary<string, HumanRoutingManifest>(StringComparer.Ordinal);
        private readonly Dictionary<string, string> hashByPath = new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly Dictionary<string, string> pathByHash = new Dictionary<string, string>(StringComparer.Ordinal);
        public HumanManifestRegistry(TextAsset[] packs, TextAsset rawLock, TextAsset uiLock, TextAsset publicData, string publicHash)
            : this(packs, rawLock, uiLock, new MobileCandidateData(publicData.bytes, publicHash), new CanonicalJsonAdapter()) { }
        public HumanManifestRegistry(TextAsset[] packs, TextAsset rawLock, TextAsset uiLock, IPublicPuzzleCatalog currentPublic, ICanonicalJson json)
        {
            if (packs == null || rawLock == null || uiLock == null || currentPublic == null || json == null)
                throw new ArgumentNullException(nameof(packs));
            Require(packs.Length == 17 && packs.All(p => p != null), "HUMAN_PACK_SET_REQUIRED");
            var sources = new Dictionary<string, TextAsset>(StringComparer.Ordinal);
            foreach (var asset in packs)
            {
                string path = asset.name + ".json";
                Require(!sources.ContainsKey(path), "HUMAN_PACK_ASSET_ALIAS"); sources.Add(path, asset);
            }
            var parsed = StrictJsonParser.Parse(rawLock.bytes, new JsonParserLimits(32768, 5, 256, 128, 32));
            Require(parsed.Succeeded, "HUMAN_RAW_LOCK_INVALID"); var root = parsed.Value!;
            Fields(root, "schemaVersion", "kind", "uiSourceSha256", "defaultManifest", "manifests");
            Require(Get(root, "schemaVersion").Kind == JsonKind.Integer && Get(root, "schemaVersion").IntegerValue == 1 &&
                Text(Get(root, "kind")) == "HUMAN_ASSIGNMENT_RAW_LOCK", "HUMAN_RAW_LOCK_HEADER");
            string uiHash = LocalSaveHash.Bytes(uiLock.bytes);
            Require(Text(Get(root, "uiSourceSha256")) == uiHash, "HUMAN_UI_RAW_LOCK_MISMATCH");
            var paths = ExpectedPackPaths();
            Require(paths.Take(9).Contains(Text(Get(root, "defaultManifest"))), "HUMAN_DEFAULT_PACK_INVALID");
            var rows = Get(root, "manifests");
            Require(rows.Kind == JsonKind.Array && rows.Items.Count == paths.Length, "HUMAN_RAW_LOCK_PACK_SET");
            var options = new List<HumanOperatorPack>(); var studies = new HashSet<string>(StringComparer.Ordinal);
            var identities = new HashSet<PublicPuzzleIdentity>(); string? build = null;
            for (int i = 0; i < paths.Length; i++)
            {
                var row = rows.Items[i]; Fields(row, "path", "sha256", "studyId", "assignmentCount");
                string path = Text(Get(row, "path")), hash = Text(Get(row, "sha256"));
                Require(path == paths[i] && sources.TryGetValue(path, out _), "HUMAN_RAW_LOCK_PACK_ORDER");
                var source = sources[path];
                Require(Get(row, "assignmentCount").Kind == JsonKind.Integer && Get(row, "assignmentCount").IntegerValue == 6,
                    "HUMAN_RAW_LOCK_PACK_COUNT");
                var catalog = new HumanAssignmentCatalog(source.bytes, hash, currentPublic, json);
                Require(catalog.UiSourceSha256 == uiHash && catalog.Assignments.Count == 6 &&
                    catalog.StudyId == Text(Get(row, "studyId")) && studies.Add(catalog.StudyId), "HUMAN_PACK_BINDING_MISMATCH");
                build ??= catalog.BuildId; Require(catalog.BuildId == build, "HUMAN_PACK_BUILD_MISMATCH");
                var purpose = i < 9 ? HumanAssignmentPurpose.CAMPAIGN_TIME : HumanAssignmentPurpose.ENDLESS_PROFILE;
                Require(catalog.Assignments.All(a => a.Purpose == purpose && identities.Add(a.Puzzle.Identity)), "HUMAN_PACK_PURPOSE_OR_ALIAS");
                Require(!byHash.ContainsKey(hash), "HUMAN_PACK_HASH_ALIAS");
                byHash.Add(hash, new HumanRoutingManifest(source.bytes, catalog)); hashByPath.Add(path, hash); pathByHash.Add(hash, path);
                options.Add(new HumanOperatorPack(path, 6));
            }
            Require(identities.Count == 102, "HUMAN_PACK_INPUT_COUNT");
            ManifestBytesSha256 = Array.AsReadOnly(paths.Select(p => hashByPath[p]).ToArray());
            Options = options.AsReadOnly();
        }
        public IReadOnlyList<string> ManifestBytesSha256 { get; }
        public IReadOnlyList<HumanOperatorPack> Options { get; }
        public HumanRoutingManifest Resolve(string hash) => byHash.TryGetValue(hash, out var value) ? value : throw new ArgumentException("HUMAN_PACK_UNKNOWN");
        public string HashForPath(string path) => hashByPath.TryGetValue(path, out var value) ? value : throw new ArgumentException("HUMAN_PACK_UNKNOWN");
        public string PathForHash(string hash) => pathByHash.TryGetValue(hash, out var value) ? value : throw new ArgumentException("HUMAN_PACK_UNKNOWN");
        public static string[] ExpectedPackPaths() => Enumerable.Range(1, 9).Select(i => "campaign-pack-" + i.ToString("00", CultureInfo.InvariantCulture) + ".json")
            .Concat(Enumerable.Range(1, 8).Select(i => "endless-pack-" + i.ToString("00", CultureInfo.InvariantCulture) + ".json")).ToArray();
        private static JsonValue Get(JsonValue root, string key) => root.Get(key) ?? throw new ArgumentException("HUMAN_RAW_LOCK_FIELDS");
        private static string Text(JsonValue value) { Require(value.Kind == JsonKind.String, "HUMAN_RAW_LOCK_TEXT"); return value.StringValue; }
        private static void Fields(JsonValue value, params string[] fields) => Require(value.Kind == JsonKind.Object && value.Members.Count == fields.Length &&
            value.Members.All(m => fields.Contains(m.Name)), "HUMAN_RAW_LOCK_FIELDS");
        private static void Require(bool value, string code) { if (!value) throw new ArgumentException(code); }
    }
}
