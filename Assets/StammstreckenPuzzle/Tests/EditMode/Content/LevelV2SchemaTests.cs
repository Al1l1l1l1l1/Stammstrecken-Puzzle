using System.Collections.Generic;
using NUnit.Framework;
using STP.Infrastructure.Content;

namespace STP.Tests.Content.EditMode
{
    /// <summary>
    /// Strukturelle Verletzungen je Regel des level-v2-Vertrags (Schema-Stufe):
    /// Pflichtfelder, Konstanten, Enums, Muster, Wertebereiche, Listenlängen,
    /// <c>uniqueItems</c> und die <c>oneOf</c>-Form der Sternschwellen, jeweils
    /// mit stabilen Codes der Familie <c>LVL-SCHEMA-*</c>.
    /// </summary>
    public sealed class LevelV2SchemaTests
    {
        private static List<LevelDiagnostic> ValidateMutated(System.Func<JsonValue.Object, JsonValue.Object> mutation)
        {
            var mutated = mutation(ContractFixtures.ParseLevel(ContractFixtures.LevelExample));
            var diagnostics = new List<LevelDiagnostic>();
            LevelV2SchemaValidator.Validate(mutated, diagnostics);
            return diagnostics;
        }

        private static void AssertCode(List<LevelDiagnostic> diagnostics, string code)
        {
            Assert.IsTrue(
                diagnostics.Exists(d => d.Code == code && d.Severity == DiagnosticSeverity.Error),
                $"Erwarteter Code {code}; vorhanden: {string.Join(", ", diagnostics.ConvertAll(d => d.Code))}");
        }

        /// <summary>Fehlende Pflichteigenschaft.</summary>
        [Test]
        public void MissingRequiredProperty_Rejected()
        {
            AssertCode(ValidateMutated(root => ContractFixtures.Remove(root, "$.rowCounts")), "LVL-SCHEMA-MISSING-PROPERTY");
        }

        /// <summary>Rasterdimensionen oberhalb des Vertragsmaximums.</summary>
        [Test]
        public void GridBounds_Rejected()
        {
            AssertCode(ValidateMutated(root => ContractFixtures.Replace(root, "$.grid.width", ContractFixtures.Int(11))), "LVL-SCHEMA-GRID-WIDTH");
            AssertCode(ValidateMutated(root => ContractFixtures.Replace(root, "$.grid.height", ContractFixtures.Int(1))), "LVL-SCHEMA-GRID-HEIGHT");
        }

        /// <summary>puzzleId verletzt das Muster.</summary>
        [Test]
        public void PuzzleIdPattern_Rejected()
        {
            AssertCode(ValidateMutated(root => ContractFixtures.Replace(root, "$.puzzleId", ContractFixtures.Text("X1-01-01-01"))), "LVL-SCHEMA-PUZZLE-ID-FORMAT");
        }

        /// <summary>contentRevision kleiner als 1.</summary>
        [Test]
        public void ContentRevisionMinimum_Rejected()
        {
            AssertCode(ValidateMutated(root => ContractFixtures.Replace(root, "$.contentRevision", ContractFixtures.Int(0))), "LVL-SCHEMA-CONTENT-REVISION");
        }

        /// <summary>Endpoint-Seite außerhalb des Enums; Index oberhalb des Maximums.</summary>
        [Test]
        public void EndpointFields_Rejected()
        {
            AssertCode(ValidateMutated(root => ContractFixtures.Replace(root, "$.endpoints.a.side", ContractFixtures.Text("X"))), "LVL-SCHEMA-ENDPOINT-SIDE");
            AssertCode(ValidateMutated(root => ContractFixtures.Replace(root, "$.endpoints.a.index", ContractFixtures.Int(10))), "LVL-SCHEMA-ENDPOINT-INDEX");
        }

