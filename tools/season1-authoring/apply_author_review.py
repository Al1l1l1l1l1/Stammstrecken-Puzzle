"""Faithfully transport explicit author decisions, never infer quality judgments.

Bound review JSON is author evidence, not a second canonical puzzle source.
Changed public input or solution requires a new individual review and binding.
"""
import json
import hashlib
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
OUT=ROOT/'Content/Reviews/season-1'

def validate_binding(d,r,proof):
    # These two restricted projections have fixed ASCII keys, integer numbers and
    # ASCII enum/ID/hash strings. Compact sorted JSON equals JCS on that domain;
    # the complete production/Node checks remain the authoritative hash evidence.
    def digest(value):
        return hashlib.sha256(json.dumps(value,sort_keys=True,separators=(',',':'),ensure_ascii=False).encode('utf-8')).hexdigest()
    public={k:d[k] for k in ('puzzleId','rulesetVersion','grid','endpoints','rowCounts','columnCounts')}
    assert digest(public)==r['publicPuzzleHash']['sha256'],d['puzzleId']+' changed public input needs review'
    solution=dict(puzzleId=d['puzzleId'],publicPuzzleHash=r['publicPuzzleHash'],path=d['solution']['path'])
    assert digest(solution)==r['solutionHash']['sha256'],d['puzzleId']+' changed solution needs review'
    assert r['publicPuzzleHash']==proof['publicPuzzleHash'] and r['solutionHash']==proof['solutionHash']

def main():
    review=json.loads((OUT/'author-review.json').read_text(encoding='utf-8'))
    assert review['source']=='author-decisions.txt'
    assert review['sourceSha256']==hashlib.sha256((OUT/'author-decisions.txt').read_text(encoding='utf-8').encode('utf-8')).hexdigest(), 'Literal author source changed; new explicit binding required'
    records={r['puzzleId']:r for r in review['levels']}
    paths=sorted((ROOT/'Content/Levels/s1').rglob('*.level-v2.json'))
    assert len(paths)==len(records)==240
    pending=[]
    for path in paths:
        d=json.loads(path.read_text(encoding='utf-8'));id=d['puzzleId'];r=records[id]
        proof=json.loads((ROOT/f'Content/Proofs/{id}/solver-v2.proof-v1.json').read_text())
        validate_binding(d,r,proof)
        assert r['qualityNote'] and r['entryDeduction'] and r['focus'] and r['expectedChainDepth']>=1
        # A repeated transport of identical judgments must preserve the current
        # revision. A later explicit metadata correction advances it once.
        changed=any(d['production'][key]!=value for key,value in (
            ('qualityNote',r['qualityNote']),('focus',r['focus']),
            ('chainDepth',r['expectedChainDepth'])))
        d['production']['qualityNote']=r['qualityNote'];d['production']['focus']=r['focus']
        d['production']['chainDepth']=r['expectedChainDepth']
        if changed:d['contentRevision']+=1
        pending.append((path,d))
    # Preflight every binding before any canonical document is touched.
    for path,d in pending:path.write_text(json.dumps(d,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    print('Transported 240 explicit hash-bound decisions; no generated quality, entry, time or product text.')

if __name__=='__main__':main()
