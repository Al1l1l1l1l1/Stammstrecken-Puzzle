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

- Kanonische 240 Level-v2-Quellen und 240 `solver-v2.proof-v1.json`; Revision 2 für die tatsächlich neu bewerteten Produktionsmetadaten, Sternsekunden weiterhin null.
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

Die neue unabhängige technische und fachliche Prüfung muss auf dem nach eigener
Fehlerbehebung eingefrorenen Stand stattfinden. Sie wird nur bei tatsächlich
vorhandener unabhängiger Ausführung organisiert; menschlicher Spieltest wird
damit weder ersetzt noch behauptet. Gesamt-DoD und Integrationsfreigabe bleiben offen.

## Echte Übergabepunkte

1. **DECISION-WP026-R01-FIRST:** Quellenbeleg Bible 14 §4, Route 1, 9/10 „erste unterschiedliche A/B-Kanten“. Entscheidung, ob frühere Seitenvielfalt ausgeschlossen ist oder der ausdrückliche Schwerpunkt gemeint ist; kein eigenmächtiger Ersatzplan. Nach Entscheidung betroffene erste Route erneut prüfen.
2. **CI-PERF-WP026:** 10×10 p95 < 250 ms/max 2 s und Gesamtbestand < 60 s auf CI-Referenzrechner (WP Tests 5, SOLVER_ARCHITECTURE §10). Bestehende Workflowbefugnis erlaubt nur Manifestzuordnung, keinen neuen Korpus-Leistungsschritt. Separater beauftragter Ausführungsweg nötig; lokale Millisekunden sind kein Ersatz.
3. **Unabhängige Neuabnahme und menschlicher Spieltest:** insbesondere Nichtredundanz verbleibender Armfamilien, sehr lange dünne Finales und neuer Route-18-Wechsel 3→6. Kein alter Teil-PASS und keine Selbstfreigabe.
4. **Historische Auswahlprovenienz:** tatsächliche Pools/Matches wiedergefunden, ursprüngliche nicht persistierte Seeds-/Generatorvarianten nicht erfunden. Aktueller Source-/Proofkorpus reproduzierbar; Entscheidung über zusätzliche historische Abnahmeanforderung beim unabhängigen Prüfer.
5. **Fremde Content-/Folgegates:** 1.440 fehlende Referenzziele bleiben Cross-Reference-FAIL; vorläufige Zeitklasse unbestimmt. Kreative Ausgestaltung/Miniaturen/Präsentation, Pseudolocale, Doppelimport, Addressables, Lizenz/Größen, Produkt-/Release-/Geräte-/Storegates bleiben offen und werden nicht abgeschwächt.

Bestehende Branch `codex/wp-026-season1-logical-content`; Basis
`c9516bdbd5858fd9003d5207ec37d180ccc6ae23`; gemeinsamer Anker
`f9231c73e45ade33450c541e77dbeff5d2eae162`; Manifest-SHA256 unverändert
`981cd37f45c0bb73106d321ad450fbf8ad8e210a508b8a177d25892058ef018c`.
Keine neue Verankerung, Paket-/Manifest-/Architektur-/Planungsänderung,
Implementierungsparallelisierung, kreative Arbeit, Zeitkalibrierung, Merge,
Runtimeimport oder Release.
