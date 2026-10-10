using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using STP.Infrastructure.Content;

namespace STP.Tests.Content.EditMode
{
    public sealed class LevelHashingTests
    {
        private const string GoldenPath = "Assets/StammstreckenPuzzle/Tests/EditMode/Content/level-v2-jcs-golden.json";

        private static LevelV2Document Read(JsonValue json)
        {
            var result = LevelV2Reader.Read(json);
            Assert.That(result.Document, Is.Not.Null, Codes.Describe(result.Diagnostics));
            return result.Document!;
        }

        private static JsonValue Doc(string file = Examples.V2) => Examples.Json(file);
        private static JsonValue Golden() => Examples.Json(GoldenPath);
        private static ProfiledHash Hash(JsonValue node) => new ProfiledHash(node.Get("profile")!.StringValue, node.Get("sha256")!.StringValue);

        [Test]
        public void ExampleHashes_MatchTheDocumentedKnownAnswers()
        {
            var doc = Read(Doc());
            Assert.That(LevelHashing.ComputeContentHash(doc), Is.EqualTo("1e025232202030bd420a1cee721585b36f9a1796476ff27cc22977c0970ff3c4"));
            Assert.That(LevelHashing.ComputePublicPuzzleHash(doc).ToString(), Is.EqualTo("STP-PUZZLE-SEMANTIC-JCS-1:b2a8bdb105dde81b9072eda0bdb602cb64390c1533494651f1951efea3225031"));
            Assert.That(LevelHashing.ComputeSolutionHash(doc).ToString(), Is.EqualTo("STP-SOLUTION-JCS-1:8e49c83742d402c70069db019841f0e5839691b692cb807c30fb9657df59d6ed"));
        }

        [Test]
        public void IndependentGoldenVectors_AreReproducedByteForByte()
        {
            var vectors = Golden().Get("hashVectors")!.Items;
            Assert.That(vectors.Count, Is.EqualTo(2));
            foreach (var vector in vectors)
            {
                string name = vector.Get("name")!.StringValue;
                var doc = Read(Doc(vector.Get("document")!.StringValue));
                Assert.That(LevelHashing.ComputeContentHash(doc), Is.EqualTo(vector.Get("contentHashSha256")!.StringValue), name);
                var publicBytes = JcsSerializer.Serialize(LevelHashing.PublicPuzzleProjection(doc)).Utf8!;
                Assert.That(Convert.ToBase64String(publicBytes), Is.EqualTo(vector.Get("publicPuzzleProjectionBase64")!.StringValue), name);
                Assert.That(LevelHashing.ComputePublicPuzzleHash(doc), Is.EqualTo(Hash(vector.Get("publicPuzzleHash")!)), name);
                var publicHash = LevelHashing.ComputePublicPuzzleHash(doc);
                var solutionBytes = JcsSerializer.Serialize(LevelHashing.SolutionProjection(doc.PuzzleId, publicHash, doc.SolutionPath)).Utf8!;
                Assert.That(Convert.ToBase64String(solutionBytes), Is.EqualTo(vector.Get("solutionProjectionBase64")!.StringValue), name);
                Assert.That(LevelHashing.ComputeSolutionHash(doc), Is.EqualTo(Hash(vector.Get("solutionHash")!)), name);
                Assert.That(doc.ProofRef.ProofHash, Is.EqualTo(Hash(vector.Get("proofRefHash")!)), name);
            }
        }

