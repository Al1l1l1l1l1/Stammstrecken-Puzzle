"""Run WP-026 authoring/checks against unchanged pure production assemblies.

Python 3 and .NET SDK 10.0.401; no additional NuGet dependencies. The separate
net10.0 harness is supplementary evidence, not a Unity build or runtime import.
"""
from pathlib import Path
import subprocess
import sys
from xml.sax.saxutils import escape

root = Path(__file__).resolve().parents[2]
scratch = root / '.utmp' / 'wp026-authoring'
sources = root / 'Assets/StammstreckenPuzzle/Scripts'
modules = {
    'Domain': ('STP.Puzzle.Domain', 'Puzzle/Domain', [], None),
    'Solver': ('STP.Puzzle.Solver', 'Puzzle/Solver', ['Domain'], None),
    'Content': ('STP.Infrastructure.Content', 'Infrastructure/Content', ['Domain'], 'ContentModuleSkeleton.cs'),
    'Editor': ('STP.Editor.Content', 'Editor/Content', ['Domain', 'Solver', 'Content'], None),
    'Author': ('WP026.Season1Authoring', None, ['Domain', 'Solver', 'Content', 'Editor'], None),
}
for name, (assembly, folder, refs, excluded) in modules.items():
    directory = scratch / name
    directory.mkdir(parents=True, exist_ok=True)
    include = str(sources / folder / '*.cs') if folder else str(root / 'tools/season1-authoring/Season1Authoring.cs')
    exclude = f' Exclude="{escape(str(sources / folder / excluded))}"' if excluded else ''
    references = ''.join(f'<ProjectReference Include="../{ref}/{ref}.csproj"/>' for ref in refs)
    executable = '<OutputType>Exe</OutputType>' if name == 'Author' else ''
    (directory / f'{name}.csproj').write_text(f'''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup>
<TargetFramework>net10.0</TargetFramework><Nullable>enable</Nullable><EnableDefaultCompileItems>false</EnableDefaultCompileItems>
<NuGetAudit>false</NuGetAudit><AssemblyName>{assembly}</AssemblyName>{executable}</PropertyGroup><ItemGroup>
<Compile Include="{escape(include)}"{exclude}/>{references}</ItemGroup></Project>''', encoding='utf-8')
project = str(scratch / 'Author/Author.csproj')
subprocess.run(['dotnet', 'restore', project, '--ignore-failed-sources'], cwd=root, check=True)
subprocess.run(['dotnet', 'build', project, '-c', 'Release', '--no-restore'], cwd=root, check=True)
subprocess.run(['dotnet', str(scratch / 'Author/bin/Release/net10.0/WP026.Season1Authoring.dll'), *sys.argv[1:]], cwd=root, check=True)
