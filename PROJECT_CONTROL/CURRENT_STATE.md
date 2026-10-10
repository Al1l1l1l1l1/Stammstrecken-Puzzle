# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und **Architecture v1.0** sind verbindlich. WP-021, WP-022 und **WP-023 sind abgeschlossen und auf `main` integriert**. Der Puzzle-Kern sowie die Level-v2-/Hash-Foundation sind freigegeben. |
| Derzeitige Phase | `WP-001` bis `WP-007` sind abgeschlossen; WP-014/WP-018, WP-021, WP-022 und **WP-023 ebenfalls abgeschlossen und integriert**. WP-023 bestand die erneute unabhängige Abschluss-QC auf dem eingefrorenen finalen PR-HEAD `f56eab34dca0346d0e339a1c5cde650e15955f68`; danach erteilte die Geschäftsführung die Integrationsfreigabe. PR #15 wurde als Merge-Commit `10405370967496f51be9e5c00c381fd8052ec072` in `main` integriert. Die vorab auf `main` vorhandenen WP-015/WP-016/WP-017-Dateien bleiben ausschließlich Planungsgrundlagen und sind keine ausführbaren Trust Anchors. |
| Historische Branches | `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern` und `feat/wp-013-level-v2-pipeline` bleiben ausschließlich lesbare Vergleichskorpora; kein Merge, Cherry-Pick, Rebase, Copy oder Übernahme ihrer PASS-Aussagen. `chore/ci-readiness-unity` bleibt historischer technischer Befund, keine Lieferquelle. Die abgeschlossene Branch `codex/wp-023-level-v2-foundation` ist nach Integration nur noch Historie; maßgeblich ist `main`. |
| Letzter verifizierter Nachweis | WP-023: erneute unabhängige Abschluss-QC **PASS** auf dem eingefrorenen finalen PR-HEAD `f56eab34dca0346d0e339a1c5cde650e15955f68`; AK-01 bis AK-09 erfüllt, kein integrationsblockierender Befund. Auf diesem Head waren Architecture Validation PR-Run `38017495962` / Push-Run `38017492793` und Unity CI PR-Run `38017495922` / Push-Run `38017492821` **SUCCESS**; Unity CI umfasste Compile, EditMode 380/380, Coverage-Evidenz, PlayMode-Smoke 27/27, Android-IL2CPP und iOS-Export/Xcode-Build. Geschäftsführungsfreigabe anschließend erteilt; Integration über PR #15 als Merge-Commit `10405370967496f51be9e5c00c381fd8052ec072`. Trust Anchor `a1d284caec7746a4aa48fbe564d37d179d60fed8`, Basis `ad0ac1b6b12f9c0acbf59d90734b3c574a2a15d7`, Manifest unverändert (SHA-256 `a8a3a5068bf6b390ad3b44b5e6d8ec36f2d93f9d849959c9230620880a5776b0`). |
| Aktuell zu prüfender Schritt | **WP-023 ist abgeschlossen.** Es besteht kein offener Prüf- oder Integrationsschritt mehr für WP-023. Der nächste Produktionsblock ist Solver-v2 gemäß ADR-031; vor jeder Implementierung muss dafür ein eigener ausführbarer Work-Package-/Manifest-Trust-Anchor auf frischem `main` hergestellt werden. |
| Nächster vorgesehener Schritt | Einen **neuen ausführbaren Solver-v2-Block** auf Basis der vorhandenen WP-016-Planungsfassung und ADR-031 definieren und nach ADR-030 gemeinsam mit eigenem unveränderlichem Production-Scope-Manifest auf einer frischen Branch vom aktuellen `main` verankern. Erst nach erfolgreicher Phase-A-Governance-/Scope-/Trust-Prüfung darf ein separater Implementierungsauftrag folgen. Kein Proof-v1/Strict-Validation/Generator/Content vorziehen. |
| Produktionscode | Auf `main` integriert: WP-021-Scaffold; WP-022-Puzzle-Kern mit validierter Puzzledefinition, Diagnostik/Completion, immutable Session/Commands/Timer und deterministischem Recovery-Solver-v1; **WP-023-Level-v2-/Hash-Foundation** in `STP.Infrastructure.Content` mit strengem Level-v2-Parser/DTO, Domainabbildung auf `PuzzleDefinition`, JCS-Kanonisierung, Content-/Public-Puzzle-/Lösungs-/Proof-Projektionshashes, Level-v1→v2-Migration, diagnostizierendem fail-closed Hashpfad und Content-EditMode-Regressionstests. Importzustand bleibt `AwaitingProofGate`, `IsRuntimeImportable` stets `false`. |
| Architekturentscheidungen | Architecture v1.0; ADR-030 regelt weiterhin den historischen Scope-Trust-Anchor. ADR-031 ist der verbindliche Zielvertrag für den **nun folgenden Solver-v2-/Proofpfad**; WP-023 selbst implementiert diesen späteren Block nicht. |
| Scope-Vertrauensanker | WP-023 und `tools/architecture-validation/scopes/WP-023.production.scope.json` wurden gemeinsam im ersten Branchcommit `a1d284caec7746a4aa48fbe564d37d179d60fed8` eingeführt. Einziger Elterncommit und Manifest-`baseCommit` sind `ad0ac1b6b12f9c0acbf59d90734b3c574a2a15d7`. Das Manifest blieb bis zur Integration byteunveränderlich. Dieser Anchor dokumentiert den abgeschlossenen WP-023-Lieferstand; der nächste Solver-v2-Block benötigt einen eigenen neuen Anchor. |
| CI-Follow-up | WP-023 ist integriert; seine finalen Abschlussnachweise sind vollständig. `validate.yml` und `unity.yml` sind in dem durch PR #15 integrierten Stand Teil von `main`. Für den nächsten Solver-v2-Block gelten die bestehenden Gates unverändert; neue Scope-/Trust-Evidenz ist auf dessen eigenem eingefrorenen Stand neu zu erheben. |

