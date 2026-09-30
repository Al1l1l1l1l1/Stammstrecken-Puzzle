using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using STP.Infrastructure.Content;

namespace STP.Tests.Content.EditMode
{
    /// <summary>
    /// JCS-Hashverträge gemäß LEVEL_DATA_FORMAT.md Abschnitt 6 und WP-013
    /// AK-04: RFC-8785-Kanonisierung (UTF-8 ohne BOM, ohne Abschlussnewline,
    /// I-JSON-Regeln), Known-Answer-Vektoren gegen die dokumentierten
    /// Fixture-Hashes, Crosscheck gegen das unabhängige CI-Werkzeug
    /// tools/architecture-validation/jcs_crosscheck.mjs sowie Semantik-
    /// invarianz und -änderung des Puzzlehashs.
    /// </summary>
    public sealed class HashContractTests
    {
        /// <summary>Die drei Projektionen reproduzieren exakt die dokumentierten Fixture-Hashes.</summary>
        [Test]
        public void KnownAnswerVectors_MatchDocumentedFixtureHashes()
        {
            var level = ContractFixtures.ParseLevel(ContractFixtures.LevelExample);
            var publicHash = PuzzleHashContracts.ComputePublicPuzzleHash(level);
            Assert.AreEqual(HashProfiles.PuzzleSemanticJcs1, publicHash.Profile);
            Assert.AreEqual(ContractFixtures.LevelExamplePublicHash, publicHash.Sha256);

            level.TryGet("puzzleId", out var puzzleId);
            level.TryGet("solution", out var solution);
            ((JsonValue.Object)solution).TryGet("path", out var path);
            var solutionHash = PuzzleHashContracts.ComputeSolutionHash(puzzleId, (JsonValue.Array)path, publicHash);
            Assert.AreEqual(HashProfiles.SolutionJcs1, solutionHash.Profile);
            Assert.AreEqual(ContractFixtures.LevelExampleSolutionHash, solutionHash.Sha256);

            var proof = (JsonValue.Object)StrictJsonParser.ParseUtf8(ContractFixtures.ReadBytes(ContractFixtures.ProofExample), JsonParseLimits.Default);
            var proofHash = PuzzleHashContracts.ComputeProofHash(proof);
            Assert.AreEqual(HashProfiles.ProofJcs1, proofHash.Profile);
            Assert.AreEqual(ContractFixtures.LevelExampleProofHash, proofHash.Sha256);

            var singleCell = ContractFixtures.ParseLevel(ContractFixtures.LevelSingleCell);
            var singlePublic = PuzzleHashContracts.ComputePublicPuzzleHash(singleCell);
            Assert.AreEqual(ContractFixtures.SingleCellPublicHash, singlePublic.Sha256);
            singleCell.TryGet("puzzleId", out var singlePuzzleId);
            singleCell.TryGet("solution", out var singleSolution);
            ((JsonValue.Object)singleSolution).TryGet("path", out var singlePath);
            Assert.AreEqual(
                ContractFixtures.SingleCellSolutionHash,
                PuzzleHashContracts.ComputeSolutionHash(singlePuzzleId, (JsonValue.Array)singlePath, singlePublic).Sha256);
            var singleProof = (JsonValue.Object)StrictJsonParser.ParseUtf8(ContractFixtures.ReadBytes(ContractFixtures.ProofSingleCell), JsonParseLimits.Default);
            Assert.AreEqual(ContractFixtures.SingleCellProofHash, PuzzleHashContracts.ComputeProofHash(singleProof).Sha256);
        }

        /// <summary>Kanonische Form: UTF-8 ohne BOM und ohne Abschlussnewline.</summary>
        [Test]
        public void CanonicalForm_HasNoBomAndNoTrailingNewline()
        {
            var bytes = JcsCanonicalizer.ToUtf8Bytes(ContractFixtures.ParseLevel(ContractFixtures.LevelExample));
            Assert.IsTrue(bytes.Length > 3);
            Assert.IsFalse(bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF);
            Assert.AreNotEqual((byte)'\n', bytes[bytes.Length - 1]);
            Assert.AreNotEqual((byte)'\r', bytes[bytes.Length - 1]);
        }

