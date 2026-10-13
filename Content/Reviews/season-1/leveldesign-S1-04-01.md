# Author-Leveldesignreview: S1-04-01 (globale Route 13)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Kontrollierter 7×7-Übergang | S1-04-01-01: 6, S1-04-01-02: 7 |
| 3–5 | Verstreute Belegungsbereiche | S1-04-01-03: 10, S1-04-01-04: 11, S1-04-01-05: 11 |
| 6–8 | Entfernte Bereiche verbinden | S1-04-01-06: 12, S1-04-01-07: 13, S1-04-01-08: 14 |
| 9–10 | Hohe Dichte | S1-04-01-09: 10, S1-04-01-10: 13 |
| 11–12 | Lange deduktive Strecke | S1-04-01-11: 29, S1-04-01-12: 26 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-04-01-01

- Input: 7×7; A W/1, B E/0. Zeilen `[1, 5, 4, 4, 6, 0, 0]`, Spalten `[4, 1, 4, 1, 4, 1, 5]`.
- Lösung: 20 Zellen, 8 Kurven; Dichte 20/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=104, maxDeductionDepth=6.
- Einstieg: Zeile 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[30, 31, 32, 33, 34, 35, 36]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 24, 46, 68, 83, 85]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-01-01, globale Route 13, Gruppe 1: Kontrollierter 7×7-Übergang. Lesbarer öffentlicher Einstieg: Zeile 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 35 konkrete Anschlussfolgen und 15 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 20/49 Zellen, 8 Kurven, 2 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ......┌
01 ┐.┌─┐.│
02 │.│.│.│
03 │.│.│.│
04 └─┘.└─┘
05 .......
06 .......
```

## S1-04-01-02

- Input: 7×7; A W/6, B S/6. Zeilen `[0, 0, 0, 0, 0, 5, 6]`, Spalten `[1, 2, 1, 2, 1, 2, 2]`.
- Lösung: 11 Zellen, 7 Kurven; Dichte 11/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=88, maxDeductionDepth=7.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[24, 25, 26, 27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 66, 76, 78, 80, 83, 84]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-01-02, globale Route 13, Gruppe 1: Kontrollierter 7×7-Übergang. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 19 konkrete Anschlussfolgen und 3 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 11/49 Zellen, 7 Kurven, 5 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .......
01 .......
02 .......
03 .......
04 .......
05 .┌─┐.┌┐
06 ─┘.└─┘│
```

## S1-04-01-03

- Input: 7×7; A N/1, B S/1. Zeilen `[5, 1, 5, 1, 5, 1, 5]`, Spalten `[0, 5, 4, 4, 4, 6, 0]`.
- Lösung: 23 Zellen, 8 Kurven; Dichte 23/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=106, maxDeductionDepth=10.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[24, 25, 26, 27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 52, 70, 79, 99, 100, 101, 102, 103, 104]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-01-03, globale Route 13, Gruppe 2: Verstreute Belegungsbereiche. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 41 konkrete Anschlussfolgen und 9 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 23/49 Zellen, 8 Kurven, 2 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .└───┐.
01 .....│.
02 .┌───┘.
03 .│.....
04 .└───┐.
05 .....│.
06 .┌───┘.
```

## S1-04-01-04

- Input: 7×7; A W/6, B N/4. Zeilen `[5, 2, 2, 2, 2, 2, 2]`, Spalten `[7, 7, 1, 1, 1, 0, 0]`.
- Lösung: 17 Zellen, 13 Kurven; Dichte 17/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=104, maxDeductionDepth=11.
- Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[43, 44, 45, 46, 47, 48, 49]`.
- Eine tatsächliche maximale Prämissenkette: `[25, 68, 80, 88, 89, 93, 94, 95, 97, 101, 102]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-01-04, globale Route 13, Gruppe 2: Verstreute Belegungsbereiche. Lesbarer öffentlicher Einstieg: Spalte 5 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 34 konkrete Anschlussfolgen und 18 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 17/49 Zellen, 13 Kurven, 2 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌───┘..
01 └┐.....
02 ┌┘.....
03 └┐.....
04 ┌┘.....
05 └┐.....
06 ─┘.....
```

## S1-04-01-05

- Input: 7×7; A N/0, B E/6. Zeilen `[6, 6, 2, 2, 1, 2, 2]`, Spalten `[1, 2, 1, 2, 2, 6, 7]`.
- Lösung: 21 Zellen, 17 Kurven; Dichte 21/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=112, maxDeductionDepth=11.
- Einstieg: Spalte 6 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 24, 47, 60, 72, 73, 97, 106, 107, 108, 109]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-01-05, globale Route 13, Gruppe 2: Verstreute Belegungsbereiche. Lesbarer öffentlicher Einstieg: Spalte 6 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 43 konkrete Anschlussfolgen und 28 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 21/49 Zellen, 17 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └┐.┌┐┌┐
01 .└─┘└┘│
02 .....┌┘
03 .....└┐
04 ......│
05 .....┌┘
06 .....└─
```

## S1-04-01-06

- Input: 7×7; A S/3, B W/3. Zeilen `[2, 5, 4, 3, 2, 1, 2]`, Spalten `[1, 3, 1, 6, 6, 2, 0]`.
- Lösung: 19 Zellen, 13 Kurven; Dichte 19/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=115, maxDeductionDepth=12.
- Einstieg: Spalte 6 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[30, 31, 32, 33, 34, 35, 36]`.
- Eine tatsächliche maximale Prämissenkette: `[11, 25, 53, 58, 80, 81, 95, 100, 102, 103, 105, 111]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-01-06, globale Route 13, Gruppe 3: Entfernte Bereiche verbinden. Lesbarer öffentlicher Einstieg: Spalte 6 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 52 konkrete Anschlussfolgen und 19 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/49 Zellen, 13 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...┌┐..
01 .┌─┘└┐.
02 .│.┌─┘.
03 ─┘.│...
04 ...└┐..
05 ....│..
06 ...┌┘..
```

