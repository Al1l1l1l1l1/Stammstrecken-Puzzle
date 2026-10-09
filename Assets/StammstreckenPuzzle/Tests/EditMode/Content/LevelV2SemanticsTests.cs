using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using STP.Infrastructure.Content;
using STP.Puzzle.Domain;

namespace STP.Tests.Content.EditMode
{
    public sealed class LevelV2SemanticsTests
    {
        private static JsonValue Doc(string file = Examples.V2) => Examples.Json(file);
        private static LevelV2LoadResult Load(JsonValue json) => LevelV2Loader.Load(json);
        private static JsonValue Parse(string text) => StrictJsonParser.ParseText(text).Value!;
        /// <summary>Independent name-based mapping (LevelTrack and CellContent share their TRACK_* member names).</summary>
        private static CellContent ContentOf(LevelTrack track) => (CellContent)Enum.Parse(typeof(CellContent), track.ToString());
        private static JsonValue Cell(int x, int y, string track) => Parse("{\"x\":" + x + ",\"y\":" + y + ",\"track\":\"" + track + "\"}");

        private static void AssertCode(JsonValue json, string code, string path, LevelDiagnosticStage stage)
        {
            var result = Load(json);
            Assert.That(result.IsValid, Is.False, "expected " + code);
            Assert.That(result.Diagnostics.Any(d => d.Code == code && d.Path == path && d.Stage == stage), Is.True, "expected " + code + " @ " + path + " but got: " + Codes.Describe(result.Diagnostics));
            Assert.That(result.ImportState, Is.EqualTo(LevelImportState.Rejected));
            Assert.That(result.ContentHash, Is.Null);
            Assert.That(result.PublicPuzzleHash, Is.Null);
        }

        // ---- positive ------------------------------------------------------------------------------------------

        [TestCase(Examples.V2)]
        [TestCase(Examples.V2Single)]
        public void RepositoryExamples_PassAllStages(string file)
        {
            var result = Load(Doc(file));
            Assert.That(result.IsValid, Is.True, Codes.Describe(result.Diagnostics));
            Assert.That(result.Definition, Is.Not.Null);
            Assert.That(result.Diagnostics, Is.Empty);
        }

        [Test]
        public void ValidDocuments_MapToTheIntegratedPuzzleDefinition()
        {
            var definition = Load(Doc()).Definition!;
            Assert.That((definition.Grid.Width, definition.Grid.Height), Is.EqualTo((4, 4)));
            Assert.That((definition.A.Side, definition.A.Index, definition.A.Cell.X, definition.A.Cell.Y), Is.EqualTo((Direction.N, 0, 0, 0)));
            Assert.That((definition.B.Side, definition.B.Index, definition.B.Cell.X, definition.B.Cell.Y), Is.EqualTo((Direction.S, 3, 3, 3)));
            Assert.That(definition.RowCounts, Is.EqualTo(new[] { 1, 3, 4, 1 }));
            Assert.That(definition.ColumnCounts, Is.EqualTo(new[] { 3, 1, 2, 3 }));
            Assert.That(definition.RulesetVersion, Is.EqualTo("train-track-v1"));

            var single = Load(Doc(Examples.V2Single)).Definition!;
            Assert.That((single.Grid.Width, single.Grid.Height, single.A.Side, single.B.Side), Is.EqualTo((2, 2, Direction.N, Direction.W)));
            Assert.That(single.A.Cell.X == single.B.Cell.X && single.A.Cell.Y == single.B.Cell.Y, Is.True, "A and B share the adjacent cell (single-cell contract)");
        }

