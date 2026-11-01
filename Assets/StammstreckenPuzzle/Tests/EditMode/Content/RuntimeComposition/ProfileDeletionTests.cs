#nullable enable
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
using STP.Application;
using STP.Infrastructure.Content;
using STP.Infrastructure.Persistence;

namespace STP.Tests.RuntimeComposition.EditMode
{
    public sealed class ProfileDeletionTests
    {
        private const string Main="profile-lifecycle-v1.json",Stage="profile-lifecycle-v1.pending.json",Previous="profile-lifecycle-v1.previous.json";
        private sealed class ProcessStop:Exception{}
        private sealed class Clock:ILocalSessionClock{public long MonotonicMilliseconds=>0;public string UtcNow=>"2026-10-21T00:00:00Z";}
        private sealed class Fixture:IDisposable
        {
            internal readonly string Root;internal readonly ICanonicalJson Json;internal readonly SaveProfileIdentity Profile;internal readonly CatalogSetIdentity Catalogs;
            internal readonly IPublicPuzzleCatalog Puzzles;internal readonly ILockedCampaignCatalog Campaign;internal readonly ILockedCompletionCatalog Completion;
            internal readonly LocalProgressPolicy Policy;internal readonly LocalProfileState Empty;internal readonly LocalSaveCodec Codec;internal readonly SaveFileSystem Files;
            internal readonly AtomicSaveRepository Repo;internal string? StopAt;internal Action<string>? ExtraFault;
            internal Fixture()
            {
                var type=Assembly.Load("STP.Tests.Application.EditMode").GetType("STP.Tests.Application.EditMode.LocalSaveContractFixtures",true)!;
                var ports=(object[])type.GetMethod("CreateContractPorts")!.Invoke(null,null)!;
                Json=(ICanonicalJson)ports[0];Profile=(SaveProfileIdentity)ports[1];Catalogs=(CatalogSetIdentity)ports[2];Puzzles=(IPublicPuzzleCatalog)ports[3];Campaign=(ILockedCampaignCatalog)ports[4];Completion=(ILockedCompletionCatalog)ports[5];
                Policy=new LocalProgressPolicy(Puzzles,Campaign,Completion,Json,new SaveLedgerCodec(Json));Empty=Policy.NewProfile(Profile,Catalogs,"2026-10-21T00:00:00Z");
                Codec=new LocalSaveCodec(Json,Profile,Catalogs,Puzzles,Campaign,Completion);
                Root=Path.GetFullPath(Path.Combine(Environment.GetEnvironmentVariable("STP_SOURCE_ROOT")??Directory.GetCurrentDirectory(),".utmp","e"+Guid.NewGuid().ToString("N").Substring(0,6)));
                SaveFileSystem.ProvisionPrivateRoot(Root);Files=new SaveFileSystem(Root,Profile,Catalogs.AggregateCatalog,p=>{ExtraFault?.Invoke(p);if(p==StopAt)throw new ProcessStop();});Repo=NewRepo();
            }
            internal AtomicSaveRepository NewRepo()=>new AtomicSaveRepository(Files,Codec,new Clock(),"fixture","delete-contract");
            internal static T Wait<T>(Task<T> task)=>task.GetAwaiter().GetResult();
            internal SaveSnapshot<LocalProfileState> Create()
            {var r=Wait(Repo.CreateProfileAsync(Empty,CancellationToken.None));Assert.That(r.Succeeded,Is.True,r.ErrorCode);return r.Snapshot!;}
            internal ProfileDeletionIntent Intent(AtomicSaveRepository? repo=null)
            {repo??=Repo;var o=Wait(repo.InspectDeletionAsync(CancellationToken.None));Assert.That(o.State,Is.Not.EqualTo(ProfileDeletionState.Failed),o.ErrorCode);return repo.CreateDeletionIntent(o);}
            internal string PathOf(string name)=>Path.Combine(Files.ProfileDirectory,name);
            internal void SeedAllRoles()
            {
                var first=Create();var r=Wait(Repo.CommitAsync(new SaveSnapshot<LocalProfileState>(Profile,Catalogs.AggregateCatalog,2,Empty),1,CancellationToken.None));Assert.That(r.Succeeded,Is.True,r.ErrorCode);
                File.Copy(PathOf("save.json"),PathOf("save.pending.json"),true);
                foreach(string d in new[]{"recovery","migration-source","diagnostics"}){Directory.CreateDirectory(PathOf(d));File.WriteAllBytes(PathOf(d+"/original.tmp"),new byte[]{1,2,3});}
                File.WriteAllText(PathOf("save.migration-source.json"),"old source");
                File.WriteAllText(PathOf("diagnostics/diagnostic-ring.jsonl"),"redacted fixture");File.WriteAllText(PathOf("diagnostics/diagnostic-ring.pending.jsonl"),"redacted pending");
            }
            public void Dispose()
            {
                string bound=Path.GetFullPath(Path.Combine(Environment.GetEnvironmentVariable("STP_SOURCE_ROOT")??Directory.GetCurrentDirectory(),".utmp"))+Path.DirectorySeparatorChar;
                Assert.That(Root.StartsWith(bound,StringComparison.OrdinalIgnoreCase),Is.True);Assert.That(Path.GetFileName(Root).Length,Is.EqualTo(7));if(Directory.Exists(Root))Directory.Delete(Root,true);
            }
        }
        private static ProfileDeletionObservation Observe(AtomicSaveRepository r)=>Fixture.Wait(r.InspectDeletionAsync(CancellationToken.None));
        private static ProfileDeletionResult Delete(AtomicSaveRepository r,ProfileDeletionIntent i)=>Fixture.Wait(r.ConfirmDeleteAsync(i,CancellationToken.None));

