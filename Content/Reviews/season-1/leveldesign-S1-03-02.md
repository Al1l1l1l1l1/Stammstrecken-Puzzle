# Author-Leveldesignreview: S1-03-02 (globale Route 10)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Strukturierte Anfangsanker | S1-03-02-01: 6, S1-03-02-02: 6 |
| 3–5 | Zahlenverteilung variieren | S1-03-02-03: 9, S1-03-02-04: 10, S1-03-02-05: 10 |
| 6–8 | Teilräume zusammenführen | S1-03-02-06: 11, S1-03-02-07: 12, S1-03-02-08: 13 |
| 9–10 | Lange asymmetrische Verbindung | S1-03-02-09: 9, S1-03-02-10: 10 |
| 11–12 | Klarer Kompetenzabruf | S1-03-02-11: 19, S1-03-02-12: 19 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-03-02-01

- Input: 6×6; A W/1, B E/5. Zeilen `[0, 4, 3, 3, 3, 5]`, Spalten `[5, 1, 5, 1, 5, 1]`.
- Lösung: 18 Zellen, 6 Kurven; Dichte 18/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=80, maxDeductionDepth=6.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[19, 40, 48, 55, 57, 65]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-02-01, globale Route 10, Gruppe 1: Strukturierte Anfangsanker. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 28 konkrete Anschlussfolgen und 12 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 18/36 Zellen, 6 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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

- Input: 6×6; A N/0, B S/5. Zeilen `[6, 1, 5, 1, 5, 1]`, Spalten `[1, 4, 3, 3, 3, 5]`.
- Lösung: 19 Zellen, 6 Kurven; Dichte 19/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=80, maxDeductionDepth=6.
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[20, 21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[19, 29, 45, 46, 64, 74]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-02-02, globale Route 10, Gruppe 1: Strukturierte Anfangsanker. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 28 konkrete Anschlussfolgen und 17 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/36 Zellen, 6 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └────┐
01 .....│
02 .┌───┘
03 .│....
04 .└───┐
05 .....│
```

## S1-03-02-03

- Input: 6×6; A W/5, B E/0. Zeilen `[5, 6, 2, 2, 2, 2]`, Spalten `[6, 6, 1, 2, 2, 2]`.
- Lösung: 19 Zellen, 16 Kurven; Dichte 19/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=89, maxDeductionDepth=9.
- Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[22, 35, 44, 47, 66, 72, 73, 74, 76]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-02-03, globale Route 10, Gruppe 2: Zahlenverteilung variieren. Lesbarer öffentlicher Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 35 konkrete Anschlussfolgen und 17 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/36 Zellen, 16 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 9. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: A an S/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[14]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 20, 37, 39, 42, 46, 64, 70, 73, 75]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-02-04, globale Route 10, Gruppe 2: Zahlenverteilung variieren. Lesbarer öffentlicher Einstieg: A an S/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 30 konkrete Anschlussfolgen und 23 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 13/36 Zellen, 6 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[26, 27, 28, 29, 30, 31]`.
- Eine tatsächliche maximale Prämissenkette: `[13, 42, 46, 52, 53, 70, 74, 75, 78, 80]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-02-05, globale Route 10, Gruppe 2: Zahlenverteilung variieren. Lesbarer öffentlicher Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 29 konkrete Anschlussfolgen und 12 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 17/36 Zellen, 13 Kurven, 1 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: Zeile 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[25, 26, 27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 20, 39, 40, 53, 58, 75, 78, 79, 82, 85]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-02-06, globale Route 10, Gruppe 3: Teilräume zusammenführen. Lesbarer öffentlicher Einstieg: Zeile 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 34 konkrete Anschlussfolgen und 13 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 13/36 Zellen, 7 Kurven, 2 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: Spalte 5 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 34, 36, 57, 67, 68, 76, 77, 78, 80, 82, 83]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-02-07, globale Route 10, Gruppe 3: Teilräume zusammenführen. Lesbarer öffentlicher Einstieg: Spalte 5 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 44 konkrete Anschlussfolgen und 14 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 20/36 Zellen, 15 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 26, 37, 49, 58, 65, 72, 73, 74, 77, 78, 81, 83]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-02-08, globale Route 10, Gruppe 3: Teilräume zusammenführen. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 35 konkrete Anschlussfolgen und 16 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 14/36 Zellen, 10 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 13. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: Zeile 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[32, 33, 34, 35, 36, 37]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 44, 66, 67, 72, 73, 78, 79, 80]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-02-09, globale Route 10, Gruppe 4: Lange asymmetrische Verbindung. Lesbarer öffentlicher Einstieg: Zeile 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 30 konkrete Anschlussfolgen und 2 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 22/36 Zellen, 18 Kurven, 2 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 9. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[31, 32, 33, 34, 35, 36]`.
- Eine tatsächliche maximale Prämissenkette: `[18, 30, 42, 43, 54, 57, 60, 70, 71, 73]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-02-10, globale Route 10, Gruppe 4: Lange asymmetrische Verbindung. Lesbarer öffentlicher Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 28 konkrete Anschlussfolgen und 16 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 14/36 Zellen, 9 Kurven, 1 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: Zeile 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 29, 31, 40, 49, 50, 54, 59, 60, 64, 68, 69, 71, 72, 73, 79, 81, 83, 84]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-02-11, globale Route 10, Gruppe 5: Klarer Kompetenzabruf. Lesbarer öffentlicher Einstieg: Zeile 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 39 konkrete Anschlussfolgen und 14 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 16/36 Zellen, 7 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 19. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
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
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[20, 21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 29, 33, 36, 37, 44, 48, 51, 52, 55, 59, 62, 68, 72, 73, 75, 85, 86, 88]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-02-12, globale Route 10, Gruppe 5: Klarer Kompetenzabruf. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 44 konkrete Anschlussfolgen und 15 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 21/36 Zellen, 15 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 19. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘┌──┐
01 └─┘..│
02 ....┌┘
03 ...┌┘.
04 .┌┐│..
05 ─┘└┘..
```
