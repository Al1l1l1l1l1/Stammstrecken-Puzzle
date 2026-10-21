#!/usr/bin/env python3
"""Read-only pinned RC-input transport; existing Production preflights still rule.

No approval, source overlay, signing, upload or store action is performed here.
The only downloadable payloads are one closed descriptor and its optional IAP
review original. GITHUB_TOKEN is accepted only from the process environment.
"""
from __future__ import annotations

import argparse
from dataclasses import dataclass
import hashlib
import http.client
import io
import ipaddress
import json
import os
from pathlib import Path
import re
import socket
import ssl
import stat
import struct
import subprocess
import sys
import time
from typing import Mapping
import urllib.error
import urllib.parse
import urllib.request
import uuid
import zipfile

API = "https://api.github.com"
API_VERSION = "2026-03-10"
MIB = 1024 * 1024
MAX_ARCHIVE = 20 * MIB
MAX_FILE = 16 * MIB
MAX_TOTAL = 20 * MIB
DESCRIPTOR = "rc-inputs.json"
IAP_ORIGINAL = "iap-review-original.json"
ENV_NAMES = (
    "STP_PRODUCTION_SOURCE_ROOT", "STP_PRODUCTION_IMPORT_REQUEST",
    "STP_PRODUCTION_APPROVAL_LOCK", "STP_PRODUCTION_APPROVAL_RAW_SHA256",
    "STP_PRODUCTION_SERVICE_PROFILE", "STP_PRODUCTION_SERVICE_PROFILE_RAW_SHA256",
    "STP_PRODUCTION_IAP_REVIEW_ORIGINAL", "STP_PRODUCTION_IAP_REVIEW_RAW_SHA256",
    "STP_PRODUCTION_IAP_POLICY_REVISION", "STP_PRODUCTION_IAP_SDK_REVISION",
    "STP_PUBLISHER_LEGAL_URL", "STP_PUBLISHER_SUPPORT_URL",
)
IAP_NAMES = ENV_NAMES[6:10]
HEX40 = re.compile(r"[0-9a-f]{40}\Z")
HEX64 = re.compile(r"[0-9a-f]{64}\Z")
TAG = re.compile(r"refs/tags/v[0-9]+\.[0-9]+\.[0-9]+-rc\.[1-9][0-9]*\Z")
REPOSITORY = re.compile(r"[A-Za-z0-9][A-Za-z0-9-]*/[A-Za-z0-9][A-Za-z0-9._-]*\Z")
APPLICATION = re.compile(r"[A-Za-z][A-Za-z0-9_]*(?:\.[A-Za-z][A-Za-z0-9_]*)+\Z")


class InputError(ValueError):
    """Stable codes only; never include tokens, signed URLs or server bodies."""


def require(condition: bool, code: str) -> None:
    if not condition:
        raise InputError("RC_INPUTS_" + code)


def sha(raw: bytes) -> str:
    return hashlib.sha256(raw).hexdigest()


def positive(value: object) -> bool:
    return type(value) is int and value > 0


def ascii_value(value: object, limit: int = 4096, empty: bool = False) -> str:
    require(type(value) is str and len(value) <= limit and (empty or bool(value)), "VALUE_INVALID")
    require(all(32 <= ord(char) <= 126 for char in value), "VALUE_CONTROL_OR_NONASCII")
    require(value == value.strip(), "VALUE_WHITESPACE")
    return value


def relative_path(value: object) -> str:
    path = ascii_value(value, 1024)
    require(re.fullmatch(r"[A-Za-z0-9][A-Za-z0-9._-]*(?:/[A-Za-z0-9][A-Za-z0-9._-]*)*", path)
            is not None, "PATH_NOT_CANONICAL")
    reserved = {"CON", "PRN", "AUX", "NUL", *(f"COM{i}" for i in range(1, 10)),
                *(f"LPT{i}" for i in range(1, 10))}
    require(all(not part.endswith(".") and part.split(".")[0].upper() not in reserved
                for part in path.split("/")), "PATH_RESERVED")
    return path


