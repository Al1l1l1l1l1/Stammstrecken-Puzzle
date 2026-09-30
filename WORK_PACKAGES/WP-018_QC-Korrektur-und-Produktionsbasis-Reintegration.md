# WP-018 – QC-Korrektur und Produktionsbasis-Reintegration

## ID

`WP-018`

**Bearbeitungsstatus:** Abgeschlossen am 2026-09-30; Governance-Korrektur nach zwei unabhängigen Astra-/Sol-QC-PASS-Berichten, ohne Produktionsimplementierung.

## Ziel

Die beiden unabhängigen QC-HIGH-Befunde zu WP-014 sind verbindlich geschlossen:

1. ADR-031 beschreibt die `minimum: 0`-Regel für Proof-v1 eindeutig als **durch WP-017 herzustellenden Zielvertrag**, nicht als gegenwärtig erfüllte Schemaeigenschaft.
2. Die historischen Branches `feat/wp-008-unity-scaffold` und `feat/wp-009-puzzle-kern` werden nicht mehr als direkt integrierbare Lieferbranches behandelt. Die Produktion erhält mit WP-019 und WP-020 zwei neue, von aktuellem `main` startende Recovery-Work-Packages mit eigenen Trust Anchors, Manifesten, CI-Evidenz und unabhängiger QC.

## Voraussetzungen

1. Vollständige Pflichtlektüre nach [`AGENTS.md`](../AGENTS.md), [`PROJECT_CONTROL/WORK_PACKAGE_RULES.md`](../PROJECT_CONTROL/WORK_PACKAGE_RULES.md), [`PROJECT_CONTROL/DEFINITION_OF_DONE.md`](../PROJECT_CONTROL/DEFINITION_OF_DONE.md), [`PROJECT_CONTROL/CURRENT_STATE.md`](../PROJECT_CONTROL/CURRENT_STATE.md), [`PROJECT_CONTROL/WORK_QUEUE.md`](../PROJECT_CONTROL/WORK_QUEUE.md), [`DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md`](../DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md) und [`PROJECT_CONTROL/WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md`](../PROJECT_CONTROL/WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md).
2. Die unabhängigen QC-Berichte zu Commit `9336dfff5ad3aed1d3155c6eba81a145f5753258` sind als Befundquelle gelesen. Sie besitzen keine Architekturautorschaft; ihre konkret nachprüfbaren Befunde werden hier durch die Geschäftsführung entschieden.
3. Historische Vergleichsquellen sind ausschließlich die Work Packages und Manifeste auf `origin/feat/wp-008-unity-scaffold` sowie `origin/feat/wp-009-puzzle-kern`. Ihre historische Code- und Evidenzlage ist nicht als Integrationserlaubnis zu deuten.
4. Ausgangsbasis ist Commit `9336dfff5ad3aed1d3155c6eba81a145f5753258`. Dieses Work Package und der [WP-018-Dokumentationsscope](../tools/architecture-validation/scopes/WP-018.documentation.scope.json) werden im selben Trust-Anchor-Commit eingeführt und danach byteunverändert belassen.

## Scope

1. Den Zielzeitpunkt in ADR-031, der Quellenklarstellung und den drei Folge-Work-Packages so präzisieren, dass bestehendes `proof-v1.schema.json` bis WP-017 als historische Übergangsform gilt und keine `solver-v2`-Proofproduktion vor WP-017 zulässig ist.
2. Den direkten Integrationsweg der historischen WP-008-/WP-009-Branches ausdrücklich schließen: kein Direktmerge, Cherry-Pick, Rebase, Konfliktauflösung auf diesen Branches oder Scope-Manifest-Umschreiben.
3. WP-019 als vollständigen Recovery-Auftrag für das Unity-Scaffold auf einer **neu von dann aktuellem `main` erzeugten** Branch definieren.
4. WP-020 als vollständigen Recovery-Auftrag für Puzzle-Domain/Solver auf einer **neu von dem durch WP-019 integrierten `main` erzeugten** Branch definieren.
5. WP-015, WP-016 und WP-017 nur in ihren Voraussetzungen, ihrer Basis- und Integrationsbenennung auf die neue Kette korrigieren; ihre technisch-fachlichen Scopes und die ADR-031-Metrikentscheidung nicht verändern.
6. `CURRENT_STATE.md`, `WORK_QUEUE.md`, die Quellenklarstellung und den Workflow auf die korrigierte Gate- und Integrationskette fortschreiben.

