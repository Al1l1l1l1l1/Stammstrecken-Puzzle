# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und **Architecture v1.0** sind verbindlich. WP-021 und WP-022 sind abgeschlossen und auf `main` integriert. Der Puzzle-Kern ist freigegeben; die Level-v2-/Hash-Foundation (WP-023) ist implementiert, aber noch nicht abgeschlossen oder integriert. |
| Derzeitige Phase | `WP-001` bis `WP-007` sind abgeschlossen; WP-014/WP-018, WP-021 und WP-022 ebenfalls. **WP-023 – Level-v2-Foundation und Hashverträge: Phase A abgeschlossen; Phase B implementiert und lokal verifiziert (Implementierungsstand `4e5fbd327cacd021a35f8a8c839fb69f92dbe9f2`). Echte Unity-CI-Nachweise (Compile, EditMode inklusive Content-Assemblies, Coverage) sind NOT_EXECUTED (Docker-Hub-Pull-Limit der Runner); unabhängige Abschluss-QC steht aus. Nicht abgeschlossen, nicht integriert.** Die vorab auf `main` vorhandene WP-015-Datei bleibt ausschließlich Planungsgrundlage und ist kein ausführbarer Trust Anchor. |
| Historische Branches | `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern` und `feat/wp-013-level-v2-pipeline` bleiben ausschließlich lesbare Vergleichskorpora; kein Merge, Cherry-Pick, Rebase, Copy oder Übernahme ihrer PASS-Aussagen. `chore/ci-readiness-unity` bleibt historischer technischer Befund, keine Lieferquelle. |
| Letzter verifizierter Nachweis | WP-023 Phase B auf Branch `codex/wp-023-level-v2-foundation`, Implementierungsstand `4e5fbd327cacd021a35f8a8c839fb69f92dbe9f2` (Trust Anchor `a1d284caec7746a4aa48fbe564d37d179d60fed8`, Basis `ad0ac1b6b12f9c0acbf59d90734b3c574a2a15d7`, Manifest unverändert, SHA-256 `a8a3a506…776b0`). Lokal (unabhängiger .NET-Harness, kein Unity): 307/307 Content-Tests, 41/41 Domain-/Solver-Regressionen. GitHub: Architecture Validation PR-Run `37992242788` und Push-Run `37992238851` **SUCCESS** (Architecture-only, Golden-Crosscheck gegen Node-Referenz, kanonischer WP-023-Production-Scope jeweils mit Self-/Negativtests). Unity CI (`37992242745`, `37992238852`): Preflight, Config, Evidence-Guard, iOS-Umgebung SUCCESS; `unity-compile-editmode`, Linux- und Android-Umgebung scheitern wiederholt vor Codeausführung am Docker-Hub-Pull-Limit (`toomanyrequests`); Folgejobs SKIPPED. |
| Aktuell zu prüfender Schritt | Unity-CI-Läufe auf dem finalen PR-HEAD erneut erheben (Compile, EditMode `STP.Tests.Content.EditMode`, Coverage-Evidenz). Bis dahin ist `AK-08`/`AK-09` nicht erfüllt. Kein Code-, Scope- oder Trust-Blocker bekannt; der Zustand ist ein Infrastrukturnachweis-Blocker (Docker-Hub-Pull-Limit), der nicht durch Gateänderung umgangen wird. |
| Nächster vorgesehener Schritt | 1. Unity-CI auf dem finalen PR-HEAD erfolgreich nachweisen. 2. **Unabhängige Abschluss-QC** auf dem eingefrorenen finalen PR-HEAD (nicht durch den Implementierungsagenten). 3. Geschäftsführungsfreigabe und Integration von PR #15. Kein Solver-v2/Proof/Generator/Content vorziehen. |
| Produktionscode | Auf `main` integriert: WP-021-Scaffold und WP-022-Puzzle-Kern mit validierter Puzzledefinition, Diagnostik/Completion, immutable Session/Commands/Timer und deterministischem Recovery-Solver-v1. WP-023 Phase B ergänzt auf der Branch (noch nicht auf `main`) in `STP.Infrastructure.Content`: strikten Level-v2-Parser/DTO, Domainabbildung auf `PuzzleDefinition`, JCS-Kanonisierung, Content-/Public-Puzzle-/Lösungs-/Proof-Projektionshashes, Level-v1→v2-Migration und die Content-EditMode-Tests. Importzustand bleibt `AwaitingProofGate`, `IsRuntimeImportable` stets `false`. |
| Architekturentscheidungen | Architecture v1.0; ADR-030 regelt weiterhin den historischen Scope-Trust-Anchor. ADR-031 bleibt Zielvertrag für den späteren Solver-v2-/Proofpfad und wird durch WP-023 nicht vorgezogen. |
| Scope-Vertrauensanker | WP-023 und `tools/architecture-validation/scopes/WP-023.production.scope.json` wurden gemeinsam im ersten Branchcommit `a1d284caec7746a4aa48fbe564d37d179d60fed8` eingeführt. Einziger Elterncommit und Manifest-`baseCommit` sind `ad0ac1b6b12f9c0acbf59d90734b3c574a2a15d7`. Das Manifest bleibt ab diesem Anker byteunveränderlich. |
| CI-Follow-up | `validate.yml` zeigt ausschließlich auf das WP-023-Manifest und prüft zusätzlich die JCS-/Hash-Goldenvektoren gegen die Node-Referenz. `unity.yml` führt die Content-Assemblies in EditMode-Evidenz und Coverage-Pflichtmengen (keine Gateabsenkung). Governance-/Scope-/Trust-/Golden-Nachweis für Phase B ist PASS; der Unity-Nachweis ist NOT_EXECUTED. |

