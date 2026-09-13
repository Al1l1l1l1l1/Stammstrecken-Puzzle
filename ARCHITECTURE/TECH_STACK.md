# Technology Stack v0.4

## 1. Verbindliche Baseline

| Bereich | Festlegung | Pinning |
|---|---|---|
| Game Engine | **Unity 6.3 LTS** | **6000.3.23f1** als initiale exakte Baseline; `ProjectVersion.txt` ist autoritativ. |
| Sprache | **C# 9** mit Nullable Reference Types | Keine erzwungene neuere Sprachversion. |
| Release-Runtime | **IL2CPP** | Android ARM64 und iOS ARM64. |
| Rendering | Universal Render Pipeline, 2D-/2.5D-fokussiert | Mit Editor-/Package-Lock. |
| Runtime-UI | UI Toolkit | Kein paralleles uGUI-System ohne neues ADR. |
| Input | Unity Input System | `com.unity.inputsystem` **1.20.0**. |
| Assets | Unity Addressables | `com.unity.addressables` **2.10.3**, Launchkatalog lokal. |
| Lokalisierung | Unity Localization | `com.unity.localization` **1.5.13**. |
| JSON | Unity Newtonsoft Json | `com.unity.nuget.newtonsoft-json` **3.2.2**. |
| In-App-Kauf | Unity IAP | `com.unity.purchasing` **5.4.3**. |
| Werbung/Consent | Google Mobile Ads Unity Plugin inklusive UMP | **11.5.0**. |
| Analytics | Firebase Unity SDK | **13.16.0** als geprüfte Integrationslinie; bis zum plattformbezogenen prä-SDK-Fence-Nachweis im Productionprofil nicht importieren/aktivieren. |
| Crashdiagnose | lokaler redigierter Diagnosering plus Plattformlogs | Firebase Crashlytics 13.16.0 ist in Production ausgeschlossen; erneute Aufnahme nur per neuem ADR. |
| Tests | Unity Test Framework und NUnit | Mit Unity 6000.3 fest gekoppelte Core-Paketversion. |
| CI/CD | GitHub Actions und native Store-CLIs/APIs | Actions ausschließlich per unveränderlichem Commit-SHA pinnen. |

Unity 6.3 LTS wird bis Dezember 2027 regulär unterstützt.[1] Die konkrete Patchbaseline `6000.3.23f1` war zum Architekturstand der jüngste veröffentlichte Patch dieser LTS-Linie. Eine spätere `6000.3.xf1`-Aktualisierung erfolgt ausschließlich über einen isolierten, vollständig getesteten Dependency-Pull-Request.

## 2. Plattformbaselines

| Ziel | Mindestbetriebssystem | Buildziel | Ausgabe |
|---|---:|---:|---|
| Android | Android 8.0 / API 26 | `targetSdkVersion` und `compileSdkVersion` mindestens API 36 und bei Release mindestens aktuelles Google-Play-Mandat | Android App Bundle (`.aab`), ARM64, IL2CPP |
| iOS | iOS 15.0 | Xcode 26 oder neuer mit iOS-26-SDK oder aktuellerem App-Store-Mandat | signiertes `.ipa`, ARM64, IL2CPP |

Die Mindestbetriebssysteme begrenzen den initialen Testvertrag. Eine Absenkung oder Anhebung ist eine bewusste Reichweitenentscheidung und benötigt einen neuen ADR mit Geräte-/Marktdaten. Google Play verlangt seit 31. August 2026 API 36 für neue Apps und Updates.[2] Apple verlangt seit 28. April 2026 Xcode 26 und iOS-26-SDK für Uploads.[3]

Begründung, Alternativen, Referenzgeräte und die verpflichtende Reichweitenprüfung stehen in ADR-012.[6] Die Mindestwerte werden nicht mit den jeweils aktuellen Store-Uploadzielen verwechselt.

