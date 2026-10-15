using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using STP.Editor.Content;
string source = "Assets/StammstreckenPuzzle/Tests/EditMode/Content/proof-v1-solver-v2-root.level.json";
var original = File.ReadAllBytes(source);
var baseline = ProofGenerator.Generate(original);
Console.WriteLine($"Existing FIXTURE_ONLY baseline: succeeded={baseline.Succeeded}; solver={baseline.Solver?.Classification}");
if (!baseline.Succeeded) return 1;
string[] fields = {"content/titleKey", "content/statusKey", "production/entryDeductionKey", "completion/mapSegmentId", "completion/trainMomentId", "completion/resultTextKey"};
// These scratch references have no localization/catalog/asset targets. Test the
// actual proof boundary, without implying cross-reference or product approval.
var draft = JsonNode.Parse(original)!.AsObject();
foreach (string field in fields) {
 var parts = field.Split('/');
 draft[parts[0]]![parts[1]] = "wp026.probe.unresolved." + parts[1].ToLowerInvariant();
}
var draftBytes = Encoding.UTF8.GetBytes(draft.ToJsonString());
var draftProof = ProofGenerator.Generate(draftBytes);
bool sameProof = draftProof.Succeeded && baseline.Proof!.CanonicalBytes!.SequenceEqual(draftProof.Proof!.CanonicalBytes!);
Console.WriteLine($"Populated unresolved scratch references: succeeded={draftProof.Succeeded}; solverInvoked={draftProof.Solver!=null}; sameProofBytes={sameProof}");
if (!sameProof) return 3;
var technical = LevelValidationPipeline.ValidateOne(new LevelValidationInput(source, draftBytes,
 baseline.Level.Document!.ProofRef.ArtifactId, baseline.Proof!.CanonicalBytes), strict: true);
Console.WriteLine($"Strict technical proof boundary: succeeded={technical.Succeeded}; proofGatePassed={technical.ProofGatePassed}; runtimeImportable={technical.IsRuntimeImportable}; releaseLockEligible={technical.IsReleaseLockEligible}");
if (!technical.Succeeded || technical.IsRuntimeImportable || technical.IsReleaseLockEligible) return 4;
foreach (string field in fields) foreach(string mode in new[]{"missing", "null", "empty"}) {
 var doc = JsonNode.Parse(original)!.AsObject(); var parts=field.Split('/'); var obj=doc[parts[0]]!.AsObject();
 if(mode=="missing") obj.Remove(parts[1]); else obj[parts[1]]=mode=="null" ? null : JsonValue.Create("");
 var result=ProofGenerator.Generate(Encoding.UTF8.GetBytes(doc.ToJsonString()));
 Console.WriteLine($"{field} {mode}: succeeded={result.Succeeded}; solverInvoked={result.Solver!=null}; proofPresent={result.Proof!=null}; diagnostics={string.Join(";", result.Diagnostics.Select(d => d.Code+"@"+d.Path))}");
 if(result.Succeeded || result.Solver!=null || result.Proof!=null || result.Diagnostics.Count==0) return 2;
}
Console.WriteLine("PASS: fixture and populated unresolved scratch references pass the technical proof boundary; 18 incomplete-reference variants rejected. Cross-references NOT verified. No Season-1 puzzle generated.");
return 0;
