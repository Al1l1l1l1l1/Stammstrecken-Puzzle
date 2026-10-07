# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und **Architecture v1.0** sind verbindlich. `main` enthält die Governance-Entscheidungen aus PR #6, PR #7, PR #8 und PR #10 sowie das freigegebene WP-021-Unity-Scaffold aus [PR #11](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/11). **WP-021 ist abgeschlossen und integriert.** |
| Derzeitige Phase | `WP-001` bis `WP-007` sind abgeschlossen; WP-014/WP-018 und WP-021 ebenfalls. **WP-022 Phase B vollständig implementiert und technisch bereit für unabhängige Abschluss-QC** auf `codex/wp-022-puzzle-kern-recovery`; eigene technische AK-01–AK-09 und Implementierungsanteil AK-10 auf 1b6e9ef erfüllt. Kein offener technischer Scopeblocker; Gesamt-DoD/QC/Integration noch offen. |
| Historische Branches | `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern` und `feat/wp-013-level-v2-pipeline` bleiben archivierte, lesbare Vergleichskorpora; keiner ist direkt integrierbar oder darf gerebased, gecherry-picked oder konfliktaufgelöst werden. `chore/ci-readiness-unity` ist **kein** Produktions-Branch, sondern ein historischer technischer Befund zur CI-Readiness; auch aus ihm wurde nichts übernommen. |
| Letzter verifizierter Nachweis | WP-022-Codecommit **1b6e9ef08c2d12d0ebec835f4147891855e41f28** normal gepusht. Eigene gepinnte Architecture-PR 37653358842/Push 37653352967 SUCCESS; vollständiger Unity-Push 37653352943 **SUCCESS, zehn Jobs**: EditMode 33/33, PlayMode 27/27/QA, Android Development IL2CPP und iOS Export/Xcode 26.3/iOS-SDK 26.2 **BUILD SUCCEEDED**. Domain-/Solver-Coverage 96,70 %/98,68 %, erste tatsächliche Mutationbaseline 73/24 mit 67/23 erkannt und 6/1 äquivalent ausgeschlossen, keine nicht äquivalenten Überlebenden; CI-Budget p95 4,589 ms/Maximum 23,222 ms. Job-/Artefakt-/Sourcehashnachweise im WP-022. |
| Aktuell zu prüfender Schritt | WP-021-Closeout PR #12 integriert; Main-/Branchbasis weiterhin `8c0d73ac72c01c8bc079a69a00ccf4e5b759c1aa`. Neue Domain-/Solver-C# und eigene Tests ausschließlich direkt in vier erlaubten Modulen; unity.yml nur tatsächliches Testgate/vier zusätzliche Coveragepflichtassemblies. Technischer Stand eingefroren auf 1b6e9ef; abschließende Evidenznachführung nur WP/CURRENT_STATE/WORK_QUEUE. Deren aktueller Remote-/PR-HEAD ist für unabhängige QC frisch zu lesen und gegen bytegleiche technische Quellen des geprüften Codecommits zu verifizieren. |
| Nächster vorgesehener Schritt | **Zwei unabhängige Abschluss-QCs auf demselben abschließenden eingefrorenen Branch-HEAD** (Astra/Sol, kein Berichtsaustausch vor eigener Abgabe), anschließend ausdrückliche Integrationsentscheidung. [PR #13](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/13) bleibt Draft/unmerged. Anker ed89889/Manifest unverändert; kein Definitions-/Technikblocker. Keine QC im Namen anderer erstellt, keine Integration/Folgefreigabe. |
| Produktionscode | Auf main weiterhin nur das integrierte WP-021-Scaffold. Auf WP-022-Branch neu gebaut: validierte Puzzledefinition, objektive Diagnostik/Completion, immutable Session/Commands/Timer und deterministischer Recovery-Solver-v1. Kein UI-/Bootstrapaufruf, Hash-/Saveadapter, Reward, Hint oder Content-/Proofimport; keine WP-015/016/017-Vorwegnahme. |
| Architekturentscheidungen | Architecture v1.0; 31 ADRs, davon 22 angenommen und aktuell wirksam. ADR-031 bleibt ein verbindlicher Zielvertrag; dessen Proofschema-Implementierung liegt ausschließlich in WP-017 nach der künftigen Recovery-Kette. |
| Scope-Vertrauensanker | WP-022 und [eigenes Manifest](../tools/architecture-validation/scopes/WP-022.production.scope.json) wurden gemeinsam erstmals im ersten Branchcommit `ed898894573f2bb3456d21f22a8522c6b8825621` als A eingeführt; einziger Elterncommit = baseCommit = `8c0d73ac72c01c8bc079a69a00ccf4e5b759c1aa`. Manifest ab diesem Anker byteunveränderlich (ADR-030). Bestehende WP-021-/WP-014-/WP-018-Anker und Manifeste bleiben unverändert; historische WP-021-Basis `e4f8cc1` und Anker `fc10c61` bleiben erhalten. |
| CI-Follow-up | Tatsächliche eigene neue Ubuntu-Governance-CI auf 1b6e9ef: beide Self-/Negativtestmodi 17/18 PASS, Scope/Trust/sauberer HEAD. Vollständiger Unity-Push 37653352943 SUCCESS; Unity-PR 37653358829 Compile/EditMode/PlayMode/Android ebenfalls SUCCESS, zusätzliche iOS-Wiederholung bei Dokumentation noch laufend, kein zusätzlicher PASS vorweggenommen. Beide Coveragegates messen alle zwölf Pflichtassemblies einschließlich Nullen; Branchpunkte 0/0, keine Branchcoveragebehauptung. Bekannter Windows-Selftestbefund V03-005-ABSOLUTE unverändert separat FAIL. Reiner Evidenzfolgecommit ändert keine getestete technische Quelle; dessen neu ausgelöste CI ist gesondert aktuell zu lesen. |

## Verbindliche Grundlage

Die Quellenhierarchie in [`AGENTS.md`](../AGENTS.md) gilt unverändert. Der spätere separate Manus-Chat ist ausdrücklich keine Architekturquelle. ADR-031 ergänzt ADR-007 und ADR-021 ausschließlich für die dort beschriebenen `solver-v2`-Zielmetriken, Proofregeneration und Strict-Semantik; bis WP-017 ersetzt sie keine gegenwärtige Schemaimplementierung.

## Verbindliche Integrations- und Umsetzungsreihenfolge

1. **Governance abgeschlossen:** WP-014/WP-018 wurden nach unabhängiger QC über [PR #6](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/6) integriert; der formale Closeout erfolgte über [PR #7](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/7); die Trust-Anchor-Korrektur über [PR #8](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/8); die Hold-Policy über [PR #10](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/10).
2. **WP-021 abgeschlossen und integriert:** Trust Anchor `fc10c61` erhalten, AK-01–AK-08 und eigene CI-Nachweise belegt; unabhängige Abschluss-QC **PASS**, kein technischer Restblocker vor Integration, Geschäftsführungsfreigabe. [PR #11](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/11) ist als Merge-Commit `cd1a048fe7c77f971c00613de5687a0cac1ec395` auf `main` integriert.
3. **Puzzle-Kern Phase B technisch vollständig/prüfbereit:** WP-022/eigenes Manifest unverändert ab gemeinsamem Anker `ed89889`, Basis main 8c0d73a; eigene neue Implementation und vollständige technische Nachweise auf 1b6e9ef, Draft-PR #13. Zwei unabhängige Abschluss-QCs und ausdrückliche Integration bleiben erforderlich. WP-015 bleibt bis zur Integration gesperrt.
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
