# WP-021 – Unity-Scaffold-Recovery

## ID

`WP-021`

**Bearbeitungsstatus:** WP-021-Korrekturen H1–H4/M1 lokal geprüft und im Commit `2868df4e2f3529d2d81d213ede745f1507558244` auf den bestehenden Branch gepusht (2026-10-03), Remote-HEAD verifiziert. Alle vier neuen CI-Läufe sind beendet und vollständig ausgewertet: GitHub-Account-/Billing-Sperre verhindert jeden Runnerstart; keine neuen ausgeführten CI-/iOS-/Coverage-/Guard-Nachweise. WP-021 bleibt **BLOCKED für den technischen Abschluss** und offen für unabhängige QC. Kein Merge, kein Schließen des PRs und keine Folgeaufgaben. Alte CI-Runs werden nicht als Korrekturbeleg anerkannt.

## Ziel

Auf der von aktuellem `main` (Commit `e4f8cc1d0f0d0590fd7508992af464c0230ef314`) abgezweigten Branch `feat/wp-021-unity-scaffold-recovery` wird das Unity-6.3-Produktionsscaffold mit der dafür erforderlichen CI-/Testgrundlage vollständig neu hergestellt und mit eigenen, commitgebundenen Nachweisen belegt:

- Unity-Projekt mit exakt Unity **6000.3.23f1** (`ProjectSettings/ProjectVersion.txt` autoritativ), C# 9, projektweit aktivierten Nullable Reference Types, Asset Serialization **Force Text**, Version Control Mode **Visible Meta Files** sowie gepinnten und eingecheckten Paket-/Projektlocks (`Packages/manifest.json`, `Packages/packages-lock.json`).
- `Toolchain.lock.md` gemäß `ARCHITECTURE/TECH_STACK.md` Abschnitt 6.
- Der vollständige normative `.asmdef`-Modulgraph gemäß [`ARCHITECTURE/MODULE_BOUNDARIES.md`](../ARCHITECTURE/MODULE_BOUNDARIES.md) Abschnitt 3 (vierzehn Produktionsassemblies) und Abschnitt 4 (neun Testassemblies an den dort normierten physischen Pfaden unter `Assets/StammstreckenPuzzle/Tests/`). Domain/Solver bleiben typenlose, compilerfähige Assemblies; Application erhält die fünfzehn normativen Port-Skelette sowie die fail-closed `ApplicationComposition`; Adapter-, Präsentations-, Editor- und Bootstrap-Skelette nur, soweit sie für den vollständigen Composition Graph erforderlich sind.
- Dedizierte QA-Szene und der Compile-/Composition-Smoke `STP.Tests.Bootstrap.PlayMode.BootstrapCompositionSmoke` gemäß `ARCHITECTURE/MODULE_BOUNDARIES.md` Abschnitt 7.
- Ausführbare CI-Basis mit eigenen, auf dieser Branch commitgebundenen Nachweisen für Compile, EditMode, Bootstrap-PlayMode-Smoke, Android-Development-IL2CPP sowie den von `ARCHITECTURE/BUILD_AND_RELEASE.md` und `ARCHITECTURE/TEST_STRATEGY.md` verlangten iOS-Nachweis.
- Wahrheitsgemäß dokumentierter Geräte-/Device-Farm-Verfügbarkeitszustand.
- Erste Coverage-Baseline nach Assembly aus dem tatsächlich ausgeführten EditMode-Lauf sowie wahrheitsgemäßer Mutationsstand gemäß `ARCHITECTURE/TEST_STRATEGY.md` Abschnitt 12.
- Mechanische Governance-Nachführung in `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md`.

WP-021 enthält **keine** Puzzlefachlogik. Der fachliche Puzzle-Stack (Domain, Solver, Commands, Completion, Leveldaten, JCS, Proof, Pipeline, Generator, Authoring) ist ausdrücklich **WP-022** und späteren Work Packages vorbehalten. Diese Trennung ist verbindlich und wird nicht wieder zusammengeführt.

## Voraussetzungen

1. Der bisherige WP-021-Hold ist durch die Geschäftsführung ausdrücklich aufgehoben: Die CI-Readiness-Voraussetzungen (Unity-Personal-Aktivierung, Linux-/Android-Runner, macOS/iOS-Runner, Compile, EditMode, PlayMode, Android-IL2CPP, iOS-Compile) sind real nachgewiesen. Die benötigten Repository-Secrets `UNITY_EMAIL` und `UNITY_PASSWORD` sowie die Repository-Variable `UNITY_RUNNERS_READY=true` sind konfiguriert.
2. Verbindliche Lesereihenfolge aus [`AGENTS.md`](../AGENTS.md) ist abgeschlossen: Projektübergabe, Konzeptindex, Master-Spezifikation, [`PROJECT_CONTROL/CURRENT_STATE.md`](../PROJECT_CONTROL/CURRENT_STATE.md), [`PROJECT_CONTROL/WORK_QUEUE.md`](../PROJECT_CONTROL/WORK_QUEUE.md), [`PROJECT_CONTROL/WORK_PACKAGE_RULES.md`](../PROJECT_CONTROL/WORK_PACKAGE_RULES.md), [`PROJECT_CONTROL/DEFINITION_OF_DONE.md`](../PROJECT_CONTROL/DEFINITION_OF_DONE.md), [`PROJECT_CONTROL/AI_HANDOVER_RULES.md`](../PROJECT_CONTROL/AI_HANDOVER_RULES.md), [`PROJECT_CONTROL/WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md`](../PROJECT_CONTROL/WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md), [`WORK_PACKAGES/WP-018_QC-Korrektur-und-Produktionsbasis-Reintegration.md`](../WORK_PACKAGES/WP-018_QC-Korrektur-und-Produktionsbasis-Reintegration.md), die archivierten Planungsarchive [`WORK_PACKAGES/WP-019_Unity-Scaffold-Recovery-auf-aktuellem-Main.md`](../WORK_PACKAGES/WP-019_Unity-Scaffold-Recovery-auf-aktuellem-Main.md) und [`WORK_PACKAGES/WP-020_Puzzle-Kern-Recovery-auf-aktuellem-Main.md`](../WORK_PACKAGES/WP-020_Puzzle-Kern-Recovery-auf-aktuellem-Main.md), [`ARCHITECTURE/MODULE_BOUNDARIES.md`](../ARCHITECTURE/MODULE_BOUNDARIES.md), [`ARCHITECTURE/TECH_STACK.md`](../ARCHITECTURE/TECH_STACK.md), [`ARCHITECTURE/BUILD_AND_RELEASE.md`](../ARCHITECTURE/BUILD_AND_RELEASE.md), [`ARCHITECTURE/TEST_STRATEGY.md`](../ARCHITECTURE/TEST_STRATEGY.md) sowie die ADR-001, ADR-002, ADR-012, ADR-013, ADR-018, ADR-026, ADR-030 und ADR-031.
3. `origin/main` steht bei `e4f8cc1d0f0d0590fd7508992af464c0230ef314`; es existieren weder eine WP-021-Branch, ein WP-021-Manifest noch ein offener WP-021-PR; der Arbeitsbaum ist sauber. Die Branch `feat/wp-021-unity-scaffold-recovery` zweigt exakt von diesem Commit ab und enthält zu Beginn keine eigenen Änderungen.
4. Die historischen Branches `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern` und `feat/wp-013-level-v2-pipeline` sowie der Branch `chore/ci-readiness-unity` sind ausschließlich lesbare Vergleichskorpora beziehungsweise ein historischer technischer Befund. Sie sind keine Lieferquelle: kein Merge, Cherry-Pick, Rebase, Copy ihrer Dateien und keine Übernahme ihrer PASS-Aussagen. Technische Lösungen werden aus dem aktuellen Architekturvertrag neu implementiert und auf dieser Branch erneut verifiziert.
5. WP-021 und [`tools/architecture-validation/scopes/WP-021.production.scope.json`](../tools/architecture-validation/scopes/WP-021.production.scope.json) werden vor allen fachlichen Änderungen gemeinsam im selben Trust-Anchor-Commit eingeführt (ADR-030). Der `baseCommit` des Manifests ist der Elterncommit dieses gemeinsamen Ankers, also `e4f8cc1d0f0d0590fd7508992af464c0230ef314`. Das Manifest bleibt danach byteunveränderlich.

