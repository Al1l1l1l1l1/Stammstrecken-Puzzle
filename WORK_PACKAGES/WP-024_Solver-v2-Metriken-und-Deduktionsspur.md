# WP-024 – Solver-v2-Metriken und Deduktionsspur

## ID

`WP-024`

**Bearbeitungsstatus:** **Phase A (Definition und Verankerung) abgeschlossen. Die Implementierung (Phase B) ist nicht beauftragt und darf erst nach separatem Auftrag beginnen.**

[Scope](../tools/architecture-validation/scopes/WP-024.production.scope.json)

## Ziel

Auf einer frischen Branch vom aktuellen `main` wird nach dem integrierten Level-v2-/Hash-Block (WP-023) der nächste fachliche Produktionsblock ausführbar gemacht: Der deterministische Recovery-Solver `solver-v1` aus WP-022 wird zu `solver-v2` weiterentwickelt. Die vier Metriken `searchNodes`, `deductionSteps`, `maxDeductionDepth` und `requiredGuessDepth` entsprechen exakt [`ADR-031`](../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md). Jede gezählte Root-Deduktion besitzt eine auditierbare Spur aus `ruleCode`, konkreter Konklusion und minimalen Prämissen, aus der sich die Root-Metriken rekonstruieren lassen. Der normale Kampagnengate (`requiredGuessDepth = 0`) steht als ausdrückliche, vom Solver verwendbare Prüfung bereit.

Dieses Work Package erzeugt weder Proofdateien noch Proofschema, Strict-Validation, Pipeline, Generator oder Content. Das ist der spätere, eigene Proof-v1-Block.

Die vorhandene [`WP-016_Solver-v2-Metriken-und-Deduktionsspur.md`](./WP-016_Solver-v2-Metriken-und-Deduktionsspur.md) bleibt fachliche Planungsgrundlage. Sie liegt bereits auf `main` und kann daher gemäß [`ADR-030`](../DECISIONS/ADR-030-wp-scope-trust-anchor.md) nicht selbst der gemeinsame Erstanker mit einem neuen Scope-Manifest sein. WP-024 macht diesen fachlichen Block erstmals ausführbar. Übernommen wird nur, was mit dem heutigen `main`, den integrierten Ergebnissen von WP-021 bis WP-023 und den bindenden ADRs übereinstimmt (siehe „Herkunft / Abgrenzung zu WP-016“).

## Voraussetzungen

1. Die Lesereihenfolge aus [`AGENTS.md`](../AGENTS.md) ist vollständig einzuhalten. Danach sind mindestens [`PROJECT_CONTROL/CURRENT_STATE.md`](../PROJECT_CONTROL/CURRENT_STATE.md), [`PROJECT_CONTROL/WORK_QUEUE.md`](../PROJECT_CONTROL/WORK_QUEUE.md), [`PROJECT_CONTROL/AI_HANDOVER_RULES.md`](../PROJECT_CONTROL/AI_HANDOVER_RULES.md), [`PROJECT_CONTROL/DEFINITION_OF_DONE.md`](../PROJECT_CONTROL/DEFINITION_OF_DONE.md), dieses Work Package, die WP-016-Planungsfassung, [`ARCHITECTURE/SOLVER_ARCHITECTURE.md`](../ARCHITECTURE/SOLVER_ARCHITECTURE.md), [`ARCHITECTURE/PUZZLE_ENGINE.md`](../ARCHITECTURE/PUZZLE_ENGINE.md), [`ARCHITECTURE/MODULE_BOUNDARIES.md`](../ARCHITECTURE/MODULE_BOUNDARIES.md), [`ARCHITECTURE/TEST_STRATEGY.md`](../ARCHITECTURE/TEST_STRATEGY.md), [`ADR-007`](../DECISIONS/ADR-007-solver-und-eindeutigkeitspruefung.md), [`ADR-021`](../DECISIONS/ADR-021-puzzleidentitaet-und-proofartefakte.md), [`ADR-026`](../DECISIONS/ADR-026-validator-evidenz-und-scope-vertrauensanker.md), ADR-030 und ADR-031 zu lesen.
2. WP-022 (Puzzle-Kern mit Recovery-Solver) und WP-023 (Level-v2-/Hash-Foundation) sind abgeschlossen und integriert. Ausgangsbasis dieses Auftrags ist `main` bei `ab117f459689d2b2937d436bb1a763296a65d52a`.
3. Dieses Work Package und sein [Production-Scope-Manifest](../tools/architecture-validation/scopes/WP-024.production.scope.json) werden gemeinsam erstmals im selben ersten Branchcommit eingeführt. Dessen einziger Elterncommit und der `baseCommit` des Manifests sind exakt `ab117f459689d2b2937d436bb1a763296a65d52a`.
4. Das WP-024-Manifest ist nach diesem gemeinsamen Add-Commit byteunveränderlich.
5. Die vorhandene WP-016-Datei ist ausschließlich Planungs-/Vergleichsquelle und wird nicht als Trust Anchor behandelt. Der historische WP-013-Branch dient höchstens als lesbare Negativreferenz; seine Metrikimplementierung wird nicht übernommen.
6. Die offenen Produktblocker `BLOCKER-PROD-001`, `BLOCKER-PROD-002` und `BLOCKER-PROD-003` bleiben fail-closed und werden von diesem Block weder aufgelöst noch umgangen.

