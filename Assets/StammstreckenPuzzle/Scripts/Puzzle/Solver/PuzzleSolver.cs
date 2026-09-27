using System;
using STP.Puzzle.Domain;

namespace STP.Puzzle.Solver
{
    /// <summary>
    /// Deterministischer Constraint-Solver gemäß SOLVER_ARCHITECTURE.md und
    /// ADR-007. Er konsumiert ausschließlich die öffentliche
    /// <see cref="PuzzleDefinition"/> und zählt Lösungen bis zum Limit zwei mit
    /// Depth-First Search, Minimum Remaining Values und festgelegten
    /// Tiebreakern. Er kennt weder eine gespeicherte Authoringlösung noch
    /// Unity und trifft keine UI-, Sterne- oder Werbeentscheidung.
    /// </summary>
    public static class PuzzleSolver
    {
        /// <summary>
        /// Versionierte Solverkennung (`solver-v1`): umfasst
        /// Constraintsemantik, Tiebreaker, Wertreihenfolge und
        /// Metrikdefinitionen.
        /// </summary>
        public const string SolverVersion = "solver-v1";

        /// <summary>Standardbudget an Suchknoten; Überschreitung ergibt <see cref="SolutionClassification.Indeterminate"/>.</summary>
        public const long DefaultMaxSearchNodes = 200_000;

        /// <summary>
        /// Löst das Puzzle aus dem unveränderten öffentlichen Puzzleinput und
        /// klassifiziert die Lösungsmenge: keine Lösung, genau eine Lösung,
        /// zwei oder mehr, oder unbestimmt bei erschöpftem Ressourcenlimit.
        /// Für identische Eingaben entstehen identische Ergebnisse und Metriken.
        /// </summary>
        public static SolverResult Solve(
            PuzzleDefinition definition,
            long maxSearchNodes = DefaultMaxSearchNodes)
        {
            if (definition is null)
            {
                throw new ArgumentNullException(nameof(definition));
            }
            if (maxSearchNodes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSearchNodes));
            }

            var search = new SearchRun(definition, maxSearchNodes);
            return search.Execute();
        }

        private sealed class SearchRun
        {
            private readonly PuzzleDefinition _definition;
            private readonly long _maxSearchNodes;
            private long _searchNodes;
            private bool _budgetExceeded;
            private int _solutions;
            private CellContent[]? _firstSolution;
            private int _firstSolutionGuessDepth;
            private long _deductionSteps;

            internal SearchRun(PuzzleDefinition definition, long maxSearchNodes)
            {
                _definition = definition;
                _maxSearchNodes = maxSearchNodes;
            }

            internal SolverResult Execute()
            {
                var core = SolverCore.CreateInitial(_definition, out _deductionSteps);
                if (core is not null)
                {
                    Search(core, guessDepth: 0);
                }

                var pathLength = 0;
                if (_firstSolution is not null)
                {
                    foreach (var cell in _firstSolution)
                    {
                        if (CellContentSemantics.IsConcreteTrack(cell))
                        {
                            pathLength++;
                        }
                    }
                }

                var classification = _budgetExceeded
                    ? SolutionClassification.Indeterminate
                    : _solutions switch
                    {
                        0 => SolutionClassification.Unsatisfiable,
                        1 => SolutionClassification.Unique,
                        _ => SolutionClassification.MultipleOrMore,
                    };

                return new SolverResult(
                    classification,
                    _solutions,
                    classification == SolutionClassification.Unique ? _firstSolution : null,
                    new SolverMetrics(_searchNodes, _deductionSteps, _solutions > 0 ? _firstSolutionGuessDepth : 0, pathLength));
            }

            private void Search(SolverCore core, int guessDepth)
            {
                if (_solutions >= 2 || _budgetExceeded)
                {
                    return;
                }

                var cellIndex = core.MostConstrainedCell();
                if (cellIndex < 0)
                {
                    var cells = core.ExtractCells();
                    if (CompletionEvaluator.IsSolved(_definition, cells))
                    {
                        if (_solutions == 0)
                        {
                            _firstSolution = cells;
                            _firstSolutionGuessDepth = guessDepth;
                        }
                        _solutions++;
                    }
                    return;
                }

                foreach (var value in CellDomain.OrderedValues(core.Domains[cellIndex]))
                {
                    if (_solutions >= 2 || _budgetExceeded)
                    {
                        return;
                    }
                    if (_searchNodes >= _maxSearchNodes)
                    {
                        _budgetExceeded = true;
                        return;
                    }
                    _searchNodes++;
                    var next = core.Clone();
                    if (next.Assign(cellIndex, value))
                    {
                        Search(next, guessDepth + 1);
                    }
                }
                _deductionSteps = core.DeductionSteps;
            }
        }
    }
}
