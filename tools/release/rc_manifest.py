"""ADR047 manifest reader. No signing, upload, tag, SDK or authorization discovery.

AuthorizationContext and trusted_receipts are explicit protected-caller inputs. A role,
PASS string or digest found in an assessed file can never create either capability.
"""
from __future__ import annotations

import copy
import hashlib
import json
import os
import re
import shutil
import stat
from dataclasses import dataclass
from pathlib import Path
from types import MappingProxyType

import jsonschema

IDENTITY = ('releaseVersion', 'candidateTag', 'candidateOrdinal', 'gitCommit', 'workflow', 'platform', 'buildNumber', 'buildProfile', 'applicationIdentifier')
DEBUG = ('developmentBuild', 'profiler', 'testCrash', 'sandboxProviders')
GATES = ('SOURCE_CALIBRATION', 'OWNER_GAME_ACCEPTANCE', 'SIGNING_PACKAGE', 'DEVICE_REGRESSION', 'PROVIDER_PRIVACY', 'RIGHTS_LEGAL', 'STORE_CHANNEL', 'INDEPENDENT_RC')
BLOCKERS = ('BLOCKER-PROD-001', 'BLOCKER-PROD-002', 'BLOCKER-PROD-003')
MUTABLE = frozenset(('evidenceGates', 'blockerState', 'promotionAllowed'))
INPUTS = ('toolchainLock', 'packageLock', 'releaseLock', 'approvalLock', 'productionConfig', 'blockersSource', 'runtimeBindings', 'signingVerification', 'storeReceipt')
PROFILES = ('dienstbeginn', 'grosslage', 'regelbetrieb', 'stoerungslage')
CHECKS = ('ACTUAL_ARTIFACT_SIGNATURE', 'ACTUAL_PACKAGE_VERSION', 'ACTUAL_NATIVE_PRIVACY', 'IDENTITY_BOUND')
RUNTIME_FIELDS = ('runtimeImportVersion', 'use', 'status', 'releaseLockRawSha256', 'releaseLockJcsSha256', 'approvalLockRawSha256', 'publicCatalog', 'campaignCatalog', 'campaignCompletionCatalog', 'endlessCompletionCatalog', 'calibrationCatalog', 'cosmeticsCatalog', 'qualityProfiles', 'catalogSet', 'railwayAssetsRawSha256', 'campaignTextRawSha256', 'uiSourceLockRawSha256', 'playerFiles')


class ManifestError(ValueError):
    pass


def need(value, code):
    if not value:
        raise ManifestError(code)


def fields(value, names):
    need(type(value) is dict and set(value) == set(names), 'RC_FIELDS_INVALID')


def version_one(value):
    return type(value) is int and value == 1


def no_links(path):
    path = Path(os.path.abspath(path))
    for node in (path, *path.parents):
        try: info = os.lstat(node)
        except FileNotFoundError: continue
        need(not stat.S_ISLNK(info.st_mode) and not getattr(info, 'st_file_attributes', 0) & 0x400, 'RC_REPARSE_FORBIDDEN')
    return path


def artifact_hash(path):
    path = no_links(path)
    need(path.is_file() and path.stat().st_size > 0, 'RC_ARTIFACT_MISSING')
    with path.open('rb') as stream: return hashlib.file_digest(stream, 'sha256').hexdigest()


def digest(raw):
    return hashlib.sha256(raw).hexdigest()


def hash_value(value):
    need(type(value) is str and re.fullmatch('[0-9a-f]{64}', value), 'RC_HASH_INVALID')
    return value


def text(value):
    need(type(value) is str and bool(value.strip()) and not any(ord(c) < 32 or 0xd800 <= ord(c) <= 0xdfff for c in value), 'RC_TEXT_INVALID')
    return value


def pairs(items):
    result = {}
    for key, value in items:
        need(key not in result, 'RC_DUPLICATE_FIELD')
        result[key] = value
    return result


