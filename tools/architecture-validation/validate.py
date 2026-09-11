#!/usr/bin/env python3
from __future__ import annotations

import argparse
import base64
import copy
import fnmatch
import hashlib
import json
import re
import subprocess
import sys
from pathlib import Path
from typing import Any, Callable

try:
    import jsonschema
except ImportError as exc:
    print("SETUP ERROR: install tools/architecture-validation/requirements.lock.txt", file=sys.stderr)
    raise SystemExit(2) from exc

ROOT = Path(__file__).resolve().parents[2]
TOOL = Path(__file__).resolve().parent
ERRORS: list[str] = []
PASSES: list[tuple[str, str]] = []

WP_HEADINGS = [
    "## ID", "## Ziel", "## Voraussetzungen", "## Scope",
    "## Betroffene Dateien/Module", "## Ausdrücklich nicht erlaubte Änderungen",
    "## Akzeptanzkriterien", "## Tests", "## Risikoklasse", "## Definition of Done",
]
ADR_HEADINGS = [
    "## Status", "## Datum", "## Kontext", "## Entscheidung", "## Begründung",
    "## Betrachtete Alternativen", "## Konsequenzen", "## Betroffene Artefakte",
    "## Validierung", "## Ersetzt / ersetzt durch",
]
CURRENT_ADR_STATUS = {
    1: "Angenommen", 2: "Angenommen", 3: "Ersetzt", 4: "Ersetzt",
    5: "Angenommen", 6: "Ersetzt", 7: "Angenommen", 8: "Ersetzt",
    9: "Ersetzt", 10: "Angenommen", 11: "Angenommen", 12: "Angenommen",
    13: "Angenommen", 14: "Ersetzt", 15: "Ersetzt", 16: "Angenommen",
    17: "Ersetzt", 18: "Angenommen", 19: "Angenommen", 20: "Angenommen",
    21: "Angenommen", 22: "Angenommen", 23: "Angenommen",
}
SUPERSEDES = {3: 13, 4: 21, 6: 14, 8: 15, 9: 17, 14: 19, 15: 20, 17: 22}
ARCH_DOCS = [
    "ARCHITECTURE/ARCHITECTURE.md", "ARCHITECTURE/TECH_STACK.md",
    "ARCHITECTURE/MODULE_BOUNDARIES.md", "ARCHITECTURE/GAME_STATE_MODEL.md",
    "ARCHITECTURE/LEVEL_DATA_FORMAT.md", "ARCHITECTURE/PUZZLE_ENGINE.md",
    "ARCHITECTURE/SOLVER_ARCHITECTURE.md", "ARCHITECTURE/PERSISTENCE.md",
    "ARCHITECTURE/MOBILE_SERVICES.md", "ARCHITECTURE/CONTENT_PIPELINE.md",
    "ARCHITECTURE/CONTENT_CATALOGS.md", "ARCHITECTURE/OBSERVABILITY.md",
    "ARCHITECTURE/TEST_STRATEGY.md", "ARCHITECTURE/BUILD_AND_RELEASE.md",
    "ARCHITECTURE/OPEN_BLOCKERS.md", "ARCHITECTURE/PRIVACY_PROVIDER_EVIDENCE.md",
]
SCHEMA_EXAMPLES: dict[str, tuple[str, list[str]]] = {
    "level-v1": ("ARCHITECTURE/schemas/level-v1.schema.json", ["ARCHITECTURE/examples/level-v1.example.json", "ARCHITECTURE/examples/level-v1.single-cell.example.json"]),
    "level-v2": ("ARCHITECTURE/schemas/level-v2.schema.json", ["ARCHITECTURE/examples/level-v2.example.json", "ARCHITECTURE/examples/level-v2.single-cell.example.json"]),
    "proof-v1": ("ARCHITECTURE/schemas/proof-v1.schema.json", ["ARCHITECTURE/examples/proof-v1.example.json", "ARCHITECTURE/examples/proof-v1.single-cell.example.json"]),
    "campaign-v1": ("ARCHITECTURE/schemas/campaign-v1.schema.json", ["ARCHITECTURE/examples/campaign-v1.example.json"]),
    "campaign-v2": ("ARCHITECTURE/schemas/campaign-v2.schema.json", ["ARCHITECTURE/examples/campaign-v2.example.json"]),
    "completion-v1": ("ARCHITECTURE/schemas/completion-v1.schema.json", ["ARCHITECTURE/examples/completion-v1.example.json"]),
    "cosmetics-v1": ("ARCHITECTURE/schemas/cosmetics-v1.schema.json", ["ARCHITECTURE/examples/cosmetics-v1.example.json"]),
    "cosmetics-v2": ("ARCHITECTURE/schemas/cosmetics-v2.schema.json", ["ARCHITECTURE/examples/cosmetics-v2.example.json"]),
    "release-lock-v1": ("ARCHITECTURE/schemas/release-lock-v1.schema.json", ["ARCHITECTURE/examples/release-lock-v1.example.json"]),
    "release-manifest-v1": ("ARCHITECTURE/schemas/release-manifest-v1.schema.json", ["ARCHITECTURE/examples/release-manifest-v1.rc.example.json", "ARCHITECTURE/examples/release-manifest-v1.staging.example.json"]),
    "scope-manifest-v1": ("tools/architecture-validation/scope-manifest-v1.schema.json", ["tools/architecture-validation/scopes/WP-003.documentation.scope.json"]),
}
REQUIRED_FILES = [
    *[ROOT / item for item in ARCH_DOCS],
    *[ROOT / schema for schema, _ in SCHEMA_EXAMPLES.values()],
    *[ROOT / example for _, examples in SCHEMA_EXAMPLES.values() for example in examples],
    *[ROOT / f"DECISIONS/ADR-{number:03d}-" for number in []],
    ROOT / "DECISIONS/README.md",
    ROOT / "WORK_PACKAGES/WP-001_Technische_Produktionsspezifikation.md",
    ROOT / "WORK_PACKAGES/WP-002_Architecture-v0.2-Korrekturen.md",
    ROOT / "WORK_PACKAGES/WP-003_Architecture-v0.3-Finalkorrekturen.md",
    TOOL / "README.md", TOOL / "requirements.lock.txt", TOOL / "jcs_crosscheck.mjs",
    TOOL / "fixtures/duplicate-key.invalid.json", TOOL / "fixtures/float-token.invalid.json",
    TOOL / "fixtures/save-payload-v1.golden.json", TOOL / "fixtures/save-payload-v1.expected.json",
    TOOL / "fixtures/review-contracts-v0.3.json", TOOL / "fixtures/endless-save-v2.example.json",
    TOOL / "fixtures/endless-save-v1-to-v2.golden.json", TOOL / "fixtures/iap-state-machine-v1.json",
    TOOL / "fixtures/privacy-lifecycle-v1.json", TOOL / "fixtures/level-progress-v1-to-v2.golden.json",
    ROOT / "ARCHITECTURE/examples/promotion-receipt-v1.example.json",
]
TRACK_PORTS = {
    "TRACK_NS": {"N", "S"}, "TRACK_EW": {"E", "W"}, "TRACK_NE": {"N", "E"},
    "TRACK_ES": {"E", "S"}, "TRACK_SW": {"S", "W"}, "TRACK_WN": {"W", "N"},
}
SIDE_DELTA = {"N": (0, -1), "E": (1, 0), "S": (0, 1), "W": (-1, 0)}
OPPOSITE = {"N": "S", "E": "W", "S": "N", "W": "E"}
EXPECTED_BOOTSTRAP_REFS = {
    "STP.Application", "STP.Audio", "STP.Infrastructure.Content", "STP.Infrastructure.Persistence",
    "STP.MobileServices.Google", "STP.MobileServices.Store", "STP.Platform",
    "STP.Presentation.UI", "STP.Presentation.World",
}


def fail(code: str) -> None:
    ERRORS.append(code)


def run_group(category: str, name: str, function: Callable[[], None]) -> None:
    before = len(ERRORS)
    try:
        function()
    except Exception as exc:  # accumulate rather than hiding later failures
        fail(f"{name}:exception:{type(exc).__name__}:{exc}")
    if len(ERRORS) == before:
        PASSES.append((category, name))


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
    if any(0xD800 <= ord(char) <= 0xDFFF for char in value):
        raise ValueError("unpaired surrogate not allowed")


def utf16_sort_key(value: str) -> bytes:
    validate_string(value)
    return value.encode("utf-16be")


