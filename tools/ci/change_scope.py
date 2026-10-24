#!/usr/bin/env python3
"""Fail-closed change classification for the STP CI workflows.

The tool deliberately uses only the Python standard library.  It classifies both
sides of renames/deletions, treats incomplete Git event data as product-relevant,
and exposes small GitHub Actions outputs without making a skipped Unity job look
like a successful Unity result.
"""
from __future__ import annotations

import argparse
import dataclasses
import json
import pathlib
import re
import subprocess
import sys
from typing import Iterable, Mapping, Sequence

sys.dont_write_bytecode = True


DOC_PREFIXES = (
    "ARCHITECTURE/",
    "DECISIONS/",
    "PROJECT_CONTROL/",
    "Stammstrecken_Puzzle_Konzept_00-15/",
    "WORK_PACKAGES/",
    "Content/Reviews/",
    "docs/",
)
TEXT_PREFIXES = ("Content/Localization/", "tools/text-library/")
TOOL_PREFIXES = ("tools/ci/", "tools/architecture-validation/")
SCOPE_PREFIX = "tools/architecture-validation/scopes/"
PRODUCT_PREFIXES = ("Assets/", "Packages/", "ProjectSettings/")
# Authoring input to HumanReadback/ProductionImport, not a release status report.
HUMAN_ASSIGNMENT_BINDINGS = "Content/Quality/human/assignment-source-bindings.json"
# Descriptive inspections of already-built artifacts; never build/provider inputs.
NATIVE_INSPECTION_PATH = re.compile(
    r"Release/(?:ANDROID_BUILD|HUMAN_ANDROID_BUILD|IOS_EXPORT)[1-9][0-9]*_INSPECTION\.json"
)
UNITY_WORKFLOW = ".github/workflows/unity.yml"
JCS_PATHS = {
    "tools/architecture-validation/jcs_crosscheck.mjs",
    "Assets/StammstreckenPuzzle/Tests/EditMode/Content/level-v2-jcs-golden.json",
}
ROUTING_NEEDS = {
    "project-preflight": {"[changes-preflight]"},
    "unity-config": {"[changes-preflight]"},
    "unity-evidence-guard": {"[project-preflight, unity-config]", "[changes-preflight, project-preflight, unity-config]"},
    "unity-environment-linux": {"[changes-preflight]"},
    "unity-environment-android": {"[changes-preflight]"},
    "unity-environment-ios": {"[changes-preflight]"},
    "unity-compile-editmode": {"[project-preflight, unity-config]", "[changes-preflight, project-preflight, unity-config]"},
    "unity-playmode-bootstrap-smoke": {
        "[project-preflight, unity-config, unity-compile-editmode]",
        "[changes-preflight, project-preflight, unity-config, unity-compile-editmode]",
    },
    "unity-android-development-il2cpp": {
        "[project-preflight, unity-config, unity-playmode-bootstrap-smoke]",
        "[changes-preflight, project-preflight, unity-config, unity-playmode-bootstrap-smoke]",
    },
    "unity-ios-export": {
        "[project-preflight, unity-config, unity-android-development-il2cpp]",
        "[changes-preflight, project-preflight, unity-config, unity-android-development-il2cpp]",
    },
}
ROUTING_IFS = {
    "project-preflight": {"needs.changes-preflight.outputs.requires_unity == 'true'"},
    "unity-config": {"needs.changes-preflight.outputs.requires_unity == 'true'"},
    "unity-evidence-guard": {"needs.changes-preflight.outputs.requires_unity == 'true'"},
    "unity-environment-linux": {"needs.changes-preflight.outputs.requires_unity == 'true'"},
    "unity-environment-android": {"needs.changes-preflight.outputs.requires_unity == 'true'"},
    "unity-environment-ios": {"needs.changes-preflight.outputs.requires_unity == 'true'"},
    "unity-compile-editmode": {
        "needs.unity-config.outputs.ready == 'true'",
        "needs.changes-preflight.outputs.requires_unity == 'true' && needs.unity-config.outputs.ready == 'true'",
    },
    "unity-playmode-bootstrap-smoke": {
        "needs.unity-config.outputs.ready == 'true'",
        "needs.changes-preflight.outputs.requires_unity == 'true' && needs.unity-config.outputs.ready == 'true'",
    },
    "unity-android-development-il2cpp": {
        "needs.unity-config.outputs.ready == 'true'",
        "needs.changes-preflight.outputs.requires_unity == 'true' && needs.unity-config.outputs.ready == 'true'",
    },
    "unity-ios-export": {
        "needs.unity-config.outputs.ready == 'true'",
        "needs.changes-preflight.outputs.requires_unity == 'true' && needs.unity-config.outputs.ready == 'true'",
    },
}


