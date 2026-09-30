# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und **Architecture v1.0** sind verbindlich. `main` enthält die freigegebene Governance-Entscheidung WP-014/ADR-031/WP-018 aus PR #6, aber weiterhin keinen freigegebenen Produktionscode. |
| Derzeitige Phase | `WP-001` bis `WP-007` sind abgeschlossen. Die Governance-Packages WP-014 und WP-018 sind ebenfalls abgeschlossen. Produktionsimplementierung hat nicht begonnen; WP-019 ist der nächste kontrollierte Auftrag. |
| Historische Branches | `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern` und `feat/wp-013-level-v2-pipeline` sind archivierte, lesbare Vergleichskorpora. Keiner ist direkt integrierbar oder darf gerebased, gecherry-picked oder konfliktaufgelöst werden. |
| Letzter verifizierter Nachweis | WP-018-Korrektur-Head `7b65c0c…`: Validator mit 18 lokalen Prüfgruppen PASS, beide unabhängigen QC-Berichte PASS ohne BLOCKER/HIGH und GitHub-Actions für Push/PR SUCCESS. PR #6 wurde als Merge-Commit `7b0d65a…` integriert. |
| Aktuell zu prüfender Schritt | Vor WP-019-Start die reale Verfügbarkeit von `UNITY_LICENSE` sowie Linux/Android- und macOS/iOS-Runnern gemäß WP-019 prüfen. Fehlt einer dieser Nachweise, ist WP-019 fail-closed blockiert. |
| Nächster vorgesehener Schritt | Bei erfüllten WP-019-Voraussetzungen: Kimi ausschließlich mit WP-019 auf einer neuen Main-basierten Production-Branch beauftragen; danach Astra/Sol-QC. WP-020 → WP-015 → WP-016 → WP-017 bleiben harte Folgegates. |
| Produktionscode | **Noch nicht freigegeben.** WP-019 ist zur Implementierungsbeauftragung vorbereitet, aber historische Produktbranches und die Governance-Historie sind kein Ersatz für dessen eigene Recovery- und CI-Nachweise. |
| Architekturentscheidungen | Architecture v1.0; 31 ADRs, davon 22 angenommen und aktuell wirksam. ADR-031 ist ein verbindlicher Zielvertrag, dessen Proofschema-Implementierung erst WP-017 herstellt. |
| Scope-Vertrauensanker | WP-014 und WP-018 besitzen jeweils eigenen dokumentationsweiten Trust Anchor. Jede Produktionsrecovery erhält einen neuen Production-Anchor auf ihrer frischen Main-basierten Branch. |
| CI-Follow-up | Der Architecture-Validation-Workflow bleibt bindend. Für WP-019 und Folgepakete sind reale Unity-/CI-Nachweise sowie Astra-/Sol-QC Pflicht; fehlende Lizenz/Runner bleiben Blocker. |

## Verbindliche Grundlage

Die Quellenhierarchie in [`AGENTS.md`](../AGENTS.md) gilt unverändert. Der spätere separate Manus-Chat ist ausdrücklich keine Architekturquelle. ADR-031 ergänzt ADR-007 und ADR-021 ausschließlich für die dort beschriebenen `solver-v2`-Zielmetriken, die Proofregeneration und Strict-Semantik; bis WP-017 ersetzt sie keine gegenwärtige Schemaimplementierung.

## Verbindliche Integrations- und Umsetzungsreihenfolge

1. **Governance-QC:** WP-018 hat die HIGH-Befunde geschlossen; beide unabhängigen PASS-Berichte liegen vor und [PR #6](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/6) ist nach `main` integriert.
2. **Produktionsbasis:** WP-019 rekonstruiert das Unity-Scaffold auf einer neuen Main-basierten Branch mit neuem Trust Anchor und realen CI-Nachweisen. Der historische WP-008-Branch wird nicht verwendet.
3. **Puzzle-Kern:** WP-020 rekonstruiert Domain/Solver auf einer neuen, nach WP-019 integrierten Main-Branch. Der historische WP-009-Branch wird nicht verwendet.
4. **Content-/Proofkette:** Erst danach folgen in harter Reihenfolge WP-015 → WP-016 → WP-017.
5. **Contentproduktion:** Erst nach WP-017 dürfen konkrete Season-1-Level, Generatorprofile, Zeitkalibrierung und weitere Produktionsblöcke geplant oder begonnen werden.

## Offene Produktfolgeblocker

| ID | Status | Blockiert |
|---|---|---|
| `BLOCKER-PROD-001` | Offen, fail-closed | Hint-Entitlement / Rewarded-Hint-Economy. |
| `BLOCKER-PROD-002` | Offen, fail-closed | Kalendertag-, Zeitzonen-, Offline- und Manipulationspolicy für Tagesansprüche. |
| `BLOCKER-PROD-003` | Offen, fail-closed | Produktfreigegebenes Generator-Qualitätsprofil für Dauerbaustellenlevel. |

Diese Blocker bleiben unverändert. Die zusätzliche Unity-Lizenz-/Runnerverfügbarkeit ist ein operativer Abschlussblocker für WP-019, jedoch keine Änderung dieser Produktblocker.

## Einstieg für die nächste Instanz

| Datei | Zweck |
|---|---|
| `../AGENTS.md` | Verbindliche Lesereihenfolge und Quellenhierarchie. |
| `WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md` | Quellenkorrektur sowie Abgrenzung aller historischen Prototypbranches. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Bindender `solver-v2`-Zielvertrag und Übergang bis WP-017. |
| `../WORK_PACKAGES/WP-018_QC-Korrektur-und-Produktionsbasis-Reintegration.md` | Abgeschlossener Governance-Korrekturauftrag und Abschlussnachweis. |
| `../WORK_PACKAGES/WP-019_Unity-Scaffold-Recovery-auf-aktuellem-Main.md` | Erster Main-basierter Produktionsrecovery-Auftrag. |
| `../WORK_PACKAGES/WP-020_Puzzle-Kern-Recovery-auf-aktuellem-Main.md` | Zweiter Main-basierter Produktionsrecovery-Auftrag. |
| `../WORK_PACKAGES/WP-015_Level-v2-Foundation-und-Hashvertraege.md` | Folgender Level-/Hashauftrag nach WP-020. |
| `../PROJECT_CONTROL/WORK_QUEUE.md` | Priorisierte, harte Reihenfolge. |

Eine neue Instanz beginnt erneut mit `AGENTS.md`; Chatkontext und historische Branches ersetzen keinen aktuellen Repository- und Evidenzstand.
