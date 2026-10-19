using System;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using STP.Infrastructure.Content;

namespace STP.Tests.Content.EditMode
{
    /// <summary>Exercises the real adapter and inner interface without adding a test assembly reference edge.</summary>
    public sealed class CanonicalJsonAdapterTests
    {
        private static byte[] Bytes(string text) => Encoding.UTF8.GetBytes(text);
        private static Type AppType(string name) => typeof(CanonicalJsonAdapter).GetInterfaces()
            .Single(t => t.Name == "ICanonicalJson").Assembly.GetType("STP.Application." + name, true)!;
        private static byte[]? Canonical(string text, out string? error) => new CanonicalJsonAdapter()
            .CanonicalizeBytes(Bytes(text), 262144, 16, 8192, 1024, 64, out error);

        [Test]
        public void AdapterUsesExistingExactJcsBytesAndDoesNotMutateInput()
        {
            const string source = "{\"z\":-9007199254740991,\"a\":[true,null,\"Ö\\n\\t\"],\"😀\":1,\"\\ue000\":2}";
            var input = Bytes(source); var original = (byte[])input.Clone();
            var result = new CanonicalJsonAdapter().CanonicalizeBytes(input,262144,16,8192,1024,64,out var error);
            Assert.That(error, Is.Null);
            Assert.That(result, Is.EqualTo(JcsSerializer.Serialize(StrictJsonParser.Parse(input).Value).Utf8));
            Assert.That(Encoding.UTF8.GetString(result!), Is.EqualTo("{\"a\":[true,null,\"Ö\\n\\t\"],\"z\":-9007199254740991,\"😀\":1,\"\ue000\":2}"));
            Assert.That(Convert.ToBase64String(result!),Is.EqualTo("eyJhIjpbdHJ1ZSxudWxsLCLDllxuXHQiXSwieiI6LTkwMDcxOTkyNTQ3NDA5OTEsIvCfmIAiOjEsIu6AgCI6Mn0="));
            Assert.That(LevelHashing.Sha256Hex(result!),Is.EqualTo("722aa2cf748c46258055152d850ac11064c75eb28739b3c57db5cc43e0786423"));
            Assert.That(input, Is.EqualTo(original));
            Assert.That(result![0], Is.EqualTo((byte)'{'));
            Assert.That(result[result.Length-1], Is.EqualTo((byte)'}'));
        }

        [TestCase("{\"x\":1,\"x\":2}")]
        [TestCase("{\"x\":1.0}")]
        [TestCase("{\"x\":1e0}")]
        [TestCase("{\"x\":9007199254740992}")]
        [TestCase("{\"x\":-9007199254740992}")]
        [TestCase("{\"x\":\"\\ud800\"}")]
        [TestCase("{\"x\":\"\\udc00\"}")]
        [TestCase("{\"x\":NaN}")]
        [TestCase("{} trailing")]
        [TestCase("/*comment*/{}")]
        public void InvalidJsonNeverGetsCanonicalBytes(string input)
        { Assert.That(Canonical(input,out var error),Is.Null); Assert.That(error,Is.Not.Null.And.Not.Empty); }

        [Test]
        public void ParserResourceLimitsAndInvalidUtf8AreEnforced()
        {
            var adapter = new CanonicalJsonAdapter();
            Assert.That(adapter.CanonicalizeBytes(Bytes("{}"),1,16,8192,1024,64,out var size),Is.Null);
            Assert.That(size,Is.Not.Null);
            Assert.That(adapter.CanonicalizeBytes(new byte[]{0xff},1024,16,8192,1024,64,out var utf8),Is.Null);
            Assert.That(utf8,Is.Not.Null);
            Assert.That(adapter.CanonicalizeBytes(new byte[]{0xef,0xbb,0xbf,123,125},1024,16,8192,1024,64,out var bom),Is.Null);
            Assert.That(bom,Is.Not.Null);
            Assert.That(adapter.CanonicalizeBytes(Bytes("[1,2]"),1024,16,8192,1,64,out var array),Is.Null);
            Assert.That(array,Is.Not.Null);
            Assert.That(adapter.CanonicalizeBytes(Bytes("{}"),0,16,8192,1024,64,out var invalid),Is.Null);
            Assert.That(invalid,Is.EqualTo("JSON_LIMITS_INVALID"));
        }

