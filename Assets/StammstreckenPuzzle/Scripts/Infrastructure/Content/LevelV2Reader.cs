using System;
using System.Collections.Generic;
using System.Globalization;

namespace STP.Infrastructure.Content
{
    /// <summary>Result of the schema stage: a typed document or one or more stable diagnostics (never both).</summary>
    public sealed class LevelV2ReadResult
    {
        public LevelV2Document? Document { get; }
        public IReadOnlyList<LevelDiagnostic> Diagnostics { get; }
        public bool Succeeded => Document != null && Diagnostics.Count == 0;

        internal LevelV2ReadResult(LevelV2Document? document, IReadOnlyList<LevelDiagnostic> diagnostics) { Document = document; Diagnostics = LevelDiagnosticList.Freeze(diagnostics); }
    }

    /// <summary>
    /// Schema stage of the Level-v2 pipeline. Implements <c>ARCHITECTURE/schemas/level-v2.schema.json</c> as a closed,
    /// hand-written reader: unknown, missing, mistyped or out-of-range values are rejected; there are no defaults,
    /// no coercions and no automatic type construction.
    /// </summary>
    public static class LevelV2Reader
    {
        private static readonly string[] Root = { "documentSchemaVersion", "rulesetVersion", "puzzleId", "contentRevision", "grid", "endpoints", "rowCounts", "columnCounts", "content", "production", "completion", "solution", "proofRef" };
        private const int MaxSize = LevelV2Contract.MaxSchemaGridSize;

        public static LevelV2ReadResult Read(JsonValue? json)
        {
            var ctx = new SchemaContext();
            if (json == null || json.Kind != JsonKind.Object)
            {
                ctx.Add(LevelDiagnosticCodes.SchemaType, string.Empty, "Expected an object.");
                return new LevelV2ReadResult(null, ctx.Diagnostics);
            }
            var version = json.Get("documentSchemaVersion");
            if (version != null && version.Kind == JsonKind.Integer && version.IntegerValue != LevelV2Contract.DocumentSchemaVersion)
            {
                ctx.Add(LevelDiagnosticCodes.VersionUnknown, "/documentSchemaVersion", "Unsupported documentSchemaVersion " + version.IntegerValue.ToString(CultureInfo.InvariantCulture) + "; the Level-v2 reader accepts only 2.");
                return new LevelV2ReadResult(null, ctx.Diagnostics);
            }

            ctx.BeginObject(json, string.Empty, Root);
            long? documentVersion = ctx.ConstInt(json, string.Empty, "documentSchemaVersion", LevelV2Contract.DocumentSchemaVersion, LevelDiagnosticCodes.VersionUnknown);
            string? ruleset = ctx.Const(json, string.Empty, "rulesetVersion", LevelV2Contract.RulesetVersion, LevelDiagnosticCodes.VersionRulesetUnknown);
            string? puzzleId = ctx.Pattern(json, string.Empty, "puzzleId", LevelPatterns.IsPuzzleId);
            long? contentRevision = ctx.Int(json, string.Empty, "contentRevision", 1, JsonValue.MaxSafeInteger);
            var grid = ReadGrid(ctx, json);
            var endpoints = json.Get("endpoints");
            LevelEndpointDto? endpointA = null, endpointB = null;
            if (endpoints != null && ctx.BeginObject(endpoints, "/endpoints", "a", "b"))
            {
                endpointA = ReadEndpoint(ctx, endpoints, "/endpoints", "a");
                endpointB = ReadEndpoint(ctx, endpoints, "/endpoints", "b");
            }
            int[]? rows = ctx.IntArray(json, string.Empty, "rowCounts", 2, MaxSize, 0, MaxSize);
            int[]? columns = ctx.IntArray(json, string.Empty, "columnCounts", 2, MaxSize, 0, MaxSize);
            var content = ReadContent(ctx, json);
            var production = ReadProduction(ctx, json);
            var completion = ReadCompletion(ctx, json);
            var path = ReadSolution(ctx, json);
            var proofRef = ReadProofRef(ctx, json);

            if (ctx.HasErrors) return new LevelV2ReadResult(null, ctx.Diagnostics);
            var document = new LevelV2Document((int)documentVersion!.Value, ruleset!, puzzleId!, contentRevision!.Value, grid!, endpointA!, endpointB!,
                Array.AsReadOnly(rows!), Array.AsReadOnly(columns!), content!, production!, completion!, path!, proofRef!);
            return new LevelV2ReadResult(document, ctx.Diagnostics);
        }

