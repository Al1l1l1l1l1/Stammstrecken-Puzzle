using STP.Application.Ports;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// WP-021-Skelett des Content-Moduls gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 3
    /// (JSON-Parsing, Schema-/Semantikadapter, Level-/Katalogports und Addressable-Mapping).
    /// Implementiert die vier normativen Katalogports aus Abschnitt 6 ohne Fachlogik:
    /// kein Dateizugriff, kein Kataloginhalt, keine Spielerzustandsänderung.
    /// </summary>
    public sealed class ContentModuleSkeleton : ILevelCatalog, ICampaignCatalog, ICompletionCatalog, ICosmeticsCatalog
    {
    }
}
