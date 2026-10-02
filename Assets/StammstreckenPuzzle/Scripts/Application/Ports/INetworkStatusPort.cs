namespace STP.Application.Ports
{
    /// <summary>
    /// Normativer Application-Port gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 6.
    /// Minimale Verantwortung: grobe Erreichbarkeit als Hinweis liefern.
    /// Verbotene Verantwortung: Erfolg eines Dienstaufrufs garantieren.
    /// Implementierendes Modul: <c>STP.Platform</c>.
    /// WP-021-Skelett ohne Mitglieder; fachliche Mitglieder folgen mit den zuständigen Work Packages.
    /// </summary>
    public interface INetworkStatusPort
    {
    }
}
