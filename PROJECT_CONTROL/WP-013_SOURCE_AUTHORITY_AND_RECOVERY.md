# WP-013 – Quellenautorität, Auditkorrektur und Wiederanlauf

**Status:** Verbindliche Governance-Klarstellung
**Datum:** 2026-09-30
**Geltung:** Für alle Folgearbeiten zu `WP-013`, Solvermetriken, `proof-v1`, `solver-v2` und deren Integration nach `main`.

## 1. Anlass

Ein vorheriger Auditbericht verwendete für die Aussage zu `maxDeductionDepth` die Formulierung „vorhandene technische Entscheidungsgrundlage“. Diese Formulierung war nicht hinreichend präzise. Sie wird durch diese Datei berichtigt.

## 2. Nichtautoritative Quelle

Die damals gemeinte Quelle war **ein späterer separater Manus-Chat** mit der Task-ID `VYxI5smHMcpqhcg4j6XoF6`. Dort hatte eine frühere Instanz am 2026-09-30 einen eigenständigen technischen Befund zu `searchNodes` und `maxDeductionDepth` formuliert und eine Datei `entscheidung_wp013_solvermetriken.md` ausgegeben.

Dieser Chat gehörte nicht zum rekonstruierten ursprünglichen Projektkontext, wurde nicht als ADR oder Repository-Artefakt persistiert und ist daher gemäß [`AGENTS.md`](../AGENTS.md) Abschnitt 2 **keine verbindliche Projektquelle**. Seine Aussagen werden nicht als Entscheidung, Begründung oder Autorität in ADR-031 verwendet. Sie dürfen nur als historischer Hinweis auf eine zu prüfende Fragestellung behandelt werden.

## 3. Autoritative Neubewertung

Die Neubewertung stützt sich ausschließlich auf folgende persistente Quellen sowie auf die am 2026-09-30 durch den verantwortlichen Projektarchitekten getroffene und in ADR-031 dokumentierte Entscheidung:

| Quelle | Autoritative Aussage für die Neubewertung |
|---|---|
| [`Stammstrecken_Puzzle_Konzept_00-15/04_Schwierigkeit_Feldgroessen_und_Levelgenerierung.md`](../Stammstrecken_Puzzle_Konzept_00-15/04_Schwierigkeit_Feldgroessen_und_Levelgenerierung.md) | Schwierigkeit entsteht aus nachvollziehbaren Schlussketten; normale Kampagnenlevel dürfen kein willkürliches Raten verlangen. |
| [`DECISIONS/ADR-007-solver-und-eindeutigkeitspruefung.md`](../DECISIONS/ADR-007-solver-und-eindeutigkeitspruefung.md) | Deterministische Suche, erklärbare Deduktionen, Proofmetriken und Versionspflicht bei Metrikänderungen. |
| [`DECISIONS/ADR-021-puzzleidentitaet-und-proofartefakte.md`](../DECISIONS/ADR-021-puzzleidentitaet-und-proofartefakte.md) | Proof ist ein regenerierbares Cacheartefakt; ein neuer Solver darf einen neuen Proof erzeugen, ohne Puzzleidentität zu ändern. |
| [`ARCHITECTURE/SOLVER_ARCHITECTURE.md`](../ARCHITECTURE/SOLVER_ARCHITECTURE.md) | Jede Reduktion benötigt Regelcode, Prämissen und Konklusion; gespeicherte Proofs sind in CI neu zu berechnen; `solver-v1` umfasst Metrikdefinitionen. |
| [`ARCHITECTURE/LEVEL_DATA_FORMAT.md`](../ARCHITECTURE/LEVEL_DATA_FORMAT.md) | Gleiches Puzzle und gleiche Solverversion müssen bytegleichen Proof ergeben; eine neue Solverversion darf Proofs erneuern. |
| [`ARCHITECTURE/CONTENT_PIPELINE.md`](../ARCHITECTURE/CONTENT_PIPELINE.md) | Gemeinsame Pipeline für GUI, CI und Build; Proofregeneration ist Pflicht; `--strict` behandelt Warnungen als Fehler. |

## 4. Endgültige Einordnung von WP-013

Der historische Branch `origin/feat/wp-013-level-v2-pipeline` auf Commit `21fb4dbaafbdb1c7946408085f92a4f131f97f22` bleibt **ein nicht integrierter Prototyp und keine Mergequelle**. Seine brauchbaren Teilideen dürfen in den klar abgegrenzten Folge-Work-Packages nur nach erneuter Prüfung übernommen werden.

Der Branch wird nicht direkt nach `main` gemergt, weil:

1. sein `baseCommit` `767c01e…` kein Vorfahr von `origin/main` ist;
2. die Merge-Simulation Konflikte in `.github/workflows/validate.yml` und `PROJECT_CONTROL/CURRENT_STATE.md` zeigte;
3. sein Proofvertrag und seine Pipeline die in ADR-031 entschiedenen Eigenschaften nicht erfüllen.

## 5. Verbindlicher Wiederanlauf

1. Die historischen Branches `WP-008` und `WP-009` sind wegen nicht vorfahriger Basen, Konflikten und unveränderlichen historischen Scope-Manifests keine Direktmergequellen. Sie bleiben lesbare Vergleichskorpora.
2. Die vorab auf `main` dokumentierten WP-019/WP-020 sind wegen fehlender gemeinsamer Erstverankerung mit ihren eigenen Manifesten archivierte Planungsunterlagen und nicht ausführbar.
3. Nach der Trust-Anchor-Korrektur wird WP-021 das Unity-Scaffold und danach WP-022 den Puzzle-Kern jeweils erst gemeinsam mit einem eigenen Manifest auf einer frischen Main-basierten Branch verankern und wiederherstellen.
4. `WP-015`, `WP-016` und `WP-017` bauen erst nach integrierten WP-021 und WP-022 auf `main` auf; sie übernehmen keinerlei Autorität aus dem separaten Chat oder dem WP-013-Prototyp.
5. Die Architekturentscheidung ist ausschließlich [`ADR-031`](../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md).
6. Astra und Sol prüfen die Ergebnisse der jeweiligen Work Packages unabhängig und ohne gegenseitige Abstimmung. Kimi erhält ausschließlich die fertig definierten Arbeitsaufträge und keine Entscheidungsbefugnis.

## Referenzen

[1]: ../AGENTS.md "Verbindliche Agentenleitlinie"
[2]: ../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md "ADR-031 – Solver-v2-Metriken und Proofregeneration"
[3]: ../PROJECT_CONTROL/WORK_QUEUE.md "Produktionswarteschlange"
