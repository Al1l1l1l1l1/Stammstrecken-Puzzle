using STP.Application.Ports;

namespace STP.Infrastructure.Persistence
{
    /// <summary>
    /// WP-008-Adapter-Skelett fuer den Persistenz-Port. Save-Serializer, atomare
    /// Dateien, Migrationen, Hashprofile, Ledgercheckpoint und Backup folgen mit
    /// WP-009. Dieses Skelett enthaelt keine Fachlogik und keinen Dateizugriff.
    /// </summary>
    public sealed class SaveRepositoryAdapter : ISaveRepository
    {
    }
}
