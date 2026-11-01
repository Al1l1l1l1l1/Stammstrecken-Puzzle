#!/usr/bin/env python3
"""One prospective WP046 authority for the literal ADR023 -> ADR047 EOF append.

This does not expand the historical manifest.  The caller must still perform all
normal manifest/trust checks and report this separately authorized metadata path.
Only the Git adapter is an integration entry; the pure context is for inspection
and deterministic tests, not an independently supplied approval DTO.
"""
from __future__ import annotations

from dataclasses import dataclass
import hashlib
import json
import os
from pathlib import Path
import re
import subprocess
from typing import Collection


TARGET_PATH = "DECISIONS/ADR-023-releasekandidat-und-kosmetikclaims.md"
RECORD_PATH = "PROJECT_CONTROL/ProductionBlocks/ReleaseCandidate/ADR023_047_METADATA_AUTHORIZATION.json"
MANIFEST_PATH = "tools/architecture-validation/scopes/WP-046.production.scope.json"
MANIFEST_SHA256 = "95efcc68444f2e38d5bad24c7ba48a1f84336c96d1ec3147c97f8eb07d5cd389"
MANIFEST_BASE = "bd05da191cd2bf8da89830f7db6da4941cc62343"
SCOPE_ANCHOR = "39aaac5bdccfc612d71d012e556db80f96f2dcf9"
SOURCE_BASE = "ab5c3e172ed2ed6970499739dc6ee452a7d582ff"
ACCEPTED_ADR_PATH = "DECISIONS/ADR-047-produktionspaketierung-und-release-manifest-v2.md"
ACCEPTED_ADR_SHA256 = "eb3c7e077502d22647cdff3f69a0772ba674707c71a317aa73b40bcf3d7531ff"
OLD_TARGET_SHA256 = "23f758a5f240c3b32c2f8be6fd07896157fdf57b1db8345f5acbf8fa69f224fd"
APPEND_UTF8 = (
    "\n## Nachfolgerverweis vom 22.10.2026\n\n"
    "[ADR-047](./ADR-047-produktionspaketierung-und-release-manifest-v2.md) "
    "ersetzt für neue reale Kandidaten ausschließlich die V1-Manifestnutzung und "
    "die vollständig unveränderliche Manifestidentität durch die dort eng "
    "begrenzte, vorgängergebundene V2-Evidenzfortschreibung. Productionidentität, "
    "Artefaktgleichheit, alle übrigen Manifestfelder und sämtliche Kosmetikverträge "
    "bleiben gültig. Die ursprüngliche Entscheidung und ihre historischen "
    "Nachweise bleiben unverändert.\n"
)
APPEND_BYTES = APPEND_UTF8.encode("utf-8")
HEX40 = re.compile(r"[0-9a-f]{40}\Z")


class MetadataAuthorityError(ValueError):
    """The one additional authority was not proved; leave TARGET_PATH denied."""


def _require(condition: bool, code: str) -> None:
    if not condition:
        raise MetadataAuthorityError("metadata-authority:" + code)


def _sha(raw: bytes) -> str:
    return hashlib.sha256(raw).hexdigest()


def expected_record(old_target_bytes: bytes) -> dict[str, object]:
    """Exact planned decision body; writing it alone grants no authority."""
    _require(_sha(old_target_bytes) == OLD_TARGET_SHA256, "old-target-hash")
    return {
        "schemaVersion": 1,
        "kind": "WP046_ADR023_047_METADATA_AUTHORIZATION",
        "status": "ACCEPTED",
        "authority": "RELEASELEAD_R",
        "mandate": "MANUS-2026-10-20-02",
        "workPackageId": "WP-046",
        "manifestPath": MANIFEST_PATH,
        "manifestRawSha256": MANIFEST_SHA256,
        "scopeAnchorCommit": SCOPE_ANCHOR,
        "sourceBaseCommit": SOURCE_BASE,
        "acceptedAdrPath": ACCEPTED_ADR_PATH,
        "acceptedAdrRawSha256": ACCEPTED_ADR_SHA256,
        "targetPath": TARGET_PATH,
        "oldTargetRawSha256": OLD_TARGET_SHA256,
        "appendUtf8": APPEND_UTF8,
        "appendRawSha256": _sha(APPEND_BYTES),
        "newTargetRawSha256": _sha(old_target_bytes + APPEND_BYTES),
    }


