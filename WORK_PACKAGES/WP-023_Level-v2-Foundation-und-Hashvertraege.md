# WP-023 – Level-v2-Foundation und Hashverträge

## ID

`WP-023`

**Bearbeitungsstatus:** **Phase A abgeschlossen: definiert, ADR-030-konform verankert und durch die kanonische Governance-/Scope-/Trust-CI erfolgreich geprüft. Implementierung noch nicht begonnen; bereit für separaten Phase-B-Implementierungsauftrag.**

[Scope](../tools/architecture-validation/scopes/WP-023.production.scope.json)

## Ziel

Auf einer frischen Branch vom aktuellen `main` wird nach dem integrierten WP-022-Puzzle-Kern die nächste fachliche Produktionsstufe hergestellt: die belastbare Level-v2-Datenbasis mit strengem Parser/DTO, deterministischer Abbildung auf die vorhandene `PuzzleDefinition`, JSON-Kanonisierung sowie den verbindlichen Content-/Public-Puzzle-Hashverträgen.

Die vorhandene `WP-015_Level-v2-Foundation-und-Hashvertraege.md` bleibt fachliche Planungsgrundlage. Sie liegt bereits auf `main` und kann daher gemäß ADR-030 nicht selbst nachträglich der gemeinsame Erstanker mit einem neuen Scope-Manifest sein. WP-023 macht diesen fachlichen Block erstmals ausführbar.

WP-023 endet ausdrücklich vor solver-v2-Metriken/Rootspur, Proofregeneration/Strict-Validation, Generator/Authoring und konkreter Contentproduktion.

## Voraussetzungen

1. Die Lesereihenfolge aus `AGENTS.md` ist vollständig einzuhalten. Danach sind mindestens `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md`, dieses Work Package, die WP-015-Planungsfassung, `ARCHITECTURE/LEVEL_DATA_FORMAT.md`, `ARCHITECTURE/CONTENT_PIPELINE.md`, `ARCHITECTURE/PUZZLE_ENGINE.md`, `ARCHITECTURE/MODULE_BOUNDARIES.md`, `ARCHITECTURE/TEST_STRATEGY.md`, ADR-021, ADR-026, ADR-030 und ADR-031 zu lesen.
2. WP-022 ist abgeschlossen und integriert. Ausgangsbasis dieses Auftrags ist `main` bei `ad0ac1b6b12f9c0acbf59d90734b3c574a2a15d7`.
3. Dieses Work Package und sein [Production-Scope-Manifest](../tools/architecture-validation/scopes/WP-023.production.scope.json) werden gemeinsam erstmals im selben ersten Branchcommit eingeführt. Dessen einziger Elterncommit und der `baseCommit` des Manifests sind exakt `ad0ac1b6b12f9c0acbf59d90734b3c574a2a15d7`.
4. Das WP-023-Manifest ist nach diesem gemeinsamen Add-Commit byteunveränderlich.
5. Die vorhandene WP-015-Datei ist ausschließlich Planungs-/Vergleichsquelle und wird nicht als eigener Trust Anchor behandelt.

## Scope

### Phase A – Definition und Verankerung

1. Dieses Work Package und das eigene Production-Scope-Manifest gemeinsam im ersten Branchcommit anlegen.
2. Danach `CURRENT_STATE.md` und `WORK_QUEUE.md` mechanisch auf WP-023 als aktuellen Produktionsblock nachführen und in `validate.yml` ausschließlich die Production-Scope-Manifestzuordnung auf WP-023 umstellen.
3. Historischen Anker, Elternbasis, Manifestbytes, Scopegrenzen und vollständigen Branchdiff prüfen; Architecture-/Production-Scope-Selftests in der vorgesehenen CI ausführen.
4. Noch keine Produktions-, Test-, Schema-, Architektur- oder Unity-CI-Implementierung in Phase A.

### Phase B – Implementierung nach separatem Auftrag

