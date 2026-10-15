"""WP-032 diagnostic: independently retain composition and canonical evidence bindings.

This supplements the unchanged authoritative scope validator; it grants no gate waiver.
"""
import argparse, base64, hashlib, json, subprocess
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
C = '060c1c4b01458a80835094d5bd004be8c36eec54'
D = 'd7f69f66e6d58191fa44d64a20523f78ad9a5432'
M = 'd90e798dd9b26d3f152f3b0c2d1d52cae641eb16'
A = '9c1859f649201602e34770c5399f8fca5a39c878'
B = 'c9516bdbd5858fd9003d5207ec37d180ccc6ae23'
CONFLICTS = {'.github/workflows/validate.yml', 'PROJECT_CONTROL/CURRENT_STATE.md', 'PROJECT_CONTROL/WORK_QUEUE.md'}

def git(*args):
    return subprocess.check_output(['git', *args], cwd=ROOT)

def blob(ref, path):
    return git('show', f'{ref}:{path}')

def tree(ref):
    result = {}
    for row in git('ls-tree', '-rz', '--full-tree', ref).split(b'\0'):
        if row:
            value, path = row.split(b'\t', 1)
            result[path.decode('utf-8')] = value.decode('ascii')
    return result

def sha(data):
    return hashlib.sha256(data).hexdigest()

def verify(ref):
    assert git('show', '-s', '--format=%P', C).decode().strip().split() == [D, M]
    assert git('merge-base', D, M).decode().strip() == B
    assert git('merge-base', '--is-ancestor', C, ref) == b''
    assert git('merge-base', '--is-ancestor', A, ref) == b''
    trees = {r: tree(r) for r in (B, D, M, C, A, ref)}
    before, delivery, main, common, author, now = (trees[r] for r in (B, D, M, C, A, ref))
    paths = set(before) | set(delivery) | set(main)
    count = 0
    for path in sorted(paths - CONFLICTS):
        old, left, right = before.get(path), delivery.get(path), main.get(path)
        if left == right:
            expected = left
        elif left == old:
            expected = right
        elif right == old:
            expected = left
        else:
            raise AssertionError(('unaccounted conflict', path))
        assert common.get(path) == expected, path
        count += 1
    assert count == 1109 and set(common) == paths
    # The already composed input is not hidden by the new manifest base.
    # Check all original main imports and all non-documentary delivery trees again.
    unchanged = []
    prefixes = ('Assets/', 'Packages/', 'ProjectSettings/', 'ARCHITECTURE/', 'DECISIONS/',
                'Stammstrecken_Puzzle_Konzept_00-15/', 'Content/Levels/', 'Content/Proofs/',
                'tools/architecture-validation/', 'tools/season1-authoring/', 'tools/wp030-investigation/')
    manifest = 'tools/architecture-validation/scopes/WP-032.production.scope.json'
    for path in sorted(set(common) | set(now)):
        if path.startswith(prefixes) and path != manifest:
            assert now.get(path) == common.get(path), path
            unchanged.append(path)
    for path in main:
        if path not in CONFLICTS and main.get(path) != before.get(path):
            assert now.get(path) == main[path], ('main import', path)
    for path in author:
        if path.startswith('PROJECT_CONTROL/Integration/WP-031_') or path == 'tools/wp031-integration/prepare.py':
            assert now.get(path) == author[path], ('author evidence', path)
    # The only workflow mutation is the active prospective manifest assignment.
    assert blob(ref, '.github/workflows/validate.yml') == blob(C, '.github/workflows/validate.yml').replace(
        b'WP-031.production.scope.json', b'WP-032.production.scope.json')
    review = 'Content/Reviews/season-1/'
    bindings = json.loads(blob(ref, review + 'F01_BINDINGS.json'))
    assert 'Git blob' in bindings['fileHashProfile']['byteBasis']
    for path, expected in bindings['files'].items():
        assert sha(blob(ref, review + path)) == expected, path
    inventory = json.loads(blob(ref, 'PROJECT_CONTROL/Integration/WP-031_SHA256.json'))
    for item in inventory:
        data = blob(ref, item['path'])
        assert len(data) == item['bytes'] and sha(data) == item['sha256'], item['path']
        assert 'Git blob' in item['byteBasis']
    # Decode via the existing original-report transport implementation, without normalization.
    import importlib.util
    spec = importlib.util.spec_from_file_location('wp031_transport', ROOT / 'tools/wp031-integration/prepare.py')
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    original = module.decode(ROOT / review / 'WP-026_Gesamtabnahme_b1457ba.txt')
    assert len(original) == 39661 and sha(original) == '9c0a03fe060e86e69d5041bb3e72e72a55c198029ac3a9990222e16d99e15433'
    return dict(ref=git('rev-parse', ref).decode().strip(), tree=git('rev-parse', ref+'^{tree}').decode().strip(),
                composedInput=C, delivery=D, main=M, authorCorrection=A, nonConflictPathsVerified=count,
                exactUnchangedProductionSourcePaths=len(unchanged), f01GitBindings=len(bindings['files']),
                priorEvidenceGitBindings=len(inventory), originalDecodedSha256=sha(original),
                originalScopeResult='WP-031 combined FAIL9 preserved; not reclassified',
                boundary='diagnostic provenance only; authoritative scope, independent conflict review and actual common CI remain separate')

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--ref', default='HEAD')
    parser.add_argument('--output', type=Path)
    args = parser.parse_args()
    result = verify(args.ref)
    text = json.dumps(result, ensure_ascii=False, indent=2) + '\n'
    if args.output:
        args.output.write_text(text, encoding='utf-8', newline='\n')
    print(text, end='')
