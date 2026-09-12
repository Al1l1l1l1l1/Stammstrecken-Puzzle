# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und fachliche Spezifikationen sind verbindlich. **Architecture v0.4** ist dokumentiert und angenommen; Produktionscode existiert weiterhin nicht. |
| Derzeitige Phase | `WP-001` bis `WP-004` sind abgeschlossen. Die sieben Restbefunde `V03-001` bis `V03-007` wurden gezielt geschlossen, ohne eine neue Architekturphase oder Produktentscheidung einzuführen. |
| Letzter abgeschlossener Schritt | Architecture v0.4 wurde mit prä-SDK-Privacy-Fence, vollständigem Endless-Open-Lifecycle, semantisch gehärtetem Validator, kompatiblen Level-/Cosmetics-Schemata, verankertem Scope-Manifest, korrekter ADR-Historie und konkretem Store-Rolloutvertrag lokal abgenommen. |
| Nächster vorgesehener Schritt | Ein enger unabhängiger Architecture-v1.0-Freigabereview benötigt ein eigenes Work Package. Zusätzlich muss **vor dem ersten produktiven Coding-Work-Package** zwingend ein separates CI-Setup-Work-Package abgeschlossen sein. |
| Produktionscode | **Nicht vorhanden.** Das Repository enthält Konzept-, Architektur-, Governance-, Schema-, Fixture- und Validatorartefakte. |
| Aktuell gültige Architekturversion | **Architecture v0.4**, angenommen am 2026-09-12; Einstieg: `../ARCHITECTURE/ARCHITECTURE.md`. |
| Architekturentscheidungen | 26 ADRs: 17 angenommen und aktuell wirksam, 9 als ersetzte Historie erhalten. Autoritativer Index: `../DECISIONS/README.md`. |
| CI-Follow-up | **Nicht begonnen, non-blocking für v0.4, zwingend vor Produktionscoding.** Das CI-Setup-WP muss GitHub-Actions-Workflow, autorisierte Workflowberechtigungen, gepinnte Umgebung, Architecture Validator, Self-/Negativtests, commitgebundenen PASS und Pflichtcheck vor Merge liefern. |

## Verbindliche Grundlage

Die Konzeptdateien definieren unverändert den bestätigten Produktstand. Architecture v0.4 übersetzt ihn in technische Grenzen und dokumentiert fehlende Produktentscheidungen als fail-closed Folgeblocker. Lokale Struktur-, Semantik- und Scopebelege bleiben ausdrücklich von manuellen Dokumentreviews sowie späteren Unity-, Produktionscode-, Geräte-, SDK-, CI- und Storebelegen getrennt.

## Work-Package-Kette

`WP-001` dokumentiert die ursprüngliche technische Produktionsspezifikation. `WP-002` schloss zwölf Sol-Review-Findings und hob auf Architecture v0.2. `WP-003` adressierte acht Astra-Findings und hob auf Architecture v0.3. `WP-004` schloss ausschließlich die sieben verbliebenen Befunde `V03-001` bis `V03-007` und hebt den angenommenen Zwischenstand auf Architecture v0.4, ohne v1.0 freizugeben.

Der kanonische lokale Abnahmelauf lautet:

```bash
../.venv-stp-architecture/bin/python tools/architecture-validation/validate.py \
  --scope documentation \
  --scope-manifest tools/architecture-validation/scopes/WP-004.documentation.scope.json \
  --self-test
```

Ein Abschluss ist nur gültig, wenn Architecture-only- und Documentation-Scope-Lauf, Self-/Negativtests, `git diff --check`, Scope-/Secret-/Produktdateiprüfung, enger unabhängiger Delta-Review und Remote-Nachweis erfolgreich sind. Der GitHub-Actions-Nachweis ist in WP-004 **NON-BLOCKING WITH FOLLOW-UP / NOT EXECUTED**, weil die aktive GitHub-App Workflowdateien ohne `workflows`-Berechtigung nicht pushen darf. Unity-, Geräte-, SDK- und Storetests bleiben **REQUIRED_LATER/NOT_EXECUTED**, weil WP-004 keinen Produktionscode oder Unity-Scaffold erzeugen durfte.

## Zwingendes CI-Gate vor Produktionscoding

Das nächste produktive Coding-Work-Package darf erst angelegt beziehungsweise begonnen werden, wenn ein separates CI-Setup-Work-Package abgeschlossen ist. Dieses muss mindestens liefern:

1. einen GitHub-Actions-Workflow mit autorisierter Workflowberechtigung;
2. eine gepinnte Python-/Node-/spätere Unity-Umgebung;
3. Ausführung des Architecture Validators einschließlich Self-/Negativtests;
4. einen commitgebundenen PASS für den tatsächlichen Scope;
5. einen geschützten Pflichtcheck vor Merge.

Die fehlende Workflowberechtigung blockiert Architecture v0.4 und den engen v1.0-Freigabereview nicht, wohl aber den Beginn von Produktionscoding.

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
| `../ARCHITECTURE/ARCHITECTURE.md` | Verbindlicher Einstieg und Systemübersicht für Architecture v0.4. |
| `../DECISIONS/README.md` | Aktueller ADR-Index, Status und Superseding-Regeln. |
| `../WORK_PACKAGES/WP-004_Architecture-v0.4-Abschlusskorrekturen.md` | Scope, sieben Restbefunde und Abschlussnachweise. |
| `../tools/architecture-validation/README.md` | Reproduzierbarer Scope-, Setup-, Evidenz- und Validatorvertrag. |
| `../ARCHITECTURE/OPEN_BLOCKERS.md` | Drei offene Produktfolgeblocker. |
| `WORK_QUEUE.md` | Priorisierte nächste Produktionsblöcke und zwingendes CI-Gate. |

Eine neue Instanz beginnt erneut mit `AGENTS.md` und der dort vorgeschriebenen Lesereihenfolge. Chatkontext ersetzt keinen Repositoryzustand.
