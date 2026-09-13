# WP-003 – Architecture-v0.3-Finalkorrekturen

## ID

`WP-003`

**Bearbeitungsstatus:** Abgeschlossen am 2026-09-12 auf Branch `arch/architecture-v0.1`.

## Ziel

Die acht Findings `FINAL-001` bis `FINAL-008` des unabhängigen Astra-Reviews werden vollständig, widerspruchsfrei und maschinenprüfbar geschlossen. Das Ergebnis heißt **Architecture v0.3**. Architecture v1.0 wird in diesem Work Package ausdrücklich nicht freigegeben.

Die Korrekturrunde erzeugt keinen Produktionscode, ändert keine bestätigte Produktentscheidung und löst keinen der drei bekannten Produktfolgeblocker. Der Abschluss ist nur zulässig, wenn der kanonische Architekturvalidator in einer dokumentierten sauberen Umgebung besteht und sein Ergebnis eindeutig dem Abschlusscommit zugeordnet werden kann.

## Voraussetzungen

Vor der Bearbeitung sind `AGENTS.md` und die dort vorgeschriebene Lesereihenfolge vollständig abzuarbeiten. Zusätzlich sind Architecture v0.2, alle aktuellen Architecture Decision Records (ADRs), alle Dateien unter `PROJECT_CONTROL/`, `WP-001`, `WP-002`, das vollständige Validatorpaket und alle von den acht Findings betroffenen Verträge zu lesen.

Ausgangspunkt ist der am 2026-09-12 verifizierte Remote-Commit `6152eead04494386404241967da0d8a62e741718` auf `origin/arch/architecture-v0.1`. Lokaler Branch und Remote-Branch waren identisch und sauber. `main` und `origin/main` waren identisch bei `3236334a65d0517c543af0366ba7bc4081bdc263`.

SDK-spezifische Aussagen zu Firebase Analytics, Firebase Crashlytics, Unity In-App Purchasing (IAP), Google User Messaging Platform (UMP) und Google Mobile Ads müssen anhand aktueller offizieller Anbieterinformationen für die gepinnten Versionen belegt werden. Die bestehenden Produktquellen und die drei offenen Produktfolgeblocker bleiben verbindlich.

## Scope

Erlaubt und erforderlich sind ausschließlich die folgenden fachlich zusammenhängenden Korrekturen und ihre notwendigen konsistenten Folgeänderungen:

1. `FINAL-001`: Die normative Assembly-Allowlist erlaubt `STP.Bootstrap` genau die Referenzen, die eine Composition Root zur Erzeugung und Verdrahtung von Application, Ports und Adaptern benötigt. Der Graph bleibt azyklisch. Es wird kein Dependency-Injection-Framework und keine zusätzliche DI-Abstraktion eingeführt. Ein verbindlicher späterer Compile-/Composition-Smoke und eine ausführbare Validatorprüfung werden definiert.
2. `FINAL-002`: Dauerhafte Endless-Identitäts-/Deduplikationswahrheit wird von begrenzten Diagnosedetails getrennt. Beliebig lange zulässige Folgen terminaler Ergebnisse bleiben ohne Produktlimit verarbeitbar. Save-, Migration-, Kompaktierungs- und Gegenprobenverträge werden deterministisch und tatsächlich begrenzt gefasst; das fehlerhafte 20/64/64-Argument wird entfernt.
3. `FINAL-003`: Für die gepinnten SDK-Versionen wird ein fail-closed nativer Consent-/Telemetrie-Lebenszyklus für Fresh Install, persistierten Override, Upgrade, Policyinvalidierung, Widerruf, erneute Freigabe, Neustart und Offlinefall definiert. Physische Geräte-/Network-Capture-Gates decken Fresh Install, Upgrade mit zuvor aktiver Telemetrie, Widerruf und erneute Aktivierung ab.
4. `FINAL-004`: Der Architekturvalidator erhält strukturierte oder ausführbare Prüfungen für ADR-Indexstatus, Level-ID gegen Season-/Hierarchiedaten, Zwei-/Drei-Sterne-Schwellen, IAP-Reihenfolge, Bootstrapgraph und neue Retentioninvarianten. Negative Mutationen weisen die Erkennung dieser Fehler nach. Der Bericht trennt Dokument-/Strukturprüfung, semantische Architekturprüfung, spätere Produktionscodeprüfung und spätere Geräteprüfung.
5. `FINAL-005`: Dauerhafte fachliche Puzzleidentität, serialisiertes Dokumentformat und versionierter Solver-/Proofnachweis werden getrennt. Hashstabilität, Schemamigration, Proofregeneration, Fortschrittserhalt, Release-Lock und Altversionsprüfung werden eindeutig definiert und durch Golden-/Migrationstests belegt.
6. `FINAL-006`: Allgemeine Architektur-/Governanceprüfungen werden von auftragsabhängigen Git-Scope-Prüfungen getrennt. Dieses Work Package verwendet weiterhin einen Documentation-/Tooling-only-Modus; spätere Produktions-Work-Packages können denselben Validator mit einem expliziten, fail-closed Produktionsmodus nutzen.
7. `FINAL-007`: Staging und öffentlich promotierbarer Release Candidate werden getrennt. Nur ein bereits mit finaler App-/Bundle-ID, freigegebener Productionkonfiguration, Release-Manifest sowie Git-/Content-/Toolchainbezug gebautes Artefakt ist ohne Neubau promotierbar.
8. `FINAL-008`: Der Cosmetics-Katalog erhält einen versionierten Vertrag für meilensteinbasierte Berechtigung, autoritative Bedingungen, atomare idempotente Ownership-Vergabe, Wiederholung, Migration und Katalogrevision, ohne konkrete neue Items oder Meilensteinwerte zu erfinden.
9. Alle notwendigen Überschriften, Navigations-, ADR-, Status-, Inventar-, Test- und Übergabeaussagen werden konsistent auf Architecture v0.3 aktualisiert.
10. Der kanonische Validator wird lokal in einer sauberen unterstützten Umgebung auf dem geprüften Commit ausgeführt. Eine zusätzliche GitHub-Actions-Integration ist bevorzugt, aber nur zulässig, wenn die verfügbare GitHub-Autorisierung Workflowdateien pushen darf; andernfalls wird die konkrete Nichtausführbarkeit dokumentiert und nicht als PASS gewertet.

## Betroffene Dateien/Module

