using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using STP.Infrastructure.Content;

namespace STP.Tests.Content.EditMode
{
    /// <summary>
    /// Level-v1 to Level-v2 migration (LEVEL_DATA_FORMAT.md section 9): copy semantics, determinism, idempotence, neutral field
    /// mapping, hash verification of the legacy source, all six historical focus values and the editorial-decision boundary.
    /// </summary>
    public sealed class LevelMigrationTests
    {
        private const string ProgressGolden = "tools/architecture-validation/fixtures/level-progress-v1-to-v2.golden.json";
        private static readonly string[] LegacyFocus = { "OCCUPANCY", "EXCLUSION", "ENDPOINT_GEOMETRY", "CHAIN", "DENSITY", "COMBINATION" };

        private static JsonValue V1(string file = Examples.V1) => Examples.Json(file);
        private static string Jcs(JsonValue? json) => JcsSerializer.Serialize(json).Text!;

        /// <summary>Recomputes the three recorded legacy hashes of an edited (still schema-valid) v1 tree, so edits do not trip the hash check.</summary>
        private static JsonValue Rehash(JsonValue v1)
        {
            var read = LevelV1Reader.Read(v1);
            Assert.That(read.Document, Is.Not.Null, Codes.Describe(read.Diagnostics));
            var document = read.Document!;
            var json = JsonEdit.Set(v1, "/validation/puzzleHashSha256", JsonEdit.Str(LevelHashing.ComputeLegacyV1PuzzleHash(document).Sha256));
            json = JsonEdit.Set(json, "/validation/solutionHashSha256", JsonEdit.Str(LevelHashing.ComputeLegacyV1SolutionHashHex(document)));
            return JsonEdit.Set(json, "/validation/proof/proofHashSha256", JsonEdit.Str(LevelHashing.ComputeLegacyV1ProofHashHex(document)));
        }

        private static JsonValue Parse(string text)
        {
            var parsed = StrictJsonParser.ParseText(text);
            Assert.That(parsed.Value, Is.Not.Null, parsed.Error?.ToString());
            return parsed.Value!;
        }

        // ---- positive fixtures ---------------------------------------------------------------------------------

        [TestCase(Examples.V1, Examples.V2)]
        [TestCase(Examples.V1Single, Examples.V2Single)]
        public void V1Examples_MigrateToExactlyTheCommittedV2Examples(string v1File, string v2File)
        {
            var result = LevelV1ToV2Migrator.Migrate(V1(v1File));
            Assert.That(result.Succeeded, Is.True, Codes.Describe(result.Diagnostics));
            Assert.That(result.WasMigrated, Is.True);
            Assert.That(Jcs(result.Json), Is.EqualTo(Jcs(Examples.Json(v2File))), "the migrated tree must be JCS-identical to the committed Level-v2 example");

            var expected = Examples.V2Document(v2File);
            Assert.That(result.ContentHash, Is.EqualTo(LevelHashing.ComputeContentHash(expected)));
            Assert.That(result.PublicPuzzleHash, Is.EqualTo(LevelHashing.ComputePublicPuzzleHash(expected)));
            Assert.That(result.SolutionHash, Is.EqualTo(LevelHashing.ComputeSolutionHash(expected)));
            Assert.That(result.Document!.ProofRef.ProofHash, Is.EqualTo(expected.ProofRef.ProofHash));
            Assert.That(result.Document.ProofRef.ArtifactId, Is.EqualTo(expected.ProofRef.ArtifactId));
        }

        [Test]
        public void MainExample_ProducesTheDocumentedKnownAnswers()
        {
            var result = LevelV1ToV2Migrator.Migrate(V1());
            Assert.That(result.ContentHash, Is.EqualTo("1e025232202030bd420a1cee721585b36f9a1796476ff27cc22977c0970ff3c4"));
            Assert.That(result.PublicPuzzleHash!.ToString(), Is.EqualTo("STP-PUZZLE-SEMANTIC-JCS-1:b2a8bdb105dde81b9072eda0bdb602cb64390c1533494651f1951efea3225031"));
            Assert.That(result.SolutionHash!.ToString(), Is.EqualTo("STP-SOLUTION-JCS-1:8e49c83742d402c70069db019841f0e5839691b692cb807c30fb9657df59d6ed"));
            Assert.That(result.Document!.ProofRef.ProofHash.ToString(), Is.EqualTo("STP-PROOF-JCS-1:b1ad9067e113f1a08b9f4db2d0b66cb41d1726e7290a182fffc11a2ec299ab25"));
        }

