using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using STP.Infrastructure.Content;

namespace STP.Tests.Content.EditMode
{
    public sealed class LevelV2ReaderTests
    {
        private static JsonValue Doc(string file = Examples.V2) => Examples.Json(file);

        private static LevelV2ReadResult Read(JsonValue json) => LevelV2Reader.Read(json);

        private static void AssertRejected(JsonValue json, string code, string path)
        {
            var result = Read(json);
            Assert.That(result.Succeeded, Is.False, "expected rejection of " + code + " at " + path);
            Assert.That(result.Document, Is.Null);
            Assert.That(result.Diagnostics.Any(d => d.Code == code && d.Path == path), Is.True, "expected " + code + " @ " + path + " but got: " + Codes.Describe(result.Diagnostics));
            Assert.That(result.Diagnostics.All(d => d.Stage == LevelDiagnosticStage.Schema), Is.True);
        }

        [TestCase(Examples.V2)]
        [TestCase(Examples.V2Single)]
        public void BothRepositoryExamples_AreSchemaValid_AndRoundTripToIdenticalCanonicalJson(string file)
        {
            var json = Doc(file);
            var result = Read(json);
            Assert.That(result.Succeeded, Is.True, Codes.Describe(result.Diagnostics));
            Assert.That(JcsSerializer.Serialize(result.Document!.ToJson()).Text, Is.EqualTo(JcsSerializer.Serialize(json).Text));
        }

        [Test]
        public void Example_IsReadIntoTypedValues()
        {
            var doc = Read(Doc()).Document!;
            Assert.That(doc.DocumentSchemaVersion, Is.EqualTo(2));
            Assert.That(doc.RulesetVersion, Is.EqualTo("train-track-v1"));
            Assert.That(doc.PuzzleId, Is.EqualTo("S1-01-01-01"));
            Assert.That(doc.ContentRevision, Is.EqualTo(1));
            Assert.That((doc.Grid.Width, doc.Grid.Height), Is.EqualTo((4, 4)));
            Assert.That((doc.EndpointA.Side, doc.EndpointA.Index, doc.EndpointB.Side, doc.EndpointB.Index), Is.EqualTo((LevelSide.N, 0, LevelSide.S, 3)));
            Assert.That(doc.RowCounts, Is.EqualTo(new[] { 1, 3, 4, 1 }));
            Assert.That(doc.ColumnCounts, Is.EqualTo(new[] { 3, 1, 2, 3 }));
            Assert.That(doc.Production.Focus, Is.EqualTo(new[] { LevelFocus.OCCUPANCY, LevelFocus.ENDPOINT_GEOMETRY }));
            Assert.That(doc.Production.StarThresholdsSeconds, Is.Null);
            Assert.That(doc.SolutionPath.Count, Is.EqualTo(9));
            Assert.That(doc.SolutionPath[2].Track, Is.EqualTo(LevelTrack.TRACK_NE));
            Assert.That(doc.ProofRef.ProofHash.Profile, Is.EqualTo("STP-PROOF-JCS-1"));
        }

        [Test]
        public void RootMustBeAnObject()
        {
            foreach (var value in new[] { JsonValue.CreateArray(new JsonValue[0]), JsonValue.Null, JsonValue.CreateString("x"), JsonValue.CreateInteger(2) })
                AssertRejected(value, LevelDiagnosticCodes.SchemaType, string.Empty);
            Assert.That(Read(null!).Diagnostics.Single().Code, Is.EqualTo(LevelDiagnosticCodes.SchemaType));
        }

        [TestCase("documentSchemaVersion")] [TestCase("rulesetVersion")] [TestCase("puzzleId")] [TestCase("contentRevision")] [TestCase("grid")] [TestCase("endpoints")]
        [TestCase("rowCounts")] [TestCase("columnCounts")] [TestCase("content")] [TestCase("production")] [TestCase("completion")] [TestCase("solution")] [TestCase("proofRef")]
        public void EveryRootPropertyIsRequired(string name) => AssertRejected(JsonEdit.Remove(Doc(), "/" + name), LevelDiagnosticCodes.SchemaRequired, "/" + name);

