# Author-Leveldesignreview: S1-01-03 (globale Route 03)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Randzahlenanker | S1-01-03-01: 4, S1-01-03-02: 5 |
| 3–5 | Angrenzende Endpunktkanten | S1-01-03-03: 6, S1-01-03-04: 6, S1-01-03-05: 6 |
| 6–8 | Belegung und Geometrie | S1-01-03-06: 7, S1-01-03-07: 7, S1-01-03-08: 7 |
| 9–10 | Dichtekontrast | S1-01-03-09: 7, S1-01-03-10: 7 |
| 11–12 | Sichere Ecken und Kette | S1-01-03-11: 9, S1-01-03-12: 12 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-01-03-01

- Input: 4×5; A N/2, B S/3. Zeilen `[1, 1, 1, 2, 3]`, Spalten `[0, 2, 5, 1]`.
- Lösung: 8 Zellen, 4 Kurven; Dichte 8/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=47, maxDeductionDepth=4.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[2, 15, 28, 30]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-03-01, globale Route 3, Gruppe 1: Randzahlenanker. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 15 konkrete Anschlussfolgen und 8 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 8/20 Zellen, 4 Kurven, 1 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 4. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..│.
01 ..│.
02 ..│.
03 .┌┘.
04 .└─┐
```

## S1-01-03-02

- Input: 4×5; A N/1, B E/2. Zeilen `[4, 4, 3, 0, 0]`, Spalten `[3, 3, 2, 3]`.
- Lösung: 11 Zellen, 9 Kurven; Dichte 11/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=45, maxDeductionDepth=5.
- Einstieg: Zeile 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[8, 29, 41, 42, 43]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-03-02, globale Route 3, Gruppe 1: Randzahlenanker. Lesbarer öffentlicher Einstieg: Zeile 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 13 konkrete Anschlussfolgen und 1 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 11/20 Zellen, 9 Kurven, 2 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 5. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘┌┐
01 │┌┘│
02 └┘.└
03 ....
04 ....
```

## S1-01-03-03

- Input: 4×5; A N/0, B W/3. Zeilen `[2, 2, 2, 2, 2]`, Spalten `[5, 5, 0, 0]`.
- Lösung: 10 Zellen, 9 Kurven; Dichte 10/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=43, maxDeductionDepth=6.
- Einstieg: Spalte 2 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[22, 23, 24, 25, 26]`.
- Eine tatsächliche maximale Prämissenkette: `[2, 32, 38, 39, 40, 42]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-03-03, globale Route 3, Gruppe 2: Angrenzende Endpunktkanten. Lesbarer öffentlicher Einstieg: Spalte 2 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 11 konkrete Anschlussfolgen und 0 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 10/20 Zellen, 9 Kurven, 2 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └┐..
01 ┌┘..
02 └┐..
03 ┐│..
04 └┘..
```

## S1-01-03-04

- Input: 4×5; A E/1, B S/0. Zeilen `[0, 1, 4, 4, 2]`, Spalten `[3, 3, 2, 3]`.
- Lösung: 11 Zellen, 9 Kurven; Dichte 11/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=50, maxDeductionDepth=6.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[5, 19, 29, 44, 46, 47]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-03-04, globale Route 3, Gruppe 2: Angrenzende Endpunktkanten. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 18 konkrete Anschlussfolgen und 5 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 11/20 Zellen, 9 Kurven, 1 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....
01 ...┌
02 ┌─┐│
03 └┐└┘
04 ┌┘..
```

## S1-01-03-05

- Input: 4×5; A W/1, B S/2. Zeilen `[2, 4, 1, 1, 2]`, Spalten `[1, 1, 3, 5]`.
- Lösung: 10 Zellen, 5 Kurven; Dichte 10/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=44, maxDeductionDepth=6.
- Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[14, 15, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 21, 29, 32, 33, 35]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-03-05, globale Route 3, Gruppe 2: Angrenzende Endpunktkanten. Lesbarer öffentlicher Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 13 konkrete Anschlussfolgen und 10 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 10/20 Zellen, 5 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌┐
01 ──┘│
02 ...│
03 ...│
04 ..┌┘
```

## S1-01-03-06

- Input: 4×5; A E/2, B W/3. Zeilen `[4, 3, 3, 4, 3]`, Spalten `[3, 5, 4, 5]`.
- Lösung: 17 Zellen, 12 Kurven; Dichte 17/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=55, maxDeductionDepth=7.
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 28, 32, 36, 48, 49, 50]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-03-06, globale Route 3, Gruppe 3: Belegung und Geometrie. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 25 konkrete Anschlussfolgen und 3 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 17/20 Zellen, 12 Kurven, 0 Null- und 4 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──┐
01 └┐.│
02 .└┐└
03 ─┐└┐
04 .└─┘
```

