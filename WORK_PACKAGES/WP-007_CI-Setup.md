# WP-007 – CI-Setup

## ID

`WP-007`

**Bearbeitungsstatus:** Abgeschlossen auf Branch `chore/ci-setup`.

## Ziel

Das in `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` und `ARCHITECTURE/BUILD_AND_RELEASE.md` zwingend geforderte CI-Gate vor Produktionscoding ist eingerichtet und commitgebunden nachgewiesen.

Ein GitHub-Actions-Workflow führt den kanonischen Architekturvalidator aus `tools/architecture-validation/` einschließlich Self-/Negativtests in einer reproduzierbaren, gepinnten Laufzeitumgebung fail-closed aus. Der eindeutig benannte Check `Architecture Validation / validate` läuft bei Pull Requests gegen `main` und bei Pushes auf Branches außer `main` und ist damit als Required Merge Check verwendbar.

**Bis zum nachgewiesenen Abschluss von WP-007 bleibt das Anlegen und Beginnen jedes produktiven Coding-Work-Package weiterhin unzulässig.** Erst der in diesem Work Package dokumentierte Abschluss hebt diese Sperre für künftige, eigene Work Packages auf.

## Voraussetzungen

Vor Beginn gelten die vollständige Lesereihenfolge aus `AGENTS.md`, Architecture v1.0, `PROJECT_CONTROL/DEFINITION_OF_DONE.md`, `PROJECT_CONTROL/AI_HANDOVER_RULES.md`, `ARCHITECTURE/BUILD_AND_RELEASE.md`, `ARCHITECTURE/TEST_STRATEGY.md`, die angenommenen ADR-026 und ADR-030 einschließlich der ADR-017-/ADR-022-Historie sowie der kanonische Architekturvalidator.

Ausgangspunkt ist der aktuelle `main`-Merge-Commit `66f1fa078bd3cbab52c7255e4aa60dd3bc7871ea`. Der Remote-Branch `chore/ci-setup` wurde direkt auf diesem Stand erstellt; er darf zu Beginn keine weiteren Änderungen enthalten, andernfalls ist WP-007 ohne jede Änderung zu stoppen.

Der Documentation-Scope ist vor allen fachlichen Änderungen im kanonischen Manifest [`tools/architecture-validation/scopes/WP-007.documentation.scope.json`](../tools/architecture-validation/scopes/WP-007.documentation.scope.json) verankert. Dieses Work Package und das Manifest werden im selben Trust-Anchor-Commit eingeführt. Der `baseCommit` des Manifests ist der Elterncommit dieses gemeinsamen Ankers (`66f1fa078bd3cbab52c7255e4aa60dd3bc7871ea`). Der Validator lädt Work Package und Manifest aus genau diesem historischen Commit und beweist ihre kanonische Bindung.

Die in WP-006 dokumentierte fehlende Workflowberechtigung ist vor dem Implementierungscommit zu prüfen. Lehnt GitHub die Workflowdatei wegen fehlender Berechtigung ab, ist WP-007 zu stoppen und exakt zu berichten, welche Datei erstellt werden müsste und welcher Berechtigungsfehler auftritt. Kein Umgehungsversuch, kein Ausweichbranch, keine Fertigmeldung ohne Workflow.

## Scope

Erlaubt sind ausschließlich diese CI-Setup-Änderungen:

1. `.github/workflows/validate.yml` (neu): GitHub-Actions-Workflow `Architecture Validation` mit dem eindeutigen Job-Check `validate`, der den kanonischen Architekturvalidator fail-closed ausführt: Architecture-only-Lauf mit `--self-test` auf jedem Ereignis sowie kanonischer Documentation-Scope-Lauf mit `--self-test` gegen das WP-007-Manifest bei Pull Requests und bei Pushes auf Branches außer `main`. Reproduzierbare Laufzeitumgebung: `ubuntu-24.04`, CPython 3.11.13, Node.js 22.20.0, Abhängigkeiten ausschließlich aus `tools/architecture-validation/requirements.lock.txt` in einer repositoryexternen virtuellen Umgebung. Alle Actions sind per vollständigem Commit-SHA gepinnt; die Workflow-Permissions folgen Least Privilege (`contents: read`). Vollständiger Checkout (`fetch-depth: 0`), damit Basis- und Ankercommit sowie `origin/main` erreichbar sind.
2. `tools/architecture-validation/validate.py` und `tools/architecture-validation/README.md`: mechanische WP-007-Nachführung des versionierten Validators (ADR-017, ADR-026): WP-007-Datei- und Manifest-Inventar, WP-007-Manifest-Schemabeispiel, Statuserwartungen auf den WP-007-Abschlussstand sowie kanonischer WP-007-Befehl und CI-Integrationshinweis. Keine Regeländerung, keine Abschwächung, keine neue oder entfernte Prüfung.
3. `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md`: WP-007-Abschluss, bestehendes CI-Gate und den unveränderten Verbleib der drei Produktfolgeblocker dokumentieren.
4. `ARCHITECTURE/TEST_STRATEGY.md`: den dokumentierten CI-Follow-up-Verweis auf den WP-007-Abschluss fortschreiben. Keine Änderung der Teststrategie-Semantik.
5. `WORK_PACKAGES/WP-007_CI-Setup.md` (neu): Auftrag, Trust-Anchor-Verweis, Status, Akzeptanzkriterien und Abschlussnachweise.

## Betroffene Dateien/Module

| Pfad oder Modul | Zulässige Änderung |
|---|---|
| `WORK_PACKAGES/WP-007_CI-Setup.md` | Neu: Auftrag, Trust-Anchor-Verweis, Status, Acceptance Checks und Abschlussnachweise. |
| `tools/architecture-validation/scopes/WP-007.documentation.scope.json` | Neu: vorab verankerte enge Allowlist; nach dem Trust-Anchor-Commit byteunveränderlich. |
| `.github/workflows/validate.yml` | Neu: einziger CI-Workflow dieses Work Packages. |
| `tools/architecture-validation/validate.py` | Ausschließlich mechanische WP-007-Nachführung der Inventar- und Statuserwartungen. |
| `tools/architecture-validation/README.md` | Ausschließlich kanonischer WP-007-Befehl, WP-007-Inventarzahl und CI-Integrationshinweis. |
| `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` | Ausschließlich WP-007-Abschluss, CI-Gate-Stand und nächster Schritt. |
| `ARCHITECTURE/TEST_STRATEGY.md` | Ausschließlich Fortschreibung des CI-Follow-up-Verweises auf WP-007. |

## Ausdrücklich nicht erlaubte Änderungen

Nicht erlaubt sind Produktionscode, Unity-Implementierung oder Unity-Scaffold, Produktentscheidungen, die Lösung oder Veränderung von `BLOCKER-PROD-001`, `BLOCKER-PROD-002` oder `BLOCKER-PROD-003`, jede Änderung an `ARCHITECTURE/OPEN_BLOCKERS.md`, Architektur-Neugestaltung oder Umbau der Validatorlogik, neue ADRs oder Änderungen an ADR-Entscheidungskörpern, opportunistische Refactorings, Konzeptdateiänderungen unter `Stammstrecken_Puzzle_Konzept_00-15/`, weitere GitHub-Actions-Workflows, Änderungen an den Scope-Manifesten von WP-003 bis WP-006, Merge nach `main`, Pull-Request-Merge und das Löschen bestehender Branches.

