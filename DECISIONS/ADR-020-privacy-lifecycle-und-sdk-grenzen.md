# ADR-020 – Privacy-Lifecycle und SDK-Grenzen

## Status

**Angenommen**

## Datum

2026-09-12

## Kontext

Firebase Analytics und Crashlytics sind standardmäßig sammelnd; ihre Runtime-Overrides bleiben über Appstarts erhalten. Crashlytics wendet einen Widerruf erst beim nächsten Start an und speichert bei deaktivierter Übertragung Berichte lokal, die beim späteren Aktivieren gesendet werden. UMP autorisiert ausschließlich Werbeanfragen. Unity IAP 5.4.3 sammelt für seine Funktion immer personenbezogene Developer Data und besitzt keinen eigenen Consentservice.

Architecture v0.2 nannte diese Bausteine, modellierte aber weder einen vollständig versionierten Entscheidrecord noch alle Fresh-Install-, Upgrade-, Widerruf-, Restart-, Offline- und Re-enable-Übergänge.

## Entscheidung

### Lokale Entscheidwahrheit

Ein versionierter `PrivacyDecisionRecord` ist die einzige lokale Entscheidwahrheit. Er enthält ausschließlich `recordVersion`, `policyRevision`, `sdkContractRevision`, `decisionRevision`, `capturedAtUtc`, getrennte Capabilities für Ads/Analytics/Crash, `validity` und `source`. UMP-Strings, Advertising IDs und personenbezogene Kennungen werden nicht gespeichert.

Fehlender, defekter, unbekannter oder revisionsinkompatibler Record ergibt `INVALID`; alle effektiven Capabilities bleiben `false`. `desiredCapabilities` aus einem validen Record und `effectiveCapabilities` nach erfolgreichem Provider-Effekt sind getrennt.

### Ads

UMP `Update()` läuft bei jedem Start. Erst nach aktuellem Update/Form und `CanRequestAds() == true` darf Google Mobile Ads initialisiert oder geladen werden. Ein alter lokaler Cache ersetzt UMP nicht. Widerruf verwirft geladene beziehungsweise ausstehende Anzeigen; es gibt keine App-Queue.

### Analytics

Firebase Analytics wird nativ auf Android und iOS standardmäßig deaktiviert und startet ohne Ad-ID-/Personalisierungssignale. Ein gültiger aktueller Record darf den Runtime-Override aktivieren. Widerruf setzt ihn sofort auf `false`.

Bei einer inkompatiblen Privacy-/SDK-Revision wird ein **Reset-only-Build** verwendet: der permanente native Deaktivierungsschalter ist gesetzt, zusätzlich wird der persistente Runtime-Override auf `false` geschrieben. In diesem Binary ist Re-enable absichtlich unmöglich. Erst ein späterer Build darf den permanenten Schalter entfernen; der persistierte Runtimewert bleibt dann sicher `false` bis zu einem neuen gültigen Entscheid.

### Crashdiagnose

Firebase Crashlytics ist im Productionprofil ausgeschlossen. Der dokumentierte Provider bietet keinen belegten sofort wirksamen Widerruf innerhalb desselben Prozesses: `false` gilt erst beim nächsten Start; lokal gesammelte Berichte könnten beim Re-enable übertragen werden. Ein späterer Productioneinsatz benötigt vorab einen neuen ADR und einen auf den dann gepinnten SDKs belegten Mechanismus für sofortiges Stoppen und sichere Vorfreigabe-/Widerrufs-Löschung. Lokale, redigierte Diagnose bleibt erlaubt.

### IAP

Unity IAP 5.4.3 wird niemals beim Bootstrap initialisiert. Es startet nur nach sichtbarer Kauf-/Restoreaktion oder persistenter Recovery und nur bei freigegebenem `IapPrivacyReadiness`-Releasebeleg. Der Beleg umfasst Dateninventar, Unity-Authentication-Abhängigkeit, Dashboard-/Storedeklarationen, Privacy Policy und zuständige Freigabe. Fehlt er, bleibt der Adapter `NOT_ALLOWED`.

## Begründung

Der Vertrag verwendet nur belegte Providersemantik. Er behauptet weder eine sofortige Crashlytics-Deaktivierung noch koppelt er UMP an Analytics/Crash. Die Reset-only-Stufe verhindert, dass ein früher persistiertes Analytics-`true` bei einem invalidierenden Upgrade vor Applicationcode wieder aktiv wird.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Nur native Default-Off-Flags | Werden durch persistente Runtime-Overrides überstimmt. |
| Crashlytics im Prozess widerrufen | Anbieter dokumentiert Wirkung erst beim nächsten Start; deshalb nicht beweisbar. |
| UMP als globale Telemetrieeinwilligung | UMP belegt nur Werbeanfragbarkeit. |
| IAP beim Bootstrap | Aktiviert always-collected Daten ohne sichtbare Aktion und Readiness-Gate. |

## Konsequenzen

Firebase Unity SDK bleibt auf 13.16.0 gepinnt; das Productionprofil importiert Analytics, nicht Crashlytics. Google Mobile Ads Unity bleibt 11.5.0, Unity IAP 5.4.3. Jede Änderung dieser Pins oder nativer Konfiguration erhöht `sdkContractRevision` und erzwingt einen Privacy-Diff.

Die lokalen Architecture-v0.3-Tests prüfen Zustandsmodell, Dokumentvertrag und native Konfigurationsanforderungen. Vier physische Android-/iOS-Captures – Fresh Install, Upgrade mit früher aktiver Telemetrie, Widerruf und Re-enable – bleiben spätere Releasegates und dürfen lokal nicht als bestanden erscheinen.

## Betroffene Artefakte

`ARCHITECTURE/MOBILE_SERVICES.md`, `ARCHITECTURE/OBSERVABILITY.md`, `ARCHITECTURE/TECH_STACK.md`, `ARCHITECTURE/BUILD_AND_RELEASE.md`, Teststrategie und Architekturvalidator.

## Validierung

Ein ausführbares Referenzmodell prüft die Übergänge und Effekt-Reihenfolge. Mutationen für altes Override, UMP-Achskopplung, Ads vor `CanRequestAds`, Crashlytics im Productionprofil, Analytics-Re-enable im Reset-only-Build und eager IAP müssen scheitern. Physische Network-Captures bleiben `REQUIRED_LATER`.

## Ersetzt / ersetzt durch

Ersetzt [ADR-015](./ADR-015-mobile-transaktionen-und-privacy-default-off.md). Die Analytics-Upgrade-, Re-enable- und Widerrufsanteile werden durch [ADR-024](./ADR-024-privacy-bootstrap-fence-und-widerruf.md) ersetzt; Ads-, Crashlytics- und IAP-Grenzen bleiben angenommen.

## Referenzen

[1]: https://firebase.google.com/docs/analytics/android/configure-data-collection "Firebase Analytics Android collection controls"
[2]: https://firebase.google.com/docs/analytics/ios/configure-data-collection "Firebase Analytics Apple collection controls"
[3]: https://firebase.google.com/docs/crashlytics/unity/customize-crash-reports "Firebase Crashlytics Unity opt-in reporting"
[4]: https://developers.google.com/admob/unity/privacy "Google UMP Unity"
[5]: https://docs.unity.com/en-us/iap/privacy-and-consent/overview "Unity IAP 5.4 Privacy overview"
[6]: ../WORK_PACKAGES/WP-003_Architecture-v0.3-Finalkorrekturen.md "WP-003"
