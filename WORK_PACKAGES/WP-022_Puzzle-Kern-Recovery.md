# WP-022 – Puzzle-Kern-Recovery

## ID

`WP-022`

**Bearbeitungsstatus (2026-10-07):** Vollständig definierter Recovery-Auftrag; gemeinsame Erstverankerung mit eigenem Production-Scope-Manifest auf `codex/wp-022-puzzle-kern-recovery`. Die gegenwärtige Beauftragung umfasst ausschließlich Definition, Verankerung und Governanceprüfung. **Implementierung nicht begonnen und erst durch separaten Implementierungsauftrag zulässig.** WP-022 ist als Produktionspaket weder abgeschlossen noch integriert.

## Ziel

Auf der frischen Branch vom tatsächlich verifizierten `main`-HEAD **`8c0d73ac72c01c8bc079a69a00ccf4e5b759c1aa`** entsteht nach separater Implementierungsbeauftragung ein neu implementierter, reiner Puzzle-Kern auf dem integrierten WP-021-Scaffold: validierte Domainwerte, sichtbare objektive Diagnostik, exakte Completion, immutable Session-/Command-Verarbeitung und deterministische Constraint-/MRV-DFS-Klassifikation bis Lösungslimit zwei. Alle fachlichen Nachweise werden auf dieser Branch neu erzeugt.

Der Solver-Ausgangskern ist eine **historische Recovery-Basis `solver-v1`**, keine Freigabe von `solver-v1` für neue Produktion, Proofgenerierung oder Spielerhints. ADR-031 bleibt bindend: WP-016 entwickelt diese Basis zu `solver-v2`; ausschließlich WP-017 schließt den Produktionsproofvertrag. WP-022 liefert keinen importierbaren Content und aktiviert keine Puzzlefunktion im Bootstrap oder in der UI.

## Voraussetzungen

1. Pflichtlektüre in der Reihenfolge aus [AGENTS.md](../AGENTS.md): Projektübergabe, Konzeptindex, Master-Spezifikation, [CURRENT_STATE](../PROJECT_CONTROL/CURRENT_STATE.md) und dieses Work Package; anschließend [WORK_QUEUE](../PROJECT_CONTROL/WORK_QUEUE.md), [WORK_PACKAGE_RULES](../PROJECT_CONTROL/WORK_PACKAGE_RULES.md), [DEFINITION_OF_DONE](../PROJECT_CONTROL/DEFINITION_OF_DONE.md), [AI_HANDOVER_RULES](../PROJECT_CONTROL/AI_HANDOVER_RULES.md) und [Quellenautorität/Recovery](../PROJECT_CONTROL/WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md).
2. Fachlektüre: [Puzzle Engine](../ARCHITECTURE/PUZZLE_ENGINE.md), [Game State Model](../ARCHITECTURE/GAME_STATE_MODEL.md), [Solver Architecture](../ARCHITECTURE/SOLVER_ARCHITECTURE.md), [Module Boundaries](../ARCHITECTURE/MODULE_BOUNDARIES.md), [Test Strategy](../ARCHITECTURE/TEST_STRATEGY.md), [Level Data Format](../ARCHITECTURE/LEVEL_DATA_FORMAT.md), [Content Pipeline](../ARCHITECTURE/CONTENT_PIPELINE.md), [offene Produktblocker](../ARCHITECTURE/OPEN_BLOCKERS.md), Konzeptdateien [03](../Stammstrecken_Puzzle_Konzept_00-15/03_Raetselkern_und_Interaktionsmodell.md) und [04](../Stammstrecken_Puzzle_Konzept_00-15/04_Schwierigkeit_Feldgroessen_und_Levelgenerierung.md); ADR-[005](../DECISIONS/ADR-005-deterministisches-command-state-modell.md), [007](../DECISIONS/ADR-007-solver-und-eindeutigkeitspruefung.md), [013](../DECISIONS/ADR-013-zyklusfreie-ports-und-modulgrenzen.md), [021](../DECISIONS/ADR-021-puzzleidentitaet-und-proofartefakte.md), [026](../DECISIONS/ADR-026-validator-evidenz-und-scope-vertrauensanker.md), [030](../DECISIONS/ADR-030-wp-scope-trust-anchor.md) und [031](../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md), [Validator-README](../tools/architecture-validation/README.md) sowie WP-[015](./WP-015_Level-v2-Foundation-und-Hashvertraege.md), [016](./WP-016_Solver-v2-Metriken-und-Deduktionsspur.md), [017](./WP-017_Proof-v1-Regeneration-und-Strict-Validation.md).
3. WP-014/WP-018 und [WP-021](./WP-021_Unity-Scaffold-Recovery.md) sind integriert. Tatsächlich frisch geprüft: PR #11 integriert als `cd1a048fe7c77f971c00613de5687a0cac1ec395`; formaler Closeout [PR #12](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/12) integriert am 2026-10-07 als `8c0d73ac72c01c8bc079a69a00ccf4e5b759c1aa`. Git-Historie und GitHub-Metadaten stimmen überein; der Main-Baum entspricht dem Closeout-Head `7575f45d2f8e771e85a9b6ae483ff1ade409aaa2`. WP-021 dokumentiert unabhängige Abschluss-QC PASS, Geschäftsführungsfreigabe und keinen technischen Restblocker vor Integration.
4. Vor Branchanlage war der Arbeitsbaum sauber; WP-022-Datei und Manifest existierten auf dieser Basis nicht. Die Branch beginnt exakt bei dem unter Ziel genannten HEAD. Die integrierten Domain-/Solver- und zugehörigen EditMode-Assemblies sind noch typenlos; Unity `6000.3.23f1`, Modulgraph 14+9, Ports, Composition und bestehende CI bleiben Grundlage. Vor Implementierung sind vorhandene Lizenz-/Runner-Zugänge zu prüfen; fehlende Zugänge werden als Ausführungsblocker dokumentiert, niemals als PASS ersetzt.
5. Dieses Work Package und [das eigene Production-Scope-Manifest](../tools/architecture-validation/scopes/WP-022.production.scope.json) werden **gemeinsam erstmals im selben ersten Add-Commit** eingeführt. Dessen einziger Elterncommit ist `baseCommit = 8c0d73ac72c01c8bc079a69a00ccf4e5b759c1aa`. Das Manifest bleibt anschließend byteunveränderlich. Vor jedem Produktionsdiff sind historischer WP-Blob/Manifestlink, Add-Status, Elternbasis und Bytegleichheit zu prüfen.
6. [WP-020](./WP-020_Puzzle-Kern-Recovery-auf-aktuellem-Main.md) bleibt ausschließlich archiviertes Planungs-/Vergleichsmaterial. Historische Branches WP-008, WP-009, WP-013 und `chore/ci-readiness-unity` sind keine Lieferquellen. Kein historisches PASS ersetzt eigene Evidenz.
7. Ein separater Implementierungsauftrag muss vor dem ersten C#-/Test-/Unity-CI-Implementierungsdiff ausdrücklich erteilt sein. Die Verankerung selbst autorisiert nur die unten getrennte Phase A.

