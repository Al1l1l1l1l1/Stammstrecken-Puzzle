# Externe Begleitliste zu WP-046

Stand01.11.2026. Die vollständige aktuelle technische Prüfkette ist angenommen. Diese Beiträge werden für die jeweils genannten nächsten Schritte gebraucht.

| Was du bereitstellen oder erledigen musst | Wann es gebraucht wird |
|---|---|
| Deine Rückmeldung zu Gestaltung, Bedienung und Spielqualität | Jetzt mit der geprüften [Main22-Spiel-App](ANDROID_BUILD22_INSPECTION.json); vor der Zusammenführung nach Main. |
| Ein Androidtelefon und echte Erstkontaktspieler mit unterschiedlichen Spielstilen; später ein iPhone | Android jetzt für den echten Handtest; am01.11. war keines angeschlossen. [Human11](HUMAN_ANDROID_BUILD11_INSPECTION.json) ist für neue Messreihen bereit. Die [Durchführung in der Hauptübergabe](../PROJECT_CONTROL/ProductionBlocks/ReleaseCandidate/HANDOVER.md#echte-erstkontakte-mit-human11) nennt Stichprobe, Bedienung und Export. iPhone vor signierter iOS-Geräteprüfung. |
| Google-Play-Kontotyp, Erstellungsdatum und vorhandenen Produktionszugang nennen; reale Storetester organisieren | Vor geschlossenem Play-Test und öffentlichem Vertrieb. Die konkreten Konto-Testvoraussetzungen werden dafür geprüft; keine feste Testerzahl ohne Kontokontext behauptet. |
| Appbezogene Rollen für Apple Developer/App Store Connect, Google Play und AdMob; endgültige App-, Anzeigen- und Kaufkennungen | Vor Tests mit echten Diensten und Storekanälen: Einwilligung, Werbung, Kauf und Wiederherstellung. |
| Zuständige Person für Android-/Apple-Signierung und Befugnis zur internen Testverteilung | Vor signierten Testfassungen, TestFlight und Play-Uploads. Einrichtung und Freigaberollen stehen im [Runbook](RC_PIPELINE.md). Der aktuelle iOS-Compile ist unsigniert und noch keine installierbare iPhone-Testfassung. |
| Herausgeber, tatsächliches Abrechnungsland der Anzeigen-/Storekonten, Support-/Impressumkontakt und öffentliche Rechts-/Datenschutz-/Supportseiten; qualifizierte Prüfung | Endgültige Angaben vor Produktionsbuild; passende SDK-Bedingungszuordnung und Rechts-/Datenschutz-/Rechteprüfung vor abschließender Storefreigabe. |

**Bereit:** Spiel-App `.utmp/wp046-native/android-build22-verified/stp-mobile-preview.apk`; Mess-App `.utmp/wp046-native/human-android-build11-verified/stp-human-calibration.apk`. Beide sind unabhängig geprüfte lokale Android-Testfassungen. Human11 bindet Android08/Revision8. Echte Geräte-/Menschenprüfungen stehen aus. Alte Messreihen unverändert abschließen und Originalexporte vor Personen-/Laufwechsel oder Appupdate sichern; alte Installationsdateien und Spielstände erhalten. Anders signierte CI-APKs ersetzen die lokale Fassung nicht automatisch.

Kontozugang über Rollen. Schlüssel, Passwörter und andere Geheimwerte nur von der zuständigen Person direkt in geschützte CI-Eingaben; keine Geheimwerte oder Kaufbelege im Chat/Repository. Öffentliche Einreichung und Veröffentlichung bleiben eigene Freigabeschritte. Fehlendes blockiert nur den jeweiligen Schritt.
