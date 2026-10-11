# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und **Architecture v1.0** sind verbindlich. WP-021, WP-022 und **WP-023 sind abgeschlossen und auf `main` integriert**. Der Puzzle-Kern sowie die Level-v2-/Hash-Foundation sind freigegeben. |
| Derzeitige Phase | `WP-001` bis `WP-007` sind abgeschlossen; WP-014/WP-018, WP-021, WP-022 und **WP-023 ebenfalls abgeschlossen und integriert**. WP-023 bestand die erneute unabhängige Abschluss-QC auf dem eingefrorenen finalen PR-HEAD `f56eab34dca0346d0e339a1c5cde650e15955f68`; danach erteilte die Geschäftsführung die Integrationsfreigabe. PR #15 wurde als Merge-Commit `10405370967496f51be9e5c00c381fd8052ec072` in `main` integriert. Die vorab auf `main` vorhandenen WP-015/WP-016/WP-017-Dateien bleiben ausschließlich Planungsgrundlagen und sind keine ausführbaren Trust Anchors. **WP-024 (Solver-v2-Metriken und Deduktionsspur) ist nach ADR-030 verankert; Phase A ist abgeschlossen, Phase B (Implementierung) ist umgesetzt. Die erste unabhängige Abschluss-QC auf PR-HEAD `6f74ebbb6379d5c8a95c4515f664e6d4d2048bc4` endete mit FAIL (zwei HIGH-Befunde: unvollständige öffentliche Fakten der Deduktionsspur und eine Lücke im harten Zeitbudget). Beide sind innerhalb von WP-024 korrigiert (Abschnitt „QC-Korrektur“ im Work Package); die erneute unabhängige Abschlussprüfung auf dem neuen finalen PR-HEAD steht aus. Kein Merge.** |
| Historische Branches | `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern` und `feat/wp-013-level-v2-pipeline` bleiben ausschließlich lesbare Vergleichskorpora; kein Merge, Cherry-Pick, Rebase, Copy oder Übernahme ihrer PASS-Aussagen. `chore/ci-readiness-unity` bleibt historischer technischer Befund, keine Lieferquelle. Die abgeschlossene Branch `codex/wp-023-level-v2-foundation` ist nach Integration nur noch Historie; maßgeblich ist `main`. |
| Letzter verifizierter Nachweis | WP-023: erneute unabhängige Abschluss-QC **PASS** auf dem eingefrorenen finalen PR-HEAD `f56eab34dca0346d0e339a1c5cde650e15955f68`; AK-01 bis AK-09 erfüllt, kein integrationsblockierender Befund. Auf diesem Head waren Architecture Validation PR-Run `38017495962` / Push-Run `38017492793` und Unity CI PR-Run `38017495922` / Push-Run `38017492821` **SUCCESS**; Unity CI umfasste Compile, EditMode 380/380, Coverage-Evidenz, PlayMode-Smoke 27/27, Android-IL2CPP und iOS-Export/Xcode-Build. Geschäftsführungsfreigabe anschließend erteilt; Integration über PR #15 als Merge-Commit `10405370967496f51be9e5c00c381fd8052ec072`. Trust Anchor `a1d284caec7746a4aa48fbe564d37d179d60fed8`, Basis `ad0ac1b6b12f9c0acbf59d90734b3c574a2a15d7`, Manifest unverändert (SHA-256 `a8a3a5068bf6b390ad3b44b5e6d8ec36f2d93f9d849959c9230620880a5776b0`). |
| Aktuell zu prüfender Schritt | **WP-024 Phase B nach QC-Korrektur:** erneute unabhängige Abschlussprüfung (Metriksemantik und Traceherkunft gegen ADR-031, Prämissenfakten, hartes Budget, Teststärke, Scope, CI-Evidenz) auf dem eingefrorenen finalen HEAD von PR #17 (Draft, nicht gemergt). Details im Abschnitt „QC-Korrektur nach der ersten unabhängigen Abschluss-QC“ von WP-024; die Zahlen des ersten „Phase-B-Nachweises“ gelten nicht für den korrigierten Stand. |
| Nächster vorgesehener Schritt | Erneute unabhängige Abschlussprüfung von WP-024 auf dem finalen PR-HEAD nach der QC-Korrektur, danach Geschäftsführungsentscheidung zur Integration. Kein Proof-v1/Strict-Validation/Generator/Content vorziehen. Der Proof-v1-Block erhält erst nach Integration von WP-024 einen eigenen ausführbaren Startanker. |
| Produktionscode | Auf `main` integriert: WP-021-Scaffold; WP-022-Puzzle-Kern mit validierter Puzzledefinition, Diagnostik/Completion, immutable Session/Commands/Timer und deterministischem Recovery-Solver-v1; **WP-023-Level-v2-/Hash-Foundation** in `STP.Infrastructure.Content` mit strengem Level-v2-Parser/DTO, Domainabbildung auf `PuzzleDefinition`, JCS-Kanonisierung, Content-/Public-Puzzle-/Lösungs-/Proof-Projektionshashes, Level-v1→v2-Migration, diagnostizierendem fail-closed Hashpfad und Content-EditMode-Regressionstests. Importzustand bleibt `AwaitingProofGate`, `IsRuntimeImportable` stets `false`. **Auf dem WP-024-Branch (noch nicht integriert):** `solver-v2` mit auditierbarer Root-Deduktionsspur (jeder Schritt deklariert genau die öffentlichen Fakten, die seine Ableitung liest), den vier ADR-031-Metriken (nur bei `UNIQUE`, `UNIQUE` erst nach einer Budgetprüfung, die der gesamten Ergebnisarbeit folgt) und dem fail-closed `CampaignGate`; kein Proof, kein Schema. |
| Architekturentscheidungen | Architecture v1.0; ADR-030 regelt weiterhin den historischen Scope-Trust-Anchor. ADR-031 ist der verbindliche Zielvertrag für den **nun folgenden Solver-v2-/Proofpfad**; WP-023 selbst implementiert diesen späteren Block nicht. |
| Scope-Vertrauensanker | WP-023 und `tools/architecture-validation/scopes/WP-023.production.scope.json` wurden gemeinsam im ersten Branchcommit `a1d284caec7746a4aa48fbe564d37d179d60fed8` eingeführt. Einziger Elterncommit und Manifest-`baseCommit` sind `ad0ac1b6b12f9c0acbf59d90734b3c574a2a15d7`. Das Manifest blieb bis zur Integration byteunveränderlich. Dieser Anchor dokumentiert den abgeschlossenen WP-023-Lieferstand; der nächste Solver-v2-Block benötigt einen eigenen neuen Anchor. |
| Scope-Vertrauensanker WP-024 | WP-024 und `tools/architecture-validation/scopes/WP-024.production.scope.json` wurden gemeinsam im ersten Branchcommit `3e7f1ab2e56853294a4f09650d8ec361ab4eca1a` eingeführt. Einziger Elterncommit und Manifest-`baseCommit` sind `ab117f459689d2b2937d436bb1a763296a65d52a`. Das Manifest ist ab dem Anker byteunveränderlich. |
| CI-Follow-up | WP-023 ist integriert; seine finalen Abschlussnachweise sind vollständig. `validate.yml` und `unity.yml` sind in dem durch PR #15 integrierten Stand Teil von `main`. Für den nächsten Solver-v2-Block gelten die bestehenden Gates unverändert; neue Scope-/Trust-Evidenz ist auf dessen eigenem eingefrorenen Stand neu zu erheben. |

