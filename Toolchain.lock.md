# Toolchain Lock (WP-021, Unity-Scaffold-Recovery)

Dieser Lock folgt `ARCHITECTURE/TECH_STACK.md` Abschnitt 6. Er enthält die für WP-021
reproduzierbar ableitbaren Werte. Felder, die erst aus einem realen Build auf einem
reproduzierbaren Runner ausgelesen werden können, sind wahrheitsgemäß als
**NOT_DETERMINED** markiert; sie werden ausschließlich aus WP-021-eigenen CI-Läufen
mit Commit-Bezug ergänzt und dürfen nicht geraten werden.

## Editor

| Komponente | Wert | Quelle |
|---|---|---|
| Unity Editor | `6000.3.23f1` | ADR-001 / `ProjectSettings/ProjectVersion.txt` (autoritativ) |
| Editor-Changeset | `09d2ecc7fb28` | `m_EditorVersionWithRevision` in `ProjectSettings/ProjectVersion.txt` |
| Unity-Installationsmodule (Android Build Support, iOS Build Support, IL2CPP) | Android Build Support + iOS Build Support auf den WP-021-Runnern; IL2CPP im Editor enthalten (`Data/il2cpp`, CI-Umgebungsjob belegt) | WP-021 CI-Run `37069959561` |

## Sprache und Runtime

| Komponente | Wert | Quelle |
|---|---|---|
| C#-Sprachversion | 9 (Unity-6000.3-Standard, kein Override) | ADR-002 |
| Nullable Reference Types | aktiviert (`-nullable:enable` je `csc.rsp` in jeder Assembly) | ADR-002 |
| Eigene Warnungen als Fehler | aktiviert (`-warnaserror+` je `csc.rsp`) | ADR-002 |
| Release-Scripting-Backend | IL2CPP (per Build-Entrypoint je Plattform gesetzt) | ADR-002 |
| API Compatibility | `.NET Standard 2.1` (`apiCompatibilityLevel: 3`) | `ARCHITECTURE/TECH_STACK.md` Abschnitt 4 |
| Managed Stripping | Explizit **Medium** für Android und iOS; Preflight und beide Buildentrypoints prüfen fail-closed | `ARCHITECTURE/TECH_STACK.md` Abschnitt 4; `ProjectSettings/ProjectSettings.asset`, `StpBuildEntrypoints` |
| Incremental GC | aktiviert (`gcIncremental: 1`) | `ARCHITECTURE/TECH_STACK.md` Abschnitt 4 |

## Eingecheckte Paketpins (`Packages/manifest.json`)

| Paket | Version | Quelle |
|---|---|---|
| `com.unity.inputsystem` | `1.20.0` | `ARCHITECTURE/TECH_STACK.md` Abschnitt 1 |
| `com.unity.addressables` | `2.10.3` | `ARCHITECTURE/TECH_STACK.md` Abschnitt 1 |
| `com.unity.localization` | `1.5.13` | `ARCHITECTURE/TECH_STACK.md` Abschnitt 1 |
| `com.unity.nuget.newtonsoft-json` | `3.2.2` | `ARCHITECTURE/TECH_STACK.md` Abschnitt 1 |
| `com.unity.render-pipelines.universal` | `17.3.0` | Unity-6000.3-Core-Paketlinie, aus der gepinnten Editorinstallation ausgelesen (`BuiltInPackages`) |
| `com.unity.test-framework` | `1.6.0` | Unity-6000.3-Core-Paketlinie (`BuiltInPackages`) |
| `com.unity.testtools.codecoverage` | `1.3.0` | Unity-Registry (Mindest-Unity 2021.3); offizielles Unity-Testwerkzeug für die von `ARCHITECTURE/TEST_STRATEGY.md` Abschnitt 12 verlangte Coverage-Baseline |

