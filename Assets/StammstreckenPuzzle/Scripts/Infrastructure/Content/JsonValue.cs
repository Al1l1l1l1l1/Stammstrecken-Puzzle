using System.Collections.Generic;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Knoten des strikten JSON-Dokumentmodells gemäß LEVEL_DATA_FORMAT.md
    /// Abschnitte 6 und 11. Das Modell kennt bewusst keine Fließkommazahlen:
    /// Nicht-I-JSON-Zahlentoken werden bereits vom Parser abgelehnt.
    /// </summary>
    public abstract class JsonValue
    {
        private JsonValue() { }

        /// <summary>JSON null.</summary>
        public sealed class Null : JsonValue
        {
            /// <summary>Einzige Instanz.</summary>
            public static readonly Null Instance = new Null();
            private Null() { }
        }

        /// <summary>JSON true/false.</summary>
        public sealed class Boolean : JsonValue
        {
            /// <summary>true.</summary>
            public static readonly Boolean True = new Boolean(true);

            /// <summary>false.</summary>
            public static readonly Boolean False = new Boolean(false);

            /// <summary>Wert.</summary>
            public bool Value { get; }

            private Boolean(bool value)
            {
                Value = value;
            }
        }

        /// <summary>JSON-Ganzzahl im interoperablen sicheren Bereich (I-JSON).</summary>
        public sealed class Integer : JsonValue
        {
            /// <summary>Wert (Betrag höchstens 2^53-1, beim Parsen erzwungen).</summary>
            public long Value { get; }

            /// <summary>Erstellt eine Ganzzahl.</summary>
            public Integer(long value)
            {
                Value = value;
            }
        }

        /// <summary>JSON-Zeichenkette (Surrogate sind beim Parsen paarweise geprüft).</summary>
        public sealed class String : JsonValue
        {
            /// <summary>Wert.</summary>
            public string Value { get; }

            /// <summary>Erstellt eine Zeichenkette.</summary>
            public String(string value)
            {
                Value = value;
            }
        }

        /// <summary>JSON-Array.</summary>
        public sealed class Array : JsonValue
        {
            /// <summary>Elemente in Dokumentreihenfolge.</summary>
            public IReadOnlyList<JsonValue> Items { get; }

            /// <summary>Erstellt ein Array.</summary>
            public Array(IReadOnlyList<JsonValue> items)
            {
                Items = items;
            }
        }

        /// <summary>
        /// JSON-Objekt. Eigenschaften liegen in Dokumentreihenfolge vor;
        /// doppelte Schlüssel werden bereits beim Parsen abgelehnt.
        /// </summary>
        public sealed class Object : JsonValue
        {
            /// <summary>Eigenschaften in Dokumentreihenfolge.</summary>
            public IReadOnlyList<KeyValuePair<string, JsonValue>> Properties { get; }

            /// <summary>Erstellt ein Objekt.</summary>
            public Object(IReadOnlyList<KeyValuePair<string, JsonValue>> properties)
            {
                Properties = properties;
            }

            /// <summary>Prüft, ob eine Eigenschaft existiert.</summary>
            public bool Contains(string key)
            {
                return TryGet(key, out _);
            }

            /// <summary>Liefert die Eigenschaft zum Schlüssel, falls vorhanden.</summary>
            public bool TryGet(string key, out JsonValue value)
            {
                foreach (var pair in Properties)
                {
                    if (pair.Key == key)
                    {
                        value = pair.Value;
                        return true;
                    }
                }
                value = Null.Instance;
                return false;
            }
        }
    }
}