def _pairs(pairs: list[tuple[str, object]]) -> dict[str, object]:
    result: dict[str, object] = {}
    for key, value in pairs:
        require(key not in result, "JSON_DUPLICATE_KEY")
        result[key] = value
    return result


def _bad_number(_: str) -> None:
    raise InputError("RC_INPUTS_JSON_NONINTEGER")


def json_object(raw: bytes) -> dict[str, object]:
    try:
        value = json.loads(raw.decode("utf-8"), object_pairs_hook=_pairs,
                           parse_float=_bad_number, parse_constant=_bad_number)
    except (UnicodeError, json.JSONDecodeError):
        raise InputError("RC_INPUTS_JSON_INVALID") from None
    require(type(value) is dict, "JSON_OBJECT_REQUIRED")
    return value


@dataclass(frozen=True)
class Expected:
    repository: str
    run_id: int
    ref: str
    commit: str
    platform: str
    application_id: str
    artifact_id: int
    zip_sha256: str

    def validate(self) -> None:
        require(REPOSITORY.fullmatch(self.repository) is not None
                and not self.repository.endswith((".", ".git")), "REPOSITORY_INVALID")
        require(positive(self.run_id) and positive(self.artifact_id), "ID_INVALID")
        require(TAG.fullmatch(self.ref) is not None, "REF_INVALID")
        require(HEX40.fullmatch(self.commit) is not None, "COMMIT_INVALID")
        require(self.platform in ("ANDROID", "IOS"), "PLATFORM_INVALID")
        require(APPLICATION.fullmatch(self.application_id) is not None, "APP_ID_INVALID")
        require(HEX64.fullmatch(self.zip_sha256) is not None, "ZIP_HASH_INVALID")


def check_artifact_metadata(expected: Expected, artifact: Mapping[str, object],
                            run: Mapping[str, object]) -> None:
    """Transport provenance only. Successful source CI is not source approval."""
    expected.validate()
    repository = run.get("repository")
    head_repository = run.get("head_repository")
    require(type(repository) is dict and type(head_repository) is dict, "RUN_REPOSITORY_MISSING")
    require(repository.get("full_name") == expected.repository
            and head_repository.get("full_name") == expected.repository
            and positive(repository.get("id")) and positive(head_repository.get("id"))
            and head_repository.get("id") == repository["id"],
            "RUN_REPOSITORY_MISMATCH")
    branch = expected.ref.removeprefix("refs/tags/")
    require(run.get("id") == expected.run_id and type(run.get("id")) is int,
            "RUN_ID_MISMATCH")
    require(run.get("head_sha") == expected.commit and run.get("head_branch") == branch,
            "RUN_REF_COMMIT_MISMATCH")
    require(run.get("status") == "completed" and run.get("conclusion") == "success", "RUN_NOT_SUCCESSFUL")
    require(artifact.get("id") == expected.artifact_id and type(artifact.get("id")) is int
            and artifact.get("expired") is False, "ARTIFACT_ID_OR_EXPIRY")
    require(positive(artifact.get("size_in_bytes")) and artifact["size_in_bytes"] <= MAX_ARCHIVE,
            "ARTIFACT_SIZE")
    origin = artifact.get("workflow_run")
    require(type(origin) is dict and origin.get("id") == expected.run_id
            and type(origin.get("id")) is int and origin.get("head_sha") == expected.commit
            and origin.get("head_branch") == branch
            and positive(origin.get("repository_id")) and positive(origin.get("head_repository_id"))
            and origin.get("repository_id") == repository["id"]
            and origin.get("head_repository_id") == repository["id"], "ARTIFACT_RUN_MISMATCH")
    digest = artifact.get("digest")
    require(digest is None or digest == "sha256:" + expected.zip_sha256, "ARTIFACT_DIGEST_MISMATCH")


