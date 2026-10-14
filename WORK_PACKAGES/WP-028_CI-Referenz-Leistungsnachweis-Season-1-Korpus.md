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

## Implementierung und lokale Vorprüfung – 14.10.2026

Status: Implementierung geliefert, CI und unabhängige QC noch offen; kein technischer Abschluss. Anker `0d17996819ed8ed346cbdf1d25739dd37b46be97`, einziger Parent/Manifestbasis `d97c39d03f59a2dbc34930f14e3cb74ebeb2e118`. Manifestdatei SHA-256 `419ac8e075d582fc099fa7def7610fa7fb48e254be7684ec63f03bf13abff4bb`; unverändert zu halten. Der erlaubte Diff umfasst Auftrag, Manifest, die einzelne Manifestvariable in validate.yml, den einen neuen Content-EditMode-Test und seine tatsächlich von Unity erzeugte .meta. Kein Korpus-, Produktions-, bestehender Test- oder unity.yml-Diff.

Der Test wählt den tatsächlichen Unity-Projektcheckout, fordert die vollständige kanonische 5×4×12-ID-/Pfadmenge, prüft solver-v2-/proofRef-Bindung und liest ausschließlich kanonische Level-/Proofbytes. Er misst die sieben echten 10×10-IDs nach je fünf Warm-ups mit je 50 vollständigen Solve-Aufrufen. Jede Klassifikation muss UNIQUE/1 mit SearchNodes/GuessDepth 0 sein. Danach liest er die gesamten Eingaben erneut und prüft dieselben Bytes durch frische Strict-ValidateOne-Aufrufe in vier festen Partitionen (MaxDegreeOfParallelism=4); Ausnahmen und sämtliche fehlenden/fehlerhaften Resultate werden ordinal ausgewertet und schlagen fehl. Import-/Release-Flags bleiben false. Eine einzige JCS-NUnit-Zeile enthält alle 350 Samples, die vier Metriken je gemessenem Input, Protokoll/Budgets, Commit, Korpusfingerprint und Laufzeitumgebung. Millisekunden stehen als invariante Round-trip-Dezimalstrings, damit das produktive Integer-only-JCS-DOM keine Präzision verliert.

Fingerprintprotokoll: alle 480 relativen Level-/Proofpfade gemeinsam ordinal sortieren; je Datei UTF-8-Pfadlänge, Pfadbytes, Dateibyteslänge und unveränderte Dateibytes in dieser Reihenfolge; Längen jeweils UInt64 big-endian; SHA-256 über den gesamten Stream. Lokaler Korpusfingerprint `29c16b2704125ea1f38dda74e1a9016e631c6f4ae9e19f3161d1009f4ee4b679`.

Lokaler echter Unity-Vorlauf: Unity 6000.3.23f1, Windows, erfolgreich lizenzaktiviert; vollständiger EditMode 455/455 Passed, 0 failed/skipped, Exitcode 0. Neue Testdauer 5,999181 s, alle 240 Proofgates bestanden. 350 Samples, p95-Index ceil(0,95×350)-1=332, p95 14,4855 ms, Maximum 232,7817 ms, vollständige frische Strict-Korpuszeit 970,2373 ms; alle Rohsamples aus dem XML separat sortiert und diese Werte nachgerechnet. XML SHA-256 `f207b0a91f7dae7dcfb77a8400f6ee760a0d05071f3cc8bafc6a90a575be5973`. Ausführung am Anker-HEAD mit noch uncommittiertem Implementierungsdiff; dies ist ausdrücklich nur lokale Vorprüfung und kein eingefrorener CI-Nachweis. Der von Unity erzeugte einzelne Whitespace-Diff in ShaderGraphSettings.asset wurde wieder byteinhaltlich zurückgenommen; kein Projektsettings-Scope.

Lokaler Windows-Validator: zunächst Sandbox-Path.resolve-Zugriff verhindert, danach außerhalb der Sandbox bekannter plattformabhängiger Selbsttest `V03-005-ABSOLUTE` FAIL (Windows wertet /tmp/scope.json anders als POSIX). Keine Validatoränderung. Linux-Vorprüfung nutzt vorhandenes WSL-Ubuntu mit Python 3.14.4/jsonschema 4.19.2 und verifiziertes Node 22.20.0; Ergebnisse werden vor Veröffentlichung separat geprüft. Fehlender Node wurde im ignorierten Scratch ergänzt. Ein Versuch der gelockten Linux-venv scheiterte an nicht installiertem ensurepip; die reguläre CI muss ihre gelockten Python-/Validatorabhängigkeiten selbst erfolgreich installieren.

