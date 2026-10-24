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
    /// <summary>Explicit runtime composition from already checked immutable ADR044 bytes; no source approval, migration or SDK initialization.</summary>
    public sealed class ProductionGameComposition
    {
        private ProductionGameComposition(RuntimeGameSources sources,SaveProfileIdentity profile,AtomicSaveRepository repository,
            PersistedPuzzleSession persisted,AutosaveCoordinator autosave,LocalMetagamePolicy metagame,LocalCosmeticsPolicy cosmetics,
            LocalPostClearPolicy postClear,LocalEndlessPolicy endless)
        {Sources=sources;Profile=profile;Repository=repository;Persisted=persisted;Autosave=autosave;Metagame=metagame;CosmeticPolicy=cosmetics;PostClear=postClear;EndlessPolicy=endless;}
        public RuntimeGameSources Sources{get;}public IPublicPuzzleCatalog Catalog=>Sources.Catalog;public RuntimeProgressCatalog Progress=>Sources.Progress;public LockedCosmeticsCatalog Cosmetics=>Sources.Cosmetics;
        public SaveProfileIdentity Profile{get;}public AtomicSaveRepository Repository{get;}public PersistedPuzzleSession Persisted{get;}public AutosaveCoordinator Autosave{get;}
        public LocalMetagamePolicy Metagame{get;}public LocalCosmeticsPolicy CosmeticPolicy{get;}public LocalPostClearPolicy PostClear{get;}public IReadOnlyList<CalibratedEndlessProfile> CalibratedProfiles=>Sources.CalibratedProfiles;
        public RuntimeCompletionCatalog EndlessCompletion=>Sources.EndlessCompletion;public LocalEndlessPolicy EndlessPolicy{get;}
        public static Task<ProductionGameComposition> CreateAsync(RuntimeGameSources sources,string privateRoot,ILocalSessionClock clock,
            ILocalSaveDispatcher dispatcher,string appVersion,string buildId,CancellationToken cancellation)
        {
            if(sources==null||clock==null||dispatcher==null)throw new ArgumentNullException();
            if(string.IsNullOrWhiteSpace(appVersion)||string.IsNullOrWhiteSpace(buildId))throw new ArgumentException("RUNTIME_BUILD_DIAGNOSIS_REQUIRED");
            return Task.Run(()=>
            {
                cancellation.ThrowIfCancellationRequested();ICanonicalJson json=new CanonicalJsonAdapter();
                var corpus=sources.Catalog;var progress=sources.Progress;var profile=new SaveProfileIdentity(sources.Use==CatalogUse.Production?"stp-production":"fixture-production-composition",sources.Use==CatalogUse.Production?"player":"t",sources.Use);
                var rewardSources=new[]{RetainedCompletionRewardBinding.FromRuntimeWrapper(LocalPostClearPolicy.PolicyVersion,sources.GetFileBytes("CampaignCompletion.json"),json,sources.CampaignCompletion.Identity,corpus),RetainedCompletionRewardBinding.FromRuntimeWrapper(LocalPostClearPolicy.PolicyVersion,sources.GetFileBytes("EndlessCompletion.json"),json,sources.EndlessCompletion.Identity,corpus)};
                var retainedProfiles=sources.CalibratedProfiles.Select(p=>RetainedCalibratedEndlessBinding.FromDocuments(sources.GetFileBytes("EndlessCompletion.json"),sources.GetFileBytes("Profiles/"+p.Id+".json"),corpus,sources.EndlessCompletion.Identity,p.HashSha256,json)).ToArray();
                var retained=RetainedContractBindings.ForRuntimeWithCalibratedEndless(sources.Use,RetainedContractBindings.ForCosmetics(sources.Cosmetics.Identity,progress.Campaign.Identity,sources.GetFileBytes("Cosmetics.json"),sources.GetFileBytes("Campaign.json"),corpus,json),rewardSources,retainedProfiles);
                var codec=LocalSaveCodec.ForMetagame(json,profile,progress.CatalogSet,corpus,progress.Campaign,sources.CampaignCompletion,retained);
                var ledger=new SaveLedgerCodec(json);var policy=new LocalProgressPolicy(corpus,progress.Campaign,sources.CampaignCompletion,json,ledger);
                var metagame=new LocalMetagamePolicy(corpus,sources.CampaignCompletion.Identity,ledger);var cosmetics=new LocalCosmeticsPolicy(sources.Cosmetics,progress.Campaign,json,ledger);var postClear=new LocalPostClearPolicy(corpus,sources.CampaignCompletion.Identity,ledger);
                var endless=new LocalEndlessPolicy(corpus,new[]{sources.EndlessCompletion.GetEndlessValues()},json,ledger,sources.CalibratedProfiles);
                // Every byte/typed catalog/policy/retained binding above is checked before the first filesystem effect.
                cancellation.ThrowIfCancellationRequested();SaveFileSystem.ProvisionPrivateRoot(privateRoot);
                var files=new SaveFileSystem(privateRoot,profile,progress.CatalogSet.AggregateCatalog);SavedPublic240Composition.ValidateStoragePathLengths(files);
                var repository=new AtomicSaveRepository(files,codec,clock,appVersion,buildId);var persisted=new PersistedPuzzleSession(repository,corpus,policy,profile,progress.CatalogSet,clock,dispatcher);persisted.ConfigureEndless(endless);
                return new ProductionGameComposition(sources,profile,repository,persisted,new AutosaveCoordinator(persisted),metagame,cosmetics,postClear,endless);
            },cancellation);
        }
    }
}
