# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Die strategische Produktkonzeption, fachlichen Spezifikationen und **Architecture v0.2** sind dokumentiert und verbindlich. Die modellunabhängige Projektsteuerung ist eingerichtet. Produktionscode existiert weiterhin nicht. |
| Derzeitige Phase | Die technische Produktionsspezifikation `WP-001` und die Sol-Review-Korrekturrunde `WP-002` sind abgeschlossen. Architecture v0.2 ist bereit für den ausdrücklich vorgesehenen unabhängigen Astra-Finalreview. |
| Letzter abgeschlossener Schritt | Alle zwölf Sol-Review-Findings sowie zusätzliche unabhängige Befunde zu I-JSON, nativer Privacy, Unlockgraph und endlichem Endless-State wurden geschlossen; der finale unabhängige Re-Review meldet keine offenen Befunde ab MEDIUM. |
| Nächster vorgesehener Schritt | Unabhängigen Astra-Finalreview auf dem gepushten Branch durchführen. Erst nach dessen erfolgreicher Abnahme darf Architecture v1.0 durch ein eigenes Work Package freigegeben werden. |
| Produktionscode | **Nicht vorhanden.** Dieses Repository enthält weiterhin Konzept-, Architektur-, Governance-, Schema-, Fixture- und Entwicklungstooling-Artefakte. |
| Aktuell gültige Architekturversion | **Architecture v0.2**, angenommen am 2026-09-08; Einstieg: `../ARCHITECTURE/ARCHITECTURE.md`. |
| Architekturentscheidungen | 17 ADRs: 13 angenommen und aktuell wirksam, 4 als ersetzte Historie erhalten. Autoritativer Index: `../DECISIONS/README.md`. |

## Verbindliche Grundlage

Die bestehenden Konzeptdateien definieren unverändert den bestätigten Produktstand. Architecture v0.2 übersetzt ihn in technische Grenzen und dokumentiert fehlende Produktentscheidungen ausdrücklich als Folgeblocker, statt Werte zu erfinden. Änderungen an Engine-Linie, Plattformbaseline, Datenformat, Modulgrenzen, Persistenzwahrheit, externen Providern oder den neuen Transaktions-/Katalogverträgen benötigen ein neues beziehungsweise ersetzendes ADR und ein freigegebenes Work Package.

## Abschlussnachweis WP-001 und WP-002

`WP-001` dokumentiert die ursprüngliche technische Produktionsspezifikation. `WP-002` korrigiert die zwölf Findings des unabhängigen Sol-Reviews und hebt den aktuellen Stand auf Architecture v0.2. Das frühere nicht regelkonforme Work-Package-ID-Format wurde ohne Sonderregel auf das einheitliche Schema `WP-###` migriert.

Der reproduzierbare Abnahmelauf liegt vollständig unter `../tools/architecture-validation/` und wird aus der Repositorywurzel nach dem dort dokumentierten Setup mit folgendem Befehl ausgeführt:

```bash
../.venv-stp-architecture/bin/python tools/architecture-validation/validate.py --self-test
```

Ein Abschluss ist nur gültig, wenn dieser Lauf, `git diff --check`, die Diff-/Produktdateiprüfung und der Remote-Branch-Nachweis erfolgreich sind. Spielbuild und Laufzeittests bleiben nicht anwendbar, weil dieses Work Package ausdrücklich keinen Produktionscode oder Unity-Scaffold erzeugt.

## Offene Produktfolgeblocker

| ID | Status | Blockiert |
|---|---|---|
| `BLOCKER-PROD-001` | **Offen, fail-closed** | Produktvertrag für Hinweisanspruch, Lebensdauer, Moduswirkung und Rewarded-Hint-Economy. |
| `BLOCKER-PROD-002` | **Offen, fail-closed** | Kalendertag-, Zeitzonen-, Offline- und Manipulationspolicy für den Tagesanspruch. |
| `BLOCKER-PROD-003` | **Offen, fail-closed** | Kalibriertes, produktfreigegebenes Generator-Qualitätsprofil für die Dauerbaustelle. |

Diese Punkte blockieren Architecture v1.0 **nicht**. Sie blockieren erst die jeweils betroffenen späteren Feature- oder Release-Work-Packages. Das autoritative Register ist `../ARCHITECTURE/OPEN_BLOCKERS.md`.

## Einstieg für die nächste Instanz

| Datei | Zweck |
|---|---|
| `../ARCHITECTURE/ARCHITECTURE.md` | Verbindlicher Einstieg und Systemübersicht für Architecture v0.2. |
| `../DECISIONS/README.md` | Aktueller ADR-Index, Status und Superseding-Regeln. |
| `../WORK_PACKAGES/WP-001_Technische_Produktionsspezifikation.md` | Historischer Scope und Abschluss der ursprünglichen Architekturarbeit. |
| `../WORK_PACKAGES/WP-002_Architecture-v0.2-Korrekturen.md` | Scope, zwölf Reviewbefunde, Akzeptanzkriterien und Abschlussnachweise der v0.2-Korrekturrunde. |
| `../tools/architecture-validation/README.md` | Reproduzierbarer Setup- und Validatorvertrag. |
| `../ARCHITECTURE/OPEN_BLOCKERS.md` | Drei offene, nicht eigenmächtig zu lösende Produktfolgeblocker. |
| `WORK_QUEUE.md` | Priorisierte Reihenfolge der nächsten Produktionsblöcke. |

Eine neue Instanz beginnt erneut mit `AGENTS.md` und der dort vorgeschriebenen Lesereihenfolge. Chatkontext ersetzt keinen Repositoryzustand.
