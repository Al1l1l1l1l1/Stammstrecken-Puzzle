using System;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using STP.Editor.Content;
using STP.Infrastructure.Content;
using STP.Puzzle.Solver;

internal static class Program
{
    // Same production parser/semantic/hash/solver/proof pipeline as Unity; no alternate proof implementation.
    private static int Main(string[] args)
    {
        try
        {
            var root = Directory.GetCurrentDirectory();
            var files = Directory.GetFiles(Path.Combine(root, "Content/Levels/s1"), "*.level-v2.json", SearchOption.AllDirectories).OrderBy(f => f, StringComparer.Ordinal).ToArray();
            if (files.Length != 240) throw new InvalidOperationException("Expected exactly 240 levels.");
            if (args.Length == 2 && args[0] == "--regenerate")
            {
                foreach(string id in args[1].Split(','))
                {
                var target = files.Single(f => Path.GetFileName(f) == id + ".level-v2.json");
                var generated = ProofGenerator.Generate(File.ReadAllBytes(target), new SolverLimits(10000000, 60000));
                if (!generated.Succeeded) throw new InvalidOperationException(string.Join(";", generated.Diagnostics.Select(d => d.Code + ":" + d.Message)));
                if (generated.Solver!.Metrics!.RequiredGuessDepth != 0) throw new InvalidOperationException("Campaign guess gate failed.");
                var level = JsonNode.Parse(File.ReadAllText(target))!;
                string artifact = "proofs/" + id + "/solver-v2.proof-v1.json";
                level["proofRef"]!["artifactId"] = artifact;
                level["proofRef"]!["proofHash"]!["sha256"] = generated.Proof!.Document!.ProofHash.Sha256;
                // Write only after fresh uniqueness, no-guess and authoring-solution equality have succeeded.
                File.WriteAllBytes(Path.Combine(root, "Content/Proofs", id, "solver-v2.proof-v1.json"), generated.Proof.CanonicalBytes!);
                File.WriteAllText(target, level.ToJsonString(new System.Text.Json.JsonSerializerOptions { WriteIndented = true }) + "\n");
                Console.WriteLine("REGENERATED " + id);
                }
            }
            else if (args.Length != 0) throw new ArgumentException("Usage: ProofAudit [--regenerate S1-SS-RR-PP[,S1-SS-RR-PP]]");
            int count = 0;
            foreach (var file in files)
            {
                var bytes = File.ReadAllBytes(file); var loaded = LevelV2Loader.Load(bytes);
                if (!loaded.IsValid) throw new InvalidOperationException(file + ":" + string.Join(";", loaded.Diagnostics.Select(d => d.Code)));
                string id = loaded.Document!.PuzzleId;
                string artifact = "proofs/" + id + "/solver-v2.proof-v1.json";
                var input = new LevelValidationInput(Path.GetRelativePath(root, file).Replace('\\', '/'), bytes, artifact,
                    File.ReadAllBytes(Path.Combine(root, "Content/Proofs", id, "solver-v2.proof-v1.json")));
                var result = LevelValidationPipeline.ValidateOne(input, true, new SolverLimits(10000000, 60000));
                if (!result.Succeeded) throw new InvalidOperationException(id + ":" + string.Join(";", result.Diagnostics.Select(d => d.Code + ":" + d.Message)));
                count++;
                if (count % 48 == 0) Console.WriteLine("VALIDATED " + count + "/240");
            }
            Console.WriteLine("PASS: all 240 sources freshly solved, unique, no-guess, solution/proof/hash bound; DRAFT and human gates unchanged.");
            return 0;
        }
        catch (Exception e) { Console.Error.WriteLine(e); return 1; }
    }
}