## Prüf- und Integrationsübergabe

Auf dem nachfolgenden eingefrorenen Implementierungs-HEAD sind die echten regulären Push-Runs abzuwarten (PR-Trigger der unveränderten Workflows gilt nur für main). Pflicht: alle regulären Checks, unity-compile-editmode-Test-XML, genau eine WP028_CORPUS_PERFORMANCE-Zeile, Commit/Fingerprint, 350 Samples, Index 332, Budgets und volle 240 frische Proofgates selbst lesen/nachrechnen. Artefaktbytes hashen. Den HEAD danach durch Berichtskommentare/PR-Beschreibung dokumentieren statt durch rekursive Evidenzcommits zu verändern.

Unabhängige Astra-QC: Korpusbezug, tatsächliche IDs/Inputs, Produktgrenzen und falsche Freigabeschlüsse. Unabhängige Sol-QC: gemeinsamer historischer Anker/Manifestbytes, abschließender Dateiscope, unveränderter CI-Weg, Messprotokoll und ausschließlich kindliche Integrationsrichtung. Beide auf exakt demselben eingefrorenen HEAD, keine Selbstfreigabe. Bei Findings nur innerhalb WP-028 korrigieren, neu einfrieren und erforderliche CI/QC erneut auf dem neuen HEAD. Kein bestehender HardBudget-Test wird repariert, umgangen oder wiederholt.

Erst nach AK-01 bis AK-08 und beiden unabhängigen Prüfungen ohne offenen BLOCKER/HIGH: Unter-PR ausschließlich in WP-026 integrieren, mit Expected-HEAD-Absicherung; danach reguläre Folgechecks auf dem neuen WP-026-Head lesen. PR #21 bleibt offen und benötigt eine neue eigenständige Gesamtprüfung. Menschlicher Spieltest, historischer Unity-Timeoutbefund, 1.440 Referenzziele, Zeitklassen und Cross-Reference-/Import-/Produkt-/Releasegates bleiben offen. Die auf main integrierten WP-027-Produktklarstellungen gelten unabhängig davon; ihre Dateien/ADRs werden in WP-028 nicht übernommen.

## Lokaler Prüfabschluss vor CI-Freeze

Architecture-only mit --self-test im vorhandenen Linux: **PASS, 17 lokale Prüfgruppen**. WP-028-Production-Scope mit --self-test auf `b5eb8b2d766a31ca48cddb25fc4931f69bf6496e`: **PASS, 18 lokale Prüfgruppen**, sauberer Arbeitsbaum. Der lokale main-Ref wurde nach geprüfter Fast-forward-Elternschaft ausschließlich auf den bereits vorhandenen Remote-main-Quellenstand d90e798 nachgeführt; keine Integration eines WP-028-Commits nach main. WSL-Git erhielt nur pro Prozess core.autocrlf=true entsprechend dem Windows-Checkout; der unveränderte Validator lief vollständig. Die vorigen umgebungsbedingten FAILs (Windows-POSIX-Selbsttest, fehlender Node, kurzzeitiger Unity-Whitespace und veralteter lokaler main-Zeiger) bleiben hier als tatsächliche Vorläufe dokumentiert; kein Test/Validator wurde geändert.

Git diff --check gegen d97c39d PASS. Vollständiger getrackter/untracked Scope geprüft: exakt die fünf erlaubten Pfade; Level-, Proof-, Review-, Solver-, Pipeline-, bestehende Test- und unity.yml-Bäume identisch zum Parent. Gemeinsamer Anker/Parent/Manifestlink bestätigt, Manifest unverändert. Fingerprint zusätzlich mit unabhängigem Python-Byteframing über 240 Quellen und 240 Proofs nachgerechnet: `29c16b2704125ea1f38dda74e1a9016e631c6f4ae9e19f3161d1009f4ee4b679`.

