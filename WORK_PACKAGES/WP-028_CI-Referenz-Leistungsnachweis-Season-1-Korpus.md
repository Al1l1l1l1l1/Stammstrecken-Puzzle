# WP-028 – CI-Referenz-Leistungsnachweis Season-1-Korpus

## ID

`WP-028`

**Status:** Definiert, nicht begonnen. Dieser Brief ist noch **kein** Work-Package- oder Manifestanker. Erst nach Integration von WP-027 auf `main` erstellt die Ausführungsrolle auf dem exakt benannten kindlichen Branch von PR #21 eine inhaltlich gleichwertige Kopie als `WORK_PACKAGES/WP-028_CI-Referenz-Leistungsnachweis-Season-1-Korpus.md` und fügt im selben ersten Commit ihr eigenes Manifest hinzu. Dieses Paket ist kein Auftrag zur Änderung oder Freigabe des Season-1-Contents.

Das spätere Production-Scope-Manifest ist absichtlich nicht vorab im Repository angelegt: Es muss gemeinsam mit dem späteren WP-028 auf dessen tatsächlichem Elterncommit `d97c39d03f59a2dbc34930f14e3cb74ebeb2e118` entstehen, damit ADR-030 erfüllt bleibt.

## Ziel

Der echte logische Season-1-Korpus auf PR #21 erhält auf dem bestehenden CI-Referenzweg einen fail-closed, reproduzierbaren Nachweis für die bereits verbindlichen Solverbudgets: sieben tatsächliche 10×10-Level mit p95 < 250 ms und Maximum < 2 s sowie vollständige frische technische Validierung aller 240 Korpuseinträge in unter 60 s, parallelisiert und deterministisch ausgewertet.

## Voraussetzungen

