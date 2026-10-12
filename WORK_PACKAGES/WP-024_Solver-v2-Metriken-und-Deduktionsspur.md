# WP-024 – Solver-v2-Metriken und Deduktionsspur

## ID

`WP-024`

**Bearbeitungsstatus:** **Phase A (Definition und Verankerung) abgeschlossen. Phase B (Implementierung) ist umgesetzt. Die erste unabhängige Abschluss-QC (Head `6f74ebb…`) und die erneute unabhängige Abschluss-QC (Head `59cf00c…`) endeten jeweils mit FAIL; beide Befundrunden sind innerhalb von WP-024 korrigiert (Abschnitte „QC-Korrektur nach der ersten unabhängigen Abschluss-QC“ und „Zweite QC-Korrektur nach der erneuten unabhängigen Abschluss-QC“). Der Stand ist technisch bereit für eine weitere unabhängige Abschlussprüfung auf dem eingefrorenen finalen PR-HEAD; diese Prüfung und die Integration stehen aus (kein Merge, PR #17 bleibt Draft).**

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

## Phase-B-Nachweis – 2026-10-11

> **Überholt durch den Abschnitt „QC-Korrektur nach der ersten unabhängigen Abschluss-QC“ weiter unten.** Dieser Abschnitt dokumentiert den ersten Phase-B-Stand (PR-HEAD `6f74ebbb6379d5c8a95c4515f664e6d4d2048bc4`). Die unabhängige Abschluss-QC auf diesem Head endete mit **FAIL** (zwei HIGH-Befunde). Testzahlen, Coverage, Mutationsläufe und CI-Läufe dieses Abschnitts gelten **nicht** für den korrigierten Stand und wurden nicht übertragen; die Aussagen zu `AK-03`, `AK-08`, `AK-09` und `AK-12` sind durch die Neuprüfung unten ersetzt.

Ausgangs-HEAD des Auftrags: `aa9349b616f467107e0604f3b0d79b8c7bdd07e8`. Code-Stand dieses Nachweises: Commit `fa64a83` (Docs-Folgecommits ändern keinen Code).

### Gelieferter Umfang

- `PuzzleSolver.SolverVersion` und jedes Ergebnis: `solver-v2`. Root-Propagation wird getrennt von der Suche über den internen `RootDeductionTracer` protokolliert; `searchNodes` und `requiredGuessDepth` kommen aus dem DFS, `deductionSteps` und `maxDeductionDepth` ausschließlich aus der Spur. Zustandsbudget, Prüfreihenfolge, MRV, Wertreihenfolge, Lösungslimit zwei und die Klassifikation bleiben unverändert.
- Neue öffentliche Typen: `DeductionStep`, `DeductionPremise`, `PublicFactKind`, `CandidateValue`, `SolverMetrics`, `CampaignGate`, `CampaignGateOutcome`. `SolverResult` trägt `DeductionTrace` und `Metrics`.
- Metriken und Spur gibt es nur bei `UNIQUE`; bei 0, 2+, `INDETERMINATE` sind sie leer. Ein nicht ableitbarer Spureintrag macht ein eindeutiges Ergebnis zu `INDETERMINATE` (fail-closed).
- Der Kampagnengate (`CampaignGate.Evaluate`) ist fail-closed: freigabefähig ist nur `UNIQUE` mit Metriken und `requiredGuessDepth = 0`.
- Dokumentation: `SOLVER_ARCHITECTURE.md` (Abschnitt 13) und `TEST_STRATEGY.md` (Abschnitt 4.1) auf den gelieferten Stand gebracht.

### Geänderte Dateigruppen

Alle Änderungen liegen innerhalb des Manifests: `Scripts/Puzzle/Solver/` (`PuzzleSolver.cs`, neu `DeductionTrace.cs`, `RootDeductionTracer.cs`, `CampaignGate.cs` samt `.meta`), `Tests/EditMode/Solver/` (`PuzzleSolverTests.cs` nur Kennung `solver-v2`; neu `TraceAudit.cs`, `SolverV2MetricsTests.cs`, `TracerPredicateTests.cs` samt `.meta`), `ARCHITECTURE/SOLVER_ARCHITECTURE.md`, `ARCHITECTURE/TEST_STRATEGY.md`, dieses Work Package, `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md`. Unverändert: Manifest, `.github/workflows/*`, `asmdef`/`csc.rsp`, Domain, Content, Schemas, ADRs.

### Akzeptanzkriterien

| ID | Stand | Nachweis |
|---|---|---|
| `AK-01` | erfüllt | Phase-A-Nachweis; Manifest-SHA-256 `2055ed1a…e631` unverändert gegen den Anker `3e7f1ab2…` und gegen den HEAD. |
| `AK-02` | erfüllt | `SolverVersion_IsSolverV2_ForTheConstantAndEveryResultKind`; `PuzzleSolverTests` auf `solver-v2`. |
| `AK-03` | erfüllt | `TraceTypes_AreImmutable…`, `Tracer_Replays…`, `TraceAudit_RejectsForgedTraces…`, `TracerPredicateTests`, Auditor-Prüfung aller Spuren der Exhaustive-/Seeded-Läufe (Sicherheit, Prämissenreihenfolge, Irredundanz). |
| `AK-04` | erfüllt | `PureDeductive_SingleCell…`, `PureDeductive_TopPair…` (wörtliche Spuren, `searchNodes = 0`, `requiredGuessDepth = 0`), `NoRootStep_GivesMaxDeductionDepthZero…`. |
| `AK-05` | erfüllt | `SearchCases_CountEveryAssumption…` (u. a. 4×4 mit tiefem verworfenem Zweig: 10 Knoten, 20 Root-Schritte, Tiefe 2). |
| `AK-06` | erfüllt | `SearchCases…` (`requiredGuessDepth` 1, 2 und 3 bei 2, 4 bzw. 6 begonnenen Annahmen) und Seeded-4×4. |
| `AK-07` | erfüllt | `TraceAudit.Recompute` rechnet Schritte und Tiefe unabhängig aus den Prämissen nach; in allen Metriktests verglichen. |
| `AK-08` | erfüllt | `CampaignGate_*`, `NonUniqueAndLimitedResults_*`, `Classify_*`. |
| `AK-09` | erfüllt | Bestehende Exhaustive-/Metamorphose-/Determinismus-/Budgettests unverändert grün; Exhaustive 39.664 Definitionen gegen den unabhängigen Oracle (1.800 UNIQUE, davon 1.600 Root-only, 200 mit einer Annahme). |
| `AK-10` | erfüllt | `RepeatedRuns_AreByteIdentical…`; 10×10-Budget in CI: p95 14,5 ms, Maximum 36,0 ms (Grenzen 250 ms / 2 s). |
| `AK-11` | erfüllt | Abschnitt 13 / 4.1; Solver hängt weiter nur von der Domain ab; keine Proof-, Schema-, Pipeline-, Strict-, Generator- oder Contentänderung. |
| `AK-12` | erfüllt, siehe Evidenz | Validatoren, Scope, Unity-CI; Solver-Coverage 99,4 % gegenüber Baseline 98,7 %. |

### Tests und Evidenz

- Solver-EditMode lokal (NUnit 3.14, .NET 8): 43/43. Unity-EditMode in CI auf `fa64a83`: 401/401, davon Solver 43/43. Der lokale Harness ist nur Vorprüfung; ein zusätzlicher NUnit-3.5.0-Kompilierlauf hat den CI-Fehler `Does.Not.Contain(int)` (Commit `2ff6dc0`) reproduziert, der in `9e6b066` behoben wurde.
- Mutationsnachweis (lokal, auf Scratchkopien): 26 verfälschte Solvervarianten, alle von mindestens einem Test erkannt (siehe `TEST_STRATEGY.md` 4.1). Nicht Teil der CI.
- Coverage `STP.Puzzle.Solver` (Unity-OpenCover, Zeilen): Baseline WP-023 98,7 % (375/380); erster Stand von Phase B 90,7 % (747/824), weil Prädikate für `LOOP_PREVENTION`, `LOCAL_DEGREE_REQUIRED` und `CONNECTIVITY_PRESERVATION` in keinem Root-Lauf feuern; nach `TracerPredicateTests` **99,4 % (819/824)**. Die fünf verbleibenden unbedeckten Zeilen liegen in `PuzzleSolver.cs` und sind unverändert die des Baselinelaufs. Keine Ausnahme nötig.
- Performance: `WP024_BUDGET solver=solver-v2 … p95_ms=14.490 max_ms=35.967` (PR-Run `38094975426`); `WP022_BUDGET` p95 14,859 ms, Maximum 39,766 ms.
- CI auf `fa64a83`: Architecture Validation PR-Run `38094975504` und Push-Run `38094972446` SUCCESS. Unity CI Push-Run `38094972458` (Wiederholung 2) **SUCCESS** über alle Jobs: Compile/EditMode 401/401, PlayMode-Smoke, Android-IL2CPP, iOS-Export. Der erste Versuch dieses Laufs und Teile des PR-Runs `38094975426` (PlayMode, Android, iOS) wurden ohne Testergebnis von der Concurrency-Gruppe `unity-personal-seat` verdrängt (Runner-Abbruch nach vier Sekunden ohne Schritte, kein Fehlschlag). Der PR-Run lieferte Compile/EditMode SUCCESS mit denselben 401/401. Der Nachweis auf dem finalen PR-HEAD (Docs-Commit) wird nach dessen Läufen im Abschlussbericht ausgewiesen.
- Lokal: Architecture- und Production-Scope-Validator mit `--self-test` PASS; `git diff --check` sauber; Manifest unverändert.

### Bekannte Einschränkungen

1. Metriken und Spur nur für `UNIQUE` (konservative Lesart von ADR-031 Abschnitt 4, „Offene Grenzfälle“ Nr. 1). Keine eigene Festlegung für 0, 2+ und `INDETERMINATE`; kein Blocker.
2. Bei mehreren irredundanten Prämissenmengen wählt eine feste Löschreihenfolge (jüngster Eintrag zuerst) genau eine. Das ist eine Festlegung innerhalb von ADR-031 und wird erst mit `solver-v3` änderbar.
3. `CONNECTIVITY_PRESERVATION`, `LOOP_PREVENTION` und `LOCAL_DEGREE_REQUIRED` treten in den untersuchten Root-Läufen (rund 19.000) nicht auf; ihre Ableitungsprüfungen sind durch White-Box-Tests gegen die echten Stufen belegt, nicht durch Rootläufe.
4. Kein Proof, keine Schemaänderung, keine Strict-Validation: ein Root-only-`solver-v2`-Ergebnis ist weiterhin nicht importierbar oder releasefähig. `BLOCKER-PROD-001` bis `-003` bleiben unberührt.
5. Es gibt keine unabhängige Abschluss-QC; sie wurde nicht simuliert.

### Nächster Schritt

Unabhängige Abschlussprüfung auf dem eingefrorenen finalen PR-HEAD, danach Geschäftsführungsentscheidung zur Integration. Erst nach Integration von WP-024 erhält der Proof-v1-Block einen eigenen ausführbaren Startanker (ADR-030).

## QC-Korrektur nach der ersten unabhängigen Abschluss-QC – 2026-10-11

> **Teilweise überholt durch den Abschnitt „Zweite QC-Korrektur nach der erneuten unabhängigen Abschluss-QC“ weiter unten.** Die erneute unabhängige Abschluss-QC auf `59cf00c6a526d3434c178b44f5ac4b09cacadcc5` (dieser Stand plus die Zweitprüfung) endete mit **FAIL**. Überholt sind ausdrücklich: der Vertrag „ein Schritt deklariert genau die Fakten, die seine Ableitung liest“ und die Aussage, eine zu weit gefasste `GRID_SIZE`-Deklaration sei „konservativ“ und zulässig (Befund 1 unten, „Vertrag der Spur“ und „Grenzen der Aussage“); die Beschränkung der vor der letzten Budgetprüfung gebauten Ergebnisarbeit auf das `UNIQUE`-Ergebnis (Befund 2 unten, „Geltungsbereich“ und „Änderung“); Beobachtung 3 der Zweitprüfung; Einschränkung 2 und der nächste Schritt am Ende dieses Abschnitts. Die übrigen Aussagen und die Historie bleiben gültig.

Status: Korrektur umgesetzt innerhalb von WP-024, alle lokalen Prüfungen auf dem neuen Stand wiederholt; die **erneute unabhängige Abschluss-QC steht aus** (keine Selbst-QC). PR #17 bleibt Draft und wird nicht gemergt. Das Production-Scope-Manifest ist unverändert (SHA-256 `2055ed1a9e8985fb6898ad6f914e7775ff1ae75148118a6e1b8086d20b69e631`), keine Datei außerhalb des Manifests wurde berührt, kein Blocker.

Anlass: Die unabhängige Abschluss-QC auf `6f74ebbb6379d5c8a95c4515f664e6d4d2048bc4` ergab FAIL mit zwei integrationsblockierenden HIGH-Befunden. Die Prüfmeldung ist keine Spezifikation; beide Befunde wurden zuerst als externe Behauptungen behandelt, unabhängig reproduziert und gegen Repository, ADR-031 und `SOLVER_ARCHITECTURE.md` Abschnitt 13 bewertet.

### QC-Befund 1 – Deduktionsspur / Prämissen (`RootDeductionTracer`)

| Frage | Ergebnis |
|---|---|
| Gemeldet | Veröffentlichte Root-Deduktionen (etwa `BORDER_PORT_FORBIDDEN`) folgen nicht vollständig aus ihren veröffentlichten Prämissen: Ein Gegenbeispiel ändert eine nicht deklarierte Endpoint-Konfiguration, alle deklarierten Prämissen bleiben, die Konklusion gilt nicht mehr. |
| Reproduziert | **Ja, und breiter als gemeldet.** (a) Unabhängiger Enumerator (`SmallPathOracle`, ohne Solver): 3×3, A = W/0, B = E/2, Zeilen (3,1,1), Spalten (1,1,3). Der Lieferstand veröffentlichte `BORDER_PORT_FORBIDDEN` in (1,0) mit `GRID_SIZE` als einziger Prämisse. In 3×3 mit A = N/1, B = E/2 (gleiche Rastergröße) ist `NS` in (1,0) die einzige Lösung, die Konklusion also falsch. (b) Mechanisch über 1.806 `UNIQUE`-Puzzles (Fixtures und alle eindeutigen 2×2-, 2×3-, 3×2-, 3×3-Puzzles): Wurde jeder nicht deklarierte Endpoint auf jede andere Position verschoben (nur gültige Puzzles), ließ sich `BORDER_PORT_FORBIDDEN` in 6.842 von 9.311 Schritten und `ENDPOINT_ENTRY_REQUIRED` in 2.567 von 3.579 Schritten nicht mehr ableiten. Bei `ENDPOINT_ENTRY_REQUIRED` fehlte der jeweils **andere** Endpoint (`<grid A>` ohne `B` in `TopPair`@0,0: `WN` fällt wegen des W-Ports, den `B` belegen könnte). (c) `LINE_ALL_REMAINING_OCCUPIED`: Ohne `GRID_SIZE` ändert eine größere Linienlänge die Zahl der noch belegbaren Zellen (2.955 von 5.562 Schritten nach Rastervergrößerung nicht mehr ableitbar). (d) `LOCAL_DEGREE_REQUIRED`: Auf einem konstruierten Zustand (die Regel feuert in keinem Root-Lauf) hing die Entfernung von `NS` am Randpunkt (1,0) an den Endpoints, die nicht deklariert waren. Nicht betroffen waren `NEIGHBOR_PORT_REQUIRED`, `LINE_ZERO`, `LINE_TARGET_REACHED` sowie nach der Prüfung der Prädikate `LOOP_PREVENTION` und `CONNECTIVITY_PRESERVATION`. |
| Ursache | `SOLVER_ARCHITECTURE.md` 13.3 und `RootDeductionTracer.FactsOf` legten die öffentlichen Fakten **statisch je Regelcode** fest. Ob ein Außenport ein Endpointport ist, hängt aber von beiden Endpoints ab (`PuzzleDefinition.IsEndpointPort`), und die Linienlänge hängt an der Rastergröße. Die Ableitungsprüfung las diese Fakten, die Deklaration nannte sie nicht. |
| Betroffener Repository-Vertrag | ADR-031 („konkrete Konklusion und die minimalen referenzierten Prämissen“, Spur als Audit-Grundlage), `SOLVER_ARCHITECTURE.md` 13.3, `AK-03` (auditierbare, sichere, ableitbare Spur), Auditierbarkeit und Prämissenvertrag. Metrikwerte waren nicht falsch, die Spur trug sie aber nicht vollständig. |
| Vertrag der Spur (präzisiert, keine neue Entscheidung) | Ein veröffentlichter Schritt gilt für **jedes** gültige Puzzle, das in seinen deklarierten Fakten übereinstimmt. Er deklariert deshalb genau die öffentlichen Fakten, die seine Ableitung liest: Ein ungelesener Fakt kann die Konklusion nicht beeinflussen, ein gelesener wird immer deklariert. Die früheren Einträge bleiben irredundant (unverändert); die Fakten werden auf genau dieser Menge bestimmt. |
| Änderung | `RootDeductionTracer`: Die Ableitungsprüfung gibt jetzt die gelesenen Fakten zurück (`Derive`, `Holds` ist darauf aufgesetzt). Der Tracer speichert sie je Eintrag, `Build` veröffentlicht sie, der Auditor leitet sie aus den deklarierten Prämissen selbst ab und weist jede Abweichung ab (fehlend oder überzählig). Neu ist die Endpointprüfung mit Vermerk, welcher Endpoint sie entscheidet (passender Endpoint allein; negativ beide). `FactsOf` mit statischer Regeltabelle entfällt. `SOLVER_ARCHITECTURE.md` 13.3 beschreibt die Fakten je Regel. Veröffentlichte Spuren ändern sich: Randschritte und `ENDPOINT_ENTRY_REQUIRED` tragen `ENDPOINT_A` und `ENDPOINT_B`, `LINE_ALL_REMAINING_OCCUPIED` zusätzlich `GRID_SIZE`; Metriken, Tiefen, Prämissen früherer Einträge, Klassifikation und Suche bleiben unverändert. |
| Regressionsevidenz | Neue Datei `TracePremiseSufficiencyTests` (6 Tests): Perturbationsorakel mit der echten Solverstufe (724 Referenzpuzzles, 9.289 Schritte, 96.702 gültige Puzzles, die in den deklarierten Fakten übereinstimmen und sonst frei sind); Gegenprobe, dass jeder deklarierte Endpoint-, Summen- und `LINE_ALL`-Rasterfakt notwendig ist; wörtlicher Gegenfall des Prüfberichts mit dem unabhängigen Enumerator; konstruierte Zustände für `LOCAL_DEGREE_REQUIRED`, `LOOP_PREVENTION`, `CONNECTIVITY_PRESERVATION` und `EMPTY` allein an einer Endpoint-Zelle; Abgleich der verfolgten Endpointprüfung mit `PuzzleDefinition.IsEndpointPort` für alle Zellen und Seiten; Auditor-Mutationen (Fakt fehlt, Fakt überzählig, Deklaration des ersten Stands). Die Goldens `PureDeductive_SingleCell_…` und `PureDeductive_TopPair_…` schrieben den Fehler fest (`<grid>` bzw. `<grid A>` für Randschritte) und sind mit Begründung im Test auf `<grid A B>` korrigiert; ihr Inhalt wurde von Hand neu abgeleitet. Das ist eine Korrektur eines falschen Erwartungswerts, keine Abschwächung: Auditor-, Irredundanz- und Metriktests blieben unverändert. Alle neuen Tests scheitern gegen den vorherigen Tracer (zum Beispiel „SingleCell step#1 BORDER_PORT_FORBIDDEN declares [grid] but is not derived for 2x2 A=N/1 B=E/0“). |

**Überholt (zweite QC-Korrektur, Befund 2):** Die folgende Einschätzung war keine zulässige Einschränkung, sondern eine Unterschreitung des Minimalitätsvertrags von ADR-031 Abschnitt 3. Grenzen der Aussage: `GRID_SIZE` wird überall deklariert, wo die Regel die Rastergeometrie liest. Das ist für Randseiten N und W sowie für Nachbarn, die eine Prämisse selbst benennt, nicht zwingend nötig (Orakel: nicht widerlegbar), aber sicher; es ändert die Spur dieser Schritte gegenüber dem ersten Stand nicht. Die Prädikate für `LOOP_PREVENTION` und `CONNECTIVITY_PRESERVATION` sind nur auf konstruierten Zuständen geprüft; dort widersprechen alle verschobenen Endpoints den Prämissen, die Endpointfakten sind daher durch das Lesen der Regel begründet und nicht durch ein Gegenbeispiel als notwendig belegt.

### QC-Befund 2 – harte Budgetbehandlung (`PuzzleSolver.Solve`)

| Frage | Ergebnis |
|---|---|
| Gemeldet | Ein Solverlauf kann das Zeitbudget nach der letzten Budgetprüfung überschreiten und trotzdem `UNIQUE` und kampagnenfähig zurückgeben (QC: 32×32 mit sehr knappem Restbudget). |
| Reproduziert | **Ja.** Im Lieferstand lagen Deduplizierung, `RootDeductionTracer.Build`, `SolverMetrics.FromTrace` und die Konstruktion des `UNIQUE`-Ergebnisses **nach** der letzten `CheckBudget()`-Prüfung. 32×32-Serpentine (494 Pfadzellen, 1.639 Spurschritte, 17 Uhrablesungen): rund 48,8 ms bis zur letzten Prüfung, rund 1,3 ms Arbeit danach. Mit einem Budget dazwischen lieferte der Solver in 125 von 200 Läufen `UNIQUE`; in 3 davon wurde das Ergebnis nach bereits erschöpftem Budget zurückgegeben (Überschreitung im Median 0,86 ms, maximal 47,69 ms), und `CampaignGate` bewertete ein solches Ergebnis als `ELIGIBLE`. Andere Abbruchwege teilen die Ursache nicht: `maxStates` wird vor der Arbeit jedes Knotens geprüft und setzt `INDETERMINATE`, Cancellation, Zeit und rückläufige Uhr laufen alle durch dieselbe letzte Prüfung. |
| Betroffener Repository-Vertrag | ADR-031 („Timeout, Ressourcenlimit und Cancellation sind `INDETERMINATE`, nie `UNIQUE`“), `AK-08` (fail-closed Gate), `AK-09` (Budget-/Limitverhalten). |
| Geltungsbereich des harten Limits | **Überholt (zweite QC-Korrektur, Befund 1): die Änderung unten setzte das nur für das `UNIQUE`-Ergebnis um.** Budgetierte Arbeit ist alles bis zur Rückgabe eines Ergebnisses: Suche, Vollständigkeitsprüfung, Deduplizierung des Recovery-Protokolls, Spuraufbau, Metriken und Ergebnisobjekt. `INDETERMINATE` ist Pflicht, wenn die letzte Prüfung nach dieser Arbeit Zeit, Cancellation oder Uhr verletzt. Das Budget bleibt kooperativ: ein Abschnitt zwischen zwei Prüfungen wird nicht unterbrochen; ein verspäteter Lauf liefert aber nie `UNIQUE`. Nach der letzten Prüfung folgt nur die Rückgabe des fertigen Objekts. |
| Änderung | `PuzzleSolver.Solve`: Das `UNIQUE`-Ergebnis (Spur, Metriken, Ergebnisobjekt) wird vor der einen letzten `CheckBudget()`-Prüfung gebaut; danach entscheidet nur noch, ob zurückgegeben oder zu `INDETERMINATE` herabgestuft wird. Die Zahl und Reihenfolge der Uhrablesungen ist unverändert (die bestehenden Tests mit 8/11/51 Ablesungen plus einer letzten laufen unverändert). `SOLVER_ARCHITECTURE.md` 13.2 beschreibt das harte Budget. |
| Regressionsevidenz | `PuzzleSolverTests`: `HardBudget_UniqueNeedsAProbeAfterTheWholeTraceBuild_AtEveryInterruptionKind` (Serpentine 10×10 und 32×32, Timeout/Cancellation/rückläufige Uhr nur an der letzten Prüfung; kein `UNIQUE`, kein `ELIGIBLE`, keine Metriken, keine weitere Ablesung; Budgetgrenze exklusiv; `maxStates` 1 und 0) und `HardBudget_NoBudgetedWorkRunsBetweenTheLastProbeAndTheReturn` (Wanduhr, Minimum über 25 Läufe von Rückgabe minus letzte Prüfung unter 0,5 % der Laufzeit; gemessen 0,001 % nach der Korrektur, 2,4 % im Lieferstand). Der zweite Test scheitert gegen den vorherigen Solver, die Skripttests bestätigen die Semantik der letzten Prüfung. Mutationen „letzte Prüfung vor dem Spuraufbau“, „letzte Prüfung entfernt“, „`UNIQUE` trotz unterbrochener letzter Prüfung“ werden erkannt. |

### Unabhängige Zweitprüfung der Korrektur – 2026-10-12

Status: Verifikation durch einen weiteren Agenten auf einen erneuten Auftrag zum QC-FAIL von `6f74ebb…`; **keine Abschluss-QC** und kein Urteil über die Integrationsfähigkeit. Der PR-HEAD war dabei bereits `e5eb9a5c3815d9c28bdf10ba248f2edabfd1275b` (die Korrektur oben). Beide Befunde wurden deshalb nicht übernommen, sondern erneut als externe Behauptungen gegen `6f74ebb…` und `e5eb9a5…` mit eigenen Prüfern reproduziert, die weder Solverlogik noch Repositorytests verwenden. Die Prüfer liegen bewusst nicht im Repository (außerhalb des Manifests); die Methode ist so beschrieben, dass sie neu herstellbar ist. **Ergebnis: Beide Befunde sind auf `6f74ebb…` reproduzierbar und auf `e5eb9a5…` nicht mehr. Produktionscode, Tests, Manifest und Workflows blieben unverändert; dieser Abschnitt ist die einzige Änderung der Zweitprüfung.**

| Befund | Unabhängiger Prüfer | `6f74ebb…` | `e5eb9a5…` |
|---|---|---|---|
| 1 Prämissen | Eigener Pfadenumerator (alle einfachen A→B-Pfade, ohne Domain- und Solvercode) mit **Gegenmodellsuche** je veröffentlichtem Schritt: Gibt es ein gültiges Puzzle, das in genau den deklarierten Fakten übereinstimmt (Rastergröße nur, wenn deklariert), dessen Lösung alle Prämisseneinträge (entfernte Werte) erfüllt und die Konklusion verletzt? Jedes Gegenmodell wurde mit `PuzzleEvaluator.Evaluate(...).Completed` bestätigt. Je `UNIQUE`-Ergebnis zusätzlich: Spurstruktur, kein Entfernen des Lösungswerts, `RemainingValues` gegen die Zellhistorie, Tiefe, Metriken aus der Spur, `requiredGuessDepth = 0` genau bei Root-only. | 3×3 vollständig (1.124 `UNIQUE`, 22.680 Schritte): **12.460 Schritte verletzt**, nämlich `BORDER_PORT_FORBIDDEN` 6.752 von 6.752, `ENDPOINT_ENTRY_REQUIRED` 1.472 von 2.240, `LINE_ALL_REMAINING_OCCUPIED` 4.236 von 4.236; alle übrigen Regeln 0. Mehr als gemeldet, deckungsgleich mit der Reproduktion oben. | 3×3, 2×2, 2×3, 3×2, 2×4, 4×2, 3×4 und 4×3 vollständig, 4×4 jedes dritte `UNIQUE`-Puzzle: zusammen 18.767 Puzzles, 559.721 Schritte, **0 verletzt, 0 unentschieden, keine Strukturabweichung**. Solver und Orakel stimmen in allen 38.272 Definitionen dieser Raster überein (`UNIQUE` genau bei einer Lösung, kein `INDETERMINATE` durch einen fälschlich fehlerhaften Tracer). Dazu 2.560 Root-Läufe beliebiger, auch widersprüchlicher Puzzles in 4×4 und 5×5 (Rückgriff auf die echte Root-Propagation): 0 verletzt, der Tracer war nie fehlerhaft. |
| 2 Hartes Budget | Über die öffentliche Uhr-Injektion von `SolverLimits`: (a) arbeitsproportionale Uhr (vom Solverthread allozierte Bytes, deterministisch) mit Budget zwischen letzter Prüfung und Rückgabe; (b) Ablauf und Cancellation an jeder Prüfposition; (c) Zustandslimit-Sweep; (d) echte Uhr in µs bei 32×32. Invariante: Ist das Budget bei der Rückgabe verbraucht, muss das Ergebnis `INDETERMINATE` ohne Metriken und Spur sein. | Nach der letzten Prüfung noch 23.608 Byte (17 %, 4×4) bis 1.159.928 Byte (2,7 %, 32×32) Arbeit; in allen sechs Puzzles (Zeilenlinie und Rahmen, je 4×4, 10×10, 32×32) kam bei verbrauchtem Budget `UNIQUE` mit Metriken und `CampaignGate` `ELIGIBLE` (17 Verstöße der arbeitsproportionalen Uhr). Echte Uhr 32×32: `UNIQUE` trotz verbrauchtem Budget in 2 von 40 bzw. 1 von 40 Läufen. (b) und (c): 0 Verstöße, der Fehler liegt nach der letzten Prüfung und nicht davor. | 0 Byte nach der letzten Prüfung in allen sechs Puzzles; bei verbrauchtem Budget immer `INDETERMINATE`, sonst `UNIQUE`; (a) bis (d) 0 Verstöße, echte Uhr 0 von 80 Läufen. |

- Die Tests von `e5eb9a5…` gegen den Produktionscode von `6f74ebb…` (lokal): 9 von 52 scheitern, darunter `HardBudget_NoBudgetedWorkRunsBetweenTheLastProbeAndTheReturn` und die Prämissentests. Die Regressionstests erkennen also beide Befunde.
- 17 weitere gezielte Mutationen des korrigierten Codes (Fakten je Regel zu viel oder zu wenig, statische Regeltabelle wieder eingeführt, letzte Prüfung vor Deduplizierung, letzte Prüfung entfernt, Spuraufbau nach der letzten Prüfung, Grenzwert `>` statt `>=`, Zustandslimit entfernt): alle von mindestens einem Test erkannt.

Beobachtungen ohne Änderungsbedarf:

1. „Spuraufbau nach der letzten Prüfung“ bei unveränderter Ablesezahl wird **nur** von `HardBudget_NoBudgetedWorkRunsBetweenTheLastProbeAndTheReturn` erkannt; die Skripttests (`HardBudget_UniqueNeedsAProbeAfterTheWholeTraceBuild_…`, `FinalClassification_…`) legen die Semantik der letzten Prüfung fest, unterscheiden diese Reihenfolge aber nicht. Der Wanduhrtest hat in der Unity-CI auf `e5eb9a5…` großen Abstand zur Schwelle (0,0007 ms nach der letzten Prüfung bei 634 ms Laufzeit, Schwelle 0,5 %; im Lieferstand 2,4 %). Ein deterministischer Ersatz ist über die öffentliche API nicht möglich, weil interne Arbeit nicht beobachtbar ist.
2. `LOCAL_DEGREE_REQUIRED` und `LOOP_PREVENTION` kommen in keiner der geprüften Spuren und Root-Läufe vor; `CONNECTIVITY_PRESERVATION` kommt in Root-Läufen widersprüchlicher Puzzles vor (47 Läufe in 4×4 und 5×5), dort ohne Verletzung. Das bestätigt die Einschränkung oben (Prädikate auf konstruierten Zuständen belegt) und widerlegt sie nicht. Eine Erreichbarkeitsaussage ist daraus nicht abgeleitet.
3. **Überholt (zweite QC-Korrektur, Befund 1):** Nicht-`UNIQUE`-Ergebnisse bauen ihr kleines Ergebnisobjekt (Kopie von Pfad und Recovery-Protokoll) nach der letzten Prüfung. Das Urteil hängt nicht davon ab, ein solches Ergebnis ist nie kampagnenfähig; bewusst nicht geändert. *Diese Bewertung war falsch: Damit konnte eine definitive Klassifikation trotz erschöpften Budgets zurückgegeben werden; fehlende Kampagnenfähigkeit ist keine Ausnahme vom Budgetvertrag.*
4. Grenze der Zweitprüfung: Das Orakel entscheidet vollständig bis 4×4; 5×5 wurde stichprobenartig geprüft, bei 6×6 und größer erreicht die Gegenmodellsuche ihre Suchgrenze (unentschieden, nicht als Bestätigung gewertet).

### Geänderte Dateien (alle im Manifest)

`Scripts/Puzzle/Solver/PuzzleSolver.cs` (Reihenfolge am Ende von `Solve`), `Scripts/Puzzle/Solver/RootDeductionTracer.cs` (gelesene Fakten), `Tests/EditMode/Solver/PuzzleSolverTests.cs` (2 neue Tests, 3 Fälle), `Tests/EditMode/Solver/SolverV2MetricsTests.cs` (Goldens `PureDeductive_*` korrigiert, kein Test entfernt oder abgeschwächt), neu `Tests/EditMode/Solver/TracePremiseSufficiencyTests.cs` samt `.meta`, `ARCHITECTURE/SOLVER_ARCHITECTURE.md` (13.2, 13.3), `ARCHITECTURE/TEST_STRATEGY.md` (4.1), dieses Work Package, `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md`. Unverändert: Manifest, `.github/workflows/*`, `asmdef`/`csc.rsp`, Domain, Content, Schemas, ADRs, `DeductionTrace.cs`, `CampaignGate.cs`.

### Akzeptanzkriterien nach der Korrektur

`AK-01`, `AK-02`, `AK-04` bis `AK-07`, `AK-10`, `AK-11` bleiben mit den oben genannten Tests erfüllt (die Goldens aus `AK-04` wurden wie beschrieben korrigiert, `searchNodes`, Tiefen und Metriken sind unverändert). Neu bewertet:

| ID | Stand | Nachweis |
|---|---|---|
| `AK-03` | erfüllt | Auditor leitet Fakten und Prämissen aus der Spur selbst ab; Perturbationsorakel und Gegenfall des Prüfberichts (`TracePremiseSufficiencyTests`); Exhaustive-Lauf auditiert weiter alle 1.800 Spuren. |
| `AK-08` | erfüllt | `CampaignGate_*`, `NonUniqueAndLimitedResults_*`, `Classify_*` und die `HardBudget_*`-Tests: ein erst nach der letzten Prüfung erschöpftes Budget liefert nie `ELIGIBLE`. |
| `AK-09` | erfüllt | Bestehende Budget-/Limit-/Exhaustive-/Determinismustests unverändert grün (Exhaustive 39.664 Definitionen, 1.800 `UNIQUE`, 1.600 Root-only, 200 mit einer Annahme) plus die `HardBudget_*`-Tests. |
| `AK-12` | siehe Evidenz | Validatoren, Scope, Unity-CI auf dem neuen Head (nicht aus dem ersten Stand übertragen). |

### Tests und Evidenz des korrigierten Stands

- Solver-EditMode lokal (NUnit 3.14, .NET 8, Harness außerhalb des Repositorys): **52/52** (vorher 43: +3 Fälle `HardBudget_*` in 2 Tests, +6 `TracePremiseSufficiencyTests`). Das ist Vorprüfung; verbindlich ist die Unity-CI.
- Mutationsnachweis auf dem korrigierten Stand (lokal, Scratchkopien): 24 Varianten, alle von mindestens einem Test erkannt: 15 zur geänderten Logik (Budgetreihenfolge, gelesene Fakten, Auditor) und 9 zu den in „Tests“ Nr. 6 vorgeschriebenen Zähl-, Spur- und Gatemutationen (Liste in `TEST_STRATEGY.md` 4.1). Der Lauf mit 26 Varianten des ersten Stands wurde nicht wiederholt und wird nicht als Nachweis des neuen Stands geführt.
- Neue Wiederholung der Befund-1-Reproduktion auf dem korrigierten Tracer: Verschiebung jedes nicht deklarierten Endpoints in 1.806 `UNIQUE`-Puzzles lässt keinen Schritt mehr unableitbar (0 von 9.311 `BORDER_PORT_FORBIDDEN`, 0 von 3.579 `ENDPOINT_ENTRY_REQUIRED`, 0 bei allen übrigen Regeln).
- Lokale Validatoren, Diffprüfungen und CI-Läufe des neuen Heads: siehe Abschlussbericht dieses Auftrags und die Check-Runs von PR #17; CI-Nachweise gelten immer nur für den ausgewiesenen Head.

### Bekannte Einschränkungen und nächster Schritt

1. Das Zeitbudget ist kooperativ (siehe oben); es gibt keine Preemption. `UNIQUE` nach erschöpftem Budget ist ausgeschlossen, ein Überschreiten innerhalb eines Abschnitts zwischen zwei Prüfungen nicht.
2. **Überholt (zweite QC-Korrektur, Befund 2):** Die Faktennotwendigkeit ist für `GRID_SIZE` nur teilweise belegt (siehe oben); die Deklaration ist sicher, in Einzelfällen aber weiter gefasst als nötig.
3. Die übrigen Einschränkungen des ersten Phase-B-Stands gelten unverändert (Metriken nur bei `UNIQUE`, feste Löschreihenfolge der Prämissen, kein Proof, keine Schemaänderung).
4. **Überholt:** Nächster Schritt: erneute **unabhängige** Abschluss-QC auf dem eingefrorenen finalen PR-HEAD, danach Geschäftsführungsentscheidung zur Integration. Keine Selbst-QC, kein Merge. (Diese QC fand auf `59cf00c…` statt und endete mit FAIL; der aktuelle nächste Schritt steht im Abschnitt der zweiten QC-Korrektur.)

## Zweite QC-Korrektur nach der erneuten unabhängigen Abschluss-QC – 2026-10-12

Status: Korrektur innerhalb von WP-024 umgesetzt, alle lokalen Prüfungen auf dem neuen Stand wiederholt; eine **weitere unabhängige Abschluss-QC steht aus** (keine Selbst-QC). PR #17 bleibt Draft und wird nicht gemergt. Das Production-Scope-Manifest ist unverändert (SHA-256 `2055ed1a9e8985fb6898ad6f914e7775ff1ae75148118a6e1b8086d20b69e631`), keine Datei außerhalb des Manifests wurde berührt, kein Blocker.

Anlass: Die erneute unabhängige Abschluss-QC auf `59cf00c6a526d3434c178b44f5ac4b09cacadcc5` (erste Korrektur `e5eb9a5…` plus Doku der Zweitprüfung) ergab **FAIL** mit zwei Befunden: (1) HIGH, Budget bei nicht eindeutigen Ergebnissen; (2) MEDIUM, integrationsblockierend wegen `AK-03` und `AK-11`, entbehrliche öffentliche Prämissen. Die Prüfmeldung ist keine Spezifikation. Beide Befunde wurden als externe Behauptungen behandelt, auf `59cf00c…` reproduziert und gegen ADR-031, `SOLVER_ARCHITECTURE.md` und dieses Work Package bewertet. Die korrigierte Fassung des Vertrags steht in `SOLVER_ARCHITECTURE.md` 13.2 und 13.3.

### Zweite QC, Befund 1 – Budget bei nicht eindeutigen Ergebnissen (`PuzzleSolver.Solve`)

| Frage | Ergebnis |
|---|---|
| Gemeldet | Die erste Korrektur schützt nur die Ergebnisbildung von `UNIQUE`. Bei `UNSATISFIABLE` und `MULTIPLE_OR_MORE` wird nach der letzten `CheckBudget()`-Prüfung noch ein `SolverResult` samt Kopien von Pfad und Reduktionen gebaut; diese definitiven Klassifikationen können daher trotz zwischenzeitlich überschrittenem Budget zurückkommen. Reproduktionen: 2×2 N0/W0 Zeilen (2,0) Spalten (1,1); 3×3 N0/N2 alle Zahlen 3; dieselbe 3×3-Mehrdeutigkeit in 32×32 mit übrigen Zahlen 0. |
| Reproduziert | **Ja, und breiter als gemeldet.** Auf `59cf00c…` mit eigener Reproduktion außerhalb des Repositorys: (a) arbeitsproportionale Uhr (vom Solverthread allozierte Bytes, .NET 8), Budget so gesetzt, dass genau die letzte Prüfung noch im Budget liegt: Nach der letzten Prüfung entstanden noch 160 B (2×2 unlösbar), 472 B (3×3 mehrdeutig) und 9.560 B (32×32 mehrdeutig, Kopie von Pfad und 1.167 Reduktionen); zurück kamen `UNSATISFIABLE` bzw. `MULTIPLE_OR_MORE`, obwohl das Budget bei der Rückgabe verbraucht war. Kontrolle `UNIQUE`-Serpentine 32×32: 0 B nach der letzten Prüfung, korrekt `INDETERMINATE`. (b) Stoppuhr, Minimum über 200 Läufe von Rückgabe minus letzte Prüfung: 3,75 µs (32×32 mehrdeutig) gegenüber 0,29 µs (`UNIQUE`). (c) Dieselbe Ursache betraf auch `INDETERMINATE`: Das fail-closed Ergebnis wurde ebenfalls erst nach der letzten Prüfung gebaut (nicht gemeldet, mitkorrigiert; das Urteil war dort richtig, die Rückgabe aber nicht die einzige Arbeit nach der Prüfung). |
| Betroffener Repository-Vertrag | ADR-031 (Timeout, Ressourcenlimit und Cancellation sind `INDETERMINATE`), Phase B Nr. 7 dieses Work Packages (harte Budgetbehandlung bleibt erhalten), `AK-08`, `AK-09`, `SOLVER_ARCHITECTURE.md` 13.2 („nach der letzten Prüfung folgt nur noch die Rückgabe des fertigen Objekts“). Die frühere Fassung von 13.2 nannte als budgetierte Ergebnisarbeit nur das `UNIQUE`-Ergebnis, und die Zweitprüfung hielt die Konstruktion der übrigen Ergebnisse nach der Prüfung ausdrücklich für unbedenklich, weil sie nie kampagnenfähig sind. Keine Quelle sieht diese Ausnahme vor: eine definitive Klassifikation nach erschöpftem Budget verletzt den Budgetvertrag unabhängig von der Kampagnenfähigkeit. |
| Änderung | `PuzzleSolver.Solve` baut das Ergebnis der ermittelten Klassifikation (jede Art) **und** das fail-closed `INDETERMINATE`-Ergebnis vollständig vor der einen letzten Budgetprüfung; danach wird nur noch eines der beiden fertigen Objekte zurückgegeben. Klassifikation, Suche, Zahl und Reihenfolge der Uhrablesungen sind unverändert (die Skripttests mit 8/11/51 Ablesungen plus einer letzten laufen unverändert). `SOLVER_ARCHITECTURE.md` 13.2 nennt die budgetierte Arbeit für jede Ergebnisart. |
| Regressionsevidenz | Neuer Test `HardBudget_NoResultIsBuiltAfterTheLastProbe_ForEveryResultKind` (Wanduhr, weil die öffentliche Schnittstelle Arbeit nach der letzten Prüfung nicht anders sichtbar macht: für jede Ergebnisart mit großem Ergebnis, nämlich `UNSATISFIABLE` (Serpentine 32×32 mit einer verschobenen Spaltenzahl, 1.693 Reduktionen), `MULTIPLE_OR_MORE` (die gemeldete 3×3-Mehrdeutigkeit in 32×32 sowie eine Mehrdeutigkeit mit einem ersten Pfad von mehreren hundert Zellen), `INDETERMINATE` (Zustandslimit nach der ersten Lösung) und `UNIQUE` (Serpentine 32×32), muss das Minimum über 25 Läufe von Rückgabe minus letzte Prüfung, vermindert um denselben Wert eines winzigen Ergebnisses (2×2 unlösbar, gleiche Rückgabekosten), unter einem Viertel dessen liegen, was das Kopieren von Pfad und Protokoll in ein Ergebnisobjekt im selben Lauf kostet. Lokal (.NET 8): korrigierter Stand 0,19 bis 0,38 µs über dem Vergleichswert bei 3,5 bis 5,4 µs Kopierzeit; Stand `59cf00c…` 3,8 bis 5,6 µs; eine Mutante, die das Ergebnis vor der Prüfung baut und danach ein zweites Mal, 1,6 bis 2,2 µs). Er scheitert gegen den Produktionscode von `59cf00c…` für `UNSATISFIABLE`, beide `MULTIPLE_OR_MORE`-Fälle und `INDETERMINATE` und besteht auf dem neuen Stand. Erhalt der ersten Korrektur: `HardBudget_UniqueNeedsAProbeAfterTheWholeTraceBuild_AtEveryInterruptionKind`, `HardBudget_NoBudgetedWorkRunsBetweenTheLastProbeAndTheReturn` und `FinalClassification_RechecksTimeoutCancellationAndMonotonicity` (0, 1, 2 Lösungen; Timeout, Cancellation, rückläufige Uhr nur an der letzten Prüfung) laufen unverändert. Mutationen siehe unten. Eine deterministische Messung über allozierte Bytes ist in Unity-Mono nicht verfügbar (die API liefert dort 0); sie wurde nur für die Reproduktion genutzt. |

### Zweite QC, Befund 2 – entbehrliche öffentliche Prämissen (`RootDeductionTracer`)

| Frage | Ergebnis |
|---|---|
| Gemeldet | 2×2, A = N/0, B = W/0, Zeilen und Spalten (1,0): Schritt #0 lässt an (0,0) nur `WN`; Schritt #4 `NEIGHBOR_PORT_REQUIRED` entfernt `SW` an (1,0) und veröffentlicht `GRID_SIZE` und #0 als Prämissen. #0 allein trägt die Konklusion (`WN` hat keinen Ostanschluss, `SW` rechts daneben braucht genau diesen), die Rastergröße ist unerheblich. Verlangt wird keine global kürzeste Herleitung. |
| Reproduziert | **Ja, und breiter als gemeldet.** Ursache: Die erste Korrektur deklarierte jeden Fakt, den die Ableitungsprüfung *liest*, nicht jeden, den die Konklusion *braucht*. Unabhängige Gegenmodellsuche außerhalb des Repositorys (echte Solverstufe auf den Prämissen, gültige Puzzles bis eine Spalte und Zeile größer, alle Endpointlagen, lösbare Summen aus dem Pfadenumerator) über 807 `UNIQUE`-Puzzles mit 10.961 Schritten: `GRID_SIZE` ohne Gegenmodell in **2.675 von 2.675** `NEIGHBOR_PORT_REQUIRED`-Schritten, **1.185 von 3.310** `BORDER_PORT_FORBIDDEN`-Schritten (Seiten N und W sind in jedem Raster außen) und **478 von 1.588** `ENDPOINT_ENTRY_REQUIRED`-Schritten (Endpoint auf N oder W); alle übrigen deklarierten Fakten und alle referenzierten Einträge hatten Gegenmodelle. Semantische Bestätigung des gemeldeten Falls: In allen Lösungen aller Puzzles von 2..4 × 2..4 Zellen (beliebige Endpoints und Summen, 45.028 Lösungen) steht neben `WN` an (0,0) nie `SW` an (1,0). Dieselbe Ursache ohne Vorkommen im Korpus: `LOOP_PREVENTION` deklarierte `GRID_SIZE`, `CONNECTIVITY_PRESERVATION` statisch alle drei Fakten, `LOCAL_DEGREE_REQUIRED` die Fakten aller Ports statt eines ungestützten. Außerdem kannte das Modell die Rastergrenzen 2..32 nicht (Spalte 31 ist immer am Ostrand, eine 32er-Linie kann nicht länger werden). |
| Bewertung gegen die Quellen | ADR-031 Abschnitt 3 verlangt „die minimalen referenzierten Prämissen“, `AK-03` minimale Prämissen, `AK-11` eine Dokumentation, die ADR-031 nicht widerspricht. Die Aussage der ersten Korrektur, eine zu weit gefasste `GRID_SIZE`-Deklaration sei „konservativ“ und „sicher“ (Abschnitt „QC-Korrektur…“, „Grenzen der Aussage“ und Einschränkung 2; `SOLVER_ARCHITECTURE.md` 13.3 alter Fassung), war keine zulässige Einschränkung, sondern eine Unterschreitung dieser Quellen. Minimal heißt hier wie bei den Einträgen **irredundant**: Keine einzelne deklarierte Prämisse kann wegfallen, ohne dass ein gültiges Puzzle die Herleitung widerlegt. Eine global kürzeste Prämissenmenge wird weiterhin nicht verlangt und nicht eingeführt. |
| Änderung | `RootDeductionTracer`: Die öffentlichen Fakten werden wie die Einträge minimiert. Ausgehend von den Fakten, die die Regel überhaupt berücksichtigen kann, wird in fester Reihenfolge `GRID_SIZE`, `ENDPOINT_B`, `ENDPOINT_A`, Linienfakt jeder Fakt entfernt, ohne den der Schritt für jedes übereinstimmende gültige Puzzle hinreichend bleibt. Die Hinreichend-Prüfung ist je Regel exakt (Tabelle in `SOLVER_ARCHITECTURE.md` 13.3): lokale Lagen der Außenseiten und Endpoints für Rand-, Endpoint- und Gradregel unter Beachtung der Rastergrenzen 2..32; keine Fakten für Nachbar- und Schleifenregel; Linienfakt (und bei `LINE_ALL_REMAINING_OCCUPIED` `GRID_SIZE`, außer die Linie hat Länge 32) für Linienregeln; Komponenten aller Rastergrößen bis +1 und aller Endpointlagen für die Konnektivität. Der Auditor prüft jetzt: ableitbar, Fakten kanonisch, nur von der Regel berücksichtigte Fakten, zusammen hinreichend, jeder einzeln notwendig, Linienregel nennt die Linie der eigenen Zelle, Einträge (wahlweise) irredundant unter den deklarierten Fakten. Veröffentlichte Spuren ändern sich nur in den Fakten (weniger oder gleich viele); Einträge, Tiefen, Metriken, Klassifikation und Suche bleiben unverändert (Vergleich über 2.382 Puzzles mit 49.368 Spurschritten, nämlich alle eindeutigen 2×2- bis 3×3-Puzzles, die Fixtures und 600 zufällige 4×4-Puzzles, zwischen `59cf00c…` und dem neuen Stand: Klassifikation, Metriken, Regeln, Zellen, Werte, Einträge und Tiefen identisch; die Fakten sind in 28.134 Schritten gleich und in 21.234 Schritten um `GRID_SIZE` kleiner (14.120 Nachbar-, 5.602 Rand- und 1.512 Endpointschritte), in keinem Schritt größer oder anders). |
| Regressionsevidenz | `TracePremiseSufficiencyTests` (umgebaut, kein Prüfziel abgeschwächt): Hinreichend-Orakel unverändert, jetzt gegen die kleineren Faktenmengen (724 Puzzles, 9.289 Schritte, 106.758 übereinstimmende Puzzles, davon 5.956 leer erfüllt); neu ein Notwendigkeitsorakel für **jede** deklarierte Prämisse, Fakten und Einträge (jedes dritte Referenzpuzzle: Rand 939/939 ohne A, 939/939 ohne B, 615/615 ohne Raster; Endpoint 435/435 ohne A, 433/433 ohne B, 334/334 ohne Raster; `LINE_ALL_REMAINING_OCCUPIED` 528/528 ohne Linie, 528/528 ohne Raster, 16/16 ohne Eintrag; `LINE_TARGET_REACHED` 229/229 ohne Linie, 254/254 ohne Eintrag; `LINE_ZERO` 175/175 ohne Linie; Nachbar 1.684/1.684 ohne Eintrag, kein Nachbarschritt mit Fakt); der gemeldete Fall mit #0 als einziger Prämisse, semantisch über den Pfadenumerator und operativ über die echte Stufe in allen übereinstimmenden Puzzles bis 4×4; der Gegenfall der ersten QC jetzt mit `ENDPOINT_A` und `ENDPOINT_B` ohne `GRID_SIZE` und ohne Gegenbeispiel in 2..4 × 3..4; die Rastergrenze 32 (Spalte 31, 32er-Zeile) gegenüber 31; die seltenen Regeln auf konstruierten Zuständen und zwei echten Konnektivitätsschritten aus Root-Läufen; Abgleich der lokalen Randprüfung mit der echten Stufe 0 für jede Zelle, jeden Wert und jedes Endpointpaar; Auditor gegen fehlende, entbehrliche, doppelte und umgeordnete Fakten und gegen die Deklarationen beider früherer Stände. Die Goldens `PureDeductive_SingleCell_…` und `PureDeductive_TopPair_…` schrieben die entbehrlichen Fakten fest und sind mit von Hand neu hergeleiteter Begründung korrigiert (Korrektur falscher Erwartungswerte, keine Abschwächung). Gegen den Tracer von `59cf00c…` scheitern 11 von 55 Tests (einschließlich des neuen Budgettests) der neuen bzw. geänderten Tests. |

Weitere Prüfung derselben Ursachen auf gleichartige Pfade: Die Hinreichend-Suche lief zusätzlich exhaustiv (alle Rastergrößen bis +1 höchstens 5, alle Endpointpaare, alle lösbaren Summen bei Linienregeln) über alle 10.961 Schritte des Korpus: 3.365.021 übereinstimmende Puzzles, kein Fehler. Für die im `UNIQUE`-Korpus nie feuernden Regeln wurden die Root-Läufe von 36.017 beliebigen, auch widersprüchlichen Definitionen (3×3 bis 5×5) ausgewertet: 635 Konnektivitätsschritte (133 ohne Fakt, 492 mit `GRID_SIZE`, 10 mit `GRID_SIZE` und `ENDPOINT_A`), jeder hinreichend in allen übereinstimmenden Puzzles bis 6×6 und jeder deklarierte Fakt und Eintrag notwendig. `LOOP_PREVENTION` trat dort nicht auf; `LOCAL_DEGREE_REQUIRED` kann im Solver nicht feuern (die Stufe läuft erst am Fixpunkt der Stufen 0 bis 2, an dem jeder verbliebene Port gestützt ist) und ist auf konstruierten Zuständen geprüft. Die Prüfer dieser Absätze liegen nicht im Repository (außerhalb des Manifests); die Methode ist beschrieben und neu herstellbar.

### Geänderte Dateien (zweite Korrektur, alle im Manifest)

`Scripts/Puzzle/Solver/PuzzleSolver.cs` (Ergebnisbildung vor der letzten Prüfung für jede Ergebnisart), `Scripts/Puzzle/Solver/RootDeductionTracer.cs` (minimale öffentliche Fakten, Auditor), `Tests/EditMode/Solver/PuzzleSolverTests.cs` (1 neuer Test), `Tests/EditMode/Solver/TracePremiseSufficiencyTests.cs` (Notwendigkeits-, Grenz-, Randprädikat- und Seltene-Regeln-Tests, ersetzt den Test der gelesenen Endpointfakten), `Tests/EditMode/Solver/SolverV2MetricsTests.cs` (Goldens, verfälschte Spuren), `Tests/EditMode/Solver/TracerPredicateTests.cs` (Linienfakt einer fremden Linie), `ARCHITECTURE/SOLVER_ARCHITECTURE.md` (13.2, 13.3, 13.6), `ARCHITECTURE/TEST_STRATEGY.md` (4.1), dieses Work Package, `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md`. Unverändert: Manifest, `.github/workflows/*`, `asmdef`/`csc.rsp`, Domain, Content, Schemas, ADRs, `DeductionTrace.cs`, `CampaignGate.cs`.

### Akzeptanzkriterien nach der zweiten Korrektur

`AK-01`, `AK-02`, `AK-04` bis `AK-07` und `AK-10` bleiben mit ihren Tests erfüllt (Metriken, Tiefen, Suchwerte und Spurlängen sind unverändert; nur die Faktenlisten der Goldens aus `AK-04` sind korrigiert). Neu bewertet:

| ID | Stand | Nachweis |
|---|---|---|
| `AK-03` | erfüllt | Prämissen hinreichend und minimal (irredundant) für Fakten und Einträge: Hinreichend- und Notwendigkeitsorakel, gemeldeter Fall, Auditor; Exhaustive-Lauf auditiert weiter alle 1.800 Spuren. |
| `AK-08` | erfüllt | Kein Budget-, Zeit- oder Cancellationabbruch wird zu einer definitiven Klassifikation; `CampaignGate_*`, `NonUniqueAndLimitedResults_*`, `Classify_*`, alle `HardBudget_*`-Tests. |
| `AK-09` | erfüllt | Harte Budgetbehandlung für jede Ergebnisart; Exhaustive 39.664 Definitionen (1.800 `UNIQUE`, 1.600 Root-only, 200 mit einer Annahme), Metamorphosen, Determinismus, Budgettests unverändert grün. |
| `AK-11` | erfüllt | `SOLVER_ARCHITECTURE.md` 13.2/13.3 und `TEST_STRATEGY.md` 4.1 beschreiben den gelieferten Stand ohne Widerspruch zu ADR-031; die „konservative“ Faktenaussage ist entfernt und hier als überholt gekennzeichnet. Solver hängt weiter nur von der Domain ab. |
| `AK-12` | siehe Evidenz | Validatoren, Scope, Unity-CI auf dem neuen Head (nicht übertragen). |

Die übrigen als „konservativ“ bezeichneten Punkte wurden gegen die Quellen geprüft: Metriken und Spur nur bei `UNIQUE` ist die in „Offene Grenzfälle“ Nr. 1 dieses Work Packages ausdrücklich als einzige zulässig bezeichnete Lesart von ADR-031 Abschnitt 4 und keine Abweichung; das kooperative Zeitbudget (keine Unterbrechung innerhalb eines Abschnitts, aber nie eine definitive Klassifikation nach Ablauf) widerspricht keiner Quelle.

### Tests und Evidenz der zweiten Korrektur

- Solver-EditMode lokal (NUnit 3.14, .NET 8, Harness außerhalb des Repositorys): **55/55** (vorher 52: +1 `HardBudget_NoResultIsBuiltAfterTheLastProbe_ForEveryResultKind`; in `TracePremiseSufficiencyTests` +3 neue Tests `SecondQcFinding2_…`, `GridSizeBound_…`, `BorderPredicate_AgreesWithTheRealStageZero_…`, wobei `TrackedEndpointPort_…` entfällt, weil die geprüfte Methode (gelesene Endpointfakten) nicht mehr existiert; der Ersatz prüft die neue lokale Randprüfung gegen die echte Stufe 0. Umbenannt und verschärft: `DeclaredEndpointLineAndSizeFacts_AreNecessary_…` zu `EveryDeclaredPremise_IsNecessary_…` (jetzt alle Fakten einschließlich `GRID_SIZE` und alle Einträge), `CraftedStates_…` zu `RareRules_…` (mehr Fälle, strenge Notwendigkeit), `Audit_RejectsAnyPublicFactSetThatIsNotExactlyTheFactsTheDerivationReads` zu `Audit_RejectsAnyPublicFactSetThatIsInsufficientOrRedundant`). Laufzeit lokal 33 s. Zusätzlich gegen NUnit 3.5.0 (Unity-Version) kompiliert. Das ist Vorprüfung; verbindlich ist die Unity-CI.
- Gegenprobe: Die neuen bzw. geänderten Tests gegen den Produktionscode von `59cf00c…` (lokal): 11 von 55 scheitern, darunter der neue Budgettest und alle Prämissentests. Beide Befunde werden also erkannt.
- Mutationsnachweis auf dem korrigierten Stand (lokal, Scratchkopien, nicht Teil der CI): 38 Varianten. 33 werden von mindestens einem Test erkannt: die neun in „Tests“ Nr. 6 vorgeschriebenen bzw. bisherigen Zähl-, Spur- und Gatemutationen (Root als Suchknoten, Branchreduktionen in der Root-Spur, verworfene Zweige in `requiredGuessDepth`, fehlende Deduplizierung, keine Eintragsminimierung, Tiefe als Summe, Gate lässt Tiefe 1 zu, Version `solver-v1`, Spurfehler bleibt `UNIQUE`), fünf Budgetvarianten (nicht eindeutiges Ergebnis bzw. `INDETERMINATE` nach der Prüfung gebaut, letzte Prüfung entfernt, Unterbrechung an der letzten Prüfung ignoriert, letzte Prüfung vor dem Ergebnisbau) und 19 Fakten-/Auditorvarianten (unter anderem `GRID_SIZE` wieder für Nachbarschritte, keine Faktenminimierung, Rastergrenze 32 ignoriert, Endpoint nie bzw. nur anderswo, `LINE_ALL` mit und ohne `GRID_SIZE`, Löschreihenfolge, Konnektivität mit nur einer Rastergröße, ohne Pflichtzellen, mit einem Port für zwei Endpoints, mit stets erreichtem B, Gradregel ohne Stütze außerhalb, Auditor ohne Notwendigkeits-, Hinreichend-, Linien- oder Eintragsprüfung, Linienregel ohne Zahl). Fünf Varianten überleben und sind nachweislich gleichwertig zum Original (gleiches Ergebnis für jede Eingabe): `GRID_SIZE` als Kandidat der Nachbarregel (wird stets wieder entfernt), Untergrenze 2 der Rastergröße und Index eines deklarierten Endpoints bei der Ausdehnung (die zusätzlich zugelassenen Lagen fügen nur Außenseiten hinzu, die jeden Wert ebenso verwerfen, bzw. Größen, in denen der Endpoint gar nicht liegt), zwei Endpoints auf demselben Port (ergibt dieselbe Portmenge wie ein Endpoint dort und einer anderswo), explizite Leerregel der Endpoint-Zelle (folgt bereits aus der Portregel).
- Unabhängige Prüfung außerhalb des Repositorys: siehe die beiden Befundtabellen und den Absatz zu gleichartigen Pfaden.
- Lokale Validatoren, Diffprüfungen und CI-Läufe des neuen Heads: im Abschlussbericht dieses Auftrags und in den Check-Runs von PR #17; CI-Nachweise gelten nur für den ausgewiesenen Head.

### Bekannte Einschränkungen und nächster Schritt

1. Das Zeitbudget ist kooperativ: Ein Abschnitt zwischen zwei Prüfungen wird nicht unterbrochen; nach Ablauf gibt es nie eine definitive Klassifikation.
2. Der Nachweis „nichts nach der letzten Prüfung“ für große Ergebnisse ist eine Wanduhrmessung (die öffentliche Schnittstelle macht die Arbeit nach der Prüfung nicht anders sichtbar). Sie vergleicht mit der im selben Lauf gemessenen Kopierzeit und einem kleinen Vergleichsergebnis; lokal liegt der korrigierte Stand um das 2,4- bis 7-fache unter der Schwelle, der Stand `59cf00c…` um das Vier- bis Sechsfache darüber. Wie sich das Verhältnis in der Unity-CI (Mono, Coverage) einstellt, zeigt erst deren Lauf; die Messwerte werden dort ausgegeben (`WP024_HARD_BUDGET_KIND`). Die Skripttests legen die Semantik der letzten Prüfung deterministisch fest.
3. Minimal heißt irredundant, nicht global kürzest. Bei mehreren irredundanten Mengen entscheidet die feste Löschreihenfolge (Einträge jüngster zuerst; Fakten `GRID_SIZE`, `ENDPOINT_B`, `ENDPOINT_A`). Das ist eine Festlegung innerhalb von ADR-031 und erst mit `solver-v3` änderbar.
4. Die Notwendigkeit eines Fakts wird im Test durch Gegenmodelle in Rastern bis eine Spalte und Zeile größer gezeigt; die exakte Prüfung im Tracer deckt alle Größen 2..32 ab (größere Raster als +1 verhalten sich für die Regeln gleich, siehe `SOLVER_ARCHITECTURE.md` 13.3).
5. Metriken und Spur nur bei `UNIQUE`, kein Proof, keine Schemaänderung, keine Strict-Validation; `BLOCKER-PROD-001` bis `-003` unberührt.
6. Nächster Schritt: weitere **unabhängige** Abschluss-QC auf dem eingefrorenen finalen PR-HEAD, danach Geschäftsführungsentscheidung zur Integration. Keine Selbst-QC, kein Merge.
