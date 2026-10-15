"""Run the unchanged 240-entry verification in an isolated current-source export.

Generated review data stays in scratch. Compare every regenerated root trace
against canonical Git bytes and preserve existing level/proof bytes unchanged.
"""
import hashlib
import json
from pathlib import Path
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[2]
DEST = ROOT/'.utmp/wp034/production'
paths = subprocess.check_output(['git', 'ls-files', '--', 'Assets/StammstreckenPuzzle/Scripts', 'Content', 'ARCHITECTURE', 'tools/season1-authoring', 'tools/architecture-validation'], cwd=ROOT, text=True).splitlines()
hashes = {}
for name in paths:
    path = ROOT/name
    if not path.is_file():
        continue
    raw = subprocess.check_output(['git', 'show', 'HEAD:'+name], cwd=ROOT) if name.startswith('Content/') else path.read_bytes()
    target = DEST/name
    target.parent.mkdir(parents=True, exist_ok=True)
    target.write_bytes(raw)
    if name.startswith(('Content/Levels/', 'Content/Proofs/', 'Content/Reviews/season-1/root-trace-')):
        hashes[name] = hashlib.sha256(raw).hexdigest()
r = subprocess.run([sys.executable, '-B', str(DEST/'tools/season1-authoring/run_authoring.py'), 'verify'], cwd=DEST, text=True, encoding='utf-8', errors='replace', capture_output=True)
(DEST.parent/'production.txt').write_text(r.stdout+r.stderr, encoding='utf-8')
print(r.stdout+r.stderr)
assert r.returncode == 0
roots = {}
for name, digest in hashes.items():
    raw = (DEST/name).read_bytes()
    if name.startswith('Content/Reviews/season-1/root-trace-'):
        # C# indented JSON uses platform newlines. Compare the canonical Git newline profile explicitly; retain
        # both byte hashes, and also compare decoded objects so no value/ordering change is hidden by formatting.
        expected = subprocess.check_output(['git', 'show', 'HEAD:'+name], cwd=ROOT)
        assert json.loads(raw) == json.loads(expected), name
        assert hashlib.sha256(raw.replace(b'\r\n', b'\n')).hexdigest() == digest, name
        roots[name] = dict(gitBlobSha256=digest, generatedRawSha256=hashlib.sha256(raw).hexdigest(), gitLfProfileEqual=True, jsonEqual=True)
    else:
        assert hashlib.sha256(raw).hexdigest() == digest, name
report = dict(exactCanonicalInputHashes={n:h for n,h in hashes.items() if n not in roots}, regeneratedRoots=roots, generatedSummary=json.loads((DEST/'Content/Reviews/season-1/technical-evidence.json').read_text()), source='Current working source; scratch export; not Unity CI')
(ROOT/'PROJECT_CONTROL/Investigations/WP-034_PRODUCTION.json').write_text(json.dumps(report, indent=2)+'\n', encoding='utf-8')
print('All canonical level/proof bytes unchanged; 20 root traces JSON-identical and identical in Git LF profile (raw generation hashes separate).')
