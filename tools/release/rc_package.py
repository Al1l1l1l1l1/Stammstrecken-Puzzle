"""Protected-job RC packaging. No upload, tag, provisioning updates, approval creation or credential installation.

All invocations fail closed before signing on invalid/missing bound inputs. This module is inert on import.
"""
from __future__ import annotations

import argparse
import contextlib
import datetime as dt
import hashlib
import json
import os
import plistlib
import re
import shutil
import shlex
import struct
import subprocess
import sys
import signal
import stat
import threading
import xml.etree.ElementTree as ET
import zipfile
from pathlib import Path, PurePosixPath, PureWindowsPath

IDENTITY_FIELDS = frozenset(('releaseVersion', 'candidateTag', 'candidateOrdinal', 'gitCommit', 'workflow', 'platform', 'buildNumber', 'buildProfile', 'applicationIdentifier'))
NATIVE_FIELDS = frozenset(('nativeReceiptVersion', 'stage', 'identity', 'identityRawSha256', 'buildInfoRawSha256', 'nativeInputIndexRawSha256', 'artifact', 'adMobAppId', 'androidRoot', 'scope', 'debugFlags', 'androidMinifyRelease'))
BUILD_INFO_FIELDS = frozenset(('buildInfoVersion', 'releaseVersion', 'buildNumber', 'gitCommit', 'candidateTag', 'builtAtUtc', 'unityVersion', 'packageLockSha256', 'contentLockSha256', 'saveSchemaVersion', 'levelSchemaVersion', 'solverVersion', 'workflow', 'workflowRunId'))
DEBUG_FIELDS = frozenset(('developmentBuild', 'profiler', 'testCrash', 'sandboxProviders'))
ANDROID = '{http://schemas.android.com/apk/res/android}'
SIG = re.compile(r'META-INF/[^/]+\.(?:RSA|DSA|EC|SF)\Z', re.I)
SAFE_ENV = ('PATH', 'HOME', 'USERPROFILE', 'SystemRoot', 'TEMP', 'TMP', 'JAVA_HOME', 'LANG', 'LC_ALL', 'DEVELOPER_DIR', 'SDKROOT')


def subprocess_environment(extra=None):
    return {**{k: os.environ[k] for k in SAFE_ENV if k in os.environ}, **(extra or {})}


class PackagingError(ValueError):
    pass


def need(value, code):
    if not value:
        raise PackagingError(code)


def digest(raw):
    return hashlib.sha256(raw).hexdigest()


def no_links(path):
    path = Path(os.path.abspath(path))
    for part in (path, *path.parents):
        try:
            metadata = os.lstat(part)
        except FileNotFoundError:
            continue
        need(not stat.S_ISLNK(metadata.st_mode) and not (getattr(metadata, 'st_file_attributes', 0) & getattr(stat, 'FILE_ATTRIBUTE_REPARSE_POINT', 0x400)), 'RC_REPARSE_FORBIDDEN')
    return path


def original(path, expected=None, limit=64 * 1024 * 1024):
    path = no_links(path)
    need(path.is_file() and 0 < path.stat().st_size <= limit, 'RC_INPUT_FILE_INVALID')
    raw = path.read_bytes()
    if expected is not None:
        need(isinstance(expected, str) and re.fullmatch(r'[0-9a-f]{64}', expected) and digest(raw) == expected, 'RC_INPUT_RAW_MISMATCH')
    return raw


def pairs(items):
    result = {}
    for key, value in items:
        need(key not in result, 'RC_DUPLICATE_JSON_FIELD')
        result[key] = value
    return result


def read_json(raw):
    try:
        return json.loads(raw.decode('utf-8'), object_pairs_hook=pairs, parse_constant=lambda _: (_ for _ in ()).throw(PackagingError('RC_NONFINITE_JSON')))
    except (UnicodeError, json.JSONDecodeError) as ex:
        raise PackagingError('RC_JSON_INVALID') from ex


def fields(value, expected):
    need(type(value) is dict and set(value) == set(expected), 'RC_FIELDS_INVALID')


def identity_check(value, platform, env=None):
    fields(value, IDENTITY_FIELDS)
    need(all(type(value[k]) is str for k in IDENTITY_FIELDS - {'candidateOrdinal'}), 'RC_IDENTITY_TYPES_INVALID')
    v = value['releaseVersion']; ordinal = value['candidateOrdinal']
    need(re.fullmatch(r'(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)', v), 'RC_VERSION_INVALID')
    need(type(ordinal) is int and 0 < ordinal <= 2147483647 and value['candidateTag'] == f'v{v}-rc.{ordinal}', 'RC_TAG_VERSION_MISMATCH')
    need(re.fullmatch(r'[0-9a-f]{40}', value['gitCommit']) and re.fullmatch(r'[A-Za-z0-9_-]+\.ya?ml@[0-9a-f]{40}', value['workflow']), 'RC_SOURCE_INVALID')
    need(value['buildProfile'] == 'production' and value['platform'] == platform and platform in ('ANDROID', 'IOS'), 'RC_PRODUCTION_PLATFORM_REQUIRED')
    need(re.fullmatch(r'[A-Za-z][A-Za-z0-9]*(?:\.[A-Za-z][A-Za-z0-9]*)+', value['applicationIdentifier']) and not re.search(r'\.(qa|dev|human|test|staging)$', value['applicationIdentifier'], re.I), 'RC_PRODUCTION_APPLICATION_REQUIRED')
    need(re.fullmatch(r'[1-9][0-9]*', value['buildNumber']) and int(value['buildNumber']) <= 2100000000, 'RC_BUILD_NUMBER_INVALID')
    if env is not None:
        need(env.get('GITHUB_ACTIONS') == 'true' and env.get('GITHUB_REF_TYPE') == 'tag', 'RC_PROTECTED_TAG_JOB_REQUIRED')
        need(env.get('GITHUB_REF_NAME') == value['candidateTag'] and env.get('GITHUB_SHA') == value['gitCommit'] and env.get('GITHUB_WORKFLOW_SHA') == value['workflow'].split('@')[1], 'RC_CI_SOURCE_MISMATCH')
        run = env.get('GITHUB_RUN_NUMBER', ''); attempt = env.get('GITHUB_RUN_ATTEMPT', '')
        need(re.fullmatch(r'[1-9][0-9]*', run) and re.fullmatch(r'[1-9][0-9]*', attempt), 'RC_CI_NUMBER_INVALID')
        need(1 <= int(attempt) <= 999 and int(run) * 1000 + int(attempt) <= 2100000000 and str(int(run) * 1000 + int(attempt)) == value['buildNumber'], 'RC_CI_NUMBER_MISMATCH')
    return value


def clean_zip(path):
    archive = zipfile.ZipFile(path)
    names = archive.namelist()
    need(len(names) == len(set(names)), 'RC_DUPLICATE_ZIP_ENTRY')
    need(all(n and not n.startswith('/') and '\\' not in n and ':' not in n and not any(p in ('.', '..') for p in n.rstrip('/').split('/')) for n in names), 'RC_ZIP_PATH_INVALID')
    return archive


