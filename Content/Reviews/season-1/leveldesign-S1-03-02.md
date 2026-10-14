# Author-Leveldesignreview: S1-03-02 (globale Route 10)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Strukturierte Anfangsanker | S1-03-02-01: 6, S1-03-02-02: 8 |
| 3–5 | Zahlenverteilung variieren | S1-03-02-03: 9, S1-03-02-04: 10, S1-03-02-05: 10 |
| 6–8 | Teilräume zusammenführen | S1-03-02-06: 11, S1-03-02-07: 12, S1-03-02-08: 13 |
| 9–10 | Lange asymmetrische Verbindung | S1-03-02-09: 9, S1-03-02-10: 10 |
| 11–12 | Klarer Kompetenzabruf | S1-03-02-11: 19, S1-03-02-12: 19 |

**Individuelle Routenbewertung:** Der nahe gedrehte Serpentinenzwilling in 2 wurde gezielt ersetzt; 1 bleibt als strukturierter Rückgriff erhalten. 3–5 verändern die Bedeutung kleiner gleicher Zahlen durch obere/untere Dichteverlagerung. 6–8 koppeln zwei Gebiete über jeweils andere Einserengpässe. 9/10 sind deutlich unterschiedliche asymmetrische Dichtebilder; geringere aktive Höhe in 9 ist dafür passend. Die Abschlüsse haben beide Tiefe 19, aber unterschiedliche Kurvenlast (7/15) und Startanker. Das trennt sinnvolle fachliche Progression von einem bloßen Zahlenanstieg.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-03-02-01

- Input: 6×6; A W/1, B E/5. Zeilen `[0, 4, 3, 3, 3, 5]`, Spalten `[5, 1, 5, 1, 5, 1]`.
- Lösung: 18 Zellen, 6 Kurven; Dichte 18/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=80, maxDeductionDepth=6.
- Einstieg: Die obere Nullzeile begrenzt drei lange Belegungsarme. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[19, 40, 48, 55, 57, 65]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'OCCUPANCY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Counts 1 in Spalten 1/3/5 bestimmen die drei Querübergänge. Das klare Muster ist als Wiedereinstieg passend; unmittelbar folgende Position 2 wiederholt es allerdings fast gedreht mit nur einer zusätzlichen Zelle, weshalb dort gezielt eine andere Anfangsgeometrie gewählt wird.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ......
01 ┐.┌─┐.
02 │.│.│.
03 │.│.│.
04 │.│.│.
05 └─┘.└─
```

## S1-03-02-02

- Input: 6×6; A E/1, B S/0. Zeilen `[2, 2, 2, 1, 3, 6]`, Spalten `[2, 2, 1, 1, 4, 6]`.
- Lösung: 16 Zellen, 9 Kurven; Dichte 16/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=79, maxDeductionDepth=8.
- Einstieg: Die volle Spalte 5 und volle Unterzeile geben zwei quer zueinander liegende Anfangsanker. Belegschritte: `[24, 35, 36, 37, 38]`.
- Eine tatsächliche maximale Prämissenkette: `[22, 30, 40, 42, 57, 65, 70, 77]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Gezielt ersetzt: ein kleines Kopfstück rechts und ein unterer linker Doppelknick sind über den langen Randarm verbunden. Zwei Einserspalten im Zentrum erlauben nur den Bodenübergang; die 16-Zellen-Lösung verlangt einen anderen Zugang als die drei vertikalen Arme von Position 1 und ist kein gedrehter Zwilling.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....┌┐
01 ....│└
02 ....└┐
03 .....│
04 ┌┐...│
05 │└───┘
```

## S1-03-02-03

- Input: 6×6; A W/5, B E/0. Zeilen `[5, 6, 2, 2, 2, 2]`, Spalten `[6, 6, 1, 2, 2, 2]`.
- Lösung: 19 Zellen, 16 Kurven; Dichte 19/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=89, maxDeductionDepth=9.
- Einstieg: Die volle Zeile 1 kreuzt zwei volle linke Spalten. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[22, 35, 44, 47, 66, 72, 73, 74, 76]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'DENSITY']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Zweierzeilen unten tragen kleine Wechselkurven, oben liegt eine fast volle Querfläche. Gleiche kleine Spaltenwerte rechts haben dadurch eine andere Bedeutung als die unteren Zweierwerte; die Verteilungsänderung ist auf dem tatsächlichen Raster sichtbar.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐.┌┐┌
01 │└─┘└┘
02 └┐....
03 ┌┘....
04 └┐....
05 ─┘....
```

