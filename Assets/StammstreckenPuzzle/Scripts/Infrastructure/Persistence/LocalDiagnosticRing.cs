using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;

namespace STP.Infrastructure.Persistence
{
    /// <summary>One profile's bounded, local-only, non-authoritative diagnostic JSONL ring. Never a save/receipt/replay source.</summary>
    public sealed class LocalDiagnosticRing
    {
        public const string DirectoryName="diagnostics";
        public const string RingFileName="diagnostic-ring.jsonl";
        public const string PendingFileName="diagnostic-ring.pending.jsonl";
        public const int MaximumQueuedOperations=64;
        private const long MaxSequence=9007199254740991L;
        private static readonly UTF8Encoding Utf8=new UTF8Encoding(false,true);
        private static readonly JsonResourceLimits RowLimits=new JsonResourceLimits(2048,4,160,2,16);
        private readonly string profileDirectory,directory,main,pending,mutexName;
        private readonly ICanonicalJson json;
        private readonly LocalDiagnosticEnvironment environment;
        private readonly LocalDiagnosticLimits limits;
        private readonly Func<DateTimeOffset> utc;
        private readonly Action<LocalDiagnosticFaultPoint>? fault;
        private readonly bool debug;
        private readonly object lifecycle=new object();
        private readonly SemaphoreSlim writer=new SemaphoreSlim(1,1);
        private readonly HashSet<Task> tasks=new HashSet<Task>();
        private bool closed;
        private Task? drain;
        private Task<bool>? clear;