def jcs_text(value: Any) -> str:
    if value is None: return "null"
    if value is True: return "true"
    if value is False: return "false"
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
    if numbers != list(range(1, 24)):
        fail(f"inventory:adr-sequence:{numbers}")


def check_wp_identifiers(ids: list[str]) -> list[str]:
    errors: list[str] = []
    if any(re.fullmatch(r"WP-[0-9]{3}", item) is None for item in ids): errors.append("wp:id-format")
    if len(ids) != len(set(ids)): errors.append("wp:id-duplicate")
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
        if any(value < 0 for value in positions) or positions != sorted(positions):
            fail(f"wp:headings:{path.name}")
        ids.append(file_id)
    for error in check_wp_identifiers(ids): fail(error)
    all_text = "\n".join(
        path.read_text(encoding="utf-8")
        for base in ("ARCHITECTURE", "DECISIONS", "PROJECT_CONTROL", "WORK_PACKAGES")
        for path in (ROOT / base).rglob("*") if path.is_file() and path.suffix in {".md", ".json"}
    )
    if re.search(r"WP-[A-Z]+-[0-9]+", all_text): fail("wp:obsolete-nonconforming-id")


def adr_index_errors(index_text: str) -> list[str]:
    errors: list[str] = []
    for number in range(1, 24):
        token = f"ADR-{number:03d}"
        if token not in index_text:
            errors.append(f"adr-index:missing:{number:03d}")
            continue
        row = next((line for line in index_text.splitlines() if line.startswith(f"| [{token}]")), None)
        if row is None or f"**{CURRENT_ADR_STATUS[number]}**" not in row:
            errors.append(f"adr-index:status:{number:03d}")
        expected_file = next(ROOT.glob(f"DECISIONS/{token}-*.md"), None)
        if row is not None and (expected_file is None or f"[{token}](./{expected_file.name})" not in row):
            errors.append(f"adr-index:link:{number:03d}")
    if "Architecture v0.3" not in index_text or "15 sind angenommen" not in index_text or "8 bleiben als ersetzte" not in index_text:
        errors.append("adr-index:summary")
    return errors


def adr_check() -> None:
    index = (ROOT / "DECISIONS/README.md").read_text(encoding="utf-8")
    for error in adr_index_errors(index): fail(error)
    texts: dict[int, str] = {}
    for path in sorted((ROOT / "DECISIONS").glob("ADR-*.md")):
        number = int(path.name[4:7])
        text = path.read_text(encoding="utf-8")
        positions = [text.find(heading) for heading in ADR_HEADINGS]
        if any(value < 0 for value in positions) or positions != sorted(positions): fail(f"adr:headings:{path.name}")
        match = re.search(r"## Status\s+\*\*(Angenommen|Ersetzt|Entwurf|Verworfen)\*\*", text)
        if not match or match.group(1) != CURRENT_ADR_STATUS[number]: fail(f"adr:status:{path.name}")
        texts[number] = text
    for old, new in SUPERSEDES.items():
        if f"ADR-{new:03d}" not in texts[old] or f"ADR-{old:03d}" not in texts[new]:
            fail(f"adr:superseding:{old}:{new}")
    if "vollständig ersetzt durch [adr-021]" not in texts[4].lower() or "ersetzt [adr-004]" not in texts[21].lower() or "vollständig" not in texts[21].lower(): fail("adr:004-021-full-replacement")
    if "adr-004 bleibt" in texts[16].lower(): fail("adr:016-stale-004-validity")


def markdown_link_check() -> None:
    inline = re.compile(r"\[[^\]]+\]\(([^)]+)\)")
    reference = re.compile(r"^\[[^\]]+\]:\s+(\S+)", re.MULTILINE)
    for base in ("ARCHITECTURE", "DECISIONS", "PROJECT_CONTROL", "WORK_PACKAGES", "tools/architecture-validation"):
        for path in (ROOT / base).rglob("*.md"):
            text = path.read_text(encoding="utf-8")
            for raw in inline.findall(text) + reference.findall(text):
                target = raw.strip().strip("<>").split("#", 1)[0]
                if not target or target.startswith(("http://", "https://", "mailto:")): continue
                if not (path.parent / target).resolve().exists(): fail(f"link:broken:{path.relative_to(ROOT)}:{target}")


def schema_check() -> None:
    for name, (schema_rel, example_rels) in SCHEMA_EXAMPLES.items():
        try:
            schema = load_json(ROOT / schema_rel)
            jsonschema.Draft202012Validator.check_schema(schema)
            validator = jsonschema.Draft202012Validator(schema)
            for rel in example_rels: validator.validate(load_json(ROOT / rel))
        except Exception as exc:
            fail(f"schema:{name}:{exc}")


def endpoint_cell(endpoint: dict[str, Any], width: int, height: int) -> tuple[int, int]:
    side, index = endpoint["side"], endpoint["index"]
    return {"N": (index, 0), "S": (index, height - 1), "W": (0, index), "E": (width - 1, index)}[side]


def enumerate_simple_coordinate_paths(level: dict[str, Any]) -> int:
    width, height = level["grid"]["width"], level["grid"]["height"]
    start = endpoint_cell(level["endpoints"]["a"], width, height)
    end = endpoint_cell(level["endpoints"]["b"], width, height)
    rows, cols = [0] * height, [0] * width
    rows[start[1]], cols[start[0]] = 1, 1
    count = 0
    def dfs(current: tuple[int, int], seen: set[tuple[int, int]], row_used: list[int], col_used: list[int]) -> None:
        nonlocal count
        if count >= 2: return
        if current == end:
            if row_used == level["rowCounts"] and col_used == level["columnCounts"]: count += 1
            return
        x, y = current
        for side in ("E", "S", "W", "N"):
            dx, dy = SIDE_DELTA[side]
            nxt = (x + dx, y + dy)
            nx, ny = nxt
            if not (0 <= nx < width and 0 <= ny < height) or nxt in seen: continue
            if row_used[ny] >= level["rowCounts"][ny] or col_used[nx] >= level["columnCounts"][nx]: continue
            new_rows, new_cols = row_used.copy(), col_used.copy()
            new_rows[ny] += 1; new_cols[nx] += 1
            dfs(nxt, seen | {nxt}, new_rows, new_cols)
    dfs(start, {start}, rows, cols)
    return count


def basic_path_errors(level: dict[str, Any]) -> list[str]:
    errors: list[str] = []
    width, height = level["grid"]["width"], level["grid"]["height"]
    a, b = level["endpoints"]["a"], level["endpoints"]["b"]
    if a == b: errors.append("level:endpoints-identical")
    for label, endpoint in (("a", a), ("b", b)):
        limit = width if endpoint["side"] in {"N", "S"} else height
        if endpoint["index"] >= limit: errors.append(f"level:endpoint-{label}-range")
    path = level["solution"]["path"]
    coords = [(cell["x"], cell["y"]) for cell in path]
    if len(coords) != len(set(coords)): errors.append("level:path-duplicate")
    rows, cols = [0] * height, [0] * width
    for x, y in coords:
        if not (0 <= x < width and 0 <= y < height): errors.append("level:path-range")
        else: rows[y] += 1; cols[x] += 1
    if rows != level["rowCounts"] or cols != level["columnCounts"]: errors.append("level:counts")
    for left, right in zip(path, path[1:]):
        delta = (right["x"] - left["x"], right["y"] - left["y"])
        direction = next((side for side, vector in SIDE_DELTA.items() if vector == delta), None)
        if direction is None or direction not in TRACK_PORTS[left["track"]] or OPPOSITE[direction] not in TRACK_PORTS[right["track"]]:
            errors.append("level:path-connection")
    if coords[0] != endpoint_cell(a, width, height) or a["side"] not in TRACK_PORTS[path[0]["track"]]: errors.append("level:endpoint-a-shape")
    if coords[-1] != endpoint_cell(b, width, height) or b["side"] not in TRACK_PORTS[path[-1]["track"]]: errors.append("level:endpoint-b-shape")
    if enumerate_simple_coordinate_paths(level) != 1: errors.append("level:not-unique")
    return errors


