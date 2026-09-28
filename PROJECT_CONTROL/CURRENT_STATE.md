# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und fachliche Spezifikationen sind verbindlich. **Architecture v1.0** ist der freigegebene Architekturstand; Produktionscode existiert weiterhin nicht. |
| Derzeitige Phase | `WP-001` bis `WP-007` sind abgeschlossen. WP-006 führte ausschließlich die formale administrative Promotion des unabhängig freigegebenen Architecture-v0.5-Stands auf Architecture v1.0 durch, ohne Architekturänderung, neue Produktentscheidung oder neue Reviewrunde. WP-007 richtete das zwingende CI-Gate vor Produktionscoding ein: GitHub-Actions-Workflow mit gepinnter Laufzeitumgebung, kanonischer Architekturvalidator einschließlich Self-/Negativtests, positiver und negativer commitgebundener CI-Nachweis auf dem Branch `chore/ci-setup`, ohne Merge nach `main`. `WP-011` (PR-Scope-Checkout-Korrektur) ist nach `main` gemergt (Merge-Commit `dca18b8fe228b6e852a0603b6cfeafc1795589bc`) und behebt den bestätigten Scope-Checkout-Deadlock bei Pull Requests; der main-CI-Lauf ist PASS. `WP-010` (Unity-Testassembly-Standort-Klarstellung) ist nach `main` gemergt (Merge-Commit `6010b9de587a0aca26deacea1e1fb87a5f5f3868`) und behebt den unabhängig durch GPT-5.6 Sol High bestätigten Architektur-Dokumentationsfehler B-01 ausschließlich in `ARCHITECTURE/MODULE_BOUNDARIES.md`. `WP-008` (Unity-6.3-Produktionsscaffold, Modulgraph und ausführbare CI-Basis) ist auf `feat/wp-008-unity-scaffold` umgesetzt: Unity-Projektbasis (6000.3.23f1, Force Text, Visible Meta Files, Paketlocks, `Toolchain.lock.md`), vollständiger normativer `.asmdef`-Modulgraph, Bootstrap-Composition mit QA-Szene, neun Testassemblies unter `Assets/StammstreckenPuzzle/Tests/` einschließlich `BootstrapCompositionSmoke` (B-01 durch die WP-010-Klarstellung aufgelöst) sowie Unity-CI mit fail-closed Evidence-Guard; offen bleiben ausschließlich die CI-Buildnachweise (B-02: `secrets.UNITY_LICENSE`/Runner, Android, iOS). Die Integration von WP-008 nach `main` ist als Pull Request vorbereitet. |
| Letzter abgeschlossener Schritt | WP-010 ist nach `main` gemergt (Merge-Commit `6010b9de587a0aca26deacea1e1fb87a5f5f3868`): `ARCHITECTURE/MODULE_BOUNDARIES.md` dokumentiert in Abschnitt 2 die tatsächliche physische Unity-Testverzeichnisstruktur unter `Assets/StammstreckenPuzzle/Tests/` und ordnet in Abschnitt 4 alle neun Testassemblies eindeutig diesen Pfaden zu, einschließlich `STP.Tests.Bootstrap.PlayMode`; wurzelige `Tests/`-Darstellung, Modulgraphsemantik, Referenzregeln und Teststrategie blieben unverändert, kein neuer ADR. Die commitgebundenen Nachweise stehen in `../WORK_PACKAGES/WP-010_Unity-Testassembly-Standort-Klarstellung.md`. Zuvor wurde `WP-011` nach `main` gemergt (Merge-Commit `dca18b8fe228b6e852a0603b6cfeafc1795589bc`, Scope-Lauf auf dem tatsächlichen PR-Head). Auf `feat/wp-008-unity-scaffold` ist WP-008 bis auf die CI-Buildnachweise (B-02) fertiggestellt und die Integration nach `main` vorbereitet: Der Integrationsmerge löst die Divergenz in `.github/workflows/validate.yml` (WP-008-Production-Scope-Manifest bei vollständig erhaltenem WP-011-PR-Head-Checkout) und in dieser Datei; WP-008-Trust-Anchor `8d3e245fcab8bce42b5e8efdded480f47dacfbb8` und WP-008-Manifest bleiben unverändert. |
| Nächster vorgesehener Schritt | Verbindliche Integrationsreihenfolge: 1. `WP-011` nach `main` mergen (erledigt, Merge-Commit `dca18b8`); 2. danach `WP-010` nach `main` mergen (erledigt, Merge-Commit `6010b9d`); 3. danach `WP-008` als Pull Request gegen `main` integrieren (vorbereitet; WP-008-Trust-Anchor und WP-008-Manifest bleiben unverändert); 4. `WP-009` erst nach erfolgreicher Integration von `WP-008` behandeln. Owner-Aufgabe unverändert: `secrets.UNITY_LICENSE` und reproduzierbare Runner konfigurieren (`vars.UNITY_RUNNERS_READY=true`); danach laufen die Unity-Nachweisjobs von `.github/workflows/unity.yml` unverändert und liefern die ausstehenden Buildnachweise (B-02). Bis dahin gilt weiterhin: Jedes künftige Work Package verankert sein eigenes Scope-Manifest im gemeinsamen Trust-Anchor-Commit und setzt `STP_SCOPE_MANIFEST` in `../.github/workflows/validate.yml` auf dieses Manifest. |
| Produktionscode | **Nur WP-008-Scaffold-Skelette** (`.asmdef`-Modulgraph, Ports, Adapter-, Präsentations- und Bootstrap-Composition ohne jede Fachlogik, Editor-Build-Entrypoints, QA-Szene) einschließlich der neun Testassemblies unter `Assets/StammstreckenPuzzle/Tests/` mit `BootstrapCompositionSmoke`. Keine Puzzle-, Solver-, Economy-, Persistenz- oder sonstige Featurelogik; der fachliche Puzzle-Stack bleibt `WP-009` vorbehalten. |
| Aktuell gültige Architekturversion | **Architecture v1.0**, angenommen am 2026-09-13; Einstieg: `../ARCHITECTURE/ARCHITECTURE.md`. |
| Architekturentscheidungen | 30 ADRs: 21 angenommen und aktuell wirksam, 9 als ersetzte Historie erhalten. Autoritativer Index: `../DECISIONS/README.md`. |
| Scope-Vertrauensanker | `WP-006` und `WP-006.documentation.scope.json` wurden gemeinsam in Commit `3e8441830552a2d99c4546fcebc2dc1b23de58cf` eingeführt; der WP-005-Anker `ebf522a9ef3c28035341cc04dbd6d8251b107603` bleibt als historischer Nachweis bestehen. `WP-007` und `WP-007.documentation.scope.json` wurden gemeinsam im Trust-Anchor-Commit `280dbee4ea0134e18a21317ea87ee22aafd68852` auf `chore/ci-setup` eingeführt; der Validator liest beide historischen Blobs und ihre exakte Verknüpfung aus diesem Commit. `WP-008` und `WP-008.production.scope.json` wurden gemeinsam im Trust-Anchor-Commit `8d3e245fcab8bce42b5e8efdded480f47dacfbb8` auf `feat/wp-008-unity-scaffold` eingeführt (Elterncommit = `baseCommit` = `3c1a6988c1aab2084763edf772b6adc268874865`); danach ist das Manifest byteunveränderlich. `WP-011` und `WP-011.documentation.scope.json` wurden gemeinsam im Trust-Anchor-Commit `78f38fa90b3ca492ce33da5423215056056bee1b` auf `chore/wp-011-pr-scope-checkout` eingeführt; dessen Elterncommit und Manifest-`baseCommit` ist `3c1a6988c1aab2084763edf772b6adc268874865`. `WP-010` und `WP-010.documentation.scope.json` wurden gemeinsam im Trust-Anchor-Commit `2357d51b1da3d92f351e5494cdaafe139e4c7497` auf `docs/wp-010-tests-standort` eingeführt; dessen Elterncommit und Manifest-`baseCommit` ist `dca18b8fe228b6e852a0603b6cfeafc1795589bc`. |
| CI-Follow-up | **Abgeschlossen (`WP-007`).** Das CI-Gate ist eingerichtet und commitgebunden nachgewiesen: autorisierter Workflow `../.github/workflows/validate.yml`, gepinnte Umgebung (`ubuntu-24.04`, CPython 3.11.13, Node.js 22.20.0, SHA-gepinnte Actions, `contents: read`), Architecture Validator mit Self-/Negativtests, commitgebundener PASS für den WP-007-Scope und der eindeutige Check `Architecture Validation / validate`, der als Required Merge Check verwendbar ist. |

