# Author-Leveldesignreview: S1-04-02 (globale Route 14)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Deutlicher Anfang | S1-04-02-01: 6, S1-04-02-02: 7 |
| 3–5 | Mittlere Randzahlen | S1-04-02-03: 13, S1-04-02-04: 17, S1-04-02-05: 18 |
| 6–8 | Kombination statt einzelner Voll-/Nullzahl | S1-04-02-06: 19, S1-04-02-07: 20, S1-04-02-08: 22 |
| 9–10 | Ungleichmäßige Dichte und Entfernung | S1-04-02-09: 12, S1-04-02-10: 13 |
| 11–12 | Lange Schlusskette | S1-04-02-11: 24, S1-04-02-12: 26 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-04-02-01

- Input: 7×7; A N/3, B W/3. Zeilen `[3, 1, 1, 3, 3, 0, 0]`, Spalten `[2, 5, 3, 1, 0, 0, 0]`.
- Lösung: 11 Zellen, 7 Kurven; Dichte 11/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=87, maxDeductionDepth=6.
- Einstieg: Zeile 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[24, 25, 26, 27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 43, 62, 69, 70, 78]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-02-01, globale Route 14, Gruppe 1: Deutlicher Anfang. Lesbarer öffentlicher Einstieg: Zeile 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 17 konkrete Anschlussfolgen und 9 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 11/49 Zellen, 7 Kurven, 5 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌─┘...
01 .│.....
02 .│.....
03 ┐└┐....
04 └─┘....
05 .......
06 .......
```

## S1-04-02-02

- Input: 7×7; A W/0, B E/0. Zeilen `[5, 4, 4, 4, 4, 5, 3]`, Spalten `[7, 1, 7, 1, 6, 1, 6]`.
- Lösung: 29 Zellen, 8 Kurven; Dichte 29/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=110, maxDeductionDepth=7.
- Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[24, 25, 26, 27, 28, 29]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 38, 43, 61, 62, 82, 93]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-02-02, globale Route 14, Gruppe 1: Deutlicher Anfang. Lesbarer öffentlicher Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 42 konkrete Anschlussfolgen und 20 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 29/49 Zellen, 8 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.┌─┐.┌
01 │.│.│.│
02 │.│.│.│
03 │.│.│.│
04 │.│.│.│
05 │.│.└─┘
06 └─┘....
```

## S1-04-02-03

- Input: 7×7; A N/0, B S/1. Zeilen `[5, 1, 4, 1, 6, 1, 6]`, Spalten `[1, 5, 4, 4, 5, 2, 3]`.
- Lösung: 24 Zellen, 8 Kurven; Dichte 24/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=105, maxDeductionDepth=13.
- Einstieg: A an N/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[0]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 27, 42, 49, 55, 57, 59, 74, 82, 87, 90, 92, 96]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-02-03, globale Route 14, Gruppe 2: Mittlere Randzahlen. Lesbarer öffentlicher Einstieg: A an N/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 44 konkrete Anschlussfolgen und 19 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 24/49 Zellen, 8 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 13. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └───┐..
01 ....│..
02 .┌──┘..
03 .│.....
04 .└────┐
05 ......│
06 .┌────┘
```

## S1-04-02-04

- Input: 7×7; A E/1, B S/3. Zeilen `[3, 5, 6, 3, 1, 2, 1]`, Spalten `[4, 2, 6, 4, 2, 1, 2]`.
- Lösung: 21 Zellen, 13 Kurven; Dichte 21/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=116, maxDeductionDepth=17.
- Einstieg: A an E/1 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[8]`.
- Eine tatsächliche maximale Prämissenkette: `[20, 26, 42, 43, 50, 55, 59, 81, 101, 102, 104, 107, 108, 111, 112, 113, 115]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-02-04, globale Route 14, Gruppe 2: Mittlere Randzahlen. Lesbarer öffentlicher Einstieg: A an E/1 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 50 konkrete Anschlussfolgen und 27 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 21/49 Zellen, 13 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 17. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐....
01 │.│┌┐.┌
02 │.└┘└─┘
03 └─┐....
04 ..│....
05 ..└┐...
06 ...│...
```

## S1-04-02-05

- Input: 7×7; A W/0, B E/6. Zeilen `[5, 3, 1, 1, 1, 2, 2]`, Spalten `[1, 1, 1, 1, 2, 3, 6]`.
- Lösung: 15 Zellen, 6 Kurven; Dichte 15/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=117, maxDeductionDepth=18.
- Einstieg: A an W/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[0]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 24, 30, 31, 38, 44, 45, 52, 58, 59, 66, 72, 73, 85, 95, 96, 105, 111]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-02-05, globale Route 14, Gruppe 2: Mittlere Randzahlen. Lesbarer öffentlicher Einstieg: A an W/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 50 konkrete Anschlussfolgen und 34 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 15/49 Zellen, 6 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 18. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ────┐..
01 ....└─┐
02 ......│
03 ......│
04 ......│
05 .....┌┘
06 .....└─
```

## S1-04-02-06

- Input: 7×7; A E/3, B S/1. Zeilen `[5, 2, 3, 6, 2, 2, 1]`, Spalten `[6, 5, 1, 3, 4, 1, 1]`.
- Lösung: 21 Zellen, 11 Kurven; Dichte 21/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=120, maxDeductionDepth=19.
- Einstieg: A an E/3 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[12]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 38, 46, 55, 64, 76, 79, 83, 84, 86, 101, 105, 106, 108, 109, 111, 113, 115, 116]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-02-06, globale Route 14, Gruppe 3: Kombination statt einzelner Voll-/Nullzahl. Lesbarer öffentlicher Einstieg: A an E/3 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 57 konkrete Anschlussfolgen und 27 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 21/49 Zellen, 11 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 19. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌───┐..
01 │...│..
02 │..┌┘..
03 └┐.└───
04 ┌┘.....
05 └┐.....
06 .│.....
```

