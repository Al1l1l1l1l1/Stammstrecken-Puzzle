#!/usr/bin/env python3
"""ADR042 final Human APK manifest gate. Preserves actual bytes; never a physical capture or native SDK absence claim."""
from pathlib import Path
import argparse
import hashlib
import json
import re
import shutil
import subprocess
import sys
import xml.etree.ElementTree as ET

APPLICATION_ID = "com.STP.StammstreckenPuzzle.human"
MAIN_ACTIVITY = "com.unity3d.player.UnityPlayerGameActivity"
ANDROID = "{http://schemas.android.com/apk/res/android}"


def validate_manifest(data: bytes, export_resource_reference: str | None = "@xml/stp_human_export_paths") -> list[str]:
    root = ET.fromstring(data)
    problems = []
    def require(condition, code):
        if not condition:
            problems.append(code)
    require(root.tag == "manifest" and root.get("package") == APPLICATION_ID, "HUMAN_PACKAGE_INVALID")
    apps = root.findall("application")
    if len(apps) != 1:
        return problems + ["HUMAN_APPLICATION_COUNT_INVALID"]
    app = apps[0]
    own = APPLICATION_ID + ".DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION"
    permissions = [node for node in root if node.tag.startswith("uses-permission")]
    require(all(node.tag == "uses-permission" and node.get(ANDROID + "name") == own for node in permissions), "HUMAN_PERMISSION_FORBIDDEN")
    require(not any(root.findall(tag) for tag in ("permission-tree", "permission-group", "instrumentation", "queries")), "HUMAN_EXTERNAL_ENTRY_FORBIDDEN")
    declared = root.findall("permission")
    require(len(declared) <= 1 and all(node.get(ANDROID + "name") == own and node.get(ANDROID + "protectionLevel") in ("signature", "0x2") for node in declared), "HUMAN_CUSTOM_PERMISSION_FORBIDDEN")
    require(len(permissions) == len(declared), "HUMAN_LOCAL_PERMISSION_BINDING_INVALID")
    require(app.get(ANDROID + "allowBackup") == "false" and app.get(ANDROID + "fullBackupContent") == "false", "HUMAN_BACKUP_MUST_BE_OFF")
    require(not any(app.get(ANDROID + name) for name in ("name", "backupAgent", "manageSpaceActivity", "process")), "HUMAN_CUSTOM_APPLICATION_FORBIDDEN")
    require(app.get(ANDROID + "appComponentFactory") in (None, "", "androidx.core.app.CoreComponentFactory"), "HUMAN_COMPONENT_FACTORY_FORBIDDEN")
    require(all(node.tag in ("activity", "meta-data", "provider") for node in app), "HUMAN_COMPONENT_FORBIDDEN")
    providers = app.findall("provider")
    require(len(providers) <= 1, "HUMAN_PROVIDER_COUNT_INVALID")
    for provider in providers:
        require(provider.attrib == {ANDROID + "name": "com.stp.human.StpHumanFileProvider", ANDROID + "authorities": APPLICATION_ID + ".humanexports", ANDROID + "exported": "false", ANDROID + "grantUriPermissions": "true"}, "HUMAN_LOCAL_PROVIDER_INVALID")
        details = list(provider)
        require(len(details) == 1 and details[0].tag == "meta-data" and details[0].attrib == {ANDROID + "name": "android.support.FILE_PROVIDER_PATHS", ANDROID + "resource": export_resource_reference} and not list(details[0]) and export_resource_reference is not None, "HUMAN_LOCAL_PROVIDER_PATH_BINDING_INVALID")
    activities = app.findall("activity")
    require(len(activities) == 1 and activities[0].get(ANDROID + "name") == MAIN_ACTIVITY and activities[0].get(ANDROID + "exported") == "true", "HUMAN_MAIN_ACTIVITY_INVALID")
    if len(activities) == 1:
        intents = activities[0].findall("intent-filter")
        require(len(intents) == 1 and len(intents[0].findall("action")) == 1 and intents[0].find("action").get(ANDROID + "name") == "android.intent.action.MAIN" and len(intents[0].findall("category")) == 1 and intents[0].find("category").get(ANDROID + "name") == "android.intent.category.LAUNCHER" and not intents[0].findall("data"), "HUMAN_LAUNCH_INTENT_INVALID")
    metadata = [node.get(ANDROID + "name") for node in app.findall("meta-data")]
    require(len(set(metadata)) == len(metadata) and all(name and (name.startswith("unity.") or name == "notch.config") for name in metadata), "HUMAN_PROVIDER_METADATA_FORBIDDEN")
    return sorted(set(problems))


def validate_export_paths(data: bytes) -> list[str]:
    root = ET.fromstring(data)
    children = list(root)
    valid = root.tag == "paths" and not root.attrib and len(children) == 1 and children[0].tag == "files-path" and children[0].attrib == {"name": "human_exports", "path": "human-exports/"} and not list(children[0])
    return [] if valid else ["HUMAN_EXPORT_PATHS_INVALID"]


