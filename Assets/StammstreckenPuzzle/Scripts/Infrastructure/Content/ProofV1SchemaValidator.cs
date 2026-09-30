using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Strukturelle Prüfung von proof-v1-Artefakten gegen das kanonische
    /// Schemadokument ARCHITECTURE/schemas/proof-v1.schema.json
    /// (Draft 2020-12) als zweckgebauter Prüfer für genau diesen Vertrag
    /// (Typen, Pflichtfelder, <c>additionalProperties: false</c>, Konstanten,
    /// Muster, Wertebereiche). Verletzungen tragen stabile Codes der Familie
    /// <c>PRF-FORMAT-*</c>.
    /// </summary>
    public static class ProofV1SchemaValidator
    {
        private static readonly Regex PuzzleIdPattern = new Regex(@"^S[1-9][0-9]*-[0-9]{2}-[0-9]{2}-[0-9]{2}$", RegexOptions.CultureInvariant);
        private static readonly Regex SolverVersionPattern = new Regex(@"^solver-v[1-9][0-9]*$", RegexOptions.CultureInvariant);

        private static readonly string[] RootKeys =
        {
            "proofFormatVersion", "puzzleId", "publicPuzzleHash", "solutionHash",
            "solverVersion", "solutionCount", "metrics", "proofHash",
        };

        /// <summary>
        /// Prüft ein geparstes Artefakt strukturell und erzeugt bei Erfolg
        /// das Artefaktmodell; bei mindestens einer Verletzung ist das
        /// Ergebnis <c>null</c> (fail-closed).
        /// </summary>
        public static ProofV1Document? Validate(JsonValue root, List<LevelDiagnostic> diagnostics)
        {
            var cursor = new LevelV2SchemaValidator.Cursor(diagnostics, "PRF-FORMAT-TYPE", "PRF-FORMAT-ADDITIONAL-PROPERTY", "PRF-FORMAT-MISSING-PROPERTY");
            var rootObject = cursor.Object(root, "$", "PRF-FORMAT-ROOT");
            if (rootObject is null)
            {
                return null;
            }
            cursor.CheckKeys(rootObject, "$", RootKeys);
            cursor.RequireKeys(rootObject, "$", RootKeys);

            var proofFormatVersion = cursor.IntegerMember(rootObject, "$", "proofFormatVersion");
            if (proofFormatVersion.HasValue && proofFormatVersion.Value != 1L)
            {
                cursor.Fail("PRF-FORMAT-VERSION", "$.proofFormatVersion", $"Unbekannte proofFormatVersion {proofFormatVersion.Value}; erwartet wird 1.");
            }

            string? puzzleId = null;
            var puzzleIdText = cursor.StringMember(rootObject, "$", "puzzleId");
            if (puzzleIdText is not null)
            {
                if (!PuzzleIdPattern.IsMatch(puzzleIdText))
                {
                    cursor.Fail("PRF-FORMAT-PUZZLE-ID", "$.puzzleId", $"puzzleId '{puzzleIdText}' verletzt das vertragliche Muster.");
                }
                else
                {
                    puzzleId = puzzleIdText;
                }
            }

            var publicPuzzleHash = LevelV2SchemaValidator.ReadProfiledHash(cursor, rootObject, "publicPuzzleHash", "$.publicPuzzleHash", "PRF-FORMAT-HASH");
            var solutionHash = LevelV2SchemaValidator.ReadProfiledHash(cursor, rootObject, "solutionHash", "$.solutionHash", "PRF-FORMAT-HASH");

            string? solverVersion = null;
            var solverVersionText = cursor.StringMember(rootObject, "$", "solverVersion");
            if (solverVersionText is not null)
            {
                if (!SolverVersionPattern.IsMatch(solverVersionText))
                {
                    cursor.Fail("PRF-FORMAT-SOLVER-VERSION", "$.solverVersion", $"solverVersion '{solverVersionText}' verletzt das vertragliche Muster.");
                }
                else
                {
                    solverVersion = solverVersionText;
                }
            }

            var solutionCount = cursor.IntegerMember(rootObject, "$", "solutionCount");
            if (solutionCount.HasValue && solutionCount.Value != 1L)
            {
                cursor.Fail("PRF-FORMAT-SOLUTION-COUNT", "$.solutionCount", $"solutionCount {solutionCount.Value} ist unzulässig; verbindlich ist 1.");
            }

            ProofV1Metrics? metrics = null;
            var metricsObject = cursor.ObjectMember(rootObject, "$", "metrics");
            if (metricsObject is not null)
            {
                cursor.CheckKeys(metricsObject, "$.metrics", "searchNodes", "deductionSteps", "maxDeductionDepth", "requiredGuessDepth");
                cursor.RequireKeys(metricsObject, "$.metrics", "searchNodes", "deductionSteps", "maxDeductionDepth", "requiredGuessDepth");
                var searchNodes = cursor.IntegerMember(metricsObject, "$.metrics", "searchNodes", 1, null, "PRF-FORMAT-METRICS");
                var deductionSteps = cursor.IntegerMember(metricsObject, "$.metrics", "deductionSteps", 1, null, "PRF-FORMAT-METRICS");
                var maxDeductionDepth = cursor.IntegerMember(metricsObject, "$.metrics", "maxDeductionDepth", 1, null, "PRF-FORMAT-METRICS");
                var requiredGuessDepth = cursor.IntegerMember(metricsObject, "$.metrics", "requiredGuessDepth", 0, null, "PRF-FORMAT-METRICS");
                if (searchNodes.HasValue && deductionSteps.HasValue && maxDeductionDepth.HasValue && requiredGuessDepth.HasValue)
                {
                    metrics = new ProofV1Metrics(searchNodes.Value, deductionSteps.Value, maxDeductionDepth.Value, requiredGuessDepth.Value);
                }
            }

            var proofHash = LevelV2SchemaValidator.ReadProfiledHash(cursor, rootObject, "proofHash", "$.proofHash", "PRF-FORMAT-HASH");

            if (cursor.HasErrors)
            {
                return null;
            }
            return new ProofV1Document(
                proofFormatVersion!.Value,
                puzzleId!,
                publicPuzzleHash!.Value,
                solutionHash!.Value,
                solverVersion!,
                solutionCount!.Value,
                metrics!,
                proofHash!.Value,
                rootObject);
        }
    }
}