| Pfad oder Modul | Zulässige Änderung |
|---|---|
| `WORK_PACKAGES/WP-003_Architecture-v0.3-Finalkorrekturen.md` | Neu anlegen; Auftrag, Status, Finding-Matrix, Tests und Abschlussnachweise dokumentieren. |
| `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` | Ist-Stand, nächster Schritt und Architecture-v0.3-Abschluss konsistent aktualisieren. |
| `DECISIONS/README.md` | ADR-Inventar, Geltung und Superseding auf den tatsächlichen v0.3-Stand aktualisieren. |
| Betroffene bestehende ADRs | Nur Status-/Ersetzungsverweise und historisch eindeutige Klarstellungen; keine stille wesentliche Entscheidungsänderung. |
| `DECISIONS/ADR-018-*.md` und weitere technisch notwendige Folge-ADRs | Neu anlegen, wenn ein langfristiger Vertrag neu eingeführt oder ein angenommener Vertrag ersetzt wird. |
| `ARCHITECTURE/ARCHITECTURE.md` | Architecture-v0.3-Navigation, Finding-Matrix und Status aktualisieren. |
| `ARCHITECTURE/MODULE_BOUNDARIES.md` | Bootstrap-/Composition-Root-Allowlist, Graph und späteren Compile-Smoke korrigieren. |
| `ARCHITECTURE/PERSISTENCE.md`, `ARCHITECTURE/SOLVER_ARCHITECTURE.md`, `ARCHITECTURE/GAME_STATE_MODEL.md` | Endless-Retention, Deduplikation, Migration und fachliche Identität konsistent korrigieren. |
| `ARCHITECTURE/MOBILE_SERVICES.md`, `ARCHITECTURE/OBSERVABILITY.md` | Nativen Consent-/Telemetrie-Lebenszyklus und Geräte-/Capture-Gates korrigieren. |
| `ARCHITECTURE/LEVEL_DATA_FORMAT.md`, `ARCHITECTURE/CONTENT_PIPELINE.md` | Puzzleidentität, Dokumentversion, Proofartefakte, Migration und Release-Lock trennen. |
| `ARCHITECTURE/CONTENT_CATALOGS.md` | Meilensteinbasierte kosmetische Berechtigung und Ownership-Verträge ergänzen. |
| `ARCHITECTURE/BUILD_AND_RELEASE.md`, `ARCHITECTURE/TECH_STACK.md` | Release-Candidate-Identität, Distribution, Toolchain- und Providerverträge korrigieren. |
| `ARCHITECTURE/TEST_STRATEGY.md` | Neue strukturierte Tests, Negativmutationen, Compile-/Geräte-/CI-Gates und Validatoraussagegrenzen ergänzen. |
| `ARCHITECTURE/OPEN_BLOCKERS.md` | Ausschließlich Versionsstand aktualisieren; Inhalt und Status der drei Blocker bleiben unverändert. |
| `ARCHITECTURE/schemas/level-v1.schema.json`, `ARCHITECTURE/schemas/cosmetics-v1.schema.json` sowie technisch notwendige neue versionierte Schemata | Nur für FINAL-005 beziehungsweise FINAL-008 notwendige Vertragsänderungen. |
| `ARCHITECTURE/examples/*.json` | Bestehende Goldens migrieren und minimale positive/negative Vertragsfixtures ergänzen; keine konkreten Produktinhalte erfinden. |
| `tools/architecture-validation/**` | Validator, Modi, strukturierte Prüfungen, Fixtures, Mutationen, Dokumentation und Lock aktualisieren. |

## Ausdrücklich nicht erlaubte Änderungen

Nicht erlaubt sind Produktionscode, Unity-Projektdateien, ausführbare Spielimplementierung, zusätzliche DI-Abstraktionen oder DI-Frameworks, 240 konkrete Season-1-Rätsel, konkrete finale Zwei-/Drei-Sterne-Zeitwerte, neue kosmetische Items, Preise oder konkrete Meilensteine, UI-/Audio-/Markenassets, Store- oder Providerkonfiguration und Änderungen an bestätigten Produktentscheidungen.

