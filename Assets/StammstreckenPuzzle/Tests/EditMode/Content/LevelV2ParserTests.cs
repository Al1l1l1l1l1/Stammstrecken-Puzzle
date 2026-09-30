using System.Collections.Generic;
using NUnit.Framework;
using STP.Infrastructure.Content;
using STP.Puzzle.Domain;

namespace STP.Tests.Content.EditMode
{
    /// <summary>
    /// Parse-Stufe des Level-v2-Parsers gemäß LEVEL_DATA_FORMAT.md Abschnitt
    /// 11 und WP-013 AK-02: Vertragsfixtures werden akzeptiert und auf
    /// gültige <see cref="PuzzleDefinition"/>-Objekte gemappt (inklusive
    /// Ein-Zellen-Sonderfall); malformed JSON, Duplicate Keys,
    /// Ressourcenüberschreitungen, polymorphe Typmetadaten und unbekannte
    /// <c>documentSchemaVersion</c>/<c>rulesetVersion</c> werden fail-closed
    /// mit stabilen Diagnosecodes abgelehnt.
    /// </summary>
    public sealed class LevelV2ParserTests
    {
        private static List<LevelDiagnostic> Diagnostics()
        {
            return new List<LevelDiagnostic>();
        }

        private static string ParseCode(byte[] bytes, JsonParseLimits? limits = null)
        {
            try
            {
                StrictJsonParser.ParseUtf8(bytes, limits ?? JsonParseLimits.Default);
                return null!;
            }
            catch (StrictJsonException ex)
            {
                return ex.Code;
            }
        }

        /// <summary>Beide Vertragsfixtures werden akzeptiert und strukturell auf gültige DTOs abgebildet.</summary>
        [Test]
        public void ContractFixtures_ParseAndMapToDtos()
        {
            foreach (var path in new[] { ContractFixtures.LevelExample, ContractFixtures.LevelSingleCell })
            {
                var parsed = StrictJsonParser.ParseUtf8(ContractFixtures.ReadBytes(path), JsonParseLimits.Default);
                var diagnostics = Diagnostics();
                var document = LevelV2SchemaValidator.Validate(parsed, diagnostics);

                Assert.NotNull(document, path);
                Assert.AreEqual(0, diagnostics.Count, path);
                Assert.AreEqual(2, document!.DocumentSchemaVersion);
                Assert.AreEqual("train-track-v1", document.RulesetVersion);
            }
        }

        /// <summary>Die 4×4-Vertragsfixture mappt auf eine gültige Domaindefinition.</summary>
        [Test]
        public void ExampleFixture_MapsToValidPuzzleDefinition()
        {
            var document = LevelV2SchemaValidator.Validate(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), Diagnostics());
            Assert.NotNull(document);
            var diagnostics = Diagnostics();
            var definition = LevelV2DomainMapper.Map(document!, diagnostics);

            Assert.NotNull(definition, string.Join("; ", diagnostics));
            Assert.AreEqual(new GridSize(4, 4), definition!.Grid);
            Assert.AreEqual(new Endpoint(Direction.N, 0), definition.A);
            Assert.AreEqual(new Endpoint(Direction.S, 3), definition.B);
            Assert.AreEqual("train-track-v1", definition.RulesetVersion);
        }

        /// <summary>Der Ein-Zellen-Sonderfall (A und B an derselben Zelle) mappt auf eine gültige Definition.</summary>
        [Test]
        public void SingleCellFixture_MapsToValidPuzzleDefinition()
        {
            var document = LevelV2SchemaValidator.Validate(ContractFixtures.ParseLevel(ContractFixtures.LevelSingleCell), Diagnostics());
            Assert.NotNull(document);
            var diagnostics = Diagnostics();
            var definition = LevelV2DomainMapper.Map(document!, diagnostics);

            Assert.NotNull(definition, string.Join("; ", diagnostics));
            Assert.AreEqual(new GridSize(2, 2), definition!.Grid);
            Assert.AreEqual(definition.A.AdjacentCell(definition.Grid), definition.B.AdjacentCell(definition.Grid));
            Assert.AreEqual(1, document!.Solution.Path.Count);
        }

        /// <summary>Malformed JSON wird mit stabilem Parsecode abgelehnt.</summary>
        [Test]
        public void MalformedJson_Rejected()
        {
            Assert.AreEqual("LVL-PARSE-SYNTAX", ParseCode(ContractFixtures.ToBytes("{\"grid\": }")));
            Assert.AreEqual("LVL-PARSE-SYNTAX", ParseCode(ContractFixtures.ToBytes("{\"a\":1")));
            Assert.AreEqual("LVL-PARSE-TRAILING", ParseCode(ContractFixtures.ToBytes("{} {}")));
            Assert.AreEqual("LVL-PARSE-EMPTY", ParseCode(new byte[0]));
        }

