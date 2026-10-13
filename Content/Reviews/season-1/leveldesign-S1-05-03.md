# Author-Leveldesignreview: S1-05-03 (globale Route 19)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Bekannte Startmuster | S1-05-03-01: 7, S1-05-03-02: 8 |
| 3–5 | Mehrere lange Ketten | S1-05-03-03: 12, S1-05-03-04: 10, S1-05-03-05: 15 |
| 6–8 | Wenige direkte Extreme | S1-05-03-06: 14, S1-05-03-07: 15, S1-05-03-08: 16 |
| 9–10 | Problematische Kanten und Kurven | S1-05-03-09: 14, S1-05-03-10: 15 |
| 11–12 | Große prüfbare Höhepunkte | S1-05-03-11: 31, S1-05-03-12: 29 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-05-03-01

- Input: 9×9; A S/1, B W/1. Zeilen `[3, 3, 2, 2, 2, 1, 1, 1, 2]`, Spalten `[9, 6, 2, 0, 0, 0, 0, 0, 0]`.
- Lösung: 17 Zellen, 13 Kurven; Dichte 17/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=138, maxDeductionDepth=7.
- Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[40, 41, 42, 43, 44, 45, 46, 47, 48]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 95, 127, 128, 130, 131, 136]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-03-01, globale Route 19, Gruppe 1: Bekannte Startmuster. Lesbarer öffentlicher Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 30 konkrete Anschlussfolgen und 9 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 17/81 Zellen, 13 Kurven, 6 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐......
01 ┘┌┘......
02 ┌┘.......
03 └┐.......
04 ┌┘.......
05 │........
06 │........
07 │........
08 └┐.......
```

## S1-05-03-02

- Input: 9×9; A N/1, B S/8. Zeilen `[8, 1, 7, 1, 7, 1, 8, 1, 8]`, Spalten `[0, 4, 6, 5, 5, 5, 5, 5, 7]`.
- Lösung: 42 Zellen, 10 Kurven; Dichte 42/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=177, maxDeductionDepth=8.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 49, 74, 89, 119, 128, 143, 149]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-03-02, globale Route 19, Gruppe 1: Bekannte Startmuster. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 70 konkrete Anschlussfolgen und 30 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 42/81 Zellen, 10 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 8. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .└──────┐
01 ........│
02 ..┌─────┘
03 ..│......
04 ..└─────┐
05 ........│
06 .┌──────┘
07 .│.......
08 .└──────┐
```

## S1-05-03-03

- Input: 9×9; A W/0, B E/8. Zeilen `[1, 7, 5, 5, 5, 5, 6, 4, 1]`, Spalten `[7, 1, 6, 1, 7, 1, 7, 1, 8]`.
- Lösung: 39 Zellen, 10 Kurven; Dichte 39/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=183, maxDeductionDepth=12.
- Einstieg: A an W/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[0]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 39, 62, 89, 96, 108, 112, 128, 134, 152, 155, 165]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-03-03, globale Route 19, Gruppe 2: Mehrere lange Ketten. Lesbarer öffentlicher Einstieg: A an W/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 77 konkrete Anschlussfolgen und 42 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 39/81 Zellen, 10 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐........
01 │.┌─┐.┌─┐
02 │.│.│.│.│
03 │.│.│.│.│
04 │.│.│.│.│
05 │.│.│.│.│
06 └─┘.│.│.│
07 ....└─┘.│
08 ........└
```

## S1-05-03-04

- Input: 9×9; A W/8, B N/7. Zeilen `[1, 1, 2, 1, 1, 1, 2, 4, 9]`, Spalten `[1, 2, 2, 1, 1, 1, 1, 6, 7]`.
- Lösung: 22 Zellen, 11 Kurven; Dichte 22/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=159, maxDeductionDepth=10.
- Einstieg: Zeile 8 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[40, 41, 42, 43, 44, 45, 46, 47]`.
- Eine tatsächliche maximale Prämissenkette: `[45, 82, 112, 113, 114, 144, 149, 151, 152, 158]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-03-04, globale Route 19, Gruppe 2: Mehrere lange Ketten. Lesbarer öffentlicher Einstieg: Zeile 8 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 52 konkrete Anschlussfolgen und 59 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 22/81 Zellen, 11 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .......│.
01 .......│.
02 .......└┐
03 ........│
04 ........│
05 ........│
06 .......┌┘
07 .┌┐....└┐
08 ─┘└─────┘
```

