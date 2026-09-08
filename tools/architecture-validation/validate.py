#!/usr/bin/env python3
from __future__ import annotations

import argparse
import base64
import copy
import hashlib
import json
import re
import subprocess
import sys
from collections import defaultdict
from pathlib import Path
from typing import Any, Callable

try:
    import jsonschema
except ImportError as exc:  # pragma: no cover - explicit setup failure
    print("SETUP ERROR: install tools/architecture-validation/requirements.lock.txt", file=sys.stderr)
    raise SystemExit(2) from exc

ROOT = Path(__file__).resolve().parents[2]
TOOL = Path(__file__).resolve().parent
ERRORS: list[str] = []
PASSES: list[str] = []

WP_HEADINGS = [
    "## ID",
    "## Ziel",
    "## Voraussetzungen",
    "## Scope",
    "## Betroffene Dateien/Module",
    "## Ausdrücklich nicht erlaubte Änderungen",
    "## Akzeptanzkriterien",
    "## Tests",
    "## Risikoklasse",
    "## Definition of Done",
]
ADR_HEADINGS = [
    "## Status",
    "## Datum",
    "## Kontext",
    "## Entscheidung",
    "## Begründung",
    "## Betrachtete Alternativen",
    "## Konsequenzen",
    "## Betroffene Artefakte",
    "## Validierung",
    "## Ersetzt / ersetzt durch",
]
CURRENT_ADR_STATUS = {
    1: "Angenommen",
    2: "Angenommen",
    3: "Ersetzt",
    4: "Angenommen",
    5: "Angenommen",
    6: "Ersetzt",
    7: "Angenommen",
    8: "Ersetzt",
    9: "Ersetzt",
    10: "Angenommen",
    11: "Angenommen",
    12: "Angenommen",
    13: "Angenommen",
    14: "Angenommen",
    15: "Angenommen",
    16: "Angenommen",
    17: "Angenommen",
}
SUPERSEDES = {3: 13, 6: 14, 8: 15, 9: 17}
ARCH_DOCS = [
    "ARCHITECTURE/ARCHITECTURE.md",
    "ARCHITECTURE/TECH_STACK.md",
    "ARCHITECTURE/MODULE_BOUNDARIES.md",
    "ARCHITECTURE/GAME_STATE_MODEL.md",
    "ARCHITECTURE/LEVEL_DATA_FORMAT.md",
    "ARCHITECTURE/PUZZLE_ENGINE.md",
    "ARCHITECTURE/SOLVER_ARCHITECTURE.md",
    "ARCHITECTURE/PERSISTENCE.md",
    "ARCHITECTURE/MOBILE_SERVICES.md",
    "ARCHITECTURE/CONTENT_PIPELINE.md",
    "ARCHITECTURE/CONTENT_CATALOGS.md",
    "ARCHITECTURE/OBSERVABILITY.md",
    "ARCHITECTURE/TEST_STRATEGY.md",
    "ARCHITECTURE/BUILD_AND_RELEASE.md",
    "ARCHITECTURE/OPEN_BLOCKERS.md",
]
SCHEMAS = {
    "level": ROOT / "ARCHITECTURE/schemas/level-v1.schema.json",
    "campaign": ROOT / "ARCHITECTURE/schemas/campaign-v1.schema.json",
    "completion": ROOT / "ARCHITECTURE/schemas/completion-v1.schema.json",
    "cosmetics": ROOT / "ARCHITECTURE/schemas/cosmetics-v1.schema.json",
}
EXAMPLES = {
    "level": [
        ROOT / "ARCHITECTURE/examples/level-v1.example.json",
        ROOT / "ARCHITECTURE/examples/level-v1.single-cell.example.json",
    ],
    "campaign": [ROOT / "ARCHITECTURE/examples/campaign-v1.example.json"],
    "completion": [ROOT / "ARCHITECTURE/examples/completion-v1.example.json"],
    "cosmetics": [ROOT / "ARCHITECTURE/examples/cosmetics-v1.example.json"],
}
REQUIRED_FILES = [
    *[ROOT / path for path in ARCH_DOCS],
    *SCHEMAS.values(),
    *[path for paths in EXAMPLES.values() for path in paths],
    ROOT / "DECISIONS/README.md",
    ROOT / "WORK_PACKAGES/WP-001_Technische_Produktionsspezifikation.md",
    ROOT / "WORK_PACKAGES/WP-002_Architecture-v0.2-Korrekturen.md",
    TOOL / "README.md",
    TOOL / "requirements.lock.txt",
    TOOL / "jcs_crosscheck.mjs",
    TOOL / "fixtures/duplicate-key.invalid.json",
    TOOL / "fixtures/float-token.invalid.json",
    TOOL / "fixtures/save-payload-v1.golden.json",
    TOOL / "fixtures/save-payload-v1.expected.json",
    TOOL / "fixtures/review-contracts-v0.2.json",
]

TRACK_PORTS = {
    "TRACK_NS": {"N", "S"},
    "TRACK_EW": {"E", "W"},
    "TRACK_NE": {"N", "E"},
    "TRACK_ES": {"E", "S"},
    "TRACK_SW": {"S", "W"},
    "TRACK_WN": {"W", "N"},
}
SIDE_DELTA = {"N": (0, -1), "E": (1, 0), "S": (0, 1), "W": (-1, 0)}
OPPOSITE = {"N": "S", "E": "W", "S": "N", "W": "E"}


def fail(code: str) -> None:
    ERRORS.append(code)


def run_group(name: str, function: Callable[[], None]) -> None:
    before = len(ERRORS)
    function()
    if len(ERRORS) == before:
        PASSES.append(name)


def reject_duplicates(pairs: list[tuple[str, Any]]) -> dict[str, Any]:
    result: dict[str, Any] = {}
    for key, value in pairs:
        if key in result:
            raise ValueError(f"duplicate key: {key}")
        result[key] = value
    return result