        [Test]
        public void ProgressGolden_BindsTheActualSourceTargetAndLockDocuments()
        {
            var golden = Examples.Json(ProgressGolden);
            foreach (string name in new[] { "sourceDocument", "targetDocument", "releaseLockDocument" })
            {
                var node = golden.Get(name)!;
                var file = Examples.Json(node.Get("path")!.StringValue);
                Assert.That(LevelHashing.ComputeContentHash(file), Is.EqualTo(node.Get("documentSha256")!.StringValue), name);
            }
            string sourcePath = golden.Get("sourceDocument")!.Get("path")!.StringValue;
            var migrated = LevelV1ToV2Migrator.Migrate(Examples.Json(sourcePath));
            Assert.That(migrated.Succeeded, Is.True, Codes.Describe(migrated.Diagnostics));
            Assert.That(migrated.ContentHash, Is.EqualTo(golden.Get("targetDocument")!.Get("documentSha256")!.StringValue));

            var binding = golden.Get("releaseLockBinding")!;
            Assert.That(binding.Get("legacyProfile")!.StringValue, Is.EqualTo(LevelHashProfiles.LevelV1Puzzle));
            Assert.That(binding.Get("puzzleId")!.StringValue, Is.EqualTo(migrated.Document!.PuzzleId));
            var legacy = LevelHashing.ComputeLegacyV1PuzzleHash(Examples.V1Document(sourcePath));
            Assert.That(legacy.Sha256, Is.EqualTo(binding.Get("legacySha256")!.StringValue));
            Assert.That(golden.Get("source")!.Get("puzzleHashSha256")!.StringValue, Is.EqualTo(legacy.Sha256));
            var publicHash = binding.Get("publicPuzzleHash")!;
            Assert.That(migrated.PublicPuzzleHash, Is.EqualTo(new ProfiledHash(publicHash.Get("profile")!.StringValue, publicHash.Get("sha256")!.StringValue)));
            var expected = golden.Get("expected")!;
            var atCompletion = expected.Get("publicPuzzleHashAtFirstCompletion")!;
            Assert.That(migrated.PublicPuzzleHash, Is.EqualTo(new ProfiledHash(atCompletion.Get("profile")!.StringValue, atCompletion.Get("sha256")!.StringValue)));
            Assert.That(expected.Get("puzzleId")!.StringValue, Is.EqualTo(migrated.Document.PuzzleId));
        }

        [Test]
        public void Migration_MapsIdToPuzzleId_AndKeepsEveryNeutralFieldUnchanged()
        {
            var source = Examples.V1Document();
            var target = LevelV1ToV2Migrator.Migrate(source).Document!;
            Assert.That(target.DocumentSchemaVersion, Is.EqualTo(2));
            Assert.That(target.PuzzleId, Is.EqualTo(source.Id));
            Assert.That(target.RulesetVersion, Is.EqualTo(source.RulesetVersion));
            Assert.That(target.ContentRevision, Is.EqualTo(source.ContentRevision), "the format change alone must not raise contentRevision");
            var before = source.ToJson();
            var after = target.ToJson();
            foreach (string member in new[] { "rulesetVersion", "contentRevision", "grid", "endpoints", "rowCounts", "columnCounts", "content", "production", "completion", "solution" })
                Assert.That(Jcs(after.Get(member)), Is.EqualTo(Jcs(before.Get(member))), member);
            Assert.That(Jcs(after.Get("puzzleId")), Is.EqualTo(Jcs(before.Get("id"))));
            Assert.That(target.RowCounts, Is.EqualTo(source.RowCounts));
            Assert.That(target.ColumnCounts, Is.EqualTo(source.ColumnCounts));
            Assert.That(target.ProofRef.ProofFormatVersion, Is.EqualTo(1));
            Assert.That(target.ProofRef.ArtifactId, Is.EqualTo("proofs/S1-01-01-01/solver-v1.proof-v1.json"));
        }

