"""Controlled contract fixtures. No actual source/signing/store/Human acceptance."""
from __future__ import annotations

import copy
import json
import os
from pathlib import Path
import subprocess
import sys
import tempfile
import unittest
from unittest.mock import patch
from types import SimpleNamespace

import jsonschema
import rc_manifest as m
import rc_manifest_cli as cli

REPO = Path(os.environ['STP_REPO_ROOT'])
STAGE = Path(__file__).resolve().parents[2]
SCHEMA = m.parse((STAGE/'ARCHITECTURE/schemas/release-manifest-v2.schema.json').read_bytes())
RELEASE = m.parse((REPO/'ARCHITECTURE/schemas/release-lock-v1.schema.json').read_bytes())
LEGACY = m.parse((REPO/'ARCHITECTURE/schemas/release-manifest-v1.schema.json').read_bytes())


class Fixture:
    def __init__(self, root, platform='ANDROID'):
        self.root = root; self.identity = dict(releaseVersion='1.2.3', candidateTag='v1.2.3-rc.4', candidateOrdinal=4,
            gitCommit='1'*40, workflow='release.yml@'+'2'*40, platform=platform, buildNumber='9001', buildProfile='production', applicationIdentifier='invalid.controlled.rc')
        self.artifact = root/'CONTROLLED-NOT-SIGNED-ARTIFACT.bin'; self.artifact.write_bytes(b'CONTROLLED CONTRACT BYTES; NO SIGNATURE OR STORE CLAIM')
        self.artifact_hash = m.digest(self.artifact.read_bytes()); self.certificate = m.digest(b'CONTROLLED CERTIFICATE ORACLE')
        self.inputs = {}; self.trust = {}
        self.put_input('toolchainLock', {'CONTROLLED':'toolchain'}); self.put_input('packageLock', {'CONTROLLED':'packages'})
        self.put_input('productionConfig', {'CONTROLLED':'inert profile'}); self.put_input('blockersSource', b'CONTROLLED Open blocker original; no qualification')
        refs = {}; catalogs = []
        for key in ('publicCatalog','campaignCatalog','campaignCompletionCatalog','endlessCompletionCatalog','calibrationCatalog','cosmeticsCatalog'):
            value = {'catalogId': 'controlled-'+key, 'revision':1, 'hashSha256':m.digest(key.encode())};refs[key]=value;catalogs.append({'id':value['catalogId'],'sha256':value['hashSha256']})
        profiles=[{'profileId':p,'hashSha256':m.digest(p.encode())} for p in m.PROFILES];catalogs += [{'id':p['profileId'],'sha256':p['hashSha256']} for p in profiles]
        h=lambda profile:{'profile':profile,'sha256':'3'*64}
        puzzles=[dict(puzzleId=f'S1-{s:02}-{t:02}-{p:02}',documentSchemaVersion=2,documentSha256='4'*64,publicPuzzleHash=h('STP-PUZZLE-SEMANTIC-JCS-1'),solutionHash=h('STP-SOLUTION-JCS-1'),proofFormatVersion=1,solverVersion='solver-v2',proofHash=h('STP-PROOF-JCS-1'),legacyBindings=[]) for s in range(1,6) for t in range(1,5) for p in range(1,13)]
        self.put_input('releaseLock', dict(lockFormatVersion=1,releaseId='v1.2.3-rc.4',gitCommit='5'*40,toolchainLockSha256=self.inputs['toolchainLock']['rawSha256'],catalogHashes=catalogs,puzzles=puzzles))
        acceptance=self.put('controlled-acceptance.txt',b'CONTROLLED ONLY. No actual acceptance.'); evidence=self.put('controlled-evidence.txt',b'CONTROLLED source evidence')
        self.put_input('approvalLock',dict(approvalLockVersion=1,kind='PRODUCTION_SOURCE_APPROVAL_LOCK',use='Production',releaseLock=self.inputs['releaseLock'],evidence=[evidence],sources=[self.inputs['releaseLock'],self.inputs['toolchainLock']],acceptedBy='CONTROLLED-NOT-A-PERSON',decisionRef='CONTROLLED-ONLY',acceptance=acceptance))
        runtime=dict(runtimeImportVersion=1,use='Production',status='PRODUCT_APPROVED',releaseLockRawSha256=self.inputs['releaseLock']['rawSha256'],releaseLockJcsSha256=self.inputs['releaseLock']['rawSha256'],approvalLockRawSha256=self.inputs['approvalLock']['rawSha256'],**refs,qualityProfiles=profiles,catalogSet={'catalogId':'controlled-set','revision':1,'hashSha256':'6'*64},railwayAssetsRawSha256='7'*64,campaignTextRawSha256='8'*64,uiSourceLockRawSha256='9'*64,playerFiles=[])
        self.put_input('runtimeBindings',runtime)
        symbol=self.put('controlled-symbols.bin',b'CONTROLLED symbol oracle')
        sign=dict(verificationVersion=1,kind='ACTUAL_SIGNED_ANDROID_AAB' if platform=='ANDROID' else 'ACTUAL_SIGNED_IOS_IPA',identity=self.identity,artifactSha256=self.artifact_hash,signingFingerprintSha256=self.certificate,checks=list(m.CHECKS),evidence=[symbol])
        self.put_input('signingVerification',sign)
        delivery=self.put('controlled-delivered.bin',b'CONTROLLED STORE DERIVATION; DIFFERENT BYTES AND SIGNER')
        mapping=self.put('controlled-store-mapping.txt',b'CONTROLLED store oracle, no real service'); previous=self.put('controlled-store-previous.txt',b'CONTROLLED known initial release oracle')
        self.put_input('storeReceipt',dict(storeReceiptVersion=1,identity=self.identity,storeBuildReference='CONTROLLED-NOT-A-STORE-REFERENCE',uploadArtifactSha256=self.artifact_hash,uploadSigningFingerprintSha256=self.certificate,deliveredArtifacts=[{**delivery,'signingFingerprintSha256':m.digest(b'CONTROLLED DIFFERENT DELIVERY SIGNER')}],mappingOriginal=mapping,previousStoreBuildNumber=None,previousStoreState='INITIAL_RELEASE_CONFIRMED',previousStoreVersionOriginal=previous))
        self.trust={k:self.inputs[k]['rawSha256'] for k in ('runtimeBindings','signingVerification','storeReceipt')}
        self.request={'identity':self.identity,'inputs':self.inputs,'evidence':{k:None for k in m.GATES},'blockers':{k:None for k in m.BLOCKERS}}

    def put(self,name,value):
        raw=value if type(value) is bytes else m.canonical(value);p=self.root/name;p.parent.mkdir(parents=True,exist_ok=True);p.write_bytes(raw)
        return {'path':name,'rawSha256':m.digest(raw)}

    def put_input(self,key,value): self.inputs[key]=self.put(key+'.json',value)
    def get(self,key):return m.parse((self.root/self.inputs[key]['path']).read_bytes())
    def change(self,key,mutate):
        value=self.get(key);mutate(value);self.put_input(key,value)
        if key in self.trust:self.trust[key]=self.inputs[key]['rawSha256']

    def build(self,auth=None,previous=None):
        return m.build_manifest(self.request,schema=SCHEMA,release_schema=RELEASE,originals=m.Originals(self.root),artifact_path=self.artifact,expected_app=self.identity['applicationIdentifier'],ci_build_number=self.identity['buildNumber'],trusted_receipts=self.trust,authorizations=auth,predecessor=previous)

    def closed(self):
        raw,prov=self.build();manifest=m.parse(raw);runtime=self.get('runtimeBindings');store=self.get('storeReceipt');hashes={}
        for key in m.GATES+m.BLOCKERS:
            original=self.inputs['signingVerification'] if key=='SIGNING_PACKAGE' else self.put('gate-'+key+'.txt',b'CONTROLLED shared Markdown; roles do not grant authority')
            proof={'original':original,'bindings':m._expected_scope(key,manifest,runtime,store,self.inputs),'resources':[self.inputs['blockersSource']]}
            self.request['evidence' if key in m.GATES else 'blockers'][key]=proof;hashes[key]=original['rawSha256']
        return m.AuthorizationContext(m.digest(m.canonical(m.immutable(manifest))),hashes),m.RetainedManifest(raw,prov,None,self.trust)


