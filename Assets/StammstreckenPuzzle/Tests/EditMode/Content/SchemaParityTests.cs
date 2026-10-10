using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using STP.Infrastructure.Content;

namespace STP.Tests.Content.EditMode
{
    /// <summary>
    /// Parity between the committed JSON Schemas (the contract) and the C# readers. An independent mini validator reads the real
    /// schema files; every structural property (closed objects, required members, kinds, bounds, enums, patterns, constants, array
    /// rules) is checked against the reader, and randomized mutants must receive the identical accept/reject verdict.
    /// </summary>
    public sealed class SchemaParityTests
    {
        private const string V2Schema = "ARCHITECTURE/schemas/level-v2.schema.json";
        private const string V1Schema = "ARCHITECTURE/schemas/level-v1.schema.json";

        private static JsonValue Schema(string file = V2Schema) => Examples.Json(file);
        private static JsonValue Parse(string text) => StrictJsonParser.ParseText(text).Value!;
        private static string Jcs(JsonValue? json) => JcsSerializer.Serialize(json).Text!;

        private static JsonValue WithThresholds(JsonValue document) =>
            JsonEdit.Set(document, "/production/starThresholdsSeconds", Parse("{\"twoStars\":90,\"threeStars\":60,\"calibrationVersion\":\"calibration-1\"}"));

        private static LevelV2ReadResult ReadV2(JsonValue json) => LevelV2Reader.Read(json);
        private static bool Has(IEnumerable<LevelDiagnostic> diagnostics, string code, string path) => diagnostics.Any(d => d.Code == code && d.Path == path);

        // ---- mini validator (independent subset implementation of JSON Schema 2020-12) -------------------------------

        private sealed class MiniValidator
        {
            private static readonly HashSet<string> Annotations = new HashSet<string> { "$schema", "$id", "title", "description", "$defs" };
            private readonly JsonValue _root;
            internal MiniValidator(JsonValue root) { _root = root; }

            internal JsonValue Resolve(JsonValue schema)
            {
                var reference = schema.Get("$ref");
                if (reference == null) return schema;
                string pointer = reference.StringValue;
                if (!pointer.StartsWith("#/", StringComparison.Ordinal)) throw new AssertionException("unsupported $ref " + pointer);
                var node = _root;
                foreach (string token in pointer.Substring(2).Split('/')) node = node.Get(token) ?? throw new AssertionException("unresolved $ref " + pointer);
                return Resolve(node);
            }

            internal bool IsValid(JsonValue schema, JsonValue instance)
            {
                schema = Resolve(schema);
                foreach (var member in schema.Members)
                {
                    if (!Known(member.Name)) throw new AssertionException("the schema uses a keyword the parity validator does not implement: " + member.Name);
                    if (!Check(member.Name, member.Value, schema, instance)) return false;
                }
                return true;
            }

            private static bool Known(string keyword)
            {
                if (Annotations.Contains(keyword)) return true;
                switch (keyword)
                {
                    case "type": case "const": case "enum": case "required": case "properties": case "additionalProperties": case "items": case "minItems": case "maxItems":
                    case "uniqueItems": case "minimum": case "maximum": case "minLength": case "maxLength": case "pattern": case "oneOf":
                        return true;
                    default: return false;
                }
            }

            private bool Check(string keyword, JsonValue argument, JsonValue schema, JsonValue instance)
            {
                switch (keyword)
                {
                    case "type": return KindMatches(argument.StringValue, instance);
                    case "const": return Jcs(argument) == Jcs(instance);
                    case "enum": return argument.Items.Any(candidate => Jcs(candidate) == Jcs(instance));
                    case "oneOf": return argument.Items.Count(branch => IsValid(branch, instance)) == 1;
                    case "required":
                        return instance.Kind != JsonKind.Object || argument.Items.All(name => instance.Get(name.StringValue) != null);
                    case "properties":
                        if (instance.Kind != JsonKind.Object) return true;
                        foreach (var member in instance.Members)
                        {
                            var child = argument.Get(member.Name);
                            if (child != null && !IsValid(child, member.Value)) return false;
                        }
                        return true;
                    case "additionalProperties":
                        if (argument.Kind != JsonKind.Boolean || argument.BooleanValue) throw new AssertionException("only additionalProperties:false is supported");
                        return instance.Kind != JsonKind.Object || instance.Members.All(m => schema.Get("properties")!.Get(m.Name) != null);
                    case "items":
                        return instance.Kind != JsonKind.Array || instance.Items.All(item => IsValid(argument, item));
                    case "minItems": return instance.Kind != JsonKind.Array || instance.Items.Count >= argument.IntegerValue;
                    case "maxItems": return instance.Kind != JsonKind.Array || instance.Items.Count <= argument.IntegerValue;
                    case "uniqueItems":
                        if (!argument.BooleanValue || instance.Kind != JsonKind.Array) return true;
                        return instance.Items.Select(Jcs).Distinct().Count() == instance.Items.Count;
                    case "minimum": return instance.Kind != JsonKind.Integer || instance.IntegerValue >= argument.IntegerValue;
                    case "maximum": return instance.Kind != JsonKind.Integer || instance.IntegerValue <= argument.IntegerValue;
                    case "minLength": return instance.Kind != JsonKind.String || CodePoints(instance.StringValue) >= argument.IntegerValue;
                    case "maxLength": return instance.Kind != JsonKind.String || CodePoints(instance.StringValue) <= argument.IntegerValue;
                    case "pattern": return instance.Kind != JsonKind.String || Regex(argument.StringValue).IsMatch(instance.StringValue);
                    default: return true;
                }
            }

