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
    }
}