def _object(pairs: list[tuple[str, object]]) -> dict[str, object]:
    result: dict[str, object] = {}
    for key, value in pairs:
        _require(key not in result, "duplicate-record-key")
        result[key] = value
    return result


def _invalid_number(_: str) -> None:
    raise MetadataAuthorityError("metadata-authority:noninteger-record-number")


def _read_record(raw: bytes) -> dict[str, object]:
    try:
        value = json.loads(raw.decode("utf-8"), object_pairs_hook=_object,
                           parse_float=_invalid_number, parse_constant=_invalid_number)
    except (UnicodeDecodeError, json.JSONDecodeError) as error:
        raise MetadataAuthorityError("metadata-authority:invalid-record-json") from error
    _require(type(value) is dict, "record-object")
    _require(type(value.get("schemaVersion")) is int, "record-version-type")
    return value


@dataclass(frozen=True)
class MetadataAuthorityContext:
    changed_paths: tuple[str, ...]
    manifest_path: str
    work_package_id: str
    requested_scope: str
    # Bytes are supplied for each actual role; no normalization or digest-only DTO.
    manifest_at_anchor: bytes
    manifest_at_source_base: bytes
    manifest_at_authorization: bytes
    manifest_current: bytes
    target_at_manifest_base: bytes
    target_at_source_base: bytes
    target_at_authorization_parent: bytes
    target_at_authorization: bytes
    target_current: bytes
    accepted_adr_at_source_base: bytes | None
    accepted_adr_at_authorization: bytes
    accepted_adr_current: bytes
    record_at_add: bytes | None
    record_current: bytes | None
    authorization_add_commits: tuple[str, ...]
    authorization_parents: tuple[str, ...]
    authorization_is_ancestor_of_head: bool
    scope_anchor_is_ancestor_of_source_base: bool
    # Every target-touching commit in SOURCE_BASE..HEAD, including side branches.
    target_changes: tuple[tuple[str, bool], ...]
    record_history: tuple[tuple[bytes, str], ...]
    target_modes: tuple[str, ...]
    record_modes: tuple[str, ...]


def authorize_context(context: MetadataAuthorityContext) -> str | None:
    """Prove only TARGET_PATH; never return caller-selected extra paths."""
    if TARGET_PATH not in context.changed_paths:
        return None
    _require((context.manifest_path, context.work_package_id, context.requested_scope)
             == (MANIFEST_PATH, "WP-046", "production"), "wrong-scope")
    _require(all(_sha(raw) == MANIFEST_SHA256 for raw in (
        context.manifest_at_anchor, context.manifest_at_source_base,
        context.manifest_at_authorization, context.manifest_current)), "manifest-binding")
    _require(context.scope_anchor_is_ancestor_of_source_base is True, "scope-anchor-ancestry")
    _require(len(context.authorization_add_commits) == 1, "record-add-count")
    authorization = context.authorization_add_commits[0]
    _require(HEX40.fullmatch(authorization) is not None, "authorization-commit")
    _require(context.authorization_parents == (SOURCE_BASE,), "authorization-parent")
    _require(context.authorization_is_ancestor_of_head is True, "authorization-not-reachable")
    _require(context.record_at_add is not None and context.record_current is not None,
             "record-not-anchored")
    _require(context.record_at_add == context.record_current, "record-mutated")
    _require(bool(context.record_history)
             and all(raw == context.record_at_add and mode == "100644"
                     for raw, mode in context.record_history), "record-historically-mutated")
    old = context.target_at_source_base
    _require(all(raw == old for raw in (
        context.target_at_manifest_base, context.target_at_authorization_parent,
        context.target_at_authorization)), "old-target-role-mismatch")
    expected = expected_record(old)
    _require(_read_record(context.record_current) == expected, "record-binding")
    _require(context.accepted_adr_at_source_base is None, "accepted-adr-not-absent-at-base")
    _require(all(_sha(raw) == ACCEPTED_ADR_SHA256 for raw in (
        context.accepted_adr_at_authorization, context.accepted_adr_current)), "accepted-adr-binding")
    _require(len(context.target_modes) == 5
             and all(mode == "100644" for mode in context.target_modes), "target-file-mode")
    _require(context.record_modes == ("100644", "100644"), "record-file-mode")
    _require(context.target_current == old + APPEND_BYTES, "not-exact-eof-append")
    for commit, strictly_after in context.target_changes:
        _require(HEX40.fullmatch(commit) is not None and commit != authorization
                 and strictly_after is True, "target-changed-before-authorization")
    return TARGET_PATH


