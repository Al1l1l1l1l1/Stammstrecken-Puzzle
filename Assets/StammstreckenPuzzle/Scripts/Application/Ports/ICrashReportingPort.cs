namespace STP.Application.Ports
{
    /// <summary>
    /// Normativer Application-Port gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 6.
    /// Minimale Verantwortung: freigegebene Keys, Logs und Exceptions senden.
    /// Verbotene Verantwortung: Save-Inhalte hochladen oder vor Capability aktivieren.
    /// Implementierendes Modul: <c>STP.MobileServices.Google</c>.
    /// WP-021-Skelett ohne Mitglieder; fachliche Mitglieder folgen mit den zuständigen Work Packages.
    /// </summary>
    public interface ICrashReportingPort
    {
    }
}