        /// <summary>Use the actual SaveFileSystem.ProfileDirectory, never an app-global or Human root. Construction performs no IO.</summary>
        public LocalDiagnosticRing(string exactProfileDirectory,ICanonicalJson canonicalJson,LocalDiagnosticEnvironment environment,
            bool production=true,bool enableDevelopmentDebug=false,LocalDiagnosticLimits? limits=null,
            Func<DateTimeOffset>? utcNow=null,Action<LocalDiagnosticFaultPoint>? faultInjector=null)
        {
            if(exactProfileDirectory==null||!Path.IsPathRooted(exactProfileDirectory)||exactProfileDirectory.StartsWith("\\\\",StringComparison.Ordinal)||exactProfileDirectory.StartsWith("//",StringComparison.Ordinal)||
                Path.DirectorySeparatorChar=='\\'&&(exactProfileDirectory.Length<3||exactProfileDirectory[1]!=':'||exactProfileDirectory[2]!='\\'&&exactProfileDirectory[2]!='/'))throw new ArgumentException("DIAGNOSTIC_PROFILE_ROOT_INVALID");
            profileDirectory=Path.GetFullPath(exactProfileDirectory).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar);
            if(profileDirectory.Length==0||profileDirectory==Path.GetPathRoot(profileDirectory)?.TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar))throw new ArgumentException("DIAGNOSTIC_PROFILE_ROOT_INVALID");
            directory=Path.Combine(profileDirectory,DirectoryName);main=Path.Combine(directory,RingFileName);pending=Path.Combine(directory,PendingFileName);
            string identity=Path.DirectorySeparatorChar=='\\'?directory.ToUpperInvariant():directory;
            mutexName="STP.LocalDiagnostic."+LocalSaveHash.Bytes(Utf8.GetBytes(identity));
            json=canonicalJson??throw new ArgumentNullException(nameof(canonicalJson));this.environment=environment??throw new ArgumentNullException(nameof(environment));
            this.limits=limits??new LocalDiagnosticLimits();utc=utcNow??(()=>DateTimeOffset.UtcNow);fault=faultInjector;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            debug=!production&&enableDevelopmentDebug;
#else
            debug=false;
#endif
        }
        public bool IsClosed { get {lock(lifecycle)return closed;} }
        /// <summary>Typed local event only; DEBUG can never be enabled for a Production instance.</summary>
        public Task<LocalDiagnosticWriteStatus> RecordAsync(LocalDiagnosticEvent code)=>Enqueue(LocalDiagnosticSchema.Event(code));
        /// <summary>Exact four-code callback allowlist. Unknown strings are not retained, logged or captured by a task.</summary>
        public Task<LocalDiagnosticWriteStatus> RecordServiceNoticeAsync(string? code)=>Enqueue(LocalDiagnosticSchema.Notice(code));
        /// <summary>Reads only the typed coarse status; does not inspect a provider payload/OperationId. Code is classified synchronously then discarded.</summary>
        public Task<LocalDiagnosticWriteStatus> RecordServiceResultAsync(LocalDiagnosticOperation operation,MobileServiceStatus status,string? code=null)=>Enqueue(LocalDiagnosticSchema.Service(operation,status,code));
        /// <summary>Only a fixed technical category survives. Message, type name, stack and Data are never read or held by queued work.</summary>
        public Task<LocalDiagnosticWriteStatus> RecordExceptionAsync(Exception exception,bool fatal=false)
        {if(exception==null)return Task.FromResult(LocalDiagnosticWriteStatus.Dropped);return Enqueue(LocalDiagnosticSchema.Event(fatal?LocalDiagnosticEvent.FatalException:LocalDiagnosticEvent.UnexpectedException,LocalDiagnosticSchema.Category(exception)));}
        private Task<LocalDiagnosticWriteStatus> Enqueue(LocalDiagnosticSignal? signal)
        {
            lock(lifecycle)
            {
                if(closed)return Task.FromResult(LocalDiagnosticWriteStatus.Closed);
                if(!signal.HasValue||signal.Value.Severity=="DEBUG"&&!debug||tasks.Count>=MaximumQueuedOperations)return Task.FromResult(LocalDiagnosticWriteStatus.Dropped);
                return Track(WriteOnWorker(signal.Value));
            }
        }
        private T Track<T>(T task)where T:Task
        {
            tasks.Add(task);
            _=task.ContinueWith(done=>{lock(lifecycle)tasks.Remove(done);},CancellationToken.None,TaskContinuationOptions.ExecuteSynchronously,TaskScheduler.Default);
            return task;
        }
        private async Task<LocalDiagnosticWriteStatus> WriteOnWorker(LocalDiagnosticSignal signal)
        {
            await writer.WaitAsync().ConfigureAwait(false);
            try{return await Task.Run(()=>Write(signal)).ConfigureAwait(false);}finally{writer.Release();}
        }
        /// <summary>Best-effort retained window, including at most 32 fixed-code breadcrumbs. Null means unavailable/corrupt/closed.</summary>
        public Task<LocalDiagnosticSnapshot?> ReadSnapshotAsync()
        {
            lock(lifecycle)
            {
                if(closed||tasks.Count>=MaximumQueuedOperations)return Task.FromResult<LocalDiagnosticSnapshot?>(null);
                return Track(ReadOnWorker());
            }
        }
        private async Task<LocalDiagnosticSnapshot?> ReadOnWorker()
        {
            await writer.WaitAsync().ConfigureAwait(false);
            try{return await Task.Run(()=>
            {
                try{CheckPaths();if(!Directory.Exists(directory))return new LocalDiagnosticSnapshot(Array.Empty<LocalDiagnosticEntry>());using(Exclusive()){var rows=Read(out bool corrupt);return corrupt?null:new LocalDiagnosticSnapshot(rows.Select(r=>r.Entry));}}
                catch(Exception e)when(Operational(e)){return null;}
            }).ConfigureAwait(false);}finally{writer.Release();}
        }
        /// <summary>Seals this instance before observing all started operations. No new/late writes are accepted; does not reopen.</summary>
        public Task CloseAndDrainAsync()
        {
            lock(lifecycle){Task work=SealAndDrainWork();return clear==null?work:Task.WhenAll(work,clear);}
        }
        private Task SealAndDrainWork()
        {
            lock(lifecycle){closed=true;return drain??(drain=Task.WhenAll(tasks.ToArray()));}
        }
        /// <summary>Idempotently closes, drains then removes only this ring and its pending file. Leaves unknown neighbors/other profiles untouched.</summary>
        public Task<bool> ClearAsync()
        {lock(lifecycle){closed=true;if(clear==null||clear.IsCompleted&&!clear.IsFaulted&&!clear.IsCanceled&&!clear.Result)clear=ClearAfterDrain();return clear;}}
        private async Task<bool> ClearAfterDrain()
        {
            await SealAndDrainWork().ConfigureAwait(false);
            return await Task.Run(()=>
            {
                try
                {
                    if(!Directory.Exists(directory)){CheckPaths();return true;}
                    CheckPaths();using(Exclusive()){fault?.Invoke(LocalDiagnosticFaultPoint.BeforeClear);Delete(main);Delete(pending);return true;}
                }
                catch(Exception e)when(Operational(e)){return false;}
            }).ConfigureAwait(false);
        }
        private LocalDiagnosticWriteStatus Write(LocalDiagnosticSignal signal)
        {
            try
            {
                CheckPaths();if(!Directory.Exists(profileDirectory))return LocalDiagnosticWriteStatus.Unavailable;
                Directory.CreateDirectory(directory);CheckPaths();
                using(Exclusive())
                {
                    var rows=Read(out bool corrupt);long sequence=rows.Count==0?0:rows[rows.Count-1].Entry.Sequence;
                    if(corrupt){rows.Clear();sequence=0;Add(rows,ref sequence,LocalDiagnosticSchema.Event(LocalDiagnosticEvent.CorruptRingReset)!.Value);}
                    if(!Add(rows,ref sequence,signal))return LocalDiagnosticWriteStatus.Dropped;
                    while(rows.Count>limits.Entries||rows.Sum(r=>r.Bytes.Length+1)>limits.Bytes)rows.RemoveAt(0);
                    fault?.Invoke(LocalDiagnosticFaultPoint.BeforeWrite);CheckPaths();
                    using(var stream=new FileStream(pending,FileMode.Create,FileAccess.Write,FileShare.None))
                    {foreach(var row in rows){stream.Write(row.Bytes,0,row.Bytes.Length);stream.WriteByte(10);}stream.Flush(true);}
                    fault?.Invoke(LocalDiagnosticFaultPoint.AfterFlush);CheckPaths();fault?.Invoke(LocalDiagnosticFaultPoint.BeforeReplace);
                    Native.Replace(pending,main);fault?.Invoke(LocalDiagnosticFaultPoint.AfterReplace);return LocalDiagnosticWriteStatus.Written;
                }
            }
            catch(Exception e)when(Operational(e)){return LocalDiagnosticWriteStatus.Unavailable;}
        }
        private bool Add(List<Row> rows,ref long sequence,LocalDiagnosticSignal signal)
        {
            if(sequence>=MaxSequence)return false;
            string time=utc().ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'",CultureInfo.InvariantCulture);
            var entry=new LocalDiagnosticEntry(time,++sequence,signal.Severity,signal.Code,signal.Module,environment.BuildId,environment.AppVersion,environment.Platform,signal.Outcome,signal.Category);
            byte[] bytes=Encode(entry);if(bytes.Length+1>limits.Bytes)return false;rows.Add(new Row(entry,bytes));return true;
        }
        private byte[] Encode(LocalDiagnosticEntry e)
        {
            string Q(string value)=>LocalSaveHash.Quote(value);
            string text="{\"timestampUtc\":"+Q(e.TimestampUtc)+",\"monotonicSequence\":"+e.Sequence.ToString(CultureInfo.InvariantCulture)+",\"severity\":"+Q(e.Severity)+",\"eventCode\":"+Q(e.EventCode)+",\"correlationId\":"+Q("local-"+e.Sequence.ToString(CultureInfo.InvariantCulture))+",\"buildId\":"+Q(e.BuildId)+",\"appVersion\":"+Q(e.AppVersion)+",\"platform\":"+Q(e.Platform.ToString())+",\"module\":"+Q(e.Module)+",\"fields\":{\"outcome\":"+Q(e.Outcome.ToString())+",\"category\":"+Q(e.Category.ToString())+"}}";
            var result=json.Canonicalize(Utf8.GetBytes(text),RowLimits);if(!result.Succeeded)throw new IOException("DIAGNOSTIC_ENCODING_REJECTED");return result.GetUtf8()!;
        }
        private List<Row> Read(out bool corrupt)
        {
            var rows=new List<Row>();corrupt=false;fault?.Invoke(LocalDiagnosticFaultPoint.BeforeRead);
            try
            {
                byte[] bytes;using(var stream=new FileStream(main,FileMode.Open,FileAccess.Read,FileShare.Read))
                {if(stream.Length>limits.Bytes){corrupt=true;return rows;}bytes=new byte[checked((int)stream.Length)];int n=0;while(n<bytes.Length){int got=stream.Read(bytes,n,bytes.Length-n);if(got==0)throw new IOException("DIAGNOSTIC_READ_CHANGED");n+=got;}if(stream.ReadByte()!=-1)throw new IOException("DIAGNOSTIC_READ_CHANGED");}
                if(bytes.Length==0)return rows;if(bytes[bytes.Length-1]!=10)throw new FormatException();
                string text=Utf8.GetString(bytes);foreach(string line in text.Substring(0,text.Length-1).Split('\n'))
                {
                    byte[] raw=Utf8.GetBytes(line);var parsed=json.Canonicalize(raw,RowLimits);if(!parsed.Succeeded||!raw.SequenceEqual(parsed.GetUtf8()!))throw new FormatException();
                    var f=CanonicalDtoCursor.FromCanonical(raw).Fields("timestampUtc","monotonicSequence","severity","eventCode","correlationId","buildId","appVersion","platform","module","fields");
                    string time=f[0].String();if(!DateTimeOffset.TryParseExact(time,"yyyy-MM-dd'T'HH:mm:ss.fff'Z'",CultureInfo.InvariantCulture,DateTimeStyles.AssumeUniversal|DateTimeStyles.AdjustToUniversal,out _))throw new FormatException();
                    long sequence=f[1].Integer();if(sequence<1||sequence>MaxSequence||rows.Count>0&&sequence<=rows[rows.Count-1].Entry.Sequence||f[4].String()!="local-"+sequence.ToString(CultureInfo.InvariantCulture))throw new FormatException();
                    var env=new LocalDiagnosticEnvironment(f[5].String(),f[6].String(),f[7].EnumValue<LocalDiagnosticPlatform>());var fields=f[9].Fields("outcome","category");
                    var outcome=fields[0].EnumValue<LocalDiagnosticOutcome>();var category=fields[1].EnumValue<LocalDiagnosticCategory>();string severity=f[2].String(),code=f[3].String(),module=f[8].String();
                    if(!LocalDiagnosticSchema.Known(code,module,severity,outcome,category))throw new FormatException();
                    rows.Add(new Row(new LocalDiagnosticEntry(time,sequence,severity,code,module,env.BuildId,env.AppVersion,env.Platform,outcome,category),raw));if(rows.Count>limits.Entries)throw new FormatException();
                }
                return rows;
            }
            catch(FileNotFoundException){return rows;}
            catch(DirectoryNotFoundException){return rows;}
            catch(Exception e)when(e is FormatException||e is DecoderFallbackException||e is SaveCodecFailure||e is ArgumentException){corrupt=true;rows.Clear();return rows;}
        }
        private static bool Operational(Exception e)=>e is IOException||e is UnauthorizedAccessException||e is TimeoutException||e is OperationCanceledException||e is InvalidOperationException||e is DllNotFoundException||e is EntryPointNotFoundException||e is System.Security.SecurityException;
        private void CheckPaths(){foreach(string path in new[]{profileDirectory,directory,main,pending})CheckReparse(path);}
        private static void CheckReparse(string path)
        {for(string? current=Path.GetFullPath(path);current!=null;current=Path.GetDirectoryName(current)){try{if((File.GetAttributes(current)&FileAttributes.ReparsePoint)!=0)throw new IOException("DIAGNOSTIC_REPARSE_REJECTED");}catch(FileNotFoundException){}catch(DirectoryNotFoundException){}if(Path.GetDirectoryName(current)==current)break;}}
        private static void Delete(string path){CheckReparse(path);File.Delete(path);}
        private IDisposable Exclusive()
        {
            CheckPaths();if(Path.DirectorySeparatorChar=='/')return Native.LockDirectory(directory);
            var mutex=new Mutex(false,mutexName);bool held=false;
            try{try{held=mutex.WaitOne(250);}catch(AbandonedMutexException){held=true;}if(!held)throw new TimeoutException("DIAGNOSTIC_WRITER_BUSY");return new HeldMutex(mutex);}
            catch{if(held)mutex.ReleaseMutex();mutex.Dispose();throw;}
        }
        private sealed class HeldMutex:IDisposable
        {private Mutex? mutex;internal HeldMutex(Mutex value){mutex=value;}public void Dispose(){var value=Interlocked.Exchange(ref mutex,null);if(value!=null){value.ReleaseMutex();value.Dispose();}}}
        private sealed class Row
        {internal Row(LocalDiagnosticEntry entry,byte[] bytes){Entry=entry;Bytes=bytes;}internal readonly LocalDiagnosticEntry Entry;internal readonly byte[] Bytes;}
        private static class Native
        {
#if UNITY_IOS && !UNITY_EDITOR
            private const string Lib="__Internal";
#else
            private const string Lib="libc";
#endif
            [DllImport(Lib,EntryPoint="rename",SetLastError=true)]private static extern int Rename([MarshalAs(UnmanagedType.LPUTF8Str)]string source,[MarshalAs(UnmanagedType.LPUTF8Str)]string destination);
#if UNITY_EDITOR || !UNITY_ANDROID && !UNITY_IOS
            [DllImport("kernel32.dll",EntryPoint="MoveFileExW",CharSet=CharSet.Unicode,SetLastError=true)][return:MarshalAs(UnmanagedType.Bool)]private static extern bool MoveFile(string source,string destination,int flags);
#endif
            [DllImport(Lib,EntryPoint="open",SetLastError=true)]private static extern int Open([MarshalAs(UnmanagedType.LPUTF8Str)]string path,int flags);
            [DllImport(Lib,EntryPoint="flock",SetLastError=true)]private static extern int Flock(int descriptor,int operation);
            [DllImport(Lib,EntryPoint="close")]private static extern int Close(int descriptor);
            internal static void Replace(string source,string destination)
            {
#if UNITY_EDITOR || !UNITY_ANDROID && !UNITY_IOS
                bool ok=Path.DirectorySeparatorChar=='\\'?MoveFile(source,destination,1|8):Rename(source,destination)==0;
#else
                bool ok=Rename(source,destination)==0;
#endif
                if(!ok)throw new IOException("DIAGNOSTIC_ATOMIC_REPLACE_FAILED");
            }
            internal static IDisposable LockDirectory(string path)
            {
                int flags;
#if UNITY_IOS && !UNITY_EDITOR
                flags=0x01000000;
#elif UNITY_ANDROID && !UNITY_EDITOR
                flags=1<<19;
#else
                if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))flags=0x01000000;else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))flags=1<<19;else throw new IOException("DIAGNOSTIC_NATIVE_LOCK_UNSUPPORTED");
#endif
                int descriptor=Open(path,flags);if(descriptor<0)throw new IOException("DIAGNOSTIC_NATIVE_LOCK_FAILED");
                try{var clock=System.Diagnostics.Stopwatch.StartNew();while(Flock(descriptor,2|4)!=0){int error=Marshal.GetLastWin32Error();if(error!=11&&error!=35&&error!=4)throw new IOException("DIAGNOSTIC_NATIVE_LOCK_FAILED");if(clock.ElapsedMilliseconds>=250)throw new TimeoutException("DIAGNOSTIC_WRITER_BUSY");Thread.Sleep(5);}return new HeldDirectory(descriptor);}
                catch{Close(descriptor);throw;}
            }
            private sealed class HeldDirectory:IDisposable
            {private int descriptor;internal HeldDirectory(int value){descriptor=value;}public void Dispose(){int value=Interlocked.Exchange(ref descriptor,-1);if(value>=0)Close(value);}}
        }
    }
}