def load_json(path: Path) -> Any:
    return json.loads(path.read_text(encoding="utf-8"), object_pairs_hook=reject_duplicates)


def validate_string(value: str) -> None:
    for char in value:
        code = ord(char)
        if 0xD800 <= code <= 0xDFFF:
            raise ValueError("unpaired surrogate not allowed")


def utf16_sort_key(value: str) -> bytes:
    validate_string(value)
    return value.encode("utf-16be")


def jcs_text(value: Any) -> str:
    if value is None:
        return "null"
    if value is True:
        return "true"
    if value is False:
        return "false"
    if isinstance(value, int) and not isinstance(value, bool):
        if not -(2**53 - 1) <= value <= 2**53 - 1:
            raise ValueError("integer outside interoperable safe range")
        return str(value)
    if isinstance(value, float):
        raise ValueError("floats are forbidden by STP contracts")
    if isinstance(value, str):
        validate_string(value)
        return json.dumps(value, ensure_ascii=False, separators=(",", ":"))
    if isinstance(value, list):
        return "[" + ",".join(jcs_text(item) for item in value) + "]"
    if isinstance(value, dict):
        if not all(isinstance(key, str) for key in value):
            raise ValueError("JSON object key must be a string")
        keys = sorted(value, key=utf16_sort_key)
        return "{" + ",".join(jcs_text(key) + ":" + jcs_text(value[key]) for key in keys) + "}"
    raise ValueError(f"unsupported JSON type: {type(value).__name__}")


def jcs_bytes(value: Any) -> bytes:
    return jcs_text(value).encode("utf-8")


def digest(value: Any) -> str:
    return hashlib.sha256(jcs_bytes(value)).hexdigest()


def inventory_check() -> None:
    for path in REQUIRED_FILES:
        if not path.is_file() or path.stat().st_size == 0:
            fail(f"inventory:missing-or-empty:{path.relative_to(ROOT)}")
    adrs = sorted((ROOT / "DECISIONS").glob("ADR-*.md"))
    numbers = [int(re.match(r"ADR-(\d{3})-", path.name).group(1)) for path in adrs]
    if numbers != list(range(1, 18)):
        fail(f"inventory:adr-sequence:{numbers}")


def check_wp_identifiers(ids: list[str]) -> list[str]:
    errors = []
    if any(re.fullmatch(r"WP-[0-9]{3}", item) is None for item in ids):
        errors.append("wp:id-format")
    if len(ids) != len(set(ids)):
        errors.append("wp:id-duplicate")
    return errors


def work_package_check() -> None:
    files = sorted((ROOT / "WORK_PACKAGES").glob("WP-*.md"))
    ids: list[str] = []
    for path in files:
        match = re.match(r"(WP-[0-9]{3})_", path.name)
        if not match:
            fail(f"wp:filename:{path.name}")
            continue
        file_id = match.group(1)
        text = path.read_text(encoding="utf-8")
        title = re.search(r"^# (WP-[^ ]+) ", text, re.MULTILINE)
        body = re.search(r"^`(WP-[^`]+)`$", text, re.MULTILINE)
        if not title or not body or title.group(1) != file_id or body.group(1) != file_id:
            fail(f"wp:id-content:{path.name}")
        positions = [text.find(heading) for heading in WP_HEADINGS]
        if any(position < 0 for position in positions) or positions != sorted(positions):
            fail(f"wp:headings:{path.name}")
        ids.append(file_id)
    for error in check_wp_identifiers(ids):
        fail(error)
    all_repo_text = "\n".join(
        path.read_text(encoding="utf-8")
        for base in ("ARCHITECTURE", "DECISIONS", "PROJECT_CONTROL", "WORK_PACKAGES")
        for path in (ROOT / base).rglob("*")
        if path.is_file() and path.suffix in {".md", ".json"}
    )
    if re.search(r"WP-[A-Z]+-[0-9]+", all_repo_text):
        fail("wp:obsolete-nonconforming-id")


def adr_index_errors(index_text: str) -> list[str]:
    errors: list[str] = []
    for number in range(1, 18):
        token = f"ADR-{number:03d}"
        if token not in index_text:
            errors.append(f"adr-index:missing:{token}")
    if "Architecture v0.2" not in index_text or "13 sind angenommen" not in index_text or "4 bleiben als ersetzte" not in index_text:
        errors.append("adr-index:summary")
    return errors


def adr_check() -> None:
    index_text = (ROOT / "DECISIONS/README.md").read_text(encoding="utf-8")
    for error in adr_index_errors(index_text):
        fail(error)
    adrs = sorted((ROOT / "DECISIONS").glob("ADR-*.md"))
    by_number: dict[int, str] = {}
    for path in adrs:
        number = int(path.name[4:7])
        text = path.read_text(encoding="utf-8")
        positions = [text.find(heading) for heading in ADR_HEADINGS]
        if any(position < 0 for position in positions) or positions != sorted(positions):
            fail(f"adr:headings:{path.name}")
        status_match = re.search(r"## Status\s+\*\*(Angenommen|Ersetzt|Entwurf|Verworfen)\*\*", text)
        if not status_match or status_match.group(1) != CURRENT_ADR_STATUS[number]:
            fail(f"adr:status:{path.name}")
        by_number[number] = text
    for old, new in SUPERSEDES.items():
        if f"ADR-{new:03d}" not in by_number[old] or f"ADR-{old:03d}" not in by_number[new]:
            fail(f"adr:superseding:{old}:{new}")


def markdown_link_check() -> None:
    inline = re.compile(r"\[[^\]]+\]\(([^)]+)\)")
    reference = re.compile(r"^\[[^\]]+\]:\s+(\S+)", re.MULTILINE)
    md_files = [
        path
        for base in ("ARCHITECTURE", "DECISIONS", "PROJECT_CONTROL", "WORK_PACKAGES", "tools/architecture-validation")
        for path in (ROOT / base).rglob("*.md")
    ]
    for path in md_files:
        text = path.read_text(encoding="utf-8")
        for raw in inline.findall(text) + reference.findall(text):
            target = raw.strip().strip("<>")
            if target.startswith(("http://", "https://", "mailto:", "#")):
                continue
            target = target.split("#", 1)[0]
            if target and not (path.parent / target).resolve().exists():
                fail(f"link:broken:{path.relative_to(ROOT)}:{target}")