        private static LevelGridDto? ReadGrid(SchemaContext ctx, JsonValue root)
        {
            var node = root.Get("grid");
            if (node == null || !ctx.BeginObject(node, "/grid", "width", "height")) return null;
            long? width = ctx.Int(node, "/grid", "width", 2, MaxSize);
            long? height = ctx.Int(node, "/grid", "height", 2, MaxSize);
            return width == null || height == null ? null : new LevelGridDto((int)width.Value, (int)height.Value);
        }

        private static LevelEndpointDto? ReadEndpoint(SchemaContext ctx, JsonValue parent, string parentPath, string name)
        {
            var node = parent.Get(name);
            if (node == null) return null;
            string path = SchemaContext.Join(parentPath, name);
            if (!ctx.BeginObject(node, path, "side", "index")) return null;
            LevelSide? side = ctx.EnumMember<LevelSide>(node, path, "side");
            long? index = ctx.Int(node, path, "index", 0, MaxSize - 1);
            return side == null || index == null ? null : new LevelEndpointDto(side.Value, (int)index.Value);
        }

        private static LevelContentDto? ReadContent(SchemaContext ctx, JsonValue root)
        {
            var node = root.Get("content");
            if (node == null || !ctx.BeginObject(node, "/content", "season", "networkSection", "route", "position", "titleKey", "statusKey")) return null;
            long? season = ctx.Int(node, "/content", "season", 1, JsonValue.MaxSafeInteger);
            long? section = ctx.Int(node, "/content", "networkSection", 1, JsonValue.MaxSafeInteger);
            long? route = ctx.Int(node, "/content", "route", 1, JsonValue.MaxSafeInteger);
            long? position = ctx.Int(node, "/content", "position", 1, JsonValue.MaxSafeInteger);
            string? titleKey = ctx.Pattern(node, "/content", "titleKey", LevelPatterns.IsLocalizationKeyV2);
            string? statusKey = ctx.Pattern(node, "/content", "statusKey", LevelPatterns.IsLocalizationKeyV2);
            return season == null || section == null || route == null || position == null || titleKey == null || statusKey == null
                ? null : new LevelContentDto(season.Value, section.Value, route.Value, position.Value, titleKey, statusKey);
        }

        private static LevelProductionDto? ReadProduction(SchemaContext ctx, JsonValue root)
        {
            var node = root.Get("production");
            if (node == null || !ctx.BeginObject(node, "/production", "entryDeductionKey", "focus", "chainDepth", "qualityNote", "timeClass", "starThresholdsSeconds")) return null;
            string? entryKey = ctx.Pattern(node, "/production", "entryDeductionKey", LevelPatterns.IsLocalizationKeyV2);
            var focusNode = ctx.ArrayMember(node, "/production", "focus", 1, int.MaxValue);
            List<LevelFocus>? focus = null;
            if (focusNode != null)
            {
                focus = new List<LevelFocus>();
                bool ok = true;
                for (int i = 0; i < focusNode.Items.Count; i++)
                {
                    LevelFocus? item = ctx.EnumValue<LevelFocus>(focusNode.Items[i], "/production/focus/" + i.ToString(CultureInfo.InvariantCulture));
                    if (item == null) { ok = false; continue; }
                    if (focus.Contains(item.Value))
                    {
                        ctx.Add(LevelDiagnosticCodes.SchemaUnique, "/production/focus/" + i.ToString(CultureInfo.InvariantCulture), "Focus values must be unique.");
                        ok = false;
                        continue;
                    }
                    focus.Add(item.Value);
                }
                if (!ok) focus = null;
            }
            long? chainDepth = ctx.Int(node, "/production", "chainDepth", 1, JsonValue.MaxSafeInteger);
            string? qualityNote = ctx.Str(node, "/production", "qualityNote", 1);
            string? timeClass = ctx.Str(node, "/production", "timeClass", 1);
            var thresholdsNode = node.Get("starThresholdsSeconds");
            bool thresholdsOk = false;
            LevelStarThresholdsDto? thresholds = null;
            if (thresholdsNode != null)
            {
                if (thresholdsNode.Kind == JsonKind.Null) thresholdsOk = true;
                else if (thresholdsNode.Kind != JsonKind.Object) ctx.Add(LevelDiagnosticCodes.SchemaType, "/production/starThresholdsSeconds", "Expected null or an object.");
                else if (ctx.BeginObject(thresholdsNode, "/production/starThresholdsSeconds", "twoStars", "threeStars", "calibrationVersion"))
                {
                    long? two = ctx.Int(thresholdsNode, "/production/starThresholdsSeconds", "twoStars", 1, JsonValue.MaxSafeInteger);
                    long? three = ctx.Int(thresholdsNode, "/production/starThresholdsSeconds", "threeStars", 1, JsonValue.MaxSafeInteger);
                    string? calibration = ctx.Str(thresholdsNode, "/production/starThresholdsSeconds", "calibrationVersion", 1);
                    if (two != null && three != null && calibration != null) { thresholds = new LevelStarThresholdsDto(two.Value, three.Value, calibration); thresholdsOk = true; }
                }
            }
            return entryKey == null || focus == null || chainDepth == null || qualityNote == null || timeClass == null || !thresholdsOk
                ? null : new LevelProductionDto(entryKey, focus.AsReadOnly(), chainDepth.Value, qualityNote, timeClass, thresholds);
        }

