using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;
using STP.Infrastructure.Content;
using STP.Infrastructure.Persistence;

namespace STP.Bootstrap
{
    /// <summary>Presentation dependencies from an already composed game. This view grants no source or save authority.</summary>
    internal sealed class FullGameRuntimeContext
    {
        private readonly MobileCandidateComposition? candidate;
        private readonly ProductionGameComposition? runtime;
        internal FullGameRuntimeContext(MobileCandidateComposition value)
        {
            candidate=value??throw new ArgumentNullException(nameof(value));Catalog=value.Catalog;Campaign=value.Progress.Campaign;
            Catalogs=value.Progress.CatalogSet;Cosmetics=value.Cosmetics;Profile=value.Profile;Repository=value.Repository;
            Persisted=value.Persisted;Metagame=value.Metagame;CosmeticPolicy=value.CosmeticPolicy;PostClear=value.PostClear;
            EndlessPolicy=value.EndlessPolicy;
            EndlessProfileIds=Array.AsReadOnly(value.EndlessProfiles?.Profiles.Select(p=>p.Id).ToArray()??Array.Empty<string>());
        }
        internal FullGameRuntimeContext(ProductionGameComposition value)
        {
            runtime=value??throw new ArgumentNullException(nameof(value));Catalog=value.Catalog;Campaign=value.Progress.Campaign;
            Catalogs=value.Progress.CatalogSet;Cosmetics=value.Cosmetics;Profile=value.Profile;Repository=value.Repository;
            Persisted=value.Persisted;Metagame=value.Metagame;CosmeticPolicy=value.CosmeticPolicy;PostClear=value.PostClear;
            EndlessPolicy=value.EndlessPolicy;
            var displayOrder=new[]{"dienstbeginn","regelbetrieb","stoerungslage","grosslage"};
            if(value.CalibratedProfiles.Count!=4||!value.CalibratedProfiles.Select(p=>p.Id).OrderBy(x=>x,StringComparer.Ordinal)
                .SequenceEqual(displayOrder.OrderBy(x=>x,StringComparer.Ordinal)))throw new ArgumentException("ENDLESS_PROFILE_NAMED_SET_REQUIRED");
            EndlessProfileIds=Array.AsReadOnly(displayOrder);
        }
        internal IPublicPuzzleCatalog Catalog{get;}
        internal ILockedCampaignCatalog Campaign{get;}
        internal CatalogSetIdentity Catalogs{get;}
        internal LockedCosmeticsCatalog Cosmetics{get;}
        internal SaveProfileIdentity Profile{get;}
        internal AtomicSaveRepository Repository{get;}
        internal PersistedPuzzleSession Persisted{get;}
        internal LocalMetagamePolicy Metagame{get;}
        internal LocalCosmeticsPolicy CosmeticPolicy{get;}
        internal LocalPostClearPolicy PostClear{get;}
        internal LocalEndlessPolicy? EndlessPolicy{get;}
        internal IReadOnlyList<string> EndlessProfileIds{get;}
        internal Task<SaveCommitResult<LocalProfileState>> StartEndlessAsync(SavedPublic240ReviewSession saved,string profileId,string seed,CancellationToken cancellation)
        {
            if(EndlessPolicy==null||!EndlessProfileIds.Contains(profileId))throw new ArgumentException("ENDLESS_PROFILE_UNAVAILABLE");
            if(candidate!=null)return saved.StartEndlessAsync(EndlessPolicy,candidate.EndlessProfiles!.Get(profileId),seed,cancellation);
            var profile=runtime!.CalibratedProfiles.Single(p=>p.Id==profileId);
            return saved.StartEndlessAsync(EndlessPolicy,profile.Profile,seed,cancellation);
        }
    }
}