def parse(raw):
    need(type(raw) is bytes and 0 < len(raw) <= 16 * 1024 * 1024, 'RC_JSON_SIZE')
    try:
        value = json.loads(raw.decode('utf-8'), object_pairs_hook=pairs,
                           parse_float=lambda _: (_ for _ in ()).throw(ManifestError('RC_INTEGER_JSON_REQUIRED')),
                           parse_constant=lambda _: (_ for _ in ()).throw(ManifestError('RC_NONFINITE_JSON')))
        canonical(value)
        return value
    except (UnicodeError, json.JSONDecodeError, RecursionError) as exc:
        raise ManifestError('RC_JSON_INVALID') from exc


def canonical(value):
    # All hashed contracts here have closed ASCII member names, safe integers,
    # booleans/null and Unicode strings: this is their existing JCS projection.
    def check(v):
        if type(v) is dict:
            for k, x in v.items():
                need(type(k) is str and k.isascii(), 'RC_JCS_MEMBER_INVALID')
                check(x)
        elif type(v) is list:
            for x in v:
                check(x)
        elif type(v) is int:
            need(abs(v) <= 9007199254740991, 'RC_SAFE_INTEGER_REQUIRED')
        elif type(v) is str:
            need(not any(0xd800 <= ord(c) <= 0xdfff for c in v), 'RC_UNICODE_INVALID')
        else:
            need(v is None or type(v) is bool, 'RC_JCS_TYPE_INVALID')
    check(value)
    return json.dumps(value, sort_keys=True, ensure_ascii=False, separators=(',', ':'), allow_nan=False).encode('utf-8')


def identity_check(value, expected_app, ci_build_number=None, previous_store_number=None):
    fields(value, IDENTITY)
    need(all(type(value[k]) is str for k in IDENTITY if k != 'candidateOrdinal'), 'RC_IDENTITY_TYPE')
    version = value['releaseVersion']; ordinal = value['candidateOrdinal']
    need(re.fullmatch(r'(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)', version), 'RC_VERSION_INVALID')
    need(type(ordinal) is int and 1 <= ordinal <= 2147483647 and value['candidateTag'] == f'v{version}-rc.{ordinal}', 'RC_TAG_MISMATCH')
    need(re.fullmatch('[0-9a-f]{40}', value['gitCommit']) and re.fullmatch(r'[A-Za-z0-9_-]+\.ya?ml@[0-9a-f]{40}', value['workflow']), 'RC_SOURCE_IDENTITY_INVALID')
    need(value['platform'] in ('ANDROID', 'IOS') and value['buildProfile'] == 'production', 'RC_PRODUCTION_REQUIRED')
    app = value['applicationIdentifier']
    need(app == expected_app and re.fullmatch(r'[A-Za-z][A-Za-z0-9]*(?:\.[A-Za-z][A-Za-z0-9]*)+', app) and not re.search(r'\.(qa|dev|human|test|staging)$', app, re.I), 'RC_APPLICATION_MISMATCH')
    number = value['buildNumber']
    need(re.fullmatch('[1-9][0-9]*', number) and len(number) <= 15 and int(number) <= 9007199254740991, 'RC_BUILD_NUMBER_INVALID')
    need(value['platform'] != 'ANDROID' or int(number) <= 2100000000, 'RC_ANDROID_BUILD_NUMBER_LIMIT')
    if ci_build_number is not None:
        need(type(ci_build_number) is str and number == ci_build_number, 'RC_CI_NUMBER_MISMATCH')
    if previous_store_number is not None:
        need(type(previous_store_number) is str and re.fullmatch('[1-9][0-9]*', previous_store_number) and int(number) > int(previous_store_number), 'RC_STORE_NUMBER_NOT_INCREASING')
    return value


def immutable(manifest):
    return {k: v for k, v in manifest.items() if k not in MUTABLE}


@dataclass(frozen=True)
class AuthorizationContext:
    """Injected by a qualified protected caller, never reconstructed from provenance."""
    subject_immutable_jcs_sha256: str
    original_hashes: object

    def __post_init__(self):
        hash_value(self.subject_immutable_jcs_sha256)
        need(type(self.original_hashes) is dict and set(self.original_hashes) <= set(GATES + BLOCKERS), 'RC_AUTHORIZATION_KEYS')
        for value in self.original_hashes.values():
            hash_value(value)
        object.__setattr__(self, 'original_hashes', MappingProxyType(dict(self.original_hashes)))


