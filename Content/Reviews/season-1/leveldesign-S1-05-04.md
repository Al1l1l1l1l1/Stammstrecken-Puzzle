# Author-Leveldesignreview: S1-05-04 (globale Route 20)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Souveräner Wiedereinstieg | S1-05-04-01: 7, S1-05-04-02: 8 |
| 3–5 | Komplexe Belegung | S1-05-04-03: 12, S1-05-04-04: 13, S1-05-04-05: 11 |
| 6–8 | Mehrere tiefe 10×10-Ketten | S1-05-04-06: 23, S1-05-04-07: 23, S1-05-04-08: 27 |
| 9–10 | Finale räumliche Variation | S1-05-04-09: 15, S1-05-04-10: 17 |
| 11–12 | Finale 10×10-Meldungen | S1-05-04-11: 42, S1-05-04-12: 40 |

Die Positionen 1–2 setzen direkte öffentliche Anker; 3–5 verändern den im Brief benannten Schwerpunkt; 6–8 verbinden ihn mit Folgeableitungen. 9–10 dienen räumlicher Variation und dürfen eine geringere Root-Tiefe als die Verdichtung haben. 11–12 sind zwei deduktive Abschlüsse. Root-Tiefe ist ein Solverabhängigkeitsmaß und keine bestätigte menschliche Schwierigkeit.

**Offener fachlicher Prüfpunkt:** Die große Tiefenzunahme an mehreren Routenabschlüssen muss im unabhängigen Contentreview und Spieltest auf Zugänglichkeit geprüft werden. Geringe Dichte kann lange Ketten über viele Achsen erzeugen; sie ist kein automatischer Mangel, hohe Tiefe allein aber kein Qualitäts-PASS.

## S1-05-04-01

- Input: 9×9; A W/3, B S/2. Zeilen `[0, 0, 0, 1, 1, 1, 1, 4, 4]`, Spalten `[6, 2, 2, 2, 0, 0, 0, 0, 0]`.
- Lösung: 12 Zellen, 7 Kurven; Dichte 12/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=131, maxDeductionDepth=7.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 69, 114, 123, 124, 125, 126]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-04-01, globale Route 20, Gruppe 1: Souveräner Wiedereinstieg. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 21 konkrete Anschlussfolgen und 17 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 12/81 Zellen, 7 Kurven, 8 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 7. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .........
01 .........
02 .........
03 ┐........
04 │........
05 │........
06 │........
07 │┌─┐.....
08 └┘┌┘.....
```

## S1-05-04-02

- Input: 9×9; A N/1, B S/8. Zeilen `[8, 1, 8, 1, 7, 1, 7, 1, 8]`, Spalten `[0, 7, 5, 5, 5, 5, 5, 6, 4]`.
- Lösung: 42 Zellen, 10 Kurven; Dichte 42/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=177, maxDeductionDepth=8.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 56, 73, 76, 102, 110, 135, 155]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-04-02, globale Route 20, Gruppe 1: Souveräner Wiedereinstieg. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 70 konkrete Anschlussfolgen und 30 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 42/81 Zellen, 10 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 8. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .└──────┐
01 ........│
02 .┌──────┘
03 .│.......
04 .└─────┐.
05 .......│.
06 .┌─────┘.
07 .│.......
08 .└──────┐
```

## S1-05-04-03

