using System;
using System.Threading.Tasks;

namespace STP.Application.Ports
{
    /// <summary>
    /// Normativer Application-Port gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 6.
    /// Minimale Verantwortung: Verfügbarkeit, Load, Show und typisiertes Ergebnis.
    /// Verbotene Verantwortung: Werbefrequenz oder Zeitpunkt entscheiden.
    /// Implementierendes Modul: <c>STP.MobileServices.Google</c>.
    /// WP-021-Skelett ohne Mitglieder; fachliche Mitglieder folgen mit den zuständigen Work Packages.
    /// </summary>
    public interface IAdsPort
    {
        /// <summary>Recovery-capable reward evidence; Application deduplicates and grants atomically.</summary>
        event Action<AdRewardEvidence>? RewardEarned;
        /// <summary>Loads only after effective consent; no policy or automatic retry belongs here.</summary>
        Task<MobileResult<LoadedAd>> LoadAsync(MobileAdKind kind, MobileOperation operation);
        /// <summary>Shows a once-only handle; rewarded ads require the already durable reservation.</summary>
        Task<MobileResult<AdPresentation>> ShowAsync(LoadedAd ad, AdRewardReservation? reservation, MobileOperation operation);
        /// <summary>Destroys cached loads and prevents future loads/shows until consent reapplies.</summary>
        void DiscardLoadedAds();
    }
}
