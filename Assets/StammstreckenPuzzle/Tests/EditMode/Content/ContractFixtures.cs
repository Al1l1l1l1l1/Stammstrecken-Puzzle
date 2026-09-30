using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using STP.Infrastructure.Content;

namespace STP.Tests.Content.EditMode
{
    /// <summary>
    /// Gemeinsamer Zugriff der Content-EditMode-Tests auf die
    /// FIXTURE_ONLY-Vertragsfixtures unter ARCHITECTURE/examples/ gemäß
    /// TEST_STRATEGY.md Abschnitt 5 (Golden Fixtures). Die Fixtures werden
    /// ausschließlich gelesen; Mutationen entstehen nur im Speicher.
    /// </summary>
    internal static class ContractFixtures
    {
        internal const string LevelExample = "ARCHITECTURE/examples/level-v2.example.json";
        internal const string LevelSingleCell = "ARCHITECTURE/examples/level-v2.single-cell.example.json";
        internal const string ProofExample = "ARCHITECTURE/examples/proof-v1.example.json";
        internal const string ProofSingleCell = "ARCHITECTURE/examples/proof-v1.single-cell.example.json";

        internal const string ProofExampleArtifactId = "proofs/S1-01-01-01/solver-v1.proof-v1.json";
        internal const string ProofSingleCellArtifactId = "proofs/S99-01-01-01/solver-v1.proof-v1.json";

        internal const string LevelExamplePublicHash = "b2a8bdb105dde81b9072eda0bdb602cb64390c1533494651f1951efea3225031";
        internal const string LevelExampleSolutionHash = "8e49c83742d402c70069db019841f0e5839691b692cb807c30fb9657df59d6ed";
        internal const string LevelExampleProofHash = "b1ad9067e113f1a08b9f4db2d0b66cb41d1726e7290a182fffc11a2ec299ab25";
        internal const string SingleCellPublicHash = "0aa237181732c0b33cf5203302ffb54c76362a5d6a635b54dfaa758e310fa544";
        internal const string SingleCellSolutionHash = "4a1f1b5cbf6b7ff4998144c393f4d6b879d4daeffc648c0114e34ad099d8182e";
        internal const string SingleCellProofHash = "60836f9234091bcddd0b8fc167e639c6104093559299f8c8ffc61e67907699e4";

        internal static string ProjectRoot => Path.GetFullPath(Path.Combine(UnityEngine.Application.dataPath, ".."));

        internal static byte[] ReadBytes(string repositoryRelativePath)
        {
            return File.ReadAllBytes(Path.Combine(ProjectRoot, repositoryRelativePath));
        }

        internal static JsonValue.Object ParseLevel(string repositoryRelativePath)
        {
            return (JsonValue.Object)StrictJsonParser.ParseUtf8(ReadBytes(repositoryRelativePath), JsonParseLimits.Default);
        }

        internal static byte[] ToBytes(JsonValue value)
        {
            return JcsCanonicalizer.ToUtf8Bytes(value);
        }

        internal static byte[] ToBytes(string json)
        {
            return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false).GetBytes(json);
        }

        internal static JsonValue.Integer Int(long value)
        {
            return new JsonValue.Integer(value);
        }

        internal static JsonValue.String Text(string value)
        {
            return new JsonValue.String(value);
        }

        /// <summary>Ersetzt den Wert an einem Punktpfad (zum Beispiel $.content.season oder $.solution.path[0].x) und liefert ein neues Dokument.</summary>
        internal static JsonValue.Object Replace(JsonValue.Object root, string path, JsonValue replacement)
        {
            var segments = Segments(path);
            if (segments.Length == 0)
            {
                throw new ArgumentException("Die Wurzel kann nicht ersetzt werden.", nameof(path));
            }
            return (JsonValue.Object)ReplaceAt(root, segments, 0, replacement);
        }

        /// <summary>Fügt einem Objekt an einem Punktpfad eine neue Eigenschaft hinzu.</summary>
        internal static JsonValue.Object Add(JsonValue.Object root, string path, string key, JsonValue value)
        {
            var target = Navigate(root, path);
            if (target is not JsonValue.Object obj)
            {
                throw new ArgumentException($"Pfad '{path}' zeigt auf kein Objekt.", nameof(path));
            }
            if (obj.Contains(key))
            {
                throw new ArgumentException($"Schlüssel '{key}' existiert bereits.", nameof(key));
            }
            var pairs = new List<KeyValuePair<string, JsonValue>>(obj.Properties)
            {
                new KeyValuePair<string, JsonValue>(key, value),
            };
            return (JsonValue.Object)ReplaceAt(root, Segments(path), 0, new JsonValue.Object(pairs));
        }

        /// <summary>Entfernt eine Eigenschaft oder ein Arrayelement an einem Punktpfad.</summary>
        internal static JsonValue.Object Remove(JsonValue.Object root, string path)
        {
            var segments = Segments(path);
            if (segments.Length == 0)
            {
                throw new ArgumentException("Die Wurzel kann nicht entfernt werden.", nameof(path));
            }
            return (JsonValue.Object)RemoveAt(root, segments, 0);
        }

