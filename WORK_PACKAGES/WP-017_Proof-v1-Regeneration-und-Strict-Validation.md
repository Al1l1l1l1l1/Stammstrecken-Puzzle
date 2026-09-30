# WP-017 – Proof-v1-Regeneration und Strict-Validation

## ID

`WP-017`

## Ziel

`proof-v1` wird als vollständig regenerierbarer, byteidentisch gebundener `solver-v2`-Nachweis implementiert. Die gemeinsame LevelValidationPipeline prüft sowohl gespeicherte als auch frisch erzeugte Proofs, lehnt jede Divergenz mit `PRF-NONDETERMINISTIC` ab und behandelt Warnungen in Strict-Modus in Einzel- wie Batchresultaten konsistent als Fehler.

## Voraussetzungen

1. Vollständige Pflichtlektüre nach [`AGENTS.md`](../AGENTS.md).
2. WP-014, WP-018, WP-019, WP-020, WP-015 und WP-016 sind abgeschlossen und nach `main` integriert. Die historischen WP-008-/WP-009-Branches sind nur Vergleichskorpora.
3. [`ADR-007`](../DECISIONS/ADR-007-solver-und-eindeutigkeitspruefung.md), [`ADR-021`](../DECISIONS/ADR-021-puzzleidentitaet-und-proofartefakte.md), [`ADR-031`](../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md), [`ARCHITECTURE/LEVEL_DATA_FORMAT.md`](../ARCHITECTURE/LEVEL_DATA_FORMAT.md), [`ARCHITECTURE/CONTENT_PIPELINE.md`](../ARCHITECTURE/CONTENT_PIPELINE.md) und dieses Work Package sind gelesen.
4. WP-016 liefert eine auditable `solver-v2`-Spur und alle vier definierten Metriken.
5. Der historische WP-013-Branch darf ausschließlich als lesbare Negativreferenz genutzt werden.

## Scope

1. Eine eigene, von aktuellem `main` abgezweigte Branch und ein neues WP-017-Scope-Manifest im gemeinsamen Trust-Anchor-Commit anlegen.
2. `proof-v1.schema.json`, DTOs, Parser, kanonischen Serializer, profilierten Hash und Binding gemäß ADR-031 implementieren oder vervollständigen.
3. Alle vier Metriken im Proof-v1-Schema als nichtnegative Ganzzahlen behandeln; `proofFormatVersion` bleibt 1.
4. Einen `solver-v2`-Proofgenerator implementieren, dessen Ausgabe vor jeder weiteren Verwendung selbst schema- und hashvalidiert wird.
5. Die gemeinsame `LevelValidationPipeline` mit den Stufen Parse → Schema → Domain → Semantik → Solver/Proofregeneration implementieren bzw. vervollständigen.
6. Frische und gespeicherte Proofbytes bytegenau vergleichen. Unterschied bei identischem Puzzlehash und `solver-v2` ist `PRF-NONDETERMINISTIC`.
7. Strict-Semantik in `LevelValidationResult`, `ValidateOne`, Batchreport, `Succeeded`, `FailedStage` und Fehler-/Warnzählern einheitlich implementieren.
8. Neue `solver-v2`-Proofgoldens ausschließlich aus der Produktionsgenerierung erzeugen, im Test erneut generieren und mit den gespeicherten Bytes vergleichen.
9. Architektur-, Datenformat-, Pipeline- und Testdokumente auf den tatsächlich gelieferten Stand aktualisieren.

## Betroffene Dateien/Module

Neu oder geändert, abschließend:

- `ARCHITECTURE/schemas/proof-v1.schema.json`
- `ARCHITECTURE/examples/proof-v1*.json`
- `ARCHITECTURE/LEVEL_DATA_FORMAT.md`
- `ARCHITECTURE/CONTENT_PIPELINE.md`
- `ARCHITECTURE/SOLVER_ARCHITECTURE.md` nur zur Einbindung der bereits implementierten solver-v2-Regression
- `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Content/` für Proof-DTO, Serializer, Binding und Diagnosemodell
- `Assets/StammstreckenPuzzle/Scripts/Editor/Content/` für Proofgenerator und gemeinsame Pipeline
- `Assets/StammstreckenPuzzle/Tests/EditMode/Content/`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Solver/` nur für notwendige generierte Proofgoldens
- `WORK_PACKAGES/WP-017_Proof-v1-Regeneration-und-Strict-Validation.md`
- `tools/architecture-validation/scopes/WP-017.production.scope.json` **(neu)**
- `.github/workflows/validate.yml` ausschließlich zur Umschaltung auf das neue Manifest
- `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md` als mechanische Abschlussnachführung

## Ausdrücklich nicht erlaubte Änderungen

- Keine Änderung der in ADR-031 beschlossenen solver-v2-Metriksemantik oder der Solver-Tiebreaker.
- Keine Änderung von Puzzle-ID, semantischem Puzzlehashvertrag, Kampagnenstruktur, Save-Migration, Release-Lock, Generator, UI, Assets, Privacy oder Monetarisierung.
- Keine Production-Season-1-Level oder automatische Qualitätsfreigabe.
- Keine Ausnahmeregel, die einen nicht regenerierbaren oder nur selbstgehashten Proof akzeptiert.
- Keine Herabstufung von `PRF-NONDETERMINISTIC` oder Strict-Fehlern zu Warnungen.

## Akzeptanzkriterien

1. Ein rein deduktiver `solver-v2`-Proof mit `searchNodes = 0` ist schema- und bindungsgültig.
2. Alle `solver-v2`-Prooffixtures stammen aus dem Produktionsgenerator; Deserialisierung, Selbsthash und erneute Generierung ergeben byteidentische Ergebnisse.
3. Ein Proof mit geänderter Metrik und neu berechnetem Selbsthash schlägt trotzdem mit `PRF-NONDETERMINISTIC` fehl, wenn er vom frisch generierten Proof abweicht.
4. Cross-Puzzle-Copy, stale Lösungshash, falsche Artefakt-ID, falsches Hashprofil, unbekannte Solverversion, falscher Solverversionseintrag und fehlender Proof schlagen fail-closed mit stabilen `PRF-*`-Codes fehl.
5. `--strict` verändert jede betroffene Warnung im `LevelValidationResult` zu einem Fehler; `ValidateOne` und Batchreport melden denselben Erfolg-/Fehlschlagstatus.
6. Ohne fehlerfreie Proofregeneration kann kein Level in einen Runtimekatalog oder Release-Lock gelangen.
7. Die Pipeline ist deterministisch hinsichtlich Eingabereihenfolge, Diagnosereihenfolge, Proofbytes und Batchreihenfolge.

## Tests

- Vollständiger lokaler Architekturvalidator mit WP-017-Production-Scope und Self-/Negativtests.
- `git diff --check` gegen den Manifest-Basecommit.
- Unity Compile und alle Content-/Solver-EditMode-Tests im zugelassenen CI-Runner.
- Positiv- und Negativtests für alle Akzeptanzkriterien; insbesondere Selbsthash-aber-falsch-Golden, bytegenaue Regeneration, Strict-Einzelresultat und Strict-Batchreport.
- Unabhängiger Node-JCS-Crosscheck für die tatsächliche Proofprojektion.
- Astra-QC und Sol-QC unabhängig auf dem eingefrorenen PR-Head nach erfolgreichem CI.

## Risikoklasse

**Hoch.** Proofidentität, Campaigngates und CI-Freigabe werden hier verbunden. Eine falsche Bindung kann falsche Eindeutigkeit, falsche Schwierigkeit oder nicht reproduzierbare Release-Locks zulassen.

## Definition of Done

Es gelten vollständig [`PROJECT_CONTROL/DEFINITION_OF_DONE.md`](../PROJECT_CONTROL/DEFINITION_OF_DONE.md) und zusätzlich:

- alle Akzeptanzkriterien sind mit einem konkreten Test und der zugehörigen CI-Evidenz im Work Package dokumentiert;
- die gespeicherten `solver-v1`-Prototypfixtures sind nicht mehr als aktuelle Goldens referenziert;
- Astra- und Sol-Berichte sind unabhängig, enthalten keine offenen Blocker oder High-Befunde und sind im WP verlinkt;
- `CURRENT_STATE.md` nennt den erreichten Produktionsbasisstand und den nächsten eindeutig zulässigen Content-Schritt;
- der PR ist erst nach diesen Nachweisen nach `main` integriert.

## Rollen und Übergabe

- **Implementierung:** Kimi oder ein gleichwertiger Implementierungsagent arbeitet ausschließlich gegen dieses Work Package; er darf keine ADR, Metrik oder Scopegrenze verändern.
- **Astra-QC:** unabhängige Prüfung von Proofbindung, Versionierung, Cache-/Regenerationseigenschaft und Fehlercodes.
- **Sol-QC:** unabhängige Prüfung von Strict-Semantik, Regressionstests, CI-/Scope-Evidenz und Integrationsreife.
- Beide QC-Rollen prüfen denselben eingefrorenen PR-Head getrennt. Ein positiver Implementierungstest ersetzt keine dieser unabhängigen Prüfungen.
