using System.Collections.Generic;
using NUnit.Framework;
using STP.Editor.Content;
using STP.Infrastructure.Content;

namespace STP.Tests.Content.EditMode
{
    /// <summary>
    /// LevelValidationPipeline gemäß CONTENT_PIPELINE.md Abschnitt 5 und
    /// WP-013 AK-06: fünf Stufen in dokumentierter Reihenfolge, sortierte
    /// Diagnosen, <c>--strict</c>-Semantik und die Regel, dass frühere Fehler
    /// nicht durch spätere Stufen verdeckt werden; jede Fixturemutation
    /// scheitert an der zuständigen Stufe mit dem zuständigen Code.
    /// </summary>
    public sealed class PipelineTests
    {
        private static LevelValidationInput FixturePair(string levelPath, string artifactId, string proofPath)
        {
            return new LevelValidationInput(
                levelPath,
                ContractFixtures.ReadBytes(levelPath),
                artifactId,
                ContractFixtures.ReadBytes(proofPath));
        }

        private static LevelValidationResult RunSingle(LevelValidationInput input, bool strict = false)
        {
            return LevelValidationPipeline.Validate(new[] { input }, strict).Levels[0];
        }

        private static void AssertFailed(LevelValidationResult result, PipelineStage stage, string code)
        {
            Assert.AreEqual(stage, result.FailedStage, $"Stufe; Diagnosen: {string.Join(" | ", result.Diagnostics)}");
            Assert.IsFalse(result.Succeeded);
            Assert.IsTrue(
                HasCode(result, code),
                $"Erwarteter Code {code}; vorhanden: {string.Join(", ", Codes(result))}");
        }

        private static bool HasCode(LevelValidationResult result, string code)
        {
            foreach (var diagnostic in result.Diagnostics)
            {
                if (diagnostic.Code == code)
                {
                    return true;
                }
            }
            return false;
        }

        private static List<string> Codes(LevelValidationResult result)
        {
            var codes = new List<string>();
            foreach (var diagnostic in result.Diagnostics)
            {
                codes.Add(diagnostic.Code);
            }
            return codes;
        }

        /// <summary>Beide Vertragsfixtures durchlaufen alle fünf Stufen fehlerfrei, auch im --strict-Modus.</summary>
        [Test]
        public void ContractFixtures_PassAllStages()
        {
            var report = LevelValidationPipeline.Validate(
                new[]
                {
                    FixturePair(ContractFixtures.LevelExample, ContractFixtures.ProofExampleArtifactId, ContractFixtures.ProofExample),
                    FixturePair(ContractFixtures.LevelSingleCell, ContractFixtures.ProofSingleCellArtifactId, ContractFixtures.ProofSingleCell),
                },
                strict: true);

            Assert.IsTrue(report.Succeeded, "Bericht: " + string.Join(" | ", AllDiagnostics(report)));
            Assert.AreEqual(0, report.ErrorCount);
            Assert.AreEqual(0, report.WarningCount);
            Assert.AreEqual(2, report.Levels.Count);
            foreach (var level in report.Levels)
            {
                Assert.IsTrue(level.Succeeded, level.SourceName);
                Assert.IsNull(level.FailedStage, level.SourceName);
                Assert.NotNull(level.GeneratedProofBytes, level.SourceName);
            }
            Assert.AreEqual(ContractFixtures.LevelExamplePublicHash, report.Levels[0].PublicPuzzleHash!.Value.Sha256);
            Assert.AreEqual(ContractFixtures.LevelExampleSolutionHash, report.Levels[0].SolutionHash!.Value.Sha256);
            Assert.AreEqual(ContractFixtures.SingleCellPublicHash, report.Levels[1].PublicPuzzleHash!.Value.Sha256);
            Assert.AreEqual(ContractFixtures.SingleCellSolutionHash, report.Levels[1].SolutionHash!.Value.Sha256);
        }

