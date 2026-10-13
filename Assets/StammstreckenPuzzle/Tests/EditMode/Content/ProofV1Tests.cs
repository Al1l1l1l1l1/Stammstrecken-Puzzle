using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using STP.Editor.Content;
using STP.Infrastructure.Content;
using STP.Puzzle.Solver;

namespace STP.Tests.Content.EditMode
{
    /// <summary>Production-generated fixtures, fresh regeneration, complete binding and strict single/batch regressions for WP-025.</summary>
    public sealed class ProofV1Tests
    {
        private const string Prefix = "Assets/StammstreckenPuzzle/Tests/EditMode/Content/proof-v1-solver-v2-";
        private static readonly string[] Names = { "root", "single", "search" };
        private static byte[] Bytes(JsonValue value)
        {
            var result = JcsSerializer.Serialize(value);
            Assert.That(result.Succeeded, Is.True, result.Error?.ToString());
            return result.Utf8!;
        }
        private static JsonValue Obj(params (string Name, JsonValue Value)[] values) => JsonValue.CreateObject(values.Select(v => new JsonMember(v.Name, v.Value)).ToList());
        private static JsonValue Ints(params int[] values) => JsonValue.CreateArray(values.Select(v => JsonEdit.Int(v)).ToList());
        private static JsonValue Endpoint(string side, int index) => Obj(("side", JsonEdit.Str(side)), ("index", JsonEdit.Int(index)));

        private static JsonValue Source(string name)
        {
            if (name == "root") return Examples.Json(Examples.V2);
            if (name == "single") return Examples.Json(Examples.V2Single);
            // FIXTURE_ONLY unique 2x3 search reference. Authoring path is explicit, not used by the production solver.
            return WithPath(Examples.Json(Examples.V2Single), 2, 3, "N", 0, "N", 1,
                new[] { (0,0,"TRACK_NS"), (0,1,"TRACK_NS"), (0,2,"TRACK_NE"), (1,2,"TRACK_WN"), (1,1,"TRACK_NS"), (1,0,"TRACK_NS") });
        }

        private static JsonValue WithPath(JsonValue source, int w, int h, string a, int ai, string b, int bi, (int X, int Y, string Track)[] cells)
        {
            var rows = new int[h]; var columns = new int[w];
            foreach (var c in cells) { rows[c.Y]++; columns[c.X]++; }
            source = JsonEdit.Set(source, "/grid", Obj(("width", JsonEdit.Int(w)), ("height", JsonEdit.Int(h))));
            source = JsonEdit.Set(source, "/endpoints", Obj(("a", Endpoint(a, ai)), ("b", Endpoint(b, bi))));
            source = JsonEdit.Set(source, "/rowCounts", Ints(rows)); source = JsonEdit.Set(source, "/columnCounts", Ints(columns));
            return JsonEdit.Set(source, "/solution/path", JsonValue.CreateArray(cells.Select(c => Obj(("x", JsonEdit.Int(c.X)), ("y", JsonEdit.Int(c.Y)), ("track", JsonEdit.Str(c.Track)))).ToList()));
        }

        private static JsonValue Bound(JsonValue source, ProofV1Document proof) => JsonEdit.Set(source, "/proofRef",
            Obj(("artifactId", JsonEdit.Str(ProofV1Codec.ArtifactId(proof.PuzzleId, proof.SolverVersion))), ("proofFormatVersion", JsonEdit.Int(1)), ("proofHash", proof.ProofHash.ToJson())));
        private static JsonValue Rehash(JsonValue proof)
        {
            var projection = JsonValue.CreateObject(proof.Members.Where(m => m.Name != "proofHash").ToList());
            return JsonEdit.Set(proof, "/proofHash", LevelHashing.ComputeProofHash(projection).ToJson());
        }
        private static LevelValidationInput Input(JsonValue source, byte[]? proof, string path = "Content/Levels/fixture.json", IEnumerable<LevelDiagnostic>? diagnostics = null, string? artifact = null)
            => new LevelValidationInput(path, Bytes(source), artifact ?? source.Get("proofRef")!.Get("artifactId")!.StringValue, proof, diagnostics);
        private static JsonValue Golden(string name) => Examples.Json(Prefix + name + ".json");
        private static JsonValue GoldenLevel(string name) => Examples.Json(Prefix + name + ".level.json");
        private static void Rejected(LevelValidationResult result, string code)
        {
            Assert.That(result.Succeeded, Is.False, Codes.Describe(result.Diagnostics));
            Assert.That(Codes.Of(result.Diagnostics), Does.Contain(code));
            Assert.That(result.ProofGatePassed, Is.False); Assert.That(result.Proof, Is.Null);
            Assert.That(result.IsRuntimeImportable, Is.False); Assert.That(result.IsReleaseLockEligible, Is.False);
        }