def check_production_environment(value: Mapping[str, object]) -> None:
    require(value.get("name") == "production" and positive(value.get("id")), "ENVIRONMENT_IDENTITY")
    rules = value.get("protection_rules")
    require(type(rules) is list, "ENVIRONMENT_RULES_MISSING")
    for rule in rules:
        if type(rule) is not dict or rule.get("type") != "required_reviewers":
            continue
        reviewers = rule.get("reviewers")
        if type(reviewers) is list and any(
                type(item) is dict and item.get("type") in ("User", "Team")
                and type(item.get("reviewer")) is dict and positive(item["reviewer"].get("id"))
                for item in reviewers):
            return
    raise InputError("RC_INPUTS_ENVIRONMENT_REQUIRED_REVIEWER_MISSING")


class _NoRedirect(urllib.request.HTTPRedirectHandler):
    def redirect_request(self, req, fp, code, msg, headers, newurl):
        return None


def _bounded_read(response, maximum: int, seconds: float = 90) -> bytes:
    length = response.headers.get("Content-Length")
    if length is not None:
        require(re.fullmatch(r"[0-9]+", length) is not None and int(length) <= maximum,
                "HTTP_BODY_SIZE")
    deadline = time.monotonic() + seconds
    data = bytearray()
    while True:
        require(time.monotonic() < deadline, "HTTP_DEADLINE")
        chunk = response.read(min(65536, maximum + 1 - len(data)))
        if not chunk:
            break
        data.extend(chunk)
        require(len(data) <= maximum, "HTTP_BODY_SIZE")
    return bytes(data)


def _public_addresses(url: str) -> tuple[str, tuple[str, ...]]:
    ascii_value(url, 8192)
    try:
        parsed = urllib.parse.urlsplit(url)
        require(parsed.scheme == "https" and parsed.hostname is not None and not parsed.username
                and not parsed.password and parsed.port in (None, 443) and not parsed.fragment,
                "DOWNLOAD_HTTPS_REQUIRED")
        host = parsed.hostname
        require(re.fullmatch(r"[A-Za-z0-9](?:[A-Za-z0-9.-]*[A-Za-z0-9])?", host) is not None,
                "DOWNLOAD_HOST_INVALID")
        addresses = tuple(sorted({item[4][0] for item in socket.getaddrinfo(
            host, 443, type=socket.SOCK_STREAM)}))
        require(bool(addresses), "DOWNLOAD_DNS_EMPTY")
        for value in addresses:
            address = ipaddress.ip_address(value)
            require(address.is_global and not address.is_multicast and not address.is_reserved,
                    "DOWNLOAD_NONPUBLIC_ADDRESS")
            if isinstance(address, ipaddress.IPv6Address):
                require(not address.ipv4_mapped and not address.sixtofour and not address.teredo,
                        "DOWNLOAD_TRANSITION_ADDRESS")
        return host, addresses
    except InputError:
        raise
    except (ValueError, OSError):
        raise InputError("RC_INPUTS_DOWNLOAD_ADDRESS_INVALID") from None


class _PinnedHTTPSConnection(http.client.HTTPSConnection):
    """Pin the already checked public IP while preserving Host, TLS SNI and cert checks."""
    def __init__(self, host: str, *, checked_host: str, addresses: tuple[str, ...], **kwargs):
        super().__init__(host, **kwargs)
        require(self.host == checked_host and self.port == 443, "DOWNLOAD_CONNECTION_HOST")
        self.addresses = addresses

    def connect(self) -> None:
        require(self._tunnel_host is None, "DOWNLOAD_PROXY_FORBIDDEN")
        raw = socket.create_connection((self.addresses[0], 443), self.timeout)
        try:
            self.sock = self._context.wrap_socket(raw, server_hostname=self.host)
        except Exception:
            raw.close()
            raise


class _PinnedHTTPSHandler(urllib.request.HTTPSHandler):
    def __init__(self, host: str, addresses: tuple[str, ...]):
        super().__init__(context=ssl.create_default_context())
        self.host, self.addresses = host, addresses

    def https_open(self, request):
        def connection(host, **kwargs):
            return _PinnedHTTPSConnection(host, checked_host=self.host,
                                          addresses=self.addresses, **kwargs)
        return self.do_open(connection, request, context=self._context)