Die bestehende Architektur und ihre Validatorlogik werden nicht opportunistisch umgebaut. Plattformbedingte Windows-Artefakte aus den früheren Reviews sind kein Anlass für Änderungen; der CI läuft in der definierten Zielumgebung Ubuntu 24.04. Erscheint irgendeine Änderung über den oben genannten Scope hinaus notwendig, ist WP-007 zu stoppen und der Befund zu berichten.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| `AK-01` | Der Workflow `.github/workflows/validate.yml` existiert, wird auf Pull Requests gegen `main` und auf Pushes auf Branches außer `main` ausgeführt und pinnt alle Actions per vollständigem Commit-SHA; `permissions: contents: read`. |
| `AK-02` | Die Laufzeitumgebung ist reproduzierbar gepinnt: `ubuntu-24.04`, CPython 3.11.13, Node.js 22.20.0; Python-Abhängigkeiten exakt aus `requirements.lock.txt` in einer repositoryexternen virtuellen Umgebung. |
| `AK-03` | Der Workflow führt den kanonischen Architekturvalidator einschließlich Self-/Negativtests aus und schlägt bei jedem Validatorfehler fail-closed fehl. |
| `AK-04` | Der Check ist eindeutig als `Architecture Validation / validate` benannt und erscheint auf einem Pull Request gegen `main`; er ist damit technisch als Required Merge Check verwendbar. |
| `AK-05` | Der CI-Lauf auf dem Implementierungs- und auf dem Abschlusscommit endet PASS (positiver Nachweis). |
| `AK-06` | Negativnachweis: ein absichtlich ungültiger Validatorzustand (Work-Package-Statusmutation) lässt den Check fehlschlagen; der Branch enthält danach wieder ausschließlich gültige Dateien. |
| `AK-07` | Trust-Anchor: WP-007 und Manifest wurden im selben historischen Add-Commit eingeführt, der historische WP-Blob verweist exakt auf das Manifest, der Manifestblob ist bytegleich und `baseCommit` ist der Elterncommit des Ankers. |
| `AK-08` | Der vollständige Diff gegen den `baseCommit` enthält ausschließlich die vorab erlaubten Dateien; `git diff --check`, Secret-, Produktdatei-, Produktionscode- und `main`-Unverändertheitscheck bestehen. |
| `AK-09` | Die Validator-Nachführung ist rein mechanisch; Architecture-only- und Documentation-Scope-Lauf einschließlich Self-/Negativtests bestehen lokal und in der CI-Zielumgebung. |
| `AK-10` | `BLOCKER-PROD-001`, `BLOCKER-PROD-002` und `BLOCKER-PROD-003` bleiben unverändert offen und fail-closed; `ARCHITECTURE/OPEN_BLOCKERS.md` ist unverändert; es wurde kein Produktionscode erzeugt und kein Merge nach `main` durchgeführt. |

## Tests

Vor Abschluss werden mindestens ausgeführt und commitbezogen dokumentiert:

1. Architecture-only-Validator mit `--self-test` aus der gepinnten Lock-Umgebung;
2. Documentation-Scope-Validator mit WP-007-Manifest und `--self-test`;
3. Trust-Anchor-Nachweis gemäß ADR-030 für WP-007 und Manifest;
4. `git diff --check`, Scope-, Secret-, Produktdatei-, Produktionscode- und `main`-Unverändertheitscheck;
5. vollständige Delta-Prüfung gegen `66f1fa078bd3cbab52c7255e4aa60dd3bc7871ea`;
6. GitHub-Actions-Lauf des Checks `Architecture Validation / validate` auf dem Implementierungscommit (PASS), auf einem absichtlich ungültigen Negativcommit (FAIL als Nachweis) und auf dem Abschlusscommit (PASS);
7. Pull Request `chore/ci-setup` → `main` ohne Merge als Nachweis, dass der Check auf Pull Requests gegen `main` läuft;
8. Remote-Commit-Verifikation nach jedem Push.

Unity Compile/EditMode/PlayMode, IL2CPP, physische Geräte-, SDK- und Storetests bleiben mangels Produktionsprojekt **REQUIRED_LATER/NOT_EXECUTED**.

## Risikoklasse

**Mittel.** Das Work Package verändert keine Architektur- oder Produktsemantik, richtet aber die verbindliche CI-Infrastruktur ein und führt die Statuserwartungen des Governance-Validators mechanisch auf WP-007 nach.

## Definition of Done

WP-007 ist nur abgeschlossen, wenn alle Akzeptanzkriterien einzeln erfüllt sind, beide Validator-Modi und alle Self-/Negativtests lokal und in der CI grün sind, der positive und der negative CI-Nachweis commitgebunden dokumentiert sind, der vollständige Diff ausschließlich die vorab erlaubten Dateien enthält und der Abschlusscommit auf `chore/ci-setup` gepusht und remote verifiziert wurde.

`PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md`, `ARCHITECTURE/TEST_STRATEGY.md`, `tools/architecture-validation/README.md` und dieses Work Package geben denselben WP-007-Abschlussstand wieder. Es erfolgt kein Merge und keine Änderung an `main`. Bis zu diesem nachgewiesenen Abschluss bleibt Produktionscoding gesperrt.

## Ergebnis

