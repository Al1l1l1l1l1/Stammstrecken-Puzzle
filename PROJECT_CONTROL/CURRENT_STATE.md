# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und **Architecture v1.0** sind verbindlich. `main` enthält die Governance-Entscheidungen aus PR #6, PR #7, PR #8 und PR #10 sowie das freigegebene WP-021-Unity-Scaffold aus [PR #11](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/11). **WP-021 ist abgeschlossen und integriert.** |
| Derzeitige Phase | `WP-001` bis `WP-007` sind abgeschlossen; WP-014/WP-018 und WP-021 ebenfalls. Die unabhängige WP-021-Abschluss-QC war **PASS**; die Geschäftsführung gab die Integration frei. Vor der Integration bestand **kein technischer Restblocker**. WP-022 ist als nächster Produktionsschritt freigegeben, aber noch nicht definiert, verankert oder begonnen. |
| Historische Branches | `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern` und `feat/wp-013-level-v2-pipeline` bleiben archivierte, lesbare Vergleichskorpora; keiner ist direkt integrierbar oder darf gerebased, gecherry-picked oder konfliktaufgelöst werden. `chore/ci-readiness-unity` ist **kein** Produktions-Branch, sondern ein historischer technischer Befund zur CI-Readiness; auch aus ihm wurde nichts übernommen. |
| Letzter verifizierter Nachweis | GitHub bestätigt PR #11 als erfolgreich integriert am 2026-10-06: Merge-Commit `cd1a048fe7c77f971c00613de5687a0cac1ec395`, PR-Head `c93d4c192e55b26ae91db02d7e0c77b9ffd9f54b`; der Mergebaum ist identisch zum PR-Head. Dessen vier PR-/Push-Runs sind COMPLETED / SUCCESS: Architecture `37394173626`/`37394167950`, Unity `37394172281`/`37394167674`. AK-01–AK-08, echte Coverage, EditMode 1/1, PlayMode 27/27, Android-IL2CPP und iOS-Compile sind im WP-021 belegt. |
| Aktuell zu prüfender Schritt | Reiner formaler Dokumentations-Closeout auf `codex/wp-021-closeout`, abgezweigt vom verifizierten integrierten `main` (`cd1a048`). Ausschließlich WP-021, CURRENT_STATE und WORK_QUEUE werden nachgeführt; historische Implementierungsevidenz und der unveränderliche Scope-Trust-Anchor bleiben erhalten. Eigener PR gegen `main` zur Abnahme; kein WP-022-Start. |
| Nächster vorgesehener Schritt | **WP-022 ist durch die WP-021-Integration freigegeben.** Geschäftsführung / Projektarchitekt müssen WP-022 erst auf einer neuen Branch vom dann aktuellen `main` zusammen mit seinem eigenen Production-Scope-Manifest im selben ersten Add-Commit vollständig definieren und verankern (ADR-030). Erst danach ist eine separate Implementierungsbeauftragung zulässig. Dieser Closeout legt weder WP-022 noch dessen Manifest an. |
| Produktionscode | WP-021-Unity-Scaffold auf `main` freigegeben und integriert (Unity `6000.3.23f1`, 14 Produktions- und 9 Testassemblies, 15 Ports, fail-closed Composition, QA-Szene, Bootstrap-Smoke, Build-Entrypoints und CI; **keine** Puzzlefachlogik). Der fachliche Puzzle-Kern bleibt dem noch zu definierenden WP-022 vorbehalten. |
| Architekturentscheidungen | Architecture v1.0; 31 ADRs, davon 22 angenommen und aktuell wirksam. ADR-031 bleibt ein verbindlicher Zielvertrag; dessen Proofschema-Implementierung liegt ausschließlich in WP-017 nach der künftigen Recovery-Kette. |
| Scope-Vertrauensanker | WP-021 und `tools/architecture-validation/scopes/WP-021.production.scope.json` wurden gemeinsam im Trust-Anchor-Commit `fc10c61` eingeführt (Elterncommit = `baseCommit` = `e4f8cc1d0f0d0590fd7508992af464c0230ef314`); danach ist das Manifest byteunveränderlich. WP-014/WP-018 bleiben unverändert. Für WP-022 und alle Folgepakete gilt ohne Ausnahme: Work Package und eigenes Production-Scope-Manifest werden gemeinsam erstmals auf der jeweiligen frischen Implementierungsbranch hinzugefügt. |
| CI-Follow-up | `validate.yml` läuft mit `STP_SCOPE: production` und `STP_SCOPE_MANIFEST: tools/architecture-validation/scopes/WP-021.production.scope.json` (WP-011-PR-Head-Mechanik unverändert). `unity.yml` ist mit WP-021 neu eingerichtet: Preflight mit statischem Modulgraph-Check, fail-closed `unity-config`/`unity-evidence-guard`, drei Umgebungsjobs ohne Secrets sowie die serialisierten Lizenzjobs Compile, EditMode inkl. Coverage, Bootstrap-PlayMode-Smoke, Android-Development-IL2CPP und iOS-Export/Compile; Aktivierung der Unity-Personal-Lizenz per Unity Licensing Client mit `secrets.UNITY_EMAIL`/`secrets.UNITY_PASSWORD`; `vars.UNITY_RUNNERS_READY=true` ist gesetzt. |

