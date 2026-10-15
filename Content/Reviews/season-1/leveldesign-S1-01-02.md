# Author-Leveldesignreview: S1-01-02 (globale Route 02)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Hohe Randzahlen | S1-01-02-01: 4, S1-01-02-02: 5 |
| 3–5 | Belegung als optionale Notiz | S1-01-02-03: 6, S1-01-02-04: 6, S1-01-02-05: 6 |
| 6–8 | Gekreuzte Belegungs-/Ausschlusslogik | S1-01-02-06: 7, S1-01-02-07: 7, S1-01-02-08: 7 |
| 9–10 | Nahe Endpunkte | S1-01-02-09: 6, S1-01-02-10: 6 |
| 11–12 | Längere eindeutige Kette | S1-01-02-11: 9, S1-01-02-12: 11 |

**Individuelle Routenbewertung:** Die senkrechten und waagerechten Vollanker wechseln gezielt von 1–5; 6–8 verknüpfen sie mit Engpässen oder voneinander entfernten Vollzeilen. Position 9 reduziert die Fläche bewusst für nahe Endpunkte, Position 10 kontrastiert dieselbe Nähe durch einen langen Umweg und höhere Dichte. Die angrenzenden Endpunktzellen von 10 haben Abstand 3, nicht die Länge des Lösungspfads; diese Unterscheidung wurde bei der eigenen Bewertung korrigiert. 11/12 erhöhen vor allem die Formlast innerhalb sicher belegter Zellen. Die Kurvenzahl 15 im Abschluss ist die konkrete Lesbarkeitsfrage; der Solveranstieg allein setzt keine neue Grenze.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-01-02-01

- Input: 4×5; A E/4, B W/1. Zeilen `[0, 1, 4, 3, 1]`, Spalten `[3, 2, 1, 3]`.
- Lösung: 9 Zellen, 6 Kurven; Dichte 9/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=43, maxDeductionDepth=4.
- Einstieg: Die volle Zeile 2 gibt einen quer über das neue 4×5-Feld reichenden Belegungsanker. Belegschritte: `[21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[4, 18, 32, 33]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY']`; erwartete gedankliche Folge 3 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die leere obere Zeile hält den Größenwechsel übersichtlich; die Einserzeilen an den beiden Endpunkten erlauben sichere Ausschlüsse. Der mittlere Rückknick verschiebt die Formlast ins Zentrum, weshalb dies trotz ähnlichem Randzug kein bloßes verlängertes Erstlevel ist.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....
01 ┐...
02 │┌─┐
03 └┘.│
04 ...└
```

## S1-01-02-02

- Input: 4×5; A S/1, B W/0. Zeilen `[3, 2, 1, 2, 2]`, Spalten `[1, 5, 4, 0]`.
- Lösung: 10 Zellen, 7 Kurven; Dichte 10/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=51, maxDeductionDepth=5.
- Einstieg: Spalte 1 ist über alle fünf Zeilen belegt. Belegschritte: `[18, 19, 20, 21]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 14, 27, 28, 44]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'CHAIN']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Spalte 2 mit vier Zellen muss zu diesem durchgehenden Anker passen, während die rechte Nullspalte frei bleibt. Der untere Wechsel zwischen den beiden Spalten verändert mehrfach die Form; eine Belegungsnotiz ist hilfreich, aber nicht als Pflicht behauptet.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ──┐.
01 .┌┘.
02 .│..
03 .└┐.
04 .┌┘.
```

## S1-01-02-03

- Input: 4×5; A N/2, B W/4. Zeilen `[2, 2, 2, 4, 3]`, Spalten `[2, 1, 5, 5]`.
- Lösung: 13 Zellen, 11 Kurven; Dichte 13/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=45, maxDeductionDepth=6.
- Einstieg: Spalten 2 und 3 sind beide voll; A an N/2 fixiert den nördlichen Außenport. Belegschritte: `[16, 25, 26, 27]`.
- Eine tatsächliche maximale Prämissenkette: `[15, 22, 33, 34, 36, 40]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei obere Zweierzeilen begrenzen die Breite der Kurvenfolge, bevor die volle Zeile 3 die linke Hälfte einbindet. Eine Notiz trennt hier anschaulich sichere Belegung und noch offene Form; elf Kurven sind die neue Geometrielast.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└┐
01 ..┌┘
02 ..└┐
03 ┌─┐│
04 ┘.└┘
```

## S1-01-02-04

- Input: 4×5; A E/3, B W/1. Zeilen `[4, 4, 1, 1, 0]`, Spalten `[2, 2, 2, 4]`.
- Lösung: 10 Zellen, 8 Kurven; Dichte 10/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=44, maxDeductionDepth=6.
- Einstieg: Die oberen beiden Zeilen sind voll, die untere Zeile ist leer. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[15, 29, 35, 40, 41, 42]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'EXCLUSION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der rechte Vierercount und die Einserzeilen 2/3 verbinden das dichte Kopfstück mit dem Ausgang E/3. Anders als Position 3 ist die sichere Fläche horizontal; dieser Achsenwechsel rechtfertigt die ähnliche Belegungsdichte.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐┌┐
01 ┘└┘│
02 ...│
03 ...└
04 ....
```

