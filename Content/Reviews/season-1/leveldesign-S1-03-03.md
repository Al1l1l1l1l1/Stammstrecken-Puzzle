# Author-Leveldesignreview: S1-03-03 (globale Route 11)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Vertraute Linien | S1-03-03-01: 6, S1-03-03-02: 6 |
| 3–5 | Teilpfade durch Counts trennen | S1-03-03-03: 9, S1-03-03-04: 10, S1-03-03-05: 10 |
| 6–8 | Belegung, Ausschluss und Geometrie | S1-03-03-06: 12, S1-03-03-07: 12, S1-03-03-08: 13 |
| 9–10 | Entfernte A/B und Kurven | S1-03-03-09: 11, S1-03-03-10: 12 |
| 11–12 | Große 7×6-Abschlüsse | S1-03-03-11: 25, S1-03-03-12: 23 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-03-03-01

- Input: 6×6; A N/0, B S/5. Zeilen `[6, 1, 4, 1, 4, 1]`, Spalten `[1, 1, 4, 3, 3, 5]`.
- Lösung: 17 Zellen, 6 Kurven; Dichte 17/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=77, maxDeductionDepth=6.
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[20, 21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[19, 29, 49, 50, 64, 72]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-03-01, globale Route 11, Gruppe 1: Vertraute Linien. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 25 konkrete Anschlussfolgen und 19 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 17/36 Zellen, 6 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └────┐
01 .....│
02 ..┌──┘
03 ..│...
04 ..└──┐
05 .....│
```

## S1-03-03-02

- Input: 6×6; A N/1, B S/2. Zeilen `[1, 2, 1, 1, 1, 3]`, Spalten `[5, 3, 1, 0, 0, 0]`.
- Lösung: 9 Zellen, 4 Kurven; Dichte 9/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=67, maxDeductionDepth=6.
- Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[34, 35, 36, 37, 38]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 20, 25, 51, 53, 64]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-03-02, globale Route 11, Gruppe 1: Vertraute Linien. Lesbarer öffentlicher Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 15 konkrete Anschlussfolgen und 12 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 9/36 Zellen, 4 Kurven, 3 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .│....
01 ┌┘....
02 │.....
03 │.....
04 │.....
05 └─┐...
```

## S1-03-03-03

- Input: 6×6; A N/3, B S/4. Zeilen `[1, 2, 2, 4, 5, 5]`, Spalten `[0, 2, 3, 6, 5, 3]`.
- Lösung: 19 Zellen, 16 Kurven; Dichte 19/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=86, maxDeductionDepth=9.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[25, 26, 27, 28, 29]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 23, 35, 51, 65, 78, 79, 80, 82]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-03-03, globale Route 11, Gruppe 2: Teilpfade durch Counts trennen. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 32 konkrete Anschlussfolgen und 12 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/36 Zellen, 16 Kurven, 1 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 9. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...│..
01 ...└┐.
02 ...┌┘.
03 ..┌┘┌┐
04 .┌┘┌┘│
05 .└─┘┌┘
```

## S1-03-03-04

- Input: 6×6; A N/2, B E/1. Zeilen `[3, 5, 2, 3, 5, 3]`, Spalten `[6, 5, 4, 1, 4, 1]`.
- Lösung: 21 Zellen, 13 Kurven; Dichte 21/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=86, maxDeductionDepth=10.
- Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 29, 46, 48, 54, 56, 66, 73, 80, 81]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-03-04, globale Route 11, Gruppe 2: Teilpfade durch Counts trennen. Lesbarer öffentlicher Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 38 konkrete Anschlussfolgen und 14 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 21/36 Zellen, 13 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐│...
01 │└┘.┌─
02 │...│.
03 └┐..│.
04 ┌┘┌─┘.
05 └─┘...
```

## S1-03-03-05

- Input: 6×6; A W/0, B S/4. Zeilen `[1, 3, 4, 2, 2, 2]`, Spalten `[3, 1, 4, 5, 1, 0]`.
- Lösung: 14 Zellen, 11 Kurven; Dichte 14/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=74, maxDeductionDepth=10.
- Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[34, 35, 36, 37, 38]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 20, 39, 40, 50, 64, 67, 68, 70, 72]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-03-05, globale Route 11, Gruppe 2: Teilpfade durch Counts trennen. Lesbarer öffentlicher Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 22 konkrete Anschlussfolgen und 17 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 14/36 Zellen, 11 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.....
01 │.┌┐..
02 └─┘│..
03 ..┌┘..
04 ..└┐..
05 ...└┐.
```

## S1-03-03-06

- Input: 7×6; A E/0, B W/4. Zeilen `[7, 6, 2, 2, 3, 3]`, Spalten `[6, 6, 4, 2, 2, 2, 1]`.
- Lösung: 23 Zellen, 20 Kurven; Dichte 23/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=105, maxDeductionDepth=12.
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[22, 23, 24, 25, 26, 27]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 38, 46, 80, 85, 89, 90, 93, 95, 98, 100, 104]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-03-06, globale Route 11, Gruppe 3: Belegung, Ausschluss und Geometrie. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 43 konkrete Anschlussfolgen und 19 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 23/42 Zellen, 20 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐┌┐┌─
01 └┐└┘└┘.
02 ┌┘.....
03 └┐.....
04 ┐└┐....
05 └─┘....
```