## Scope

Erlaubt sind ausschließlich:

1. **Trust Anchor (vor jeder Scaffold-Datei):** dieses Work Package und das Production-Scope-Manifest im selben ersten Commit der Branch; anschließende Trust-Anchor-Verifikation (gemeinsamer Add-Status, `baseCommit`, historischer Manifestlink, Bytegleichheit).
2. **Unity-Projektanlage:** `ProjectSettings/**`, `Packages/manifest.json` und `Packages/packages-lock.json` mit exakt gepinnten Versionen gemäß `ARCHITECTURE/TECH_STACK.md` Abschnitt 1 und der im Abschnitt „Paketpins" dokumentierten Beleglage; `Toolchain.lock.md` (Runner-abhängige Werte zunächst wahrheitsgemäß als `NOT_DETERMINED`, Nachpflege ausschließlich aus den WP-021-eigenen CI-Läufen mit Commit-Bezug).
3. **Normativer `.asmdef`-Modulgraph:** alle vierzehn Produktionsassemblies aus `ARCHITECTURE/MODULE_BOUNDARIES.md` Abschnitt 3 und alle neun Testassemblies aus Abschnitt 4 an deren normierten physischen Pfaden; exakte Referenzmengen einschließlich exakt neun interner Bootstrap-Ziele (ADR-018), `noEngineReferences: true` für `STP.Puzzle.Domain`, `STP.Puzzle.Solver` und `STP.Application`, Editor-Grenze für `STP.Editor.Content` und `STP.Editor.Build`, EditMode-Grenze für die EditMode-Testassemblies sowie je Assembly eine `csc.rsp` mit `-nullable:enable` und `-warnaserror+` (ADR-002).
4. **Compilerfähige Skelette ohne Fachlogik:** die fünfzehn normativen Application-Ports (Abschnitt 6), `ApplicationComposition` (jeder Port genau einmal gebunden, null-frei, fail-closed bei Doppelbindung oder ungebundenem Zugriff, kein stiller Fallback) und `ApplicationRoot`; je ein dokumentiertes Skelett je Adapter-/Präsentations-/Editor-Assembly ohne Dateizugriff, SDK-Referenz oder fachliche Entscheidungslogik; `STP.Bootstrap` mit `BootstrapComposition` (manuelle Konstruktorinjektion in der Reihenfolge aus Abschnitt 7; Adapter werden erstellt, aber nicht initialisiert), `BootstrapCompositionResult` und `BootstrapInstaller` als einzigem Entry-Installer der QA-Szene (MonoBehaviour nur als Lifecycle-Adapter); `STP.Editor.Build`-Entrypoints für Preflight, Android-Development-IL2CPP und iOS-Export gemäß `ARCHITECTURE/BUILD_AND_RELEASE.md`.
5. **QA-Szene und Composition-Smoke:** `Assets/StammstreckenPuzzle/Scenes/QA/BootstrapComposition.unity` und `STP.Tests.Bootstrap.PlayMode.BootstrapCompositionSmoke` mit den in Abschnitt 7 geforderten Behauptungen (genau ein Binding pro Application-Port, vollständiger Application-/UI-/World-Graph, keine nicht dokumentierten Null-/Fallback-Ports, kein optionaler Providerstart, Fehlschlag einer Doppelbindung).
6. **CI-Basis:** Umschaltung von `STP_SCOPE_MANIFEST` in `.github/workflows/validate.yml` auf das WP-021-Production-Manifest (der WP-011-PR-Head-Checkout-Mechanismus bleibt unverändert) sowie der neue Workflow `.github/workflows/unity.yml` mit immer laufendem Projekt-Preflight (Versionspin, Pflichtartefakte, statischer Modulgraph-Check der realen `.asmdef`-Dateien gegen die normativen Tabellen aus Abschnitt 3 und 4 inklusive Azyklizität, `noEngineReferences`, Editor-Grenzen, `csc.rsp`-Konventionen und Guardrail-Tokens), fail-closed `unity-config`/`unity-evidence-guard` sowie den Nachweisjobs Compile, EditMode, Bootstrap-PlayMode-Smoke, Android-Development-IL2CPP und iOS-Export/Compile. Alle Actions werden per vollständigem Commit-SHA gepinnt; Permissions folgen Least Privilege; keine Secrets im Repository; die Aktivierung der Unity-Personal-Lizenz erfolgt über `secrets.UNITY_EMAIL`/`secrets.UNITY_PASSWORD`; die Lizenzjobs werden wegen der Ein-Instanz-Bedingung des Personal-Seats serialisiert und geben den Seat nach jedem Lauf best-effort frei.
7. **Evidenz und Dokumentation:** wahrheitsgemäßer Geräte-/Device-Farm-Verfügbarkeitszustand; erste Coverage-Baseline nach Assembly aus dem real ausgeführten EditMode-Lauf und wahrheitsgemäßer Mutationsstand (Domain/Solver sind typenlos; die erste reale Messung erfolgt mit WP-022); Trust-Anchor-, Scope-, `git diff --check`-, Secret- und Produktquellennachweise; Abschluss-/Zwischenstandsdokumentation in diesem Work Package, `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md`.

## Paketpins

Die Paketpins folgen `ARCHITECTURE/TECH_STACK.md` Abschnitt 1 und sind zum Zeitpunkt der Verankerung belegt:

| Paket | Version | Beleg |
|---|---|---|
| `com.unity.inputsystem` | `1.20.0` | TECH_STACK.md Abschnitt 1; Unity-Registry (`packages.unity.com`, Abruf 2026-10-02) |
| `com.unity.addressables` | `2.10.3` | TECH_STACK.md Abschnitt 1 (verbindliche Linie); Unity-Registry |
| `com.unity.localization` | `1.5.13` | TECH_STACK.md Abschnitt 1; Unity-Registry |
| `com.unity.nuget.newtonsoft-json` | `3.2.2` | TECH_STACK.md Abschnitt 1; Unity-Registry |
| `com.unity.render-pipelines.universal` | `17.3.0` | Core-Paketlinie der gepinnten Editorversion, aus der lokalen Installation `6000.3.23f1` ausgelesen (`BuiltInPackages/com.unity.render-pipelines.universal/package.json`) |
| `com.unity.test-framework` | `1.6.0` | TECH_STACK.md Abschnitt 1 (Core-Kopplung); aus `BuiltInPackages` der gepinnten Editorversion ausgelesen |
| `com.unity.testtools.codecoverage` | `1.3.0` | Unity-Registry (Mindest-Unity 2021.3); rein editor-/testseitiges Unity-Paket |

Begründung für `com.unity.testtools.codecoverage`: `ARCHITECTURE/TEST_STRATEGY.md` Abschnitt 12 verlangt mit dem ersten Code-Work-Package eine Coverage-Baseline nach Assembly; die Messung ist mit Standardbibliothek oder den vorhandenen Paketen nicht möglich (TECH_STACK.md Abschnitt 3 Regel 4). Das Paket ist das offizielle Unity-Testwerkzeug dafür, wirkt ausschließlich im Editor-/Testkontext und hat keine Produktionslaufzeitwirkung. Sollte die unabhängige QC diese Paketeinführung als architekturpflichtige Entscheidung werten, ist sie mit einer einzeiligen Manifeständerung entfernbar und die Coverage-Baseline wird stattdessen als eigener Blocker ausgewiesen.

Bewusst **nicht** eingebunden (Architekturverbot beziehungsweise spätere Gates): `com.unity.purchasing` (IAP folgt mit dem zuständigen späteren Work Package), Google Mobile Ads / Firebase Analytics / Firebase Crashlytics (kein SDK-Import vor den Privacy-Gates; TECH_STACK.md Abschnitt 3 Regel 11), Unity Gaming Services / Unity Analytics und Visual Scripting (Architekturverbot).

## Betroffene Dateien/Module

Alle folgenden Pfade stehen im vorab verankerten Production-Scope-Manifest. Nicht vorhandene Zielartefakte werden neu angelegt.

