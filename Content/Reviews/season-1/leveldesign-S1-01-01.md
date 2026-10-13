# Author-Leveldesignreview: S1-01-01 (globale Route 01)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | A/B und Randzahlen | S1-01-01-01: 4, S1-01-01-02: 4 |
| 3–5 | Nullen und Ausschluss | S1-01-01-03: 5, S1-01-01-04: 5, S1-01-01-05: 5 |
| 6–8 | Volle Linien | S1-01-01-06: 6, S1-01-01-07: 6, S1-01-01-08: 6 |
| 9–10 | Verschiedene A/B-Kanten | S1-01-01-09: 6, S1-01-01-10: 6 |
| 11–12 | Einstiegsmuster kombinieren | S1-01-01-11: 7, S1-01-01-12: 9 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-01-01-01

- Input: 4×4; A E/2, B N/0. Zeilen `[4, 3, 1, 0]`, Spalten `[2, 2, 1, 3]`.
- Lösung: 8 Zellen, 5 Kurven; Dichte 8/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=33, maxDeductionDepth=4.
- Einstieg: Zeile 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[18, 19, 20, 21]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 15, 22, 31]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-01-01, globale Route 1, Gruppe 1: A/B und Randzahlen. Lesbarer öffentlicher Einstieg: Zeile 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 7 konkrete Anschlussfolgen und 4 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 8/16 Zellen, 5 Kurven, 1 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 4. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 │┌─┐
01 └┘.│
02 ...└
03 ....
```

## S1-01-01-02

- Input: 4×4; A E/2, B W/2. Zeilen `[0, 0, 4, 2]`, Spalten `[1, 1, 2, 2]`.
- Lösung: 6 Zellen, 4 Kurven; Dichte 6/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=32, maxDeductionDepth=4.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[12, 13, 14, 15]`.
- Eine tatsächliche maximale Prämissenkette: `[20, 23, 27, 28]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-01-02, globale Route 1, Gruppe 1: A/B und Randzahlen. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 6 konkrete Anschlussfolgen und 2 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 6/16 Zellen, 4 Kurven, 2 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 4. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....
01 ....
02 ──┐┌
03 ..└┘
```

## S1-01-01-03

- Input: 4×4; A W/2, B N/2. Zeilen `[3, 2, 3, 3]`, Spalten `[4, 4, 3, 0]`.
- Lösung: 11 Zellen, 9 Kurven; Dichte 11/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=44, maxDeductionDepth=5.
- Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[19, 20, 21, 22]`.
- Eine tatsächliche maximale Prämissenkette: `[5, 26, 37, 41, 42]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-01-03, globale Route 1, Gruppe 2: Nullen und Ausschluss. Lesbarer öffentlicher Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 18 konkrete Anschlussfolgen und 1 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 11/16 Zellen, 9 Kurven, 1 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 5. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┘.
01 └┐..
02 ┐└┐.
03 └─┘.
```

## S1-01-01-04

- Input: 4×4; A S/2, B E/0. Zeilen `[2, 2, 2, 1]`, Spalten `[0, 0, 4, 3]`.
- Lösung: 7 Zellen, 5 Kurven; Dichte 7/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=35, maxDeductionDepth=5.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 26, 27, 29, 33]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-01-04, globale Route 1, Gruppe 2: Nullen und Ausschluss. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 9 konkrete Anschlussfolgen und 3 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 7/16 Zellen, 5 Kurven, 2 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 5. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌─
01 ..└┐
02 ..┌┘
03 ..│.
```

## S1-01-01-05

- Input: 4×4; A S/0, B E/1. Zeilen `[0, 4, 3, 1]`, Spalten `[3, 2, 2, 1]`.
- Lösung: 8 Zellen, 5 Kurven; Dichte 8/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=38, maxDeductionDepth=5.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[12, 13, 14, 15]`.
- Eine tatsächliche maximale Prämissenkette: `[8, 19, 33, 35, 36]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-01-05, globale Route 1, Gruppe 2: Nullen und Ausschluss. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 12 konkrete Anschlussfolgen und 4 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 8/16 Zellen, 5 Kurven, 1 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 5. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....
01 ┌┐┌─
02 │└┘.
03 │...
```

## S1-01-01-06

- Input: 4×4; A N/2, B W/1. Zeilen `[2, 3, 3, 4]`, Spalten `[3, 1, 4, 4]`.
- Lösung: 12 Zellen, 9 Kurven; Dichte 12/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=35, maxDeductionDepth=6.
- Einstieg: Zeile 3 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[12, 13, 14, 15]`.
- Eine tatsächliche maximale Prämissenkette: `[13, 17, 27, 28, 31, 34]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-01-06, globale Route 1, Gruppe 3: Volle Linien. Lesbarer öffentlicher Einstieg: Zeile 3 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 11 konkrete Anschlussfolgen und 3 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 12/16 Zellen, 9 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└┐
01 ┐.┌┘
02 │.└┐
03 └──┘
```