class GitHubDownload:
    """Fixed-origin API auth and a completely separate credential-free download."""
    def __init__(self, token: str):
        ascii_value(token)
        self.token = token
        self.api_opener = urllib.request.build_opener(
            urllib.request.ProxyHandler({}), _NoRedirect(),
            urllib.request.HTTPSHandler(context=ssl.create_default_context()))

    def _api(self, path: str, redirect: bool = False):
        # path is built only from validated repository/positive IDs below.
        require(path.startswith("/repos/") and "?" not in path and "#" not in path, "API_PATH")
        request = urllib.request.Request(API + path, method="GET", headers={
            "Authorization": "Bearer " + self.token, "Accept": "application/vnd.github+json",
            "X-GitHub-Api-Version": API_VERSION, "User-Agent": "STP-RC-Inputs/1"})
        try:
            try:
                response = self.api_opener.open(request, timeout=30)
            except urllib.error.HTTPError as error:
                response = error
            with response:
                if redirect:
                    require(response.code == 302, "DOWNLOAD_REDIRECT_REQUIRED")
                    location = response.headers.get("Location")
                    require(type(location) is str, "DOWNLOAD_LOCATION_MISSING")
                    return location
                require(response.code == 200, "API_RESPONSE_REJECTED")
                return json_object(_bounded_read(response, MIB, 30))
        except InputError:
            raise
        except (OSError, urllib.error.URLError, ValueError):
            raise InputError("RC_INPUTS_API_READ_FAILED") from None

    def archive(self, expected: Expected, require_environment: bool) -> bytes:
        expected.validate()
        base = "/repos/" + expected.repository
        run = self._api(base + "/actions/runs/" + str(expected.run_id))
        artifact = self._api(base + "/actions/artifacts/" + str(expected.artifact_id))
        check_artifact_metadata(expected, artifact, run)
        if require_environment:
            check_production_environment(self._api(base + "/environments/production"))
        location = self._api(base + "/actions/artifacts/" + str(expected.artifact_id) + "/zip", True)
        host, addresses = _public_addresses(location)
        # This opener has no auth/cookie/proxy handler and never follows redirects.
        opener = urllib.request.build_opener(
            urllib.request.ProxyHandler({}), _NoRedirect(), _PinnedHTTPSHandler(host, addresses))
        request = urllib.request.Request(location, method="GET", headers={
            "Accept": "application/zip", "User-Agent": "STP-RC-Inputs/1"})
        try:
            with opener.open(request, timeout=30) as response:
                require(response.code == 200, "DOWNLOAD_RESPONSE_REJECTED")
                return _bounded_read(response, MAX_ARCHIVE)
        except InputError:
            raise
        except (OSError, urllib.error.URLError, ValueError):
            raise InputError("RC_INPUTS_DOWNLOAD_FAILED") from None


