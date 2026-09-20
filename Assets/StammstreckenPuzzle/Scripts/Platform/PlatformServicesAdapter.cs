using STP.Application.Ports;

namespace STP.Platform
{
    /// <summary>
    /// WP-008-Adapter-Skelett fuer die Plattform-Ports. App-Lifecycle,
    /// Netzwerkstatus, Safe Area, Haptik und Plattforminformationen folgen mit
    /// WP-009. Dieses Skelett enthaelt keine Fachlogik.
    /// </summary>
    public sealed class PlatformServicesAdapter : IClock, IAppLifecyclePort, INetworkStatusPort, IHapticsPort
    {
    }
}
