using System;
using System.Collections.Generic;
using System.Linq;
using STP.Application;

namespace STP.Infrastructure.Content
{
    /// <summary>Closed ADR044 public runtime reader; source approval and release-lock import remain independent gates.</summary>
    public sealed class RuntimePublicPuzzleCatalog : IPublicPuzzleCatalog
    {
        /// <summary>Copied immutable puzzle inputs.</summary>
        private readonly IReadOnlyList<PublicPuzzleInput> puzzles;
        /// <summary>Verifies the existing public projection/hash and the injected full payload lock before publishing all 240 inputs.</summary>
        public RuntimePublicPuzzleCatalog(byte[] source, CatalogIdentity expected)
        {
            if(expected==null)throw new ArgumentNullException(nameof(expected));Require(expected.Use==CatalogUse.Production||expected.Use==CatalogUse.ContractTest,"RUNTIME_PUBLIC_CONSUMER_FORBIDDEN");
            var parsed = StrictJsonParser.Parse(source, new JsonParserLimits(1048576, 12, 1024, 1024, 16));
            if (!parsed.Succeeded) throw new ArgumentException("RUNTIME_PUBLIC_JSON:" + parsed.Error!.Code);
            var root = Shape(parsed.Value!, "schemaVersion", "catalogId", "revision", "use", "status", "puzzles", "catalogHashSha256");
            string use=expected.Use==CatalogUse.Production?"Production":"ContractTest",status=expected.Use==CatalogUse.Production?"PRODUCT_APPROVED":"FIXTURE_ONLY";
            Require(Integer(Need(root, "schemaVersion")) == 1 && Text(Need(root, "catalogId")) == expected.CatalogId && Integer(Need(root, "revision")) == expected.Revision && Text(Need(root, "use")) == use && Text(Need(root, "status")) == status, "RUNTIME_PUBLIC_HEADER");
            var entries = Need(root, "puzzles"); Require(entries.Kind == JsonKind.Array && entries.Items.Count == 240, "RUNTIME_PUBLIC_PUBLIC_COUNT");
            string hash = Text(Need(root, "catalogHashSha256"));
            Require(hash == expected.HashSha256 && LevelHashing.HashProjection(JsonValue.CreateObject(root.Members.Where(m => m.Name != "catalogHashSha256"))) == hash, "RUNTIME_PUBLIC_LOCK_MISMATCH");
            var result = new List<PublicPuzzleInput>();
            for (int i = 0; i < 240; i++)
            {
                var entry = Shape(entries.Items[i], "puzzleId", "rulesetVersion", "grid", "endpoints", "rowCounts", "columnCounts", "publicPuzzleHash");
                string id = "S1-" + (i / 48 + 1).ToString("D2") + "-" + (i % 48 / 12 + 1).ToString("D2") + "-" + (i % 12 + 1).ToString("D2");
                Require(Text(Need(entry, "puzzleId")) == id, "RUNTIME_PUBLIC_PUBLIC_ORDER");
                string rules = Text(Need(entry, "rulesetVersion")); var grid = Shape(Need(entry, "grid"), "width", "height");
                int width = Integer(Need(grid, "width")), height = Integer(Need(grid, "height")); Require(width >= 4 && width <= 10 && height >= 4 && height <= 10, "RUNTIME_PUBLIC_GRID_RANGE");
                var endpoints = Shape(Need(entry, "endpoints"), "a", "b"); var a = Shape(Need(endpoints, "a"), "side", "index"); var b = Shape(Need(endpoints, "b"), "side", "index");
                var aSide = Side(Text(Need(a, "side"))); var bSide = Side(Text(Need(b, "side"))); int aIndex = Integer(Need(a, "index")), bIndex = Integer(Need(b, "index"));
                int[] rows = Counts(Need(entry, "rowCounts")), columns = Counts(Need(entry, "columnCounts")); var publicHash = Shape(Need(entry, "publicPuzzleHash"), "profile", "sha256");
                var identity = new PublicPuzzleIdentity(id, Text(Need(publicHash, "profile")), Text(Need(publicHash, "sha256")));
                var projection = LevelHashing.PublicPuzzleProjection(id, rules, new LevelGridDto(width, height), new LevelEndpointDto((LevelSide)aSide, aIndex), new LevelEndpointDto((LevelSide)bSide, bIndex), rows, columns);
                Require(LevelHashing.HashProjection(projection) == identity.HashSha256, "RUNTIME_PUBLIC_PUBLIC_PUZZLE_HASH");
                result.Add(new PublicPuzzleInput(identity, rules, width, height, aSide, aIndex, bSide, bIndex, rows, columns));
            }
            Identity = expected; puzzles = result.AsReadOnly();
        }
        /// <summary>Explicit locked runtime catalog provenance.</summary>
        public CatalogIdentity Identity { get; }
        /// <summary>Complete copied season inputs.</summary>
        public IReadOnlyList<PublicPuzzleInput> Puzzles => puzzles;
        /// <summary>Exact immutable identity lookup, with no same-ID/hash fallback.</summary>
        public bool TryGet(PublicPuzzleIdentity identity, out PublicPuzzleInput? puzzle) { puzzle = puzzles.SingleOrDefault(p => p.Identity.Equals(identity)); return puzzle != null; }
        private static void Require(bool value, string code) { if (!value) throw new ArgumentException(code); }
        private static JsonValue Need(JsonValue value, string name) => value.Get(name) ?? throw new ArgumentException("RUNTIME_PUBLIC_PUBLIC_FIELDS");
        private static JsonValue Shape(JsonValue value, params string[] names) { Require(value.Kind == JsonKind.Object && value.Members.Count == names.Length && value.Members.All(m => names.Contains(m.Name, StringComparer.Ordinal)), "RUNTIME_PUBLIC_PUBLIC_FIELDS"); return value; }
        private static string Text(JsonValue value) => value.Kind == JsonKind.String ? value.StringValue : throw new ArgumentException("RUNTIME_PUBLIC_PUBLIC_STRING");
        private static int Integer(JsonValue value) => value.Kind == JsonKind.Integer && value.IntegerValue >= 0 && value.IntegerValue <= int.MaxValue ? (int)value.IntegerValue : throw new ArgumentException("RUNTIME_PUBLIC_PUBLIC_INTEGER");
        private static int[] Counts(JsonValue value) { Require(value.Kind == JsonKind.Array && value.Items.Count <= 32, "RUNTIME_PUBLIC_PUBLIC_COUNTS"); return value.Items.Select(Integer).ToArray(); }
        private static PublicPuzzleSide Side(string side) => side == "N" ? PublicPuzzleSide.N : side == "E" ? PublicPuzzleSide.E : side == "S" ? PublicPuzzleSide.S : side == "W" ? PublicPuzzleSide.W : throw new ArgumentException("RUNTIME_PUBLIC_PUBLIC_SIDE");
    }
}
