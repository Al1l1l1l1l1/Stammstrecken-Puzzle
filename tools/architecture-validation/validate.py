#!/usr/bin/env python3
from __future__ import annotations

import argparse
import base64
import copy
from datetime import datetime, timezone
import fnmatch
import hashlib
import json
import posixpath
import re
import subprocess
import sys
from pathlib import Path, PurePosixPath
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
    21: "Angenommen", 22: "Ersetzt", 23: "Angenommen", 24: "Angenommen",
    25: "Angenommen", 26: "Angenommen",
    27: "Angenommen", 28: "Angenommen", 29: "Angenommen", 30: "Angenommen",
    31: "Angenommen",
}
SUPERSEDES = {3: 13, 4: 21, 6: 14, 8: 15, 9: 17, 14: 19, 15: 20, 17: 22, 22: 26}
FOLLOW_UPS = {(25, 27), (23, 28), (23, 29), (26, 30)}
ADR016_HISTORY_COMMIT = "6152eead04494386404241967da0d8a62e741718"
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
    "cosmetics-v2": ("ARCHITECTURE/schemas/cosmetics-v2.schema.json", ["ARCHITECTURE/examples/cosmetics-v2.example.json", "ARCHITECTURE/examples/cosmetics-v2.draft.example.json"]),
    "release-lock-v1": ("ARCHITECTURE/schemas/release-lock-v1.schema.json", ["ARCHITECTURE/examples/release-lock-v1.example.json"]),
    "release-manifest-v1": ("ARCHITECTURE/schemas/release-manifest-v1.schema.json", ["ARCHITECTURE/examples/release-manifest-v1.rc.example.json", "ARCHITECTURE/examples/release-manifest-v1.staging.example.json"]),
    "scope-manifest-v1": ("tools/architecture-validation/scope-manifest-v1.schema.json", ["tools/architecture-validation/scopes/WP-003.documentation.scope.json", "tools/architecture-validation/scopes/WP-004.documentation.scope.json", "tools/architecture-validation/scopes/WP-005.documentation.scope.json", "tools/architecture-validation/scopes/WP-006.documentation.scope.json", "tools/architecture-validation/scopes/WP-007.documentation.scope.json"]),
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
    ROOT / "WORK_PACKAGES/WP-004_Architecture-v0.4-Abschlusskorrekturen.md",
    ROOT / "WORK_PACKAGES/WP-005_Architecture-v0.5-letzte-High-Korrekturen.md",
    ROOT / "WORK_PACKAGES/WP-006_Architecture-v1.0-Promotion.md",
    ROOT / "WORK_PACKAGES/WP-007_CI-Setup.md",
    TOOL / "README.md", TOOL / "requirements.lock.txt", TOOL / "jcs_crosscheck.mjs",
    TOOL / "fixtures/duplicate-key.invalid.json", TOOL / "fixtures/float-token.invalid.json",
    TOOL / "fixtures/save-payload-v1.golden.json", TOOL / "fixtures/save-payload-v1.expected.json",
    TOOL / "fixtures/review-contracts-v0.3.json", TOOL / "fixtures/review-contracts-v0.4.json", TOOL / "fixtures/review-contracts-v0.5.json", TOOL / "fixtures/endless-save-v2.example.json",
    TOOL / "fixtures/endless-save-v1-to-v2.golden.json", TOOL / "fixtures/iap-state-machine-v1.json",
    TOOL / "fixtures/privacy-lifecycle-v1.json", TOOL / "fixtures/privacy-lifecycle-v2.json", TOOL / "fixtures/level-progress-v1-to-v2.golden.json",
    TOOL / "fixtures/cosmetics-lifecycle-v1.json", TOOL / "fixtures/rollout-metric-v1.json",
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
    if numbers != list(range(1, 32)):
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


def section_body(text: str, heading: str, next_heading: str) -> str:
    return text.split(heading, 1)[1].split(next_heading, 1)[0].strip()


def adr_index_errors(index_text: str) -> list[str]:
    errors: list[str] = []
    for number in range(1, 32):
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
    if "Architecture v1.0" not in index_text or "22 sind angenommen" not in index_text or "9 bleiben als ersetzte" not in index_text:
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
    for old, new in FOLLOW_UPS:
        if f"ADR-{new:03d}" not in texts[old] or f"ADR-{old:03d}" not in texts[new]:
            fail(f"adr:follow-up:{old}:{new}")
    if "vollständig ersetzt durch [adr-021]" not in texts[4].lower() or "ersetzt [adr-004]" not in texts[21].lower() or "vollständig" not in texts[21].lower(): fail("adr:004-021-full-replacement")
    superseding_section = section_body(texts[16], "## Ersetzt / ersetzt durch", "## Referenzen")
    if "adr-004 bleibt" in superseding_section.lower(): fail("adr:016-stale-004-validity")
    historical = subprocess.run(
        ["git", "-C", str(ROOT), "show", f"{ADR016_HISTORY_COMMIT}:DECISIONS/ADR-016-katalogvertraege-und-endless-identitaet.md"],
        text=True, capture_output=True, check=False,
    )
    if historical.returncode != 0:
        fail("adr:016-history-source-unavailable")
    elif section_body(texts[16], "## Entscheidung", "## Begründung") != section_body(historical.stdout, "## Entscheidung", "## Begründung"):
        fail("adr:016-historical-decision-mutated")
    if "ADR-024" not in texts[20] or "ADR-020" not in texts[24]: fail("adr:020-024-follow-up")
    if "ADR-025" not in texts[19] or "ADR-019" not in texts[25]: fail("adr:019-025-follow-up")


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


def level_progress_migration_errors(golden: dict[str, Any]) -> list[str]:
    errors: list[str] = []
    source, binding = golden["source"], golden["releaseLockBinding"]
    documents: dict[str, dict[str, Any]] = {}
    for key, expected_version_field, expected_version in (
        ("sourceDocument", "schemaVersion", 1),
        ("targetDocument", "documentSchemaVersion", 2),
        ("releaseLockDocument", "lockFormatVersion", 1),
    ):
        document = golden[key]; path = ROOT / document["path"]
        if not path.is_file(): errors.append(f"level-progress-migration:document-binding:{key}"); continue
        documents[key] = load_json(path)
        metadata_field = "lockFormatVersion" if key == "releaseLockDocument" else "documentSchemaVersion"
        if document.get(metadata_field) != expected_version: errors.append(f"level-progress-migration:document-version:{key}")
        if digest(documents[key]) != document["documentSha256"]: errors.append(f"level-progress-migration:document-binding:{key}")
        if documents[key].get(expected_version_field) != expected_version: errors.append(f"level-progress-migration:document-version:{key}")
    if set(documents) != {"sourceDocument", "targetDocument", "releaseLockDocument"}: return errors
    source_document, target_document, lock = documents["sourceDocument"], documents["targetDocument"], documents["releaseLockDocument"]
    if source_document["id"] != source["levelId"] or source_document["validation"]["puzzleHashSha256"] != source["puzzleHashSha256"]: errors.append("level-progress-migration:source-identity")
    if target_document["puzzleId"] != binding["puzzleId"]: errors.append("level-progress-migration:target-identity")
    entry = next((item for item in lock["puzzles"] if item["puzzleId"] == binding["puzzleId"]), None)
    if entry is None or binding["publicPuzzleHash"] != entry["publicPuzzleHash"] or entry["documentSha256"] != golden["targetDocument"]["documentSha256"] or {"profile": binding["legacyProfile"], "sha256": binding["legacySha256"]} not in entry["legacyBindings"]: errors.append("level-progress-migration:release-lock")
    expected = {key: value for key, value in source.items() if key not in {"levelId", "puzzleHashSha256"}}
    expected["saveSchemaVersion"] = 2; expected["puzzleId"] = binding["puzzleId"]; expected["publicPuzzleHashAtFirstCompletion"] = binding["publicPuzzleHash"]
    if expected != golden["expected"]: errors.append("level-progress-migration:golden")
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
    v1_schema = jsonschema.Draft202012Validator(load_json(ROOT / SCHEMA_EXAMPLES["level-v1"][0]))
    v2_schema = jsonschema.Draft202012Validator(load_json(ROOT / SCHEMA_EXAMPLES["level-v2"][0]))
    for focus in ("OCCUPANCY", "EXCLUSION", "ENDPOINT_GEOMETRY", "CHAIN", "DENSITY", "COMBINATION"):
        legacy_focus = copy.deepcopy(legacy_levels[0]); legacy_focus["production"]["focus"] = [focus]
        current_focus = copy.deepcopy(current_levels[0]); current_focus["production"]["focus"] = [focus]
        if list(v1_schema.iter_errors(legacy_focus)): fail(f"level-migration:v1-focus-source:{focus}")
        if list(v2_schema.iter_errors(current_focus)): fail(f"level-migration:v2-focus-compatibility:{focus}")
    for error in level_progress_migration_errors(load_json(TOOL / "fixtures/level-progress-v1-to-v2.golden.json")): fail(error)


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


def expand_campaign_subject(subject: str, puzzle_ids: list[str]) -> set[str]:
    return {puzzle_id for puzzle_id in puzzle_ids if puzzle_id == subject or puzzle_id.startswith(subject + "-")}


def cosmetic_status_transition_allowed(before: str, after: str) -> bool:
    return (before, after) in {("DRAFT", "ACTIVE"), ("ACTIVE", "HIDDEN"), ("HIDDEN", "ACTIVE"), ("ACTIVE", "TOMBSTONE"), ("HIDDEN", "TOMBSTONE")} or before == after


def cosmetic_milestone_binding(catalog: dict[str, Any], item: dict[str, Any], puzzle_ids: list[str], first_clears: set[str]) -> tuple[bool, dict[str, Any]]:
    eligibility = item.get("milestoneEligibility", {})
    subjects = sorted(eligibility.get("requiredCampaignSubjectIds", []))
    required: set[str] = set()
    for subject in subjects:
        required |= expand_campaign_subject(subject, puzzle_ids)
    projection = {
        "catalogId": catalog["catalogId"],
        "catalogRevision": catalog["catalogRevision"],
        "catalogHashSha256": digest(catalog),
        "itemId": item["id"],
        "acquisition": item.get("acquisition"),
        "eligibilityContractVersion": eligibility.get("contractVersion"),
        "kind": eligibility.get("kind"),
        "requiredCampaignSubjectIds": subjects,
        "requiredPuzzleIds": sorted(required),
        "validatedFirstClearPuzzleIds": sorted(required & first_clears),
    }
    return bool(required) and required.issubset(first_clears), {
        "catalogId": projection["catalogId"],
        "catalogRevision": projection["catalogRevision"],
        "catalogHashSha256": projection["catalogHashSha256"],
        "itemId": projection["itemId"],
        "acquisition": projection["acquisition"],
        "eligibilityContractVersion": projection["eligibilityContractVersion"],
        "eligibilityProjectionHashSha256": digest(projection),
    }