## S1-04-02-07

- Input: 7×7; A S/0, B N/6. Zeilen `[2, 4, 6, 2, 3, 1, 1]`, Spalten `[3, 4, 4, 1, 1, 3, 3]`.
- Lösung: 19 Zellen, 12 Kurven; Dichte 19/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=117, maxDeductionDepth=20.
- Einstieg: A an S/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[17]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 24, 36, 37, 38, 50, 51, 54, 64, 69, 91, 92, 94, 98, 100, 102, 105, 110, 112, 113]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-02-07, globale Route 14, Gruppe 3: Kombination statt einzelner Voll-/Nullzahl. Lesbarer öffentlicher Einstieg: A an S/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 52 konkrete Anschlussfolgen und 28 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/49 Zellen, 12 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 20. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .....┌┘
01 .┌┐..└┐
02 .│└───┘
03 .└┐....
04 ┌─┘....
05 │......
06 │......
```

## S1-04-02-08

- Input: 7×7; A N/4, B W/1. Zeilen `[1, 4, 4, 4, 4, 3, 2]`, Spalten `[1, 1, 4, 1, 6, 4, 5]`.
- Lösung: 22 Zellen, 11 Kurven; Dichte 22/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=122, maxDeductionDepth=22.
- Einstieg: A an N/4 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[4]`.
- Eine tatsächliche maximale Prämissenkette: `[4, 24, 36, 37, 48, 53, 54, 62, 67, 77, 82, 93, 94, 95, 97, 103, 104, 113, 114, 116, 119, 120]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-02-08, globale Route 14, Gruppe 3: Kombination statt einzelner Voll-/Nullzahl. Lesbarer öffentlicher Einstieg: A an N/4 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 61 konkrete Anschlussfolgen und 27 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 22/49 Zellen, 11 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 22. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....│..
01 ──┐.│..
02 ..│.│┌┐
03 ..│.└┘│
04 ..└─┐.│
05 ....└┐│
06 .....└┘
```

## S1-04-02-09

- Input: 7×7; A S/6, B S/0. Zeilen `[0, 0, 0, 0, 2, 7, 6]`, Spalten `[2, 2, 2, 2, 2, 2, 3]`.
- Lösung: 15 Zellen, 12 Kurven; Dichte 15/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=96, maxDeductionDepth=12.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[24, 25, 26, 27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 59, 67, 70, 71, 83, 84, 88, 89, 92, 93, 94]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-02-09, globale Route 14, Gruppe 4: Ungleichmäßige Dichte und Entfernung. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 26 konkrete Anschlussfolgen und 6 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 15/49 Zellen, 12 Kurven, 4 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .......
01 .......
02 .......
03 .......
04 .....┌┐
05 ┌┐┌┐┌┘│
06 │└┘└┘.│
```

## S1-04-02-10

- Input: 7×7; A S/2, B N/2. Zeilen `[1, 2, 1, 2, 1, 3, 3]`, Spalten `[0, 0, 4, 5, 4, 0, 0]`.
- Lösung: 13 Zellen, 8 Kurven; Dichte 13/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=101, maxDeductionDepth=13.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[2, 25, 54, 55, 75, 81, 84, 85, 86, 87, 89, 93, 94]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-02-10, globale Route 14, Gruppe 4: Ungleichmäßige Dichte und Entfernung. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 33 konkrete Anschlussfolgen und 12 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 13/49 Zellen, 8 Kurven, 4 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 13. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..│....
01 ..└┐...
02 ...│...
03 ...└┐..
04 ....│..
05 ..┌┐│..
06 ..│└┘..
```

## S1-04-02-11

- Input: 7×7; A S/2, B N/0. Zeilen `[1, 1, 5, 6, 5, 6, 7]`, Spalten `[6, 5, 5, 4, 3, 3, 5]`.
- Lösung: 31 Zellen, 22 Kurven; Dichte 31/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=122, maxDeductionDepth=24.
- Einstieg: Zeile 6 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 24, 36, 37, 48, 65, 66, 76, 81, 85, 87, 91, 95, 97, 98, 99, 103, 104, 105, 106, 107, 112, 116, 120]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-02-11, globale Route 14, Gruppe 5: Lange Schlusskette. Lesbarer öffentlicher Einstieg: Zeile 6 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 58 konkrete Anschlussfolgen und 18 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 31/49 Zellen, 22 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 24. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 │......
01 │......
02 └─┐..┌┐
03 .┌┘┌─┘│
04 ┌┘┌┘..│
05 │┌┘┌┐.│
06 └┘┌┘└─┘
```

## S1-04-02-12

- Input: 7×7; A W/0, B S/6. Zeilen `[1, 2, 2, 1, 3, 3, 3]`, Spalten `[2, 2, 3, 1, 3, 2, 2]`.
- Lösung: 15 Zellen, 11 Kurven; Dichte 15/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=119, maxDeductionDepth=26.
- Einstieg: A an W/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[0]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 24, 30, 31, 38, 43, 44, 50, 55, 56, 62, 66, 67, 72, 76, 77, 82, 89, 90, 95, 97, 98, 101, 109, 110, 117]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-02-12, globale Route 14, Gruppe 5: Lange Schlusskette. Lesbarer öffentlicher Einstieg: A an W/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 55 konkrete Anschlussfolgen und 34 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 15/49 Zellen, 11 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 26. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐......
01 └┐.....
02 .└┐....
03 ..│....
04 ..└─┐..
05 ....│┌┐
06 ....└┘│
```
