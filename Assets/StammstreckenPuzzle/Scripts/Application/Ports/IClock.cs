namespace STP.Application.Ports
{
    /// <summary>
    /// Normativer Application-Port gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 6.
    /// Minimale Verantwortung: UTC für ausdrücklich freigegebene Policies und monotone aktive Zeit liefern.
    /// Verbotene Verantwortung: Wanduhr als Rätseltimer verwenden.
    /// Implementierendes Modul: <c>STP.Platform</c>.
    /// WP-021-Skelett ohne Mitglieder; fachliche Mitglieder folgen mit den zuständigen Work Packages.
    /// </summary>
    public interface IClock
    {
    }
}
