# Author-Leveldesignreview: S1-04-03 (globale Route 15)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Bekannte Muster im größeren Feld | S1-04-03-01: 6, S1-04-03-02: 7 |
| 3–5 | Sichere Ausschlüsse | S1-04-03-03: 10, S1-04-03-04: 10, S1-04-03-05: 11 |
| 6–8 | Parallele Teilketten | S1-04-03-06: 12, S1-04-03-07: 14, S1-04-03-08: 16 |
| 9–10 | Geometrie aus Belegung | S1-04-03-09: 12, S1-04-03-10: 14 |
| 11–12 | Anspruchsvolle 8×8-Puzzles | S1-04-03-11: 34, S1-04-03-12: 33 |

**Individuelle Routenbewertung:** Die bekannten Randmuster in 1/2 entlasten vor den Ausschlüssen der Kopf-/Bodenfelder 3–5. Der 8×8-Wechsel in 6 ist mit einer Nullzeile und einem klaren rechten Arm zugänglich. 7 aktiviert entfernte Vollspalten, der beinahe gedrehte Zwilling 8 wurde gezielt ersetzt. 9/10 trennen volle Belegung und erst danach erzwungene Geometrie auf andere Weise. Die dünnen Abschlüsse ohne Vollanker erreichen hohe technische Tiefe; ihre sichtbare Einfachheit ersetzt keine Prüfung der menschlichen Schlusslast. Nähe von 2 zur Vorroute und von 7 zu späteren Wiedereinstiegen wird ausdrücklich als Rückgriff dokumentiert.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-04-03-01

- Input: 7×7; A N/5, B E/5. Zeilen `[2, 1, 1, 1, 2, 2, 2]`, Spalten `[0, 0, 0, 0, 0, 4, 7]`.
- Lösung: 11 Zellen, 7 Kurven; Dichte 11/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=85, maxDeductionDepth=6.
- Einstieg: Die volle rechte Spalte und die fünf linken Nullspalten begrenzen den ruhigen Einstieg. Belegschritte: `[59, 60, 61, 62, 63, 64]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 72, 74, 77, 78, 84]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'EXCLUSION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei mittlere Einserzeilen sichern den geraden Schenkel, oben und unten liegen verschiedene Rückknicke. Ein bewusst sehr schmaler Rückgriff auf bekannte Muster vor dem späteren 8×8-Wechsel; die Randlage reduziert Orientierungslast, nicht die Anzahl neuer Regeln.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .....└┐
01 ......│
02 ......│
03 ......│
04 .....┌┘
05 .....│┌
06 .....└┘
```

## S1-04-03-02

- Input: 7×7; A W/0, B E/0. Zeilen `[5, 4, 4, 4, 5, 2, 3]`, Spalten `[7, 1, 7, 1, 5, 1, 5]`.
- Lösung: 27 Zellen, 8 Kurven; Dichte 27/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=107, maxDeductionDepth=7.
- Einstieg: Zwei volle linke Spalten liefern bekannte Belegung. Belegschritte: `[24, 25, 26, 27, 28, 29]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 38, 48, 63, 64, 84, 93]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'COMBINATION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Gegenüber S1-04-02-02 endet der rechte Umweg eine Zeile früher, die beiden Ports bleiben gleich. Diese nahe Wiederholung aktiviert dasselbe Muster unter anderen Restcounts; nur hier als Wiedereinstieg behalten, während spätere direkte Serpentinenzwillinge innerhalb der Verdichtung geändert werden.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.┌─┐.┌
01 │.│.│.│
02 │.│.│.│
03 │.│.│.│
04 │.│.└─┘
05 │.│....
06 └─┘....
```

## S1-04-03-03

- Input: 7×7; A S/1, B N/1. Zeilen `[5, 5, 2, 2, 1, 3, 1]`, Spalten `[2, 4, 5, 6, 2, 0, 0]`.
- Lösung: 19 Zellen, 14 Kurven; Dichte 19/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=97, maxDeductionDepth=10.
- Einstieg: A an S/1 belegt die Einserzeile unten. Belegschritte: `[1, 18]`.
- Eine tatsächliche maximale Prämissenkette: `[18, 24, 57, 58, 73, 85, 86, 87, 90, 92]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'CHAIN']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die zwei rechten Nullspalten und zwei dichten Kopfzeilen erlauben sichere frühe Ausschlüsse. Der mittlere Rückknick muss über Spaltenreste geklärt werden, bevor der untere Querarm feststeht; geometrisch direkter N/1–S/1-Weg ist mit den hohen mittleren Counts unvereinbar.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘┌─┐..
01 └─┘┌┘..
02 ..┌┘...
03 ..└┐...
04 ...│...
05 .┌─┘...
06 .│.....
```

## S1-04-03-04

