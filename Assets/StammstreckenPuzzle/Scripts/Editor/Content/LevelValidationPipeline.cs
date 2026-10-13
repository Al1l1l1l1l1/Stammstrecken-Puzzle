using System;
using System.Collections.Generic;
using System.Linq;
using STP.Infrastructure.Content;
using STP.Puzzle.Solver;

namespace STP.Editor.Content
{
    /// <summary>Immutable source snapshot for headless and GUI validation; proof IDs are logical, never opened as paths.</summary>
    public sealed class LevelValidationInput
    {
        private readonly byte[]? _level;
        private readonly byte[]? _proof;
        public string SourcePath { get; }
        public string ArtifactId { get; }
        public byte[]? LevelBytes => _level == null ? null : (byte[])_level.Clone();
        public byte[]? ProofBytes => _proof == null ? null : (byte[])_proof.Clone();
        /// <summary>Additional authoring/cross-reference diagnostics can only add restrictions; strict promotion preserves their codes.</summary>
        public IReadOnlyList<LevelDiagnostic> AdditionalDiagnostics { get; }
        public LevelValidationInput(string sourcePath, byte[]? levelBytes, string artifactId, byte[]? proofBytes, IEnumerable<LevelDiagnostic>? additionalDiagnostics = null)
        {
            SourcePath = Normalize(sourcePath ?? throw new ArgumentNullException(nameof(sourcePath)));
            ArtifactId = artifactId ?? throw new ArgumentNullException(nameof(artifactId));
            _level = levelBytes == null ? null : (byte[])levelBytes.Clone(); _proof = proofBytes == null ? null : (byte[])proofBytes.Clone();
            AdditionalDiagnostics = Array.AsReadOnly((additionalDiagnostics ?? Array.Empty<LevelDiagnostic>()).ToArray());
        }
        private static string Normalize(string path)
        {
            var parts = new List<string>();
            foreach (string part in path.Replace('\\', '/').Split('/'))
            {
                if (part.Length == 0 || part == ".") continue;
                if (part == ".." || part.Contains(":")) throw new ArgumentException("Source must be a repository-relative path.", nameof(path));
                parts.Add(part);
            }
            if (parts.Count == 0 || path.StartsWith("/", StringComparison.Ordinal) || path.StartsWith("\\", StringComparison.Ordinal))
                throw new ArgumentException("Source must be a repository-relative path.", nameof(path));
            return string.Join("/", parts);
        }
    }

    /// <summary>Shared Parse → Schema → Domain → Semantic → Hash → Solver/regeneration → Proof pipeline. No import or release side effects.</summary>
    public static class LevelValidationPipeline
    {
        public static LevelValidationResult ValidateOne(LevelValidationInput input, bool strict = false, SolverLimits? limits = null)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var level = LevelV2Loader.Load(input.LevelBytes);
            return ValidateLoaded(input, level, strict, limits);
        }

        private static LevelValidationResult ValidateLoaded(LevelValidationInput input, LevelV2LoadResult level, bool strict, SolverLimits? limits)
        {
            var diagnostics = new List<LevelDiagnostic>(input.AdditionalDiagnostics);
            LevelValidationResult Result(ProofReadResult? proof = null) => new LevelValidationResult(input.SourcePath, level, proof, diagnostics, strict);
            if (!level.IsValid || diagnostics.Any(d => d.Stage <= LevelDiagnosticStage.Hash && (d.Severity == LevelDiagnosticSeverity.Error || strict))) return Result();
            // Always regenerate from public input. A stored, self-consistent cache cannot substitute for this pass.
            var generated = ProofGenerator.Generate(level, limits);
            diagnostics.AddRange(generated.Diagnostics);
            if (!generated.Succeeded) return Result();
            var gate = CampaignGate.Evaluate(generated.Solver);
            if (gate != CampaignGateOutcome.ELIGIBLE)
            {
                diagnostics.Add(new LevelDiagnostic(LevelDiagnosticStage.Solver, ProofDiagnosticCodes.GuessDepth, string.Empty, "CampaignGate rejected the solver result: " + gate));
                return Result();
            }
            var stored = ProofV1Codec.Read(input.ProofBytes);
            diagnostics.AddRange(stored.Diagnostics);
            if (!stored.Succeeded) return Result();
            diagnostics.AddRange(ProofV1Codec.Bind(level, input.ArtifactId, stored));
            if (diagnostics.Any(d => d.Severity == LevelDiagnosticSeverity.Error)) return Result();
            if (!stored.CanonicalBytes!.SequenceEqual(generated.Proof!.CanonicalBytes!))
            {
                diagnostics.Add(new LevelDiagnostic(LevelDiagnosticStage.Proof, ProofDiagnosticCodes.Nondeterministic, string.Empty,
                    "Stored and freshly regenerated canonical solver-v2 proof bytes differ."));
                return Result();
            }
            return Result(generated.Proof);
        }

        public static LevelValidationReport ValidateBatch(IEnumerable<LevelValidationInput> inputs, bool strict = false, SolverLimits? limits = null)
        {
            var ordered = (inputs ?? throw new ArgumentNullException(nameof(inputs))).OrderBy(i => i.SourcePath, StringComparer.Ordinal)
                .ThenBy(i => LevelHashing.Sha256Hex(i.LevelBytes ?? Array.Empty<byte>()), StringComparer.Ordinal)
                .ThenBy(i => LevelHashing.Sha256Hex(i.ProofBytes ?? Array.Empty<byte>()), StringComparer.Ordinal).ThenBy(i => i.ArtifactId, StringComparer.Ordinal).ToArray();
            var duplicates = new HashSet<string>(ordered.GroupBy(i => i.SourcePath, StringComparer.Ordinal).Where(g => g.Count() > 1).Select(g => g.Key), StringComparer.Ordinal);
            // All sources finish each existing stage before any source enters the next stage (CONTENT_PIPELINE section 5).
            var parsed = ordered.Select(i => StrictJsonParser.Parse(i.LevelBytes)).ToArray();
            var read = parsed.Select(p => p.Value == null ? null : LevelV2Reader.Read(p.Value)).ToArray();
            var mapped = read.Select(r => r?.Document == null ? null : LevelV2DomainMapper.Map(r.Document)).ToArray();
            var semantics = mapped.Select((m, i) => m?.Definition == null ? null : LevelV2Semantics.Validate(read[i]!.Document!, m.Definition)).ToArray();
            var loaded = ordered.Select((input, i) => parsed[i].Value == null ? new LevelV2LoadResult(null, null, new[] { parsed[i].Error! })
                : read[i]!.Document == null ? new LevelV2LoadResult(null, null, read[i]!.Diagnostics)
                : mapped[i]!.Definition == null ? new LevelV2LoadResult(read[i]!.Document, null, mapped[i]!.Diagnostics)
                : new LevelV2LoadResult(read[i]!.Document, mapped[i]!.Definition, semantics[i]!)).ToArray();
            var results = new List<LevelValidationResult>();
            for (int i = 0; i < ordered.Length; i++)
            {
                var input = ordered[i];
                if (duplicates.Contains(input.SourcePath))
                    results.Add(new LevelValidationResult(input.SourcePath, loaded[i], null,
                        new[] { new LevelDiagnostic(LevelDiagnosticStage.Parse, "LVL-SOURCE-DUPLICATE", string.Empty, "Normalized source path occurs more than once.") }, strict));
                else results.Add(ValidateLoaded(input, loaded[i], strict, limits));
            }
            return new LevelValidationReport(results);
        }
    }
}
