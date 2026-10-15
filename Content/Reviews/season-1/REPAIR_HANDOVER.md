## Aktueller Originaltransport / WP-031 – 15.10.2026

Die Gesamtabnahme ist am verlinkten TXT-Pfad als **STP_ORIGINAL_REPORT_BASE64_V1** verlustfrei gespeichert. Original-SHA256 9c0a03fe060e86e69d5041bb3e72e72a55c198029ac3a9990222e16d99e15433 bezeichnet die vollständig dekodierten Originalbytes, einschließlich der zusätzlichen Schlussleerzeile. Der Transportcontainer hat einen eigenen Dateihash in F01_BINDINGS.json. `tools/wp031-integration/prepare.py decode --output <Scratchdatei>` stellt die unveränderte Originaldatei wieder her und prüft ihren Hash. Kein Originaltext wurde getrimmt oder redigiert. Der unveränderte Klartext und das isolierte Archiv liegen weiter in der ursprünglichen externen Auditablage.

Eigener erster Nacharbeitsstand f8e1dfc: Architecture Run38651701033 / Job116151786197 scheiterte tatsächlich an `scope:diff-check`: zusätzliche Schlussleerzeile des unveränderten Originalberichts. Lokaler vorheriger Scope lief vor diesem Commit mit untracked Originaldatei; dessen Whitespaceprüfung deckte die erst eingecheckte Schlussleerzeile nicht ab. Der neue verlustfreie Transport behebt die eigene Persistierungsgrenze, ohne Originalbytes oder Qualitätsanforderung zu ändern. Der tatsächliche Lieferstand 93b49a9 hat Architecture und sämtliche zehn Unity-Jobs einschließlich beider Plattformkompilationen bestanden. Nachgelagerte Dokumentations-/Bindungskorrekturen werden separat geprüft; keine Übertragung eines alten PASS auf geänderte Inhalte. O-02 bleibt ursächlich offen, keine Gesamt-DoD-Freigabe.

## Aktuelle Nachführung zur Gesamtabnahme – 15.10.2026

Stand der Originalgesamtabnahme: `b1457ba6a61826fe3b64a83323c27a13e9d23080`; Quellenstand main: `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16`. Historische Manifestbasen bleiben WP-026=c9516bd und WP-028=d97c39d. Keine Umdeutung des PR-Metadatenfelds base_sha zu aktuellem main.

**WP-027 entscheidet R01-FIRST und R18-MIDDLE.** Bible §§4.1/8.1: erster gezielter Variationsschwerpunkt, frühere Portseitenvielfalt zulässig; vorbereitender lokaler Einser-Portanker zulässig, weitere Lösung kombiniert mittlere Resthinweise. Keine neue Port-/Einserquote. Quellen: [WP-027 auf main](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/blob/d90e798dd9b26d3f152f3b0c2d1d52cae641eb16/WORK_PACKAGES/WP-027_Season-1-Quellenklarstellung-und-CI-Leistungsweg.md), dort bestätigte Bible und ADR-032.

**CI-PERF-WP026 auf b1457ba erbracht.** WP-028: Freeze b449ab7, PR #25 ausschließlich nach WP-026 integriert als b1457ba, gleicher Tree 2bc50e6. Historische Unity-CI auf b1457ba [38637012837](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/38637012837): 455/455 EditMode, 27/27 PlayMode, Android IL2CPP und iOS/Xcode erfolgreich; Architecture [38637012841](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/38637012841) erfolgreich. 350 Samples, p95-Index 332, p95 38,2026 ms, Maximum 72,0952 ms, 240 frische Proofgates in 2.586,8097 ms mit vier Arbeitern. Git/Linux-Fingerprint 6fc307231c43d3f09c60fa3116c8517588a40855c4266e8ec4dd120ed0b6372a; Windows-CRLF-Fingerprint 29c16b2704125ea1f38dda74e1a9016e631c6f4ae9e19f3161d1009f4ee4b679. Dies sind Nachweise dieses historischen Standes, kein vorweggenommener PASS neuer Metadaten.