Die Benutzeroberfläche wird für Safe Areas, dynamische Auflösung und mindestens die Seitenverhältnisse 16:9 bis 22:9 ausgelegt. Die finale Orientierungsentscheidung ist ein offenes Produktdetail; die Architektur hält Präsentationslogik von Bildschirmgeometrie getrennt.

## 3. Paketregeln

1. `Packages/manifest.json`, `Packages/packages-lock.json`, `ProjectVersion.txt` und native Dependency-Locks werden eingecheckt.
2. Versionsbereiche, Floating Tags, Git-Branches und `latest` sind verboten.
3. GitHub Actions werden auf vollständige Commit-SHAs gepinnt. Der lesbare Release-Tag steht nur als Kommentar daneben.
4. Ein Paket wird nur eingeführt, wenn eine konkrete Fähigkeit nicht mit Standardbibliothek oder vorhandenem Paket angemessen lösbar ist.
5. Jedes Drittanbieterpaket benötigt Eigentümer, Lizenz, Datenfluss, Plattformen, aktuelle Version, Updatequelle und Entfernungsplan im SDK-Inventar.
6. Pre-release-, experimentelle und veraltete Pakete sind in Production verboten.
7. SDKs werden nur in der Assembly ihres Adapters referenziert.
8. Dependency-Updates ändern nie gleichzeitig Produktverhalten oder fachliche Logik.
9. Ein Update muss Changelog, Datenschutzdeklarationen, IL2CPP/AOT, App-Größe, Build und Gerätetests prüfen.
10. Sicherheits- oder Store-Kompatibilitätsupdates dürfen beschleunigt werden, umgehen aber keine Gates.
11. Unity Analytics, Unity Cloud Diagnostics und nicht benötigte Unity-Gaming-Services-Pakete sind in Production nicht eingebunden. Google Mobile Ads muss nativ default-off und UMP-gesteuert sein. Firebase Crashlytics ist im Productionprofil nicht importiert. Firebase Analytics bleibt ebenfalls ausgeschlossen, bis ADR-024 durch einen physischen plattformbezogenen prä-SDK-Fence-Nachweis erfüllt ist; bloße Application-No-ops genügen nie.

## 4. Unity-Projektkonfiguration

| Einstellung | Vorgabe |
|---|---|
| Asset Serialization | Force Text |
| Version Control Mode | Visible Meta Files |
| Scripting Backend | IL2CPP für alle Releaseprofile |
| API Compatibility | Höchste von Unity 6.3 offiziell unterstützte .NET-Standard-Basis; keine benutzerdefinierte Runtime. |
| Managed Stripping | Medium als Startwert; High erst nach Linker- und SDK-Tests. |
| Incremental GC | Aktiviert, sofern Profiling keinen Plattformfehler zeigt. |
| Color Space | Linear für 2.5D-Präsentation; UI-Kontrast geräteprüfen. |
| Graphics APIs Android | Vulkan primär, OpenGLES3 als Fallback; keine GLES2-Unterstützung. |
| Graphics APIs iOS | Metal. |
| Compression | Android ASTC mit ETC2-Fallback nur bei nachgewiesenem Gerätebedarf; iOS ASTC. |
| Orientation | Noch nicht produktseitig festgelegt; UI muss ohne Domainänderung konfigurierbar bleiben. |
| Domain Reload Optimierung | Darf lokale Iteration verbessern, ist aber in CI deaktiviert beziehungsweise reproduzierbar konfiguriert. |

## 5. Bewusst nicht verwendete Technologien

| Technologie | Grund |
|---|---|
| DOTS/ECS | Kein nachgewiesener Bedarf für ein 10×10-Puzzle; erhöht Abstraktion. |
| Unity Physics für Puzzlelogik | Gleisregeln sind diskret und deterministisch, nicht physikalisch. |
| Dependency-Injection-Container | Manuelle Konstruktorinjektion ist transparenter und AOT-sicherer. |
| Remote Config für Produktregeln | Bestätigte Regeln dürfen nicht serverseitig still verändert werden. |
| Cloud Save zum Launch | Konto-, Konflikt- und Datenschutzproduktentscheidung fehlt; lokaler Save ist ausreichend. |
| Remote Addressables zum Launch | Season-1-Inhalte sind lokal und offline verfügbar. |
| FMOD/Wwise | Der bestätigte Audioumfang rechtfertigt keine weitere Runtime und Lizenz. |
| Visual Scripting | Schlecht diffbar und für autonome Agenten weniger maschinell prüfbar. |

