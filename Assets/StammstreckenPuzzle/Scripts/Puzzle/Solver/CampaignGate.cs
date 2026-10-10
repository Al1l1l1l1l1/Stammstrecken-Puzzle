namespace STP.Puzzle.Solver
{
    /// <summary>Solver-side verdict for the normal campaign gate (ADR-031 section 4). Not an importer decision.</summary>
    public enum CampaignGateOutcome { ELIGIBLE, GUESS_DEPTH_REQUIRED, NOT_UNIQUE, INDETERMINATE }

    /// <summary>
    /// Fail-closed campaign gate: only a UNIQUE result with verified metrics and
    /// <c>requiredGuessDepth = 0</c> is eligible. Zero, multiple, limited, cancelled,
    /// missing or metric-less results are never eligible.
    /// </summary>
    public static class CampaignGate
    {
        public static CampaignGateOutcome Evaluate(SolverResult? result)
        {
            if (result == null) return CampaignGateOutcome.INDETERMINATE;
            switch (result.Classification)
            {
                case SolverClassification.UNSATISFIABLE:
                case SolverClassification.MULTIPLE_OR_MORE:
                    return CampaignGateOutcome.NOT_UNIQUE;
                case SolverClassification.UNIQUE:
                    if (result.Metrics == null || result.SolutionCount != 1) return CampaignGateOutcome.INDETERMINATE;
                    return result.Metrics.RequiredGuessDepth > 0 ? CampaignGateOutcome.GUESS_DEPTH_REQUIRED : CampaignGateOutcome.ELIGIBLE;
                default:
                    return CampaignGateOutcome.INDETERMINATE;
            }
        }

        public static bool IsEligible(SolverResult? result) => Evaluate(result) == CampaignGateOutcome.ELIGIBLE;
    }
}