        [TestCase("")] [TestCase("/grid")] [TestCase("/endpoints")] [TestCase("/endpoints/a")] [TestCase("/endpoints/b")] [TestCase("/content")] [TestCase("/production")]
        [TestCase("/completion")] [TestCase("/solution")] [TestCase("/solution/path/0")] [TestCase("/proofRef")] [TestCase("/proofRef/proofHash")]
        public void UnknownPropertiesAreRejectedInEveryObject(string path) =>
            AssertRejected(JsonEdit.Add(Doc(), path, "unexpected", JsonEdit.Int(1)), LevelDiagnosticCodes.SchemaUnknownProperty, path + "/unexpected");

        [Test]
        public void UnknownPropertyInsideStarThresholds_IsRejected()
        {
            var json = JsonEdit.Set(Doc(), "/production/starThresholdsSeconds", Parse("{\"twoStars\":90,\"threeStars\":60,\"calibrationVersion\":\"c1\",\"extra\":1}"));
            AssertRejected(json, LevelDiagnosticCodes.SchemaUnknownProperty, "/production/starThresholdsSeconds/extra");
        }

        private static JsonValue Parse(string text) => StrictJsonParser.ParseText(text).Value!;

        [TestCase(1)] [TestCase(3)] [TestCase(0)] [TestCase(-2)]
        public void UnknownDocumentVersion_IsRejectedBeforeAnythingElse(int version)
        {
            var result = Read(JsonEdit.Set(JsonEdit.Remove(Doc(), "/grid"), "/documentSchemaVersion", JsonEdit.Int(version)));
            Assert.That(result.Diagnostics.Select(d => d.Code), Is.EqualTo(new[] { LevelDiagnosticCodes.VersionUnknown }), "only the version diagnostic is reported for an unknown version");
            Assert.That(result.Diagnostics[0].Path, Is.EqualTo("/documentSchemaVersion"));
        }

        [Test]
        public void VersionAndConstantTypeMismatches_AreSchemaErrors()
        {
            AssertRejected(JsonEdit.Set(Doc(), "/documentSchemaVersion", JsonEdit.Str("2")), LevelDiagnosticCodes.SchemaType, "/documentSchemaVersion");
            AssertRejected(JsonEdit.Set(Doc(), "/rulesetVersion", JsonEdit.Int(1)), LevelDiagnosticCodes.SchemaType, "/rulesetVersion");
            AssertRejected(JsonEdit.Set(Doc(), "/rulesetVersion", JsonEdit.Str("train-track-v2")), LevelDiagnosticCodes.VersionRulesetUnknown, "/rulesetVersion");
            AssertRejected(JsonEdit.Set(Doc(), "/rulesetVersion", JsonEdit.Str("Train-Track-V1")), LevelDiagnosticCodes.VersionRulesetUnknown, "/rulesetVersion");
            AssertRejected(JsonEdit.Set(Doc(), "/proofRef/proofFormatVersion", JsonEdit.Int(2)), LevelDiagnosticCodes.VersionProofFormatUnknown, "/proofRef/proofFormatVersion");
            AssertRejected(JsonEdit.Set(Doc(), "/proofRef/proofFormatVersion", JsonEdit.Str("1")), LevelDiagnosticCodes.SchemaType, "/proofRef/proofFormatVersion");
        }

        [TestCase("S0-01-01-01")] [TestCase("s1-01-01-01")] [TestCase("S1-1-01-01")] [TestCase("S1-01-01-1")] [TestCase("S1-001-01-01")] [TestCase("S1-01-01-01-01")]
        [TestCase("S1-01-01-01\n")] [TestCase("S1-01-01-01 ")] [TestCase(" S1-01-01-01")] [TestCase("S01-01-01-01")] [TestCase("S-01-01-01")] [TestCase("S1_01_01_01")] [TestCase("")]
        [TestCase("S1-0a-01-01")] [TestCase("S1-01-01-01\u0660")]
        public void PuzzleIdPattern_IsStrict(string id) => AssertRejected(JsonEdit.Set(Doc(), "/puzzleId", JsonEdit.Str(id)), LevelDiagnosticCodes.SchemaPattern, "/puzzleId");

