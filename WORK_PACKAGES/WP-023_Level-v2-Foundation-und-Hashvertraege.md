# WP-023 – Level-v2-Foundation und Hashverträge

## ID

`WP-023`

**Bearbeitungsstatus:** **Abgeschlossen und integriert. Die erste unabhängige Abschluss-QC auf `182db13af5f8e2d1db3acadc1318153575baac85` endete mit FAIL (vier Befunde). Die vier Befunde wurden eigenständig reproduziert und im Scope von WP-023 behoben. Die erneute unabhängige Abschluss-QC auf dem eingefrorenen finalen PR-HEAD `f56eab34dca0346d0e339a1c5cde650e15955f68` meldete PASS ohne integrationsblockierenden Befund; AK-01 bis AK-09 wurden als erfüllt bewertet. Die Geschäftsführung erteilte die Integrationsfreigabe. PR #15 wurde als Merge-Commit `10405370967496f51be9e5c00c381fd8052ec072` in `main` integriert.**

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

## Phase-B-Nachweis – 2026-10-09

### Stand

- Branch `codex/wp-023-level-v2-foundation`, Draft-PR #15. Implementierungsstand: Commit `4e5fbd327cacd021a35f8a8c839fb69f92dbe9f2` auf `d59bd394…` (Phase A); danach `90c4703…` (Dokumentation) und `716e9fb3c1e23103e65ed9c6dc382437c228b05c` (Ein-Zeilen-Korrektur eines Test-Constraints, siehe unten). Code-Stand mit vollständigem CI-Nachweis ist `716e9fb…`. Alle späteren Commits ändern ausschließlich Dokumentation (`WP-023`, `CURRENT_STATE.md`, `WORK_QUEUE.md`) und lassen sämtliche Unity-relevanten Eingaben (Assets, Workflows, Schemas, Beispiele) unberührt.
- Trust Anchor `a1d284caec7746a4aa48fbe564d37d179d60fed8` und Basis `ad0ac1b6b12f9c0acbf59d90734b3c574a2a15d7` unverändert. Das Manifest hat SHA-256 `a8a3a5068bf6b390ad3b44b5e6d8ec36f2d93f9d849959c9230620880a5776b0` und ist am Anker, im lokalen Branch und im Remote-Stand byteidentisch.
- PR #15 ist nicht gemergt und bleibt Draft.

### Funktional umgesetzt (`Assets/StammstreckenPuzzle/Scripts/Infrastructure/Content/`)

| Baustein | Aufgabe |
|---|---|
| `StrictJsonParser`, `JsonValue` | Strikter, nie werfender Parser: BOM, ungültiges UTF-8, Kommentare, nachlaufende Kommas/Inhalte, doppelte Schlüssel, Float/Exponent, `-0`, Zahlen außerhalb ±(2^53−1), lone Surrogates, Steuerzeichen; Grenzen 256 KiB / Tiefe 16 / String 8192 / Array 1024 / Objekt 64. |
| `LevelV2Contract`, `LevelSchemaReader`, `LevelV2Reader`, `LevelV2Document` | Geschlossener Strukturvertrag (spiegelt `level-v2.schema.json`), typisierte DTOs, keine stillen Defaults, jede Abweichung genau eine stabile Diagnose (`LVL-VERSION-*`, `LVL-SCHEMA-*`). |
| `LevelV2DomainMapper`, `LevelV2Semantics` | Deterministische Abbildung auf die vorhandene `PuzzleDefinition` über deren öffentliche Verträge; die Domain bleibt autoritativ. Zusatzprüfung Listenreihenfolge der Authoringlösung A→B, Zeilen-/Spaltenzahlen, Zeitordnung, Importzustand `AwaitingProofGate` (`LVL-IMPORT-PROOF-GATE-MISSING`, `IsRuntimeImportable` stets `false`). |
| `JcsSerializer` | RFC-8785-Kanonisierung für Integer-JSON (UTF-16-Schlüsselordnung, RFC-Escaping, UTF-8 ohne BOM, ohne Newline). |
| `LevelHashing` | Content-Hash (SHA-256 über JCS des Gesamtdokuments, bare Hex `documentSha256`), Public-Puzzle-Hash (`STP-PUZZLE-SEMANTIC-JCS-1`), Lösungshash (`STP-SOLUTION-JCS-1`), Proof-Projektionshash (`STP-PROOF-JCS-1`), Legacy-v1-Puzzlehash (`STP-LEVEL-V1-PUZZLE-JCS-1`); Projektionen strikt getrennt. |
| `LevelV1Reader`, `LevelV1Document`, `LevelV1ToV2Migrator` | Strikter v1-Leser; Migration v1→v2 deterministisch und idempotent, auf Kopie, mit Neuberechnung und Abgleich der drei aufgezeichneten v1-Hashes; `proofRef` nur aus aufgezeichneten v1-Fakten abgeleitet; bei nicht neutral übernehmbaren Werten `LVL_MIGRATION_NEEDS_EDITORIAL_DECISION`. |
| `LevelV2Loader` | Fassade Parse → Schema → Domain-Abbildung → Semantik (`Load(byte[] | JsonValue)`, `LoadText`); eine spätere Stufe verdeckt nie eine frühere. |
| `LevelDiagnostic` | Stabile Diagnosecodes mit Pfad; Codefamilien in `LEVEL_DATA_FORMAT.md` §8 dokumentiert. |