        [Test]
        public void LegacyV1GoldenVectors_AreReproducedAndMatchTheRecordedHashes()
        {
            foreach (var vector in Golden().Get("legacyHashVectors")!.Items)
            {
                string name = vector.Get("name")!.StringValue;
                var doc = Examples.V1Document(vector.Get("document")!.StringValue);
                Assert.That(Convert.ToBase64String(JcsSerializer.Serialize(LevelHashing.LegacyV1PuzzleProjection(doc)).Utf8!), Is.EqualTo(vector.Get("puzzleProjectionBase64")!.StringValue), name);
                Assert.That(LevelHashing.ComputeLegacyV1PuzzleHash(doc), Is.EqualTo(Hash(vector.Get("legacyPuzzleHash")!)), name);
                Assert.That(LevelHashing.ComputeLegacyV1PuzzleHash(doc).Sha256, Is.EqualTo(vector.Get("recordedPuzzleHashSha256")!.StringValue), name);
                Assert.That(LevelHashing.ComputeLegacyV1SolutionHashHex(doc), Is.EqualTo(vector.Get("solutionHashSha256")!.StringValue), name);
                Assert.That(LevelHashing.ComputeLegacyV1SolutionHashHex(doc), Is.EqualTo(vector.Get("recordedSolutionHashSha256")!.StringValue), name);
                Assert.That(LevelHashing.ComputeLegacyV1ProofHashHex(doc), Is.EqualTo(vector.Get("proofHashSha256")!.StringValue), name);
                Assert.That(LevelHashing.ComputeLegacyV1ProofHashHex(doc), Is.EqualTo(vector.Get("recordedProofHashSha256")!.StringValue), name);
                Assert.That(LevelHashing.ComputeContentHash(doc.ToJson()), Is.EqualTo(vector.Get("contentHashSha256")!.StringValue), name);
            }
        }

        [TestCase(Examples.V2, Examples.Proof)]
        [TestCase(Examples.V2Single, Examples.ProofSingle)]
        public void ProofProjectionHash_ReproducesTheProofArtifactAndTheLevelReference(string levelFile, string proofFile)
        {
            var doc = Read(Doc(levelFile));
            var proof = Doc(proofFile);
            var withoutHash = JsonValue.CreateObject(proof.Members.Where(m => m.Name != "proofHash"));
            var computed = LevelHashing.ComputeProofHash(withoutHash);
            Assert.That(computed.Profile, Is.EqualTo("STP-PROOF-JCS-1"));
            Assert.That(computed, Is.EqualTo(Hash(proof.Get("proofHash")!)));
            Assert.That(computed, Is.EqualTo(doc.ProofRef.ProofHash));
            Assert.That(Hash(proof.Get("publicPuzzleHash")!), Is.EqualTo(LevelHashing.ComputePublicPuzzleHash(doc)));
            Assert.That(Hash(proof.Get("solutionHash")!), Is.EqualTo(LevelHashing.ComputeSolutionHash(doc)));
            Assert.That(proof.Get("puzzleId")!.StringValue, Is.EqualTo(doc.PuzzleId));
        }

        [Test]
        public void ReleaseLockExample_IsBoundToTheComputedHashes()
        {
            var lockEntry = Doc(Examples.ReleaseLock).Get("puzzles")!.Items.Single();
            var json = Doc();
            var doc = Read(json);
            Assert.That(lockEntry.Get("documentSha256")!.StringValue, Is.EqualTo(LevelHashing.ComputeContentHash(json)));
            Assert.That(lockEntry.Get("documentSchemaVersion")!.IntegerValue, Is.EqualTo(doc.DocumentSchemaVersion));
            Assert.That(Hash(lockEntry.Get("publicPuzzleHash")!), Is.EqualTo(LevelHashing.ComputePublicPuzzleHash(doc)));
            Assert.That(Hash(lockEntry.Get("solutionHash")!), Is.EqualTo(LevelHashing.ComputeSolutionHash(doc)));
            Assert.That(Hash(lockEntry.Get("proofHash")!), Is.EqualTo(doc.ProofRef.ProofHash));
            var legacy = LevelHashing.ComputeLegacyV1PuzzleHash(Examples.V1Document());
            Assert.That(lockEntry.Get("legacyBindings")!.Items.Select(Hash), Does.Contain(legacy));
        }

        [Test]
        public void Projections_AreStrictlySeparated()
        {
            var doc = Read(Doc());
            var publicProjection = LevelHashing.PublicPuzzleProjection(doc);
            Assert.That(publicProjection.Members.Select(m => m.Name).OrderBy(n => n, StringComparer.Ordinal), Is.EqualTo(new[] { "columnCounts", "endpoints", "grid", "puzzleId", "rowCounts", "rulesetVersion" }));
            var solutionProjection = LevelHashing.SolutionProjection(doc.PuzzleId, LevelHashing.ComputePublicPuzzleHash(doc), doc.SolutionPath);
            Assert.That(solutionProjection.Members.Select(m => m.Name).OrderBy(n => n, StringComparer.Ordinal), Is.EqualTo(new[] { "path", "publicPuzzleHash", "puzzleId" }));
            Assert.That(solutionProjection.Get("publicPuzzleHash")!.Members.Select(m => m.Name).OrderBy(n => n, StringComparer.Ordinal), Is.EqualTo(new[] { "profile", "sha256" }));
            string content = LevelHashing.ComputeContentHash(doc);
            Assert.That(new[] { content, LevelHashing.ComputePublicPuzzleHash(doc).Sha256, LevelHashing.ComputeSolutionHash(doc).Sha256, doc.ProofRef.ProofHash.Sha256 }.Distinct().Count(), Is.EqualTo(4));
            Assert.That(publicProjection.Get("solution"), Is.Null, "the public projection never contains the authoring solution");
        }

