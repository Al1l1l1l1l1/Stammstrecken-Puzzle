# Build and Release v0.2

## 1. Ziel

Jeder Storekandidat ist aus Commit, Tag, Toolchain, Contenthash und Konfiguration reproduzierbar. Kein Release hängt von einer lokalen Entwickleroberfläche oder Chatwissen ab. Öffentliche Promotion bleibt eine bewusste, separat bestätigte Handlung.

## 2. Buildprofile

| Profil | Zweck | App-/Bundle-ID | Dienste | Signierung/Verteilung |
|---|---|---|---|---|
| `dev` | lokale Entwicklung | `.dev` | Fakes oder offizielle Test-IDs; externe Telemetrie aus | lokal/Development |
| `qa` | automatisierte und manuelle QA | `.qa` | Sandbox, Testanzeigen, Store-Sandbox | internes Signing |
| `staging` | releaseähnlicher Kandidat | `.staging` | getrennte nicht öffentliche Providerprojekte | TestFlight / Play Internal |
| `production` | Storeprodukt | final | Productionkonfiguration nach Consent und Freigabe | App Store / Play Tracks |

Bundle-Identifier, AdMob-App-IDs, Firebasekonfigurationen und IAP-Produkt-IDs sind je Profil getrennt. Ein Productionbuild bricht ab, wenn Test-IDs, Debugdefines, Development Build, Autoconnect Profiler oder Testcrashcode erkannt werden. Ein Nicht-Productionbuild bricht ab, wenn Production-Ad-/Store-IDs erkannt werden.

## 3. Versionierung

Die Nutzerfassung folgt Semantic Versioning `MAJOR.MINOR.PATCH`. Pre-Releases verwenden `-rc.N`. Android `versionCode` und iOS `CFBundleVersion` sind streng steigende CI-generierte Ganzzahlen und werden als Releasebeleg gespeichert.

| Änderung | Versionsregel |
|---|---|
| kompatibler Bugfix/Contentkorrektur | PATCH |
| rückwärtskompatible Funktion/Seasoninhalt | MINOR |
| inkompatible Save-/Produktgrenze trotz Migration oder grundlegend neuer Rulesetvertrag | MAJOR |
| Storekandidat | `vX.Y.Z-rc.N` annotierter Tag |
| öffentlicher Release | `vX.Y.Z` annotierter Tag auf exakt geprüftem Commit |

Ein Releasebuild enthält `build-info.json` mit Version, Buildnummer, Commit, Tag, UTC-Buildzeit, Unityversion, Paketlockhash, Contenthash, Save-/Level-/Solverversion und Workflow-Run-ID. Keine Secrets.

## 4. Branch- und Freigabevertrag

`main` ist geschützt. Produktionsarbeit erfolgt auf Work-Package-Branches und gelangt nur über reviewte Pull Requests mit grünen Pflichtchecks in den Integrationsstand. Architecture v0.2 wird im Korrekturauftrag `WP-002` auf `arch/architecture-v0.1` dokumentiert und weder gemergt noch in einem Pull Request automatisch weiterverarbeitet.

Release-Tags zeigen auf unveränderte geprüfte Commits. Nach Tagging wird kein Artefakt lokal „repariert“. Eine Änderung erzeugt einen neuen Commit und neuen Kandidaten. Production-Environment und öffentliche Storepromotion verwenden GitHub-Environment-Protection und manuellen Approval.

## 5. GitHub-Actions-Pipelines

### 5.1 `validate.yml`

Läuft auf jedem Pull Request:

1. Pfad-/Scope- und Secretcheck;
2. den eingecheckten Befehl `python tools/architecture-validation/validate.py --self-test` einschließlich Markdownlinks, JSON Schema, Governance und Architekturcontracts;
3. Paket-/Lizenz-/SDK-Inventar;
4. Compile und EditMode;
5. Content-/Solverprüfung für betroffene Daten;
6. PlayMode;
7. Android-Development-IL2CPP-Build;
8. Testberichte und Artefaktmanifest.

### 5.2 `nightly.yml`

Läuft geplant auf aktuellem Integrationsstand:

- saubere Reimports und vollständiger Katalog;
- deterministischer Doppelbuild der generierten Contentartefakte;
- Android und iOS IL2CPP;
- Long-run und Performance sowie vorbereitende Emulator-/Simulator-Smokes;
- physische Gerätesmokes auf den inventarisierten Referenzgeräten oder einer ausdrücklich freigegebenen Device-Farm mit physischen Geräten;
- Dependency-, Lizenz-, Privacy-Manifest- und Größenberichte.

### 5.3 `release-candidate.yml`

Läuft nur auf `v*-rc.*`-Tag:

- alle Validierungs-/Nightlygates erneut ohne Wiederverwendung unbewiesener lokaler Outputs;
- signierte AAB-/IPA-Erzeugung in geschütztem Environment;
- Symbole, Mapping, SBOM/SDK-Inventar, Lizenzen und Checksummen;
- Upload zu Play Internal und TestFlight;
- installierter Smoke aus Storekanal;
- unveränderliches Release-Manifest.

### 5.4 `promote-release.yml`

Promotet exakt bereits geprüftes Artefakt. Es baut nicht neu. Erst nach manueller Freigabe darf stufenweiser öffentlicher Rollout starten. Ein Store ohne artefaktgenaue Promotion erhält denselben Hash/Build erneut über die API.

Alle verwendeten Actions werden per vollständigem Commit-SHA gepinnt. Workflowpermissions folgen Least Privilege; `GITHUB_TOKEN` ist standardmäßig read-only.

## 6. Runner und Toolchain

| Plattform | Runner | Toolchain |
|---|---|---|
| Editor/Android | Linux x64 mit Unity 6000.3.23f1, Android Build Support, gepinntem JDK/SDK/NDK | Container oder reproduzierbares Runnerimage; Unity-Lizenz secret. |
| iOS | macOS mit Unity 6000.3.23f1 und Xcode 26+ / iOS-26-SDK+ | Keychain temporär, CocoaPods/SPM-Locks eingecheckt. |

Die bei Release aktuelle Storeanforderung hat Vorrang vor diesen Mindestständen. Google Play verlangt zum Dokumentstand API 36.[1] App Store Connect verlangt Xcode 26 mit iOS-26-SDK oder neuer.[2] Ein automatisierter Preflight liest aktuelle, im Repository bewusst aktualisierte Baselines; er senkt sie nie automatisch.

## 7. Android-Build

- Ausgabe ausschließlich AAB für Store, APK nur für interne Gerätesmokes;
- IL2CPP ARM64; weitere ABIs nur nach bestätigter Reichweitenentscheidung;
- minSdk 26, target/compile mindestens 36 und aktuelles Mandat;
- Gradle, Android Gradle Plugin, JDK, SDK und NDK aus Unitytoolchain dokumentiert;
- Play App Signing verwenden; Upload-Key getrennt geschützt;
- R8/Minify nur mit SDK-/Reflection-/IAP-/Ads-Smokes und Mappingarchiv;
- Berechtigungsdiff als Gate; neue gefährliche Berechtigung blockiert bis Begründung und Storedeklaration;
- Data-Safety-Antworten gegen SDK-Inventar und Telemetrieschema prüfen;
- IL2CPP- und native Crashsymbole zu Crashlytics hochladen und archivieren.

Die App benötigt für den lokalen Rätselkern keine gefährliche Androidberechtigung. SDK-manifeste werden nach Merge geprüft; unerwartete Permissions sind Buildfehler.

## 8. iOS-Build

- Unity exportiert ein Xcodeprojekt; `xcodebuild archive` und Export laufen headless;
- IL2CPP ARM64, min iOS 15.0, aktuelles vorgeschriebenes SDK;
- automatische oder manuelle Signierung ist CI-weit einheitlich und dokumentiert;
- Distributionzertifikate und App-Store-Connect-API-Key nur im geschützten Environment;
- Entitlements-Allowlist; neue Capability blockiert;
- `PrivacyInfo.xcprivacy` für App und SDKaggregation prüfen;
- Required Reason APIs, Trackingdomains und gesammelte Datentypen mit tatsächlichem SDK-Inventar abgleichen.[3]
- dSYM/BCSymbolMaps soweit relevant archivieren und Crashlytics-Symbolstatus prüfen;
- expliziter Restore-Purchases-Einstieg im UI-Smoke.

Ein iOS-Simulator erfüllt weder diesen Gerätesmoke noch den Privacy-Capture-Vertrag. Er darf nur vorbereitende UI-/Buildsignale liefern.

## 9. Secrets und Signing

Keystore, Passwörter, Applezertifikate, Provisioning, API-Schlüssel und Unity-Lizenz liegen ausschließlich in geschützten Secretstores. CI schreibt sie in temporäre Dateien mit restriktiven Rechten und entfernt sie in `always()`-Cleanup. Logs maskieren bekannte Werte; ein nachgelagerter Secret-Scan prüft Artefakte.

Pull-Request-Workflows aus nicht vertrauenswürdigen Branches erhalten keine Signing-/Productionsecrets. Production-Environment ist nur aus Tagworkflow und nach Approval erreichbar. Schlüsselrotation wird halbjährlich getestet und bei Personal-/Zugriffswechsel sofort ausgeführt.

## 10. Releaseartefakte und Provenienz

Jeder Kandidat bewahrt mindestens:

- signiertes AAB/IPA beziehungsweise Storebuildreferenz;
- SHA-256 der verteilten Artefakte;
- `build-info.json` und Release-Manifest;
- Testberichte und vollständige Gateübersicht;
- Content-/Addressables-/Localization-Manifeste;
- SDK-/Lizenzinventar und Software Bill of Materials;
- Android Mapping/IL2CPP-Symbole und iOS dSYMs;
- Privacy Manifest, Permission-/Entitlementdiff und Store-Preflight;
- bekannte Einschränkungen und Freigabeentscheidung.

