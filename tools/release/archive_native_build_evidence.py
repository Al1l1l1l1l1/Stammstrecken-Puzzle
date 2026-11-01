#!/usr/bin/env python3
"""Retain the exact current APK's build-time inputs under an already archived audit directory."""
from __future__ import annotations
import hashlib
import importlib.util
import json
from pathlib import Path
import re
import subprocess
import sys
import zipfile

sys.dont_write_bytecode = True


def digest(raw: bytes) -> str:
    return hashlib.sha256(raw).hexdigest()


def need(value: bool, code: str) -> None:
    if not value:
        raise ValueError("NATIVE_RETENTION_" + code)


def original(path: Path) -> tuple[dict, bytes]:
    raw = path.read_bytes()
    need(0 < len(raw) <= 67108864, "RECEIPT_SIZE")
    value = json.loads(raw)
    need(isinstance(value, dict), "RECEIPT_OBJECT")
    return value, raw


def apk_identity(apk: Path) -> tuple[str, int, str]:
    raw = apk.read_bytes()
    with zipfile.ZipFile(apk) as package:
        name = "assets/bin/Data/boot.config"
        need(package.namelist().count(name) == 1, "ONE_BOOT_CONFIG_REQUIRED")
        need(package.getinfo(name).file_size <= 1048576, "BOOT_CONFIG_SIZE")
        boot = package.read(name)
    matches = re.findall(rb"(?m)^build-guid=([0-9A-Fa-f-]+)\r?$", boot)
    need(len(matches) == 1, "ONE_ACTUAL_APK_BUILD_GUID_REQUIRED")
    guid = matches[0].decode("ascii").replace("-", "").lower()
    need(re.fullmatch(r"[0-9a-f]{32}", guid) is not None and bool(guid.strip("0")), "ACTUAL_APK_BUILD_GUID")
    return digest(raw), len(raw), guid


def select_evidence(evidence_root: Path, apk_hash: str, apk_size: int, apk_guid: str) -> Path:
    need(evidence_root.is_dir(), "EVIDENCE_ROOT_REQUIRED")
    candidates = []
    for directory in sorted(evidence_root.iterdir()):
        if not directory.is_dir() or re.fullmatch(r"[0-9a-f]{32}", directory.name) is None:
            continue
        need(not directory.is_symlink() and directory.resolve().parent == evidence_root.resolve(), "NONCE_DIRECTORY_SCOPE")
        terminal_path = directory / "terminal-unity-build.json"
        if not terminal_path.exists():
            # An earlier failed build can retain only BEFORE/AFTER originals; it is never selected.
            continue
        terminal, _ = original(terminal_path)
        if terminal.get("actualUnityOutputSha256") != apk_hash:
            continue
        guid = terminal.get("unityBuildGuid")
        need(isinstance(guid, str) and guid.replace("-", "").lower() == apk_guid, "ACTUAL_APK_GUID_MISMATCH")
        after, _ = original(directory / "inputs-after.json")
        matches = [row for row in after.get("nativeOutputs", []) if row.get("sha256") == apk_hash and row.get("sizeBytes") == apk_size]
        need(len(matches) == 1, "ACTUAL_APK_OUTPUT_RECORD_REQUIRED")
        candidates.append(directory)
    need(len(candidates) == 1, "ONE_MATCHING_CURRENT_NONCE_REQUIRED")
    return candidates[0]