## S1-03-02-04

- Input: 6×6; A S/0, B N/5. Zeilen `[4, 2, 1, 4, 1, 1]`, Spalten `[3, 1, 3, 4, 1, 1]`.
- Lösung: 13 Zellen, 6 Kurven; Dichte 13/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=80, maxDeductionDepth=10.
- Einstieg: A an S/0 und die Einserzeile unten schließen deren übrige Zellen aus. Belegschritte: `[5, 14]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 20, 37, 39, 42, 46, 64, 70, 73, 75]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'EXCLUSION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die Einsercounts in Spalten 1/4/5 erlauben einzelne Querübergänge zwischen oberen und unteren Viererreihen. Kein Extremwert fixiert eine ganze Linie; die wechselnde Lage gleich großer Zeilenwerte führt zu einem versetzten langen Weg statt einem Vollflächenbogen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌──┘
01 ..└┐..
02 ...│..
03 ┌──┘..
04 │.....
05 │.....
```

## S1-03-02-05

- Input: 6×6; A N/1, B W/5. Zeilen `[3, 1, 2, 2, 5, 4]`, Spalten `[2, 2, 3, 6, 4, 0]`.
- Lösung: 17 Zellen, 13 Kurven; Dichte 17/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=81, maxDeductionDepth=10.
- Einstieg: Die volle Spalte 3 liefert einen vertikalen Anker zwischen unterschiedlich dichten Gebieten. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[13, 42, 46, 52, 53, 70, 74, 75, 78, 80]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Nordport links und die rechte Nullspalte schließen zwei Ränder; die unteren Fünfer-/Viererreihen müssen mit den oberen Dreier-/Zweiercounts kombiniert werden. Der dichte Bereich liegt nun unten, die Kurvenserie rechts; klare Verteilungsvariation zu 3.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .└─┐..
01 ...│..
02 ...└┐.
03 ...┌┘.
04 ┌─┐└┐.
05 ┘.└─┘.
```

## S1-03-02-06

- Input: 6×6; A N/1, B E/2. Zeilen `[1, 2, 3, 3, 4, 0]`, Spalten `[0, 2, 4, 1, 3, 3]`.
- Lösung: 13 Zellen, 7 Kurven; Dichte 13/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=86, maxDeductionDepth=11.
- Einstieg: A an N/1 belegt die einzige Zelle der oberen Zeile. Belegschritte: `[1, 9]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 20, 39, 40, 53, 58, 75, 78, 79, 82, 85]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Linke und untere Nullachse begrenzen fünf aktive Zeilen/Spalten. Der Einsercount von Spalte 3 koppelt den linken Nordarm mit dem rechten Ostgebiet nur im unteren Querweg; genau dieser Übergang führt zwei Teilräume zusammen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .│....
01 .└┐...
02 ..│.┌─
03 ..│.└┐
04 ..└──┘
05 ......
```

## S1-03-02-07

- Input: 6×6; A S/2, B W/2. Zeilen `[4, 5, 3, 1, 3, 4]`, Spalten `[2, 1, 3, 3, 5, 6]`.
- Lösung: 20 Zellen, 15 Kurven; Dichte 20/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=90, maxDeductionDepth=12.
- Einstieg: Spalte 5 ist vollständig belegt und verbindet obere und untere Hinweise. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 34, 36, 57, 67, 68, 76, 77, 78, 80, 82, 83]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Westport liegt links des oberen Umwegs; der Südport muss über eine separate untere Doppelkurve erreicht werden. Count 1 in Spalte 1 und die mittlere Einserzeile erzwingen unterschiedliche Engpässe auf beiden Seiten des langen rechten Schenkels.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌──┐
01 ┌─┘.┌┘
02 ┘...└┐
03 .....│
04 ...┌┐│
05 ..┌┘└┘
```

## S1-03-02-08

