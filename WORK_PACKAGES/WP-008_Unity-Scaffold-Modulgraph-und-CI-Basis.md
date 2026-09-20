# WP-008 – Unity-6.3-Produktionsscaffold, Modulgraph und ausführbare CI-Basis

## ID

`WP-008`

**Bearbeitungsstatus:** In Bearbeitung – **Gate A abgeschlossen (PASS)**. **Gate B weitestgehend umgesetzt, aber blockiert vor Abschluss** durch B-01 (Tests-Standort: fehlende Architekturentscheidung außerhalb des WP-008-Scopes) und B-02 (Unity-Lizenz/Runner nicht konfiguriert). Stand, Befunde und Nachweise siehe Abschnitt „Gate-B-Status, Befunde und Nachweise".

## Ziel

Eine belastbare technische Produktionsbasis für Stammstrecken-Puzzle ist hergestellt und nachgewiesen:

- Unity-Projekt mit exakt Unity **6000.3.23f1** (`ProjectSettings/ProjectVersion.txt` autoritativ), C# 9, projektweit aktivierten Nullable Reference Types, Asset Serialization **Force Text**, Version Control Mode **Visible Meta Files** sowie gepinnten und eingecheckten Paket-/Projektlocks (`Packages/manifest.json`, `Packages/packages-lock.json`).
- `Toolchain.lock.md` gemäß `ARCHITECTURE/TECH_STACK.md` Abschnitt 6.
- Der vollständige normative `.asmdef`-Modulgraph gemäß [`ARCHITECTURE/MODULE_BOUNDARIES.md`](../ARCHITECTURE/MODULE_BOUNDARIES.md): `STP.Puzzle.Domain`, `STP.Puzzle.Solver`, `STP.Application`, `STP.Infrastructure.Content`, `STP.Infrastructure.Persistence`, `STP.MobileServices.Google`, `STP.MobileServices.Store`, `STP.Platform`, `STP.Audio`, `STP.Presentation.UI`, `STP.Presentation.World`, `STP.Bootstrap`, `STP.Editor.Content`, `STP.Editor.Build` sowie die zugehörigen Testassemblies einschließlich `STP.Tests.Bootstrap.PlayMode`. Domain/Solver/Application werden nur als compilerfähige Modul-/Port-Skelette angelegt; Adapter-, Präsentations- und Bootstrap-Skelette nur, soweit sie für den vollständigen Composition Graph erforderlich sind.
- Dedizierte QA-Szene und der Compile-/Composition-Smoke `STP.Tests.Bootstrap.PlayMode.BootstrapCompositionSmoke` gemäß `ARCHITECTURE/MODULE_BOUNDARIES.md` Abschnitt 7.
- Ausführbare CI-Basis mit Nachweisen für Compile, EditMode, Bootstrap-PlayMode-Smoke, Android-Development-IL2CPP sowie den von `ARCHITECTURE/BUILD_AND_RELEASE.md` und `ARCHITECTURE/TEST_STRATEGY.md` verlangten iOS-Nachweis.
- Wahrheitsgemäß dokumentierter Geräte-/Device-Farm-Verfügbarkeitszustand.
- Erste Coverage-/Mutation-Baseline für den tatsächlich entstandenen Code gemäß `ARCHITECTURE/TEST_STRATEGY.md` Abschnitt 12.
- Mechanische Validator-/Governance-Nachführung ohne Regeländerung.

WP-008 ist bewusst von **WP-009** getrennt. WP-009 wird später den fachlichen Puzzle-Stack enthalten (Domain-Fachlogik, Solver, Level-v2/JCS, proof-v1, LevelValidationPipeline, Levelauthoring). Diese Trennung ist verbindlich und wird nicht wieder zusammengeführt.

## Voraussetzungen

- Verbindliche Lesereihenfolge aus [`AGENTS.md`](../AGENTS.md) ist abgeschlossen: Projektübergabe, Konzeptindex, Master-Spezifikation, [`PROJECT_CONTROL/CURRENT_STATE.md`](../PROJECT_CONTROL/CURRENT_STATE.md), [`PROJECT_CONTROL/WORK_QUEUE.md`](../PROJECT_CONTROL/WORK_QUEUE.md), [`PROJECT_CONTROL/WORK_PACKAGE_RULES.md`](../PROJECT_CONTROL/WORK_PACKAGE_RULES.md), [`PROJECT_CONTROL/DEFINITION_OF_DONE.md`](../PROJECT_CONTROL/DEFINITION_OF_DONE.md), [`PROJECT_CONTROL/AI_HANDOVER_RULES.md`](../PROJECT_CONTROL/AI_HANDOVER_RULES.md), [`ARCHITECTURE/ARCHITECTURE.md`](../ARCHITECTURE/ARCHITECTURE.md), [`ARCHITECTURE/MODULE_BOUNDARIES.md`](../ARCHITECTURE/MODULE_BOUNDARIES.md), [`ARCHITECTURE/TECH_STACK.md`](../ARCHITECTURE/TECH_STACK.md), [`ARCHITECTURE/BUILD_AND_RELEASE.md`](../ARCHITECTURE/BUILD_AND_RELEASE.md), [`ARCHITECTURE/TEST_STRATEGY.md`](../ARCHITECTURE/TEST_STRATEGY.md) sowie die angenommenen ADR-001, ADR-002, ADR-012, ADR-013, ADR-018, ADR-026 und ADR-030 einschließlich der für das First-Scaffold-Gate unmittelbar referenzierten ADR-Historie (ADR-017, ADR-022).
- [`WP-007`](../WORK_PACKAGES/WP-007_CI-Setup.md) ist abgeschlossen; das CI-Gate vor Produktionscoding ist eingerichtet und commitgebunden nachgewiesen. WP-007 dient als Referenz für die Trust-Anchor-/Scope-Mechanik; sein Scope wird nicht kopiert.
- Arbeitsbranch ist exakt `feat/wp-008-unity-scaffold`, Ausgangsstand `origin/main` mit Base-Commit `3c1a6988c1aab2084763edf772b6adc268874865`; der Branch enthält zu Beginn keine eigenen Änderungen.
- Der Production-Scope ist vor allen fachlichen Änderungen im kanonischen Manifest [`tools/architecture-validation/scopes/WP-008.production.scope.json`](../tools/architecture-validation/scopes/WP-008.production.scope.json) verankert. Dieses Work Package und das Manifest werden gemäß ADR-030 im selben Trust-Anchor-Commit eingeführt; der `baseCommit` des Manifests ist der Elterncommit dieses gemeinsamen Ankers (`3c1a6988c1aab2084763edf772b6adc268874865`). Der Validator lädt Work Package und Manifest aus genau diesem historischen Commit und beweist ihre kanonische Bindung.
- Die in ADR-012 für das erste Produktionsscaffold vorgeschriebene Reichweitenprüfung wird in Gate A durchgeführt und in diesem Work Package dokumentiert, bevor irgendeine Scaffold-Datei entsteht. Erfordert die Prüfung eine neue Architekturentscheidung, eine Änderung einer angenommenen Baseline oder einen Nachfolge-ADR, ist WP-008 sofort zu stoppen: kein Unity-Scaffold, keine `.asmdef`, kein Produktionscode; die fehlende Entscheidung ist exakt zu melden.

