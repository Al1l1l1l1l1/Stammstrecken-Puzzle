# Author-Leveldesignreview: S1-04-03 (globale Route 15)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Bekannte Muster im größeren Feld | S1-04-03-01: 6, S1-04-03-02: 7 |
| 3–5 | Sichere Ausschlüsse | S1-04-03-03: 10, S1-04-03-04: 10, S1-04-03-05: 11 |
| 6–8 | Parallele Teilketten | S1-04-03-06: 12, S1-04-03-07: 14, S1-04-03-08: 14 |
| 9–10 | Geometrie aus Belegung | S1-04-03-09: 12, S1-04-03-10: 14 |
| 11–12 | Anspruchsvolle 8×8-Puzzles | S1-04-03-11: 34, S1-04-03-12: 33 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-04-03-01

- Input: 7×7; A N/5, B E/5. Zeilen `[2, 1, 1, 1, 2, 2, 2]`, Spalten `[0, 0, 0, 0, 0, 4, 7]`.
- Lösung: 11 Zellen, 7 Kurven; Dichte 11/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=85, maxDeductionDepth=6.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[24, 25, 26, 27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 72, 74, 77, 78, 84]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-03-01, globale Route 15, Gruppe 1: Bekannte Muster im größeren Feld. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 17 konkrete Anschlussfolgen und 3 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 11/49 Zellen, 7 Kurven, 5 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[24, 25, 26, 27, 28, 29]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 38, 48, 63, 64, 84, 93]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-03-02, globale Route 15, Gruppe 1: Bekannte Muster im größeren Feld. Lesbarer öffentlicher Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 39 konkrete Anschlussfolgen und 22 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 27/49 Zellen, 8 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[36, 37, 38, 39, 40, 41]`.
- Eine tatsächliche maximale Prämissenkette: `[18, 24, 57, 58, 73, 85, 86, 87, 90, 92]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-03-03, globale Route 15, Gruppe 2: Sichere Ausschlüsse. Lesbarer öffentlicher Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 27 konkrete Anschlussfolgen und 18 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/49 Zellen, 14 Kurven, 2 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[24, 25, 26, 27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 38, 60, 72, 96, 97, 98, 99, 100, 101]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-03-04, globale Route 15, Gruppe 2: Sichere Ausschlüsse. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 38 konkrete Anschlussfolgen und 12 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 27/49 Zellen, 8 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[24, 25, 26, 27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 42, 56, 71, 79, 80, 81, 83, 103, 104, 105]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-03-05, globale Route 15, Gruppe 2: Sichere Ausschlüsse. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 37 konkrete Anschlussfolgen und 16 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/49 Zellen, 12 Kurven, 2 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: Zeile 7 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[35, 36, 37, 38, 39, 40, 41, 42]`.
- Eine tatsächliche maximale Prämissenkette: `[27, 43, 49, 81, 83, 96, 97, 108, 124, 125, 128, 129]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-03-06, globale Route 15, Gruppe 3: Parallele Teilketten. Lesbarer öffentlicher Einstieg: Zeile 7 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 43 konkrete Anschlussfolgen und 36 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 20/64 Zellen, 14 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[28, 29, 30, 31, 32, 33, 34]`.
- Eine tatsächliche maximale Prämissenkette: `[9, 51, 59, 68, 70, 80, 81, 103, 104, 105, 106, 107, 120, 123]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-03-07, globale Route 15, Gruppe 3: Parallele Teilketten. Lesbarer öffentlicher Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 67 konkrete Anschlussfolgen und 27 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 35/64 Zellen, 8 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 14. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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

- Input: 8×8; A N/1, B S/0. Zeilen `[7, 1, 8, 1, 8, 1, 8, 1]`, Spalten `[5, 4, 4, 4, 4, 4, 4, 6]`.
- Lösung: 35 Zellen, 8 Kurven; Dichte 35/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=140, maxDeductionDepth=14.
- Einstieg: Zeile 2 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[20, 52, 73, 84, 86, 87, 88, 89, 90, 91, 92, 93, 110, 116]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-03-08, globale Route 15, Gruppe 3: Parallele Teilketten. Lesbarer öffentlicher Einstieg: Zeile 2 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 56 konkrete Anschlussfolgen und 26 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 35/64 Zellen, 8 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 14. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .└─────┐
01 .......│
02 ┌──────┘
03 │.......
04 └──────┐
05 .......│
06 ┌──────┘
07 │.......
```

## S1-04-03-09

- Input: 8×8; A E/4, B W/6. Zeilen `[0, 0, 0, 8, 4, 3, 3, 2]`, Spalten `[5, 4, 4, 2, 1, 1, 1, 2]`.
- Lösung: 20 Zellen, 14 Kurven; Dichte 20/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=128, maxDeductionDepth=12.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 62, 93, 100, 107, 108, 110, 112, 120, 124, 125, 127]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-03-09, globale Route 15, Gruppe 4: Geometrie aus Belegung. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 41 konkrete Anschlussfolgen und 20 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 20/64 Zellen, 14 Kurven, 3 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[9, 42, 55, 61, 86, 97, 99, 103, 120, 121, 123, 124, 128, 130]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-03-10, globale Route 15, Gruppe 4: Geometrie aus Belegung. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 54 konkrete Anschlussfolgen und 23 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 21/64 Zellen, 17 Kurven, 3 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 14. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: A an N/5 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[5]`.
- Eine tatsächliche maximale Prämissenkette: `[20, 28, 42, 43, 49, 56, 57, 63, 70, 71, 77, 84, 85, 91, 98, 99, 105, 112, 113, 119, 125, 126, 127, 131, 132, 133, 135, 141, 142, 143, 144, 146, 147, 151]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-03-11, globale Route 15, Gruppe 5: Anspruchsvolle 8×8-Puzzles. Lesbarer öffentlicher Einstieg: A an N/5 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 75 konkrete Anschlussfolgen und 45 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/64 Zellen, 10 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 34. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: A an E/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[7]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 28, 35, 43, 49, 50, 57, 63, 64, 71, 72, 77, 83, 88, 89, 95, 96, 100, 105, 106, 110, 115, 118, 119, 123, 126, 130, 134, 135, 137, 147, 148, 150]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-03-12, globale Route 15, Gruppe 5: Anspruchsvolle 8×8-Puzzles. Lesbarer öffentlicher Einstieg: A an E/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 71 konkrete Anschlussfolgen und 47 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 17/64 Zellen, 11 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 33. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
