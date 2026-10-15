"""Prepare a reviewable selection from ignored candidate pools, never a quality approval.

Uses actual solver metrics and the existing Content-Bible's per-route briefs.
The resulting scratch selection must be read/reviewed by the level author before
materialization. The final canonical sources, not this scratch file, are authority.
"""
import copy
import json
import math
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
POOL = ROOT / '.utmp/wp026-pools'
BRIEFS = [
    ['A/B und Randzahlen', 'Nullen und Ausschluss', 'Volle Linien', 'Verschiedene A/B-Kanten', 'Einstiegsmuster kombinieren'],
    ['Hohe Randzahlen', 'Belegung als optionale Notiz', 'Gekreuzte Belegungs-/Ausschlusslogik', 'Nahe Endpunkte', 'Längere eindeutige Kette'],
    ['Randzahlenanker', 'Angrenzende Endpunktkanten', 'Belegung und Geometrie', 'Dichtekontrast', 'Sichere Ecken und Kette'],
    ['Einstiegsmuster', 'Hohe und niedrige Randzahlen', 'Belegungsbereiche verbinden', 'Gegenüberliegende Endpunkte', 'Abschnittskompetenz'],
    ['Größenübergang', 'Volle und leere Linien kreuzen', 'Mehrere Achsen verbinden', 'Andere Eckzwänge', 'Mittlere Dichte'],
    ['Sichere Belegung', 'Dichtere Struktur', 'Mehrere Achsen', 'Lange Verbindung bei begrenzter Dichte', 'Kompakte Verdichtung'],
    ['Bekannte Sicherheiten', 'Mittlere Randzahlen', 'Teilwege durch Folgeausschlüsse', 'Asymmetrie und Kurven', 'Anspruchsvolle 6×5-Kette'],
    ['Größere Informationsmenge', 'Weit getrennte Bereiche', 'Kreuzschlussketten', 'Hohe Dichte', 'Lange aufgebaute Verbindung'],
    ['Deutliche Randzahlenanker', 'Ungünstigere A/B-Lage', 'Lokale Ketten wirken entfernt', 'Weniger Extremwerte', 'Faire 6×6-Höhepunkte'],
    ['Strukturierte Anfangsanker', 'Zahlenverteilung variieren', 'Teilräume zusammenführen', 'Lange asymmetrische Verbindung', 'Klarer Kompetenzabruf'],
    ['Vertraute Linien', 'Teilpfade durch Counts trennen', 'Belegung, Ausschluss und Geometrie', 'Entfernte A/B und Kurven', 'Große 7×6-Abschlüsse'],
    ['Hohe und niedrige Counts', 'Abhängige Ketten', 'Mittlere Zahlen, wenige Extreme', 'Schwierige Endpunktkanten', 'Strategischer Abschnittsabschluss'],
    ['Kontrollierter 7×7-Übergang', 'Verstreute Belegungsbereiche', 'Entfernte Bereiche verbinden', 'Hohe Dichte', 'Lange deduktive Strecke'],
    ['Deutlicher Anfang', 'Mittlere Randzahlen', 'Kombination statt einzelner Voll-/Nullzahl', 'Ungleichmäßige Dichte und Entfernung', 'Lange Schlusskette'],
    ['Bekannte Muster im größeren Feld', 'Sichere Ausschlüsse', 'Parallele Teilketten', 'Geometrie aus Belegung', 'Anspruchsvolle 8×8-Puzzles'],
    ['Klare Startanker', 'Alternativen schrittweise ausschließen', 'Hohe Verzahnung', 'Knappe Counts und andere Endpunkte', 'Strategischer Abschnittsabschluss'],
    ['Zugängliche Extremwerte', 'Entfernte Belegungsbereiche', 'Ausschluss über das Raster', 'Lange Pfade und Kurven', 'Faire große Abschlüsse'],
    ['Ruhige klare Belegung', 'Mehrere mittlere Hinweise', 'Lokale und globale Folgerungen', 'Hohe Dichte und Geometrie', '9×9-Meisterschaft'],
    ['Bekannte Startmuster', 'Mehrere lange Ketten', 'Wenige direkte Extreme', 'Problematische Kanten und Kurven', 'Große prüfbare Höhepunkte'],
    ['Souveräner Wiedereinstieg', 'Komplexe Belegung', 'Mehrere tiefe 10×10-Ketten', 'Finale räumliche Variation', 'Finale 10×10-Meldungen'],
]

def group(position):
    return 0 if position <= 2 else 1 if position <= 5 else 2 if position <= 8 else 3 if position <= 10 else 4

