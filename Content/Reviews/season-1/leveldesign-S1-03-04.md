# Author-Leveldesignreview: S1-03-04 (globale Route 12)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Hohe und niedrige Counts | S1-03-04-01: 6, S1-03-04-02: 7 |
| 3–5 | Abhängige Ketten | S1-03-04-03: 10, S1-03-04-04: 11, S1-03-04-05: 12 |
| 6–8 | Mittlere Zahlen, wenige Extreme | S1-03-04-06: 13, S1-03-04-07: 13, S1-03-04-08: 14 |
| 9–10 | Schwierige Endpunktkanten | S1-03-04-09: 10, S1-03-04-10: 11 |
| 11–12 | Strategischer Abschnittsabschluss | S1-03-04-11: 22, S1-03-04-12: 24 |

**Individuelle Routenbewertung:** Die extrem klaren Rückgriffe in 1/2 entlasten vor den abhängigen räumlichen Folgen in 3–5. 6–8 besitzen jeweils höchstens einen öffentlichen Extremcount und müssen restliche Übergänge kombinieren; die Armwiederholung in 8 verliert gegenüber ihrer Vorroute den Nullzeilenanker. 9/10 konzentrieren unterschiedliche Formlast an innenliegenden bzw. weit entfernten Seiteneintritten. Die Abschlüsse verbinden einen Vollspaltenanker mit verstreuten Bögen oder eine Einserfolge mit zwei unteren Viererreihen. Ihre Tiefe 22/24 markiert noch offenen Spieltestbedarf, keine automatisch bewiesene strategische Qualität.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-03-04-01

- Input: 7×6; A N/0, B S/6. Zeilen `[7, 1, 5, 1, 5, 1]`, Spalten `[1, 1, 4, 3, 3, 3, 5]`.
- Lösung: 20 Zellen, 6 Kurven; Dichte 20/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=89, maxDeductionDepth=6.
- Einstieg: Die volle obere Zeile steht klar den Einserzeilen 1/3/5 gegenüber. Belegschritte: `[22, 23, 24, 25, 26, 27]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 33, 54, 55, 73, 83]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die unteren Fünferbänder beginnen in Spalte 2, also haben die linken Einsercounts eine andere Rolle als die langen rechten Arme. Als hoher/niedriger Randzahlrückgriff vor dem Abschnittsfinale plausibel; die wiederkehrende Serpentine ist hier eine Wiederholung, keine behauptete neue Idee.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └─────┐
01 ......│
02 ..┌───┘
03 ..│....
04 ..└───┐
05 ......│
```

## S1-03-04-02

- Input: 7×6; A S/0, B W/2. Zeilen `[0, 2, 2, 2, 2, 2]`, Spalten `[5, 5, 0, 0, 0, 0, 0]`.
- Lösung: 10 Zellen, 9 Kurven; Dichte 10/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=73, maxDeductionDepth=7.
- Einstieg: Die fünf rechten Nullspalten begrenzen den Wiedereinstieg auf zwei Spalten. Belegschritte: `[24, 37, 38, 39, 40, 41]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 29, 64, 65, 67, 69, 71]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Beide aktiven Spalten haben Count 5, obwohl das Feld sechs Zeilen hoch ist; die obere Nullzeile liefert die fehlende Stelle. Die vielen kleinen Kurven verbinden W/2 und S/0 statt zweier äußerer Enden; dieser enge Einstieg wird nur hier als ruhige Reaktivierung behalten.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .......
01 ┌┐.....
02 ┘│.....
03 ┌┘.....
04 └┐.....
05 ┌┘.....
```

## S1-03-04-03

- Input: 7×6; A W/2, B N/3. Zeilen `[1, 3, 4, 2, 3, 7]`, Spalten `[4, 1, 1, 4, 3, 5, 2]`.
- Lösung: 20 Zellen, 9 Kurven; Dichte 20/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=89, maxDeductionDepth=10.
- Einstieg: Die volle Unterzeile verknüpft die links und rechts liegenden Randgebiete. Belegschritte: `[28, 29, 30, 31, 32, 33, 34]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 22, 48, 53, 55, 64, 69, 84, 85, 86]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: B an N/3 belegt die Einserzeile oben, die mittigen Einserspalten beschränken den Querübergang nach links. Der obere kleine Rückknick muss sich an die lange äußere Verbindung anschließen; sichere Folgen beginnen an beiden Enden und treffen im Bodenanker aufeinander.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...│...
01 ...│┌┐.
02 ┐..└┘│.
03 │....│.
04 │....└┐
05 └─────┘
```

## S1-03-04-04

