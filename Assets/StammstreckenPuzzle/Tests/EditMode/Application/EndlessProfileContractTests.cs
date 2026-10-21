using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using STP.Application;

namespace STP.Tests.Application.EditMode
{
    public sealed class EndlessProfileContractTests
    {
        private static string Root=>Environment.GetEnvironmentVariable("STP_SOURCE_ROOT")??Directory.GetCurrentDirectory();
        private static string PublicSource=>File.ReadAllText(Path.Combine(Root,"Assets/StammstreckenPuzzle/UI/MobileCandidate.public.json"));
        private static string ProfileSource=>File.ReadAllText(Path.Combine(Root,"Content/Generator/path-search-v1.profiles.json"));
        private static IPublicPuzzleCatalog Corpus()
        {
            string source=PublicSource;string hash=Regex.Match(source,"\"catalogHashSha256\"\\s*:\\s*\"([a-f0-9]{64})\"").Groups[1].Value;
            return LocalSaveContractFixtures.Construct<IPublicPuzzleCatalog>("STP.Infrastructure.Content","STP.Infrastructure.Content.MobileCandidateData",Encoding.UTF8.GetBytes(source),hash);
        }
        private static object Read(string source,IPublicPuzzleCatalog corpus)=>LocalSaveContractFixtures.Construct<object>("STP.Infrastructure.Content","STP.Infrastructure.Content.LockedEndlessProfiles",Encoding.UTF8.GetBytes(source),corpus);
        [Test]
        public void ActualFourProfilesAreCompleteLockedDraftTargetsAndDescriptorHashIsStable()
        {
            var corpus=Corpus();var reader=Read(ProfileSource,corpus);var profiles=(System.Collections.Generic.IReadOnlyList<EndlessSearchProfile>)LocalSaveContractFixtures.Get(reader,"Profiles")!;
            Assert.That(profiles.Select(p=>p.Id),Is.EqualTo(new[]{"dienstbeginn","regelbetrieb","stoerungslage","grosslage"}));
            Assert.That(profiles.Select(p=>p.Search.MinSize),Is.EqualTo(new[]{4,5,6,8}));Assert.That(profiles.Select(p=>p.Search.MaxSize),Is.EqualTo(new[]{5,6,8,10}));
            foreach(var profile in profiles)
            {
                var parameters=new SearchGeneratorParameters(profile.Id,profile.HashSha256,PuzzleCandidateSearch.NoveltyVersion,corpus.Identity,Array.Empty<PublicPuzzleInput>(),profile.Search);
                string hash=LocalSaveHash.Bytes(LocalSaveHash.Canonical(LocalSaveContractFixtures.Json,parameters.Projection()));
                var first=LocalEndlessDescriptor.Create("train-track-v1",PuzzleCandidateSearch.GeneratorVersion,"1","1",parameters,hash);
                var second=LocalEndlessDescriptor.Create("train-track-v1",PuzzleCandidateSearch.GeneratorVersion,"1","1",parameters,hash);
                Assert.That(EndlessProjection.DescriptorId(first,LocalSaveContractFixtures.Json),Is.EqualTo(EndlessProjection.DescriptorId(second,LocalSaveContractFixtures.Json)));
                var other=LocalEndlessDescriptor.Create("train-track-v1",PuzzleCandidateSearch.GeneratorVersion,"1","2",parameters,hash);
                Assert.That(EndlessProjection.DescriptorId(first,LocalSaveContractFixtures.Json),Is.Not.EqualTo(EndlessProjection.DescriptorId(other,LocalSaveContractFixtures.Json)));
            }
        }
        [Test]
        public void ProfileReaderRejectsPromotionMissingSolverWrongCorpusUnknownFieldsAndHashMutation()
        {
            var corpus=Corpus();string source=ProfileSource;
            foreach(string changed in new[]{source.Replace("DRAFT","PRODUCT_APPROVED"),source.Replace("solver-v2","invented-solver"),source.Replace(corpus.Identity.HashSha256,new string('0',64)),source.Replace("\"parameters\":","\"unknown\":0,\"parameters\":"),source.Replace("\"minSize\": 4","\"minSize\": 5")})
                Assert.Throws<TargetInvocationException>(()=>Read(changed,corpus));
            var production=new LocalSaveContractFixtures.PublicCatalog(new CatalogIdentity("production-public",1,corpus.Identity.HashSha256,CatalogUse.Production,CatalogApprovalStatus.ProductApproved),corpus.Puzzles.ToArray());
            Assert.Throws<TargetInvocationException>(()=>Read(source,production));
        }
    }
}