@dataclasses.dataclass(frozen=True)
class Change:
    """One Git name-status record, retaining both sides of a move/copy."""

    status: str
    paths: tuple[str, ...]


@dataclasses.dataclass(frozen=True)
class Classification:
    requires_unity: bool
    unity_status: str
    run_text: bool
    run_jcs: bool
    source_change_class: tuple[str, ...]
    reasons: tuple[str, ...]
    source_product_changed: bool
    pr_merge_product_changed: bool

    def outputs(self) -> Mapping[str, str]:
        return {
            "requires_unity": str(self.requires_unity).lower(),
            "run_text": str(self.run_text).lower(),
            "run_jcs": str(self.run_jcs).lower(),
            "source_product_changed": str(self.source_product_changed).lower(),
            "pr_merge_product_changed": str(self.pr_merge_product_changed).lower(),
            "source_change_class": ",".join(self.source_change_class),
            "reason": ";".join(self.reasons),
            "unity_status": self.unity_status,
        }


class GitBasisError(RuntimeError):
    """Raised when an event range cannot be inspected completely."""


def _run_git(args: Sequence[str], *, cwd: pathlib.Path) -> bytes:
    process = subprocess.run(
        ["git", *args], cwd=cwd, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=False
    )
    if process.returncode != 0:
        message = process.stderr.decode("utf-8", "replace").strip()
        raise GitBasisError(f"git {' '.join(args)} failed: {message or process.returncode}")
    return process.stdout


def _valid_revision(revision: str | None, *, cwd: pathlib.Path) -> bool:
    if not revision or set(revision) == {"0"}:
        return False
    try:
        _run_git(["rev-parse", "--verify", f"{revision}^{{commit}}"], cwd=cwd)
    except GitBasisError:
        return False
    return True


def _parse_name_status(raw: bytes) -> list[Change]:
    """Parse NUL-safe Git name-status output, including both sides of moves."""
    fields = raw.split(b"\0")
    if fields and fields[-1] == b"":
        fields.pop()
    records: list[Change] = []
    index = 0
    while index < len(fields):
        status = fields[index].decode("utf-8", "surrogateescape")
        index += 1
        if not status:
            raise GitBasisError("empty name-status record")
        code = status[0]
        count = 2 if code in {"R", "C"} else 1
        if code not in {"A", "M", "D", "T", "U", "X", "B", "R", "C"} or index + count > len(fields):
            raise GitBasisError(f"unsupported or truncated name-status record: {status!r}")
        paths = tuple(item.decode("utf-8", "surrogateescape") for item in fields[index : index + count])
        index += count
        records.append(Change(status=status, paths=paths))
    return records


def diff_records(base: str, head: str, *, cwd: pathlib.Path) -> list[Change]:
    """Read a NUL-safe, exact-rename Git diff or fail closed."""
    if not _valid_revision(base, cwd=cwd) or not _valid_revision(head, cwd=cwd):
        raise GitBasisError("missing, zero, or unresolvable Git event basis")
    raw = _run_git(
        ["diff", "--name-status", "-z", "--find-renames=100%", "--find-copies=100%", base, head],
        cwd=cwd,
    )
    return _parse_name_status(raw)


def worktree_records(base: str, *, cwd: pathlib.Path) -> list[Change]:
    """Inspect base→index/worktree plus untracked files for local evidence."""
    if not _valid_revision(base, cwd=cwd):
        raise GitBasisError("missing, zero, or unresolvable worktree basis")
    raw = _run_git(
        ["diff", "--name-status", "-z", "--find-renames=100%", "--find-copies=100%", base], cwd=cwd
    )
    records = _parse_name_status(raw)
    untracked = _run_git(["ls-files", "--others", "--exclude-standard", "-z"], cwd=cwd)
    records.extend(
        Change("A", (path.decode("utf-8", "surrogateescape"),))
        for path in untracked.split(b"\0")
        if path
    )
    return records


def path_kind(path: str) -> str:
    """Return the least-privileged CI class for a tracked repository path."""
    pure = pathlib.PurePosixPath(path)
    if (
        not path
        or path.startswith("/")
        or "\\" in path
        or any(part in {"", ".", ".."} for part in pure.parts)
    ):
        return "unknown"
    if path == UNITY_WORKFLOW or path.startswith(".github/workflows/"):
        return "workflow"
    if path.startswith(SCOPE_PREFIX):
        return "governance"
    if path.startswith(TEXT_PREFIXES):
        return "text"
    if path.startswith(PRODUCT_PREFIXES) or path in {"Toolchain.lock.md", "tools/ci/prepare_coverage.py", HUMAN_ASSIGNMENT_BINDINGS}:
        return "product"
    if path.startswith(TOOL_PREFIXES):
        return "tooling"
    if NATIVE_INSPECTION_PATH.fullmatch(path):
        return "governance"
    if path.startswith(DOC_PREFIXES) or path.endswith(".md"):
        return "governance"
    return "unknown"


