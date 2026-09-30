# WP-015 – Level-v2-Foundation und Hashverträge

## ID

`WP-015`

## Ziel

 Auf einem aktuellen `main`, der WP-014, WP-018, WP-019 und WP-020 vollständig integriert enthält, entsteht eine produktionsnahe, aber noch prooflose Level-v2-Basis: striktes JSON-Parsing, Level-v2-Struktur- und Semantikprüfung, Domain-Mapping sowie die drei JCS-/SHA-256-Projektionen. Sie liefert keine importierbare Produktion und keinen Solverproof; diese Grenze wird erst WP-017 schließen.

## Voraussetzungen

1. Vollständige Pflichtlektüre nach [`AGENTS.md`](../AGENTS.md).
2. WP-014 und WP-018 sowie die Main-basierten Recovery-Packages WP-019 und WP-020 sind jeweils nach `main` integriert; zugehörige Required Checks und die dort geforderten Nachweise sind PASS. Die historischen WP-008-/WP-009-Branches sind keine Lieferbranches.
3. [`ADR-007`](../DECISIONS/ADR-007-solver-und-eindeutigkeitspruefung.md), [`ADR-021`](../DECISIONS/ADR-021-puzzleidentitaet-und-proofartefakte.md) und [`ADR-031`](../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md) sind gelesen.
4. [`ARCHITECTURE/LEVEL_DATA_FORMAT.md`](../ARCHITECTURE/LEVEL_DATA_FORMAT.md), [`ARCHITECTURE/CONTENT_PIPELINE.md`](../ARCHITECTURE/CONTENT_PIPELINE.md) und dieses Work Package sind gelesen.
5. Der historische Branch `origin/feat/wp-013-level-v2-pipeline` darf nur als lesbarer Vergleichskorpus dienen; er ist keine Mergequelle.

## Scope

1. Eine eigene, von aktuellem `main` abgezweigte Implementierungsbranch und ein neues WP-015-Scope-Manifest im gemeinsamen Trust-Anchor-Commit anlegen.
2. Die Parsergrenzen für untrusted Level-JSON implementieren: UTF-8 ohne BOM, keine Kommentare oder Duplicate Keys, keine Float-/Exponenttokens, keine unpaarigen Surrogates sowie dokumentierte Grenzen für Größe, Verschachtelung und Strings.
3. Die `level-v2`-DTOs, das geschlossene Schema, Domain-Mapping und die semantischen Levelregeln implementieren.
4. JCS für den explizit integer-only gehaltenen Vertrag, Hashprofilregistry und die öffentlichen Puzzle-/Lösungsprojektionen implementieren.
5. `proofRef` nur auf geschlossene Form und bekannte Profilnamen prüfen; keine Proofdatei laden, generieren, binden oder als importfähig ausgeben.
6. Reine `FIXTURE_ONLY`-Level- und Hashfixtures sowie EditMode-Tests für Parser, Schema, Domain, Semantik, JCS und Hashprojektionen erstellen.
7. Die betroffenen Architektur- und Testdokumente auf den tatsächlichen, begrenzten Lieferumfang aktualisieren.

## Betroffene Dateien/Module

Neu oder geändert, abschließend:

- `ARCHITECTURE/schemas/level-v2.schema.json`
- `ARCHITECTURE/examples/level-v2*.json`
- `ARCHITECTURE/LEVEL_DATA_FORMAT.md`
- `ARCHITECTURE/CONTENT_PIPELINE.md` (nur Kennzeichnung der noch fehlenden Proofstufe)
- `Assets/StammstreckenPuzzle/Scripts/Infrastructure/Content/` für Parser, DTOs, JCS, Hashprofile, Schema-, Domain- und Semantikadapter
- `Assets/StammstreckenPuzzle/Tests/EditMode/Content/` für die genannten Vertrags- und Mutationsprüfungen
- die bereits vorhandenen `.asmdef`/`csc.rsp` nur, soweit die bestehende normierte Assemblystruktur dies zwingend verlangt
- `WORK_PACKAGES/WP-015_Level-v2-Foundation-und-Hashvertraege.md`
- `tools/architecture-validation/scopes/WP-015.production.scope.json` **(neu)**
- `.github/workflows/validate.yml` ausschließlich zur Umschaltung auf das neue Manifest
- `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md` als mechanische Abschlussnachführung