        [Test]
        public void MappedDefinition_EqualsADirectDomainConstruction()
        {
            var mapped = Load(Doc()).Definition!;
            var grid = GridSize.Create(4, 4).Value!;
            var direct = PuzzleDefinition.Create(grid, Endpoint.Create(grid, Direction.N, 0).Value, Endpoint.Create(grid, Direction.S, 3).Value, new[] { 1, 3, 4, 1 }, new[] { 3, 1, 2, 3 }, "train-track-v1").Value!;
            Assert.That(mapped.Grid.CellCount, Is.EqualTo(direct.Grid.CellCount));
            Assert.That(mapped.RowCounts, Is.EqualTo(direct.RowCounts));
            Assert.That(mapped.ColumnCounts, Is.EqualTo(direct.ColumnCounts));
            Assert.That((mapped.A.Side, mapped.A.Index, mapped.B.Side, mapped.B.Index), Is.EqualTo((direct.A.Side, direct.A.Index, direct.B.Side, direct.B.Index)));
        }

        [Test]
        public void AuthoringSolution_IsAcceptedByTheDomainEvaluatorAsCompleted()
        {
            foreach (string file in Examples.V2Files)
            {
                var result = Load(Doc(file));
                var cells = new CellContent[result.Definition!.Grid.CellCount];
                foreach (var cell in result.Document!.SolutionPath) cells[cell.Y * result.Definition.Grid.Width + cell.X] = ContentOf(cell.Track);
                var evaluation = PuzzleEvaluator.Evaluate(result.Definition, cells);
                Assert.That(evaluation.Completed, Is.True, file);
                Assert.That(evaluation.Diagnostics, Is.Empty, file);
            }
        }

        [Test]
        public void Season2_IsNotLimitedToTheSeasonOneBounds()
        {
            var json = Doc();
            json = JsonEdit.Set(json, "/puzzleId", JsonEdit.Str("S2-06-05-13"));
            json = JsonEdit.Set(json, "/content/season", JsonEdit.Int(2));
            json = JsonEdit.Set(json, "/content/networkSection", JsonEdit.Int(6));
            json = JsonEdit.Set(json, "/content/route", JsonEdit.Int(5));
            json = JsonEdit.Set(json, "/content/position", JsonEdit.Int(13));
            Assert.That(Load(json).IsValid, Is.True);
        }

        // ---- identity ------------------------------------------------------------------------------------------

        [Test]
        public void IdentityRules()
        {
            AssertCode(JsonEdit.Set(Doc(), "/puzzleId", JsonEdit.Str("S1-01-01-02")), LevelDiagnosticCodes.IdContentMismatch, "/puzzleId", LevelDiagnosticStage.Semantic);
            AssertCode(JsonEdit.Set(Doc(), "/content/season", JsonEdit.Int(2)), LevelDiagnosticCodes.IdContentMismatch, "/puzzleId", LevelDiagnosticStage.Semantic);
            AssertCode(JsonEdit.Set(Doc(), "/content/networkSection", JsonEdit.Int(2)), LevelDiagnosticCodes.IdContentMismatch, "/puzzleId", LevelDiagnosticStage.Semantic);
            AssertCode(JsonEdit.Set(Doc(), "/content/route", JsonEdit.Int(2)), LevelDiagnosticCodes.IdContentMismatch, "/puzzleId", LevelDiagnosticStage.Semantic);
            AssertCode(JsonEdit.Set(Doc(), "/content/position", JsonEdit.Int(2)), LevelDiagnosticCodes.IdContentMismatch, "/puzzleId", LevelDiagnosticStage.Semantic);
            AssertCode(JsonEdit.Set(Doc(), "/puzzleId", JsonEdit.Str("S99999999999999999999-01-01-01")), LevelDiagnosticCodes.IdContentMismatch, "/puzzleId", LevelDiagnosticStage.Semantic);
        }

        [TestCase("S1-06-01-01", "networkSection", 6, LevelDiagnosticCodes.IdSeasonOneSection, "/content/networkSection")]
        [TestCase("S1-01-05-01", "route", 5, LevelDiagnosticCodes.IdSeasonOneRoute, "/content/route")]
        [TestCase("S1-01-01-13", "position", 13, LevelDiagnosticCodes.IdSeasonOnePosition, "/content/position")]
        [TestCase("S1-00-01-01", "networkSection", 1, LevelDiagnosticCodes.IdContentMismatch, "/puzzleId")]
        public void SeasonOneHierarchy_IsLimitedTo5x4x12(string id, string field, int value, string code, string path)
        {
            var json = JsonEdit.Set(Doc(), "/puzzleId", JsonEdit.Str(id));
            json = JsonEdit.Set(json, "/content/" + field, JsonEdit.Int(value));
            AssertCode(json, code, path, LevelDiagnosticStage.Semantic);
        }