## Verbindliche Grundlage

Die Quellenhierarchie in [`AGENTS.md`](../AGENTS.md) gilt unverändert. Chatverläufe und Modellidentitäten sind keine Projektquelle. Die vorhandene WP-015-Datei ist historische Planungsgrundlage; der ausführbare aktuelle Auftrag ist WP-023.

## Verbindliche Integrations- und Umsetzungsreihenfolge

1. **WP-021 abgeschlossen und integriert:** Unity-Scaffold freigegeben und formal geschlossen.
2. **WP-022 abgeschlossen und integriert:** Puzzle-Kern unabhängig PASS, integriert über PR #13 und formal geschlossen über PR #14.
3. **WP-023 – Level-v2-/Hash-Foundation:** Phase A abgeschlossen, Phase B implementiert; als Nächstes Unity-CI-Nachweis, danach unabhängige Abschluss-QC und Integration.
4. **Solver-v2:** erst nach integrierter Level-v2-/Hash-Foundation.
5. **Proof-v1/Strict-Validation:** erst nach integriertem Solver-v2.
6. **Contentproduktion:** erst nach vollständig integrierter Level-/Solver-/Proofpipeline.

## Offene Produktfolgeblocker

| ID | Status | Blockiert |
|---|---|---|
| `BLOCKER-PROD-001` | Offen, fail-closed | Hint-Entitlement / Rewarded-Hint-Economy. |
| `BLOCKER-PROD-002` | Offen, fail-closed | Kalendertag-, Zeitzonen-, Offline- und Manipulationspolicy für Tagesansprüche. |
| `BLOCKER-PROD-003` | Offen, fail-closed | Produktfreigegebenes Generator-Qualitätsprofil für Dauerbaustellenlevel. |

Diese Produktblocker bleiben unverändert und werden durch WP-023 nicht umgangen.

## Einstieg für die nächste Instanz

| Datei | Zweck |
|---|---|
| `../AGENTS.md` | Verbindliche Lesereihenfolge und Quellenhierarchie. |
| `../WORK_PACKAGES/WP-023_Level-v2-Foundation-und-Hashvertraege.md` | Aktueller ausführbarer Produktionsauftrag; Phase A abgeschlossen, Phase B implementiert (Abschnitt „Phase-B-Nachweis“), Unity-Nachweis und Abschluss-QC offen. |
| `../tools/architecture-validation/scopes/WP-023.production.scope.json` | Eigenes unveränderliches Scope-Manifest des aktuellen Auftrags. |
| `../WORK_PACKAGES/WP-015_Level-v2-Foundation-und-Hashvertraege.md` | Historische Planungsgrundlage; nicht ausführbarer Trust Anchor. |
| `../WORK_PACKAGES/WP-022_Puzzle-Kern-Recovery.md` | Abgeschlossener, integrierter Puzzle-Kern als technische Basis. |
| `../DECISIONS/ADR-030-wp-scope-trust-anchor.md` | Verbindliche historische Scope-Verankerung. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Grenze zum nachfolgenden Solver-v2-/Proofblock. |
| `WORK_QUEUE.md` | Autoritative Priorisierung und Startgates. |

Eine neue Instanz beginnt erneut mit `AGENTS.md`; der aktuelle Repository-Stand ersetzt jeden Chatkontext.