class _Git:
    """Read-only object/history access.  No checkout/index/ref mutations."""

    def __init__(self, repository: Path):
        self.repository = repository

    def _run(self, *arguments: str, allow_false: bool = False) -> subprocess.CompletedProcess[bytes]:
        environment = dict(os.environ, GIT_OPTIONAL_LOCKS="0")
        result = subprocess.run(
            ["git", "--literal-pathspecs", "-c", "core.fsmonitor=false", "-C",
             str(self.repository), *arguments], capture_output=True, check=False, env=environment)
        _require(result.returncode == 0 or (allow_false and result.returncode == 1),
                 "git-read-failed")
        return result

    def text(self, *arguments: str) -> str:
        return self._run(*arguments).stdout.decode("utf-8", errors="strict").strip()

    def blob(self, commit: str, path: str) -> bytes:
        return self._run("show", f"{commit}:{path}").stdout

    def optional_blob(self, commit: str, path: str) -> bytes | None:
        # Only a successful empty tree lookup proves absence. A Git failure does not.
        if not self._run("ls-tree", "-z", commit, "--", path).stdout:
            return None
        return self.blob(commit, path)

    def ancestor(self, earlier: str, later: str) -> bool:
        return self._run("merge-base", "--is-ancestor", earlier, later,
                         allow_false=True).returncode == 0

    def tree_mode(self, commit: str, path: str) -> str:
        raw = self._run("ls-tree", "-z", commit, "--", path).stdout
        rows = raw.rstrip(b"\0").split(b"\0")
        _require(len(rows) == 1 and b"\t" in rows[0], "missing-tree-file")
        metadata, actual_path = rows[0].split(b"\t", 1)
        parts = metadata.decode("ascii").split()
        _require(len(parts) == 3 and parts[1] == "blob"
                 and actual_path == path.encode("utf-8"), "nonliteral-tree-file")
        return parts[0]

    def index_mode(self, path: str) -> str:
        raw = self._run("ls-files", "--stage", "-z", "--", path).stdout
        rows = raw.rstrip(b"\0").split(b"\0")
        _require(len(rows) == 1 and b"\t" in rows[0], "missing-index-file")
        metadata, actual_path = rows[0].split(b"\t", 1)
        parts = metadata.decode("ascii").split()
        _require(len(parts) == 3 and parts[2] == "0"
                 and actual_path == path.encode("utf-8"), "nonliteral-index-file")
        return parts[0]


def _read_regular(repository: Path, relative: str) -> bytes:
    candidate = repository / relative
    # Literal constants only; also reject symlinks/junctions inside those paths.
    _require(candidate.resolve(strict=True) == candidate, "noncanonical-working-file")
    _require(candidate.is_file() and not candidate.is_symlink(), "nonregular-working-file")
    return candidate.read_bytes()