        [Test]
        public void SeasonOneBoundaries_AreInclusive()
        {
            var json = JsonEdit.Set(Doc(), "/puzzleId", JsonEdit.Str("S1-05-04-12"));
            json = JsonEdit.Set(json, "/content/networkSection", JsonEdit.Int(5));
            json = JsonEdit.Set(json, "/content/route", JsonEdit.Int(4));
            json = JsonEdit.Set(json, "/content/position", JsonEdit.Int(12));
            Assert.That(Load(json).IsValid, Is.True);
        }

        // ---- time ----------------------------------------------------------------------------------------------

        [TestCase(60, 60)] [TestCase(60, 90)] [TestCase(1, 2)]
        public void TimeOrder_RequiresThreeStarsStrictlyBelowTwoStars(int two, int three)
        {
            var json = JsonEdit.Set(Doc(), "/production/starThresholdsSeconds", Parse("{\"twoStars\":" + two + ",\"threeStars\":" + three + ",\"calibrationVersion\":\"c\"}"));
            AssertCode(json, LevelDiagnosticCodes.TimeOrder, "/production/starThresholdsSeconds", LevelDiagnosticStage.Semantic);
        }

        [TestCase(90, 60)] [TestCase(2, 1)]
        public void TimeOrder_AcceptsAStrictlyOrderedPair(int two, int three)
        {
            var json = JsonEdit.Set(Doc(), "/production/starThresholdsSeconds", Parse("{\"twoStars\":" + two + ",\"threeStars\":" + three + ",\"calibrationVersion\":\"c\"}"));
            Assert.That(Load(json).IsValid, Is.True);
            Assert.That(Load(Doc()).Document!.Production.StarThresholdsSeconds, Is.Null, "null is valid until calibration exists");
        }

        // ---- proof reference / hash profile --------------------------------------------------------------------

        [TestCase("FOO-JCS-1")] [TestCase("stp-proof-jcs-1")] [TestCase("STP-PROOF-JCS-2")] [TestCase("STP-PUZZLE-SEMANTIC-JCS-1")] [TestCase("STP-SOLUTION-JCS-1")] [TestCase("STP-LEVEL-V1-PUZZLE-JCS-1")]
        public void ProofReference_AcceptsOnlyTheProofProfile(string profile) =>
            AssertCode(JsonEdit.Set(Doc(), "/proofRef/proofHash/profile", JsonEdit.Str(profile)), LevelDiagnosticCodes.HashProfileUnknown, "/proofRef/proofHash/profile", LevelDiagnosticStage.Hash);

        // ---- domain mapping ------------------------------------------------------------------------------------

        [Test]
        public void EndpointIndexBeyondTheSide_IsAMappingError()
        {
            AssertCode(JsonEdit.Set(Doc(), "/endpoints/a/index", JsonEdit.Int(4)), LevelDiagnosticCodes.EndpointRange, "/endpoints/a/index", LevelDiagnosticStage.DomainMap);
            AssertCode(JsonEdit.Set(Doc(), "/endpoints/b/index", JsonEdit.Int(9)), LevelDiagnosticCodes.EndpointRange, "/endpoints/b/index", LevelDiagnosticStage.DomainMap);
            var eastSide = JsonEdit.Set(JsonEdit.Set(Doc(), "/endpoints/a/side", JsonEdit.Str("E")), "/endpoints/a/index", JsonEdit.Int(4));
            AssertCode(eastSide, LevelDiagnosticCodes.EndpointRange, "/endpoints/a/index", LevelDiagnosticStage.DomainMap);
        }