## S1-03-03-07

- Input: 7×6; A E/2, B S/1. Zeilen `[3, 3, 4, 2, 1, 6]`, Spalten `[0, 1, 1, 4, 3, 5, 5]`.
- Lösung: 19 Zellen, 11 Kurven; Dichte 19/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=100, maxDeductionDepth=12.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[22, 23, 24, 25, 26, 27]`.
- Eine tatsächliche maximale Prämissenkette: `[16, 32, 38, 39, 48, 53, 67, 77, 79, 89, 93, 94]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-03-07, globale Route 11, Gruppe 3: Belegung, Ausschluss und Geometrie. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 42 konkrete Anschlussfolgen und 17 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/42 Zellen, 11 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...┌─┐.
01 ...│.└┐
02 ...└─┐└
03 .....└┐
04 ......│
05 .┌────┘
```

## S1-03-03-08

- Input: 7×6; A W/0, B E/2. Zeilen `[1, 4, 4, 4, 6, 0]`, Spalten `[5, 1, 4, 1, 4, 1, 3]`.
- Lösung: 19 Zellen, 8 Kurven; Dichte 19/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=100, maxDeductionDepth=13.
- Einstieg: Zeile 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[28, 29, 30, 31, 32, 33, 34]`.
- Eine tatsächliche maximale Prämissenkette: `[15, 38, 56, 66, 69, 70, 71, 85, 86, 87, 88, 90, 92]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-03-08, globale Route 11, Gruppe 3: Belegung, Ausschluss und Geometrie. Lesbarer öffentlicher Einstieg: Zeile 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 43 konkrete Anschlussfolgen und 14 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/42 Zellen, 8 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 13. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐......
01 │.┌─┐..
02 │.│.│.┌
03 │.│.│.│
04 └─┘.└─┘
05 .......
```

## S1-03-03-09

- Input: 7×6; A W/3, B S/5. Zeilen `[0, 0, 0, 2, 6, 6]`, Spalten `[3, 3, 2, 2, 2, 2, 0]`.
- Lösung: 14 Zellen, 11 Kurven; Dichte 14/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=91, maxDeductionDepth=11.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[22, 23, 24, 25, 26, 27, 28]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 45, 61, 72, 79, 80, 81, 82, 86, 87, 89]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-03-09, globale Route 11, Gruppe 4: Entfernte A/B und Kurven. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 29 konkrete Anschlussfolgen und 4 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 14/42 Zellen, 11 Kurven, 4 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .......
01 .......
02 .......
03 ─┐.....
04 ┌┘┌┐┌┐.
05 └─┘└┘│.
```

## S1-03-03-10

- Input: 7×6; A S/3, B N/0. Zeilen `[5, 1, 1, 3, 3, 2]`, Spalten `[1, 1, 4, 4, 5, 0, 0]`.
- Lösung: 15 Zellen, 8 Kurven; Dichte 15/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=96, maxDeductionDepth=12.
- Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[27, 28, 29, 30, 31, 32]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 22, 39, 40, 57, 72, 77, 82, 88, 91, 93, 95]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-03-10, globale Route 11, Gruppe 4: Entfernte A/B und Kurven. Lesbarer öffentlicher Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 38 konkrete Anschlussfolgen und 14 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 15/42 Zellen, 8 Kurven, 2 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └───┐..
01 ....│..
02 ....│..
03 ..┌┐│..
04 ..│└┘..
05 ..└┐...
```

## S1-03-03-11

- Input: 7×6; A S/0, B E/1. Zeilen `[2, 3, 1, 1, 5, 1]`, Spalten `[2, 1, 1, 1, 4, 2, 2]`.
- Lösung: 13 Zellen, 7 Kurven; Dichte 13/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=100, maxDeductionDepth=25.
- Einstieg: A an S/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[15]`.
- Eine tatsächliche maximale Prämissenkette: `[15, 22, 34, 35, 39, 44, 45, 49, 54, 55, 59, 64, 65, 69, 74, 75, 78, 82, 83, 84, 88, 89, 90, 93, 97]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-03-11, globale Route 11, Gruppe 5: Große 7×6-Abschlüsse. Lesbarer öffentlicher Einstieg: A an S/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 45 konkrete Anschlussfolgen und 29 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 13/42 Zellen, 7 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 25. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .....┌┐
01 ....┌┘└
02 ....│..
03 ....│..
04 ┌───┘..
05 │......
```

## S1-03-03-12

- Input: 7×6; A S/2, B W/4. Zeilen `[7, 6, 3, 2, 6, 4]`, Spalten `[3, 5, 2, 4, 3, 5, 6]`.
- Lösung: 28 Zellen, 19 Kurven; Dichte 28/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=107, maxDeductionDepth=23.
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[22, 23, 24, 25, 26, 27, 28]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 29, 39, 55, 56, 73, 76, 80, 83, 84, 85, 87, 90, 93, 94, 96, 97, 98, 100, 101, 103, 104, 106]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-03-03-12, globale Route 11, Gruppe 5: Große 7×6-Abschlüsse. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 55 konkrete Anschlussfolgen und 10 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 28/42 Zellen, 19 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 23. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──┐┌─┐
01 └┐.└┘┌┘
02 .│...└┐
03 .│....│
04 ─┘.┌─┐│
05 ..┌┘.└┘
```