        [TestCase("S1-01-01-01")] [TestCase("S12-99-00-07")] [TestCase("S99-01-01-01")] [TestCase("S1234567890-01-01-01")]
        public void PuzzleIdPattern_AcceptsTheDocumentedShape(string id) => Assert.That(Read(JsonEdit.Set(Doc(), "/puzzleId", JsonEdit.Str(id))).Succeeded, Is.True);

        [Test]
        public void ScalarTypeMismatches_AreTypeErrors()
        {
            AssertRejected(JsonEdit.Set(Doc(), "/puzzleId", JsonEdit.Int(1)), LevelDiagnosticCodes.SchemaType, "/puzzleId");
            AssertRejected(JsonEdit.Set(Doc(), "/contentRevision", JsonEdit.Str("1")), LevelDiagnosticCodes.SchemaType, "/contentRevision");
            AssertRejected(JsonEdit.Set(Doc(), "/contentRevision", JsonValue.Null), LevelDiagnosticCodes.SchemaType, "/contentRevision");
            AssertRejected(JsonEdit.Set(Doc(), "/grid/width", JsonEdit.Str("4")), LevelDiagnosticCodes.SchemaType, "/grid/width");
            AssertRejected(JsonEdit.Set(Doc(), "/grid", JsonValue.CreateArray(new JsonValue[0])), LevelDiagnosticCodes.SchemaType, "/grid");
            AssertRejected(JsonEdit.Set(Doc(), "/rowCounts", JsonEdit.Int(1)), LevelDiagnosticCodes.SchemaType, "/rowCounts");
            AssertRejected(JsonEdit.Set(Doc(), "/rowCounts/0", JsonEdit.Str("1")), LevelDiagnosticCodes.SchemaType, "/rowCounts/0");
            AssertRejected(JsonEdit.Set(Doc(), "/production/focus", JsonEdit.Str("OCCUPANCY")), LevelDiagnosticCodes.SchemaType, "/production/focus");
            AssertRejected(JsonEdit.Set(Doc(), "/production/focus/0", JsonEdit.Int(1)), LevelDiagnosticCodes.SchemaType, "/production/focus/0");
            AssertRejected(JsonEdit.Set(Doc(), "/production/starThresholdsSeconds", JsonEdit.Int(0)), LevelDiagnosticCodes.SchemaType, "/production/starThresholdsSeconds");
            AssertRejected(JsonEdit.Set(Doc(), "/production/starThresholdsSeconds", JsonEdit.Str("none")), LevelDiagnosticCodes.SchemaType, "/production/starThresholdsSeconds");
            AssertRejected(JsonEdit.Set(Doc(), "/solution/path", JsonEdit.Str("x")), LevelDiagnosticCodes.SchemaType, "/solution/path");
            AssertRejected(JsonEdit.Set(Doc(), "/solution/path/0/x", JsonValue.FromBoolean(false)), LevelDiagnosticCodes.SchemaType, "/solution/path/0/x");
            AssertRejected(JsonEdit.Set(Doc(), "/proofRef/proofHash", JsonEdit.Str("abc")), LevelDiagnosticCodes.SchemaType, "/proofRef/proofHash");
        }

        [TestCase("/contentRevision", 0)] [TestCase("/contentRevision", -1)]
        [TestCase("/grid/width", 1)] [TestCase("/grid/width", 11)] [TestCase("/grid/height", 1)] [TestCase("/grid/height", 11)]
        [TestCase("/endpoints/a/index", -1)] [TestCase("/endpoints/a/index", 10)] [TestCase("/endpoints/b/index", 10)]
        [TestCase("/rowCounts/0", -1)] [TestCase("/rowCounts/0", 11)] [TestCase("/columnCounts/3", -1)] [TestCase("/columnCounts/3", 11)]
        [TestCase("/content/season", 0)] [TestCase("/content/networkSection", 0)] [TestCase("/content/route", 0)] [TestCase("/content/position", 0)]
        [TestCase("/production/chainDepth", 0)] [TestCase("/production/chainDepth", -3)]
        [TestCase("/solution/path/0/x", -1)] [TestCase("/solution/path/0/x", 10)] [TestCase("/solution/path/0/y", -1)] [TestCase("/solution/path/0/y", 10)]
        public void IntegerBounds_AreEnforced(string path, int value) => AssertRejected(JsonEdit.Set(Doc(), path, JsonEdit.Int(value)), LevelDiagnosticCodes.SchemaRange, path);