def execute_cosmetics_scenario(scenario: dict[str, Any], catalogs: dict[str, dict[str, Any]], puzzle_ids: list[str]) -> tuple[dict[str, Any], list[str]]:
    catalog_name = scenario.get("catalog", "cosmetics-v2.example.json")
    catalog = catalogs[catalog_name]
    item = copy.deepcopy(next(item for item in catalog["items"] if item["id"] == scenario["itemId"]))
    state = copy.deepcopy(scenario["initial"]); balance = state["balancePatience"]
    reservations = {entry["claimId"]: entry for entry in state.get("claimReservations", [])}
    ownership = {entry["itemId"]: entry for entry in state.get("ownershipRecords", [])}
    first_clears = set(state.get("firstClearPuzzleIds", [])); eligible = False; result = "NO_RESULT"; errors: list[str] = []
    runtime_catalog = catalog["approvalStatus"] in {"FIXTURE_ONLY", "PRODUCT_APPROVED"}
    for index, event in enumerate(scenario["events"]):
        kind = event["kind"]
        if kind == "FIRST_CLEAR": first_clears.add(event["puzzleId"])
        elif kind == "REMOVE_FIRST_CLEAR": first_clears.discard(event["puzzleId"])
        elif kind == "CATALOG_STATUS_CHANGED":
            if not cosmetic_status_transition_allowed(item["status"], event["status"]): errors.append(f"cosmetics:status-transition:{index}")
            else: item["status"] = event["status"]
        elif kind == "PURCHASE":
            if not runtime_catalog or item["status"] != "ACTIVE": result = "COSMETIC_NOT_RUNTIME_ELIGIBLE"
            elif item["id"] in ownership: result = "ALREADY_OWNED"
            elif item.get("acquisition") != "PATIENCE_PURCHASE" or balance < item.get("pricePatience", 2**53): result = "NOT_ELIGIBLE"
            else:
                balance -= item["pricePatience"]
                ownership[item["id"]] = {"itemId": item["id"], "grantKind": "PATIENCE_PURCHASE"}
                result = "COMMITTED"
        elif kind == "EVALUATE_MILESTONE":
            if item["id"] in ownership:
                result = "OWNERSHIP_PRESERVED_NO_NEW_GRANT" if item["status"] in {"HIDDEN", "TOMBSTONE"} else "ALREADY_OWNED"; continue
            if not runtime_catalog or item["status"] != "ACTIVE" or item.get("acquisition") != "MILESTONE_GRANT":
                result = "COSMETIC_NOT_RUNTIME_ELIGIBLE"; continue
            eligible, _ = cosmetic_milestone_binding(catalog, item, puzzle_ids, first_clears)
            result = "ELIGIBLE" if eligible else "NOT_ELIGIBLE"
        elif kind == "RESERVE_CLAIM":
            eligible_now, binding = cosmetic_milestone_binding(catalog, item, puzzle_ids, first_clears)
            claim_id = f"cosmetic-milestone-claim:v1:{catalog['catalogId']}:{item['id']}"
            expected = {**binding, "claimId": claim_id}
            supplied = {key: event.get(key) for key in expected}
            if not runtime_catalog or item["status"] != "ACTIVE" or item.get("acquisition") != "MILESTONE_GRANT" or not eligible_now or item["id"] in ownership:
                errors.append(f"cosmetics:reservation-eligibility:{index}")
            elif supplied != expected or not isinstance(event.get("operationId"), str) or not event["operationId"] or not isinstance(event.get("claimGeneration"), int) or event["claimGeneration"] < 1:
                errors.append(f"cosmetics:reservation-binding:{index}")
            elif claim_id in reservations:
                errors.append(f"cosmetics:reservation-duplicate:{index}")
            else:
                reservations[claim_id] = {**expected, "operationId": event["operationId"], "claimGeneration": event["claimGeneration"], "claimState": "RESERVED"}
                eligible = True; result = "RESERVED"
        elif kind in {"CRASH", "RESTART"}:
            continue
        elif kind == "COMMIT_CLAIM":
            claim_id = event.get("claimId")
            if item["id"] in ownership:
                existing = ownership[item["id"]]
                if existing.get("claimId") == claim_id: result = "ALREADY_OWNED"
                else: errors.append(f"cosmetics:claim-collision:{index}")
                continue
            reservation = reservations.get(claim_id)
            if reservation is None:
                errors.append(f"cosmetics:commit-without-reservation:{index}"); continue
            binding_keys = {"claimId", "operationId", "claimGeneration", "itemId", "catalogId", "catalogRevision", "catalogHashSha256", "acquisition", "eligibilityContractVersion", "eligibilityProjectionHashSha256"}
            if any(event.get(key) != reservation.get(key) for key in binding_keys) or reservation.get("acquisition") != "MILESTONE_GRANT" or reservation.get("claimState") != "RESERVED":
                errors.append(f"cosmetics:commit-binding:{index}"); continue
            eligible_now, binding = cosmetic_milestone_binding(catalog, item, puzzle_ids, first_clears)
            if not eligible_now or any(reservation.get(key) != value for key, value in binding.items()):
                errors.append(f"cosmetics:commit-eligibility:{index}"); continue
            ownership[item["id"]] = {**reservation, "grantKind": "MILESTONE_CLAIM", "claimState": "COMMITTED"}
            del reservations[claim_id]
            result = "COMMITTED"
        else: errors.append(f"cosmetics:unknown-event:{index}:{kind}")
    return {"result": result, "owned": item["id"] in ownership, "balancePatience": balance, "openReservationCount": len(reservations), "ownershipCount": len(ownership)}, errors


def catalog_check() -> None:
    campaign = load_json(ROOT / "ARCHITECTURE/examples/campaign-v2.example.json")
    completion = load_json(ROOT / "ARCHITECTURE/examples/completion-v1.example.json")
    cosmetics = load_json(ROOT / "ARCHITECTURE/examples/cosmetics-v2.example.json")
    cosmetics_draft = load_json(ROOT / "ARCHITECTURE/examples/cosmetics-v2.draft.example.json")
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
    errors.extend(cosmetics_errors(cosmetics_draft, subjects))
    for error in errors: fail(error)
    lifecycle = load_json(TOOL / "fixtures/cosmetics-lifecycle-v1.json")
    catalogs = {"cosmetics-v2.example.json": cosmetics, "cosmetics-v2.draft.example.json": cosmetics_draft}
    for scenario in lifecycle["scenarios"]:
        actual, transition_errors = execute_cosmetics_scenario(scenario, catalogs, puzzle_ids)
        for error in transition_errors: fail(f"{error}:{scenario['name']}")
        if actual != scenario["expected"]: fail(f"cosmetics:scenario-expected:{scenario['name']}")
    scenarios = {scenario["name"]: scenario for scenario in lifecycle["scenarios"]}
    for negative in lifecycle.get("negativeScenarios", []):
        scenario = copy.deepcopy(scenarios[negative["baseScenario"]])
        mutation = negative["mutation"]
        if mutation == "REMOVE_RESERVE_EVENT":
            scenario["events"] = [event for event in scenario["events"] if event["kind"] != "RESERVE_CLAIM"]
        elif mutation == "REMOVE_FIRST_CLEAR_AFTER_RESERVE":
            scenario["events"] = [event for event in scenario["events"] if event["kind"] != "FIRST_CLEAR"]
        elif mutation == "COMMIT_WRONG_ITEM":
            next(event for event in scenario["events"] if event["kind"] == "COMMIT_CLAIM")["itemId"] = "fixture-purchase-object"
        elif mutation == "COMMIT_WRONG_CATALOG_REVISION":
            next(event for event in scenario["events"] if event["kind"] == "COMMIT_CLAIM")["catalogRevision"] = 2
        elif mutation == "REMOVE_FIRST_CLEAR_BEFORE_COMMIT":
            commit_index = next(index for index, event in enumerate(scenario["events"]) if event["kind"] == "COMMIT_CLAIM")
            scenario["events"].insert(commit_index, {"kind": "REMOVE_FIRST_CLEAR", "puzzleId": "S1-01-01-01"})
        else:
            fail(f"cosmetics:unknown-negative-mutation:{mutation}")
            continue
        _, transition_errors = execute_cosmetics_scenario(scenario, catalogs, puzzle_ids)
        if not any(error.startswith(negative["expectedError"]) for error in transition_errors):
            fail(f"cosmetics:negative-not-rejected:{negative['name']}")


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
    raw_watermark = fixture.get("highestReservedOrdinal")
    if not isinstance(raw_watermark, str) or re.fullmatch(r"0|[1-9][0-9]*", raw_watermark) is None:
        return errors + ["endless:watermark-format"]
    try: watermark = int(raw_watermark)
    except Exception: return errors + ["endless:watermark-format"]
    if watermark > 2**64 - 1: errors.append("endless:watermark-range")
    open_ordinals: set[int] = set()
    allowed_states = {"RESERVED_NOT_GENERATED", "ACTIVE_DRAFT", "COMPLETION_CLAIM_OPEN"}
    for record in fixture.get("openEndless", []):
        raw_ordinal = record.get("generationOrdinal")
        if not isinstance(raw_ordinal, str) or re.fullmatch(r"[1-9][0-9]*", raw_ordinal) is None:
            errors.append("endless:open-ordinal-format"); continue
        ordinal = int(raw_ordinal)
        if ordinal > watermark or ordinal in open_ordinals: errors.append("endless:open-ordinal")
        open_ordinals.add(ordinal)
        if record.get("state") not in allowed_states: errors.append("endless:open-state")
        descriptor = record.get("descriptor", {})
        if descriptor.get("generationOrdinal") != raw_ordinal or record.get("id") != endless_identity(descriptor): errors.append("endless:identity")
        if record.get("state") == "RESERVED_NOT_GENERATED":
            if any(key in record for key in ("publicPuzzleInput", "sessionState", "claim", "completionCommitId")): errors.append("endless:reserved-shape")
        elif record.get("state") == "ACTIVE_DRAFT":
            required = {"publicPuzzleInput", "publicPuzzleHash", "solutionHash", "proofHash", "sessionState"}
            if not required.issubset(record) or any(key in record for key in ("claim", "completionCommitId")): errors.append("endless:active-shape")
        elif record.get("state") == "COMPLETION_CLAIM_OPEN":
            if not {"puzzleId", "completionCommitId", "claim"}.issubset(record) or any(key in record for key in ("publicPuzzleInput", "sessionState")): errors.append("endless:claim-shape")
            claim = record.get("claim", {})
            expected_claim = f"reward-claim:POST_CLEAR_PATIENCE:{record.get('puzzleId')}"
            expected_operation = f"reward-operation:POST_CLEAR_PATIENCE:{record.get('puzzleId')}"
            allowed_claim_states = {"RESERVED", "LOCAL_DECISION_PENDING", "PROVIDER_RESERVED", "RECONCILIATION_REQUIRED", "REWARD_CONFIRMED", "NO_REWARD_CONFIRMED"}
            if record.get("puzzleId") != record.get("id") or claim.get("claimId") != expected_claim or claim.get("localOperationId") != expected_operation or claim.get("reasonCode") != "POST_CLEAR_PATIENCE" or claim.get("claimStatus") not in allowed_claim_states: errors.append("endless:claim-binding")
            provider_required = claim.get("claimStatus") in {"PROVIDER_RESERVED", "RECONCILIATION_REQUIRED", "REWARD_CONFIRMED", "NO_REWARD_CONFIRMED"}
            if provider_required != bool(claim.get("providerOperationId")): errors.append("endless:claim-provider-binding")
    if len(open_ordinals) > 20: errors.append("endless:open-capacity")
    if any(key in fixture for key in ("terminalIntervals", "terminalDetails", "terminalCheckpoint")): errors.append("endless:terminal-state-present")
    return errors


def post_clear_patience_amount() -> int:
    completion = load_json(ROOT / "ARCHITECTURE/examples/completion-v1.example.json")
    definitions = [item for item in completion.get("rewardDefinitions", []) if item.get("reasonCode") == "POST_CLEAR_PATIENCE"]
    if len(definitions) != 1 or definitions[0].get("amountPatience") != 10: raise ValueError("completion:post-clear-patience-definition")
    return definitions[0]["amountPatience"]