## Scope

**Phase A – jetzt autorisierte Definition und Verankerung:**

1. Ausschließlich dieses WP und sein Manifest im ersten Branchcommit anlegen; anschließend Trust Anchor tatsächlich prüfen und remote sichern.
2. In einem Folgecommit CURRENT_STATE und WORK_QUEUE mechanisch auf „definiert/verankert, Implementierung nicht begonnen“ nachführen und in `validate.yml` ausschließlich `STP_SCOPE_MANIFEST` auf das eigene Manifest umschalten. `STP_SCOPE: production`, PR-Head-Checkout, Pins, Permissions und Fail-closed-Verhalten bleiben unverändert.
3. Architecture-only- und kanonischen Production-Scope-Lauf einschließlich Self-/Negativtests, positiven Scope-Lauf, vollständigen Diff-/Quellen-/Secret- und Trustnachweis ausführen; Ergebnisse und Einschränkungen hier dokumentieren. Ein Definitions-PR gegen `main` dient der Reviewbarkeit, **nicht** einer Integrationsfreigabe vor Implementierung/QC.

**Phase B – erst nach separatem Implementierungsauftrag:**

1. **Domainwerte/Definition:** `GridSize` (beide Achsen 2..32), rasterrelative Koordinaten, N/E/S/W, genau sechs Trackformen und die neun CellContent-Werte; Endpointabbildung und vollständig validierte `PuzzleDefinition` nach Puzzle Engine Abschnitte 2/3. Unterschiedliche A/B-Außenanschlüsse dürfen dieselbe Innenzelle treffen, sofern eine erlaubte Form beide Ports verbindet. Ein Ein-Zellen-Pfad bedeutet **keinen 1×1-Raster**. Unbekanntes Ruleset, falsche Längen/Werte/Summen oder unbelegbare Endpointzellen werden typisiert abgewiesen. Eindeutigkeit ist kein Konstruktionskriterium.
2. **Objektive Diagnostik/Completion:** sichtbare Mengen zählen Tracks plus MARK_OCCUPIED; Completion zählt ausschließlich Tracks. Alle sechs Diagnosebedingungen aus Puzzle Engine Abschnitt 5, stabile Koordinaten-/Codesortierung, Graphgrade, Außenports, exakte Counts und vollständige Traversierung nach Abschnitten 6/7 implementieren. Offene Nachbarannahmen bleiben unbewertet; Hilfsmarkierungen außerhalb der fertigen Strecke sind toleriert. Kein Lösungs-/Solverwissen in Live-Diagnostik.
3. **Session/Commands:** immutable `PuzzleSessionState`, sortierte atomare CellDiffs und CommandResult-/Eventtypen nach ADR-005 und Game State Model. `ApplyCellContent`, `ApplyCellContentBatch`, `UndoLastAction` vollständig implementieren; Revision/Command-ID, zulässige Ready-/Active-Phasen, No-op, ganze Batchabweisung, maximal 256 Undo-Schritte, sticky Markierungs-/Hintflags, correctionCount und einmaliges PuzzleSolved. Undo erhöht die Revision, setzt Timer/Flags nicht zurück. Zusätzlich ausschließlich die **reinen Domainanteile** von `PauseForLifecycle`/`ResumeFromLifecycle` für aktive Zeit und Suspended-Übergänge; kein Savezugriff oder Plattformadapter. Abschluss endet an `SolvedPendingCommit`; TrainRide, Results, ProgressCommitted und weitere Applicationübergänge werden nicht implementiert.
4. **Identität/Zeitgrenze:** Puzzle-ID, profiliertes Public-Puzzle-Hashtripel und Attempt-ID werden als bereits gelieferte Werte gehalten; keine Hashberechnung, Registry, JSON-/Savekanonisierung oder Migration. Monotone Zeit wird als expliziter Wert injiziert; keine Abhängigkeit auf den Application-Port IClock, keine Wanduhr-/UUID-Erzeugung im Kern. UTC-Diagnose-/Savefelder werden nur als extern gelieferte Daten gehalten, nie als Timerquelle verwendet. Mode ist Datenvertrag, keine Fortschritts-/Rewardpolicy. hintCount wird erhalten, aber kein RequestHint/HintPresented-/Creditfeature umgesetzt.
5. **Deterministischer Solver-Ausgangskern:** ausschließlich öffentliche PuzzleDefinition als Input, sieben Solverwerte EMPTY plus sechs Formen, Constraints und Fixpunkt-Propagation in der Reihenfolge aus Solver Architecture Abschnitte 2/3; interne sichere Reduktionen mit stabilen Regelcodes, referenzierten Zellen/Linien und Konklusion. MRV nach y/x, Werte `EMPTY, TRACK_NS, TRACK_EW, TRACK_NE, TRACK_ES, TRACK_SW, TRACK_WN`, DFS, sofortiges Limit zwei und 0/1/2+-Klassifikation nach Abschnitt 4. Cancellation, Zeit-/Ressourcenlimit liefern INDETERMINATE, auch nach einer bereits gefundenen Lösung. Ergebnis enthält Klassifikation, begrenzte Lösungszahl und deterministischen A–B-Pfad; technische Rohinstrumentierung/Strukturdaten dürfen intern gemessen werden, sind keine freigegebenen Schwierigkeits- oder Proofmetriken. Keine auditable solver-v2-Rootspur, keine vier ADR-031-Metrikverträge und kein Kampagnen-/Hintgate vor WP-016. `solver-v1` bezeichnet nur diese Recovery-Basis und erreicht keinen neuen Produktionsproof oder Featureaufruf.
6. **Eigene Tests:** ausschließlich Domain-/Solver-EditMode-Tests einschließlich Buildern und kleinen `FIXTURE_ONLY`-Testdaten innerhalb dieser beiden Testmodule; unabhängiger Kleinstraum-Pfad-Enumerator, Properties, Replay und Metamorphosen. Der Enumerator darf keine Completion-/Constraintimplementierung des Prüflings wiederverwenden und wird nicht ausgeliefert. Testprojektionen für Snapshot-/Eventbytevergleich sind rein testseitig, deterministisch und keine JCS-/Saveimplementierung.
7. **Eng begrenzte spätere Unity-CI-Anpassung:** vorhandener EditMode-Aufruf erfasst beide bestehenden Assemblies bereits. In `unity.yml` ausschließlich ein fail-closed Ergebnisgate ergänzen, das tatsächlich ausgeführte, erfolgreiche Testfälle **beider** Assemblies verlangt (fehlend/leer/Skipped/Failed ist FAILURE), sowie beide bestehenden Coverage-Gates um die jetzt instrumentierbaren Domain-/Solver- und zugehörigen Testassemblies erweitern. Die acht bisherigen Pflichtassemblies, echte Messdatenpflicht einschließlich expliziter Nullen, Collector-Pins, QA-Smoke, Compile, Android/iOS und Personal-Seat-Serialisierung bleiben erhalten. Keine Assemblyreferenzänderung oder Test-/Coverageabschwächung.
8. **Evidenz/Übergabe:** echte Coverage- und erste Domain-/Solver-Mutationsbaseline, Regel-Mutationsnachweise, 10×10-CI-Budgetmessung, eigener Compile/EditMode/PlayMode-/Android-IL2CPP-/iOS-Compile-Nachweis. Mechanische Statusnachführung nur in diesem WP, CURRENT_STATE und WORK_QUEUE. Rohlogs, Reports, Caches und Builds bleiben ignoriert. Physische Geräte-/SDK-/Storegates werden wahrheitsgemäß getrennt als REQUIRED_LATER/NOT_EXECUTED ausgewiesen.

