namespace STP.Application.Ports
{
    /// <summary>
    /// Normativer Application-Port gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 6.
    /// Minimale Verantwortung: Produkte, Kaufbeleg, lokale Prüfung, Acknowledge/Finish, Pending, Restore und Revocation abbilden.
    /// Verbotene Verantwortung: Entitlement direkt vergeben.
    /// Implementierendes Modul: <c>STP.MobileServices.Store</c>.
    /// WP-021-Skelett ohne Mitglieder; fachliche Mitglieder folgen mit den zuständigen Work Packages.
    /// </summary>
    public interface IPurchasePort
    {
    }
}