## Scope

### Phase A – Definition und Verankerung

1. Dieses Work Package und das eigene Production-Scope-Manifest gemeinsam im ersten Branchcommit anlegen.
2. Danach `CURRENT_STATE.md` und `WORK_QUEUE.md` mechanisch auf WP-024 als aktuellen Produktionsblock nachführen und in `validate.yml` ausschließlich die Production-Scope-Manifestzuordnung auf WP-024 umstellen.
3. Historischen Anker, Elternbasis, Manifestbytes, Scopegrenzen und vollständigen Branchdiff prüfen; Architecture-/Production-Scope-Validatoren samt Self-/Negativtests ausführen.
4. Keine Solver-, Test-, Schema-, Architektur- oder Unity-CI-Implementierung in Phase A.

### Phase B – Implementierung nach separatem Auftrag

1. **Kennung:** `solver-v2` ist die Produktionssolverkennung (`PuzzleSolver.SolverVersion` und jedes Ergebnis). Die Kennung `solver-v1` bleibt als Wert in historischen Daten lesbar, wird aber nicht mehr für neue Ergebnisse verwendet.
2. **Trennung Root und Suche:** Root-Propagation (vor der ersten DFS-Annahme) und DFS-Metriksammlung sind strikt getrennt. Der heutige Solver führt Reduktionen von Root und Zweigen in einer gemeinsamen Liste und zählt den Root im internen Zustandsbudget mit; diese Vermischung darf in keine Metrik eingehen.
3. **Deduktionsspur:** Jede Root-Reduktion liefert einen unveränderlichen Eintrag mit `ruleCode`, konkreter Konklusion und minimalen Prämissen. Identische Reduktionen werden dedupliziert. Prämissen verweisen ausschließlich auf öffentliche Puzzlefakten oder frühere Root-Einträge. Keine Unity-, UI-, PlayerPrefs-, Asset- oder Persistenzdaten. Es werden keine veränderlichen Sammlungen herausgegeben.
4. **Metriken exakt nach ADR-031, Abschnitt 2:**
   - `searchNodes`: Zahl der tatsächlich begonnenen DFS-Wertannahmen bis zum Lösungslimit zwei; Root-Propagation zählt nicht, erfolgreiche und verworfene Zweige zählen.
   - `deductionSteps`: Zahl der deduplizierten, erklärbaren, sicheren Root-Reduktionen; alle Suchzweige sind ausgeschlossen.
   - `maxDeductionDepth`: öffentliche Startfakten Tiefe 0; Reduktion ohne abgeleitete Prämisse Tiefe 1, sonst `1 + max(Tiefe der abgeleiteten Prämissen)`; kein Root-Schritt ergibt 0; Suchannahmen und ihre Folgen sind ausgeschlossen.
   - `requiredGuessDepth`: Zahl gleichzeitig aktiver DFS-Annahmen auf dem deterministischen Pfad zur einzigen gefundenen Lösung; Root-only ergibt 0; vorher untersuchte, verworfene Zweige zählen nicht.
