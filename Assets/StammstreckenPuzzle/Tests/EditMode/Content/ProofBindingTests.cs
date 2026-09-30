using System.Collections.Generic;
using NUnit.Framework;
using STP.Infrastructure.Content;

namespace STP.Tests.Content.EditMode
{
    /// <summary>
    /// proof-v1-Bindungen gemäß LEVEL_DATA_FORMAT.md Abschnitt 7 und WP-013
    /// AK-05: Artefaktformat, Puzzle-/Lösungsbindung (Cross-Puzzle-Copy,
    /// stale Lösungshash), Lösung genau eins, Proofhash (Metrikänderung)
    /// sowie die <c>proofRef</c>-Bindung an Artefakt-ID, Proofformat und
    /// Proofhash.
    /// </summary>
    public sealed class ProofBindingTests
    {
        private sealed class BindingContext
        {
            internal LevelV2Document Level = null!;
            internal ProofV1Document Proof = null!;
            internal ProfiledHash PublicHash;
            internal ProfiledHash SolutionHash;
        }

        private static BindingContext Load(string levelPath, string proofPath)
        {
            var levelDiagnostics = new List<LevelDiagnostic>();
            var level = LevelV2SchemaValidator.Validate(ContractFixtures.ParseLevel(levelPath), levelDiagnostics);
            Assert.NotNull(level, "Level-Schemafehler: " + string.Join("; ", levelDiagnostics));

            var proof = Reload((JsonValue.Object)StrictJsonParser.ParseUtf8(ContractFixtures.ReadBytes(proofPath), JsonParseLimits.Default));

            var publicHash = PuzzleHashContracts.ComputePublicPuzzleHash(level!.Source);
            level.Source.TryGet("puzzleId", out var puzzleId);
            level.Source.TryGet("solution", out var solution);
            ((JsonValue.Object)solution).TryGet("path", out var path);
            var solutionHash = PuzzleHashContracts.ComputeSolutionHash(puzzleId, (JsonValue.Array)path, publicHash);
            return new BindingContext
            {
                Level = level,
                Proof = proof,
                PublicHash = publicHash,
                SolutionHash = solutionHash,
            };
        }

        /// <summary>Deserialisiert eine Artefaktquelle erneut (DTO-Felder stammen aus der Quelle).</summary>
        private static ProofV1Document Reload(JsonValue.Object source)
        {
            var diagnostics = new List<LevelDiagnostic>();
            var proof = ProofV1SchemaValidator.Validate(source, diagnostics);
            Assert.NotNull(proof, "Proof-Formatfehler: " + string.Join("; ", diagnostics));
            return proof!;
        }

        /// <summary>Mutiert die kanonische Quellform des Kontextartefakts.</summary>
        private static JsonValue.Object MutateArtifact(BindingContext context, System.Func<JsonValue.Object, JsonValue.Object> mutation)
        {
            var source = (JsonValue.Object)StrictJsonParser.ParseUtf8(ProofV1Serializer.ToCanonicalBytes(context.Proof), JsonParseLimits.Default);
            return mutation(source);
        }

        /// <summary>Baut ein in sich konsistentes Artefakt aus einem DTO (Proofhash neu versiegelt, Quellform passend).</summary>
        private static ProofV1Document Reseal(ProofV1Document proof)
        {
            var proofHash = ProofV1Serializer.ComputeProofHash(proof);
            var sealedProof = new ProofV1Document(
                proof.ProofFormatVersion,
                proof.PuzzleId,
                proof.PublicPuzzleHash,
                proof.SolutionHash,
                proof.SolverVersion,
                proof.SolutionCount,
                proof.Metrics,
                proofHash,
                null);
            var source = ProofV1Serializer.ToJson(sealedProof);
            return new ProofV1Document(
                sealedProof.ProofFormatVersion,
                sealedProof.PuzzleId,
                sealedProof.PublicPuzzleHash,
                sealedProof.SolutionHash,
                sealedProof.SolverVersion,
                sealedProof.SolutionCount,
                sealedProof.Metrics,
                proofHash,
                source);
        }

        private static List<LevelDiagnostic> Bind(BindingContext context, string artifactId, ProofV1Document? proofOverride = null, LevelV2Document? levelOverride = null)
        {
            var diagnostics = new List<LevelDiagnostic>();
            ProofV1Binding.Validate(
                levelOverride ?? context.Level,
                proofOverride ?? context.Proof,
                context.PublicHash,
                context.SolutionHash,
                "solver-v1",
                artifactId,
                diagnostics);
            return diagnostics;
        }

        private static void AssertOnlyCode(List<LevelDiagnostic> diagnostics, string code)
        {
            Assert.AreEqual(1, diagnostics.Count, $"Erwartet wurde genau {code}; vorhanden: {string.Join(", ", diagnostics.ConvertAll(d => d.Code))}");
            Assert.AreEqual(code, diagnostics[0].Code);
        }