        [Test]
        public void ExplicitInnerPortUsesSameBytesAndDefensiveResultCopies()
        {
            var adapter = new CanonicalJsonAdapter();
            var port = typeof(CanonicalJsonAdapter).GetInterfaces().Single(t=>t.Name=="ICanonicalJson");
            var limits = Activator.CreateInstance(AppType("JsonResourceLimits"),262144,16,8192,1024,64);
            var source=Bytes("{\"z\":2,\"a\":1}");
            var result=port.GetMethod("Canonicalize")!.Invoke(adapter,new[]{(object)source,limits})!;
            var kind=result.GetType();
            Assert.That(kind.GetProperty("Succeeded")!.GetValue(result),Is.True);
            Assert.That(kind.GetProperty("ErrorCode")!.GetValue(result),Is.Null);
            var get=kind.GetMethod("GetUtf8")!;
            var first=(byte[])get.Invoke(result,null)!;
            Assert.That(first,Is.EqualTo(Bytes("{\"a\":1,\"z\":2}")));
            source[0]=0;first[0]=0;
            Assert.That((byte[])get.Invoke(result,null)!,Is.EqualTo(Bytes("{\"a\":1,\"z\":2}")));
            var bad=port.GetMethod("Canonicalize")!.Invoke(adapter,new[]{(object)Bytes("{\"x\":1,\"x\":2}"),limits})!;
            Assert.That(kind.GetProperty("Succeeded")!.GetValue(bad),Is.False);
            Assert.That(get.Invoke(bad,null),Is.Null);
            Assert.That(kind.GetProperty("ErrorCode")!.GetValue(bad),Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void ResultFactoriesAndLimitsRejectIncoherentInputs()
        {
            var result=AppType("CanonicalJsonResult");var source=Bytes("{}");
            var ok=result.GetMethod("Success")!.Invoke(null,new object[]{source})!;
            source[0]=0;
            Assert.That(result.GetMethod("GetUtf8")!.Invoke(ok,null),Is.EqualTo(Bytes("{}")));
            Assert.Throws<TargetInvocationException>(()=>result.GetMethod("Failure")!.Invoke(null,new object[]{""}));
            Assert.Throws<TargetInvocationException>(()=>Activator.CreateInstance(AppType("JsonResourceLimits"),0,16,8192,1024,64));
        }

        private static object New(string type, params object[] args) => Activator.CreateInstance(AppType(type),args)!;
        private static object EnumValue(string type,string value) => Enum.Parse(AppType(type),value);
        private static object? Property(object value,string name) => value.GetType().GetProperty(name)!.GetValue(value);

        [Test]
        public void PublicPuzzleContractsValidateAndOwnTheirInputArrays()
        {
            var identity=New("PublicPuzzleIdentity","qa-contract","STP-PUZZLE-SEMANTIC-JCS-1",new string('a',64));
            var side=EnumValue("PublicPuzzleSide","W");
            var other=EnumValue("PublicPuzzleSide","E");
            var rows=new[]{2,0};var columns=new[]{1,1};
            var puzzle=New("PublicPuzzleInput",identity,"train-track-v1",2,2,side,0,other,0,rows,columns);
            rows[0]=0;columns[0]=0;
            Assert.That(Property(puzzle,"RowCounts"),Is.EqualTo(new[]{2,0}));
            Assert.That(Property(puzzle,"ColumnCounts"),Is.EqualTo(new[]{1,1}));
            Assert.That(puzzle.GetType().GetMethod("ToDomainDefinition")!.Invoke(puzzle,null),Is.Not.Null);
            Assert.Throws<TargetInvocationException>(()=>New("PublicPuzzleIdentity","x","wrong-profile",new string('a',64)));
            Assert.Throws<TargetInvocationException>(()=>New("PublicPuzzleIdentity","x","STP-PUZZLE-SEMANTIC-JCS-1",new string('A',64)));
            Assert.Throws<TargetInvocationException>(()=>New("PublicPuzzleInput",identity,"train-track-v1",2,2,side,0,other,0,new[]{0,0},new[]{0,0}));
        }

        [Test]
        public void CatalogContractsRejectPromotionAndUnconfirmedRewardValues()
        {
            var production=EnumValue("CatalogUse","Production");
            var review=EnumValue("CatalogUse","IsolatedReview");
            var draft=EnumValue("CatalogApprovalStatus","Draft");
            var catalog=New("CatalogIdentity","qa-public240-v1",1,new string('b',64),review,draft);
            Assert.Throws<TargetInvocationException>(()=>New("CatalogIdentity","product",1,new string('b',64),production,draft));
            Assert.Throws<TargetInvocationException>(()=>New("CatalogIdentity","qa-public240-v1",1,new string('b',64),production,EnumValue("CatalogApprovalStatus","ProductApproved")));
            var identity=New("PublicPuzzleIdentity","puzzle","STP-PUZZLE-SEMANTIC-JCS-1",new string('a',64));
            var required=new[]{"previous"};
            var entry=New("LockedCampaignEntry",catalog,identity,"season","section","route",1,required);
            required[0]="mutated";
            Assert.That(Property(entry,"RequiredFirstClears"),Is.EqualTo(new[]{"previous"}));
            var values=New("CompletionValues",catalog,identity,15,5,10,60,150,null!,null!);
            Assert.That(Property(values,"TwoStarMs"),Is.Null);
            Assert.That(Property(values,"ThreeStarMs"),Is.Null);
            Assert.Throws<TargetInvocationException>(()=>New("CompletionValues",catalog,identity,99,5,10,60,150,null!,null!));
            Assert.Throws<TargetInvocationException>(()=>New("CompletionValues",catalog,identity,15,5,10,60,150,(long)100,(long)200));
            Assert.Throws<TargetInvocationException>(()=>New("CompletionValues",catalog,identity,15,5,10,60,150,(long)100,(long)100));
        }

        [Test]
        public void SaveBindingsAndClosedResultUnionsPreserveIsolation()
        {
            var review=EnumValue("CatalogUse","IsolatedReview");
            var production=EnumValue("CatalogUse","Production");
            var profile=New("SaveProfileIdentity","qa-public240-v1","tester",review);
            var catalog=New("CatalogIdentity","qa-public240-v1",1,new string('b',64),review,EnumValue("CatalogApprovalStatus","Draft"));
            Assert.Throws<TargetInvocationException>(()=>New("SaveProfileIdentity","qa-public240-v1","tester",production));
            Assert.Throws<TargetInvocationException>(()=>New("SaveProfileIdentity","product","tester",review));
            var marker=AppType("IImmutableSavePayload");
            // This test-only empty immutable payload has no implementation or production assembly reference edge.
            var assembly=System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("TContractPayload"),System.Reflection.Emit.AssemblyBuilderAccess.Run);
            var type=assembly.DefineDynamicModule("Tests").DefineType("EmptyImmutablePayload",TypeAttributes.Public|TypeAttributes.Sealed);
            type.AddInterfaceImplementation(marker);type.DefineDefaultConstructor(MethodAttributes.Public);
            var payload=Activator.CreateInstance(type.CreateTypeInfo()!.AsType())!;
            var snapshotType=AppType("SaveSnapshot\u00601").MakeGenericType(marker);
            var snapshot=Activator.CreateInstance(snapshotType,profile,catalog,(long)1,payload)!;
            Assert.That(Property(snapshot,"Generation"),Is.EqualTo(1L));
            Assert.That(Property(snapshot,"Payload"),Is.SameAs(payload));
            Assert.Throws<TargetInvocationException>(()=>Activator.CreateInstance(snapshotType,profile,catalog,(long)-1,payload));
            Assert.Throws<TargetInvocationException>(()=>Activator.CreateInstance(snapshotType,profile,catalog,(long)9007199254740992L,payload));
            var wrongCatalog=New("CatalogIdentity","product",1,new string('b',64),production,EnumValue("CatalogApprovalStatus","ProductApproved"));
            Assert.Throws<TargetInvocationException>(()=>Activator.CreateInstance(snapshotType,profile,wrongCatalog,(long)1,payload));
            foreach(var resultName in new[]{"SaveLoadResult\u00601","SaveCommitResult\u00601"})
            {
                var resultType=AppType(resultName).MakeGenericType(marker);
                var success=resultType.GetMethods().Single(m=>m.Name=="Success" && m.GetParameters().Length==1).Invoke(null,new[]{snapshot})!;
                Assert.That(Property(success,"Succeeded"),Is.True);
                Assert.That(Property(success,"Snapshot"),Is.SameAs(snapshot));
                Assert.That(Property(success,"ErrorCode"),Is.Null);
                var statusType=resultType.GetMethods().Single(m=>m.Name=="Failure" && m.GetParameters().Length==2).GetParameters()[0].ParameterType;
                foreach(var status in Enum.GetValues(statusType))
                {
                    var name=status!.ToString();
                    if(name=="Loaded"||name=="Committed"||name=="NeedsInitialization")
                        Assert.Throws<TargetInvocationException>(()=>resultType.GetMethods().Single(m=>m.Name=="Failure" && m.GetParameters().Length==2).Invoke(null,new[]{status,(object)"SAVE_TEST"}));
                    else
                    {
                        var failure=resultType.GetMethods().Single(m=>m.Name=="Failure" && m.GetParameters().Length==2).Invoke(null,new[]{status,(object)"SAVE_TEST"})!;
                        Assert.That(Property(failure,"Succeeded"),Is.False);
                        Assert.That(Property(failure,"Snapshot"),Is.Null);
                        Assert.That(Property(failure,"ErrorCode"),Is.EqualTo("SAVE_TEST"));
                    }
                }
            }
            var loadType=AppType("SaveLoadResult\u00601").MakeGenericType(marker);
            var empty=loadType.GetMethod("NeedsInitialization")!.Invoke(null,null)!;
            Assert.That(Property(empty,"Snapshot"),Is.Null);
            Assert.That(Property(empty,"Succeeded"),Is.False);
        }

        [TestCase("{\"a\":1,\"b\":2}",16,8192,1024,1)]
        [TestCase("[[[0]]]",1,8192,1024,64)]
        [TestCase("\"abcd\"",16,3,1024,64)]
        [TestCase("{\"abcd\":0}",16,3,1024,64)]
        public void EveryIndependentParserLimitIsApplied(string input,int depth,int text,int array,int members)
        {
            var bytes=new CanonicalJsonAdapter().CanonicalizeBytes(Bytes(input),262144,depth,text,array,members,out var code);
            Assert.That(bytes,Is.Null);Assert.That(code,Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void RecoveryRequestsBindEveryCandidateAndSplitBrainIsNeverSuccess()
        {
            var review=EnumValue("CatalogUse","IsolatedReview");
            var profile=New("SaveProfileIdentity","qa-public240-v1","tester",review);
            var catalog=New("CatalogIdentity","qa-public240-v1",1,new string('b',64),review,EnumValue("CatalogApprovalStatus","Draft"));
            object Candidate(string kind,string status,long? generation,string? hash,string? source) => New("SaveCandidateProvenance",EnumValue("SaveCandidateKind",kind),EnumValue("SaveCandidateStatus",status),generation!,hash!,source!);
            object Observe(object main,object backup,object pending)
            {
                var array=Array.CreateInstance(AppType("SaveCandidateProvenance"),3);
                array.SetValue(main,0);array.SetValue(backup,1);array.SetValue(pending,2);
                return New("SaveRepositoryObservation",profile,catalog,array);
            }
            var main=Candidate("Main","Valid",2,new string('a',64),new string('c',64));
            var backup=Candidate("Backup","Valid",2,new string('b',64),new string('d',64));
            var missing=Candidate("Pending","Missing",null,null,null);
            var observed=Observe(main,backup,missing);
            var request=New("SaveRecoveryRequest",observed,EnumValue("SaveCandidateKind","Backup"));
            var matches=request.GetType().GetMethod("Matches")!;
            Assert.That(matches.Invoke(request,new[]{observed}),Is.True);
            var newerMain=Candidate("Main","Valid",3,new string('a',64),new string('e',64));
            Assert.That(matches.Invoke(request,new[]{Observe(newerMain,backup,missing)}),Is.False);
            var changedBytes=Candidate("Main","Valid",2,new string('a',64),new string('f',64));
            Assert.That(matches.Invoke(request,new[]{Observe(changedBytes,backup,missing)}),Is.False);
            var unsupported=Candidate("Pending","Unsupported",null,null,new string('f',64));
            Assert.Throws<TargetInvocationException>(()=>New("SaveRecoveryRequest",Observe(main,backup,unsupported),EnumValue("SaveCandidateKind","Backup")));
            Assert.Throws<TargetInvocationException>(()=>New("SaveRecoveryRequest",observed,EnumValue("SaveCandidateKind","Pending")));
            Assert.Throws<TargetInvocationException>(()=>Candidate("Main","Valid",2,null,new string('c',64)));
            var marker=AppType("IImmutableSavePayload");
            var assembly=System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("TRecoveryPayload"),System.Reflection.Emit.AssemblyBuilderAccess.Run);
            var type=assembly.DefineDynamicModule("Tests").DefineType("EmptyRecoveryPayload",TypeAttributes.Public|TypeAttributes.Sealed);
            type.AddInterfaceImplementation(marker);type.DefineDefaultConstructor(MethodAttributes.Public);
            var payload=Activator.CreateInstance(type.CreateTypeInfo()!.AsType())!;
            var snapshot=Activator.CreateInstance(AppType("SaveSnapshot\u00601").MakeGenericType(marker),profile,catalog,(long)2,payload)!;
            var resultType=AppType("SaveLoadResult\u00601").MakeGenericType(marker);
            var split=resultType.GetMethod("SplitBrain")!.Invoke(null,new[]{snapshot,observed})!;
            Assert.That(Property(split,"Succeeded"),Is.False);
            Assert.That(Property(split,"Snapshot"),Is.Null);
            Assert.That(Property(split,"ConservativeSnapshot"),Is.SameAs(snapshot));
            Assert.That(Property(split,"Observation"),Is.SameAs(observed));
            Assert.That(Property(split,"Status")!.ToString(),Is.EqualTo("SplitBrain"));
            var noMain=Candidate("Main","Missing",null,null,null);
            var pending=Candidate("Pending","Valid",2,new string('a',64),new string('e',64));
            var withoutMain=Observe(noMain,backup,pending);
            var failureObserved=resultType.GetMethods().Single(m=>m.Name=="Failure" && m.GetParameters().Length==3);
            var fail=failureObserved.Invoke(null,new[]{EnumValue("SaveLoadStatus","SplitBrain"),(object)"SAVE_SPLIT_BRAIN",withoutMain})!;
            Assert.That(Property(fail,"Snapshot"),Is.Null);Assert.That(Property(fail,"ConservativeSnapshot"),Is.Null);
            Assert.That(Property(fail,"Succeeded"),Is.False);Assert.That(Property(fail,"Observation"),Is.SameAs(withoutMain));
            var selection=New("SaveRecoveryRequest",Property(fail,"Observation")!,EnumValue("SaveCandidateKind","Backup"));
            Assert.That(matches.Invoke(selection,new[]{withoutMain}),Is.True);
            var changedPending=Candidate("Pending","Valid",3,new string('a',64),new string('f',64));
            Assert.That(matches.Invoke(selection,new[]{Observe(noMain,backup,changedPending)}),Is.False);
            var loaded=resultType.GetMethods().Single(m=>m.Name=="Success" && m.GetParameters().Length==2).Invoke(null,new[]{snapshot,withoutMain})!;
            Assert.That(Property(loaded,"Observation"),Is.SameAs(withoutMain));
            Assert.That(Property(loaded,"Succeeded"),Is.True);
            var port=AppType("ISaveRepository\u00601").MakeGenericType(marker);
            var method=port.GetMethod("RecoverAsync")!;
            Assert.That(method.GetParameters().Length,Is.EqualTo(2));
            Assert.That(method.GetParameters()[0].ParameterType,Is.EqualTo(AppType("SaveRecoveryRequest")));
            Assert.That(method.GetParameters()[1].ParameterType,Is.EqualTo(typeof(System.Threading.CancellationToken)));
        }
    }
}
