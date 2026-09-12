# Privacy Provider Evidence für Architecture v0.4

**Stand:** 2026-09-12

## 1. Zweck und Aussagegrenze

Dieses Dokument sichert die extern verifizierten Herstellerregeln, aus denen der lokale Privacy-Vertrag abgeleitet wird. Es ist kein Gerätebeleg. Native Manifest-/`Info.plist`-Wirkung, direkte Upgradepfade, Prozessreihenfolge und Netzwerkverkehr müssen später auf physischen Android- und iOS-Geräten bewiesen werden.

## 2. Gepinnte SDK-Linien und Providerregeln

| Provider | Pin / Productionrolle | Verifizierte Herstellerregel | Architekturschluss |
|---|---|---|---|
| Firebase Analytics Unity | 13.16.0; **bis Fence-Nachweis nicht im Productionpfad aktivierbar** | Android kennt `firebase_analytics_collection_enabled=false` als initialen Default und `firebase_analytics_collection_deactivated=true` als permanente Deaktivierung. iOS kennt `FIREBASE_ANALYTICS_COLLECTION_ENABLED=NO` und `FIREBASE_ANALYTICS_COLLECTION_DEACTIVATED=YES`. Runtime-`SetAnalyticsCollectionEnabled` überdauert App-Sessions. | Ein alter persistierter Runtime-Override kann einen späteren Default überstimmen. Ein Reset-only-Zwischenbuild ist kein sicherer Migrationsanker, wenn Nutzer ihn überspringen oder nie starten. Ohne belegten prä-SDK-Mechanismus bleibt Analytics Production-aus. |
| Firebase Crashlytics Unity | 13.16.0; **Production ausgeschlossen** | Collection lässt sich opt-in konfigurieren; der Runtime-Override persistiert, und Änderungen können erst beim nächsten Lauf vollständig wirken. Nicht gesendete Reports können gelöscht werden. | Die einfachste belastbare Productiongrenze bleibt Package-/Binary-Ausschluss. Lokale redigierte Diagnose ersetzt keinen Crashprovider. |
| Google Mobile Ads Unity / UMP | 11.5.0 | Consentinformation wird bei jedem Start aktualisiert. Anzeigenanfragen sind nur zulässig, wenn `CanRequestAds()` wahr ist; Privacyoptionen können erneut erforderlich werden. | Ads bleiben uninitialisiert beziehungsweise ohne Requestfreigabe, bis der aktuelle UMP-Startfluss sie erlaubt. Geladene Ads werden bei Widerruf verworfen. |
| Unity IAP | 5.4.3; lazy readiness | IAP erhebt verpflichtende Developer Data und kann nach Kaufnachweisen wiederholt Pending-/Replay-Ereignisse liefern. Entitlement soll vor Storefinalisierung dauerhaft erfüllt werden. | IAP ist kein an den optionalen Analytics-Consent gekoppelter Dienst. Es wird nur für Kaufaktion oder Recovery/readiness initialisiert; Offenlegung und Datenminimierung bleiben Pflicht. |

## 3. Prä-SDK-Fence und direkte Upgradepfade

Ein **prä-SDK-Deny-Fence** ist nur erfüllt, wenn auf beiden Plattformen vor jeder möglichen Firebase-Analytics-Initialisierung oder -Erfassung ein beliebiger persistierter Runtime-Override nachweislich auf `false` neutralisiert wird. Eine Application-Capability, die nach Unity-Start auf `false` steht, genügt nicht. Ebenso genügt ein Reset-only-Build nicht, weil App Stores weder die Installation noch den Start jeder Zwischenversion garantieren.

Für die gepinnte Unity-/Firebase-Integration liegt im Repository kein solcher plattformbezogener Nachweis vor. Deshalb verlangt ADR-024 fail-closed:

> Analytics bleibt in Production deaktiviert, bis ein separates Implementierungs-/Release-Work-Package den prä-SDK-Fence, direkte Legacy-Upgrades, den „installiert, nie gestartet“-Pfad, Crashinjektion und Netzwerkstille auf physischen Android- und iOS-Geräten belegt.

Ein künftiger Nachweis muss die exakten exportierten Android-Manifest- und Apple-`Info.plist`-Werte, Firebase-Initialisierungsreihenfolge, persistierte Runtime-Overrides, Process-Restarts und beobachteten Netzwerkverkehr an den unveränderlichen Releasekandidaten binden.

## 4. Widerruf und Native-Synchronisierung

`PrivacyDecisionRecord.nativeSyncState` trennt lokale Entscheidung von bestätigtem Providerzustand. `REVOKE_PENDING` wird vor Native Disable persistiert. Ein Crash davor oder danach startet erneut mit prä-SDK-Deny-Fence und Reconciliation. Erst bestätigtes Native Disable führt zu `REVOKED_CONFIRMED`. `ENABLE_PENDING` bleibt capability-seitig aus, bis der Providerapply bestätigt ist. Ein Providercallback darf keinen neueren Widerruf überschreiben.

Das ausführbare Fixture [`privacy-lifecycle-v2.json`](../tools/architecture-validation/fixtures/privacy-lifecycle-v2.json) modelliert Fresh Install, direkten Legacy-Sprung, übersprungenen Reset-only-Build, Widerrufs-Crash, Native-Disable-Fehler, Recovery, Re-enable-Referenzpfad und Offlinefälle. Der lokale Validator beweist nur Reducer- und Fixturesemantik; physische Providerwirkung bleibt `REQUIRED_LATER/NOT_EXECUTED`.

## 5. Quellen

[1]: https://firebase.google.com/support/release-notes/unity "Firebase Unity SDK Release Notes – 13.16.0"
[2]: https://github.com/googleads/googleads-mobile-unity/releases/tag/v11.5.0 "Google Mobile Ads Unity Plugin 11.5.0"
[3]: https://firebase.google.com/docs/analytics/android/configure-data-collection "Firebase Analytics – Android data collection"
[4]: https://firebase.google.com/docs/analytics/ios/configure-data-collection "Firebase Analytics – Apple data collection"
[5]: https://firebase.google.com/docs/crashlytics/unity/customize-crash-reports "Firebase Crashlytics Unity – opt-in reporting"
[6]: https://firebase.google.com/docs/crashlytics/android/customize-crash-reports "Firebase Crashlytics Android – collection controls"
[7]: https://firebase.google.com/docs/crashlytics/ios/customize-crash-reports "Firebase Crashlytics Apple – collection controls"
[8]: https://developers.google.com/admob/unity/privacy "Google UMP for Unity"
[9]: https://docs.unity.com/en-us/iap/privacy-and-consent/overview "Unity IAP privacy and consent"
[10]: https://docs.unity.com/en-us/iap/purchases "Unity IAP purchase processing"
[11]: ../DECISIONS/ADR-024-privacy-bootstrap-fence-und-widerruf.md "ADR-024 – Privacy-Bootstrap-Fence und Widerruf"
