#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;
using STP.Bootstrap;
using STP.Infrastructure.Content;

namespace STP.Tests.RuntimeComposition.EditMode
{
    /// <summary>Controlled fixture dates and callback evidence; actual shared writer, codec and physical repository.
    /// No native provider, human result or production approval is represented by these tests.</summary>
    public sealed class MobileCandidateDailyDateTests
    {
        private const string FixturePath="Assets/StammstreckenPuzzle/Tests/PlayMode/Bootstrap/RuntimeFixtures";
        private static readonly string[] Files={"Public.json","Campaign.json","CampaignCompletion.json","EndlessCompletion.json","Calibration.json","Cosmetics.json","Profiles/dienstbeginn.json","Profiles/regelbetrieb.json","Profiles/stoerungslage.json","Profiles/grosslage.json","RailwayAssets.json","CampaignText.json"};
        private sealed class Clock:ILocalSessionClock
        {public long MonotonicMilliseconds=>0;public string UtcNow=>"2026-10-22T00:00:00Z";}
        private sealed class Roll:ILocalDailyRandom
        {internal int Value,Calls;public int Next100(){Calls++;return Value;}}
        private sealed class Owner:ILocalSaveDispatcher
        {
            private readonly int thread=Thread.CurrentThread.ManagedThreadId;
            private readonly ConcurrentQueue<(Action,TaskCompletionSource<bool>)> queue=new ConcurrentQueue<(Action,TaskCompletionSource<bool>)>();
            public Task InvokeAsync(Action action)
            {
                if(Thread.CurrentThread.ManagedThreadId==thread){action();return Task.CompletedTask;}
                var completion=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                queue.Enqueue((action,completion));return completion.Task;
            }
            internal void Pump(Task task)
            {
                var timer=Stopwatch.StartNew();
                while(!task.IsCompleted)
                {
                    while(queue.TryDequeue(out var item))try{item.Item1();item.Item2.TrySetResult(true);}catch(Exception e){item.Item2.TrySetException(e);}
                    if(timer.ElapsedMilliseconds>30000)Assert.Fail("Daily fixture owner timeout");Thread.Sleep(1);
                }
                task.GetAwaiter().GetResult();
            }
            internal T Pump<T>(Task<T> task){Pump((Task)task);return task.GetAwaiter().GetResult();}
        }
        private sealed class Fixture:IDisposable
        {
            internal readonly Owner Owner=new Owner();internal readonly ProductionGameComposition Composition;
            internal readonly SavedPublic240ReviewSession Saved;
            private readonly string root;
            internal Fixture()
            {
                string sourceRoot=Path.GetFullPath(Environment.GetEnvironmentVariable("STP_SOURCE_ROOT")??Directory.GetCurrentDirectory());
                string directory=Path.Combine(sourceRoot,FixturePath);var bytes=Files.ToDictionary(p=>p,p=>File.ReadAllBytes(Path.Combine(directory,p)),StringComparer.Ordinal);
                byte[] binding=File.ReadAllBytes(Path.Combine(directory,"Runtime.bindings.json"));
                var parsed=StrictJsonParser.Parse(binding,new JsonParserLimits(65536,64,65536,1048576,256));Assert.That(parsed.Succeeded,Is.True);
                var value=parsed.Value!;Assert.That(value.Get("use")!.StringValue,Is.EqualTo("ContractTest"));Assert.That(value.Get("status")!.StringValue,Is.EqualTo("FIXTURE_ONLY"));
                var sources=RuntimeGameSources.ReadContractTest(bytes,binding,LocalSaveHash.Bytes(binding),value.Get("releaseLockRawSha256")!.StringValue,value.Get("releaseLockJcsSha256")!.StringValue,value.Get("uiSourceLockRawSha256")!.StringValue);
                root=Path.Combine(sourceRoot,".utmp","d"+Guid.NewGuid().ToString("N").Substring(0,6));var clock=new Clock();
                Composition=Owner.Pump(ProductionGameComposition.CreateAsync(sources,root,clock,Owner,"fixture","daily-date-fixture",CancellationToken.None));
                Saved=SavedPublic240ReviewSession.ForLockedRuntime(Composition.Persisted,Composition.Autosave,Composition.Catalog,clock,Owner,Composition.Profile);
                Owner.Pump(Saved.InitializeAsync(CancellationToken.None));Owner.Pump(Saved.CreateProfileAsync(CancellationToken.None));
                Assert.That(Saved.ConfirmedSnapshot!.Payload.Metagame?.Daily??Array.Empty<LocalDailyClaim>(),Is.Empty);
            }
            internal LocalProfileState State=>Saved.ConfirmedSnapshot!.Payload;
            internal SaveCommitResult<LocalProfileState> Run(Func<PersistedPuzzleSession,CancellationToken,Task<SaveCommitResult<LocalProfileState>>> action)
            {
                var result=Owner.Pump(Saved.RunProfileActionAsync(action,CancellationToken.None));Assert.That(result.Succeeded,Is.True,result.ErrorCode);return result;
            }
            internal void Free(string date,Roll roll)=>Run((use,ct)=>use.ClaimFreeDailyAsync(Composition.Metagame,roll,date,ct));
            internal AdRewardReservation Reserve(string date,Roll roll)
            {
                var result=Run((use,ct)=>use.ReserveDailyRewardAsync(Composition.Metagame,roll,date,Guid.NewGuid().ToString("D"),ct));
                return LocalMetagameTransactions.RewardReservation(result,"daily:v1:"+date+":REWARDED");
            }
            internal void Reload()
            {
                Owner.Pump(Saved.ReloadAsync(CancellationToken.None));
                var load=Owner.Pump(Composition.Repository.LoadAsync(CancellationToken.None));Assert.That(load.Succeeded,Is.True,load.ErrorCode);
                Assert.That(load.Snapshot!.Generation,Is.EqualTo(Saved.ConfirmedSnapshot!.Generation));
            }
            internal long Balance=>State.LedgerCheckpoint.BalancePatience+State.EconomyJournal.Sum(e=>e.Amount);
            public void Dispose()
            {
                Saved.Dispose();string expected=Path.GetFullPath(Path.Combine(Environment.GetEnvironmentVariable("STP_SOURCE_ROOT")??Directory.GetCurrentDirectory(),".utmp"))+Path.DirectorySeparatorChar;
                Assert.That(Path.GetFullPath(root).StartsWith(expected,StringComparison.OrdinalIgnoreCase),Is.True);
                Assert.That(Path.GetFileName(root).Length,Is.EqualTo(7));if(Directory.Exists(root))Directory.Delete(root,true);
            }
        }
        // Invoke the actual adapter's pure read boundaries without constructing Unity UI/native objects.
        private static string Date(Func<DateTime> clock)=>(string)typeof(MobileCandidateExperience).GetMethod("DailyDate",BindingFlags.NonPublic|BindingFlags.Static,null,new[]{typeof(Func<DateTime>)},null)!.Invoke(null,new object[]{clock})!;
        private static object Read(LocalProfileState? state,string date,bool ads=false)
        {
            Func<DateTime> clock=()=>DateTime.ParseExact(date,"yyyy-MM-dd",CultureInfo.InvariantCulture);
            return typeof(MobileCandidateExperience).GetMethod("ReadDaily",BindingFlags.NonPublic|BindingFlags.Static,null,new[]{typeof(LocalProfileState),typeof(Func<DateTime>),typeof(bool)},null)!.Invoke(null,new object?[]{state,clock,ads})!;
        }
        private static T Fact<T>(object view,string name)=>(T)view.GetType().GetProperty(name)!.GetValue(view)!;
        private static LocalDailyClaim Claim(LocalProfileState state,string date,string slot)=>state.Metagame!.Daily.Single(c=>c.Date==date&&c.Slot==slot);
        private static AdRewardEvidence Evidence(AdRewardReservation reservation)=>new AdRewardEvidence(reservation,reservation.OperationId);

