using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using STP.Application;

namespace STP.Tests.Application.EditMode
{
    public sealed class HumanCalibrationPersistenceTests
    {
        private static readonly ICanonicalJson Json=LocalSaveContractFixtures.Json;
        private static object Codec(IHumanAssignmentCatalog catalog)=>LocalSaveContractFixtures.Construct<object>("STP.Infrastructure.Persistence","STP.Infrastructure.Persistence.HumanCalibrationCodec",Json,catalog);
        private static byte[] Encode(object codec,HumanObservationSnapshot snapshot)=>(byte[])LocalSaveContractFixtures.Call(codec,"Encode",snapshot)!;
        private static HumanObservationSnapshot Decode(object codec,byte[] bytes){try{return(HumanObservationSnapshot)LocalSaveContractFixtures.Call(codec,"Decode",bytes)!;}catch(System.Reflection.TargetInvocationException e)when(e.InnerException!=null){throw e.InnerException;}}
        private static IHumanCalibrationRepository Store(string root,IHumanAssignmentCatalog catalog,Action<string>? fault=null)=>LocalSaveContractFixtures.Construct<IHumanCalibrationRepository>("STP.Infrastructure.Persistence","STP.Infrastructure.Persistence.HumanCalibrationStore",root,catalog,Json,fault!);
        private static string Root()=>Path.Combine(Directory.GetCurrentDirectory(),".utmp","human-"+Guid.NewGuid().ToString("N").Substring(0,10));
        private static HumanCalibrationRun EmptyRun(HumanCalibrationTestFactory f)=>new HumanCalibrationRun(f.Catalog.ManifestBytesSha256,Guid.NewGuid().ToString("N"),HumanRunOrigin.AUTOMATED_TEST,f.Clock.UtcNow,0,Array.Empty<HumanTrialRecord>());
        private static string Record(string root,IHumanAssignmentCatalog catalog,string token)=>Path.Combine(root,"human-calibration-v1",catalog.ManifestBytesSha256,token,"observations.json");
        private sealed class OtherManifest:IHumanAssignmentCatalog
        {
            private readonly IHumanAssignmentCatalog source;internal OtherManifest(IHumanAssignmentCatalog source){this.source=source;}public string ManifestBytesSha256=>new string('e',64);public string StudyId=>source.StudyId;public int Revision=>source.Revision+1;public string BuildId=>source.BuildId;public string UiSourceSha256=>source.UiSourceSha256;public string PublicCatalogHashSha256=>source.PublicCatalogHashSha256;public System.Collections.Generic.IReadOnlyList<HumanAssignment> Assignments=>source.Assignments;
        }
        [Test]
        public void DifferentManifestCannotAutomaticallyStartFreshParticipantOverExistingRun()
        {
            using var f=new HumanCalibrationTestFactory();f.Pump(f.Session.BeginRunAsync(CancellationToken.None));string root=Root();var original=Store(root,f.Catalog);var started=original.StartRunAsync(f.Repository.Snapshot!.Run,null,CancellationToken.None).GetAwaiter().GetResult();var other=new OtherManifest(f.Catalog);var changed=Store(root,other);Assert.That(changed.LoadAsync(CancellationToken.None).GetAwaiter().GetResult().Status,Is.EqualTo(HumanStoreStatus.Unsupported));var run=new HumanCalibrationRun(other.ManifestBytesSha256,Guid.NewGuid().ToString("N"),HumanRunOrigin.AUTOMATED_TEST,f.Clock.UtcNow,0,Array.Empty<HumanTrialRecord>());Assert.That(changed.StartRunAsync(run,null,CancellationToken.None).GetAwaiter().GetResult().Status,Is.EqualTo(HumanStoreStatus.Conflict));Assert.That(changed.StartRunAsync(run,started.Snapshot!.Run.SessionToken,CancellationToken.None).GetAwaiter().GetResult().Status,Is.EqualTo(HumanStoreStatus.Conflict));Assert.That(original.LoadAsync(CancellationToken.None).GetAwaiter().GetResult().Snapshot!.Run.SessionToken,Is.EqualTo(started.Snapshot.Run.SessionToken));
        }
        [Test]
        public void PhysicalNextParticipantRequiresExactCompletedPriorTokenAndKeepsOriginalBytes()
        {
            using var f=new HumanCalibrationTestFactory();f.Pump(f.Session.BeginRunAsync(CancellationToken.None));string root=Root();var store=Store(root,f.Catalog);var first=store.StartRunAsync(f.Repository.Snapshot!.Run,null,CancellationToken.None).GetAwaiter().GetResult();var proposed=EmptyRun(f);Assert.That(store.StartRunAsync(proposed,first.Snapshot!.Run.SessionToken,CancellationToken.None).GetAwaiter().GetResult().Status,Is.EqualTo(HumanStoreStatus.Conflict));f.CompleteRunByExplicitAborts();Assert.That(store.CommitAsync(new HumanObservationSnapshot(2,f.Repository.Snapshot!.Run),1,CancellationToken.None).GetAwaiter().GetResult().Succeeded,Is.True);string originalPath=Record(root,f.Catalog,first.Snapshot.Run.SessionToken);byte[] original=File.ReadAllBytes(originalPath);
            Assert.That(store.StartRunAsync(proposed,null,CancellationToken.None).GetAwaiter().GetResult().Status,Is.EqualTo(HumanStoreStatus.Conflict));Assert.That(store.StartRunAsync(proposed,Guid.NewGuid().ToString("N"),CancellationToken.None).GetAwaiter().GetResult().Status,Is.EqualTo(HumanStoreStatus.Conflict));var next=store.StartRunAsync(proposed,first.Snapshot.Run.SessionToken,CancellationToken.None).GetAwaiter().GetResult();Assert.That(next.Status,Is.EqualTo(HumanStoreStatus.Committed));Assert.That(next.Snapshot!.Generation,Is.EqualTo(1));Assert.That(next.Snapshot.Run.Trials,Is.Empty);Assert.That(next.Snapshot.Run.NextAssignmentIndex,Is.Zero);Assert.That(File.ReadAllBytes(originalPath),Is.EqualTo(original));Assert.That(Decode(Codec(f.Catalog),original).Run.NextAssignmentIndex,Is.EqualTo(2));Assert.That(File.Exists(Path.Combine(Path.GetDirectoryName(originalPath)!,"completed-index.json")),Is.True);var reload=Store(root,f.Catalog).LoadAsync(CancellationToken.None).GetAwaiter().GetResult();Assert.That(reload.Status,Is.EqualTo(HumanStoreStatus.Ready));Assert.That(reload.Snapshot!.Run.SessionToken,Is.EqualTo(proposed.SessionToken));Assert.That(store.StartRunAsync(EmptyRun(f),first.Snapshot.Run.SessionToken,CancellationToken.None).GetAwaiter().GetResult().Status,Is.EqualTo(HumanStoreStatus.Conflict));Assert.That(File.ReadAllBytes(originalPath),Is.EqualTo(original));
        }
        [TestCase("PARTICIPANT_ARCHIVE_PENDING_FLUSHED")]
        [TestCase("PARTICIPANT_ARCHIVE_MAIN_REPLACED")]
        [TestCase("RECORD_PENDING_FLUSHED")]
        [TestCase("RECORD_MAIN_REPLACED")]
        [TestCase("INDEX_PENDING_FLUSHED")]
        [TestCase("INDEX_MAIN_REPLACED")]
        public void ParticipantSwitchFaultsNeverCreateAutomaticTokensOrLoseOriginalCompletedRun(string point)
        {
            using var f=new HumanCalibrationTestFactory();f.Pump(f.Session.BeginRunAsync(CancellationToken.None));string root=Root();string? armed=null;var store=Store(root,f.Catalog,p=>{if(p==armed){armed=null;throw new IOException("controlled participant transition crash "+point);}});var first=store.StartRunAsync(f.Repository.Snapshot!.Run,null,CancellationToken.None).GetAwaiter().GetResult();f.CompleteRunByExplicitAborts();Assert.That(store.CommitAsync(new HumanObservationSnapshot(2,f.Repository.Snapshot!.Run),1,CancellationToken.None).GetAwaiter().GetResult().Succeeded,Is.True);string path=Record(root,f.Catalog,first.Snapshot!.Run.SessionToken);byte[] original=File.ReadAllBytes(path);var proposed=EmptyRun(f);armed=point;var uncertain=store.StartRunAsync(proposed,first.Snapshot.Run.SessionToken,CancellationToken.None).GetAwaiter().GetResult();Assert.That(uncertain.Status,Is.EqualTo(HumanStoreStatus.RecoveryRequired));var reloaded=Store(root,f.Catalog);var observed=reloaded.LoadAsync(CancellationToken.None).GetAwaiter().GetResult();Assert.That(observed.Status,Is.Not.EqualTo(HumanStoreStatus.Missing));if(!observed.Succeeded)observed=reloaded.RecoverAsync(observed.ObservationToken!,CancellationToken.None).GetAwaiter().GetResult();Assert.That(observed.Succeeded,Is.True);Assert.That(new[]{first.Snapshot.Run.SessionToken,proposed.SessionToken},Does.Contain(observed.Snapshot!.Run.SessionToken));Assert.That(File.ReadAllBytes(path),Is.EqualTo(original));Assert.That(reloaded.StartRunAsync(EmptyRun(f),null,CancellationToken.None).GetAwaiter().GetResult().Status,Is.EqualTo(HumanStoreStatus.Conflict));Assert.That(reloaded.LoadAsync(CancellationToken.None).GetAwaiter().GetResult().Snapshot!.Run.SessionToken,Is.EqualTo(observed.Snapshot.Run.SessionToken));
        }
        [TestCase("RECOVERY_INDEX_ANCHOR_COMMITTED")]
        [TestCase("RECOVERY_ROOT_MAIN_DELETED")]
        [TestCase("RECOVERY_ROLES_CLEARED")]
        public void SecondParticipantRecoveryAfterHistoricalAnchorKeepsExactActiveTokenAcrossIndexDeletion(string point)
        {
            using var f=new HumanCalibrationTestFactory();f.Pump(f.Session.BeginRunAsync(CancellationToken.None));string root=Root();string? armed=null;var store=Store(root,f.Catalog,p=>{if(armed==p){armed=null;throw new IOException("second participant recovery crash "+point);}});var first=store.StartRunAsync(f.Repository.Snapshot!.Run,null,CancellationToken.None).GetAwaiter().GetResult();f.CompleteRunByExplicitAborts();Assert.That(store.CommitAsync(new HumanObservationSnapshot(2,f.Repository.Snapshot!.Run),1,CancellationToken.None).GetAwaiter().GetResult().Succeeded,Is.True);string firstPath=Record(root,f.Catalog,first.Snapshot!.Run.SessionToken);File.WriteAllBytes(firstPath+".bak",Encoding.UTF8.GetBytes("first participant damaged role"));var firstPending=store.LoadAsync(CancellationToken.None).GetAwaiter().GetResult();Assert.That(store.RecoverAsync(firstPending.ObservationToken!,CancellationToken.None).GetAwaiter().GetResult().Succeeded,Is.True);Assert.That(File.Exists(Path.Combine(Path.GetDirectoryName(firstPath)!,"recovery-anchor.json")),Is.True);byte[] firstOriginal=File.ReadAllBytes(firstPath);
            f.Pump(f.Session.BeginNextParticipantAsync(CancellationToken.None));var second=store.StartRunAsync(f.Repository.Snapshot!.Run,first.Snapshot.Run.SessionToken,CancellationToken.None).GetAwaiter().GetResult();Assert.That(second.Succeeded,Is.True);bool preserveForeignBackup=point=="RECOVERY_ROOT_MAIN_DELETED";if(!preserveForeignBackup){f.Pump(f.Session.ReserveExposureAsync(CancellationToken.None));Assert.That(store.CommitAsync(new HumanObservationSnapshot(2,f.Repository.Snapshot!.Run),1,CancellationToken.None).GetAwaiter().GetResult().Succeeded,Is.True);}string secondPath=Record(root,f.Catalog,second.Snapshot!.Run.SessionToken);File.WriteAllBytes(secondPath+".bak",Encoding.UTF8.GetBytes("second participant damaged role"));var pending=store.LoadAsync(CancellationToken.None).GetAwaiter().GetResult();armed=point;Assert.That(store.RecoverAsync(pending.ObservationToken!,CancellationToken.None).GetAwaiter().GetResult().Status,Is.EqualTo(HumanStoreStatus.RecoveryRequired));if(preserveForeignBackup){Assert.That(File.Exists(Path.Combine(root,"human-calibration-v1","active-run.json")),Is.False);Assert.That(Encoding.UTF8.GetString(File.ReadAllBytes(Path.Combine(root,"human-calibration-v1","active-run.json.bak"))),Does.Contain(first.Snapshot.Run.SessionToken));}
            var reloaded=Store(root,f.Catalog);var observed=reloaded.LoadAsync(CancellationToken.None).GetAwaiter().GetResult();Assert.That(observed.Status,Is.EqualTo(HumanStoreStatus.RecoveryRequired));var recovered=reloaded.RecoverAsync(observed.ObservationToken!,CancellationToken.None).GetAwaiter().GetResult();Assert.That(recovered.Succeeded,Is.True,"Status="+recovered.Status+"; ErrorCode="+recovered.ErrorCode+"; root="+root);Assert.That(recovered.Snapshot!.Run.SessionToken,Is.EqualTo(second.Snapshot.Run.SessionToken));if(preserveForeignBackup)Assert.That(recovered.Snapshot.Run.Trials,Is.Empty);else Assert.That(recovered.Snapshot.Run.Trials.Single().Exposure,Is.EqualTo(HumanExposureState.EXPOSURE_PENDING));Assert.That(File.ReadAllBytes(firstPath),Is.EqualTo(firstOriginal));Assert.That(File.Exists(Path.Combine(root,"human-calibration-v1","active-recovery-index.json")),Is.False);Assert.That(reloaded.LoadAsync(CancellationToken.None).GetAwaiter().GetResult().Status,Is.EqualTo(HumanStoreStatus.Ready));
        }
        private static byte[] RehashPayload(byte[] source,Func<string,string> mutation)
        {
            string all=Encoding.UTF8.GetString(source);int start=all.IndexOf("\"payload\":",StringComparison.Ordinal)+10;int depth=0,end=start;bool text=false,escape=false;
            for(;end<all.Length;end++){char c=all[end];if(text){if(escape)escape=false;else if(c=='\\')escape=true;else if(c=='\"')text=false;}else if(c=='\"')text=true;else if(c=='{'||c=='[')depth++;else if(c=='}'||c==']'){depth--;if(depth==0){end++;break;}}}
            string payload=all.Substring(start,end-start);var old=Encoding.UTF8.GetBytes(payload);var next=LocalSaveHash.Canonical(Json,mutation(payload));return Encoding.UTF8.GetBytes(all.Replace(payload,Encoding.UTF8.GetString(next)).Replace(LocalSaveHash.Bytes(old),LocalSaveHash.Bytes(next)));
        }
        [Test]
        public void ActualObservationCodecRoundTripsRealDomainHintsAndExplicitAutomatedOrigin()
        {
            using var f=new HumanCalibrationTestFactory();f.OpenFirst();f.Session.Pause(0);var hint=f.Pump(f.Session.PrepareHintAsync(CancellationToken.None));f.Pump(f.Session.CommitHintDisplayAsync(hint!.AcknowledgeDisplayed(),CancellationToken.None));var codec=Codec(f.Catalog);byte[] bytes=Encode(codec,f.Repository.Snapshot!);var reread=Decode(codec,bytes);
            Assert.That(reread.Generation,Is.EqualTo(f.Repository.Snapshot!.Generation));Assert.That(reread.Run.Origin,Is.EqualTo(HumanRunOrigin.AUTOMATED_TEST));Assert.That(reread.Run.Trials.Single().Checkpoint.HintCount,Is.EqualTo(1));Assert.That(reread.Run.Trials.Single().Hints.Single().State,Is.EqualTo(HumanHintState.HINT_DISPLAY_CONFIRMED));Assert.That(Encode(codec,reread),Is.EqualTo(bytes));Assert.That(Encoding.UTF8.GetString(bytes),Does.Not.Contain("ledger").And.Not.Contain("entitlement").And.Not.Contain("payloadVersion"));
        }
        [TestCase("skip-all")]
        [TestCase("future-trial")]
        [TestCase("unconfirmed-prefix")]
        public void RehashedObservationCannotSkipAssignmentsOrInventConfirmedPrefix(string mutation)
        {
            using var f=new HumanCalibrationTestFactory();f.OpenFirst();var codec=Codec(f.Catalog);byte[] source=Encode(codec,f.Repository.Snapshot!);
            var changed=RehashPayload(source,s=>mutation=="skip-all"?s.Replace("\"nextAssignmentIndex\":0","\"nextAssignmentIndex\":2"):mutation=="unconfirmed-prefix"?s.Replace("\"nextAssignmentIndex\":0","\"nextAssignmentIndex\":1"):s.Replace("assigned-0","assigned-1"));
            Assert.Throws<ArgumentException>(()=>Decode(codec,changed));
        }
        [TestCase("hash")]
        [TestCase("source")]
        [TestCase("extra")]
        [TestCase("version")]
        public void RecordRejectsWrongHashSourceVersionAndUnexpectedGameFields(string mutation)
        {
            using var f=new HumanCalibrationTestFactory();f.OpenFirst();var codec=Codec(f.Catalog);string source=Encoding.UTF8.GetString(Encode(codec,f.Repository.Snapshot!));
            string altered=mutation=="hash"?source.Replace("\"payloadSha256\":\"","\"payloadSha256\":\"0"):mutation=="source"?source.Replace(f.Catalog.UiSourceSha256,new string('e',64)):mutation=="version"?source.Replace("\"humanRecordVersion\":1","\"humanRecordVersion\":2"):source.Replace("\"humanRecordVersion\":1","\"humanRecordVersion\":1,\"ledger\":{}");
            Assert.That(()=>Decode(codec,Encoding.UTF8.GetBytes(altered)),Throws.Exception);Assert.That(()=>Decode(codec,Encoding.UTF8.GetBytes("{\"recordVersion\":3,\"profile\":{}}")),Throws.Exception);
        }
        [Test]
        public void ActualSessionReloadThroughPhysicalCodecPreservesPossibleHintThenSolvesAndAdvances()
        {
            using var f=new HumanCalibrationTestFactory();string root=Root();var store=Store(root,f.Catalog);string token;Guid exposed;
            using(var session=new AssignedHumanSession(f.Catalog,store,f.Clock,f.Dispatcher,HumanRunOrigin.AUTOMATED_TEST))
            {
                f.Pump(session.InitializeAsync(CancellationToken.None));f.Pump(session.BeginRunAsync(CancellationToken.None));f.Pump(session.ReserveExposureAsync(CancellationToken.None));exposed=session.State.Exposure!.ExposureId;f.Pump(session.ConfirmExposureVisibleAsync(session.State.Exposure.AcknowledgeVisible(0),CancellationToken.None));f.Pump(session.RecordFirstContactAsync(HumanFirstContact.NEVER_SEEN,false,CancellationToken.None));session.Pause(0);Assert.That(f.Pump(session.PrepareHintAsync(CancellationToken.None)),Is.Not.Null);token=store.LoadAsync(CancellationToken.None).GetAwaiter().GetResult().Snapshot!.Run.SessionToken;
            }
            using(var resumed=new AssignedHumanSession(f.Catalog,Store(root,f.Catalog),f.Clock,f.Dispatcher,HumanRunOrigin.AUTOMATED_TEST))
            {
                f.Pump(resumed.InitializeAsync(CancellationToken.None));Assert.That(resumed.State.Trial!.Interrupted,Is.True);Assert.That(resumed.HintCredits,Is.EqualTo(2));Assert.That(resumed.State.Trial.Hints.Single().State,Is.EqualTo(HumanHintState.UNCERTAIN));f.Pump(resumed.ReserveExposureAsync(CancellationToken.None));Assert.That(resumed.State.Exposure!.ExposureId,Is.EqualTo(exposed));f.Pump(resumed.ConfirmExposureVisibleAsync(resumed.State.Exposure.AcknowledgeVisible(0),CancellationToken.None));Assert.That(resumed.State.Trial.EligibleFirstContact,Is.False);Assert.That(resumed.State.PreviewActiveMs,Is.Null);
                for(int x=0;x<4;x++){f.Clock.MonotonicMilliseconds+=10;Assert.That(resumed.Apply(x,0,DevelopmentTool.TrackEW,f.Clock.MonotonicMilliseconds),Is.EqualTo(DevelopmentCommandStatus.Applied));}f.Pump(resumed.TickAsync(CancellationToken.None));Assert.That(resumed.State.CanStartRide,Is.True);Assert.That(resumed.BeginTrainRide(resumed.AttemptId),Is.EqualTo(DevelopmentCommandStatus.Applied));Assert.That(resumed.CompleteTrainRide(resumed.AttemptId),Is.EqualTo(DevelopmentCommandStatus.Applied));f.Pump(resumed.NextAsync(CancellationToken.None));Assert.That(resumed.State.Assignment!.Id,Is.EqualTo(f.Catalog.Assignments[1].Id));
            }
            var loaded=Store(root,f.Catalog).LoadAsync(CancellationToken.None).GetAwaiter().GetResult();Assert.That(loaded.Status,Is.EqualTo(HumanStoreStatus.Ready));Assert.That(loaded.Snapshot!.Run.SessionToken,Is.EqualTo(token));Assert.That(loaded.Snapshot.Run.NextAssignmentIndex,Is.EqualTo(1));Assert.That(loaded.Snapshot.Run.Trials.Single().Outcome,Is.EqualTo(HumanTrialOutcome.SOLVED_UNREVIEWED));Assert.That(loaded.Snapshot.Run.Trials.Single().Checkpoint.ActiveElapsedMs,Is.EqualTo(30));Assert.That(loaded.Snapshot.Run.Trials.Single().TimingComplete,Is.False);
        }
        [Test]
        public void PhysicalStoreUsesIndependentNamespaceCasAndOriginalByteExport()
        {
            using var f=new HumanCalibrationTestFactory();f.Pump(f.Session.BeginRunAsync(CancellationToken.None));string root=Root();Directory.CreateDirectory(root);File.WriteAllText(Path.Combine(root,"product-save-sentinel"),"unchanged");var store=Store(root,f.Catalog);Assert.That(store.LoadAsync(CancellationToken.None).GetAwaiter().GetResult().Status,Is.EqualTo(HumanStoreStatus.Missing));var started=store.StartRunAsync(f.Repository.Snapshot!.Run,null,CancellationToken.None).GetAwaiter().GetResult();Assert.That(started.Status,Is.EqualTo(HumanStoreStatus.Committed));f.Pump(f.Session.ReserveExposureAsync(CancellationToken.None));var second=new HumanObservationSnapshot(2,f.Repository.Snapshot!.Run);Assert.That(store.CommitAsync(second,1,CancellationToken.None).GetAwaiter().GetResult().Status,Is.EqualTo(HumanStoreStatus.Committed));Assert.That(store.CommitAsync(second,1,CancellationToken.None).GetAwaiter().GetResult().Status,Is.EqualTo(HumanStoreStatus.Conflict));
            var loaded=Store(root,f.Catalog).LoadAsync(CancellationToken.None).GetAwaiter().GetResult();Assert.That(loaded.Status,Is.EqualTo(HumanStoreStatus.Ready));Assert.That(loaded.Snapshot!.Run.SessionToken,Is.EqualTo(started.Snapshot!.Run.SessionToken));var exported=store.ExportAsync(CancellationToken.None).GetAwaiter().GetResult();Assert.That(exported,Is.EqualTo(File.ReadAllBytes(Path.Combine(root,"human-calibration-v1",f.Catalog.ManifestBytesSha256,loaded.Snapshot.Run.SessionToken,"observations.json"))));Assert.That(Decode(Codec(f.Catalog),exported).Generation,Is.EqualTo(2));Assert.That(File.ReadAllText(Path.Combine(root,"product-save-sentinel")),Is.EqualTo("unchanged"));
            Assert.That(store.StartRunAsync(new HumanCalibrationRun(f.Catalog.ManifestBytesSha256,Guid.NewGuid().ToString("N"),HumanRunOrigin.AUTOMATED_TEST,f.Clock.UtcNow,0,Array.Empty<HumanTrialRecord>()),null,CancellationToken.None).GetAwaiter().GetResult().Status,Is.EqualTo(HumanStoreStatus.Conflict));
        }
        [TestCase("RECORD_PENDING_FLUSHED")]
        [TestCase("RECORD_MAIN_REPLACED")]
        [TestCase("INDEX_PENDING_FLUSHED")]
        [TestCase("INDEX_MAIN_REPLACED")]
        public void ActualAtomicFaultWindowsKeepSameRunAndExplicitRecoveryPreservesSources(string point)
        {
            using var f=new HumanCalibrationTestFactory();f.Pump(f.Session.BeginRunAsync(CancellationToken.None));string root=Root();string? armed=null;var store=Store(root,f.Catalog,p=>{if(p==armed){armed=null;throw new IOException("Controlled physical fault "+point);}});var start=store.StartRunAsync(f.Repository.Snapshot!.Run,null,CancellationToken.None).GetAwaiter().GetResult();Assert.That(start.Succeeded,Is.True);f.Pump(f.Session.ReserveExposureAsync(CancellationToken.None));armed=point;var result=store.CommitAsync(new HumanObservationSnapshot(2,f.Repository.Snapshot!.Run),1,CancellationToken.None).GetAwaiter().GetResult();Assert.That(result.Status,Is.EqualTo(HumanStoreStatus.RecoveryRequired));var observation=store.LoadAsync(CancellationToken.None).GetAwaiter().GetResult();Assert.That(observation.Status,Is.Not.EqualTo(HumanStoreStatus.Missing));if(!observation.Succeeded){Assert.That(observation.ObservationToken,Is.Not.Null);observation=store.RecoverAsync(observation.ObservationToken!,CancellationToken.None).GetAwaiter().GetResult();}
            Assert.That(observation.Succeeded,Is.True);Assert.That(observation.Snapshot!.Run.SessionToken,Is.EqualTo(start.Snapshot!.Run.SessionToken));Assert.That(observation.Snapshot.Generation,Is.EqualTo(2));Assert.That(observation.Snapshot.Run.Trials.Single().Exposure,Is.EqualTo(HumanExposureState.EXPOSURE_PENDING));Assert.That(Store(root,f.Catalog).LoadAsync(CancellationToken.None).GetAwaiter().GetResult().Status,Is.EqualTo(HumanStoreStatus.Ready));
        }
        [Test]
        [TestCase("RECOVERY_ANCHOR_COMMITTED")]
        [TestCase("RECOVERY_ROLES_CLEARED")]
        public void CrashDuringRecoveryAlwaysRetainsDurableRunAnchorAndPossibleExposure(string point)
        {
            using var f=new HumanCalibrationTestFactory();f.Pump(f.Session.BeginRunAsync(CancellationToken.None));string root=Root();string? armed=null;var store=Store(root,f.Catalog,p=>{if(armed==p){armed=null;throw new IOException("Recovery crash "+point);}});var start=store.StartRunAsync(f.Repository.Snapshot!.Run,null,CancellationToken.None).GetAwaiter().GetResult();f.Pump(f.Session.ReserveExposureAsync(CancellationToken.None));Assert.That(store.CommitAsync(new HumanObservationSnapshot(2,f.Repository.Snapshot!.Run),1,CancellationToken.None).GetAwaiter().GetResult().Succeeded,Is.True);string record=Path.Combine(root,"human-calibration-v1",f.Catalog.ManifestBytesSha256,start.Snapshot!.Run.SessionToken,"observations.json");File.WriteAllBytes(record+".bak",Encoding.UTF8.GetBytes("damaged role"));var observed=store.LoadAsync(CancellationToken.None).GetAwaiter().GetResult();armed=point;Assert.That(store.RecoverAsync(observed.ObservationToken!,CancellationToken.None).GetAwaiter().GetResult().Status,Is.EqualTo(HumanStoreStatus.RecoveryRequired));
            var reload=Store(root,f.Catalog);var pending=reload.LoadAsync(CancellationToken.None).GetAwaiter().GetResult();Assert.That(pending.Status,Is.EqualTo(HumanStoreStatus.RecoveryRequired));var recovered=reload.RecoverAsync(pending.ObservationToken!,CancellationToken.None).GetAwaiter().GetResult();Assert.That(recovered.Succeeded,Is.True,"Status="+recovered.Status+"; ErrorCode="+recovered.ErrorCode+"; root="+root);Assert.That(recovered.Snapshot!.Run.SessionToken,Is.EqualTo(start.Snapshot.Run.SessionToken));Assert.That(recovered.Snapshot.Run.Trials.Single().Exposure,Is.EqualTo(HumanExposureState.EXPOSURE_PENDING));Assert.That(recovered.Snapshot.Generation,Is.EqualTo(2));
        }
        [Test]
        public void InvalidBackupIsArchivedByteExactlyBeforeExplicitRecoveryAndSourceOrphansNeverBecomeMissing()
        {
            using var f=new HumanCalibrationTestFactory();f.Pump(f.Session.BeginRunAsync(CancellationToken.None));string root=Root();var store=Store(root,f.Catalog);var started=store.StartRunAsync(f.Repository.Snapshot!.Run,null,CancellationToken.None).GetAwaiter().GetResult();string record=Path.Combine(root,"human-calibration-v1",f.Catalog.ManifestBytesSha256,started.Snapshot!.Run.SessionToken,"observations.json");byte[] corrupt=Encoding.UTF8.GetBytes("exact preserved damaged observation");File.WriteAllBytes(record+".bak",corrupt);var pending=store.LoadAsync(CancellationToken.None).GetAwaiter().GetResult();Assert.That(pending.Status,Is.EqualTo(HumanStoreStatus.RecoveryRequired));var recovered=store.RecoverAsync(pending.ObservationToken!,CancellationToken.None).GetAwaiter().GetResult();Assert.That(recovered.Succeeded,Is.True,"Status="+recovered.Status+"; ErrorCode="+recovered.ErrorCode+"; root="+root);Assert.That(Directory.GetFiles(Path.Combine(root,"human-calibration-v1",f.Catalog.ManifestBytesSha256,"_recovery"),"*",SearchOption.AllDirectories).Any(p=>File.ReadAllBytes(p).SequenceEqual(corrupt)),Is.True);File.Delete(record);File.Delete(Path.Combine(root,"human-calibration-v1","active-run.json"));Assert.That(store.LoadAsync(CancellationToken.None).GetAwaiter().GetResult().Status,Is.Not.EqualTo(HumanStoreStatus.Missing));
        }
    }
}
