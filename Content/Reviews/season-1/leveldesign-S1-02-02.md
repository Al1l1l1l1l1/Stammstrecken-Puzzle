# Author-Leveldesignreview: S1-02-02 (globale Route 06)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Sichere Belegung | S1-02-02-01: 5, S1-02-02-02: 6 |
| 3–5 | Dichtere Struktur | S1-02-02-03: 8, S1-02-02-04: 9, S1-02-02-05: 9 |
| 6–8 | Mehrere Achsen | S1-02-02-06: 7, S1-02-02-07: 8, S1-02-02-08: 8 |
| 9–10 | Lange Verbindung bei begrenzter Dichte | S1-02-02-09: 7, S1-02-02-10: 8 |
| 11–12 | Kompakte Verdichtung | S1-02-02-11: 10, S1-02-02-12: 14 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-02-02-01

- Input: 5×5; A N/0, B S/4. Zeilen `[5, 1, 4, 1, 4]`, Spalten `[1, 4, 3, 3, 4]`.
- Lösung: 15 Zellen, 6 Kurven; Dichte 15/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=58, maxDeductionDepth=5.
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[16, 17, 18, 19]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 21, 36, 43, 51]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-02-01, globale Route 6, Gruppe 1: Sichere Belegung. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 20 konkrete Anschlussfolgen und 10 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 15/25 Zellen, 6 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 5. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └───┐
01 ....│
02 .┌──┘
03 .│...
04 .└──┐
```

## S1-02-02-02

- Input: 5×5; A E/1, B W/3. Zeilen `[2, 5, 1, 1, 0]`, Spalten `[4, 2, 1, 1, 1]`.
- Lösung: 9 Zellen, 4 Kurven; Dichte 9/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=50, maxDeductionDepth=6.
- Einstieg: Zeile 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[24, 25, 26, 27, 28]`.
- Eine tatsächliche maximale Prämissenkette: `[11, 29, 37, 39, 47, 49]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-02-02, globale Route 6, Gruppe 1: Sichere Belegung. Lesbarer öffentlicher Einstieg: Zeile 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 13 konkrete Anschlussfolgen und 10 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 9/25 Zellen, 4 Kurven, 1 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐...
01 │└───
02 │....
03 ┘....
04 .....
```

## S1-02-02-03

- Input: 5×5; A W/2, B S/3. Zeilen `[0, 3, 3, 4, 4]`, Spalten `[4, 4, 4, 2, 0]`.
- Lösung: 14 Zellen, 11 Kurven; Dichte 14/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=62, maxDeductionDepth=8.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[16, 17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 21, 48, 51, 55, 56, 57, 59]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-02-03, globale Route 6, Gruppe 2: Dichtere Struktur. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 23 konkrete Anschlussfolgen und 2 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 14/25 Zellen, 11 Kurven, 2 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 8. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .....
01 ┌─┐..
02 ┘┌┘..
03 ┌┘┌┐.
04 └─┘│.
```

## S1-02-02-04

- Input: 5×5; A W/1, B E/4. Zeilen `[4, 3, 1, 3, 3]`, Spalten `[2, 1, 5, 4, 2]`.
- Lösung: 14 Zellen, 10 Kurven; Dichte 14/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=65, maxDeductionDepth=9.
- Einstieg: Spalte 2 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[16, 17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[18, 23, 28, 43, 52, 59, 60, 61, 63]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-02-04, globale Route 6, Gruppe 2: Dichtere Struktur. Lesbarer öffentlicher Einstieg: Spalte 2 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 32 konkrete Anschlussfolgen und 8 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 14/25 Zellen, 10 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 9. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──┐.
01 ┘.┌┘.
02 ..│..
03 ..│┌┐
04 ..└┘└
```

## S1-02-02-05

