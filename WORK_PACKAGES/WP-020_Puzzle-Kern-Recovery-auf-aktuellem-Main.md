# WP-020 – Puzzle-Kern-Recovery auf aktuellem Main

## ID

`WP-020`

## Ziel

Auf einer **neu vom durch WP-019 integrierten `main` abgezweigten Branch** wird der reine Puzzle-Kern unabhängig wiederhergestellt: Domainwerte, objektive Diagnostik, Completion, immutable Session-/Command-Verarbeitung sowie der deterministische `solver-v1`-Ausgangskern mit Lösungslimit zwei. Der historische WP-009-Branch dient nur als lesbarer Vergleichskorpus; sein Code, seine Commits, seine Manifeste und seine Testprotokolle werden nicht direkt übernommen.

WP-020 stellt absichtlich nur den durch ADR-007 geforderten Ausgangskern her. Die `solver-v2`-Metrik-/Trace-Weiterentwicklung bleibt ausschließlich WP-016; Level-v2/JCS, Proof und Pipeline bleiben WP-015/WP-017 vorbehalten.

## Voraussetzungen

1. WP-014, WP-018 und WP-019 sind vollständig nach `main` integriert; die neue Branch startet vom konkreten WP-019-Merge-Commit.
2. [`ARCHITECTURE/PUZZLE_ENGINE.md`](../ARCHITECTURE/PUZZLE_ENGINE.md), [`ARCHITECTURE/GAME_STATE_MODEL.md`](../ARCHITECTURE/GAME_STATE_MODEL.md), [`ARCHITECTURE/SOLVER_ARCHITECTURE.md`](../ARCHITECTURE/SOLVER_ARCHITECTURE.md), [`ARCHITECTURE/LEVEL_DATA_FORMAT.md`](../ARCHITECTURE/LEVEL_DATA_FORMAT.md), ADR-005, ADR-007, ADR-021, ADR-031, `DEFINITION_OF_DONE.md` und die aktuelle Queue sind vollständig gelesen.
3. Der historische WP-009-Auftrag/Manifest auf `origin/feat/wp-009-puzzle-kern` wurde nur als Vergleich gelesen; die neue Branch besitzt einen eigenen Trust Anchor und ein eigenes Production-Scope-Manifest.
4. WP-019-Unity-/CI-Nachweise sind tatsächlich PASS. Ein lediglich historischer oder lokal behaupteter WP-008-Nachweis genügt nicht.

## Scope

1. Reine Domainwerte und `PuzzleDefinition` mit vollständiger Konstruktionsvalidierung implementieren: sechs Gleisformen, Richtungen, Zellen, Raster, Endpoints und Ein-Zellen-A/B-Sonderfall.
2. Objektive sichtbare Diagnostik und Completion-Prüfung implementieren: Randzahlen, Anschlüsse, Grade, Schleifen, getrennte Komponenten und Traversierung; ohne Lösungswissen oder UI-Leck.
3. Immutable `PuzzleSessionState` und Commands `ApplyCellContent`, `ApplyCellContentBatch`, `UndoLastAction` mit Revision, No-op, Batchatomarität, Sticky Flags und Timerstart implementieren.
4. Den deterministischen `solver-v1`-Ausgangskern implementieren: Propagation, MRV-DFS, feste Tiebreaker/Wertreihenfolge, Lösungslimit zwei, Klassifikation und Rohmetriken. Keine `solver-v2`-Trace-/Metrikweiterentwicklung.
5. Domain-/Solver-EditMode-Tests, unabhängigen Kleinstraum-Enumerator und Metamorphosetests implementieren; alle Evidenz neu ausführen.
6. Eigenen Scope/Trust Anchor, CI-Scope-Umschaltung und rein mechanische Steuerungsnachführung herstellen.

## Betroffene Dateien/Module

| Pfad oder Modul | Zulässige Änderung |
|---|---|
| `Assets/StammstreckenPuzzle/Scripts/Puzzle/Domain/` | Domainwerte, Definition, Diagnosen, Completion, Session und Commands. |
| `Assets/StammstreckenPuzzle/Scripts/Puzzle/Solver/` | Deterministischer `solver-v1`-Ausgangskern, Propagation, Suche, Ergebnis-/Metriktypen. |
| `Assets/StammstreckenPuzzle/Tests/EditMode/Domain/`, `Assets/StammstreckenPuzzle/Tests/EditMode/Solver/` | Deterministische Domain-/Solver- und Gegenprobentests. |
| `.github/workflows/validate.yml`, `.github/workflows/unity.yml` | Nur neue Manifestumschaltung und die nötigen Testassemblyreferenzen im aktuellen CI-Vertrag. |
| `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` | Mechanische Status-/Nächster-Schritt-Nachführung. |
| `WORK_PACKAGES/WP-020_Puzzle-Kern-Recovery-auf-aktuellem-Main.md`, eigenes WP-020-Manifest | Neuer Auftrag, Trust Anchor, Evidenz und Abschlussdokumentation. |

