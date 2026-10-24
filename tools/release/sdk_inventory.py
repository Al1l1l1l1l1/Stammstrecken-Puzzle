#!/usr/bin/env python3
"""Inventory the actual locked/imported mobile SDK sources. This is never runtime/legal evidence."""
from __future__ import annotations

import argparse
import hashlib
import json
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

PINS = {
    "com.google.ads.mobile": "11.5.0",
    "com.google.external-dependency-manager": "1.2.187",
    "com.unity.purchasing": "5.4.3",
    "com.unity.services.core": "1.18.0",
}
EXCLUDED = ("firebase", "crashlytics", "com.unity.services.analytics", "com.unity.services.authentication", "mediation")


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def build_inventory(root: Path) -> dict:
    lock_file = root / "Packages/packages-lock.json"
    manifest_file = root / "Packages/manifest.json"
    manifest = json.loads(manifest_file.read_text(encoding="utf-8-sig"))
    locked = json.loads(lock_file.read_text(encoding="utf-8-sig"))["dependencies"]
    problems = []
    for name in sorted(set(locked) | set(manifest["dependencies"])):
        if any(blocked in name.lower() for blocked in EXCLUDED):
            problems.append(f"SDK_EXCLUDED_PROVIDER:{name}")
    for name, version in PINS.items():
        if locked.get(name, {}).get("version") != version:
            problems.append(f"SDK_LOCK_PIN_MISMATCH:{name}")
        if name != "com.unity.services.core" and manifest["dependencies"].get(name) != version:
            problems.append(f"SDK_MANIFEST_PIN_MISMATCH:{name}")

    packages = []
    cache = root / "Library/PackageCache"
    for name, expected in PINS.items():
        candidates = []
        for directory in sorted(cache.glob(name + "@*")):
            metadata = directory / "package.json"
            if not metadata.is_file():
                continue
            parsed = json.loads(metadata.read_text(encoding="utf-8-sig"))
            if parsed.get("name") == name and parsed.get("version") == expected:
                candidates.append(directory)
        if len(candidates) != 1:
            problems.append(f"SDK_IMPORTED_SOURCE_NOT_UNIQUE:{name}:{len(candidates)}")
            continue
        directory = candidates[0]
        licenses = []
        for file in sorted(directory.iterdir()):
            if file.is_file() and not file.name.endswith(".meta") and (file.name.lower().startswith("license") or file.name == "Third Party Notices.md"):
                licenses.append({"path": file.relative_to(root).as_posix(), "sha256": sha256(file)})
        if not licenses:
            problems.append(f"SDK_LICENSE_SOURCE_MISSING:{name}")
        native = []
        for file in sorted(directory.rglob("*Dependencies.xml")):
            tree = ET.parse(file)
            for item in tree.findall("./androidPackages/androidPackage"):
                native.append({"platform": "android", "specifier": item.attrib.get("spec"), "source": file.relative_to(root).as_posix(), "sourceSha256": sha256(file)})
            for item in tree.findall("./iosPods/iosPod"):
                native.append({"platform": "ios", "name": item.attrib.get("name"), "specifier": item.attrib.get("version"), "source": file.relative_to(root).as_posix(), "sourceSha256": sha256(file)})
        iap_android = directory / "Plugins/UnityPurchasing/Android/IAPResolver/IAPAndroidDependencies.cs"
        if iap_android.is_file():
            native.append({"platform": "android", "source": iap_android.relative_to(root).as_posix(), "sourceSha256": sha256(iap_android), "inspectionRequired": "Resolve exact declared billing dependencies and archive Gradle lock/native artifact hashes."})
        packages.append({"name": name, "version": expected, "packageJsonSha256": sha256(directory / "package.json"), "licenses": licenses, "nativeDeclarations": native})

    source_paths = (
        "Assets/StammstreckenPuzzle/Settings/Resources/BillingMode.json",
        "Assets/StammstreckenPuzzle/Settings/Resources/BillingMode.json.meta",
        "Assets/StammstreckenPuzzle/Scripts/Editor/Build/MobileEditorPrivacyDefaults.cs",
        "Assets/StammstreckenPuzzle/Scripts/Editor/Build/MobilePrivacyBuildGuard.cs",
        "Assets/Plugins/Android/StpPrivacy.androidlib/AndroidManifest.xml",
        "Assets/Plugins/iOS/StpStoreKitVerifier.swift",
        "Assets/Plugins/iOS/StpHaptics.mm",
        "Assets/StammstreckenPuzzle/Scripts/Platform/NativeHapticsAdapter.cs",
        "Content/Assets/build-product-audio.cjs",
        "Assets/StammstreckenPuzzle/Audio/Cues/audio-provenance-v1.json",
        "Assets/StammstreckenPuzzle/Art/Fonts/PROVENANCE.json",
    )
    application_sources = []
    for relative in source_paths:
        file = root / relative
        if not file.is_file():
            problems.append(f"SDK_APPLICATION_SOURCE_MISSING:{relative}")
        else:
            application_sources.append({"path": relative, "sha256": sha256(file)})
    billing_mode = root / source_paths[0]
    if billing_mode.is_file() and json.loads(billing_mode.read_text(encoding="utf-8-sig")) != {"androidStore": "GooglePlay"}:
        problems.append("SDK_CANONICAL_BILLING_MODE_MISMATCH")
    if (root / "Assets/Resources/BillingMode.json").is_file():
        problems.append("SDK_GENERATED_BILLING_MODE_DUPLICATE_PENDING_NORMALIZATION")

    return {
        "inventoryVersion": 1,
        "evidenceKind": "IMPORTED_SDK_SOURCE_INVENTORY",
        "manifestSha256": sha256(manifest_file),
        "packageLockSha256": sha256(lock_file),
        "problems": sorted(problems),
        "packages": packages,
        "applicationSourceInputs": application_sources,
        "editorStartupOrderValidated": False,
        "nativeResolvedLocksValidated": False,
        "runtimeValidated": False,
        "sandboxValidated": False,
        "physicalPrivacyValidated": False,
        "legalApproved": False,
        "promotionAllowed": False,
    }


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[2])
    parser.add_argument("--output", type=Path)
    args = parser.parse_args()
    try:
        inventory = build_inventory(args.root.resolve())
    except (OSError, ValueError, KeyError, ET.ParseError) as error:
        print(f"SDK_SOURCE_INVENTORY_FAILED:{type(error).__name__}", file=sys.stderr)
        return 2
    text = json.dumps(inventory, indent=2, ensure_ascii=False) + "\n"
    if args.output:
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_text(text, encoding="utf-8", newline="\n")
        print(f"SDK_SOURCE_INVENTORY: packages={len(inventory['packages'])}, problems={len(inventory['problems'])}; runtime/legal/promotion not attested")
    else:
        print(text, end="")
    return 1 if inventory["problems"] else 0


if __name__ == "__main__":
    raise SystemExit(main())