        /// <summary>Kehrt die Eigenschaftsreihenfolge der Wurzel um (Schlüsselreihenfolge ist semantikneutral).</summary>
        internal static JsonValue.Object ReverseRootKeys(JsonValue.Object root)
        {
            var pairs = new List<KeyValuePair<string, JsonValue>>(root.Properties);
            pairs.Reverse();
            return new JsonValue.Object(pairs);
        }

        private static JsonValue Navigate(JsonValue root, string path)
        {
            var current = root;
            foreach (var segment in Segments(path))
            {
                var (key, index) = ParseSegment(segment);
                if (current is JsonValue.Object obj)
                {
                    if (!obj.TryGet(key, out var next))
                    {
                        throw new ArgumentException($"Schlüssel '{key}' nicht gefunden.");
                    }
                    current = next;
                }
                else
                {
                    throw new ArgumentException($"Segment '{segment}' trifft auf ein Nicht-Objekt.");
                }
                if (index >= 0)
                {
                    if (current is not JsonValue.Array array || index >= array.Items.Count)
                    {
                        throw new ArgumentException($"Segment '{segment}' erwartet ein ausreichend langes Array.");
                    }
                    current = array.Items[index];
                }
            }
            return current;
        }

        private static string[] Segments(string path)
        {
            if (!path.StartsWith("$", StringComparison.Ordinal))
            {
                throw new ArgumentException($"Pfad '{path}' beginnt nicht mit $.", nameof(path));
            }
            return path.Length == 1 ? new string[0] : path.Substring(2).Split('.');
        }

        private static JsonValue ReplaceAt(JsonValue current, string[] segments, int depth, JsonValue replacement)
        {
            if (depth == segments.Length)
            {
                return replacement;
            }
            var (key, index) = ParseSegment(segments[depth]);
            var leaf = depth == segments.Length - 1;
            if (current is JsonValue.Object obj)
            {
                var pairs = new List<KeyValuePair<string, JsonValue>>();
                var found = false;
                foreach (var pair in obj.Properties)
                {
                    if (pair.Key == key)
                    {
                        found = true;
                        if (leaf && index < 0)
                        {
                            pairs.Add(new KeyValuePair<string, JsonValue>(pair.Key, replacement));
                        }
                        else
                        {
                            pairs.Add(new KeyValuePair<string, JsonValue>(pair.Key, ReplaceInValue(pair.Value, segments, depth, index, leaf, replacement)));
                        }
                    }
                    else
                    {
                        pairs.Add(pair);
                    }
                }
                if (!found)
                {
                    throw new ArgumentException($"Schlüssel '{key}' nicht gefunden.");
                }
                return new JsonValue.Object(pairs);
            }
            throw new ArgumentException($"Segment '{segments[depth]}' trifft auf ein Nicht-Objekt.");
        }

        private static JsonValue ReplaceInValue(JsonValue value, string[] segments, int depth, int index, bool leaf, JsonValue replacement)
        {
            if (index >= 0)
            {
                if (value is not JsonValue.Array array || index >= array.Items.Count)
                {
                    throw new ArgumentException("Arrayindex außerhalb des Bereichs.");
                }
                var items = new List<JsonValue>(array.Items);
                items[index] = leaf ? replacement : ReplaceAt(items[index], segments, depth + 1, replacement);
                return new JsonValue.Array(items);
            }
            return ReplaceAt(value, segments, depth + 1, replacement);
        }

        private static JsonValue RemoveAt(JsonValue current, string[] segments, int depth)
        {
            var (key, index) = ParseSegment(segments[depth]);
            var leaf = depth == segments.Length - 1;
            if (current is JsonValue.Object obj)
            {
                var pairs = new List<KeyValuePair<string, JsonValue>>();
                foreach (var pair in obj.Properties)
                {
                    if (pair.Key == key)
                    {
                        if (leaf && index < 0)
                        {
                            continue;
                        }
                        if (index >= 0)
                        {
                            if (pair.Value is not JsonValue.Array array || index >= array.Items.Count)
                            {
                                throw new ArgumentException("Arrayindex außerhalb des Bereichs.");
                            }
                            var items = new List<JsonValue>(array.Items);
                            if (leaf)
                            {
                                items.RemoveAt(index);
                            }
                            else
                            {
                                items[index] = RemoveAt(items[index], segments, depth + 1);
                            }
                            pairs.Add(new KeyValuePair<string, JsonValue>(pair.Key, new JsonValue.Array(items)));
                        }
                        else
                        {
                            pairs.Add(new KeyValuePair<string, JsonValue>(pair.Key, RemoveAt(pair.Value, segments, depth + 1)));
                        }
                    }
                    else
                    {
                        pairs.Add(pair);
                    }
                }
                return new JsonValue.Object(pairs);
            }
            throw new ArgumentException($"Segment '{segments[depth]}' trifft auf ein Nicht-Objekt.");
        }

        private static (string Key, int Index) ParseSegment(string segment)
        {
            var bracket = segment.IndexOf('[');
            if (bracket < 0)
            {
                return (segment, -1);
            }
            var key = segment.Substring(0, bracket);
            var index = int.Parse(segment.Substring(bracket + 1, segment.Length - bracket - 2), System.Globalization.CultureInfo.InvariantCulture);
            return (key, index);
        }
    }
}
