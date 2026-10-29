#!/usr/bin/env python3
"""Transport-only fixtures; no real network, Git, approval, Unity or signing."""
from __future__ import annotations

from contextlib import redirect_stderr, redirect_stdout
from dataclasses import replace
import hashlib
import io
import json
import os
from pathlib import Path
import shutil
import stat
import struct
import subprocess
import sys
import tempfile
import unittest
from unittest import mock
import urllib.error
import warnings
import uuid
import zipfile

sys.dont_write_bytecode = True
sys.path.insert(0, str(Path(__file__).resolve().parent))
import rc_ci_inputs as subject  # noqa: E402

COMMIT = "a" * 40
REPO = "Example/TrainPuzzle"
TAG = "refs/tags/v1.2.3-rc.1"
APP = "com.example.trainpuzzle"
PROFILE = "Assets/StammstreckenPuzzle/Settings/Production.asset"
REQUEST = "Content/Import/request.json"
APPROVAL = "Content/Import/approval.json"
REVIEW = b'{"kind":"FIXTURE_ONLY","status":"PASS","role":"not-an-approval"}\n'


def encoded(value) -> bytes:
    return (json.dumps(value, sort_keys=True) + "\n").encode()


def zipped(entries, *, compression=zipfile.ZIP_DEFLATED) -> bytes:
    buffer = io.BytesIO()
    with warnings.catch_warnings():
        warnings.simplefilter("ignore", UserWarning)  # Intentional duplicate fixture.
        with zipfile.ZipFile(buffer, "w", compression) as archive:
            for name, raw in entries:
                archive.writestr(name, raw)
    return buffer.getvalue()


class FixtureCheckout(subject.GitCheckout):
    """Serve controlled immutable Git blobs; exercise the actual verification code."""
    def __init__(self, root: Path, blobs: dict[str, bytes]):
        super().__init__(root)
        self.blobs = blobs
        self.commit = COMMIT
        self.tag_commit = COMMIT
        self.tag_type = b"tag"
        self.origin = "https://github.com/" + REPO + ".git"
        self.mode = "100644"
        self.calls = []

    def _git(self, *args: str) -> bytes:
        self.calls.append(args)
        if args == ("rev-parse", "--show-toplevel"):
            return str(self.root).encode()
        if args == ("remote", "get-url", "origin"):
            return self.origin.encode()
        if args == ("rev-parse", "--verify", "HEAD"):
            return self.commit.encode()
        if args == ("cat-file", "-t", TAG):
            return self.tag_type
        if args == ("rev-parse", "--verify", TAG + "^{commit}"):
            return self.tag_commit.encode()
        if args[:3] == ("ls-tree", "-z", COMMIT):
            relative = args[-1]
            if relative not in self.blobs:
                return b""
            oid = hashlib.sha1(self.blobs[relative]).hexdigest()
            return f"{self.mode} blob {oid}\t{relative}\0".encode()
        if args[:2] in (("cat-file", "-s"), ("cat-file", "blob")):
            for raw in self.blobs.values():
                if hashlib.sha1(raw).hexdigest() == args[-1]:
                    return str(len(raw)).encode() if args[1] == "-s" else raw
        raise AssertionError(args)


class Response(io.BytesIO):
    def __init__(self, raw=b"", *, code=200, headers=None):
        super().__init__(raw)
        self.code = code
        self.headers = headers or {}


