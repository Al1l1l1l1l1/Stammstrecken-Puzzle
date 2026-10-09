using System;
using System.Collections.Generic;

namespace STP.Infrastructure.Content
{
    public enum LevelSide { N, E, S, W }

    public enum LevelTrack { TRACK_NS, TRACK_EW, TRACK_NE, TRACK_ES, TRACK_SW, TRACK_WN }

    /// <summary>Authoring focus values. The first six are the Level-v1 values and keep their meaning unchanged in v2.</summary>
    public enum LevelFocus { OCCUPANCY, EXCLUSION, ENDPOINT_GEOMETRY, CHAIN, DENSITY, COMBINATION, ROW_COLUMN_COUNTS, CONNECTIVITY, NO_BRANCH, NO_LOOP }

    public sealed class LevelGridDto
    {
        public int Width { get; }
        public int Height { get; }
        internal LevelGridDto(int width, int height) { Width = width; Height = height; }
        internal JsonValue ToJson() => LevelJson.Obj(("width", LevelJson.Int(Width)), ("height", LevelJson.Int(Height)));
    }

    public sealed class LevelEndpointDto
    {
        public LevelSide Side { get; }
        public int Index { get; }
        internal LevelEndpointDto(LevelSide side, int index) { Side = side; Index = index; }
        internal JsonValue ToJson() => LevelJson.Obj(("side", LevelJson.Str(Side.ToString())), ("index", LevelJson.Int(Index)));
    }

    public sealed class LevelContentDto
    {
        public long Season { get; }
        public long NetworkSection { get; }
        public long Route { get; }
        public long Position { get; }
        public string TitleKey { get; }
        public string StatusKey { get; }
        internal LevelContentDto(long season, long networkSection, long route, long position, string titleKey, string statusKey)
        { Season = season; NetworkSection = networkSection; Route = route; Position = position; TitleKey = titleKey; StatusKey = statusKey; }
        internal JsonValue ToJson() => LevelJson.Obj(
            ("season", LevelJson.Int(Season)), ("networkSection", LevelJson.Int(NetworkSection)), ("route", LevelJson.Int(Route)), ("position", LevelJson.Int(Position)),
            ("titleKey", LevelJson.Str(TitleKey)), ("statusKey", LevelJson.Str(StatusKey)));
    }

    public sealed class LevelStarThresholdsDto
    {
        public long TwoStars { get; }
        public long ThreeStars { get; }
        public string CalibrationVersion { get; }
        internal LevelStarThresholdsDto(long twoStars, long threeStars, string calibrationVersion)
        { TwoStars = twoStars; ThreeStars = threeStars; CalibrationVersion = calibrationVersion; }
        internal JsonValue ToJson() => LevelJson.Obj(("twoStars", LevelJson.Int(TwoStars)), ("threeStars", LevelJson.Int(ThreeStars)), ("calibrationVersion", LevelJson.Str(CalibrationVersion)));
    }

    public sealed class LevelProductionDto
    {
        public string EntryDeductionKey { get; }
        public IReadOnlyList<LevelFocus> Focus { get; }
        public long ChainDepth { get; }
        public string QualityNote { get; }
        public string TimeClass { get; }
        /// <summary>Null means "not calibrated yet"; the validator never invents seconds.</summary>
        public LevelStarThresholdsDto? StarThresholdsSeconds { get; }
        internal LevelProductionDto(string entryDeductionKey, IReadOnlyList<LevelFocus> focus, long chainDepth, string qualityNote, string timeClass, LevelStarThresholdsDto? thresholds)
        { EntryDeductionKey = entryDeductionKey; Focus = focus; ChainDepth = chainDepth; QualityNote = qualityNote; TimeClass = timeClass; StarThresholdsSeconds = thresholds; }
        internal JsonValue ToJson()
        {
            var focus = new List<JsonValue>();
            foreach (var item in Focus) focus.Add(LevelJson.Str(item.ToString()));
            return LevelJson.Obj(
                ("entryDeductionKey", LevelJson.Str(EntryDeductionKey)),
                ("focus", JsonValue.CreateArray(focus)),
                ("chainDepth", LevelJson.Int(ChainDepth)),
                ("qualityNote", LevelJson.Str(QualityNote)),
                ("timeClass", LevelJson.Str(TimeClass)),
                ("starThresholdsSeconds", StarThresholdsSeconds == null ? JsonValue.Null : StarThresholdsSeconds.ToJson()));
        }
    }

    public sealed class LevelCompletionDto
    {
        public string MapSegmentId { get; }
        public string TrainMomentId { get; }
        public string ResultTextKey { get; }
        internal LevelCompletionDto(string mapSegmentId, string trainMomentId, string resultTextKey)
        { MapSegmentId = mapSegmentId; TrainMomentId = trainMomentId; ResultTextKey = resultTextKey; }
        internal JsonValue ToJson() => LevelJson.Obj(("mapSegmentId", LevelJson.Str(MapSegmentId)), ("trainMomentId", LevelJson.Str(TrainMomentId)), ("resultTextKey", LevelJson.Str(ResultTextKey)));
    }