def archive_files(raw: bytes, expected_sha256: str) -> dict[str, bytes]:
    require(len(raw) <= MAX_ARCHIVE and HEX64.fullmatch(expected_sha256) is not None
            and sha(raw) == expected_sha256, "ZIP_RAW_HASH_MISMATCH")
    # No self-extracting prefix, appended archive/comment or multidisk/ZIP64 container.
    require(len(raw) >= 22 and raw[-22:-18] == b"PK\x05\x06", "ZIP_END_RECORD")
    _, disk, cd_disk, disk_count, count, cd_size, cd_offset, comment = struct.unpack("<4s4H2IH", raw[-22:])
    require(disk == cd_disk == comment == 0 and disk_count == count and count in (1, 2)
            and cd_offset + cd_size == len(raw) - 22, "ZIP_CONTAINER_INVALID")
    try:
        with zipfile.ZipFile(io.BytesIO(raw)) as archive:
            entries = archive.infolist()
            require(len(entries) == count and not archive.comment
                    and min(entry.header_offset for entry in entries) == 0, "ZIP_ENTRY_COUNT")
            names = [entry.filename for entry in entries]
            require(len(set(name.casefold() for name in names)) == len(names)
                    and set(names) in ({DESCRIPTOR}, {DESCRIPTOR, IAP_ORIGINAL}), "ZIP_NAMES")
            total = 0
            for entry in entries:
                require(entry.orig_filename == entry.filename and entry.filename.isascii()
                        and not entry.is_dir() and not entry.comment, "ZIP_NAME_INVALID")
                mode = (entry.external_attr >> 16) & 0xFFFF
                require(stat.S_IFMT(mode) in (0, stat.S_IFREG)
                        and not (entry.external_attr & 0x410), "ZIP_SPECIAL_FILE")
                require(not entry.flag_bits & 1
                        and entry.compress_type in (zipfile.ZIP_STORED, zipfile.ZIP_DEFLATED), "ZIP_ENCODING")
                require(0 < entry.file_size <= MAX_FILE
                        and entry.file_size <= max(4096, entry.compress_size * 200), "ZIP_FILE_SIZE_OR_RATIO")
                total += entry.file_size
            require(total <= MAX_TOTAL, "ZIP_TOTAL_SIZE")
            files: dict[str, bytes] = {}
            for entry in entries:
                with archive.open(entry, "r") as source:
                    value = source.read(entry.file_size + 1)
                require(len(value) == entry.file_size, "ZIP_SIZE_MISMATCH")
                files[entry.filename] = value
            return files
    except InputError:
        raise
    except (zipfile.BadZipFile, RuntimeError, OSError, ValueError, EOFError):
        raise InputError("RC_INPUTS_ZIP_INVALID") from None


def regular_file(path: Path) -> bytes:
    require(path.is_absolute() and path.resolve(strict=True) == path and path.is_file()
            and not path.is_symlink(), "REGULAR_CANONICAL_FILE_REQUIRED")
    require(0 < path.stat().st_size <= MAX_FILE, "FILE_SIZE")
    return path.read_bytes()


class GitCheckout:
    """Read-only commit/tag/blob checks; no copy, filter, checkout or source mutation."""
    def __init__(self, root: Path):
        self.root = root

    def _git(self, *args: str) -> bytes:
        environment = {key: value for key, value in os.environ.items()
                       if key not in ("GITHUB_TOKEN", "GH_TOKEN", "ACTIONS_RUNTIME_TOKEN")
                       and not key.startswith("GIT_TRACE")}
        environment["GIT_OPTIONAL_LOCKS"] = "0"
        try:
            result = subprocess.run(["git", "--literal-pathspecs", "-c", "core.fsmonitor=false",
                                     "-C", str(self.root), *args], capture_output=True,
                                    check=False, timeout=30, env=environment)
        except (OSError, subprocess.TimeoutExpired):
            raise InputError("RC_INPUTS_GIT_READ_FAILED") from None
        require(result.returncode == 0, "GIT_READ_FAILED")
        return result.stdout

    def verify(self, expected: Expected) -> None:
        expected.validate()
        require(self.root.is_absolute() and self.root.resolve(strict=True) == self.root
                and self.root.is_dir(), "CHECKOUT_CANONICAL_REQUIRED")
        top = Path(self._git("rev-parse", "--show-toplevel").decode("utf-8").strip())
        require(top.resolve(strict=True) == self.root, "CHECKOUT_ROOT_MISMATCH")
        origin = self._git("remote", "get-url", "origin").decode("utf-8").strip()
        expected_origins = {"https://github.com/" + expected.repository,
                            "https://github.com/" + expected.repository + ".git",
                            "git@github.com:" + expected.repository + ".git"}
        require(origin in expected_origins, "CHECKOUT_REPOSITORY_MISMATCH")
        require(self._git("rev-parse", "--verify", "HEAD").strip().decode("ascii") == expected.commit,
                "CHECKOUT_COMMIT_MISMATCH")
        require(self._git("cat-file", "-t", expected.ref).strip() == b"tag", "ANNOTATED_TAG_REQUIRED")
        require(self._git("rev-parse", "--verify", expected.ref + "^{commit}").strip().decode("ascii")
                == expected.commit, "CHECKOUT_TAG_COMMIT_MISMATCH")

    def tracked_bytes(self, commit: str, relative: str) -> bytes:
        relative_path(relative)
        row = self._git("ls-tree", "-z", commit, "--", relative)
        require(row.count(b"\0") == 1 and row.endswith(b"\0") and b"\t" in row,
                "TRACKED_FILE_REQUIRED")
        metadata, actual_path = row[:-1].split(b"\t", 1)
        fields = metadata.decode("ascii").split()
        require(len(fields) == 3 and fields[:2] == ["100644", "blob"]
                and HEX40.fullmatch(fields[2]) is not None and actual_path == relative.encode("ascii"),
                "TRACKED_ORDINARY_FILE_REQUIRED")
        size = self._git("cat-file", "-s", fields[2]).strip()
        require(re.fullmatch(rb"[0-9]+", size) is not None and 0 < int(size) <= MAX_FILE,
                "TRACKED_FILE_SIZE")
        blob = self._git("cat-file", "blob", fields[2])
        require(len(blob) == int(size), "TRACKED_BLOB_SIZE")
        current = regular_file(self.root / relative)
        require(current == blob, "CHECKOUT_FILE_BYTES_MISMATCH")
        return blob


