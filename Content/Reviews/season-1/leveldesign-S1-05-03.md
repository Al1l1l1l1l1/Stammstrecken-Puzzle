# Author-Leveldesignreview: S1-05-03 (globale Route 19)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Bekannte Startmuster | S1-05-03-01: 7, S1-05-03-02: 8 |
| 3–5 | Mehrere lange Ketten | S1-05-03-03: 12, S1-05-03-04: 10, S1-05-03-05: 15 |
| 6–8 | Wenige direkte Extreme | S1-05-03-06: 14, S1-05-03-07: 15, S1-05-03-08: 16 |
| 9–10 | Problematische Kanten und Kurven | S1-05-03-09: 14, S1-05-03-10: 15 |
| 11–12 | Große prüfbare Höhepunkte | S1-05-03-11: 31, S1-05-03-12: 29 |

**Individuelle Routenbewertung:** Der erste Wiedereinstieg wurde gezielt korrigiert, weil sechs Nullspalten dem ausdrücklich nicht vereinfachten Start widersprechen; 2 erhält große Achsenabdeckung als bekanntes Muster. 3–5 verbinden ungleich lange vertikale Arme, einen breiten Bodenanker und unterschiedlich lange Querbänder. 6–8 besitzen jeweils nur einen Extremanker und müssen übrige Restcounts kombinieren; die Nähe anderer Serpentinen ist konkret dokumentiert statt als Neuheit ausgegeben. 9/10 dürfen räumlich reduzieren, um problematische Kanten und Kurven zu variieren. Die Abschlüsse mit gleichen Ports testen unterschiedliche Verteilung der 22 Zellen und bleiben auf menschliche Schlusslast zu prüfen.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-05-03-01

- Input: 9×9; A E/8, B N/0. Zeilen `[9, 4, 2, 1, 1, 1, 1, 2, 2]`, Spalten `[2, 2, 1, 1, 1, 1, 1, 5, 9]`.
- Lösung: 23 Zellen, 11 Kurven; Dichte 23/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=149, maxDeductionDepth=7.
- Einstieg: Die volle obere Zeile und volle Spalte 8 bieten bekannte, getrennte Anfangsanker. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39]`.
- Eine tatsächliche maximale Prämissenkette: `[37, 72, 91, 92, 94, 137, 139]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'COMBINATION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Gezielt ersetzt: alle neun Zeilen und Spalten sind aktiv, keine öffentliche Nullzahl reduziert das Raster auf einen kleinen Teil. Der Nordport links und Ostport rechts unten liegen an zwei verschiedenen Kurvengebieten; Fünferreste in Spalte 7 und Zweierreihen oben/unten verlangen genaue Formfestlegung trotz sicherer Rahmenbelegung. Der Wiedereinstieg behält echte 9×9-Ausdehnung bei kurzer Rootfolge.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 │┌──────┐
01 └┘.....┌┘
02 .......└┐
03 ........│
04 ........│
05 ........│
06 ........│
07 .......┌┘
08 .......└─
```

## S1-05-03-02

- Input: 9×9; A N/1, B S/8. Zeilen `[8, 1, 7, 1, 7, 1, 8, 1, 8]`, Spalten `[0, 4, 6, 5, 5, 5, 5, 5, 7]`.
- Lösung: 42 Zellen, 10 Kurven; Dichte 42/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=177, maxDeductionDepth=8.
- Einstieg: Die linke Nullspalte macht die oberen und unteren Achterbänder innerhalb der aktiven Breite voll. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 49, 74, 89, 119, 128, 143, 149]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die beiden mittleren Siebenerbänder beginnen dagegen erst in Spalte 2; ihre Restzahlen müssen mit den weiter reichenden Außengebieten zusammenpassen. Die hohe Achsenabdeckung und 42 Zellen erhalten die räumliche Last eines bekannten Startmusters, während der nahe spätere Zwilling S1-05-04-02 geändert wird.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .└──────┐
01 ........│
02 ..┌─────┘
03 ..│......
04 ..└─────┐
05 ........│
06 .┌──────┘
07 .│.......
08 .└──────┐
```

