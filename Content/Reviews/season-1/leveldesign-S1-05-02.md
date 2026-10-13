# Author-Leveldesignreview: S1-05-02 (globale Route 18)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Ruhige klare Belegung | S1-05-02-01: 7, S1-05-02-02: 7 |
| 3–5 | Mehrere mittlere Hinweise | S1-05-02-03: 11, S1-05-02-04: 10, S1-05-02-05: 12 |
| 6–8 | Lokale und globale Folgerungen | S1-05-02-06: 14, S1-05-02-07: 15, S1-05-02-08: 16 |
| 9–10 | Hohe Dichte und Geometrie | S1-05-02-09: 14, S1-05-02-10: 14 |
| 11–12 | 9×9-Meisterschaft | S1-05-02-11: 32, S1-05-02-12: 30 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-05-02-01

- Input: 8×8; A E/5, B W/1. Zeilen `[0, 2, 1, 6, 8, 3, 0, 0]`, Spalten `[4, 5, 2, 2, 1, 1, 2, 3]`.
- Lösung: 20 Zellen, 14 Kurven; Dichte 20/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=127, maxDeductionDepth=7.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 61, 96, 112, 114, 115, 121]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-02-01, globale Route 18, Gruppe 1: Ruhige klare Belegung. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 37 konkrete Anschlussfolgen und 20 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 20/64 Zellen, 14 Kurven, 3 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ........
01 ─┐......
02 .│......
03 ┌┘┌┐..┌┐
04 │┌┘└──┘│
05 └┘.....└
06 ........
07 ........
```

## S1-05-02-02

- Input: 8×8; A W/0, B E/0. Zeilen `[6, 4, 4, 4, 4, 4, 5, 3]`, Spalten `[8, 1, 8, 1, 7, 1, 7, 1]`.
- Lösung: 34 Zellen, 8 Kurven; Dichte 34/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=142, maxDeductionDepth=7.
- Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[28, 29, 30, 31, 32, 33, 34]`.
- Eine tatsächliche maximale Prämissenkette: `[20, 58, 65, 86, 87, 109, 122]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-02-02, globale Route 18, Gruppe 1: Ruhige klare Belegung. Lesbarer öffentlicher Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 56 konkrete Anschlussfolgen und 30 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 34/64 Zellen, 8 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.┌─┐.┌─
01 │.│.│.│.
02 │.│.│.│.
03 │.│.│.│.
04 │.│.│.│.
05 │.│.│.│.
06 │.│.└─┘.
07 └─┘.....
```

## S1-05-02-03

- Input: 8×8; A N/0, B S/2. Zeilen `[7, 1, 6, 1, 5, 1, 4, 1]`, Spalten `[1, 4, 5, 4, 4, 5, 3, 0]`.
- Lösung: 26 Zellen, 8 Kurven; Dichte 26/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=145, maxDeductionDepth=11.
- Einstieg: Spalte 7 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[41, 42, 43, 44, 45, 46, 47]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 50, 80, 81, 96, 102, 111, 121, 133, 138, 142]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-02-03, globale Route 18, Gruppe 2: Mehrere mittlere Hinweise. Lesbarer öffentlicher Einstieg: Spalte 7 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 61 konkrete Anschlussfolgen und 31 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 26/64 Zellen, 8 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └─────┐.
01 ......│.
02 .┌────┘.
03 .│......
04 .└───┐..
05 .....│..
06 ..┌──┘..
07 ..│.....
```

## S1-05-02-04

- Input: 8×8; A S/7, B N/0. Zeilen `[5, 8, 1, 1, 1, 1, 1, 1]`, Spalten `[2, 2, 2, 1, 2, 2, 1, 7]`.
- Lösung: 19 Zellen, 10 Kurven; Dichte 19/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=128, maxDeductionDepth=10.
- Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[34, 54, 63, 69, 74, 79, 84, 89, 94, 120]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-02-04, globale Route 18, Gruppe 2: Mehrere mittlere Hinweise. Lesbarer öffentlicher Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 44 konkrete Anschlussfolgen und 44 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/64 Zellen, 10 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 │┌┐.┌┐..
01 └┘└─┘└─┐
02 .......│
03 .......│
04 .......│
05 .......│
06 .......│
07 .......│
```

