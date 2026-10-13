using System;
using System.Collections.Generic;
using System.Linq;

namespace STP.Infrastructure.Content
{
    /// <summary>One immutable validation result. Strict promotion happens here, before status and counters are computed.</summary>
    public sealed class LevelValidationResult
    {
        public string SourcePath { get; }
        public LevelV2LoadResult Level { get; }
        public ProofReadResult? Proof { get; }
        public bool Strict { get; }
        public IReadOnlyList<LevelDiagnostic> Diagnostics { get; }
        public int ErrorCount { get; }
        public int WarningCount { get; }
        public bool Succeeded => Level.IsValid && Proof != null && Proof.Succeeded && ErrorCount == 0;
        public bool ProofGatePassed => Succeeded;
        public LevelDiagnosticStage? FailedStage { get; }
        /// <summary>Remaining cross-reference, product and release-lock gates are outside this WP and stay closed.</summary>
        public bool IsRuntimeImportable => false;
        public bool IsReleaseLockEligible => false;
        public IReadOnlyList<LevelDiagnostic> ImportBlockers { get; }

        internal LevelValidationResult(string sourcePath, LevelV2LoadResult level, ProofReadResult? proof,
            IEnumerable<LevelDiagnostic> diagnostics, bool strict)
        {
            SourcePath = sourcePath ?? throw new ArgumentNullException(nameof(sourcePath));
            Level = level ?? throw new ArgumentNullException(nameof(level)); Strict = strict;
            var effective = new List<LevelDiagnostic>(level.Diagnostics);
            effective.AddRange(diagnostics ?? throw new ArgumentNullException(nameof(diagnostics)));
            if (proof != null) effective.AddRange(proof.Diagnostics);
            if (level.IsValid && proof == null && !effective.Any(d => d.Severity == LevelDiagnosticSeverity.Error || strict))
                effective.Add(new LevelDiagnostic(LevelDiagnosticStage.Proof, ProofDiagnosticCodes.Missing, "/proofRef", "No validated regenerated proof."));
            var ordered = effective.Select(d => strict && d.Severity == LevelDiagnosticSeverity.Warning
                    ? new LevelDiagnostic(d.Stage, d.Code, d.Path, d.Message, LevelDiagnosticSeverity.Error) : d)
                .Distinct().OrderBy(d => d.Stage).ThenBy(d => d.Path, StringComparer.Ordinal)
                .ThenBy(d => d.Code, StringComparer.Ordinal).ThenBy(d => d.Message, StringComparer.Ordinal).ThenBy(d => d.Severity).ToArray();
            Diagnostics = Array.AsReadOnly(ordered);
            ErrorCount = ordered.Count(d => d.Severity == LevelDiagnosticSeverity.Error);
            WarningCount = ordered.Length - ErrorCount;
            FailedStage = ordered.FirstOrDefault(d => d.Severity == LevelDiagnosticSeverity.Error)?.Stage;
            Proof = ErrorCount == 0 ? proof : null;
            ImportBlockers = Succeeded ? Array.AsReadOnly(new[] {
                new LevelDiagnostic(LevelDiagnosticStage.Import, "LVL-IMPORT-RELEASE-GATE-MISSING", string.Empty,
                    "Proof passed; cross-reference, product approval and release-lock validation are still required.") }) : Diagnostics;
        }
    }

    /// <summary>Deterministically ordered batch results. Counters and success are derived solely from the individual results.</summary>
    public sealed class LevelValidationReport
    {
        public IReadOnlyList<LevelValidationResult> Results { get; }
        public int ErrorCount { get; }
        public int WarningCount { get; }
        public int FailedCount { get; }
        public bool Succeeded => FailedCount == 0;
        public LevelDiagnosticStage? FailedStage { get; }
        public LevelValidationReport(IEnumerable<LevelValidationResult> results)
        {
            var ordered = (results ?? throw new ArgumentNullException(nameof(results))).OrderBy(r => r.SourcePath, StringComparer.Ordinal).ToArray();
            Results = Array.AsReadOnly(ordered); ErrorCount = ordered.Sum(r => r.ErrorCount); WarningCount = ordered.Sum(r => r.WarningCount);
            FailedCount = ordered.Count(r => !r.Succeeded);
            FailedStage = ordered.Where(r => r.FailedStage.HasValue).Select(r => r.FailedStage).OrderBy(s => s).FirstOrDefault();
        }
    }
}
