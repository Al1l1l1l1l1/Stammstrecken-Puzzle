# WP-010 – Unity-Testassembly-Standort-Klarstellung

## ID

`WP-010`

**Bearbeitungsstatus:** In Bearbeitung auf Branch `docs/wp-010-tests-standort`.

## Ziel

Der unabhängig durch GPT-5.6 Sol High bestätigte Architektur-Dokumentationsfehler B-01 ist in `ARCHITECTURE/MODULE_BOUNDARIES.md` behoben: Die dokumentierte Repository-Struktur und die Testassembly-Tabelle weisen künftig die tatsächliche physische Unity-Testverzeichnisstruktur unter `Assets/StammstreckenPuzzle/Tests/` aus und ordnen alle bestehenden Testassemblies eindeutig diesen Pfaden zu. Ein gewöhnlicher repositorywurzeliger `Tests/`-Ordner außerhalb von `Assets/` oder einem eingebundenen Unity-Package wird nicht als normale Unity-Testassembly importiert; die bisherige Darstellung eines wurzeligen `Tests/`-Ordners war eine Dokumentationsungenauigkeit. Die logischen Assemblynamen, der Modulgraph, seine Referenzregeln und die Teststrategie bleiben unverändert.

Feststehender B-01-Befund (Sol High): B-01 ist technisch real. Verbindliche Sol-High-Entscheidung (Lösung A): Tests physisch unter `Assets/StammstreckenPuzzle/Tests/`; kein lokales Zusatzpackage erforderlich; kein neuer ADR erforderlich; die bisherigen logischen Assemblynamen bleiben unverändert; es handelt sich um eine minimale Klarstellung der bestehenden Architecture v1.0, die mit ihr vollständig kompatibel ist.

## Voraussetzungen

Vollständige Lesereihenfolge aus `AGENTS.md`, Architecture v1.0, `PROJECT_CONTROL/DEFINITION_OF_DONE.md`, `PROJECT_CONTROL/AI_HANDOVER_RULES.md`, `PROJECT_CONTROL/WORK_PACKAGE_RULES.md`, `PROJECT_CONTROL/WORK_QUEUE.md`, `ARCHITECTURE/MODULE_BOUNDARIES.md`, `ARCHITECTURE/TECH_STACK.md`, `ARCHITECTURE/TEST_STRATEGY.md`, `ARCHITECTURE/BUILD_AND_RELEASE.md`, die ADR-001, ADR-013, ADR-018, ADR-026 und ADR-030 sowie `WORK_PACKAGES/WP-011_PR-Scope-Checkout-Korrektur.md` als Referenz für Trust Anchor und Documentation Scope.

Ausgangspunkt ist der `main`-Merge-Commit `dca18b8fe228b6e852a0603b6cfeafc1795589bc` (WP-011-Merge). Der Branch `docs/wp-010-tests-standort` basiert exakt darauf. WP-008 läuft separat auf `feat/wp-008-unity-scaffold` und wird nicht verändert, nicht gemergt und nicht gerebaselt; der WP-008-Trust-Anchor und das WP-008-Manifest bleiben unverändert. WP-009 bleibt für den fachlichen Puzzle-Stack reserviert und wird nicht begonnen.

Begründung des Scope-Modus: WP-010 ändert ausschließlich eine Architekturdokumentationsdatei, die CI-Workflowdatei und Governance-Statusdateien ohne Produktartefakte; derselbe Documentation-Scope wurde bereits mit WP-007 und WP-011 verwendet. Ein neuer Scope-Typ wird nicht erfunden.

WP-010 und [`tools/architecture-validation/scopes/WP-010.documentation.scope.json`](../tools/architecture-validation/scopes/WP-010.documentation.scope.json) werden vor der fachlichen Änderung gemeinsam im Trust-Anchor-Commit eingeführt (ADR-030). Der `baseCommit` des Manifests ist der Elterncommit dieses gemeinsamen Ankers, also `dca18b8fe228b6e852a0603b6cfeafc1795589bc`. Das Manifest ist danach byteunverändert zu belassen.