        [Test]
        public void ContentRevision_IsCarriedOver_AndNotRaised()
        {
            var edited = Rehash(JsonEdit.Set(V1(), "/contentRevision", JsonEdit.Int(7)));
            var result = LevelV1ToV2Migrator.Migrate(edited);
            Assert.That(result.Succeeded, Is.True, Codes.Describe(result.Diagnostics));
            Assert.That(result.Document!.ContentRevision, Is.EqualTo(7));
        }

        [Test]
        public void Migration_IsDeterministic_AndIndependentOfInputFormatting()
        {
            string baseline = Jcs(LevelV1ToV2Migrator.Migrate(V1()).Json);
            for (int i = 0; i < 5; i++) Assert.That(Jcs(LevelV1ToV2Migrator.Migrate(V1()).Json), Is.EqualTo(baseline));

            var reversed = JsonValue.CreateObject(V1().Members.Reverse().ToList());
            Assert.That(Jcs(LevelV1ToV2Migrator.Migrate(reversed).Json), Is.EqualTo(baseline), "member order of the source is irrelevant");

            string pretty = RepoFiles.Text(Examples.V1);
            string minified = Jcs(V1());
            foreach (string text in new[] { pretty, minified, pretty.Replace("\n", "\r\n") })
            {
                var result = LevelV1ToV2Migrator.Migrate(TextMutation.Utf8(text));
                Assert.That(result.Succeeded, Is.True, Codes.Describe(result.Diagnostics));
                Assert.That(Jcs(result.Json), Is.EqualTo(baseline));
            }
        }

        [Test]
        public void Migration_WorksOnACopy_AndLeavesTheSourceUntouched()
        {
            var json = V1();
            string before = Jcs(json);
            var read = LevelV1Reader.Read(json);
            var document = read.Document!;
            string documentBefore = Jcs(document.ToJson());
            LevelV1ToV2Migrator.Migrate(json);
            LevelV1ToV2Migrator.Migrate(document);
            Assert.That(Jcs(json), Is.EqualTo(before));
            Assert.That(Jcs(document.ToJson()), Is.EqualTo(documentBefore));
            Assert.That(Jcs(document.ToJson()), Is.EqualTo(before), "the typed v1 document reproduces the source exactly");
        }

        // ---- idempotence ---------------------------------------------------------------------------------------

        [TestCase(Examples.V1)]
        [TestCase(Examples.V1Single)]
        public void MigrateToCurrent_IsIdempotent(string v1File)
        {
            var first = LevelV1ToV2Migrator.MigrateToCurrent(V1(v1File));
            Assert.That(first.Succeeded, Is.True, Codes.Describe(first.Diagnostics));
            Assert.That(first.WasMigrated, Is.True);

            var second = LevelV1ToV2Migrator.MigrateToCurrent(first.Json);
            Assert.That(second.Succeeded, Is.True, Codes.Describe(second.Diagnostics));
            Assert.That(second.WasMigrated, Is.False, "an already current document is passed through");
            Assert.That(Jcs(second.Json), Is.EqualTo(Jcs(first.Json)));
            Assert.That(second.ContentHash, Is.EqualTo(first.ContentHash));

            var third = LevelV1ToV2Migrator.MigrateToCurrent(second.Json);
            Assert.That(third.ContentHash, Is.EqualTo(first.ContentHash));
        }

        [TestCase(Examples.V2)]
        [TestCase(Examples.V2Single)]
        public void CommittedV2Examples_PassThroughUnchanged(string v2File)
        {
            var result = LevelV1ToV2Migrator.MigrateToCurrent(RepoFiles.Bytes(v2File));
            Assert.That(result.Succeeded, Is.True, Codes.Describe(result.Diagnostics));
            Assert.That(result.WasMigrated, Is.False);
            Assert.That(Jcs(result.Json), Is.EqualTo(Jcs(Examples.Json(v2File))));
            Assert.That(result.ContentHash, Is.EqualTo(LevelHashing.ComputeContentHash(Examples.V2Document(v2File))));
        }