def elf_loads(raw):
    need(raw[:7] == b'\x7fELF\x02\x01\x01' and struct.unpack_from('<H', raw, 18)[0] == 183, 'RC_ARM64_ELF_REQUIRED')
    offset = struct.unpack_from('<Q', raw, 32)[0]; width, count = struct.unpack_from('<HH', raw, 54)
    need(width == 56 and 0 < count < 128 and offset + width * count <= len(raw), 'RC_ELF_HEADER_INVALID')
    headers = [struct.unpack_from('<IIQQQQQQ', raw, offset + i * width) for i in range(count)]
    loads = [h for h in headers if h[0] == 1]
    need(loads and all(h[7] >= 16384 and h[7] & (h[7] - 1) == 0 and (h[3] - h[2]) % h[7] == 0 for h in loads), 'RC_ELF_16K_LOAD_REQUIRED')
    for relro in (h for h in headers if h[0] == 0x6474E552):
        end = relro[3] + relro[6]
        if end % 16384:
            own = [h for h in loads if h[3] == relro[3] and h[6] == relro[6]]
            need(len(own) == 1, 'RC_ELF_RELRO_16K_UNSAFE')
            lo = relro[3] // 16384 * 16384; hi = (end + 16383) // 16384 * 16384
            need(not any(h != own[0] and h[1] & 2 and h[3] < hi and h[3] + h[6] > lo for h in loads), 'RC_ELF_RELRO_WRITABLE_OVERLAP')
    return len(loads)


def bundle_payload(path, unsigned):
    with clean_zip(path) as archive:
        names = set(archive.namelist())
        need({'BundleConfig.pb', 'base/manifest/AndroidManifest.xml'} <= names, 'RC_AAB_REQUIRED')
        if unsigned:
            need(not any(SIG.fullmatch(n) for n in names), 'RC_AAB_ALREADY_SIGNED')
        libraries = []
        payload = {}
        for info in archive.infolist():
            raw = archive.read(info)
            if not info.is_dir() and not (SIG.fullmatch(info.filename) or info.filename.upper() == 'META-INF/MANIFEST.MF'):
                payload[info.filename] = digest(raw)
            if raw.startswith(b'\x7fELF') or info.filename.endswith('.so'):
                need(re.fullmatch(r'base/lib/arm64-v8a/[^/]+\.so', info.filename), 'RC_AAB_FOREIGN_NATIVE_ABI')
                libraries.append({'path': info.filename, 'rawSha256': digest(raw), 'loadCount': elf_loads(raw)})
        need(libraries, 'RC_AAB_NATIVE_LIBRARIES_REQUIRED')
        return payload, libraries


def manifest_check(raw, identity, ad_id):
    need(b'<!DOCTYPE' not in raw.upper() and len(raw) <= 4 * 1024 * 1024, 'RC_MANIFEST_INVALID')
    root = ET.fromstring(raw)
    need(root.tag == 'manifest' and root.get('package') == identity['applicationIdentifier'], 'RC_PACKAGED_APPLICATION_MISMATCH')
    need(root.get(ANDROID + 'versionName') == identity['releaseVersion'] and root.get(ANDROID + 'versionCode') == identity['buildNumber'], 'RC_PACKAGED_VERSION_MISMATCH')
    apps = root.findall('application'); need(len(apps) == 1, 'RC_APPLICATION_COUNT_INVALID'); app = apps[0]
    need(app.get(ANDROID + 'debuggable', 'false') == 'false' and app.get(ANDROID + 'testOnly', 'false') == 'false', 'RC_PACKAGED_DEBUG_FORBIDDEN')
    sdks = root.findall('uses-sdk'); need(len(sdks) == 1 and int(sdks[0].get(ANDROID + 'minSdkVersion', '0')) == 26 and int(sdks[0].get(ANDROID + 'targetSdkVersion', '0')) >= 36, 'RC_SDK_BASELINE_REQUIRED')
    def metadata(name):
        return [n.get(ANDROID + 'value') for n in app.findall('meta-data') if n.get(ANDROID + 'name') == name]
    need(metadata('com.google.android.gms.ads.APPLICATION_ID') == [ad_id], 'RC_GMA_APPLICATION_MISMATCH')
    need(metadata('com.google.android.gms.ads.DELAY_APP_MEASUREMENT_INIT') == ['true'], 'RC_PRIVACY_DELAY_REQUIRED')
    need(not any(n.get(ANDROID + 'name') == 'com.google.android.gms.permission.AD_ID' for tag in ('uses-permission', 'uses-permission-sdk-23') for n in root.findall(tag)), 'RC_AD_ID_FORBIDDEN')
    need(not any('firebase' in n.get(ANDROID + 'name', '').lower() for n in app.findall('provider')), 'RC_FIREBASE_PROVIDER_FORBIDDEN')
    return root


class Job:
    def __init__(self, output):
        self.output = no_links(output)
        need(not self.output.exists(), 'RC_FRESH_OUTPUT_REQUIRED')
        self.output.mkdir(parents=True)
        if os.name != 'nt':
            self.output.chmod(0o700)
        self.tools = []; self.redactions = []

    def write(self, name, raw):
        path = self.output / name; no_links(path); path.parent.mkdir(parents=True, exist_ok=True)
        with path.open('xb') as file:
            file.write(raw)
        return {'path': name, 'rawSha256': digest(raw)}

    def json(self, name, value):
        return self.write(name, (json.dumps(value, ensure_ascii=False, indent=2) + '\n').encode('utf-8'))

    def tool(self, path):
        path = no_links(path); raw = original(path, limit=512 * 1024 * 1024)
        self.tools.append({'path': str(path), 'rawSha256': digest(raw)})
        return str(path)

    def run(self, label, argv, cwd=None, env=None, timeout=1800, binary=False):
        # argv is never printed; passwords use env-source switches, never literal argument interpolation.
        flags = {'creationflags': subprocess.CREATE_NO_WINDOW | subprocess.CREATE_NEW_PROCESS_GROUP} if os.name == 'nt' else {'start_new_session': True}
        process = subprocess.Popen(argv, cwd=cwd, env=env if env is not None else subprocess_environment(), stdout=subprocess.PIPE, stderr=subprocess.PIPE, shell=False, **flags)
        buffers = [bytearray(), bytearray()]; overflow = threading.Event()
        def drain(pipe, buffer):
            try:
                while True:
                    chunk = pipe.read(65536)
                    if not chunk:
                        break
                    if len(buffer) + len(chunk) > 32 * 1024 * 1024:
                        overflow.set()
                    else:
                        buffer.extend(chunk)
            finally:
                pipe.close()
        readers = [threading.Thread(target=drain, args=(pipe, buffer), daemon=True) for pipe, buffer in zip((process.stdout, process.stderr), buffers)]
        for reader in readers:
            reader.start()
        timed_out = False
        try:
            process.wait(timeout=timeout)
        except subprocess.TimeoutExpired:
            timed_out = True
            if os.name == 'nt':
                try:
                    subprocess.run([str(Path(os.environ['SystemRoot']) / 'System32/taskkill.exe'), '/PID', str(process.pid), '/T', '/F'], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, timeout=5, shell=False)
                except (OSError, subprocess.SubprocessError):
                    pass
                if process.poll() is None:
                    process.kill()
            else:
                try:
                    os.killpg(process.pid, signal.SIGKILL)
                except ProcessLookupError:
                    pass
            try:
                process.wait(timeout=5)
            except subprocess.TimeoutExpired:
                pass
        for reader in readers:
            reader.join(timeout=3)
        def redact(raw):
            for secret in self.redactions:
                raw = raw.replace(secret.encode('utf-8'), b'[REDACTED]')
            return raw
        self.write(label + '.stderr.txt', redact(bytes(buffers[1])))
        if not binary:
            self.write(label + '.stdout.txt', redact(bytes(buffers[0])))
        self.json(label + '.exit.json', {'exitCode': process.returncode, 'timedOut': timed_out, 'diagnosticOverflow': overflow.is_set(), 'pipeDrainComplete': not any(r.is_alive() for r in readers)})
        need(not timed_out and not overflow.is_set() and not any(r.is_alive() for r in readers) and process.returncode == 0, 'RC_COMMAND_FAILED:' + label)
        return bytes(buffers[0])


