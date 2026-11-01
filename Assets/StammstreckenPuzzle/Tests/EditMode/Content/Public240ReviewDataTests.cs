using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using STP.Infrastructure.Content;

namespace STP.Tests.Content.EditMode
{
    public sealed class Public240ReviewDataTests
    {
        private static string Asset => Path.Combine(Environment.GetEnvironmentVariable("STP_SOURCE_ROOT") ?? Directory.GetCurrentDirectory(), "Assets/StammstreckenPuzzle/UI/Public240.public.json");
        private static JsonValue Document() => StrictJsonParser.Parse(File.ReadAllBytes(Asset)).Value!;
        private static byte[] Bytes(JsonValue value) => JcsSerializer.Serialize(value).Utf8!;
        private static JsonValue Change(JsonValue value, string name, JsonValue replacement) => JsonValue.CreateObject(
            value.Members.Where(member => member.Name != name).Concat(new[] { new JsonMember(name, replacement) }));
        private static JsonValue First(JsonValue root, Func<JsonValue,JsonValue> mutate) => Change(root,"puzzles",JsonValue.CreateArray(
            root.Get("puzzles")!.Items.Select((entry,index)=>index==0?mutate(entry):entry)));

        [Test]
        public void ActualAsset_All240DomainMappingsAndPublicHashesAreVerified_AndByteLockMatches()
        {
            byte[] raw = File.ReadAllBytes(Asset);
            Public240ReviewData.VerifyAssetBytes(raw);
            Assert.That(Public240ReviewData.ValidateAndCount(raw),Is.EqualTo(240));
            byte[] lf = Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(raw).Replace("\r\n", "\n"));
            Assert.That(LevelHashing.Sha256Hex(lf),Is.EqualTo("c21c9ef55ab34c369c3d6aa16c08c066a96ea9bcab4286ff73e3d402d33e17c2"));
            Public240ReviewData.VerifyAssetBytes(Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(lf).Replace("\n", "\r\n")));
            // Property order/formatting is not semantic identity; the separate serialized-asset byte gate is exact.
            Assert.That(Public240ReviewData.ValidateAndCount(Bytes(Document())),Is.EqualTo(240));
            Assert.Throws<ArgumentException>(()=>Public240ReviewData.VerifyAssetBytes(Bytes(Document())));
        }

        [TestCase("solution")][TestCase("notes")][TestCase("proofRef")][TestCase("production")][TestCase("content")][TestCase("completion")][TestCase("solutionHash")]
        public void HiddenKnowledgeOrUnknownFields_AreRejectedAtRootAndEveryPuzzle(string field)
        {
            var root = Document(); var injected=JsonValue.CreateString("MUST_NOT_SHIP");
            Assert.Throws<ArgumentException>(()=>Public240ReviewData.ValidateAndCount(Bytes(Change(root,field,injected))));
            Assert.Throws<ArgumentException>(()=>Public240ReviewData.ValidateAndCount(Bytes(First(root,entry=>Change(entry,field,injected)))));
            Assert.Throws<ArgumentException>(()=>Public240ReviewData.ValidateAndCount(Bytes(First(root,entry=>Change(entry,"grid",Change(entry.Get("grid")!,field,injected))))));
            Assert.Throws<ArgumentException>(()=>Public240ReviewData.ValidateAndCount(Bytes(First(root,entry=>Change(entry,"endpoints",Change(entry.Get("endpoints")!,field,injected))))));
        }

        [Test]
        public void MissingDuplicateForeignIdAndSameIdDifferentHash_AreRejected()
        {
            var root=Document();var entries=root.Get("puzzles")!.Items;
            Assert.Throws<ArgumentException>(()=>Public240ReviewData.ValidateAndCount(Bytes(Change(root,"puzzles",JsonValue.CreateArray(entries.Skip(1))))));
            Assert.Throws<ArgumentException>(()=>Public240ReviewData.ValidateAndCount(Bytes(Change(root,"puzzles",JsonValue.CreateArray(entries.Select((item,i)=>i==1?entries[0]:item))))));
            Assert.Throws<ArgumentException>(()=>Public240ReviewData.ValidateAndCount(Bytes(First(root,item=>Change(item,"puzzleId",JsonValue.CreateString("S1-99-01-01"))))));
            Assert.Throws<ArgumentException>(()=>Public240ReviewData.ValidateAndCount(Bytes(First(root,item=>Change(item,"publicPuzzleHash",Change(item.Get("publicPuzzleHash")!,"sha256",JsonValue.CreateString(new string('a',64))))))));
            Assert.Throws<ArgumentException>(()=>Public240ReviewData.ValidateAndCount(Bytes(First(root,item=>JsonValue.CreateObject(item.Members.Where(member=>member.Name!="rowCounts"))))));
        }

        [TestCase("schemaVersion", "2")][TestCase("revision", "0")][TestCase("use", "\"Production\"")][TestCase("status", "\"PRODUCT_APPROVED\"")][TestCase("catalogHashSha256", "\"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\"")]
        public void HeaderAndCatalogHashCannotBePromotedOrForged(string name,string value)
        {
            Assert.Throws<ArgumentException>(()=>Public240ReviewData.ValidateAndCount(Bytes(Change(Document(),name,StrictJsonParser.ParseText(value).Value!))));
        }

        [Test]
        public void ValidReversedEndpoints_WithRecomputedPuzzleAndCatalogHashes_CannotReplaceTheLockedCatalog()
        {
            var root=Document();var original=root.Get("puzzles")!.Items[0];var endpoints=original.Get("endpoints")!;
            // A/B reversal preserves the genuine valid grid/counts/ports, but is a different public identity.
            var swapped=Change(original,"endpoints",JsonValue.CreateObject(new[]{
                new JsonMember("a",endpoints.Get("b")!),new JsonMember("b",endpoints.Get("a")!)}));
            var projection=JsonValue.CreateObject(swapped.Members.Where(member=>member.Name!="publicPuzzleHash"));
            string publicHash=LevelHashing.HashProjection(projection);
            Assert.That(publicHash,Is.Not.EqualTo(original.Get("publicPuzzleHash")!.Get("sha256")!.StringValue));
            swapped=Change(swapped,"publicPuzzleHash",Change(swapped.Get("publicPuzzleHash")!,"sha256",JsonValue.CreateString(publicHash)));
            var changed=First(root,_=>swapped);
            string catalogHash=LevelHashing.HashProjection(JsonValue.CreateObject(changed.Members.Where(member=>member.Name!="catalogHashSha256")));
            changed=Change(changed,"catalogHashSha256",JsonValue.CreateString(catalogHash));
            Assert.That(catalogHash,Is.Not.EqualTo(root.Get("catalogHashSha256")!.StringValue));
            Assert.Throws<ArgumentException>(()=>Public240ReviewData.ValidateAndCount(Bytes(changed)),"Self-consistent hashes do not authorize replacing the source-locked catalog.");
        }

        [TestCase("{\"schemaVersion\":1,\"schemaVersion\":1}")][TestCase("{\"schemaVersion\":1.0}")][TestCase("{\"schemaVersion\":1e0}")][TestCase("{\"schemaVersion\":9007199254740992}")][TestCase("{\"x\":\"\\ud800\"}")]
        public void DuplicateFloatUnsafeIntegerAndUnicodeInputsNeverFallback(string json)
            => Assert.Throws<ArgumentException>(()=>Public240ReviewData.ValidateAndCount(Encoding.UTF8.GetBytes(json)));

        [Test]
        public void InvalidSideDimensionsCountsAndResourceBoundsFailClosed()
        {
            var root=Document();
            foreach(var replacement in new[]{JsonValue.CreateInteger(-1),JsonValue.CreateInteger(1),JsonValue.CreateInteger(33),JsonValue.CreateString("4")})
                Assert.Throws<ArgumentException>(()=>Public240ReviewData.ValidateAndCount(Bytes(First(root,e=>Change(e,"grid",Change(e.Get("grid")!,"width",replacement))))));
            Assert.Throws<ArgumentException>(()=>Public240ReviewData.ValidateAndCount(Bytes(First(root,e=>Change(e,"endpoints",Change(e.Get("endpoints")!,"a",Change(e.Get("endpoints")!.Get("a")!,"side",JsonValue.CreateString("north"))))))));
            Assert.Throws<ArgumentException>(()=>Public240ReviewData.ValidateAndCount(new byte[1024*1024+1]));
            Assert.Throws<ArgumentException>(()=>Public240ReviewData.ValidateAndCount(new byte[]{0xff}));
            Assert.Throws<ArgumentException>(()=>Public240ReviewData.ValidateAndCount(File.ReadAllBytes(Asset).Take(400).ToArray()));
        }
    }
}