def _indent(line: str) -> int:
    return len(line) - len(line.lstrip(" "))


def normalize_unity_control(text: str) -> str:
    """Strip authorized routing-only portions before comparing Unity recipes.

    This is intentionally *not* a general YAML parser.  The fixed workflow has a
    constrained, indentation-based structure.  All commands, actions, runners,
    containers, evidence steps and other job recipes remain in the comparison.
    Only event routing, the dedicated cheap changes-preflight job, job/step
    ``if``/``needs`` edges and job outputs are routing controls.
    """
    kept: list[str] = []
    in_on = False
    in_jobs = False
    skipped_job = False
    skipping_outputs = False
    current_job = ""

    for raw in text.splitlines():
        stripped = raw.strip()
        indent = _indent(raw)
        if not stripped or stripped.startswith("#"):
            continue
        if indent == 0 and re.fullmatch(r"on:\s*(?:#.*)?", stripped):
            in_on = True
            in_jobs = False
            skipped_job = False
            skipping_outputs = False
            continue
        if in_on:
            if indent == 0:
                in_on = False
            else:
                continue
        if indent == 0 and re.fullmatch(r"jobs:\s*(?:#.*)?", stripped):
            in_jobs = True
            skipped_job = False
            skipping_outputs = False
            kept.append("jobs:")
            continue
        if in_jobs and indent == 2:
            match = re.fullmatch(r"([A-Za-z0-9_-]+):\s*(?:#.*)?", stripped)
            if match:
                current_job = match.group(1)
                skipped_job = current_job == "changes-preflight"
                skipping_outputs = False
                if not skipped_job:
                    kept.append(raw)
                continue
        if skipped_job:
            continue
        if skipping_outputs:
            if indent > 4:
                continue
            skipping_outputs = False
        if current_job == "changes-preflight" and indent == 4 and re.fullmatch(r"outputs:\s*(?:#.*)?", stripped):
            skipping_outputs = True
            continue
        if in_jobs and indent == 4:
            if stripped.startswith("needs:") and stripped.removeprefix("needs:").strip() in ROUTING_NEEDS.get(current_job, set()):
                continue
            if stripped.startswith("if:") and stripped.removeprefix("if:").strip() in ROUTING_IFS.get(current_job, set()):
                continue
        kept.append(raw)
    return "\n".join(kept) + "\n"


def _git_file(revision: str, path: str, *, cwd: pathlib.Path) -> str | None:
    process = subprocess.run(
        ["git", "show", f"{revision}:{path}"],
        cwd=cwd,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        check=False,
    )
    if process.returncode != 0:
        return None
    return process.stdout.decode("utf-8", "surrogateescape")


def unity_recipe_changed(base: str, head: str, *, cwd: pathlib.Path) -> bool:
    before = _git_file(base, UNITY_WORKFLOW, cwd=cwd)
    after = _git_file(head, UNITY_WORKFLOW, cwd=cwd)
    if before is None or after is None:
        return True
    return normalize_unity_control(before) != normalize_unity_control(after)


def unity_recipe_worktree_changed(base: str, *, cwd: pathlib.Path) -> bool:
    before = _git_file(base, UNITY_WORKFLOW, cwd=cwd)
    path = cwd / UNITY_WORKFLOW
    if before is None or not path.is_file():
        return True
    after = path.read_text(encoding="utf-8", errors="surrogateescape")
    return normalize_unity_control(before) != normalize_unity_control(after)


def _kinds(records: Iterable[Change]) -> set[str]:
    return {path_kind(path) for record in records for path in record.paths}


def _has_path(records: Iterable[Change], path: str) -> bool:
    return any(candidate == path for record in records for candidate in record.paths)


def _product_relevant(kinds: set[str], *, recipe_changed: bool) -> bool:
    return bool({"product", "unknown"} & kinds) or recipe_changed