5. **Rekonstruierbarkeit:** `deductionSteps` und `maxDeductionDepth` müssen allein aus der gelieferten Spur berechenbar sein. Eine bloße numerische Tiefenbuchführung genügt nicht.
6. **Kampagnengate:** Eine ausdrückliche, vom Solver verwendbare Prüfung stellt fest, dass `requiredGuessDepth > 0` nicht für den normalen Kampagnenimport freigabefähig ist (ADR-031, Abschnitt 4). Ein nicht eindeutiges oder unbestimmtes Ergebnis ist ebenfalls nie freigabefähig. Der Gate ist eine Solverprüfung, kein Importer und keine Pipeline.
7. **Erhalt des Bestandsverhaltens:** Constraintsemantik, Propagationsreihenfolge, MRV mit `y`-dann-`x`-Tiebreaker, Wertreihenfolge `EMPTY`, `TRACK_NS`, `TRACK_EW`, `TRACK_NE`, `TRACK_ES`, `TRACK_SW`, `TRACK_WN`, Lösungslimit zwei, Klassifikation 0/1/2+ sowie harte Budget- und Cancellation-Behandlung bleiben funktional erhalten. Ein Timeout oder Ressourcenlimit ist `INDETERMINATE`, niemals `UNIQUE`.
8. **Dokumentation:** `SOLVER_ARCHITECTURE.md` (Versionierung, Spur, Metriken, Gate) und `TEST_STRATEGY.md` (nur die neuen expliziten Solver-v2-Referenztests) auf den tatsächlich gelieferten Stand bringen. Keine Vertragsänderung gegenüber ADR-031.
9. **Tests:** vollständige Solver-EditMode-Referenz-, Metamorphose-, Mutations- und Performancetests.
10. **Übergabe:** Ergebnisse, Tests, Einschränkungen und nächsten Schritt in diesem Work Package sowie in `CURRENT_STATE.md` und `WORK_QUEUE.md` persistieren.

## Betroffene Dateien/Module

Die unveränderliche technische Außengrenze ist das [WP-024-Production-Scope-Manifest](../tools/architecture-validation/scopes/WP-024.production.scope.json). Inhaltlich vorgesehen sind ausschließlich:

- `WORK_PACKAGES/WP-024_Solver-v2-Metriken-und-Deduktionsspur.md`
- `tools/architecture-validation/scopes/WP-024.production.scope.json`
- `PROJECT_CONTROL/CURRENT_STATE.md`
- `PROJECT_CONTROL/WORK_QUEUE.md`
- `.github/workflows/validate.yml` ausschließlich zur Umschaltung der Production-Scope-Manifestzuordnung
- `ARCHITECTURE/SOLVER_ARCHITECTURE.md`
- `ARCHITECTURE/TEST_STRATEGY.md` nur für die neuen expliziten Solver-v2-Referenztests
- bestehende Assembly `Assets/StammstreckenPuzzle/Scripts/Puzzle/Solver/` (nur `*.cs` und `*.cs.meta`, flach)
- bestehende Testassembly `Assets/StammstreckenPuzzle/Tests/EditMode/Solver/` (nur `*.cs` und `*.cs.meta`, flach)

Keine neuen Assemblies, keine neuen Abhängigkeiten. `STP.Puzzle.Solver` darf weiterhin ausschließlich von `STP.Puzzle.Domain` abhängen. `asmdef` und `csc.rsp` der Solver- und Solver-Testassembly sowie `unity.yml` sind bewusst nicht im Manifest: Die Solver-Assemblies sind bereits in der EditMode- und Coverage-Evidenz der Unity-CI enthalten. Wird eine dieser Dateien zwingend gebraucht, ist das ein Blocker und erfordert einen neuen Auftrag, keine stille Scope-Erweiterung.

## Ausdrücklich nicht erlaubte Änderungen

