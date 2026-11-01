#!/usr/bin/env python3
"""Verify one Unity/OpenCover initial + final Full inventory without changing XML.

The final Full file is the denominator, never a merge of visits from snapshots.
Unity's JIT may fill previously zero source columns.  Additional offsets are
accepted only for the narrowly checked generated async MoveNext refinement
described in compare_method; this is not a general aggregate-count tolerance.
Only Python's standard library is used. No minimum coverage is imposed.
"""
from __future__ import annotations

import argparse
from collections import defaultdict
from dataclasses import dataclass
from decimal import Decimal, InvalidOperation
import hashlib
import json
from pathlib import Path
import re
import sys
import xml.etree.ElementTree as ET

sys.dont_write_bytecode = True

REQUIRED_MODULES = frozenset({
    "STP.Application", "STP.Bootstrap", "STP.Editor.Build",
    "STP.MobileServices.Google", "STP.MobileServices.Store",
    "STP.Presentation.UI", "STP.Presentation.World", "STP.Tests.Bootstrap.PlayMode",
    "STP.Puzzle.Domain", "STP.Puzzle.Solver",
    "STP.Tests.Domain.EditMode", "STP.Tests.Solver.EditMode",
    "STP.Infrastructure.Content", "STP.Tests.Content.EditMode",
})
COUNTERS = ("numSequencePoints", "visitedSequencePoints", "numBranchPoints",
            "visitedBranchPoints", "numMethods", "visitedMethods", "numClasses", "visitedClasses")
MEASURED_METHOD_ATTRIBUTES = {"visited", "sequenceCoverage", "branchCoverage", "crapScore"}
POINT_ATTRIBUTES = {"vc", "uspid", "ordinal", "offset", "sl", "sc", "el", "ec", "bec", "bev", "fileid"}


class CoverageError(ValueError):
    """Incomplete, inconsistent, ambiguous or unsupported evidence."""


def require(condition: bool, message: str) -> None:
    if not condition:
        raise CoverageError(message)


def number(raw: str | None, context: str) -> int:
    require(raw is not None and re.fullmatch(r"[0-9]+", raw) is not None, f"{context}: invalid/missing count")
    return int(raw)  # type: ignore[arg-type]


def children(node: ET.Element, tag: str, context: str) -> list[ET.Element]:
    require(all(child.tag == tag for child in node), f"{context}: unsupported child")
    return list(node)


def one(node: ET.Element, tag: str, context: str) -> ET.Element:
    matches = node.findall(tag)
    require(len(matches) == 1, f"{context}: exactly one {tag} required")
    return matches[0]


def text(node: ET.Element, tag: str, context: str) -> str:
    value = one(node, tag, context).text
    require(bool(value and value.strip()) and value == value.strip(), f"{context}: invalid/missing {tag}")
    return value  # type: ignore[return-value]


def percent(raw: str | None, visited: int, total: int, context: str) -> None:
    try:
        value = Decimal(raw if raw is not None else "NaN")
    except InvalidOperation as error:
        raise CoverageError(f"{context}: invalid percentage") from error
    expected = Decimal(100) * visited / total if total else Decimal(0)
    # The XML displays two decimal places (Unity uses midpoint-to-even). This
    # half-display-unit bound concerns only rendering; ALL integer row counts
    # below must match exactly, and the report recomputes the actual ratio.
    require(value.is_finite() and 0 <= value <= 100 and
            abs(value - expected) <= Decimal(".005"), f"{context}: contradictory percentage")


def summary(node: ET.Element, expected: dict[str, int], context: str) -> dict[str, int]:
    item = one(node, "Summary", context)
    values = {field: number(item.get(field), f"{context}/{field}") for field in COUNTERS}
    require(values == expected, f"{context}: Summary does not equal actual point/method/class rows: {values} != {expected}")
    for kind in ("SequencePoints", "BranchPoints", "Methods", "Classes"):
        require(values["visited" + kind] <= values["num" + kind], f"{context}: visited exceeds total")
    percent(item.get("sequenceCoverage"), values["visitedSequencePoints"], values["numSequencePoints"], context + "/sequenceCoverage")
    percent(item.get("branchCoverage"), values["visitedBranchPoints"], values["numBranchPoints"], context + "/branchCoverage")
    return values