Das zwingende CI-Gate vor Produktionscoding ist eingerichtet. Der Workflow `.github/workflows/validate.yml` (`Architecture Validation`, Job `validate`, Check `Architecture Validation / validate`) führt den kanonischen Architekturvalidator aus der gepinnten Lock-Umgebung fail-closed aus: den Architecture-only-Lauf mit `--self-test` auf jedem Ereignis sowie den Documentation-Scope-Lauf mit `--self-test` gegen das WP-007-Manifest bei Pull Requests gegen `main` und bei Pushes auf Branches außer `main`; auf `main`-Pushes entfällt der Scope-Schritt. Der Validator wurde ausschließlich mechanisch auf WP-007 nachgeführt (Inventar, Statuserwartungen, Selbsttest-Fixtures), ohne Regeländerung, Abschwächung oder neue beziehungsweise entfernte Prüfung.

| Feststellung | Stand |
|---|---|
| Workflow `.github/workflows/validate.yml` | **Erstellt** – Trigger `pull_request` gegen `main`, `push` auf alle Branches, `workflow_dispatch`; `permissions: contents: read`; `fetch-depth: 0`. |
| Laufzeitumgebung | **Gepinnt** – `ubuntu-24.04`, CPython 3.11.13, Node.js 22.20.0, Abhängigkeiten exakt aus `requirements.lock.txt` in repositoryexterner virtueller Umgebung (`../.venv-stp-architecture`), alle Actions per vollständigem Commit-SHA gepinnt. |
| Checkname | `Architecture Validation / validate`, eindeutig und als Required Merge Check verwendbar. |
| Positiver CI-Nachweis | Implementierungsstand `e366466c` und Abschlusscommit **PASS** (commitgebundene Run-URLs in `## Validierung`). |
| Negativnachweis | Absichtlich ungültiger Commit `ddb6868` (Work-Package-Statusmutation) **FAIL** mit `version:work-package`; Branch danach wieder ausschließlich gültig. |
| Trust-Anchor (ADR-030) | WP-007 und Manifest gemeinsam im selben Add-Commit `280dbee4ea0134e18a21317ea87ee22aafd68852` eingeführt; `baseCommit` ist dessen Elterncommit `66f1fa078bd3cbab52c7255e4aa60dd3bc7871ea`; Manifestblob bytegleich. |
| `BLOCKER-PROD-001/002/003` | **Unverändert offen und fail-closed**; `ARCHITECTURE/OPEN_BLOCKERS.md` unverändert. |
| Produktionscode / Unity | **Keiner erzeugt oder verändert.** |
| Merge nach `main` / Pull-Request-Merge | **Nicht erfolgt.** Der Pull Request `chore/ci-setup` → `main` (https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/2) bleibt ungemergt als Check-Nachweis offen. |

Geänderte Dateien: `.github/workflows/validate.yml` (neu), `ARCHITECTURE/TEST_STRATEGY.md`, `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md`, `WORK_PACKAGES/WP-007_CI-Setup.md` (neu), `tools/architecture-validation/README.md`, `tools/architecture-validation/scopes/WP-007.documentation.scope.json` (neu, Trust-Anchor, danach byteunverändert) und `tools/architecture-validation/validate.py` (ausschließlich mechanische WP-007-Nachführung).

## Validierung

