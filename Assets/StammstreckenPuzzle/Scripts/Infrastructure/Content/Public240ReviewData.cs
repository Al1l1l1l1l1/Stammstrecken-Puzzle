using System;
using System.Collections.Generic;
using System.Linq;
using STP.Application;

namespace STP.Infrastructure.Content
{
    /// <summary>Closed public-only catalog for the separately authorized ADR034 review. No authoring document is read here.</summary>
    public sealed class Public240ReviewData : IPublicPuzzleCatalog
    {
        /// <summary>Exact generated UTF8-LF public asset hash, independently recorded in the repository-only lock.</summary>
        public const string ExpectedAssetSha256 = "c21c9ef55ab34c369c3d6aa16c08c066a96ea9bcab4286ff73e3d402d33e17c2";
        private const string ExpectedCatalogHashSha256 = "f64721318b2fffa5179cfc3acef8aa2176115717c02f15c1f3a581dab6b562a4";
        private readonly IReadOnlyList<PublicPuzzleInput> puzzles;
        /// <summary>Validates every closed record, semantic hash, identity and domain mapping before publishing any entry.</summary>
        public Public240ReviewData(byte[] utf8)
        {
            var parsed = StrictJsonParser.Parse(utf8, new JsonParserLimits(1024 * 1024, 12, 1024, 1024, 16));
            if (!parsed.Succeeded) throw new ArgumentException("PUBLIC240_JSON:" + parsed.Error!.Code);
            var root = Shape(parsed.Value!, "schemaVersion", "catalogId", "revision", "use", "status", "puzzles", "catalogHashSha256");
            if (Integer(root.Get("schemaVersion")) != 1 || Text(root.Get("catalogId")) != "qa-public240-v1-public" ||
                Integer(root.Get("revision")) != 1 || Text(root.Get("use")) != "IsolatedReview" || Text(root.Get("status")) != "DRAFT")
                throw new ArgumentException("PUBLIC240_HEADER");
            var entries = root.Get("puzzles")!;
            if (entries.Kind != JsonKind.Array || entries.Items.Count != 240) throw new ArgumentException("PUBLIC240_COUNT");
            var items = new List<PublicPuzzleInput>();
            for (int index = 0; index < entries.Items.Count; index++)
            {
                var entry = Shape(entries.Items[index], "puzzleId", "rulesetVersion", "grid", "endpoints", "rowCounts", "columnCounts", "publicPuzzleHash");
                string expected = "S1-" + (index / 48 + 1).ToString("00", System.Globalization.CultureInfo.InvariantCulture) + "-" +
                    (index % 48 / 12 + 1).ToString("00", System.Globalization.CultureInfo.InvariantCulture) + "-" +
                    (index % 12 + 1).ToString("00", System.Globalization.CultureInfo.InvariantCulture);
                string id = Text(entry.Get("puzzleId"));
                if (id != expected) throw new ArgumentException("PUBLIC240_ID_ORDER");
                var grid = Shape(entry.Get("grid")!, "width", "height");
                int width = Integer(grid.Get("width")), height = Integer(grid.Get("height"));
                var endpoints = Shape(entry.Get("endpoints")!, "a", "b");
                var a = Shape(endpoints.Get("a")!, "side", "index"); var b = Shape(endpoints.Get("b")!, "side", "index");
                var aSide = Side(Text(a.Get("side"))); var bSide = Side(Text(b.Get("side")));
                int aIndex = Integer(a.Get("index")), bIndex = Integer(b.Get("index"));
                var rows = Counts(entry.Get("rowCounts")); var columns = Counts(entry.Get("columnCounts"));
                var hash = Shape(entry.Get("publicPuzzleHash")!, "profile", "sha256");
                var identity = new PublicPuzzleIdentity(id, Text(hash.Get("profile")), Text(hash.Get("sha256")));
                string ruleset = Text(entry.Get("rulesetVersion"));
                // Reuse the exact existing public projection, including validated side/index spelling.
                var projection = LevelHashing.PublicPuzzleProjection(id, ruleset, new LevelGridDto(width, height),
                    new LevelEndpointDto((LevelSide)aSide, aIndex), new LevelEndpointDto((LevelSide)bSide, bIndex), rows, columns);
                if (LevelHashing.HashProjection(projection) != identity.HashSha256) throw new ArgumentException("PUBLIC240_PUZZLE_HASH");
                items.Add(new PublicPuzzleInput(identity, ruleset, width, height, aSide, aIndex, bSide, bIndex, rows, columns));
            }
            string catalogHash = Text(root.Get("catalogHashSha256"));
            var payload = JsonValue.CreateObject(root.Members.Where(member => member.Name != "catalogHashSha256"));
            if (LevelHashing.HashProjection(payload) != catalogHash) throw new ArgumentException("PUBLIC240_CATALOG_HASH");
            if (catalogHash != ExpectedCatalogHashSha256) throw new ArgumentException("PUBLIC240_CATALOG_BINDING");
            Identity = new CatalogIdentity("qa-public240-v1-public", 1, catalogHash, CatalogUse.IsolatedReview, CatalogApprovalStatus.Draft);
            puzzles = items.AsReadOnly();
        }

