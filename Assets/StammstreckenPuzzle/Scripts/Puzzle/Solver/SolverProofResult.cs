using System;
using System.Collections.Generic;
using STP.Puzzle.Domain;

namespace STP.Puzzle.Solver
{
    /// <summary>
    /// Eine Zelle des gefundenen Lösungspfads (Koordinate plus konkrete
    /// Gleisform), geordnet vom Außenanschluss A zum Außenanschluss B.
    /// </summary>
    public readonly struct SolverPathCell
    {
        /// <summary>Rasterkoordinate.</summary>
        public CellCoordinate Coordinate { get; }

        /// <summary>Konkrete Gleisform.</summary>
        public TrackShape Track { get; }

        /// <summary>Erstellt die Pfadzelle.</summary>
        public SolverPathCell(CellCoordinate coordinate, TrackShape track)
        {
            Coordinate = coordinate;
            Track = track;
        }
    }

    /// <summary>
    /// Typisiertes Proof-Ergebnismodell gemäß der dokumentierten
    /// Modulverantwortung von STP.Puzzle.Solver (Constraintmodell,
    /// Propagation, Suche, Proof und Deduktionsspur): Bündelung von
    /// Lösungsklassifikation, gefundenem Lösungspfad, den dokumentierten
    /// Metriken und der <c>solver-v1</c>-Versionskonstante als Eingabe für
    /// die Proofgenerierung. Constraintsemantik, Tiebreaker,
    /// Wertreihenfolge und die bestehenden Metrikdefinitionen des Solvers
    /// bleiben unverändert; <see cref="MaxDeductionDepth"/> ergänzt die in
    /// SOLVER_ARCHITECTURE.md Abschnitte 5 und 7 dokumentierte, bislang
    /// nicht berechnete Metrik „maximale Deduktionskettentiefe“ rein
    /// additiv (längste Prämissenabhängigkeit der Propagation; statische
    /// Raster-/Endpointreduktionen haben Tiefe 1, jede davon abhängige
    /// Reduktion erhöht die Tiefe um eins; Suchannahmen sind keine
    /// Deduktionen und setzen die Kettentiefe der betroffenen Zelle zurück).
    /// </summary>
    public sealed class SolverProofResult
    {
        /// <summary>Lösungsklassifikation.</summary>
        public SolutionClassification Classification { get; }

        /// <summary>Anzahl gefundener Lösungen, begrenzt auf zwei.</summary>
        public int SolutionCount { get; }

        /// <summary>
        /// Gefundener Lösungspfad von A nach B (nur bei
        /// <see cref="SolutionClassification.Unique"/>), sonst <c>null</c>.
        /// </summary>
        public IReadOnlyList<SolverPathCell>? UniqueSolutionPath { get; }

        /// <summary>Rohmetriken der Ausführung (unveränderte solver-v1-Definitionen).</summary>
        public SolverMetrics Metrics { get; }

        /// <summary>Maximale Deduktionskettentiefe der Propagation (längste Prämissenabhängigkeit).</summary>
        public long MaxDeductionDepth { get; }

        /// <summary>Versionierte Solverkennung (<c>solver-v1</c>).</summary>
        public string SolverVersion { get; }

        /// <summary>Erstellt das Proof-Ergebnismodell.</summary>
        public SolverProofResult(
            SolutionClassification classification,
            int solutionCount,
            IReadOnlyList<SolverPathCell>? uniqueSolutionPath,
            SolverMetrics metrics,
            long maxDeductionDepth,
            string solverVersion)
        {
            Classification = classification;
            SolutionCount = solutionCount;
            UniqueSolutionPath = uniqueSolutionPath;
            Metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
            MaxDeductionDepth = maxDeductionDepth;
            SolverVersion = solverVersion ?? throw new ArgumentNullException(nameof(solverVersion));
        }
    }
}
