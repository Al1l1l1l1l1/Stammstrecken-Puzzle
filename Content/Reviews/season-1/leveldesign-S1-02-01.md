# Author-Leveldesignreview: S1-02-01 (globale Route 05)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Größenübergang | S1-02-01-01: 5, S1-02-01-02: 6 |
| 3–5 | Volle und leere Linien kreuzen | S1-02-01-03: 5, S1-02-01-04: 6, S1-02-01-05: 6 |
| 6–8 | Mehrere Achsen verbinden | S1-02-01-06: 7, S1-02-01-07: 8, S1-02-01-08: 8 |
| 9–10 | Andere Eckzwänge | S1-02-01-09: 7, S1-02-01-10: 8 |
| 11–12 | Mittlere Dichte | S1-02-01-11: 10, S1-02-01-12: 14 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-02-01-01

- Input: 5×5; A S/0, B N/2. Zeilen `[3, 3, 1, 1, 1]`, Spalten `[5, 2, 2, 0, 0]`.
- Lösung: 9 Zellen, 4 Kurven; Dichte 9/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=55, maxDeductionDepth=5.
- Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[24, 25, 26, 27]`.
- Eine tatsächliche maximale Prämissenkette: `[22, 43, 49, 52, 53]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-01-01, globale Route 5, Gruppe 1: Größenübergang. Lesbarer öffentlicher Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 17 konkrete Anschlussfolgen und 8 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 9/25 Zellen, 4 Kurven, 2 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 5. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐│..
01 │└┘..
02 │....
03 │....
04 │....
```

## S1-02-01-02

- Input: 5×5; A E/1, B W/3. Zeilen `[0, 1, 2, 3, 5]`, Spalten `[2, 1, 1, 3, 4]`.
- Lösung: 11 Zellen, 8 Kurven; Dichte 11/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=49, maxDeductionDepth=6.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[16, 17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 24, 35, 41, 44, 48]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-01-02, globale Route 5, Gruppe 1: Größenübergang. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 10 konkrete Anschlussfolgen und 9 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 11/25 Zellen, 8 Kurven, 1 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .....
01 ....┌
02 ...┌┘
03 ┐..└┐
04 └───┘
```

## S1-02-01-03

- Input: 5×5; A N/2, B S/3. Zeilen `[3, 2, 1, 1, 1]`, Spalten `[0, 0, 1, 5, 2]`.
- Lösung: 8 Zellen, 4 Kurven; Dichte 8/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=52, maxDeductionDepth=5.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[2, 28, 35, 36, 48]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-01-03, globale Route 5, Gruppe 2: Volle und leere Linien kreuzen. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 13 konkrete Anschlussfolgen und 9 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 8/25 Zellen, 4 Kurven, 2 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 5. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└─┐
01 ...┌┘
02 ...│.
03 ...│.
04 ...│.
```

## S1-02-01-04

- Input: 5×5; A N/1, B E/3. Zeilen `[4, 5, 3, 1, 0]`, Spalten `[3, 3, 1, 2, 4]`.
- Lösung: 13 Zellen, 9 Kurven; Dichte 13/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=51, maxDeductionDepth=6.
- Einstieg: Zeile 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[25, 26, 27, 28, 29]`.
- Eine tatsächliche maximale Prämissenkette: `[18, 33, 38, 42, 46, 47]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-01-04, globale Route 5, Gruppe 2: Volle und leere Linien kreuzen. Lesbarer öffentlicher Einstieg: Zeile 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 14 konkrete Anschlussfolgen und 6 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 13/25 Zellen, 9 Kurven, 1 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘.┌┐
01 │┌─┘│
02 └┘..│
03 ....└
04 .....
```

## S1-02-01-05