## S1-01-02-05

- Input: 4×5; A E/4, B N/1. Zeilen `[2, 1, 2, 2, 4]`, Spalten `[5, 4, 1, 1]`.
- Lösung: 11 Zellen, 7 Kurven; Dichte 11/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=48, maxDeductionDepth=6.
- Einstieg: Die untere volle Zeile verknüpft vier sichere Belegungen. Belegschritte: `[14, 15, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[16, 24, 36, 38, 39, 46]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'COMBINATION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die volle linke Spalte kreuzt den unteren Anker, aber der Nordport sitzt daneben. Counts 1 in den rechten Spalten erlauben frühe Restzahlabschlüsse; die zwei linken Rückknicke zeigen erneut, warum grau noch keine Gleisform ist.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘..
01 │...
02 └┐..
03 ┌┘..
04 └───
```

## S1-01-02-06

- Input: 4×5; A E/2, B S/0. Zeilen `[3, 3, 3, 1, 2]`, Spalten `[3, 5, 1, 3]`.
- Lösung: 12 Zellen, 9 Kurven; Dichte 12/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=47, maxDeductionDepth=7.
- Einstieg: Die volle Spalte 1 bildet eine Achse durch das gesamte Feld. Belegschritte: `[14, 15, 16, 17, 18]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 21, 24, 26, 35, 36, 43]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Dreierzeilen oben und ein Einserengpass in Zeile 3 verlagern Ausschlüsse weit weg vom Südanschluss. Der obere Bogen und das untere kurze Knie müssen über denselben vertikalen Anker zusammenpassen; passend für gekreuzte Belegungs- und Ausschlusslogik.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌─┐
01 ┌┘.│
02 └┐.└
03 .│..
04 ┌┘..
```

## S1-01-02-07

- Input: 4×5; A N/2, B E/2. Zeilen `[4, 4, 2, 3, 3]`, Spalten `[5, 3, 4, 4]`.
- Lösung: 16 Zellen, 11 Kurven; Dichte 16/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=51, maxDeductionDepth=7.
- Einstieg: Die oberen beiden vollen Zeilen machen Belegung sicher, nicht die jeweilige Form. Belegschritte: `[14, 15, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 25, 27, 40, 42, 45, 48]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'DENSITY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Das dichte 16-Zellen-Feld lässt unten ein L-förmiges Restgebiet frei; ein langer linker Schenkel und ein kurzer rechter Ausgang tragen unterschiedliche Counts. Die später ähnliche S1-01-04-03 verschiebt den südlichen Rückweg und die Ports; die Paarentscheidung wird im Ähnlichkeitsbericht separat festgehalten.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐└┐
01 │└─┘
02 │..┌
03 │.┌┘
04 └─┘.
```

## S1-01-02-08