def retain(root: Path, apk: Path, package_audit: Path, output: Path, evidence_root: Path | None = None, expected_checkout: str | None = None) -> dict:
    root = root.resolve()
    apk = apk.resolve()
    package_audit = package_audit.resolve()
    output = output.resolve()
    evidence_root = (evidence_root or root / "Builds/NativeInputEvidence").resolve()
    need(all(path.is_relative_to(root) for path in (apk, package_audit, output, evidence_root)), "WORKSPACE_SCOPE")
    need(not output.exists(), "FRESH_OUTPUT_REQUIRED")
    audit, audit_raw = original(package_audit)
    apk_hash, apk_size, apk_guid = apk_identity(apk)
    need(audit.get("problems") == [] and audit.get("apkSha256") == apk_hash and audit.get("apkSizeBytes") == apk_size, "ACTUAL_SUCCESSFUL_APK_AUDIT_REQUIRED")
    evidence = select_evidence(evidence_root, apk_hash, apk_size, apk_guid)
    receipt_names = ["terminal-unity-build.json", "inputs-before.json", "packaging-task-inputs-before.json", "inputs-after.json"]
    receipts = [(name, original(evidence / name)[1]) for name in receipt_names]
    terminal = json.loads(receipts[0][1])
    need(terminal.get("nonce") == evidence.name, "SELECTED_NONCE_NAME_MISMATCH")
    if expected_checkout is not None:
        need(re.fullmatch(r"[0-9a-f]{40}", expected_checkout) is not None, "CALLER_CHECKOUT_FORMAT")
        need(terminal.get("sourceWitness") == "CALLER_DECLARED_CHECKOUT:" + expected_checkout, "CALLER_CHECKOUT_MISMATCH")
    before = json.loads(receipts[1][1])
    # Preserve generated source bytes as well as hashes. Packaging input byte copies are deliberately not universal:
    # task inputs can include signing keys. The exact packaging-input hash report is retained without copying keys.
    generated = []
    for row in before.get("generatedBuildInputs", []):
        source = Path(row["path"]).resolve()
        need(source.is_relative_to(root), "GENERATED_SOURCE_SCOPE")
        raw = source.read_bytes()
        need(len(raw) == row.get("sizeBytes") and digest(raw) == row.get("sha256"), "GENERATED_SOURCE_CHANGED")
        generated.append((source.relative_to(root), raw))
    need(bool(generated), "GENERATED_SOURCES_REQUIRED")
    output.mkdir(parents=True, exist_ok=False)
    nonce_copy = output / evidence.name
    nonce_copy.mkdir()
    receipt_rows = []
    for name, raw in receipts:
        path = nonce_copy / name
        with path.open("xb") as stream:
            stream.write(raw)
        receipt_rows.append({"path": path.relative_to(output).as_posix(), "sha256": digest(raw), "sizeBytes": len(raw)})
    generated_rows = []
    for relative, raw in generated:
        path = output / "generated-build-inputs" / relative
        need(path.resolve().is_relative_to(output), "GENERATED_OUTPUT_SCOPE")
        path.parent.mkdir(parents=True, exist_ok=True)
        with path.open("xb") as stream:
            stream.write(raw)
        generated_rows.append({"path": path.relative_to(output).as_posix(), "sha256": digest(raw), "sizeBytes": len(raw)})
    adapter_path = Path(__file__).resolve().with_name("adapt_native_build_inputs.py")
    spec = importlib.util.spec_from_file_location("stp_retention_adapter", adapter_path)
    need(spec is not None and spec.loader is not None, "ADAPTER_MODULE_REQUIRED")
    adapter = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(adapter)
    retained = output / "retained-inputs"
    adapted = adapter.adapt(nonce_copy, apk, package_audit, retained)
    collector_path = Path(__file__).resolve().with_name("collect_native_license_sources.py")
    materials = output / "notice-materials"
    process = subprocess.run([sys.executable, str(collector_path), "--resolved-artifacts", str(retained / "resolved-artifacts.json"), "--package-audit", str(package_audit), "--output", str(materials)], capture_output=True, timeout=180)
    with (output / "collector.stdout.txt").open("xb") as stream:
        stream.write(process.stdout)
    with (output / "collector.stderr.txt").open("xb") as stream:
        stream.write(process.stderr)
    need(process.returncode == 0, "COLLECTOR_FAILED")
    inventory, inventory_raw = original(materials / "inventory.json")
    need(inventory.get("problems") == [] and inventory.get("inspectedNativePackageSha256") == apk_hash, "COLLECTOR_OUTPUT_BINDING")
    for name, raw in receipts:
        need((evidence / name).read_bytes() == raw, "ORIGINAL_RECEIPT_CHANGED")
    need(package_audit.read_bytes() == audit_raw and digest(apk.read_bytes()) == apk_hash, "AUDIT_OR_APK_CHANGED")
    need(adapted["binding"]["nonce"] == evidence.name and adapted["binding"]["actualOutputSha256"] == apk_hash, "RETAINED_IDENTITY")
    files = []
    for path in sorted(output.rglob("*")):
        if path.is_file():
            raw = path.read_bytes()
            files.append({"path": path.relative_to(output).as_posix(), "sha256": digest(raw), "sizeBytes": len(raw)})
    record = {"evidenceKind": "ACTUAL_APK_BUILD_TIME_INPUT_RETENTION_INSIDE_ARCHIVED_AUDIT", "status": "PASS_CURRENT_APK_NONCE_ARTIFACTS_AND_MATERIALS_RETAINED", "apkSha256": apk_hash, "apkSizeBytes": apk_size, "actualApkBuildGuid": apk_guid, "selectedNonce": evidence.name, "terminalUnityBuildGuid": terminal["unityBuildGuid"], "actualPackageAuditSha256": digest(audit_raw), "sourceWitness": terminal.get("sourceWitness"), "sourceWitnessIsCallerDeclaration": True, "callerExpectedCheckout": expected_checkout, "callerExpectedCheckoutBound": expected_checkout is not None, "originalReceipts": receipt_rows, "generatedBuildInputFiles": generated_rows, "retainedRuntimeArtifactFiles": len(adapted["artifacts"]), "retainedRuntimeDirectoryArtifacts": len(adapted["retainedDirectoryInputs"]), "materials": {key: inventory[key] for key in ("artifactCount", "cachedPomCount", "embeddedNoticeCount", "problems")}, "materialInventorySha256": digest(inventory_raw), "allRetainedFiles": files, "newResolutionOrBuildPerformed": False, "independentTerminalProcessWitnessProvided": False, "limits": ["The APK's actual build-guid, hash and size select one original nonce; the Unity process exit must be retained independently by the caller/CI. Any expected checkout is matched as an explicit caller label, not an independent clean-source or Git attestation.", "Packaging-task input hashes are retained. Arbitrary task input bytes are not copied because inputs can include signing keys. Generated scripts and actual runtime archives/directories are retained explicitly.", "Cached POM declarations and embedded notices are original materials, not full binary notice coverage or qualified rights approval. No production signing, device/provider/Human/Owner/legal/Main/Store acceptance."], "legalApproved": False, "promotionAllowed": False}
    with (output / "retention.json").open("x", encoding="utf-8", newline="\n") as stream:
        json.dump(record, stream, indent=2, ensure_ascii=False)
        stream.write("\n")
    return record
