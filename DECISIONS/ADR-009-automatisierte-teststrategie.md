# ADR-009 – Automatisierte Tests als verbindliche Architekturgrenze

## Status

**Ersetzt**

## Datum

2026-09-07

## Kontext

Der Product Owner prüft Code nicht selbst. Wechselnde Agenten benötigen schnelle, deterministische Qualitätsnachweise. Risiken liegen besonders in Puzzlelogik, Eindeutigkeit, Datenmigration, Belohnungsidempotenz, externen SDKs und mobilen Releasebuilds.

## Entscheidung

Die Teststrategie ist pyramidenförmig und risikobasiert. Reine Domain-, Solver-, Migrations- und Policy-Tests bilden die breite Basis als Unity EditMode-Tests. Property-, Metamorphic-, Golden- und Mutationstests ergänzen Beispieltests. PlayMode-Tests prüfen Composition Root, UI-Zustände, Szenen und lokale Adapter. Geräte-Smoke-Tests prüfen Android und iOS sowie echte Sandbox-SDKs.

Jeder Fehler erhält vor oder mit der Korrektur einen reproduzierenden Test. Zufallstests verwenden protokollierte feste Seeds. Flaky Tests werden nicht blind wiederholt; sie blockieren den Merge bis zur Behebung oder ausdrücklich dokumentierten Quarantäne mit Eigentümer und Frist.

Verbindliche Merge-Gates sind Kompilierung ohne eigene Warnungen, Domain-/Solver-/Contract-/Migrations-Tests, Levelschema und semantische Validierung, Assembly-Grenzprüfung sowie mindestens ein Android-Development-Build. Release-Gates ergänzen iOS- und Android-IL2CPP-Builds, Gerätetests, Store-Preflight, Symbolprüfung und den vollständigen kuratierten Levelkatalog.

Testcode darf Production Contracts verwenden, aber Produktionscode enthält keine Testsonderpfade. Externe Dienste werden über dieselben Ports mit Fakes getestet.

## Begründung

Die meisten Fehler werden schnell und ohne Gerät entdeckt; integrationsnahe Risiken bleiben durch kleinere höhere Schichten abgedeckt. Property- und Metamorphic-Tests eignen sich besonders für kombinatorische Puzzle- und Migrationsregeln.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Hauptsächlich manuelles QA | Nicht ausreichend reproduzierbar und für Agentenübergaben ungeeignet. |
| Nur End-to-End-Tests | Langsam, fragil und schlechte Fehlerlokalisierung. |
| Nur Coverage-Ziel | Hohe Abdeckung beweist keine relevanten Invarianten. |
| Snapshot-Tests für jede UI | Zu änderungssensitiv; gezielte Zustands- und wenige Visual-Regression-Tests sind besser. |

## Konsequenzen

Testdaten, Fakes und Builder sind erstklassige Module. CI-Kosten und Gerätezugang sind einzuplanen. Coverage wird berichtet, ist aber kein Ersatz für Invarianten; Mindestwerte werden erst nach dem Produktions-Scaffold festgelegt.

## Betroffene Artefakte

`ARCHITECTURE/TEST_STRATEGY.md`, `ARCHITECTURE/BUILD_AND_RELEASE.md` und alle späteren Testassemblies.

## Validierung

Ein absichtlich eingebauter Vertragsbruch muss jedes definierte Gate nachweislich fehlschlagen lassen. Release-Kandidaten speichern Test-, Build-, Katalog- und Symbolnachweise als Artefakte.

## Ersetzt / ersetzt durch

Ersetzt keinen früheren ADR. Ersetzt durch [ADR-017](./ADR-017-versionierter-architekturvalidator.md).

## Referenzen

[1]: ../PROJECT_CONTROL/DEFINITION_OF_DONE.md "Definition of Done für technische Aufgaben"
[2]: ../AGENTS.md "Verbindliche Agentenleitlinie"