| Pfad oder Modul | Zulässige Änderung |
|---|---|
| `WORK_PACKAGES/WP-021_Unity-Scaffold-Recovery.md` | Neu: Auftrag, Trust-Anchor-Verweis, Vergleichsmatrix, Status, Akzeptanznachweise und Abschlussdokumentation. |
| `tools/architecture-validation/scopes/WP-021.production.scope.json` | Neu: vorab verankerte Production-Scope-Allowlist; nach dem Trust-Anchor-Commit byteunveränderlich. |
| `ProjectSettings/**` | Neu: Unity-Projekteinstellungen einschließlich autoritativer `ProjectVersion.txt` (6000.3.23f1), Force Text, Visible Meta Files und Plattformbaselines gemäß `ARCHITECTURE/TECH_STACK.md`. |
| `Packages/*` | Neu: `manifest.json` und `packages-lock.json` mit exakt gepinnten Paketversionen. |
| `Assets.meta`, `Assets/*`, `Assets/StammstreckenPuzzle/**` | Neu: Unity-Asset-Struktur, `.asmdef`-Dateien, compilerfähige Skelette, QA-Szene, Testassemblies/Smoke an den normierten Pfaden und zugehörige Meta-Dateien. |
| `Toolchain.lock.md` | Neu: Toolchain-Lock gemäß `ARCHITECTURE/TECH_STACK.md` Abschnitt 6. |
| `.github/workflows/validate.yml` | Ausschließlich Umstellung des Scope-Laufs auf das WP-021-Production-Manifest (`STP_SCOPE` und `STP_SCOPE_MANIFEST`). |
| `.github/workflows/unity.yml` | Neu: Unity-CI-Workflow für Preflight, Compile, EditMode, Bootstrap-PlayMode-Smoke, Android-Development-IL2CPP und iOS-Export/Compile; SHA-gepinnte Actions, Least Privilege. |
| `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` | Ausschließlich mechanische WP-021-Status- und Hold-Nachführung. |

## Ausdrücklich nicht erlaubte Änderungen

- Puzzle-Domain-, Solver- (einschließlich `solver-v1`/`solver-v2`), Command-, Completion-, Level-v2-, JCS-, Proof-, Pipeline-, Generator- oder Editor-Authoring-Fachlogik; konkrete Rätsel oder Produktionscontent.
- Progression, Economy, Persistenzfachlogik, Ads, IAP, Consent, Analytics oder fertige UI-Funktionen; SDK-Importe oder -Aktivierungen (insbesondere `com.unity.purchasing`, Google Mobile Ads, Firebase) über die im Scope genannten Pakete hinaus.
- Produktentscheidungen jeder Art; neue Architekturentscheidungen; Änderungen an `ARCHITECTURE/`, `DECISIONS/` oder `Stammstrecken_Puzzle_Konzept_00-15/`; Änderungen an `ARCHITECTURE/OPEN_BLOCKERS.md` oder an `BLOCKER-PROD-001/002/003`.
- Änderungen an historischen Scope-Manifesten; jede Änderung am WP-021-Manifest nach dem Trust-Anchor-Commit.
- Merge, Cherry-Pick, Rebase, Force Push oder Konfliktauflösung aus den historischen Branches `feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern`, `feat/wp-013-level-v2-pipeline` oder aus `chore/ci-readiness-unity`; Übernahme von Dateien oder PASS-Aussagen aus diesen Branches; Änderungen an `main`.
- Beiläufige Refactorings oder nicht aufgelistete Dateien/Module.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| `AK-01` | Trust-Anchor: WP-021 und `WP-021.production.scope.json` wurden im selben historischen Add-Commit eingeführt, der historische WP-Blob verweist exakt auf das Manifest, der Manifestblob ist bytegleich, `baseCommit` ist der Elterncommit des Ankers (`e4f8cc1d0f0d0590fd7508992af464c0230ef314`); der Anker ist remote auf `feat/wp-021-unity-scaffold-recovery` verifiziert. |
| `AK-02` | `ProjectSettings/ProjectVersion.txt` trägt exakt `6000.3.23f1`; C# 9, Nullable, Force Text und Visible Meta Files sind eingestellt; `Packages/manifest.json` und `Packages/packages-lock.json` sind gepinnt und eingecheckt; `Toolchain.lock.md` existiert und seine Runner-abhängigen Werte werden ausschließlich aus WP-021-eigenen CI-Läufen mit Commit-Bezug ergänzt. |
| `AK-03` | Der `.asmdef`-Modulgraph bildet die normativen Tabellen aus `ARCHITECTURE/MODULE_BOUNDARIES.md` Abschnitt 3 und 4 vollständig ab: alle vierzehn Produktions- und neun Testassemblies existieren an den normierten Pfaden, Referenzmengen entsprechen exakt der Tabelle (Bootstrap exakt neun interne Ziele), der Graph ist azyklisch, `noEngineReferences: true` für Domain/Solver/Application, EditMode-Testassemblies sind Editor-only. |
| `AK-04` | Sämtliche Skelette kompilieren ohne fachliche Puzzle-, Solver-, Economy-, Persistence- oder Featurelogik; die fünfzehn Ports sind vollständig; `ApplicationComposition` ist fail-closed; MonoBehaviours enthalten keine fachliche Entscheidungslogik; Guardrail-Tokens (`PlayerPrefs`, `Resources.Load`, `ServiceLocator`) kommen im Projektcode nicht vor. |
| `AK-05` | Dedizierte QA-Szene und `STP.Tests.Bootstrap.PlayMode.BootstrapCompositionSmoke` belegen die Behauptungen aus `ARCHITECTURE/MODULE_BOUNDARIES.md` Abschnitt 7 einschließlich Fehlschlag einer Doppelbindung. |
| `AK-06` | CI-Nachweise für Compile, EditMode, Bootstrap-PlayMode-Smoke, Android-Development-IL2CPP und den iOS-Export/Compile sind auf `feat/wp-021-unity-scaffold-recovery` commitgebunden dokumentiert; der Check `Architecture Validation / validate` läuft mit dem WP-021-Production-Manifest PASS; ohne Lizenzkonfiguration bleibt der `unity-evidence-guard` nachweisbar fail-closed. |
| `AK-07` | Coverage-Baseline nach Assembly aus dem tatsächlich ausgeführten EditMode-Lauf ist dokumentiert; der Mutationsstand ist wahrheitsgemäß dokumentiert (Domain/Solver typenlos); der Geräte-/Device-Farm-Zustand ist wahrheitsgemäß dokumentiert (kein physisches Gerät, keine Device-Farm). |
| `AK-08` | Der vollständige Diff gegen den `baseCommit` enthält ausschließlich die im Manifest erlaubten Pfade; `git diff --check`, Secret- und Produktquellenprüfung bestehen; keine Historienoperation aus archivierten Branches. |

## Tests

Vor Abschluss werden mindestens ausgeführt und commitbezogen dokumentiert:

1. Architecture-only-Validator mit `--self-test` aus der gepinnten Lock-Umgebung;
2. Production-Scope-Validator mit dem WP-021-Manifest und `--self-test` (kanonischer Befehl: `python tools/architecture-validation/validate.py --scope production --scope-manifest tools/architecture-validation/scopes/WP-021.production.scope.json --self-test`);
3. Trust-Anchor-Nachweis gemäß ADR-030 für WP-021 und Manifest;
4. Statischer Modulgraph-Check der realen `.asmdef`-Dateien in der CI (Projekt-Preflight);
5. Unity-Compile aller Assemblies, EditMode-Tests, PlayMode-Ausführung des `BootstrapCompositionSmoke` in der QA-Szene;
6. Android-Development-IL2CPP-Build und iOS-Export/Compile, jeweils commitgebunden in der CI;
7. Coverage-Messung des EditMode-Laufs und Dokumentation der Baseline nach Assembly;
8. `git diff --check`, Scope-, Secret- und Produktquellenprüfung sowie vollständige Delta-Prüfung gegen `e4f8cc1d0f0d0590fd7508992af464c0230ef314`;
9. Remote-Commit-Verifikation nach jedem Push.

Physische Geräte-, SDK-Sandbox- und Storetests bleiben **REQUIRED_LATER/NOT_EXECUTED**; ein physischer Gerätesmoke ist für WP-021 nicht erforderlich. Die unabhängige Astra-/Sol-QC erfolgt anschließend auf dem eingefrorenen PR-Head und ist nicht Teil dieses Implementierungsauftrags.

## Risikoklasse

**Hoch.** Erstes integrierbares Produktionsscaffold auf `main`: Es legt das Unity-Produktionsprojekt, den compilerwirksamen Modulgraph und die Unity-CI an und berührt damit Build- und Releasefähigkeit. Es trifft jedoch keine Produkt- oder Architekturentscheidung.

