using STP.Application.Ports;

namespace STP.Infrastructure.Persistence
{
    /// <summary>
    /// WP-021-Skelett des Persistenz-Moduls gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 3
    /// (Save-Serializer, atomare Dateien, Migrationen, Hashprofile, Ledgercheckpoint und Backup).
    /// Implementiert den <see cref="ISaveRepository"/>-Port aus Abschnitt 6 ohne Fachlogik:
    /// kein Dateizugriff, kein Snapshot, keine Fortschrittsberechnung.
    /// </summary>
    public sealed class PersistenceModuleSkeleton : ISaveRepository
    {
    }
}