**Neue unabhängige Gesamtabnahme durchgeführt, KEIN GESAMT-PASS / NICHT INTEGRATIONSBEREIT.** [Originalbericht](WP-026_Gesamtabnahme_b1457ba.txt), byteunverändert, SHA256 9c0a03fe060e86e69d5041bb3e72e72a55c198029ac3a9990222e16d99e15433. Evidenzarchiv SHA256 e1a974ebfdf1f87ee9af6bb08981563e8ce3a5a3166c0c21aada92d81cfb4258, ausschließlich isoliert. Alle 240 logischen Rätsel/20 Routen gegen WP-027 geprüft; F-01 begrenzter Sachtextbefund, keine neue Puzzlelogikabweichung. Originale frühere Prüfberichte bleiben unveränderte standgebundene Historie.

F-01 ist unter dem bestehenden WP-026-Metadatenscope und separat verankerter WP-029-Nachweisführung korrigiert: x0=0, x1=2, x3=6, x4=5. Ein Level Revision 2→3, kein neuer öffentlicher Input, keine neue Lösung oder Proofbytes. Reale neue Bindungs-/Regenerationsnachweise und Grenzen stehen in [ACCEPTANCE_FOLLOWUP.md](ACCEPTANCE_FOLLOWUP.md). Separate unabhängige Fachprüfung auf 93b49a9 bestätigt F-01 einschließlich eigener 240er-Metadaten-/Bytegegenprüfungen. Ihr Statusbefund QC-CONTENT-01 und der technische Byteprofilbefund werden hier eng korrigiert; gezielte Nachprüfung des Korrekturdiffs bleibt erforderlich.

O-04: Die drei Konflikte sind im gemeinsamen Preview 060c1c4 tatsächlich gelöst; der kombinierte Baum wurde mit Produktion, Schema/Node/Root, Architektur und vollständiger Unity-CI (455 EditMode, 27 PlayMode, Android, iOS/Xcode) geprüft. Das dort aufgerufene unveränderte WP-031-Scopegate meldet neun Fehler beim Main-Import; dieser FAIL bleibt erhalten. Die unabhängige technische Gegenprüfung weist einen prospektiven neuen engen Auftrag auf Parent 060c1c4 als grundsätzlich zulässigen ADR-030-Weg nach; dafür ist WP-032 separat zu verankern und die Herkunft beider Lieferungen plus Konfliktdelta unabhängig abzunehmen. Kein pauschal behaupteter neuer Architekturentscheid erforderlich, kein nachträgliches Grünfärben von WP-031. Weiter offen: O-01 menschlicher Spieltest, O-02 historische Timeoutursache, O-04 dieser getrennte Integrationsvorbereitungs-/Prüfschritt, O-05 1.440 Referenzziele, O-06 Zeitklasse sowie O-07 kreative/Import-/Produkt-/Release-/Geräte-/Storegates. WP-030 hat die Untersuchung geliefert, keine kausale Reparatur. Keine Erweiterung alter Paketdefinitionen oder Manifeste, kein Gesamt-PASS, Main-Merge, Runtimeimport oder Release. Aktuelle gezielte QC-Ergebnisse und Korrekturen bleiben an ihren tatsächlichen Prüfstand gebunden.

Die nachfolgenden älteren Abschnitte beschreiben ihren damaligen Stand. Ihre überholten Entscheidungs-/CI-PERF-Statusangaben sind keine aktuelle Blockerliste.

# WP-026: nachgebesserter logischer Bestand, erneute Abnahme offen

Geliefert sind 240 konkrete logische Authoringrätsel mit Lösungen und echten
Produktionsproofs. Alle 240 Einzelbewertungen und 20 Routenurteile sind neu
individuell ausgearbeitet; 15 gezielte Logikänderungen, 225 erhaltene öffentliche
Inputs/bytegleiche Proofs. Keine fertig ausgestaltete oder freigegebene Season.

