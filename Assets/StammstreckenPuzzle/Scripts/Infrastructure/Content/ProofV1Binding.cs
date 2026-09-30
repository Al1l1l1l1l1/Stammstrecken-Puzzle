using System;
using System.Collections.Generic;

namespace STP.Infrastructure.Content
{
    /// <summary>
    /// Bindungsprüfungen der Familie <c>PRF-*</c> gemäß LEVEL_DATA_FORMAT.md
    /// Abschnitte 7 und 8 sowie SOLVER_ARCHITECTURE.md Abschnitt 5:
    /// Puzzlebindung (Copy-Paste eines Proofs auf ein anderes Puzzle wird
    /// erkannt), Lösungsbindung (stale Lösungshash wird erkannt), Lösung
    /// genau eins, Proofhash (Metrikänderungen werden erkannt) und die
    /// <c>proofRef</c>-Bindung des Leveldokuments an Artefakt-ID, Proofformat
    /// und Proofhash. Ein unbekanntes Profil ist ein harter Fehler; kein Hash
    /// wird anhand seines Wertes heuristisch gedeutet.
    /// </summary>
    public static class ProofV1Binding
    {
        /// <summary>
        /// Prüft die Bindung eines proof-v1-Artefakts an den Leveldatensatz
        /// und dessen <c>proofRef</c>. <paramref name="expectedPublicPuzzleHash"/>
        /// und <paramref name="expectedSolutionHash"/> werden aus dem
        /// Leveldatensatz neu berechnet; <paramref name="providedArtifactId"/>
        /// benennt die bereitgestellte Artefaktidentität, der
        /// <c>proofRef.artifactId</c> entsprechen muss.
        /// </summary>
        public static void Validate(
            LevelV2Document level,
            ProofV1Document proof,
            ProfiledHash expectedPublicPuzzleHash,
            ProfiledHash expectedSolutionHash,
            string expectedSolverVersion,
            string providedArtifactId,
            List<LevelDiagnostic> diagnostics)
        {
            if (level is null)
            {
                throw new ArgumentNullException(nameof(level));
            }
            if (proof is null)
            {
                throw new ArgumentNullException(nameof(proof));
            }
            if (diagnostics is null)
            {
                throw new ArgumentNullException(nameof(diagnostics));
            }

            if (proof.PuzzleId != level.PuzzleId)
            {
                diagnostics.Add(LevelDiagnostic.Error("PRF-PUZZLE-ID-MISMATCH", "$.puzzleId", $"Das Proofartefakt bindet an puzzleId '{proof.PuzzleId}', der Datensatz trägt '{level.PuzzleId}'."));
            }

            CheckProfile(proof.PublicPuzzleHash, HashProfiles.PuzzleSemanticJcs1, "PRF-PUBLIC-HASH-PROFILE-MISMATCH", "$.publicPuzzleHash.profile", diagnostics);
            if (proof.PublicPuzzleHash != expectedPublicPuzzleHash)
            {
                diagnostics.Add(LevelDiagnostic.Error("PRF-PUBLIC-HASH-MISMATCH", "$.publicPuzzleHash", "Der öffentliche Puzzlehash des Artefakts stimmt nicht mit dem aus dem Datensatz berechneten Hash überein (Copy-Paste auf ein anderes Puzzle)."));
            }

            CheckProfile(proof.SolutionHash, HashProfiles.SolutionJcs1, "PRF-SOLUTION-HASH-PROFILE-MISMATCH", "$.solutionHash.profile", diagnostics);
            if (proof.SolutionHash != expectedSolutionHash)
            {
                diagnostics.Add(LevelDiagnostic.Error("PRF-SOLUTION-HASH-MISMATCH", "$.solutionHash", "Der Lösungshash des Artefakts stimmt nicht mit dem aus der Authoringlösung berechneten Hash überein (stale Lösungshash)."));
            }

            if (proof.SolutionCount != 1L)
            {
                diagnostics.Add(LevelDiagnostic.Error("PRF-SOLUTION-COUNT", "$.solutionCount", $"solutionCount {proof.SolutionCount} ist unzulässig; verbindlich ist genau 1."));
            }

            if (proof.SolverVersion != expectedSolverVersion)
            {
                diagnostics.Add(LevelDiagnostic.Error("PRF-SOLVER-VERSION-MISMATCH", "$.solverVersion", $"Das Artefakt bindet an solverVersion '{proof.SolverVersion}', geprüft wird '{expectedSolverVersion}'."));
            }

            CheckProfile(proof.ProofHash, HashProfiles.ProofJcs1, "PRF-PROOF-HASH-PROFILE-MISMATCH", "$.proofHash.profile", diagnostics);
            if (proof.Source is not null)
            {
                var recomputed = PuzzleHashContracts.ComputeProofHash(proof.Source);
                if (proof.ProofHash != recomputed)
                {
                    diagnostics.Add(LevelDiagnostic.Error("PRF-PROOF-HASH-MISMATCH", "$.proofHash", "Der Proofhash stimmt nicht mit dem über die Artefaktfelder neu berechneten Hash überein (Metrik- oder Feldänderung)."));
                }
            }

            if (level.ProofRef.ArtifactId != providedArtifactId)
            {
                diagnostics.Add(LevelDiagnostic.Error("PRF-PROOFREF-ARTIFACT-ID-MISMATCH", "$.proofRef.artifactId", $"proofRef.artifactId '{level.ProofRef.ArtifactId}' benennt eine andere Artefaktidentität als die bereitgestellte '{providedArtifactId}'."));
            }
            if (level.ProofRef.ProofFormatVersion != proof.ProofFormatVersion)
            {
                diagnostics.Add(LevelDiagnostic.Error("PRF-PROOFREF-FORMAT-MISMATCH", "$.proofRef.proofFormatVersion", $"proofRef.proofFormatVersion {level.ProofRef.ProofFormatVersion} weicht vom Artefaktformat {proof.ProofFormatVersion} ab."));
            }
            if (level.ProofRef.ProofHash != proof.ProofHash)
            {
                diagnostics.Add(LevelDiagnostic.Error("PRF-PROOFREF-HASH-MISMATCH", "$.proofRef.proofHash", "proofRef.proofHash bindet nicht den Proofhash des Artefakts."));
            }
        }

        private static void CheckProfile(ProfiledHash hash, string expectedProfile, string code, string subject, List<LevelDiagnostic> diagnostics)
        {
            if (!HashProfiles.IsKnown(hash.Profile))
            {
                diagnostics.Add(LevelDiagnostic.Error("LVL-HASH-UNKNOWN-PROFILE", subject, $"Unbekanntes Hashprofil '{hash.Profile}'."));
                return;
            }
            if (hash.Profile != expectedProfile)
            {
                diagnostics.Add(LevelDiagnostic.Error(code, subject, $"Profil '{hash.Profile}' ist an dieser Stelle unzulässig; erwartet wird '{expectedProfile}'."));
            }
        }
    }
}