## Scope

Erlaubt sind ausschließlich diese Änderungen:

1. `WORK_PACKAGES/WP-010_Unity-Testassembly-Standort-Klarstellung.md` (neu): Auftrag, B-01-Befund, Sol-High-Entscheidung, Trust-Anchor-Verweis, Status, Akzeptanzkriterien und Abschlussnachweise.
2. `tools/architecture-validation/scopes/WP-010.documentation.scope.json` (neu): vorab verankerte enge Allowlist; nach dem Trust-Anchor-Commit byteunverändert.
3. `ARCHITECTURE/MODULE_BOUNDARIES.md`: ausschließlich Abschnitt 2 (tatsächliche physische Unity-Testverzeichnisstruktur), Abschnitt 4 (eindeutige Zuordnung aller bestehenden Testassemblies zu diesen Pfaden, einschließlich `STP.Tests.Bootstrap.PlayMode`) sowie erforderliche unmittelbar damit zusammenhängende Pfadreferenzen innerhalb derselben Datei (Bootstrap-Smoke, QA-Szene).
4. `.github/workflows/validate.yml`: `STP_SCOPE_MANIFEST` auf das WP-010-Manifest setzen. Der durch WP-011 eingeführte PR-Head-Checkout-Mechanismus bleibt unverändert: Architecture-only-Lauf auf dem synthetischen Merge-Stand, Scope-Lauf auf dem tatsächlichen PR-Head, Push- und Main-Push-Verhalten unverändert.
5. `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md`: ausschließlich mechanische WP-010-Statusnachführung einschließlich der Dokumentation, dass nach WP-010 der bestehende WP-008-Branch fortgesetzt wird und WP-009 der nächste fachliche Puzzle-Stack bleibt.

Eine Änderung an `tools/architecture-validation/validate.py` oder `tools/architecture-validation/README.md` ist nicht vorgesehen; analog WP-011 verlangen die bestehenden Regeln keine mechanische Inventarnachführung des Validators.

## Betroffene Dateien/Module

| Pfad oder Modul | Zulässige Änderung |
|---|---|
| `WORK_PACKAGES/WP-010_Unity-Testassembly-Standort-Klarstellung.md` | Neu: Auftrag, B-01-Befund, Sol-High-Entscheidung, Trust-Anchor-Verweis, Status, Akzeptanzkriterien, Tests und Abschlussnachweise. |
| `tools/architecture-validation/scopes/WP-010.documentation.scope.json` | Neu: vorab verankerte enge Allowlist; nach dem Trust-Anchor-Commit byteunverändert. |
| `ARCHITECTURE/MODULE_BOUNDARIES.md` | B-01-Klarstellung: physische Testverzeichnisstruktur in Abschnitt 2, Assembly-zu-Pfad-Zuordnung in Abschnitt 4 inklusive Bootstrap-Assembly, unmittelbar zusammenhängende Pfadreferenzen innerhalb derselben Datei. |
| `.github/workflows/validate.yml` | Minimaländerung: `STP_SCOPE_MANIFEST` auf WP-010-Manifest; WP-011-PR-Head-Checkout unverändert. |
| `PROJECT_CONTROL/CURRENT_STATE.md` | Ausschließlich WP-010-Status und nächster Schritt. |
| `PROJECT_CONTROL/WORK_QUEUE.md` | Ausschließlich WP-010-Status und Integrationsreihenfolge. |

## Ausdrücklich nicht erlaubte Änderungen

Nicht erlaubt sind Produktionscode, Unity-Implementierung oder Unity-Scaffold, Produktentscheidungen, neue ADRs oder Änderungen an ADR-Entscheidungskörpern (insbesondere ADR-001, ADR-013, ADR-018, ADR-026 und ADR-030), Änderungen an anderen Architekturdateien als `ARCHITECTURE/MODULE_BOUNDARIES.md` (insbesondere `TECH_STACK.md`, `TEST_STRATEGY.md` und `BUILD_AND_RELEASE.md`), Konzeptdateien unter `Stammstrecken_Puzzle_Konzept_00-15/`, jede semantische Änderung an `tools/architecture-validation/validate.py`, jede Änderung an `tools/architecture-validation/README.md`, Änderungen an bestehenden Scope-Manifesten, die Aufnahme künstlicher Änderungen in das Repository, Merge nach `main`, Pull-Request-Erstellung oder -Merge, Force Push, das Löschen bestehender Branches, Änderungen an den WP-008- und WP-009-Zuordnungen sowie Änderungen am WP-008-Trust-Anchor und am WP-008-Manifest.

