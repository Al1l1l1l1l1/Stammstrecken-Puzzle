"""Preserve and evaluate a downloaded CI artifact; verify its GitHub ZIP digest."""
import argparse
import base64
import gzip
import hashlib
import json
import math
from pathlib import Path
import xml.etree.ElementTree as ET
import zipfile

ROOT = Path(__file__).resolve().parents[2]
ap = argparse.ArgumentParser()
ap.add_argument('--zip', type=Path, required=True)
ap.add_argument('--sha256', required=True)
ap.add_argument('--run', type=int, required=True)
ap.add_argument('--artifact', type=int, required=True)
ap.add_argument('--commit', required=True)
ap.add_argument('--kind', choices=['edit', 'play', 'android'], required=True)
args = ap.parse_args()
raw = args.zip.read_bytes()
assert hashlib.sha256(raw).hexdigest() == args.sha256
report = dict(run=args.run, commit=args.commit, artifact=args.artifact, zipSha256=args.sha256, zipBytes=len(raw), members=[])
if args.kind in ['edit', 'play']:
    report['originalZipBase64'] = base64.b64encode(raw).decode()
with zipfile.ZipFile(args.zip) as z:
    for name in z.namelist():
        if name.endswith('/'):
            continue
        value = z.read(name)
        member = dict(name=name, bytes=len(value), sha256=hashlib.sha256(value).hexdigest())
        if name in ['editmode.xml', 'playmode.xml']:
            member['originalXmlGzipBase64'] = base64.b64encode(gzip.compress(value, mtime=0)).decode()
            root = ET.fromstring(value)
            member['testRun'] = dict(root.attrib)
            assert int(root.get('failed', '-1')) == 0 and int(root.get('skipped', '-1')) == 0
            selected = []
            for case in root.iter('test-case'):
                output = case.findtext('output') or ''
                if any(s in case.get('name', '') for s in ['HardBudget_', 'LargeRootTrace_', 'Season1Corpus']):
                    selected.append(dict(attributes=dict(case.attrib), output=output, failure=case.findtext('failure/message')))
                for line in output.splitlines():
                    if line.startswith('WP028_CORPUS_PERFORMANCE '):
                        assert 'corpusPerformance' not in member, 'exactly one corpus result is required'
                        c = json.loads(line.split(' ', 1)[1])
                        samples = sorted(float(s) for row in c['measurements'] for s in row['sampleMs'])
                        assert c['commit'] == args.commit and c['sampleCount'] == len(samples) == 350
                        assert c['solverVersion'] == 'solver-v2' and c['proofFormatVersion'] == 1
                        assert c['warmupsPerInput'] == 5 and c['samplesPerInput'] == 50 and c['parallelWorkers'] == 4
                        assert c['corpusCount'] == c['proofGatePassedCount'] == 240 and c['failures'] == []
                        assert all(row['searchNodes'] == row['requiredGuessDepth'] == 0 and len(row['sampleMs']) == 50 for row in c['measurements'])
                        index = math.ceil(.95 * len(samples)) - 1
                        assert c['p95Index'] == index == 332 and float(c['p95Ms']) == samples[index] < 250
                        assert float(c['maximumMs']) == samples[-1] < 2000 and float(c['corpusWallClockMs']) < 60000
                        member['corpusPerformance'] = c
                        member['independentlyRecalculated'] = dict(sampleCount=len(samples), p95Index=index, p95Ms=samples[index], maximumMs=samples[-1])
            member['selectedTests'] = selected
            if name == 'editmode.xml':
                assert 'corpusPerformance' in member
        report['members'].append(member)
dest = ROOT/f'PROJECT_CONTROL/Investigations/WP-034_CI_{args.run}_{args.kind}.json'
dest.write_text(json.dumps(report, indent=2)+'\n', encoding='utf-8')
print(dest.name, 'verified', [(m['name'], m.get('testRun', {}).get('result')) for m in report['members'] if 'testRun' in m])
