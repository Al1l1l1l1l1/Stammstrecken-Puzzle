# WP-004 – Architecture-v0.4-Abschlusskorrekturen

## ID

`WP-004`

**Bearbeitungsstatus:** Abgeschlossen am 2026-09-12 auf Branch `arch/architecture-v0.1`.

## Ziel

Die sieben Restbefunde `V03-001` bis `V03-007` des unabhängigen Architecture-v0.3-Reviews werden vollständig, widerspruchsfrei und mit nachweislich zutreffenden Validatoraussagen geschlossen. Das Ergebnis heißt **Architecture v0.4**. Architecture v1.0 wird in diesem Work Package ausdrücklich nicht freigegeben.

Die Korrekturrunde erzeugt keinen Produktionscode, verändert keine bestätigte Produktentscheidung, löst keinen der drei Produktfolgeblocker und führt keine neue allgemeine Architekturphase ein.

## Voraussetzungen

Vor Beginn gelten die vollständige Lesereihenfolge aus `AGENTS.md`, Architecture v0.3, die für die sieben Findings relevanten Architektur- und ADR-Dateien, `PROJECT_CONTROL/DEFINITION_OF_DONE.md`, `PROJECT_CONTROL/AI_HANDOVER_RULES.md` sowie der kanonische Architekturvalidator.

Ausgangspunkt ist der verifizierte Commit `a5b181de4698d3aa09b280bc96d27d5b648d5acb` auf lokalem und entferntem Branch `arch/architecture-v0.1`. Lokaler `main` und `origin/main` sind zu Beginn identisch bei `3236334a65d0517c543af0366ba7bc4081bdc263`.

Der Documentation-Scope ist vor allen fachlichen Korrekturen im kanonischen, unveränderlichen Manifest [`tools/architecture-validation/scopes/WP-004.documentation.scope.json`](../tools/architecture-validation/scopes/WP-004.documentation.scope.json) verankert. Der Abschlussvalidator muss dessen ursprünglichen Git-Blob verwenden und eine nachträgliche Erweiterung im Arbeitsdiff abweisen.

Der fehlende GitHub-Actions-Zugang ist für Architecture v0.4 **non-blocking with follow-up**. Vor dem ersten produktiven Coding-Work-Package muss jedoch ein eigenes CI-Setup-Work-Package abgeschlossen werden.

## Scope

Erlaubt sind ausschließlich die folgenden sieben Korrekturen und zwingend notwendige konsistente Folgeänderungen:

1. `V03-001`: Privacy-Upgrade, direkte Versionssprünge, nie gestartete Zwischenversionen, Widerrufs-Crashfenster, native Reconciliation vor möglicher Erfassung sowie Fresh-/Upgrade-/Revoke-/Recovery-/Restart-/Re-enable-/Offline-Übergänge fail-closed präzisieren.
2. `V03-002`: Endless-Lifecycle um persistierte Reservation vor Generierung, reproduzierbare Generierung, crashsicheres Resume, getrennte Completion und nachgelagerte Claims sowie begrenzte Terminalisierung/Kompaktierung ergänzen. Die UInt64-Watermark bleibt bestehen.
3. `V03-003`: Validatoraussagen auf tatsächlich geprüfte Semantik begrenzen und Watermark-, Privacy-, Migration-, Release-Lock-, Cosmetics- sowie gegebenenfalls strukturierte IAP-Prüfungen mit gezielten Negativmutationen ergänzen.
4. `V03-004`: Alle in Level v1 gültigen Fokuswerte in Level v2 kompatibel halten und `DRAFT` im Cosmetics-v2-Authoringvertrag erlauben; positive und negative Fixtures ergänzen, ohne Produktwerte zu ändern.
5. `V03-005`: Globale oder global-äquivalente Scopepatterns, absolute/externe Manifeste und unversionierte beziehungsweise nicht eindeutig dem Work Package zugeordnete Scope-Manifeste fail-closed abweisen.
6. `V03-006`: Den ursprünglichen historischen Text von ADR-016 wiederherstellen; aktuelle Normen ausschließlich in nachfolgenden ADRs führen und bidirektionale Superseding-Verweise sowie ADR-Index konsistent halten.
7. `V03-007`: Die operative Rolloutmetrik ohne Crashlytics konkret auf Quelle, Population, Verfügbarkeit und Verhalten bei fehlender Datengrundlage festlegen oder ehrlich als verpflichtendes Release-Follow-up ausweisen.
8. Architecture-, ADR-, Work-Package-, Project-Control-, Validator- und Navigationsstatus konsistent auf **Architecture v0.4** aktualisieren.
9. Das zwingende CI-Setup-Work-Package vor dem ersten produktiven Coding-Work-Package in `CURRENT_STATE.md` und `WORK_QUEUE.md` verankern. In WP-004 wird kein GitHub-Actions-Workflow erzwungen.