def v1_hash_errors(level: dict[str, Any]) -> list[str]:
    puzzle = {key: level[key] for key in ("schemaVersion", "rulesetVersion", "id", "grid", "endpoints", "rowCounts", "columnCounts")}
    solution = {"id": level["id"], "path": level["solution"]["path"]}
    p = level["validation"]["proof"]
    proof = {"solverVersion": level["validation"]["solverVersion"], "solutionCount": level["validation"]["solutionCount"], "searchNodes": p["searchNodes"], "deductionSteps": p["deductionSteps"], "maxDeductionDepth": p["maxDeductionDepth"], "requiredGuessDepth": p["requiredGuessDepth"]}
    expected = (level["validation"]["puzzleHashSha256"], level["validation"]["solutionHashSha256"], p["proofHashSha256"])
    return [] if (digest(puzzle), digest(solution), digest(proof)) == expected else ["level-v1:hash"]


def v2_binding_errors(level: dict[str, Any], proof: dict[str, Any]) -> list[str]:
    errors = basic_path_errors(level)
    match = re.fullmatch(r"S([1-9][0-9]*)-([0-9]{2})-([0-9]{2})-([0-9]{2})", level["puzzleId"])
    content = level["content"]
    if not match or tuple(map(int, match.groups())) != (content["season"], content["networkSection"], content["route"], content["position"]): errors.append("level-v2:id-content")
    if content["season"] == 1:
        if not 1 <= content["networkSection"] <= 5: errors.append("level-v2:s1-section-range")
        if not 1 <= content["route"] <= 4: errors.append("level-v2:s1-route-range")
        if not 1 <= content["position"] <= 12: errors.append("level-v2:s1-position-range")
    thresholds = level["production"]["starThresholdsSeconds"]
    if thresholds is not None and thresholds["threeStars"] >= thresholds["twoStars"]: errors.append("level-v2:time-order")
    public_projection = {key: level[key] for key in ("puzzleId", "rulesetVersion", "grid", "endpoints", "rowCounts", "columnCounts")}
    public_hash = {"profile": "STP-PUZZLE-SEMANTIC-JCS-1", "sha256": digest(public_projection)}
    solution_projection = {"puzzleId": level["puzzleId"], "publicPuzzleHash": public_hash, "path": level["solution"]["path"]}
    solution_hash = {"profile": "STP-SOLUTION-JCS-1", "sha256": digest(solution_projection)}
    if proof.get("puzzleId") != level["puzzleId"]: errors.append("proof:puzzle-id")
    if proof.get("publicPuzzleHash") != public_hash: errors.append("proof:puzzle-hash")
    if proof.get("solutionHash") != solution_hash: errors.append("proof:solution-hash")
    proof_projection = {key: value for key, value in proof.items() if key != "proofHash"}
    expected_proof_hash = {"profile": "STP-PROOF-JCS-1", "sha256": digest(proof_projection)}
    if proof.get("proofHash") != expected_proof_hash: errors.append("proof:self-hash")
    if level["proofRef"].get("proofHash") != expected_proof_hash or level["proofRef"].get("proofFormatVersion") != proof.get("proofFormatVersion"):
        errors.append("proof:level-reference")
    return errors


def level_and_proof_check() -> None:
    legacy_levels: list[dict[str, Any]] = []
    for rel in SCHEMA_EXAMPLES["level-v1"][1]:
        level = load_json(ROOT / rel)
        legacy_levels.append(level)
        for error in basic_path_errors(level) + v1_hash_errors(level): fail(f"{error}:{Path(rel).name}")
    pairs = [
        ("ARCHITECTURE/examples/level-v2.example.json", "ARCHITECTURE/examples/proof-v1.example.json"),
        ("ARCHITECTURE/examples/level-v2.single-cell.example.json", "ARCHITECTURE/examples/proof-v1.single-cell.example.json"),
    ]
    current_levels: list[dict[str, Any]] = []
    for level_rel, proof_rel in pairs:
        current = load_json(ROOT / level_rel)
        current_levels.append(current)
        for error in v2_binding_errors(current, load_json(ROOT / proof_rel)):
            fail(f"{error}:{Path(level_rel).name}")
    for legacy, current in zip(legacy_levels, current_levels):
        if legacy["id"] != current["puzzleId"] or any(legacy[key] != current[key] for key in ("rulesetVersion", "contentRevision", "grid", "endpoints", "rowCounts", "columnCounts", "content", "production", "completion", "solution")):
            fail(f"level-migration:not-neutral:{legacy['id']}")
    golden = load_json(TOOL / "fixtures/level-progress-v1-to-v2.golden.json")
    source, binding = golden["source"], golden["releaseLockBinding"]
    lock = load_json(ROOT / "ARCHITECTURE/examples/release-lock-v1.example.json")
    entry = next((item for item in lock["puzzles"] if item["puzzleId"] == binding["puzzleId"]), None)
    if entry is None or binding["publicPuzzleHash"] != entry["publicPuzzleHash"] or {"profile": binding["legacyProfile"], "sha256": binding["legacySha256"]} not in entry["legacyBindings"]:
        fail("level-progress-migration:release-lock")
    expected = {key: value for key, value in source.items() if key not in {"levelId", "puzzleHashSha256"}}
    expected["saveSchemaVersion"] = 2
    expected["puzzleId"] = binding["puzzleId"]
    expected["publicPuzzleHashAtFirstCompletion"] = binding["publicPuzzleHash"]
    if expected != golden["expected"]: fail("level-progress-migration:golden")


def campaign_subjects(campaign: dict[str, Any]) -> tuple[set[str], list[str], list[str]]:
    subjects: set[str] = set(); puzzle_ids: list[str] = []; errors: list[str] = []
    unlocks: dict[str, dict[str, Any]] = {}; deps: dict[str, set[str]] = {}
    def add(identifier: str, unlock: dict[str, Any], parent: str | None = None) -> None:
        if identifier in subjects: errors.append(f"catalog:duplicate-subject:{identifier}")
        subjects.add(identifier); unlocks[identifier] = unlock; deps[identifier] = ({parent} if parent else set()) | set(unlock["requiredSubjectIds"])
    def order(items: list[dict[str, Any]], scope: str) -> None:
        if [item["order"] for item in items] != list(range(1, len(items) + 1)): errors.append(f"catalog:order:{scope}")
    order(campaign["seasons"], "seasons")
    for season in campaign["seasons"]:
        season_match = re.fullmatch(r"S([1-9][0-9]*)", season["id"])
        if season_match is None: errors.append("catalog:season-id")
        season_number = int(season_match.group(1)) if season_match else None
        add(season["id"], season["unlock"]); order(season["sections"], season["id"])
        for section in season["sections"]:
            section_match = re.fullmatch(r"S([1-9][0-9]*)-([0-9]{2})", section["id"])
            add(section["id"], section["unlock"], season["id"]); order(section["routes"], section["id"])
            if not section["id"].startswith(season["id"] + "-"): errors.append("catalog:section-parent")
            if section_match is None or int(section_match.group(1)) != season_number: errors.append("catalog:section-id")
            if season_number == 1 and section_match and not 1 <= int(section_match.group(2)) <= 5: errors.append("catalog:s1-section-range")
            for route in section["routes"]:
                route_match = re.fullmatch(r"S([1-9][0-9]*)-([0-9]{2})-([0-9]{2})", route["id"])
                add(route["id"], route["unlock"], section["id"]); order(route["levels"], route["id"])
                if not route["id"].startswith(section["id"] + "-"): errors.append("catalog:route-parent")
                if route_match is None or section_match is None or route_match.groups()[:2] != section_match.groups(): errors.append("catalog:route-id")
                if season_number == 1 and route_match and not 1 <= int(route_match.group(3)) <= 4: errors.append("catalog:s1-route-range")
                for ref in route["levels"]:
                    puzzle_id = ref["puzzleId"]
                    puzzle_ids.append(puzzle_id); add(puzzle_id, ref["unlock"], route["id"])
                    if not puzzle_id.startswith(route["id"] + "-"): errors.append("catalog:puzzle-parent")
                    puzzle_match = re.fullmatch(r"S([1-9][0-9]*)-([0-9]{2})-([0-9]{2})-([0-9]{2})", puzzle_id)
                    if puzzle_match is None or route_match is None or puzzle_match.groups()[:3] != route_match.groups(): errors.append("catalog:puzzle-id")
                    if season_number == 1 and puzzle_match and not 1 <= int(puzzle_match.group(4)) <= 12: errors.append("catalog:s1-position-range")
    for identifier, unlock in unlocks.items():
        refs = set(unlock["requiredSubjectIds"])
        if unlock["kind"] == "ALWAYS" and refs: errors.append("catalog:always-refs")
        if unlock["kind"] == "ALL_FIRST_CLEARS" and (not refs or not refs.issubset(subjects)): errors.append("catalog:unlock-reference")
        if identifier in refs: errors.append("catalog:unlock-self")
    visiting: set[str] = set(); visited: set[str] = set()
    def cycle(identifier: str) -> bool:
        if identifier in visiting: return True
        if identifier in visited: return False
        visiting.add(identifier)
        result = any(cycle(dep) for dep in deps.get(identifier, set()) if dep in subjects)
        visiting.remove(identifier); visited.add(identifier)
        return result
    if any(cycle(identifier) for identifier in sorted(subjects)): errors.append("catalog:unlock-cycle")
    reachable: set[str] = set(); changed = True
    while changed:
        changed = False
        for identifier in sorted(subjects - reachable):
            unlock = unlocks[identifier]
            if (unlock["kind"] == "ALWAYS" and deps[identifier].issubset(reachable)) or (unlock["kind"] == "ALL_FIRST_CLEARS" and deps[identifier] and deps[identifier].issubset(reachable)):
                reachable.add(identifier); changed = True
    if reachable != subjects: errors.append("catalog:unlock-unreachable")
    if len(puzzle_ids) != len(set(puzzle_ids)): errors.append("catalog:puzzle-duplicate")
    return subjects, puzzle_ids, errors