@dataclass(frozen=True)
class RetainedManifest:
    manifest_raw: bytes
    provenance_raw: bytes
    authorizations: AuthorizationContext | None
    trusted_receipts: object
    predecessor: RetainedManifest | None = None

    def __post_init__(self):
        need(type(self.trusted_receipts) is dict and set(self.trusted_receipts) == {'runtimeBindings', 'signingVerification', 'storeReceipt'}, 'RC_PRIOR_RECEIPT_TRUST_REQUIRED')
        for value in self.trusted_receipts.values(): hash_value(value)
        object.__setattr__(self, 'trusted_receipts', MappingProxyType(dict(self.trusted_receipts)))


class Originals:
    def __init__(self, root):
        self.root = Path(os.path.abspath(root))
        self.seen = {}

    def resolve(self, name):
        need(type(name) is str and name and '\\' not in name and ':' not in name and not name.startswith('/') and all(p not in ('', '.', '..') for p in name.split('/')), 'RC_ORIGINAL_PATH')
        path = no_links(self.root.joinpath(*name.split('/')))
        need(path.suffix.lower() not in ('.p12', '.pfx', '.jks', '.keystore', '.key') and not any(p.lower() in ('secrets', 'credentials', 'keychains') for p in name.split('/')), 'RC_SECRET_INPUT_FORBIDDEN')
        need(path.is_file(), 'RC_ORIGINAL_MISSING')
        return path

    def reference(self, ref):
        path = self.file(ref)
        need(path.stat().st_size <= 16 * 1024 * 1024, 'RC_ORIGINAL_SIZE')
        raw = path.read_bytes(); need(digest(raw) == ref['rawSha256'], 'RC_ORIGINAL_CHANGED')
        return raw

    def file(self, ref):
        fields(ref, ('path', 'rawSha256'))
        path = self.resolve(ref['path']); expected = hash_value(ref['rawSha256'])
        need(0 < path.stat().st_size <= 8 * 1024 ** 3, 'RC_ORIGINAL_SIZE')
        with path.open('rb') as stream: actual = hashlib.file_digest(stream, 'sha256').hexdigest()
        need(actual == expected, 'RC_ORIGINAL_RAW_MISMATCH')
        previous = self.seen.setdefault(ref['path'], expected)
        need(previous == expected, 'RC_ORIGINAL_CHANGED')
        return path

    def unchanged(self):
        for name, expected in list(self.seen.items()):
            self.file({'path': name, 'rawSha256': expected})


def schema_read(raw, schema):
    value = parse(raw)
    try:
        jsonschema.Draft202012Validator(schema).validate(value)
    except jsonschema.ValidationError as exc:
        raise ManifestError('RC_SCHEMA_REJECTED') from exc
    return value


def read_v1(raw, v1_schema):
    """Historical grammar only. No approval and no automatic v2 conversion."""
    return schema_read(raw, v1_schema)


def _refs(values, originals):
    need(type(values) is list and 0 < len(values) <= 8192, 'RC_EVIDENCE_SET_EMPTY')
    names = set()
    for value in values:
        originals.file(value)
        need(value['path'] not in names, 'RC_REFERENCE_DUPLICATE')
        names.add(value['path'])