- Input: 9×9; A W/0, B E/7. Zeilen `[4, 3, 6, 5, 5, 5, 5, 7, 0]`, Spalten `[8, 1, 8, 1, 8, 1, 6, 1, 6]`.
- Lösung: 40 Zellen, 10 Kurven; Dichte 40/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=180, maxDeductionDepth=12.
- Einstieg: Zeile 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 47, 65, 80, 87, 88, 89, 116, 117, 125, 152, 162]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-04-03, globale Route 20, Gruppe 2: Komplexe Belegung. Lesbarer öffentlicher Einstieg: Zeile 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 77 konkrete Anschlussfolgen und 28 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 40/81 Zellen, 10 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 12. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.┌─┐....
01 │.│.│....
02 │.│.│.┌─┐
03 │.│.│.│.│
04 │.│.│.│.│
05 │.│.│.│.│
06 │.│.│.│.│
07 └─┘.└─┘.└
08 .........
```

## S1-05-04-04

- Input: 9×9; A E/2, B W/6. Zeilen `[8, 4, 3, 1, 1, 3, 3, 2, 0]`, Spalten `[8, 6, 3, 1, 1, 1, 1, 2, 2]`.
- Lösung: 25 Zellen, 16 Kurven; Dichte 25/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=180, maxDeductionDepth=13.
- Einstieg: Zeile 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 44, 64, 88, 123, 131, 133, 140, 144, 147, 153, 158, 178]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-04-04, globale Route 20, Gruppe 2: Komplexe Belegung. Lesbarer öffentlicher Einstieg: Zeile 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 75 konkrete Anschlussfolgen und 47 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 25/81 Zellen, 16 Kurven, 1 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 13. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──────┐.
01 └┐.....└┐
02 ┌┘......└
03 │........
04 │........
05 └─┐......
06 ┐┌┘......
07 └┘.......
08 .........
```

## S1-05-04-05

- Input: 9×9; A N/2, B S/5. Zeilen `[7, 2, 2, 2, 1, 2, 3, 4, 1]`, Spalten `[0, 0, 1, 1, 1, 3, 3, 8, 7]`.
- Lösung: 24 Zellen, 16 Kurven; Dichte 24/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=156, maxDeductionDepth=11.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[40, 41, 42, 43, 44, 45, 46, 47]`.
- Eine tatsächliche maximale Prämissenkette: `[28, 36, 84, 85, 107, 123, 150, 151, 152, 153, 155]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-04-05, globale Route 20, Gruppe 2: Komplexe Belegung. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 51 konkrete Anschlussfolgen und 39 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 24/81 Zellen, 16 Kurven, 2 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 11. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└─────┐
01 .......┌┘
02 .......└┐
03 .......┌┘
04 .......│.
05 .......└┐
06 ......┌┐│
07 .....┌┘└┘
08 .....│...
```

## S1-05-04-06

- Input: 10×10; A N/2, B S/6. Zeilen `[8, 1, 8, 1, 6, 1, 6, 1, 5, 1]`, Spalten `[0, 0, 7, 5, 5, 5, 6, 5, 2, 3]`.
- Lösung: 38 Zellen, 10 Kurven; Dichte 38/100. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=202, maxDeductionDepth=23.
- Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[45, 46, 47, 48, 49, 50, 51, 52, 53]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 85, 95, 104, 106, 108, 110, 112, 114, 137, 139, 152, 153, 156, 158, 160, 162, 164, 166, 170, 171, 185, 197]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-04-06, globale Route 20, Gruppe 3: Mehrere tiefe 10×10-Ketten. Lesbarer öffentlicher Einstieg: Spalte 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 82 konkrete Anschlussfolgen und 35 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 38/100 Zellen, 10 Kurven, 2 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 23. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└──────┐
01 .........│
02 ..┌──────┘
03 ..│.......
04 ..└────┐..
05 .......│..
06 ..┌────┘..
07 ..│.......
08 ..└───┐...
09 ......│...
```

## S1-05-04-07