def inputs(args, platform, bind_ci=True):
    raw = original(args.identity, args.identity_sha256, 8192)
    identity = identity_check(read_json(raw), platform, os.environ if bind_ci else None)
    receipt_raw = original(args.native_receipt, args.native_receipt_sha256, 65536)
    receipt = read_json(receipt_raw); fields(receipt, NATIVE_FIELDS)
    identity_check(receipt['identity'], platform)
    need(type(receipt['nativeReceiptVersion']) is int and receipt['nativeReceiptVersion'] == 1 and receipt['identity'] == identity and receipt['identityRawSha256'] == digest(raw), 'RC_NATIVE_IDENTITY_MISMATCH')
    need(receipt['stage'] == ('UNSIGNED_AAB' if platform == 'ANDROID' else 'IOS_SOURCE_EXPORT'), 'RC_NATIVE_STAGE_INVALID')
    fields(receipt['debugFlags'], DEBUG_FIELDS); need(all(v is False for v in receipt['debugFlags'].values()), 'RC_NATIVE_DEBUG_FLAGS_FORBIDDEN')
    need(receipt['androidMinifyRelease'] is (False if platform == 'ANDROID' else None), 'RC_R8_REQUIRES_APPROVED_SMOKE_AND_MAPPING')
    need(re.fullmatch(r'[0-9a-f]{32}', receipt['scope']) and re.fullmatch(r'ca-app-pub-[0-9]{16}~[0-9]{10}', receipt['adMobAppId']) and not receipt['adMobAppId'].startswith('ca-app-pub-3940256099942544'), 'RC_NATIVE_SCOPE_INVALID')
    build_info_check(original(Path(args.native_receipt).parent / 'build-info.original.json', receipt['buildInfoRawSha256'], 8192), identity)
    verify_native_inputs(args, receipt, bind_ci or platform == 'ANDROID')
    return identity, receipt, raw, receipt_raw


def recorded_path(value):
    need(type(value) is str and value and not any(ord(c) < 32 for c in value), 'RC_RECORDED_PATH_INVALID')
    result = PureWindowsPath(value) if re.match(r'^[A-Za-z]:[\\/]', value) else PurePosixPath(value)
    need(isinstance(result, PureWindowsPath) or '\\' not in value, 'RC_RECORDED_PATH_INVALID')
    need(result.is_absolute() and all(p not in ('.', '..') for p in result.parts), 'RC_RECORDED_PATH_INVALID')
    return result


def native_base(args, receipt):
    override = getattr(args, 'native_output_root', None)
    if override is not None:
        root = no_links(override)
        need(root.is_dir() and no_links(args.native_receipt).parent == root, 'RC_NATIVE_OUTPUT_OVERRIDE_MISMATCH')
        return root
    return no_links(receipt['artifact']).parent


def native_aab(args, receipt):
    name = recorded_path(receipt['artifact']).name
    need(re.fullmatch(r'[A-Za-z0-9][A-Za-z0-9._-]*\.aab', name), 'RC_RECORDED_AAB_NAME_INVALID')
    return native_base(args, receipt) / name


def verify_native_inputs(args, receipt, verify_files, allowed_changed=()):
    raw = original(Path(args.native_receipt).parent / 'native-inputs.json', receipt['nativeInputIndexRawSha256'], 8 * 1024 * 1024)
    value = read_json(raw); fields(value, ('nativeInputIndexVersion', 'artifactKind', 'files'))
    need(type(value['nativeInputIndexVersion']) is int and value['nativeInputIndexVersion'] == 1 and value['artifactKind'] == ('AAB' if receipt['stage'] == 'UNSIGNED_AAB' else 'IOS_SOURCE_EXPORT'), 'RC_NATIVE_INPUT_INDEX_INVALID')
    need(type(value['files']) is list and 0 < len(value['files']) <= 20000, 'RC_NATIVE_INPUT_COUNT_INVALID')
    seen = set()
    artifact = native_aab(args, receipt) if receipt['stage'] == 'UNSIGNED_AAB' else (no_links(receipt['artifact']) if verify_files else Path(recorded_path(receipt['artifact']).name))
    base = artifact.parent if receipt['stage'] == 'UNSIGNED_AAB' else artifact
    for row in value['files']:
        fields(row, ('path', 'rawSha256')); relative = row['path']
        need(type(relative) is str and relative and not relative.startswith('/') and '\\' not in relative and ':' not in relative and all(ord(c) >= 32 for c in relative) and all(p and p not in ('.', '..') for p in relative.split('/')) and relative not in seen, 'RC_NATIVE_INPUT_PATH_INVALID'); seen.add(relative)
        need(re.fullmatch(r'[0-9a-f]{64}', row['rawSha256']), 'RC_NATIVE_INPUT_HASH_INVALID')
        if verify_files and relative not in allowed_changed:
            original(base / relative, row['rawSha256'], 2 * 1024**3)
    if receipt['stage'] == 'UNSIGNED_AAB':
        need(artifact.name in seen and len(seen) >= 2 and all(p == artifact.name or ('/' not in p and p.endswith('.symbols.zip')) or p.startswith('android-native-originals/') for p in seen), 'RC_NATIVE_INPUT_AAB_INVALID')
    if verify_files and receipt['stage'] == 'IOS_SOURCE_EXPORT' and not allowed_changed:
        actual = set()
        for path in base.rglob('*'):
            no_links(path)
            if path.is_file():
                actual.add(path.relative_to(base).as_posix())
        # The protected, separately pinned Podfile.lock may be supplied after Unity's source export.
        need(actual - {'Podfile.lock'} == seen - {'Podfile.lock'}, 'RC_IOS_NATIVE_INPUT_SET_CHANGED')
    return raw


def elf_build_id(raw):
    need(len(raw) >= 64 and raw[:7] == b'\x7fELF\x02\x01\x01' and struct.unpack_from('<H', raw, 18)[0] == 183, 'RC_SYMBOL_ARM64_ELF_REQUIRED')
    offset = struct.unpack_from('<Q', raw, 32)[0]; width, count = struct.unpack_from('<HH', raw, 54)
    need(width == 56 and count < 128 and offset + width * count <= len(raw), 'RC_SYMBOL_ELF_HEADERS_INVALID')
    values = set()
    for n in range(count):
        ph = struct.unpack_from('<IIQQQQQQ', raw, offset + n * width)
        if ph[0] != 4:
            continue
        pos, end = ph[2], ph[2] + ph[5]
        need(end <= len(raw), 'RC_SYMBOL_ELF_NOTE_INVALID')
        while pos < end:
            need(pos + 12 <= end, 'RC_SYMBOL_ELF_NOTE_INVALID')
            namesz, descsz, kind = struct.unpack_from('<III', raw, pos); pos += 12
            name = raw[pos:pos + namesz]; pos += (namesz + 3) & ~3
            desc = raw[pos:pos + descsz]; pos += (descsz + 3) & ~3
            need(pos <= end and len(desc) == descsz, 'RC_SYMBOL_ELF_NOTE_INVALID')
            if name == b'GNU\0' and kind == 3:
                need(8 <= len(desc) <= 64, 'RC_SYMBOL_BUILD_ID_INVALID'); values.add(desc.hex())
    need(len(values) == 1, 'RC_SYMBOL_SINGLE_BUILD_ID_REQUIRED')
    return next(iter(values))


