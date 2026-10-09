using System;
using System.Collections.Generic;
using System.Globalization;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Shared closed-schema reading primitives for the Level-v1 and Level-v2 readers. Every violation becomes a
    /// stable diagnostic; nothing is defaulted, coerced or skipped. Diagnostics are produced in schema property order
    /// (unknown properties sorted ordinally), so they do not depend on member order of the input.
    /// </summary>
    internal sealed class SchemaContext
    {
        private readonly List<LevelDiagnostic> _diagnostics = new List<LevelDiagnostic>();

        public IReadOnlyList<LevelDiagnostic> Diagnostics => _diagnostics;
        public bool HasErrors => _diagnostics.Count > 0;

        public void Add(string code, string path, string message) =>
            _diagnostics.Add(new LevelDiagnostic(LevelDiagnosticStage.Schema, code, path, message));

        public static string Join(string path, string name) => path + "/" + name;

        /// <summary>Checks that <paramref name="value"/> is an object with exactly the given (all required) properties.</summary>
        public bool BeginObject(JsonValue? value, string path, params string[] properties)
        {
            if (value == null || value.Kind != JsonKind.Object)
            {
                Add(LevelDiagnosticCodes.SchemaType, path, "Expected an object.");
                return false;
            }
            var unknown = new List<string>();
            foreach (var member in value.Members)
                if (System.Array.IndexOf(properties, member.Name) < 0) unknown.Add(member.Name);
            unknown.Sort(string.CompareOrdinal);
            foreach (string name in unknown) Add(LevelDiagnosticCodes.SchemaUnknownProperty, Join(path, name), "Property is not part of the closed schema.");
            foreach (string name in properties)
                if (value.Get(name) == null) Add(LevelDiagnosticCodes.SchemaRequired, Join(path, name), "Required property is missing.");
            return true;
        }

        public static int CodePointCount(string text)
        {
            int count = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsHighSurrogate(text[i]) && i + 1 < text.Length && char.IsLowSurrogate(text[i + 1])) i++;
                count++;
            }
            return count;
        }

        public long? IntegerValue(JsonValue value, string path, long min, long max)
        {
            if (value.Kind != JsonKind.Integer) { Add(LevelDiagnosticCodes.SchemaType, path, "Expected an integer."); return null; }
            long number = value.IntegerValue;
            if (number < min || number > max)
            {
                Add(LevelDiagnosticCodes.SchemaRange, path, "Integer must be within " + min.ToString(CultureInfo.InvariantCulture) + ".." + (max == long.MaxValue ? "max" : max.ToString(CultureInfo.InvariantCulture)) + ".");
                return null;
            }
            return number;
        }

        public long? Int(JsonValue obj, string path, string name, long min, long max)
        {
            var value = obj.Get(name);
            return value == null ? (long?)null : IntegerValue(value, Join(path, name), min, max);
        }

        public string? StringValue(JsonValue value, string path, int minLength, int maxLength = int.MaxValue)
        {
            if (value.Kind != JsonKind.String) { Add(LevelDiagnosticCodes.SchemaType, path, "Expected a string."); return null; }
            string text = value.StringValue;
            int length = CodePointCount(text);
            if (length < minLength || length > maxLength)
            {
                Add(LevelDiagnosticCodes.SchemaLength, path, "String length must be within " + minLength.ToString(CultureInfo.InvariantCulture) + ".." + (maxLength == int.MaxValue ? "max" : maxLength.ToString(CultureInfo.InvariantCulture)) + ".");
                return null;
            }
            return text;
        }

        public string? Str(JsonValue obj, string path, string name, int minLength, int maxLength = int.MaxValue)
        {
            var value = obj.Get(name);
            return value == null ? null : StringValue(value, Join(path, name), minLength, maxLength);
        }

        public string? Pattern(JsonValue obj, string path, string name, Func<string, bool> matcher, int maxLength = int.MaxValue)
        {
            var value = obj.Get(name);
            if (value == null) return null;
            string childPath = Join(path, name);
            if (value.Kind != JsonKind.String) { Add(LevelDiagnosticCodes.SchemaType, childPath, "Expected a string."); return null; }
            string text = value.StringValue;
            if (!matcher(text) || CodePointCount(text) > maxLength) { Add(LevelDiagnosticCodes.SchemaPattern, childPath, "String does not match the required pattern."); return null; }
            return text;
        }

        public string? Const(JsonValue obj, string path, string name, string expected, string code)
        {
            var value = obj.Get(name);
            if (value == null) return null;
            string childPath = Join(path, name);
            if (value.Kind != JsonKind.String) { Add(LevelDiagnosticCodes.SchemaType, childPath, "Expected a string."); return null; }
            if (!string.Equals(value.StringValue, expected, StringComparison.Ordinal)) { Add(code, childPath, "Only the value '" + expected + "' is supported."); return null; }
            return expected;
        }

        public long? ConstInt(JsonValue obj, string path, string name, long expected, string code)
        {
            var value = obj.Get(name);
            if (value == null) return null;
            string childPath = Join(path, name);
            if (value.Kind != JsonKind.Integer) { Add(LevelDiagnosticCodes.SchemaType, childPath, "Expected an integer."); return null; }
            if (value.IntegerValue != expected) { Add(code, childPath, "Only the value " + expected.ToString(CultureInfo.InvariantCulture) + " is supported."); return null; }
            return expected;
        }

        public T? EnumValue<T>(JsonValue value, string path, int allowedMembers = int.MaxValue) where T : struct
        {
            if (value.Kind != JsonKind.String) { Add(LevelDiagnosticCodes.SchemaType, path, "Expected a string."); return null; }
            string[] names = System.Enum.GetNames(typeof(T));
            for (int i = 0; i < names.Length && i < allowedMembers; i++)
                if (string.Equals(names[i], value.StringValue, StringComparison.Ordinal)) return (T)System.Enum.Parse(typeof(T), names[i]);
            Add(LevelDiagnosticCodes.SchemaEnum, path, "Value is not one of the closed enumeration.");
            return null;
        }

        public T? EnumMember<T>(JsonValue obj, string path, string name) where T : struct
        {
            var value = obj.Get(name);
            return value == null ? (T?)null : EnumValue<T>(value, Join(path, name));
        }

        /// <summary>A string restricted to a closed list of allowed values.</summary>
        public string? StringEnum(JsonValue obj, string path, string name, string[] allowed)
        {
            var value = obj.Get(name);
            if (value == null) return null;
            string childPath = Join(path, name);
            if (value.Kind != JsonKind.String) { Add(LevelDiagnosticCodes.SchemaType, childPath, "Expected a string."); return null; }
            if (System.Array.IndexOf(allowed, value.StringValue) < 0) { Add(LevelDiagnosticCodes.SchemaEnum, childPath, "Value is not one of the closed enumeration."); return null; }
            return value.StringValue;
        }

        public JsonValue? ArrayMember(JsonValue obj, string path, string name, int minItems, int maxItems)
        {
            var value = obj.Get(name);
            if (value == null) return null;
            string childPath = Join(path, name);
            if (value.Kind != JsonKind.Array) { Add(LevelDiagnosticCodes.SchemaType, childPath, "Expected an array."); return null; }
            if (value.Items.Count < minItems || value.Items.Count > maxItems)
            {
                Add(LevelDiagnosticCodes.SchemaLength, childPath, "Array length must be within " + minItems.ToString(CultureInfo.InvariantCulture) + ".." + (maxItems == int.MaxValue ? "max" : maxItems.ToString(CultureInfo.InvariantCulture)) + ".");
                return null;
            }
            return value;
        }

        /// <summary>Reads an array of bounded integers; returns null if any problem was reported.</summary>
        public int[]? IntArray(JsonValue obj, string path, string name, int minItems, int maxItems, int minValue, int maxValue)
        {
            var array = ArrayMember(obj, path, name, minItems, maxItems);
            if (array == null) return null;
            var result = new int[array.Items.Count];
            bool ok = true;
            for (int i = 0; i < result.Length; i++)
            {
                long? value = IntegerValue(array.Items[i], Join(Join(path, name), i.ToString(CultureInfo.InvariantCulture)), minValue, maxValue);
                if (value == null) ok = false; else result[i] = (int)value.Value;
            }
            return ok ? result : null;
        }
    }
}