        /// <summary>Doppelte Objektschlüssel werden abgelehnt.</summary>
        [Test]
        public void DuplicateKeys_Rejected()
        {
            Assert.AreEqual("LVL-PARSE-DUPLICATE-KEY", ParseCode(ContractFixtures.ToBytes("{\"a\":1,\"a\":2}")));
            Assert.AreEqual("LVL-PARSE-DUPLICATE-KEY", ParseCode(ContractFixtures.ToBytes("{\"a\":{\"b\":1,\"b\":2}}")));
        }

        /// <summary>Fließkomma- und Exponententokens sind verboten (I-JSON).</summary>
        [Test]
        public void FloatTokens_Rejected()
        {
            Assert.AreEqual("LVL-PARSE-FLOAT-TOKEN", ParseCode(ContractFixtures.ToBytes("{\"a\":1.5}")));
            Assert.AreEqual("LVL-PARSE-FLOAT-TOKEN", ParseCode(ContractFixtures.ToBytes("{\"a\":1e2}")));
            Assert.AreEqual("LVL-PARSE-FLOAT-TOKEN", ParseCode(ContractFixtures.ToBytes("{\"a\":-0.25}")));
        }

        /// <summary>Ungepaarte Surrogate werden abgelehnt; gültige Paare sind zulässig.</summary>
        [Test]
        public void Surrogates_ValidatedStrictly()
        {
            Assert.AreEqual("LVL-PARSE-SURROGATE", ParseCode(ContractFixtures.ToBytes("{\"a\":\"\\uD800x\"}")));
            Assert.AreEqual("LVL-PARSE-SURROGATE", ParseCode(ContractFixtures.ToBytes("{\"a\":\"\\uDC00\"}")));
            Assert.AreEqual("LVL-PARSE-SURROGATE", ParseCode(ContractFixtures.ToBytes("{\"a\":\"\\uD800\"}")));
            Assert.DoesNotThrow(() => StrictJsonParser.ParseUtf8(ContractFixtures.ToBytes("{\"a\":\"\\uD83D\\uDE00\"}"), JsonParseLimits.Default));
        }

        /// <summary>Ungültige UTF-8-Sequenzen und BOM werden abgelehnt.</summary>
        [Test]
        public void EncodingAndBom_Rejected()
        {
            Assert.AreEqual("LVL-PARSE-ENCODING", ParseCode(new byte[] { 0x7B, 0x22, 0x61, 0x22, 0x3A, 0x22, 0xC3, 0x28, 0x22, 0x7D }));
            Assert.AreEqual("LVL-PARSE-BOM", ParseCode(new byte[] { 0xEF, 0xBB, 0xBF, 0x7B, 0x7D }));
        }

        /// <summary>Nicht maskierte Steuerzeichen und ungültige Escapes werden abgelehnt.</summary>
        [Test]
        public void ControlCharactersAndEscapes_Rejected()
        {
            Assert.AreEqual("LVL-PARSE-CONTROL-CHAR", ParseCode(new byte[] { 0x7B, 0x22, 0x61, 0x22, 0x3A, 0x22, 0x09, 0x22, 0x7D }));
            Assert.AreEqual("LVL-PARSE-ESCAPE", ParseCode(ContractFixtures.ToBytes("{\"a\":\"\\q\"}")));
            Assert.AreEqual("LVL-PARSE-ESCAPE", ParseCode(ContractFixtures.ToBytes("{\"a\":\"\\u12xz\"}")));
        }

