"""Validate all actual Season-1 schemas/JCS bindings and review trace records.

Run in the pinned validator environment (CPython 3.11.13, Node 22.20.0,
requirements.lock.txt), using the unchanged Node reference implementation.
Does not imply cross-reference, human review, import or release approval.
"""
import base64
import hashlib
import json
import subprocess
from pathlib import Path
from jsonschema import Draft202012Validator

ROOT=Path(__file__).resolve().parents[2]
OUT=ROOT/'Content/Reviews/season-1'
SCRATCH=ROOT/'.utmp/wp026-jcs'

def strict_read(path):
    def pairs(items):
        result={}
        for k,v in items:
            if k in result:raise ValueError('duplicate JSON key: '+k)
            result[k]=v
        return result
    return json.loads(path.read_text(encoding='utf-8'),object_pairs_hook=pairs)

def jcs(value, name):
    path=SCRATCH/f'{name}.json';path.write_text(json.dumps(value,ensure_ascii=False,separators=(',',':')),encoding='utf-8')
    result=subprocess.run(['node','tools/architecture-validation/jcs_crosscheck.mjs',str(path)],cwd=ROOT,check=True,text=True,capture_output=True)
    output=json.loads(result.stdout);canonical=base64.b64decode(output['canonicalBase64'])
    assert hashlib.sha256(canonical).hexdigest()==output['sha256']
    return canonical,output['sha256']

def main():
    assert subprocess.check_output(['node','--version'],text=True).strip()=='v22.20.0'
    SCRATCH.mkdir(parents=True,exist_ok=True)
    levels=sorted((ROOT/'Content/Levels/s1').rglob('*.level-v2.json'))
    assert len(levels)==240
    level_schema=Draft202012Validator(strict_read(ROOT/'ARCHITECTURE/schemas/level-v2.schema.json'))
    proof_schema=Draft202012Validator(strict_read(ROOT/'ARCHITECTURE/schemas/proof-v1.schema.json'))
    trace={entry['puzzleId']:entry for p in OUT.glob('root-trace-*.json') for entry in strict_read(p)}
    evidence={entry['puzzleId']:entry for entry in strict_read(OUT/'technical-evidence.json')['levelsEvidence']}
    inventory={entry['puzzleId']:entry for entry in strict_read(OUT/'level-inventory.json')}
    entries=[];ids=set()
    for path in levels:
        d=strict_read(path);id=d['puzzleId'];assert id not in ids;ids.add(id);level_schema.validate(d)
        s,r,p=d['content']['networkSection'],d['content']['route'],d['content']['position']
        assert id==f'S1-{s:02}-{r:02}-{p:02}'
        assert path.relative_to(ROOT).as_posix()==f'Content/Levels/s1/{s:02}/{r:02}/{id}.level-v2.json'
        proof_path=ROOT/f'Content/Proofs/{id}/solver-v2.proof-v1.json';proof=strict_read(proof_path);proof_schema.validate(proof)
        assert proof['puzzleId']==id and proof['solverVersion']=='solver-v2' and proof['solutionCount']==1
        public={k:d[k] for k in ('puzzleId','rulesetVersion','grid','endpoints','rowCounts','columnCounts')}
        _,pubhash=jcs(public,id+'-public')
        assert proof['publicPuzzleHash']==dict(profile='STP-PUZZLE-SEMANTIC-JCS-1',sha256=pubhash)
        solution=dict(puzzleId=id,publicPuzzleHash=proof['publicPuzzleHash'],path=d['solution']['path'])
        _,solhash=jcs(solution,id+'-solution');assert proof['solutionHash']==dict(profile='STP-SOLUTION-JCS-1',sha256=solhash)
        projection={k:v for k,v in proof.items() if k!='proofHash'}
        _,proofhash=jcs(projection,id+'-proof-projection');assert proof['proofHash']==dict(profile='STP-PROOF-JCS-1',sha256=proofhash)
        canonical,_=jcs(proof,id+'-proof');assert canonical==proof_path.read_bytes()
        assert d['proofRef']==dict(artifactId=f'proofs/{id}/solver-v2.proof-v1.json',proofFormatVersion=1,proofHash=proof['proofHash'])
        _,documenthash=jcs(d,id+'-document');assert documenthash==evidence[id]['documentSha256']
        assert evidence[id]['publicPuzzleHash']==pubhash and evidence[id]['solutionHash']==solhash and evidence[id]['proofHash']==proofhash
        assert trace[id]['publicPuzzleHash']==proof['publicPuzzleHash'] and trace[id]['proofHash']==proofhash
        steps=trace[id]['steps'];depths=[];unique=set();w,h=d['grid']['width'],d['grid']['height']
        solution_values={(c['x'],c['y']):c['track'] for c in d['solution']['path']}
        for i,step in enumerate(steps):
            assert step['index']==i
            indices=[pr['traceIndex'] for pr in step['premises'] if not pr['isPublicFact']]
            assert all(0<=j<i for j in indices)
            depth=1+max((depths[j] for j in indices),default=0);depths.append(depth);assert depth==step['depth']
            cell=step['cell'];assert 0<=cell['x']<w and 0<=cell['y']<h
            value=solution_values.get((cell['x'],cell['y']),'EMPTY');assert value not in step['removedValues']
            key=(cell['x'],cell['y'],tuple(step['removedValues']),tuple(step['remainingValues']));assert key not in unique;unique.add(key)
        assert proof['metrics']['deductionSteps']==len(steps) and proof['metrics']['maxDeductionDepth']==max(depths)
        assert proof['metrics']['requiredGuessDepth']==0 and proof['metrics']['searchNodes']==0
        assert inventory[id]['metrics']==proof['metrics']
        assert len(inventory[id]['maxChainTraceIndices'])==max(depths)
        assert d['production']['starThresholdsSeconds'] is None
        entries.append(dict(puzzleId=id,publicPuzzleHash=pubhash,solutionHash=solhash,proofHash=proofhash,documentSha256=documenthash,nodeCanonicalProofBytesEqual=True))
        if len(entries)%48==0:print(f'SCHEMA/NODE {len(entries)}/240',flush=True)
    expected={f'S1-{s:02}-{r:02}-{p:02}' for s in range(1,6) for r in range(1,5) for p in range(1,13)}
    assert ids==expected and set(trace)==ids and set(inventory)==ids and set(evidence)==ids
    references=strict_read(OUT/'unresolved-references.json');assert references['unresolvedCount']==1440 and references['gate']=='FAIL_MISSING_TARGET_SOURCES'
    report=dict(schemaLevelsPassed=240,schemaProofsPassed=240,nodePublicSolutionProofProjectionArtifactAndDocumentChecks=1200,
                rootTraceAndInventoryBindingsPassed=240,full5x4x12InventoryPassed=True,unresolvedReferences=1440,crossReferenceGate='FAIL_MISSING_TARGET_SOURCES',
                nodeVersion='22.20.0',entries=entries)
    (OUT/'schema-node-evidence.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    print('PASS: 240 real Level-/Proofschemas, 1200 actual Node projections/artifacts/documents, 240 trace/inventory bindings; cross-reference gate remains FAIL.')

if __name__=='__main__':main()
