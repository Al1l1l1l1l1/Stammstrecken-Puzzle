#!/usr/bin/env python3
"""Collect actual cached POMs and embedded notices from an offline Gradle artifact report; no legal approval."""
from __future__ import annotations
import argparse
import hashlib
import io
import json
from pathlib import Path
import xml.etree.ElementTree as ET
import zipfile


def digest(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--resolved-artifacts", type=Path, required=True)
    parser.add_argument("--apk-audit", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True, help="New source bundle directory; never overwritten")
    args = parser.parse_args()
    raw = args.resolved_artifacts.read_bytes()
    resolved = json.loads(raw)
    apk_audit = json.loads(args.apk_audit.read_bytes())
    out = args.output.resolve()
    out.mkdir(parents=True, exist_ok=False)
    modules, problems, seen = [], [], set()
    for artifact in resolved["artifacts"]:
        key = tuple(artifact[name] for name in ("group", "name", "version", "extension", "sha256"))
        if key in seen:
            continue
        seen.add(key)
        source = Path(artifact["file"])
        if digest(source.read_bytes()) != artifact["sha256"]:
            raise ValueError("RESOLVED_ARTIFACT_CHANGED:" + artifact["name"])
        group, name, version = (artifact[key] for key in ("group", "name", "version"))
        directory = out / (group or "local-package") / name / (version or "unversioned")
        directory.mkdir(parents=True, exist_ok=True)
        module = {key: artifact[key] for key in ("group", "name", "version", "extension", "sha256")}
        module.update({"artifactFilename": source.name, "sizeBytes": source.stat().st_size, "poms": [], "embeddedNotices": []})
        for index, pom in enumerate(sorted(source.parents[1].rglob("*.pom")) if group else []):
            original = pom.read_bytes()
            path = directory / ("upstream-" + str(index) + ".pom")
            path.write_bytes(original)
            doc = ET.fromstring(original)
            licenses = [{"name": entry.findtext("{*}name"), "url": entry.findtext("{*}url"), "distribution": entry.findtext("{*}distribution")} for entry in doc.findall("{*}licenses/{*}license")]
            module["poms"].append({"path": path.relative_to(out).as_posix(), "sha256": digest(original), "declaredLicenses": licenses,
                "upstreamUrl": doc.findtext("{*}url"), "parent": {key: doc.findtext("{*}parent/{*}" + key) for key in ("groupId", "artifactId", "version")}})

        def notices(archive: zipfile.ZipFile, prefix: str) -> None:
            for entry in archive.infolist():
                basename = entry.filename.replace("\\", "/").rsplit("/", 1)[-1].lower()
                if entry.is_dir() or not any(word in basename for word in ("license", "notice", "copying", "copyright")):
                    continue
                if entry.file_size > 16777216:
                    raise ValueError("NOTICE_SIZE_UNEXPECTED")
                original = archive.read(entry)
                path = directory / ("notice-" + str(len(module["embeddedNotices"])) + ".txt")
                path.write_bytes(original)
                module["embeddedNotices"].append({"path": path.relative_to(out).as_posix(), "archiveEntry": prefix + entry.filename, "sha256": digest(original)})

        if zipfile.is_zipfile(source):
            with zipfile.ZipFile(source) as archive:
                notices(archive, "")
                if "classes.jar" in archive.namelist():
                    with zipfile.ZipFile(io.BytesIO(archive.read("classes.jar"))) as classes:
                        notices(classes, "classes.jar!")
        if not module["poms"] and group:
            problems.append("NO_CACHED_POM:" + group + ":" + name + ":" + version)
        module["licenseReviewRequired"] = True
        modules.append(module)
    summary = {"evidenceKind": "POSTBUILD_OFFLINE_NATIVE_ARTIFACT_AND_LICENSE_SOURCE_COLLECTION", "configuration": resolved["configuration"],
        "inspectedApkSha256": apk_audit["apkSha256"], "resolvedArtifactReportSha256": digest(raw),
        "scope": "Original cached POM declarations and embedded notices for resolved external AAR/JAR inputs. Does not attest complete final binary notices, transitive license inheritance, legal terms acceptance or exact Git source.",
        "reportedArtifactReferences": len(resolved["artifacts"]), "artifactCount": len(modules), "cachedPomCount": sum(len(m["poms"]) for m in modules), "embeddedNoticeCount": sum(len(m["embeddedNotices"]) for m in modules),
        "modules": modules, "problems": problems, "legalApproved": False, "termsAccepted": False, "completeBinaryNoticeCoverageApproved": False, "promotionAllowed": False}
    (out / "inventory.json").write_text(json.dumps(summary, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print(json.dumps({key: summary[key] for key in ("artifactCount", "cachedPomCount", "embeddedNoticeCount", "problems")}))
    return 1 if problems else 0


if __name__ == "__main__":
    raise SystemExit(main())