## Betroffene Dateien/Module

Die Pfadallowlist des [WP-022-Manifests](../tools/architecture-validation/scopes/WP-022.production.scope.json) ist die unveränderliche technische Außengrenze; die obigen Tätigkeitsgrenzen gelten zusätzlich.

| Pfad/Modul | Zulässige Änderung |
|---|---|
| `WORK_PACKAGES/WP-022_Puzzle-Kern-Recovery.md` | **Neu in Phase A:** Auftrag, Quellenabgleich, Anker-/Prüfnachweise und spätere Fortschritts-/Abschlussdokumentation. |
| `tools/architecture-validation/scopes/WP-022.production.scope.json` | **Neu in Phase A:** eigene enge Allowlist; nach gemeinsamem Add-Commit keine Änderung. |
| `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` | Phase A und B: ausschließlich WP-022-Status, Nachweise, nächster Schritt; keine Folgefreigabe vor Integration. |
| `.github/workflows/validate.yml` | Phase A: ausschließlich WP-022-Manifestumschaltung. |
| `Assets/StammstreckenPuzzle/Scripts/Puzzle/Domain/*.cs` und `*.cs.meta` | **Neu erst in Phase B:** Werte, Definition, Diagnostik, Completion, Session, Commands und reine Timer-/Lifecycleanteile in STP.Puzzle.Domain. |
| `Assets/StammstreckenPuzzle/Scripts/Puzzle/Solver/*.cs` und `*.cs.meta` | **Neu erst in Phase B:** Recovery-Solver, interne Reduktions-/Ergebnistypen, Limits in STP.Puzzle.Solver. |
| `Assets/StammstreckenPuzzle/Tests/EditMode/Domain/*.cs` und `*.cs.meta` | **Neu erst in Phase B:** Domain-/Command-/Replay-/Propertytests und testseitige Builder/Goldens. |
| `Assets/StammstreckenPuzzle/Tests/EditMode/Solver/*.cs` und `*.cs.meta` | **Neu erst in Phase B:** Solverfälle, unabhängiger Enumerator, Metamorphosen, Limits, Budget- und Determinismustests. |
| `.github/workflows/unity.yml` | Erst Phase B: ausschließlich EditMode-Ausführungsevidenzgate und Erweiterung der beiden Coverage-Pflichtmengen wie Scopepunkt B7. |

