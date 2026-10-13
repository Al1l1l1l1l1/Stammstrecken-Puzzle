# Author-Leveldesignreview: S1-05-01 (globale Route 17)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Zugängliche Extremwerte | S1-05-01-01: 7, S1-05-01-02: 7 |
| 3–5 | Entfernte Belegungsbereiche | S1-05-01-03: 10, S1-05-01-04: 11, S1-05-01-05: 10 |
| 6–8 | Ausschluss über das Raster | S1-05-01-06: 13, S1-05-01-07: 13, S1-05-01-08: 15 |
| 9–10 | Lange Pfade und Kurven | S1-05-01-09: 12, S1-05-01-10: 14 |
| 11–12 | Faire große Abschlüsse | S1-05-01-11: 32, S1-05-01-12: 30 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-05-01-01

- Input: 8×8; A E/7, B S/0. Zeilen `[0, 0, 0, 0, 0, 0, 5, 7]`, Spalten `[2, 2, 1, 1, 2, 1, 2, 1]`.
- Lösung: 12 Zellen, 7 Kurven; Dichte 12/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=110, maxDeductionDepth=7.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 76, 91, 92, 96, 97, 107]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-01-01, globale Route 17, Gruppe 1: Zugängliche Extremwerte. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 22 konkrete Anschlussfolgen und 4 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 12/64 Zellen, 7 Kurven, 6 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ........
01 ........
02 ........
03 ........
04 ........
05 ........
06 ┌┐..┌─┐.
07 │└──┘.└─
```

## S1-05-01-02

- Input: 8×8; A W/0, B E/0. Zeilen `[6, 4, 4, 4, 4, 5, 2, 3]`, Spalten `[8, 1, 8, 1, 6, 1, 6, 1]`.
- Lösung: 32 Zellen, 8 Kurven; Dichte 32/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=139, maxDeductionDepth=7.
- Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[28, 29, 30, 31, 32, 33, 34]`.
- Eine tatsächliche maximale Prämissenkette: `[20, 58, 70, 88, 89, 111, 122]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-01-02, globale Route 17, Gruppe 1: Zugängliche Extremwerte. Lesbarer öffentlicher Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 53 konkrete Anschlussfolgen und 32 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 32/64 Zellen, 8 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.┌─┐.┌─
01 │.│.│.│.
02 │.│.│.│.
03 │.│.│.│.
04 │.│.│.│.
05 │.│.└─┘.
06 │.│.....
07 └─┘.....
```

## S1-05-01-03

- Input: 8×8; A N/0, B S/0. Zeilen `[8, 1, 7, 1, 5, 1, 6, 1]`, Spalten `[3, 5, 4, 4, 4, 5, 2, 3]`.
- Lösung: 30 Zellen, 8 Kurven; Dichte 30/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=140, maxDeductionDepth=10.
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[28, 29, 30, 31, 32, 33, 34]`.
- Eine tatsächliche maximale Prämissenkette: `[20, 35, 51, 52, 60, 89, 91, 111, 120, 130]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-01-03, globale Route 17, Gruppe 2: Entfernte Belegungsbereiche. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 55 konkrete Anschlussfolgen und 34 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 30/64 Zellen, 8 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └──────┐
01 .......│
02 .┌─────┘
03 .│......
04 .└───┐..
05 .....│..
06 ┌────┘..
07 │.......
```

## S1-05-01-04

- Input: 8×8; A E/0, B S/2. Zeilen `[1, 1, 3, 2, 2, 6, 1, 1]`, Spalten `[0, 0, 3, 1, 1, 3, 4, 5]`.
- Lösung: 17 Zellen, 9 Kurven; Dichte 17/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=131, maxDeductionDepth=11.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[42, 43, 44, 45, 46, 47]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 34, 54, 60, 77, 101, 104, 119, 121, 128, 129]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-01-04, globale Route 17, Gruppe 2: Entfernte Belegungsbereiche. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 45 konkrete Anschlussfolgen und 35 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 17/64 Zellen, 9 Kurven, 2 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .......┌
01 .......│
02 .....┌─┘
03 .....└┐.
04 ......└┐
05 ..┌────┘
06 ..│.....
07 ..│.....
```

