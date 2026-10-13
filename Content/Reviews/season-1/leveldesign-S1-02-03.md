# Author-Leveldesignreview: S1-02-03 (globale Route 07)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Bekannte Sicherheiten | S1-02-03-01: 5, S1-02-03-02: 5 |
| 3–5 | Mittlere Randzahlen | S1-02-03-03: 9, S1-02-03-04: 9, S1-02-03-05: 9 |
| 6–8 | Teilwege durch Folgeausschlüsse | S1-02-03-06: 11, S1-02-03-07: 11, S1-02-03-08: 12 |
| 9–10 | Asymmetrie und Kurven | S1-02-03-09: 8, S1-02-03-10: 9 |
| 11–12 | Anspruchsvolle 6×5-Kette | S1-02-03-11: 12, S1-02-03-12: 16 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-02-03-01

- Input: 5×5; A W/0, B E/3. Zeilen `[4, 3, 3, 4, 0]`, Spalten `[4, 1, 4, 1, 4]`.
- Lösung: 14 Zellen, 6 Kurven; Dichte 14/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=57, maxDeductionDepth=5.
- Einstieg: Zeile 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[16, 17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[15, 28, 31, 39, 45]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-03-01, globale Route 7, Gruppe 1: Bekannte Sicherheiten. Lesbarer öffentlicher Einstieg: Zeile 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 20 konkrete Anschlussfolgen und 6 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 14/25 Zellen, 6 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 5. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.┌─┐
01 │.│.│
02 │.│.│
03 └─┘.└
04 .....
```

## S1-02-03-02

- Input: 5×5; A S/4, B W/4. Zeilen `[0, 0, 0, 2, 5]`, Spalten `[1, 1, 2, 2, 1]`.
- Lösung: 7 Zellen, 5 Kurven; Dichte 7/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=46, maxDeductionDepth=5.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[16, 17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[31, 35, 39, 43, 44]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-03-02, globale Route 7, Gruppe 1: Bekannte Sicherheiten. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 7 konkrete Anschlussfolgen und 3 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 7/25 Zellen, 5 Kurven, 3 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 5. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .....
01 .....
02 .....
03 ..┌┐.
04 ──┘└┐
```

## S1-02-03-03

- Input: 5×5; A S/3, B W/2. Zeilen `[4, 3, 4, 1, 2]`, Spalten `[1, 3, 1, 4, 5]`.
- Lösung: 14 Zellen, 9 Kurven; Dichte 14/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=58, maxDeductionDepth=9.
- Einstieg: Spalte 4 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[20, 21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 16, 38, 45, 49, 50, 52, 53, 56]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-03-03, globale Route 7, Gruppe 2: Mittlere Randzahlen. Lesbarer öffentlicher Einstieg: Spalte 4 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 21 konkrete Anschlussfolgen und 11 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 14/25 Zellen, 9 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 9. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌──┐
01 .│.┌┘
02 ─┘.└┐
03 ....│
04 ...┌┘
```

## S1-02-03-04

- Input: 5×5; A S/1, B N/3. Zeilen `[4, 1, 2, 2, 2]`, Spalten `[3, 4, 3, 1, 0]`.
- Lösung: 11 Zellen, 8 Kurven; Dichte 11/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=56, maxDeductionDepth=9.
- Einstieg: Spalte 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[20, 21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[4, 31, 34, 35, 41, 42, 45, 51, 52]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-03-04, globale Route 7, Gruppe 2: Mittlere Randzahlen. Lesbarer öffentlicher Einstieg: Spalte 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 20 konkrete Anschlussfolgen und 9 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 11/25 Zellen, 8 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 9. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──┘.
01 │....
02 └┐...
03 .└┐..
04 .┌┘..
```

## S1-02-03-05

- Input: 5×5; A W/2, B N/1. Zeilen `[1, 2, 4, 4, 3]`, Spalten `[1, 4, 4, 3, 2]`.
- Lösung: 14 Zellen, 11 Kurven; Dichte 14/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=66, maxDeductionDepth=9.
- Einstieg: A an W/2 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[7]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 16, 27, 28, 41, 48, 53, 54, 58]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-03-05, globale Route 7, Gruppe 2: Mittlere Randzahlen. Lesbarer öffentlicher Einstieg: A an W/2 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 29 konkrete Anschlussfolgen und 11 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 14/25 Zellen, 11 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 9. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .│...
01 .└┐..
02 ─┐└┐.
03 .└┐└┐
04 ..└─┘
```

## S1-02-03-06

- Input: 6×5; A W/3, B S/5. Zeilen `[0, 5, 5, 5, 2]`, Spalten `[1, 3, 3, 2, 4, 4]`.
- Lösung: 17 Zellen, 13 Kurven; Dichte 17/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=78, maxDeductionDepth=11.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[18, 19, 20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[10, 24, 47, 55, 60, 65, 68, 69, 70, 75, 76]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-03-06, globale Route 7, Gruppe 3: Teilwege durch Folgeausschlüsse. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 33 konkrete Anschlussfolgen und 7 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 17/30 Zellen, 13 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ......
01 .┌─┐┌┐
02 .└┐└┘│
03 ──┘.┌┘
04 ....└┐
```