## Definition of Done

WP-021 ist nur abgeschlossen, wenn alle Akzeptanzkriterien einzeln erfüllt sind, beide Validator-Modi einschließlich Self-/Negativtests grün sind, die Unity-CI-Nachweise (Compile, EditMode, Bootstrap-PlayMode-Smoke, Android-Development-IL2CPP, iOS) auf dieser Branch commitgebunden dokumentiert sind, der vollständige Diff ausschließlich die im Manifest erlaubten Pfade enthält, `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md` denselben WP-021-Stand wiedergeben, der Arbeitsbaum sauber ist, die Branch gepusht ist und ein Pull Request gegen `main` erstellt ist. Es erfolgt **kein Merge** durch den Implementierungsagenten; die unabhängige Astra-/Sol-QC und die Integrationsentscheidung folgen danach.

## Vergleichsmatrix (Bindung an den aktuellen Vertrag)

Jeder technische Zweck wird an den aktuellen Architekturvertrag gebunden, nicht an historische Branchautorität. Die archivierte Planungsunterlage WP-019 diente nur als Strukturreferenz für diese Zuordnung.

| Scaffold-Element | Verbindliche Quelle im aktuellen Vertrag |
|---|---|
| Editorversion `6000.3.23f1`, Changeset `09d2ecc7fb28` | TECH_STACK.md Abschnitt 1, ADR-001; `ProjectVersion.txt` autoritativ |
| C# 9, Nullable, Warnungen als Fehler | ADR-002; `csc.rsp` je Assembly |
| Paketpins | TECH_STACK.md Abschnitt 1 und 3; Beleglage im Abschnitt „Paketpins" dieses Auftrags |
| Vierzehn Produktionsassemblies und Referenzmengen | MODULE_BOUNDARIES.md Abschnitt 3; ADR-013; ADR-018 (Bootstrap exakt neun interne Ziele) |
| Neun Testassemblies und physische Pfade | MODULE_BOUNDARIES.md Abschnitt 4 (Stand WP-010-Klarstellung) |
| Fünfzehn Application-Ports | MODULE_BOUNDARIES.md Abschnitt 6 |
| Composition Root, Reihenfolge, Smoke-Behauptungen, QA-Szene | MODULE_BOUNDARIES.md Abschnitt 7; ADR-018 |
| Build-Entrypoints und Plattformbaselines (IL2CPP, ARM64, minSdk 26, targetSdk 36, iOS 15.0, Xcode 26+) | BUILD_AND_RELEASE.md Abschnitte 5–9; TECH_STACK.md Abschnitt 2; ADR-012 |
| CI-Gates, Belegkategorien, Trust-Anchor | TEST_STRATEGY.md Abschnitt 11; ADR-026; ADR-030 |
| Coverage-/Mutations-Baseline | TEST_STRATEGY.md Abschnitt 12 |
| Unity-Personal-Aktivierung in der CI | Bestätigte Lizenzlage (Unity Personal) und Startgate der Geschäftsführung; Umsetzung über den Unity Licensing Client der gepinnten Editorversion |
| Geräte-/Device-Farm-Zustand | TEST_STRATEGY.md Abschnitt 11; wahrheitsgemäß: kein physisches Gerät, keine Device-Farm |

## Trust-Anchor-Nachweis

WP-021 und [`tools/architecture-validation/scopes/WP-021.production.scope.json`](../tools/architecture-validation/scopes/WP-021.production.scope.json) wurden gemeinsam im ersten Commit der Branch `feat/wp-021-unity-scaffold-recovery` eingeführt (Trust-Anchor-Commit `fc10c6141c7c720215f4f1f6670f87956baa346c`; beide Pfade mit Add-Status, im Elterncommit `e4f8cc1d0f0d0590fd7508992af464c0230ef314` nicht vorhanden). Der `baseCommit` des Manifests ist exakt dieser Elterncommit.

**Verifikation (2026-10-02, lokaler kanonischer Lauf):** Der Production-Scope-Validator lud Work Package und Manifest aus genau diesem Ankercommit: `SCOPE_CONTEXT workPackage=WP-021 base=e4f8cc1d0f0d0590fd7508992af464c0230ef314 head=fc10c6141c7c720215f4f1f6670f87956baa346c worktreeDirty=false`, `LOCAL_SCOPE PASS` (gemeinsamer Add-Status, `baseCommit` korrekt, historischer Manifestlink im WP-Blob exakt aufgelöst, Manifestblob bytegleich). Der Anker wurde auf `feat/wp-021-unity-scaffold-recovery` gepusht und ist remote verifiziert. Das Manifest ist seit dem Ankercommit byteunverändert.

## Umsetzung, Nachweise und Abschlussstand

**Historischer Bearbeitungsstatus (2026-10-02):** Implementierung und eigene Nachweise abgeschlossen; unabhängige Astra-/Sol-QC auf dem PR-Head ausstehend. Aktueller Korrekturstand: siehe nachfolgenden Abschnitt „Korrekturrunde 2026-10-03“.

### Umsetzung

Die Implementierung erfolgte vollständig neu aus dem aktuellen Architekturvertrag auf `main` (`e4f8cc1`): Unity-Projekt frisch mit der lokalen gepinnten Editorinstallation `6000.3.23f1` erzeugt und normalisiert; Modulgraph, Ports, Composition, Skelette, QA-Szene und Smoke aus `ARCHITECTURE/MODULE_BOUNDARIES.md` (Abschnitte 3, 4, 6, 7), ADR-018, `ARCHITECTURE/TECH_STACK.md`, `ARCHITECTURE/BUILD_AND_RELEASE.md` und `ARCHITECTURE/TEST_STRATEGY.md` implementiert. Es wurde **keine** Datei aus den archivierten Branches (`feat/wp-008-unity-scaffold`, `feat/wp-009-puzzle-kern`, `feat/wp-013-level-v2-pipeline`) oder aus `chore/ci-readiness-unity` gemergt, gecherry-picked, gerebased oder kopiert und keine PASS-Aussage daraus übernommen; alle Nachweise wurden auf dieser Branch erneut ausgeführt.

| Commit | Inhalt |
|---|---|
| `fc10c61` | Trust-Anchor: Work Package und Production-Scope-Manifest gemeinsam eingeführt (ADR-030). |
| `f2b694c` | `validate.yml`: Scope-Lauf auf das WP-021-Production-Manifest umgeschaltet. |
| `ae75b22` | Unity-Projektbasis `6000.3.23f1` (ProjectSettings, Paketlocks, Toolchain.lock). |
| `81ff049` | Normativer `.asmdef`-Modulgraph (14 Produktions-, 9 Testassemblies), 15 Ports, fail-closed `ApplicationComposition`, Skelette, `BootstrapComposition`, QA-Szene, `StpBuildEntrypoints`, `BootstrapCompositionSmoke`. |
| `d7b7274` | Unity-CI (`unity.yml`) mit Personal-Aktivierung und serialisierten Nachweisjobs. |
| `75715da` | Trailing-Whitespace in Unity-generierten Meta-/Asset-Dateien normalisiert (`git diff --check`). |
| `456267c` | Trust-Anchor-Nachweis im Work Package dokumentiert. |
| `2e12c93` | PlayMode-Coverage im Workflow; Toolchain.lock-Werte aus WP-021-eigenen CI-Läufen. |

### Lokale Verifikation (Unity `6000.3.23f1`, 2026-10-02)

| Nachweis | Ergebnis |
|---|---|
| Compile aller Assemblies | PASS (Import fehlerfrei, `-warnaserror+` aktiv) |
| EditMode | Exit 0 (`Test run completed. Exiting with code 0`; 1 Addressables-Paketstub `DocExampleCode.TestStub`, keine Scaffold-eigenen EditMode-Tests) |
| PlayMode | **5/5 PASS** (`QaScene_ComposesCompleteGraph_ExactlyOneBindingPerPort_NoProviderStart`, `Composition_RejectsNullPort_FailClosed`, `Composition_SecondBindProviders_FailsAsDoubleBinding`, `Composition_Get_UnboundPort_FailsWithoutFallback`, `Composition_Get_ProviderPort_BeforeBindProviders_FailsWithoutFallback`) |

