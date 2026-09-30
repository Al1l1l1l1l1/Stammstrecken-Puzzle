using System;
using STP.Infrastructure.Content;
using STP.Puzzle.Solver;

namespace STP.Editor.Content
{
    /// <summary>
    /// Proofgenerierung der Solverstufe gemäß LEVEL_DATA_FORMAT.md Abschnitt 7
    /// und SOLVER_ARCHITECTURE.md Abschnitt 5: Aus dem Solverlauf über den
    /// unveränderten öffentlichen Puzzleinput entsteht das proof-v1-Artefakt
    /// mit <c>solver-v1</c>, <c>solutionCount</c> 1 und den dokumentierten
    /// Metriken; der Proofhash schließt alle Felder außer sich selbst ein.
    /// Gleicher Input und gleiche Solverversion erzeugen bytegleichen Proof
    /// (kanonische JCS-Serialisierung).
    /// </summary>
    public static class ProofGenerator
    {
        /// <summary>
        /// Erzeugt das proof-v1-Artefakt aus einem eindeutigen
        /// Solverlauf. Erfordert <see cref="SolutionClassification.Unique"/>;
        /// der Vergleich der gefundenen Lösung mit der Authoringlösung
        /// erfolgt über den kanonischen Lösungshash durch den Aufrufer.
        /// </summary>
        public static ProofV1Document Generate(
            SolverProofResult run,
            string puzzleId,
            ProfiledHash publicPuzzleHash,
            ProfiledHash solutionHash)
        {
            if (run is null)
            {
                throw new ArgumentNullException(nameof(run));
            }
            if (run.Classification != SolutionClassification.Unique || run.UniqueSolutionPath is null)
            {
                throw new ArgumentException(
                    "Ein proof-v1 kann nur aus einer eindeutigen Lösung erzeugt werden.",
                    nameof(run));
            }

            var metrics = new ProofV1Metrics(
                run.Metrics.SearchNodes,
                run.Metrics.DeductionSteps,
                run.MaxDeductionDepth,
                run.Metrics.RequiredGuessDepth);
            var unsigned = new ProofV1Document(
                proofFormatVersion: 1,
                puzzleId: puzzleId,
                publicPuzzleHash: publicPuzzleHash,
                solutionHash: solutionHash,
                solverVersion: run.SolverVersion,
                solutionCount: 1,
                metrics: metrics,
                proofHash: default,
                source: null);
            var proofHash = ProofV1Serializer.ComputeProofHash(unsigned);
            return new ProofV1Document(
                proofFormatVersion: 1,
                puzzleId: puzzleId,
                publicPuzzleHash: publicPuzzleHash,
                solutionHash: solutionHash,
                solverVersion: run.SolverVersion,
                solutionCount: 1,
                metrics: metrics,
                proofHash: proofHash,
                source: null);
        }

        /// <summary>Erzeugt das Artefakt und serialisiert es in kanonische JCS-Bytes.</summary>
        public static byte[] GenerateCanonicalBytes(
            SolverProofResult run,
            string puzzleId,
            ProfiledHash publicPuzzleHash,
            ProfiledHash solutionHash)
        {
            return ProofV1Serializer.ToCanonicalBytes(Generate(run, puzzleId, publicPuzzleHash, solutionHash));
        }
    }
}