## 6. Toolchain-Lock und Upgradeprozess

`Toolchain.lock.md` wird mit dem ersten Produktions-Scaffold erzeugt und enthält Editorhash, Unity-Module, JDK, Android SDK/NDK, Gradle, Xcode, CocoaPods, Ruby/CLI-Versionen und Store-CLI-Versionen. Der Inhalt ist aus dem Build auslesbar und wird als Releaseartefakt gespeichert.

Ein Upgrade-Pull-Request muss in dieser Reihenfolge arbeiten:

1. Ausgangsbuild und Tests auf alter Version dokumentieren.
2. Genau eine Dependency-Gruppe ändern.
3. Lockfiles und SDK-Inventar aktualisieren.
4. Lizenz-, Privacy-Manifest- und Datenflussdiff prüfen.
5. EditMode-, PlayMode- und Contenttests ausführen.
6. Android- und iOS-IL2CPP-Build erzeugen.
7. Kernflow auf Geräten und SDK-Sandbox prüfen.
8. Buildgröße, Startzeit und Crashsymbolik vergleichen.
9. ADR nur dann ersetzen, wenn Release-Linie, Anbieter oder Systemgrenze wechselt.

Jeder SDK-Updatevergleich umfasst außerdem native Android-Manifest-/iOS-`Info.plist`-Defaults, Providerdashboard-Einstellungen und physische Netzwerkbelege für Fresh Install, direkten Sprung von jedem noch unterstützten Legacy-Build mit früher aktivem Override, installierten aber nie gestarteten Zwischenbuild, Widerruf mit Crashinjektion und Re-enable. Kann der prä-SDK-Deny-Fence, neue optionale Übertragung oder crashsicherer Widerruf nicht belegt werden, bleibt Analytics ausgeschlossen beziehungsweise das Update blockiert.

## 7. Konfigurations- und Secretvertrag

Öffentliche, nicht geheime Dienst-IDs liegen getrennt je Buildprofil in typisierten Konfigurationsassets. Geheimnisse existieren nur in CI-Secrets oder Storeinfrastruktur. Kein Secret wird in `StreamingAssets`, Addressables, PlayerPrefs, Logs oder Crashkeys abgelegt. Buildskripte brechen bei fehlenden Production-Werten ab; `dev` und `qa` verwenden ausschließlich Sandbox-/Test-IDs.

## Referenzen

[1]: https://unity.com/releases/unity-6/support "Unity 6 release support"
[2]: https://developer.android.com/google/play/requirements/target-sdk "Meet Google Play's target API level requirement"
[3]: https://developer.apple.com/news/upcoming-requirements/ "Apple Upcoming Requirements"
[4]: ../DECISIONS/ADR-001-unity-6-3-lts.md "ADR-001 – Unity 6.3 LTS als Game Engine"
[5]: ../DECISIONS/ADR-011-ui-assets-lokalisierung-und-audio.md "ADR-011 – UI Toolkit, lokale Addressables, Unity Localization und Unity Audio"
[6]: ../DECISIONS/ADR-012-mobile-plattformbaselines.md "ADR-012 – Mobile Plattformbaselines für Android und iOS"
[7]: ./PRIVACY_PROVIDER_EVIDENCE.md "Privacy Provider Evidence für Architecture v0.4"
[8]: ../DECISIONS/ADR-024-privacy-bootstrap-fence-und-widerruf.md "ADR-024 – Privacy-Bootstrap-Fence und Widerruf"
