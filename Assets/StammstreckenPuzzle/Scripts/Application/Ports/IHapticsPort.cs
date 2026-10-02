namespace STP.Application.Ports
{
    /// <summary>
    /// Normativer Application-Port gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 6.
    /// Minimale Verantwortung: semantische Haptik-Cues ausführen.
    /// Verbotene Verantwortung: Gameplayfeedback als Richtigkeitsurteil erfinden.
    /// Implementierendes Modul: <c>STP.Platform</c>.
    /// WP-021-Skelett ohne Mitglieder; fachliche Mitglieder folgen mit den zuständigen Work Packages.
    /// </summary>
    public interface IHapticsPort
    {
    }
}
