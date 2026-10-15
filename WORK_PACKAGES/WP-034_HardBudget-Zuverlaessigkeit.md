# WP-034 – HardBudget-Zuverlässigkeit

## ID

`WP-034`

Status: Prospektiv beauftragt, vor Implementierung verankern; keine Ursachen- oder Abschlussbehauptung.

[Eigenes Production-Scope-Manifest](../tools/architecture-validation/scopes/WP-034.production.scope.json)

## Ziel

O-02 anhand originaler FAIL/PASS-Evidenz und regulärer Unity-CI kausal untersuchen und eine nachgewiesene vermeidbare Ursache im bestehenden Solver-/Testpfad beheben. Fachliche Budgetaussagen und solver-v2-Ausgaben bleiben erhalten; historische unbeobachtete Hostumstände werden nicht erfunden.

## Voraussetzungen

Ausdrücklicher Nutzerauftrag O-02 vom 15.10.2026 autorisiert Folgepaket, Diagnose, ursachenbasierte Reparatur, normale Commits/Pushes und Draft-PR, keine Main-Integration. AGENTS-Lesereihenfolge, WP-026/028/030/031/032/033, aktuelle Übergaben, WORK_QUEUE/WORK_PACKAGE_RULES/AI_HANDOVER_RULES/DoD, ADR-007/026/030/031/032, SOLVER_ARCHITECTURE, TEST_STRATEGY, Validator-README und tatsächliche Solver-/Testquellen. Basis genau a2e26a658a2bcdd47002698c150cb5736d9760da; remote main d90e798, PR21 offen/Draft. Sauberer Checkout und keine andere aktive Projektbearbeitung bei Übernahme. Eigenen Branch codex/wp-034-hardbudget-reliability und gemeinsamen Erst-Add-Anker mit einzigem Parent gleich Manifestbasis vor fachlicher Änderung erstellen; Manifest danach byteunveränderlich.

## Scope

1. Originalruns 38668535139/38668540223 sowie WP-030-FAIL/PASS mit tatsächlichen Checkout-SHAs, XMLs, vollständigen Joblogs und Artefakthashes vergleichen; vorhandene Nachweise und laufende Arbeit verifizieren.
2. Im bestehenden HardBudget-Test reguläre Phasen-/CPU-/GC-/Speicherdiagnostik außerhalb der gemessenen Nachlauf-/Kopierintervalle ergänzen. Baseline mit unveränderter Produktion in echter CI erfassen. Nicht verfügbare Allokationszähler ausdrücklich kennzeichnen. Ergänzende reproduzierbare Scratchprofile und kontrollierte Alt-/Neu-Vergleiche sind Diagnose, kein CI-Ersatz.
3. Bei belegtem Aufwand ausschließlich mechanisch äquivalente Optimierungen in RootDeductionTracer.Record/View/Extent oder RecoveryReduction-/Ergebnisaufbereitung in PuzzleSolver durchführen. Keine Änderung von Constraint-/Prämissenwahl, Löschreihenfolge, Traceform, Metriken, Suchfolge, Budgetprüfreihenfolge oder öffentlichen APIs. Falls diese Invarianz nicht belegbar ist: konkrete ADR-/Scopegrenze dokumentieren, abhängige Änderung unterlassen, zulässige Diagnose abschließen.
4. Vor Produktionskorrektur passende Regressionen im bestehenden Solver-Testmodul und reproduzierbare Alt-/Neu-Differentialprobe festlegen/ausführen. Alle sechs Originalfälle, zwei Warm-ups, 25 Samples, Grenzsuche, 40%-Assertion, 180-Sekunden-Testgrenze und übrige Budgetprüfungen erhalten. Vermeidbare Arbeitslast beseitigen, keine Retries/Quarantäne/Lockerung.
5. Vollständige relevante Solver-/Domain-/Contentregressionen, alle 240 Proofbytes/Rootspuren/Metriken, Strict/Mutation/Metamorphose, Architecture/eigener Scope mit Selftests und reguläre Unity-CI am tatsächlichen Freeze prüfen. ALT-/NEU- und Mutationsbefunde an tatsächliche Quellen binden. Bekannte historische FAILs bleiben FAIL.
6. Unabhängige technische Abschlussrolle auf eingefrorenem HEAD tatsächlich getrennt ausführen lassen (Subagent ausdrücklich für diese Prüfrolle autorisiert), Befunde im Scope korrigieren und betroffene Nachweise erneuern. Keine Selbstfreigabe; falls unabhängige Ausführung extern blockiert, vollständige prüffertige Übergabe.
7. Ergebnisse, AK/DoD, Originalbelege und Grenzen dauerhaft im eigenen Bericht sowie aktuellem WP/Steuerungsstatus dokumentieren; eigene normale Draft-Unter-PR ausschließlich gegen WP026, keine automatische Integration. Alte Paketdefinitionen/Manifeste/Originalberichte unverändert.

## Betroffene Dateien/Module