def cosmetics_errors(cosmetics: dict[str, Any], subjects: set[str]) -> list[str]:
    errors: list[str] = []
    ids = [item["id"] for item in cosmetics["items"]]
    if len(ids) != len(set(ids)): errors.append("cosmetics:duplicate-id")
    for item in cosmetics["items"]:
        mode = item["acquisition"]
        if mode == "PATIENCE_PURCHASE" and ("pricePatience" not in item or "milestoneEligibility" in item): errors.append("cosmetics:purchase-shape")
        if mode == "MILESTONE_GRANT":
            eligibility = item.get("milestoneEligibility", {})
            refs = eligibility.get("requiredCampaignSubjectIds", [])
            if "pricePatience" in item or eligibility.get("contractVersion") != 1 or eligibility.get("kind") != "ALL_FIRST_CLEARS_OF_CAMPAIGN_SUBJECTS": errors.append("cosmetics:milestone-shape")
            if not refs or not set(refs).issubset(subjects): errors.append("cosmetics:milestone-reference")
        if mode == "DEFAULT" and ("pricePatience" in item or "milestoneEligibility" in item): errors.append("cosmetics:default-shape")
    return errors


def catalog_check() -> None:
    campaign = load_json(ROOT / "ARCHITECTURE/examples/campaign-v2.example.json")
    completion = load_json(ROOT / "ARCHITECTURE/examples/completion-v1.example.json")
    cosmetics = load_json(ROOT / "ARCHITECTURE/examples/cosmetics-v2.example.json")
    levels = [load_json(ROOT / "ARCHITECTURE/examples/level-v2.example.json"), load_json(ROOT / "ARCHITECTURE/examples/level-v2.single-cell.example.json")]
    subjects, puzzle_ids, errors = campaign_subjects(campaign)
    expected_fixture_ids = {level["puzzleId"] for level in levels if level["puzzleId"].startswith("S1-")}
    if not set(puzzle_ids).issubset(expected_fixture_ids): errors.append("catalog:puzzle-reference")
    moments = {(item["mapSegmentId"], item["trainMomentId"], item["resultTextKey"]) for item in completion["completionMoments"]}
    for level in levels:
        value = level["completion"]
        if (value["mapSegmentId"], value["trainMomentId"], value["resultTextKey"]) not in moments: errors.append(f"catalog:completion-reference:{level['puzzleId']}")
    rewards = {item["reasonCode"]: item for item in completion["rewardDefinitions"]}
    if rewards.get("LEVEL_FIRST_CLEAR", {}).get("amountPatience") != 15 or rewards.get("POST_CLEAR_PATIENCE", {}).get("amountPatience") != 10:
        errors.append("catalog:confirmed-reward-values")
    errors.extend(cosmetics_errors(cosmetics, subjects))
    for error in errors: fail(error)


def parse_assembly_graph(text: str) -> tuple[set[str], dict[str, set[str]]]:
    section = text.split("## 3. Einzige normative Produktionsassembly-Allowlist", 1)[1].split("## 4.", 1)[0]
    nodes: set[str] = set(); rows: list[tuple[str, str]] = []
    for line in section.splitlines():
        if not line.startswith("| `STP."): continue
        columns = [column.strip() for column in line.strip().strip("|").split("|")]
        assembly = columns[0].strip("`"); nodes.add(assembly); rows.append((assembly, columns[2]))
    edges = {node: set() for node in nodes}
    for assembly, refs in rows:
        for target in re.findall(r"`(STP\.[A-Za-z.]+)`", refs):
            if target not in nodes: fail(f"assembly:unknown-target:{assembly}:{target}")
            else: edges[assembly].add(target)
    return nodes, edges


def graph_has_cycle(nodes: set[str], edges: dict[str, set[str]]) -> bool:
    visiting: set[str] = set(); visited: set[str] = set()
    def visit(node: str) -> bool:
        if node in visiting: return True
        if node in visited: return False
        visiting.add(node)
        if any(visit(target) for target in edges.get(node, set())): return True
        visiting.remove(node); visited.add(node); return False
    return any(visit(node) for node in sorted(nodes))


def mermaid_edges(text: str) -> set[tuple[str, str]]:
    section = text.split("```mermaid", 1)[1].split("```", 1)[0]
    aliases: dict[str, str] = {}
    for line in section.splitlines():
        for alias, label in re.findall(r"\b([A-Za-z][A-Za-z0-9_]*)\[([^\]]+)\]", line): aliases[alias] = label
    result: set[tuple[str, str]] = set()
    for line in section.splitlines():
        match = re.search(r"^\s*([A-Za-z][A-Za-z0-9_]*)(?:\[[^\]]+\])?\s*-->\s*([A-Za-z][A-Za-z0-9_]*)", line)
        if match and match.group(1) in aliases and match.group(2) in aliases:
            result.add((aliases[match.group(1)], aliases[match.group(2)]))
    return result


def assembly_errors(text: str) -> list[str]:
    errors: list[str] = []
    nodes, edges = parse_assembly_graph(text)
    if graph_has_cycle(nodes, edges): errors.append("assembly:cycle")
    if edges.get("STP.Bootstrap") != EXPECTED_BOOTSTRAP_REFS: errors.append("assembly:bootstrap-exact")
    if edges.get("STP.MobileServices.Google") != {"STP.Application"} or edges.get("STP.MobileServices.Store") != {"STP.Application"}: errors.append("assembly:adapter-direction")
    table_edges = {(source, target) for source, targets in edges.items() for target in targets}
    if mermaid_edges(text) != table_edges: errors.append("assembly:mermaid-drift")
    if "STP.MobileServices.Contracts" in nodes: errors.append("assembly:obsolete-contracts")
    return errors


def assembly_check() -> None:
    for error in assembly_errors((ROOT / "ARCHITECTURE/MODULE_BOUNDARIES.md").read_text(encoding="utf-8")): fail(error)


def endless_identity(descriptor: dict[str, Any]) -> str:
    projection = {key: descriptor[key] for key in ("endlessContractVersion", "rulesetVersion", "generatorVersion", "seed", "generationOrdinal", "parameterHashSha256")}
    return "E1-" + digest(projection)


def endless_fixture_errors(fixture: dict[str, Any]) -> list[str]:
    errors: list[str] = []
    if fixture.get("saveSchemaVersion") != 2: errors.append("endless:save-version")
    try: watermark = int(fixture["highestReservedOrdinal"])
    except Exception: return errors + ["endless:watermark-format"]
    active: set[int] = set()
    for draft in fixture.get("activeDrafts", []):
        ordinal = int(draft["generationOrdinal"])
        if ordinal <= 0 or ordinal > watermark or ordinal in active: errors.append("endless:active-ordinal")
        active.add(ordinal)
        if draft["descriptor"]["generationOrdinal"] != draft["generationOrdinal"] or draft["id"] != endless_identity(draft["descriptor"]): errors.append("endless:identity")
    if len(active) > 20: errors.append("endless:active-capacity")
    if any(key in fixture for key in ("terminalIntervals", "terminalDetails", "terminalCheckpoint")): errors.append("endless:terminal-state-present")
    return errors