## Verbindliche Grundlage

Die Konzeptdateien definieren unverändert den bestätigten Produktstand. Architecture v1.0 übersetzt ihn in technische Grenzen und dokumentiert fehlende Produktentscheidungen als fail-closed Folgeblocker. Lokale Struktur-, Semantik- und Scopebelege bleiben ausdrücklich von manuellen Dokumentreviews sowie späteren Unity-, Produktionscode-, Geräte-, SDK- und Storebelegen getrennt. Der CI-Nachweis des Architecture Validators ist seit WP-007 verbindlicher Bestandteil jedes Work-Package-Abschlusses.

## Work-Package-Kette

`WP-001` dokumentiert die ursprüngliche technische Produktionsspezifikation. `WP-002` schloss zwölf Sol-Review-Findings und hob auf Architecture v0.2. `WP-003` adressierte acht Astra-Findings und hob auf Architecture v0.3. `WP-004` schloss sieben verbliebene V03-Befunde und hob auf Architecture v0.4. `WP-005` schloss ausschließlich vier letzte HIGH-Lücken und hob den angenommenen Zwischenstand auf Architecture v0.5. `WP-006` promovierte den unabhängig freigegebenen v0.5-Stand rein formal auf Architecture v1.0, ohne eine technische Änderung. `WP-007` richtete das zwingende CI-Gate vor Produktionscoding ein und wies es commitgebunden auf `chore/ci-setup` nach, ohne Architekturänderung und ohne Merge. `WP-008` baute auf `feat/wp-008-unity-scaffold` das Unity-6.3-Produktionsscaffold mit normativem `.asmdef`-Modulgraph, Bootstrap-Composition, QA-Szene und neun Testassemblies unter `Assets/StammstreckenPuzzle/Tests/` auf; offen bleiben dort nur die CI-Buildnachweise (B-02), die Integration nach `main` ist als Pull Request vorbereitet. `WP-011` korrigierte auf `chore/wp-011-pr-scope-checkout` den bestätigten Scope-Checkout-Deadlock bei Pull Requests (Scope-Lauf auf tatsächlichem PR-Head, Architecture-only-Lauf unverändert auf synthetischem Merge-Stand), ohne Validator-, ADR- oder Architekturänderung; WP-011 ist inzwischen nach `main` gemergt. `WP-010` korrigierte auf `docs/wp-010-tests-standort` die dokumentierte physische Testverzeichnisstruktur in `ARCHITECTURE/MODULE_BOUNDARIES.md` (Tests physisch unter `Assets/StammstreckenPuzzle/Tests/`, alle neun Testassemblies eindeutig zugeordnet), ohne neue Architekturentscheidung und ohne ADR-Änderung; WP-010 ist inzwischen nach `main` gemergt.

