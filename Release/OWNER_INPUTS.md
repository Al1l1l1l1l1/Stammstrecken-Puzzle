# Externe Begleitliste zu WP-046

Stand 21.10.2026. R führt die vollständige Appentwicklung parallel weiter. Die folgenden externen Angaben sind noch nicht belegt; betroffen ist jeweils nur der genannte Test-/Freigabeschritt.

| Was wird gebraucht? | Wann / wofür? |
|---|---|
| Play-Kontotyp, Erstellungsdatum, Productionaccess und reale Tester | Früh für den Testtrack: persönliche Konten nach 13.11.2023 können 12 durchgehend 14 Tage optierte Closed-Test-Tester plus Productionaccessprüfung brauchen. |
| Apple-/Play-/AdMobrollen, finale je Profil getrennte App-/Adunit-/Produkt-IDs | Vor echten UMP-/Anzeigen-/Kauf-/Restore-Sandboxtests. Bis dahin offizielle Testanzeigen und fail-closed Dienste. |
| Play-App-Signing/Uploadkey, Apple-Team/Provisioning/CIrollen und Befugnis zur internen Verteilung | Vor signierten internen Android-/iOSbuilds und TestFlight-/Play-Trackupload. |
| Physische Android-/iOSgeräte und reale Spieltester | Jetzt für die Handprobe der korrigierten Haupt-APK7: kleine/große Telefonansicht, acht direkte Werkzeuge und ortsfestes10×10. Die getrennte Human-APK4 enthält jetzt alle17 neutral wählbaren Sechsergruppen/102Aufgaben; echte Erstkontakte mit lokalem Export werden für Zeit-/Difficultyannahme gebraucht; vor M3 sind reale Android-/iOS-Privacy-, Kauf-, Lifecycle- und Storekanalbelege nötig. |
| Rechtlicher Publisher, Support-/Impressumkontakt, öffentliche Privacy-/Support-URL und qualifizierte Rechts-/Datenschutzreview | Vor tatsächlicher IAPreadiness und finaler Privacy-/Rechte-/Storefreigabe; Storetexte entstehen parallel. |

**Sicherer Weg:** Nicht geheime Status/IDs können ergänzt werden. Kontozugang über appbezogene Rollen; Keystore/Passwörter/Appleprivatekeys/APIsecrets ausschließlich von der zuständigen Person direkt in geschützte CI-Environment-Secrets. Keine Secretwerte oder Kaufreceipts in Chat/Repository. R benötigt nur Secretnamen und tatsächlich eingeräumte Befugnisse.

Am 21.10.2026 offiziell geprüft: [Google API36](https://developer.android.com/google/play/requirements/target-sdk), [Apple Xcode26/iOS26-SDK](https://developer.apple.com/news/upcoming-requirements/), [Play-Testvoraussetzung](https://support.google.com/googleplay/android-developer/answer/14151465?hl=en). Externe Verträge/Zahlungen und öffentliche Einreichung/Promotion bleiben konkrete Ownerhandlungen.
