using STP.Application.Ports;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// WP-008-Adapter-Skelett fuer die Content-Ports. Mit WP-013 sind der
    /// Level-v2-Parser, die JCS-Hashvertraege und die proof-v1-Bindungen in
    /// dieser Assembly implementiert; die Runtime-Katalogports
    /// (ILevelCatalog, ICampaignCatalog, ICompletionCatalog, ICosmeticsCatalog)
    /// bleiben ausdruecklich Skelette, Addressable-Mapping folgt in spaeteren
    /// Work Packages. Dieses Skelett enthaelt keine Fachlogik und keinen
    /// Dateizugriff.
    /// </summary>
    public sealed class ContentCatalogAdapter : ILevelCatalog, ICampaignCatalog, ICompletionCatalog, ICosmeticsCatalog
    {
    }
}
