using System;
using System.Collections.Generic;
using System.Globalization;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Legacy reader for <c>ARCHITECTURE/schemas/level-v1.schema.json</c> (closed, unchanged contract). It exists only as
    /// the validated source of the Level-v1 to Level-v2 migration and for the legacy hash profile.
    /// </summary>
    public static class LevelV1Reader
    {
        private const int MaxSize = 32;
        private const int LegacyFocusValues = 6;
        private static readonly string[] Root = { "schemaVersion", "rulesetVersion", "id", "contentRevision", "grid", "endpoints", "rowCounts", "columnCounts", "content", "production", "completion", "solution", "validation" };
        private static readonly string[] TimeClasses = { "UNSET", "INTRO", "SHORT", "MEDIUM", "LONG", "EXPERT" };

        public static LevelV1ReadResult Read(JsonValue? json)
        {
            var ctx = new SchemaContext();
            if (json == null || json.Kind != JsonKind.Object)
            {
                ctx.Add(LevelDiagnosticCodes.SchemaType, string.Empty, "Expected an object.");
                return new LevelV1ReadResult(null, ctx.Diagnostics);
            }
            var version = json.Get("schemaVersion");
            if (version != null && version.Kind == JsonKind.Integer && version.IntegerValue != LevelV2Contract.LegacySchemaVersion)
            {
                ctx.Add(LevelDiagnosticCodes.VersionUnknown, "/schemaVersion", "Unsupported schemaVersion " + version.IntegerValue.ToString(CultureInfo.InvariantCulture) + "; the Level-v1 reader accepts only 1.");
                return new LevelV1ReadResult(null, ctx.Diagnostics);
            }

            ctx.BeginObject(json, string.Empty, Root);
            long? schemaVersion = ctx.ConstInt(json, string.Empty, "schemaVersion", LevelV2Contract.LegacySchemaVersion, LevelDiagnosticCodes.VersionUnknown);
            string? ruleset = ctx.Const(json, string.Empty, "rulesetVersion", LevelV2Contract.RulesetVersion, LevelDiagnosticCodes.VersionRulesetUnknown);
            string? id = ctx.Pattern(json, string.Empty, "id", LevelPatterns.IsPuzzleId);
            long? revision = ctx.Int(json, string.Empty, "contentRevision", 1, JsonValue.MaxSafeInteger);
            LevelGridDto? grid = null;
            var gridNode = json.Get("grid");
            if (gridNode != null && ctx.BeginObject(gridNode, "/grid", "width", "height"))
            {
                long? width = ctx.Int(gridNode, "/grid", "width", 2, MaxSize);
                long? height = ctx.Int(gridNode, "/grid", "height", 2, MaxSize);
                if (width != null && height != null) grid = new LevelGridDto((int)width.Value, (int)height.Value);
            }
            LevelEndpointDto? endpointA = null, endpointB = null;
            var endpoints = json.Get("endpoints");
            if (endpoints != null && ctx.BeginObject(endpoints, "/endpoints", "a", "b"))
            {
                endpointA = ReadEndpoint(ctx, endpoints, "a");
                endpointB = ReadEndpoint(ctx, endpoints, "b");
            }
            int[]? rows = ctx.IntArray(json, string.Empty, "rowCounts", 2, MaxSize, 0, MaxSize);
            int[]? columns = ctx.IntArray(json, string.Empty, "columnCounts", 2, MaxSize, 0, MaxSize);
            var content = ReadContent(ctx, json);
            var production = ReadProduction(ctx, json);
            var completion = ReadCompletion(ctx, json);
            var path = ReadSolution(ctx, json);
            var validation = ReadValidation(ctx, json);

            if (ctx.HasErrors) return new LevelV1ReadResult(null, ctx.Diagnostics);
            var document = new LevelV1Document((int)schemaVersion!.Value, ruleset!, id!, revision!.Value, grid!, endpointA!, endpointB!, Array.AsReadOnly(rows!), Array.AsReadOnly(columns!),
                content!, production!, completion!, path!, validation!);
            return new LevelV1ReadResult(document, ctx.Diagnostics);
        }

        private static LevelEndpointDto? ReadEndpoint(SchemaContext ctx, JsonValue parent, string name)
        {
            var node = parent.Get(name);
            if (node == null) return null;
            string path = "/endpoints/" + name;
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
            string? titleKey = ctx.Pattern(node, "/content", "titleKey", LevelPatterns.IsLocalizationKeyV1, 160);
            string? statusKey = ctx.Pattern(node, "/content", "statusKey", LevelPatterns.IsLocalizationKeyV1, 160);
            return season == null || section == null || route == null || position == null || titleKey == null || statusKey == null
                ? null : new LevelContentDto(season.Value, section.Value, route.Value, position.Value, titleKey, statusKey);
        }

        private static LevelProductionDto? ReadProduction(SchemaContext ctx, JsonValue root)
        {
            var node = root.Get("production");
            if (node == null || !ctx.BeginObject(node, "/production", "entryDeductionKey", "focus", "chainDepth", "qualityNote", "timeClass", "starThresholdsSeconds")) return null;
            string? entryKey = ctx.Pattern(node, "/production", "entryDeductionKey", LevelPatterns.IsLocalizationKeyV1, 160);
            var focusNode = ctx.ArrayMember(node, "/production", "focus", 1, int.MaxValue);
            List<LevelFocus>? focus = null;
            if (focusNode != null)
            {
                focus = new List<LevelFocus>();
                bool ok = true;
                for (int i = 0; i < focusNode.Items.Count; i++)
                {
                    string itemPath = "/production/focus/" + i.ToString(CultureInfo.InvariantCulture);
                    LevelFocus? item = ctx.EnumValue<LevelFocus>(focusNode.Items[i], itemPath, LegacyFocusValues);
                    if (item == null) { ok = false; continue; }
                    if (focus.Contains(item.Value)) { ctx.Add(LevelDiagnosticCodes.SchemaUnique, itemPath, "Focus values must be unique."); ok = false; continue; }
                    focus.Add(item.Value);
                }
                if (!ok) focus = null;
            }
            long? chainDepth = ctx.Int(node, "/production", "chainDepth", 0, JsonValue.MaxSafeInteger);
            string? qualityNote = ctx.Str(node, "/production", "qualityNote", 1, 1000);
            string? timeClass = ctx.StringEnum(node, "/production", "timeClass", TimeClasses);
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
                    string? calibration = ctx.Str(thresholdsNode, "/production/starThresholdsSeconds", "calibrationVersion", 1, 64);
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
            string? segment = ctx.Pattern(node, "/completion", "mapSegmentId", LevelPatterns.IsStableIdV1, 100);
            string? moment = ctx.Pattern(node, "/completion", "trainMomentId", LevelPatterns.IsStableIdV1, 100);
            string? resultKey = ctx.Pattern(node, "/completion", "resultTextKey", LevelPatterns.IsLocalizationKeyV1, 160);
            return segment == null || moment == null || resultKey == null ? null : new LevelCompletionDto(segment, moment, resultKey);
        }

        private static IReadOnlyList<LevelPathCellDto>? ReadSolution(SchemaContext ctx, JsonValue root)
        {
            var node = root.Get("solution");
            if (node == null || !ctx.BeginObject(node, "/solution", "path")) return null;
            var pathNode = ctx.ArrayMember(node, "/solution", "path", 1, 1024);
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

        private static LevelV1ValidationDto? ReadValidation(SchemaContext ctx, JsonValue root)
        {
            var node = root.Get("validation");
            if (node == null || !ctx.BeginObject(node, "/validation", "puzzleHashSha256", "solutionHashSha256", "solverVersion", "solutionCount", "proof")) return null;
            string? puzzleHash = ctx.Pattern(node, "/validation", "puzzleHashSha256", LevelPatterns.IsSha256Hex);
            string? solutionHash = ctx.Pattern(node, "/validation", "solutionHashSha256", LevelPatterns.IsSha256Hex);
            string? solver = ctx.Pattern(node, "/validation", "solverVersion", LevelPatterns.IsSolverVersion);
            long? count = ctx.Int(node, "/validation", "solutionCount", 1, 1);
            long? searchNodes = null, deduction = null, depth = null, guess = null;
            string? proofHash = null;
            var proof = node.Get("proof");
            if (proof != null && ctx.BeginObject(proof, "/validation/proof", "searchNodes", "deductionSteps", "maxDeductionDepth", "requiredGuessDepth", "proofHashSha256"))
            {
                searchNodes = ctx.Int(proof, "/validation/proof", "searchNodes", 0, JsonValue.MaxSafeInteger);
                deduction = ctx.Int(proof, "/validation/proof", "deductionSteps", 0, JsonValue.MaxSafeInteger);
                depth = ctx.Int(proof, "/validation/proof", "maxDeductionDepth", 0, JsonValue.MaxSafeInteger);
                guess = ctx.Int(proof, "/validation/proof", "requiredGuessDepth", 0, JsonValue.MaxSafeInteger);
                proofHash = ctx.Pattern(proof, "/validation/proof", "proofHashSha256", LevelPatterns.IsSha256Hex);
            }
            return puzzleHash == null || solutionHash == null || solver == null || count == null || searchNodes == null || deduction == null || depth == null || guess == null || proofHash == null
                ? null : new LevelV1ValidationDto(puzzleHash, solutionHash, solver, (int)count.Value, searchNodes.Value, deduction.Value, depth.Value, guess.Value, proofHash);
        }
    }
}
