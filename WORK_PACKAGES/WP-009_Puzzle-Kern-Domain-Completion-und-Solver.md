# WP-009 – Puzzle-Kern: Domain, Completion und deterministischer Solver

## ID

`WP-009`

**Bearbeitungsstatus:** **Abgeschlossen** (2026-09-28). Implementierung, vier Reviewbefunde
und gezielter Recheck abgeschlossen; kein konkreter Codeblocker. Details siehe Abschnitt
„Abschlussdokumentation".

**Trust Anchor:** Dieses Work Package und sein Production-Scope-Manifest
[`tools/architecture-validation/scopes/WP-009.production.scope.json`](../tools/architecture-validation/scopes/WP-009.production.scope.json)
werden gemeinsam in einem Trust-Anchor-Commit eingeführt (ADR-030); der Anker wird auf
`feat/wp-009-puzzle-kern` verankert und remote verifiziert. Der `baseCommit` des Manifests
ist der Elterncommit des Ankers (`edb713efc1580bcf0f8074dc7667c0f179a723ab`, WP-008-HEAD).

## Ziel

Implementierung des fachlichen Puzzle-Kerns als reine, deterministische C#-Domain- und
Solverbibliotheken gemäß den verbindlichen Verträgen
[`ARCHITECTURE/PUZZLE_ENGINE.md`](../ARCHITECTURE/PUZZLE_ENGINE.md),
[`ARCHITECTURE/GAME_STATE_MODEL.md`](../ARCHITECTURE/GAME_STATE_MODEL.md),
[`ARCHITECTURE/SOLVER_ARCHITECTURE.md`](../ARCHITECTURE/SOLVER_ARCHITECTURE.md),
[`ARCHITECTURE/LEVEL_DATA_FORMAT.md`](../ARCHITECTURE/LEVEL_DATA_FORMAT.md),
[`ADR-005`](../DECISIONS/ADR-005-deterministisches-command-state-modell.md) und
[`ADR-007`](../DECISIONS/ADR-007-solver-und-eindeutigkeitspruefung.md) auf dem
WP-008-Produktionsscaffold.

Konkret entstehen:

- Die Domainwerte (`Direction`, `TrackShape`, `CellContent`, `GridSize`, `CellCoordinate`,
  `Endpoint`, `PuzzleDefinition`) mit vollständiger Konstruktionsvalidierung.
- Die sichtbaren objektiven Diagnosen und die exakte Completion-Prüfung des
  A–B-Pfades inklusive Randzahlen, Grad-, Schleifen- und Traversierungsregeln.
- Die immutable `PuzzleSessionState`-/Command-Verarbeitung für `ApplyCellContent`,
  `ApplyCellContentBatch` und `UndoLastAction` mit Revisionen, No-op-Erkennung,
  Batchatomarität, Sticky-Flags und Timerstart erst bei echter Zelländerung.
- Der deterministische Constraint-Solver `STP.Puzzle.Solver` mit Propagation,
  Depth-First Search mit Minimum Remaining Values, festgelegten Tiebreakern,
  Lösungslimit zwei (`UNSATISFIABLE`/`UNIQUE`/`MULTIPLE_OR_MORE`/`INDETERMINATE`)
  und den grundlegenden Rohmetriken.
- Aussagekräftige EditMode-Tests für reguläre, ungültige und Grenzfälle beider
  Assemblies.

## Voraussetzungen

- WP-008-Produktionsscaffold auf Commit `edb713efc1580bcf0f8074dc7667c0f179a723ab`
  (Branch `feat/wp-008-unity-scaffold`); WP-009 arbeitet auf dem davon abgeleiteten
  Branch `feat/wp-009-puzzle-kern`. WP-008 erhält keinen Spielecode mehr.
- Lokaler Unity-Nachweis des Scaffolding (Unity 6000.3.23f1, Bootstrap PlayMode 8/8 PASS)
  liegt vor; die offenen WP-008-Buildnachweise (GitHub-CI-Runner, Android, iOS)
  blockieren WP-009 ausdrücklich nicht.
- `BLOCKER-PROD-001/002/003` bleiben unverändert offen und fail-closed: Hintcredits,
  Generator-Qualitätsprofil und Produktfreigaben sind nicht Teil dieses Work Packages.

## Scope

Erlaubt ist ausschließlich:

1. Fachlicher Puzzle-Domaincode in `STP.Puzzle.Domain` gemäß den oben genannten
   Verträgen (Werte, Definition, Diagnosen, Completion, Session/Commands).
2. Solvercode in `STP.Puzzle.Solver` gemäß `SOLVER_ARCHITECTURE.md` (Constraint-Modell,
   Propagation, Suche, Klassifikation, Rohmetriken, `solver-v1`-Versionierungskonstante).
