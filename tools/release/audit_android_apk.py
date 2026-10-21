#!/usr/bin/env python3
"""Archive and inspect one actual APK. No device, store, production-signing or exact-Git-source attestation."""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path
import shutil
import subprocess
import sys
import xml.etree.ElementTree as ET
import zipfile


def sha(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def non_empty(value: str) -> str:
    if not value.strip():
        raise argparse.ArgumentTypeError("A non-empty explicit value is required")
    return value


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--apk", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True, help="New archive directory; never overwritten")
    parser.add_argument("--build-log", type=Path, required=True)
    parser.add_argument("--editor-data", type=Path, required=True)
    parser.add_argument("--build-label", type=non_empty, required=True, help="Explicit identity of this actual build; never inferred from GitHEAD")
    parser.add_argument("--source-binding", type=non_empty, required=True, help="Caller-provided exact build-input description, including commit and preserved deltas when available; not independently attested by this tool")
    args = parser.parse_args()
    root = Path(__file__).resolve().parents[2]
    out = args.output.resolve()
    out.mkdir(parents=True, exist_ok=False)
    apk = out / args.apk.name
    shutil.copy2(args.apk, apk)
    android = args.editor_data / "PlaybackEngines/AndroidPlayer"
    java = android / "OpenJDK/bin/java.exe"
    buildtools = android / "SDK/build-tools/36.0.0"
    artifacts: list[dict] = []
    problems: list[str] = []

    def record(path: Path, kind: str) -> None:
        artifacts.append({"path": path.relative_to(out).as_posix(), "kind": kind, "sizeBytes": path.stat().st_size, "sha256": sha(path)})

    def command(name: str, argv: list[str]) -> str:
        result = subprocess.run(argv, capture_output=True, text=True, encoding="utf-8", errors="replace")
        path = out / name
        path.write_text(result.stdout, encoding="utf-8")
        if result.stderr:
            (out / (name + ".stderr.txt")).write_text(result.stderr, encoding="utf-8")
            record(out / (name + ".stderr.txt"), "CLI_DIAGNOSTIC")
        record(path, "CLI_OUTPUT")
        if result.returncode:
            problems.append("CLI_FAILED:" + name + ":" + str(result.returncode))
        return result.stdout

    record(apk, "ACTUAL_APK")
    copied_log = out / "unity-build.log"
    shutil.copy2(args.build_log, copied_log)
    record(copied_log, "ORIGINAL_BUILD_LOG")
    if (root / "Library/LastBuild.buildreport").is_file():
        dest = out / "LastBuild.buildreport"
        shutil.copy2(root / "Library/LastBuild.buildreport", dest)
        record(dest, "ORIGINAL_UNITY_BUILD_REPORT_BINARY")
    gradle = root / "Library/Bee/Android/Prj/IL2CPP/Gradle"
    patterns = ("**/manifest-merger-*-report.txt", "**/manifest-merger-blame-*-report.txt", "**/stp-privacy/*.txt", "**/AndroidManifest.xml", "**/build.gradle", "stp-privacy.gradle", "gradle.properties")
    copied: set[Path] = set()
    for pattern in patterns:
        for source in gradle.glob(pattern):
            if source.is_file() and source not in copied:
                copied.add(source)
                dest = out / "generated-gradle-inputs" / source.relative_to(gradle)
                dest.parent.mkdir(parents=True, exist_ok=True)
                shutil.copy2(source, dest)
                record(dest, "GENERATED_GRADLE_INPUT_OR_MERGE_OUTPUT")
    for dirname in ("Library/Bee/artifacts/Android/ManagedStripped", "Library/Bee/artifacts/Android/il2cppOutput/data"):
        source_dir = root / dirname
        if source_dir.is_dir():
            for source in source_dir.rglob("*"):
                if source.is_file():
                    dest = out / "bee-inputs" / source.relative_to(root / "Library/Bee")
                    dest.parent.mkdir(parents=True, exist_ok=True)
                    shutil.copy2(source, dest)
                    record(dest, "BEE_MANAGED_OR_IL2CPP_INPUT")
    il2cpp = root / "Library/Bee/artifacts/Android/il2cppOutput"
    if il2cpp.is_dir():
        generated = out / "il2cpp-generated-inputs.zip"
        with zipfile.ZipFile(generated, "x", zipfile.ZIP_DEFLATED, compresslevel=1) as archive:
            for source in sorted(il2cpp.rglob("*")):
                if source.is_file() and source.suffix in (".cpp", ".c", ".h", ".hpp", ".json", ".dat", ".rsp"):
                    archive.write(source, source.relative_to(il2cpp).as_posix())
        record(generated, "BEE_GENERATED_IL2CPP_INPUT_ARCHIVE")
    signature = command("signature-verification.txt", [str(java), "-jar", str(buildtools / "lib/apksigner.jar"), "verify", "--verbose", "--print-certs", str(apk)])
    command("badging.txt", [str(buildtools / "aapt2.exe"), "dump", "badging", str(apk)])
    analyzer = [str(java), "-Dcom.android.sdklib.toolsdir=" + str(android / "SDK/cmdline-tools/16.0"), "-classpath", str(android / "SDK/cmdline-tools/16.0/lib/apkanalyzer-classpath.jar"), "com.android.tools.apk.analyzer.ApkAnalyzerCli"]
    manifest = command("packaged-AndroidManifest.xml", analyzer + ["manifest", "print", str(apk)])
    android_ns = "{http://schemas.android.com/apk/res/android}"
    manifest_summary: dict = {}
    try:
        document = ET.fromstring(manifest)
        application = document.find("application")
        sdk = document.find("uses-sdk")
        permissions = [item.get(android_ns + "name") for item in document.findall("uses-permission")]
        providers = [item.get(android_ns + "name") for item in application.findall("provider")] if application is not None else []
        delay = [item.get(android_ns + "value") for item in application.findall("meta-data") if item.get(android_ns + "name") == "com.google.android.gms.ads.DELAY_APP_MEASUREMENT_INIT"] if application is not None else []
        manifest_summary = {"package": document.get("package"), "minSdk": sdk.get(android_ns + "minSdkVersion") if sdk is not None else None,
            "targetSdk": sdk.get(android_ns + "targetSdkVersion") if sdk is not None else None, "permissions": permissions, "providers": providers,
            "measurementDelayValues": delay, "debuggable": application.get(android_ns + "debuggable") if application is not None else None}
        if "com.google.android.gms.permission.AD_ID" in permissions:
            problems.append("PACKAGED_AD_ID_PERMISSION_PRESENT")
        if delay != ["true"]:
            problems.append("PACKAGED_MEASUREMENT_DELAY_NOT_TRUE")
        if any("firebase" in (provider or "").lower() for provider in providers):
            problems.append("PACKAGED_FIREBASE_PROVIDER_PRESENT")
        if manifest_summary["package"] != "com.STP.StammstreckenPuzzle.qa":
            problems.append("CANDIDATE_PACKAGE_MISMATCH")
    except ET.ParseError:
        problems.append("PACKAGED_MANIFEST_XML_UNREADABLE")
    native_libraries, properties = [], {}
    with zipfile.ZipFile(apk) as archive:
        for name in archive.namelist():
            if name.startswith("lib/") and name.endswith(".so"):
                data = archive.read(name)
                native_libraries.append({"path": name, "sizeBytes": len(data), "sha256": hashlib.sha256(data).hexdigest(), "elfMachine": int.from_bytes(data[18:20], "little") if data.startswith(b"\x7fELF") else None})
            elif name.endswith(".properties") and any(token in name.lower() for token in ("ads", "billing", "messaging", "measurement")):
                properties[name] = archive.read(name).decode("utf-8", errors="replace")
    abis = sorted({item["path"].split("/")[1] for item in native_libraries})
    if abis != ["arm64-v8a"] or any(item["elfMachine"] != 183 for item in native_libraries):
        problems.append("NATIVE_ARM64_ABI_MISMATCH")
    if "Android Debug" not in signature:
        problems.append("DEBUG_SIGNER_CLASSIFICATION_REQUIRES_REVIEW")
    result = {"evidenceKind": "ACTUAL_ANDROID_APK_ARTIFACT_INSPECTION", "buildLabel": args.build_label, "apkSha256": sha(apk), "apkSizeBytes": apk.stat().st_size,
        "sourceBinding": args.source_binding, "sourceBindingIsCallerDeclaration": True,
        "manifest": manifest_summary, "abis": abis, "nativeLibraries": native_libraries, "packagedSdkVersionProperties": properties,
        "artifacts": artifacts, "problems": problems, "productionSigned": False, "physicalDeviceValidated": False,
        "sandboxValidated": False, "physicalPrivacyValidated": False, "legalApproved": False, "promotionAllowed": False}
    (out / "audit.json").write_text(json.dumps(result, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print(json.dumps({"archive": str(out), "buildLabel": args.build_label, "apkSha256": result["apkSha256"], "bytes": result["apkSizeBytes"], "abis": abis, "problems": problems}))
    return 1 if problems else 0


if __name__ == "__main__":
    sys.exit(main())