## S1-05-03-05

- Input: 9×9; A N/1, B S/8. Zeilen `[6, 1, 4, 1, 4, 1, 4, 1, 6]`, Spalten `[0, 1, 1, 7, 5, 5, 7, 1, 1]`.
- Lösung: 28 Zellen, 10 Kurven; Dichte 28/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=168, maxDeductionDepth=15.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[31, 56, 75, 76, 85, 114, 135, 142, 145, 147, 150, 152, 155, 157, 160]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-03-05, globale Route 19, Gruppe 2: Mehrere lange Ketten. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 69 konkrete Anschlussfolgen und 40 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 28/81 Zellen, 10 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 15. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .└────┐..
01 ......│..
02 ...┌──┘..
03 ...│.....
04 ...└──┐..
05 ......│..
06 ...┌──┘..
07 ...│.....
08 ...└────┐
```

## S1-05-03-06

- Input: 9×9; A W/2, B E/7. Zeilen `[3, 2, 3, 6, 5, 5, 7, 1, 0]`, Spalten `[5, 1, 7, 1, 7, 1, 4, 1, 5]`.
- Lösung: 32 Zellen, 10 Kurven; Dichte 32/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=171, maxDeductionDepth=14.
- Einstieg: Zeile 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[40, 41, 42, 43, 44, 45, 46, 47, 48]`.
- Eine tatsächliche maximale Prämissenkette: `[22, 34, 50, 73, 86, 87, 88, 89, 90, 110, 111, 120, 144, 153]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-03-06, globale Route 19, Gruppe 3: Wenige direkte Extreme. Lesbarer öffentlicher Einstieg: Zeile 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 72 konkrete Anschlussfolgen und 34 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 32/81 Zellen, 10 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 14. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌─┐....
01 ..│.│....
02 ┐.│.│....
03 │.│.│.┌─┐
04 │.│.│.│.│
05 │.│.│.│.│
06 └─┘.└─┘.│
07 ........└
08 .........
```

## S1-05-03-07

- Input: 9×9; A N/2, B S/8. Zeilen `[5, 1, 6, 1, 6, 1, 7, 1, 9]`, Spalten `[3, 5, 5, 5, 5, 5, 7, 1, 1]`.
- Lösung: 37 Zellen, 10 Kurven; Dichte 37/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=178, maxDeductionDepth=15.
- Einstieg: Zeile 8 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39]`.
- Eine tatsächliche maximale Prämissenkette: `[39, 47, 64, 100, 108, 122, 136, 148, 150, 151, 152, 153, 154, 162, 166]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-03-07, globale Route 19, Gruppe 3: Wenige direkte Extreme. Lesbarer öffentlicher Einstieg: Zeile 8 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 73 konkrete Anschlussfolgen und 42 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 37/81 Zellen, 10 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 15. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└───┐..
01 ......│..
02 .┌────┘..
03 .│.......
04 .└────┐..
05 ......│..
06 ┌─────┘..
07 │........
08 └───────┐
```

## S1-05-03-08

- Input: 9×9; A N/2, B S/7. Zeilen `[6, 1, 8, 1, 8, 1, 5, 1, 5]`, Spalten `[3, 2, 3, 6, 5, 5, 5, 7, 0]`.
- Lösung: 36 Zellen, 10 Kurven; Dichte 36/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=166, maxDeductionDepth=16.
- Einstieg: Spalte 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 51, 66, 79, 82, 84, 88, 90, 103, 104, 118, 131, 149, 153, 155, 159]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-03-08, globale Route 19, Gruppe 3: Wenige direkte Extreme. Lesbarer öffentlicher Einstieg: Spalte 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 69 konkrete Anschlussfolgen und 27 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 36/81 Zellen, 10 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 16. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└────┐.
01 .......│.
02 ┌──────┘.
03 │........
04 └──────┐.
05 .......│.
06 ...┌───┘.
07 ...│.....
08 ...└───┐.
```

## S1-05-03-09

