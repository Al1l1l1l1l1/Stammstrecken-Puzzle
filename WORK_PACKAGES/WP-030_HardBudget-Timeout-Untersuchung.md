# WP-030 – HardBudget-Timeout-Untersuchung

## ID

`WP-030`

Status: verankert, Untersuchung noch nicht begonnen. Enger Untersuchungsauftrag O-02 aus dem Nutzerauftrag vom 15.10.2026; keine Wiederaufnahme oder Erweiterung von WP-024/WP-026/WP-028.

[Production-Scope-Manifest](../tools/architecture-validation/scopes/WP-030.production.scope.json)

## Ziel

Den Originaltimeout auf 96697ab anhand seiner Originalevidenz und gezielter Gegenprüfungen einordnen. Nachgewiesene Ursachen von Hypothesen trennen; konkreten weiter nötigen Korrektur-/Entscheidungsbedarf übergeben, ohne durch eine erfolgreiche Wiederholung Schließung zu behaupten.

## Voraussetzungen

AGENTS-Lesereihenfolge, WP-026/WP-028, Gesamtabnahme b1457ba, ADR-030/031/032, Solverarchitektur, TEST_STRATEGY insbesondere §§4.1/13/15, DoD und Übergaberegeln vollständig beachten. Original-XML/Logs in ci-repair-96697ab-artifact-evidence.json; main-Quellenstand d90e798. Eigener gemeinsamer Erst-Add von WP/Manifest auf f8e1dfc8dc1fdc7846d26060e0b354b6e4a32af8; danach unveränderlich. Keine parallele Untersuchung sichtbar.

## Scope

1. Original-FAIL-XML dekomprimieren, Bytehash/Run/HEAD/Test/Output prüfen; tatsächlichen erfolgreichen Vergleichslauf desselben Heads getrennt auswerten. Gesamtlaufzeit, ausgeführte Fälle, Resultate, Test-/Solverquellgleichheit und übrige Testdauern vergleichen.
2. Einen isolierten reproduzierbaren Diagnoseharness gegen unveränderte Produktionsquellen und den Originaltest erstellen; ausschließlich Scratchkopie um Phasen-, CPU-/Wanduhr- und GC-Messausgaben ergänzen. Keine Originalassertion, Schleife, Anzahl, Grenze oder Fixture ändern. Kalte/warm wirkende Phasen, Zustandsgrenzsuche und sechs Ergebnisarten untersuchen; ergänzende gezielte Kontrollaufrufe von echter Produktion.
3. Tatsächliche aktuelle reguläre CI lesen; keine Wiederholung des alten GitHub-Runs und keine Quarantäne/Flakyfreigabe. Neue Diagnoseergebnisse sind eigene Untersuchung, keine unabhängige Abnahme.
4. Befund, Evidenzgrenzen und konkreten autorisierungsbedürftigen Korrekturbedarf im eigenen Bericht und CURRENT_STATE sichern. Normale Commits/Pushes am PR-21-Lieferzweig erlaubt.

## Betroffene Dateien/Module

Neu: dieses WP/eigenes Manifest; tools/wp030-investigation/run_probe.py; PROJECT_CONTROL/Investigations/WP-030_TIMEOUT.md, WP-030_EVIDENCE.json und WP-030_PROBE.txt. Bestehend: CURRENT_STATE/WORK_QUEUE nur mechanischer Untersuchungsstand; validate.yml nur eigene Manifestzuordnung. Originaltest, Solver, Workflow und alte Berichte ausschließlich lesen.

## Ausdrücklich nicht erlaubte Änderungen

Keine Produktions-, Test-, Fixture-, Timeout-, Budget-, Workflowausführungs-, Runner-, Unity-/Solverversion-, Abhängigkeits-, Architektur- oder Projektregeländerung. Keine Reparatur ohne belegten Bedarf und ausdrücklich passend autorisierten Scope. Kein historischer CI-Retry, keine Quarantäne oder erfolgreiche Wiederholung als Ursachenbeweis. Kein Merge nach main, Import, Release oder WP-026-Gesamtfreigabe.

## Akzeptanzkriterien

AK-01: eigener historischer Anker und enger vollständiger Scope nachgewiesen. AK-02: Originalbefund mit Bytehash, Kopfstand, Testausgabe, Vergleich und Quellgleichheit rekonstruiert. AK-03: gezielte tatsächlich ausgeführte Phasen-/Last-/Kontrollproben reproduzierbar und von CI unterschieden. AK-04: kausale Befunde, Hypothesen und fehlende Telemetrie getrennt; Originalbefund nur bei tatsächlich belegter Ursache/Korrektur geschlossen, andernfalls konkreter enger Folgeauftrag zur Entscheidung. AK-05: eigener Status/CURRENT_STATE, echte CI-/Freezeübergabe und unabhängige Prüfung oder prüffertige externe Übergabe; keine Selbstfreigabe.

## Tests

Architecture-only und eigener Production-Scope mit --self-test; git diff --check; originale XML-Hashprüfung und alle korrespondierenden Testdauern; Originaltest-Solverblobvergleich; Diagnosebuild und Ausführung aller sechs unveränderten Ergebnisfälle, separate Prüfung der Grenzsuche und CPU-/Wall-/GC-Messung. Neue CI am tatsächlichen eingefrorenen Stand lesen. Ein nicht erklärter Originalzustand bleibt offen; kein künstliches PASS.

## Risikoklasse

Mittel: Untersuchung ohne Produktionsänderung; unbelegte Ursachenbehauptung könnte ein hartes Zuverlässigkeitsgate fälschlich schließen.

## Definition of Done

Projektweite DoD gilt. AK einzeln mit tatsächlicher Evidenz bewerten; echte Ursache oder präzise offene kausale Lücke und Entscheidung dokumentieren. Eine vollständig ausgeführte Untersuchung kann eine offene Ursache ergeben; sie ist dann ausdrücklich keine Reparatur/Schließung des WP-026-Zuverlässigkeitsbefunds. Unabhängige Nachprüfung vor technischer Gesamtfreigabe bleibt erhalten.

## Ergebnis und Übergabe – 15.10.2026

Anker 438b45e, Parent/Manifestbasis f8e1dfc, Manifest unverändert. Originalbefund mit Hash und allen sechs Ausgaben rekonstruiert; 37 Testdauern verglichen, Test-/Domain-/Solverblobs identisch. Instrumentierte Scratchprobe hat alle Originalassertionen tatsächlich ausgeführt; hohe Allokationslast auf großen 32×32-Fällen nachgewiesen, historische GC-/CPU-/Schedulingursache nicht bewiesen. Eigene vollständige Evidenz und konkreter diagnostischer Folgeauftrag in PROJECT_CONTROL/Investigations/WP-030_TIMEOUT.md. Keine Reparatur angewandt, O-02 bleibt offen. AK-02/03/04 erbracht im Untersuchungsumfang, AK-01 kanonischer eigener Scope vor Freeze, AK-05 prüffertige externe Nachprüfung/CI nach tatsächlicher Ausführung. Kein Gesamt-DoD-/Freigabe-PASS. Letzter Schritt: kausale Grenze mit gezielten Kontrollen eingegrenzt; nächster Schritt: vorgeschlagenen engen Testdiagnostikauftrag entscheiden und unabhängige Prüfung des Freeze.