def endless_check() -> None:
    fixture = load_json(TOOL / "fixtures/endless-save-v2.example.json")
    for error in endless_fixture_errors(fixture): fail(error)
    watermark = 0; active: dict[int, str] = {}; terminal_duplicates = 0
    for ordinal in range(1, 10001):
        if len(active) >= 20: fail("endless:simulation-capacity"); break
        watermark += 1; active[watermark] = "ACTIVE"
        del active[ordinal]
        if ordinal <= watermark and ordinal not in active: terminal_duplicates += 1
    if watermark != 10000 or active or terminal_duplicates != 10000: fail("endless:long-run")
    golden = load_json(TOOL / "fixtures/endless-save-v1-to-v2.golden.json")
    source, expected = golden["source"], golden["expected"]
    highest = int(source["nextGenerationOrdinal"]) - 1
    active_ordinals = {int(item["generationOrdinal"]) for item in source["activeDrafts"]}
    terminal: set[int] = set()
    for interval in source["terminalIntervals"]:
        terminal.update(range(int(interval["firstOrdinal"]), int(interval["lastOrdinal"]) + 1))
    if active_ordinals & terminal or active_ordinals | terminal != set(range(1, highest + 1)): fail("endless:migration-prefix")
    if expected != {"saveSchemaVersion": 2, "highestReservedOrdinal": str(highest), "activeOrdinals": [str(value) for value in sorted(active_ordinals)], "economyBalancePatience": source["economyBalancePatience"]}:
        fail("endless:migration-golden")


def execute_iap_trace(trace: dict[str, Any]) -> list[str]:
    errors: list[str] = []; state = "STARTED"; tx: str | None = None; entitlement = False; finalize_requested = False
    for index, event in enumerate(trace["events"]):
        kind = event["kind"]
        if kind == "RECEIVE_EVIDENCE" and state == "STARTED": tx = event["transactionKey"]; state = "EVIDENCE_RECEIVED"
        elif kind == "VERIFY" and state == "EVIDENCE_RECEIVED": state = "VERIFIED" if event.get("valid") else "REJECTED"
        elif kind == "PERSIST_ATOMIC_GRANT" and state == "VERIFIED" and event.get("entitlement") is True and event.get("state") == "GRANTED_NOT_FINALIZED": entitlement = True; state = "GRANTED_NOT_FINALIZED"
        elif kind == "REQUEST_FINALIZE":
            expected = "ACKNOWLEDGE" if trace["platform"] == "GOOGLE" else "FINISH"
            if state != "GRANTED_NOT_FINALIZED" or not entitlement or event.get("method") != expected or event.get("transactionKey") != tx: errors.append(f"iap:finalize-order:{index}")
            else: finalize_requested = True
        elif kind == "FINALIZE_SUCCEEDED" and finalize_requested and state == "GRANTED_NOT_FINALIZED" and event.get("transactionKey") == tx: state = "FINALIZED"
        else: errors.append(f"iap:transition:{index}:{kind}")
    if state != trace["expectedState"]: errors.append("iap:expected-state")
    return errors


def iap_check() -> None:
    fixture = load_json(TOOL / "fixtures/iap-state-machine-v1.json")
    for trace in fixture["traces"]:
        for error in execute_iap_trace(trace): fail(f"{error}:{trace['name']}")


def privacy_errors(value: dict[str, Any]) -> list[str]:
    errors: list[str] = []
    providers = value["productionProviders"]
    if providers != {"ads": "GoogleMobileAdsUnity-11.5.0", "analytics": "FirebaseAnalyticsUnity-13.16.0", "crash": "EXCLUDED", "iap": "UnityIAP-5.4.3-LAZY_READINESS"}: errors.append("privacy:provider-pins")
    scenarios = {item["name"]: item for item in value["scenarios"]}
    if set(scenarios) != {"FRESH_INSTALL", "OFFLINE_VALID_RESTART", "OFFLINE_INVALID_RESTART", "UPGRADE_PRIOR_ACTIVE", "REVOCATION", "REENABLE", "RESTART_AFTER_REVOKE"}: errors.append("privacy:scenario-set")
    required = {
        "FRESH_INSTALL": {"EFFECTIVE_ALL_FALSE", "ANALYTICS_OVERRIDE_FALSE", "UMP_UPDATE"},
        "OFFLINE_INVALID_RESTART": {"EFFECTIVE_ALL_FALSE", "ANALYTICS_OVERRIDE_FALSE"},
        "UPGRADE_PRIOR_ACTIVE": {"ANALYTICS_PERMANENT_DEACTIVATION_PRESENT", "ANALYTICS_OVERRIDE_FALSE", "EFFECTIVE_ALL_FALSE"},
        "REVOCATION": {"PERSIST_REVOKED", "EFFECTIVE_ALL_FALSE", "ANALYTICS_OVERRIDE_FALSE", "DISCARD_ADS"},
        "REENABLE": {"PERSIST_VALID_CURRENT", "SET_ANALYTICS_PRIVACY_SIGNALS", "ANALYTICS_OVERRIDE_TRUE", "ADS_INITIALIZE"},
        "RESTART_AFTER_REVOKE": {"EFFECTIVE_ALL_FALSE", "ANALYTICS_OVERRIDE_FALSE"},
    }
    for name, effects in required.items():
        if name not in scenarios or not effects.issubset(set(scenarios[name]["requiredEffects"])): errors.append(f"privacy:effects:{name}")
    if "CRASH_INITIALIZE" not in scenarios.get("REENABLE", {}).get("forbiddenEffects", []): errors.append("privacy:crash-exclusion")
    reen = scenarios.get("REENABLE", {}).get("requiredEffects", [])
    if reen and not (reen.index("PERSIST_VALID_CURRENT") < reen.index("ANALYTICS_OVERRIDE_TRUE") and reen.index("SET_ANALYTICS_PRIVACY_SIGNALS") < reen.index("ANALYTICS_OVERRIDE_TRUE")): errors.append("privacy:enable-order")
    return errors


def privacy_check() -> None:
    for error in privacy_errors(load_json(TOOL / "fixtures/privacy-lifecycle-v1.json")): fail(error)


def release_errors(rc: dict[str, Any], staging: dict[str, Any], receipt: dict[str, Any]) -> list[str]:
    errors: list[str] = []
    if rc["buildProfile"] != "production" or not rc["promotionAllowed"] or any(rc["debugFlags"].values()): errors.append("release:rc-not-production-identical")
    if any(token in rc["applicationIdentifier"].lower() for token in (".dev", ".qa", ".staging")): errors.append("release:rc-app-id")
    if staging["buildProfile"] != "staging" or staging["promotionAllowed"] is not False: errors.append("release:staging-promotable")
    identity_fields = ["releaseVersion", "gitCommit", "platform", "buildNumber", "applicationIdentifier", "signingFingerprintSha256", "toolchainLockSha256", "packageLockSha256", "contentLockSha256", "productionConfigSha256", "artifactSha256", "storeBuildReference"]
    if any(receipt.get(key) != rc.get(key) for key in identity_fields): errors.append("release:promotion-identity")
    return errors


def release_check() -> None:
    rc = load_json(ROOT / "ARCHITECTURE/examples/release-manifest-v1.rc.example.json")
    staging = load_json(ROOT / "ARCHITECTURE/examples/release-manifest-v1.staging.example.json")
    receipt = load_json(ROOT / "ARCHITECTURE/examples/promotion-receipt-v1.example.json")
    for error in release_errors(rc, staging, receipt): fail(error)
    lock = load_json(ROOT / "ARCHITECTURE/examples/release-lock-v1.example.json")
    level = load_json(ROOT / "ARCHITECTURE/examples/level-v2.example.json")
    proof = load_json(ROOT / "ARCHITECTURE/examples/proof-v1.example.json")
    entry = lock["puzzles"][0]
    if entry["puzzleId"] != level["puzzleId"] or entry["documentSha256"] != digest(level) or entry["proofHash"] != proof["proofHash"]:
        fail("release-lock:binding")


