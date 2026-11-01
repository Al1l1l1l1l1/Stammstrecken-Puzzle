using System.Threading.Tasks;

namespace STP.Application.Ports
{
    /// <summary>
    /// Normativer Application-Port gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 6.
    /// Minimale Verantwortung: Status aktualisieren, Optionen zeigen, explizite Capabilities liefern.
    /// Verbotene Verantwortung: Produktnavigation blockieren oder unklare Zustände freigeben.
    /// Implementierendes Modul: <c>STP.MobileServices.Google</c>.
    /// WP-021-Skelett ohne Mitglieder; fachliche Mitglieder folgen mit den zuständigen Work Packages.
    /// </summary>
    public interface IConsentPort
    {
        /// <summary>Effective capabilities always start false.</summary>
        MobileCapabilities Capabilities { get; }
        /// <summary>Runs the current UMP start flow, producing a candidate to persist.</summary>
        Task<MobileResult<ConsentSnapshot>> RefreshAsync(MobileOperation operation);
        /// <summary>Immediately gates ads off, then presents required privacy options.</summary>
        Task<MobileResult<ConsentSnapshot>> ShowPrivacyOptionsAsync(MobileOperation operation);
        /// <summary>Applies only the current candidate after its durable Application decision commit.</summary>
        bool ConfirmDecisionPersisted(ConsentSnapshot snapshot, long committedGeneration);
        /// <summary>Immediately gates off and invalidates pending/loaded ads and stale callbacks.</summary>
        void Revoke();
    }
}
