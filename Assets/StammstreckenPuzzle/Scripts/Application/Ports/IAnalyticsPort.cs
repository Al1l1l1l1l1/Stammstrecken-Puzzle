namespace STP.Application.Ports
{
    /// <summary>
    /// Normativer Application-Port gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 6.
    /// Minimale Verantwortung: freigegebene, schema-konforme Events senden.
    /// Verbotene Verantwortung: Gameplay-Wahrheit speichern oder vor Capability senden.
    /// Implementierendes Modul: <c>STP.MobileServices.Google</c>.
    /// WP-021-Skelett ohne Mitglieder; fachliche Mitglieder folgen mit den zuständigen Work Packages.
    /// </summary>
    public interface IAnalyticsPort
    {
    }
}