`BLOCKER-PROD-001`, `BLOCKER-PROD-002` und `BLOCKER-PROD-003` bleiben unverändert **offen und fail-closed**. `main` darf weder verändert noch gemergt werden. Ein Pull Request darf nicht automatisch gemergt werden. Architecture v1.0 darf in diesem Work Package nicht ausgerufen werden.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| `AK-01` | `STP.Bootstrap` darf die Composition Root implementieren, ohne die normative Allowlist zu verletzen; der vollständige Assemblygraph bleibt azyklisch und der spätere Compile-/Composition-Smoke ist verbindlich spezifiziert. |
| `AK-02` | Endless-Deduplikationswahrheit bleibt für beliebig lange zulässige Terminalfolgen korrekt, während aktive Runtime-/Diagnosedetails deterministisch begrenzt bleiben; keine neue Nutzungsgrenze entsteht. |
| `AK-03` | Fresh Install, persistierter Override, Upgrade, Invalidierung, Widerruf, Re-Enable, Neustart und Offlinefall besitzen für die gepinnten SDKs eindeutige fail-closed Übergänge und Geräte-/Capture-Gates. |
| `AK-04` | Der Validator erkennt ausführbar einen falschen ADR-Indexstatus, Level-ID-/Season-Widerspruch, `threeStarSeconds > twoStarSeconds`, IAP-Finalisierung vor persistentem Grant, falsche Bootstrapkante sowie verletzte neue Retentioninvarianten. |
| `AK-05` | Puzzleidentität, Dokument-/Schemaversion und Solver-/Proofartefakt sind getrennt; neutrale Schemamigration und Proofregeneration erhalten Puzzleidentität und Spielerfortschritt. |
| `AK-06` | Der Validator besitzt getrennte dokumentierte Modi für allgemeine Architekturprüfung und auftragsabhängige Scopeprüfung. Dieses Paket bleibt Documentation-/Tooling-only; spätere Produktionsdateien werden nicht pauschal durch den allgemeinen Modus verboten. |
| `AK-07` | Ein promotierbarer Release Candidate verwendet bereits finale App-/Bundle-ID und Productionkonfiguration und ist über ein Release-Manifest eindeutig an Git, Content und Toolchain gebunden; Staging ist ausdrücklich nicht promotierbar. |
| `AK-08` | Das Cosmetics-Schema stellt abstrakte meilensteinbasierte Berechtigung dar; Application vergibt Ownership einmalig, idempotent und atomar unter einer kataloggebundenen Claim-ID; Wiederholung und Migration sind definiert. |
| `AK-09` | Architecture-, ADR-, Work-Package-, Project-Control- und Validatorstatus sind dokumentübergreifend konsistent **v0.3**, niemals v1.0. |
| `AK-10` | Alle acht Findings besitzen belastbare Negativmutationen oder strukturierte Gegenproben; Fixturekonstanten und Wortsuche allein gelten nicht als semantischer Nachweis. |
| `AK-11` | Der kanonische Validator trennt im Bericht lokale Dokument-/Struktur- und Semantikprüfungen von späteren Produktionscode- und physischen Geräteprüfungen und überdehnt keine PASS-Aussage. |
| `AK-12` | Die drei Produktfolgeblocker bleiben unverändert offen und fail-closed; keine Produktquelle und keine bestätigte Produktentscheidung wurde verändert. |
| `AK-13` | Der vollständige Diff enthält nur die im Scope erlaubten Dokumentations-, ADR-, Governance-, Schema-, Fixture-, Validator- und CI-Artefakte; kein Produktionscode, kein Merge und keine Änderung an `main`. |
| `AK-14` | Der Abschluss-PASS ist durch einen erfolgreichen kanonischen Lauf in sauberer Umgebung und, sofern technisch möglich, einen erfolgreichen GitHub-Actions-Lauf eindeutig dem Abschlusscommit zugeordnet. |

## Tests

Vor Abschluss sind mindestens die folgenden Prüfungen auszuführen und mit Commitbezug zu dokumentieren:

1. vollständiges Datei-, ADR-, Work-Package- und Versionsinventar;
2. relative Markdown-Linkprüfung und ADR-Indexstatus gegen tatsächliche ADR-Metadaten;
3. JSON-Syntax, Duplicate-Key-, JSON-Schema-Draft-2020-12- und Cross-Reference-Prüfung aller Schemata und Beispiele;
4. strukturierte Level-ID-/Season-/Hierarchie- und Zwei-/Drei-Sterne-Schwellenprüfung;
5. Puzzleidentitäts-, Schemamigrations-, Proofregenerations-, Release-Lock- und Fortschrittserhalt-Goldens;
6. Assemblygraphprüfung einschließlich korrekter Bootstrapreferenzen, Zyklusgegenprobe und Spezifikation des späteren Compile-/Composition-Smokes;
7. ausführbare IAP-Zustands-/Reihenfolgeprüfung einschließlich Gegenprobe „Finalisierung vor persistentem Grant“;
8. Endless-Gegenproben für lange alternierende terminale Folgen, Duplicate-Erkennung, Resume, Kompaktierung und Migration ohne Nutzungsgrenze;
9. Consent-/Telemetrie-Vertragstests für Fresh Install, Upgrade mit aktivem Alt-Override, Invalidierung, Widerruf, Re-Enable, Neustart und Offlinefall; physische Geräte-/Network-Capture-Gates werden als **später auszuführen** und nicht als lokal bestanden ausgewiesen;
10. Cosmetics-Schema-, Milestone-Eligibility-, Claim-ID-, Atomaritäts-, Wiederholungs-, Revision- und Migrationstests ohne konkrete Produktwerte;
11. Validator-Selbsttests mit Negativmutation für jedes Finding `FINAL-001` bis `FINAL-008` sowie die vier nachgewiesenen FINAL-004-Beispiele;
12. beide Validator-Modi: allgemeine Architecture-/Governanceprüfung und `--scope documentation`, einschließlich einer Gegenprobe, dass `--scope production` legitime Produktionssuffixe nicht pauschal verbietet;
13. `git diff --check`, Scope-, Secret-, Produktdatei-, Produktionscode- und `main`-Unverändertheitsprüfung;
14. frische repositoryexterne Umgebung aus gepinntem Lock; anschließend kanonischer lokaler Lauf;
15. GitHub-Actions-Workflow auf dem Abschlusscommit, sofern mit Branchschutz und vorhandener CI-Architektur ausführbar; andernfalls ist die konkrete technische Nichtausführbarkeit als neuer Blocker zu dokumentieren und `AK-14` nicht als PASS zu melden.

Ein Unity-/Storebuild bleibt nicht anwendbar, weil dieses Work Package keinen Produktionscode oder Unity-Scaffold erzeugen darf. Der spätere Compile-/Composition-Smoke und die physischen Geräteprüfungen werden spezifiziert, aber nicht als in diesem Repository bereits ausgeführt dargestellt.

## Risikoklasse

**Hoch.** Die Korrekturen betreffen Compilergrenzen, langfristige Persistenz und Deduplikation, Datenschutz-/SDK-Lifecycle, Identitäts- und Migrationsverträge, IAP-Transaktionssicherheit, Releaseprovenienz und die Glaubwürdigkeit des Architekturvalidators. Fehler könnten späteren Produktionscode, Spielerfortschritt, Datenschutz oder Store-Releases beschädigen.

## Definition of Done

Das Work Package ist nur abgeschlossen, wenn alle acht Findings vollständig umgesetzt und jeweils `CLOSED` sind, alle Akzeptanzkriterien einzeln belegt und sämtliche lokal ausführbaren vorgeschriebenen Tests erfolgreich ausgeführt wurden. Der Validator muss nachweislich die geforderten Negativmutationen erkennen und seine Aussagegrenzen im Bericht offenlegen.

Jede wesentliche Änderung an einer angenommenen Architekturentscheidung benötigt ein neues angenommenes oder ersetzendes ADR mit expliziter Vorgängerreferenz. `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md`, `DECISIONS/README.md`, dieses Work Package und die Architektur-Navigation müssen denselben tatsächlichen Architecture-v0.3-Stand wiedergeben.

Der Abschlusscommit verwendet eine aussagekräftige Nachricht, vorzugsweise `docs(architecture): address Astra final review findings for v0.3`. Danach wird ausschließlich `arch/architecture-v0.1` gepusht. Es erfolgt kein Merge und keine Architecture-v1.0-Freigabe.

## Ergebnis

**Architecture v0.3 ist angenommen; Architecture v1.0 ist ausdrücklich nicht freigegeben.** Alle acht beauftragten Findings sind geschlossen. Ein unabhängiger Re-Review fand vier zusätzliche mittlere Konsistenzlücken zu Status, ADR-Indexstatus, Season-1-Hierarchiegrenzen und ADR-004/ADR-021-Ablösung; auch diese wurden vor Abschluss geschlossen und in Validator-Selbsttests überführt.

