# Externe Begleitliste zu WP-046

Stand 25.10.2026. Die Appentwicklung läuft weiter. Diese noch offenen Beiträge werden erst für die jeweils genannten Schritte gebraucht.

| Was wird gebraucht? | Wann / wofür? |
|---|---|
| Google-Play-Kontotyp und Erstellungsdatum nennen; echte Tester organisieren | Früh vor dem geschlossenen Storetest. Für persönliche Konten nach dem 13.11.2023 können zwölf durchgehend 14 Tage angemeldete Tester und die anschließende Freigabe für den öffentlichen Vertrieb erforderlich sein. |
| Zugriff auf Apple Developer/App Store Connect, Google Play und AdMob per Rolleneinladung bereitstellen; endgültige App-, Anzeigen- und Kaufkennungen festlegen | Vor Tests mit den echten Diensten: Einwilligung, Werbung, Kauf und Wiederherstellung. Die Spielentwicklung verwendet bis dahin die vorhandenen Testwege. |
| Zuständige Person für Android-/Apple-Signierung benennen und interne Testverteilung erlauben | Vor signierten Testfassungen und dem Hochladen zu TestFlight oder Google Play. Die Person hinterlegt Schlüssel und weitere geschützte Angaben direkt in der Buildumgebung. Die genaue Einrichtung einschließlich Freigabeperson für `production` steht im [Runbook](RC_PIPELINE.md). |
| Androidtelefon und echte Erstkontaktspieler bereitstellen; später ein iPhone | Android jetzt für deine Spielprobe der Haupt-APK18 und echte Messungen mit der getrennten Human-APK8. Das iPhone folgt, sobald die signierte iOS-Testfassung bereitsteht. Vor dem Release brauchen wir außerdem Geräteprüfungen für Datenschutz, Käufe, Unterbrechen/Fortsetzen, Bedienung und Leistung. [Spielpaket](ANDROID_BUILD18_INSPECTION.json), [Messpaket](HUMAN_ANDROID_BUILD8_INSPECTION.json). |
| Rechtlichen Herausgeber, Support-/Impressumkontakt und öffentliche Rechts-/Datenschutz-/Supportseiten bereitstellen; qualifizierte Rechts- und Datenschutzprüfung veranlassen | Endgültige Angaben vor dem Produktionsbuild; die Prüfung vor der abschließenden Kauf-, Rechte-, Datenschutz- und Storefreigabe. Entwürfe und Umsetzung laufen währenddessen weiter. |

Deine Spielprobe zu Gestaltung, Bedienung und Spielqualität wird vor der Zusammenführung nach Main gebraucht. Die technische Umsetzung läuft weiter. Die erneute lesende Abfrage am 25.10.2026 (Berlin) fand kein angeschlossenes Androidgerät.

Für neue Messreihen bleibt Human-APK8 mit Android06/Revision6/UI75 und 102 Aufgaben vorgesehen. Bestehende Messreihen mit ihrer bisherigen Fassung abschließen und vor Update oder nächster Person exportieren; Originale behalten. Alte Ergebnisse werden nicht umbenannt oder automatisch übertragen. Installationsdateien: `.utmp/wp046-native/android-build18-verified/stp-mobile-preview.apk` und `.utmp/wp046-native/human-android-build8-verified/stp-human-calibration.apk`.

**Sicherer Weg:** Nicht geheime Status/IDs können ergänzt werden. Kontozugang über appbezogene Rollen; Keystore/Passwörter/Appleprivatekeys/APIsecrets ausschließlich von der zuständigen Person direkt in geschützte CI-Environment-Secrets. Keine Secretwerte oder Kaufreceipts in Chat/Repository. R benötigt nur Secretnamen und tatsächlich eingeräumte Befugnisse.

Am 21.10.2026 offiziell geprüft: [Google API36](https://developer.android.com/google/play/requirements/target-sdk), [Apple Xcode26/iOS26-SDK](https://developer.apple.com/news/upcoming-requirements/), [Play-Testvoraussetzung](https://support.google.com/googleplay/android-developer/answer/14151465?hl=en). Externe Verträge/Zahlungen und öffentliche Einreichung/Promotion bleiben konkrete Ownerhandlungen.
