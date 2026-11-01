#!/usr/bin/env python3
"""Restore one hash-bound reference's exact Git bytes after CRLF checkout conversion.

Run before local Windows Unity tests. Never accepts content changes, changes an
oracle, or rewrites Git history. A differing checkout is preserved before repair.
"""
from pathlib import Path
import argparse
import hashlib
import subprocess

REFERENCE = "PROJECT_CONTROL/ProductionBlocks/Public240Save/S/GOLDENS_PRECODEC_V7.json"
EXPECTED_SHA256 = "ee0ef35bd3dbf03396fc5fd30389b9e75e486f8b08982b2efde3ffec1cba4ed0"


def reject_reparse(path: Path, root: Path) -> None:
    for item in (path, *path.parents):
        if item.is_symlink() or item.exists() and getattr(item.stat(follow_symlinks=False), "st_file_attributes", 0) & 0x400:
            raise ValueError("GOLDEN_REPARSE_PATH_REJECTED")
        if item == root:
            return
    raise ValueError("GOLDEN_PATH_OUTSIDE_ROOT")


def restore(root: Path) -> bool:
    root = root.resolve(strict=True)
    reject_reparse(root / REFERENCE, root)
    target = (root / REFERENCE).resolve(strict=True)
    if not target.is_relative_to(root) or not (root / "AGENTS.md").is_file():
        raise ValueError("GOLDEN_REFERENCE_ROOT_INVALID")
    canonical = subprocess.check_output(["git", "-C", str(root), "show", "HEAD:" + REFERENCE])
    digest = lambda data: hashlib.sha256(data).hexdigest()
    if digest(canonical) != EXPECTED_SHA256:
        raise ValueError("GOLDEN_GIT_ORIGINAL_HASH_MISMATCH")
    actual = target.read_bytes()
    if actual == canonical:
        return False
    if actual.replace(b"\r\n", b"\n") != canonical:
        raise ValueError("GOLDEN_CHECKOUT_HAS_NON_EOL_CHANGES")
    preserved = root / ".utmp" / "checkout-originals" / (digest(actual) + ".bin")
    reject_reparse(preserved, root)
    if not preserved.parent.resolve().is_relative_to(root):
        raise ValueError("GOLDEN_BACKUP_ROOT_INVALID")
    preserved.parent.mkdir(parents=True, exist_ok=True)
    try:
        with preserved.open("xb") as stream:
            stream.write(actual)
    except FileExistsError:
        if preserved.read_bytes() != actual:
            raise ValueError("GOLDEN_BACKUP_HASH_MISMATCH")
    if preserved.read_bytes() != actual:
        raise ValueError("GOLDEN_BACKUP_REREAD_MISMATCH")
    if target.read_bytes() != actual:
        raise ValueError("GOLDEN_CHECKOUT_CHANGED_DURING_READ")
    target.write_bytes(canonical)
    if target.read_bytes() != canonical:
        raise ValueError("GOLDEN_RESTORE_REREAD_MISMATCH")
    return True


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--repo-root", type=Path, default=Path.cwd())
    args = parser.parse_args()
    try:
        changed = restore(args.repo_root)
        print("Golden reference: exact original Git bytes " + ("restored; CRLF checkout preserved." if changed else "verified; unchanged."))
    except (OSError, ValueError, subprocess.CalledProcessError) as error:
        parser.exit(1, "GOLDEN_CHECKOUT_FAILED: " + str(error) + "\n")
