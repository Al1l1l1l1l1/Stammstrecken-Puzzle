using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using STP.Bootstrap;

namespace STP.Editor.Build
{
    /// <summary>Separate protected build inputs. No request field, DTO approval flag or default selects production trust.</summary>
    public sealed class ProductionImportPreflightInputs
    {
        public ProductionImportPreflightInputs(string repositoryRoot,string sourceRoot,string request,string trustedApprovalLock,string expectedApprovalRawSha256,string dotnet10)
        {RepositoryRoot=repositoryRoot;SourceRoot=sourceRoot;Request=request;TrustedApprovalLock=trustedApprovalLock;ExpectedApprovalRawSha256=expectedApprovalRawSha256;Dotnet10=dotnet10;}
        public string RepositoryRoot{get;}public string SourceRoot{get;}public string Request{get;}public string TrustedApprovalLock{get;}public string ExpectedApprovalRawSha256{get;}public string Dotnet10{get;}
        public static ProductionImportPreflightInputs FromEnvironment(string explicitRepositoryRoot)=>new ProductionImportPreflightInputs(explicitRepositoryRoot,
            Environment.GetEnvironmentVariable("STP_PRODUCTION_SOURCE_ROOT")??"",Environment.GetEnvironmentVariable("STP_PRODUCTION_IMPORT_REQUEST")??"",
            Environment.GetEnvironmentVariable("STP_PRODUCTION_APPROVAL_LOCK")??"",Environment.GetEnvironmentVariable("STP_PRODUCTION_APPROVAL_RAW_SHA256")??"",Environment.GetEnvironmentVariable("STP_DOTNET10")??"");
        public IReadOnlyList<string> MissingInputNames
        {
            get
            {
                var values=new[]{RepositoryRoot,SourceRoot,Request,TrustedApprovalLock,ExpectedApprovalRawSha256,Dotnet10};
                var names=new[]{"repositoryRoot","STP_PRODUCTION_SOURCE_ROOT","STP_PRODUCTION_IMPORT_REQUEST","STP_PRODUCTION_APPROVAL_LOCK","STP_PRODUCTION_APPROVAL_RAW_SHA256","STP_DOTNET10"};
                return System.Array.AsReadOnly(names.Where((name,i)=>string.IsNullOrWhiteSpace(values[i])).ToArray());
            }
        }
    }

    /// <summary>Only the actual importer plus repeated byte/reader checks can construct this value. No native or Human release authority.</summary>
    public sealed class ProductionImportPreflightResult
    {
        private readonly Dictionary<string,byte[]> files;
        private readonly string repositoryRoot,sourceRoot;
        private readonly Dictionary<string,string> compilerInputs;
        internal ProductionImportPreflightResult(string directory,Dictionary<string,byte[]> files,ProductionImportOutputFacts facts,string repositoryRoot,string sourceRoot,Dictionary<string,string> compilerInputs)
        {OutputDirectory=directory;this.files=files.ToDictionary(p=>p.Key,p=>(byte[])p.Value.Clone(),StringComparer.Ordinal);Facts=facts;this.repositoryRoot=repositoryRoot;this.sourceRoot=sourceRoot;this.compilerInputs=new Dictionary<string,string>(compilerInputs,StringComparer.Ordinal);}
        public string OutputDirectory{get;}public ProductionImportOutputFacts Facts{get;}
        public string AuditDirectory=>Directory.GetParent(OutputDirectory)!.FullName;
        public IReadOnlyList<string> PlayerPaths=>ProductionImportOutputVerifier.PlayerPaths;
        public byte[] GetPlayerBytes(string path)=>files.TryGetValue(path,out var value)?(byte[])value.Clone():throw new ArgumentException("PREFLIGHT_PLAYER_PATH_NOT_BOUND");
        /// <summary>Call immediately before copying these exact checked bytes into assets. Never reparses or repairs source.</summary>
        public void VerifyPlayerFilesUnchanged()
        {foreach(var p in files)ProductionImportPreflight.RequireSameBytes(ProductionImportPreflight.Inside(OutputDirectory,"player/"+p.Key),p.Value,"PREFLIGHT_OUTPUT_CHANGED");ProductionImportPreflight.CheckOutputFileSet(OutputDirectory);}
        /// <summary>Root calls this immediately before its first asset/settings mutation and again before building.</summary>
        public void VerifyStillCurrent()
        {ProductionImportPreflight.VerifyOriginalSnapshot(sourceRoot,Facts.OriginalRawHashes);ProductionImportPreflight.VerifyCompilerInputs(repositoryRoot,compilerInputs);VerifyPlayerFilesUnchanged();}
    }

