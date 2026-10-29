using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;
using STP.Infrastructure.Content;
using STP.Infrastructure.Persistence;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

namespace STP.Tests.Calibration.EditMode
{
    /// <summary>Uses the frozen ADR044 synthetic FIXTURE_ONLY source fixture, never a production source or approval.</summary>
    public sealed class LockedRuntimeBridgeTests
    {
        private static readonly object Source=SharedFixture();
        private static object SharedFixture()
        {
            // Reuse the exact independently reviewed synthetic source set; no parallel hand-written catalog truth.
            var lazy=typeof(CalibratedEndlessTests).GetField("Shared",BindingFlags.Static|BindingFlags.NonPublic)!.GetValue(null)!;
            return lazy.GetType().GetProperty("Value")!.GetValue(lazy)!;
        }
        private static T F<T>(string field)=>(T)Source.GetType().GetField(field,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(Source)!;
        private sealed class Clock:ILocalSessionClock
        {public string UtcNow=>"2026-10-21T00:00:00Z";public long Now;public long MonotonicMilliseconds=>Now;}
        private sealed class Owner:ILocalSaveDispatcher
        {
            private readonly int thread=Thread.CurrentThread.ManagedThreadId;
            private readonly ConcurrentQueue<(Action,TaskCompletionSource<bool>)> queue=new ConcurrentQueue<(Action,TaskCompletionSource<bool>)>();
            public Task InvokeAsync(Action action){if(Thread.CurrentThread.ManagedThreadId==thread){action();return Task.CompletedTask;}var t=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);queue.Enqueue((action,t));return t.Task;}
            public void Drain(){while(queue.TryDequeue(out var t)){try{t.Item1();t.Item2.TrySetResult(true);}catch(Exception e){t.Item2.TrySetException(e);}}}
            public void Until(Func<bool> done){var timer=Stopwatch.StartNew();while(!done()){Drain();if(timer.ElapsedMilliseconds>30000)Assert.Fail("Fixture owner queue did not settle");Thread.Sleep(1);}Drain();}
            public void Pump(Task task){Until(()=>task.IsCompleted);task.GetAwaiter().GetResult();}
            public T Pump<T>(Task<T> task){Pump((Task)task);return task.GetAwaiter().GetResult();}
        }
        private sealed class Repository:ILocalProfileRepository
        {
            internal readonly AtomicSaveRepository Inner;internal bool Hold;internal TaskCompletionSource<SaveCommitResult<LocalProfileState>>? Waiting;internal SaveCommitResult<LocalProfileState>? Committed;internal bool Unknown;
            internal Repository(AtomicSaveRepository inner){Inner=inner;}
            public Task<SaveLoadResult<LocalProfileState>> LoadAsync(CancellationToken ct)=>Inner.LoadAsync(ct);
            public Task<SaveLoadResult<LocalProfileState>> RecoverAsync(SaveRecoveryRequest request,CancellationToken ct)=>Inner.RecoverAsync(request,ct);
            public Task<SaveCommitResult<LocalProfileState>> CreateProfileAsync(LocalProfileState state,CancellationToken ct)=>Inner.CreateProfileAsync(state,ct);
            public Task<SaveCommitResult<LocalProfileState>> ConfirmedResetCorruptProfileAsync(SaveRepositoryObservation observation,LocalProfileState state,CancellationToken ct)=>Inner.ConfirmedResetCorruptProfileAsync(observation,state,ct);
            public async Task<SaveCommitResult<LocalProfileState>> CommitAsync(SaveSnapshot<LocalProfileState> snapshot,long expected,CancellationToken ct)
            {
                if(Unknown)return SaveCommitResult<LocalProfileState>.Failure(SaveCommitStatus.RecoveryRequired,"FIXTURE_UNCERTAIN_WRITE");
                var result=await Inner.CommitAsync(snapshot,expected,ct).ConfigureAwait(false);
                if(!Hold)return result;Committed=result;Waiting=new TaskCompletionSource<SaveCommitResult<LocalProfileState>>(TaskCreationOptions.RunContinuationsAsynchronously);return await Waiting.Task.ConfigureAwait(false);
            }
            internal void Release(){Hold=false;Waiting!.SetResult(Committed!);}
        }
        private sealed class Fixture:IDisposable
        {
            internal readonly Owner Owner=new Owner();internal readonly Clock Clock=new Clock();internal readonly string Root;
            internal readonly SaveProfileIdentity Profile=F<SaveProfileIdentity>("Profile");internal readonly RuntimePublicPuzzleCatalog Corpus=F<RuntimePublicPuzzleCatalog>("Corpus");internal readonly RuntimeProgressCatalog Catalogs=F<RuntimeProgressCatalog>("ProgressCatalog");
            internal readonly LocalProgressPolicy Progress=F<LocalProgressPolicy>("Progress");internal readonly LocalEndlessPolicy Endless=F<LocalEndlessPolicy>("Endless");internal readonly LocalSaveCodec Codec=F<LocalSaveCodec>("Codec");
            internal readonly LocalMetagamePolicy Hints;internal Repository Repo=null!;internal PersistedPuzzleSession Use=null!;internal SavedPublic240ReviewSession Saved=null!;
            internal Fixture()
            {
                Root=Path.GetFullPath(Path.Combine(Environment.GetEnvironmentVariable("STP_SOURCE_ROOT")??Directory.GetCurrentDirectory(),".utmp","bridge-"+Guid.NewGuid().ToString("N").Substring(0,8)));Directory.CreateDirectory(Root);
                Hints=new LocalMetagamePolicy(Corpus,F<RuntimeCompletionCatalog>("CampaignCompletion").Identity,F<SaveLedgerCodec>("Ledger"));Bind();
                Assert.That(Owner.Pump(Repo.CreateProfileAsync(F<LocalProfileState>("Cleared"),CancellationToken.None)).Succeeded,Is.True);Owner.Pump(Saved.InitializeAsync(CancellationToken.None));
            }
            private void Bind()
            {
                Repo=new Repository(new AtomicSaveRepository(new SaveFileSystem(Root,Profile,Catalogs.CatalogSet.AggregateCatalog),Codec,Clock,"fixture","fixture"));Use=new PersistedPuzzleSession(Repo,Corpus,Progress,Profile,Catalogs.CatalogSet,Clock,Owner);Use.ConfigureEndless(Endless);
                Saved=SavedPublic240ReviewSession.ForLockedRuntime(Use,new AutosaveCoordinator(Use),Corpus,Clock,Owner,Profile);
            }
            internal void Reload(){Saved.Dispose();Owner.Drain();Bind();Owner.Pump(Saved.InitializeAsync(CancellationToken.None));}
            internal void Hint()
            {
                var hint=Owner.Pump(Saved.PrepareHintAsync(Hints,CancellationToken.None));Assert.That(hint,Is.Not.Null);Assert.That(Owner.Pump(Saved.CommitHintDisplayAsync(Hints,hint!.AcknowledgeDisplayed(),CancellationToken.None)).Succeeded,Is.True);
                Assert.That(Hints.HintCredits(Saved.ConfirmedSnapshot!.Payload,Saved.State.CurrentIdentity!),Is.EqualTo(2));Owner.Pump(Saved.SetSuspendedAsync(false,CancellationToken.None));
            }
            internal void SolveCurrent()
            {
                var view=Saved.PuzzleView!;var session=Use.Session!;var solved=PuzzleSolver.Solve(session.Definition,new SolverLimits(1000000,5000));Assert.That(solved.Classification,Is.EqualTo(SolverClassification.UNIQUE));
                Direction Toward(CellCoordinate a,CellCoordinate b)=>b.Y<a.Y?Direction.N:b.X>a.X?Direction.E:b.Y>a.Y?Direction.S:Direction.W;
                for(int i=0;i<solved.Path.Count;i++){var cell=solved.Path[i];var incoming=i==0?session.Definition.A.Side:Toward(cell,solved.Path[i-1]);var outgoing=i==solved.Path.Count-1?session.Definition.B.Side:Toward(cell,solved.Path[i+1]);int ports=(1<<(int)incoming)|(1<<(int)outgoing);int shape=Enumerable.Range(0,6).Single(n=>TrackGeometry.Ports((TrackShape)n)==ports);Clock.Now=1000*i/(solved.Path.Count-1);Assert.That(view.Apply(cell.X,cell.Y,(DevelopmentTool)(shape+3),Clock.Now),Is.EqualTo(DevelopmentCommandStatus.Applied));}
            }
            internal void Ride(){var attempt=Saved.PuzzleView!.AttemptId;Assert.That(Saved.BeginTrainRide(attempt),Is.EqualTo(DevelopmentCommandStatus.Applied));Assert.That(Saved.PuzzleView.CompleteTrainRide(attempt),Is.EqualTo(DevelopmentCommandStatus.Applied));Assert.That(Saved.State.Status,Is.EqualTo(SavedReviewStatus.Results));}
            public void Dispose(){Saved.Dispose();Owner.Drain();}
        }
        [Test]public void FixtureCampaignHintPhysicalRestoreAndCompletionUseOneAcknowledgedResult()
        {
            using var f=new Fixture();f.Owner.Pump(f.Saved.SelectPuzzleAsync(0,false,CancellationToken.None));Assert.That(f.Saved.State.CanMutate,Is.True);f.Hint();Assert.That(f.Saved.PuzzleView!.TimerStarted,Is.False);Assert.That(f.Use.Session!.HintCount,Is.EqualTo(1));
            f.Reload();Assert.That(f.Saved.HasDraftForPuzzle(0),Is.True);f.Owner.Pump(f.Saved.SelectPuzzleAsync(0,true,CancellationToken.None));Assert.That(f.Use.Session!.HintCount,Is.EqualTo(1));f.SolveCurrent();
            f.Repo.Hold=true;var completion=f.Saved.TickAutosaveAsync(CancellationToken.None);f.Owner.Until(()=>f.Repo.Waiting!=null);Assert.That(f.Saved.State.CanStartRide,Is.False);Assert.That(f.Saved.State.Progress!.SelectedStars,Is.EqualTo(1));f.Repo.Release();f.Owner.Pump(completion);
            Assert.That(f.Saved.State.Progress!.SelectedStars,Is.EqualTo(3));Assert.That(f.Saved.State.CanStartRide,Is.True);long balance=f.Saved.State.Progress.BalancePatience;f.Ride();f.Owner.Pump(f.Saved.TickAutosaveAsync(CancellationToken.None));Assert.That(f.Saved.State.Progress.BalancePatience,Is.EqualTo(balance));
            var loaded=f.Owner.Pump(f.Repo.LoadAsync(CancellationToken.None));Assert.That(loaded.Succeeded,Is.True,loaded.ErrorCode);Assert.That(loaded.Snapshot!.Payload.Levels.Single(l=>l.Identity.Equals(f.Corpus.Puzzles[0].Identity)).StarsHighWater,Is.EqualTo(3));
        }
        [Test]public void FixtureEndlessHintRestartPhysicalResumeAndThreeStarResultUseGenericView()
        {
            using var f=new Fixture();var profile=F<CalibratedEndlessProfile[]>("Profiles")[0].Profile;Assert.That(f.Owner.Pump(f.Saved.StartEndlessAsync(f.Endless,profile,"2",CancellationToken.None)).Succeeded,Is.True);string id=f.Saved.State.CurrentIdentity!.PuzzleId;Assert.That(id,Does.StartWith("E1-"));f.Hint();f.Owner.Pump(f.Saved.RestartAsync(CancellationToken.None));Assert.That(f.Hints.HintCredits(f.Saved.ConfirmedSnapshot!.Payload,f.Saved.State.CurrentIdentity!),Is.EqualTo(2));
            f.Reload();Assert.That(f.Owner.Pump(f.Saved.ResumeEndlessAsync(f.Endless,id,CancellationToken.None)).Succeeded,Is.True);Assert.That(f.Saved.CurrentModeName,Is.EqualTo("ENDLESS"));f.SolveCurrent();f.Owner.Pump(f.Saved.TickAutosaveAsync(CancellationToken.None));Assert.That(f.Saved.State.Progress!.SelectedStars,Is.EqualTo(3));Assert.That(f.Use.GetConfirmedEndlessResult(id)!.Stars,Is.EqualTo(3));f.Ride();
            var loaded=f.Owner.Pump(f.Repo.LoadAsync(CancellationToken.None));Assert.That(loaded.Succeeded,Is.True,loaded.ErrorCode);Assert.That(loaded.Snapshot!.Payload.Endless.OpenEndless.OfType<OpenEndlessCompletion>().Single().Result!.Stars,Is.EqualTo(3));
        }
        [Test]public void ProductionAndLegacyEntryPointsRejectFixtureReviewAndCandidateProfileMixing()
        {
            using var f=new Fixture();var production=new SaveProfileIdentity("stp-production","player",CatalogUse.Production);var review=new SaveProfileIdentity(SaveProfileIdentity.Public240ReviewNamespace,"r",CatalogUse.IsolatedReview);var candidate=new SaveProfileIdentity(SaveProfileIdentity.MobileCandidateNamespace,"player",CatalogUse.InternalPreview);
            string root=Environment.GetEnvironmentVariable("STP_SOURCE_ROOT")??Directory.GetCurrentDirectory();byte[] candidateBytes=File.ReadAllBytes(Path.Combine(root,"Assets/StammstreckenPuzzle/UI/MobileCandidate.public.json"));var parsed=StrictJsonParser.Parse(candidateBytes,new JsonParserLimits(1048576,16,4096,4096,32));Assert.That(parsed.Succeeded,Is.True);var actualDraft=new MobileCandidateData(candidateBytes,parsed.Value!.Get("catalogHashSha256")!.StringValue);
            Assert.Throws<ArgumentException>(()=>SavedPublic240ReviewSession.ForLockedRuntime(f.Use,new AutosaveCoordinator(f.Use),actualDraft,f.Clock,f.Owner,production));
            Assert.Throws<ArgumentException>(()=>SavedPublic240ReviewSession.ForLockedRuntime(f.Use,new AutosaveCoordinator(f.Use),f.Corpus,f.Clock,f.Owner,production));
            Assert.Throws<ArgumentException>(()=>SavedPublic240ReviewSession.ForLockedRuntime(f.Use,new AutosaveCoordinator(f.Use),f.Corpus,f.Clock,f.Owner,review));Assert.Throws<ArgumentException>(()=>SavedPublic240ReviewSession.ForLockedRuntime(f.Use,new AutosaveCoordinator(f.Use),f.Corpus,f.Clock,f.Owner,candidate));
            Assert.Throws<ArgumentException>(()=>new SavedPublic240ReviewSession(f.Use,new AutosaveCoordinator(f.Use),f.Corpus,f.Clock,f.Owner));Assert.Throws<ArgumentException>(()=>SavedPublic240ReviewSession.ForMobileCandidate(f.Use,new AutosaveCoordinator(f.Use),f.Corpus,f.Clock,f.Owner,f.Profile));
            Assert.Throws<ArgumentException>(()=>new SaveProfileIdentity(SaveProfileIdentity.Public240ReviewNamespace,"player",CatalogUse.Production));Assert.Throws<ArgumentException>(()=>new SaveProfileIdentity(SaveProfileIdentity.MobileCandidateNamespace,"player",CatalogUse.Production));
        }
        [Test]public void WrongExpectedRuntimeProfileCannotAcquireLoadAuthorityOrMutate()
        {
            using var f=new Fixture();using var wrong=SavedPublic240ReviewSession.ForLockedRuntime(f.Use,new AutosaveCoordinator(f.Use),f.Corpus,f.Clock,f.Owner,new SaveProfileIdentity(f.Profile.Namespace,"other",CatalogUse.ContractTest));f.Owner.Pump(wrong.InitializeAsync(CancellationToken.None));
            Assert.That(wrong.State.Status,Is.EqualTo(SavedReviewStatus.Unsupported));Assert.That(wrong.State.ErrorCode,Is.EqualTo("SAVE_REVIEW_PROFILE_BINDING_MISMATCH"));Assert.That(wrong.ConfirmedSnapshot,Is.Null);Assert.That(wrong.CanRunProfileAction,Is.False);Assert.That(wrong.CanEnterPuzzle(0),Is.False);
        }
        [Test]public void FixtureModalAllowsProfileActionButUnknownWriteClosesAllAuthority()
        {
            using var f=new Fixture();f.Owner.Pump(f.Saved.SelectPuzzleAsync(0,false,CancellationToken.None));f.Owner.Pump(f.Saved.SetSuspendedAsync(true,CancellationToken.None));Assert.That(f.Saved.State.CanNavigate,Is.False);Assert.That(f.Saved.CanRunProfileAction,Is.True);f.Repo.Unknown=true;
            var result=f.Owner.Pump(f.Saved.RunProfileActionAsync((use,ct)=>use.SaveSettingsAsync(LocalSettings.Create("de-DE",Array.Empty<LocalAudioPreference>(),true,null),ct),CancellationToken.None));Assert.That(result.Succeeded,Is.False);Assert.That(f.Saved.CanRunProfileAction,Is.False);Assert.That(f.Saved.State.CanMutate,Is.False);Assert.That(f.Saved.ConfirmedSnapshot,Is.Null);
        }
    }
}
