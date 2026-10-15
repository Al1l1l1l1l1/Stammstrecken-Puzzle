# Unabhängige technische Nachprüfung WP-029/WP-030/WP-031

Prüfrolle: separate frisch instanziierte Codex-Prüfrolle `/root/independent_technical_qc`. Tatsächlich unabhängige Ausführung vom Autor; keine Behauptung einer Astra-/Sol-Modellidentität, keine Autoren- oder Reparaturrolle, keine Ergebnisabstimmung mit der anderen QC-Rolle. Stand: 15.10.2026. Dieser Bericht ist technische Evidenz und keine Produkt-, Integrations-, Import- oder Releasefreigabe.

## Urteil

**Begrenzte technische Teilaussagen bestanden; Nachweisnacharbeit erforderlich, kein Gesamt-PASS.** F-01 ist räumlich korrekt behoben, genau eine Revision steigt 2→3, alle 240 öffentlichen Inputs/Lösungen/Proofbytes bleiben erhalten. O-02 ist als Untersuchung methodisch ausreichend abgegrenzt; die historische Ursache bleibt unbewiesen. Der gemeinsame Baum wurde tatsächlich technisch geprüft und bewahrt die Elterninhalte. Seine neun WP-031-Scopefehler sind real.

Zwei Befunde verbleiben: **QC-TECH-01 (P2)**: unprofilierte Working-Tree-Hashes in F01_BINDINGS und WP-031_SHA256 stimmen teilweise nicht mit den kanonischen Repositorybytes überein. **QC-TECH-02 (P2)**: die pauschale Folgerung, ein grundsätzlich neuer externer Architektur-/Verfahrensentscheid sei für jeden weiteren Integrationsabnahmeweg zwingend, geht über die höheren Regeln hinaus. Ein prospektives enges Folgepaket mit eigenem Anker auf dem konkret geprüften Kombinationscommit ist grundsätzlich möglich; Einzelheiten unten. Daraus folgt keinerlei automatisches Bestehen des alten Scopegates.

## Prüfgegenstand und Grenzen

* Liefer-/Dokumentationsfreeze: `93b49a9c9439d8aaab0540a15181b0431751a1fb`, Tree `cba8fc82976186d7da22192e324c48154a80272c`.
* Technischer Lieferelternstand: `d7f69f66e6d58191fa44d64a20523f78ad9a5432`.
* Getrenntes main: `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16`.
* Kombination: `060c1c4b01458a80835094d5bd004be8c36eec54`, Tree `652661ddd524cbfaea1bdeb2f5de2e8d4ea9c101`; Eltern exakt d7f69f6/d90e798; Mergebase c9516bd.
* Originalgesamtabnahme: b1457ba; historische Scopebasen sind davon getrennt.

AGENTS.md und die vollständige Startlesereihenfolge wurden selbst gelesen, anschließend WP-026/028/029/030/031, WP-027/ADR-032 aus main, ADR-021/026/030/031, Paket-, Übergabe- und DoD-Regeln, Scopevalidator/README und einschlägige Teststrategie, Originalbefund und konkrete Implementierungen. Der Originalreport ist Evidenz, keine zusätzliche Spezifikation. Kein erneuter vollständiger 240-Level-Contentreview, kein menschlicher Spieltest, keine erneute WP-028-Modellabnahme behauptet. WP-028 nennt historisch Astra und Sol; diese Prüfung simuliert deren Identität nicht und ersetzt deren damalige dokumentierte Rollen nicht.

Originale/getrackte Dateien und bestehende Gitrefs wurden nicht verändert. Eigene erzeugende Prüfungen liefen in `.utmp/wp031-independent-technical/delivery`, `combined` und `production`. Root und ursprünglicher Preview waren bei der Schlusskontrolle sauber. Eigene Scratchklone enthalten ihre normalen Clone-/Detached-Checkout-Metadaten; keine Änderungen an den ursprünglichen Refs. Ein zusätzliches Anlegen eines lokalen main-Refs im Scratch wurde von der automatischen Freigabeprüfung abgelehnt, nicht ausgeführt und nicht umgangen. Ohne diese unnötige Refmutation konnte die Lieferprüfung regulär erfolgen.

## Tatsächlich selbst ausgeführte Gegenprüfungen

