# Author-Leveldesignreview: S1-02-03 (globale Route 07)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Bekannte Sicherheiten | S1-02-03-01: 5, S1-02-03-02: 5 |
| 3–5 | Mittlere Randzahlen | S1-02-03-03: 9, S1-02-03-04: 9, S1-02-03-05: 9 |
| 6–8 | Teilwege durch Folgeausschlüsse | S1-02-03-06: 11, S1-02-03-07: 11, S1-02-03-08: 12 |
| 9–10 | Asymmetrie und Kurven | S1-02-03-09: 8, S1-02-03-10: 9 |
| 11–12 | Anspruchsvolle 6×5-Kette | S1-02-03-11: 12, S1-02-03-12: 16 |

**Individuelle Routenbewertung:** Nach dem ruhigen Rückgriff isolieren 3–5 zunehmend mittlere Counts, mit jeweils höchstens einem Extremanker. Bei 6 wächst nur die Breite; Fünferzeilen bilden die klaren neuen Anker. 7/8 testen den Ausschluss einer direkten Verbindung mit unterschiedlichen Querarmen. 9/10 sind absichtlich nahe als oben/unten-Dichtevergleich, kein behauptetes Neuheits-PASS aus unterschiedlichen IDs. Die Abschlüsse verlangen breites Überkreuzlesen; der Wachstumsschritt liegt konkret in den entfernten Portgebieten und Lückenpositionen.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-02-03-01

- Input: 5×5; A W/0, B E/3. Zeilen `[4, 3, 3, 4, 0]`, Spalten `[4, 1, 4, 1, 4]`.
- Lösung: 14 Zellen, 6 Kurven; Dichte 14/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=57, maxDeductionDepth=5.
- Einstieg: Die untere Nullzeile begrenzt drei lange senkrechte Bereiche. Belegschritte: `[16, 17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[15, 28, 31, 39, 45]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'EXCLUSION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die niedrigen Counts 1 in Spalten 1/3 erlauben nur Querübergänge, die Viererspalten tragen die langen Schenkel. Die nahe Belegung zu S1-02-02-01 wird bewusst als ruhige Reaktivierung behalten: kein direkter Vollcount, andere Seiteneintritte und nur ein niedriger Übergang je Zwischenachse.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.┌─┐
01 │.│.│
02 │.│.│
03 └─┘.└
04 .....
```

## S1-02-03-02

