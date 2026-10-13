# Author-Leveldesignreview: S1-01-04 (globale Route 04)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Einstiegsmuster | S1-01-04-01: 4, S1-01-04-02: 5 |
| 3–5 | Hohe und niedrige Randzahlen | S1-01-04-03: 6, S1-01-04-04: 6, S1-01-04-05: 6 |
| 6–8 | Belegungsbereiche verbinden | S1-01-04-06: 7, S1-01-04-07: 7, S1-01-04-08: 7 |
| 9–10 | Gegenüberliegende Endpunkte | S1-01-04-09: 7, S1-01-04-10: 7 |
| 11–12 | Abschnittskompetenz | S1-01-04-11: 9, S1-01-04-12: 12 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-01-04-01

- Input: 4×5; A W/2, B E/4. Zeilen `[0, 0, 1, 3, 4]`, Spalten `[3, 1, 2, 2]`.
- Lösung: 8 Zellen, 6 Kurven; Dichte 8/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=39, maxDeductionDepth=4.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 23, 30, 37]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-04-01, globale Route 4, Gruppe 1: Einstiegsmuster. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 7 konkrete Anschlussfolgen und 4 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 8/20 Zellen, 6 Kurven, 2 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 4. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....
01 ....
02 ┐...
03 │.┌┐
04 └─┘└
```

## S1-01-04-02

- Input: 4×5; A N/3, B E/1. Zeilen `[2, 3, 3, 0, 0]`, Spalten `[0, 2, 3, 3]`.
- Lösung: 8 Zellen, 7 Kurven; Dichte 8/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=46, maxDeductionDepth=5.
- Einstieg: Zeile 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[9, 28, 34, 36, 38]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-04-02, globale Route 4, Gruppe 1: Einstiegsmuster. Lesbarer öffentlicher Einstieg: Zeile 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 14 konkrete Anschlussfolgen und 1 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 8/20 Zellen, 7 Kurven, 3 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 5. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌┘
01 .┌┘┌
02 .└─┘
03 ....
04 ....
```

## S1-01-04-03

