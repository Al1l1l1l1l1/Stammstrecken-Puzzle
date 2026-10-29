using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Infrastructure.Persistence;

namespace STP.Tests.Persistence.EditMode
{
    /// <summary>Real bounded private files and the actual shared strict JSON adapter; no Unity or provider runs.</summary>
    public sealed class LocalDiagnosticRingTests
    {
        private string root=null!,fixtureBase=null!,profile=null!;
        private readonly List<LocalDiagnosticRing> owned=new List<LocalDiagnosticRing>();
        private string DirectoryPath=>Path.Combine(profile,LocalDiagnosticRing.DirectoryName);
        private string RingPath=>Path.Combine(DirectoryPath,LocalDiagnosticRing.RingFileName);
        private string PendingPath=>Path.Combine(DirectoryPath,LocalDiagnosticRing.PendingFileName);
        [SetUp]public void Setup()
        {
            fixtureBase=Path.GetFullPath(Environment.GetEnvironmentVariable("STP_TEST_TEMP_ROOT")??Path.GetTempPath());
            root=Path.Combine(fixtureBase,"stp-diag-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);
            File.WriteAllText(Path.Combine(root,"fixture-owned.marker"),"LocalDiagnosticRingTests");profile=Path.Combine(root,"profile");Directory.CreateDirectory(profile);
            File.WriteAllText(Path.Combine(profile,"save.json"),"unchanged business save sentinel");
        }
        [TearDown]public void Cleanup()
        {
            foreach(var ring in owned)ring.CloseAndDrainAsync().GetAwaiter().GetResult();owned.Clear();
            string full=Path.GetFullPath(root),prefix=fixtureBase.TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar)+Path.DirectorySeparatorChar;
            Assert.That(full.StartsWith(prefix,StringComparison.OrdinalIgnoreCase)&&Path.GetFileName(full).StartsWith("stp-diag-",StringComparison.Ordinal),Is.True);
            Assert.That(File.ReadAllText(Path.Combine(full,"fixture-owned.marker")),Is.EqualTo("LocalDiagnosticRingTests"));Directory.Delete(full,true);
        }
        private static object Adapter()=>Activator.CreateInstance(Assembly.Load("STP.Infrastructure.Content").GetType("STP.Infrastructure.Content.CanonicalJsonAdapter",true)!)!;
        private LocalDiagnosticRing Ring(LocalDiagnosticLimits? limits=null,bool production=true,bool debug=false,Action<LocalDiagnosticFaultPoint>? fault=null)
        {
            var env=new LocalDiagnosticEnvironment("wp046-diagnostic-tests","1.0",LocalDiagnosticPlatform.Editor);
            var ring=(LocalDiagnosticRing)Activator.CreateInstance(typeof(LocalDiagnosticRing),profile,Adapter(),env,production,debug,limits,new Func<DateTimeOffset>(()=>new DateTimeOffset(2026,10,21,12,0,0,TimeSpan.FromHours(2))),fault)!;owned.Add(ring);return ring;
        }
        private static void Run(Func<Task> action)=>Task.Run(action).GetAwaiter().GetResult();
        private static byte[] Canonical(string text)
        {
            object adapter=Adapter();object?[] args={Encoding.UTF8.GetBytes(text),1048576,8,1024,1024,32,null};
            return(byte[])adapter.GetType().GetMethod("CanonicalizeBytes")!.Invoke(adapter,args)!;
        }
        private static async Task<LocalDiagnosticWriteStatus> Service(LocalDiagnosticRing ring,LocalDiagnosticOperation operation,string status,string? rawCode=null)
        {
            Type type=Assembly.Load("STP.Application").GetType("STP.Application.MobileServiceStatus",true)!;
            return await ((Task<LocalDiagnosticWriteStatus>)typeof(LocalDiagnosticRing).GetMethod("RecordServiceResultAsync")!.Invoke(ring,new object?[]{operation,Enum.Parse(type,status),rawCode})!).ConfigureAwait(false);
        }
        [Test]public void RealCanonicalJsonlHasOnlyClosedFieldsUtcAndLocalSequenceAndProductionDebugIsOff()=>Run(async()=>
        {
            var ring=Ring(debug:true);Assert.That(await ring.RecordAsync(LocalDiagnosticEvent.DebugBreadcrumb),Is.EqualTo(LocalDiagnosticWriteStatus.Dropped));Assert.That(Directory.Exists(DirectoryPath),Is.False);
            Assert.That(await ring.RecordAsync(LocalDiagnosticEvent.AppStarted),Is.EqualTo(LocalDiagnosticWriteStatus.Written));
            Assert.That(await ring.RecordServiceNoticeAsync("MOBILE_PROFILE_SYNCED"),Is.EqualTo(LocalDiagnosticWriteStatus.Written));
            var view=await ring.ReadSnapshotAsync();Assert.That(view!.Entries.Count,Is.EqualTo(2));Assert.That(view.Entries[0].TimestampUtc,Is.EqualTo("2026-10-21T10:00:00.000Z"));
            Assert.That(view.Entries.Select(e=>e.Sequence),Is.EqualTo(new long[]{1,2}));
            string[] lines=File.ReadAllLines(RingPath);Assert.That(lines.All(l=>Encoding.UTF8.GetBytes(l).SequenceEqual(Canonical(l))),Is.True);
            Assert.That(lines[0],Does.Contain("\"correlationId\":\"local-1\"").And.Contain("\"fields\":{\"category\":\"None\",\"outcome\":\"Succeeded\"}"));Assert.That(File.Exists(PendingPath),Is.False);
        });
        [Test]public void StrictRedactionNeverTouchesOrPersistsExceptionTextDataOrProviderIdentifier()=>Run(async()=>
        {
            var ring=Ring();var e=new SecretException();e.Data["receipt"]="SECRET_RECEIPT_CELL_CONTENT";
            await ring.RecordExceptionAsync(e);await ring.RecordExceptionAsync(new IOException("C:/private/SECRET_SAVE.json"),true);
            Assert.That(await ring.RecordServiceNoticeAsync("USER@EXAMPLE.ORG_SECRET_TRANSACTION"),Is.EqualTo(LocalDiagnosticWriteStatus.Dropped));
            Assert.That(await ring.RecordAsync((LocalDiagnosticEvent)999),Is.EqualTo(LocalDiagnosticWriteStatus.Dropped));
            await Service(ring,LocalDiagnosticOperation.Purchase,"Pending","SECRET_PROVIDER_TRANSACTION_RECEIPT");
            string bytes=File.ReadAllText(RingPath);Assert.That(bytes,Does.Not.Contain("SECRET").And.Not.Contain("private").And.Not.Contain("@"));
            var rows=(await ring.ReadSnapshotAsync())!.Entries;Assert.That(rows.Count,Is.EqualTo(3));Assert.That(rows[0].Category,Is.EqualTo(LocalDiagnosticCategory.Other));Assert.That(rows[1].Severity,Is.EqualTo("FATAL"));Assert.That(rows[1].Category,Is.EqualTo(LocalDiagnosticCategory.IO));
        });
        private sealed class SecretException:Exception {public override string Message=>throw new InvalidOperationException("Message was accessed");}
        [TestCase("bad@example.org")][TestCase("C:/private/path")][TestCase("wp046-good\nSECRET")][TestCase("wp046-good\"receipt")]
        public void ArbitraryBuildStringsAreNotDiagnosticMetadata(string build)
        {Assert.Throws<ArgumentException>(()=>new LocalDiagnosticEnvironment(build,"1.0",LocalDiagnosticPlatform.Editor));Assert.That(Directory.Exists(DirectoryPath),Is.False);}
        [Test]public void StoreAdsAggregatesAreOnlyTheRetainedWindowAndNoFillIsNotAnException()=>Run(async()=>
        {
            var ring=Ring(new LocalDiagnosticLimits(3));await Service(ring,LocalDiagnosticOperation.Rewarded,"Unavailable","ADS_NO_FILL");await Service(ring,LocalDiagnosticOperation.Purchase,"Pending");await Service(ring,LocalDiagnosticOperation.Restore,"Succeeded");
            var before=(await ring.ReadSnapshotAsync())!;Assert.That(before.Entries[0].Outcome,Is.EqualTo(LocalDiagnosticOutcome.NoFill));Assert.That(before.Entries[0].Severity,Is.EqualTo("INFO"));Assert.That(before.Aggregates["ADS-REWARDED-RESULT:NoFill"],Is.EqualTo(1));
            await Service(ring,LocalDiagnosticOperation.Consent,"NotAllowed");var after=(await ring.ReadSnapshotAsync())!;Assert.That(after.Aggregates.ContainsKey("ADS-REWARDED-RESULT:NoFill"),Is.False);Assert.That(after.Entries.Count,Is.EqualTo(3));
        });
        [Test]public void ThousandRowDefaultCapacityAndMonotonicSequenceSurviveRestartWithoutAppendOverflow()=>Run(async()=>
        {
            var ring=Ring();await ring.RecordAsync(LocalDiagnosticEvent.AppStarted);await ring.CloseAndDrainAsync();string line=File.ReadAllLines(RingPath).Single();
            File.WriteAllText(RingPath,string.Concat(Enumerable.Range(1,1000).Select(i=>line.Replace("\"monotonicSequence\":1","\"monotonicSequence\":"+i).Replace("\"local-1\"","\"local-"+i+"\"")+"\n")),new UTF8Encoding(false));
            var restarted=Ring();Assert.That(await restarted.RecordAsync(LocalDiagnosticEvent.AppStopped),Is.EqualTo(LocalDiagnosticWriteStatus.Written));var snapshot=(await restarted.ReadSnapshotAsync())!;
            Assert.That(snapshot.Entries.Count,Is.EqualTo(1000));Assert.That(snapshot.Entries.First().Sequence,Is.EqualTo(2));Assert.That(snapshot.Entries.Last().Sequence,Is.EqualTo(1001));Assert.That(snapshot.Breadcrumbs.Count,Is.EqualTo(32));Assert.That(new FileInfo(RingPath).Length,Is.LessThanOrEqualTo(1048576));
        });
        [Test]public void ByteLimitRotatesBeforeEntryLimitAndNeitherMaximumCanBeRelaxed()=>Run(async()=>
        {
            Assert.Throws<ArgumentException>(()=>new LocalDiagnosticLimits(1001));Assert.Throws<ArgumentException>(()=>new LocalDiagnosticLimits(1000,1048577));
            var ring=Ring(new LocalDiagnosticLimits(1000,900));for(int i=0;i<7;i++)Assert.That(await ring.RecordAsync(LocalDiagnosticEvent.AppStarted),Is.EqualTo(LocalDiagnosticWriteStatus.Written));
            var rows=(await ring.ReadSnapshotAsync())!.Entries;Assert.That(rows.Count,Is.LessThan(7));Assert.That(rows.Last().Sequence,Is.EqualTo(7));Assert.That(new FileInfo(RingPath).Length,Is.LessThanOrEqualTo(900));Assert.That(File.Exists(PendingPath),Is.False);
        });
        [Test]public void OnlyExplicitNonProductionDevelopmentCanWriteDebugAndStalePendingIsNeverAHistorySource()=>Run(async()=>
        {
            var ring=Ring(production:false,debug:true);Assert.That(await ring.RecordAsync(LocalDiagnosticEvent.DebugBreadcrumb),Is.EqualTo(LocalDiagnosticWriteStatus.Written));await ring.CloseAndDrainAsync();
            File.WriteAllText(PendingPath,"SECRET_RECEIPT_AND_SAVE_CONTENT");var restarted=Ring();Assert.That((await restarted.ReadSnapshotAsync())!.Entries.Count,Is.EqualTo(1));
            Assert.That(await restarted.RecordAsync(LocalDiagnosticEvent.AppStarted),Is.EqualTo(LocalDiagnosticWriteStatus.Written));Assert.That(File.Exists(PendingPath),Is.False);Assert.That(File.ReadAllText(RingPath),Does.Not.Contain("SECRET"));
        });
        [TestCase("relative/profile")][TestCase("\\\\server\\share\\profile")][TestCase("//server/share/profile")][TestCase("C:drive-relative")]
        public void RelativeAndNetworkRootsAreRejectedBeforeAnyFilesystemOperation(string path)
        {
            var env=new LocalDiagnosticEnvironment("wp046-diagnostic-tests","1.0",LocalDiagnosticPlatform.Editor);
            var error=Assert.Throws<TargetInvocationException>(()=>Activator.CreateInstance(typeof(LocalDiagnosticRing),path,Adapter(),env,true,false,null,null,null));Assert.That(error!.InnerException,Is.TypeOf<ArgumentException>());
        }
        [TestCase("json")][TestCase("utf8")][TestCase("newline")][TestCase("unknown-field")][TestCase("duplicate-key")][TestCase("foreign-correlation")][TestCase("unknown-code")][TestCase("unsafe-sequence")][TestCase("order")][TestCase("oversize")]
        public void CorruptDiagnosticOnlyIsReplacedWithoutRetainingInjectedSecretsOrTouchingSave(string variant)=>Run(async()=>
        {
            var first=Ring();await first.RecordAsync(LocalDiagnosticEvent.AppStarted);await first.CloseAndDrainAsync();string line=File.ReadAllLines(RingPath).Single();byte[] raw;
            switch(variant)
            {
                case "json":raw=Encoding.UTF8.GetBytes("{SECRET_RECEIPT\n");break;
                case "utf8":raw=new byte[]{0xff,10};break;
                case "newline":raw=Encoding.UTF8.GetBytes(line);break;
                case "unknown-field":raw=Canonical(line.Substring(0,line.Length-1)+",\"receipt\":\"SECRET_TRANSACTION\"}").Concat(new byte[]{10}).ToArray();break;
                case "duplicate-key":raw=Encoding.UTF8.GetBytes(line.Substring(0,line.Length-1)+",\"eventCode\":\"SECRET_TRANSACTION\"}\n");break;
                case "foreign-correlation":raw=Encoding.UTF8.GetBytes(line.Replace("local-1","SECRET_TRANSACTION")+"\n");break;
                case "unknown-code":raw=Encoding.UTF8.GetBytes(line.Replace("BLD-APP-STARTED","SECRET_TRANSACTION")+"\n");break;
                case "unsafe-sequence":raw=Encoding.UTF8.GetBytes(line.Replace("\"monotonicSequence\":1","\"monotonicSequence\":9007199254740992")+"\n");break;
                case "order":raw=Encoding.UTF8.GetBytes(line+"\n"+line+"\n");break;
                default:raw=Enumerable.Repeat((byte)'x',1048577).ToArray();break;
            }
            File.WriteAllBytes(RingPath,raw);var ring=Ring();Assert.That(await ring.ReadSnapshotAsync(),Is.Null);Assert.That(await ring.RecordAsync(LocalDiagnosticEvent.AppStarted),Is.EqualTo(LocalDiagnosticWriteStatus.Written));
            var rows=(await ring.ReadSnapshotAsync())!.Entries;Assert.That(rows.Select(r=>r.EventCode),Is.EqualTo(new[]{"SAV-DIAGNOSTIC-RING-RESET","BLD-APP-STARTED"}));Assert.That(File.ReadAllText(RingPath),Does.Not.Contain("SECRET"));Assert.That(File.ReadAllText(Path.Combine(profile,"save.json")),Is.EqualTo("unchanged business save sentinel"));
        });
        [TestCase(LocalDiagnosticFaultPoint.BeforeRead)][TestCase(LocalDiagnosticFaultPoint.BeforeWrite)][TestCase(LocalDiagnosticFaultPoint.AfterFlush)][TestCase(LocalDiagnosticFaultPoint.BeforeReplace)][TestCase(LocalDiagnosticFaultPoint.AfterReplace)]
        public void IoAndAtomicRotationWindowsYieldOnlyCompleteOldOrNewRing(LocalDiagnosticFaultPoint point)=>Run(async()=>
        {
            var first=Ring();await first.RecordAsync(LocalDiagnosticEvent.AppStarted);await first.CloseAndDrainAsync();byte[] original=File.ReadAllBytes(RingPath);
            var broken=Ring(fault:p=>{if(p==point)throw new IOException("SECRET_OS_PATH");});Assert.That(await broken.RecordAsync(LocalDiagnosticEvent.AppStopped),Is.EqualTo(LocalDiagnosticWriteStatus.Unavailable));await broken.CloseAndDrainAsync();
            var restarted=Ring();var snapshot=(await restarted.ReadSnapshotAsync())!;Assert.That(snapshot,Is.Not.Null);Assert.That(snapshot.Entries.Count,Is.EqualTo(point==LocalDiagnosticFaultPoint.AfterReplace?2:1));
            if(point!=LocalDiagnosticFaultPoint.AfterReplace)Assert.That(File.ReadAllBytes(RingPath),Is.EqualTo(original));
            Assert.That(await restarted.RecordAsync(LocalDiagnosticEvent.AppStarted),Is.EqualTo(LocalDiagnosticWriteStatus.Written));Assert.That(File.Exists(PendingPath),Is.False);Assert.That(File.ReadAllText(RingPath),Does.Not.Contain("SECRET"));
        });
        [Test]public void MultiplePhysicalInstancesSerializeRowsAndSequenceInsteadOfOverwritingEachOther()=>Run(async()=>
        {
            var a=Ring();var b=Ring();var tasks=Enumerable.Range(0,24).Select(i=>(i%2==0?a:b).RecordAsync(LocalDiagnosticEvent.AppStarted)).ToArray();
            var statuses=await Task.WhenAll(tasks);Assert.That(statuses.All(s=>s==LocalDiagnosticWriteStatus.Written),Is.True);
            var rows=(await a.ReadSnapshotAsync())!.Entries;Assert.That(rows.Count,Is.EqualTo(24));Assert.That(rows.Select(e=>e.Sequence),Is.EqualTo(Enumerable.Range(1,24).Select(i=>(long)i)));
        });
        [Test]public void CloseSealsBeforeDrainAndClearCannotRaceHeldWriteOrAcceptLateNotice()=>Run(async()=>
        {
            using var entered=new ManualResetEventSlim();using var release=new ManualResetEventSlim();
            var ring=Ring(fault:p=>{if(p==LocalDiagnosticFaultPoint.BeforeWrite){entered.Set();if(!release.Wait(10000))throw new TimeoutException();}});
            Task<LocalDiagnosticWriteStatus> write=ring.RecordAsync(LocalDiagnosticEvent.AppStarted);Assert.That(entered.Wait(5000),Is.True);
            Task<bool> clear=ring.ClearAsync();Assert.That(ring.IsClosed,Is.True);Assert.That(clear.IsCompleted,Is.False);Assert.That(await ring.RecordServiceNoticeAsync("MOBILE_PROFILE_SYNCED"),Is.EqualTo(LocalDiagnosticWriteStatus.Closed));
            release.Set();Assert.That(await write,Is.EqualTo(LocalDiagnosticWriteStatus.Written));Assert.That(await clear,Is.True);Assert.That(await ring.ClearAsync(),Is.True);Assert.That(File.Exists(RingPath)||File.Exists(PendingPath),Is.False);
            Assert.That(File.ReadAllText(Path.Combine(profile,"save.json")),Is.EqualTo("unchanged business save sentinel"));
        });
        [Test]public void ExternalCloseWaitsForAlreadyStartedClearWithoutSelfAwait()=>Run(async()=>
        {
            using var entered=new ManualResetEventSlim();using var release=new ManualResetEventSlim();
            var ring=Ring(fault:p=>{if(p==LocalDiagnosticFaultPoint.BeforeClear){entered.Set();if(!release.Wait(10000))throw new TimeoutException();}});
            Assert.That(await ring.RecordAsync(LocalDiagnosticEvent.AppStarted),Is.EqualTo(LocalDiagnosticWriteStatus.Written));
            Task<bool> clearing=ring.ClearAsync();Assert.That(entered.Wait(5000),Is.True);
            try
            {
                Task closing=ring.CloseAndDrainAsync();Assert.That(closing.IsCompleted,Is.False);
                Assert.That(await ring.RecordServiceNoticeAsync("MOBILE_PROFILE_SYNCED"),Is.EqualTo(LocalDiagnosticWriteStatus.Closed));
                release.Set();Assert.That(await clearing,Is.True);await closing;
                Assert.That(File.Exists(RingPath)||File.Exists(PendingPath),Is.False);
                // A new profile owner can only create its fresh ring after the old clear has drained.
                var fresh=Ring();Assert.That(await fresh.RecordAsync(LocalDiagnosticEvent.AppStarted),Is.EqualTo(LocalDiagnosticWriteStatus.Written));
                await ring.CloseAndDrainAsync();Assert.That(File.Exists(RingPath),Is.True);
            }
            finally{release.Set();}
        });
        [Test]public void QueueIsBoundedAndExplicitClearRetriesIoFailureWithoutReopeningWriter()=>Run(async()=>
        {
            using var entered=new ManualResetEventSlim();using var release=new ManualResetEventSlim();bool failClear=true;
            var ring=Ring(fault:p=>{if(p==LocalDiagnosticFaultPoint.BeforeWrite){entered.Set();if(!release.Wait(10000))throw new TimeoutException();}if(p==LocalDiagnosticFaultPoint.BeforeClear&&failClear){failClear=false;throw new IOException();}});
            var writes=new List<Task<LocalDiagnosticWriteStatus>>{ring.RecordAsync(LocalDiagnosticEvent.AppStarted)};Assert.That(entered.Wait(5000),Is.True);
            for(int i=0;i<LocalDiagnosticRing.MaximumQueuedOperations+8;i++)writes.Add(ring.RecordAsync(LocalDiagnosticEvent.ProfileSynced));Assert.That(writes.Count(t=>t.IsCompleted&&t.Result==LocalDiagnosticWriteStatus.Dropped),Is.GreaterThanOrEqualTo(8));
            var closing=ring.CloseAndDrainAsync();Assert.That(closing.IsCompleted,Is.False);release.Set();await Task.WhenAll(writes);await closing;
            Assert.That(await ring.ClearAsync(),Is.False);Assert.That(await ring.RecordAsync(LocalDiagnosticEvent.AppStarted),Is.EqualTo(LocalDiagnosticWriteStatus.Closed));Assert.That(await ring.ClearAsync(),Is.True);
        });
        [Test]public void MissingProfileDoesNotCreateAuthorityAndClearPreservesNeighborsAndOtherProfiles()=>Run(async()=>
        {
            Assert.That(Path.GetFullPath(profile).StartsWith(Path.GetFullPath(root)+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase),Is.True);
            Directory.Delete(profile,true);var ring=Ring();Assert.That(await ring.RecordAsync(LocalDiagnosticEvent.AppStarted),Is.EqualTo(LocalDiagnosticWriteStatus.Unavailable));Assert.That(Directory.Exists(profile),Is.False);Assert.That(await ring.ClearAsync(),Is.True);
            Directory.CreateDirectory(profile);var another=Ring();await another.RecordAsync(LocalDiagnosticEvent.AppStarted);string foreign=Path.Combine(root,"other-profile");Directory.CreateDirectory(foreign);File.WriteAllText(Path.Combine(foreign,"sentinel"),"foreign");File.WriteAllText(Path.Combine(DirectoryPath,"unknown.json"),"neighbor");
            Assert.That(await another.ClearAsync(),Is.True);Assert.That(File.ReadAllText(Path.Combine(DirectoryPath,"unknown.json")),Is.EqualTo("neighbor"));Assert.That(File.ReadAllText(Path.Combine(foreign,"sentinel")),Is.EqualTo("foreign"));
        });
        [Test]public void ActualReparseDirectoryIsNeverFollowedForWriteOrClear()=>Run(async()=>
        {
            string target=Path.Combine(root,"reparse-target");Directory.CreateDirectory(target);string sentinel=Path.Combine(target,LocalDiagnosticRing.RingFileName);File.WriteAllText(sentinel,"outside ring sentinel");
            bool windows=Path.DirectorySeparatorChar=='\\';
            var start=new ProcessStartInfo
            {
                FileName=windows?"cmd.exe":"/bin/ln",
                Arguments=windows?"/d /c mklink /J \""+DirectoryPath+"\" \""+target+"\"":"-s \""+target+"\" \""+DirectoryPath+"\"",
                UseShellExecute=false,CreateNoWindow=true,WindowStyle=ProcessWindowStyle.Hidden,RedirectStandardOutput=true,RedirectStandardError=true
            };
            try
            {
                using(var process=Process.Start(start)!){string output=process.StandardOutput.ReadToEnd()+process.StandardError.ReadToEnd();Assert.That(process.WaitForExit(10000),Is.True);Assert.That(process.ExitCode,Is.Zero,output);}
                Assert.That((File.GetAttributes(DirectoryPath)&FileAttributes.ReparsePoint)!=0,Is.True);var ring=Ring();Assert.That(await ring.RecordAsync(LocalDiagnosticEvent.AppStarted),Is.EqualTo(LocalDiagnosticWriteStatus.Unavailable));Assert.That(await ring.ClearAsync(),Is.False);Assert.That(File.ReadAllText(sentinel),Is.EqualTo("outside ring sentinel"));
            }
            finally{Assert.That(Path.GetFullPath(DirectoryPath).StartsWith(Path.GetFullPath(root)+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase),Is.True);if(Directory.Exists(DirectoryPath)&&(File.GetAttributes(DirectoryPath)&FileAttributes.ReparsePoint)!=0)Directory.Delete(DirectoryPath,false);}
        });
    }
}
