"""Protected-workflow entry for ADR047. Never signs, uploads, tags or publishes.

The caller must protect the expected trust-bundle digest and qualify its source.
Reading a JSON role/acceptedBy string is deliberately not an authorization route.
"""
from __future__ import annotations

import argparse
import os
from pathlib import Path
import re
import sys

import rc_manifest as m


def original(path, expected=None):
    path = m.no_links(path)
    m.need(path.is_file() and 0 < path.stat().st_size <= 16 * 1024 * 1024, 'RC_INPUT_MISSING')
    raw = path.read_bytes()
    if expected is not None: m.need(m.digest(raw) == m.hash_value(expected), 'RC_EXPECTED_INPUT_RAW_MISMATCH')
    return raw


def write_new(path, raw):
    path = m.no_links(path)
    m.need(path.parent.is_dir(), 'RC_OUTPUT_PARENT_MISSING')
    with path.open('xb') as stream: stream.write(raw); stream.flush(); os.fsync(stream.fileno())


def from_ci(expected_app, platform, env):
    m.need(env.get('GITHUB_ACTIONS') == 'true' and env.get('GITHUB_REF_TYPE') == 'tag', 'RC_TAG_JOB_REQUIRED')
    tag = env.get('GITHUB_REF_NAME', '')
    match = re.fullmatch(r'v((?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*))-rc\.([1-9][0-9]*)', tag)
    m.need(match is not None, 'RC_TAG_MISMATCH')
    run = env.get('GITHUB_RUN_NUMBER', ''); attempt = env.get('GITHUB_RUN_ATTEMPT', '')
    m.need(re.fullmatch('[1-9][0-9]*', run) and re.fullmatch('[1-9][0-9]*', attempt) and len(run) <= 12 and 1 <= int(attempt) <= 999, 'RC_CI_NUMBER_INVALID')
    number = str(int(run) * 1000 + int(attempt))
    m.need(int(number) <= 2100000000, 'RC_SHARED_CI_NUMBER_LIMIT')
    identity = dict(releaseVersion=match[1], candidateTag=tag, candidateOrdinal=int(match[2]), gitCommit=env.get('GITHUB_SHA'),
        workflow='release.yml@' + env.get('GITHUB_WORKFLOW_SHA', ''), platform=platform,
        buildNumber=number, buildProfile='production', applicationIdentifier=expected_app)
    return m.identity_check(identity, expected_app, number)


def context(value):
    if value is None: return None
    m.fields(value, ('subjectImmutableJcsSha256', 'authorizedOriginalHashes'))
    return m.AuthorizationContext(value['subjectImmutableJcsSha256'], value['authorizedOriginalHashes'])


def run(args):
    if args.command == 'identity-from-ci':
        value = from_ci(args.expected_app, args.platform, os.environ)
        write_new(args.out, m.canonical(value)); return
    if args.command in ('identity', 'build-info'):
        identity = m.parse(original(args.identity, args.identity_sha256))
        m.identity_check(identity, args.expected_app, args.ci_build_number)
        if args.command == 'identity': return
        package = original(args.package_lock); release = m.parse(original(args.content_lock))
        value = m.build_info(identity, expected_app=args.expected_app, ci_build_number=args.ci_build_number,
            built_at_utc=args.built_at_utc, unity_version=args.unity_version,
            package_lock_sha256=m.digest(package), content_lock_sha256=m.digest(m.canonical(release)),
            save_schema_version=args.save_schema_version, level_schema_version=args.level_schema_version,
            solver_version=args.solver_version, workflow_run_id=args.workflow_run_id)
        write_new(args.out, m.canonical(value)); return
    request = m.parse(original(args.request, args.request_sha256))
    m.fields(request, ('buildRequestVersion', 'identity', 'inputs', 'evidence', 'blockers', 'history'))
    m.need(m.version_one(request['buildRequestVersion']) and type(request['history']) is list and len(request['history']) < 256, 'RC_REQUEST_VERSION_HISTORY')
    # This is the explicitly protected caller boundary, not discovery in a report.
    trust = m.parse(original(args.trust_bundle, args.trust_bundle_sha256))
    m.fields(trust, ('receiptHashes', 'historyReceiptHashes', 'authorizationContexts'))
    m.need(type(trust['authorizationContexts']) is dict and type(trust['historyReceiptHashes']) is dict, 'RC_AUTHORIZATION_CONTEXTS')
    contexts = {name: context(value) for name, value in trust['authorizationContexts'].items()}
    m.need('current' in contexts, 'RC_CURRENT_CONTEXT_REQUIRED')
    originals = m.Originals(args.source_root); predecessor = None
    for entry in request['history']:
        m.fields(entry, ('manifest', 'provenance', 'authorizationContextKey', 'receiptTrustKey'))
        key = entry['authorizationContextKey']; m.need(key in contexts, 'RC_PRIOR_AUTHORIZATION_CONTEXT_REQUIRED')
        trust_key = entry['receiptTrustKey']; m.need(trust_key in trust['historyReceiptHashes'], 'RC_PRIOR_RECEIPT_TRUST_REQUIRED')
        predecessor = m.RetainedManifest(originals.reference(entry['manifest']), originals.reference(entry['provenance']),
            contexts[key], trust['historyReceiptHashes'][trust_key], predecessor)
    schema_root = Path(args.schema_root)
    schema = m.parse(original(schema_root/'ARCHITECTURE/schemas/release-manifest-v2.schema.json'))
    release_schema = m.parse(original(schema_root/'ARCHITECTURE/schemas/release-lock-v1.schema.json'))
    raw, provenance = m.build_manifest({key: request[key] for key in ('identity', 'inputs', 'evidence', 'blockers')},
        schema=schema, release_schema=release_schema, originals=originals, artifact_path=args.artifact,
        expected_app=args.expected_app, ci_build_number=args.ci_build_number, trusted_receipts=trust['receiptHashes'],
        authorizations=contexts['current'], predecessor=predecessor)
    m.publish(args.out, raw, provenance, originals)


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    sub = parser.add_subparsers(dest='command', required=True)
    stamp = sub.add_parser('identity-from-ci'); stamp.add_argument('--platform', choices=('ANDROID', 'IOS'), required=True)
    stamp.add_argument('--expected-app', required=True); stamp.add_argument('--out', required=True)
    for command in ('identity', 'build-info'):
        p = sub.add_parser(command)
        for flag in ('identity', 'identity-sha256', 'expected-app', 'ci-build-number'): p.add_argument('--'+flag, required=True)
        if command == 'build-info':
            for flag in ('built-at-utc', 'unity-version', 'package-lock', 'content-lock', 'solver-version', 'workflow-run-id', 'out'): p.add_argument('--'+flag, required=True)
            for flag in ('save-schema-version', 'level-schema-version'): p.add_argument('--'+flag, type=int, required=True)
    p = sub.add_parser('build')
    for flag in ('request', 'request-sha256', 'source-root', 'artifact', 'expected-app', 'ci-build-number', 'trust-bundle', 'trust-bundle-sha256', 'schema-root', 'out'): p.add_argument('--'+flag, required=True)
    try:
        args = parser.parse_args(argv); run(args)
        print('RC metadata validated. No public action or qualified approval is created by this tool.')
        return 0
    except (m.ManifestError, OSError, ValueError, KeyError, TypeError) as error:
        # Never print original bodies, passwords, arguments, environment or paths.
        print(str(error) if isinstance(error, m.ManifestError) else 'RC_INPUT_OR_OUTPUT_FAILED', file=sys.stderr)
        return 2


if __name__ == '__main__': raise SystemExit(main())
