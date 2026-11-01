using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using STP.Application;
using STP.Infrastructure.Content;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

internal static class Program
{
    private static readonly JsonSerializerOptions Pretty = new JsonSerializerOptions { WriteIndented = true };
    private static readonly ICanonicalJson Canonical = new CanonicalJsonAdapter();
    private static string Hash(byte[] data) => LocalSaveHash.Bytes(data);
    private static byte[] Jcs(string text) => LocalSaveHash.Canonical(Canonical, text);
    private static void Write(string path, JsonNode value) => File.WriteAllText(path, value.ToJsonString(Pretty).Replace("\r\n", "\n") + "\n");
    private static Direction Toward(CellCoordinate from, CellCoordinate to) => to.Y < from.Y ? Direction.N : to.X > from.X ? Direction.E : to.Y > from.Y ? Direction.S : Direction.W;
    private static JsonObject PathCell(PuzzleDefinition definition, SolverResult result, int index)
    {
        var cell = result.Path[index];
        var incoming = index == 0 ? definition.A.Side : Toward(cell, result.Path[index - 1]);
        var outgoing = index == result.Path.Count - 1 ? definition.B.Side : Toward(cell, result.Path[index + 1]);
        int ports = (1 << (int)incoming) | (1 << (int)outgoing);
        int shape = Enumerable.Range(0, 6).Single(s => TrackGeometry.Ports((TrackShape)s) == ports);
        return new JsonObject { ["x"] = cell.X, ["y"] = cell.Y, ["track"] = "TRACK_" + ((TrackShape)shape).ToString() };
    }
    private static async Task<int> Main(string[] args)
    {
        try
        {
            if (args.Length != 1) throw new ArgumentException("One new output directory is required");
            var output = Path.GetFullPath(args[0]);
            if (Directory.Exists(output) || File.Exists(output)) throw new IOException("Output already exists");
            Directory.CreateDirectory(output);
            var ui = Path.Combine(Directory.GetCurrentDirectory(), "Assets/StammstreckenPuzzle/UI");
            var bindingBytes = File.ReadAllBytes(Path.Combine(ui, "MobileCandidate.bindings.json"));
            var binding = JsonNode.Parse(bindingBytes)!;
            var corpusBytes = File.ReadAllBytes(Path.Combine(ui, "MobileCandidate.public.json"));
            var corpus = new MobileCandidateData(corpusBytes, (string)binding["publicCatalogHashSha256"]!);
            var profileBytes = File.ReadAllBytes(Path.Combine(ui, "MobileCandidate.endless-profiles.json"));
            var profiles = new LockedEndlessProfiles(profileBytes, corpus).Profiles;
            var definitions = corpus.Puzzles.Select(p => p.ToDomainDefinition()).ToArray();
            var history = new List<PublicPuzzleInput>();
            var all = new JsonArray();
            var selected = new JsonArray();
            var attempts = new JsonArray();
            var sourceFiles = new JsonArray();
            foreach (var file in new[] { "MobileCandidate.public.json", "MobileCandidate.bindings.json", "MobileCandidate.endless-profiles.json", "MobileCandidate.endless-bindings.json" })
                sourceFiles.Add(new JsonObject { ["path"] = "Assets/StammstreckenPuzzle/UI/" + file, ["sha256"] = Hash(File.ReadAllBytes(Path.Combine(ui, file))) });
            foreach (var file in new[] { "Assets/StammstreckenPuzzle/Scripts/Application/PuzzleCandidateSearch.cs", "Assets/StammstreckenPuzzle/Scripts/Puzzle/Solver/PuzzleSolver.cs", "Assets/StammstreckenPuzzle/Scripts/Application/SearchGeneratorParameters.cs", "tools/campaign/EndlessSample/Program.cs", "tools/campaign/EndlessSample/EndlessSample.csproj", "Assets/StammstreckenPuzzle/Scripts/Infrastructure/Content/ProofV1.cs", "Assets/StammstreckenPuzzle/Scripts/Infrastructure/Content/EndlessProofV1Codec.cs" })
                sourceFiles.Add(new JsonObject { ["path"] = file, ["sha256"] = Hash(File.ReadAllBytes(file)) });
            for (int profileIndex = 0; profileIndex < profiles.Count; profileIndex++)
            {
                var profile = profiles[profileIndex];
                var pool = new List<(PublicPuzzleInput Puzzle, JsonObject Record, int Steps, int Depth)>();
                for (int trial = 0; trial < 96 && pool.Count < 24; trial++)
                {
                    ulong seed = (ulong)(202610210000L + profileIndex * 10000L + trial);
                    var recent = history.Skip(Math.Max(0, history.Count - 50)).ToArray();
                    var parameters = new SearchGeneratorParameters(profile.Id, profile.HashSha256, PuzzleCandidateSearch.NoveltyVersion, corpus.Identity, recent, profile.Search);
                    var parameterBytes = Jcs(parameters.Projection());
                    var descriptor = LocalEndlessDescriptor.Create("train-track-v1", PuzzleCandidateSearch.GeneratorVersion, seed.ToString(CultureInfo.InvariantCulture), (history.Count + 1).ToString(CultureInfo.InvariantCulture), parameters, Hash(parameterBytes));
                    string id = EndlessProjection.DescriptorId(descriptor, Canonical);
                    var timer = System.Diagnostics.Stopwatch.StartNew();
                    var candidate = await PuzzleCandidateSearch.GenerateAsync(seed, profile.Search, definitions, recent.Select(p => p.ToDomainDefinition()).ToArray(), CancellationToken.None);
                    attempts.Add(new JsonObject { ["profileId"] = profile.Id, ["seed"] = seed.ToString(CultureInfo.InvariantCulture), ["status"] = candidate.Status, ["searchAttempts"] = candidate.Attempts, ["observedWallMs"] = timer.ElapsedMilliseconds });
                    if (!candidate.Succeeded) { Console.WriteLine(profile.Id + " rejected seed=" + seed + " " + candidate.Status); continue; }
                    var definition = candidate.Puzzle!;
                    var fresh = PuzzleSolver.Solve(definition, new SolverLimits(10000000, 60000));
                    if (fresh.Classification != SolverClassification.UNIQUE || fresh.Metrics == null || fresh.Metrics.RequiredGuessDepth != 0) throw new InvalidOperationException("Independent fresh verification failed");
                    var temporary = new PublicPuzzleInput(new PublicPuzzleIdentity(id, PublicPuzzleIdentity.SemanticHashProfile, new string('0', 64)), definition.RulesetVersion, definition.Grid.Width, definition.Grid.Height, (PublicPuzzleSide)definition.A.Side, definition.A.Index, (PublicPuzzleSide)definition.B.Side, definition.B.Index, definition.RowCounts, definition.ColumnCounts);
                    string publicHash = Hash(Jcs(EndlessProjection.PublicInput(temporary, false)));
                    var puzzle = new PublicPuzzleInput(new PublicPuzzleIdentity(id, PublicPuzzleIdentity.SemanticHashProfile, publicHash), temporary.RulesetVersion, temporary.Width, temporary.Height, temporary.A.Side, temporary.A.Index, temporary.B.Side, temporary.B.Index, temporary.RowCounts, temporary.ColumnCounts);
                    EndlessProjection.ValidatePublic(puzzle, Canonical);
                    var publicBytes = Jcs(EndlessProjection.PublicInput(puzzle, true));
                    var path = new JsonArray(Enumerable.Range(0, fresh.Path.Count).Select(i => (JsonNode)PathCell(definition, fresh, i)).ToArray());
                    var solution = new JsonObject { ["puzzleId"] = id, ["publicPuzzleHash"] = new JsonObject { ["profile"] = PublicPuzzleIdentity.SemanticHashProfile, ["sha256"] = publicHash }, ["path"] = path };
                    var solutionBytes = Jcs(solution.ToJsonString());
                    var m = fresh.Metrics;
                    var solutionHash = new ProfiledHash(LevelHashProfiles.Solution, Hash(solutionBytes));
                    var proof = EndlessProofV1Codec.Create(id, new ProfiledHash(PublicPuzzleIdentity.SemanticHashProfile, publicHash), solutionHash, fresh.SolverVersion, m.SearchNodes, m.DeductionSteps, m.MaxDeductionDepth, m.RequiredGuessDepth);
                    if (!proof.Succeeded) throw new InvalidOperationException("Existing proof codec rejected actual solver proof: " + string.Join(";", proof.Diagnostics.Select(d => d.Code + ":" + d.Message)));
                    var bindingErrors = EndlessProofV1Codec.ValidateAgainst(descriptor, puzzle, solutionHash, proof);
                    if (bindingErrors.Count != 0) throw new InvalidOperationException("Endless proof binding rejected: " + string.Join(";", bindingErrors.Select(d => d.Code)));
                    var dir = Path.Combine(output, profile.Id, id); Directory.CreateDirectory(dir);
                    File.WriteAllBytes(Path.Combine(dir, "public.json"), publicBytes);
                    File.WriteAllBytes(Path.Combine(dir, "parameters.json"), parameterBytes);
                    var descriptorJson = new JsonObject { ["endlessContractVersion"] = 1, ["rulesetVersion"] = descriptor.RulesetVersion, ["generatorVersion"] = descriptor.GeneratorVersion, ["seed"] = descriptor.Seed, ["generationOrdinal"] = descriptor.GenerationOrdinal, ["parameterHashSha256"] = descriptor.ParameterHashSha256 };
                    File.WriteAllBytes(Path.Combine(dir, "descriptor.internal.json"), Jcs(descriptorJson.ToJsonString()));
                    File.WriteAllBytes(Path.Combine(dir, "solution.internal.json"), solutionBytes);
                    File.WriteAllBytes(Path.Combine(dir, "proof.internal.json"), proof.CanonicalBytes!);
                    var record = new JsonObject { ["profileId"] = profile.Id, ["profileHashSha256"] = profile.HashSha256, ["seed"] = descriptor.Seed, ["generationOrdinal"] = descriptor.GenerationOrdinal, ["parameterHashSha256"] = descriptor.ParameterHashSha256, ["puzzleId"] = id, ["publicPuzzleHashSha256"] = publicHash, ["publicSourceSha256"] = Hash(publicBytes), ["solutionHashSha256"] = Hash(solutionBytes), ["proofHashSha256"] = proof.Document!.ProofHash.Sha256, ["proofSourceSha256"] = Hash(proof.CanonicalBytes!), ["width"] = puzzle.Width, ["height"] = puzzle.Height, ["deductionSteps"] = m.DeductionSteps, ["maxDeductionDepth"] = m.MaxDeductionDepth, ["requiredGuessDepth"] = m.RequiredGuessDepth, ["publicFile"] = Path.GetRelativePath(output, Path.Combine(dir, "public.json")).Replace('\\', '/') };
                    Write(Path.Combine(dir, "source-audit.internal.json"), record);
                    pool.Add((puzzle, record, checked((int)m.DeductionSteps), checked((int)m.MaxDeductionDepth)));
                    history.Add(puzzle); all.Add(record.DeepClone());
                    Console.WriteLine(profile.Id + " " + pool.Count + "/24 " + puzzle.Width + "x" + puzzle.Height + " steps=" + m.DeductionSteps + " depth=" + m.MaxDeductionDepth + " seed=" + seed);
                }
                if (pool.Count < 24) throw new InvalidOperationException("Bounded pool incomplete: " + profile.Id + " " + pool.Count);
                var ranked = pool.OrderBy(x => x.Puzzle.Width * x.Puzzle.Height).ThenBy(x => x.Steps).ThenBy(x => x.Depth).ThenBy(x => x.Puzzle.Identity.PuzzleId, StringComparer.Ordinal).ToArray();
                for (int i = 0; i < 12; i++)
                {
                    var item = (JsonObject)ranked[i * (ranked.Length - 1) / 11].Record.DeepClone();
                    item["technicalSampleBand"] = i < 4 ? "LOWER_PROXY_BAND" : i < 8 ? "MIDDLE_PROXY_BAND" : "UPPER_PROXY_BAND";
                    selected.Add(item);
                }
            }
            Write(Path.Combine(output, "sample-audit.internal.json"), new JsonObject { ["schemaVersion"] = 1, ["status"] = "DRAFT_TECHNICALLY_CURATED_UNISSUED_INPUTS", ["humanStatus"] = "NOT_EXECUTED", ["humanTimingOrApprovalProduced"] = false, ["sourceFiles"] = sourceFiles, ["pool"] = all, ["attempts"] = attempts, ["selected"] = selected, ["selectionMethod"] = "12 size/solver-proxy quantiles from 24 actually generated, independently re-solved candidates per profile; no human difficulty inference. Existing descriptor/source identities only, never product reservations, progress, rewards or issued endless rounds. Internal solution/proof/proxy files must not enter participant assets." });
            Console.WriteLine("DRAFT SAMPLE READY: " + selected.Count + " selected from " + all.Count + "; no human data, issuance or product approval.");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