def _inputs(manifest, provenance, originals, trusted_receipts):
    inputs = provenance['inputs']; fields(inputs, INPUTS)
    raw = {name: originals.reference(inputs[name]) for name in INPUTS}
    need(type(trusted_receipts) is dict and set(trusted_receipts) == {'runtimeBindings', 'signingVerification', 'storeReceipt'}, 'RC_TRUSTED_RECEIPTS_REQUIRED')
    for name, expected in trusted_receipts.items():
        need(hash_value(expected) == digest(raw[name]), 'RC_TRUSTED_RECEIPT_MISMATCH')
    for field, key in (('toolchainLockSha256', 'toolchainLock'), ('packageLockSha256', 'packageLock'), ('productionConfigSha256', 'productionConfig')):
        need(manifest[field] == digest(raw[key]), 'RC_INPUT_LOCK_MISMATCH')
    release = schema_read(raw['releaseLock'], provenance['_releaseSchema'])
    need(release['toolchainLockSha256'] == manifest['toolchainLockSha256'] and digest(canonical(release)) == manifest['contentLockSha256'], 'RC_RELEASE_LOCK_MISMATCH')
    need(release['releaseId'] in (manifest['candidateTag'], 'v' + manifest['releaseVersion']) and not release['releaseId'].startswith('fixture-'), 'RC_RELEASE_VERSION_MISMATCH')
    approval = parse(raw['approvalLock'])
    fields(approval, ('approvalLockVersion', 'kind', 'use', 'releaseLock', 'evidence', 'sources', 'acceptedBy', 'decisionRef', 'acceptance'))
    need(version_one(approval['approvalLockVersion']) and approval['kind'] == 'PRODUCTION_SOURCE_APPROVAL_LOCK' and approval['use'] == 'Production', 'RC_APPROVAL_SOURCE_KIND')
    # AcceptedBy/decisionRef are preserved context, never authority checks.
    originals.reference(approval['acceptance']); _refs(approval['evidence'], originals); _refs(approval['sources'], originals)
    need(approval['releaseLock'] == inputs['releaseLock'], 'RC_APPROVAL_RELEASE_MISMATCH')
    runtime = parse(raw['runtimeBindings']); fields(runtime, RUNTIME_FIELDS)
    need(version_one(runtime['runtimeImportVersion']) and runtime['use'] == 'Production' and runtime['status'] == 'PRODUCT_APPROVED', 'RC_RUNTIME_USE')
    need(runtime['releaseLockRawSha256'] == digest(raw['releaseLock']) and runtime['releaseLockJcsSha256'] == manifest['contentLockSha256'] and runtime['approvalLockRawSha256'] == digest(raw['approvalLock']), 'RC_RUNTIME_LOCK_BINDING')
    profiles = runtime['qualityProfiles']
    need(type(profiles) is list and len(profiles) == 4 and [p.get('profileId') for p in profiles] == list(PROFILES), 'RC_PROFILE_NAMED_SET')
    catalogs = {}
    for name in ('publicCatalog', 'campaignCatalog', 'campaignCompletionCatalog', 'endlessCompletionCatalog', 'calibrationCatalog', 'cosmeticsCatalog'):
        ref = runtime[name]; fields(ref, ('catalogId', 'revision', 'hashSha256'))
        need(type(ref['revision']) is int and ref['revision'] > 0 and ref['catalogId'] not in catalogs, 'RC_RUNTIME_CATALOG_REFERENCE')
        catalogs[text(ref['catalogId'])] = hash_value(ref['hashSha256'])
    for p in profiles:
        fields(p, ('profileId', 'hashSha256')); need(p['profileId'] not in catalogs, 'RC_CATALOG_COLLISION'); catalogs[p['profileId']] = hash_value(p['hashSha256'])
    actual_catalogs = {x['id']: x['sha256'] for x in release['catalogHashes']}
    need(len(actual_catalogs) == len(release['catalogHashes']) and actual_catalogs == catalogs and len(release['puzzles']) == 240, 'RC_RELEASE_COMPLETE_CATALOGS')
    expected_ids = {f'S1-{section:02}-{route:02}-{puzzle:02}' for section in range(1, 6) for route in range(1, 5) for puzzle in range(1, 13)}
    need({p['puzzleId'] for p in release['puzzles']} == expected_ids, 'RC_RELEASE_PUZZLE_MEMBERSHIP')
    sign = parse(raw['signingVerification'])
    fields(sign, ('verificationVersion', 'kind', 'identity', 'artifactSha256', 'signingFingerprintSha256', 'checks', 'evidence'))
    identity_check(sign['identity'], manifest['applicationIdentifier'])
    need(version_one(sign['verificationVersion']) and sign['kind'] == ('ACTUAL_SIGNED_ANDROID_AAB' if manifest['platform'] == 'ANDROID' else 'ACTUAL_SIGNED_IOS_IPA') and sign['identity'] == {k: manifest[k] for k in IDENTITY}, 'RC_SIGNING_IDENTITY')
    need(sign['artifactSha256'] == manifest['artifactSha256'] and sign['signingFingerprintSha256'] == manifest['signingFingerprintSha256'] and sign['checks'] == list(CHECKS), 'RC_SIGNING_FACTS')
    _refs(sign['evidence'], originals)
    store = parse(raw['storeReceipt'])
    fields(store, ('storeReceiptVersion', 'identity', 'storeBuildReference', 'uploadArtifactSha256', 'uploadSigningFingerprintSha256', 'deliveredArtifacts', 'mappingOriginal', 'previousStoreBuildNumber', 'previousStoreState', 'previousStoreVersionOriginal'))
    identity_check(store['identity'], manifest['applicationIdentifier'])
    need(version_one(store['storeReceiptVersion']) and store['identity'] == sign['identity'] and store['storeBuildReference'] == manifest['storeBuildReference'] and store['uploadArtifactSha256'] == manifest['artifactSha256'] and store['uploadSigningFingerprintSha256'] == manifest['signingFingerprintSha256'], 'RC_STORE_UPLOAD_BINDING')
    originals.reference(store['mappingOriginal'])
    originals.reference(store['previousStoreVersionOriginal'])
    previous = store['previousStoreBuildNumber']
    need(previous is None or type(previous) is str, 'RC_STORE_PREVIOUS_NUMBER_TYPE')
    need(store['previousStoreState'] == ('INITIAL_RELEASE_CONFIRMED' if previous is None else 'KNOWN_VERSION'), 'RC_STORE_PREVIOUS_UNCONFIRMED')
    # null means an explicitly checked initial channel; the protected store receipt
    # cannot be replaced with an absent account treated as proven monotonicity.
    identity_check(store['identity'], manifest['applicationIdentifier'], previous_store_number=previous)
    delivered = store['deliveredArtifacts']; need(type(delivered) is list and 0 < len(delivered) <= 256, 'RC_DELIVERY_REQUIRED')
    names = set()
    for item in delivered:
        fields(item, ('path', 'rawSha256', 'signingFingerprintSha256')); hash_value(item['signingFingerprintSha256'])
        originals.file({'path': item['path'], 'rawSha256': item['rawSha256']}); need(item['path'] not in names, 'RC_DELIVERY_DUPLICATE'); names.add(item['path'])
    return runtime, store


