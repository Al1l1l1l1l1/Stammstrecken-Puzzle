# ADR-032 – CI-Referenz-Leistungsnachweis für den Season-1-Korpus

## Status

**Angenommen**

## Datum

2026-10-14

## Kontext

Der logische Season-1-Bestand in WP-026 enthält auf seinem Draft-Stand 240 konkrete Levelquellen und gebundene `solver-v2`-Proofs. Für die sieben tatsächlichen 10×10-Inputs und für den gesamten 240er-Korpus fehlt jedoch der geforderte Leistungsnachweis auf dem CI-Referenzweg.

Der bestehende Test `TenByTen_ForcedPaths_AreDeductive_FullyAudited_AndWithinTheTraceInclusiveBudget` misst nur fünf ausdrücklich als `FIXTURE_ONLY` markierte Zwangspfade. Er ist kein Nachweis für kuratierte Season-1-Daten. Autorbetriebene lokale Messungen sind nützliches Diagnosematerial, aber wegen abweichender Hardware, Toolchain und Nachweiskette kein Ersatz für CI.

Der verbindliche Budgetvertrag bleibt unverändert:

| Operation | Bestehende Grenze |
|---|---:|
| Lösung zählen bis zwei, kuratiert 10×10 | p95 < 250 ms, Maximum < 2 s |
| gesamter Season-1-Katalog in CI | < 60 s, parallelisiert und deterministisch ausgewertet |

Die Entscheidung präzisiert ausschließlich den fehlenden Ausführungs- und Evidenzweg. Sie ändert weder `solver-v2`, `proof-v1`, Metriksemantik, Qualitätsmindestwerte noch die noch offenen Content-, Import- oder Releasegates.

## Entscheidung

### 1. Verbindlicher Referenzweg

Der Nachweis wird als zusätzlicher EditMode-Test im bestehenden `unity-compile-editmode`-Job erbracht. Maßgeblich ist der tatsächlich ausgeführte Job auf einem eingefrorenen Pull-Request-Head mit dem bereits verbindlichen Runnervertrag:

- GitHub Actions `ubuntu-24.04`;
- gepinnter Linux-Unity-Container aus `.github/workflows/unity.yml`;
- Unity `6000.3.23f1`;
- vorhandene, echte Unity-Lizenzaktivierung;
- aktuelle kanonische Quellen und Proofs aus genau diesem Checkout.

Es wird **kein** separater manueller `workflow_dispatch`-Sonderpfad, kein lokaler Ersatzbeleg und kein ungebundener Profilinglauf geschaffen. Der normale EditMode-Job archiviert sein Testresultat bereits; dessen Run, Commit und Test-XML sind die Nachweiskette.

### 2. Korpus und Testprotokoll

WP-028 implementiert genau einen neuen Content-EditMode-Test mit folgendem geschlossenen Protokoll:

1. Er lädt ausschließlich die kanonischen `Content/Levels/s1/**`-Levelquellen und ihre über `proofRef` referenzierten kanonischen `Content/Proofs/**/solver-v2.proof-v1.json`-Proofs aus dem ausgecheckten Repository.
2. Er fordert exakt die vollständige Season-1-Menge `5 × 4 × 12 = 240` eindeutiger IDs. Fehlende, doppelte, nicht lesbare oder nicht an `solver-v2` gebundene Eingaben lassen den Test fail-closed fehlschlagen.
3. Er führt für jeden Korpuseintrag die vorhandene Produktions-`LevelValidationPipeline` mit frischer Solver-/Proofprüfung und unveränderter Strict-Semantik aus. Jeder Eintrag muss den technischen Proofgate bestehen; spätere Cross-Reference-, Produkt- oder Releasefreigaben werden dadurch nicht vorweggenommen.
4. Er misst die sieben tatsächlichen 10×10-Season-1-Level `S1-05-04-06` bis `S1-05-04-12` direkt mit dem Produktionssolver aus ihrem öffentlichen Puzzleinput. Nach fünf nicht gemessenen Warm-ups je Level folgen 50 gemessene Läufe je Level, insgesamt 350 Samples. Jeder Lauf muss `UNIQUE`, `searchNodes = 0` und `requiredGuessDepth = 0` liefern. p95 ist das geordnete Sample an Index `ceil(0,95 × n) − 1`; Maximum ist der größte Einzelwert. Beide bestehenden Grenzen müssen erfüllt sein.
5. Die frische technische Validierung aller 240 Quellen erfolgt mit fester Parallelität `MaxDegreeOfParallelism = 4`, unabhängigen Eingabesnapshots und ordinal sortierter Ergebnisauswertung. Ihre gesamte Wall-Clock-Zeit muss unter 60 Sekunden liegen. Ein Timeout, eine Ausnahme, ein unbestimmtes Solverergebnis oder ein nicht bestandener Eintrag ist Fehler, niemals ein übersprungener Datensatz.
6. Der Test schreibt genau eine maschinenlesbare, stabile Ergebniszeile in den NUnit-Output: Protokollversion, GitHub-SHA/Commit, Solver-/Proofformatversion, Anzahl/IDs der 10×10-Level, Korpusfingerprint über ordinal sortierte relative Level- und Proofpfade samt Bytes, Warm-up-/Sample-/Parallelitätsparameter, p95, Maximum, Gesamtdauer sowie Unity-/Runtime-/OS-/Prozessorinformation. Die Werte selbst werden nicht als eingecheckte Golden- oder Freigabequelle gespeichert.

Änderungen an Korpusauflösung, Messalgorithmus, Warm-up-/Samplezahl, Parallelitätsgrad, Budgets oder Referenzumgebung ändern den Nachweisvertrag und benötigen einen nachfolgenden ADR.

### 3. Ausführungs- und Integrationsgrenze

Der Test ist ein technisch isolierter Nachweis als **WP-028**:

- Sein gemeinsamer Work-Package-/Manifest-Anker entsteht als eigener, kindlicher Branch von `d97c39d03f59a2dbc34930f14e3cb74ebeb2e118`, dem gegenwärtigen WP-026-PR-Head.
- Die Unter-PR zielt ausschließlich auf `codex/wp-026-season1-logical-content`, nicht auf `main`. Nach unabhängiger Prüfung darf sie nur in diesen offenen WP-026-Lieferzweig integriert werden.
- WP-027 wird getrennt nach `main` integriert. Beim späteren möglichen Merge von PR #21 enthält der resultierende Integrationsstand sowohl die Produktklarstellungen aus WP-027 als auch den WP-028-Leistungsnachweis.
- WP-028 verändert keine Level-/Proofquellen, keine Content-Bible, keine Solver-/Pipelineproduktion und keinen bestehenden `HardBudget`-Timeouttest. Der dokumentierte Unity-Push-Timeout bleibt ein eigenständiger WP-026-Befund und wird weder als bestanden erklärt noch durch Retry, Quarantäne oder Budgeterhöhung verdeckt.

## Begründung

Die Content-Bible und die Schwierigkeitsspezifikation versprechen kuratierte, faire Kampagnenlevel mit einer nachvollziehbaren Denkstruktur. ADR-007 verlangt für größte vorgesehene Raster einen CI-Leistungsnachweis; der Solververtrag konkretisiert dessen Budgets. Die neue Messung prüft daher die tatsächlichen Season-1-Eingaben im vorhandenen Produktionsvalidierungspfad statt vereinfachte Fixtures oder lokale Ersatzwerte zu verwenden.

