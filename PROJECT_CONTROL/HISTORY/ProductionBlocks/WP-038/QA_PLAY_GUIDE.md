# Interne Rätselschleife – Bedienung und Prüfumfang

Die Szene `Assets/StammstreckenPuzzle/Scenes/QA/DevelopmentPuzzle.unity` ist der eigene interne QA-Einstieg. Sie enthält ein neues öffentliches 4×4-Testrätsel ohne Kampagnen-, Autorenlösungs-, Proof- oder Textbibliotheksreferenzen. Die zusätzliche 10×10-Definition dient der automatisierten Layout-/Vertragsprüfung. Dies ist kein WP026-Kampagnen-Spieltest und keine vollständige freigegebene Spielversion.

## Bedienung

Im festgelegten Unity6000.3.23f1-Projekt die QA-Szene öffnen und Play starten. Ein Werkzeug wählen, danach ein Feld betätigen. Sechs Gleisformen, X als Leernotiz, Grau als Belegungsnotiz und Leeren sind direkt erreichbar. Gelb bezeichnet das aktive Werkzeug beziehungsweise ausgewählte Felder; es bewertet keine Lösung. Die Randzahlen zeigen ausschließlich sichtbare Belegung im Verhältnis zum öffentlichen Ziel.

„Mehrfachauswahl“ sammelt Felder, ohne sie bereits zu verändern. Erst „Auswahl anwenden“ führt das gewählte Werkzeug als einen atomaren Befehl aus. „Rückgängig“ nimmt genau einen solchen Befehl zurück. Auswahl und wirkungslose Eingaben starten die Uhr nicht; echte Änderungen starten sie. App-Pause beziehungsweise Fokusverlust schließt Hintergrundzeit aus.

Eine tatsächlich regelgültige A→B-Verbindung sperrt die Eingabe und startet den neutralen technischen Zug auf genau der selbst gebauten Zellfolge. Das Ergebnis erscheint erst nach Fahrtende. „Neuer Versuch“ erzeugt danach eine neue Versuchidentität. Der Abschluss bleibt ausdrücklich ungespeichert; es entstehen keine Sterne, Punkte, Rewards oder Kampagnenfortschritte. Szenen-/Appneustart verwirft diesen QA-Versuch.

## Reproduktion und Builds

`STP.Editor.Build.DevelopmentPuzzleBuild.EnsureArtifacts` erzeugt ausschließlich die benannte eigene QA-Szene, Panelsettings und das technische Trainmaterial deterministisch. `VerifyArtifacts` prüft deren vorhandenen Inhalt ohne Reparatur. Reguläre Android-/iOS-Builds laufen weiter über `STP.Editor.Build.StpBuildEntrypoints.BuildAndroidDevelopmentIl2Cpp` beziehungsweise `ExportIosXcodeProjectIl2Cpp`; neue Spielszene zuerst, unveränderte Composition-Smoke-Szene zusätzlich. Beide Plattformen behalten die explizite QA-ID `com.STP.StammstreckenPuzzle.qa`. Ein Productionprofil ist nicht zugelassen.

Die abschließende Ausführungsübergabe benennt tatsächliche Code-/Build-/CI-/Grafikstände und verfügbare Artefakte. Dieser Bedienungstext allein behauptet keinen bereits ausgeführten Unity-/Geräte-/Grafiktest. Physische Geräte, finale Gesten/Accessibility/Motion, menschliche Kampagnenprüfung, produktive Content-/Saveverbindung und Release bleiben eigene nachfolgende Nachweise.
