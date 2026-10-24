#!/usr/bin/env python3
"""Validate WP-036 authoring sources; never imports or approves game content."""
from __future__ import annotations

import argparse
from collections import Counter, defaultdict
import copy
import hashlib
import json
from pathlib import Path
import re
import subprocess
import sys
import unicodedata

import jsonschema

ROOT = Path(__file__).resolve().parents[2]
SOURCE_REF = "dc1652befd7545b4c2a2dd0580f35a4e8e05d72d"
MAIN_REF = "d90e798dd9b26d3f152f3b0c2d1d52cae641eb16"
SOURCE = ROOT / "Content/Localization/source/de-DE.json"
SCHEMA = ROOT / "Content/Localization/source-v1.schema.json"
REPORTS = ROOT / "Content/Reviews/text-library/WP-036"
BINDINGS = REPORTS / "source-bindings.json"
UNRESOLVED = "Content/Reviews/season-1/unresolved-references.json"
AUTHOR_REVIEW = "Content/Reviews/season-1/author-review.json"
TOKEN = re.compile(r"\{([a-z][A-Za-z0-9]*)\}")
KEY = re.compile(r"^[a-z][a-z0-9]*(?:[.-][a-z0-9]+)+$")
LEVEL_KEY = re.compile(r"^level\.s1\.\d{2}\.\d{2}\.\d{2}\.(title|status|entry|result)$")
FIELDS = ("titleKey", "statusKey", "entryDeductionKey", "resultTextKey")
FIXED_LABELS = ("STRECKE FREIGEBEN", "NÄCHSTE STÖRUNG", "BETRIEBSWERK", "Angekommen", "Anschluss erwischt", "Tatsächlich pünktlich")
KNOWN_NAMESPACES = ("ui.", "level.", "season.", "section.", "route.", "cosmetic.", "train.", "daily.", "profile.", "hint.")


