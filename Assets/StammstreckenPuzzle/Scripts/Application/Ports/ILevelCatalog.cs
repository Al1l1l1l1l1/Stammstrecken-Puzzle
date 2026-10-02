namespace STP.Application.Ports
{
    /// <summary>
    /// Normativer Application-Port gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 6.
    /// Minimale Verantwortung: Definition nach stabiler ID liefern und Katalogrevision melden.
    /// Verbotene Verantwortung: Spielerzustand verändern.
    /// Implementierendes Modul: <c>STP.Infrastructure.Content</c>.
    /// WP-021-Skelett ohne Mitglieder; fachliche Mitglieder folgen mit den zuständigen Work Packages.
    /// </summary>
    public interface ILevelCatalog
    {
    }
}