    /// <summary>Stable failure only. Full process diagnostics remain in the append-only local attempt, never copied to Player.</summary>
    public sealed class ProductionImportPreflightException : InvalidOperationException
    {
        internal ProductionImportPreflightException(string code,string? auditDirectory=null):base(code){Code=code;AuditDirectory=auditDirectory;}
        public string Code{get;}public string? AuditDirectory{get;}
    }

    /// <summary>Source-only preflight. Performs no AssetDatabase, ProjectSettings, SDK, delete, download or install action.</summary>
    public static class ProductionImportPreflight
    {
        private const string Project="tools/campaign/ProductionImport/ProductionImport.csproj";
        private static readonly string[] SourceFolders={"Assets/StammstreckenPuzzle/Scripts/Puzzle/Domain","Assets/StammstreckenPuzzle/Scripts/Puzzle/Solver","Assets/StammstreckenPuzzle/Scripts/Application","Assets/StammstreckenPuzzle/Scripts/Infrastructure/Content","Assets/StammstreckenPuzzle/Scripts/Infrastructure/Persistence","tools/campaign/ProductionImport"};
        public static Task<ProductionImportPreflightResult> RunAsync(ProductionImportPreflightInputs inputs,CancellationToken cancellation)
            =>Task.Run(()=>{var run=Run(inputs,false,cancellation);return new ProductionImportPreflightResult(run.Output,run.Player,run.Facts,run.Repository,run.Source,run.CompilerInputs);},cancellation);
#if UNITY_INCLUDE_TESTS
        /// <summary>Technical CLI integration only. It can return no Production result, approval or approved files.</summary>
        public static Task<string> RunContractTestForTestsAsync(string repositoryRoot,string fixtureSourceRoot,string request,string dotnet10,CancellationToken cancellation)
            =>Task.Run(()=>Run(new ProductionImportPreflightInputs(repositoryRoot,fixtureSourceRoot,request,"","",dotnet10),true,cancellation).Output,cancellation);
#endif
        private sealed class RunOutput
        {internal string Output="",Repository="",Source="";internal Dictionary<string,byte[]> Player=new Dictionary<string,byte[]>(StringComparer.Ordinal);internal Dictionary<string,string> CompilerInputs=new Dictionary<string,string>(StringComparer.Ordinal);internal ProductionImportOutputFacts Facts=null!;}
        private static RunOutput Run(ProductionImportPreflightInputs input,bool fixture,CancellationToken cancellation)
        {
            if(input==null)throw Fail("PREFLIGHT_INPUTS_REQUIRED");
            var missing=input.MissingInputNames.Where(n=>!fixture||(n!="STP_PRODUCTION_APPROVAL_LOCK"&&n!="STP_PRODUCTION_APPROVAL_RAW_SHA256")).ToArray();
            if(missing.Length!=0)throw Fail("PREFLIGHT_MISSING_INPUTS:"+string.Join(",",missing));
            string repository=Absolute(input.RepositoryRoot,true),source=Absolute(input.SourceRoot,true),dotnet=Absolute(input.Dotnet10,false);
            RequireBuildPathGrammar(repository);string project=Inside(repository,Project);ReadBounded(project,1048576);ProductionImportOutputVerifier.RequireRelativePath(input.Request);
            byte[] request=ReadBounded(Inside(source,input.Request),16777216);byte[]? approval=null;IReadOnlyList<string> pointers;
            if(fixture)
            {
#if UNITY_INCLUDE_TESTS
                pointers=ProductionImportOutputVerifier.CheckContractTestRequest(request);
#else
                throw Fail("PREFLIGHT_FIXTURE_FORBIDDEN");
#endif
            }
            else
            {
                pointers=ProductionImportOutputVerifier.CheckProductionRequest(request);
                ProductionImportOutputVerifier.RequireRelativePath(input.TrustedApprovalLock);ProductionImportOutputVerifier.RequireHash(input.ExpectedApprovalRawSha256);
                approval=ReadBounded(Inside(source,input.TrustedApprovalLock),16777216);ProductionImportOutputVerifier.CheckProductionApproval(approval,input.ExpectedApprovalRawSha256);
            }
            byte[] release=ReadBounded(Inside(source,pointers[0]),16777216),ui=ReadBounded(Inside(source,pointers[1]),16777216);
            var compilerBefore=CaptureCompilerInputs(repository);
            cancellation.ThrowIfCancellationRequested();string attempt=CreateAttempt(repository);string output=Path.Combine(attempt,"import");
            try
            {
                WriteNew(Path.Combine(attempt,"compiler-inputs.txt"),InventoryText(compilerBefore));
                var version=Execute(dotnet,new[]{"--version"},repository,attempt,"01-sdk",30,cancellation);
                if(!System.Text.RegularExpressions.Regex.IsMatch(version.Trim(),@"\A10\.[0-9]+\.[0-9]+(?:[-+][A-Za-z0-9.-]+)?\z"))throw Fail("PREFLIGHT_DOTNET10_REQUIRED",attempt);
                string feed=Path.Combine(attempt,"empty-feed"),obj=Path.Combine(attempt,"obj"),bin=Path.Combine(attempt,"bin");Directory.CreateDirectory(feed);
                string config=Path.Combine(attempt,"offline-nuget.config");WriteNew(config,"<configuration><packageSources><clear/><add key=\"local-empty-only\" value=\""+Xml(feed)+"\"/></packageSources><fallbackPackageFolders><clear/></fallbackPackageFolders></configuration>\n");
                var properties=new[]{Property("StpRepositoryRoot",repository),Property("BaseIntermediateOutputPath",obj+Path.DirectorySeparatorChar),Property("MSBuildProjectExtensionsPath",obj+Path.DirectorySeparatorChar),Property("OutputPath",bin+Path.DirectorySeparatorChar),Property("RestorePackagesPath",Path.Combine(attempt,"packages")),"-p:StpOverlayProps=","-p:ImportDirectoryBuildProps=false","-p:ImportDirectoryBuildTargets=false","-p:NuGetAudit=false","-p:RestoreFallbackFolders=","-p:UseSharedCompilation=false","-nodeReuse:false"};
                Execute(dotnet,new[]{"restore",project,Property("RestoreConfigFile",config),"--disable-parallel","--verbosity","minimal"}.Concat(properties).ToArray(),repository,attempt,"02-restore",180,cancellation);
                Execute(dotnet,new[]{"build",project,"--no-restore","--configuration","Release","--verbosity","minimal"}.Concat(properties).ToArray(),repository,attempt,"03-build",180,cancellation);
                string dll=Path.Combine(bin,"ProductionImport.dll");ReadBounded(dll,16777216);
                var arguments=new List<string>{dll,"--source-root",source,"--request",input.Request,"--out",output};
                if(!fixture){arguments.Add("--trusted-approval-lock");arguments.Add(input.TrustedApprovalLock);arguments.Add("--trusted-approval-sha256");arguments.Add(input.ExpectedApprovalRawSha256);}
                Execute(dotnet,arguments.ToArray(),repository,attempt,"04-import",1800,cancellation);
                VerifyCompilerInputs(repository,compilerBefore);RequireSameBytes(Inside(source,input.Request),request,"PREFLIGHT_REQUEST_CHANGED");
                RequireSameBytes(Inside(source,pointers[0]),release,"PREFLIGHT_RELEASE_CHANGED");RequireSameBytes(Inside(source,pointers[1]),ui,"PREFLIGHT_UI_CHANGED");
                CheckOutputFileSet(output);var player=new Dictionary<string,byte[]>(StringComparer.Ordinal);foreach(string name in ProductionImportOutputVerifier.PlayerPaths)player.Add(name,ReadBounded(Inside(output,"player/"+name),8388608));
                byte[] bindings=ReadBounded(Inside(output,"authoring/source-bindings.json"),16777216),rawLock=ReadBounded(Inside(output,"authoring/player-raw-lock.json"),1048576);ProductionImportOutputFacts facts;
                if(fixture)
                {
#if UNITY_INCLUDE_TESTS
                    facts=ProductionImportOutputVerifier.VerifyContractTest(player,bindings,rawLock,release,ui);
#else
                    throw Fail("PREFLIGHT_FIXTURE_FORBIDDEN",attempt);
#endif
                }
                else facts=ProductionImportOutputVerifier.VerifyProduction(player,bindings,rawLock,release,ui,approval!,input.TrustedApprovalLock,input.ExpectedApprovalRawSha256);
                RequireOriginal(facts.OriginalRawHashes,input.Request,ProductionImportOutputVerifier.RawHash(request));
                RequireOriginal(facts.OriginalRawHashes,pointers[0],facts.ReleaseLockRawSha256);RequireOriginal(facts.OriginalRawHashes,pointers[1],facts.UiSourceLockRawSha256);
                if(!fixture)RequireOriginal(facts.OriginalRawHashes,input.TrustedApprovalLock,input.ExpectedApprovalRawSha256);
                VerifyOriginalSnapshot(source,facts.OriginalRawHashes);
                foreach(var file in player)RequireSameBytes(Inside(output,"player/"+file.Key),file.Value,"PREFLIGHT_OUTPUT_CHANGED");
                RequireSameBytes(Inside(output,"authoring/source-bindings.json"),bindings,"PREFLIGHT_OUTPUT_CHANGED");RequireSameBytes(Inside(output,"authoring/player-raw-lock.json"),rawLock,"PREFLIGHT_OUTPUT_CHANGED");
                VerifyCompilerInputs(repository,compilerBefore);cancellation.ThrowIfCancellationRequested();
                WriteNew(Path.Combine(attempt,"result.txt"),"scope="+(fixture?"CONTRACT_TEST_ONLY":"PRODUCTION_SOURCE_PREFLIGHT_ONLY")+"\nplayerFiles=13\nbindingsRawSha256="+facts.BindingsRawSha256+"\nreleaseRawSha256="+facts.ReleaseLockRawSha256+"\nreleaseJcsSha256="+facts.ReleaseLockJcsSha256+"\napprovalRawSha256="+(facts.ApprovalLockRawSha256??"null")+"\nuiRawSha256="+facts.UiSourceLockRawSha256+"\noriginalCount="+facts.OriginalRawHashes.Count+"\nnativeOrHumanReleaseApproval=false\n");
                return new RunOutput{Output=output,Player=player,Facts=facts,Repository=repository,Source=source,CompilerInputs=compilerBefore};
            }
            catch(Exception error)
            {
                // Never print original contents, environment, credentials, or arbitrary exception text into a Player/Unity log.
                string code=error is ProductionImportPreflightException known?known.Code:error is OperationCanceledException?"PREFLIGHT_CANCELLED":System.Text.RegularExpressions.Regex.IsMatch(error.Message,@"\A(?:PREFLIGHT|RUNTIME|CAT|GEN|STAR|SAVE)_[A-Z0-9_]+\z")?error.Message:"PREFLIGHT_STOPPED:"+error.GetType().Name;
                WriteNew(Path.Combine(attempt,"failure.txt"),code+"\n");throw Fail(code,attempt);
            }
        }
        private static void RequireOriginal(IReadOnlyDictionary<string,string> originals,string path,string hash)
        {if(!originals.TryGetValue(path,out var value)||value!=hash)throw Fail("PREFLIGHT_REQUIRED_ORIGINAL_NOT_BOUND");}
        public static void VerifyOriginalSnapshot(string sourceRoot,IReadOnlyDictionary<string,string> expected)
        {
            string root=Absolute(sourceRoot,true);if(expected==null||expected.Count==0||expected.Count>8192)throw Fail("PREFLIGHT_ORIGINAL_COUNT_INVALID");long total=0;
            foreach(var row in expected){ProductionImportOutputVerifier.RequireHash(row.Value);string file=Inside(root,row.Key);var info=new FileInfo(file);if(!info.Exists||info.Length<=0||info.Length>16777216||(total+=info.Length)>536870912)throw Fail("PREFLIGHT_ORIGINAL_SIZE_INVALID");if(FileHash(file)!=row.Value)throw Fail("PREFLIGHT_ORIGINAL_SNAPSHOT_DRIFT");}
        }
        internal static void CheckOutputFileSet(string output)
        {
            Absolute(output,true);var expected=ProductionImportOutputVerifier.PlayerPaths.Select(p=>"player/"+p).Concat(new[]{"authoring/source-bindings.json","authoring/player-raw-lock.json"}).OrderBy(x=>x,StringComparer.Ordinal).ToArray();
            var actual=SafeFiles(output).Select(p=>p.Substring(output.TrimEnd(Path.DirectorySeparatorChar).Length+1).Replace('\\','/')).OrderBy(x=>x,StringComparer.Ordinal).ToArray();if(!actual.SequenceEqual(expected))throw Fail("PREFLIGHT_OUTPUT_FILE_SET_INVALID");
        }
        internal static string Inside(string root,string relative)
        {ProductionImportOutputVerifier.RequireRelativePath(relative);string full=Path.GetFullPath(Path.Combine(root,relative.Replace('/',Path.DirectorySeparatorChar)));if(!full.StartsWith(Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar,StringComparison.Ordinal))throw Fail("PREFLIGHT_PATH_ESCAPE");CheckNoLinks(full);return full;}
        private static string Absolute(string value,bool directory)
        {if(string.IsNullOrWhiteSpace(value)||!Path.IsPathRooted(value)||value.Any(char.IsControl))throw Fail("PREFLIGHT_ABSOLUTE_PATH_REQUIRED");string full=Path.GetFullPath(value);if(!string.Equals(Path.GetPathRoot(value.Replace(Path.AltDirectorySeparatorChar,Path.DirectorySeparatorChar)),Path.GetPathRoot(full),StringComparison.OrdinalIgnoreCase))throw Fail("PREFLIGHT_ABSOLUTE_PATH_REQUIRED");CheckNoLinks(full);if(directory?!Directory.Exists(full):!File.Exists(full))throw Fail(directory?"PREFLIGHT_DIRECTORY_MISSING":"PREFLIGHT_FILE_MISSING");return full;}
        private static void CheckNoLinks(string full)
        {for(string? p=Path.GetFullPath(full);p!=null;p=Path.GetDirectoryName(p))if((File.Exists(p)||Directory.Exists(p))&&(File.GetAttributes(p)&FileAttributes.ReparsePoint)!=0)throw Fail("PREFLIGHT_REPARSE_FORBIDDEN");}
        internal static void RequireSameBytes(string path,byte[] bytes,string code)
        {if(!ReadBounded(path,16777216).SequenceEqual(bytes))throw Fail(code);}
        private static byte[] ReadBounded(string path,int max)
        {CheckNoLinks(path);var info=new FileInfo(path);if(!info.Exists||info.Length<=0||info.Length>max)throw Fail("PREFLIGHT_FILE_SIZE_OR_MISSING");byte[] bytes=File.ReadAllBytes(path);if(bytes.Length<=0||bytes.Length>max)throw Fail("PREFLIGHT_FILE_SIZE_OR_MISSING");return bytes;}
        private static string FileHash(string path)
        {CheckNoLinks(path);using var stream=File.OpenRead(path);using var sha=SHA256.Create();return string.Concat(sha.ComputeHash(stream).Select(b=>b.ToString("x2",System.Globalization.CultureInfo.InvariantCulture)));}
        private static Dictionary<string,string> CaptureCompilerInputs(string repository)
        {
            var files=SourceFolders.SelectMany(p=>SafeFiles(Inside(repository,p)).Where(f=>f.EndsWith(".cs",StringComparison.Ordinal))).Concat(new[]{Project,"Assets/StammstreckenPuzzle/Scripts/Editor/Content/LevelValidationPipeline.cs","Assets/StammstreckenPuzzle/Scripts/Editor/Content/ProofGenerator.cs","tools/campaign/HumanReadback/SourceBundle.cs","tools/campaign/HumanReadback/Readback.cs"}.Select(p=>Inside(repository,p)));
            var result=new Dictionary<string,string>(StringComparer.Ordinal);foreach(string p in files.OrderBy(p=>p,StringComparer.Ordinal)){string rel=p.Substring(repository.TrimEnd(Path.DirectorySeparatorChar).Length+1).Replace('\\','/');if(!result.ContainsKey(rel))result.Add(rel,FileHash(p));}return result;
        }
        private static IEnumerable<string> SafeFiles(string root)
        {var pending=new Stack<string>();pending.Push(root);int count=0;while(pending.Count!=0){string directory=pending.Pop();CheckNoLinks(directory);foreach(string file in Directory.GetFiles(directory)){CheckNoLinks(file);if(++count>16384)throw Fail("PREFLIGHT_FILE_COUNT_LIMIT");yield return file;}foreach(string child in Directory.GetDirectories(directory)){CheckNoLinks(child);pending.Push(child);}}}
        internal static void VerifyCompilerInputs(string repository,Dictionary<string,string> expected)
        {var current=CaptureCompilerInputs(repository);if(current.Count!=expected.Count||expected.Any(e=>!current.TryGetValue(e.Key,out var hash)||hash!=e.Value))throw Fail("PREFLIGHT_COMPILER_SOURCE_DRIFT");}
        private static string InventoryText(Dictionary<string,string> value)=>string.Join("\n",value.OrderBy(p=>p.Key,StringComparer.Ordinal).Select(p=>p.Value+"  "+p.Key))+"\n";
        private static string CreateAttempt(string repository)
        {string parent=Inside(repository,".utmp/production-preflight");Directory.CreateDirectory(parent);CheckNoLinks(parent);string path=Path.Combine(parent,DateTime.UtcNow.ToString("yyyyMMddTHHmmssfff",System.Globalization.CultureInfo.InvariantCulture)+"-"+Guid.NewGuid().ToString("N"));if(Directory.Exists(path)||File.Exists(path))throw Fail("PREFLIGHT_ATTEMPT_EXISTS");Directory.CreateDirectory(path);return path;}
        private static void WriteNew(string path,string content)
        {using var stream=new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.Read);byte[] bytes=new UTF8Encoding(false).GetBytes(content);stream.Write(bytes,0,bytes.Length);stream.Flush(true);}
        internal static void RequireBuildPathGrammar(string value)
        {if(value.Any(c=>";,%$@()*?\"".Contains(c)))throw Fail("PREFLIGHT_BUILD_PATH_UNSUPPORTED");}
        private static string Property(string name,string value)=>"-p:"+name+"="+string.Concat(value.Select(c=>"%$@';,()*?".Contains(c)?"%"+((int)c).ToString("X2",System.Globalization.CultureInfo.InvariantCulture):c.ToString()));
        private static string Xml(string value)=>value.Replace("&","&amp;").Replace("\"","&quot;").Replace("<","&lt;").Replace(">","&gt;");
        /// <summary>Direct process argument encoding. No shell, command substitution or string-built command execution.</summary>
        public static string QuoteProcessArgument(string value)
        {
            if(value==null||value.Any(char.IsControl))throw Fail("PREFLIGHT_PROCESS_ARGUMENT_INVALID");var text=new StringBuilder("\"");int slashes=0;
            foreach(char c in value){if(c=='\\'){slashes++;continue;}if(c=='\"'){text.Append('\\',slashes*2+1);text.Append(c);}else{text.Append('\\',slashes);text.Append(c);}slashes=0;}
            return text.Append('\\',slashes*2).Append('"').ToString();
        }
        private static string Execute(string executable,string[] args,string working,string attempt,string stage,int timeoutSeconds,CancellationToken cancellation)
        {
            var start=new ProcessStartInfo(executable,string.Join(" ",args.Select(QuoteProcessArgument))){WorkingDirectory=working,UseShellExecute=false,CreateNoWindow=true,RedirectStandardOutput=true,RedirectStandardError=true};
            start.EnvironmentVariables["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";start.EnvironmentVariables["DOTNET_SKIP_FIRST_TIME_EXPERIENCE"]="1";start.EnvironmentVariables["DOTNET_NOLOGO"]="1";start.EnvironmentVariables["DOTNET_CLI_HOME"]=Path.Combine(attempt,"dotnet-home");start.EnvironmentVariables["MSBUILDDISABLENODEREUSE"]="1";start.EnvironmentVariables["DOTNET_ADD_GLOBAL_TOOLS_TO_PATH"]="false";start.EnvironmentVariables["DOTNET_GENERATE_ASPNET_CERTIFICATE"]="false";start.EnvironmentVariables["DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE"]="true";start.EnvironmentVariables["DOTNET_CLI_WORKLOAD_UPDATE_ADVERTISING_MANIFEST_DISABLE"]="true";
            var process=new Process{StartInfo=start};var stdout=new FileStream(Path.Combine(attempt,stage+".stdout.log"),FileMode.CreateNew,FileAccess.Write,FileShare.Read);var stderr=new FileStream(Path.Combine(attempt,stage+".stderr.log"),FileMode.CreateNew,FileAccess.Write,FileShare.Read);bool detached=false;
            try
            {
            if(!process.Start())throw Fail("PREFLIGHT_PROCESS_START_FAILED",attempt);var output=process.StandardOutput.BaseStream.CopyToAsync(stdout);var errors=process.StandardError.BaseStream.CopyToAsync(stderr);var watch=Stopwatch.StartNew();bool interrupted=false;
            while(!process.WaitForExit(100)){if(cancellation.IsCancellationRequested||watch.Elapsed.TotalSeconds>timeoutSeconds){interrupted=true;try{process.Kill();}catch(InvalidOperationException){}break;}}
            bool exited=process.WaitForExit(5000),drained=false;string actualExitCode=exited?process.ExitCode.ToString(System.Globalization.CultureInfo.InvariantCulture):"unavailable";var pipes=Task.WhenAll(output,errors);
            try{drained=pipes.Wait(3000);}catch(AggregateException){/* Preserve actual diagnostics and fail closed below. */}
            if(!drained)
            {
                // Mono's synchronous pipe Dispose can itself wait for a descendant's handle. Transfer ownership
                // to the completion continuation; the Editor fails now while remaining diagnostic bytes stay local.
                detached=true;
                _=pipes.ContinueWith(t=>{_ =t.Exception;try{stdout.Flush(true);stderr.Flush(true);}catch{}try{stdout.Dispose();}catch{}try{stderr.Dispose();}catch{}try{process.Dispose();}catch{}},CancellationToken.None,TaskContinuationOptions.None,TaskScheduler.Default);
            }
            else{stdout.Flush(true);stderr.Flush(true);stdout.Dispose();stderr.Dispose();}
            WriteNew(Path.Combine(attempt,stage+".exit.txt"),"actualExitCode="+actualExitCode+"\ninterrupted="+interrupted.ToString().ToLowerInvariant()+"\npipeDrainComplete="+drained.ToString().ToLowerInvariant()+"\n");
            if(!exited)throw Fail("PREFLIGHT_PROCESS_EXIT_NOT_CONFIRMED",attempt);
            if(interrupted)throw Fail(cancellation.IsCancellationRequested?"PREFLIGHT_CANCELLED":"PREFLIGHT_PROCESS_TIMEOUT",attempt);
            if(!drained)throw Fail("PREFLIGHT_DIAGNOSTIC_PIPE_TIMEOUT",attempt);
            if(process.ExitCode!=0)throw Fail("PREFLIGHT_PROCESS_FAILED:"+stage,attempt);
            return stage=="01-sdk"?File.ReadAllText(Path.Combine(attempt,stage+".stdout.log")):string.Empty;
            }
            finally{if(!detached){stdout.Dispose();stderr.Dispose();process.Dispose();}}
        }
        private static ProductionImportPreflightException Fail(string code,string? attempt=null)=>new ProductionImportPreflightException(code,attempt);
    }
}