        [TestCase(false,"2026-10-22")][TestCase(true,"2026-10-22")]
        [TestCase(false,"2026-10-20")][TestCase(true,"2026-10-20")]
        public void OtherDatePendingVideoCannotHideTodaysFreeAndLateEvidenceStillGrantsExactlyOnce(bool reconciliationRequired,string currentLocalDate)
        {
            using var f=new Fixture();string old="2026-10-21";var freeRoll=new Roll{Value=0};f.Free(old,freeRoll);
            var oldReservation=f.Reserve(old,new Roll{Value=95});
            if(reconciliationRequired)f.Run((use,ct)=>use.ResolveDailyRewardAsync(f.Composition.Metagame,oldReservation,false,ct));
            f.Reload();var previous=Claim(f.State,old,"REWARDED");Assert.That(previous.Reward!.State,Is.EqualTo(reconciliationRequired?"RECONCILIATION_REQUIRED":"RESERVED"));
            string today=Date(()=>DateTime.ParseExact(currentLocalDate,"yyyy-MM-dd",CultureInfo.InvariantCulture));var view=Read(f.State,today,true);
            Assert.That(Fact<string>(view,"DateLabel"),Is.EqualTo(currentLocalDate));Assert.That(Fact<bool>(view,"FreeAvailable"),Is.True);
            Assert.That(Fact<bool>(view,"VideoAvailable"),Is.False);Assert.That(Fact<bool>(view,"PendingClaim"),Is.False);Assert.That(Fact<int?>(view,"ConfirmedAmount"),Is.Null);
            var roll=new Roll{Value=60};f.Free(today,roll);Assert.That(roll.Calls,Is.EqualTo(1));long balance=f.Balance;
            var duplicate=new Roll{Value=99};f.Free(today,duplicate);Assert.That(duplicate.Calls,Is.Zero);Assert.That(f.Balance,Is.EqualTo(balance));
            Assert.That(Claim(f.State,old,"REWARDED").Reward!.OperationId,Is.EqualTo(oldReservation.OperationId));
            Assert.That(Claim(f.State,old,"REWARDED").Roll,Is.EqualTo(95));Assert.That(Claim(f.State,old,"REWARDED").Terminal,Is.Null);
            view=Read(f.State,today,true);Assert.That(Fact<bool>(view,"FreeAvailable"),Is.False);Assert.That(Fact<bool>(view,"VideoAvailable"),Is.True);Assert.That(Fact<int?>(view,"ConfirmedAmount"),Is.EqualTo(15));
            var wrong=new AdRewardEvidence(new AdRewardReservation(oldReservation.ClaimId,"unrelated-operation",oldReservation.SaveGeneration),"unrelated-operation");
            f.Run((use,ct)=>use.ConfirmDailyRewardAsync(f.Composition.Metagame,wrong,ct));Assert.That(f.Balance,Is.EqualTo(balance));Assert.That(Claim(f.State,old,"REWARDED").Terminal,Is.Null);
            f.Run((use,ct)=>use.ConfirmDailyRewardAsync(f.Composition.Metagame,Evidence(oldReservation),ct));Assert.That(f.Balance,Is.EqualTo(balance+25));
            f.Run((use,ct)=>use.ConfirmDailyRewardAsync(f.Composition.Metagame,Evidence(oldReservation),ct));Assert.That(f.Balance,Is.EqualTo(balance+25));
            f.Reload();Assert.That(Claim(f.State,old,"REWARDED").Terminal!.Amount,Is.EqualTo(25));Assert.That(Claim(f.State,today,"FREE").Terminal!.Amount,Is.EqualTo(15));
            Assert.That(Fact<int?>(Read(f.State,today,true),"ConfirmedAmount"),Is.EqualTo(15));Assert.That(f.State.Metagame!.Daily.Count,Is.EqualTo(3));
        }

