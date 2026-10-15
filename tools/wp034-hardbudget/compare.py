"""Build isolated old/current production and compare exact solver outputs and cost.

Uses existing .NET10 and NUnit3.14 DLL. No dependencies or goldens are changed.
The original HardBudget test executes all assertions. This is supplementary
diagnosis, never a Unity CI replacement. --profile instruments scratch only.
"""
import argparse
import hashlib
import json
from pathlib import Path
import subprocess
from xml.sax.saxutils import escape

ROOT = Path(__file__).resolve().parents[2]
BASE = 'a2e26a658a2bcdd47002698c150cb5736d9760da'
SCRATCH = ROOT / '.utmp/wp034'


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument('--nunit', type=Path, required=True)
    ap.add_argument('--version', choices=['old', 'current'], required=True)
    ap.add_argument('--profile', action='store_true')
    ap.add_argument('--mutation', choices=['late-result', 'omit-restore', 'reverse-deletion'])
    args = ap.parse_args()
    assert not args.mutation or args.version == 'current'
    dest = SCRATCH / (args.version + ('-profile' if args.profile else '') + ('-'+args.mutation if args.mutation else ''))
    dest.mkdir(parents=True, exist_ok=True)
    paths = subprocess.check_output(['git', 'ls-tree', '-r', '--name-only', BASE, '--', 'Assets/StammstreckenPuzzle/Scripts/Puzzle/Domain', 'Assets/StammstreckenPuzzle/Scripts/Puzzle/Solver'], cwd=ROOT, text=True).splitlines()
    paths += ['Assets/StammstreckenPuzzle/Tests/EditMode/Solver/' + n for n in ['PuzzleSolverTests.cs', 'SmallPathOracle.cs']]
    hashes = {}
    for path in paths:
        if not path.endswith('.cs'):
            continue
        raw = subprocess.check_output(['git', 'show', BASE + ':' + path], cwd=ROOT) if args.version == 'old' else (ROOT/path).read_bytes()
        text = raw.decode('utf-8-sig').replace('\r\n', '\n')
        hashes[path] = hashlib.sha256(text.encode()).hexdigest()
        if args.mutation == 'late-result' and path.endswith('PuzzleSolver.cs'):
            marker = 'return interrupted ? exhausted : result;'
            assert text.count(marker) == 1
            text = text.replace(marker, 'var chosen = interrupted ? exhausted : result; return new SolverResult(chosen.Classification, chosen.SolutionCount, chosen.Path, chosen.Reductions, chosen.Metrics, chosen.DeductionTrace);')
        if args.mutation == 'omit-restore' and path.endswith('RootDeductionTracer.cs'):
            marker = 'view[changedCell] = saved;'
            assert text.count(marker) == 1
            text = text.replace(marker, '')
        if args.mutation == 'reverse-deletion' and path.endswith('RootDeductionTracer.cs'):
            marker = 'for (int k = candidates.Count - 1; k >= 0; k--)'
            assert text.count(marker) == 1
            text = text.replace(marker, 'for (int k = 0; k < candidates.Count; k++)')
        if args.profile and path.endswith('RootDeductionTracer.cs'):
            for name, signature in [('Record', 'internal void Record(string rule, int cell, byte before, byte after, string? axis, int line)'), ('View', 'private byte[] View(bool[] included)'), ('Extent', 'private void Extent(byte[] view, int cell, int facts, out int needW, out int needH)')]:
                marker = signature + '\n        {'
                assert text.count(marker) == 1, marker
                text = text.replace(marker, marker + '\n            using var wp034 = Perf.Measure("' + name + '");')
        (dest/Path(path).name).write_text(text, encoding='utf-8')
    (dest/'Program.cs').write_text(PROGRAM, encoding='utf-8')
    (dest/'Perf.cs').write_text(PERF, encoding='utf-8')
    for module, assembly, files, refs in [
        ('Domain', 'STP.Puzzle.Domain', [p.name for p in (ROOT/'Assets/StammstreckenPuzzle/Scripts/Puzzle/Domain').glob('*.cs')], []),
        ('Solver', 'STP.Puzzle.Solver', [p.name for p in (ROOT/'Assets/StammstreckenPuzzle/Scripts/Puzzle/Solver').glob('*.cs')] + ['Perf.cs'], ['Domain']),
        ('Probe', 'STP.Tests.Solver.EditMode', ['Program.cs', 'PuzzleSolverTests.cs', 'SmallPathOracle.cs'], ['Domain', 'Solver'])]:
        folder = dest/module
        folder.mkdir(exist_ok=True)
        compile = ''.join('<Compile Include="../' + escape(p) + '"/>' for p in files)
        references = ''.join('<ProjectReference Include="../' + p + '/' + p + '.csproj"/>' for p in refs)
        if module == 'Probe':
            references += '<Reference Include="nunit.framework"><HintPath>' + escape(str(args.nunit.resolve())) + '</HintPath></Reference>'
        (folder/(module+'.csproj')).write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework><Nullable>enable</Nullable><EnableDefaultCompileItems>false</EnableDefaultCompileItems><NuGetAudit>false</NuGetAudit><AssemblyName>' + assembly + '</AssemblyName>' + ('<OutputType>Exe</OutputType>' if module == 'Probe' else '') + '</PropertyGroup><ItemGroup>' + compile + references + '</ItemGroup></Project>', encoding='utf-8')
    logs = []
    for index, cmd in enumerate([['dotnet', 'build', str(dest/'Probe/Probe.csproj'), '-c', 'Release'], ['dotnet', str(dest/'Probe/bin/Release/net10.0/STP.Tests.Solver.EditMode.dll')]]):
        r = subprocess.run(cmd, cwd=ROOT, text=True, capture_output=True, encoding='utf-8', errors='replace')
        logs.append('$ ' + ' '.join(cmd) + '\n' + r.stdout + r.stderr)
        (dest/'output.txt').write_text('\n'.join(logs), encoding='utf-8')
        print(r.stdout + r.stderr)
        if index == 0:
            assert r.returncode == 0, 'A compiler failure is never a killed mutation'
    snapshots = [line for line in r.stdout.splitlines() if line.startswith(('SNAPSHOT ', 'TRACE_CONTRACT '))]
    if args.version == 'current':
        expected = [line for line in (SCRATCH/'old/output.txt').read_text(encoding='utf-8').splitlines() if line.startswith(('SNAPSHOT ', 'TRACE_CONTRACT '))]
        same = snapshots == expected and len(expected) == 13
        if args.mutation:
            killed = r.returncode != 0 or not same
            (dest/'mutation.json').write_text(json.dumps(dict(mutation=args.mutation, runtimeExit=r.returncode, snapshotMatch=same, killed=killed), indent=2)+'\n', encoding='utf-8')
            assert killed, 'Mutation survived'
            print('MUTATION KILLED', args.mutation)
        else:
            assert r.returncode == 0 and same, 'Current result differs from the pre-repair contract'
    else:
        assert r.returncode == 0
    (dest/'sources.json').write_text(json.dumps(hashes, indent=2)+'\n', encoding='utf-8')


