"""Controlled metadata/ZIP/command tests. No source approval, signing key, SDK, Unity or store action."""
import copy
import importlib.util
import json
import os
import plistlib
from pathlib import Path
import struct
import sys
import tempfile
from types import SimpleNamespace
import unittest
from unittest.mock import patch
import zipfile

SPEC = importlib.util.spec_from_file_location('rc_package', Path(__file__).with_name('rc_package.py'))
rc = importlib.util.module_from_spec(SPEC); SPEC.loader.exec_module(rc)


def identity():
    return dict(releaseVersion='1.2.3', candidateTag='v1.2.3-rc.4', candidateOrdinal=4,
                gitCommit='1' * 40, workflow='release-candidate.yml@' + '2' * 40,
                platform='ANDROID', buildNumber='12003', buildProfile='production', applicationIdentifier='contract.stp.player')


def manifest():
    return b'''<manifest xmlns:android="http://schemas.android.com/apk/res/android" package="contract.stp.player" android:versionName="1.2.3" android:versionCode="12003"><uses-sdk android:minSdkVersion="26" android:targetSdkVersion="36"/><application android:debuggable="false"><meta-data android:name="com.google.android.gms.ads.APPLICATION_ID" android:value="ca-app-pub-1234567890123456~1234567890"/><meta-data android:name="com.google.android.gms.ads.DELAY_APP_MEASUREMENT_INIT" android:value="true"/></application></manifest>'''


AD = 'ca-app-pub-1234567890123456~1234567890'


def controlled_elf(align=16384, shared_relro=False):
    raw = bytearray(512); raw[:7] = b'\x7fELF\x02\x01\x01'
    struct.pack_into('<HH', raw, 16, 3, 183); struct.pack_into('<Q', raw, 32, 64); struct.pack_into('<HH', raw, 54, 56, 4)
    headers = [(1, 5, 0, 0, 0, 300, 300, align), (1, 6, 0, 16384, 0, 0, 8192 if shared_relro else 4096, align), (1, 6, 0, 32768, 0, 0, 256, align), (0x6474E552, 4, 0, 16384, 0, 0, 4096, 1)]
    for n, h in enumerate(headers):
        struct.pack_into('<IIQQQQQQ', raw, 64 + n * 56, *h)
    return bytes(raw)


def controlled_macho(signature=b'CONTROLLED-SIGNATURE', changed=False):
    raw = bytearray(256 + len(signature))
    struct.pack_into('<IIIIIIII', raw, 0, 0xfeedfacf, 0x100000c, 0, 2, 3, 112, 0, 0)
    struct.pack_into('<II16sQQQQIIII', raw, 32, 0x19, 72, b'__LINKEDIT', 0, 4096, 192, len(raw) - 192, 1, 1, 0, 0)
    struct.pack_into('<II16s', raw, 104, 0x1b, 24, bytes.fromhex('1' * 32))
    struct.pack_into('<IIII', raw, 128, 0x1d, 16, 256, len(signature))
    raw[144:256] = bytes([7 if changed else 6]) * 112; raw[256:] = signature
    return bytes(raw)


def build_info():
    return dict(buildInfoVersion=1, **{k: identity()[k] for k in ('releaseVersion', 'buildNumber', 'gitCommit', 'candidateTag', 'workflow')},
                builtAtUtc='2026-10-22T00:00:00Z', unityVersion='6000.3.23f1', packageLockSha256='a' * 64, contentLockSha256='b' * 64,
                saveSchemaVersion=3, levelSchemaVersion=2, solverVersion='solver-v2', workflowRunId='1234')


def symbol_elf(build_id=b'c' * 20):
    raw = bytearray(1024); raw[:512] = controlled_elf()
    struct.pack_into('<IIQQQQQQ', raw, 64, 4, 4, 320, 0, 0, 36, 36, 4)
    struct.pack_into('<III', raw, 320, 4, 20, 3); raw[332:336] = b'GNU\0'; raw[336:356] = build_id
    struct.pack_into('<Q', raw, 40, 512); struct.pack_into('<HH', raw, 58, 64, 1)
    struct.pack_into('<IIQQQQIIQQ', raw, 512, 0, 2, 0, 0, 640, 48, 0, 0, 8, 24)
    return raw