## Verbindliche Grundlage

Die Quellenhierarchie in [`AGENTS.md`](../AGENTS.md) gilt unverändert. Chatverläufe und Modellidentitäten sind keine Projektquelle. Die vorhandenen WP-015/WP-016/WP-017-Dateien sind historische Planungsgrundlagen und keine ausführbaren Trust Anchors.

## Verbindliche Integrations- und Umsetzungsreihenfolge

1. **WP-021 abgeschlossen und integriert:** Unity-Scaffold freigegeben und formal geschlossen.
2. **WP-022 abgeschlossen und integriert:** Puzzle-Kern unabhängig PASS, integriert über PR #13 und formal geschlossen über PR #14.
3. **WP-023 abgeschlossen und integriert:** Level-v2-/Hash-Foundation nach erster QC-FAIL-Runde nachgebessert, erneute unabhängige Abschluss-QC auf `f56eab34…` = PASS, Geschäftsführungsfreigabe, Integration über PR #15 / Merge-Commit `10405370967496f51be9e5c00c381fd8052ec072`.
4. **Solver-v2 (WP-024):** aktueller Produktionsblock; Phase A abgeschlossen, Phase B implementiert, nach der ersten QC-FAIL-Runde korrigiert und technisch bereit für die erneute unabhängige Abschlussprüfung (nicht gemergt). Die vorhandene WP-016-Datei ist nur Planungsgrundlage.
5. **Proof-v1/Strict-Validation:** erst nach integriertem WP-024; eigener ausführbarer Startanker.
6. **Contentproduktion:** erst nach vollständig integrierter Level-/Solver-/Proofpipeline.

## Offene Produktfolgeblocker

| ID | Status | Blockiert |
|---|---|---|
| `BLOCKER-PROD-001` | Offen, fail-closed | Hint-Entitlement / Rewarded-Hint-Economy. |
| `BLOCKER-PROD-002` | Offen, fail-closed | Kalendertag-, Zeitzonen-, Offline- und Manipulationspolicy für Tagesansprüche. |
| `BLOCKER-PROD-003` | Offen, fail-closed | Produktfreigegebenes Generator-Qualitätsprofil für Dauerbaustellenlevel. |

Diese Produktblocker bleiben unverändert und werden durch WP-023 oder den folgenden Solver-/Proofpfad nicht umgangen.

## Einstieg für die nächste Instanz

| Datei | Zweck |
|---|---|
| `../AGENTS.md` | Verbindliche Lesereihenfolge und Quellenhierarchie. |
| `WORK_QUEUE.md` | Autoritative Priorisierung und Startgates; WP-023 abgeschlossen, WP-024 (Solver-v2) aktueller Produktionsblock, Phase B implementiert und nach der ersten QC-FAIL-Runde korrigiert, erneute Abschlussprüfung offen. |
| `../WORK_PACKAGES/WP-023_Level-v2-Foundation-und-Hashvertraege.md` | Abgeschlossener und integrierter Level-v2-/Hash-Block als technische Basis. |
| `../WORK_PACKAGES/WP-024_Solver-v2-Metriken-und-Deduktionsspur.md` | Aktueller, ADR-030-verankerter Solver-v2-Auftrag (Phase A abgeschlossen, Phase-B-Nachweis und QC-Korrektur enthalten). |
| `../WORK_PACKAGES/WP-016_Solver-v2-Metriken-und-Deduktionsspur.md` | Historische Planungsgrundlage für WP-024; nicht selbst ausführbarer Trust Anchor. |
| `../DECISIONS/ADR-030-wp-scope-trust-anchor.md` | Verbindliche historische Scope-Verankerung; Grundlage des WP-024-Ankers. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Verbindlicher Zielvertrag und Grenze für den folgenden Solver-v2-/Proofpfad. |
| `DEFINITION_OF_DONE.md` | Mindestnachweise für jeden technischen Abschluss. |

Eine neue Instanz beginnt erneut mit `AGENTS.md`; der aktuelle Repository-Stand ersetzt jeden Chatkontext.
