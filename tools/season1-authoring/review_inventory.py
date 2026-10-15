"""Render factual inputs and explicitly authored, hash-bound individual decisions.

Consumes final canonical sources and actual solver traces. It does not assign an
independent quality approval, invent user-facing texts, or create target catalogs.
"""
import json
import hashlib
import sys
from pathlib import Path
sys.dont_write_bytecode = True
from select_candidates import BRIEFS, features, group, symmetry_key
from review_similarity import compare

ROOT=Path(__file__).resolve().parents[2]
OUT=ROOT/'Content/Reviews/season-1'

def main():
    docs=[json.loads(p.read_text(encoding='utf-8')) for p in sorted((ROOT/'Content/Levels/s1').rglob('*.level-v2.json'))]
    assert len(docs)==240
    assert len(__import__('sys').argv)==1, 'Generated --annotate mode removed'
    review=json.loads((OUT/'author-review.json').read_text(encoding='utf-8'))
    assert review['sourceSha256']==hashlib.sha256((OUT/'author-decisions.txt').read_text(encoding='utf-8').encode('utf-8')).hexdigest()
    author={r['puzzleId']:r for r in review['levels']}
    assert len(author)==240 and len(review['routes'])==20
    traces={entry['puzzleId']:entry['steps'] for p in OUT.glob('root-trace-*.json') for entry in json.loads(p.read_text(encoding='utf-8'))}
    proofs={d['puzzleId']:json.loads((ROOT/f'Content/Proofs/{d["puzzleId"]}/solver-v2.proof-v1.json').read_text()) for d in docs}
    sym={d['puzzleId']:symmetry_key(d) for d in docs}
    assert len(set(sym.values()))==240,'A symmetric duplicate needs replacement, never relabeling.'
    inventory=[]
    reference_fields=[('content','titleKey','Localization'),('content','statusKey','Localization'),('production','entryDeductionKey','Localization'),
                      ('completion','mapSegmentId','Completion/World'),('completion','trainMomentId','Completion/World'),('completion','resultTextKey','Localization')]
    refs=[]
    for d in docs:
        id=d['puzzleId'];f=features(d);m=proofs[id]['metrics'];steps=traces[id]
        s,r,p=d['content']['networkSection'],d['content']['route'],d['content']['position'];route=(s-1)*4+r
        a=author[id];proof=proofs[id]
        assert a['publicPuzzleHash']==proof['publicPuzzleHash'] and a['solutionHash']==proof['solutionHash']
        assert a['qualityNote']==d['production']['qualityNote'] and a['focus']==d['production']['focus'] and a['expectedChainDepth']==d['production']['chainDepth']
        entry=a['entryDeduction'];anchor=a['anchor'];g=group(p)
        if anchor=='A':entry_steps=[st['index'] for st in steps if st['ruleCode']=='ENDPOINT_ENTRY_REQUIRED']
        else:
            kind,axis,i=anchor[0],anchor[1],int(anchor[2:]);counts=d['rowCounts'] if axis=='Y' else d['columnCounts']
            length=d['grid']['width' if axis=='Y' else 'height']
            assert counts[i]==(0 if kind=='Z' else length),id+' invalid author anchor'
            code='LINE_ZERO' if kind=='Z' else 'LINE_ALL_REMAINING_OCCUPIED'
            entry_steps=[st['index'] for st in steps if st['ruleCode']==code and st['cell']['y' if axis=='Y' else 'x']==i]
        assert entry_steps,id+' author anchor absent from actual trace'
        deep=max(steps,key=lambda st:st['depth']);chain=[];current=deep
        while True:
            chain.append(current['index']);prior=[steps[pr['traceIndex']] for pr in current['premises'] if not pr['isPublicFact']]
            if not prior:break
            current=max(prior,key=lambda st:(st['depth'],-st['index']))
        chain.reverse();assert len(chain)==m['maxDeductionDepth']
        rules={code:sum(st['ruleCode']==code for st in steps) for code in sorted({st['ruleCode'] for st in steps})}
        inventory.append(dict(puzzleId=id,authoringStatus='DRAFT_REVIEW_PENDING',globalRoute=route,group=g+1,brief=BRIEFS[route-1][g],
                              grid=d['grid'],endpoints=d['endpoints'],rowCounts=d['rowCounts'],columnCounts=d['columnCounts'],
                              pathLength=f['length'],densityNumerator=f['length'],densityDenominator=d['grid']['width']*d['grid']['height'],
                              turns=f['turns'],zeroLines=f['zeros'],fullLines=f['full'],endpointRelation=f['relation'],endpointDistance=f['distance'],
                              metrics=m,entryDeduction=entry,entryTraceIndices=entry_steps,maxChainTraceIndices=chain,ruleCounts=rules,
                              qualityNote=a['qualityNote'],focus=a['focus'],expectedChainDepth=a['expectedChainDepth'],chainEstimateStatus='INDIVIDUAL_AUTHOR_ESTIMATE_NOT_PLAYTESTED',activeWidth=f['activeWidth'],activeHeight=f['activeHeight'],authorAssessment='INDIVIDUAL_AUTHOR_REVIEW; independent approval pending',
                              timeClassStatus='UNCLASSIFIED_UNCALIBRATED',miniatureStatus='INTERNAL_TEXT_GRID_ONLY',proofHash=proofs[id]['proofHash']))
        for section,field,owner in reference_fields:
            refs.append(dict(puzzleId=id,pointer=f'/{section}/{field}',reference=d[section][field],status='UNRESOLVED_TARGET',owner=owner,
                             requiredSource='Content/Localization/source/de-DE.json' if owner=='Localization' else 'Content/Catalogs/completion-v1.json plus typed World/Asset manifests',
                             effect='blocks cross-reference/content completion/import; not a proof PASS',fallbackUsed=False))
    # Actual cross-reference inventory: no target source exists. Keep this gate failing.
    assert not (ROOT/'Content/Localization/source/de-DE.json').exists()
    assert not (ROOT/'Content/Catalogs/completion-v1.json').exists()
    (OUT/'level-inventory.json').write_text(json.dumps(inventory,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    (OUT/'unresolved-references.json').write_text(json.dumps(dict(gate='FAIL_MISSING_TARGET_SOURCES',entries=refs,unresolvedCount=len(refs),fallbackUsed=False),ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    pairs=compare(docs,OUT)
    for route in range(1,21):
        selected=[i for i in inventory if i['globalRoute']==route];s=(route-1)//4+1;r=(route-1)%4+1
        lines=[f'# Author-Leveldesignreview: S1-{s:02}-{r:02} (globale Route {route:02})','',
               'Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.',
               'Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.','',
               '## Routenbogen','', '| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |','|---|---|---|']
        for g,label in enumerate(('1–2','3–5','6–8','9–10','11–12')):
            members=[i for i in selected if i['group']==g+1]
            lines.append(f'| {label} | {BRIEFS[route-1][g]} | '+', '.join(f'{i["puzzleId"]}: {i["metrics"]["maxDeductionDepth"]}' for i in members)+' |')
        lines+=['','**Individuelle Routenbewertung:** '+review['routes'][str(route)],'', 'Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.','']
        for item in selected:
            d=next(d for d in docs if d['puzzleId']==item['puzzleId']);w,h=d['grid']['width'],d['grid']['height'];m=item['metrics'];a,b=d['endpoints'].values()
            lines += [f'## {item["puzzleId"]}','',f'- Input: {w}×{h}; A {a["side"]}/{a["index"]}, B {b["side"]}/{b["index"]}. Zeilen `{d["rowCounts"]}`, Spalten `{d["columnCounts"]}`.',
                      f'- Lösung: {item["pathLength"]} Zellen, {item["turns"]} Kurven; Dichte {item["densityNumerator"]}/{item["densityDenominator"]}. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.',
                      f'- Nachweis: UNIQUE/1, searchNodes={m["searchNodes"]}, requiredGuessDepth={m["requiredGuessDepth"]}, deductionSteps={m["deductionSteps"]}, maxDeductionDepth={m["maxDeductionDepth"]}.',
                      f'- Einstieg: {item["entryDeduction"]} Belegschritte: `{item["entryTraceIndices"]}`.',
                      f'- Eine tatsächliche maximale Prämissenkette: `{item["maxChainTraceIndices"]}` im zugehörigen Rootspurbericht.',
                      f'- Individueller Fokus: `{item["focus"]}`; erwartete gedankliche Folge {item["expectedChainDepth"]} (eigene vorläufige Schätzung, nicht gemessen).',
                      f'- Individuelle fachliche Entscheidung: {item["qualityNote"]}',
                      '- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.','', '```text']
            chars={'TRACK_NS':'│','TRACK_EW':'─','TRACK_NE':'└','TRACK_ES':'┌','TRACK_SW':'┐','TRACK_WN':'┘'}
            grid=[['.']*w for _ in range(h)]
            for cell in d['solution']['path']:grid[cell['y']][cell['x']]=chars[cell['track']]
            lines += [f'{y:02} '+''.join(row) for y,row in enumerate(grid)]+['```','']
        (OUT/f'leveldesign-S1-{s:02}-{r:02}.md').write_text('\n'.join(lines).rstrip()+'\n',encoding='utf-8')
    print(f'Persisted 240 individual assessments/entries, 20 route reviews, 20 actual traces; cross-reference gate FAIL with {len(refs)} missing targets; exact symmetric duplicates 0.')


if __name__=='__main__':main()
