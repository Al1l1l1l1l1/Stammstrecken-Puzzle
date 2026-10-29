"""Controlled source-integrity tests; not genuine Unity coverage measurements."""
from __future__ import annotations

import json
import os
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

import prepare_coverage as subject


class PreparationTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory(prefix="stp-coverage-source-", dir=os.environ.get("STP_TEST_TEMP_ROOT"))
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.cache = self.root / "Library/PackageCache"
        self.package = self.cache / "com.unity.testtools.codecoverage@fixture"
        self.package.mkdir(parents=True)
        self.manifest = self.package / "package.json"
        self.manifest.write_text(json.dumps({"name": "com.unity.testtools.codecoverage", "version": "1.3.0"}))
        self.originals = {}
        self.expected = {}
        rules = []
        for relative, _, _, replacements in subject.RULES:
            text = "\n".join("\n".join([before] * count) for before, _, count in replacements) + "\n"
            if relative == subject.REPORTER:
                text += "\n".join(after for _, after, _ in subject.FULL_REPLACEMENTS) + "\n"
            raw = text.encode()
            prepared = subject.replace_exact(raw, replacements)
            path = self.package / relative
            path.parent.mkdir(parents=True, exist_ok=True)
            path.write_bytes(raw)
            self.originals[relative] = raw
            self.expected[relative] = prepared
            rules.append((relative, subject.digest(raw), subject.digest(prepared), replacements))
        self.rules = tuple(rules)
        self.addCleanup(patch.stopall)
        patch.object(subject, "RULES", self.rules).start()
        patch.object(subject, "FULL", self.rules[0][1]).start()

    def assert_originals(self):
        for relative, raw in self.originals.items():
            self.assertEqual((self.package / relative).read_bytes(), raw)

    def test_all_three_known_sources_are_prepared_and_reported(self):
        rows = subject.prepare(self.root)
        self.assertEqual(len(rows), 3)
        for row in rows:
            raw = (self.package / row["path"]).read_bytes()
            self.assertEqual(raw, self.expected[row["path"]])
            self.assertEqual(subject.digest(raw), row["afterSha256"])

    def test_prepared_source_is_idempotent(self):
        subject.prepare(self.root)
        rows = subject.prepare(self.root)
        self.assertTrue(all(row["beforeSha256"] == row["afterSha256"] for row in rows))

    def test_known_partial_cache_is_completed_without_rewriting_prepared_file(self):
        (self.package / subject.REPORTER).write_bytes(self.expected[subject.REPORTER])
        rows = subject.prepare(self.root)
        self.assertEqual(rows[0]["beforeSha256"], rows[0]["afterSha256"])
        self.assertTrue(all((self.package / p).read_bytes() == raw for p, raw in self.expected.items()))

    def test_upstream_first_gets_exact_existing_full_export(self):
        raw = self.originals[subject.REPORTER]
        for before, after, _ in subject.FULL_REPLACEMENTS:
            raw = raw.replace(after.encode(), before.encode())
        (self.package / subject.REPORTER).write_bytes(raw)
        with patch.object(subject, "UPSTREAM", subject.digest(raw)):
            subject.prepare(self.root)
        self.assertEqual((self.package / subject.REPORTER).read_bytes(), self.expected[subject.REPORTER])

    def test_unknown_third_source_rejects_before_any_write(self):
        relative = self.rules[-1][0]
        raw = self.originals[relative] + b"unknown edit"
        (self.package / relative).write_bytes(raw)
        self.originals[relative] = raw
        with self.assertRaisesRegex(subject.PreparationError, "unknown"):
            subject.prepare(self.root)
        self.assert_originals()

    def test_unknown_result_hash_rejects_before_any_write(self):
        relative, original, _, changes = self.rules[-1]
        with patch.object(subject, "RULES", (*self.rules[:-1], (relative, original, "0" * 64, changes))):
            with self.assertRaisesRegex(subject.PreparationError, "differs"):
                subject.prepare(self.root)
        self.assert_originals()

    def test_replacement_cardinality_rejects_before_any_write(self):
        relative, original, prepared, changes = self.rules[-1]
        before, after, count = changes[0]
        with patch.object(subject, "RULES", (*self.rules[:-1], (relative, original, prepared, ((before, after, count + 1),)))):
            with self.assertRaisesRegex(subject.PreparationError, "count"):
                subject.prepare(self.root)
        self.assert_originals()

    def test_missing_source_rejects_before_any_write(self):
        relative = self.rules[-1][0]
        (self.package / relative).unlink()
        self.originals.pop(relative)
        with self.assertRaisesRegex(subject.PreparationError, "missing"):
            subject.prepare(self.root)
        self.assert_originals()

    def test_multiple_packages_reject_before_any_write(self):
        (self.cache / "com.unity.testtools.codecoverage@other").mkdir()
        with self.assertRaisesRegex(subject.PreparationError, "exactly"):
            subject.prepare(self.root)
        self.assert_originals()

    def test_other_name_or_version_reject_before_any_write(self):
        for name, version in (("different", "1.3.0"), ("com.unity.testtools.codecoverage", "1.3.1")):
            with self.subTest(name=name, version=version):
                self.manifest.write_text(json.dumps({"name": name, "version": version}))
                with self.assertRaisesRegex(subject.PreparationError, "exactly"):
                    subject.prepare(self.root)
                self.assert_originals()

    def test_linked_source_or_cache_is_rejected(self):
        for linked in (self.package / self.rules[-1][0], self.cache):
            with self.subTest(linked=str(linked)):
                with patch.object(Path, "is_symlink", lambda path: path == linked):
                    with self.assertRaisesRegex(subject.PreparationError, "link"):
                        subject.prepare(self.root)
                self.assert_originals()

    def test_mid_write_failure_restores_original_sources(self):
        write = Path.write_bytes
        failing = self.package / self.rules[1][0]
        failed = False

        def write_once(path, data):
            nonlocal failed
            if path == failing and not failed:
                failed = True
                write(path, b"partial failed write")
                raise OSError("controlled write failure")
            return write(path, data)

        with patch.object(Path, "write_bytes", write_once):
            with self.assertRaisesRegex(subject.PreparationError, "restored"):
                subject.prepare(self.root)
        self.assert_originals()


if __name__ == "__main__":
    unittest.main()
