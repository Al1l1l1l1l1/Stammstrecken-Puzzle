using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using STP.Infrastructure.Content;

namespace STP.Tests.Content.EditMode
{
    public sealed class JcsSerializerTests
    {
        private static JsonValue Golden() => Examples.Json("Assets/StammstreckenPuzzle/Tests/EditMode/Content/level-v2-jcs-golden.json");

        [Test]
        public void GoldenCases_ProduceByteIdenticalCanonicalFormAndHash()
        {
            var cases = Golden().Get("jcsCases")!.Items;
            Assert.That(cases.Count, Is.GreaterThanOrEqualTo(12));
            foreach (var testCase in cases)
            {
                string name = testCase.Get("name")!.StringValue;
                var parsed = StrictJsonParser.ParseText(testCase.Get("inputJson")!.StringValue);
                Assert.That(parsed.Succeeded, Is.True, name + ": " + parsed.Error);
                var jcs = JcsSerializer.Serialize(parsed.Value);
                Assert.That(jcs.Succeeded, Is.True, name);
                Assert.That(Convert.ToBase64String(jcs.Utf8!), Is.EqualTo(testCase.Get("canonicalBase64")!.StringValue), name);
                Assert.That(LevelHashing.Sha256Hex(jcs.Utf8!), Is.EqualTo(testCase.Get("sha256")!.StringValue), name);
            }
        }

        [Test]
        public void Output_HasNoBomNoTrailingNewline_AndIsPlainUtf8()
        {
            foreach (var input in new[] { "{}", "[]", "{\"a\":\"\u00E9\"}", RepoFiles.Text(Examples.V2) })
            {
                var bytes = JcsSerializer.Serialize(StrictJsonParser.ParseText(input).Value).Utf8!;
                Assert.That(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF, Is.False);
                Assert.That(bytes[bytes.Length - 1], Is.Not.EqualTo((byte)'\n'));
            }
        }

        [Test]
        public void MemberOrderAndFormatting_DoNotChangeTheCanonicalBytes()
        {
            var a = StrictJsonParser.ParseText("{\"b\":[1,{\"y\":2,\"x\":1}],\"a\":null}").Value!;
            var b = StrictJsonParser.ParseText("\r\n{ \"a\" : null,\n \"b\" : [ 1 , { \"x\" : 1 , \"y\" : 2 } ] }\t").Value!;
            Assert.That(JcsSerializer.Serialize(a).Text, Is.EqualTo("{\"a\":null,\"b\":[1,{\"x\":1,\"y\":2}]}"));
            Assert.That(JcsSerializer.Serialize(b).Utf8, Is.EqualTo(JcsSerializer.Serialize(a).Utf8));
        }

        [Test]
        public void Keys_AreSortedByUtf16CodeUnits_NotByCodePoint()
        {
            var value = JsonValue.CreateObject(new[]
            {
                new JsonMember("\uFF5E", JsonValue.CreateInteger(1)),
                new JsonMember("\U0001F600", JsonValue.CreateInteger(2)),
                new JsonMember("a", JsonValue.CreateInteger(3)),
                new JsonMember("A", JsonValue.CreateInteger(4)),
                new JsonMember("", JsonValue.CreateInteger(5))
            });
            Assert.That(JcsSerializer.Serialize(value).Text, Is.EqualTo("{\"\":5,\"A\":4,\"a\":3,\"\U0001F600\":2,\"\uFF5E\":1}"));
        }

        [Test]
        public void StringEscaping_FollowsRfc8785()
        {
            var text = JsonValue.CreateString("\u0000\u0001\u001f\b\f\n\r\t\"\\/\u007f\u2028\u00E9\U0001F680");
            Assert.That(JcsSerializer.Serialize(text).Text, Is.EqualTo("\"\\u0000\\u0001\\u001f\\b\\f\\n\\r\\t\\\"\\\\/\u007f\u2028\u00E9\U0001F680\""));
        }

        [Test]
        public void Integers_AreWrittenAsPlainDecimal_AndBoundaryIsEnforced()
        {
            var array = JsonValue.CreateArray(new[] { 0L, -1L, 7L, JsonValue.MaxSafeInteger, -JsonValue.MaxSafeInteger }.Select(JsonValue.CreateInteger));
            Assert.That(JcsSerializer.Serialize(array).Text, Is.EqualTo("[0,-1,7,9007199254740991,-9007199254740991]"));
            Assert.Throws<ArgumentOutOfRangeException>(() => JsonValue.CreateInteger(JsonValue.MaxSafeInteger + 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => JsonValue.CreateInteger(-JsonValue.MaxSafeInteger - 1));
        }

        [Test]
        public void UnpairedSurrogates_AreRejectedInValuesAndKeys_WithAPath()
        {
            var inValue = JsonValue.CreateObject(new[] { new JsonMember("k", JsonValue.CreateString("a\ud800b")) });
            var lowInValue = JsonValue.CreateArray(new[] { JsonValue.CreateString("\udc00") });
            var inKey = JsonValue.CreateObject(new[] { new JsonMember("\ud800", JsonValue.Null) });
            var trailingHigh = JsonValue.CreateString("x\ud83d");
            foreach (var value in new[] { inValue, lowInValue, inKey, trailingHigh })
            {
                var result = JcsSerializer.Serialize(value);
                Assert.That(result.Succeeded, Is.False);
                Assert.That(result.Utf8, Is.Null);
                Assert.That(result.Error!.Code, Is.EqualTo(LevelDiagnosticCodes.JcsSurrogate));
            }
            Assert.That(JcsSerializer.Serialize(inValue).Error!.Path, Is.EqualTo("/k"));
            Assert.That(JcsSerializer.Serialize(JsonValue.CreateString("\uD83D\uDE80")).Succeeded, Is.True);
        }

        [Test]
        public void InvalidInputs_AreReportedNotThrown()
        {
            Assert.That(JcsSerializer.Serialize(null).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.JcsInvalidInput));
            Assert.Throws<ArgumentException>(() => JsonValue.CreateObject(new[] { new JsonMember("a", JsonValue.Null), new JsonMember("a", JsonValue.Null) }));
            Assert.Throws<ArgumentNullException>(() => new JsonMember("a", null!));
        }

        [Test]
        public void Canonicalisation_IsIdempotent_OverAllRepositoryExamples()
        {
            foreach (string file in Examples.V2Files.Concat(Examples.V1Files).Concat(new[] { Examples.Proof, Examples.ProofSingle, Examples.ReleaseLock }))
            {
                string canonical = JcsSerializer.Serialize(Examples.Json(file)).Text!;
                string again = JcsSerializer.Serialize(StrictJsonParser.ParseText(canonical).Value).Text!;
                Assert.That(again, Is.EqualTo(canonical), file);
            }
        }
    }
}