class PackagingTests(unittest.TestCase):
    def test_controlled_identity_is_only_job_metadata(self):
        self.assertEqual(rc.identity_check(identity(), 'ANDROID')['buildNumber'], '12003')

    def test_duplicate_raw_json_rejected(self):
        with self.assertRaises(rc.PackagingError):
            rc.read_json(b'{"releaseVersion":"1","releaseVersion":"2"}')

    def test_env_and_ci_overflow(self):
        obj = identity()
        env = dict(GITHUB_ACTIONS='true', GITHUB_REF_TYPE='tag', GITHUB_REF_NAME=obj['candidateTag'], GITHUB_SHA=obj['gitCommit'], GITHUB_WORKFLOW_SHA='2' * 40, GITHUB_RUN_NUMBER='12', GITHUB_RUN_ATTEMPT='3')
        self.assertEqual(rc.identity_check(obj, 'ANDROID', env), obj)
        for key, value in (('GITHUB_ACTIONS', 'false'), ('GITHUB_REF_TYPE', 'branch'), ('GITHUB_REF_NAME', 'v1.2.3'), ('GITHUB_RUN_ATTEMPT', '1000'), ('GITHUB_RUN_NUMBER', '2100000'), ('GITHUB_SHA', '3' * 40)):
            with self.subTest(key=key), self.assertRaises(rc.PackagingError):
                rc.identity_check(obj, 'ANDROID', dict(env, **{key: value}))

    def test_signed_marker_is_never_unsigned(self):
        with tempfile.TemporaryDirectory(prefix='stp-rc-fixture-', dir=os.environ.get('STP_TEST_TEMP_ROOT')) as temp:
            for suffix in ('RSA', 'SF', 'EC', 'DSA'):
                path = Path(temp) / (suffix + '.aab')
                with zipfile.ZipFile(path, 'w') as archive:
                    archive.writestr('BundleConfig.pb', b'x'); archive.writestr('base/manifest/AndroidManifest.xml', b'x'); archive.writestr('META-INF/DEBUG.' + suffix, b'x')
                with self.assertRaises(rc.PackagingError):
                    rc.bundle_payload(path, True)

    def test_unexpected_native_path_and_zip_traversal_rejected(self):
        with tempfile.TemporaryDirectory(prefix='stp-rc-fixture-', dir=os.environ.get('STP_TEST_TEMP_ROOT')) as temp:
            for n, name in enumerate(('base/lib/x86_64/libbad.so', '../outside', 'base/assets/renamed-elf')):
                path = Path(temp) / (str(n) + '.aab')
                with zipfile.ZipFile(path, 'w') as archive:
                    archive.writestr('BundleConfig.pb', b'x'); archive.writestr('base/manifest/AndroidManifest.xml', b'x'); archive.writestr(name, controlled_elf())
                with self.assertRaises(rc.PackagingError):
                    rc.bundle_payload(path, True)

    def test_real_header_math_relro_own_load_exception(self):
        self.assertEqual(rc.elf_loads(controlled_elf()), 3)
        for raw in (controlled_elf(4096), controlled_elf(shared_relro=True), b'notELF'):
            with self.assertRaises((rc.PackagingError, struct.error)):
                rc.elf_loads(raw)

    def test_all_payload_bytes_including_non_signature_meta_inf_are_bound(self):
        with tempfile.TemporaryDirectory(prefix='stp-rc-fixture-', dir=os.environ.get('STP_TEST_TEMP_ROOT')) as temp:
            path = Path(temp) / 'controlled.aab'
            with zipfile.ZipFile(path, 'w') as archive:
                archive.writestr('BundleConfig.pb', b'x'); archive.writestr('base/manifest/AndroidManifest.xml', b'x'); archive.writestr('base/lib/arm64-v8a/libcontrolled.so', controlled_elf()); archive.writestr('META-INF/NOTICE', b'original')
            payload, libs = rc.bundle_payload(path, True)
            self.assertIn('META-INF/NOTICE', payload); self.assertEqual(len(libs), 1)

    def test_manifest_current_identity_is_necessary_not_approval(self):
        self.assertEqual(rc.manifest_check(manifest(), identity(), AD).tag, 'manifest')

    def test_fresh_output_cannot_overwrite_prior_failure_or_success(self):
        with tempfile.TemporaryDirectory(prefix='stp-rc-fixture-', dir=os.environ.get('STP_TEST_TEMP_ROOT')) as temp:
            with self.assertRaises(rc.PackagingError):
                rc.Job(temp)

    def test_job_argv_is_literal_and_diagnostics_redacted(self):
        with tempfile.TemporaryDirectory(prefix='stp-rc-fixture-', dir=os.environ.get('STP_TEST_TEMP_ROOT')) as temp:
            job = rc.Job(Path(temp) / 'new'); job.redactions = ['controlled-secret-value']
            raw = job.run('literal', [sys.executable, '-c', 'import sys;print(sys.argv[1]);print("controlled-secret-value",file=sys.stderr)', 'space " quote ; $(not-shell)'])
            self.assertEqual(raw.strip(), b'space " quote ; $(not-shell)')
            for path in job.output.rglob('*'):
                if path.is_file(): self.assertNotIn(b'controlled-secret-value', path.read_bytes())

    def test_job_timeout_is_bounded_and_has_no_success(self):
        with tempfile.TemporaryDirectory(prefix='stp-rc-fixture-', dir=os.environ.get('STP_TEST_TEMP_ROOT')) as temp:
            job = rc.Job(Path(temp) / 'new')
            with self.assertRaises(rc.PackagingError):
                job.run('held', [sys.executable, '-c', 'import time;time.sleep(20)'], timeout=.1)
            self.assertTrue(json.loads((job.output / 'held.exit.json').read_text())['timedOut'])

    def test_only_explicit_secrets_can_reach_signer_environment(self):
        with patch.dict(os.environ, {'STP_RC_KEYSTORE_PASSWORD': 'controlled-keystore', 'GITHUB_TOKEN': 'controlled-token', 'JAVA_TOOL_OPTIONS': '-arbitrary'}):
            env = rc.subprocess_environment()
            self.assertNotIn('STP_RC_KEYSTORE_PASSWORD', env); self.assertNotIn('GITHUB_TOKEN', env); self.assertNotIn('JAVA_TOOL_OPTIONS', env)
            self.assertEqual(rc.subprocess_environment({'STP_RC_KEY_PASSWORD': 'controlled-only'})['STP_RC_KEY_PASSWORD'], 'controlled-only')

    def test_python311_reparse_attribute_is_rejected_without_is_junction_api(self):
        with patch.object(rc.os, 'lstat', return_value=SimpleNamespace(st_mode=0o040755, st_file_attributes=0x400)):
            with self.assertRaises(rc.PackagingError):
                rc.no_links(Path.cwd() / 'controlled-junction-marker')

    def test_application_profile_is_not_a_global_framework_build_setting(self):
        model = {'objects': {'a': {'isa': 'PBXNativeTarget', 'productType': 'com.apple.product-type.application', 'name': 'Unity-iPhone', 'buildConfigurationList': 'ac'}, 'f': {'isa': 'PBXNativeTarget', 'productType': 'com.apple.product-type.framework', 'name': 'UnityFramework', 'buildConfigurationList': 'fc'}, 'ac': {'isa': 'XCConfigurationList', 'buildConfigurations': ['ab']}, 'fc': {'isa': 'XCConfigurationList', 'buildConfigurations': ['fb']}, 'ab': {'isa': 'XCBuildConfiguration', 'buildSettings': {'PRODUCT_BUNDLE_IDENTIFIER': 'contract.stp.player'}}, 'fb': {'isa': 'XCBuildConfiguration', 'buildSettings': {}}}}
        output = rc.manual_project_settings(model, identity(), 'ABCDEFGHIJ', '00000000-0000-0000-0000-000000000000', 'A' * 40, '/runner/temp/key chain')
        self.assertIn('PROVISIONING_PROFILE_SPECIFIER', output['objects']['ab']['buildSettings']); self.assertNotIn('PROVISIONING_PROFILE_SPECIFIER', output['objects']['fb']['buildSettings'])
        self.assertEqual(model['objects']['ab']['buildSettings'], {'PRODUCT_BUNDLE_IDENTIFIER': 'contract.stp.player'})
        model['objects']['fb']['buildSettings']['PROVISIONING_PROFILE_SPECIFIER'] = 'unexpected'
        with self.assertRaises(rc.PackagingError):
            rc.manual_project_settings(model, identity(), 'ABCDEFGHIJ', 'x', 'A' * 40, '/tmp/keychain')

    def test_missing_inputs_exit_before_output_or_subprocess(self):
        with tempfile.TemporaryDirectory(prefix='stp-rc-fixture-', dir=os.environ.get('STP_TEST_TEMP_ROOT')) as temp:
            output = Path(temp) / 'new'
            argv = ['android-verify', '--identity', str(Path(temp) / 'missing'), '--identity-sha256', 'a' * 64, '--native-receipt', 'missing', '--native-receipt-sha256', 'b' * 64, '--output', str(output), '--certificate-sha256', 'c' * 64, '--jdk', 'missing', '--bundletool', 'missing', '--bundletool-sha256', 'd' * 64, '--signed-artifact', 'missing']
            with patch.object(rc.subprocess, 'Popen') as proc:
                self.assertEqual(rc.main(argv), 2); proc.assert_not_called(); self.assertFalse(output.exists())

    def test_build_info_exact_actual_fourteen_fields(self):
        obj = build_info(); raw = json.dumps(obj).encode()
        self.assertEqual(rc.build_info_check(raw, identity()), obj)
        for update in ({'extra': True}, {'saveSchemaVersion': True}, {'releaseVersion': '2.0.0'}, {'builtAtUtc': '2026-02-30T00:00:00Z'}, {'solverVersion': 'solver-v1'}, {'packageLockSha256': 'not-hash'}):
            with self.subTest(update=update), self.assertRaises((rc.PackagingError, ValueError)):
                rc.build_info_check(json.dumps(dict(obj, **update)).encode(), identity())

    def test_native_input_index_binds_actual_android_symbols_and_drift(self):
        with tempfile.TemporaryDirectory(dir=os.environ.get('STP_TEST_TEMP_ROOT')) as temp:
            root = Path(temp); artifact = root / 'game.aab'; artifact.write_bytes(b'controlled-aab')
            symbol = root / 'game.symbols.zip'; symbol.write_bytes(b'controlled-symbol')
            obj = {'nativeInputIndexVersion': 1, 'artifactKind': 'AAB', 'files': [{'path': p.name, 'rawSha256': rc.digest(p.read_bytes())} for p in (artifact, symbol)]}
            raw = json.dumps(obj).encode(); (root / 'native-inputs.json').write_bytes(raw)
            receipt = {'artifact': str(artifact), 'stage': 'UNSIGNED_AAB', 'nativeInputIndexRawSha256': rc.digest(raw)}
            args = SimpleNamespace(native_receipt=root / 'receipt.json')
            self.assertEqual(rc.verify_native_inputs(args, receipt, True), raw)
            symbol.write_bytes(b'changed-symbol')
            with self.assertRaises(rc.PackagingError): rc.verify_native_inputs(args, receipt, True)
            obj['files'] = obj['files'][:1]; raw = json.dumps(obj).encode(); (root / 'native-inputs.json').write_bytes(raw); receipt['nativeInputIndexRawSha256'] = rc.digest(raw)
            with self.assertRaises(rc.PackagingError): rc.verify_native_inputs(args, receipt, True)

    def test_temporary_project_settings_restore_on_success_and_archive_throw(self):
        with tempfile.TemporaryDirectory(dir=os.environ.get('STP_TEST_TEMP_ROOT')) as temp:
            path = Path(temp) / 'project.pbxproj'; path.write_bytes(b'original exact CRLF\r\n')
            for throwing in (False, True):
                try:
                    with rc.temporary_project_settings(path, b'original exact CRLF\r\n', b'changed'):
                        self.assertEqual(path.read_bytes(), b'changed')
                        if throwing: raise RuntimeError('controlled-archive-failure')
                except RuntimeError:
                    self.assertTrue(throwing)
                self.assertEqual(path.read_bytes(), b'original exact CRLF\r\n')

    def test_macho_resigning_changes_only_signature_and_linkedit_sizes(self):
        first = rc.macho_code(controlled_macho())
        self.assertEqual(first, rc.macho_code(controlled_macho(b'LONGER-CONTROLLED-RESIGNATURE')))
        self.assertNotEqual(first, rc.macho_code(controlled_macho(changed=True)))
        with self.assertRaises(rc.PackagingError): rc.macho_code(b'foreign')
        encrypted = bytearray(controlled_macho()); struct.pack_into('<I', encrypted, 4, 7)
        with self.assertRaises(rc.PackagingError): rc.macho_code(encrypted)
        oversized = bytearray(controlled_macho()); struct.pack_into('<Q', oversized, 64, 0x4000000000000000)
        with self.assertRaises(rc.PackagingError): rc.macho_code(oversized)
        bad_extent = bytearray(controlled_macho()); struct.pack_into('<Q', bad_extent, 80, 1)
        with self.assertRaises(rc.PackagingError): rc.macho_code(bad_extent)

    def test_archive_index_and_final_payload_reject_foreign_same_metadata_binary(self):
        with tempfile.TemporaryDirectory(dir=os.environ.get('STP_TEST_TEMP_ROOT')) as temp:
            root = Path(temp); archive = root / 'archive'; app = archive / 'Products/Applications/Player.app'; actual = root / 'actual'
            app.mkdir(parents=True); actual.mkdir()
            info = plistlib.dumps({'CFBundleExecutable': 'Player', 'CFBundleIdentifier': 'contract.stp.player'})
            for target in (app, actual):
                (target / 'Info.plist').write_bytes(info); (target / 'Player').write_bytes(controlled_macho())
            index = rc.archive_index(identity(), b'exact-native-receipt', archive); raw = json.dumps(index).encode()
            rc.verify_archive_index(raw, identity(), b'exact-native-receipt', archive)
            (actual / 'Player').write_bytes(controlled_macho(b'DIFFERENT-VALID-SIGNATURE'))
            self.assertIn('Player', rc.compare_archive_app(app, actual))
            (actual / 'Player').write_bytes(controlled_macho(changed=True))
            with self.assertRaises(rc.PackagingError): rc.compare_archive_app(app, actual)
            (app / 'new-unindexed').write_bytes(b'drift')
            with self.assertRaises(rc.PackagingError): rc.verify_archive_index(raw, identity(), b'exact-native-receipt', archive)
            with self.assertRaises(rc.PackagingError): rc.verify_archive_index(raw, identity(), b'foreign-receipt', archive)

    def test_elf_symbol_build_id_is_actual_note_not_filename(self):
        raw = bytearray(controlled_elf()); struct.pack_into('<IIQQQQQQ', raw, 64, 4, 4, 320, 0, 0, 36, 36, 4)
        struct.pack_into('<III', raw, 320, 4, 20, 3); raw[332:336] = b'GNU\0'; raw[336:356] = b'c' * 20
        self.assertEqual(rc.elf_build_id(raw), (b'c' * 20).hex())
        with self.assertRaises(rc.PackagingError): rc.elf_build_id(controlled_elf())

    def test_actual_symbol_zip_core_build_ids_bind_bundle_not_just_metadata(self):
        with tempfile.TemporaryDirectory(dir=os.environ.get('STP_TEST_TEMP_ROOT')) as temp:
            root = Path(temp); artifact = root / 'controlled.aab'; symbol = root / 'controlled.symbols.zip'
            core = ('libmain.so', 'libunity.so', 'libil2cpp.so')
            with zipfile.ZipFile(artifact, 'w') as z:
                for name in core: z.writestr('base/lib/arm64-v8a/' + name, symbol_elf())
            def inputs(mismatch=False, no_table=False):
                with zipfile.ZipFile(symbol, 'w') as z:
                    for name in core:
                        raw = symbol_elf(b'd' * 20 if mismatch and name == 'libunity.so' else b'c' * 20)
                        if no_table: struct.pack_into('<H', raw, 60, 0)
                        z.writestr('arm64-v8a/' + name, raw)
                obj = {'nativeInputIndexVersion': 1, 'artifactKind': 'AAB', 'files': [{'path': p.name, 'rawSha256': rc.digest(p.read_bytes())} for p in (artifact, symbol)]}
                raw = json.dumps(obj).encode(); (root / 'native-inputs.json').write_bytes(raw)
                return {'artifact': str(artifact), 'stage': 'UNSIGNED_AAB', 'nativeInputIndexRawSha256': rc.digest(raw)}
            receipt = inputs(); args = SimpleNamespace(native_receipt=root / 'receipt.json')
            refs, evidence = rc.android_symbols(rc.Job(root / 'positive'), args, receipt, artifact)
            self.assertEqual(len(refs), 1); self.assertEqual(len(evidence), 2)
            for n, changes in enumerate(({'mismatch': True}, {'no_table': True})):
                receipt = inputs(**changes)
                with self.assertRaises(rc.PackagingError): rc.android_symbols(rc.Job(root / ('negative-' + str(n))), args, receipt, artifact)

    def test_final_ipa_symbols_require_its_actual_archive_uuid(self):
        with tempfile.TemporaryDirectory(dir=os.environ.get('STP_TEST_TEMP_ROOT')) as temp:
            root = Path(temp); archive = root / 'archive'; app = archive / 'Products/Applications/Player.app'; actual = root / 'actual'
            app.mkdir(parents=True); actual.mkdir()
            for target in (app, actual):
                (target / 'Player').write_bytes(controlled_macho())
                (target / 'Info.plist').write_bytes(plistlib.dumps({'CFBundleExecutable': 'Player'}))
            dsym = archive / 'dSYMs/Player.app.dSYM/Contents/Resources/DWARF/Player'; dsym.parent.mkdir(parents=True); dsym.write_bytes(b'CONTROLLED_DSYM_TOOL_STUB_ONLY')
            obj = dict(identity(), platform='IOS'); index = json.dumps(rc.archive_index(obj, b'bound-source', archive)).encode()
            good = rc.Job(root / 'good'); good.run = lambda *a: b'UUID: 11111111-1111-1111-1111-111111111111 (arm64) controlled'
            symbols, reports = rc.ipa_symbols(good, archive, actual, obj, b'bound-source', index)
            self.assertEqual(1, len(symbols)); self.assertEqual(2, len(reports))
            bad = rc.Job(root / 'bad'); bad.run = lambda *a: b'UUID: 22222222-2222-2222-2222-222222222222 (arm64) controlled'
            with self.assertRaises(rc.PackagingError): rc.ipa_symbols(bad, archive, actual, obj, b'bound-source', index)
            self.assertFalse((bad.output / 'signing-verification.json').exists())

    def test_downloaded_android_originals_map_windows_and_posix_without_old_paths(self):
        for recorded_root in ('C:\\old-runner\\Bee\\Gradle', '/old-runner/Bee/Gradle'):
            with self.subTest(origin=recorded_root), tempfile.TemporaryDirectory(dir=os.environ.get('STP_TEST_TEMP_ROOT')) as temp:
                root = Path(temp); native = root / 'download'; native.mkdir(); sep = '\\' if recorded_root.startswith('C:') else '/'
                (native / 'receipt.json').write_bytes(b'original receipt with old absolute paths')
                (native / 'game.aab').write_bytes(b'controlled AAB'); (native / 'game.symbols.zip').write_bytes(b'controlled symbols')
                receipt = {'artifact': recorded_root + sep + 'game.aab', 'stage': 'UNSIGNED_AAB', 'androidRoot': recorded_root, 'scope': 'a' * 32, 'adMobAppId': AD}
                for task in ('processReleaseMainManifest', 'processReleaseManifestForPackage'):
                    relative = 'launcher/build/intermediates/' + task + '/AndroidManifest.xml'
                    path = native / 'android-native-originals' / relative; path.parent.mkdir(parents=True); path.write_bytes(manifest())
                    report = native / 'android-native-originals/launcher/build/reports/stp-production-native' / receipt['scope'] / (task + '-raw.txt'); report.parent.mkdir(parents=True, exist_ok=True)
                    report.write_bytes(('scope='+receipt['scope']+'\r\ntask='+task+'\r\nmanifest='+recorded_root+sep+relative.replace('/', sep)+'\r\nsha256='+rc.digest(manifest())+'\r\napplicationId=contract.stp.player\r\nadMobAppId='+AD+'\r\n').encode())
                def reindex():
                    files=[{'path':p.relative_to(native).as_posix(),'rawSha256':rc.digest(p.read_bytes())} for p in native.rglob('*') if p.is_file() and p.name not in ('receipt.json','native-inputs.json')]
                    raw=json.dumps({'nativeInputIndexVersion':1,'artifactKind':'AAB','files':files}).encode();(native/'native-inputs.json').write_bytes(raw);receipt['nativeInputIndexRawSha256']=rc.digest(raw)
                reindex(); args=SimpleNamespace(native_receipt=native/'receipt.json',native_output_root=native)
                before={p:p.read_bytes() for p in native.rglob('*') if p.is_file()}
                self.assertEqual(rc.native_aab(args,receipt),native/'game.aab')
                self.assertEqual(4,len(rc.native_reports(rc.Job(root/'audit'),args,receipt,identity())))
                self.assertTrue(all(p.read_bytes()==raw for p,raw in before.items()))
                report=next((native/'android-native-originals').rglob('*-raw.txt'));report.write_bytes(report.read_bytes().replace(b'manifest='+recorded_root.encode(),b'manifest='+recorded_root.encode()+sep.encode()+b'..'+sep.encode()+b'foreign'))
                reindex()
                with self.assertRaises(rc.PackagingError):rc.native_reports(rc.Job(root/'foreign'),args,receipt,identity())
                foreign=SimpleNamespace(native_receipt=native/'receipt.json',native_output_root=root)
                with self.assertRaises(rc.PackagingError):rc.verify_native_inputs(foreign,receipt,True)

    def test_readonly_ios_source_index_never_accesses_original_runner_path(self):
        with tempfile.TemporaryDirectory(dir=os.environ.get('STP_TEST_TEMP_ROOT')) as temp:
            root=Path(temp); raw=json.dumps({'nativeInputIndexVersion':1,'artifactKind':'IOS_SOURCE_EXPORT','files':[{'path':'Data/original','rawSha256':'a'*64}]}).encode();(root/'native-inputs.json').write_bytes(raw)
            receipt={'artifact':'/nonexistent/old-runner/Xcode','stage':'IOS_SOURCE_EXPORT','nativeInputIndexRawSha256':rc.digest(raw)}
            with patch.object(rc, 'no_links', wraps=rc.no_links) as paths:
                self.assertEqual(raw,rc.verify_native_inputs(SimpleNamespace(native_receipt=root/'receipt.json'),receipt,False))
                self.assertFalse(any('/nonexistent/' in str(call.args[0]) for call in paths.call_args_list))