def sum_counts(items: list[dict[str, int]]) -> dict[str, int]:
    return {key: sum(item[key] for item in items) for key in COUNTERS}


@dataclass(frozen=True)
class Point:
    offset: int
    file: str
    line: int
    end_line: int
    column: int
    end_column: int
    visits: int
    ordinal: int
    branch_exits: int
    visited_branch_exits: int

    @property
    def source(self) -> tuple[str, int, int, int, int]:
        return self.file, self.line, self.end_line, self.column, self.end_column


@dataclass
class Method:
    name: str
    class_name: str
    token: int
    file_ref: str
    attributes: dict[str, str]
    points: dict[int, Point]
    branches: dict[tuple[tuple[str, str], ...], int]
    counts: dict[str, int]


@dataclass
class Module:
    name: str
    metadata: tuple
    files: dict[str, tuple[tuple[str, str], ...]]
    classes: frozenset[str]
    methods: dict[str, Method]
    counts: dict[str, int]


@dataclass
class Snapshot:
    path: Path
    sha256: str
    attributes: dict[str, str]
    modules: dict[str, Module]
    counts: dict[str, int]


def element_identity(node: ET.Element) -> tuple:
    return node.tag, tuple(sorted(node.attrib.items())), (node.text or "").strip(), tuple(element_identity(c) for c in node)


def read_method(node: ET.Element, class_name: str, files: dict, seen_ids: set[int], context: str) -> Method:
    require({c.tag for c in node} <= {"Summary", "MetadataToken", "Name", "FileRef", "SequencePoints", "BranchPoints"},
            f"{context}: unsupported method shape")
    name = text(node, "Name", context)
    require(set(node.attrib) == MEASURED_METHOD_ATTRIBUTES | {"cyclomaticComplexity", "nPathComplexity", "isConstructor", "isStatic", "isGetter", "isSetter"},
            f"{context}: missing/unsupported method attributes")
    for key in ("isConstructor", "isStatic", "isGetter", "isSetter"):
        require(node.get(key) in {"true", "false"}, f"{context}: invalid method flag")
    for key in ("cyclomaticComplexity", "nPathComplexity"):
        number(node.get(key), context + "/" + key)
    token = number(text(node, "MetadataToken", context), context + "/MetadataToken")
    require(token >> 24 == 6 and token & 0xFFFFFF > 0, f"{context}: invalid MethodDef token")
    ref = one(node, "FileRef", context).get("uid")
    require(ref in files, f"{context}: unknown method FileRef")
    require(node.get("visited") in {"true", "false"}, f"{context}: invalid visited flag")
    points: dict[int, Point] = {}
    for p in children(one(node, "SequencePoints", context), "SequencePoint", context):
        require(set(p.attrib) == POINT_ATTRIBUTES and not list(p), f"{context}: unsupported sequence-point shape")
        values = {k: number(p.get(k), context + "/" + k) for k in POINT_ATTRIBUTES - {"fileid"}}
        uid = p.get("fileid")
        # Partial-class field initializers can legitimately reference another
        # mapped file; comparison still preserves each original point's file.
        require(uid in files, f"{context}: unknown point FileRef")
        require(values["sl"] > 0 and (values["el"], values["ec"]) >= (values["sl"], values["sc"]), f"{context}: invalid source range")
        require((values["sc"] == 0) == (values["ec"] == 0), f"{context}: partially missing columns")
        require(values["uspid"] > 0 and values["uspid"] not in seen_ids, f"{context}: duplicate/invalid point id")
        require(values["offset"] not in points, f"{context}: duplicate sequence offset")
        require(values["bev"] <= values["bec"], f"{context}: invalid visited branch exits")
        seen_ids.add(values["uspid"])
        points[values["offset"]] = Point(values["offset"], uid, values["sl"], values["el"], values["sc"], values["ec"],
                                          values["vc"], values["ordinal"], values["bec"], values["bev"])
    require(bool(points), f"{context}: method has no sequence points")
    require(node.get("visited") == str(any(p.visits > 0 for p in points.values())).lower(), f"{context}: visited flag contradicts vc rows")
    branches: dict[tuple[tuple[str, str], ...], int] = {}
    for p in children(one(node, "BranchPoints", context), "BranchPoint", context):
        require(set(p.attrib) == {"vc", "uspid", "ordinal", "offset", "sl", "path", "offsetend", "fileid"} and not list(p), f"{context}: unsupported branch-point shape")
        for field in set(p.attrib) - {"fileid"}:
            number(p.get(field), context + "/branch/" + field)
        require(p.get("fileid") in files, f"{context}: unknown branch FileRef")
        uid = int(p.attrib["uspid"])
        require(uid > 0 and uid not in seen_ids, f"{context}: duplicate/invalid branch id")
        seen_ids.add(uid)
        identity = tuple(sorted((k, v) for k, v in p.attrib.items() if k not in {"vc", "uspid"}))
        require(identity not in branches, f"{context}: duplicate branch point")
        branches[identity] = int(p.attrib["vc"])
    expected = dict(zip(COUNTERS, (len(points), sum(p.visits > 0 for p in points.values()), len(branches),
                                  sum(v > 0 for v in branches.values()), 1, int(node.get("visited") == "true"), 0, 0)))
    counts = summary(node, expected, context)
    percent(node.get("sequenceCoverage"), counts["visitedSequencePoints"], counts["numSequencePoints"], context)
    percent(node.get("branchCoverage"), counts["visitedBranchPoints"], counts["numBranchPoints"], context)
    attributes = {k: v for k, v in node.attrib.items() if k not in MEASURED_METHOD_ATTRIBUTES}
    return Method(name, class_name, token, ref, attributes, points, branches, counts)