Der bestehende WP-011-PR-Checkout-Mechanismus bleibt inklusive seiner Governance unverändert erhalten. Die Semantik des Modulgraphen, seiner Referenzregeln und der Teststrategie bleibt vollständig bestehen. Erscheint irgendeine Änderung außerhalb des oben genannten Scopes notwendig – insbesondere eine neue Architekturentscheidung –, ist WP-010 zu stoppen und der Befund zu berichten.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| `AK-01` | Trust-Anchor: WP-010 und Manifest wurden im selben historischen Add-Commit eingeführt, der historische WP-Blob verweist exakt auf das Manifest, der Manifestblob ist bytegleich und `baseCommit` ist der Elterncommit des Ankers (`dca18b8fe228b6e852a0603b6cfeafc1795589bc`). |
| `AK-02` | Abschnitt 2 von `ARCHITECTURE/MODULE_BOUNDARIES.md` zeigt die tatsächliche physische Unity-Testverzeichnisstruktur unter `Assets/StammstreckenPuzzle/Tests/`; ein repositorywurzeliger `Tests/`-Ordner ist darin nicht mehr dokumentiert. |
| `AK-03` | Abschnitt 4 ordnet alle neun Testassemblies eindeutig ihren physischen Pfaden zu, einschließlich `STP.Tests.Bootstrap.PlayMode` → `Assets/StammstreckenPuzzle/Tests/PlayMode/Bootstrap/`; `Fixtures/` und `Golden/` sind als reine Testdatenordner ohne `.asmdef` gekennzeichnet. |
| `AK-04` | Der Bootstrap-Composition-Smoke ist als `Assets/StammstreckenPuzzle/Tests/PlayMode/Bootstrap/BootstrapCompositionSmoke.cs` mit dem vollständigen Testtyp `STP.Tests.Bootstrap.PlayMode.BootstrapCompositionSmoke` und der unveränderten QA-Szene `Assets/StammstreckenPuzzle/Scenes/QA/BootstrapComposition.unity` referenziert. |
| `AK-05` | Kein neuer ADR; keine Änderung an ADR-Entscheidungskörpern; `TECH_STACK.md`, `TEST_STRATEGY.md` und `BUILD_AND_RELEASE.md` sind unverändert; die Semantik von Modulgraph, Referenzregeln und Teststrategie ist vollständig bewahrt. |
| `AK-06` | `STP_SCOPE_MANIFEST` in `.github/workflows/validate.yml` zeigt auf `tools/architecture-validation/scopes/WP-010.documentation.scope.json`; der WP-011-PR-Head-Checkout ist unverändert vorhanden (Architecture-only-Lauf auf synthetischem Merge-Stand, Scope-Lauf auf tatsächlichem PR-Head, Push- und Main-Push-Verhalten unverändert). |
| `AK-07` | `git diff --check` besteht; der vollständige Diff gegen den `baseCommit` enthält ausschließlich die im Manifest erlaubten Dateien. |
| `AK-08` | Architecture-only-Lauf und WP-010-Scope-Lauf mit `--self-test` bestehen lokal; der verbindliche GitHub-Actions-Nachweis des Checks `Architecture Validation / validate` wird geprüft, soweit Zugriff möglich ist, andernfalls dem Owner als ausstehende Prüfung gemeldet. |
| `AK-09` | Push ausschließlich auf `docs/wp-010-tests-standort`; kein Merge nach `main`, kein PR, kein Force Push und kein Checkout/Merge/Rebase von WP-008; die Fortsetzung von WP-008 und die Reservierung von WP-009 sind dokumentiert. |