def review_contract_check() -> None:
    contract = load_json(TOOL / "fixtures/review-contracts-v0.3.json")
    checks = [
        contract.get("architectureVersion") == "0.3",
        set(contract["assembly"].get("bootstrapReferences", [])) == EXPECTED_BOOTSTRAP_REFS,
        contract["save"].get("schemaVersion") == 2,
        contract["reward"].get("claimTemplate") == "reward-claim:POST_CLEAR_PATIENCE:<puzzleId>",
        contract["iap"].get("grantBeforeStoreFinalize") is True,
        contract["privacy"].get("crashlyticsProductionMode") == "EXCLUDED",
        contract["privacy"].get("analyticsInvalidationMode") == "RESET_ONLY_BUILD",
        contract["endless"].get("terminalRetentionLimit") is None,
        contract["puzzleIdentity"].get("levelDocumentVersion") == 2,
        contract["release"].get("promotionRebuildAllowed") is False,
        contract["cosmetics"].get("milestoneLedgerDelta") == 0,
    ]
    if not all(checks): fail("review-contract:v0.3")
    tokens = {
        "FINAL-001": ("ARCHITECTURE/MODULE_BOUNDARIES.md", "BootstrapCompositionSmoke"),
        "FINAL-002": ("ARCHITECTURE/PERSISTENCE.md", "highestReservedOrdinal"),
        "FINAL-003": ("ARCHITECTURE/MOBILE_SERVICES.md", "Reset-only-Build"),
        "FINAL-004": ("ARCHITECTURE/TEST_STRATEGY.md", "REQUIRED_LATER/NOT_EXECUTED"),
        "FINAL-005": ("ARCHITECTURE/LEVEL_DATA_FORMAT.md", "STP-PUZZLE-SEMANTIC-JCS-1"),
        "FINAL-006": ("ARCHITECTURE/TEST_STRATEGY.md", "--scope-manifest"),
        "FINAL-007": ("ARCHITECTURE/BUILD_AND_RELEASE.md", "buildProfile: production"),
        "FINAL-008": ("ARCHITECTURE/CONTENT_CATALOGS.md", "cosmetic-milestone-claim:v1"),
    }
    for finding, (rel, token) in tokens.items():
        if token.lower() not in (ROOT / rel).read_text(encoding="utf-8").lower(): fail(f"review-doc:{finding}:{rel}")


def cross_tool_hash_check() -> None:
    fixture = TOOL / "fixtures/save-payload-v1.golden.json"
    value = load_json(fixture); expected = load_json(TOOL / "fixtures/save-payload-v1.expected.json")
    python_bytes = jcs_bytes(value)
    for invalid_name in ("duplicate-key.invalid.json", "float-token.invalid.json"):
        try:
            jcs_bytes(load_json(TOOL / f"fixtures/{invalid_name}"))
            fail(f"jcs:python-invalid-not-rejected:{invalid_name}")
        except ValueError: pass
    completed = subprocess.run(["node", str(TOOL / "jcs_crosscheck.mjs"), str(fixture)], cwd=ROOT, text=True, capture_output=True, check=False)
    if completed.returncode != 0: fail(f"jcs:node:{completed.stderr.strip()}"); return
    node_result = json.loads(completed.stdout)
    if base64.b64decode(node_result["canonicalBase64"]) != python_bytes: fail("jcs:cross-tool-bytes")
    if expected["canonicalByteLength"] != len(python_bytes) or expected["payloadSha256"] != hashlib.sha256(python_bytes).hexdigest(): fail("jcs:golden")
    for invalid_name, expected_text in (("duplicate-key.invalid.json", "duplicate key"), ("float-token.invalid.json", "floating-point")):
        result = subprocess.run(["node", str(TOOL / "jcs_crosscheck.mjs"), str(TOOL / f"fixtures/{invalid_name}")], cwd=ROOT, text=True, capture_output=True, check=False)
        if result.returncode == 0 or expected_text not in result.stderr: fail(f"jcs:node-invalid:{invalid_name}")


def scope_manifest_errors(manifest: dict[str, Any], changed: list[str], requested_scope: str) -> list[str]:
    errors: list[str] = []
    if manifest.get("scope") != requested_scope: errors.append("scope:mode-mismatch")
    patterns = manifest.get("allowedPathPatterns", [])
    for pattern in patterns:
        parts = Path(pattern).parts
        if pattern.startswith("/") or ".." in parts or pattern == "**": errors.append(f"scope:unsafe-pattern:{pattern}")
    for rel in changed:
        if rel.startswith("/") or ".." in Path(rel).parts: errors.append(f"scope:unsafe-path:{rel}"); continue
        if not any(fnmatch.fnmatchcase(rel, pattern) for pattern in patterns): errors.append(f"scope:out-of-scope:{rel}")
        if rel.startswith("Stammstrecken_Puzzle_Konzept_00-15/"): errors.append(f"scope:product-source:{rel}")
        if requested_scope == "documentation":
            forbidden_roots = ("Assets/", "Packages/", "ProjectSettings/", "Content/")
            forbidden_suffixes = {".cs", ".asmdef", ".unity", ".prefab", ".uxml", ".uss", ".shader"}
            if rel.startswith(forbidden_roots) or Path(rel).suffix.lower() in forbidden_suffixes: errors.append(f"scope:production-artifact:{rel}")
    return errors


def normalized_blocker_text(text: str) -> str:
    return re.sub(r"2026-09-(?:08|12)|Architecture v0\.[23]|Architecture-v0\.[23]", "<VERSION-METADATA>", text)


def git_scope_check(scope: str, manifest_path: Path) -> None:
    manifest = load_json(manifest_path)
    schema = load_json(TOOL / "scope-manifest-v1.schema.json")
    jsonschema.Draft202012Validator(schema).validate(manifest)
    base = manifest["baseCommit"]
    exists = subprocess.run(["git", "-C", str(ROOT), "cat-file", "-e", f"{base}^{{commit}}"], capture_output=True, check=False)
    if exists.returncode != 0: fail("scope:base-unavailable"); return
    ancestor = subprocess.run(["git", "-C", str(ROOT), "merge-base", "--is-ancestor", base, "HEAD"], capture_output=True, check=False)
    if ancestor.returncode != 0: fail("scope:base-not-ancestor")
    if re.fullmatch(r"WP-[0-9]{3}", manifest.get("workPackageId", "")) is None or not list((ROOT / "WORK_PACKAGES").glob(manifest["workPackageId"] + "_*.md")):
        fail("scope:work-package")
    name_status = subprocess.run(["git", "-C", str(ROOT), "diff", "--name-status", base, "--"], text=True, capture_output=True, check=True).stdout.splitlines()
    tracked: list[str] = []
    for line in name_status:
        columns = line.split("\t")
        if columns[0].startswith(("R", "C")) and len(columns) == 3:
            tracked.extend(columns[1:])
        elif len(columns) >= 2:
            tracked.append(columns[-1])
    untracked = subprocess.run(["git", "-C", str(ROOT), "ls-files", "--others", "--exclude-standard"], text=True, capture_output=True, check=True).stdout.splitlines()
    changed = sorted(set(tracked + untracked))
    for error in scope_manifest_errors(manifest, changed, scope): fail(error)
    blocker_rel = "ARCHITECTURE/OPEN_BLOCKERS.md"
    if blocker_rel in changed:
        base_text = subprocess.run(["git", "-C", str(ROOT), "show", f"{base}:{blocker_rel}"], text=True, capture_output=True, check=True).stdout
        current_text = (ROOT / blocker_rel).read_text(encoding="utf-8")
        if normalized_blocker_text(base_text) != normalized_blocker_text(current_text): fail("scope:blocker-content-changed")
    diff_check = subprocess.run(["git", "-C", str(ROOT), "diff", "--check", base, "--"], text=True, capture_output=True, check=False)
    if diff_check.returncode != 0: fail("scope:diff-check")
    secret_pattern = re.compile(r"-----BEGIN (?:RSA |OPENSSH )?PRIVATE KEY-----|AKIA[0-9A-Z]{16}|ghp_[A-Za-z0-9]{20,}|sk-[A-Za-z0-9]{20,}")
    for rel in changed:
        path = ROOT / rel
        if path.is_file():
            try: text = path.read_text(encoding="utf-8")
            except UnicodeDecodeError: continue
            if secret_pattern.search(text): fail(f"scope:secret-pattern:{rel}")
    main_remote_result = subprocess.run(["git", "-C", str(ROOT), "rev-parse", "--verify", "refs/remotes/origin/main"], text=True, capture_output=True, check=False)
    if main_remote_result.returncode != 0:
        fail("git:origin-main-unavailable")
        return
    main_local_result = subprocess.run(["git", "-C", str(ROOT), "rev-parse", "--verify", "refs/heads/main"], text=True, capture_output=True, check=False)
    if main_local_result.returncode == 0 and main_local_result.stdout.strip() != main_remote_result.stdout.strip():
        fail("git:main-changed")