Der unveränderte Originalbericht steht in
[`WP-026_Unabhaengige_Pruefung_PR21_3c9b7d0.md`](WP-026_Unabhaengige_Pruefung_PR21_3c9b7d0.md).
Die getrennte Befundbearbeitung mit jedem V-/Q-/O-Status, Quellenbeleg und
echten offenen Entscheidungen steht in [`REVIEW_RESOLUTION.md`](REVIEW_RESOLUTION.md).
Die frühere Eigenbewertung und der ursprüngliche technische Teil-PASS werden
nicht auf den geänderten Bestand übertragen.

## Nachprüfbare Artefakte

- Kanonische 240 Level-v2-Quellen und 240 `solver-v2.proof-v1.json`; Revision 2, bei drei sachlichen Nachkorrekturen Revision 3; Sternsekunden weiterhin null.
- `author-decisions.txt`: tatsächliche literal verfasste Einstiege/Fokuswerte/Qualitätsurteile, 20 Routenurteile, 240 vorläufige gedankliche Kettenschätzungen; `author-review.json`: Public-Puzzle-/Lösungsbindungen.
- 20 `leveldesign-S1-*.md` mit konkretem Gitter, Counts, Ports, tatsächlicher maximaler Rootkette und eigenem Urteil; 20 frische Rootspurdateien.
- `targeted-changes.json`, `candidate-provenance.json`, `repair-summary-facts.json`, `similarity-report.json` und `PAIR_ADJUDICATION.md` für Änderungen, tatsächliche Provenienzgrenzen und vollständige D4-Ähnlichkeit.
- `technical-evidence.json`, `schema-node-evidence.json`, `repair-technical-validation.txt`, `repair-countercheck-*.txt`, `repair-regression-validation.txt` und Original-NUnit-XMLbytes als originalXmlBase64 in repair-regression-evidence.json (TXT als Git-normalisierte Lesefassung).
- Tatsächlich heruntergeladene alte CI-Artefakte mit gehashtem Inhalt, vier abgerufene Joblogs, echte XML-/Coverage-/Buildbelege in `ci-baseline-*`; ausdrücklich an `3c9b7d0…` gebunden.

Alle aktuellen Rätsel sind UNIQUE/1 mit searchNodes=0 und requiredGuessDepth=0.
Vor dem Einfrieren werden die endgültigen realen eigenen Schema-/Produktions-/
Node-/Root-/Gegenprüf- und Scope-Läufe gemeinsam persistiert; der abschließende
eingefrorene Commit und seine tatsächliche neue CI stehen am bestehenden Draft-PR
[21](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/21).
Ein Bericht zu einem anderen HEAD ist keine neue Abnahme.

## Reproduktion und Evidenzrollen

Vorhandene Produktionsmodule, Schema-/Hash-/Solver-/Strictverträge und Tests
bleiben unverändert. Reine Authoringhilfen nutzen ihre APIs; Methodik und
konkrete Befehle in `tools/season1-authoring/AUTHOR_REVIEW_METHOD.md`.
Die gesonderte lokale Linux-Umgebung verwendet CPython 3.11.13, Node 22.20.0
und exakt die sechs vorhandenen Lockpakete. Die C++-/Python-Gegenprüfungen aus
dem Archiv laufen ausschließlich isoliert; der vorübergehend dafür bereitgestellte
Compiler ist keine Projektabhängigkeit. Autorbetrieb mit unabhängig geschriebenem
Prüfercode ist eigene Zusatzprüfung, keine unabhängige Freigaberolle.

Die unabhängigen Rollen haben den gesamten Stand `96697ab` selbst geprüft;
ihre vollständigen Berichte und der zwingende CI-Nachtrag sind unverändert als
`INDEPENDENT_TECHNICAL_96697ab.md`, `INDEPENDENT_TECHNICAL_CI_ADDENDUM_96697ab.md`
und `INDEPENDENT_CONTENT_96697ab.md` persistiert. Drei begrenzte sachliche
Textfehler wurden daraufhin korrigiert; public Inputs/Lösungen/Proofbytes bleiben
gleich, betroffene Dokumentbindungen und das Route-18-Urteil sind erneuert.
Tatsächliche unabhängige Nachprüfung dieser Folgeänderungen nach neuem Freeze,
abschließender HEAD/Scope/Remote/CI-Bezug und unveränderte vollständige Berichte
werden am bestehenden Draft-PR persistiert. Kein alter Teil-PASS gilt automatisch.
Menschlicher Spieltest, Gesamt-DoD und Integrationsfreigabe bleiben offen.

