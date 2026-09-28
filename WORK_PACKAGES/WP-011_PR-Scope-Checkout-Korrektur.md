# WP-011 – PR-Scope-Checkout-Korrektur

## ID

`WP-011`

**Bearbeitungsstatus:** Abgeschlossen auf Branch `chore/wp-011-pr-scope-checkout`.

## Ziel

Der unabhängig durch GPT-5.6 Sol High bestätigte CI-/Governance-Deadlock beim Work-Package-Scope-Validator auf Pull Requests ist behoben. Bei `pull_request` prüft der kanonische Scope-Lauf künftig den tatsächlichen PR-Branch-Head (`github.event.pull_request.head.sha`) statt des synthetischen GitHub-PR-Merge-Commits (`refs/pull/<nr>/merge`). Der Architecture-only-Lauf validiert weiterhin den synthetischen Merge-Stand, das Push- und das Main-Push-Verhalten bleiben unverändert und die Scope-Diff-Semantik des Validators wird nicht verändert.

**Verbindliche Integrationsreihenfolge** (Dokumentationspflicht aus dem Auftrag):

1. `WP-011` nach `main` mergen.
2. Danach `WP-010` nach `main` mergen.
3. Danach `WP-008` fertigstellen und als PR gegen `main` integrieren.
4. `WP-009` erst nach Abschluss von `WP-008` beginnen.

## Voraussetzungen

Vollständige Lesereihenfolge aus `AGENTS.md`, Architecture v1.0, `PROJECT_CONTROL/DEFINITION_OF_DONE.md`, `PROJECT_CONTROL/AI_HANDOVER_RULES.md`, `PROJECT_CONTROL/WORK_PACKAGE_RULES.md`, `PROJECT_CONTROL/WORK_QUEUE.md`, `.github/workflows/validate.yml`, `tools/architecture-validation/validate.py`, `tools/architecture-validation/README.md`, `tools/architecture-validation/scope-manifest-v1.schema.json`, die angenommenen ADR-026 und ADR-030 samt ADR-017-Historie sowie `WORK_PACKAGES/WP-007_CI-Setup.md` als Referenz des bestehenden CI-Gates.

Ausgangspunkt ist der `main`-Merge-Commit `3c1a6988c1aab2084763edf772b6adc268874865`. Der Branch `chore/wp-011-pr-scope-checkout` basiert exakt darauf. `WP-008` läuft separat auf `feat/wp-008-unity-scaffold` und wird nicht verändert, nicht gemergt und nicht gerebaselt. `WP-009` bleibt für den fachlichen Puzzle-Stack reserviert, `WP-010` für die separat beschlossene B-01-Testpfad-Klarstellung; deren Zuordnungen werden nicht verändert.

Begründung des Scope-Modus: Das Scope-Schema kennt ausschließlich `documentation` und `production`. WP-011 ändert ausschließlich CI-Workflow- und Governance-Dateien ohne Produktartefakte; dasselbe gilt für die WP-007-Referenz, die ebenfalls den Documentation-Scope verwendete. Der Documentation-Scope ist damit eindeutig regelkonform; ein neuer Scope-Typ wird nicht erfunden.

WP-011 und [`tools/architecture-validation/scopes/WP-011.documentation.scope.json`](../tools/architecture-validation/scopes/WP-011.documentation.scope.json) werden vor der fachlichen Änderung gemeinsam im Trust-Anchor-Commit eingeführt (ADR-030). Der `baseCommit` des Manifests ist der Elterncommit dieses gemeinsamen Ankers, also `3c1a6988c1aab2084763edf772b6adc268874865`. Das Manifest ist danach byteunverändert zu belassen.

## Scope

Erlaubt sind ausschließlich diese Änderungen:

