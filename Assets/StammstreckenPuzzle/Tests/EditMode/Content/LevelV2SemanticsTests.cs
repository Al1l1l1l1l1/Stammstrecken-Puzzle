using System.Collections.Generic;
using NUnit.Framework;
using STP.Infrastructure.Content;
using STP.Puzzle.Domain;

namespace STP.Tests.Content.EditMode
{
    /// <summary>
    /// Semantische Leveldiagnosen: je ein Vertreter der dokumentierten
    /// Codefamilien <c>LVL-ID-*</c>, <c>LVL-GRID-*</c>, <c>LVL-ENDPOINT-*</c>,
    /// <c>LVL-PATH-*</c>, <c>LVL-RULE-*</c>, <c>LVL-COUNT-*</c> und
    /// <c>LVL-TIME-ORDER</c> sowie <c>LVL-HASH-*</c> gemäß
    /// LEVEL_DATA_FORMAT.md Abschnitt 8 und WP-013 AK-03.
    /// </summary>
    public sealed class LevelV2SemanticsTests
    {
        private static List<LevelDiagnostic> ValidateSemantics(JsonValue.Object root)
        {
            var schemaDiagnostics = new List<LevelDiagnostic>();
            var document = LevelV2SchemaValidator.Validate(root, schemaDiagnostics);
            Assert.NotNull(document, "Schemafehler: " + string.Join("; ", schemaDiagnostics));
            var diagnostics = new List<LevelDiagnostic>();
            var definition = LevelV2DomainMapper.Map(document!, diagnostics);
            if (definition is not null)
            {
                LevelV2Semantics.Validate(document!, definition, diagnostics);
            }
            return diagnostics;
        }

        private static void AssertCode(List<LevelDiagnostic> diagnostics, string code)
        {
            Assert.IsTrue(
                diagnostics.Exists(d => d.Code == code && d.Severity == DiagnosticSeverity.Error),
                $"Erwarteter Code {code}; vorhanden: {string.Join(", ", diagnostics.ConvertAll(d => d.Code + "@" + d.Subject))}");
        }

        /// <summary>Die gültigen Vertragsfixtures erzeugen keinerlei semantische Diagnosen.</summary>
        [Test]
        public void ContractFixtures_HaveNoSemanticDiagnostics()
        {
            Assert.AreEqual(0, ValidateSemantics(ContractFixtures.ParseLevel(ContractFixtures.LevelExample)).Count);
            Assert.AreEqual(0, ValidateSemantics(ContractFixtures.ParseLevel(ContractFixtures.LevelSingleCell)).Count);
        }

        /// <summary>LVL-ID-*: ID-Segmente müssen exakt den Content-Werten entsprechen.</summary>
        [Test]
        public void IdSegmentMismatch_Detected()
        {
            var mutated = ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.content.season", ContractFixtures.Int(2));
            AssertCode(ValidateSemantics(mutated), "LVL-ID-SEGMENT-MISMATCH");
        }

        /// <summary>LVL-ID-*: Season-1-Bereiche 5×4×12.</summary>
        [Test]
        public void Season1Ranges_Detected()
        {
            var mutated = ContractFixtures.Replace(
                ContractFixtures.Replace(
                    ContractFixtures.ParseLevel(ContractFixtures.LevelExample),
                    "$.puzzleId", ContractFixtures.Text("S1-06-01-01")),
                "$.content.networkSection", ContractFixtures.Int(6));
            AssertCode(ValidateSemantics(mutated), "LVL-ID-SEASON1-SECTION-RANGE");
        }

        /// <summary>LVL-GRID-*: Zeilen- und Spaltensummen müssen gleich sein (Domain-Mapping-Stufe).</summary>
        [Test]
        public void SumMismatch_Detected()
        {
            var mutated = ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.rowCounts[0]", ContractFixtures.Int(2));
            AssertCode(ValidateSemantics(mutated), "LVL-GRID-SUM-MISMATCH");
        }

