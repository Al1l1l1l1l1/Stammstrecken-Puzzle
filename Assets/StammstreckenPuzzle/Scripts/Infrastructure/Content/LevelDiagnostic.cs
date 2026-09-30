using System;
using System.Collections.Generic;

namespace STP.Infrastructure.Content
{
    /// <summary>Schweregrad einer Validierungsdiagnose.</summary>
    public enum DiagnosticSeverity : byte
    {
        /// <summary>Fehler; der Datensatz ist ungültig.</summary>
        Error = 0,

        /// <summary>Warnung; im <c>--strict</c>-Modus als Fehler behandelt.</summary>
        Warning = 1,
    }

    /// <summary>
    /// Eine Validierungsdiagnose mit stabilem Diagnosecode gemäß
    /// LEVEL_DATA_FORMAT.md Abschnitt 8 und CONTENT_PIPELINE.md Abschnitt 5.
    /// Diagnosen werden deterministisch sortiert (Code, dann Bezug, dann Meldung).
    /// </summary>
    public sealed class LevelDiagnostic
    {
        /// <summary>Stabiler Diagnosecode (zum Beispiel <c>LVL-GRID-SUM-MISMATCH</c>).</summary>
        public string Code { get; }

        /// <summary>Schweregrad.</summary>
        public DiagnosticSeverity Severity { get; }

        /// <summary>Fachlicher Bezug (zum Beispiel JSON-Pfad oder Koordinate); leer, wenn ohne Bezug.</summary>
        public string Subject { get; }

        /// <summary>Lesbare Einzelheiten.</summary>
        public string Message { get; }

        /// <summary>Erstellt eine Diagnose.</summary>
        public LevelDiagnostic(string code, DiagnosticSeverity severity, string subject, string message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Severity = severity;
            Subject = subject ?? string.Empty;
            Message = message ?? string.Empty;
        }

        /// <summary>Erstellt eine Fehlerdiagnose.</summary>
        public static LevelDiagnostic Error(string code, string subject, string message)
        {
            return new LevelDiagnostic(code, DiagnosticSeverity.Error, subject, message);
        }

        /// <summary>Erstellt eine Warnungsdiagnose.</summary>
        public static LevelDiagnostic Warning(string code, string subject, string message)
        {
            return new LevelDiagnostic(code, DiagnosticSeverity.Warning, subject, message);
        }

        /// <summary>Deterministische Ordnung: Code, dann Bezug, dann Meldung (jeweils ordinal).</summary>
        public static int CompareOrdinal(LevelDiagnostic? left, LevelDiagnostic? right)
        {
            if (ReferenceEquals(left, right))
            {
                return 0;
            }
            if (left is null)
            {
                return -1;
            }
            if (right is null)
            {
                return 1;
            }
            var byCode = string.CompareOrdinal(left.Code, right.Code);
            if (byCode != 0)
            {
                return byCode;
            }
            var bySubject = string.CompareOrdinal(left.Subject, right.Subject);
            if (bySubject != 0)
            {
                return bySubject;
            }
            return string.CompareOrdinal(left.Message, right.Message);
        }

        /// <summary>Sortiert Diagnosen deterministisch nach <see cref="CompareOrdinal"/>.</summary>
        public static List<LevelDiagnostic> Sorted(IEnumerable<LevelDiagnostic> diagnostics)
        {
            var list = new List<LevelDiagnostic>(diagnostics);
            list.Sort(CompareOrdinal);
            return list;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Subject.Length == 0 ? $"{Severity} {Code}: {Message}" : $"{Severity} {Code} [{Subject}]: {Message}";
        }
    }
}
