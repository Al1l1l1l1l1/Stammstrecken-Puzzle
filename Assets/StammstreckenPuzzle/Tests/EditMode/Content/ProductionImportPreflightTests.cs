using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace STP.Tests.Content.EditMode
{
    /// <summary>Data-independent gate tests; no approved content, privacy record or Human observation is manufactured.</summary>
    public sealed class ProductionImportPreflightTests
    {
        private static Type Build(string name)=>Assembly.Load("STP.Editor.Build").GetType("STP.Editor.Build."+name,true)!;
        private static Type Verifier=>Assembly.Load("STP.Bootstrap").GetType("STP.Bootstrap.ProductionImportOutputVerifier",true)!;
        private static object? Call(Type type,string method,params object[] args)
        {try{return type.GetMethod(method,BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static)!.Invoke(null,args);}catch(TargetInvocationException e){throw e.InnerException!;}}
        private static string Hash(byte[] value)=>(string)Call(Verifier,"RawHash",value)!;
        private static void Reject(string code,Action action){var error=Assert.Catch(()=>action());Assert.That(error!.Message,Does.Contain(code));}
        private sealed class Temp : IDisposable
        {
            private readonly string boundary;
            internal readonly string Root;
            internal Temp(){string? configured=Environment.GetEnvironmentVariable("STP_TEST_TEMP_ROOT");string parent=string.IsNullOrWhiteSpace(configured)?Path.GetTempPath():configured!;boundary=Path.GetFullPath(Path.Combine(parent,"stp-production-preflight-tests"));Root=Path.Combine(boundary,Guid.NewGuid().ToString("N"));Assert.That(Directory.Exists(Root)||File.Exists(Root),Is.False);Directory.CreateDirectory(Root);}
            public void Dispose(){string resolved=Path.GetFullPath(Root);Assert.That(resolved.StartsWith(boundary.TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar,StringComparison.Ordinal),Is.True);if(Directory.Exists(resolved))Directory.Delete(resolved,true);}
        }
        [Test]
        public void MissingInputsNameEveryActualProtectedInputWithoutAValuesOrApprovalFlag()
        {
            var input=Activator.CreateInstance(Build("ProductionImportPreflightInputs"),new object[]{"","","","","",""})!;
            var missing=(IReadOnlyList<string>)input.GetType().GetProperty("MissingInputNames")!.GetValue(input)!;
            Assert.That(missing,Is.EqualTo(new[]{"repositoryRoot","STP_PRODUCTION_SOURCE_ROOT","STP_PRODUCTION_IMPORT_REQUEST","STP_PRODUCTION_APPROVAL_LOCK","STP_PRODUCTION_APPROVAL_RAW_SHA256","STP_DOTNET10"}));
            var task=(Task)Call(Build("ProductionImportPreflight"),"RunAsync",input,CancellationToken.None)!;Reject("PREFLIGHT_MISSING_INPUTS:",()=>task.GetAwaiter().GetResult());
        }
        [TestCase("../outside")][TestCase("/rooted")][TestCase("C:/drive")][TestCase("a\\b")][TestCase("a//b")][TestCase("a/./b")][TestCase("a\nb")]
        public void RelativeReferencesNeverEscapeOrAlias(string path)=>Reject("PREFLIGHT_RELATIVE_PATH_INVALID",()=>Call(Verifier,"RequireRelativePath",path));
        [Test]
        public void SpacesSemicolonAndAmpersandAreDataInAnOtherwiseBoundedRelativeReference()
            =>Assert.DoesNotThrow(()=>Call(Verifier,"RequireRelativePath","source folder ; & quoted/request.json"));
        [TestCase("ContractTest")][TestCase("InternalPreview")][TestCase("DRAFT")][TestCase("QA")]
        public void QaAndFixtureRequestsCannotStartProductionPreflight(string use)
        {
            byte[] request=Encoding.UTF8.GetBytes("{\"importRequestVersion\":1,\"use\":\""+use+"\",\"publicCatalog\":null,\"campaign\":null,\"campaignCompletion\":null,\"endlessCompletion\":null,\"calibration\":null,\"cosmetics\":null,\"profiles\":[],\"levels\":[],\"localizations\":[],\"railwayAssets\":null,\"releaseLock\":\"release.json\",\"toolchainLock\":null,\"uiSourceLock\":\"ui.json\",\"evidence\":[],\"publishedLocks\":[]}");
            Reject("PREFLIGHT_REQUEST_USE_INVALID",()=>Call(Verifier,"CheckProductionRequest",request));
        }
        [Test]
        public void ACallerApprovalFieldOrUnknownRequestVersionCannotSupplyAuthority()
        {
            Reject("PREFLIGHT_FIELDS_INVALID",()=>Call(Verifier,"CheckProductionRequest",Encoding.UTF8.GetBytes("{\"productApproved\":true}")));
            var fixture=Encoding.UTF8.GetBytes("{\"fixtureEvidenceVersion\":1,\"kind\":\"SYNTHETIC_CONTRACT_TEST_NO_HUMAN_EVIDENCE\"}");
            Reject("PREFLIGHT_APPROVAL_RAW_MISMATCH",()=>Call(Verifier,"CheckProductionApproval",fixture,new string('0',64)));
            Reject("PREFLIGHT_FIELDS_INVALID",()=>Call(Verifier,"CheckProductionApproval",fixture,Hash(fixture)));
        }
        [TestCase("\n")][TestCase(" ")]
        public void ProtectedHashRequiresExactlySixtyFourLowercaseHexCharacters(string suffix)
            =>Reject("PREFLIGHT_HASH_INVALID",()=>Call(Verifier,"RequireHash",new string('a',64)+suffix));
        [Test]
        public void ActualOriginalByteDriftIsRejectedWithoutCopyingAnyOriginalIntoTheOutput()
        {
            using var temp=new Temp();string file=Path.Combine(temp.Root,"original.json");byte[] original=Encoding.UTF8.GetBytes("{\"technical\":1}");File.WriteAllBytes(file,original);var expected=new Dictionary<string,string>{{"original.json",Hash(original)}};
            Assert.DoesNotThrow(()=>Call(Build("ProductionImportPreflight"),"VerifyOriginalSnapshot",temp.Root,expected));File.AppendAllText(file," ");Reject("PREFLIGHT_ORIGINAL_SNAPSHOT_DRIFT",()=>Call(Build("ProductionImportPreflight"),"VerifyOriginalSnapshot",temp.Root,expected));Assert.That(Directory.GetFiles(temp.Root).Length,Is.EqualTo(1));
        }
        [Test]
        public void OnlyThirteenPlayerPayloadsAndTwoInternalBindingFilesMayExistInActualOutput()
        {
            using var temp=new Temp();var paths=(IReadOnlyList<string>)Verifier.GetProperty("PlayerPaths")!.GetValue(null)!;
            foreach(string name in paths.Concat(new[]{"../authoring/source-bindings.json","../authoring/player-raw-lock.json"})){string file=Path.GetFullPath(Path.Combine(temp.Root,"player",name.Replace('/',Path.DirectorySeparatorChar)));Directory.CreateDirectory(Path.GetDirectoryName(file)!);File.WriteAllText(file,"{}");}
            Assert.DoesNotThrow(()=>Call(Build("ProductionImportPreflight"),"CheckOutputFileSet",temp.Root));File.WriteAllText(Path.Combine(temp.Root,"player","participant-export.json"),"{}");Reject("PREFLIGHT_OUTPUT_FILE_SET_INVALID",()=>Call(Build("ProductionImportPreflight"),"CheckOutputFileSet",temp.Root));
        }
        [TestCase("a\nb")][TestCase("a\0b")]
        public void ProcessArgumentsRejectControlCharactersWithoutAChildProcess(string value)
            =>Reject("PREFLIGHT_PROCESS_ARGUMENT_INVALID",()=>Call(Build("ProductionImportPreflight"),"QuoteProcessArgument",value));
        [Test]
        public void DirectProcessEncodingPreservesQuotesAndTrailingBackslashesAsSingleArguments()
        {
            Assert.That(Call(Build("ProductionImportPreflight"),"QuoteProcessArgument",""),Is.EqualTo("\"\""));
            Assert.That(Call(Build("ProductionImportPreflight"),"QuoteProcessArgument","a b;&"),Is.EqualTo("\"a b;&\""));
            Assert.That(Call(Build("ProductionImportPreflight"),"QuoteProcessArgument","a\"b\\"),Is.EqualTo("\"a\\\"b\\\\\""));
        }
        [TestCase("repo;property")][TestCase("repo%3Bproperty")][TestCase("repo,property")]
        public void MsbuildReservedRepositoryPathsFailClosedBeforeAnyChildOrAttempt(string value)
            =>Reject("PREFLIGHT_BUILD_PATH_UNSUPPORTED",()=>Call(Build("ProductionImportPreflight"),"RequireBuildPathGrammar",value));
        [Test]
        public void CompilerSourceFileMutationAndAdditionalCompileInputAreDetected()
        {
            using var temp=new Temp();string[] folders={"Assets/StammstreckenPuzzle/Scripts/Puzzle/Domain","Assets/StammstreckenPuzzle/Scripts/Puzzle/Solver","Assets/StammstreckenPuzzle/Scripts/Application","Assets/StammstreckenPuzzle/Scripts/Infrastructure/Content","Assets/StammstreckenPuzzle/Scripts/Infrastructure/Persistence","tools/campaign/ProductionImport"};
            foreach(string folder in folders)Directory.CreateDirectory(Path.Combine(temp.Root,folder.Replace('/',Path.DirectorySeparatorChar)));
            string[] explicitFiles={"tools/campaign/ProductionImport/ProductionImport.csproj","Assets/StammstreckenPuzzle/Scripts/Editor/Content/LevelValidationPipeline.cs","Assets/StammstreckenPuzzle/Scripts/Editor/Content/ProofGenerator.cs","tools/campaign/HumanReadback/SourceBundle.cs","tools/campaign/HumanReadback/Readback.cs"};
            foreach(string relative in explicitFiles){string file=Path.Combine(temp.Root,relative.Replace('/',Path.DirectorySeparatorChar));Directory.CreateDirectory(Path.GetDirectoryName(file)!);File.WriteAllText(file,"technical-test");}
            object snapshot=Call(Build("ProductionImportPreflight"),"CaptureCompilerInputs",temp.Root)!;
            Assert.DoesNotThrow(()=>Call(Build("ProductionImportPreflight"),"VerifyCompilerInputs",temp.Root,snapshot));
            string first=Path.Combine(temp.Root,"tools","campaign","ProductionImport","ProductionImport.csproj");File.AppendAllText(first,"changed");Reject("PREFLIGHT_COMPILER_SOURCE_DRIFT",()=>Call(Build("ProductionImportPreflight"),"VerifyCompilerInputs",temp.Root,snapshot));
            File.WriteAllText(first,"technical-test");File.WriteAllText(Path.Combine(temp.Root,"tools","campaign","ProductionImport","Additional.cs"),"technical-test");Reject("PREFLIGHT_COMPILER_SOURCE_DRIFT",()=>Call(Build("ProductionImportPreflight"),"VerifyCompilerInputs",temp.Root,snapshot));
        }
    }
}