## Tests

Vor Abschluss werden mindestens ausgeführt und commitbezogen dokumentiert:

1. Architecture-only-Validator mit `--self-test` aus der gepinnten Lock-Umgebung;
2. Documentation-Scope-Validator mit WP-010-Manifest und `--self-test`;
3. Trust-Anchor-Nachweis gemäß ADR-030 für WP-010 und Manifest;
4. `git diff --check` und vollständige Delta-Prüfung gegen `dca18b8fe228b6e852a0603b6cfeafc1795589bc`;
5. Manuelle Gegenprüfung der MODULE_BOUNDARIES-Klarstellung gegen die feststehende B-01-Entscheidung (alle elf physischen Pfade, neun `.asmdef`-Namenszuordnungen, Bootstrap-Smoke-Pfad, QA-Szene);
6. YAML-Plausibilitätsprüfung der geänderten Workflowdatei inklusive unverändertem PR-Head-Checkout;
7. Remote-Commit-Verifikation nach jedem Push.

Unity Compile/EditMode/PlayMode, IL2CPP, physische Geräte-, SDK- und Storetests bleiben mangels Produktionsprojekt **REQUIRED_LATER/NOT_EXECUTED**.

## Risikoklasse

**Niedrig.** Das Work Package verändert keine Architektur- oder Produktsemantik, sondern korrigiert ausschließlich eine dokumentierte physische Pfaldarstellung im Einklang mit der verbindlich bestätigten B-01-Entscheidung. Der Eingriff in die CI-Workflowdatei beschränkt sich auf den Manifestpfad.

## Definition of Done

WP-010 ist nur abgeschlossen, wenn alle Akzeptanzkriterien einzeln erfüllt sind, beide Validator-Modi einschließlich Self-/Negativtests lokal bestehen, der vollständige Diff gegen den `baseCommit` ausschließlich die im WP-010-Manifest erlaubten Dateien enthält und der Abschlussstand auf `docs/wp-010-tests-standort` gepusht und remote verifiziert wurde.

`PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` und dieses Work Package geben denselben WP-010-Stand wieder. Es erfolgt kein Merge nach `main` und kein Pull Request. `ARCHITECTURE/OPEN_BLOCKERS.md` bleibt unverändert; die drei Produktfolgeblocker bleiben offen und fail-closed.

## Ergebnis

Pending – wird im Abschlusscommit dokumentiert.

## Validierung

Pending – wird im Abschlusscommit dokumentiert.

## Referenzen

[1]: ../AGENTS.md "Verbindliche Agentenleitlinie"
[2]: ../PROJECT_CONTROL/CURRENT_STATE.md "Aktueller Projektstand"
[3]: ../PROJECT_CONTROL/WORK_QUEUE.md "Produktionswarteschlange"
[4]: ../PROJECT_CONTROL/DEFINITION_OF_DONE.md "Definition of Done für technische Aufgaben"
[5]: ../PROJECT_CONTROL/AI_HANDOVER_RULES.md "Verbindliche Regeln für die KI-Übergabe"
[6]: ../ARCHITECTURE/MODULE_BOUNDARIES.md "Module Boundaries v0.3"
[7]: ../DECISIONS/ADR-026-validator-evidenz-und-scope-vertrauensanker.md "ADR-026 – Validator-Evidenz und Scope-Vertrauensanker"
[8]: ../DECISIONS/ADR-030-wp-scope-trust-anchor.md "ADR-030 – Gemeinsamer historischer WP-/Scope-Trust-Anchor"
[9]: ../WORK_PACKAGES/WP-011_PR-Scope-Checkout-Korrektur.md "WP-011 – PR-Scope-Checkout-Korrektur"
[10]: ../.github/workflows/validate.yml "GitHub-Actions-Workflow Architecture Validation"
[11]: ../tools/architecture-validation/scopes/WP-010.documentation.scope.json "WP-010-Documentation-Scope-Manifest (Trust-Anchor)"
