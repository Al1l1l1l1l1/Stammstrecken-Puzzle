# WP-023 – Level-v2-Foundation und Hashverträge

## ID

`WP-023`

**Bearbeitungsstatus:** **Phase B implementiert und verifiziert (Code-Stand `716e9fb3c1e23103e65ed9c6dc382437c228b05c`): Alle GitHub-Actions-Workflows (Architecture Validation und Unity CI inklusive Compile, EditMode 349/349, Coverage-Evidenz, PlayMode-Smoke, Android-IL2CPP und iOS-Export) sind auf diesem Stand SUCCESS. Technisch bereit für die unabhängige Abschluss-QC auf dem eingefrorenen finalen PR-HEAD. Abschluss-QC und Geschäftsführungsfreigabe stehen aus; WP-023 ist nicht abgeschlossen und nicht integriert.**

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
| `LevelV2Loader` | Fassade Parse → Schema → Domain-Abbildung → Semantik (`Load(byte[] \| JsonValue)`, `LoadText`); eine spätere Stufe verdeckt nie eine frühere. |
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

- **Abschluss-QC und Geschäftsführungsfreigabe stehen aus.** Die Unity-CI-Evidenz ist auf dem Code-Stand `716e9fb…` erbracht. Der finale PR-HEAD ist ein reiner Dokumentationsnachfolger davon; die Abschluss-QC prüft dessen Checks auf dem eingefrorenen HEAD neu.
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

1. Unabhängige Abschluss-QC auf dem eingefrorenen finalen PR-HEAD von PR #15; das ausführende System darf nicht der Implementierungsagent sein. Die Checks des dann eingefrorenen HEADs sind dort neu zu erheben.
2. Erst nach PASS der Abschluss-QC und Geschäftsführungsfreigabe Integration. PR #15 wird bis dahin nicht gemergt (Draft).
3. Danach Freigabe des separaten Solver-v2-Blocks (ADR-031); er wird durch WP-023 nicht vorgezogen.