def publisher_url(value: str) -> None:
    ascii_value(value, 2048)
    try:
        uri = urllib.parse.urlsplit(value)
        host = uri.hostname
        require(uri.scheme == "https" and host is not None and "." in host
                and re.fullmatch(r"[A-Za-z0-9](?:[A-Za-z0-9.-]*[A-Za-z0-9])?", host) is not None
                and not uri.username and not uri.password and uri.port in (None, 443)
                and not uri.query and not host.endswith("."), "PUBLISHER_PUBLIC_HTTPS_REQUIRED")
        try:
            ipaddress.ip_address(host)
        except ValueError:
            return
        raise InputError("RC_INPUTS_PUBLISHER_PUBLIC_DNS_REQUIRED")
    except ValueError:
        raise InputError("RC_INPUTS_PUBLISHER_PUBLIC_HTTPS_REQUIRED") from None


def descriptor_environment(files: Mapping[str, bytes], expected: Expected,
                           checkout: GitCheckout) -> tuple[dict[str, str], dict[str, bytes]]:
    value = json_object(files[DESCRIPTOR])
    require(set(value) == {"schemaVersion", "platform", "gitCommit", "applicationIdentifier", "environment"}
            and type(value["schemaVersion"]) is int and value["schemaVersion"] == 1,
            "DESCRIPTOR_FIELDS")
    require(value["platform"] == expected.platform and value["gitCommit"] == expected.commit
            and value["applicationIdentifier"] == expected.application_id, "DESCRIPTOR_IDENTITY")
    env = value["environment"]
    require(type(env) is dict and set(env) == set(ENV_NAMES), "ENV_FIELDS")
    result = {name: ascii_value(env[name], empty=name in IAP_NAMES) for name in ENV_NAMES}
    require(result["STP_PRODUCTION_SOURCE_ROOT"] == ".", "SOURCE_ROOT_LITERAL_REQUIRED")
    snapshots: dict[str, bytes] = {}
    for name in ("STP_PRODUCTION_IMPORT_REQUEST", "STP_PRODUCTION_APPROVAL_LOCK",
                 "STP_PRODUCTION_SERVICE_PROFILE"):
        relative = relative_path(result[name])
        require(relative not in snapshots, "SOURCE_ROLE_ALIAS")
        snapshots[relative] = checkout.tracked_bytes(expected.commit, relative)
    profile = result["STP_PRODUCTION_SERVICE_PROFILE"]
    require(profile.startswith("Assets/StammstreckenPuzzle/Settings/") and profile.endswith(".asset"),
            "SERVICE_PROFILE_PATH")
    for path_name, hash_name in (
            ("STP_PRODUCTION_APPROVAL_LOCK", "STP_PRODUCTION_APPROVAL_RAW_SHA256"),
            ("STP_PRODUCTION_SERVICE_PROFILE", "STP_PRODUCTION_SERVICE_PROFILE_RAW_SHA256")):
        require(HEX64.fullmatch(result[hash_name]) is not None
                and sha(snapshots[result[path_name]]) == result[hash_name], "SOURCE_RAW_HASH_MISMATCH")
    if IAP_ORIGINAL in files:
        json_object(files[IAP_ORIGINAL])  # Syntax only; PASS/roles are never approval.
        require(result[IAP_NAMES[0]] == IAP_ORIGINAL and HEX64.fullmatch(result[IAP_NAMES[1]]) is not None
                and sha(files[IAP_ORIGINAL]) == result[IAP_NAMES[1]]
                and bool(result[IAP_NAMES[2]]) and bool(result[IAP_NAMES[3]]), "IAP_BINDING")
    else:
        require(all(result[name] == "" for name in IAP_NAMES), "IAP_ORIGINAL_REQUIRED")
    for name in ("STP_PUBLISHER_LEGAL_URL", "STP_PUBLISHER_SUPPORT_URL"):
        publisher_url(result[name])
    result["STP_PRODUCTION_SOURCE_ROOT"] = str(checkout.root)
    return result, snapshots


