"""Lossless report transport and isolated, explicitly unapproved merge preview."""
from pathlib import Path
import argparse,base64,hashlib,json,subprocess,textwrap
ROOT=Path(__file__).resolve().parents[2]
MAIN='d90e798dd9b26d3f152f3b0c2d1d52cae641eb16'
BASE='c9516bdbd5858fd9003d5207ec37d180ccc6ae23'
ORIGINAL='9c0a03fe060e86e69d5041bb3e72e72a55c198029ac3a9990222e16d99e15433'
REPORT='Content/Reviews/season-1/WP-026_Gesamtabnahme_b1457ba.txt'
CONFLICTS=['.github/workflows/validate.yml','PROJECT_CONTROL/CURRENT_STATE.md','PROJECT_CONTROL/WORK_QUEUE.md']
OUT=ROOT/'PROJECT_CONTROL/Integration'
def git(*args,cwd=ROOT,check=True):return subprocess.run(['git',*args],cwd=cwd,capture_output=True,check=check)
def blob(rev,p,cwd=ROOT):
 r=git('show',rev+':'+p,cwd=cwd,check=False);return r.stdout if r.returncode==0 else None
def write(p,s):p.parent.mkdir(parents=True,exist_ok=True);p.write_text(s,encoding='utf-8',newline='\n')
def decode(p):
 data=p.read_text(encoding='utf-8');assert data.startswith('STP_ORIGINAL_REPORT_BASE64_V1\n')
 raw=base64.b64decode(data.split('\nbase64:\n',1)[1],validate=False);assert hashlib.sha256(raw).hexdigest()==ORIGINAL;return raw
def transport():
 raw=blob('f8e1dfc',REPORT);assert hashlib.sha256(raw).hexdigest()==ORIGINAL
 p=ROOT/REPORT
 container='STP_ORIGINAL_REPORT_BASE64_V1\noriginalName=WP-026_Gesamtabnahme_b1457ba.txt\noriginalSha256='+ORIGINAL+'\nencoding=base64; decode payload to recover every original UTF-8 byte including final blank line\nbase64:\n'+'\n'.join(textwrap.wrap(base64.b64encode(raw).decode(),76))+'\n'
 write(p,container);assert decode(p)==raw
 jpath=ROOT/'Content/Reviews/season-1/F01_BINDINGS.json';j=json.loads(jpath.read_text(encoding='utf-8'));j['files'][Path(REPORT).name]=hashlib.sha256(p.read_bytes()).hexdigest();j['originalReportTransport']=dict(encoding='STP_ORIGINAL_REPORT_BASE64_V1',decodedSha256=ORIGINAL,originalByteCount=len(raw),fullByteEqualityVerified=True,reason='preserve original final blank line without violating diff --check; no original text trimmed');write(jpath,json.dumps(j,indent=2)+'\n')
 note='''## Aktueller Originaltransport / WP-031 – 15.10.2026

Die Gesamtabnahme ist am verlinkten TXT-Pfad als **STP_ORIGINAL_REPORT_BASE64_V1** verlustfrei gespeichert. Original-SHA256 9c0a03fe060e86e69d5041bb3e72e72a55c198029ac3a9990222e16d99e15433 bezeichnet die vollständig dekodierten Originalbytes, einschließlich der zusätzlichen Schlussleerzeile. Der Transportcontainer hat einen eigenen Dateihash in F01_BINDINGS.json. `tools/wp031-integration/prepare.py decode --output <Scratchdatei>` stellt die unveränderte Originaldatei wieder her und prüft ihren Hash. Kein Originaltext wurde getrimmt oder redigiert. Der unveränderte Klartext und das isolierte Archiv liegen weiter in der ursprünglichen externen Auditablage.

Eigener erster Nacharbeitsstand f8e1dfc: Architecture Run38651701033 / Job116151786197 scheiterte tatsächlich an `scope:diff-check`: zusätzliche Schlussleerzeile des unveränderten Originalberichts. Lokaler vorheriger Scope lief vor diesem Commit mit untracked Originaldatei; dessen Whitespaceprüfung deckte die erst eingecheckte Schlussleerzeile nicht ab. Der neue verlustfreie Transport behebt die eigene Persistierungsgrenze, ohne Originalbytes oder Qualitätsanforderung zu ändern. Neuer finaler CI-Stand muss separat bestehen. O-02 bleibt ursächlich offen; aktuelle unabhängige gezielte Prüfung und Gesamt-DoD bleiben offen.

'''
 for n in ['ACCEPTANCE_FOLLOWUP.md','HANDOVER.md','REPAIR_HANDOVER.md','REVIEW_RESOLUTION.md']:
  q=ROOT/'Content/Reviews/season-1'/n;write(q,note+q.read_text(encoding='utf-8'))
 p=ROOT/'.github/workflows/validate.yml';write(p,p.read_text(encoding='utf-8').replace('WP-030.production.scope.json','WP-031.production.scope.json'))
 for name in ['CURRENT_STATE.md','WORK_QUEUE.md']:
  p=ROOT/'PROJECT_CONTROL'/name;s=p.read_text(encoding='utf-8')
  if name=='CURRENT_STATE.md':
   lines=s.splitlines()
   for i,l in enumerate(lines):
    if l.startswith('| Aktuell zu prüfender Schritt |'):lines[i]='| Aktuell zu prüfender Schritt | WP-029 F-01/Status und WP-030-Untersuchung geliefert; WP-031 verlustfreier Originaltransport und isolierte gemeinsame Integrationsprobe. Keine Gesamtfreigabe. |'
    if l.startswith('| Nächster vorgesehener Schritt |'):lines[i]='| Nächster vorgesehener Schritt | Tatsächlichen gemeinsamen WP-026/main-Preview prüfen; zulässigen Integrationsscope und diagnostischen O-02-Folgeauftrag entscheiden; gezielte unabhängige Nachprüfung. |'
   s='\n'.join(lines)+'\n'
  write(p,s+'\n## WP-031: tatsächliche Integrationsvorbereitung – 15.10.2026\n\nOriginalbericht ohne Inhaltsänderung vollständig verlustfrei Base64-kodiert; Dekodierung/Originalhash/Bytegleichheit bestanden. Eigener f8e1dfc-Whitespace-FAIL bleibt als Historie dokumentiert. Main d90e798 und WP-026-Lieferstand werden ausschließlich in isoliertem Git-Preview konfliktaufgelöst und tatsächlich geprüft. Die Einzelmanifestgrenze für fremde Manifest-/Produktquellenimporte bleibt Pflichtgate; keine Regeländerung und keine Übernahme nach main. Vollständiger neuer Stand in PROJECT_CONTROL/Integration/WP-031_HANDOVER.md nach Ausführung.\n')
 for wp in ['WP-029_Abnahme-Nachfuehrung-WP026.md','WP-030_HardBudget-Timeout-Untersuchung.md']:
  p=ROOT/'WORK_PACKAGES'/wp;s=p.read_text(encoding='utf-8');a=s.index('Status:');b=s.index('\n',a)
  status='Status: Eigene Umsetzung/Prüfungen geliefert, unabhängige gezielte Nachprüfung offen. WP-029: F-01 korrigiert; Originaltransport-Whitespacegrenze unter WP-031 verlustfrei nachgeführt.' if '029' in wp else 'Status: Untersuchung mit Original-/Gegenproben geliefert; historische Ursache nicht bewiesen, O-02 bleibt offen. Enger diagnostischer Folgeauftrag zur Entscheidung; keine Reparatur oder Selbstfreigabe.'
  write(p,s[:a]+status+s[b:])
 print('PASS original full-byte equality:',ORIGINAL,len(raw),'bytes; transport sha256',hashlib.sha256((ROOT/REPORT).read_bytes()).hexdigest())