        [Test]
        public void IdenticalEndpoints_AreAMappingError() =>
            AssertCode(JsonEdit.Set(Doc(), "/endpoints/b", Parse("{\"side\":\"N\",\"index\":0}")), LevelDiagnosticCodes.EndpointIdentical, "/endpoints/b", LevelDiagnosticStage.DomainMap);

        [Test]
        public void CountArrayLengthsMustMatchTheGrid()
        {
            AssertCode(JsonEdit.Set(Doc(), "/rowCounts", Parse("[1,3,4]")), LevelDiagnosticCodes.GridRowCountLength, "/rowCounts", LevelDiagnosticStage.DomainMap);
            AssertCode(JsonEdit.Set(Doc(), "/rowCounts", Parse("[1,3,4,1,0]")), LevelDiagnosticCodes.GridRowCountLength, "/rowCounts", LevelDiagnosticStage.DomainMap);
            AssertCode(JsonEdit.Set(Doc(), "/columnCounts", Parse("[3,1,2]")), LevelDiagnosticCodes.GridColumnCountLength, "/columnCounts", LevelDiagnosticStage.DomainMap);
            AssertCode(JsonEdit.Set(Doc(), "/columnCounts", Parse("[3,1,2,3,0]")), LevelDiagnosticCodes.GridColumnCountLength, "/columnCounts", LevelDiagnosticStage.DomainMap);
        }

        [Test]
        public void CountValuesMustFitTheGrid_AndSumsMustAgree()
        {
            AssertCode(JsonEdit.Set(Doc(), "/rowCounts", Parse("[1,3,5,1]")), LevelDiagnosticCodes.GridRowCountValue, "/rowCounts/2", LevelDiagnosticStage.DomainMap);
            AssertCode(JsonEdit.Set(Doc(), "/columnCounts", Parse("[3,1,2,5]")), LevelDiagnosticCodes.GridColumnCountValue, "/columnCounts/3", LevelDiagnosticStage.DomainMap);
            AssertCode(JsonEdit.Set(Doc(), "/rowCounts", Parse("[1,3,4,2]")), LevelDiagnosticCodes.GridCountSum, "/rowCounts", LevelDiagnosticStage.DomainMap);
            AssertCode(JsonEdit.Set(Doc(), "/columnCounts", Parse("[0,0,0,0]")), LevelDiagnosticCodes.GridCountSum, "/rowCounts", LevelDiagnosticStage.DomainMap);
        }

        [Test]
        public void EndpointCellsNeedPositiveRowAndColumnCounts()
        {
            var json = JsonEdit.Set(JsonEdit.Set(Doc(), "/rowCounts", Parse("[0,4,4,1]")), "/columnCounts", Parse("[3,1,2,3]"));
            AssertCode(json, LevelDiagnosticCodes.EndpointCountZero, "/endpoints/a", LevelDiagnosticStage.DomainMap);
        }

        [Test]
        public void MappingErrors_StopThePipeline_SoNoLaterStageMasksOrAddsNoise()
        {
            var result = Load(JsonEdit.Set(JsonEdit.Set(Doc(), "/rowCounts", Parse("[1,3,4]")), "/puzzleId", JsonEdit.Str("S1-01-01-02")));
            Assert.That(result.Diagnostics.All(d => d.Stage == LevelDiagnosticStage.DomainMap), Is.True, Codes.Describe(result.Diagnostics));
            Assert.That(result.Definition, Is.Null);
            Assert.That(result.Document, Is.Not.Null, "the schema stage had passed");
        }

        // ---- solution path -------------------------------------------------------------------------------------

        [Test]
        public void PathCellOutsideTheGrid_IsRejected() =>
            AssertCode(JsonEdit.Set(Doc(), "/solution/path/8/y", JsonEdit.Int(4)), LevelDiagnosticCodes.PathOutOfBounds, "/solution/path/8", LevelDiagnosticStage.Semantic);

        [Test]
        public void RepeatedCoordinate_IsRejected()
        {
            var json = JsonEdit.Set(Doc(), "/solution/path/8", Cell(3, 2, "TRACK_NS"));
            AssertCode(json, LevelDiagnosticCodes.PathDuplicate, "/solution/path/8", LevelDiagnosticStage.Semantic);
        }