1. Vollständige Lesereihenfolge aus [AGENTS.md](../AGENTS.md): Projektübergabe 15, Index 00, Master 09, CURRENT_STATE und dieses WP; anschließend Content-Bible 14, Konzept 04, WORK_QUEUE, DEFINITION_OF_DONE, CONTENT_PIPELINE, SOLVER_ARCHITECTURE, BUILD_AND_RELEASE, ADR-007, ADR-030, ADR-031 und ADR-032.
2. WP-027 ist nach unabhängiger QC auf `main` integriert. Seine in Content-Bible 14 persistierten Produktklarstellungen gelten; diese technische Arbeit ändert sie nicht.
3. Der tatsächliche Startpunkt ist **exakt** `d97c39d03f59a2dbc34930f14e3cb74ebeb2e118` auf `codex/wp-026-season1-logical-content` (PR #21), nicht ein lokaler Ersatz, historischer Fixture-Stand oder ein pauschal neuer `main`-Branch. Vor Beginn ist Remote-Head, Elternschaft, sauberer Arbeitsbaum und fehlende parallele Bearbeitung zu belegen.
4. Der erste Commit des neuen kindlichen WP-028-Branches führt dieses Work Package und sein Manifest gemeinsam hinzu. Dessen einziger Elterncommit ist `d97c39d03f59a2dbc34930f14e3cb74ebeb2e118`; `baseCommit` des Manifests stimmt bytegenau damit überein und bleibt danach unverändert.
5. Die bestehende Unity-CI ist tatsächlich lauffähig: Unity-Personalzugang und `UNITY_RUNNERS_READY=true`; ohne diese Voraussetzungen wird kein lokaler Ersatz oder simuliertes Ergebnis behauptet.
6. Der bestehende WP-026-Stand bleibt in seinem Scope, seinem Anker und seinen noch offenen Gates unverändert. Insbesondere bilden Produktklarstellung, CI-PERF, unabhängige Nachprüfung, menschlicher Spieltest, 1.440 offene Referenzziele, Zeitklassen, Cross-Reference-, Import-, Produkt- und Releasegates getrennte Aussagen.

## Scope

1. Den gemeinsamen WP-028-/Manifestanker auf dem genannten PR-21-Head einrichten und den bestehenden Korpus dabei unverändert lassen.
2. Die kanonische Architecture-Validation im Branch auf das eigene WP-028-Manifest umstellen; keine neue CI-Pipeline, keine variablen Dispatchparameter und keine Änderung des Unity-Workflows.
3. Einen einzigen neuen Content-EditMode-Test implementieren, der ausschließlich die Korpus- und Messregeln aus ADR-032 ausführt:
   - exakt 240 aktuelle Season-1-Levelquellen und die über `proofRef` gebundenen `solver-v2`-Proofs aus dem Checkout finden;
   - mit vorhandener Produktions-`LevelValidationPipeline` frisch und strict validieren;
   - die sieben realen 10×10-IDs `S1-05-04-06` bis `S1-05-04-12` nach fünf Warm-ups je Input 50-mal je Input direkt mit dem vorhandenen Produktionssolver messen;
   - p95 und Maximum gegen die unveränderten 250-ms-/2-s-Grenzen prüfen;
   - die vollständige technische Korpusvalidierung mit exakt vier parallelen Arbeitern ausführen und gegen <60 s prüfen;
   - alle Ergebnisse ordinal sortiert auswerten und eine stabile, vollständige Nachweiszeile in den NUnit-Output schreiben.
4. Die Testresultate ausschließlich über den regulären `unity-compile-editmode`-Job auf einem eingefrorenen Head erzeugen, Run/Commit/Artefakt tatsächlich lesen und den Nachweis im WP-028-PR transparent verlinken.
5. Eine Unter-PR mit Ziel `codex/wp-026-season1-logical-content` anlegen. Nach bestandener vorgeschriebener unabhängiger QC darf nur diese Unter-PR in den offenen WP-026-Zweig integriert werden. PR #21 gegen `main` bleibt offen; kein Merge nach `main`, kein Runtimeimport und kein Release.

## Betroffene Dateien/Module

Abschließende Außengrenze ist das oben verlinkte Manifest. Zulässig sind ausschließlich:

| Pfad | Funktion |
|---|---|
| `WORK_PACKAGES/WP-028_CI-Referenz-Leistungsnachweis-Season-1-Korpus.md` **neu im WP-028-Branch** | Eigener Auftrag, Anker, Ausführungs- und Abschlussnachweis. |
| `tools/architecture-validation/scopes/WP-028.production.scope.json` **neu im WP-028-Branch** | Eigener unveränderlicher Production-Scope. |
| `.github/workflows/validate.yml` | Ausschließlich Umstellung von `STP_SCOPE_MANIFEST` auf das bereits verankerte WP-028-Manifest; keine andere Workflowänderung. |
| `Assets/StammstreckenPuzzle/Tests/EditMode/Content/Season1CorpusPerformanceTests.cs` **neu** | Einziger neue EditMode-Test für Korpus-/Leistungsnachweis. |
| `Assets/StammstreckenPuzzle/Tests/EditMode/Content/Season1CorpusPerformanceTests.cs.meta` **neu, falls Unity sie beim Import erzeugt** | Unity-Metadaten zum neuen Test. |

## Ausdrücklich nicht erlaubte Änderungen

- Keine Änderung an `Content/Levels/**`, `Content/Proofs/**`, `Content/Reviews/**`, Content-Bible, Konzepten, ADRs, Katalogen, Lokalisierung, Assets oder kreativen Inhalten.
- Keine Änderung an Solver-, Domain-, Proof-, Hash-, Parser-, Schema-, Strict-, CampaignGate- oder Pipelineproduktionscode; keine neue Metrik, Qualitätsquote, Schwierigkeitsformel oder Contententscheidung.
- Keine Änderung an `.github/workflows/unity.yml`, Runnerimage, Unityversion, Jobtimeout, Unity-Lizenzweg oder bestehenden Testausführungsparametern.
- Keine Änderung, Quarantäne, Wiederholungsschleife oder Budgeterhöhung am bestehenden `HardBudget_NoResultIsBuiltAfterTheLastProbe_ForEveryResultKind` oder einem anderen bestehenden Test. Der bekannte Timeoutbefund bleibt separat offen, bis ein eigener passender Auftrag seine Ursache behandelt.
- Kein Benchmark gegen Fixtures, Scratchdateien, lokale Assets, historische oder beliebige 10×10-Raster anstelle der sieben konkreten Season-1-Inputs.
- Kein Merge/Rebase/Cherry-Pick in oder nach `main`, kein Runtimeimport, keine Produkt-/Import-/Releasefreigabe, keine Freigabe von Cross-References, Zeitklassen oder menschlichen Qualitätsurteilen.
- Keine Änderung der WP-026-Definition, ihres Manifests, ihres Ankers oder ihrer bisherigen Evidenz.

## Akzeptanzkriterien

| ID | Kriterium |
|---|---|
| AK-01 | WP-028 und sein Manifest sind erstmals gemeinsam im eigenen Commit auf exakt `d97c39d03f59a2dbc34930f14e3cb74ebeb2e118` eingeführt; Parent, Manifestbasis, lokaler Link und Manifestbytes erfüllen ADR-030. |
| AK-02 | Der neue Test liest ausschließlich den aktuellen kanonischen PR-21-Korpus, fordert exakt 240 eindeutige IDs sowie die durch `proofRef` gebundenen `solver-v2`-Proofs und kann nicht auf Fixtures/Scratch/Teilbestände ausweichen. |
| AK-03 | Für jeden der 240 Eingänge läuft die unveränderte Produktions-`LevelValidationPipeline` frisch und strict. Jeder Erfolg ist ein Proofgate-Erfolg; der Test behauptet ausdrücklich keine Cross-Reference-, Produkt-, Import- oder Releasefreigabe. |
| AK-04 | Die Messung umfasst genau die sieben realen 10×10-IDs, je fünf Warm-ups und 50 Messungen; jeder Messlauf ist `UNIQUE` mit `searchNodes = 0` und `requiredGuessDepth = 0`; p95 <250 ms und Maximum <2 s werden mit der ADR-032-Indexformel geprüft. |
| AK-05 | Die technische Vollkorpusvalidierung verwendet exakt vier parallele Arbeiter, wertet Ergebnisse ordinal aus, scheitert bei jedem Fehler/Timeout/Indeterminate und liegt unter 60 s. |
| AK-06 | NUnit-Ausgabe enthält die vollständige stabile ADR-032-Nachweiszeile inklusive Korpusfingerprint, Commit, Protokollparametern, Metriken, Dauer und Laufzeitumgebung. |
| AK-07 | Architecture-only, WP-028-Production-Scope, Git-Whitespace, vollständiger erlaubter Diff sowie alle regulären Unity-Checks bestehen auf demselben eingefrorenen Head. Der konkrete `unity-compile-editmode`-Run und sein Testartefakt sind tatsächlich ausgelesen. |
| AK-08 | Astra prüft unabhängig Korpusbezug, Produktgrenzen und falsche Freigabeschlüsse; Sol prüft unabhängig Trust Anchor, Scope, CI-Pfad, Messprotokoll und Integrationsrichtung. Kein offener BLOCKER-/HIGH-Befund verbleibt vor der Unter-PR-Integration in den WP-026-Zweig. |

## Tests

1. `python tools/architecture-validation/validate.py --self-test`.
2. `python tools/architecture-validation/validate.py --scope production --scope-manifest tools/architecture-validation/scopes/WP-028.production.scope.json --self-test`.
3. `git diff --check d97c39d03f59a2dbc34930f14e3cb74ebeb2e118 --` sowie vollständige Sicht auf getrackte und untracked Dateien, Anker-/Parent-/Manifest-/Linkprüfung.
4. Lokaler Unity-EditMode-Lauf nur als Vorprüfung, soweit die exakt referenzierte Unitytoolchain tatsächlich vorhanden ist; bei Nichtverfügbarkeit **NOT_EXECUTED**, kein Ersatz-PASS.
5. Verpflichtend: regulärer GitHub-`unity-compile-editmode`-Run auf dem eingefrorenen WP-028-Head. Artefakt/Test-XML und Nachweiszeile selbst lesen; p95, Maximum und Gesamtzeit gegen ADR-032 nachrechnen.
6. Zwei unabhängige QC-Berichte auf exakt demselben Head; nach Integration der Unter-PR in den WP-026-Zweig laufen die regulären Checks nochmals auf dessen neuen Head. Diese Folgechecks ersetzen nicht die weiter erforderliche WP-026-Gesamtabnahme.

## Risikoklasse

**Hoch.** Der Auftrag berührt CI- und Performancefreigabegrenzen für den ersten vollständigen Kampagnenkorpus. Eine Messung gegen falsche Inputs, ein übersehener Ausreißer oder eine versteckte Lockerung würde eine unzutreffende Releasefähigkeit vortäuschen. Der Scope bleibt deshalb bewusst test- und evidenzorientiert.

## Definition of Done

Die [projektweite Definition of Done](../PROJECT_CONTROL/DEFINITION_OF_DONE.md) gilt vollständig. Das später verankerte WP-028 ist nur dann technisch erfüllt, wenn AK-01 bis AK-08 belegt sind und die Unter-PR korrekt in den weiterhin offenen WP-026-Zweig integriert wurde. Die Integration in diesen Lieferzweig schließt weder WP-026 noch Season 1 ab. Vor einem späteren Merge von PR #21 bleiben insbesondere die neue unabhängige WP-026-Gesamtabnahme, menschlicher Spieltest, der eigenständige bestehende Unity-Timeoutbefund, 1.440 fehlende Referenzziele, Zeitklassen und alle Cross-Reference-/Import-/Produkt-/Releasegates unverändert offen.
## Ausführungsverankerung – 14.10.2026

Dieser Auftrag übernimmt den vollständigen integrierten Ausführungsbrief von main `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16`; seine Formulierung „definiert, nicht begonnen“ oben beschreibt die Quellenfassung vor diesem gemeinsamen Erst-Add. Aktueller Status: verankert, Implementierung noch nicht begonnen. Quellenautorität: [integrierter Brief](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/blob/d90e798dd9b26d3f152f3b0c2d1d52cae641eb16/PROJECT_CONTROL/WP-028_EXECUTION_BRIEF.md) und dessen Pflichtquellen einschließlich [ADR-032](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/blob/d90e798dd9b26d3f152f3b0c2d1d52cae641eb16/DECISIONS/ADR-032-season1-ci-korpus-leistungsnachweis.md). Diese Quellen werden nicht in den Kindbranch kopiert oder integriert.

Eigener verbindlicher [Production-Scope](../tools/architecture-validation/scopes/WP-028.production.scope.json). Gemeinsamer Erst-Add mit exakt einem Parent `d97c39d03f59a2dbc34930f14e3cb74ebeb2e118`; Manifestbasis identisch. Branch `codex/wp-028-season1-ci-performance`; Unter-PR ausschließlich gegen `codex/wp-026-season1-logical-content`.

Vorprüfung: Remote-fetch erfolgreich; origin/main exakt d90e798; origin/WP-026 und GitHub PR #21 exakt d97c39d, offen und Draft. Kein weiterer offener PR bei GitHub-Suche, kein WP-028-Remote-Branch und keine laufende parallele WP-028-Bearbeitung in der verfügbaren Chatübersicht; einziges lokales Worktree, sauberer Arbeitsbaum. Die tatsächlichen bestehenden CI-Jobmetadaten von Run 38495509881 bestätigen echte Lizenzaktivierung, Runnerguard und Unity-EditMode-Erfolg auf der Elternbasis. Neue CI auf WP-028 bleibt separat nachzuweisen.

Der abschließende erlaubte Dateiscope enthält CURRENT_STATE.md nicht. Diese Datei wird im WP-028-Kindbranch deshalb nicht geändert; projektweite Abschlussnachführung ist ein ausdrücklich getrennt zu verankernder Übergabepunkt. Kein Abschluss oder Selbstfreigabe wird behauptet.
