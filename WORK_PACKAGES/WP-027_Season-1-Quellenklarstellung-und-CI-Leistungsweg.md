# WP-027 – Season-1-Quellenklarstellung und CI-Leistungsweg

## ID

`WP-027`

**Status:** In Bearbeitung. Dieses Paket trifft und persistiert die ausdrücklich angeforderte Geschäftsführungsentscheidung zu den zwei eng begrenzten Content-Bible-Auslegungen und beschließt den technischen CI-Leistungsweg. Es erstellt den verbindlichen Ausführungsbrief für das nachfolgende, noch nicht gestartete WP-028, aber **nicht** dessen eigenen Trust Anchor. Es ersetzt weder den bestehenden WP-026-Umfang noch seinen unveränderlichen Scope.

[Governance-Scope-Manifest](../tools/architecture-validation/scopes/WP-027.governance.scope.json)

## Ziel

Die zwei offenen, auslegungsbedingten Befunde aus WP-026 sind durch eine ausdrücklich bestätigte und in der zuständigen Content-Bible fortgeschriebene Produktentscheidung beendet. Der fehlende Leistungsnachweis des 240er-Bestands erhält einen separat verankerten, technisch überprüfbaren CI-Ausführungsweg. Die Grenze zwischen bestehendem PR #21, diesem Governancepaket und dem nachfolgenden CI-Paket bleibt nachvollziehbar und unvermischt.

## Voraussetzungen

1. Vollständige Lesereihenfolge aus [AGENTS.md](../AGENTS.md): Projektübergabe 15, Index 00, Master 09, CURRENT_STATE und dieses WP; anschließend Konzeptbausteine 03, 04 und 14, WORK_QUEUE, WORK_PACKAGE_RULES, DEFINITION_OF_DONE, CONTENT_PIPELINE, SOLVER_ARCHITECTURE, BUILD_AND_RELEASE sowie ADR-030 und ADR-031.
2. Die ausdrücklich angeforderte heutige Geschäftsführungsentscheidung des Auftraggebers zur Relevanz und Lösung der offenen WP-026-Punkte. Sie ist die neue bestätigte Produktentscheidung im Sinn von AGENTS.md §3; weder ein späterer separater Chat noch Prüfermeinungen ersetzen sie.
3. Rekonstruierter ursprünglicher Hauptchat als **Kontextbeleg, nicht als selbstständige Repositoryautorität**: die bestätigten Ursprungsentscheidungen zur Schwierigkeitslogik (Denkstruktur aus Randzahlen, A/B-Lage, Dichte, Schlussketten und Geometrie; keine starre Einzelzahlregel) sowie zum 240er-Kampagnenrahmen. Die dauerhaft maßgeblichen Fassungen dieser Entscheidungen liegen in Konzept 04 und Content-Bible 14 vor.
4. WP-026 / Draft-PR #21 auf `d97c39d03f59a2dbc34930f14e3cb74ebeb2e118` ist ein getrennter, noch offener Lieferstand. Sein Anker `f9231c7…`, Basis `c9516bd…` und Manifest-SHA256 `981cd37f45c0bb73106d321ad450fbf8ad8e210a508b8a177d25892058ef018c` bleiben unverändert.
5. Der gemeinsame Anker dieses Pakets entsteht von `c9516bdbd5858fd9003d5207ec37d180ccc6ae23` aus. WP-028 erhält erst auf seinem eigenen kindlichen Branch von `d97c39d03f59a2dbc34930f14e3cb74ebeb2e118` seinen gemeinsamen WP-/Manifestanker; der Ausführungsbrief dieses Pakets darf nicht als solcher Anker missverstanden werden.

## Scope