        [Test]
        public void NonAdjacentConsecutiveCells_AreRejected() =>
            AssertCode(JsonEdit.Set(Doc(), "/solution/path/4", Cell(2, 3, "TRACK_WN")), LevelDiagnosticCodes.PathNotAdjacent, "/solution/path/4", LevelDiagnosticStage.Semantic);

        [Test]
        public void ShapesThatDoNotConnect_AreRejected() =>
            AssertCode(JsonEdit.Set(Doc(), "/solution/path/3/track", JsonEdit.Str("TRACK_NS")), LevelDiagnosticCodes.PathPortMismatch, "/solution/path/3", LevelDiagnosticStage.Semantic);

        [Test]
        public void ReversedPath_FailsBothEndpointCells()
        {
            var reversed = JsonValue.CreateArray(Doc().Get("solution")!.Get("path")!.Items.Reverse());
            var result = Load(JsonEdit.Set(Doc(), "/solution/path", reversed));
            Assert.That(Codes.Of(result.Diagnostics), Does.Contain(LevelDiagnosticCodes.EndpointACell));
            Assert.That(Codes.Of(result.Diagnostics), Does.Contain(LevelDiagnosticCodes.EndpointBCell));
        }

        [Test]
        public void EndpointShapes_MustOpenTowardsTheEndpoints()
        {
            AssertCode(JsonEdit.Set(Doc(), "/solution/path/0/track", JsonEdit.Str("TRACK_EW")), LevelDiagnosticCodes.EndpointAShape, "/solution/path/0", LevelDiagnosticStage.Semantic);
            AssertCode(JsonEdit.Set(Doc(), "/solution/path/8/track", JsonEdit.Str("TRACK_EW")), LevelDiagnosticCodes.EndpointBShape, "/solution/path/8", LevelDiagnosticStage.Semantic);
        }

        [Test]
        public void RowAndColumnCounts_MustEqualTheSolutionDerivedCounts()
        {
            var rows = Load(JsonEdit.Set(Doc(), "/rowCounts", Parse("[1,3,3,2]")));
            Assert.That(rows.Diagnostics.Where(d => d.Code == LevelDiagnosticCodes.CountRowMismatch).Select(d => d.Path), Is.EqualTo(new[] { "/rowCounts/2", "/rowCounts/3" }));
            var columns = Load(JsonEdit.Set(Doc(), "/columnCounts", Parse("[3,1,3,2]")));
            Assert.That(columns.Diagnostics.Where(d => d.Code == LevelDiagnosticCodes.CountColumnMismatch).Select(d => d.Path), Is.EqualTo(new[] { "/columnCounts/2", "/columnCounts/3" }));
        }

        [Test]
        public void OpenConnections_AreRejected()
        {
            var result = Load(JsonEdit.Set(Doc(), "/solution/path/0/track", JsonEdit.Str("TRACK_NE")));
            Assert.That(result.Diagnostics.Any(d => d.Code == LevelDiagnosticCodes.RuleOpenConnection && d.Path == "/solution/path/0"), Is.True, Codes.Describe(result.Diagnostics));
        }

        [Test]
        public void ConnectionsIntoANonConsecutivePathCell_AreLoops()
        {
            var result = Load(JsonEdit.Set(Doc(), "/solution/path/4/track", JsonEdit.Str("TRACK_EW")));
            Assert.That(result.Diagnostics.Any(d => d.Code == LevelDiagnosticCodes.RuleLoop && d.Path == "/solution/path/4"), Is.True, Codes.Describe(result.Diagnostics));
        }

        [Test]
        public void ADifferentButConsistentSolution_IsStillAValidDocument()
        {
            // Same public puzzle (rows/columns/endpoints) cannot be satisfied by another A-B path here, so changing the path must fail on counts or shape.
            var json = JsonEdit.Set(Doc(), "/solution/path/5", Cell(2, 1, "TRACK_NS"));
            Assert.That(Load(json).IsValid, Is.False);
        }