1. **Level-v2-Vertrag:** bestehenden Level-v2-Vertrag gemäß `LEVEL_DATA_FORMAT.md` konsistent umsetzen; `schemaVersion`, `ruleset`, Raster, Randzahlen, Endpoints, `publicPuzzleHashProfile`, Hashfelder und erlaubtes Proof-Pass-through eindeutig und fail-closed behandeln.
2. **Parser/DTO:** Level-v2 JSON strikt und deterministisch parsen; unbekannte, fehlende, falsch typisierte oder ungültige Werte typisiert abweisen; keine stillen Defaults oder toleranten Fallbacks.
3. **Semantikadapter:** aus gültigen Level-v2-Daten exakt die integrierte `PuzzleDefinition` erzeugen. Die Puzzle-Regeln werden nicht dupliziert oder verändert; Domainvalidierung bleibt autoritativ.
4. **JCS-Kanonisierung:** die für Hashprojektionen benötigte JSON-Kanonisierung deterministisch nach dem bindenden Projektvertrag implementieren und mit Golden-/Negativfällen absichern.
5. **Hashverträge:** Content-Hash und Public-Puzzle-Hash exakt nach ihren vorgesehenen Projektionen/Profile berechnen; Projektionen nicht vermischen; bytegenau reproduzierbar.
6. **Migration:** Level-v1 → Level-v2 nur soweit im bestehenden Vertrag vorgesehen, deterministisch und ohne Produkt-/Contententscheidung; keine Proofregeneration oder solver-v2-Aufwertung vorwegnehmen.
7. **Schema/Dokumentation/Beispiele:** `LEVEL_DATA_FORMAT.md`, `CONTENT_PIPELINE.md`, `level-v2.schema.json` und die zwei bestehenden Level-v2-Beispiele nur soweit erforderlich mit der realen Implementierung konsistent halten. Kein proof-v1-Schema ändern.
8. **Tests:** Content-EditMode-Tests für Parser, Schema-/Semantikgrenzen, JCS, Hashprojektionen, Migration, Determinismus und Negativfälle.
9. **CI/Evidenz:** echte Unity-Compile-/EditMode-/Regressionsevidenz sowie bestehende Build-/QA-Gates erhalten. `unity.yml` nur soweit ändern, wie für die tatsächliche Ausführung/Nachweisführung des bestehenden Content-Testmoduls erforderlich; keine Gateabsenkung.
10. **Übergabe:** Ergebnisse, Tests, Einschränkungen und nächsten Schritt in diesem WP sowie `CURRENT_STATE.md`/`WORK_QUEUE.md` persistieren.

## Betroffene Dateien/Module

Die unveränderliche technische Außengrenze ist das [WP-023-Production-Scope-Manifest](../tools/architecture-validation/scopes/WP-023.production.scope.json). Inhaltlich vorgesehen sind ausschließlich:

- `WORK_PACKAGES/WP-023_Level-v2-Foundation-und-Hashvertraege.md`
- `tools/architecture-validation/scopes/WP-023.production.scope.json`
- `PROJECT_CONTROL/CURRENT_STATE.md`
- `PROJECT_CONTROL/WORK_QUEUE.md`
- `.github/workflows/validate.yml`
- `.github/workflows/unity.yml`
- `ARCHITECTURE/LEVEL_DATA_FORMAT.md`
- `ARCHITECTURE/CONTENT_PIPELINE.md`
- `ARCHITECTURE/schemas/level-v2.schema.json`
- `ARCHITECTURE/examples/level-v2.example.json`
- `ARCHITECTURE/examples/level-v2.single-cell.example.json`
- bestehende Assembly `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Content/`
- bestehende Testassembly `Assets/StammstreckenPuzzle/Tests/EditMode/Content/`

Keine neuen Assemblies oder Abhängigkeiten. Bestehende `.asmdef`/`csc.rsp` nur dann ändern, wenn die reale Implementierung dies innerhalb des bereits beschlossenen Modulvertrags zwingend erfordert.

## Ausdrücklich nicht erlaubte Änderungen