Der geschichtete Branchweg respektiert gleichzeitig die Grenzen beider noch offenen Pakete: WP-026 bleibt Eigentümer seiner 240 Quellen; WP-027 bleibt eine Entscheidung auf `main`; WP-028 kann seinen Code nach eigenem Trust Anchor gegen den unveränderten tatsächlichen Korpus prüfen. Die Prüfung ist damit reproduzierbar, fail-closed und ohne nachträgliche Änderung des bisherigen WP-026-Scopes möglich.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Bestehende 10×10-Fixtures als Season-Nachweis deklarieren | Abgelehnt: Fixtures sind ausdrücklich nicht kuratierter Season-1-Content. |
| Lokale Autorlaufzeiten übernehmen | Abgelehnt: keine CI-Referenz, keine gleichwertige Toolchain-/Commitbindung. |
| Manuellen Workflow mit frei wählbaren Parametern hinzufügen | Abgelehnt: erzeugt einen optionalen, schlechter prüfbaren Sonderpfad. |
| Budget nur gegen den schnellsten Einzelwert messen | Abgelehnt: p95 und Maximum sind bewusst beide Teil des bestehenden Vertrags. |
| Das 240er-Ergebnis als Import-/Releasefreigabe werten | Abgelehnt: Cross-References, Produktstatus, Human Review, Spieltest, Geräte- und Releasegates bleiben unabhängige Voraussetzungen. |
| Den bestehenden Unity-Timeouttest lockern oder wiederholen | Abgelehnt: betrifft einen getrennten, ungeklärten Befund und wäre keine Korpusleistungsprüfung. |

## Konsequenzen

- Der fehlende `CI-PERF-WP026`-Nachweis besitzt einen eng begrenzten, ausführbaren und revisionsgebundenen Weg.
- Ein grüner WP-028-Test belegt ausschließlich die technische Solver-/Proofleistung auf dem genannten Korpus und Referenzjob; er beendet weder WP-026 noch dessen unabhängige fachliche Abnahme.
- Die späteren Pflichtgates für Cross-References, Lokalisierung, Doppelimport, Addressables, Lizenzen, Produktfreigabe, Geräte, Store und Release bleiben unverändert.
- Ein fehlgeschlagener Lauf ist ein echter Befund des gebundenen Commits. Er wird dokumentiert und innerhalb eines passend beauftragten Scopes untersucht; keine implizite Flaky-Ausnahme.

## Betroffene Artefakte

- `ARCHITECTURE/SOLVER_ARCHITECTURE.md`
- `.github/workflows/unity.yml`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Content/Season1CorpusPerformanceTests.cs` (neu durch WP-028)
- `WORK_PACKAGES/WP-028_CI-Referenz-Leistungsnachweis-Season-1-Korpus.md`
- `tools/architecture-validation/scopes/WP-028.production.scope.json`
- WP-026 / Draft-PR #21 als Lieferzweig der Korpusdaten

## Validierung

Die Entscheidung gilt als technisch umgesetzt, wenn WP-028 seinen gemeinsamen Trust Anchor korrekt nachweist, der neue Test ausschließlich reale Season-1-Quellen verwendet und ein CI-`unity-compile-editmode`-Run auf dem eingefrorenen WP-028-Head alle bestehenden EditMode-Tests einschließlich der neuen Ergebniszeile besteht. Zwei unabhängige QC-Rollen prüfen danach getrennt den Produkt-/Korpusbezug und Scope-/CI-/Ankerbezug.

## Ersetzt / ersetzt durch

Ersetzt keinen ADR. Ergänzt ADR-007 im dort offen gelassenen CI-Leistungsnachweis sowie den bestehenden Budgetvertrag in `SOLVER_ARCHITECTURE.md` Abschnitt 10. Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: ./ADR-007-solver-und-eindeutigkeitspruefung.md "ADR-007 – Deterministischer Constraint-Solver mit Lösungslimit zwei"
[2]: ../ARCHITECTURE/SOLVER_ARCHITECTURE.md "Solver Architecture"
[3]: ../ARCHITECTURE/CONTENT_PIPELINE.md "Content Pipeline"
[4]: ../ARCHITECTURE/BUILD_AND_RELEASE.md "Build and Release"
[5]: ../Stammstrecken_Puzzle_Konzept_00-15/04_Schwierigkeit_Feldgroessen_und_Levelgenerierung.md "Schwierigkeit, Feldgrößen und Levelgenerierung"
[6]: ../Stammstrecken_Puzzle_Konzept_00-15/14_Season_1_Content_Bible.md "Season-1-Content-Bible"
[7]: https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/21 "Draft-PR #21 – WP-026-Lieferzweig"
[8]: ../PROJECT_CONTROL/WP-028_EXECUTION_BRIEF.md "Verbindlicher Ausführungsbrief für WP-028"