- Input: 5×5; A S/4, B W/4. Zeilen `[0, 0, 0, 2, 5]`, Spalten `[1, 1, 2, 2, 1]`.
- Lösung: 7 Zellen, 5 Kurven; Dichte 7/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=46, maxDeductionDepth=5.
- Einstieg: Die volle untere Zeile ist ein vertrauter sicherer Einstieg. Belegschritte: `[31, 32, 33]`.
- Eine tatsächliche maximale Prämissenkette: `[31, 35, 39, 43, 44]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY']`; erwartete gedankliche Folge 3 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Nur die Zweierreihe darüber verlangt einen mittigen Umweg, die oberen drei Nullzeilen entfernen Ablenkung. Dieses kurze zweite Wiedereinstiegsfeld ist keine neue Schwerpunktlektion und passt zur ruhigen Aktivierung vor mittleren Randzahlen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .....
01 .....
02 .....
03 ..┌┐.
04 ──┘└┐
```

## S1-02-03-03

- Input: 5×5; A S/3, B W/2. Zeilen `[4, 3, 4, 1, 2]`, Spalten `[1, 3, 1, 4, 5]`.
- Lösung: 14 Zellen, 9 Kurven; Dichte 14/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=58, maxDeductionDepth=9.
- Einstieg: Die volle rechte Spalte ist der einzige Extremanker; die übrigen Zeilenwerte müssen kombiniert werden. Belegschritte: `[20, 21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 16, 38, 45, 49, 50, 52, 53, 56]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der linke Westport belegt die Einzerspalte 0, während die Dreierspalte 1 den oberen Arm festlegt. Ein kleiner rechter Rückknick trennt zwei längere Schenkel; damit arbeiten mittlere Counts und Endpunktrestzahlen gemeinsam, nicht eine Folge voller Linien.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌──┐
01 .│.┌┘
02 ─┘.└┐
03 ....│
04 ...┌┘
```

## S1-02-03-04

- Input: 5×5; A S/1, B N/3. Zeilen `[4, 1, 2, 2, 2]`, Spalten `[3, 4, 3, 1, 0]`.
- Lösung: 11 Zellen, 8 Kurven; Dichte 11/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=56, maxDeductionDepth=9.
- Einstieg: Die rechte Nullspalte schließt eine Randseite aus. Belegschritte: `[20, 21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[4, 31, 34, 35, 41, 42, 45, 51, 52]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'COMBINATION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Nordanschluss verbraucht Count 1 in Spalte 3, was dessen übrige Zellen entfernt. Zwei Viererwerte oben/links reichen nicht für eine fertige Linie; die untere kleine Kurve muss mit den beiden mittleren Dreier-/Vierercounts zusammenpassen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──┘.
01 │....
02 └┐...
03 .└┐..
04 .┌┘..
```

## S1-02-03-05

- Input: 5×5; A W/2, B N/1. Zeilen `[1, 2, 4, 4, 3]`, Spalten `[1, 4, 4, 3, 2]`.
- Lösung: 14 Zellen, 11 Kurven; Dichte 14/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=66, maxDeductionDepth=9.
- Einstieg: B an N/1 belegt die Einserzeile oben allein. Belegschritte: `[1, 7]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 16, 27, 28, 41, 48, 53, 54, 58]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Alle Achsen sind aktiv, kein Count ist Null oder voll; Spalte 0 wird allein vom Westport belegt. Der breite untere Rücklauf verbindet die versetzten mittleren Teilbögen, deshalb ist die höhere Kurvenlast ein konkreter Fortschritt gegenüber den langen Schenkeln von 3/4.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .│...
01 .└┐..
02 ─┐└┐.
03 .└┐└┐
04 ..└─┘
```

## S1-02-03-06

- Input: 6×5; A W/3, B S/5. Zeilen `[0, 5, 5, 5, 2]`, Spalten `[1, 3, 3, 2, 4, 4]`.
- Lösung: 17 Zellen, 13 Kurven; Dichte 17/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=78, maxDeductionDepth=11.
- Einstieg: Die obere Nullzeile begrenzt das neue 6×5-Feld. Belegschritte: `[18, 19, 20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[10, 24, 47, 55, 60, 65, 68, 69, 70, 75, 76]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'COMBINATION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei Fünferzeilen erlauben jeweils genau eine Lücke, die aber an verschiedenen Spalten liegen muss. Der Einsercount am Westport und die beiden Viererspalten rechts koppeln die Lückenfolge an den Südanschluss; sinnvoller Größenwechsel mit mehreren begrenzten Teilwegen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ......
01 .┌─┐┌┐
02 .└┐└┘│
03 ──┘.┌┘
04 ....└┐
```

## S1-02-03-07

- Input: 6×5; A E/4, B N/0. Zeilen `[4, 2, 2, 2, 2]`, Spalten `[1, 1, 1, 4, 4, 1]`.
- Lösung: 12 Zellen, 9 Kurven; Dichte 12/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=73, maxDeductionDepth=11.
- Einstieg: B an N/0 verbraucht Count 1 in Spalte 0. Belegschritte: `[0, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 18, 26, 27, 41, 48, 49, 56, 63, 64, 71]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'CHAIN']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die benachbarten Einsercounts der linken Spalten legen den oberen Querarm fest; die beiden Viererspalten rechts müssen die mittlere Wechselkurve aufnehmen. Trotz eines erkennbaren Knickmotivs unterscheiden sich der lange obere Arm und der Ausgang unten rechts vom vorigen Fünferflächenfeld.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └──┐..
01 ...└┐.
02 ...┌┘.
03 ...└┐.
04 ....└─
```

## S1-02-03-08

- Input: 6×5; A S/4, B N/4. Zeilen `[1, 4, 2, 1, 5]`, Spalten `[3, 3, 2, 2, 3, 0]`.
- Lösung: 13 Zellen, 6 Kurven; Dichte 13/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=71, maxDeductionDepth=12.
- Einstieg: B an N/4 belegt die einzige Zelle der oberen Zeile. Belegschritte: `[4, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[4, 21, 27, 32, 40, 42, 48, 55, 56, 63, 65, 67]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'COMBINATION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die fast volle Unterzeile kann wegen der Nullspalte 5 nur fünf bestimmte Zellen tragen, der Einser in Zeile 3 setzt einen zweiten Engpass. Die getrennten Querarme oben und unten werden erst über den linken Zweierbereich verbunden; die zunächst plausible direkte Vertikale zwischen N/4–S/4 ist zahlenwidrig.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....│.
01 .┌──┘.
02 ┌┘....
03 │.....
04 └───┐.
```

## S1-02-03-09

- Input: 6×5; A E/4, B N/1. Zeilen `[6, 6, 1, 1, 1]`, Spalten `[2, 2, 2, 2, 2, 5]`.
- Lösung: 15 Zellen, 11 Kurven; Dichte 15/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=63, maxDeductionDepth=8.
- Einstieg: Die oberen beiden Zeilen sind voll und die rechte Spalte ist voll. Belegschritte: `[18, 19, 20, 21, 22]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 46, 47, 52, 53, 59, 60, 61]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'DENSITY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Nordport liegt innen, während der Ostport weit unten sitzt; der dichte obere Kurvenzug endet in einem langen klaren rechten Schenkel. Als bewusste asymmetrische Variation darf die Kurvenlast fast ganz oben liegen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘┌┐┌┐
01 └─┘└┘│
02 .....│
03 .....│
04 .....└
```

## S1-02-03-10

- Input: 6×5; A S/0, B N/4. Zeilen `[2, 1, 1, 5, 6]`, Spalten `[1, 2, 2, 2, 3, 5]`.
- Lösung: 15 Zellen, 12 Kurven; Dichte 15/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=73, maxDeductionDepth=9.
- Einstieg: Die untere Vollzeile gibt einen sicheren Bodenanker. Belegschritte: `[18, 19, 20, 21, 22]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 25, 36, 60, 65, 66, 68, 69, 71]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'DENSITY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der dichte Kurvenzug liegt nun unten, aber der Nordport rechts führt zunächst durch zwei Einserzeilen. Nähe zu Position 9 ist ein tatsächlicher Motivvergleich: ein zusätzlicher Zweierrest oben, fünf statt sechs Belegungen im unteren Nachbarband und andere Endkanten verändern die Schlussfolge. Beide bleiben als räumlicher Dichtevergleich erhalten.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....└┐
01 .....│
02 .....│
03 .┌┐┌┐│
04 ┌┘└┘└┘
```

## S1-02-03-11

- Input: 6×5; A N/5, B W/1. Zeilen `[4, 5, 5, 3, 2]`, Spalten `[4, 5, 5, 1, 2, 2]`.
- Lösung: 19 Zellen, 15 Kurven; Dichte 19/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=80, maxDeductionDepth=12.
- Einstieg: Die vollen Spalten 1 und 2 verbinden mehrere Belegungsbereiche. Belegschritte: `[18, 19, 20, 21, 22]`.
- Eine tatsächliche maximale Prämissenkette: `[22, 28, 32, 50, 57, 67, 68, 69, 72, 73, 75, 77]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Nordport sitzt weit rechts, die Einzerspalte 3 ist ein Querengpass in Zeile 2. Die unteren Zweier-/Dreierzeilen werden dagegen links geklärt; dies schafft voneinander entfernte Folgen in einem breiteren Feld als Position 5.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐..│
01 ┘┌┘.┌┘
02 ┌┘┌─┘.
03 └┐│...
04 .└┘...
```

## S1-02-03-12

- Input: 6×5; A W/4, B E/2. Zeilen `[4, 3, 5, 5, 5]`, Spalten `[5, 5, 3, 5, 1, 3]`.
- Lösung: 22 Zellen, 16 Kurven; Dichte 22/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=81, maxDeductionDepth=16.
- Einstieg: Spalte 0 ist voll und grenzt an den Westanschluss unten. Belegschritte: `[18, 19, 20, 21]`.
- Eine tatsächliche maximale Prämissenkette: `[19, 36, 39, 47, 48, 49, 50, 51, 53, 54, 55, 56, 68, 71, 77, 80]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'DENSITY']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei Fünferzeilen verteilen ihre einzelnen Lücken verschieden; Count 1 in Spalte 4 bestimmt den späten Querübergang nach rechts. Die sehr dichte Formfolge verlangt 16 Kurven und ist ein plausibler anspruchsvoller Abschluss, aber ihr Lesbarkeitsrisiko muss extern gespielt werden.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──┐..
01 └┐.│..
02 ┌┘┌┘.┌
03 └┐└┐.│
04 ─┘.└─┘
```
