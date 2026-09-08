# ADR-017 – Versionierter reproduzierbarer Architekturvalidator

## Status

**Angenommen**

## Datum

2026-09-08

## Kontext

ADR-009 hat automatisierte Tests korrekt als Architekturgrenze festgelegt. Der Abschluss von Architecture v0.1 verwies jedoch auf ein absolutes Sandboxskript außerhalb des Repositorys. Spätere Agenten konnten den Nachweis aus einem sauberen Checkout nicht reproduzieren. Der Prüfer akzeptierte außerdem die inzwischen auf `WP-001` korrigierte, ehemals regelwidrige Work-Package-ID als PASS.

## Entscheidung

Die risikobasierte Testpyramide, Contracttests, Property-/Metamorphic-Tests, fehlende automatische Retries und mobile Release-Gates bleiben erhalten. Zusätzlich ist `tools/architecture-validation/` die einzige autoritative Implementierung der Dokument-/Architekturabnahme.

Das Verzeichnis enthält:

- `README.md` mit Plattformvoraussetzungen, Setup und exakt einem kanonischen Ausführungsbefehl;
- `requirements.lock.txt` mit exakt gepinnten Python-Abhängigkeiten;
- `validate.py` als Einstiegspunkt;
- `jcs_crosscheck.mjs` als unabhängige zweite JCS-/Hashimplementierung;
- positive und negative Fixtures beziehungsweise programmgesteuerte Mutations-Selbsttests.

Der Validator läuft ausschließlich gegen Repositorypfade relativ zu seiner eigenen Position. Er darf keine absoluten, Manus- oder nutzerspezifischen Pfade voraussetzen. Er prüft mindestens Inventar, Work-Package-IDs, ADR-Index und Superseding, relative Links, JSON/Schema, Levelsemantik und Eindeutigkeit, Katalogreferenzen, JCS-Goldens, Save-Hashvertrag, Assemblygraph, Reward-/IAP-/Privacy-/Deviceverträge, Architecture-v0.2-Versionen sowie Diff-/Produktguardrails. Ein absichtlich mutiertes Fixture für jeden Reviewpunkt muss die zugehörige Prüfung nachweislich fehlschlagen lassen.

Ein dokumentierter PASS ist nur gültig, wenn der kanonische Befehl mit Exitcode 0 endet und der Bericht die Anzahl bestandener Prüfgruppen ausgibt. Fehlende Abhängigkeit, übersprungener Selbsttest oder unbekannte Regel ist kein PASS. Der Validator ist Governance-/Entwicklungstooling und kein Spielproduktionscode.

## Begründung

Ein eingecheckter Einstiegspunkt macht den Architekturbeleg aus jedem sauberen Checkout reproduzierbar. Gepinnte Abhängigkeiten und eine unabhängige zweite Hashimplementierung verringern Umgebungs- und Common-Mode-Risiken. Negative Selbsttests verhindern Prüfer, die nur immergrüne Erfolgsmeldungen erzeugen.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Temporäres Sandboxskript | Nicht übergabefähig und nicht aus dem Repository reproduzierbar. |
| Nur CI-Workflow ohne lokales Tool | Erschwert lokale Diagnose und bindet den Vertrag an einen Anbieter. |
| Nur manuelle Checkliste | Kann maschinenprüfbare ID-, Schema-, Hash- und Graphfehler übersehen. |
| Ungepinnte Python-Abhängigkeiten | Spätere API-/Verhaltensänderungen könnten den Nachweis verändern. |

## Konsequenzen

Jede relevante Governance- oder Vertragsänderung muss Validator und Selbsttests im selben Work Package aktualisieren. Das Tool darf nicht still an fehlerhafte Dokumente angepasst werden. Der erste Produktions-Scaffold integriert denselben Befehl in Pull-Request-CI.

## Betroffene Artefakte

`tools/architecture-validation/`, `ARCHITECTURE/TEST_STRATEGY.md`, `ARCHITECTURE/BUILD_AND_RELEASE.md`, `PROJECT_CONTROL/DEFINITION_OF_DONE.md`, Work Packages und spätere GitHub-Actions-Workflows.

## Validierung

Ein sauberer Checkout erstellt eine frische virtuelle Umgebung, installiert ausschließlich `requirements.lock.txt` und führt `python tools/architecture-validation/validate.py --self-test` aus. Der Befehl muss bei intaktem Repository 0 und nach jeder vorgesehenen Mutation ungleich 0 liefern.

## Ersetzt / ersetzt durch

Ersetzt [ADR-009](./ADR-009-automatisierte-teststrategie.md). Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: ../tools/architecture-validation/README.md "Architecture Validation Tool"
[2]: ../ARCHITECTURE/TEST_STRATEGY.md "Test Strategy v0.2"
[3]: ../WORK_PACKAGES/WP-002_Architecture-v0.2-Korrekturen.md "WP-002 – Architecture-v0.2-Korrekturen"
