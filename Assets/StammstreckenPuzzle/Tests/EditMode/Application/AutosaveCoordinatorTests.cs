using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;

namespace STP.Tests.Application.EditMode
{
    public sealed class AutosaveCoordinatorTests
    {
        private static async Task<(LocalSaveContractFixtures.Setup Setup,PersistedPuzzleSession Use,AutosaveCoordinator Auto,LocalSaveContractFixtures.Repository Repo,LocalSaveContractFixtures.Clock Clock)> Ready()
        {var s=new LocalSaveContractFixtures.Setup();var clock=new LocalSaveContractFixtures.Clock();var repo=new LocalSaveContractFixtures.Repository{Loaded=SaveLoadResult<LocalProfileState>.Success(new SaveSnapshot<LocalProfileState>(s.Profile,s.Aggregate,1,s.Empty),s.Observe())};var use=new PersistedPuzzleSession(repo,s.Puzzles,s.Policy,s.Profile,s.Catalogs,clock,new LocalSaveContractFixtures.Dispatcher());await use.LoadAsync(CancellationToken.None);LocalSaveContractFixtures.Call(use,"Start",s.Puzzles.Puzzles[0].Identity,Guid.NewGuid());return(s,use,new AutosaveCoordinator(use),repo,clock);}
        [Test]
        public async Task Debounce499500AndContinuousMutation19992000UseExactMonotonicBoundaries()
        {
            var f=await Ready();LocalSaveContractFixtures.Mark(f.Use,f.Clock,100,"MARK_EMPTY",0);f.Auto.NotifyMutation(0);f.Clock.MonotonicMilliseconds=499;Assert.That(await f.Auto.TickAsync(499,CancellationToken.None),Is.Null);Assert.That(f.Repo.Commits,Is.Zero);f.Clock.MonotonicMilliseconds=500;Assert.That((await f.Auto.TickAsync(500,CancellationToken.None))!.Succeeded,Is.True);Assert.That(f.Auto.IsDirty,Is.False);
            f=await Ready();int i=0;foreach(long time in new long[]{0,400,800,1200,1600,1999}){LocalSaveContractFixtures.Mark(f.Use,f.Clock,100+i,i%2==0?"MARK_EMPTY":"MARK_OCCUPIED",time);f.Auto.NotifyMutation(time);i++;}Assert.That(await f.Auto.TickAsync(1999,CancellationToken.None),Is.Null);f.Clock.MonotonicMilliseconds=2000;Assert.That((await f.Auto.TickAsync(2000,CancellationToken.None))!.Succeeded,Is.True);Assert.That(f.Repo.Commits,Is.EqualTo(1));
        }
        [Test]
        public async Task OldAcknowledgementCannotClearOrLoseANewerActualMutation()
        {
            var f=await Ready();LocalSaveContractFixtures.Mark(f.Use,f.Clock,100,"MARK_EMPTY",0);f.Auto.NotifyMutation(0);var ack=new TaskCompletionSource<SaveCommitResult<LocalProfileState>>(TaskCreationOptions.RunContinuationsAsynchronously);var entered=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);f.Repo.OnCommit=(s,g,ct)=>{entered.TrySetResult(true);return ack.Task;};f.Clock.MonotonicMilliseconds=500;var saving=f.Auto.TickAsync(500,CancellationToken.None);await entered.Task;Assert.That(saving.IsCompleted,Is.False);LocalSaveContractFixtures.Mark(f.Use,f.Clock,101,"MARK_OCCUPIED",550);f.Auto.NotifyMutation(550);var old=f.Repo.LastOffered!;ack.SetResult(SaveCommitResult<LocalProfileState>.Success(old));await saving;Assert.That(f.Auto.IsDirty,Is.True);f.Repo.OnCommit=null;f.Clock.MonotonicMilliseconds=1049;Assert.That(await f.Auto.TickAsync(1049,CancellationToken.None),Is.Null);f.Clock.MonotonicMilliseconds=1050;await f.Auto.TickAsync(1050,CancellationToken.None);Assert.That(f.Auto.IsDirty,Is.False);Assert.That(f.Repo.Commits,Is.EqualTo(2));Assert.That(f.Use.Snapshot!.Generation,Is.EqualTo(3));var cp=LocalSaveContractFixtures.Get(f.Use.Snapshot.Payload.Drafts[0],"Checkpoint")!;Assert.That(LocalSaveContractFixtures.Get(cp,"Revision"),Is.EqualTo(2));
        }
        [Test]
        public async Task CompletionWaitsForEarlierDraftAndRemovesItInTheLaterConfirmedSnapshot()
        {
            var f=await Ready();LocalSaveContractFixtures.Mark(f.Use,f.Clock,100,"MARK_EMPTY",0);f.Auto.NotifyMutation(0);var ack=new TaskCompletionSource<SaveCommitResult<LocalProfileState>>(TaskCreationOptions.RunContinuationsAsynchronously);var entered=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);f.Repo.OnCommit=(s,g,ct)=>{entered.TrySetResult(true);return ack.Task;};f.Clock.MonotonicMilliseconds=500;var draft=f.Auto.TickAsync(500,CancellationToken.None);await entered.Task;LocalSaveContractFixtures.SolveCurrent(f.Use,f.Clock);f.Auto.NotifyMutation(f.Clock.MonotonicMilliseconds);var completion=f.Auto.CommitCompletionAsync(CancellationToken.None);Assert.That(completion.IsCompleted,Is.False);Assert.That(f.Use.CanPresentResults,Is.False);var offered=f.Repo.LastOffered!;f.Repo.OnCommit=null;ack.SetResult(SaveCommitResult<LocalProfileState>.Success(offered));await draft;await completion;Assert.That(f.Repo.Commits,Is.EqualTo(2));Assert.That(f.Use.Snapshot!.Generation,Is.EqualTo(3));Assert.That(f.Use.Snapshot.Payload.Drafts,Is.Empty);Assert.That(f.Use.CanPresentResults,Is.True);Assert.That(f.Auto.IsDirty,Is.False);
        }
        [Test]
        public async Task LifecycleFlushIsImmediateAndSuspendedBackgroundTimeDoesNotEnterCheckpoint()
        {
            var f=await Ready();LocalSaveContractFixtures.Mark(f.Use,f.Clock,100,"MARK_EMPTY",0);f.Auto.NotifyMutation(0);LocalSaveContractFixtures.Lifecycle(f.Use,f.Clock,101,true,500);f.Auto.NotifyMutation(500);f.Clock.MonotonicMilliseconds=10000;Assert.That((await f.Auto.FlushAsync(CancellationToken.None))!.Succeeded,Is.True);var cp=LocalSaveContractFixtures.Get(f.Use.Snapshot!.Payload.Drafts[0],"Checkpoint")!;Assert.That(LocalSaveContractFixtures.Get(cp,"ActiveElapsedMs"),Is.EqualTo(500));LocalSaveContractFixtures.Lifecycle(f.Use,f.Clock,102,false,10000);LocalSaveContractFixtures.Mark(f.Use,f.Clock,103,"MARK_OCCUPIED",10050);f.Auto.NotifyMutation(10050);await f.Auto.FlushAsync(CancellationToken.None);cp=LocalSaveContractFixtures.Get(f.Use.Snapshot!.Payload.Drafts[0],"Checkpoint")!;Assert.That(LocalSaveContractFixtures.Get(cp,"ActiveElapsedMs"),Is.EqualTo(550));
        }
        [Test]
        public async Task FailedSaveRetainsDirtyVersionAndAConfirmedRetryAloneClearsIt()
        {var f=await Ready();LocalSaveContractFixtures.Mark(f.Use,f.Clock,100,"MARK_EMPTY",0);f.Auto.NotifyMutation(0);var original=f.Use.Snapshot;f.Repo.OnCommit=(s,g,ct)=>Task.FromResult(SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.Failed,"SAVE_FLUSH_FAILED"));f.Clock.MonotonicMilliseconds=500;Assert.That((await f.Auto.TickAsync(500,CancellationToken.None))!.Succeeded,Is.False);Assert.That(f.Auto.IsDirty,Is.True);Assert.That(f.Use.Snapshot,Is.SameAs(original));f.Repo.OnCommit=null;Assert.That((await f.Auto.FlushAsync(CancellationToken.None))!.Succeeded,Is.True);Assert.That(f.Auto.IsDirty,Is.False);Assert.That(f.Use.Snapshot!.Generation,Is.EqualTo(2));}
        [Test]
        public async Task CancellingAQueuedLifecycleWaitDoesNotCancelOrDuplicateTheInFlightWrite()
        {var f=await Ready();LocalSaveContractFixtures.Mark(f.Use,f.Clock,100,"MARK_EMPTY",0);f.Auto.NotifyMutation(0);var ack=new TaskCompletionSource<SaveCommitResult<LocalProfileState>>(TaskCreationOptions.RunContinuationsAsynchronously);var entered=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);f.Repo.OnCommit=(s,g,ct)=>{entered.TrySetResult(true);return ack.Task;};f.Clock.MonotonicMilliseconds=500;var saving=f.Auto.TickAsync(500,CancellationToken.None);await entered.Task;using(var cancellation=new CancellationTokenSource()){var flush=f.Auto.FlushAsync(cancellation.Token);cancellation.Cancel();try{await flush;Assert.Fail("Queued waiter must cancel");}catch(OperationCanceledException){}}Assert.That(f.Auto.IsDirty,Is.True);Assert.That(f.Repo.Commits,Is.EqualTo(1));ack.SetResult(SaveCommitResult<LocalProfileState>.Success(f.Repo.LastOffered!));await saving;Assert.That(f.Auto.IsDirty,Is.False);Assert.That(f.Repo.Commits,Is.EqualTo(1));}
        [Test]
        public async Task CancelledUnknownCommitKeepsDirtyAndBlocksEffectsUntilFreshLoad()
        {var f=await Ready();LocalSaveContractFixtures.Mark(f.Use,f.Clock,100,"MARK_EMPTY",0);f.Auto.NotifyMutation(0);f.Repo.OnCommit=(s,g,ct)=>Task.FromException<SaveCommitResult<LocalProfileState>>(new OperationCanceledException());f.Clock.MonotonicMilliseconds=500;Assert.That((await f.Auto.TickAsync(500,CancellationToken.None))!.Status,Is.EqualTo(SaveCommitStatus.RecoveryRequired));Assert.That(f.Auto.IsDirty,Is.True);Assert.That(f.Use.CanMutate||f.Use.CanPresentResults,Is.False);Assert.That(f.Repo.Commits,Is.EqualTo(1));}
    }
}
