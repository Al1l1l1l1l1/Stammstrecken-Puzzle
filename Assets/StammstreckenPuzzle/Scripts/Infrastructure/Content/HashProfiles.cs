using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Profilregistry der profilierten Hashverträge gemäß LEVEL_DATA_FORMAT.md
    /// Abschnitt 6. Bekannt sind die drei aktuellen Projektionen
    /// <c>STP-PUZZLE-SEMANTIC-JCS-1</c>, <c>STP-SOLUTION-JCS-1</c> und
    /// <c>STP-PROOF-JCS-1</c> sowie der historische Leseeintrag
    /// <c>STP-LEVEL-V1-PUZZLE-JCS-1</c>. Ein unbekanntes Profil ist ein harter
    /// Fehler (<c>LVL-HASH-*</c>); die v1-Projektionsberechnung und die
    /// v1→v2-Migration sind nicht Teil von WP-013.
    /// </summary>
    public static class HashProfiles
    {
        /// <summary>Profil der öffentlichen semantischen Puzzleprojektion.</summary>
        public const string PuzzleSemanticJcs1 = "STP-PUZZLE-SEMANTIC-JCS-1";

        /// <summary>Profil der kanonischen Lösungsprojektion.</summary>
        public const string SolutionJcs1 = "STP-SOLUTION-JCS-1";

        /// <summary>Profil der Proofprojektion (proof-v1 ohne proofHash).</summary>
        public const string ProofJcs1 = "STP-PROOF-JCS-1";

        /// <summary>Historischer Leseeintrag der unveränderten v1-Projektion (keine Berechnung in WP-013).</summary>
        public const string LevelV1PuzzleJcs1 = "STP-LEVEL-V1-PUZZLE-JCS-1";

        private static readonly HashSet<string> KnownProfiles = new HashSet<string>(System.StringComparer.Ordinal)
        {
            PuzzleSemanticJcs1,
            SolutionJcs1,
            ProofJcs1,
            LevelV1PuzzleJcs1,
        };

        /// <summary>Prüft, ob ein Profilname registriert (bekannt) ist.</summary>
        public static bool IsKnown(string profile)
        {
            return profile is not null && KnownProfiles.Contains(profile);
        }

        /// <summary>Registrierte Profile in deterministischer Reihenfolge.</summary>
        public static IReadOnlyList<string> Registered { get; } =
            new[] { PuzzleSemanticJcs1, SolutionJcs1, ProofJcs1, LevelV1PuzzleJcs1 };

        /// <summary>Berechnet den kleingeschriebenen SHA-256-Hexwert der kanonischen Bytes eines Dokuments.</summary>
        public static string Sha256Hex(JsonValue canonicalDocument)
        {
            return Sha256Hex(JcsCanonicalizer.ToUtf8Bytes(canonicalDocument));
        }

        /// <summary>Berechnet den kleingeschriebenen SHA-256-Hexwert beliebiger Bytes.</summary>
        public static string Sha256Hex(byte[] bytes)
        {
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(bytes);
                var builder = new StringBuilder(64);
                foreach (var b in hash)
                {
                    builder.Append(HexDigitsLower[(b >> 4) & 0xF]);
                    builder.Append(HexDigitsLower[b & 0xF]);
                }
                return builder.ToString();
            }
        }

        private const string HexDigitsLower = "0123456789abcdef";
    }
}
