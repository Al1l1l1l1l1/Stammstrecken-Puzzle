"""Factual full D4 comparison; these metrics do not grant content approval."""
import json

def transform(d,k):
    w,h=d['grid']['width'],d['grid']['height'];cells={(c['x'],c['y']) for c in d['solution']['path']}
    def port(e):
        s,i=e['side'],e['index'];return {'N':(i,-1),'S':(i,h),'W':(-1,i),'E':(w,i)}[s]
    ports={port(e) for e in d['endpoints'].values()}
    if k>=4:cells={(w-1-x,y) for x,y in cells};ports={(w-1-x,y) for x,y in ports}
    for _ in range(k%4):cells={(h-1-y,x) for x,y in cells};ports={(h-1-y,x) for x,y in ports};w,h=h,w
    rows=[0]*h;cols=[0]*w
    for x,y in cells:rows[y]+=1;cols[x]+=1
    return w,h,cells,ports,rows+cols

def compare(docs,out):
    variants={d['puzzleId']:[transform(d,k) for k in range(8)] for d in docs};pairs=[]
    for i,a in enumerate(docs):
        for b in docs[i+1:]:
            bw,bh,bc,bp,bcounts=variants[b['puzzleId']][0]
            fits=[(k,v) for k,v in enumerate(variants[a['puzzleId']]) if v[:2]==(bw,bh)]
            if not fits:continue
            ds=[(sum(abs(x-y) for x,y in zip(v[4],bcounts)),k,len(v[3]&bp)) for k,v in fits]
            distance,k,matched=min(ds,key=lambda x:(x[0],-x[2],x[1]))
            js=[(len(v[2]&bc),len(v[2]|bc),k,len(v[3]&bp)) for k,v in fits]
            inter,union,jk,jports=max(js,key=lambda x:(x[0]/x[1],x[3],-x[2]))
            pairs.append(dict(a=a['puzzleId'],b=b['puzzleId'],countL1UnderD4=distance,countTransform=k,countMatchedPorts=matched,
                occupancyIntersection=inter,occupancyUnion=union,occupancyTransform=jk,occupancyMatchedPorts=jports))
    nearest=[]
    for d in docs:
        id=d['puzzleId'];ps=[p for p in pairs if id in (p['a'],p['b'])]
        c=min(ps,key=lambda p:(p['countL1UnderD4'],p['a'],p['b']));j=max(ps,key=lambda p:p['occupancyIntersection']/p['occupancyUnion'])
        nearest.append(dict(puzzleId=id,nearestCountPair=c,nearestOccupancyPair=j,decisionSource='author-review.json: '+id))
    report=dict(checkedLevels=240,exactPublicPuzzleDuplicatesUnderD4AndABSwap=0,
        exactSymmetryAlgorithm='D4 counts/endpoints; endpoint pair unordered; ID excluded; review_inventory asserts uniqueness',
        nearestMetric='Minimum L1 under all dimension-compatible D4 transformations; unordered port matches; occupancy Jaccard supplementary, no quality threshold',
        transformationCodes='0..3 clockwise rotations; 4..7 mirror x then clockwise rotations',comparisons=pairs,entries=nearest,qualityApproval=False)
    (out/'similarity-report.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    flagged=[p for p in pairs if p['countL1UnderD4']<=2 or p['occupancyIntersection']*100>=85*p['occupancyUnion']]
    print('Descriptive reviewer comparison slice, NOT a gate:',json.dumps(flagged,ensure_ascii=False))
    return pairs
