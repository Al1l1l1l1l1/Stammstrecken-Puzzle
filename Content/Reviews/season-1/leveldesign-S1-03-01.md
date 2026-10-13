# Author-Leveldesignreview: S1-03-01 (globale Route 09)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Deutliche Randzahlenanker | S1-03-01-01: 6, S1-03-01-02: 6 |
| 3–5 | Ungünstigere A/B-Lage | S1-03-01-03: 9, S1-03-01-04: 10, S1-03-01-05: 10 |
| 6–8 | Lokale Ketten wirken entfernt | S1-03-01-06: 11, S1-03-01-07: 12, S1-03-01-08: 13 |
| 9–10 | Weniger Extremwerte | S1-03-01-09: 11, S1-03-01-10: 13 |
| 11–12 | Faire 6×6-Höhepunkte | S1-03-01-11: 22, S1-03-01-12: 23 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-03-01-01

- Input: 6×6; A W/1, B E/4. Zeilen `[0, 1, 4, 3, 5, 0]`, Spalten `[4, 1, 3, 1, 3, 1]`.
- Lösung: 13 Zellen, 6 Kurven; Dichte 13/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=77, maxDeductionDepth=6.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 29, 43, 61, 64, 68]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-01-01, globale Route 9, Gruppe 1: Deutliche Randzahlenanker. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 25 konkrete Anschlussfolgen und 11 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 13/36 Zellen, 6 Kurven, 2 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ......
01 ┐.....
02 │.┌─┐.
03 │.│.│.
04 └─┘.└─
05 ......
```

## S1-03-01-02

- Input: 6×6; A E/2, B N/0. Zeilen `[6, 5, 1, 0, 0, 0]`, Spalten `[2, 2, 1, 2, 2, 3]`.
- Lösung: 12 Zellen, 9 Kurven; Dichte 12/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=69, maxDeductionDepth=6.
- Einstieg: Zeile 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 50, 54, 55, 66, 67]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-01-02, globale Route 9, Gruppe 1: Deutliche Randzahlenanker. Lesbarer öffentlicher Einstieg: Zeile 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 15 konkrete Anschlussfolgen und 6 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 12/36 Zellen, 9 Kurven, 3 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 │┌─┐┌┐
01 └┘.└┘│
02 .....└
03 ......
04 ......
05 ......
```

## S1-03-01-03

- Input: 6×6; A N/4, B W/4. Zeilen `[4, 5, 2, 2, 3, 3]`, Spalten `[6, 6, 4, 1, 2, 0]`.
- Lösung: 19 Zellen, 15 Kurven; Dichte 19/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=89, maxDeductionDepth=9.
- Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[31, 32, 33, 34, 35, 36]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 52, 63, 68, 77, 78, 85, 86, 87]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-01-03, globale Route 9, Gruppe 2: Ungünstigere A/B-Lage. Lesbarer öffentlicher Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 35 konkrete Anschlussfolgen und 11 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/36 Zellen, 15 Kurven, 1 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 9. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐.│.
01 └┐└─┘.
02 ┌┘....
03 └┐....
04 ┐└┐...
05 └─┘...
```

## S1-03-01-04

- Input: 6×6; A S/0, B N/4. Zeilen `[1, 1, 2, 1, 3, 6]`, Spalten `[1, 1, 1, 2, 5, 4]`.
- Lösung: 14 Zellen, 8 Kurven; Dichte 14/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=78, maxDeductionDepth=10.
- Einstieg: Zeile 5 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[25, 26, 27, 28, 29]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 33, 57, 60, 68, 70, 71, 74, 75, 77]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-01-04, globale Route 9, Gruppe 2: Ungünstigere A/B-Lage. Lesbarer öffentlicher Einstieg: Zeile 5 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 28 konkrete Anschlussfolgen und 21 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 14/36 Zellen, 8 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....│.
01 ....│.
02 ....└┐
03 .....│
04 ...┌┐│
05 ┌──┘└┘
```

## S1-03-01-05

