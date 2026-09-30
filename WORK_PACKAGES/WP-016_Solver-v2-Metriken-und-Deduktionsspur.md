# WP-016 – Solver-v2-Metriken und Deduktionsspur

## ID

`WP-016`

## Ziel

Der bisherige Solver wird zu `solver-v2` weiterentwickelt. Seine vier Proofmetriken entsprechen exakt ADR-031, jede Root-Deduktion besitzt eine auditierbare Spur, und normale Kampagnenlevel können technisch an `requiredGuessDepth = 0` geprüft werden. Dieses Package erzeugt noch keine Proofdateien und keine Contentpipeline; das ist WP-017.

## Voraussetzungen

1. Vollständige Pflichtlektüre nach [`AGENTS.md`](../AGENTS.md).
2. WP-014, WP-018, WP-019 und WP-020 sind nach `main` integriert; WP-015 ist abgeschlossen und nach `main` integriert. Die historischen WP-008-/WP-009-Branches sind nur Vergleichskorpora.
3. [`ADR-007`](../DECISIONS/ADR-007-solver-und-eindeutigkeitspruefung.md), [`ADR-021`](../DECISIONS/ADR-021-puzzleidentitaet-und-proofartefakte.md), [`ADR-031`](../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md) und [`ARCHITECTURE/SOLVER_ARCHITECTURE.md`](../ARCHITECTURE/SOLVER_ARCHITECTURE.md) sind gelesen.
4. Der historische WP-013-Branch wird nur als Negativvergleich benutzt; seine Metrikimplementierung darf nicht übernommen werden, ohne jedes Akzeptanzkriterium dieses Packages nachzuweisen.

## Scope

1. Eine eigene, von aktuellem `main` abgezweigte Branch und ein neues WP-016-Scope-Manifest im gemeinsamen Trust-Anchor-Commit anlegen.
2. `solver-v2` als aktuelle Produktionssolverkennung implementieren; `solver-v1` bleibt nur lesbar/historisch.
3. Root-Propagation und DFS-Metriksammlung strikt trennen.
4. Für jede Root-Reduktion eine unveränderliche Deduktionsspur mit `ruleCode`, minimalen Prämissen und konkreter Konklusion liefern.
5. `deductionSteps`, `maxDeductionDepth`, `searchNodes` und `requiredGuessDepth` exakt nach ADR-031 berechnen.
6. Den Pfad zur einzigen Lösung so erfassen, dass `requiredGuessDepth` die maximal gleichzeitig aktiven Annahmen genau dieses deterministischen Lösungspfads misst.
7. Den normalen Kampagnengate als ausdrückliche, vom Solver verwendbare Prüfung bereitstellen: `requiredGuessDepth > 0` ist nicht freigabefähig.
8. Die Solverarchitektur und Teststrategie aktualisieren und vollständige Solver-EditMode-Referenztests ergänzen.

## Betroffene Dateien/Module

Neu oder geändert, abschließend:

- `ARCHITECTURE/SOLVER_ARCHITECTURE.md`
- `ARCHITECTURE/TEST_STRATEGY.md` nur für neue explizite Solver-v2-Referenztests
- `Assets/StammstreckenPuzzle/Scripts/Puzzle/Solver/`
- `Assets/StammstreckenPuzzle/Tests/EditMode/Solver/`
- `WORK_PACKAGES/WP-016_Solver-v2-Metriken-und-Deduktionsspur.md`
- `tools/architecture-validation/scopes/WP-016.production.scope.json` **(neu)**
- `.github/workflows/validate.yml` ausschließlich zur Umschaltung auf das neue Manifest
- `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md` als mechanische Abschlussnachführung

## Ausdrücklich nicht erlaubte Änderungen

- Keine Änderung des öffentlichen Puzzle-Regelwerks, der sechs Gleisformen, der Domain-Completionsemantik, des Spielerzustands oder der Kampagnenregeln.
- Keine Änderung am Level-v2-Parser, JCS, Proofschema, Proofserializer, Contentpipeline oder Importer.
- Keine Season-1-Level, Generator-, Save-, UI-, Asset-, Privacy- oder Monetarisierungsänderungen.
- Keine Manipulation von Kennzahlen, um bestehende historische Fixturewerte nachzubilden.
- Keine neue Metrikdefinition; ADR-031 ist abschließend.