def _expected_scope(key, manifest, runtime, store, inputs):
    delivery = copy.deepcopy(store['deliveredArtifacts'])
    identity = {k: manifest[k] for k in IDENTITY}
    actual = {'identity': identity, 'storeBuildReference': manifest['storeBuildReference'], 'uploadArtifactSha256': manifest['artifactSha256'], 'delivery': delivery}
    source = {'contentLockSha256': manifest['contentLockSha256'], 'approvalLockRawSha256': inputs['approvalLock']['rawSha256'], 'runtimeBindingsRawSha256': inputs['runtimeBindings']['rawSha256'], 'uiSourceLockRawSha256': runtime['uiSourceLockRawSha256'], 'calibrationCatalogSha256': runtime['calibrationCatalog']['hashSha256'], 'qualityProfiles': runtime['qualityProfiles']}
    if key == 'SOURCE_CALIBRATION': return source
    if key == 'SIGNING_PACKAGE': return {'identity': identity, 'artifactSha256': manifest['artifactSha256'], 'signingFingerprintSha256': manifest['signingFingerprintSha256'], 'signingVerificationRawSha256': inputs['signingVerification']['rawSha256']}
    if key == 'RIGHTS_LEGAL': return {k: manifest[k] for k in ('packageLockSha256', 'contentLockSha256', 'productionConfigSha256')}
    if key == 'STORE_CHANNEL': return {**actual, 'uploadSigningFingerprintSha256': manifest['signingFingerprintSha256']}
    if key == 'PROVIDER_PRIVACY': return {**actual, 'productionConfigSha256': manifest['productionConfigSha256']}
    if key == 'OWNER_GAME_ACCEPTANCE': return {**actual, 'contentLockSha256': manifest['contentLockSha256'], 'productionConfigSha256': manifest['productionConfigSha256'], 'uiSourceLockRawSha256': runtime['uiSourceLockRawSha256']}
    if key == 'INDEPENDENT_RC': return {**actual, **{k: manifest[k] for k in ('signingFingerprintSha256', 'contentLockSha256', 'productionConfigSha256')}}
    if key == 'DEVICE_REGRESSION': return actual
    if key == 'BLOCKER-PROD-003': return {**actual, **source, 'implementationArea': 'CALIBRATED_ENDLESS'}
    return {**actual, 'productionConfigSha256': manifest['productionConfigSha256'], 'implementationArea': 'HINT_CREDITS' if key == 'BLOCKER-PROD-001' else 'DAILY_CLAIMS'}


