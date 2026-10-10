using System;
using System.Collections.Generic;

namespace STP.Infrastructure.Content
{
    /// <summary>Shared constants of the Level-v2 / hash contracts (LEVEL_DATA_FORMAT.md, ADR-021).</summary>
    public static class LevelV2Contract
    {
        public const int DocumentSchemaVersion = 2;
        public const int LegacySchemaVersion = 1;
        public const string RulesetVersion = "train-track-v1";
        public const int ProofFormatVersion = 1;
        /// <summary>Upper bound of the Level-v2 schema for grid sizes, counts and index values (the Domain itself allows up to 32).</summary>
        public const int MaxSchemaGridSize = 10;
    }

    /// <summary>Registry of the closed hash profile names. An unknown profile is a hard error, never interpreted heuristically.</summary>
    public static class LevelHashProfiles
    {
        public const string PuzzleSemantic = "STP-PUZZLE-SEMANTIC-JCS-1";
        public const string Solution = "STP-SOLUTION-JCS-1";
        public const string Proof = "STP-PROOF-JCS-1";
        public const string LevelV1Puzzle = "STP-LEVEL-V1-PUZZLE-JCS-1";

        public static bool IsKnown(string? profile) =>
            string.Equals(profile, PuzzleSemantic, StringComparison.Ordinal)
            || string.Equals(profile, Solution, StringComparison.Ordinal)
            || string.Equals(profile, Proof, StringComparison.Ordinal)
            || string.Equals(profile, LevelV1Puzzle, StringComparison.Ordinal);
    }

    /// <summary>A hash stored as <c>{profile, sha256}</c>; sha256 is 64 lowercase hexadecimal characters.</summary>
    public sealed class ProfiledHash : IEquatable<ProfiledHash>
    {
        public string Profile { get; }
        public string Sha256 { get; }

        public ProfiledHash(string profile, string sha256)
        {
            Profile = profile ?? throw new ArgumentNullException(nameof(profile));
            Sha256 = sha256 ?? throw new ArgumentNullException(nameof(sha256));
        }

        public bool Equals(ProfiledHash? other) => other != null && string.Equals(Profile, other.Profile, StringComparison.Ordinal) && string.Equals(Sha256, other.Sha256, StringComparison.Ordinal);
        public override bool Equals(object? obj) => Equals(obj as ProfiledHash);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Profile) * 31 + StringComparer.Ordinal.GetHashCode(Sha256);
        public override string ToString() => Profile + ":" + Sha256;

        public JsonValue ToJson() => JsonValue.CreateObject(new[]
        {
            new JsonMember("profile", JsonValue.CreateString(Profile)),
            new JsonMember("sha256", JsonValue.CreateString(Sha256))
        });
    }

    /// <summary>Hand-written, ECMA-262-anchored matchers for the schema patterns (no regex engine, no culture or newline surprises).</summary>
    internal static class LevelPatterns
    {
        private static bool Lower(char c) => c >= 'a' && c <= 'z';
        private static bool Digit(char c) => c >= '0' && c <= '9';
        private static bool LowerOrDigit(char c) => Lower(c) || Digit(c);

        /// <summary><c>^S[1-9][0-9]*-[0-9]{2}-[0-9]{2}-[0-9]{2}$</c></summary>
        public static bool IsPuzzleId(string value)
        {
            if (value.Length < 11 || value[0] != 'S' || value[1] < '1' || value[1] > '9') return false;
            int i = 2;
            while (i < value.Length && Digit(value[i])) i++;
            for (int segment = 0; segment < 3; segment++)
            {
                if (i + 3 > value.Length || value[i] != '-' || !Digit(value[i + 1]) || !Digit(value[i + 2])) return false;
                i += 3;
            }
            return i == value.Length;
        }

        /// <summary>Parses the four numeric segments of a syntactically valid puzzle id; false on 64-bit overflow.</summary>
        public static bool TryParsePuzzleId(string value, out long season, out long section, out long route, out long position)
        {
            season = section = route = position = 0;
            if (!IsPuzzleId(value)) return false;
            string[] parts = value.Substring(1).Split('-');
            return parts.Length == 4
                && long.TryParse(parts[0], System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out season)
                && long.TryParse(parts[1], System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out section)
                && long.TryParse(parts[2], System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out route)
                && long.TryParse(parts[3], System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out position);
        }

        /// <summary>Level-v2 <c>^[a-z][a-z0-9]*(?:[.-][a-z0-9]+)+$</c></summary>
        public static bool IsLocalizationKeyV2(string value)
        {
            if (value.Length == 0 || !Lower(value[0])) return false;
            int i = 1;
            while (i < value.Length && LowerOrDigit(value[i])) i++;
            int groups = 0;
            while (i < value.Length)
            {
                if (value[i] != '.' && value[i] != '-') return false;
                i++;
                int start = i;
                while (i < value.Length && LowerOrDigit(value[i])) i++;
                if (i == start) return false;
                groups++;
            }
            return groups >= 1;
        }

        /// <summary>Level-v1 <c>^[a-z][a-z0-9-]*(\.[a-z0-9-]+)+$</c> (the maxLength 160 is checked by the caller).</summary>
        public static bool IsLocalizationKeyV1(string value)
        {
            if (value.Length == 0 || !Lower(value[0])) return false;
            string[] segments = value.Split('.');
            if (segments.Length < 2) return false;
            for (int s = 0; s < segments.Length; s++)
            {
                string segment = segments[s];
                if (segment.Length == 0) return false;
                for (int i = 0; i < segment.Length; i++)
                    if (!(LowerOrDigit(segment[i]) || segment[i] == '-')) return false;
            }
            return true;
        }

        /// <summary>Level-v1 <c>^[a-z][a-z0-9]*(?:-[a-z0-9]+)*$</c> (the maxLength 100 is checked by the caller).</summary>
        public static bool IsStableIdV1(string value)
        {
            if (value.Length == 0 || !Lower(value[0])) return false;
            int i = 1;
            while (i < value.Length && LowerOrDigit(value[i])) i++;
            while (i < value.Length)
            {
                if (value[i] != '-') return false;
                i++;
                int start = i;
                while (i < value.Length && LowerOrDigit(value[i])) i++;
                if (i == start) return false;
            }
            return true;
        }

        /// <summary><c>^[A-Za-z0-9._/-]+$</c></summary>
        public static bool IsArtifactId(string value)
        {
            if (value.Length == 0) return false;
            foreach (char c in value)
                if (!(Lower(c) || (c >= 'A' && c <= 'Z') || Digit(c) || c == '.' || c == '_' || c == '/' || c == '-')) return false;
            return true;
        }

        /// <summary><c>^[0-9a-f]{64}$</c></summary>
        public static bool IsSha256Hex(string value)
        {
            if (value.Length != 64) return false;
            foreach (char c in value)
                if (!(Digit(c) || (c >= 'a' && c <= 'f'))) return false;
            return true;
        }

        /// <summary><c>^solver-v[1-9][0-9]*$</c></summary>
        public static bool IsSolverVersion(string value)
        {
            const string prefix = "solver-v";
            if (value.Length <= prefix.Length || !value.StartsWith(prefix, StringComparison.Ordinal)) return false;
            if (value[prefix.Length] < '1' || value[prefix.Length] > '9') return false;
            for (int i = prefix.Length + 1; i < value.Length; i++) if (!Digit(value[i])) return false;
            return true;
        }
    }
}