## S1-05-03-03

- Input: 9×9; A W/0, B E/8. Zeilen `[1, 7, 5, 5, 5, 5, 6, 4, 1]`, Spalten `[7, 1, 6, 1, 7, 1, 7, 1, 8]`.
- Lösung: 39 Zellen, 10 Kurven; Dichte 39/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=183, maxDeductionDepth=12.
- Einstieg: A an W/0 belegt die Einserzeile oben. Belegschritte: `[0, 31]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 39, 62, 89, 96, 108, 112, 128, 134, 152, 155, 165]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Fünf vertikale Arme beginnen und enden in unterschiedlichen Höhen; vier Einserspalten erlauben nur ihre getrennten Querübergänge. Kein öffentlicher Extremcount macht Belegung fertig; mehrere mittlere Restzahlfolgen öffnen gemeinsam den Weg bis E/8.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐........
01 │.┌─┐.┌─┐
02 │.│.│.│.│
03 │.│.│.│.│
04 │.│.│.│.│
05 │.│.│.│.│
06 └─┘.│.│.│
07 ....└─┘.│
08 ........└
```

## S1-05-03-04

- Input: 9×9; A W/8, B N/7. Zeilen `[1, 1, 2, 1, 1, 1, 2, 4, 9]`, Spalten `[1, 2, 2, 1, 1, 1, 1, 6, 7]`.
- Lösung: 22 Zellen, 11 Kurven; Dichte 22/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=159, maxDeductionDepth=10.
- Einstieg: Die volle Unterzeile verbindet die beiden entfernten Randgebiete. Belegschritte: `[40, 41, 42, 43, 44, 45, 46, 47]`.
- Eine tatsächliche maximale Prämissenkette: `[45, 82, 112, 113, 114, 144, 149, 151, 152, 158]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die oberen Einserzeilen legen den rechten Startarm fest, eine Zweierzeile erzeugt jedoch einen Versatz. Der kleine untere Doppelknick links muss mit den zwei Zweierspalten abgestimmt werden; die sichere Bodenbelegung eröffnet erst zusammen mit dem rechten Restcount die Form.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .......│.
01 .......│.
02 .......└┐
03 ........│
04 ........│
05 ........│
06 .......┌┘
07 .┌┐....└┐
08 ─┘└─────┘
```

## S1-05-03-05

