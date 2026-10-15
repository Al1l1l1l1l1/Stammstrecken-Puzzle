"""Supplementary pure regression suite, excluding the Unity-only corpus test."""
from pathlib import Path
import subprocess
import sys
from xml.sax.saxutils import escape

ROOT = Path(__file__).resolve().parents[2]
DEST = ROOT/'.utmp/wp034/suite'
LIBS = ROOT/'.utmp/wp025-separated/Tests/bin/Release/net10.0'
for module, assembly, folder, refs, exclude in [
    ('Domain', 'STP.Puzzle.Domain', 'Puzzle/Domain', [], ''),
    ('Solver', 'STP.Puzzle.Solver', 'Puzzle/Solver', ['Domain'], ''),
    ('Content', 'STP.Infrastructure.Content', 'Infrastructure/Content', ['Domain'], 'ContentModuleSkeleton.cs'),
    ('Editor', 'STP.Editor.Content', 'Editor/Content', ['Domain', 'Solver', 'Content'], ''),
    ('Tests', 'STP.Tests.Content.EditMode', None, ['Domain', 'Solver', 'Content', 'Editor'], '')]:
    d = DEST/module
    d.mkdir(parents=True, exist_ok=True)
    if folder:
        source = ROOT/'Assets/StammstreckenPuzzle/Scripts'/folder
        paths = [p for p in source.glob('*.cs') if p.name != exclude]
    else:
        paths = [p for area in ['Domain', 'Solver', 'Content'] for p in (ROOT/'Assets/StammstreckenPuzzle/Tests/EditMode'/area).glob('*.cs') if p.name != 'Season1CorpusPerformanceTests.cs']
        (d/'Program.cs').write_text('return new NUnitLite.AutoRun().Execute(args);', encoding='utf-8')
        paths.append(d/'Program.cs')
    compile = ''.join('<Compile Include="'+escape(str(p))+'"/>' for p in paths)
    references = ''.join('<ProjectReference Include="../'+p+'/'+p+'.csproj"/>' for p in refs)
    if not folder:
        for name in ['nunit.framework', 'nunitlite']:
            path = LIBS/(name+'.dll')
            assert path.is_file(), path
            references += '<Reference Include="'+name+'"><HintPath>'+escape(str(path))+'</HintPath></Reference>'
    (d/(module+'.csproj')).write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net10.0</TargetFramework><Nullable>enable</Nullable><EnableDefaultCompileItems>false</EnableDefaultCompileItems><NuGetAudit>false</NuGetAudit><AssemblyName>'+assembly+'</AssemblyName>'+('<OutputType>Exe</OutputType>' if not folder else '')+'</PropertyGroup><ItemGroup>'+compile+references+'</ItemGroup></Project>', encoding='utf-8')
subprocess.run(['dotnet', 'build', str(DEST/'Tests/Tests.csproj'), '-c', 'Release'], cwd=ROOT, check=True)
subprocess.run(['dotnet', str(DEST/'Tests/bin/Release/net10.0/STP.Tests.Content.EditMode.dll'), *sys.argv[1:]], cwd=ROOT, check=True)
