using System;
using System.Collections.Generic;
using STP.Application;

namespace STP.Infrastructure.Content
{
    /// <summary>Endless-only binding failures; existing campaign diagnostics and profiles retain their contracts.</summary>
    public static class EndlessProofDiagnosticCodes
    {
        public const string Descriptor="PRF-ENDLESS-DESCRIPTOR-MISMATCH";
        public const string GuessDepth="PRF-ENDLESS-GUESS-DEPTH";
    }

    /// <summary>Strict E1-only proof-v1 entry with the unchanged proof/hash projection. A readable proof is not fresh solver authority.</summary>
    public static class EndlessProofV1Codec
    {
        public static ProofReadResult Read(byte[]? bytes)=>ProofV1Core.ReadEndless(bytes);
        public static ProofReadResult Read(JsonValue? json)=>ProofV1Core.ReadEndless(json);

        /// <summary>Serializes actual solver facts and validates the resulting closed E1 artifact.</summary>
        public static ProofReadResult Create(string puzzleId,ProfiledHash puzzleHash,ProfiledHash solutionHash,string solverVersion,
            long searchNodes,long deductionSteps,long maxDeductionDepth,long requiredGuessDepth)
            =>ProofV1Core.CreateEndless(puzzleId,puzzleHash,solutionHash,solverVersion,searchNodes,deductionSteps,maxDeductionDepth,requiredGuessDepth);

        /// <summary>
        /// Binds an E1 proof to the complete descriptor, recomputed public projection and actual STP-SOLUTION-JCS-1 hash.
        /// The caller must compute actualSolutionHash from the actual A/B-ordered solution projection and separately
        /// freshly solve the public input. This method neither runs a solver nor accepts cached uniqueness as authority.
        /// </summary>
        public static IReadOnlyList<LevelDiagnostic> ValidateAgainst(LocalEndlessDescriptor descriptor,PublicPuzzleInput puzzle,
            ProfiledHash actualSolutionHash,ProofReadResult proof)
        {
            if(descriptor==null)throw new ArgumentNullException(nameof(descriptor));
            if(puzzle==null)throw new ArgumentNullException(nameof(puzzle));
            if(actualSolutionHash==null)throw new ArgumentNullException(nameof(actualSolutionHash));
            if(proof==null)throw new ArgumentNullException(nameof(proof));
            if(!proof.Succeeded)return proof.Diagnostics;
            var errors=new List<LevelDiagnostic>();var document=proof.Document!;var canonical=new CanonicalJsonAdapter();
            void Add(string code,string path,string message)=>errors.Add(ProofV1Core.Error(code,path,message));
            if(!(descriptor.Parameters is SearchGeneratorParameters parameters)||descriptor.GeneratorVersion!=PuzzleCandidateSearch.GeneratorVersion)
                Add(EndlessProofDiagnosticCodes.Descriptor,"/descriptor","A complete supported search descriptor is required.");
            else
            {
                var parameterHash=LocalSaveHash.Bytes(LocalSaveHash.Canonical(canonical,parameters.Projection()));
                if(parameterHash!=descriptor.ParameterHashSha256)
                    Add(EndlessProofDiagnosticCodes.Descriptor,"/descriptor/parameterHashSha256","The retained full parameters do not match the descriptor hash.");
            }
            var id=EndlessProjection.DescriptorId(descriptor,canonical);
            if(document.PuzzleId!=id||puzzle.Identity.PuzzleId!=id||descriptor.RulesetVersion!=puzzle.RulesetVersion)
                Add(ProofDiagnosticCodes.Puzzle,"/puzzleId","Proof and public input must bind the exact descriptor-computed E1 identity and ruleset.");
            var publicHash=new ProfiledHash(LevelHashProfiles.PuzzleSemantic,
                LocalSaveHash.Bytes(LocalSaveHash.Canonical(canonical,EndlessProjection.PublicInput(puzzle,false))));
            if(puzzle.Identity.HashProfile!=LevelHashProfiles.PuzzleSemantic)
                Add(ProofDiagnosticCodes.Profile,"/publicPuzzleHash/profile","The public input has the wrong semantic hash profile.");
            if(puzzle.Identity.HashSha256!=publicHash.Sha256||!document.PublicPuzzleHash.Equals(publicHash))
                Add(ProofDiagnosticCodes.Puzzle,"/publicPuzzleHash","Proof and public input must bind the actual public projection hash.");
            if(actualSolutionHash.Profile!=LevelHashProfiles.Solution)
                Add(ProofDiagnosticCodes.Profile,"/solutionHash/profile","The actual solution hash has the wrong profile.");
            else if(!document.SolutionHash.Equals(actualSolutionHash))
                Add(ProofDiagnosticCodes.Solution,"/solutionHash","Proof does not bind the actual solution projection hash.");
            if(document.SolverVersion!="solver-v2")
                Add(ProofDiagnosticCodes.SolverVersion,"/solverVersion","Endless candidate use requires solver-v2.");
            if(document.Metrics.RequiredGuessDepth!=0)
                Add(EndlessProofDiagnosticCodes.GuessDepth,"/metrics/requiredGuessDepth","Endless candidate use requires a NoGuess proof.");
            return LevelDiagnosticList.Freeze(errors);
        }
    }
}
