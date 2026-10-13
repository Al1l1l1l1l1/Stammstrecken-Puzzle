# Author-Leveldesignreview: S1-04-04 (globale Route 16)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Klare Startanker | S1-04-04-01: 7, S1-04-04-02: 7 |
| 3–5 | Alternativen schrittweise ausschließen | S1-04-04-03: 10, S1-04-04-04: 11, S1-04-04-05: 12 |
| 6–8 | Hohe Verzahnung | S1-04-04-06: 12, S1-04-04-07: 13, S1-04-04-08: 14 |
| 9–10 | Knappe Counts und andere Endpunkte | S1-04-04-09: 11, S1-04-04-10: 15 |
| 11–12 | Strategischer Abschnittsabschluss | S1-04-04-11: 33, S1-04-04-12: 30 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-04-04-01

- Input: 8×8; A W/0, B E/0. Zeilen `[8, 6, 0, 0, 0, 0, 0, 0]`, Spalten `[1, 2, 2, 1, 2, 2, 2, 2]`.
- Lösung: 14 Zellen, 12 Kurven; Dichte 14/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=109, maxDeductionDepth=7.
- Einstieg: Zeile 2 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[34, 35, 36, 37, 38, 39, 40, 41]`.
- Eine tatsächliche maximale Prämissenkette: `[30, 85, 92, 93, 103, 106, 107]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-04-01, globale Route 16, Gruppe 1: Klare Startanker. Lesbarer öffentlicher Einstieg: Zeile 2 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 19 konkrete Anschlussfolgen und 2 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 14/64 Zellen, 12 Kurven, 6 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ─┐┌─┐┌┐┌
01 .└┘.└┘└┘
02 ........
03 ........
04 ........
05 ........
06 ........
07 ........
```

## S1-04-04-02

- Input: 8×8; A W/0, B E/2. Zeilen `[1, 1, 6, 4, 4, 4, 4, 6]`, Spalten `[8, 1, 6, 1, 6, 1, 6, 1]`.
- Lösung: 30 Zellen, 8 Kurven; Dichte 30/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=140, maxDeductionDepth=7.
- Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[35, 36, 37, 38, 39, 40, 41]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 29, 51, 76, 96, 112, 123]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-04-02, globale Route 16, Gruppe 1: Klare Startanker. Lesbarer öffentlicher Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 54 konkrete Anschlussfolgen und 34 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 30/64 Zellen, 8 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.......
01 │.......
02 │.┌─┐.┌─
03 │.│.│.│.
04 │.│.│.│.
05 │.│.│.│.
06 │.│.│.│.
07 └─┘.└─┘.
```

## S1-04-04-03

- Input: 8×8; A N/1, B S/1. Zeilen `[7, 1, 7, 1, 7, 1, 7, 1]`, Spalten `[0, 6, 4, 4, 4, 4, 4, 6]`.
- Lösung: 32 Zellen, 8 Kurven; Dichte 32/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=133, maxDeductionDepth=10.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[35, 36, 37, 38, 39, 40, 41]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 57, 83, 98, 126, 127, 128, 129, 130, 131]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-04-03, globale Route 16, Gruppe 2: Alternativen schrittweise ausschließen. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 50 konkrete Anschlussfolgen und 22 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 32/64 Zellen, 8 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 10. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .└─────┐
01 .......│
02 .┌─────┘
03 .│......
04 .└─────┐
05 .......│
06 .┌─────┘
07 .│......
```

## S1-04-04-04

- Input: 8×8; A E/2, B W/7. Zeilen `[0, 0, 1, 1, 2, 1, 6, 8]`, Spalten `[2, 2, 1, 1, 2, 2, 4, 5]`.
- Lösung: 19 Zellen, 14 Kurven; Dichte 19/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=124, maxDeductionDepth=11.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[52, 61, 92, 100, 112, 116, 117, 118, 120, 121, 123]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-04-04, globale Route 16, Gruppe 2: Alternativen schrittweise ausschließen. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 38 konkrete Anschlussfolgen und 28 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/64 Zellen, 14 Kurven, 2 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ........
01 ........
02 .......┌
03 .......│
04 ......┌┘
05 ......│.
06 ┌┐..┌┐└┐
07 ┘└──┘└─┘
```

