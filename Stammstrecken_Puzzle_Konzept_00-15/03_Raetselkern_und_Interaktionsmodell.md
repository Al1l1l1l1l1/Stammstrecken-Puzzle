# Stammstrecken-Puzzle – Rätselkern und Interaktionsmodell

**Status:** Aktuelle beschlossene Produktfassung, 20.10.2026, MANUS-2026-10-20-02. Die direkten Ownerentscheidungen O1/O2 aus Issue38 ersetzen ausdrücklich den bisherigen Mehrfachauswahl-/Anwendenablauf und die bisherige gleiches-Werkzeug-No-op-Regel. Historische Vorbildbeobachtungen bleiben in Konzept02 und Git nachvollziehbar; dies ist das aktuelle Modell unseres Spiels.

## 1. Rätselkern

Ein Level besteht aus einem Rechteckraster, äußeren A/B-Anschlüssen und einer Zahl für jede Zeile und Spalte. Diese Zahlen geben an, wie viele Rasterfelder der jeweiligen Linie Schienen enthalten. Die Lösung ist genau eine zusammenhängende einfache A–B-Strecke; keine Kreuzung, Schleife, Verzweigung, zusätzlichen losen Gleise oder falschen Außenanschlüsse. Einzig die belegten Felder der gültigen Strecke erfüllen die Randzahlen.

Der Spieler entscheidet, welche Felder belegt sind und welche der sechs Formen in ihnen liegt. Beide Denkebenen können ineinandergreifen. Es gibt keine Pflicht, zuerst Notizen zu setzen. Season1 enthält keinerlei Sonderfelder oder Zusatzregeln.

## 2. Feldinhalte und semantische Trennung

| Inhalt | Darstellung | Bedeutung |
|---|---|---|
| Leer/unberührt | Neutrales Rasterfeld | Keine Notiz oder konkrete Schiene. |
| Leerannahme | Rotes X | Der Spieler vermutet oder folgert, dass hier keine Schiene liegt. |
| Belegungsannahme | Kleines graues Standardgleis | Hier wird eine Schiene vermutet; ihre konkrete Form ist noch offen. |
| Konkrete Schiene | Braune Gleisform | Ein konkreter Abschnitt der Strecke wurde gesetzt. |

X und Grau sind vollständig optionale eigene Hilfsnotizen, keine Levelvorgaben oder automatischen Richtigkeitsurteile. Sie dürfen umfangreich, punktuell oder gar nicht verwendet werden und verhindern niemals drei Sterne. Braun bezeichnet Gleisinhalt. Gelb bezeichnet ausschließlich das aktuell aktive Werkzeug beziehungsweise kurzes neutrales Eingabefeedback; es ist kein Feldinhalt, keine Gleisfarbe und kein Richtigkeitsstatus.

## 3. Genau sechs Gleisformen

| Form | Anschlüsse |
|---|---|
| Senkrecht | Oben ↔ unten |
| Waagerecht | Links ↔ rechts |
| Kurve oben–rechts | Oben ↔ rechts |
| Kurve rechts–unten | Rechts ↔ unten |
| Kurve unten–links | Unten ↔ links |
| Kurve links–oben | Links ↔ oben |

## 4. Aktuelle direkte Bedienung

Der Ablauf ist immer **Werkzeug wählen → Felder einzeln direkt antippen**. Das Werkzeug bleibt aktiv, bis ein anderes gewählt wird. Jeder Tap bearbeitet genau das getroffene Feld sofort, ohne Vormerken, Sammelauswahl oder zusätzliche Anwendung/Bestätigung.

Für jede der sechs Gleisformen sowie X und Grau gilt dieselbe Operation:

- Ist der Feldinhalt anders als das aktive Werkzeug, wird der Inhalt durch dieses Werkzeug ersetzt.
- Entspricht der Feldinhalt genau dem aktiven Werkzeug, wird das Feld vollständig geleert.
- Das aktive Werkzeug bleibt in beiden Fällen unverändert.

