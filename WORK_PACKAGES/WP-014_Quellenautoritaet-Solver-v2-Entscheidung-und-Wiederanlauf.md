# WP-014 – Quellenautorität, Solver-v2-Entscheidung und Wiederanlauf

## ID

`WP-014`

## Ziel

Die Quellenautorität für den historischen WP-013-Prototyp ist verbindlich geklärt. ADR-031 definiert `solver-v2`, Proofregeneration und Strict-Semantik ausschließlich aus persistenten Projektquellen. Die nachgeordneten, vollständig abgegrenzten Work Packages WP-015 bis WP-017 und die Integrationsreihenfolge nach `main` sind festgelegt. Dieses Package liefert ausschließlich Governance; es implementiert keinen Produktionscode.

## Voraussetzungen

1. Vollständige Pflichtlektüre nach [`AGENTS.md`](../AGENTS.md).
2. [`DECISIONS/README.md`](../DECISIONS/README.md), [`DECISIONS/ADR-007-solver-und-eindeutigkeitspruefung.md`](../DECISIONS/ADR-007-solver-und-eindeutigkeitspruefung.md), [`DECISIONS/ADR-021-puzzleidentitaet-und-proofartefakte.md`](../DECISIONS/ADR-021-puzzleidentitaet-und-proofartefakte.md), [`ARCHITECTURE/SOLVER_ARCHITECTURE.md`](../ARCHITECTURE/SOLVER_ARCHITECTURE.md), [`ARCHITECTURE/LEVEL_DATA_FORMAT.md`](../ARCHITECTURE/LEVEL_DATA_FORMAT.md) und [`ARCHITECTURE/CONTENT_PIPELINE.md`](../ARCHITECTURE/CONTENT_PIPELINE.md) sind gelesen.
3. Der historische WP-013-Branch sowie der externe Auditbericht wurden nur als Befundgegenstand gelesen. Ein späterer separater Chat ist ausdrücklich keine Projektquelle.
4. Die Ausgangsbasis ist `origin/main` auf `6010b9de587a0aca26deacea1e1fb87a5f5f3868`. Der [WP-014-Dokumentationsscope](../tools/architecture-validation/scopes/WP-014.documentation.scope.json) wird gemeinsam mit diesem Work Package verankert und danach byteunverändert belassen.

## Scope

1. Quellenherkunft des separaten späteren Chats und des WP-013-Prototyps nachvollziehbar dokumentieren.
2. ADR-031 mit präzisen Metrikdefinitionen, Versionierungsregel, Proofregeneration und Strict-Semantik erstellen.
3. WP-015, WP-016 und WP-017 als vollständige, ausführbare Folgeaufträge erstellen; Kimi erhält darin nur Implementierungsverantwortung, Astra und Sol nur unabhängige QC.
4. ADR-Register, `CURRENT_STATE.md`, `WORK_QUEUE.md` und die CI-Scope-Verankerung auf diesen Governance-Stand fortschreiben.
5. Den Architekturvalidator ausschließlich für die fortlaufende ADR-Inventur, ADR-031-Statusprüfung und 22/9-Indexzusammenfassung erweitern.
6. Die frühere Auditformulierung außerhalb des Repositories berichtigen, sodass sie die separate Chatquelle nicht länger als technische Entscheidungsgrundlage ausgibt.

## Betroffene Dateien/Module

Neu oder geändert, abschließend:

- `DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` **(neu)**
- `DECISIONS/README.md`
- `PROJECT_CONTROL/WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md` **(neu)**
- `PROJECT_CONTROL/CURRENT_STATE.md`
- `PROJECT_CONTROL/WORK_QUEUE.md`
- `WORK_PACKAGES/WP-014_Quellenautoritaet-Solver-v2-Entscheidung-und-Wiederanlauf.md` **(neu)**
- `WORK_PACKAGES/WP-015_Level-v2-Foundation-und-Hashvertraege.md` **(neu)**
- `WORK_PACKAGES/WP-016_Solver-v2-Metriken-und-Deduktionsspur.md` **(neu)**
- `WORK_PACKAGES/WP-017_Proof-v1-Regeneration-und-Strict-Validation.md` **(neu)**
- `tools/architecture-validation/scopes/WP-014.documentation.scope.json` **(neu)**
- `tools/architecture-validation/validate.py` ausschließlich für ADR-031-Inventur, Status- und Zusammenfassungsprüfung
- `.github/workflows/validate.yml` ausschließlich zur Umschaltung auf dieses Manifest

## Ausdrücklich nicht erlaubte Änderungen