## Akzeptanzkriterien

1. `PuzzleSolver.SolverVersion` und jedes Proofresultat melden `solver-v2`.
2. Ein rein deduktiver Referenzfall liefert `searchNodes = 0`, `requiredGuessDepth = 0` und seine aus Trace rekonstruierbaren Root-Metriken.
3. Ein Referenzfall mit verworfenen DFS-Zweigen beweist, dass `searchNodes` alle tatsächlichen Annahmen zählt, aber `deductionSteps` und `maxDeductionDepth` ausschließlich aus der Rootspur stammen.
4. Ein eindeutiger Suchfall beweist, dass `requiredGuessDepth` nur die aktiv verschachtelten Annahmen auf dem deterministischen Erfolgspfad misst und nicht die Tiefe verworfener Zweige.
5. Jeder Traceeintrag referenziert nur öffentliche Puzzlefakten oder frühere Roottrace-Einträge; kein Eintrag enthält UI-, PlayerPrefs-, Asset- oder Unitydaten.
6. Die 0-/1-/2+-Klassifikation, Lösungslimit zwei, MRV-/Tiebreakerdeterminismus und harte Budgetbehandlung bleiben funktional erhalten.
7. Rotations-/Spiegelmetamorphosen und ein kleiner unabhängiger Exhaustive Enumerator bestätigen die Lösungsklasse. Exakte Metrikgoldens werden nur für absichtlich festgelegte Referenzfälle verwendet.

## Tests

- Vollständiger lokaler Architekturvalidator mit WP-016-Production-Scope und Self-/Negativtests.
- `git diff --check` gegen den Manifest-Basecommit.
- Unity Compile und alle Solver-EditMode-Tests im zugelassenen CI-Runner.
- Unit- und Mutationstests für Traceprämissen, Root-/DFS-Trennung, Nullwerte, Suchpfadtiefe, Timeout und deterministische Wiederholung.
- Performancebeleg gegen die dokumentierten 10×10-Budgets; Überschreitung ist ein expliziter Befund, nie ein stiller Unique-Erfolg.
- Astra-QC und Sol-QC unabhängig auf dem eingefrorenen PR-Head nach erfolgreichem CI.

## Risikoklasse

**Hoch.** Solvermetrik, Eindeutigkeit und Hint-/Contentqualität sind Kernverträge. Eine falsche Trennung kann Campaigncontent als deduktiv deklarieren, obwohl er Annahmen erfordert.

## Definition of Done

Es gelten vollständig [`PROJECT_CONTROL/DEFINITION_OF_DONE.md`](../PROJECT_CONTROL/DEFINITION_OF_DONE.md) und zusätzlich:

- jeder Akzeptanzpunkt ist mit konkretem Testfall und Ergebnis im WP dokumentiert;
- es gibt keine offene Abweichung zwischen Trace und Metrikgoldens;
- Astra- und Sol-Berichte sind unabhängig, enthalten keine offenen Blocker oder High-Befunde und sind im WP verlinkt;
- `CURRENT_STATE.md` benennt WP-016 als abgeschlossen und WP-017 als nächsten Schritt;
- der PR ist erst nach diesen Nachweisen nach `main` integriert.

## Rollen und Übergabe

- **Implementierung:** Kimi oder ein gleichwertiger Implementierungsagent darf ausschließlich die oben benannte Solver- und Testoberfläche ändern; keine Entscheidungen neu auslegen.
- **Astra-QC:** unabhängiger Review der Metriksemantik, Traceherkunft und ADR-031-Konformität.
- **Sol-QC:** unabhängiger Review der Teststärke, Metamorphosen, Budgetbehandlung, Scope und CI-Evidenz.
- Die QC-Berichte müssen unabhängig entstehen; keiner der beiden Reviewer erhält den Bericht des anderen vor Abschluss seines eigenen Reviews.