def schema_check() -> None:
    loaded_schemas: dict[str, Any] = {}
    for name, path in SCHEMAS.items():
        try:
            schema = load_json(path)
            jsonschema.Draft202012Validator.check_schema(schema)
            loaded_schemas[name] = schema
        except Exception as exc:
            fail(f"schema:{name}:{exc}")
    for name, paths in EXAMPLES.items():
        for path in paths:
            try:
                value = load_json(path)
                jsonschema.Draft202012Validator(loaded_schemas[name]).validate(value)
            except Exception as exc:
                fail(f"schema-example:{path.name}:{exc}")


def endpoint_cell(endpoint: dict[str, Any], width: int, height: int) -> tuple[int, int]:
    side, index = endpoint["side"], endpoint["index"]
    return {"N": (index, 0), "S": (index, height - 1), "W": (0, index), "E": (width - 1, index)}[side]


def level_projection_hashes(level: dict[str, Any]) -> tuple[str, str, str]:
    puzzle = {key: level[key] for key in ("schemaVersion", "rulesetVersion", "id", "grid", "endpoints", "rowCounts", "columnCounts")}
    solution = {"id": level["id"], "path": level["solution"]["path"]}
    proof = level["validation"]["proof"]
    proof_projection = {
        "solverVersion": level["validation"]["solverVersion"],
        "solutionCount": level["validation"]["solutionCount"],
        "searchNodes": proof["searchNodes"],
        "deductionSteps": proof["deductionSteps"],
        "maxDeductionDepth": proof["maxDeductionDepth"],
        "requiredGuessDepth": proof["requiredGuessDepth"],
    }
    return digest(puzzle), digest(solution), digest(proof_projection)


def enumerate_simple_coordinate_paths(level: dict[str, Any]) -> int:
    width, height = level["grid"]["width"], level["grid"]["height"]
    start = endpoint_cell(level["endpoints"]["a"], width, height)
    end = endpoint_cell(level["endpoints"]["b"], width, height)
    row_targets, col_targets = level["rowCounts"], level["columnCounts"]
    rows, cols = [0] * height, [0] * width
    rows[start[1]] = 1
    cols[start[0]] = 1
    count = 0

    def dfs(current: tuple[int, int], seen: set[tuple[int, int]], row_used: list[int], col_used: list[int]) -> None:
        nonlocal count
        if count >= 2:
            return
        if current == end:
            if row_used == row_targets and col_used == col_targets:
                count += 1
            return
        x, y = current
        for side in ("E", "S", "W", "N"):
            dx, dy = SIDE_DELTA[side]
            nxt = (x + dx, y + dy)
            nx, ny = nxt
            if not (0 <= nx < width and 0 <= ny < height) or nxt in seen:
                continue
            if row_used[ny] >= row_targets[ny] or col_used[nx] >= col_targets[nx]:
                continue
            next_rows, next_cols = row_used.copy(), col_used.copy()
            next_rows[ny] += 1
            next_cols[nx] += 1
            dfs(nxt, seen | {nxt}, next_rows, next_cols)

    dfs(start, {start}, rows, cols)
    return count


def validate_level_semantics(level: dict[str, Any]) -> list[str]:
    errors: list[str] = []
    width, height = level["grid"]["width"], level["grid"]["height"]
    a, b = level["endpoints"]["a"], level["endpoints"]["b"]
    if a == b:
        errors.append("level:endpoints-identical")
    if a["index"] >= (width if a["side"] in {"N", "S"} else height):
        errors.append("level:endpoint-a-range")
    if b["index"] >= (width if b["side"] in {"N", "S"} else height):
        errors.append("level:endpoint-b-range")
    path = level["solution"]["path"]
    if len(path) < 1:
        errors.append("level:path-empty")
        return errors
    coords = [(cell["x"], cell["y"]) for cell in path]
    if len(coords) != len(set(coords)):
        errors.append("level:path-duplicate")
    rows, cols = [0] * height, [0] * width
    for x, y in coords:
        if not (0 <= x < width and 0 <= y < height):
            errors.append("level:path-range")
            continue
        rows[y] += 1
        cols[x] += 1
    if rows != level["rowCounts"] or cols != level["columnCounts"]:
        errors.append("level:counts")
    for left, right in zip(path, path[1:]):
        delta = (right["x"] - left["x"], right["y"] - left["y"])
        direction = next((side for side, value in SIDE_DELTA.items() if value == delta), None)
        if direction is None or direction not in TRACK_PORTS[left["track"]] or OPPOSITE[direction] not in TRACK_PORTS[right["track"]]:
            errors.append("level:path-connection")
    if coords[0] != endpoint_cell(a, width, height) or a["side"] not in TRACK_PORTS[path[0]["track"]]:
        errors.append("level:endpoint-a-shape")
    if coords[-1] != endpoint_cell(b, width, height) or b["side"] not in TRACK_PORTS[path[-1]["track"]]:
        errors.append("level:endpoint-b-shape")
    expected = (
        level["validation"]["puzzleHashSha256"],
        level["validation"]["solutionHashSha256"],
        level["validation"]["proof"]["proofHashSha256"],
    )
    if level_projection_hashes(level) != expected:
        errors.append("level:hash")
    if enumerate_simple_coordinate_paths(level) != 1:
        errors.append("level:not-unique")
    return errors


def level_semantic_check() -> None:
    for path in EXAMPLES["level"]:
        for error in validate_level_semantics(load_json(path)):
            fail(f"{error}:{path.name}")


