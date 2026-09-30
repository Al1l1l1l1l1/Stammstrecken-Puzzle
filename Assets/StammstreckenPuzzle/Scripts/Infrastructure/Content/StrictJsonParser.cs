using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Strikter UTF-8-JSON-Parser für lokale Dateien als untrusted Input gemäß
    /// LEVEL_DATA_FORMAT.md Abschnitt 11. Er erzwingt die Ressourcengrenzen aus
    /// <see cref="JsonParseLimits"/>, lehnt doppelte Schlüssel, Fließkomma- und
    /// Exponententokens (Nicht-I-JSON), ungültige Surrogate, Steuerzeichen,
    /// BOM und polymorphe Typmetadaten (<c>$type</c>) ab. Eine polymorphe
    /// Typnamen- oder automatische Typkonstruktion existiert in diesem Parser
    /// nicht; sie ist damit strukturell deaktiviert. Zahlentokens werden nur
    /// als Ganzzahlen im interoperablen sicheren Bereich (Betrag ≤ 2^53-1)
    /// akzeptiert. Fehler tragen stabile Codes der Familie <c>LVL-PARSE-*</c>.
    /// </summary>
    public static class StrictJsonParser
    {
        private const char ByteOrderMark = (char)0xFEFF;
        private const char MaxControlCharacter = (char)0x1F;
        private const char HighSurrogateMin = (char)0xD800;
        private const char HighSurrogateMax = (char)0xDBFF;
        private const char LowSurrogateMin = (char)0xDC00;
        private const char LowSurrogateMax = (char)0xDFFF;

        /// <summary>
        /// Parst ein UTF-8-JSON-Dokument vollständig. Jede Verletzung der
        /// Eingangsregeln führt fail-closed zu einer
        /// <see cref="StrictJsonException"/> mit stabilem Diagnosecode.
        /// </summary>
        public static JsonValue ParseUtf8(byte[] bytes, JsonParseLimits limits)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            if (limits is null)
            {
                throw new ArgumentNullException(nameof(limits));
            }
            if (bytes.Length == 0)
            {
                throw new StrictJsonException("LVL-PARSE-EMPTY", "Die Eingabe ist leer.");
            }
            if (bytes.Length > limits.MaxBytes)
            {
                throw new StrictJsonException("LVL-PARSE-RESOURCE-BYTES", $"Die Eingabe überschreitet das Größenlimit von {limits.MaxBytes} Bytes.");
            }
            string text;
            try
            {
                text = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true).GetString(bytes);
            }
            catch (DecoderFallbackException ex)
            {
                throw new StrictJsonException("LVL-PARSE-ENCODING", "Ungültige UTF-8-Sequenz: " + ex.Message);
            }
            if (text.Length > 0 && text[0] == ByteOrderMark)
            {
                throw new StrictJsonException("LVL-PARSE-BOM", "Eine Bytereihenfolge-Markierung (BOM) ist nicht zulässig.");
            }
            var reader = new Reader(text, limits);
            var value = reader.ParseValue(0);
            reader.SkipWhitespace();
            if (!reader.AtEnd)
            {
                throw new StrictJsonException("LVL-PARSE-TRAILING", "Daten nach dem Ende des JSON-Dokuments.");
            }
            return value;
        }

        private sealed class Reader
        {
            private readonly string _text;
            private readonly JsonParseLimits _limits;
            private int _index;

            internal Reader(string text, JsonParseLimits limits)
            {
                _text = text;
                _limits = limits;
            }

            internal bool AtEnd => _index >= _text.Length;

            internal void SkipWhitespace()
            {
                while (_index < _text.Length && (_text[_index] == ' ' || _text[_index] == '\t' || _text[_index] == '\n' || _text[_index] == '\r'))
                {
                    _index++;
                }
            }

            internal JsonValue ParseValue(int depth)
            {
                if (depth > _limits.MaxDepth)
                {
                    throw new StrictJsonException("LVL-PARSE-RESOURCE-DEPTH", $"Die Verschachtelungstiefe überschreitet das Limit von {_limits.MaxDepth}.");
                }
                SkipWhitespace();
                if (AtEnd)
                {
                    throw new StrictJsonException("LVL-PARSE-SYNTAX", "Unerwartetes Ende der Eingabe.");
                }
                var c = _text[_index];
                if (c == '"')
                {
                    return new JsonValue.String(ParseString());
                }
                if (c == '{')
                {
                    return ParseObject(depth);
                }
                if (c == '[')
                {
                    return ParseArray(depth);
                }
                if (c == '-' || (c >= '0' && c <= '9'))
                {
                    return ParseNumber();
                }
                if (MatchLiteral("true"))
                {
                    return JsonValue.Boolean.True;
                }
                if (MatchLiteral("false"))
                {
                    return JsonValue.Boolean.False;
                }
                if (MatchLiteral("null"))
                {
                    return JsonValue.Null.Instance;
                }
                throw new StrictJsonException("LVL-PARSE-SYNTAX", $"Unerwartetes Token an Position {_index}.");
            }

            private bool MatchLiteral(string literal)
            {
                if (string.CompareOrdinal(_text, _index, literal, 0, literal.Length) != 0)
                {
                    return false;
                }
                _index += literal.Length;
                return true;
            }

            private string ParseString()
            {
                _index++;
                var builder = new StringBuilder();
                while (true)
                {
                    if (_index >= _text.Length)
                    {
                        throw new StrictJsonException("LVL-PARSE-SYNTAX", "Nicht abgeschlossene Zeichenkette.");
                    }
                    var c = _text[_index];
                    if (c == '"')
                    {
                        _index++;
                        break;
                    }
                    if (c == '\\')
                    {
                        _index++;
                        if (_index >= _text.Length)
                        {
                            throw new StrictJsonException("LVL-PARSE-ESCAPE", "Nicht abgeschlossene Escapesequenz.");
                        }
                        var esc = _text[_index];
                        switch (esc)
                        {
                            case '"': builder.Append('"'); _index++; break;
                            case '\\': builder.Append('\\'); _index++; break;
                            case '/': builder.Append('/'); _index++; break;
                            case 'b': builder.Append('\b'); _index++; break;
                            case 'f': builder.Append('\f'); _index++; break;
                            case 'n': builder.Append('\n'); _index++; break;
                            case 'r': builder.Append('\r'); _index++; break;
                            case 't': builder.Append('\t'); _index++; break;
                            case 'u':
                                builder.Append(ParseUnicodeEscape());
                                break;
                            default:
                                throw new StrictJsonException("LVL-PARSE-ESCAPE", $"Ungültige Escapesequenz an Position {_index}.");
                        }
                    }
                    else
                    {
                        if (c <= MaxControlCharacter)
                        {
                            throw new StrictJsonException("LVL-PARSE-CONTROL-CHAR", "Nicht maskiertes Steuerzeichen in Zeichenkette.");
                        }
                        builder.Append(c);
                        _index++;
                    }
                    if (builder.Length > _limits.MaxStringLength)
                    {
                        throw new StrictJsonException("LVL-PARSE-RESOURCE-STRING", $"Eine Zeichenkette überschreitet das Längenlimit von {_limits.MaxStringLength}.");
                    }
                }
                var result = builder.ToString();
                ValidateSurrogates(result);
                return result;
            }

            private char ParseUnicodeEscape()
            {
                _index++;
                if (_index + 4 > _text.Length)
                {
                    throw new StrictJsonException("LVL-PARSE-ESCAPE", "Abgeschnittene Unicode-Escapesequenz.");
                }
                var value = 0;
                for (var i = 0; i < 4; i++)
                {
                    var c = _text[_index + i];
                    int digit;
                    if (c >= '0' && c <= '9')
                    {
                        digit = c - '0';
                    }
                    else if (c >= 'a' && c <= 'f')
                    {
                        digit = c - 'a' + 10;
                    }
                    else if (c >= 'A' && c <= 'F')
                    {
                        digit = c - 'A' + 10;
                    }
                    else
                    {
                        throw new StrictJsonException("LVL-PARSE-ESCAPE", $"Ungültige Unicode-Escapesequenz an Position {_index}.");
                    }
                    value = (value << 4) | digit;
                }
                _index += 4;
                return (char)value;
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
                            throw new StrictJsonException("LVL-PARSE-SURROGATE", "Ungepaartes High-Surrogate in Zeichenkette.");
                        }
                        var next = value[i + 1];
                        if (next < LowSurrogateMin || next > LowSurrogateMax)
                        {
                            throw new StrictJsonException("LVL-PARSE-SURROGATE", "Ungepaartes High-Surrogate in Zeichenkette.");
                        }
                        i++;
                    }
                    else if (c >= LowSurrogateMin && c <= LowSurrogateMax)
                    {
                        throw new StrictJsonException("LVL-PARSE-SURROGATE", "Ungepaartes Low-Surrogate in Zeichenkette.");
                    }
                }
            }

            private JsonValue.Object ParseObject(int depth)
            {
                _index++;
                var pairs = new List<KeyValuePair<string, JsonValue>>();
                var keys = new HashSet<string>(StringComparer.Ordinal);
                SkipWhitespace();
                if (!AtEnd && _text[_index] == '}')
                {
                    _index++;
                    return new JsonValue.Object(pairs);
                }
                while (true)
                {
                    SkipWhitespace();
                    if (AtEnd || _text[_index] != '"')
                    {
                        throw new StrictJsonException("LVL-PARSE-SYNTAX", $"Objektschlüssel an Position {_index} erwartet.");
                    }
                    var key = ParseString();
                    if (key == "$type")
                    {
                        throw new StrictJsonException("LVL-PARSE-TYPE-METADATA", "Polymorphe Typmetadaten ($type) sind deaktiviert.");
                    }
                    if (!keys.Add(key))
                    {
                        throw new StrictJsonException("LVL-PARSE-DUPLICATE-KEY", $"Doppelter Objektschlüssel: {key}.");
                    }
                    SkipWhitespace();
                    if (AtEnd || _text[_index] != ':')
                    {
                        throw new StrictJsonException("LVL-PARSE-SYNTAX", $"Doppelpunkt an Position {_index} erwartet.");
                    }
                    _index++;
                    var value = ParseValue(depth + 1);
                    pairs.Add(new KeyValuePair<string, JsonValue>(key, value));
                    SkipWhitespace();
                    if (AtEnd)
                    {
                        throw new StrictJsonException("LVL-PARSE-SYNTAX", "Nicht abgeschlossenes Objekt.");
                    }
                    if (_text[_index] == '}')
                    {
                        _index++;
                        return new JsonValue.Object(pairs);
                    }
                    if (_text[_index] != ',')
                    {
                        throw new StrictJsonException("LVL-PARSE-SYNTAX", $"Komma an Position {_index} erwartet.");
                    }
                    _index++;
                }
            }

            private JsonValue.Array ParseArray(int depth)
            {
                _index++;
                var items = new List<JsonValue>();
                SkipWhitespace();
                if (!AtEnd && _text[_index] == ']')
                {
                    _index++;
                    return new JsonValue.Array(items);
                }
                while (true)
                {
                    items.Add(ParseValue(depth + 1));
                    if (items.Count > _limits.MaxArrayElements)
                    {
                        throw new StrictJsonException("LVL-PARSE-RESOURCE-ARRAY", $"Ein Array überschreitet das Elementlimit von {_limits.MaxArrayElements}.");
                    }
                    SkipWhitespace();
                    if (AtEnd)
                    {
                        throw new StrictJsonException("LVL-PARSE-SYNTAX", "Nicht abgeschlossenes Array.");
                    }
                    if (_text[_index] == ']')
                    {
                        _index++;
                        return new JsonValue.Array(items);
                    }
                    if (_text[_index] != ',')
                    {
                        throw new StrictJsonException("LVL-PARSE-SYNTAX", $"Komma an Position {_index} erwartet.");
                    }
                    _index++;
                }
            }

            private JsonValue.Integer ParseNumber()
            {
                var start = _index;
                if (_text[_index] == '-')
                {
                    _index++;
                }
                if (AtEnd)
                {
                    throw new StrictJsonException("LVL-PARSE-SYNTAX", "Ungültige Zahl.");
                }
                if (_text[_index] == '0')
                {
                    _index++;
                }
                else if (_text[_index] >= '1' && _text[_index] <= '9')
                {
                    while (!AtEnd && _text[_index] >= '0' && _text[_index] <= '9')
                    {
                        _index++;
                    }
                }
                else
                {
                    throw new StrictJsonException("LVL-PARSE-SYNTAX", "Ungültige Zahl.");
                }
                if (!AtEnd && (_text[_index] == '.' || _text[_index] == 'e' || _text[_index] == 'E'))
                {
                    throw new StrictJsonException("LVL-PARSE-FLOAT-TOKEN", "Fließkomma- und Exponententokens sind verboten (I-JSON).");
                }
                var token = _text.Substring(start, _index - start);
                if (!long.TryParse(token, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var value))
                {
                    throw new StrictJsonException("LVL-PARSE-INTEGER-RANGE", "Ganzzahl außerhalb des 64-Bit-Bereichs.");
                }
                const long safeLimit = (1L << 53) - 1;
                if (value > safeLimit || value < -safeLimit)
                {
                    throw new StrictJsonException("LVL-PARSE-INTEGER-RANGE", "Ganzzahl außerhalb des interoperablen sicheren Bereichs (2^53-1).");
                }
                return new JsonValue.Integer(value);
            }
        }
    }
}