- Input: 5×5; A N/4, B W/3. Zeilen `[2, 2, 2, 4, 4]`, Spalten `[1, 2, 1, 5, 5]`.
- Lösung: 14 Zellen, 11 Kurven; Dichte 14/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=61, maxDeductionDepth=9.
- Einstieg: Spalte 3 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[20, 21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[4, 38, 46, 47, 49, 56, 57, 58, 60]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-02-05, globale Route 6, Gruppe 2: Dichtere Struktur. Lesbarer öffentlicher Einstieg: Spalte 3 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 23 konkrete Anschlussfolgen und 11 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 14/25 Zellen, 11 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 9. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...┌┘
01 ...└┐
02 ...┌┘
03 ─┐.└┐
04 .└──┘
```

## S1-02-02-06

- Input: 5×5; A E/3, B W/4. Zeilen `[0, 0, 0, 5, 5]`, Spalten `[2, 2, 2, 2, 2]`.
- Lösung: 10 Zellen, 10 Kurven; Dichte 10/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=53, maxDeductionDepth=7.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[16, 17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 39, 44, 45, 48, 49, 51]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-02-06, globale Route 6, Gruppe 3: Mehrere Achsen. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 14 konkrete Anschlussfolgen und 0 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 10/25 Zellen, 10 Kurven, 3 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .....
01 .....
02 .....
03 ┌┐┌┐┌
04 ┘└┘└┘
```

## S1-02-02-07

- Input: 5×5; A N/0, B S/1. Zeilen `[3, 2, 2, 2, 1]`, Spalten `[1, 5, 4, 0, 0]`.
- Lösung: 10 Zellen, 8 Kurven; Dichte 10/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=56, maxDeductionDepth=8.
- Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[31, 32, 33, 34]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 20, 39, 40, 50, 51, 53, 55]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-02-07, globale Route 6, Gruppe 3: Mehrere Achsen. Lesbarer öffentlicher Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 17 konkrete Anschlussfolgen und 7 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 10/25 Zellen, 8 Kurven, 2 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 8. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └─┐..
01 .┌┘..
02 .└┐..
03 .┌┘..
04 .│...
```

## S1-02-02-08

- Input: 5×5; A S/0, B N/1. Zeilen `[5, 5, 4, 2, 2]`, Spalten `[4, 5, 4, 3, 2]`.
- Lösung: 18 Zellen, 16 Kurven; Dichte 18/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=65, maxDeductionDepth=8.
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[16, 17, 18, 19]`.
- Eine tatsächliche maximale Prämissenkette: `[19, 28, 43, 48, 51, 58, 59, 60]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-02-08, globale Route 6, Gruppe 3: Mehrere Achsen. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 26 konkrete Anschlussfolgen und 7 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 18/25 Zellen, 16 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 8. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘┌─┐
01 │┌┘┌┘
02 └┘┌┘.
03 .┌┘..
04 ┌┘...
```

## S1-02-02-09

- Input: 5×5; A E/1, B W/2. Zeilen `[0, 2, 5, 3, 0]`, Spalten `[1, 1, 2, 3, 3]`.
- Lösung: 10 Zellen, 6 Kurven; Dichte 10/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=57, maxDeductionDepth=7.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[16, 17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 32, 37, 50, 51, 54, 56]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-02-09, globale Route 6, Gruppe 4: Lange Verbindung bei begrenzter Dichte. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 18 konkrete Anschlussfolgen und 5 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 10/25 Zellen, 6 Kurven, 2 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .....
01 ...┌─
02 ──┐└┐
03 ..└─┘
04 .....
```

## S1-02-02-10

- Input: 5×5; A W/0, B E/2. Zeilen `[5, 4, 2, 0, 0]`, Spalten `[1, 2, 2, 3, 3]`.
- Lösung: 11 Zellen, 8 Kurven; Dichte 11/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=58, maxDeductionDepth=8.
- Einstieg: Zeile 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[20, 21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 31, 40, 48, 52, 53, 54, 56]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-02-10, globale Route 6, Gruppe 4: Lange Verbindung bei begrenzter Dichte. Lesbarer öffentlicher Einstieg: Zeile 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 19 konkrete Anschlussfolgen und 4 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 11/25 Zellen, 8 Kurven, 2 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 8. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ─┐┌─┐
01 .└┘┌┘
02 ...└─
03 .....
04 .....
```

## S1-02-02-11

- Input: 5×5; A S/4, B N/1. Zeilen `[2, 1, 4, 4, 1]`, Spalten `[4, 3, 1, 2, 2]`.
- Lösung: 12 Zellen, 8 Kurven; Dichte 12/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=58, maxDeductionDepth=10.
- Einstieg: A an S/4 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[15]`.
- Eine tatsächliche maximale Prämissenkette: `[15, 16, 20, 24, 27, 46, 47, 48, 51, 52]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-02-11, globale Route 6, Gruppe 5: Kompakte Verdichtung. Lesbarer öffentlicher Einstieg: A an S/4 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 23 konkrete Anschlussfolgen und 12 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 12/25 Zellen, 8 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘...
01 │....
02 │┌─┐.
03 └┘.└┐
04 ....│
```

## S1-02-02-12

- Input: 5×5; A S/1, B N/3. Zeilen `[1, 2, 1, 3, 2]`, Spalten `[2, 2, 3, 2, 0]`.
- Lösung: 9 Zellen, 6 Kurven; Dichte 9/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=57, maxDeductionDepth=14.
- Einstieg: Spalte 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 18, 24, 28, 32, 35, 39, 40, 47, 49, 52, 53, 54, 56]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-02-02-12, globale Route 6, Gruppe 5: Kompakte Verdichtung. Lesbarer öffentlicher Einstieg: Spalte 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 24 konkrete Anschlussfolgen und 10 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 9/25 Zellen, 6 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 14. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...│.
01 ..┌┘.
02 ..│..
03 ┌─┘..
04 └┐...
```
