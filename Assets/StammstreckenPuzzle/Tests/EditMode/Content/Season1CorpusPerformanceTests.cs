using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Editor.Content;
using STP.Infrastructure.Content;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Content.EditMode
{
    /// <summary>ADR-032: actual checkout inputs, fresh proofgate only; no content/import/release approval.</summary>
    public sealed class Season1CorpusPerformanceTests
    {
        private const int Warmups = 5, SamplesPerInput = 50, Workers = 4;
        private static readonly string[] TenByTenIds = Enumerable.Range(6, 7)
            .Select(n => "S1-05-04-" + n.ToString("D2", CultureInfo.InvariantCulture)).ToArray();

        private sealed class Entry
        {
            internal readonly string Id, LevelPath, ProofPath, ArtifactId;
            internal readonly byte[] LevelBytes, ProofBytes;
            internal readonly PuzzleDefinition Definition;
            internal Entry(string id, string levelPath, string proofPath, string artifactId,
                byte[] levelBytes, byte[] proofBytes, PuzzleDefinition definition)
            {
                Id = id; LevelPath = levelPath; ProofPath = proofPath; ArtifactId = artifactId;
                LevelBytes = levelBytes; ProofBytes = proofBytes; Definition = definition;
            }
        }

        [Test]
        public void CanonicalSeason1_FreshStrictProofGate_AndReferenceBudgets()
        {
            // Unity's project path selects this checkout, never STP_REPO_ROOT, fixtures or a scratch fallback.
            string root = Directory.GetParent(UnityEngine.Application.dataPath)!.FullName;
            string? workspace = Environment.GetEnvironmentVariable("GITHUB_WORKSPACE");
            if (!string.IsNullOrEmpty(workspace))
                Assert.That(Path.GetFullPath(root), Is.EqualTo(Path.GetFullPath(workspace!)));
            string commit = CheckoutCommit(root);
            Entry[] corpus = LoadCorpus(root);
            string fingerprint = Fingerprint(corpus);
            Assert.That(corpus.Where(e => e.Definition.Grid.Width == 10 && e.Definition.Grid.Height == 10)
                .Select(e => e.Id).ToArray(), Is.EqualTo(TenByTenIds), "exactly the seven actual 10x10 inputs");

            var samples = new List<double>(350);
            var measurements = new List<JsonValue>();
            foreach (string id in TenByTenIds)
            {
                Entry entry = corpus.Single(e => e.Id == id);
                for (int i = 0; i < Warmups; i++) RequireDeductive(PuzzleSolver.Solve(entry.Definition), id);
                var perInput = new List<double>(SamplesPerInput);
                SolverResult? last = null;
                for (int i = 0; i < SamplesPerInput; i++)
                {
                    // Time the complete production Solve, including root trace and result construction.
                    var watch = Stopwatch.StartNew();
                    last = PuzzleSolver.Solve(entry.Definition);
                    watch.Stop();
                    perInput.Add(watch.Elapsed.TotalMilliseconds);
                    RequireDeductive(last, id + " sample " + i);
                }
                samples.AddRange(perInput);
                measurements.Add(Obj(("id", Str(id)), ("sampleMs", JsonValue.CreateArray(perInput.Select(Ms))),
                    ("searchNodes", Int(last!.Metrics!.SearchNodes)), ("requiredGuessDepth", Int(last.Metrics.RequiredGuessDepth)),
                    ("deductionSteps", Int(last.Metrics.DeductionSteps)), ("maxDeductionDepth", Int(last.Metrics.MaxDeductionDepth))));
            }

            // Include fresh file reads, parsing, independent snapshots and deterministic evaluation in wall-clock time.
            var total = Stopwatch.StartNew();
            Entry[] fresh = LoadCorpus(root);
            Assert.That(Fingerprint(fresh), Is.EqualTo(fingerprint), "checkout inputs changed during measurement");
            var results = new LevelValidationResult?[fresh.Length];
            var errors = new string?[fresh.Length];
            Parallel.For(0, Workers, new ParallelOptions { MaxDegreeOfParallelism = Workers }, worker =>
            {
                // Four fixed partitions; each entry owns its copied production input. No shared solver state.
                for (int index = worker; index < fresh.Length; index += Workers)
                {
                    Entry entry = fresh[index];
                    try
                    {
                        var input = new LevelValidationInput(entry.LevelPath, entry.LevelBytes, entry.ArtifactId, entry.ProofBytes);
                        results[index] = LevelValidationPipeline.ValidateOne(input, strict: true);
                    }
                    catch (Exception error) { errors[index] = error.GetType().FullName + ": " + error.Message; }
                }
            });
            var failures = new List<string>();
            // LoadCorpus is ordinal by relative path; worker completion order cannot affect evaluation/output.
            for (int i = 0; i < fresh.Length; i++)
            {
                var result = results[i];
                if (errors[i] != null) failures.Add(fresh[i].LevelPath + ": " + errors[i]);
                else if (result == null) failures.Add(fresh[i].LevelPath + ": missing result");
                else if (!result.Strict || !result.Succeeded || !result.ProofGatePassed || result.ErrorCount != 0 || result.WarningCount != 0)
                    failures.Add(fresh[i].LevelPath + ": " + Codes.Describe(result.Diagnostics));
                else if (result.IsRuntimeImportable || result.IsReleaseLockEligible)
                    failures.Add(fresh[i].LevelPath + ": unexpected import/release eligibility");
            }
            total.Stop();
            double[] ordered = samples.OrderBy(ms => ms).ToArray();
            int p95Index = (int)Math.Ceiling(0.95 * ordered.Length) - 1;
            double p95 = ordered[p95Index], maximum = ordered[ordered.Length - 1];
            var evidence = Obj(("protocolVersion", Str("ADR-032-v1")), ("commit", Str(commit)),
                ("solverVersion", Str(PuzzleSolver.SolverVersion)), ("proofFormatVersion", Int(1)),
                ("corpusCount", Int(fresh.Length)), ("corpusFingerprintSha256", Str(fingerprint)),
                ("fingerprintEncoding", Str("ordinal paths; UTF8 path and raw bytes each prefixed by UInt64 big-endian length")),
                ("tenByTenCount", Int(TenByTenIds.Length)), ("tenByTenIds", JsonValue.CreateArray(TenByTenIds.Select(Str))),
                ("warmupsPerInput", Int(Warmups)), ("samplesPerInput", Int(SamplesPerInput)), ("sampleCount", Int(ordered.Length)),
                ("parallelWorkers", Int(Workers)), ("p95Index", Int(p95Index)), ("p95Ms", Ms(p95)), ("maximumMs", Ms(maximum)),
                ("corpusWallClockMs", Ms(total.Elapsed.TotalMilliseconds)), ("p95BudgetMsExclusive", Int(250)),
                ("maximumBudgetMsExclusive", Int(2000)), ("corpusBudgetMsExclusive", Int(60000)),
                ("proofGatePassedCount", Int(results.Count(r => r != null && r.ProofGatePassed))),
                ("measurements", JsonValue.CreateArray(measurements)), ("failures", JsonValue.CreateArray(failures.Select(Str))),
                ("approvalBoundary", Str("technical proofgate only; cross-reference/product/import/release gates remain closed")),
                ("unityVersion", Str(UnityEngine.Application.unityVersion)), ("runtime", Str(System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription)),
                ("os", Str(System.Runtime.InteropServices.RuntimeInformation.OSDescription)),
                ("processor", Str(UnityEngine.SystemInfo.processorType)), ("processorCount", Int(Environment.ProcessorCount)),
                ("processArchitecture", Str(System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString())));
            var encoded = JcsSerializer.Serialize(evidence);
            Assert.That(encoded.Succeeded, Is.True, encoded.Error?.ToString());
            TestContext.Out.WriteLine("WP028_CORPUS_PERFORMANCE " + encoded.Text);
            Assert.That(failures, Is.Empty, string.Join("\n", failures));
            Assert.That(ordered.Length, Is.EqualTo(350));
            Assert.That(p95, Is.LessThan(250.0), "ADR-032 p95 at ceil(0.95*n)-1");
            Assert.That(maximum, Is.LessThan(2000.0), "ADR-032 largest individual sample");
            Assert.That(total.Elapsed.TotalMilliseconds, Is.LessThan(60000.0), "ADR-032 complete fresh strict corpus");
        }

        private static Entry[] LoadCorpus(string root)
        {
            string directory = Path.Combine(root, "Content", "Levels", "s1");
            Assert.That(Directory.Exists(directory), Is.True, "canonical Season-1 directory missing");
            string[] paths = Directory.GetFiles(directory, "*.level-v2.json", SearchOption.AllDirectories)
                .Select(p => "Content/Levels/s1/" + p.Substring(directory.Length + 1).Replace('\\', '/'))
                .OrderBy(p => p, StringComparer.Ordinal).ToArray();
            Assert.That(paths.Length, Is.EqualTo(240), "no partial corpus");
            var entries = new List<Entry>(240);
            var expected = new HashSet<string>(StringComparer.Ordinal);
            for (int section = 1; section <= 5; section++)
                for (int route = 1; route <= 4; route++)
                    for (int position = 1; position <= 12; position++)
                        expected.Add(string.Format(CultureInfo.InvariantCulture, "S1-{0:D2}-{1:D2}-{2:D2}", section, route, position));
            foreach (string path in paths)
            {
                byte[] level = File.ReadAllBytes(Path.Combine(root, path));
                var loaded = LevelV2Loader.Load(level);
                Assert.That(loaded.IsValid, Is.True, path + ": " + Codes.Describe(loaded.Diagnostics));
                var doc = loaded.Document!;
                Assert.That(expected.Remove(doc.PuzzleId), Is.True, "duplicate/unexpected ID: " + doc.PuzzleId);
                string[] parts = doc.PuzzleId.Split('-');
                Assert.That(path, Is.EqualTo("Content/Levels/s1/" + parts[1] + "/" + parts[2] + "/" + doc.PuzzleId + ".level-v2.json"));
                string artifact = ProofV1Codec.ArtifactId(doc.PuzzleId, PuzzleSolver.SolverVersion);
                Assert.That(doc.ProofRef.ArtifactId, Is.EqualTo(artifact), path + ": solver-v2 binding required");
                Assert.That(doc.ProofRef.ProofFormatVersion, Is.EqualTo(1));
                // Logical artifact IDs are never opened as filesystem paths; only this exact canonical mapping is allowed.
                string proofPath = "Content/Proofs/" + doc.PuzzleId + "/solver-v2.proof-v1.json";
                byte[] proof = File.ReadAllBytes(Path.Combine(root, proofPath));
                var read = ProofV1Codec.Read(proof);
                Assert.That(read.Succeeded, Is.True, proofPath + ": " + Codes.Describe(read.Diagnostics));
                Assert.That(read.Document!.SolverVersion, Is.EqualTo(PuzzleSolver.SolverVersion));
                Assert.That(ProofV1Codec.Bind(loaded, artifact, read), Is.Empty, proofPath + ": proofRef binding");
                entries.Add(new Entry(doc.PuzzleId, path, proofPath, artifact, level, proof, loaded.Definition!));
            }
            Assert.That(expected, Is.Empty, "missing Season-1 IDs");
            return entries.ToArray();
        }

        private static void RequireDeductive(SolverResult result, string context)
        {
            Assert.That(result.SolverVersion, Is.EqualTo("solver-v2"), context);
            Assert.That(result.Classification, Is.EqualTo(SolverClassification.UNIQUE), context);
            Assert.That(result.SolutionCount, Is.EqualTo(1), context);
            Assert.That(result.Metrics, Is.Not.Null, context);
            Assert.That(result.Metrics!.SearchNodes, Is.EqualTo(0), context);
            Assert.That(result.Metrics.RequiredGuessDepth, Is.EqualTo(0), context);
        }

        private static string Fingerprint(IEnumerable<Entry> corpus)
        {
            var files = corpus.SelectMany(e => new[] { (Path: e.LevelPath, Bytes: e.LevelBytes), (Path: e.ProofPath, Bytes: e.ProofBytes) })
                .OrderBy(f => f.Path, StringComparer.Ordinal);
            using (var stream = new MemoryStream())
            {
                foreach (var file in files)
                {
                    byte[] path = new UTF8Encoding(false, true).GetBytes(file.Path);
                    WriteLength(stream, path.Length); stream.Write(path, 0, path.Length);
                    WriteLength(stream, file.Bytes.Length); stream.Write(file.Bytes, 0, file.Bytes.Length);
                }
                return LevelHashing.Sha256Hex(stream.ToArray());
            }
        }

        private static void WriteLength(Stream stream, int length)
        {
            for (int shift = 56; shift >= 0; shift -= 8) stream.WriteByte((byte)((ulong)length >> shift));
        }

        private static string CheckoutCommit(string root)
        {
            var start = new ProcessStartInfo("git", "rev-parse HEAD")
            { WorkingDirectory = root, UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
            using (var process = Process.Start(start)!)
            {
                string commit = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();
                Assert.That(process.ExitCode, Is.EqualTo(0), "cannot resolve checkout commit");
                Assert.That(commit, Does.Match("^[0-9a-f]{40}$"));
                string? githubSha = Environment.GetEnvironmentVariable("GITHUB_SHA");
                if (!string.IsNullOrEmpty(githubSha)) Assert.That(commit, Is.EqualTo(githubSha), "CI checkout/SHA mismatch");
                return commit;
            }
        }

        private static JsonValue Obj(params (string Name, JsonValue Value)[] fields) =>
            JsonValue.CreateObject(fields.Select(f => new JsonMember(f.Name, f.Value)));
        private static JsonValue Str(string value) => JsonValue.CreateString(value);
        private static JsonValue Int(long value) => JsonValue.CreateInteger(value);
        // Invariant round-trip strings retain the full measured precision in this integer-only JCS DOM.
        private static JsonValue Ms(double value) => Str(value.ToString("R", CultureInfo.InvariantCulture));
    }
}
