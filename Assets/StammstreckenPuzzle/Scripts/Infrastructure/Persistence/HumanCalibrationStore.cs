using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;

namespace STP.Infrastructure.Persistence
{
    /// <summary>Atomic local observation storage under an independent ADR042 namespace; never opens a game profile.</summary>
    public sealed class HumanCalibrationStore:IHumanCalibrationRepository
    {
        private readonly string root;private readonly IHumanAssignmentCatalog assignments;private readonly HumanCalibrationCodec codec;private readonly ICanonicalJson json;private readonly Action<string>? fault;
        private sealed class Index {internal Index(string token,long generation,string hash){Token=token;Generation=generation;Hash=hash;}internal string Token;internal long Generation;internal string Hash;}
        private sealed class Observed {internal HumanStoreStatus Status;internal HumanObservationSnapshot? Snapshot;internal string Token="";internal Index? Index;internal string Error="HUMAN_OBSERVATION_REQUIRED";}
        public HumanCalibrationStore(string privateRoot,IHumanAssignmentCatalog assignments,ICanonicalJson json,Action<string>? faultInjector=null)
        {if(privateRoot==null||!Path.IsPathRooted(privateRoot))throw new ArgumentException("HUMAN_ROOT_NOT_ABSOLUTE");root=Path.Combine(Path.GetFullPath(privateRoot),"human-calibration-v1");this.assignments=assignments??throw new ArgumentNullException(nameof(assignments));if(!System.Text.RegularExpressions.Regex.IsMatch(assignments.ManifestBytesSha256??"","^[a-f0-9]{64}$"))throw new ArgumentException("HUMAN_MANIFEST_HASH_INVALID");this.json=json??throw new ArgumentNullException(nameof(json));codec=new HumanCalibrationCodec(json,assignments);fault=faultInjector;CheckPath(root);}
        private string IndexPath=>Path.Combine(root,"active-run.json");
        private string ActiveRecoveryIndex=>Path.Combine(root,"active-recovery-index.json");
        private string RecordPath(string token){if(!Guid.TryParseExact(token,"N",out _))throw new IOException("HUMAN_RUN_TOKEN_INVALID");return Path.Combine(root,assignments.ManifestBytesSha256,token,"observations.json");}
        private static string[] Roles(string path)=>new[]{path,path+".bak",path+".tmp"};
        private string RecoveryAnchor(string token)=>Path.Combine(Path.GetDirectoryName(RecordPath(token))!,"recovery-anchor.json");
        private string ArchivedIndex(string token)=>Path.Combine(Path.GetDirectoryName(RecordPath(token))!,"completed-index.json");
        private static void CheckPath(string path){for(string? p=Path.GetFullPath(path);p!=null;p=Path.GetDirectoryName(p)){if((Directory.Exists(p)||File.Exists(p))&&(File.GetAttributes(p)&FileAttributes.ReparsePoint)!=0)throw new IOException("HUMAN_REPARSE_REJECTED");}}
        private byte[]? Read(string path,int limit){CheckPath(path);if(!File.Exists(path))return null;var info=new FileInfo(path);if(info.Length>limit)throw new IOException("HUMAN_FILE_SIZE_INVALID");return File.ReadAllBytes(path);}
        private byte[] IndexBytes(Index index)=>LocalSaveHash.Canonical(json,LocalSaveCodec.Object(("humanIndexVersion","1"),("manifestBytesSha256",LocalSaveHash.Quote(assignments.ManifestBytesSha256)),("sessionToken",LocalSaveHash.Quote(index.Token)),("generation",index.Generation.ToString(System.Globalization.CultureInfo.InvariantCulture)),("recordBytesSha256",LocalSaveHash.Quote(index.Hash))));
        private Index DecodeIndex(byte[] source)
        {var p=json.Canonicalize(source,new JsonResourceLimits(4096,4,256,32,16));if(!p.Succeeded)throw new ArgumentException("HUMAN_INDEX_INVALID");var f=CanonicalDtoCursor.FromCanonical(p.GetUtf8()!).Fields("humanIndexVersion","manifestBytesSha256","sessionToken","generation","recordBytesSha256");f[0].RequireInteger(1);if(f[1].String()!=assignments.ManifestBytesSha256)throw new ArgumentException("HUMAN_INDEX_SOURCE_UNSUPPORTED");string token=f[2].String();if(!Guid.TryParseExact(token,"N",out _)||f[3].Integer()<1||!System.Text.RegularExpressions.Regex.IsMatch(f[4].String(),"^[a-f0-9]{64}$"))throw new ArgumentException("HUMAN_INDEX_INVALID");return new Index(token,f[3].Integer(),f[4].String());}
        private Observed Observe()
        {
            var bytes=new List<byte[]>();var indices=new List<(int Role,Index Index)>();bool invalid=false;string? token=null;
            var indexSources=Roles(IndexPath).Concat(new[]{ActiveRecoveryIndex}).ToArray();for(int i=0;i<indexSources.Length;i++){var b=Read(indexSources[i],4096);bytes.Add(b??Array.Empty<byte>());if(b==null)continue;try{indices.Add((i,DecodeIndex(b)));}catch(ArgumentException e){if(e.Message=="HUMAN_INDEX_SOURCE_UNSUPPORTED")return new Observed{Status=HumanStoreStatus.Unsupported,Error=e.Message,Token=LocalSaveHash.Bytes(b)};invalid=true;}catch(SaveCodecFailure){invalid=true;}}
            if(indices.Count>0)
            {
                var main=indices.Where(x=>x.Role==0).Select(x=>x.Index).SingleOrDefault()??indices.Where(x=>x.Role==3).Select(x=>x.Index).SingleOrDefault();
                if(main!=null){token=main.Token;foreach(var other in indices.Where(x=>x.Index.Token!=token))if(other.Role!=1||!IsArchivedCompleted(other.Index,bytes))invalid=true;}
                else{if(indices.Select(x=>x.Index.Token).Distinct().Count()!=1)return new Observed{Status=HumanStoreStatus.Corrupt,Error="HUMAN_RUN_INDEX_AMBIGUOUS",Token=LocalSaveHash.Bytes(Encoding.UTF8.GetBytes(string.Join(":",bytes.Select(LocalSaveHash.Bytes))))};token=indices.OrderByDescending(x=>x.Index.Generation).First().Index.Token;}
            }
            else
            {
                var folder=Path.Combine(root,assignments.ManifestBytesSha256);var found=Directory.Exists(folder)?Directory.GetDirectories(folder).Select(Path.GetFileName).Where(n=>n!=null&&Guid.TryParseExact(n,"N",out _)).ToArray():Array.Empty<string?>();
                if(found.Length==1)token=found[0];else if(found.Length>1)invalid=true;
                if(Directory.Exists(root)&&Directory.GetDirectories(root).Any(d=>Path.GetFileName(d)!=assignments.ManifestBytesSha256))invalid=true;
            }
            var records=new List<(int Role,HumanObservationSnapshot Snapshot,string Hash)>();
            if(token!=null){var sources=Roles(RecordPath(token)).Concat(new[]{RecoveryAnchor(token)}).ToArray();for(int i=0;i<sources.Length;i++){var b=Read(sources[i],8388608);bytes.Add(b??Array.Empty<byte>());if(b==null)continue;try{var snapshot=codec.Decode(b);if(snapshot.Run.SessionToken!=token)throw new ArgumentException();records.Add((i,snapshot,LocalSaveHash.Bytes(b)));}catch(Exception e)when(e is ArgumentException||e is SaveCodecFailure||e is InvalidOperationException){invalid=true;}}}
            string observation=LocalSaveHash.Bytes(Encoding.UTF8.GetBytes(string.Join(":",bytes.Select(LocalSaveHash.Bytes))+":"+(token??"none")));
            if(indices.Count==0&&records.Count==0&&!invalid&&token==null)return new Observed{Status=HumanStoreStatus.Missing,Token=observation,Error="HUMAN_RUN_MISSING"};
            if(records.Count==0)return new Observed{Status=HumanStoreStatus.RecoveryRequired,Token=observation,Error="HUMAN_RECORD_RECOVERY_REQUIRED"};
            var highest=records.OrderByDescending(x=>x.Snapshot.Generation).First();if(records.Any(x=>x.Snapshot.Generation==highest.Snapshot.Generation&&x.Hash!=highest.Hash))return new Observed{Status=HumanStoreStatus.Corrupt,Token=observation,Error="HUMAN_SPLIT_BRAIN"};
            var index=indices.Where(x=>x.Index.Token==token).OrderByDescending(x=>x.Index.Generation).Select(x=>x.Index).FirstOrDefault();bool ready=!invalid&&highest.Role==0&&indices.Any(x=>x.Role==0&&x.Index.Token==token&&x.Index.Generation==highest.Snapshot.Generation&&x.Index.Hash==highest.Hash)&&index!=null&&index.Generation==highest.Snapshot.Generation;
            return new Observed{Status=ready?HumanStoreStatus.Ready:HumanStoreStatus.RecoveryRequired,Snapshot=highest.Snapshot,Index=index,Token=observation,Error=ready?"NONE":"HUMAN_EXPLICIT_RECOVERY_REQUIRED"};
        }
        private bool IsArchivedCompleted(Index expected,List<byte[]> observedBytes)
        {
            var index=Read(ArchivedIndex(expected.Token),4096);var record=Read(RecordPath(expected.Token),8388608);observedBytes.Add(index??Array.Empty<byte>());observedBytes.Add(record??Array.Empty<byte>());if(index==null||record==null)return false;
            try{var archived=DecodeIndex(index);var snapshot=codec.Decode(record);return archived.Token==expected.Token&&archived.Generation==expected.Generation&&archived.Hash==expected.Hash&&LocalSaveHash.Bytes(record)==expected.Hash&&snapshot.Generation==expected.Generation&&snapshot.Run.SessionToken==expected.Token&&snapshot.Run.NextAssignmentIndex==assignments.Assignments.Count;}
            catch(Exception e)when(e is ArgumentException||e is SaveCodecFailure||e is InvalidOperationException){return false;}
        }
        private T Exclusive<T>(CancellationToken ct,Func<T> action)
        {
            CheckPath(root);if(Path.DirectorySeparatorChar=='/')return ExclusiveDirectory(ct,action);
            using var mutex=new Mutex(false,"STP.HumanObservation."+LocalSaveHash.Bytes(Encoding.UTF8.GetBytes(root.ToUpperInvariant())));bool entered=false;var elapsed=System.Diagnostics.Stopwatch.StartNew();try{while(!entered){ct.ThrowIfCancellationRequested();if(elapsed.ElapsedMilliseconds>=30000)throw new IOException("HUMAN_WRITER_BUSY");try{entered=mutex.WaitOne(100);}catch(AbandonedMutexException){entered=true;}}return action();}finally{if(entered)mutex.ReleaseMutex();}
        }
        private T ExclusiveDirectory<T>(CancellationToken ct,Func<T> action)
        {
            int descriptor=-1;try
            {
                Directory.CreateDirectory(root);CheckPath(root);descriptor=NativeDirectory.Open(root,NativeDirectory.Flags());if(descriptor<0)throw new IOException("HUMAN_NATIVE_LOCK_OPEN_FAILED");var elapsed=System.Diagnostics.Stopwatch.StartNew();
                while(true){ct.ThrowIfCancellationRequested();if(NativeDirectory.Flock(descriptor,2|4)==0)break;int error=Marshal.GetLastWin32Error();if(error!=11&&error!=35&&error!=4)throw new IOException("HUMAN_NATIVE_LOCK_UNSUPPORTED_"+error);if(elapsed.ElapsedMilliseconds>=30000)throw new IOException("HUMAN_WRITER_BUSY");Thread.Sleep(10);}ct.ThrowIfCancellationRequested();return action();
            }
            catch(DllNotFoundException e){throw new IOException("HUMAN_NATIVE_LOCK_UNAVAILABLE",e);}catch(EntryPointNotFoundException e){throw new IOException("HUMAN_NATIVE_LOCK_UNAVAILABLE",e);}finally{if(descriptor>=0)NativeDirectory.Close(descriptor);}
        }
        private static class NativeDirectory
        {
            internal static int Flags()
            {
#if UNITY_IOS && !UNITY_EDITOR
                return 0x01000000;
#elif UNITY_ANDROID && !UNITY_EDITOR
                return 1<<19;
#else
                if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))return 0x01000000;if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))return 1<<19;throw new IOException("HUMAN_NATIVE_LOCK_PLATFORM_UNSUPPORTED");