## Ausdrücklich nicht erlaubte Änderungen

- Kein Merge, Cherry-Pick, Rebase oder Konfliktauflösung aus `feat/wp-009-puzzle-kern`; keine Nutzung seines historischen Manifests als eigenen Scope.
- Kein `solver-v2`, kein Roottrace, keine neue Metriksemantik und keine Änderung von ADR-031; WP-016 ist allein zuständig.
- Kein Level-v2-Parser, JCS, Proof-v1, Proofgenerator, ValidationPipeline, Generator, Authoringeditor oder Produktionscontent.
- Keine Application-, Persistenz-, UI-, World-, Adapter- oder Bootstrap-Fachlogik, keine neue Assembly, kein SDK und keine Produktentscheidung.
- Keine Änderung offener Produktblocker, Architekturverträge oder ADRs.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| `AK-01` | Neuer Main-basierter WP-020-Trust Anchor/Manifest ist historisch gebunden und byteunverändert; historische WP-009-Artefakte sind nicht in der Git-Historie übernommen. |
| `AK-02` | `PuzzleDefinition` erzwingt sämtliche Invarianten inklusive Ein-Zellen-A/B-Fall; ungültige Definitionen sind nicht konstruierbar. |
| `AK-03` | Die objektive Diagnostik deckt die dokumentierten Bedingungen ab, ist stabil sortiert und enthält kein Lösungswissen. |
| `AK-04` | Completion akzeptiert nur den vollständigen einfachen A–B-Pfad und lehnt offene Enden, Schleifen, getrennte Komponenten, Count- und Endpointfehler ab; Hilfsmarkierungen außerhalb der Strecke bleiben toleriert. |
| `AK-05` | Commands sind atomar und deterministisch: No-op ohne Ereignis, Batch ganz oder gar nicht, Undo begrenzt/korrekt, falsche Revision abgelehnt und Timerstart nur bei echter Zelländerung. |
| `AK-06` | Der Solver klassifiziert 0/1/2+-Fälle deterministisch, stoppt bei zwei Lösungen, verwendet festgelegte MRV-/Werttiebreaker und liefert bei Ressourcenlimit niemals `UNIQUE`. |
| `AK-07` | Alle Domain-/Solver-EditMode-, Metamorphose- und unabhängigen Enumeratorgegenproben laufen auf dem realen WP-019-Scaffold in CI PASS; Scope-/Diff-/Secretprüfungen sind PASS. |

## Tests

1. WP-020-Production-Scope- und Architecture-Validator mit `--self-test`.
2. Trust-Anchor- und historischer Vergleichsnachweis.
3. Unity-EditMode für Domain und Solver; zusätzliche Kataloge für 0/1/2+, Ein-Zellen, Schleife, getrennte Komponente, Commands, Undo, No-op, Revision und Metamorphosen.
4. Unabhängiger Exhaustive Enumerator über kleine Rastersubräume gegen den Solver.
5. CI-Compile/Assemblygraph sowie Astra- und Sol-QC unabhängig auf eingefrorenem PR-Head.

## Risikoklasse

**Hoch.** Completion, Eindeutigkeit und deterministische Sessionsemantik sind fachlicher Kern und Vorbedingung für alle späteren Content- und Proofgates.

## Definition of Done

Alle Kriterien, Tests, Scope-/Trust-/CI-Evidenzen und beide unabhängigen QC-Berichte müssen PASS sein; es gibt keine offenen BLOCKER/HIGH. Erst dann wird WP-020 nach `main` integriert und WP-015 freigegeben.

## Rollen und Übergabe

- **Implementierung:** Kimi oder gleichwertiger Agent implementiert ausschließlich diesen neuen Main-basierten Scope.
- **Astra-QC:** unabhängige Review von Domain-/Completion-/Solververtrag und Übergang zu WP-016.
- **Sol-QC:** unabhängige Review von Tests, Scope, CI-Evidenz, Historienabgrenzung und Integrationsreife.
- Der historische WP-009-Branch bleibt Vergleichsmaterial, nicht direkt integrierbarer Lieferstand.
