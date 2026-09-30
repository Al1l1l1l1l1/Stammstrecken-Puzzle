namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Ressourcengrenzen des strikten JSON-Parsers für untrusted Input gemäß
    /// LEVEL_DATA_FORMAT.md Abschnitt 11: Dateigröße, Verschachtelungstiefe,
    /// Stringlänge und Arraylänge (technische Begrenzung, unter anderem der
    /// Pfad- und Zähllisten) werden fest begrenzt.
    /// </summary>
    public sealed class JsonParseLimits
    {
        /// <summary>Standardgrenzen für Level- und Proofdokumente.</summary>
        public static readonly JsonParseLimits Default = new JsonParseLimits();

        /// <summary>Erstellt Grenzen; ohne Angaben gelten die Standardwerte.</summary>
        public JsonParseLimits(int maxBytes = 1_048_576, int maxDepth = 64, int maxStringLength = 16_384, int maxArrayElements = 10_000)
        {
            MaxBytes = maxBytes;
            MaxDepth = maxDepth;
            MaxStringLength = maxStringLength;
            MaxArrayElements = maxArrayElements;
        }

        /// <summary>Maximale Eingabegröße in Bytes (Standard 1 MiB).</summary>
        public int MaxBytes { get; }

        /// <summary>Maximale Verschachtelungstiefe von Objekten und Arrays (Standard 64).</summary>
        public int MaxDepth { get; }

        /// <summary>Maximale Länge einer einzelnen JSON-Zeichenkette in Zeichen (Standard 16.384).</summary>
        public int MaxStringLength { get; }

        /// <summary>Maximale Elementzahl eines einzelnen JSON-Arrays (Standard 10.000).</summary>
        public int MaxArrayElements { get; }
    }
}