### CI-Nachweise auf `feat/wp-021-unity-scaffold-recovery`

| Lauf | Commit | Ergebnis |
|---|---|---|
| `37069959561` | `75715da` | **Alle Jobs SUCCESS**: `Architecture Validation / validate` PASS, `project-preflight` PASS (statischer Modulgraph-Check), `unity-evidence-guard` PASS (ready-Pfad), drei Umgebungsjobs PASS, `unity-compile-editmode` PASS (`STP Preflight PASS`, EditMode Exit 0, OpenCover-Aufzeichnung), `unity-playmode-bootstrap-smoke` PASS (Exit 0), `unity-android-development-il2cpp` PASS (`STP Build PASS: Android -> Builds/Android/stp-qa-development.apk (801174900 Bytes)`), `unity-ios-export` PASS (`STP Build PASS: iOS -> Builds/iOS/Xcode`, `** BUILD SUCCEEDED **` unter Xcode 26.3). Personal-Aktivierung in jedem Lizenzjob (`Activation processed successfully`, `Seat ID …-UnityPersonal`), Seat jeweils danach freigegeben. |
| `37078237256` | `2e12c93` | **Alle Jobs SUCCESS** (Wiederholung nach Coverage-/Lock-Ergänzung; dieselben Nachweislinien). |
| `37083860138` | `37eb49b` (workflow_dispatch, Negativprobe mit `UNITY_RUNNERS_READY=false`) | **Fail-closed belegt**: `unity-evidence-guard` **FAILURE** mit den fünf NOT_EXECUTED/BLOCKED-Zeilen; alle vier Lizenzjobs **SKIPPED**; `project-preflight` und die drei Umgebungsjobs PASS. Kein Job täuschte einen Nachweis vor. Die Variable wurde unmittelbar danach wieder auf `true` gesetzt. |

Die Lizenzjobs liefen wegen der Ein-Instanz-Bedingung des Personal-Seats serialisiert (needs-Kette plus Concurrency-Gruppe). Ein vorübergehender Aktivierungsfehler (`400 Bad Request` am Lizenzserver) trat einmalig bei überlappenden Läufen auf (Seat-Konkurrenz) und war im unmittelbar folgenden Lauf wieder fehlerfrei — das dokumentierte Fluktuationsrisiko des Personal-Seats; die fail-closed-Struktur machte den Befund sichtbar, ohne ihn zu verbergen.

### Coverage-Baseline (erste Messung, WP-021-eigener PlayMode-Lauf `37078237256`, OpenCover)

Sequenzpunkte besucht/gesamt (besucht: `_0001.xml`, gesamt: `_0000.xml`); Branchpunkte liegen im Skelett nicht vor (`0/0`).

| Assembly | Sequenzpunkte | Methoden |
|---|---|---|
| `STP.Application` | 34/34 (**100,0 %**) | 8/8 |
| `STP.Bootstrap` | 40/41 (**97,6 %**) | 14/15 |
| `STP.Editor.Build` | 0/37 (**0,0 %** — Entrypoints werden nicht durch Tests, sondern durch die CI selbst ausgeführt) | 0/4 |
| `STP.MobileServices.Google` | 1/5 (**20,0 %** — Skelett; `Initialize` bleibt bewusst ungestartet) | 1/3 |
| `STP.MobileServices.Store` | 1/5 (**20,0 %** — Skelett; `Initialize` bleibt bewusst ungestartet) | 1/3 |
| `STP.Tests.Bootstrap.PlayMode` | 67/67 (**100,0 %**) | 11/11 |
| `STP.Puzzle.Domain`, `STP.Puzzle.Solver`, `STP.Infrastructure.Content`, `STP.Infrastructure.Persistence`, `STP.Platform`, `STP.Audio`, `STP.Presentation.UI`, `STP.Presentation.World`, `STP.Editor.Content` | keine Sequenzpunkte (typenlose Skelette) | — |

**Mutationsstand (wahrheitsgemäß):** `STP.Puzzle.Domain` und `STP.Puzzle.Solver` enthalten keine Typen; es existieren keine Mutationsgegenstände. Die erste reale Mutationsbaseline wird mit WP-022 (Puzzle-Kern) erhoben. Die EditMode-Coverage des Scaffolds ist wahrheitsgemäß **0 %** (der Scaffold besitzt noch keine eigenen EditMode-Tests; die Composition wird im PlayMode-Smoke ausgeführt und ist dort gemessen).

### Geräte-/Device-Farm-Zustand

Wahrheitsgemäß: Es existiert kein physisches Referenzgerät und keine freigegebene Device-Farm. Ein physischer Gerätesmoke ist für WP-021 nicht erforderlich und bleibt **REQUIRED_LATER/NOT_EXECUTED**; Store-/SDK-Sandboxtests bleiben **REQUIRED_LATER/NOT_EXECUTED**.

### Scope-, Diff- und Quellennachweise

| Nachweis | Ergebnis |
|---|---|
| Production-Scope-Validator (`--self-test`, kanonischer Befehl) | `LOCAL_SCOPE PASS` gegen `WP-021.production.scope.json` |
| Architecture-only-Validator (`--self-test`) | PASS in der CI (lokal unter Windows scheitert weiterhin nur das vorab existierende POSIX-Selbsttest-Artefakt `V03-005-ABSOLUTE`) |
| `git diff --check` gegen `e4f8cc1` | PASS (nach Meta-Whitespace-Normalisierung in `75715da`) |
| Manifest-Bytegleichheit | PASS (seit `fc10c61` unverändert) |
| Secret-/Produktquellenprüfung | Keine Secrets im Repository; Aktivierung ausschließlich über `secrets.UNITY_EMAIL`/`secrets.UNITY_PASSWORD`; keine Produktdateien außerhalb des Manifests |
| Historienabgrenzung | Keine Git-Übernahme aus archivierten Branches oder `chore/ci-readiness-unity` (Umsetzung aus dem Vertrag, eigenständig verifiziert) |

### Akzeptanzkriterien im Einzelnen

| AK | Ergebnis | Beleg |
|---|---|---|
| `AK-01` | Erfüllt | Trust-Anchor `fc10c61`, Verifikation oben; remote verifiziert. |
| `AK-02` | Erfüllt | `ProjectVersion.txt` exakt `6000.3.23f1`; C# 9/Nullable/Force Text/Visible Meta Files; Paketlocks gepinnt; `Toolchain.lock.md` mit WP-021-eigenen Runner-Werten (Run `37069959561`). |
| `AK-03` | Erfüllt | 14+9 Assemblies an den normierten Pfaden; exakte Referenzen; azyklisch; `noEngineReferences`; EditMode-Testassemblies Editor-only; statischer Check in CI `37069959561`/`37078237256` PASS. |
| `AK-04` | Erfüllt | Kompilation fehlerfrei mit `-warnaserror+`; 15 Ports; `ApplicationComposition` fail-closed inkl. `BindProviders`-Doppelbindung; keine Guardrail-Tokens; MonoBehaviours ohne Fachlogik. |
| `AK-05` | Erfüllt | QA-Szene + `BootstrapCompositionSmoke` 5/5 PASS lokal und in CI (PlayMode, Runs oben). |
| `AK-06` | Erfüllt | CI-Nachweise commitgebunden (Runs `37069959561`, `37078237256`); `Architecture Validation / validate` mit WP-021-Manifest PASS; Guard-Fail-closed ausgeführt belegt (Run `37083860138`: FAILURE + SKIPPED ohne Secrets-Bereitschaft). |
| `AK-07` | Erfüllt | Coverage-Baseline oben (realer PlayMode-Lauf); Mutationsstand wahrheitsgemäß dokumentiert; Gerätezustand wahrheitsgemäß dokumentiert. |
| `AK-08` | Erfüllt | Diff nur Manifest-Pfade; `git diff --check` PASS; Secret-/Produktquellenprüfung PASS; keine Historienoperation aus archivierten Branches. |

## Korrekturrunde 2026-10-03 – uncommitteter Zwischenstand

### Übernahme und Quellenprüfung