Neu: dieses WP; eigenes Manifest; tools/wp034-hardbudget/*.py, *.cs, *.md als reproduzierbare Diagnose-/Differentialhilfen; PROJECT_CONTROL/Investigations/WP-034*.md, *.json, *.txt als eigene Originalevidenz-/Prüf-/Übergabedateien. Bestehend: Assets/StammstreckenPuzzle/Scripts/Puzzle/Solver/RootDeductionTracer.cs und PuzzleSolver.cs ausschließlich äquivalente Aufwandskorrekturen nach Scope3; Assets/StammstreckenPuzzle/Tests/EditMode/Solver/PuzzleSolverTests.cs und SolverV2MetricsTests.cs für Diagnose und Regressionen; .github/workflows/validate.yml nur aktive Manifestzuordnung; PROJECT_CONTROL/CURRENT_STATE.md und WORK_QUEUE.md sowie WORK_PACKAGES/WP-026_Season-1-Logischer-Raetselbestand.md ausschließlich aktuelle Ergebnis-/Statusfortschreibung. Scratch und Downloads im ignorierten .utmp/wp034. Andere Quellen ausschließlich lesen.

## Ausdrücklich nicht erlaubte Änderungen

Keine Main-Integration, Runtimeimporte, Release-/Gesamtfreigabe, neue Produkt-/Architekturentscheidung, neue Solverversion oder stiller Wechsel der solver-v2-Semantik. Keine Änderungen alter Manifeste/Definitionen, Konzepte, ADRs, Architektur/Teststrategie/Validator, Schema, Level-/Proof-/Rootartefakte oder Authoringlogik. Keine Änderung von Unity-/Runner-/Coverage-/Lizenz-/Workflowausführung, Testzeitgrenzen, Budgets, Originalfall-/Warm-up-/Samplezahl, Grenzsuche oder Assertionstärke; keine Retries, Quarantäne oder bloße Wiederholung als Reparaturnachweis. Keine Wiederaufnahme abgeschlossener anderer Nacharbeiten.

## Akzeptanzkriterien

AK-01 eigener gemeinsamer historischer Anker/Parent/Manifestlink, unverändertes Manifest und vollständiger erlaubter Diff nachgewiesen. AK-02 aktuelle Original-FAIL/PASS-XMLs/Logs/Checkoutstände/Hashes ausgewertet, belegte Ursache/Hypothesen/historisch nicht rekonstruierbare Umstände getrennt. AK-03 reguläre unveränderte Baseline mit neuer Diagnose und kontrollierter kausaler Vergleich einer zulässigen Korrektur; kein zufälliges Grün als Beweis. AK-04 relevante Fehlerbehauptung regressions-/mutationsgesichert, alle ursprünglichen Budgetaussagen erhalten, semantische Ausgabeidentität einschließlich Trace/Prämissen/Metriken/240 Proofs belegt. AK-05 erforderliche vollständige lokale und echte CI-Prüfungen am eingefrorenen Lieferstand erfolgreich, Originalevidenz tatsächlich gelesen und gebunden. AK-06 getrennte unabhängige Abschlussprüfung oder konkrete externe Prüfgrenze und vollständige prüffertige Übergabe; kein eigener PASS als Rollenprüfung. AK-07 aktuelle Status-/Branch-/Commit-/Tree-/PR-/CI-/Restgrenzen und letzter/nächster Schritt persistent, kein WP026-Gesamt-PASS.

## Tests

1. Git-Anker/Parent/Manifestbytes/Allowlist/untracked/Whitespace und unveränderte Fremdquellen; kanonisches Linux CPython3.11.13/Node22.20.0/Lock: Architecture-only und eigener Production-Scope jeweils --self-test.
2. Original-ZIP-Digest/Memberhash/XML-Testfallausgabe und vollständige korrespondierende Joblogs; tatsächlichen Checkout aus Logs verifizieren; Testdauern/Failursache/alle sechs Ergebnisfälle vergleichen.
3. Diagnosebaseline in regulärer CI vor Produktionseingriff; Phasen außerhalb Zeitintervalle. Kontrollierter Alt-/Neu-Quellvergleich mit gleichen sechs Fällen/Parametern, Wanduhr/CPU/GC/Allokation soweit verfügbar. Regression vor Korrektur und Mutationen gegen die Budget-/Äquivalenzbehauptung nach Korrektur.
4. Vollständige reine Domain-/Solver-/Contenttests und echte Unity-EditMode inkl. Coverage; Produktionverify aller240 mit Regeneration/Strict/3840 Metamorphosen/1440 Mutationen, Schema/Node/Root. Keine Goldenaktualisierung. Byteidentität und ursprüngliche Trace-/Metrik-/Premissenordnung prüfen.
5. Reguläre CI am endgültigen Lieferstand: Architecture/Scope, EditMode, PlayMode/QA, Android Development IL2CPP und iOS/Xcode; Test-XMLs/Logs/Artefakte selbst auswerten. CI-PERF-WP026 bleibt eigenes unverändertes ADR032-Protokoll.
6. Getrennte unabhängige technische Prüfung von Ursache, Korrektur, semantischer Invarianz, Regression, Scope und echter CI auf demselben Freeze vor jeder späteren Integration.

## Risikoklasse

Hoch: Zuverlässigkeits- und harte Budgetgrenze; eine Optimierung darf keine solver-v2-/Proof-/Traceänderung verdecken.

## Definition of Done

Projektweite DoD unverändert; AK einzeln belegen. Vollständige zulässige Umsetzung bis tatsächlicher unabhängiger Prüfung oder konkreter externer Entscheidungsgrenze. Keine Ursache aus erfolgreichem Lauf ableiten. Bei nötiger Semantik-/Architekturänderung oder fehlendem externem Zugang konkreten Blocker und vollständig erledigte unabhängige Teilarbeit übergeben. WP/CURRENT_STATE/WORK_QUEUE und eigene Übergabe aktualisieren, echte CI/Freeze/QC standgebunden, keine Main-/Gesamtfreigabe.