## S1-05-01-05

- Input: 8×8; A N/6, B W/5. Zeilen `[6, 8, 1, 1, 1, 1, 0, 0]`, Spalten `[6, 2, 1, 1, 2, 2, 2, 2]`.
- Lösung: 18 Zellen, 11 Kurven; Dichte 18/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=125, maxDeductionDepth=10.
- Einstieg: Zeile 6 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[43, 44, 45, 46, 47, 48, 49, 50]`.
- Eine tatsächliche maximale Prämissenkette: `[16, 42, 93, 97, 101, 102, 103, 105, 106, 108]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-01-05, globale Route 17, Gruppe 2: Entfernte Belegungsbereiche. Lesbarer öffentlicher Einstieg: Zeile 6 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 40 konkrete Anschlussfolgen und 27 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 18/64 Zellen, 11 Kurven, 2 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐..┌┐└┐
01 │└──┘└─┘
02 │.......
03 │.......
04 │.......
05 ┘.......
06 ........
07 ........
```

## S1-05-01-06

- Input: 8×8; A S/4, B N/1. Zeilen `[1, 5, 5, 1, 1, 1, 4, 5]`, Spalten `[0, 2, 1, 4, 3, 3, 3, 7]`.
- Lösung: 23 Zellen, 12 Kurven; Dichte 23/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=138, maxDeductionDepth=13.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[35, 36, 37, 38, 39, 40, 41]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 28, 49, 50, 85, 92, 93, 116, 131, 132, 133, 134, 137]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-01-06, globale Route 17, Gruppe 3: Ausschluss über das Raster. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 54 konkrete Anschlussfolgen und 33 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 23/64 Zellen, 12 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 13. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .│......
01 .└─┐..┌┐
02 ...└──┘│
03 .......│
04 .......│
05 .......│
06 ...┌─┐.│
07 ...└┐└─┘
```

## S1-05-01-07

- Input: 8×8; A E/3, B S/1. Zeilen `[0, 0, 2, 8, 3, 2, 1, 1]`, Spalten `[4, 5, 1, 1, 1, 2, 2, 1]`.
- Lösung: 17 Zellen, 9 Kurven; Dichte 17/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=128, maxDeductionDepth=13.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 51, 93, 96, 97, 100, 103, 105, 112, 113, 114, 118, 126]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-01-07, globale Route 17, Gruppe 3: Ausschluss über das Raster. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 42 konkrete Anschlussfolgen und 30 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 17/64 Zellen, 9 Kurven, 2 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 13. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ........
01 ........
02 ┌┐......
03 │└───┐┌─
04 │....└┘.
05 └┐......
06 .│......
07 .│......
```

## S1-05-01-08

- Input: 8×8; A N/2, B S/0. Zeilen `[3, 2, 2, 2, 2, 5, 2, 6]`, Spalten `[7, 7, 3, 2, 3, 2, 0, 0]`.
- Lösung: 24 Zellen, 16 Kurven; Dichte 24/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=137, maxDeductionDepth=15.
- Einstieg: Spalte 6 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[26, 51, 57, 64, 71, 72, 78, 82, 106, 125, 129, 130, 131, 133, 135]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-01-08, globale Route 17, Gruppe 3: Ausschluss über das Raster. Lesbarer öffentlicher Einstieg: Spalte 6 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 49 konkrete Anschlussfolgen und 24 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 24/64 Zellen, 16 Kurven, 2 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 15. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┘.....
01 └┐......
02 ┌┘......
03 └┐......
04 ┌┘......
05 └───┐...
06 ....└┐..
07 ┌────┘..
```

## S1-05-01-09

