# ADR-021 – Puzzleidentität und versionierte Proofartefakte

## Status

**Angenommen**

## Datum

2026-09-12

## Kontext

Level v1 vermischt Dokumentformat, stabile fachliche Identität, öffentliche Puzzleprojektion, Lösung und Solverproof. Sein Puzzlehash enthält `schemaVersion`; eine reine Formatmigration verändert daher den Fortschrittsanker. Der Proofhash bindet Puzzle- und Lösungshash nicht ausdrücklich und ist nicht als eigenständiges regenerierbares Artefakt versioniert.

## Entscheidung

ADR-021 ersetzt ADR-004 **vollständig** und restatiert dessen weiterhin geltende Grundsätze: Kanonische Levelquelle ist UTF-8-JSON ohne Kommentare; JSON Schema Draft 2020-12 prüft Struktur und Typen; ein separater semantischer Validator prüft Pfad, Randzahlen, Endpunkte, Eindeutigkeit und Contenthierarchie; Authoringdaten enthalten Lösung und Produktionsmetadaten; eine deterministische Pipeline erzeugt daraus Runtimekatalog und lokale Assets; generierte Unityartefakte sind nie Authoringquelle; veröffentlichter semantischer Puzzleinput ist unter derselben `puzzleId` unveränderlich; Migrationen sind reine sequenzielle Funktionen und unbekannte Versionen schlagen fail-closed fehl.

Es gelten drei getrennte Ebenen:

1. `puzzleId` ist die dauerhafte fachliche Identität; für bestehende Kampagnenlevel ist der Wert unverändert die hierarchische Kennung `S<season>-<section>-<route>-<position>`.
2. `documentSchemaVersion` wählt ausschließlich Parser und reine Migratorfolge.
3. `proof-v1` ist ein regenerierbares, versioniertes Solvernachweisartefakt.

Level v2 verwendet das Hashprofil `STP-PUZZLE-SEMANTIC-JCS-1` über exakt `{puzzleId,rulesetVersion,grid,endpoints,rowCounts,columnCounts}`. Dokumentversion, Revision, Texte, Completionmetadaten, Produktionsnotizen, Authoringlösung und Proof sind nicht Teil dieser Projektion. Der Lösungshash `STP-SOLUTION-JCS-1` bindet `{puzzleId,publicPuzzleHash,path}`.

Ein `proof-v1` enthält mindestens `proofFormatVersion`, `puzzleId`, profilierten öffentlichen Puzzlehash, profilierten Lösungshash, `solverVersion`, `solutionCount` und deterministische Metriken. `STP-PROOF-JCS-1` hasht alle diese Felder außer dem Hashfeld. Der Levelsource referenziert nur Artefakt-ID, Format und Hash.

Das bisherige v1-Profil bleibt als `STP-LEVEL-V1-PUZZLE-JCS-1` lesbar. Ein unprofilierter Hash wird nie heuristisch umgedeutet. V1→v2 kopiert `id` nach `puzzleId`, erhält Produktinhalt und Lösung, berechnet die neue semantische Projektion und erhöht keine redaktionelle Revision. Nicht neutral ableitbare Eingaben stoppen mit `LVL_MIGRATION_NEEDS_EDITORIAL_DECISION`.

Progress und Drafts binden an `{puzzleId, publicPuzzleHash.profile, publicPuzzleHash.sha256}`. Eine neutrale Dokument- oder Proofmigration erhält Erstabschluss, Stern-High-Water, terminale Rewards, zulässige Bestzeit und Resume. Ein anderer semantischer Puzzlehash unter derselben ID ist verboten; eine echte logische Korrektur benötigt eine neue Puzzle-ID. Eine Alt-Save-Übertragung ist nur mit eindeutigem historischem Release-Lock erlaubt, sonst `SAVE_LEVEL_IDENTITY_UNRESOLVED`.

Jeder promotierbare Release besitzt einen append-only `release-lock-v1`, der Puzzle-, Dokument-, Lösungs- und Proofidentität sowie Git-, Katalog- und Toolchainbezug festhält. Gleiche Puzzle-ID mit anderem semantischem Hash ist `LOCK_PUBLISHED_PUZZLE_MUTATED`; eine andere Dokumentversion oder ein neu gebundener Proof ist zulässig.

## Begründung

Die fachliche Puzzleidentität bleibt stabil, während Parser und Solver weiterentwickelt werden können. Fortschritt hängt nur an tatsächlich relevantem Puzzleinhalt, nicht am Dateiformat oder Cacheartefakt.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Schema-/Proofversion im Puzzlehash | Erzeugt künstliche neue Identitäten bei neutralen Migrationen. |
| Proof weiterhin im Leveldokument | Vermischt Source und regenerierbaren Cache. |
| Fortschritt nur nach ID übertragen | Könnte Fortschritt auf logisch anderes Rätsel übertragen. |
| Nur aktueller Release-Lock | Beweist keine Unveränderlichkeit gegenüber veröffentlichten Vorgängern. |

## Konsequenzen

`level-v1` und `campaign-v1` bleiben unveränderte Legacyreader-Verträge. Neue Produktion verwendet `level-v2`, `campaign-v2`, `proof-v1` und `release-lock-v1`. Der Architecture-v0.3-Validator prüft Schemas, JCS-Projektionen, Goldens, Bindungen und Migration; die tatsächliche Proofregeneration durch den späteren Produktionssolver bleibt ein späteres EditMode-Gate.

Die getrennte Endless-ID `E1-…` und BLOCKER-PROD-003 bleiben unverändert.

## Betroffene Artefakte

Level-/Campaign-/Proof-/Release-Lock-Schemas und Fixtures, `LEVEL_DATA_FORMAT.md`, Contentpipeline, Solver, Persistenz, Zustandsmodell, Buildvertrag, Teststrategie und Architekturvalidator.

## Validierung

Goldens belegen: neutrale v1→v2-Migration mit stabilem semantischen Hash, Proofbindung, Proofregeneration bei neuer Solverversion ohne neue Puzzle-ID, fail-closed Saveauflösung sowie Release-Lock-Historienvergleich. Copy-Paste-, stale-, unbekannte Profil- und semantische Mutationen müssen scheitern.

## Ersetzt / ersetzt durch

Ersetzt [ADR-004](./ADR-004-json-leveldaten-und-content-pipeline.md) vollständig, indem alle fortgeltenden Source-, Schema-, Semantik-, Unveränderlichkeits-, Pipeline- und Migrationsgrundsätze oben restatiert und die Identitäts-/Proofanteile korrigiert werden. Ergänzt [ADR-007](./ADR-007-solver-und-eindeutigkeitspruefung.md). Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: ../ARCHITECTURE/LEVEL_DATA_FORMAT.md "Level Data Format v0.3"
[2]: ../ARCHITECTURE/CONTENT_PIPELINE.md "Content Pipeline v0.3"
[3]: ../ARCHITECTURE/SOLVER_ARCHITECTURE.md "Solver Architecture v0.3"
[4]: ../WORK_PACKAGES/WP-003_Architecture-v0.3-Finalkorrekturen.md "WP-003"
