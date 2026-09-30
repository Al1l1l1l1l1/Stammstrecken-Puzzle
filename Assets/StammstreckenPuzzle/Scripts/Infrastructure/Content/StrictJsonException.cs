using System;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Verletzung der strikten JSON-Eingangsregeln (untrusted Input gemäß
    /// LEVEL_DATA_FORMAT.md Abschnitt 11). Trägt einen stabilen Diagnosecode
    /// der Familie <c>LVL-PARSE-*</c>.
    /// </summary>
    public sealed class StrictJsonException : Exception
    {
        /// <summary>Stabiler Diagnosecode (<c>LVL-PARSE-*</c>).</summary>
        public string Code { get; }

        /// <summary>Erstellt die Ausnahme mit Diagnosecode und Meldung.</summary>
        public StrictJsonException(string code, string message)
            : base(message)
        {
            Code = code;
        }
    }
}