class ManifestTests(unittest.TestCase):
    def setUp(self):
        parent=Path(os.environ.get('STP_RC_TEST_SCRATCH',str(REPO/'.utmp/wp046-b-rc-manifest/checks')));parent.mkdir(parents=True,exist_ok=True)
        self.temp=tempfile.TemporaryDirectory(prefix='CONTROLLED-',dir=parent);self.addCleanup(self.temp.cleanup);self.f=Fixture(Path(self.temp.name))

    def read(self,raw,prov,auth=None,previous=None):
        return m.read_v2(raw,prov,schema=SCHEMA,release_schema=RELEASE,originals=m.Originals(self.f.root),artifact_path=self.f.artifact,expected_app=self.f.identity['applicationIdentifier'],trusted_receipts=self.f.trust,authorizations=auth,predecessor=previous)

    def test_schema_and_v1_bytes_remain_separate(self):
        jsonschema.Draft202012Validator.check_schema(SCHEMA)
        for name in ('release-manifest-v1.rc.example.json','release-manifest-v1.staging.example.json'):
            raw=(REPO/'ARCHITECTURE/examples'/name).read_bytes();self.assertEqual(m.read_v1(raw,LEGACY),json.loads(raw));self.assertEqual(raw,(REPO/'ARCHITECTURE/examples'/name).read_bytes())
            with self.assertRaises(m.ManifestError):m.schema_read(raw,SCHEMA)

    def test_open_candidate_is_false_and_accepts_distinct_upload_delivery_signers(self):
        raw,prov=self.f.build();value=self.read(raw,prov);self.assertIs(value['promotionAllowed'],False)
        self.assertNotEqual(self.f.get('storeReceipt')['deliveredArtifacts'][0]['signingFingerprintSha256'],value['signingFingerprintSha256'])

    def test_all_closed_requires_injected_qualified_context_and_full_actual_bindings(self):
        auth,previous=self.f.closed();raw,prov=self.f.build(auth,previous);value=self.read(raw,prov,auth,previous);self.assertIs(value['promotionAllowed'],True)

    def test_roles_markdown_and_own_provenance_cannot_authorize(self):
        auth,previous=self.f.closed()
        with self.assertRaisesRegex(m.ManifestError,'RC_QUALIFIED_AUTHORIZATION_REQUIRED'):self.f.build(None,previous)
        with self.assertRaisesRegex(m.ManifestError,'RC_QUALIFIED_AUTHORIZATION_REQUIRED'):self.f.build(m.AuthorizationContext(auth.subject_immutable_jcs_sha256,{}),previous)

    def test_same_markdown_hash_for_foreign_candidate_has_no_authority(self):
        auth,previous=self.f.closed();foreign=m.AuthorizationContext('a'*64,dict(auth.original_hashes))
        with self.assertRaisesRegex(m.ManifestError,'RC_QUALIFIED_AUTHORIZATION_REQUIRED'):self.f.build(foreign,previous)

    def test_every_missing_gate_and_blocker_prevents_true(self):
        auth,previous=self.f.closed()
        for key in m.GATES+m.BLOCKERS:
            container=self.f.request['evidence' if key in m.GATES else 'blockers'];saved=container[key];container[key]=None
            raw,prov=self.f.build(auth,previous);self.assertIs(m.parse(raw)['promotionAllowed'],False,key);container[key]=saved

    def test_every_gate_scope_is_checked_independently(self):
        auth,previous=self.f.closed()
        for key in m.GATES+m.BLOCKERS:
            proof=self.f.request['evidence' if key in m.GATES else 'blockers'][key];saved=proof['bindings'];proof['bindings']={**saved,'UNKNOWN':'forged'}
            with self.assertRaisesRegex(m.ManifestError,'RC_EVIDENCE_SCOPE_MISMATCH',msg=key):self.f.build(auth,previous)
            proof['bindings']=saved

    def test_closed_proof_without_original_or_resource_is_rejected(self):
        auth,previous=self.f.closed();proof=self.f.request['evidence']['OWNER_GAME_ACCEPTANCE'];proof['resources']=[]
        with self.assertRaisesRegex(m.ManifestError,'RC_EVIDENCE_SET_EMPTY'):self.f.build(auth,previous)
        proof['resources']=[self.f.inputs['blockersSource']];(self.f.root/proof['original']['path']).unlink()
        with self.assertRaisesRegex(m.ManifestError,'RC_ORIGINAL_MISSING'):self.f.build(auth,previous)

    def test_successor_changes_only_evidence_blockers_promotion_and_binds_raw_and_jcs(self):
        auth,previous=self.f.closed();raw,prov=self.f.build(auth,previous);self.assertEqual(m.immutable(m.parse(raw)),m.immutable(m.parse(previous.manifest_raw)))
        self.assertEqual(m.parse(prov)['predecessor'],{'rawSha256':m.digest(previous.manifest_raw),'jcsSha256':m.digest(m.canonical(m.parse(previous.manifest_raw)))})
        # A third record revalidates the exact complete retained ancestry.
        current=m.RetainedManifest(raw,prov,auth,self.f.trust,previous);self.f.build(auth,current)

    def test_successor_immutable_field_changes_fail_even_with_reforged_provenance(self):
        raw,prov=self.f.build();prior=m.RetainedManifest(raw,prov,None,self.f.trust)
        value=m.parse(raw);value['workflow']='release.yml@'+'f'*40;changed=m.canonical(value);p=m.parse(prov);p['manifestRawSha256']=m.digest(changed);p['manifestJcsSha256']=m.digest(changed);p['predecessor']={'rawSha256':m.digest(raw),'jcsSha256':m.digest(m.canonical(m.parse(raw)))}
        with self.assertRaises(m.ManifestError):self.read(changed,m.canonical(p),previous=prior)

    def test_predecessor_raw_and_jcs_are_independent(self):
        raw,prov=self.f.build();pretty=json.dumps(m.parse(raw),indent=2).encode();p=m.parse(prov);p['manifestRawSha256']=m.digest(pretty);prettyprov=m.canonical(p)
        auth,_=self.f.closed();prior=m.RetainedManifest(pretty,prettyprov,None,self.f.trust);newraw,newprov=self.f.build(auth,prior);changed=m.parse(newprov);self.assertNotEqual(changed['predecessor']['rawSha256'],changed['predecessor']['jcsSha256']);changed['predecessor']['rawSha256']=changed['predecessor']['jcsSha256']
        with self.assertRaisesRegex(m.ManifestError,'RC_PREDECESSOR_HASH_MISMATCH'):self.read(newraw,m.canonical(changed),auth,prior)

    def test_true_with_open_gate_and_false_with_everything_closed_rejected(self):
        raw,prov=self.f.build();v=m.parse(raw);v['promotionAllowed']=True
        with self.assertRaises(m.ManifestError):m.schema_read(m.canonical(v),SCHEMA)
        auth,prior=self.f.closed();raw,prov=self.f.build(auth,prior);v=m.parse(raw);v['promotionAllowed']=False;p=m.parse(prov);p['manifestRawSha256']=p['manifestJcsSha256']=m.digest(m.canonical(v))
        with self.assertRaisesRegex(m.ManifestError,'RC_PROMOTION_NOT_COMPUTED'):self.read(m.canonical(v),m.canonical(p),auth,prior)

    def test_missing_extra_duplicate_and_wrong_typed_schema_members(self):
        raw,prov=self.f.build();v=m.parse(raw)
        for key in v:
            bad=copy.deepcopy(v);del bad[key]
            with self.assertRaises(m.ManifestError):m.schema_read(m.canonical(bad),SCHEMA)
        for bad in ({**v,'extra':0},{**v,'manifestVersion':True},{**v,'candidateOrdinal':True},{**v,'debugFlags':{**v['debugFlags'],'profiler':True}}):
            with self.assertRaises(m.ManifestError):self.read(m.canonical(bad),prov)
        with self.assertRaises(m.ManifestError):m.parse(b'{"x":1,"x":2}')
        for bad in (b'{}\n{}',b'\xef\xbb\xbf{}',b'{"n":NaN}',b'{"n":1.0}',b'{"n":9007199254740992}'):
            with self.assertRaises(m.ManifestError):m.parse(bad)

    def test_gate_syntax_and_provenance_tampering(self):
        raw,prov=self.f.build();value=m.parse(raw)
        for key in m.GATES:
            v=copy.deepcopy(value);v['evidenceGates'][key]['originalRawSha256']='a'*64
            with self.assertRaises(m.ManifestError):m.schema_read(m.canonical(v),SCHEMA)
        p=m.parse(prov);p['manifestRawSha256']='0'*64
        with self.assertRaisesRegex(m.ManifestError,'RC_MANIFEST_PROVENANCE_MISMATCH'):self.read(raw,m.canonical(p))

    def test_artifact_byte_change_and_wrong_receipt_trust_fail(self):
        raw,prov=self.f.build();self.f.artifact.write_bytes(b'changed')
        with self.assertRaisesRegex(m.ManifestError,'RC_ARTIFACT_MISMATCH'):self.read(raw,prov)
        self.f.artifact.write_bytes(b'CONTROLLED CONTRACT BYTES; NO SIGNATURE OR STORE CLAIM');self.f.trust['signingVerification']='f'*64
        with self.assertRaisesRegex(m.ManifestError,'RC_TRUSTED_RECEIPT_MISMATCH'):self.read(raw,prov)

    def test_original_changed_after_snapshot_and_unknown_paths_rejected(self):
        originals=m.Originals(self.f.root);ref=self.f.inputs['productionConfig'];originals.reference(ref);(self.f.root/ref['path']).write_bytes(b'other')
        with self.assertRaises(m.ManifestError):originals.unchanged()
        for path in ('../outside','/root/x','C:/secret','sub\\x','sub//x','sub/./x','private.key'):
            with self.assertRaises(m.ManifestError):originals.reference({'path':path,'rawSha256':'a'*64})

    def test_empty_or_unsigned_or_extra_signing_checks_rejected(self):
        for mutate in (lambda d:d.update(kind='UNSIGNED_AAB'),lambda d:d.update(signingFingerprintSha256=None),lambda d:d['checks'].append('STUB_PASSED')):
            old=self.f.get('signingVerification');self.f.change('signingVerification',mutate)
            with self.assertRaises(m.ManifestError):self.f.build()
            self.f.put_input('signingVerification',old);self.f.trust['signingVerification']=self.f.inputs['signingVerification']['rawSha256']

    def test_store_unknown_previous_account_or_nonmonotonic_number_rejected(self):
        for mutate in (lambda d:d.update(previousStoreState='UNKNOWN'),lambda d:d.update(previousStoreState='KNOWN_VERSION',previousStoreBuildNumber='9001'),lambda d:d.update(previousStoreState='KNOWN_VERSION',previousStoreBuildNumber='10000'),lambda d:d.update(storeBuildReference='')):
            old=self.f.get('storeReceipt');self.f.change('storeReceipt',mutate)
            with self.assertRaises(m.ManifestError):self.f.build()
            self.f.put_input('storeReceipt',old);self.f.trust['storeReceipt']=self.f.inputs['storeReceipt']['rawSha256']

    def test_store_delivery_mapping_and_previous_actual_originals_required(self):
        for field in ('mappingOriginal','previousStoreVersionOriginal'):
            old=self.f.get('storeReceipt');self.f.change('storeReceipt',lambda d:d[field].update(rawSha256='0'*64))
            with self.assertRaises(m.ManifestError):self.f.build()
            self.f.put_input('storeReceipt',old);self.f.trust['storeReceipt']=self.f.inputs['storeReceipt']['rawSha256']

    def test_draft_fixture_runtime_and_duplicate_puzzle_fail_even_when_header_rehashed(self):
        for use,status in (('InternalPreview','DRAFT'),('ContractTest','FIXTURE_ONLY')):
            old=self.f.get('runtimeBindings');self.f.change('runtimeBindings',lambda d:d.update(use=use,status=status))
            with self.assertRaisesRegex(m.ManifestError,'RC_RUNTIME_USE'):self.f.build()
            self.f.put_input('runtimeBindings',old);self.f.trust['runtimeBindings']=self.f.inputs['runtimeBindings']['rawSha256']
        self.f.change('releaseLock',lambda d:d['puzzles'].__setitem__(0,d['puzzles'][1]))
        with self.assertRaises(m.ManifestError):self.f.build()

    def test_identity_negative_matrix_and_android_limit(self):
        for key,val in (('releaseVersion','01.2.3'),('candidateTag','v1.2.3-rc.4\n'),('candidateOrdinal',True),('gitCommit','A'*40),('workflow','release.yml@'+'a'*40+'\n'),('buildNumber','0'),('buildNumber','2100000001'),('applicationIdentifier','invalid.qa'),('buildProfile','staging'),('platform','ANY')):
            bad={**self.f.identity,key:val}
            with self.assertRaises(m.ManifestError,msg=key):m.identity_check(bad,self.f.identity['applicationIdentifier'])
        with self.assertRaises(m.ManifestError):m.identity_check(self.f.identity,self.f.identity['applicationIdentifier'],'9002')

    def test_ci_same_run_attempt_same_both_platforms_and_no_untrusted_branch(self):
        env=dict(GITHUB_ACTIONS='true',GITHUB_REF_TYPE='tag',GITHUB_REF_NAME='v1.2.3-rc.4',GITHUB_SHA='1'*40,GITHUB_WORKFLOW_SHA='2'*40,GITHUB_RUN_NUMBER='9',GITHUB_RUN_ATTEMPT='1')
        self.assertEqual(cli.from_ci('invalid.controlled.rc','ANDROID',env)['buildNumber'],cli.from_ci('invalid.controlled.rc','IOS',env)['buildNumber'])
        for key,val in (('GITHUB_ACTIONS','false'),('GITHUB_REF_TYPE','branch'),('GITHUB_RUN_ATTEMPT','1000'),('GITHUB_REF_NAME','main'),('GITHUB_SHA','?')):
            with self.assertRaises(m.ManifestError):cli.from_ci('invalid.controlled.rc','ANDROID',{**env,key:val})
        for platform in ('ANDROID','IOS'):
            with self.assertRaisesRegex(m.ManifestError,'RC_SHARED_CI_NUMBER_LIMIT'):cli.from_ci('invalid.controlled.rc',platform,{**env,'GITHUB_RUN_NUMBER':'2100000'})

    def test_ios_receipt_roles_are_separate_from_android(self):
        target=self.f.root/'ios';target.mkdir();ios=Fixture(target,'IOS');self.assertIs(m.parse(ios.build()[0])['promotionAllowed'],False)

    def test_build_info_fourteen_exact_actual_fields_and_no_bool_versions(self):
        actual=dict(identity=self.f.identity,expected_app=self.f.identity['applicationIdentifier'],ci_build_number='9001',built_at_utc='2026-10-22T00:01:02Z',unity_version='6000.3.23f1',package_lock_sha256='a'*64,content_lock_sha256='b'*64,save_schema_version=4,level_schema_version=2,solver_version='solver-v2',workflow_run_id='123')
        value=m.build_info(**actual);self.assertEqual(len(value),14);self.assertEqual(m.read_build_info(m.canonical(value),**actual),value)
        for key,val in (('buildInfoVersion',True),('saveSchemaVersion',True),('levelSchemaVersion',True),('contentLockSha256','c'*64),('builtAtUtc','2026-02-30T00:00:00Z'),('workflowRunId','0')):
            with self.assertRaises(m.ManifestError):m.read_build_info(m.canonical({**value,key:val}),**actual)

    def test_publish_is_append_only_and_preserves_all_original_bytes(self):
        originals=m.Originals(self.f.root);raw,prov=self.f.build();self.read(raw,prov);originals.reference(self.f.inputs['productionConfig']);out=self.f.root/'published'
        m.publish(out,raw,prov,originals);self.assertEqual((out/'release-manifest.json').read_bytes(),raw);self.assertFalse((out/'INCOMPLETE').exists())
        with self.assertRaisesRegex(m.ManifestError,'RC_OUTPUT_NOT_NEW'):m.publish(out,raw,prov,originals)

    def test_private_key_material_never_copied(self):
        original=self.f.put('controlled-not-a-public-cert.txt',b'-----BEGIN '+b'PRIVATE KEY-----\nCONTROLLED-FAKE\n-----END PRIVATE KEY-----\n')
        source=m.Originals(self.f.root);source.reference(original)
        with self.assertRaisesRegex(m.ManifestError,'RC_SECRET_INPUT_FORBIDDEN'):m.publish(self.f.root/'rejected',b'{}',b'{}',source)
        self.assertTrue((self.f.root/'rejected/INCOMPLETE').exists());self.assertFalse((self.f.root/'rejected/originals').exists())

    def test_initial_vs_known_store_number_success_and_null_does_not_mean_unknown(self):
        self.f.change('storeReceipt',lambda d:d.update(previousStoreState='KNOWN_VERSION',previousStoreBuildNumber='9000'))
        self.f.build()
        self.f.change('storeReceipt',lambda d:d.update(previousStoreState='KNOWN_VERSION',previousStoreBuildNumber=None))
        with self.assertRaisesRegex(m.ManifestError,'RC_STORE_PREVIOUS_UNCONFIRMED'):self.f.build()

    def test_boolean_headers_and_forged_binding_primitive_types_rejected(self):
        for key,version in (('approvalLock','approvalLockVersion'),('runtimeBindings','runtimeImportVersion'),('signingVerification','verificationVersion'),('storeReceipt','storeReceiptVersion')):
            old=self.f.get(key);self.f.change(key,lambda d:d.update({version:True}))
            with self.assertRaises(m.ManifestError,msg=key):self.f.build()
            self.f.put_input(key,old)
            if key in self.f.trust:self.f.trust[key]=self.f.inputs[key]['rawSha256']
        auth,prior=self.f.closed();proof=self.f.request['evidence']['OWNER_GAME_ACCEPTANCE'];proof['bindings']['identity']['candidateOrdinal']=True
        with self.assertRaisesRegex(m.ManifestError,'RC_EVIDENCE_SCOPE_MISMATCH'):self.f.build(auth,prior)

    def test_false_successor_reopens_gate_without_resurrecting_old_true(self):
        auth,prior=self.f.closed();raw,prov=self.f.build(auth,prior);true_record=m.RetainedManifest(raw,prov,auth,self.f.trust,prior)
        self.f.request['evidence']['OWNER_GAME_ACCEPTANCE']=None
        newraw,newprov=self.f.build(auth,true_record);self.assertIs(m.parse(newraw)['promotionAllowed'],False)
        self.assertNotEqual(raw,newraw);self.assertIs(m.parse(raw)['promotionAllowed'],True)
        self.read(newraw,newprov,auth,true_record)

    def test_hash_newlines_and_android_schema_boundary(self):
        raw,_=self.f.build();manifest=m.parse(raw)
        for field in ('artifactSha256','gitCommit','releaseVersion','candidateTag','workflow','applicationIdentifier','buildNumber'):
            bad={**manifest,field:manifest[field]+'\n'}
            with self.assertRaises(m.ManifestError,msg=field):m.schema_read(m.canonical(bad),SCHEMA)
        for number,ok in (('2099999999',True),('2100000000',True),('2100000001',False),('999999999',True),('0001',False)):
            bad={**manifest,'buildNumber':number}
            if ok:m.schema_read(m.canonical(bad),SCHEMA)
            else:
                with self.assertRaises(m.ManifestError):m.schema_read(m.canonical(bad),SCHEMA)

    def test_actual_cli_closed_fixture_build_and_missing_source_reject_without_output(self):
        # Explicitly controlled external receipt oracle. This is not a real release.
        config={**self.f.request,'buildRequestVersion':1,'history':[]};request=self.f.put('CONTROLLED-build-request.json',config)
        trusted=self.f.put('CONTROLLED-external-trust.json',{'receiptHashes':self.f.trust,'historyReceiptHashes':{},'authorizationContexts':{'current':None}})
        schema_root=self.f.root/'schema-root';schema_dir=schema_root/'ARCHITECTURE/schemas';schema_dir.mkdir(parents=True)
        (schema_dir/'release-manifest-v2.schema.json').write_bytes(m.canonical(SCHEMA));(schema_dir/'release-lock-v1.schema.json').write_bytes(m.canonical(RELEASE))
        output=self.f.root/'CONTROLLED-CLI-OUTPUT'
        args=[sys.executable,str(Path(cli.__file__)), 'build','--request',str(self.f.root/request['path']),'--request-sha256',request['rawSha256'],'--source-root',str(self.f.root),'--artifact',str(self.f.artifact),'--expected-app',self.f.identity['applicationIdentifier'],'--ci-build-number','9001','--trust-bundle',str(self.f.root/trusted['path']),'--trust-bundle-sha256',trusted['rawSha256'],'--schema-root',str(schema_root),'--out',str(output)]
        env={k:v for k,v in os.environ.items() if k in ('PATH','SystemRoot','WINDIR','TEMP','TMP')};env['PYTHONDONTWRITEBYTECODE']='1'
        result=subprocess.run(args,env=env,capture_output=True,text=True,timeout=30);self.assertEqual(result.returncode,0,result.stderr)
        raw=(output/'release-manifest.json').read_bytes();prov=(output/'release-provenance.internal.json').read_bytes()
        value=m.read_v2(raw,prov,schema=SCHEMA,release_schema=RELEASE,originals=m.Originals(output/'originals'),artifact_path=self.f.artifact,expected_app=self.f.identity['applicationIdentifier'],trusted_receipts=self.f.trust)
        self.assertIs(value['promotionAllowed'],False)
        original=(self.f.root/self.f.inputs['signingVerification']['path']);original.unlink();rejected=self.f.root/'CONTROLLED-REJECTED';args[-1]=str(rejected)
        fail=subprocess.run(args,env=env,capture_output=True,text=True,timeout=30);self.assertEqual(fail.returncode,2);self.assertFalse(rejected.exists());self.assertNotIn('CONTROLLED CERTIFICATE',fail.stderr)

    def test_windows_reparse_attribute_is_rejected_without_is_junction_api(self):
        # Controlled lstat result, not a claim of creating an actual NTFS junction.
        original=m.os.lstat;bridge=self.f.root/'controlled-junction'
        def attrs(path,*args,**kwargs):
            if Path(path)==bridge:return SimpleNamespace(st_mode=0o40755,st_file_attributes=0x400)
            return original(path,*args,**kwargs)
        with patch.object(m.os,'lstat',side_effect=attrs):
            with self.assertRaisesRegex(m.ManifestError,'RC_REPARSE_FORBIDDEN'):m.no_links(bridge/'nested'/'new-file.json')

    def test_renewed_signing_evidence_retains_distinct_authorized_prior_receipt_hashes(self):
        auth,genesis=self.f.closed();raw,prov=self.f.build(auth,genesis)
        previous=m.RetainedManifest(raw,prov,auth,self.f.trust,genesis)
        sign=self.f.get('signingVerification');new_resource=self.f.put('controlled-second-verification.txt',b'CONTROLLED second read-only verification of the same artifact')
        sign['evidence'].append(new_resource);new_ref=self.f.put('signing-verification-renewed.json',sign)
        self.f.inputs['signingVerification']=new_ref;self.f.trust['signingVerification']=new_ref['rawSha256']
        self.f.request['evidence']['SIGNING_PACKAGE']['original']=new_ref
        self.f.request['evidence']['SIGNING_PACKAGE']['bindings']['signingVerificationRawSha256']=new_ref['rawSha256']
        new_hashes=dict(auth.original_hashes);new_hashes['SIGNING_PACKAGE']=new_ref['rawSha256'];renewed=m.AuthorizationContext(auth.subject_immutable_jcs_sha256,new_hashes)
        newraw,newprov=self.f.build(renewed,previous)
        self.assertEqual(m.immutable(m.parse(raw)),m.immutable(m.parse(newraw)))
        self.assertNotEqual(m.parse(raw)['evidenceGates']['SIGNING_PACKAGE'],m.parse(newraw)['evidenceGates']['SIGNING_PACKAGE'])
        self.assertIs(self.read(newraw,newprov,renewed,previous)['promotionAllowed'],True)
        wrong=m.RetainedManifest(raw,prov,auth,self.f.trust,genesis)
        with self.assertRaisesRegex(m.ManifestError,'RC_TRUSTED_RECEIPT_MISMATCH'):self.read(newraw,newprov,renewed,wrong)


if __name__=='__main__':unittest.main(verbosity=2)