        /// <summary>Die Vertragsfixtures binden fehlerfrei (Artefaktformat, Puzzle-/Lösungsbindung, Proofhash, proofRef).</summary>
        [Test]
        public void FixtureArtifacts_BindSuccessfully()
        {
            var standard = Load(ContractFixtures.LevelExample, ContractFixtures.ProofExample);
            Assert.AreEqual(0, Bind(standard, ContractFixtures.ProofExampleArtifactId).Count);

            var singleCell = Load(ContractFixtures.LevelSingleCell, ContractFixtures.ProofSingleCell);
            Assert.AreEqual(0, Bind(singleCell, ContractFixtures.ProofSingleCellArtifactId).Count);
        }

        /// <summary>Copy-Paste eines Proofs auf ein anderes Puzzle wird erkannt.</summary>
        [Test]
        public void CrossPuzzleCopy_Detected()
        {
            var context = Load(ContractFixtures.LevelExample, ContractFixtures.ProofSingleCell);
            var diagnostics = Bind(context, ContractFixtures.ProofSingleCellArtifactId);

            Assert.IsTrue(diagnostics.Exists(d => d.Code == "PRF-PUZZLE-ID-MISMATCH"));
            Assert.IsTrue(diagnostics.Exists(d => d.Code == "PRF-PUBLIC-HASH-MISMATCH"));
            Assert.IsTrue(diagnostics.Exists(d => d.Code == "PRF-PROOFREF-HASH-MISMATCH"));
        }

        /// <summary>Bindet das Level mit einem auf das versiegelte Artefakt aktualisierten proofRef.</summary>
        private static List<LevelDiagnostic> BindWithUpdatedProofRef(BindingContext context, ProofV1Document artifact, string expectedCode)
        {
            var updatedLevel = ContractFixtures.Replace(
                ContractFixtures.ParseLevel(ContractFixtures.LevelExample),
                "$.proofRef.proofHash.sha256",
                ContractFixtures.Text(artifact.ProofHash.Sha256));
            var levelDiagnostics = new List<LevelDiagnostic>();
            var level = LevelV2SchemaValidator.Validate(updatedLevel, levelDiagnostics);
            Assert.NotNull(level);
            return Bind(context, ContractFixtures.ProofExampleArtifactId, artifact, level);
        }

        /// <summary>Ein stale Lösungshash wird erkannt, ohne andere Bindungen auszulösen.</summary>
        [Test]
        public void StaleSolutionHash_Detected()
        {
            var context = Load(ContractFixtures.LevelExample, ContractFixtures.ProofExample);
            var mutated = MutateArtifact(context, root => ContractFixtures.Replace(root, "$.solutionHash.sha256", ContractFixtures.Text(new string('1', 64))));
            var staleProof = Reseal(Reload(mutated));

            AssertOnlyCode(BindWithUpdatedProofRef(context, staleProof, "PRF-SOLUTION-HASH-MISMATCH"), "PRF-SOLUTION-HASH-MISMATCH");
        }

        /// <summary>Eine Metrikänderung ohne Neuversiegelung wird über den Proofhash erkannt.</summary>
        [Test]
        public void MetricChange_Detected()
        {
            var context = Load(ContractFixtures.LevelExample, ContractFixtures.ProofExample);
            var mutated = MutateArtifact(context, root => ContractFixtures.Replace(root, "$.metrics.deductionSteps", ContractFixtures.Int(10)));
            var tamperedProof = Reload(mutated);

            AssertOnlyCode(Bind(context, ContractFixtures.ProofExampleArtifactId, tamperedProof), "PRF-PROOF-HASH-MISMATCH");
        }

        /// <summary>proofRef bindet Artefakt-ID, Proofformat und Proofhash.</summary>
        [Test]
        public void ProofRefBinding_Detected()
        {
            var context = Load(ContractFixtures.LevelExample, ContractFixtures.ProofExample);

            AssertOnlyCode(Bind(context, "proofs/andere-id.json"), "PRF-PROOFREF-ARTIFACT-ID-MISMATCH");

            var mutatedLevel = ContractFixtures.Replace(
                ContractFixtures.ParseLevel(ContractFixtures.LevelExample),
                "$.proofRef.proofHash.sha256",
                ContractFixtures.Text(new string('2', 64)));
            var levelDiagnostics = new List<LevelDiagnostic>();
            var level = LevelV2SchemaValidator.Validate(mutatedLevel, levelDiagnostics);
            Assert.NotNull(level);
            AssertOnlyCode(Bind(context, ContractFixtures.ProofExampleArtifactId, null, level), "PRF-PROOFREF-HASH-MISMATCH");
        }

