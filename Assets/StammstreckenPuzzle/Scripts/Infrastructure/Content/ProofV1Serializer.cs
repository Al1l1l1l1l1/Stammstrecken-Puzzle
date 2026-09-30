using System.Collections.Generic;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Serialisierung und Deserialisierung des proof-v1-Artefaktmodells. Die
    /// Serialisierung erzeugt die kanonische JCS-Form (UTF-8 ohne BOM, ohne
    /// Abschlussnewline); gleiche Feldwerte ergeben damit bytegleiche
    /// Artefakte (LEVEL_DATA_FORMAT.md Abschnitt 7).
    /// </summary>
    public static class ProofV1Serializer
    {
        /// <summary>Baut die Dokumentform des Artefakts (alle Felder einschließlich proofHash).</summary>
        public static JsonValue.Object ToJson(ProofV1Document document)
        {
            var projection = ToJsonWithoutProofHash(document);
            var pairs = new List<KeyValuePair<string, JsonValue>>(projection.Properties)
            {
                new KeyValuePair<string, JsonValue>("proofHash", PuzzleHashContracts.BuildProfiledHash(document.ProofHash)),
            };
            return new JsonValue.Object(pairs);
        }

        /// <summary>Baut die Proofprojektion (alle Felder außer proofHash).</summary>
        public static JsonValue.Object ToJsonWithoutProofHash(ProofV1Document document)
        {
            return new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
            {
                new KeyValuePair<string, JsonValue>("proofFormatVersion", new JsonValue.Integer(document.ProofFormatVersion)),
                new KeyValuePair<string, JsonValue>("puzzleId", new JsonValue.String(document.PuzzleId)),
                new KeyValuePair<string, JsonValue>("publicPuzzleHash", PuzzleHashContracts.BuildProfiledHash(document.PublicPuzzleHash)),
                new KeyValuePair<string, JsonValue>("solutionHash", PuzzleHashContracts.BuildProfiledHash(document.SolutionHash)),
                new KeyValuePair<string, JsonValue>("solverVersion", new JsonValue.String(document.SolverVersion)),
                new KeyValuePair<string, JsonValue>("solutionCount", new JsonValue.Integer(document.SolutionCount)),
                new KeyValuePair<string, JsonValue>("metrics", new JsonValue.Object(new List<KeyValuePair<string, JsonValue>>
                {
                    new KeyValuePair<string, JsonValue>("searchNodes", new JsonValue.Integer(document.Metrics.SearchNodes)),
                    new KeyValuePair<string, JsonValue>("deductionSteps", new JsonValue.Integer(document.Metrics.DeductionSteps)),
                    new KeyValuePair<string, JsonValue>("maxDeductionDepth", new JsonValue.Integer(document.Metrics.MaxDeductionDepth)),
                    new KeyValuePair<string, JsonValue>("requiredGuessDepth", new JsonValue.Integer(document.Metrics.RequiredGuessDepth)),
                })),
            });
        }

        /// <summary>Berechnet den Proofhash über die Artefaktfelder (STP-PROOF-JCS-1 über alle Felder außer proofHash).</summary>
        public static ProfiledHash ComputeProofHash(ProofV1Document document)
        {
            return new ProfiledHash(HashProfiles.ProofJcs1, HashProfiles.Sha256Hex(ToJsonWithoutProofHash(document)));
        }

        /// <summary>Serialisiert das Artefakt in kanonische JCS-Bytes.</summary>
        public static byte[] ToCanonicalBytes(ProofV1Document document)
        {
            return JcsCanonicalizer.ToUtf8Bytes(ToJson(document));
        }

        /// <summary>Parst und prüft ein Artefakt aus rohen Bytes (Parse- und Formatstufe).</summary>
        public static ProofV1Document? Deserialize(byte[] bytes, JsonParseLimits limits, List<LevelDiagnostic> diagnostics)
        {
            JsonValue parsed;
            try
            {
                parsed = StrictJsonParser.ParseUtf8(bytes, limits);
            }
            catch (StrictJsonException ex)
            {
                diagnostics.Add(LevelDiagnostic.Error(ex.Code, "$", ex.Message));
                return null;
            }
            return ProofV1SchemaValidator.Validate(parsed, diagnostics);
        }
    }
}