Alle neuen C#-Dateien liegen direkt in diesen vier bestehenden Modulverzeichnissen und erhalten eingecheckte Meta-Dateien. Bestehende `.asmdef`, `csc.rsp`, deren Meta-Dateien und Verzeichnismetadaten werden **nicht geändert**. Keine gemeinsamen Fixtures-/Golden-Roots, neuen Ordner/Assemblies, Pakete oder Tools sind erforderlich. Weitere Fach-/ADR-/Produktdateien werden nur gelesen/geprüft, niemals geändert.

## Ausdrücklich nicht erlaubte Änderungen

- In Phase A keinerlei Domain-, Solver-, Command-, Session-, Test-C# oder sonstige Produktions-/Unity-CI-Implementierung.
- **WP-015 exklusiv:** Level-v2-Parser/DTO/Schema/Semantikadapter, Domain-Mapping aus JSON, JCS, SHA-256-Projektionen/-Registry, Levelmigration. Die reine Domain-Konstruktionsvalidierung gehört hingegen hierher.
- **WP-016 exklusiv:** solver-v2-Kennung, auditable Roottrace-/Prämissenstruktur, Root-/DFS-Metriktrennung, vier ADR-031-Metriken und Kampagnenprüfung requiredGuessDepth = 0. Keine Vorabimplementierung hinter einem anderen Namen.
- **WP-017 exklusiv:** Proof-v1-Schemaänderung einschließlich minimum: 0, Proof-DTO/Parser/Serializer/Generator/Binding/Hash, Proofregeneration/-Goldens, LevelValidationPipeline, Strict-Einzel-/Batchsemantik, Runtimekatalog-/Release-Lock-Importfreigabe.
- Keine Generator-/Authoring-/Contentproduktion, Season-1-Level, Zeitkalibrierung, Hintauswahl/-Entitlement/-Rewarded-Hint, Daily- oder Endlessfeature.
- Keine Application-, Persistence-, Infrastructure.Content-, Editor-, UI-, World-, Bootstrap-, Platform-, Audio- oder Mobile-Service-Fachlogik; keine Aktivierung des Kerns in Composition/QA-Szene; keine Reward-/Fortschritts-/Save-/SDK-Operationen oder externen Calls.
- Keine neuen Ports, Assemblies, Referenzen, Abhängigkeiten, Toolchain-/Paket-/Projekt-/Szenenänderungen; kein Redo und keine zusätzlichen Produktregeln/Sonderfelder.
- Keine Änderungen an AGENTS, ARCHITECTURE, DECISIONS, Produktkonzept, Validator/Schema/Lock/Fixtures, historischen Work Packages oder Scope-Manifesten. BLOCKER-PROD-001/002/003 bleiben unverändert offen und fail-closed.
- Kein Merge, Cherry-Pick, Rebase, Copy oder Konfliktauflösung aus historischen Branches; keine Übernahme ihrer Dateien, Manifeste, Logs oder PASS-Aussagen. Keine Änderung von main, kein Force Push oder Merge durch den Implementierungsagenten.
- Kein Scope-/Akzeptanz-/Testabsenken für einen grünen Nachweis; keine nicht aufgelisteten Dateien oder beiläufigen Refactorings.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| `AK-01` | Historischer erster Branchcommit führt exakt WP und eigenes Manifest gemeinsam als A ein; Elterncommit/baseCommit ist der verifizierte Main-HEAD, historischer Manifestlink und WP-ID stimmen, Manifest bleibt bytegleich; Branch/Anker remote verifiziert. |
| `AK-02` | Domainkonstruktion erfüllt alle Puzzle-Engine-Invarianten und 2..32-Grenzen; ungültige Inputs liefern typisierte Fehler, Außenanschlüsse derselben Zelle sind korrekt, alle Collections gegen externe Mutation geschützt. |
| `AK-03` | Objektive Mengen und sechs Diagnosen sind allein aus sichtbarem State korrekt/stabil sortiert; kein Authoring-/Solverwissen und keine Falschbewertung einer unbestimmten Nachbarzelle oder plausiblen Markierung. |
| `AK-04` | Completion akzeptiert nur den vollständigen einfachen A–B-Pfad mit allen konkreten Tracks; Ein-Zellen-Pfad und externe Hilfsmarkierungen funktionieren; Counts, offene Enden, falsche Ports, Schleifen und weitere Komponenten werden abgewiesen. |
| `AK-05` | Einzel/Batch/Undo sind immutable und atomar, No-op ohne Event/Revision/Undo/Timerstart, stale Revision und doppelte Command-ID ohne zweite Mutation; Batch 1..N eindeutig, maximal 256 Diffs; echte Änderungen/sticky Flags/correctionCount korrekt und PuzzleSolved höchstens einmal je Versuch. |
| `AK-06` | Timer startet nur bei echter Zelländerung, verwendet injizierte monotone Werte, akkumuliert nur aktive Perioden und zählt Pause nicht; Undo erhält gestarteten Timer/Flags; unzulässige Sessionphasen mutieren nicht; Replay liefert gleiche testseitige Snapshotbytes/Diagnosen/Events. |
| `AK-07` | Solver erfüllt Constraints, Fixpunkt-/MRV-/Wertordnung und Limit zwei; 0/1/2+-Fälle korrekt, A–B-Pfad deterministisch, Ressourcenlimit/Cancellation stets INDETERMINATE; nur Recovery-solver-v1, keine solver-v2-/Proof-/Hint-/Produktionsfreigabe. |
| `AK-08` | Unabhängiger Enumerator prüft erschöpfend alle Endpoint-/Count-Kombinationen auf 2×2 und 2×3 (Transposition 3×2 über Metamorphose); alle Rotationen/Spiegelungen/A–B-Tausche bewahren Lösungsklasse. Für 3×3 zusätzlich fest protokollierter Teilkorpus. Keine Wiederverwendung des Prüflings im Orakel. |
| `AK-09` | Echte Branch-CI: Compile, beide EditMode-Assemblies mit ausgeführten Tests, unveränderter QA-PlayMode-Smoke, Android Development IL2CPP und iOS Export/Compile PASS; neue Coveragepflichtdaten vorhanden, erste Domain-/Solver-Mutationsbaseline und für jede Regel ein erkannter Mutant dokumentiert; 10×10-CI-Solverbudget p95 < 250 ms, Maximum < 2 s belegt. |
| `AK-10` | Beide kanonischen Validator-Modi mit Self-/Negativtests auf Ubuntu PASS, kompletter Basisdiff/Trust/Secret-/Quellencheck PASS; ausschließlich Manifestpfade und erlaubte CI-Deltas, keine historische Übernahme/Architektur-/Produktänderung; Übergabe konsistent. |