        [Test]
        public void NeutralChanges_DoNotChangeThePublicPuzzleHash_ButChangeTheContentHash()
        {
            var baseline = Doc();
            var baselineDoc = Read(baseline);
            var publicHash = LevelHashing.ComputePublicPuzzleHash(baselineDoc);
            var solutionHash = LevelHashing.ComputeSolutionHash(baselineDoc);
            string contentHash = LevelHashing.ComputeContentHash(baselineDoc);
            var neutral = new Dictionary<string, JsonValue>
            {
                ["/contentRevision"] = JsonEdit.Int(2),
                ["/content/titleKey"] = JsonEdit.Str("level.other.title"),
                ["/content/statusKey"] = JsonEdit.Str("level.other.status"),
                ["/production/qualityNote"] = JsonEdit.Str("changed note"),
                ["/production/focus"] = JsonValue.CreateArray(new[] { JsonEdit.Str("CHAIN") }),
                ["/production/chainDepth"] = JsonEdit.Int(9),
                ["/production/timeClass"] = JsonEdit.Str("SHORT"),
                ["/production/entryDeductionKey"] = JsonEdit.Str("level.other.entry"),
                ["/production/starThresholdsSeconds"] = StrictJsonParser.ParseText("{\"twoStars\":90,\"threeStars\":60,\"calibrationVersion\":\"c\"}").Value!,
                ["/completion/mapSegmentId"] = JsonEdit.Str("other-segment"),
                ["/completion/trainMomentId"] = JsonEdit.Str("other-moment"),
                ["/completion/resultTextKey"] = JsonEdit.Str("result.other"),
                ["/proofRef/artifactId"] = JsonEdit.Str("proofs/other/solver-v2.proof-v1.json"),
                ["/proofRef/proofHash/sha256"] = JsonEdit.Str(new string('1', 64)),
                ["/proofRef/proofHash/profile"] = JsonEdit.Str("STP-PROOF-JCS-1"),
            };
            foreach (var change in neutral)
            {
                var doc = Read(JsonEdit.Set(baseline, change.Key, change.Value));
                if (change.Key != "/proofRef/proofHash/profile")
                    Assert.That(LevelHashing.ComputeContentHash(doc), Is.Not.EqualTo(contentHash), change.Key + " is part of the document");
                Assert.That(LevelHashing.ComputePublicPuzzleHash(doc), Is.EqualTo(publicHash), change.Key);
                Assert.That(LevelHashing.ComputeSolutionHash(doc), Is.EqualTo(solutionHash), change.Key);
            }
        }

        [Test]
        public void PublicPuzzleChanges_ChangeThePublicHash_AndTheSolutionHashThatEmbedsIt()
        {
            var baseline = Doc();
            var baselineDoc = Read(baseline);
            var publicHash = LevelHashing.ComputePublicPuzzleHash(baselineDoc);
            var solutionHash = LevelHashing.ComputeSolutionHash(baselineDoc);
            var changes = new Dictionary<string, JsonValue>
            {
                ["/puzzleId"] = JsonEdit.Str("S1-01-01-02"),
                ["/grid/width"] = JsonEdit.Int(5),
                ["/grid/height"] = JsonEdit.Int(5),
                ["/endpoints/a/side"] = JsonEdit.Str("W"),
                ["/endpoints/a/index"] = JsonEdit.Int(1),
                ["/endpoints/b/side"] = JsonEdit.Str("E"),
                ["/endpoints/b/index"] = JsonEdit.Int(2),
                ["/rowCounts/0"] = JsonEdit.Int(2),
                ["/columnCounts/0"] = JsonEdit.Int(2),
            };
            foreach (var change in changes)
            {
                var doc = Read(JsonEdit.Set(baseline, change.Key, change.Value));
                Assert.That(LevelHashing.ComputePublicPuzzleHash(doc), Is.Not.EqualTo(publicHash), change.Key);
                Assert.That(LevelHashing.ComputeSolutionHash(doc), Is.Not.EqualTo(solutionHash), change.Key);
            }
        }