        [Test]
        public void MigrateToCurrent_AppliedToBytesOfV1_MatchesTheJsonEntryPoint()
        {
            var fromBytes = LevelV1ToV2Migrator.MigrateToCurrent(RepoFiles.Bytes(Examples.V1));
            var fromJson = LevelV1ToV2Migrator.MigrateToCurrent(V1());
            Assert.That(fromBytes.ContentHash, Is.EqualTo(fromJson.ContentHash));
            var direct = LevelV1ToV2Migrator.Migrate(RepoFiles.Bytes(Examples.V1));
            Assert.That(direct.ContentHash, Is.EqualTo(fromJson.ContentHash));
        }

        [Test]
        public void MigratedDocument_PassesTheFullLoaderPipeline()
        {
            var result = LevelV1ToV2Migrator.Migrate(V1());
            var load = LevelV2Loader.Load(result.Json);
            Assert.That(load.IsValid, Is.True, Codes.Describe(load.Diagnostics));
            Assert.That(load.ContentHash, Is.EqualTo(result.ContentHash));
            Assert.That(load.PublicPuzzleHash, Is.EqualTo(result.PublicPuzzleHash));
            Assert.That(load.SolutionHash, Is.EqualTo(result.SolutionHash));
        }

        // ---- all six historical focus values -------------------------------------------------------------------

        [Test]
        public void TheSixTestedFocusValues_AreExactlyTheLegacySchemaEnum_AndAllExistInTheV2Schema()
        {
            string[] Enum(string schemaFile) => Examples.Json(schemaFile).Get("properties")!.Get("production")!.Get("properties")!.Get("focus")!.Get("items")!.Get("enum")!.Items.Select(i => i.StringValue).ToArray();
            string[] legacy = Enum("ARCHITECTURE/schemas/level-v1.schema.json");
            string[] current = Enum("ARCHITECTURE/schemas/level-v2.schema.json");
            Assert.That(legacy, Is.EqualTo(LegacyFocus));
            Assert.That(current.Take(6).ToArray(), Is.EqualTo(legacy), "the v2 schema keeps the six historical values first and unchanged");
            foreach (string value in legacy) Assert.That(current, Does.Contain(value));
            Assert.That(current.Length, Is.GreaterThan(legacy.Length));
        }

        [Test]
        public void AllSixLegacyFocusValues_StayValidAndUnchangedInV2()
        {
            Assert.That(LegacyFocus.Length, Is.EqualTo(6));
            foreach (string focus in LegacyFocus)
            {
                var edited = Rehash(JsonEdit.Set(V1(), "/production/focus", JsonValue.CreateArray(new[] { JsonEdit.Str(focus) })));
                var result = LevelV1ToV2Migrator.Migrate(edited);
                Assert.That(result.Succeeded, Is.True, focus + ": " + Codes.Describe(result.Diagnostics));
                var focusNode = result.Json!.Get("production")!.Get("focus")!;
                Assert.That(focusNode.Items.Select(i => i.StringValue).ToArray(), Is.EqualTo(new[] { focus }), focus);
                Assert.That(LevelV2Loader.Load(result.Json).IsValid, Is.True, focus);
            }
        }

        [Test]
        public void AllSixLegacyFocusValuesTogether_KeepTheirOrder()
        {
            var all = JsonValue.CreateArray(LegacyFocus.Select(f => JsonEdit.Str(f)).ToList());
            var result = LevelV1ToV2Migrator.Migrate(Rehash(JsonEdit.Set(V1(), "/production/focus", all)));
            Assert.That(result.Succeeded, Is.True, Codes.Describe(result.Diagnostics));
            Assert.That(result.Json!.Get("production")!.Get("focus")!.Items.Select(i => i.StringValue).ToArray(), Is.EqualTo(LegacyFocus));
        }

        [Test]
        public void ANewV2OnlyFocusValue_IsNotAcceptedInAV1Source()
        {
            var edited = JsonEdit.Set(V1(), "/production/focus", JsonValue.CreateArray(new[] { JsonEdit.Str("NO_LOOP") }));
            var result = LevelV1ToV2Migrator.Migrate(edited);
            Assert.That(result.Document, Is.Null);
            Assert.That(Codes.Of(result.Diagnostics), Does.Contain(LevelDiagnosticCodes.SchemaEnum).Or.Contain(LevelDiagnosticCodes.SchemaRange));
        }

