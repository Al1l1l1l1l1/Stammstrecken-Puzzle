# Toolchain Lock (WP-008, erster Produktions-Scaffold)

Dieser Lock folgt `ARCHITECTURE/TECH_STACK.md` Abschnitt 6. Er enthält die für WP-008
reproduzierbar ableitbaren Werte. Felder, die erst aus einem realen Build auf einem
reproduzierbaren Runner ausgelesen werden können, sind wahrheitsgemäß als
**NOT_DETERMINED** markiert; sie werden mit dem ersten verfügbaren Unity-Runner
ergänzt und dürfen nicht geraten werden. Vollständig aus dem Build auslesbar ist
dieser Lock damit erst nach Verfügbarkeit von Unity-Lizenz und Runnern (siehe
WP-008-Abschlussrisiken).

## Editor

| Komponente | Wert | Quelle |
|---|---|---|
| Unity Editor | `6000.3.23f1` | ADR-001 / `ProjectSettings/ProjectVersion.txt` (autoritativ) |
| Editor-Changeset | `09d2ecc7fb28` | Unity Release Notes 6000.3.23f1 (Download-URLs sind changeset-adressiert) |
| `m_EditorVersionWithRevision` | `6000.3.23f1 (09d2ecc7fb28)` | `ProjectSettings/ProjectVersion.txt` |
| Unity-Installationsmodule (Android Build Support, iOS Build Support, IL2CPP) | **NOT_DETERMINED** | Erst aus Runner-Installation auslesbar |

## Sprache und Runtime

| Komponente | Wert | Quelle |
|---|---|---|
| C#-Sprachversion | 9 (Unity-6000.3-Standard, kein Override) | ADR-002 |
| Nullable Reference Types | aktiviert (`-nullable:enable` je `csc.rsp` in jeder Assembly) | ADR-002 |
| Eigene Warnungen als Fehler | aktiviert (`-warnaserror+` je `csc.rsp`) | ADR-002 |
| Release-Scripting-Backend | IL2CPP (per Build-Entrypoint je Plattform gesetzt) | ADR-002 |
| API Compatibility | `.NET Standard 2.1` (`apiCompatibilityLevel: 3`) | `ARCHITECTURE/TECH_STACK.md` Abschnitt 4 |
| Managed Stripping | Unity-Plattformdefault (Medium als Startwert gemäß TECH_STACK; explizite Setzung erfolgt mit dem ersten realen Buildprofil) | `ARCHITECTURE/TECH_STACK.md` Abschnitt 4 |
| Incremental GC | aktiviert (`gcIncremental: 1`) | `ARCHITECTURE/TECH_STACK.md` Abschnitt 4 |

## Eingecheckte Paketpins (`Packages/manifest.json`)

| Paket | Version | Quelle |
|---|---|---|
| `com.unity.inputsystem` | `1.20.0` | `ARCHITECTURE/TECH_STACK.md` Abschnitt 1 |
| `com.unity.addressables` | `2.10.3` | `ARCHITECTURE/TECH_STACK.md` Abschnitt 1 |
| `com.unity.localization` | `1.5.13` | `ARCHITECTURE/TECH_STACK.md` Abschnitt 1 |
| `com.unity.nuget.newtonsoft-json` | `3.2.2` | `ARCHITECTURE/TECH_STACK.md` Abschnitt 1 |
| `com.unity.render-pipelines.universal` | `17.3.0` | Unity-6000.3-Core-Package-Linie (URP-Changelog 17.3; Real-Projekt-Anker 6000.3.12/13f1) |
| `com.unity.test-framework` | `1.6.0` | Unity-6000.3-Core-Package-Linie (TF-1.6-Changelog) |

`Packages/packages-lock.json` ist aus den Registry-Metadaten (packages.unity.com)
und der 6000.3-Core-Package-Linie abgeleitet und eingecheckt. Der erste reale
Unity-Resolve auf einem Runner bestätigt den Lock; ein dabei entstehender
normalisierender Diff wird als eigener Commit dokumentiert.

Bewusst **nicht** eingebunden (WP-008-Scope-OUT beziehungsweise Architekturverbot):
`com.unity.purchasing` (IAP ist WP-008-Scope-OUT; die gepinnte Linie 5.4.3 folgt mit
dem zuständigen späteren Work Package), Google Mobile Ads / Firebase Analytics
(kein SDK-Import vor den Privacy-Gates; Firebase Crashlytics ist im
Productionprofil ausgeschlossen), Unity Gaming Services / Unity Analytics
(Architekturverbot), Visual Scripting (Architekturverbot).

Hinweis zur Addressables-Linie: Die Unity-Doku führt für 6000.3 die
„verified"-Linie addressables@4.0; die Architektur pinnt verbindlich 2.10.3
(TECH_STACK.md, ADR-011). 2.10.3 ist für Unity 6000.x freigegeben
(Paket-Mindesteditor 6000.0). Ein Linienwechsel erfolgt nur über einen isolierten
Dependency-Pull-Request gemäß TECH_STACK.md Abschnitt 6.

## Plattform-Toolchains

| Komponente | Wert | Quelle |
|---|---|---|
| Android minSdk / targetSdk / compileSdk | 26 / 36 / 36 | ADR-012; Google-Play-Mandat API 36 seit 2026-08-31 |
| Android ABI | ARM64 | ADR-012 |
| JDK (Unity-gebündelt) | **NOT_DETERMINED** | Erst aus Runner-Installation auslesbar |
| Android SDK / NDK (Unity-gebündelt) | **NOT_DETERMINED** | Erst aus Runner-Installation auslesbar |
| Gradle (Unity-gebündelt) | **NOT_DETERMINED** | Erst aus Runner-Installation auslesbar |
| Xcode | 26 oder neuer mit iOS-26-SDK+ | ADR-012; App-Store-Mandat seit 2026-04-28 |
| iOS Deployment Target | 15.0 | ADR-012 |
| CocoaPods / Ruby / Fastlane / Store-CLIs | **NOT_DETERMINED** | Erst aus macOS-Runner auslesbar |

## CI-Laufzeitumgebungen

| Umgebung | Wert | Quelle |
|---|---|---|
| Architecture Validation (`validate.yml`) | `ubuntu-24.04`, CPython 3.11.13, Node.js 22.20.0, SHA-gepinnte Actions, `contents: read` | WP-007 |
| Unity CI (`unity.yml`) | Preflight: `ubuntu-24.04`, CPython 3.11.13; Unity-Jobs: `ubuntu-24.04` (Linux/Android) und `macos-15` (iOS) mit Unity `6000.3.23f1` — **aktiv erst nach Konfiguration von `secrets.UNITY_LICENSE` und `vars.UNITY_RUNNERS_READY=true`** | WP-008 |

## Reproduzierbarkeitsregeln

1. `ProjectSettings/ProjectVersion.txt` ist die autoritative Editor-Versionsquelle;
   CI-Runnerimage und Buildmetadaten müssen denselben Wert tragen (ADR-001).
2. Versionsbereiche, Floating Tags, Git-Branches und `latest` sind verboten
   (TECH_STACK.md Abschnitt 3).
3. Ein Patchwechsel innerhalb `6000.3` erfolgt nur in einem eigenen Pull Request,
   der diesen Lock aktualisiert (ADR-001).
