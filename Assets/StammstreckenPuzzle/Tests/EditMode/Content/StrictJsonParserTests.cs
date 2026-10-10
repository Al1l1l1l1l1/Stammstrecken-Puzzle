using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using STP.Infrastructure.Content;

namespace STP.Tests.Content.EditMode
{
    public sealed class StrictJsonParserTests
    {
        private static JsonParseResult ParseAscii(string text, JsonParserLimits? limits = null) => StrictJsonParser.Parse(Encoding.UTF8.GetBytes(text), limits);

        [Test]
        public void Accepts_AllScalarKinds_NestedValues_AndInsignificantWhitespace()
        {
            var result = ParseAscii(" \t\r\n{ \"a\" : [ 1 , -2 , true , false , null , \"x\" ] , \"b\" : { } } \n");
            Assert.That(result.Succeeded, Is.True, result.Error?.ToString());
            var root = result.Value!;
            Assert.That(root.Kind, Is.EqualTo(JsonKind.Object));
            var a = root.Get("a")!;
            Assert.That(a.Items.Select(i => i.Kind), Is.EqualTo(new[] { JsonKind.Integer, JsonKind.Integer, JsonKind.Boolean, JsonKind.Boolean, JsonKind.Null, JsonKind.String }));
            Assert.That(a.Items[1].IntegerValue, Is.EqualTo(-2));
            Assert.That(root.Get("b")!.Members.Count, Is.EqualTo(0));
            Assert.That(root.Get("missing"), Is.Null);
        }

        [TestCase("0", 0L)]
        [TestCase("-1", -1L)]
        [TestCase("9007199254740991", 9007199254740991L)]
        [TestCase("-9007199254740991", -9007199254740991L)]
        public void Accepts_IntegersInsideTheSafeRange(string token, long expected)
        {
            var result = ParseAscii("[" + token + "]");
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Value!.Items[0].IntegerValue, Is.EqualTo(expected));
        }

        [Test]
        public void Accepts_TopLevelScalars()
        {
            Assert.That(ParseAscii("null").Value!.Kind, Is.EqualTo(JsonKind.Null));
            Assert.That(ParseAscii("true").Value!.BooleanValue, Is.True);
            Assert.That(ParseAscii("\"x\"").Value!.StringValue, Is.EqualTo("x"));
            Assert.That(ParseAscii("7").Value!.IntegerValue, Is.EqualTo(7));
        }

        [Test]
        public void Decodes_EscapesAndSurrogatePairs_AndKeepsUnicodeKeys()
        {
            var result = ParseAscii("{\"k\\u00e9\":\"\\u0041\\n\\\"\\\\\\/\\ud83d\\ude80\\u0000\"}");
            Assert.That(result.Succeeded, Is.True, result.Error?.ToString());
            var value = result.Value!.Get("k\u00E9")!.StringValue;
            Assert.That(value, Is.EqualTo("A\n\"\\/\U0001F680\0"));
            var raw = StrictJsonParser.Parse(new UTF8Encoding(false).GetBytes("[\"\U0001F680\u00E9\"]"));
            Assert.That(raw.Value!.Items[0].StringValue, Is.EqualTo("\U0001F680\u00E9"));
        }

        [Test]
        public void GoldenRejectCases_ProduceTheExpectedStableCode()
        {
            var golden = Examples.Json("Assets/StammstreckenPuzzle/Tests/EditMode/Content/level-v2-jcs-golden.json");
            var cases = golden.Get("rejectCases")!.Items;
            Assert.That(cases.Count, Is.GreaterThanOrEqualTo(30));
            foreach (var testCase in cases)
            {
                string name = testCase.Get("name")!.StringValue;
                byte[] input = Convert.FromBase64String(testCase.Get("inputBase64")!.StringValue);
                var result = StrictJsonParser.Parse(input);
                Assert.That(result.Value, Is.Null, name);
                Assert.That(result.Error, Is.Not.Null, name);
                Assert.That(result.Error!.Code, Is.EqualTo(testCase.Get("expectedCode")!.StringValue), name + ": " + result.Error);
                Assert.That(result.Error.Stage, Is.EqualTo(LevelDiagnosticStage.Parse), name);
            }
        }

