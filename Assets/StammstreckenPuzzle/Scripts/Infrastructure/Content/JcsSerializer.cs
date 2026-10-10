using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace STP.Infrastructure.Content
{
    /// <summary>Outcome of a canonicalisation. Exactly one of <see cref="Utf8"/> / <see cref="Error"/> is set.</summary>
    public sealed class JcsResult
    {
        public byte[]? Utf8 { get; }
        public string? Text { get; }
        public LevelDiagnostic? Error { get; }
        public bool Succeeded => Error == null;

        private JcsResult(byte[]? utf8, string? text, LevelDiagnostic? error) { Utf8 = utf8; Text = text; Error = error; }
        internal static JcsResult Ok(string text, byte[] utf8) => new JcsResult(utf8, text, null);
        internal static JcsResult Fail(LevelDiagnostic error) => new JcsResult(null, null, error);
    }

    /// <summary>
    /// RFC 8785 (JSON Canonicalization Scheme) for the STP integer-only I-JSON profile: object members sorted by
    /// UTF-16 code units, no insignificant whitespace, minimal string escaping, integers in the safe range as plain
    /// decimal, output as UTF-8 without BOM and without trailing newline. Floating-point numbers do not exist in
    /// <see cref="JsonValue"/> and unpaired surrogates are rejected.
    /// </summary>
    public static class JcsSerializer
    {
        private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);

        public static JcsResult Serialize(JsonValue? value)
        {
            if (value == null) return JcsResult.Fail(new LevelDiagnostic(LevelDiagnosticStage.Hash, LevelDiagnosticCodes.JcsInvalidInput, string.Empty, "Cannot canonicalise a null reference."));
            var builder = new StringBuilder();
            LevelDiagnostic? error = Write(value, builder, string.Empty);
            if (error != null) return JcsResult.Fail(error);
            string text = builder.ToString();
            return JcsResult.Ok(text, StrictUtf8.GetBytes(text));
        }

        /// <summary>Convenience for internally constructed, known-valid projections; throws when canonicalisation fails.</summary>
        internal static byte[] SerializeOrThrow(JsonValue value)
        {
            var result = Serialize(value);
            if (result.Utf8 == null) throw new InvalidOperationException(result.Error?.ToString() ?? "JCS failure");
            return result.Utf8;
        }

        private static LevelDiagnostic? Write(JsonValue value, StringBuilder builder, string path)
        {
            switch (value.Kind)
            {
                case JsonKind.Null: builder.Append("null"); return null;
                case JsonKind.Boolean: builder.Append(value.BooleanValue ? "true" : "false"); return null;
                case JsonKind.Integer:
                    long number = value.IntegerValue;
                    if (number < -JsonValue.MaxSafeInteger || number > JsonValue.MaxSafeInteger)
                        return new LevelDiagnostic(LevelDiagnosticStage.Hash, LevelDiagnosticCodes.JcsIntegerRange, path, "Integer outside the I-JSON safe range.");
                    builder.Append(number.ToString(CultureInfo.InvariantCulture));
                    return null;
                case JsonKind.String: return WriteString(value.StringValue, builder, path);
                case JsonKind.Array:
                    builder.Append('[');
                    for (int i = 0; i < value.Items.Count; i++)
                    {
                        if (i > 0) builder.Append(',');
                        var error = Write(value.Items[i], builder, path + "/" + i.ToString(CultureInfo.InvariantCulture));
                        if (error != null) return error;
                    }
                    builder.Append(']');
                    return null;
                case JsonKind.Object:
                    var members = new List<JsonMember>(value.Members);
                    members.Sort((left, right) => string.CompareOrdinal(left.Name, right.Name));
                    builder.Append('{');
                    for (int i = 0; i < members.Count; i++)
                    {
                        if (i > 0) builder.Append(',');
                        var keyError = WriteString(members[i].Name, builder, path + "/" + members[i].Name);
                        if (keyError != null) return keyError;
                        builder.Append(':');
                        var valueError = Write(members[i].Value, builder, path + "/" + members[i].Name);
                        if (valueError != null) return valueError;
                    }
                    builder.Append('}');
                    return null;
                default:
                    return new LevelDiagnostic(LevelDiagnosticStage.Hash, LevelDiagnosticCodes.JcsInvalidInput, path, "Unsupported JSON value kind.");
            }
        }

        private static LevelDiagnostic? WriteString(string text, StringBuilder builder, string path)
        {
            builder.Append('"');
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (char.IsHighSurrogate(c))
                {
                    if (i + 1 >= text.Length || !char.IsLowSurrogate(text[i + 1]))
                        return new LevelDiagnostic(LevelDiagnosticStage.Hash, LevelDiagnosticCodes.JcsSurrogate, path, "Unpaired high surrogate.");
                    builder.Append(c).Append(text[i + 1]);
                    i++;
                    continue;
                }
                if (char.IsLowSurrogate(c))
                    return new LevelDiagnostic(LevelDiagnosticStage.Hash, LevelDiagnosticCodes.JcsSurrogate, path, "Unpaired low surrogate.");
                switch (c)
                {
                    case '"': builder.Append("\\\""); break;
                    case '\\': builder.Append("\\\\"); break;
                    case '\b': builder.Append("\\b"); break;
                    case '\f': builder.Append("\\f"); break;
                    case '\n': builder.Append("\\n"); break;
                    case '\r': builder.Append("\\r"); break;
                    case '\t': builder.Append("\\t"); break;
                    default:
                        if (c < 0x20) builder.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                        else builder.Append(c);
                        break;
                }
            }
            builder.Append('"');
            return null;
        }
    }
}