3. EditMode-Tests in `STP.Tests.Domain.EditMode` und `STP.Tests.Solver.EditMode`.
4. Mechanische Governance-Nachführungen: dieses Work Package, sein Scope-Manifest,
   die CI-Scope-Umschaltung in `.github/workflows/validate.yml` sowie
   `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md`.

## Betroffene Dateien/Module

| Pfad/Modul | Änderung |
|---|---|
| `Assets/StammstreckenPuzzle/Scripts/Puzzle/Domain/` | Neu: Domainwerte, Definition, Diagnosen, Completion, Session/Commands. |
| `Assets/StammstreckenPuzzle/Scripts/Puzzle/Solver/` | Neu: Constraint-Solver, Propagation, Suche, Ergebnis-/Metriktypen. |
| `Assets/StammstreckenPuzzle/Tests/EditMode/Domain/` | Neu: Domain-EditMode-Tests. |
| `Assets/StammstreckenPuzzle/Tests/EditMode/Solver/` | Neu: Solver-EditMode-Tests. |
| `.github/workflows/validate.yml` | Mechanische Umschaltung von `STP_SCOPE_MANIFEST` auf das WP-009-Manifest. |
| `.github/workflows/unity.yml` | Mechanische Spiegelung der Solver-Testassembly-Referenzen im statischen Modulgraph-Check (`STP.Tests.Solver.EditMode` benötigt die direkte Referenz auf `STP.Puzzle.Domain`; Unity-Assemblyreferenzen sind nicht transitiv). |
| `WORK_PACKAGES/WP-009_Puzzle-Kern-Domain-Completion-und-Solver.md` | Neu: dieses Work Package. |
| `tools/architecture-validation/scopes/WP-009.production.scope.json` | Neu: verankertes Production-Scope-Manifest. |
| `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` | Statusfortschreibung zu WP-009. |

## Ausdrücklich nicht erlaubte Änderungen

- Kein Level-v2-Parser, kein JCS-Hashing, kein proof-v1, keine LevelValidationPipeline,
  kein Generator, kein Levelauthoring/Editorfenster (folgen späteren Work Packages).
- Keine Hint-Economy, keine Credit- oder Hintpersistenz (`BLOCKER-PROD-001`, fail-closed);
  kein `RequestHint`-Produktivpfad.
- Keine Application-, Persistenz-, UI-, World-, Adapter- oder Bootstrap-Fachlogik;
  keine Änderung an bestehenden Produktionsassemblies außerhalb von
  `STP.Puzzle.Domain` und `STP.Puzzle.Solver`.
- Keine neuen Assemblies, keine neuen Architekturentscheidungen, keine Änderung an
  `ARCHITECTURE/` oder `DECISIONS/`.
- Keine Änderung am WP-008-Trust-Anchor oder am WP-008-Scope-Manifest; kein Merge
  von `main`; keine Produktentscheidungen; keine Lösung von
  `BLOCKER-PROD-001/002/003`.
- Keine Werbung, kein Shop, kein Analytics, keine Release-Infrastruktur.
- Kein Vergleich mit Authoringlösungen in Completion oder Solverausgabe an die UI.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| `AK-01` | Trust Anchor: WP-009 und sein Manifest wurden gemeinsam im selben Add-Commit eingeführt, der Manifest-`baseCommit` ist der Elterncommit des Ankers (`edb713e…`), der Anker ist remote auf `feat/wp-009-puzzle-kern` verifiziert, das Manifest ist seitdem byteunveränderlich. |
| `AK-02` | `PuzzleDefinition` erzwingt alle Konstruktionsinvarianten aus PUZZLE_ENGINE.md Abschnitt 3 inklusive Ein-Zellen-A/B-Sonderfall; ungültige Definitionen können nicht konstruiert werden. |
| `AK-03` | Die objektiven Diagnosen melden exakt die sechs dokumentierten Bedingungen (Zeilen-/Spaltenüberschreitung, Rasteraustritt, Anschlusswiderspruch, vorzeitige Schleife, Endpoint-Mismatch), sortiert und ohne Lösungswissen. |
| `AK-04` | Die Completion-Prüfung akzeptiert genau die sieben Bedingungen aus PUZZLE_ENGINE.md Abschnitt 6 (inklusive Ein-Zellen-Pfad und Toleranz von Hilfsmarkierungen außerhalb der Strecke) und lehnt getrennte Komponenten, Schleifen, offene Enden und falsche Randzahlen ab. |
| `AK-05` | Commands sind atomar und deterministisch: No-op ohne Ereignis, Batch ganz oder gar nicht, Undo bis 256 Diffs ohne Zurücknahme sticky Flags/Timer, `STALE_COMMAND` bei falscher Revision, Timerstart erst bei echter Zelländerung. |
| `AK-06` | Der Solver klassifiziert bekannte 0-/1-/2+-Lösungsfälle korrekt, bricht deterministisch nach zwei Lösungen ab und liefert für identische Eingaben identische Ergebnisse und Metriken; ein Ressourcenlimit ergibt `INDETERMINATE`, niemals `UNIQUE`. |
| `AK-07` | EditMode-Tests decken die in den Verträgen geforderten Pflichtfälle ab (alle Formen, Randanschlüsse, Ein-Zellen-Pfad, Schleife, getrennte Komponente, Markierungssemantik, Batchatomarität, No-op, Undo, Completion, Metamorphose) und laufen lokal im Unity Editor grün; der Ausführungsstand wird wahrheitsgemäß berichtet. |
| `AK-08` | Der vollständige Diff gegen den `baseCommit` enthält ausschließlich Manifest-erlaubte Pfade; `git diff --check` und der kanonische Scope-Lauf bestehen. |