## Verbindliche Grundlage

Die Quellenhierarchie in [`AGENTS.md`](../AGENTS.md) gilt unverändert. Chatverläufe und Modellidentitäten sind keine Projektquelle. Die vorhandenen WP-015/WP-016/WP-017-Dateien sind historische Planungsgrundlagen und keine ausführbaren Trust Anchors.

## Verbindliche Integrations- und Umsetzungsreihenfolge

1. **WP-021 abgeschlossen und integriert:** Unity-Scaffold freigegeben und formal geschlossen.
2. **WP-022 abgeschlossen und integriert:** Puzzle-Kern unabhängig PASS, integriert über PR #13 und formal geschlossen über PR #14.
3. **WP-023 abgeschlossen und integriert:** Level-v2-/Hash-Foundation nach erster QC-FAIL-Runde nachgebessert, erneute unabhängige Abschluss-QC auf `f56eab34…` = PASS, Geschäftsführungsfreigabe, Integration über PR #15 / Merge-Commit `10405370967496f51be9e5c00c381fd8052ec072`.
4. **Solver-v2:** nächster Produktionsblock; eigener ausführbarer Work-Package-/Manifest-Anker erst nach dem nun integrierten WP-023. Die vorhandene WP-016-Datei ist nur Planungsgrundlage.
5. **Proof-v1/Strict-Validation:** erst nach integriertem Solver-v2.
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
| `WORK_QUEUE.md` | Autoritative Priorisierung und Startgates; WP-023 abgeschlossen, Solver-v2 als nächster Produktionsblock. |
| `../WORK_PACKAGES/WP-023_Level-v2-Foundation-und-Hashvertraege.md` | Abgeschlossener und integrierter Level-v2-/Hash-Block als technische Basis. |
| `../WORK_PACKAGES/WP-016_Solver-v2-Metriken-und-Deduktionsspur.md` | Historische Planungsgrundlage für den nächsten Solver-v2-Block; nicht selbst ausführbarer Trust Anchor. |
| `../DECISIONS/ADR-030-wp-scope-trust-anchor.md` | Verbindliche historische Scope-Verankerung für den neu anzulegenden ausführbaren Solver-v2-Auftrag. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Verbindlicher Zielvertrag und Grenze für den folgenden Solver-v2-/Proofpfad. |
| `DEFINITION_OF_DONE.md` | Mindestnachweise für jeden technischen Abschluss. |

Eine neue Instanz beginnt erneut mit `AGENTS.md`; der aktuelle Repository-Stand ersetzt jeden Chatkontext.
