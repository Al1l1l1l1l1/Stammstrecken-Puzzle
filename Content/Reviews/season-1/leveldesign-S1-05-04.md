# Author-Leveldesignreview: S1-05-04 (globale Route 20)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Souveräner Wiedereinstieg | S1-05-04-01: 7, S1-05-04-02: 10 |
| 3–5 | Komplexe Belegung | S1-05-04-03: 12, S1-05-04-04: 13, S1-05-04-05: 11 |
| 6–8 | Mehrere tiefe 10×10-Ketten | S1-05-04-06: 23, S1-05-04-07: 23, S1-05-04-08: 27 |
| 9–10 | Finale räumliche Variation | S1-05-04-09: 15, S1-05-04-10: 17 |
| 11–12 | Finale 10×10-Meldungen | S1-05-04-11: 42, S1-05-04-12: 40 |

**Individuelle Routenbewertung:** Der reduzierte Rückgriff 1 ist nach seinem eigenen Brief zulässig, der nahezu gleiche zweite Serpentinenstart wurde ersetzt. 3–5 kombinieren unterschiedlich lange Arme und Knickgruppen im 9×9-Feld. 6–8 wechseln planmäßig auf 10×10; ihre tiefen realen Folgen verbinden Querbänder, vertikale Arme oder Kopf-/Mittelkurven. 9 bietet ruhige Gebietsplanung, 10 einen unmittelbaren Vollspaltenzugang mit dichter Bodenform. Die beiden dünnen Finales 22/100 und 24/100 decken alle Achsen ab, haben jedoch besonders lange serielle Folgen (42/40); dieser konkrete Abschlussrisikopunkt wird extern gespielt, weder automatisch bestanden noch nach einer erfundenen Tiefegrenze ersetzt.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-05-04-01

- Input: 9×9; A W/3, B S/2. Zeilen `[0, 0, 0, 1, 1, 1, 1, 4, 4]`, Spalten `[6, 2, 2, 2, 0, 0, 0, 0, 0]`.
- Lösung: 12 Zellen, 7 Kurven; Dichte 12/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=131, maxDeductionDepth=7.
- Einstieg: Die oberen drei Nullzeilen und fünf rechten Nullspalten liefern einen klaren letzten Wiedereinstieg. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 69, 114, 123, 124, 125, 126]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'OCCUPANCY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Vier Einserzeilen sichern einen linken Schenkel, die zwei Viererreihen unten einen kurzen Doppelbogen. Der Brief erlaubt hier souveräne Reaktivierung; die starke Vereinfachung ist daher anders als in S1-05-03-01 zulässig und bewusst kurz vor der finalen Laststeigerung platziert.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .........
01 .........
02 .........
03 ┐........
04 │........
05 │........
06 │........
07 │┌─┐.....
08 └┘┌┘.....
```

## S1-05-04-02

- Input: 9×9; A E/1, B S/2. Zeilen `[5, 5, 1, 1, 1, 2, 2, 2, 5]`, Spalten `[0, 0, 1, 1, 3, 9, 6, 2, 2]`.
- Lösung: 24 Zellen, 17 Kurven; Dichte 24/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=163, maxDeductionDepth=10.
- Einstieg: Die volle Spalte 5 und drei mittlere Einserzeilen sichern einen bekannten senkrechten Anker. Belegschritte: `[58, 59, 60, 61, 62, 63, 64, 65, 66]`.
- Eine tatsächliche maximale Prämissenkette: `[25, 57, 75, 76, 93, 104, 139, 140, 153, 154]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'COMBINATION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Gezielt ersetzt durch 24 Zellen und 17 Kurven: ein doppelter Kopfknick, zwei mittlere rechte Wechselkurven und ein langer Bodenarm zum Südport. Zwei linke Nullspalten erleichtern den souveränen Wiederbeginn; die übrigen sieben Spalten bleiben unterschiedlich beteiligt. Diese Raumstruktur vermeidet den fast gleichen 42-Zellen-Serpentinenstart der Vorroute.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....┌─┐┌┐
01 ....└┐└┘└
02 .....│...
03 .....│...
04 .....│...
05 .....└┐..
06 .....┌┘..
07 .....└┐..
08 ..┌───┘..
```

## S1-05-04-03

- Input: 9×9; A W/0, B E/7. Zeilen `[4, 3, 6, 5, 5, 5, 5, 7, 0]`, Spalten `[8, 1, 8, 1, 8, 1, 6, 1, 6]`.
- Lösung: 40 Zellen, 10 Kurven; Dichte 40/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=180, maxDeductionDepth=12.
- Einstieg: Die untere Nullzeile begrenzt fünf vertikale Arme mit unterschiedlichen Kopf- und Bodenhöhen. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 47, 65, 80, 87, 88, 89, 116, 117, 125, 152, 162]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'DENSITY']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei Achterspalten sind innerhalb des aktiven Gebietes voll, die übrigen mittleren Werte verteilen den rechten Rückweg. Die Nähe zu S1-05-03-03 und -08 ist als Übertragung von deren ungleicher Belegung in einen komplexeren Quer-/Restzusammenhang dokumentiert; behalten, statt einen beliebigen Ähnlichkeitswert zum Ausschluss zu machen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.┌─┐....
01 │.│.│....
02 │.│.│.┌─┐
03 │.│.│.│.│
04 │.│.│.│.│
05 │.│.│.│.│
06 │.│.│.│.│
07 └─┘.└─┘.└
08 .........
```