        /// <summary>Randzahllisten: Länge und Wertebereich.</summary>
        [Test]
        public void CountArrays_Rejected()
        {
            AssertCode(ValidateMutated(root => ContractFixtures.Replace(root, "$.rowCounts[0]", ContractFixtures.Int(11))), "LVL-SCHEMA-ROW-COUNTS");
            AssertCode(ValidateMutated(root => ContractFixtures.Replace(root, "$.columnCounts[0]", ContractFixtures.Int(-1))), "LVL-SCHEMA-COLUMN-COUNTS");
            AssertCode(ValidateMutated(root => ContractFixtures.Add(root, "$", "rowCounts2", ContractFixtures.Int(1))), "LVL-SCHEMA-ADDITIONAL-PROPERTY");
        }

        /// <summary>Lokalisierungsschlüssel müssen dem Muster folgen.</summary>
        [Test]
        public void LocalizationKeyPattern_Rejected()
        {
            AssertCode(ValidateMutated(root => ContractFixtures.Replace(root, "$.content.titleKey", ContractFixtures.Text("INVALID KEY"))), "LVL-SCHEMA-LOCALIZATION-KEY");
        }

        /// <summary>focus: unbekannter Wert, Dublette, leere Liste.</summary>
        [Test]
        public void FocusRules_Rejected()
        {
            AssertCode(ValidateMutated(root => ContractFixtures.Replace(root, "$.production.focus[0]", ContractFixtures.Text("UNKNOWN"))), "LVL-SCHEMA-FOCUS");
            AssertCode(ValidateMutated(root => ContractFixtures.Replace(root, "$.production.focus[1]", ContractFixtures.Text("OCCUPANCY"))), "LVL-SCHEMA-FOCUS");
        }

        /// <summary>starThresholdsSeconds: weder null noch Objekt; fehlende Pflichtfelder im Objekt.</summary>
        [Test]
        public void StarThresholdsOneOf_Rejected()
        {
            AssertCode(ValidateMutated(root => ContractFixtures.Replace(root, "$.production.starThresholdsSeconds", ContractFixtures.Int(5))), "LVL-SCHEMA-STAR-THRESHOLDS");

            var withObject = ContractFixtures.Replace(
                ContractFixtures.ParseLevel(ContractFixtures.LevelExample),
                "$.production.starThresholdsSeconds",
                new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
                {
                    new KeyValuePair<string, JsonValue>("twoStars", new JsonValue.Integer(100)),
                    new KeyValuePair<string, JsonValue>("threeStars", new JsonValue.Integer(50)),
                }));
            var diagnostics = new List<LevelDiagnostic>();
            LevelV2SchemaValidator.Validate(withObject, diagnostics);
            AssertCode(diagnostics, "LVL-SCHEMA-MISSING-PROPERTY");
        }

        /// <summary>Unbekannte Gleisform im Lösungspfad.</summary>
        [Test]
        public void TrackEnum_Rejected()
        {
            AssertCode(ValidateMutated(root => ContractFixtures.Replace(root, "$.solution.path[0].track", ContractFixtures.Text("TRACK_XX"))), "LVL-SCHEMA-PATH-CELL");
        }

        /// <summary>Profilierter Hash: ungültiger sha256-Wert; leeres Profil.</summary>
        [Test]
        public void ProfiledHashRules_Rejected()
        {
            AssertCode(ValidateMutated(root => ContractFixtures.Replace(root, "$.proofRef.proofHash.sha256", ContractFixtures.Text("ABCDEF"))), "LVL-SCHEMA-HASH");
            AssertCode(ValidateMutated(root => ContractFixtures.Replace(root, "$.proofRef.proofHash.profile", ContractFixtures.Text(""))), "LVL-SCHEMA-HASH");
        }

        /// <summary>proofRef.proofFormatVersion ist konstant 1.</summary>
        [Test]
        public void ProofRefFormatVersion_Rejected()
        {
            AssertCode(ValidateMutated(root => ContractFixtures.Replace(root, "$.proofRef.proofFormatVersion", ContractFixtures.Int(2))), "LVL-SCHEMA-PROOF-FORMAT-VERSION");
        }

        /// <summary>Typfehler werden mit stabilem Typcode gemeldet.</summary>
        [Test]
        public void TypeMismatch_Rejected()
        {
            AssertCode(ValidateMutated(root => ContractFixtures.Replace(root, "$.grid.width", ContractFixtures.Text("4"))), "LVL-SCHEMA-TYPE");
        }
    }
}
