using System;
using System.Collections.Generic;
using STP.Application.Ports;

namespace STP.Application
{
    /// <summary>
    /// Fail-closed Portbindung der Application gemäß ARCHITECTURE/MODULE_BOUNDARIES.md
    /// Abschnitte 6 und 7: Jeder normative Port ist genau einmal gebunden, kein Binding ist
    /// null und der Zugriff auf einen ungebundenen Port schlägt fehl, statt einen stillen
    /// Null-/Fallback-Port zu liefern. Die zehn nicht-providernahen Ports werden mit
    /// <see cref="FromPorts"/> gebunden; die fünf Providerports folgen getrennt und erst
    /// danach mit <see cref="BindProviders"/> (Abschnitt 7: erlaubte externe Adapter zuletzt).
    /// Ein zweiter Aufruf von <see cref="BindProviders"/> ist eine unzulässige Doppelbindung
    /// und schlägt fehl. Dass ein Modul mehrere Ports implementiert (zum Beispiel die vier
    /// Katalogports des Content-Moduls), ist die dokumentierte Bindung und keine Doppelbindung.
    /// Enthält keine Use Cases und keine Fachlogik.
    /// </summary>
    public sealed class ApplicationComposition
    {
        private readonly Dictionary<Type, object> bindings;
        private bool providersBound;

        private ApplicationComposition(Dictionary<Type, object> bindings)
        {
            this.bindings = bindings;
        }

        /// <summary>Alle Bindungen nach Porttyp (schreibgeschützt, für den Composition-Smoke).</summary>
        public IReadOnlyDictionary<Type, object> Bindings => this.bindings;

        /// <summary>
        /// Erstellt die Portbindung der zehn nicht-providernahen Ports. Jeder Port muss
        /// nicht-null sein.
        /// </summary>
        /// <exception cref="ArgumentNullException">Ein Port ist null.</exception>
        public static ApplicationComposition FromPorts(
            ISaveRepository saves,
            ILevelCatalog levels,
            ICampaignCatalog campaigns,
            ICompletionCatalog completions,
            ICosmeticsCatalog cosmetics,
            IClock clock,
            IAudioPort audio,
            IAppLifecyclePort lifecycle,
            INetworkStatusPort network,
            IHapticsPort haptics)
        {
            var map = new Dictionary<Type, object>
            {
                [typeof(ISaveRepository)] = Require(saves, nameof(saves)),
                [typeof(ILevelCatalog)] = Require(levels, nameof(levels)),
                [typeof(ICampaignCatalog)] = Require(campaigns, nameof(campaigns)),
                [typeof(ICompletionCatalog)] = Require(completions, nameof(completions)),
                [typeof(ICosmeticsCatalog)] = Require(cosmetics, nameof(cosmetics)),
                [typeof(IClock)] = Require(clock, nameof(clock)),
                [typeof(IAudioPort)] = Require(audio, nameof(audio)),
                [typeof(IAppLifecyclePort)] = Require(lifecycle, nameof(lifecycle)),
                [typeof(INetworkStatusPort)] = Require(network, nameof(network)),
                [typeof(IHapticsPort)] = Require(haptics, nameof(haptics)),
            };

            return new ApplicationComposition(map);
        }

        /// <summary>
        /// Bindet die fünf Providerports (erlaubte externe Adapter) genau einmal.
        /// </summary>
        /// <exception cref="ArgumentNullException">Ein Providerport ist null.</exception>
        /// <exception cref="InvalidOperationException">Doppelbindung: Providerports sind bereits gebunden.</exception>
        public void BindProviders(
            IAdsPort ads,
            IPurchasePort purchases,
            IConsentPort consent,
            IAnalyticsPort analytics,
            ICrashReportingPort crashes)
        {
            if (this.providersBound)
            {
                throw new InvalidOperationException(
                    "Unzulässige Doppelbindung: die Providerports wurden bereits gebunden.");
            }

            this.bindings[typeof(IAdsPort)] = Require(ads, nameof(ads));
            this.bindings[typeof(IPurchasePort)] = Require(purchases, nameof(purchases));
            this.bindings[typeof(IConsentPort)] = Require(consent, nameof(consent));
            this.bindings[typeof(IAnalyticsPort)] = Require(analytics, nameof(analytics));
            this.bindings[typeof(ICrashReportingPort)] = Require(crashes, nameof(crashes));
            this.providersBound = true;
        }

        /// <summary>Liefert das Binding für den angefragten Port oder schlägt fail-closed fehl.</summary>
        /// <exception cref="InvalidOperationException">Der Port ist nicht gebunden.</exception>
        public TPort Get<TPort>() where TPort : class
        {
            if (this.bindings.TryGetValue(typeof(TPort), out var port))
            {
                return (TPort)port;
            }

            throw new InvalidOperationException(
                $"Port {typeof(TPort).Name} ist nicht gebunden; es gibt keinen stillen Fallback-Port.");
        }

        private static TPort Require<TPort>(TPort port, string name) where TPort : class
        {
            return port ?? throw new ArgumentNullException(name, $"Port {name} darf nicht null sein.");
        }
    }
}