        // ---- legacy hash verification --------------------------------------------------------------------------

        [TestCase("/validation/puzzleHashSha256")]
        [TestCase("/validation/solutionHashSha256")]
        [TestCase("/validation/proof/proofHashSha256")]
        public void TamperedLegacyHash_IsRejected_AtThatPath(string path)
        {
            var edited = JsonEdit.Set(V1(), path, JsonEdit.Str(new string('0', 64)));
            var result = LevelV1ToV2Migrator.Migrate(edited);
            Assert.That(result.Document, Is.Null);
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Diagnostics.Select(d => (d.Code, d.Path)).ToArray(), Is.EqualTo(new[] { (LevelDiagnosticCodes.HashMismatch, path) }));
            Assert.That(result.ContentHash, Is.Null);
            Assert.That(result.PublicPuzzleHash, Is.Null);
        }

        [Test]
        public void ChangedPublicInput_WithoutRehash_IsAHashMismatch()
        {
            var edited = JsonEdit.Set(V1(), "/rowCounts", Parse("[1,4,3,1]"));
            var result = LevelV1ToV2Migrator.Migrate(edited);
            Assert.That(result.Document, Is.Null);
            Assert.That(Codes.Of(result.Diagnostics), Does.Contain(LevelDiagnosticCodes.HashMismatch));
        }

        [Test]
        public void ChangedSolutionPath_WithoutRehash_IsAHashMismatch()
        {
            var edited = JsonEdit.Set(V1(), "/solution/path/1/track", JsonEdit.Str("TRACK_EW"));
            var result = LevelV1ToV2Migrator.Migrate(edited);
            Assert.That(result.Document, Is.Null);
            Assert.That(Codes.Of(result.Diagnostics), Does.Contain(LevelDiagnosticCodes.HashMismatch));
        }

        [Test]
        public void ChangedProofFacts_WithoutRehash_AreAHashMismatch()
        {
            foreach (string path in new[] { "/validation/proof/searchNodes", "/validation/proof/deductionSteps", "/validation/proof/maxDeductionDepth", "/validation/proof/requiredGuessDepth" })
            {
                var original = JsonEdit.Resolve(V1(), path).IntegerValue;
                var result = LevelV1ToV2Migrator.Migrate(JsonEdit.Set(V1(), path, JsonEdit.Int(original + 1)));
                Assert.That(result.Document, Is.Null, path);
                Assert.That(Codes.Of(result.Diagnostics), Does.Contain(LevelDiagnosticCodes.HashMismatch), path);
            }
        }

        [Test]
        public void EditorialAndTextFields_DoNotParticipateInTheLegacyPuzzleHash_AndSurviveUnchanged()
        {
            var edited = JsonEdit.Set(V1(), "/production/qualityNote", JsonEdit.Str("Another editorial note."));
            edited = JsonEdit.Set(edited, "/content/titleKey", JsonEdit.Str("level.s1.01.01.01.other-title"));
            var result = LevelV1ToV2Migrator.Migrate(edited);
            Assert.That(result.Succeeded, Is.True, Codes.Describe(result.Diagnostics));
            Assert.That(result.Document!.Production.QualityNote, Is.EqualTo("Another editorial note."));
            Assert.That(result.Document.Content.TitleKey, Is.EqualTo("level.s1.01.01.01.other-title"));
            Assert.That(result.PublicPuzzleHash, Is.EqualTo(LevelV1ToV2Migrator.Migrate(V1()).PublicPuzzleHash), "texts are not part of the public puzzle hash");
            Assert.That(result.ContentHash, Is.Not.EqualTo(LevelV1ToV2Migrator.Migrate(V1()).ContentHash), "but they are part of the content hash");
        }

        // ---- editorial decisions -------------------------------------------------------------------------------

        [Test]
        public void ChainDepthZero_NeedsAnEditorialDecision()
        {
            var result = LevelV1ToV2Migrator.Migrate(Rehash(JsonEdit.Set(V1(), "/production/chainDepth", JsonEdit.Int(0))));
            AssertEditorial(result, "/production/chainDepth");
        }