    public sealed class LevelPathCellDto
    {
        public int X { get; }
        public int Y { get; }
        public LevelTrack Track { get; }
        internal LevelPathCellDto(int x, int y, LevelTrack track) { X = x; Y = y; Track = track; }
        internal JsonValue ToJson() => LevelJson.Obj(("x", LevelJson.Int(X)), ("y", LevelJson.Int(Y)), ("track", LevelJson.Str(Track.ToString())));
        internal static JsonValue PathToJson(IReadOnlyList<LevelPathCellDto> path)
        {
            var cells = new List<JsonValue>(path.Count);
            foreach (var cell in path) cells.Add(cell.ToJson());
            return JsonValue.CreateArray(cells);
        }
    }

    /// <summary>Proof pass-through: only the closed reference form; the proof artifact is neither loaded nor generated nor bound here.</summary>
    public sealed class LevelProofRefDto
    {
        public string ArtifactId { get; }
        public int ProofFormatVersion { get; }
        public ProfiledHash ProofHash { get; }
        internal LevelProofRefDto(string artifactId, int proofFormatVersion, ProfiledHash proofHash)
        { ArtifactId = artifactId; ProofFormatVersion = proofFormatVersion; ProofHash = proofHash; }
        internal JsonValue ToJson() => LevelJson.Obj(("artifactId", LevelJson.Str(ArtifactId)), ("proofFormatVersion", LevelJson.Int(ProofFormatVersion)), ("proofHash", ProofHash.ToJson()));
    }

    /// <summary>
    /// Typed, immutable, schema-valid Level-v2 document (documentSchemaVersion 2). Instances are produced only by
    /// <see cref="LevelV2Reader"/>; the authoring solution is part of the document but never of the public puzzle.
    /// </summary>
    public sealed class LevelV2Document
    {
        public int DocumentSchemaVersion { get; }
        public string RulesetVersion { get; }
        public string PuzzleId { get; }
        public long ContentRevision { get; }
        public LevelGridDto Grid { get; }
        public LevelEndpointDto EndpointA { get; }
        public LevelEndpointDto EndpointB { get; }
        public IReadOnlyList<int> RowCounts { get; }
        public IReadOnlyList<int> ColumnCounts { get; }
        public LevelContentDto Content { get; }
        public LevelProductionDto Production { get; }
        public LevelCompletionDto Completion { get; }
        public IReadOnlyList<LevelPathCellDto> SolutionPath { get; }
        public LevelProofRefDto ProofRef { get; }

        internal LevelV2Document(int documentSchemaVersion, string rulesetVersion, string puzzleId, long contentRevision, LevelGridDto grid,
            LevelEndpointDto endpointA, LevelEndpointDto endpointB, IReadOnlyList<int> rowCounts, IReadOnlyList<int> columnCounts,
            LevelContentDto content, LevelProductionDto production, LevelCompletionDto completion, IReadOnlyList<LevelPathCellDto> solutionPath, LevelProofRefDto proofRef)
        {
            DocumentSchemaVersion = documentSchemaVersion; RulesetVersion = rulesetVersion; PuzzleId = puzzleId; ContentRevision = contentRevision; Grid = grid;
            EndpointA = endpointA; EndpointB = endpointB; RowCounts = rowCounts; ColumnCounts = columnCounts; Content = content; Production = production;
            Completion = completion; SolutionPath = solutionPath; ProofRef = proofRef;
        }

        /// <summary>The full document as JSON (all schema properties; semantically equal to the parsed source).</summary>
        public JsonValue ToJson() => LevelJson.Obj(
            ("documentSchemaVersion", LevelJson.Int(DocumentSchemaVersion)),
            ("rulesetVersion", LevelJson.Str(RulesetVersion)),
            ("puzzleId", LevelJson.Str(PuzzleId)),
            ("contentRevision", LevelJson.Int(ContentRevision)),
            ("grid", Grid.ToJson()),
            ("endpoints", LevelJson.Obj(("a", EndpointA.ToJson()), ("b", EndpointB.ToJson()))),
            ("rowCounts", LevelJson.IntArray(RowCounts)),
            ("columnCounts", LevelJson.IntArray(ColumnCounts)),
            ("content", Content.ToJson()),
            ("production", Production.ToJson()),
            ("completion", Completion.ToJson()),
            ("solution", LevelJson.Obj(("path", LevelPathCellDto.PathToJson(SolutionPath)))),
            ("proofRef", ProofRef.ToJson()));
    }
}

namespace STP.Infrastructure.Content
{
    /// <summary>Small helpers to build JSON projections without optional or implicit members.</summary>
    internal static class LevelJson
    {
        public static JsonValue Obj(params (string Name, JsonValue Value)[] members)
        {
            var list = new List<JsonMember>(members.Length);
            foreach (var member in members) list.Add(new JsonMember(member.Name, member.Value));
            return JsonValue.CreateObject(list);
        }
        public static JsonValue Str(string value) => JsonValue.CreateString(value);
        public static JsonValue Int(long value) => JsonValue.CreateInteger(value);
        public static JsonValue IntArray(IReadOnlyList<int> values)
        {
            var list = new List<JsonValue>(values.Count);
            foreach (int value in values) list.Add(Int(value));
            return JsonValue.CreateArray(list);
        }
    }
}