### Bekannte Abschlussrisiken (keine Startblocker)

| Risiko | Stand |
|---|---|
| Unity-Lizenz/Secret | Derzeit **nicht als verfügbar nachgewiesen**. Kann den Abschluss, nicht den Start von WP-008 blockieren. |
| Reproduzierbare Unity-Runner (Linux/Android) sowie macOS-/iOS-Nachweis | Derzeit **nicht als verfügbar nachgewiesen**. Können den Abschluss, nicht den Start von WP-008 blockieren. |
| Physischer Gerätesmoke | Für WP-008 **nicht erforderlich**. Der aktuelle Geräte-/Device-Farm-Zustand ist lediglich wahrheitsgemäß zu dokumentieren. |
| `BLOCKER-PROD-001/002/003` | Blockieren WP-008 nicht und bleiben unverändert offen und fail-closed; [`ARCHITECTURE/OPEN_BLOCKERS.md`](../ARCHITECTURE/OPEN_BLOCKERS.md) bleibt unverändert. |

## Scope

WP-008 dient ausschließlich der Herstellung der technischen Produktionsbasis. Erlaubt sind ausschließlich:

1. **Gate A (vorgelagert, vor jeder Scaffold-Datei):** dieses Work Package, das Production-Scope-Manifest, der gemeinsame Trust-Anchor-Commit nach ADR-030 mit Base `3c1a6988c1aab2084763edf772b6adc268874865`, die Validator-Prüfung des Trust Anchors sowie die ADR-012-Reichweitenprüfung mit dokumentiertem Ergebnis (PASS oder BLOCKED).
2. **Unity-Projektanlage (Gate B, erst nach dokumentiertem ADR-012-PASS):** Unity-Projektdateien unter `ProjectSettings/`, `Packages/` und `Assets/StammstreckenPuzzle/` mit exakt Unity 6000.3.23f1, C# 9, Nullable, Force Text, Visible Meta Files und gepinnten, eingecheckten Paket-/Projektlocks; `Toolchain.lock.md`.
3. **Normativer `.asmdef`-Modulgraph:** alle Produktionsassemblies und Testassemblies gemäß der einzigen normativen Allowlist in `ARCHITECTURE/MODULE_BOUNDARIES.md` Abschnitt 3 und 4, einschließlich `noEngineReferences: true` für `STP.Puzzle.Domain`, `STP.Puzzle.Solver` und `STP.Application` und der exakten Bootstrap-Referenzmenge aus ADR-018. Domain/Solver/Application nur als compilerfähige Modul-/Port-Skelette; minimale compilerfähige Port-, Adapter-, Präsentations- und Bootstrap-Skelette ausschließlich soweit für den vollständigen Composition Graph erforderlich. Keine fachliche Puzzle-, Solver-, Economy-, Persistence- oder sonstige Featurelogik.
4. **QA-Szene und Composition-Smoke:** dedizierte QA-Szene und `STP.Tests.Bootstrap.PlayMode.BootstrapCompositionSmoke` mit den in `ARCHITECTURE/MODULE_BOUNDARIES.md` Abschnitt 7 geforderten Behauptungen (genau ein Binding pro Application-Port, vollständiger Application-/UI-/World-Graph, keine nicht dokumentierten Null-/Fallback-Ports, kein optionaler Providerstart).
5. **CI-Basis:** CI-Nachweise für Compile, EditMode, Bootstrap-PlayMode-Smoke, Android-Development-IL2CPP sowie den nach `ARCHITECTURE/BUILD_AND_RELEASE.md` Abschnitt 5 und `ARCHITECTURE/TEST_STRATEGY.md` Abschnitt 11 verlangten iOS-Nachweis; Umstellung von `STP_SCOPE_MANIFEST` in `.github/workflows/validate.yml` auf das WP-008-Manifest sowie der neue Unity-CI-Workflow `.github/workflows/unity.yml`. Alle Actions bleiben per vollständigem Commit-SHA gepinnt; Least-Privilege-Permissions.
6. **Dokumentationspflichten:** wahrheitsgemäßer Geräte-/Device-Farm-Verfügbarkeitszustand, erste Coverage-/Mutation-Baseline für den tatsächlich entstandenen Code, mechanische Validator-/Governance-Nachführung (WP-008-Inventar, kanonischer WP-008-Befehl, Schema-Beispielnachführung in `tools/architecture-validation/validate.py` und `tools/architecture-validation/README.md`) sowie die Abschluss-/Zwischenstandsdokumentation in diesem Work Package, `PROJECT_CONTROL/CURRENT_STATE.md` und `PROJECT_CONTROL/WORK_QUEUE.md`.