- Input: 7×6; A E/5, B N/2. Zeilen `[2, 6, 1, 2, 2, 1]`, Spalten `[0, 2, 2, 1, 1, 3, 5]`.
- Lösung: 14 Zellen, 9 Kurven; Dichte 14/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=93, maxDeductionDepth=11.
- Einstieg: Die linke Nullspalte macht die Sechserzeile 1 innerhalb der aktiven Breite voll. Belegschritte: `[28, 29, 30, 31, 32]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 24, 38, 71, 74, 75, 76, 78, 81, 86, 91]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'EXCLUSION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Nordport links oben und der Ostport rechts unten werden über diesen Querarm verbunden; die zwei rechten Zweierreihen müssen aus den Spaltenresten folgen. Die anfängliche Querbelegung ermöglicht die nachgeordnete Kurvenfolge, sodass eine konkrete Abhängigkeit der Teilketten besteht.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌┘....
01 .└────┐
02 ......│
03 .....┌┘
04 .....└┐
05 ......└
```

## S1-03-04-05

- Input: 7×6; A N/3, B E/1. Zeilen `[1, 4, 3, 3, 4, 6]`, Spalten `[0, 5, 5, 4, 1, 1, 5]`.
- Lösung: 21 Zellen, 13 Kurven; Dichte 21/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=97, maxDeductionDepth=12.
- Einstieg: A an N/3 belegt allein die obere Zeile. Belegschritte: `[3, 8]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 24, 47, 50, 62, 75, 86, 87, 91, 92, 93, 95]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die fast volle Unterzeile verbindet eine linke Wechselkurve mit dem rechten senkrechten Ausgang; zwei mittige Einsercounts erlauben nur diesen weiten Querweg. Die rechte Formfolge hängt an Restzahlen, die links erzeugt werden, statt an einem weiteren direkten Vollanker.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...│...
01 .┌─┘..┌
02 .└┐...│
03 .┌┘...│
04 .│┌┐..│
05 .└┘└──┘
```

## S1-03-04-06

- Input: 7×6; A N/0, B S/5. Zeilen `[4, 2, 1, 1, 2, 3]`, Spalten `[1, 1, 1, 4, 5, 1, 0]`.
- Lösung: 13 Zellen, 8 Kurven; Dichte 13/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=94, maxDeductionDepth=13.
- Einstieg: A an N/0 verbraucht den Einsercount in Spalte 0. Belegschritte: `[0, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 22, 38, 39, 52, 57, 58, 64, 69, 70, 76, 85, 92]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'EXCLUSION']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei weitere Einsercounts beschränken den oberen Querarm und den Südanschluss rechts; dazwischen liegen nur mittlere Werte. Die zwei mittleren Einserzeilen lassen einen schmalen Strang übrig, der untere kleine Rückknick verlangt aber weiterhin die Kombination der Spalten 3/4.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └──┐...
01 ...└┐..
02 ....│..
03 ....│..
04 ...┌┘..
05 ...└─┐.
```

## S1-03-04-07

- Input: 7×6; A W/1, B E/5. Zeilen `[3, 2, 1, 2, 4, 5]`, Spalten `[2, 1, 6, 3, 1, 2, 2]`.
- Lösung: 17 Zellen, 12 Kurven; Dichte 17/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=93, maxDeductionDepth=13.
- Einstieg: Spalte 2 ist der einzige Vollanker und verbindet Kopf und Boden. Belegschritte: `[22, 23, 24, 25, 26, 27]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 28, 44, 54, 55, 56, 57, 66, 74, 82, 89, 90, 91]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Westport links wird durch Count 1 in Spalte 1 zum Querübergang begrenzt; unten müssen zwei getrennte Rückknicke den Ostport aufnehmen. Der Wechsel von langen Linien zu kleinen unteren Kurven passt zum Brief geringer direkter Anker.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐....
01 ┘.│....
02 ..│....
03 ..└┐...
04 ..┌┘.┌┐
05 ..└──┘└
```

## S1-03-04-08

- Input: 7×6; A W/0, B E/2. Zeilen `[1, 4, 4, 4, 4, 6]`, Spalten `[6, 1, 5, 1, 5, 1, 4]`.
- Lösung: 23 Zellen, 8 Kurven; Dichte 23/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=100, maxDeductionDepth=14.
- Einstieg: Die volle Spalte 0 sichert den linken Arm am Westanschluss. Belegschritte: `[28, 29, 30, 31, 32]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 23, 34, 54, 61, 62, 78, 79, 80, 81, 83, 90, 94, 99]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei Einserspalten koppeln vier vertikale Teilräume; weder der obere Querbereich noch der Boden ist öffentlich voll. Der fast gleiche Armschnitt von S1-03-03-08 wird hier um eine zusätzliche aktive Reihe erweitert, die Nullanker fallen weg; sinnvoller Rückgriff mit anderer Informationsgrundlage.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐......
01 │.┌─┐..
02 │.│.│.┌
03 │.│.│.│
04 │.│.│.│
05 └─┘.└─┘
```

