using System;
using System.Collections.Generic;
using STP.Puzzle.Domain;

namespace STP.Infrastructure.Content
{
    /// <summary>State of a loaded document with respect to the runtime catalog.</summary>
    public enum LevelImportState
    {
        /// <summary>At least one parse, schema, mapping or semantic diagnostic exists.</summary>
        Rejected,
        /// <summary>Technically valid, but no proof artifact is bound and no release lock exists: not importable (closed by the later proof block).</summary>
        AwaitingProofGate
    }

    /// <summary>Result of the complete Parse, Schema, Domain-map and Semantic pipeline for one Level-v2 source.</summary>
    public sealed class LevelV2LoadResult
    {
        /// <summary>The typed document once the schema stage passed (also set when later stages report diagnostics).</summary>
        public LevelV2Document? Document { get; }
        /// <summary>The integrated Domain definition once the mapping stage passed.</summary>
        public PuzzleDefinition? Definition { get; }
        /// <summary>Content-Hash (<c>documentSha256</c>); only set for valid documents.</summary>
        public string? ContentHash { get; }
        /// <summary>Public-Puzzle-Hash (<c>STP-PUZZLE-SEMANTIC-JCS-1</c>); only set for valid documents.</summary>
        public ProfiledHash? PublicPuzzleHash { get; }
        /// <summary>Solution hash (<c>STP-SOLUTION-JCS-1</c>); only set for valid documents.</summary>
        public ProfiledHash? SolutionHash { get; }
        public IReadOnlyList<LevelDiagnostic> Diagnostics { get; }
        public bool IsValid => Diagnostics.Count == 0 && Document != null && Definition != null;
        public LevelImportState ImportState => IsValid ? LevelImportState.AwaitingProofGate : LevelImportState.Rejected;
        /// <summary>Always false in this work package: a Level-v2 source can never be imported without the later proof gate.</summary>
        public bool IsRuntimeImportable => false;
        /// <summary>Why the source cannot enter a runtime catalog: its diagnostics, or the explicit missing proof gate for a valid source.</summary>
        public IReadOnlyList<LevelDiagnostic> ImportBlockers { get; }

        internal LevelV2LoadResult(LevelV2Document? document, PuzzleDefinition? definition, IReadOnlyList<LevelDiagnostic> diagnostics)
        {
            Document = document; Definition = definition; Diagnostics = diagnostics;
            if (diagnostics.Count == 0 && document != null && definition != null)
            {
                ContentHash = LevelHashing.ComputeContentHash(document);
                PublicPuzzleHash = LevelHashing.ComputePublicPuzzleHash(document);
                SolutionHash = LevelHashing.ComputeSolutionHash(document);
                ImportBlockers = new[]
                {
                    new LevelDiagnostic(LevelDiagnosticStage.Import, LevelDiagnosticCodes.ImportProofGateMissing, "/proofRef",
                        "No proof artifact is loaded, generated or bound and no release lock exists; the proof gate is closed by the later Solver-v2/Proof block.")
                };
            }
            else ImportBlockers = diagnostics;
        }
    }

    /// <summary>
    /// Entry point of the Level-v2 pipeline (CONTENT_PIPELINE.md Abschnitt 5, stages Parse, Schema, Domain map, Semantic).
    /// A later stage never masks an earlier failure. No file access, no catalog registration, no state.
    /// </summary>
    public static class LevelV2Loader
    {
        public static LevelV2LoadResult Load(byte[]? utf8, JsonParserLimits? limits = null)
        {
            var parsed = StrictJsonParser.Parse(utf8, limits);
            return parsed.Value == null ? new LevelV2LoadResult(null, null, new[] { parsed.Error! }) : Load(parsed.Value);
        }

        public static LevelV2LoadResult LoadText(string? text, JsonParserLimits? limits = null)
        {
            var parsed = StrictJsonParser.ParseText(text, limits);
            return parsed.Value == null ? new LevelV2LoadResult(null, null, new[] { parsed.Error! }) : Load(parsed.Value);
        }

        public static LevelV2LoadResult Load(JsonValue? json)
        {
            var read = LevelV2Reader.Read(json);
            if (read.Document == null) return new LevelV2LoadResult(null, null, read.Diagnostics);
            var mapped = LevelV2DomainMapper.Map(read.Document);
            if (mapped.Definition == null) return new LevelV2LoadResult(read.Document, null, mapped.Diagnostics);
            var semantic = LevelV2Semantics.Validate(read.Document, mapped.Definition);
            return new LevelV2LoadResult(read.Document, mapped.Definition, semantic);
        }
    }
}