        /// <summary>Semantikneutrale Änderungen ändern den semantischen Puzzlehash nicht.</summary>
        [Test]
        public void SemanticHash_InvariantToNeutralChanges()
        {
            var root = ContractFixtures.ParseLevel(ContractFixtures.LevelExample);
            var expected = PuzzleHashContracts.ComputePublicPuzzleHash(root).Sha256;

            var reformatted = (JsonValue.Object)StrictJsonParser.ParseUtf8(JcsCanonicalizer.ToUtf8Bytes(root), JsonParseLimits.Default);
            Assert.AreEqual(expected, PuzzleHashContracts.ComputePublicPuzzleHash(reformatted).Sha256, "Kanonische Neuserialisierung");

            var shuffled = ContractFixtures.ReverseRootKeys(root);
            Assert.AreEqual(expected, PuzzleHashContracts.ComputePublicPuzzleHash(shuffled).Sha256, "Schlüsselreihenfolge");

            var schemaVersion = ContractFixtures.Replace(root, "$.documentSchemaVersion", ContractFixtures.Int(2));
            Assert.AreEqual(expected, PuzzleHashContracts.ComputePublicPuzzleHash(schemaVersion).Sha256, "documentSchemaVersion");

            var revision = ContractFixtures.Replace(root, "$.contentRevision", ContractFixtures.Int(99));
            Assert.AreEqual(expected, PuzzleHashContracts.ComputePublicPuzzleHash(revision).Sha256, "contentRevision");

            var text = ContractFixtures.Replace(root, "$.content.titleKey", ContractFixtures.Text("level.other.title"));
            Assert.AreEqual(expected, PuzzleHashContracts.ComputePublicPuzzleHash(text).Sha256, "Texte");

            var note = ContractFixtures.Replace(root, "$.production.qualityNote", ContractFixtures.Text("Geänderte Notiz."));
            Assert.AreEqual(expected, PuzzleHashContracts.ComputePublicPuzzleHash(note).Sha256, "Produktionsnotiz");

            var proofRef = ContractFixtures.Replace(root, "$.proofRef.proofHash.sha256", ContractFixtures.Text(new string('0', 64)));
            Assert.AreEqual(expected, PuzzleHashContracts.ComputePublicPuzzleHash(proofRef).Sha256, "Proofregeneration");
        }

        /// <summary>Änderungen an Puzzle-ID, Ruleset, Raster, Endpoints oder Counts ändern den Hash.</summary>
        [Test]
        public void SemanticHash_ChangesOnSemanticFields()
        {
            var root = ContractFixtures.ParseLevel(ContractFixtures.LevelExample);
            var expected = PuzzleHashContracts.ComputePublicPuzzleHash(root).Sha256;

            Assert.AreNotEqual(expected, PuzzleHashContracts.ComputePublicPuzzleHash(
                ContractFixtures.Replace(root, "$.puzzleId", ContractFixtures.Text("S1-01-01-02"))).Sha256, "puzzleId");
            Assert.AreNotEqual(expected, PuzzleHashContracts.ComputePublicPuzzleHash(
                ContractFixtures.Replace(root, "$.rulesetVersion", ContractFixtures.Text("train-track-v2"))).Sha256, "rulesetVersion");
            Assert.AreNotEqual(expected, PuzzleHashContracts.ComputePublicPuzzleHash(
                ContractFixtures.Replace(root, "$.grid.width", ContractFixtures.Int(5))).Sha256, "grid");
            Assert.AreNotEqual(expected, PuzzleHashContracts.ComputePublicPuzzleHash(
                ContractFixtures.Replace(root, "$.endpoints.a.index", ContractFixtures.Int(1))).Sha256, "endpoints");
            Assert.AreNotEqual(expected, PuzzleHashContracts.ComputePublicPuzzleHash(
                ContractFixtures.Replace(root, "$.rowCounts[0]", ContractFixtures.Int(2))).Sha256, "rowCounts");
            Assert.AreNotEqual(expected, PuzzleHashContracts.ComputePublicPuzzleHash(
                ContractFixtures.Replace(root, "$.columnCounts[0]", ContractFixtures.Int(2))).Sha256, "columnCounts");
        }