`Packages/packages-lock.json` ist aus den Registry-Metadaten (`packages.unity.com`)
und der `BuiltInPackages`-Menge der gepinnten Editorversion abgeleitet und eingecheckt.
Der erste reale Unity-Resolve in der WP-021-CI bestätigt den Lock; ein dabei
entstehender normalisierender Diff wird als eigener Commit dokumentiert.

Bewusst **nicht** eingebunden (Architekturverbot beziehungsweise spätere Gates):
`com.unity.purchasing` (IAP folgt mit dem zuständigen späteren Work Package), Google
Mobile Ads / Firebase Analytics / Firebase Crashlytics (kein SDK-Import vor den
Privacy-Gates; `ARCHITECTURE/TECH_STACK.md` Abschnitt 3 Regel 11), Unity Gaming
Services / Unity Analytics (Architekturverbot), Visual Scripting (Architekturverbot).

## Plattform-Toolchains

| Komponente | Wert | Quelle |
|---|---|---|
| Android minSdk / targetSdk / compileSdk | 26 / 36 / 36 | ADR-012; Google-Play-Mandat API 36 seit 2026-08-31 |
| Android ABI | ARM64 | ADR-012 |
| JDK (Unity-gebündelt) | **NOT_DETERMINED** | In den WP-021-CI-Logs nicht ausgegeben; Auslesung mit einem späteren Lauf nachholen |
| Android SDK Plattformen (Unity-gebündelt) | `android-34`, `android-35`, `android-36`, `android-37.0`; Build-Tools `36.0.0` | WP-021 CI-Run `37069959561` (unity-environment-android, `SDK/platforms`, `SDK/build-tools`) |
| Android NDK (Unity-gebündelt) | `27.2.12479018` (r27c) | WP-021 CI-Run `37069959561` (`NDK/source.properties`) |
| Gradle (Unity-gebündelt) | `9.1.0` (Android Plugin `9.0.0`) | WP-021 CI-Run `37069959561` (unity-android-development-il2cpp, Buildlog) |
| Xcode | `26.3` mit `iphoneos26.2`-SDK | WP-021 CI-Run `37069959561` (unity-environment-ios, `xcodebuild -version`, `xcodebuild -showsdks`) |
| iOS Deployment Target | 15.0 | ADR-012 |
| CocoaPods / Ruby / Fastlane / Store-CLIs | **NOT_DETERMINED** | Für WP-021 nicht erforderlich und nicht aus dem Runner ausgelesen |

## CI-Laufzeitumgebungen

| Umgebung | Wert | Quelle |
|---|---|---|
| Architecture Validation (`validate.yml`) | `ubuntu-24.04`, CPython 3.11.13, Node.js 22.20.0, SHA-gepinnte Actions, `contents: read` | WP-007 |
| Unity CI (`unity.yml`) | Preflight: `ubuntu-24.04`, CPython 3.11.13; Unity-Jobs: `ubuntu-24.04` mit per Tag und Digest gepinnten `unityci/editor`-Containern `6000.3.23f1` (base/android) und `macos-15` mit Changeset-gepinntem Unity-Hub-Install (`09d2ecc7fb28`) und Xcode 26+; Aktivierung der Unity-Personal-Lizenz per Unity Licensing Client mit `secrets.UNITY_EMAIL`/`secrets.UNITY_PASSWORD`; Lizenzjobs wegen der Ein-Instanz-Bedingung des Personal-Seats serialisiert | WP-021 |

## Reproduzierbarkeitsregeln

1. `ProjectSettings/ProjectVersion.txt` ist die autoritative Editor-Versionsquelle;
   CI-Runnerimage und Buildmetadaten müssen denselben Wert tragen (ADR-001).
2. Versionsbereiche, Floating Tags, Git-Branches und `latest` sind verboten
   (TECH_STACK.md Abschnitt 3).
3. Ein Patchwechsel innerhalb `6000.3` erfolgt nur in einem eigenen Pull Request,
   der diesen Lock aktualisiert (ADR-001).