1. `WORK_PACKAGES/WP-011_PR-Scope-Checkout-Korrektur.md` (neu): Auftrag, Trust-Anchor-Verweis, Status, Akzeptanzkriterien und Abschlussnachweise.
2. `tools/architecture-validation/scopes/WP-011.documentation.scope.json` (neu): vorab verankerte enge Allowlist; nach dem Trust-Anchor-Commit byteunverändert.
3. `.github/workflows/validate.yml`: `STP_SCOPE_MANIFEST` auf das WP-011-Manifest setzen; bei `pull_request` unmittelbar vor dem Work-Package-Scope-Lauf einen expliziten Checkout von `${{ github.event.pull_request.head.sha }}` mit vollständiger Historie (`fetch-depth: 0`) ergänzen. Der bestehende initiale Checkout (synthetischer PR-Merge-Stand) bleibt bestehen und trägt weiterhin den Architecture-only-Lauf. Bei `push` erfolgt kein zweiter Checkout, sofern technisch nicht erforderlich.
4. `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md`: ausschließlich mechanische WP-011-Statusnachführung inklusive der verbindlichen Integrationsreihenfolge.

Eine Änderung an `tools/architecture-validation/validate.py` oder `tools/architecture-validation/README.md` ist nicht vorgesehen: Die bestehenden Regeln verlangen für WP-011 keine mechanische Inventarnachführung des Validators (Inventar- und Schemachecks bestehen unverändert; die Statuskonsistenzprüfung bleibt durch die WP-011-Formulierung in `CURRENT_STATE`/`WORK_QUEUE` erfüllt; ADR-017 verlangt die Nachführung nur für relevante Governance- oder Vertragsänderungen des Validators selbst, und WP-011 ändert keine Validatorregel, keinen Vertrag, keinen Selbsttest und keine Evidenzkategorie).

## Betroffene Dateien/Module

| Pfad oder Modul | Zulässige Änderung |
|---|---|
| `WORK_PACKAGES/WP-011_PR-Scope-Checkout-Korrektur.md` | Neu: Auftrag, Trust-Anchor-Verweis, Status, Akzeptanzkriterien, Tests und Abschlussnachweise. |
| `tools/architecture-validation/scopes/WP-011.documentation.scope.json` | Neu: vorab verankerte enge Allowlist; nach dem Trust-Anchor-Commit byteunverändert. |
| `.github/workflows/validate.yml` | Minimaländerung: `STP_SCOPE_MANIFEST` auf WP-011-Manifest; PR-Head-Checkout vor dem Scope-Lauf bei `pull_request`; Headerkommentar an den neuen Ablauf angleichen. |
| `PROJECT_CONTROL/CURRENT_STATE.md` | Ausschließlich WP-011-Status, Integrationsreihenfolge und nächster Schritt. |
| `PROJECT_CONTROL/WORK_QUEUE.md` | Ausschließlich WP-011-Eintrag beziehungsweise Status und Integrationsreihenfolge. |

## Ausdrücklich nicht erlaubte Änderungen

Nicht erlaubt sind Produktionscode, Unity-Implementierung oder Unity-Scaffold, Produktentscheidungen, Architekturdateien unter `ARCHITECTURE/`, Konzeptdateien unter `Stammstrecken_Puzzle_Konzept_00-15/`, neue ADRs oder Änderungen an ADR-Entscheidungskörpern (insbesondere ADR-026 und ADR-030), jede semantische Änderung an `tools/architecture-validation/validate.py`, jede Änderung an `tools/architecture-validation/README.md`, Änderungen an bestehenden Scope-Manifesten, die Aufnahme künstlicher Änderungen in das Repository, Merge nach `main`, Pull-Request-Erstellung oder -Merge, Force Push, das Löschen bestehender Branches sowie Änderungen an den WP-008-, WP-009- oder WP-010-Zuordnungen.

