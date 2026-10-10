using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Implementation limits for untrusted JSON (LEVEL_DATA_FORMAT.md Abschnitt 11). These are parser limits of this
    /// implementation, not part of the document schema; every violation is a stable fail-closed diagnostic.
    /// </summary>
    public sealed class JsonParserLimits
    {
        public int MaxBytes { get; }
        public int MaxDepth { get; }
        public int MaxStringLength { get; }
        public int MaxArrayLength { get; }
        public int MaxObjectMembers { get; }

        /// <summary>256 KiB input, nesting depth 16, strings and keys up to 8192 UTF-16 code units, arrays up to 1024 items, objects up to 64 members.</summary>
        public static JsonParserLimits Default { get; } = new JsonParserLimits(256 * 1024, 16, 8192, 1024, 64);

        public JsonParserLimits(int maxBytes, int maxDepth, int maxStringLength, int maxArrayLength, int maxObjectMembers)
        {
            if (maxBytes < 1 || maxDepth < 1 || maxStringLength < 1 || maxArrayLength < 1 || maxObjectMembers < 1)
                throw new ArgumentOutOfRangeException(nameof(maxBytes), "All parser limits must be positive.");
            MaxBytes = maxBytes; MaxDepth = maxDepth; MaxStringLength = maxStringLength; MaxArrayLength = maxArrayLength; MaxObjectMembers = maxObjectMembers;
        }
    }

    /// <summary>Outcome of parsing: a JSON value or exactly one stable parse diagnostic.</summary>
    public sealed class JsonParseResult
    {
        public JsonValue? Value { get; }
        public LevelDiagnostic? Error { get; }
        public bool Succeeded => Error == null && Value != null;

        private JsonParseResult(JsonValue? value, LevelDiagnostic? error) { Value = value; Error = error; }
        internal static JsonParseResult Ok(JsonValue value) => new JsonParseResult(value, null);
        internal static JsonParseResult Fail(string code, string path, string message) => new JsonParseResult(null, new LevelDiagnostic(LevelDiagnosticStage.Parse, code, path, message));
    }

    /// <summary>
    /// Strict RFC 8259 parser for untrusted Level JSON: UTF-8 without BOM, no comments, no duplicate keys, no
    /// floating-point or exponent tokens, integers only inside the I-JSON safe range, no unpaired surrogates (raw or
    /// escaped), no unescaped control characters, no trailing content, and bounded size, depth, string, array and
    /// object lengths. It never throws for any input.
    /// </summary>
    public static class StrictJsonParser
    {
        private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);

        public static JsonParseResult Parse(byte[]? utf8, JsonParserLimits? limits = null)
        {
            var effective = limits ?? JsonParserLimits.Default;
            if (utf8 == null || utf8.Length == 0) return JsonParseResult.Fail(LevelDiagnosticCodes.ParseEmpty, string.Empty, "Input is empty.");
            if (utf8.Length > effective.MaxBytes) return JsonParseResult.Fail(LevelDiagnosticCodes.ParseSize, string.Empty, "Input exceeds " + effective.MaxBytes.ToString(CultureInfo.InvariantCulture) + " bytes.");
            if (utf8.Length >= 3 && utf8[0] == 0xEF && utf8[1] == 0xBB && utf8[2] == 0xBF) return JsonParseResult.Fail(LevelDiagnosticCodes.ParseBom, string.Empty, "UTF-8 byte order mark is not allowed.");
            string text;
            try { text = StrictUtf8.GetString(utf8); }
            catch (ArgumentException) { return JsonParseResult.Fail(LevelDiagnosticCodes.ParseUtf8, string.Empty, "Input is not well-formed UTF-8."); }
            return ParseCore(text, effective);
        }

        /// <summary>
        /// Parses already decoded text; raw lone surrogates are rejected like escaped ones. The size limit
        /// (<see cref="JsonParserLimits.MaxBytes"/>) is measured in UTF-8 bytes exactly like <see cref="Parse"/>, so both
        /// entries accept and reject the same document for size (a lone surrogate counts as the three bytes of its
        /// replacement character; it is rejected by content afterwards anyway).
        /// </summary>
        public static JsonParseResult ParseText(string? text, JsonParserLimits? limits = null)
        {
            var effective = limits ?? JsonParserLimits.Default;
            if (text == null || text.Length == 0) return JsonParseResult.Fail(LevelDiagnosticCodes.ParseEmpty, string.Empty, "Input is empty.");
            if (Utf8SizeExceeds(text, effective.MaxBytes)) return JsonParseResult.Fail(LevelDiagnosticCodes.ParseSize, string.Empty, "Input exceeds " + effective.MaxBytes.ToString(CultureInfo.InvariantCulture) + " bytes.");
            if (text[0] == '\uFEFF') return JsonParseResult.Fail(LevelDiagnosticCodes.ParseBom, string.Empty, "Byte order mark is not allowed.");
            return ParseCore(text, effective);
        }

        /// <summary>True when the UTF-8 encoding of <paramref name="text"/> is longer than <paramref name="maxBytes"/>; stops counting as soon as the limit is passed.</summary>
        private static bool Utf8SizeExceeds(string text, int maxBytes)
        {
            long bytes = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c < 0x80) bytes += 1;
                else if (c < 0x800) bytes += 2;
                else if (char.IsHighSurrogate(c) && i + 1 < text.Length && char.IsLowSurrogate(text[i + 1])) { bytes += 4; i++; }
                else bytes += 3;
                if (bytes > maxBytes) return true;
            }
            return false;
        }

        private static JsonParseResult ParseCore(string text, JsonParserLimits limits)
        {
            var reader = new Reader(text, limits);
            try
            {
                reader.SkipWhitespace();
                if (reader.AtEnd) return JsonParseResult.Fail(LevelDiagnosticCodes.ParseEmpty, string.Empty, "Input contains no JSON value.");
                var value = reader.ParseValue(0, string.Empty);
                reader.SkipWhitespace();
                if (!reader.AtEnd) throw reader.TrailingFailure();
                return JsonParseResult.Ok(value);
            }
            catch (ParseFailure failure)
            {
                return JsonParseResult.Fail(failure.Code, failure.Path, failure.Message);
            }
        }

        private sealed class ParseFailure : Exception
        {
            public string Code { get; }
            public string Path { get; }
            public ParseFailure(string code, string path, string message) : base(message) { Code = code; Path = path; }
        }

        private sealed class Reader
        {
            private readonly string _text;
            private readonly JsonParserLimits _limits;
            private int _pos;

            public Reader(string text, JsonParserLimits limits) { _text = text; _limits = limits; }

            public bool AtEnd => _pos >= _text.Length;

            public ParseFailure Failure(string code, string path, string message) =>
                new ParseFailure(code, path, message + " (offset " + _pos.ToString(CultureInfo.InvariantCulture) + ").");

            public ParseFailure TrailingFailure() =>
                _text[_pos] == '/' ? Failure(LevelDiagnosticCodes.ParseComment, string.Empty, "Comments are not allowed")
                    : Failure(LevelDiagnosticCodes.ParseTrailingContent, string.Empty, "Unexpected content after the top-level value");

            private ParseFailure Unexpected(string path)
            {
                if (AtEnd) return Failure(LevelDiagnosticCodes.ParseSyntax, path, "Unexpected end of input");
                if (_text[_pos] == '/') return Failure(LevelDiagnosticCodes.ParseComment, path, "Comments are not allowed");
                return Failure(LevelDiagnosticCodes.ParseSyntax, path, "Unexpected character");
            }

            public void SkipWhitespace()
            {
                while (_pos < _text.Length)
                {
                    char c = _text[_pos];
                    if (c == ' ' || c == '\t' || c == '\n' || c == '\r') _pos++;
                    else break;
                }
            }

            public JsonValue ParseValue(int depth, string path)
            {
                SkipWhitespace();
                if (AtEnd) throw Unexpected(path);
                char c = _text[_pos];
                switch (c)
                {
                    case '{': return ParseObject(depth + 1, path);
                    case '[': return ParseArray(depth + 1, path);
                    case '"': return JsonValue.CreateString(ParseString(path));
                    case 't': ExpectLiteral("true", path); return JsonValue.FromBoolean(true);
                    case 'f': ExpectLiteral("false", path); return JsonValue.FromBoolean(false);
                    case 'n': ExpectLiteral("null", path); return JsonValue.Null;
                    default:
                        if (c == '-' || (c >= '0' && c <= '9')) return ParseNumber(path);
                        throw Unexpected(path);
                }
            }

            private void ExpectLiteral(string literal, string path)
            {
                if (string.CompareOrdinal(_text, _pos, literal, 0, literal.Length) != 0) throw Unexpected(path);
                _pos += literal.Length;
            }

            private JsonValue ParseObject(int depth, string path)
            {
                if (depth > _limits.MaxDepth) throw Failure(LevelDiagnosticCodes.ParseDepth, path, "Nesting deeper than " + _limits.MaxDepth.ToString(CultureInfo.InvariantCulture));
                _pos++;
                var members = new List<JsonMember>();
                var names = new HashSet<string>(StringComparer.Ordinal);
                SkipWhitespace();
                if (!AtEnd && _text[_pos] == '}') { _pos++; return JsonValue.CreateObject(members); }
                while (true)
                {
                    SkipWhitespace();
                    if (AtEnd || _text[_pos] != '"') throw Unexpected(path);
                    string key = ParseString(path);
                    string memberPath = path + "/" + key;
                    if (!names.Add(key)) throw Failure(LevelDiagnosticCodes.ParseDuplicateKey, memberPath, "Duplicate object key");
                    if (names.Count > _limits.MaxObjectMembers) throw Failure(LevelDiagnosticCodes.ParseObjectMembers, path, "Object has more than " + _limits.MaxObjectMembers.ToString(CultureInfo.InvariantCulture) + " members");
                    SkipWhitespace();
                    if (AtEnd || _text[_pos] != ':') throw Unexpected(memberPath);
                    _pos++;
                    members.Add(new JsonMember(key, ParseValue(depth, memberPath)));
                    SkipWhitespace();
                    if (AtEnd) throw Unexpected(path);
                    if (_text[_pos] == ',') { _pos++; continue; }
                    if (_text[_pos] == '}') { _pos++; return JsonValue.CreateObject(members); }
                    throw Unexpected(path);
                }
            }

            private JsonValue ParseArray(int depth, string path)
            {
                if (depth > _limits.MaxDepth) throw Failure(LevelDiagnosticCodes.ParseDepth, path, "Nesting deeper than " + _limits.MaxDepth.ToString(CultureInfo.InvariantCulture));
                _pos++;
                var items = new List<JsonValue>();
                SkipWhitespace();
                if (!AtEnd && _text[_pos] == ']') { _pos++; return JsonValue.CreateArray(items); }
                while (true)
                {
                    if (items.Count + 1 > _limits.MaxArrayLength) throw Failure(LevelDiagnosticCodes.ParseArrayLength, path, "Array has more than " + _limits.MaxArrayLength.ToString(CultureInfo.InvariantCulture) + " items");
                    items.Add(ParseValue(depth, path + "/" + items.Count.ToString(CultureInfo.InvariantCulture)));
                    SkipWhitespace();
                    if (AtEnd) throw Unexpected(path);
                    if (_text[_pos] == ',') { _pos++; continue; }
                    if (_text[_pos] == ']') { _pos++; return JsonValue.CreateArray(items); }
                    throw Unexpected(path);
                }
            }

            private JsonValue ParseNumber(string path)
            {
                int start = _pos;
                bool negative = false;
                if (_text[_pos] == '-') { negative = true; _pos++; }
                if (AtEnd || _text[_pos] < '0' || _text[_pos] > '9') throw Failure(LevelDiagnosticCodes.ParseSyntax, path, "Invalid number");
                int digitsStart = _pos;
                if (_text[_pos] == '0')
                {
                    _pos++;
                    if (!AtEnd && _text[_pos] >= '0' && _text[_pos] <= '9') throw Failure(LevelDiagnosticCodes.ParseSyntax, path, "Leading zeros are not allowed");
                }
                else
                {
                    while (!AtEnd && _text[_pos] >= '0' && _text[_pos] <= '9') _pos++;
                }
                int digitsEnd = _pos;
                if (!AtEnd && (_text[_pos] == '.' || _text[_pos] == 'e' || _text[_pos] == 'E'))
                    throw Failure(LevelDiagnosticCodes.ParseFloat, path, "Fraction and exponent number tokens are forbidden");
                int digitCount = digitsEnd - digitsStart;
                if (negative && digitCount == 1 && _text[digitsStart] == '0')
                    throw Failure(LevelDiagnosticCodes.ParseNegativeZero, path, "Negative zero is not allowed");
                if (digitCount > 16) throw Failure(LevelDiagnosticCodes.ParseNumberRange, path, "Integer outside the I-JSON safe range");
                long magnitude = long.Parse(_text.Substring(digitsStart, digitCount), NumberStyles.None, CultureInfo.InvariantCulture);
                if (magnitude > JsonValue.MaxSafeInteger) throw Failure(LevelDiagnosticCodes.ParseNumberRange, path, "Integer outside the I-JSON safe range");
                _ = start;
                return JsonValue.CreateInteger(negative ? -magnitude : magnitude);
            }

            private string ParseString(string path)
            {
                _pos++; // opening quote
                var builder = new StringBuilder();
                while (true)
                {
                    if (AtEnd) throw Failure(LevelDiagnosticCodes.ParseSyntax, path, "Unterminated string");
                    char c = _text[_pos];
                    if (c == '"')
                    {
                        _pos++;
                        return builder.ToString();
                    }
                    if (builder.Length >= _limits.MaxStringLength) throw StringLengthFailure(path);
                    if (c < 0x20) throw Failure(LevelDiagnosticCodes.ParseControlCharacter, path, "Unescaped control character");
                    if (c == '\\')
                    {
                        _pos++;
                        if (AtEnd) throw Failure(LevelDiagnosticCodes.ParseSyntax, path, "Unterminated escape sequence");
                        char escape = _text[_pos];
                        switch (escape)
                        {
                            case '"': builder.Append('"'); _pos++; break;
                            case '\\': builder.Append('\\'); _pos++; break;
                            case '/': builder.Append('/'); _pos++; break;
                            case 'b': builder.Append('\b'); _pos++; break;
                            case 'f': builder.Append('\f'); _pos++; break;
                            case 'n': builder.Append('\n'); _pos++; break;
                            case 'r': builder.Append('\r'); _pos++; break;
                            case 't': builder.Append('\t'); _pos++; break;
                            case 'u':
                                _pos++;
                                int first = ReadHex4(path);
                                if (first >= 0xD800 && first <= 0xDBFF)
                                {
                                    if (_pos + 1 >= _text.Length || _text[_pos] != '\\' || _text[_pos + 1] != 'u')
                                        throw Failure(LevelDiagnosticCodes.ParseSurrogate, path, "Unpaired high surrogate escape");
                                    _pos += 2;
                                    int second = ReadHex4(path);
                                    if (second < 0xDC00 || second > 0xDFFF)
                                        throw Failure(LevelDiagnosticCodes.ParseSurrogate, path, "Unpaired high surrogate escape");
                                    // A pair adds two UTF-16 code units at once: it needs room for both.
                                    if (builder.Length + 2 > _limits.MaxStringLength) throw StringLengthFailure(path);
                                    builder.Append((char)first).Append((char)second);
                                }
                                else if (first >= 0xDC00 && first <= 0xDFFF)
                                {
                                    throw Failure(LevelDiagnosticCodes.ParseSurrogate, path, "Unpaired low surrogate escape");
                                }
                                else builder.Append((char)first);
                                break;
                            default:
                                throw Failure(LevelDiagnosticCodes.ParseEscape, path, "Invalid escape sequence");
                        }
                        continue;
                    }
                    if (char.IsHighSurrogate(c))
                    {
                        if (_pos + 1 >= _text.Length || !char.IsLowSurrogate(_text[_pos + 1])) throw Failure(LevelDiagnosticCodes.ParseSurrogate, path, "Unpaired high surrogate");
                        if (builder.Length + 2 > _limits.MaxStringLength) throw StringLengthFailure(path);
                        builder.Append(c).Append(_text[_pos + 1]);
                        _pos += 2;
                        continue;
                    }
                    if (char.IsLowSurrogate(c)) throw Failure(LevelDiagnosticCodes.ParseSurrogate, path, "Unpaired low surrogate");
                    builder.Append(c);
                    _pos++;
                }
            }

            private ParseFailure StringLengthFailure(string path) =>
                Failure(LevelDiagnosticCodes.ParseStringLength, path, "String longer than " + _limits.MaxStringLength.ToString(CultureInfo.InvariantCulture) + " UTF-16 code units");

            private int ReadHex4(string path)
            {
                if (_pos + 4 > _text.Length) throw Failure(LevelDiagnosticCodes.ParseEscape, path, "Incomplete unicode escape");
                int value = 0;
                for (int i = 0; i < 4; i++)
                {
                    char c = _text[_pos + i];
                    int digit = c >= '0' && c <= '9' ? c - '0' : c >= 'a' && c <= 'f' ? c - 'a' + 10 : c >= 'A' && c <= 'F' ? c - 'A' + 10 : -1;
                    if (digit < 0) throw Failure(LevelDiagnosticCodes.ParseEscape, path, "Invalid unicode escape");
                    value = value * 16 + digit;
                }
                _pos += 4;
                return value;
            }
        }
    }
}