Für Phase A werden AK-01 und der auf Definition/Verankerung anwendbare Teil von AK-10 geprüft. **AK-02–AK-09 und Implementierungs-/QC-/Integrationsanteile von AK-10 bleiben offen**, ohne dass daraus ein Definitionsblocker abgeleitet wird.

## Tests

**Verankerung (Phase A):**

1. `python tools/architecture-validation/validate.py --self-test` (Architecture-only).
2. `python tools/architecture-validation/validate.py --scope production --scope-manifest tools/architecture-validation/scopes/WP-022.production.scope.json --self-test` sowie derselbe positive Lauf ohne Selftest, aus der gepinnten Lockumgebung. Die vollständigen Selftests müssen auf der vorgesehenen Ubuntu-24.04-CI bestehen; der bereits auf der Basis vorhandene Windowsfehler V03-005-ABSOLUTE wird separat als FAIL dokumentiert, nicht repariert oder ausgenommen.
3. Gitnachweis: gemeinsamer erster Add-Commit mit genau zwei Pfaden, beide im Elternbaum absent, historischer lokaler Manifestlink/IDs, genau ein Manifest-Add-Anker, ein Elterncommit/baseCommit, HEAD-Ancestry und historische/aktuelle Manifestbytes. Nach Push tatsächlichen Remote-HEAD prüfen.
4. `git diff --check 8c0d73ac72c01c8bc079a69a00ccf4e5b759c1aa`; exakte Phase-A-Fünf-Datei-Allowlist; bytegleiche Produktions-/Test-/Architektur-/Produkt-/Validator-/Unity-CI-/Altmanifestquellen. validate.yml gegenüber Basis nach Rücknahme ausschließlich der Manifestzeile bytegleich. Secret-Patternprüfung durch Validator plus Diffreview.
5. Manueller Quellen-/Grenzenreview gegen ADR-005/007/021/030/031, WP-021-Closeout und WP-015/016/017. Keine eigenständige unabhängige Astra-/Sol-Abnahme behaupten. Unity-Ausführung ist kein eigener Definitionsnachweis; automatisch ausgelöste Unity-CI wird getrennt berichtet und ersetzt keine künftige Implementierungsevidenz.