def catalog_semantic_errors(
    campaign: dict[str, Any],
    completion: dict[str, Any],
    cosmetics: dict[str, Any],
    level_ids: set[str],
    levels: list[dict[str, Any]],
) -> list[str]:
    errors: list[str] = []
    all_subject_ids: set[str] = set()
    campaign_level_ids: list[str] = []
    unlocks: dict[str, dict[str, Any]] = {}
    dependencies: dict[str, set[str]] = {}

    def add_subject(subject_id: str, unlock: dict[str, Any], parent_id: str | None = None) -> None:
        if subject_id in all_subject_ids:
            errors.append(f"catalog:duplicate-subject:{subject_id}")
        all_subject_ids.add(subject_id)
        unlocks[subject_id] = unlock
        dependencies[subject_id] = ({parent_id} if parent_id else set()) | set(unlock["requiredSubjectIds"])

    def check_order(items: list[dict[str, Any]], scope: str) -> None:
        ids = [item["id"] for item in items]
        orders = [item["order"] for item in items]
        if len(ids) != len(set(ids)):
            errors.append(f"catalog:duplicate-id:{scope}")
        if orders != list(range(1, len(items) + 1)):
            errors.append(f"catalog:order:{scope}")

    check_order(campaign["seasons"], "seasons")
    for season in campaign["seasons"]:
        add_subject(season["id"], season["unlock"])
        check_order(season["sections"], season["id"])
        for section in season["sections"]:
            add_subject(section["id"], section["unlock"], season["id"])
            if not section["id"].startswith(season["id"] + "-"):
                errors.append(f"catalog:section-parent:{section['id']}")
            check_order(section["routes"], section["id"])
            for route in section["routes"]:
                add_subject(route["id"], route["unlock"], section["id"])
                if not route["id"].startswith(section["id"] + "-"):
                    errors.append(f"catalog:route-parent:{route['id']}")
                levels_in_route = route["levels"]
                level_orders = [item["order"] for item in levels_in_route]
                if level_orders != list(range(1, len(levels_in_route) + 1)):
                    errors.append(f"catalog:order:{route['id']}")
                for level_ref in levels_in_route:
                    level_id = level_ref["levelId"]
                    campaign_level_ids.append(level_id)
                    add_subject(level_id, level_ref["unlock"], route["id"])
                    if not level_id.startswith(route["id"] + "-"):
                        errors.append(f"catalog:level-parent:{level_id}")

    if len(campaign_level_ids) != len(set(campaign_level_ids)):
        errors.append("catalog:campaign-level-duplicate")
    if not set(campaign_level_ids).issubset(level_ids):
        errors.append("catalog:campaign-level-reference")
    for subject_id, unlock in unlocks.items():
        refs = unlock["requiredSubjectIds"]
        if unlock["kind"] == "ALWAYS" and refs:
            errors.append("catalog:always-has-references")
        if unlock["kind"] == "ALL_FIRST_CLEARS" and (not refs or not set(refs).issubset(all_subject_ids)):
            errors.append("catalog:unlock-reference")
        if subject_id in refs:
            errors.append(f"catalog:unlock-self-reference:{subject_id}")

    visiting: set[str] = set()
    visited: set[str] = set()

    def has_cycle(subject_id: str) -> bool:
        if subject_id in visiting:
            return True
        if subject_id in visited:
            return False
        visiting.add(subject_id)
        for dependency in dependencies.get(subject_id, set()):
            if dependency in all_subject_ids and has_cycle(dependency):
                return True
        visiting.remove(subject_id)
        visited.add(subject_id)
        return False

    if any(has_cycle(subject_id) for subject_id in sorted(all_subject_ids)):
        errors.append("catalog:unlock-cycle")

    reachable: set[str] = set()
    changed = True
    while changed:
        changed = False
        for subject_id in sorted(all_subject_ids - reachable):
            unlock = unlocks[subject_id]
            if unlock["kind"] == "ALWAYS" and dependencies[subject_id].issubset(reachable):
                reachable.add(subject_id)
                changed = True
            elif unlock["kind"] == "ALL_FIRST_CLEARS" and dependencies[subject_id] and dependencies[subject_id].issubset(reachable):
                reachable.add(subject_id)
                changed = True
    if reachable != all_subject_ids:
        errors.append("catalog:unlock-unreachable")

    moment_ids = [item["id"] for item in completion["completionMoments"]]
    if len(moment_ids) != len(set(moment_ids)):
        errors.append("catalog:completion-id-duplicate")
    reason_codes = [item["reasonCode"] for item in completion["rewardDefinitions"]]
    if len(reason_codes) != len(set(reason_codes)):
        errors.append("catalog:reward-code-duplicate")
    moment_tuples = {
        (item["mapSegmentId"], item["trainMomentId"], item["resultTextKey"])
        for item in completion["completionMoments"]
    }
    for level in levels:
        value = level["completion"]
        if (value["mapSegmentId"], value["trainMomentId"], value["resultTextKey"]) not in moment_tuples:
            errors.append(f"catalog:completion-reference:{level['id']}")
    reward = {item["reasonCode"]: item for item in completion["rewardDefinitions"]}
    if reward.get("LEVEL_FIRST_CLEAR", {}).get("amountPatience") != 15 or reward.get("POST_CLEAR_PATIENCE", {}).get("amountPatience") != 10:
        errors.append("catalog:confirmed-reward-values")
    cosmetic_ids = [item["id"] for item in cosmetics["items"]]
    if len(cosmetic_ids) != len(set(cosmetic_ids)):
        errors.append("catalog:cosmetic-duplicate")
    return errors


def catalog_check() -> None:
    campaign = load_json(EXAMPLES["campaign"][0])
    completion = load_json(EXAMPLES["completion"][0])
    cosmetics = load_json(EXAMPLES["cosmetics"][0])
    levels = [load_json(path) for path in EXAMPLES["level"]]
    for error in catalog_semantic_errors(campaign, completion, cosmetics, {level["id"] for level in levels}, levels):
        fail(error)


