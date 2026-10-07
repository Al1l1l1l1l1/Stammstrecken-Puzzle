# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und **Architecture v1.0** sind verbindlich. `main` enthält die Governance-Entscheidungen aus PR #6, PR #7, PR #8 und PR #10 sowie das freigegebene WP-021-Unity-Scaffold aus [PR #11](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/11). **WP-021 ist abgeschlossen und integriert.** |
| Derzeitige Phase | `WP-001` bis `WP-007` sind abgeschlossen; WP-014/WP-018 und WP-021 ebenfalls. Die unabhängige WP-021-Abschluss-QC war **PASS**; die Geschäftsführung gab die Integration frei. Vor der Integration bestand **kein technischer Restblocker**. WP-022 ist vollständig definiert und gemeinsam mit eigenem Manifest auf `codex/wp-022-puzzle-kern-recovery` verankert; **Implementierung nicht begonnen**, separate Implementierungsbeauftragung ausstehend. |
| Historische Branches | `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern` und `feat/wp-013-level-v2-pipeline` bleiben archivierte, lesbare Vergleichskorpora; keiner ist direkt integrierbar oder darf gerebased, gecherry-picked oder konfliktaufgelöst werden. `chore/ci-readiness-unity` ist **kein** Produktions-Branch, sondern ein historischer technischer Befund zur CI-Readiness; auch aus ihm wurde nichts übernommen. |
| Letzter verifizierter Nachweis | GitHub bestätigt PR #11 als erfolgreich integriert am 2026-10-06: Merge-Commit `cd1a048fe7c77f971c00613de5687a0cac1ec395`, PR-Head `c93d4c192e55b26ae91db02d7e0c77b9ffd9f54b`; der Mergebaum ist identisch zum PR-Head. Dessen vier PR-/Push-Runs sind COMPLETED / SUCCESS: Architecture `37394173626`/`37394167950`, Unity `37394172281`/`37394167674`. AK-01–AK-08, echte Coverage, EditMode 1/1, PlayMode 27/27, Android-IL2CPP und iOS-Compile sind im WP-021 belegt. |
| Aktuell zu prüfender Schritt | WP-021-Closeout ist über [PR #12](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/12) am 2026-10-07 integriert. Tatsächlicher frisch verifizierter `main`-HEAD/Branchbasis: `8c0d73ac72c01c8bc079a69a00ccf4e5b759c1aa`. WP-022-Definitions-/Verankerungsschritt (Phase A) umfasst exakt WP, eigenes Manifest, CURRENT_STATE, WORK_QUEUE und die Manifestzeile in validate.yml; kein Produktions-/Test-C# oder Unity-CI-Diff. Governance-/Scope-/Trust-Nachweise werden im WP-022 fortgeschrieben. |
| Nächster vorgesehener Schritt | Zuerst **BLOCKER-WP022-REMOTE-001**: GitHub weist Git-/Blobübertragungen mit internem Serverfehler ab. Remote-Branch steht noch auf Basis 8c0d73a; Anker ed89889/Folgecommit fe0a3cc sind nur lokal. Letzten lokalen Evidence-HEAD normal pushen, Remote-Anker/HEAD prüfen, Definitions-PR und gepinnte Ubuntu-CI nachweisen. Danach separate Implementierungsbeauftragung für [WP-022](../WORK_PACKAGES/WP-022_Puzzle-Kern-Recovery.md), Phase B. Kein Definitionsblocker, aber Phase A formal noch offen; keine Implementierung/QC/Integration oder Folgefreigabe. |
| Produktionscode | WP-021-Unity-Scaffold auf `main` freigegeben und integriert (Unity `6000.3.23f1`, 14 Produktions- und 9 Testassemblies, 15 Ports, fail-closed Composition, QA-Szene, Bootstrap-Smoke, Build-Entrypoints und CI; **keine** Puzzlefachlogik). WP-022 ist jetzt definiert/verankert; der fachliche Puzzle-Kern ist weiterhin nicht implementiert und bleibt seiner separat zu beauftragenden Phase B vorbehalten. |
| Architekturentscheidungen | Architecture v1.0; 31 ADRs, davon 22 angenommen und aktuell wirksam. ADR-031 bleibt ein verbindlicher Zielvertrag; dessen Proofschema-Implementierung liegt ausschließlich in WP-017 nach der künftigen Recovery-Kette. |
| Scope-Vertrauensanker | WP-022 und [eigenes Manifest](../tools/architecture-validation/scopes/WP-022.production.scope.json) wurden gemeinsam erstmals im ersten Branchcommit `ed898894573f2bb3456d21f22a8522c6b8825621` als A eingeführt; einziger Elterncommit = baseCommit = `8c0d73ac72c01c8bc079a69a00ccf4e5b759c1aa`. Manifest ab diesem Anker byteunveränderlich (ADR-030). Bestehende WP-021-/WP-014-/WP-018-Anker und Manifeste bleiben unverändert; historische WP-021-Basis `e4f8cc1` und Anker `fc10c61` bleiben erhalten. |
| CI-Follow-up | Lokaler positiver WP-022-Scope/Trust PASS (17 Prüfgruppen); Windows-Selftests nur bekannter V03-005-ABSOLUTE FAIL. Vorhandene Ubuntu-WSL-Diagnose auf fe0a3cc: Architecture-only 17 und Scope 18 Prüfgruppen inkl. Selftests PASS, wegen abweichender Runtime-/Paketversionen kein gepinnter CI-Abschlussbeleg. Eigene Remote-/CI-Nachweise extern BLOCKED/NOT_EXECUTED. validate.yml lokal ausschließlich auf WP-022-Manifest umgeschaltet; STP_SCOPE/PR-Head-Mechanik und unity.yml unverändert. Details und Fortsetzung im WP-022. |

## Verbindliche Grundlage

Die Quellenhierarchie in [`AGENTS.md`](../AGENTS.md) gilt unverändert. Der spätere separate Manus-Chat ist ausdrücklich keine Architekturquelle. ADR-031 ergänzt ADR-007 und ADR-021 ausschließlich für die dort beschriebenen `solver-v2`-Zielmetriken, Proofregeneration und Strict-Semantik; bis WP-017 ersetzt sie keine gegenwärtige Schemaimplementierung.

## Verbindliche Integrations- und Umsetzungsreihenfolge

1. **Governance abgeschlossen:** WP-014/WP-018 wurden nach unabhängiger QC über [PR #6](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/6) integriert; der formale Closeout erfolgte über [PR #7](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/7); die Trust-Anchor-Korrektur über [PR #8](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/8); die Hold-Policy über [PR #10](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/10).
2. **WP-021 abgeschlossen und integriert:** Trust Anchor `fc10c61` erhalten, AK-01–AK-08 und eigene CI-Nachweise belegt; unabhängige Abschluss-QC **PASS**, kein technischer Restblocker vor Integration, Geschäftsführungsfreigabe. [PR #11](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/11) ist als Merge-Commit `cd1a048fe7c77f971c00613de5687a0cac1ec395` auf `main` integriert.
3. **Puzzle-Kern definiert und lokal verankert:** WP-022/eigenes Manifest erstmals gemeinsam im Anker `ed89889` auf frischer Branch von `main` `8c0d73a` nach integriertem WP-021-Closeout (PR #12). Remote-Sicherung/gepinnte CI noch extern blockiert (BLOCKER-WP022-REMOTE-001). Phase A ist ausschließlich Definition/Verankerung; erst ihr vollständiger Nachweis erlaubt die separate Phase-B-Beauftragung. WP-022 ist nicht implementiert oder integriert; WP-015 bleibt bis zur Integration gesperrt.
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
| `../WORK_PACKAGES/WP-022_Puzzle-Kern-Recovery.md` | Vollständiger aktueller Recovery-Auftrag, enge Phase-A-/Phase-B-Grenzen, eigener Trust Anchor und tatsächliche Verankerungsnachweise; Implementierung separat zu beauftragen. |
| `../tools/architecture-validation/scopes/WP-022.production.scope.json` | Eigenes unveränderliches Production-Scope-Manifest ab gemeinsamem Add-Anker ed89889, Basis 8c0d73a. |
| `WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md` | Quellenkorrektur sowie Abgrenzung historischer Branches. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Bindender `solver-v2`-Zielvertrag und künftige Recovery-Reihenfolge. |
| `../WORK_PACKAGES/WP-018_QC-Korrektur-und-Produktionsbasis-Reintegration.md` | Abgeschlossener Governance-Auftrag mit nachträglicher Trust-Anchor-Korrektur. |
| `../WORK_PACKAGES/WP-019_Unity-Scaffold-Recovery-auf-aktuellem-Main.md` | Archivierte, nicht ausführbare Planungsunterlage. |
| `../WORK_PACKAGES/WP-020_Puzzle-Kern-Recovery-auf-aktuellem-Main.md` | Archivierte, nicht ausführbare Planungsunterlage. |
| `../PROJECT_CONTROL/WORK_QUEUE.md` | Autoritative Priorisierung und Startgates. |

Eine neue Instanz beginnt erneut mit `AGENTS.md`; Chatkontext, historische Branches und vorab angelegte Planungsunterlagen ersetzen keinen aktuellen Work-Package-/Manifest-Trust-Anchor.