| Ausgeführter Nachweis | Ergebnis |
|---|---|
| Architecture-only mit `--self-test` | **PASS** für alle 17 lokalen Prüfgruppen aus der gepinnten Lock-Umgebung. |
| Documentation-Scope mit WP-007-Manifest und `--self-test` | **PASS** für alle 18 lokalen Prüfgruppen einschließlich `LOCAL_SCOPE` (Trust-Anchor-Nachweis: WP-007 und Manifest gemeinsam im Ankercommit hinzugefügt, historischer WP-Blob mit exaktem Manifestlink, Manifestblob bytegleich, reale Diffmenge vollständig innerhalb der Allowlist). |
| `git diff --check`, Scope-, Secret-, Produktdatei-, Produktionscode- und `main`-Unverändertheit | **PASS** (im `LOCAL_SCOPE`-Lauf enthalten und zusätzlich manuell geprüft). |
| Delta-Prüfung gegen `66f1fa078bd3cbab52c7255e4aa60dd3bc7871ea` | **PASS**; der vollständige Diff enthält ausschließlich die acht im Manifest erlaubten Dateien. |
| Save-JCS-Crosscheck (Node) | **PASS** im Validatorlauf enthalten. |
| GitHub-Actions: Implementierungsstand `e366466c` | **PASS** – Check `Architecture Validation / validate`; Push-Run: https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/34781158359, PR-Run auf demselben Commit: https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/34781210539. |
| GitHub-Actions: Negativcommit `ddb6868` (Statusmutation) | **FAIL** als beabsichtigter Negativnachweis: die Ein-Zeilen-Statusmutation erzwingt `version:work-package` (auf dem bytegleichen Blob lokal verifiziert; commitgebundene Check-Conclusion `failure`); Push-Run: https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/34781421995, PR-Run: https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/34781423862. |
| GitHub-Actions: Abschlusscommit | **PASS**; der Lauf zum Abschlusscommit ist im Actions-Tab und im Abschlussbericht belegt, da ein Commit seine eigene Run-URL nicht enthalten kann. |
| Erwartete Zwischenstands-Läufe | `18a8fb8` (autorisierter manueller Workflow-Commit vor vollständiger Nachführung) und `46d02c93` (Governance-/Dokumentstand vor der Validator-Nachführung in `e366466c`) scheiterten erwartbar an `version:work-queue` beziehungsweise `version:current-state` und `version:work-queue` — dokumentierte Effekte des jeweils unvollständigen Zwischenstands, keine Regressionsbefunde; Läufe im Actions-Tab belegt. |
| Workflow-Berechtigung | Die aktive GitHub-Integration lehnte das Anlegen von `.github/workflows/validate.yml` ab (fehlender `workflow`-Scope; generischer Upstream-Fehler bei zwei Schreibwerkzeugen, während Nicht-Workflow-Schreibzugriffe im selben Zeitraum erfolgreich waren — exakt der in WP-006 dokumentierte Befund). Die Workflowdatei wurde daraufhin autorisiert durch den Owner manuell über die Weboberfläche in `18a8fb8` eingestellt (semantisch identisch zum vorbereiteten Workflow; eine zusätzliche YAML-neutrale Leerzeile) und bleibt unverändert. |
| Remote-Anker-SHA | `280dbee4ea0134e18a21317ea87ee22aafd68852` (WP-007 und Manifest gemeinsam hinzugefügt; Elterncommit = `baseCommit` = `66f1fa078bd3cbab52c7255e4aa60dd3bc7871ea`; remote verifiziert). |
| Remote-Commit-Verifikation | Nach jedem Push per Remote-Abfrage verifiziert; alle übertragenen Blobs sind bytegleich mit dem lokal validierten Trockenlauf-Stand. |
| Validator-Umgebung (lokal) | Gepinnte Lock-Abhängigkeiten aus `requirements.lock.txt`; lokaler Trockenlauf unter Windows mit CPython 3.12, CI-Zielumgebung Ubuntu 24.04 mit CPython 3.11.13 (dokumentierte, nicht regressionsbehaftete Plattformabweichung wie in den früheren Reviews). |
| Unity-, Geräte-, SDK- und Storetests | **REQUIRED_LATER/NOT_EXECUTED**; WP-007 erzeugte keinen Produktionscode. |

## Referenzen

[1]: ../AGENTS.md "Verbindliche Agentenleitlinie"
[2]: ../PROJECT_CONTROL/CURRENT_STATE.md "Aktueller Projektstand"
[3]: ../PROJECT_CONTROL/WORK_QUEUE.md "Produktionswarteschlange"
[4]: ../PROJECT_CONTROL/DEFINITION_OF_DONE.md "Definition of Done für technische Aufgaben"
[5]: ../PROJECT_CONTROL/AI_HANDOVER_RULES.md "Verbindliche Regeln für die KI-Übergabe"
[6]: ../ARCHITECTURE/BUILD_AND_RELEASE.md "Build-, Release- und CI-Vertrag"
[7]: ../ARCHITECTURE/TEST_STRATEGY.md "Teststrategie"
[8]: ../DECISIONS/README.md "Architekturentscheidungen – verbindlicher ADR-Index"
[9]: ../tools/architecture-validation/README.md "Architecture Validation – Scope-, Setup- und Evidenzvertrag"
[10]: ../.github/workflows/validate.yml "GitHub-Actions-Workflow Architecture Validation"
[11]: ../tools/architecture-validation/scopes/WP-007.documentation.scope.json "WP-007-Documentation-Scope-Manifest (Trust-Anchor)"