def parse_assembly_graph(text: str) -> tuple[set[str], dict[str, set[str]]]:
    section = text.split("## 3. Einzige normative Produktionsassembly-Allowlist", 1)[1].split("## 4.", 1)[0]
    nodes: set[str] = set()
    raw_rows: list[tuple[str, str]] = []
    for line in section.splitlines():
        if not line.startswith("| `STP."):
            continue
        columns = [column.strip() for column in line.strip().strip("|").split("|")]
        assembly = columns[0].strip("`")
        nodes.add(assembly)
        raw_rows.append((assembly, columns[2]))
    edges: dict[str, set[str]] = {node: set() for node in nodes}
    for assembly, references in raw_rows:
        for target in re.findall(r"`(STP\.[A-Za-z.]+)`", references):
            if target not in nodes:
                fail(f"assembly:unknown-target:{assembly}:{target}")
            else:
                edges[assembly].add(target)
    return nodes, edges


def graph_has_cycle(nodes: set[str], edges: dict[str, set[str]]) -> bool:
    visiting: set[str] = set()
    visited: set[str] = set()

    def visit(node: str) -> bool:
        if node in visiting:
            return True
        if node in visited:
            return False
        visiting.add(node)
        if any(visit(target) for target in edges.get(node, set())):
            return True
        visiting.remove(node)
        visited.add(node)
        return False

    return any(visit(node) for node in sorted(nodes))


def assembly_check() -> None:
    text = (ROOT / "ARCHITECTURE/MODULE_BOUNDARIES.md").read_text(encoding="utf-8")
    nodes, edges = parse_assembly_graph(text)
    if graph_has_cycle(nodes, edges):
        fail("assembly:cycle")
    required = {"STP.Puzzle.Domain", "STP.Puzzle.Solver", "STP.Application", "STP.Bootstrap", "STP.Platform"}
    if not required.issubset(nodes):
        fail("assembly:required-node")
    if "STP.MobileServices.Contracts" in nodes:
        fail("assembly:obsolete-contracts-node")
    if edges.get("STP.MobileServices.Google") != {"STP.Application"} or edges.get("STP.MobileServices.Store") != {"STP.Application"}:
        fail("assembly:adapter-direction")


def review_contract_errors(contract: dict[str, Any], module_text: str) -> list[str]:
    errors: list[str] = []
    if contract.get("architectureVersion") != "0.2" or contract.get("workPackagePattern") != "^WP-[0-9]{3}$":
        errors.append("review:version-or-wp")
    if contract["assembly"].get("portsOwner") != "STP.Application":
        errors.append("review:ports-owner")
    save = contract["save"]
    if save.get("hashProfile") != "STP-SAVE-JCS-1" or save.get("schemaVersion") != 1:
        errors.append("review:save-profile")
    if (save.get("journalMaxEntries"), save.get("journalMaxCanonicalBytes"), save.get("compactAtEntries"), save.get("compactAtCanonicalBytes"), save.get("diagnosticTailEntries")) != (512, 262144, 384, 196608, 64):
        errors.append("review:ledger-bounds")
    reward = contract["reward"]
    if reward.get("claimTemplate") != "reward-claim:POST_CLEAR_PATIENCE:<levelId>" or reward.get("providerIdIsBusinessKey") is not False:
        errors.append("review:reward-claim")
    iap = contract["iap"]
    if iap.get("grantBeforeStoreFinalize") is not True or iap.get("googleFinalization") != "ACKNOWLEDGE" or iap.get("appleFinalization") != "FINISH":
        errors.append("review:iap-order")
    privacy = contract["privacy"]
    if any(privacy.get(key) is not False for key in ("analyticsDefault", "crashReportsDefault", "adsBeforeCanRequestAds", "unityDeveloperDataPackagesIncluded", "unknownCapabilityValue")):
        errors.append("review:privacy-default")
    device = contract["deviceEvidence"]
    if device.get("emulatorAlonePasses") is not False or device.get("simulatorAlonePasses") is not False or device.get("physicalAndroidRequired") is not True or device.get("physicalIosRequired") is not True:
        errors.append("review:device-evidence")
    endless = contract["endless"]
    expected_projection = ["endlessContractVersion", "rulesetVersion", "generatorVersion", "seed", "generationOrdinal", "parameterHashSha256"]
    if (
        endless.get("contractVersion") != 1
        or endless.get("idPrefix") != "E1-"
        or endless.get("identityProjection") != expected_projection
        or endless.get("maxActiveDescriptors") != 20
        or endless.get("maxTerminalIntervals") != 64
        or endless.get("terminalDetailWindow") != 64
    ):
        errors.append("review:endless-identity")
    phrase = "eine fachlich kohärente, einzeln testbare änderung mit explizit aufgelisteten betroffenen modulen"
    if phrase not in module_text.lower():
        errors.append("review:wp-module-rule")
    return errors


