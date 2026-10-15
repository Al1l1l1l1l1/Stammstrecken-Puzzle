# WP-032: eigener gemeinsamer Integrationsprüfstand

**Kein Gesamt-WP-026-/Human-/Main-/Import-/Release-PASS.** Neue prospektive enge Integrationsvorbereitung gemäß ausdrücklicher Nutzerbeauftragung; kein neuer Architekturvertrag und kein rückwirkender PASS des alten Importscopegates.

| Stand | Identität / Übergabepunkt |
|---|---|
| Quellen main | d90e798dd9b26d3f152f3b0c2d1d52cae641eb16, unverändert |
| Technische Lieferung | d7f69f66e6d58191fa44d64a20523f78ad9a5432 |
| Ursprüngliche Zusammensetzung | C060=060c1c4b01458a80835094d5bd004be8c36eec54, Tree652661d, Eltern d7/d90 |
| Eigener WP-/Manifestanker | fdee9569eb52ee66b23028433c86d5d9f56dacdb, einziger Parent/BaseC060 |
| Geprüfte P2-Korrekturlieferung | 9c1859f649201602e34770c5399f8fca5a39c878, beide gezielten unabhängigen Rollen PASS |
| Dokumentarische Zusammenführung | 37aff065c7cba3ed6917d1ed8e38b81890f5cce2, Treecd809d5a17b8ee29cb46368c0d75d3fdc5683db4, Eltern fdee956/9c1859f |
| Eigener Arbeitsbranch | codex/wp-032-integration-acceptance, getrennte Draft-Übergabe gegen offenen WP-026-Lieferzweig |

Der tatsächliche endgültige Commit/Tree, finale Scope-/CI-/Rollenberichte und Artefakte werden nach Ausführung unverändert am [übergeordneten PR21](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/21) sowie am eigenen Draft-PR gebunden. Keine Übertragung eines früheren PASS auf geänderte Inhalte, kein Chat als einzige Ergebnisquelle.

## Getrennte Eingangskontrolle statt verdeckter Basisverschiebung

Die unabhängige technische Gegenprüfung QC-TECH-02 präzisiert ADR026/030: ein neues ausdrücklich beauftragtes prospektives WP darf auf dem unter WP031 schon autorisierten gemeinsamen C060 als Parent ankern. Der eigene kleine Delta ist **kein** Herkunfts- oder Importnachweis. Deshalb sind folgende getrennte Eingangsprüfungen zwingend und tatsächlich ausgeführt:

- Sämtliche drei Eingabebäume vollständig verglichen:1109 Nichtkonfliktpfade nach Blob und Modus exakt dem zuständigen Elternstand zugeordnet. Eigene Nachrechnung in verify.py; unabhängige ursprüngliche technische Rolle hat sie separat neu gerechnet.
- Alle historischen Einzelanker und Manifestbytes erhalten; WP026/WP028/WP029/WP030/WP031 sowie MainWP027 bleiben eigene Aufträge/Nachweise. WP032 hat nur sein eigenes neues Manifest in seiner Allowlist. Originalrollenberichte und ursprünglicher Gesamtbericht unverändert.
- Die drei Konfliktentscheidungen sind im eigenen Abschnitt unten konkret abgenommen; alle sonstigen Mainimporte und Produktion/Test/Level/Proof/Architektur/Validator/Authoringquellen zum C060 exakt erhalten (967 überprüfte Quellpfade).
- Der alte tatsächliche WP031-Kombinationsscope bleibt [FAIL9](WP-031_COMBINED_SCOPE.txt), einschließlich tatsächlicher CI38652697909. Keine Manifestaufweitung, kein Governance-Umbuchen, keine Validator-/Testausnahme und kein nachträgliches Umlabeln dieses Laufs.

`python -B tools/wp032-integration/verify.py --ref <tatsächlicher-Commit> --output <Scratchdatei>` prüft diese Eingänge samt neun F01- und21 kanonischen Nachweisbindungen sowie Originalreportdekodierung erneut. Ergänzendes Diagnosewerkzeug; der unveränderte kanonische Scopevalidator und die unabhängige manuelle Konfliktprüfung bleiben separate Pflichtnachweise.

## Konkrete Konfliktentscheidungen