- Input: 10×10; A W/1, B E/7. Zeilen `[3, 3, 3, 6, 5, 5, 7, 2, 0, 0]`, Spalten `[6, 1, 7, 1, 7, 1, 4, 1, 5, 1]`.
- Lösung: 34 Zellen, 10 Kurven; Dichte 34/100. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=222, maxDeductionDepth=23.
- Einstieg: Zeile 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[36, 37, 38, 39, 40, 41, 42, 43, 44, 45]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 62, 79, 80, 83, 90, 118, 124, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 153, 166, 190, 195, 204]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-04-07, globale Route 20, Gruppe 3: Mehrere tiefe 10×10-Ketten. Lesbarer öffentlicher Einstieg: Zeile 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 101 konkrete Anschlussfolgen und 40 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 34/100 Zellen, 10 Kurven, 2 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 23. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌─┐.....
01 ┐.│.│.....
02 │.│.│.....
03 │.│.│.┌─┐.
04 │.│.│.│.│.
05 │.│.│.│.│.
06 └─┘.└─┘.│.
07 ........└─
08 ..........
09 ..........
```

## S1-05-04-08

- Input: 10×10; A S/0, B N/6. Zeilen `[6, 6, 2, 2, 2, 2, 2, 4, 1, 1]`, Spalten `[3, 1, 8, 8, 2, 2, 2, 2, 0, 0]`.
- Lösung: 28 Zellen, 22 Kurven; Dichte 28/100. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=210, maxDeductionDepth=27.
- Einstieg: Spalte 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[45, 46, 47, 48, 49, 50, 51, 52, 53]`.
- Eine tatsächliche maximale Prämissenkette: `[26, 36, 78, 79, 81, 93, 141, 166, 167, 168, 169, 170, 172, 173, 174, 176, 178, 179, 180, 182, 184, 185, 186, 188, 205, 206, 208]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-04-08, globale Route 20, Gruppe 3: Mehrere tiefe 10×10-Ketten. Lesbarer öffentlicher Einstieg: Spalte 8 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 83 konkrete Anschlussfolgen und 50 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 28/100 Zellen, 22 Kurven, 2 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 27. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌┐┌┐└┐..
01 ..│└┘└─┘..
02 ..└┐......
03 ..┌┘......
04 ..└┐......
05 ..┌┘......
06 ..└┐......
07 ┌──┘......
08 │.........
09 │.........
```

## S1-05-04-09

- Input: 10×10; A S/3, B W/4. Zeilen `[0, 0, 0, 3, 7, 7, 3, 1, 1, 5]`, Spalten `[1, 1, 1, 3, 2, 2, 3, 7, 3, 4]`.
- Lösung: 27 Zellen, 13 Kurven; Dichte 27/100. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=186, maxDeductionDepth=15.
- Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. Belegschritte: `[36, 37, 38, 39, 40, 41, 42, 43, 44, 45]`.
- Eine tatsächliche maximale Prämissenkette: `[16, 66, 88, 89, 114, 128, 129, 130, 131, 167, 173, 174, 180, 183, 184]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-04-09, globale Route 20, Gruppe 4: Finale räumliche Variation. Lesbarer öffentlicher Einstieg: Zeile 0 hat Count 0: alle Zellen dieser Linie sind leer. Das ist eine sichere öffentliche Aussage, keine Spielermarkierungspflicht. 63 konkrete Anschlussfolgen und 40 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 27/100 Zellen, 13 Kurven, 3 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 15. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..........
01 ..........
02 ..........
03 .......┌─┐
04 ───┐..┌┘.│
05 ...└──┘┌┐│
06 .......│└┘
07 .......│..
08 .......│..
09 ...┌───┘..
```

## S1-05-04-10

- Input: 10×10; A W/0, B S/9. Zeilen `[1, 1, 1, 1, 1, 1, 2, 5, 7, 9]`, Spalten `[10, 4, 2, 2, 2, 3, 1, 3, 1, 1]`.
- Lösung: 29 Zellen, 19 Kurven; Dichte 29/100. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=199, maxDeductionDepth=17.
- Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. Belegschritte: `[45, 46, 47, 48, 49, 50, 51, 52, 53]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 36, 64, 123, 146, 157, 166, 167, 168, 172, 174, 175, 188, 189, 193, 194, 197]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-04-10, globale Route 20, Gruppe 4: Finale räumliche Variation. Lesbarer öffentlicher Einstieg: Spalte 0 verlangt die gesamte Linienlänge: alle Zellen dieser Linie müssen belegt sein. Die Form bleibt zunächst offen. 70 konkrete Anschlussfolgen und 71 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 29/100 Zellen, 19 Kurven, 0 Null- und 1 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 17. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.........
01 │.........
02 │.........
03 │.........
04 │.........
05 │.........
06 └┐........
07 ┌┘...┌─┐..
08 │┌┐┌┐│.│..
09 └┘└┘└┘.└─┐
```