def digest(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def reject_duplicates(pairs):
    result = {}
    for key, value in pairs:
        if key in result:
            raise ValueError(f"duplicate JSON property: {key}")
        result[key] = value
    return result


def parse(data: bytes):
    return json.loads(data.decode("utf-8"), object_pairs_hook=reject_duplicates)


def git(*args: str, data: bytes | None = None) -> bytes:
    result = subprocess.run(["git", "-C", str(ROOT), *args], input=data, capture_output=True)
    if result.returncode:
        raise ValueError(f"git {' '.join(args[:2])}: {result.stderr.decode('utf-8', errors='replace').strip()}")
    return result.stdout


def tree_paths(ref: str) -> list[str]:
    return git("ls-tree", "-r", "--name-only", ref).decode("utf-8").splitlines()


def git_objects(ref: str, paths: list[str]) -> dict[str, bytes]:
    """Read blobs in one process, preserving their exact Git bytes."""
    if not paths:
        return {}
    payload = "".join(f"{ref}:{path}\n" for path in paths).encode("utf-8")
    output = git("cat-file", "--batch", data=payload)
    cursor = 0
    objects = {}
    for path in paths:
        end = output.find(b"\n", cursor)
        header = output[cursor:end].decode("ascii")
        pieces = header.split()
        if len(pieces) != 3 or pieces[1] != "blob":
            raise ValueError(f"missing/non-blob Git source {ref}:{path}: {header}")
        size = int(pieces[2])
        cursor = end + 1
        objects[path] = output[cursor:cursor + size]
        cursor += size
        if output[cursor:cursor + 1] != b"\n":
            raise ValueError(f"invalid cat-file framing: {path}")
        cursor += 1
    if cursor != len(output):
        raise ValueError("unexpected trailing cat-file bytes")
    return objects


def level_layout():
    result = []
    for section in range(1, 6):
        for route in range(1, 5):
            for position in range(1, 13):
                puzzle_id = f"S1-{section:02}-{route:02}-{position:02}"
                result.append((puzzle_id, f"Content/Levels/s1/{section:02}/{route:02}/{puzzle_id}.level-v2.json", section, route, position))
    return result


def extract_quote(review: str, puzzle_id: str) -> str:
    match = re.search(rf"^## {re.escape(puzzle_id)}\s*\n(.*?)(?=^## |\Z)", review, re.MULTILINE | re.DOTALL)
    if not match:
        raise ValueError(f"review block missing: {puzzle_id}")
    entry = re.search(r"^- Einstieg: (.*?) Belegschritte:", match.group(1), re.MULTILINE)
    if not entry:
        raise ValueError(f"entry sentence missing: {puzzle_id}")
    return entry.group(1)


def endpoint_cell(level: dict, name: str) -> tuple[int, int]:
    endpoint = level["endpoints"][name]
    side, index = endpoint["side"], endpoint["index"]
    width, height = level["grid"]["width"], level["grid"]["height"]
    return {"N": (index, 0), "S": (index, height - 1), "W": (0, index), "E": (width - 1, index)}[side]


def fact_errors(level: dict, fact: dict) -> list[str]:
    """Check the explicitly supplied elementary anchor, not the full prose."""
    errors = []
    if not isinstance(fact, dict):
        return ["missing-mathematical-fact"]
    kind = fact.get("kind")
    if kind in {"FULL_ROW", "ZERO_ROW", "FULL_COLUMN", "ZERO_COLUMN"}:
        axis = "ROW" if kind.endswith("ROW") else "COLUMN"
        conclusion = "FULL" if kind.startswith("FULL") else "EMPTY"
        fact = dict(fact, kind="LINE_COUNT", axis=axis, conclusion=conclusion)
        kind = "LINE_COUNT"
    if kind == "LINE_COUNT":
        axis, index = fact.get("axis"), fact.get("index")
        if axis not in {"ROW", "COLUMN"} or type(index) is not int:
            return ["invalid-line-fact"]
        counts = level["rowCounts" if axis == "ROW" else "columnCounts"]
        extent = level["grid"]["width" if axis == "ROW" else "height"]
        if not 0 <= index < len(counts):
            return ["line-fact-index"]
        count = counts[index]
        if "count" in fact and fact["count"] != count:
            errors.append("line-fact-count")
        conclusion = fact.get("conclusion")
        if conclusion == "FULL" and count != extent:
            errors.append("line-fact-not-full")
        elif conclusion == "EMPTY" and count != 0:
            errors.append("line-fact-not-empty")
        elif conclusion not in {"FULL", "EMPTY"}:
            errors.append("line-fact-conclusion")
    elif kind in {"ENDPOINT", "ENDPOINT_LINE"}:
        endpoint = fact.get("endpoint")
        if endpoint not in {"a", "b"}:
            return ["endpoint-fact-name"]
        x, y = endpoint_cell(level, endpoint)
        if (x, y) not in {(cell["x"], cell["y"]) for cell in level["solution"]["path"]}:
            errors.append("endpoint-not-in-bound-authoring-solution")
        if kind == "ENDPOINT_LINE":
            axis, index = fact.get("axis"), fact.get("index")
            if axis not in {"ROW", "COLUMN"} or index != (y if axis == "ROW" else x):
                return errors + ["endpoint-line-coordinate"]
            count = level["rowCounts" if axis == "ROW" else "columnCounts"][index]
            if fact.get("count") != count:
                errors.append("endpoint-line-count")
            conclusion = fact.get("conclusion")
            if conclusion == "REST_EMPTY" and count != 1:
                errors.append("endpoint-line-not-singleton")
            elif conclusion not in {"REST_EMPTY", "OCCUPIED_ENDPOINT"}:
                errors.append("endpoint-line-conclusion")
    else:
        errors.append("unknown-mathematical-fact-kind")
    return errors


def placeholders(text: str) -> tuple[set[str], bool]:
    names = set(TOKEN.findall(text))
    remainder = TOKEN.sub("", text)
    return names, "{" not in remainder and "}" not in remainder


def validate(source, bindings, schema, objects, paths) -> tuple[list[str], dict]:
    errors = []
    schema_errors = sorted(jsonschema.Draft202012Validator(schema).iter_errors(source), key=lambda item: str(list(item.path)))
    for error in schema_errors:
        errors.append(f"schema:{'/'.join(map(str, error.path))}:{error.message}")
    if schema_errors:
        return errors, {}
    entries = source["entries"]
    by_key = {}
    for entry in entries:
        key = entry["key"]
        if key in by_key:
            errors.append(f"duplicate-key:{key}")
        by_key[key] = entry
        for name in ("text", "description", "shortText"):
            if name not in entry:
                continue
            text = entry[name]
            if text != unicodedata.normalize("NFC", text):
                errors.append(f"not-nfc:{key}:{name}")
            if not text.strip():
                errors.append(f"blank:{key}:{name}")
            if any(unicodedata.category(char) in {"Cs", "Cc"} and char != "\n" for char in text) or "\ufffd" in text:
                errors.append(f"invalid-unicode:{key}:{name}")
            if name != "description":
                names, valid = placeholders(text)
                if not valid or names != set(entry["placeholders"]):
                    errors.append(f"placeholder-set:{key}:{name}")
                if len(text) > entry["charLimit"]:
                    errors.append(f"over-limit:{key}:{name}:{len(text)}>{entry['charLimit']}")
        if key.endswith(".entry") and entry["audience"] != "INTERNAL":
            errors.append(f"internal-entry-visible:{key}")
    labels = {entry["text"].rstrip(".") for entry in entries if entry["audience"] == "PLAYER"}
    for label in FIXED_LABELS:
        if label not in labels:
            errors.append(f"fixed-label-missing:{label}")
    if not isinstance(bindings, dict) or bindings.get("sourceRef") != SOURCE_REF:
        errors.append("bindings:pinned-source-ref")
        return errors, {}
    bound_levels = bindings.get("levels", [])
    if not isinstance(bound_levels, list) or any(not isinstance(item, dict) for item in bound_levels):
        return errors + ["bindings:levels-must-be-record-array"], {}
    layout = level_layout()
    author_entries = {}
    author_raw = objects.get(AUTHOR_REVIEW)
    if author_raw is None:
        errors.append("author-review:missing-pinned-object")
    else:
        if bindings.get("authorReviewPath") != AUTHOR_REVIEW or bindings.get("authorReviewSha256") != digest(author_raw):
            errors.append("bindings:author-review-byte-binding")
        author_review = parse(author_raw)
        author_levels = author_review.get("levels", []) if isinstance(author_review, dict) else []
        if not isinstance(author_levels, list) or any(not isinstance(item, dict) for item in author_levels):
            errors.append("author-review:levels-must-be-record-array")
        else:
            author_ids = [item.get("puzzleId") for item in author_levels]
            if author_ids != [item[0] for item in layout]:
                errors.append("author-review:240-ordered-identities")
            if any(not isinstance(puzzle_id, str) for puzzle_id in author_ids):
                errors.append("author-review:invalid-puzzle-id")
            for puzzle_id, count in Counter(item for item in author_ids if isinstance(item, str)).items():
                if count != 1:
                    errors.append(f"author-review:duplicate-puzzle-id:{puzzle_id}")
            author_entries = {item.get("puzzleId"): item for item in author_levels if isinstance(item.get("puzzleId"), str)}
    expected_paths = [item[1] for item in layout]
    actual_paths = sorted(path for path in paths if re.fullmatch(r"Content/Levels/s1/\d{2}/\d{2}/S1-\d{2}-\d{2}-\d{2}\.level-v2\.json", path))
    if actual_paths != expected_paths:
        errors.append("source:240-exact-level-paths")
    if [item.get("puzzleId") for item in bound_levels] != [item[0] for item in layout]:
        errors.append("bindings:240-ordered-identities")
    if len(bound_levels) != 240:
        errors.append("bindings:240-count")
    expected_keys = set()
    ordered_expected_keys = []
    expected_pairs = set()
    checked_facts = []
    for index, (puzzle_id, path, section, route, position) in enumerate(layout):
        if index >= len(bound_levels):
            break
        binding = bound_levels[index]
        prefix = f"binding:{puzzle_id}"
        try:
            raw = objects[path]
            level = parse(raw)
            if binding.get("path") != path or binding.get("sha256") != digest(raw) or level["puzzleId"] != puzzle_id:
                errors.append(f"{prefix}:level-byte-binding")
            content = level["content"]
            if [content["season"], content["networkSection"], content["route"], content["position"]] != [1, section, route, position]:
                errors.append(f"{prefix}:hierarchy")
            actual_keys = {"titleKey": content["titleKey"], "statusKey": content["statusKey"], "entryDeductionKey": level["production"]["entryDeductionKey"], "resultTextKey": level["completion"]["resultTextKey"]}
            for field, key in actual_keys.items():
                expected_keys.add(key)
                ordered_expected_keys.append(key)
                expected_pairs.add((puzzle_id, key))
                if binding.get(field) != key:
                    errors.append(f"{prefix}:key-binding:{field}")
                entry = by_key.get(key)
                if entry is None:
                    errors.append(f"missing-level-key:{puzzle_id}:{key}")
                elif entry["audience"] != ("INTERNAL" if field == "entryDeductionKey" else "PLAYER"):
                    errors.append(f"{prefix}:audience:{field}")
            review_path = f"Content/Reviews/season-1/leveldesign-S1-{section:02}-{route:02}.md"
            review_raw = objects[review_path]
            if binding.get("reviewPath") != review_path or binding.get("reviewSha256") != digest(review_raw):
                errors.append(f"{prefix}:review-byte-binding")
            quote = extract_quote(review_raw.decode("utf-8"), puzzle_id)
            if binding.get("entrySourceQuote") != quote:
                errors.append(f"{prefix}:entry-source-quote")
            if author_entries.get(puzzle_id, {}).get("entryDeduction") != quote:
                errors.append(f"author-review:{puzzle_id}:entry-quote-mismatch")
            entry_fact = binding.get("entryFact", {})
            if not isinstance(entry_fact, dict) or entry_fact.get("kind") != "SOURCE_REVIEW_QUOTE" or ("quote" in entry_fact and entry_fact["quote"] != quote):
                errors.append(f"{prefix}:entry-fact-source")
            entry = by_key.get(actual_keys["entryDeductionKey"])
            if entry and binding.get("entryTextSha256") != digest(entry["text"].encode("utf-8")):
                errors.append(f"{prefix}:entry-text-hash")
            facts = {field: level[field] for field in ("grid", "endpoints", "rowCounts", "columnCounts")}
            if binding.get("publicFacts") != facts:
                errors.append(f"{prefix}:public-facts")
            fact = binding.get("mathematicalFact")
            for error in fact_errors(level, fact):
                errors.append(f"{prefix}:{error}")
            checked_facts.append({"puzzleId": puzzle_id, "mathematicalFact": fact, "sourceQuote": quote})
        except (KeyError, TypeError, ValueError, IndexError) as error:
            errors.append(f"{prefix}:malformed:{error}")
    actual_level_keys = {key for key in by_key if key.startswith("level.s1.")}
    if actual_level_keys != expected_keys:
        errors.append(f"level-key-set:missing={len(expected_keys - actual_level_keys)}:extra={len(actual_level_keys - expected_keys)}")
    if [entry["key"] for entry in entries if entry["key"].startswith("level.s1.")] != ordered_expected_keys:
        errors.append("level-key-order:expected-title-status-entry-result-per-level-in-campaign-order")
    unresolved = parse(objects[UNRESOLVED])
    references = unresolved.get("entries", [])
    text_refs = [item for item in references if item.get("owner") == "Localization"]
    foreign_refs = [item for item in references if item.get("owner") == "Completion/World"]
    if len(references) != 1440 or unresolved.get("unresolvedCount") != 1440 or len(text_refs) != 960 or len(foreign_refs) != 480:
        errors.append("source:1440-reference-inventory")
    if {(item["puzzleId"], item["reference"]) for item in text_refs} != expected_pairs:
        errors.append("source:text-reference-set")
    for item in foreign_refs:
        if item["reference"] in by_key:
            errors.append(f"foreign-target-created-as-text:{item['reference']}")
    checked_quotes = {item["puzzleId"]: item["sourceQuote"] for item in checked_facts}
    report = {
        "schemaVersion": 1,
        "sourceRef": SOURCE_REF,
        "authorReviewPath": AUTHOR_REVIEW,
        "authorReviewSha256": digest(author_raw) if author_raw is not None else None,
        "authorEntryQuoteAgreements": sum(puzzle_id in checked_quotes and item.get("entryDeduction") == checked_quotes[puzzle_id] for puzzle_id, item in author_entries.items()),
        "sourceCount": len(entries),
        "levels": 240,
        "textReferencesExpected": 960,
        "textReferencesCovered": sum(key in by_key for key in expected_keys),
        "levelPlayerEntries": sum(key in by_key and by_key[key]["audience"] == "PLAYER" for key in expected_keys),
        "levelInternalEntries": sum(key in by_key and by_key[key]["audience"] == "INTERNAL" for key in expected_keys),
        "foreignReferencesUnresolved": len(foreign_refs),
        "availabilityCounts": dict(sorted(Counter(entry["availability"] for entry in entries).items())),
        "audienceCounts": dict(sorted(Counter(entry["audience"] for entry in entries).items())),
        "status": "DRAFT",
        "fullCrossReferenceGate": "NOT_PASSED_BY_THIS_CHECK",
        "runtimeImport": "NOT_IMPLEMENTED_OR_APPROVED_BY_THIS_CHECK",
        "creativeProductLegalReleaseApproval": "NOT_GRANTED",
        "deductionCheckBoundary": "Exact pinned author-review/route-review agreement and level/text-byte provenance plus one independently checked elementary public-input anchor per level. This does not prove every paraphrased sentence, re-run solver validation, or replace editorial review.",
        "lengthBoundary": "Unicode code points of source strings, including literal placeholder tokens; proposed limits are not rendered-width or substituted-value/layout/accessibility approval.",
        "checkedEntryAnchors": checked_facts,
        "errors": errors,
        "result": "PASS" if not errors else "FAIL",
    }
    return errors, report


def repeat_report(entries):
    exact = defaultdict(list)
    sentences = defaultdict(set)
    phrases = defaultdict(set)
    for entry in entries:
        if entry["audience"] != "PLAYER":
            continue
        key, text = entry["key"], entry["text"]
        exact[text.casefold().strip()].append(key)
        for sentence in re.split(r"(?<=[.!?])\s+", text):
            if len(sentence) >= 15:
                sentences[sentence.casefold()].add(key)
        words = re.findall(r"\b[\wÄÖÜäöüß]+\b", text.casefold())
        for index in range(len(words) - 4):
            phrases[" ".join(words[index:index + 5])].add(key)
    return {
        "method": "PLAYER source strings, casefolded exact strings/sentences and five-word windows. Findings require editorial judgement; deliberate labels/refrains are not automatically failures.",
        "exactRepeatedTexts": [{"text": text, "keys": keys} for text, keys in sorted(exact.items()) if len(keys) > 1],
        "repeatedSentences": [{"sentence": text, "keys": sorted(keys)} for text, keys in sorted(sentences.items()) if len(keys) > 1],
        "repeatedFiveWordPhrases": [{"phrase": phrase, "keys": sorted(keys)} for phrase, keys in sorted(phrases.items()) if len(keys) >= 3],
    }


def pseudolocale(entries):
    mapping = str.maketrans("aAeEiIoOuUyY", "áÁëËïÏôÔüÜýÝ")
    output = []
    for entry in entries:
        if entry["audience"] != "PLAYER":
            continue
        parts = re.split(r"(\{[a-z][A-Za-z0-9]*\})", entry["text"])
        expanded = "".join(part if TOKEN.fullmatch(part) else part.translate(mapping) for part in parts)
        expansion = max(1, (len(TOKEN.sub("", entry["text"])) * 35 + 99) // 100)
        text = f"⟦{expanded}{'~' * expansion}⟧"
        output.append({"key": entry["key"], "text": text, "sourceCodePoints": len(entry["text"]), "pseudoCodePoints": len(text), "sourceProposedLimit": entry["charLimit"]})
    return {"locale": "qps-ploc", "status": "LAYOUT_TEST_MATERIAL_ONLY", "method": "Accent Latin vowels, preserve placeholder tokens verbatim, append 35% non-placeholder padding and visible delimiters. No Unity or device layout test performed.", "entries": output}


def inventory(objects, entries):
    found = defaultdict(list)
    def visit(value, path, pointer=""):
        if isinstance(value, dict):
            for field, child in value.items():
                child_pointer = pointer + "/" + field
                if field.endswith("Key") and isinstance(child, str) and KEY.fullmatch(child):
                    found[child].append({"path": path, "pointer": child_pointer})
                visit(child, path, child_pointer)
        elif isinstance(value, list):
            for index, child in enumerate(value):
                visit(child, path, pointer + f"/{index}")
    for path, raw in sorted(objects.items()):
        try:
            if path.endswith(".json"):
                visit(parse(raw), path)
            elif path.endswith((".cs", ".uxml")):
                text = raw.decode("utf-8")
                for candidate in re.findall(r'["\']([a-z][a-z0-9.-]+)["\']', text):
                    if candidate.startswith(KNOWN_NAMESPACES) and KEY.fullmatch(candidate):
                        found[candidate].append({"path": path, "pointer": "quoted-key-candidate"})
        except (UnicodeError, ValueError):
            continue
    existing = {entry["key"] for entry in entries}
    values = []
    for key, locations in sorted(found.items()):
        fixture_only = all(item["path"].startswith(("ARCHITECTURE/examples/", "tools/architecture-validation/fixtures/")) or "/Tests/" in item["path"] for item in locations)
        values.append({"key": key, "covered": key in existing, "classification": "FIXTURE_OR_TEST_ONLY" if fixture_only else "PRODUCTION_REFERENCE", "locations": locations})
    return {"baseRef": MAIN_REF, "method": "Tracked base JSON fields ending Key plus quoted key candidates in C#/UXML. Fixture/test references are inventoried separately and do not require fictional production translations.", "references": values, "missingFixtureKeys": [item["key"] for item in values if not item["covered"] and item["classification"] == "FIXTURE_OR_TEST_ONLY"], "missingProductionKeys": [item["key"] for item in values if not item["covered"] and item["classification"] == "PRODUCTION_REFERENCE"]}


def mutation_tests(source, bindings, schema, objects, paths):
    outcomes = []
    first_entry_key = next(entry["key"] for entry in source["entries"] if entry["key"].endswith(".entry"))
    tests = ("missing-key", "duplicate-key", "illegal-placeholder", "over-limit", "wrong-binding", "player-entry", "false-mathematical-anchor", "wrong-quote", "wrong-typed-placeholder", "non-nfc", "author-hash", "author-quote")
    for test in tests:
        changed = copy.deepcopy(source)
        changed_bindings = copy.deepcopy(bindings)
        changed_objects = objects
        expected_diagnostic = None
        if test == "missing-key":
            changed["entries"] = [entry for entry in changed["entries"] if entry["key"] != first_entry_key]
        elif test == "duplicate-key":
            changed["entries"].append(copy.deepcopy(changed["entries"][0]))
        elif test == "illegal-placeholder":
            changed["entries"][0]["text"] += " {unauthorized}"
            changed["entries"][0]["charLimit"] += 100
        elif test == "over-limit":
            changed["entries"][0]["charLimit"] = 1
        elif test == "wrong-binding":
            changed_bindings["levels"][0]["sha256"] = "0" * 64
        elif test == "player-entry":
            next(entry for entry in changed["entries"] if entry["key"] == first_entry_key)["audience"] = "PLAYER"
        elif test == "false-mathematical-anchor":
            changed_bindings["levels"][0]["mathematicalFact"] = {"kind": "LINE_COUNT", "axis": "ROW", "index": 0, "count": -1, "conclusion": "EMPTY"}
        elif test == "wrong-quote":
            changed_bindings["levels"][0]["entrySourceQuote"] = "Nicht belegte Aussage."
        elif test == "wrong-typed-placeholder":
            changed["entries"][0]["placeholders"] = {"points": "FLOAT"}
        elif test == "non-nfc":
            changed["entries"][0]["text"] = "u\u0308"
        elif test == "author-hash":
            changed_bindings["authorReviewSha256"] = "0" * 64
            expected_diagnostic = "bindings:author-review-byte-binding"
        elif test == "author-quote":
            author = parse(objects[AUTHOR_REVIEW])
            author["levels"][0]["entryDeduction"] = "Unbelegte geänderte Autorenaussage."
            changed_objects = dict(objects)
            changed_objects[AUTHOR_REVIEW] = json.dumps(author, ensure_ascii=False).encode("utf-8")
            changed_bindings["authorReviewSha256"] = digest(changed_objects[AUTHOR_REVIEW])
            expected_diagnostic = f"author-review:{author['levels'][0]['puzzleId']}:entry-quote-mismatch"
        errors, _ = validate(changed, changed_bindings, schema, changed_objects, paths)
        detected = expected_diagnostic in errors if expected_diagnostic else bool(errors)
        outcomes.append({"mutation": test, "detected": detected, "expectedDiagnostic": expected_diagnostic, "firstDiagnostic": errors[0] if errors else None})
    return outcomes


def write_json(path, value):
    path.write_text(json.dumps(value, ensure_ascii=False, indent=2) + "\n", encoding="utf-8", newline="\n")


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source", type=Path, default=SOURCE)
    parser.add_argument("--bindings", type=Path, default=BINDINGS)
    parser.add_argument("--self-test", action="store_true")
    parser.add_argument("--report-dir", type=Path)
    parser.add_argument("--write-reports", action="store_true")
    args = parser.parse_args()
    try:
        source_raw, binding_raw = args.source.read_bytes(), args.bindings.read_bytes()
        source, bindings, schema = parse(source_raw), parse(binding_raw), parse(SCHEMA.read_bytes())
        jsonschema.Draft202012Validator.check_schema(schema)
        paths = tree_paths(SOURCE_REF)
        selected = [path for path in paths if path.startswith("Content/Levels/s1/") and path.endswith(".level-v2.json") or re.fullmatch(r"Content/Reviews/season-1/leveldesign-S1-\d{2}-\d{2}\.md", path) or path in {UNRESOLVED, AUTHOR_REVIEW}]
        objects = git_objects(SOURCE_REF, selected)
        errors, report = validate(source, bindings, schema, objects, paths)
        base_paths = tree_paths(MAIN_REF)
        inventory_paths = [path for path in base_paths if path.endswith((".json", ".cs", ".uxml")) and path.startswith(("Content/", "Assets/", "ARCHITECTURE/examples/", "tools/architecture-validation/fixtures/"))]
        key_inventory = inventory(git_objects(MAIN_REF, inventory_paths), source["entries"])
        for key in key_inventory["missingProductionKeys"]:
            errors.append(f"existing-production-key-missing:{key}")
        mutations = mutation_tests(source, bindings, schema, objects, paths) if args.self_test and not errors else []
        for mutation in mutations:
            if not mutation["detected"]:
                errors.append(f"undetected-mutation:{mutation['mutation']}")
        report.update({"sourceSha256": digest(source_raw), "sourceBindingsSha256": digest(binding_raw), "schemaSha256": digest(SCHEMA.read_bytes()), "selfTestExecuted": bool(mutations), "mutations": mutations, "errors": errors, "result": "PASS" if not errors else "FAIL"})
        report_dir = args.report_dir or (REPORTS if args.write_reports else None)
        if report_dir:
            report_dir = report_dir.resolve()
            report_dir.relative_to(REPORTS.resolve())
            report_dir.mkdir(parents=True, exist_ok=True)
            write_json(report_dir / "technical-validation.json", report)
            write_json(report_dir / "repetition-report.json", repeat_report(source["entries"]))
            write_json(report_dir / "pseudolocale.json", pseudolocale(source["entries"]))
            write_json(report_dir / "existing-key-inventory.json", key_inventory)
        print(f"TEXT_LIBRARY {report['result']}: entries={len(source['entries'])}; season1={report.get('textReferencesCovered', 0)}/960; internal={report.get('levelInternalEntries', 0)}; foreign-unresolved={report.get('foreignReferencesUnresolved', 480)}; mutations={len(mutations)}")
        for error in errors:
            print("ERROR " + error)
        print("BOUNDARY DRAFT authoring/provenance/input-anchor checks only; no runtime, whole-paraphrase proof, creative/product/legal or release approval.")
        return 1 if errors else 0
    except (ValueError, OSError, KeyError, TypeError, jsonschema.SchemaError) as error:
        print(f"TEXT_LIBRARY FAIL: {type(error).__name__}: {error}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
