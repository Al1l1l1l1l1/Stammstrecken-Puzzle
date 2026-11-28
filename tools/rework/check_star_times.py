"""Replay distinct campaign samples against public clues, proof metrics and projected star times."""
import json
import math
from collections import defaultdict
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]


def read(path):
    return json.loads((ROOT / path).read_bytes())


def estimate_with_notes(level, steps):
    path = level["solution"]["path"]
    count = len(path)
    # Budget for reading each deduction, placing tracks, and marking one in three empty cells.
    notes = math.ceil((len(level["rowCounts"]) * len(level["columnCounts"]) - count) / 3)
    return math.ceil(steps * 0.35 + count * 0.9 + notes * 1.0 + 3)


def main():
    groups = defaultdict(list)
    estimates = read("Content/Quality/s1/star-time-estimates.json")
    projected = {row["puzzleId"]: row for row in read("Assets/StammstreckenPuzzle/UI/MobileCandidate.completion.json")["starTimes"]}
    for row in estimates["entries"]:
        groups[row["size"]].append(row)
        assert projected[row["puzzleId"]]["threeStarMs"] == 1000 * row["threeStarSeconds"]
        assert projected[row["puzzleId"]]["twoStarMs"] == 1000 * row["twoStarSeconds"]
    by_id = {p.name.split(".")[0]: p for p in (ROOT / "Content/Levels/s1").rglob("*.level-v2.json")}
    for size, entries in groups.items():
        ordered = sorted(entries, key=lambda e: (e["deductionSteps"], e["puzzleId"]))
        for q in range(5):
            entry = ordered[round((len(ordered) - 1) * q / 4)]
            pid = entry["puzzleId"]
            proof = read(f"Content/Proofs/{pid}/solver-v2.proof-v1.json")
            level = read(by_id[pid])
            path = level["solution"]["path"]
            assert proof["solutionCount"] == 1 and proof["metrics"]["requiredGuessDepth"] == 0
            assert proof["proofHash"] == level["proofRef"]["proofHash"]
            assert [sum(c["y"] == y for c in path) for y in range(len(level["rowCounts"]))] == level["rowCounts"]
            assert [sum(c["x"] == x for c in path) for x in range(len(level["columnCounts"]))] == level["columnCounts"]
            assert entry["deductionSteps"] == proof["metrics"]["deductionSteps"]
            seconds = estimate_with_notes(level, entry["deductionSteps"])
            print(f"{size} {pid}: mit Notizen geschätzt {seconds}s, drei Sterne bis {entry['threeStarSeconds']}s")
            assert seconds <= entry["threeStarSeconds"], pid


if __name__ == "__main__":
    main()