        [Test]
        public void PendingTodayVideoDisablesOnlyItsOwnSlotAndAnOlderFreeReservationAlsoCannotSelectTheDate()
        {
            using var f=new Fixture();string old="2026-10-20",today="2026-10-22";
            f.Run((use,ct)=>use.RunExternalProfileActionAsync(async token=>
            {
                var load=await f.Composition.Repository.LoadAsync(token).ConfigureAwait(false);var before=load.Snapshot!;
                var state=f.Composition.Metagame.ReserveDaily(before.Payload,old,false,"free-unused",before.Generation+1,new Roll{Value=90});
                return await f.Composition.Repository.CommitAsync(new SaveSnapshot<LocalProfileState>(before.Profile,before.Catalog,before.Generation+1,state),before.Generation,token).ConfigureAwait(false);
            },ct));
            Assert.That(Claim(f.State,old,"FREE").Terminal,Is.Null);Assert.That(Fact<bool>(Read(f.State,today),"FreeAvailable"),Is.True);
            f.Free(today,new Roll());var pending=f.Reserve(today,new Roll{Value=60});
            var view=Read(f.State,today,true);Assert.That(Fact<bool>(view,"FreeAvailable"),Is.False);Assert.That(Fact<bool>(view,"VideoAvailable"),Is.False);Assert.That(Fact<bool>(view,"PendingClaim"),Is.True);
            f.Run((use,ct)=>use.ResolveDailyRewardAsync(f.Composition.Metagame,pending,true,ct));
            Assert.That(Fact<bool>(Read(f.State,today,true),"VideoAvailable"),Is.True);Assert.That(Fact<bool>(Read(f.State,today,false),"VideoAvailable"),Is.False);
            var retryRoll=new Roll{Value=99};var retry=f.Reserve(today,retryRoll);Assert.That(retryRoll.Calls,Is.Zero);Assert.That(Claim(f.State,today,"REWARDED").Roll,Is.EqualTo(60));
            long balance=f.Balance;f.Run((use,ct)=>use.ConfirmDailyRewardAsync(f.Composition.Metagame,Evidence(pending),ct));Assert.That(f.Balance,Is.EqualTo(balance));
            f.Run((use,ct)=>use.ConfirmDailyRewardAsync(f.Composition.Metagame,Evidence(retry),ct));Assert.That(f.Balance,Is.EqualTo(balance+15));f.Reload();
            Assert.That(Claim(f.State,old,"FREE").Roll,Is.EqualTo(90));Assert.That(Claim(f.State,old,"FREE").Terminal,Is.Null);
        }

