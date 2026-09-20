using UnityEngine;

namespace STP.Bootstrap
{
    /// <summary>
    /// WP-008-Szenenentry der dedizierten QA-Szene. Einziger dokumentierter
    /// Entry-Installer dieser Szene; er wird von Bootstrap gespeist und baut den
    /// Composition Graph genau einmal auf. MonoBehaviour dient hier ausschliesslich
    /// als Unity-Lifecycle-Adapter und enthaelt keine fachliche Entscheidungslogik
    /// (MODULE_BOUNDARIES.md Abschnitt 7).
    /// </summary>
    public sealed class BootstrapInstaller : MonoBehaviour
    {
        /// <summary>
        /// Der in dieser Szene aufgebaute Composition Graph. Vor Awake null.
        /// </summary>
        public BootstrapCompositionResult? Composition { get; private set; }

        private void Awake()
        {
            Composition = BootstrapComposition.Compose();
        }
    }
}
