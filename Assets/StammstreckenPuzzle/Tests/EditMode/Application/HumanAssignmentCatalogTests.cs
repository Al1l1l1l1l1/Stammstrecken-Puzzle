using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using STP.Application;

namespace STP.Tests.Application.EditMode
{
    public sealed class HumanAssignmentCatalogTests
    {
        private readonly HumanCalibrationTestFactory.TestCatalog source=new HumanCalibrationTestFactory.TestCatalog();
        private IPublicPuzzleCatalog Public=>new LocalSaveContractFixtures.PublicCatalog(new CatalogIdentity("qa-mobile-candidate-human-test",1,source.PublicCatalogHashSha256,CatalogUse.InternalPreview,CatalogApprovalStatus.Draft),source.Assignments.Select(a=>a.Puzzle).ToArray());
        private string Item(HumanAssignment a)=>LocalSaveFixtureJson.Object(("assignmentId",LocalSaveHash.Quote(a.Id)),("purpose","\"CAMPAIGN_TIME\""),("publicPuzzleInput",EndlessProjection.PublicInput(a.Puzzle,true)),("publicSourceSha256",LocalSaveHash.Quote(a.PublicSourceSha256)),("sourceSha256",LocalSaveHash.Quote(a.SourceSha256)),("contentRevision","1"),("profileId","null"),("profileHashSha256","null"));
        private string Manifest(string items)=>LocalSaveFixtureJson.Object(("schemaVersion","1"),("kind","\"HUMAN_CALIBRATION_ASSIGNMENTS\""),("approvalStatus","\"DRAFT\""),("applicationId","\"com.STP.StammstreckenPuzzle.human\""),("studyId","\"controlled-human-test\""),("revision","1"),("buildId","\"controlled-automated-test\""),("uiSourceSha256",LocalSaveHash.Quote(source.UiSourceSha256)),("publicCatalogHashSha256",LocalSaveHash.Quote(source.PublicCatalogHashSha256)),("assignments","["+items+"]"));
        private IHumanAssignmentCatalog Read(string text,string? hash=null,IPublicPuzzleCatalog? current=null)
        {byte[] bytes=Encoding.UTF8.GetBytes(text);try{return LocalSaveContractFixtures.Construct<IHumanAssignmentCatalog>("STP.Infrastructure.Content","STP.Infrastructure.Content.HumanAssignmentCatalog",bytes,hash??LocalSaveHash.Bytes(bytes),current??Public,LocalSaveContractFixtures.Json);}catch(System.Reflection.TargetInvocationException e)when(e.InnerException!=null){throw e.InnerException;}}
        [Test]
        public void DirectAssignedInputPreservesExactCurrentIdentityWithoutProgressOrProviderFields()
        {
            string manifest=Manifest(string.Join(",",source.Assignments.Select(Item)));var read=Read(manifest);Assert.That(read.Assignments.Count,Is.EqualTo(2));Assert.That(read.Assignments[0].Puzzle.Identity,Is.EqualTo(source.Assignments[0].Puzzle.Identity));Assert.That(read.ManifestBytesSha256,Is.EqualTo(LocalSaveHash.Bytes(Encoding.UTF8.GetBytes(manifest))));Assert.That(read.Assignments[0].ContentRevision,Is.EqualTo(1));Assert.That(read.Assignments[0].ProfileId,Is.Null);Assert.That(manifest,Does.Not.Contain("solution").And.Not.Contain("qualityNotes").And.Not.Contain("firstClear").And.Not.Contain("entitlement"));
        }
        [TestCase("raw-hash")]
        [TestCase("semantic-hash")]
        [TestCase("public-source")]
        [TestCase("extra-field")]
        [TestCase("missing-field")]
        [TestCase("duplicate-key")]
        [TestCase("version")]
        [TestCase("wrong-app")]
        [TestCase("corpus")]
        [TestCase("alias")]
        [TestCase("unknown-puzzle")]
        public void AssignmentRejectsUnboundSourcesAliasesMalformedAndUnknownInputs(string mutation)
        {
            string item=Item(source.Assignments[0]);string manifest=Manifest(item);
            if(mutation=="raw-hash"){Assert.Throws<ArgumentException>(()=>Read(manifest,new string('0',64)));return;}
            if(mutation=="semantic-hash")manifest=manifest.Replace(source.Assignments[0].Puzzle.Identity.HashSha256,new string('e',64));
            if(mutation=="public-source")manifest=manifest.Replace(source.Assignments[0].PublicSourceSha256,new string('e',64));
            if(mutation=="extra-field")manifest=manifest.Replace("\"schemaVersion\":1","\"schemaVersion\":1,\"solution\":[]");
            if(mutation=="missing-field")manifest=manifest.Replace("\"contentRevision\":1,","");
            if(mutation=="duplicate-key")manifest=manifest.Replace("\"schemaVersion\":1","\"schemaVersion\":1,\"schemaVersion\":1");
            if(mutation=="version")manifest=manifest.Replace("\"schemaVersion\":1","\"schemaVersion\":2");
            if(mutation=="wrong-app")manifest=manifest.Replace(".human\"",".qa\"");
            if(mutation=="corpus")manifest=manifest.Replace(source.PublicCatalogHashSha256,new string('e',64));
            if(mutation=="alias")manifest=Manifest(item+","+item.Replace("assigned-0","assigned-alias"));
            if(mutation=="unknown-puzzle"){var current=new LocalSaveContractFixtures.PublicCatalog(Public.Identity,new[]{source.Assignments[1].Puzzle});Assert.Throws<ArgumentException>(()=>Read(manifest,current:current));return;}
            Assert.Throws<ArgumentException>(()=>Read(manifest));
        }
    }
    internal static class LocalSaveFixtureJson
    {
        internal static string Object(params(string Name,string Value)[] fields)=>"{"+string.Join(",",fields.Select(f=>LocalSaveHash.Quote(f.Name)+":"+f.Value))+"}";
    }
}