BAD_IDENTITIES = {
    'extra': {'unknown': True}, 'qa': {'buildProfile': 'qa'}, 'fixture_platform': {'platform': 'FIXTURE'},
    'tag_mismatch': {'candidateTag': 'v1.2.3-rc.5'}, 'bool_ordinal': {'candidateOrdinal': True}, 'zero_ordinal': {'candidateOrdinal': 0},
    'leading_zero_version': {'releaseVersion': '01.2.3'}, 'huge_build': {'buildNumber': '2100000001'}, 'zero_build': {'buildNumber': '0'}, 'bool_build': {'buildNumber': True},
    'short_commit': {'gitCommit': '1' * 39}, 'bad_workflow': {'workflow': 'release.yml@main'}, 'qa_app': {'applicationIdentifier': 'contract.stp.qa'}, 'human_app': {'applicationIdentifier': 'contract.stp.human'},
}
for name, updates in BAD_IDENTITIES.items():
    def case(self, updates=updates):
        with self.assertRaises(rc.PackagingError):
            rc.identity_check(dict(identity(), **updates), 'ANDROID')
    setattr(PackagingTests, 'test_identity_reject_' + name, case)

BAD_MANIFESTS = {
    'different_version': (b'android:versionCode="12003"', b'android:versionCode="12004"'),
    'different_package': (b'package="contract.stp.player"', b'package="contract.stp.other"'),
    'debuggable': (b'android:debuggable="false"', b'android:debuggable="true"'),
    'old_target': (b'android:targetSdkVersion="36"', b'android:targetSdkVersion="35"'),
    'delay_off': (b'INIT" android:value="true"', b'INIT" android:value="false"'),
    'duplicate_gma': (b'</application>', b'<meta-data android:name="com.google.android.gms.ads.APPLICATION_ID" android:value="other"/></application>'),
    'ad_id': (b'</manifest>', b'<uses-permission android:name="com.google.android.gms.permission.AD_ID"/></manifest>'),
    'ad_id_sdk23': (b'</manifest>', b'<uses-permission-sdk-23 android:name="com.google.android.gms.permission.AD_ID"/></manifest>'),
    'firebase': (b'</application>', b'<provider android:name="com.google.firebase.Provider"/></application>'),
}
for name, (old, new) in BAD_MANIFESTS.items():
    def case(self, old=old, new=new):
        with self.assertRaises(rc.PackagingError):
            rc.manifest_check(manifest().replace(old, new), identity(), AD)
    setattr(PackagingTests, 'test_manifest_reject_' + name, case)


if __name__ == '__main__':
    unittest.main(verbosity=2)
