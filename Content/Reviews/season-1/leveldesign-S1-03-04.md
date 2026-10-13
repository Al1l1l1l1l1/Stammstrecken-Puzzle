# Author-Leveldesignreview: S1-03-04 (globale Route 12)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Hohe und niedrige Counts | S1-03-04-01: 6, S1-03-04-02: 7 |
| 3–5 | Abhängige Ketten | S1-03-04-03: 10, S1-03-04-04: 11, S1-03-04-05: 12 |
| 6–8 | Mittlere Zahlen, wenige Extreme | S1-03-04-06: 13, S1-03-04-07: 13, S1-03-04-08: 14 |
| 9–10 | Schwierige Endpunktkanten | S1-03-04-09: 10, S1-03-04-10: 11 |
| 11–12 | Strategischer Abschnittsabschluss | S1-03-04-11: 22, S1-03-04-12: 24 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-03-04-01

- Input: 7×6; A N/0, B S/6. Zeilen `[7, 1, 5, 1, 5, 1]`, Spalten `[1, 1, 4, 3, 3, 3, 5]`.
- Lösung: 20 Zellen, 6 Kurven; Dichte 20/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=89, maxDeductionDepth=6.
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[22, 23, 24, 25, 26, 27]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 33, 54, 55, 73, 83]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-04-01, globale Route 12, Gruppe 1: Hohe und niedrige Counts. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 29 konkrete Anschlussfolgen und 22 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 20/42 Zellen, 6 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └─────┐
01 ......│
02 ..┌───┘
03 ..│....
04 ..└───┐
05 ......│
```

## S1-03-04-02

- Input: 7×6; A S/0, B W/2. Zeilen `[0, 2, 2, 2, 2, 2]`, Spalten `[5, 5, 0, 0, 0, 0, 0]`.
- Lösung: 10 Zellen, 9 Kurven; Dichte 10/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=73, maxDeductionDepth=7.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[22, 23, 24, 25, 26, 27, 28]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 29, 64, 65, 67, 69, 71]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-04-02, globale Route 12, Gruppe 1: Hohe und niedrige Counts. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 11 konkrete Anschlussfolgen und 0 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 10/42 Zellen, 9 Kurven, 6 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .......
01 ┌┐.....
02 ┘│.....
03 ┌┘.....
04 └┐.....
05 ┌┘.....
```

## S1-03-04-03

- Input: 7×6; A W/2, B N/3. Zeilen `[1, 3, 4, 2, 3, 7]`, Spalten `[4, 1, 1, 4, 3, 5, 2]`.
- Lösung: 20 Zellen, 9 Kurven; Dichte 20/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=89, maxDeductionDepth=10.
- Einstieg: Zeile 5 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[28, 29, 30, 31, 32, 33, 34]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 22, 48, 53, 55, 64, 69, 84, 85, 86]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-04-03, globale Route 12, Gruppe 2: Abhängige Ketten. Lesbarer öffentlicher Einstieg: Zeile 5 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 32 konkrete Anschlussfolgen und 21 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 20/42 Zellen, 9 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...│...
01 ...│┌┐.
02 ┐..└┘│.
03 │....│.
04 │....└┐
05 └─────┘
```

## S1-03-04-04

- Input: 7×6; A E/5, B N/2. Zeilen `[2, 6, 1, 2, 2, 1]`, Spalten `[0, 2, 2, 1, 1, 3, 5]`.
- Lösung: 14 Zellen, 9 Kurven; Dichte 14/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=93, maxDeductionDepth=11.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[28, 29, 30, 31, 32]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 24, 38, 71, 74, 75, 76, 78, 81, 86, 91]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-04-04, globale Route 12, Gruppe 2: Abhängige Ketten. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 36 konkrete Anschlussfolgen und 20 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 14/42 Zellen, 9 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌┘....
01 .└────┐
02 ......│
03 .....┌┘
04 .....└┐
05 ......└
```

## S1-03-04-05

- Input: 7×6; A N/3, B E/1. Zeilen `[1, 4, 3, 3, 4, 6]`, Spalten `[0, 5, 5, 4, 1, 1, 5]`.
- Lösung: 21 Zellen, 13 Kurven; Dichte 21/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=97, maxDeductionDepth=12.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[28, 29, 30, 31, 32]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 24, 47, 50, 62, 75, 86, 87, 91, 92, 93, 95]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-04-05, globale Route 12, Gruppe 2: Abhängige Ketten. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 37 konkrete Anschlussfolgen und 16 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 21/42 Zellen, 13 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...│...
01 .┌─┘..┌
02 .└┐...│
03 .┌┘...│
04 .│┌┐..│
05 .└┘└──┘
```

## S1-03-04-06

- Input: 7×6; A N/0, B S/5. Zeilen `[4, 2, 1, 1, 2, 3]`, Spalten `[1, 1, 1, 4, 5, 1, 0]`.
- Lösung: 13 Zellen, 8 Kurven; Dichte 13/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=94, maxDeductionDepth=13.
- Einstieg: Spalte 6 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[32, 33, 34, 35, 36, 37]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 22, 38, 39, 52, 57, 58, 64, 69, 70, 76, 85, 92]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-04-06, globale Route 12, Gruppe 3: Mittlere Zahlen, wenige Extreme. Lesbarer öffentlicher Einstieg: Spalte 6 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 36 konkrete Anschlussfolgen und 23 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 13/42 Zellen, 8 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 13. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └──┐...
01 ...└┐..
02 ....│..
03 ....│..
04 ...┌┘..
05 ...└─┐.
```

