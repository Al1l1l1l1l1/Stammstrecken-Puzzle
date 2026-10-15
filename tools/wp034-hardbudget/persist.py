"""Preserve own raw supplementary evidence with explicit byte profiles."""
import base64
import gzip
import hashlib
import json
from pathlib import Path
import subprocess
import zipfile

ROOT = Path(__file__).resolve().parents[2]
S = ROOT/'.utmp/wp034'
files = ['old/output.txt', 'old/sources.json', 'old-profile/output.txt', 'old-profile/sources.json',
         'current/output.txt', 'current/sources.json', 'current-profile/output.txt', 'current-profile/sources.json',
         'pre-repair-regression.xml', 'pre-repair-regression.txt', 'all-pure.xml', 'all-pure.txt',
         'production.txt', 'schema-node.txt', 'scope-before.txt',
         'job116206010861.txt', 'job116206038091.txt', 'baseline-job.txt']
for name in ['late-result', 'omit-restore', 'reverse-deletion']:
    files += ['mutation-'+name+'.txt', 'current-'+name+'/mutation.json']
payloads = []
for name in files:
    path = S/name
    if not path.exists():
        continue
    raw = path.read_bytes()
    payloads.append(dict(path=name, bytes=len(raw), sha256=hashlib.sha256(raw).hexdigest(), originalGzipBase64=base64.b64encode(gzip.compress(raw, mtime=0)).decode()))
with zipfile.ZipFile(S/'baseline.zip') as z:
    xml = z.read('editmode.xml')
baseline = dict(commit='840b92942bf03192780a48ddfb62be1530b59b9c', run=38724396166, job=116386670858, artifact=11968246066,
                zipSha256=hashlib.sha256((S/'baseline.zip').read_bytes()).hexdigest(), xmlSha256=hashlib.sha256(xml).hexdigest(),
                originalXmlGzipBase64=base64.b64encode(gzip.compress(xml, mtime=0)).decode())
source = subprocess.check_output(['git', 'diff', '--', 'Assets/StammstreckenPuzzle/Scripts/Puzzle/Solver', 'Assets/StammstreckenPuzzle/Tests/EditMode/Solver'], cwd=ROOT)
data = dict(role='Author-operated supplementary probes, no independent approval', baseline=baseline, payloads=payloads,
            workingSourceDiffSha256=hashlib.sha256(source).hexdigest(),
            byteProfiles='Payload hashes bind exact saved bytes. Job logs are connector-decoded text saved with LF, not ZIP bytes. XMLs preserve exact artifact member bytes. Source hashes in compare.py normalize CRLF to LF. Production root output has separate raw and Git-LF hashes.')
(ROOT/'PROJECT_CONTROL/Investigations/WP-034_LOCAL.json').write_text(json.dumps(data, indent=2)+'\n', encoding='utf-8')
print('Persisted', len(payloads), 'payloads; source diff', data['workingSourceDiffSha256'])