1. Quellenlage und Befunde aus dem ursprünglichen Produktkontext, den maßgeblichen persistenten Quellen und dem aktuellen 240er-Prüfstand gegeneinander abgleichen; Prüferbefunde bleiben Evidenz, nicht Produktautorität.
2. Die beiden Product-Interpretationen verbindlich in Content-Bible 14 konkretisieren:
   - Route 1, Positionen 9–10: „erste unterschiedliche A/B-Kanten“ bedeutet die erste **gezielte räumliche Variationslektion** der Route, nicht ein rückwirkendes Verbot unterschiedlicher Außenkanten in Positionen 1–8.
   - Route 18, Positionen 3–5: „mehrere mittlere Hinweise als einziges Orientierungsmittel“ bezeichnet die erste produktive Deduktionsphase. Ein durch einen Endpoint unmittelbar erschöpfter Einsercount darf nur den lokalen Eintritt zeigen; die restliche Strecke muss aus der Kombination mehrerer mittlerer Restzahlen folgen. Daraus entstehen weder eine numerische Einserquote noch ein allgemeines Einserverbot.
3. ADR-032 als technische Entscheidung für den CI-Referenznachweis schreiben und im ADR-Index registrieren.
4. Den vollständigen verbindlichen WP-028-Ausführungsbrief erstellen. Erst die spätere Ausführungsrolle kopiert ihn auf dem kindlichen PR-21-Branch in `WORK_PACKAGES/`, fügt dort gleichzeitig ihr eigenes Production-Scope-Manifest hinzu und verankert beides gemeinsam. Es darf erst nach erfolgreicher Integration dieses Pakets ausgeführt werden.
5. CURRENT_STATE und WORK_QUEUE auf die beschlossene Reihenfolge fortschreiben. Den 240er-Lieferstand, seine offenen menschlichen und Fremdcontent-Gates sowie den dokumentierten Unity-Timeout nicht wegschreiben oder als erfüllt ausgeben.
6. Die Architektur-/Scopeprüfung und eine unabhängige Fach- und Integrations-QC auf exakt demselben eingefrorenen Head ausführen; keine Produktionsimplementierung, kein Contenttransfer und kein Merge dieses oder eines fremden Pakets.

## Betroffene Dateien/Module

Abschließende Außengrenze ist das oben verlinkte Manifest. Zulässig sind ausschließlich:

| Pfad | Funktion |
|---|---|
| `WORK_PACKAGES/WP-027_Season-1-Quellenklarstellung-und-CI-Leistungsweg.md` **neu** | Auftrag, Entscheidungsbegründung, Nachweise und Abschlussstatus dieses Pakets. |
| `tools/architecture-validation/scopes/WP-027.governance.scope.json` **neu** | Unveränderlicher, produktquellenfähiger Governance-Scopeanker für WP-027. |
| `Stammstrecken_Puzzle_Konzept_00-15/14_Season_1_Content_Bible.md` | Zuständige persistente Produktquelle für die zwei Klarstellungen. |
| `DECISIONS/ADR-032-season1-ci-korpus-leistungsnachweis.md` **neu**, `DECISIONS/README.md` | Technische CI-Entscheidung und Index. |
| `PROJECT_CONTROL/WP-028_EXECUTION_BRIEF.md` **neu** | Verbindlicher Ausführungsbrief für den noch nicht gestarteten Folgeauftrag; kein vorgezogener WP-028- oder Manifestanker. |
| `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` | Mechanische Fortschreibung von Entscheidungsstand, Reihenfolge und Gates. |
| `tools/architecture-validation/validate.py`, `tools/architecture-validation/scope-manifest-v1.schema.json` | Eng begrenzte Registrierung des Governance-Scopes und von ADR-032; keine Produktionssemantik. |

## Ausdrücklich nicht erlaubte Änderungen

- Keine Änderung des bestehenden WP-026, seines Manifests, seiner Branch, seiner 240 Levelquellen, Proofs, Reviews oder der in PR #21 belegten Historie.
- Keine Änderung an Puzzle-, Solver-, Proof-, Hash-, Schema- oder Strict-Semantik; keine neue Metrik, Schwierigkeitsschwelle, Einserquote, Portfolge oder Qualitätsgrenze.
- Keine Produktionsimplementierung des WP-028, keine Änderung an `Assets/`, `Content/`, `.github/workflows/` oder technischen Testdateien.
- Kein Merge, Rebase, Cherry-Pick, Runtimeimport, Release, Release-Lock, Produktstatuswechsel oder Gesamt-DoD-PASS.
- Keine künstliche Aufhebung verbleibender WP-026-Gates: menschlicher Spieltest, unabhängige Abnahme nach Freeze, fehlende Referenzziele, Zeitklassenzuordnung, Import-/Releasegates und dokumentierter Unity-Timeout bleiben getrennt offen.