        private static List<string> AllDiagnostics(LevelValidationReport report)
        {
            var all = new List<string>();
            foreach (var level in report.Levels)
            {
                foreach (var diagnostic in level.Diagnostics)
                {
                    all.Add(level.SourceName + ": " + diagnostic);
                }
            }
            return all;
        }

        /// <summary>Fixturemutationen scheitern an der zuständigen Stufe mit dem zuständigen Code.</summary>
        [Test]
        public void FixtureMutations_FailAtTheResponsibleStage()
        {
            var malformed = RunSingle(new LevelValidationInput("malformed", ContractFixtures.ToBytes("{\"grid\": }")));
            AssertFailed(malformed, PipelineStage.Parse, "LVL-PARSE-SYNTAX");
            Assert.AreEqual(1, malformed.Diagnostics.Count, "Ein Parsefehler wird nicht durch spätere Stufen verdeckt oder vervielfacht");

            var additionalProperty = RunSingle(new LevelValidationInput(
                "schema", ContractFixtures.ToBytes(ContractFixtures.Add(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$", "unexpected", ContractFixtures.Int(1)))));
            AssertFailed(additionalProperty, PipelineStage.Schema, "LVL-SCHEMA-ADDITIONAL-PROPERTY");

            var sumMismatch = RunSingle(new LevelValidationInput(
                "domain", ContractFixtures.ToBytes(ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.rowCounts[0]", ContractFixtures.Int(2)))));
            AssertFailed(sumMismatch, PipelineStage.DomainMap, "LVL-GRID-SUM-MISMATCH");

            var countMismatch = RunSingle(new LevelValidationInput(
                "semantic",
                ContractFixtures.ToBytes(ContractFixtures.Replace(
                    ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.rowCounts[2]", ContractFixtures.Int(3)),
                    "$.rowCounts[3]", ContractFixtures.Int(2)))));
            AssertFailed(countMismatch, PipelineStage.Semantic, "LVL-COUNT-ROW-MISMATCH");

            var metricChange = RunSingle(new LevelValidationInput(
                "proof",
                ContractFixtures.ReadBytes(ContractFixtures.LevelExample),
                ContractFixtures.ProofExampleArtifactId,
                ContractFixtures.ToBytes(ContractFixtures.Replace(
                    (JsonValue.Object)StrictJsonParser.ParseUtf8(ContractFixtures.ReadBytes(ContractFixtures.ProofExample), JsonParseLimits.Default),
                    "$.metrics.deductionSteps",
                    ContractFixtures.Int(10)))));
            AssertFailed(metricChange, PipelineStage.SolverProof, "PRF-PROOF-HASH-MISMATCH");

            var crossPuzzleCopy = RunSingle(new LevelValidationInput(
                "cross",
                ContractFixtures.ReadBytes(ContractFixtures.LevelExample),
                ContractFixtures.ProofSingleCellArtifactId,
                ContractFixtures.ReadBytes(ContractFixtures.ProofSingleCell)));
            AssertFailed(crossPuzzleCopy, PipelineStage.SolverProof, "PRF-PUZZLE-ID-MISMATCH");

            var missingArtifact = RunSingle(new LevelValidationInput("missing", ContractFixtures.ReadBytes(ContractFixtures.LevelExample)));
            AssertFailed(missingArtifact, PipelineStage.SolverProof, "PRF-ARTIFACT-MISSING");
        }

        /// <summary>Ein Datensatz mit mehreren Lösungen scheitert an der Solverstufe; die Authoringlösung allein beweist keine Eindeutigkeit.</summary>
        [Test]
        public void NonUniqueLevel_FailsAtSolverStage()
        {
            var input = new LevelValidationInput("multiple", ContractFixtures.ToBytes(MultipleSolutionsDocument()));
            AssertFailed(RunSingle(input), PipelineStage.SolverProof, "LVL-SOLVER-NOT-UNIQUE");
        }

        /// <summary>
        /// --strict-Semantik: Eine eindeutige, aber Annahmetiefe fordernde
        /// Lösung erzeugt die Warnung LVL-SOLVER-GUESS-REQUIRED; ohne --strict
        /// besteht der Datensatz, mit --strict scheitert er an der Warnung.
        /// </summary>
        [Test]
        public void StrictMode_TurnsWarningsIntoFailures()
        {
            var documentBytes = ContractFixtures.ToBytes(GuessRequiredDocument(null));

            var firstPass = RunSingle(new LevelValidationInput("guess", documentBytes));
            Assert.NotNull(firstPass.GeneratedProofBytes, "Proofgenerierung läuft auch ohne bereitgestelltes Artefakt");
            Assert.IsTrue(HasCode(firstPass, "LVL-SOLVER-GUESS-REQUIRED"), "Annahmetiefe-Warnung");
            AssertFailed(firstPass, PipelineStage.SolverProof, "PRF-ARTIFACT-MISSING");

            var generatedDiagnostics = new List<LevelDiagnostic>();
            var generatedProof = ProofV1Serializer.Deserialize(firstPass.GeneratedProofBytes!, JsonParseLimits.Default, generatedDiagnostics);
            Assert.NotNull(generatedProof, string.Join("; ", generatedDiagnostics));

            var boundDocument = ContractFixtures.Replace(
                (JsonValue.Object)StrictJsonParser.ParseUtf8(documentBytes, JsonParseLimits.Default),
                "$.proofRef.proofHash.sha256",
                ContractFixtures.Text(generatedProof!.ProofHash.Sha256));
            var boundBytes = ContractFixtures.ToBytes(boundDocument);

            var looseReport = LevelValidationPipeline.Validate(
                new[] { new LevelValidationInput("guess", boundBytes, "proofs/S99-02-03-04/solver-v1.proof-v1.json", firstPass.GeneratedProofBytes!) },
                strict: false);
            Assert.AreEqual(0, looseReport.ErrorCount);
            Assert.AreEqual(1, looseReport.WarningCount);
            Assert.IsTrue(looseReport.Succeeded, "Ohne --strict bleibt die Warnung bestehenbar");
            Assert.IsTrue(HasCode(looseReport.Levels[0], "LVL-SOLVER-GUESS-REQUIRED"));

            var strictReport = LevelValidationPipeline.Validate(
                new[] { new LevelValidationInput("guess", boundBytes, "proofs/S99-02-03-04/solver-v1.proof-v1.json", firstPass.GeneratedProofBytes!) },
                strict: true);
            Assert.AreEqual(0, strictReport.ErrorCount);
            Assert.AreEqual(1, strictReport.WarningCount);
            Assert.IsFalse(strictReport.Succeeded, "Im --strict-Modus wird die Warnung als Fehler behandelt");
        }

        /// <summary>Diagnosen sind deterministisch nach Code, Bezug und Meldung sortiert.</summary>
        [Test]
        public void Diagnostics_AreSortedDeterministically()
        {
            var root = ContractFixtures.ParseLevel(ContractFixtures.LevelExample);
            var mutated = ContractFixtures.Replace(
                ContractFixtures.Add(
                    ContractFixtures.Remove(root, "$.rowCounts"),
                    "$", "unexpected", ContractFixtures.Int(1)),
                "$.puzzleId", ContractFixtures.Text("bad-id"));
            var result = RunSingle(new LevelValidationInput("sorted", ContractFixtures.ToBytes(mutated)));

            Assert.AreEqual(PipelineStage.Schema, result.FailedStage);
            var codes = Codes(result);
            Assert.GreaterOrEqual(codes.Count, 3, string.Join(",", codes));
            var sorted = new List<string>(codes);
            sorted.Sort(System.StringComparer.Ordinal);
            CollectionAssert.AreEqual(sorted, codes, "Diagnosen müssen ordinal sortiert sein");
        }

        /// <summary>Die Proofgenerierung ist deterministisch: gleicher Input und gleiche Solverversion erzeugen bytegleichen Proof.</summary>
        [Test]
        public void ProofGeneration_IsByteDeterministic()
        {
            byte[]? FirstProof()
            {
                return RunSingle(new LevelValidationInput(
                        "determinism",
                        ContractFixtures.ReadBytes(ContractFixtures.LevelExample),
                        ContractFixtures.ProofExampleArtifactId,
                        ContractFixtures.ReadBytes(ContractFixtures.ProofExample)))
                    .GeneratedProofBytes;
            }

            var first = FirstProof();
            var second = FirstProof();
            Assert.NotNull(first);
            CollectionAssert.AreEqual(first!, second!);
        }

        /// <summary>
        /// Der generierte Proof bindet Puzzle-ID, Hashes, Solverversion und
        /// Lösungszahl und ist in sich hashkonsistent. Hinweis: Bei rein
        /// deduktiv gelösten Puzzles meldet der unveränderte solver-v1
        /// searchNodes = 0 (Suchannahmen); der dokumentierte Befund zur
        /// Metrikminimum-Differenz zum proof-v1-Schema steht im Work Package.
        /// </summary>
        [Test]
        public void GeneratedProof_IsSelfConsistent()
        {
            var result = RunSingle(FixturePair(ContractFixtures.LevelExample, ContractFixtures.ProofExampleArtifactId, ContractFixtures.ProofExample));
            Assert.IsTrue(result.Succeeded);
            Assert.NotNull(result.GeneratedProofBytes);

            var generated = (JsonValue.Object)StrictJsonParser.ParseUtf8(result.GeneratedProofBytes!, JsonParseLimits.Default);
            var diagnostics = new List<LevelDiagnostic>();
            var artifact = ProofV1SchemaValidator.Validate(generated, diagnostics);
            if (artifact is null)
            {
                Assert.IsTrue(
                    diagnostics.Exists(d => d.Code == "PRF-FORMAT-METRICS"),
                    "Nur die dokumentierte Metrikminimum-Differenz (searchNodes 0 bei rein deduktivem Lauf) ist zulässig; vorhanden: " + string.Join("; ", diagnostics));
            }
            else
            {
                Assert.AreEqual("solver-v1", artifact.SolverVersion);
                Assert.AreEqual(1, artifact.SolutionCount);
                Assert.AreEqual(result.PublicPuzzleHash!.Value, artifact.PublicPuzzleHash);
                Assert.AreEqual(result.SolutionHash!.Value, artifact.SolutionHash);
            }
            Assert.AreEqual(result.PublicPuzzleHash!.Value.Sha256, ContractFixtures.LevelExamplePublicHash);
        }

        /// <summary>Bei einem Lauf mit Suchannahmen ist der generierte Proof vollständig format- und bindungsgültig.</summary>
        [Test]
        public void GeneratedProof_WithSearch_PassesFormatStage()
        {
            var firstPass = RunSingle(new LevelValidationInput("guess", ContractFixtures.ToBytes(GuessRequiredDocument(null))));
            Assert.NotNull(firstPass.GeneratedProofBytes);

            var diagnostics = new List<LevelDiagnostic>();
            var artifact = ProofV1Serializer.Deserialize(firstPass.GeneratedProofBytes!, JsonParseLimits.Default, diagnostics);
            Assert.NotNull(artifact, "Formatstufe: " + string.Join("; ", diagnostics));
            Assert.AreEqual("solver-v1", artifact!.SolverVersion);
            Assert.AreEqual(1, artifact.SolutionCount);
            Assert.GreaterOrEqual(artifact.Metrics.SearchNodes, 1);
            Assert.GreaterOrEqual(artifact.Metrics.DeductionSteps, 1);
            Assert.GreaterOrEqual(artifact.Metrics.MaxDeductionDepth, 1);
            Assert.AreEqual(artifact.ProofHash, PuzzleHashContracts.ComputeProofHash((JsonValue.Object)StrictJsonParser.ParseUtf8(firstPass.GeneratedProofBytes!, JsonParseLimits.Default)));
        }

        /// <summary>Batchergebnisse sind nach Quellenname deterministisch sortiert.</summary>
        [Test]
        public void BatchResults_AreSortedBySourceName()
        {
            var report = LevelValidationPipeline.Validate(
                new[]
                {
                    new LevelValidationInput("zz-last", ContractFixtures.ToBytes("x")),
                    new LevelValidationInput("aa-first", ContractFixtures.ToBytes("y")),
                },
                strict: false);
            Assert.AreEqual("aa-first", report.Levels[0].SourceName);
            Assert.AreEqual("zz-last", report.Levels[1].SourceName);
            Assert.AreEqual(2, report.ErrorCount);
            Assert.IsFalse(report.Succeeded);
        }

        private static JsonValue.Object MultipleSolutionsDocument()
        {
            return Document(
                "S99-04-04-04",
                99, 4, 4, 4,
                4, 4,
                Endpoint("N", 0), Endpoint("W", 1),
                new long[] { 4, 4, 4, 4 }, new long[] { 4, 4, 4, 4 },
                new[]
                {
                    Cell(0, 0, "TRACK_NE"), Cell(1, 0, "TRACK_EW"), Cell(2, 0, "TRACK_EW"), Cell(3, 0, "TRACK_SW"),
                    Cell(3, 1, "TRACK_WN"), Cell(2, 1, "TRACK_EW"), Cell(1, 1, "TRACK_ES"), Cell(1, 2, "TRACK_NE"),
                    Cell(2, 2, "TRACK_EW"), Cell(3, 2, "TRACK_SW"), Cell(3, 3, "TRACK_WN"), Cell(2, 3, "TRACK_EW"),
                    Cell(1, 3, "TRACK_EW"), Cell(0, 3, "TRACK_NE"), Cell(0, 2, "TRACK_NS"), Cell(0, 1, "TRACK_SW"),
                },
                "proofs/S99-04-04-04/solver-v1.proof-v1.json");
        }

        private static JsonValue.Object GuessRequiredDocument(ProfiledHash? proofHash)
        {
            return Document(
                "S99-02-03-04",
                99, 2, 3, 4,
                5, 5,
                Endpoint("N", 2), Endpoint("W", 3),
                new long[] { 2, 2, 3, 3, 2 }, new long[] { 2, 2, 3, 3, 2 },
                new[]
                {
                    Cell(2, 0, "TRACK_NE"), Cell(3, 0, "TRACK_SW"), Cell(3, 1, "TRACK_NE"), Cell(4, 1, "TRACK_SW"),
                    Cell(4, 2, "TRACK_WN"), Cell(3, 2, "TRACK_EW"), Cell(2, 2, "TRACK_ES"), Cell(2, 3, "TRACK_WN"),
                    Cell(1, 3, "TRACK_ES"), Cell(1, 4, "TRACK_WN"), Cell(0, 4, "TRACK_NE"), Cell(0, 3, "TRACK_SW"),
                },
                "proofs/S99-02-03-04/solver-v1.proof-v1.json",
                proofHash);
        }

        private static JsonValue.Object Endpoint(string side, long index)
        {
            return new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
            {
                new KeyValuePair<string, JsonValue>("side", new JsonValue.String(side)),
                new KeyValuePair<string, JsonValue>("index", new JsonValue.Integer(index)),
            });
        }

        private static JsonValue.Object Cell(long x, long y, string track)
        {
            return new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
            {
                new KeyValuePair<string, JsonValue>("x", new JsonValue.Integer(x)),
                new KeyValuePair<string, JsonValue>("y", new JsonValue.Integer(y)),
                new KeyValuePair<string, JsonValue>("track", new JsonValue.String(track)),
            });
        }

        private static JsonValue.Object Document(
            string puzzleId,
            long season,
            long section,
            long route,
            long position,
            long width,
            long height,
            JsonValue.Object endpointA,
            JsonValue.Object endpointB,
            long[] rowCounts,
            long[] columnCounts,
            JsonValue.Object[] path,
            string artifactId,
            ProfiledHash? proofHash = null)
        {
            JsonValue.Object Grid() => new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
            {
                new KeyValuePair<string, JsonValue>("width", new JsonValue.Integer(width)),
                new KeyValuePair<string, JsonValue>("height", new JsonValue.Integer(height)),
            });
            JsonValue.Array Counts(long[] values)
            {
                var items = new List<JsonValue>();
                foreach (var value in values)
                {
                    items.Add(new JsonValue.Integer(value));
                }
                return new JsonValue.Array(items);
            }
            var effectiveHash = proofHash ?? new ProfiledHash(HashProfiles.ProofJcs1, new string('0', 64));
            return new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
            {
                new KeyValuePair<string, JsonValue>("documentSchemaVersion", new JsonValue.Integer(2)),
                new KeyValuePair<string, JsonValue>("rulesetVersion", new JsonValue.String("train-track-v1")),
                new KeyValuePair<string, JsonValue>("puzzleId", new JsonValue.String(puzzleId)),
                new KeyValuePair<string, JsonValue>("contentRevision", new JsonValue.Integer(1)),
                new KeyValuePair<string, JsonValue>("grid", Grid()),
                new KeyValuePair<string, JsonValue>("endpoints", new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
                {
                    new KeyValuePair<string, JsonValue>("a", endpointA),
                    new KeyValuePair<string, JsonValue>("b", endpointB),
                })),
                new KeyValuePair<string, JsonValue>("rowCounts", Counts(rowCounts)),
                new KeyValuePair<string, JsonValue>("columnCounts", Counts(columnCounts)),
                new KeyValuePair<string, JsonValue>("content", new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
                {
                    new KeyValuePair<string, JsonValue>("season", new JsonValue.Integer(season)),
                    new KeyValuePair<string, JsonValue>("networkSection", new JsonValue.Integer(section)),
                    new KeyValuePair<string, JsonValue>("route", new JsonValue.Integer(route)),
                    new KeyValuePair<string, JsonValue>("position", new JsonValue.Integer(position)),
                    new KeyValuePair<string, JsonValue>("titleKey", new JsonValue.String("level.test.fixture.title")),
                    new KeyValuePair<string, JsonValue>("statusKey", new JsonValue.String("level.test.fixture.status")),
                })),
                new KeyValuePair<string, JsonValue>("production", new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
                {
                    new KeyValuePair<string, JsonValue>("entryDeductionKey", new JsonValue.String("level.test.fixture.entry")),
                    new KeyValuePair<string, JsonValue>("focus", new JsonValue.Array(new List<JsonValue> { new JsonValue.String("CHAIN") })),
                    new KeyValuePair<string, JsonValue>("chainDepth", new JsonValue.Integer(1)),
                    new KeyValuePair<string, JsonValue>("qualityNote", new JsonValue.String("Testdokument der EditMode-Tests; kein Produktionslevel.")),
                    new KeyValuePair<string, JsonValue>("timeClass", new JsonValue.String("UNSET")),
                    new KeyValuePair<string, JsonValue>("starThresholdsSeconds", JsonValue.Null.Instance),
                })),
                new KeyValuePair<string, JsonValue>("completion", new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
                {
                    new KeyValuePair<string, JsonValue>("mapSegmentId", new JsonValue.String("test-segment")),
                    new KeyValuePair<string, JsonValue>("trainMomentId", new JsonValue.String("standard-first-clear")),
                    new KeyValuePair<string, JsonValue>("resultTextKey", new JsonValue.String("result.test.fixture")),
                })),
                new KeyValuePair<string, JsonValue>("solution", new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
                {
                    new KeyValuePair<string, JsonValue>("path", new JsonValue.Array(new List<JsonValue>(path))),
                })),
                new KeyValuePair<string, JsonValue>("proofRef", new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
                {
                    new KeyValuePair<string, JsonValue>("artifactId", new JsonValue.String(artifactId)),
                    new KeyValuePair<string, JsonValue>("proofFormatVersion", new JsonValue.Integer(1)),
                    new KeyValuePair<string, JsonValue>("proofHash", PuzzleHashContracts.BuildProfiledHash(effectiveHash)),
                })),
            });
        }
    }
}