def execute_endless_scenario(scenario: dict[str, Any]) -> tuple[dict[str, Any], list[str]]:
    state = copy.deepcopy(scenario["initial"])
    open_records = {int(item["generationOrdinal"]): copy.deepcopy(item) for item in state.get("openEndless", [])}
    watermark = int(state["highestReservedOrdinal"])
    balance = state.get("economyBalancePatience", 0)
    save_generation = state.get("saveGeneration", 0)
    last_result = "NO_RESULT"
    errors: list[str] = []
    for index, event in enumerate(scenario["events"]):
        kind = event["kind"]
        if kind == "CRASH":
            continue
        if kind == "RESTART":
            continue
        if kind == "RESERVE":
            if len(open_records) >= 20 or watermark >= 2**64 - 1:
                errors.append(f"endless:transition:{index}:reserve")
                continue
            watermark += 1
            save_generation += 1
            descriptor = {
                "endlessContractVersion": 1, "rulesetVersion": "train-track-v1",
                "generatorVersion": event["generatorVersion"], "seed": event["seed"],
                "generationOrdinal": str(watermark), "parameterHashSha256": event["parameterHashSha256"],
            }
            open_records[watermark] = {"state": "RESERVED_NOT_GENERATED", "generationOrdinal": str(watermark), "id": endless_identity(descriptor), "descriptor": descriptor}
        elif kind == "GENERATE_AND_PROMOTE":
            ordinal = int(event["generationOrdinal"]); record = open_records.get(ordinal)
            if record is None or record["state"] != "RESERVED_NOT_GENERATED": errors.append(f"endless:transition:{index}:generate"); continue
            save_generation += 1
            record["state"] = "ACTIVE_DRAFT"; record["publicPuzzleInput"] = {"puzzleId": record["id"]}; record["publicPuzzleHash"] = {"profile": "STP-PUZZLE-SEMANTIC-JCS-1", "sha256": "b" * 64}; record["solutionHash"] = {"profile": "STP-SOLUTION-JCS-1", "sha256": "c" * 64}; record["proofHash"] = {"profile": "STP-PROOF-JCS-1", "sha256": "d" * 64}; record["sessionState"] = {"moveCount": 0}
        elif kind == "COMPLETE":
            ordinal = int(event["generationOrdinal"]); record = open_records.get(ordinal)
            if record is None or record["state"] != "ACTIVE_DRAFT": errors.append(f"endless:transition:{index}:complete"); continue
            save_generation += 1
            open_records[ordinal] = {"state": "COMPLETION_CLAIM_OPEN", "generationOrdinal": record["generationOrdinal"], "id": record["id"], "descriptor": record["descriptor"], "puzzleId": record["id"], "completionCommitId": f"completion-{ordinal}", "claim": {"claimId": f"reward-claim:POST_CLEAR_PATIENCE:{record['id']}", "localOperationId": f"reward-operation:POST_CLEAR_PATIENCE:{record['id']}", "reasonCode": "POST_CLEAR_PATIENCE", "claimStatus": "LOCAL_DECISION_PENDING", "providerOperationId": None}}
        elif kind == "SKIP_REWARD":
            ordinal = int(event["generationOrdinal"]); record = open_records.get(ordinal)
            if record is None:
                if 1 <= ordinal <= watermark:
                    last_result = "ENDLESS_TERMINAL_DUPLICATE"
                    continue
                errors.append(f"endless:transition:{index}:skip-missing"); continue
            claim = record.get("claim", {})
            if record.get("state") != "COMPLETION_CLAIM_OPEN" or claim.get("claimStatus") != "LOCAL_DECISION_PENDING" or claim.get("providerOperationId") is not None:
                last_result = "SKIP_REJECTED_PROVIDER_RESERVED"
                continue
            if event.get("expectedSaveGeneration") != save_generation:
                last_result = "SAVE_GENERATION_CONFLICT"
                continue
            if event.get("puzzleId") != record.get("puzzleId") or event.get("claimId") != claim.get("claimId") or event.get("localOperationId") != claim.get("localOperationId"):
                errors.append(f"endless:transition:{index}:skip-binding"); continue
            del open_records[ordinal]
            save_generation += 1
            last_result = "SKIPPED_NO_REWARD"
        elif kind == "RESERVE_REWARD_PROVIDER":
            ordinal = int(event["generationOrdinal"]); record = open_records.get(ordinal)
            if record is None:
                if 1 <= ordinal <= watermark:
                    last_result = "PROVIDER_START_ABORTED_TERMINAL"
                    continue
                errors.append(f"endless:transition:{index}:provider-reserve-missing"); continue
            claim = record.get("claim", {})
            if event.get("expectedSaveGeneration") != save_generation:
                last_result = "SAVE_GENERATION_CONFLICT"
                continue
            if record.get("state") != "COMPLETION_CLAIM_OPEN" or claim.get("claimStatus") != "LOCAL_DECISION_PENDING" or claim.get("providerOperationId") is not None:
                errors.append(f"endless:transition:{index}:provider-reserve-state"); continue
            if event.get("puzzleId") != record.get("puzzleId") or event.get("claimId") != claim.get("claimId") or event.get("localOperationId") != claim.get("localOperationId") or not event.get("providerOperationId"):
                errors.append(f"endless:transition:{index}:provider-reserve-binding"); continue
            claim["claimStatus"] = "PROVIDER_RESERVED"
            claim["providerOperationId"] = event["providerOperationId"]
            save_generation += 1
            last_result = "PROVIDER_RESERVED"
        elif kind == "REWARD_RESULT":
            ordinal = int(event["generationOrdinal"]); record = open_records.get(ordinal)
            if record is None:
                if 1 <= ordinal <= watermark:
                    last_result = "LATE_PROVIDER_CALLBACK_QUARANTINED"
                    continue
                errors.append(f"endless:transition:{index}:reward-result"); continue
            if record["state"] != "COMPLETION_CLAIM_OPEN": errors.append(f"endless:transition:{index}:reward-result"); continue
            claim = record["claim"]
            if event.get("puzzleId") != record.get("puzzleId") or event.get("claimId") != claim.get("claimId") or event.get("localOperationId") != claim.get("localOperationId") or not event.get("providerOperationId"): errors.append(f"endless:transition:{index}:reward-binding"); continue
            if claim.get("claimStatus") not in {"PROVIDER_RESERVED", "RECONCILIATION_REQUIRED", "REWARD_CONFIRMED", "NO_REWARD_CONFIRMED"} or claim.get("providerOperationId") != event["providerOperationId"]: errors.append(f"endless:transition:{index}:reward-provider-binding"); continue
            outcome = event.get("outcome")
            terminal_outcome = {"REWARD_CONFIRMED": "REWARDED", "NO_REWARD_CONFIRMED": "NO_REWARD"}.get(claim.get("claimStatus"))
            if terminal_outcome is not None and outcome != terminal_outcome: errors.append(f"endless:transition:{index}:reward-terminal-conflict"); continue
            if outcome == "REWARDED": claim["claimStatus"] = "REWARD_CONFIRMED"
            elif outcome == "UNCERTAIN": claim["claimStatus"] = "RECONCILIATION_REQUIRED"
            elif outcome == "NO_REWARD": claim["claimStatus"] = "NO_REWARD_CONFIRMED"
            else: errors.append(f"endless:transition:{index}:reward-outcome")
            if outcome in {"REWARDED", "UNCERTAIN", "NO_REWARD"}: save_generation += 1
        elif kind == "COMMIT_CLAIM":
            ordinal = int(event["generationOrdinal"]); record = open_records.get(ordinal)
            if record is None or record["state"] != "COMPLETION_CLAIM_OPEN": errors.append(f"endless:transition:{index}:claim"); continue
            claim = record["claim"]
            if event.get("puzzleId") != record.get("puzzleId") or event.get("claimId") != claim.get("claimId") or event.get("localOperationId") != claim.get("localOperationId") or event.get("providerOperationId") != claim.get("providerOperationId") or claim.get("claimStatus") != "REWARD_CONFIRMED" or not claim.get("providerOperationId"):
                errors.append(f"endless:transition:{index}:claim-binding"); continue
            if "amountPatience" in event:
                errors.append(f"endless:transition:{index}:claim-amount-source"); continue
            balance += post_clear_patience_amount(); del open_records[ordinal]; save_generation += 1
        elif kind == "CLOSE_NO_REWARD":
            ordinal = int(event["generationOrdinal"]); record = open_records.get(ordinal)
            if record is None or record["state"] != "COMPLETION_CLAIM_OPEN" or event.get("puzzleId") != record.get("puzzleId") or event.get("claimId") != record["claim"].get("claimId") or event.get("localOperationId") != record["claim"].get("localOperationId") or event.get("providerOperationId") != record["claim"].get("providerOperationId") or record["claim"].get("claimStatus") != "NO_REWARD_CONFIRMED" or not record["claim"].get("providerOperationId"): errors.append(f"endless:transition:{index}:close"); continue
            del open_records[ordinal]; save_generation += 1
        elif kind == "ABANDON":
            ordinal = int(event["generationOrdinal"]); record = open_records.get(ordinal)
            if record is None or record["state"] not in {"RESERVED_NOT_GENERATED", "ACTIVE_DRAFT"}: errors.append(f"endless:transition:{index}:abandon"); continue
            del open_records[ordinal]; save_generation += 1
        else:
            errors.append(f"endless:transition:{index}:unknown")
    result: dict[str, Any] = {"highestReservedOrdinal": str(watermark), "openCount": len(open_records), "economyBalancePatience": balance}
    if len(open_records) == 1:
        only = next(iter(open_records.values())); result.update({"openState": only["state"], "id": only["id"]})
        if "claimStatus" in scenario["expected"]: result["claimStatus"] = only.get("claim", {}).get("claimStatus")
    if "lastResult" in scenario["expected"]: result["lastResult"] = last_result
    if "saveGeneration" in scenario["expected"]: result["saveGeneration"] = save_generation
    expected_terminal = scenario["expected"].get("terminalOrdinal")
    if expected_terminal is not None and not (1 <= int(expected_terminal) <= watermark and int(expected_terminal) not in open_records): errors.append("endless:terminal-predicate")
    if result != {key: value for key, value in scenario["expected"].items() if key != "terminalOrdinal"}: errors.append("endless:scenario-expected")
    return result, errors


def endless_check() -> None:
    fixture = load_json(TOOL / "fixtures/endless-save-v2.example.json")
    for error in endless_fixture_errors(fixture): fail(error)
    for scenario in fixture.get("transitionScenarios", []):
        _, errors = execute_endless_scenario(scenario)
        for error in errors: fail(f"{error}:{scenario['name']}")
    descriptor_template = {"generatorVersion": "fixture-generator-v1", "parameterHashSha256": "a" * 64}
    long_run = {"initial": {"highestReservedOrdinal": "0", "openEndless": [], "economyBalancePatience": 0}, "events": [], "expected": {"highestReservedOrdinal": "10000", "openCount": 0, "economyBalancePatience": 66670}}
    expected_save_generation = 0
    for ordinal in range(1, 10001):
        long_run["events"].append({"kind": "RESERVE", "seed": str(ordinal), **descriptor_template})
        expected_save_generation += 1
        if ordinal % 3 == 0:
            long_run["events"].append({"kind": "ABANDON", "generationOrdinal": str(ordinal)})
            expected_save_generation += 1
        else:
            descriptor = {"endlessContractVersion": 1, "rulesetVersion": "train-track-v1", "generatorVersion": descriptor_template["generatorVersion"], "seed": str(ordinal), "generationOrdinal": str(ordinal), "parameterHashSha256": descriptor_template["parameterHashSha256"]}
            puzzle_id = endless_identity(descriptor)
            claim_id = f"reward-claim:POST_CLEAR_PATIENCE:{puzzle_id}"
            local_operation_id = f"reward-operation:POST_CLEAR_PATIENCE:{puzzle_id}"
            provider_operation_id = f"provider-{ordinal}"
            bound = {"generationOrdinal": str(ordinal), "puzzleId": puzzle_id, "claimId": claim_id, "localOperationId": local_operation_id, "providerOperationId": provider_operation_id}
            long_run["events"].append({"kind": "GENERATE_AND_PROMOTE", "generationOrdinal": str(ordinal)})
            expected_save_generation += 1
            long_run["events"].append({"kind": "COMPLETE", "generationOrdinal": str(ordinal)})
            expected_save_generation += 1
            long_run["events"].append({"kind": "RESERVE_REWARD_PROVIDER", "expectedSaveGeneration": expected_save_generation, **bound})
            expected_save_generation += 1
            long_run["events"].append({"kind": "REWARD_RESULT", **bound, "outcome": "REWARDED"})
            expected_save_generation += 1
            long_run["events"].append({"kind": "COMMIT_CLAIM", **bound})
            expected_save_generation += 1
    _, long_errors = execute_endless_scenario(long_run)
    for error in long_errors: fail(f"{error}:long-run")
    skip_run = {"initial": {"highestReservedOrdinal": "0", "openEndless": [], "economyBalancePatience": 0}, "events": [], "expected": {"highestReservedOrdinal": "100", "openCount": 0, "economyBalancePatience": 0}}
    expected_save_generation = 0
    for ordinal in range(1, 101):
        descriptor = {"endlessContractVersion": 1, "rulesetVersion": "train-track-v1", "generatorVersion": descriptor_template["generatorVersion"], "seed": f"skip-{ordinal}", "generationOrdinal": str(ordinal), "parameterHashSha256": descriptor_template["parameterHashSha256"]}
        puzzle_id = endless_identity(descriptor)
        skip_run["events"].append({"kind": "RESERVE", "seed": f"skip-{ordinal}", **descriptor_template})
        expected_save_generation += 1
        skip_run["events"].append({"kind": "GENERATE_AND_PROMOTE", "generationOrdinal": str(ordinal)})
        expected_save_generation += 1
        skip_run["events"].append({"kind": "COMPLETE", "generationOrdinal": str(ordinal)})
        expected_save_generation += 1
        skip_run["events"].append({"kind": "SKIP_REWARD", "expectedSaveGeneration": expected_save_generation, "generationOrdinal": str(ordinal), "puzzleId": puzzle_id, "claimId": f"reward-claim:POST_CLEAR_PATIENCE:{puzzle_id}", "localOperationId": f"reward-operation:POST_CLEAR_PATIENCE:{puzzle_id}"})
        expected_save_generation += 1
    _, skip_errors = execute_endless_scenario(skip_run)
    for error in skip_errors: fail(f"{error}:100-skips")
    golden = load_json(TOOL / "fixtures/endless-save-v1-to-v2.golden.json")
    source, expected = golden["source"], golden["expected"]
    highest = int(source["nextGenerationOrdinal"]) - 1
    active_ordinals = {int(item["generationOrdinal"]) for item in source["activeDrafts"]}
    terminal: set[int] = set()
    for interval in source["terminalIntervals"]:
        terminal.update(range(int(interval["firstOrdinal"]), int(interval["lastOrdinal"]) + 1))
    claims = {int(item["generationOrdinal"]): item for item in source.get("openRewardClaims", [])}
    if active_ordinals & terminal or active_ordinals | terminal != set(range(1, highest + 1)) or not set(claims).issubset(terminal): fail("endless:migration-prefix")
    migrated: list[dict[str, Any]] = []
    for item in source["activeDrafts"]: migrated.append(copy.deepcopy(item))
    for ordinal, item in claims.items():
        record = copy.deepcopy(item); record["state"] = "COMPLETION_CLAIM_OPEN"
        record["claim"]["reasonCode"] = "POST_CLEAR_PATIENCE"
        record["claim"]["localOperationId"] = f"reward-operation:POST_CLEAR_PATIENCE:{record['puzzleId']}"
        migrated.append(record)
    migrated.sort(key=lambda item: int(item["generationOrdinal"]))
    derived = {"saveSchemaVersion": 2, "highestReservedOrdinal": str(highest), "openEndless": migrated, "economyBalancePatience": source["economyBalancePatience"]}
    for error in endless_fixture_errors(derived): fail(f"endless:migration-{error}")
    if expected != derived: fail("endless:migration-golden")


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


