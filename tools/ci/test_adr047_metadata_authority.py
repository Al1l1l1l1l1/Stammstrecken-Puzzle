#!/usr/bin/env python3
"""Controlled complete contexts and mocked read-only Git; never invokes Git."""
from __future__ import annotations

from dataclasses import replace
import json
from pathlib import Path
import subprocess
import sys
import unittest
from unittest import mock

sys.dont_write_bytecode = True
sys.path.insert(0, str(Path(__file__).resolve().parent))
import adr047_metadata_authority as subject  # noqa: E402


AUTH = "a" * 40
AFTER = "b" * 40


def _repository() -> Path:
    for parent in Path(__file__).resolve().parents:
        if (parent / subject.MANIFEST_PATH).is_file():
            return parent
    raise AssertionError("Repository source fixtures unavailable")


def _record_bytes(value: dict[str, object]) -> bytes:
    return (json.dumps(value, ensure_ascii=False, indent=2) + "\n").encode("utf-8")


class MetadataAuthorityTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls) -> None:
        repository = _repository()
        current = (repository / subject.TARGET_PATH).read_bytes()
        # After integration the same historical prefix remains the fixture.
        cls.old = (current[:-len(subject.APPEND_BYTES)]
                   if current.endswith(subject.APPEND_BYTES) else current)
        cls.expected = subject.expected_record(cls.old)
        cls.record = _record_bytes(cls.expected)
        cls.manifest = (repository / subject.MANIFEST_PATH).read_bytes()
        cls.adr = (repository / subject.ACCEPTED_ADR_PATH).read_bytes()

    def context(self, **changes: object) -> subject.MetadataAuthorityContext:
        context = subject.MetadataAuthorityContext(
            changed_paths=(subject.TARGET_PATH,), manifest_path=subject.MANIFEST_PATH,
            work_package_id="WP-046", requested_scope="production",
            manifest_at_anchor=self.manifest, manifest_at_source_base=self.manifest,
            manifest_at_authorization=self.manifest, manifest_current=self.manifest,
            target_at_manifest_base=self.old, target_at_source_base=self.old,
            target_at_authorization_parent=self.old, target_at_authorization=self.old,
            target_current=self.old + subject.APPEND_BYTES,
            accepted_adr_at_source_base=None, accepted_adr_at_authorization=self.adr,
            accepted_adr_current=self.adr, record_at_add=self.record, record_current=self.record,
            authorization_add_commits=(AUTH,), authorization_parents=(subject.SOURCE_BASE,),
            authorization_is_ancestor_of_head=True,
            scope_anchor_is_ancestor_of_source_base=True, target_changes=(),
            record_history=((self.record, "100644"),),
            target_modes=("100644",) * 5, record_modes=("100644", "100644"))
        return replace(context, **changes)

    def denied(self, **changes: object) -> None:
        with self.assertRaises(subject.MetadataAuthorityError):
            subject.authorize_context(self.context(**changes))

    def test_exact_planned_worktree_append_after_auth_before_append_commit(self) -> None:
        self.assertEqual(subject.TARGET_PATH, subject.authorize_context(self.context()))
        self.assertEqual(17, len(self.expected))
        self.assertEqual(self.old, self.context().target_current[:-len(subject.APPEND_BYTES)])

    def test_committed_append_strictly_after_authorization(self) -> None:
        self.assertEqual(subject.TARGET_PATH, subject.authorize_context(
            self.context(target_changes=((AFTER, True),))))

    def test_only_target_is_returned_and_other_paths_receive_no_authority(self) -> None:
        for paths in ((), ("DECISIONS/ADR-024-other.md",), (subject.RECORD_PATH,),
                      ("Assets/secret.cs", "../outside")):
            with self.subTest(paths=paths):
                self.assertIsNone(subject.authorize_context(self.context(changed_paths=paths)))
        self.assertEqual(subject.TARGET_PATH, subject.authorize_context(self.context(
            changed_paths=(subject.TARGET_PATH, "DECISIONS/ADR-024-other.md"))))

    def test_wrong_scope_wp_or_manifest_path_denied(self) -> None:
        for change in ({"manifest_path": "other.json"}, {"work_package_id": "WP-045"},
                       {"requested_scope": "documentation"}):
            with self.subTest(change=change):
                self.denied(**change)

    def test_every_manifest_role_must_match_original_raw_bytes(self) -> None:
        for role in ("manifest_at_anchor", "manifest_at_source_base",
                     "manifest_at_authorization", "manifest_current"):
            with self.subTest(role=role):
                self.denied(**{role: self.manifest + b"\n"})
        self.denied(scope_anchor_is_ancestor_of_source_base=False)

    def test_missing_multiple_or_malformed_add_anchor_denied(self) -> None:
        for additions in ((), (AUTH, AFTER), ("not-a-commit",)):
            with self.subTest(additions=additions):
                self.denied(authorization_add_commits=additions)

    def test_exact_single_parent_and_reachable_authorization_required(self) -> None:
        for parents in ((), (AFTER,), (subject.SOURCE_BASE, AFTER)):
            with self.subTest(parents=parents):
                self.denied(authorization_parents=parents)
        self.denied(authorization_is_ancestor_of_head=False)
        self.denied(authorization_is_ancestor_of_head="true")

    def test_untracked_missing_or_current_mutated_record_denied(self) -> None:
        self.denied(record_at_add=None)
        self.denied(record_current=None)
        self.denied(record_current=self.record + b"\n")

    def test_mutated_then_restored_record_history_is_still_denied(self) -> None:
        self.denied(record_history=())
        self.denied(record_history=((self.record + b"\n", "100644"),
                                    (self.record, "100644")))
        self.denied(record_history=((self.record, "100755"),))

    def test_every_record_field_is_closed_and_exact(self) -> None:
        for key in self.expected:
            value = dict(self.expected)
            value[key] = 99 if key == "schemaVersion" else "WRONG"
            raw = _record_bytes(value)
            with self.subTest(key=key):
                self.denied(record_at_add=raw, record_current=raw,
                            record_history=((raw, "100644"),))
        for value in ({key: val for key, val in self.expected.items() if key != "mandate"},
                      dict(self.expected, extraAllowedPaths=["DECISIONS/**"])):
            raw = _record_bytes(value)
            self.denied(record_at_add=raw, record_current=raw,
                        record_history=((raw, "100644"),))

    def test_duplicate_json_key_boolean_version_float_invalid_utf8_denied(self) -> None:
        invalid = [b'{"schemaVersion":1,"schemaVersion":1}', b"[]", b"null", b"{", b"\xff",
                   self.record.replace(b'"schemaVersion": 1', b'"schemaVersion": true'),
                   self.record.replace(b'"schemaVersion": 1', b'"schemaVersion": 1.0'),
                   self.record.replace(b'"schemaVersion": 1', b'"schemaVersion": NaN')]
        for raw in invalid:
            with self.subTest(raw=raw[:70]):
                self.denied(record_at_add=raw, record_current=raw,
                            record_history=((raw, "100644"),))

    def test_all_historical_target_roles_preserve_exact_original(self) -> None:
        for role in ("target_at_manifest_base", "target_at_source_base",
                     "target_at_authorization_parent", "target_at_authorization"):
            with self.subTest(role=role):
                self.denied(**{role: self.old + b"\n"})

    def test_simultaneous_authorization_and_append_denied(self) -> None:
        self.denied(target_at_authorization=self.old + subject.APPEND_BYTES)
        self.denied(target_changes=((AUTH, False),))

    def test_prior_sibling_posthoc_or_unknown_target_change_denied(self) -> None:
        for changes in (((AFTER, False),), (("invalid", True),), ((AUTH, True),),
                        ((AFTER, "true"),)):
            with self.subTest(changes=changes):
                self.denied(target_changes=changes)

    def test_accepted_adr_original_required_in_all_roles(self) -> None:
        self.denied(accepted_adr_at_source_base=self.adr)
        for role in ("accepted_adr_at_authorization", "accepted_adr_current"):
            with self.subTest(role=role):
                self.denied(**{role: self.adr + b"\n"})

    def test_changed_old_body_eol_or_any_different_append_denied(self) -> None:
        for raw in (self.old, self.old + subject.APPEND_BYTES + b"\n",
                    self.old + subject.APPEND_BYTES * 2,
                    self.old.replace(b"\n", b"\r\n") + subject.APPEND_BYTES,
                    self.old + subject.APPEND_BYTES.replace(b"ADR-047", b"ADR-048"),
                    b"X" + self.old[1:] + subject.APPEND_BYTES):
            with self.subTest(length=len(raw)):
                self.denied(target_current=raw)

    def test_file_modes_cannot_expand_append_into_chmod_or_symlink(self) -> None:
        for index in range(5):
            modes = list(self.context().target_modes)
            modes[index] = "120000" if index % 2 else "100755"
            with self.subTest(index=index):
                self.denied(target_modes=tuple(modes))
        self.denied(target_modes=())
        self.denied(record_modes=("100644", "100755"))
        self.denied(record_modes=())

    def fake_git(self):
        owner = self

        class FakeGit:
            def __init__(self, _: Path):
                pass

            def text(self, *args: str) -> str:
                if args[0] == "log":
                    return AUTH
                if args[:3] == ("rev-list", "--parents", "-n"):
                    return f"{AUTH} {subject.SOURCE_BASE}"
                if args[-1] == subject.RECORD_PATH:
                    return AUTH
                if args[0] == "rev-list":
                    return ""  # Actual working-tree append, not committed yet.
                raise AssertionError(args)

            def blob(self, commit: str, path: str) -> bytes:
                if path == subject.MANIFEST_PATH:
                    return owner.manifest
                if path == subject.TARGET_PATH:
                    return owner.old
                if path == subject.RECORD_PATH:
                    return owner.record
                if path == subject.ACCEPTED_ADR_PATH:
                    return owner.adr
                raise AssertionError((commit, path))

            def ancestor(self, _: str, __: str) -> bool:
                return True

            def optional_blob(self, commit: str, path: str) -> bytes | None:
                owner.assertEqual(subject.SOURCE_BASE, commit)
                owner.assertEqual(subject.ACCEPTED_ADR_PATH, path)
                return None

            def tree_mode(self, _: str, __: str) -> str:
                return "100644"

            def index_mode(self, _: str) -> str:
                return "100644"

        return FakeGit

    def test_adapter_assembles_complete_context_for_uncommitted_append(self) -> None:
        files = {subject.MANIFEST_PATH: self.manifest, subject.TARGET_PATH: self.old + subject.APPEND_BYTES,
                 subject.RECORD_PATH: self.record, subject.ACCEPTED_ADR_PATH: self.adr}
        with mock.patch.object(subject, "_Git", self.fake_git()), mock.patch.object(
                subject, "_read_regular", side_effect=lambda root, path: files[path]):
            self.assertEqual(subject.TARGET_PATH, subject.authorized_metadata_path(
                _repository(), [subject.TARGET_PATH], manifest_path=subject.MANIFEST_PATH,
                work_package_id="WP-046", requested_scope="production"))

    def test_adapter_untracked_record_is_not_authority(self) -> None:
        fake = mock.Mock()
        fake.text.return_value = ""
        with mock.patch.object(subject, "_Git", return_value=fake):
            with self.assertRaisesRegex(subject.MetadataAuthorityError, "record-add-count"):
                subject.authorized_metadata_path(
                    _repository(), [subject.TARGET_PATH], manifest_path=subject.MANIFEST_PATH,
                    work_package_id="WP-046", requested_scope="production")
        fake.blob.assert_not_called()

    def test_adapter_unaffected_path_does_not_read_files_or_git(self) -> None:
        with mock.patch.object(subject, "_Git") as git:
            self.assertIsNone(subject.authorized_metadata_path(
                Path("missing"), ["DECISIONS/ADR-024.md"], manifest_path="not-consulted",
                work_package_id="other", requested_scope="other"))
            git.assert_not_called()

    def test_adapter_missing_file_io_or_git_error_fails_closed(self) -> None:
        with mock.patch.object(subject, "_Git", self.fake_git()), mock.patch.object(
                subject, "_read_regular", side_effect=OSError("controlled unreadable")):
            with self.assertRaisesRegex(subject.MetadataAuthorityError, "incomplete-readback"):
                subject.authorized_metadata_path(
                    _repository(), [subject.TARGET_PATH], manifest_path=subject.MANIFEST_PATH,
                    work_package_id="WP-046", requested_scope="production")

    def test_git_commands_are_readonly_literal_and_optional_locks_disabled(self) -> None:
        calls: list[list[str]] = []

        def runner(command: list[str], **kwargs: object):
            calls.append(command)
            self.assertEqual("0", kwargs["env"]["GIT_OPTIONAL_LOCKS"])
            self.assertIn("--literal-pathspecs", command)
            if "ls-tree" in command:
                output = f"100644 blob {'c' * 40}\t{subject.TARGET_PATH}\0".encode()
            elif "ls-files" in command:
                output = f"100644 {'c' * 40} 0\t{subject.TARGET_PATH}\0".encode()
            else:
                output = b"controlled\n"
            return subprocess.CompletedProcess(command, 0, stdout=output, stderr=b"")

        with mock.patch.object(subject.subprocess, "run", side_effect=runner):
            git = subject._Git(_repository())
            self.assertEqual("controlled", git.text("log", "--format=%H"))
            self.assertEqual(b"controlled\n", git.blob(AUTH, subject.TARGET_PATH))
            self.assertTrue(git.ancestor(AUTH, "HEAD"))
            self.assertEqual("100644", git.tree_mode(AUTH, subject.TARGET_PATH))
            self.assertEqual("100644", git.index_mode(subject.TARGET_PATH))
        self.assertEqual(5, len(calls))
        self.assertTrue(all(not ({"add", "commit", "checkout", "reset", "update-ref"} & set(call))
                            for call in calls))

    def test_git_unmerged_index_or_nonliteral_tree_path_denied(self) -> None:
        bad_rows = [b"", f"100644 blob {'c' * 40}\tother.md\0".encode(),
                    f"100644 blob {'c' * 40}\t{subject.TARGET_PATH}\0".encode() * 2]
        for row in bad_rows:
            with self.subTest(row=row[:50]), mock.patch.object(subject.subprocess, "run", return_value=
                    subprocess.CompletedProcess([], 0, stdout=row, stderr=b"")):
                with self.assertRaises(subject.MetadataAuthorityError):
                    subject._Git(_repository()).tree_mode(AUTH, subject.TARGET_PATH)
        row = f"100644 {'c' * 40} 2\t{subject.TARGET_PATH}\0".encode()
        with mock.patch.object(subject.subprocess, "run", return_value=
                subprocess.CompletedProcess([], 0, stdout=row, stderr=b"")):
            with self.assertRaises(subject.MetadataAuthorityError):
                subject._Git(_repository()).index_mode(subject.TARGET_PATH)

    def test_optional_historical_absence_requires_successful_tree_lookup(self) -> None:
        with mock.patch.object(subject.subprocess, "run", return_value=
                subprocess.CompletedProcess([], 0, stdout=b"", stderr=b"")):
            self.assertIsNone(subject._Git(_repository()).optional_blob(
                subject.SOURCE_BASE, subject.ACCEPTED_ADR_PATH))
        with mock.patch.object(subject.subprocess, "run", return_value=
                subprocess.CompletedProcess([], 128, stdout=b"", stderr=b"controlled failure")):
            with self.assertRaises(subject.MetadataAuthorityError):
                subject._Git(_repository()).optional_blob(subject.SOURCE_BASE, subject.ACCEPTED_ADR_PATH)


if __name__ == "__main__":
    unittest.main(verbosity=2)
