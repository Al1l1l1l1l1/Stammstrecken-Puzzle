# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und **Architecture v1.0** sind verbindlich. WP-021 und WP-022 sind abgeschlossen und auf `main` integriert. Der Puzzle-Kern ist freigegeben; die Level-v2-/Hash-Foundation (WP-023) ist implementiert und CI-verifiziert. Die erste unabhängige Abschluss-QC (auf `182db13…`) endete mit FAIL; die vier Befunde sind behoben und CI-verifiziert (Code-Stand `167a6ac…`). Eine erneute unabhängige Abschluss-QC und die Integration stehen aus. |
| Derzeitige Phase | `WP-001` bis `WP-007` sind abgeschlossen; WP-014/WP-018, WP-021 und WP-022 ebenfalls. **WP-023 – Level-v2-Foundation und Hashverträge: Phase A abgeschlossen; Phase B implementiert; die erste unabhängige Abschluss-QC auf `182db13af5f8e2d1db3acadc1318153575baac85` endete mit FAIL (vier Befunde). Die Nachbesserung ist implementiert und verifiziert (Code-Stand `167a6ac920a87d74b63d4cf63426e8d9d4909145`): Alle GitHub-Actions-Workflows inklusive Unity-Compile, EditMode 380/380, Coverage-Evidenz, PlayMode-Smoke, Android-IL2CPP und iOS-Export sind auf diesem Stand SUCCESS. Erneute unabhängige Abschluss-QC und Geschäftsführungsfreigabe stehen aus; nicht abgeschlossen, nicht integriert.** Die vorab auf `main` vorhandene WP-015-Datei bleibt ausschließlich Planungsgrundlage und ist kein ausführbarer Trust Anchor. |
| Historische Branches | `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern` und `feat/wp-013-level-v2-pipeline` bleiben ausschließlich lesbare Vergleichskorpora; kein Merge, Cherry-Pick, Rebase, Copy oder Übernahme ihrer PASS-Aussagen. `chore/ci-readiness-unity` bleibt historischer technischer Befund, keine Lieferquelle. |
| Letzter verifizierter Nachweis | WP-023 auf Branch `codex/wp-023-level-v2-foundation` (Draft-PR #15), Code-Stand `167a6ac920a87d74b63d4cf63426e8d9d4909145` (Trust Anchor `a1d284caec7746a4aa48fbe564d37d179d60fed8`, Basis `ad0ac1b6b12f9c0acbf59d90734b3c574a2a15d7`, Manifest unverändert, SHA-256 `a8a3a506…776b0`). Behobene QC-Befunde: (1) alleinstehendes Surrogat in einem per `JsonValue`-Fabriken gebauten Dokument → jetzt Diagnose `LVL-JCS-SURROGATE` statt Exception, (2) `ParseText` misst die Größengrenze in UTF-8-Bytes, (3) `JsonValue.Items`/`Members` und alle Ergebnislisten sind schreibgeschützte Sichten, (4) ein Surrogatpaar kann die Stringgrenze nicht mehr überschreiten. GitHub Actions auf diesem Stand: Architecture Validation PR-Run `38014310417` / Push-Run `38014306432` **SUCCESS**; Unity CI Push-Run `38014306441` und PR-Run `38014310372` (Versuch 2) **SUCCESS** (Editor 6000.3.23f1; EditMode 380/380, davon Content 338, Domain 19, Solver 22; Coverage-Evidenz PASS, `STP.Infrastructure.Content` 97,7 % Sequenzpunkte; PlayMode-Smoke, Android-IL2CPP, iOS-Export SUCCESS). Lokal zusätzlich: .NET-Harness 338/338 Content, 41/41 Domain/Solver. Vorverlauf: `28b0f72…` war nicht Unity-grün (echter Compilefehler CS8602 in `StrictJsonParser.ParseText`, behoben in `167a6ac…`); der PR-Run-Versuch 1 auf `167a6ac…` wurde durch die Concurrency-Gruppe `unity-personal-seat` abgebrochen und per Rerun wiederholt. Keine Gateabsenkung. |
| Aktuell zu prüfender Schritt | Erneute unabhängige Abschluss-QC von WP-023 auf dem eingefrorenen finalen PR-HEAD (Checks dort neu erheben). Kein bekannter Code-, Scope-, Trust- oder CI-Blocker. Details zu den vier Befunden und ihrer Behebung: WP-023, Abschnitt „QC-FAIL-Nachbesserung“. |
| Nächster vorgesehener Schritt | 1. **Erneute unabhängige Abschluss-QC** auf dem eingefrorenen finalen PR-HEAD (nicht durch den Implementierungsagenten). 2. Geschäftsführungsfreigabe und Integration von PR #15 (bis dahin Draft, nicht gemergt). 3. Danach separater Solver-v2-Block. Kein Solver-v2/Proof/Generator/Content vorziehen. |
| Produktionscode | Auf `main` integriert: WP-021-Scaffold und WP-022-Puzzle-Kern mit validierter Puzzledefinition, Diagnostik/Completion, immutable Session/Commands/Timer und deterministischem Recovery-Solver-v1. WP-023 Phase B (inklusive QC-FAIL-Nachbesserung) ergänzt auf der Branch (noch nicht auf `main`) in `STP.Infrastructure.Content`: strikten Level-v2-Parser/DTO, Domainabbildung auf `PuzzleDefinition`, JCS-Kanonisierung, Content-/Public-Puzzle-/Lösungs-/Proof-Projektionshashes, Level-v1→v2-Migration und die Content-EditMode-Tests. Importzustand bleibt `AwaitingProofGate`, `IsRuntimeImportable` stets `false`. |
| Architekturentscheidungen | Architecture v1.0; ADR-030 regelt weiterhin den historischen Scope-Trust-Anchor. ADR-031 bleibt Zielvertrag für den späteren Solver-v2-/Proofpfad und wird durch WP-023 nicht vorgezogen. |
| Scope-Vertrauensanker | WP-023 und `tools/architecture-validation/scopes/WP-023.production.scope.json` wurden gemeinsam im ersten Branchcommit `a1d284caec7746a4aa48fbe564d37d179d60fed8` eingeführt. Einziger Elterncommit und Manifest-`baseCommit` sind `ad0ac1b6b12f9c0acbf59d90734b3c574a2a15d7`. Das Manifest bleibt ab diesem Anker byteunveränderlich. |
| CI-Follow-up | `validate.yml` zeigt ausschließlich auf das WP-023-Manifest und prüft zusätzlich die JCS-/Hash-Goldenvektoren gegen die Node-Referenz. `unity.yml` führt die Content-Assemblies in EditMode-Evidenz und Coverage-Pflichtmengen (keine Gateabsenkung). Governance-/Scope-/Trust-/Golden- und Unity-Nachweis sind auf dem Code-Stand `167a6ac…` PASS (die Nachbesserung änderte weder Workflows noch Golden-Vektoren noch Manifest). |

## Verbindliche Grundlage

Die Quellenhierarchie in [`AGENTS.md`](../AGENTS.md) gilt unverändert. Chatverläufe und Modellidentitäten sind keine Projektquelle. Die vorhandene WP-015-Datei ist historische Planungsgrundlage; der ausführbare aktuelle Auftrag ist WP-023.

## Verbindliche Integrations- und Umsetzungsreihenfolge

1. **WP-021 abgeschlossen und integriert:** Unity-Scaffold freigegeben und formal geschlossen.
2. **WP-022 abgeschlossen und integriert:** Puzzle-Kern unabhängig PASS, integriert über PR #13 und formal geschlossen über PR #14.
3. **WP-023 – Level-v2-/Hash-Foundation:** Phase A abgeschlossen, Phase B implementiert und CI-verifiziert; erste Abschluss-QC = FAIL, Nachbesserung implementiert und CI-verifiziert; als Nächstes erneute unabhängige Abschluss-QC, danach Integration.
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
| `../WORK_PACKAGES/WP-023_Level-v2-Foundation-und-Hashvertraege.md` | Aktueller ausführbarer Produktionsauftrag; Phase A abgeschlossen, Phase B implementiert (Abschnitt „Phase-B-Nachweis“), erste Abschluss-QC FAIL und Nachbesserung CI-verifiziert (Abschnitt „QC-FAIL-Nachbesserung“), erneute Abschluss-QC offen. |
| `../tools/architecture-validation/scopes/WP-023.production.scope.json` | Eigenes unveränderliches Scope-Manifest des aktuellen Auftrags. |
| `../WORK_PACKAGES/WP-015_Level-v2-Foundation-und-Hashvertraege.md` | Historische Planungsgrundlage; nicht ausführbarer Trust Anchor. |
| `../WORK_PACKAGES/WP-022_Puzzle-Kern-Recovery.md` | Abgeschlossener, integrierter Puzzle-Kern als technische Basis. |
| `../DECISIONS/ADR-030-wp-scope-trust-anchor.md` | Verbindliche historische Scope-Verankerung. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Grenze zum nachfolgenden Solver-v2-/Proofblock. |
| `WORK_QUEUE.md` | Autoritative Priorisierung und Startgates. |

Eine neue Instanz beginnt erneut mit `AGENTS.md`; der aktuelle Repository-Stand ersetzt jeden Chatkontext.