**Implementierung (Phase B):**

- Positiv-/Negativkatalog für jede Definitionsinvariante, alle sechs Trackports und Transformationen; technischer Ein-Zellen-Pfad auf gültigem Raster, unbekanntes Ruleset, Endpoint-/Countfehler und externe Collectionmutation.
- Sichtbare Mengen inkl. Grau; offene/exakte/überschrittene Linien, alle sechs Diagnosen, unbestimmte Nachbarn, geschlossene Schleife, getrennte Tracks, falsche Außenkanten, Markierungen auf/außerhalb finalem Pfad. Keine Lösung wird der Diagnostik übergeben.
- Command-/Replayfälle: Einzel, CLEAR zu UNSET, gemischter/all-no-op Batch, leere/doppelte/ungültige Batchkoordinaten, stale Revision, doppelte ID, Phasenabweisung, 256/257 Undo-Schritte, sticky Flags, correctionCount, einmalige Lösung, Pause/Resume ohne Hintergrundzeit; feste injizierte Zeit-/Identitätsinputs. Testbytes verwenden eine explizite testseitige Projektion ohne JCS-/Savecode.
- 0/1/2+-Solverfälle, lange Kette, Ein-Zellen-Pfad, 10×10-Grenzfixture; deterministische Wiederholung von Lösung/Reduktionsreihenfolge, MRV-/Wertordnung, Limit zwei, Cancellation/Timeout/Nodebudget vor und nach erstem Fund. Known-Answer-Expectations werden unabhängig begründet, nicht aus dem Prüfling als Golden übernommen.
- Enumerator gemäß AK-08, feste Seeds und minimierte Gegenbeispiele; Rotation/Spiegelung/A–B-Tausch ohne Gleichsetzung rotationsabhängiger Laufstatistik. Jede einzelne entfernte Definitions-/Completion-/Constraint-/Commandregel muss mindestens einen fachlichen Test brechen. Mutationanzahl, erkannte/überlebende Mutanten, Ausschlüsse und Score pro Assembly werden aus tatsächlichen Läufen berichtet; keine erfundene Prozentbaseline.
- Unity 6000.3.23f1: CI-Modulgraph 14+9, echter Compile/EditMode, Coverage inklusive expliziter Messdaten für Domain/Solver/Testmodule und bisherige Pflichtassemblies; vorhandene Bootstrap-PlayMode-Regression vollständig, Android Development IL2CPP, iOS Export/Xcode Compile. Fehlende/Skipped/0 Tests sind kein PASS. CI-/Remote-HEAD, Run-/Job-/Artefakt-IDs, Testzahlen und Ergebnisse persistieren.
- 10×10-Solver-CI-Budgetmessung mit Warm-up, Wiederholungen, Rechner/Toolchain/Commit/Korpus und p95/Maximum. Domaincommand-/Completionbudgets auf niedrigstem physischem Referenzgerät sowie Hint-/Store-/SDK-Smokes bleiben getrennt REQUIRED_LATER/NOT_EXECUTED; kein Gerät und keine freigegebene Device-Farm vorhanden. Lokale CPU-Messungen sind kein Gerätebeleg.
- Danach unabhängige Astra- und Sol-QC auf identischem eingefrorenem PR-HEAD, ohne gegenseitige Revieweinsicht vor dem eigenen Bericht.

## Risikoklasse

**Hoch.** Completion, deterministische Zustandsübergänge und Eindeutigkeitsklassifikation bilden den fachlichen Kern aller späteren Content-/Proofgates. Der gegenwärtige Verankerungsschritt ändert nur Auftrag, Scopebindung und Governance-/CI-Manifestzuordnung; er enthält keine Implementierung und trifft keine neue Architektur-/Produktentscheidung.

## Definition of Done

Es gilt vollständig die [projektweite Definition of Done](../PROJECT_CONTROL/DEFINITION_OF_DONE.md).