## Betroffene Dateien/Module

| Pfad oder Modul | Zulässige Änderung |
|---|---|
| `WORK_PACKAGES/WP-004_Architecture-v0.4-Abschlusskorrekturen.md` | Auftrag, Status, Finding-Matrix, Tests und Abschlussnachweise. |
| `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` | Architecture-v0.4-Stand und zwingendes CI-Setup-WP vor Produktionscoding. |
| `ARCHITECTURE/ARCHITECTURE.md` | Navigation, Status und V03-001-bis-V03-007-Matrix. |
| `ARCHITECTURE/MOBILE_SERVICES.md`, `ARCHITECTURE/OBSERVABILITY.md`, `ARCHITECTURE/PRIVACY_PROVIDER_EVIDENCE.md`, `ARCHITECTURE/GAME_STATE_MODEL.md` | Ausschließlich Privacy-Lifecycle und Zustandsfolgen aus V03-001. |
| `ARCHITECTURE/PERSISTENCE.md`, `ARCHITECTURE/SOLVER_ARCHITECTURE.md`, `ARCHITECTURE/GAME_STATE_MODEL.md` | Ausschließlich Endless-Reservation, Generation, Completion, Claim und Kompaktierung aus V03-002. |
| `ARCHITECTURE/LEVEL_DATA_FORMAT.md`, `ARCHITECTURE/CONTENT_CATALOGS.md`, `ARCHITECTURE/BUILD_AND_RELEASE.md`, `ARCHITECTURE/TEST_STRATEGY.md` | Ausschließlich Kompatibilitäts-, Rollout-, Scope- und Testfolgeänderungen der sieben Findings. |
| `ARCHITECTURE/schemas/level-v2.schema.json`, `ARCHITECTURE/schemas/cosmetics-v2.schema.json` | V03-004-Kompatibilitätskorrekturen. |
| `ARCHITECTURE/examples/**`, `tools/architecture-validation/fixtures/**` | Minimale positive/negative Fixtures und Goldens für die sieben Findings; keine Produktinhalte. |
| `DECISIONS/ADR-016-katalogvertraege-und-endless-identitaet.md` | Historischen Text wiederherstellen und ausschließlich historische Superseding-Hinweise ergänzen. |
| `DECISIONS/README.md`, betroffene bestehende ADRs und technisch notwendige neue Folge-ADRs | Aktuelle Entscheidungen ohne rückwirkende Umschreibung führen; Index und Superseding konsistent halten. |
| `tools/architecture-validation/validate.py`, `tools/architecture-validation/README.md`, `tools/architecture-validation/scope-manifest-v1.schema.json`, `tools/architecture-validation/scopes/**` | Tatsächliche semantische Prüfungen, Berichtskategorien, Scope-Härtung, v0.4-Manifest und Selbsttests. |
| `ARCHITECTURE/OPEN_BLOCKERS.md` | Ausschließlich Versionsstand aktualisieren; die drei Produktblocker inhaltlich unverändert lassen. |

## Ausdrücklich nicht erlaubte Änderungen

Nicht erlaubt sind Produktionscode, Unity-Projektdateien, neue Frameworks, neue Subsysteme, neue Produktentscheidungen, zusätzliche Produktfeatures, Änderungen an Konzeptdateien, konkrete Season-1-Puzzles, konkrete Zeitwerte, neue Cosmetics oder Preise, die Lösung der drei Produktfolgeblocker, ein GitHub-Actions-Workflow ohne vorhandene Berechtigung, Änderungen an `main`, Merge oder automatische Pull-Request-Erstellung.

