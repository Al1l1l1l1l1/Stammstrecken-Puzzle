# Author-Leveldesignreview: S1-01-02 (globale Route 02)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Hohe Randzahlen | S1-01-02-01: 4, S1-01-02-02: 5 |
| 3–5 | Belegung als optionale Notiz | S1-01-02-03: 6, S1-01-02-04: 6, S1-01-02-05: 6 |
| 6–8 | Gekreuzte Belegungs-/Ausschlusslogik | S1-01-02-06: 7, S1-01-02-07: 7, S1-01-02-08: 7 |
| 9–10 | Nahe Endpunkte | S1-01-02-09: 6, S1-01-02-10: 6 |
| 11–12 | Längere eindeutige Kette | S1-01-02-11: 9, S1-01-02-12: 11 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-01-02-01

- Input: 4×5; A E/4, B W/1. Zeilen `[0, 1, 4, 3, 1]`, Spalten `[3, 2, 1, 3]`.
- Lösung: 9 Zellen, 6 Kurven; Dichte 9/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=43, maxDeductionDepth=4.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[4, 18, 32, 33]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-02-01, globale Route 2, Gruppe 1: Hohe Randzahlen. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 11 konkrete Anschlussfolgen und 7 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 9/20 Zellen, 6 Kurven, 1 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 4. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....
01 ┐...
02 │┌─┐
03 └┘.│
04 ...└
```

## S1-01-02-02

- Input: 4×5; A S/1, B W/0. Zeilen `[3, 2, 1, 2, 2]`, Spalten `[1, 5, 4, 0]`.
- Lösung: 10 Zellen, 7 Kurven; Dichte 10/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=51, maxDeductionDepth=5.
- Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[22, 23, 24, 25, 26]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 14, 27, 28, 44]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-02-02, globale Route 2, Gruppe 1: Hohe Randzahlen. Lesbarer öffentlicher Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 19 konkrete Anschlussfolgen und 5 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 10/20 Zellen, 7 Kurven, 1 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 5. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ──┐.
01 .┌┘.
02 .│..
03 .└┐.
04 .┌┘.
```

## S1-01-02-03

- Input: 4×5; A N/2, B W/4. Zeilen `[2, 2, 2, 4, 3]`, Spalten `[2, 1, 5, 5]`.
- Lösung: 13 Zellen, 11 Kurven; Dichte 13/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=45, maxDeductionDepth=6.
- Einstieg: Zeile 3 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[15, 22, 33, 34, 36, 40]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-02-03, globale Route 2, Gruppe 2: Belegung als optionale Notiz. Lesbarer öffentlicher Einstieg: Zeile 3 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 13 konkrete Anschlussfolgen und 7 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 13/20 Zellen, 11 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└┐
01 ..┌┘
02 ..└┐
03 ┌─┐│
04 ┘.└┘
```

## S1-01-02-04

- Input: 4×5; A E/3, B W/1. Zeilen `[4, 4, 1, 1, 0]`, Spalten `[2, 2, 2, 4]`.
- Lösung: 10 Zellen, 8 Kurven; Dichte 10/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=44, maxDeductionDepth=6.
- Einstieg: Zeile 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[24, 25, 26, 27]`.
- Eine tatsächliche maximale Prämissenkette: `[15, 29, 35, 40, 41, 42]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-02-04, globale Route 2, Gruppe 2: Belegung als optionale Notiz. Lesbarer öffentlicher Einstieg: Zeile 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 12 konkrete Anschlussfolgen und 6 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 10/20 Zellen, 8 Kurven, 1 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐┌┐
01 ┘└┘│
02 ...│
03 ...└
04 ....
```

## S1-01-02-05

