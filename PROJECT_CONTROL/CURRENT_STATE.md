# Aktueller Projektstand

| Feld | Aktueller Stand |
|---|---|
| Projektstatus | Produktkonzeption und **Architecture v1.0** sind verbindlich. WP-021 und WP-022 sind abgeschlossen und auf `main` integriert. Der Puzzle-Kern ist freigegeben; der nächste fachliche Block ist die Level-v2-/Hash-Foundation. |
| Derzeitige Phase | `WP-001` bis `WP-007` sind abgeschlossen; WP-014/WP-018, WP-021 und WP-022 ebenfalls. **WP-023 – Level-v2-Foundation und Hashverträge: Phase A abgeschlossen; definiert, ADR-030-konform verankert und durch Governance-/Scope-/Trust-CI geprüft. Implementierung noch nicht begonnen.** Die vorab auf `main` vorhandene WP-015-Datei bleibt ausschließlich Planungsgrundlage und ist kein ausführbarer Trust Anchor. |
| Historische Branches | `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern` und `feat/wp-013-level-v2-pipeline` bleiben ausschließlich lesbare Vergleichskorpora; kein Merge, Cherry-Pick, Rebase, Copy oder Übernahme ihrer PASS-Aussagen. `chore/ci-readiness-unity` bleibt historischer technischer Befund, keine Lieferquelle. |
| Letzter verifizierter Nachweis | WP-023 Phase A auf Branch `codex/wp-023-level-v2-foundation`: gültiger Trust Anchor `a1d284caec7746a4aa48fbe564d37d179d60fed8`, Basis `ad0ac1b6b12f9c0acbf59d90734b3c574a2a15d7`, Manifest unverändert. Architecture Validation PR-Run `37984954679`, Job `114004511519`, **COMPLETED / SUCCESS**; Architecture-only und kanonischer WP-023-Production-Scope jeweils einschließlich Self-/Negativtests erfolgreich. |
| Aktuell zu prüfender Schritt | Phase A ist abgeschlossen. Kein Definitions-, Scope- oder Trust-Blocker. Die automatisch ausgelöste unveränderte Scaffold-Unity-CI ist kein eigener Phase-A-Abschlussnachweis und wird nicht als PASS vorweggenommen. |
| Nächster vorgesehener Schritt | **Separater Implementierungsauftrag für WP-023:** Level-v2 Parser/DTO, Domainabbildung, JCS, Hashverträge, vorgesehene v1→v2-Migration und Content-EditMode-Tests innerhalb des unveränderlichen WP-023-Scopes. Kein Solver-v2/Proof/Generator/Content vorziehen. |
| Produktionscode | Auf `main` integriert: WP-021-Scaffold und WP-022-Puzzle-Kern mit validierter Puzzledefinition, Diagnostik/Completion, immutable Session/Commands/Timer und deterministischem Recovery-Solver-v1. WP-023 hat in Phase A keinen Produktionscode geändert. |
| Architekturentscheidungen | Architecture v1.0; ADR-030 regelt weiterhin den historischen Scope-Trust-Anchor. ADR-031 bleibt Zielvertrag für den späteren Solver-v2-/Proofpfad und wird durch WP-023 nicht vorgezogen. |
| Scope-Vertrauensanker | WP-023 und `tools/architecture-validation/scopes/WP-023.production.scope.json` wurden gemeinsam im ersten Branchcommit `a1d284caec7746a4aa48fbe564d37d179d60fed8` eingeführt. Einziger Elterncommit und Manifest-`baseCommit` sind `ad0ac1b6b12f9c0acbf59d90734b3c574a2a15d7`. Das Manifest bleibt ab diesem Anker byteunveränderlich. |
| CI-Follow-up | `validate.yml` zeigt ausschließlich auf das WP-023-Manifest. Phase-A-Governance-/Scope-/Trust-Nachweis ist PASS. Für Phase B sind eigene Implementierungs-, Test-, Unity-/Build- und Abschluss-QC-Nachweise neu zu erzeugen. |

## Verbindliche Grundlage

Die Quellenhierarchie in [`AGENTS.md`](../AGENTS.md) gilt unverändert. Chatverläufe und Modellidentitäten sind keine Projektquelle. Die vorhandene WP-015-Datei ist historische Planungsgrundlage; der ausführbare aktuelle Auftrag ist WP-023.

## Verbindliche Integrations- und Umsetzungsreihenfolge

1. **WP-021 abgeschlossen und integriert:** Unity-Scaffold freigegeben und formal geschlossen.
2. **WP-022 abgeschlossen und integriert:** Puzzle-Kern unabhängig PASS, integriert über PR #13 und formal geschlossen über PR #14.
3. **WP-023 – Level-v2-/Hash-Foundation:** Phase A abgeschlossen; als Nächstes separate Implementierung, danach unabhängige Abschluss-QC und Integration.
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
| `../WORK_PACKAGES/WP-023_Level-v2-Foundation-und-Hashvertraege.md` | Aktueller ausführbarer Produktionsauftrag; Phase A abgeschlossen. |
| `../tools/architecture-validation/scopes/WP-023.production.scope.json` | Eigenes unveränderliches Scope-Manifest des aktuellen Auftrags. |
| `../WORK_PACKAGES/WP-015_Level-v2-Foundation-und-Hashvertraege.md` | Historische Planungsgrundlage; nicht ausführbarer Trust Anchor. |
| `../WORK_PACKAGES/WP-022_Puzzle-Kern-Recovery.md` | Abgeschlossener, integrierter Puzzle-Kern als technische Basis. |
| `../DECISIONS/ADR-030-wp-scope-trust-anchor.md` | Verbindliche historische Scope-Verankerung. |
| `../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Grenze zum nachfolgenden Solver-v2-/Proofblock. |
| `WORK_QUEUE.md` | Autoritative Priorisierung und Startgates. |

Eine neue Instanz beginnt erneut mit `AGENTS.md`; der aktuelle Repository-Stand ersetzt jeden Chatkontext.