Artefakte erhalten eine dokumentierte Aufbewahrungsfrist. Symbole und Release-Manifeste werden mindestens so lange wie die unterstützte Appversion aufbewahrt.

## 11. Release-Gates

| Gate | Verantwortungsnachweis |
|---|---|
| Produkt | bestätigter Contentumfang, Texte, Zeitwerte, Werbefrei-Preis und UX-Details. |
| Recht | Marke/Icon/Screens, Datenschutz, Consenttexte, Anbieter und Storedeklarationen geprüft. |
| Technik | alle Tests, Performance, Save-Migration, Contentsolver, Geräte und Symbolik grün. |
| Store | Bundle-/Produkt-IDs, Verträge, Altersfreigabe, Data Safety/App Privacy und Metadaten vollständig. |
| Betrieb | Dashboards, Alarmwege, Rollout-/Stopkriterien, Support-/Recoverytext vorhanden. |
| Security | Secrets, Permissions, SDK-Lizenzen/Signaturen und Dependencyrisiken geprüft. |

Architecture v0.2 erfüllt diese späteren Release-Gates nicht selbst. Sie definiert sie. Vorhandene offene Produktpunkte sind daher keine verdeckt als erledigt dargestellten Werte.

## 12. Rollout und Rollback

Öffentliche Releases starten gestuft. Vor jeder Erhöhung werden Crash-free Sessions, Save-Recovery, Reward-/Purchasefehler, Start-/Completionfunnel und Storefeedback geprüft. Harte Stoppsignale sind Saveverlust, falsche Währung/Entitlements, mehrdeutiger Content, nicht symbolisierte Crashspitze oder Consent-/Datenschutzverletzung.

Mobile Stores erlauben kein echtes Binärrollback für bereits installierte Versionen. Reaktion:

1. Rollout sofort pausieren;
2. fehlerhaften Track nicht weiter promoten;
3. falls möglich alten geprüften Storestand für neue Installationen beibehalten;
4. Hotfix aus neuem Commit und neuem Patchtag bauen;
5. Save-/Contentmigration vorwärtskompatibel halten;
6. niemals Fortschritt durch Herunterstufen des Saveformats opfern.

Remote Config darf keinen Code-, Save-, Ruleset- oder Monetarisierungsrollback simulieren.

## 13. Store- und Provider-Voraussetzungen

Vor Sandbox-/Releaseintegration werden benötigt: Apple Developer/App Store Connect, Google Play Console, Unity-Lizenz, AdMob-/Mediationkonto, Firebaseprojekte je Environment, IAP-Produkte, Signingmaterial und rechtlich freigegebene Datenschutz-/Consentangaben. Fehlende Zugänge blockieren das jeweilige Implementierungs- oder Release-Work-Package, nicht die vorliegende Architekturspezifikation.

## 14. Releasecheckliste

Ein Kandidat ist promotable, wenn:

1. Tag und Commit geschützt und unverändert sind;
2. Toolchain/Packages exakt dem Lock entsprechen;
3. vollständiger Levelkatalog schema-, semantik- und solvergrün ist;
4. Saves einschließlich Upgrade vom ältesten unterstützten Stand funktionieren;
5. Android/iOS aus internen Storekanälen auf den vorgeschriebenen **physischen** Referenzgeräten oder einer freigegebenen physischen Device-Farm installiert und kerngetestet wurden; Emulator-/Simulatorergebnisse allein gelten nicht;
6. Ads/Reward/IAP/Restore/Consent mit Sandboxfällen bestanden sind;
7. Privacy-/Permission-/Entitlement-/SDK-Diffs freigegeben sind und der Fresh-Install-Netzwerknachweis vor Consent auf beiden physischen Plattformgeräten keinen unerlaubten Traffic zeigt;
8. Symbole erfolgreich hochgeladen und Testcrash symbolisiert sind;
9. SBOM, Lizenzen und Artefakthashes archiviert sind;
10. öffentlicher Rollout separat genehmigt ist.

## Referenzen

[1]: https://developer.android.com/google/play/requirements/target-sdk "Meet Google Play's target API level requirement"
[2]: https://developer.apple.com/news/upcoming-requirements/ "Apple Upcoming Requirements"
[3]: https://developer.apple.com/documentation/bundleresources/privacy-manifest-files "Privacy manifest files"
[4]: ../DECISIONS/ADR-010-build-release-und-observability.md "ADR-010 – GitHub Actions, Store-Artefakte und Observability"
[5]: ../DECISIONS/ADR-012-mobile-plattformbaselines.md "ADR-012 – Mobile Plattformbaselines für Android und iOS"
