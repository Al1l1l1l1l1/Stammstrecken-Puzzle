#!/usr/bin/env python3
"""Validate retained build-time receipts and preserve their actual artifacts for the existing license collector."""
from __future__ import annotations
import argparse
import hashlib
import json
from pathlib import Path
import re


def digest(raw: bytes) -> str:
    return hashlib.sha256(raw).hexdigest()


def need(condition: bool, code: str) -> None:
    if not condition:
        raise ValueError("NATIVE_ADAPTER_" + code)


def load(path: Path) -> tuple[dict, bytes]:
    raw = path.read_bytes()
    need(0 < len(raw) <= 67108864, "RECEIPT_SIZE")
    value = json.loads(raw)
    need(isinstance(value, dict), "RECEIPT_OBJECT")
    return value, raw


def file_bytes(row: dict) -> bytes:
    path = Path(row["path"])
    need(path.is_absolute() and path.is_file(), "ACTUAL_FILE_REQUIRED")
    raw = path.read_bytes()
    need(len(raw) == row["sizeBytes"] and digest(raw) == row["sha256"], "ACTUAL_INPUT_CHANGED")
    return raw


def token(value: str) -> str:
    need(isinstance(value, str) and value not in (".", "..") and re.fullmatch(r"[A-Za-z0-9][A-Za-z0-9_.-]*", value), "COORDINATE_TOKEN")
    return value


def classify_module_coordinates(row: dict) -> tuple[str, str, str, str]:
    """Gradle flatDir components can have a module ID with both group/version empty."""
    group, name, version = (row.get(key) for key in ("group", "module", "version"))
    need(all(isinstance(value, str) for value in (group, name, version)), "MODULE_COORDINATE_TYPES")
    name = token(name)
    if group == "" and version == "":
        return "LOCAL_FLATDIR_MODULE", "", name, ""
    need(bool(group) and bool(version), "PARTIAL_MODULE_COORDINATES")
    return "MAVEN_MODULE", token(group), name, token(version)