Testassemblies werden so angelegt, dass sie dem Modulgraphen aus `ARCHITECTURE/MODULE_BOUNDARIES.md` entsprechen und vom Unity Test Framework ausführbar sind. Erweist sich die vorgesehene Repository-Struktur dafür als nicht ohne Architekturänderung umsetzbar, ist WP-008 zu stoppen und der Befund zu melden; keine eigene Abweichung.

## Betroffene Dateien/Module

Alle folgenden Pfade stehen im vorab verankerten Production-Scope-Manifest. Nicht vorhandene Zielartefakte werden neu angelegt.

| Pfad oder Modul | Zulässige Änderung |
|---|---|
| `WORK_PACKAGES/WP-008_Unity-Scaffold-Modulgraph-und-CI-Basis.md` | Neu: Auftrag, Trust-Anchor-Verweis, ADR-012-Reichweitenprüfung, Status, Akzeptanznachweise und Abschlussdokumentation. |
| `tools/architecture-validation/scopes/WP-008.production.scope.json` | Neu: vorab verankerte Production-Scope-Allowlist; nach dem Trust-Anchor-Commit byteunveränderlich. |
| `ProjectSettings/**` | Neu: Unity-Projekteinstellungen einschließlich autoritativer `ProjectVersion.txt` (6000.3.23f1), Force Text, Visible Meta Files, IL2CPP-/Plattformbaselines gemäß `ARCHITECTURE/TECH_STACK.md`. |
| `Packages/*` | Neu: `manifest.json` und `packages-lock.json` mit exakt gepinnten Paketversionen gemäß `ARCHITECTURE/TECH_STACK.md`. |
| `Assets.meta`, `Assets/*`, `Assets/StammstreckenPuzzle/**` | Neu: Unity-Asset-Struktur, `.asmdef`-Dateien, compilerfähige Modul-/Port-/Adapter-/Präsentations-/Bootstrap-Skelette, QA-Szene und zugehörige Meta-Dateien. |
| `Tests.meta`, `Tests/**` | Neu: Testassemblies gemäß `ARCHITECTURE/MODULE_BOUNDARIES.md` Abschnitt 4 einschließlich `STP.Tests.Bootstrap.PlayMode`. |
| `Toolchain.lock.md` | Neu: Toolchain-Lock gemäß `ARCHITECTURE/TECH_STACK.md` Abschnitt 6. |
| `.github/workflows/validate.yml` | Ausschließlich Umstellung des Scope-Laufs auf das WP-008-Production-Manifest (`STP_SCOPE_MANIFEST` und Scope-Modus). |
| `.github/workflows/unity.yml` | Neu: Unity-CI-Workflow für Compile, EditMode, Bootstrap-PlayMode-Smoke, Android-Development-IL2CPP und iOS-Nachweis; SHA-gepinnte Actions, Least Privilege. |
| `tools/architecture-validation/validate.py` | Ausschließlich mechanische WP-008-Nachführung (Inventar, Schema-Beispiele, Statuserwartungen). Keine Regeländerung, keine Abschwächung. |
| `tools/architecture-validation/README.md` | Ausschließlich kanonischer WP-008-Befehl und WP-008-Inventarnachführung. |
| `PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md` | Ausschließlich WP-008-Stand, Trust-Anchor und nächster Schritt. |

## Ausdrücklich nicht erlaubte Änderungen

Nicht erlaubt sind insbesondere:

- Puzzle-Domain-/Command-/Completion-Fachlogik; Solver, Constraint Propagation, Suche, Deduction Tracer, Hintlogik oder Solverperformance; Level-v2-Parser oder fachlicher Levelvertrag; JCS-Hashimplementierung; proof-v1-Erzeugung; LevelValidationPipeline; Levelauthoring-/Editorfenster oder fachliche Batchvalidierung; Generator; konkrete Rätsel.
- Progression, Economy, Persistenzfachlogik, Ads, IAP, Consent, Analytics oder fertige UI-Funktionen; SDK-Importe oder -Aktivierungen (insbesondere Google Mobile Ads, Firebase Analytics/Crashlytics, Unity IAP) über das für den Composition Graph minimal erforderliche Skelett hinaus.
- Produktentscheidungen jeder Art.
- Änderungen an `BLOCKER-PROD-001`, `BLOCKER-PROD-002` oder `BLOCKER-PROD-003` sowie an [`ARCHITECTURE/OPEN_BLOCKERS.md`](../ARCHITECTURE/OPEN_BLOCKERS.md).
- Neue Architekturentscheidungen oder Änderungen an ADR-Entscheidungskörpern ohne vorherigen STOPP; Änderungen an `ARCHITECTURE/` und `DECISIONS/`.
- Änderungen an den Scope-Manifesten von WP-003 bis WP-007; jede Änderung am WP-008-Manifest nach dem Trust-Anchor-Commit.
- Änderungen an Konzeptdateien unter `Stammstrecken_Puzzle_Konzept_00-15/`.
- Merge nach `main`, Pull Request, Force Push, Rebase bestehender Remote-Historie; Pushes ausschließlich auf `feat/wp-008-unity-scaffold`.
- Beiläufige Refactorings oder nicht aufgelistete Dateien/Module.

## Akzeptanzkriterien

