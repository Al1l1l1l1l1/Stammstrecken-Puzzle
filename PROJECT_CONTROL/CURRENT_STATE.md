# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und **Architecture v1.0** sind verbindlich. `main` enthält die Governance-Entscheidungen aus PR #6, PR #7, PR #8 und PR #10 sowie das freigegebene WP-021-Unity-Scaffold aus [PR #11](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/11) und den freigegebenen Puzzle-Kern aus [PR #13](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/13). **WP-021 und WP-022 sind abgeschlossen und integriert.** |
| Derzeitige Phase | `WP-001` bis `WP-007` sind abgeschlossen; WP-014/WP-018, WP-021 und WP-022 ebenfalls. WP-022 wurde auf dem finalen PR-HEAD `b257efd98bd14e7f66ffc40916a0b584c3f51fb7` unabhängig mit **PASS** geprüft und nach ausdrücklicher Geschäftsführungsfreigabe über PR #13 integriert. Vor der Integration war keine technische Korrektur mehr erforderlich. |
| Historische Branches | `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern` und `feat/wp-013-level-v2-pipeline` bleiben archivierte, lesbare Vergleichskorpora; keiner ist direkt integrierbar oder darf gerebased, gecherry-picked oder konfliktaufgelöst werden. `chore/ci-readiness-unity` ist **kein** Produktions-Branch, sondern ein historischer technischer Befund zur CI-Readiness; auch aus ihm wurde nichts übernommen. |
| Letzter verifizierter Nachweis | WP-022-Korrekturcommit `22f44441b60a515fc12a1ae851cb42aecb8f8808`: vollständiger Unity-PR-Run 37666423945 SUCCESS mit EditMode 42/42, PlayMode 27/27/QA, Android-IL2CPP und iOS/Xcode BUILD SUCCEEDED; Governance/Scope PASS. Finaler eingefrorener PR-HEAD `b257efd98bd14e7f66ffc40916a0b584c3f51fb7` wurde unabhängig mit PASS geprüft und anschließend unverändert integriert. |
| Aktuell zu prüfender Schritt | Reiner formaler WP-022-Closeout nach erfolgreicher Integration. Keine Produktionscodeänderung. Die Steuerungsdateien werden auf den tatsächlich integrierten Stand nachgeführt. |
| Nächster vorgesehener Schritt | **Level-v2/Hash-Fundament ist der nächste Produktionsblock.** Vor dessen Implementierung muss der dafür geltende Work-Package-/Scope-Startzustand auf dem aktuellen `main` ADR-030-konform hergestellt werden. Die vorhandene WP-015-Planungsdatei liegt bereits auf `main` und kann daher nicht selbst die geforderte gemeinsame Erstverankerung mit einem neuen Manifest liefern; bis dieser Startzustand sauber hergestellt ist, beginnt keine Implementierung. |
| Produktionscode | Auf `main` integriert: WP-021-Scaffold sowie WP-022-Puzzle-Kern mit validierter Puzzledefinition, objektiver Diagnostik/Completion, immutable Session/Commands/Timer und deterministischem Recovery-Solver-v1. Kein UI-/Bootstrapaufruf, Hash-/Saveadapter, Reward, Hint oder Content-/Proofimport; WP-015/016/017 bleiben getrennte Folgeblöcke. |
| Architekturentscheidungen | Architecture v1.0; 31 ADRs, davon 22 angenommen und aktuell wirksam. ADR-031 bleibt ein verbindlicher Zielvertrag; dessen Proofschema-Implementierung liegt ausschließlich in WP-017 nach der künftigen Recovery-Kette. |
| Scope-Vertrauensanker | WP-022 und [eigenes Manifest](../tools/architecture-validation/scopes/WP-022.production.scope.json) wurden gemeinsam erstmals im ersten Branchcommit `ed898894573f2bb3456d21f22a8522c6b8825621` als A eingeführt; einziger Elterncommit = baseCommit = `8c0d73ac72c01c8bc079a69a00ccf4e5b759c1aa`. Manifest ab diesem Anker byteunveränderlich (ADR-030). Bestehende WP-021-/WP-014-/WP-018-Anker und Manifeste bleiben unverändert. |
| CI-Follow-up | WP-022 verfügt über vollständige commitgebundene Governance-/Unity-/Android-/iOS-/Coverage-/Mutations-/Budgetnachweise. Der bekannte Windows-Selftestbefund V03-005-ABSOLUTE bleibt separat dokumentiert und war kein WP-022-Integrationsblocker. Für Folgepakete werden ausschließlich deren eigene neue Evidenzen verwendet. |

## Verbindliche Grundlage

