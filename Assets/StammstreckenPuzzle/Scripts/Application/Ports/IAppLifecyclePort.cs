namespace STP.Application.Ports
{
    /// <summary>
    /// Normativer Application-Port gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 6.
    /// Minimale Verantwortung: Foreground, Background, Suspend und Quit melden.
    /// Verbotene Verantwortung: Puzzlezeit selbst berechnen.
    /// Implementierendes Modul: <c>STP.Platform</c>.
    /// WP-021-Skelett ohne Mitglieder; fachliche Mitglieder folgen mit den zuständigen Work Packages.
    /// </summary>
    public interface IAppLifecyclePort
    {
    }
}
