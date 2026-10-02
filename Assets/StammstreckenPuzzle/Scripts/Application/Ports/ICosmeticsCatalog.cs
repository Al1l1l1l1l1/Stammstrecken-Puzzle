namespace STP.Application.Ports
{
    /// <summary>
    /// Normativer Application-Port gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 6.
    /// Minimale Verantwortung: Itemstatus, Preis und Referenzen liefern.
    /// Verbotene Verantwortung: Ownership vergeben oder Saldo prüfen.
    /// Implementierendes Modul: <c>STP.Infrastructure.Content</c>.
    /// WP-021-Skelett ohne Mitglieder; fachliche Mitglieder folgen mit den zuständigen Work Packages.
    /// </summary>
    public interface ICosmeticsCatalog
    {
    }
}