def read_snapshot(path: Path) -> Snapshot:
    try:
        raw = path.read_bytes()
        require(b"<!DOCTYPE" not in raw.upper() and b"<!ENTITY" not in raw.upper(), f"{path}: XML entities unsupported")
        root = ET.fromstring(raw)
    except (OSError, ET.ParseError) as error:
        raise CoverageError(f"{path}: unreadable/invalid XML: {error}") from error
    require(root.tag == "CoverageSession" and {c.tag for c in root} == {"Summary", "Modules"}, f"{path}: no Full CoverageSession")
    modules: dict[str, Module] = {}
    seen_ids: set[int] = set()
    for node in children(one(root, "Modules", str(path)), "Module", str(path)):
        require({c.tag for c in node} <= {"Summary", "ModuleName", "ModulePath", "ModuleTime", "Files", "Classes"},
                f"{path}: unsupported Full module shape")
        for tag in ("ModulePath", "ModuleTime"):
            require(len(node.findall(tag)) <= 1, f"{path}: duplicate module metadata")
        name = text(node, "ModuleName", str(path))
        require(name not in modules, f"{path}: duplicate module {name}")
        context = f"{path}/{name}"
        files = {}
        paths = set()
        for f in children(one(node, "Files", context), "File", context):
            uid, source = f.get("uid"), f.get("fullPath")
            require(set(f.attrib) == {"uid", "fullPath"} and not list(f), f"{context}: unsupported File shape")
            require(number(uid, context + "/File/uid") > 0 and uid not in files and bool(source) and source not in paths,
                    f"{context}: duplicate/invalid source file")
            files[uid] = tuple(sorted(f.attrib.items()))
            paths.add(source)
        require(bool(files), f"{context}: empty File map")
        methods: dict[str, Method] = {}
        tokens: set[int] = set()
        classes: set[str] = set()
        class_counts = []
        for c in children(one(node, "Classes", context), "Class", context):
            require(not c.attrib and {child.tag for child in c} == {"Summary", "FullName", "Methods"},
                    f"{context}: unsupported Class shape")
            cn = text(c, "FullName", context)
            require(cn not in classes, f"{context}: duplicate class")
            classes.add(cn)
            members = []
            for m in children(one(c, "Methods", context), "Method", context):
                method = read_method(m, cn, files, seen_ids, context)
                require(method.name not in methods and method.token not in tokens, f"{context}: duplicate method/token")
                methods[method.name] = method
                tokens.add(method.token)
                members.append(method.counts)
            require(bool(members), f"{context}: empty class")
            counts = sum_counts(members)
            counts.update(numClasses=1, visitedClasses=int(any(m["visitedMethods"] for m in members)))
            class_counts.append(summary(c, counts, context + "/" + cn))
        require(bool(methods), f"{context}: empty method inventory")
        counts = summary(node, sum_counts(class_counts), context)
        metadata = (tuple(sorted(node.attrib.items())), tuple(element_identity(c) for c in node if c.tag not in {"Summary", "Files", "Classes"}))
        modules[name] = Module(name, metadata, files, frozenset(classes), methods, counts)
    require(bool(modules), f"{path}: no modules")
    counts = summary(root, sum_counts([m.counts for m in modules.values()]), str(path))
    return Snapshot(path, hashlib.sha256(raw).hexdigest(), dict(root.attrib), modules, counts)