def android_symbols(job, args, receipt, aab):
    index_raw = verify_native_inputs(args, receipt, True)
    rows = read_json(index_raw)['files']; base = native_base(args, receipt)
    expected = {}
    with clean_zip(aab) as bundle:
        for name in ('libmain.so', 'libunity.so', 'libil2cpp.so'):
            expected[name] = elf_build_id(bundle.read('base/lib/arm64-v8a/' + name))
    matched = {}; refs = []
    for row in rows:
        if not row['path'].endswith('.symbols.zip'):
            continue
        path = base / row['path']; raw = original(path, row['rawSha256'], 2 * 1024**3)
        with clean_zip(path) as symbols:
            for info in symbols.infolist():
                if info.is_dir():
                    continue
                data = symbols.read(info)
                if not data.startswith(b'\x7fELF'):
                    continue
                need('arm64-v8a' in info.filename.split('/'), 'RC_SYMBOL_FOREIGN_ABI')
                name = Path(info.filename).name.replace('.sym.so', '.so').replace('.dbg.so', '.so')
                if name in expected:
                    actual = elf_build_id(data)
                    need(actual == expected[name] and name not in matched, 'RC_SYMBOL_BUILD_ID_MISMATCH')
                    section_offset = struct.unpack_from('<Q', data, 40)[0]; section_width, section_count = struct.unpack_from('<HH', data, 58)
                    need(section_width == 64 and 0 < section_count < 65535 and section_offset + section_count * section_width <= len(data), 'RC_SYMBOL_SECTION_TABLE_REQUIRED')
                    sections = [struct.unpack_from('<IIQQQQIIQQ', data, section_offset + i * section_width) for i in range(section_count)]
                    need(any(s[1] == 2 and s[5] >= 24 and s[9] == 24 and s[4] + s[5] <= len(data) for s in sections), 'RC_SYMBOL_TABLE_REQUIRED')
                    matched[name] = actual
        refs.append(job.write('symbols/' + path.name, raw))
    need(matched == expected, 'RC_ANDROID_CORE_SYMBOLS_REQUIRED')
    evidence = [job.write('native-inputs.json', index_raw), job.json('actual-symbol-binding.json', {'kind': 'ACTUAL_ANDROID_ELF_BUILD_IDS', 'libraries': matched, 'symbols': refs})]
    return refs, evidence


@contextlib.contextmanager
def temporary_project_settings(path, before, configured):
    need(original(path) == before, 'RC_IOS_PROJECT_CHANGED')
    try:
        path.write_bytes(configured)
        yield
    finally:
        no_links(path)
        # Restore the exact generated pre-signing original, including after tool failure.
        path.write_bytes(before)
        need(original(path) == before, 'RC_IOS_PROJECT_RESTORE_FAILED')


def manual_project_settings(model, identity, team, profile_uuid, certificate_sha1, keychain):
    # A profile belongs only to the application target. Passing it as a global xcodebuild override
    # would also apply it to frameworks/Pods and is therefore deliberately forbidden here.
    result = json.loads(json.dumps(model)); objects = result.get('objects', {})
    apps = [v for v in objects.values() if type(v) is dict and v.get('isa') == 'PBXNativeTarget' and v.get('productType') == 'com.apple.product-type.application']
    need(len(apps) == 1 and apps[0].get('name') == 'Unity-iPhone', 'RC_EXACT_IOS_APPLICATION_TARGET_REQUIRED')
    for target in (v for v in objects.values() if type(v) is dict and v.get('isa') == 'PBXNativeTarget' and v.get('name') in ('Unity-iPhone', 'UnityFramework')):
        config_list = objects.get(target.get('buildConfigurationList'), {})
        need(config_list.get('isa') == 'XCConfigurationList' and config_list.get('buildConfigurations'), 'RC_IOS_CONFIGURATION_LIST_INVALID')
        for config_id in config_list['buildConfigurations']:
            config = objects.get(config_id, {}); settings = config.get('buildSettings')
            need(config.get('isa') == 'XCBuildConfiguration' and type(settings) is dict, 'RC_IOS_CONFIGURATION_INVALID')
            if target is apps[0]:
                need(settings.get('PRODUCT_BUNDLE_IDENTIFIER') == identity['applicationIdentifier'], 'RC_IOS_SOURCE_BUNDLE_MISMATCH')
                settings['PROVISIONING_PROFILE_SPECIFIER'] = profile_uuid
            else:
                need(not settings.get('PROVISIONING_PROFILE_SPECIFIER') and not settings.get('PROVISIONING_PROFILE'), 'RC_FRAMEWORK_PROVISIONING_FORBIDDEN')
            settings['DEVELOPMENT_TEAM'] = team; settings['CODE_SIGN_STYLE'] = 'Manual'; settings['CODE_SIGN_IDENTITY'] = certificate_sha1
            settings['OTHER_CODE_SIGN_FLAGS'] = '--keychain ' + shlex.quote(str(keychain))
    return result


def build_info_check(raw, identity):
    value = read_json(raw); fields(value, BUILD_INFO_FIELDS)
    need(type(value['buildInfoVersion']) is int and value['buildInfoVersion'] == 1 and type(value['saveSchemaVersion']) is int and value['saveSchemaVersion'] == 3 and type(value['levelSchemaVersion']) is int and value['levelSchemaVersion'] == 2 and value['solverVersion'] == 'solver-v2', 'RC_BUILD_INFO_CONTRACT_INVALID')
    for key in ('releaseVersion', 'buildNumber', 'gitCommit', 'candidateTag', 'workflow'):
        need(value[key] == identity[key], 'RC_BUILD_INFO_IDENTITY_MISMATCH')
    need(value['unityVersion'] == '6000.3.23f1' and re.fullmatch(r'[1-9][0-9]*', value['workflowRunId']), 'RC_BUILD_INFO_TOOLCHAIN_INVALID')
    for key in ('packageLockSha256', 'contentLockSha256'):
        need(re.fullmatch(r'[0-9a-f]{64}', value[key]), 'RC_BUILD_INFO_HASH_INVALID')
    need(re.fullmatch(r'[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}Z', value['builtAtUtc']), 'RC_BUILD_INFO_UTC_INVALID')
    dt.datetime.strptime(value['builtAtUtc'], '%Y-%m-%dT%H:%M:%SZ')
    return value


def native_reports(job, args, receipt, identity):
    index_raw = verify_native_inputs(args, receipt, True)
    indexed = {row['path']: row['rawSha256'] for row in read_json(index_raw)['files']}
    root = native_base(args, receipt) / 'android-native-originals'; no_links(root)
    report_root = root / 'launcher/build/reports/stp-production-native' / receipt['scope']
    need(report_root.is_dir(), 'RC_BOTH_NATIVE_MERGE_REPORTS_REQUIRED')
    refs = []; kinds = set()
    for number, path in enumerate(sorted(report_root.glob('*.txt'))):
        key = 'android-native-originals/' + path.relative_to(root).as_posix()
        need(key in indexed, 'RC_NATIVE_REPORT_NOT_INDEXED')
        raw = original(path, indexed[key], limit=16384); data = {}
        for line in raw.decode('utf-8').splitlines():
            pair = line.split('=', 1); need(len(pair) == 2 and pair[0] not in data, 'RC_NATIVE_REPORT_INVALID'); data[pair[0]] = pair[1]
        fields(data, ('scope', 'task', 'manifest', 'sha256', 'applicationId', 'adMobAppId'))
        need(data['scope'] == receipt['scope'] and data['applicationId'] == identity['applicationIdentifier'] and data['adMobAppId'] == receipt['adMobAppId'], 'RC_NATIVE_REPORT_SCOPE_MISMATCH')
        task = data['task']; need(task in ('processReleaseMainManifest', 'processReleaseManifestForPackage'), 'RC_RELEASE_MERGE_REQUIRED'); kinds.add(task)
        origin = recorded_path(receipt['androidRoot']); recorded = recorded_path(data['manifest'])
        need(type(origin) is type(recorded) and recorded.is_relative_to(origin), 'RC_NATIVE_MANIFEST_PATH_INVALID')
        relative = recorded.relative_to(origin).as_posix(); key = 'android-native-originals/' + relative
        need(key in indexed and indexed[key] == data['sha256'], 'RC_NATIVE_MANIFEST_NOT_INDEXED')
        manifest_path = no_links(root / relative)
        manifest_raw = original(manifest_path, indexed[key], 4194304); manifest_check(manifest_raw, identity, receipt['adMobAppId'])
        refs.append(job.write(f'native/merge-{number}.xml', manifest_raw)); refs.append(job.write(f'native/report-{number}.txt', raw))
    need(kinds == {'processReleaseMainManifest', 'processReleaseManifestForPackage'}, 'RC_BOTH_NATIVE_MERGE_REPORTS_REQUIRED')
    return refs