#endif
            }
#if UNITY_IOS && !UNITY_EDITOR
            private const string Library="__Internal";
#else
            private const string Library="libc";
#endif
            [DllImport(Library,EntryPoint="open",SetLastError=true)]internal static extern int Open([MarshalAs(UnmanagedType.LPUTF8Str)]string path,int flags);
            [DllImport(Library,EntryPoint="flock",SetLastError=true)]internal static extern int Flock(int descriptor,int operation);
            [DllImport(Library,EntryPoint="close",SetLastError=true)]internal static extern int Close(int descriptor);
        }
        private HumanStoreResult LoadResult(Observed o)=>new HumanStoreResult(o.Status,o.Error,o.Status==HumanStoreStatus.Ready?o.Snapshot:null,o.Token);
        public Task<HumanStoreResult> LoadAsync(CancellationToken cancellation)=>Task.Run(()=>Safe(()=>Exclusive(cancellation,()=>LoadResult(Observe()))),cancellation);
        private static HumanStoreResult Safe(Func<HumanStoreResult> action){try{return action();}catch(OperationCanceledException){return new HumanStoreResult(HumanStoreStatus.Cancelled,"HUMAN_CANCELLED");}catch(Exception e)when(e is IOException||e is UnauthorizedAccessException||e is ArgumentException||e is SaveCodecFailure){return new HumanStoreResult(HumanStoreStatus.RecoveryRequired,"HUMAN_STORAGE_OBSERVATION_REQUIRED");}}
        private void AtomicWrite(string target,byte[] bytes,string point)
        {
            CheckPath(target);Directory.CreateDirectory(Path.GetDirectoryName(target)!);CheckPath(target);string pending=target+".tmp";
            using(var stream=new FileStream(pending,FileMode.Create,FileAccess.Write,FileShare.None)){stream.Write(bytes,0,bytes.Length);stream.Flush(true);}fault?.Invoke(point+"_PENDING_FLUSHED");
            if(File.Exists(target))File.Replace(pending,target,target+".bak");else File.Move(pending,target);fault?.Invoke(point+"_MAIN_REPLACED");
            var actual=Read(target,8388608);if(actual==null||!actual.SequenceEqual(bytes))throw new IOException("HUMAN_REREAD_BYTES_MISMATCH");
        }
        private HumanStoreResult Write(HumanObservationSnapshot snapshot,CancellationToken ct)
        {
            var bytes=codec.Encode(snapshot);ct.ThrowIfCancellationRequested();
            try
            {AtomicWrite(RecordPath(snapshot.Run.SessionToken),bytes,"RECORD");var reread=codec.Decode(Read(RecordPath(snapshot.Run.SessionToken),8388608)!);if(reread.Generation!=snapshot.Generation)throw new IOException("HUMAN_REREAD_VALIDATION_FAILED");var index=new Index(snapshot.Run.SessionToken,snapshot.Generation,LocalSaveHash.Bytes(bytes));AtomicWrite(IndexPath,IndexBytes(index),"INDEX");var observed=Observe();if(observed.Status!=HumanStoreStatus.Ready||observed.Snapshot?.Generation!=snapshot.Generation)throw new IOException("HUMAN_INDEX_REREAD_FAILED");if(File.Exists(ActiveRecoveryIndex)){CheckPath(ActiveRecoveryIndex);File.Delete(ActiveRecoveryIndex);observed=Observe();if(observed.Status!=HumanStoreStatus.Ready||observed.Snapshot?.Generation!=snapshot.Generation)throw new IOException("HUMAN_RECOVERY_INDEX_REREAD_FAILED");}return new HumanStoreResult(HumanStoreStatus.Committed,"NONE",snapshot,observed.Token);}
            catch(Exception e)when(e is IOException||e is UnauthorizedAccessException||e is OperationCanceledException){Observed? observed=null;try{observed=Observe();}catch(Exception failure)when(failure is IOException||failure is UnauthorizedAccessException){}return new HumanStoreResult(HumanStoreStatus.RecoveryRequired,"HUMAN_WRITE_OUTCOME_UNKNOWN",observationToken:observed?.Token);}
        }
        public Task<HumanStoreResult> StartRunAsync(HumanCalibrationRun run,string? expectedPriorToken,CancellationToken cancellation)=>Task.Run(()=>Safe(()=>Exclusive(cancellation,()=>
        {
            var o=Observe();if(run.NextAssignmentIndex!=0||run.Trials.Count!=0)return new HumanStoreResult(HumanStoreStatus.Conflict,"HUMAN_NEW_PARTICIPANT_MUST_BE_EMPTY",observationToken:o.Token);
            var start=new HumanObservationSnapshot(1,run);codec.Encode(start);
            if(expectedPriorToken==null){if(o.Status!=HumanStoreStatus.Missing)return new HumanStoreResult(HumanStoreStatus.Conflict,"HUMAN_EXISTING_RUN_REQUIRES_EXPLICIT_ARCHIVE",observationToken:o.Token);}
            else
            {
                if(o.Status!=HumanStoreStatus.Ready||o.Snapshot==null||o.Index==null||o.Snapshot.Run.SessionToken!=expectedPriorToken||run.SessionToken==expectedPriorToken||o.Snapshot.Run.NextAssignmentIndex!=assignments.Assignments.Count||Directory.Exists(Path.GetDirectoryName(RecordPath(run.SessionToken))!))return new HumanStoreResult(HumanStoreStatus.Conflict,"HUMAN_NEXT_PARTICIPANT_NOT_ALLOWED",observationToken:o.Token);
                AtomicWrite(ArchivedIndex(expectedPriorToken),IndexBytes(o.Index),"PARTICIPANT_ARCHIVE");if(!IsArchivedCompleted(o.Index,new List<byte[]>()))throw new IOException("HUMAN_COMPLETED_ARCHIVE_REREAD_FAILED");
            }
            return Write(start,cancellation);
        })),cancellation);
        public Task<HumanStoreResult> CommitAsync(HumanObservationSnapshot snapshot,long expectedGeneration,CancellationToken cancellation)=>Task.Run(()=>Safe(()=>Exclusive(cancellation,()=>
        {var o=Observe();if(o.Status!=HumanStoreStatus.Ready)return LoadResult(o);if(o.Snapshot!.Generation!=expectedGeneration||snapshot.Generation!=expectedGeneration+1||snapshot.Run.SessionToken!=o.Snapshot.Run.SessionToken)return new HumanStoreResult(HumanStoreStatus.Conflict,"HUMAN_OBSERVATION_CONFLICT",observationToken:o.Token);return Write(snapshot,cancellation);})),cancellation);
        public Task<HumanStoreResult> RecoverAsync(string observationToken,CancellationToken cancellation)=>Task.Run(()=>Safe(()=>Exclusive(cancellation,()=>
        {var o=Observe();if(o.Token!=observationToken)return new HumanStoreResult(HumanStoreStatus.Conflict,"HUMAN_OBSERVATION_CHANGED",observationToken:o.Token);if(o.Status==HumanStoreStatus.Ready)return LoadResult(o);if(o.Status!=HumanStoreStatus.RecoveryRequired||o.Snapshot==null)return LoadResult(o);ArchiveRecoveryRoles(o.Snapshot,cancellation);return Write(o.Snapshot,cancellation);})),cancellation);
        private void ArchiveRecoveryRoles(HumanObservationSnapshot snapshot,CancellationToken ct)
        {
            string token=snapshot.Run.SessionToken;var paths=Roles(IndexPath).Concat(Roles(RecordPath(token))).Concat(new[]{RecoveryAnchor(token),ActiveRecoveryIndex}).Where(File.Exists).ToArray();
            var archive=Path.Combine(root,assignments.ManifestBytesSha256,"_recovery",Guid.NewGuid().ToString("N"));CheckPath(archive);Directory.CreateDirectory(archive);
            for(int i=0;i<paths.Length;i++)
            {
                ct.ThrowIfCancellationRequested();var bytes=Read(paths[i],8388608)!;var destination=Path.Combine(archive,i.ToString("D2")+"-"+Path.GetFileName(paths[i]));
                using(var stream=new FileStream(destination,FileMode.CreateNew,FileAccess.Write,FileShare.None)){stream.Write(bytes,0,bytes.Length);stream.Flush(true);}
                if(!Read(destination,8388608)!.SequenceEqual(bytes))throw new IOException("HUMAN_RECOVERY_ARCHIVE_REREAD_FAILED");
            }
            ct.ThrowIfCancellationRequested();string anchor=RecoveryAnchor(token);string temporary=anchor+"."+Guid.NewGuid().ToString("N")+".tmp";var verified=codec.Encode(snapshot);
            using(var stream=new FileStream(temporary,FileMode.CreateNew,FileAccess.Write,FileShare.None)){stream.Write(verified,0,verified.Length);stream.Flush(true);}
            if(!Read(temporary,8388608)!.SequenceEqual(verified))throw new IOException("HUMAN_RECOVERY_ANCHOR_REREAD_FAILED");
            // A real backup path also preserves the prior authority on Unity's Windows replacement boundary.
            if(File.Exists(anchor))File.Replace(temporary,anchor,Path.Combine(archive,"replaced-record-anchor.json"));else File.Move(temporary,anchor);fault?.Invoke("RECOVERY_ANCHOR_COMMITTED");
            string pendingIndex=ActiveRecoveryIndex+"."+Guid.NewGuid().ToString("N")+".tmp";var indexBytes=IndexBytes(new Index(token,snapshot.Generation,LocalSaveHash.Bytes(verified)));
            using(var stream=new FileStream(pendingIndex,FileMode.CreateNew,FileAccess.Write,FileShare.None)){stream.Write(indexBytes,0,indexBytes.Length);stream.Flush(true);}
            if(!Read(pendingIndex,4096)!.SequenceEqual(indexBytes))throw new IOException("HUMAN_RECOVERY_INDEX_ANCHOR_REREAD_FAILED");
            if(File.Exists(ActiveRecoveryIndex))File.Replace(pendingIndex,ActiveRecoveryIndex,Path.Combine(archive,"replaced-active-recovery-index.json"));else File.Move(pendingIndex,ActiveRecoveryIndex);fault?.Invoke("RECOVERY_INDEX_ANCHOR_COMMITTED");
            foreach(var path in paths.Where(p=>p!=anchor&&p!=ActiveRecoveryIndex)){CheckPath(path);File.Delete(path);if(path==IndexPath)fault?.Invoke("RECOVERY_ROOT_MAIN_DELETED");}fault?.Invoke("RECOVERY_ROLES_CLEARED");
        }
        public Task<byte[]> ExportAsync(CancellationToken cancellation)=>Task.Run(()=>Exclusive(cancellation,()=>
        {var o=Observe();if(o.Status!=HumanStoreStatus.Ready||o.Snapshot==null)throw new IOException("HUMAN_EXPORT_REQUIRES_CONFIRMED_RECORD");return Read(RecordPath(o.Snapshot.Run.SessionToken),8388608)!;}),cancellation);
    }
}