- Input: 6×6; A E/5, B W/1. Zeilen `[0, 1, 1, 2, 5, 5]`, Spalten `[5, 3, 1, 2, 1, 2]`.
- Lösung: 14 Zellen, 10 Kurven; Dichte 14/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=86, maxDeductionDepth=13.
- Einstieg: Die obere Nullzeile begrenzt die Fläche, der Westport darunter belegt eine Einserzeile. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 26, 37, 49, 58, 65, 72, 73, 74, 77, 78, 81, 83]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der lange linke Schenkel und der breite untere Ostbogen verbinden sich über den mittleren Einsercount in Spalte 2. Gegenüber S1-03-01-04 liegt der schmale Eingang links oben und der dichte Bereich unten rechts; gleiche Belegungsnähe verdeckt diese andere Zugangskette nicht.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ......
01 ┐.....
02 │.....
03 └┐....
04 ┌┘.┌─┐
05 └──┘.└
```

## S1-03-02-09

- Input: 6×6; A E/2, B W/2. Zeilen `[6, 6, 5, 5, 0, 0]`, Spalten `[4, 4, 2, 4, 4, 4]`.
- Lösung: 22 Zellen, 18 Kurven; Dichte 22/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=84, maxDeductionDepth=9.
- Einstieg: Die oberen zwei Vollzeilen und die beiden unteren Nullzeilen sind sichere Flächenanker. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 44, 66, 67, 72, 73, 78, 79, 80]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: 22 Zellen liegen asymmetrisch nur in vier oberen Reihen; die Ports E/2 und W/2 liegen im dichten Inneren. Die frei bleibenden Zellen in Spalte 2 teilen zwei Kurvengebiete; die reduzierte Höhe ist Teil des verlangten Dichtewechsels.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐┌─┐
01 └┐└┘┌┘
02 ┐│.┌┘┌
03 └┘.└─┘
04 ......
05 ......
```

## S1-03-02-10

- Input: 6×6; A S/4, B W/0. Zeilen `[3, 3, 1, 1, 2, 4]`, Spalten `[2, 4, 6, 1, 1, 0]`.
- Lösung: 14 Zellen, 9 Kurven; Dichte 14/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=80, maxDeductionDepth=10.
- Einstieg: Die volle Spalte 2 koppelt den kompakten Kopf an den unteren Bogen. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[18, 30, 42, 43, 54, 57, 60, 70, 71, 73]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die Einserzeilen 2/3 erzeugen eine lange schmale Mitte, während Kopf und Boden verschiedene Querformen tragen. Das Feld hat 14 statt 22 Zellen und keine lange horizontale Serpentine; so liefert die Variante ein klar anderes Dichtebild trotz geringer aktiver Randfläche.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐┌┐...
01 └┘│...
02 ..│...
03 ..│...
04 .┌┘...
05 .└──┐.
```

## S1-03-02-11

- Input: 6×6; A N/0, B E/2. Zeilen `[3, 4, 3, 2, 4, 0]`, Spalten `[5, 1, 1, 4, 2, 3]`.
- Lösung: 16 Zellen, 7 Kurven; Dichte 16/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=87, maxDeductionDepth=19.
- Einstieg: Die untere Nullzeile begrenzt die beiden langen vertikalen Arme. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 29, 31, 40, 49, 50, 54, 59, 60, 64, 68, 69, 71, 72, 73, 79, 81, 83, 84]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Zwei mittlere Einsercounts verbinden sie nur unten; der rechte Kopf muss außerdem den Ostport aufnehmen. Tiefe 19 entsteht aus diesem seriellen U-Umweg mit kleiner Kopfdifferenzierung, dessen übersichtlicher Umriss einen klaren Kompetenzabruf plausibel macht.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 │...┌┐
01 │..┌┘│
02 │..│.└
03 │..│..
04 └──┘..
05 ......
```

## S1-03-02-12

- Input: 6×6; A W/5, B N/1. Zeilen `[6, 4, 2, 2, 3, 4]`, Spalten `[3, 4, 4, 4, 3, 3]`.
- Lösung: 21 Zellen, 15 Kurven; Dichte 21/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=90, maxDeductionDepth=19.
- Einstieg: Die volle obere Zeile gibt einen vertrauten sicheren Anfang. Belegschritte: `[20, 21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 29, 33, 36, 37, 44, 48, 51, 52, 55, 59, 62, 68, 72, 73, 75, 85, 86, 88]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Westanschluss unten ist durch mehrere Kurven von der Nordspalte getrennt. Gleichartige mittlere Spaltenwerte müssen die versetzte Treppe rechts und die kurze linke Kurve tragen; gegenüber 11 steigt die Formlast bei derselben maximalen Root-Tiefe.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘┌──┐
01 └─┘..│
02 ....┌┘
03 ...┌┘.
04 .┌┐│..
05 ─┘└┘..
```