- Keine Änderung des WP-024-Production-Scope-Manifests nach dem Trust-Anchor-Commit.
- Keine Änderung des öffentlichen Puzzle-Regelwerks, der sechs Gleisformen, der Domain-Completionsemantik, des Spielerzustands oder der Kampagnenregeln. Keine Änderung der Domain.
- Keine Änderung von Level-v2-Parser, JCS, Hashverträgen, Migration oder `STP.Infrastructure.Content`.
- Keine Änderung von `proof-v1.schema.json`, Proof-Beispielen, Proof-DTOs, Proofserializer oder Proofdateien; keine Erzeugung von Proofdateien. `proofFormatVersion` bleibt 1; die Schemaanpassung auf `minimum: 0` gehört ausdrücklich zum späteren Proof-Block.
- Keine Strict-Validation, `LevelValidationPipeline`, Proofregeneration, `PRF-*`-Prüfung, Release-Lock-Freigabe oder Importer.
- Keine Generator-/Authoringfunktion, Season-1-Level, Zeitkalibrierung oder Contentproduktion.
- Keine UI-, Bootstrap-, Save-, Reward-, Hint-, Daily-, Store-, Analytics- oder Werbelogik. Der Solver liefert keine Hint-Economy-Entscheidung; `BLOCKER-PROD-001` bleibt unberührt.
- Keine Änderung von Tiebreakern, Wertreihenfolge oder Metrikdefinitionen gegenüber ADR-031 und `SOLVER_ARCHITECTURE.md`; ADR-031 ist abschließend. Jeder spätere Eingriff erfordert `solver-v3` und eine neue ADR-Prüfung.
- Keine Manipulation von Kennzahlen, um historische Fixturewerte nachzubilden. Kein Test wird abgeschwächt oder entfernt, um eine Änderung durchzubringen.
- Keine Änderung an `unity.yml`, an `asmdef`/`csc.rsp` oder an ADRs; keine Gateabsenkung.
- Keine neue Architektur- oder Produktentscheidung; Widersprüche werden als Blocker dokumentiert.
- Kein Merge, Cherry-Pick, Rebase oder Copy aus historischen Implementierungsbranches (insbesondere WP-013); keine Übernahme historischer PASS-Aussagen.
- Keine beiläufigen Refactorings außerhalb des Work-Package-Scopes.

## Akzeptanzkriterien

`AK-01` ist in Phase A prüfbar. `AK-02` bis `AK-12` sind Kriterien der Implementierungsphase B.

| ID | Prüfkriterium |
|---|---|
| `AK-01` | WP-024 und eigenes Manifest sind im ersten Branchcommit gemeinsam als neue Dateien verankert; Elterncommit/baseCommit ist exakt `ab117f459689d2b2937d436bb1a763296a65d52a`; der historische WP-Link zeigt auf genau dieses Manifest; das Manifest bleibt byteunverändert und remote nachweisbar. |
| `AK-02` | `PuzzleSolver.SolverVersion` und jedes Ergebnis melden `solver-v2`. Kein Pfad erzeugt noch Ergebnisse unter `solver-v1`. |
| `AK-03` | Jeder Root-Spureintrag besitzt `ruleCode`, konkrete Konklusion und minimale Prämissen. Prämissen sind nur öffentliche Puzzlefakten oder frühere Root-Einträge; Einträge sind dedupliziert und unveränderlich; kein Eintrag enthält Unity-, UI-, PlayerPrefs- oder Assetdaten. |
| `AK-04` | Ein rein deduktiver Referenzfall liefert `searchNodes = 0`, `requiredGuessDepth = 0` und die aus der Spur rekonstruierbaren Root-Metriken. Ohne Root-Schritt ist `maxDeductionDepth = 0`. |
| `AK-05` | Ein Referenzfall mit verworfenen DFS-Zweigen beweist, dass `searchNodes` alle tatsächlich begonnenen Annahmen (erfolgreiche und verworfene, nicht den Root) zählt, während `deductionSteps` und `maxDeductionDepth` ausschließlich aus der Rootspur stammen. |
| `AK-06` | Ein eindeutiger Suchfall beweist, dass `requiredGuessDepth` nur die gleichzeitig aktiven Annahmen auf dem deterministischen Erfolgspfad misst und nicht Zahl oder Tiefe verworfener Zweige. |
| `AK-07` | `deductionSteps` und `maxDeductionDepth` sind in einem unabhängigen Testcode allein aus der Spur nachrechenbar und stimmen mit den vom Solver gemeldeten Werten überein. |
| `AK-08` | Der Kampagnengate weist `requiredGuessDepth > 0` ab. Nicht eindeutige und unbestimmte Ergebnisse sind nie freigabefähig; ein Budget-, Zeit- oder Cancellationabbruch wird nie zu `UNIQUE`. |
| `AK-09` | Klassifikation 0/1/2+, Lösungslimit zwei, MRV-/Tiebreakerdeterminismus, Wertreihenfolge und harte Budgetbehandlung bleiben funktional erhalten. Rotations-/Spiegelmetamorphosen und der kleine unabhängige Exhaustive Enumerator bestätigen die Lösungsklasse. Exakte Metrikgoldens gibt es nur für absichtlich festgelegte Referenzfälle. |
| `AK-10` | Wiederholte Läufe liefern identische Ergebnisse und identische Spuren. Der 10×10-Performancebeleg hält die Budgets aus `TEST_STRATEGY.md`; eine Überschreitung ist ein ausgewiesener Befund, nie ein stiller Erfolg. |
| `AK-11` | `SOLVER_ARCHITECTURE.md` und `TEST_STRATEGY.md` entsprechen dem gelieferten Stand und widersprechen ADR-031 nicht. Der Solver hängt weiter ausschließlich von der Domain ab; kein Proof-, Schema-, Pipeline-, Strict-, Generator- oder Contentartefakt wurde geändert. |
| `AK-12` | Vorgeschriebene Governance-/Scope-/Trust-/Secret-/Diffprüfungen und reale Unity-/CI-Nachweise sind PASS; keine Änderung außerhalb des Manifests, keine Gateabsenkung, keine Coverage-Verschlechterung ohne begründete Ausnahme. |