def read_v2(manifest_raw, provenance_raw, *, schema, release_schema, originals, artifact_path, expected_app,
            trusted_receipts, authorizations=None, predecessor=None, _depth=0):
    need(_depth < 256, 'RC_HISTORY_LIMIT')
    manifest = schema_read(manifest_raw, schema)
    identity_check({k: manifest[k] for k in IDENTITY}, expected_app)
    need(manifest['manifestVersion'] == 2 and not any(manifest['debugFlags'].values()), 'RC_RELEASE_DEBUG_FORBIDDEN')
    provenance = parse(provenance_raw)
    fields(provenance, ('provenanceVersion', 'manifestRawSha256', 'manifestJcsSha256', 'predecessor', 'inputs', 'evidence', 'blockers'))
    need(version_one(provenance['provenanceVersion']) and provenance['manifestRawSha256'] == digest(manifest_raw) and provenance['manifestJcsSha256'] == digest(canonical(manifest)), 'RC_MANIFEST_PROVENANCE_MISMATCH')
    actual_artifact = artifact_hash(artifact_path)
    need(actual_artifact == manifest['artifactSha256'], 'RC_ARTIFACT_MISMATCH')
    # Validator schema is trusted code, not an assessed provenance field.
    provenance['_releaseSchema'] = release_schema
    runtime, store = _inputs(manifest, provenance, originals, trusted_receipts)
    del provenance['_releaseSchema']
    fields(provenance['evidence'], GATES); fields(provenance['blockers'], BLOCKERS)
    subject = digest(canonical(immutable(manifest)))
    for key in GATES + BLOCKERS:
        gate = key in GATES
        state = manifest['evidenceGates'][key]['state'] if gate else manifest['blockerState'][key]
        closed = state == ('VERIFIED' if gate else 'CLOSED_WITH_EVIDENCE')
        proof = provenance['evidence' if gate else 'blockers'][key]
        if not closed:
            need(proof is None, 'RC_OPEN_GATE_HAS_PROOF'); continue
        fields(proof, ('original', 'bindings', 'resources'))
        original = originals.reference(proof['original']); actual_hash = digest(original)
        need(authorizations is not None and authorizations.subject_immutable_jcs_sha256 == subject and authorizations.original_hashes.get(key) == actual_hash, 'RC_QUALIFIED_AUTHORIZATION_REQUIRED')
        if gate: need(manifest['evidenceGates'][key]['originalRawSha256'] == actual_hash, 'RC_GATE_ORIGINAL_MISMATCH')
        expected = _expected_scope(key, manifest, runtime, store, provenance['inputs'])
        need(canonical(proof['bindings']) == canonical(expected), 'RC_EVIDENCE_SCOPE_MISMATCH')
        _refs(proof['resources'], originals)
        if key == 'SIGNING_PACKAGE': need(actual_hash == provenance['inputs']['signingVerification']['rawSha256'], 'RC_SIGNING_GATE_NOT_VERIFIER')
    promotable = all(v['state'] == 'VERIFIED' for v in manifest['evidenceGates'].values()) and all(v == 'CLOSED_WITH_EVIDENCE' for v in manifest['blockerState'].values())
    need(manifest['promotionAllowed'] is promotable, 'RC_PROMOTION_NOT_COMPUTED')
    if predecessor is None:
        need(provenance['predecessor'] is None, 'RC_PREDECESSOR_REQUIRED')
    else:
        need(isinstance(predecessor, RetainedManifest), 'RC_RETAINED_RECORD_REQUIRED')
        prior_raw = predecessor.manifest_raw
        # Revalidate the actual retained predecessor, including original authority.
        prior_manifest = read_v2(prior_raw, predecessor.provenance_raw, schema=schema, release_schema=release_schema,
            originals=originals, artifact_path=artifact_path, expected_app=expected_app,
            trusted_receipts=dict(predecessor.trusted_receipts), authorizations=predecessor.authorizations,
            predecessor=predecessor.predecessor, _depth=_depth+1)
        fields(provenance['predecessor'], ('rawSha256', 'jcsSha256'))
        need(provenance['predecessor'] == {'rawSha256': digest(prior_raw), 'jcsSha256': digest(canonical(prior_manifest))}, 'RC_PREDECESSOR_HASH_MISMATCH')
        need(immutable(manifest) == immutable(prior_manifest), 'RC_SUCCESSOR_IDENTITY_CHANGED')
    originals.unchanged()
    return copy.deepcopy(manifest)


