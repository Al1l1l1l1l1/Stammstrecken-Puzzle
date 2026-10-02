using STP.Application.Ports;

namespace STP.Audio
{
    /// <summary>
    /// WP-021-Skelett des Audio-Moduls gemäß ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 3
    /// (Audio-Cue-Mapping, AudioMixer und Lifecycle).
    /// Implementiert den <see cref="IAudioPort"/>-Port aus Abschnitt 6 ohne Fachlogik.
    /// </summary>
    public sealed class AudioModuleSkeleton : IAudioPort
    {
    }
}
