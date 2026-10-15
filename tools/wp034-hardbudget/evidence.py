"""Recheck downloaded original ZIPs/XMLs and preserve lossless XML evidence."""
import base64
import gzip
import hashlib
import json
from pathlib import Path
import statistics
import xml.etree.ElementTree as ET
import zipfile

ROOT = Path(__file__).resolve().parents[2]
SCRATCH = ROOT/'.utmp/wp034'
OUT = ROOT/'PROJECT_CONTROL/Investigations/WP-034_ORIGINALS.json'
NAME = 'HardBudget_NoResultIsBuiltAfterTheLastProbe_ForEveryResultKind'


def sha(raw):
    return hashlib.sha256(raw).hexdigest()


def main():
    records = []
    times = []
    for run, artifact, digest in [
        (38668535139, 11943314122, 'e021e4f7b97a4f06165a34ad073f87e3f7e52146097f70524e602164b1d83af6'),
        (38668540223, 11943991211, 'fe97dec7c86582cee8f9b4b4f8a3d01e02e139947333b6ff7d7b9b0ecf1b8ace')]:
        raw = (SCRATCH/f'{artifact}.zip').read_bytes()
        assert sha(raw) == digest
        with zipfile.ZipFile(SCRATCH/f'{artifact}.zip') as z:
            member = next(n for n in z.namelist() if n.endswith('editmode.xml'))
            xml = z.read(member)
        root = ET.fromstring(xml)
        case = next(c for c in root.iter('test-case') if c.get('name') == NAME)
        corpus = next(json.loads(line.split(' ', 1)[1]) for c in root.iter('test-case') for line in (c.findtext('output') or '').splitlines() if line.startswith('WP028_CORPUS_PERFORMANCE '))
        times.append({c.get('fullname'): float(c.get('duration', '0')) for c in root.iter('test-case')})
        records.append(dict(run=run, artifact=artifact, zipBytes=len(raw), zipSha256=digest, originalZipBase64=base64.b64encode(raw).decode(),
                            xmlMember=member, xmlBytes=len(xml), xmlSha256=sha(xml),
                            originalXmlGzipBase64=base64.b64encode(gzip.compress(xml, mtime=0)).decode(),
                            root=dict(root.attrib), test=dict(case.attrib), output=case.findtext('output'),
                            failure=case.findtext('failure/message'),
                            observedEnvironmentAndSeparateCorpusMeasurement={k: corpus[k] for k in ['commit', 'processor', 'processorCount', 'runtime', 'os', 'p95Ms', 'maximumMs', 'corpusWallClockMs']}))
    pairs = [dict(test=n, passSeconds=times[0][n], failSeconds=times[1][n], ratio=times[1][n]/times[0][n]) for n in sorted(times[0]) if times[0][n] > .1]
    data = dict(records=records, durationPairs=pairs, medianRatio=statistics.median(p['ratio'] for p in pairs), boundary='Historical scheduling/GC/CPU circumstances not observable; artifact bytes and test outputs are evidence, not a cause claim.')
    OUT.write_text(json.dumps(data, ensure_ascii=False, indent=2)+'\n', encoding='utf-8')
    print('Original evidence verified:', [(x['run'], x['test']['result'], x['test']['duration']) for x in records], 'median ratio', data['medianRatio'])


if __name__ == '__main__':
    main()