## S1-05-04-04

- Input: 9×9; A E/2, B W/6. Zeilen `[8, 4, 3, 1, 1, 3, 3, 2, 0]`, Spalten `[8, 6, 3, 1, 1, 1, 1, 2, 2]`.
- Lösung: 25 Zellen, 16 Kurven; Dichte 25/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=180, maxDeductionDepth=13.
- Einstieg: A an E/2 verbraucht einen der zwei Counts der rechten Spalte. Belegschritte: `[12, 19]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 44, 64, 88, 123, 131, 133, 140, 144, 147, 153, 158, 178]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der obere Achterarm verbindet zwei Randgebiete, die linken Sechser-/Achtercounts tragen zwei räumlich getrennte Rückknicke. Vier Einserspalten beschränken den Querübergang im Kopf; darunter bestimmen die kleinen Zeilenreste die Kurven statt einer ganzen Vollfläche.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──────┐.
01 └┐.....└┐
02 ┌┘......└
03 │........
04 │........
05 └─┐......
06 ┐┌┘......
07 └┘.......
08 .........
```

## S1-05-04-05

- Input: 9×9; A N/2, B S/5. Zeilen `[7, 2, 2, 2, 1, 2, 3, 4, 1]`, Spalten `[0, 0, 1, 1, 1, 3, 3, 8, 7]`.
- Lösung: 24 Zellen, 16 Kurven; Dichte 24/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=156, maxDeductionDepth=11.
- Einstieg: B an S/5 belegt allein die unterste Zeile. Belegschritte: `[2, 28]`.
- Eine tatsächliche maximale Prämissenkette: `[28, 36, 84, 85, 107, 123, 150, 151, 152, 153, 155]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'EXCLUSION']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die zwei linken Nullspalten und drei Einsercounts begrenzen den langen Kopfarm. Rechts müssen zwei fast volle Spalten eine Wechselkurve und einen zweiten unteren Doppelknick aufnehmen; die mittleren Dreiercounts verknüpfen sie, wodurch Belegung und Form mehrmals ineinandergreifen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└─────┐
01 .......┌┘
02 .......└┐
03 .......┌┘
04 .......│.
05 .......└┐
06 ......┌┐│
07 .....┌┘└┘
08 .....│...
```

## S1-05-04-06

- Input: 10×10; A N/2, B S/6. Zeilen `[8, 1, 8, 1, 6, 1, 6, 1, 5, 1]`, Spalten `[0, 0, 7, 5, 5, 5, 6, 5, 2, 3]`.
- Lösung: 38 Zellen, 10 Kurven; Dichte 38/100. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=202, maxDeductionDepth=23.
- Einstieg: B an S/6 belegt allein die unterste Zeile des neuen 10×10-Feldes. Belegschritte: `[2, 32]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 85, 95, 104, 106, 108, 110, 112, 114, 137, 139, 152, 153, 156, 158, 160, 162, 164, 166, 170, 171, 185, 197]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 9 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Zwei Nullspalten grenzen die Fläche ein, mehrere breite Querbänder werden nach unten kürzer. Die fünf mittleren Spaltencounts und Einserübergänge verzahnen die getrennten Bandfolgen; die Folge bis Tiefe 23 ist auf echter Rootspur gebunden, ihre menschliche Last bleibt offen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└──────┐
01 .........│
02 ..┌──────┘
03 ..│.......
04 ..└────┐..
05 .......│..
06 ..┌────┘..
07 ..│.......
08 ..└───┐...
09 ......│...
```