        [TestCase("/grid/width", 2)] [TestCase("/grid/width", 10)] [TestCase("/grid/height", 2)] [TestCase("/grid/height", 10)] [TestCase("/endpoints/a/index", 9)]
        [TestCase("/rowCounts/0", 0)] [TestCase("/rowCounts/0", 10)] [TestCase("/contentRevision", 1)] [TestCase("/contentRevision", 9007199254740991)]
        [TestCase("/production/chainDepth", 1)] [TestCase("/solution/path/0/x", 9)] [TestCase("/solution/path/0/y", 9)]
        public void IntegerBounds_AcceptTheDocumentedExtremes_AtTheSchemaStage(string path, long value) => Assert.That(Read(JsonEdit.Set(Doc(), path, JsonEdit.Int(value))).Succeeded, Is.True);

        [Test]
        public void ArrayLengths_AreEnforced()
        {
            var tooShort = JsonEdit.Set(Doc(), "/rowCounts", JsonValue.CreateArray(new[] { JsonEdit.Int(1) }));
            AssertRejected(tooShort, LevelDiagnosticCodes.SchemaLength, "/rowCounts");
            var tooLong = JsonEdit.Set(Doc(), "/columnCounts", JsonValue.CreateArray(Enumerable.Repeat(JsonEdit.Int(1), 11)));
            AssertRejected(tooLong, LevelDiagnosticCodes.SchemaLength, "/columnCounts");
            AssertRejected(JsonEdit.Set(Doc(), "/production/focus", JsonValue.CreateArray(new JsonValue[0])), LevelDiagnosticCodes.SchemaLength, "/production/focus");
            AssertRejected(JsonEdit.Set(Doc(), "/solution/path", JsonValue.CreateArray(new JsonValue[0])), LevelDiagnosticCodes.SchemaLength, "/solution/path");
            Assert.That(Read(JsonEdit.Set(Doc(), "/rowCounts", JsonValue.CreateArray(Enumerable.Repeat(JsonEdit.Int(1), 10)))).Succeeded, Is.True, "10 items satisfy the schema; grid consistency is a later stage");
        }

        [TestCase("/endpoints/a/side", "X")] [TestCase("/endpoints/a/side", "n")] [TestCase("/endpoints/b/side", "")] [TestCase("/endpoints/b/side", "NORTH")]
        [TestCase("/solution/path/0/track", "TRACK_XX")] [TestCase("/solution/path/0/track", "track_ns")] [TestCase("/solution/path/0/track", "TRACK_SN")] [TestCase("/solution/path/0/track", "")]
        [TestCase("/production/focus/0", "occupancy")] [TestCase("/production/focus/0", "UNKNOWN_FOCUS")] [TestCase("/production/focus/0", "")]
        public void ClosedEnumerations_AreEnforced(string path, string value) => AssertRejected(JsonEdit.Set(Doc(), path, JsonEdit.Str(value)), LevelDiagnosticCodes.SchemaEnum, path);

        [Test]
        public void FocusValues_AllowAllTenValues_AndForbidDuplicates()
        {
            foreach (string name in Enum.GetNames(typeof(LevelFocus)))
            {
                var result = Read(JsonEdit.Set(Doc(), "/production/focus", JsonValue.CreateArray(new[] { JsonEdit.Str(name) })));
                Assert.That(result.Succeeded, Is.True, name);
                Assert.That(result.Document!.Production.Focus.Single().ToString(), Is.EqualTo(name));
            }
            Assert.That(Enum.GetNames(typeof(LevelFocus)), Is.EqualTo(new[] { "OCCUPANCY", "EXCLUSION", "ENDPOINT_GEOMETRY", "CHAIN", "DENSITY", "COMBINATION", "ROW_COLUMN_COUNTS", "CONNECTIVITY", "NO_BRANCH", "NO_LOOP" }));
            var duplicated = JsonEdit.Set(Doc(), "/production/focus", JsonValue.CreateArray(new[] { JsonEdit.Str("CHAIN"), JsonEdit.Str("DENSITY"), JsonEdit.Str("CHAIN") }));
            AssertRejected(duplicated, LevelDiagnosticCodes.SchemaUnique, "/production/focus/2");
        }