DOCUMENT_REVIEW_TOKENS: dict[str, list[tuple[str, str]]] = {
    "ARCH-REV-003": [
        ("ARCHITECTURE/PERSISTENCE.md", "STP-SAVE-JCS-1"),
        ("ARCHITECTURE/PERSISTENCE.md", "Byte Order Mark"),
        ("ARCHITECTURE/PERSISTENCE.md", "UTF-16-Codeeinheiten"),
        ("ARCHITECTURE/PERSISTENCE.md", "ohne Unicode-Normalisierung"),
        ("ARCHITECTURE/PERSISTENCE.md", "Fließkommazahlen"),
        ("ARCHITECTURE/PERSISTENCE.md", "SAVE_HASH_PROFILE_UNSUPPORTED"),
        ("ARCHITECTURE/PERSISTENCE.md", "Golden Tests"),
    ],
    "ARCH-REV-004": [
        ("ARCHITECTURE/PERSISTENCE.md", "reward-claim:POST_CLEAR_PATIENCE:<levelId>"),
        ("ARCHITECTURE/PERSISTENCE.md", "Provider-Reward-ID"),
        ("ARCHITECTURE/PERSISTENCE.md", "`RESERVED`"),
        ("ARCHITECTURE/PERSISTENCE.md", "`RECONCILIATION_REQUIRED`"),
        ("ARCHITECTURE/PERSISTENCE.md", "gemeinsamen Savecommit"),
    ],
    "ARCH-REV-005": [
        ("ARCHITECTURE/MOBILE_SERVICES.md", "Receipt-/Tokenantwort"),
        ("ARCHITECTURE/MOBILE_SERVICES.md", "GRANTED_NOT_FINALIZED"),
        ("ARCHITECTURE/MOBILE_SERVICES.md", "Google Purchase acknowledge"),
        ("ARCHITECTURE/MOBILE_SERVICES.md", "Apple Transaction finish"),
        ("ARCHITECTURE/MOBILE_SERVICES.md", "REVOCATION_CONFIRMED"),
        ("ARCHITECTURE/MOBILE_SERVICES.md", "Restore"),
    ],
    "ARCH-REV-006": [
        ("ARCHITECTURE/MOBILE_SERVICES.md", "firebase_analytics_collection_enabled=false"),
        ("ARCHITECTURE/MOBILE_SERVICES.md", "FIREBASE_ANALYTICS_COLLECTION_ENABLED=NO"),
        ("ARCHITECTURE/MOBILE_SERVICES.md", "firebase_crashlytics_collection_enabled=false"),
        ("ARCHITECTURE/MOBILE_SERVICES.md", "FirebaseCrashlyticsCollectionEnabled=false"),
        ("ARCHITECTURE/MOBILE_SERVICES.md", "deleteUnsentReports"),
        ("ARCHITECTURE/MOBILE_SERVICES.md", "Unity Installation ID"),
        ("ARCHITECTURE/MOBILE_SERVICES.md", "CanRequestAds"),
        ("ARCHITECTURE/MOBILE_SERVICES.md", "physischen"),
    ],
    "ARCH-REV-007": [
        ("ARCHITECTURE/CONTENT_CATALOGS.md", "ICampaignCatalog"),
        ("ARCHITECTURE/CONTENT_CATALOGS.md", "ICompletionCatalog"),
        ("ARCHITECTURE/CONTENT_CATALOGS.md", "ICosmeticsCatalog"),
        ("ARCHITECTURE/CONTENT_CATALOGS.md", "autoritative Preisquelle"),
        ("ARCHITECTURE/CONTENT_CATALOGS.md", "cosmetic-purchase:<itemId>"),
        ("ARCHITECTURE/CONTENT_CATALOGS.md", "catalogRevision"),
        ("ARCHITECTURE/CONTENT_CATALOGS.md", "Unlockgraph einschließlich impliziter Elternabhängigkeiten"),
    ],
    "ARCH-REV-008": [
        ("ARCHITECTURE/MODULE_BOUNDARIES.md", "IHapticsPort"),
        ("ARCHITECTURE/MODULE_BOUNDARIES.md", "ISaveSyncPort` existiert nicht"),
        ("ARCHITECTURE/MODULE_BOUNDARIES.md", "STP.MobileServices.Contracts"),
    ],
    "ARCH-REV-009": [
        ("ARCHITECTURE/PERSISTENCE.md", "höchstens 512 Einträge"),
        ("ARCHITECTURE/PERSISTENCE.md", "256 KiB"),
        ("ARCHITECTURE/PERSISTENCE.md", "höchstens 20 aktive Records"),
        ("ARCHITECTURE/PERSISTENCE.md", "höchstens 64 Intervalle"),
        ("ARCHITECTURE/PERSISTENCE.md", "jüngsten 64 terminalen Instanzen"),
    ],
    "ARCH-REV-010": [
        ("ARCHITECTURE/SOLVER_ARCHITECTURE.md", "generatorVersion"),
        ("ARCHITECTURE/SOLVER_ARCHITECTURE.md", "parameterHashSha256"),
        ("ARCHITECTURE/SOLVER_ARCHITECTURE.md", "E1-<hex>"),
        ("ARCHITECTURE/SOLVER_ARCHITECTURE.md", "öffentlichen Puzzleinput"),
        ("ARCHITECTURE/SOLVER_ARCHITECTURE.md", "ENDLESS_RETENTION_LIMIT"),
        ("ARCHITECTURE/SOLVER_ARCHITECTURE.md", "jüngsten 64 terminalen Instanzen"),
    ],
    "ARCH-REV-011": [
        ("PROJECT_CONTROL/WORK_PACKAGE_RULES.md", "fachlich kohärente, einzeln testbare Änderung mit explizit aufgelisteten betroffenen Modulen"),
        ("ARCHITECTURE/MODULE_BOUNDARIES.md", "fachlich kohärente, einzeln testbare Änderung mit explizit aufgelisteten betroffenen Modulen"),
    ],
}


def document_review_errors(documents: dict[str, str]) -> list[str]:
    errors: list[str] = []
    for finding, requirements in DOCUMENT_REVIEW_TOKENS.items():
        for path, token in requirements:
            if token.lower() not in documents[path].lower():
                errors.append(f"review-doc:{finding}:{path}:{token}")
    return errors


def review_contract_check() -> None:
    contract = load_json(TOOL / "fixtures/review-contracts-v0.2.json")
    module_text = (ROOT / "ARCHITECTURE/MODULE_BOUNDARIES.md").read_text(encoding="utf-8")
    for error in review_contract_errors(contract, module_text):
        fail(error)
    paths = {path for requirements in DOCUMENT_REVIEW_TOKENS.values() for path, _ in requirements}
    documents = {path: (ROOT / path).read_text(encoding="utf-8") for path in paths}
    for error in document_review_errors(documents):
        fail(error)