def _outside_checkout(checkout: Path, candidate: Path) -> None:
    require(candidate != checkout and checkout not in candidate.parents, "OUTPUT_INSIDE_CHECKOUT")


def _write_new_readonly(path: Path, raw: bytes) -> None:
    flags = os.O_WRONLY | os.O_CREAT | os.O_EXCL | getattr(os, "O_NOFOLLOW", 0)
    with os.fdopen(os.open(path, flags, 0o600), "wb") as stream:
        stream.write(raw)
        stream.flush()
        os.fsync(stream.fileno())
    path.chmod(0o444)
    require(regular_file(path) == raw, "EXTRACTED_FILE_CHANGED")


def _append_environment(path: Path, runner_temp: Path, values: Mapping[str, str]) -> None:
    require(path.is_absolute() and path.resolve(strict=True) == path and runner_temp in path.parents
            and path.is_file() and not path.is_symlink(), "GITHUB_ENV_PATH")
    require(set(values) == set(ENV_NAMES), "OUTPUT_ENV_FIELDS")
    text = "".join(name + "=" + ascii_value(values[name], empty=name in IAP_NAMES) + "\n"
                   for name in ENV_NAMES)
    flags = os.O_WRONLY | os.O_APPEND | getattr(os, "O_NOFOLLOW", 0)
    with os.fdopen(os.open(path, flags), "ab") as stream:
        require(stat.S_ISREG(os.fstat(stream.fileno()).st_mode), "GITHUB_ENV_NOT_REGULAR")
        stream.write(text.encode("utf-8"))
        stream.flush()
        os.fsync(stream.fileno())


