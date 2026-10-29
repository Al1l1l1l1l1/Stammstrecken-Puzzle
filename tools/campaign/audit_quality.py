#!/usr/bin/env python3
"""Audit actual authoring inputs and schedule P02 human sampling without inventing play times."""
from __future__ import annotations
import hashlib
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]


def main() -> None:
    destination = ROOT / "Content/Quality/s1/difficulty-and-human-sample.json"
    if destination.exists():
        previous = json.loads(destination.read_text(encoding="utf-8"))
        if previous.get("starTimesReleased") or any(e.get("firstContactTimesSeconds") or e.get("humanStatus") != "NOT_EXECUTED" for e in previous.get("entries", [])):
            raise ValueError("Existing human evidence requires explicit source reconciliation; refuse to replace it with an unmeasured audit.")
    entries = []
    for file in sorted((ROOT / "Content/Levels/s1").rglob("*.level-v2.json")):
        level = json.loads(file.read_text(encoding="utf-8"))
        puzzle_id = level["puzzleId"]
        proof_path = ROOT / "Content" / "Proofs" / puzzle_id / "solver-v2.proof-v1.json"
        proof = json.loads(proof_path.read_text(encoding="utf-8"))
        width, height = level["grid"]["width"], level["grid"]["height"]
        rows, cols = level["rowCounts"], level["columnCounts"]
        if proof["puzzleId"] != puzzle_id or proof["proofHash"] != level["proofRef"]["proofHash"]:
            raise ValueError("Source/proof binding mismatch: " + puzzle_id)
        section, route, position = (int(s) for s in puzzle_id.split("-")[1:])
        targeted = section == 5 or (width == 10 and height == 10) or (section >= 4 and position >= 11)
        human_sample = position in (1, 12) or (width == 10 and height == 10) or level["contentRevision"] >= 3
        metrics = proof["metrics"]
        ten_screen = width != 10 or height != 10 or (metrics["deductionSteps"] >= 210 and metrics["maxDeductionDepth"] >= 23 and metrics["requiredGuessDepth"] == 0)
        entries.append({"puzzleId": puzzle_id, "width": width, "height": height,
                        "authoringPath": file.relative_to(ROOT).as_posix(),
                        "sourceSha256": hashlib.sha256(file.read_bytes()).hexdigest(),
                        "metrics": metrics, "trivialLines": sum(n <= 1 or n == width for n in rows) + sum(n <= 1 or n == height for n in cols),
                        "occupiedCells": sum(rows), "targetedLateReview": targeted,
                        "tenByTenTechnicalPrescreen": ten_screen, "humanSampleRequired": human_sample,
                        "humanStatus": "NOT_EXECUTED", "firstContactTimesSeconds": [],
                        "timeClassStatus": "UNCALIBRATED", "ownerNamedCase": puzzle_id == "S1-05-04-12"})
    if len(entries) != 240 or sum(x["puzzleId"].startswith("S1-05-") for x in entries) != 48:
        raise ValueError("Campaign coverage is not complete")
    report = {"schemaVersion": 1, "policy": "MANUS-2026-10-20-02-P02", "approvalStatus": "DRAFT",
              "levelCount": len(entries), "targetedLateCount": sum(x["targetedLateReview"] for x in entries),
              "initialHumanSampleCount": sum(x["humanSampleRequired"] for x in entries),
              "humanAcceptance": "NOT_EXECUTED", "starTimesReleased": False, "entries": entries}
    destination.parent.mkdir(parents=True, exist_ok=True)
    destination.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8", newline="\n")
    print(f"PASS: {len(entries)} bound inputs, {report['targetedLateCount']} targeted late tasks, {report['initialHumanSampleCount']} initial human cases; no human/time approval asserted.")


if __name__ == "__main__":
    main()