def authorized_metadata_path(repository: Path, changed_paths: Collection[str], *,
                             manifest_path: str, work_package_id: str,
                             requested_scope: str) -> str | None:
    """Return the one separately proved path, or raise and leave it out of scope.

    Works immediately after the auth commit with an unstaged working-tree append;
    an untracked or simultaneous authorization cannot satisfy historical proof.
    All ordinary scope/trust guards remain the caller's responsibility.
    """
    if TARGET_PATH not in changed_paths:
        return None
    _require((manifest_path, work_package_id, requested_scope)
             == (MANIFEST_PATH, "WP-046", "production"), "wrong-scope")
    try:
        root = repository.resolve(strict=True)
        git = _Git(root)
        additions = tuple(filter(None, git.text(
            "log", "--full-history", "--diff-filter=A", "--format=%H", "HEAD", "--", RECORD_PATH).splitlines()))
        _require(len(additions) == 1 and HEX40.fullmatch(additions[0]) is not None,
                 "record-add-count")
        authorization = additions[0]
        parents = git.text("rev-list", "--parents", "-n", "1", authorization).split()
        _require(bool(parents) and parents[0] == authorization, "authorization-history")
        _require(tuple(parents[1:]) == (SOURCE_BASE,), "authorization-parent")
        changes = tuple(filter(None, git.text(
            "rev-list", "--full-history", f"{SOURCE_BASE}..HEAD", "--", TARGET_PATH).splitlines()))
        record_changes = tuple(filter(None, git.text(
            "rev-list", "--full-history", f"{SOURCE_BASE}..HEAD", "--", RECORD_PATH).splitlines()))
        context = MetadataAuthorityContext(
            changed_paths=tuple(changed_paths), manifest_path=manifest_path,
            work_package_id=work_package_id, requested_scope=requested_scope,
            manifest_at_anchor=git.blob(SCOPE_ANCHOR, MANIFEST_PATH),
            manifest_at_source_base=git.blob(SOURCE_BASE, MANIFEST_PATH),
            manifest_at_authorization=git.blob(authorization, MANIFEST_PATH),
            manifest_current=_read_regular(root, MANIFEST_PATH),
            target_at_manifest_base=git.blob(MANIFEST_BASE, TARGET_PATH),
            target_at_source_base=git.blob(SOURCE_BASE, TARGET_PATH),
            target_at_authorization_parent=git.blob(parents[1], TARGET_PATH),
            target_at_authorization=git.blob(authorization, TARGET_PATH),
            target_current=_read_regular(root, TARGET_PATH),
            accepted_adr_at_source_base=git.optional_blob(SOURCE_BASE, ACCEPTED_ADR_PATH),
            accepted_adr_at_authorization=git.blob(authorization, ACCEPTED_ADR_PATH),
            accepted_adr_current=_read_regular(root, ACCEPTED_ADR_PATH),
            record_at_add=git.blob(authorization, RECORD_PATH),
            record_current=_read_regular(root, RECORD_PATH),
            authorization_add_commits=additions, authorization_parents=tuple(parents[1:]),
            authorization_is_ancestor_of_head=git.ancestor(authorization, "HEAD"),
            scope_anchor_is_ancestor_of_source_base=git.ancestor(SCOPE_ANCHOR, SOURCE_BASE),
            target_changes=tuple((commit, commit != authorization
                                  and git.ancestor(authorization, commit)) for commit in changes),
            record_history=tuple((git.blob(commit, RECORD_PATH), git.tree_mode(commit, RECORD_PATH))
                                 for commit in record_changes),
            target_modes=tuple(git.tree_mode(commit, TARGET_PATH) for commit in (
                MANIFEST_BASE, SOURCE_BASE, authorization, "HEAD")) + (git.index_mode(TARGET_PATH),),
            record_modes=(git.tree_mode(authorization, RECORD_PATH), git.index_mode(RECORD_PATH)),
        )
        return authorize_context(context)
    except MetadataAuthorityError:
        raise
    except (OSError, UnicodeError, ValueError) as error:
        raise MetadataAuthorityError("metadata-authority:incomplete-readback") from error