- Input: 9×9; A E/5, B N/1. Zeilen `[8, 9, 2, 2, 2, 2, 0, 0, 0]`, Spalten `[2, 2, 2, 2, 2, 2, 1, 6, 6]`.
- Lösung: 25 Zellen, 21 Kurven; Dichte 25/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=157, maxDeductionDepth=14.
- Einstieg: Zeile 6 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[41, 42, 43, 44, 45, 46, 47, 48, 49]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 71, 107, 116, 119, 120, 121, 123, 124, 126, 127, 151, 154, 155]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-03-09, globale Route 19, Gruppe 4: Problematische Kanten und Kurven. Lesbarer öffentlicher Einstieg: Zeile 6 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 51 konkrete Anschlussfolgen und 25 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 25/81 Zellen, 21 Kurven, 3 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 14. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘┌┐┌┐.┌┐
01 └─┘└┘└─┘│
02 .......┌┘
03 .......└┐
04 .......┌┘
05 .......└─
06 .........
07 .........
08 .........
```

## S1-05-03-10

- Input: 9×9; A S/5, B W/8. Zeilen `[0, 0, 0, 5, 5, 5, 3, 3, 3]`, Spalten `[6, 6, 2, 3, 1, 6, 0, 0, 0]`.
- Lösung: 24 Zellen, 15 Kurven; Dichte 24/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=153, maxDeductionDepth=15.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 69, 112, 130, 136, 137, 138, 140, 142, 143, 144, 145, 148, 149, 152]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-03-10, globale Route 19, Gruppe 4: Problematische Kanten und Kurven. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 46 konkrete Anschlussfolgen und 11 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 24/81 Zellen, 15 Kurven, 6 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 15. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .........
01 .........
02 .........
03 ┌┐.┌─┐...
04 │└┐│.│...
05 └┐└┘.│...
06 ┌┘...│...
07 └┐...│...
08 ─┘...│...
```

## S1-05-03-11

- Input: 9×9; A E/8, B W/1. Zeilen `[2, 4, 2, 2, 4, 2, 2, 1, 3]`, Spalten `[1, 3, 5, 4, 2, 2, 3, 1, 1]`.
- Lösung: 22 Zellen, 14 Kurven; Dichte 22/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=188, maxDeductionDepth=31.
- Einstieg: A an E/8 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[31]`.
- Eine tatsächliche maximale Prämissenkette: `[31, 47, 66, 67, 75, 84, 85, 90, 96, 97, 102, 108, 109, 115, 122, 123, 127, 132, 133, 138, 144, 145, 150, 165, 170, 174, 178, 180, 182, 185, 186]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-03-11, globale Route 19, Gruppe 5: Große prüfbare Höhepunkte. Lesbarer öffentlicher Einstieg: A an E/8 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 86 konkrete Anschlussfolgen und 59 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 22/81 Zellen, 14 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 31. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌┐.....
01 ──┘│.....
02 ..┌┘.....
03 .┌┘......
04 .└──┐....
05 ....└┐...
06 .....└┐..
07 ......│..
08 ......└──
```

## S1-05-03-12

- Input: 9×9; A E/8, B W/1. Zeilen `[4, 3, 1, 2, 2, 2, 1, 1, 6]`, Spalten `[2, 3, 6, 6, 1, 1, 1, 1, 1]`.
- Lösung: 22 Zellen, 12 Kurven; Dichte 22/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=185, maxDeductionDepth=29.
- Einstieg: A an E/8 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[31]`.
- Eine tatsächliche maximale Prämissenkette: `[31, 39, 48, 49, 57, 66, 67, 75, 84, 85, 93, 102, 103, 111, 120, 121, 128, 147, 148, 150, 153, 154, 156, 164, 165, 169, 170, 180, 182]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-03-12, globale Route 19, Gruppe 5: Große prüfbare Höhepunkte. Lesbarer öffentlicher Einstieg: A an E/8 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 83 konkrete Anschlussfolgen und 59 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 22/81 Zellen, 12 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 29. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──┐.....
01 ┘.┌┘.....
02 ..│......
03 .┌┘......
04 .└┐......
05 ..└┐.....
06 ...│.....
07 ...│.....
08 ...└─────
```