- Input: 4×5; A S/0, B W/1. Zeilen `[0, 4, 3, 1, 4]`, Spalten `[2, 3, 3, 4]`.
- Lösung: 12 Zellen, 7 Kurven; Dichte 12/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=53, maxDeductionDepth=7.
- Einstieg: Die obere Nullzeile schließt den Raum über den beiden Endpunktgebieten. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[4, 25, 34, 44, 49, 50, 51]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'EXCLUSION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Zwei volle Querzeilen liegen auseinander und werden nur am rechten Rand verbunden. Der innere Rückknick der oberen Hälfte wird durch Dreiercounts getrennt vom langen unteren Bogen geklärt; diese räumliche Trennung erweitert Position 7 trotz geringerer Dichte.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....
01 ─┐┌┐
02 .└┘│
03 ...│
04 ┌──┘
```

## S1-01-02-09

- Input: 4×5; A E/4, B W/4. Zeilen `[0, 0, 0, 4, 4]`, Spalten `[2, 2, 2, 2]`.
- Lösung: 8 Zellen, 8 Kurven; Dichte 8/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=43, maxDeductionDepth=6.
- Einstieg: Die drei oberen Nullzeilen begrenzen die Aufgabe auf die unteren beiden Zeilen. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 32, 36, 37, 40, 41]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 3 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Bewusster enger Variantenraum für nahe Endpunkte: W/4 und E/4 liegen an derselben Zeile, alle Spalten haben Count 2. Die acht Kurven sind vollständig durch die kurze Verschlingung bestimmt; frühe Wiederholung ist hier zulässig und passt zum nahen Endpunktbrief.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....
01 ....
02 ....
03 ┌┐┌┐
04 ┘└┘└
```

## S1-01-02-10

- Input: 4×5; A S/3, B E/1. Zeilen `[3, 4, 1, 2, 4]`, Spalten `[4, 5, 3, 2]`.
- Lösung: 14 Zellen, 9 Kurven; Dichte 14/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=50, maxDeductionDepth=6.
- Einstieg: Zeile 1 ist voll und enthält den Ausgang E/1. Belegschritte: `[14, 15, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[5, 24, 28, 44, 46, 47]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'DENSITY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: E/1 und S/3 grenzen an Zellen mit Manhattanabstand 3; die Endpunkte bleiben trotz des langen Umwegs nah. Der zweite Vollanker unten erzeugt eine deutlich dichtere Variante als Position 9; die nahe Verbindung darf gerade nicht entlang des rechten Randes abgekürzt werden.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐.
01 └┐└─
02 .│..
03 ┌┘..
04 └──┐
```

## S1-01-02-11

- Input: 4×5; A W/1, B S/1. Zeilen `[2, 3, 3, 3, 2]`, Spalten `[5, 5, 3, 0]`.
- Lösung: 13 Zellen, 11 Kurven; Dichte 13/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=48, maxDeductionDepth=9.
- Einstieg: Die rechte Nullspalte sperrt alle östlichen Fortsetzungen des Dreispaltengebiets. Belegschritte: `[22, 23, 24, 25, 26]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 29, 32, 35, 36, 41, 43, 45, 47]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Zwei volle linke Spalten lassen nur die Kurven und den wechselnden Dreierrest rechts offen. Die verschachtelten kleinen Bögen verlangen einen längeren Anschlussablauf als der weite U-Bogen in Position 10; bei ähnlicher Dichte entsteht eine andere Schlussaufgabe.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐..
01 ┘└┐.
02 ┌┐│.
03 │└┘.
04 └┐..
```

## S1-01-02-12

- Input: 4×5; A S/2, B E/3. Zeilen `[3, 4, 4, 4, 2]`, Spalten `[4, 5, 5, 3]`.
- Lösung: 17 Zellen, 15 Kurven; Dichte 17/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=58, maxDeductionDepth=11.
- Einstieg: Die mittleren drei vollen Zeilen geben eine zusammenhängend belegte Fläche. Belegschritte: `[18, 19, 20, 21]`.
- Eine tatsächliche maximale Prämissenkette: `[9, 28, 30, 31, 39, 50, 51, 52, 53, 54, 57]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'DENSITY']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: 15 Kurven auf 17 Zellen verlangen die genaue Formfestlegung durch die Randcounts 4/5/5/3. Der dichte Abschluss ist klar an mehreren Linien zugänglich, aber der Sprung von Tiefe 6 in Position 10 zu 11 hier ist im Spieltest auf Überlastung zu prüfen; technische Lösbarkeit allein genügt nicht.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐.
01 └┐└┐
02 ┌┘┌┘
03 └┐└─
04 .└┐.
```