        private static LevelCompletionDto? ReadCompletion(SchemaContext ctx, JsonValue root)
        {
            var node = root.Get("completion");
            if (node == null || !ctx.BeginObject(node, "/completion", "mapSegmentId", "trainMomentId", "resultTextKey")) return null;
            string? segment = ctx.Str(node, "/completion", "mapSegmentId", 1);
            string? moment = ctx.Str(node, "/completion", "trainMomentId", 1);
            string? resultKey = ctx.Pattern(node, "/completion", "resultTextKey", LevelPatterns.IsLocalizationKeyV2);
            return segment == null || moment == null || resultKey == null ? null : new LevelCompletionDto(segment, moment, resultKey);
        }

        private static IReadOnlyList<LevelPathCellDto>? ReadSolution(SchemaContext ctx, JsonValue root)
        {
            var node = root.Get("solution");
            if (node == null || !ctx.BeginObject(node, "/solution", "path")) return null;
            var pathNode = ctx.ArrayMember(node, "/solution", "path", 1, int.MaxValue);
            if (pathNode == null) return null;
            var cells = new List<LevelPathCellDto>(pathNode.Items.Count);
            bool ok = true;
            for (int i = 0; i < pathNode.Items.Count; i++)
            {
                string cellPath = "/solution/path/" + i.ToString(CultureInfo.InvariantCulture);
                var cell = pathNode.Items[i];
                if (!ctx.BeginObject(cell, cellPath, "x", "y", "track")) { ok = false; continue; }
                long? x = ctx.Int(cell, cellPath, "x", 0, MaxSize - 1);
                long? y = ctx.Int(cell, cellPath, "y", 0, MaxSize - 1);
                LevelTrack? track = ctx.EnumMember<LevelTrack>(cell, cellPath, "track");
                if (x == null || y == null || track == null) { ok = false; continue; }
                cells.Add(new LevelPathCellDto((int)x.Value, (int)y.Value, track.Value));
            }
            return ok ? cells.AsReadOnly() : null;
        }

        private static LevelProofRefDto? ReadProofRef(SchemaContext ctx, JsonValue root)
        {
            var node = root.Get("proofRef");
            if (node == null || !ctx.BeginObject(node, "/proofRef", "artifactId", "proofFormatVersion", "proofHash")) return null;
            string? artifactId = ctx.Pattern(node, "/proofRef", "artifactId", LevelPatterns.IsArtifactId);
            long? format = ctx.ConstInt(node, "/proofRef", "proofFormatVersion", LevelV2Contract.ProofFormatVersion, LevelDiagnosticCodes.VersionProofFormatUnknown);
            ProfiledHash? hash = ReadProfiledHash(ctx, node, "/proofRef", "proofHash");
            return artifactId == null || format == null || hash == null ? null : new LevelProofRefDto(artifactId, (int)format.Value, hash);
        }

        internal static ProfiledHash? ReadProfiledHash(SchemaContext ctx, JsonValue parent, string parentPath, string name)
        {
            var node = parent.Get(name);
            if (node == null) return null;
            string path = SchemaContext.Join(parentPath, name);
            if (!ctx.BeginObject(node, path, "profile", "sha256")) return null;
            string? profile = ctx.Str(node, path, "profile", 1);
            string? sha = ctx.Pattern(node, path, "sha256", LevelPatterns.IsSha256Hex);
            return profile == null || sha == null ? null : new ProfiledHash(profile, sha);
        }
    }
}