## S1-01-01-07

- Input: 4×4; A W/2, B S/1. Zeilen `[3, 4, 2, 2]`, Spalten `[2, 3, 4, 2]`.
- Lösung: 11 Zellen, 9 Kurven; Dichte 11/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=39, maxDeductionDepth=6.
- Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[12, 13, 14, 15]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 16, 22, 24, 25, 36]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-01-07, globale Route 1, Gruppe 3: Volle Linien. Lesbarer öffentlicher Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 13 konkrete Anschlussfolgen und 5 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 11/16 Zellen, 9 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌─┐
01 ┌┘┌┘
02 ┘.│.
03 .┌┘.
```

## S1-01-01-08

- Input: 4×4; A W/3, B N/2. Zeilen `[3, 1, 1, 1]`, Spalten `[4, 1, 1, 0]`.
- Lösung: 6 Zellen, 3 Kurven; Dichte 6/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=36, maxDeductionDepth=6.
- Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[2, 18, 24, 29, 30, 34]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-01-08, globale Route 1, Gruppe 3: Volle Linien. Lesbarer öffentlicher Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 11 konkrete Anschlussfolgen und 7 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 6/16 Zellen, 3 Kurven, 1 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┘.
01 │...
02 │...
03 ┘...
```

## S1-01-01-09

- Input: 4×4; A W/0, B E/3. Zeilen `[1, 2, 1, 3]`, Spalten `[2, 3, 1, 1]`.
- Lösung: 7 Zellen, 4 Kurven; Dichte 7/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=36, maxDeductionDepth=6.
- Einstieg: A an W/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[0]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 12, 20, 21, 27, 32]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-01-09, globale Route 1, Gruppe 4: Verschiedene A/B-Kanten. Lesbarer öffentlicher Einstieg: A an W/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 12 konkrete Anschlussfolgen und 9 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 7/16 Zellen, 4 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐...
01 └┐..
02 .│..
03 .└──
```

## S1-01-01-10

- Input: 4×4; A N/1, B S/1. Zeilen `[3, 1, 1, 3]`, Spalten `[0, 2, 2, 4]`.
- Lösung: 8 Zellen, 4 Kurven; Dichte 8/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=35, maxDeductionDepth=6.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[12, 13, 14, 15]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 16, 22, 23, 24, 33]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-01-10, globale Route 1, Gruppe 4: Verschiedene A/B-Kanten. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 11 konkrete Anschlussfolgen und 4 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 8/16 Zellen, 4 Kurven, 1 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .└─┐
01 ...│
02 ...│
03 .┌─┘
```

## S1-01-01-11

- Input: 4×4; A E/3, B N/1. Zeilen `[1, 4, 4, 3]`, Spalten `[3, 4, 2, 3]`.
- Lösung: 12 Zellen, 9 Kurven; Dichte 12/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=37, maxDeductionDepth=7.
- Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[15, 16, 17, 18]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 12, 26, 28, 29, 33, 34]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-01-11, globale Route 1, Gruppe 5: Einstiegsmuster kombinieren. Lesbarer öffentlicher Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 11 konkrete Anschlussfolgen und 4 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 12/16 Zellen, 9 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .│..
01 ┌┘┌┐
02 │┌┘│
03 └┘.└
```

## S1-01-01-12

- Input: 4×4; A E/2, B S/1. Zeilen `[2, 3, 4, 1]`, Spalten `[2, 4, 3, 1]`.
- Lösung: 10 Zellen, 7 Kurven; Dichte 10/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=41, maxDeductionDepth=9.
- Einstieg: Zeile 2 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[12, 13, 14]`.
- Eine tatsächliche maximale Prämissenkette: `[9, 15, 26, 27, 34, 36, 37, 38, 40]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-01-12, globale Route 1, Gruppe 5: Einstiegsmuster kombinieren. Lesbarer öffentlicher Einstieg: Zeile 2 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 17 konkrete Anschlussfolgen und 5 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 10/16 Zellen, 7 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 9. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌┐.
01 ┌┘│.
02 └┐└─
03 .│..
```