## S1-05-04-07

- Input: 10×10; A W/1, B E/7. Zeilen `[3, 3, 3, 6, 5, 5, 7, 2, 0, 0]`, Spalten `[6, 1, 7, 1, 7, 1, 4, 1, 5, 1]`.
- Lösung: 34 Zellen, 10 Kurven; Dichte 34/100. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=222, maxDeductionDepth=23.
- Einstieg: Die beiden unteren Nullzeilen begrenzen fünf vertikale Teilräume. Belegschritte: `[36, 37, 38, 39, 40, 41, 42, 43, 44, 45]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 62, 79, 80, 83, 90, 118, 124, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 153, 166, 190, 195, 204]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 9 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die Armhöhen sind unterschiedlich; fünf Einserspalten erlauben nur Querübergänge, der Ostport sitzt vor dem leeren Boden. Die getrennten mittleren Restzahlfolgen müssen dieselbe lange Strecke ergeben; gegenüber 6 wechselt das 10×10-Muster seine Achse und sein Endpunktgebiet.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌─┐.....
01 ┐.│.│.....
02 │.│.│.....
03 │.│.│.┌─┐.
04 │.│.│.│.│.
05 │.│.│.│.│.
06 └─┘.└─┘.│.
07 ........└─
08 ..........
09 ..........
```

## S1-05-04-08

- Input: 10×10; A S/0, B N/6. Zeilen `[6, 6, 2, 2, 2, 2, 2, 4, 1, 1]`, Spalten `[3, 1, 8, 8, 2, 2, 2, 2, 0, 0]`.
- Lösung: 28 Zellen, 22 Kurven; Dichte 28/100. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=210, maxDeductionDepth=27.
- Einstieg: Die beiden rechten Nullspalten begrenzen eine kurvenreiche Kopfbelegung. Belegschritte: `[45, 46, 47, 48, 49, 50, 51, 52, 53]`.
- Eine tatsächliche maximale Prämissenkette: `[26, 36, 78, 79, 81, 93, 141, 166, 167, 168, 169, 170, 172, 173, 174, 176, 178, 179, 180, 182, 184, 185, 186, 188, 205, 206, 208]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 10 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die beiden Achterspalten 2/3 tragen einen langen mittleren Wechselzug, der oben zu einem separaten Doppelbogen und unten zum Südport links führen muss. 22 Kurven statt zehn in 6/7 erhöhen die Formlast; die zwei Kopfsechserreihen hängen an anderen Spaltenresten als die mittleren Zweierzeilen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌┐┌┐└┐..
01 ..│└┘└─┘..
02 ..└┐......
03 ..┌┘......
04 ..└┐......
05 ..┌┘......
06 ..└┐......
07 ┌──┘......
08 │.........
09 │.........
```

## S1-05-04-09

- Input: 10×10; A S/3, B W/4. Zeilen `[0, 0, 0, 3, 7, 7, 3, 1, 1, 5]`, Spalten `[1, 1, 1, 3, 2, 2, 3, 7, 3, 4]`.
- Lösung: 27 Zellen, 13 Kurven; Dichte 27/100. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=186, maxDeductionDepth=15.
- Einstieg: Die drei oberen Nullzeilen erlauben ruhige Orientierung im unteren Gebiet. Belegschritte: `[36, 37, 38, 39, 40, 41, 42, 43, 44, 45]`.
- Eine tatsächliche maximale Prämissenkette: `[16, 66, 88, 89, 114, 128, 129, 130, 131, 167, 173, 174, 180, 183, 184]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Westport links und Südport innen sind durch einen zentralen Querarm und rechten Doppelbogen verbunden. Drei Einsercounts links legen die lange Zufahrt fest, zwei weitere Einserzeilen den rechten Schenkel; diese finale Variante verteilt Planung in klar getrennte Flächen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..........
01 ..........
02 ..........
03 .......┌─┐
04 ───┐..┌┘.│
05 ...└──┘┌┐│
06 .......│└┘
07 .......│..
08 .......│..
09 ...┌───┘..
```