def compare_method(before: Method, after: Method, context: str) -> dict | None:
    require((before.class_name, before.token, before.file_ref, before.attributes) ==
            (after.class_name, after.token, after.file_ref, after.attributes), f"{context}: method metadata/source changed")
    require(before.branches.keys() == after.branches.keys(), f"{context}: branch inventory changed")
    require(before.points.keys() <= after.points.keys(), f"{context}: original offsets removed")
    refined = 0
    for offset, old in before.points.items():
        current = after.points[offset]
        require((old.file, old.line, old.end_line, old.ordinal, old.branch_exits) ==
                (current.file, current.line, current.end_line, current.ordinal, current.branch_exits),
                f"{context}: original offset/file/line/range changed")
        if (old.column, old.end_column) != (current.column, current.end_column):
            require(old.column == old.end_column == 0 and current.column > 0 and current.end_column > 0,
                    f"{context}: existing nonzero columns changed/removed")
            refined += 1
    added = after.points.keys() - before.points.keys()
    if added:
        # Unity's evidenced async-lambda JIT expansion inserts a pair of offsets
        # inside existing IL bounds. Each pair points to the SAME source span as
        # the preceding retained anchor, whose absent columns have just resolved.
        # Both retained anchors must resolve columns; no new source span, branch,
        # method, file, first/last IL offset, or previously known column may change.
        match = re.fullmatch(r"System\.Void (.+/<.+>d(?:__\d+)?)::MoveNext\(\)", before.name)
        require(match is not None and match[1] == before.class_name and
                all(before.attributes.get(k) == "false" for k in ("isConstructor", "isStatic", "isGetter", "isSetter")),
                f"{context}: additional offsets outside generated async MoveNext")
        require(not before.branches and refined > 0, f"{context}: unsupported async refinement")
        brackets: dict[tuple[int, int], list[Point]] = defaultdict(list)
        original_offsets = sorted(before.points)
        for offset in sorted(added):
            require(original_offsets[0] < offset < original_offsets[-1], f"{context}: new offset outside original method range")
            lo = max(o for o in original_offsets if o < offset)
            hi = min(o for o in original_offsets if o > offset)
            old_l, old_r = before.points[lo], before.points[hi]
            left, right, point = after.points[lo], after.points[hi], after.points[offset]
            require(old_l.column == old_l.end_column == old_r.column == old_r.end_column == 0 and
                    left.column > 0 and right.column > 0 and
                    point.source == left.source and point.file == right.file and
                    min(left.line, right.line) <= point.line <= max(left.end_line, right.end_line) and
                    point.ordinal == left.ordinal == right.ordinal == 0 and
                    point.branch_exits == point.visited_branch_exits == 0,
                    f"{context}: new offset is not a resolved-column async anchor refinement")
            brackets[lo, hi].append(point)
        require(all(len(pair) == 2 and pair[0].source == pair[1].source and pair[0].visits == pair[1].visits
                    for pair in brackets.values()), f"{context}: unsupported async offset-pair shape")
    if not added and not refined:
        return None
    return {"method": before.name, "metadataToken": before.token,
            "baselineSequencePoints": len(before.points), "finalSequencePoints": len(after.points),
            "resolvedColumnPoints": refined, "addedAsyncOffsets": sorted(added)}


