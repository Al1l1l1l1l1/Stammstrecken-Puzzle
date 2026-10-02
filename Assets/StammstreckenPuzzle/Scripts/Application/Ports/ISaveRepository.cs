namespace STP.Application.Ports
{
    /// <summary>
    /// Normativer Application-Port gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 6.
    /// Minimale Verantwortung: Snapshot laden, atomar speichern, Backupstatus melden.
    /// Verbotene Verantwortung: Fortschritt berechnen.
    /// Implementierendes Modul: <c>STP.Infrastructure.Persistence</c>.
    /// WP-021-Skelett ohne Mitglieder; fachliche Mitglieder folgen mit den zuständigen Work Packages.
    /// </summary>
    public interface ISaveRepository
    {
    }
}