        /// <summary>Explicit golden update command; production output is self-validated before any fixture is written.</summary>
        public static void RegenerateGoldens()
        {
            foreach (string name in Names)
            {
                var source = Source(name); var generated = ProofGenerator.Generate(Bytes(source));
                Assert.That(generated.Succeeded, Is.True, Codes.Describe(generated.Diagnostics));
                var proof = generated.Proof!;
                Assert.That(ProofV1Codec.Read(proof.CanonicalBytes).Succeeded, Is.True);
                File.WriteAllBytes(Path.Combine(RepoFiles.Root(), Prefix + name + ".json"), proof.CanonicalBytes!);
                File.WriteAllBytes(Path.Combine(RepoFiles.Root(), Prefix + name + ".level.json"), Bytes(Bound(source, proof.Document!)));
                Console.WriteLine("PRODUCTION_GOLDEN " + name + " " + proof.Document!.ProofHash + " metrics=" + Encoding.UTF8.GetString(Bytes(proof.Document.ToJson().Get("metrics")!)));
            }
        }

        [TestCase("root")][TestCase("single")][TestCase("search")]
        public void GeneratedGoldens_AreSelfValidAndRegenerateByteExactly(string name)
        {
            byte[] expected = RepoFiles.Bytes(Prefix + name + ".json");
            var stored = ProofV1Codec.Read(expected);
            Assert.That(stored.Succeeded, Is.True, Codes.Describe(stored.Diagnostics));
            Assert.That(stored.CanonicalBytes, Is.EqualTo(expected), "UTF8, no BOM and no trailing newline");
            for (int run = 0; run < 3; run++)
            {
                var fresh = ProofGenerator.Generate(RepoFiles.Bytes(Prefix + name + ".level.json"));
                Assert.That(fresh.Succeeded, Is.True, Codes.Describe(fresh.Diagnostics));
                Assert.That(fresh.Proof!.CanonicalBytes, Is.EqualTo(expected));
                Assert.That(fresh.Solver!.Metrics!.DeductionSteps, Is.EqualTo(stored.Document!.Metrics.DeductionSteps));
                Assert.That(fresh.Solver.DeductionTrace.Count, Is.EqualTo(stored.Document.Metrics.DeductionSteps));
            }
            if (name != "search")
            {
                Assert.That(stored.Document!.Metrics.SearchNodes, Is.EqualTo(0));
                Assert.That(LevelValidationPipeline.ValidateOne(Input(GoldenLevel(name), expected), true).Succeeded, Is.True);
            }
            else
            {
                Assert.That(stored.Document!.Metrics.SearchNodes, Is.GreaterThan(0));
                Assert.That(stored.Document.Metrics.RequiredGuessDepth, Is.GreaterThan(0));
                Rejected(LevelValidationPipeline.ValidateOne(Input(GoldenLevel(name), expected)), ProofDiagnosticCodes.GuessDepth);
            }
        }