def preview():
 OUT.mkdir(parents=True,exist_ok=True);target=ROOT/'.utmp/wp031-preview';assert not target.exists(),'refuse to overwrite an existing preview'
 assert not git('status','--porcelain').stdout.strip(),'freeze delivery before preview'
 source=git('rev-parse','HEAD').stdout.decode().strip()
 remote=git('ls-remote','origin','refs/heads/main').stdout.decode().split()[0];assert remote==MAIN,'main changed: audit new source before proceeding'
 git('clone','--no-hardlinks',str(ROOT),str(target));git('remote','set-url','origin','https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle.git',cwd=target)
 git('switch','-c','codex/wp-031-integration-preview',source,cwd=target)
 merged=git('merge','--no-commit','--no-ff',MAIN,cwd=target,check=False)
 actual=git('diff','--name-only','--diff-filter=U',cwd=target).stdout.decode().splitlines();assert sorted(actual)==CONFLICTS,(actual,merged.stdout,merged.stderr)
 workflow=blob(MAIN,CONFLICTS[0],target).decode().replace('STP_SCOPE: governance','STP_SCOPE: production').replace('WP-027.governance.scope.json','WP-031.production.scope.json');write(target/CONFLICTS[0],workflow)
 ours=blob(source,CONFLICTS[1],target).decode();main=blob(MAIN,CONFLICTS[1],target).decode();lines=ours.splitlines();mainrows={l.split('|')[1].strip():l for l in main.splitlines() if l.startswith('| ')}
 for i,l in enumerate(lines):
  if l.startswith('| Projektstatus |'):lines[i]=mainrows['Projektstatus'].replace('WP-027 klärt zwei Content-Bible-Auslegungen und den getrennten CI-Leistungsweg','WP-027 klärt zwei Content-Bible-Auslegungen; WP-028 hat den getrennten CI-Leistungsnachweis im offenen Lieferzweig erbracht')
  if l.startswith('| Architekturentscheidungen |'):lines[i]=mainrows['Architekturentscheidungen']
  if l.startswith('| Letzter verifizierter Nachweis |'):lines[i]='| Letzter verifizierter Nachweis | Gesamtabnahme b1457ba durchgeführt, kein Gesamt-PASS; F-01 und neue eigene Nachweise unter WP-029, Timeoutuntersuchung unter WP-030. Dieser kombinierte Preview wird neu geprüft, keine Übertragung alter PASS-Aussagen. |'
 insert=next(i for i,l in enumerate(lines) if l.startswith('| CI WP-025 |'))
 row=mainrows['Scope-Vertrauensanker WP-027'].split(' WP-028 besitzt bewusst')[0]+' WP-028 besitzt seinen eigenen Anker 0d17996 auf d97c39d; unverändert. |'
 lines[insert:insert]=[row,mainrows['CI WP-027']]
 s='\n'.join(lines)+'\n';s+='\n## Erhaltener WP-027-Quellen-/Integrationsnachweis\n\nWP-027 ist auf main d90e798 abgeschlossen und integriert (PR #23 / 7d95b59; Closeout PR #24). Content-Bible §§4.1/8.1, ADR-032 und WP-028_EXECUTION_BRIEF werden byteunverändert aus main übernommen. WP-028 ist inzwischen ausschließlich im WP-026-Lieferzweig integriert; dessen Nachweis und die neue Gesamtabnahme sind oben aktuell eingeordnet. Keine Produkt-/Architekturentscheidung durch die Konfliktauflösung.\n';write(target/CONFLICTS[1],s)
 ours=blob(source,CONFLICTS[2],target).decode();main=blob(MAIN,CONFLICTS[2],target).decode();lines=main.splitlines()
 for i,l in enumerate(lines):
  if l.startswith('| 6 | WP-026'):lines[i]='| 6 | WP-026 – Season-1-logischer Rätselbestand | **Gesamtabnahme durchgeführt, weiterhin offen** | b1457ba unter WP-027 geprüft; F-01 eng korrigiert. Menschlicher Spieltest, historische Timeoutursache, zulässiger Integrationsscope/QC, 1440 Ziele, Zeitklasse und Folgegates offen. |'
  if l.startswith('| 8 | WP-028'):lines[i]='| 8 | WP-028 – CI-Referenz-Leistungsnachweis Season-1-Korpus | **Technisch erbracht, ausschließlich im offenen WP-026 integriert** | PR #25: b449ab7 unabhängig geprüft, Merge b1457ba nur in WP-026; Folge-CI und reale 350 Samples/240 Proofgates erfüllt. Keine Gesamt-/Import-/Releasefreigabe. |'
 footer=ours[ours.index('## Aktuelle technische Nacharbeit'):];write(target/CONFLICTS[2],'\n'.join(lines)+'\n\n'+footer)
 git('add','--',*CONFLICTS,cwd=target);assert not git('diff','--name-only','--diff-filter=U',cwd=target).stdout
 git('-c','core.safecrlf=false','commit','-m','merge: isolated WP-026/main preview; scope approval explicitly unresolved',cwd=target)
 head=git('rev-parse','HEAD',cwd=target).stdout.decode().strip();tree=git('rev-parse','HEAD^{tree}',cwd=target).stdout.decode().strip()
 # Gather the complete union of the three input trees explicitly.
 names=set()
 for rev in [source,MAIN,BASE]:names.update(git('ls-tree','-r','--name-only',rev,cwd=target).stdout.decode().splitlines())
 verified=0
 for p in sorted(names):
  if p in CONFLICTS:continue
  old=blob(BASE,p,target);a=blob(source,p,target);b=blob(MAIN,p,target);c=blob(head,p,target)
  if a==b:expected=a
  elif a==old:expected=b
  elif b==old:expected=a
  else:raise AssertionError('unaccounted overlapping change '+p)
  assert c==expected,p;verified+=1
 patch=git('diff',source,head,'--',*CONFLICTS,cwd=target).stdout; (OUT/'WP-031_CONFLICT_RESOLUTION.patch').write_bytes(patch)
 result=dict(deliverySource=source,mainSource=MAIN,mergeBase=BASE,previewBranch='codex/wp-031-integration-preview',previewCommit=head,previewTree=tree,parents=git('show','-s','--format=%P',head,cwd=target).stdout.decode().strip().split(),resolvedConflicts=CONFLICTS,nonConflictPathsVerified=verified,originalDecodedSha256=hashlib.sha256(decode(target/REPORT)).hexdigest(),status='CONTENT_COMBINATION_CREATED; checks and scope decision pending; no main merge',scratchCheckout=str(target))
 write(OUT/'WP-031_PREVIEW.json',json.dumps(result,indent=2)+'\n');print(json.dumps(result,indent=2))
def main():
 ap=argparse.ArgumentParser();ap.add_argument('mode',choices=['transport','decode','preview']);ap.add_argument('--output',type=Path);a=ap.parse_args()
 if a.mode=='transport':transport()
 elif a.mode=='preview':preview()
 else:
  raw=decode(ROOT/REPORT)
  if a.output:a.output.write_bytes(raw)
  print('PASS decoded original SHA256',hashlib.sha256(raw).hexdigest(),len(raw),'bytes')
if __name__=='__main__':main()
