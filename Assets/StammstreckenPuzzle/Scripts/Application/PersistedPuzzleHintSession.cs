using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Puzzle.Domain;

namespace STP.Application
{
    public sealed partial class PersistedPuzzleSession
    {
        /// <summary>Prepares one public-root conclusion off the application thread, spending nothing.</summary>
        public async Task<SafePuzzleHint?> PrepareHintAsync(LocalMetagamePolicy hints, CancellationToken cancellationToken)
        {
            if (hints == null) throw new ArgumentNullException(nameof(hints));
            var input = await Capture(() =>
            {
                LocalModel.Require(CanMutate && session != null && durable != null, "SAVE_USE_CASE_BLOCKED");
                var identity = new PublicPuzzleIdentity(session!.PuzzleId, session.PublicPuzzleHashProfile, session.PublicPuzzleHashSha256);
                puzzles.TryGet(identity,out var puzzle);if(puzzle==null)puzzle=durable!.Payload.Endless.OpenEndless.OfType<ActiveEndlessDraft>().SingleOrDefault(a=>a.PublicPuzzleInput.Identity.Equals(identity))?.PublicPuzzleInput;
                LocalModel.Require(puzzle!=null,"HINT_IDENTITY_NOT_ACTIVE");
                return (Snapshot: durable!, Puzzle: puzzle!, Checkpoint: PuzzleSessionCheckpoint.Export(session, clock.MonotonicMilliseconds));
            }).ConfigureAwait(false);
            return await Task.Run(() => hints.HintCredits(input.Snapshot.Payload, input.Puzzle.Identity) > 0 ? SafePuzzleHints.Prepare(input.Puzzle, input.Checkpoint, cancellationToken) : null, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>Serially commits one actual display, its credit and the live attempt's HintCount together.</summary>
        public async Task<SaveCommitResult<LocalProfileState>> CommitHintDisplayAsync(LocalMetagamePolicy hints, SafeHintDisplayReceipt receipt, CancellationToken cancellationToken)
        {
            if (hints == null || receipt == null) throw new ArgumentNullException();
            await writer.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var replay = await Capture(() => durable?.Payload.Metagame?.Hints.Any(h => h.Identity.Equals(receipt.Identity) && h.DisplayedIds.Contains(receipt.DisplayId.ToString("D"))) == true ? durable : null).ConfigureAwait(false);
                if(replay!=null)return SaveCommitResult<LocalProfileState>.Success(replay);
                var input = await Capture(() =>
                {
                    LocalModel.Require(CanMutate && session != null && durable != null, "SAVE_USE_CASE_BLOCKED");
                    LocalModel.Require(session!.AttemptId == receipt.AttemptId && session.Revision == receipt.ExpectedRevision && session.PuzzleId == receipt.Identity.PuzzleId && session.PublicPuzzleHashProfile == receipt.Identity.HashProfile && session.PublicPuzzleHashSha256 == receipt.Identity.HashSha256, "HINT_DISPLAY_ATTEMPT_MISMATCH");
                    blocked = true;
                    return (Snapshot: durable!, Session: session, Monotonic: clock.MonotonicMilliseconds, Utc: clock.UtcNow);
                }).ConfigureAwait(false);
                LocalProfileState payload;
                try
                {
                    payload = await Task.Run(() =>
                    {
                        var checkpoint = PuzzleSessionCheckpoint.Export(input.Session!, input.Monotonic);
                        var draft = LocalDraft.Create(receipt.Identity, input.Utc, checkpoint);
                        var current = input.Session!.Mode==SessionMode.ENDLESS?
                            (endlessPolicy??throw new ArgumentException("ENDLESS_COMPOSITION_REQUIRED")).SaveDraft(input.Snapshot.Payload,input.Session,input.Monotonic):policy.WithDraft(input.Snapshot.Payload, draft);
                        return hints.ConsumeHintDisplay(current, receipt);
                    }, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    await dispatcher.InvokeAsync(() => { blocked = false; }).ConfigureAwait(false); throw;
                }
                var result = await CommitUnderLock(payload, false, cancellationToken).ConfigureAwait(false);
                if (result.Succeeded && result.Snapshot != null)
                {
                    var savedCheckpoint=input.Session!.Mode==SessionMode.ENDLESS?result.Snapshot.Payload.Endless.OpenEndless.OfType<ActiveEndlessDraft>().Single(a=>a.PublicPuzzleInput.Identity.Equals(receipt.Identity)).Checkpoint:result.Snapshot.Payload.Drafts.Single(d=>d.Identity.Equals(receipt.Identity)).Checkpoint;
                    var restored = savedCheckpoint.Restore(input.Session!.Definition, receipt.Identity.PuzzleId, receipt.Identity.HashProfile, receipt.Identity.HashSha256);
                    await dispatcher.InvokeAsync(() =>
                    {
                        if (!ReferenceEquals(session, input.Session) || restored.Value == null) { blocked = true; Status = PersistedSessionStatus.RecoveryRequired; return; }
                        session = restored.Value; restoredCheckpoint = savedCheckpoint;
                    }).ConfigureAwait(false);
                }
                else if(result.Status==SaveCommitStatus.Failed||result.Status==SaveCommitStatus.Cancelled)
                    await dispatcher.InvokeAsync(()=>{if(ReferenceEquals(session,input.Session))blocked=false;}).ConfigureAwait(false);
                return result;
            }
            finally { writer.Release(); }
        }
    }
}
