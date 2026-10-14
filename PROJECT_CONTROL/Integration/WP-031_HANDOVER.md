# WP-026-Nacharbeiten: prüffertige Übergabe vom 15.10.2026

**Kein Gesamt-PASS, keine Main-Integration, kein Runtimeimport oder Release.** F-01 und aktuelle Statuskorrekturen umgesetzt; O-02 untersucht und kausal offen; O-04 im isolierten gemeinsamen Git-Baum aufgelöst und tatsächlich geprüft, Übernahme am unveränderten Scopegate blockiert. Unabhängige gezielte Nachprüfung bleibt erforderlich; eigene Werkzeuge/CI ersetzen sie nicht.

## Getrennte Liefer- und Quellenstände

| Stand | Identität / Bedeutung |
|---|---|
| Originalgesamtabnahme | b1457ba6a61826fe3b64a83323c27a13e9d23080, tatsächlich durchgeführt, kein Gesamt-PASS |
| Maßgebliches main | d90e798dd9b26d3f152f3b0c2d1d52cae641eb16, unverändert; WP-027/Bible-Klarstellungen/ADR-032 |
| WP-029-Anker | 78e8dc9, einziger Parent/Manifestbasis b1457ba |
| F-01-/Statusstand | f8e1dfc8dc1fdc7846d26060e0b354b6e4a32af8; eigener erster CI-Whitespace-FAIL transparent erhalten |
| WP-030-Anker | 438b45e, einziger Parent/Manifestbasis f8e1dfc |
| Untersuchungscommit | 21f8ce5d5f9ed39b4f63c4372b5c4e92b1b1fb2a |
| WP-031-Anker | f1f0c65, einziger Parent/Manifestbasis 21f8ce5 |
| Technischer Lieferfreeze | d7f69f66e6d58191fa44d64a20523f78ad9a5432; PR #21 / codex/wp-026-season1-logical-content |
| Gemeinsamer Preview | 060c1c4b01458a80835094d5bd004be8c36eec54, Tree 652661ddd524cbfaea1bdeb2f5de2e8d4ea9c101; Eltern d7f69f6 und main d90e798 |
| Preview-Übergabe | [Draft-Unter-PR #26](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/26), Branch codex/wp-031-integration-preview; kein Merge autorisiert |

Historische WP-026-/WP-028-Anker, Manifestbasen und Manifestbytes unverändert: f9231c7/c9516bd/981cd37f…018c beziehungsweise 0d17996/d97c39d/419ac8e0…f4bb. WP-027 und sein Manifest/Produktquellen werden ausschließlich im Preview bytegetreu aus main erhalten. Nachgelagerte reine Nachweis-/Übergabecommits am Lieferzweig sind separat durch ihren tatsächlichen PR-HEAD und CI zu binden; sie verändern diesen eingefrorenen Preview nicht. Kein fälschliches Gleichsetzen von Quellenstand, Lieferstand, Manifestbasis oder PR-base_sha.

## F-01: umgesetzt, gezielte unabhängige Nachprüfung offen

Die falsche Nachbarschaft wurde präzisiert: linke Nullspalte x0; Fünfercount x4 rechts der vollen Spalte x3. Tatsächliche Counts [0,2,3,6,5,3]. Literal, Autoren-JSON/-Quellhash, eine Levelrevision 2→3, Inventar, Routenreview, neue Produktions-/Dokument-/Nodebindungen konsistent erneuert. 240 öffentliche Inputs und Lösungen sowie 240 Proofbytes erhalten; zweimaliger Transport byte-idempotent. Siehe F01_BINDINGS.json/F01_VALIDATION.txt im Reviewscope und eigene WP-029-Nachweise. R01-/R18-Urteile nur mechanisch auf Manus' bestätigte Lesart bezogen; keine neue Produktentscheidung.

Der Originalbericht ist vollständig verlustfrei im gekennzeichneten Base64-TXT-Transport gespeichert. Dekodierter Originalhash `9c0a03fe060e86e69d5041bb3e72e72a55c198029ac3a9990222e16d99e15433`, 39.661 Bytes, einschließlich der unveränderten Schlussleerzeile. Containerdateihash getrennt in F01_BINDINGS.json; Dekodierbefehl `python -B tools/wp031-integration/prepare.py decode --output <Scratchdatei>`. Isoliertes Evidenzarchiv 42.903.748 Bytes, SHA256 `e1a974ebfdf1f87ee9af6bb08981563e8ce3a5a3166c0c21aada92d81cfb4258`. Originale externe Ablage: `C:/Users/Anwender/.codex/visualizations/2026/10/14/01a13c6b-0929-7472-99ff-8d249ffd3cd5/wp026-audit/`. Originalprüfberichte nicht redigiert.

Eigener erster Persistierungsversuch auf f8e1dfc scheiterte in echter CI an `scope:diff-check` wegen der zusätzlichen Original-Schlussleerzeile. Die lokale Vorprüfung hatte die noch untracked Datei nicht im Git-Whitespacevergleich. Behoben durch verlustfreien Transport, kein Trim, keine `.gitattributes`-Ausnahme, keine Testlockerung. Tatsächlicher neuer Architecture-Run38652534600 auf d7f69f6 SUCCESS; EditMode-Artefakt11935741917 nach Download selbst gehasht und gelesen: 455/455, kein Failed/Skipped, XML-SHA256 d98b967b1414821ac7e75e3772ff921b40054109de9f56c37bb68ef7965f971e. Vollständige Daten in WP-031_CI_DELIVERY_D7.json; keine vorweggenommenen weiteren Build-PASS.

## O-03: aktuelle Einordnung geliefert

WP-027 entscheidet R01-FIRST/R18-MIDDLE in Bible §§4.1/8.1. WP-028 erbringt CI-PERF auf b1457ba; die neue Gesamtabnahme ist tatsächlich durchgeführt und meldet keinen Gesamt-PASS. Diese drei Dinge stehen in CURRENT_STATE, WORK_QUEUE, WP-026, den aktuellen Reviewübergaben und PR #21. Ältere Abschnitte/Originalberichte bleiben klar als standgebundene Historie; ihre damaligen NOT_EXECUTED-/Entscheidungsvorbehalte sind keine heutigen Blocker.

Auf dem korrigierten d7f69f6 erneut tatsächliche CI-Messung: 350 Samples / Index332, p95 34,7518 ms, Maximum 69,815 ms, 240 frische Proofgates in 2074,2593 ms, vier Arbeiter. Neuer Git-Rawfingerprint `360f3309ff958ab6871f963354ea5ec691a096b0aec68bf19165a1ea43cea6bf`; Änderung gegenüber b1457ba ist die eine redaktionelle Levelrevision/Qualitätsnotiz. Keine Puzzle-/Lösungs-/Proofänderung und keine Übertragung des alten Rohbytefingerprints.

## O-02: Untersuchung geliefert, historische Ursache offen

Vollständiger eigenständiger [WP-030-Bericht](../Investigations/WP-030_TIMEOUT.md) mit originalen XML-Hashes, 37 Zeitvergleichen, Quellgleichheit und tatsächlichen sechs Phasen. Original-FAIL hatte bereits alle sechs Ergebnisarten mit unauffälligen Nachlaufverhältnissen protokolliert; die Zustandsgrenze war in FAIL und PASS gleich 2. Große 32×32-Budgetfälle waren im Push etwa doppelt so langsam, der Median aller 37 Tests dagegen 1,0346. Neue Scratchprobe belegt hohe Allokationslast, keine historische CPU-/GC-/Schedulerursache. Aktueller d7-CI-HardBudget-PASS in 82,823298 s erklärt den alten 197,533248-s-Timeout nicht.

Konkreter offener Entscheidungsbedarf: eigener enger Testdiagnostikauftrag mit Phasen-/CPU-/GC-/Allokationsausgaben im bestehenden regulären CI-Weg und Korrelation beim nächsten Vorfall, unveränderte Assertions/25 Samples/Budgets/Timeout/Runner. Keine Reparatur des ausgeschlossenen Originaltests oder Solvers angewandt. Keine Quarantäne, kein alter CI-Retry, keine erfolgreiche Wiederholung als Ursachenbeweis. O-02 bleibt ein Zuverlässigkeitsgate.

## O-04: Kombination erstellt und geprüft, Scopefreigabe offen

Alle drei Konflikte isoliert gelöst. Workflow behält Main-Ausführung, nur eigene Manifestzuordnung; CURRENT_STATE erhält aktuellen WP-026-Stand und WP-027-Quellennachweis; WORK_QUEUE erhält Manus' Main-Prioritätsfolge mit aktualisierten WP-026-/WP-028-Statusangaben. Keine stillschweigende Umplanung. Alle **1109 Nichtkonfliktpfade** gegen die drei Eingabebäume geprüft: jeweils exakt der zuständige Elternblob. Main-Bible/ADR-032/Validator und sämtliche historischen Manifeste erhalten; alle Lieferquellen/Proofs/Tests/Produktionsmodule ebenfalls. Reproduzierbarer Ersteller `tools/wp031-integration/prepare.py preview`; tatsächliche Eltern-/Treemenge in WP-031_PREVIEW.json und nutzbarer Zero-Context-Konfliktpatch WP-031_CONFLICT_RESOLUTION.patch.

Tatsächlich auf **060c1c4** ausgeführt: drei Regenerationen je Level, Strict-One/Batch/Reverse, 3840 Metamorphosen, 1440 verworfene Mutationen, 240 frische Produktions-/240 Proofschemas und 1200 Nodebindungen, 20 Rootdateien neu erzeugt und verglichen. Stabile Produktionsevidenz identisch, ausschließlich neue lokale Messzeiten; aktueller Nodebeleg identisch. Generierte Outputs außerhalb des Preview gesichert, anschließend nur die eigenen erzeugten Evidenzdateien aus HEAD wiederhergestellt; Preview sauber und unverändert. Keine alte getrennte Branchprüfung als gemeinsamen PASS ausgegeben.

Linux CPython3.11.13 / Node22.20.0 / genau sechs Lockpakete: Architecture-only 17 Gruppen PASS. Unverändertes Production-Scopegate im gemeinsamen Baum **FAIL mit neun konkreten Fehlern**, sowohl lokal als auch echter CI38652697909 / Job116155055227. Außerhalb des eigenen Deltas liegen die aus main übernommenen ADR-032-/Index-/Ausführungsbrief-/Bible-/WP-027-/Validator-/Scopeschema-/Manifestdateien; die Bible meldet zusätzlich `scope:product-source`. Die Allowlist fremder Manifeste würde ihrerseits `scope:other-manifests-allowed` ergeben, ein pauschaler Governance-Modus würde Produktionsartefakte nicht legitimieren. Einzelmanifest- und Quellenregeln wurden nicht umgeschrieben. Die CI führt beide Lieferungen tatsächlich im selben Baum aus, das Scope-FAIL bleibt wirksam.

Ein lokaler erster Previewlauf hatte zusätzlich `manifest-mutated-after-anchor`, weil Windows beim Klonen ausschließlich die Working-Tree-Zeilenenden der Manifeste in CRLF umgewandelt hatte. Exakte unveränderte Gitblobbytes lokal wiederhergestellt, keine Manifest-Gitänderung; Wiederholung ergibt die gleichen neun Fehler wie Linux-CI. Vorlauf in WP-031_COMBINED_SCOPE_WINDOWS_CRLF.txt, Umgebung in WP-031_ENVIRONMENT.json, kanonischer tatsächlicher Folgelauf in WP-031_COMBINED_SCOPE.txt. Keine Umdeutung des Vorlaufs zu PASS.

**Konkrete Übernahmegrenze:** Es fehlt ein bestätigter Integrationsabnahmeweg für diesen mehrschichtigen Nicht-main-Stand. Vorschlag zur Entscheidung: provenancegebundene Zusammensetzungsprüfung der bereits getrennt verankerten Lieferungen plus exakt begrenztes Konfliktdelta und vollständige gemeinsame technische CI, ohne historische Manifeste zu ändern. Die aktuelle Instanz führt keinen neuen ADR, keine Validatorausnahme und keinen Scope-Bypass ein. Deshalb wird der Preview nicht in PR #21 übernommen und kein Main-Merge ausgeführt. Draft #26 ist der konkrete reviewbare Entscheidungs-/Prüfstand.

## Pflichtprüfungen, Status und nächste Übergabe

Eigener Lieferzweigscope WP-031: lokale 18 Gruppen PASS; aktueller d7-CI-Architecture-PASS und tatsächliches EditMode-XML gelesen. Gemeinsamer Preview: lokale Produktions-/Schema-/Root-/Architekturprüfungen wie oben; echte Unity-CI38652697933 besteht am gemeinsamen Preview EditMode 455/455, kein Failed/Skipped; Original-XML 57a691e29d55ebed913bc6691178284821b0583f9e4cd06d672edeb1551eb9e0, ZIP-Artefakt11936206872 / f7c47e8024f26aa828a25c4055cfe03f8b22cb676b35ccdba48262e868ddd514 tatsächlich gehasht/geparst. 350 Samples nachgerechnet: p95 43,8702 ms, Maximum103,8109 ms, 240 frische Proofgates in2147,5077 ms, Fingerprint360f3309…a6bf. Original-XMLbytes zusätzlich verlustfrei gzip/base64 in WP-031_CI_PREVIEW_060.json persistiert. Der aktuelle HardBudget-PASS84,001766 s erklärt die historische Ursache weiterhin nicht. Weitere Unity-Schlussstände bleiben separat. Deren finale Test-/Build-/Artefaktbindung sowie die CI nach dem reinen Übergabecommit werden nach tatsächlicher Ausführung als unveränderte PR-Nachweise gesichert, ohne rekursive neue Belegcommits. Nur dann tatsächlich abgeschlossene Jobs als PASS nennen. Prüfsummen der eigenen Nachweise im zugehörigen Manifest, jeweilige Run/Commit/Artefaktbindung in den Daten.

WP-031 AK-01/02 in eigener technischer Prüfung erfüllt; AK-03 gemeinsame lokale Prüfung erbracht, echter CI-Schlussstand getrennt nachzuweisen; Scopepflichtgate bleibt FAIL. AK-04 konkrete Grenze/Status/PR geliefert. AK-05 unabhängige gezielte Prüfung **nicht simuliert**, prüffertig extern zu übergeben. Technischer Gesamtabschluss/Integrationsfreigabe bleiben offen.

Menschlicher Spieltest, 1440 offene Referenzziele/Cross-Reference-FAIL, UNCALIBRATED/null Sternsekunden, kreative/Import-/Produkt-/Release-/Geräte-/Storegates bleiben unverändert. Nächster Schritt: enger O-02-Diagnostik- und O-04-Integrationsweg zur Entscheidung; danach passende unabhängige gezielte Nachprüfung des tatsächlich gewählten gemeinsamen Freeze. Keine Gesamtfreigabe aus erledigten Teilaufgaben.