        /// <summary>Eine abweichende Solverversion im Artefakt wird erkannt.</summary>
        [Test]
        public void SolverVersionMismatch_Detected()
        {
            var context = Load(ContractFixtures.LevelExample, ContractFixtures.ProofExample);
            var mutated = MutateArtifact(context, root => ContractFixtures.Replace(root, "$.solverVersion", ContractFixtures.Text("solver-v2")));
            var mismatchedProof = Reseal(Reload(mutated));

            AssertOnlyCode(BindWithUpdatedProofRef(context, mismatchedProof, "PRF-SOLVER-VERSION-MISMATCH"), "PRF-SOLVER-VERSION-MISMATCH");
        }

        /// <summary>Ein unbekanntes Profil im Artefakt ist ein harter Fehler; ein bekanntes Profil am falschen Slot wird erkannt.</summary>
        [Test]
        public void ProfileRules_Detected()
        {
            var context = Load(ContractFixtures.LevelExample, ContractFixtures.ProofExample);

            var unknown = MutateArtifact(context, root => ContractFixtures.Replace(root, "$.publicPuzzleHash.profile", ContractFixtures.Text("STP-UNKNOWN-JCS-9")));
            var unknownProof = Reseal(Reload(unknown));
            Assert.IsTrue(Bind(context, ContractFixtures.ProofExampleArtifactId, unknownProof).Exists(d => d.Code == "LVL-HASH-UNKNOWN-PROFILE"));

            var wrongSlot = MutateArtifact(context, root => ContractFixtures.Replace(root, "$.publicPuzzleHash.profile", ContractFixtures.Text(HashProfiles.SolutionJcs1)));
            var wrongSlotProof = Reseal(Reload(wrongSlot));
            Assert.IsTrue(Bind(context, ContractFixtures.ProofExampleArtifactId, wrongSlotProof).Exists(d => d.Code == "PRF-PUBLIC-HASH-PROFILE-MISMATCH"));
        }

        /// <summary>Formatverletzungen des Artefakts tragen PRF-FORMAT-Codes.</summary>
        [Test]
        public void ArtifactFormatViolations_Detected()
        {
            var context = Load(ContractFixtures.LevelExample, ContractFixtures.ProofExample);

            var badCount = MutateArtifact(context, root => ContractFixtures.Replace(root, "$.solutionCount", ContractFixtures.Int(0)));
            var countDiagnostics = new List<LevelDiagnostic>();
            Assert.Null(ProofV1SchemaValidator.Validate(badCount, countDiagnostics));
            Assert.IsTrue(countDiagnostics.Exists(d => d.Code == "PRF-FORMAT-SOLUTION-COUNT"));

            var badVersion = MutateArtifact(context, root => ContractFixtures.Replace(root, "$.proofFormatVersion", ContractFixtures.Int(2)));
            var versionDiagnostics = new List<LevelDiagnostic>();
            Assert.Null(ProofV1SchemaValidator.Validate(badVersion, versionDiagnostics));
            Assert.IsTrue(versionDiagnostics.Exists(d => d.Code == "PRF-FORMAT-VERSION"));

            var badMetric = MutateArtifact(context, root => ContractFixtures.Replace(root, "$.metrics.requiredGuessDepth", ContractFixtures.Int(-1)));
            var metricDiagnostics = new List<LevelDiagnostic>();
            Assert.Null(ProofV1SchemaValidator.Validate(badMetric, metricDiagnostics));
            Assert.IsTrue(metricDiagnostics.Exists(d => d.Code == "PRF-FORMAT-METRICS"));

            var additional = MutateArtifact(context, root => ContractFixtures.Add(root, "$", "extra", ContractFixtures.Int(1)));
            var additionalDiagnostics = new List<LevelDiagnostic>();
            Assert.Null(ProofV1SchemaValidator.Validate(additional, additionalDiagnostics));
            Assert.IsTrue(additionalDiagnostics.Exists(d => d.Code == "PRF-FORMAT-ADDITIONAL-PROPERTY"));
        }

        /// <summary>Serialisierung und Deserialisierung sind exakt runderläuflig (kanonische Bytes bleiben gleich).</summary>
        [Test]
        public void Serialization_RoundTripsExactly()
        {
            foreach (var path in new[] { ContractFixtures.ProofExample, ContractFixtures.ProofSingleCell })
            {
                var diagnostics = new List<LevelDiagnostic>();
                var proof = ProofV1Serializer.Deserialize(ContractFixtures.ReadBytes(path), JsonParseLimits.Default, diagnostics);
                Assert.NotNull(proof);
                var bytes = ProofV1Serializer.ToCanonicalBytes(proof!);

                var secondDiagnostics = new List<LevelDiagnostic>();
                var reparsed = ProofV1Serializer.Deserialize(bytes, JsonParseLimits.Default, secondDiagnostics);
                Assert.NotNull(reparsed, string.Join("; ", secondDiagnostics));
                CollectionAssert.AreEqual(bytes, ProofV1Serializer.ToCanonicalBytes(reparsed!));
                Assert.AreEqual(proof!.ProofHash, reparsed!.ProofHash);
            }
        }
    }
}