**Phase-A-Abschluss:** Gemeinsamer unveränderlicher Anker/Remote-Nachweis, vollständiger ausführbarer Vertrag, konsistente CURRENT_STATE/WORK_QUEUE, auf das eigene Manifest umgeschaltete Governance-CI und erfolgreiche Ubuntu-Self-/Negativtests sowie positiver Scope-/Diff-/Trust-/Quellencheck sind dokumentiert. Alle zehn Pflichtabschnitte existieren; kein offener Definitionswiderspruch. Sauberer Arbeitsbaum und reviewbarer Definitions-PR. Keine Produktionsdatei geändert. Das Ergebnis heißt „definiert und verankert; separate Implementierung ausstehend“, niemals „WP-022 abgeschlossen“.

**WP-022-Gesamtabschluss erst nach Phase B:** alle AK einzeln mit echten commitgebundenen Tests/Build-/Coverage-/Mutations-/Scope-/Budgetnachweisen erfüllt, keine offenen BLOCKER/HIGH im Scope, vollständige Dokumentation und zwei unabhängige QC-Berichte verlinkt. Implementierungsagent pusht den geprüften HEAD, erstellt/aktualisiert den PR, führt aber keinen Merge aus. Erst die anschließende ausdrückliche Integrationsentscheidung und Integration nach main geben WP-015 frei. Physische Geräte-/SDK-/Storegates bleiben entsprechend dem bestehenden Vertrag getrennte spätere Nachweise; ihre Nichtausführung wird niemals als PASS angegeben.

## Quellenabgleich mit dem Planungsarchiv

| WP-020-Absicht | Heutige verbindliche Bindung in WP-022 |
|---|---|
| Scaffold als Voraussetzung | WP-021 integriert über PR #11; formaler Closeout PR #12 auf tatsächlichem Main-HEAD; WP-019 ist archiviert. |
| Domain/Diagnostik/Completion | ADR-005, Puzzle Engine, Game State Model; reine Domain, Ein-Zellen-Pfad bei Rasterachsen mindestens 2. |
| Session/Commands/Timer | Drei Zellcommands plus notwendige reine Pause-/Resumezeitgrenze; Application, Save, Rewards und Hintfeature bleiben ausgeschlossen. |
| solver-v1-Ausgangskern/Rohmetriken | Nur historische Recovery-/Testbasis für Klassifikation/Propagation/Suche; ADR-031 verbietet neue Produktionsproofs mit solver-v1. Keine neue Metrikdefinition; solver-v2-Spur und vier Proofmetriken exklusiv WP-016. |
| Tests/CI | Bestehende reale WP-021-Assemblies und Jobs; keine neuen Assemblyreferenzen nötig. Phase B erweitert ausschließlich EditMode-Ausführungs- und Coveragepflichtnachweise. |
| Scopeanker | Eigenes WP-022 und eigenes Manifest erstmals gemeinsam auf frischer Main-Branch; WP-020 und historische WP-009-Manifeste begründen keine Autorisierung. |
| Content-/Prooffolge | Harte Folge WP-022 integriert → WP-015 → WP-016 → WP-017 → Contentproduktion unverändert. |

## Rollen und Übergabe

- Geschäftsführung/Projektarchitekt verantworten den hier vollständig definierten Auftrag und die separate Implementierungs-/Integrationsfreigabe.
- Kimi oder gleichwertiger Implementierungsagent führt erst nach separatem Auftrag ausschließlich Phase B aus; keine Scope-/ADR-Entscheidungsbefugnis.
- Astra prüft unabhängig Domain-/Completion-/Session-/Solververtrag und die Grenze zu ADR-031/WP-016; Sol unabhängig Teststärke, Scope/Trust, CI und Integrationsreife. Beide prüfen denselben eingefrorenen HEAD; kein Berichtsaustausch vor eigener Abgabe. Diese Rollen werden durch den Definitionsagenten nicht simuliert.
- Ergebnis, tatsächliche Prüfläufe, Dateiliste, Einschränkungen/Blocker und nächster Schritt werden gemäß AI_HANDOVER_RULES hier und in CURRENT_STATE/WORK_QUEUE fortgeschrieben. Chat und historische Branches sind keine Übernahmegrundlage.

## Verankerungsnachweis und aktueller Ausführungsstand

### Tatsächlicher gemeinsamer Anker und lokale Prüfung – 2026-10-07

Trust-Anchor-Commit **`ed898894573f2bb3456d21f22a8522c6b8825621`**, erster Commit auf `codex/wp-022-puzzle-kern-recovery`, einziger Elterncommit/Manifestbasis **`8c0d73ac72c01c8bc079a69a00ccf4e5b759c1aa`**. Sein Diff enthält exakt zwei Adds: dieses Work Package und das eigene Manifest. Beide fehlen im Elternbaum. Der historische WP-Blob bindet die korrekten H1-/Body-IDs und den exakt lokal aufgelösten Manifestlink. Manifestblob **`1cd2def6f6f79da01e1e93173832df380e4d9ac1`**, SHA-256 der unveränderten Bytes **`003f5d47d1177d70fa49359200127a243ca4265f2124a1dc2401ac36ea2988e6`**.