def execute_privacy_scenario(scenario: dict[str, Any]) -> tuple[dict[str, Any], list[str]]:
    decision = "NONE"; sync = "UNKNOWN"; analytics = False; ads = False; fence = False
    signals_applied = False; native_enabled = False; native_disabled = False; result: str | None = None
    errors: list[str] = []
    for index, event in enumerate(scenario["events"]):
        kind = event["kind"]
        if kind == "BOOTSTRAP_PRE_SDK_DENY":
            fence = event.get("available") is True; analytics = False; ads = False
        elif kind == "LOAD_DECISION":
            record = event["record"]
            mapping = {
                "NONE": ("NONE", "UNKNOWN"),
                "VALID_CURRENT_ALLOW_ANALYTICS": ("VALID_CURRENT_ALLOW_ANALYTICS", "UNKNOWN"),
                "REVOKED_CONFIRMED_CURRENT": ("REVOKED", "REVOKED_CONFIRMED"),
                "ENABLED_CONFIRMED_CURRENT": ("ALLOW_ANALYTICS", "ENABLED_CONFIRMED"),
                "REVOKE_PENDING_CURRENT": ("REVOKED", "REVOKE_PENDING"),
                "ENABLED_CONFIRMED_STALE_REVISION": ("INVALID", "UNKNOWN"),
            }
            if record not in mapping: errors.append(f"privacy:record:{index}")
            else: decision, sync = mapping[record]
            analytics = False; ads = False
            if decision == "INVALID": result = "FAIL_CLOSED"
            elif sync == "REVOKED_CONFIRMED": result = "DENIED_CONFIRMED"
        elif kind in {"START_UMP", "NETWORK_CHANGED"}:
            continue
        elif kind == "PERSIST_NEW_ALLOW_DECISION":
            decision = "ALLOW_ANALYTICS"; sync = "ENABLE_PENDING"; analytics = False
        elif kind == "APPLY_PRIVACY_SIGNALS":
            signals_applied = event.get("success") is True
            if not signals_applied: result = "RECONCILIATION_REQUIRED"
        elif kind == "REQUEST_REENABLE":
            if not fence:
                analytics = False; result = "PRE_SDK_FENCE_REQUIRED"
            elif decision not in {"ALLOW_ANALYTICS", "VALID_CURRENT_ALLOW_ANALYTICS"} or not signals_applied:
                errors.append(f"privacy:enable-precondition:{index}")
            else:
                sync = "ENABLE_PENDING"
        elif kind == "APPLY_NATIVE_ENABLE":
            if not fence or sync != "ENABLE_PENDING" or not signals_applied or event.get("success") is not True:
                errors.append(f"privacy:native-enable:{index}")
            else: native_enabled = True
        elif kind == "CONFIRM_ENABLED":
            if not native_enabled or sync != "ENABLE_PENDING": errors.append(f"privacy:confirm-enabled:{index}")
            else: sync = "ENABLED_CONFIRMED"; analytics = True; result = "ENABLED_CONFIRMED"
        elif kind == "REQUEST_REVOKE":
            decision = "REVOKED"; sync = "REVOKE_PENDING"; analytics = False; ads = False; result = "RECONCILIATION_REQUIRED"
        elif kind == "APPLY_NATIVE_DISABLE":
            if sync != "REVOKE_PENDING": errors.append(f"privacy:disable-precondition:{index}")
            native_disabled = event.get("success") is True
            if not native_disabled: result = "RECONCILIATION_REQUIRED"
        elif kind == "CONFIRM_REVOKED":
            if sync != "REVOKE_PENDING" or not native_disabled: errors.append(f"privacy:confirm-revoked:{index}")
            else: sync = "REVOKED_CONFIRMED"; result = "REVOKED_CONFIRMED"
        elif kind in {"CRASH", "RESTART"}:
            analytics = False; ads = False; signals_applied = False; native_enabled = False; native_disabled = False
        else:
            errors.append(f"privacy:unknown-event:{index}:{kind}")
    if result is None:
        result = "FAIL_CLOSED" if sync == "UNKNOWN" else ("DENIED_CONFIRMED" if sync in {"DENIED_CONFIRMED", "REVOKED_CONFIRMED"} else "RECONCILIATION_REQUIRED")
    return {"decision": decision, "nativeSyncState": sync, "analytics": analytics, "ads": ads, "result": result}, errors


def privacy_errors(value: dict[str, Any]) -> list[str]:
    errors: list[str] = []
    providers = value.get("productionProviders", {})
    expected_providers = {"ads": "GoogleMobileAdsUnity-11.5.0", "analytics": "EXCLUDED_UNTIL_PRE_SDK_FENCE_PROVEN", "crash": "EXCLUDED", "iap": "UnityIAP-5.4.3-LAZY_READINESS"}
    if value.get("contractVersion") != 2 or providers != expected_providers: errors.append("privacy:provider-pins")
    scenarios = {item["name"]: item for item in value.get("scenarios", [])}
    expected_names = {"FRESH_INSTALL", "DIRECT_LEGACY_UPGRADE_STALE_TRUE", "RESET_ONLY_INSTALLED_NEVER_LAUNCHED", "OFFLINE_CONFIRMED_DENIED_RESTART", "REVOKE_CRASH_AFTER_PENDING", "REVOKE_DISABLE_FAILURE", "REENABLE_WITHOUT_FENCE", "REENABLE_WITH_PROVEN_FENCE_REFERENCE", "OFFLINE_INVALID_REVISION"}
    if set(scenarios) != expected_names: errors.append("privacy:scenario-set")
    direct_events = scenarios.get("DIRECT_LEGACY_UPGRADE_STALE_TRUE", {}).get("events", [])
    if not direct_events or direct_events[0].get("kind") != "BOOTSTRAP_PRE_SDK_DENY" or direct_events[0].get("available") is not False or direct_events[0].get("legacyPersistedAnalyticsOverride") is not True:
        errors.append("privacy:direct-upgrade-input")
    intermediate_events = scenarios.get("RESET_ONLY_INSTALLED_NEVER_LAUNCHED", {}).get("events", [])
    if not intermediate_events or intermediate_events[0].get("kind") != "BOOTSTRAP_PRE_SDK_DENY" or intermediate_events[0].get("resetOnlyIntermediateLaunchObserved") is not False:
        errors.append("privacy:intermediate-input")
    for name, scenario in scenarios.items():
        actual, scenario_errors = execute_privacy_scenario(scenario)
        errors.extend(f"{error}:{name}" for error in scenario_errors)
        if actual != scenario.get("expected"): errors.append(f"privacy:expected:{name}")
    reference = scenarios.get("REENABLE_WITH_PROVEN_FENCE_REFERENCE", {})
    if reference.get("productionExecutable") is not False: errors.append("privacy:reenable-reference-only")
    return errors


def privacy_check() -> None:
    for error in privacy_errors(load_json(TOOL / "fixtures/privacy-lifecycle-v2.json")): fail(error)


def release_errors(rc: dict[str, Any], staging: dict[str, Any], receipt: dict[str, Any]) -> list[str]:
    errors: list[str] = []
    if rc["buildProfile"] != "production" or not rc["promotionAllowed"] or any(rc["debugFlags"].values()): errors.append("release:rc-not-production-identical")
    if any(token in rc["applicationIdentifier"].lower() for token in (".dev", ".qa", ".staging")): errors.append("release:rc-app-id")
    if staging["buildProfile"] != "staging" or staging["promotionAllowed"] is not False: errors.append("release:staging-promotable")
    identity_fields = ["releaseVersion", "gitCommit", "platform", "buildNumber", "applicationIdentifier", "signingFingerprintSha256", "toolchainLockSha256", "packageLockSha256", "contentLockSha256", "productionConfigSha256", "artifactSha256", "storeBuildReference"]
    if any(receipt.get(key) != rc.get(key) for key in identity_fields): errors.append("release:promotion-identity")
    return errors


def release_lock_errors(lock: dict[str, Any], level: dict[str, Any], proof: dict[str, Any]) -> list[str]:
    errors: list[str] = []
    entry = lock["puzzles"][0]
    public_projection = {key: level[key] for key in ("puzzleId", "rulesetVersion", "grid", "endpoints", "rowCounts", "columnCounts")}
    public_hash = {"profile": "STP-PUZZLE-SEMANTIC-JCS-1", "sha256": digest(public_projection)}
    solution_projection = {"puzzleId": level["puzzleId"], "publicPuzzleHash": public_hash, "path": level["solution"]["path"]}
    solution_hash = {"profile": "STP-SOLUTION-JCS-1", "sha256": digest(solution_projection)}
    checks = {
        "puzzle-id": entry.get("puzzleId") == level.get("puzzleId") == proof.get("puzzleId"),
        "document-version": entry.get("documentSchemaVersion") == level.get("documentSchemaVersion"),
        "document-hash": entry.get("documentSha256") == digest(level),
        "public-puzzle-hash": entry.get("publicPuzzleHash") == public_hash == proof.get("publicPuzzleHash"),
        "solution-hash": entry.get("solutionHash") == solution_hash == proof.get("solutionHash"),
        "proof-format": entry.get("proofFormatVersion") == proof.get("proofFormatVersion") == level.get("proofRef", {}).get("proofFormatVersion"),
        "solver-version": entry.get("solverVersion") == proof.get("solverVersion"),
        "proof-hash": entry.get("proofHash") == proof.get("proofHash") == level.get("proofRef", {}).get("proofHash"),
    }
    for field, valid in checks.items():
        if not valid: errors.append(f"release-lock:{field}")
    return errors


def release_check() -> None:
    rc = load_json(ROOT / "ARCHITECTURE/examples/release-manifest-v1.rc.example.json")
    staging = load_json(ROOT / "ARCHITECTURE/examples/release-manifest-v1.staging.example.json")
    receipt = load_json(ROOT / "ARCHITECTURE/examples/promotion-receipt-v1.example.json")
    for error in release_errors(rc, staging, receipt): fail(error)
    lock = load_json(ROOT / "ARCHITECTURE/examples/release-lock-v1.example.json")
    level = load_json(ROOT / "ARCHITECTURE/examples/level-v2.example.json")
    proof = load_json(ROOT / "ARCHITECTURE/examples/proof-v1.example.json")
    for error in release_lock_errors(lock, level, proof): fail(error)


def parse_utc_timestamp(value: Any) -> datetime | None:
    if not isinstance(value, str) or not value.endswith("Z"):
        return None
    try:
        parsed = datetime.fromisoformat(value[:-1] + "+00:00")
    except ValueError:
        return None
    return parsed if parsed.tzinfo == timezone.utc else None


def rollout_decision(contract: dict[str, Any], gate: dict[str, Any]) -> str:
    evidence = gate.get("evidence", {})
    platform = evidence.get("platform")
    item = next((entry for entry in contract.get("platforms", []) if entry.get("platform") == platform), None)
    if item is None:
        return "PAUSE_NO_ADVANCE"
    required = set(contract.get("evidence", {}).get("requiredFields", []))
    if not required.issubset(evidence) or any(evidence.get(key) in {None, ""} for key in required if key not in {"freshness", "population"}):
        return "PAUSE_NO_ADVANCE"
    if evidence.get("source") != item.get("source") or evidence.get("metric") != item.get("metric"):
        return "PAUSE_NO_ADVANCE"
    if evidence.get("releaseIdentity") != gate.get("expectedReleaseIdentity") or evidence.get("storeBuildReference") != gate.get("expectedStoreBuildReference"):
        return "PAUSE_NO_ADVANCE"
    stage = next((entry for entry in item.get("stages", []) if entry.get("percentage") == evidence.get("stagePercentage")), None)
    start = parse_utc_timestamp(evidence.get("windowStartUtc")); end = parse_utc_timestamp(evidence.get("windowEndUtc")); observed = parse_utc_timestamp(evidence.get("observedAtUtc"))
    freshness = evidence.get("freshness") if isinstance(evidence.get("freshness"), dict) else {}
    observed_through = parse_utc_timestamp(freshness.get("observedThroughUtc"))
    if stage is None or None in {start, end, observed, observed_through} or not freshness.get("sourceField"):
        return "PAUSE_NO_ADVANCE"
    assert start is not None and end is not None and observed is not None and observed_through is not None
    window_hours = (end - start).total_seconds() / 3600
    data_age_hours = (observed - observed_through).total_seconds() / 3600
    if window_hours < stage["minimumObservationHours"] or observed < end or data_age_hours < 0 or data_age_hours > item["availability"]["maximumAgeHours"]:
        return "PAUSE_NO_ADVANCE"
    if evidence.get("reportingComplete") is not True or not isinstance(evidence.get("reviewer"), str) or not evidence["reviewer"].strip():
        return "PAUSE_NO_ADVANCE"
    numerator, denominator, crash_rate = evidence.get("numerator"), evidence.get("denominator"), evidence.get("crashRate")
    if isinstance(numerator, bool) or isinstance(denominator, bool) or isinstance(crash_rate, bool) or not isinstance(numerator, (int, float)) or not isinstance(denominator, (int, float)) or not isinstance(crash_rate, (int, float)):
        return "PAUSE_NO_ADVANCE"
    if numerator < 0 or denominator <= 0 or numerator > denominator or not 0 <= crash_rate <= 1 or abs((numerator / denominator) - crash_rate) > 1e-12:
        return "PAUSE_NO_ADVANCE"
    population = evidence.get("population") if isinstance(evidence.get("population"), dict) else {}
    if platform == "ANDROID":
        distinct_users = population.get("distinctUsers")
        if not isinstance(distinct_users, int) or isinstance(distinct_users, bool) or distinct_users < item["availability"]["minimumDistinctUsers"] or denominator != distinct_users:
            return "PAUSE_NO_ADVANCE"
    elif platform == "IOS":
        sessions, active_devices = population.get("sessions"), population.get("activeDevices")
        if not isinstance(sessions, int) or isinstance(sessions, bool) or not isinstance(active_devices, int) or isinstance(active_devices, bool):
            return "PAUSE_NO_ADVANCE"
        if sessions < item["availability"]["minimumSessions"] or active_devices < item["availability"]["minimumActiveDevices"] or denominator != sessions:
            return "PAUSE_NO_ADVANCE"
    else:
        return "PAUSE_NO_ADVANCE"
    if crash_rate >= contract["thresholds"]["haltInclusive"]: return "HALT_AND_ROLL_BACK_IF_AVAILABLE"
    if crash_rate >= contract["thresholds"]["warnInclusive"]: return "PAUSE_AND_INVESTIGATE"
    return "ADVANCE_OR_HOLD_AT_100"