1. **validate.yml:** Main-Version/Ausführungsverhalten und Fail-closed-Selftestweg erhalten. Zuerst nur aktive031-Zuordnung für C060, jetzt allein eigene prospektive032-Zuordnung. Rohvergleich des gesamten Workflows gegenC060 mit genau dieser Ersetzung; Unity-Workflow unverändert.
2. **CURRENT_STATE:** neuester eigener Liefer-/QC-/Timeoutstatus aus9c1859f übernommen; MainWP027-Anker-/CI-Zeilen und Main-Bible-/ADR032-Quellenbeleg ausC060 erhalten. Quellenmain, Lieferhead, ursprüngliche Gesamtprüfung, historische Manifestbasen und neuer gemeinsamer Prüfstand ausdrücklich getrennt. Keine neue Produkt-/Architekturbehauptung.
3. **WORK_QUEUE:** Main-Prioritätsfolge einschließlich WP027 und WP028 unverändert. Ausschließlich aktuelle WP026-/WP028-/Nacharbeitsstatus und eigener WP032-Ergebnisverweis; keine neue Manus-Planung. Technische CI-PERF ist erbracht, Human-/Zeit-/Referenz-/Importgates bleiben getrennt.

Beim anschließenden rein dokumentarischen Zusammenführen9c1859f nach eigenem Anker gab es nur CURRENT_STATE als Konflikt; oben beschriebene Zusammensetzung erhält explizit die beiden MainWP027-Zeilen und den Quellennachweis. Keine automatische Übernahme fremder Statusverluste.

## Tatsächliche neue lokale gemeinsame Ausführung

Auf37aff06 neue getrennte Produktionsassemblies gebaut:0 Warnungen/0 Fehler. Vollständiger bestehender Verify:240 Level/Proofs, drei Regenerationen je Quelle, Strict-One/Batch/Reverse,3840 Metamorphosen und1440 erwartungsgemäß verworfene Mutationen. Neuer Linux-CPython3.11.13/Node22.20.0-Lauf mit exakt sechs Lockpaketen:240 Levelschemas/240 Proofschemas,1200 tatsächliche Nodeprojektionen/-Artefakte/-Dokumente und240 Root-/Inventarbindungen. Alle20 frischen Rootdateien semantisch exakt mit Gitstand verglichen, stabile Produktionsevidenz identisch; ausschließlich neue lokale Messzeiten. Eigene Ausgaben in WP-032_GENERATED_TECHNICAL.json, WP-032_GENERATED_NODE.json und WP-032_LOCAL_RESULTS.json. Diese lokalen Zahlen sind kein Unity-/CI-Leistungsersatz und keine Spielzeitkalibrierung.

Die erzeugten Contentnachweise werden nach Vergleich nur aus ihren unveränderten Gitblobs wiederhergestellt. Eigene neue Roh-/Bindungsdaten liegen getrennt im032-Bereich; kein fremdes Produktionsartefakt verändert. Finale kanonische Architecture-/WP032-Scope-/Selftests und echte gemeinsame Unity-CI werden am tatsächlichen neuen Freeze separat gelesen/gebunden; nicht vorweggenommen. Historische93/C060-Unityläufe bestanden455/27 und beide Plattformkompilationen, bleiben jedoch genau ihre eigenen Ergebnisse.

## Unabhängige Rollen, Korrekturen und verbleibende Gates

Originale auf93/C060: [Fachbericht](WP-031_QC_CONTENT_93.md), [Technikbericht](WP-031_QC_TECHNICAL_93_C060.md). Alle drei P2-Nacharbeiten auf9c1859f gezielt unabhängig geschlossen: [Fachaddendum](WP-032_QC_CONTENT_CORRECTION_9c.md), [Technikaddendum](WP-032_QC_TECHNICAL_CORRECTION_9c.md), unveränderte Originalbytes. Neuer eigener gemeinsamer Freeze erhält zusätzlich seine eigene gezielte technische und fachliche Prüfung; diese wird separat am PR persistiert, keine Selbstabnahme und keine simulierte Modellidentität. Endgültige AK-/DoD-Reichweite aus tatsächlichen neuen Ergebnissen ableiten.

F01-Ortsangabe/Revision/kanonische Bindungen korrigiert;240 öffentliche Inputs/Lösungen/Proofbytes erhalten. O03-Status/Manus-Auslegungen/erbrachte CI-PERF/durchgeführte Gesamtabnahme richtig eingeordnet. O02-Untersuchung akzeptabel, historische Ursache unbewiesen; eng vorgeschlagene reguläre Testdiagnostik zur Entscheidung, keine Test-/Solverreparatur ausgeführt. O04-Konfliktauflösung und gemeinsame lokale Prüfung tatsächlich durchgeführt; eigener endgültiger Scope/CI/QC und konkreter Übergabestand sind separat zu belegen. Kein pauschaler neuer externer Verfahrensentscheid allein wegen des alten Einzelmanifest-Importscopefehlers.

Menschlicher Spieltest,1440 Referenzziele/Cross-Reference-FAIL,UNCALIBRATED/null Sternsekunden, kreative/Import-/Produkt-/Release-/Geräte-/Storegates bleiben unverändert. Keine Integration nach main, kein Runtimeimport oder Release. Die technische Integrationsvorbereitung erteilt keine Gesamtfreigabe.