Pflichtlektüre nach AGENTS.md und den Voraussetzungen dieses Work Packages vollständig gelesen; Änderungen ausschließlich gegen den aktuellen Architekturvertrag implementiert. Ausgangscheckout: `C:/STP/Stammstrecken-Puzzle-WP008`, Branch `feat/wp-021-unity-scaffold-recovery`, HEAD `ca5236f64af3304203270739a959e2d82111ab82`. `git status --short --untracked-files=all` war bei Übernahme **leer**. Die im Auftrag angekündigten uncommitteten H1–H3-Korrekturen waren in diesem Checkout nicht vorhanden. `Assets/Editor/StpQcWp021Setup.cs` war ebenfalls nicht vorhanden. Es wurde nichts verworfen, zurückgesetzt, gelöscht, committed oder gepusht. Die folgenden Korrekturen stammen aus dieser Runde; sie sind kein Wiederherstellen aus historischen Branches.

### Befunde und Korrekturen

| Befund | Anfangsbefund im tatsächlichen Code | Aktueller Stand |
|---|---|---|
| H1 | `Bindings` gab das mutable Dictionary als `IReadOnlyDictionary` zurück; Cast und externe Mutation waren möglich. | **PASS lokal**: private Binding-Tabelle hinter `ReadOnlyDictionary`; generische/nichtgenerische Mutationsversuche werden abgewiesen. |
| H2 | Die fünf Providerports wurden einzeln validiert und sofort gesetzt; Null an Position 2–5 hinterließ Teilbindings. | **PASS lokal**: alle fünf Argumente werden vor der ersten Mutation geprüft; jeder Nullfall lässt alle fünf Ports ungebunden, lokale Bindings unverändert und vollständigen Retry möglich; Doppelbindung bleibt fail-closed. |
| H3 | UI/World waren parameterlos und wurden vor ApplicationRoot erstellt; kein tatsächliches Application-Wiring. | **PASS lokal**: lokale Portbindung → ApplicationRoot → UI/World mit derselben injizierten Root → Google/Store → Providerbindung. Szene prüft Referenzidentität; zusätzlicher Test prüft die echten Konstruktorinstruktionen der kompilierten Compose-Methode, ohne Produktions-Reflexions-Wiring. |
| H4 | URP-Paket und Global Settings existierten; Graphics/Quality hatten keine aktive Pipeline. | Implementiert und lokal geprüft: eingecheckte URP-Pipeline mit 2D-Renderer, Graphics-Zuweisung und explizite identische Zuweisung für alle sechs Quality-Level. Assettypen, Rendererreferenz, Defaultindex und effektive Pipeline sind geprüft. Neue CI-/Plattformnachweise ausstehend. |
| M1 | `managedStrippingLevel: {}`; Toolchain-Lock behauptete erst später zu setzenden Medium-Startwert. | Implementiert und lokal geprüft: Android und iOS explizit Medium, über Unity-API verifiziert; Toolchain-Lock korrigiert. Preflight und beide Plattformbuilds prüfen URP und Medium fail-closed. Neue commitgebundene IL2CPP-/iOS-CI-Nachweise ausstehend. |

Die neuen URP-Assets stammen aus dem 2D-Projekttemplate der installierten, gepinnten Unity-Version `6000.3.23f1` (`com.unity.template.2d-cross-platform-2d-6.1.6.tgz`), wurden unter den WP-021-Assetpfaden mit eigenen stabilen GUIDs angelegt und durch URP `17.3.0` importiert/normalisiert. Keine Datei und kein PASS aus einem historischen Branch wurden übernommen. Paketpins, Architektur-/Produktquellen, Scope-Manifest und Trust Anchor bleiben unverändert. Keine Puzzlefachlogik, keine SDKs, kein neues Work Package, keine lokale Unity-/UPM-Reparatur oder Installation.

### Reale lokale Tests und Grenzen

