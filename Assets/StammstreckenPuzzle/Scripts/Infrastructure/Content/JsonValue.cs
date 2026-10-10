using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace STP.Infrastructure.Content
{
    /// <summary>The closed set of JSON value kinds of the STP integer-only JSON profile (no floating-point kind exists).</summary>
    public enum JsonKind
    {
        Null,
        Boolean,
        Integer,
        String,
        Array,
        Object
    }

    /// <summary>A named member of a JSON object. Names are unique inside one object.</summary>
    public sealed class JsonMember
    {
        public string Name { get; }
        public JsonValue Value { get; }

        public JsonMember(string name, JsonValue value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    /// <summary>
    /// Immutable JSON document node. Numbers are exclusively integers inside the I-JSON safe range
    /// (the parser and <see cref="CreateInteger"/> enforce this). Object member names are unique, and
    /// <see cref="Items"/> and <see cref="Members"/> are read-only views that never expose the internal storage, so a
    /// node cannot change after construction. Strings are the one kind that is not always canonicalisable:
    /// <see cref="CreateString"/> accepts any .NET string including lone UTF-16 surrogates, while
    /// <see cref="JcsSerializer"/> rejects them as <c>LVL-JCS-SURROGATE</c>; the parser never produces them.
    /// </summary>
    public sealed class JsonValue
    {
        /// <summary>Largest integer magnitude that survives an IEEE-754 double round trip (2^53 - 1).</summary>
        public const long MaxSafeInteger = 9007199254740991L;

        // Declared before the shared instances: the constructor assigns these empty views.
        private static readonly ReadOnlyCollection<JsonValue> NoItems = new ReadOnlyCollection<JsonValue>(Array.Empty<JsonValue>());
        private static readonly ReadOnlyCollection<JsonMember> NoMembers = new ReadOnlyCollection<JsonMember>(Array.Empty<JsonMember>());

        private static readonly JsonValue NullInstance = new JsonValue(JsonKind.Null, false, 0, string.Empty, null, null);
        private static readonly JsonValue TrueInstance = new JsonValue(JsonKind.Boolean, true, 0, string.Empty, null, null);
        private static readonly JsonValue FalseInstance = new JsonValue(JsonKind.Boolean, false, 0, string.Empty, null, null);

        private readonly bool _boolean;
        private readonly long _integer;
        private readonly string _string;
        private readonly JsonValue[]? _items;
        private readonly JsonMember[]? _members;
        private readonly ReadOnlyCollection<JsonValue> _itemsView;
        private readonly ReadOnlyCollection<JsonMember> _membersView;

        public JsonKind Kind { get; }

        private JsonValue(JsonKind kind, bool boolean, long integer, string text, JsonValue[]? items, JsonMember[]? members)
        {
            Kind = kind;
            _boolean = boolean;
            _integer = integer;
            _string = text;
            _items = items;
            _members = members;
            _itemsView = items == null || items.Length == 0 ? NoItems : Array.AsReadOnly(items);
            _membersView = members == null || members.Length == 0 ? NoMembers : Array.AsReadOnly(members);
        }

        public static JsonValue Null => NullInstance;
        public static JsonValue FromBoolean(bool value) => value ? TrueInstance : FalseInstance;

        /// <summary>Creates an integer node; throws for magnitudes beyond the I-JSON safe range (programming error).</summary>
        public static JsonValue CreateInteger(long value)
        {
            if (value < -MaxSafeInteger || value > MaxSafeInteger) throw new ArgumentOutOfRangeException(nameof(value), "Integer outside the I-JSON safe range.");
            return new JsonValue(JsonKind.Integer, false, value, string.Empty, null, null);
        }

        /// <summary>
        /// Creates a string node. Any .NET string is accepted, including lone surrogates (a DOM can be built
        /// programmatically); canonicalisation and hashing reject those with <c>LVL-JCS-SURROGATE</c>.
        /// </summary>
        public static JsonValue CreateString(string value) => new JsonValue(JsonKind.String, false, 0, value ?? throw new ArgumentNullException(nameof(value)), null, null);

        public static JsonValue CreateArray(IEnumerable<JsonValue> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            var copy = new List<JsonValue>();
            foreach (var item in items) copy.Add(item ?? throw new ArgumentException("Array item is null.", nameof(items)));
            return new JsonValue(JsonKind.Array, false, 0, string.Empty, copy.ToArray(), null);
        }

        /// <summary>Creates an object node; throws for duplicate member names (programming error, never produced by the parser).</summary>
        public static JsonValue CreateObject(IEnumerable<JsonMember> members)
        {
            if (members == null) throw new ArgumentNullException(nameof(members));
            var copy = new List<JsonMember>();
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var member in members)
            {
                if (member == null) throw new ArgumentException("Object member is null.", nameof(members));
                if (!names.Add(member.Name)) throw new ArgumentException("Duplicate object member name: " + member.Name, nameof(members));
                copy.Add(member);
            }
            return new JsonValue(JsonKind.Object, false, 0, string.Empty, null, copy.ToArray());
        }

        public bool BooleanValue => Kind == JsonKind.Boolean ? _boolean : throw new InvalidOperationException("Not a boolean.");
        public long IntegerValue => Kind == JsonKind.Integer ? _integer : throw new InvalidOperationException("Not an integer.");
        public string StringValue => Kind == JsonKind.String ? _string : throw new InvalidOperationException("Not a string.");

        /// <summary>Array items in document order as a read-only view (never the internal storage); empty for non-arrays.</summary>
        public IReadOnlyList<JsonValue> Items => _itemsView;

        /// <summary>Object members in document order as a read-only view (never the internal storage); empty for non-objects.</summary>
        public IReadOnlyList<JsonMember> Members => _membersView;

        /// <summary>Returns the member value with the given ordinal name, or null when absent or not an object.</summary>
        public JsonValue? Get(string name)
        {
            if (_members == null) return null;
            for (int i = 0; i < _members.Length; i++)
                if (string.Equals(_members[i].Name, name, StringComparison.Ordinal)) return _members[i].Value;
            return null;
        }
    }
}