- Keine Änderung des WP-023-Production-Scope-Manifests nach dem Trust-Anchor-Commit.
- Keine Änderung des fachlichen Puzzle-Kerns aus WP-022 außer Nutzung seiner öffentlichen Verträge.
- Keine solver-v2-Kennung, Rootspur, ADR-031-Metrikimplementierung oder Schwierigkeitsmetriken.
- Keine Proof-v1-Regeneration, Proofgeneratoren, Strict-Validation-Pipeline oder Release-Lock-Freigabe.
- Keine Generator-/Authoringfunktion, Season-1-Level, Zeitkalibrierung oder Contentproduktion.
- Keine UI-, Bootstrap-, Save-, Reward-, Hint-, Daily-, Store-, Analytics- oder Werbelogik.
- Keine neue Architektur- oder Produktentscheidung; Widersprüche werden als Blocker dokumentiert.
- Kein Merge, Cherry-Pick, Rebase oder Copy aus historischen Implementierungsbranches; keine Übernahme historischer PASS-Aussagen.
- Keine beiläufigen Refactorings außerhalb des Work-Package-Scopes.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| `AK-01` | WP-023 und eigenes Manifest sind im ersten Branchcommit gemeinsam als neue Dateien verankert; Elterncommit/baseCommit ist exakt `ad0ac1b6b12f9c0acbf59d90734b3c574a2a15d7`; historischer WP-Link zeigt auf genau dieses Manifest; Manifest bleibt byteunverändert und remote nachweisbar. |
| `AK-02` | Level-v2-Parser akzeptiert ausschließlich den verbindlichen Vertrag und weist unbekannte, fehlende, falsch typisierte oder semantisch ungültige Daten fail-closed ab. |
| `AK-03` | Gültige Level-v2-Daten werden deterministisch auf die integrierte `PuzzleDefinition` abgebildet; Puzzle-Regeln werden nicht dupliziert oder verändert. |
| `AK-04` | JCS-/Kanonisierungsverhalten ist bytegenau deterministisch und durch positive sowie negative Golden-/Crosschecks belegt. |
| `AK-05` | Content-Hash und Public-Puzzle-Hash verwenden exakt ihre vorgesehenen Projektionen/Profile und sind über unabhängige Testvektoren reproduzierbar. |
| `AK-06` | Eine vorgesehene v1→v2-Migration ist deterministisch und erzeugt ausschließlich schema-/semantikgültige v2-Daten. |
| `AK-07` | Schema, Dokumentation, Beispiele und reale Implementierung widersprechen sich nicht; Proofdaten bleiben nur im erlaubten Pass-through-Bereich, ohne den späteren Proof-Block vorwegzunehmen. |
| `AK-08` | Content-EditMode-Tests decken Parser, Semantik, JCS, Hashgrenzen, Migration, Determinismus und Negativfälle belastbar ab; vorhandene WP-022-Regressionen bleiben grün. |
| `AK-09` | Vorgeschriebene Governance-/Scope-/Trust-/Secret-/Diffprüfungen und reale Unity-/CI-Nachweise sind PASS; keine Änderung außerhalb des Manifests und keine Gateabsenkung. |

## Tests

1. Beide kanonischen Architecture-/Production-Scope-Validatorläufe inklusive Self-/Negativtests in der vorgesehenen Ubuntu-CI.
2. Trust-Anchor-/Manifest-/Basis-/Ancestry-/vollständiger Diffnachweis.
3. Unity-Compile und Content-EditMode-Tests; bestehende Puzzle-/Bootstrap-Regressionen dürfen nicht verschlechtert werden.
4. Parser-/Semantik-Positiv- und Negativkatalog einschließlich unbekannter Felder, Typfehler, Grenzwerte und ungültiger Domainabbildungen.
5. Bytegenaue JCS-/Hash-Golden- und Gegenproben einschließlich unterschiedlicher Feldreihenfolgen und äquivalenter Eingabedarstellungen, soweit der Vertrag dies vorsieht.
6. Migrations-Golden-/Negativfälle und, soweit vertraglich verlangt, Idempotenz.
7. Bestehende Android-/iOS-/QA-Nachweise gemäß projektweiter DoD, soweit das Work Package sie verlangt; kein nicht ausgeführter Lauf wird als PASS dargestellt.
8. Vor Integration eine unabhängige Abschluss-QC auf dem eingefrorenen finalen PR-HEAD.

