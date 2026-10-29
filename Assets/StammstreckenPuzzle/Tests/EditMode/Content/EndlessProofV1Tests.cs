using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Infrastructure.Content;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Content.EditMode
{
    /// <summary>Separate E1 grammar, unchanged proof projection and real descriptor/public/solution binding without a new test assembly edge.</summary>
    public sealed class EndlessProofV1Tests
    {
        private static readonly Lazy<Fixture> Actual=new Lazy<Fixture>(()=>new Fixture());
        private static JsonValue Baseline=>Actual.Value.Proof.Document!.ToJson();
        private static JsonValue Parse(string text)=>StrictJsonParser.ParseText(text).Value!;
        private static byte[] Bytes(JsonValue json)=>JcsSerializer.Serialize(json).Utf8!;
        private static JsonValue Rehash(JsonValue value)=>JsonEdit.Set(value,"/proofHash",LevelHashing.ComputeProofHash(JsonValue.CreateObject(value.Members.Where(m=>m.Name!="proofHash").ToList())).ToJson());
        private static void Reject(ProofReadResult result,string code)
        {Assert.That(result.Succeeded,Is.False);Assert.That(Codes.Of(result.Diagnostics),Does.Contain(code));}

        [Test]
        public void ActualGeneratedCandidate_SeparateFreshNoGuessSolve_BindsAndMatchesUnchangedRuntimePayload()
        {
            var f=Actual.Value;Assert.That(f.Id,Does.StartWith("E1-"));Assert.That(f.Id.Length,Is.EqualTo(67));Assert.That(f.Proof.Succeeded,Is.True,Codes.Describe(f.Proof.Diagnostics));
            Assert.That(f.Fresh.Classification,Is.EqualTo(SolverClassification.UNIQUE));Assert.That(f.Fresh.SolverVersion,Is.EqualTo("solver-v2"));Assert.That(f.Fresh.Metrics!.RequiredGuessDepth,Is.Zero);
            Assert.That(f.Validate(f.Proof),Is.Empty);Assert.That(EndlessProofV1Codec.Read(f.Proof.CanonicalBytes).CanonicalBytes,Is.EqualTo(f.Proof.CanonicalBytes));
            var m=f.Fresh.Metrics;
            // Exact existing LocalEndlessPolicy payload, independently serialized in its existing field order.
            string Ref(string profile,string sha)=>"{\"profile\":\""+profile+"\",\"sha256\":\""+sha+"\"}";
            var runtime="{\"proofFormatVersion\":1,\"puzzleId\":\""+f.Id+"\",\"publicPuzzleHash\":"+Ref(LevelHashProfiles.PuzzleSemantic,f.PublicHash.Sha256)+",\"solutionHash\":"+Ref(LevelHashProfiles.Solution,f.SolutionHash.Sha256)+",\"solverVersion\":\"solver-v2\",\"solutionCount\":1,\"metrics\":{\"searchNodes\":"+m.SearchNodes+",\"deductionSteps\":"+m.DeductionSteps+",\"maxDeductionDepth\":"+m.MaxDeductionDepth+",\"requiredGuessDepth\":"+m.RequiredGuessDepth+"}}";
            Assert.That(Bytes(Parse(runtime)),Is.EqualTo(Bytes(f.Proof.Document!.Projection())));Assert.That(LevelHashing.HashProjection(Parse(runtime)),Is.EqualTo(f.Proof.Document.ProofHash.Sha256));
            Assert.That(ProofV1Codec.Read(f.Proof.CanonicalBytes).Succeeded,Is.False,"Campaign reader must retain its S-only guarantee.");
            Assert.That(ProofV1Codec.Create(f.Id,f.PublicHash,f.SolutionHash,"solver-v2",m.SearchNodes,m.DeductionSteps,m.MaxDeductionDepth,0).Succeeded,Is.False);
        }

        [Test]
        public void IdentitySpacesRemainDisjoint_AndEndlessSchemaChangesOnlyItsOwnIdentityEntry()
        {
            var campaign=RepoFiles.Bytes("Assets/StammstreckenPuzzle/Tests/EditMode/Content/proof-v1-solver-v2-root.json");Assert.That(ProofV1Codec.Read(campaign).Succeeded,Is.True);Assert.That(EndlessProofV1Codec.Read(campaign).Succeeded,Is.False);
            foreach(var id in new[]{"S1-01-01-01","E1-"+new string('A',64),"e1-"+new string('a',64),"E1-"+new string('a',63),"E1-"+new string('a',65),"E1-"+new string('g',64),"E1-"+new string('a',64)+"\n"})
                Reject(EndlessProofV1Codec.Read(Rehash(JsonEdit.Set(Baseline,"/puzzleId",JsonEdit.Str(id)))),"PRF-SCHEMA-PATTERN");
            string relative="ARCHITECTURE/schemas/endless-proof-v1.schema.json";var overlay=Environment.GetEnvironmentVariable("STP_CONTENT_STAGED_ROOT");var path=Path.Combine(overlay??RepoFiles.Root(),relative);
            var schema=StrictJsonParser.Parse(File.ReadAllBytes(path)).Value!;var original=Examples.Json("ARCHITECTURE/schemas/proof-v1.schema.json");
            Assert.That(schema.Get("properties")!.Get("puzzleId")!.Get("pattern")!.StringValue,Is.EqualTo("^E1-[0-9a-f]{64}$"));
            Assert.That(schema.Get("properties")!.Get("puzzleId")!.Get("minLength")!.IntegerValue,Is.EqualTo(67));Assert.That(schema.Get("properties")!.Get("puzzleId")!.Get("maxLength")!.IntegerValue,Is.EqualTo(67),"An explicit length also rejects trailing newlines that regex dollar anchors can otherwise accept.");
            foreach(var member in original.Members.Where(m=>m.Name!="$id"&&m.Name!="title"&&m.Name!="properties"))Assert.That(Bytes(schema.Get(member.Name)!),Is.EqualTo(Bytes(member.Value)),member.Name);
            foreach(var member in original.Get("properties")!.Members.Where(m=>m.Name!="puzzleId"))Assert.That(Bytes(schema.Get("properties")!.Get(member.Name)!),Is.EqualTo(Bytes(member.Value)),member.Name);
        }

        [Test]
        public void ClosedObjectsRejectEveryMissingExtraOrWrongTypedMember_AndParserRejectsDuplicates()
        {
            foreach(var path in new[]{"","/metrics","/publicPuzzleHash","/solutionHash","/proofHash"})
            {
                Reject(EndlessProofV1Codec.Read(JsonEdit.Add(Baseline,path,"unexpected",JsonEdit.Int(0))),"PRF-SCHEMA-UNKNOWN-PROPERTY");
                foreach(var member in JsonEdit.Resolve(Baseline,path).Members)
                {
                    Assert.That(EndlessProofV1Codec.Read(JsonEdit.Remove(Baseline,path+"/"+member.Name)).Succeeded,Is.False,path+"/"+member.Name);
                    Assert.That(EndlessProofV1Codec.Read(JsonEdit.Set(Baseline,path+"/"+member.Name,JsonValue.Null)).Succeeded,Is.False,path+"/"+member.Name);
                }
            }
            foreach(var malformed in new[]{"","{}","{","[]","null","true","{\"puzzleId\":\"one\",\"puzzleId\":\"two\"}","\uFEFF{}"})Assert.That(EndlessProofV1Codec.Read(Encoding.UTF8.GetBytes(malformed)).Succeeded,Is.False);
            Assert.That(EndlessProofV1Codec.Read(new byte[]{0xff}).Succeeded,Is.False);Assert.That(EndlessProofV1Codec.Read((byte[]?)null).Diagnostics.Single().Code,Is.EqualTo(ProofDiagnosticCodes.Missing));
            var text=Encoding.UTF8.GetString(Bytes(Baseline));var duplicated=text.Replace("\"metrics\":{","\"metrics\":{\"searchNodes\":0,");Assert.That(Codes.Of(EndlessProofV1Codec.Read(Encoding.UTF8.GetBytes(duplicated)).Diagnostics),Does.Contain("PRF-PARSE-DUPLICATE-KEY"));
            Reject(EndlessProofV1Codec.Read(JsonEdit.Set(Baseline,"/proofFormatVersion",JsonEdit.Int(2))),ProofDiagnosticCodes.Format);
            Reject(EndlessProofV1Codec.Read(JsonEdit.Set(Baseline,"/solutionCount",JsonEdit.Int(2))),ProofDiagnosticCodes.NotUnique);
        }

        [TestCase("searchNodes")][TestCase("deductionSteps")][TestCase("maxDeductionDepth")][TestCase("requiredGuessDepth")]
        public void MetricsUseSameSafeIntegerContract_AndTamperRequiresExactSelfHash(string metric)
        {
            foreach(var value in new[]{0L,1L,JsonValue.MaxSafeInteger})Assert.That(EndlessProofV1Codec.Read(Rehash(JsonEdit.Set(Baseline,"/metrics/"+metric,JsonEdit.Int(value)))).Succeeded,Is.True);
            Assert.That(EndlessProofV1Codec.Read(Rehash(JsonEdit.Set(Baseline,"/metrics/"+metric,JsonEdit.Int(-1)))).Succeeded,Is.False);
            var old=JsonEdit.Resolve(Baseline,"/metrics/"+metric).IntegerValue.ToString(System.Globalization.CultureInfo.InvariantCulture);var text=Encoding.UTF8.GetString(Bytes(Baseline));
            foreach(var bad in new[]{"0.0","9007199254740992"})Assert.That(EndlessProofV1Codec.Read(Encoding.UTF8.GetBytes(text.Replace("\""+metric+"\":"+old,"\""+metric+"\":"+bad))).Succeeded,Is.False);
            Reject(EndlessProofV1Codec.Read(JsonEdit.Set(Baseline,"/metrics/"+metric,JsonEdit.Int(9999))),ProofDiagnosticCodes.Hash);
        }

        [TestCase("publicPuzzleHash")][TestCase("solutionHash")][TestCase("proofHash")]
        public void HashSlotsRejectWrongProfilesAndNonLowercaseDigests(string slot)
        {
            var changed=JsonEdit.Set(Baseline,"/"+slot+"/profile",JsonEdit.Str("UNKNOWN"));
            Reject(EndlessProofV1Codec.Read(slot=="proofHash"?changed:Rehash(changed)),ProofDiagnosticCodes.Profile);
            foreach(var hash in new[]{new string('A',64),new string('a',63),new string('g',64)})Assert.That(EndlessProofV1Codec.Read(JsonEdit.Set(Baseline,"/"+slot+"/sha256",JsonEdit.Str(hash))).Succeeded,Is.False);
        }

        [TestCase("seed")][TestCase("ordinal")][TestCase("parameters")][TestCase("recent")]
        public void DescriptorRecomputationRejectsForeignOrInconsistentFullInputs(string mutation)
        {
            var f=Actual.Value;var descriptor=f.MutateDescriptor(mutation);var errors=f.Validate(f.Proof,descriptor);
            Assert.That(errors,Is.Not.Empty);Assert.That(Codes.Of(errors),Does.Contain(mutation=="parameters"||mutation=="recent"?EndlessProofDiagnosticCodes.Descriptor:ProofDiagnosticCodes.Puzzle));
        }

        [Test]
        public void BindingRejectsForeignPublicOrSolutionEvenAfterProofSelfRehash_AndRequiresV2NoGuess()
        {
            var f=Actual.Value;
            foreach(var slot in new[]{"publicPuzzleHash","solutionHash"})
            {
                var proof=EndlessProofV1Codec.Read(Rehash(JsonEdit.Set(Baseline,"/"+slot+"/sha256",JsonEdit.Str(new string('0',64)))));Assert.That(proof.Succeeded,Is.True);
                Assert.That(Codes.Of(f.Validate(proof)),Does.Contain(slot=="publicPuzzleHash"?ProofDiagnosticCodes.Puzzle:ProofDiagnosticCodes.Solution));
            }
            Assert.That(Codes.Of(f.Validate(f.Proof,puzzle:f.WrongPublicIdentity())),Does.Contain(ProofDiagnosticCodes.Puzzle));
            Assert.That(Codes.Of(f.Validate(f.Proof,solution:new ProfiledHash(LevelHashProfiles.Solution,new string('0',64)))),Does.Contain(ProofDiagnosticCodes.Solution));
            Assert.That(Codes.Of(f.Validate(f.Proof,solution:f.PublicHash)),Does.Contain(ProofDiagnosticCodes.Profile));
            var old=EndlessProofV1Codec.Read(Rehash(JsonEdit.Set(Baseline,"/solverVersion",JsonEdit.Str("solver-v1"))));Assert.That(old.Succeeded,Is.True);Assert.That(Codes.Of(f.Validate(old)),Does.Contain(ProofDiagnosticCodes.SolverVersion));
            Reject(EndlessProofV1Codec.Read(Rehash(JsonEdit.Set(Baseline,"/solverVersion",JsonEdit.Str("solver-v99")))),ProofDiagnosticCodes.SolverUnknown);
            var guessing=EndlessProofV1Codec.Read(Rehash(JsonEdit.Set(Baseline,"/metrics/requiredGuessDepth",JsonEdit.Int(1))));Assert.That(guessing.Succeeded,Is.True);Assert.That(Codes.Of(f.Validate(guessing)),Does.Contain(EndlessProofDiagnosticCodes.GuessDepth));
        }

        [Test]
        public void ReadAndBindingDoNotPretendToRunFreshSolver_AndResultsRemainImmutable()
        {
            var f=Actual.Value;var tampered=EndlessProofV1Codec.Read(Rehash(JsonEdit.Set(Baseline,"/metrics/deductionSteps",JsonEdit.Int(f.Fresh.Metrics!.DeductionSteps+1))));
            Assert.That(tampered.Succeeded,Is.True);Assert.That(f.Validate(tampered),Is.Empty,"Structural/identity binding has no hidden fresh-solver authority.");
            var fresh=PuzzleSolver.Solve(f.Definition,new SolverLimits(10000000,60000));Assert.That(fresh.Classification,Is.EqualTo(SolverClassification.UNIQUE));Assert.That(fresh.Metrics!.DeductionSteps,Is.Not.EqualTo(tampered.Document!.Metrics.DeductionSteps));
            var original=f.Proof.CanonicalBytes!;f.Proof.CanonicalBytes![0]=0;Assert.That(f.Proof.CanonicalBytes,Is.EqualTo(original));
            var errors=f.Validate(tampered,solution:f.PublicHash);Assert.Throws<NotSupportedException>(()=>((IList<LevelDiagnostic>)errors).Clear());
            Assert.That(Bytes(f.Proof.Document!.Projection()).Length,Is.GreaterThan(0));Assert.That(f.Proof.Document.Projection().Members.Count,Is.EqualTo(7));
        }

        private sealed class Fixture
        {
            private readonly Assembly app=Assembly.Load("STP.Application");private readonly object parameters,corpusIdentity,search,profile;internal readonly object Descriptor,Puzzle;
            internal readonly string Id;internal readonly ProfiledHash PublicHash,SolutionHash;internal readonly ProofReadResult Proof;internal readonly PuzzleDefinition Definition;internal readonly SolverResult Fresh;
            internal Fixture()
            {
                var binding=Examples.Json("Assets/StammstreckenPuzzle/UI/MobileCandidate.bindings.json");var corpus=new MobileCandidateData(RepoFiles.Bytes("Assets/StammstreckenPuzzle/UI/MobileCandidate.public.json"),binding.Get("publicCatalogHashSha256")!.StringValue);corpusIdentity=Get(corpus,"Identity");
                var profiles=Activator.CreateInstance(typeof(LockedEndlessProfiles),RepoFiles.Bytes("Assets/StammstreckenPuzzle/UI/MobileCandidate.endless-profiles.json"),corpus)!;profile=((IEnumerable)Get(profiles,"Profiles")).Cast<object>().First();search=Get(profile,"Search");
                parameters=Parameters(false,false);var parameterHash=LevelHashing.HashProjection(Parse((string)Call(parameters,"Projection")!));Descriptor=DescriptorFor("202610210000","1",parameters,parameterHash);Id=(string)Static("EndlessProjection","DescriptorId",Descriptor,new CanonicalJsonAdapter())!;
                var campaign=((IEnumerable)Get(corpus,"Puzzles")).Cast<object>().Select(p=>(PuzzleDefinition)Call(p,"ToDomainDefinition")!).ToArray();
                var generation=(Task)Static("PuzzleCandidateSearch","GenerateAsync",202610210000UL,search,campaign,Array.Empty<PuzzleDefinition>(),CancellationToken.None,null,null)!;generation.GetAwaiter().GetResult();var candidate=Get(generation,"Result");Assert.That((bool)Get(candidate,"Succeeded"),Is.True,Get(candidate,"Status").ToString());Definition=(PuzzleDefinition)Get(candidate,"Puzzle");
                Fresh=PuzzleSolver.Solve(Definition,new SolverLimits(10000000,60000));Assert.That(Fresh.Classification,Is.EqualTo(SolverClassification.UNIQUE));Assert.That(Fresh.Metrics!.RequiredGuessDepth,Is.Zero);
                var temporary=Input(new string('0',64));PublicHash=LevelHashing.Compute(LevelHashProfiles.PuzzleSemantic,Parse((string)Static("EndlessProjection","PublicInput",temporary,false)!)).Hash!;Puzzle=Input(PublicHash.Sha256);
                var path=new List<JsonValue>();for(var i=0;i<Fresh.Path.Count;i++)
                {
                    var c=Fresh.Path[i];var incoming=i==0?Definition.A.Side:Toward(c,Fresh.Path[i-1]);var outgoing=i==Fresh.Path.Count-1?Definition.B.Side:Toward(c,Fresh.Path[i+1]);var ports=(1<<(int)incoming)|(1<<(int)outgoing);var shape=Enumerable.Range(0,6).Single(s=>TrackGeometry.Ports((TrackShape)s)==ports);
                    path.Add(JsonValue.CreateObject(new[]{new JsonMember("x",JsonEdit.Int(c.X)),new JsonMember("y",JsonEdit.Int(c.Y)),new JsonMember("track",JsonEdit.Str("TRACK_"+((TrackShape)shape).ToString()))}));
                }
                var solution=JsonValue.CreateObject(new[]{new JsonMember("puzzleId",JsonEdit.Str(Id)),new JsonMember("publicPuzzleHash",PublicHash.ToJson()),new JsonMember("path",JsonValue.CreateArray(path))});SolutionHash=LevelHashing.Compute(LevelHashProfiles.Solution,solution).Hash!;
                var m=Fresh.Metrics;Proof=EndlessProofV1Codec.Create(Id,PublicHash,SolutionHash,Fresh.SolverVersion,m.SearchNodes,m.DeductionSteps,m.MaxDeductionDepth,m.RequiredGuessDepth);
            }
            internal object WrongPublicIdentity()=>Input(new string('0',64));
            internal object MutateDescriptor(string mutation)=>DescriptorFor(mutation=="seed"?"202610210001":"202610210000",mutation=="ordinal"?"2":"1",mutation=="parameters"?Parameters(true,false):mutation=="recent"?Parameters(false,true):parameters,(string)Get(Descriptor,"ParameterHashSha256"));
            internal IReadOnlyList<LevelDiagnostic> Validate(ProofReadResult proof,object? descriptor=null,object? puzzle=null,ProfiledHash? solution=null)=>(IReadOnlyList<LevelDiagnostic>)typeof(EndlessProofV1Codec).GetMethod("ValidateAgainst")!.Invoke(null,new[]{descriptor??Descriptor,puzzle??Puzzle,solution??SolutionHash,proof})!;
            private object Parameters(bool changed,bool recent)
            {var history=Array.CreateInstance(Type("PublicPuzzleInput"),recent?1:0);if(recent)history.SetValue(Puzzle,0);return New("SearchGeneratorParameters",Get(profile,"Id"),changed?new string('2',64):Get(profile,"HashSha256"),"public-d4-l1-v1",corpusIdentity,history,search);}
            private object DescriptorFor(string seed,string ordinal,object p,string hash)=>Static("LocalEndlessDescriptor","Create","train-track-v1","path-search-v1",seed,ordinal,p,hash)!;
            private object Input(string hash)=>New("PublicPuzzleInput",New("PublicPuzzleIdentity",Id,"STP-PUZZLE-SEMANTIC-JCS-1",hash),Definition.RulesetVersion,Definition.Grid.Width,Definition.Grid.Height,Enum.ToObject(Type("PublicPuzzleSide"),(int)Definition.A.Side),Definition.A.Index,Enum.ToObject(Type("PublicPuzzleSide"),(int)Definition.B.Side),Definition.B.Index,Definition.RowCounts,Definition.ColumnCounts);
            private Type Type(string name)=>app.GetType("STP.Application."+name,true)!;
            private object New(string name,params object[] args)=>Activator.CreateInstance(Type(name),args)!;
            private object? Static(string type,string name,params object?[] args)=>Type(type).GetMethods().Single(m=>m.Name==name&&m.GetParameters().Length==args.Length).Invoke(null,args);
            private static object Get(object value,string property)=>value.GetType().GetProperty(property)!.GetValue(value)!;
            private static object? Call(object value,string method)=>value.GetType().GetMethod(method)!.Invoke(value,null);
            private static Direction Toward(CellCoordinate from,CellCoordinate to)=>to.Y<from.Y?Direction.N:to.X>from.X?Direction.E:to.Y>from.Y?Direction.S:Direction.W;
        }
    }
}