        [Test]
        public void HistoricalLoadIsReadOnlyAndCancelledConfirmationHasNoDiskEffect()
        {
            using var f=new Fixture();Assert.That(Fixture.Wait(f.Repo.LoadAsync(CancellationToken.None)).Status,Is.EqualTo(SaveLoadStatus.NeedsInitialization));Assert.That(Directory.Exists(f.Files.ProfileDirectory),Is.False);
            var observation=Observe(f.Repo);var intent=f.Repo.CreateDeletionIntent(observation);Assert.That(Directory.Exists(f.Files.ProfileDirectory),Is.False);
            using var cts=new CancellationTokenSource();cts.Cancel();var cancelled=Fixture.Wait(f.Repo.ConfirmDeleteAsync(intent,cts.Token));
            Assert.That(cancelled.Status,Is.EqualTo(ProfileDeletionStatus.Cancelled));Assert.That(Directory.Exists(f.Files.ProfileDirectory),Is.False);
            Assert.That(Fixture.Wait(f.Repo.LoadAsync(CancellationToken.None)).ErrorCode,Is.EqualTo("SAVE_PROFILE_REPOSITORY_RETIRED"));
        }
        [Test]
        public void ExactProfileDeletionRemovesAllRolesArchivesAndRingButNeverAnotherProfileOrHumanData()
        {
            using var f=new Fixture();f.SeedAllRoles();byte[] before=File.ReadAllBytes(f.PathOf("save.json"));
            string sibling=Path.Combine(Path.GetDirectoryName(f.Files.ProfileDirectory)!,"other-player");Directory.CreateDirectory(sibling);string other=Path.Combine(sibling,"save.json");File.WriteAllBytes(other,before);
            string human=Path.Combine(f.Root,"human-calibration","original.json");Directory.CreateDirectory(Path.GetDirectoryName(human)!);File.WriteAllBytes(human,before);
            var intent=f.Intent();Assert.That(Delete(f.Repo,intent).Succeeded,Is.True);Assert.That(Directory.GetFiles(f.Files.ProfileDirectory,"*",SearchOption.AllDirectories).Select(Path.GetFileName),Is.EquivalentTo(new[]{Main}));
            var record=new ProfileLifecycleCodec(f.Json).Decode(File.ReadAllBytes(f.PathOf(Main)));Assert.That(record.DeletePending,Is.False);Assert.That(record.WriterEpoch,Is.EqualTo(intent.TargetEpoch));Assert.That(File.ReadAllBytes(other),Is.EqualTo(before));Assert.That(File.ReadAllBytes(human),Is.EqualTo(before));
            var fresh=f.NewRepo();Assert.That(Fixture.Wait(fresh.CreateProfileAsync(f.Empty,CancellationToken.None)).ErrorCode,Is.EqualTo("SAVE_PROFILE_EPOCH_LOAD_REQUIRED"));
            Assert.That(Fixture.Wait(fresh.LoadAsync(CancellationToken.None)).Status,Is.EqualTo(SaveLoadStatus.NeedsInitialization));Assert.That(Fixture.Wait(fresh.CreateProfileAsync(f.Empty,CancellationToken.None)).Succeeded,Is.True);
        }
        [Test]
        public void StaleBoundColdAndFailedCreateRepositoriesCannotAdoptNewEpochAndLostAckNeverErasesNewProfile()
        {
            using var f=new Fixture();var snapshot=f.Create();var bound=f.NewRepo();var cold=f.NewRepo();var failedCreate=f.NewRepo();
            var oldObservation=Fixture.Wait(bound.LoadAsync(CancellationToken.None)).Observation!;
            Assert.That(Fixture.Wait(failedCreate.CreateProfileAsync(f.Empty,CancellationToken.None)).Status,Is.EqualTo(SaveCommitStatus.Conflict));
            Assert.That(Fixture.Wait(failedCreate.CommitAsync(new SaveSnapshot<LocalProfileState>(f.Profile,f.Catalogs.AggregateCatalog,2,f.Empty),1,CancellationToken.None)).ErrorCode,Is.EqualTo("SAVE_PROFILE_EPOCH_LOAD_REQUIRED"));
            var intent=f.Intent();f.StopAt="delete.before-ack";Assert.Throws<ProcessStop>(()=>Delete(f.Repo,intent));f.StopAt=null;
            var fresh=f.NewRepo();Assert.That(Fixture.Wait(fresh.LoadAsync(CancellationToken.None)).Status,Is.EqualTo(SaveLoadStatus.NeedsInitialization));Assert.That(Fixture.Wait(fresh.CreateProfileAsync(f.Empty,CancellationToken.None)).Succeeded,Is.True);byte[] newBytes=File.ReadAllBytes(f.PathOf("save.json"));
            Assert.That(Delete(f.Repo,intent).Succeeded,Is.True);Assert.That(File.ReadAllBytes(f.PathOf("save.json")),Is.EqualTo(newBytes));
            Assert.That(Fixture.Wait(bound.LoadAsync(CancellationToken.None)).ErrorCode,Is.EqualTo("SAVE_PROFILE_EPOCH_CONFLICT"));
            var offered=new SaveSnapshot<LocalProfileState>(f.Profile,f.Catalogs.AggregateCatalog,2,snapshot.Payload);
            Assert.That(Fixture.Wait(bound.CommitAsync(offered,1,CancellationToken.None)).ErrorCode,Is.EqualTo("SAVE_PROFILE_EPOCH_CONFLICT"));
            Assert.That(Fixture.Wait(cold.CommitAsync(offered,1,CancellationToken.None)).ErrorCode,Is.EqualTo("SAVE_PROFILE_EPOCH_LOAD_REQUIRED"));
            Assert.That(Fixture.Wait(cold.RecoverAsync(new SaveRecoveryRequest(oldObservation,SaveCandidateKind.Main),CancellationToken.None)).ErrorCode,Is.EqualTo("SAVE_PROFILE_EPOCH_LOAD_REQUIRED"));
            Assert.That(Fixture.Wait(cold.ConfirmedResetCorruptProfileAsync(oldObservation,f.Empty,CancellationToken.None)).ErrorCode,Is.EqualTo("SAVE_PROFILE_EPOCH_LOAD_REQUIRED"));
            Assert.That(Fixture.Wait(f.Repo.CreateProfileAsync(f.Empty,CancellationToken.None)).ErrorCode,Is.EqualTo("SAVE_PROFILE_REPOSITORY_RETIRED"));Assert.That(File.ReadAllBytes(f.PathOf("save.json")),Is.EqualTo(newBytes));
        }
        [Test]
        public void EveryFreshExplicitDeletionHasOneNewEpochAndOldIntentConflictsWithLaterDeletion()
        {
            using var f=new Fixture();f.Create();var one=f.Intent();Assert.That(Delete(f.Repo,one).Succeeded,Is.True);
            var second=f.NewRepo();var two=second.CreateDeletionIntent(Observe(second));Assert.That(two.TargetEpoch,Is.Not.EqualTo(one.TargetEpoch));Assert.That(Delete(second,two).Succeeded,Is.True);
            Assert.That(Delete(f.Repo,one).Status,Is.EqualTo(ProfileDeletionStatus.Conflict));Assert.That(new ProfileLifecycleCodec(f.Json).Decode(File.ReadAllBytes(f.PathOf(Main))).WriterEpoch,Is.EqualTo(two.TargetEpoch));
        }
        [Test]
        public void ReadyConfirmationAllowsOwnAlreadyStartedSaveAndRingDrainUnderTheSameEpoch()
        {
            using var f=new Fixture();f.Create();var intent=f.Intent();
            Assert.That(Fixture.Wait(f.Repo.CommitAsync(new SaveSnapshot<LocalProfileState>(f.Profile,f.Catalogs.AggregateCatalog,2,f.Empty),1,CancellationToken.None)).Succeeded,Is.True);
            Directory.CreateDirectory(f.PathOf("diagnostics"));File.WriteAllText(f.PathOf("diagnostics/diagnostic-ring.jsonl"),"last redacted buffered fixture before drain");
            Assert.That(Delete(f.Repo,intent).Succeeded,Is.True);Assert.That(File.Exists(f.PathOf("save.json")),Is.False);Assert.That(Directory.Exists(f.PathOf("diagnostics")),Is.False);
        }
        [TestCase("main")][TestCase("stage")][TestCase("source")]
        public void InvalidLifecycleRequiresFreshExplicitByteBoundConfirmation(string change)
        {
            using var f=new Fixture();f.Create();File.WriteAllText(f.PathOf(Main),"{");var observed=Observe(f.Repo);Assert.That(observed.State,Is.EqualTo(ProfileDeletionState.Invalid));Assert.That(observed.WriterEpoch,Is.Null);var intent=f.Repo.CreateDeletionIntent(observed);
            if(change=="main")File.WriteAllText(f.PathOf(Main),"[0]");if(change=="stage")File.WriteAllText(f.PathOf(Stage),"bad stage");if(change=="source")File.AppendAllText(f.PathOf("save.json")," ");
            Assert.That(Delete(f.Repo,intent).Status,Is.EqualTo(ProfileDeletionStatus.Conflict));Assert.That(File.Exists(f.PathOf("save.json")),Is.True);
            var fresh=f.NewRepo();Assert.That(Fixture.Wait(fresh.LoadAsync(CancellationToken.None)).ErrorCode,Is.EqualTo("SAVE_PROFILE_LIFECYCLE_INVALID"));
            Assert.That(Delete(fresh,fresh.CreateDeletionIntent(Observe(fresh))).Succeeded,Is.True);Assert.That(File.Exists(f.PathOf("save.json")),Is.False);
        }
        [TestCase("lifecycle.pending.open.before")][TestCase("lifecycle.pending.open.after")][TestCase("lifecycle.pending.partial")]
        [TestCase("lifecycle.pending.written")][TestCase("lifecycle.pending.flush.before")][TestCase("lifecycle.pending.flush.after")]
        [TestCase("lifecycle.pending.closed")][TestCase("lifecycle.pending.stage-verified")][TestCase("lifecycle.pending.replace.before")]
        [TestCase("lifecycle.pending.replace.after")][TestCase("lifecycle.pending.main-verified")][TestCase("delete.pending-verified")]
        [TestCase("delete.plan-verified")][TestCase("delete.file.before")][TestCase("delete.file.after")]
        [TestCase("delete.directory.before")][TestCase("delete.directory.after")][TestCase("delete.data.verified")]
        [TestCase("lifecycle.complete.open.before")][TestCase("lifecycle.complete.open.after")][TestCase("lifecycle.complete.partial")]
        [TestCase("lifecycle.complete.written")][TestCase("lifecycle.complete.flush.before")][TestCase("lifecycle.complete.flush.after")]
        [TestCase("lifecycle.complete.closed")][TestCase("lifecycle.complete.stage-verified")][TestCase("lifecycle.complete.replace.before")]
        [TestCase("lifecycle.complete.replace.after")][TestCase("lifecycle.complete.main-verified")][TestCase("lifecycle.cleanup.before")]
        [TestCase("lifecycle.cleanup.after")][TestCase("delete.before-ack")]
        public void SimulatedProcessInterruptionNeverReopensUsageBeforeExplicitlyConfirmedCompletion(string point)
        {
            using var f=new Fixture();f.SeedAllRoles();var intent=f.Intent();f.StopAt=point;Assert.Throws<ProcessStop>(()=>Delete(f.Repo,intent),point);f.StopAt=null;
            var retry=f.NewRepo();var observation=Observe(retry);Assert.That(observation.State,Is.Not.EqualTo(ProfileDeletionState.Failed),observation.ErrorCode);
            var load=Fixture.Wait(retry.LoadAsync(CancellationToken.None));if(observation.State!=ProfileDeletionState.Ready)Assert.That(load.Succeeded,Is.False,point);
            ProfileDeletionResult result;
            if(observation.State==ProfileDeletionState.Pending)result=Fixture.Wait(retry.ResumePendingDeletionAsync(observation,CancellationToken.None));
            else if(observation.State==ProfileDeletionState.Invalid)result=Delete(retry,retry.CreateDeletionIntent(observation)); // Actual renewed explicit confirmation, never implicit repair.
            else result=Delete(f.Repo,intent);
            Assert.That(result.Succeeded,Is.True,point+":"+result.ErrorCode);
            Assert.That(Directory.GetFiles(f.Files.ProfileDirectory,"*",SearchOption.AllDirectories).Select(Path.GetFileName),Is.EquivalentTo(new[]{Main}),point);
            var newRepo=f.NewRepo();Assert.That(Fixture.Wait(newRepo.LoadAsync(CancellationToken.None)).Status,Is.EqualTo(SaveLoadStatus.NeedsInitialization),point);
        }
        [Test]
        public void IntentDoesNotAuthorizeDifferentProfileAndPendingResumeRequiresExactFreshObservation()
        {
            using var f=new Fixture();f.Create();var intent=f.Intent();f.StopAt="delete.pending-verified";Assert.Throws<ProcessStop>(()=>Delete(f.Repo,intent));f.StopAt=null;
            var r=f.NewRepo();var observation=Observe(r);Assert.That(observation.State,Is.EqualTo(ProfileDeletionState.Pending));
            File.AppendAllText(f.PathOf("save.json")," ");Assert.That(Fixture.Wait(r.ResumePendingDeletionAsync(observation,CancellationToken.None)).Status,Is.EqualTo(ProfileDeletionStatus.Conflict));
            Assert.That(Fixture.Wait(r.ResumePendingDeletionAsync(Observe(r),CancellationToken.None)).Succeeded,Is.True);
            var otherProfile=new SaveProfileIdentity("other-profile","other",CatalogUse.ContractTest);var otherCodec=new LocalSaveCodec(f.Json,otherProfile,f.Catalogs,f.Puzzles,f.Campaign,f.Completion);var other=new AtomicSaveRepository(new SaveFileSystem(f.Root,otherProfile,f.Catalogs.AggregateCatalog),otherCodec,new Clock(),"fixture","fixture");
            Assert.That(Delete(other,intent).Status,Is.EqualTo(ProfileDeletionStatus.Conflict));
        }
        [TestCase("lifecycle.pending.replace.after")][TestCase("lifecycle.cleanup.before")][TestCase("lifecycle.cleanup.after")]
        public void LaterDeletionWithPreviousEpochBackupCanResumeWithoutHistoricalAuthorityResurrection(string point)
        {
            using var f=new Fixture();f.Create();var first=f.Intent();Assert.That(Delete(f.Repo,first).Succeeded,Is.True);var second=f.NewRepo();Fixture.Wait(second.LoadAsync(CancellationToken.None));Assert.That(Fixture.Wait(second.CreateProfileAsync(f.Empty,CancellationToken.None)).Succeeded,Is.True);
            var next=second.CreateDeletionIntent(Observe(second));f.StopAt=point;Assert.Throws<ProcessStop>(()=>Delete(second,next));f.StopAt=null;
            var retry=f.NewRepo();var observation=Observe(retry);Assert.That(observation.State,Is.EqualTo(ProfileDeletionState.Pending));Assert.That(observation.WriterEpoch,Is.EqualTo(next.TargetEpoch));
            Assert.That(Fixture.Wait(retry.ResumePendingDeletionAsync(observation,CancellationToken.None)).Succeeded,Is.True);Assert.That(Delete(f.Repo,first).Status,Is.EqualTo(ProfileDeletionStatus.Conflict));
        }
        [Test]
        public void CancelAndIoFailureAfterPendingStayClosedAndCanOnlyFinishDeletion()
        {
            using var f=new Fixture();f.Create();var intent=f.Intent();using var cts=new CancellationTokenSource();f.ExtraFault=p=>{if(p=="delete.pending-verified")cts.Cancel();};
            var result=Fixture.Wait(f.Repo.ConfirmDeleteAsync(intent,cts.Token));Assert.That(result.Status,Is.EqualTo(ProfileDeletionStatus.Pending));Assert.That(File.Exists(f.PathOf("save.json")),Is.True);f.ExtraFault=null;
            var retry=f.NewRepo();Assert.That(Fixture.Wait(retry.LoadAsync(CancellationToken.None)).ErrorCode,Is.EqualTo("SAVE_PROFILE_DELETE_PENDING"));
            f.ExtraFault=p=>{if(p=="delete.file.before")throw new IOException("SAVE_TEST_DELETE_IO");};result=Fixture.Wait(retry.ResumePendingDeletionAsync(Observe(retry),CancellationToken.None));Assert.That(result.Status,Is.EqualTo(ProfileDeletionStatus.Pending));Assert.That(result.ErrorCode,Is.EqualTo("SAVE_TEST_DELETE_IO"));f.ExtraFault=null;
            Assert.That(Fixture.Wait(retry.ResumePendingDeletionAsync(Observe(retry),CancellationToken.None)).Succeeded,Is.True);Assert.That(Fixture.Wait(retry.LoadAsync(CancellationToken.None)).ErrorCode,Is.EqualTo("SAVE_PROFILE_REPOSITORY_RETIRED"));
        }
        [Test]
        public void ActualJunctionInsideProfileBlocksCompleteDeletionBeforeMarkerOrAnyOutsideMutation()
        {
            using var f=new Fixture();f.Create();var intent=f.Intent();string outside=Path.Combine(f.Root,"outside"),link=f.PathOf("linked-archive");Directory.CreateDirectory(outside);string sentinel=Path.Combine(outside,"original.json");File.WriteAllText(sentinel,"exact outside original");bool windows=Path.DirectorySeparatorChar=='\\';
            var start=new ProcessStartInfo{FileName=windows?"cmd.exe":"/bin/ln",Arguments=windows?"/d /c mklink /J \""+link+"\" \""+outside+"\"":"-s \""+outside+"\" \""+link+"\"",UseShellExecute=false,CreateNoWindow=true,WindowStyle=ProcessWindowStyle.Hidden,RedirectStandardOutput=true,RedirectStandardError=true};
            try
            {
                using(var process=Process.Start(start)!){string output=process.StandardOutput.ReadToEnd()+process.StandardError.ReadToEnd();Assert.That(process.WaitForExit(10000),Is.True);Assert.That(process.ExitCode,Is.Zero,output);}
                Assert.That((File.GetAttributes(link)&FileAttributes.ReparsePoint)!=0,Is.True);Assert.That(Observe(f.Repo).State,Is.EqualTo(ProfileDeletionState.Failed));
                var result=Delete(f.Repo,intent);Assert.That(result.Status,Is.EqualTo(ProfileDeletionStatus.Failed));Assert.That(result.ErrorCode,Is.EqualTo("SAVE_REPARSE_PATH_REJECTED"));Assert.That(File.Exists(f.PathOf(Main)),Is.False);Assert.That(File.ReadAllText(sentinel),Is.EqualTo("exact outside original"));Assert.That(File.Exists(f.PathOf("save.json")),Is.True);
            }
            finally{if(Directory.Exists(link))Directory.Delete(link,false);}
        }
        [Test]
        public void PlatformCaseAliasCannotCauseDeletionOfItsOwnLifecycleAnchor()
        {
            using var f=new Fixture();f.Create();File.WriteAllBytes(f.PathOf(Main.ToUpperInvariant()),new ProfileLifecycleCodec(f.Json).Encode(Guid.NewGuid().ToString("N"),false));
            var intent=f.Intent();Assert.That(Delete(f.Repo,intent).Succeeded,Is.True);Assert.That(File.Exists(f.PathOf(Main)),Is.True);Assert.That(new ProfileLifecycleCodec(f.Json).Decode(File.ReadAllBytes(f.PathOf(Main))).WriterEpoch,Is.EqualTo(intent.TargetEpoch));
        }
        [TestCase("{}")][TestCase("{\"profileLifecycleVersion\":2,\"writerEpoch\":\"11111111111111111111111111111111\",\"deletePending\":true}")]
        [TestCase("{\"profileLifecycleVersion\":1,\"writerEpoch\":\"00000000000000000000000000000000\",\"deletePending\":true}")]
        [TestCase("{\"profileLifecycleVersion\":1,\"writerEpoch\":\"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA\",\"deletePending\":true}")]
        [TestCase("{\"profileLifecycleVersion\":1,\"writerEpoch\":\"11111111111111111111111111111111\",\"deletePending\":true,\"extra\":0}")]
        [TestCase("{\"profileLifecycleVersion\":1,\"writerEpoch\":\"11111111111111111111111111111111\",\"deletePending\":true,\"deletePending\":false}")]
        public void ClosedLifecycleCodecRejectsMalformedMissingUnsupportedDuplicateOrExtraFields(string json)
        {var codec=new ProfileLifecycleCodec(new CanonicalJsonAdapter());Assert.Throws<ArgumentException>(()=>codec.Decode(Encoding.UTF8.GetBytes(json)));}
        [Test]
        public void CanonicalLifecycleRoundTripHasExactlyThreeFieldsAndNeverAltersSaveBytesOnInspect()
        {
            var codec=new ProfileLifecycleCodec(new CanonicalJsonAdapter());string epoch=Guid.NewGuid().ToString("N");byte[] bytes=codec.Encode(epoch,true);Assert.That(Encoding.UTF8.GetString(bytes),Is.EqualTo("{\"deletePending\":true,\"profileLifecycleVersion\":1,\"writerEpoch\":\""+epoch+"\"}"));Assert.That(codec.Decode(bytes).WriterEpoch,Is.EqualTo(epoch));
            using var f=new Fixture();f.Create();byte[] save=File.ReadAllBytes(f.PathOf("save.json"));Observe(f.Repo);Assert.That(File.ReadAllBytes(f.PathOf("save.json")),Is.EqualTo(save));Assert.That(File.Exists(f.PathOf(Main)),Is.False);
        }
    }
}