class RcInputsTests(unittest.TestCase):
    def setUp(self) -> None:
        self.claimed = False
        self.root = Path(tempfile.gettempdir()).resolve() / ("stp-rc-inputs-test-" + uuid.uuid4().hex)
        self.assertFalse(self.root.exists())
        self.root.mkdir()  # Inherit the test runner's Windows ACL.
        self.claimed = True
        self.checkout_root = self.root / "checkout"
        self.checkout_root.mkdir()
        self.runner = self.root / "runner"
        self.runner.mkdir()
        self.github_env = self.runner / "github-env"
        self.github_env.write_bytes(b"PREVIOUS=unchanged\n")
        self.blobs = {REQUEST: b'{"fixtureOnly":true}\n',
                      APPROVAL: b'{"fixtureOnly":"not-production"}\n',
                      PROFILE: b"fixtureOnly: not-a-production-profile\n"}
        for relative, raw in self.blobs.items():
            path = self.checkout_root / relative
            path.parent.mkdir(parents=True, exist_ok=True)
            path.write_bytes(raw)
        self.checkout = FixtureCheckout(self.checkout_root, self.blobs)
        self.environment = dict(zip(subject.ENV_NAMES, (
            ".", REQUEST, APPROVAL, subject.sha(self.blobs[APPROVAL]),
            PROFILE, subject.sha(self.blobs[PROFILE]), subject.IAP_ORIGINAL, subject.sha(REVIEW),
            "fixture-policy-v1", "fixture-sdk-1.0", "https://publisher.example/legal",
            "https://publisher.example/support")))
        self.descriptor = {"schemaVersion": 1, "platform": "ANDROID", "gitCommit": COMMIT,
                           "applicationIdentifier": APP, "environment": self.environment}
        self.archive = self.make_archive()
        self.expected = subject.Expected(REPO, 123, TAG, COMMIT, "ANDROID", APP, 456,
                                         subject.sha(self.archive))

    def tearDown(self) -> None:
        # Only our successfully claimed directory; readonly extracted originals
        # are made writable only inside that controlled test root for Windows cleanup.
        for path in self.root.rglob("*"):
            if path.is_file() and not path.is_symlink():
                path.chmod(0o600)
        if self.claimed:
            self.assertEqual(self.root, self.root.resolve())
            self.assertTrue(self.root.name.startswith("stp-rc-inputs-test-"))
            shutil.rmtree(self.root)
            self.claimed = False

    def make_archive(self, descriptor=None, review=REVIEW) -> bytes:
        entries = [(subject.DESCRIPTOR, encoded(self.descriptor if descriptor is None else descriptor))]
        if review is not None:
            entries.append((subject.IAP_ORIGINAL, review))
        return zipped(entries)

    def run_prepare(self, raw=None, expected=None):
        raw = self.archive if raw is None else raw
        expected = self.expected if expected is None else expected
        return subject.prepare(raw, expected, self.checkout, self.runner, self.github_env)

    def descriptor_denied(self, change) -> None:
        descriptor = json.loads(encoded(self.descriptor))
        change(descriptor)
        raw = self.make_archive(descriptor)
        with self.assertRaises(subject.InputError):
            self.run_prepare(raw, replace(self.expected, zip_sha256=subject.sha(raw)))
        self.assertEqual(b"PREVIOUS=unchanged\n", self.github_env.read_bytes())
        self.assertEqual([], list(self.runner.glob("stp-rc-inputs-*")))

    def metadata(self):
        run = {"id": 123, "head_sha": COMMIT, "head_branch": "v1.2.3-rc.1",
               "status": "completed", "conclusion": "success",
               "repository": {"id": 11, "full_name": REPO},
               "head_repository": {"id": 11, "full_name": REPO}}
        artifact = {"id": 456, "expired": False, "size_in_bytes": len(self.archive),
                    "digest": "sha256:" + self.expected.zip_sha256,
                    "workflow_run": {"id": 123, "repository_id": 11, "head_repository_id": 11,
                                     "head_sha": COMMIT, "head_branch": "v1.2.3-rc.1"}}
        return artifact, run

    def test_positive_exact_twelve_values_and_readonly_original_no_source_overlay(self) -> None:
        result = self.run_prepare()
        self.assertFalse(result["productionOrExternalApproval"])
        lines = self.github_env.read_text().splitlines()
        self.assertEqual(13, len(lines))
        values = dict(line.split("=", 1) for line in lines[1:])
        self.assertEqual(set(subject.ENV_NAMES), set(values))
        self.assertEqual(str(self.checkout_root), values[subject.ENV_NAMES[0]])
        original = Path(values[subject.IAP_NAMES[0]])
        self.assertEqual(REVIEW, original.read_bytes())
        self.assertFalse(original.stat().st_mode & stat.S_IWUSR)
        self.assertEqual({subject.DESCRIPTOR, subject.IAP_ORIGINAL},
                         {p.name for p in original.parent.iterdir()})
        self.assertEqual(self.blobs, {relative: (self.checkout_root / relative).read_bytes()
                                     for relative in self.blobs})
        self.assertIn(("rev-parse", "--verify", TAG + "^{commit}"), self.checkout.calls)

    def test_optional_original_absent_emits_empty_group_and_no_fake_approval(self) -> None:
        descriptor = json.loads(encoded(self.descriptor))
        for name in subject.IAP_NAMES:
            descriptor["environment"][name] = ""
        raw = self.make_archive(descriptor, review=None)
        result = self.run_prepare(raw, replace(self.expected, zip_sha256=subject.sha(raw)))
        self.assertEqual(1, result["files"])
        self.assertFalse(result["productionOrExternalApproval"])
        for name in subject.IAP_NAMES:
            self.assertIn(name + "=\n", self.github_env.read_text())

    def test_false_raw_sha_is_rejected_before_zip_parse_or_output(self) -> None:
        with mock.patch.object(subject.zipfile, "ZipFile") as parser:
            with self.assertRaisesRegex(subject.InputError, "ZIP_RAW_HASH_MISMATCH"):
                self.run_prepare(expected=replace(self.expected, zip_sha256="0" * 64))
            parser.assert_not_called()
        self.assertEqual([], list(self.runner.glob("stp-rc-inputs-*")))
        self.assertEqual(b"PREVIOUS=unchanged\n", self.github_env.read_bytes())

    def test_wrong_descriptor_identity_version_extra_field_or_environment_denied(self) -> None:
        for key, value in (("platform", "IOS"), ("gitCommit", "b" * 40),
                           ("applicationIdentifier", "com.other.app"), ("schemaVersion", True),
                           ("schemaVersion", 2), ("arbitrary", "data")):
            with self.subTest(key=key):
                self.descriptor_denied(lambda d, k=key, v=value: d.__setitem__(k, v))
        self.descriptor_denied(lambda d: d["environment"].__setitem__("GITHUB_TOKEN", "FAKE"))
        self.descriptor_denied(lambda d: d["environment"].pop(subject.ENV_NAMES[0]))

    def test_source_root_paths_aliases_and_hashes_are_closed(self) -> None:
        for value in ("/other", "C:/other", "..", "./", "somewhere"):
            with self.subTest(root=value):
                self.descriptor_denied(lambda d, v=value: d["environment"].__setitem__(subject.ENV_NAMES[0], v))
        for value in ("../request.json", "Content\\request.json", "/request.json", "C:/request.json",
                      "Content//request.json", "Content/./request.json", "Content/CON.json"):
            with self.subTest(path=value):
                self.descriptor_denied(lambda d, v=value: d["environment"].__setitem__(subject.ENV_NAMES[1], v))
        self.descriptor_denied(lambda d: d["environment"].__setitem__(subject.ENV_NAMES[2], REQUEST))
        for name in (subject.ENV_NAMES[3], subject.ENV_NAMES[5], subject.IAP_NAMES[1]):
            self.descriptor_denied(lambda d, n=name: d["environment"].__setitem__(n, "0" * 64))

    def test_newline_nul_nonascii_or_unknown_env_cannot_reach_github_env(self) -> None:
        for value in ("v1\nGITHUB_TOKEN=FAKE", "v1\rX=Y", "v1\0secret", "v1\u2028X=Y", " v1"):
            with self.subTest(value=repr(value)):
                self.descriptor_denied(lambda d, v=value: d["environment"].__setitem__(subject.IAP_NAMES[2], v))

    def test_iap_path_header_and_nonjson_binary_do_not_supply_gate_authority(self) -> None:
        self.descriptor_denied(lambda d: d["environment"].__setitem__(subject.IAP_NAMES[0], APPROVAL))
        binary = b"\x30\x82\xff\x00not-a-json-review"
        descriptor = json.loads(encoded(self.descriptor))
        descriptor["environment"][subject.IAP_NAMES[1]] = subject.sha(binary)
        raw = self.make_archive(descriptor, review=binary)
        with self.assertRaises(subject.InputError):
            self.run_prepare(raw, replace(self.expected, zip_sha256=subject.sha(raw)))
        # A JSON PASS/role header is transported byte-for-byte, never promoted.
        self.assertFalse(self.run_prepare()["productionOrExternalApproval"])

    def test_tag_commit_origin_type_and_modified_tracked_file_are_rejected(self) -> None:
        for attribute, wrong in (("commit", "b" * 40), ("tag_commit", "b" * 40),
                                 ("tag_type", b"commit"), ("origin", "https://evil.test/other.git"),
                                 ("mode", "120000")):
            previous = getattr(self.checkout, attribute)
            setattr(self.checkout, attribute, wrong)
            with self.subTest(attribute=attribute), self.assertRaises(subject.InputError):
                self.run_prepare()
            setattr(self.checkout, attribute, previous)
        (self.checkout_root / APPROVAL).write_bytes(b"modified checkout bytes")
        with self.assertRaisesRegex(subject.InputError, "CHECKOUT_FILE_BYTES_MISMATCH"):
            self.run_prepare()
        self.assertEqual(b"PREVIOUS=unchanged\n", self.github_env.read_bytes())

    def test_source_change_after_initial_validation_outputs_nothing(self) -> None:
        original = self.checkout.verify
        calls = 0

        def changing(expected):
            nonlocal calls
            calls += 1
            original(expected)
            if calls == 2:
                (self.checkout_root / REQUEST).write_bytes(b"changed during extraction")

        with mock.patch.object(self.checkout, "verify", side_effect=changing):
            with self.assertRaises(subject.InputError):
                self.run_prepare()
        self.assertEqual(b"PREVIOUS=unchanged\n", self.github_env.read_bytes())

    def test_fresh_private_directory_collision_does_not_touch_existing_sentinel(self) -> None:
        collision = self.runner / ("stp-rc-inputs-" + "c" * 32)
        collision.mkdir()
        sentinel = collision / "sentinel"
        sentinel.write_bytes(b"owned elsewhere")
        with mock.patch.object(subject.uuid, "uuid4", return_value=mock.Mock(hex="c" * 32)):
            with self.assertRaisesRegex(subject.InputError, "PRIVATE_DIRECTORY_EXISTS"):
                self.run_prepare()
        self.assertEqual(b"owned elsewhere", sentinel.read_bytes())
        self.assertEqual(b"PREVIOUS=unchanged\n", self.github_env.read_bytes())

    def test_env_or_extraction_destination_cannot_be_inside_tagged_checkout(self) -> None:
        with self.assertRaises(subject.InputError):
            subject.prepare(self.archive, self.expected, self.checkout, self.checkout_root, self.github_env)
        destination = self.checkout_root / REQUEST
        with self.assertRaises(subject.InputError):
            subject.prepare(self.archive, self.expected, self.checkout, self.runner, destination)
        self.assertEqual(self.blobs[REQUEST], destination.read_bytes())

    def test_publisher_urls_follow_existing_public_https_config_boundary(self) -> None:
        for url in ("http://publisher.example/legal", "https://user:pass@publisher.example/legal",
                    "https://127.0.0.1/legal", "https://localhost/legal", "https://publisher.example:444/legal",
                    "https://publisher.example/legal?user=42", "https://publisher.example/legal\nX=Y"):
            with self.subTest(url=url):
                self.descriptor_denied(lambda d, v=url: d["environment"].__setitem__(subject.ENV_NAMES[-1], v))

    def test_hostile_zip_names_duplicates_case_unicode_or_additional_files(self) -> None:
        names = ("../rc-inputs.json", "/rc-inputs.json", "a/rc-inputs.json", "a\\rc-inputs.json",
                 "RC-INPUTS.JSON", "rc-inputs.json/", "r\u00e4tsel.json", "key.p12", "NUL")
        for name in names:
            raw = zipped([(name, b"{}")])
            with self.subTest(name=name), self.assertRaises(subject.InputError):
                subject.archive_files(raw, subject.sha(raw))
        for entries in ([(subject.DESCRIPTOR, b"{}"), (subject.DESCRIPTOR, b"{}")],
                        [(subject.DESCRIPTOR, b"{}"), ("RC-INPUTS.JSON", b"{}")],
                        [(subject.DESCRIPTOR, b"{}"), (subject.IAP_ORIGINAL, b"{}"), ("extra.json", b"{}")]):
            raw = zipped(entries)
            with self.assertRaises(subject.InputError):
                subject.archive_files(raw, subject.sha(raw))

    def test_hostile_zip_symlink_reparse_directory_and_encryption_flags(self) -> None:
        for attributes in ((stat.S_IFLNK | 0o777) << 16, 0x400 | (stat.S_IFREG << 16), 0x10):
            entry = zipfile.ZipInfo(subject.DESCRIPTOR)
            entry.create_system = 3
            entry.external_attr = attributes
            raw = zipped([(entry, b"{}")])
            with self.subTest(attributes=attributes), self.assertRaises(subject.InputError):
                subject.archive_files(raw, subject.sha(raw))
        raw = bytearray(zipped([(subject.DESCRIPTOR, b"{}")]))
        central = raw.index(b"PK\x01\x02")
        struct.pack_into("<H", raw, central + 8, 1)
        struct.pack_into("<H", raw, 6, 1)
        with self.assertRaises(subject.InputError):
            subject.archive_files(bytes(raw), subject.sha(raw))

    def test_zip_bomb_size_ratio_total_trailing_prefix_and_crc_are_rejected(self) -> None:
        bomb = zipped([(subject.DESCRIPTOR, b"A" * 1000000)])
        with self.assertRaisesRegex(subject.InputError, "ZIP_FILE_SIZE_OR_RATIO"):
            subject.archive_files(bomb, subject.sha(bomb))
        normal = zipped([(subject.DESCRIPTOR, b"{}")], compression=zipfile.ZIP_STORED)
        too_big = bytearray(normal)
        central = too_big.index(b"PK\x01\x02")
        struct.pack_into("<I", too_big, central + 24, subject.MAX_FILE + 1)
        with self.assertRaises(subject.InputError):
            subject.archive_files(bytes(too_big), subject.sha(too_big))
        with mock.patch.object(subject, "MAX_TOTAL", 1):
            with self.assertRaisesRegex(subject.InputError, "ZIP_TOTAL_SIZE"):
                subject.archive_files(normal, subject.sha(normal))
        for raw in (b"MZprefix" + normal, normal + b"trailing", normal[:-1]):
            with self.assertRaises(subject.InputError):
                subject.archive_files(raw, subject.sha(raw))
        crc = bytearray(normal)
        local_name_length = struct.unpack_from("<H", crc, 26)[0]
        crc[30 + local_name_length] ^= 0xFF
        with self.assertRaisesRegex(subject.InputError, "ZIP_INVALID"):
            subject.archive_files(bytes(crc), subject.sha(crc))

    def test_json_duplicate_bool_version_and_noninteger_values_fail_closed(self) -> None:
        for raw in (b'{"x":1,"x":2}', b'{"x":NaN}', b'{"x":1.0}', b"[]", b"\xff"):
            with self.subTest(raw=raw), self.assertRaises(subject.InputError):
                subject.json_object(raw)

    def test_api_metadata_positive_and_exact_repo_run_ref_commit_platform_binding(self) -> None:
        artifact, run = self.metadata()
        subject.check_artifact_metadata(self.expected, artifact, run)
        mutations = [lambda a, r: r.__setitem__("id", 999),
                     lambda a, r: r["repository"].__setitem__("full_name", "Other/Repo"),
                     lambda a, r: r["head_repository"].__setitem__("id", 999),
                     lambda a, r: r.__setitem__("head_branch", "main"),
                     lambda a, r: r.__setitem__("head_sha", "b" * 40),
                     lambda a, r: r.__setitem__("status", "in_progress"),
                     lambda a, r: a.__setitem__("id", 999),
                     lambda a, r: a.__setitem__("expired", True),
                     lambda a, r: a.__setitem__("size_in_bytes", subject.MAX_ARCHIVE + 1),
                     lambda a, r: a.__setitem__("digest", "sha256:" + "0" * 64),
                     lambda a, r: a["workflow_run"].__setitem__("id", 999),
                     lambda a, r: a["workflow_run"].__setitem__("head_repository_id", 999),
                     lambda a, r: a["workflow_run"].__setitem__("head_sha", "b" * 40)]
        for mutate in mutations:
            artifact, run = self.metadata()
            mutate(artifact, run)
            with self.assertRaises(subject.InputError):
                subject.check_artifact_metadata(self.expected, artifact, run)

    def test_positive_ids_and_known_platform_ref_and_app_are_required(self) -> None:
        for changes in ({"run_id": 0}, {"artifact_id": -1}, {"run_id": True},
                        {"platform": "QA"}, {"ref": "refs/heads/main"}, {"commit": "bad"},
                        {"application_id": "not an id"}, {"repository": "owner/../other"}):
            with self.subTest(changes=changes), self.assertRaises(subject.InputError):
                replace(self.expected, **changes).validate()

    def test_environment_requires_one_actual_user_or_team_rule_not_name_only(self) -> None:
        for reviewer_type in ("User", "Team"):
            value = {"id": 1, "name": "production", "protection_rules": [
                {"type": "required_reviewers", "prevent_self_review": False,
                 "reviewers": [{"type": reviewer_type, "reviewer": {"id": 9}}]}]}
            subject.check_production_environment(value)
        for rules in ([], [{"type": "wait_timer"}], [{"type": "required_reviewers", "reviewers": []}],
                      [{"type": "required_reviewers", "reviewers": [{"type": "Robot", "reviewer": {"id": 9}}]}],
                      [{"type": "required_reviewers", "reviewers": [{"type": "User", "reviewer": {"id": 0}}]}]):
            with self.subTest(rules=rules), self.assertRaises(subject.InputError):
                subject.check_production_environment({"id": 1, "name": "production", "protection_rules": rules})

    def test_api_302_is_caught_and_public_download_has_no_token_cookie_or_redirect_follow(self) -> None:
        artifact, run = self.metadata()
        location = "https://download.example.test/archive?signature=FAKE_SIGNED"
        calls = []

        def api_open(request, timeout):
            calls.append(("api", request))
            self.assertEqual("Bearer FAKE_TOKEN", request.get_header("Authorization"))
            self.assertEqual("2026-03-10", request.get_header("X-github-api-version"))
            self.assertTrue(request.full_url.startswith("https://api.github.com/repos/" + REPO + "/"))
            if request.full_url.endswith("/zip"):
                raise urllib.error.HTTPError(request.full_url, 302, "Found", {"Location": location}, io.BytesIO())
            if "/actions/runs/" in request.full_url:
                return Response(encoded(run))
            if request.full_url.endswith("/production"):
                return Response(encoded({"id": 1, "name": "production", "protection_rules": [
                    {"type": "required_reviewers", "reviewers": [{"type": "User", "reviewer": {"id": 1}}]}]}))
            return Response(encoded(artifact))

        def public_open(request, timeout):
            calls.append(("public", request))
            self.assertEqual(location, request.full_url)
            self.assertIsNone(request.get_header("Authorization"))
            self.assertIsNone(request.get_header("Cookie"))
            return Response(self.archive)

        api = mock.Mock(open=mock.Mock(side_effect=api_open))
        public = mock.Mock(open=mock.Mock(side_effect=public_open))
        with mock.patch.object(subject.urllib.request, "build_opener", side_effect=[api, public]), \
                mock.patch.object(subject.socket, "getaddrinfo", return_value=[(2, 1, 6, "", ("93.184.216.34", 443))]):
            received = subject.GitHubDownload("FAKE_TOKEN").archive(self.expected, True)
        self.assertEqual(self.archive, received)
        self.assertEqual(["api"] * 4 + ["public"], [kind for kind, _ in calls])
        self.assertIsNone(subject._NoRedirect().redirect_request(None, None, 302, "", {}, location))
        self.assertEqual(b"PREVIOUS=unchanged\n", self.github_env.read_bytes())

    def test_http_api_redirect_error_is_not_leaked_and_no_download_or_env_on_bad_metadata(self) -> None:
        artifact, run = self.metadata()
        run["repository"]["full_name"] = "Other/Repo"
        responses = [Response(encoded(run)), Response(encoded(artifact))]
        api = mock.Mock(open=mock.Mock(side_effect=responses))
        with mock.patch.object(subject.urllib.request, "build_opener", return_value=api):
            with self.assertRaises(subject.InputError):
                subject.GitHubDownload("FAKE_TOKEN").archive(self.expected, False)
        self.assertEqual(2, api.open.call_count)
        self.assertEqual([], list(self.runner.glob("stp-rc-inputs-*")))

    def test_download_private_mixed_or_unsafe_url_is_rejected_before_connection(self) -> None:
        for url in ("http://download.example.test/file", "https://user:pass@download.example.test/file",
                    "https://download.example.test:444/file", "https://download.example.test/file#fragment",
                    "https://download.example.test/file\nAuthorization: FAKE"):
            with self.subTest(url=url), self.assertRaises(subject.InputError):
                subject._public_addresses(url)
        for ips in (("127.0.0.1",), ("10.0.0.1",), ("169.254.169.254",), ("::1",),
                    ("93.184.216.34", "192.168.1.1"), ("224.0.0.1",)):
            records = [(2, 1, 6, "", (ip, 443)) for ip in ips]
            with self.subTest(ips=ips), mock.patch.object(subject.socket, "getaddrinfo", return_value=records):
                with self.assertRaises(subject.InputError):
                    subject._public_addresses("https://download.example.test/file?signature=FAKE")

    def test_download_tls_connection_pins_ip_but_uses_original_sni_and_hostname(self) -> None:
        connection = subject._PinnedHTTPSConnection("download.example.test", checked_host="download.example.test",
                                                    addresses=("93.184.216.34",), timeout=30)
        context = mock.Mock()
        connection._context = context
        raw_socket = mock.Mock()
        with mock.patch.object(subject.socket, "create_connection", return_value=raw_socket) as create:
            connection.connect()
        create.assert_called_once_with(("93.184.216.34", 443), 30)
        context.wrap_socket.assert_called_once_with(raw_socket, server_hostname="download.example.test")

    def test_actual_handler_factory_matches_installed_python_https_api_without_network(self) -> None:
        handler = subject._PinnedHTTPSHandler("download.example.test", ("93.184.216.34",))

        def do_open(factory, request, **kwargs):
            connection = factory("download.example.test", timeout=30, **kwargs)
            self.assertIsInstance(connection, subject._PinnedHTTPSConnection)
            self.assertTrue(connection._context.check_hostname)
            self.assertEqual(subject.ssl.CERT_REQUIRED, connection._context.verify_mode)
            return connection

        with mock.patch.object(handler, "do_open", side_effect=do_open):
            handler.https_open(subject.urllib.request.Request("https://download.example.test/file"))

    def test_http_size_deadline_and_failure_are_finite_and_sanitized(self) -> None:
        with self.assertRaises(subject.InputError):
            subject._bounded_read(Response(b"123", headers={"Content-Length": "100"}), 3)
        with self.assertRaises(subject.InputError):
            subject._bounded_read(Response(b"1234"), 3)
        with mock.patch.object(subject.time, "monotonic", side_effect=[0, 91]):
            with self.assertRaises(subject.InputError):
                subject._bounded_read(Response(b"data"), 100)
        api = mock.Mock(open=mock.Mock(side_effect=urllib.error.URLError("FAKE_SIGNED_URL_AND_TOKEN")))
        with mock.patch.object(subject.urllib.request, "build_opener", return_value=api):
            with self.assertRaisesRegex(subject.InputError, "API_READ_FAILED") as error:
                subject.GitHubDownload("FAKE_TOKEN").archive(self.expected, False)
        self.assertNotIn("FAKE_SIGNED", str(error.exception))
        self.assertNotIn("FAKE_TOKEN", str(error.exception))

    def test_git_token_not_forwarded_to_child_argv_env_and_no_git_mutations(self) -> None:
        with mock.patch.dict(os.environ, {"GITHUB_TOKEN": "FAKE_TOKEN", "GH_TOKEN": "FAKE_OTHER"}), \
                mock.patch.object(subject.subprocess, "run", return_value=
                    subprocess.CompletedProcess([], 0, stdout=b"read-only", stderr=b"")) as run:
            self.assertEqual(b"read-only", subject.GitCheckout(self.checkout_root)._git("rev-parse", "HEAD"))
        command = run.call_args.args[0]
        self.assertNotIn("FAKE_TOKEN", " ".join(command))
        self.assertNotIn("GITHUB_TOKEN", run.call_args.kwargs["env"])
        self.assertNotIn("GH_TOKEN", run.call_args.kwargs["env"])
        self.assertEqual(30, run.call_args.kwargs["timeout"])

    def cli_args(self, zip_path=None):
        values = ["--repository", REPO, "--run-id", "123", "--ref", TAG, "--commit", COMMIT,
                  "--platform", "ANDROID", "--application-id", APP, "--artifact-id", "456",
                  "--zip-sha256", self.expected.zip_sha256, "--checkout", str(self.checkout_root),
                  "--runner-temp", str(self.runner), "--github-env", str(self.github_env)]
        return values + (["--zip", str(zip_path)] if zip_path else [])

    def test_cli_offline_path_is_explicit_and_never_claims_github_or_release_authority(self) -> None:
        archive_path = self.root / "offline.zip"
        archive_path.write_bytes(self.archive)
        output, errors = io.StringIO(), io.StringIO()
        with mock.patch.object(subject, "GitCheckout", return_value=self.checkout), \
                mock.patch.object(subject, "GitHubDownload") as network, redirect_stdout(output), redirect_stderr(errors):
            code = subject.main(self.cli_args(archive_path))
        self.assertEqual(0, code)
        self.assertEqual("", errors.getvalue())
        network.assert_not_called()
        summary = json.loads(output.getvalue())
        self.assertFalse(summary["githubArtifactProvenanceVerified"])
        self.assertFalse(summary["productionOrExternalApproval"])
        self.assertNotIn("https://", output.getvalue())

    def test_cli_bad_inputs_or_exceptions_do_not_log_tokens_signed_urls_or_emit_env(self) -> None:
        for arguments in (["--token", "FAKE_TOKEN"], self.cli_args() + ["--api-base", "https://evil.test"]):
            output, errors = io.StringIO(), io.StringIO()
            with redirect_stdout(output), redirect_stderr(errors):
                self.assertEqual(2, subject.main(arguments))
            self.assertEqual("", output.getvalue())
            self.assertNotIn("FAKE_TOKEN", errors.getvalue())
            self.assertNotIn("https://", errors.getvalue())
        output, errors = io.StringIO(), io.StringIO()
        with mock.patch.object(subject, "GitCheckout", side_effect=RuntimeError("FAKE_SIGNED_AND_TOKEN")), \
                redirect_stdout(output), redirect_stderr(errors):
            self.assertEqual(2, subject.main(self.cli_args()))
        self.assertEqual("", output.getvalue())
        self.assertEqual("RC_INPUTS_READ_FAILED\n", errors.getvalue())
        self.assertEqual(b"PREVIOUS=unchanged\n", self.github_env.read_bytes())

    def test_offline_cannot_claim_production_environment_rule_checked(self) -> None:
        archive_path = self.root / "offline.zip"
        archive_path.write_bytes(self.archive)
        with mock.patch.object(subject, "GitCheckout", return_value=self.checkout), \
                redirect_stderr(io.StringIO()), redirect_stdout(io.StringIO()):
            self.assertEqual(2, subject.main(self.cli_args(archive_path) + ["--require-production-environment"]))
        self.assertEqual(b"PREVIOUS=unchanged\n", self.github_env.read_bytes())


if __name__ == "__main__":
    unittest.main(verbosity=2)
