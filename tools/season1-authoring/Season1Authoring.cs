using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using STP.Editor.Content;
using STP.Infrastructure.Content;
using STP.Puzzle.Domain;
using STP.Puzzle.Solver;

// This helper constructs candidates; only the unchanged Domain/Solver/Content
// APIs determine validity, uniqueness, campaign eligibility and proof bytes.
var options = new JsonSerializerOptions { WriteIndented = true };
if (args.Length == 0) throw new ArgumentException("pool width height count seed | materialize selection.json | verify");
if (args[0] == "pool") {
 int w = int.Parse(args[1]), h = int.Parse(args[2]), count = int.Parse(args[3]), seed = int.Parse(args[4]);
 var rng = new Random(seed); var candidates = new List<JsonObject>(); var seen = new HashSet<string>();
 var total = Stopwatch.StartNew(); int attempts = 0; var rejected = new Dictionary<string,int>();
 while (candidates.Count < count && attempts < 50000) {
  attempts++; var path = Author.RandomPath(w, h, rng); if (path == null) continue;
  var doc = Author.Document("S1-01-01-01", w, h, path.Value.cells, path.Value.a, path.Value.b);
  string key = doc["endpoints"]!.ToJsonString() + doc["rowCounts"]!.ToJsonString() + doc["columnCounts"]!.ToJsonString();
  if (!seen.Add(key)) continue;
  var timer = Stopwatch.StartNew(); var proof = ProofGenerator.Generate(Author.Bytes(doc), new SolverLimits(10000, 250)); timer.Stop();
  string outcome = proof.Solver?.Classification.ToString() ?? "REJECTED";
  if (!proof.Succeeded || CampaignGate.Evaluate(proof.Solver) != CampaignGateOutcome.ELIGIBLE) {
   rejected[outcome] = rejected.GetValueOrDefault(outcome) + 1; continue;
  }
  var m = proof.Solver!.Metrics!;
  candidates.Add(new JsonObject { ["candidate"] = candidates.Count, ["width"] = w, ["height"] = h,
   ["document"] = doc,
   ["steps"] = m.DeductionSteps, ["depth"] = m.MaxDeductionDepth, ["elapsedMs"] = (int)timer.ElapsedMilliseconds });
  candidates[^1]["metrics"] = new JsonObject { ["searchNodes"] = m.SearchNodes, ["deductionSteps"] = m.DeductionSteps,
   ["maxDeductionDepth"] = m.MaxDeductionDepth, ["requiredGuessDepth"] = m.RequiredGuessDepth };
  if (candidates.Count % 20 == 0) Console.WriteLine($"pool {w}x{h}: {candidates.Count}/{count}; attempts={attempts}; seconds={(int)total.Elapsed.TotalSeconds}");
 }
 Directory.CreateDirectory(".utmp/wp026-pools");
 File.WriteAllText($".utmp/wp026-pools/{w}x{h}-{seed}.json", JsonSerializer.Serialize(candidates, options), new UTF8Encoding(false));
 Console.WriteLine($"POOL {w}x{h}: accepted={candidates.Count}; attempts={attempts}; seconds={(int)total.Elapsed.TotalSeconds}; rejected={JsonSerializer.Serialize(rejected)}");
 return candidates.Count == count ? 0 : 1;
}
if (args[0] == "materialize") {
 var selection=JsonNode.Parse(File.ReadAllText(args[1]))!.AsArray();
 if(selection.Count!=240)throw new InvalidOperationException("Selection must contain exactly 240 reviewed entries.");
 foreach(var entry in selection) {
  var doc=entry!["document"]!.AsObject();string id=doc["puzzleId"]!.GetValue<string>();
  var generated=ProofGenerator.Generate(Author.Bytes(doc));
  if(!generated.Succeeded||CampaignGate.Evaluate(generated.Solver)!=CampaignGateOutcome.ELIGIBLE)throw new InvalidOperationException(id+": not deductively unique");
  doc["proofRef"]!["proofHash"]!["sha256"]=generated.Proof!.Document!.ProofHash.Sha256;
  string[] parts=id.Split('-');string source=$"Content/Levels/s1/{parts[1]}/{parts[2]}/{id}.level-v2.json",proofPath=$"Content/Proofs/{id}/solver-v2.proof-v1.json";
  var bytes=Encoding.UTF8.GetBytes(doc.ToJsonString(options)+"\n");
  var result=LevelValidationPipeline.ValidateOne(new LevelValidationInput(source,bytes,ProofV1Codec.ArtifactId(id,"solver-v2"),generated.Proof.CanonicalBytes),strict:true);
  if(!result.Succeeded)throw new InvalidOperationException(id+": "+string.Join(",",result.Diagnostics.Select(d=>d.Code)));
  Directory.CreateDirectory(Path.GetDirectoryName(source)!);Directory.CreateDirectory(Path.GetDirectoryName(proofPath)!);
  // Avoid rewriting identical reviewed artifacts (and needless Windows file locks).
  if(!File.Exists(source)||!File.ReadAllBytes(source).SequenceEqual(bytes))File.WriteAllBytes(source,bytes);
  if(!File.Exists(proofPath)||!File.ReadAllBytes(proofPath).SequenceEqual(generated.Proof.CanonicalBytes!))File.WriteAllBytes(proofPath,generated.Proof.CanonicalBytes!);
 }
 Console.WriteLine("MATERIALIZED 240 canonical authoring sources and 240 genuine solver-v2 proofs; cross-reference/import/release gates remain open.");
 return 0;
}
if(args[0]=="verify") {
 var files=Directory.GetFiles("Content/Levels/s1","*.level-v2.json",SearchOption.AllDirectories).OrderBy(x=>x,StringComparer.Ordinal).ToArray();
 if(files.Length!=240)throw new InvalidOperationException("Expected 240 sources.");
 var inputs=new List<LevelValidationInput>();var evidence=new JsonArray();var traces=new Dictionary<string,JsonArray>();
 var times=new List<double>();var bigTimes=new List<double>();int metamorphoses=0,mutations=0;
 var traceOptions=new JsonSerializerOptions{PropertyNamingPolicy=JsonNamingPolicy.CamelCase,WriteIndented=true};traceOptions.Converters.Add(new JsonStringEnumConverter());
 var allTimer=Stopwatch.StartNew();
 foreach(string file in files) {
  byte[] bytes=File.ReadAllBytes(file);var doc=JsonNode.Parse(bytes)!.AsObject();string id=doc["puzzleId"]!.GetValue<string>();
  string proofFile=$"Content/Proofs/{id}/solver-v2.proof-v1.json";byte[] saved=File.ReadAllBytes(proofFile);
  var timer=Stopwatch.StartNew();var generated=ProofGenerator.Generate(bytes);timer.Stop();times.Add(timer.Elapsed.TotalMilliseconds);
  if(!generated.Succeeded||!saved.SequenceEqual(generated.Proof!.CanonicalBytes!))throw new InvalidOperationException(id+": regenerated bytes differ");
  for(int repeat=0;repeat<2;repeat++)if(!saved.SequenceEqual(ProofGenerator.Generate(bytes).Proof!.CanonicalBytes!))throw new InvalidOperationException(id+": repeat differs");
  var input=new LevelValidationInput(file,bytes,ProofV1Codec.ArtifactId(id,"solver-v2"),saved);inputs.Add(input);
  var strict=LevelValidationPipeline.ValidateOne(input,strict:true);
  if(!strict.Succeeded||strict.IsRuntimeImportable||strict.IsReleaseLockEligible)throw new InvalidOperationException(id+": boundary failure");
  var solver=generated.Solver!;var m=solver.Metrics!;var trace=solver.DeductionTrace;
  if(m.DeductionSteps!=trace.Count||m.MaxDeductionDepth!=trace.Max(t=>t.Depth)||m.RequiredGuessDepth!=0||m.SearchNodes!=0)throw new InvalidOperationException(id+": metric reconstruction");
  foreach(var step in trace) {
   var prior=step.Premises.Where(p=>!p.IsPublicFact).Select(p=>p.TraceIndex).ToArray();
   if(prior.Any(i=>i<0||i>=step.Index)||step.Depth!=1+prior.Select(i=>trace[i].Depth).DefaultIfEmpty(0).Max())throw new InvalidOperationException(id+": invalid trace dependency");
   int x=step.Cell.X,y=step.Cell.Y;string? trackValue=doc["solution"]!["path"]!.AsArray().FirstOrDefault(c=>c!["x"]!.GetValue<int>()==x&&c["y"]!.GetValue<int>()==y)?["track"]!.GetValue<string>();
   string value=trackValue??"EMPTY";
   if(step.RemovedValues.Any(v=>v.ToString()==value))throw new InvalidOperationException(id+": unsafe root trace");
  }
  int w=doc["grid"]!["width"]!.GetValue<int>(),h=doc["grid"]!["height"]!.GetValue<int>();if(w==10&&h==10)bigTimes.Add(timer.Elapsed.TotalMilliseconds);
  // All 16 D4 / A-B permutations are reclassified by the unchanged solver.
  for(int mirror=0;mirror<2;mirror++)for(int rotation=0;rotation<4;rotation++)for(int swap=0;swap<2;swap++) {
   var transformed=Author.Transform(doc,mirror!=0,rotation,swap!=0);
   var regen=ProofGenerator.Generate(Author.Bytes(transformed));
   if(!regen.Succeeded||CampaignGate.Evaluate(regen.Solver)!=CampaignGateOutcome.ELIGIBLE)throw new InvalidOperationException(id+": metamorphic failure");
   metamorphoses++;
  }
  // Source mutation: counts/endpoints/path; cache mutation: hash, metric, binding.
  foreach(string kind in new[]{"count","endpoint","path","proofHash","metric","artifact"}) {
   var changed=(JsonObject)doc.DeepClone();byte[] changedProof=saved;string artifact=ProofV1Codec.ArtifactId(id,"solver-v2");
   if(kind=="count")changed["rowCounts"]![0]=(changed["rowCounts"]![0]!.GetValue<int>()+1)%(w+1);
   if(kind=="endpoint")changed["endpoints"]!["a"]!["index"]=10;
   if(kind=="path")changed["solution"]!["path"]![0]!["track"]="TRACK_NS"==changed["solution"]!["path"]![0]!["track"]!.GetValue<string>()?"TRACK_EW":"TRACK_NS";
   if(kind=="proofHash")changed["proofRef"]!["proofHash"]!["sha256"]=new string('0',64);
   if(kind=="metric") {
    var p=generated.Proof!.Document!;changedProof=ProofV1Codec.Create(id,p.PublicPuzzleHash,p.SolutionHash,"solver-v2",p.Metrics.SearchNodes,p.Metrics.DeductionSteps+1,p.Metrics.MaxDeductionDepth,p.Metrics.RequiredGuessDepth).CanonicalBytes!;
    changed["proofRef"]!["proofHash"]!["sha256"]=ProofV1Codec.Read(changedProof).Document!.ProofHash.Sha256;
   }
   if(kind=="artifact")artifact="proofs/WRONG/solver-v2.proof-v1.json";
   var bad=LevelValidationPipeline.ValidateOne(new LevelValidationInput(file,Author.Bytes(changed),artifact,changedProof),strict:true);
   if(bad.Succeeded)throw new InvalidOperationException(id+": mutation survived "+kind);mutations++;
  }
  string route=id[..8];if(!traces.ContainsKey(route))traces[route]=new JsonArray();
  traces[route].Add(new JsonObject{["puzzleId"]=id,["publicPuzzleHash"]=JsonSerializer.SerializeToNode(new{profile=generated.Level.PublicPuzzleHash!.Profile,sha256=generated.Level.PublicPuzzleHash.Sha256}),
   ["solverVersion"]="solver-v2",["proofHash"]=generated.Proof!.Document!.ProofHash.Sha256,["steps"]=JsonSerializer.SerializeToNode(trace,traceOptions)});
  evidence.Add(new JsonObject{["puzzleId"]=id,["sourcePath"]=file.Replace('\\','/'),["proofPath"]=proofFile,["publicPuzzleHash"]=generated.Level.PublicPuzzleHash!.Sha256,
   ["solutionHash"]=generated.Level.SolutionHash!.Sha256,["documentSha256"]=LevelHashing.ComputeContentHash(generated.Level.Document!.ToJson()),
   ["proofHash"]=generated.Proof.Document!.ProofHash.Sha256,["metrics"]=JsonSerializer.SerializeToNode(new{searchNodes=m.SearchNodes,deductionSteps=m.DeductionSteps,maxDeductionDepth=m.MaxDeductionDepth,requiredGuessDepth=m.RequiredGuessDepth}),
   ["elapsedMicroseconds"]=(long)(timer.Elapsed.TotalMilliseconds*1000),["strictProofGatePassed"]=true,["runtimeImportable"]=false,["releaseLockEligible"]=false});
  if(evidence.Count%12==0)Console.WriteLine($"VERIFY {evidence.Count}/240; metamorphoses={metamorphoses}; rejected mutations={mutations}");
 }
 var batchTimer=Stopwatch.StartNew();var batch=LevelValidationCommand.ValidateBatch(inputs,new[]{"--strict"});batchTimer.Stop();
 if(!batch.Succeeded)throw new InvalidOperationException("Strict batch failed");
 var reverse=LevelValidationCommand.ValidateBatch(inputs.AsEnumerable().Reverse(),new[]{"--strict"});if(!reverse.Succeeded)throw new InvalidOperationException("Reverse strict batch failed");
 allTimer.Stop();
 Directory.CreateDirectory("Content/Reviews/season-1");
 foreach(var pair in traces)File.WriteAllText($"Content/Reviews/season-1/root-trace-{pair.Key}.json",pair.Value.ToJsonString(options)+"\n",new UTF8Encoding(false));
 double P95(List<double> numbers)=>numbers.OrderBy(n=>n).ElementAt((int)Math.Ceiling(numbers.Count*.95)-1);
 var summary=new JsonObject{["harness"]="net10.0 supplementary pure assemblies; not Unity/CI reference machine",["levels"]=240,["regenerationsPerSourceBeforeStrict"]=3,
  ["strictOnePassed"]=240,["strictBatchPassed"]=true,["reverseStrictBatchPassed"]=true,["metamorphosesPassed"]=metamorphoses,["mutationsRejected"]=mutations,
  ["metricsAndRootTraceReconstructed"]=240,["sourceMeasurements"]="single warm process, first generation per source; platform budget requires CI-reference evidence",
  ["solverP95Microseconds"]=(long)(P95(times)*1000),["solverMaxMicroseconds"]=(long)(times.Max()*1000),
  ["tenByTenCount"]=bigTimes.Count,["tenByTenP95Microseconds"]=(long)(P95(bigTimes)*1000),["tenByTenMaxMicroseconds"]=(long)(bigTimes.Max()*1000),
  ["wholeVerificationMilliseconds"]=(long)allTimer.Elapsed.TotalMilliseconds,["crossReferencesPassed"]=false,["runtimeImported"]=false,["levelsEvidence"]=evidence};
 summary["strictBatchWallMicroseconds"]=(long)(batchTimer.Elapsed.TotalMilliseconds*1000);
 File.WriteAllText("Content/Reviews/season-1/technical-evidence.json",summary.ToJsonString(options)+"\n",new UTF8Encoding(false));
 Console.WriteLine($"PASS technical: 240 levels/proofs, {metamorphoses} metamorphoses, {mutations} rejected mutations; verification={allTimer.ElapsedMilliseconds}ms; 10x10 p95={summary["tenByTenP95Microseconds"]!.GetValue<long>()}us/max={summary["tenByTenMaxMicroseconds"]!.GetValue<long>()}us. NOT a cross-reference/quality/import/release PASS.");
 return 0;
}
throw new ArgumentException("Unknown mode");