1. Eigener kanonischer WP-031-Liefer-Scope auf 93b49a9, Linux CPython 3.11.13, Node 22.20.0, bestehende Lockumgebung: **PASS, 18 Gruppen**, `worktreeDirty=false`, einschließlich Mutations-/Selbsttests. Ausgabe `wp031-independent-technical/delivery-scope.txt`.
2. Eigener unveränderter Scopevalidator im isolierten C060-Klon: neun identische fachliche Fehler wie CI plus `git:origin-main-unavailable`, weil der lokale Quellklon den main-Ref nur als Remote-Ref besaß und dieser nicht als lokale Branch weitergeklont wurde. Dieser zehnte Fehler ist eine explizite Prüfumgebungsgrenze, keine neue Projektabweichung und kein PASS. Die tatsächlich von mir per API gelesene CI-Ausgabe Job116155055227 enthält genau die neun dokumentierten Fehler, ohne diesen Klonfehler. `combined-scope.txt`, `api-2.json`.
3. F01-Bindungs-/Revisions-/Projektionsvergleich über alle 240 kanonischen Git-Level-/Proofblobs zwischen b1457ba und 93b49a9: öffentliche Projektionen, vollständige Authoringlösungen und sämtliche Proofbytes gleich; nur S1-03-03-03 als Leveldokument geändert, Revision 2→3. Kein zweites Level erhöht seine Revision.
4. Autorentransport auf den eigenen Lieferkopien zweimal ausgeführt, jeweils `git diff --exit-code -- Content/Levels`: **byte-idempotent**, keine Revisionsänderung. `transport.txt`.
5. Neue getrennte .NET-Produktionsassemblies aus den unveränderten **gemeinsamen** Quellen gebaut: Build **0 Warnungen, 0 Fehler**. Unveränderten Produktionsverify selbst im eigenen gemeinsamen Klon ausgeführt: **240 Level/Proofs, je drei Regenerationen vor Strict, Strict-One/Batch/Reverse, 3840 Metamorphosen und 1440 erwartungsgemäß verworfene Mutationen bestanden**. 6317 ms lokaler Verify, sieben 10×10 lokal p95/max 1619 µs. Das ist ergänzende net10.0-Evidenz, kein Unity-/CI-Ersatz. `production.txt`, `build-production.ps1`.
6. Eigene frische Schema-/Node-/Rootprüfung im isolierten gemeinsamen Klon **vollständig bestanden: 240 Levelschemas, 240 Proofschemas, 1200 tatsächliche Nodeprojektionen/-artefakte/-dokumente und 240 Trace-/Inventarbindungen**. `schema-node.txt`; vollständige eigene neue Ausgabe liegt im isolierten Klon unter `Content/Reviews/season-1/schema-node-evidence.json`. Cross-Reference-Gate bleibt FAIL. Keine Vorgänger-PASS-Übertragung.
7. Alle Nichtkonfliktpfade unabhängig über vollständige `git ls-tree -r`-Mengen und Blob-/Modeeinträge der drei Eingabebäume nachgerechnet: **1109 Pfade exakt dem zuständigen Elternstand zugeordnet**, keine zusätzlichen Previewpfade. Drei echte Konfliktpfade sind validate.yml/CURRENT_STATE/WORK_QUEUE; deren Patch und Ergebnisse manuell gelesen. Keine Produktions-/Test-/Content-/historische Manifestmutation in der Kombination.
8. Originale historische vollständige XMLs dekomprimiert, Hashes geprüft, betroffenen Test und alle gemeinsamen Tests über 100 ms verglichen; Quellenblobgleichheit der Domain-/Solver-CS sowie Originaltest/Oracle seit 96697ab selbst geprüft. Vollständige aktuelle Edit-/PlayMode-ZIPs selbst gehasht und XMLs geparst; Benchmarksamples eigenständig sortiert, p95-Index332/Maximum/Fingerprint nachgerechnet. `finish.py`/`finish.json`; die umfassendere `audit.py` zeigt die Hash-Negativbefunde ausdrücklich und endete deshalb zunächst vor ihrem Abschlussreport. Das ist keine Produktionsfehlfunktion.