## S1-02-03-07

- Input: 6×5; A E/4, B N/0. Zeilen `[4, 2, 2, 2, 2]`, Spalten `[1, 1, 1, 4, 4, 1]`.
- Lösung: 12 Zellen, 9 Kurven; Dichte 12/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=73, maxDeductionDepth=11.
- Einstieg: A an E/4 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[17]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 18, 26, 27, 41, 48, 49, 56, 63, 64, 71]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-03-07, globale Route 7, Gruppe 3: Teilwege durch Folgeausschlüsse. Lesbarer öffentlicher Einstieg: A an E/4 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 31 konkrete Anschlussfolgen und 18 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 12/30 Zellen, 9 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └──┐..
01 ...└┐.
02 ...┌┘.
03 ...└┐.
04 ....└─
```

## S1-02-03-08

- Input: 6×5; A S/4, B N/4. Zeilen `[1, 4, 2, 1, 5]`, Spalten `[3, 3, 2, 2, 3, 0]`.
- Lösung: 13 Zellen, 6 Kurven; Dichte 13/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=71, maxDeductionDepth=12.
- Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[23, 24, 25, 26]`.
- Eine tatsächliche maximale Prämissenkette: `[4, 21, 27, 32, 40, 42, 48, 55, 56, 63, 65, 67]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-03-08, globale Route 7, Gruppe 3: Teilwege durch Folgeausschlüsse. Lesbarer öffentlicher Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 30 konkrete Anschlussfolgen und 13 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 13/30 Zellen, 6 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....│.
01 .┌──┘.
02 ┌┘....
03 │.....
04 └───┐.
```

## S1-02-03-09

- Input: 6×5; A E/4, B N/1. Zeilen `[6, 6, 1, 1, 1]`, Spalten `[2, 2, 2, 2, 2, 5]`.
- Lösung: 15 Zellen, 11 Kurven; Dichte 15/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=63, maxDeductionDepth=8.
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[18, 19, 20, 21, 22]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 46, 47, 52, 53, 59, 60, 61]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-03-09, globale Route 7, Gruppe 4: Asymmetrie und Kurven. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 17 konkrete Anschlussfolgen und 15 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 15/30 Zellen, 11 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 8. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘┌┐┌┐
01 └─┘└┘│
02 .....│
03 .....│
04 .....└
```

## S1-02-03-10

- Input: 6×5; A S/0, B N/4. Zeilen `[2, 1, 1, 5, 6]`, Spalten `[1, 2, 2, 2, 3, 5]`.
- Lösung: 15 Zellen, 12 Kurven; Dichte 15/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=73, maxDeductionDepth=9.
- Einstieg: Zeile 4 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[18, 19, 20, 21, 22]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 25, 36, 60, 65, 66, 68, 69, 71]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-03-10, globale Route 7, Gruppe 4: Asymmetrie und Kurven. Lesbarer öffentlicher Einstieg: Zeile 4 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 27 konkrete Anschlussfolgen und 15 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 15/30 Zellen, 12 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 9. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....└┐
01 .....│
02 .....│
03 .┌┐┌┐│
04 ┌┘└┘└┘
```

## S1-02-03-11

- Input: 6×5; A N/5, B W/1. Zeilen `[4, 5, 5, 3, 2]`, Spalten `[4, 5, 5, 1, 2, 2]`.
- Lösung: 19 Zellen, 15 Kurven; Dichte 19/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=80, maxDeductionDepth=12.
- Einstieg: Spalte 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[18, 19, 20, 21, 22]`.
- Eine tatsächliche maximale Prämissenkette: `[22, 28, 32, 50, 57, 67, 68, 69, 72, 73, 75, 77]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-03-11, globale Route 7, Gruppe 5: Anspruchsvolle 6×5-Kette. Lesbarer öffentlicher Einstieg: Spalte 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 36 konkrete Anschlussfolgen und 11 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/30 Zellen, 15 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐..│
01 ┘┌┘.┌┘
02 ┌┘┌─┘.
03 └┐│...
04 .└┘...
```

## S1-02-03-12

- Input: 6×5; A W/4, B E/2. Zeilen `[4, 3, 5, 5, 5]`, Spalten `[5, 5, 3, 5, 1, 3]`.
- Lösung: 22 Zellen, 16 Kurven; Dichte 22/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=81, maxDeductionDepth=16.
- Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[18, 19, 20, 21]`.
- Eine tatsächliche maximale Prämissenkette: `[19, 36, 39, 47, 48, 49, 50, 51, 53, 54, 55, 56, 68, 71, 77, 80]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-03-12, globale Route 7, Gruppe 5: Anspruchsvolle 6×5-Kette. Lesbarer öffentlicher Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 39 konkrete Anschlussfolgen und 6 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 22/30 Zellen, 16 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 16. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──┐..
01 └┐.│..
02 ┌┘┌┘.┌
03 └┐└┐.│
04 ─┘.└─┘
```