## S1-05-04-10

- Input: 10×10; A W/0, B S/9. Zeilen `[1, 1, 1, 1, 1, 1, 2, 5, 7, 9]`, Spalten `[10, 4, 2, 2, 2, 3, 1, 3, 1, 1]`.
- Lösung: 29 Zellen, 19 Kurven; Dichte 29/100. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=199, maxDeductionDepth=17.
- Einstieg: Die volle linke Spalte und sechs obere Einserzeilen sichern einen langen geraden Einstieg. Belegschritte: `[45, 46, 47, 48, 49, 50, 51, 52, 53]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 36, 64, 123, 146, 157, 166, 167, 168, 172, 174, 175, 188, 189, 193, 194, 197]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der eigentliche Formenabruf liegt in den drei dichten Bodenreihen mit 19 Kurven. Gegenüber 9 ist der erste Zugriff unmittelbar, die Abschlussgeometrie dichter; das entspricht ausdrücklich zwei verschiedenen finalen räumlichen Variationen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.........
01 │.........
02 │.........
03 │.........
04 │.........
05 │.........
06 └┐........
07 ┌┘...┌─┐..
08 │┌┐┌┐│.│..
09 └┘└┘└┘.└─┐
```

## S1-05-04-11

- Input: 10×10; A S/0, B E/1. Zeilen `[2, 2, 2, 3, 1, 3, 1, 4, 3, 1]`, Spalten `[2, 1, 2, 1, 1, 3, 1, 3, 4, 4]`.
- Lösung: 22 Zellen, 13 Kurven; Dichte 22/100. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=231, maxDeductionDepth=42.
- Einstieg: A an S/0 belegt allein die Einserzeile unten. Belegschritte: `[11, 26]`.
- Eine tatsächliche maximale Prämissenkette: `[26, 36, 54, 55, 63, 72, 73, 81, 90, 91, 92, 106, 107, 114, 122, 123, 130, 138, 139, 146, 154, 155, 156, 164, 165, 166, 174, 175, 180, 186, 187, 192, 198, 199, 203, 207, 208, 210, 217, 221, 223, 229]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 12 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Mehrere Einserspalten setzen die Querübergänge einer langen diagonalen Folge, die oben noch einen rechten Doppelknick aufnehmen muss. 22 Zellen sind visuell dünn, aber alle zehn Achsen sind beteiligt; Tiefe 42 entsteht vor allem durch serielle Form-/Restzahlfolgen. Ob das ein fairer menschlicher Höhepunkt ist, bleibt konkret dem Spieltest vorbehalten.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ........┌┐
01 ........│└
02 ........└┐
03 .......┌─┘
04 .......│..
05 .....┌─┘..
06 .....│....
07 ..┌──┘....
08 ┌─┘.......
09 │.........
```

## S1-05-04-12

- Input: 10×10; A E/0, B S/1. Zeilen `[1, 3, 1, 1, 1, 1, 1, 5, 5, 5]`, Spalten `[2, 2, 2, 3, 3, 1, 1, 7, 1, 2]`.
- Lösung: 24 Zellen, 13 Kurven; Dichte 24/100. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=235, maxDeductionDepth=40.
- Einstieg: A an E/0 belegt die Einserzeile oben. Belegschritte: `[9, 27]`.
- Eine tatsächliche maximale Prämissenkette: `[9, 44, 45, 55, 56, 64, 73, 74, 82, 91, 98, 99, 107, 114, 115, 123, 130, 131, 139, 146, 147, 155, 162, 163, 171, 178, 185, 193, 194, 196, 199, 200, 202, 205, 206, 210, 224, 225, 230, 231]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 12 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Fünf mittlere Einserzeilen sichern einen langen rechten Strang, die drei Fünferreihen unten verlangen dagegen verschachtelte Kurven bis S/1. Der finale 24-Zellen-Weg besitzt volle Achsenabdeckung und eine andere Lastverteilung als 11; Tiefe 40 beweist Reproduzierbarkeit der Folge, keine Ausgestaltung oder Season-Freigabe.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .........┌
01 .......┌─┘
02 .......│..
03 .......│..
04 .......│..
05 .......│..
06 .......│..
07 ...┌───┘..
08 ┌─┐└┐.....
09 └┐└─┘.....
```
