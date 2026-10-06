using STP.Application.Ports;

namespace STP.Platform
{
    /// <summary>
    /// WP-021-Skelett des Plattform-Moduls gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 3
    /// (App-Lifecycle, Netzwerkstatus, Safe Area, Haptik und Plattforminformationen).
    /// Implementiert die vier normativen Plattform-Ports aus Abschnitt 6 ohne Fachlogik.
    /// </summary>
    public sealed class PlatformModuleSkeleton : IClock, IAppLifecyclePort, INetworkStatusPort, IHapticsPort
    {
    }
}