Negative Umgebungsvorläufe: Windows-Gitclone im Sandboxprozess scheiterte an MSYS-Named-Object-Zugriff; anschließend isoliertes reguläres WSLclone erfolgreich. Ein erster WSL-PATH-Aufruf scheiterte an Leerzeichen im geerbten PATH, danach fester begrenzter Linux-PATH erfolgreich. .NET-Restore in der Sandbox gab Exit1 ohne weiterführende Diagnose, regulär genehmigter isolierter Restore/Build erfolgreich. Die äußere PowerShell-Ausgabe des erfolgreichen Produktionslaufs suchte nach `Set-Location` die Logdatei zunächst relativ im falschen Verzeichnis; der tatsächliche absolute Logpfad wurde anschließend gelesen und enthält den vollständigen PASS. Keiner dieser Vorläufe ist als fachlicher PASS ausgegeben.

## F-01 und Originaltransport

Historische Schichten wurden selbst über genau einen erreichbaren Erst-Add, gemeinsamen WP-/Manifest-Add, historischen Link, einzigen Parent=baseCommit und unveränderte Manifestbytes geprüft:

| WP | Gemeinsamer Anker | Parent/Manifestbasis | Manifest SHA256 |
|---|---|---|---|
| 026 | f9231c73e45ade33450c541e77dbeff5d2eae162 | c9516bdbd5858fd9003d5207ec37d180ccc6ae23 | `981cd37f45c0bb73106d321ad450fbf8ad8e210a508b8a177d25892058ef018c` |
| 028 | 0d17996819ed8ed346cbdf1d25739dd37b46be97 | d97c39d03f59a2dbc34930f14e3cb74ebeb2e118 | `419ac8e075d582fc099fa7def7610fa7fb48e254be7684ec63f03bf13abff4bb` |
| 029 | 78e8dc97eb4cab4bc616cebeca0db7ba2f2882a9 | b1457ba6a61826fe3b64a83323c27a13e9d23080 | `cc4b261337072479470ba4885a5a2006b9c246af44a824f0f02216cbf03725fe` |
| 030 | 438b45e58f359caa4f0b37077c4b8ae19f6ce848 | f8e1dfc8dc1fdc7846d26060e0b354b6e4a32af8 | `8cda10e090f5964b88cfea9bcb0e9b3bf9272aead48f0f73920b40bc4c8b68a3` |
| 031 | f1f0c658e238bb32416d0c082588bbbcd1db076a | 21f8ce5d5f9ed39b4f63c4372b5c4e92b1b1fb2a | `b5432e44b861140ba534b588536b610fd4bcba715aefe3600847ef0254659610` |

Das ist ein historischer Quellen-/Ankerbeleg, kein pauschaler nachträglicher PASS jedes alten Scopes gegen die gesamte heutige Kombination.

Die reale Spaltenfolge ist `[0,2,3,6,5,3]`, Höhe6. Somit ist x0 die Nullspalte, x3 die volle Spalte und x4 mit Count5 rechts davon. Die neue Qualitätsnotiz sagt genau dies; sie behauptet keine Nachbarschaft von x0 und x3. Autorenliteral/-JSON, Levelnotiz und Dokumentprojektion sind inhaltlich konsistent. Die öffentlichen Hashprojektionen nach ADR-021 schließen diese redaktionellen Werte korrekt aus.

Der Base64-Container dekodiert zu **39661 Bytes**, SHA256 `9c0a03fe060e86e69d5041bb3e72e72a55c198029ac3a9990222e16d99e15433`, bytegleich zum ursprünglichen f8e1dfc-Klartextblob einschließlich Schlussleerzeile. Containerhash getrennt: `a8a68428785fb5964d23a4dd700f4ef0d3798c1bdbe999b7bd1652e9d5a14238`. Der Originalbericht wurde damit nicht redigiert. Historischer Whitespace-FAIL bleibt sichtbar.

### QC-TECH-01: kanonische Nachweisbindung reparieren (P2)

`Content/Reviews/season-1/F01_BINDINGS.json` verwendet unter `files` fünf Hashes der physischen Windowsarbeitskopie. Auf den unveränderten Gitblobs bzw. einem frischen LF-Checkout treffen sie nicht zu. Beispiel Inventar: erwarteter Hash `0d730782…` ist tatsächlich die CRLF-Fassung; kanonisch ist `835a960c…`. Bei technical-evidence liegen gemischte ursprüngliche Zeilenenden vor; pauschale spätere CRLF-Rekonstruktion ergibt nicht einmal zuverlässig den alten Hash. Die Datei benennt kein Profil für diese Bytebasis. Derselbe Mangel betrifft acht Einträge in `PROJECT_CONTROL/Integration/WP-031_SHA256.json`, einschließlich Bytecounts.