Der nachfolgende reine Dokumentationscommit friert den Lieferstand für echte CI und beide unabhängigen Rollen ein. CI/QC-Berichte und nachgerechnete Artefaktnachweise werden am Unter-PR persistiert, ohne dessen geprüften Head nachträglich zu verschieben. AK-01 bis AK-06 sind implementiert und lokal vorgeprüft; AK-07 und AK-08 bleiben bis zur tatsächlichen finalen CI/QC offen. Integration und technische DoD bleiben bis dahin offen.

## Eigene CI-Korrektur nach dem ersten Freeze

Der erste echte Push-CI-Freeze c27fee29f641e2a877cabfaaa3b2d9afbe3a9a7e bestand Architecture, scheiterte aber tatsächlich in Unity Run38629629717 Attempt1, EditMode Job116077773047. Originalartefakt11925358492: ZIP429222 Bytes/SHA25678c8db83501d874762f5838e85769c3cc09f3b7233969ace7ad848789f8c263f; Original-XML SHA2560172e37b409d0e8d1390c3a48ca8c5f18e12df1b4703bd0db740a377b51d4c9b. XML selbst gelesen: 455gesamt, 454Passed, 1Failed, 0skipped. Ausschließlich der neue Test scheiterte vor Messung mit CheckoutCommit/Git-Exit128. Daher keine Leistungswerte, kein AK-07-PASS und keine Integration auf c27. Die nachfolgenden PlayMode-/Android-/iOS-Jobs wurden skipped; sämtliche sechs Vorbereitungs-/Umgebungsjobs bestanden. Vollständiger CI-FAIL und Korrekturbegründung sind am [PR25-Kommentar](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/25#issuecomment-6177538765) persistiert. Decodiertes UTF8-EditMode-Joblog SHA256c0fc5d335454c9334f9a5c7cccc654199448fbc6833aac7e5ce193ea16103c56, kein Artefaktbytehash.

Die eigene Reparatur bleibt im neuen Testpfad: CheckoutCommit setzt ausschließlich für seinen read-only-Git-Unterprozess safe.directory exakt auf den bereits aus Unity ausgewählten und in CI gegen GITHUB_WORKSPACE gebundenen Checkoutroot; GIT_CONFIG_COUNT/KEY_0/VALUE_0 ändern keine Repo-/Globalconfig. stderr wird jetzt im Fehlertext ausgewiesen. HEAD/GITHUB_SHA-Bindung bleibt unverändert. Gitvertrauen als Ursache ist ohne ursprünglichen stderr eine dokumentierte Umgebungsinferenz, kein nachträglicher exakter Originalfehlertext. Isolierte lokale Linuxprobe mit GIT_TEST_ASSUME_DIFFERENT_OWNER=true und ohne Global-/Systemconfig reproduzierte Exit128/dubious ownership; derselbe Prozess mit ausschließlich diesem safe.directory lieferte korrekt c27. Kein Workflow-, Runner-, Timeout-, Mess-/Budget- oder bestehender Testdiff; kein Retry des fehlgeschlagenen Runs.

Erneute echte lokale Unity6000.3.23f1-Vorprüfung nach Codeänderung: 455/455Passed, 0Failed, Exit0; XMLSHA256cb32e0f0b1b4967ed640a2157862fe279fb25e6b9f6205cebb7a2cf12f64b329. Neuer Test 240Proofgates/350Samples, p95 9,5168ms, Maximum26,5193ms, Vollkorpus913,8666ms, gleicher Fingerprint29c16b2704125ea1f38dda74e1a9016e631c6f4ae9e19f3161d1009f4ee4b679. Wiederum lokale Vorprüfung bei c27 mit uncommittiertem Korrekturdiff, kein CI-Ersatz. Der beim Unityimport wieder erzeugte einzelne ShaderGraph-Whitespace wird vor Commit zurückgesetzt.

Der folgende Korrekturcommit ist der neue finale Freeze. Architecture/Scope, reguläre Unity-CI und beide unabhängigen Rollen müssen auf genau ihm erfolgreich nachgewiesen werden; vorherige Teilnachweise ersetzen diese neuen Nachweise nicht. Kein alter FAIL wird verdeckt oder als bestanden erklärt.
