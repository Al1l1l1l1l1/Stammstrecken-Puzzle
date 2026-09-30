using System;
using System.Collections.Generic;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// JCS-Hashprojektionen gemäß LEVEL_DATA_FORMAT.md Abschnitt 6 und ADR-021.
    /// Die Projektionen werden als UTF-8 ohne BOM und Abschlussnewline
    /// kanonisiert und mit SHA-256 gehasht:
    /// <c>STP-PUZZLE-SEMANTIC-JCS-1</c> über
    /// <c>{puzzleId,rulesetVersion,grid,endpoints,rowCounts,columnCounts}</c>,
    /// <c>STP-SOLUTION-JCS-1</c> über <c>{puzzleId,publicPuzzleHash,path}</c>
    /// und <c>STP-PROOF-JCS-1</c> über das vollständige proof-v1-Objekt ohne
    /// <c>proofHash</c>. Formatierung, Schlüsselreihenfolge,
    /// <c>documentSchemaVersion</c>, <c>contentRevision</c>, Texte, Assets und
    /// Proofregeneration ändern den semantischen Puzzlehash nicht.
    /// </summary>
    public static class PuzzleHashContracts
    {
        /// <summary>
        /// Berechnet den profilierten semantischen Puzzlehash eines
        /// level-v2-Dokuments aus seiner geparsten Dokumentwurzel.
        /// </summary>
        public static ProfiledHash ComputePublicPuzzleHash(JsonValue.Object levelDocument)
        {
            return new ProfiledHash(HashProfiles.PuzzleSemanticJcs1, HashProfiles.Sha256Hex(BuildPublicPuzzleProjection(levelDocument)));
        }

        /// <summary>Baut die öffentliche Puzzleprojektion aus der Dokumentwurzel.</summary>
        public static JsonValue.Object BuildPublicPuzzleProjection(JsonValue.Object levelDocument)
        {
            return new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
            {
                Pair(levelDocument, "puzzleId"),
                Pair(levelDocument, "rulesetVersion"),
                Pair(levelDocument, "grid"),
                Pair(levelDocument, "endpoints"),
                Pair(levelDocument, "rowCounts"),
                Pair(levelDocument, "columnCounts"),
            });
        }

        /// <summary>
        /// Berechnet den profilierten kanonischen Lösungshash über
        /// <c>{puzzleId,publicPuzzleHash,path}</c>.
        /// </summary>
        public static ProfiledHash ComputeSolutionHash(JsonValue puzzleId, JsonValue.Array path, ProfiledHash publicPuzzleHash)
        {
            return new ProfiledHash(HashProfiles.SolutionJcs1, HashProfiles.Sha256Hex(BuildSolutionProjection(puzzleId, path, publicPuzzleHash)));
        }

        /// <summary>Baut die kanonische Lösungsprojektion.</summary>
        public static JsonValue.Object BuildSolutionProjection(JsonValue puzzleId, JsonValue.Array path, ProfiledHash publicPuzzleHash)
        {
            return new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
            {
                new KeyValuePair<string, JsonValue>("puzzleId", puzzleId),
                new KeyValuePair<string, JsonValue>("publicPuzzleHash", BuildProfiledHash(publicPuzzleHash)),
                new KeyValuePair<string, JsonValue>("path", path),
            });
        }

        /// <summary>
        /// Berechnet den profilierten Proofhash über das vollständige
        /// proof-v1-Objekt ohne <c>proofHash</c>.
        /// </summary>
        public static ProfiledHash ComputeProofHash(JsonValue.Object proofDocument)
        {
            return new ProfiledHash(HashProfiles.ProofJcs1, HashProfiles.Sha256Hex(BuildProofProjection(proofDocument)));
        }

        /// <summary>Baut die Proofprojektion (alle Felder außer <c>proofHash</c>).</summary>
        public static JsonValue.Object BuildProofProjection(JsonValue.Object proofDocument)
        {
            var pairs = new List<KeyValuePair<string, JsonValue>>();
            foreach (var pair in proofDocument.Properties)
            {
                if (pair.Key != "proofHash")
                {
                    pairs.Add(pair);
                }
            }
            return new JsonValue.Object(pairs);
        }

        /// <summary>Baut die JSON-Form eines profilierten Hashs (<c>{profile,sha256}</c>).</summary>
        public static JsonValue.Object BuildProfiledHash(ProfiledHash hash)
        {
            return new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
            {
                new KeyValuePair<string, JsonValue>("profile", new JsonValue.String(hash.Profile)),
                new KeyValuePair<string, JsonValue>("sha256", new JsonValue.String(hash.Sha256)),
            });
        }

        private static KeyValuePair<string, JsonValue> Pair(JsonValue.Object document, string key)
        {
            if (!document.TryGet(key, out var value))
            {
                throw new ArgumentException($"Dem Dokument fehlt die für die Hashprojektion erforderliche Eigenschaft '{key}'.", nameof(document));
            }
            return new KeyValuePair<string, JsonValue>(key, value);
        }
    }
}