def bind_export_resource(manifest: bytes, resource_table: str) -> str:
    matches = list(re.finditer(r"(?m)^\s*resource (0x[0-9a-fA-F]{8}) xml/stp_human_export_paths\s*$", resource_table))
    if len(matches) != 1:
        raise ValueError("HUMAN_EXPORT_RESOURCE_ID_MISSING_OR_AMBIGUOUS")
    match = matches[0]
    rest = resource_table[match.end():]
    following = re.search(r"(?m)^\s*resource ", rest)
    block = rest[:following.start()] if following else rest
    files = re.findall(r"\(file\)\s+(\S+)\s+type=XML", block)
    if not files or any(path != "res/xml/stp_human_export_paths.xml" for path in files):
        raise ValueError("HUMAN_EXPORT_RESOURCE_FILE_MISMATCH")
    providers = ET.fromstring(manifest).findall("application/provider")
    if len(providers) != 1:
        raise ValueError("HUMAN_EXPORT_PROVIDER_BINDING_INVALID")
    metadata = providers[0].findall("meta-data")
    if len(metadata) != 1:
        raise ValueError("HUMAN_EXPORT_RESOURCE_REFERENCE_INVALID")
    reference = metadata[0].get(ANDROID + "resource")
    resource_id = match.group(1).lower()
    if reference not in ("@xml/stp_human_export_paths", "@" + resource_id, "@ref/" + resource_id):
        raise ValueError("HUMAN_EXPORT_RESOURCE_REFERENCE_INVALID")
    return reference


def non_empty(value):
    if not value.strip():
        raise argparse.ArgumentTypeError("Explicit non-empty metadata is required")
    return value


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--apk", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True, help="New archive, never overwritten")
    parser.add_argument("--editor-data", type=Path, required=True)
    parser.add_argument("--build-label", type=non_empty, required=True)
    parser.add_argument("--source-binding", type=non_empty, required=True)
    parser.add_argument("--build-log", type=Path, required=True)
    args = parser.parse_args()
    out = args.output.resolve()
    out.mkdir(parents=True, exist_ok=False)
    apk = out / args.apk.name
    shutil.copy2(args.apk, apk)
    shutil.copy2(args.build_log, out / "unity-build.log")
    android = args.editor_data / "PlaybackEngines/AndroidPlayer"
    java = android / "OpenJDK/bin/java.exe"
    analyzer = [str(java), "-Dcom.android.sdklib.toolsdir=" + str(android / "SDK/cmdline-tools/16.0"), "-classpath", str(android / "SDK/cmdline-tools/16.0/lib/apkanalyzer-classpath.jar"), "com.android.tools.apk.analyzer.ApkAnalyzerCli", "manifest", "print", str(apk)]
    result = subprocess.run(analyzer, capture_output=True, text=True, encoding="utf-8", errors="replace")
    xml = result.stdout.encode("utf-8")
    (out / "packaged-AndroidManifest.xml").write_bytes(xml)
    (out / "analyzer-stderr.txt").write_text(result.stderr, encoding="utf-8")
    if result.returncode:
        problems = ["HUMAN_PACKAGED_MANIFEST_EXTRACTION_FAILED"]
    else:
        try:
            resource_reference = None
            extra_problems = []
            if ET.fromstring(xml).findall("application/provider"):
                table = subprocess.run([str(android / "SDK/build-tools/36.0.0/aapt2.exe"), "dump", "resources", str(apk)], capture_output=True, text=True, encoding="utf-8", errors="replace")
                (out / "packaged-resources-table.txt").write_text(table.stdout + table.stderr, encoding="utf-8")
                if table.returncode:
                    raise ValueError("HUMAN_EXPORT_RESOURCE_TABLE_FAILED")
                resource_reference = bind_export_resource(xml, table.stdout)
                paths = subprocess.run(analyzer[:-3] + ["resources", "xml", "--file", "/res/xml/stp_human_export_paths.xml", str(apk)], capture_output=True, text=True, encoding="utf-8", errors="replace")
                (out / "packaged-stp-human-export-paths.xml").write_text(paths.stdout, encoding="utf-8")
                (out / "resource-analyzer-stderr.txt").write_text(paths.stderr, encoding="utf-8")
                if paths.returncode:
                    raise ValueError("HUMAN_EXPORT_PATHS_EXTRACTION_FAILED")
                extra_problems = validate_export_paths(paths.stdout.encode("utf-8"))
            problems = validate_manifest(xml, resource_reference) + extra_problems
        except (ET.ParseError, ValueError) as error:
            problems = [str(error) if isinstance(error, ValueError) else "HUMAN_PACKAGED_MANIFEST_UNREADABLE"]
    sha = lambda path: hashlib.sha256(path.read_bytes()).hexdigest()
    audit = {"evidenceKind": "ACTUAL_HUMAN_ANDROID_APK_MANIFEST_GUARD", "buildLabel": args.build_label, "sourceBinding": args.source_binding, "sourceBindingIsCallerDeclaration": True, "apkSha256": sha(apk), "apkSizeBytes": apk.stat().st_size, "packagedManifestSha256": sha(out / "packaged-AndroidManifest.xml"), "buildLogSha256": sha(out / "unity-build.log"), "problems": problems, "physicalNetworkCaptureValidated": False, "sdkBinaryAbsenceAsserted": False, "humanIdentityAttested": False, "humanAcceptance": False, "promotionAllowed": False}
    (out / "audit.json").write_text(json.dumps(audit, indent=2) + "\n", encoding="utf-8")
    print(json.dumps({"archive": str(out), "apkSha256": audit["apkSha256"], "problems": problems}))
    return 1 if problems else 0


if __name__ == "__main__":
    sys.exit(main())