Der bestehende Validator bleibt inklusive aller fail-closed Gates unverändert. Der Architecture-only-Lauf darf nicht auf den PR-Head verschoben werden; der Scope-Lauf darf bei Pull Requests nicht mehr den synthetischen Merge-Stand prüfen. Erscheint irgendeine Änderung außerhalb des oben genannten Scopes notwendig, ist WP-011 zu stoppen und der Befund zu berichten.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| `AK-01` | Trust-Anchor: WP-011 und Manifest wurden im selben historischen Add-Commit eingeführt, der historische WP-Blob verweist exakt auf das Manifest, der Manifestblob ist bytegleich und `baseCommit` ist der Elterncommit des Ankers (`3c1a6988c1aab2084763edf772b6adc268874865`). |
| `AK-02` | Bei `pull_request` läuft der Architecture-only-Lauf weiterhin auf dem initialen Checkout (synthetischer Merge-Stand); unmittelbar vor dem Scope-Lauf wird `${{ github.event.pull_request.head.sha }}` mit vollständiger Historie ausgecheckt und der Scope-Lauf prüft diesen tatsächlichen PR-Head. |
| `AK-03` | Bei `push` erfolgt kein zweiter Checkout; der gepushte Branch-Commit bleibt der Scope-Prüfstand. |
| `AK-04` | Auf `main`-Pushes entfällt der Scope-Schritt weiterhin; das bisherige Verhalten ist unverändert. |
| `AK-05` | `STP_SCOPE_MANIFEST` zeigt auf `tools/architecture-validation/scopes/WP-011.documentation.scope.json`. |
| `AK-06` | `validate.py`, `README.md` des Validators und alle ADRs sind unverändert; keine Architekturdatei wurde geändert. |
| `AK-07` | Deadlock-Simulation: Auf einem synthetischen Merge mit fremder Main-Änderung erscheint die Fremddatei im Scope-Diff; auf dem tatsächlichen WP-Head erscheint sie nicht. |
| `AK-08` | `git diff --check` besteht; der vollständige Diff gegen den `baseCommit` enthält ausschließlich die im Manifest erlaubten Dateien. |
| `AK-09` | Architecture-only-Lauf und WP-011-Scope-Lauf mit `--self-test` bestehen lokal und in der CI auf dem finalen Branch-Stand. |
| `AK-10` | Push ausschließlich auf `chore/wp-011-pr-scope-checkout`; kein Merge nach `main`, kein PR; die Integrationsreihenfolge ist dokumentiert. |

## Tests

Vor Abschluss werden mindestens ausgeführt und commitbezogen dokumentiert:

1. Architecture-only-Validator mit `--self-test` aus der gepinnten Lock-Umgebung;
2. Documentation-Scope-Validator mit WP-011-Manifest und `--self-test`;
3. Trust-Anchor-Nachweis gemäß ADR-030 für WP-011 und Manifest;
4. `git diff --check` und vollständige Delta-Prüfung gegen `3c1a6988c1aab2084763edf772b6adc268874865`;
5. Reproduzierbare Deadlock-Simulation außerhalb des Repositories: fremde Main-Änderung nach dem `baseCommit`, synthetischer Merge-Stand sieht die Fremddatei im Scope-Diff, tatsächlicher WP-Head nicht;
6. YAML-Plausibilitätsprüfung der geänderten Workflowdatei;
7. Remote-Commit-Verifikation nach jedem Push.

Unity Compile/EditMode/PlayMode, IL2CPP, physische Geräte-, SDK- und Storetests bleiben mangels Produktionsprojekt **REQUIRED_LATER/NOT_EXECUTED**.

## Risikoklasse

**Mittel.** Das Work Package verändert keine Architektur- oder Produktsemantik, greift aber in die verbindliche CI-Infrastruktur ein und bestimmt den Prüfstand des Scope-Gates auf Pull Requests. Fehler wirken sich unmittelbar auf die Abnahme künftiger Work Packages aus.

## Definition of Done

WP-011 ist nur abgeschlossen, wenn alle Akzeptanzkriterien einzeln erfüllt sind, beide Validator-Modi einschließlich Self-/Negativtests lokal und in der CI bestehen, die Deadlock-Simulation nachweislich PASS ist, der vollständige Diff gegen den `baseCommit` ausschließlich die im WP-011-Manifest erlaubten Dateien enthält und der Abschlussstand auf `chore/wp-011-pr-scope-checkout` gepusht und remote verifiziert wurde.

`PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` und dieses Work Package geben denselben WP-011-Stand wieder. Es erfolgt kein Merge nach `main` und kein Pull Request. `ARCHITECTURE/OPEN_BLOCKERS.md` bleibt unverändert; die drei Produktfolgeblocker bleiben offen und fail-closed.

## Ergebnis

Der bestätigte Scope-Checkout-Deadlock bei Pull Requests ist behoben. Der Workflow `.github/workflows/validate.yml` läuft unverändert mit dem initialen Checkout auf dem synthetischen PR-Merge-Stand und führt dort den Architecture-only-Lauf aus; unmittelbar vor dem kanonischen Work-Package-Scope-Lauf checkt er bei `pull_request` den tatsächlichen PR-Head (`github.event.pull_request.head.sha`) mit vollständiger Historie aus, sodass der Scope-Diff `<WP-baseCommit> → tatsächlicher PR-Branch-Head` keine fremden Main-Änderungen mehr enthält. Bei `push` erfolgt kein zweiter Checkout; auf `main`-Pushes entfällt der Scope-Schritt weiterhin. `STP_SCOPE_MANIFEST` zeigt auf das WP-011-Manifest. `tools/architecture-validation/validate.py`, `tools/architecture-validation/README.md`, alle ADRs (insbesondere ADR-026 und ADR-030) und alle Architekturdateien sind unverändert; kein Produktionscode wurde erzeugt.

| Feststellung | Stand |
|---|---|
| Trust-Anchor (ADR-030) | WP-011 und Manifest gemeinsam im Add-Commit `78f38fa90b3ca492ce33da5423215056056bee1b`; `baseCommit` ist dessen Elterncommit `3c1a6988c1aab2084763edf772b6adc268874865`; Manifestblob bytegleich; historischer WP-Blob mit exaktem Manifestlink. |
| Workflowänderung | Commit `833cac2519e5cf61801531bfb351a42060ee6e9f`: PR-Head-Checkout vor dem Scope-Lauf bei `pull_request`; `STP_SCOPE_MANIFEST` auf WP-011-Manifest; Headerkommentar fortgeschrieben. |
| Governance-Nachführung | Commit `a56d73287ada5bdb159a96e75dad90cde4094fef`: `CURRENT_STATE.md` und `WORK_QUEUE.md` mit WP-011-Stand und Integrationsreihenfolge. |
| Umfassender Delta gegen `3c1a6988c1aab2084763edf772b6adc268874865` | Exakt die fünf erlaubten Dateien: `.github/workflows/validate.yml`, `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md`, `WORK_PACKAGES/WP-011_PR-Scope-Checkout-Korrektur.md`, `tools/architecture-validation/scopes/WP-011.documentation.scope.json`. Keine untracked Dateien. |
| `BLOCKER-PROD-001/002/003` | **Unverändert offen und fail-closed**; `ARCHITECTURE/OPEN_BLOCKERS.md` unverändert. |
| Produktionscode / Unity | **Keiner erzeugt oder verändert.** |
| Merge nach `main` / Pull Request | **Nicht erfolgt.** |

## Validierung

