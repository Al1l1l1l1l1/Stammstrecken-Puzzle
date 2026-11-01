#!/usr/bin/env python3
"""Controlled lifecycle tests. Bytes are fixtures, not usable signing credentials."""
import base64
import hashlib
import json
import os
from pathlib import Path
import tempfile
import unittest
from unittest import mock
import rc_ci_credentials as subject


class CredentialsTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory(prefix='credentials-test-', dir=Path(__file__).resolve().parents[2] / '.utmp')
        self.addCleanup(self.temp.cleanup)
        self.base = Path(self.temp.name)
        self.env = {'RUNNER_TEMP': str(self.base), 'GITHUB_ACTIONS': 'true', 'GITHUB_REF_TYPE': 'tag',
                    'STP_RC_ANDROID_KEYSTORE_BASE64': base64.b64encode(b'CONTROLLED NOT A KEYSTORE').decode(),
                    'STP_RC_KEYSTORE_PASSWORD': 'test-only-store-password', 'STP_RC_KEY_PASSWORD': 'test-only-key-password'}
        self.patch = mock.patch.dict(os.environ, self.env, clear=True)
        self.patch.start()
        self.addCleanup(self.patch.stop)

    def ios(self):
        os.environ.update({'STP_RC_IOS_P12_BASE64': base64.b64encode(b'CONTROLLED NOT A P12').decode(),
            'STP_RC_IOS_CERTIFICATE_BASE64': base64.b64encode(b'CONTROLLED NOT A CERTIFICATE').decode(),
            'STP_RC_IOS_PROFILE_BASE64': base64.b64encode(b'CONTROLLED NOT A PROFILE').decode(),
            'STP_RC_CERTIFICATE_SHA256': hashlib.sha256(b'CONTROLLED NOT A CERTIFICATE').hexdigest(),
            'STP_RC_IOS_P12_PASSWORD': 'test-only-p12-password',
            'STP_RC_IOS_PROFILE_UUID': 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee'})
        self.calls = []
        self.prior = (self.base / 'prior/login.keychain-db').as_posix()
        def security(*args):
            self.calls.append(args)
            if args == ('list-keychains', '-d', 'user'):
                return ('"' + self.prior + '"\n').encode()
            if args[0] == 'create-keychain':
                Path(args[-1]).write_bytes(b'CONTROLLED KEYCHAIN PLACEHOLDER')
            if args[0] == 'delete-keychain':
                Path(args[-1]).unlink()
            return b''
        return mock.patch.object(subject, 'run_security', side_effect=security)

    def test_android_prepare_cleanup_and_idempotent_cleanup(self):
        root = subject.prepare('ANDROID')
        self.assertEqual(b'CONTROLLED NOT A KEYSTORE', (root / 'android-upload.jks').read_bytes())
        state = (root / 'cleanup-state.json').read_bytes()
        self.assertNotIn(b'test-only-store-password', state)
        self.assertNotIn(os.environ['STP_RC_ANDROID_KEYSTORE_BASE64'].encode(), state)
        subject.cleanup()
        self.assertFalse(root.exists())
        subject.cleanup()

    def test_missing_secret_creates_no_output(self):
        del os.environ['STP_RC_ANDROID_KEYSTORE_BASE64']
        with self.assertRaises(subject.CredentialError): subject.prepare('ANDROID')
        self.assertFalse(subject.root_path().exists())

    def test_invalid_base64_and_oversized_secret_fail_before_mutation(self):
        for value in ('not base64!', base64.b64encode(b'x' * (4 * 1024 * 1024 + 1)).decode()):
            with mock.patch.object(subject.os, 'environ', dict(os.environ, STP_RC_ANDROID_KEYSTORE_BASE64=value)):
                with self.assertRaises(subject.CredentialError): subject.prepare('ANDROID')
            self.assertFalse(subject.root_path().exists())

    def test_nonci_nontag_missing_password_denied(self):
        for key, value in [('GITHUB_ACTIONS', 'false'), ('GITHUB_REF_TYPE', 'branch'), ('STP_RC_KEY_PASSWORD', '')]:
            with mock.patch.dict(os.environ, {key: value}):
                with self.assertRaises(subject.CredentialError): subject.prepare('ANDROID')
                self.assertFalse(subject.root_path().exists())

    def test_existing_root_never_overwritten(self):
        subject.root_path().mkdir()
        sentinel = subject.root_path() / 'unrelated.txt'
        sentinel.write_bytes(b'KEEP')
        with self.assertRaises(subject.CredentialError): subject.prepare('ANDROID')
        self.assertEqual(b'KEEP', sentinel.read_bytes())

    def test_cleanup_modified_credential_preserves_it_and_fails(self):
        root = subject.prepare('ANDROID')
        (root / 'android-upload.jks').write_bytes(b'CHANGED')
        with self.assertRaises(subject.CredentialError): subject.cleanup()
        self.assertEqual(b'CHANGED', (root / 'android-upload.jks').read_bytes())

    def test_cleanup_unknown_file_is_never_recursively_removed(self):
        root = subject.prepare('ANDROID')
        (root / 'foreign').write_bytes(b'KEEP')
        with self.assertRaises(subject.CredentialError): subject.cleanup()
        self.assertEqual(b'KEEP', (root / 'foreign').read_bytes())

    def test_bad_cleanup_state_cannot_select_foreign_file(self):
        root = subject.prepare('ANDROID')
        state = json.loads((root / 'cleanup-state.json').read_bytes())
        state['files'] = {'../outside': 'a' * 64}
        (root / 'cleanup-state.json').write_text(json.dumps(state))
        with self.assertRaises(subject.CredentialError): subject.cleanup()
        self.assertTrue((root / 'android-upload.jks').is_file())

    def test_ios_lifecycle_restores_searchlist_and_only_own_profile(self):
        with self.ios(), mock.patch.object(subject.sys, 'platform', 'darwin'), mock.patch.object(subject.Path, 'home', return_value=self.base / 'home'):
            root = subject.prepare('IOS')
            profile = self.base / 'home/Library/MobileDevice/Provisioning Profiles/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee.mobileprovision'
            self.assertTrue(profile.is_file())
            self.assertNotIn('test-only-p12-password', (root / 'cleanup-state.json').read_text())
            subject.cleanup()
            self.assertFalse(root.exists())
            self.assertFalse(profile.exists())
            self.assertIn(('list-keychains', '-d', 'user', '-s', self.prior), self.calls)
            imports = [args for args in self.calls if args[0] == 'import']
            self.assertEqual(1, len(imports))
            self.assertNotIn('-A', imports[0])

    def test_ios_wrong_certificate_fails_before_keychain_mutation(self):
        with self.ios() as runner, mock.patch.object(subject.sys, 'platform', 'darwin'):
            os.environ['STP_RC_CERTIFICATE_SHA256'] = 'a' * 64
            with self.assertRaises(subject.CredentialError): subject.prepare('IOS')
            runner.assert_not_called()
            self.assertFalse(subject.root_path().exists())

    def test_ios_existing_profile_is_preserved(self):
        with self.ios() as runner, mock.patch.object(subject.sys, 'platform', 'darwin'), mock.patch.object(subject.Path, 'home', return_value=self.base / 'home'):
            profile = self.base / 'home/Library/MobileDevice/Provisioning Profiles/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee.mobileprovision'
            profile.parent.mkdir(parents=True)
            profile.write_bytes(b'EXISTING KEEP')
            with self.assertRaises(subject.CredentialError): subject.prepare('IOS')
            runner.assert_not_called()
            self.assertEqual(b'EXISTING KEEP', profile.read_bytes())

    def test_ios_partial_failure_cleans_files_and_restores_searchlist(self):
        with self.ios() as runner, mock.patch.object(subject.sys, 'platform', 'darwin'), mock.patch.object(subject.Path, 'home', return_value=self.base / 'home'):
            original = runner.side_effect
            def fail_import(*args):
                if args[0] == 'import': raise subject.CredentialError('CONTROLLED IMPORT FAILURE')
                return original(*args)
            runner.side_effect = fail_import
            with self.assertRaises(subject.CredentialError): subject.prepare('IOS')
            self.assertFalse(subject.root_path().exists())
            self.assertIn(('list-keychains', '-d', 'user', '-s', self.prior), self.calls)

    def test_security_failure_never_echoes_secret_or_tool_diagnostics(self):
        fake = mock.Mock(returncode=1, stdout=b'', stderr=b'TEST PRIVATE DIAGNOSTIC')
        with mock.patch.object(subject.subprocess, 'run', return_value=fake):
            with self.assertRaises(subject.CredentialError) as result: subject.run_security('import', '-P', 'SECRET TEST')
            self.assertEqual('RC_KEYCHAIN_TOOL_FAILED', str(result.exception))


if __name__ == '__main__':
    unittest.main(verbosity=2)