def build_manifest(request, *, schema, release_schema, originals, artifact_path, expected_app,
                   ci_build_number, trusted_receipts, authorizations=None, predecessor=None):
    fields(request, ('identity', 'inputs', 'evidence', 'blockers'))
    identity = identity_check(copy.deepcopy(request['identity']), expected_app, ci_build_number)
    fields(request['inputs'], INPUTS); fields(request['evidence'], GATES); fields(request['blockers'], BLOCKERS)
    raw = {key: originals.reference(value) for key, value in request['inputs'].items()}
    sign = parse(raw['signingVerification']); store = parse(raw['storeReceipt'])
    need(type(sign) is dict and type(store) is dict and 'signingFingerprintSha256' in sign and 'storeBuildReference' in store, 'RC_SIGNED_STORE_RECEIPTS_REQUIRED')
    actual_artifact = artifact_hash(artifact_path)
    gates = {key: {'state': 'OPEN_FAIL_CLOSED', 'originalRawSha256': None} if request['evidence'][key] is None else {'state': 'VERIFIED', 'originalRawSha256': request['evidence'][key]['original']['rawSha256']} for key in GATES}
    blockers = {key: 'OPEN_FAIL_CLOSED' if request['blockers'][key] is None else 'CLOSED_WITH_EVIDENCE' for key in BLOCKERS}
    manifest = {'manifestVersion': 2, **identity, 'signingFingerprintSha256': sign['signingFingerprintSha256'],
        'toolchainLockSha256': digest(raw['toolchainLock']), 'packageLockSha256': digest(raw['packageLock']),
        'contentLockSha256': digest(canonical(parse(raw['releaseLock']))), 'productionConfigSha256': digest(raw['productionConfig']),
        'artifactSha256': actual_artifact, 'storeBuildReference': text(store['storeBuildReference']),
        'promotionAllowed': all(v['state'] == 'VERIFIED' for v in gates.values()) and all(v == 'CLOSED_WITH_EVIDENCE' for v in blockers.values()),
        'debugFlags': {k: False for k in DEBUG}, 'blockerState': blockers, 'evidenceGates': gates}
    raw_manifest = canonical(manifest)
    need(predecessor is None or isinstance(predecessor, RetainedManifest), 'RC_RETAINED_RECORD_REQUIRED')
    previous = None if predecessor is None else {'rawSha256': digest(predecessor.manifest_raw), 'jcsSha256': digest(canonical(parse(predecessor.manifest_raw)))}
    provenance = {'provenanceVersion': 1, 'manifestRawSha256': digest(raw_manifest), 'manifestJcsSha256': digest(canonical(manifest)),
        'predecessor': previous, 'inputs': copy.deepcopy(request['inputs']), 'evidence': copy.deepcopy(request['evidence']), 'blockers': copy.deepcopy(request['blockers'])}
    raw_provenance = canonical(provenance)
    read_v2(raw_manifest, raw_provenance, schema=schema, release_schema=release_schema, originals=originals,
            artifact_path=artifact_path, expected_app=expected_app, trusted_receipts=trusted_receipts,
            authorizations=authorizations, predecessor=predecessor)
    return raw_manifest, raw_provenance