## S1-04-04-05

- Input: 8×8; A W/0, B E/5. Zeilen `[2, 1, 1, 1, 2, 7, 2, 3]`, Spalten `[1, 8, 3, 3, 1, 1, 1, 1]`.
- Lösung: 19 Zellen, 8 Kurven; Dichte 19/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=132, maxDeductionDepth=12.
- Einstieg: Spalte 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[35, 36, 37, 38, 39, 40, 41, 42]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 47, 62, 68, 102, 116, 118, 119, 121, 128, 129, 130]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-04-05, globale Route 16, Gruppe 2: Alternativen schrittweise ausschließen. Lesbarer öffentlicher Einstieg: Spalte 1 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 45 konkrete Anschlussfolgen und 45 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/64 Zellen, 8 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ─┐......
01 .│......
02 .│......
03 .│......
04 .└┐.....
05 .┌┘┌────
06 .│.│....
07 .└─┘....
```

## S1-04-04-06

- Input: 8×8; A E/5, B W/2. Zeilen `[7, 2, 2, 3, 1, 4, 2, 4]`, Spalten `[3, 1, 1, 4, 5, 3, 7, 1]`.
- Lösung: 25 Zellen, 10 Kurven; Dichte 25/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=138, maxDeductionDepth=12.
- Einstieg: A an E/5 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[17]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 28, 45, 57, 82, 87, 99, 107, 119, 123, 125, 131]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-04-06, globale Route 16, Gruppe 3: Hohe Verzahnung. Lesbarer öffentlicher Einstieg: A an E/5 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 61 konkrete Anschlussfolgen und 34 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 25/64 Zellen, 10 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─────┐.
01 │.....│.
02 ┘.....│.
03 ....┌─┘.
04 ....│...
05 ...┌┘.┌─
06 ...│..│.
07 ...└──┘.
```

## S1-04-04-07

- Input: 8×8; A N/4, B S/2. Zeilen `[2, 1, 4, 5, 5, 1, 1, 3]`, Spalten `[0, 0, 1, 4, 7, 5, 3, 2]`.
- Lösung: 22 Zellen, 16 Kurven; Dichte 22/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=145, maxDeductionDepth=13.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[22, 47, 64, 72, 89, 94, 100, 125, 126, 135, 137, 138, 139]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-04-07, globale Route 16, Gruppe 3: Hohe Verzahnung. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 59 konkrete Anschlussfolgen und 26 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 22/64 Zellen, 16 Kurven, 2 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 13. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....└┐..
01 .....│..
02 ...┌┐└┐.
03 ...│└┐└┐
04 ...└┐└─┘
05 ....│...
06 ....│...
07 ..┌─┘...
```

## S1-04-04-08

- Input: 8×8; A S/2, B E/2. Zeilen `[6, 3, 5, 4, 2, 1, 1, 3]`, Spalten `[8, 6, 2, 1, 1, 4, 2, 1]`.
- Lösung: 25 Zellen, 15 Kurven; Dichte 25/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=141, maxDeductionDepth=14.
- Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[33, 56, 75, 87, 101, 103, 104, 105, 106, 126, 127, 128, 129, 134]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-04-08, globale Route 16, Gruppe 3: Hohe Verzahnung. Lesbarer öffentlicher Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 62 konkrete Anschlussfolgen und 35 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 25/64 Zellen, 15 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 14. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌────┐..
01 └┐...│..
02 ┌┘...│┌─
03 └┐...└┘.
04 ┌┘......
05 │.......
06 │.......
07 └─┐.....
```

## S1-04-04-09