- Input: 9×9; A N/1, B S/8. Zeilen `[6, 1, 4, 1, 4, 1, 4, 1, 6]`, Spalten `[0, 1, 1, 7, 5, 5, 7, 1, 1]`.
- Lösung: 28 Zellen, 10 Kurven; Dichte 28/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=168, maxDeductionDepth=15.
- Einstieg: Die linke Nullspalte begrenzt die Querarme. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[31, 56, 75, 76, 85, 114, 135, 142, 145, 147, 150, 152, 155, 157, 160]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Zwei äußere Sechserbänder und drei innere Viererbänder verteilen fünf Teilgebiete über die ganze Höhe. Einsercounts in den äußeren Spalten machen Übergänge knapp; die direkte N/1–S/8-Verbindung erfüllt die hohen mittleren Counts nicht. Die wechselnde Bandbreite ist der konkrete Unterschied zu 2.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .└────┐..
01 ......│..
02 ...┌──┘..
03 ...│.....
04 ...└──┐..
05 ......│..
06 ...┌──┘..
07 ...│.....
08 ...└────┐
```

## S1-05-03-06

- Input: 9×9; A W/2, B E/7. Zeilen `[3, 2, 3, 6, 5, 5, 7, 1, 0]`, Spalten `[5, 1, 7, 1, 7, 1, 4, 1, 5]`.
- Lösung: 32 Zellen, 10 Kurven; Dichte 32/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=171, maxDeductionDepth=14.
- Einstieg: B an E/7 belegt die Einserzeile 7. Belegschritte: `[11, 22]`.
- Eine tatsächliche maximale Prämissenkette: `[22, 34, 50, 73, 86, 87, 88, 89, 90, 110, 111, 120, 144, 153]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'EXCLUSION']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Ein Nullrand unten ist der einzige Extremcount; obere Dreierreihen und untere Siebener-/Fünferreihen erzeugen verschiedene Armhöhen. Die vier Einserspalten koppeln die entfernten Teilräume, lokal kurze Verbindungen scheitern an ihren Restzahlen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌─┐....
01 ..│.│....
02 ┐.│.│....
03 │.│.│.┌─┐
04 │.│.│.│.│
05 │.│.│.│.│
06 └─┘.└─┘.│
07 ........└
08 .........
```

## S1-05-03-07

- Input: 9×9; A N/2, B S/8. Zeilen `[5, 1, 6, 1, 6, 1, 7, 1, 9]`, Spalten `[3, 5, 5, 5, 5, 5, 7, 1, 1]`.
- Lösung: 37 Zellen, 10 Kurven; Dichte 37/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=178, maxDeductionDepth=15.
- Einstieg: Die volle Unterzeile gibt einen einzigen direkten Belegungsanker. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39]`.
- Eine tatsächliche maximale Prämissenkette: `[39, 47, 64, 100, 108, 122, 136, 148, 150, 151, 152, 153, 154, 162, 166]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Alle mittleren Querbänder sind kürzer und unterschiedlich nach links versetzt; ihre Sechser-/Siebenerwerte verlangen Restzahlen in Spalten 0/1/6. Der Nordport sitzt innen und muss in die breite Bodenverbindung eingepasst werden; Kombination entsteht trotz des einzelnen Vollankers.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└───┐..
01 ......│..
02 .┌────┘..
03 .│.......
04 .└────┐..
05 ......│..
06 ┌─────┘..
07 │........
08 └───────┐
```

## S1-05-03-08

- Input: 9×9; A N/2, B S/7. Zeilen `[6, 1, 8, 1, 8, 1, 5, 1, 5]`, Spalten `[3, 2, 3, 6, 5, 5, 5, 7, 0]`.
- Lösung: 36 Zellen, 10 Kurven; Dichte 36/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=166, maxDeductionDepth=16.
- Einstieg: Die rechte Nullspalte begrenzt zwei volle aktive Achterbänder. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 51, 66, 79, 82, 84, 88, 90, 103, 104, 118, 131, 149, 153, 155, 159]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die unteren Fünferbänder beginnen erst in Spalte 3; oben müssen die seitlichen Dreier-/Zweiercounts erfüllt bleiben. Ein gleichmäßig breiter Serpentinenzug würde diese Reste verletzen; diese ungleiche Bandverteilung erklärt die Kombinationslast und die Nähe zum späteren breiteren Wiedergreifen von S1-05-04-03.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└────┐.
01 .......│.
02 ┌──────┘.
03 │........
04 └──────┐.
05 .......│.
06 ...┌───┘.
07 ...│.....
08 ...└───┐.
```

## S1-05-03-09

- Input: 9×9; A E/5, B N/1. Zeilen `[8, 9, 2, 2, 2, 2, 0, 0, 0]`, Spalten `[2, 2, 2, 2, 2, 2, 1, 6, 6]`.
- Lösung: 25 Zellen, 21 Kurven; Dichte 25/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=157, maxDeductionDepth=14.
- Einstieg: Die volle Zeile 1 trägt den dichten oberen Formenbereich. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 71, 107, 116, 119, 120, 121, 123, 124, 126, 127, 151, 154, 155]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'DENSITY']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der innenliegende Nordport und der Ostport rechts unten verlangen eine lange Wechselkurve mit 21 Kurven. Drei untere Nullzeilen erleichtern Orientierung, aber die oberen Zweiercounts begrenzen mehrere kleine Rückknicke; eine konzentrierte Endpunktvariation statt volle 9×9-Flächenlast.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘┌┐┌┐.┌┐
01 └─┘└┘└─┘│
02 .......┌┘
03 .......└┐
04 .......┌┘
05 .......└─
06 .........
07 .........
08 .........
```