            private static bool KindMatches(string type, JsonValue instance)
            {
                switch (type)
                {
                    case "object": return instance.Kind == JsonKind.Object;
                    case "array": return instance.Kind == JsonKind.Array;
                    case "string": return instance.Kind == JsonKind.String;
                    case "integer": return instance.Kind == JsonKind.Integer;
                    case "null": return instance.Kind == JsonKind.Null;
                    case "boolean": return instance.Kind == JsonKind.Boolean;
                    default: throw new AssertionException("unsupported type " + type);
                }
            }

            private static int CodePoints(string text)
            {
                int count = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (char.IsHighSurrogate(text[i]) && i + 1 < text.Length && char.IsLowSurrogate(text[i + 1])) i++;
                    count++;
                }
                return count;
            }

            private static readonly Dictionary<string, Regex> Cache = new Dictionary<string, Regex>();

            /// <summary>ECMA-262 anchors: the dollar sign matches only at the very end (the .NET dollar also matches before a trailing newline).</summary>
            internal static Regex Regex(string pattern)
            {
                if (Cache.TryGetValue(pattern, out var cached)) return cached;
                string converted = pattern.EndsWith("$", StringComparison.Ordinal) ? pattern.Substring(0, pattern.Length - 1) + "\\z" : pattern;
                return Cache[pattern] = new Regex(converted, RegexOptions.CultureInvariant);
            }
        }

        private static MiniValidator ValidatorOf(JsonValue schema) => new MiniValidator(schema);

        // ---- schema walking --------------------------------------------------------------------------------------

        private static IEnumerable<(string Path, JsonValue Schema, JsonValue Instance)> Nodes(MiniValidator validator, JsonValue schema, JsonValue instance, string path)
        {
            schema = validator.Resolve(schema);
            yield return (path, schema, instance);
            var effective = schema;
            var oneOf = schema.Get("oneOf");
            if (oneOf != null) effective = validator.Resolve(oneOf.Items.First(branch => validator.IsValid(branch, instance)));
            if (instance.Kind == JsonKind.Object && effective.Get("properties") != null)
            {
                foreach (var member in instance.Members)
                {
                    var child = effective.Get("properties")!.Get(member.Name);
                    if (child == null) continue;
                    foreach (var node in Nodes(validator, child, member.Value, path + "/" + member.Name)) yield return node;
                }
            }
            else if (instance.Kind == JsonKind.Array && effective.Get("items") != null)
            {
                for (int i = 0; i < instance.Items.Count; i++)
                    foreach (var node in Nodes(validator, effective.Get("items")!, instance.Items[i], path + "/" + i.ToString(CultureInfo.InvariantCulture))) yield return node;
            }
        }

        private static IEnumerable<JsonValue> AllSchemaObjects(JsonValue node)
        {
            if (node.Kind == JsonKind.Object)
            {
                yield return node;
                foreach (var member in node.Members) foreach (var child in AllSchemaObjects(member.Value)) yield return child;
            }
            else if (node.Kind == JsonKind.Array)
            {
                foreach (var item in node.Items) foreach (var child in AllSchemaObjects(item)) yield return child;
            }
        }

        private static IReadOnlyList<JsonValue> Documents() => new[] { Examples.Json(Examples.V2), Examples.Json(Examples.V2Single), WithThresholds(Examples.Json(Examples.V2)) };

