#!/usr/bin/env python3
"""Recover only the approved 240 authoring/proof inputs; never promote their DRAFT gates."""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path
import re
import subprocess

ROOT = Path(__file__).resolve().parents[2]
SOURCE = "dc1652befd7545b4c2a2dd0580f35a4e8e05d72d"
LOCK = ROOT / "Content/Quality/s1/review-source-lock.json"
LEVEL = re.compile(r"Content/Levels/s1/(0[1-5])/(0[1-4])/(S1-\1-\2-(?:0[1-9]|1[0-2]))\.level-v2\.json")
PROOF = re.compile(r"Content/Proofs/(S1-0[1-5]-0[1-4]-(?:0[1-9]|1[0-2]))/solver-v2\.proof-v1\.json")


def git(*args: str) -> bytes:
    return subprocess.check_output(["git", "-C", str(ROOT), *args])


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--check", action="store_true", help="Read-only comparison; never overwrite improved inputs")
    args = parser.parse_args()
    sources = []
    counts = {"level": 0, "proof": 0}
    members = set()
    for line in git("ls-tree", "-r", SOURCE, "--", "Content/Levels/s1", "Content/Proofs").decode().splitlines():
        metadata, path = line.split("\t", 1)
        mode, kind, blob = metadata.split()
        level, proof = LEVEL.fullmatch(path), PROOF.fullmatch(path)
        if mode != "100644" or kind != "blob" or (level is None and proof is None):
            raise ValueError("Unexpected source object: " + path)
        data = git("cat-file", "blob", blob)
        document = json.loads(data)
        target = ROOT / path
        if args.check:
            if not target.is_file() or target.read_bytes().replace(b"\r\n", b"\n") != data.replace(b"\r\n", b"\n"):
                raise ValueError("Input differs from imported baseline: " + path)
        elif target.exists():
            if target.read_bytes().replace(b"\r\n", b"\n") != data.replace(b"\r\n", b"\n"):
                raise ValueError("Refusing to overwrite changed authoring input: " + path)
        else:
            target.parent.mkdir(parents=True, exist_ok=True)
            target.write_bytes(data)
        label = "level" if level else "proof"
        counts[label] += 1
        if level:
            puzzle_id = level.group(3)
            if document.get("puzzleId") != puzzle_id:
                raise ValueError("Puzzle identity mismatch: " + path)
            members.add(puzzle_id)
        sources.append({"path": path, "blobId": blob, "bytes": len(data), "sha256": hashlib.sha256(data).hexdigest()})
    expected = {f"S1-{section:02}-{route:02}-{position:02}" for section in range(1, 6) for route in range(1, 5) for position in range(1, 13)}
    if counts != {"level": 240, "proof": 240} or members != expected:
        raise ValueError("Campaign input coverage is not exactly 5 x 4 x 12")
    lock = {"sourceCommit": SOURCE, "approvalStatus": "DRAFT", "runtimeImportAllowed": False,
            "releaseEligible": False, "levelCount": 240, "proofCount": 240, "sources": sources}
    encoded = (json.dumps(lock, ensure_ascii=False, indent=2) + "\n").encode("utf-8")
    if args.check:
        if LOCK.read_bytes().replace(b"\r\n", b"\n") != encoded:
            raise ValueError("Source lock mismatch")
    else:
        LOCK.parent.mkdir(parents=True, exist_ok=True)
        LOCK.write_bytes(encoded)
    print("PASS: 240 authoring inputs and 240 original proofs; DRAFT/import/release gates preserved.")


if __name__ == "__main__":
    main()