## Echte Übergabepunkte

1. **DECISION-WP026-R01-FIRST:** Quellenbeleg Bible 14 §4, Route 1, 9/10 „erste unterschiedliche A/B-Kanten“. Entscheidung, ob frühere Seitenvielfalt ausgeschlossen ist oder der ausdrückliche Schwerpunkt gemeint ist; kein eigenmächtiger Ersatzplan. Nach Entscheidung betroffene erste Route erneut prüfen.
   **DECISION-WP026-R18-MIDDLE:** Bible 14 §8, Route 18/3–5 „mehrere mittlere Hinweise als einziges Orientierungsmittel“. Konkrete Einser-Portzugänge in 05-02-03/04/05 und danach notwendige mittlere Kombination sind im Register benannt. Entscheidung über zulässige vorbereitende Einseranker oder deren Ausschluss durch „einziges“ fehlt; kein eigener Zahlenbereich oder Ersatzplan.
2. **CI-PERF-WP026:** 10×10 p95 < 250 ms/max 2 s und Gesamtbestand < 60 s auf CI-Referenzrechner (WP Tests 5, SOLVER_ARCHITECTURE §10). Bestehende Workflowbefugnis erlaubt nur Manifestzuordnung, keinen neuen Korpus-Leistungsschritt. Separater beauftragter Ausführungsweg nötig; lokale Millisekunden sind kein Ersatz.
3. **Unabhängige Neuabnahme und menschlicher Spieltest:** insbesondere Nichtredundanz verbleibender Armfamilien, sehr lange dünne Finales und neuer Route-18-Wechsel 3→6. Kein alter Teil-PASS und keine Selbstfreigabe.
4. **Dokumentierte historische Auswahlgrenze:** tatsächliche Pools/Matches wiedergefunden, ursprüngliche nicht persistierte Seeds-/Generatorvarianten nicht erfunden. Aktueller Source-/Proofkorpus reproduzierbar. Unabhängige Prüfer bestätigen heutige Kuratierung; keine zusätzliche rückwirkende Kampagnen-Seedpflicht aus Endless-Regeln ableiten.
5. **Fremde Content-/Folgegates:** 1.440 fehlende Referenzziele bleiben Cross-Reference-FAIL; vorläufige Zeitklasse unbestimmt. Kreative Ausgestaltung/Miniaturen/Präsentation, Pseudolocale, Doppelimport, Addressables, Lizenz/Größen, Produkt-/Release-/Geräte-/Storegates bleiben offen und werden nicht abgeschwächt.

Zusätzliche tatsächliche CI-Grenze: Unity-Push auf `96697ab` scheiterte am
180-s-Timeout eines unveränderten Solverfixturetests (453/454). Der getrennte
PR-Lauf besteht EditMode 454/454 und PlayMode 27/27; verfügbare neue Logs und
Artefakte wurden tatsächlich abgerufen und gehasht. Endstände getrennt in
`ci-repair-96697ab-artifact-evidence.json`/PR. Die Timeoutursache ist offen;
gegebenenfalls nötige Test-/Workflow-/Produktionscodekorrektur liegt außerhalb
dieses Manifests. Keine ungeprüfte Flaky-/Quarantänefreigabe und kein Ersatz
des gesondert fehlenden Korpus-Leistungsnachweises.

Bestehende Branch `codex/wp-026-season1-logical-content`; Basis
`c9516bdbd5858fd9003d5207ec37d180ccc6ae23`; gemeinsamer Anker
`f9231c73e45ade33450c541e77dbeff5d2eae162`; Manifest-SHA256 unverändert
`981cd37f45c0bb73106d321ad450fbf8ad8e210a508b8a177d25892058ef018c`.
Keine neue Verankerung, Paket-/Manifest-/Architektur-/Planungsänderung,
Implementierungsparallelisierung, kreative Arbeit, Zeitkalibrierung, Merge,
Runtimeimport oder Release.