- Input: 8×8; A E/6, B W/3. Zeilen `[0, 0, 2, 5, 4, 2, 5, 7]`, Spalten `[1, 4, 6, 6, 4, 1, 1, 2]`.
- Lösung: 25 Zellen, 20 Kurven; Dichte 25/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=136, maxDeductionDepth=11.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 48, 86, 94, 107, 123, 127, 128, 129, 131, 133]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-04-09, globale Route 16, Gruppe 4: Knappe Counts und andere Endpunkte. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 47 konkrete Anschlussfolgen und 23 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 25/64 Zellen, 20 Kurven, 2 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ........
01 ........
02 ..┌┐....
03 ─┐│└┐...
04 .└┘┌┘...
05 ..┌┘....
06 .┌┘┌┐..┌
07 .└─┘└──┘
```

## S1-04-04-10

- Input: 8×8; A S/7, B W/4. Zeilen `[0, 2, 6, 4, 4, 1, 1, 1]`, Spalten `[1, 1, 4, 2, 2, 2, 1, 6]`.
- Lösung: 19 Zellen, 9 Kurven; Dichte 19/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=141, maxDeductionDepth=15.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[27, 42, 66, 67, 73, 88, 89, 96, 101, 106, 126, 133, 137, 138, 139]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-04-10, globale Route 16, Gruppe 4: Knappe Counts und andere Endpunkte. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 59 konkrete Anschlussfolgen und 36 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 19/64 Zellen, 9 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 15. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ........
01 ..┌┐....
02 ..│└┐┌─┐
03 ..│.└┘.│
04 ──┘....│
05 .......│
06 .......│
07 .......│
```

## S1-04-04-11

- Input: 8×8; A W/7, B N/7. Zeilen `[2, 2, 3, 3, 4, 1, 1, 1]`, Spalten `[4, 1, 1, 2, 1, 2, 3, 3]`.
- Lösung: 17 Zellen, 11 Kurven; Dichte 17/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=152, maxDeductionDepth=33.
- Einstieg: A an W/7 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[20]`.
- Eine tatsächliche maximale Prämissenkette: `[20, 28, 42, 43, 44, 58, 59, 60, 74, 75, 79, 84, 85, 89, 94, 95, 99, 104, 105, 106, 114, 115, 118, 122, 123, 126, 130, 131, 135, 139, 143, 145, 150]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-04-11, globale Route 16, Gruppe 5: Strategischer Abschnittsabschluss. Lesbarer öffentlicher Einstieg: A an W/7 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 71 konkrete Anschlussfolgen und 47 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 17/64 Zellen, 11 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 33. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ......┌┘
01 ......└┐
02 .....┌─┘
03 ...┌─┘..
04 ┌──┘....
05 │.......
06 │.......
07 ┘.......
```

## S1-04-04-12

- Input: 8×8; A N/5, B W/6. Zeilen `[7, 4, 3, 5, 7, 2, 2, 2]`, Spalten `[8, 6, 4, 4, 1, 2, 2, 5]`.
- Lösung: 32 Zellen, 19 Kurven; Dichte 32/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=155, maxDeductionDepth=30.
- Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[28, 29, 30, 31, 32, 33, 34]`.
- Eine tatsächliche maximale Prämissenkette: `[20, 37, 40, 41, 48, 67, 78, 83, 89, 97, 100, 104, 108, 109, 113, 118, 119, 127, 128, 132, 135, 136, 137, 139, 142, 143, 147, 150, 152, 154]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-04-04-12, globale Route 16, Gruppe 5: Strategischer Abschnittsabschluss. Lesbarer öffentlicher Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 83 konkrete Anschlussfolgen und 26 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 32/64 Zellen, 19 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 30. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──┐.└─┐
01 │.┌┘...│
02 │.│....│
03 └┐└┐...│
04 ┌┘.└───┘
05 └┐......
06 ┐│......
07 └┘......
```