## S1-05-02-05

- Input: 8×8; A W/0, B E/5. Zeilen `[2, 2, 1, 1, 1, 5, 3, 6]`, Spalten `[8, 3, 1, 1, 3, 3, 1, 1]`.
- Lösung: 21 Zellen, 8 Kurven; Dichte 21/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=138, maxDeductionDepth=12.
- Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[28, 29, 30, 31, 32, 33, 34]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 39, 47, 52, 73, 90, 94, 122, 124, 126, 128, 134]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-02-05, globale Route 18, Gruppe 2: Mehrere mittlere Hinweise. Lesbarer öffentlicher Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 54 konkrete Anschlussfolgen und 41 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 21/64 Zellen, 8 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ─┐......
01 ┌┘......
02 │.......
03 │.......
04 │.......
05 │...┌───
06 │...└┐..
07 └────┘..
```

## S1-05-02-06

- Input: 9×9; A W/0, B E/7. Zeilen `[1, 4, 3, 6, 5, 5, 6, 4, 0]`, Spalten `[8, 1, 5, 1, 4, 1, 6, 1, 7]`.
- Lösung: 34 Zellen, 10 Kurven; Dichte 34/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=188, maxDeductionDepth=14.
- Einstieg: Zeile 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[40, 41, 42, 43, 44, 45, 46, 47, 48]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 55, 74, 89, 119, 120, 131, 136, 150, 162, 168, 176, 177, 182]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-02-06, globale Route 18, Gruppe 3: Lokale und globale Folgerungen. Lesbarer öffentlicher Einstieg: Zeile 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 84 konkrete Anschlussfolgen und 38 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 34/81 Zellen, 10 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 14. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐........
01 │.....┌─┐
02 │.....│.│
03 │.┌─┐.│.│
04 │.│.│.│.│
05 │.│.│.│.│
06 │.│.└─┘.│
07 └─┘.....└
08 .........
```

## S1-05-02-07

- Input: 9×9; A N/0, B S/6. Zeilen `[9, 1, 8, 1, 7, 1, 7, 1, 6]`, Spalten `[1, 7, 5, 5, 5, 5, 5, 5, 3]`.
- Lösung: 41 Zellen, 10 Kurven; Dichte 41/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=182, maxDeductionDepth=15.
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 40, 51, 89, 97, 110, 140, 142, 144, 146, 148, 151, 152, 166, 176]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-02-07, globale Route 18, Gruppe 3: Lokale und globale Folgerungen. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 77 konkrete Anschlussfolgen und 38 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 41/81 Zellen, 10 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 15. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └───────┐
01 ........│
02 .┌──────┘
03 .│.......
04 .└─────┐.
05 .......│.
06 .┌─────┘.
07 .│.......
08 .└────┐..
```

## S1-05-02-08

- Input: 9×9; A S/8, B W/1. Zeilen `[5, 8, 1, 2, 2, 3, 1, 1, 1]`, Spalten `[1, 1, 1, 2, 2, 2, 6, 5, 4]`.
- Lösung: 24 Zellen, 15 Kurven; Dichte 24/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=180, maxDeductionDepth=16.
- Einstieg: A an S/8 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[31]`.
- Eine tatsächliche maximale Prämissenkette: `[31, 39, 62, 63, 70, 90, 91, 97, 120, 121, 134, 146, 151, 152, 176, 178]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-02-08, globale Route 18, Gruppe 3: Lokale und globale Folgerungen. Lesbarer öffentlicher Einstieg: A an S/8 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 76 konkrete Anschlussfolgen und 57 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 24/81 Zellen, 15 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 16. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...┌┐┌─┐.
01 ───┘└┘┌┘.
02 ......│..
03 ......└┐.
04 ......┌┘.
05 ......└─┐
06 ........│
07 ........│
08 ........│
```

## S1-05-02-09