Beispiel: Senkrecht wählen, leeres Feld antippen → senkrechte Schiene. Dasselbe Feld wieder mit Senkrecht antippen → leer. Enthält es stattdessen eine Kurve oder X, setzt der Tap Senkrecht. Das gilt identisch für jede Kurve und beide Notiztypen; es gibt keine automatische Rotations-/Wechselreihenfolge.

**Mehrfachauswahl, Auswahl-anwenden-Button und zugehöriger Bestätigungsablauf entfallen vollständig aus der Bedienung.** Ihre alte Verwendung im Prototyp ist kein Bestandteil des Launchprodukts. Zurück bleibt ein verständliches direktes Zeichenwerkzeug, keine Auswahlverwaltung. Eine künftige Drag-/Mal-/Batchgeste ist nicht automatisch beauftragt; sie darf Direkt-Tap nicht beeinträchtigen und braucht eine ausdrückliche UXentscheidung.

## 5. Weitere Werkzeuge und Undo

`Leeren` bleibt ein direkt erreichbares Werkzeug, das einen vorhandenen Inhalt unabhängig von dessen Typ entfernt; ein bereits leeres Feld bleibt leer. `Rückgängig` stellt die letzte tatsächliche Feldänderung wieder her, einschließlich des zuletzt per gleichem Werkzeug ausgelösten Leerens. Ein No-op auf leerem Feld erzeugt keinen neuen Undo-/Save-/Zählschritt. Ein neu geschriebenes Feld ist eine atomare Spielerhandlung.

Hinweise werden nur aktiv angefordert und geben einen objektiv sicheren logischen nächsten Schluss, keine Gesamtlösung und keine heimliche Bewertung plausibler eigener Annahmen. Bezugs-/Speichersemantik steht in Konzept06 und dem aktuellen Gesamtmandat. Fehler, Undo, Notizen und Hinweise verursachen keine Lebens-/Fortschritts-/Sternstrafe; allein das zusätzliche Merkmal „direkt gelöst“ setzt eine Lösung ohne Notizen und Hinweise voraus.

## 6. Typischer, nicht vorgeschriebener Lösungsweg

Nullzeilen/-spalten können mit X ausgeschlossen werden. Volle Linien und A/B-Anschlussfelder können grau als belegt vorgemerkt oder direkt konkret gesetzt werden. Sind genügend belegte Felder einer Linie bekannt, lassen sich verbleibende Felder ausschließen. Diese Aussagen wirken auf kreuzende Linien zurück. Rand-/Nachbar-/Konnektivitätslogik bestimmt schließlich die konkreten Formen; ein belegtes Eckfeld ohne passenden Außenanschluss benötigt die entsprechende Kurve.

Das ist eine mögliche externe Denkstütze, kein Tutorialzwang oder Mindestworkflow. Systematische, gemischte und direkte schnelle Lösungen sind gleichwertig. Es gibt keine „Spielzüge“ im Schach-Sinn; Züge sind ausschließlich kosmetische Fahrzeuge für die Abschlussfahrt.

## 7. Ausführungs- und Stabilitätsregeln

Ein Tap wirkt unmittelbar auf das dargestellte Ziel; Rendering oder Autosave dürfen keine Zelle unter dem Finger verschieben. Raster, Randzahlen und Werkzeuge bleiben bei normaler Eingabe, Werkzeugwechsel, Undo und Savezustandswechsel ortsfest. Routinemäßiges Speichern geschieht im Hintergrund ohne wechselnde mehrzeilige Status-/Verwaltungstexte über dem Feld. Echte Savefehler bleiben erkennbar und werden zuverlässig behandelt, ohne durch Layoutspringen weitere Eingaben zu verfälschen.

Der Timer startet erst mit der ersten tatsächlichen Feldeingabe, nicht beim Lesen oder der bloßen Werkzeugwahl. Unterbrechung/Lifecycle und verifizierter Abschluss richten sich nach dem bestehenden Zustands-/Savevertrag. Eine gültige Strecke wird erkannt, zuverlässig gespeichert und danach durch die echte Fahrt von A nach B gewürdigt. UX-/Laufzeitakzeptanz steht in Konzept12 und der risikogerechten Definition of Done.