def cross_tool_hash_check() -> None:
    fixture = TOOL / "fixtures/save-payload-v1.golden.json"
    value = load_json(fixture)
    expected = load_json(TOOL / "fixtures/save-payload-v1.expected.json")
    python_bytes = jcs_bytes(value)
    try:
        load_json(TOOL / "fixtures/duplicate-key.invalid.json")
        fail("jcs:python-duplicate-key-not-rejected")
    except ValueError:
        pass
    try:
        jcs_bytes(load_json(TOOL / "fixtures/float-token.invalid.json"))
        fail("jcs:python-float-token-not-rejected")
    except ValueError:
        pass
    completed = subprocess.run(
        ["node", str(TOOL / "jcs_crosscheck.mjs"), str(fixture)],
        cwd=ROOT,
        text=True,
        capture_output=True,
        check=False,
    )
    if completed.returncode != 0:
        fail(f"jcs:node-exit:{completed.returncode}:{completed.stderr.strip()}")
        return
    node_result = json.loads(completed.stdout)
    if base64.b64decode(node_result["canonicalBase64"]) != python_bytes:
        fail("jcs:cross-tool-bytes")
    if node_result["sha256"] != hashlib.sha256(python_bytes).hexdigest():
        fail("jcs:cross-tool-hash")
    if expected.get("hashProfile") != "STP-SAVE-JCS-1" or expected.get("saveSchemaVersion") != 1:
        fail("jcs:expected-profile")
    if expected.get("canonicalByteLength") != len(python_bytes):
        fail("jcs:golden-byte-length")
    if expected.get("payloadSha256") != hashlib.sha256(python_bytes).hexdigest():
        fail("jcs:golden-hash")
    if python_bytes.startswith(b"\xef\xbb\xbf") or python_bytes.endswith(b"\n"):
        fail("jcs:bom-or-newline")
    invalid = subprocess.run(
        ["node", str(TOOL / "jcs_crosscheck.mjs"), str(TOOL / "fixtures/duplicate-key.invalid.json")],
        cwd=ROOT,
        text=True,
        capture_output=True,
        check=False,
    )
    if invalid.returncode == 0 or "duplicate key" not in invalid.stderr:
        fail("jcs:node-duplicate-key-not-rejected")
    float_invalid = subprocess.run(
        ["node", str(TOOL / "jcs_crosscheck.mjs"), str(TOOL / "fixtures/float-token.invalid.json")],
        cwd=ROOT,
        text=True,
        capture_output=True,
        check=False,
    )
    if float_invalid.returncode == 0 or "floating-point" not in float_invalid.stderr:
        fail("jcs:node-float-token-not-rejected")


def architecture_version_check() -> None:
    if not (ROOT / "ARCHITECTURE/ARCHITECTURE.md").read_text(encoding="utf-8").startswith("# Stammstrecken-Puzzle – Architecture v0.2"):
        fail("version:architecture")
    for rel in ARCH_DOCS:
        if rel.endswith("LEVEL_DATA_FORMAT.md"):
            continue
        first_line = (ROOT / rel).read_text(encoding="utf-8").splitlines()[0]
        if "v0.1" in first_line:
            fail(f"version:heading:{rel}")
    current = (ROOT / "PROJECT_CONTROL/CURRENT_STATE.md").read_text(encoding="utf-8")
    if "Architecture v0.2" not in current:
        fail("version:current-state")


def blockers_check() -> None:
    text = (ROOT / "ARCHITECTURE/OPEN_BLOCKERS.md").read_text(encoding="utf-8")
    for number in (1, 2, 3):
        if f"BLOCKER-PROD-00{number}" not in text:
            fail(f"blocker:missing:{number}")
    if text.count("| Status | **Offen** |") != 3 or text.count("Fail-closed") < 3:
        fail("blocker:status-or-fail-closed")


def git_scope_check() -> None:
    main_local = subprocess.run(["git", "-C", str(ROOT), "rev-parse", "main"], text=True, capture_output=True, check=True).stdout.strip()
    main_remote = subprocess.run(["git", "-C", str(ROOT), "rev-parse", "origin/main"], text=True, capture_output=True, check=True).stdout.strip()
    if main_local != main_remote:
        fail("git:main-changed")
    tracked = subprocess.run(["git", "-C", str(ROOT), "diff", "--name-only", "origin/main"], text=True, capture_output=True, check=True).stdout.splitlines()
    untracked = subprocess.run(["git", "-C", str(ROOT), "ls-files", "--others", "--exclude-standard"], text=True, capture_output=True, check=True).stdout.splitlines()
    changed = sorted(set(tracked + untracked))
    allowed_prefixes = ("ARCHITECTURE/", "DECISIONS/", "PROJECT_CONTROL/", "WORK_PACKAGES/", "tools/architecture-validation/")
    forbidden_suffixes = {".cs", ".asmdef", ".unity", ".prefab", ".uxml", ".uss", ".shader"}
    for rel in changed:
        if not rel.startswith(allowed_prefixes):
            fail(f"git:out-of-scope:{rel}")
        if Path(rel).suffix.lower() in forbidden_suffixes:
            fail(f"git:production-code:{rel}")
        if rel.startswith("Stammstrecken_Puzzle_Konzept_00-15/"):
            fail(f"git:product-file:{rel}")