        [Test]
        public void SingleCellSolution_RequiresBothExteriorPorts()
        {
            Assert.That(Load(Doc(Examples.V2Single)).IsValid, Is.True);
            var wrongShape = JsonEdit.Set(Doc(Examples.V2Single), "/solution/path/0/track", JsonEdit.Str("TRACK_NS"));
            var result = Load(wrongShape);
            Assert.That(Codes.Of(result.Diagnostics), Does.Contain(LevelDiagnosticCodes.EndpointBShape));
            var twoCells = JsonEdit.Set(Doc(Examples.V2Single), "/solution/path", Parse("[{\"x\":0,\"y\":0,\"track\":\"TRACK_WN\"},{\"x\":0,\"y\":0,\"track\":\"TRACK_WN\"}]"));
            Assert.That(Codes.Of(Load(twoCells).Diagnostics), Does.Contain(LevelDiagnosticCodes.PathDuplicate));
        }

        [Test]
        public void DiagnosticsAreDeterministic_AndStable()
        {
            var json = JsonEdit.Set(JsonEdit.Set(Doc(), "/solution/path/4/track", JsonEdit.Str("TRACK_EW")), "/rowCounts", Parse("[1,3,3,2]"));
            var first = Load(json).Diagnostics.Select(d => d.ToString()).ToArray();
            var second = Load(json).Diagnostics.Select(d => d.ToString()).ToArray();
            Assert.That(second, Is.EqualTo(first));
            Assert.That(first.Length, Is.GreaterThan(1));
        }

        // ---- agreement with the Domain authority ---------------------------------------------------------------

        private static readonly string[] Tracks = { "TRACK_NS", "TRACK_EW", "TRACK_NE", "TRACK_ES", "TRACK_SW", "TRACK_WN" };
        private static readonly int[] TrackMasks = { 5, 10, 3, 6, 12, 9 };