## Tests

1. Beide kanonischen Architecture-/Production-Scope-Validatorläufe inklusive Self-/Negativtests in der vorgesehenen Ubuntu-CI.
2. Trust-Anchor-/Manifest-/Basis-/Ancestry-/vollständiger Diffnachweis.
3. `git diff --check` gegen den Manifest-Basecommit.
4. Unity-Compile und alle Solver-EditMode-Tests im zugelassenen CI-Runner; bestehende Domain-, Content- und Bootstrap-Regressionen dürfen nicht verschlechtert werden.
5. Referenzfälle für Rootspur, Root/DFS-Trennung, Nullwerte, Suchpfadtiefe gegenüber verworfenen Zweigen, Timeout/Cancellation und deterministische Wiederholung.
6. Mutationstests: Eine absichtlich verfälschte Zählung (Root als Suchknoten, Zweigtiefe in `maxDeductionDepth`, verworfene Zweige in `requiredGuessDepth`, fehlende Deduplizierung) muss mindestens einen Test brechen.
7. Metamorphose (Rotation/Spiegelung samt Endpoints und Randzahlen) und Vergleich mit dem unabhängigen Exhaustive Enumerator für die Lösungsklasse.
8. Performancebeleg gegen die 10×10-Budgets aus `TEST_STRATEGY.md` (p95 unter 250 ms, Maximum unter 2 s im dokumentierten Referenzaufbau).
9. Coverage je Assembly gegen die dokumentierte Baseline; ein Rückgang erfordert eine begründete Ausnahme.
10. Lokaler Harness (falls genutzt) nur als zusätzliche Vorprüfung. Unity-spezifische NUnit- und Nullable-Flow-Unterschiede (Lehre aus WP-023) machen den echten Unity-Lauf maßgeblich.
11. Vor Integration eine unabhängige Abschlussprüfung auf dem eingefrorenen finalen PR-HEAD.

## Risikoklasse

**Hoch.** Solvermetrik, Eindeutigkeit und die spätere Hint- und Contentqualität sind Kernverträge. Eine falsche Trennung von Root und Suche kann Kampagnencontent als deduktiv ausweisen, obwohl er Annahmen erfordert; eine falsche Metrik würde in späteren Proofs und Release-Locks festgeschrieben.

## Definition of Done

Es gilt [`PROJECT_CONTROL/DEFINITION_OF_DONE.md`](../PROJECT_CONTROL/DEFINITION_OF_DONE.md) vollständig. WP-024 gilt erst als abgeschlossen, wenn

- alle anwendbaren Akzeptanzkriterien mit konkretem Test und tatsächlicher Evidenz erfüllt sind;
- keine offene Abweichung zwischen Spur und Metriken besteht;
- keine offenen BLOCKER/HIGH im Scope bestehen und die Abschlussdokumentation persistiert ist;
- eine unabhängige Abschlussprüfung auf dem eingefrorenen finalen PR-HEAD PASS meldet und die Geschäftsführung die Integration freigibt;
- `CURRENT_STATE.md` den erreichten Stand nennt und als nächsten Schritt einen eigenen ausführbaren Proof-v1-Startanker nach ADR-030 vorsieht.

Phase A allein endet mit: **definiert, ADR-030-konform verankert und bereit für einen separaten Implementierungsauftrag**. Sie ist keine technische Fertigstellung von WP-024.