        // ---- structure ---------------------------------------------------------------------------------------------

        [Test]
        public void EveryObjectSchema_IsClosed_AndRequiresExactlyItsProperties()
        {
            foreach (string file in new[] { V2Schema, V1Schema })
            {
                int objects = 0;
                foreach (var node in AllSchemaObjects(Schema(file)))
                {
                    var type = node.Get("type");
                    if (type == null || type.Kind != JsonKind.String || type.StringValue != "object") continue;
                    objects++;
                    var properties = node.Get("properties")!.Members.Select(m => m.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray();
                    var required = node.Get("required")!.Items.Select(i => i.StringValue).OrderBy(n => n, StringComparer.Ordinal).ToArray();
                    Assert.That(required, Is.EqualTo(properties), file + ": every property is required");
                    Assert.That(node.Get("additionalProperties")!.BooleanValue, Is.False, file + ": object schema must be closed");
                }
                Assert.That(objects, Is.GreaterThanOrEqualTo(10), file);
            }
        }

        [Test]
        public void TheParityValidator_AcceptsTheCommittedExamples_AndRejectsKnownBadDocuments()
        {
            var v2 = ValidatorOf(Schema());
            foreach (var document in Documents()) Assert.That(v2.IsValid(Schema(), document), Is.True);
            var v1 = ValidatorOf(Schema(V1Schema));
            foreach (string file in Examples.V1Files) Assert.That(v1.IsValid(Schema(V1Schema), Examples.Json(file)), Is.True, file);
            Assert.That(v2.IsValid(Schema(), JsonEdit.Remove(Examples.Json(Examples.V2), "/proofRef")), Is.False);
            Assert.That(v2.IsValid(Schema(), JsonEdit.Add(Examples.Json(Examples.V2), "", "zz", JsonEdit.Int(1))), Is.False);
            Assert.That(v2.IsValid(Schema(), JsonEdit.Set(Examples.Json(Examples.V2), "/grid/width", JsonEdit.Int(11))), Is.False);
            Assert.That(v1.IsValid(Schema(V1Schema), Examples.Json(Examples.V2)), Is.False);
        }

        [Test]
        public void EveryRequiredMember_AtEveryObject_IsReportedWhenMissing()
        {
            var schema = Schema();
            var validator = ValidatorOf(schema);
            int checkedMembers = 0;
            foreach (var document in Documents())
            {
                foreach (var (path, nodeSchema, instance) in Nodes(validator, schema, document, string.Empty))
                {
                    var required = nodeSchema.Get("required");
                    if (instance.Kind != JsonKind.Object || required == null) continue;
                    foreach (var name in required.Items.Select(i => i.StringValue))
                    {
                        var result = ReadV2(JsonEdit.Remove(document, path + "/" + name));
                        Assert.That(result.Document, Is.Null, path + "/" + name);
                        Assert.That(Has(result.Diagnostics, LevelDiagnosticCodes.SchemaRequired, path + "/" + name), Is.True, path + "/" + name + ": " + Codes.Describe(result.Diagnostics));
                        checkedMembers++;
                    }
                }
            }
            Assert.That(checkedMembers, Is.GreaterThan(60));
        }

        [Test]
        public void AnUnknownMember_AtEveryObject_IsRejected()
        {
            var schema = Schema();
            var validator = ValidatorOf(schema);
            int objects = 0;
            foreach (var document in Documents())
            {
                foreach (var (path, _, instance) in Nodes(validator, schema, document, string.Empty))
                {
                    if (instance.Kind != JsonKind.Object) continue;
                    var result = ReadV2(JsonEdit.Add(document, path, "zzUnknown", JsonEdit.Int(1)));
                    Assert.That(result.Document, Is.Null, path);
                    Assert.That(Has(result.Diagnostics, LevelDiagnosticCodes.SchemaUnknownProperty, path + "/zzUnknown"), Is.True, path + ": " + Codes.Describe(result.Diagnostics));
                    objects++;
                }
            }
            Assert.That(objects, Is.GreaterThan(20));
        }

        [Test]
        public void AWrongKind_AtEveryNode_IsASchemaTypeError_AtThatPath()
        {
            var schema = Schema();
            var validator = ValidatorOf(schema);
            int nodes = 0;
            foreach (var document in Documents())
            {
                foreach (var (path, _, instance) in Nodes(validator, schema, document, string.Empty))
                {
                    if (path.Length == 0) continue;
                    JsonValue wrong = instance.Kind == JsonKind.String ? JsonEdit.Int(5)
                        : instance.Kind == JsonKind.Integer ? JsonEdit.Str("5")
                        : instance.Kind == JsonKind.Object ? JsonValue.CreateArray(new JsonValue[0])
                        : instance.Kind == JsonKind.Array ? JsonValue.CreateObject(new JsonMember[0])
                        : JsonEdit.Str("not-null");
                    var result = ReadV2(JsonEdit.Set(document, path, wrong));
                    Assert.That(result.Document, Is.Null, path);
                    Assert.That(Has(result.Diagnostics, LevelDiagnosticCodes.SchemaType, path), Is.True, path + ": " + Codes.Describe(result.Diagnostics));
                    var asNull = ReadV2(JsonEdit.Set(document, path, JsonValue.Null));
                    if (instance.Kind != JsonKind.Null && path != "/production/starThresholdsSeconds") // null is a valid alternative of oneOf there
                    {
                        Assert.That(asNull.Document, Is.Null, path + " as null");
                        Assert.That(Has(asNull.Diagnostics, LevelDiagnosticCodes.SchemaType, path), Is.True, path + " as null: " + Codes.Describe(asNull.Diagnostics));
                    }
                    nodes++;
                }
            }
            Assert.That(nodes, Is.GreaterThan(80));
        }

        [Test]
        public void IntegerBounds_AreExactlyThoseOfTheSchema()
        {
            var schema = Schema();
            var validator = ValidatorOf(schema);
            int bounded = 0;
            foreach (var document in Documents())
            {
                foreach (var (path, nodeSchema, instance) in Nodes(validator, schema, document, string.Empty))
                {
                    if (instance.Kind != JsonKind.Integer) continue;
                    var minimum = nodeSchema.Get("minimum");
                    var maximum = nodeSchema.Get("maximum");
                    if (minimum != null)
                    {
                        Assert.That(Has(ReadV2(JsonEdit.Set(document, path, JsonEdit.Int(minimum.IntegerValue))).Diagnostics, LevelDiagnosticCodes.SchemaRange, path), Is.False, path + " at minimum");
                        var below = ReadV2(JsonEdit.Set(document, path, JsonEdit.Int(minimum.IntegerValue - 1)));
                        Assert.That(Has(below.Diagnostics, LevelDiagnosticCodes.SchemaRange, path), Is.True, path + " below minimum: " + Codes.Describe(below.Diagnostics));
                        bounded++;
                    }
                    if (maximum != null)
                    {
                        Assert.That(Has(ReadV2(JsonEdit.Set(document, path, JsonEdit.Int(maximum.IntegerValue))).Diagnostics, LevelDiagnosticCodes.SchemaRange, path), Is.False, path + " at maximum");
                        var above = ReadV2(JsonEdit.Set(document, path, JsonEdit.Int(maximum.IntegerValue + 1)));
                        Assert.That(Has(above.Diagnostics, LevelDiagnosticCodes.SchemaRange, path), Is.True, path + " above maximum: " + Codes.Describe(above.Diagnostics));
                        bounded++;
                    }
                    else if (minimum != null)
                    {
                        var top = ReadV2(JsonEdit.Set(document, path, JsonEdit.Int(JsonValue.MaxSafeInteger)));
                        Assert.That(Has(top.Diagnostics, LevelDiagnosticCodes.SchemaRange, path), Is.False, path + " has no schema maximum");
                    }
                }
            }
            Assert.That(bounded, Is.GreaterThan(40));
        }

        [Test]
        public void ReaderLevelBoundaryDocuments_AreAccepted()
        {
            var doc = Examples.Json(Examples.V2);
            var zeros10 = JsonValue.CreateArray(Enumerable.Repeat(JsonEdit.Int(0), 10).ToList());
            var edited = JsonEdit.Set(doc, "/grid/width", JsonEdit.Int(10));
            edited = JsonEdit.Set(edited, "/grid/height", JsonEdit.Int(2));
            edited = JsonEdit.Set(edited, "/rowCounts", JsonValue.CreateArray(new[] { JsonEdit.Int(10), JsonEdit.Int(10) }));
            edited = JsonEdit.Set(edited, "/columnCounts", zeros10);
            edited = JsonEdit.Set(edited, "/endpoints/a/index", JsonEdit.Int(9));
            Assert.That(ReadV2(edited).Succeeded, Is.True, "schema-level bounds: width 10, height 2, 10 columns, index 9");
            Assert.That(ReadV2(JsonEdit.Set(doc, "/contentRevision", JsonEdit.Int(JsonValue.MaxSafeInteger))).Succeeded, Is.True);
        }

        [Test]
        public void ConstantsAndTheirDedicatedCodes()
        {
            var doc = Examples.Json(Examples.V2);
            Assert.That(Codes.Of(ReadV2(JsonEdit.Set(doc, "/documentSchemaVersion", JsonEdit.Int(1))).Diagnostics), Is.EqualTo(new[] { LevelDiagnosticCodes.VersionUnknown }));
            Assert.That(Codes.Of(ReadV2(JsonEdit.Set(doc, "/documentSchemaVersion", JsonEdit.Int(3))).Diagnostics), Is.EqualTo(new[] { LevelDiagnosticCodes.VersionUnknown }));
            Assert.That(Has(ReadV2(JsonEdit.Set(doc, "/rulesetVersion", JsonEdit.Str("train-track-v2"))).Diagnostics, LevelDiagnosticCodes.VersionRulesetUnknown, "/rulesetVersion"), Is.True);
            Assert.That(Has(ReadV2(JsonEdit.Set(doc, "/proofRef/proofFormatVersion", JsonEdit.Int(2))).Diagnostics, LevelDiagnosticCodes.VersionProofFormatUnknown, "/proofRef/proofFormatVersion"), Is.True);
            Assert.That(ReadV2(JsonEdit.Set(doc, "/documentSchemaVersion", JsonEdit.Str("2"))).Document, Is.Null);
            Assert.That(ReadV2(JsonEdit.Remove(doc, "/documentSchemaVersion")).Document, Is.Null);
        }

        [Test]
        public void StringRules_FollowThePatternsAndLengthsOfTheSchema()
        {
            var schema = Schema();
            var validator = ValidatorOf(schema);
            int checkedStrings = 0;
            foreach (var document in Documents())
            {
                foreach (var (path, nodeSchema, instance) in Nodes(validator, schema, document, string.Empty))
                {
                    if (instance.Kind != JsonKind.String || nodeSchema.Get("enum") != null || nodeSchema.Get("const") != null) continue;
                    var empty = ReadV2(JsonEdit.Set(document, path, JsonEdit.Str(string.Empty)));
                    if (nodeSchema.Get("pattern") != null)
                    {
                        Assert.That(Has(empty.Diagnostics, LevelDiagnosticCodes.SchemaPattern, path), Is.True, path + ": " + Codes.Describe(empty.Diagnostics));
                        var trailingNewline = ReadV2(JsonEdit.Set(document, path, JsonEdit.Str(instance.StringValue + "\n")));
                        Assert.That(Has(trailingNewline.Diagnostics, LevelDiagnosticCodes.SchemaPattern, path), Is.True, path + " must be fully anchored");
                    }
                    else
                    {
                        Assert.That(Has(empty.Diagnostics, LevelDiagnosticCodes.SchemaLength, path), Is.True, path + ": " + Codes.Describe(empty.Diagnostics));
                        Assert.That(ReadV2(JsonEdit.Set(document, path, JsonEdit.Str("x"))).Succeeded, Is.True, path + " accepts any non-empty text");
                        string astral = "\uD83D\uDE82";
                        Assert.That(ReadV2(JsonEdit.Set(document, path, JsonEdit.Str(astral))).Succeeded, Is.True, path + " counts code points, not UTF-16 units");
                        Assert.That(ReadV2(JsonEdit.Set(document, path, JsonEdit.Str(new string('x', 5000)))).Succeeded, Is.True, path + " has no schema maximum");
                    }
                    checkedStrings++;
                }
            }
            Assert.That(checkedStrings, Is.GreaterThan(20));
        }

        [Test]
        public void Enumerations_AreExactlyTheSchemaEnumerations_AndMatchTheClrEnums()
        {
            var schema = Schema();
            string[] Enum(string pointer) => Walk(schema, pointer).Get("enum")!.Items.Select(i => i.StringValue).ToArray();
            Assert.That(Enum("$defs/endpoint/properties/side"), Is.EqualTo(System.Enum.GetNames(typeof(LevelSide))));
            Assert.That(Enum("$defs/pathCell/properties/track"), Is.EqualTo(System.Enum.GetNames(typeof(LevelTrack))));
            Assert.That(Walk(schema, "properties/production/properties/focus/items").Get("enum")!.Items.Select(i => i.StringValue).ToArray(), Is.EqualTo(System.Enum.GetNames(typeof(LevelFocus))));

            var doc = Examples.Json(Examples.V2);
            foreach (string side in Enum("$defs/endpoint/properties/side"))
                Assert.That(ReadV2(JsonEdit.Set(doc, "/endpoints/a/side", JsonEdit.Str(side))).Succeeded, Is.True, side);
            foreach (string track in Enum("$defs/pathCell/properties/track"))
                Assert.That(ReadV2(JsonEdit.Set(doc, "/solution/path/0/track", JsonEdit.Str(track))).Succeeded, Is.True, track);
            foreach (string focus in Walk(schema, "properties/production/properties/focus/items").Get("enum")!.Items.Select(i => i.StringValue))
                Assert.That(ReadV2(JsonEdit.Set(doc, "/production/focus", JsonValue.CreateArray(new[] { JsonEdit.Str(focus) }))).Succeeded, Is.True, focus);

            foreach (string invalid in new[] { "n", "NORTH", "", " N", "N ", "Track_NS", "TRACK_NS ", "track_ns", "OCCUPANCY ", "occupancy" })
            {
                Assert.That(Has(ReadV2(JsonEdit.Set(doc, "/endpoints/a/side", JsonEdit.Str(invalid))).Diagnostics, LevelDiagnosticCodes.SchemaEnum, "/endpoints/a/side"), Is.True, "side '" + invalid + "'");
                Assert.That(Has(ReadV2(JsonEdit.Set(doc, "/solution/path/0/track", JsonEdit.Str(invalid))).Diagnostics, LevelDiagnosticCodes.SchemaEnum, "/solution/path/0/track"), Is.True, "track '" + invalid + "'");
                Assert.That(Has(ReadV2(JsonEdit.Set(doc, "/production/focus/0", JsonEdit.Str(invalid))).Diagnostics, LevelDiagnosticCodes.SchemaEnum, "/production/focus/0"), Is.True, "focus '" + invalid + "'");
            }
        }

        [Test]
        public void ArrayRules_MinMaxAndUniqueness_FollowTheSchema()
        {
            var doc = Examples.Json(Examples.V2);
            var ten = JsonValue.CreateArray(Enumerable.Repeat(JsonEdit.Int(0), 10).ToList());
            var eleven = JsonValue.CreateArray(Enumerable.Repeat(JsonEdit.Int(0), 11).ToList());
            var one = JsonValue.CreateArray(new[] { JsonEdit.Int(0) });
            foreach (string name in new[] { "rowCounts", "columnCounts" })
            {
                Assert.That(Has(ReadV2(JsonEdit.Set(doc, "/" + name, one)).Diagnostics, LevelDiagnosticCodes.SchemaLength, "/" + name), Is.True, name + " min");
                Assert.That(Has(ReadV2(JsonEdit.Set(doc, "/" + name, eleven)).Diagnostics, LevelDiagnosticCodes.SchemaLength, "/" + name), Is.True, name + " max");
                Assert.That(Has(ReadV2(JsonEdit.Set(doc, "/" + name, ten)).Diagnostics, LevelDiagnosticCodes.SchemaLength, "/" + name), Is.False, name + " 10 items");
                Assert.That(Has(ReadV2(JsonEdit.Set(doc, "/" + name, JsonValue.CreateArray(new JsonValue[0]))).Diagnostics, LevelDiagnosticCodes.SchemaLength, "/" + name), Is.True, name + " empty");
            }
            Assert.That(Has(ReadV2(JsonEdit.Set(doc, "/production/focus", JsonValue.CreateArray(new JsonValue[0]))).Diagnostics, LevelDiagnosticCodes.SchemaLength, "/production/focus"), Is.True);
            Assert.That(Has(ReadV2(JsonEdit.Set(doc, "/solution/path", JsonValue.CreateArray(new JsonValue[0]))).Diagnostics, LevelDiagnosticCodes.SchemaLength, "/solution/path"), Is.True);
            var duplicate = JsonValue.CreateArray(new[] { JsonEdit.Str("CHAIN"), JsonEdit.Str("DENSITY"), JsonEdit.Str("CHAIN") });
            Assert.That(Has(ReadV2(JsonEdit.Set(doc, "/production/focus", duplicate)).Diagnostics, LevelDiagnosticCodes.SchemaUnique, "/production/focus/2"), Is.True);
            var all = JsonValue.CreateArray(System.Enum.GetNames(typeof(LevelFocus)).Select(n => JsonEdit.Str(n)).ToList());
            Assert.That(ReadV2(JsonEdit.Set(doc, "/production/focus", all)).Succeeded, Is.True, "all ten focus values together");
        }

        [Test]
        public void StarThresholds_AcceptNullOrTheClosedObject_Only()
        {
            var doc = Examples.Json(Examples.V2);
            Assert.That(ReadV2(doc).Succeeded, Is.True);
            Assert.That(ReadV2(WithThresholds(doc)).Succeeded, Is.True);
            foreach (string bad in new[] { "\"x\"", "5", "[]", "true", "{}", "{\"twoStars\":1,\"threeStars\":1}", "{\"twoStars\":0,\"threeStars\":1,\"calibrationVersion\":\"c\"}",
                "{\"twoStars\":1,\"threeStars\":1,\"calibrationVersion\":\"\"}", "{\"twoStars\":1,\"threeStars\":1,\"calibrationVersion\":\"c\",\"fourStars\":1}" })
                Assert.That(ReadV2(JsonEdit.Set(doc, "/production/starThresholdsSeconds", Parse(bad))).Document, Is.Null, bad);
        }

        // ---- verdict parity under randomized mutation --------------------------------------------------------------

        private const string StringAlphabet = "abcxyzABCXYZ0123456789.-_/ \nS:\u0663\uFF41\u00E9";

        private static string RandomString(Random random, string? seed)
        {
            var chars = new List<char>();
            if (seed != null && random.Next(3) != 0) chars.AddRange(seed);
            else for (int i = random.Next(0, 16); i > 0; i--) chars.Add(StringAlphabet[random.Next(StringAlphabet.Length)]);
            if (seed != null) for (int edits = random.Next(0, 3); edits > 0; edits--)
            {
                int at = chars.Count == 0 ? 0 : random.Next(chars.Count + 1);
                switch (random.Next(3))
                {
                    case 0: chars.Insert(Math.Min(at, chars.Count), StringAlphabet[random.Next(StringAlphabet.Length)]); break;
                    case 1: if (chars.Count > 0) chars.RemoveAt(Math.Min(at, chars.Count - 1)); break;
                    default: if (chars.Count > 0) chars[Math.Min(at, chars.Count - 1)] = StringAlphabet[random.Next(StringAlphabet.Length)]; break;
                }
            }
            return new string(chars.ToArray());
        }

        private static void CollectPaths(JsonValue node, string path, List<string> paths, List<string> objects, List<string> arrays)
        {
            if (path.Length > 0) paths.Add(path);
            if (node.Kind == JsonKind.Object)
            {
                objects.Add(path);
                foreach (var member in node.Members) CollectPaths(member.Value, path + "/" + member.Name, paths, objects, arrays);
            }
            else if (node.Kind == JsonKind.Array)
            {
                arrays.Add(path);
                for (int i = 0; i < node.Items.Count; i++) CollectPaths(node.Items[i], path + "/" + i.ToString(CultureInfo.InvariantCulture), paths, objects, arrays);
            }
        }

        private static JsonValue Mutate(JsonValue document, Random random)
        {
            int counter = 0;
            for (int operations = random.Next(1, 4); operations > 0; operations--)
            {
                var paths = new List<string>(); var objects = new List<string>(); var arrays = new List<string>();
                CollectPaths(document, string.Empty, paths, objects, arrays);
                string path = paths[random.Next(paths.Count)];
                var current = JsonEdit.Resolve(document, path);
                switch (random.Next(7))
                {
                    case 0:
                        document = JsonEdit.Set(document, path, random.Next(4) switch
                        {
                            0 => JsonValue.Null,
                            1 => JsonValue.FromBoolean(random.Next(2) == 0),
                            2 => JsonValue.CreateArray(new JsonValue[0]),
                            _ => JsonValue.CreateObject(new JsonMember[0]),
                        });
                        break;
                    case 1:
                        document = JsonEdit.Remove(document, path);
                        break;
                    case 2:
                        document = JsonEdit.Add(document, objects[random.Next(objects.Count)], "zz" + (counter++).ToString(CultureInfo.InvariantCulture), JsonEdit.Int(random.Next(3)));
                        break;
                    case 3:
                        {
                            long[] numbers = { -1, 0, 1, 2, 3, 9, 10, 11, 32, 33, 1024, JsonValue.MaxSafeInteger };
                            long next = current.Kind == JsonKind.Integer && random.Next(2) == 0 ? current.IntegerValue + (random.Next(2) == 0 ? 1 : -1) : numbers[random.Next(numbers.Length)];
                            document = JsonEdit.Set(document, path, JsonEdit.Int(next));
                            break;
                        }
                    case 4:
                        document = JsonEdit.Set(document, path, JsonEdit.Str(RandomString(random, current.Kind == JsonKind.String ? current.StringValue : null)));
                        break;
                    case 5:
                        if (arrays.Count > 0)
                        {
                            string arrayPath = arrays[random.Next(arrays.Count)];
                            var items = new List<JsonValue>(JsonEdit.Resolve(document, arrayPath).Items);
                            switch (random.Next(4))
                            {
                                case 0: if (items.Count > 0) items.Add(items[random.Next(items.Count)]); break;
                                case 1: if (items.Count > 0) items.RemoveAt(random.Next(items.Count)); break;
                                case 2: items.Add(JsonValue.Null); break;
                                default: while (items.Count < 11 && items.Count > 0) items.Add(items[0]); break;
                            }
                            document = JsonEdit.Set(document, arrayPath, JsonValue.CreateArray(items));
                        }
                        break;
                    default:
                        {
                            string[] enums = { "N", "E", "S", "W", "TRACK_NS", "TRACK_WN", "OCCUPANCY", "NO_LOOP", "train-track-v1", "solver-v1", "S1-01-01-01", "level.s1.title", "a.b", "a-b", "proofs/x.json", new string('a', 64), new string('G', 64) };
                            document = JsonEdit.Set(document, path, JsonEdit.Str(enums[random.Next(enums.Length)]));
                            break;
                        }
                }
            }
            return document;
        }

        private static void AssertVerdictParity(string label, JsonValue schema, IReadOnlyList<JsonValue> bases, Func<JsonValue, bool> readerAccepts, int rounds, int seed)
        {
            var validator = ValidatorOf(schema);
            var random = new Random(seed);
            int accepted = 0, rejected = 0;
            for (int i = 0; i < rounds; i++)
            {
                var document = Mutate(bases[random.Next(bases.Count)], random);
                bool expected = validator.IsValid(schema, document);
                bool actual = readerAccepts(document);
                Assert.That(actual, Is.EqualTo(expected), label + " round " + i + ": schema=" + expected + " reader=" + actual + "\n" + Jcs(document));
                if (expected) accepted++; else rejected++;
            }
            Assert.That(accepted, Is.GreaterThan(rounds / 50), label + ": the generator must also produce valid mutants");
            Assert.That(rejected, Is.GreaterThan(rounds / 2), label + ": the generator must mostly produce invalid mutants");
        }

        [Test]
        public void LevelV2Reader_GivesTheSameVerdictAsTheSchema_ForRandomMutants()
        {
            AssertVerdictParity("level-v2", Schema(), Documents(), json => ReadV2(json).Document != null, 12000, 20260101);
        }

        [Test]
        public void LevelV1Reader_GivesTheSameVerdictAsTheLegacySchema_ForRandomMutants()
        {
            var bases = new[] { Examples.Json(Examples.V1), Examples.Json(Examples.V1Single),
                JsonEdit.Set(Examples.Json(Examples.V1), "/production/starThresholdsSeconds", Parse("{\"twoStars\":90,\"threeStars\":60,\"calibrationVersion\":\"calibration-1\"}")) };
            AssertVerdictParity("level-v1", Schema(V1Schema), bases, json => LevelV1Reader.Read(json).Document != null, 12000, 20260102);
        }

        [Test]
        public void EveryExamplePairIsAcceptedByItsOwnReaderAndRejectedByTheOther()
        {
            foreach (string file in Examples.V2Files)
            {
                Assert.That(ReadV2(Examples.Json(file)).Succeeded, Is.True, file);
                Assert.That(LevelV1Reader.Read(Examples.Json(file)).Document, Is.Null, file);
            }
            foreach (string file in Examples.V1Files)
            {
                Assert.That(LevelV1Reader.Read(Examples.Json(file)).Succeeded, Is.True, file);
                Assert.That(ReadV2(Examples.Json(file)).Document, Is.Null, file);
            }
        }

        // ---- helpers ---------------------------------------------------------------------------------------------

        private static JsonValue Walk(JsonValue root, string pointer)
        {
            var node = root;
            foreach (string token in pointer.Split('/')) node = node.Get(token) ?? throw new AssertionException("schema pointer not found: " + pointer);
            return node;
        }
    }
}