## S1-05-03-10

- Input: 9×9; A S/5, B W/8. Zeilen `[0, 0, 0, 5, 5, 5, 3, 3, 3]`, Spalten `[6, 6, 2, 3, 1, 6, 0, 0, 0]`.
- Lösung: 24 Zellen, 15 Kurven; Dichte 24/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=153, maxDeductionDepth=15.
- Einstieg: Die oberen drei Nullzeilen und rechten drei Nullspalten grenzen ein kurvenreiches unteres Gebiet ein. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 69, 112, 130, 136, 137, 138, 140, 142, 143, 144, 145, 148, 149, 152]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'COMBINATION']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: W/8 und S/5 liegen auf problematisch angrenzenden Kanten; drei Sechsercounts sind innerhalb des aktiven Gebietes voll. Die fünfzähligen Kopfzeilen und Dreierreste unten müssen dennoch die linke Wechselkurve und rechte Gerade unterschiedlich formen; die reduzierte Fläche ist hier eine zulässige Variation.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .........
01 .........
02 .........
03 ┌┐.┌─┐...
04 │└┐│.│...
05 └┐└┘.│...
06 ┌┘...│...
07 └┐...│...
08 ─┘...│...
```

## S1-05-03-11

- Input: 9×9; A E/8, B W/1. Zeilen `[2, 4, 2, 2, 4, 2, 2, 1, 3]`, Spalten `[1, 3, 5, 4, 2, 2, 3, 1, 1]`.
- Lösung: 22 Zellen, 14 Kurven; Dichte 22/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=188, maxDeductionDepth=31.
- Einstieg: B an W/1 verbraucht Count 1 in Spalte 0. Belegschritte: `[9, 31]`.
- Eine tatsächliche maximale Prämissenkette: `[31, 47, 66, 67, 75, 84, 85, 90, 96, 97, 102, 108, 109, 115, 122, 123, 127, 132, 133, 138, 144, 145, 150, 165, 170, 174, 178, 180, 182, 185, 186]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 10 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der obere kleine Bogen führt über eine mittlere Rückkehr in den unteren diagonalen Ausgangsarm. Alle Achsen bleiben aktiv mit kleinen Zahlen; 14 Kurven und mehrere gleiche Zweierwerte müssen unter den verschiedenen Fünfer-/Viererrestcounts auseinandergehalten werden.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌┐.....
01 ──┘│.....
02 ..┌┘.....
03 .┌┘......
04 .└──┐....
05 ....└┐...
06 .....└┐..
07 ......│..
08 ......└──
```

## S1-05-03-12

- Input: 9×9; A E/8, B W/1. Zeilen `[4, 3, 1, 2, 2, 2, 1, 1, 6]`, Spalten `[2, 3, 6, 6, 1, 1, 1, 1, 1]`.
- Lösung: 22 Zellen, 12 Kurven; Dichte 22/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=185, maxDeductionDepth=29.
- Einstieg: A an E/8 verbraucht Count 1 in Spalte 8. Belegschritte: `[9, 31]`.
- Eine tatsächliche maximale Prämissenkette: `[31, 39, 48, 49, 57, 66, 67, 75, 84, 85, 93, 102, 103, 111, 120, 121, 128, 147, 148, 150, 153, 154, 156, 164, 165, 169, 170, 180, 182]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'EXCLUSION']`; erwartete gedankliche Folge 10 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Fünf Einsercounts rechts legen den langen Bodenquerarm fest, die beiden Sechsercounts links die obere verschachtelte Rückkehr. Gleiche Ports und 22 Zellen wie 11 ergeben keine Kopie: der breit verteilte Diagonalarm wird durch eine stark links konzentrierte Formfolge ersetzt, die Root-Tiefe ist sogar geringer.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──┐.....
01 ┘.┌┘.....
02 ..│......
03 .┌┘......
04 .└┐......
05 ..└┐.....
06 ...│.....
07 ...│.....
08 ...└─────
```