Korrekte kanonische F01-Dateihashes auf Freeze93:

| Datei im Reviewscope | SHA256 Gitblob/LF |
|---|---|
| level-inventory.json | `835a960cb4ab9acf1fc861c4c5ff4caf766d973663ea8067a8705b75d1fdf59d` |
| technical-evidence.json | `c4aac1763431ea10cc66ee622dcce77dc60dddb828143c6af125d5f7994d0aa6` |
| leveldesign-S1-01-01.md | `c0be736126c628e6243777ad7eb070e34b52beaf3390103cdaff6c2ee8334ea4` |
| leveldesign-S1-03-03.md | `5b9347de9073612bdfcd1f2b5fe77ca70f66be5b80c24e17bb661b0a8d1641a1` |
| leveldesign-S1-05-02.md | `dd4ee69ac11047851ba09bcdcca4adef1a0f46af37eb609c33bdd164a13cba18` |

Die acht zusätzlichen vollständigen Paare mit Bytes stehen unverändert in `wp031-independent-technical/finish.json`, Feld `hashFailures`: CI_DELIVERY_D7, CI_PREVIEW_060, CI_REFERENCES, COMBINED_BINDINGS, COMBINED_PRODUCTION, COMBINED_TECHNICAL, ENVIRONMENT und HISTORY (jeweils WP-031-Präfix/Originalsuffix).

Reproduktion: frischer Checkout mit `core.autocrlf=false`; SHA256 über jede in den beiden Dateien referenzierte Datei rechnen und vergleichen. Bereits `level-inventory.json` reproduziert die Abweichung. Abhilfe: Byteprofil/Quellencommit explizit benennen und kanonische Gitblob-Hashes/Bytecounts getrennt von gegebenenfalls aufzubewahrenden historischen Working-Tree-Hashes persistieren. ZIP-/dekodierte Original-XML-/Originalreporthashes dürfen dadurch nicht normalisiert werden. Keine Puzzle-/Proof-/Regeländerung erforderlich.

Auswirkung: keine neue Rätsellogikabweichung, aber die aktuelle portable Evidenz-/Übergabebindung ist so nicht vollständig reproduzierbar. WP-029 AK-03/05 und WP-031 AK-03/04 dürfen hinsichtlich dieser konkreten Bindung noch nicht vorbehaltlos als erfüllt gelten. Quellen: AGENTS Dokumentierter Stand/Qualitätsnachweis, AI_HANDOVER_RULES §§3/6, DoD Dokumentation/Nachvollziehbarkeit; keine zusätzliche erfundene Produktregel.

## O-02: historischer Timeout

Original FAIL-XML SHA256 `8ef8f9bf3baae10555a238d132a79b0289f2173149a5a9939d94c7390fd82bf1`: 197,533248 s, 180000-ms-NUnit-Timeout. Vergleich desselben 96697ab-HEADs SHA256 `8c8350bcca006d31f56dfae51ed03f09453c3040b39b2af771ec244a4a06130c`: 100,490521 s, PASS. Alle sechs Fallausgaben sind vorhanden, unterbrochener Langpfad jeweils maxStates2. Die fünf ausgewiesenen Überschuss/Kopierverhältnisse liegen unter40%. Das Log beweist keinen zeitaufgelösten Verlauf und kein exakt rekonstruiertes Scheduling.

37 gemeinsame Tests mit PR-Dauer >100 ms: Medianratio1,0345981951. Die drei großen Budgetfälle liegen etwa1,94–1,99. Damit ist weder „alles global doppelt so langsam“ noch „ausufernde Zustandsgrenzsuche“ von diesen Daten gedeckt. Originaltest, Oracle und Domain-/Solverblobs sind unverändert; feste Konstruktionen im betroffenen Test erklären keine seedbedingte Fixtureänderung.