static class Author {
 public static byte[] Bytes(JsonNode doc) => Encoding.UTF8.GetBytes(doc.ToJsonString());
 public static JsonObject Transform(JsonObject doc,bool mirror,int rotations,bool swap) {
  int w=doc["grid"]!["width"]!.GetValue<int>(),h=doc["grid"]!["height"]!.GetValue<int>();
  var cells=doc["solution"]!["path"]!.AsArray().Select(c=>c!["y"]!.GetValue<int>()*w+c["x"]!.GetValue<int>()).ToList();
  var endpoints=new[]{"a","b"}.Select(k=>(side:Enum.Parse<Direction>(doc["endpoints"]![k]!["side"]!.GetValue<string>()),index:doc["endpoints"]![k]!["index"]!.GetValue<int>())).ToArray();
  if(mirror){cells=cells.Select(c=>c/w*w+w-1-c%w).ToList();for(int i=0;i<2;i++){var e=endpoints[i];endpoints[i]=(e.side==Direction.N||e.side==Direction.S)?(e.side,w-1-e.index):(e.side==Direction.E?Direction.W:Direction.E,e.index);}}
  for(int r=0;r<rotations;r++) {
   cells=cells.Select(c=>(c%w)*h+h-1-c/w).ToList();
   for(int i=0;i<2;i++){var e=endpoints[i];endpoints[i]=((Direction)(((int)e.side+1)%4),e.side==Direction.E||e.side==Direction.W?h-1-e.index:e.index);}
   (w,h)=(h,w);
  }
  if(swap){cells.Reverse();(endpoints[0],endpoints[1])=(endpoints[1],endpoints[0]);}
  return Document(doc["puzzleId"]!.GetValue<string>(),w,h,cells,endpoints[0],endpoints[1]);
 }
 public static JsonObject Document(string id, int w, int h, List<int> cells, (Direction side,int index) a, (Direction side,int index) b) {
  var parts = id.Split('-'); int section = int.Parse(parts[1]), route = int.Parse(parts[2]), position = int.Parse(parts[3]);
  var rows = new int[h]; var cols = new int[w]; var path = new JsonArray();
  Direction Toward(int from,int to) => to/w < from/w ? Direction.N : to%w > from%w ? Direction.E : to/w > from/w ? Direction.S : Direction.W;
  for (int i=0;i<cells.Count;i++) {
   int c = cells[i], x=c%w,y=c/w; rows[y]++; cols[x]++;
   Direction incoming = i==0 ? a.side : Toward(c,cells[i-1]), outgoing = i==cells.Count-1 ? b.side : Toward(c,cells[i+1]);
   int ports=(1<<(int)incoming)|(1<<(int)outgoing); int shape=Enumerable.Range(0,6).Single(s=>TrackGeometry.Ports((TrackShape)s)==ports);
   path.Add(new JsonObject {["x"]=x,["y"]=y,["track"]="TRACK_"+((TrackShape)shape)});
  }
  string ns = $"level.s1.{section:00}.{route:00}.{position:00}";
  return new JsonObject { ["documentSchemaVersion"]=2,["rulesetVersion"]="train-track-v1",["puzzleId"]=id,["contentRevision"]=1,
   ["grid"]=new JsonObject{["width"]=w,["height"]=h},
   ["endpoints"]=new JsonObject{["a"]=new JsonObject{["side"]=a.side.ToString(),["index"]=a.index},["b"]=new JsonObject{["side"]=b.side.ToString(),["index"]=b.index}},
   ["rowCounts"]=JsonSerializer.SerializeToNode(rows),["columnCounts"]=JsonSerializer.SerializeToNode(cols),
   ["content"]=new JsonObject{["season"]=1,["networkSection"]=section,["route"]=route,["position"]=position,["titleKey"]=ns+".title",["statusKey"]=ns+".status"},
   ["production"]=new JsonObject{["entryDeductionKey"]=ns+".entry",["focus"]=new JsonArray("COMBINATION"),["chainDepth"]=1,
    ["qualityNote"]="Scratch candidate: individual leveldesign review pending.",["timeClass"]="UNCALIBRATED",["starThresholdsSeconds"]=null},
   ["completion"]=new JsonObject{["mapSegmentId"]=$"draft-s1-{section:00}-{route:00}-{position:00}-map",["trainMomentId"]=$"draft-s1-{section:00}-{route:00}-{position:00}-train",["resultTextKey"]=ns+".result"},
   ["solution"]=new JsonObject{["path"]=path},["proofRef"]=new JsonObject{["artifactId"]=ProofV1Codec.ArtifactId(id,"solver-v2"),["proofFormatVersion"]=1,
    ["proofHash"]=new JsonObject{["profile"]=LevelHashProfiles.Proof,["sha256"]=new string('0',64)}}};
 }
 public static (List<int> cells,(Direction side,int index) a,(Direction side,int index) b)? RandomPath(int w,int h,Random rng) {
  if (rng.Next(3)==0) {
   bool transpose=rng.Next(2)==0;int width=transpose?h:w,height=transpose?w:h;
   int x=rng.Next(Math.Max(1,width/3)),first=x;var stripes=new List<int>();bool right=true;
   for(int y=0;y<height;y+=2) {
    int target=right?rng.Next(Math.Max(x+1,width*2/3),width):rng.Next(0,Math.Min(x,width/3+1));
    if(y==0)stripes.Add(x);
    while(x!=target){x+=right?1:-1;stripes.Add(y*width+x);}
    if(y+1<height)stripes.Add((y+1)*width+x);
    if(y+2<height)stripes.Add((y+2)*width+x);
    right=!right;
   }
   // The final connector, if present, reaches the bottom exterior port.
   var result=stripes.Select(c=>transpose?(c%width)*w+c/width:c).ToList();
   return(result,(transpose?Direction.W:Direction.N,first),(transpose?Direction.E:Direction.S,x));
  }
  // Inflate a straight path with two-cell ears. This supplies dense, spatially
  // structured alternatives to short random walks; it adds no puzzle rule.
  if (rng.Next(2)==0) {
   bool horizontal=rng.Next(2)==0; int line=rng.Next(horizontal?h:w);
   var inflated=Enumerable.Range(0,horizontal?w:h).Select(i=>horizontal?line*w+i:i*w+line).ToList();
   int target=rng.Next(Math.Max(inflated.Count,(int)(w*h*0.4)),(int)(w*h*0.9)+1);
   while(inflated.Count<target) {
    var occupied=new HashSet<int>(inflated);var ears=new List<(int i,int u,int v)>();
    for(int i=0;i<inflated.Count-1;i++) {
     int u=inflated[i],v=inflated[i+1],dx=v%w-u%w,dy=v/w-u/w;
     foreach(int side in new[]{-1,1}) {
      int ux=u%w+dy*side,uy=u/w-dx*side,vx=v%w+dy*side,vy=v/w-dx*side;
      if(ux>=0&&ux<w&&uy>=0&&uy<h&&vx>=0&&vx<w&&vy>=0&&vy<h&&!occupied.Contains(uy*w+ux)&&!occupied.Contains(vy*w+vx))ears.Add((i,uy*w+ux,vy*w+vx));
     }
    }
    if(ears.Count==0)break;var ear=ears[rng.Next(ears.Count)];inflated.InsertRange(ear.i+1,new[]{ear.u,ear.v});
   }
   return(inflated,(horizontal?Direction.W:Direction.N,line),(horizontal?Direction.E:Direction.S,line));
  }
  Direction start=(Direction)rng.Next(4); int index=rng.Next(start==Direction.N||start==Direction.S?w:h);
  int c=start==Direction.N?index:start==Direction.S?(h-1)*w+index:start==Direction.W?index*w:index*w+w-1;
  var cells=new List<int>{c}; var used=new HashSet<int>{c};
  int desired=rng.Next(Math.Max(4,w+h-3),Math.Max(w+h-2,(int)(w*h*0.85)) + 1);
  for(int step=1;step<w*h;step++) {
   int x=c%w,y=c/w; var ends=new List<(Direction,int)>();
   if(y==0)ends.Add((Direction.N,x));if(x==w-1)ends.Add((Direction.E,y));if(y==h-1)ends.Add((Direction.S,x));if(x==0)ends.Add((Direction.W,y));
   if(cells.Count>=desired&&ends.Count>0) return(cells,(start,index),ends[rng.Next(ends.Count)]);
   var neighbors=new List<int>();foreach(Direction d in Enum.GetValues<Direction>()) {
    int nx=x+TrackGeometry.DeltaX(d),ny=y+TrackGeometry.DeltaY(d);if(nx>=0&&nx<w&&ny>=0&&ny<h&&!used.Contains(ny*w+nx))neighbors.Add(ny*w+nx);
   }
   if(neighbors.Count==0) { if(cells.Count>=Math.Max(4,(w+h)/2)&&ends.Count>0)return(cells,(start,index),ends[rng.Next(ends.Count)]);return null; }
   c=neighbors[rng.Next(neighbors.Count)];cells.Add(c);used.Add(c);
  }
  return null;
 }
}
