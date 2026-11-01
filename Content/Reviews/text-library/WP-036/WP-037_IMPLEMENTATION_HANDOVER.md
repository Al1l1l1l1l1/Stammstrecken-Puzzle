# WP-037 – implementierte Reparatur und prüffertige Übergabe

**Stand: 18.10.2026.** Der isolierte Zwei-Hunk-Port ist vollständig implementiert und am Codefreeze `3638dd8ba39b1f4f6867fdad03ab70afb0de79af` regulär geprüft. Architecture 39155858104 SUCCESS; Unity 39155858157 SUCCESS mit allen zehn Jobs, 455/455 EditMode und 27/27 PlayMode. Originaler HardBudget-Test Passed (72.442419 s), neuer 1639er-Test Passed (0.513261 s). Dies ist Implementierung/Eigenprüfung, kein unabhängiger AK07-PASS. Die vorgeschriebene unabhängige technische Abnahme bleibt nächster Schritt; Unterintegration danach nur mit gesonderter zuständiger Entscheidung.

## Standbindungen

| Gegenstand | Wert |
|---|---|
| Branch / Draft-Unter-PR | codex/wp-037-hardbudget-wp036 / [PR #32](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/32) |
| Ziel | codex/wp-036-vollstaendige-textbibliothek / PR #31, weiterhin ff1836e293a2a5cff69773525c347b55deb67dfb |
| Main | d90e798dd9b26d3f152f3b0c2d1d52cae641eb16 |
| Gemeinsamer WP-/Manifestanker | 4bfd4ee98153f64a948d2358c6ce30a49a441673 |
| Einziger Ankerparent / Manifestbasis | ff1836e293a2a5cff69773525c347b55deb67dfb |
| Manifest-SHA256 | 022590d076f1e6a972b445b8696c56f83acccfebbaf7e1f30475a388e18e44a4 |
| Veröffentlichter Vorbereitungskopf | bdf5cd8029dc89ad21b10fe58234a3960b4057cd |
| Finaler Codefreeze | 3638dd8ba39b1f4f6867fdad03ab70afb0de79af |
| Codefreeze-Tree | 411696a214427f91d4ec4b96dd7eeebf098b8449 |
| Autorisierte Quellreparatur | f34d6f4ca437effb0ce422015b02618b2a31a9c2 |
| Nachweis-/Statuslieferung | Der nachfolgende reine Commit, der diesen Bericht enthält; tatsächlicher veröffentlichter SHA und eigener CI-Stand werden nach Veröffentlichung im PR-Schlussnachtrag gebunden. Kein vorab erfundener Selbsthash. |

## Tatsächlich implementierter Port

Exakt zwei vorhandene Fachhunks aus der autorisierten Quelle, insgesamt **+29/−3**. Der Codecommit selbst enthält ausschließlich diese beiden Dateien:

| Datei | Vorher-Blob | Ergebnisblob | Diff |
|---|---|---|---|
| RootDeductionTracer.cs | bd719d0395aa6f5f3d6398d9ceec5d3c8fcf3eaa | a41a802fd3e3f2f54074bd0d1e0bf540d0ae5a6c | +12/−3 |
| SolverV2MetricsTests.cs | 9fb1f0482a23eecf888b7ccc6ca9be1c7f945ef1 | 6be782565505376c960fc3f040dd1de95f3202bc | +17/−0 |

Vor-Kontexte sind in WP034-Basis a2e26a6, Main d90e798, WP036-Basis ff1836e und Vorbereitung bdf5cd8 gleich. Ergebnisblobs und vollständiger Zwei-Dateien-Diff stimmen exakt mit f34d6f4 überein; Patch-SHA256 a7e0ba53caf6ab5b73cf31078d042884c0f02001a85f2da8ba3761349ff31ced.

Die View enthält pro Zelle 127, bitweise geschnitten mit den Komplementen aller Removed-Masken eingeschlossener Einträge dieser Zelle. Beim Wegfall eines Eintrags kann nur dessen Zelle abweichen. Sie wird aus allen noch eingeschlossenen Kandidaten neu gebildet; überlappende Masken bleiben korrekt. Schlägt Holds fehl, werden Eintragsauswahl und gesicherter Zellwert gemeinsam restauriert. Kandidaten-, Lösch-, Fakten-, Prämissen- und Suchordnung, Faulted-Pfad und Metriken bleiben erhalten.

PuzzleSolver.cs und PuzzleSolverTests.cs sind bytegleich; damit bleiben Budgetprüfungen und Ergebnisbaufolge unverändert. Keine Übernahme der zwölf WP034-Diagnosezeilen, des WP028-Korpustests, des WP026-Bestands oder fremder Commits. Keine alternative Optimierung. Der Timeout von 180000 ms ist der beobachtete unveränderte Unity-Runnerstandard; der bestehende Test trägt kein explizites TimeoutAttribute. Sein interner 600000 ms-Solverrahmen, sechs Fälle, zwei Warm-ups, 25 Samples, Zustandssuche und 40%-Assertion sind unverändert.

Die einzige Workflowabweichung gegenüber ff1836e bleibt der bereits vorbereitete Manifestselektor WP036→WP037 in validate.yml. Unityworkflow, Runner-/Lizenz-/Coverage-/Zeitgrenzen bleiben unverändert.

## Schutz- und Scopebindungen

[Vollständiger Hunk-/Blob-/Schutznachweis](WP-037_PORT_EVIDENCE.json) enthält 502 vor dem Port bestätigte geschützte Git-Blobs und die 500 nach dem Port unverändert geschützten Pfade. Quelle/Schemata/Texttools/Lesefassung/kreative Annahme und historische Berichte bleiben unverändert. Kanonischer Quell-SHA256 **4458cd9214ed788f69cf3816c39835393d4038bd87f6b262c52ad2b86146d53c**.

[Getrennte Scopeketten und Fachpatch-Ansicht](WP-037_SCOPE_CHAINS.txt): Main→ff1836e bleibt ursprüngliches WP036 samt unverändertem altem Manifest; ff1836e→WP037-Code-/Nachweislieferung bleibt ausschließlich WP037. Der alte WP036-Scope wird nicht über den kumulativen solverhaltigen Diff ausgeführt oder erweitert. Der neue Scope nutzt den ursprünglichen historischen Anker und unveränderte Manifestbytes.

Git verwendet hier core.autocrlf=true. Gitobjekte/CI und kanonische LF-Scratchkopie sind die Bytebasis; rohe Windows-CRLF-Hashes sind ausdrücklich separat ausgewiesen. Keine geschützte Datei wurde zur Umgehung einer Byteprüfung umgeschrieben.

## Tatsächliche Eigenprüfungen und CI

Sauberer exakt gebundener LF-Checkout auf 3638dd8, CPython 3.11.13, Node 22.20.0 und alle sechs Versionen aus requirements.lock.txt. Architecture-only 17 und WP037-Production-Scope 18 mit Self-/Negativtests, JCS 12 positive / 32 negative / 2 v2 / 2 v1, Text 1.408 / 960 Referenzen / 240 INTERNAL / 12 Mutationen und exakte Lesefassung bestanden. Originalkommandos, Versionen und vollständige Ausgaben stehen im Port-Evidenz-JSON. Git-Diffcheck bestanden. Ein gezielter Peerreview unterstützt nur diese Eigenprüfung; er ersetzt AK07 nicht.

| Regulärer Pushlauf am Codefreeze | Ergebnis |
|---|---|
| [Architecture 39155858104](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/39155858104) | COMPLETED / SUCCESS; Architecture, WP037-Scope, JCS, Text und exakte Lesefassung |
| [Unity 39155858157](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/39155858157) | COMPLETED / SUCCESS, alle zehn Jobs |
| EditMode | 455/455 Passed, 0 Failed; tatsächliche neue Original-XML gezählt |
| Original HardBudget | Passed, 72.442419 s; unveränderte sechs Fälle und 40%-Assertion |
| Neue LargeRootTrace-Regression | Passed, 0.513261 s; UNIQUE, 1.639 Schritte, (0,1639,30,0), Trace-SHA256: 6E59FFFAFB16B61C9FDDC301ED9229078FEEA11BEF1E2F44616CE7F9D8460B78 |
| PlayMode/QA | 27/27 Passed, vorgeschriebener QA-Szenentest |
| Plattformen | Android Development IL2CPP; iOS-Export und Xcode-Kompilierung ohne Signierung erfolgreich |

Beide tatsächlichen Workflows filtern pull_request auf Ziel main; der Unter-PR gegen WP036 löst deshalb reguläre **Push-CI** aus. Kein fehlender PR-Lauf wird erfunden und kein Workflowfilter geändert. Original-XMLs, vollständige Logprofile, tatsächliche Checkouts, Trees, Jobzustände, ZIP-/Member-/APKdigests und Coverage stehen in [CI-Evidenz](WP-037_CI_EVIDENCE.json) und [CI-Auswertung](WP-037_CI_AUDIT.md).

Die reine Nachweis-/Statuslieferung erhält einen eigenen nachfolgenden CI-Stand, der am veröffentlichten SHA im PR separat verfolgt wird. Laufende Checks sind kein PASS; die Codefreeze-Ergebnisse werden nicht als neue Ausführung dieses späteren Commits etikettiert.

## Erhaltene historische Befunde

ff1836e: Architecture 39079576265/39079578380 und Unity PR 39079578360 SUCCESS; Unity Push 39079576211 bleibt **FAILURE**, 453/454 EditMode, HardBudget 186.063155 s bei 180000 ms, Folgejobs skipped. Originalbindungen bleiben in TECHNICAL_CI_AUDIT.md/.json. Der spätere PASS löscht den FAIL nicht. Die ältere 89c8-Stornierung ist ein eigener historischer Stand. Vorbereitung bdf5cd8: Architecture 39153458038 und Unity 39153457924 SUCCESS, 454/454 EditMode und 27/27 PlayMode; kein Reparaturbeweis. Keine CI wiederholt, abgebrochen oder durch konkurrierenden Push verdrängt. Keine konkrete historische Host-/Scheduling-/GC-Ursache behauptet.

## Akzeptanzkriterien / DoD / unabhängige Übergabe

| Kriterium | Tatsächlicher Ausführungsstand |
|---|---|
| AK01 | Anker/Parent/Erst-Add/Link/Manifest unverändert, lokaler und regulärer WP037-Scope PASS; Vorbereitung und Codefreeze getrennt. |
| AK02 | Historische Original-FAIL/PASS-Belege unverändert wiederverwendet; neue Vorbereitung terminal geprüft, Herkunft getrennt. |
| AK03 | Exakte zwei Zielblobs/Hunks und View-/Restore-Invariante bestätigt; geschützte Dateien unverändert. |
| AK04 | Originaler HardBudget-Test und alle vorhandenen Zielprojektregressionen in tatsächlicher EditMode-CI bestanden. |
| AK05 | Neue 1639er-Regression in tatsächlicher XML Passed; keine Golden-/Semantikänderung. |
| AK06 | Tatsächliche vollständige reguläre Codefreeze-CI einschließlich Coverage, QA, Android und iOS erfolgreich; Originale gelesen/gehasht. Späterer reiner Nachweiscommit separat. |
| AK07 | **OFFEN:** getrennte unabhängige technische Abschlussabnahme auf diesem Freeze. Eigen-/Peerprüfung ist kein Rollen-PASS. |
| AK08 | Beide Scopeketten und geschützte Inhalte erhalten; Draft-Unter-PR korrekt. Unterintegration und Main-/Produkt-/Import-/Releasefreigaben offen. |
| AK09 | Eigene Dokumentation/Status/Übergabe nachgeführt; veröffentlichter Nachweis-HEAD und sein eigener CI-Schlussstand werden nach Veröffentlichung zusätzlich im PR ausgewiesen. |

Implementierung, vorgeschriebene Codefreeze-Eigenprüfungen und Buildnachweise sind vollständig. **WP037 bleibt bis unabhängiger Abnahme offen; kein selbst erteilter Gesamt-PASS.** Letzter Schritt: exakter Port, erfolgreiche Codefreeze-CI und persistente prüffertige Nachweissicherung. Nächster Schritt: unabhängige gezielte technische Prüfung von AK01–AK06 am 3638dd8 samt reinem Nachweisdelta; neue High-/Blockerbefunde sperren Integration. Danach gesonderte zuständige Unterintegrationsentscheidung mit frischem Expected-HEAD-/Ziel-HEAD-Abgleich; kein automatischer Merge. Bei später freigegebener Unterintegration eigener integrierter SHA/Tree/Diff und tatsächliche neue CI.

Alle 1.408 Texte bleiben DRAFT. Kreative Annahme WP-036-CREATIVE-02 bleibt unverändert; keine neue Textlektüre oder Copyproduktion. Owner-/Produkt-/Runtime-/Import-/Rechts-/Asset-/Zeit-/Geräte-/SDK-/Store-/Release- und Fremdgates bleiben offen. WP026 wird nicht integriert. Bedarf außerhalb der beiden Hunkgrenzen verlangt neue zuständige Entscheidung; kein Manifestwechsel.