- Input: 7×7; A N/1, B S/1. Zeilen `[6, 1, 6, 1, 6, 1, 6]`, Spalten `[0, 5, 4, 4, 4, 4, 6]`.
- Lösung: 27 Zellen, 8 Kurven; Dichte 27/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=103, maxDeductionDepth=10.
- Einstieg: Die linke Nullspalte begrenzt vier Sechserbänder auf volle aktive Reihen. Belegschritte: `[24, 25, 26, 27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 38, 60, 72, 96, 97, 98, 99, 100, 101]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'COMBINATION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Einserzeilen zwingen abwechselnd weit entfernte Übergänge; beide Ports in Spalte 1 könnten sonst eine direkte Verbindung nahelegen. Der Ausschluss der linken Kante wirkt über die gesamte Höhe auf die Bandbreite; dieses bekannte Motiv hat hier einen konkret anderen Informationsanker als die frühen öffentlichen Vollbänder.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .└────┐
01 ......│
02 .┌────┘
03 .│.....
04 .└────┐
05 ......│
06 .┌────┘
```

## S1-04-03-05

- Input: 7×7; A E/6, B W/2. Zeilen `[0, 0, 2, 2, 5, 4, 6]`, Spalten `[5, 5, 1, 1, 3, 1, 3]`.
- Lösung: 19 Zellen, 12 Kurven; Dichte 19/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=107, maxDeductionDepth=11.
- Einstieg: Die oberen beiden Nullzeilen begrenzen den Raum. Belegschritte: `[24, 25, 26, 27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 42, 56, 71, 79, 80, 81, 83, 103, 104, 105]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'CHAIN']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Westport links beginnt eine kurze Wechselkurve, der Ostport unten einen langen rechten Arm. Drei Einserspalten legen die erst spätere Verbindung am Boden fest; zwei getrennte Teilgebiete werden durch Ausschlüsse statt eine volle Linie gekoppelt.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .......
01 .......
02 ─┐.....
03 ┌┘.....
04 └┐..┌─┐
05 ┌┘..│.│
06 └───┘.└
```

## S1-04-03-06

- Input: 8×8; A W/0, B E/6. Zeilen `[4, 7, 5, 1, 1, 1, 1, 0]`, Spalten `[2, 2, 2, 2, 2, 2, 1, 7]`.
- Lösung: 20 Zellen, 14 Kurven; Dichte 20/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=131, maxDeductionDepth=12.
- Einstieg: Die untere Nullzeile begrenzt beim Wechsel auf 8×8 die rechte Verbindung. Belegschritte: `[35, 36, 37, 38, 39, 40, 41, 42]`.
- Eine tatsächliche maximale Prämissenkette: `[27, 43, 49, 81, 83, 96, 97, 108, 124, 125, 128, 129]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Vier untere Einserzeilen legen einen langen rechten Schenkel fest, oben verteilen sechs Zweiercounts die Kurvenserie. Der Einsercount in Spalte 6 fixiert nur einen Querübergang; obere und rechte Folgen beginnen getrennt und müssen zusammenpassen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐....┌─┐
01 └┐┌┐┌┘.│
02 .└┘└┘..│
03 .......│
04 .......│
05 .......│
06 .......└
07 ........
```

## S1-04-03-07

- Input: 8×8; A W/0, B E/1. Zeilen `[4, 5, 4, 4, 4, 4, 4, 6]`, Spalten `[8, 1, 8, 1, 8, 1, 7, 1]`.
- Lösung: 35 Zellen, 8 Kurven; Dichte 35/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=150, maxDeductionDepth=14.
- Einstieg: Drei volle Spalten 0/2/4 geben voneinander entfernte sichere Arme. Belegschritte: `[28, 29, 30, 31, 32, 33, 34]`.
- Eine tatsächliche maximale Prämissenkette: `[9, 51, 59, 68, 70, 80, 81, 103, 104, 105, 106, 107, 120, 123]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die drei Einserspalten dazwischen koppeln ihre Querübergänge, der rechte Siebenerarm hängt am Ostport in Zeile 1. Bekannte Armgeometrie wird auf 8×8 übertragen; die nachfolgende fast gedrehte 35-Zellen-Variante 8 wurde ersetzt, damit diese Verdichtung nicht zweimal denselben Lösungsrhythmus verlangt.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.┌─┐...
01 │.│.│.┌─
02 │.│.│.│.
03 │.│.│.│.
04 │.│.│.│.
05 │.│.│.│.
06 │.│.│.│.
07 └─┘.└─┘.
```

## S1-04-03-08

- Input: 8×8; A E/6, B W/3. Zeilen `[5, 5, 4, 5, 1, 1, 3, 3]`, Spalten `[1, 1, 2, 3, 2, 6, 8, 4]`.
- Lösung: 27 Zellen, 20 Kurven; Dichte 27/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=148, maxDeductionDepth=16.
- Einstieg: Die volle Spalte 6 und die Einserzeilen 4/5 sichern den rechten Schenkel. Belegschritte: `[35, 36, 37, 38, 39, 40, 41, 42]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 30, 46, 47, 66, 78, 79, 94, 103, 115, 118, 123, 125, 140, 144, 145]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Gezielt ersetzt durch 27 Zellen und 20 Kurven. Links liegt der Westport an einer Treppe, rechts der Ostport über einem unteren Rückbogen; oben bilden zwei verschiedene Fünferreihen weitere Teilketten. Die Sechserreste in Spalte 5 und Viererreste in Spalte 7 müssen mit diesen getrennten Ansätzen zusammenpassen; deutlich andere Verdichtung als Position 7.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...┌┐┌─┐
01 ...│└┘┌┘
02 ..┌┘.┌┘.
03 ──┘..└┐.
04 ......│.
05 ......│.
06 .....┌┘┌
07 .....└─┘
```