## Betroffene Dateien/Module

| Pfad oder Modul | Zulässige Änderung |
|---|---|
| `DECISIONS/ADR-031-solver-v2-metriken-und-proof-regeneration.md` | Zielzeitpunkt und Übergang bis WP-017 präzisieren; keine Metrikdefinition ändern. |
| `PROJECT_CONTROL/WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md` | Historische WP-008/WP-009 als nicht direkt integrierbare Vergleichsquellen und neue Recovery-Kette dokumentieren. |
| `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` | QC-Status, neue harte Reihenfolge und unmittelbare Voraussetzungen konsistent nachführen. |
| `WORK_PACKAGES/WP-015_Level-v2-Foundation-und-Hashvertraege.md`, `WP-016_Solver-v2-Metriken-und-Deduktionsspur.md`, `WP-017_Proof-v1-Regeneration-und-Strict-Validation.md` | Ausschließlich Baseline-/Voraussetzungs- und Integrationsreferenzen korrigieren. |
| `WORK_PACKAGES/WP-018_QC-Korrektur-und-Produktionsbasis-Reintegration.md` | Neu: dieser Auftrag, Trust-Anchor, QC-Befunde und Abschlussnachweise. |
| `WORK_PACKAGES/WP-019_Unity-Scaffold-Recovery-auf-aktuellem-Main.md`, `WORK_PACKAGES/WP-020_Puzzle-Kern-Recovery-auf-aktuellem-Main.md` | Neu: vollständig abgegrenzte Folgeaufträge. |
| `tools/architecture-validation/scopes/WP-018.documentation.scope.json` | Neu: vorab verankerte Dokumentations-Allowlist, nach Anker byteunveränderlich. |
| `.github/workflows/validate.yml` | Ausschließlich Umschaltung des Scope-Laufs auf WP-018. |

## Ausdrücklich nicht erlaubte Änderungen

- Kein C#, keine Unity-, Schema-, Fixture-, Content-, Save-, UI-, Asset- oder Testimplementierung.
- Keine Änderung an `ARCHITECTURE/schemas/proof-v1.schema.json`; dessen `minimum: 0`-Umsetzung gehört ausschließlich zu WP-017.
- Keine neue Produktentscheidung, kein neuer ADR und keine Änderung der in ADR-031 festgelegten Metriksemantik.
- Keine Änderung am WP-014- oder WP-018-Manifest nach ihrem jeweiligen Trust-Anchor-Commit.
- Kein Direktmerge, Cherry-Pick, Rebase, Force Push oder Konfliktauflösung auf den historischen WP-008-/WP-009-/WP-013-Branches.
- Keine Freigabe oder Produktionsdurchführung von WP-019, WP-020, WP-015, WP-016 oder WP-017 vor ihrer jeweiligen eigenen QC.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| `AK-01` | Die historische aktuelle Proof-v1-Schemaform wird als Übergang bis WP-017 beschrieben; der Root-only-`solver-v2`-Proof ist vor WP-017 nicht als schema-valide oder importierbar behauptet. |
| `AK-02` | ADR-031 behält unverändert: alle `solver-v2`-Metriken werden nach Abschluss von WP-017 nichtnegative Ganzzahlen; `proofFormatVersion` bleibt 1; `solver-v2` bleibt verbindlich. |
| `AK-03` | `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern` und `feat/wp-013-level-v2-pipeline` sind jeweils explizit keine Direktmergequelle; ihre historischen Manifeste bleiben unverändert. |
| `AK-04` | WP-019 und WP-020 benennen jeweils neue Main-basierte Branchbasis, eigenes Manifest/Trust Anchor, abschließenden Scope, historische Vergleichsgrenze, objektive Akzeptanzkriterien, Tests, Risikoklasse, DoD und Kimi-/Astra-/Sol-Rollentrennung. |
| `AK-05` | `CURRENT_STATE.md`, `WORK_QUEUE.md`, WP-015, WP-016 und WP-017 verwenden exakt dieselbe harte Kette: Governance (WP-014 + WP-018) → WP-019 → WP-020 → WP-015 → WP-016 → WP-017. |
| `AK-06` | Die Prioritätstabelle nennt für WP-016 und WP-017 ihre unmittelbaren und transitiven Main-Voraussetzungen ohne verkürzende Reihenfolgeaussage. |
| `AK-07` | Workflow, Scope-Manifest, Trust Anchor, vollständiger Git-Diff und Architecture Validator sind konsistent; der Diff enthält nur manifest-erlaubte Governance-Dateien. |