## S1-03-04-07

- Input: 7×6; A W/1, B E/5. Zeilen `[3, 2, 1, 2, 4, 5]`, Spalten `[2, 1, 6, 3, 1, 2, 2]`.
- Lösung: 17 Zellen, 12 Kurven; Dichte 17/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=93, maxDeductionDepth=13.
- Einstieg: Spalte 2 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[22, 23, 24, 25, 26, 27]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 28, 44, 54, 55, 56, 57, 66, 74, 82, 89, 90, 91]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-04-07, globale Route 12, Gruppe 3: Mittlere Zahlen, wenige Extreme. Lesbarer öffentlicher Einstieg: Spalte 2 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 37 konkrete Anschlussfolgen und 21 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 17/42 Zellen, 12 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 13. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐....
01 ┘.│....
02 ..│....
03 ..└┐...
04 ..┌┘.┌┐
05 ..└──┘└
```

## S1-03-04-08

- Input: 7×6; A W/0, B E/2. Zeilen `[1, 4, 4, 4, 4, 6]`, Spalten `[6, 1, 5, 1, 5, 1, 4]`.
- Lösung: 23 Zellen, 8 Kurven; Dichte 23/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=100, maxDeductionDepth=14.
- Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[28, 29, 30, 31, 32]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 23, 34, 54, 61, 62, 78, 79, 80, 81, 83, 90, 94, 99]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-04-08, globale Route 12, Gruppe 3: Mittlere Zahlen, wenige Extreme. Lesbarer öffentlicher Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 45 konkrete Anschlussfolgen und 16 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 23/42 Zellen, 8 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 14. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐......
01 │.┌─┐..
02 │.│.│.┌
03 │.│.│.│
04 │.│.│.│
05 └─┘.└─┘
```

## S1-03-04-09

- Input: 7×6; A E/1, B N/4. Zeilen `[7, 7, 3, 3, 0, 0]`, Spalten `[2, 4, 3, 4, 3, 2, 2]`.
- Lösung: 20 Zellen, 17 Kurven; Dichte 20/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=94, maxDeductionDepth=10.
- Einstieg: Zeile 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[22, 49, 66, 67, 76, 79, 84, 85, 90, 91]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-04-09, globale Route 12, Gruppe 4: Schwierige Endpunktkanten. Lesbarer öffentlicher Einstieg: Zeile 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 33 konkrete Anschlussfolgen und 8 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 20/42 Zellen, 17 Kurven, 2 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐┌┘┌┐
01 └┐└┘┌┘└
02 .│.┌┘..
03 .└─┘...
04 .......
05 .......
```

## S1-03-04-10

- Input: 7×6; A E/5, B W/3. Zeilen `[0, 0, 4, 7, 1, 1]`, Spalten `[1, 1, 2, 2, 2, 2, 3]`.
- Lösung: 13 Zellen, 10 Kurven; Dichte 13/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=87, maxDeductionDepth=11.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[22, 23, 24, 25, 26, 27, 28]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 47, 64, 65, 66, 70, 77, 78, 83, 84, 85]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-04-10, globale Route 12, Gruppe 4: Schwierige Endpunktkanten. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 26 konkrete Anschlussfolgen und 15 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 13/42 Zellen, 10 Kurven, 2 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .......
01 .......
02 ..┌┐┌┐.
03 ──┘└┘└┐
04 ......│
05 ......└
```

## S1-03-04-11

- Input: 7×6; A S/6, B N/4. Zeilen `[1, 3, 4, 4, 5, 3]`, Spalten `[0, 3, 3, 3, 6, 4, 1]`.
- Lösung: 20 Zellen, 14 Kurven; Dichte 20/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=102, maxDeductionDepth=22.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[28, 29, 30, 31, 32]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 41, 55, 56, 63, 67, 68, 71, 73, 75, 77, 78, 83, 85, 86, 87, 90, 91, 95, 96, 97, 100]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-04-11, globale Route 12, Gruppe 5: Strategischer Abschnittsabschluss. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 46 konkrete Anschlussfolgen und 17 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 20/42 Zellen, 14 Kurven, 1 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 22. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....│..
01 ..┌─┘..
02 .┌┘.┌┐.
03 .│.┌┘│.
04 .└─┘┌┘.
05 ....└─┐
```

## S1-03-04-12

- Input: 7×6; A W/0, B E/3. Zeilen `[3, 1, 1, 4, 4, 3]`, Spalten `[1, 1, 4, 2, 2, 3, 3]`.
- Lösung: 16 Zellen, 10 Kurven; Dichte 16/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=105, maxDeductionDepth=24.
- Einstieg: A an W/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[0]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 22, 27, 28, 34, 39, 40, 46, 50, 51, 56, 60, 61, 66, 75, 76, 83, 85, 86, 89, 95, 96, 100, 104]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-04-12, globale Route 12, Gruppe 5: Strategischer Abschnittsabschluss. Lesbarer öffentlicher Einstieg: A an W/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 49 konkrete Anschlussfolgen und 26 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 16/42 Zellen, 10 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 24. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ──┐....
01 ..│....
02 ..│....
03 ..└┐.┌─
04 ...└┐└┐
05 ....└─┘
```
