#!/usr/bin/env python3
"""Derive job metadata from the tagged checkout; the native importer still gates Production."""
from __future__ import annotations

import argparse
import datetime as dt
import os
from pathlib import Path
import re
import subprocess
import sys

import jsonschema
import rc_manifest as m
from rc_manifest_cli import from_ci


def git_blob(root, relative, *, commit='HEAD'):
    m.need(commit=='HEAD' or (type(commit) is str and re.fullmatch('[0-9a-f]{40}',commit)), 'RC_GIT_OBJECT_INVALID')
    m.need(type(relative) is str and re.fullmatch(r'[A-Za-z0-9_.-]+(?:/[A-Za-z0-9_.-]+)*', relative)
           and all(part not in ('.', '..') for part in relative.split('/')), 'RC_TRACKED_PATH_INVALID')
    path=m.no_links(root/relative)
    m.need(path.is_file() and 0<path.stat().st_size<=16*1024*1024, 'RC_TRACKED_FILE_MISSING')
    args=['git','--literal-pathspecs','-c','core.fsmonitor=false','-C',str(root)]
    result=subprocess.run(args+['ls-tree','-z',commit,'--',relative],capture_output=True,check=False)
    m.need(result.returncode==0 and re.fullmatch(rb'100644 blob [0-9a-f]{40}\t'+re.escape(relative.encode())+b'\0',result.stdout), 'RC_TRACKED_REGULAR_FILE_REQUIRED')
    original=subprocess.run(args+['show',commit+':'+relative],capture_output=True,check=False)
    raw=path.read_bytes()
    m.need(original.returncode==0 and original.stdout==raw, 'RC_TAGGED_SOURCE_CHANGED')
    return raw


def prepare(root,platform,expected_app):
    root=m.no_links(root)
    m.need(Path(os.environ.get('STP_PRODUCTION_SOURCE_ROOT','')).absolute()==root, 'RC_ACTUAL_CHECKOUT_SOURCE_REQUIRED')
    commit=subprocess.run(['git','-C',str(root),'rev-parse','HEAD'],capture_output=True,check=False)
    m.need(commit.returncode==0 and commit.stdout.decode().strip()==os.environ.get('GITHUB_SHA'), 'RC_CHECKOUT_COMMIT_MISMATCH')
    identity=from_ci(expected_app,platform,os.environ)
    repository=os.environ.get('GITHUB_REPOSITORY','')
    ref=os.environ.get('GITHUB_REF','')
    m.need(re.fullmatch(r'[A-Za-z0-9][A-Za-z0-9-]*/[A-Za-z0-9][A-Za-z0-9._-]*',repository)
           and ref=='refs/tags/'+identity['candidateTag']
           and os.environ.get('GITHUB_WORKFLOW_REF')==repository+'/.github/workflows/release.yml@'+ref,
           'RC_ACTUAL_WORKFLOW_REFERENCE_REQUIRED')
    recipe=git_blob(root,'.github/workflows/release.yml')
    m.need(git_blob(root,'.github/workflows/release.yml',commit=os.environ['GITHUB_WORKFLOW_SHA'])==recipe,
           'RC_ACTUAL_WORKFLOW_RECIPE_MISMATCH')
    request=m.parse(git_blob(root,os.environ.get('STP_PRODUCTION_IMPORT_REQUEST')))
    schema=m.parse(git_blob(root,'tools/campaign/ProductionImport/import-request.schema.json'))
    jsonschema.Draft202012Validator(schema).validate(request)
    m.need(request['use']=='Production', 'RC_PRODUCTION_IMPORT_REQUIRED')
    content=m.parse(git_blob(root,request['releaseLock']))
    version=git_blob(root,'ProjectSettings/ProjectVersion.txt').decode('utf-8')
    match=re.search(r'^m_EditorVersion: ([0-9]+\.[0-9]+\.[0-9]+[abfp][0-9]+)\r?$',version,re.M)
    m.need(match is not None, 'RC_TAGGED_UNITY_VERSION_REQUIRED')
    package=git_blob(root,'Packages/packages-lock.json')
    info=m.build_info(identity,expected_app=expected_app,ci_build_number=identity['buildNumber'],
        built_at_utc=dt.datetime.now(dt.timezone.utc).strftime('%Y-%m-%dT%H:%M:%SZ'),unity_version=match[1],
        package_lock_sha256=m.digest(package),content_lock_sha256=m.digest(m.canonical(content)),
        save_schema_version=3,level_schema_version=2,solver_version='solver-v2',workflow_run_id=os.environ.get('GITHUB_RUN_ID'))
    temp_input=os.environ.get('RUNNER_TEMP','')
    m.need(Path(temp_input).is_absolute(), 'RC_RUNNER_TEMP_REQUIRED')
    temp=m.no_links(temp_input)
    m.need(temp.is_dir(), 'RC_RUNNER_TEMP_REQUIRED')
    github_env=m.no_links(os.environ.get('GITHUB_ENV',''))
    m.need(github_env.is_relative_to(temp) and github_env.is_file() and github_env.stat().st_size<1024*1024, 'RC_JOB_ENVIRONMENT_FILE_REQUIRED')
    directory=m.no_links(temp/'stp-rc-metadata')
    native=m.no_links(temp/('stp-rc-native-'+platform.lower()))
    m.need(not directory.exists() and not native.exists(), 'RC_FRESH_METADATA_OUTPUT_REQUIRED')
    identity_raw=m.canonical(identity); info_raw=m.canonical(info)
    values={'STP_RC_IDENTITY':str(directory/'identity.json'),'STP_RC_IDENTITY_RAW_SHA256':m.digest(identity_raw),
            'STP_RC_BUILD_INFO':str(directory/'build-info.json'),'STP_RC_BUILD_INFO_RAW_SHA256':m.digest(info_raw),
            'STP_RC_NATIVE_OUTPUT_ROOT':str(native)}
    m.need(all(not any(ord(ch)<32 or ord(ch)==127 for ch in value) for value in values.values()), 'RC_JOB_ENVIRONMENT_INJECTION')
    directory.mkdir(mode=0o700)
    with (directory/'identity.json').open('xb') as stream:stream.write(identity_raw)
    with (directory/'build-info.json').open('xb') as stream:stream.write(info_raw)
    with github_env.open('ab') as stream:stream.write((''.join(key+'='+value+'\n' for key,value in values.items())).encode('utf-8'))
    return values


def main(argv=None):
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--platform',choices=('ANDROID','IOS'),required=True)
    parser.add_argument('--expected-app',required=True)
    parser.add_argument('--checkout',required=True)
    args=parser.parse_args(argv)
    try:
        prepare(args.checkout,args.platform,args.expected_app)
        print('RC_METADATA_PREPARED; actual Production import, signing, Store and external gates remain separate')
        return 0
    except (m.ManifestError,OSError,ValueError,KeyError,TypeError,jsonschema.ValidationError):
        print('RC_METADATA_INPUT_OR_OUTPUT_REJECTED',file=sys.stderr)
        return 2


if __name__=='__main__':
    raise SystemExit(main())