def adapt(evidence: Path, native_output: Path, package_audit: Path, output: Path) -> dict:
    evidence = evidence.resolve()
    need(evidence.is_dir() and not output.exists(), "FRESH_OUTPUT_AND_EVIDENCE_REQUIRED")
    terminal, terminal_raw = load(evidence / "terminal-unity-build.json")
    after, after_raw = load(evidence / "inputs-after.json")
    need(terminal.get("schemaVersion") == after.get("schemaVersion") == 1, "SCHEMA")
    nonce = terminal.get("nonce", "")
    need(re.fullmatch(r"[a-f0-9]{32}", nonce) and evidence.name == nonce and after.get("nonce") == nonce, "NONCE_MISMATCH")
    need(terminal.get("afterOriginalRawSha256") == digest(after_raw), "AFTER_HASH_MISMATCH")
    need(terminal.get("unityBuildSucceeded") is True and terminal.get("gradlePackagingReportAndActualOutputBound") is True, "TERMINAL_RECEIPT_SUCCESS_REQUIRED")
    guid = terminal.get("unityBuildGuid", "")
    need(isinstance(guid, str) and re.fullmatch(r"[a-fA-F0-9]{32}|[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}", guid) and guid.replace("-", "").strip("0"), "UNITY_BUILD_GUID")
    need(after.get("phase") == "AFTER_NATIVE_PACKAGING" and after.get("status") == "PACKAGING_ATTESTED_TERMINAL_BUILD_SUCCESS_STILL_REQUIRED", "AFTER_PHASE")
    need(after.get("currentPackageExecuted") is True and after.get("runtimeArtifactsUnchanged") is True and after.get("packagingTaskInputsUnchanged") is True and after.get("terminalGradleAndUnitySuccessVerified") is False, "PACKAGING_STATE")
    for key in ("invocationId", "sourceWitness", "variant", "artifactKind"):
        need(terminal.get(key) == after.get(key) and isinstance(after.get(key), str) and after[key], "TERMINAL_IDENTITY_MISMATCH")
    need(after["artifactKind"] in ("apk", "aab") and after.get("configuration") == after["variant"] + "RuntimeClasspath", "VARIANT_OR_KIND")
    originals = [("terminal-unity-build.json", terminal_raw), ("inputs-after.json", after_raw)]
    bound = {}
    for key, filename in (("beforeOriginal", "inputs-before.json"), ("packagingTaskInputsOriginal", "packaging-task-inputs-before.json")):
        row = after[key]
        need(row.get("file") == filename, "ORIGINAL_NAME")
        value, raw = load(evidence / filename)
        need(len(raw) == row.get("sizeBytes") and digest(raw) == row.get("sha256"), "ORIGINAL_HASH")
        for field in ("nonce", "invocationId", "requestRawSha256"):
            need(value.get(field) == after.get(field), "ORIGINAL_IDENTITY")
        originals.append((filename, raw))
        bound[key] = value
    before = bound["beforeOriginal"]
    need(before.get("phase") == "BEFORE_NATIVE_PACKAGING" and before.get("configuration") == after["configuration"] and before.get("variant") == after["variant"] and before.get("artifactKind") == after["artifactKind"], "BEFORE_VARIANT_OR_PHASE")
    need(terminal.get("hookRawSha256") in [row.get("sha256") for row in before.get("generatedBuildInputs", []) if Path(row.get("path", "")).name == "stp-native-build-inputs.gradle"], "HOOK_BINDING")
    package, audit_raw = load(package_audit)
    need(package.get("problems") == [], "PACKAGE_AUDIT_PROBLEMS")
    raw_native = native_output.read_bytes()
    native_hash = digest(raw_native)
    kind = after["artifactKind"]
    need(package.get(kind + "Sha256") == native_hash == terminal.get("actualUnityOutputSha256"), "PACKAGE_AUDIT_HASH_MISMATCH")
    size_key = kind + "SizeBytes"
    need(size_key not in package or package[size_key] == len(raw_native), "PACKAGE_AUDIT_SIZE_MISMATCH")
    matches = [row for row in after.get("nativeOutputs", []) if row.get("sha256") == native_hash and row.get("sizeBytes") == len(raw_native)]
    need(len(matches) == 1 and native_output.suffix.lower() == "." + kind, "ACTUAL_OUTPUT_BINDING")
    plans = []
    compatibility = []
    directories = []
    coordinate_hashes = {}
    for index, row in enumerate(before.get("runtimeArtifacts", [])):
        need(row.get("kind") in ("MAVEN_MODULE", "PROJECT", "LOCAL_OR_OPAQUE"), "RUNTIME_ARTIFACT_KIND")
        if "directoryFiles" in row:
            root = Path(row["path"]).resolve()
            children = []
            for number, child in enumerate(row["directoryFiles"]):
                path = Path(child["path"]).resolve()
                need(path.is_relative_to(root), "DIRECTORY_INPUT_ESCAPE")
                raw = file_bytes(child)
                relative = path.relative_to(root).as_posix()
                destination = Path("retained-artifacts") / "directories" / str(index) / relative
                plans.append((destination, raw))
                children.append({"relativePath": relative, "sha256": digest(raw), "sizeBytes": len(raw), "retainedPath": destination.as_posix()})
            need(bool(children), "EMPTY_DIRECTORY_ARTIFACT")
            directories.append({"kind": row["kind"], "component": row.get("component"), "artifact": row.get("artifact"), "coordinates": {key: row[key] for key in ("group", "module", "version") if key in row}, "sourcePath": str(root), "files": children, "legacyArchiveCollectorApplied": False})
            continue
        raw = file_bytes(row)
        source = Path(row["path"])
        actual_kind = row["kind"]
        reported_coordinates = {key: row[key] for key in ("group", "module", "version") if key in row}
        if actual_kind == "MAVEN_MODULE":
            actual_kind, group, name, version = classify_module_coordinates(row)
        if actual_kind == "MAVEN_MODULE":
            key = (group, name, version, source.suffix.lower())
            need(key not in coordinate_hashes or coordinate_hashes[key] == row["sha256"], "LEGACY_COORDINATE_COLLISION")
            coordinate_hashes[key] = row["sha256"]
            relative = Path("retained-artifacts") / "maven" / group / name / version / row["sha256"] / source.name
            # POMs are retained from the existing coordinate-bound cache, never newly resolved.
            cache = source.parents[1]
            if cache.name == version and cache.parent.name == name and cache.parent.parent.name == group:
                for number, pom in enumerate(sorted(cache.rglob("*.pom"))):
                    plans.append((relative.parent.parent / "poms" / (str(number) + "-" + pom.name), pom.read_bytes()))
        else:
            group, version = "", ""
            identity = digest((str(row.get("component")) + ":" + str(row.get("artifact"))).encode())[:16]
            name = ("flatdir-" + name if actual_kind == "LOCAL_FLATDIR_MODULE" else actual_kind.lower()) + "-" + identity
            relative = Path("retained-artifacts") / actual_kind.lower() / name / row["sha256"] / source.name
        plans.append((relative, raw))
        compatibility.append({"group": group, "name": name, "version": version, "extension": source.suffix.lstrip(".").lower(), "file": str((output / relative).resolve()), "sha256": row["sha256"], "kind": actual_kind, "reportedKind": row["kind"], "reportedCoordinates": reported_coordinates, "component": row.get("component"), "artifact": row.get("artifact"), "originalConsumedPath": str(source), "sizeBytes": len(raw)})
    need(bool(compatibility or directories), "NO_RUNTIME_ARTIFACTS")
    # Every identity and actual input is checked before creating an output bundle.
    output.mkdir(parents=True, exist_ok=False)
    seen = {}
    for relative, raw in plans:
        path = output / relative
        need(path.resolve().is_relative_to(output.resolve()), "OUTPUT_ESCAPE")
        if relative.as_posix() in seen:
            need(seen[relative.as_posix()] == digest(raw), "RETAINED_PATH_COLLISION")
            continue
        seen[relative.as_posix()] = digest(raw)
        path.parent.mkdir(parents=True, exist_ok=True)
        with path.open("xb") as stream:
            stream.write(raw)
    retained_originals = []
    for filename, raw in originals + [("package-audit.json", audit_raw)]:
        path = output / "evidence-originals" / filename
        path.parent.mkdir(parents=True, exist_ok=True)
        with path.open("xb") as stream:
            stream.write(raw)
        retained_originals.append({"path": path.relative_to(output).as_posix(), "sha256": digest(raw), "sizeBytes": len(raw)})
    report = {"evidenceKind": "VALIDATED_RETAINED_NATIVE_BUILD_TIME_RECEIPTS_FOR_LEGACY_LICENSE_COLLECTOR", "configuration": "launcher:" + after["configuration"], "offlineDiagnosticOnly": False, "artifacts": compatibility, "retainedDirectoryInputs": directories, "binding": {"nonce": nonce, "invocationId": after["invocationId"], "variant": after["variant"], "artifactKind": kind, "sourceWitness": after["sourceWitness"], "sourceWitnessIsCallerDeclaration": True, "actualOutputSha256": native_hash, "actualOutputBytes": len(raw_native), "auditedNativeOutputPath": str(native_output.resolve()), "terminalDeclaredUnityOutputPath": terminal.get("actualUnityOutput"), "terminalReceiptSha256": digest(terminal_raw), "afterOriginalSha256": digest(after_raw), "packageAuditSha256": digest(audit_raw), "packageAuditEvidenceKind": package.get("evidenceKind"), "independentTerminalProcessWitnessProvidedByAdapter": False}, "retainedOriginalReceipts": retained_originals, "retainedFileHashes": seen, "limits": ["No Gradle resolution or new build occurred. Terminal receipt structure and original/output hashes are bound; the caller must retain the actual Unity process exit/build witness separately.", "The checkout/source witness remains caller-declared, not a clean-source proof.", "Project/local artifacts and directory inputs retain their actual reported identities. Gradle module IDs with both group/version empty are explicitly LOCAL_FLATDIR_MODULE and retain reportedKind/coordinates; they receive no Maven POM or inherited license. Partial or unsafe module coordinates are rejected.", "POMs are post-build copies from an existing coordinate-bound cache, not proof that Gradle consumed their declarations. Directory artifacts are retained but need separate review; the legacy collector scans archives only.", "No complete binary notice coverage, qualified rights review, signing, device/provider/Human/Owner or Store approval is granted."], "legalApproved": False, "termsAccepted": False, "completeBinaryNoticeCoverageApproved": False, "promotionAllowed": False}
    with (output / "resolved-artifacts.json").open("x", encoding="utf-8") as stream:
        stream.write(json.dumps(report, indent=2, ensure_ascii=False) + "\n")
    return report


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--evidence-dir", type=Path, required=True)
    parser.add_argument("--native-output", type=Path, required=True)
    parser.add_argument("--package-audit", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    report = adapt(args.evidence_dir, args.native_output, args.package_audit, args.output)
    print(json.dumps({"report": str(args.output / "resolved-artifacts.json"), "fileArtifacts": len(report["artifacts"]), "directoryArtifacts": len(report["retainedDirectoryInputs"]), "nativeOutputSha256": report["binding"]["actualOutputSha256"], "newResolutionOrBuild": False}))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
