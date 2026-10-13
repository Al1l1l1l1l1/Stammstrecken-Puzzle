# Author-Leveldesignreview: S1-02-04 (globale Route 08)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Größere Informationsmenge | S1-02-04-01: 5, S1-02-04-02: 6 |
| 3–5 | Weit getrennte Bereiche | S1-02-04-03: 7, S1-02-04-04: 8, S1-02-04-05: 8 |
| 6–8 | Kreuzschlussketten | S1-02-04-06: 8, S1-02-04-07: 9, S1-02-04-08: 9 |
| 9–10 | Hohe Dichte | S1-02-04-09: 10, S1-02-04-10: 11 |
| 11–12 | Lange aufgebaute Verbindung | S1-02-04-11: 14, S1-02-04-12: 18 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-02-04-01

- Input: 6×5; A N/0, B S/5. Zeilen `[5, 1, 5, 1, 6]`, Spalten `[4, 3, 3, 3, 4, 1]`.
- Lösung: 18 Zellen, 6 Kurven; Dichte 18/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=68, maxDeductionDepth=5.
- Einstieg: Zeile 4 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[18, 19, 20, 21, 22]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 25, 39, 48, 53]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-04-01, globale Route 8, Gruppe 1: Größere Informationsmenge. Lesbarer öffentlicher Einstieg: Zeile 4 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 23 konkrete Anschlussfolgen und 12 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 18/30 Zellen, 6 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 5. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └───┐.
01 ....│.
02 ┌───┘.
03 │.....
04 └────┐
```

## S1-02-04-02

- Input: 6×5; A E/4, B N/3. Zeilen `[3, 3, 1, 1, 1]`, Spalten `[0, 0, 0, 2, 2, 5]`.
- Lösung: 9 Zellen, 5 Kurven; Dichte 9/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=62, maxDeductionDepth=6.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[23, 24, 25, 26]`.
- Eine tatsächliche maximale Prämissenkette: `[33, 43, 54, 58, 59, 60]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-04-02, globale Route 8, Gruppe 1: Größere Informationsmenge. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 17 konkrete Anschlussfolgen und 9 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 9/30 Zellen, 5 Kurven, 3 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...│┌┐
01 ...└┘│
02 .....│
03 .....│
04 .....└
```

## S1-02-04-03

- Input: 6×5; A S/5, B W/3. Zeilen `[0, 0, 0, 4, 5]`, Spalten `[2, 2, 1, 2, 1, 1]`.
- Lösung: 9 Zellen, 7 Kurven; Dichte 9/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=57, maxDeductionDepth=7.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[18, 19, 20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 36, 42, 43, 47, 48, 53]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-04-03, globale Route 8, Gruppe 2: Weit getrennte Bereiche. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 13 konkrete Anschlussfolgen und 3 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 9/30 Zellen, 7 Kurven, 3 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ......
01 ......
02 ......
03 ┐┌─┐..
04 └┘.└─┐
```

## S1-02-04-04

- Input: 6×5; A E/2, B N/1. Zeilen `[2, 4, 4, 6, 3]`, Spalten `[4, 2, 1, 4, 4, 4]`.
- Lösung: 19 Zellen, 13 Kurven; Dichte 19/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=70, maxDeductionDepth=8.
- Einstieg: Zeile 3 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[18, 19, 20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 24, 31, 42, 48, 59, 61, 64]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-04-04, globale Route 8, Gruppe 2: Weit getrennte Bereiche. Lesbarer öffentlicher Einstieg: Zeile 3 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 28 konkrete Anschlussfolgen und 10 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/30 Zellen, 13 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 8. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘....
01 │..┌─┐
02 │..└┐└
03 └──┐└┐
04 ...└─┘
```

## S1-02-04-05

- Input: 6×5; A S/2, B N/0. Zeilen `[2, 2, 2, 2, 1]`, Spalten `[3, 4, 2, 0, 0, 0]`.
- Lösung: 9 Zellen, 8 Kurven; Dichte 9/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=63, maxDeductionDepth=8.
- Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 19, 23, 47, 53, 59, 60, 61]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-04-05, globale Route 8, Gruppe 2: Weit getrennte Bereiche. Lesbarer öffentlicher Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 18 konkrete Anschlussfolgen und 9 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 9/30 Zellen, 8 Kurven, 3 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 8. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └┐....
01 ┌┘....
02 └┐....
03 .└┐...
04 ..│...
```

## S1-02-04-06

- Input: 6×5; A W/1, B S/4. Zeilen `[6, 5, 2, 2, 1]`, Spalten `[2, 2, 2, 1, 5, 4]`.
- Lösung: 16 Zellen, 13 Kurven; Dichte 16/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=70, maxDeductionDepth=8.
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[18, 19, 20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 31, 41, 42, 46, 47, 51, 58]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-04-06, globale Route 8, Gruppe 3: Kreuzschlussketten. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 24 konkrete Anschlussfolgen und 14 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 16/30 Zellen, 13 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 8. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐┌──┐
01 ┘└┘.┌┘
02 ....└┐
03 ....┌┘
04 ....│.
```

