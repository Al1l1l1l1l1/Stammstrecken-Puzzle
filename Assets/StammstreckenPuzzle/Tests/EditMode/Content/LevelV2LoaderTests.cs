using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using STP.Infrastructure.Content;
using STP.Puzzle.Domain;

namespace STP.Tests.Content.EditMode
{
    /// <summary>
    /// The complete Level-v2 pipeline (Parse, Schema, Domain map, Semantic): stage order without masking, determinism,
    /// the closed import gate, robustness against arbitrary input and the identity of the produced hashes.
    /// </summary>
    public sealed class LevelV2LoaderTests
    {
        private static JsonValue Doc(string file = Examples.V2) => Examples.Json(file);
        private static JsonValue Parse(string text) => StrictJsonParser.ParseText(text).Value!;
        private static string Jcs(JsonValue? json) => JcsSerializer.Serialize(json).Text!;
        private static LevelDiagnosticStage[] Stages(LevelV2LoadResult result) => result.Diagnostics.Select(d => d.Stage).Distinct().ToArray();

        // ---- valid documents and the import gate -----------------------------------------------------------------

        [TestCase(Examples.V2)]
        [TestCase(Examples.V2Single)]
        public void CommittedExamples_AreValid_AndAwaitTheProofGate(string file)
        {
            var result = LevelV2Loader.Load(RepoFiles.Bytes(file));
            Assert.That(result.IsValid, Is.True, Codes.Describe(result.Diagnostics));
            Assert.That(result.Diagnostics, Is.Empty);
            Assert.That(result.Document, Is.Not.Null);
            Assert.That(result.Definition, Is.Not.Null);
            Assert.That(result.ImportState, Is.EqualTo(LevelImportState.AwaitingProofGate));
            Assert.That(result.IsRuntimeImportable, Is.False, "no Level-v2 source is runtime importable before the later proof gate");
            Assert.That(result.ImportBlockers.Select(b => (b.Stage, b.Code, b.Path)).ToArray(),
                Is.EqualTo(new[] { (LevelDiagnosticStage.Import, LevelDiagnosticCodes.ImportProofGateMissing, "/proofRef") }));
            Assert.That(result.ContentHash, Is.EqualTo(LevelHashing.ComputeContentHash(Doc(file))));
            Assert.That(result.PublicPuzzleHash, Is.EqualTo(LevelHashing.ComputePublicPuzzleHash(result.Document!)));
            Assert.That(result.SolutionHash, Is.EqualTo(LevelHashing.ComputeSolutionHash(result.Document!)));
        }

        [Test]
        public void MainExample_ReproducesTheDocumentedHashes()
        {
            var result = LevelV2Loader.Load(RepoFiles.Bytes(Examples.V2));
            Assert.That(result.ContentHash, Is.EqualTo("1e025232202030bd420a1cee721585b36f9a1796476ff27cc22977c0970ff3c4"));
            Assert.That(result.PublicPuzzleHash!.ToString(), Is.EqualTo("STP-PUZZLE-SEMANTIC-JCS-1:b2a8bdb105dde81b9072eda0bdb602cb64390c1533494651f1951efea3225031"));
            Assert.That(result.SolutionHash!.ToString(), Is.EqualTo("STP-SOLUTION-JCS-1:8e49c83742d402c70069db019841f0e5839691b692cb807c30fb9657df59d6ed"));
        }

        [Test]
        public void ProofRefHashIsOnlyAReference_NoProofArtifactIsLoadedOrRequired()
        {
            var result = LevelV2Loader.Load(Doc());
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.ImportBlockers.Count, Is.EqualTo(1));
            Assert.That(result.ImportBlockers[0].Code, Is.EqualTo("LVL-IMPORT-PROOF-GATE-MISSING"));
            Assert.That(result.IsRuntimeImportable, Is.False);
        }

        [Test]
        public void ADocumentWhoseProofHashDiffersFromTheMigrationDerivation_IsStillStructurallyValid()
        {
            // proofRef.proofHash is a reference value of the later proof block; this work package neither generates nor verifies a proof artifact.
            var other = new string('a', 64);
            var json = JsonEdit.Set(Doc(), "/proofRef/proofHash/sha256", JsonEdit.Str(other));
            var result = LevelV2Loader.Load(json);
            Assert.That(result.IsValid, Is.True, Codes.Describe(result.Diagnostics));
            Assert.That(result.IsRuntimeImportable, Is.False);
        }

