using System;
using System.Collections.Generic;

namespace STP.Puzzle.Domain
{
    /// <summary>
    /// Ergebnis der Command-Verarbeitung gemäß ADR-005: entweder neuer
    /// Snapshot plus geordnete Domainereignisse oder unveränderte Ablehnung mit
    /// sortierten Diagnosecodes. Bei einem No-op wird der unveränderte Zustand
    /// ohne Ereignisse zurückgegeben.
    /// </summary>
    public sealed class CommandResult
    {
        /// <summary>Wahr, wenn der Command angenommen wurde (inklusive No-op).</summary>
        public bool Accepted { get; }

        /// <summary>Resultierender Snapshot (bei Ablehnung der unveränderte Eingangssnapshot).</summary>
        public PuzzleSessionState State { get; }

        /// <summary>Geordnete Domainereignisse; leer bei Ablehnung oder No-op.</summary>
        public IReadOnlyList<DomainEvent> Events { get; }

        /// <summary>Sortierte Diagnosecodes einer Ablehnung; leer bei Annahme.</summary>
        public IReadOnlyList<string> Diagnostics { get; }

        private CommandResult(
            bool accepted,
            PuzzleSessionState state,
            IReadOnlyList<DomainEvent> events,
            IReadOnlyList<string> diagnostics)
        {
            Accepted = accepted;
            State = state;
            Events = events;
            Diagnostics = diagnostics;
        }

        /// <summary>Annahme mit neuem Snapshot und geordneten Ereignissen.</summary>
        public static CommandResult AcceptedWith(PuzzleSessionState state, IReadOnlyList<DomainEvent> events)
        {
            return new CommandResult(true, state, events, Array.Empty<string>());
        }

        /// <summary>Ablehnung mit einem Diagnosecode; der Zustand bleibt unverändert.</summary>
        public static CommandResult Rejected(PuzzleSessionState state, string diagnosticCode)
        {
            return new CommandResult(false, state, Array.Empty<DomainEvent>(), new[] { diagnosticCode });
        }
    }
}
