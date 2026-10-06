namespace STP.Application.Ports
{
    /// <summary>
    /// Normativer Application-Port gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 6.
    /// Minimale Verantwortung: Reward- und Completiondefinitionen liefern.
    /// Verbotene Verantwortung: Rewardberechtigung oder Ledger mutieren.
    /// Implementierendes Modul: <c>STP.Infrastructure.Content</c>.
    /// WP-021-Skelett ohne Mitglieder; fachliche Mitglieder folgen mit den zuständigen Work Packages.
    /// </summary>
    public interface ICompletionCatalog
    {
    }
}