PERF = r'''using System; using System.Collections.Generic; using System.Diagnostics;
namespace STP.Puzzle.Solver {
public static class Perf {
 static readonly Dictionary<string,(long Calls,long Ticks,long Bytes)> totals = new();
 public static IDisposable Measure(string name) => new Sample(name);
 sealed class Sample : IDisposable { readonly string n; readonly long t = Stopwatch.GetTimestamp(), b = GC.GetAllocatedBytesForCurrentThread();
 public Sample(string name) { n=name; } public void Dispose() { totals.TryGetValue(n,out var v); totals[n]=(v.Calls+1,v.Ticks+Stopwatch.GetTimestamp()-t,v.Bytes+GC.GetAllocatedBytesForCurrentThread()-b); } }
 public static void Report() { foreach(var p in totals) Console.WriteLine($"PROFILE {p.Key} calls={p.Value.Calls} inclusive_ms={p.Value.Ticks*1000.0/Stopwatch.Frequency:R} inclusive_bytes={p.Value.Bytes}"); }
}}
'''

PROGRAM = r'''using System; using System.Diagnostics; using System.Linq; using System.Reflection; using System.Security.Cryptography; using System.Text; using STP.Puzzle.Solver; using STP.Puzzle.Domain; using STP.Tests.Solver.EditMode;
class Program {
 static int Main() {
  Console.WriteLine("RUNTIME "+Environment.Version+" "+Environment.OSVersion);
  var tests = new PuzzleSolverTests(); var w=Stopwatch.StartNew(); long a=GC.GetTotalAllocatedBytes(true);
  tests.HardBudget_NoResultIsBuiltAfterTheLastProbe_ForEveryResultKind();
  Console.WriteLine($"ORIGINAL_ASSERTIONS wall_ms={w.Elapsed.TotalMilliseconds:R} allocated_bytes={GC.GetTotalAllocatedBytes(true)-a}");
  foreach (var name in new[]{"UnsatisfiableSerpentine32","AmbiguousCorner32","AmbiguousLongPath32","Serpentine"}) {
   var m=typeof(PuzzleSolverTests).GetMethod(name,BindingFlags.Static|BindingFlags.NonPublic)!;
   var p=(PuzzleDefinition)m.Invoke(null,name=="Serpentine"?new object[]{32,14}:null)!;
   foreach(long states in new long[]{1,2,5000000}) {
    var r=PuzzleSolver.Solve(p,new SolverLimits(maxStates:states,monotonicNow:()=>1));
    string s=r.Classification+"|"+r.SolutionCount+"|"+string.Join(";",r.Path.Select(c=>c.X+","+c.Y))+"|"+string.Join(";",r.Reductions.Select(x=>x.ToString()))+"|"+(r.Metrics==null?"null":$"{r.Metrics.SearchNodes},{r.Metrics.DeductionSteps},{r.Metrics.MaxDeductionDepth},{r.Metrics.RequiredGuessDepth}");
    s+="|"+string.Join(";",r.DeductionTrace.Select(x=>x.ToString()));
    // ToString is not the trace contract: serialize every public field recursively below.
    s+="|"+System.Text.Json.JsonSerializer.Serialize(r.DeductionTrace);
    if(name=="Serpentine" && states==1) {
     string Fmt(DeductionStep x) => $"{x.Index}:{x.RuleCode}@{x.Cell.X},{x.Cell.Y}:{string.Join(",",x.RemovedValues.Select(v=>(int)v))}:{string.Join(",",x.RemainingValues.Select(v=>(int)v))}:{string.Join(",",x.Premises.Select(q=>$"{(q.PublicFact.HasValue?(int)q.PublicFact.Value:-1)}/{q.LineIndex}/{q.TraceIndex}"))}:{x.Depth}";
     string trace=string.Join("\n",r.DeductionTrace.Select(Fmt));
     Console.WriteLine("TRACE_CONTRACT "+Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(trace)))+" metrics="+System.Text.Json.JsonSerializer.Serialize(r.Metrics));
    }
    Console.WriteLine($"SNAPSHOT {name} states={states} sha256={Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(s)))}");
   }
  }
  Perf.Report(); return 0;
 }
}
'''

if __name__ == '__main__':
    main()