- Input: 5×5; A E/3, B W/4. Zeilen `[0, 0, 3, 5, 2]`, Spalten `[3, 3, 2, 1, 1]`.
- Lösung: 10 Zellen, 6 Kurven; Dichte 10/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=54, maxDeductionDepth=6.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[16, 17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[29, 33, 39, 47, 48, 53]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-01-05, globale Route 5, Gruppe 2: Volle und leere Linien kreuzen. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 15 konkrete Anschlussfolgen und 5 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 10/25 Zellen, 6 Kurven, 2 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .....
01 .....
02 ┌─┐..
03 └┐└──
04 ─┘...
```

## S1-02-01-06

- Input: 5×5; A E/1, B W/3. Zeilen `[3, 3, 5, 4, 2]`, Spalten `[2, 1, 4, 5, 5]`.
- Lösung: 17 Zellen, 14 Kurven; Dichte 17/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=64, maxDeductionDepth=7.
- Einstieg: Zeile 2 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[16, 17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 24, 35, 55, 57, 58, 60]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-01-06, globale Route 5, Gruppe 3: Mehrere Achsen verbinden. Lesbarer öffentlicher Einstieg: Zeile 2 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 25 konkrete Anschlussfolgen und 8 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 17/25 Zellen, 14 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌─┐
01 ..└┐└
02 ┌─┐└┐
03 ┘.└┐│
04 ...└┘
```

## S1-02-01-07

- Input: 5×5; A W/4, B N/1. Zeilen `[1, 3, 3, 1, 4]`, Spalten `[1, 4, 3, 4, 0]`.
- Lösung: 12 Zellen, 5 Kurven; Dichte 12/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=58, maxDeductionDepth=8.
- Einstieg: Spalte 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 18, 25, 47, 49, 54, 55, 56]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-01-07, globale Route 5, Gruppe 3: Mehrere Achsen verbinden. Lesbarer öffentlicher Einstieg: Spalte 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 23 konkrete Anschlussfolgen und 9 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 12/25 Zellen, 5 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 8. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .│...
01 .│┌┐.
02 .└┘│.
03 ...│.
04 ───┘.
```

## S1-02-01-08

- Input: 5×5; A S/3, B E/2. Zeilen `[3, 5, 3, 3, 4]`, Spalten `[5, 5, 4, 2, 2]`.
- Lösung: 18 Zellen, 15 Kurven; Dichte 18/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=66, maxDeductionDepth=8.
- Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[16, 17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 29, 36, 51, 52, 61, 62, 63]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-01-08, globale Route 5, Gruppe 3: Mehrere Achsen verbinden. Lesbarer öffentlicher Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 28 konkrete Anschlussfolgen und 7 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 18/25 Zellen, 15 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 8. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐..
01 └┐└─┐
02 ┌┘..└
03 │┌┐..
04 └┘└┐.
```

## S1-02-01-09

- Input: 5×5; A N/3, B W/3. Zeilen `[3, 1, 2, 3, 2]`, Spalten `[1, 5, 4, 1, 0]`.
- Lösung: 11 Zellen, 7 Kurven; Dichte 11/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=54, maxDeductionDepth=7.
- Einstieg: Spalte 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[29, 30, 31, 32, 33]`.
- Eine tatsächliche maximale Prämissenkette: `[9, 18, 41, 42, 49, 51, 53]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-01-09, globale Route 5, Gruppe 4: Andere Eckzwänge. Lesbarer öffentlicher Einstieg: Spalte 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 20 konkrete Anschlussfolgen und 8 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 11/25 Zellen, 7 Kurven, 1 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌─┘.
01 .│...
02 .└┐..
03 ─┐│..
04 .└┘..
```

## S1-02-01-10

- Input: 5×5; A E/3, B W/0. Zeilen `[1, 3, 1, 2, 3]`, Spalten `[2, 1, 4, 1, 2]`.
- Lösung: 10 Zellen, 6 Kurven; Dichte 10/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=52, maxDeductionDepth=8.
- Einstieg: A an E/3 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[10]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 16, 24, 25, 37, 46, 47, 48]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-01-10, globale Route 5, Gruppe 4: Andere Eckzwänge. Lesbarer öffentlicher Einstieg: A an E/3 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 20 konkrete Anschlussfolgen und 12 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 10/25 Zellen, 6 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 8. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐....
01 └─┐..
02 ..│..
03 ..│.┌
04 ..└─┘
```

## S1-02-01-11

- Input: 5×5; A N/0, B S/3. Zeilen `[5, 1, 5, 1, 4]`, Spalten `[4, 3, 3, 3, 3]`.
- Lösung: 16 Zellen, 6 Kurven; Dichte 16/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=57, maxDeductionDepth=10.
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[16, 17, 18, 19]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 25, 29, 31, 33, 35, 38, 39, 47, 53]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-01-11, globale Route 5, Gruppe 5: Mittlere Dichte. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 22 konkrete Anschlussfolgen und 7 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 16/25 Zellen, 6 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └───┐
01 ....│
02 ┌───┘
03 │....
04 └──┐.
```

## S1-02-01-12

- Input: 5×5; A W/2, B N/3. Zeilen `[1, 1, 3, 4, 3]`, Spalten `[3, 1, 2, 4, 2]`.
- Lösung: 12 Zellen, 7 Kurven; Dichte 12/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=60, maxDeductionDepth=14.
- Einstieg: A an W/2 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[7]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 18, 20, 24, 26, 31, 39, 41, 46, 47, 52, 54, 55, 57]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-01-12, globale Route 5, Gruppe 5: Mittlere Dichte. Lesbarer öffentlicher Einstieg: A an W/2 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 25 konkrete Anschlussfolgen und 13 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 12/25 Zellen, 7 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 14. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...│.
01 ...│.
02 ┐..└┐
03 │.┌─┘
04 └─┘..
```