## Tests

- `STP.Tests.Domain.EditMode`: Formgeometrie und Ports aller sechs Gleisformen,
  Definitionsinvarianten (ungültige Zahlen, Endpoints, Summen, Ein-Zellen-Sonderfall),
  Diagnosen je dokumentierter Bedingung, Completion positiv/negativ, Command-Atomarität,
  No-op, Undo, Sticky-Flags, Timer, Revisionsablehnung.
- `STP.Tests.Solver.EditMode`: `UNSATISFIABLE`/`UNIQUE`/`MULTIPLE_OR_MORE`-Klassifikation,
  Ein-Zellen-Pfad, lange eindeutige Kette, isolierte Schleife, getrennte Komponente,
  Determinismus (identische Metriken bei Wiederholung), Metamorphose
  (Spiegelung/Rotation mit invarianter Lösungsklasse), Abbruchlimit.
- Ausführung: lokaler Unity-EditMode-Lauf mit Unity 6000.3.23f1; das Ergebnis wird
  mit Protokoll benannt. Nicht ausführbare Nachweise werden als NOT_EXECUTED gemeldet,
  niemals simuliert.

### Lokaler EditMode-Nachweis (2026-09-27)

Headless-Lauf auf Unity 6000.3.23f1 (`-batchmode -runTests -testPlatform EditMode`,
NUnit-Engine 3.5.0.0, Lauf 2026-09-27 19:18:46Z): **54 Tests, 54 PASSED, 0 FAILED,
0 SKIPPED** für `STP.Tests.Domain.EditMode` und `STP.Tests.Solver.EditMode`
(Assemblies aus `Library/ScriptAssemblies` dieses Projekts kompiliert). Ein erster
Lauf scheiterte an drei testseitigen Erwartungsfehlern (falsche Spaltenerwartung,
`Single()` statt `Any()` bei zwei Ereignissen, ein unbeabsichtigt bereits lösender
erster Zug); Produktionscode war davon nicht betroffen. Nach den Testkorrekturen
ist der Lauf vollständig grün. GitHub-CI-Nachweis und PlayMode-/Gerätenachweise
stehen weiterhin aus (kein CI-Runner konfiguriert; kein falscher PASS gemeldet).

## Risikoklasse

Mittel. Reiner, szeneunabhängiger C#-Kern ohne Unity-, Datei- oder SDK-Zugriff;
Fehlwirkungen sind durch EditMode-Tests und Determinismusvertrag begrenzt. Keine
Persistenz-, Konsent- oder Monetarisierungsberührung.

## Definition of Done

- Alle Akzeptanzkriterien sind nachgewiesen oder mit konkretem Grund als offen
  dokumentiert; keine simulierten Nachweise.
- Der kanonische Validatorlauf mit dem WP-009-Manifest und `git diff --check`
  gegen den `baseCommit` bestehen.
- Die lokal ausführbaren EditMode-Tests sind tatsächlich gelaufen; ihr Ergebnis ist
  im Work Package dokumentiert.
- `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md` sind
  fortgeschrieben; Ergebnis, Validierung und offene Punkte sind so dokumentiert,
  dass ein neuer Agent ohne Chat-Verlauf übernehmen kann.

## Abschlussdokumentation

### Ergebnis

WP-009 ist abgeschlossen. Der fachliche Puzzle-Kern ist als reine, deterministische
C#-Domain- und Solverbibliothek implementiert:

- `STP.Puzzle.Domain` (16 Dateien): Domainwerte (`Direction`, `TrackShape`,
  `CellContent`, `GridSize`, `CellCoordinate`, `Endpoint`), vollständig validierte
  `PuzzleDefinition` inklusive Ein-Zellen-Sonderfall, die sechs objektiven Diagnosen,
  die exakte Completion-Prüfung sowie der immutable Command-/Session-Kern
  (`ApplyCellContent`, `ApplyCellContentBatch` atomar, `UndoLastAction` mit
  256er-Stack, Revisionsablehnung, No-op, Sticky-Flags, Timer erst bei echter
  Zelländerung, `PuzzleSolved` höchstens einmal je Versuch).