| ID | Prüfkriterium |
|---|---|
| `AK-01` | Trust-Anchor: WP-008 und `WP-008.production.scope.json` wurden im selben historischen Add-Commit eingeführt, der historische WP-Blob verweist exakt auf das Manifest, der Manifestblob ist bytegleich, `baseCommit` ist der Elterncommit des Ankers (`3c1a6988c1aab2084763edf772b6adc268874865`); der Anker ist remote auf `feat/wp-008-unity-scaffold` verifiziert. |
| `AK-02` | Die ADR-012-Reichweitenprüfung ist mit aktuellen Store-/Zielgruppen- und Geräteinformationen dokumentiert; Ergebnis PASS ohne neue Architekturentscheidung, andernfalls STOPP vor jeder Scaffold-Datei. |
| `AK-03` | `ProjectSettings/ProjectVersion.txt` trägt exakt `6000.3.23f1`; C# 9, Nullable, Force Text und Visible Meta Files sind eingestellt; `Packages/manifest.json` und `Packages/packages-lock.json` sind gepinnt und eingecheckt; `Toolchain.lock.md` existiert. |
| `AK-04` | Der `.asmdef`-Modulgraph bildet die normative Allowlist aus `ARCHITECTURE/MODULE_BOUNDARIES.md` Abschnitt 3 und 4 vollständig ab: alle genannten Produktions- und Testassemblies existieren, Referenzmengen entsprechen exakt der Tabelle (Bootstrap exakt neun interne Ziele), der Graph ist azyklisch, `noEngineReferences: true` für Domain/Solver/Application. |
| `AK-05` | Sämtliche Skelette kompilieren ohne fachliche Puzzle-, Solver-, Economy-, Persistence- oder Featurelogik; MonoBehaviours enthalten keine fachliche Entscheidungslogik. |
| `AK-06` | Dedizierte QA-Szene und `STP.Tests.Bootstrap.PlayMode.BootstrapCompositionSmoke` belegen die Behauptungen aus `ARCHITECTURE/MODULE_BOUNDARIES.md` Abschnitt 7. |
| `AK-07` | CI-Nachweise für Compile, EditMode, Bootstrap-PlayMode-Smoke, Android-Development-IL2CPP und den verlangten iOS-Nachweis sind commitgebunden dokumentiert; der Check `Architecture Validation / validate` läuft mit dem WP-008-Production-Manifest PASS. |
| `AK-08` | Der Geräte-/Device-Farm-Verfügbarkeitszustand ist wahrheitsgemäß dokumentiert; kein physischer Gerätesmoke wird behauptet. |
| `AK-09` | Coverage-/Mutation-Baseline für den tatsächlich entstandenen Code ist dokumentiert. |
| `AK-10` | Der vollständige Diff gegen den `baseCommit` enthält ausschließlich die im Manifest erlaubten Pfade; `git diff --check`, Secret-, Produktdatei- und `main`-Unverändertheitscheck bestehen. |
| `AK-11` | `BLOCKER-PROD-001/002/003` bleiben unverändert offen und fail-closed; kein Merge nach `main`, kein Pull Request, keine neue Architektur- oder Produktentscheidung. |

## Tests

Vor Abschluss werden mindestens ausgeführt und commitbezogen dokumentiert:

1. Architecture-only-Validator mit `--self-test` aus der gepinnten Lock-Umgebung;
2. Production-Scope-Validator mit dem WP-008-Manifest und `--self-test` (kanonischer Befehl: `python tools/architecture-validation/validate.py --scope production --scope-manifest tools/architecture-validation/scopes/WP-008.production.scope.json --self-test`);
3. Trust-Anchor-Nachweis gemäß ADR-030 für WP-008 und Manifest;
4. Unity-Compile aller Assemblies, EditMode-Tests, PlayMode-Ausführung des `BootstrapCompositionSmoke` in der QA-Szene;
5. Android-Development-IL2CPP-Build und der nach `ARCHITECTURE/BUILD_AND_RELEASE.md`/`ARCHITECTURE/TEST_STRATEGY.md` verlangte iOS-Nachweis, jeweils commitgebunden in der CI;
6. Coverage-/Mutation-Baseline-Messung für den entstandenen Code;
7. `git diff --check`, Scope-, Secret-, Produktdatei- und `main`-Unverändertheitscheck sowie vollständige Delta-Prüfung gegen `3c1a6988c1aab2084763edf772b6adc268874865`;
8. Remote-Commit-Verifikation nach jedem Push.

Physische Geräte-, SDK-Sandbox- und Storetests bleiben **REQUIRED_LATER/NOT_EXECUTED**; ein physischer Gerätesmoke ist für WP-008 nicht erforderlich. Fehlen Unity-Lizenz/Secret oder reproduzierbare Unity-Runner (Linux/Android) beziehungsweise der macOS-/iOS-Nachweis, ist der betroffene Abschlussnachweis als Blocker zu dokumentieren; eine Fertigmeldung ohne diese Nachweise ist unzulässig.

## Risikoklasse

**Hoch.** Erstes Produktionscode-Work-Package: Es legt das Unity-Produktionsprojekt, den compilerwirksamen Modulgraph und die Unity-CI an und berührt damit Build- und Releasefähigkeit. Es trifft jedoch keine Produkt- oder Architekturentscheidung; die ADR-012-Reichweitenprüfung ist als vorgeschaltetes Gate mit expliziter STOPP-Regel ausgeführt.

## Definition of Done

WP-008 ist nur abgeschlossen, wenn alle Akzeptanzkriterien einzeln erfüllt sind, beide Validator-Modi einschließlich Self-/Negativtests lokal und in der CI grün sind, die Unity-CI-Nachweise (Compile, EditMode, Bootstrap-PlayMode-Smoke, Android-Development-IL2CPP, iOS) commitgebunden dokumentiert sind, der vollständige Diff ausschließlich die im Manifest erlaubten Pfade enthält und der Abschlusscommit auf `feat/wp-008-unity-scaffold` gepusht und remote verifiziert wurde.