| Nachweis | Ergebnis / Artefakt |
|---|---|
| H1/H2-Negativprobe vor Korrektur | **11 Tests, 6 PASS / 5 FAIL**: H1 sowie Nullpositionen 2–5 scheitern wie erwartet; `Logs/wp021-before-playmode.xml`, `Logs/wp021-before-playmode-retry.log`. |
| H1–H3 nach Korrektur | **12/12 PASS** im realen Unity-PlayMode; `Logs/wp021-h123-playmode.xml`, zugehörige `.log`. |
| Gesamte Korrektursuite | **15/15 PASS** im realen Unity-PlayMode inkl. QA-Szene, kompiliertem Reihenfolgenachweis, URP und Medium; `Logs/wp021-corrections-playmode.xml`, zugehörige `.log`. Nach den URP-Normalisierungen des Android-Builds erneut **15/15 PASS** (`Logs/wp021-final-playmode.xml`, zugehörige `.log`). |
| EditMode | **1/1 PASS**, ausschließlich vorhandener Addressables-Paketstub; kein neuer Application-EditMode-Nachweis behauptet. `Logs/wp021-corrections-editmode.xml`, zugehörige `.log`. |
| Unity-/CI-Preflight | **PASS**: `STP Rendering/Stripping PASS: URP mit 2D-Renderer, 6 Quality-Level, Android/iOS Medium`, anschließend Versionspreflight PASS; `Logs/wp021-corrections-preflight.log`. Eigene Assemblies kompilieren mit Nullable und Warnungen als Fehler. |
| Statischer Modulgraph | **PASS**: bestehender Python-Code des `project-preflight` aus `unity.yml` unverändert lokal ausgeführt; 14 Produktions-/9 Testassemblies, exakte Allowlist. `Logs/wp021-modulgraph.log`. |
| Positiver Architektur-/Scope-Lauf | **PASS** (17 lokale Prüfgruppen); `Logs/wp021-before-scope-positive.log`. |
| Vollständiger Scope-Selbsttest | **FAIL nur `self-test:not-detected:V03-005-ABSOLUTE`**, bereits vor dieser Runde dokumentierter Windows-/POSIX-Befund; `Logs/wp021-before-scope-utf8.log`. Kein Abschwächen oder Ändern des Validators. Vollständiger grüner Selbsttest auf dem vorgesehenen Linux-CI-Weg bleibt erforderlich. |
| Manifestbytegleichheit | Git-Blob `734e7f676a7c80f49e2524a565055a96ab5ff33f` entspricht exakt dem Blob aus Trust Anchor `fc10c61`; Scope-Validator bestätigt gemeinsamen historischen WP-/Manifestanker. |
| Android Development IL2CPP / ARM64 | **PASS lokal** mit vorhandener Toolchain und API 36, `Logs/wp021-corrections-android.log`, `STP Build PASS`; APK `Builds/Android/stp-qa-development.apk`, tatsächliche Dateigröße **40.062.614 Bytes**, SHA-256 `4866fd9a99a97ce2571193e2f1c9908205ff55c123d7ba9c6b263f0de5e8ec87`. Keine Aussage über physischen Gerätesmoke. |
| Reales Linkerprofil | UnityLinker-RSP `Library/Bee/artifacts/rsp/11358533424958198342.rsp`: `--rule-set=Aggressive`, `--platform=Android`. Unity 6000.3 ordnet Medium diesem Ruleset zu; High wäre Experimental ([Unity-Referenzquelle](https://github.com/Unity-Technologies/UnityCsReference/blob/6000.3/Editor/Mono/Modules/BeeBuildPostprocessor.cs)). Die Unity-API-Tests bestätigen zusätzlich Medium für Android und iOS. |

Der erste sandboxierte Unity-Testversuch blieb bei der Lizenzinitialisierung stehen. Nur der selbst gestartete Testprozess wurde beendet; derselbe Test lief anschließend mit der vorhandenen Installation und vorhandenen Lizenz erfolgreich. Ein zweiter Zwischenversuch endete wegen des noch belegten Projekts. Diese Versuche bleiben als Logs erhalten und zählen nicht als PASS. Es erfolgte keine Lizenzaktivierung, Kontoreparatur, Paketänderung oder Installation durch diese Runde.

### Belegkategorien

- `LOCAL_DOCUMENT_STRUCTURE`: positive Validatorprüfungen; finaler positiver Lauf nach Dokumentnachführung in `Logs/wp021-final-scope-positive.log`.
- `LOCAL_ARCHITECTURE_SEMANTICS`: positive Validatorgruppen, tatsächlicher statischer Modulgraph und oben getrennt ausgewiesene reale Unitytests; Windows-Selbsttest weiterhin eingeschränkt.
- `MANUAL_ARCHITECTURE_REVIEW`: Architekturabgleich von Portbindung, Konstruktorinjektion, Reihenfolge, Assemblygrenzen und fehlendem Providerstart; keine neue Architekturentscheidung.
- `LOCAL_SCOPE`: aktueller Arbeitsbaum gegen unverändertes WP-021-Manifest; finaler positiver Lauf in `Logs/wp021-final-scope-positive.log`.
- `CONTRACT_ONLY`: übrige unveränderte Architekturfixtures; keine ausgeführte Fach-/SDK-/Storeimplementierung.
- `REQUIRED_LATER/NOT_EXECUTED`: neue commitgebundene CI-Nachweise, macOS/iOS-Compile sowie physische Geräte-/SDK-/Storeprüfungen. Die alten CI-Runs belegen den neuen Arbeitsbaum nicht.
- `BLOCKED` im vorangegangenen Zwischenstand: Commit/Push waren zunächst nicht freigegeben. Diese Einschränkung wurde anschließend ausdrücklich aufgehoben; die drei Produktfolgeblocker bleiben unverändert. Ein Dispatch auf dem alten Commit wäre weiterhin kein Korrekturbeleg.

### Zu behaltende Änderungen und temporäre Artefakte

Alle fachlichen Korrekturdateien dieser Runde sollen für die nächste WP-021-Prüfung **behalten** werden:

- ApplicationComposition.cs (H1/H2), ApplicationRoot.cs (korrekte Dokumentation der Bindungsphasen).
- BootstrapComposition.cs, UiPresentationSkeleton.cs, WorldPresentationSkeleton.cs (H3).
- BootstrapCompositionSmoke.cs (H1–H4/M1-Regressionen).
- StpBuildEntrypoints.cs (fail-closed URP-/Medium-Preflight vor jedem Plattformbuild).
- ProjectSettings/GraphicsSettings.asset, QualitySettings.asset (H4), ProjectSettings.asset (M1).
- Assets/DefaultVolumeProfile.asset und Assets/UniversalRenderPipelineGlobalSettings.asset (beim aktiven URP-Android-Build von Unity ergänzte Standard-Volume-Komponenten bzw. Runtime-Settings-Referenzen; für H4 behalten, keine manuelle visuelle Produktentscheidung).
- Assets/StammstreckenPuzzle/Settings.meta, Settings/StpUniversalRP.asset und `.meta`, Settings/StpRenderer2D.asset und `.meta` (H4; neu/untracked, dauerhafte Projektartefakte).
- Toolchain.lock.md und die drei Steuerungsdateien dieses WP, CURRENT_STATE.md und WORK_QUEUE.md (wahrheitsgemäßer Korrektur-/Übergabestand).

Unity schreibt beim Import zusätzliche Leerzeichen in serialisierte Dateien. Reine durch diese Testläufe erzeugte Whitespace-Differenzen werden ohne Git-Reset am Ende normalisiert; fachliche Unterschiede bleiben erhalten. `ShaderGraphSettings.asset` besitzt keinen fachlichen Korrekturdiff.

**Temporär, behalten:** vorhandene ignorierte `Library/`, `UserSettings/`, die älteren Logs `wp012-editmode.log`, `wp012-playmode.log`, `wp013-editmode.log` sowie sämtliche neuen `Logs/wp021-*` (einschließlich Test-XMLs und Fehlversuchen), lokale Buildausgaben und Unity-Temporärdateien. Alte Logs sind kein Nachweis des aktuellen WP-021-Korrekturstands. Keine Löschentscheidung getroffen. `Assets/Editor/StpQcWp021Setup.cs` fehlt in diesem Checkout; keine Lösch-/Behaltenentscheidung zu einer nicht vorliegenden Datei möglich.

Ein Scope-Lauf **während** des Android-Builds (`Logs/wp021-corrections-scope-positive.log`) scheiterte an vier vorübergehend vom bereits vorhandenen Performance-Test-Paket erzeugten `Assets/Resources/PerformanceTestRun*.json[.meta]` und an Unity-Whitespace. Das Paket entfernte seine eigenen temporären Resources im vorgesehenen Postprocess-Cleanup automatisch; der Agent löschte nichts und änderte kein Scope-Muster. Scope-Prüfungen erfolgen deshalb erst nach beendeten Unity-Prozessen. Whitespace wurde anschließend normalisiert, ohne fachliche Änderungen zurückzusetzen. Finale Befunde und Dateihashes werden in `Logs/wp021-verification-summary.json` aufbewahrt.

### Nächster Schritt / Abschlussgate

Die Geschäftsführung hat Commit/Push nach abschließender Prüfung freigegeben. Die Korrekturrunde bleibt bis zu neuen Nachweisen offen. Die bestehenden `validate.yml`-/`unity.yml`-Jobs laufen mit dem unveränderten WP-021-Manifest einschließlich Compile, EditMode, PlayMode, Coverage, Android-Development-IL2CPP und iOS-Export/Compile auf dem neuen Korrekturcommit. Die neuen Tests werden bereits vom bestehenden PlayMode-Job erfasst; beide Plattformentrypoints führen den erweiterten Preflight aus. Keine CI- oder Scopeabschwächung erforderlich. Der aktuelle Auftrag endet nach der vollständigen CI-Auswertung und dem Statusbericht; unabhängige Astra-/Sol-QC und Integration bleiben separate Gates. WP-021 ist nicht abgeschlossen, WP-022 bleibt gesperrt.

### Finale Dateiliste des uncommitteten Standes (2026-10-03)

Alle folgenden **21 Dateien behalten**; 16 versionierte Dateien geändert, 5 neue untracked Projektdateien. Keine dieser Dateien ist ein wegwerfbares Setup-Skript.

| Status | Pfad | Entscheidung |
|---|---|---|
| modified_tracked | `Assets/DefaultVolumeProfile.asset` | KEEP_WP021 |
| modified_tracked | `Assets/StammstreckenPuzzle/Scripts/Application/ApplicationComposition.cs` | KEEP_WP021 |
| modified_tracked | `Assets/StammstreckenPuzzle/Scripts/Application/ApplicationRoot.cs` | KEEP_WP021 |
| modified_tracked | `Assets/StammstreckenPuzzle/Scripts/Bootstrap/BootstrapComposition.cs` | KEEP_WP021 |
| modified_tracked | `Assets/StammstreckenPuzzle/Scripts/Editor/Build/StpBuildEntrypoints.cs` | KEEP_WP021 |
| modified_tracked | `Assets/StammstreckenPuzzle/Scripts/Presentation/UI/UiPresentationSkeleton.cs` | KEEP_WP021 |
| modified_tracked | `Assets/StammstreckenPuzzle/Scripts/Presentation/World/WorldPresentationSkeleton.cs` | KEEP_WP021 |
| modified_tracked | `Assets/StammstreckenPuzzle/Tests/PlayMode/Bootstrap/BootstrapCompositionSmoke.cs` | KEEP_WP021 |
| modified_tracked | `Assets/UniversalRenderPipelineGlobalSettings.asset` | KEEP_WP021 |
| modified_tracked | `PROJECT_CONTROL/CURRENT_STATE.md` | KEEP_WP021 |
| modified_tracked | `PROJECT_CONTROL/WORK_QUEUE.md` | KEEP_WP021 |
| modified_tracked | `ProjectSettings/GraphicsSettings.asset` | KEEP_WP021 |
| modified_tracked | `ProjectSettings/ProjectSettings.asset` | KEEP_WP021 |
| modified_tracked | `ProjectSettings/QualitySettings.asset` | KEEP_WP021 |
| modified_tracked | `Toolchain.lock.md` | KEEP_WP021 |
| modified_tracked | `WORK_PACKAGES/WP-021_Unity-Scaffold-Recovery.md` | KEEP_WP021 |
| untracked_new | `Assets/StammstreckenPuzzle/Settings.meta` | KEEP_WP021 |
| untracked_new | `Assets/StammstreckenPuzzle/Settings/StpRenderer2D.asset` | KEEP_WP021 |
| untracked_new | `Assets/StammstreckenPuzzle/Settings/StpRenderer2D.asset.meta` | KEEP_WP021 |
| untracked_new | `Assets/StammstreckenPuzzle/Settings/StpUniversalRP.asset` | KEEP_WP021 |
| untracked_new | `Assets/StammstreckenPuzzle/Settings/StpUniversalRP.asset.meta` | KEEP_WP021 |

Finaler positiver Architektur-/Scope-Lauf: **PASS (17 lokale Prüfgruppen)**. Finale Architecture-only- und Scope-Selbsttests: jeweils **FAIL ausschließlich V03-005-ABSOLUTE**, Protokolle in "Logs/wp021-final-architecture-selftest.log" und "Logs/wp021-final-scope-selftest.log". Vollständiger Diffcheck gegen e4f8cc1: **PASS**, Guardrail-/Secret-Pattern-Scan: **PASS**, Paket-/Produkt-/Architektur-/Workflow-/Manifestdiff: **leer**. HEAD unverändert ca5236f, Index unverändert, kein Commit/Push. APK-/Dateihashes und Testzahlen in "Logs/wp021-verification-summary.json". WP-021 bleibt offen.
### Freigegebene Abschlussprüfung vor dem Korrekturcommit

Die Geschäftsführung hat die 21 vorgesehenen Änderungen bestätigt und Commit/Push nach erneuter Prüfung freigegeben. Alle 18 technischen Dateien entsprechen per SHA-256 exakt dem zuvor getesteten Stand; ausschließlich die drei Steuerungsdateien wurden mechanisch auf die neue Freigabe nachgeführt. Kein Scope-, Trust-Anchor-, Architektur-, Produkt-, Paket- oder Workflowdiff wurde eingeführt.

Erneut real ausgeführt: PlayMode **15/15 PASS** (`Logs/wp021-precommit-playmode.xml`), EditMode **1/1 PASS** (Paketstub, `Logs/wp021-precommit-editmode.xml`), erweiterter Unity-Preflight **PASS** (`Logs/wp021-precommit-preflight.log`), unveränderter CI-Modulgraph **PASS 14+9** (`Logs/wp021-precommit-modulgraph.log`). Production-Scope positiv **PASS 17 Prüfgruppen** (`Logs/wp021-precommit-positive.log`); Architecture-only- und Scope-Selbsttests jeweils **FAIL ausschließlich V03-005-ABSOLUTE**, bekannte Windows-/POSIX-Einschränkung, keine Validatoränderung. Vollständiger Diffcheck, exaktes 21-Datei-Inventar, Guardrail-/Secretpatternscan und unveränderlicher Manifestblob **PASS**. Der vorherige lokale Android-IL2CPP-Nachweis bleibt per unverändertem technischem Dateistand und APK-SHA-256 nachvollziehbar; neu commitgebundene Plattformnachweise werden ausschließlich aus den jetzt folgenden CI-Läufen anerkannt.

PR #11 bleibt offen. Keine temporären Logs, Caches, Test-XMLs oder Buildausgaben werden eingecheckt. Der aktuelle Auftrag umfasst die neuen CI-Nachweise und ihren Abschlussbericht; unabhängige QC, Merge und Folgepakete werden nicht ausgeführt.
### Korrekturcommit und vollständige neue CI-Auswertung

Korrekturcommit: **`2868df4e2f3529d2d81d213ede745f1507558244`**, 21 freigegebene Projektdateien. Auf `feat/wp-021-unity-scaffold-recovery` gepusht; `git ls-remote` bestätigt denselben SHA. PR #11 bleibt offen und ungemerged. Der Arbeitsbaum war nach dem Push sauber; ignorierte Logs/Caches/Testartefakte wurden nicht committed.

Alle folgenden Läufe beziehen sich laut GitHub `head_sha` exakt auf diesen neuen Korrekturcommit, nicht auf alte Implementierungsstände:

| Lauf | Ereignis / Workflow | Vollständiges Ergebnis |
|---|---|---|
| [37149786899](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/37149786899) | push / Architecture Validation | COMPLETED / FAILURE: `validate` startete wegen GitHub-Account-/Billing-Sperre nicht. |
| [37149790370](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/37149790370) | pull_request / Architecture Validation | COMPLETED / FAILURE: gleicher Startblocker, kein Validator ausgeführt. |
| [37149786865](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/37149786865) | push / Unity CI | COMPLETED / FAILURE: fünf Startjobs FAILURE vor Runnerstart; fünf abhängige Jobs SKIPPED. |
| [37149790412](https://github.com/Al1l1l1l1l1/Stammstrecken-Puzzle/actions/runs/37149790412) | pull_request / Unity CI | COMPLETED / FAILURE: gleiche vollständige Jobverteilung. |

GitHub-Check-Annotations melden für alle zwölf fehlgeschlagenen Startjobs, dass sie wegen fehlgeschlagener Accountzahlungen oder eines zu niedrigen Ausgabenlimits nicht gestartet wurden; GitHub verweist auf „Billing & plans“. Welche dieser beiden Accountursachen konkret vorliegt, ist nicht bestimmt. macOS ergänzt einen allgemeinen Kapazitätshinweis; dieser ersetzt den expliziten Billing-Startblocker nicht. Runner-ID 0, leere Steps; ein exemplarischer Logabruf liefert 404/BlobNotFound, weil kein Joblog entstand. Alle vier Läufe besitzen **0 Artefakte**. Repositoryvariable `UNITY_RUNNERS_READY` ist lesend als `true` bestätigt; sie wurde nicht verändert. Keine Secrets, Zahlungsdaten, Ausgabenlimits oder Runnerkonfiguration geändert; keine sinnlosen Wiederholungen und keine lokale Umgebungsreparatur.

| Angeforderter neuer Nachweis | Ergebnis für den Korrekturcommit |
|---|---|
| Architecture Validation inkl. Self-/Negativtests | **BLOCKED / NOT_EXECUTED**, Job `validate` FAILURE vor Runnerstart. |
| Production Scope und Trust Anchor | **BLOCKED / NOT_EXECUTED in CI**; lokale Scope-/Bytegleichheits-/historische Anchorprüfung PASS bleibt getrennt ausgewiesen. |
| Compile / EditMode | `unity-compile-editmode` **SKIPPED**, kein neuer CI-Compile-/Testbeleg. |
| PlayMode | `unity-playmode-bootstrap-smoke` **SKIPPED**, keine neuen CI-Testzahlen. |
| Android Development IL2CPP | `unity-android-development-il2cpp` **SKIPPED**, kein neues CI-APK. Lokaler Build-PASS bleibt separat. |
| iOS Export / Xcode Compile | `unity-ios-export` **SKIPPED**, kein neuer iOS-Nachweis. |
| Coverage / Preflight | Coverage **NOT_EXECUTED**, keine OpenCover-Artefakte; `project-preflight` FAILURE vor Runnerstart. |
| Guard / Konfiguration / Umgebung | `unity-config` und alle drei `unity-environment-*` FAILURE vor Runnerstart; `unity-evidence-guard` **SKIPPED**. Weder ready- noch Negativpfad neu ausgeführt; alte Guard-Runs kein Korrekturbeleg. |

**Befundabschluss:** H1, H2 und H3 **PASS lokal / Vertragsreview**. H4 und M1 **implementiert, lokale Tests/Preflight und Android-PASS**, jedoch neue CI-/iOS-Nachweise blockiert. Kein Implementierungsfehler aus diesen nicht gestarteten Jobs ableitbar, kein CI-PASS behauptet. AK-01–AK-05 und AK-08 lokal geprüft; AK-06 und die neue CI-Coverage aus AK-07 bleiben für die Korrekturrunde offen. Vollständige DoD und unabhängige Astra-/Sol-QC sind nicht erfüllt. WP-021 bleibt offen, WP-022 gesperrt.

Die drei Steuerungsdateien dokumentieren diese tatsächliche Auswertung in einem reinen Evidenzcommit; alle technischen Dateien bleiben identisch zum Korrekturcommit. Auch dessen neue CI-Läufe müssen abschließend betrachtet werden. Der Implementierungsauftrag endet mit dem Statusbericht; der externe Billing-Blocker, ein späterer erneuter CI-Lauf und unabhängige QC sind nur dokumentierte offene Gates, keine gestarteten Folgeaufgaben. Keine Merge-/Close-Aktion.

Lokale Rohbefunde werden ausschließlich ignoriert unter `Logs/wp021-ci-*` aufbewahrt (Run-/Job-/Check-/Annotations-/Artefakt-JSON). `Logs/wp021-github.ps1` ist ein ignorierter temporärer API-Lesehelfer ohne gespeicherte Zugangsdaten. Keine temporären Artefakte werden versioniert oder gelöscht.