Die Instrumentierung in run_probe.py wurde gelesen: reine Scratchkopie, Messpunkte außerhalb der gemessenen Nachlaufintervalle, unveränderte Assertions/Warmups/25Samples/Fallgrenzen. Die autorseitig tatsächlich gemessene hohe lokale Allokationslast ist als lokale Evidenz zulässig; ich habe diesen instrumentierten Zusatzlauf nicht selbst wiederholt. Seine lokale net10.0-Laufzeit darf weder Unity-Mono-Allokationsrate noch historische GC-/CPU-/Hostursache beweisen. Bericht und vorgeschlagene weitere Phasentelemetrie trennen dies korrekt. Untersuchung im eng beschriebenen Umfang akzeptabel; O-02 bleibt gemäß TEST_STRATEGY §13 offen. Erfolgreiche neue Runs schließen es nicht. Keine Reparaturentscheidung ist aus den vorhandenen Daten bewiesen.

## O-04: höhere Regeln und zulässiger Folgeweg

Die neun Fehler sind acht `scope:out-of-scope` für die Mainimporte plus `scope:product-source` für die Bible. Das Fremdmanifest darf nicht in die WP-031-Allowlist aufgenommen werden: `scope_manifest_errors` verbietet andere Scope-Manifeste ausdrücklich. Production-Scope darf Produktquellen nicht erlauben, Governance-Scope darf Assets/Contentproduktion nicht erlauben. Ändern alter Manifeste, breitere Globs oder falsches Umbuchen nach Governance wäre unzulässig. Diesen FAIL einfach zu ignorieren ist kein zulässiger Abschluss.

### QC-TECH-02: pauschale externe Verfahrenssperre begrenzen (P2)

ADR-026/030 schreiben **pro aufgerufenem Manifest** einen vor fachlichen Änderungen gemeinsamen WP-/Manifestanker und dessen tatsächlichen Parent als Basis vor. Sie schreiben **keinen generellen main-Parent für jedes spätere WP** vor. ADR-030: „Ein späteres Production-Work-Package darf sein eigenes Manifest in einem eigenen gemeinsamen Ankercommit einführen.“ Aktuelle WP-Dateien sind keine nachträgliche Trust-Evidenz. Das lässt die historischen Scopeprüfungen unverändert und erlaubt eine prospektive neue eng abgegrenzte Prüfung. ADR-032 nutzt selbst einen kindlichen Nicht-main-Weg und behandelt den späteren kombinierten Integrationsstand gesondert; README erklärt, dass ein Mainintegrationsstand keinem einzelnen WP-Diff entspricht.

C060 ist kein heimlich eingeschleuster Produktionsstand: die Zusammenstellung im isolierten Scratch war bereits WP-031 Scope3/4 ausdrücklich erlaubt und wurde nun tatsächlich technisch und nach Herkunft geprüft. Ein neuer **ausdrücklich beauftragter Integrationsvorbereitungs-/Abnahmeauftrag auf exakt C060 als Parent** kann deshalb nach ADR-030 sein eigenes prospektives Delta verankern. Er muss vor neuen Änderungen in enger Allowlist/AK festlegen: beide eingefrorenen Eltern/Tree, unveränderte historische Manifeste/Einzelanker, vollständige Blobprovenienz, präzise manuelle Konfliktabnahme und gemeinsame echte technische CI; nur dann darf sein eigener enger Workflow-/Dokumentationsdelta scopegeprüft werden. Dies ist kein rückwirkender PASS für WP-031s Kombination. Die neun Altfehler müssen ausdrücklich als Ergebnis der alten Prüffrage erhalten bleiben. Neue Basen dürfen nicht unbelegte Herkunft oder unautorisierte Änderungen aus dem Review verschwinden lassen.

Ein Folgepaket, das nur auf C060 basiert und anschließend `diff=0` als Beweis der ganzen Importlegalität ausgäbe, wäre eine Kaschierung. Ein Folgepaket, das zusätzlich die beschriebenen unveränderten vorigen Lieferungen/Imports/Konflikte als zwingende Abnahmeeingänge tatsächlich überprüft und den eigenen neuen Delta separat prüft, ist von den gelesenen höheren Regeln grundsätzlich gedeckt. Dafür ist **keine allgemeine neue Architektur-/Validatorregel notwendig**. Das konkrete neue WP und dessen autorisierter Zweck müssen dennoch vorliegen (WORK_PACKAGE_RULES); diese Prüferrolle schreibt es nicht selbst. Die Koordination hat während der Prüfung die ausdrückliche bestehende Nutzerautorisierung für eine enge Folgeauftragverankerung bestätigt. Ein selbst formulierter WP-031-Verbots-/Statussatz schafft darüber hinaus kein neues externes Zustimmungserfordernis. Ein tatsächlicher Merge der Preview-Unter-PR oder nach main bleibt ein getrenntes, hier nicht ausgeführtes Freigabethema.