## Rollen und Übergabe

- Geschäftsführung und Projektkoordination geben Implementierung und Integration frei.
- Ein beliebiger geeigneter Implementierungsagent darf nach separatem Auftrag ausschließlich innerhalb dieses Work Packages arbeiten. Er darf keine ADR, Metrikdefinition oder Scopegrenze verändern.
- Die Schutzfunktion ist eine **unabhängige Abschlussprüfung auf dem eingefrorenen finalen Stand vor Integration** (QC- und Rollenregel in `WORK_QUEUE.md`). Sie ist unabhängig vom Implementierungsstand und prüft Metriksemantik und Traceherkunft gegen ADR-031, Teststärke, Scope und CI-Evidenz. Das konkrete ausführende System ist austauschbar und kein Bestandteil der Projektarchitektur. Eine zweite parallele Prüfung ist nur auf ausdrückliche Geschäftsführungsentscheidung nötig; dieses Work Package verlangt sie nicht.
- Jeder Agent persistiert Status, geänderte Dateien, Tests, Einschränkungen/Blocker und nächsten Schritt nach `AI_HANDOVER_RULES.md`.
- Chatverlauf und Modellidentität sind keine Projektquelle.

## Herkunft / Abgrenzung zu WP-016

`WP-016_Solver-v2-Metriken-und-Deduktionsspur.md` bleibt als vorab angelegte Planungsfassung lesbar. Gegen den heutigen Repositorystand wurde sie geprüft; nichts wurde mechanisch übernommen:

| WP-016-Aussage | Befund heute | Behandlung in WP-024 |
|---|---|---|
| Voraussetzung WP-015 abgeschlossen | WP-015 ist nur Planungsfassung; die Level-v2-/Hash-Foundation wurde als WP-023 geliefert und integriert. | Voraussetzung ist WP-022 und WP-023. |
| Branch und Manifest „im gemeinsamen Trust-Anchor-Commit“ | Gilt nach ADR-030 für ein auf `main` bereits vorhandenes Planungs-WP nicht mehr. | WP-024 und Manifest sind der gemeinsame Erstanker. |
| Rollen Kimi, Astra-QC, Sol-QC, zwei getrennte Reviewer | Die Projektregel ist modellneutral: eine unabhängige Abschlussprüfung auf dem eingefrorenen Stand, System austauschbar. | Siehe „Rollen und Übergabe“; keine Modellnamen, keine Doppel-QC. |
| `requiredGuessDepth` als „maximal gleichzeitig aktive“ Annahmen | ADR-031 definiert die Zahl gleichzeitig aktiver Annahmen auf dem deterministischen Pfad zur einzigen Lösung. | ADR-Wortlaut ist maßgeblich. |
| `solver-v1` „bleibt nur lesbar/historisch“ | Der Code trägt `solver-v1` als einzige Konstante. Die Kennung kommt zusätzlich in Level-v1-Daten, Beispielen und Content-Tests vor. | Nur die Solverkonstante und ihre Solver-Tests wechseln. Daten, Beispiele und Content bleiben unberührt (Proof-Block bzw. WP-023). |
| Nächster Schritt WP-017 | WP-017 liegt ebenfalls vorab auf `main` und ist kein Trust Anchor; seine Voraussetzungen nennen noch WP-015/WP-016 sowie Astra-/Sol-Rollen. | Der Proof-v1-Block erhält nach Integration von WP-024 einen eigenen Startanker. Die WP-017-Datei wird dort abgeglichen und hier nicht verändert. |
| Akzeptanzkriterien AK-01 bis AK-07, Tests und Dateiliste | Inhaltlich mit ADR-031 und den Bestandstests vereinbar. | Übernommen, geschärft (Rekonstruierbarkeit, Gate für nicht eindeutige Ergebnisse, Mutations- und Performancebeleg) und um den Trust-Anchor-Nachweis ergänzt. |
| „Keine Änderung des Proofschemas“ | ADR-031 verschiebt `minimum: 0` ausdrücklich auf den Proof-Block. | Unverändert ausgeschlossen. |

## Offene Grenzfälle für Phase B

Diese Punkte sind kein Phase-A-Blocker, aber vor der Implementierung zu beachten:

1. ADR-031 definiert die vier Metriken für den Fall einer einzigen gefundenen Lösung. Für `UNSATISFIABLE`, `MULTIPLE_OR_MORE` und `INDETERMINATE` legt keine Quelle fest, wie `requiredGuessDepth` oder die übrigen Werte auszuweisen sind. Phase B darf dafür keine eigene Produkt- oder Architekturentscheidung treffen. Zulässig ist nur die konservative Lesart aus ADR-031 Abschnitt 4 und `SOLVER_ARCHITECTURE.md`: Solange das Ergebnis nicht `UNIQUE` ist, gibt es keine freigabefähige Kampagnenaussage und kein Pfadmetrikergebnis wird als gültig ausgegeben. Lässt sich das nicht ohne neue Festlegung umsetzen, ist es als Blocker zu dokumentieren.
2. Das heutige Ergebnis enthält keine Metriken. `Reductions` ist ein Recovery-Protokoll, das ausdrücklich „kein Beweis, kein Hint und keine auditierbare solver-v2-Rootspur“ ist, und der interne Zustandszähler dient nur dem Budget. Beides darf nicht unbesehen als Metrik oder Spur weiterverwendet werden.
3. Solange der Proof-Block fehlt, bleiben neue Production-Proofausgaben fail-closed deaktiviert. Ein Root-only-`solver-v2`-Ergebnis ist nicht als schema-valide, importierbar oder releasefähig zu behaupten.

## Phase-A-Nachweis – 2026-10-10

- Verifizierte Ausgangsbasis: `main` `ab117f459689d2b2937d436bb1a763296a65d52a` (WP-023 formal geschlossen). Keine bestehenden WP-024-Refs auf dem Remote.
- Verbindlicher Trust Anchor: `3e7f1ab2e56853294a4f09650d8ec361ab4eca1a`, erster Branchcommit. Exakt dieses Work Package und das eigene Production-Scope-Manifest wurden dort gemeinsam als neue Dateien (`A`) eingeführt; im Elternbaum existiert keine von beiden. Der einzige Elterncommit ist `ab117f45…`, entspricht dem `baseCommit` des Manifests und ist Vorfahre des Branch-HEAD. Der historische WP-Text verlinkt das Manifest ausdrücklich.
- Das Manifest hat SHA-256 `2055ed1a9e8985fb6898ad6f914e7775ff1ae75148118a6e1b8086d20b69e631` und ist am Anker, im Branch-HEAD und im Arbeitsbaum byteidentisch; nach dem Anker gibt es keine Manifest-Änderung.
- Phase-A-Folgediff gegen die Basis beschränkt sich auf `CURRENT_STATE.md`, `WORK_QUEUE.md` und die Manifestzuordnung in `.github/workflows/validate.yml` (eine Zeile). Keine Produktions-, Test-, Schema-, Architektur-, ADR- oder Unity-CI-Änderung; kein Pfad außerhalb des Manifests; `git diff --check` gegen die Basis sauber; Secret-Scan des Diffs ohne Treffer.
- Lokale Validatorläufe (CPython 3.13, die CI nutzt 3.11.13): Architecture-only mit Self-/Negativtests PASS (17 Gruppen); Production-Scope mit WP-024-Manifest und Self-/Negativtests PASS (18 Gruppen). Eigene Negativproben auf Wegwerfbranches (Änderung an `proof-v1.schema.json` außerhalb des Manifests; nachträglich erweitertes Manifest) schlugen wie vorgesehen mit `scope:out-of-scope` bzw. `scope:manifest-mutated-after-anchor` fehl.
- Kanonische GitHub-PR-CI auf Head `ffa615dbe6e920ae5bacd83a2181515cd6eb1a94` (Draft-PR #17): Architecture Validation PR-Run `38088889033` (Job `114321045127`) und Push-Run `38088879224` (Job `114321017889`), **COMPLETED / SUCCESS**. Unity CI war zum Zeitpunkt dieser Eintragung noch nicht abgeschlossen; `project-preflight`, `unity-config`, `unity-environment-linux` und `unity-evidence-guard` waren erfolgreich. Da Phase A keinen Unity-relevanten Inhalt ändert, ist Unity CI kein Kriterium der Phase-A-Abnahme und wird hier nicht als PASS gewertet.
- Ergebnis: **Phase A abgeschlossen. Kein Definitions-, Scope- oder Trust-Blocker. WP-024 ist bereit für einen separaten Implementierungsauftrag.** Kein Merge; PR #17 bleibt Draft.
