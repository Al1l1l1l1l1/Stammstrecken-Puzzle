#!/usr/bin/env python3
"""Deterministic tests for tools/ci/change_scope.py; no Unity or network access."""
from __future__ import annotations

import pathlib
import re
import subprocess
import sys
import unittest

sys.dont_write_bytecode = True
sys.path.insert(0, str(pathlib.Path(__file__).resolve().parent))
import change_scope as subject  # noqa: E402


class ChangeScopeTests(unittest.TestCase):
    def records(self, *paths: str) -> list[subject.Change]:
        return [subject.Change("M", (path,)) for path in paths]

    def test_docs_governance_status_and_review_do_not_require_unity(self) -> None:
        result = subject.classify_records(
            self.records(
                "PROJECT_CONTROL/CURRENT_STATE.md",
                "WORK_PACKAGES/WP-045_Produktionskurskorrektur-und-Release-Gesamtmandat.md",
                "Content/Reviews/text-library/WP-036/COPY_CHANGE_REVIEW.md",
            ),
            event="push",
        )
        self.assertFalse(result.requires_unity)
        self.assertFalse(result.run_text)
        self.assertEqual("NOT_REQUIRED_SOURCE_UNCHANGED", result.outputs()["unity_status"])

    def test_text_source_and_text_tool_run_text_not_unity(self) -> None:
        result = subject.classify_records(
            self.records("Content/Localization/source/de-DE.json", "tools/text-library/validate.py"),
            event="push",
        )
        self.assertFalse(result.requires_unity)
        self.assertTrue(result.run_text)
        self.assertFalse(result.run_jcs)

    def test_native_inspection_reports_are_descriptive_evidence(self) -> None:
        reports = self.records(
            "Release/ANDROID_BUILD12_INSPECTION.json",
            "Release/HUMAN_ANDROID_BUILD7_INSPECTION.json",
            "Release/IOS_EXPORT4_INSPECTION.json",
            "Release/ANDROID_BUILD123_INSPECTION.json",
            "Release/CI_ANDROID_40143424066_INSPECTION.json",
        )
        for event in ("push", "pull_request"):
            with self.subTest(event=event):
                result = subject.classify_records(reports, event=event)
                self.assertEqual(("governance",), result.source_change_class)
                self.assertFalse(result.requires_unity)
                self.assertEqual("NOT_REQUIRED_SOURCE_UNCHANGED", result.unity_status)
                self.assertFalse(result.source_product_changed)

    def test_native_build_auditors_are_product_not_unknown_or_docs(self) -> None:
        for path in ("tools/release/audit_android_apk.py", "tools/release/audit_ios_export.py"):
            with self.subTest(path=path):
                pushed = subject.classify_records(self.records(path), event="push")
                self.assertEqual(("product",), pushed.source_change_class)
                self.assertTrue(pushed.source_product_changed)
                self.assertTrue(pushed.run_jcs)
                self.assertFalse(pushed.requires_unity)
                self.assertEqual("DEFERRED_TO_PR_CANDIDATE", pushed.unity_status)
                candidate = subject.classify_records(self.records(path), event="pull_request")
                self.assertTrue(candidate.requires_unity)
                self.assertTrue(candidate.source_product_changed)
                self.assertEqual("REQUIRED", candidate.unity_status)

    def test_actual_d9_product_range_defers_only_its_push(self) -> None:
        records = self.records(
            "Assets/Plugins/iOS/StpPosixFileBridge.mm",
            "Assets/Plugins/iOS/StpPosixFileBridge.mm.meta",
            "Assets/StammstreckenPuzzle/Scripts/Infrastructure/Persistence/HumanCalibrationStore.cs",
            "Assets/StammstreckenPuzzle/Scripts/Infrastructure/Persistence/LocalDiagnosticRing.cs",
            "Assets/StammstreckenPuzzle/Scripts/Infrastructure/Persistence/SaveFileSystem.cs",
            "tools/release/audit_ios_export.py",
        )
        pushed = subject.classify_records(records, event="push")
        self.assertEqual(("product",), pushed.source_change_class)
        self.assertEqual(("known_product_push_deferred_to_pr_candidate",), pushed.reasons)
        self.assertFalse(pushed.requires_unity)
        candidate = subject.classify_records(records, event="pull_request")
        self.assertTrue(candidate.requires_unity)
        self.assertTrue(candidate.source_product_changed)
        self.assertEqual(("pr_source_product_changed",), candidate.reasons)

    def test_auditor_allowlist_does_not_hide_new_release_inputs(self) -> None:
        for path in (
            "tools/release/audit_ios_export.py.bak", "tools/release/sub/audit_ios_export.py",
            "tools/release/other.py", "tools/release/provider-inputs.json",
        ):
            for event in ("push", "pull_request"):
                with self.subTest(path=path, event=event):
                    result = subject.classify_records(self.records(path), event=event)
                    self.assertEqual(("unknown",), result.source_change_class)
                    self.assertTrue(result.requires_unity)
                    self.assertIn("unknown_diff_fail_closed", result.reasons)

    def test_ci_inspection_report_cannot_hide_product_union_unknown_or_recipe(self) -> None:
        reports = self.records("Release/CI_ANDROID_40143424066_INSPECTION.json")
        for path in (
            "Assets/Plugins/iOS/StpPosixFileBridge.mm",
            "tools/release/audit_ios_export.py",
            "Release/provider-inputs.json",
        ):
            with self.subTest(path=path):
                union = subject.classify_records(reports, event="pull_request", merge_records=self.records(path))
                self.assertTrue(union.requires_unity)
                self.assertTrue(union.pr_merge_product_changed)
                source = subject.classify_records(reports + self.records(path), event="pull_request")
                self.assertTrue(source.requires_unity)
        for event in ("push", "pull_request"):
            self.assertTrue(subject.classify_records(reports, event=event, source_recipe_changed=True).requires_unity)
            self.assertTrue(subject.classify_records(reports, event=event, force_full=True).requires_unity)
        self.assertTrue(subject.classify_records([], event="push").requires_unity)
        self.assertTrue(subject.classify_records(self.records("tools/release/audit_ios_export.py"), event="push", source_recipe_changed=True).requires_unity)

    def test_human_assignment_bindings_still_require_product_candidate(self) -> None:
        records = self.records("Content/Quality/human/assignment-source-bindings.json")
        pushed = subject.classify_records(records, event="push")
        self.assertEqual("DEFERRED_TO_PR_CANDIDATE", pushed.unity_status)
        candidate = subject.classify_records(records, event="pull_request")
        self.assertTrue(candidate.requires_unity)
        self.assertTrue(candidate.source_product_changed)
        self.assertTrue(candidate.run_jcs)

    def test_inspection_names_do_not_hide_inputs_or_unknown_paths(self) -> None:
        for path in (
            "Release/provider-inputs.json", "Release/ANDROID_BUILD0_INSPECTION.json",
            "Release/ANDROID_BUILD01_INSPECTION.json", "Release/ANDROID_BUILD12_INSPECTION.json/inputs",
            "Release/ANDROID_BUILD12_INSPECTION.json.bak", "Release/sub/ANDROID_BUILD12_INSPECTION.json",
            "Content/Quality/human/other-bindings.json", "tools/release/ANDROID_BUILD12_INSPECTION.json",
            "Release/CI_ANDROID_0_INSPECTION.json", "Release/CI_ANDROID_01_INSPECTION.json",
            "Release/CI_ANDROID_123_INSPECTION.json/inputs", "Release/CI_ANDROID_123_INSPECTION.json.bak",
            "Release/sub/CI_ANDROID_123_INSPECTION.json", "Release/CI_IOS_123_INSPECTION.json",
        ):
            with self.subTest(path=path):
                result = subject.classify_records(self.records(path), event="push")
                self.assertEqual("REQUIRED", result.unity_status)
                self.assertIn("unknown_diff_fail_closed", result.reasons)

    def test_inspection_metadata_never_overrides_product_union_or_recipe(self) -> None:
        reports = self.records("Release/ANDROID_BUILD12_INSPECTION.json")
        for records in (
            self.records("Assets/StammstreckenPuzzle/Player.cs"),
            self.records("Content/Quality/human/assignment-source-bindings.json"),
            [subject.Change("R100", ("Release/ANDROID_BUILD12_INSPECTION.json", "Packages/provider.json"))],
        ):
            with self.subTest(records=records):
                result = subject.classify_records(reports, event="pull_request", merge_records=records)
                self.assertTrue(result.requires_unity)
                self.assertTrue(result.pr_merge_product_changed)
        for event in ("push", "pull_request"):
            self.assertTrue(subject.classify_records(reports, event=event, source_recipe_changed=True).requires_unity)
        unknown_move = [subject.Change("R100", ("Release/ANDROID_BUILD12_INSPECTION.json", "Release/provider-inputs.json"))]
        self.assertTrue(subject.classify_records(unknown_move, event="push").requires_unity)

    def test_deferred_push_product_has_honest_status(self) -> None:
        for path in (
            "Assets/StammstreckenPuzzle/Puzzle.cs",
            "Packages/manifest.json",
            "ProjectSettings/ProjectVersion.txt",
            "Toolchain.lock.md",
        ):
            with self.subTest(path=path):
                result = subject.classify_records(self.records(path), event="push")
                self.assertFalse(result.requires_unity)
                self.assertTrue(result.run_jcs)
                self.assertEqual("DEFERRED_TO_PR_CANDIDATE", result.outputs()["unity_status"])
                self.assertNotIn("NOT_REQUIRED_SOURCE_UNCHANGED", result.outputs()["unity_status"])
                self.assertNotIn("PASS", result.outputs()["unity_status"])

    def test_unknown_and_malformed_paths_fail_closed(self) -> None:
        for path in ("new-product-area/file.bin", "../escape.md", "Assets\\backslash.asset"):
            with self.subTest(path=path):
                self.assertTrue(subject.classify_records(self.records(path), event="push").requires_unity)

    def test_rename_and_delete_classify_both_sides(self) -> None:
        renamed_into_assets = [subject.Change("R100", ("PROJECT_CONTROL/old.md", "Assets/new.asset"))]
        deleted_package = [subject.Change("D", ("Packages/manifest.json",))]
        for records in (renamed_into_assets, deleted_package):
            result = subject.classify_records(records, event="push")
            self.assertFalse(result.requires_unity)
            self.assertEqual("DEFERRED_TO_PR_CANDIDATE", result.outputs()["unity_status"])

    def test_workflow_control_only_is_not_recipe_change_but_recipe_change_is(self) -> None:
        workflow = self.records(".github/workflows/unity.yml")
        self.assertFalse(subject.classify_records(workflow, event="push").requires_unity)
        result = subject.classify_records(workflow, event="push", source_recipe_changed=True)
        self.assertTrue(result.requires_unity)
        self.assertEqual("REQUIRED", result.outputs()["unity_status"])

    def test_product_pr_runs_full_candidate(self) -> None:
        product = self.records("Assets/StammstreckenPuzzle/Puzzle.cs")
        result = subject.classify_records(product, event="pull_request")
        self.assertTrue(result.requires_unity)
        self.assertEqual("REQUIRED", result.outputs()["unity_status"])
        self.assertTrue(result.source_product_changed)
        self.assertFalse(result.pr_merge_product_changed)

    def test_coverage_preparer_changes_require_a_measured_pr_candidate(self) -> None:
        records = self.records("tools/ci/prepare_coverage.py")
        candidate = subject.classify_records(records, event="pull_request")
        self.assertTrue(candidate.requires_unity)
        self.assertTrue(candidate.source_product_changed)
        pushed = subject.classify_records(records, event="push")
        self.assertEqual("DEFERRED_TO_PR_CANDIDATE", pushed.outputs()["unity_status"])

    def test_docs_pr_remains_unity_free(self) -> None:
        docs = self.records("PROJECT_CONTROL/CURRENT_STATE.md")
        result = subject.classify_records(docs, event="pull_request")
        self.assertFalse(result.requires_unity)
        self.assertEqual("NOT_REQUIRED_SOURCE_UNCHANGED", result.outputs()["unity_status"])

    def test_changed_union_requires_full_pr_candidate(self) -> None:
        docs = self.records("PROJECT_CONTROL/CURRENT_STATE.md")
        product = self.records("Assets/StammstreckenPuzzle/Puzzle.cs")
        result = subject.classify_records(docs, event="pull_request", merge_records=product)
        self.assertTrue(result.requires_unity)
        self.assertEqual("REQUIRED", result.outputs()["unity_status"])
        self.assertFalse(result.source_product_changed)
        self.assertTrue(result.pr_merge_product_changed)

    def test_true_recipe_change_in_pr_union_remains_fail_closed(self) -> None:
        docs = self.records("PROJECT_CONTROL/CURRENT_STATE.md")
        workflow = self.records(".github/workflows/unity.yml")
        result = subject.classify_records(
            docs, event="pull_request", merge_records=workflow, merge_recipe_changed=True
        )
        self.assertTrue(result.requires_unity)
        self.assertEqual("REQUIRED", result.outputs()["unity_status"])

    def test_manual_force_full_requires_unity(self) -> None:
        result = subject.classify_records(
            self.records("PROJECT_CONTROL/CURRENT_STATE.md"), event="workflow_dispatch", force_full=True
        )
        self.assertTrue(result.requires_unity)
        self.assertTrue(result.run_text)
        self.assertTrue(result.run_jcs)
        self.assertEqual("REQUIRED", result.outputs()["unity_status"])

    def test_missing_or_empty_basis_is_fail_closed(self) -> None:
        self.assertTrue(subject.classify_records([], event="push").requires_unity)
        result = subject.classify_git(
            event="push", base=None, head=None, merge=None, force_full=False, cwd=pathlib.Path(".")
        )
        self.assertTrue(result.requires_unity)
        self.assertEqual("REQUIRED", result.outputs()["unity_status"])

    def test_jcs_changes_are_selective(self) -> None:
        self.assertTrue(
            subject.classify_records(self.records("tools/architecture-validation/jcs_crosscheck.mjs"), event="push").run_jcs
        )
        self.assertFalse(
            subject.classify_records(self.records("PROJECT_CONTROL/CURRENT_STATE.md"), event="push").run_jcs
        )

    def test_worktree_records_include_untracked_files(self) -> None:
        # The production report invokes this path against the actual anchor;
        # here only the pure record classifier is asserted deterministically.
        untracked_product = [subject.Change("A", ("Assets/new-untracked.asset",))]
        result = subject.classify_records(untracked_product, event="push")
        self.assertFalse(result.requires_unity)
        self.assertEqual("DEFERRED_TO_PR_CANDIDATE", result.outputs()["unity_status"])

    def test_control_normalization_preserves_recipes(self) -> None:
        old = """on:\n  push:\n    branches: ['**']\njobs:\n  project-preflight:\n    runs-on: ubuntu-24.04\n    steps:\n      - name: check\n        run: echo old\n"""
        routing_only = """on:\n  push:\n    branches: ['**']\njobs:\n  changes-preflight:\n    outputs:\n      required: ${{ steps.classify.outputs.required }}\n    runs-on: ubuntu-24.04\n    steps:\n      - name: routing\n        run: echo routing\n  project-preflight:\n    needs: [changes-preflight]\n    if: needs.changes-preflight.outputs.requires_unity == 'true'\n    runs-on: ubuntu-24.04\n    steps:\n      - name: check\n        run: echo old\n"""
        changed_recipe = routing_only.replace("echo old", "echo changed")
        changed_step_if = routing_only.replace("run: echo old", "if: always()\n        run: echo old")
        self.assertEqual(subject.normalize_unity_control(old), subject.normalize_unity_control(routing_only))
        self.assertNotEqual(subject.normalize_unity_control(old), subject.normalize_unity_control(changed_recipe))
        self.assertNotEqual(subject.normalize_unity_control(old), subject.normalize_unity_control(changed_step_if))

    def test_actual_unity_workflow_keeps_all_existing_jobs_and_critical_steps(self) -> None:
        root = pathlib.Path(__file__).resolve().parents[2]
        workflow = (root / ".github/workflows/unity.yml").read_text(encoding="utf-8")
        expected_jobs = (
            "project-preflight",
            "unity-config",
            "unity-evidence-guard",
            "unity-environment-linux",
            "unity-environment-android",
            "unity-environment-ios",
            "unity-compile-editmode",
            "unity-playmode-bootstrap-smoke",
            "unity-android-development-il2cpp",
            "unity-ios-export",
        )
        for job in expected_jobs:
            self.assertIn(f"  {job}:\n", workflow, job)
        for step in (
            "Unity-Versionspin und Projektartefakte prüfen",
            "Statischer Modulgraph-Check der realen .asmdef-Dateien",
            "Wahrheitsgemäßer Unity-Nachweisstatus",
            "Unity-Personal-Lizenz aktivieren (Unity Licensing Client)",
            "Unity-Personal-Seat freigeben (best effort)",
            "Domain-/Solver-/Content-EditMode-Ausfuehrung verifizieren",
            "Coverage-Evidenz verifizieren und Baseline je Assembly ausgeben",
            "BootstrapCompositionSmoke (PlayMode, QA-Szene, mit Coverage)",
            "PlayMode-Evidenz mit vorgeschriebenem QA-Szenen-Smoke verifizieren",
            "Vollständigen Android-Candidate bauen und tatsächliches APK prüfen",
            "Vollständigen iOS-Candidate exportieren und Quellen prüfen",
            "Testergebnisse archivieren",
            "Android-Buildartefakt archivieren",
        ):
            self.assertIn(f"- name: {step}", workflow, step)
        self.assertEqual(workflow.count("--activate-all --include-personal"), 6)
        self.assertEqual(workflow.count("--deactivate-all || true"), 4)
        self.assertIn("name: android-device-results", workflow)
        self.assertEqual(workflow.count("actions/upload-artifact@"), 5)

    def test_actual_unity_workflow_has_cheap_preflight_and_gates_all_unity_work(self) -> None:
        root = pathlib.Path(__file__).resolve().parents[2]
        workflow = (root / ".github/workflows/unity.yml").read_text(encoding="utf-8")
        self.assertIn('pull_request:\n    branches: ["**"]', workflow)
        self.assertIn("  changes-preflight:\n", workflow)
        self.assertIn("tools/ci/test_change_scope.py", workflow)
        self.assertIn("tools/ci/change_scope.py", workflow)
        self.assertIn("NOT_REQUIRED_SOURCE_UNCHANGED", workflow)
        self.assertIn("DEFERRED_TO_PR_CANDIDATE", workflow)
        for job in (
            "project-preflight",
            "unity-config",
            "unity-evidence-guard",
            "unity-environment-linux",
            "unity-environment-android",
            "unity-environment-ios",
            "unity-compile-editmode",
            "unity-playmode-bootstrap-smoke",
            "unity-android-development-il2cpp",
            "unity-ios-export",
        ):
            start = workflow.index(f"  {job}:\n")
            match = re.search(r"^  [A-Za-z][A-Za-z0-9_-]*:\n", workflow[start + 1 :], re.MULTILINE)
            end = len(workflow) if match is None else start + 1 + match.start()
            block = workflow[start:end]
            self.assertIn("changes-preflight", block, job)
            self.assertIn("requires_unity == 'true'", block, job)
        self.assertIn("cancel-in-progress: false", workflow)

    def test_candidate_recipe_updates_keep_unaffected_jobs_and_seat_controls(self) -> None:
        root = pathlib.Path(__file__).resolve().parents[2]
        current = (root / ".github/workflows/unity.yml").read_text(encoding="utf-8")
        current_jobs = {name: body for name, body in re.findall(
            r"^  ([a-z][a-z0-9-]+):\n(.*?)(?=^  [a-z][a-z0-9-]+:\n|\Z)", current, re.M | re.S)}
        for job in ("unity-compile-editmode", "unity-playmode-bootstrap-smoke"):
            self.assertIn("tools/ci/prepare_coverage.py", current_jobs[job])
            self.assertIn("tools/ci/verify_coverage.py", current_jobs[job])
        for job in ("unity-compile-editmode", "unity-playmode-bootstrap-smoke",
                    "unity-android-development-il2cpp", "unity-ios-export"):
            block = current_jobs[job]
            self.assertIn("group: unity-personal-seat", block)
            self.assertIn("cancel-in-progress: false", block)
            self.assertIn("--activate-all --include-personal", block)
            self.assertIn("--deactivate-all || true", block)
        for job in ("unity-environment-ios", "unity-ios-export"):
            self.assertIn("bash tools/ci/install-unity-ios.sh", current_jobs[job])
        self.assertIn("-buildTarget Android", current_jobs["unity-android-development-il2cpp"])
        self.assertIn("-executeMethod STP.Editor.Build.MobilePreviewBuild.BuildAndroid", current)
        self.assertIn("tools/release/audit_android_apk.py", current)
        self.assertIn("-buildTarget iOS", current_jobs["unity-ios-export"])
        self.assertIn("-executeMethod STP.Editor.Build.MobilePreviewBuild.ExportIos", current)
        self.assertIn("pod install --project-directory=Builds/MobilePreview/iOS --repo-update", current)
        self.assertIn("-workspace Builds/MobilePreview/iOS/Unity-iPhone.xcworkspace", current)
        self.assertIn("CODE_SIGNING_ALLOWED=NO", current)
        self.assertIn("unsigned-app.zip", current)

    def test_actual_validate_workflow_is_deterministically_wired_and_scoped(self) -> None:
        root = pathlib.Path(__file__).resolve().parents[2]
        workflow = (root / ".github/workflows/validate.yml").read_text(encoding="utf-8")
        self.assertNotIn("\t", workflow)
        self.assertIn("name: Architecture Validation", workflow)
        self.assertIn('pull_request:\n    branches: ["**"]', workflow)
        self.assertIn("tools/ci/test_change_scope.py", workflow)
        self.assertIn("tools/ci/change_scope.py", workflow)
        self.assertIn("if: steps.classify.outputs.run_jcs == 'true'", workflow)
        self.assertEqual(workflow.count("if: steps.classify.outputs.run_text == 'true'"), 2)
        self.assertNotIn("--scope-manifest", workflow)
        self.assertIn("tools/rework/check_satire.py", workflow)
        self.assertIn("tools/rework/rebind_all.py --check", workflow)
        self.assertIn("--self-test", workflow)


if __name__ == "__main__":
    unittest.main(verbosity=2)