def aab_audit(job, args, aab, identity, receipt, unsigned, label):
    payload, libraries = bundle_payload(aab, unsigned)
    build_info = original(Path(args.native_receipt).parent / 'build-info.original.json', receipt['buildInfoRawSha256'], 8192)
    with clean_zip(aab) as actual:
        need(actual.read('base/assets/build-info.json') == build_info, 'RC_AAB_BUILD_INFO_BYTES_MISMATCH')
    java = job.tool(Path(args.jdk) / 'bin' / ('java.exe' if os.name == 'nt' else 'java'))
    original(args.bundletool, args.bundletool_sha256, 256 * 1024 * 1024)
    tool = job.tool(args.bundletool)
    job.run(label + '-bundletool-validate', [java, '-jar', tool, 'validate', '--bundle=' + str(aab)])
    manifest = job.run(label + '-manifest', [java, '-jar', tool, 'dump', 'manifest', '--bundle=' + str(aab), '--module=base'])
    manifest_check(manifest, identity, receipt['adMobAppId'])
    config = job.run(label + '-bundleconfig', [java, '-jar', tool, 'dump', 'config', '--bundle=' + str(aab)])
    refs = [job.write(label + '-actual-manifest.xml', manifest), job.json(label + '-native-elf.json', libraries), job.write(label + '-actual-build-info.json', build_info)]
    # This records actual bundle configuration, never substitutes for delivered split-APK/device validation.
    refs.append(job.json(label + '-bundle-facts.json', {'bundleConfigSha256': digest(config), 'payloadEntries': len(payload), 'nativeLibraryCount': len(libraries), 'limits': 'No Play split generation, store delivery, install or 16KiB runtime test.'}))
    return payload, refs


def signing_verification(job, identity, artifact, certificate_sha256, reports, kind):
    return job.json('signing-verification.json', {
        'verificationVersion': 1, 'kind': kind, 'identity': identity,
        'artifactSha256': digest(artifact.read_bytes()), 'signingFingerprintSha256': certificate_sha256,
        'checks': ['ACTUAL_ARTIFACT_SIGNATURE', 'ACTUAL_PACKAGE_VERSION', 'ACTUAL_NATIVE_PRIVACY', 'IDENTITY_BOUND'],
        'evidence': reports,
    })


def actual_android_signature(job, args, signed, expected_payload):
    java = job.tool(Path(args.jdk) / 'bin' / ('java.exe' if os.name == 'nt' else 'java'))
    verifier = Path(__file__).with_name('VerifyBundleSignatures.java'); verifier_before = original(verifier)
    verification = read_json(job.run('actual-jar-signatures', [java, str(verifier), str(signed), args.certificate_sha256]))
    need(original(verifier) == verifier_before, 'RC_SIGNER_VERIFIER_CHANGED')
    need(verification['signingFingerprintSha256'] == args.certificate_sha256 and verification['verifiedPayloadEntries'] == len(expected_payload), 'RC_PAYLOAD_SIGNATURE_MISMATCH')
    return verifier_before


def android_verify(args):
    identity, receipt, raw, receipt_raw = inputs(args, 'ANDROID', False)
    signed = no_links(args.signed_artifact); original(signed, limit=2 * 1024**3)
    need(re.fullmatch(r'[0-9a-f]{64}', args.certificate_sha256), 'RC_CERTIFICATE_PIN_REQUIRED')
    original_payload, _ = bundle_payload(native_aab(args, receipt), True)
    job = Job(args.output); job.write('identity.json', raw)
    reports = [job.write('native-rc-receipt.json', receipt_raw)] + native_reports(job, args, receipt, identity)
    signed_payload, audit = aab_audit(job, args, signed, identity, receipt, False, 'signed'); reports += audit
    need(signed_payload == original_payload, 'RC_SIGNING_CHANGED_PAYLOAD')
    actual_android_signature(job, args, signed, signed_payload)
    _, symbol_reports = android_symbols(job, args, receipt, signed); reports += symbol_reports
    signing_verification(job, identity, signed, args.certificate_sha256, reports, 'ACTUAL_SIGNED_ANDROID_AAB')


def android_sign(args):
    identity, receipt, raw, receipt_raw = inputs(args, 'ANDROID')
    artifact = native_aab(args, receipt); need(artifact.suffix == '.aab', 'RC_AAB_EXTENSION_REQUIRED'); original(artifact, limit=2 * 1024**3)
    bundle_payload(artifact, True)
    need(re.fullmatch(r'[0-9a-f]{64}', args.certificate_sha256), 'RC_CERTIFICATE_PIN_REQUIRED')
    for key in ('STP_RC_KEYSTORE_PASSWORD', 'STP_RC_KEY_PASSWORD'):
        need(bool(os.environ.get(key)), 'RC_SECRET_REQUIRED:' + key)
    keystore = no_links(args.keystore); need(keystore.is_file(), 'RC_UPLOAD_KEYSTORE_REQUIRED')
    need(re.fullmatch(r'[A-Za-z0-9_.-]{1,128}', args.key_alias), 'RC_KEY_ALIAS_INVALID')
    job = Job(args.output); job.redactions = [os.environ[k] for k in ('STP_RC_KEYSTORE_PASSWORD', 'STP_RC_KEY_PASSWORD')]
    job.write('identity.json', raw)
    reports = [job.write('native-rc-receipt.json', receipt_raw)] + native_reports(job, args, receipt, identity)
    original_payload, audit = aab_audit(job, args, artifact, identity, receipt, True, 'unsigned'); reports += audit
    signed = job.output / 'stp-production-signed.aab'
    jarsigner = job.tool(Path(args.jdk) / 'bin' / ('jarsigner.exe' if os.name == 'nt' else 'jarsigner'))
    job.run('jarsigner-sign', [jarsigner, '-keystore', str(keystore), '-storepass:env', 'STP_RC_KEYSTORE_PASSWORD', '-keypass:env', 'STP_RC_KEY_PASSWORD', '-digestalg', 'SHA-256', '-sigalg', args.signature_algorithm, '-signedjar', str(signed), str(artifact), args.key_alias], env=subprocess_environment({k: os.environ[k] for k in ('STP_RC_KEYSTORE_PASSWORD', 'STP_RC_KEY_PASSWORD')}))
    new_payload, audit = aab_audit(job, args, signed, identity, receipt, False, 'signed'); reports += audit
    need(new_payload == original_payload, 'RC_SIGNING_CHANGED_PAYLOAD')
    verifier_raw = actual_android_signature(job, args, signed, new_payload)
    reports.append(job.json('actual-signed-artifact.json', {'artifactSha256': digest(signed.read_bytes()), 'identity': identity, 'certificateSha256': args.certificate_sha256, 'verifiedPayloadEntries': len(new_payload), 'sourcePayloadPreserved': True}))
    symbols, symbol_reports = android_symbols(job, args, receipt, signed); reports += symbol_reports
    reports.append(signing_verification(job, identity, signed, args.certificate_sha256, reports.copy(), 'ACTUAL_SIGNED_ANDROID_AAB'))
    toolref = job.json('toolchain-report.json', {'tools': job.tools, 'jarVerifierRawSha256': digest(verifier_raw)})
    job.json('packaging-attestation.json', {'packagingVersion': 1, 'stage': 'SIGNED_CANDIDATE', 'identity': identity, 'artifactKind': 'AAB', 'artifactSha256': digest(signed.read_bytes()), 'signingFingerprintSha256': args.certificate_sha256, 'debugFlags': receipt['debugFlags'], 'nativeGateReports': reports, 'symbols': symbols, 'toolchainReports': [toolref]})


