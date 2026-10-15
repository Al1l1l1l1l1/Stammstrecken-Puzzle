"""Read original evidence and diagnose the unchanged HardBudget test in scratch.

Requires .NET 10 and an existing NUnit 3.14 framework DLL (--nunit). No packages
are added to the project. This supplementary probe is not Unity CI or a repair.
"""
from pathlib import Path
import argparse,base64,gzip,hashlib,json,statistics,subprocess,sys
import xml.etree.ElementTree as ET
from xml.sax.saxutils import escape

ROOT=Path(__file__).resolve().parents[2]
OUT=ROOT/'PROJECT_CONTROL/Investigations'
SCRATCH=ROOT/'.utmp/wp030-probe'
OLD='96697ab749fbea859fde8a4d4fcb42149cc2e89c'
NAME='HardBudget_NoResultIsBuiltAfterTheLastProbe_ForEveryResultKind'
TEST='Assets/StammstreckenPuzzle/Tests/EditMode/Solver/PuzzleSolverTests.cs'

def blob(rev,path):return subprocess.check_output(['git','show',rev+':'+path],cwd=ROOT)
def sha(b):return hashlib.sha256(b).hexdigest()
def main():
    ap=argparse.ArgumentParser();ap.add_argument('--nunit',type=Path,required=True);args=ap.parse_args()
    assert args.nunit.is_file(),'existing NUnit 3.14 DLL required'
    OUT.mkdir(parents=True,exist_ok=True);SCRATCH.mkdir(parents=True,exist_ok=True)
    original=json.loads((ROOT/'Content/Reviews/season-1/ci-repair-96697ab-artifact-evidence.json').read_text(encoding='utf-8'))
    runs=[];durations=[]
    for a in original['artifacts']:
        for m in a['members']:
            if m['path']!='editmode.xml':continue
            raw=gzip.decompress(base64.b64decode(m['originalXmlGzipBase64']));assert sha(raw)==m['sha256']
            root=ET.fromstring(raw);case=next(c for c in root.iter('test-case') if c.get('name')==NAME)
            output=case.findtext('output') or ''
            runs.append(dict(run=a['metadata']['workflow_run']['id'],commit=a['metadata']['workflow_run']['head_sha'],xmlSha256=sha(raw),test=dict(case.attrib),output=output,failure=case.findtext('failure/message'),allSixKindsLogged=output.count('WP024_HARD_BUDGET_KIND')==6))
            durations.append({c.get('fullname'):(float(c.get('duration','0')),c.get('start-time')) for c in root.iter('test-case')})
    assert len(runs)==2 and all(r['commit']==OLD and r['allSixKindsLogged'] for r in runs)
    a,b=durations
    pairs=[dict(test=k,pushSeconds=a[k][0],prSeconds=b[k][0],ratio=a[k][0]/b[k][0],pushStart=a[k][1],prStart=b[k][1]) for k in sorted(a.keys()&b.keys()) if b[k][0]>.1]
    paths=[TEST,'Assets/StammstreckenPuzzle/Tests/EditMode/Solver/SmallPathOracle.cs']
    paths+=subprocess.check_output(['git','ls-tree','-r','--name-only','HEAD','--','Assets/StammstreckenPuzzle/Scripts/Puzzle/Domain','Assets/StammstreckenPuzzle/Scripts/Puzzle/Solver'],cwd=ROOT,text=True).splitlines()
    paths=[p for p in paths if p.endswith('.cs')]
    source=[]
    for p in paths:
        old=blob(OLD,p);current=blob('HEAD',p);assert old==current,p+' changed'
        source.append(dict(path=p,gitBlobContentSha256=sha(current),unchangedSinceOriginal=True))
    text=(ROOT/TEST).read_text(encoding='utf-8')
    marker='foreach (var c in cases)\n            {\n                long maxStates = c.MaxStates;'
    assert text.count(marker)==1
    text=text.replace(marker,'''foreach (var c in cases)
            {
                var phaseClock = Stopwatch.StartNew();
                var phaseCpu = Process.GetCurrentProcess().TotalProcessorTime;
                long phaseAlloc = GC.GetTotalAllocatedBytes(false);
                int phaseGc = GC.CollectionCount(0);
                void EndPhase() { Console.WriteLine($"WP030_PHASE {c.Name}|wall_ms={phaseClock.Elapsed.TotalMilliseconds:R}|cpu_ms={(Process.GetCurrentProcess().TotalProcessorTime-phaseCpu).TotalMilliseconds:R}|allocated_bytes={GC.GetTotalAllocatedBytes(false)-phaseAlloc}|gen0={GC.CollectionCount(0)-phaseGc}"); }
                long maxStates = c.MaxStates;''')
    # Diagnostics only outside the measured tail/copy intervals. Every original
    # assertion, 2 warmups, 25 samples, fixture and numeric budget is retained.
    marker='baseline = minTail; Assert.That(size, Is.LessThan(20), c.Name);'
    assert text.count(marker)==1;text=text.replace(marker,'EndPhase(); '+marker)
    marker='double excess = minTail - baseline;';assert text.count(marker)==1;text=text.replace(marker,'EndPhase(); '+marker)
    (SCRATCH/'Instrumented.cs').write_text(text,encoding='utf-8')
    (SCRATCH/'Program.cs').write_text('''using System; using System.Diagnostics; using STP.Tests.Solver.EditMode;
class Program { static int Main() { var w=Stopwatch.StartNew(); Console.WriteLine("WP030_RUNTIME "+Environment.Version+" "+Environment.OSVersion+" processors="+Environment.ProcessorCount); new PuzzleSolverTests().HardBudget_NoResultIsBuiltAfterTheLastProbe_ForEveryResultKind(); Console.WriteLine("WP030_ORIGINAL_ASSERTIONS_COMPLETED wall_ms="+w.Elapsed.TotalMilliseconds); return 0; } }
''',encoding='utf-8')
    modules={
        'Domain':('STP.Puzzle.Domain','Puzzle/Domain',[]),
        'Solver':('STP.Puzzle.Solver','Puzzle/Solver',['Domain']),
        'Probe':('STP.Tests.Solver.EditMode',None,['Domain','Solver'])}
    for name,(assembly,folder,refs) in modules.items():
        directory=SCRATCH/name;directory.mkdir(exist_ok=True)
        compile=f'<Compile Include="{escape(str(ROOT/"Assets/StammstreckenPuzzle/Scripts"/folder/"*.cs"))}"/>' if folder else ''.join(f'<Compile Include="{escape(str(p))}"/>' for p in [SCRATCH/'Program.cs',SCRATCH/'Instrumented.cs',ROOT/paths[1]])
        references=''.join(f'<ProjectReference Include="../{r}/{r}.csproj"/>' for r in refs)
        if name=='Probe':references+=f'<Reference Include="nunit.framework"><HintPath>{escape(str(args.nunit.resolve()))}</HintPath></Reference>'
        exe='<OutputType>Exe</OutputType>' if name=='Probe' else ''
        (directory/f'{name}.csproj').write_text(f'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework><Nullable>enable</Nullable><EnableDefaultCompileItems>false</EnableDefaultCompileItems><NuGetAudit>false</NuGetAudit><AssemblyName>{assembly}</AssemblyName>{exe}</PropertyGroup><ItemGroup>{compile}{references}</ItemGroup></Project>',encoding='utf-8')
    logs=[]
    def run(cmd):
        p=subprocess.run(cmd,cwd=ROOT,text=True,encoding='utf-8',errors='replace',capture_output=True);logs.append('$ '+' '.join(map(str,cmd))+'\n'+p.stdout+p.stderr);assert p.returncode==0,p.stdout+p.stderr;return p.stdout
    project=str(SCRATCH/'Probe/Probe.csproj')
    run(['dotnet','restore',project,'--ignore-failed-sources']);run(['dotnet','build',project,'-c','Release','--no-restore'])
    output=run(['dotnet',str(SCRATCH/'Probe/bin/Release/net10.0/STP.Tests.Solver.EditMode.dll')])
    assert output.count('WP030_PHASE ')==6 and 'WP030_ORIGINAL_ASSERTIONS_COMPLETED' in output
    report=dict(role='author-operated investigation, no independent or Unity CI approval',sourceHead=subprocess.check_output(['git','rev-parse','HEAD'],cwd=ROOT,text=True).strip(),originalRuns=runs,comparisonTestDurations=pairs,medianRatioFor37TestsOver100ms=statistics.median(p['ratio'] for p in pairs),sourceBlobs=source,instrumentedScratchSourceSha256=sha(text.encode()),localProbe=output,originalCause='NOT_PROVEN: historic CPU/scheduler/GC telemetry unavailable; elapsed workload and completed assertions are evidenced, runner contention is a hypothesis',repairApplied=False,originalTimeoutClosed=False)
    (OUT/'WP-030_EVIDENCE.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    (OUT/'WP-030_PROBE.txt').write_text('\n'.join(logs),encoding='utf-8')
    print(output);print('Original timeout remains OPEN; full evidence written separately.')
if __name__=='__main__':main()
