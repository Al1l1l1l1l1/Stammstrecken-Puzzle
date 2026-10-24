using System;

namespace STP.Application
{
    public sealed partial class SavedPublic240ReviewSession
    {
        /// <summary>Uses already locked runtime sources. The factory grants no source approval, import or migration authority.</summary>
        public static SavedPublic240ReviewSession ForLockedRuntime(PersistedPuzzleSession persisted,AutosaveCoordinator autosave,
            IPublicPuzzleCatalog catalog,ILocalSessionClock clock,ILocalSaveDispatcher dispatcher,SaveProfileIdentity expectedProfile)
        {
            if(expectedProfile==null||(expectedProfile.Use!=CatalogUse.Production&&expectedProfile.Use!=CatalogUse.ContractTest))
                throw new ArgumentException("LOCKED_RUNTIME_PROFILE_REQUIRED");
            if(catalog==null)throw new ArgumentNullException(nameof(catalog));
            if(catalog.Puzzles.Count!=240)throw new ArgumentException("LOCKED_RUNTIME_CORPUS_REQUIRED");
            var approval=expectedProfile.Use==CatalogUse.Production?CatalogApprovalStatus.ProductApproved:CatalogApprovalStatus.FixtureOnly;
            return new SavedPublic240ReviewSession(persisted,autosave,catalog,clock,dispatcher,expectedProfile,approval);
        }
        private bool SupportsMetagame=>expectedProfile.Use==CatalogUse.InternalPreview||expectedProfile.Use==CatalogUse.Production||expectedProfile.Use==CatalogUse.ContractTest;
    }
}