- Input: 4×5; A E/4, B N/1. Zeilen `[2, 1, 2, 2, 4]`, Spalten `[5, 4, 1, 1]`.
- Lösung: 11 Zellen, 7 Kurven; Dichte 11/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=48, maxDeductionDepth=6.
- Einstieg: Zeile 4 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[14, 15, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[16, 24, 36, 38, 39, 46]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-02-05, globale Route 2, Gruppe 2: Belegung als optionale Notiz. Lesbarer öffentlicher Einstieg: Zeile 4 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 16 konkrete Anschlussfolgen und 9 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 11/20 Zellen, 7 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘..
01 │...
02 └┐..
03 ┌┘..
04 └───
```

## S1-01-02-06

- Input: 4×5; A E/2, B S/0. Zeilen `[3, 3, 3, 1, 2]`, Spalten `[3, 5, 1, 3]`.
- Lösung: 12 Zellen, 9 Kurven; Dichte 12/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=47, maxDeductionDepth=7.
- Einstieg: Spalte 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[14, 15, 16, 17, 18]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 21, 24, 26, 35, 36, 43]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-02-06, globale Route 2, Gruppe 3: Gekreuzte Belegungs-/Ausschlusslogik. Lesbarer öffentlicher Einstieg: Spalte 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 16 konkrete Anschlussfolgen und 8 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 12/20 Zellen, 9 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌─┐
01 ┌┘.│
02 └┐.└
03 .│..
04 ┌┘..
```

## S1-01-02-07

- Input: 4×5; A N/2, B E/2. Zeilen `[4, 4, 2, 3, 3]`, Spalten `[5, 3, 4, 4]`.
- Lösung: 16 Zellen, 11 Kurven; Dichte 16/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=51, maxDeductionDepth=7.
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[14, 15, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 25, 27, 40, 42, 45, 48]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-02-07, globale Route 2, Gruppe 3: Gekreuzte Belegungs-/Ausschlusslogik. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 22 konkrete Anschlussfolgen und 3 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 16/20 Zellen, 11 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐└┐
01 │└─┘
02 │..┌
03 │.┌┘
04 └─┘.
```

## S1-01-02-08

- Input: 4×5; A S/0, B W/1. Zeilen `[0, 4, 3, 1, 4]`, Spalten `[2, 3, 3, 4]`.
- Lösung: 12 Zellen, 7 Kurven; Dichte 12/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=53, maxDeductionDepth=7.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[4, 25, 34, 44, 49, 50, 51]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-02-08, globale Route 2, Gruppe 3: Gekreuzte Belegungs-/Ausschlusslogik. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 21 konkrete Anschlussfolgen und 4 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 12/20 Zellen, 7 Kurven, 1 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....
01 ─┐┌┐
02 .└┘│
03 ...│
04 ┌──┘
```

## S1-01-02-09

- Input: 4×5; A E/4, B W/4. Zeilen `[0, 0, 0, 4, 4]`, Spalten `[2, 2, 2, 2]`.
- Lösung: 8 Zellen, 8 Kurven; Dichte 8/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=43, maxDeductionDepth=6.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 32, 36, 37, 40, 41]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-02-09, globale Route 2, Gruppe 4: Nahe Endpunkte. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 11 konkrete Anschlussfolgen und 0 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 8/20 Zellen, 8 Kurven, 3 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....
01 ....
02 ....
03 ┌┐┌┐
04 ┘└┘└
```

## S1-01-02-10

- Input: 4×5; A S/3, B E/1. Zeilen `[3, 4, 1, 2, 4]`, Spalten `[4, 5, 3, 2]`.
- Lösung: 14 Zellen, 9 Kurven; Dichte 14/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=50, maxDeductionDepth=6.
- Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[14, 15, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[5, 24, 28, 44, 46, 47]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-02-10, globale Route 2, Gruppe 4: Nahe Endpunkte. Lesbarer öffentlicher Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 19 konkrete Anschlussfolgen und 6 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 14/20 Zellen, 9 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐.
01 └┐└─
02 .│..
03 ┌┘..
04 └──┐
```

## S1-01-02-11

- Input: 4×5; A W/1, B S/1. Zeilen `[2, 3, 3, 3, 2]`, Spalten `[5, 5, 3, 0]`.
- Lösung: 13 Zellen, 11 Kurven; Dichte 13/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=48, maxDeductionDepth=9.
- Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[22, 23, 24, 25, 26]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 29, 32, 35, 36, 41, 43, 45, 47]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-02-11, globale Route 2, Gruppe 5: Längere eindeutige Kette. Lesbarer öffentlicher Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 21 konkrete Anschlussfolgen und 0 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 13/20 Zellen, 11 Kurven, 1 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 9. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐..
01 ┘└┐.
02 ┌┐│.
03 │└┘.
04 └┐..
```

## S1-01-02-12

- Input: 4×5; A S/2, B E/3. Zeilen `[3, 4, 4, 4, 2]`, Spalten `[4, 5, 5, 3]`.
- Lösung: 17 Zellen, 15 Kurven; Dichte 17/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=58, maxDeductionDepth=11.
- Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[9, 28, 30, 31, 39, 50, 51, 52, 53, 54, 57]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-02-12, globale Route 2, Gruppe 5: Längere eindeutige Kette. Lesbarer öffentlicher Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 26 konkrete Anschlussfolgen und 3 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 17/20 Zellen, 15 Kurven, 0 Null- und 5 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐.
01 └┐└┐
02 ┌┘┌┘
03 └┐└─
04 .└┐.
```