## S1-03-04-09

- Input: 7×6; A E/1, B N/4. Zeilen `[7, 7, 3, 3, 0, 0]`, Spalten `[2, 4, 3, 4, 3, 2, 2]`.
- Lösung: 20 Zellen, 17 Kurven; Dichte 20/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=94, maxDeductionDepth=10.
- Einstieg: Die oberen beiden Vollzeilen legen Belegung fest, die unteren zwei Nullzeilen begrenzen den Raum. Belegschritte: `[22, 23, 24, 25, 26, 27]`.
- Eine tatsächliche maximale Prämissenkette: `[22, 49, 66, 67, 76, 79, 84, 85, 90, 91]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'DENSITY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: N/4 sitzt innen im dichten Kopfstück, E/1 am rechten Rand. Der untere Dreierrückbogen ist nicht früh als Gerade erkennbar; das Feld kontrastiert die langen Arme von 8 mit konzentrierter Formlast an schwierigen Anschlussstellen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐┌┘┌┐
01 └┐└┘┌┘└
02 .│.┌┘..
03 .└─┘...
04 .......
05 .......
```

## S1-03-04-10

- Input: 7×6; A E/5, B W/3. Zeilen `[0, 0, 4, 7, 1, 1]`, Spalten `[1, 1, 2, 2, 2, 2, 3]`.
- Lösung: 13 Zellen, 10 Kurven; Dichte 13/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=87, maxDeductionDepth=11.
- Einstieg: Die volle Zeile 3 erreicht beide Seiteneintrittsgebiete. Belegschritte: `[36, 37, 38, 39, 40, 41]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 47, 64, 65, 66, 70, 77, 78, 83, 84, 85]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: W/3 belegt den Einsercount von Spalte 0; der Ostport unten verlängert dagegen den Dreiercount von Spalte 6. Die oberen Zweierknicke müssen in den niedrigen mittigen Spaltenresten verteilt werden; der andere Leerrand und die asymmetrischen Ports rechtfertigen die dünne Variation.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .......
01 .......
02 ..┌┐┌┐.
03 ──┘└┘└┐
04 ......│
05 ......└
```

## S1-03-04-11

- Input: 7×6; A S/6, B N/4. Zeilen `[1, 3, 4, 4, 5, 3]`, Spalten `[0, 3, 3, 3, 6, 4, 1]`.
- Lösung: 20 Zellen, 14 Kurven; Dichte 20/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=102, maxDeductionDepth=22.
- Einstieg: Die volle Spalte 4 und der Nordport darin liefern sichere Belegung. Belegschritte: `[33, 34, 35, 36, 37]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 41, 55, 56, 63, 67, 68, 71, 73, 75, 77, 78, 83, 85, 86, 87, 90, 91, 95, 96, 97, 100]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 9 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die vier mittleren Reihen verteilen Zweier-/Dreierbögen verschieden; der Südport ganz rechts verbraucht den Einsercount der Randspalte. Die direkte N/4–S/6-Verbindung widerspricht den linken Dreiercounts; das Umlenken durch zwei getrennte Gebiete ist die konkrete strategische Schlusslast.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....│..
01 ..┌─┘..
02 .┌┘.┌┐.
03 .│.┌┘│.
04 .└─┘┌┘.
05 ....└─┐
```

## S1-03-04-12

- Input: 7×6; A W/0, B E/3. Zeilen `[3, 1, 1, 4, 4, 3]`, Spalten `[1, 1, 4, 2, 2, 3, 3]`.
- Lösung: 16 Zellen, 10 Kurven; Dichte 16/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=105, maxDeductionDepth=24.
- Einstieg: A an W/0 verbraucht Count 1 in Spalte 0. Belegschritte: `[0, 12]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 22, 27, 28, 34, 39, 40, 46, 50, 51, 56, 60, 61, 66, 75, 76, 83, 85, 86, 89, 95, 96, 100, 104]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 9 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die langen Einserzeilen oben legen einen linken Schenkel fest, unten sind zwei Viererreihen mit dem Ostport zu kombinieren. Der untere Doppelbogen wechselt mehrfach zwischen Spaltenresten; das ist ein anderer Abschlusstyp als das vertikale Vollankerfeld 11 trotz ähnlicher technischer Tiefe.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ──┐....
01 ..│....
02 ..│....
03 ..└┐.┌─
04 ...└┐└┐
05 ....└─┘
```