def dimensions(route, position):
    if route == 1: return 4, 4
    if route <= 4: return 4, 5
    if route <= 6: return 5, 5
    if route == 7: return (5, 5) if position <= 5 else (6, 5)
    if route == 8: return 6, 5
    if route <= 10: return 6, 6
    if route == 11: return (6, 6) if position <= 5 else (7, 6)
    if route == 12: return 7, 6
    if route <= 14: return 7, 7
    if route == 15: return (7, 7) if position <= 5 else (8, 8)
    if route <= 17: return 8, 8
    if route == 18: return (8, 8) if position <= 5 else (9, 9)
    if route == 19: return 9, 9
    return (9, 9) if position <= 5 else (10, 10)

def features(doc):
    w, h = doc['grid']['width'], doc['grid']['height']
    counts = doc['rowCounts'] + doc['columnCounts']
    a, b = doc['endpoints']['a'], doc['endpoints']['b']
    def cell(ep):
        s, i = ep['side'], ep['index']
        return (i, 0) if s == 'N' else (i, h-1) if s == 'S' else (0, i) if s == 'W' else (w-1, i)
    ac, bc = cell(a), cell(b)
    zeros = counts.count(0)
    full = sum(v == w for v in doc['rowCounts']) + sum(v == h for v in doc['columnCounts'])
    turns = sum(c['track'] not in ('TRACK_NS', 'TRACK_EW') for c in doc['solution']['path'])
    return dict(zeros=zeros, full=full, extreme=zeros+full,
                activeWidth=sum(v>0 for v in doc['columnCounts']),activeHeight=sum(v>0 for v in doc['rowCounts']),
                density=len(doc['solution']['path'])/(w*h), length=len(doc['solution']['path']),
                relation='same' if a['side'] == b['side'] else 'opposite' if abs('NESW'.index(a['side'])-'NESW'.index(b['side'])) == 2 else 'adjacent',
                distance=abs(ac[0]-bc[0])+abs(ac[1]-bc[1]), turns=turns,
                signature=tuple(counts), maxcount=max(counts), spread=max(counts)-min(counts))

def symmetry_key(doc):
    w0,h0 = doc['grid']['width'],doc['grid']['height']
    coords=[(c['x'],c['y']) for c in doc['solution']['path']]
    keys=[]
    for mirror in (False,True):
        for rotation in range(4):
            w,h=w0,h0
            path=[(w-1-x,y) if mirror else (x,y) for x,y in coords]
            ends=[dict(e) for e in doc['endpoints'].values()]
            if mirror:
                for e in ends:
                    if e['side'] in ('N','S'): e['index']=w-1-e['index']
                    else: e['side']='W' if e['side']=='E' else 'E'
            for _ in range(rotation):
                path=[(h-1-y,x) for x,y in path]
                for e in ends:
                    s=e['side']; e['side']={'N':'E','E':'S','S':'W','W':'N'}[s]
                    if s in ('E','W'): e['index']=h-1-e['index']
                w,h=h,w
            rows=[0]*h;cols=[0]*w
            for x,y in path:rows[y]+=1;cols[x]+=1
            keys.append(json.dumps([w,h,sorted((e['side'],e['index']) for e in ends),rows,cols],separators=(',',':')))
    return min(keys)

def fits(route,p,f):
    g=group(p)
    if f['relation']=='same' and not ((route,p) in ((2,9),(2,10)) or (route>=9 and g==3)):return False
    if route==1 and g==1 and f['zeros']==0:return False
    if route==1 and g==2 and f['full']==0:return False
    if route==2 and g in (0,1) and f['maxcount']<4:return False
    if route==2 and g==3 and f['distance']>3:return False
    if route==3 and g==1 and f['relation']!='adjacent':return False
    if route==4 and g==3 and f['relation']!='opposite':return False
    if route==5 and g==1 and (f['zeros']==0 or f['full']==0):return False
    if route==6 and g==1 and f['density']<.55:return False
    if route in (7,12,14,18,19) and g in (1,2) and f['extreme']>1:return False
    if route==14 and g in (1,2) and f['extreme']!=0:return False
    if route==8 and g==1 and f['distance']<6:return False
    if route in (8,13,18) and g==3 and f['density']<.5:return False
    if route in (11,14,17) and g==3 and f['distance']<5:return False
    if route in (9,16) and g==3 and f['extreme']>2:return False
    if route==17 and g==0 and f['extreme']==0:return False
    if route>=9 and g>=1 and f['density']<.25 and not (route==20 and g==4):return False
    # The advanced briefs ask for separated spatial reasoning, not a narrow
    # two-column corridor embedded into a nominally larger board.
    if route>=9 and g in (1,2,4):
        w,h=dimensions(route,p)
        if f['activeWidth']<math.ceil(w*.7) or f['activeHeight']<math.ceil(h*.7):return False
    return True