        /// <summary>Generates a random valid puzzle document (self-avoiding A-B path, counts derived from it).</summary>
        internal static JsonValue RandomValidDocument(Random random, int index)
        {
            while (true)
            {
                int width = random.Next(2, 7), height = random.Next(2, 7);
                int aSide = random.Next(4);
                int aIndex = random.Next(aSide % 2 == 0 ? width : height);
                int x = aSide == 3 ? 0 : aSide == 1 ? width - 1 : aIndex;
                int y = aSide == 0 ? 0 : aSide == 2 ? height - 1 : aIndex;
                var cells = new List<(int X, int Y)> { (x, y) };
                var visited = new HashSet<(int, int)> { (x, y) };
                int[] dx = { 0, 1, 0, -1 }, dy = { -1, 0, 1, 0 };
                int steps = random.Next(0, width * height);
                bool done = false;
                for (int step = 0; step < steps * 3 && !done; step++)
                {
                    var (cx, cy) = cells[cells.Count - 1];
                    var options = new List<int>();
                    for (int d = 0; d < 4; d++)
                    {
                        int nx = cx + dx[d], ny = cy + dy[d];
                        if (nx >= 0 && ny >= 0 && nx < width && ny < height && !visited.Contains((nx, ny))) options.Add(d);
                    }
                    if (options.Count == 0) break;
                    int pick = options[random.Next(options.Count)];
                    cells.Add((cx + dx[pick], cy + dy[pick]));
                    visited.Add(cells[cells.Count - 1]);
                    if (cells.Count >= steps) done = true;
                }
                var last = cells[cells.Count - 1];
                var exits = new List<(int Side, int Index)>();
                if (last.Y == 0) exits.Add((0, last.X));
                if (last.X == width - 1) exits.Add((1, last.Y));
                if (last.Y == height - 1) exits.Add((2, last.X));
                if (last.X == 0) exits.Add((3, last.Y));
                exits.RemoveAll(e => e.Side == aSide && e.Index == aIndex && cells.Count == 1);
                if (exits.Count == 0) continue;
                var exit = exits[random.Next(exits.Count)];
                var rows = new int[height];
                var columns = new int[width];
                var path = new List<string>();
                for (int i = 0; i < cells.Count; i++)
                {
                    int prev = i == 0 ? aSide : DirectionBetween(cells[i], cells[i - 1]);
                    int next = i == cells.Count - 1 ? exit.Side : DirectionBetween(cells[i], cells[i + 1]);
                    if (prev == next) goto retry;
                    int mask = (1 << prev) | (1 << next);
                    int shape = Array.IndexOf(TrackMasks, mask);
                    rows[cells[i].Y]++;
                    columns[cells[i].X]++;
                    path.Add("{\"x\":" + cells[i].X + ",\"y\":" + cells[i].Y + ",\"track\":\"" + Tracks[shape] + "\"}");
                }
                string[] sides = { "N", "E", "S", "W" };
                string id = "S99-01-01-" + (index % 99 + 1).ToString("00");
                string text = "{\"documentSchemaVersion\":2,\"rulesetVersion\":\"train-track-v1\",\"puzzleId\":\"" + id + "\",\"contentRevision\":1,"
                    + "\"grid\":{\"width\":" + width + ",\"height\":" + height + "},"
                    + "\"endpoints\":{\"a\":{\"side\":\"" + sides[aSide] + "\",\"index\":" + aIndex + "},\"b\":{\"side\":\"" + sides[exit.Side] + "\",\"index\":" + exit.Index + "}},"
                    + "\"rowCounts\":[" + string.Join(",", rows) + "],\"columnCounts\":[" + string.Join(",", columns) + "],"
                    + "\"content\":{\"season\":99,\"networkSection\":1,\"route\":1,\"position\":" + (index % 99 + 1) + ",\"titleKey\":\"level.fixture.random.title\",\"statusKey\":\"level.fixture.random.status\"},"
                    + "\"production\":{\"entryDeductionKey\":\"level.fixture.random.entry\",\"focus\":[\"CHAIN\"],\"chainDepth\":1,\"qualityNote\":\"generated test fixture\",\"timeClass\":\"UNSET\",\"starThresholdsSeconds\":null},"
                    + "\"completion\":{\"mapSegmentId\":\"fixture-random\",\"trainMomentId\":\"standard-first-clear\",\"resultTextKey\":\"result.fixture.random\"},"
                    + "\"solution\":{\"path\":[" + string.Join(",", path) + "]},"
                    + "\"proofRef\":{\"artifactId\":\"proofs/" + id + "/solver-v1.proof-v1.json\",\"proofFormatVersion\":1,\"proofHash\":{\"profile\":\"STP-PROOF-JCS-1\",\"sha256\":\"" + new string('0', 64) + "\"}}}";
                return Parse(text);
            retry:;
            }
        }

        /// <summary>Walks the Domain cell set from endpoint A to endpoint B and lists it in document order.</summary>
        private static JsonValue OrderFromAToB(CellContent[] cells, PuzzleDefinition definition)
        {
            int[] dx = { 0, 1, 0, -1 }, dy = { -1, 0, 1, 0 };
            int x = definition.A.Cell.X, y = definition.A.Cell.Y, entry = (int)definition.A.Side;
            var items = new List<JsonValue>();
            for (int guard = 0; guard <= cells.Length; guard++)
            {
                var content = cells[y * definition.Grid.Width + x];
                items.Add(Cell(x, y, content.ToString()));
                int ports = TrackGeometry.Ports(content) & ~(1 << entry);
                int exit = Enumerable.Range(0, 4).First(d => (ports & (1 << d)) != 0);
                if (x == definition.B.Cell.X && y == definition.B.Cell.Y && exit == (int)definition.B.Side) break;
                x += dx[exit];
                y += dy[exit];
                entry = (exit + 2) & 3;
            }
            return JsonValue.CreateArray(items);
        }

        private static int DirectionBetween((int X, int Y) from, (int X, int Y) to) => to.Y < from.Y ? 0 : to.X > from.X ? 1 : to.Y > from.Y ? 2 : 3;