        /// <summary>Alle dokumentierten Ressourcengrenzen werden durchgesetzt.</summary>
        [Test]
        public void ResourceLimits_Enforced()
        {
            Assert.AreEqual("LVL-PARSE-RESOURCE-BYTES", ParseCode(new byte[16], new JsonParseLimits(maxBytes: 8)));

            var deep = new string('[', 66) + new string(']', 66);
            Assert.AreEqual("LVL-PARSE-RESOURCE-DEPTH", ParseCode(ContractFixtures.ToBytes(deep)));
            var beyondCustomLimit = "[[[[[]]]]]";
            Assert.AreEqual("LVL-PARSE-RESOURCE-DEPTH", ParseCode(ContractFixtures.ToBytes(beyondCustomLimit), new JsonParseLimits(maxDepth: 3)));
            Assert.DoesNotThrow(() => StrictJsonParser.ParseUtf8(ContractFixtures.ToBytes("[[[]]]"), new JsonParseLimits(maxDepth: 3)));

            var longString = "{\"a\":\"" + new string('x', 17_000) + "\"}";
            Assert.AreEqual("LVL-PARSE-RESOURCE-STRING", ParseCode(ContractFixtures.ToBytes(longString)));

            var longArray = "[" + string.Join(",", System.Linq.Enumerable.Repeat("0", 10_001)) + "]";
            Assert.AreEqual("LVL-PARSE-RESOURCE-ARRAY", ParseCode(ContractFixtures.ToBytes(longArray)));
        }

        /// <summary>Polymorphe Typmetadaten sind deaktiviert.</summary>
        [Test]
        public void PolymorphicTypeMetadata_Rejected()
        {
            Assert.AreEqual("LVL-PARSE-TYPE-METADATA", ParseCode(ContractFixtures.ToBytes("{\"$type\":\"System.Object\"}")));
            Assert.AreEqual("LVL-PARSE-TYPE-METADATA", ParseCode(ContractFixtures.ToBytes("{\"a\":{\"$type\":\"X\"}}")));
        }

        /// <summary>Ganzzahlen außerhalb des interoperablen sicheren Bereichs werden abgelehnt.</summary>
        [Test]
        public void IntegerRange_Rejected()
        {
            Assert.AreEqual("LVL-PARSE-INTEGER-RANGE", ParseCode(ContractFixtures.ToBytes("{\"a\":9007199254740992}")));
            Assert.DoesNotThrow(() => StrictJsonParser.ParseUtf8(ContractFixtures.ToBytes("{\"a\":9007199254740991}"), JsonParseLimits.Default));
            Assert.DoesNotThrow(() => StrictJsonParser.ParseUtf8(ContractFixtures.ToBytes("{\"a\":-9007199254740991}"), JsonParseLimits.Default));
        }

        /// <summary>Unbekannte documentSchemaVersion schlägt fail-closed fehl.</summary>
        [Test]
        public void UnknownDocumentSchemaVersion_Rejected()
        {
            var mutated = ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.documentSchemaVersion", ContractFixtures.Int(3));
            var diagnostics = Diagnostics();
            var document = LevelV2SchemaValidator.Validate(mutated, diagnostics);

            Assert.Null(document);
            Assert.IsTrue(diagnostics.Exists(d => d.Code == "LVL-SCHEMA-DOCUMENT-SCHEMA-VERSION" && d.Severity == DiagnosticSeverity.Error));
        }

        /// <summary>Unbekannte rulesetVersion schlägt fail-closed fehl.</summary>
        [Test]
        public void UnknownRulesetVersion_Rejected()
        {
            var mutated = ContractFixtures.Replace(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.rulesetVersion", ContractFixtures.Text("train-track-v2"));
            var diagnostics = Diagnostics();
            var document = LevelV2SchemaValidator.Validate(mutated, diagnostics);

            Assert.Null(document);
            Assert.IsTrue(diagnostics.Exists(d => d.Code == "LVL-SCHEMA-RULESET-VERSION" && d.Severity == DiagnosticSeverity.Error));
        }

        /// <summary>Unbekannte Eigenschaften (additionalProperties: false) werden auf jeder Ebene abgelehnt.</summary>
        [Test]
        public void AdditionalProperties_Rejected()
        {
            var atRoot = ContractFixtures.Add(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$", "unexpected", ContractFixtures.Int(1));
            var rootDiagnostics = Diagnostics();
            Assert.Null(LevelV2SchemaValidator.Validate(atRoot, rootDiagnostics));
            Assert.IsTrue(rootDiagnostics.Exists(d => d.Code == "LVL-SCHEMA-ADDITIONAL-PROPERTY"));

            var inGrid = ContractFixtures.Add(ContractFixtures.ParseLevel(ContractFixtures.LevelExample), "$.grid", "depth", ContractFixtures.Int(1));
            var gridDiagnostics = Diagnostics();
            Assert.Null(LevelV2SchemaValidator.Validate(inGrid, gridDiagnostics));
            Assert.IsTrue(gridDiagnostics.Exists(d => d.Code == "LVL-SCHEMA-ADDITIONAL-PROPERTY" && d.Subject == "$.grid.depth"));
        }
    }
}