def publish(directory, manifest_raw, provenance_raw, originals):
    """New folder only, no predecessor overwrite, no artifact rebuild or signing."""
    directory = no_links(directory)
    need(not directory.exists() and directory.parent.is_dir(), 'RC_OUTPUT_NOT_NEW')
    originals.unchanged()
    directory.mkdir(); marker = directory / 'INCOMPLETE'
    marker.write_bytes(b'Not a completed manifest delivery. Retain for diagnosis.\n')
    def write(path, data):
        with path.open('xb') as stream: stream.write(data); stream.flush(); os.fsync(stream.fileno())
    for name, expected in list(originals.seen.items()):
        path = originals.resolve(name)
        if path.stat().st_size <= 16 * 1024 * 1024:
            raw = path.read_bytes()
            need(not re.search(rb'(?m)^-----BEGIN (?:RSA |EC )?PRIVATE KEY-----\r?$', raw), 'RC_SECRET_INPUT_FORBIDDEN')
        target = directory / 'originals' / name; target.parent.mkdir(parents=True, exist_ok=True)
        with path.open('rb') as source, target.open('xb') as dest:
            shutil.copyfileobj(source, dest); dest.flush(); os.fsync(dest.fileno())
        with target.open('rb') as stream: need(hashlib.file_digest(stream, 'sha256').hexdigest() == expected, 'RC_COPIED_ORIGINAL_CHANGED')
    originals.unchanged()
    write(directory / 'release-manifest.json', manifest_raw)
    write(directory / 'release-provenance.internal.json', provenance_raw)
    marker.unlink()


BUILD_INFO = ('buildInfoVersion', 'releaseVersion', 'buildNumber', 'gitCommit', 'candidateTag', 'builtAtUtc', 'unityVersion', 'packageLockSha256', 'contentLockSha256', 'saveSchemaVersion', 'levelSchemaVersion', 'solverVersion', 'workflow', 'workflowRunId')


def build_info(identity, *, expected_app, ci_build_number, built_at_utc, unity_version,
               package_lock_sha256, content_lock_sha256, save_schema_version, level_schema_version,
               solver_version, workflow_run_id):
    import datetime
    identity_check(identity, expected_app, ci_build_number)
    need(type(built_at_utc) is str and re.fullmatch(r'\d{4}-\d\d-\d\dT\d\d:\d\d:\d\dZ', built_at_utc), 'RC_BUILD_TIME_INVALID')
    try: datetime.datetime.strptime(built_at_utc, '%Y-%m-%dT%H:%M:%SZ')
    except ValueError as exc: raise ManifestError('RC_BUILD_TIME_INVALID') from exc
    need(re.fullmatch(r'[0-9]+\.[0-9]+\.[0-9]+[a-z][0-9]+', unity_version or '') and type(save_schema_version) is int and save_schema_version > 0 and type(level_schema_version) is int and level_schema_version > 0, 'RC_BUILD_VERSION_FACTS_INVALID')
    need(re.fullmatch(r'solver-v[1-9][0-9]*', solver_version or '') and type(workflow_run_id) is str and re.fullmatch('[1-9][0-9]*', workflow_run_id), 'RC_BUILD_WORKFLOW_FACTS_INVALID')
    return {'buildInfoVersion': 1, **{k: identity[k] for k in ('releaseVersion', 'buildNumber', 'gitCommit', 'candidateTag', 'workflow')},
        'builtAtUtc': built_at_utc, 'unityVersion': unity_version, 'packageLockSha256': hash_value(package_lock_sha256),
        'contentLockSha256': hash_value(content_lock_sha256), 'saveSchemaVersion': save_schema_version,
        'levelSchemaVersion': level_schema_version, 'solverVersion': solver_version, 'workflowRunId': workflow_run_id}


def read_build_info(raw, **actual):
    value = parse(raw); fields(value, BUILD_INFO)
    expected = build_info(**actual)
    need(canonical(value) == canonical(expected), 'RC_BUILD_INFO_ACTUAL_MISMATCH')
    return value