def self_test() -> None:
    failures: list[str] = []

    def expect(label: str, condition: bool) -> None:
        if not condition:
            failures.append(label)

    expect("ARCH-REV-001", bool(check_wp_identifiers(["WP-001", "WP-02"])))
    index = (ROOT / "DECISIONS/README.md").read_text(encoding="utf-8").replace("ADR-017", "ADR-X17")
    expect("ARCH-REV-002", bool(adr_index_errors(index)))

    document_paths = {path for requirements in DOCUMENT_REVIEW_TOKENS.values() for path, _ in requirements}
    documents = {path: (ROOT / path).read_text(encoding="utf-8") for path in document_paths}

    def detects_removed_token(finding: str, path: str, token: str) -> bool:
        mutated_documents = documents.copy()
        mutated_documents[path] = re.sub(re.escape(token), "REMOVED-CONTRACT", mutated_documents[path], flags=re.IGNORECASE)
        return any(error.startswith(f"review-doc:{finding}:") for error in document_review_errors(mutated_documents))

    expect("ARCH-REV-003", detects_removed_token("ARCH-REV-003", "ARCHITECTURE/PERSISTENCE.md", "STP-SAVE-JCS-1"))
    expect("ARCH-REV-004", detects_removed_token("ARCH-REV-004", "ARCHITECTURE/PERSISTENCE.md", "reward-claim:POST_CLEAR_PATIENCE:<levelId>"))
    expect("ARCH-REV-005", detects_removed_token("ARCH-REV-005", "ARCHITECTURE/MOBILE_SERVICES.md", "GRANTED_NOT_FINALIZED"))
    expect("ARCH-REV-006", detects_removed_token("ARCH-REV-006", "ARCHITECTURE/MOBILE_SERVICES.md", "firebase_analytics_collection_enabled=false"))

    campaign = load_json(EXAMPLES["campaign"][0])
    completion = load_json(EXAMPLES["completion"][0])
    cosmetics = load_json(EXAMPLES["cosmetics"][0])
    levels = [load_json(path) for path in EXAMPLES["level"]]
    completion["completionMoments"].append(copy.deepcopy(completion["completionMoments"][0]))
    catalog_errors = catalog_semantic_errors(campaign, completion, cosmetics, {level["id"] for level in levels}, levels)
    cyclic_campaign = load_json(EXAMPLES["campaign"][0])
    level_unlock = cyclic_campaign["seasons"][0]["sections"][0]["routes"][0]["levels"][0]["unlock"]
    level_unlock["kind"] = "ALL_FIRST_CLEARS"
    level_unlock["requiredSubjectIds"] = [cyclic_campaign["seasons"][0]["sections"][0]["routes"][0]["levels"][0]["levelId"]]
    cycle_errors = catalog_semantic_errors(cyclic_campaign, load_json(EXAMPLES["completion"][0]), cosmetics, {level["id"] for level in levels}, levels)
    expect(
        "ARCH-REV-007",
        "catalog:completion-id-duplicate" in catalog_errors
        and any(error.startswith("catalog:unlock-self-reference:") for error in cycle_errors)
        and "catalog:unlock-cycle" in cycle_errors
        and detects_removed_token("ARCH-REV-007", "ARCHITECTURE/CONTENT_CATALOGS.md", "Unlockgraph einschließlich impliziter Elternabhängigkeiten"),
    )

    text = (ROOT / "ARCHITECTURE/MODULE_BOUNDARIES.md").read_text(encoding="utf-8")
    nodes, edges = parse_assembly_graph(text)
    edges["STP.Puzzle.Domain"].add("STP.Application")
    expect("ARCH-REV-008", graph_has_cycle(nodes, edges))

    expect("ARCH-REV-009", detects_removed_token("ARCH-REV-009", "ARCHITECTURE/PERSISTENCE.md", "höchstens 512 Einträge"))
    expect("ARCH-REV-010", detects_removed_token("ARCH-REV-010", "ARCHITECTURE/SOLVER_ARCHITECTURE.md", "ENDLESS_RETENTION_LIMIT"))
    expect("ARCH-REV-011", detects_removed_token("ARCH-REV-011", "PROJECT_CONTROL/WORK_PACKAGE_RULES.md", "fachlich kohärente, einzeln testbare Änderung mit explizit aufgelisteten betroffenen Modulen"))

    schema = load_json(SCHEMAS["level"])
    schema["properties"]["solution"]["properties"]["path"]["minItems"] = 2
    single_cell = load_json(EXAMPLES["level"][1])
    expect("ARCH-REV-012", bool(list(jsonschema.Draft202012Validator(schema).iter_errors(single_cell))))

    if failures:
        for label in failures:
            fail(f"self-test:not-detected:{label}")
    else:
        PASSES.append("Mutations-Selbsttest: ARCH-REV-001 bis ARCH-REV-012 werden erkannt")


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate Stammstrecken-Puzzle Architecture v0.2")
    parser.add_argument("--self-test", action="store_true", help="run 12 negative mutation tests after normal validation")
    args = parser.parse_args()

    groups: list[tuple[str, Callable[[], None]]] = [
        ("Datei-/ADR-Inventar", inventory_check),
        ("Work-Package-IDs und Struktur", work_package_check),
        ("ADR-Index, Status und Superseding", adr_check),
        ("Relative Markdownlinks", markdown_link_check),
        ("JSON Schema Draft 2020-12", schema_check),
        ("Levelsemantik, Hashes und Eindeutigkeit", level_semantic_check),
        ("Campaign-/Completion-/Cosmetics-Cross-References", catalog_check),
        ("Zyklusfreier Assemblygraph", assembly_check),
        ("Zwölf Architecture-v0.2-Reviewverträge", review_contract_check),
        ("Save-JCS Python/Node-Crosscheck", cross_tool_hash_check),
        ("Architecture-v0.2-Status", architecture_version_check),
        ("Drei offene fail-closed Produktblocker", blockers_check),
        ("Git-Scope und unverändertes main", git_scope_check),
    ]
    for name, function in groups:
        run_group(name, function)
    if args.self_test:
        self_test()

    print("ARCHITECTURE VALIDATION v0.2")
    for item in PASSES:
        print(f"PASS  {item}")
    if ERRORS:
        for item in ERRORS:
            print(f"FAIL  {item}")
        print(f"RESULT FAIL ({len(ERRORS)} Fehler)")
        return 1
    print(f"RESULT PASS ({len(PASSES)} Prüfgruppen)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
