# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und fachliche Spezifikationen sind verbindlich. **Architecture v0.3** ist dokumentiert und angenommen; Produktionscode existiert weiterhin nicht. |
| Derzeitige Phase | `WP-001`, `WP-002` und `WP-003` sind abgeschlossen. Die acht Astra-Finalreview-Findings sowie vier beim unabhängigen v0.3-Re-Review erkannte mittlere Konsistenzlücken wurden geschlossen. |
| Letzter abgeschlossener Schritt | Architecture v0.3 wurde mit eingecheckten Schemas, Goldens, Scope-Manifest und Mutations-Selbsttests commitgebunden lokal abgenommen. Lokale Belege bleiben ausdrücklich von späteren CI-, Unity-, Geräte-, SDK- und Storebelegen getrennt. |
| Nächster vorgesehener Schritt | Unabhängiger Architecture-v1.0-Freigabereview in einem neuen Work Package. Erst dessen ausdrückliche Entscheidung darf v1.0 ausrufen. |
| Produktionscode | **Nicht vorhanden.** Das Repository enthält Konzept-, Architektur-, Governance-, Schema-, Fixture- und Validatorartefakte. |
| Aktuell gültige Architekturversion | **Architecture v0.3**, angenommen am 2026-09-12; Einstieg: `../ARCHITECTURE/ARCHITECTURE.md`. |
| Architekturentscheidungen | 23 ADRs: 15 angenommen und aktuell wirksam, 8 als ersetzte Historie erhalten. Autoritativer Index: `../DECISIONS/README.md`. |

## Verbindliche Grundlage

Die Konzeptdateien definieren unverändert den bestätigten Produktstand. Architecture v0.3 übersetzt ihn in technische Grenzen und dokumentiert fehlende Produktentscheidungen als fail-closed Folgeblocker. Sie trennt ausdrücklich lokale Contractbelege von späteren Unity-, Geräte-, SDK- und Storebelegen.

## Work-Package-Kette

`WP-001` dokumentiert die ursprüngliche technische Produktionsspezifikation. `WP-002` schloss zwölf Sol-Review-Findings und hob auf Architecture v0.2. `WP-003` adressiert die acht Astra-Findings und hebt den angenommenen Zwischenstand auf Architecture v0.3, ohne v1.0 freizugeben.

Der kanonische lokale Abnahmelauf lautet:

```bash
../.venv-stp-architecture/bin/python tools/architecture-validation/validate.py \
  --scope documentation \
  --scope-manifest tools/architecture-validation/scopes/WP-003.documentation.scope.json \
  --self-test
```

Ein Abschluss ist nur gültig, wenn dieser Lauf, `git diff --check`, Scope-/Secret-/Produktdateiprüfung, unabhängiger Review und Remote-Nachweis erfolgreich sind. Der bevorzugte GitHub-Actions-Nachweis ist **BLOCKED/NOT EXECUTED**, weil die aktive GitHub-App Workflowdateien ohne `workflows`-Berechtigung nicht pushen darf. Unity-, Geräte-, SDK- und Storetests bleiben **REQUIRED_LATER/NOT_EXECUTED**, weil WP-003 keinen Produktionscode oder Unity-Scaffold erzeugen durfte.

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
| `../ARCHITECTURE/ARCHITECTURE.md` | Verbindlicher Einstieg und Systemübersicht für Architecture v0.3. |
| `../DECISIONS/README.md` | Aktueller ADR-Index, Status und Superseding-Regeln. |
| `../WORK_PACKAGES/WP-003_Architecture-v0.3-Finalkorrekturen.md` | Scope, acht Astra-Findings und Abschlussnachweise. |
| `../tools/architecture-validation/README.md` | Reproduzierbarer Scope-, Setup- und Validatorvertrag. |
| `../ARCHITECTURE/OPEN_BLOCKERS.md` | Drei offene Produktfolgeblocker. |
| `WORK_QUEUE.md` | Priorisierte nächste Produktionsblöcke. |

Eine neue Instanz beginnt erneut mit `AGENTS.md` und der dort vorgeschriebenen Lesereihenfolge. Chatkontext ersetzt keinen Repositoryzustand.