def classify_records(
    source_records: Sequence[Change],
    *,
    event: str,
    source_recipe_changed: bool = False,
    merge_records: Sequence[Change] = (),
    merge_recipe_changed: bool = False,
    force_full: bool = False,
) -> Classification:
    """Classify already-inspected records; exported for deterministic unit tests."""
    reasons: list[str] = []
    source_kinds = _kinds(source_records)
    merge_kinds = _kinds(merge_records)
    uncertain = "unknown" in source_kinds or "unknown" in merge_kinds
    source_product = _product_relevant(source_kinds, recipe_changed=source_recipe_changed)
    merge_product = _product_relevant(merge_kinds, recipe_changed=merge_recipe_changed)

    if force_full:
        requires_unity = True
        unity_status = "REQUIRED"
        reasons.append("force_full")
    elif not source_records:
        # A known empty name-status list is not a valid safety basis for a CI
        # reduction.  It can result from an incomplete event range or ref race.
        requires_unity = True
        unity_status = "REQUIRED"
        reasons.append("event_basis_empty_fail_closed")
    elif uncertain:
        # A path outside the explicit classifier, including one in the
        # synthetic PR union, is never eligible for deferred execution.
        requires_unity = True
        unity_status = "REQUIRED"
        reasons.append("unknown_diff_fail_closed")
    elif event == "push":
        if source_recipe_changed:
            # A real job/step/action/runner/evidence recipe change must prove
            # itself immediately.  Routing-only workflow changes normalize to
            # false and remain eligible for the documentation path below.
            requires_unity = True
            unity_status = "REQUIRED"
            reasons.append("push_unity_recipe_change_fail_closed")
        elif source_product:
            # The product candidate will be built once at its PR merge
            # candidate.  A complete, classified working-branch push carries
            # only the inexpensive preflight and must not masquerade as Unity
            # success or as an unchanged source.
            requires_unity = False
            unity_status = "DEFERRED_TO_PR_CANDIDATE"
            reasons.append("known_product_push_deferred_to_pr_candidate")
        else:
            requires_unity = False
            unity_status = "NOT_REQUIRED_SOURCE_UNCHANGED"
            reasons.append("docs_or_routing_source_unchanged")
    elif event == "pull_request":
        # The PR is the sole full Unity candidate for a known product branch.
        # source_records is base→source; merge_records is source→synthetic
        # merge.  Their product union protects both the actual branch change
        # and changes introduced by the candidate/base merge resolution.
        requires_unity = source_product or merge_product
        if source_product:
            reasons.append("pr_source_product_changed")
        if merge_product:
            reasons.append("pr_synthetic_source_base_union_changed")
        if not requires_unity:
            reasons.append("pr_source_and_union_unchanged")
        unity_status = "REQUIRED" if requires_unity else "NOT_REQUIRED_SOURCE_UNCHANGED"
    else:
        requires_unity = True
        unity_status = "REQUIRED"
        reasons.append("unsupported_event_fail_closed")

    text_changed = "text" in source_kinds or "text" in merge_kinds
    jcs_changed = source_product or merge_product or any(
        path in JCS_PATHS for record in (*source_records, *merge_records) for path in record.paths
    )
    # A forced manual full/RC run intentionally restores both selective checks.
    run_text = force_full or text_changed or uncertain
    run_jcs = force_full or jcs_changed or uncertain
    ordered_classes = tuple(sorted(source_kinds or {"unknown"}))
    return Classification(
        requires_unity=requires_unity,
        unity_status=unity_status,
        run_text=run_text,
        run_jcs=run_jcs,
        source_change_class=ordered_classes,
        reasons=tuple(reasons),
        source_product_changed=source_product,
        pr_merge_product_changed=merge_product,
    )


def classify_git(
    *,
    event: str,
    base: str | None,
    head: str | None,
    merge: str | None,
    force_full: bool,
    cwd: pathlib.Path,
) -> Classification:
    """Inspect event refs and turn any missing basis into a full fail-closed run."""
    if force_full:
        return classify_records([Change("F", ("forced-manual-run",))], event=event, force_full=True)
    if event not in {"push", "pull_request"}:
        return classify_records([Change("E", ("unsupported-event",))], event=event)
    try:
        source_records = diff_records(base or "", head or "", cwd=cwd)
        source_recipe = _has_path(source_records, UNITY_WORKFLOW) and unity_recipe_changed(
            base or "", head or "", cwd=cwd
        )
        merge_records: list[Change] = []
        merge_recipe = False
        if event == "pull_request":
            if not _valid_revision(merge, cwd=cwd):
                raise GitBasisError("missing synthetic pull-request merge revision")
            merge_records = diff_records(head or "", merge or "", cwd=cwd)
            merge_recipe = _has_path(merge_records, UNITY_WORKFLOW) and unity_recipe_changed(
                head or "", merge or "", cwd=cwd
            )
        return classify_records(
            source_records,
            event=event,
            source_recipe_changed=source_recipe,
            merge_records=merge_records,
            merge_recipe_changed=merge_recipe,
        )
    except GitBasisError as error:
        return Classification(
            requires_unity=True,
            unity_status="REQUIRED",
            run_text=True,
            run_jcs=True,
            source_change_class=("unknown",),
            reasons=("event_basis_missing_fail_closed", str(error).replace(";", ",")),
            source_product_changed=True,
            pr_merge_product_changed=event == "pull_request",
        )