## Risikoklasse

**Hoch.** Levelidentität, Hashes und deterministische Datenabbildung sind Grundlage für spätere Solver-v2-, Proof-, Katalog- und Releaseverträge.

## Definition of Done

Es gilt `PROJECT_CONTROL/DEFINITION_OF_DONE.md` vollständig. WP-023 gilt erst als abgeschlossen, wenn alle anwendbaren Akzeptanzkriterien mit tatsächlichen Nachweisen erfüllt sind, keine offenen BLOCKER/HIGH im Scope bestehen, die Abschlussdokumentation persistiert ist, eine unabhängige Abschluss-QC auf dem eingefrorenen finalen PR-HEAD PASS meldet und die Geschäftsführung die Integration freigibt.

Phase A allein endet mit: **definiert, ADR-030-konform verankert und bereit für einen separaten Implementierungsauftrag**. Sie ist keine technische Fertigstellung von WP-023.

## Rollen und Übergabe

- Geschäftsführung/Projektkoordination geben Implementierung und Integration frei.
- Ein beliebiger geeigneter Implementierungsagent darf nach separatem Auftrag ausschließlich innerhalb dieses Work Packages arbeiten.
- Die Abschlussprüfung erfolgt unabhängig vom Implementierungsagenten auf dem eingefrorenen finalen Stand; das konkrete KI-System ist austauschbar.
- Jeder Agent persistiert Status, geänderte Dateien, Tests, Einschränkungen/Blocker und nächsten Schritt nach `AI_HANDOVER_RULES.md`.
- Chatverlauf und Modellidentität sind keine Projektquelle.

## Herkunft / Abgrenzung zu WP-015

`WP-015_Level-v2-Foundation-und-Hashvertraege.md` bleibt als vorab angelegte Planungsfassung lesbar. Die fachliche Absicht wird nur soweit übernommen, wie sie mit dem heutigen `main`, den integrierten WP-021/WP-022-Ergebnissen und den aktuell bindenden ADRs übereinstimmt. WP-023 ist der erste ausführbare, historisch korrekt verankerte Auftrag für diesen Block.

## Phase-A-Nachweis – 2026-10-09

- Verifizierte Ausgangsbasis: `main` `ad0ac1b6b12f9c0acbf59d90734b3c574a2a15d7`.
- Verbindlicher Trust Anchor: `a1d284caec7746a4aa48fbe564d37d179d60fed8`; exakt dieses Work Package und das eigene Production-Scope-Manifest wurden gemeinsam im ersten Branchcommit eingeführt. Der historische WP-Text verlinkt das Manifest ausdrücklich; `baseCommit` entspricht dem einzigen Elterncommit.
- Das Production-Scope-Manifest ist seit dem Anker unverändert.
- Phase-A-Folgediff beschränkt sich auf `CURRENT_STATE.md`, `WORK_QUEUE.md` und die Manifestzuordnung in `.github/workflows/validate.yml`; keine Produktions-, Test-, Schema-, Architektur- oder Unity-CI-Implementierung.
- Kanonische GitHub-PR-CI auf Head `b7bf1c845a660afdee7d0bb9f9cb13d4775ae61c`: Architecture Validation Run `37984954679`, Job `114004511519`, **COMPLETED / SUCCESS**. Architecture-only-Lauf mit Self-/Negativtests und kanonischer WP-023-Production-Scope-Lauf mit Self-/Negativtests jeweils erfolgreich.
- Der zunächst vor Produktionsbeginn angelegte fehlerhafte Phase-A-Anker wurde verworfen, nachdem die fail-closed Prüfung einen fehlenden historischen Markdown-Link zwischen WP und Manifest korrekt beanstandet hatte. Zu diesem Zeitpunkt existierte keinerlei WP-023-Produktionsimplementierung; die aktuelle Branchhistorie enthält ausschließlich den korrigierten gültigen Anker.
- Ergebnis: **Phase A abgeschlossen. Kein Definitions-, Scope- oder Trust-Blocker. WP-023 ist bereit für einen separaten Implementierungsauftrag.**
