using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using STP.Infrastructure.Content;

namespace STP.Tests.Content.EditMode
{
    /// <summary>Access to the real repository contract files (schemas, examples, goldens) used as test vectors.</summary>
    internal static class RepoFiles
    {
        private static string? _root;

        internal static string Root()
        {
            if (_root != null) return _root;
            string? env = Environment.GetEnvironmentVariable("STP_REPO_ROOT");
            if (!string.IsNullOrEmpty(env) && IsRoot(env!)) return _root = env!;
            foreach (string start in new[] { Directory.GetCurrentDirectory(), TestContext.CurrentContext.TestDirectory, AppDomain.CurrentDomain.BaseDirectory })
            {
                var dir = string.IsNullOrEmpty(start) ? null : new DirectoryInfo(start);
                while (dir != null)
                {
                    if (IsRoot(dir.FullName)) return _root = dir.FullName;
                    dir = dir.Parent;
                }
            }
            throw new AssertionException("Repository root (AGENTS.md next to Assets/ and ARCHITECTURE/) was not found; set STP_REPO_ROOT.");
        }

        private static bool IsRoot(string path) =>
            File.Exists(Path.Combine(path, "AGENTS.md")) && Directory.Exists(Path.Combine(path, "Assets")) && Directory.Exists(Path.Combine(path, "ARCHITECTURE"));

        internal static byte[] Bytes(string repoRelative) => File.ReadAllBytes(Path.Combine(Root(), repoRelative.Replace('/', Path.DirectorySeparatorChar)));
        internal static string Text(string repoRelative) => new UTF8Encoding(false, true).GetString(Bytes(repoRelative));

        internal static string TestAsset(string name) => Text("Assets/StammstreckenPuzzle/Tests/EditMode/Content/" + name);
    }

    internal static class Examples
    {
        internal const string V2 = "ARCHITECTURE/examples/level-v2.example.json";
        internal const string V2Single = "ARCHITECTURE/examples/level-v2.single-cell.example.json";
        internal const string V1 = "ARCHITECTURE/examples/level-v1.example.json";
        internal const string V1Single = "ARCHITECTURE/examples/level-v1.single-cell.example.json";
        internal const string Proof = "ARCHITECTURE/examples/proof-v1.example.json";
        internal const string ProofSingle = "ARCHITECTURE/examples/proof-v1.single-cell.example.json";
        internal const string ReleaseLock = "ARCHITECTURE/examples/release-lock-v1.example.json";

        internal static string[] V2Files => new[] { V2, V2Single };
        internal static string[] V1Files => new[] { V1, V1Single };

        internal static JsonValue Json(string repoRelative)
        {
            var parsed = StrictJsonParser.Parse(RepoFiles.Bytes(repoRelative));
            Assert.That(parsed.Succeeded, Is.True, repoRelative + ": " + parsed.Error);
            return parsed.Value!;
        }

        internal static LevelV2Document V2Document(string repoRelative = V2)
        {
            var read = LevelV2Reader.Read(Json(repoRelative));
            Assert.That(read.Succeeded, Is.True, string.Join("; ", read.Diagnostics.Select(d => d.ToString())));
            return read.Document!;
        }

        internal static LevelV1Document V1Document(string repoRelative = V1)
        {
            var read = LevelV1Reader.Read(Json(repoRelative));
            Assert.That(read.Succeeded, Is.True, string.Join("; ", read.Diagnostics.Select(d => d.ToString())));
            return read.Document!;
        }
    }

    internal static class TextMutation
    {
        /// <summary>Replaces exactly one occurrence; the test fails if the anchor does not exist exactly once.</summary>
        internal static string Replace(string text, string find, string replacement)
        {
            int first = text.IndexOf(find, StringComparison.Ordinal);
            Assert.That(first, Is.GreaterThanOrEqualTo(0), "mutation anchor not found: " + find);
            Assert.That(text.IndexOf(find, first + 1, StringComparison.Ordinal), Is.EqualTo(-1), "mutation anchor not unique: " + find);
            return text.Substring(0, first) + replacement + text.Substring(first + find.Length);
        }

        internal static byte[] Utf8(string text) => new UTF8Encoding(false).GetBytes(text);
    }

    internal static class Codes
    {
        internal static string[] Of(IEnumerable<LevelDiagnostic> diagnostics) => diagnostics.Select(d => d.Code).ToArray();
        internal static string Describe(IEnumerable<LevelDiagnostic> diagnostics) => string.Join(" | ", diagnostics.Select(d => d.ToString()));
        internal static string Hex(byte[] bytes) => LevelHashing.Sha256Hex(bytes);
    }

    /// <summary>Immutable-tree editing for mutation tests: set, remove and insert by slash separated path (array indexes are numbers).</summary>
    internal static class JsonEdit
    {
        internal static JsonValue Set(JsonValue root, string path, JsonValue replacement) => Apply(root, Split(path), 0, replacement, remove: false);
        internal static JsonValue Remove(JsonValue root, string path) => Apply(root, Split(path), 0, null, remove: true);
        internal static JsonValue Add(JsonValue root, string path, string name, JsonValue value)
        {
            var target = Resolve(root, path);
            var members = new List<JsonMember>(target.Members) { new JsonMember(name, value) };
            return Set(root, path, JsonValue.CreateObject(members));
        }
        internal static JsonValue Resolve(JsonValue root, string path)
        {
            var current = root;
            foreach (string token in Split(path))
            {
                if (current.Kind == JsonKind.Array) current = current.Items[int.Parse(token, System.Globalization.CultureInfo.InvariantCulture)];
                else current = current.Get(token) ?? throw new AssertionException("path not found: " + path);
            }
            return current;
        }
        internal static JsonValue Int(long value) => JsonValue.CreateInteger(value);
        internal static JsonValue Str(string value) => JsonValue.CreateString(value);

        private static string[] Split(string path) => path.Length == 0 ? new string[0] : path.TrimStart('/').Split('/');

        private static JsonValue Apply(JsonValue node, string[] tokens, int index, JsonValue? replacement, bool remove)
        {
            if (index == tokens.Length) return replacement ?? throw new InvalidOperationException("no replacement");
            string token = tokens[index];
            bool last = index == tokens.Length - 1;
            if (node.Kind == JsonKind.Array)
            {
                int position = int.Parse(token, System.Globalization.CultureInfo.InvariantCulture);
                var items = new List<JsonValue>(node.Items);
                if (last && remove) items.RemoveAt(position);
                else items[position] = Apply(items[position], tokens, index + 1, replacement, remove);
                return JsonValue.CreateArray(items);
            }
            if (node.Kind != JsonKind.Object) throw new AssertionException("cannot descend into a scalar at " + token);
            var members = new List<JsonMember>();
            bool found = false;
            foreach (var member in node.Members)
            {
                if (member.Name != token) { members.Add(member); continue; }
                found = true;
                if (last && remove) continue;
                members.Add(new JsonMember(token, Apply(member.Value, tokens, index + 1, replacement, remove)));
            }
            if (!found)
            {
                if (remove || !last) throw new AssertionException("path segment not found: " + token);
                members.Add(new JsonMember(token, replacement!));
            }
            return JsonValue.CreateObject(members);
        }
    }
}