## Aktuelle echte CI und Rohartefakte

API selbst gelesen: Lieferung Architecture38654476627/Job116160864613 SUCCESS. Unity38654476651: EditMode und PlayMode sowie Android erfolgreich; iOS Job116174588123 bei Abfrage noch in_progress. Preview Unity38652697933: alle zehn Jobs einschließlich Android/iOS SUCCESS. Preview Architecture38652697909/Job116155055227 bleibt FAIL9. Keine Retries/Merges ausgelöst.

Selbst gehashte/gelesene ZIPs/XMLs:

| Stand/Test | ZIP SHA256 | XML SHA256 | Ergebnis |
|---|---|---|---|
| 93 EditMode | `f37652896b68f2b0d11a503d30cfb348241f336a1d43a0043fdff13cb67e1759` | `7f4386be0175363ab01e16e270a1493d91776f7f758893aae3ae89c0a57cb9f4` | 455/455 |
| 93 PlayMode | `fab2d39ac0cec0cbbb67b6bd5fe37796e9dc2e5a3dbd9d0c3eaa2a7c2d7813d5` | `a64cebadb8e451165ae0ef58780da426f74636c543a6c86d355b6f349f9287d9` | 27/27 |
| C060 EditMode | `f7c47e8024f26aa828a25c4055cfe03f8b22cb676b35ccdba48262e868ddd514` | `57a691e29d55ebed913bc6691178284821b0583f9e4cd06d672edeb1551eb9e0` | 455/455 |
| C060 PlayMode | `5998642d3a88c681a75e1c45ffbce5b2b32409464e38c812c9069ec90cbdffb6` | `f685300ecbfa0f0eb8abc709ebf92cdc58dff0d6a4409d60e71b85dfcc446f89` | 27/27 |

Alle ohne Failed/Skipped. Die Lieferartefakt-Metadaten und Digest-/Commitzuordnung wurden über die API zusätzlich selbst gelesen. In beiden EditMode-XMLs genau eine WP028-Zeile, 350 Samples, Index332. Eigene Fingerprint-Gegenrechnung über rohe Git/LF-Bytes ergibt `360f3309ff958ab6871f963354ea5ec691a096b0aec68bf19165a1ea43cea6bf`. Lieferung p95/max40,209/91,1282ms, Vollkorpus2690,417ms; C06043,8702/103,8109ms und2147,5077ms. Diese Werte bestehen ADR-032, keine Übertragung historischer Werte und keine Spielerzeitkalibrierung. Android-/iOS-Buildoutput wurde hier über echte Job-/Stepzustände beurteilt, nicht als physischer Gerätetest.

## Akzeptanz-/DoD-Schlussgrenze

WP-029 F01-Orts-/Revisions-/Idempotenz-/Inputaussagen technisch bestätigt; portable Nachweisdateibindung wegen QC-TECH-01 nachzuarbeiten. WP-030 Untersuchungsergebnis bestätigt im begrenzten Untersuchungsumfang; Originalursache und notwendige spätere Zuverlässigkeitsbehandlung bleiben offen. WP-031 Kombination/Elternprovenienz/echte gemeinsame Produktionsprüfungen bestätigt; ursprünglicher kombinierter Scope bleibt FAIL, CI-Liefer-iOS zum Abfragezeitpunkt noch offen. Gezielte unabhängige technische Rolle ist jetzt tatsächlich ausgeführt, keine Selbstabnahme.

Keine Freigabe von Humanreview/Spieltest, 1440 fehlenden Referenzzielen, Cross-Reference-FAIL, UNCALIBRATED/null-Sternsekunden, kreativen Assets, Import, Runtime, Produkt, Release, Gerät oder Store. Kein Gesamt-WP-026-/Gesamt-DoD-PASS. Empfohlener nächster Schritt: eng beauftragte kanonische Bindungskorrektur und Statuspräzisierung, anschließend prospektiv verankertes Integrationsfolgepaket mit unveränderten historischen Quellen, dann gezielte unabhängige Nachprüfung genau seiner tatsächlichen neuen Freezes.
