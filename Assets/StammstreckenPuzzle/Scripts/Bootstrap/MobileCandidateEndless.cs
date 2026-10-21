using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using STP.Application;
using STP.Presentation.UI;

namespace STP.Bootstrap
{
    public sealed partial class MobileCandidateExperience
    {
        private bool generatingEndless;
        private readonly Func<string> nextEndlessSeed;
        private string? activeEndlessTextId,activeEndlessProfile;
        private void BindEndless()
        {
            if(composition.EndlessPolicy==null||composition.EndlessProfiles==null)return;
            presenter.BindEndless(ReadEndless,id=>TrackUserAction(StartEndlessAsync(id)),id=>TrackUserAction(ResumeEndlessAsync(id)),id=>TrackUserAction(AbandonEndlessAsync(id)));
            presenter.BindEndlessReward(CanRequestEndlessVideo,(id,video)=>TrackUserAction(RequestEndlessRewardAsync(id,video)),CanSkipEndlessReward);
            presenter.BindDynamicPuzzleText(ReadEndlessText);
        }
        private ProductionPuzzleDisplay? ReadEndlessText(string id)
        {
            var item=State?.Endless.OpenEndless.SingleOrDefault(e=>e.Id==id);
            if(item?.Descriptor.Parameters is SearchGeneratorParameters parameters)
            {activeEndlessTextId=id;activeEndlessProfile=parameters.ProfileId;}
            return id==activeEndlessTextId&&activeEndlessProfile!=null?presenter.ResolveEndlessDisplay(activeEndlessProfile):null;
        }
        private bool EndlessUnlocked=>State!=null&&composition.Catalog.Puzzles.All(p=>
            State.Levels.Any(l=>l.Identity.Equals(p.Identity)&&l.FirstClear!=null));
        private EndlessView ReadEndless()
        {
            var state=State;bool ready=state!=null&&!externalInputSuspended&&saved.CanRunProfileAction&&!generatingEndless;
            var records=state?.Endless.OpenEndless.Select(e=>new EndlessEntryView(e.Id,
                ((SearchGeneratorParameters)e.Descriptor.Parameters).ProfileId,
                e is ReservedEndless?EndlessEntryPhase.Reserved:e is ActiveEndlessDraft?EndlessEntryPhase.Active:EndlessEntryPhase.CompletionPending,
                ready&&(e is ReservedEndless||e is ActiveEndlessDraft),ready&&(e is ReservedEndless||e is ActiveEndlessDraft))).ToArray()
                ??Array.Empty<EndlessEntryView>();
            var profiles=ready&&EndlessUnlocked&&records.Length<20?composition.EndlessProfiles!.Profiles.Select(p=>p.Id).ToArray():Array.Empty<string>();
            string status=generatingEndless?"generating":saved.EndlessGenerationStatus;
            return new EndlessView(EndlessUnlocked,generatingEndless,profiles,records,status);
        }
        private static string FreshEndlessSeed()
        {
            var bytes=new byte[8];using(var source=RandomNumberGenerator.Create())source.GetBytes(bytes);
            return BitConverter.ToUInt64(bytes,0).ToString(CultureInfo.InvariantCulture);
        }
        private async Task StartEndlessAsync(string profileId)
        {
            if(generatingEndless||externalInputSuspended||!saved.CanRunProfileAction||!EndlessUnlocked||composition.EndlessProfiles==null||composition.EndlessPolicy==null)
                throw new InvalidOperationException("ENDLESS_UNAVAILABLE");
            var profile=composition.EndlessProfiles.Get(profileId);generatingEndless=true;
            try
            {
                var result=await saved.StartEndlessAsync(composition.EndlessPolicy,profile,nextEndlessSeed(),cancellation);
                RequireCommit(result);
                var active=result.Snapshot!.Payload.Endless.OpenEndless.OfType<ActiveEndlessDraft>()
                    .SingleOrDefault(e=>e.GenerationOrdinal==result.Snapshot.Payload.Endless.HighestReservedOrdinal);
                if(active!=null&&saved.PuzzleView?.Puzzle.Identity.Equals(active.PublicPuzzleInput.Identity)==true)presenter.ShowCurrentPuzzle();
            }
            finally{generatingEndless=false;presenter.Refresh();}
        }
        private async Task ResumeEndlessAsync(string id)
        {
            if(generatingEndless||externalInputSuspended||!saved.CanRunProfileAction||composition.EndlessPolicy==null)throw new InvalidOperationException("ENDLESS_BUSY");
            generatingEndless=true;
            try
            {
                RequireCommit(await saved.ResumeEndlessAsync(composition.EndlessPolicy,id,cancellation));
                if(saved.CurrentModeName=="ENDLESS"&&saved.PuzzleView?.Puzzle.Identity.PuzzleId==id)presenter.ShowCurrentPuzzle();
            }
            finally{generatingEndless=false;presenter.Refresh();}
        }
        private async Task AbandonEndlessAsync(string id)
        {
            if(externalInputSuspended||!saved.CanRunProfileAction||composition.EndlessPolicy==null)throw new InvalidOperationException("ENDLESS_UNAVAILABLE");
            RequireCommit(await saved.AbandonEndlessAsync(composition.EndlessPolicy,id,cancellation));presenter.Refresh();
        }
        private OpenEndlessCompletion? EndlesCompletion(string id)=>State?.Endless.OpenEndless.OfType<OpenEndlessCompletion>().SingleOrDefault(c=>c.Id==id);
        private bool CanRequestEndlessVideo(string id)=>!generatingEndless&&!externalInputSuspended&&saved.CanRunProfileAction&&
            services.Capabilities.CanRequestAds&&EndlesCompletion(id)?.Claim.ClaimStatus=="LOCAL_DECISION_PENDING";
        private bool CanSkipEndlessReward(string id)=>!generatingEndless&&!externalInputSuspended&&saved.CanRunProfileAction&&EndlesCompletion(id)?.Claim.ClaimStatus=="LOCAL_DECISION_PENDING";
        private CompletionBonusView ReadEndlessBonus(string id)
        {
            var done=EndlesCompletion(id);
            var terminal=State?.Endless.EconomyReceipts.SingleOrDefault(t=>t.SubjectId==id&&t.ReasonCode=="POST_CLEAR_PATIENCE");
            bool pending=done?.Claim.ClaimStatus=="PROVIDER_RESERVED"||done?.Claim.ClaimStatus=="RECONCILIATION_REQUIRED";
            return new CompletionBonusView(CanRequestEndlessVideo(id),CanSkipEndlessReward(id),pending,terminal==null?(int?)null:(int)terminal.Amount);
        }
        private async Task RequestEndlessRewardAsync(string id,bool video)
        {
            if(composition.EndlessPolicy==null)throw new InvalidOperationException("ENDLESS_UNAVAILABLE");
            if(!video)
            {
                if(!CanSkipEndlessReward(id))throw new InvalidOperationException("ENDLESS_SKIP_UNAVAILABLE");
                await CommitAsync((use,ct)=>use.SkipEndlessRewardAsync(composition.EndlessPolicy,id,ct));return;
            }
            if(!CanRequestEndlessVideo(id))throw new InvalidOperationException("ENDLESS_VIDEO_UNAVAILABLE");
            var committed=await saved.RunProfileActionAsync((use,ct)=>use.ReserveEndlessRewardAsync(composition.EndlessPolicy,id,ct),cancellation);
            RequireCommit(committed);var reservation=LocalEndlessPolicy.RewardReservation(committed,id);
            var shown=await services.ShowRewardedAsync(reservation,cancellation);await services.PendingCallbacks;
            await CommitAsync((use,ct)=>use.ResolveEndlessRewardAsync(composition.EndlessPolicy,reservation,DefinitelyBeforeShow(shown.Code),ct));
        }
    }
}