def prepare(raw_zip: bytes, expected: Expected, checkout: GitCheckout, runner_temp: Path,
            github_env: Path) -> dict[str, object]:
    """Validate first; extract only into a fresh private runner directory, then emit 12 values."""
    expected.validate()
    files = archive_files(raw_zip, expected.zip_sha256)  # SHA before parsing/extraction.
    checkout.verify(expected)
    environment, snapshots = descriptor_environment(files, expected, checkout)
    require(runner_temp.is_absolute() and runner_temp.resolve(strict=True) == runner_temp
            and runner_temp.is_dir(), "RUNNER_TEMP_CANONICAL_REQUIRED")
    _outside_checkout(checkout.root, runner_temp)
    require(checkout.root not in github_env.parents, "OUTPUT_INSIDE_CHECKOUT")
    # Check the env destination before claiming/outputting any new directory.
    require(github_env.resolve(strict=True) == github_env and github_env.is_file()
            and runner_temp in github_env.parents, "GITHUB_ENV_PATH")
    private = runner_temp / ("stp-rc-inputs-" + uuid.uuid4().hex)
    try:
        # Windows inherits the already protected RunnerTemp ACL. Python's special
        # 0700 ACL would exclude an active Windows AppContainer from its own files.
        private.mkdir(mode=0o777 if os.name == "nt" else 0o700, exist_ok=False)
    except FileExistsError:
        raise InputError("RC_INPUTS_PRIVATE_DIRECTORY_EXISTS") from None
    for name, data in files.items():
        _write_new_readonly(private / name, data)
    if IAP_ORIGINAL in files:
        environment[IAP_NAMES[0]] = str(private / IAP_ORIGINAL)
    checkout.verify(expected)
    for relative, previous in snapshots.items():
        require(checkout.tracked_bytes(expected.commit, relative) == previous, "SOURCE_CHANGED_DURING_READ")
    for name, previous in files.items():
        require(regular_file(private / name) == previous, "EXTRACTED_FILE_CHANGED")
    _append_environment(github_env, runner_temp, environment)
    return {"status": "ARCHIVE_SOURCE_VALIDATED", "platform": expected.platform,
            "artifactId": expected.artifact_id, "sourceRunId": expected.run_id,
            "zipRawSha256": expected.zip_sha256, "files": len(files),
            "productionOrExternalApproval": False}


class _Parser(argparse.ArgumentParser):
    def error(self, message):
        raise InputError("RC_INPUTS_CLI_ARGUMENTS_INVALID")


def main(argv: list[str] | None = None) -> int:
    try:
        parser = _Parser(description="Pinned read-only RC input bridge; no approval or signing.")
        for name in ("repository", "run-id", "ref", "commit", "platform", "application-id",
                     "artifact-id", "zip-sha256", "checkout", "runner-temp", "github-env"):
            parser.add_argument("--" + name, required=True)
        parser.add_argument("--require-production-environment", action="store_true")
        parser.add_argument("--zip", type=Path, help="Offline archive/source validation only; no API provenance.")
        args = parser.parse_args(argv)
        require(re.fullmatch(r"[1-9][0-9]*", args.run_id) is not None
                and re.fullmatch(r"[1-9][0-9]*", args.artifact_id) is not None, "ID_INVALID")
        expected = Expected(args.repository, int(args.run_id), args.ref, args.commit, args.platform,
                            args.application_id, int(args.artifact_id), args.zip_sha256)
        expected.validate()
        checkout = GitCheckout(Path(args.checkout))
        checkout.verify(expected)  # Fail a foreign checkout before any network request.
        if args.zip:
            require(not args.require_production_environment, "OFFLINE_CANNOT_VERIFY_ENVIRONMENT")
            require(args.zip.is_file() and args.zip.stat().st_size <= MAX_ARCHIVE, "OFFLINE_ZIP_SIZE")
            raw = args.zip.read_bytes()
        else:
            require(os.environ.get("GITHUB_ENV") == args.github_env, "GITHUB_ENV_ACTUAL_PATH_REQUIRED")
            raw = GitHubDownload(os.environ.get("GITHUB_TOKEN", "")).archive(
                expected, args.require_production_environment)
        summary = prepare(raw, expected, checkout, Path(args.runner_temp), Path(args.github_env))
        summary["githubArtifactProvenanceVerified"] = not bool(args.zip)
        summary["productionEnvironmentRuleChecked"] = bool(args.require_production_environment)
        print(json.dumps(summary, sort_keys=True, separators=(",", ":"), allow_nan=False))
        return 0
    except InputError as error:
        print(str(error), file=sys.stderr)
        return 2
    except Exception:
        # No raw exception/traceback: urllib exceptions can contain the signed URL.
        print("RC_INPUTS_READ_FAILED", file=sys.stderr)
        return 2


if __name__ == "__main__":
    raise SystemExit(main())