## Tests

1. `git diff --check` gegen den WP-018-Manifest-Basecommit.
2. Documentation-Scope-Validator mit WP-018-Manifest und `--self-test` aus der gepinnten Umgebung.
3. Trust-Anchor-Nachweis: WP-018 und Manifest gemeinsam eingeführt, Manifestblob bytegleich, `baseCommit` = Anker-Elterncommit, historischer WP-Blob enthält den exakten lokalen Manifestlink.
4. Read-only-Ancestry- und `merge-tree`-Nachweis für die historischen WP-008-/WP-009-Branches; Konflikte und historische Scopegrenzen sind in der Wiederherstellungsentscheidung nachvollziehbar.
5. Zwei neue unabhängige QC-Berichte auf demselben eingefrorenen Korrektur-Commit: Astra prüft Zielvertrag/Architektur; Sol prüft Scope, Trust, Integrationsausführbarkeit und Testevidenz.

## Risikoklasse

**Hoch.** Die Korrektur bestimmt, welche historische Produktionsarbeit als Evidenz dienen darf, und schützt die Scope-/CI-Vertrauensgrenze vor einer konfliktbehafteten Schein-Integration.

## Definition of Done

WP-018 ist erst abgeschlossen, wenn alle Akzeptanzkriterien und Tests nachweislich PASS sind, Astra und Sol keine offenen BLOCKER/HIGH-Befunde melden, die Korrekturbranch sauber ist und `CURRENT_STATE.md`/`WORK_QUEUE.md` die Recovery-Kette konsistent wiedergeben. Erst danach darf ein Implementierungsagent mit WP-019 beginnen; dieser Auftrag selbst führt keinen Produktionscode aus.

## Rollen und Übergabe

- **Geschäftsführung / Projektarchitekt:** entscheidet die QC-Abhilfe und definiert WP-019/WP-020 vollständig.
- **Kimi:** keine Rolle in WP-018; darf erst gegen einen vollständig definierten, nach WP-018 freigegebenen Recovery-Auftrag implementieren.
- **Astra-QC:** unabhängige Prüfung der Zielvertrags- und Architekturkonsistenz.
- **Sol-QC:** unabhängige Prüfung von Scope, Trust Anchor, Integrationspfad und Evidenz.
- Astra und Sol prüfen ohne Einsicht in die Ergebnisse des jeweils anderen Reviewers.

## Abschlussnachweis

- Der WP-018-Trust Anchor `7226cbd6e76c0ddca5ff81dd9eeb211eec2ea61f` hat den korrekten Basecommit `9336dfff5ad3aed1d3155c6eba81a145f5753258`; der Manifestblob blieb bis zum geprüften Head byteunverändert.
- Der Documentation-Scope-Validator mit `--self-test` hat für den Korrektur-Head `7b65c0cf70ba9e9ead65df8a749f9324ae6ebc30` 18 lokale Prüfgruppen PASS gemeldet; `git diff --check` war sauber.
- Astra und Sol haben den eingefrorenen Korrektur-Head unabhängig mit **PASS** und ohne offene BLOCKER/HIGH-Befunde freigegeben.
- [PR #6](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/pull/6) wurde als Merge-Commit `7b0d65ac01334f2457e3a47f3b79ca0b5e1e9542` nach `main` integriert. Die Trust-Anchor-Commits bleiben dadurch in der Main-Historie erhalten.
- Nächste zulässige Arbeit ist ausschließlich WP-019 auf einer neuen Main-basierten Production-Branch. Fehlende Unity-Lizenz oder Runner bleiben gemäß WP-019 ein Start-/Abschlussblocker; WP-020 bis WP-017 bleiben gesperrt.