def status_consistency_errors(architecture: str, current: str, queue: str, work_package: str) -> list[str]:
    errors: list[str] = []
    if not architecture.startswith("# Stammstrecken-Puzzle – Architecture v0.3") or "**Status:** Angenommen" not in architecture: errors.append("version:architecture")
    if "**Architecture v0.3**" not in current or "Architecture v0.3 (Abnahmekandidat)" in current or "`WP-001`, `WP-002` und `WP-003` sind abgeschlossen" not in current: errors.append("version:current-state")
    if "Architecture v0.3" not in queue or "WP-001`, `WP-002`, `WP-003" not in queue or "Unabhängiger Architecture-v1.0-Freigabereview | **Nicht begonnen**" not in queue: errors.append("version:work-queue")
    if "**Bearbeitungsstatus:** Abgeschlossen" not in work_package: errors.append("version:work-package")
    return errors


def version_and_blocker_check() -> None:
    for error in status_consistency_errors(
        (ROOT / "ARCHITECTURE/ARCHITECTURE.md").read_text(encoding="utf-8"),
        (ROOT / "PROJECT_CONTROL/CURRENT_STATE.md").read_text(encoding="utf-8"),
        (ROOT / "PROJECT_CONTROL/WORK_QUEUE.md").read_text(encoding="utf-8"),
        (ROOT / "WORK_PACKAGES/WP-003_Architecture-v0.3-Finalkorrekturen.md").read_text(encoding="utf-8"),
    ): fail(error)
    blockers = (ROOT / "ARCHITECTURE/OPEN_BLOCKERS.md").read_text(encoding="utf-8")
    for number in (1, 2, 3):
        if f"BLOCKER-PROD-00{number}" not in blockers: fail(f"blocker:missing:{number}")
    if blockers.count("| Status | **Offen** |") != 3 or blockers.count("Fail-closed") < 3: fail("blocker:status")