## S1-02-04-07

- Input: 6×5; A W/2, B E/2. Zeilen `[0, 6, 6, 0, 0]`, Spalten `[2, 2, 2, 2, 2, 2]`.
- Lösung: 12 Zellen, 12 Kurven; Dichte 12/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=67, maxDeductionDepth=9.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[18, 19, 20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 46, 52, 53, 58, 59, 62, 63, 65]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-04-07, globale Route 8, Gruppe 3: Kreuzschlussketten. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 21 konkrete Anschlussfolgen und 0 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 12/30 Zellen, 12 Kurven, 3 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 9. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ......
01 ┌┐┌┐┌┐
02 ┘└┘└┘└
03 ......
04 ......
```

## S1-02-04-08

- Input: 6×5; A N/2, B S/2. Zeilen `[3, 2, 2, 4, 4]`, Spalten `[0, 2, 3, 5, 5, 0]`.
- Lösung: 15 Zellen, 12 Kurven; Dichte 15/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=73, maxDeductionDepth=9.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[18, 19, 20, 21, 22]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 38, 59, 60, 61, 62, 64, 67, 72]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-04-08, globale Route 8, Gruppe 3: Kreuzschlussketten. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 27 konkrete Anschlussfolgen und 5 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 15/30 Zellen, 12 Kurven, 2 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 9. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└─┐.
01 ...┌┘.
02 ...└┐.
03 .┌─┐│.
04 .└┐└┘.
```

## S1-02-04-09

- Input: 6×5; A E/0, B S/2. Zeilen `[4, 5, 4, 2, 1]`, Spalten `[3, 4, 4, 2, 2, 1]`.
- Lösung: 16 Zellen, 13 Kurven; Dichte 16/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=75, maxDeductionDepth=10.
- Einstieg: A an E/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[5]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 19, 37, 39, 44, 47, 51, 60, 67, 68]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-04-09, globale Route 8, Gruppe 4: Hohe Dichte. Lesbarer öffentlicher Einstieg: A an E/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 31 konkrete Anschlussfolgen und 14 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 16/30 Zellen, 13 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐..┌─
01 │└┐┌┘.
02 └┐└┘..
03 .└┐...
04 ..│...
```

## S1-02-04-10

- Input: 6×5; A W/1, B S/1. Zeilen `[5, 5, 3, 4, 4]`, Spalten `[5, 5, 4, 5, 2, 0]`.
- Lösung: 21 Zellen, 17 Kurven; Dichte 21/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=74, maxDeductionDepth=11.
- Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 38, 41, 44, 45, 52, 54, 56, 57, 66, 68]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-04-10, globale Route 8, Gruppe 4: Hohe Dichte. Lesbarer öffentlicher Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 30 konkrete Anschlussfolgen und 4 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 21/30 Zellen, 17 Kurven, 1 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐┌─┐.
01 ┘└┘┌┘.
02 ┌┐.│..
03 │└┐│..
04 └┐└┘..
```

## S1-02-04-11

- Input: 6×5; A E/1, B W/3. Zeilen `[4, 3, 6, 4, 3]`, Spalten `[2, 1, 5, 4, 4, 4]`.
- Lösung: 20 Zellen, 14 Kurven; Dichte 20/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=75, maxDeductionDepth=14.
- Einstieg: Zeile 2 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[18, 19, 20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[10, 24, 47, 49, 50, 51, 54, 55, 57, 61, 62, 69, 70, 74]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-04-11, globale Route 8, Gruppe 5: Lange aufgebaute Verbindung. Lesbarer öffentlicher Einstieg: Zeile 2 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 33 konkrete Anschlussfolgen und 10 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 20/30 Zellen, 14 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 14. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌──┐
01 ..└┐.└
02 ┌─┐└─┐
03 ┘.│.┌┘
04 ..└─┘.
```

## S1-02-04-12

- Input: 6×5; A E/0, B N/1. Zeilen `[4, 4, 3, 6, 6]`, Spalten `[4, 5, 2, 2, 5, 5]`.
- Lösung: 23 Zellen, 19 Kurven; Dichte 23/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=82, maxDeductionDepth=18.
- Einstieg: Zeile 3 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[18, 19, 20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 35, 44, 45, 48, 58, 59, 60, 62, 63, 64, 66, 68, 69, 71, 73, 74, 79]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-04-12, globale Route 8, Gruppe 5: Lange aufgebaute Verbindung. Lesbarer öffentlicher Einstieg: Zeile 3 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 36 konkrete Anschlussfolgen und 7 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 23/30 Zellen, 19 Kurven, 0 Null- und 5 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 18. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘..┌─
01 └┐..└┐
02 .│..┌┘
03 ┌┘┌┐└┐
04 └─┘└─┘
```