        /// <summary>Primitive validation entry for content tooling without a new test-assembly dependency.</summary>
        public static int ValidateAndCount(byte[] utf8) => new Public240ReviewData(utf8).Puzzles.Count;
        /// <summary>Verifies the explicit serialized public asset against its separately audited byte binding.</summary>
        public static void VerifyAssetBytes(byte[] utf8)
        {
            if (utf8 == null) throw new ArgumentException("PUBLIC240_ASSET_BYTES");
            // Git may expand CRLF in a fresh Windows checkout. Only this declared byte normalization is permitted.
            string text;
            try { text = new System.Text.UTF8Encoding(false, true).GetString(utf8); }
            catch (System.Text.DecoderFallbackException) { throw new ArgumentException("PUBLIC240_ASSET_BYTES"); }
            byte[] lf = new System.Text.UTF8Encoding(false).GetBytes(text.Replace("\r\n", "\n"));
            if (LevelHashing.Sha256Hex(lf) != ExpectedAssetSha256) throw new ArgumentException("PUBLIC240_ASSET_BYTES");
        }
        /// <summary>Exact public review catalog, never product-approved.</summary>
        public CatalogIdentity Identity { get; }
        /// <summary>All 240 immutable public inputs, in stable subject order.</summary>
        public IReadOnlyList<PublicPuzzleInput> Puzzles => puzzles;
        /// <summary>Matches the whole identity triplet; same ID/different hash is absent.</summary>
        public bool TryGet(PublicPuzzleIdentity identity, out PublicPuzzleInput? puzzle)
        {
            puzzle = identity == null ? null : puzzles.FirstOrDefault(item => item.Identity.Equals(identity));
            return puzzle != null;
        }

        private static JsonValue Shape(JsonValue value, params string[] names)
        {
            if (value == null || value.Kind != JsonKind.Object || value.Members.Count != names.Length ||
                value.Members.Any(member => !names.Contains(member.Name, StringComparer.Ordinal))) throw new ArgumentException("PUBLIC240_PROPERTIES");
            return value;
        }
        private static string Text(JsonValue? value) => value != null && value.Kind == JsonKind.String ? value.StringValue : throw new ArgumentException("PUBLIC240_STRING");
        private static int Integer(JsonValue? value) => value != null && value.Kind == JsonKind.Integer && value.IntegerValue >= 0 && value.IntegerValue <= int.MaxValue ? (int)value.IntegerValue : throw new ArgumentException("PUBLIC240_INTEGER");
        private static int[] Counts(JsonValue? value)
        {
            if (value == null || value.Kind != JsonKind.Array || value.Items.Count > 32) throw new ArgumentException("PUBLIC240_COUNTS");
            return value.Items.Select(Integer).ToArray();
        }
        private static PublicPuzzleSide Side(string side) => side switch
        {
            "N" => PublicPuzzleSide.N, "E" => PublicPuzzleSide.E, "S" => PublicPuzzleSide.S, "W" => PublicPuzzleSide.W,
            _ => throw new ArgumentException("PUBLIC240_SIDE")
        };
    }
}