def main():
    pools={}
    for filename in sorted(POOL.glob('*.json')):
        for candidate in json.loads(filename.read_text(encoding='utf-8')):
            doc=candidate['document'];dims=(doc['grid']['width'],doc['grid']['height'])
            candidate['pool']=filename.name;candidate['features']=features(doc);candidate['symmetry']=symmetry_key(doc)
            pools.setdefault(dims,[]).append(candidate)
    chosen=[];used=set();percentiles=[.18,.25,.4,.45,.5,.6,.65,.7,.6,.7,.88,.98]
    for route in range(1,21):
        section=(route-1)//4+1;local=(route-1)%4+1;route_candidates=[]
        for p in range(1,13):
            candidates=[c for c in pools[dimensions(route,p)] if c['symmetry'] not in used and fits(route,p,c['features'])]
            # Author selection preference, not a new product/validator threshold:
            # remove the near-identical count patterns found in the own review.
            candidates=[c for c in candidates if all(
                dimensions(route,p)!=(old['document']['grid']['width'],old['document']['grid']['height']) or
                sum(abs(a-b) for a,b in zip(c['features']['signature'],old['features']['signature']))>2
                for old in chosen)]
            if not candidates:raise ValueError(f'No candidate fits route={route} position={p}. Needs author intervention, no relaxed rule.')
            if route==8 and group(p)==4:candidates=[c for c in candidates if c['features']['length']>=18]
            if route==20 and group(p)==2:candidates=[c for c in candidates if c['depth']>=20 and c['features']['extreme']<=2]
            if not candidates:raise ValueError(f'Final author selection lacks suitable route={route} position={p}')
            depths=sorted(c['depth'] for c in candidates);q=1 if route>=9 and group(p)==4 else percentiles[p-1]
            target=depths[min(len(depths)-1,round((len(depths)-1)*q))]
            def score(c):
                f=c['features'];value=-abs(c['depth']-target)*2
                # Count/endpoint/geometry variety is assessed independently of depth.
                value+=min((sum(abs(a-b) for a,b in zip(f['signature'],old['features']['signature'])) for old in route_candidates if len(f['signature'])==len(old['features']['signature'])),default=5)*.4
                if p>2:value+=f['turns']*.05
                value+=min(f['distance'],6)*.08
                if group(p)==4:value+=f['length']*.035
                return value
            best=max(candidates,key=lambda c:(score(c),-c['candidate']))
            used.add(best['symmetry']);route_candidates.append(best);copydoc=copy.deepcopy(best['document'])
            id=f'S1-{section:02}-{local:02}-{p:02}';ns=f'level.s1.{section:02}.{local:02}.{p:02}'
            copydoc['puzzleId']=id;copydoc['content'].update(networkSection=section,route=local,position=p,titleKey=ns+'.title',statusKey=ns+'.status')
            copydoc['production']['entryDeductionKey']=ns+'.entry';copydoc['production']['chainDepth']=1
            copydoc['completion']=dict(mapSegmentId=f'draft-s1-{section:02}-{local:02}-{p:02}-map',trainMomentId=f'draft-s1-{section:02}-{local:02}-{p:02}-train',resultTextKey=ns+'.result')
            copydoc['proofRef']['artifactId']=f'proofs/{id}/solver-v2.proof-v1.json'
            focus=BRIEFS[route-1][group(p)];f=best['features'];w,h=dimensions(route,p)
            copydoc['production']['qualityNote']='Scratch candidate: individual leveldesign review pending.'
            # Candidate scratch metadata is not an author judgment. Explicit input-bound
            # review must assign focus, entry and expected chain before acceptance.
            chosen.append(dict(document=copydoc,pool=best['pool'],candidate=best['candidate'],brief=focus,metrics=best['metrics'],features=f))
        print(f'Route {route:02}: '+', '.join(f'{c["depth"]}/{c["features"]["length"]}/{c["features"]["relation"]}' for c in route_candidates))
    target=ROOT/'.utmp/wp026-selection.json';target.write_text(json.dumps(chosen,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    print(f'Prepared {len(chosen)} candidates for AUTHOR review: {target}')

if __name__=='__main__':main()