        [Test]
        public void GridWiderThanTheV2Maximum_NeedsAnEditorialDecision()
        {
            var json = V1();
            json = JsonEdit.Set(json, "/grid/width", JsonEdit.Int(12));
            json = JsonEdit.Set(json, "/columnCounts", JsonValue.CreateArray(Enumerable.Repeat(0, 12).Select(v => JsonEdit.Int(v)).ToList()));
            json = JsonEdit.Set(json, "/endpoints/b/index", JsonEdit.Int(3));
            var result = LevelV1ToV2Migrator.Migrate(Rehash(json));
            AssertEditorial(result, null);
            Assert.That(result.Diagnostics.Select(d => d.Path), Does.Contain("/grid/width"));
        }

        [TestCase("/validation/proof/searchNodes")]
        [TestCase("/validation/proof/deductionSteps")]
        [TestCase("/validation/proof/maxDeductionDepth")]
        public void ProofFactsBelowTheProofV1Minimum_NeedAnEditorialDecision(string path)
        {
            var result = LevelV1ToV2Migrator.Migrate(Rehash(JsonEdit.Set(V1(), path, JsonEdit.Int(0))));
            AssertEditorial(result, path);
        }

        [Test]
        public void V1KeysThatV2DoesNotAccept_NeedAnEditorialDecision()
        {
            // v1 allows hyphens inside dotted segments such as "a.--" while v2 only allows separators between alphanumeric groups.
            var json = Rehash(JsonEdit.Set(V1(), "/content/titleKey", JsonEdit.Str("level.s1.--")));
            Assert.That(LevelV1Reader.Read(json).Document, Is.Not.Null, "precondition: still a valid Level-v1 document");
            var result = LevelV1ToV2Migrator.Migrate(json);
            AssertEditorial(result, "/content/titleKey");
        }

        [Test]
        public void EditorialDecision_NeverProducesADocumentOrHashes_AndInventsNothing()
        {
            var result = LevelV1ToV2Migrator.Migrate(Rehash(JsonEdit.Set(V1(), "/production/chainDepth", JsonEdit.Int(0))));
            Assert.That(result.Document, Is.Null);
            Assert.That(result.Json, Is.Null);
            Assert.That(result.ContentHash, Is.Null);
            Assert.That(result.PublicPuzzleHash, Is.Null);
            Assert.That(result.SolutionHash, Is.Null);
            Assert.That(result.WasMigrated, Is.False);
        }

        [Test]
        public void SemanticallyBrokenSolution_WithMatchingLegacyHashes_IsStillRejected()
        {
            var json = Rehash(JsonEdit.Set(V1(), "/solution/path/1/track", JsonEdit.Str("TRACK_EW")));
            var result = LevelV1ToV2Migrator.Migrate(json);
            Assert.That(result.Document, Is.Null);
            Assert.That(result.Diagnostics, Is.Not.Empty);
            Assert.That(Codes.Of(result.Diagnostics), Does.Not.Contain(LevelDiagnosticCodes.HashMismatch));
        }

        [Test]
        public void IdThatContradictsTheContentSegments_IsRejectedAfterTheMapping()
        {
            var json = Rehash(JsonEdit.Set(V1(), "/content/position", JsonEdit.Int(2)));
            var result = LevelV1ToV2Migrator.Migrate(json);
            Assert.That(result.Document, Is.Null);
            Assert.That(Codes.Of(result.Diagnostics), Does.Contain(LevelDiagnosticCodes.IdContentMismatch));
        }

        // ---- negative inputs -----------------------------------------------------------------------------------

        [Test]
        public void ANonV1Document_IsNotMigratedByTheV1Entry()
        {
            var result = LevelV1ToV2Migrator.Migrate(Examples.Json(Examples.V2));
            Assert.That(result.Document, Is.Null);
            Assert.That(result.Diagnostics, Is.Not.Empty);
        }

