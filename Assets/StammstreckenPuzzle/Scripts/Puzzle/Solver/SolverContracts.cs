using System;
using STP.Puzzle.Domain;
using System.Collections.Generic;

namespace STP.Puzzle.Solver
{
    /// <summary>
    /// Lösungsklassifikation gemäß SOLVER_ARCHITECTURE.md Abschnitt 4.
    /// </summary>
    public enum SolutionClassification : byte
    {
        /// <summary>Keine Lösung.</summary>
        Unsatisfiable = 0,

        /// <summary>Genau eine Lösung.</summary>
        Unique = 1,

        /// <summary>Zwei oder mehr Lösungen (Zählung bricht bei zwei ab).</summary>
        MultipleOrMore = 2,

        /// <summary>Ressourcenlimit erreicht; niemals als eindeutig deuten.</summary>
        Indeterminate = 3,
    }

    /// <summary>
    /// Deterministische Rohmetriken des Solvers gemäß SOLVER_ARCHITECTURE.md
    /// Abschnitt 7. Sie sind keine endgültige menschliche Schwierigkeit.
    /// </summary>
    public sealed class SolverMetrics
    {
        /// <summary>Zahl der Suchknoten (Wertannahmen in der Tiefensuche).</summary>
        public long SearchNodes { get; }

        /// <summary>Zahl deduplizierter Kandidatenreduktionen durch Propagation.</summary>
        public long DeductionSteps { get; }

        /// <summary>Annahmetiefe, auf der die erste Lösung gefunden wurde (0 = rein deduktiv).</summary>
        public int RequiredGuessDepth { get; }

        /// <summary>Belegte Zellen der eindeutigen Lösung (0, wenn keine eindeutige Lösung vorliegt).</summary>
        public int PathLength { get; }

        /// <summary>Erstellt die Metriken.</summary>
        public SolverMetrics(long searchNodes, long deductionSteps, int requiredGuessDepth, int pathLength)
        {
            SearchNodes = searchNodes;
            DeductionSteps = deductionSteps;
            RequiredGuessDepth = requiredGuessDepth;
            PathLength = pathLength;
        }
    }

    /// <summary>
    /// Ergebnis einer Solverausführung aus dem öffentlichen Puzzleinput.
    /// </summary>
    public sealed class SolverResult
    {
        /// <summary>Lösungsklassifikation.</summary>
        public SolutionClassification Classification { get; }

        /// <summary>Anzahl gefundener Lösungen, begrenzt auf zwei.</summary>
        public int SolutionCount { get; }

        /// <summary>
        /// Zellstand der eindeutigen Lösung (konkrete Gleise, sonst
        /// <see cref="CellContent.Unset"/>) oder <c>null</c>, wenn die
        /// Klassifikation nicht <see cref="SolutionClassification.Unique"/> ist.
        /// </summary>
        public IReadOnlyList<CellContent>? UniqueSolution { get; }

        /// <summary>Rohmetriken der Ausführung.</summary>
        public SolverMetrics Metrics { get; }

        /// <summary>Erstellt das Ergebnis.</summary>
        public SolverResult(
            SolutionClassification classification,
            int solutionCount,
            IReadOnlyList<CellContent>? uniqueSolution,
            SolverMetrics metrics)
        {
            Classification = classification;
            SolutionCount = solutionCount;
            UniqueSolution = uniqueSolution;
            Metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
        }
    }
}