- Input: 4×5; A N/0, B S/1. Zeilen `[4, 3, 1, 4, 4]`, Spalten `[4, 4, 3, 5]`.
- Lösung: 16 Zellen, 10 Kurven; Dichte 16/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=52, maxDeductionDepth=6.
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[14, 15, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[15, 25, 32, 35, 40, 41]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-04-03, globale Route 4, Gruppe 2: Hohe und niedrige Randzahlen. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 20 konkrete Anschlussfolgen und 4 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 16/20 Zellen, 10 Kurven, 0 Null- und 4 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 │┌─┐
01 └┘.│
02 ...│
03 ┌─┐│
04 └┐└┘
```

## S1-01-04-04

- Input: 4×5; A W/1, B S/1. Zeilen `[2, 2, 1, 1, 1]`, Spalten `[2, 5, 0, 0]`.
- Lösung: 7 Zellen, 3 Kurven; Dichte 7/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=45, maxDeductionDepth=6.
- Einstieg: Spalte 2 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[2, 29, 35, 36, 37, 39]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-04-04, globale Route 4, Gruppe 2: Hohe und niedrige Randzahlen. Lesbarer öffentlicher Einstieg: Spalte 2 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 14 konkrete Anschlussfolgen und 5 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 7/20 Zellen, 3 Kurven, 2 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐..
01 ┘│..
02 .│..
03 .│..
04 .│..
```

## S1-01-04-05

- Input: 4×5; A N/1, B S/1. Zeilen `[4, 4, 4, 1, 2]`, Spalten `[3, 4, 5, 3]`.
- Lösung: 15 Zellen, 12 Kurven; Dichte 15/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=50, maxDeductionDepth=6.
- Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[14, 15, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 25, 38, 44, 45, 46]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-04-05, globale Route 4, Gruppe 2: Hohe und niedrige Randzahlen. Lesbarer öffentlicher Einstieg: Zeile 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 18 konkrete Anschlussfolgen und 5 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 15/20 Zellen, 12 Kurven, 0 Null- und 4 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 6. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘┌┐
01 │┌┘│
02 └┘┌┘
03 ..│.
04 .┌┘.
```

## S1-01-04-06

- Input: 4×5; A S/2, B W/1. Zeilen `[3, 3, 2, 3, 3]`, Spalten `[5, 5, 4, 0]`.
- Lösung: 14 Zellen, 11 Kurven; Dichte 14/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=53, maxDeductionDepth=7.
- Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[23, 24, 25, 26, 27]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 29, 45, 46, 48, 49, 51]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-04-06, globale Route 4, Gruppe 3: Belegungsbereiche verbinden. Lesbarer öffentlicher Einstieg: Spalte 3 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 21 konkrete Anschlussfolgen und 1 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 14/20 Zellen, 11 Kurven, 1 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐.
01 ┘┌┘.
02 ┌┘..
03 │┌┐.
04 └┘│.
```

## S1-01-04-07

- Input: 4×5; A S/0, B E/1. Zeilen `[0, 2, 4, 4, 3]`, Spalten `[3, 2, 4, 4]`.
- Lösung: 13 Zellen, 9 Kurven; Dichte 13/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=48, maxDeductionDepth=7.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[19, 27, 32, 33, 37, 40, 46]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-04-07, globale Route 4, Gruppe 3: Belegungsbereiche verbinden. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 16 konkrete Anschlussfolgen und 3 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 13/20 Zellen, 9 Kurven, 1 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....
01 ..┌─
02 ┌┐└┐
03 │└┐│
04 │.└┘
```

## S1-01-04-08

- Input: 4×5; A N/2, B E/4. Zeilen `[2, 2, 2, 2, 2]`, Spalten `[0, 0, 5, 5]`.
- Lösung: 10 Zellen, 9 Kurven; Dichte 10/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=44, maxDeductionDepth=7.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[14, 15, 16, 17, 18]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 32, 33, 34, 36, 41, 42]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-04-08, globale Route 4, Gruppe 3: Belegungsbereiche verbinden. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 12 konkrete Anschlussfolgen und 0 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 10/20 Zellen, 9 Kurven, 2 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└┐
01 ..┌┘
02 ..└┐
03 ..┌┘
04 ..└─
```

## S1-01-04-09

- Input: 4×5; A W/1, B E/3. Zeilen `[4, 4, 2, 2, 0]`, Spalten `[2, 2, 4, 4]`.
- Lösung: 12 Zellen, 10 Kurven; Dichte 12/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=47, maxDeductionDepth=7.
- Einstieg: Zeile 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[15, 27, 35, 41, 42, 43, 44]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-04-09, globale Route 4, Gruppe 4: Gegenüberliegende Endpunkte. Lesbarer öffentlicher Einstieg: Zeile 4 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 15 konkrete Anschlussfolgen und 4 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 12/20 Zellen, 10 Kurven, 1 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐┌┐
01 ┘└┘│
02 ..┌┘
03 ..└─
04 ....
```

## S1-01-04-10

- Input: 4×5; A E/0, B W/1. Zeilen `[1, 3, 2, 3, 2]`, Spalten `[1, 3, 2, 5]`.
- Lösung: 11 Zellen, 6 Kurven; Dichte 11/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=49, maxDeductionDepth=7.
- Einstieg: Spalte 3 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[4, 18, 36, 39, 43, 46, 48]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-04-10, globale Route 4, Gruppe 4: Gegenüberliegende Endpunkte. Lesbarer öffentlicher Einstieg: Spalte 3 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 19 konkrete Anschlussfolgen und 9 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 11/20 Zellen, 6 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...┌
01 ─┐.│
02 .│.│
03 .└┐│
04 ..└┘
```

## S1-01-04-11

- Input: 4×5; A W/4, B S/3. Zeilen `[3, 2, 3, 2, 2]`, Spalten `[5, 1, 3, 3]`.
- Lösung: 12 Zellen, 5 Kurven; Dichte 12/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=51, maxDeductionDepth=9.
- Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[16, 17, 18, 19]`.
- Eine tatsächliche maximale Prämissenkette: `[10, 15, 25, 27, 29, 34, 41, 46, 50]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-04-11, globale Route 4, Gruppe 5: Abschnittskompetenz. Lesbarer öffentlicher Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 22 konkrete Anschlussfolgen und 8 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 12/20 Zellen, 5 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 9. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐.
01 │.│.
02 │.└┐
03 │..│
04 ┘..│
```

## S1-01-04-12

- Input: 4×5; A E/3, B W/4. Zeilen `[3, 4, 4, 3, 1]`, Spalten `[4, 4, 3, 4]`.
- Lösung: 15 Zellen, 14 Kurven; Dichte 15/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=54, maxDeductionDepth=12.
- Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[10, 24, 27, 28, 35, 37, 38, 41, 42, 46, 50, 53]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-01-04-12, globale Route 4, Gruppe 5: Abschnittskompetenz. Lesbarer öffentlicher Einstieg: Zeile 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 24 konkrete Anschlussfolgen und 5 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 15/20 Zellen, 14 Kurven, 0 Null- und 2 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌─┐
01 ┌┘┌┘
02 └┐└┐
03 ┌┘.└
04 ┘...
```