def classify_worktree(*, event: str, base: str | None, force_full: bool, cwd: pathlib.Path) -> Classification:
    """Classify a local base→worktree comparison for pre-push verification."""
    if force_full:
        return classify_records([Change("F", ("forced-manual-run",))], event=event, force_full=True)
    try:
        records = worktree_records(base or "", cwd=cwd)
        recipe_changed = _has_path(records, UNITY_WORKFLOW) and unity_recipe_worktree_changed(base or "", cwd=cwd)
        return classify_records(records, event=event, source_recipe_changed=recipe_changed)
    except GitBasisError as error:
        return Classification(
            requires_unity=True,
            unity_status="REQUIRED",
            run_text=True,
            run_jcs=True,
            source_change_class=("unknown",),
            reasons=("worktree_basis_missing_fail_closed", str(error).replace(";", ",")),
            source_product_changed=True,
            pr_merge_product_changed=False,
        )


def _self_test() -> None:
    docs = [Change("M", ("PROJECT_CONTROL/CURRENT_STATE.md",))]
    text = [Change("M", ("Content/Localization/source/de-DE.json",))]
    asset = [Change("M", ("Assets/StammstreckenPuzzle/Puzzle.cs",))]
    unknown = [Change("M", ("unexpected.bin",))]
    rename = [Change("R100", ("PROJECT_CONTROL/old.md", "Assets/new.asset"))]
    control = [Change("M", (UNITY_WORKFLOW,))]

    assert not classify_records(docs, event="push").requires_unity
    assert classify_records(text, event="push").run_text and not classify_records(text, event="push").requires_unity
    deferred = classify_records(asset, event="push")
    assert not deferred.requires_unity
    assert deferred.outputs()["unity_status"] == "DEFERRED_TO_PR_CANDIDATE"
    assert classify_records(unknown, event="push").requires_unity
    assert classify_records(rename, event="push").outputs()["unity_status"] == "DEFERRED_TO_PR_CANDIDATE"
    assert not classify_records(control, event="push", source_recipe_changed=False).requires_unity
    assert classify_records(control, event="push", source_recipe_changed=True).requires_unity
    assert classify_records(asset, event="pull_request", merge_records=[]).requires_unity
    assert classify_records(docs, event="pull_request", merge_records=asset).requires_unity
    assert classify_records([], event="push").requires_unity
    assert classify_records(docs, event="push", force_full=True).requires_unity
    print("change_scope self-test PASS: docs, text, deferred push, PR candidate union, unknown, recipe and fail-closed cases")


def _parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--event", choices=("push", "pull_request", "workflow_dispatch"), required=False)
    parser.add_argument("--base")
    parser.add_argument("--head")
    parser.add_argument("--merge", help="synthetic PR merge revision; required for pull_request")
    parser.add_argument("--force-full", action="store_true")
    parser.add_argument("--github-output", type=pathlib.Path)
    parser.add_argument("--repo", type=pathlib.Path, default=pathlib.Path("."))
    parser.add_argument("--worktree-base", help="classify base→current worktree (local verification only)")
    parser.add_argument("--self-test", action="store_true")
    return parser


def main(argv: Sequence[str] | None = None) -> int:
    args = _parser().parse_args(argv)
    if args.self_test:
        _self_test()
        return 0
    if not args.event:
        raise SystemExit("--event is required unless --self-test is used")
    if args.worktree_base:
        result = classify_worktree(
            event=args.event,
            base=args.worktree_base,
            force_full=args.force_full,
            cwd=args.repo.resolve(),
        )
    else:
        result = classify_git(
            event=args.event,
            base=args.base,
            head=args.head,
            merge=args.merge,
            force_full=args.force_full,
            cwd=args.repo.resolve(),
        )
    outputs = result.outputs()
    print(json.dumps(outputs, sort_keys=True))
    if args.github_output:
        with args.github_output.open("a", encoding="utf-8", newline="\n") as handle:
            for key, value in outputs.items():
                handle.write(f"{key}={value}\n")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
