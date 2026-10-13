"""Probe WP-026's reference boundaries; creates only an ignored .utmp harness.

Requires Python 3 and installed .NET SDK 10.0.401 (net10.0). No NuGet packages.
Compiles unchanged repository sources in their separate pure assemblies. This is
a supplementary API experiment, not Unity compilation or Season-1 production.
"""
from pathlib import Path
import subprocess
from xml.sax.saxutils import escape

root = Path(__file__).resolve().parents[2]
scratch = root / ".utmp" / "wp026-reference-gate"
sources = root / "Assets/StammstreckenPuzzle/Scripts"
modules = {
    "Domain": ("STP.Puzzle.Domain", "Puzzle/Domain", [], None),
    "Solver": ("STP.Puzzle.Solver", "Puzzle/Solver", ["Domain"], None),
    "Content": ("STP.Infrastructure.Content", "Infrastructure/Content", ["Domain"], "ContentModuleSkeleton.cs"),
    "Editor": ("STP.Editor.Content", "Editor/Content", ["Domain", "Solver", "Content"], None),
    "Probe": ("WP026.ReferenceGateProbe", None, ["Editor"], None),
}
for name, (assembly, folder, refs, excluded) in modules.items():
    directory = scratch / name
    directory.mkdir(parents=True, exist_ok=True)
    include = str(sources / folder / "*.cs") if folder else str(root / "tools/season1-authoring/ReferenceGateProbe.cs")
    exclude = f' Exclude="{escape(str(sources / folder / excluded))}"' if excluded else ""
    project_refs = "".join(f'<ProjectReference Include="../{ref}/{ref}.csproj"/>' for ref in refs)
    executable = "<OutputType>Exe</OutputType>" if name == "Probe" else ""
    xml = f'''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup>
<TargetFramework>net10.0</TargetFramework><Nullable>enable</Nullable>
<EnableDefaultCompileItems>false</EnableDefaultCompileItems><NuGetAudit>false</NuGetAudit>
<AssemblyName>{assembly}</AssemblyName>{executable}</PropertyGroup><ItemGroup>
<Compile Include="{escape(include)}"{exclude}/>{project_refs}</ItemGroup></Project>'''
    (directory / f"{name}.csproj").write_text(xml, encoding="utf-8")

project = str(scratch / "Probe/Probe.csproj")
for command in (
    ["dotnet", "--version"],
    ["dotnet", "restore", project, "--ignore-failed-sources"],
    ["dotnet", "build", project, "-c", "Release", "--no-restore"],
    ["dotnet", str(scratch / "Probe/bin/Release/net10.0/WP026.ReferenceGateProbe.dll")],
):
    subprocess.run(command, cwd=root, check=True)
