# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und **Architecture v1.0** sind verbindlich. `main` enthält die Governance-Entscheidungen aus PR #6, PR #7, PR #8 und PR #10 sowie das freigegebene WP-021-Unity-Scaffold aus [PR #11](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/11). **WP-021 ist abgeschlossen und integriert.** |
| Derzeitige Phase | `WP-001` bis `WP-007` sind abgeschlossen; WP-014/WP-018 und WP-021 ebenfalls. **WP-022 AK-07-Abschlussgrenzenblocker behoben; betroffene technische Nachweise auf 22f4444 erfolgreich erneuert. Technisch erneut bereit für unabhängige Abschluss-QC.** Gesamt-DoD/QC/Integration offen. |
| Historische Branches | `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern` und `feat/wp-013-level-v2-pipeline` bleiben archivierte, lesbare Vergleichskorpora; keiner ist direkt integrierbar oder darf gerebased, gecherry-picked oder konfliktaufgelöst werden. `chore/ci-readiness-unity` ist **kein** Produktions-Branch, sondern ein historischer technischer Befund zur CI-Readiness; auch aus ihm wurde nichts übernommen. |
| Letzter verifizierter Nachweis | Korrekturcommit **22f44441b60a515fc12a1ae851cb42aecb8f8808** normal gepusht/PR-HEAD verifiziert. Neun deterministische Regressionen alt FAIL/neu PASS; neuer Unity-PR-Run **37666423945 SUCCESS, zehn Jobs**: EditMode 42/42, PlayMode 27/27/QA, Android-IL2CPP, iOS/Xcode BUILD SUCCEEDED. Neue Coverage Solver 375/380, Domain 527/545; Solvermutation 25/24/1 äquivalent, keine nicht äquivalenten Überlebenden; PR-CI-Budget p95 8,549/Maximum 23,877 ms; Ubuntu-17/18 PASS. Exakte Source-/Job-/Artifactbindung im jüngsten WP-Abschluss. |
| Aktuell zu prüfender Schritt | Finale Zeit-/Cancellation-/Monotonieprüfung nach Completion und materialisierter Reduktionsdeduplizierung unmittelbar vor Klassifikation. AK-07 wieder vollständig erfüllt; exakt ausgeschöpftes Knotenbudget korrekt. Abschließender Evidencecommit nur drei Steuerungsdateien, technische Quellen bytegleich zum neuen geprüften 22f4444. Seinen tatsächlichen aktuellen PR-/Remote-HEAD und Governance-CI frisch prüfen. |
| Nächster vorgesehener Schritt | **Zwei unabhängige Abschluss-QCs auf demselben finalen eingefrorenen Branch-HEAD**, danach ausdrückliche Integrationsentscheidung. PR #13 bleibt Draft/unmerged. Kein verbleibender technischer Scopeblocker der Korrektur, keine Integration/Folgefreigabe. |
| Produktionscode | Auf main weiterhin nur das integrierte WP-021-Scaffold. Auf WP-022-Branch neu gebaut: validierte Puzzledefinition, objektive Diagnostik/Completion, immutable Session/Commands/Timer und deterministischer Recovery-Solver-v1. Kein UI-/Bootstrapaufruf, Hash-/Saveadapter, Reward, Hint oder Content-/Proofimport; keine WP-015/016/017-Vorwegnahme. |
| Architekturentscheidungen | Architecture v1.0; 31 ADRs, davon 22 angenommen und aktuell wirksam. ADR-031 bleibt ein verbindlicher Zielvertrag; dessen Proofschema-Implementierung liegt ausschließlich in WP-017 nach der künftigen Recovery-Kette. |
| Scope-Vertrauensanker | WP-022 und [eigenes Manifest](../tools/architecture-validation/scopes/WP-022.production.scope.json) wurden gemeinsam erstmals im ersten Branchcommit `ed898894573f2bb3456d21f22a8522c6b8825621` als A eingeführt; einziger Elterncommit = baseCommit = `8c0d73ac72c01c8bc079a69a00ccf4e5b759c1aa`. Manifest ab diesem Anker byteunveränderlich (ADR-030). Bestehende WP-021-/WP-014-/WP-018-Anker und Manifeste bleiben unverändert; historische WP-021-Basis `e4f8cc1` und Anker `fc10c61` bleiben erhalten. |
| CI-Follow-up | Neue Code-CI: Architecture-PR 37666423942/Push 37666417106 SUCCESS, Ubuntu-Selftests 17/18; kompletter Unity-PR 37666423945 SUCCESS. Redundanter Unity-Push 37666417012 nur EditMode SUCCESS, PlayMode durch Lizenz-Concurrency CANCELLED/Builds SKIPPED, ausdrücklich kein vollständiger Push-PASS. Domainmutation für bytegleiche Quellen historisch weiter gültig; Solver/Coverage/Budget/Builds neu belegt. Windows-Selftest V03-005-ABSOLUTE unverändert separat FAIL. Finaler Dokumentationspush löst separate CI aus; kein PASS vorweggenommen. |