        /// <summary>LVL-GRID-*: Listenlänge muss zur Rasterachse passen (Domain-Mapping-Stufe).</summary>
        [Test]
        public void RowCountLength_Detected()
        {
            var mutated = ContractFixtures.Remove(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.rowCounts[3]");
            AssertCode(ValidateSemantics(mutated), "LVL-GRID-ROWCOUNT-LENGTH");
        }

        /// <summary>LVL-ENDPOINT-*: Identische Außenanschlüsse sind unzulässig.</summary>
        [Test]
        public void IdenticalEndpoints_Detected()
        {
            var mutated = ContractFixtures.Replace(
                ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.endpoints.b.side", ContractFixtures.Text("N")),
                "$.endpoints.b.index", ContractFixtures.Int(0));
            AssertCode(ValidateSemantics(mutated), "LVL-ENDPOINT-IDENTICAL");
        }

        /// <summary>LVL-ENDPOINT-*: Index jenseits der Rasterachse (schemafest, semantisch ungültig).</summary>
        [Test]
        public void EndpointIndexOutOfRange_Detected()
        {
            var mutated = ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.endpoints.b.index", ContractFixtures.Int(4));
            AssertCode(ValidateSemantics(mutated), "LVL-ENDPOINT-INDEX-OUT-OF-RANGE");
        }

        /// <summary>LVL-ENDPOINT-*: Angrenzende Zelle muss laut Randzahlen belegt sein können.</summary>
        [Test]
        public void AdjacentCellEmpty_Detected()
        {
            var mutated = ContractFixtures.Replace(
                ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.rowCounts[0]", ContractFixtures.Int(0)),
                "$.rowCounts[2]", ContractFixtures.Int(4));
            AssertCode(ValidateSemantics(mutated), "LVL-ENDPOINT-ADJACENT-CELL-EMPTY");
        }

        /// <summary>LVL-PATH-*: Keine Koordinatenwiederholung im Lösungspfad.</summary>
        [Test]
        public void PathDuplicate_Detected()
        {
            var mutated = ContractFixtures.Replace(
                ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.solution.path[1].x", ContractFixtures.Int(0)),
                "$.solution.path[1].y", ContractFixtures.Int(0));
            AssertCode(ValidateSemantics(mutated), "LVL-PATH-DUPLICATE-CELL");
        }

        /// <summary>LVL-PATH-*: Aufeinanderfolgende Pfadzellen sind orthogonal benachbart.</summary>
        [Test]
        public void PathNotAdjacent_Detected()
        {
            var mutated = ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.solution.path[3].x", ContractFixtures.Int(3));
            AssertCode(ValidateSemantics(mutated), "LVL-PATH-NOT-ADJACENT");
        }

        /// <summary>LVL-PATH-*: Anschlüsse benachbarter Pfadzellen müssen zusammenpassen.</summary>
        [Test]
        public void PathConnectionMismatch_Detected()
        {
            var mutated = ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.solution.path[1].track", ContractFixtures.Text("TRACK_EW"));
            AssertCode(ValidateSemantics(mutated), "LVL-PATH-CONNECTION-MISMATCH");
        }

        /// <summary>LVL-PATH-*: Erste/letzte Pfadzelle muss die Außenanschlüsse bedienen.</summary>
        [Test]
        public void PathEndpointMismatch_Detected()
        {
            var mutated = ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.solution.path[0].track", ContractFixtures.Text("TRACK_EW"));
            AssertCode(ValidateSemantics(mutated), "LVL-PATH-ENDPOINT-A-MISMATCH");

            var mutatedB = ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.solution.path[8].track", ContractFixtures.Text("TRACK_EW"));
            AssertCode(ValidateSemantics(mutatedB), "LVL-PATH-ENDPOINT-B-MISMATCH");
        }

        /// <summary>LVL-RULE-*: Eine Gleisform darf nicht im Raster oder an einer falschen Außenkante enden.</summary>
        [Test]
        public void RuleOpenEnd_Detected()
        {
            var mutated = ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.solution.path[4].track", ContractFixtures.Text("TRACK_ES"));
            AssertCode(ValidateSemantics(mutated), "LVL-RULE-OPEN-END");
        }

        /// <summary>LVL-RULE-*: Keine Verbindung außerhalb der Pfadfolge (Schleife oder Abzweigung).</summary>
        [Test]
        public void RuleLoop_Detected()
        {
            var mutated = ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.solution.path[7].track", ContractFixtures.Text("TRACK_SW"));
            AssertCode(ValidateSemantics(mutated), "LVL-RULE-LOOP");
        }

        /// <summary>LVL-COUNT-*: Aus der Lösung abgeleitete Randzahlen müssen den deklarierten entsprechen.</summary>
        [Test]
        public void CountMismatch_Detected()
        {
            var mutated = ContractFixtures.Replace(
                ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.rowCounts[2]", ContractFixtures.Int(3)),
                "$.rowCounts[3]", ContractFixtures.Int(2));
            AssertCode(ValidateSemantics(mutated), "LVL-COUNT-ROW-MISMATCH");
        }

        /// <summary>LVL-TIME-ORDER: threeStars strikt kleiner twoStars oder vollständig null.</summary>
        [Test]
        public void TimeOrder_Detected()
        {
            var thresholds = new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
            {
                new KeyValuePair<string, JsonValue>("twoStars", new JsonValue.Integer(100)),
                new KeyValuePair<string, JsonValue>("threeStars", new JsonValue.Integer(100)),
                new KeyValuePair<string, JsonValue>("calibrationVersion", new JsonValue.String("test-v1")),
            });
            var mutated = ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.production.starThresholdsSeconds", thresholds);
            AssertCode(ValidateSemantics(mutated), "LVL-TIME-ORDER");
        }

        /// <summary>LVL-TIME-ORDER: Gültige Schwellen erzeugen keine Diagnose.</summary>
        [Test]
        public void TimeOrder_ValidThresholdsPass()
        {
            var thresholds = new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
            {
                new KeyValuePair<string, JsonValue>("twoStars", new JsonValue.Integer(100)),
                new KeyValuePair<string, JsonValue>("threeStars", new JsonValue.Integer(50)),
                new KeyValuePair<string, JsonValue>("calibrationVersion", new JsonValue.String("test-v1")),
            });
            var mutated = ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.production.starThresholdsSeconds", thresholds);
            Assert.AreEqual(0, ValidateSemantics(mutated).Count);
        }

        /// <summary>LVL-HASH-*: Ein unbekanntes Profil ist ein harter Fehler.</summary>
        [Test]
        public void UnknownHashProfile_Detected()
        {
            var mutated = ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.proofRef.proofHash.profile", ContractFixtures.Text("STP-UNKNOWN-JCS-9"));
            AssertCode(ValidateSemantics(mutated), "LVL-HASH-UNKNOWN-PROFILE");
            Assert.IsFalse(HashProfiles.IsKnown("STP-UNKNOWN-JCS-9"));
            Assert.IsTrue(HashProfiles.IsKnown(HashProfiles.PuzzleSemanticJcs1));
            Assert.IsTrue(HashProfiles.IsKnown(HashProfiles.SolutionJcs1));
            Assert.IsTrue(HashProfiles.IsKnown(HashProfiles.ProofJcs1));
            Assert.IsTrue(HashProfiles.IsKnown(HashProfiles.LevelV1PuzzleJcs1));
        }
    }
}