### Geänderte Dateigruppen

| Gruppe | Dateien |
|---|---|
| Produktion | 15 neue `*.cs` samt `.meta` in `Scripts/Infrastructure/Content/` (flach, keine neue Assembly, `asmdef`/`csc.rsp` unverändert). |
| Tests | 9 neue `*.cs` samt `.meta` und `level-v2-jcs-golden.json` samt `.meta` in `Tests/EditMode/Content/`. |
| Architekturdokumente | `ARCHITECTURE/LEVEL_DATA_FORMAT.md` (§6.1 Content-/Public-Puzzle-Hash, §6.2 JCS-Profil und Grenzen, Diagnosefamilien, Pipeline-/Importzustand, Migrationsumsetzung, Parsergrenzen), `ARCHITECTURE/CONTENT_PIPELINE.md` (Umsetzungsstand). |
| CI | `.github/workflows/validate.yml` (Schritt „JCS-/Hash-Goldenvektoren gegen die Node-Referenz prüfen“), `.github/workflows/unity.yml` (Content-Assemblies in EditMode-Evidenz und beiden Coverage-Pflichtmengen; keine Gateabsenkung). |
| Projektsteuerung | dieses WP, `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md`. |
| Bewusst unverändert | `level-v2.schema.json`, beide Level-v2-Beispiele, `proof-v1.schema.json`, WP-023-Manifest, Domain-/Solver-Code, `asmdef`/`csc.rsp`. Die bestehenden Artefakte waren mit der Implementierung bereits konsistent; die Konsistenz wird durch `SchemaParityTests` und das Golden-Crosscheck dauerhaft geprüft. |

### Tests und Nachweise

Lokal ist kein Unity-Editor verfügbar. Die lokale Verifikation lief deshalb in einem unabhängigen .NET-8-Harness (außerhalb des Repositorys), der dieselben Quelldateien von Domain, Solver und Content mit NUnit kompiliert und ausführt. Das ist **kein** Ersatz für den Unity-Compile; den maßgeblichen Nachweis erbringt der echte Unity-Lauf in der CI (nächster Abschnitt).