Der kanonische lokale Abnahmelauf lautet:

```bash
../.venv-stp-architecture/bin/python tools/architecture-validation/validate.py \
  --scope documentation \
  --scope-manifest tools/architecture-validation/scopes/WP-007.documentation.scope.json \
  --self-test
```

Ein Abschluss ist nur gültig, wenn Architecture-only- und Documentation-Scope-Lauf, Self-/Negativtests, `git diff --check`, Scope-/Secret-/Produktdateiprüfung und Remote-Nachweis erfolgreich sind. Der GitHub-Actions-Nachweis des Checks `Architecture Validation / validate` ist seit WP-007 **verbindlich** und wurde in WP-007 positiv (Implementierungsstand und Abschlusscommit PASS) und negativ (absichtlich ungültiger Commit FAIL) ausgeführt. Unity-CI-Nachweise bleiben **NOT_EXECUTED/BLOCKED**, solange `secrets.UNITY_LICENSE` und reproduzierbare Runner nicht konfiguriert sind (WP-008, B-02); Geräte-, SDK- und Storetests bleiben **REQUIRED_LATER/NOT_EXECUTED**.

## CI-Gate vor Produktionscoding: erfüllt

Das zwingende CI-Gate vor Produktionscoding ist mit WP-007 vollständig erfüllt:

1. GitHub-Actions-Workflow mit autorisierter Workflowberechtigung: `../.github/workflows/validate.yml` ist remote wirksam; nach dem dokumentierten Berechtigungsbefund der aktiven GitHub-Integration (fehlender `workflow`-Scope) wurde die Datei autorisiert durch den Owner manuell über die Weboberfläche eingestellt und läuft in der CI.
2. Gepinnte Python-/Node-Umgebung: `ubuntu-24.04`, CPython 3.11.13, Node.js 22.20.0, Abhängigkeiten exakt aus `tools/architecture-validation/requirements.lock.txt` in einer repositoryexternen virtuellen Umgebung; eine spätere Unity-Umgebung folgt mit dem ersten Coding-Work-Package.
3. Der Architecture Validator läuft in der CI einschließlich Self-/Negativtests fail-closed.
4. Der commitgebundene PASS für den tatsächlichen WP-007-Scope ist nachgewiesen.
5. Der eindeutig benannte Check `Architecture Validation / validate` läuft auf Pull Requests gegen `main` und ist technisch als Required Merge Check verwendbar; die verbindliche Branch-Schutz-Konfiguration obliegt einer separaten Owner-Entscheidung.

## Offene Produktfolgeblocker

| ID | Status | Blockiert |
|---|---|---|
| `BLOCKER-PROD-001` | **Offen, fail-closed** | Produktvertrag für Hinweisanspruch, Lebensdauer, Moduswirkung und Rewarded-Hint-Economy. |
| `BLOCKER-PROD-002` | **Offen, fail-closed** | Kalendertag-, Zeitzonen-, Offline- und Manipulationspolicy für den Tagesanspruch. |
| `BLOCKER-PROD-003` | **Offen, fail-closed** | Kalibriertes, produktfreigegebenes Generator-Qualitätsprofil für die Dauerbaustelle. |

Diese Punkte blockieren Architecture v1.0 nicht, solange betroffene Features deaktiviert bleiben. Das autoritative Register ist `../ARCHITECTURE/OPEN_BLOCKERS.md`.

## Einstieg für die nächste Instanz

| Datei | Zweck |
|---|---|
| `../ARCHITECTURE/ARCHITECTURE.md` | Verbindlicher Einstieg und Systemübersicht für Architecture v1.0. |
| `../DECISIONS/README.md` | Aktueller ADR-Index, Status und Superseding-Regeln. |
| `../WORK_PACKAGES/WP-008_Unity-Scaffold-Modulgraph-und-CI-Basis.md` | WP-008: Unity-Scaffold, Modulgraph und CI-Basis; Trust-Anchor, Gate-A-/Gate-B-Stand, Befunde B-01/B-02 und Nachweise. |
| `../WORK_PACKAGES/WP-007_CI-Setup.md` | CI-Setup, WP-007-Trust-Anchor und commitgebundene CI-Nachweise. |
| `../WORK_PACKAGES/WP-011_PR-Scope-Checkout-Korrektur.md` | PR-Scope-Checkout-Korrektur, WP-011-Trust-Anchor und Abschlussnachweise. |
| `../WORK_PACKAGES/WP-010_Unity-Testassembly-Standort-Klarstellung.md` | B-01-Testpfad-Klarstellung, WP-010-Trust-Anchor und Abschlussnachweise. |
| `../WORK_PACKAGES/WP-006_Architecture-v1.0-Promotion.md` | Formale v1.0-Promotion, Trust-Anchor und Abschlussnachweise. |
| `../WORK_PACKAGES/WP-005_Architecture-v0.5-letzte-High-Korrekturen.md` | Vier HIGH-Korrekturen, WP-005-Trust-Anchor und Abschlussnachweise. |
| `../tools/architecture-validation/README.md` | Reproduzierbarer Scope-, Setup-, Evidenz-, Validator- und CI-Integrationsvertrag. |
| `../ARCHITECTURE/OPEN_BLOCKERS.md` | Drei offene Produktfolgeblocker. |
| `WORK_QUEUE.md` | Priorisierte nächste Produktionsblöcke und erfülltes CI-Gate. |

Eine neue Instanz beginnt erneut mit `AGENTS.md` und der dort vorgeschriebenen Lesereihenfolge. Chatkontext ersetzt keinen Repositoryzustand.