def file_inventory(root):
    root = no_links(root); need(root.is_dir(), 'RC_ARCHIVE_REQUIRED')
    result = {}
    for path in sorted(root.rglob('*'), key=lambda p: p.relative_to(root).as_posix()):
        no_links(path)
        if path.is_file():
            need(len(result) < 20000, 'RC_ARCHIVE_INPUT_COUNT_INVALID')
            result[path.relative_to(root).as_posix()] = digest(original(path, limit=2 * 1024**3))
    need(result, 'RC_ARCHIVE_EMPTY')
    return result


def archive_index(identity, receipt_raw, archive):
    return {'archiveInputVersion': 1, 'identity': identity, 'nativeReceiptRawSha256': digest(receipt_raw),
            'files': [{'path': name, 'rawSha256': sha} for name, sha in file_inventory(archive).items()]}


def verify_archive_index(raw, identity, receipt_raw, archive):
    value = read_json(raw); fields(value, ('archiveInputVersion', 'identity', 'nativeReceiptRawSha256', 'files'))
    identity_check(value['identity'], identity['platform'])
    need(type(value['archiveInputVersion']) is int and value['archiveInputVersion'] == 1 and value['identity'] == identity and value['nativeReceiptRawSha256'] == digest(receipt_raw), 'RC_ARCHIVE_SOURCE_BINDING_INVALID')
    need(type(value['files']) is list and 0 < len(value['files']) <= 20000, 'RC_ARCHIVE_INPUT_COUNT_INVALID')
    names = []
    for row in value['files']:
        fields(row, ('path', 'rawSha256')); names.append(row['path'])
    need(all(type(n) is str for n in names) and len(names) == len(set(names)) and names == sorted(names), 'RC_ARCHIVE_INPUT_ORDER_INVALID')
    need(value == archive_index(identity, receipt_raw, archive), 'RC_ARCHIVE_BYTES_CHANGED')