| Finding | Status | Abschlussbeleg |
|---|---|---|
| `FINAL-001` | **CLOSED** | Exakte neunkantige Bootstrap-Allowlist, Mermaid-Parität, zyklusfreier Graph, ADR-018 und verbindlicher späterer `BootstrapCompositionSmoke`. |
| `FINAL-002` | **CLOSED** | Save v2 mit UInt64-Watermark und höchstens 20 aktiven Drafts; 10.000 alternierende Terminaltransitionen, Duplicate-, Resume- und v1→v2-Goldenprüfung. |
| `FINAL-003` | **CLOSED** | Versionierter Decision-Record, Reset-only-Upgrade, Fresh/Offline/Revocation/Re-enable-Vertragsmodell, Crashlytics aus Production und vier spätere physische Capture-Gates. |
| `FINAL-004` | **CLOSED** | Strukturierte ADR-Index-, Season-1-Hierarchie-, Sternschwellen-, IAP-, Assembly-, Retention-, Status- und Negativmutationsprüfungen. |
| `FINAL-005` | **CLOSED** | `puzzleId`, `documentSchemaVersion` und `proof-v1` getrennt; profilierte Hashes, Release-Lock, neutrale Dokumentmigration und Fortschrittserhalt-Golden. |
| `FINAL-006` | **CLOSED** | Architecture-only-, Documentation- und Production-Scope getrennt; reales Diff inklusive Renames/untracked, Work-Package-Manifest und CI. |
| `FINAL-007` | **CLOSED** | Staging nicht promotable; Production-RC besitzt finale ID/Konfiguration und wird über Manifest/Receipt ohne Rebuild artefaktgleich promotiert. |
| `FINAL-008` | **CLOSED** | `cosmetics-v2` mit diskriminierten Erwerbsmodi, Campaign-Eligibility und atomarem idempotentem Ownershipclaim ohne Ledgerdelta. |

## Akzeptanznachweis

| Kriterium | Ergebnis |
|---|---|
| `AK-01` bis `AK-08` | **PASS – lokal semantisch/strukturell belegt.** Die jeweiligen Verträge und Negativmutationen sind im kanonischen Validator aktiv. |
| `AK-09` | **PASS.** Architecture, ADR-Index, `CURRENT_STATE`, `WORK_QUEUE`, WP-003 und Validator nennen konsistent v0.3; v1.0 bleibt unfreigegeben. |
| `AK-10` | **PASS.** Jedes Finding besitzt eine echte Dokument-, Graph-, Schema-, Fixture- oder Zustandsmutation; zusätzliche Mutationen decken ADR-Indexstatus, S1-Grenzen und Statuskonsistenz ab. |
| `AK-11` | **PASS.** Report trennt `LOCAL_DOCUMENT_STRUCTURE`, `LOCAL_ARCHITECTURE_SEMANTICS`, `LOCAL_SCOPE`, `CONTRACT_ONLY`, `REQUIRED_LATER/NOT_EXECUTED` und `BLOCKED`. |
| `AK-12` | **PASS.** Genau drei Produktfolgeblocker bleiben offen/fail-closed; Produktquellen sind unverändert und im Scope unabhängig von Modus geschützt. |
| `AK-13` | **PASS.** Scope-Manifest erlaubt nur Architektur-, ADR-, Governance- und Validator-/Fixture-Artefakte; kein Produktcode, kein Merge, `main` unverändert. |
| `AK-14` | **BLOCKED für GitHub Actions; lokaler Commit-PASS belegt.** Frische externe Virtualenv und beide Validator-Modi bestanden auf dem konkreten Abschlusscommit. Der Push einer gepinnten Workflowdatei wurde von GitHub abgelehnt, weil die aktive GitHub-App keine `workflows`-Berechtigung besitzt. Dieser externe CI-Teil ist ausdrücklich kein PASS. |

## Ausgeführte Prüfungen

