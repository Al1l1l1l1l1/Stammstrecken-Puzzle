namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Typisiertes Artefaktmodell von proof-v1 gemäß
    /// ARCHITECTURE/schemas/proof-v1.schema.json und LEVEL_DATA_FORMAT.md
    /// Abschnitt 7: bindet <c>puzzleId</c>, profilierten Puzzlehash,
    /// profilierten Lösungshash, <c>solverVersion</c>, <c>solutionCount</c>
    /// und die Metriken; der Proofhash schließt alle diese Werte ein.
    /// </summary>
    public sealed class ProofV1Document
    {
        /// <summary>Proofformatversion (Vertrag: konstant 1).</summary>
        public long ProofFormatVersion { get; }

        /// <summary>Fachliche Puzzleidentität.</summary>
        public string PuzzleId { get; }

        /// <summary>Profilierter öffentlicher Puzzlehash (<c>STP-PUZZLE-SEMANTIC-JCS-1</c>).</summary>
        public ProfiledHash PublicPuzzleHash { get; }

        /// <summary>Profilierter Lösungshash (<c>STP-SOLUTION-JCS-1</c>).</summary>
        public ProfiledHash SolutionHash { get; }

        /// <summary>Solverversion (zum Beispiel <c>solver-v1</c>).</summary>
        public string SolverVersion { get; }

        /// <summary>Lösungszahl (Vertrag: konstant 1).</summary>
        public long SolutionCount { get; }

        /// <summary>Proofmetriken.</summary>
        public ProofV1Metrics Metrics { get; }

        /// <summary>Profilierter Proofhash (<c>STP-PROOF-JCS-1</c>) über alle vorgenannten Felder.</summary>
        public ProfiledHash ProofHash { get; }

        /// <summary>Geparste Dokumentwurzel, aus der das Artefakt gelesen wurde (bei Deserialisierung), sonst <c>null</c>.</summary>
        public JsonValue.Object? Source { get; }

        /// <summary>Erstellt das Artefaktmodell.</summary>
        public ProofV1Document(
            long proofFormatVersion,
            string puzzleId,
            ProfiledHash publicPuzzleHash,
            ProfiledHash solutionHash,
            string solverVersion,
            long solutionCount,
            ProofV1Metrics metrics,
            ProfiledHash proofHash,
            JsonValue.Object? source)
        {
            ProofFormatVersion = proofFormatVersion;
            PuzzleId = puzzleId;
            PublicPuzzleHash = publicPuzzleHash;
            SolutionHash = solutionHash;
            SolverVersion = solverVersion;
            SolutionCount = solutionCount;
            Metrics = metrics;
            ProofHash = proofHash;
            Source = source;
        }
    }

    /// <summary>Metriken des proof-v1-Artefakts.</summary>
    public sealed class ProofV1Metrics
    {
        /// <summary>Zahl der Suchknoten.</summary>
        public long SearchNodes { get; }

        /// <summary>Zahl der Deduktionsschritte.</summary>
        public long DeductionSteps { get; }

        /// <summary>Maximale Deduktionskettentiefe.</summary>
        public long MaxDeductionDepth { get; }

        /// <summary>Benötigte Annahmetiefe (0 = rein deduktiv).</summary>
        public long RequiredGuessDepth { get; }

        /// <summary>Erstellt die Metriken.</summary>
        public ProofV1Metrics(long searchNodes, long deductionSteps, long maxDeductionDepth, long requiredGuessDepth)
        {
            SearchNodes = searchNodes;
            DeductionSteps = deductionSteps;
            MaxDeductionDepth = maxDeductionDepth;
            RequiredGuessDepth = requiredGuessDepth;
        }
    }
}