- Input: 9×9; A W/0, B E/7. Zeilen `[4, 3, 6, 5, 5, 5, 5, 5, 6]`, Spalten `[9, 1, 9, 1, 9, 1, 7, 1, 6]`.
- Lösung: 44 Zellen, 10 Kurven; Dichte 44/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=185, maxDeductionDepth=14.
- Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 59, 66, 74, 75, 76, 105, 106, 121, 141, 147, 154, 157, 177]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-02-09, globale Route 18, Gruppe 4: Hohe Dichte und Geometrie. Lesbarer öffentlicher Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 82 konkrete Anschlussfolgen und 33 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 44/81 Zellen, 10 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 14. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.┌─┐....
01 │.│.│....
02 │.│.│.┌─┐
03 │.│.│.│.│
04 │.│.│.│.│
05 │.│.│.│.│
06 │.│.│.│.│
07 │.│.│.│.└
08 └─┘.└─┘..
```

## S1-05-02-10

- Input: 9×9; A N/1, B S/7. Zeilen `[6, 1, 7, 1, 8, 1, 8, 1, 8]`, Spalten `[6, 5, 5, 5, 5, 5, 6, 4, 0]`.
- Lösung: 41 Zellen, 10 Kurven; Dichte 41/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=187, maxDeductionDepth=14.
- Einstieg: Spalte 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[31, 66, 73, 81, 88, 97, 100, 115, 131, 145, 149, 155, 172, 182]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-02-10, globale Route 18, Gruppe 4: Hohe Dichte und Geometrie. Lesbarer öffentlicher Einstieg: Spalte 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 81 konkrete Anschlussfolgen und 31 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 41/81 Zellen, 10 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 14. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .└────┐..
01 ......│..
02 ┌─────┘..
03 │........
04 └──────┐.
05 .......│.
06 ┌──────┘.
07 │........
08 └──────┐.
```

## S1-05-02-11

- Input: 9×9; A S/7, B N/3. Zeilen `[1, 2, 1, 1, 3, 1, 4, 5, 3]`, Spalten `[3, 3, 6, 4, 1, 2, 1, 1, 0]`.
- Lösung: 21 Zellen, 14 Kurven; Dichte 21/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=180, maxDeductionDepth=32.
- Einstieg: Spalte 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[47, 48, 49, 50, 51, 52, 53, 54]`.
- Eine tatsächliche maximale Prämissenkette: `[30, 46, 69, 70, 77, 85, 86, 91, 97, 98, 104, 111, 112, 118, 125, 126, 131, 135, 142, 146, 150, 153, 155, 156, 159, 160, 163, 166, 168, 171, 177, 179]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-02-11, globale Route 18, Gruppe 5: 9×9-Meisterschaft. Lesbarer öffentlicher Einstieg: Spalte 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 79 konkrete Anschlussfolgen und 52 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 21/81 Zellen, 14 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 32. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...│.....
01 ..┌┘.....
02 ..│......
03 ..│......
04 ┌─┘......
05 │........
06 └┐┌┐.....
07 .└┘└─┐...
08 .....└─┐.
```

## S1-05-02-12

- Input: 9×9; A S/8, B W/1. Zeilen `[3, 3, 2, 1, 1, 3, 1, 1, 7]`, Spalten `[6, 4, 6, 1, 1, 1, 1, 1, 1]`.
- Lösung: 22 Zellen, 11 Kurven; Dichte 22/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=186, maxDeductionDepth=30.
- Einstieg: A an S/8 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[31]`.
- Eine tatsächliche maximale Prämissenkette: `[31, 39, 48, 49, 57, 66, 67, 75, 84, 85, 93, 102, 103, 111, 120, 121, 129, 138, 139, 149, 159, 160, 161, 173, 176, 178, 180, 182, 183, 184]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-02-12, globale Route 18, Gruppe 5: 9×9-Meisterschaft. Lesbarer öffentlicher Einstieg: A an S/8 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 91 konkrete Anschlussfolgen und 55 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 22/81 Zellen, 11 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 30. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐......
01 ┘┌┘......
02 ┌┘.......
03 │........
04 │........
05 └─┐......
06 ..│......
07 ..│......
08 ..└─────┐
```