        [Test]
        public void TheMappedDefinition_CarriesExactlyThePublicInputs()
        {
            var result = LevelV2Loader.Load(Doc());
            var definition = result.Definition!;
            Assert.That(definition.Grid.Width, Is.EqualTo(4));
            Assert.That(definition.Grid.Height, Is.EqualTo(4));
            Assert.That(definition.A.Side, Is.EqualTo(Direction.N));
            Assert.That(definition.A.Index, Is.EqualTo(0));
            Assert.That(definition.B.Side, Is.EqualTo(Direction.S));
            Assert.That(definition.B.Index, Is.EqualTo(3));
            Assert.That(definition.RowCounts, Is.EqualTo(new[] { 1, 3, 4, 1 }));
            Assert.That(definition.ColumnCounts, Is.EqualTo(new[] { 3, 1, 2, 3 }));
            Assert.That(definition.RulesetVersion, Is.EqualTo("train-track-v1"));
        }

        // ---- stage order: a later stage never masks an earlier failure --------------------------------------------

        [Test]
        public void ParseFailure_YieldsOnlyParseDiagnostics_AndNothingElse()
        {
            var result = LevelV2Loader.LoadText("{\"documentSchemaVersion\": 2,}");
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ImportState, Is.EqualTo(LevelImportState.Rejected));
            Assert.That(Stages(result), Is.EqualTo(new[] { LevelDiagnosticStage.Parse }));
            Assert.That(result.Document, Is.Null);
            Assert.That(result.Definition, Is.Null);
            AssertNoHashes(result);
        }

        [Test]
        public void SchemaFailure_StopsBeforeMapping_EvenWhenTheContentIsAlsoSemanticallyBroken()
        {
            var json = JsonEdit.Set(Doc(), "/solution/path/1/track", JsonEdit.Str("TRACK_EW"));
            json = JsonEdit.Add(json, "", "unexpected", JsonEdit.Int(1));
            var result = LevelV2Loader.Load(json);
            Assert.That(Stages(result), Is.EqualTo(new[] { LevelDiagnosticStage.Schema }));
            Assert.That(Codes.Of(result.Diagnostics), Is.EqualTo(new[] { LevelDiagnosticCodes.SchemaUnknownProperty }));
            Assert.That(result.Document, Is.Null);
            Assert.That(result.Definition, Is.Null);
            AssertNoHashes(result);
        }

        [Test]
        public void MappingFailure_KeepsTheDocument_ButStopsBeforeTheSemanticStage()
        {
            var json = JsonEdit.Set(Doc(), "/solution/path/1/track", JsonEdit.Str("TRACK_EW"));
            json = JsonEdit.Set(json, "/rowCounts", Parse("[1,3,4,2]"));
            var result = LevelV2Loader.Load(json);
            Assert.That(Stages(result), Is.EqualTo(new[] { LevelDiagnosticStage.DomainMap }));
            Assert.That(result.Document, Is.Not.Null);
            Assert.That(result.Definition, Is.Null);
            Assert.That(result.IsValid, Is.False);
            AssertNoHashes(result);
        }

        [Test]
        public void SemanticFailure_KeepsDocumentAndDefinition_AndReportsOnlySemanticDiagnostics()
        {
            var json = JsonEdit.Set(Doc(), "/solution/path/1/track", JsonEdit.Str("TRACK_EW"));
            var result = LevelV2Loader.Load(json);
            Assert.That(Stages(result), Is.EqualTo(new[] { LevelDiagnosticStage.Semantic }));
            Assert.That(result.Document, Is.Not.Null);
            Assert.That(result.Definition, Is.Not.Null);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ImportState, Is.EqualTo(LevelImportState.Rejected));
            AssertNoHashes(result);
        }

        [Test]
        public void ARejectedSource_ReportsItsDiagnosticsAsImportBlockers_AndIsNeverImportable()
        {
            foreach (var json in new[]
            {
                JsonEdit.Set(Doc(), "/solution/path/1/track", JsonEdit.Str("TRACK_EW")),
                JsonEdit.Set(Doc(), "/rowCounts", Parse("[1,3,4,2]")),
                JsonEdit.Remove(Doc(), "/proofRef"),
            })
            {
                var result = LevelV2Loader.Load(json);
                Assert.That(result.IsValid, Is.False);
                Assert.That(result.IsRuntimeImportable, Is.False);
                Assert.That(result.ImportState, Is.EqualTo(LevelImportState.Rejected));
                Assert.That(result.ImportBlockers, Is.EqualTo(result.Diagnostics));
                Assert.That(result.ImportBlockers, Is.Not.Empty);
            }
        }

        [Test]
        public void AMissingProofRef_IsASchemaError_NotAnImportGate()
        {
            var result = LevelV2Loader.Load(JsonEdit.Remove(Doc(), "/proofRef"));
            Assert.That(Codes.Of(result.Diagnostics), Is.EqualTo(new[] { LevelDiagnosticCodes.SchemaRequired }));
            Assert.That(result.Diagnostics[0].Path, Is.EqualTo("/proofRef"));
        }

        [Test]
        public void ALegacyV1Document_IsNotAValidV2Source()
        {
            var result = LevelV2Loader.Load(RepoFiles.Bytes(Examples.V1));
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Document, Is.Null);
            Assert.That(result.IsRuntimeImportable, Is.False);
        }

        [Test]
        public void UnknownDocumentVersion_IsExactlyOneVersionDiagnostic()
        {
            var result = LevelV2Loader.Load(JsonEdit.Set(Doc(), "/documentSchemaVersion", JsonEdit.Int(3)));
            Assert.That(Codes.Of(result.Diagnostics), Is.EqualTo(new[] { LevelDiagnosticCodes.VersionUnknown }));
        }

        // ---- entry points ----------------------------------------------------------------------------------------

        [Test]
        public void BytesTextAndJsonEntryPoints_AgreeOnValidAndInvalidInput()
        {
            string text = RepoFiles.Text(Examples.V2);
            AssertSame(LevelV2Loader.Load(TextMutation.Utf8(text)), LevelV2Loader.LoadText(text));
            AssertSame(LevelV2Loader.Load(TextMutation.Utf8(text)), LevelV2Loader.Load(Doc()));
            int at = text.IndexOf("\"TRACK_NS\"", StringComparison.Ordinal);
            string broken = text.Substring(0, at) + "\"TRACK_EW\"" + text.Substring(at + "\"TRACK_NS\"".Length);
            AssertSame(LevelV2Loader.Load(TextMutation.Utf8(broken)), LevelV2Loader.LoadText(broken));
        }

        [Test]
        public void NullAndEmptyInputs_AreParseOrSchemaDiagnostics_NotExceptions()
        {
            Assert.That(LevelV2Loader.Load((byte[]?)null).IsValid, Is.False);
            Assert.That(LevelV2Loader.Load(new byte[0]).Diagnostics.Select(d => d.Code).ToArray(), Is.EqualTo(new[] { LevelDiagnosticCodes.ParseEmpty }));
            Assert.That(LevelV2Loader.LoadText(null).IsValid, Is.False);
            Assert.That(LevelV2Loader.LoadText(string.Empty).Diagnostics.Select(d => d.Code).ToArray(), Is.EqualTo(new[] { LevelDiagnosticCodes.ParseEmpty }));
            Assert.That(LevelV2Loader.Load((JsonValue?)null).IsValid, Is.False);
            Assert.That(LevelV2Loader.LoadText("[]").Diagnostics.Select(d => d.Code).ToArray(), Is.EqualTo(new[] { LevelDiagnosticCodes.SchemaType }));
            Assert.That(LevelV2Loader.LoadText("null").IsValid, Is.False);
            Assert.That(LevelV2Loader.LoadText("1").IsValid, Is.False);
        }

        [Test]
        public void ADocumentOver256KiB_IsRejectedBeforeParsing()
        {
            var bytes = new byte[JsonParserLimits.Default.MaxBytes + 1];
            for (int i = 0; i < bytes.Length; i++) bytes[i] = (byte)' ';
            var result = LevelV2Loader.Load(bytes);
            Assert.That(Codes.Of(result.Diagnostics), Is.EqualTo(new[] { LevelDiagnosticCodes.ParseSize }));
        }

        [Test]
        public void CustomLimits_AreHonouredByTheLoader()
        {
            var tight = new JsonParserLimits(maxBytes: 64, maxDepth: 16, maxStringLength: 8192, maxArrayLength: 1024, maxObjectMembers: 64);
            var result = LevelV2Loader.Load(RepoFiles.Bytes(Examples.V2), tight);
            Assert.That(Codes.Of(result.Diagnostics), Is.EqualTo(new[] { LevelDiagnosticCodes.ParseSize }));
            Assert.That(LevelV2Loader.LoadText(RepoFiles.Text(Examples.V2), tight).IsValid, Is.False);
        }

        // ---- determinism -----------------------------------------------------------------------------------------

        [Test]
        public void LoadIsDeterministic_AndIndependentOfFormattingAndMemberOrder()
        {
            string pretty = RepoFiles.Text(Examples.V2);
            var baseline = LevelV2Loader.LoadText(pretty);
            var variants = new List<string>
            {
                Jcs(Doc()),
                pretty.Replace("\n", "\r\n"),
                pretty.Replace("  ", "\t"),
                "\n\n  " + pretty + "\n\n",
                Jcs(JsonValue.CreateObject(Doc().Members.Reverse().ToList())),
            };
            foreach (string variant in variants)
            {
                var result = LevelV2Loader.LoadText(variant);
                Assert.That(result.IsValid, Is.True, Codes.Describe(result.Diagnostics));
                Assert.That(result.ContentHash, Is.EqualTo(baseline.ContentHash));
                Assert.That(result.PublicPuzzleHash, Is.EqualTo(baseline.PublicPuzzleHash));
                Assert.That(result.SolutionHash, Is.EqualTo(baseline.SolutionHash));
            }
            for (int i = 0; i < 20; i++) Assert.That(LevelV2Loader.LoadText(pretty).ContentHash, Is.EqualTo(baseline.ContentHash));
        }

        [Test]
        public void DiagnosticsAreReportedInAStableOrder_AcrossRepeatedLoads()
        {
            var json = JsonEdit.Set(JsonEdit.Set(Doc(), "/solution/path/4/track", JsonEdit.Str("TRACK_EW")), "/solution/path/0/x", JsonEdit.Int(3));
            var first = LevelV2Loader.Load(json).Diagnostics.Select(d => d.ToString()).ToArray();
            for (int i = 0; i < 10; i++) Assert.That(LevelV2Loader.Load(json).Diagnostics.Select(d => d.ToString()).ToArray(), Is.EqualTo(first));
        }

        [Test]
        public void Culture_DoesNotInfluenceTheResult()
        {
            var original = System.Globalization.CultureInfo.CurrentCulture;
            try
            {
                string expected = LevelV2Loader.Load(Doc()).ContentHash!;
                foreach (string name in new[] { "de-DE", "tr-TR", "ar-SA", "th-TH" })
                {
                    System.Globalization.CultureInfo? culture = null;
                    try { culture = new System.Globalization.CultureInfo(name); }
                    catch (System.Globalization.CultureNotFoundException) { continue; } // culture data is runtime dependent; the contract does not depend on it
                    System.Globalization.CultureInfo.CurrentCulture = culture;
                    var result = LevelV2Loader.Load(Doc());
                    Assert.That(result.IsValid, Is.True, name);
                    Assert.That(result.ContentHash, Is.EqualTo(expected), name);
                    var broken = LevelV2Loader.Load(JsonEdit.Set(Doc(), "/solution/path/1/track", JsonEdit.Str("TRACK_EW")));
                    Assert.That(broken.Diagnostics.Select(d => d.ToString()).ToArray(), Is.EqualTo(LevelV2Loader.Load(JsonEdit.Set(Doc(), "/solution/path/1/track", JsonEdit.Str("TRACK_EW"))).Diagnostics.Select(d => d.ToString()).ToArray()), name);
                }
            }
            finally { System.Globalization.CultureInfo.CurrentCulture = original; }
        }

        // ---- robustness ------------------------------------------------------------------------------------------

        [Test]
        public void RandomBytes_NeverThrow_AndNeverProduceAValidDocument()
        {
            var random = new Random(99);
            for (int i = 0; i < 400; i++)
            {
                var bytes = new byte[random.Next(0, 600)];
                random.NextBytes(bytes);
                var result = LevelV2Loader.Load(bytes);
                Assert.That(result.IsValid, Is.False, "round " + i);
                Assert.That(result.IsRuntimeImportable, Is.False);
                Assert.That(result.Diagnostics, Is.Not.Empty);
            }
        }

        [Test]
        public void ByteLevelMutationsOfAValidDocument_NeverThrow_AndKeepTheResultInvariants()
        {
            byte[] original = RepoFiles.Bytes(Examples.V2);
            var random = new Random(2718);
            int rejected = 0;
            for (int i = 0; i < 1500; i++)
            {
                var mutated = (byte[])original.Clone();
                switch (random.Next(4))
                {
                    case 0: mutated[random.Next(mutated.Length)] = (byte)random.Next(256); break;
                    case 1: mutated = mutated.Take(random.Next(mutated.Length)).ToArray(); break;
                    case 2: { int at = random.Next(mutated.Length); mutated = mutated.Take(at).Concat(new[] { (byte)random.Next(256) }).Concat(mutated.Skip(at)).ToArray(); break; }
                    default: { int at = random.Next(mutated.Length); mutated = mutated.Take(at).Concat(mutated.Skip(at + 1)).ToArray(); break; }
                }
                var result = LevelV2Loader.Load(mutated);
                AssertInvariants(result, "round " + i);
                if (!result.IsValid) rejected++;
            }
            Assert.That(rejected, Is.GreaterThan(1000), "the large majority of random byte edits must be rejected");
        }

        [Test]
        public void StructuralMutationsOfAValidTree_NeverThrow_AndKeepTheResultInvariants()
        {
            var random = new Random(8128);
            var paths = new List<string>();
            Collect(Doc(), string.Empty, paths);
            var replacements = new Func<JsonValue>[]
            {
                () => JsonValue.Null, () => JsonValue.FromBoolean(true), () => JsonValue.CreateInteger(0), () => JsonValue.CreateInteger(-1), () => JsonValue.CreateInteger(JsonValue.MaxSafeInteger),
                () => JsonValue.CreateString(string.Empty), () => JsonValue.CreateString("x"), () => JsonValue.CreateArray(new JsonValue[0]), () => JsonValue.CreateObject(new JsonMember[0]),
            };
            for (int i = 0; i < 800; i++)
            {
                string path = paths[random.Next(paths.Count)];
                var json = random.Next(3) == 0 && path.Length > 0 ? JsonEdit.Remove(Doc(), path) : JsonEdit.Set(Doc(), path, replacements[random.Next(replacements.Length)]());
                AssertInvariants(LevelV2Loader.Load(json), "round " + i + " " + path);
            }
        }

        // ---- helpers ---------------------------------------------------------------------------------------------

        private static void Collect(JsonValue node, string path, List<string> paths)
        {
            if (path.Length > 0) paths.Add(path);
            if (node.Kind == JsonKind.Object) foreach (var member in node.Members) Collect(member.Value, path + "/" + member.Name, paths);
            else if (node.Kind == JsonKind.Array) for (int i = 0; i < node.Items.Count; i++) Collect(node.Items[i], path + "/" + i.ToString(System.Globalization.CultureInfo.InvariantCulture), paths);
        }

        private static void AssertNoHashes(LevelV2LoadResult result)
        {
            Assert.That(result.ContentHash, Is.Null);
            Assert.That(result.PublicPuzzleHash, Is.Null);
            Assert.That(result.SolutionHash, Is.Null);
            Assert.That(result.IsRuntimeImportable, Is.False);
        }

        private static void AssertInvariants(LevelV2LoadResult result, string context)
        {
            Assert.That(result.IsRuntimeImportable, Is.False, context);
            if (result.IsValid)
            {
                Assert.That(result.Diagnostics, Is.Empty, context);
                Assert.That(result.Document, Is.Not.Null, context);
                Assert.That(result.Definition, Is.Not.Null, context);
                Assert.That(result.ContentHash, Is.Not.Null, context);
                Assert.That(result.ImportState, Is.EqualTo(LevelImportState.AwaitingProofGate), context);
                Assert.That(result.ImportBlockers.Select(b => b.Code).ToArray(), Is.EqualTo(new[] { LevelDiagnosticCodes.ImportProofGateMissing }), context);
                // A document that is valid must reproduce itself: its own JSON loads valid with the same content hash.
                var again = LevelV2Loader.Load(result.Document!.ToJson());
                Assert.That(again.IsValid, Is.True, context);
                Assert.That(again.ContentHash, Is.EqualTo(result.ContentHash), context);
            }
            else
            {
                Assert.That(result.Diagnostics, Is.Not.Empty, context);
                Assert.That(result.ImportState, Is.EqualTo(LevelImportState.Rejected), context);
                Assert.That(result.ContentHash, Is.Null, context);
                Assert.That(result.PublicPuzzleHash, Is.Null, context);
                Assert.That(result.SolutionHash, Is.Null, context);
                Assert.That(result.ImportBlockers, Is.EqualTo(result.Diagnostics), context);
            }
            foreach (var diagnostic in result.Diagnostics)
            {
                Assert.That(diagnostic.Code, Is.Not.Empty, context);
                Assert.That(diagnostic.Message, Is.Not.Empty, context);
                Assert.That(diagnostic.Path, Is.Not.Null, context);
            }
        }

        private static void AssertSame(LevelV2LoadResult expected, LevelV2LoadResult actual)
        {
            Assert.That(actual.IsValid, Is.EqualTo(expected.IsValid));
            Assert.That(actual.ContentHash, Is.EqualTo(expected.ContentHash));
            Assert.That(actual.Diagnostics.Select(d => d.ToString()).ToArray(), Is.EqualTo(expected.Diagnostics.Select(d => d.ToString()).ToArray()));
        }
    }
}
