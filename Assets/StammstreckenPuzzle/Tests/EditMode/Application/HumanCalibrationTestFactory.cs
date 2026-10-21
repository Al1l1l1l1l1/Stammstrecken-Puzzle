using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;

namespace STP.Tests.Application.EditMode
{
    /// <summary>Only a test assembly supplies controlled observations; never included in a human player.</summary>
    public sealed class HumanCalibrationTestFactory:IDisposable
    {
        public sealed class TestClock:ILocalSessionClock{public long MonotonicMilliseconds{get;set;}public string UtcNow=>"2026-10-21T00:00:00Z";}
        public sealed class TestCatalog:IHumanAssignmentCatalog
        {
            public string ManifestBytesSha256=>new string('a',64);public string StudyId=>"controlled-human-test";public int Revision=>1;public string BuildId=>"controlled-automated-test";public string UiSourceSha256=>new string('b',64);public string PublicCatalogHashSha256=>new string('c',64);public IReadOnlyList<HumanAssignment> Assignments{get;}
            public TestCatalog()
            {var list=new List<HumanAssignment>();for(int i=0;i<2;i++){string id="S1-05-04-"+(11+i).ToString("D2");var temporary=new PublicPuzzleInput(new PublicPuzzleIdentity(id,PublicPuzzleIdentity.SemanticHashProfile,new string('0',64)),"train-track-v1",4,4,PublicPuzzleSide.W,0,PublicPuzzleSide.E,0,new[]{4,0,0,0},new[]{1,1,1,1});string hash=LocalSaveHash.Bytes(LocalSaveHash.Canonical(LocalSaveContractFixtures.Json,EndlessProjection.PublicInput(temporary,false)));var input=new PublicPuzzleInput(new PublicPuzzleIdentity(id,PublicPuzzleIdentity.SemanticHashProfile,hash),temporary.RulesetVersion,4,4,temporary.A.Side,0,temporary.B.Side,0,temporary.RowCounts,temporary.ColumnCounts);list.Add(new HumanAssignment("assigned-"+i,HumanAssignmentPurpose.CAMPAIGN_TIME,input,LocalSaveHash.Bytes(LocalSaveHash.Canonical(LocalSaveContractFixtures.Json,EndlessProjection.PublicInput(input,true))),new string('d',64),1,null,null));}Assignments=list.AsReadOnly();}
        }
        public sealed class MemoryRepository:IHumanCalibrationRepository
        {
            public HumanObservationSnapshot? Snapshot{get;set;}public HumanObservationSnapshot? Offered{get;set;}public HumanCalibrationRun? StartOffered{get;private set;}public List<HumanObservationSnapshot> Archived{get;}=new List<HumanObservationSnapshot>();public TaskCompletionSource<bool>? Hold{get;set;}public bool UnknownNext{get;set;}public bool Unknown{get;private set;}public int Commits{get;private set;}public Exception? ThrowNext{get;set;}
            public Task<HumanStoreResult> LoadAsync(CancellationToken ct){ct.ThrowIfCancellationRequested();return Task.FromResult(Unknown?new HumanStoreResult(HumanStoreStatus.RecoveryRequired,"CONTROLLED_UNKNOWN",observationToken:"controlled-observation"):Snapshot==null?new HumanStoreResult(HumanStoreStatus.Missing,"HUMAN_RUN_MISSING"):new HumanStoreResult(HumanStoreStatus.Ready,"NONE",Snapshot));}
            public async Task<HumanStoreResult> StartRunAsync(HumanCalibrationRun run,string? expected,CancellationToken ct)
            {StartOffered=run;if(Hold!=null){var hold=Hold;await hold.Task.ConfigureAwait(false);Hold=null;}if(ThrowNext!=null){var error=ThrowNext;ThrowNext=null;throw error;}ct.ThrowIfCancellationRequested();if(expected==null?Snapshot!=null:Snapshot?.Run.SessionToken!=expected||Snapshot.Run.NextAssignmentIndex!=2||run.SessionToken==expected)return new HumanStoreResult(HumanStoreStatus.Conflict,"CONTROLLED_EXISTS");if(Snapshot!=null)Archived.Add(Snapshot);Snapshot=new HumanObservationSnapshot(1,run);if(UnknownNext){UnknownNext=false;Unknown=true;return new HumanStoreResult(HumanStoreStatus.RecoveryRequired,"CONTROLLED_UNKNOWN",observationToken:"controlled-observation");}return new HumanStoreResult(HumanStoreStatus.Committed,"NONE",Snapshot);}
            public async Task<HumanStoreResult> CommitAsync(HumanObservationSnapshot offered,long expected,CancellationToken ct)
            {Offered=offered;Commits++;if(Hold!=null){var hold=Hold;await hold.Task.ConfigureAwait(false);Hold=null;}if(ThrowNext!=null){var error=ThrowNext;ThrowNext=null;throw error;}ct.ThrowIfCancellationRequested();if(Snapshot?.Generation!=expected)return new HumanStoreResult(HumanStoreStatus.Conflict,"CONTROLLED_CAS");Snapshot=offered;if(UnknownNext){UnknownNext=false;Unknown=true;return new HumanStoreResult(HumanStoreStatus.RecoveryRequired,"CONTROLLED_UNKNOWN",observationToken:"controlled-observation");}return new HumanStoreResult(HumanStoreStatus.Committed,"NONE",offered);}
            public Task<HumanStoreResult> RecoverAsync(string token,CancellationToken ct){ct.ThrowIfCancellationRequested();Unknown=false;return Task.FromResult(new HumanStoreResult(HumanStoreStatus.Ready,"NONE",Snapshot!));}
            public Task<byte[]> ExportAsync(CancellationToken ct)=>Task.FromResult(Array.Empty<byte>());
        }
        public sealed class QueueDispatcher:ILocalSaveDispatcher
        {
            private readonly int owner=Thread.CurrentThread.ManagedThreadId;private readonly ConcurrentQueue<(Action,TaskCompletionSource<bool>)> queue=new ConcurrentQueue<(Action,TaskCompletionSource<bool>)>();
            public Task InvokeAsync(Action action){if(Thread.CurrentThread.ManagedThreadId==owner){action();return Task.CompletedTask;}var completion=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);queue.Enqueue((action,completion));return completion.Task;}
            public int PendingCount=>queue.Count;
            public bool DrainOne(){if(!queue.TryDequeue(out var item))return false;try{item.Item1();item.Item2.TrySetResult(true);}catch(Exception error){item.Item2.TrySetException(error);}return true;}
            public void Drain(){while(DrainOne()){};}
        }
        public TestCatalog Catalog{get;}public MemoryRepository Repository{get;}public TestClock Clock{get;}public QueueDispatcher Dispatcher{get;}public AssignedHumanSession Session{get;}
        public HumanCalibrationTestFactory():this(null){}
        public HumanCalibrationTestFactory(MemoryRepository? repository)
        {Catalog=new TestCatalog();Repository=repository??new MemoryRepository();Clock=new TestClock();Dispatcher=new QueueDispatcher();Session=new AssignedHumanSession(Catalog,Repository,Clock,Dispatcher,HumanRunOrigin.AUTOMATED_TEST);Pump(Session.InitializeAsync(CancellationToken.None));}
        public void Until(Func<bool> predicate){var watch=Stopwatch.StartNew();while(!predicate()){Dispatcher.Drain();if(watch.ElapsedMilliseconds>15000)throw new TimeoutException("Controlled human dispatcher stalled");Thread.Sleep(1);}Dispatcher.Drain();}
        public void Pump(Task task){Until(()=>task.IsCompleted);task.GetAwaiter().GetResult();}
        public T Pump<T>(Task<T> task){Pump((Task)task);return task.GetAwaiter().GetResult();}
        public void OpenFirst(){Pump(Session.BeginRunAsync(CancellationToken.None));Pump(Session.ReserveExposureAsync(CancellationToken.None));ConfirmVisible();}
        public void CompleteRunByExplicitAborts(){if(!Session.State.HasRun)Pump(Session.BeginRunAsync(CancellationToken.None));while(Session.State.Assignment!=null){Pump(Session.ReserveExposureAsync(CancellationToken.None));Pump(Session.AbortAsync(CancellationToken.None));Pump(Session.NextAsync(CancellationToken.None));}}
        public void ConfirmVisible(){Clock.MonotonicMilliseconds++;Pump(Session.ConfirmExposureVisibleAsync(Session.State.Exposure!.AcknowledgeVisible(Clock.MonotonicMilliseconds),CancellationToken.None));Pump(Session.RecordFirstContactAsync(HumanFirstContact.NEVER_SEEN,false,CancellationToken.None));}
        public void Dispose(){Session.Dispose();Dispatcher.Drain();}
    }
}
