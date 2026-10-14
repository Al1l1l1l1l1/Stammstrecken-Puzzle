# Author-Leveldesignreview: S1-03-01 (globale Route 09)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Deutliche Randzahlenanker | S1-03-01-01: 6, S1-03-01-02: 6 |
| 3–5 | Ungünstigere A/B-Lage | S1-03-01-03: 9, S1-03-01-04: 10, S1-03-01-05: 10 |
| 6–8 | Lokale Ketten wirken entfernt | S1-03-01-06: 11, S1-03-01-07: 12, S1-03-01-08: 13 |
| 9–10 | Weniger Extremwerte | S1-03-01-09: 11, S1-03-01-10: 13 |
| 11–12 | Faire 6×6-Höhepunkte | S1-03-01-11: 22, S1-03-01-12: 23 |

**Individuelle Routenbewertung:** 6×6 wird durch zwei sehr klare Randzahlbilder eingeführt; die reduzierte Fläche in 1/2 ist dafür sachlich passend. 3–5 verlagern Ports fern von ihren Vollankern, 6–8 verbinden lokale Einserengpässe mit entfernten Gebieten. 9/10 haben weniger öffentliche Extremwerte, obwohl die aktive Fläche schmaler sein kann. 11/12 entfernen die Vollanker ganz und verlängern die Folge von Restzahlen deutlich (22/23). Das ist eine konkrete Schlusslaststeigerung, die weder allein durch Solverwerte freigegeben noch ohne Spieltest pauschal abgesenkt wird.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-03-01-01

