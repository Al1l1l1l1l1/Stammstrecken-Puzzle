#!/usr/bin/env python3
"""Temporary credentials for the protected RC job; never signs or uploads itself."""
from __future__ import annotations

import argparse
import base64
import hashlib
import json
import os
from pathlib import Path
import re
import secrets
import shlex
import subprocess
import sys


class CredentialError(ValueError):
    pass


def need(condition, code):
    if not condition:
        raise CredentialError(code)


def no_links(path):
    path = Path(path).absolute()
    for part in (path, *path.parents):
        if part.exists() or part.is_symlink():
            need(not part.is_symlink() and not (getattr(part.lstat(), 'st_file_attributes', 0) & 0x400), 'RC_CREDENTIAL_LINK_FORBIDDEN')
    need(path.resolve() == path, 'RC_CREDENTIAL_CANONICAL_PATH_REQUIRED')
    return path


def run_security(*args):
    # Passwords are passed directly, never shell-interpolated or copied to logs/errors.
    try:
        result = subprocess.run(['/usr/bin/security', *args], capture_output=True, timeout=90, check=False)
    except (OSError, subprocess.SubprocessError):
        raise CredentialError('RC_KEYCHAIN_TOOL_FAILED') from None
    need(result.returncode == 0, 'RC_KEYCHAIN_TOOL_FAILED')
    return result.stdout


def decode(name, maximum):
    value = os.environ.get(name, '')
    need(bool(value) and len(value) <= maximum * 2, 'RC_CREDENTIAL_SECRET_MISSING_OR_LARGE')
    try:
        raw = base64.b64decode(value, validate=True)
    except (ValueError, base64.binascii.Error):
        raise CredentialError('RC_CREDENTIAL_BASE64_INVALID') from None
    need(0 < len(raw) <= maximum, 'RC_CREDENTIAL_SECRET_SIZE_INVALID')
    return raw


def root_path():
    value = os.environ.get('RUNNER_TEMP', '')
    need(bool(value) and Path(value).is_absolute(), 'RC_RUNNER_TEMP_REQUIRED')
    temp = no_links(value)
    need(temp.is_dir(), 'RC_RUNNER_TEMP_REQUIRED')
    return temp / 'stp-rc-credentials'


def state_write(root, value):
    path = root / 'cleanup-state.json'
    data = (json.dumps(value, sort_keys=True) + '\n').encode()
    # This is job-local cleanup state, never authority or an uploaded artifact.
    temporary = root / 'cleanup-state.next'
    with temporary.open('xb') as stream:
        stream.write(data)
    os.chmod(temporary, 0o600)
    temporary.replace(path)


def prepare(platform):
    need(os.environ.get('GITHUB_ACTIONS') == 'true', 'RC_ACTIONS_JOB_REQUIRED')
    need(os.environ.get('GITHUB_REF_TYPE') == 'tag', 'RC_TAG_JOB_REQUIRED')
    root = no_links(root_path())
    need(not root.exists(), 'RC_CREDENTIAL_ROOT_ALREADY_EXISTS')
    need(platform in ('ANDROID', 'IOS'), 'RC_CREDENTIAL_PLATFORM_INVALID')
    values = {}
    prior = []
    profile = None
    if platform == 'ANDROID':
        values['android-upload.jks'] = decode('STP_RC_ANDROID_KEYSTORE_BASE64', 4 * 1024 * 1024)
        need(all(os.environ.get(name) for name in ('STP_RC_KEYSTORE_PASSWORD', 'STP_RC_KEY_PASSWORD')), 'RC_ANDROID_PASSWORD_MISSING')
    else:
        need(sys.platform == 'darwin', 'RC_MACOS_KEYCHAIN_REQUIRED')
        values['apple-distribution.p12'] = decode('STP_RC_IOS_P12_BASE64', 4 * 1024 * 1024)
        values['apple-distribution.cer'] = decode('STP_RC_IOS_CERTIFICATE_BASE64', 65536)
        values['distribution.mobileprovision'] = decode('STP_RC_IOS_PROFILE_BASE64', 1024 * 1024)
        certificate = os.environ.get('STP_RC_CERTIFICATE_SHA256', '')
        need(re.fullmatch('[0-9a-f]{64}', certificate) and hashlib.sha256(values['apple-distribution.cer']).hexdigest() == certificate, 'RC_PUBLIC_CERTIFICATE_HASH_MISMATCH')
        need(bool(os.environ.get('STP_RC_IOS_P12_PASSWORD')), 'RC_P12_PASSWORD_MISSING')
        uuid = os.environ.get('STP_RC_IOS_PROFILE_UUID', '')
        need(re.fullmatch('[0-9a-fA-F]{8}(?:-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}', uuid), 'RC_PROFILE_UUID_INVALID')
        profile = no_links(Path.home() / 'Library/MobileDevice/Provisioning Profiles' / (uuid + '.mobileprovision'))
        need(not profile.exists(), 'RC_PROFILE_DESTINATION_ALREADY_EXISTS')
        prior = shlex.split(run_security('list-keychains', '-d', 'user').decode('utf-8', errors='strict'))
        need(all(Path(item).is_absolute() for item in prior), 'RC_KEYCHAIN_LIST_INVALID')
    root.mkdir(mode=0o700)
    state = {'version': 1, 'platform': platform,
             'files': {name: hashlib.sha256(raw).hexdigest() for name, raw in values.items()},
             'profilePath': None if profile is None else str(profile),
             'priorKeychains': prior, 'keychainPlanned': platform == 'IOS'}
    state_write(root, state)
    try:
        for name, raw in values.items():
            with (root / name).open('xb') as stream:
                stream.write(raw)
            os.chmod(root / name, 0o600)
        if platform == 'IOS':
            keychain = root / 'signing.keychain-db'
            password = secrets.token_urlsafe(40)
            run_security('create-keychain', '-p', password, str(keychain))
            run_security('set-keychain-settings', '-lut', '21600', str(keychain))
            run_security('unlock-keychain', '-p', password, str(keychain))
            run_security('import', str(root / 'apple-distribution.p12'), '-k', str(keychain),
                         '-P', os.environ['STP_RC_IOS_P12_PASSWORD'], '-T', '/usr/bin/codesign', '-T', '/usr/bin/security')
            run_security('set-key-partition-list', '-S', 'apple-tool:,apple:,codesign:', '-s', '-k', password, str(keychain))
            run_security('list-keychains', '-d', 'user', '-s', str(keychain), *prior)
            profile.parent.mkdir(parents=True, exist_ok=True)
            with profile.open('xb') as stream:
                stream.write(values['distribution.mobileprovision'])
            os.chmod(profile, 0o600)
    except BaseException:
        try:
            cleanup()
        except CredentialError:
            pass
        raise
    return root


