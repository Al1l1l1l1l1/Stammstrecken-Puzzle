# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und **Architecture v1.0** sind verbindlich. `main` enthält die Governance-Entscheidung aus PR #6 und den WP-018-Closeout aus PR #7, aber keinen freigegebenen Produktionscode. |
| Derzeitige Phase | `WP-001` bis `WP-007` sind abgeschlossen. WP-014/WP-018 sind ebenfalls abgeschlossen. Eine nachträgliche Trust-Anchor-Korrektur ist in Prüfung: WP-019/WP-020 sind vor Implementierung zurückgezogene Planungsunterlagen, weil sie vor ihren eigenen Manifesten auf `main` lagen. |
| Historische Branches | `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern` und `feat/wp-013-level-v2-pipeline` sind archivierte, lesbare Vergleichskorpora. Keiner ist direkt integrierbar oder darf gerebased, gecherry-picked oder konfliktaufgelöst werden. |
| Letzter verifizierter Nachweis | PR #6 und PR #7 sind erfolgreich gemergt; deren Architecture-Validation-Actions waren erfolgreich. Die erneute Anwendung von ADR-030 hat danach die WP-019/WP-020-Pre-Anchor-Inkonsistenz vor jedem Produktionsstart fail-closed erkannt. |
| Aktuell zu prüfender Schritt | Diese Dokumentationskorrektur muss die IDs WP-019/WP-020 archivieren und die Nachfolgekette auf neue, künftig erst gemeinsam mit Manifest verankerte WP-021/WP-022 umstellen. Bis zu ihrem Merge gibt es keinen Implementierungsauftrag. |
| Nächster vorgesehener Schritt | Nach fehlerfreier Korrektur-QC: WP-021 auf einer neuen Main-basierten Branch mit **gleichzeitigem** WP-021-/Manifest-Trust-Anchor definieren. Erst bei nachgewiesener Unity-Lizenz sowie Linux/Android- und macOS/iOS-Runnern darf Kimi dessen Production-Scope ausführen. |
| Produktionscode | **Nicht freigegeben.** WP-019/WP-020 sind keine Lieferbranches; historische Produktbranches und Governance-Historie ersetzen weder einen eigenen Recovery-Anchor noch reale CI-Nachweise. |
| Architekturentscheidungen | Architecture v1.0; 31 ADRs, davon 22 angenommen und aktuell wirksam. ADR-031 bleibt ein verbindlicher Zielvertrag; dessen Proofschema-Implementierung liegt ausschließlich in WP-017 nach der künftigen Recovery-Kette. |
| Scope-Vertrauensanker | WP-014/WP-018 bleiben unverändert. Für WP-021 und WP-022 gilt ohne Ausnahme: das ausführbare Work Package und sein eigenes Production-Scope-Manifest müssen gemeinsam erstmals auf ihrer jeweiligen frischen Implementierungsbranch hinzugefügt werden. |
| CI-Follow-up | Der Architecture-Validation-Workflow bleibt bindend. Lokales Unity und `UNITY_LICENSE` sind in dieser Sandbox nicht verfügbar; der GitHub-App fehlen Leserechte auf Actions-Secrets/Variablen. Diese Lage ist **keine** Bestätigung der WP-021-Startvoraussetzungen und wird fail-closed behandelt. |

## Verbindliche Grundlage

Die Quellenhierarchie in [`AGENTS.md`](../AGENTS.md) gilt unverändert. Der spätere separate Manus-Chat ist ausdrücklich keine Architekturquelle. ADR-031 ergänzt ADR-007 und ADR-021 ausschließlich für die dort beschriebenen `solver-v2`-Zielmetriken, Proofregeneration und Strict-Semantik; bis WP-017 ersetzt sie keine gegenwärtige Schemaimplementierung.

## Verbindliche Integrations- und Umsetzungsreihenfolge

1. **Governance abgeschlossen:** WP-014/WP-018 wurden nach unabhängiger QC über [PR #6](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/6) integriert; der formale Closeout erfolgte über [PR #7](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/7).
2. **Trust-Anchor-Korrektur:** WP-019/WP-020 bleiben archivierte Planungen und werden nicht ausgeführt. Die Korrektur muss zuerst auf `main` integriert sein.
3. **Produktionsbasis:** WP-021 wird erst auf einer frischen Main-basierten Branch gemeinsam mit seinem neuen Production-Manifest verankert und nach allen eigenen CI-/QC-Nachweisen integriert.
4. **Puzzle-Kern:** WP-022 wird erst nach integriertem WP-021 auf einer neuen Branch gemeinsam mit eigenem Manifest verankert und umgesetzt.
5. **Content-/Proofkette:** Erst danach folgen WP-015 → WP-016 → WP-017 in harter Reihenfolge.
6. **Contentproduktion:** Erst nach WP-017 dürfen konkrete Season-1-Level, Generatorprofile, Zeitkalibrierung und weitere Produktionsblöcke beginnen.

## Offene Produktfolgeblocker

| ID | Status | Blockiert |
|---|---|---|
| `BLOCKER-PROD-001` | Offen, fail-closed | Hint-Entitlement / Rewarded-Hint-Economy. |
| `BLOCKER-PROD-002` | Offen, fail-closed | Kalendertag-, Zeitzonen-, Offline- und Manipulationspolicy für Tagesansprüche. |
| `BLOCKER-PROD-003` | Offen, fail-closed | Produktfreigegebenes Generator-Qualitätsprofil für Dauerbaustellenlevel. |

Diese Produktblocker bleiben unverändert. Zusätzlich ist die echte WP-021-Lizenz-/Runnerverfügbarkeit noch nicht nachgewiesen; sie ist ein operatives Start-/Abschlussgate, keine Änderung eines Produktblockers.

## Einstieg für die nächste Instanz

| Datei | Zweck |
|---|---|
| `../AGENTS.md` | Verbindliche Lesereihenfolge und Quellenhierarchie. |
| `WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md` | Quellenkorrektur sowie Abgrenzung historischer Branches. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Bindender `solver-v2`-Zielvertrag und künftige Recovery-Reihenfolge. |
| `../WORK_PACKAGES/WP-018_QC-Korrektur-und-Produktionsbasis-Reintegration.md` | Abgeschlossener Governance-Auftrag mit nachträglicher Trust-Anchor-Korrektur. |
| `../WORK_PACKAGES/WP-019_Unity-Scaffold-Recovery-auf-aktuellem-Main.md` | Archivierte, nicht ausführbare Planungsunterlage. |
| `../WORK_PACKAGES/WP-020_Puzzle-Kern-Recovery-auf-aktuellem-Main.md` | Archivierte, nicht ausführbare Planungsunterlage. |
| `../PROJECT_CONTROL/WORK_QUEUE.md` | Autoritative Priorisierung und Startgates. |

Eine neue Instanz beginnt erneut mit `AGENTS.md`; Chatkontext, historische Branches und vorab angelegte Planungsunterlagen ersetzen keinen aktuellen Work-Package-/Manifest-Trust-Anchor.
