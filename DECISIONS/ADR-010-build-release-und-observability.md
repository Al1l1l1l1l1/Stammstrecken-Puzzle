# ADR-010 – GitHub Actions, signierte Store-Artefakte und datensparsame Observability

## Status

**Angenommen**

## Datum

2026-09-07

## Kontext

Reproduzierbare Android- und iOS-Releases müssen ohne lokale Einzelperson ausführbar, prüfbar und diagnostizierbar sein. Buildsignaturen, Storezugänge und personenbezogene Telemetrie benötigen klare Grenzen. Google Play verlangt seit 31. August 2026 für neue Apps und Updates mindestens Android 16 / API 36.[1] Apple verlangt seit 28. April 2026 Builds mit Xcode 26 oder neuer und dem iOS-26-SDK.[2]

## Entscheidung

**GitHub Actions** ist die Continuous-Integration- und Releaseorchestrierung. Unity wird direkt im Batchmode auf einer exakt gepinnten Editorversion ausgeführt; die Workflowdateien rufen versionierte Repository-Buildmethoden auf. Android-Tests und Builds laufen auf Linux, iOS-Export und Xcode-Archivierung auf macOS.

Vier Konfigurationen sind verbindlich: `dev`, `qa`, `staging` und `production`. Bundle-Identifier, App-IDs, Dienstkonfigurationen und Storetracks sind getrennt. Produktionssignaturen und Store-API-Schlüssel liegen ausschließlich als geschützte GitHub-Secrets beziehungsweise in der jeweiligen Signing-Infrastruktur. Sie werden nie in Repository, Buildlog oder Artefaktnamen geschrieben.

Pull Requests erzeugen Testberichte und einen nicht signierten Android-Development-Build. Ein signierter Kandidat entsteht nur aus einem annotierten Tag `vMAJOR.MINOR.PATCH-rc.N` auf einem geschützten Commit. Veröffentlichung in Google Play und App Store Connect erfolgt zunächst in interne Testkanäle. Die Promotion zu öffentlichen Tracks ist ein separater, nachvollziehbarer manueller Approval-Schritt.

Observability besteht aus strukturierten lokalen Logs, Firebase Crashlytics und freigegebenen Analytics-Events. Logs verwenden Eventcodes, Build-ID, Appversion und nicht personenbezogene Kontextfelder; sie enthalten niemals Savegame, Werbe-ID, Transaktionsbeleg, Freitext des Nutzers oder vollständige Levelpfade. Release-Symbole werden hochgeladen und als geschützte Artefakte aufbewahrt. Apple Privacy Manifest und Store-Datendeklarationen werden pro Release aus einem SDK-Inventar geprüft.[3]

## Begründung

GitHub Actions liegt neben Code, ADRs und Reviews und ist für wechselnde Agenten transparent. Getrennte Plattformrunner entsprechen den nativen Toolchains. Ein expliziter Approval-Schritt verhindert versehentliche öffentliche Releases. Strukturierte, datensparsame Diagnosen liefern verwertbare Fehlerbilder ohne Spielstände zu exfiltrieren.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Lokale manuelle Builds | Nicht reproduzierbar und von Einzelwissen abhängig. |
| Unity Build Automation als alleiniger Dienst | Geeignet, aber zusätzliche Dienstbindung und weniger unmittelbare Repository-Transparenz. |
| Fastlane als primäre Orchestrierung | Leistungsfähig, aber eine zusätzliche Abstraktionsschicht; native CLIs und APIs genügen zunächst. |
| Automatische öffentliche Veröffentlichung | Zu hohes Risiko für Store- und Produktfehler. |

## Konsequenzen

Ein macOS-Runner, Unity-Lizenzierung und geschützte Storecredentials werden vor iOS-Release benötigt. Plattformanforderungen sind bei jedem Release neu zu prüfen und nicht dauerhaft auf die heutigen Mindestwerte festzuschreiben. Telemetrieschema und SDK-Inventar werden versionierte Releaseartefakte.

## Betroffene Artefakte

`ARCHITECTURE/BUILD_AND_RELEASE.md`, `ARCHITECTURE/OBSERVABILITY.md`, `ARCHITECTURE/TEST_STRATEGY.md` und spätere `.github/workflows/`-Dateien.

## Validierung

Ein Dry Run erzeugt für denselben Commit reproduzierbare Buildmetadaten, Testberichte, SBOM/SDK-Inventar und Symbole. Store-Preflight prüft Target API, Xcode/SDK, Signatur, Privacy Manifest, Versionierung und getrennte App-IDs. Ein kontrollierter Testcrash muss symbolisiert und ohne verbotene Nutzdaten sichtbar sein.

## Ersetzt / ersetzt durch

Ersetzt keinen früheren ADR. Wird durch [ADR-023](./ADR-023-releasekandidat-und-kosmetikclaims.md) um production-identische Releasekandidaten und artefaktgleiche Promotion ergänzt; die Grundentscheidung bleibt angenommen.

## Referenzen

[1]: https://developer.android.com/google/play/requirements/target-sdk "Meet Google Play's target API level requirement"
[2]: https://developer.apple.com/news/upcoming-requirements/ "Apple Upcoming Requirements"
[3]: https://developer.apple.com/documentation/bundleresources/privacy-manifest-files "Privacy manifest files"