        [Test]
        public void NullAndGarbageInputs_FailWithDiagnostics_AndNeverThrow()
        {
            Assert.That(LevelV1ToV2Migrator.Migrate((LevelV1Document?)null).Diagnostics, Is.Not.Empty);
            Assert.That(LevelV1ToV2Migrator.Migrate((JsonValue?)null).Diagnostics, Is.Not.Empty);
            Assert.That(LevelV1ToV2Migrator.Migrate((byte[]?)null).Diagnostics, Is.Not.Empty);
            Assert.That(LevelV1ToV2Migrator.MigrateToCurrent((JsonValue?)null).Diagnostics, Is.Not.Empty);
            Assert.That(LevelV1ToV2Migrator.MigrateToCurrent((byte[]?)null).Diagnostics, Is.Not.Empty);
            Assert.That(LevelV1ToV2Migrator.Migrate(TextMutation.Utf8("[]")).Diagnostics, Is.Not.Empty);
            Assert.That(LevelV1ToV2Migrator.Migrate(TextMutation.Utf8("{")).Diagnostics.Select(d => d.Stage).Distinct().ToArray(), Is.EqualTo(new[] { LevelDiagnosticStage.Parse }));
        }

        [Test]
        public void ABomPrefixedSource_IsRejectedByTheStrictParser()
        {
            var bytes = new byte[] { 0xEF, 0xBB, 0xBF }.Concat(RepoFiles.Bytes(Examples.V1)).ToArray();
            var result = LevelV1ToV2Migrator.Migrate(bytes);
            Assert.That(result.Document, Is.Null);
            Assert.That(Codes.Of(result.Diagnostics), Is.EqualTo(new[] { LevelDiagnosticCodes.ParseBom }));
        }

        [Test]
        public void DuplicateKeysInTheSource_AreRejectedBeforeAnyMigration()
        {
            string text = TextMutation.Replace(RepoFiles.Text(Examples.V1), "\"rulesetVersion\": \"train-track-v1\",", "\"rulesetVersion\": \"train-track-v1\", \"rulesetVersion\": \"train-track-v1\",");
            var result = LevelV1ToV2Migrator.Migrate(TextMutation.Utf8(text));
            Assert.That(result.Document, Is.Null);
            Assert.That(Codes.Of(result.Diagnostics), Is.EqualTo(new[] { LevelDiagnosticCodes.ParseDuplicateKey }));
        }

        [Test]
        public void UnknownLegacyProperty_IsRejectedByTheClosedV1Reader()
        {
            var json = JsonEdit.Add(V1(), "", "extra", JsonEdit.Int(1));
            var result = LevelV1ToV2Migrator.Migrate(json);
            Assert.That(result.Document, Is.Null);
            Assert.That(Codes.Of(result.Diagnostics), Does.Contain(LevelDiagnosticCodes.SchemaUnknownProperty));
        }

        [Test]
        public void SchemaVersionThree_IsUnknown()
        {
            var result = LevelV1ToV2Migrator.Migrate(JsonEdit.Set(V1(), "/schemaVersion", JsonEdit.Int(3)));
            Assert.That(Codes.Of(result.Diagnostics), Is.EqualTo(new[] { LevelDiagnosticCodes.VersionUnknown }));
        }

        [Test]
        public void MigrateToCurrent_RejectsAmbiguousAndUnknownVersions()
        {
            var both = JsonEdit.Add(V1(), "", "documentSchemaVersion", JsonEdit.Int(2));
            Assert.That(Codes.Of(LevelV1ToV2Migrator.MigrateToCurrent(both).Diagnostics), Is.EqualTo(new[] { LevelDiagnosticCodes.VersionUnknown }));
            Assert.That(Codes.Of(LevelV1ToV2Migrator.MigrateToCurrent(JsonEdit.Set(V1(), "/schemaVersion", JsonEdit.Int(0))).Diagnostics), Is.EqualTo(new[] { LevelDiagnosticCodes.VersionUnknown }));
            Assert.That(Codes.Of(LevelV1ToV2Migrator.MigrateToCurrent(JsonEdit.Set(Examples.Json(Examples.V2), "/documentSchemaVersion", JsonEdit.Int(3))).Diagnostics), Is.EqualTo(new[] { LevelDiagnosticCodes.VersionUnknown }));
            Assert.That(Codes.Of(LevelV1ToV2Migrator.MigrateToCurrent(JsonEdit.Remove(V1(), "/schemaVersion")).Diagnostics), Is.EqualTo(new[] { LevelDiagnosticCodes.VersionUnknown }));
            Assert.That(Codes.Of(LevelV1ToV2Migrator.MigrateToCurrent(Parse("[]")).Diagnostics), Is.EqualTo(new[] { LevelDiagnosticCodes.SchemaType }));
            Assert.That(Codes.Of(LevelV1ToV2Migrator.MigrateToCurrent(Parse("{\"schemaVersion\":\"1\"}")).Diagnostics), Is.EqualTo(new[] { LevelDiagnosticCodes.VersionUnknown }));
        }

