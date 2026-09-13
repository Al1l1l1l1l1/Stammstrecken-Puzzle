# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und fachliche Spezifikationen sind verbindlich. **Architecture v1.0** ist der freigegebene Architekturstand; Produktionscode existiert weiterhin nicht. |
| Derzeitige Phase | `WP-001` bis `WP-006` sind abgeschlossen. WP-006 führte ausschließlich die formale administrative Promotion des unabhängig freigegebenen Architecture-v0.5-Stands auf Architecture v1.0 durch, ohne Architekturänderung, neue Produktentscheidung oder neue Reviewrunde. |
| Letzter abgeschlossener Schritt | Der unabhängige Abschlussreview von Architecture v0.5 (Commit `79f64d7191672175ede1153c9458be2207ce3c62`) ist bestanden: `HIGH-1` bis `HIGH-4` CLOSED, 0 neue BLOCKER, 0 neue HIGH, relevante Acceptance-/Validator-Tests PASS. Architecture v1.0 wurde formal promoviert und dokumentiert. |
| Nächster vorgesehener Schritt | **Vor dem ersten produktiven Coding-Work-Package** muss zwingend ein separates CI-Setup-Work-Package abgeschlossen sein. |
| Produktionscode | **Nicht vorhanden.** Das Repository enthält Konzept-, Architektur-, Governance-, Schema-, Fixture- und Validatorartefakte. |
| Aktuell gültige Architekturversion | **Architecture v1.0**, angenommen am 2026-09-13; Einstieg: `../ARCHITECTURE/ARCHITECTURE.md`. |
| Architekturentscheidungen | 30 ADRs: 21 angenommen und aktuell wirksam, 9 als ersetzte Historie erhalten. Autoritativer Index: `../DECISIONS/README.md`. |
| Scope-Vertrauensanker | `WP-006` und `WP-006.documentation.scope.json` wurden gemeinsam in Commit `3e8441830552a2d99c4546fcebc2dc1b23de58cf` eingeführt; der Validator liest beide historischen Blobs und ihre exakte Verknüpfung aus diesem Commit. Der WP-005-Anker `ebf522a9ef3c28035341cc04dbd6d8251b107603` bleibt als historischer Nachweis bestehen. |
| CI-Follow-up | **Nicht begonnen, non-blocking für v1.0, zwingend vor Produktionscoding.** Das CI-Setup-WP muss GitHub-Actions-Workflow, autorisierte Workflowberechtigungen, gepinnte Umgebung, Architecture Validator, Self-/Negativtests, commitgebundenen PASS und Pflichtcheck vor Merge liefern. |

## Verbindliche Grundlage

Die Konzeptdateien definieren unverändert den bestätigten Produktstand. Architecture v1.0 übersetzt ihn in technische Grenzen und dokumentiert fehlende Produktentscheidungen als fail-closed Folgeblocker. Lokale Struktur-, Semantik- und Scopebelege bleiben ausdrücklich von manuellen Dokumentreviews sowie späteren Unity-, Produktionscode-, Geräte-, SDK-, CI- und Storebelegen getrennt.

## Work-Package-Kette

`WP-001` dokumentiert die ursprüngliche technische Produktionsspezifikation. `WP-002` schloss zwölf Sol-Review-Findings und hob auf Architecture v0.2. `WP-003` adressierte acht Astra-Findings und hob auf Architecture v0.3. `WP-004` schloss sieben verbliebene V03-Befunde und hob auf Architecture v0.4. `WP-005` schloss ausschließlich vier letzte HIGH-Lücken und hob den angenommenen Zwischenstand auf Architecture v0.5. `WP-006` promovierte den unabhängig freigegebenen v0.5-Stand rein formal auf Architecture v1.0, ohne eine technische Änderung.

Der kanonische lokale Abnahmelauf lautet:

```bash
../.venv-stp-architecture/bin/python tools/architecture-validation/validate.py \
  --scope documentation \
  --scope-manifest tools/architecture-validation/scopes/WP-006.documentation.scope.json \
  --self-test
```

Ein Abschluss ist nur gültig, wenn Architecture-only- und Documentation-Scope-Lauf, Self-/Negativtests, `git diff --check`, Scope-/Secret-/Produktdateiprüfung und Remote-Nachweis erfolgreich sind. Der GitHub-Actions-Nachweis ist in WP-006 **NON-BLOCKING WITH FOLLOW-UP / NOT EXECUTED**, weil die aktive GitHub-App Workflowdateien ohne `workflows`-Berechtigung nicht pushen darf. Unity-, Geräte-, SDK- und Storetests bleiben **REQUIRED_LATER/NOT_EXECUTED**, weil WP-006 keinen Produktionscode oder Unity-Scaffold erzeugen durfte.

## Zwingendes CI-Gate vor Produktionscoding

Das nächste produktive Coding-Work-Package darf erst angelegt beziehungsweise begonnen werden, wenn ein separates CI-Setup-Work-Package abgeschlossen ist. Dieses muss mindestens liefern:

1. einen GitHub-Actions-Workflow mit autorisierter Workflowberechtigung;
2. eine gepinnte Python-/Node-/spätere Unity-Umgebung;
3. Ausführung des Architecture Validators einschließlich Self-/Negativtests;
4. einen commitgebundenen PASS für den tatsächlichen Scope;
5. einen geschützten Pflichtcheck vor Merge.

Die fehlende Workflowberechtigung blockiert Architecture v1.0 nicht, wohl aber den Beginn von Produktionscoding.

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
| `../WORK_PACKAGES/WP-006_Architecture-v1.0-Promotion.md` | Formale v1.0-Promotion, Trust-Anchor und Abschlussnachweise. |
| `../WORK_PACKAGES/WP-005_Architecture-v0.5-letzte-High-Korrekturen.md` | Vier HIGH-Korrekturen, WP-005-Trust-Anchor und Abschlussnachweise. |
| `../tools/architecture-validation/README.md` | Reproduzierbarer Scope-, Setup-, Evidenz- und Validatorvertrag. |
| `../ARCHITECTURE/OPEN_BLOCKERS.md` | Drei offene Produktfolgeblocker. |
| `WORK_QUEUE.md` | Priorisierte nächste Produktionsblöcke und zwingendes CI-Gate. |

Eine neue Instanz beginnt erneut mit `AGENTS.md` und der dort vorgeschriebenen Lesereihenfolge. Chatkontext ersetzt keinen Repositoryzustand.