def verify(directory: Path) -> dict:
    require(directory.is_dir(), f"{directory}: coverage directory missing")
    paths = sorted(directory.rglob("TestCoverageResults_*.xml"))
    require(bool(paths), "no TestCoverageResults_*.xml")
    for path in paths:
        require(re.fullmatch(r"TestCoverageResults_[0-9]{4}\.xml", path.name) is not None, f"unsupported data filename: {path}")
    require(len({p.parent for p in paths}) == 1, "multiple coverage sessions")
    initial = [p for p in paths if p.name == "TestCoverageResults_0000.xml"]
    final = [p for p in paths if p.name != "TestCoverageResults_0000.xml"]
    require(len(initial) == 1 and len(final) == 1, "exactly initial 0000 + one final Full measurement required")
    before, after = read_snapshot(initial[0]), read_snapshot(final[0])
    require(before.attributes == after.attributes, "CoverageSession metadata changed")
    require(before.modules.keys() == after.modules.keys(), "baseline/final module inventory differs")
    require(REQUIRED_MODULES <= before.modules.keys(), f"required module(s) missing: {sorted(REQUIRED_MODULES - before.modules.keys())}")
    require(all(before.counts[k] == 0 for k in ("visitedSequencePoints", "visitedBranchPoints", "visitedMethods", "visitedClasses")), "initial 0000 is not an unvisited inventory")
    report = {"denominator": "final Full actual row inventory", "initial": {"path": str(before.path), "rawSha256": before.sha256},
              "final": {"path": str(after.path), "rawSha256": after.sha256}, "minimumCoverage": None, "modules": []}
    for name in sorted(before.modules):
        old, current = before.modules[name], after.modules[name]
        require(old.metadata == current.metadata and old.files == current.files and old.classes == current.classes,
                f"{name}: module metadata/File map/class inventory changed")
        require(old.methods.keys() == current.methods.keys(), f"{name}: method inventory changed")
        refinements = [item for key in sorted(old.methods)
                       if (item := compare_method(old.methods[key], current.methods[key], name + "/" + key)) is not None]
        if not name.startswith("STP."):
            continue
        count = current.counts
        require(count["numSequencePoints"] > 0 and count["numMethods"] > 0, f"{name}: no usable denominator")
        delta = count["numSequencePoints"] - old.counts["numSequencePoints"]
        require(delta == sum(len(r["addedAsyncOffsets"]) for r in refinements), f"{name}: unexplained sequence-count delta")
        report["modules"].append({"name": name, "baselineSequencePoints": old.counts["numSequencePoints"],
                                  "finalFullSequencePointsDenominator": count["numSequencePoints"],
                                  "visitedSequencePoints": count["visitedSequencePoints"], "methodDenominator": count["numMethods"],
                                  "visitedMethods": count["visitedMethods"], "addedAsyncSequencePoints": delta,
                                  "resolvedColumnPoints": sum(r["resolvedColumnPoints"] for r in refinements),
                                  "refinements": refinements})
    return report


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("directory", nargs="?", type=Path, default=Path("TestResults/Coverage"))
    parser.add_argument("--report", type=Path, help="new derived JSON report; never overwrite an existing path")
    args = parser.parse_args(argv)
    try:
        report = verify(args.directory)
        if args.report is not None:
            # exclusive creation preserves prior reports and prevents raw-XML overwrite
            with args.report.open("x", encoding="utf-8", newline="\n") as stream:
                json.dump(report, stream, indent=2)
                stream.write("\n")
        print("Coverage: denominator = final Full actual row inventory; initial 0000 retained and compared.")
        for module in report["modules"]:
            n, d = module["visitedSequencePoints"], module["finalFullSequencePointsDenominator"]
            print(f"{module['name']}: {n}/{d} sequence points ({100 * n / d:.1f}%), methods "
                  f"{module['visitedMethods']}/{module['methodDenominator']}; "
                  f"initial {module['baselineSequencePoints']}, +{module['addedAsyncSequencePoints']} checked async points, "
                  f"{module['resolvedColumnPoints']} resolved source columns")
            for item in module["refinements"]:
                if item["addedAsyncOffsets"]:
                    print(f"  generated MoveNext refinement: token {item['metadataToken']}, "
                          f"offsets {item['addedAsyncOffsets']}, {item['method']}")
        print(f"Coverage evidence PASS: {len(report['modules'])} STP assemblies; no minimum coverage; no XML rewritten.")
        return 0
    except (CoverageError, OSError) as error:
        print(f"Coverage evidence FAILURE: {error}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