def rollout_errors(contract: dict[str, Any]) -> list[str]:
    errors: list[str] = []
    if contract.get("contractVersion") != 2 or contract.get("metricId") != "store-crash-rate-v2": errors.append("rollout:contract")
    if contract.get("thresholds") != {"warnInclusive": 0.005, "haltInclusive": 0.01}: errors.append("rollout:thresholds")
    platforms = {item["platform"]: item for item in contract.get("platforms", [])}
    if set(platforms) != {"ANDROID", "IOS"}: errors.append("rollout:platforms"); return errors
    android, ios = platforms["ANDROID"], platforms["IOS"]
    if android.get("source") != "Google Play Developer Reporting API vitals.crashrate" or android.get("metric") != "userPerceivedCrashRate" or android.get("releaseIdentityDimension") != "versionCode": errors.append("rollout:android-source")
    if android.get("availability") != {"minimumDistinctUsers": 100, "freshnessFieldRequired": True, "maximumAgeHours": 48, "insufficientDataAction": "PAUSE_NO_ADVANCE"}: errors.append("rollout:android-availability")
    if [item["percentage"] for item in android.get("stages", [])] != [1, 5, 20, 50, 100]: errors.append("rollout:android-stages")
    if ios.get("source") != "App Store Connect Analytics App Crashes plus App Sessions reports" or ios.get("metric") != "crashes divided by sessions" or ios.get("releaseIdentityDimension") != "appVersion": errors.append("rollout:ios-source")
    if ios.get("availability") != {"minimumSessions": 100, "minimumActiveDevices": 5, "maximumAgeHours": 120, "insufficientDataAction": "PAUSE_NO_ADVANCE"}: errors.append("rollout:ios-availability")
    if [item["percentage"] for item in ios.get("stages", [])] != [1, 2, 5, 10, 20, 50, 100]: errors.append("rollout:ios-stages")
    decisions = {item["action"] for item in contract.get("decisions", [])}
    if decisions != {"PAUSE_NO_ADVANCE", "ADVANCE_OR_HOLD_AT_100", "PAUSE_AND_INVESTIGATE", "HALT_AND_ROLL_BACK_IF_AVAILABLE"}: errors.append("rollout:decisions")
    required = {"platform", "releaseIdentity", "storeBuildReference", "stagePercentage", "windowStartUtc", "windowEndUtc", "observedAtUtc", "source", "metric", "numerator", "denominator", "crashRate", "freshness", "population", "reportingComplete", "reviewer"}
    if set(contract.get("evidence", {}).get("requiredFields", [])) != required: errors.append("rollout:evidence-fields")
    fixtures = contract.get("evidenceFixtures", [])
    if {item.get("evidence", {}).get("platform") for item in fixtures} != {"ANDROID", "IOS"}: errors.append("rollout:evidence-fixtures")
    for gate in fixtures:
        if rollout_decision(contract, gate) != gate.get("expectedDecision"): errors.append(f"rollout:evidence-decision:{gate.get('name')}")
    if fixtures:
        threshold_gate = copy.deepcopy(fixtures[0]); threshold_gate["evidence"]["numerator"] = 5; threshold_gate["evidence"]["denominator"] = 1000; threshold_gate["evidence"]["population"]["distinctUsers"] = 1000; threshold_gate["evidence"]["crashRate"] = 0.005
        if rollout_decision(contract, threshold_gate) != "PAUSE_AND_INVESTIGATE": errors.append("rollout:warn")
        threshold_gate["evidence"]["numerator"] = 10; threshold_gate["evidence"]["crashRate"] = 0.01
        if rollout_decision(contract, threshold_gate) != "HALT_AND_ROLL_BACK_IF_AVAILABLE": errors.append("rollout:halt")
    return errors


def rollout_check() -> None:
    for error in rollout_errors(load_json(TOOL / "fixtures/rollout-metric-v1.json")): fail(error)