def self_test(scope: str, manifest_path: Path) -> None:
    failures: list[str] = []
    def expect(label: str, condition: bool) -> None:
        if not condition: failures.append(label)

    # FINAL-001: deleting the direct Bootstrap -> Application edge must fail exact graph parity.
    module_text = (ROOT / "ARCHITECTURE/MODULE_BOUNDARIES.md").read_text(encoding="utf-8")
    mutated = module_text.replace("`STP.Application`, `STP.Infrastructure.Content`", "`STP.Infrastructure.Content`", 1).replace("    Bootstrap[STP.Bootstrap] --> App\n", "")
    expect("FINAL-001", "assembly:bootstrap-exact" in assembly_errors(mutated))

    # FINAL-002: terminal collections in Save v2 and duplicate active ordinals must fail.
    endless = load_json(TOOL / "fixtures/endless-save-v2.example.json")
    endless["terminalIntervals"] = []
    expect("FINAL-002", "endless:terminal-state-present" in endless_fixture_errors(endless))

    # FINAL-003: removing the reset-only upgrade effect must fail.
    privacy = load_json(TOOL / "fixtures/privacy-lifecycle-v1.json")
    upgrade = next(item for item in privacy["scenarios"] if item["name"] == "UPGRADE_PRIOR_ACTIVE")
    upgrade["requiredEffects"].remove("ANALYTICS_PERMANENT_DEACTIVATION_PRESENT")
    expect("FINAL-003", "privacy:effects:UPGRADE_PRIOR_ACTIVE" in privacy_errors(privacy))

    # FINAL-004/006: documentation scope must reject a production source path and unsafe wildcard.
    manifest = load_json(manifest_path)
    expect("FINAL-004", any(item.startswith("scope:out-of-scope") or item.startswith("scope:production-artifact") for item in scope_manifest_errors(manifest, ["Assets/StammstreckenPuzzle/Scripts/Foo.cs"], "documentation")))
    unsafe = copy.deepcopy(manifest); unsafe["allowedPathPatterns"] = ["**"]
    expect("FINAL-006", "scope:unsafe-pattern:**" in scope_manifest_errors(unsafe, [], "documentation"))
    production = copy.deepcopy(manifest); production["scope"] = "production"; production["allowedPathPatterns"] = ["Assets/**/*.cs"]
    expect("FINAL-006-PRODUCTION", not scope_manifest_errors(production, ["Assets/StammstreckenPuzzle/Scripts/Foo.cs"], "production"))
    product_scope = copy.deepcopy(production); product_scope["allowedPathPatterns"] = ["Stammstrecken_Puzzle_Konzept_00-15/**"]
    expect("FINAL-006-PRODUCT-SOURCE", any(item.startswith("scope:product-source:") for item in scope_manifest_errors(product_scope, ["Stammstrecken_Puzzle_Konzept_00-15/09_Train_Track_Master_Spezifikation.md"], "production")))
    blocker_text = (ROOT / "ARCHITECTURE/OPEN_BLOCKERS.md").read_text(encoding="utf-8")
    expect("FINAL-006-BLOCKERS", normalized_blocker_text(blocker_text) != normalized_blocker_text(blocker_text.replace("**Offen**", "**Geschlossen**", 1)))
    status_args = [
        (ROOT / "ARCHITECTURE/ARCHITECTURE.md").read_text(encoding="utf-8"),
        (ROOT / "PROJECT_CONTROL/CURRENT_STATE.md").read_text(encoding="utf-8"),
        (ROOT / "PROJECT_CONTROL/WORK_QUEUE.md").read_text(encoding="utf-8"),
        (ROOT / "WORK_PACKAGES/WP-003_Architecture-v0.3-Finalkorrekturen.md").read_text(encoding="utf-8"),
    ]
    status_args[2] = status_args[2].replace("Architecture v0.3", "Architecture v0.2")
    expect("FINAL-004-STATUS-CONSISTENCY", "version:work-queue" in status_consistency_errors(*status_args))

    # FINAL-005: copied proof bound to another puzzle must fail.
    level = load_json(ROOT / "ARCHITECTURE/examples/level-v2.example.json")
    proof = load_json(ROOT / "ARCHITECTURE/examples/proof-v1.example.json"); proof["puzzleId"] = "S1-01-01-02"
    expect("FINAL-005", "proof:puzzle-id" in v2_binding_errors(level, proof))

    # FINAL-007: a changed promotion artifact hash must fail identity equality.
    rc = load_json(ROOT / "ARCHITECTURE/examples/release-manifest-v1.rc.example.json")
    staging = load_json(ROOT / "ARCHITECTURE/examples/release-manifest-v1.staging.example.json")
    receipt = load_json(ROOT / "ARCHITECTURE/examples/promotion-receipt-v1.example.json"); receipt["artifactSha256"] = "0" * 64
    expect("FINAL-007", "release:promotion-identity" in release_errors(rc, staging, receipt))

    # FINAL-008: unknown milestone subject must fail catalog semantics.
    campaign = load_json(ROOT / "ARCHITECTURE/examples/campaign-v2.example.json"); subjects, _, _ = campaign_subjects(campaign)
    cosmetics = load_json(ROOT / "ARCHITECTURE/examples/cosmetics-v2.example.json")
    milestone = next(item for item in cosmetics["items"] if item["acquisition"] == "MILESTONE_GRANT")
    milestone["milestoneEligibility"]["requiredCampaignSubjectIds"] = ["S9-99-99"]
    expect("FINAL-008", "cosmetics:milestone-reference" in cosmetics_errors(cosmetics, subjects))

    # Retained regression mutations from Architecture v0.2.
    expect("REG-WP", bool(check_wp_identifiers(["WP-001", "WP-02"])))
    index_text = (ROOT / "DECISIONS/README.md").read_text(encoding="utf-8")
    expect("REG-ADR-MISSING", bool(adr_index_errors(index_text.replace("ADR-023", "ADR-X23"))))
    expect("FINAL-004-ADR-STATUS", "adr-index:status:023" in adr_index_errors(index_text.replace("| [ADR-023](./ADR-023-releasekandidat-und-kosmetikclaims.md) | Releasekandidat-Identität und kosmetische Meilensteinclaims | **Angenommen** |", "| [ADR-023](./ADR-023-releasekandidat-und-kosmetikclaims.md) | Releasekandidat-Identität und kosmetische Meilensteinclaims | **Ersetzt** |")))
    expect("FINAL-004-ADR-LINK", "adr-index:link:023" in adr_index_errors(index_text.replace("./ADR-023-releasekandidat-und-kosmetikclaims.md", "./ADR-022-validator-scope-und-belegkategorien.md", 1)))
    inconsistent_level = load_json(ROOT / "ARCHITECTURE/examples/level-v2.example.json")
    inconsistent_level["content"]["season"] = 2
    expect("FINAL-004-LEVEL-ID", "level-v2:id-content" in v2_binding_errors(inconsistent_level, load_json(ROOT / "ARCHITECTURE/examples/proof-v1.example.json")))
    for field, value, code in (("networkSection", 6, "level-v2:s1-section-range"), ("route", 5, "level-v2:s1-route-range"), ("position", 13, "level-v2:s1-position-range")):
        out_of_range = load_json(ROOT / "ARCHITECTURE/examples/level-v2.example.json")
        out_of_range["content"][field] = value
        expect(f"FINAL-004-S1-{field.upper()}", code in v2_binding_errors(out_of_range, load_json(ROOT / "ARCHITECTURE/examples/proof-v1.example.json")))
    campaign_section = load_json(ROOT / "ARCHITECTURE/examples/campaign-v2.example.json")
    section = campaign_section["seasons"][0]["sections"][0]; section["id"] = "S1-06"; section["routes"][0]["id"] = "S1-06-01"; section["routes"][0]["levels"][0]["puzzleId"] = "S1-06-01-01"
    expect("FINAL-004-CAMPAIGN-SECTION", "catalog:s1-section-range" in campaign_subjects(campaign_section)[2])
    campaign_route = load_json(ROOT / "ARCHITECTURE/examples/campaign-v2.example.json")
    route = campaign_route["seasons"][0]["sections"][0]["routes"][0]; route["id"] = "S1-01-05"; route["levels"][0]["puzzleId"] = "S1-01-05-01"
    expect("FINAL-004-CAMPAIGN-ROUTE", "catalog:s1-route-range" in campaign_subjects(campaign_route)[2])
    campaign_position = load_json(ROOT / "ARCHITECTURE/examples/campaign-v2.example.json")
    campaign_position["seasons"][0]["sections"][0]["routes"][0]["levels"][0]["puzzleId"] = "S1-01-01-13"
    expect("FINAL-004-CAMPAIGN-POSITION", "catalog:s1-position-range" in campaign_subjects(campaign_position)[2])
    bad_thresholds = load_json(ROOT / "ARCHITECTURE/examples/level-v2.example.json")
    bad_thresholds["production"]["starThresholdsSeconds"] = {"twoStars": 100, "threeStars": 120}
    expect("FINAL-004-STAR-THRESHOLDS", "level-v2:time-order" in v2_binding_errors(bad_thresholds, load_json(ROOT / "ARCHITECTURE/examples/proof-v1.example.json")))
    bad_trace = load_json(TOOL / "fixtures/iap-state-machine-v1.json")["traces"][0]
    events = bad_trace["events"]; bad_trace["events"] = [events[0], events[1], events[3], events[2], events[4]]
    expect("REG-IAP", any(item.startswith("iap:finalize-order") for item in execute_iap_trace(bad_trace)))
    bad_schema = load_json(ROOT / "ARCHITECTURE/schemas/level-v2.schema.json")
    bad_schema["properties"]["solution"]["properties"]["path"]["minItems"] = 2
    single = load_json(ROOT / "ARCHITECTURE/examples/level-v2.single-cell.example.json")
    expect("REG-SINGLE-CELL", bool(list(jsonschema.Draft202012Validator(bad_schema).iter_errors(single))))

    if failures:
        for label in failures: fail(f"self-test:not-detected:{label}")
    else:
        PASSES.append(("LOCAL_ARCHITECTURE_SEMANTICS", "Mutations-Selbsttest: FINAL-001 bis FINAL-008 und Regressionen werden erkannt"))


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate Stammstrecken-Puzzle Architecture v0.3")
    parser.add_argument("--scope", choices=("documentation", "production"))
    parser.add_argument("--scope-manifest", type=Path)
    parser.add_argument("--self-test", action="store_true")
    args = parser.parse_args()
    if bool(args.scope) != bool(args.scope_manifest):
        parser.error("--scope and --scope-manifest must be provided together")
    manifest_path = None if args.scope_manifest is None else (args.scope_manifest if args.scope_manifest.is_absolute() else ROOT / args.scope_manifest)

    groups: list[tuple[str, str, Callable[[], None]]] = [
        ("LOCAL_DOCUMENT_STRUCTURE", "Datei-/ADR-Inventar", inventory_check),
        ("LOCAL_DOCUMENT_STRUCTURE", "Work-Package-IDs und Struktur", work_package_check),
        ("LOCAL_DOCUMENT_STRUCTURE", "ADR-Index, Status und Superseding", adr_check),
        ("LOCAL_DOCUMENT_STRUCTURE", "Relative Markdownlinks", markdown_link_check),
        ("LOCAL_ARCHITECTURE_SEMANTICS", "JSON Schema Draft 2020-12", schema_check),
        ("LOCAL_ARCHITECTURE_SEMANTICS", "Level-v1-Legacy und Level-v2-/Proofbindung", level_and_proof_check),
        ("LOCAL_ARCHITECTURE_SEMANTICS", "Campaign-/Completion-/Cosmetics-v2-Semantik", catalog_check),
        ("LOCAL_ARCHITECTURE_SEMANTICS", "Zyklusfreier exakter Assemblygraph", assembly_check),
        ("LOCAL_ARCHITECTURE_SEMANTICS", "Endless-Watermark und Save-v1→v2", endless_check),
        ("LOCAL_ARCHITECTURE_SEMANTICS", "Ausführbare IAP-Zustandsmaschine", iap_check),
        ("LOCAL_ARCHITECTURE_SEMANTICS", "Privacy-Lifecycle-Fixture", privacy_check),
        ("LOCAL_ARCHITECTURE_SEMANTICS", "RC-/Promotion- und Release-Lock-Identität", release_check),
        ("LOCAL_ARCHITECTURE_SEMANTICS", "Acht Architecture-v0.3-Reviewverträge", review_contract_check),
        ("LOCAL_ARCHITECTURE_SEMANTICS", "Save-JCS Python/Node-Crosscheck", cross_tool_hash_check),
        ("LOCAL_DOCUMENT_STRUCTURE", "Architecture-v0.3-Status und drei Produktblocker", version_and_blocker_check),
    ]
    if args.scope and manifest_path:
        groups.append(("LOCAL_SCOPE", "Git-Diff gegen versioniertes Scope-Manifest", lambda: git_scope_check(args.scope, manifest_path)))
    for category, name, function in groups: run_group(category, name, function)
    mutation_manifest = manifest_path or ROOT / "tools/architecture-validation/scopes/WP-003.documentation.scope.json"
    if args.self_test: self_test(args.scope or "documentation", mutation_manifest)

    print(f"ARCHITECTURE VALIDATION v0.3 scope={args.scope or 'architecture-only'}")
    for category, item in PASSES: print(f"{category} PASS  {item}")
    if args.scope and manifest_path:
        head = subprocess.run(["git", "-C", str(ROOT), "rev-parse", "HEAD"], text=True, capture_output=True, check=True).stdout.strip()
        dirty = bool(subprocess.run(["git", "-C", str(ROOT), "status", "--porcelain"], text=True, capture_output=True, check=True).stdout)
        manifest = load_json(manifest_path)
        print(f"SCOPE_CONTEXT  workPackage={manifest['workPackageId']} base={manifest['baseCommit']} head={head} worktreeDirty={str(dirty).lower()}")
    print("CONTRACT_ONLY  JSON-/Markdown-Verträge, Fixtures und Mutationsmodelle; kein Unity-Produktionscode")
    print("REQUIRED_LATER/NOT_EXECUTED  Unity Compile/EditMode/PlayMode, BootstrapCompositionSmoke, IL2CPP, physische Privacy-Gerätecaptures, SDK-Sandbox, Storeupload/-promotion")
    print("BLOCKED  BLOCKER-PROD-001, BLOCKER-PROD-002, BLOCKER-PROD-003 bleiben fail-closed")
    if ERRORS:
        for item in ERRORS: print(f"FAIL  {item}")
        print(f"RESULT FAIL ({len(ERRORS)} Fehler)")
        return 1
    print(f"RESULT PASS ({len(PASSES)} lokale Prüfgruppen)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