- Input: 6×6; A W/1, B E/4. Zeilen `[0, 1, 4, 3, 5, 0]`, Spalten `[4, 1, 3, 1, 3, 1]`.
- Lösung: 13 Zellen, 6 Kurven; Dichte 13/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=77, maxDeductionDepth=6.
- Einstieg: Die obere und untere Nullzeile begrenzen vier aktive Reihen. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 29, 43, 61, 64, 68]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'COMBINATION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei Einsercounts in den Zwischen- und Randspalten fixieren die Übergänge zwischen den langen Schenkeln. Der übersichtliche W/1–E/4-Zug aktiviert bekannte Randzahlanker im größeren 6×6-Feld, ohne aus zwei leeren Reihen eine künstliche Schwierigkeit abzuleiten.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ......
01 ┐.....
02 │.┌─┐.
03 │.│.│.
04 └─┘.└─
05 ......
```

## S1-03-01-02

- Input: 6×6; A E/2, B N/0. Zeilen `[6, 5, 1, 0, 0, 0]`, Spalten `[2, 2, 1, 2, 2, 3]`.
- Lösung: 12 Zellen, 9 Kurven; Dichte 12/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=69, maxDeductionDepth=6.
- Einstieg: Die volle obere Zeile sichert die Kopfbelegung. Belegschritte: `[20, 21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 50, 54, 55, 66, 67]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei untere Nullzeilen halten die Informationsmenge beim Wiedereinstieg ruhig; die zweite Zeile hat genau eine Lücke. Die Endpunkte N/0–E/2 binden eine kompakte Doppelkurve an einen kurzen rechten Schenkel und unterscheiden dies vom langen Dreiarmzug in 1.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 │┌─┐┌┐
01 └┘.└┘│
02 .....└
03 ......
04 ......
05 ......
```

## S1-03-01-03

- Input: 6×6; A N/4, B W/4. Zeilen `[4, 5, 2, 2, 3, 3]`, Spalten `[6, 6, 4, 1, 2, 0]`.
- Lösung: 19 Zellen, 15 Kurven; Dichte 19/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=89, maxDeductionDepth=9.
- Einstieg: Die volle linke Spalte ist ein sicherer Anker trotz des Nordports rechts. Belegschritte: `[20, 21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 52, 63, 68, 77, 78, 85, 86, 87]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Nordport in Spalte 4 verbraucht dort einen der zwei Counts, während Spalte 3 nur einen Übergang trägt. Die untere Wechselkurve kann erst über die dichte linke Belegung bestimmt werden; die räumlich ungünstige Portlage ist konkret wirksam.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐.│.
01 └┐└─┘.
02 ┌┘....
03 └┐....
04 ┐└┐...
05 └─┘...
```

## S1-03-01-04

- Input: 6×6; A S/0, B N/4. Zeilen `[1, 1, 2, 1, 3, 6]`, Spalten `[1, 1, 1, 2, 5, 4]`.
- Lösung: 14 Zellen, 8 Kurven; Dichte 14/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=78, maxDeductionDepth=10.
- Einstieg: Die untere Vollzeile schafft einen sicheren Queranker fern vom Nordport. Belegschritte: `[25, 26, 27, 28, 29]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 33, 57, 60, 68, 70, 71, 74, 75, 77]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'COMBINATION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei Einsercounts in den linken Spalten begrenzen den Bodenweg, zwei obere Einserzeilen dagegen den rechten Schenkel. Die Entfernung der Startansätze bindet lokale Folgen an einen späteren unteren Rückknick; die ähnliche Maske von S1-03-02-08 liegt dort mit anderem Kopf-/Bodenrest und Seitenausgängen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....│.
01 ....│.
02 ....└┐
03 .....│
04 ...┌┐│
05 ┌──┘└┘
```

## S1-03-01-05

- Input: 6×6; A N/2, B S/4. Zeilen `[2, 2, 2, 4, 4, 6]`, Spalten `[5, 5, 2, 3, 3, 2]`.
- Lösung: 20 Zellen, 16 Kurven; Dichte 20/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=91, maxDeductionDepth=10.
- Einstieg: Die volle Unterzeile verbindet die beiden unteren Belegungsgebiete. Belegschritte: `[20, 21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[19, 39, 52, 57, 58, 63, 64, 84, 87, 89]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die oberen Zweierzeilen bilden links einen Kurvenzug, unten muss ein separater rechter Rücklauf zum Südport passen. Fast volle Spalten links und kleine Counts rechts koppeln die Gebiete über die gesamte Breite; die Mehrlast liegt in der versetzten unteren Form.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌┘...
01 ┌┘....
02 └┐....
03 ┌┘.┌┐.
04 │..│└┐
05 └──┘┌┘
```

## S1-03-01-06

- Input: 6×6; A N/0, B S/3. Zeilen `[3, 4, 5, 1, 3, 1]`, Spalten `[3, 1, 1, 4, 3, 5]`.
- Lösung: 17 Zellen, 8 Kurven; Dichte 17/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=85, maxDeductionDepth=11.
- Einstieg: B an S/3 belegt allein die unterste Zeile. Belegschritte: `[0, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 24, 25, 30, 43, 57, 62, 75, 78, 79, 80]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'EXCLUSION']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die Einserzeile 3 setzt einen weit entfernten rechten Engpass, während die obere linke A-Folge an den Fünfercount in Zeile 2 anschließt. Deren Ausschlüsse wirken über die beiden Einserspalten im Zentrum bis zum unteren Ausgang; ein tatsächlicher entfernter Folgeeffekt.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 │...┌┐
01 │..┌┘│
02 └──┘.│
03 .....│
04 ...┌─┘
05 ...│..
```

## S1-03-01-07

- Input: 6×6; A S/2, B N/3. Zeilen `[3, 1, 1, 6, 2, 2]`, Spalten `[2, 3, 2, 2, 2, 4]`.
- Lösung: 15 Zellen, 8 Kurven; Dichte 15/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=76, maxDeductionDepth=12.
- Einstieg: Die volle Zeile 3 verknüpft das linke und rechte Gebiet. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[16, 26, 35, 36, 53, 56, 57, 58, 59, 70, 73, 74]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Zwei obere Einserzeilen lassen den Nordbogen nur rechts fortlaufen; die unteren Zweierreihen müssen dennoch links zum Südport zurückkehren. Der lokale rechte Schenkel erzwingt deshalb einen rasterweiten Querzug, statt dass die nahe Nord-/Südspalte direkt verbunden werden kann.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...└─┐
01 .....│
02 .....│
03 ┌────┘
04 └┐....
05 .└┐...
```

## S1-03-01-08

- Input: 6×6; A S/5, B W/1. Zeilen `[3, 3, 6, 5, 4, 1]`, Spalten `[4, 5, 5, 3, 1, 4]`.
- Lösung: 22 Zellen, 15 Kurven; Dichte 22/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=86, maxDeductionDepth=13.
- Einstieg: A an S/5 belegt die einzige Zelle der unteren Zeile. Belegschritte: `[6, 19]`.
- Eine tatsächliche maximale Prämissenkette: `[19, 27, 32, 58, 61, 64, 66, 72, 73, 76, 77, 81, 85]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'DENSITY']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Das breite mittlere Vollband koppelt einen kurvenreichen linken Bereich mit dem langen rechten Schenkel. Count 1 in Spalte 4 fixiert genau den Querübergang, wodurch eine lokale Restzahl die entfernten Kurvenformen beeinflusst; klar andere Dichte als 6/7.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐...
01 ┘┌┘...
02 ┌┘┌──┐
03 └┐└┐.│
04 .└─┘.│
05 .....│
```