        [Test]
        public void MigrateToCurrent_ValidatesAnAlreadyCurrentDocument_InsteadOfTrustingIt()
        {
            var broken = JsonEdit.Set(Examples.Json(Examples.V2), "/solution/path/1/track", JsonEdit.Str("TRACK_EW"));
            var result = LevelV1ToV2Migrator.MigrateToCurrent(broken);
            Assert.That(result.Document, Is.Null);
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Diagnostics, Is.Not.Empty);
        }

        // ---- property: derived v1 sources of random valid v2 puzzles ---------------------------------------------

        /// <summary>Builds the legacy v1 source of a v2 document (the inverse of the neutral mapping) with freshly computed legacy hashes.</summary>
        private static JsonValue LegacyOf(JsonValue v2, LevelV2Document document)
        {
            var members = new List<JsonMember>();
            foreach (var member in v2.Members)
            {
                if (member.Name == "documentSchemaVersion") members.Add(new JsonMember("schemaVersion", JsonEdit.Int(1)));
                else if (member.Name == "puzzleId") members.Add(new JsonMember("id", member.Value));
                else if (member.Name == "proofRef") members.Add(new JsonMember("validation", Parse(
                    "{\"puzzleHashSha256\":\"" + new string('0', 64) + "\",\"solutionHashSha256\":\"" + new string('0', 64) + "\",\"solverVersion\":\"solver-v1\",\"solutionCount\":1,"
                    + "\"proof\":{\"searchNodes\":1,\"deductionSteps\":" + document.SolutionPath.Count + ",\"maxDeductionDepth\":1,\"requiredGuessDepth\":0,\"proofHashSha256\":\"" + new string('0', 64) + "\"}}")));
                else members.Add(member);
            }
            return Rehash(JsonValue.CreateObject(members));
        }

        [Test]
        public void RandomValidPuzzles_SurviveALegacyRoundTrip_WithIdenticalPublicIdentity()
        {
            var random = new Random(20240607);
            for (int i = 0; i < 200; i++)
            {
                var v2 = LevelV2SemanticsTests.RandomValidDocument(random, i);
                var document = LevelV2Reader.Read(v2).Document!;
                var legacy = LegacyOf(v2, document);
                var result = LevelV1ToV2Migrator.Migrate(legacy);
                Assert.That(result.Succeeded, Is.True, "round " + i + ": " + Codes.Describe(result.Diagnostics) + "\n" + Jcs(legacy));
                Assert.That(result.PublicPuzzleHash, Is.EqualTo(LevelHashing.ComputePublicPuzzleHash(document)), "round " + i);
                Assert.That(result.SolutionHash, Is.EqualTo(LevelHashing.ComputeSolutionHash(document)), "round " + i);
                Assert.That(Jcs(result.Json!.Get("solution")), Is.EqualTo(Jcs(v2.Get("solution"))), "round " + i);
                var again = LevelV1ToV2Migrator.MigrateToCurrent(result.Json);
                Assert.That(again.ContentHash, Is.EqualTo(result.ContentHash), "round " + i + " idempotence");
            }
        }

        private static void AssertEditorial(LevelMigrationResult result, string? path)
        {
            Assert.That(result.Document, Is.Null, Codes.Describe(result.Diagnostics));
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Diagnostics, Is.Not.Empty);
            Assert.That(result.Diagnostics.Select(d => d.Code).Distinct().ToArray(), Is.EqualTo(new[] { LevelDiagnosticCodes.MigrationNeedsEditorialDecision }), Codes.Describe(result.Diagnostics));
            Assert.That(result.Diagnostics.All(d => d.Stage == LevelDiagnosticStage.Migration), Is.True);
            if (path != null) Assert.That(result.Diagnostics.Select(d => d.Path), Does.Contain(path));
        }
    }
}