        [Test]
        public void SolutionChanges_ChangeOnlyTheSolutionHash()
        {
            var baseline = Doc();
            var baselineDoc = Read(baseline);
            foreach (string path in new[] { "/solution/path/0/track", "/solution/path/3/x", "/solution/path/8/y" })
            {
                JsonValue value = path.EndsWith("track", StringComparison.Ordinal) ? JsonEdit.Str("TRACK_EW") : JsonEdit.Int(7);
                var doc = Read(JsonEdit.Set(baseline, path, value));
                Assert.That(LevelHashing.ComputePublicPuzzleHash(doc), Is.EqualTo(LevelHashing.ComputePublicPuzzleHash(baselineDoc)), path);
                Assert.That(LevelHashing.ComputeSolutionHash(doc), Is.Not.EqualTo(LevelHashing.ComputeSolutionHash(baselineDoc)), path);
            }
        }

        [Test]
        public void FormattingAndMemberOrder_DoNotChangeAnyHash()
        {
            string original = RepoFiles.Text(Examples.V2);
            string minified = JcsSerializer.Serialize(StrictJsonParser.ParseText(original).Value).Text!;
            string crlf = original.Replace("\n", "\r\n");
            var reordered = StrictJsonParser.ParseText(original).Value!;
            reordered = JsonValue.CreateObject(reordered.Members.Reverse());
            string reorderedText = JcsSerializer.Serialize(reordered).Text!;
            var all = new List<LevelV2LoadResult>
            {
                LevelV2Loader.LoadText(original), LevelV2Loader.LoadText(minified), LevelV2Loader.LoadText(crlf), LevelV2Loader.Load(reordered),
                LevelV2Loader.Load(Encoding.UTF8.GetBytes("  \t" + original + "\n\n"))
            };
            foreach (var result in all) Assert.That(result.IsValid, Is.True, Codes.Describe(result.Diagnostics));
            Assert.That(all.Select(r => r.ContentHash).Distinct().Count(), Is.EqualTo(1));
            Assert.That(all.Select(r => r.PublicPuzzleHash!.ToString()).Distinct().Count(), Is.EqualTo(1));
            Assert.That(all.Select(r => r.SolutionHash!.ToString()).Distinct().Count(), Is.EqualTo(1));
            Assert.That(reorderedText, Is.EqualTo(minified));
        }

        [Test]
        public void ContentHash_IsThePlainHashOfTheCanonicalDocumentBytes()
        {
            var json = Doc();
            Assert.That(LevelHashing.ComputeContentHash(json), Is.EqualTo(LevelHashing.Sha256Hex(JcsSerializer.Serialize(json).Utf8!)));
            Assert.That(LevelHashing.ComputeContentHash(json), Does.Match("^[0-9a-f]{64}$"));
        }

        [Test]
        public void Sha256_MatchesTheFipsKnownAnswers()
        {
            Assert.That(LevelHashing.Sha256Hex(new byte[0]), Is.EqualTo("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"));
            Assert.That(LevelHashing.Sha256Hex(Encoding.ASCII.GetBytes("abc")), Is.EqualTo("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad"));
            Assert.That(LevelHashing.Sha256Hex(Encoding.ASCII.GetBytes("abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq")), Is.EqualTo("248d6a61d20638b8e5c026930c3e6039a33ce45964ff2167f6ecedd419db06c1"));
            Assert.Throws<ArgumentNullException>(() => LevelHashing.Sha256Hex(null!));
        }