`PROJECT_CONTROL/CURRENT_STATE.md`, `PROJECT_CONTROL/WORK_QUEUE.md`, `tools/architecture-validation/README.md` und dieses Work Package geben denselben WP-008-Stand wieder. Es erfolgt kein Merge und keine Änderung an `main`. Die fachliche Puzzle-Implementierung bleibt vollständig WP-009 vorbehalten.

## Trust-Anchor-Nachweis (Gate A)

WP-008 und [`tools/architecture-validation/scopes/WP-008.production.scope.json`](../tools/architecture-validation/scopes/WP-008.production.scope.json) wurden gemeinsam im Trust-Anchor-Commit `8d3e245fcab8bce42b5e8efdded480f47dacfbb8` eingeführt (beide Pfade mit Add-Status, im Elterncommit nicht vorhanden). Der Elterncommit des Ankers ist `3c1a6988c1aab2084763edf772b6adc268874865` und entspricht exakt dem `baseCommit` des Manifests. Der Anker wurde ausschließlich auf `feat/wp-008-unity-scaffold` gepusht und remote verifiziert; `main` bleibt unverändert auf `3c1a6988c1aab2084763edf772b6adc268874865`. Der nachfolgende Commit `3c1ffe939e55d9959dc4e3a1e8fecbbb2ee54660` stellte den CI-Scope-Lauf in [`.github/workflows/validate.yml`](../.github/workflows/validate.yml) mechanisch auf das WP-008-Production-Manifest um (`STP_SCOPE: production`, `STP_SCOPE_MANIFEST`). Das Manifest ist seit dem Ankercommit byteunveränderlich.

## ADR-012-Reichweitenprüfung (Gate A)

Gemäß [ADR-012](../DECISIONS/ADR-012-mobile-plattformbaselines.md) wurde die Reichweite vor dem ersten Produktions-Scaffold mit aktuellen Store-/Zielgruppen- und Geräteinformationen geprüft. **Stichtag der Prüfung: 2026-09-14.**

### Geprüfte Baselines (Architecture v1.0, unverändert)

| Plattform | Deployment-Minimum | Upload-/Releaseziel |
|---|---|---|
| Android | Android 8.0 / API 26, ARM64, IL2CPP | `targetSdkVersion`/`compileSdkVersion` mindestens API 36 sowie aktuelles Google-Play-Mandat |
| iOS | iOS 15.0, ARM64, IL2CPP | Xcode 26+ mit iOS-26-SDK+ sowie aktuelles App-Store-Mandat |

### Befunde mit aktuellen Informationen

| Prüfpunkt | Aktueller Befund (Stichtag 2026-09-14) | Auswirkung auf die Baseline |
|---|---|---|
| Google-Play-Uploadziel | Seit 2026-08-31 müssen neue Apps und Updates Android 16 (API 36) oder höher targeten; Fristverlängerung bis 2026-11-01 nur auf Antrag. | Keine: das Architekturmuster „mindestens API 36 und bei Release aktuelles Mandat" deckt dies bereits ab; das Deployment-Minimum ist davon nicht berührt. |
| App-Store-Uploadziel | Seit 2026-04-28 müssen Uploads mit Xcode 26+ und einem 26er-SDK gebaut sein; das betrifft nur das Build-SDK, nicht das Deployment Target. | Keine: die Architektur verlangt bereits Xcode 26+/iOS-26-SDK+; iOS 15.0 als Deployment Target bleibt zulässig. |
| Unity-6000.3-Minima | Unity 6000.3 unterstützt Android ab API 25 und iOS ab 15.0 als Mindest-Deployment-Targets. | Keine: API 26 und iOS 15.0 sind mit der gepinnten Editorversion baubar; iOS 15.0 ist exakt die Unity-Untergrenze. |
| SDK-Linie Google Mobile Ads 11.5.0 | Mindestanforderungen Android API 23 und iOS 13.0 laut aktueller Anbieterdoku. | Keine: beide Minima liegen unter den Baselines; keine notwendige SDK-Linie ist inkompatibel. |
| Zielgruppenabdeckung Android | API 26+ (Android 8.0+, 2017) deckt etwa 96 % der aktiven Geräte weltweit ab. | Keine relevante Zielgruppenausgrenzung für die deutschsprachige, breite Puzzle-Zielgruppe. |
| Zielgruppenabdeckung iOS | iOS 15 läuft ab iPhone 6s/SE (2015); Geräte auf iOS 15 oder älter liegen 2026 nur noch im niedrigen einstelligen Prozentbereich. | Keine relevante Zielgruppenausgrenzung. |
| Vorhandene Geräte | Derzeit ist **kein** physisches Referenzgerät und **keine** freigegebene Device-Farm als verfügbar nachgewiesen (wahrheitsgemäßer Zustand). | Für WP-008 ist kein physischer Gerätesmoke erforderlich; die physische Gerätematrix aus ADR-012/`ARCHITECTURE/TEST_STRATEGY.md` Abschnitt 10 bleibt verbindlich und ist vor den entsprechenden späteren Geräte-/Release-Gates bereitzustellen. Diese Gates bleiben REQUIRED_LATER/NOT_EXECUTED. |
| Unity 6000.3 mit Xcode 26 | Die Unity-Doku empfiehlt „Xcode 16 oder später" und belegt die Kombination mit Xcode 26 nicht explizit. | Kein Architekturbefund; die Kombination ist im iOS-CI-Nachweis von Gate B praktisch zu verifizieren. |

