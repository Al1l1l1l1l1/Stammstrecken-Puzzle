# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und fachliche Spezifikationen sind verbindlich. **Architecture v1.0** ist der freigegebene Architekturstand; der fachliche Puzzle-Kern ist mit `WP-009` als erster Produktionscode implementiert. |
| Derzeitige Phase | `WP-001` bis `WP-007` sind abgeschlossen. `WP-008` (Unity-6.3-Produktionsscaffold, Modulgraph und ausführbare CI-Basis) ist weitestgehend umgesetzt: Unity-Projektbasis (6000.3.23f1), vollständiger normativer `.asmdef`-Modulgraph mit Bootstrap-Composition, QA-Szene und lokaler Unity-Nachweis (Compile fehlerfrei, Bootstrap-PlayMode 8/8 PASS); offen bleiben ausschließlich die Buildnachweise (B-02: GitHub-Unity-Lizenz/Runner, Android, iOS) — sie blockieren die weitere Spieleentwicklung nicht. `WP-009` (Puzzle-Kern: Domain, Completion und deterministischer Solver) ist **abgeschlossen**: implementiert, vier Reviewbefunde behoben, gezielter Recheck erfolgreich, lokaler Unity-EditMode-Nachweis 65/65 grün; kein konkreter Codeblocker. `WP-013` (Level-v2-Parser, JCS-Hashverträge, proof-v1 und LevelValidationPipeline) ist als formaler Arbeitsauftrag **angelegt** (Trust-Anchor `e09658ba964b76d2a36b4baf07e69a2ccc26d952` auf `feat/wp-013-level-v2-pipeline`, CI-Scope-Lauf auf `WP-013.production.scope.json` umgeschaltet); Produktionscode von WP-013 ist noch nicht implementiert. |
| Letzter abgeschlossener Schritt | `WP-009` auf `feat/wp-009-puzzle-kern`: Puzzle-Domain (16 Dateien: Werte, validierte Definition, sechs objektive Diagnosen, exakte Completion, Command-/Session-Kern mit Undo/Revisionsmodell) und Constraint-Solver (4 Dateien, `solver-v1`: Propagation, MRV-DFS, Lösungslimit zwei, Rohmetriken) mit 64 eigenen EditMode-Testmethoden. Vier Reviewbefunde (Immutability, malformed Commands, `FromPorts(X,X)`, Summengleichheitstest) im Reviewfix-Commit `38c004c762754173d5c37518f4e3248228d8c0af` behoben und per gezieltem Recheck bestätigt. Lokaler Unity-Lauf (6000.3.23f1): 65/65 PASSED (64 eigene Tests plus 1 Addressables-Pakettest); wahrheitsgemäß getrennt davon hat der unabhängige Recheck den Code auf GitHub geprüft, ohne Unity erneut auszuführen. |
| Nächster vorgesehener Schritt | Umsetzung von `WP-013` (Level-v2-Parser, JCS-Hashverträge, proof-v1 und LevelValidationPipeline) auf `feat/wp-013-level-v2-pipeline`; der Arbeitsauftrag ist angelegt, Produktionscode ist noch nicht implementiert. Danach folgen Generator und Levelauthoring in späteren, noch zu vergebenden Work Packages. Owner-Aufgabe unverändert: GitHub-Unity-Lizenz/Runner konfigurieren (`secrets.UNITY_LICENSE`, `vars.UNITY_RUNNERS_READY=true`); die fehlenden CI-Buildnachweise (WP-008 B-02, GitHub-Unity-CI allgemein) sind kein Blocker für die weitere Spieleentwicklung. |
| Produktionscode | WP-008-Scaffold (`.asmdef`-Modulgraph, Ports, Adapter-, Präsentations- und Bootstrap-Composition ohne Featurelogik, Editor-Build-Entrypoints, QA-Szene) **und WP-009-Puzzle-Kern** (`STP.Puzzle.Domain` und `STP.Puzzle.Solver` als reine deterministische C#-Bibliotheken ohne Unity-/Datei-/SDK-Zugriff). Keine Economy-, Persistenz- oder sonstige Featurelogik; Hint-System weiterhin fail-closed (`BLOCKER-PROD-001`). |
| Aktuell gültige Architekturversion | **Architecture v1.0**, angenommen am 2026-09-13; Einstieg: `../ARCHITECTURE/ARCHITECTURE.md`. |
| Architekturentscheidungen | 30 ADRs: 21 angenommen und aktuell wirksam, 9 als ersetzte Historie erhalten. Autoritativer Index: `../DECISIONS/README.md`. |
| Scope-Vertrauensanker | `WP-006` und `WP-006.documentation.scope.json` wurden gemeinsam in Commit `3e8441830552a2d99c4546fcebc2dc1b23de58cf` eingeführt; der WP-005-Anker `ebf522a9ef3c28035341cc04dbd6d8251b107603` bleibt als historischer Nachweis bestehen. `WP-007` und `WP-007.documentation.scope.json` wurden gemeinsam im Trust-Anchor-Commit `280dbee4ea0134e18a21317ea87ee22aafd68852` auf `chore/ci-setup` eingeführt; der Validator liest beide historischen Blobs und ihre exakte Verknüpfung aus diesem Commit. `WP-008` und `WP-008.production.scope.json` wurden gemeinsam im Trust-Anchor-Commit `8d3e245fcab8bce42b5e8efdded480f47dacfbb8` auf `feat/wp-008-unity-scaffold` eingeführt (Elterncommit = `baseCommit` = `3c1a6988c1aab2084763edf772b6adc268874865`); danach ist das Manifest byteunveränderlich. `WP-009` und `WP-009.production.scope.json` wurden gemeinsam im Trust-Anchor-Commit `0047e329a4bbf3a04b5824ddf0d94a57db992b26` auf `feat/wp-009-puzzle-kern` eingeführt (Elterncommit = `baseCommit` = `edb713efc1580bcf0f8074dc7667c0f179a723ab`); danach ist das Manifest byteunveränderlich. `WP-013` und `WP-013.production.scope.json` wurden gemeinsam im Trust-Anchor-Commit `e09658ba964b76d2a36b4baf07e69a2ccc26d952` auf `feat/wp-013-level-v2-pipeline` eingeführt (Elterncommit = `baseCommit` = `767c01efb3720b3dae080280f58c568fbb36686f`); danach ist das Manifest byteunveränderlich. |
| CI-Follow-up | **Abgeschlossen (`WP-007`).** Das CI-Gate ist eingerichtet und commitgebunden nachgewiesen: autorisierter Workflow `../.github/workflows/validate.yml`, gepinnte Umgebung (`ubuntu-24.04`, CPython 3.11.13, Node.js 22.20.0, SHA-gepinnte Actions, `contents: read`), Architecture Validator mit Self-/Negativtests, commitgebundener PASS für den WP-007-Scope und der eindeutige Check `Architecture Validation / validate`, der als Required Merge Check verwendbar ist. |

## Verbindliche Grundlage

Die Konzeptdateien definieren unverändert den bestätigten Produktstand. Architecture v1.0 übersetzt ihn in technische Grenzen und dokumentiert fehlende Produktentscheidungen als fail-closed Folgeblocker. Lokale Struktur-, Semantik- und Scopebelege bleiben ausdrücklich von manuellen Dokumentreviews sowie späteren Unity-, Produktionscode-, Geräte-, SDK- und Storebelegen getrennt. Der CI-Nachweis des Architecture Validators ist seit WP-007 verbindlicher Bestandteil jedes Work-Package-Abschlusses.

## Work-Package-Kette

`WP-001` dokumentiert die ursprüngliche technische Produktionsspezifikation. `WP-002` schloss zwölf Sol-Review-Findings und hob auf Architecture v0.2. `WP-003` adressierte acht Astra-Findings und hob auf Architecture v0.3. `WP-004` schloss sieben verbliebene V03-Befunde und hob auf Architecture v0.4. `WP-005` schloss ausschließlich vier letzte HIGH-Lücken und hob den angenommenen Zwischenstand auf Architecture v0.5. `WP-006` promovierte den unabhängig freigegebenen v0.5-Stand rein formal auf Architecture v1.0, ohne eine technische Änderung. `WP-007` richtete das zwingende CI-Gate vor Produktionscoding ein und wies es commitgebunden auf `chore/ci-setup` nach, ohne Architekturänderung und ohne Merge. `WP-008` baute das Unity-6.3-Produktionsscaffold mit normativem Modulgraph, Bootstrap-Composition und lokaler Unity-Verifikation auf `feat/wp-008-unity-scaffold` auf; offen bleiben dort nur die CI-Buildnachweise (B-02). `WP-009` implementierte den fachlichen Puzzle-Kern (Domain, Completion und deterministischen Solver) auf `feat/wp-009-puzzle-kern`, behob vier Reviewbefunde und wies die EditMode-Tests lokal mit 65/65 PASS nach. `WP-013` wurde als formaler Arbeitsauftrag für die Leveldaten-Validierungskette (Level-v2-Parser, JCS-Hashverträge, proof-v1 und LevelValidationPipeline) auf `feat/wp-013-level-v2-pipeline` angelegt; seine Umsetzung steht noch aus.

Der kanonische lokale Abnahmelauf lautet:

```bash
../.venv-stp-architecture/bin/python tools/architecture-validation/validate.py \
  --scope documentation \
  --scope-manifest tools/architecture-validation/scopes/WP-007.documentation.scope.json \
  --self-test
```

Ein Abschluss ist nur gültig, wenn Architecture-only- und Documentation-Scope-Lauf, Self-/Negativtests, `git diff --check`, Scope-/Secret-/Produktdateiprüfung und Remote-Nachweis erfolgreich sind. Der GitHub-Actions-Nachweis des Checks `Architecture Validation / validate` ist seit WP-007 **verbindlich** und wurde in WP-007 positiv (Implementierungsstand und Abschlusscommit PASS) und negativ (absichtlich ungültiger Commit FAIL) ausgeführt. Unity-, Geräte-, SDK- und Storetests bleiben **REQUIRED_LATER/NOT_EXECUTED**, weil weiterhin kein Produktionscode oder Unity-Scaffold existiert.

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
| `../WORK_PACKAGES/WP-013_Level-v2-Parser-JCS-Hashvertraege-proof-v1-und-LevelValidationPipeline.md` | Angelegter Arbeitsauftrag: Leveldaten-Validierungskette (Level-v2-Parser, JCS-Hashverträge, proof-v1, LevelValidationPipeline); Umsetzung ausstehend. |
| `../WORK_PACKAGES/WP-009_Puzzle-Kern-Domain-Completion-und-Solver.md` | Abgeschlossenes Work Package: Puzzle-Kern (Domain, Completion, Solver), Reviewfix- und lokaler EditMode-Nachweis. |
| `../WORK_PACKAGES/WP-008_Unity-Scaffold-Modulgraph-und-CI-Basis.md` | Unity-Produktionsscaffold: umgesetzt mit lokalem Unity-Nachweis; offen nur CI-Buildnachweise (B-02). |
| `../WORK_PACKAGES/WP-007_CI-Setup.md` | CI-Setup, WP-007-Trust-Anchor und commitgebundene CI-Nachweise. |
| `../WORK_PACKAGES/WP-006_Architecture-v1.0-Promotion.md` | Formale v1.0-Promotion, Trust-Anchor und Abschlussnachweise. |
| `../WORK_PACKAGES/WP-005_Architecture-v0.5-letzte-High-Korrekturen.md` | Vier HIGH-Korrekturen, WP-005-Trust-Anchor und Abschlussnachweise. |
| `../tools/architecture-validation/README.md` | Reproduzierbarer Scope-, Setup-, Evidenz-, Validator- und CI-Integrationsvertrag. |
| `../ARCHITECTURE/OPEN_BLOCKERS.md` | Drei offene Produktfolgeblocker. |
| `WORK_QUEUE.md` | Priorisierte nächste Produktionsblöcke und erfülltes CI-Gate. |

Eine neue Instanz beginnt erneut mit `AGENTS.md` und der dort vorgeschriebenen Lesereihenfolge. Chatkontext ersetzt keinen Repositoryzustand.