        [Test]
        public void TravelRollbackSelectsActualLocalDateWithoutUnlockingPreviouslyClaimedRetainedSlot()
        {
            using var f=new Fixture();f.Free("2026-10-22",new Roll{Value=95});
            string date=Date(()=>new DateTime(2026,10,20,23,59,59));Assert.That(date,Is.EqualTo("2026-10-20"));Assert.That(Fact<bool>(Read(f.State,date),"FreeAvailable"),Is.True);
            f.Free(date,new Roll{Value=0});f.Reload();date=Date(()=>new DateTime(2026,10,22));
            Assert.That(Fact<bool>(Read(f.State,date),"FreeAvailable"),Is.False);var duplicate=new Roll{Value=0};f.Free(date,duplicate);
            Assert.That(duplicate.Calls,Is.Zero);Assert.That(Claim(f.State,date,"FREE").Roll,Is.EqualTo(95));Assert.That(f.State.Metagame!.Daily.Count,Is.EqualTo(2));
        }

        [Test]
        public void LocalDateSourceIsCapturedOnceAndAnUnloadedProfileCannotOfferClaims()
        {
            int reads=0;string date=Date(()=>{reads++;return new DateTime(2026,10,21,23,59,59).AddDays(reads-1);});Assert.That(reads,Is.EqualTo(1));Assert.That(date,Is.EqualTo("2026-10-21"));
            var view=Read(null,date,true);Assert.That(Fact<bool>(view,"FreeAvailable"),Is.False);Assert.That(Fact<bool>(view,"VideoAvailable"),Is.False);Assert.That(Fact<bool>(view,"PendingClaim"),Is.False);Assert.That(Fact<int?>(view,"ConfirmedAmount"),Is.Null);
        }
    }
}