        [Test]
        public void CanonicalProof_AcceptsFormattingAndMemberOrder_ButPreservesExactProjection()
        {
            var json = Golden("root");
            var reversed = JsonValue.CreateObject(json.Members.Reverse().ToList());
            var spaced = Encoding.UTF8.GetBytes(" \n" + JcsSerializer.Serialize(reversed).Text + "\n ");
            Assert.That(ProofV1Codec.Read(spaced).CanonicalBytes, Is.EqualTo(RepoFiles.Bytes(Prefix + "root.json")));
            Assert.That(LevelValidationPipeline.ValidateOne(Input(GoldenLevel("root"), spaced)).Succeeded, Is.True);
            var p = ProofV1Codec.Read(json).Document!;
            Assert.That(p.Projection().Get("proofHash"), Is.Null);
            Assert.That(p.Projection().Members.Count, Is.EqualTo(7));
            Assert.That(LevelHashing.ComputeProofHash(p.Projection()), Is.EqualTo(p.ProofHash));
        }

        [TestCase("searchNodes")][TestCase("deductionSteps")][TestCase("maxDeductionDepth")][TestCase("requiredGuessDepth")]
        public void EveryMetric_AllowsZero_AndRejectsNegativeFloatOrMissing(string metric)
        {
            var schema = Examples.Json("ARCHITECTURE/schemas/proof-v1.schema.json").Get("properties")!.Get("metrics")!.Get("properties")!.Get(metric)!;
            Assert.That(schema.Get("type")!.StringValue, Is.EqualTo("integer")); Assert.That(schema.Get("minimum")!.IntegerValue, Is.EqualTo(0));
            string path = "/metrics/" + metric;
            foreach (long value in new[] { 0L, 1L, JsonValue.MaxSafeInteger })
                Assert.That(ProofV1Codec.Read(Rehash(JsonEdit.Set(Golden("root"), path, JsonEdit.Int(value)))).Succeeded, Is.True, metric + "=" + value);
            Assert.That(ProofV1Codec.Read(Rehash(JsonEdit.Set(Golden("root"), path, JsonEdit.Int(-1)))).Succeeded, Is.False);
            Assert.That(ProofV1Codec.Read(JsonEdit.Remove(Golden("root"), path)).Succeeded, Is.False);
            var text = JcsSerializer.Serialize(Golden("root")).Text!;
            var old = Golden("root").Get("metrics")!.Get(metric)!.IntegerValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var floating = text.Replace("\"" + metric + "\":" + old, "\"" + metric + "\":0.0");
            Assert.That(Codes.Of(ProofV1Codec.Read(Encoding.UTF8.GetBytes(floating)).Diagnostics), Does.Contain("PRF-PARSE-FLOAT"));
        }

        [TestCase("searchNodes")][TestCase("deductionSteps")][TestCase("maxDeductionDepth")][TestCase("requiredGuessDepth")]
        public void SelfConsistentChangedMetric_IsStillNondeterministic(string metric)
        {
            var changed = Rehash(JsonEdit.Set(Golden("root"), "/metrics/" + metric, JsonEdit.Int(999)));
            var proof = ProofV1Codec.Read(changed); Assert.That(proof.Succeeded, Is.True);
            var level = Bound(GoldenLevel("root"), proof.Document!);
            Rejected(LevelValidationPipeline.ValidateOne(Input(level, Bytes(changed))), ProofDiagnosticCodes.Nondeterministic);
        }