| Phase-A-Prüfung | Tatsächliches Ergebnis |
|---|---|
| Basis/Closeout | Frisches Git-Fetch/ls-remote und GitHub-PR-Metadaten: main 8c0d73a, PR #11/#12 merged; integrierter Closeoutbaum identisch zu 7575f45. |
| Positiver kanonischer WP-022-Production-Scope | **PASS, 17 lokale Prüfgruppen**, eigener historischer Anker und vollständiger Branch-/Arbeitsbaumdiff gegen 8c0d73a. |
| Architecture-only und Production-Scope mit Selftest auf Windows | Jeweils **FAIL ausschließlich self-test:not-detected:V03-005-ABSOLUTE**. Derselbe Fehler schon auf unveränderter Basis; keine Ausnahme oder Validatoränderung. Vollständiger Ubuntu-Nachweis noch ausstehend. |
| Separater Trust-/Grenzenreview | **PASS**: gemeinsamer erster Zwei-Datei-Add, Elternbasis, historischer Link/IDs, genau ein erreichbarer Manifest-Add, Ancestry und Bytegleichheit; 9 erlaubte Pfadproben akzeptiert und 32 verbotene Pfadproben korrekt abgewiesen, einschließlich Content-/Proof-/Application-/Architektur-/Folge-WP-/Altmanifestpfaden. |
| Vollständiger Basisdiff/Quellen | **PASS**, exakt fünf Phase-A-Dateien; git diff --check, Secret-Patternscan und Quellenreview. Alle Produktions-/Test-/Architektur-/Produkt-/Validator-/Unity-CI-/Altmanifestquellen ohne Diff; validate.yml nach Rücknahme allein der Manifestzeile bytegleich. Keine Mergecommits nach Basis, keine historische Dateiübernahme. |
| Akzeptanz-/DoD-Abgrenzung | AK-01 lokal und Phase-A-Anteil AK-10 lokal erfüllt; Remote-/Ubuntu-Nachweis folgt. AK-02–AK-09, Implementierung, unabhängige QC und Integration **nicht ausgeführt/offen**. |

Der erste sandboxierte Scopeversuch konnte den Manifestpfad nicht auflösen; der bestehende externe Lockinterpreter wurde anschließend mit tatsächlichem Lesezugriff ausgeführt. Dessen erster Scopeversuch zeigte den noch veralteten lokalen main-Verweis cd1a048 gegenüber frisch abgerufenem origin/main 8c0d73a. Ausschließlich dieser lokale Verweis wurde per nicht erzwungenem Fast-Forward auf den **bereits integrierten** Stand synchronisiert; kein neuer Main-Commit und kein Remote-Main-Schreibzugriff. Danach bestand der positive Scope, die zwei Selftestmodi scheitern nur am bekannten Windowsbefund. Diese Zwischenversuche sind kein PASS und erfordern keine Produkt-/Architekturentscheidung.

Ignorierte Reproduktionsnachweise: `Logs/wp022-pre-anchor-{architecture-selftest,structure}.log`, `Logs/wp022-anchor-{architecture-selftest,scope-selftest,scope-positive}.log`, `Logs/wp022-anchor-review.py` und `.json`. Kein temporäres Artefakt wird versioniert. Keine Produktionslogik/Test-C# implementiert, unity.yml unverändert.

### Evidenzkategorien und Übergabe

- **LOCAL_DOCUMENT_STRUCTURE:** Pflichtstruktur/IDs, lokale Links und konsistenter Status tatsächlich geprüft.
- **LOCAL_ARCHITECTURE_SEMANTICS:** positive Schema-/Fixture-/Modell-/Crosscheck-Gruppen ausgeführt; Windows-Selftestgrenze separat als FAIL, keine C#-Solvervalidierung behauptet.
- **MANUAL_ARCHITECTURE_REVIEW:** Quellenabgleich und Scopegrenzen gegen ADR-005/007/021/030/031 und Folge-WPs geprüft; keine neue Architekturentscheidung und keine unabhängige Astra-/Sol-Abnahme.
- **LOCAL_SCOPE:** vollständiger realer Diff/Trust-/Manifestcheck PASS, enge eigene Allowlist.
- **CONTRACT_ONLY:** Domain-/Solver-/Session-/Test-/Proofverträge; deren Implementierung bleibt ausstehend.
- **REQUIRED_LATER/NOT_EXECUTED:** Phase-B-Produktionscodevalidierung, physische Geräte-/SDK-/Storegates; automatisch ausgelöste Scaffold-CI wird separat berichtet.
- **BLOCKED:** BLOCKER-PROD-001/002/003 bleiben offen und fail-closed; sie betreffen ausgeschlossene Folgefeatures und sind keine WP-022-Definitionsblocker. Kein offener Definitionswiderspruch aus dem Quellenreview.

**Nächster zulässiger Schritt:** Phase-A-Remote-/Ubuntu-Nachweise sichern und dokumentieren; danach separater Implementierungsauftrag für Phase B auf dieser Branch. WP-022-Gesamtstatus bleibt offen/nicht implementiert; WP-015/016/017 bleiben bis zur jeweiligen Vorgängerintegration gesperrt.
