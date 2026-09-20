using STP.Application.Ports;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// WP-008-Adapter-Skelett fuer die Content-Ports. JSON-Parsing, Schema-/
    /// Semantikadapter, Level-/Katalogzugriff und Addressable-Mapping folgen mit
    /// WP-009. Dieses Skelett enthaelt keine Fachlogik und keinen Dateizugriff.
    /// </summary>
    public sealed class ContentCatalogAdapter : ILevelCatalog, ICampaignCatalog, ICompletionCatalog, ICosmeticsCatalog
    {
    }
}
