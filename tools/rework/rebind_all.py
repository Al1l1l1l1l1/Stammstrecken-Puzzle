#!/usr/bin/env python3
"""Bindet alle abgeleiteten Dateien und Hash-Ketten in einem Schritt neu.

Aufruf im Repository-Root:   python3 tools/rework/rebind_all.py
Nur prüfen (nichts schreiben): python3 tools/rework/rebind_all.py --check

Wann ausführen: einmal am Ende eines Arbeitsblocks, nachdem Texte, UI-Dateien,
Präsentationscode (.cs unter Scripts/Presentation/UI oder /World), Art-Dateien
(png/svg/ttf/otf) oder Kataloge geändert wurden. Nicht nach jeder Kleinigkeit.

Was das Skript tut (Reihenfolge ist wichtig):
 1. Kampagnentexte: Assets/.../UI/ProductionCampaignText.json aus
    Content/Localization/source/de-DE.json neu erzeugen (title/status/result,
    sourceSha256 = Roh-SHA256 der Quelle).
 2. Leseausgabe: tools/text-library/render.py (READING_EDITION.md).
 3. Mobile-Kandidat: node tools/campaign/project_candidate.cjs (+ --check).
 4. Endlos-Stichprobe: sourceFiles[].sha256 in sample-audit.internal.json.
 5. Human-Packs + UI-Lock: node tools/campaign/project_human_assignments.cjs
    in ein frisches Temp-Verzeichnis, danach 17 Packs, raw-sha256-lock.json und
    ui-source-lock.json nach Assets/.../UI/HumanAssignments/ kopieren und
    internal/source-bindings.json nach Content/Quality/human/.
 6. Feste Zahlen in Tests: Dateianzahl des UI-Locks (z. B. 79) und den
    Public-Katalog-Hash in Tests und in der QA-Szene nachziehen.

Die Test-Fixtures unter Tests/PlayMode/Bootstrap/RuntimeFixtures/ sind davon
unabhängig und werden NICHT angefasst.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import re
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
UI = ROOT / "Assets/StammstreckenPuzzle/UI"
SOURCE = ROOT / "Content/Localization/source/de-DE.json"
CAMPAIGN_TEXT = UI / "ProductionCampaignText.json"
SAMPLE_AUDIT = ROOT / "Content/Quality/endless/p05-structure-rework-final-v2/sample-audit.internal.json"
HUMAN_DIR = UI / "HumanAssignments"
HUMAN_BINDINGS = ROOT / "Content/Quality/human/assignment-source-bindings.json"
BUILD_ID = "stp-3d-rework-unbuilt-v1"
STUDY_ID = "wp046-human"

COUNT_RULES = [
    (ROOT / "Assets/StammstreckenPuzzle/Tests/EditMode/Content/HumanPackSceneBindingTests.cs",
     r"(ui\.files\.Length,Is\.EqualTo\()(\d+)(\))"),
    (ROOT / "tools/campaign/ProductionImport/UiTransfer.cs",
     r"(oldFiles\.Count==)(\d+)(,\"IMPORT_UI_METADATA_TRANSFER_REQUIRES_COMPLETE_)\d+(_SOURCE_SET\")"),
    (ROOT / "tools/campaign/HumanReadback/SelfTests.cs",
     r"(\"files\"\)\.Count==)(\d+)(,\"Actual)\d+(fileUIset\")"),
    (ROOT / "tools/campaign/tests/project_human_assignments.test.cjs",
     r"(files\.length,)(\d+)(\))"),
]
PUBLIC_HASH_RULES = [
    (ROOT / "Assets/StammstreckenPuzzle/Tests/EditMode/Content/HumanPackSceneBindingTests.cs",
     r'(CurrentPublicHash=")([0-9a-f]{64})(")'),
    (ROOT / "Assets/StammstreckenPuzzle/Scenes/QA/HumanCalibration.unity",
     r"(publicCatalogHashSha256: )([0-9a-f]{64})()"),
]

changed: list[str] = []


def sha(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def put(path: Path, data: bytes, check: bool) -> None:
    old = path.read_bytes() if path.exists() else None
    if old == data:
        return
    changed.append(str(path.relative_to(ROOT)))
    if not check:
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_bytes(data)


def run(cmd: list[str]) -> str:
    result = subprocess.run(cmd, cwd=ROOT, text=True, capture_output=True)
    if result.returncode != 0:
        sys.stderr.write(result.stdout + result.stderr)
        raise SystemExit(f"FEHLER in: {' '.join(cmd)}")
    return result.stdout


def step_campaign_text(check: bool) -> None:
    raw = SOURCE.read_bytes()
    source = json.loads(raw)
    texts = {entry["key"]: entry["text"] for entry in source["entries"]}
    current = json.loads(CAMPAIGN_TEXT.read_text(encoding="utf-8"))
    for entry in current["entries"]:
        stem = "level.s1." + entry["puzzleId"][3:].replace("-", ".")
        for field in ("title", "status", "result"):
            key = f"{stem}.{field}"
            if key not in texts:
                raise SystemExit(f"FEHLER: Schlüssel fehlt in de-DE.json: {key}")
            entry[field] = texts[key]
    current["sourceSha256"] = sha(raw)
    data = (json.dumps(current, indent=2, ensure_ascii=False) + "\n").encode("utf-8")
    put(CAMPAIGN_TEXT, data, check)


def step_reading_edition(check: bool) -> None:
    if check:
        result = subprocess.run([sys.executable, "tools/text-library/render.py", "--check"], cwd=ROOT, text=True, capture_output=True)
        if result.returncode != 0:
            changed.append("Content/Reviews/text-library/WP-036/READING_EDITION.md")
    else:
        run([sys.executable, "tools/text-library/render.py"])


def step_candidate(check: bool) -> None:
    if check:
        result = subprocess.run(["node", "tools/campaign/project_candidate.cjs", "--check"], cwd=ROOT, text=True, capture_output=True)
        if result.returncode != 0:
            changed.append("Assets/StammstreckenPuzzle/UI/MobileCandidate.* (project_candidate --check rot)")
        return
    run(["node", "tools/campaign/project_candidate.cjs"])
    run(["node", "tools/campaign/project_candidate.cjs", "--check"])


def step_sample_audit(check: bool) -> None:
    audit = json.loads(SAMPLE_AUDIT.read_text(encoding="utf-8"))
    for item in audit["sourceFiles"]:
        item["sha256"] = sha((ROOT / item["path"]).read_bytes())
    data = (json.dumps(audit, indent=2, ensure_ascii=False) + "\n").encode("utf-8")
    put(SAMPLE_AUDIT, data, check)


def step_human(check: bool) -> int:
    with tempfile.TemporaryDirectory() as temp:
        out = Path(temp) / "hp"
        run(["node", "tools/campaign/project_human_assignments.cjs", "--repo-root", ".", "--out", str(out),
             "--build-id", BUILD_ID, "--study-id", STUDY_ID, "--endless-sample", str(SAMPLE_AUDIT.relative_to(ROOT))])
        for file in sorted((out / "participant").glob("*.json")):
            put(HUMAN_DIR / file.name, file.read_bytes(), check)
        put(HUMAN_BINDINGS, (out / "internal/source-bindings.json").read_bytes(), check)
        ui_lock = json.loads((out / "participant/ui-source-lock.json").read_text(encoding="utf-8"))
        return len(ui_lock["files"])


def replace_rules(rules, value: str, check: bool) -> None:
    for path, pattern in rules:
        text = path.read_text(encoding="utf-8")
        if not re.search(pattern, text):
            raise SystemExit(f"FEHLER: Muster nicht gefunden in {path.relative_to(ROOT)}: {pattern}")
        def swap(match: re.Match[str]) -> str:
            groups = match.groups()
            if len(groups) == 4:
                return f"{groups[0]}{value}{groups[2]}{value}{groups[3]}"
            return f"{groups[0]}{value}{groups[2]}"
        put(path, re.sub(pattern, swap, text).encode("utf-8"), check)


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--check", action="store_true", help="nur prüfen, nichts schreiben; Exit 1 bei Abweichung")
    args = parser.parse_args()
    if not (ROOT / "AGENTS.md").is_file():
        raise SystemExit("Bitte im Repository ausführen.")
    step_campaign_text(args.check)
    if args.check and changed:
        # Folgeschritte rechnen mit den Repo-Dateien; Abweichung reicht als Befund.
        pass
    step_reading_edition(args.check)
    step_candidate(args.check)
    step_sample_audit(args.check)
    count = step_human(args.check)
    replace_rules(COUNT_RULES, str(count), args.check)
    bindings = json.loads((UI / "MobileCandidate.bindings.json").read_text(encoding="utf-8"))
    replace_rules(PUBLIC_HASH_RULES, bindings["publicCatalogHashSha256"], args.check)
    if changed:
        label = "VERALTET (bitte ohne --check ausführen)" if args.check else "NEU GEBUNDEN"
        print(f"{label}: {len(changed)} Datei(en)")
        for item in changed:
            print("  " + item)
        if args.check:
            raise SystemExit(1)
    else:
        print(f"PASS: alle abgeleiteten Dateien und Hash-Ketten aktuell (UI-Lock {count} Dateien).")


if __name__ == "__main__":
    main()