def cleanup():
    root = no_links(root_path())
    if not root.exists():
        return
    need(root.is_dir(), 'RC_CREDENTIAL_CLEANUP_ROOT_INVALID')
    marker = no_links(root / 'cleanup-state.json')
    need(marker.is_file() and marker.stat().st_size <= 65536, 'RC_CREDENTIAL_CLEANUP_STATE_MISSING')
    try:
        state = json.loads(marker.read_bytes())
    except (ValueError, UnicodeError):
        raise CredentialError('RC_CREDENTIAL_CLEANUP_STATE_INVALID') from None
    need(type(state) is dict and set(state) == {'version', 'platform', 'files', 'profilePath', 'priorKeychains', 'keychainPlanned'}
         and type(state['version']) is int and state['version'] == 1 and state['platform'] in ('ANDROID', 'IOS'), 'RC_CREDENTIAL_CLEANUP_STATE_INVALID')
    expected_names = {'android-upload.jks'} if state['platform'] == 'ANDROID' else {'apple-distribution.p12', 'apple-distribution.cer', 'distribution.mobileprovision'}
    need(type(state['files']) is dict and set(state['files']) == expected_names
         and all(type(value) is str and re.fullmatch('[0-9a-f]{64}', value) for value in state['files'].values()), 'RC_CREDENTIAL_CLEANUP_STATE_INVALID')
    need(state['keychainPlanned'] is (state['platform'] == 'IOS') and type(state['priorKeychains']) is list
         and all(type(item) is str and Path(item).is_absolute() for item in state['priorKeychains']), 'RC_CREDENTIAL_CLEANUP_STATE_INVALID')
    failures = []
    def attempt(action):
        try:
            action()
        except (CredentialError, OSError):
            failures.append('RC_CREDENTIAL_CLEANUP_FAILED')
    if state['platform'] == 'IOS':
        need(sys.platform == 'darwin', 'RC_MACOS_KEYCHAIN_REQUIRED')
        profile = no_links(state['profilePath'])
        need(profile.parent == no_links(Path.home() / 'Library/MobileDevice/Provisioning Profiles')
             and re.fullmatch('[0-9a-fA-F]{8}(?:-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}\\.mobileprovision', profile.name), 'RC_CREDENTIAL_PROFILE_CLEANUP_INVALID')
        if profile.exists():
            def remove_profile():
                need(hashlib.sha256(profile.read_bytes()).hexdigest() == state['files']['distribution.mobileprovision'], 'RC_CREDENTIAL_PROFILE_CHANGED')
                profile.unlink()
            attempt(remove_profile)
        attempt(lambda: run_security('list-keychains', '-d', 'user', '-s', *state['priorKeychains']))
        keychain = no_links(root / 'signing.keychain-db')
        if keychain.exists():
            attempt(lambda: run_security('delete-keychain', str(keychain)))
    else:
        need(state['profilePath'] is None and not state['priorKeychains'], 'RC_CREDENTIAL_CLEANUP_STATE_INVALID')
    for name, expected in state['files'].items():
        path = no_links(root / name)
        if path.exists():
            def remove_file(path=path, expected=expected):
                need(path.is_file() and hashlib.sha256(path.read_bytes()).hexdigest() == expected, 'RC_CREDENTIAL_FILE_CHANGED')
                path.unlink()
            attempt(remove_file)
    need(not failures, 'RC_CREDENTIAL_CLEANUP_FAILED')
    need(set(path.name for path in root.iterdir()) == {'cleanup-state.json'}, 'RC_CREDENTIAL_CLEANUP_UNKNOWN_FILE')
    marker.unlink()
    root.rmdir()


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    commands = parser.add_subparsers(dest='command', required=True)
    commands.add_parser('prepare').add_argument('--platform', choices=('ANDROID', 'IOS'), required=True)
    commands.add_parser('cleanup')
    args = parser.parse_args(argv)
    try:
        if args.command == 'prepare':
            prepare(args.platform)
            print('TEMPORARY_CREDENTIALS_PREPARED; signing and upload not executed')
        else:
            cleanup()
            print('TEMPORARY_CREDENTIALS_CLEANED')
        return 0
    except (CredentialError, OSError, ValueError):
        print('RC_CREDENTIAL_OPERATION_FAILED; inspect protected configuration without logging secret values', file=sys.stderr)
        return 2


if __name__ == '__main__':
    raise SystemExit(main())