        [Test]
        public void ProfileRegistry_IsClosed()
        {
            foreach (string profile in new[] { "STP-PUZZLE-SEMANTIC-JCS-1", "STP-SOLUTION-JCS-1", "STP-PROOF-JCS-1", "STP-LEVEL-V1-PUZZLE-JCS-1" })
                Assert.That(LevelHashProfiles.IsKnown(profile), Is.True, profile);
            foreach (string profile in new[] { "", "stp-proof-jcs-1", "STP-PROOF-JCS-2", "STP-PROOF-JCS-1 ", "SHA256", "STP-DOCUMENT-JCS-1" })
                Assert.That(LevelHashProfiles.IsKnown(profile), Is.False, profile);
            Assert.That(LevelHashProfiles.IsKnown(null), Is.False);
        }

        [Test]
        public void Compute_DispatchesByProfile_AndRejectsUnknownProfilesHard()
        {
            var projection = StrictJsonParser.ParseText("{\"b\":1,\"a\":2}").Value!;
            var ok = LevelHashing.Compute("STP-PROOF-JCS-1", projection);
            Assert.That(ok.Succeeded, Is.True);
            Assert.That(ok.Hash!.Sha256, Is.EqualTo(LevelHashing.Sha256Hex(Encoding.ASCII.GetBytes("{\"a\":2,\"b\":1}"))));
            var unknown = LevelHashing.Compute("STP-FUTURE-JCS-9", projection);
            Assert.That(unknown.Hash, Is.Null);
            Assert.That(unknown.Error!.Code, Is.EqualTo(LevelDiagnosticCodes.HashProfileUnknown));
            Assert.That(LevelHashing.Compute(null, projection).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.HashProfileUnknown));
            Assert.That(LevelHashing.Compute("STP-PROOF-JCS-1", null).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.JcsInvalidInput));
            Assert.That(LevelHashing.Compute("STP-PROOF-JCS-1", JsonValue.CreateString("\ud800")).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.JcsSurrogate));
        }

        [Test]
        public void Verify_DistinguishesMatchMismatchAndUnknownProfile()
        {
            var doc = Read(Doc());
            var expected = LevelHashing.ComputePublicPuzzleHash(doc);
            Assert.That(LevelHashing.Verify(expected, expected, "/x"), Is.Null);
            var flipped = new ProfiledHash(expected.Profile, (expected.Sha256[0] == '0' ? "1" : "0") + expected.Sha256.Substring(1));
            Assert.That(LevelHashing.Verify(flipped, expected, "/x")!.Code, Is.EqualTo(LevelDiagnosticCodes.HashMismatch));
            Assert.That(LevelHashing.Verify(new ProfiledHash("STP-NEW-JCS-2", expected.Sha256), expected, "/x")!.Code, Is.EqualTo(LevelDiagnosticCodes.HashProfileUnknown));
            Assert.That(LevelHashing.Verify(new ProfiledHash("STP-SOLUTION-JCS-1", expected.Sha256), expected, "/x")!.Code, Is.EqualTo(LevelDiagnosticCodes.HashProfileUnknown), "a known profile in the wrong slot is not accepted");
            Assert.That(LevelHashing.Verify(null, expected, "/x")!.Code, Is.EqualTo(LevelDiagnosticCodes.HashMismatch));
            Assert.That(LevelHashing.Verify(flipped, expected, "/x")!.Path, Is.EqualTo("/x"));
        }

        [Test]
        public void HashesAreIndependentOfProcessCultureAndRepeatable()
        {
            var original = System.Globalization.CultureInfo.CurrentCulture;
            try
            {
                var doc = Read(Doc());
                string expected = LevelHashing.ComputeContentHash(doc);
                foreach (string culture in new[] { "de-DE", "tr-TR", "ar-SA", "th-TH", "fr-FR" })
                {
                    try { System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo(culture); }
                    catch (System.Globalization.CultureNotFoundException) { continue; }
                    Assert.That(LevelHashing.ComputeContentHash(Read(Doc())), Is.EqualTo(expected), culture);
                    Assert.That(LevelHashing.ComputePublicPuzzleHash(doc).Sha256, Is.EqualTo("b2a8bdb105dde81b9072eda0bdb602cb64390c1533494651f1951efea3225031"), culture);
                    Assert.That(LevelV2Loader.LoadText(RepoFiles.Text(Examples.V2)).IsValid, Is.True, culture);
                }
            }
            finally { System.Globalization.CultureInfo.CurrentCulture = original; }
        }
    }
}