def review_contract_check() -> None:
    contract = load_json(TOOL / "fixtures/review-contracts-v0.5.json")
    findings = contract.get("findings", {})
    endless = findings.get("HIGH-001-ENDLESS-SKIP", {})
    cosmetics = findings.get("HIGH-002-COSMETICS-RESERVATION", {})
    rollout = findings.get("HIGH-003-ROLLOUT-EVIDENCE", {})
    anchor = findings.get("HIGH-004-WP-SCOPE-ANCHOR", {})
    checks = [
        contract.get("architectureVersion") == "0.5" and contract.get("workPackageId") == "WP-005",
        endless.get("initialClaimStatus") == "LOCAL_DECISION_PENDING",
        endless.get("localSkipRequiresProviderOperation") is False and endless.get("localSkipLedgerDelta") == 0,
        endless.get("providerReservationBeforeSdk") is True and endless.get("skipProviderRaceUsesSaveGeneration") is True and endless.get("minimumRepeatedSkips") == 100,
        cosmetics.get("reservationRequiredBeforeCommit") is True and cosmetics.get("eligibilityValidatedBeforeReservation") is True and cosmetics.get("atomicOwnershipCommit") is True,
        set(cosmetics.get("bindings", [])) == {"claimId", "operationId", "claimGeneration", "itemId", "catalogId", "catalogRevision", "catalogHashSha256", "acquisition", "eligibilityContractVersion", "eligibilityProjectionHashSha256"},
        rollout.get("completeEvidenceObjectRequired") is True and rollout.get("incompleteEvidenceAction") == "PAUSE_NO_ADVANCE",
        anchor.get("workPackageAndManifestSameAddCommit") is True and anchor.get("historicalWorkPackageBlobRequired") is True and anchor.get("historicalExactManifestLinkRequired") is True and anchor.get("futureProductionManifestAllowedWithOwnAnchor") is True,
    ]
    if not all(checks): fail("review-contract:v0.5")
    tokens = {
        "HIGH-001": ("ARCHITECTURE/PERSISTENCE.md", "LOCAL_DECISION_PENDING"),
        "HIGH-002": ("ARCHITECTURE/CONTENT_CATALOGS.md", "eligibilityProjectionHashSha256"),
        "HIGH-003": ("ARCHITECTURE/BUILD_AND_RELEASE.md", "reportingComplete"),
        "HIGH-004": ("ARCHITECTURE/TEST_STRATEGY.md", "historischen Work-Package-Blob"),
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


def is_global_pattern(pattern: str) -> bool:
    parts = pattern.split("/")
    return pattern in {"*", "**", "**/*", "*/**"} or all(part in {"*", "**"} for part in parts)


def segment_glob_match(path: str, pattern: str) -> bool:
    path_parts = PurePosixPath(path).parts
    pattern_parts = PurePosixPath(pattern).parts
    def match(path_index: int, pattern_index: int) -> bool:
        if pattern_index == len(pattern_parts): return path_index == len(path_parts)
        token = pattern_parts[pattern_index]
        if token == "**":
            return match(path_index, pattern_index + 1) or (path_index < len(path_parts) and match(path_index + 1, pattern_index))
        return path_index < len(path_parts) and fnmatch.fnmatchcase(path_parts[path_index], token) and match(path_index + 1, pattern_index + 1)
    return match(0, 0)


def scope_manifest_errors(manifest: dict[str, Any], changed: list[str], requested_scope: str, manifest_rel: str | None = None) -> list[str]:
    errors: list[str] = []
    if manifest.get("scope") != requested_scope: errors.append("scope:mode-mismatch")
    patterns = manifest.get("allowedPathPatterns", [])
    for pattern in patterns:
        parts = PurePosixPath(pattern).parts
        if pattern.startswith("/") or "\\" in pattern or ".." in parts or is_global_pattern(pattern): errors.append(f"scope:unsafe-pattern:{pattern}")
        if not parts or any(token in parts[0] for token in ("*", "?", "[")): errors.append(f"scope:unsafe-root-segment:{pattern}")
        if any("**" in part and part != "**" for part in parts): errors.append(f"scope:unsafe-double-star-segment:{pattern}")
    if manifest_rel is not None:
        wp = manifest.get("workPackageId", "")
        expected = f"tools/architecture-validation/scopes/{wp}.{requested_scope}.scope.json"
        if manifest_rel != expected: errors.append("scope:manifest-name-or-location")
        if manifest_rel not in patterns: errors.append("scope:manifest-self-not-explicit")
        if any(pattern.startswith("tools/architecture-validation/scopes/") and pattern != manifest_rel for pattern in patterns):
            errors.append("scope:other-manifests-allowed")
    for rel in changed:
        if rel.startswith("/") or "\\" in rel or ".." in PurePosixPath(rel).parts: errors.append(f"scope:unsafe-path:{rel}"); continue
        if not any(segment_glob_match(rel, pattern) for pattern in patterns): errors.append(f"scope:out-of-scope:{rel}")
        if rel.startswith("Stammstrecken_Puzzle_Konzept_00-15/"): errors.append(f"scope:product-source:{rel}")
        if requested_scope == "documentation":
            forbidden_roots = ("Assets/", "Packages/", "ProjectSettings/", "Content/")
            forbidden_suffixes = {".cs", ".asmdef", ".unity", ".prefab", ".uxml", ".uss", ".shader"}
            if rel.startswith(forbidden_roots) or Path(rel).suffix.lower() in forbidden_suffixes: errors.append(f"scope:production-artifact:{rel}")
    return errors


def normalized_blocker_text(text: str) -> str:
    return re.sub(r"2026-09-(?:08|12|13)|Architecture v0\.[2345]|Architecture-v0\.[2345]", "<VERSION-METADATA>", text)


def scope_manifest_trust_errors(manifest: dict[str, Any], manifest_rel: str, tracked: bool, anchors: list[str], anchor_parent: str, anchor_bytes: bytes, current_bytes: bytes) -> list[str]:
    errors: list[str] = []
    expected = f"tools/architecture-validation/scopes/{manifest.get('workPackageId')}.{manifest.get('scope')}.scope.json"
    if manifest_rel != expected: errors.append("scope:manifest-name-or-location")
    if not tracked: errors.append("scope:manifest-unversioned")
    if len(anchors) != 1: errors.append("scope:manifest-anchor-count")
    if anchor_parent != manifest.get("baseCommit"): errors.append("scope:manifest-base-not-anchor-parent")
    if anchor_bytes != current_bytes: errors.append("scope:manifest-mutated-after-anchor")
    return errors


def historical_link_targets(work_package_rel: str, text: str) -> set[str]:
    inline = re.findall(r"\[[^\]]+\]\(([^)]+)\)", text)
    references = re.findall(r"^\[[^\]]+\]:\s+(\S+)", text, re.MULTILINE)
    targets: set[str] = set()
    for raw in inline + references:
        target = raw.strip().strip("<>").split("#", 1)[0]
        if not target or target.startswith(("http://", "https://", "mailto:")) or target.startswith("/") or "\\" in target:
            continue
        normalized = posixpath.normpath(posixpath.join(posixpath.dirname(work_package_rel), target))
        if normalized != ".." and not normalized.startswith("../"):
            targets.add(normalized)
    return targets


def scope_anchor_binding_errors(manifest: dict[str, Any], manifest_rel: str, added_paths: set[str], parent_paths: set[str], historical_work_packages: dict[str, str]) -> list[str]:
    errors: list[str] = []
    wp_id = manifest.get("workPackageId", "")
    candidates = {path: text for path, text in historical_work_packages.items() if re.fullmatch(rf"WORK_PACKAGES/{re.escape(wp_id)}_[^/]+\.md", path)}
    if manifest_rel not in added_paths: errors.append("scope:manifest-not-added-in-anchor")
    if len(candidates) != 1: errors.append("scope:historical-work-package-count")
    if manifest_rel in parent_paths: errors.append("scope:manifest-existed-before-anchor")
    if any(path in parent_paths for path in candidates): errors.append("scope:work-package-existed-before-anchor")
    if len(candidates) != 1:
        return errors
    work_package_rel, text = next(iter(candidates.items()))
    if work_package_rel not in added_paths: errors.append("scope:work-package-not-added-in-anchor")
    title = re.search(r"^# (WP-[0-9]{3})\b", text, re.MULTILINE)
    body = re.search(r"^`(WP-[0-9]{3})`$", text, re.MULTILINE)
    if not title or not body or title.group(1) != wp_id or body.group(1) != wp_id: errors.append("scope:historical-work-package-id")
    if manifest_rel not in historical_link_targets(work_package_rel, text): errors.append("scope:historical-work-package-manifest-link")
    return errors


def scope_manifest_argument_error(argument: Path) -> str | None:
    raw = argument.as_posix()
    if argument.is_absolute(): return "scope:absolute-manifest-argument"
    if "\\" in raw or ".." in PurePosixPath(raw).parts: return "scope:external-manifest-argument"
    return None


def git_scope_check(scope: str, manifest_path: Path) -> None:
    lexical = manifest_path.absolute()
    try:
        resolved = manifest_path.resolve(strict=True)
    except OSError:
        fail("scope:manifest-unavailable")
        return
    if resolved != lexical:
        fail("scope:manifest-noncanonical-or-symlink")
        return
    try:
        manifest_rel = resolved.relative_to(ROOT.resolve()).as_posix()
    except ValueError:
        fail("scope:external-manifest")
        return
    manifest = load_json(resolved)
    schema = load_json(TOOL / "scope-manifest-v1.schema.json")
    jsonschema.Draft202012Validator(schema).validate(manifest)
    tracked = subprocess.run(["git", "-C", str(ROOT), "ls-files", "--error-unmatch", "--", manifest_rel], text=True, capture_output=True, check=False)
    anchor_result = subprocess.run(["git", "-C", str(ROOT), "log", "--diff-filter=A", "--format=%H", "--", manifest_rel], text=True, capture_output=True, check=True)
    anchors = [line for line in anchor_result.stdout.splitlines() if line]
    if len(anchors) != 1:
        for error in scope_manifest_trust_errors(manifest, manifest_rel, tracked.returncode == 0, anchors, "", b"", resolved.read_bytes()): fail(error)
        return
    anchor = anchors[0]
    anchor_parent = subprocess.run(["git", "-C", str(ROOT), "rev-parse", f"{anchor}^"], text=True, capture_output=True, check=False)
    anchor_blob = subprocess.run(["git", "-C", str(ROOT), "show", f"{anchor}:{manifest_rel}"], capture_output=True, check=False)
    if anchor_parent.returncode != 0 or anchor_blob.returncode != 0:
        fail("scope:manifest-anchor-unavailable")
        return
    base = manifest["baseCommit"]
    for error in scope_manifest_trust_errors(manifest, manifest_rel, tracked.returncode == 0, anchors, anchor_parent.stdout.strip(), anchor_blob.stdout, resolved.read_bytes()): fail(error)
    anchor_changes = subprocess.run(["git", "-C", str(ROOT), "diff-tree", "--no-commit-id", "--name-status", "-r", anchor], text=True, capture_output=True, check=True).stdout.splitlines()
    added_paths = {line.split("\t", 1)[1] for line in anchor_changes if line.startswith("A\t")}
    anchor_tree = subprocess.run(["git", "-C", str(ROOT), "ls-tree", "-r", "--name-only", anchor, "--", "WORK_PACKAGES"], text=True, capture_output=True, check=True).stdout.splitlines()
    parent_tree = subprocess.run(["git", "-C", str(ROOT), "ls-tree", "-r", "--name-only", anchor_parent.stdout.strip(), "--", "WORK_PACKAGES", manifest_rel], text=True, capture_output=True, check=True).stdout.splitlines()
    wp_prefix = f"WORK_PACKAGES/{manifest.get('workPackageId', '')}_"
    wp_paths = [path for path in anchor_tree if path.startswith(wp_prefix) and path.endswith(".md")]
    historical_work_packages: dict[str, str] = {}
    for path in wp_paths:
        blob = subprocess.run(["git", "-C", str(ROOT), "show", f"{anchor}:{path}"], text=True, capture_output=True, check=False)
        if blob.returncode == 0: historical_work_packages[path] = blob.stdout
    for error in scope_anchor_binding_errors(manifest, manifest_rel, added_paths, set(parent_tree), historical_work_packages): fail(error)
    anchor_ancestor = subprocess.run(["git", "-C", str(ROOT), "merge-base", "--is-ancestor", anchor, "HEAD"], capture_output=True, check=False)
    if anchor_ancestor.returncode != 0: fail("scope:manifest-anchor-not-ancestor")
    exists = subprocess.run(["git", "-C", str(ROOT), "cat-file", "-e", f"{base}^{{commit}}"], capture_output=True, check=False)
    if exists.returncode != 0: fail("scope:base-unavailable"); return
    ancestor = subprocess.run(["git", "-C", str(ROOT), "merge-base", "--is-ancestor", base, "HEAD"], capture_output=True, check=False)
    if ancestor.returncode != 0: fail("scope:base-not-ancestor")
    if re.fullmatch(r"WP-[0-9]{3}", manifest.get("workPackageId", "")) is None or not list((ROOT / "WORK_PACKAGES").glob(manifest["workPackageId"] + "_*.md")):
        fail("scope:work-package")
    name_status = subprocess.run(["git", "-C", str(ROOT), "diff", "--name-status", base, "--"], text=True, capture_output=True, check=True).stdout.splitlines()
    tracked_paths: list[str] = []
    for line in name_status:
        columns = line.split("\t")
        if columns[0].startswith(("R", "C")) and len(columns) == 3:
            tracked_paths.extend(columns[1:])
        elif len(columns) >= 2:
            tracked_paths.append(columns[-1])
    untracked = subprocess.run(["git", "-C", str(ROOT), "ls-files", "--others", "--exclude-standard"], text=True, capture_output=True, check=True).stdout.splitlines()
    changed = sorted(set(tracked_paths + untracked))
    for rel in changed:
        candidate = ROOT / rel
        if candidate.exists():
            try: candidate.resolve().relative_to(ROOT.resolve())
            except ValueError: fail(f"scope:symlink-escape:{rel}")
    for error in scope_manifest_errors(manifest, changed, scope, manifest_rel): fail(error)
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
    if not architecture.startswith("# Stammstrecken-Puzzle – Architecture v1.0") or "**Status:** Angenommen" not in architecture: errors.append("version:architecture")
    if "**Architecture v1.0**" not in current or "Architecture v1.0 (Abnahmekandidat)" in current or "`WP-001` bis `WP-007` sind abgeschlossen" not in current: errors.append("version:current-state")
    if "Architecture v1.0" not in queue or "WP-001` bis `WP-005" not in queue or "CI-Setup-Work-Package | **Abgeschlossen (`WP-007`)**" not in queue: errors.append("version:work-queue")
    if "**Bearbeitungsstatus:** Abgeschlossen" not in work_package: errors.append("version:work-package")
    return errors


def version_and_blocker_check() -> None:
    for error in status_consistency_errors(
        (ROOT / "ARCHITECTURE/ARCHITECTURE.md").read_text(encoding="utf-8"),
        (ROOT / "PROJECT_CONTROL/CURRENT_STATE.md").read_text(encoding="utf-8"),
        (ROOT / "PROJECT_CONTROL/WORK_QUEUE.md").read_text(encoding="utf-8"),
        (ROOT / "WORK_PACKAGES/WP-007_CI-Setup.md").read_text(encoding="utf-8"),
    ): fail(error)
    blockers = (ROOT / "ARCHITECTURE/OPEN_BLOCKERS.md").read_text(encoding="utf-8")
    for number in (1, 2, 3):
        if f"BLOCKER-PROD-00{number}" not in blockers: fail(f"blocker:missing:{number}")
    if blockers.count("| Status | **Offen** |") != 3 or blockers.count("Fail-closed") < 3: fail("blocker:status")


def evidence_summary_lines() -> list[str]:
    return [
        "MANUAL_ARCHITECTURE_REVIEW  Normative IAP-Dokumentreihenfolge und textuelle Systembehauptungen; kein automatischer PASS",
        "CONTRACT_ONLY  JSON-/Markdown-Verträge, Fixtures und Mutationsmodelle; kein Unity-Produktionscode",
        "REQUIRED_LATER/NOT_EXECUTED  production-code-validation: Unity Compile/EditMode/PlayMode, BootstrapCompositionSmoke und IL2CPP",
        "REQUIRED_LATER/NOT_EXECUTED  device-validation: physische Privacy-Gerätecaptures und SDK-Sandbox",
        "REQUIRED_LATER/NOT_EXECUTED  store-validation: Storeupload und Promotion",
        "BLOCKED  BLOCKER-PROD-001, BLOCKER-PROD-002, BLOCKER-PROD-003 bleiben fail-closed",
    ]


def self_test(scope: str, manifest_path: Path) -> None:
    failures: list[str] = []
    def expect(label: str, condition: bool) -> None:
        if not condition: failures.append(label)

    # Retained architecture regressions.
    module_text = (ROOT / "ARCHITECTURE/MODULE_BOUNDARIES.md").read_text(encoding="utf-8")
    mutated = module_text.replace("`STP.Application`, `STP.Infrastructure.Content`", "`STP.Infrastructure.Content`", 1).replace("    Bootstrap[STP.Bootstrap] --> App\n", "")
    expect("REG-ASSEMBLY", "assembly:bootstrap-exact" in assembly_errors(mutated))
    expect("REG-WP", bool(check_wp_identifiers(["WP-001", "WP-02"])))

    # V03-001: privacy inputs and ordering are executed, not only listed.
    privacy = load_json(TOOL / "fixtures/privacy-lifecycle-v2.json")
    bad_direct = copy.deepcopy(privacy)
    next(item for item in bad_direct["scenarios"] if item["name"] == "DIRECT_LEGACY_UPGRADE_STALE_TRUE")["events"][0]["legacyPersistedAnalyticsOverride"] = False
    expect("V03-001-DIRECT-UPGRADE", "privacy:direct-upgrade-input" in privacy_errors(bad_direct))
    bad_intermediate = copy.deepcopy(privacy)
    next(item for item in bad_intermediate["scenarios"] if item["name"] == "RESET_ONLY_INSTALLED_NEVER_LAUNCHED")["events"][0]["resetOnlyIntermediateLaunchObserved"] = True
    expect("V03-001-NEVER-LAUNCHED", "privacy:intermediate-input" in privacy_errors(bad_intermediate))
    bad_revoke = copy.deepcopy(privacy)
    revoke = next(item for item in bad_revoke["scenarios"] if item["name"] == "REVOKE_CRASH_AFTER_PENDING")
    revoke["events"] = [event for event in revoke["events"] if event["kind"] != "APPLY_NATIVE_DISABLE"]
    expect("V03-001-REVOKE-ORDER", any(item.startswith("privacy:disable-precondition") or item.startswith("privacy:expected") for item in privacy_errors(bad_revoke)))
    bad_reference = copy.deepcopy(privacy)
    next(item for item in bad_reference["scenarios"] if item["name"] == "REENABLE_WITH_PROVEN_FENCE_REFERENCE")["productionExecutable"] = True
    expect("V03-001-FAIL-CLOSED", "privacy:reenable-reference-only" in privacy_errors(bad_reference))

    # V03-002: watermark, open shapes, transitions and claim retention.
    endless = load_json(TOOL / "fixtures/endless-save-v2.example.json")
    bad_watermark = copy.deepcopy(endless); bad_watermark["highestReservedOrdinal"] = "18446744073709551616"
    expect("V03-002-WATERMARK-RANGE", "endless:watermark-range" in endless_fixture_errors(bad_watermark))
    bad_open = copy.deepcopy(endless); bad_open["openEndless"][0]["generationOrdinal"] = "4"; bad_open["openEndless"][0]["descriptor"]["generationOrdinal"] = "4"
    expect("V03-002-OPEN-ABOVE-WATERMARK", "endless:open-ordinal" in endless_fixture_errors(bad_open))
    bad_shape = copy.deepcopy(endless); bad_shape["openEndless"][0]["sessionState"] = {"moveCount": 0}
    expect("V03-002-RESERVED-SHAPE", "endless:reserved-shape" in endless_fixture_errors(bad_shape))
    wrong_open_puzzle = copy.deepcopy(endless); wrong_open_puzzle["openEndless"][2]["puzzleId"] = endless["openEndless"][1]["id"]
    expect("V03-002-OPEN-PUZZLE-BINDING", "endless:claim-binding" in endless_fixture_errors(wrong_open_puzzle))
    bad_sequence = copy.deepcopy(endless["transitionScenarios"][1]); bad_sequence["events"][1] = {"kind": "COMPLETE", "generationOrdinal": "1"}
    expect("V03-002-TRANSITION", any(item.startswith("endless:transition") for item in execute_endless_scenario(bad_sequence)[1]))
    bad_claim = copy.deepcopy(endless["transitionScenarios"][1]); bad_claim["events"] = bad_claim["events"][:-1]
    expect("V03-002-CLAIM-RETENTION", "endless:scenario-expected" in execute_endless_scenario(bad_claim)[1])
    wrong_amount = copy.deepcopy(endless["transitionScenarios"][1]); wrong_amount["events"][-1]["amountPatience"] = 999
    expect("V03-002-CLAIM-AMOUNT", any("claim-amount-source" in item for item in execute_endless_scenario(wrong_amount)[1]))
    wrong_claim_id = copy.deepcopy(endless["transitionScenarios"][1]); wrong_claim_id["events"][-1]["claimId"] = "reward-claim:POST_CLEAR_PATIENCE:E1-wrong"
    expect("V03-002-CLAIM-ID", any("claim-binding" in item for item in execute_endless_scenario(wrong_claim_id)[1]))
    wrong_puzzle_id = copy.deepcopy(endless["transitionScenarios"][1]); wrong_puzzle_id["events"][-2]["puzzleId"] = endless["openEndless"][1]["id"]
    expect("V03-002-CALLBACK-PUZZLE-ID", any("reward-binding" in item for item in execute_endless_scenario(wrong_puzzle_id)[1]))
    wrong_local_operation = copy.deepcopy(endless["transitionScenarios"][1]); wrong_local_operation["events"][-2]["localOperationId"] = "reward-operation:foreign"
    expect("V03-002-CALLBACK-LOCAL-OPERATION", any("reward-binding" in item for item in execute_endless_scenario(wrong_local_operation)[1]))
    wrong_provider_operation = copy.deepcopy(endless["transitionScenarios"][1]); wrong_provider_operation["events"][-1]["providerOperationId"] = "provider-foreign"
    expect("V03-002-COMMIT-PROVIDER-OPERATION", any("claim-binding" in item for item in execute_endless_scenario(wrong_provider_operation)[1]))
    no_reward_result = copy.deepcopy(endless["transitionScenarios"][1]); no_reward_result["events"] = [event for event in no_reward_result["events"] if event["kind"] != "REWARD_RESULT"]
    expect("V03-002-CLAIM-STATUS", any("claim-binding" in item for item in execute_endless_scenario(no_reward_result)[1]))
    callback_without_provider_reservation = copy.deepcopy(endless["transitionScenarios"][1]); callback_without_provider_reservation["events"] = [event for event in callback_without_provider_reservation["events"] if event["kind"] != "RESERVE_REWARD_PROVIDER"]
    expect("V03-002-CALLBACK-WITHOUT-PROVIDER-RESERVATION", any("reward-provider-binding" in item for item in execute_endless_scenario(callback_without_provider_reservation)[1]))
    skip_scenario = copy.deepcopy(next(item for item in endless["transitionScenarios"] if item["name"] == "COMPLETE_SKIP_LOCAL_CRASH_RESTART_DUPLICATE"))
    skip_result, skip_errors = execute_endless_scenario(skip_scenario)
    expect("HIGH-001-LOCAL-SKIP-POSITIVE", not skip_errors and skip_result == {key: value for key, value in skip_scenario["expected"].items() if key != "terminalOrdinal"})
    expect("HIGH-001-NO-PROVIDER-ID", all("providerOperationId" not in event for event in skip_scenario["events"] if event["kind"] == "SKIP_REWARD"))
    bad_skip_binding = copy.deepcopy(skip_scenario); next(event for event in bad_skip_binding["events"] if event["kind"] == "SKIP_REWARD")["localOperationId"] = "reward-operation:foreign"
    expect("HIGH-001-SKIP-BINDING", any("skip-binding" in item for item in execute_endless_scenario(bad_skip_binding)[1]))
    provider_wins = copy.deepcopy(next(item for item in endless["transitionScenarios"] if item["name"] == "PROVIDER_RESERVATION_WINS_SKIP_RACE"))
    provider_result, provider_errors = execute_endless_scenario(provider_wins)
    expect("HIGH-001-PROVIDER-WINS", not provider_errors and provider_result == provider_wins["expected"])
    local_wins = copy.deepcopy(next(item for item in endless["transitionScenarios"] if item["name"] == "LOCAL_SKIP_WINS_LATE_PROVIDER_CALLBACK"))
    local_result, local_errors = execute_endless_scenario(local_wins)
    expect("HIGH-001-LOCAL-WINS-LATE-CALLBACK", not local_errors and local_result == {key: value for key, value in local_wins["expected"].items() if key != "terminalOrdinal"})
    skip_after_reward = copy.deepcopy(next(item for item in endless["transitionScenarios"] if item["name"] == "COMPLETE_THEN_CLAIM")); committed_reward = next(event for event in skip_after_reward["events"] if event["kind"] == "COMMIT_CLAIM"); skip_after_reward["events"].append({"kind": "SKIP_REWARD", "generationOrdinal": committed_reward["generationOrdinal"], "expectedSaveGeneration": 6, "puzzleId": committed_reward["puzzleId"], "claimId": committed_reward["claimId"], "localOperationId": committed_reward["localOperationId"]})
    reward_result, reward_errors = execute_endless_scenario(skip_after_reward)
    expect("HIGH-001-SKIP-AFTER-REWARD", not reward_errors and reward_result["economyBalancePatience"] == 10 and reward_result["openCount"] == 0)

    # V03-003: complete migration and release-lock cross references.
    migration = load_json(TOOL / "fixtures/level-progress-v1-to-v2.golden.json")
    for label, mutator, expected_code in (
        ("LEVEL-ID", lambda value: value["source"].__setitem__("levelId", "S1-01-01-99"), "level-progress-migration:source-identity"),
        ("LEGACY-HASH", lambda value: value["source"].__setitem__("puzzleHashSha256", "0" * 64), "level-progress-migration:source-identity"),
        ("SOURCE-SCHEMA", lambda value: value["sourceDocument"].__setitem__("documentSchemaVersion", 9), "level-progress-migration:document-version:sourceDocument"),
        ("SOURCE-DOCUMENT-HASH", lambda value: value["sourceDocument"].__setitem__("documentSha256", "0" * 64), "level-progress-migration:document-binding:sourceDocument"),
    ):
        changed = copy.deepcopy(migration); mutator(changed)
        expect(f"V03-003-MIGRATION-{label}", expected_code in level_progress_migration_errors(changed))
    lock = load_json(ROOT / "ARCHITECTURE/examples/release-lock-v1.example.json")
    level = load_json(ROOT / "ARCHITECTURE/examples/level-v2.example.json")
    proof = load_json(ROOT / "ARCHITECTURE/examples/proof-v1.example.json")
    for field, code, value in (
        ("solutionHash", "release-lock:solution-hash", {"profile": "STP-SOLUTION-JCS-1", "sha256": "0" * 64}),
        ("solverVersion", "release-lock:solver-version", "solver-v99"),
        ("documentSchemaVersion", "release-lock:document-version", 99),
        ("proofHash", "release-lock:proof-hash", {"profile": "STP-PROOF-JCS-1", "sha256": "0" * 64}),
    ):
        changed = copy.deepcopy(lock); changed["puzzles"][0][field] = value
        expect(f"V03-003-LOCK-{field}", code in release_lock_errors(changed, level, proof))

    # V03-004: all legacy focus values and schema-valid DRAFT authoring.
    v2_schema = jsonschema.Draft202012Validator(load_json(ROOT / "ARCHITECTURE/schemas/level-v2.schema.json"))
    for focus in ("EXCLUSION", "CHAIN", "DENSITY", "COMBINATION"):
        changed = copy.deepcopy(level); changed["production"]["focus"] = [focus]
        expect(f"V03-004-FOCUS-{focus}", not list(v2_schema.iter_errors(changed)))
    cosmetics_schema = jsonschema.Draft202012Validator(load_json(ROOT / "ARCHITECTURE/schemas/cosmetics-v2.schema.json"))
    draft_catalog = load_json(ROOT / "ARCHITECTURE/examples/cosmetics-v2.draft.example.json")
    expect("V03-004-COSMETICS-DRAFT", not list(cosmetics_schema.iter_errors(draft_catalog)))

    # HIGH-002: persisted Cosmetics reservation and atomic ownership commit.
    campaign = load_json(ROOT / "ARCHITECTURE/examples/campaign-v2.example.json"); _, puzzle_ids, _ = campaign_subjects(campaign)
    catalogs = {"cosmetics-v2.example.json": load_json(ROOT / "ARCHITECTURE/examples/cosmetics-v2.example.json"), "cosmetics-v2.draft.example.json": draft_catalog}
    lifecycle = load_json(TOOL / "fixtures/cosmetics-lifecycle-v1.json")
    grant = copy.deepcopy(next(item for item in lifecycle["scenarios"] if item["name"] == "MILESTONE_RESERVE_CRASH_RESTART_COMMIT"))
    grant_result, grant_errors = execute_cosmetics_scenario(grant, catalogs, puzzle_ids)
    expect("HIGH-002-POSITIVE-CRASH-RESTART", not grant_errors and grant_result == grant["expected"])
    no_reservation = copy.deepcopy(grant); no_reservation["events"] = [event for event in no_reservation["events"] if event["kind"] != "RESERVE_CLAIM"]
    expect("HIGH-002-NO-RESERVATION", any("commit-without-reservation" in item for item in execute_cosmetics_scenario(no_reservation, catalogs, puzzle_ids)[1]))
    reserve_event = copy.deepcopy(next(event for event in grant["events"] if event["kind"] == "RESERVE_CLAIM")); reserve_event["claimState"] = "RESERVED"
    forged = copy.deepcopy(grant); forged["initial"]["claimReservations"] = [reserve_event]; forged["initial"]["firstClearPuzzleIds"] = []; forged["events"] = [copy.deepcopy(next(event for event in grant["events"] if event["kind"] == "COMMIT_CLAIM"))]
    expect("HIGH-002-FORGED-WITHOUT-ELIGIBILITY", any("commit-eligibility" in item for item in execute_cosmetics_scenario(forged, catalogs, puzzle_ids)[1]))
    for label, field, value in (("CLAIM-ID", "claimId", "cosmetic-milestone-claim:v1:foreign:foreign"), ("ITEM", "itemId", "fixture-purchase-object"), ("ACQUISITION", "acquisition", "PATIENCE_PURCHASE"), ("REVISION", "catalogRevision", 2), ("CATALOG-HASH", "catalogHashSha256", "0" * 64), ("OPERATION", "operationId", "cosmetic-operation:foreign"), ("GENERATION", "claimGeneration", 2), ("ELIGIBILITY-VERSION", "eligibilityContractVersion", 2), ("ELIGIBILITY-HASH", "eligibilityProjectionHashSha256", "0" * 64)):
        changed = copy.deepcopy(grant); next(event for event in changed["events"] if event["kind"] == "COMMIT_CLAIM")[field] = value
        field_errors = execute_cosmetics_scenario(changed, catalogs, puzzle_ids)[1]
        expect(f"HIGH-002-WRONG-{label}", any(("commit-without-reservation" if field == "claimId" else "commit-binding") in item for item in field_errors))
    changed_eligibility = copy.deepcopy(grant); commit_index = next(index for index, event in enumerate(changed_eligibility["events"]) if event["kind"] == "COMMIT_CLAIM"); changed_eligibility["events"].insert(commit_index, {"kind": "REMOVE_FIRST_CLEAR", "puzzleId": "S1-01-01-01"})
    expect("HIGH-002-CHANGED-ELIGIBILITY", any("commit-eligibility" in item for item in execute_cosmetics_scenario(changed_eligibility, catalogs, puzzle_ids)[1]))
    replay = copy.deepcopy(next(item for item in lifecycle["scenarios"] if item["name"] == "MILESTONE_DUPLICATE_IDEMPOTENT")); replay_result, replay_errors = execute_cosmetics_scenario(replay, catalogs, puzzle_ids)
    expect("HIGH-002-REPLAY-IDEMPOTENT", not replay_errors and replay_result == replay["expected"] and replay_result["ownershipCount"] == 1)
    expect("V03-003-COSMETICS-STATUS", not cosmetic_status_transition_allowed("DRAFT", "TOMBSTONE"))

    # V03-005: segment globs, repository-local manifest and immutable trust anchor.
    manifest = load_json(manifest_path)
    manifest_rel = manifest_path.relative_to(ROOT).as_posix()
    expect("V03-005-PRODUCT", any(item.startswith("scope:out-of-scope") or item.startswith("scope:production-artifact") for item in scope_manifest_errors(manifest, ["Assets/StammstreckenPuzzle/Scripts/Foo.cs"], "documentation", manifest_rel)))
    for pattern in ("*", "**", "**/*", "*/**"):
        unsafe = copy.deepcopy(manifest); unsafe["allowedPathPatterns"] = [pattern]
        expect(f"V03-005-GLOBAL-{pattern}", f"scope:unsafe-pattern:{pattern}" in scope_manifest_errors(unsafe, [], "documentation"))
    shallow = copy.deepcopy(manifest); shallow["allowedPathPatterns"] = ["ARCHITECTURE/*"]
    expect("V03-005-SEGMENT", "scope:out-of-scope:ARCHITECTURE/examples/x.json" in scope_manifest_errors(shallow, ["ARCHITECTURE/examples/x.json"], "documentation"))
    expect("V03-005-ABSOLUTE", scope_manifest_argument_error(Path("/tmp/scope.json")) == "scope:absolute-manifest-argument")
    expect("V03-005-EXTERNAL", scope_manifest_argument_error(Path("../scope.json")) == "scope:external-manifest-argument")
    expect("V03-005-UNVERSIONED", "scope:manifest-unversioned" in scope_manifest_trust_errors(manifest, manifest_rel, False, ["a"], manifest["baseCommit"], b"x", b"x"))
    wrong_wp = copy.deepcopy(manifest); wrong_wp["workPackageId"] = "WP-999"
    expect("V03-005-WP-ASSOCIATION", "scope:manifest-name-or-location" in scope_manifest_trust_errors(wrong_wp, manifest_rel, True, ["a"], wrong_wp["baseCommit"], b"x", b"x"))
    expect("V03-005-MUTATED-ANCHOR", "scope:manifest-mutated-after-anchor" in scope_manifest_trust_errors(manifest, manifest_rel, True, ["a"], manifest["baseCommit"], b"original", b"changed"))
    expect("V03-005-SCOPE-ESCAPE", "scope:unsafe-path:../escape" in scope_manifest_errors(manifest, ["../escape"], "documentation", manifest_rel))
    production = copy.deepcopy(manifest); production["scope"] = "production"; production["allowedPathPatterns"] = ["Assets/**/*.cs"]
    expect("V03-005-PRODUCTION", not scope_manifest_errors(production, ["Assets/StammstreckenPuzzle/Scripts/Foo.cs"], "production"))
    product_scope = copy.deepcopy(production); product_scope["allowedPathPatterns"] = ["Stammstrecken_Puzzle_Konzept_00-15/**"]
    expect("V03-005-PRODUCT-SOURCE", any(item.startswith("scope:product-source:") for item in scope_manifest_errors(product_scope, ["Stammstrecken_Puzzle_Konzept_00-15/09_Train_Track_Master_Spezifikation.md"], "production")))
    wp_rel = f"WORK_PACKAGES/{manifest['workPackageId']}_Test.md"
    wp_link = f"../{manifest_rel}"
    historical_wp = f"# {manifest['workPackageId']} – Test\n\n## ID\n\n`{manifest['workPackageId']}`\n\n[Scope]({wp_link})\n"
    added = {manifest_rel, wp_rel}; historical = {wp_rel: historical_wp}
    expect("HIGH-004-COMMON-ANCHOR-POSITIVE", not scope_anchor_binding_errors(manifest, manifest_rel, added, set(), historical))
    expect("HIGH-004-WP-ADDED-LATER", "scope:work-package-existed-before-anchor" in scope_anchor_binding_errors(manifest, manifest_rel, {manifest_rel}, {wp_rel}, historical))
    expect("HIGH-004-SEPARATE-ADD-COMMITS", "scope:work-package-not-added-in-anchor" in scope_anchor_binding_errors(manifest, manifest_rel, {manifest_rel}, set(), historical))
    historical_no_link = {wp_rel: historical_wp.replace(f"[Scope]({wp_link})", "Scope folgt später")}
    expect("HIGH-004-LATER-REFERENCE", "scope:historical-work-package-manifest-link" in scope_anchor_binding_errors(manifest, manifest_rel, added, set(), historical_no_link))
    wrong_historical_id = {wp_rel: historical_wp.replace(f"`{manifest['workPackageId']}`", "`WP-999`")}
    expect("HIGH-004-WRONG-WP-ID", "scope:historical-work-package-id" in scope_anchor_binding_errors(manifest, manifest_rel, added, set(), wrong_historical_id))
    other_manifest_rel = f"tools/architecture-validation/scopes/{manifest['workPackageId']}.alternate.scope.json"
    other_manifest_errors = scope_manifest_errors(manifest, [], "documentation", other_manifest_rel) + scope_anchor_binding_errors(manifest, other_manifest_rel, {other_manifest_rel, wp_rel}, set(), historical)
    expect("HIGH-004-OTHER-MANIFEST-SAME-SCOPE", "scope:manifest-name-or-location" in other_manifest_errors or "scope:historical-work-package-manifest-link" in other_manifest_errors)
    production_manifest = copy.deepcopy(manifest); production_manifest["workPackageId"] = "WP-900"; production_manifest["scope"] = "production"
    production_rel = "tools/architecture-validation/scopes/WP-900.production.scope.json"; production_wp_rel = "WORK_PACKAGES/WP-900_Produktionspaket.md"
    production_wp = "# WP-900 – Produktionspaket\n\n## ID\n\n`WP-900`\n\n[Scope](../tools/architecture-validation/scopes/WP-900.production.scope.json)\n"
    expect("HIGH-004-FUTURE-PRODUCTION-ALLOWED", not scope_anchor_binding_errors(production_manifest, production_rel, {production_rel, production_wp_rel}, set(), {production_wp_rel: production_wp}))

    # V03-006: historical ADR-016 decision body must remain byte-equivalent as text.
    current_016 = (ROOT / "DECISIONS/ADR-016-katalogvertraege-und-endless-identitaet.md").read_text(encoding="utf-8")
    historical_016 = subprocess.run(["git", "-C", str(ROOT), "show", f"{ADR016_HISTORY_COMMIT}:DECISIONS/ADR-016-katalogvertraege-und-endless-identitaet.md"], text=True, capture_output=True, check=True).stdout
    mutated_016 = current_016.replace("campaign-v1", "campaign-v9", 1)
    expect("V03-006-HISTORY", section_body(mutated_016, "## Entscheidung", "## Begründung") != section_body(historical_016, "## Entscheidung", "## Begründung"))

    # HIGH-003: no automatic advance without complete, exact, fresh and sufficiently populated Store evidence.
    rollout = load_json(TOOL / "fixtures/rollout-metric-v1.json")
    gates = {item["evidence"]["platform"]: item for item in rollout["evidenceFixtures"]}
    expect("HIGH-003-ANDROID-POSITIVE", rollout_decision(rollout, gates["ANDROID"]) == "ADVANCE_OR_HOLD_AT_100")
    expect("HIGH-003-IOS-POSITIVE", rollout_decision(rollout, gates["IOS"]) == "ADVANCE_OR_HOLD_AT_100")
    for field in rollout["evidence"]["requiredFields"]:
        changed = copy.deepcopy(gates["ANDROID"]); changed["evidence"].pop(field, None)
        expect(f"HIGH-003-MISSING-{field}", rollout_decision(rollout, changed) == "PAUSE_NO_ADVANCE")
    mutations = [
        ("STALE", lambda gate: gate["evidence"]["freshness"].__setitem__("observedThroughUtc", "2026-09-01T00:00:00Z")),
        ("WRONG-RELEASE", lambda gate: gate["evidence"].__setitem__("releaseIdentity", "foreign")),
        ("WRONG-BUILD", lambda gate: gate["evidence"].__setitem__("storeBuildReference", "foreign")),
        ("WRONG-SOURCE", lambda gate: gate["evidence"].__setitem__("source", "foreign")),
        ("WRONG-METRIC", lambda gate: gate["evidence"].__setitem__("metric", "foreign")),
        ("SHORT-WINDOW", lambda gate: gate["evidence"].__setitem__("windowEndUtc", "2026-09-10T01:00:00Z")),
        ("INCOMPLETE", lambda gate: gate["evidence"].__setitem__("reportingComplete", False)),
        ("BAD-RATE", lambda gate: gate["evidence"].__setitem__("crashRate", 0.9)),
    ]
    for label, mutator in mutations:
        changed = copy.deepcopy(gates["ANDROID"]); mutator(changed)
        expect(f"HIGH-003-{label}", rollout_decision(rollout, changed) == "PAUSE_NO_ADVANCE")
    android_population = copy.deepcopy(gates["ANDROID"]); android_population["evidence"]["population"].pop("distinctUsers")
    expect("HIGH-003-ANDROID-POPULATION", rollout_decision(rollout, android_population) == "PAUSE_NO_ADVANCE")
    ios_population = copy.deepcopy(gates["IOS"]); ios_population["evidence"]["population"].pop("activeDevices")
    expect("HIGH-003-IOS-POPULATION", rollout_decision(rollout, ios_population) == "PAUSE_NO_ADVANCE")

    # IAP fixture order is executable; the normative prose remains a manual review item.
    bad_trace = load_json(TOOL / "fixtures/iap-state-machine-v1.json")["traces"][0]
    events = bad_trace["events"]; bad_trace["events"] = [events[0], events[1], events[3], events[2], events[4]]
    expect("V03-003-IAP-FIXTURE", any(item.startswith("iap:finalize-order") for item in execute_iap_trace(bad_trace)))
    report = evidence_summary_lines()
    expect("V03-003-REPORT-CATEGORIES", {line.split("  ", 1)[0] for line in report} == {"MANUAL_ARCHITECTURE_REVIEW", "CONTRACT_ONLY", "REQUIRED_LATER/NOT_EXECUTED", "BLOCKED"})
    expect("V03-003-NO-FALSE-PASS", all(not line.split("  ", 1)[0].endswith("PASS") for line in report))

    # Retained governance, puzzle and blocker regressions.
    blocker_text = (ROOT / "ARCHITECTURE/OPEN_BLOCKERS.md").read_text(encoding="utf-8")
    expect("REG-BLOCKERS", normalized_blocker_text(blocker_text) != normalized_blocker_text(blocker_text.replace("**Offen**", "**Geschlossen**", 1)))
    status_args = [
        (ROOT / "ARCHITECTURE/ARCHITECTURE.md").read_text(encoding="utf-8"),
        (ROOT / "PROJECT_CONTROL/CURRENT_STATE.md").read_text(encoding="utf-8"),
        (ROOT / "PROJECT_CONTROL/WORK_QUEUE.md").read_text(encoding="utf-8"),
        (ROOT / "WORK_PACKAGES/WP-007_CI-Setup.md").read_text(encoding="utf-8"),
    ]
    status_args[2] = status_args[2].replace("Architecture v1.0", "Architecture v0.5")
    expect("REG-STATUS", "version:work-queue" in status_consistency_errors(*status_args))
    index_text = (ROOT / "DECISIONS/README.md").read_text(encoding="utf-8")
    expect("REG-ADR-MISSING", bool(adr_index_errors(index_text.replace("ADR-026", "ADR-X26"))))
    inconsistent_level = load_json(ROOT / "ARCHITECTURE/examples/level-v2.example.json")
    inconsistent_level["content"]["season"] = 2
    expect("REG-LEVEL-ID", "level-v2:id-content" in v2_binding_errors(inconsistent_level, proof))
    bad_schema = load_json(ROOT / "ARCHITECTURE/schemas/level-v2.schema.json")
    bad_schema["properties"]["solution"]["properties"]["path"]["minItems"] = 2
    single = load_json(ROOT / "ARCHITECTURE/examples/level-v2.single-cell.example.json")
    expect("REG-SINGLE-CELL", bool(list(jsonschema.Draft202012Validator(bad_schema).iter_errors(single))))

    if failures:
        for label in failures: fail(f"self-test:not-detected:{label}")
    else:
        PASSES.append(("LOCAL_ARCHITECTURE_SEMANTICS", "Mutations-Selbsttest: vier v0.5-HIGH-Korrekturen, V03-Regressionen und Guardrails werden erkannt"))


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate Stammstrecken-Puzzle Architecture v1.0")
    parser.add_argument("--scope", choices=("documentation", "production"))
    parser.add_argument("--scope-manifest", type=Path)
    parser.add_argument("--self-test", action="store_true")
    args = parser.parse_args()
    if bool(args.scope) != bool(args.scope_manifest):
        parser.error("--scope and --scope-manifest must be provided together")
    if args.scope_manifest is not None and scope_manifest_argument_error(args.scope_manifest):
        parser.error("--scope-manifest must be a canonical repository-relative path")
    manifest_path = None if args.scope_manifest is None else ROOT / args.scope_manifest

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
        ("LOCAL_ARCHITECTURE_SEMANTICS", "Strukturierte IAP-Fixture-Zustandsmaschine", iap_check),
        ("LOCAL_ARCHITECTURE_SEMANTICS", "Ausführbarer Privacy-v2-Lifecycle", privacy_check),
        ("LOCAL_ARCHITECTURE_SEMANTICS", "RC-/Promotion- und Release-Lock-Identität", release_check),
        ("LOCAL_ARCHITECTURE_SEMANTICS", "Store-Crashrate und Rolloutentscheidung", rollout_check),
        ("LOCAL_ARCHITECTURE_SEMANTICS", "Vier Architecture-v0.5-HIGH-Verträge", review_contract_check),
        ("LOCAL_ARCHITECTURE_SEMANTICS", "Save-JCS Python/Node-Crosscheck", cross_tool_hash_check),
        ("LOCAL_DOCUMENT_STRUCTURE", "Architecture-v1.0-Status und drei Produktblocker", version_and_blocker_check),
    ]
    if args.scope and manifest_path:
        groups.append(("LOCAL_SCOPE", "Git-Diff gegen versioniertes Scope-Manifest", lambda: git_scope_check(args.scope, manifest_path)))
    for category, name, function in groups: run_group(category, name, function)
    mutation_manifest = manifest_path or ROOT / "tools/architecture-validation/scopes/WP-005.documentation.scope.json"
    if args.self_test: self_test(args.scope or "documentation", mutation_manifest)

    print(f"ARCHITECTURE VALIDATION v1.0 scope={args.scope or 'architecture-only'}")
    for category, item in PASSES: print(f"{category} PASS  {item}")
    if args.scope and manifest_path:
        head = subprocess.run(["git", "-C", str(ROOT), "rev-parse", "HEAD"], text=True, capture_output=True, check=True).stdout.strip()
        dirty = bool(subprocess.run(["git", "-C", str(ROOT), "status", "--porcelain"], text=True, capture_output=True, check=True).stdout)
        manifest = load_json(manifest_path)
        print(f"SCOPE_CONTEXT  workPackage={manifest['workPackageId']} base={manifest['baseCommit']} head={head} worktreeDirty={str(dirty).lower()}")
    for line in evidence_summary_lines(): print(line)
    if ERRORS:
        for item in ERRORS: print(f"FAIL  {item}")
        print(f"RESULT FAIL ({len(ERRORS)} Fehler)")
        return 1
    print(f"RESULT PASS ({len(PASSES)} lokale Prüfgruppen)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
