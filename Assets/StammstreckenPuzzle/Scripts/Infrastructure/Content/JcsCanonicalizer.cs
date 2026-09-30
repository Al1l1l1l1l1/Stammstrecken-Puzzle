using System;
using System.Globalization;
using System.Text;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// RFC-8785/JCS-Kanonisierung gemäß LEVEL_DATA_FORMAT.md Abschnitt 6:
    /// UTF-8 ohne BOM und ohne Abschlussnewline unter I-JSON-Regeln. Objekt-
    /// schlüssel werden nach UTF-16-Codeeinheiten sortiert (ordinal),
    /// Zeichenketten werden minimal maskiert (nur Anführungszeichen,
    /// Umkehrstrich und Steuerzeichen unterhalb 0x20), Ganzzahlen werden
    /// dezimal ausgegeben. Fließkommazahlen existieren im Dokumentmodell
    /// nicht; ungültige Surrogate werden abgelehnt.
    /// </summary>
    public static class JcsCanonicalizer
    {
        private const char MaxControlCharacter = (char)0x1F;
        private const char HighSurrogateMin = (char)0xD800;
        private const char HighSurrogateMax = (char)0xDBFF;
        private const char LowSurrogateMin = (char)0xDC00;
        private const char LowSurrogateMax = (char)0xDFFF;
        private const string HexDigits = "0123456789abcdef";

        /// <summary>Liefert die kanonischen RFC-8785-Bytes (UTF-8 ohne BOM, ohne Abschlussnewline).</summary>
        public static byte[] ToUtf8Bytes(JsonValue value)
        {
            return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
                .GetBytes(ToCanonicalString(value));
        }

        /// <summary>Liefert die kanonische RFC-8785-Zeichenform.</summary>
        public static string ToCanonicalString(JsonValue value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            var builder = new StringBuilder();
            Append(value, builder);
            return builder.ToString();
        }

        private static void Append(JsonValue value, StringBuilder builder)
        {
            switch (value)
            {
                case JsonValue.Null _:
                    builder.Append("null");
                    return;
                case JsonValue.Boolean boolean:
                    builder.Append(boolean.Value ? "true" : "false");
                    return;
                case JsonValue.Integer integer:
                    builder.Append(integer.Value.ToString(CultureInfo.InvariantCulture));
                    return;
                case JsonValue.String text:
                    AppendString(text.Value, builder);
                    return;
                case JsonValue.Array array:
                    builder.Append('[');
                    for (var i = 0; i < array.Items.Count; i++)
                    {
                        if (i > 0)
                        {
                            builder.Append(',');
                        }
                        Append(array.Items[i], builder);
                    }
                    builder.Append(']');
                    return;
                case JsonValue.Object obj:
                    var keys = new string[obj.Properties.Count];
                    for (var i = 0; i < keys.Length; i++)
                    {
                        keys[i] = obj.Properties[i].Key;
                    }
                    Array.Sort(keys, StringComparer.Ordinal);
                    builder.Append('{');
                    for (var i = 0; i < keys.Length; i++)
                    {
                        if (i > 0)
                        {
                            builder.Append(',');
                        }
                        AppendString(keys[i], builder);
                        builder.Append(':');
                        obj.TryGet(keys[i], out var member);
                        Append(member, builder);
                    }
                    builder.Append('}');
                    return;
                default:
                    throw new ArgumentException($"Nicht unterstützter JSON-Wert: {value.GetType().Name}.", nameof(value));
            }
        }

        private static void AppendString(string value, StringBuilder builder)
        {
            ValidateSurrogates(value);
            builder.Append('"');
            foreach (var c in value)
            {
                switch (c)
                {
                    case '"': builder.Append("\\\""); break;
                    case '\\': builder.Append("\\\\"); break;
                    case '\b': builder.Append("\\b"); break;
                    case '\t': builder.Append("\\t"); break;
                    case '\n': builder.Append("\\n"); break;
                    case '\f': builder.Append("\\f"); break;
                    case '\r': builder.Append("\\r"); break;
                    default:
                        if (c <= MaxControlCharacter)
                        {
                            builder.Append("\\u00");
                            builder.Append(HexDigits[(c >> 4) & 0xF]);
                            builder.Append(HexDigits[c & 0xF]);
                        }
                        else
                        {
                            builder.Append(c);
                        }
                        break;
                }
            }
            builder.Append('"');
        }

        private static void ValidateSurrogates(string value)
        {
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (c >= HighSurrogateMin && c <= HighSurrogateMax)
                {
                    if (i + 1 >= value.Length)
                    {
                        throw new ArgumentException("Ungepaartes High-Surrogate ist nicht I-JSON-konform.", nameof(value));
                    }
                    var next = value[i + 1];
                    if (next < LowSurrogateMin || next > LowSurrogateMax)
                    {
                        throw new ArgumentException("Ungepaartes High-Surrogate ist nicht I-JSON-konform.", nameof(value));
                    }
                    i++;
                }
                else if (c >= LowSurrogateMin && c <= LowSurrogateMax)
                {
                    throw new ArgumentException("Ungepaartes Low-Surrogate ist nicht I-JSON-konform.", nameof(value));
                }
            }
        }
    }
}