| Prüfung | Ergebnis |
|---|---|
| Frische Umgebung aus `requirements.lock.txt` | **PASS** mit CPython 3.11 und Node 22 in repositoryexterner Virtualenv. |
| Architecture-only: `validate.py --self-test` | **PASS**, 16 lokale Prüfgruppen. |
| WP-003 Documentation-Scope | **PASS**, 17 lokale Prüfgruppen einschließlich realer Git-Diffmenge. |
| JSON/Schemas/Goldens/Hashes/Migration | **PASS** für alle elf Schemaverträge, v1-/v2-Levels, Proofs, Release-Lock, Progress-/Endless-Migration und Python/Node-JCS. |
| Negativmutationen | **PASS**; FINAL-001 bis FINAL-008, ADR-Indexstatus, S1-Grenzen, Sternreihenfolge, IAP-Finalisierung, Scope und Blockeränderung wurden erkannt. |
| Commitgebundener lokaler Abschluss | **PASS** für Architecture-only und Documentation-Scope im sauberen Arbeitsbaum des Abschlusscommits. |
| GitHub-Actions-Ausführung | **BLOCKED/NOT EXECUTED.** GitHub lehnte den Workflow-Push mit „refusing to allow a GitHub App to create or update workflow … without `workflows` permission“ ab; die nicht pushbare Workflowdatei ist deshalb nicht Teil des Abschlusscommits. |
| Diff-/Whitespace-/Secret-/Produktdateiprüfung | **PASS** vor Abschluss; wird auf dem finalen Index wiederholt. |
| Unabhängiger Re-Review | Erster Lauf: vier MEDIUM-Befunde erkannt und geschlossen. Gezielter zweiter Lauf: **keine HIGH-, MEDIUM- oder LOW-Befunde**. |

## Bewusste Beleggrenzen

`CONTRACT_ONLY` bedeutet, dass Architekturvertrag, Schema, Fixture und Referenzmodell vorliegen, aber noch kein Unity-Produktionscode existiert. Unity Compile/EditMode/PlayMode, echter `BootstrapCompositionSmoke`, C#-Produktionssolver, IL2CPP, physische Privacy-Network-Captures, SDK-Sandbox, Storeupload und artefaktgleiche Storepromotion bleiben **REQUIRED_LATER/NOT_EXECUTED** und sind kein lokaler PASS.

Die drei Produktfolgeblocker `BLOCKER-PROD-001`, `BLOCKER-PROD-002` und `BLOCKER-PROD-003` bleiben **BLOCKED/open/fail-closed**. Zusätzlich besteht ausschließlich für den optional bevorzugten Remote-CI-Nachweis ein externer Berechtigungsblocker: Die aktive GitHub-App darf `.github/workflows/*` ohne `workflows`-Berechtigung nicht erstellen oder aktualisieren. Die lokale commitgebundene Abnahme bleibt davon unberührt.

## Übergabe

Der Abschlusscommit verwendet `docs(architecture): address Astra final review findings for v0.3` und wird ausschließlich auf `arch/architecture-v0.1` gepusht. `main` bleibt unverändert; es erfolgt kein Merge und keine v1.0-Freigabe. Der nächste zulässige Schritt ist ein unabhängiger Architecture-v1.0-Freigabereview in einem neuen Work Package.

## Referenzen

[1]: ../AGENTS.md "Verbindliche Agentenleitlinie"
[2]: ../PROJECT_CONTROL/WORK_PACKAGE_RULES.md "Regeln für Work Packages"
[3]: ../PROJECT_CONTROL/DEFINITION_OF_DONE.md "Definition of Done für technische Aufgaben"
[4]: ../PROJECT_CONTROL/AI_HANDOVER_RULES.md "Verbindliche Regeln für die KI-Übergabe"
[5]: ../ARCHITECTURE/ARCHITECTURE.md "Stammstrecken-Puzzle – Architecture"
[6]: ../DECISIONS/README.md "Architekturentscheidungen – verbindlicher ADR-Index"
