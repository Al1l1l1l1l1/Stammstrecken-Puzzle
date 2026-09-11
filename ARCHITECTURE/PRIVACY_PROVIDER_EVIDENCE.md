# Privacy Provider Evidence für Architecture v0.3

**Stand:** 2026-09-12
**Zweck:** Nachvollziehbare Herstellerbasis für `ADR-020` und die Mobile-/Observability-Verträge. Dieses Dokument ist keine Rechtsberatung und ersetzt keine Releasefreigabe.

## Gepinnte SDK-Linien

| Anbieter | Pin | Verifizierter Herstellerstand |
|---|---:|---|
| Firebase Unity SDK | `13.16.0` | Am 27. August 2026 veröffentlicht; verwendet Firebase C++ 13.11.0, Android BoM 34.18.0 und iOS Cocoapods 12.18.0.[1] |
| Google Mobile Ads Unity Plugin | `11.5.0` | Am 3. September 2026 veröffentlicht; bindet GMA iOS 13.9.0 und Android Next-Gen 1.4.0.[2] |
| Unity IAP | `5.4.3` | Gepinnter IAP-5.4-Patch; Privacyvertrag gilt für 5.4 und neuer.[3] |

## Verifizierte Lifecycle-Regeln

Firebase Analytics lässt die automatische Sammlung nativ über `firebase_analytics_collection_enabled=false` auf Android beziehungsweise `FIREBASE_ANALYTICS_COLLECTION_ENABLED=NO` auf Apple deaktivieren. Ein Runtimeaufruf kann die Sammlung aktivieren oder wieder suspendieren. Auf Apple dokumentiert Firebase ausdrücklich, dass der Runtimewert über Appausführungen persistiert und den temporären Plist-Wert überstimmt. Der permanente Schalter `firebase_analytics_collection_deactivated=true` beziehungsweise `FIREBASE_ANALYTICS_COLLECTION_DEACTIVATED=YES` hat Vorrang und kann im selben Binary nicht per Runtime reaktiviert werden.[4][5]

Firebase Crashlytics sammelt standardmäßig automatisch. Ein Runtime-Override persistiert über Starts. Ein späteres Opt-out wird laut Android-, Apple- und Unity-Dokumentation erst beim nächsten Appstart wirksam. Bei deaktivierter Sammlung speichert Crashlytics Berichte lokal; nach späterer Aktivierung werden diese Berichte übertragen.[6][7][8] Diese dokumentierte Semantik erfüllt keinen belegbaren sofortigen Widerruf innerhalb desselben Prozesses. Deshalb ist Crashlytics im v0.3-Productionprofil ausgeschlossen.

Google UMP verlangt `ConsentInformation.Update()` bei jedem Start. `CanRequestAds()` ist vor dem Update immer `false`; Ads dürfen erst nach Update/Form und `CanRequestAds()==true` initialisiert oder geladen werden. UMP ist die Autorität für Werbeanfragen, nicht für Firebase Analytics oder Crashdiagnose.[9]

Unity IAP 5.4 und neuer sammelt zur Funktion immer Player ID, Unity Installation ID, Geräteinformationen, Session IDs und Land. Es aktiviert die Unity-Authentication-Abhängigkeit und besitzt keinen eigenen Consentservice. Eine App muss notwendige Rechtsgrundlage, Privacy Policy, Storedeklarationen und Datenverarbeitung selbst freigeben.[3]

## Architekturableitung

Die dokumentierten Providermechanismen führen zu drei fail-closed Grenzen:

1. **Analytics-Invalidierung benötigt Reset-only:** Ein invalidierender Build setzt permanenten Analytics-Off und schreibt zusätzlich den persistierten Runtimewert `false`. Re-enable ist erst in einem späteren Build ohne permanenten Schalter und nach neuem gültigem Entscheid möglich.
2. **Crashlytics bleibt aus Production entfernt:** Solange kein später gepinntes SDK einen belegten sofortigen Widerruf sowie sichere Behandlung lokal vor Freigabe erfasster Berichte ermöglicht, gibt es keinen Production-Enable-Pfad.
3. **IAP bleibt lazy und readiness-gesteuert:** Keine Bootstrapinitialisierung; nur sichtbare Kauf-/Restoreaktion oder persistente Recovery nach dokumentierter Releasefreigabe.

## Quellen

[1]: https://firebase.google.com/support/release-notes/unity "Firebase Unity SDK Release Notes – 13.16.0"
[2]: https://github.com/googleads/googleads-mobile-unity/releases/tag/v11.5.0 "Google Mobile Ads Unity Plugin 11.5.0"
[3]: https://docs.unity.com/en-us/iap/privacy-and-consent/overview "Unity IAP 5.4 Privacy overview"
[4]: https://firebase.google.com/docs/analytics/android/configure-data-collection "Firebase Analytics Android collection controls"
[5]: https://firebase.google.com/docs/analytics/ios/configure-data-collection "Firebase Analytics Apple collection controls"
[6]: https://firebase.google.com/docs/crashlytics/android/customize-crash-reports "Firebase Crashlytics Android opt-in reporting"
[7]: https://firebase.google.com/docs/crashlytics/ios/customize-crash-reports "Firebase Crashlytics Apple opt-in reporting"
[8]: https://firebase.google.com/docs/crashlytics/unity/customize-crash-reports "Firebase Crashlytics Unity opt-in reporting"
[9]: https://developers.google.com/admob/unity/privacy "Google UMP Unity"