Die Quellenhierarchie in [`AGENTS.md`](../AGENTS.md) gilt unverändert. Der spätere separate Manus-Chat ist ausdrücklich keine Architekturquelle. ADR-031 ergänzt ADR-007 und ADR-021 ausschließlich für die dort beschriebenen `solver-v2`-Zielmetriken, Proofregeneration und Strict-Semantik; bis WP-017 ersetzt sie keine gegenwärtige Schemaimplementierung.

## Verbindliche Integrations- und Umsetzungsreihenfolge

1. **Governance abgeschlossen:** WP-014/WP-018 wurden nach unabhängiger QC über [PR #6](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/6) integriert; der formale Closeout erfolgte über [PR #7](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/7); die Trust-Anchor-Korrektur über [PR #8](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/8); die Hold-Policy über [PR #10](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/10).
2. **WP-021 abgeschlossen und integriert:** Trust Anchor `fc10c61` erhalten, AK-01–AK-08 und eigene CI-Nachweise belegt; unabhängige Abschluss-QC **PASS**, kein technischer Restblocker vor Integration, Geschäftsführungsfreigabe. [PR #11](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/11) ist als Merge-Commit `cd1a048fe7c77f971c00613de5687a0cac1ec395` auf `main` integriert; formaler Closeout über PR #12.
3. **WP-022 abgeschlossen und integriert:** eigener Trust Anchor `ed89889`, AK-01–AK-10 technisch belegt, AK-07-Korrektur erneut vollständig nachgewiesen; unabhängige Abschluss-QC **PASS** auf finalem Head `b257efd98bd14e7f66ffc40916a0b584c3f51fb7`, Geschäftsführungsfreigabe und Integration über [PR #13](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/13) als Merge-Commit `a85ea224d109ad1713a226481a91f6e24208177d`.
4. **Content-/Proofkette:** Als Nächstes folgt die Level-v2-/Hash-Foundation, danach Solver-v2 und Proof-v1 in harter Reihenfolge. Vor jedem neuen Produktionspaket gilt der eigene ADR-030-konforme Work-Package-/Manifest-Anker.
5. **Contentproduktion:** Erst nach vollständig integrierter Level-/Solver-/Proofpipeline dürfen konkrete Season-1-Level, Generatorprofile, Zeitkalibrierung und weitere Produktionsblöcke beginnen.

## Offene Produktfolgeblocker

| ID | Status | Blockiert |
|---|---|---|
| `BLOCKER-PROD-001` | Offen, fail-closed | Hint-Entitlement / Rewarded-Hint-Economy. |
| `BLOCKER-PROD-002` | Offen, fail-closed | Kalendertag-, Zeitzonen-, Offline- und Manipulationspolicy für Tagesansprüche. |
| `BLOCKER-PROD-003` | Offen, fail-closed | Produktfreigegebenes Generator-Qualitätsprofil für Dauerbaustellenlevel. |

Diese Produktblocker bleiben unverändert und werden durch WP-022 nicht berührt.

## Einstieg für die nächste Instanz

| Datei | Zweck |
|---|---|
| `../AGENTS.md` | Verbindliche Lesereihenfolge und Quellenhierarchie. |
| `../WORK_PACKAGES/WP-021_Unity-Scaffold-Recovery.md` | Abgeschlossenes und integriertes Unity-Scaffold mit Trust Anchor, Akzeptanznachweisen, unabhängiger QC-Freigabe und formalem Closeout. |
| `../WORK_PACKAGES/WP-022_Puzzle-Kern-Recovery.md` | Abgeschlossener und integrierter Puzzle-Kern mit Trust Anchor, technischen Nachweisen, unabhängiger Abschluss-QC und Integrationsnachweis. |
| `../tools/architecture-validation/scopes/WP-022.production.scope.json` | Eigenes unveränderliches Production-Scope-Manifest ab gemeinsamem Add-Anker ed89889, Basis 8c0d73a. |
| `WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md` | Quellenkorrektur sowie Abgrenzung historischer Branches. |
| `../DECISIONS/ADR-030-wp-scope-trust-anchor.md` | Verbindliche gemeinsame Erstverankerung jedes neuen ausführbaren Work Packages mit eigenem Manifest. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Bindender `solver-v2`-Zielvertrag und künftige Recovery-Reihenfolge. |
| `../WORK_PACKAGES/WP-015_Level-v2-Foundation-und-Hashvertraege.md` | Vorhandene Planungsfassung für den nächsten fachlichen Block; vor Implementierung ist der ADR-030-konforme ausführbare Startzustand herzustellen. |
| `../PROJECT_CONTROL/WORK_QUEUE.md` | Autoritative Priorisierung und Startgates. |

Eine neue Instanz beginnt erneut mit `AGENTS.md`; Chatkontext und historische Branches ersetzen keinen aktuellen Work-Package-/Manifest-Trust-Anchor.