`BLOCKER-PROD-001`, `BLOCKER-PROD-002` und `BLOCKER-PROD-003` bleiben unverändert offen und fail-closed. Architecture v1.0 darf nicht ausgerufen werden.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| `AK-01` | Direkte Upgrades, nie gestartete Zwischenbuilds und ein Crash zwischen Application- und nativem Widerrufszustand können vor möglicher optionaler Datenerfassung keinen vertrauenswürdigen `true`-Override hinterlassen; unbekannte oder nicht sicher deaktivierbare Zustände bleiben fail-closed. |
| `AK-02` | Endless besitzt einen persistierten Zustand `RESERVED_NOT_GENERATED`; Generation ist an die Reservation gebunden und reproduzierbar/idempotent; Completion, spätere Claims und endgültige Kompaktierung sind getrennt und begrenzt. |
| `AK-03` | Der Validator lehnt fachlich ungültige Watermarks, Privacykombinationen, Migrationsquellen, Release-Lock-Bindungen und Cosmetics-Transitions ab. IAP-Dokumentsemantik wird nur als automatischer PASS ausgewiesen, wenn sie strukturiert geprüft wird. |
| `AK-04` | Level v2 akzeptiert alle vier v1-Fokuswerte `EXCLUSION`, `CHAIN`, `DENSITY`, `COMBINATION`; Cosmetics v2 akzeptiert einen gültigen `DRAFT`-Authoringkatalog. |
| `AK-05` | Scopepatterns `*`, `**`, `**/*` und andere global äquivalente Muster sowie absolute, externe, unversionierte oder nicht WP-zugeordnete Manifeste werden abgewiesen; Produktdateischutz bleibt erhalten. |
| `AK-06` | ADR-016 entspricht wieder seinem angenommenen historischen Stand; spätere Änderungen stehen ausschließlich in nachfolgenden ADRs und alle Superseding-Verweise sind bidirektional konsistent. |
| `AK-07` | Rolloutmetrik nennt konkrete Quelle und Population oder ist explizit `REQUIRED_LATER/NOT_EXECUTED`; fehlende Daten erlauben keine automatische Fortsetzung und führen nicht zur Wiedereinführung von Crashlytics. |
| `AK-08` | Validatorbericht trennt `LOCAL_DOCUMENT_STRUCTURE`, `LOCAL_ARCHITECTURE_SEMANTICS`, `MANUAL_ARCHITECTURE_REVIEW`, `LOCAL_SCOPE`, `CONTRACT_ONLY`, `REQUIRED_LATER/NOT_EXECUTED` und `BLOCKED`, ohne Abdeckung zu überdehnen. |
| `AK-09` | Architecture-, ADR-, Project-Control-, Work-Package- und Validatorstatus sind konsistent Architecture v0.4, niemals v1.0. |
| `AK-10` | Vor dem ersten produktiven Coding-Work-Package ist ein eigenes CI-Setup-WP als zwingendes Queue-Gate dokumentiert; die aktuelle fehlende Workflowberechtigung blockiert Architecture v0.4 nicht. |
| `AK-11` | Die drei Produktfolgeblocker bleiben inhaltlich unverändert offen/fail-closed; keine Produktdatei und keine bestätigte Produktentscheidung wurde verändert. |
| `AK-12` | Der vollständige Diff enthält nur die ausdrücklich erlaubten Dokumentations-, ADR-, Governance-, Schema-, Fixture- und Validatorartefakte; kein Produktionscode, kein Merge und keine Änderung an `main`. |

## Tests

Vor Abschluss werden mindestens ausgeführt und commitbezogen dokumentiert:

1. Architecture-only-Validator, Documentation-Scope-Validator und vollständiger Self-Test in einer sauberen repositoryexternen Umgebung aus dem gepinnten Lock;
2. Privacy-Zustandsmodelltests für Fresh Install, normales Upgrade, direkten Versionssprung, nie gestarteten Zwischenbuild, Widerrufs-Crashfenster, Recovery, Restart, Re-enable und Offlinefall;
3. Endless-Transitionstests für Reservation vor Generierung, Crash direkt nach Reservation, deterministische Regeneration, aktiven Draft, Completion, Abandon, nachgelagerten Claim, Duplicate und lange Nutzung ohne wachsende Historie;
4. negative Watermarktests einschließlich ungültiger Grenzen und inkonsistenter aktiver Ordinals;
5. Migrations-Goldens mit positiver Quellbindung sowie Negativmutationen für falsche `levelId`, Legacyhash, Schema- und Source-Version;
6. Release-Lock-Cross-Reference-Tests für Puzzlehash, Lösungshash, Proofhash, Solverversion und Dokumentversion;
7. ausführbare Cosmetics-Eligibility-/Ownershiptransitions einschließlich DRAFT-Fixture, Duplicate, nicht berechtigt, bereits besessen und atomarem Claim;
8. Schema-Kompatibilitätstest für alle vier bisherigen Fokuswerte;
9. Scope-Negativtests für `*`, `**`, `**/*`, globale Äquivalente, absolute/externe/unversionierte/falsch zugeordnete Manifeste und Scope-Escape;
10. ADR-016-Historienvergleich gegen den Stand vor Architecture v0.3 sowie strukturierte bidirektionale Superseding-Prüfung;
11. Rolloutmetrikkonsistenz und Negativtest gegen unbelegte automatische Freigabe bei fehlenden Daten;
12. `git diff --check`, vollständiger Scope-, Secret-, Produktdatei-, Produktionscode- und `main`-Unverändertheitscheck;
13. gezielter unabhängiger Delta-Review ausschließlich für `V03-001` bis `V03-007` und zwingende Folgeänderungen;
14. erneuter vollständiger Lauf auf dem konkreten Abschlusscommit vor Push sowie Remote-Commit-Verifikation nach Push.

Unity Compile/EditMode/PlayMode, `BootstrapCompositionSmoke`, IL2CPP, physische Privacy-Gerätecaptures, SDK-Sandbox und Storepromotion bleiben mangels Produktionsprojekt **REQUIRED_LATER/NOT_EXECUTED**. Der GitHub-Actions-Nachweis bleibt ein verpflichtendes separates CI-Follow-up vor Produktionscoding.

## Ergebnis und Abnahmenachweis

| Finding | Status | Abschlussbeleg |
|---|---|---|
| `V03-001` | **CLOSED** | Ausführbarer Privacy-v2-Reducer deckt Fresh Install, direkten Legacy-Sprung mit altem Native-Override, nie gestarteten Zwischenbuild, `REVOKE_PENDING`, Crash/Restart, Re-enable und Offline ab; Production bleibt ohne belegten prä-SDK-Fence fail-closed. |
| `V03-002` | **CLOSED** | `openEndless` modelliert Reservation, Generierung, Draft, Completionclaim und Terminalisierung; Claim-ID, Providerergebnis, Claimstatus und kataloggebundener Betrag sind ausführbar gebunden; 10.000 gemischte Zyklen bleiben bounded. |
| `V03-003` | **CLOSED** | Validator prüft Schema-, Reducer-, Cross-Reference-, Migrations-, Release-Lock-, Cosmetics-, IAP-Fixture- und Reportsemantik mit gezielten Negativmutationen; nicht ausgeführte Belege erscheinen nie als lokaler PASS. |
| `V03-004` | **CLOSED** | Level v2 akzeptiert `EXCLUSION`, `CHAIN`, `DENSITY`, `COMBINATION`; schema-gültiges DRAFT-Cosmetics-Fixture wird zur Runtime abgewiesen. |
| `V03-005` | **CLOSED** | Segmentglobvertrag, globale Patternverbote, repositoryrelativer kanonischer Manifestpfad, WP-Zuordnung und unveränderlicher Vorab-Anker werden geprüft. |
| `V03-006` | **CLOSED** | ADR-016-Entscheidungskörper stimmt mit Commit `6152eead04494386404241967da0d8a62e741718` überein; aktuelle Regeln stehen in ADR-024 bis ADR-026 und der Index ist konsistent. |
| `V03-007` | **CLOSED** | `store-crash-rate-v1` bindet Android/iOS an konkrete Storequelle, Releasepopulation, Mindestmenge, Freshness und `PAUSE_NO_ADVANCE`; Crashlytics bleibt in Production ausgeschlossen. |

