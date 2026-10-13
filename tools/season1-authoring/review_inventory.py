"""Persist factual review inputs, per-level author assessment and unresolved targets.

Consumes final canonical sources and actual solver traces. It does not assign an
independent quality approval, invent user-facing texts, or create target catalogs.
"""
import json
import sys
from pathlib import Path
sys.dont_write_bytecode = True
from select_candidates import BRIEFS, features, group, symmetry_key

ROOT=Path(__file__).resolve().parents[2]
OUT=ROOT/'Content/Reviews/season-1'

def main():
    docs=[json.loads(p.read_text(encoding='utf-8')) for p in sorted((ROOT/'Content/Levels/s1').rglob('*.level-v2.json'))]
    assert len(docs)==240
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
        # First author-intended entry: direct zero/full line if applicable;
        # otherwise the public endpoint fact, with a concrete cell/port.
        zeros=[('Zeile',i) for i,n in enumerate(d['rowCounts']) if n==0]+[('Spalte',i) for i,n in enumerate(d['columnCounts']) if n==0]
        full=[('Zeile',i) for i,n in enumerate(d['rowCounts']) if n==d['grid']['width']]+[('Spalte',i) for i,n in enumerate(d['columnCounts']) if n==d['grid']['height']]
        g=group(p)
        if zeros and not (route==1 and g==2):
            axis,i=zeros[0];entry=f'{axis} {i} hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht.'
            entry_steps=[st['index'] for st in steps if st['ruleCode']=='LINE_ZERO' and
                         st['cell']['y' if axis=='Zeile' else 'x']==i]
        elif full:
            axis,i=full[0];entry=f'{axis} {i} verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen.'
            entry_steps=[st['index'] for st in steps if st['ruleCode']=='LINE_ALL_REMAINING_OCCUPIED' and
                         st['cell']['y' if axis=='Zeile' else 'x']==i]
        else:
            a=d['endpoints']['a'];entry=f'A an {a["side"]}/{a["index"]} erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren.'
            w,h=d['grid']['width'],d['grid']['height'];side,index=a['side'],a['index']
            ax=index if side in ('N','S') else 0 if side=='W' else w-1
            ay=index if side in ('E','W') else 0 if side=='N' else h-1
            entry_steps=[st['index'] for st in steps if st['ruleCode']=='ENDPOINT_ENTRY_REQUIRED' and st['cell']==dict(x=ax,y=ay)]
        assert entry_steps,id+' entry not supported by actual trace'
        deep=max(steps,key=lambda st:st['depth']);chain=[];current=deep
        while True:
            chain.append(current['index']);prior=[steps[pr['traceIndex']] for pr in current['premises'] if not pr['isPublicFact']]
            if not prior:break
            current=max(prior,key=lambda st:(st['depth'],-st['index']))
        chain.reverse();assert len(chain)==m['maxDeductionDepth']
        rules={code:sum(st['ruleCode']==code for st in steps) for code in sorted({st['ruleCode'] for st in steps})}
        if '--annotate' in sys.argv:
            note=(f'Authorbewertung {id}, globale Route {route}, Gruppe {g+1}: {BRIEFS[route-1][g]}. '
                  f'Lesbarer öffentlicher Einstieg: {entry} '
                  f'{rules.get("NEIGHBOR_PORT_REQUIRED",0)} konkrete Anschlussfolgen und {rules.get("LINE_TARGET_REACHED",0)} Restzahlabschlüsse verbinden die Belegung mit Geometrie. '
                  f'{f["length"]}/{d["grid"]["width"]*d["grid"]["height"]} Zellen, {f["turns"]} Kurven, {f["zeros"]} Null- und {f["full"]} Voll-Linien ergeben die konkrete räumliche Variante. '
                  f'Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette {m["maxDeductionDepth"]}. '
                  f'Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. '
                  f'Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.')
            d['production']['qualityNote']=note
            path=ROOT/f'Content/Levels/s1/{s:02}/{r:02}/{id}.level-v2.json'
            path.write_text(json.dumps(d,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
        inventory.append(dict(puzzleId=id,authoringStatus='DRAFT_REVIEW_PENDING',globalRoute=route,group=g+1,brief=BRIEFS[route-1][g],
                              grid=d['grid'],endpoints=d['endpoints'],rowCounts=d['rowCounts'],columnCounts=d['columnCounts'],
                              pathLength=f['length'],densityNumerator=f['length'],densityDenominator=d['grid']['width']*d['grid']['height'],
                              turns=f['turns'],zeroLines=f['zeros'],fullLines=f['full'],endpointRelation=f['relation'],endpointDistance=f['distance'],
                              metrics=m,entryDeduction=entry,entryTraceIndices=entry_steps,maxChainTraceIndices=chain,ruleCounts=rules,
                              qualityNote=d['production']['qualityNote'],authorAssessment='TECHNICALLY_FAIR_DEDUCTIVE; independent learning/legibility review and human playtest pending',
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
    pairs=[]
    for i,a in enumerate(docs):
        candidates=[]
        for b in docs:
            if a['puzzleId']==b['puzzleId'] or a['grid']!=b['grid']:continue
            distance=sum(abs(x-y) for x,y in zip(a['rowCounts']+a['columnCounts'],b['rowCounts']+b['columnCounts']))
            candidates.append((distance,b['puzzleId']))
        distance,neighbor=min(candidates)
        pairs.append(dict(puzzleId=a['puzzleId'],nearestSameOrientationCountVector=neighbor,l1CountDistance=distance,
                          assessment='SIMILAR_COUNTS_REVIEW_ENDPOINTS_AND_GEOMETRY' if distance<=2 else 'DIFFERENT_COUNT_VECTOR; motif reuse remains a content-review question'))
    (OUT/'similarity-report.json').write_text(json.dumps(dict(exactPublicPuzzleDuplicatesUnderD4AndABSwap=0,checkedLevels=240,
                     exactSymmetryAlgorithm='D4 coordinate/endpoint/count transformation; endpoint pair unordered; puzzleId excluded',
                     nearestMetric='L1 counts, same grid orientation only; no automatic human novelty threshold',entries=pairs),ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    for route in range(1,21):
        selected=[i for i in inventory if i['globalRoute']==route];s=(route-1)//4+1;r=(route-1)%4+1
        lines=[f'# Author-Leveldesignreview: S1-{s:02}-{r:02} (globale Route {route:02})','',
               'Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.',
               'Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.','',
               '## Routenbogen','', '| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |','|---|---|---|']
        for g,label in enumerate(('1–2','3–5','6–8','9–10','11–12')):
            members=[i for i in selected if i['group']==g+1]
            lines.append(f'| {label} | {BRIEFS[route-1][g]} | '+', '.join(f'{i["puzzleId"]}: {i["metrics"]["maxDeductionDepth"]}' for i in members)+' |')
        lines+=['','Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.',
                '','**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.','']
        for item in selected:
            d=next(d for d in docs if d['puzzleId']==item['puzzleId']);w,h=d['grid']['width'],d['grid']['height'];m=item['metrics'];a,b=d['endpoints'].values()
            lines += [f'## {item["puzzleId"]}','',f'- Input: {w}×{h}; A {a["side"]}/{a["index"]}, B {b["side"]}/{b["index"]}. Zeilen `{d["rowCounts"]}`, Spalten `{d["columnCounts"]}`.',
                      f'- Lösung: {item["pathLength"]} Zellen, {item["turns"]} Kurven; Dichte {item["densityNumerator"]}/{item["densityDenominator"]}. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.',
                      f'- Nachweis: UNIQUE/1, searchNodes={m["searchNodes"]}, requiredGuessDepth={m["requiredGuessDepth"]}, deductionSteps={m["deductionSteps"]}, maxDeductionDepth={m["maxDeductionDepth"]}.',
                      f'- Einstieg: {item["entryDeduction"]} Belegschritte: `{item["entryTraceIndices"]}`.',
                      f'- Eine tatsächliche maximale Prämissenkette: `{item["maxChainTraceIndices"]}` im zugehörigen Rootspurbericht.',
                      f'- Fachliche Begründung: {item["qualityNote"]}',
                      '- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.','', '```text']
            chars={'TRACK_NS':'│','TRACK_EW':'─','TRACK_NE':'└','TRACK_ES':'┌','TRACK_SW':'┐','TRACK_WN':'┘'}
            grid=[['.']*w for _ in range(h)]
            for cell in d['solution']['path']:grid[cell['y']][cell['x']]=chars[cell['track']]
            lines += [f'{y:02} '+''.join(row) for y,row in enumerate(grid)]+['```','']
        (OUT/f'leveldesign-S1-{s:02}-{r:02}.md').write_text('\n'.join(lines).rstrip()+'\n',encoding='utf-8')
    print(f'Persisted 240 individual assessments/entries, 20 route reviews, 20 actual traces; cross-reference gate FAIL with {len(refs)} missing targets; exact symmetric duplicates 0.')
    near=[p for p in pairs if p['l1CountDistance']<=2]
    print('Similar-count pairs for author inspection:',json.dumps(near,ensure_ascii=False))

if __name__=='__main__':main()