## S1-04-01-07

- Input: 7×7; A S/6, B N/2. Zeilen `[1, 2, 2, 2, 4, 4, 4]`, Spalten `[0, 0, 7, 6, 2, 3, 1]`.
- Lösung: 19 Zellen, 16 Kurven; Dichte 19/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=113, maxDeductionDepth=13.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[2, 26, 48, 77, 88, 97, 98, 99, 101, 106, 107, 108, 109]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-01-07, globale Route 13, Gruppe 3: Entfernte Bereiche verbinden. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 43 konkrete Anschlussfolgen und 18 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/49 Zellen, 16 Kurven, 2 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 13. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..│....
01 ..└┐...
02 ..┌┘...
03 ..└┐...
04 ..┌┘┌┐.
05 ..│┌┘│.
06 ..└┘.└┐
```

## S1-04-01-08

- Input: 7×7; A N/1, B E/3. Zeilen `[2, 1, 3, 3, 4, 5, 7]`, Spalten `[7, 4, 2, 2, 1, 4, 5]`.
- Lösung: 25 Zellen, 17 Kurven; Dichte 25/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=117, maxDeductionDepth=14.
- Einstieg: Zeile 6 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[24, 25, 26, 27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 69, 75, 84, 85, 86, 90, 98, 101, 106, 111, 112, 113, 114]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-01-08, globale Route 13, Gruppe 3: Entfernte Bereiche verbinden. Lesbarer öffentlicher Einstieg: Zeile 6 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 51 konkrete Anschlussfolgen und 23 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 25/49 Zellen, 17 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 14. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘.....
01 │......
02 │....┌┐
03 │....│└
04 └┐...└┐
05 ┌┘┌┐..│
06 └─┘└──┘
```

## S1-04-01-09

- Input: 7×7; A W/0, B E/2. Zeilen `[4, 3, 4, 4, 4, 4, 6]`, Spalten `[7, 1, 7, 1, 7, 1, 5]`.
- Lösung: 29 Zellen, 8 Kurven; Dichte 29/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=113, maxDeductionDepth=10.
- Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[24, 25, 26, 27, 28, 29]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 45, 53, 58, 59, 88, 103, 104, 106, 111]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-01-09, globale Route 13, Gruppe 4: Hohe Dichte. Lesbarer öffentlicher Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 48 konkrete Anschlussfolgen und 18 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 29/49 Zellen, 8 Kurven, 0 Null- und 3 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.┌─┐..
01 │.│.│..
02 │.│.│.┌
03 │.│.│.│
04 │.│.│.│
05 │.│.│.│
06 └─┘.└─┘
```

## S1-04-01-10

- Input: 7×7; A S/3, B E/3. Zeilen `[0, 5, 6, 3, 4, 4, 3]`, Spalten `[5, 6, 4, 5, 2, 2, 1]`.
- Lösung: 25 Zellen, 19 Kurven; Dichte 25/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=115, maxDeductionDepth=13.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[24, 25, 26, 27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 38, 49, 53, 59, 76, 77, 97, 101, 102, 103, 110, 111]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-01-10, globale Route 13, Gruppe 4: Hohe Dichte. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 48 konkrete Anschlussfolgen und 17 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 25/49 Zellen, 19 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 13. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .......
01 ┌─┐┌┐..
02 └┐└┘└┐.
03 .│...└─
04 ┌┘┌┐...
05 │┌┘│...
06 └┘.│...
```

## S1-04-01-11

- Input: 7×7; A E/6, B W/1. Zeilen `[2, 3, 2, 2, 2, 1, 2]`, Spalten `[2, 2, 2, 2, 2, 3, 1]`.
- Lösung: 14 Zellen, 12 Kurven; Dichte 14/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=116, maxDeductionDepth=29.
- Einstieg: A an E/6 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[23]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 29, 36, 37, 42, 48, 49, 54, 60, 61, 65, 70, 71, 75, 80, 81, 84, 88, 89, 92, 96, 97, 99, 102, 103, 106, 108, 112, 113]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-01-11, globale Route 13, Gruppe 5: Lange deduktive Strecke. Lesbarer öffentlicher Einstieg: A an E/6 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 53 konkrete Anschlussfolgen und 35 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 14/49 Zellen, 12 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 29. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐.....
01 ┘└┐....
02 ..└┐...
03 ...└┐..
04 ....└┐.
05 .....│.
06 .....└─
```

## S1-04-01-12

- Input: 7×7; A E/0, B S/3. Zeilen `[3, 2, 1, 3, 4, 3, 4]`, Spalten `[3, 4, 4, 5, 2, 1, 1]`.
- Lösung: 20 Zellen, 15 Kurven; Dichte 20/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=118, maxDeductionDepth=26.
- Einstieg: A an E/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[6]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 24, 30, 37, 38, 44, 51, 55, 56, 61, 62, 67, 73, 76, 84, 88, 92, 95, 104, 107, 109, 110, 111, 113, 115, 117]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-01-12, globale Route 13, Gruppe 5: Lange deduktive Strecke. Lesbarer öffentlicher Einstieg: A an E/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 55 konkrete Anschlussfolgen und 28 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 20/49 Zellen, 15 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 26. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....┌──
01 ...┌┘..
02 ...│...
03 .┌┐│...
04 ┌┘└┘...
05 │┌┐....
06 └┘└┐...
```