| Nachweis | Ergebnis |
|---|---|
| Content-EditMode-Tests (Harness) | **307 / 307 bestanden**: `StrictJsonParserTests` 16, `JcsSerializerTests` 9, `LevelV2ReaderTests` 140, `LevelV2SemanticsTests` 43, `LevelHashingTests` 17, `LevelMigrationTests` 43, `LevelV2LoaderTests` 24, `SchemaParityTests` 15 (ausgeführte Testfälle je Klasse). Abgedeckt: Positiv-/Negativkatalog je Stufe, alle Feld- und Typfehler, Grenzwerte, Zufallsdokumente gegen die Domain, Determinismus und Feldreihenfolge-Invarianz, Hashprojektionen und deren Trennung, Migration inklusive Idempotenz und Editorialfälle, Schema-Parität (Parser akzeptiert/verwirft exakt wie das JSON-Schema). |
| Bestehende Domain-/Solver-Regressionen (Harness) | 41 / 41 bestanden (19 Domain, 22 Solver). |
| Kompilierbarkeit gegen `netstandard2.1` / NUnit 3.5 (Unity-nahe Zielplattform, `warnaserror`, nullable) | Produktion und Tests kompilieren ohne Warnungen. |
| Golden-Vektoren | 12 JCS-Fälle, 37 Ablehnungsfälle (davon 32 zusätzlich von der Node-Referenz abgelehnt, die übrigen 5 sind C#-spezifisch, weil die Node-Referenz `-0`, ungültiges UTF-8 und unbegrenzte Tiefe akzeptiert), 2 v2- und 2 v1-Dokumentvektoren. Die Werte stammen aus der unabhängigen Node-Referenz `tools/architecture-validation/jcs_crosscheck.mjs`, nicht aus der C#-Implementierung. |
| Mutationsprobe | Eine absichtliche Änderung der Rastergrenze in `LevelV2Contract` lässt Tests fehlschlagen; Datei danach byteidentisch wiederhergestellt. |
| Architecture-only- und Production-Scope-Validator (lokal) | PASS inklusive Self-/Negativtests; Manifest-Unveränderlichkeit lokal und remote belegt. |
| Preflight (verbotene Tokens `PlayerPrefs`, `Resources.Load`, `ServiceLocator`) | PASS. |

### Tatsächliche GitHub-Actions-Ergebnisse

**Code-Stand `716e9fb3c1e23103e65ed9c6dc382437c228b05c` (maßgeblich):**

| Workflow / Job | Ergebnis |
|---|---|
| Architecture Validation, PR-Run `37996329618` und Push-Run `37996324134` (Architecture-only mit Self-/Negativtests, Golden-Crosscheck gegen Node-Referenz, kanonischer WP-023-Production-Scope mit Self-/Negativtests) | **SUCCESS** |
| Unity CI, PR-Run `37996329689` (alle Jobs) | **SUCCESS** |
| Unity CI, Push-Run `37996324128` (alle Jobs) | **SUCCESS** |
| Darin: `project-preflight`, `unity-config`, `unity-evidence-guard`, `unity-environment-linux/android/ios` | **SUCCESS** |
| Darin: `unity-compile-editmode` (PR-Job `114043329416`, Push-Job `114043312301`): Editor 6000.3.23f1, Kompilierung, EditMode mit Coverage | **SUCCESS**. NUnit-Ergebnis `editmode.xml`: **349 / 349 bestanden, 0 fehlgeschlagen, 0 übersprungen**; `STP.Tests.Content.EditMode` 307 / 307, `STP.Tests.Domain.EditMode` 19 / 19, `STP.Tests.Solver.EditMode` 22 / 22 (plus 1 Fremdtest aus dem Addressables-Paket). |
| Darin: Coverage-Evidenz (Pflichtmenge um die Content-Assemblies erweitert, keine Mindest-Coverage, keine Gateabsenkung) | **PASS**, 14 STP-Assemblies. Gemessene Baseline: `STP.Infrastructure.Content` 1882/1926 Sequenzpunkte (97,7 %), Methoden 298/304; `STP.Tests.Content.EditMode` 2487/2525 (98,5 %); `STP.Puzzle.Domain` 96,9 %; `STP.Puzzle.Solver` 98,7 %. |
| Darin: `unity-playmode-bootstrap-smoke`, `unity-android-development-il2cpp`, `unity-ios-export` | **SUCCESS** |

**Verlauf bis dahin (zur Nachvollziehbarkeit, nicht als PASS gewertet):**

1. Auf `4e5fbd3` und `90c4703` scheiterten `unity-compile-editmode` sowie die Linux-/Android-Umgebung mehrfach vor jeder Codeausführung im Schritt „Initialize containers“ bzw. Image-Pull (`toomanyrequests: You have reached your unauthenticated pull rate limit`, zeitweise Timeouts gegen `auth.docker.io`). Das war ein Infrastrukturfehler des Docker Hub für die gemeinsam genutzten Runner. Es wurde nichts umgangen: kein Gate abgesenkt, keine Imagequelle gewechselt, keine Workflow-Änderung dafür.
2. Beim vierten Wiederholungsversuch auf `90c4703` lief der Unity-Container erstmals. Kompilierung erfolgreich, EditMode 348 / 349: `LevelV2LoaderTests.ProofRefHashIsOnlyAReference_NoProofArtifactIsLoadedOrRequired` schlug fehl (`System.ArgumentException : Property Count was not found`). Ursache: Das mit Unity gelieferte NUnit löst `Has.Count` per Reflection auf, und `ImportBlockers` ist zur Laufzeit ein Array. Das lokale .NET-Harness (neueres NUnit) hatte das nicht gezeigt. Korrektur in `716e9fb`: Assertion auf die typisierte `IReadOnlyList.Count`. Produktionscode und Gates blieben unverändert; der Unity-Lauf auf `716e9fb` ist vollständig grün.
3. Frühere Läufe auf `4e5fbd3`: Architecture Validation PR-Run `37992242788`, Push-Run `37992238851`: **SUCCESS**.

### Auslegungsentscheidungen (in `LEVEL_DATA_FORMAT.md` dokumentiert)

1. **Content-Hash** = SHA-256 über die JCS-Kanonisierung des vollständigen Level-v2-Dokuments (bare Hex, `documentSha256`); trägt kein Profil und wird nie mit den vier Profilhashes vermischt.
2. Die Begriffe `schemaVersion` und `ruleset` im WP-Text entsprechen im Vertrag `documentSchemaVersion` und `rulesetVersion`.
3. Parsergrenzen (256 KiB, Tiefe 16, String 8192, Array 1024, Objekt 64) sind Implementierungsgrenzen des Parsers; das Schema begrenzt das Raster weiterhin auf höchstens 10×10.
4. `-0` wird abgelehnt (`LVL-PARSE-*`), obwohl JSON es syntaktisch erlaubt: Integer-JCS kennt keine negative Null.
5. Zusätzliche Diagnosefamilien (`LVL-PARSE-*`, `LVL-VERSION-*`, `LVL-SCHEMA-*`, `LVL-DOMAIN-*`, `LVL-JCS-*`, `LVL-IMPORT-*`) ergänzen die vorhandenen Codes; keine vorhandene Bedeutung wurde geändert.
6. Die Listenreihenfolge der Authoringlösung muss von A nach B laufen; die Domain bewertet dieselbe Lösung als ungeordnete Zellmenge. Beides muss gelten.
7. Der Importzustand einer fehlerfreien Quelle ist stets `AwaitingProofGate`; `IsRuntimeImportable` ist in WP-023 immer `false`.

### Einschränkungen und offene Punkte

- **Überholt durch den Abschnitt „QC-FAIL-Nachbesserung“:** Die erste Abschluss-QC auf `182db13…` (reiner Dokumentationsnachfolger von `716e9fb…`) endete mit FAIL. Der maßgebliche Code-Stand und die maßgebliche CI-Evidenz sind seither die des Nachbesserungsabschnitts; die Testzahlen dieses Abschnitts (307 Content-Tests, 349 EditMode gesamt) gelten nur für `716e9fb…`/`182db13…`.
- Keine Proofartefakt-Erzeugung, kein Solver-v2, keine Rootspur, keine ADR-031-Metriken, keine Proof-v1-Regeneration, keine Strict-Validation-Pipeline: `proofRef` wird in der Migration nur als Referenz aus aufgezeichneten v1-Fakten abgeleitet. Das Proofgate schließt der spätere Solver-v2-/Proof-Block.
- Katalogübergreifende Season-1-Prüfungen (Cross-reference, 5×4×12-Struktur) sind nicht Teil von WP-023.
- Die Node-Referenz akzeptiert `-0`, ungültiges UTF-8 und unbegrenzte Tiefe; diese Ablehnungsfälle sind im Golden mit `nodeCrosscheckRejects: false` markiert und nur durch die C#-Tests abgesichert.
- Der lokale Harness ist ein unabhängiger .NET-Lauf, kein Unity-Compile. Er hatte eine Unity-spezifische NUnit-Abweichung (`Has.Count` auf Array) nicht gezeigt; sie wurde erst im echten Unity-Lauf sichtbar und behoben. Die Unity-Läufe sind der maßgebliche Nachweis.
- Die Unity-Läufe auf `716e9fb…` belegen das Verhalten der Unity-Version 6000.3.23f1 in der CI. Physische Gerätetests, Store-Uploads und Release-Freigaben wurden nicht ausgeführt (bleiben `REQUIRED_LATER`).

### Akzeptanzkriterien

| ID | Stand | Nachweis |
|---|---|---|
| `AK-01` | **PASS** | Anker `a1d284ca…` (Elterncommit `ad0ac1b6…`) führt WP-023 und Manifest gemeinsam ein; Manifest-SHA-256 `a8a3a506…776b0` an Anker, lokal und remote identisch; WP verlinkt das Manifest; Architecture-Validation-Production-Scope-Lauf SUCCESS. |
| `AK-02` | **PASS** | `StrictJsonParserTests`, `LevelV2ReaderTests`, `SchemaParityTests`, `LevelV2LoaderTests`; Unity-EditMode `STP.Tests.Content.EditMode` 307 / 307 (CI), Harness 307 / 307. |
| `AK-03` | **PASS** | `LevelV2SemanticsTests` inklusive Zufallsdokumente gegen `PuzzleDefinition`/`PuzzleEvaluator`; Domainregeln nicht dupliziert; Unity-EditMode bestanden. |
| `AK-04` | **PASS** | `JcsSerializerTests` und 12 JCS- sowie 37 Ablehnungsfälle; unabhängiger Node-Crosscheck in Architecture Validation SUCCESS; C#-Ausführung unter Unity bestanden. |
| `AK-05` | **PASS** | `LevelHashingTests`; v2- und v1-Dokumentvektoren durch die Node-Referenz in Architecture Validation reproduziert; C#-Ausführung unter Unity bestanden. |
| `AK-06` | **PASS** | `LevelMigrationTests` (43 Testfälle): Golden, Idempotenz, Editorialfälle, alle sechs Legacy-Fokuswerte; Ergebnis ist stets schema- und semantikgültig; Unity-EditMode bestanden. |
| `AK-07` | **PASS** | Schema, Beispiele, Doku und Implementierung widerspruchsfrei (`SchemaParityTests`, Architecture-Validator); proof-v1-Schema unverändert; `proofRef` nur Pass-through bzw. Ableitung aus v1-Fakten. |
| `AK-08` | **PASS** | Alle Testgruppen vorhanden; Unity-EditMode 349 / 349 (Content 307, Domain 19, Solver 22); WP-022-Regressionen unverändert grün. |
| `AK-09` | **PASS für alle bis zum Code-Stand `716e9fb…` ausführbaren Prüfungen; Abschluss-QC offen** | Architecture Validation (Architecture-only, Production-Scope, Self-/Negativtests, Golden-Crosscheck) SUCCESS; Unity CI vollständig SUCCESS (Compile, EditMode, Coverage-Evidenz, PlayMode-Smoke, Android-IL2CPP, iOS-Export, Preflight, Evidence-Guard); keine Gateabsenkung; keine Änderung außerhalb des Manifests. Die unabhängige Abschluss-QC auf dem eingefrorenen finalen PR-HEAD ist laut DoD zusätzlich erforderlich und noch nicht erfolgt. |

### Nächster Schritt

Siehe Abschnitt „QC-FAIL-Nachbesserung“ (Nächster Schritt) am Ende dieser Datei; dieser Abschnitt ist durch die FAIL-Entscheidung der ersten Abschluss-QC überholt.

## QC-FAIL-Nachbesserung – 2026-10-10

### Anlass und Vorgehen

- Die erste unabhängige Abschluss-QC auf dem PR-HEAD `182db13af5f8e2d1db3acadc1318153575baac85` endete mit **FAIL** und meldete vier Befunde. Die Nachbesserung erfolgt innerhalb von WP-023 auf demselben Branch `codex/wp-023-level-v2-foundation`; PR #15 bleibt Draft und ist nicht gemergt.
- Die Befunde wurden als zu verifizierend behandelt: Zuerst wurden Regressionstests geschrieben und gegen den **unveränderten** Produktionscode von `182db13…` ausgeführt (21 von 337 Content-Tests rot, ausschließlich die neuen Tests; die übrigen 316 grün). Erst danach wurde korrigiert. Trust Anchor `a1d284ca…`, Basis `ad0ac1b6…` und das Manifest (SHA-256 `a8a3a506…776b0`, byteidentisch) sind unverändert; alle geänderten Dateien liegen in der Manifest-Allowlist.
- Keine Produkt- oder Architekturentscheidung, keine Scope-Erweiterung, keine Vorwegnahme von Solver-v2/Proof-v1.

### Befunde, eigene Reproduktion und Korrektur

| # | Befund | Reproduktion auf `182db13…` | Vertragsgrundlage | Korrektur |
|---|---|---|---|---|
| 1 | Alleinstehendes Surrogat in einem per `JsonValue`-Fabriken gebauten Dokument erreicht am Eingang `LevelV2Loader.Load(JsonValue)` die Hashberechnung und löst eine unbehandelte Exception aus. | **Bestätigt** (Testlauf und separater Reproduktionslauf gegen `182db13…`). `LevelV2Loader.Load` mit `CreateString("note\ud800")` in `/production/qualityNote` → `InvalidOperationException: LVL-JCS-SURROGATE @ /production/qualityNote` aus dem `LevelV2LoadResult`-Konstruktor (`LevelHashing.HashProjection` → `JcsSerializer.SerializeOrThrow`). Ebenso betroffen (von der QC nicht genannt, gleiche Ursache): `LevelV1ToV2Migrator.Migrate` (v1-Freitext `qualityNote`, nicht Teil der Legacy-Hashes), `MigrateToCurrent` (v2-Pass-through) und der Struktur-Fuzz nach Erweiterung um Surrogatwerte. | `LEVEL_DATA_FORMAT.md` §6.2 (alleinstehende Surrogate sind harte Fehler `LVL-JCS-*`), §8 (`LVL-JCS-*` ist eine stabile Diagnosefamilie der Kanonisierung; eine abgelehnte Quelle meldet Diagnosen als Importblocker), `CONTENT_PIPELINE.md` §5 (Stufen liefern Diagnosen, eine spätere Stufe verdeckt nie eine frühere). Die Diagnose existiert im Vertrag bereits; nur der Weg dorthin warf. | Neuer interner, nicht werfender `LevelHashing.TryComputeDocumentHashes` (nutzt `JcsSerializer.Serialize` und `LevelHashing.Compute`); `LevelV2LoadResult` und `LevelMigrationResult` melden den Fehler als einzige Hash-Stufen-Diagnose (`LVL-JCS-SURROGATE`, Pfad des Strings): Ergebnis `Rejected`, keine Hashes, `ImportBlockers` = Diagnosen; Migration liefert `Document == null`, `WasMigrated == false`. Frühere Stufen behalten Vorrang. Die öffentlichen werfenden Hash-Helfer bleiben, sind jetzt per XML-Doku als „nur für bekannt gültige Projektionen“ gekennzeichnet. |
| 2 | `StrictJsonParser.ParseText` misst `MaxBytes` mit `text.Length`. | **Bestätigt.** Ein Array aus 20 Strings zu je 8000 × U+20AC (alle innerhalb der String-/Arraygrenzen): ca. 160 000 Zeichen, ca. 480 000 UTF-8-Bytes. `ParseText` akzeptierte das Dokument (`Error == null`), `Parse(byte[])` lehnte es mit `LVL-PARSE-SIZE` ab. Zusätzlich lieferte die Fehlermeldung „characters“. | `LEVEL_DATA_FORMAT.md` §11: „Eingabegröße 256 KiB“ (Byte-Einheit); `StrictJsonParser`/`JsonParserLimits.MaxBytes` heißt und dokumentiert Bytes; beide Eingänge sollen dasselbe Dokument gleich bewerten. | `ParseText` zählt UTF-8-Bytes (`Utf8SizeExceeds`, bricht bei Überschreitung sofort ab, kein Werfen bei alleinstehenden Surrogaten: sie zählen wie ihr Ersatzzeichen drei Bytes und werden danach ohnehin inhaltlich abgelehnt); Meldung „bytes“; Größe bleibt das erste Tor wie beim Byte-Eingang. §11 präzisiert die Einheit. |
| 3 | `JsonValue.Items`/`Members` geben die internen Arrays heraus; Rückkonvertierung und Mutation erzeugen doppelte Schlüssel, die anschließend kanonisiert werden. | **Bestätigt**, zusätzlich in einem separaten Reproduktionslauf gegen den ausgecheckten Stand `182db13…`: `doc.Members is JsonMember[]` → `True`; `((JsonMember[])doc.Members)[1] = new JsonMember("a", …)` auf `{"a":1,"b":2}` lieferte kanonisiert `{"a":1,"a":99}`. Bei der Durchsicht fand sich dieselbe Ursache in den Ergebnistypen (derselbe Lauf): `LevelV2LoadResult.Diagnostics` war eine `List<LevelDiagnostic>`; nach `((IList<LevelDiagnostic>)result.Diagnostics).Clear()` meldete ein semantisch abgelehntes Dokument `IsValid == true`, `ImportState == AwaitingProofGate` bei `ContentHash == null`. | `JsonValue` ist als unveränderlicher DOM dokumentiert; `CreateObject` verbietet doppelte Namen (Parser: `LVL-PARSE-*`, doppelte Schlüssel); `LEVEL_DATA_FORMAT.md` §11 „doppelte Schlüssel (mit Pfad)“ sind verboten; die DTOs verwenden bereits schreibgeschützte Sichten. | `Items`/`Members` liefern `ReadOnlyCollection`-Sichten (kein Array, keine `List`), leere Container teilen unveränderliche statische Sichten. Gleiche Maßnahme für alle öffentlichen Ergebnislisten (`LevelV2ReadResult`, `LevelV1ReadResult`, `LevelV2MapResult`, `LevelV2LoadResult` inkl. `ImportBlockers`, `LevelMigrationResult`, Rückgabe von `LevelV2Semantics.Validate`) über `LevelDiagnosticList.Freeze` (schreibgeschützte Momentaufnahme). Die Klassendoku von `JsonValue` ist korrigiert (Strings können alleinstehende Surrogate tragen; Kanonisierung lehnt sie ab). |
| 4 | Beim Anhängen eines Surrogatpaars kann `MaxStringLength` um eine UTF-16-Codeeinheit überschritten werden. | **Bestätigt.** Mit Limit 4: `["abc🚀"]` (roh und als `\ud83d\ude80`) wurde akzeptiert (5 Codeeinheiten); Standardgrenze: 8191 × `a` + Paar ergab 8193 Codeeinheiten. Gilt für Strings und Objektschlüssel. | `LEVEL_DATA_FORMAT.md` §11: „String- und Schlüssellänge 8192 UTF-16-Codeeinheiten“. | Vor dem Anhängen eines Paares (rohe und `\u`-Form) wird Platz für zwei Codeeinheiten verlangt (`LVL-PARSE-STRING-LENGTH`); gemeinsamer Helfer für die Meldung. §11 präzisiert „ein Paar zählt als zwei“. |

### Geänderte Dateien

| Datei | Änderung |
|---|---|
| `Scripts/Infrastructure/Content/JsonValue.cs` | Schreibgeschützte Sichten für `Items`/`Members`; Doku korrigiert (Befund 3). |
| `Scripts/Infrastructure/Content/StrictJsonParser.cs` | UTF-8-Bytemessung in `ParseText`; Platzprüfung für Surrogatpaare (Befunde 2 und 4). |
| `Scripts/Infrastructure/Content/LevelHashing.cs` | Nicht werfender `TryComputeDocumentHashes`; Doku der werfenden Helfer (Befund 1). |
| `Scripts/Infrastructure/Content/LevelV2Loader.cs`, `LevelV1ToV2Migrator.cs` | Hash-Stufen-Diagnose statt Exception; schreibgeschützte Ergebnislisten (Befunde 1 und 3). |
| `Scripts/Infrastructure/Content/LevelDiagnostic.cs` | Interner Helfer `LevelDiagnosticList.Freeze` (Befund 3, gleiche Ursache). |
| `Scripts/Infrastructure/Content/LevelV2Reader.cs`, `LevelV1Document.cs`, `LevelV2DomainMapper.cs`, `LevelV2Semantics.cs` | Ergebnislisten werden eingefroren (Befund 3, gleiche Ursache). |
| `Tests/EditMode/Content/JsonValueTests.cs` (+ `.meta`, neu) | 6 Tests zur Unveränderlichkeit des DOM. |
| `Tests/EditMode/Content/StrictJsonParserTests.cs`, `LevelV2LoaderTests.cs`, `LevelMigrationTests.cs` | Regressionstests zu den Befunden 1 bis 4, erweiterter Struktur-Fuzz um Surrogatwerte. |
| `ARCHITECTURE/LEVEL_DATA_FORMAT.md` | Klarstellungen §6.2 (Surrogate im DOM, werfende vs. diagnostizierende Hashwege) und §11 (Einheiten, unveränderlicher DOM). Keine Vertragsänderung. |
| `WORK_PACKAGES/WP-023_…md`, `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` | Statusfortschreibung. |
| Bewusst unverändert | WP-023-Manifest, Schema, Beispiele, Golden-Vektoren (`level-v2-jcs-golden.json`), Workflows, `asmdef`/`csc.rsp`, Domain-/Solver-Code. |

### Tests und Nachweise

Lokal ist weiterhin kein Unity-Editor verfügbar; der .NET-8-Harness (außerhalb des Repositorys, dieselben Quelldateien) ist kein Ersatz für den Unity-Lauf.

| Nachweis | Ergebnis |
|---|---|
| Reproduktion auf unverändertem Produktionscode von `182db13…` | 21 von 337 Content-Tests rot, davon Befund 1 (DOM-Loader, Migration direkt und Pass-through, Reihenfolge-Test, Struktur-Fuzz), Befund 2 (4 Grenzfälle, Mehrbyte-Test), Befund 3 (4 DOM-Tests, 2 Ergebnislisten-Tests), Befund 4 (4 Fälle, Standardgrenze). Der anschließend geschärfte Mehrbyte-Test wurde gegen den alten Parser erneut rot bestätigt (`ParseText` ohne Fehler). |
| Content-EditMode-Tests (Harness), nach der Korrektur | **338 / 338 bestanden**: `JcsSerializerTests` 9, `JsonValueTests` 6, `LevelHashingTests` 17, `LevelMigrationTests` 46, `LevelV2LoaderTests` 30, `LevelV2ReaderTests` 140, `LevelV2SemanticsTests` 43, `SchemaParityTests` 15, `StrictJsonParserTests` 32. |
| Domain-/Solver-Regressionen (Harness) | 41 / 41 bestanden. |
| Kompilierbarkeit gegen `netstandard2.1` / NUnit 3.5 (`warnaserror`, nullable) | Produktion und Tests ohne Warnungen. |
| Golden-Crosscheck gegen Node-Referenz (lokal) | PASS: 12 JCS-Fälle, 32 abgelehnte Eingaben, 2 v2- und 2 v1-Dokumentvektoren. |
| Architecture-only- und Production-Scope-Validator (lokal, Self-/Negativtests) | PASS (17 bzw. 18 Prüfgruppen); Manifest-SHA-256 unverändert. |
| Preflight (verbotene Tokens) | PASS (keine Treffer). |

### Tatsächliche GitHub-Actions-Ergebnisse

**Maßgeblicher Code-Stand `167a6ac920a87d74b63d4cf63426e8d9d4909145`** (enthält die Korrekturen aus `28b0f72…` plus eine Compilekorrektur, siehe Verlauf):

| Workflow / Job | Ergebnis |
|---|---|
| Architecture Validation, PR-Run `38014310417` und Push-Run `38014306432` (Architecture-only mit Self-/Negativtests, Golden-Crosscheck gegen Node-Referenz, kanonischer WP-023-Production-Scope mit Self-/Negativtests) | **SUCCESS** |
| Unity CI, Push-Run `38014306441` (alle Jobs) | **SUCCESS** |
| Unity CI, PR-Run `38014310372` (Versuch 2, alle Jobs) | **SUCCESS** (Versuch 1: Job `unity-compile-editmode` wurde durch die Concurrency-Gruppe `unity-personal-seat` als wartender Job ersetzt und auf `cancelled` gesetzt, ohne Code auszuführen; Wiederholung per „Re-run failed jobs“, keine Änderung) |
| Darin: `project-preflight`, `unity-config`, `unity-evidence-guard`, `unity-environment-linux/android/ios` | **SUCCESS** |
| Darin: `unity-compile-editmode` (Push-Job `114101144699`, PR-Job `114106061621`): Editor 6000.3.23f1, Kompilierung, EditMode mit Coverage | **SUCCESS**. NUnit-Ergebnis `editmode.xml` (beide Läufe): **380 / 380 bestanden, 0 fehlgeschlagen, 0 übersprungen**; `STP.Tests.Content.EditMode` 338 / 338, `STP.Tests.Domain.EditMode` 19 / 19, `STP.Tests.Solver.EditMode` 22 / 22 (plus 1 Fremdtest aus dem Addressables-Paket). |
| Darin: Coverage-Evidenz (Pflichtmenge unverändert, keine Mindest-Coverage, keine Gateabsenkung) | **PASS**. Gemessene Baseline: `STP.Infrastructure.Content` 1961/2008 Sequenzpunkte (97,7 %), Methoden 310/316; `STP.Tests.Content.EditMode` 2774/2812 (98,6 %); `STP.Puzzle.Domain` 96,9 %; `STP.Puzzle.Solver` 98,7 %. |
| Darin: `unity-playmode-bootstrap-smoke`, `unity-android-development-il2cpp`, `unity-ios-export` | **SUCCESS** (Push- und PR-Lauf) |

**Verlauf (zur Nachvollziehbarkeit, nicht als PASS gewertet):**

1. Auf `28b0f72eaf342d8b8d01ab993757005d448d7c58` waren beide Architecture-Validation-Läufe SUCCESS (PR `38014007238`, Push `38014002671`), aber `unity-compile-editmode` schlug in beiden Unity-Läufen (PR `38014007166`, Push `38014002745`) mit einem **echten Compilefehler** fehl: `StrictJsonParser.cs(76,17): error CS8602: Dereference of a possibly null reference.` Ursache: Nach `string.IsNullOrEmpty(text)` blieb im Unity-Compiler (unannotierte `netstandard2.1`-Referenzen) der Nullzustand von `text` „möglicherweise null“; der lokale .NET-Harness kennt die Annotation und hat den Fehler nicht gezeigt. Das war ein Fehler der Nachbesserung selbst, kein Infrastrukturproblem. `28b0f72…` ist deshalb **nicht** Unity-grün und überholt.
2. Behoben in `167a6ac…` durch eine explizite `text == null || text.Length == 0`-Prüfung (kein Verhaltenswechsel); alle Gates unverändert. Die Unity-Läufe auf `167a6ac…` sind vollständig grün.
3. Lehre für künftige Arbeiten am Content-Modul: Nullable-Flow darf nicht von BCL-Annotationen wie `[NotNullWhen]` abhängen; der lokale Harness ersetzt den Unity-Compile nicht.

### Einschränkungen und offene Punkte

- **Überholt durch den Abschlussabschnitt unten:** Die erneute unabhängige Abschluss-QC und die Geschäftsführungsfreigabe standen zum Zeitpunkt der Nachbesserung noch aus.
- Der finale PR-HEAD ist ein reiner Dokumentationsnachfolger des Code-Stands `167a6ac…`; die finale QC erhob die Checks auf dem eingefrorenen Head neu.
- Die unter Befund 3 genannte Ausweitung auf die Ergebnislisten ist eine Erweiterung innerhalb desselben Moduls und derselben Ursache (herausgegebener veränderlicher Speicher), keine neue Funktion.
- `JcsSerializer.Serialize` und die DTO-Projektionen rekursieren über die Tiefe eines von Hand gebauten DOM; die Tiefe ist nur für Parser-Eingaben begrenzt (Tiefe 16). Die finale unabhängige QC bewertete diesen dokumentierten Randfall als nicht integrationsblockierend, da die geschlossenen Levelverträge solche Strukturen vor der Hashberechnung abweisen.
- Alle Einschränkungen des Phase-B-Abschnitts (Node-Referenz akzeptiert `-0`/ungültiges UTF-8/unbegrenzte Tiefe, Gerätetests und Store-Uploads `REQUIRED_LATER`) gelten unverändert.

### Nächster Schritt

Der in diesem Abschnitt beschriebene Prüf- und Integrationsschritt ist erledigt. Maßgeblich ist der folgende Abschlussabschnitt.

## Abschluss und Integration – 2026-10-10

- **Finale unabhängige Abschluss-QC:** PASS auf dem eingefrorenen PR-HEAD `f56eab34dca0346d0e339a1c5cde650e15955f68`; kein integrationsblockierender Befund. AK-01 bis AK-09 wurden als erfüllt bewertet.
- **Finale CI-/Build-Evidenz auf dem geprüften Head:** Architecture Validation PR-Run `38017495962` und Push-Run `38017492793` SUCCESS; Unity CI PR-Run `38017495922` und Push-Run `38017492821` SUCCESS mit Compile, EditMode 380/380, Coverage-Evidenz, PlayMode-Smoke 27/27, Android-IL2CPP und iOS-Export/Xcode-Build.
- **Trust/Sscope:** Trust Anchor `a1d284caec7746a4aa48fbe564d37d179d60fed8`; Manifest bis zur Integration byteunverändert, SHA-256 `a8a3a5068bf6b390ad3b44b5e6d8ec36f2d93f9d849959c9230620880a5776b0`.
- **Geschäftsführungsfreigabe:** erteilt nach PASS der unabhängigen Abschluss-QC.
- **Integration:** PR #15 wurde mit exakt dem geprüften Head in `main` integriert. Merge-Commit: `10405370967496f51be9e5c00c381fd8052ec072`.
- **Definition of Done:** Für WP-023 vollständig erfüllt. Die bewusst späteren Punkte (physische Gerätetests, Store-/Release-Schritte sowie Solver-v2/Proof/Strict-Validation/Contentproduktion) liegen außerhalb des WP-023-Abschlusses.
- **Nächster Produktionsblock:** Solver-v2-Metriken und Deduktionsspur gemäß ADR-031. Die vorhandene WP-016-Datei bleibt Planungsgrundlage; vor Implementierung ist ein neuer ausführbarer Work-Package-/Manifest-Trust-Anchor nach ADR-030 auf frischem `main` erforderlich.