## Ausdrücklich nicht erlaubte Änderungen

- Kein `ProofV1*`-DTO, kein Proofserializer, kein Proofgenerator, keine Proofbindung und keine `LevelValidationPipeline`.
- Keine Änderung an `STP.Puzzle.Solver`, einschließlich Solverversion, Suche, Metrik oder Deduktionsspur.
- Keine Kampagnen-, Season-1-, Generator-, Save-, UI-, Asset-, Monetarisierungs- oder Lokalisierungsproduktion.
- Keine Änderung von `proof-v1.schema.json`; dies gehört ausschließlich zu WP-017.
- Kein Direktmerge, Cherry-Pick oder Rebase des historischen WP-013-Branches.
- Keine neue Architekturentscheidung oder Erweiterung von ADR-031.

## Akzeptanzkriterien

1. Jede gültige Level-v2-Fixture durchläuft Parse, Schema, Domain und Semantik ohne Fehler.
2. Mutationen für BOM, Duplicate Key, Float, unpaariges Surrogate, unbekannte Version, zusätzliche Eigenschaft, falsche Grid-/Count-Länge, falschen Endpoint, Pfadwiederholung, offene Verbindung, Schleife, Countabweichung und falsche Zeitordnung liefern stabile fail-closed Diagnosen.
3. `STP-PUZZLE-SEMANTIC-JCS-1` und `STP-SOLUTION-JCS-1` haben dokumentierte Known-Answer-Goldens; neutrale Metadatenänderungen ändern den Puzzlehash nicht, öffentliche Puzzleänderungen schon.
4. Ein Node-basierter JCS-Crosscheck bestätigt die kanonischen Bytes und SHA-256-Werte für alle Projektionen.
5. Eine Levelquelle kann durch WP-015 **nicht** in einen Laufzeitkatalog oder Release-Lock importiert werden; der fehlende Proofgate ist als expliziter, testbarer Zustand dokumentiert.
6. Der Scope-Diff enthält ausschließlich die hier genannten Dateien; keine Secrets, Binärartefakte oder Productioncontent.

## Tests

- Vollständiger lokaler Architekturvalidator mit WP-015-Production-Scope und Self-/Negativtests.
- `git diff --check` gegen den Manifest-Basecommit.
- Unity Compile und relevante EditMode-Tests im zugelassenen CI-Runner.
- Parser-Fuzz-/Grenztests innerhalb der dokumentierten Ressourcenlimits.
- C#-zu-Node-JCS-Crosscheck und alle Known-Answer-/Mutationstests.
- Astra-QC und Sol-QC unabhängig auf demselben eingefrorenen PR-Head nach erfolgreichem CI.

## Risikoklasse

**Hoch.** Untrusted-Input-Parser und Hashidentität liegen im Content- und Releasepfad. Ein falscher Hash oder eine unvollständige Fail-closed-Grenze würde spätere Proof- und Savegarantien unterlaufen.

## Definition of Done

Es gelten vollständig [`PROJECT_CONTROL/DEFINITION_OF_DONE.md`](../PROJECT_CONTROL/DEFINITION_OF_DONE.md) und zusätzlich:

- alle Akzeptanzkriterien mit Link auf Test-/CI-Evidenz im WP dokumentiert;
- Astra- und Sol-Berichte sind unabhängig, enthalten keine offenen Blocker oder High-Befunde und sind im WP verlinkt;
- `CURRENT_STATE.md` benennt WP-015 als abgeschlossen und WP-016 als nächsten Schritt;
- der PR ist erst nach diesen Nachweisen nach `main` integriert.

## Rollen und Übergabe

- **Implementierung:** Kimi oder ein gleichwertiger Implementierungsagent darf ausschließlich diesen feststehenden Scope ausführen; keine ADR-/WP-Änderung ohne neuen Auftrag.
- **Astra-QC:** unabhängige Architektur-, Parser- und Hashvertragsprüfung.
- **Sol-QC:** unabhängige Scope-, Test-, CI- und Integrationsprüfung.
- Astra und Sol erhalten den identischen eingefrorenen PR-Head, nicht die Ergebnisse des jeweils anderen Reviews.