        [Test]
        public void RandomValidPuzzles_AreValid_AndTheDomainEvaluatorAgrees()
        {
            var random = new Random(4711);
            for (int i = 0; i < 400; i++)
            {
                var json = RandomValidDocument(random, i);
                var result = Load(json);
                Assert.That(result.IsValid, Is.True, "round " + i + ": " + Codes.Describe(result.Diagnostics) + "\n" + JcsSerializer.Serialize(json).Text);
                var cells = new CellContent[result.Definition!.Grid.CellCount];
                foreach (var cell in result.Document!.SolutionPath) cells[cell.Y * result.Definition.Grid.Width + cell.X] = ContentOf(cell.Track);
                Assert.That(PuzzleEvaluator.Evaluate(result.Definition, cells).Completed, Is.True, "round " + i);
            }
        }

        [Test]
        public void SemanticVerdict_IsSoundAgainstTheDomainEvaluator_UnderRandomPathMutations()
        {
            var random = new Random(31337);
            int accepted = 0, rejected = 0, ordering = 0;
            for (int i = 0; i < 3000; i++)
            {
                var json = RandomValidDocument(random, i);
                var items = new List<JsonValue>(json.Get("solution")!.Get("path")!.Items);
                int operation = random.Next(7);
                int at = random.Next(items.Count);
                switch (operation)
                {
                    case 0: items[at] = JsonEdit.Set(items[at], "/track", JsonEdit.Str(Tracks[random.Next(6)])); break;
                    case 1: items[at] = JsonEdit.Set(items[at], "/x", JsonEdit.Int(random.Next(0, 7))); break;
                    case 2: items[at] = JsonEdit.Set(items[at], "/y", JsonEdit.Int(random.Next(0, 7))); break;
                    case 3: if (items.Count > 1) { int other = random.Next(items.Count); var tmp = items[at]; items[at] = items[other]; items[other] = tmp; } break;
                    case 4: if (items.Count > 1) items.RemoveAt(at); break;
                    case 5: items.Add(items[random.Next(items.Count)]); break;
                    default: items.Reverse(); break;
                }
                json = JsonEdit.Set(json, "/solution/path", JsonValue.CreateArray(items));
                var result = Load(json);
                if (result.Definition == null) continue;
                int width = result.Definition.Grid.Width, height = result.Definition.Grid.Height;
                var cells = new CellContent[width * height];
                bool buildable = true;
                foreach (var cell in result.Document!.SolutionPath)
                {
                    if (cell.X >= width || cell.Y >= height || cells[cell.Y * width + cell.X] != CellContent.UNSET) { buildable = false; break; }
                    cells[cell.Y * width + cell.X] = ContentOf(cell.Track);
                }
                bool valid = result.IsValid;
                if (valid) accepted++; else rejected++;
                if (!buildable) { Assert.That(valid, Is.False, "round " + i + " cannot be a valid solution"); continue; }
                bool completed = PuzzleEvaluator.Evaluate(result.Definition, cells).Completed;
                string context = "round " + i + " semantic=" + valid + " domain=" + completed + "\n" + Codes.Describe(result.Diagnostics) + "\n" + JcsSerializer.Serialize(json).Text;
                if (valid) Assert.That(completed, Is.True, "soundness: " + context);
                else if (completed)
                {
                    // The Domain evaluates an unordered cell set; the document contract additionally demands the listing order A to B.
                    var ordered = JsonEdit.Set(json, "/solution/path", OrderFromAToB(cells, result.Definition));
                    Assert.That(Load(ordered).IsValid, Is.True, "the only admissible difference is the listing order: " + context);
                    ordering++;
                }
            }
            Assert.That(accepted, Is.GreaterThan(0), "some mutations (identity swaps) must remain valid");
            Assert.That(rejected, Is.GreaterThan(1000));
            Assert.That(ordering, Is.GreaterThan(0), "reversed listings are exercised");
        }
    }
}
