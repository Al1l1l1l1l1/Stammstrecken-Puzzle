using UnityEngine;

namespace STP.Bootstrap
{
    /// <summary>
    /// Einziger Entry-Installer der dedizierten QA-Szene
    /// <c>Assets/StammstreckenPuzzle/Scenes/QA/BootstrapComposition.unity</c>
    /// (ARCHITECTURE/MODULE_BOUNDARIES.md Abschnitt 7). MonoBehaviour ausschließlich als
    /// Unity-Lifecycle-Adapter ohne fachliche Entscheidungslogik: Er startet genau einmal
    /// die Composition Root und legt das Ergebnis für den Composition-Smoke offen.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BootstrapInstaller : MonoBehaviour
    {
        /// <summary>Das beim Szenenstart erstellte Composition-Ergebnis (nach Awake nicht null).</summary>
        public BootstrapCompositionResult Result { get; private set; } = null!;

        private void Awake()
        {
            this.Result = BootstrapComposition.Compose();
        }
    }
}