- `STP.Puzzle.Solver` (4 Dateien, `solver-v1`): Constraint-Propagation
  (Raster-/Endpointports, Nachbarkonsistenz, Zeilen-/Spaltenzahlen, Subtourverbot,
  globale A-B-Erreichbarkeit), Depth-First Search mit Minimum Remaining Values,
  festgelegten Tiebreakern und Wertreihenfolge, Lösungslimit zwei mit
  `UNSATISFIABLE`/`UNIQUE`/`MULTIPLE_OR_MORE`/`INDETERMINATE` und Rohmetriken.
- EditMode-Tests in `STP.Tests.Domain.EditMode` und `STP.Tests.Solver.EditMode`
  (64 eigene Testmethoden): Formgeometrie, Definitionsinvarianten, Diagnosen,
  Completion positiv/negativ, Command-Atomarität, Undo, Timer, Immutability,
  Command-Validierung, Solver-Klassifikation 0/1/2+, Determinismus, Metamorphose
  und unabhängige Brute-Force-Gegenprobe auf kleinen Rastern.

### Review und Recheck

Ein unabhängiger Read-only-Review der Implementierung ergab vier konkrete Befunde;
die eigentliche Lösungsprüfung und Solver-Klassifikation wurde dabei ohne Befund
geprüft (u. a. 19.832 kleine Definitionen gegen einen unabhängigen Pfad-Enumerator,
ohne Abweichung). Alle vier Befunde sind behoben und per gezieltem Recheck bestätigt:

1. Immutability: öffentlich exponierte Collections von `PuzzleSessionState`,
   `PuzzleDefinition` und `CellDiff` waren per Rückcast mutierbar; behoben durch
   defensive Kopien, schreibgeschützte Exposition und Beschränkung von `With(...)`
   auf den internen Bereich.
2. Malformed Commands brachen mit ungefangenen Ausnahmen ab; behoben durch
   deterministische Ablehnung mit Diagnosecode `INVALID_COMMAND` (Command-Hülle,
   Änderungsliste, Enum-Bereich).
3. `TrackShapeGeometry.FromPorts` akzeptierte identische Ports; behoben durch
   Ablehnung mit `ArgumentException`.
4. Fehlerhafter Summengleichheitstest (beide Summen faktisch 1); behoben durch
   isolierten Testfall mit ausschließlich abweichenden Summen.

Reviewfix-Commit: `38c004c762754173d5c37518f4e3248228d8c0af`. Der gezielte
unabhängige Recheck dieser vier Punkte ist erfolgreich abgeschlossen; es wurde
kein weiterer konkreter WP-009-Codeblocker festgestellt.

### Validierung

- **Lokaler Unity-Nachweis (durch den ausführenden Agenten):** Headless-EditMode-Lauf
  auf Unity 6000.3.23f1 (2026-09-28 19:22:06Z, NUnit 3.5.0.0): **65 Tests, 65 PASSED,
  0 FAILED, 0 SKIPPED**. Davon 64 eigene STP-Testmethoden; der 65. Fall ist
  `AddressableAssets.DocExampleCode.TestStub.RequiredTest`, ein vom
  Addressables-Paket mitgelieferter Doc-Example-Test (erklärt die Zählungsdifferenz
  zur Methodenzahl).
- **Wahrheitsgemäße Trennung:** Der unabhängige Recheck hat den Code auf GitHub
  geprüft, Unity aber nicht selbst erneut ausgeführt; der 65/65-Nachweis stammt
  ausschließlich aus dem lokalen Lauf des ausführenden Agenten.
- **GitHub-CI:** Für die GitHub-Unity-CI liegt weiterhin kein nachgewiesener PASS vor
  (keine Unity-Lizenz/Runner konfiguriert; der `unity-evidence-guard` bleibt
  fail-closed). Das ist ein dokumentierter Infrastrukturzustand und **kein Blocker
  für die weitere Spieleentwicklung**; die Unity-Nachweise bleiben REQUIRED_LATER.
- `git diff --check` gegen den `baseCommit` besteht; sämtliche Delta-Dateien liegen
  innerhalb des verankerten WP-009-Production-Scopes.

### Verbleibende Punkte (keine WP-009-Blocker)

- Level-v2-Parser, JCS-Hashverträge, proof-v1, LevelValidationPipeline, Generator
  und Levelauthoring folgen in späteren Work Packages (siehe Scope-Abgrenzung).
- Hint-System bleibt wegen `BLOCKER-PROD-001` fail-closed.
- GitHub-Unity-CI-Nachweise (Compile/EditMode/PlayMode/Android/iOS) bleiben offen,
  solange Lizenz und Runner fehlen; sie blockieren die weitere Spieleentwicklung nicht.