        [TestCase("/puzzleId", "S1-01-01-02", ProofDiagnosticCodes.Puzzle)]
        [TestCase("/solverVersion", "solver-v1", ProofDiagnosticCodes.SolverVersion)]
        [TestCase("/solverVersion", "solver-v99", ProofDiagnosticCodes.SolverUnknown)]
        [TestCase("/solutionHash/sha256", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", ProofDiagnosticCodes.Solution)]
        [TestCase("/publicPuzzleHash/sha256", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", ProofDiagnosticCodes.Puzzle)]
        public void RehashedBindingMutations_FailClosedWithStableCodes(string path, string value, string code)
        {
            var proof = Rehash(JsonEdit.Set(Golden("root"), path, JsonEdit.Str(value)));
            Rejected(LevelValidationPipeline.ValidateOne(Input(GoldenLevel("root"), Bytes(proof))), code);
        }

        [TestCase("publicPuzzleHash")][TestCase("solutionHash")][TestCase("proofHash")]
        public void WrongSlotAndUnknownProfiles_AreRejected(string slot)
        {
            foreach (string profile in new[] { "UNKNOWN", LevelHashProfiles.LevelV1Puzzle, slot == "solutionHash" ? LevelHashProfiles.Proof : LevelHashProfiles.Solution })
                Rejected(LevelValidationPipeline.ValidateOne(Input(GoldenLevel("root"), Bytes(JsonEdit.Set(Golden("root"), "/" + slot + "/profile", JsonEdit.Str(profile))))), ProofDiagnosticCodes.Profile);
        }

        [Test]
        public void CrossPuzzleCopy_AndArtifactReferenceFailures_CannotReachImportOrRelease()
        {
            Rejected(LevelValidationPipeline.ValidateOne(Input(GoldenLevel("root"), RepoFiles.Bytes(Prefix + "single.json"))), ProofDiagnosticCodes.Puzzle);
            foreach (string artifact in new[] { "proofs/S1-01-01-01/solver-v1.proof-v1.json", "proofs/elsewhere.json", "../proofs/x.json" })
            {
                Rejected(LevelValidationPipeline.ValidateOne(Input(GoldenLevel("root"), RepoFiles.Bytes(Prefix + "root.json"), artifact: artifact)), ProofDiagnosticCodes.Artifact);
                var level = JsonEdit.Set(GoldenLevel("root"), "/proofRef/artifactId", JsonEdit.Str(artifact));
                Rejected(LevelValidationPipeline.ValidateOne(Input(level, RepoFiles.Bytes(Prefix + "root.json"))), ProofDiagnosticCodes.Artifact);
            }
            Rejected(LevelValidationPipeline.ValidateOne(Input(GoldenLevel("root"), null)), ProofDiagnosticCodes.Missing);
            // The existing level-semantic stage rejects this reference before proof processing, preserving its original LVL code.
            Rejected(LevelValidationPipeline.ValidateOne(Input(JsonEdit.Set(GoldenLevel("root"), "/proofRef/proofHash/profile", JsonEdit.Str("UNKNOWN")), RepoFiles.Bytes(Prefix + "root.json"))), LevelDiagnosticCodes.HashProfileUnknown);
            Rejected(LevelValidationPipeline.ValidateOne(Input(JsonEdit.Set(GoldenLevel("root"), "/proofRef/proofHash/sha256", JsonEdit.Str(new string('0',64))), RepoFiles.Bytes(Prefix + "root.json"))), ProofDiagnosticCodes.Hash);
            var valid = LevelValidationPipeline.ValidateOne(Input(GoldenLevel("root"), RepoFiles.Bytes(Prefix + "root.json")));
            Assert.That(valid.ProofGatePassed, Is.True); Assert.That(valid.IsRuntimeImportable, Is.False); Assert.That(valid.IsReleaseLockEligible, Is.False);
            Assert.That(Codes.Of(valid.ImportBlockers), Is.EqualTo(new[] { "LVL-IMPORT-RELEASE-GATE-MISSING" }));
        }

        [Test]
        public void MalformedMissingUnknownAndTamperedProofs_HaveDeterministicErrors()
        {
            foreach (string path in new[] { "", "/metrics", "/publicPuzzleHash", "/solutionHash", "/proofHash" })
            {
                var unknown = JsonEdit.Add(Golden("root"), path, "unexpected", JsonEdit.Int(0));
                Assert.That(Codes.Of(ProofV1Codec.Read(unknown).Diagnostics), Does.Contain("PRF-SCHEMA-UNKNOWN-PROPERTY"));
                var obj = JsonEdit.Resolve(Golden("root"), path);
                foreach (var member in obj.Members)
                    Assert.That(ProofV1Codec.Read(JsonEdit.Remove(Golden("root"), path + "/" + member.Name)).Succeeded, Is.False, path + "/" + member.Name);
            }
            Assert.That(Codes.Of(ProofV1Codec.Read(JsonEdit.Set(Golden("root"), "/proofFormatVersion", JsonEdit.Int(2))).Diagnostics), Does.Contain(ProofDiagnosticCodes.Format));
            Assert.That(Codes.Of(ProofV1Codec.Read(JsonEdit.Set(Golden("root"), "/solutionCount", JsonEdit.Int(2))).Diagnostics), Does.Contain(ProofDiagnosticCodes.NotUnique));
            Assert.That(Codes.Of(ProofV1Codec.Read(JsonEdit.Set(Golden("root"), "/metrics/searchNodes", JsonEdit.Int(1))).Diagnostics), Does.Contain(ProofDiagnosticCodes.Hash));
            foreach (string bad in new[] { "", "{}", "{", "{\"x\":1,\"x\":2}", "null", "[]", "1", "true", "// hi" })
                Assert.That(ProofV1Codec.Read(Encoding.UTF8.GetBytes(bad)).Succeeded, Is.False, bad);
            Assert.That(ProofV1Codec.Read(Encoding.UTF8.GetBytes("\uFEFF{}" )).Succeeded, Is.False);
            Assert.That(ProofV1Codec.Read(new byte[] { 0xff }).Succeeded, Is.False);
            Assert.That(ProofV1Codec.Read(Examples.Json(Examples.Proof)).Succeeded, Is.True, "Historical solver-v1 is only a reader contract.");
        }

        [Test]
        public void EarlierLevelFailures_StopBeforeAnyProofFallback()
        {
            var parsed = LevelValidationPipeline.ValidateOne(new LevelValidationInput("a.json", Encoding.UTF8.GetBytes("{"), "x", null));
            Assert.That(parsed.FailedStage, Is.EqualTo(LevelDiagnosticStage.Parse));
            Assert.That(parsed.Diagnostics.All(d => d.Stage == LevelDiagnosticStage.Parse), Is.True);
            var schema = LevelValidationPipeline.ValidateOne(Input(JsonEdit.Remove(GoldenLevel("root"), "/grid"), null));
            Assert.That(schema.FailedStage, Is.EqualTo(LevelDiagnosticStage.Schema));
            Assert.That(schema.Diagnostics.All(d => d.Stage == LevelDiagnosticStage.Schema), Is.True);
            var bad = JsonEdit.Set(GoldenLevel("single"), "/rowCounts", Ints(2,0));
            var rejected = LevelValidationPipeline.ValidateOne(Input(bad, null));
            Assert.That(rejected.Succeeded, Is.False); Assert.That(rejected.Diagnostics.All(d => d.Stage < LevelDiagnosticStage.Solver), Is.True);
        }

        [Test]
        public void FreshRegenerationAlwaysRuns_MissingCacheCannotHideCancellationOrBudgetFailure()
        {
            foreach (var limits in new[] { new SolverLimits(maxStates:0), new SolverLimits(timeBudgetMs:0), new SolverLimits(cancellation:new CancellationToken(true)) })
            {
                var result = LevelValidationPipeline.ValidateOne(Input(GoldenLevel("root"), null), limits:limits);
                Rejected(result, ProofDiagnosticCodes.Indeterminate);
                Assert.That(Codes.Of(result.Diagnostics), Does.Not.Contain(ProofDiagnosticCodes.Missing));
            }
        }

        [Test]
        public void MultiplePublicSolutions_AreNeverProofEligible_EvenWithAValidAuthoringPath()
        {
            var level = WithPath(Source("single"), 3,3,"N",0,"N",2,new[] {
                (0,0,"TRACK_NS"),(0,1,"TRACK_NS"),(0,2,"TRACK_NE"),(1,2,"TRACK_EW"),(2,2,"TRACK_WN"),
                (2,1,"TRACK_SW"),(1,1,"TRACK_NE"),(1,0,"TRACK_ES"),(2,0,"TRACK_WN") });
            Assert.That(LevelV2Loader.Load(level).IsValid, Is.True);
            var result = ProofGenerator.Generate(Bytes(level)); Assert.That(result.Succeeded, Is.False);
            Assert.That(Codes.Of(result.Diagnostics), Does.Contain(ProofDiagnosticCodes.NotUnique));
        }

        [Test]
        public void StrictCommand_PromotesEachWarningWithoutChangingCodeStageOrMessage_AndBatchAgrees()
        {
            var warning = new LevelDiagnostic(LevelDiagnosticStage.Import, "TEST-AUTHORING-WARNING", "/fixture", "Explicit test notice", LevelDiagnosticSeverity.Warning);
            var input = Input(GoldenLevel("root"), RepoFiles.Bytes(Prefix+"root.json"), diagnostics:new[] { warning });
            var normal = LevelValidationPipeline.ValidateOne(input); Assert.That(normal.Succeeded, Is.True);
            Assert.That(normal.WarningCount, Is.EqualTo(1)); Assert.That(normal.ErrorCount, Is.EqualTo(0)); Assert.That(normal.FailedStage, Is.Null);
            var strict = LevelValidationPipeline.ValidateOne(input, true); Rejected(strict, warning.Code);
            Assert.That(strict.ErrorCount, Is.EqualTo(1)); Assert.That(strict.WarningCount, Is.EqualTo(0)); Assert.That(strict.FailedStage, Is.EqualTo(warning.Stage));
            var diagnostic = strict.Diagnostics.Single(); Assert.That(diagnostic.Code, Is.EqualTo(warning.Code)); Assert.That(diagnostic.Path, Is.EqualTo(warning.Path)); Assert.That(diagnostic.Message, Is.EqualTo(warning.Message));
            foreach (bool mode in new[] { false,true })
            {
                var batch = LevelValidationCommand.ValidateBatch(new[] { input }, mode ? new[] { "--strict" } : Array.Empty<string>());
                var one = mode ? strict : normal;
                Assert.That(batch.Succeeded, Is.EqualTo(one.Succeeded)); Assert.That(batch.FailedCount, Is.EqualTo(mode ? 1 : 0));
                Assert.That(batch.ErrorCount, Is.EqualTo(one.ErrorCount)); Assert.That(batch.WarningCount, Is.EqualTo(one.WarningCount)); Assert.That(batch.FailedStage, Is.EqualTo(one.FailedStage));
                Assert.That(batch.Results[0].Diagnostics, Is.EqualTo(one.Diagnostics));
            }
            Assert.Throws<ArgumentException>(() => LevelValidationCommand.ValidateBatch(new[] { input }, new[] { "--ignore-warnings" }));
            Assert.That(warning.Severity, Is.EqualTo(LevelDiagnosticSeverity.Warning), "strict never mutates the source notice");
        }

        [Test]
        public void DiagnosticsAndBatchOrdering_AreStableAndImmutable()
        {
            var warnings = new[] { new LevelDiagnostic(LevelDiagnosticStage.Import,"Z","/z","z",LevelDiagnosticSeverity.Warning),
                new LevelDiagnostic(LevelDiagnosticStage.Import,"A","/a","a",LevelDiagnosticSeverity.Warning) };
            var a = Input(GoldenLevel("root"), RepoFiles.Bytes(Prefix+"root.json"), ".\\Content//a.json", warnings);
            var b = Input(GoldenLevel("single"), RepoFiles.Bytes(Prefix+"single.json"), "Content/b.json", warnings.Reverse());
            foreach (bool strict in new[] { false,true })
            {
                var forward = LevelValidationPipeline.ValidateBatch(new[] { a,b }, strict);
                var reverse = LevelValidationPipeline.ValidateBatch(new[] { b,a }, strict);
                Assert.That(forward.Results.Select(r => r.SourcePath), Is.EqualTo(new[] { "Content/a.json","Content/b.json" }));
                Assert.That(forward.Results.Select(r => Codes.Describe(r.Diagnostics)), Is.EqualTo(reverse.Results.Select(r => Codes.Describe(r.Diagnostics))));
                Assert.That(forward.ErrorCount, Is.EqualTo(reverse.ErrorCount)); Assert.That(forward.WarningCount, Is.EqualTo(reverse.WarningCount));
                Assert.Throws<NotSupportedException>(() => ((IList<LevelValidationResult>)forward.Results).Clear());
                Assert.Throws<NotSupportedException>(() => ((IList<LevelDiagnostic>)forward.Results[0].Diagnostics).Clear());
            }
            var proof = ProofV1Codec.Read(Golden("root")); var before = proof.CanonicalBytes!; proof.CanonicalBytes![0] = 0;
            Assert.That(proof.CanonicalBytes, Is.EqualTo(before));
            Assert.Throws<NotSupportedException>(() => ((IList<LevelDiagnostic>)proof.Diagnostics).Clear());
            Rejected(LevelValidationPipeline.ValidateBatch(new[] { a,a }).Results[0], "LVL-SOURCE-DUPLICATE");
        }

        [TestCase(LevelDiagnosticStage.Parse)][TestCase(LevelDiagnosticStage.Schema)][TestCase(LevelDiagnosticStage.DomainMap)]
        [TestCase(LevelDiagnosticStage.Semantic)][TestCase(LevelDiagnosticStage.Hash)][TestCase(LevelDiagnosticStage.Migration)]
        [TestCase(LevelDiagnosticStage.Solver)][TestCase(LevelDiagnosticStage.Proof)][TestCase(LevelDiagnosticStage.Import)]
        public void StrictPromotesWarningsFromEveryStage_InEachResultAndReport(LevelDiagnosticStage stage)
        {
            var warning = new LevelDiagnostic(stage,"TEST-STAGE-WARNING","/test","stage warning",LevelDiagnosticSeverity.Warning);
            var input = Input(GoldenLevel("root"), RepoFiles.Bytes(Prefix+"root.json"), diagnostics:new[] { warning });
            var normal = LevelValidationPipeline.ValidateOne(input); Assert.That(normal.Succeeded, Is.True);
            var strict = LevelValidationCommand.ValidateBatch(new[] { input }, new[] { "--strict" });
            Assert.That(strict.Succeeded, Is.False); Assert.That(strict.ErrorCount, Is.EqualTo(1)); Assert.That(strict.WarningCount, Is.EqualTo(0));
            Assert.That(strict.Results[0].FailedStage, Is.EqualTo(stage)); Assert.That(strict.Results[0].Diagnostics.Single().Code, Is.EqualTo(warning.Code));
        }

        [Test]
        public void MixedBatch_UsesEachActualResultStatus_AndInputSnapshotsCannotBeMutated()
        {
            byte[] levelBytes = Bytes(GoldenLevel("root")), proofBytes = RepoFiles.Bytes(Prefix+"root.json");
            var warning = new LevelDiagnostic(LevelDiagnosticStage.Import,"TEST-WARNING","/test","warning",LevelDiagnosticSeverity.Warning);
            var notices = new List<LevelDiagnostic> { warning };
            var good = new LevelValidationInput("a.json",levelBytes,GoldenLevel("root").Get("proofRef")!.Get("artifactId")!.StringValue,proofBytes,notices);
            levelBytes[0]=0; proofBytes[0]=0; notices.Clear(); good.LevelBytes![0]=0; good.ProofBytes![0]=0;
            Assert.That(LevelValidationPipeline.ValidateOne(good).Succeeded, Is.True);
            var bad = new LevelValidationInput("b.json",Encoding.UTF8.GetBytes("{"),"x",null);
            foreach(bool strict in new[] { false,true })
            {
                var batch = LevelValidationPipeline.ValidateBatch(new[] { bad,good },strict);
                Assert.That(batch.FailedCount, Is.EqualTo(strict ? 2 : 1)); Assert.That(batch.ErrorCount, Is.EqualTo(strict ? 2 : 1));
                Assert.That(batch.WarningCount, Is.EqualTo(strict ? 0 : 1)); Assert.That(batch.FailedStage, Is.EqualTo(LevelDiagnosticStage.Parse));
            }
        }

        [Test]
        public void InvalidDiagnosticSeverity_CannotBypassStrictPromotion()
        {
            foreach (int invalid in new[] { -1, 2, int.MaxValue })
                Assert.Throws<ArgumentOutOfRangeException>(() => new LevelDiagnostic(LevelDiagnosticStage.Import,
                    "TEST-INVALID-SEVERITY", "/test", "invalid severity", (LevelDiagnosticSeverity)invalid));
        }
    }
}