## S1-04-03-09

- Input: 8×8; A E/4, B W/6. Zeilen `[0, 0, 0, 8, 4, 3, 3, 2]`, Spalten `[5, 4, 4, 2, 1, 1, 1, 2]`.
- Lösung: 20 Zellen, 14 Kurven; Dichte 20/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=128, maxDeductionDepth=12.
- Einstieg: Die volle Zeile 3 macht ihre Belegung sicher, die drei oberen Nullzeilen schließen den Kopf aus. Belegschritte: `[52, 53, 54, 55, 56, 57, 58, 59]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 62, 93, 100, 107, 108, 110, 112, 120, 124, 125, 127]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'COMBINATION']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die restlichen unteren Counts verteilen einen linken Doppelbogen, während der Ostport rechts an einem langen Kopfarm sitzt. Die Form dieser vollen Belegung ist daher nicht vorweggenommen; die schmalere aktive Höhe gehört zur vorgesehenen räumlichen Variation.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ........
01 ........
02 ........
03 ┌─┐┌───┐
04 │.└┘...└
05 └─┐.....
06 ┐┌┘.....
07 └┘......
```

## S1-04-03-10

- Input: 8×8; A E/1, B S/3. Zeilen `[0, 1, 2, 2, 2, 3, 5, 6]`, Spalten `[0, 0, 2, 2, 4, 4, 5, 4]`.
- Lösung: 21 Zellen, 17 Kurven; Dichte 21/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=135, maxDeductionDepth=14.
- Einstieg: A an E/1 belegt allein diese Einserzeile. Belegschritte: `[9, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[9, 42, 55, 61, 86, 97, 99, 103, 120, 121, 123, 124, 128, 130]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'COMBINATION']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Eine diagonale Folge führt in zwei dichte Bodenbögen, die über mittlere Spaltencounts verbunden werden. Die oberen Ausschlüsse sichern zunächst Belegung, die konkreten Kurven folgen erst aus dem Anschluss an die unteren Sechser-/Fünferreihen; andere Geometrie als 9.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ........
01 .......┌
02 ......┌┘
03 .....┌┘.
04 ....┌┘..
05 ....└─┐.
06 ..┌─┐.└┐
07 ..└┐└──┘
```

## S1-04-03-11

- Input: 8×8; A N/5, B S/0. Zeilen `[3, 3, 1, 2, 2, 1, 6, 1]`, Spalten `[2, 1, 1, 1, 1, 5, 4, 4]`.
- Lösung: 19 Zellen, 10 Kurven; Dichte 19/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=155, maxDeductionDepth=34.
- Einstieg: B an S/0 belegt die Einserzeile unten allein. Belegschritte: `[5, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[20, 28, 42, 43, 49, 56, 57, 63, 70, 71, 77, 84, 85, 91, 98, 99, 105, 112, 113, 119, 125, 126, 127, 131, 132, 133, 135, 141, 142, 143, 144, 146, 147, 151]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'EXCLUSION']`; erwartete gedankliche Folge 10 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Vier Einserspalten fixieren den langen Querweg in Zeile 6, während die rechte obere Doppelkurve erst aus den Dreier-/Vierercounts folgt. 19 Zellen durch alle Achsen sind leicht zu orientieren, aber die lange Folge bis zum rechten Kopf bleibt ein gezieltes Risiko für den Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .....│┌┐
01 .....└┘│
02 .......│
03 ......┌┘
04 .....┌┘.
05 .....│..
06 ┌────┘..
07 │.......
```

## S1-04-03-12

- Input: 8×8; A E/0, B S/0. Zeilen `[2, 1, 2, 3, 1, 2, 3, 3]`, Spalten `[2, 2, 3, 3, 1, 2, 3, 1]`.
- Lösung: 17 Zellen, 11 Kurven; Dichte 17/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=152, maxDeductionDepth=33.
- Einstieg: A an E/0 verbraucht Count 1 in Spalte 7. Belegschritte: `[7, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 28, 35, 43, 49, 50, 57, 63, 64, 71, 72, 77, 83, 88, 89, 95, 96, 100, 105, 106, 110, 115, 118, 119, 123, 126, 130, 134, 135, 137, 147, 148, 150]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 10 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die versetzte Treppe führt durch mehrere kleine Restzahlen bis zu einem linken Doppelknick unten. Kein Voll-/Nullanker erleichtert eine ganze Linie; das ist ein anderer dünner Abschlusstyp als der weite Querarm in 11, trotz ähnlicher technischer Tiefe 33/34.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ......┌─
01 ......│.
02 .....┌┘.
03 ...┌─┘..
04 ...│....
05 ..┌┘....
06 ┌┐│.....
07 │└┘.....
```