- Input: 6×6; A N/2, B S/4. Zeilen `[2, 2, 2, 4, 4, 6]`, Spalten `[5, 5, 2, 3, 3, 2]`.
- Lösung: 20 Zellen, 16 Kurven; Dichte 20/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=91, maxDeductionDepth=10.
- Einstieg: Zeile 5 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[20, 21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[19, 39, 52, 57, 58, 63, 64, 84, 87, 89]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-01-05, globale Route 9, Gruppe 2: Ungünstigere A/B-Lage. Lesbarer öffentlicher Einstieg: Zeile 5 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 41 konkrete Anschlussfolgen und 16 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 20/36 Zellen, 16 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌┘...
01 ┌┘....
02 └┐....
03 ┌┘.┌┐.
04 │..│└┐
05 └──┘┌┘
```

## S1-03-01-06

- Input: 6×6; A N/0, B S/3. Zeilen `[3, 4, 5, 1, 3, 1]`, Spalten `[3, 1, 1, 4, 3, 5]`.
- Lösung: 17 Zellen, 8 Kurven; Dichte 17/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=85, maxDeductionDepth=11.
- Einstieg: A an N/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[0]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 24, 25, 30, 43, 57, 62, 75, 78, 79, 80]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-01-06, globale Route 9, Gruppe 3: Lokale Ketten wirken entfernt. Lesbarer öffentlicher Einstieg: A an N/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 36 konkrete Anschlussfolgen und 19 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 17/36 Zellen, 8 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 │...┌┐
01 │..┌┘│
02 └──┘.│
03 .....│
04 ...┌─┘
05 ...│..
```

## S1-03-01-07

- Input: 6×6; A S/2, B N/3. Zeilen `[3, 1, 1, 6, 2, 2]`, Spalten `[2, 3, 2, 2, 2, 4]`.
- Lösung: 15 Zellen, 8 Kurven; Dichte 15/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=76, maxDeductionDepth=12.
- Einstieg: Zeile 3 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[16, 26, 35, 36, 53, 56, 57, 58, 59, 70, 73, 74]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-01-07, globale Route 9, Gruppe 3: Lokale Ketten wirken entfernt. Lesbarer öffentlicher Einstieg: Zeile 3 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 29 konkrete Anschlussfolgen und 20 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 15/36 Zellen, 8 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...└─┐
01 .....│
02 .....│
03 ┌────┘
04 └┐....
05 .└┐...
```

## S1-03-01-08

- Input: 6×6; A S/5, B W/1. Zeilen `[3, 3, 6, 5, 4, 1]`, Spalten `[4, 5, 5, 3, 1, 4]`.
- Lösung: 22 Zellen, 15 Kurven; Dichte 22/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=86, maxDeductionDepth=13.
- Einstieg: Zeile 2 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[19, 27, 32, 58, 61, 64, 66, 72, 73, 76, 77, 81, 85]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-01-08, globale Route 9, Gruppe 3: Lokale Ketten wirken entfernt. Lesbarer öffentlicher Einstieg: Zeile 2 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 37 konkrete Anschlussfolgen und 11 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 22/36 Zellen, 15 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 13. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐...
01 ┘┌┘...
02 ┌┘┌──┐
03 └┐└┐.│
04 .└─┘.│
05 .....│
```

## S1-03-01-09

- Input: 6×6; A E/1, B S/2. Zeilen `[2, 3, 2, 2, 2, 1]`, Spalten `[0, 0, 4, 5, 2, 1]`.
- Lösung: 12 Zellen, 9 Kurven; Dichte 12/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=80, maxDeductionDepth=11.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[25, 26, 27, 28, 29]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 40, 47, 56, 58, 60, 61, 62, 64, 69, 70]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-01-09, globale Route 9, Gruppe 4: Weniger Extremwerte. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 30 konkrete Anschlussfolgen und 13 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 12/36 Zellen, 9 Kurven, 2 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...┌┐.
01 ...│└─
02 ..┌┘..
03 ..└┐..
04 ..┌┘..
05 ..│...
```

## S1-03-01-10

- Input: 6×6; A N/1, B S/4. Zeilen `[5, 1, 5, 1, 4, 1]`, Spalten `[0, 4, 3, 3, 4, 3]`.
- Lösung: 17 Zellen, 6 Kurven; Dichte 17/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=81, maxDeductionDepth=13.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[25, 26, 27, 28, 29]`.
- Eine tatsächliche maximale Prämissenkette: `[18, 23, 38, 39, 49, 53, 55, 57, 59, 62, 63, 71, 77]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-01-10, globale Route 9, Gruppe 4: Weniger Extremwerte. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 32 konkrete Anschlussfolgen und 12 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 17/36 Zellen, 6 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 13. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .└───┐
01 .....│
02 .┌───┘
03 .│....
04 .└──┐.
05 ....│.
```

## S1-03-01-11

- Input: 6×6; A N/3, B E/4. Zeilen `[4, 2, 1, 2, 4, 2]`, Spalten `[2, 4, 4, 3, 1, 1]`.
- Lösung: 15 Zellen, 9 Kurven; Dichte 15/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=88, maxDeductionDepth=22.
- Einstieg: A an N/3 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[3]`.
- Eine tatsächliche maximale Prämissenkette: `[13, 23, 29, 31, 32, 44, 48, 49, 53, 54, 59, 61, 66, 67, 68, 69, 74, 75, 78, 81, 85, 87]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-01-11, globale Route 9, Gruppe 5: Faire 6×6-Höhepunkte. Lesbarer öffentlicher Einstieg: A an N/3 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 40 konkrete Anschlussfolgen und 21 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 15/36 Zellen, 9 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 22. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──┘..
01 └┐....
02 .│....
03 .└┐...
04 ..│┌──
05 ..└┘..
```

## S1-03-01-12

- Input: 6×6; A E/0, B W/4. Zeilen `[3, 2, 1, 2, 2, 2]`, Spalten `[2, 3, 3, 2, 1, 1]`.
- Lösung: 12 Zellen, 8 Kurven; Dichte 12/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=86, maxDeductionDepth=23.
- Einstieg: A an E/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[5]`.
- Eine tatsächliche maximale Prämissenkette: `[5, 20, 25, 31, 32, 37, 43, 46, 47, 51, 52, 56, 61, 63, 64, 67, 69, 73, 76, 77, 79, 83, 84]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-01-12, globale Route 9, Gruppe 5: Faire 6×6-Höhepunkte. Lesbarer öffentlicher Einstieg: A an E/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 38 konkrete Anschlussfolgen und 24 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 12/36 Zellen, 8 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 23. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...┌──
01 ..┌┘..
02 ..│...
03 .┌┘...
04 ┐│....
05 └┘....
```