## Verbindliche Grundlage

Die Quellenhierarchie in [`AGENTS.md`](../AGENTS.md) gilt unverändert. Der spätere separate Manus-Chat ist ausdrücklich keine Architekturquelle. ADR-031 ergänzt ADR-007 und ADR-021 ausschließlich für die dort beschriebenen `solver-v2`-Zielmetriken, Proofregeneration und Strict-Semantik; bis WP-017 ersetzt sie keine gegenwärtige Schemaimplementierung.

## Verbindliche Integrations- und Umsetzungsreihenfolge

1. **Governance abgeschlossen:** WP-014/WP-018 wurden nach unabhängiger QC über [PR #6](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/6) integriert; der formale Closeout erfolgte über [PR #7](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/7); die Trust-Anchor-Korrektur über [PR #8](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/8); die Hold-Policy über [PR #10](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/10).
2. **WP-021 abgeschlossen und integriert:** Trust Anchor `fc10c61` erhalten, AK-01–AK-08 und eigene CI-Nachweise belegt; unabhängige Abschluss-QC **PASS**, kein technischer Restblocker vor Integration, Geschäftsführungsfreigabe. [PR #11](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/11) ist als Merge-Commit `cd1a048fe7c77f971c00613de5687a0cac1ec395` auf `main` integriert.
3. **Puzzle-Kern als nächster Produktionsschritt freigegeben:** WP-022 muss vor jeder Umsetzung auf einer neuen Branch vom aktuellen `main` gemeinsam mit seinem eigenen Manifest erstmals definiert und verankert werden. WP-022 ist noch nicht begonnen; dieser Closeout ist keine Implementierungsbeauftragung.
4. **Content-/Proofkette:** Erst danach folgen WP-015 → WP-016 → WP-017 in harter Reihenfolge.
5. **Contentproduktion:** Erst nach WP-017 dürfen konkrete Season-1-Level, Generatorprofile, Zeitkalibrierung und weitere Produktionsblöcke beginnen.

## Offene Produktfolgeblocker

| ID | Status | Blockiert |
|---|---|---|
| `BLOCKER-PROD-001` | Offen, fail-closed | Hint-Entitlement / Rewarded-Hint-Economy. |
| `BLOCKER-PROD-002` | Offen, fail-closed | Kalendertag-, Zeitzonen-, Offline- und Manipulationspolicy für Tagesansprüche. |
| `BLOCKER-PROD-003` | Offen, fail-closed | Produktfreigegebenes Generator-Qualitätsprofil für Dauerbaustellenlevel. |

Diese Produktblocker bleiben unverändert und werden durch WP-021 nicht berührt.

## Einstieg für die nächste Instanz

| Datei | Zweck |
|---|---|
| `../AGENTS.md` | Verbindliche Lesereihenfolge und Quellenhierarchie. |
| `../WORK_PACKAGES/WP-021_Unity-Scaffold-Recovery.md` | Abgeschlossenes und integriertes Unity-Scaffold mit Trust Anchor, Akzeptanznachweisen, unabhängiger QC-Freigabe und formalem Closeout. |
| `WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md` | Quellenkorrektur sowie Abgrenzung historischer Branches. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Bindender `solver-v2`-Zielvertrag und künftige Recovery-Reihenfolge. |
| `../WORK_PACKAGES/WP-018_QC-Korrektur-und-Produktionsbasis-Reintegration.md` | Abgeschlossener Governance-Auftrag mit nachträglicher Trust-Anchor-Korrektur. |
| `../WORK_PACKAGES/WP-019_Unity-Scaffold-Recovery-auf-aktuellem-Main.md` | Archivierte, nicht ausführbare Planungsunterlage. |
| `../WORK_PACKAGES/WP-020_Puzzle-Kern-Recovery-auf-aktuellem-Main.md` | Archivierte, nicht ausführbare Planungsunterlage. |
| `../PROJECT_CONTROL/WORK_QUEUE.md` | Autoritative Priorisierung und Startgates. |

Eine neue Instanz beginnt erneut mit `AGENTS.md`; Chatkontext, historische Branches und vorab angelegte Planungsunterlagen ersetzen keinen aktuellen Work-Package-/Manifest-Trust-Anchor.