## S1-05-04-11

- Input: 10×10; A S/0, B E/1. Zeilen `[2, 2, 2, 3, 1, 3, 1, 4, 3, 1]`, Spalten `[2, 1, 2, 1, 1, 3, 1, 3, 4, 4]`.
- Lösung: 22 Zellen, 13 Kurven; Dichte 22/100. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=231, maxDeductionDepth=42.
- Einstieg: A an S/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[26]`.
- Eine tatsächliche maximale Prämissenkette: `[26, 36, 54, 55, 63, 72, 73, 81, 90, 91, 92, 106, 107, 114, 122, 123, 130, 138, 139, 146, 154, 155, 156, 164, 165, 166, 174, 175, 180, 186, 187, 192, 198, 199, 203, 207, 208, 210, 217, 221, 223, 229]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-04-11, globale Route 20, Gruppe 5: Finale 10×10-Meldungen. Lesbarer öffentlicher Einstieg: A an S/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 109 konkrete Anschlussfolgen und 78 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 22/100 Zellen, 13 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 42. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ........┌┐
01 ........│└
02 ........└┐
03 .......┌─┘
04 .......│..
05 .....┌─┘..
06 .....│....
07 ..┌──┘....
08 ┌─┘.......
09 │.........
```

## S1-05-04-12

- Input: 10×10; A E/0, B S/1. Zeilen `[1, 3, 1, 1, 1, 1, 1, 5, 5, 5]`, Spalten `[2, 2, 2, 3, 3, 1, 1, 7, 1, 2]`.
- Lösung: 24 Zellen, 13 Kurven; Dichte 24/100. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=235, maxDeductionDepth=40.
- Einstieg: A an E/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. Belegschritte: `[9]`.
- Eine tatsächliche maximale Prämissenkette: `[9, 44, 45, 55, 56, 64, 73, 74, 82, 91, 98, 99, 107, 114, 115, 123, 130, 131, 139, 146, 147, 155, 162, 163, 171, 178, 185, 193, 194, 196, 199, 200, 202, 205, 206, 210, 224, 225, 230, 231]` im zugehörigen Rootspurbericht.
- Fachliche Begründung: Authorbewertung S1-05-04-12, globale Route 20, Gruppe 5: Finale 10×10-Meldungen. Lesbarer öffentlicher Einstieg: A an E/0 erzwingt Belegung und passenden Außenport der angrenzenden Zelle. Von dort Randzahlreste und Gegenports kombinieren. 112 konkrete Anschlussfolgen und 76 Restzahlabschlüsse verbinden die Belegung mit Geometrie. 24/100 Zellen, 13 Kurven, 0 Null- und 0 Voll-Linien ergeben die konkrete räumliche Variante. Fairness technisch belegt: UNIQUE/1 ohne Suchannahme; maximale tatsächliche Root-Prämissenkette 40. Keine identische öffentliche Aufgabe unter Rotation, Spiegelung oder A/B-Tausch im 240er-Bestand. Wiederkehrende Regeln dienen dem Lernbogen; menschliche Lesbarkeit, erwartete Schwierigkeit und Nichtredundanz benötigen unabhängigen Contentreview und Spieltest.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .........┌
01 .......┌─┘
02 .......│..
03 .......│..
04 .......│..
05 .......│..
06 .......│..
07 ...┌───┘..
08 ┌─┐└┐.....
09 └┐└─┘.....
```