## S1-01-03-07

- Input: 4×5; A S/2, B E/0. Zeilen `[4, 3, 1, 3, 1]`, Spalten `[4, 2, 4, 2]`.
- Lösung: 12 Zellen, 7 Kurven; Dichte 12/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=48, maxDeductionDepth=7.
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[14, 15, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 17, 22, 26, 35, 38, 39]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-03-07, globale Route 3, Gruppe 3: Belegung und Geometrie. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 18 konkrete Anschlussfolgen und 8 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 12/20 Zellen, 7 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐┌
01 │.└┘
02 │...
03 └─┐.
04 ..│.
```

## S1-01-03-08

- Input: 4×5; A E/0, B S/1. Zeilen `[3, 2, 2, 1, 1]`, Spalten `[0, 5, 3, 1]`.
- Lösung: 9 Zellen, 5 Kurven; Dichte 9/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=48, maxDeductionDepth=7.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 25, 30, 38, 39, 44, 45]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-03-08, globale Route 3, Gruppe 3: Belegung und Geometrie. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 17 konkrete Anschlussfolgen und 7 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 9/20 Zellen, 5 Kurven, 1 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌──
01 .└┐.
02 .┌┘.
03 .│..
04 .│..
```

## S1-01-03-09

- Input: 4×5; A S/1, B N/3. Zeilen `[2, 2, 1, 4, 4]`, Spalten `[2, 2, 4, 5]`.
- Lösung: 13 Zellen, 10 Kurven; Dichte 13/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=49, maxDeductionDepth=7.
- Einstieg: Zeile 3 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 23, 32, 33, 38, 39, 41]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-03-09, globale Route 3, Gruppe 4: Dichtekontrast. Lesbarer öffentlicher Einstieg: Zeile 3 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 17 konkrete Anschlussfolgen und 7 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 13/20 Zellen, 10 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌┘
01 ..└┐
02 ...│
03 ┌─┐│
04 └┐└┘
```

## S1-01-03-10

- Input: 4×5; A W/2, B S/2. Zeilen `[3, 4, 3, 2, 1]`, Spalten `[2, 4, 5, 2]`.
- Lösung: 13 Zellen, 11 Kurven; Dichte 13/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=51, maxDeductionDepth=7.
- Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 19, 25, 38, 42, 44, 49]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-03-10, globale Route 3, Gruppe 4: Dichtekontrast. Lesbarer öffentlicher Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 20 konkrete Anschlussfolgen und 6 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 13/20 Zellen, 11 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌─┐
01 ┌┘┌┘
02 ┘┌┘.
03 .└┐.
04 ..│.
```

## S1-01-03-11

- Input: 4×5; A E/4, B N/2. Zeilen `[2, 4, 4, 2, 4]`, Spalten `[4, 4, 4, 4]`.
- Lösung: 16 Zellen, 11 Kurven; Dichte 16/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=56, maxDeductionDepth=9.
- Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[2, 25, 31, 32, 33, 39, 50, 52, 53]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-03-11, globale Route 3, Gruppe 5: Sichere Ecken und Kette. Lesbarer öffentlicher Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 26 konkrete Anschlussfolgen und 3 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 16/20 Zellen, 11 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 9. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└┐
01 ┌─┐│
02 └┐└┘
03 ┌┘..
04 └───
```

## S1-01-03-12

- Input: 4×5; A S/3, B N/1. Zeilen `[2, 1, 2, 2, 2]`, Spalten `[3, 3, 2, 1]`.
- Lösung: 9 Zellen, 8 Kurven; Dichte 9/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=49, maxDeductionDepth=12.
- Einstieg: A an S/3 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[13]`.
- Eine tatsächliche maximale Prämissenkette: `[13, 17, 22, 23, 25, 28, 29, 32, 36, 37, 41, 42]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-03-12, globale Route 3, Gruppe 5: Sichere Ecken und Kette. Lesbarer öffentlicher Einstieg: A an S/3 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 20 konkrete Anschlussfolgen und 11 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 9/20 Zellen, 8 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘..
01 │...
02 └┐..
03 .└┐.
04 ..└┐
```