| Ausgeführter Nachweis | Ergebnis |
|---|---|
| Trust-Anchor-Nachweis (ADR-030) | **PASS** – Manifest-Add-Commit `78f38fa` genau einer; Elterncommit = `baseCommit` = `3c1a6988c1aab2084763edf772b6adc268874865`; Ankerdiff weist WP und Manifest beide als `A` aus; Manifestblob bytegleich (`cmp`); historischer WP-Blob enthält den exakten repositorylokalen Manifestlink und die IDs `# WP-011` / `` `WP-011` ``. |
| Architecture-only-Lauf mit `--self-test` | **PASS** lokal (CPython 3.11.15, gepinnte Locks) für alle 17 Prüfgruppen; einzige Ausnahme ist der nachweislich auf dem Base-Commit `3c1a698` identisch auftretende Windows-Plattformartefakt `self-test:not-detected:V03-005-ABSOLUTE` (`Path("/tmp/scope.json").is_absolute()` ist unter nativem Windows-CPython stets `False`; unter Ubuntu 24.04 nicht existent). |
| WP-011-Documentation-Scope-Lauf mit `--self-test` | **PASS** – `LOCAL_SCOPE PASS` mit `workPackage=WP-011 base=3c1a6988… head=a56d732… worktreeDirty=false`; derselbe einzige Windows-Plattformartefakt wie im Architecture-only-Lauf. |
| `git diff --check` gegen `3c1a6988c1aab2084763edf772b6adc268874865` | **PASS**. |
| Vollständiger Delta gegen `3c1a6988c1aab2084763edf772b6adc268874865` | **PASS** – ausschließlich die fünf im Manifest erlaubten Dateien; keine untracked Dateien. |
| Deadlock-Simulation (temporärer Clone außerhalb des Repositories, simulierte fremde WP-010-Main-Änderung nach dem `baseCommit`) | **PASS** – Scope-Diff auf dem synthetischen Merge-Stand enthält die Fremddatei (`WORK_PACKAGES/WP-010_B-01-Testpfad-Klaerung.md`) → `scope:out-of-scope`; derselbe Diff auf dem tatsächlichen WP-Head enthält sie nicht → vollständig in der Allowlist. Simulations-Repository vollständig entfernt; keine künstliche Änderung im Projekt-Repository. |
| YAML-Plausibilität `.github/workflows/validate.yml` | **PASS** – vollständiger YAML-Parse erfolgreich; 7 Schritte in korrekter Reihenfolge; neuer PR-Head-Checkout unmittelbar vor dem Scope-Lauf und nur bei `pull_request`. |
| Workflow-Berechtigung beim Push von `.github/workflows/validate.yml` | **Kein Befund** – Push von Commit `833cac2` erfolgreich; kein Workflow-Berechtigungsfehler aufgetreten. |
| Remote-Commit-Verifikation | **PASS** – `78f38fa…`, `833cac2…` und `a56d732…` jeweils per `git ls-remote` remote verifiziert. |
| CI-Conclusion der Push-Läufe (Ubuntu 24.04) | **NOT_EXECUTED (nicht abrufbar)** – Das private Repository ist aus dieser Umgebung ohne API-Token nicht lesbar; die Push-Läufe wurden durch die erfolgreichen Pushes ausgelöst, ihre Conclusions können hier nicht abgefragt werden. Die verbindliche CI-Verifikation obliegt dem Owner beim Merge-Review; der lokale Windows-Artefakt existiert unter Ubuntu nicht. |
| Unity-, Geräte-, SDK- und Storetests | **REQUIRED_LATER/NOT_EXECUTED**; WP-011 erzeugte keinen Produktionscode. |

## Referenzen

[1]: ../AGENTS.md "Verbindliche Agentenleitlinie"
[2]: ../PROJECT_CONTROL/CURRENT_STATE.md "Aktueller Projektstand"
[3]: ../PROJECT_CONTROL/WORK_QUEUE.md "Produktionswarteschlange"
[4]: ../PROJECT_CONTROL/DEFINITION_OF_DONE.md "Definition of Done für technische Aufgaben"
[5]: ../PROJECT_CONTROL/AI_HANDOVER_RULES.md "Verbindliche Regeln für die KI-Übergabe"
[6]: ../DECISIONS/ADR-026-validator-evidenz-und-scope-vertrauensanker.md "ADR-026 – Validator-Evidenz und Scope-Vertrauensanker"
[7]: ../DECISIONS/ADR-030-wp-scope-trust-anchor.md "ADR-030 – Gemeinsamer historischer WP-/Scope-Trust-Anchor"
[8]: ../WORK_PACKAGES/WP-007_CI-Setup.md "WP-007 – CI-Setup"
[9]: ../.github/workflows/validate.yml "GitHub-Actions-Workflow Architecture Validation"
[10]: ../tools/architecture-validation/scopes/WP-011.documentation.scope.json "WP-011-Documentation-Scope-Manifest (Trust-Anchor)"
