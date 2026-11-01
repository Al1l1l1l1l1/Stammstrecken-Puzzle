#!/usr/bin/env python3
"""Prepare the pinned Unity collector without changing measurement or old XML.

Unity 6000.3.23f1 exposes UInt32 hit counts. Coverage 1.3.0 narrows them to
Int32 when exporting. These exact source patches retain those measured values
in Int64 XML attributes and retain the existing complete Full export.
"""
from __future__ import annotations

import hashlib
import json
from pathlib import Path
import sys

sys.dont_write_bytecode = True

REPORTER = "Editor/CoverageFormats/OpenCover/OpenCoverReporter.cs"
UPSTREAM = "f6e219730c2b59f83b715e062a11785fc60e21d0f6f4e1594c3e970c50d610b2"
FULL = "5cc701951d59089b5c988e821af22ab3ef7d828100461582ba1b80dc290fb5b2"
FULL_REPLACEMENTS = (
    ("GenerateOpenCoverSession(CoverageReportType.CoveredMethodsOnly, testMethodInfo)",
     "GenerateOpenCoverSession(CoverageReportType.Full, testMethodInfo)", 1),
    ("m_Writer.WriteCoverageSession(CoverageReportType.CoveredMethodsOnly)",
     "m_Writer.WriteCoverageSession(CoverageReportType.Full)", 1),
)
RULES = (
    (REPORTER, FULL,
     "19cb7522b1d6380a7232318d77414580d14cc3326e05a3b386bc0ec90b7be840", (
         ("int totalVisitCount = 0;", "long totalVisitCount = 0;", 1),
         ("(int)classMethodSequencePoint.hitCount", "(long)classMethodSequencePoint.hitCount", 1),
         ("(int)coveredSequencePoint.hitCount", "(long)coveredSequencePoint.hitCount", 2),
     )),
    ("Editor/CoverageFormats/OpenCover/Model/InstrumentationPoint.cs",
     "98da9de5e84df2b8ed4f3b87d79c8707987a8022605c2f8cb63f1b0a6cb29f0c",
     "d0e640bf430017a90fa7b4e78f0f36832409bdc39dfe5d26bade1d1835467187", (
         ("public static int GetVisitCount(uint spid)", "public static long GetVisitCount(uint spid)", 1),
         ("public int VisitCount { get; set; }", "public long VisitCount { get; set; }", 1),
     )),
    ("Editor/CoverageFormats/OpenCover/Model/TrackedMethod.cs",
     "e8f587efc44555dcc2ccd8abb3eb6c13ca16d0715cd25df4150ce082be75b8b0",
     "bf92cdc41dd83256a3a477f4bc363f7c51dc15f4f5b6c40768d83c2c7e16427e", (
         ("public int VisitCount { get; set; }", "public long VisitCount { get; set; }", 1),
     )),
)


class PreparationError(RuntimeError):
    """Unknown collector sources or incomplete preparation; Unity must not run."""


def digest(raw: bytes) -> str:
    return hashlib.sha256(raw).hexdigest()


def replace_exact(raw: bytes, replacements: tuple) -> bytes:
    text = raw.decode("utf-8")
    for before, after, count in replacements:
        if text.count(before) != count:
            raise PreparationError("collector replacement count differs")
        text = text.replace(before, after)
    return text.encode("utf-8")


def checked_path(package: Path, relative: str) -> Path:
    path = package / relative
    for item in (path, *path.parents):
        if item.is_symlink() or (hasattr(item, "is_junction") and item.is_junction()):
            raise PreparationError("collector path contains a link")
        if item == package:
            break
    if not path.is_file() or not path.resolve().is_relative_to(package.resolve()):
        raise PreparationError("collector source missing or outside package")
    return path


def prepare(project: Path) -> list[dict]:
    project = project.resolve(strict=True)
    cache = project / "Library/PackageCache"
    for item in (cache, *cache.parents):
        if item.is_symlink() or (hasattr(item, "is_junction") and item.is_junction()):
            raise PreparationError("collector cache contains a link")
        if item == project:
            break
    packages = list(cache.glob("com.unity.testtools.codecoverage@*"))
    if len(packages) != 1:
        raise PreparationError("exactly Code Coverage 1.3.0 is required")
    package = packages[0]
    if package.is_symlink() or (hasattr(package, "is_junction") and package.is_junction()):
        raise PreparationError("collector package is linked")
    manifest = json.loads(checked_path(package, "package.json").read_bytes())
    if manifest.get("name") != "com.unity.testtools.codecoverage" or manifest.get("version") != "1.3.0":
        raise PreparationError("exactly Code Coverage 1.3.0 is required")
    plans = []
    for relative, original_hash, prepared_hash, replacements in RULES:
        path = checked_path(package, relative)
        original = path.read_bytes()
        current_hash = digest(original)
        prepared = original
        if current_hash != prepared_hash:
            if relative == REPORTER and current_hash == UPSTREAM:
                prepared = replace_exact(prepared, FULL_REPLACEMENTS)
            if digest(prepared) != original_hash:
                raise PreparationError(f"unknown collector source: {relative}")
            prepared = replace_exact(prepared, replacements)
            if digest(prepared) != prepared_hash:
                raise PreparationError(f"prepared collector source differs: {relative}")
        plans.append((path, original, prepared, relative))
    # Validate every input and every resulting hash before changing any file.
    attempted = []
    try:
        for path, original, prepared, relative in plans:
            if prepared != original:
                attempted.append((path, original))
                path.write_bytes(prepared)
    except OSError as error:
        for path, original in reversed(attempted):
            path.write_bytes(original)
        raise PreparationError("collector write failed; original sources restored") from error
    return [{"path": relative, "beforeSha256": digest(original),
             "afterSha256": digest(prepared)}
            for _, original, prepared, relative in plans]


def main() -> int:
    try:
        rows = prepare(Path.cwd())
    except (OSError, ValueError, PreparationError) as error:
        print(f"Coverage collector FAILURE: {error}", file=sys.stderr)
        return 1
    print("Coverage collector PASS: pinned 1.3.0 Full; genuine UInt32 API counts exported in Int64; "
          + json.dumps(rows, separators=(",", ":")))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