- Kein C#, keine Unitydatei, keine `.asmdef`, kein Parser, kein Solver, kein Proofgenerator, keine Pipeline und keine Testimplementierung.
- Keine Änderung von JSON-Schemas, Fixtures, Levelcontent, Katalogen, Saves, Assets, Produktentscheidungen oder offenen Produktblockern.
- Kein Direktmerge, Cherry-Pick oder Rebase des historischen WP-013-Branches.
- Keine Übernahme einer nicht persistenten Chatentscheidung als Architekturquelle.
- Kein Produktionsimplementierungsauftrag, bevor die beiden unabhängigen QC-Berichte dieses Packages abgeschlossen sind.

## Akzeptanzkriterien

1. Die separate spätere Manus-Unterhaltung ist konkret benannt, ausdrücklich als nichtautoritative Quelle ausgeschlossen und wird in ADR-031 nicht als Begründung verwendet.
2. Der Architekturvalidator akzeptiert ADR-031 als 31. fortlaufenden, angenommenen ADR und prüft die 22/9-Indexzusammenfassung.
3. ADR-031 definiert alle vier Metriken mit exakter Zählgrenze, `solver-v2`, Proof-v1-Kompatibilität, Regenerationsvergleich und Strict-Semantik.
4. ADR-031 ergänzt ADR-007 und ADR-021 nur in klar benannten Teilen; Register, Ersetzungsabschnitt und Referenzen sind konsistent.
5. WP-015, WP-016 und WP-017 haben jeweils alle Pflichtabschnitte nach `WORK_PACKAGE_RULES.md`, abschließende Scopegrenzen, objektive Akzeptanzkriterien, konkrete Tests, Risikoklasse, Definition of Done sowie getrennte Kimi-/Astra-/Sol-Rollen.
6. `CURRENT_STATE.md` und `WORK_QUEUE.md` nennen die harte Reihenfolge: WP-008 → WP-009 → WP-015 → WP-016 → WP-017; WP-013 ist nicht direkt integrierbar.
7. Das Scope-Manifest und der Workflow prüfen genau diesen Governance-Diff gegen den dokumentierten Main-Basecommit.

## Tests

- Architekturvalidator mit `--scope documentation`, WP-014-Manifest und `--self-test`.
- `git diff --check` gegen den Manifest-Basecommit.
- Prüfung aller Markdown-Links und ADR-/Work-Package-Referenzen durch den Architekturvalidator.
- Zwei unabhängige, rein lesende QC-Berichte auf demselben eingefrorenen Branch-Head:
  - **Astra:** Quellenhierarchie, ADR-Logik, Metrikdefinition und Produktkonformität.
  - **Sol:** Work-Package-Grenzen, Scope-Manifest, Reihenfolge, CI-/DoD-Nachweise und Integrationsrisiko.
- Kein Merge, solange einer der Berichte einen offenen Blocker oder High-Befund enthält.

## Risikoklasse

**Hoch.** Diese Entscheidung bestimmt Solver-/Proofidentität, den integrativen Wiederanlauf und die Grenzen aller nachfolgenden Produktionsaufträge.

## Definition of Done

Es gelten vollständig [`PROJECT_CONTROL/DEFINITION_OF_DONE.md`](../PROJECT_CONTROL/DEFINITION_OF_DONE.md) und zusätzlich:

- alle Akzeptanzkriterien sind gegen die eingecheckten Artefakte geprüft;
- Astra- und Sol-Berichte sind unabhängig, enthalten keine offenen Blocker oder High-Befunde und sind im WP dokumentiert;
- der Governance-PR hat den Architecture-Validation-Check bestanden;
- `CURRENT_STATE.md` und `WORK_QUEUE.md` führen WP-014 nach Merge als abgeschlossen und WP-008 als nächsten Implementierungsschritt;
- der Auditbericht außerhalb des Repositorys ist hinsichtlich Quellenherkunft berichtigt.

## Rollen und Übergabe

- **Geschäftsführung / Projektarchitekt:** erstellt und verantwortet die Entscheidung und Aufträge dieses Work Packages.
- **Kimi:** keine Rolle in WP-014; darf erst nach Abschluss dieses Packages gegen WP-015, WP-016 oder WP-017 implementieren.
- **Astra-QC:** unabhängiges Architektur- und Quellenreview.
- **Sol-QC:** unabhängiges Scope-, Test- und Integrationsreview.
- Astra und Sol prüfen ohne gegenseitige Berichte und ohne die Entscheidung neu zu entwerfen.