        [TestCase("")] [TestCase("level")] [TestCase("Level.s1")] [TestCase("level.S1")] [TestCase("level..x")] [TestCase("level.x.")] [TestCase(".level.x")] [TestCase("-level.x")]
        [TestCase("1level.x")] [TestCase("level.x y")] [TestCase("level._x")] [TestCase("level.x-")] [TestCase("level-.x")] [TestCase("level.x\n")] [TestCase("level.\u00E9")]
        public void LocalizationKeyPattern_IsStrict(string key)
        {
            foreach (string path in new[] { "/content/titleKey", "/content/statusKey", "/production/entryDeductionKey", "/completion/resultTextKey" })
                AssertRejected(JsonEdit.Set(Doc(), path, JsonEdit.Str(key)), LevelDiagnosticCodes.SchemaPattern, path);
        }

        [TestCase("level.s1.01.title")] [TestCase("a.b")] [TestCase("a-b")] [TestCase("result.route-open.standard")] [TestCase("a1.b2-c3.d4")] [TestCase("level.fixture.single-cell.title")]
        public void LocalizationKeyPattern_AcceptsTheDocumentedShape(string key) =>
            Assert.That(Read(JsonEdit.Set(Doc(), "/content/titleKey", JsonEdit.Str(key))).Succeeded, Is.True);

        [Test]
        public void StringLengthConstraints_AreEnforced()
        {
            foreach (string path in new[] { "/production/qualityNote", "/production/timeClass", "/completion/mapSegmentId", "/completion/trainMomentId" })
                AssertRejected(JsonEdit.Set(Doc(), path, JsonEdit.Str("")), LevelDiagnosticCodes.SchemaLength, path);
            Assert.That(Read(JsonEdit.Set(Doc(), "/production/timeClass", JsonEdit.Str("ANY_FREE_TEXT_FOR_V2"))).Succeeded, Is.True, "v2 does not restrict timeClass to the v1 list");
        }

        [Test]
        public void StarThresholds_ObjectFormIsValidatedStructurally()
        {
            Assert.That(Read(JsonEdit.Set(Doc(), "/production/starThresholdsSeconds", Parse("{\"twoStars\":90,\"threeStars\":60,\"calibrationVersion\":\"cal-1\"}"))).Succeeded, Is.True);
            AssertRejected(JsonEdit.Set(Doc(), "/production/starThresholdsSeconds", Parse("{}")), LevelDiagnosticCodes.SchemaRequired, "/production/starThresholdsSeconds/twoStars");
            AssertRejected(JsonEdit.Set(Doc(), "/production/starThresholdsSeconds", Parse("{\"twoStars\":0,\"threeStars\":1,\"calibrationVersion\":\"c\"}")), LevelDiagnosticCodes.SchemaRange, "/production/starThresholdsSeconds/twoStars");
            AssertRejected(JsonEdit.Set(Doc(), "/production/starThresholdsSeconds", Parse("{\"twoStars\":2,\"threeStars\":1,\"calibrationVersion\":\"\"}")), LevelDiagnosticCodes.SchemaLength, "/production/starThresholdsSeconds/calibrationVersion");
            AssertRejected(JsonEdit.Set(Doc(), "/production/starThresholdsSeconds", Parse("{\"twoStars\":\"2\",\"threeStars\":1,\"calibrationVersion\":\"c\"}")), LevelDiagnosticCodes.SchemaType, "/production/starThresholdsSeconds/twoStars");
        }