- Input: 8×8; A W/0, B S/1. Zeilen `[5, 4, 1, 3, 3, 1, 1, 1]`, Spalten `[1, 7, 4, 5, 2, 0, 0, 0]`.
- Lösung: 19 Zellen, 11 Kurven; Dichte 19/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=135, maxDeductionDepth=12.
- Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[41, 42, 43, 44, 45, 46, 47]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 28, 79, 80, 88, 102, 104, 113, 116, 126, 127, 128]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-01-09, globale Route 17, Gruppe 4: Lange Pfade und Kurven. Lesbarer öffentlicher Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 48 konkrete Anschlussfolgen und 24 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/64 Zellen, 11 Kurven, 3 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ─┐┌─┐...
01 .└┘┌┘...
02 ...│....
03 .┌┐│....
04 .│└┘....
05 .│......
06 .│......
07 .│......
```

## S1-05-01-10

- Input: 8×8; A N/7, B E/5. Zeilen `[5, 3, 1, 4, 4, 3, 0, 0]`, Spalten `[0, 0, 0, 5, 4, 5, 4, 2]`.
- Lösung: 20 Zellen, 13 Kurven; Dichte 20/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=134, maxDeductionDepth=14.
- Einstieg: Zeile 6 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 62, 67, 81, 94, 105, 108, 114, 116, 118, 126, 127, 129, 130]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-01-10, globale Route 17, Gruppe 4: Lange Pfade und Kurven. Lesbarer öffentlicher Einstieg: Zeile 6 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 48 konkrete Anschlussfolgen und 9 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 20/64 Zellen, 13 Kurven, 5 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 14. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...┌┐┌─┘
01 ...│└┘..
02 ...│....
03 ...│┌─┐.
04 ...└┘┌┘.
05 .....└──
06 ........
07 ........
```

## S1-05-01-11

- Input: 8×8; A S/7, B W/0. Zeilen `[4, 4, 3, 2, 1, 1, 1, 1]`, Spalten `[2, 2, 1, 2, 2, 1, 2, 5]`.
- Lösung: 17 Zellen, 11 Kurven; Dichte 17/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=152, maxDeductionDepth=32.
- Einstieg: A an S/7 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[27]`.
- Eine tatsächliche maximale Prämissenkette: `[27, 34, 42, 43, 50, 58, 59, 66, 74, 75, 82, 90, 91, 94, 98, 99, 105, 112, 113, 115, 118, 119, 121, 124, 125, 126, 130, 140, 141, 143, 144, 148]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-01-11, globale Route 17, Gruppe 5: Faire große Abschlüsse. Lesbarer öffentlicher Einstieg: A an S/7 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 72 konkrete Anschlussfolgen und 47 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 17/64 Zellen, 11 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 32. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐┌─┐....
01 └┘.└┐...
02 ....└─┐.
03 ......└┐
04 .......│
05 .......│
06 .......│
07 .......│
```

## S1-05-01-12

- Input: 8×8; A E/0, B W/5. Zeilen `[1, 3, 1, 1, 1, 2, 5, 5]`, Spalten `[3, 1, 2, 2, 2, 6, 1, 2]`.
- Lösung: 19 Zellen, 12 Kurven; Dichte 19/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=151, maxDeductionDepth=30.
- Einstieg: A an E/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[7]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 34, 35, 43, 44, 50, 57, 58, 64, 71, 76, 77, 83, 88, 89, 95, 100, 101, 107, 112, 115, 121, 125, 129, 143, 144, 146, 147, 148, 150]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-01-12, globale Route 17, Gruppe 5: Faire große Abschlüsse. Lesbarer öffentlicher Einstieg: A an E/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 71 konkrete Anschlussfolgen und 44 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/64 Zellen, 12 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 30. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .......┌
01 .....┌─┘
02 .....│..
03 .....│..
04 .....│..
05 ┐....│..
06 │.┌┐┌┘..
07 └─┘└┘...
```