Der erste unabhängige Delta-Review identifizierte neben dem erwarteten offenen WP-Status eine überbreite Report-Selbsttestprüfung und eine fehlende fachliche Bindung des Endless-Claims. Beide wurden korrigiert. Der anschließende fokussierte Re-Review meldete **`ZERO OPEN TECHNICAL FINDINGS; ONLY EXPECTED WP STATUS CLOSURE REMAINS`**. Dieser erwartete Statusrest ist mit dem vorliegenden Abschluss behoben. Die danach durchgeführte vollständige Staging-Diff-Prüfung deckte noch zwei enge Kopplungslücken zwischen E1-/Puzzle-ID sowie lokaler und Provideroperation auf. Nach ihrer Korrektur meldete der gezielte Recheck **`FINAL ENDLESS CLAIM RECHECK PASS — ZERO ISSUES`**.

Der kanonische Documentation-Scope-Lauf umfasst `LOCAL_DOCUMENT_STRUCTURE`, `LOCAL_ARCHITECTURE_SEMANTICS`, `LOCAL_SCOPE` und den vollständigen Mutations-Selbsttest. `MANUAL_ARCHITECTURE_REVIEW` bestätigt separat die konsistente IAP-Reihenfolge Verifikation → atomarer lokaler Grant → Google Acknowledge/Apple Finish → lokaler Finalstatus. `CONTRACT_ONLY`, `REQUIRED_LATER/NOT_EXECUTED` und `BLOCKED` bleiben ausdrücklich ohne PASS-Behauptung.

Vor Commit werden zusätzlich `git diff --check`, vollständige Scope-/Secret-/Produktdateiwachen und der Vergleich von lokalem/remote `main` ausgeführt. Nach dem Abschlusscommit werden Architecture-only- und Documentation-Scope-Lauf exakt auf diesem Commit wiederholt; erst danach wird ausschließlich `arch/architecture-v0.1` gepusht und der Remotecommit verifiziert. Es erfolgt kein Merge und keine Pull-Request-Erstellung.

## Risikoklasse

**Hoch.** Die Korrekturen betreffen Privacy-Fail-closed-Verhalten, crashsichere Persistenz und Deduplikation, Migrationsidentität, IAP- und Cosmetics-Transaktionen, Releaseprovenienz sowie die Glaubwürdigkeit des Validators.

## Definition of Done

Das Work Package ist nur abgeschlossen, wenn alle sieben Findings einzeln als `CLOSED` belegt sind, alle lokal ausführbaren Akzeptanzkriterien und Tests bestanden haben, die Validatorberichte nur tatsächlich geprüfte Aussagen als PASS ausgeben und ein gezielter unabhängiger Delta-Review keine offenen HIGH- oder MEDIUM-Befunde innerhalb des Scopes meldet.

Jede materielle Korrektur einer angenommenen Architekturentscheidung wird in einem nachfolgenden ADR dokumentiert. `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md`, `DECISIONS/README.md`, dieses Work Package und die Architekturnavigation müssen denselben Architecture-v0.4-Stand wiedergeben. Der Abschlusscommit wird ausschließlich auf `arch/architecture-v0.1` gepusht; `main` bleibt unverändert und es erfolgt kein Merge.

## Referenzen

[1]: ../AGENTS.md "Verbindliche Agentenleitlinie"
[2]: ../PROJECT_CONTROL/WORK_PACKAGE_RULES.md "Regeln für Work Packages"
[3]: ../PROJECT_CONTROL/DEFINITION_OF_DONE.md "Definition of Done für technische Aufgaben"
[4]: ../PROJECT_CONTROL/AI_HANDOVER_RULES.md "Verbindliche Regeln für die KI-Übergabe"
[5]: ../ARCHITECTURE/ARCHITECTURE.md "Stammstrecken-Puzzle – Architecture"
[6]: ../DECISIONS/README.md "Architekturentscheidungen – verbindlicher ADR-Index"