## Akzeptanzkriterien

| ID | Kriterium |
|---|---|
| AK-01 | Die Quellenhierarchie ist schriftlich getrennt: Originalchat nur Kontext, Content-Bible als zuständige persistente Produktquelle, heutige Geschäftsführungsentscheidung als bestätigende neue Produktentscheidung, Prüferberichte als Evidenz. |
| AK-02 | Content-Bible 14 beseitigt beide Mehrdeutigkeiten ohne neue Zahlen-/Metrik-/Portregeln; die vorhandenen Route-1- und Route-18-Inputs werden dadurch in ihrer dokumentierten Rolle eindeutig beurteilt. |
| AK-03 | ADR-032 ist vollständig, angenommen, im Index registriert und legt einen nachweisbaren, fail-closed CI-Referenzweg fest, ohne Solversemantik oder Budgets zu ändern. |
| AK-04 | Der vollständige WP-028-Ausführungsbrief erzwingt seinen späteren gemeinsamen Work-Package-/Manifestanker auf `d97c39d03f59a2dbc34930f14e3cb74ebeb2e118`, eine Unter-PR nur zum WP-026-Lieferzweig und ausschließlich die nötige CI-/Testimplementierung. Kein künftiger Anker wird in WP-027 vorweggenommen. |
| AK-05 | CURRENT_STATE und WORK_QUEUE stellen klar: WP-026 bleibt offen; keine semantische Eigentümerentscheidung und kein zusätzlicher CI-Weg sind mehr unbestimmt, aber die tatsächliche Ausführung, unabhängige Neuabnahme und alle weiteren Pflichtgates stehen weiterhin aus. |
| AK-06 | Der vollständige Diff liegt innerhalb des WP-027-Manifests; Architecture-only, WP-027-Scope, Git-Whitespace und ADR-/Statuskonsistenz bestehen. |
| AK-07 | Zwei unabhängige QC-Rollen beurteilen auf demselben Freeze getrennt die Produkt-/Quellen- und die Integrations-/Scope-Seite. Kein HIGH- oder BLOCKER-Befund bleibt offen. |

## Tests

1. `python tools/architecture-validation/validate.py --self-test`.
2. `python tools/architecture-validation/validate.py --scope governance --scope-manifest tools/architecture-validation/scopes/WP-027.governance.scope.json --self-test`.
3. `git diff --check c9516bdbd5858fd9003d5207ec37d180ccc6ae23 --` sowie Prüfung von Anker, Parent, Manifestbytegleichheit und realer Diffmenge inklusive untracked Dateien.
4. Manuelle Gegenprobe der Route-1-Inputs S1-01-01-01 bis -10 und der Route-18-Inputs S1-05-02-03 bis -05 gegen die neue Bible-Fassung; keine neue technische Invariante daraus ableiten.
5. Zwei voneinander getrennte unabhängige QC-Berichte auf demselben eingefrorenen Head: Astra für Quellen-/Produktentscheidung, Sol für ADR-/Trust-Anchor-/Integrationsfolge.

## Risikoklasse

**Hoch.** Die Entscheidung präzisiert das didaktische Produktversprechen und etabliert einen dauerhaften CI-Nachweisweg. Falsche oder zu enge Regeln könnten den vorhandenen Content unberechtigt verwerfen oder neue, nicht bestätigte Qualitätsbarrieren einführen.

## Definition of Done

Die projektweite [Definition of Done](../PROJECT_CONTROL/DEFINITION_OF_DONE.md) gilt vollständig. Dieses Paket ist erst abgeschlossen, wenn AK-01 bis AK-07 einzeln belegt, die beiden QC-Berichte auf demselben Freeze ausgewertet und alle verbleibenden WP-026-Gates ehrlich fortgeschrieben sind. Der Ausführungsbrief für WP-028 ist kein Work-Package- oder Manifestanker und kein Beginn oder Abschluss seiner Implementierung. Die Integration dieses Governancepakets und jeder Contenttransfer benötigen weiterhin die jeweils dokumentierte Prüf- und Freigabereihenfolge.