        [Test]
        public void ParseText_RejectsBomAndRawLoneSurrogates_AndNull()
        {
            Assert.That(StrictJsonParser.ParseText("\uFEFF{}").Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseBom));
            Assert.That(StrictJsonParser.ParseText("[\"a\ud800b\"]").Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseSurrogate));
            Assert.That(StrictJsonParser.ParseText("[\"a\udc00b\"]").Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseSurrogate));
            Assert.That(StrictJsonParser.ParseText("[\"\ud800\"]").Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseSurrogate));
            Assert.That(StrictJsonParser.ParseText(null).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseEmpty));
            Assert.That(StrictJsonParser.ParseText("").Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseEmpty));
            Assert.That(StrictJsonParser.Parse(null).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseEmpty));
            Assert.That(StrictJsonParser.ParseText("[\"\uD83D\uDE80\"]").Succeeded, Is.True);
        }

        [Test]
        public void DuplicateKeys_AreRejectedAtEveryNestingLevel_WithTheMemberPath()
        {
            var result = ParseAscii("{\"outer\":{\"inner\":[{\"x\":1,\"x\":2}]}}");
            Assert.That(result.Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseDuplicateKey));
            Assert.That(result.Error.Path, Is.EqualTo("/outer/inner/0/x"));
            Assert.That(ParseAscii("{\"a\":{\"x\":1},\"b\":{\"x\":2}}").Succeeded, Is.True, "equal names in different objects are not duplicates");
        }

        [Test]
        public void Limits_AreEnforcedExactlyAtTheirBoundary()
        {
            var limits = new JsonParserLimits(maxBytes: 64, maxDepth: 3, maxStringLength: 5, maxArrayLength: 3, maxObjectMembers: 2);
            Assert.That(ParseAscii("[[[1]]]", limits).Succeeded, Is.True, "depth 3 is allowed");
            Assert.That(ParseAscii("[[[[1]]]]", limits).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseDepth));
            Assert.That(ParseAscii("{\"a\":{\"b\":{}}}", limits).Succeeded, Is.True);
            Assert.That(ParseAscii("{\"a\":{\"b\":{\"c\":{}}}}", limits).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseDepth));
            Assert.That(ParseAscii("[\"abcde\"]", limits).Succeeded, Is.True, "string length 5 is allowed");
            Assert.That(ParseAscii("[\"abcdef\"]", limits).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseStringLength));
            Assert.That(ParseAscii("{\"abcdef\":1}", limits).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseStringLength), "keys are limited like strings");
            Assert.That(ParseAscii("[1,2,3]", limits).Succeeded, Is.True);
            Assert.That(ParseAscii("[1,2,3,4]", limits).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseArrayLength));
            Assert.That(ParseAscii("{\"a\":1,\"b\":2}", limits).Succeeded, Is.True);
            Assert.That(ParseAscii("{\"a\":1,\"b\":2,\"c\":3}", limits).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseObjectMembers));
            string exact = "[" + new string('1', 62) + "]";
            Assert.That(exact.Length, Is.EqualTo(64));
            Assert.That(ParseAscii(exact, new JsonParserLimits(64, 3, 5, 3, 2)).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseNumberRange), "exactly at the byte limit is parsed (and then fails on its content)");
            Assert.That(ParseAscii(" " + exact, new JsonParserLimits(64, 3, 5, 3, 2)).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseSize), "one byte over the limit");
        }

        [Test]
        public void DefaultLimits_AreTheDocumentedImplementationLimits()
        {
            var limits = JsonParserLimits.Default;
            Assert.That(new[] { limits.MaxBytes, limits.MaxDepth, limits.MaxStringLength, limits.MaxArrayLength, limits.MaxObjectMembers }, Is.EqualTo(new[] { 262144, 16, 8192, 1024, 64 }));
            string deep = new string('[', 16) + new string(']', 16);
            Assert.That(ParseAscii(deep).Succeeded, Is.True);
            Assert.That(ParseAscii("[" + deep + "]").Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseDepth));
            Assert.That(ParseAscii("[\"" + new string('a', 8192) + "\"]").Succeeded, Is.True);
            Assert.That(ParseAscii("[\"" + new string('a', 8193) + "\"]").Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseStringLength));
            Assert.That(ParseAscii("[" + string.Join(",", Enumerable.Repeat("0", 1024)) + "]").Succeeded, Is.True);
            Assert.That(ParseAscii("[" + string.Join(",", Enumerable.Repeat("0", 1025)) + "]").Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseArrayLength));
            Assert.That(StrictJsonParser.Parse(new byte[262145]).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseSize));
            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonParserLimits(0, 1, 1, 1, 1));
        }

        // ---- QC finding 2: the input size limit is a limit in UTF-8 bytes on every entry ---------------------------

        [TestCase("[\"abc\"]")]
        [TestCase("[\"\u00E9\u00E9\u00E9\"]")]
        [TestCase("[\"\u20AC\u20AC\u20AC\"]")]
        [TestCase("[\"\U0001F680\U0001F680\"]")]
        [TestCase("[\"a\u00E9\u20AC\U0001F680\"]")]
        public void TheSizeLimit_IsMeasuredInUtf8Bytes_AndTextAndByteEntriesAgreeExactlyAtTheBoundary(string text)
        {
            int bytes = new UTF8Encoding(false).GetByteCount(text);
            var atLimit = new JsonParserLimits(bytes, 16, 8192, 1024, 64);
            var belowLimit = new JsonParserLimits(bytes - 1, 16, 8192, 1024, 64);
            Assert.That(StrictJsonParser.Parse(new UTF8Encoding(false).GetBytes(text), atLimit).Succeeded, Is.True, "bytes at the limit");
            Assert.That(StrictJsonParser.ParseText(text, atLimit).Succeeded, Is.True, "text at the limit");
            Assert.That(StrictJsonParser.Parse(new UTF8Encoding(false).GetBytes(text), belowLimit).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseSize), "bytes one over");
            Assert.That(StrictJsonParser.ParseText(text, belowLimit).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseSize), "text one over");
        }

        [Test]
        public void MultiByteText_ThatFitsInCharactersButNotInBytes_IsRejectedLikeTheByteEntry()
        {
            // Reproduction of the QC finding with the default limits: 20 strings of 8000 x U+20AC (every string is within
            // the 8192 string limit, the array within 1024) are about 160 thousand characters (below 256 KiB) but about
            // 480 thousand UTF-8 bytes (above 256 KiB). Before the fix the text entry accepted this document.
            string text = "[" + string.Join(",", Enumerable.Repeat("\"" + new string('\u20AC', 8000) + "\"", 20)) + "]";
            Assert.That(text.Length, Is.LessThan(JsonParserLimits.Default.MaxBytes));
            byte[] bytes = new UTF8Encoding(false).GetBytes(text);
            Assert.That(bytes.Length, Is.GreaterThan(JsonParserLimits.Default.MaxBytes));
            var byteEntry = StrictJsonParser.Parse(bytes);
            var textEntry = StrictJsonParser.ParseText(text);
            Assert.That(byteEntry.Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseSize));
            Assert.That(textEntry.Error, Is.Not.Null, "the text entry must not accept what the byte entry rejects for size");
            Assert.That(textEntry.Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseSize));
            Assert.That(textEntry.Error.Message, Does.Contain("bytes"));

            // The same document within the byte limit is accepted by both entries.
            string small = "[" + string.Join(",", Enumerable.Repeat("\"" + new string('\u20AC', 8000) + "\"", 10)) + "]";
            Assert.That(new UTF8Encoding(false).GetByteCount(small), Is.LessThanOrEqualTo(JsonParserLimits.Default.MaxBytes));
            Assert.That(StrictJsonParser.ParseText(small).Succeeded, Is.True);
            Assert.That(StrictJsonParser.Parse(new UTF8Encoding(false).GetBytes(small)).Succeeded, Is.True);
        }

        [Test]
        public void TheTextSizeCheck_IsAFirstGate_AndNeverThrowsForLoneSurrogates()
        {
            var tiny = new JsonParserLimits(4, 16, 8192, 1024, 64);
            Assert.That(StrictJsonParser.ParseText("[\"\ud800\"]", tiny).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseSize), "size is decided before content, like the byte entry");
            Assert.That(StrictJsonParser.ParseText("[\"\ud800\"]").Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseSurrogate));
        }

        // ---- QC finding 4: the string length limit holds across surrogate pairs ----------------------------------

        [TestCase("[\"ab\U0001F680\"]", true)]
        [TestCase("[\"abc\U0001F680\"]", false)]
        [TestCase("[\"ab\\ud83d\\ude80\"]", true)]
        [TestCase("[\"abc\\ud83d\\ude80\"]", false)]
        [TestCase("[\"\U0001F680\U0001F680\"]", true)]
        [TestCase("[\"a\U0001F680\U0001F680\"]", false)]
        [TestCase("{\"ab\U0001F680\":1}", true)]
        [TestCase("{\"abc\U0001F680\":1}", false)]
        public void AStringIsBoundedInUtf16CodeUnits_ASurrogatePairCountsTwo(string text, bool accepted)
        {
            var limits = new JsonParserLimits(1024, 16, 4, 1024, 64);
            foreach (var result in new[] { StrictJsonParser.ParseText(text, limits), StrictJsonParser.Parse(new UTF8Encoding(false).GetBytes(text), limits) })
            {
                if (accepted) Assert.That(result.Succeeded, Is.True, result.Error?.ToString());
                else Assert.That(result.Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseStringLength), text);
            }
        }

        [Test]
        public void TheDefaultStringLimit_CannotBeExceededByAFinalSurrogatePair()
        {
            string fits = "[\"" + new string('a', 8190) + "\U0001F680\"]";
            string over = "[\"" + new string('a', 8191) + "\U0001F680\"]";
            string escapedOver = "[\"" + new string('a', 8191) + "\\ud83d\\ude80\"]";
            Assert.That(StrictJsonParser.ParseText(fits).Succeeded, Is.True);
            Assert.That(StrictJsonParser.ParseText(fits).Value!.Items[0].StringValue.Length, Is.EqualTo(8192));
            Assert.That(StrictJsonParser.ParseText(over).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseStringLength));
            Assert.That(StrictJsonParser.ParseText(escapedOver).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseStringLength));
        }

        [Test]
        public void DeepNesting_NeverOverflowsTheStack()
        {
            string hostile = new string('[', 200000);
            Assert.That(StrictJsonParser.ParseText(hostile, new JsonParserLimits(1000000, 16, 8192, 1024, 64)).Error!.Code, Is.EqualTo(LevelDiagnosticCodes.ParseDepth));
        }

        [Test]
        public void Parsing_IsDeterministic_AndOrderPreserving()
        {
            string text = RepoFiles.Text(Examples.V2);
            var first = StrictJsonParser.ParseText(text).Value!;
            var second = StrictJsonParser.ParseText(text).Value!;
            Assert.That(JcsSerializer.Serialize(first).Text, Is.EqualTo(JcsSerializer.Serialize(second).Text));
            Assert.That(first.Members.Select(m => m.Name).First(), Is.EqualTo("documentSchemaVersion"), "document order is preserved by the DOM");
        }

        [Test]
        public void Fuzz_RandomBytes_AndSingleByteMutations_NeverThrow_AndAlwaysReturnExactlyOneOutcome()
        {
            var random = new Random(20261009);
            byte[] baseline = RepoFiles.Bytes(Examples.V2);
            for (int round = 0; round < 3000; round++)
            {
                byte[] input;
                if (round % 2 == 0)
                {
                    input = new byte[random.Next(0, 64)];
                    random.NextBytes(input);
                }
                else
                {
                    input = (byte[])baseline.Clone();
                    int flips = random.Next(1, 4);
                    for (int i = 0; i < flips; i++) input[random.Next(input.Length)] = (byte)random.Next(256);
                }
                var result = StrictJsonParser.Parse(input);
                Assert.That((result.Value == null) != (result.Error == null), Is.True, "exactly one of value/error, round " + round);
                if (result.Error != null) Assert.That(result.Error.Code, Does.StartWith("LVL-PARSE-"));
                else Assert.That(JcsSerializer.Serialize(result.Value).Succeeded, Is.True, "every parsed value is canonicalisable, round " + round);
            }
        }

        [Test]
        public void Fuzz_TokenSoup_NeverThrows()
        {
            string[] tokens = { "{", "}", "[", "]", ",", ":", "\"a\"", "\"\\u00", "\\", "1", "-", "0", "1.5", "e3", "true", "nul", "/", "//", "\u00E9", "\ud800", " ", "\n", "\"" };
            var random = new Random(7);
            for (int round = 0; round < 5000; round++)
            {
                var builder = new StringBuilder();
                int count = random.Next(1, 14);
                for (int i = 0; i < count; i++) builder.Append(tokens[random.Next(tokens.Length)]);
                var result = StrictJsonParser.ParseText(builder.ToString());
                Assert.That((result.Value == null) != (result.Error == null), Is.True, builder.ToString());
            }
        }
    }
}
