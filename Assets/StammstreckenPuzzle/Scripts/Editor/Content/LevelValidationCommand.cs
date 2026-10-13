using System;
using System.Collections.Generic;
using STP.Infrastructure.Content;
using STP.Puzzle.Solver;

namespace STP.Editor.Content
{
    /// <summary>Headless command adapter. Authoring/CI callers supply snapshots and explicit command options.</summary>
    public static class LevelValidationCommand
    {
        /// <summary>--strict is applied to every individual result, before the batch report is built. Unknown options fail closed.</summary>
        public static LevelValidationReport ValidateBatch(IEnumerable<LevelValidationInput> inputs, IEnumerable<string> arguments, SolverLimits? limits = null)
        {
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            bool strict = false;
            foreach (string argument in arguments)
            {
                if (argument != "--strict") throw new ArgumentException("Unknown validation option: " + argument, nameof(arguments));
                strict = true;
            }
            return LevelValidationPipeline.ValidateBatch(inputs, strict, limits);
        }
    }
}