        [TestCase("")] [TestCase("has space")] [TestCase("a\\b")] [TestCase("a:b")] [TestCase("proofs/\u00E9.json")]
        public void ArtifactIdPattern_IsStrict(string id) => AssertRejected(JsonEdit.Set(Doc(), "/proofRef/artifactId", JsonEdit.Str(id)), LevelDiagnosticCodes.SchemaPattern, "/proofRef/artifactId");

        [TestCase("")] [TestCase("abc")] [TestCase("B1AD9067E113F1A08B9F4DB2D0B66CB41D1726E7290A182FFC11A2EC299AB25")] [TestCase("g1ad9067e113f1a08b9f4db2d0b66cb41d1726e7290a182fffc11a2ec299ab25")]
        [TestCase("b1ad9067e113f1a08b9f4db2d0b66cb41d1726e7290a182fffc11a2ec299ab25\n")] [TestCase("b1ad9067e113f1a08b9f4db2d0b66cb41d1726e7290a182fffc11a2ec299ab250")]
        public void Sha256Pattern_IsStrict(string hash) => AssertRejected(JsonEdit.Set(Doc(), "/proofRef/proofHash/sha256", JsonEdit.Str(hash)), LevelDiagnosticCodes.SchemaPattern, "/proofRef/proofHash/sha256");

        [Test]
        public void HashProfile_MustBeANonEmptyString_AtTheSchemaStage()
        {
            AssertRejected(JsonEdit.Set(Doc(), "/proofRef/proofHash/profile", JsonEdit.Str("")), LevelDiagnosticCodes.SchemaLength, "/proofRef/proofHash/profile");
            AssertRejected(JsonEdit.Set(Doc(), "/proofRef/proofHash/profile", JsonEdit.Int(1)), LevelDiagnosticCodes.SchemaType, "/proofRef/proofHash/profile");
            Assert.That(Read(JsonEdit.Set(Doc(), "/proofRef/proofHash/profile", JsonEdit.Str("FUTURE-PROFILE-9"))).Succeeded, Is.True, "profile names are judged by the hash stage, not by the schema");
        }

        [Test]
        public void SeveralErrors_AreAllReported_InSchemaOrder_IndependentOfMemberOrder()
        {
            var broken = JsonEdit.Set(JsonEdit.Set(JsonEdit.Add(JsonEdit.Remove(Doc(), "/grid"), "", "zzz", JsonEdit.Int(1)), "/contentRevision", JsonEdit.Int(0)), "/rowCounts/0", JsonEdit.Str("x"));
            var shuffled = JsonValue.CreateObject(broken.Members.Reverse());
            var first = Read(broken).Diagnostics;
            var second = Read(shuffled).Diagnostics;
            Assert.That(first.Count, Is.GreaterThanOrEqualTo(4), Codes.Describe(first));
            Assert.That(second.Select(d => d.ToString()), Is.EqualTo(first.Select(d => d.ToString())));
            Assert.That(first.Select(d => d.Path).ToArray(), Is.EqualTo(new[] { "/zzz", "/grid", "/contentRevision", "/rowCounts/0" }));
        }

        [Test]
        public void Reader_NeverThrowsOnStructuralNoise()
        {
            var random = new Random(99);
            var baseline = Doc();
            string[] paths = { "/grid", "/endpoints/a", "/rowCounts", "/content", "/production/focus", "/production/starThresholdsSeconds", "/completion", "/solution/path/1", "/proofRef/proofHash" };
            JsonValue[] noise = { JsonValue.Null, JsonEdit.Int(7), JsonEdit.Str("x"), JsonValue.CreateArray(new JsonValue[0]), JsonValue.CreateObject(new JsonMember[0]), JsonValue.FromBoolean(true) };
            for (int i = 0; i < 2000; i++)
            {
                var json = baseline;
                int edits = random.Next(1, 4);
                for (int e = 0; e < edits; e++) json = JsonEdit.Set(json, paths[random.Next(paths.Length)], noise[random.Next(noise.Length)]);
                var result = Read(json);
                Assert.That(result.Succeeded, Is.EqualTo(result.Diagnostics.Count == 0 && result.Document != null));
                Assert.That(result.Document == null, Is.EqualTo(result.Diagnostics.Count > 0));
            }
        }
    }
}