Quellen: [Google Play target API level](https://developer.android.com/google/play/requirements/target-sdk), [Apple Upcoming Requirements](https://developer.apple.com/news/upcoming-requirements/), [Unity 6000.3 Android requirements](https://docs.unity3d.com/6000.3/Documentation/Manual/android-requirements-and-compatibility.html), [Unity 6000.3 iOS requirements](https://docs.unity3d.com/6000.3/Documentation/Manual/ios-requirements-and-compatibility.html), [Google Mobile Ads Unity Quickstart](https://developers.google.com/admob/unity/quick-start), [apilevels.com](https://apilevels.com/), [Apple App Store support – Gerätestatistik](https://developer.apple.com/support/app-store/).

### Ergebnis der Reichweitenprüfung

**PASS.** Die Prüfung bestätigt die bestehenden Architecture-v1.0-Baselines vollständig: keine relevante Zielgruppenausgrenzung, keine inkompatible notwendige SDK-Linie, keine Abweichung von den Store-Mandaten. **Es ist keine neue Architekturentscheidung, keine Änderung einer angenommenen Baseline und kein Nachfolge-ADR erforderlich.** Damit ist Gate A abgeschlossen; Gate B (Unity-Scaffold) ist freigegeben, aber in diesem Arbeitsstand noch nicht begonnen.

## Gate-B-Status, Befunde und Nachweise

### Umgesetzter Scaffold-Stand

**Unity-Projektbasis.** `ProjectSettings/ProjectVersion.txt` trägt exakt `6000.3.23f1` mit Changeset `09d2ecc7fb28` (autoritativ gemäß ADR-001). `ProjectSettings/EditorSettings.asset` setzt Force Text (`m_SerializationMode: 2`) und Visible Meta Files. `ProjectSettings/ProjectSettings.asset` setzt Linear Color Space, Input System (New) (`activeInputHandler: 1`), `.NET Standard 2.1` (`apiCompatibilityLevel: 3`), minSdk 26, targetSdk 36, iOS Deployment Target 15.0 und ARM64; das Scripting Backend wird je Plattform durch die Build-Entrypoints auf IL2CPP gesetzt (Mono bleibt Editor-/Entwicklungsiteration). `ProjectSettings/EditorBuildSettings.asset` verankert die QA-Szene. `Packages/manifest.json` und `Packages/packages-lock.json` enthalten ausschließlich Versionen, die aus den Architekturverträgen (TECH_STACK.md) oder der quellenfest belegten Unity-6000.3-Core-Linie ableitbar sind; `Toolchain.lock.md` dokumentiert alle Pins und markiert nicht auslesbare Werte wahrheitsgemäß als NOT_DETERMINED. Die `.meta`-Abdeckung unter `Assets/` ist vollständig (Visible Meta Files).

**Normativer Modulgraph.** Alle vierzehn Produktionsassemblies aus [`ARCHITECTURE/MODULE_BOUNDARIES.md`](../ARCHITECTURE/MODULE_BOUNDARIES.md) Abschnitt 3 existieren als `.asmdef` mit exakt den normativen Referenzmengen: Bootstrap mit exakt den neun internen Zielen (ADR-018), `noEngineReferences: true` für `STP.Puzzle.Domain`, `STP.Puzzle.Solver` und `STP.Application`, Editor-Grenze `includePlatforms: ["Editor"]` für `STP.Editor.Content` und `STP.Editor.Build`. Jede Assembly trägt eine `csc.rsp` mit `-nullable:enable` und `-warnaserror+` (ADR-002). Domain und Solver enthalten bewusst keine Typen: Jeder Typ wäre entweder verbotene Fachlogik oder verbotener Platzhalter; die Fachverträge folgen mit WP-009. `STP.Application` enthält die fünfzehn normativen Port-Skelette (Abschnitt 6), `ApplicationComposition` (jeder Port genau einmal gebunden, null-frei, fail-closed bei Doppelbindung oder ungebundenem Zugriff, kein stiller Fallback) und `ApplicationRoot`. Die acht Adapter-/Präsentationsmodule enthalten je ein dokumentiertes Skelett ohne Fachlogik, ohne Dateizugriff und ohne SDK-Referenz.

**Bootstrap/Composition Root.** `BootstrapComposition.Compose()` erstellt den Graphen in der dokumentierten Reihenfolge (Content, Persistenz, Clock/Lifecycle, Audio, Application, Presentation, danach externe Adapter; ADR-018, MODULE_BOUNDARIES.md Abschnitt 7) per manueller Konstruktorinjektion. Kein Service Locator, keine veränderliche Registry, kein Reflexions-Wiring, keine Szenensuche, kein Providerstart: Die Google-/Store-Adapter werden erstellt, aber nicht initialisiert. `BootstrapCompositionResult` legt alle Instanzen offen, damit der Smoke jede Portbindung referenzgleich prüfen kann. `BootstrapInstaller` ist der einzige Entry-Installer der dedizierten QA-Szene `Assets/StammstreckenPuzzle/Scenes/QA/BootstrapComposition.unity` (MonoBehaviour nur als Lifecycle-Adapter).

**Editor.Build.** `StpBuildEntrypoints` bietet `VerifyProjectVersion` (Preflight: Editorversion und ProjectVersion.txt exakt, QA-Szene vorhanden), `BuildAndroidDevelopmentIl2Cpp` (IL2CPP, ARM64, minSdk 26, targetSdk 36, Development) und `ExportIosXcodeProjectIl2Cpp` (IL2CPP, iOS 15.0) für die headless CI.

**CI.** `.github/workflows/unity.yml` enthält den immer laufenden, echt ausführbaren `project-preflight` (Unity-Versionspin, Pflichtartefakte, statischer Modulgraph-Check der realen `.asmdef`-Dateien gegen die normative Allowlist inklusive Azyklizität, noEngineReferences, Editor-Grenzen, csc.rsp-Konventionen und Guardrail-Tokens) sowie die Unity-Nachweisjobs (Compile/EditMode, PlayMode-Smoke, Android-Development-IL2CPP, iOS-Export/Compile auf `macos-15`). Diese Jobs laufen erst nach Konfiguration von `secrets.UNITY_LICENSE` und `vars.UNITY_RUNNERS_READY=true`; andernfalls schlägt der `unity-evidence-guard` fail-closed fehl und meldet jede Zeile wahrheitsgemäß als NOT_EXECUTED/BLOCKED. Kein Job täuscht einen erfolgreichen Unity-Nachweis vor.

**Governance.** `tools/architecture-validation/validate.py` und `tools/architecture-validation/README.md` wurden ausschließlich mechanisch nachgeführt (WP-008-Inventar einschließlich vierzehn `.asmdef`-Dateien, Paketlocks, Toolchain-Lock, Unity-Workflow; Schema-Beispiel; kanonischer WP-008-Befehl). Keine Regeländerung.

### Befund und Blocker B-01: Testassemblies-Standort (fehlende Architekturentscheidung, STOPP-Bedingung)

**Befund.** [`ARCHITECTURE/MODULE_BOUNDARIES.md`](../ARCHITECTURE/MODULE_BOUNDARIES.md) Abschnitt 2 sieht die Testassemblies unter dem repositorywurzeligen `Tests/` vor (EditMode/, PlayMode/, Device/, Fixtures/, Golden/). Die offizielle Unity-Dokumentation belegt, dass Unity ausschließlich Dateien unter `Assets/` sowie in Paketen importiert und kompiliert und dass Test-Assemblies des Unity Test Framework im Assets-Ordner oder in Paketen liegen müssen: [Introduction to importing assets](https://docs.unity3d.com/6000.3/Documentation/Manual/ImportingAssets.html), [Special folder names](https://docs.unity3d.com/6000.3/Documentation/Manual/SpecialFolders.html), [Create a test assembly](https://docs.unity3d.com/Packages/com.unity.test-framework@1.4/manual/workflow-create-test-assembly.html). Ein wurzeliger `Tests/`-Ordner wird folglich nicht importiert: keine `.meta`-Generierung, kein Compile, keine Testausführung. `BootstrapCompositionSmoke` und alle Testassemblies aus Abschnitt 4 wären an der dokumentierten Position nicht ausführbar.

**Getroffene Maßnahme.** Gemäß der in diesem Work Package festgelegten Stop-Regel wurde **keine eigene Abweichung** umgesetzt: keine Testassembly unter `Assets/` (Strukturabweichung) und keine inerte Testassembly unter `Tests/` (vorgetäuschte Ausführbarkeit). Jede Alternative — Testassemblies unter `Assets/StammstreckenPuzzle/Tests/`, ein lokales Test-Paket per `file:`-Referenz oder eine andere Konstruktion — verändert die dokumentierte Struktur und erfordert eine Architekturänderung, die außerhalb des byte-unveränderlichen WP-008-Scopes liegt.

**Exakt fehlende Entscheidung.** Der verbindliche physische Ort aller Testassemblies einschließlich `STP.Tests.Bootstrap.PlayMode` ist zu entscheiden und in `ARCHITECTURE/MODULE_BOUNDARIES.md` fortzuschreiben (gegebenenfalls per ADR). Kandidaten: (a) Testassemblies unter `Assets/StammstreckenPuzzle/Tests/` mit Anpassung von Abschnitt 2, (b) `Tests/` als lokales Unity-Paket (`package.json` plus `file:`-Referenz in `Packages/manifest.json`), (c) eine andere dokumentierte Entscheidung. Danach kann WP-008 die Testassemblies und den Smoke nachliefern.

**Blockiert durch B-01:** Testassemblies (AK-04 teilweise), `BootstrapCompositionSmoke` (AK-06), Coverage-/Mutation-Baseline und damit der WP-008-Abschluss. Der vorgesehene Smoke-Testumfang bleibt der aus MODULE_BOUNDARIES.md Abschnitt 7: Kompilieren der echten `.asmdef`-Kante, Start der Root in der dedizierten QA-Szene, genau ein Binding pro Application-Port (referenzgleich gegen die Adapterinstanzen aus `BootstrapCompositionResult`), vollständiger Application-/UI-/World-Graph, keine nicht dokumentierten Null-/Fallback-Ports (fail-closed-Eigenschaften von `ApplicationComposition`), kein optionaler Providerstart sowie Fehlschlag einer Doppelbindung über `BindProviders`.

### Blocker B-02: Unity-Lizenz und reproduzierbare Runner (bekanntes Abschlussrisiko)

Unity-Lizenz/Secret (`secrets.UNITY_LICENSE`) und reproduzierbare Unity-Runner für Linux/Android sowie macOS/iOS (`vars.UNITY_RUNNERS_READY`) sind weiterhin **nicht als verfügbar nachgewiesen**. Folglich NOT_EXECUTED/BLOCKED: Unity Compile, EditMode, `BootstrapCompositionSmoke`-Ausführung, Android-Development-IL2CPP und iOS-Export/Compile. Der `unity-evidence-guard` in `.github/workflows/unity.yml` hält diesen Zustand fail-closed rot. Ebenfalls davon abhängig und offen: Bestätigung des handabgeleiteten `packages-lock.json` durch den ersten realen Unity-Resolve (ein etwaiger normalisierender Diff wird als eigener Commit dokumentiert), Normalisierung der `ProjectSettings.asset` durch den ersten Editor-Open sowie die NOT_DETERMINED-Felder des `Toolchain.lock.md`. Der Geräte-/Device-Farm-Zustand ist unverändert: keine physischen Referenzgeräte und keine Device-Farm nachgewiesen; für WP-008 ist kein physischer Gerätesmoke erforderlich.

### Coverage-/Mutation-Baseline

**NOT_EXECUTED.** Ohne ausgeführten Unity-Testlauf existiert keine messbare Abdeckung; es werden keine Zahlen erfunden. Der tatsächlich entstandene Produktionscode besteht ausschließlich aus Composition-/Port-/Adapter-Skeletten ohne Fachlogik. Die erste Coverage-/Mutation-Baseline nach Assembly wird mit dem ersten ausgeführten EditMode-/PlayMode-Lauf gemäß [`ARCHITECTURE/TEST_STRATEGY.md`](../ARCHITECTURE/TEST_STRATEGY.md) Abschnitt 12 erhoben.

### Nachweismatrix (Gate B)

| Nachweis | Status |
|---|---|
| `git diff --check` gegen `3c1a6988c1aab2084763edf772b6adc268874865` | PASS |
| Statische Scope-Konformität aller 155 geänderten/neuen Dateien gegen `WP-008.production.scope.json` (Segmentglob-Spiegel in Node, kein Validatorlauf) | PASS |
| `.meta`-Vollständigkeit unter `Assets/` | PASS |
| Architecture Validator (`--self-test`) | NOT_EXECUTED lokal (kein Python auf dem Windows-Host); CI-gebunden auf dem Push-Commit |
| Production-Scope-Validator (`--scope production --self-test`) | NOT_EXECUTED lokal; CI-gebunden auf dem Push-Commit |
| Statischer Modulgraph-Check (`unity.yml` project-preflight) | CI-gebunden auf dem Push-Commit |
| Unity Compile | BLOCKED (B-02) |
| EditMode | BLOCKED (B-02); zusätzlich B-01 für die Testassemblies |
| Bootstrap PlayMode Smoke | BLOCKED (B-01 und B-02) |
| Android Development IL2CPP | BLOCKED (B-02) |
| iOS Export/Compile | BLOCKED (B-02) |
| Coverage-/Mutation-Baseline | NOT_EXECUTED (B-02) |
| Physischer Gerätesmoke | NOT_EXECUTED — für WP-008 nicht erforderlich; Geräte-/Device-Farm-Zustand wahrheitsgemäß dokumentiert |

### Nächste Schritte vor WP-008-Abschluss

1. **B-01 auflösen:** Entscheidung über den physischen Ort der Testassemblies außerhalb von WP-008 (Architekturklarstellung in `ARCHITECTURE/MODULE_BOUNDARIES.md`, gegebenenfalls ADR) und gegebenenfalls Scope-Fortschreibung; danach Testassemblies und `BootstrapCompositionSmoke` in WP-008 nachliefern.
2. **B-02 auflösen:** Owner konfiguriert `secrets.UNITY_LICENSE` und reproduzierbare Runner (`vars.UNITY_RUNNERS_READY=true`); danach laufen die Unity-Nachweisjobs von `.github/workflows/unity.yml` unverändert.
3. Anschließend: ersten realen Unity-Resolve des `packages-lock.json` bestätigen, Toolchain-Lock-NOT_DETERMINED-Felder aus dem Runner auslesen, erste Coverage-/Mutation-Baseline erheben, WP-008-Abschluss gemäß Akzeptanzkriterien und Definition of Done.

## Referenzen

[1]: ../AGENTS.md "Verbindliche Agentenleitlinie"
[2]: ../PROJECT_CONTROL/CURRENT_STATE.md "Aktueller Projektstand"
[3]: ../PROJECT_CONTROL/WORK_QUEUE.md "Produktionswarteschlange"
[4]: ../PROJECT_CONTROL/DEFINITION_OF_DONE.md "Definition of Done für technische Aufgaben"
[5]: ../PROJECT_CONTROL/AI_HANDOVER_RULES.md "Verbindliche Regeln für die KI-Übergabe"
[6]: ../ARCHITECTURE/ARCHITECTURE.md "Stammstrecken-Puzzle – Architecture v1.0"
[7]: ../ARCHITECTURE/MODULE_BOUNDARIES.md "Module Boundaries – normative Assembly-Allowlist"
[8]: ../ARCHITECTURE/TECH_STACK.md "Technology Stack – Unity-Baseline und Paketregeln"
[9]: ../ARCHITECTURE/BUILD_AND_RELEASE.md "Build-, Release- und CI-Vertrag"
[10]: ../ARCHITECTURE/TEST_STRATEGY.md "Teststrategie"
[11]: ../ARCHITECTURE/OPEN_BLOCKERS.md "Offene Produktfolgeblocker"
[12]: ../DECISIONS/ADR-001-unity-6-3-lts.md "ADR-001 – Unity 6.3 LTS als Game Engine"
[13]: ../DECISIONS/ADR-002-csharp-9-und-il2cpp.md "ADR-002 – C# 9 und IL2CPP"
[14]: ../DECISIONS/ADR-012-mobile-plattformbaselines.md "ADR-012 – Mobile Plattformbaselines"
[15]: ../DECISIONS/ADR-013-zyklusfreie-ports-und-modulgrenzen.md "ADR-013 – Zyklusfreie Ports und Modulgrenzen"
[16]: ../DECISIONS/ADR-018-bootstrap-composition-root.md "ADR-018 – Bootstrap Composition Root"
[17]: ../DECISIONS/ADR-026-validator-evidenz-und-scope-vertrauensanker.md "ADR-026 – Validator-Evidenz und Scope-Vertrauensanker"
[18]: ../DECISIONS/ADR-030-wp-scope-trust-anchor.md "ADR-030 – Gemeinsamer historischer WP-/Scope-Trust-Anchor"
[19]: ../WORK_PACKAGES/WP-007_CI-Setup.md "WP-007 – CI-Setup (Referenz für Trust-Anchor-/Scope-Mechanik)"
[20]: ../tools/architecture-validation/README.md "Architecture Validation – Scope-, Setup- und Evidenzvertrag"
[21]: ../tools/architecture-validation/scopes/WP-008.production.scope.json "WP-008-Production-Scope-Manifest (Trust-Anchor)"