def macho_code(raw):
    # Re-signing replaces LC_CODE_SIGNATURE data and the resulting LINKEDIT size.
    # Every other Mach-O byte and the actual build UUID must remain identical.
    need(len(raw) >= 32 and raw[:4] == b'\xcf\xfa\xed\xfe', 'RC_THIN_ARM64_MACHO_REQUIRED')
    magic, cpu, subtype, kind, count, commands_size, flags, reserved = struct.unpack_from('<IIIIIIII', raw)
    need(cpu == 0x100000c and 0 < count < 2048 and 32 + commands_size <= len(raw), 'RC_MACHO_HEADER_INVALID')
    normalized = bytearray(raw); pos = 32; uuids = []; signatures = []; linkedit = []
    for _ in range(count):
        need(pos + 8 <= 32 + commands_size, 'RC_MACHO_COMMAND_INVALID')
        command, size = struct.unpack_from('<II', raw, pos)
        need(size >= 8 and pos + size <= 32 + commands_size, 'RC_MACHO_COMMAND_INVALID')
        if command == 0x1b:
            need(size == 24, 'RC_MACHO_UUID_INVALID'); uuids.append(raw[pos + 8:pos + 24].hex())
        elif command == 0x1d:
            need(size == 16, 'RC_MACHO_SIGNATURE_INVALID'); signatures.append(struct.unpack_from('<II', raw, pos + 8))
            normalized[pos + 8:pos + 16] = b'\0' * 8
        elif command == 0x19:
            need(size >= 72, 'RC_MACHO_SEGMENT_INVALID')
            if raw[pos + 8:pos + 24].rstrip(b'\0') == b'__LINKEDIT':
                linkedit.append(struct.unpack_from('<QQQQ', raw, pos + 24))
                normalized[pos + 32:pos + 40] = b'\0' * 8
                normalized[pos + 48:pos + 56] = b'\0' * 8
        elif command == 0x2c:
            need(size >= 20 and struct.unpack_from('<I', raw, pos + 16)[0] == 0, 'RC_ENCRYPTED_ARCHIVE_UNSUPPORTED')
        pos += size
    need(pos == 32 + commands_size and len(uuids) == 1 and len(signatures) == 1, 'RC_MACHO_UUID_SIGNATURE_REQUIRED')
    start, length = signatures[0]
    need(length > 0 and start >= pos and start + length == len(raw), 'RC_MACHO_SIGNATURE_RANGE_INVALID')
    need(len(linkedit) == 1, 'RC_MACHO_SINGLE_LINKEDIT_REQUIRED')
    address, memory, offset, size = linkedit[0]
    need(offset >= pos and offset <= start and size > 0 and offset + size == len(raw) and address % 4096 == 0 and memory in {((size + page - 1) // page) * page for page in (4096, 16384)} and address + memory <= 0xffffffffffffffff, 'RC_MACHO_LINKEDIT_EXTENT_INVALID')
    return {'uuid': uuids[0], 'codeSha256': digest(bytes(normalized[:start]))}


def compare_archive_app(archived_app, actual_app):
    def payload(root):
        result = {}
        for path in sorted(root.rglob('*')):
            no_links(path)
            if not path.is_file():
                continue
            relative = path.relative_to(root).as_posix()
            if '_CodeSignature' in relative.split('/') or relative == 'embedded.mobileprovision':
                continue
            raw = original(path, limit=2 * 1024**3)
            result[relative] = macho_code(raw) if raw[:4] == b'\xcf\xfa\xed\xfe' else {'rawSha256': digest(raw)}
        return result
    archived = payload(archived_app); actual = payload(actual_app)
    need(actual == archived, 'RC_IPA_ARCHIVE_PAYLOAD_MISMATCH')
    images = {name: fact for name, fact in actual.items() if 'uuid' in fact}
    need(images, 'RC_ARCHIVE_MACHO_REQUIRED')
    return images


def ipa_symbols(job, archive, actual_app, identity, receipt_raw, index_raw):
    verify_archive_index(index_raw, identity, receipt_raw, archive)
    apps = list((archive / 'Products/Applications').glob('*.app')); need(len(apps) == 1, 'RC_EXACTLY_ONE_ARCHIVED_APP_REQUIRED')
    images = compare_archive_app(apps[0], actual_app)
    main_info = plistlib.loads(original(actual_app / 'Info.plist')); main_name = main_info.get('CFBundleExecutable')
    need(main_name in images, 'RC_MAIN_MACHO_REQUIRED')
    required = {images[main_name]['uuid']}
    for name, value in images.items():
        if name.endswith('/UnityFramework.framework/UnityFramework'):
            required.add(value['uuid'])
    symbols = []; symbol_uuids = set()
    dsym_root = no_links(archive / 'dSYMs'); need(dsym_root.is_dir(), 'RC_DSYM_REQUIRED')
    for n, path in enumerate(sorted(dsym_root.rglob('*'))):
        no_links(path)
        if not path.is_file():
            continue
        symbols.append(job.write('symbols/' + path.relative_to(dsym_root).as_posix(), original(path, limit=2 * 1024**3)))
        if path.parent.name == 'DWARF':
            output = job.run('dsym-uuid-' + str(n), ['/usr/bin/dwarfdump', '--uuid', str(path)])
            symbol_uuids.update(u.decode().replace('-', '').lower() for u in re.findall(rb'UUID: ([A-Fa-f0-9-]+) \(arm64\)', output))
    need(symbols and required <= symbol_uuids, 'RC_DSYM_FINAL_IPA_UUID_MISMATCH')
    verify_archive_index(index_raw, identity, receipt_raw, archive)
    refs = [job.write('native/archive-inputs.json', index_raw), job.json('actual-ipa-archive-binding.json', {'kind': 'ACTUAL_FINAL_IPA_ARCHIVE_CODE_AND_RESOURCES', 'images': images, 'requiredDsymUuids': sorted(required), 'symbols': symbols})]
    return symbols, refs


def verify_actual_ipa(job, ipa, identity, receipt, receipt_raw, certificate_sha256, team, profile_uuid, expected_build_info, archive, index_raw):
    # Validate the final IPA after export/re-signing, not merely its earlier xcarchive.
    unpacked = job.output / 'actual-ipa'; unpacked.mkdir()
    with clean_zip(ipa) as zip_ipa:
        for entry in zip_ipa.infolist():
            need((entry.external_attr >> 16) & 0o170000 != 0o120000, 'RC_IPA_SYMLINK_FORBIDDEN')
        zip_ipa.extractall(unpacked)
    app_paths = list((unpacked / 'Payload').glob('*.app')); need(len(app_paths) == 1, 'RC_EXACTLY_ONE_IPA_APP_REQUIRED')
    app = app_paths[0]
    need(original(app / 'build-info.json', receipt['buildInfoRawSha256'], 8192) == expected_build_info, 'RC_IPA_BUILD_INFO_BYTES_MISMATCH')
    info = plistlib.loads(original(app / 'Info.plist'))
    need(info.get('CFBundleIdentifier') == identity['applicationIdentifier'] and info.get('CFBundleVersion') == identity['buildNumber'] and info.get('CFBundleShortVersionString') == identity['releaseVersion'], 'RC_IPA_IDENTITY_MISMATCH')
    need(info.get('GADApplicationIdentifier') == receipt['adMobAppId'] and info.get('GADDelayAppMeasurementInit') is True and 'NSUserTrackingUsageDescription' not in info, 'RC_IPA_NATIVE_PRIVACY_MISMATCH')
    need(int(info.get('MinimumOSVersion', '0').split('.')[0]) >= 15, 'RC_IOS15_MINIMUM_REQUIRED')
    executable = info.get('CFBundleExecutable')
    need(type(executable) is str and re.fullmatch(r'[A-Za-z0-9_.-]+', executable), 'RC_IOS_EXECUTABLE_INVALID')
    arch = job.run('actual-ipa-arm64', ['/usr/bin/lipo', '-archs', str(app / executable)]).decode('utf-8').strip()
    need(arch == 'arm64', 'RC_IOS_ARM64_REQUIRED')
    job.run('actual-ipa-codesign-verify', ['/usr/bin/codesign', '--verify', '--deep', '--strict', str(app)])
    prefix = str(job.output / 'certificate-')
    job.run('actual-ipa-certificate', ['/usr/bin/codesign', '-d', '--extract-certificates', prefix, str(app)])
    cert = original(prefix + '0', limit=65536)
    need(digest(cert) == certificate_sha256, 'RC_ACTUAL_IOS_CERTIFICATE_MISMATCH')
    entraw = job.run('actual-ipa-entitlements', ['/usr/bin/codesign', '-d', '--entitlements', ':-', str(app)])
    ent = plistlib.loads(entraw)
    need(set(ent) <= {'application-identifier', 'com.apple.developer.team-identifier', 'get-task-allow', 'beta-reports-active', 'keychain-access-groups'} and ent.get('get-task-allow', False) is False and ent.get('application-identifier') == team + '.' + identity['applicationIdentifier'] and ent.get('com.apple.developer.team-identifier') == team, 'RC_ENTITLEMENTS_NOT_ALLOWED')
    need(ent.get('keychain-access-groups', [team + '.' + identity['applicationIdentifier']]) == [team + '.' + identity['applicationIdentifier']], 'RC_KEYCHAIN_ENTITLEMENT_NOT_ALLOWED')
    # Provisioning is inspected in memory; complete profiles/devices are never copied into diagnostics.
    profile_raw = job.run('actual-ipa-profile', ['/usr/bin/security', 'cms', '-D', '-i', str(app / 'embedded.mobileprovision')], binary=True)
    profile = plistlib.loads(profile_raw)
    need(profile.get('UUID') == profile_uuid and profile.get('TeamIdentifier') == [team] and 'ProvisionedDevices' not in profile and not profile.get('ProvisionsAllDevices', False), 'RC_APPSTORE_PROFILE_REQUIRED')
    need(profile.get('ExpirationDate') and profile['ExpirationDate'].replace(tzinfo=dt.timezone.utc) > dt.datetime.now(dt.timezone.utc) and cert in profile.get('DeveloperCertificates', []), 'RC_PROFILE_CERTIFICATE_EXPIRED_OR_MISMATCH')
    profile_ent = profile.get('Entitlements', {})
    need(profile_ent.get('get-task-allow', True) is False and profile_ent.get('application-identifier') == team + '.' + identity['applicationIdentifier'], 'RC_DISTRIBUTION_PROFILE_IDENTITY_MISMATCH')
    reports = [job.write('verified-native-receipt.json', receipt_raw), job.json('actual-signed-artifact.json', {'artifactSha256': digest(ipa.read_bytes()), 'identity': identity, 'certificateSha256': certificate_sha256, 'entitlements': ent, 'profileRawSha256': digest(original(app / 'embedded.mobileprovision')), 'profileUuid': profile_uuid})]
    reports.append(job.write('actual-ipa-info.plist', plistlib.dumps(info)))
    reports.append(job.write('actual-ipa-build-info.json', expected_build_info))
    symbols, symbol_reports = ipa_symbols(job, archive, app, identity, receipt_raw, index_raw); reports += symbol_reports
    return reports, symbols


def ios_package(args):
    identity, receipt, raw, receipt_raw = inputs(args, 'IOS')
    need(sys.platform == 'darwin', 'RC_MACOS_XCODE_REQUIRED')
    export = no_links(receipt['artifact']); need(export.is_dir(), 'RC_IOS_EXPORT_REQUIRED')
    lock = original(export / 'Podfile.lock', args.pod_lock_sha256); original(export / 'Podfile', args.podfile_sha256)
    need(re.fullmatch(r'[A-Z0-9]{10}', args.team) and re.fullmatch(r'[0-9A-Fa-f-]{36}', args.profile_uuid) and re.fullmatch(r'[0-9a-f]{64}', args.certificate_sha256), 'RC_IOS_SIGNING_INPUT_INVALID')
    keychain = no_links(args.keychain); temp = no_links(os.environ.get('RUNNER_TEMP', '/nonexistent-runner-temp'))
    need(keychain.is_file() and keychain.is_relative_to(temp) and keychain != temp, 'RC_TEMPORARY_RUNNER_KEYCHAIN_REQUIRED')
    cert = original(args.certificate, args.certificate_sha256, 65536)
    job = Job(args.output); job.write('identity.json', raw); job.write('native-rc-receipt.json', receipt_raw)
    source_index_ref = job.write('native-inputs.json', verify_native_inputs(args, receipt, False))
    pod_lock_ref = job.write('native/Podfile.lock', lock)
    xcode = job.tool(args.xcodebuild); pod = job.tool(args.pod)
    job.run('xcode-version', [xcode, '-version']); job.run('sdk-version', [xcode, '-version', '-sdk', 'iphoneos', 'ProductVersion'])
    xver = (job.output / 'xcode-version.stdout.txt').read_text(); sdkver = (job.output / 'sdk-version.stdout.txt').read_text()
    need(re.search(r'Xcode (\d+)', xver) and int(re.search(r'Xcode (\d+)', xver)[1]) >= 26 and int(sdkver.strip().split('.')[0]) >= 26, 'RC_XCODE26_IOS26_BASELINE_REQUIRED')
    sha1 = hashlib.sha1(cert).hexdigest().upper()
    identities = job.run('available-signing-identities', ['/usr/bin/security', 'find-identity', '-v', '-p', 'codesigning', str(keychain)])
    need(sha1.encode() in identities.upper(), 'RC_PINNED_IDENTITY_NOT_IN_KEYCHAIN')
    env = subprocess_environment({'COCOAPODS_DISABLE_STATS': 'true'})
    job.run('pod-deployment', [pod, 'install', '--deployment', '--no-repo-update'], cwd=export, env=env)
    need(original(export / 'Podfile.lock') == lock, 'RC_POD_LOCK_CHANGED')
    verify_native_inputs(args, receipt, True, ('Unity-iPhone.xcodeproj/project.pbxproj',))
    workspace = export / 'Unity-iPhone.xcworkspace'; need(workspace.is_dir(), 'RC_PODS_WORKSPACE_REQUIRED')
    project_path = no_links(export / 'Unity-iPhone.xcodeproj/project.pbxproj')
    project_before = original(project_path); job.write('native/pbx-before-manual-signing.pbxproj', project_before)
    xml_project = job.run('decode-pbx-project', ['/usr/bin/plutil', '-convert', 'xml1', '-o', '-', str(project_path)])
    model = plistlib.loads(xml_project); configured = manual_project_settings(model, identity, args.team, args.profile_uuid, sha1, keychain)
    configured_raw = plistlib.dumps(configured, sort_keys=False)
    with temporary_project_settings(project_path, project_before, configured_raw):
        job.write('native/pbx-after-manual-signing.pbxproj', original(project_path))
        job.json('native/pbx-signing-delta.json', {'beforeRawSha256': digest(project_before), 'afterRawSha256': digest(configured_raw), 'scope': 'GENERATED_XCODE_APPLICATION_AND_UNITYFRAMEWORK_SIGNING_SETTINGS_ONLY'})
        archive = job.output / 'Production.xcarchive'
        job.run('xcode-archive', [xcode, '-workspace', str(workspace), '-scheme', 'Unity-iPhone', '-configuration', 'Release', '-sdk', 'iphoneos', '-destination', 'generic/platform=iOS', '-archivePath', str(archive), 'archive'], timeout=7200)
        index_ref = job.json('archive-inputs.json', archive_index(identity, receipt_raw, archive))
        index_raw = original(job.output / index_ref['path'], index_ref['rawSha256'], 8 * 1024 * 1024)
        options = {'method': 'app-store-connect', 'destination': 'export', 'teamID': args.team, 'signingStyle': 'manual', 'signingCertificate': sha1, 'provisioningProfiles': {identity['applicationIdentifier']: args.profile_uuid}, 'manageAppVersionAndBuildNumber': False, 'uploadSymbols': False}
        option_path = job.output / 'ExportOptions.plist'; job.write(option_path.name, plistlib.dumps(options))
        export_dir = job.output / 'export'
        job.run('xcode-export', [xcode, '-exportArchive', '-archivePath', str(archive), '-exportPath', str(export_dir), '-exportOptionsPlist', str(option_path)], timeout=3600)
    job.json('native/pbx-restored.json', {'rawSha256': digest(original(project_path)), 'matchesPreSigningOriginal': original(project_path) == project_before})
    ipas = list(export_dir.glob('*.ipa')); need(len(ipas) == 1, 'RC_EXACTLY_ONE_IPA_REQUIRED')
    reports, symbols = verify_actual_ipa(job, ipas[0], identity, receipt, receipt_raw, args.certificate_sha256, args.team, args.profile_uuid, original(Path(args.native_receipt).parent / 'build-info.original.json', receipt['buildInfoRawSha256'], 8192), archive, index_raw)
    reports += [source_index_ref, pod_lock_ref]
    reports.append(signing_verification(job, identity, ipas[0], args.certificate_sha256, reports.copy(), 'ACTUAL_SIGNED_IOS_IPA'))
    toolref = job.json('toolchain-report.json', {'tools': job.tools, 'podLockRawSha256': digest(lock)})
    job.json('packaging-attestation.json', {'packagingVersion': 1, 'stage': 'SIGNED_CANDIDATE', 'identity': identity, 'artifactKind': 'IPA', 'artifactSha256': digest(ipas[0].read_bytes()), 'signingFingerprintSha256': args.certificate_sha256, 'debugFlags': receipt['debugFlags'], 'nativeGateReports': reports, 'symbols': symbols, 'toolchainReports': [toolref]})


def ios_verify(args):
    identity, receipt, raw, receipt_raw = inputs(args, 'IOS', False)
    need(sys.platform == 'darwin', 'RC_MACOS_CODESIGN_REQUIRED')
    need(re.fullmatch(r'[A-Z0-9]{10}', args.team) and re.fullmatch(r'[0-9A-Fa-f-]{36}', args.profile_uuid) and re.fullmatch(r'[0-9a-f]{64}', args.certificate_sha256), 'RC_IOS_SIGNING_INPUT_INVALID')
    artifact = no_links(args.signed_artifact); original(artifact, limit=2 * 1024**3)
    job = Job(args.output); job.write('identity.json', raw)
    archive = no_links(args.archive)
    index_raw = original(args.archive_index, args.archive_index_sha256, 8 * 1024 * 1024)
    verify_archive_index(index_raw, identity, receipt_raw, archive)
    reports, _ = verify_actual_ipa(job, artifact, identity, receipt, receipt_raw, args.certificate_sha256, args.team, args.profile_uuid, original(Path(args.native_receipt).parent / 'build-info.original.json', receipt['buildInfoRawSha256'], 8192), archive, index_raw)
    reports.append(job.write('native-inputs.json', verify_native_inputs(args, receipt, False)))
    signing_verification(job, identity, artifact, args.certificate_sha256, reports, 'ACTUAL_SIGNED_IOS_IPA')


def parser():
    result = argparse.ArgumentParser(description=__doc__)
    commands = result.add_subparsers(dest='command', required=True)
    for command in ('android-sign', 'android-verify', 'ios-package', 'ios-verify'):
        p = commands.add_parser(command)
        for name in ('identity', 'identity-sha256', 'native-receipt', 'native-receipt-sha256', 'output', 'certificate-sha256'):
            p.add_argument('--' + name, required=True)
        if command.startswith('android-'):
            for name in ('jdk', 'bundletool', 'bundletool-sha256'):
                p.add_argument('--' + name, required=True)
            if command == 'android-sign':
                p.add_argument('--keystore', required=True); p.add_argument('--key-alias', required=True)
                p.add_argument('--signature-algorithm', choices=('SHA256withRSA', 'SHA256withECDSA'), default='SHA256withRSA')
            else:
                p.add_argument('--signed-artifact', required=True)
                p.add_argument('--native-output-root', help='Relocated complete native output; receipt/index bytes remain unchanged.')
        elif command == 'ios-verify':
            for name in ('signed-artifact', 'team', 'profile-uuid', 'archive', 'archive-index', 'archive-index-sha256'):
                p.add_argument('--' + name, required=True)
        else:
            for name in ('xcodebuild', 'pod', 'pod-lock-sha256', 'podfile-sha256', 'team', 'profile-uuid', 'keychain', 'certificate'):
                p.add_argument('--' + name, required=True)
    return result


def main(argv=None):
    args = parser().parse_args(argv)
    try:
        {'android-sign': android_sign, 'android-verify': android_verify, 'ios-package': ios_package, 'ios-verify': ios_verify}[args.command](args)
    except Exception as error:
        # Never include external exception messages: they may contain filesystem paths or credential-tool output.
        code = str(error) if type(error) is PackagingError else 'RC_PACKAGING_FAILED:' + type(error).__name__
        print(code, file=sys.stderr)
        return 2
    print('RC_SIGNED_VERIFICATION_COMPLETE_NO_UPLOAD_OR_PROMOTION' if args.command.endswith('-verify') else 'RC_SIGNED_PACKAGING_COMPLETE_NO_UPLOAD_OR_PROMOTION')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