## Verbindliche Grundlage

Die Quellenhierarchie in [`AGENTS.md`](../AGENTS.md) gilt unverändert. Der spätere separate Manus-Chat ist ausdrücklich keine Architekturquelle. ADR-031 ergänzt ADR-007 und ADR-021 ausschließlich für die dort beschriebenen `solver-v2`-Zielmetriken, Proofregeneration und Strict-Semantik; bis WP-017 ersetzt sie keine gegenwärtige Schemaimplementierung.

## Verbindliche Integrations- und Umsetzungsreihenfolge

1. **Governance abgeschlossen:** WP-014/WP-018 wurden nach unabhängiger QC über [PR #6](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/6) integriert; der formale Closeout erfolgte über [PR #7](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/7); die Trust-Anchor-Korrektur über [PR #8](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/8); die Hold-Policy über [PR #10](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/10).
2. **WP-021 abgeschlossen und integriert:** Trust Anchor `fc10c61` erhalten, AK-01–AK-08 und eigene CI-Nachweise belegt; unabhängige Abschluss-QC **PASS**, kein technischer Restblocker vor Integration, Geschäftsführungsfreigabe. [PR #11](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/11) ist als Merge-Commit `cd1a048fe7c77f971c00613de5687a0cac1ec395` auf `main` integriert.
3. **Puzzle-Kern erneut technisch prüfbereit:** WP-022-Anker/Manifest unverändert, AK-07-Abschlussgrenze korrigiert; erforderliche neue sourcegebundene Nachweise auf 22f4444 PASS. Zwei unabhängige Abschluss-QCs und ausdrückliche Integration erforderlich; WP-015 bleibt gesperrt.
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
| `../WORK_PACKAGES/WP-022_Puzzle-Kern-Recovery.md` | Verbindlicher Recovery-Auftrag, enge Grenzen, unveränderter Trust Anchor und aktuelle Phase-B-Implementierungs-/Prüfnachweise; unabhängige QC/Integration bleiben getrennt. |
| `../tools/architecture-validation/scopes/WP-022.production.scope.json` | Eigenes unveränderliches Production-Scope-Manifest ab gemeinsamem Add-Anker ed89889, Basis 8c0d73a. |
| `WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md` | Quellenkorrektur sowie Abgrenzung historischer Branches. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Bindender `solver-v2`-Zielvertrag und künftige Recovery-Reihenfolge. |
| `../WORK_PACKAGES/WP-018_QC-Korrektur-und-Produktionsbasis-Reintegration.md` | Abgeschlossener Governance-Auftrag mit nachträglicher Trust-Anchor-Korrektur. |
| `../WORK_PACKAGES/WP-019_Unity-Scaffold-Recovery-auf-aktuellem-Main.md` | Archivierte, nicht ausführbare Planungsunterlage. |
| `../WORK_PACKAGES/WP-020_Puzzle-Kern-Recovery-auf-aktuellem-Main.md` | Archivierte, nicht ausführbare Planungsunterlage. |
| `../PROJECT_CONTROL/WORK_QUEUE.md` | Autoritative Priorisierung und Startgates. |

Eine neue Instanz beginnt erneut mit `AGENTS.md`; Chatkontext, historische Branches und vorab angelegte Planungsunterlagen ersetzen keinen aktuellen Work-Package-/Manifest-Trust-Anchor.