## S1-03-01-09

- Input: 6×6; A E/1, B S/2. Zeilen `[2, 3, 2, 2, 2, 1]`, Spalten `[0, 0, 4, 5, 2, 1]`.
- Lösung: 12 Zellen, 9 Kurven; Dichte 12/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=80, maxDeductionDepth=11.
- Einstieg: B an S/2 verbraucht den Einsercount der unteren Zeile. Belegschritte: `[7, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 40, 47, 56, 58, 60, 61, 62, 64, 69, 70]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'EXCLUSION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die zwei linken Nullspalten begrenzen die Fläche, jedoch gibt es keinen Vollcount. Der Ostport oben und ein kleines Kopfstück führen in die mittige Wechselkurve; als Variation mit weniger Extremwerten zulässig, die schmale aktive Breite wird ausdrücklich erklärt statt durch eine 70%-Regel ausgeschlossen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...┌┐.
01 ...│└─
02 ..┌┘..
03 ..└┐..
04 ..┌┘..
05 ..│...
```

## S1-03-01-10

- Input: 6×6; A N/1, B S/4. Zeilen `[5, 1, 5, 1, 4, 1]`, Spalten `[0, 4, 3, 3, 4, 3]`.
- Lösung: 17 Zellen, 6 Kurven; Dichte 17/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=81, maxDeductionDepth=13.
- Einstieg: Die einzige Nullspalte links schließt die äußere Kante aus. Belegschritte: `[25, 26, 27, 28, 29]`.
- Eine tatsächliche maximale Prämissenkette: `[18, 23, 38, 39, 49, 53, 55, 57, 59, 62, 63, 71, 77]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'COMBINATION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Fünferreihen sind innerhalb der fünf aktiven Spalten voll, zwischen ihnen liegt jeweils eine Einserzeile. Die zwei langen Umwege unterscheiden sich in ihrer rechten Ausdehnung und münden in S/4; Belegung ist leicht zugänglich, Form und Restzahlen wirken über die gesamte Höhe.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .└───┐
01 .....│
02 .┌───┘
03 .│....
04 .└──┐.
05 ....│.
```

## S1-03-01-11

- Input: 6×6; A N/3, B E/4. Zeilen `[4, 2, 1, 2, 4, 2]`, Spalten `[2, 4, 4, 3, 1, 1]`.
- Lösung: 15 Zellen, 9 Kurven; Dichte 15/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=88, maxDeductionDepth=22.
- Einstieg: B an E/4 verbraucht Count 1 in Spalte 5. Belegschritte: `[3, 13]`.
- Eine tatsächliche maximale Prämissenkette: `[13, 23, 29, 31, 32, 44, 48, 49, 53, 54, 59, 61, 66, 67, 68, 69, 74, 75, 78, 81, 85, 87]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der obere linke Bogen und der untere kleine Rückknick müssen durch die Vierercounts der beiden mittleren Spalten verbunden werden. Nur 15 Zellen bei voller Achsenabdeckung machen die lange serielle Kette sichtbar; Tiefe 22 bedeutet hier nicht 22 unbekannte neue Regeln, menschliche Last bleibt offen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──┘..
01 └┐....
02 .│....
03 .└┐...
04 ..│┌──
05 ..└┘..
```

## S1-03-01-12

- Input: 6×6; A E/0, B W/4. Zeilen `[3, 2, 1, 2, 2, 2]`, Spalten `[2, 3, 3, 2, 1, 1]`.
- Lösung: 12 Zellen, 8 Kurven; Dichte 12/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=86, maxDeductionDepth=23.
- Einstieg: A an E/0 belegt zugleich den Einsercount von Spalte 5. Belegschritte: `[5, 12]`.
- Eine tatsächliche maximale Prämissenkette: `[5, 20, 25, 31, 32, 37, 43, 46, 47, 51, 52, 56, 61, 63, 64, 67, 69, 73, 76, 77, 79, 83, 84]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der obere Arm läuft versetzt bis zum unteren Westport und kehrt dort zunächst nach Süden zurück. Die mittleren Dreier-/Zweiercounts begrenzen den Weg ohne Voll-/Nullanker; der dünne diagonale Abschluss unterscheidet sich durch die untere Endpunktkurve von 11, bleibt ein Spieltestrisiko zur langen Anschlussfolge.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...┌──
01 ..┌┘..
02 ..│...
03 .┌┘...
04 ┐│....
05 └┘....
```
