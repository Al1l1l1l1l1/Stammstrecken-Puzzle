using System;
using System.Linq;

namespace STP.Application
{
    /// <summary>Source keys for the existing closed preferences model and ADR011 mixer groups.</summary>
    public static class LocalSettingsPolicy
    {
        /// <summary>Current reduced-motion/ride-skip preference source, not an accessibility acceptance.</summary>
        public const string AccessibilityRevision = "accessibility-launch-v1";
        /// <summary>Copies preferences without altering any progress, provider, hint or inventory fact.</summary>
        public static LocalProfileState Apply(LocalProfileState state, LocalSettings settings)
        {
            if(state==null||settings==null)throw new ArgumentNullException();
            LocalModel.Require(settings.Locale==null||settings.Locale=="de-DE","SETTINGS_LOCALE_UNSUPPORTED");
            LocalModel.Require(settings.AudioGroups.All(a=>new[]{"Master","Music","SFX","Ambience","UI"}.Contains(a.GroupId)),"SETTINGS_AUDIO_GROUP_UNSUPPORTED");
            LocalModel.Require(settings.Accessibility==null||settings.Accessibility.ContractRevision==AccessibilityRevision,"SETTINGS_ACCESSIBILITY_UNSUPPORTED");
            return LocalProfileState.Create(state.Profile,state.Levels,state.Endless,state.LedgerCheckpoint,state.EconomyJournal,state.Inventory,state.CosmeticClaimReservations,state.SeasonProgressCache,state.Drafts,state.RewardClaims,state.DailyStatus,state.Entitlements,state.PendingOperations,settings,state.DataLifecycle,state.HintEntitlement).WithMetagame(state.Metagame);
        }
    }
}
