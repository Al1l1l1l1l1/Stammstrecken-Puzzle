#!/usr/bin/env python3
"""Controlled CI facts and filesystem outputs; no Production acceptance or Unity run."""
import json
import os
from pathlib import Path
import subprocess
import tempfile
import unittest
from unittest import mock
import rc_ci_metadata as subject


class MetadataTests(unittest.TestCase):
    def setUp(self):
        self.temp=tempfile.TemporaryDirectory(prefix='metadata-test-',dir=Path(__file__).resolve().parents[2]/'.utmp')
        self.addCleanup(self.temp.cleanup)
        self.base=Path(self.temp.name);self.repo=self.base/'checkout';self.repo.mkdir()
        self.env_file=self.base/'github-env';self.env_file.write_bytes(b'PRIOR=value\n')
        self.sha='a'*40
        request={'importRequestVersion':1,'use':'Production','publicCatalog':{'catalogId':'controlled','revision':1},
            **{key:key+'.json' for key in ('campaign','campaignCompletion','endlessCompletion','calibration','cosmetics','railwayAssets','toolchainLock','uiSourceLock')},
            'releaseLock':'release.json','profiles':['profile'+str(i)+'.json' for i in range(4)],
            'levels':[{'level':'level'+str(i)+'.json','proof':'proof'+str(i)+'.json'} for i in range(240)],
            'localizations':['locale.json'],'evidence':['controlled-not-approval.json'],'publishedLocks':[]}
        actual_schema=Path(__file__).resolve().parents[2]/'tools/campaign/ProductionImport/import-request.schema.json'
        self.sources={'.github/workflows/release.yml':b'name: controlled recipe fixture\n','request.json':json.dumps(request).encode(),'release.json':b'{"controlled":"NOT AN APPROVED CONTENT LOCK"}',
            'tools/campaign/ProductionImport/import-request.schema.json':actual_schema.read_bytes(),
            'Packages/packages-lock.json':b'{"controlled":"package lock fixture"}',
            'ProjectSettings/ProjectVersion.txt':b'm_EditorVersion: 6000.3.23f1\nm_EditorVersionWithRevision: unused\n'}
        env={'STP_PRODUCTION_SOURCE_ROOT':str(self.repo),'STP_PRODUCTION_IMPORT_REQUEST':'request.json',
            'RUNNER_TEMP':str(self.base),'GITHUB_ENV':str(self.env_file),'GITHUB_SHA':self.sha,
            'GITHUB_ACTIONS':'true','GITHUB_REF_TYPE':'tag','GITHUB_REF_NAME':'v1.0.0-rc.1',
            'GITHUB_REPOSITORY':'controlled/fixture','GITHUB_REF':'refs/tags/v1.0.0-rc.1',
            'GITHUB_WORKFLOW_REF':'controlled/fixture/.github/workflows/release.yml@refs/tags/v1.0.0-rc.1',
            'GITHUB_WORKFLOW_SHA':'b'*40,'GITHUB_RUN_NUMBER':'12','GITHUB_RUN_ATTEMPT':'2','GITHUB_RUN_ID':'1234'}
        patch=mock.patch.dict(os.environ,env,clear=True);patch.start();self.addCleanup(patch.stop)
        self.workflow_recipe=self.sources['.github/workflows/release.yml']
        patch=mock.patch.object(subject,'git_blob',side_effect=lambda root,path,commit='HEAD':self.sources[path] if commit=='HEAD' else self.workflow_recipe);patch.start();self.addCleanup(patch.stop)
        self.head=mock.patch.object(subject.subprocess,'run',return_value=subprocess.CompletedProcess([],0,stdout=(self.sha+'\n').encode(),stderr=b''))
        self.head.start();self.addCleanup(self.head.stop)

    def call(self):return subject.prepare(self.repo,'ANDROID','de.stammstrecken.puzzle')

    def test_actual_metadata_bytes_and_only_five_environment_outputs(self):
        values=self.call();identity=subject.m.parse(Path(values['STP_RC_IDENTITY']).read_bytes())
        info=subject.m.parse(Path(values['STP_RC_BUILD_INFO']).read_bytes())
        self.assertEqual('12002',identity['buildNumber'])
        self.assertEqual(14,len(info));self.assertEqual(9,len(identity));self.assertEqual(5,len(values))
        self.assertEqual(subject.m.digest(subject.m.canonical(subject.m.parse(self.sources['release.json']))),info['contentLockSha256'])
        self.assertTrue(self.env_file.read_text().startswith('PRIOR=value\n'))
        self.assertFalse(Path(values['STP_RC_NATIVE_OUTPUT_ROOT']).exists())

    def test_branch_wrong_head_wrong_source_fail_without_output(self):
        for key,value in [('GITHUB_REF_TYPE','branch'),('GITHUB_SHA','c'*40),('STP_PRODUCTION_SOURCE_ROOT',str(self.base))]:
            with mock.patch.dict(os.environ,{key:value}):
                with self.assertRaises(subject.m.ManifestError):self.call()
                self.assertFalse((self.base/'stp-rc-metadata').exists())

    def test_contracttest_request_is_never_production(self):
        self.sources['request.json']=self.sources['request.json'].replace(b'Production',b'ContractTest')
        with self.assertRaises(subject.m.ManifestError):self.call()
        self.assertFalse((self.base/'stp-rc-metadata').exists())

    def test_environment_target_outside_runner_temp_and_control_characters_fail(self):
        with mock.patch.dict(os.environ,{'GITHUB_ENV':str(self.repo.parent.parent/'outside')}):
            with self.assertRaises(subject.m.ManifestError):self.call()
        with mock.patch.dict(os.environ,{'GITHUB_RUN_ID':'1234\nINJECT=YES'}):
            with self.assertRaises(subject.m.ManifestError):self.call()
        self.assertEqual(b'PRIOR=value\n',self.env_file.read_bytes())

    def test_existing_outputs_are_never_replaced(self):
        directory=self.base/'stp-rc-metadata';directory.mkdir();(directory/'keep').write_bytes(b'KEEP')
        with self.assertRaises(subject.m.ManifestError):self.call()
        self.assertEqual(b'KEEP',(directory/'keep').read_bytes())

    def test_invalid_tag_run_and_attempt_cannot_produce_metadata(self):
        for key,value in [('GITHUB_REF_NAME','v1.0.0'),('GITHUB_RUN_NUMBER','0'),('GITHUB_RUN_ATTEMPT','1000'),('GITHUB_WORKFLOW_SHA','not-a-sha')]:
            with mock.patch.dict(os.environ,{key:value}):
                with self.assertRaises(subject.m.ManifestError):self.call()
        self.assertFalse((self.base/'stp-rc-metadata').exists())

    def test_foreign_workflow_path_repository_ref_or_recipe_cannot_stamp_release_recipe(self):
        for key,value in [('GITHUB_WORKFLOW_REF','controlled/fixture/.github/workflows/foreign.yml@refs/tags/v1.0.0-rc.1'),
                          ('GITHUB_REPOSITORY','foreign/repository'),('GITHUB_REF','refs/tags/v1.0.0-rc.2')]:
            with mock.patch.dict(os.environ,{key:value}):
                with self.assertRaises(subject.m.ManifestError):self.call()
        self.workflow_recipe=b'name: DIFFERENT ACTUAL RECIPE\n'
        with self.assertRaises(subject.m.ManifestError):self.call()
        self.assertFalse((self.base/'stp-rc-metadata').exists())


if __name__=='__main__':unittest.main(verbosity=2)