        /// <summary>
        /// Live-Crosscheck: die C#-Kanonisierung und Hashes stimmen mit dem
        /// unabhängigen CI-Werkzeug jcs_crosscheck.mjs (Node.js) überein.
        /// Wird ohne erreichbaren Node-Interpreter als Inconclusive gemeldet
        /// und niemals simuliert.
        /// </summary>
        [Test]
        public void Crosscheck_NodeToolProducesIdenticalResults()
        {
            var node = FindOnPath("node") ?? FindOnPath("node.exe");
            var script = Path.Combine(ContractFixtures.ProjectRoot, "tools/architecture-validation/jcs_crosscheck.mjs");
            if (node is null || !File.Exists(script))
            {
                Assert.Inconclusive("Node.js oder jcs_crosscheck.mjs ist in dieser Umgebung nicht verfügbar.");
                return;
            }

            var projections = new Dictionary<string, JsonValue>
            {
                ["public-standard"] = PuzzleHashContracts.BuildPublicPuzzleProjection(ContractFixtures.ParseLevel(ContractFixtures.LevelExample)),
                ["public-single-cell"] = PuzzleHashContracts.BuildPublicPuzzleProjection(ContractFixtures.ParseLevel(ContractFixtures.LevelSingleCell)),
            };
            var level = ContractFixtures.ParseLevel(ContractFixtures.LevelExample);
            level.TryGet("puzzleId", out var puzzleId);
            level.TryGet("solution", out var solution);
            ((JsonValue.Object)solution).TryGet("path", out var path);
            var publicHash = PuzzleHashContracts.ComputePublicPuzzleHash(level);
            projections["solution-standard"] = PuzzleHashContracts.BuildSolutionProjection(puzzleId, (JsonValue.Array)path, publicHash);
            var proof = (JsonValue.Object)StrictJsonParser.ParseUtf8(ContractFixtures.ReadBytes(ContractFixtures.ProofExample), JsonParseLimits.Default);
            projections["proof-standard"] = PuzzleHashContracts.BuildProofProjection(proof);

            var tempDir = Path.Combine(Path.GetTempPath(), "stp-jcs-crosscheck");
            Directory.CreateDirectory(tempDir);
            foreach (var pair in projections)
            {
                var bytes = JcsCanonicalizer.ToUtf8Bytes(pair.Value);
                var file = Path.Combine(tempDir, pair.Key + ".json");
                File.WriteAllBytes(file, bytes);

                using var process = new Process();
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = node,
                    Arguments = "\"" + script + "\" \"" + file + "\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                process.Start();
                var stdout = process.StandardOutput.ReadToEnd();
                var stderr = process.StandardError.ReadToEnd();
                process.WaitForExit(30000);
                Assert.AreEqual(0, process.ExitCode, $"jcs_crosscheck fehlgeschlagen: {stderr}");

                var tool = (JsonValue.Object)StrictJsonParser.ParseUtf8(
                    new System.Text.UTF8Encoding(false).GetBytes(stdout), JsonParseLimits.Default);
                tool.TryGet("sha256", out var toolHash);
                tool.TryGet("canonicalBase64", out var toolBase64);
                var csharpHash = HashProfiles.Sha256Hex(bytes);
                Assert.AreEqual(csharpHash, ((JsonValue.String)toolHash).Value, $"SHA-256 weicht für {pair.Key} ab");
                Assert.AreEqual(
                    System.Convert.ToBase64String(bytes),
                    ((JsonValue.String)toolBase64).Value,
                    $"Kanonische Bytes weichen für {pair.Key} ab");
            }
        }

        private static string? FindOnPath(string executable)
        {
            var path = System.Environment.GetEnvironmentVariable("PATH");
            if (path is null)
            {
                return null;
            }
            foreach (var directory in path.Split(Path.PathSeparator))
            {
                if (directory.Length == 0)
                {
                    continue;
                }
                var candidate = Path.Combine(directory, executable);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
            return null;
        }
    }
}
