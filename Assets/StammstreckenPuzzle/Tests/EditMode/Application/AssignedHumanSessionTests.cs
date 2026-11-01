using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using STP.Application;

namespace STP.Tests.Application.EditMode
{
    public sealed class AssignedHumanSessionTests
    {
        [TestCase("next")]
        [TestCase("abort")]
        [TestCase("unknown")]
        public void ActualViewIsCoherentDuringOwnerCommitAckBeforeFinallyPublication(string operation)
        {
            using var f=new HumanCalibrationTestFactory();f.OpenFirst();
            if(operation=="next")
            {
                for(int x=0;x<4;x++){f.Clock.MonotonicMilliseconds+=10;Assert.That(f.Session.Apply(x,0,DevelopmentTool.TrackEW,f.Clock.MonotonicMilliseconds),Is.EqualTo(DevelopmentCommandStatus.Applied));}f.Pump(f.Session.TickAsync(CancellationToken.None));Assert.That(f.Session.BeginTrainRide(f.Session.AttemptId),Is.EqualTo(DevelopmentCommandStatus.Applied));Assert.That(f.Session.CompleteTrainRide(f.Session.AttemptId),Is.EqualTo(DevelopmentCommandStatus.Applied));
            }
            f.Repository.Hold=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);f.Repository.Offered=null;f.Repository.UnknownNext=operation=="unknown";var pending=operation=="next"?f.Session.NextAsync(CancellationToken.None):f.Session.AbortAsync(CancellationToken.None);f.Until(()=>f.Repository.Offered!=null);Assert.That(f.Session.PuzzleView,Is.SameAs(f.Session));Assert.That(f.Session.State.ExposureReady,Is.True);Assert.That(f.Session.State.CanInput,Is.False);long beforeAckVersion=f.Session.State.Version;f.Repository.Hold.SetResult(true);
            Assert.That(SpinWait.SpinUntil(()=>f.Dispatcher.PendingCount>0,15000),Is.True,"Actual repository completion must enqueue its owner acknowledgement");Assert.That(f.Dispatcher.DrainOne(),Is.True);
            // No further owner publication is drained: this is the actual acknowledgement/finally gap.
            Assert.That(pending.IsCompleted,Is.False);Assert.That(f.Session.PuzzleView,Is.Null);Assert.That(f.Session.State.ExposureReady,Is.False);Assert.That(f.Session.State.Version,Is.GreaterThan(beforeAckVersion));Assert.That(f.Session.State.CanInput,Is.False);Assert.That(f.Session.State.CanAdvance,Is.False);Assert.That(f.Session.State.CanRestart,Is.False);
            if(operation=="unknown")Assert.Throws<InvalidOperationException>(()=>f.Pump(pending));else f.Pump(pending);Assert.That(f.Session.PuzzleView,Is.Null);if(operation=="next")Assert.That(f.Session.State.Assignment!.Id,Is.EqualTo(f.Catalog.Assignments[1].Id));
        }
        [Test]
        public void CompleteHasNoRemainingAssignmentActionsAndSecondNextCannotAdvancePastManifest()
        {
            using var f=new HumanCalibrationTestFactory();f.CompleteRunByExplicitAborts();long generation=f.Repository.Snapshot!.Generation;Assert.That(f.Session.State.IsFinished,Is.True);Assert.That(f.Session.State.Assignment,Is.Null);Assert.That(f.Session.State.CanAdvance,Is.False);Assert.That(f.Session.State.CanRestart,Is.False);Assert.That(f.Session.State.CanAbort,Is.False);Assert.Throws<ArgumentException>(()=>f.Pump(f.Session.NextAsync(CancellationToken.None)));Assert.Throws<ArgumentException>(()=>f.Pump(f.Session.RestartAsync(CancellationToken.None)));Assert.Throws<ArgumentException>(()=>f.Pump(f.Session.AbortAsync(CancellationToken.None)));Assert.That(f.Repository.Snapshot.Generation,Is.EqualTo(generation));Assert.That(f.Repository.Snapshot.Run.NextAssignmentIndex,Is.EqualTo(f.Catalog.Assignments.Count));Assert.That(f.Session.State.IsRecoveryRequired,Is.False);Assert.That(f.Session.State.CanBeginNextParticipant,Is.True);
        }
        [Test]
        public void NextParticipantRequiresCompletedRunAndExplicitActionPreservingPriorOriginalRun()
        {
            using var f=new HumanCalibrationTestFactory();Assert.That(f.Session.State.CanBeginNextParticipant,Is.False);Assert.Throws<ArgumentException>(()=>f.Pump(f.Session.BeginNextParticipantAsync(CancellationToken.None)));f.CompleteRunByExplicitAborts();string first=f.Repository.Snapshot!.Run.SessionToken;Assert.That(f.Session.State.IsFinished,Is.True);Assert.That(f.Session.State.CanBeginNextParticipant,Is.True);f.Session.SetSuspension(HumanPauseReason.External,true);Assert.That(f.Session.State.CanBeginNextParticipant,Is.False);f.Session.SetSuspension(HumanPauseReason.External,false);
            f.Repository.Hold=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);var start=f.Session.BeginNextParticipantAsync(CancellationToken.None);f.Until(()=>f.Repository.StartOffered?.SessionToken!=first);Assert.That(f.Session.State.CanBeginNextParticipant,Is.False);Assert.That(f.Session.State.CanInput,Is.False);Assert.That(f.Session.State.ExposureReady,Is.False);Assert.That(f.Repository.Snapshot.Run.SessionToken,Is.EqualTo(first));f.Repository.Hold.SetResult(true);f.Pump(start);
            Assert.That(f.Repository.Snapshot.Run.SessionToken,Is.Not.EqualTo(first));Assert.That(f.Repository.Archived.Single().Run.SessionToken,Is.EqualTo(first));Assert.That(f.Repository.Archived.Single().Run.NextAssignmentIndex,Is.EqualTo(2));Assert.That(f.Session.State.IsFinished,Is.False);Assert.That(f.Session.State.Assignment!.Id,Is.EqualTo(f.Catalog.Assignments[0].Id));Assert.That(f.Session.State.FirstContactRecorded,Is.False);Assert.That(f.Repository.Snapshot.Run.Trials,Is.Empty);f.Pump(f.Session.ReserveExposureAsync(CancellationToken.None));Assert.That(f.Session.HintCredits,Is.EqualTo(3));Assert.That(f.Session.State.CanInput,Is.False);Assert.That(f.Session.State.Trial!.Contact,Is.Null);
        }
        [TestCase("unknown")]
        [TestCase("throw")]
        [TestCase("cancel")]
        public void NextParticipantUnknownWriteRemainsClosedAndNeverStartsAgainAutomatically(string failure)
        {
            using var f=new HumanCalibrationTestFactory();f.CompleteRunByExplicitAborts();string prior=f.Repository.Snapshot!.Run.SessionToken;f.Repository.Hold=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);using var cancel=new CancellationTokenSource();var start=f.Session.BeginNextParticipantAsync(cancel.Token);f.Until(()=>f.Repository.StartOffered!.SessionToken!=prior);if(failure=="unknown")f.Repository.UnknownNext=true;else if(failure=="throw")f.Repository.ThrowNext=new System.IO.IOException("controlled new participant failure");else cancel.Cancel();f.Repository.Hold.SetResult(true);Assert.That(()=>f.Pump(start),Throws.Exception);Assert.That(f.Session.State.IsRecoveryRequired,Is.True);Assert.That(f.Session.State.CanBeginNextParticipant,Is.False);Assert.That(f.Session.PuzzleView,Is.Null);string actual=f.Repository.Snapshot!.Run.SessionToken;f.Pump(f.Session.RetryAsync(CancellationToken.None));Assert.That(f.Repository.Snapshot.Run.SessionToken,Is.EqualTo(actual));Assert.That(actual==prior,Is.EqualTo(failure!="unknown"));Assert.That(f.Repository.Archived.Count,Is.EqualTo(failure=="unknown"?1:0));
        }
        [Test]
        public void QueuedDuplicateNextParticipantCannotCreateAnotherRunAndReloadKeepsFinishedToken()
        {
            var repository=new HumanCalibrationTestFactory.MemoryRepository();string original;
            using(var first=new HumanCalibrationTestFactory(repository)){first.CompleteRunByExplicitAborts();original=repository.Snapshot!.Run.SessionToken;}
            using var f=new HumanCalibrationTestFactory(repository);Assert.That(f.Repository.Snapshot!.Run.SessionToken,Is.EqualTo(original));Assert.That(f.Session.State.CanBeginNextParticipant,Is.True);f.Repository.Hold=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);var start=f.Session.BeginNextParticipantAsync(CancellationToken.None);var duplicate=f.Session.BeginNextParticipantAsync(CancellationToken.None);f.Until(()=>f.Repository.StartOffered!.SessionToken!=original);Assert.That(duplicate.IsCompleted,Is.False);f.Repository.Hold.SetResult(true);f.Pump(start);Assert.Throws<ArgumentException>(()=>f.Pump(duplicate));Assert.That(repository.Archived.Count,Is.EqualTo(1));
        }
        [TestCase(false)]
        [TestCase(true)]
        public void RepositoryThrowOrCancellationAfterAttemptNeverReopensInput(bool cancellation)
        {
            using var f=new HumanCalibrationTestFactory();f.OpenFirst();f.Repository.Hold=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);f.Repository.Offered=null;using var cancel=new CancellationTokenSource();var abort=f.Session.AbortAsync(cancel.Token);f.Until(()=>f.Repository.Offered!=null);
            if(cancellation)cancel.Cancel();else f.Repository.ThrowNext=new System.IO.IOException("Controlled repository failure after call");f.Repository.Hold.SetResult(true);
            if(cancellation)Assert.Throws<OperationCanceledException>(()=>f.Pump(abort));else Assert.Throws<System.IO.IOException>(()=>f.Pump(abort));
            Assert.That(f.Session.State.IsRecoveryRequired,Is.True);Assert.That(f.Session.State.ExposureReady,Is.False);Assert.That(f.Session.State.CanInput,Is.False);Assert.That(f.Session.State.CanRestart,Is.False);Assert.That(f.Session.State.CanAbort,Is.False);Assert.That(f.Session.PuzzleView,Is.Null);
        }
        [Test]
        public void RetryWaitsForHeldCommitAndCannotOverwriteNewerAuthority()
        {
            using var f=new HumanCalibrationTestFactory();f.OpenFirst();f.Repository.Hold=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);f.Repository.Offered=null;var abort=f.Session.AbortAsync(CancellationToken.None);f.Until(()=>f.Repository.Offered!=null);var retry=f.Session.RetryAsync(CancellationToken.None);Assert.That(retry.IsCompleted,Is.False);Assert.That(f.Session.State.CanInput,Is.False);f.Repository.Hold.SetResult(true);f.Pump(abort);f.Pump(retry);Assert.That(f.Session.State.Trial!.Outcome,Is.EqualTo(HumanTrialOutcome.ABORTED));Assert.That(f.Repository.Snapshot!.Run.Trials.Single().Outcome,Is.EqualTo(HumanTrialOutcome.ABORTED));Assert.That(f.Session.State.CanAdvance,Is.True);
        }
        [Test]
        public void ConcurrentHintRequestsCannotComputeOrSpendPastTheSamePendingDisplay()
        {
            using var f=new HumanCalibrationTestFactory();f.OpenFirst();
            using var firstCancellation=new CancellationTokenSource();using var secondCancellation=new CancellationTokenSource();using var thirdCancellation=new CancellationTokenSource();
            f.Repository.Hold=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);f.Repository.Offered=null;f.Repository.ResetCommitObservation();
            var first=f.Session.PrepareHintAsync(firstCancellation.Token);var second=f.Session.PrepareHintAsync(secondCancellation.Token);
            f.Until(()=>f.Repository.ObservedCommit!=null||first.IsCompleted&&second.IsCompleted);
            var observed=f.Repository.ObservedCommit;
            Assert.That(observed,Is.Not.Null,"The concurrency boundary requires an actual pending hint commit.");
            bool firstOffered=observed!.Cancellation==firstCancellation.Token;
            Assert.That(firstOffered||observed.Cancellation==secondCancellation.Token,Is.True,"The held commit must identify one of these two requests.");
            var pending=firstOffered?first:second;var competing=second;
            if(!firstOffered)
            {
                Assert.That(f.Pump(first),Is.Null,"A first request that did not offer the held commit must return a legitimate unavailable hint.");
                competing=f.Session.PrepareHintAsync(thirdCancellation.Token);
            }
            var offeredHint=observed.Snapshot.Run.Trials.Single().Hints.Single();
            Assert.That(offeredHint.State,Is.EqualTo(HumanHintState.HINT_DISPLAY_PENDING));
            Assert.That(observed.Cancellation.IsCancellationRequested,Is.False);
            Assert.That(pending.IsCompleted,Is.False);Assert.That(competing.IsCompleted,Is.False);
            f.Repository.Hold.SetResult(true);
            var hint=f.Pump(pending);Assert.That(hint,Is.Not.Null);
            var reservedId=typeof(SafePuzzleHint).GetProperty("ReservedDisplayId",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
            Assert.That(reservedId,Is.Not.Null,"The Editor-only identity check must read the existing internal reservation without acknowledging display.");
            Assert.That(reservedId!.PropertyType,Is.EqualTo(typeof(Guid)));Assert.That((Guid)reservedId.GetValue(hint)!,Is.EqualTo(offeredHint.DisplayId));
            Assert.Throws<ArgumentException>(()=>f.Pump(competing));
            Assert.That(f.Session.HintCredits,Is.EqualTo(2));Assert.That(f.Repository.Snapshot!.Run.Trials.Single().Hints.Count,Is.EqualTo(1));Assert.That(f.Session.State.IsRecoveryRequired,Is.False);
        }
        [Test]
        public void ExposureIsInvisibleUntilPendingCommitAndInputNeedsRealVisibleReceiptAndSelfReport()
        {
            using var f=new HumanCalibrationTestFactory();f.Pump(f.Session.BeginRunAsync(CancellationToken.None));f.Repository.Hold=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);var reserve=f.Session.ReserveExposureAsync(CancellationToken.None);f.Until(()=>f.Repository.Offered!=null);
            Assert.That(f.Session.State.ExposureReady,Is.False);Assert.That(f.Session.PuzzleView,Is.Null);Assert.Throws<InvalidOperationException>(()=>{var _=f.Session.Puzzle;});Assert.That(f.Repository.Offered!.Run.Trials.Single().Exposure,Is.EqualTo(HumanExposureState.EXPOSURE_PENDING));
            f.Repository.Hold.SetResult(true);f.Pump(reserve);Assert.That(f.Session.State.ExposureReady,Is.True);Assert.That(f.Session.State.CanInput,Is.False);Assert.That(f.Session.Puzzle.Identity,Is.EqualTo(f.Catalog.Assignments[0].Puzzle.Identity));
            var receipt=f.Session.State.Exposure!.AcknowledgeVisible(f.Clock.MonotonicMilliseconds);f.Pump(f.Session.ConfirmExposureVisibleAsync(receipt,CancellationToken.None));Assert.That(f.Session.State.CanInput,Is.False);f.Pump(f.Session.RecordFirstContactAsync(HumanFirstContact.NEVER_SEEN,false,CancellationToken.None));Assert.That(f.Session.State.CanInput,Is.True);Assert.That(f.Session.State.Trial!.EligibleFirstContact,Is.True);Assert.That(f.Session.TimerStarted,Is.False);
        }
        [Test]
        public void SameProductPauseAndMonotonicCommandsKeepTrueActiveTimeAndSavingNeverWithdrawsReveal()
        {
            using var f=new HumanCalibrationTestFactory();f.OpenFirst();f.Clock.MonotonicMilliseconds=100;Assert.That(f.Session.Apply(0,0,DevelopmentTool.TrackEW,100),Is.EqualTo(DevelopmentCommandStatus.Applied));f.Clock.MonotonicMilliseconds=200;f.Session.SetSuspension(HumanPauseReason.External,true);long elapsed=f.Session.ElapsedAt(200);f.Clock.MonotonicMilliseconds=1000;Assert.That(f.Session.Resume(1000),Is.EqualTo(DevelopmentCommandStatus.Rejected));Assert.That(f.Session.ElapsedAt(1000),Is.EqualTo(elapsed));f.Session.SetSuspension(HumanPauseReason.External,false);Assert.That(f.Session.State.CanInput,Is.True);Assert.That(f.Session.ElapsedAt(1100),Is.EqualTo(elapsed+100));
            f.Clock.MonotonicMilliseconds=1500;f.Repository.Hold=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);f.Repository.Offered=null;var tick=f.Session.TickAsync(CancellationToken.None);f.Until(()=>f.Repository.Offered!=null);Assert.That(f.Session.State.ExposureReady,Is.True);Assert.That(f.Session.PuzzleView,Is.SameAs(f.Session));Assert.That(f.Session.State.CanInput,Is.False);f.Repository.Hold.SetResult(true);f.Pump(tick);Assert.That(f.Session.State.CanInput,Is.True);Assert.That(f.Session.State.Trial!.Checkpoint.Mode.ToString(),Is.EqualTo("PRACTICE"));
        }
        [Test]
        public void HintPreparationAfterCommonHelpPauseWaitsForPendingCommitAndDuplicateDisplayDoesNotSpendTwice()
        {
            using var f=new HumanCalibrationTestFactory();f.OpenFirst();f.Session.Pause(f.Clock.MonotonicMilliseconds);f.Session.SetSuspension(HumanPauseReason.Hint,true);f.Repository.Hold=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);f.Repository.Offered=null;var prepare=f.Session.PrepareHintAsync(CancellationToken.None);f.Until(()=>f.Repository.Offered!=null);Assert.That(prepare.IsCompleted,Is.False);Assert.That(f.Repository.Offered!.Run.Trials.Last().Hints.Single().State,Is.EqualTo(HumanHintState.HINT_DISPLAY_PENDING));f.Repository.Hold.SetResult(true);var hint=f.Pump(prepare);Assert.That(hint,Is.Not.Null);Assert.That(f.Session.HintCredits,Is.EqualTo(2));Assert.That(f.Session.State.Trial!.Checkpoint.HintCount,Is.Zero);Assert.That(f.Session.TimerStarted,Is.False);
            var receipt=hint!.AcknowledgeDisplayed();f.Pump(f.Session.CommitHintDisplayAsync(receipt,CancellationToken.None));long revision=f.Session.Revision;f.Pump(f.Session.CommitHintDisplayAsync(receipt,CancellationToken.None));Assert.That(f.Session.Revision,Is.EqualTo(revision));Assert.That(f.Session.State.Trial!.Checkpoint.HintCount,Is.EqualTo(1));Assert.That(f.Session.HintCredits,Is.EqualTo(2));f.Session.SetSuspension(HumanPauseReason.Hint,false);f.Session.Resume(f.Clock.MonotonicMilliseconds);f.Pump(f.Session.RestartAsync(CancellationToken.None));Assert.That(f.Session.HintCredits,Is.EqualTo(2));Assert.Throws<ArgumentException>(()=>f.Pump(f.Session.CommitHintDisplayAsync(receipt,CancellationToken.None)));
        }
        [Test]
        public void RestartAndProcessReloadNeverReturnFreshFirstContactOrPossibleHintBudget()
        {
            var repository=new HumanCalibrationTestFactory.MemoryRepository();string token;Guid exposure;SafeHintDisplayReceipt oldHint;
            using(var first=new HumanCalibrationTestFactory(repository)){first.OpenFirst();token=repository.Snapshot!.Run.SessionToken;exposure=first.Session.State.Exposure!.ExposureId;first.Session.Pause(first.Clock.MonotonicMilliseconds);var hint=first.Pump(first.Session.PrepareHintAsync(CancellationToken.None));oldHint=hint!.AcknowledgeDisplayed();Assert.That(first.Session.HintCredits,Is.EqualTo(2));}
            using var reload=new HumanCalibrationTestFactory(repository);Assert.That(repository.Snapshot!.Run.SessionToken,Is.EqualTo(token));Assert.That(reload.Session.State.ExposureReady,Is.False);Assert.That(reload.Session.State.Trial!.Interrupted,Is.True);Assert.That(reload.Session.State.Trial.Hints.Single().State,Is.EqualTo(HumanHintState.UNCERTAIN));Assert.That(reload.Session.HintCredits,Is.EqualTo(2));reload.Pump(reload.Session.ReserveExposureAsync(CancellationToken.None));Assert.That(reload.Session.State.Exposure!.ExposureId,Is.EqualTo(exposure));reload.Clock.MonotonicMilliseconds++;reload.Pump(reload.Session.ConfirmExposureVisibleAsync(reload.Session.State.Exposure.AcknowledgeVisible(reload.Clock.MonotonicMilliseconds),CancellationToken.None));Assert.That(reload.Session.State.Trial!.EligibleFirstContact,Is.False);Assert.That(reload.Session.State.Trial.TimingComplete,Is.False);Assert.That(reload.Session.State.PreviewActiveMs,Is.Null);Assert.Throws<ArgumentException>(()=>reload.Pump(reload.Session.CommitHintDisplayAsync(oldHint,CancellationToken.None)));
        }
        [Test]
        public void CrashBeforeAnyCommandOrVisibleReceiptKeepsSamePendingExposureAndCannotBeFresh()
        {
            var repository=new HumanCalibrationTestFactory.MemoryRepository();HumanExposureReceipt old;
            using(var first=new HumanCalibrationTestFactory(repository)){first.Pump(first.Session.BeginRunAsync(CancellationToken.None));first.Pump(first.Session.ReserveExposureAsync(CancellationToken.None));old=first.Session.State.Exposure!.AcknowledgeVisible(0);Assert.That(first.Session.TimerStarted,Is.False);}
            using var reload=new HumanCalibrationTestFactory(repository);reload.Pump(reload.Session.ReserveExposureAsync(CancellationToken.None));Assert.Throws<ArgumentException>(()=>reload.Pump(reload.Session.ConfirmExposureVisibleAsync(old,CancellationToken.None)));reload.ConfirmVisible();Assert.That(reload.Session.State.Trial!.PriorExposure,Is.True);Assert.That(reload.Session.State.Trial.EligibleFirstContact,Is.False);
        }
        [Test]
        public void RealSolvedDomainWaitsForObservationAckThenRideAndNextWithoutAnyProductProgress()
        {
            using var f=new HumanCalibrationTestFactory();f.OpenFirst();for(int x=0;x<4;x++){f.Clock.MonotonicMilliseconds+=10;Assert.That(f.Session.Apply(x,0,DevelopmentTool.TrackEW,f.Clock.MonotonicMilliseconds),Is.EqualTo(DevelopmentCommandStatus.Applied));}Assert.That(f.Session.State.CanStartRide,Is.False);Assert.That(f.Session.Phase,Is.EqualTo(DevelopmentPreviewPhase.Editing));
            f.Repository.Hold=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);f.Repository.Offered=null;var complete=f.Session.TickAsync(CancellationToken.None);f.Until(()=>f.Repository.Offered!=null);Assert.That(f.Session.State.CanStartRide,Is.False);Assert.That(f.Session.BeginTrainRide(f.Session.AttemptId),Is.EqualTo(DevelopmentCommandStatus.Rejected));f.Repository.Hold.SetResult(true);f.Pump(complete);Assert.That(f.Session.State.Trial!.Outcome,Is.EqualTo(HumanTrialOutcome.SOLVED_UNREVIEWED));Assert.That(f.Session.State.CanStartRide,Is.True);Assert.That(f.Session.BeginTrainRide(f.Session.AttemptId),Is.EqualTo(DevelopmentCommandStatus.Applied));Assert.That(f.Session.CompleteTrainRide(f.Session.AttemptId),Is.EqualTo(DevelopmentCommandStatus.Applied));Assert.That(f.Session.State.CanAdvance,Is.True);f.Pump(f.Session.NextAsync(CancellationToken.None));Assert.That(f.Session.State.Assignment!.Id,Is.EqualTo(f.Catalog.Assignments[1].Id));Assert.That(f.Session.PuzzleView,Is.Null);Assert.That(f.Repository.Snapshot!.Run.Origin,Is.EqualTo(HumanRunOrigin.AUTOMATED_TEST));
        }
        [Test]
        public void UnknownExposureWriteDoesNotRevealAndRequiresExplicitRecoveryWithoutNewToken()
        {
            using var f=new HumanCalibrationTestFactory();f.Pump(f.Session.BeginRunAsync(CancellationToken.None));string token=f.Repository.Snapshot!.Run.SessionToken;f.Repository.UnknownNext=true;Assert.Throws<InvalidOperationException>(()=>f.Pump(f.Session.ReserveExposureAsync(CancellationToken.None)));Assert.That(f.Session.PuzzleView,Is.Null);Assert.That(f.Session.State.IsRecoveryRequired,Is.True);f.Pump(f.Session.RetryAsync(CancellationToken.None));Assert.That(f.Repository.Snapshot!.Run.SessionToken,Is.EqualTo(token));Assert.That(f.Session.State.Trial!.Interrupted,Is.True);Assert.That(f.Session.State.ExposureReady,Is.False);f.Pump(f.Session.ReserveExposureAsync(CancellationToken.None));f.ConfirmVisible();Assert.That(f.Session.State.Trial!.EligibleFirstContact,Is.False);
        }
    }
}
