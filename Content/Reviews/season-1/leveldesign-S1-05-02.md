# Author-Leveldesignreview: S1-05-02 (globale Route 18)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Ruhige klare Belegung | S1-05-02-01: 7, S1-05-02-02: 11 |
| 3–5 | Mehrere mittlere Hinweise | S1-05-02-03: 18, S1-05-02-04: 22, S1-05-02-05: 24 |
| 6–8 | Lokale und globale Folgerungen | S1-05-02-06: 14, S1-05-02-07: 15, S1-05-02-08: 16 |
| 9–10 | Hohe Dichte und Geometrie | S1-05-02-09: 14, S1-05-02-10: 14 |
| 11–12 | 9×9-Meisterschaft | S1-05-02-11: 32, S1-05-02-12: 30 |

**Individuelle Routenbewertung:** Der erste Wiedereinstieg bleibt klar begrenzt; der wiederholte Vierarmstart 2 wurde für mehr Eigenständigkeit ersetzt. Die ursprünglichen Null-/Vollanker in 3–5 wurden entfernt. Die neuen Felder starten jedoch jeweils mit einem direkt erschöpften Einsercount am Port; danach müssen mehrere mittlere Restzahlen kombiniert werden. Die in WP-027 / Bible §8.1 bestätigte Lesart erlaubt diese lokalen Einser-Portanker; die notwendige anschließende mittlere Restkoordination erfüllt den präzisierten Lernzweck. Die neuen unterschiedlichen Geometrien haben Root-Tiefen 18/22/24; bei 6 beginnt 9×9 mit klarerem Nullrand und Tiefe 14. Die technische Folge ist nicht monoton; menschlich sind weder der Anspruch von 3–5 noch die anschließende Verdichtung freigegeben. Die Verdichtung verbindet mittlere Restkoordination mit älteren Flächenankern auf größerem Raster. Ob dieser Wechsel als Verdichtung erlebt wird, ist ein konkreter Spieltestpunkt 3→6. 7/8 verknüpfen lange Bänder und getrennte Kopfgebiete. 9/10 sind nahe, aber absichtlich verschiedene Dichte-/Endpunktzugänge und werden begründet behalten. Die langen dünnen Abschlüsse bleiben gesondert auf menschliche Meisterschaft zu prüfen; 9×9 und technische Lösbarkeit allein sind keine Abnahme.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-05-02-01

- Input: 8×8; A E/5, B W/1. Zeilen `[0, 2, 1, 6, 8, 3, 0, 0]`, Spalten `[4, 5, 2, 2, 1, 1, 2, 3]`.
- Lösung: 20 Zellen, 14 Kurven; Dichte 20/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=127, maxDeductionDepth=7.
- Einstieg: Die volle Zeile 4 und drei äußere Nullzeilen geben klare Belegung. Belegschritte: `[36, 37, 38, 39, 40, 41, 42, 43]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 61, 96, 112, 114, 115, 121]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'EXCLUSION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Ein kurzer linker Startarm und eine kurvenreiche Mittelreihe binden den Ostport rechts unten ein. Anders als die reine Langlaufmaske ist die Formlast im klar begrenzten Gebiet verteilt; ruhiger Wiederbeginn trotz 14 Kurven.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ........
01 ─┐......
02 .│......
03 ┌┘┌┐..┌┐
04 │┌┘└──┘│
05 └┘.....└
06 ........
07 ........
```

## S1-05-02-02

- Input: 8×8; A W/1, B E/6. Zeilen `[0, 2, 2, 1, 1, 2, 7, 6]`, Spalten `[7, 5, 2, 2, 1, 2, 1, 1]`.
- Lösung: 21 Zellen, 14 Kurven; Dichte 21/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=139, maxDeductionDepth=11.
- Einstieg: Die obere Nullzeile und zwei mittlere Einserzeilen begrenzen einen klaren linken Schenkel. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 37, 64, 78, 110, 123, 132, 133, 134, 135, 137]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'COMBINATION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Gezielt ersetzt durch 21 Zellen mit zwei unteren verschieden dichten Querreihen 7/6. Links müssen die fast volle Spalte 0 und Fünferspalte 1 zusammenpassen, rechts erlauben Einsercounts einen langen Ausgangsarm. Das aktiviert bekannte Belegung in einer anderen Raumstruktur als der Vierarmzug der Vorroute; die anspruchsvollere Formlast bleibt Spieltestpunkt.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ........
01 ─┐......
02 ┌┘......
03 │.......
04 │.......
05 └┐......
06 ┌┘┌┐.┌──
07 └─┘└─┘..
```

## S1-05-02-03

- Input: 8×8; A E/7, B W/1. Zeilen `[3, 3, 7, 4, 2, 1, 1, 1]`, Spalten `[4, 4, 4, 1, 1, 1, 3, 4]`.
- Lösung: 22 Zellen, 14 Kurven; Dichte 22/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=140, maxDeductionDepth=18.
- Einstieg: A an E/7 belegt die Einserzeile unten allein; die Spaltenreste bestimmen danach den unteren Strang. Belegschritte: `[8, 27]`.
- Eine tatsächliche maximale Prämissenkette: `[27, 34, 42, 43, 50, 58, 59, 66, 74, 75, 79, 84, 85, 93, 99, 119, 123, 124]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Gezielt ersetzt ohne öffentliche Null-/Volllinie. Drei linke Vierercounts müssen zwei Kopfkurven und einen mittleren Querarm verteilen; die drei zentralen Einsercounts erlauben nur einen Übergang zum rechten Dreier-/Vierergebiet. 22 Zellen und 14 Kurven liefern mehrere mittlere Hinweise als Orientierungsquelle statt eines fertigen Linienankers.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐.....
01 ┘┌┘.....
02 ┌┘┌───┐.
03 └─┘...│.
04 ......└┐
05 .......│
06 .......│
07 .......└
```

## S1-05-02-04

- Input: 8×8; A S/3, B N/7. Zeilen `[5, 4, 1, 1, 2, 1, 4, 4]`, Spalten `[6, 5, 3, 4, 1, 1, 1, 1]`.
- Lösung: 22 Zellen, 12 Kurven; Dichte 22/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=150, maxDeductionDepth=22.
- Einstieg: B an N/7 verbraucht Count 1 in Spalte 7 und schließt deren übrige Zellen aus. Belegschritte: `[7, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 28, 35, 43, 44, 51, 59, 60, 67, 75, 76, 83, 91, 113, 126, 128, 132, 133, 135, 137, 138, 143]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'EXCLUSION']`; erwartete gedankliche Folge 9 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Gezielt ersetzt ohne Null-/Volllinie. Vier rechte Einsercounts legen den oberen Querarm fest; links müssen Sechser-/Fünferreste den langen Schenkel und den unteren Doppelknick gemeinsam tragen. Die zwei Viererreihen unten haben verschiedene Formen, die erst aus den mittleren Restzahlen folgen; anderer Zugang als der rechte Strang in 3.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...┌───┘
01 ┌──┘....
02 │.......
03 │.......
04 └┐......
05 .│......
06 ┌┘┌┐....
07 └─┘│....
```

## S1-05-02-05

- Input: 8×8; A W/0, B S/3. Zeilen `[6, 3, 3, 1, 2, 2, 3, 5]`, Spalten `[1, 1, 1, 2, 2, 5, 6, 7]`.
- Lösung: 25 Zellen, 15 Kurven; Dichte 25/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=161, maxDeductionDepth=24.
- Einstieg: A an W/0 verbraucht Count 1 in Spalte 0. Belegschritte: `[0, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 28, 35, 36, 44, 51, 52, 60, 67, 68, 80, 86, 87, 98, 104, 105, 114, 130, 131, 145, 146, 148, 157, 159]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 9 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Gezielt ersetzt ohne Null-/Volllinie. Die beiden benachbarten Einsercounts begrenzen den langen oberen Arm, rechts tragen Fünfer-/Sechser-/Siebenerwerte mehrere verschieden große Knicke. Der Südport innen ist durch einen kleinen unteren Doppelbogen vom rechten Schenkel getrennt; diese Geometrie muss aus mehreren mittleren Zeilenresten bestimmt werden. Der längere neue Einführungsbogen 18/22/24 bleibt konkret extern zu spielen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ─────┐..
01 .....│┌┐
02 .....└┘│
03 .......│
04 ......┌┘
05 ......└┐
06 .....┌┐│
07 ...┌─┘└┘
```

## S1-05-02-06

- Input: 9×9; A W/0, B E/7. Zeilen `[1, 4, 3, 6, 5, 5, 6, 4, 0]`, Spalten `[8, 1, 5, 1, 4, 1, 6, 1, 7]`.
- Lösung: 34 Zellen, 10 Kurven; Dichte 34/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=188, maxDeductionDepth=14.
- Einstieg: A an W/0 belegt die Einserzeile oben allein. Belegschritte: `[0, 22]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 55, 74, 89, 119, 120, 131, 136, 150, 162, 168, 176, 177, 182]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Vier Einserspalten verknüpfen fünf lange Arme in unterschiedlichen Höhen; die untere Nullzeile begrenzt ihre Längen. Beim Wechsel auf 9×9 müssen entfernte Spaltenreste dieselbe lange Strecke beschreiben; lokal passende Kurzverbindungen lassen noch unerfüllte entfernte Counts.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐........
01 │.....┌─┐
02 │.....│.│
03 │.┌─┐.│.│
04 │.│.│.│.│
05 │.│.│.│.│
06 │.│.└─┘.│
07 └─┘.....└
08 .........
```

## S1-05-02-07

- Input: 9×9; A N/0, B S/6. Zeilen `[9, 1, 8, 1, 7, 1, 7, 1, 6]`, Spalten `[1, 7, 5, 5, 5, 5, 5, 5, 3]`.
- Lösung: 41 Zellen, 10 Kurven; Dichte 41/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=182, maxDeductionDepth=15.
- Einstieg: Die volle obere Zeile trägt den Nordport und den ersten breiten Querarm. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 40, 51, 89, 97, 110, 140, 142, 144, 146, 148, 151, 152, 166, 176]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die folgenden Querbänder werden stufenweise kürzer; Einserzeilen legen nur Übergänge, die mittleren Spaltencounts ihre Lage fest. Nähe zum alten S1-05-04-02 ist bekannt, aber jener zweite Wiedereinstieg wird geändert; hier bleibt das breite 9×9-Motiv als längere verteilte Schlussfolge.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └───────┐
01 ........│
02 .┌──────┘
03 .│.......
04 .└─────┐.
05 .......│.
06 .┌─────┘.
07 .│.......
08 .└────┐..
```

## S1-05-02-08

- Input: 9×9; A S/8, B W/1. Zeilen `[5, 8, 1, 2, 2, 3, 1, 1, 1]`, Spalten `[1, 1, 1, 2, 2, 2, 6, 5, 4]`.
- Lösung: 24 Zellen, 15 Kurven; Dichte 24/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=180, maxDeductionDepth=16.
- Einstieg: A an S/8 belegt die Einserzeile unten allein. Belegschritte: `[9, 31]`.
- Eine tatsächliche maximale Prämissenkette: `[31, 39, 62, 63, 70, 90, 91, 97, 120, 121, 134, 146, 151, 152, 176, 178]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'EXCLUSION']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die drei linken Einsercounts bilden die lange Westzufahrt, während mehrere mittlere Counts rechts zwei verschieden große Kurvengebiete eingrenzen. Der obere Kopf und untere Schenkel müssen über die rechte Restzahlfolge zusammenpassen; globaler Zusammenhang wird ohne neue Regel geprüft.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...┌┐┌─┐.
01 ───┘└┘┌┘.
02 ......│..
03 ......└┐.
04 ......┌┘.
05 ......└─┐
06 ........│
07 ........│
08 ........│
```

## S1-05-02-09

- Input: 9×9; A W/0, B E/7. Zeilen `[4, 3, 6, 5, 5, 5, 5, 5, 6]`, Spalten `[9, 1, 9, 1, 9, 1, 7, 1, 6]`.
- Lösung: 44 Zellen, 10 Kurven; Dichte 44/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=185, maxDeductionDepth=14.
- Einstieg: Drei volle Spalten 0/2/4 liefern sichere Belegung im 9×9-Feld. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 59, 66, 74, 75, 76, 105, 106, 121, 141, 147, 154, 157, 177]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die rechte Teilkurve beginnt erst in Zeile 2 und endet am Ostport vor dem Boden. Mit 44 Zellen ist das Feld tatsächlich dicht; die unterschiedliche Höhe der fünf Arme verlangt Spaltenreste statt einer vollständig regelmäßigen Serpentine.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.┌─┐....
01 │.│.│....
02 │.│.│.┌─┐
03 │.│.│.│.│
04 │.│.│.│.│
05 │.│.│.│.│
06 │.│.│.│.│
07 │.│.│.│.└
08 └─┘.└─┘..
```

## S1-05-02-10

- Input: 9×9; A N/1, B S/7. Zeilen `[6, 1, 7, 1, 8, 1, 8, 1, 8]`, Spalten `[6, 5, 5, 5, 5, 5, 6, 4, 0]`.
- Lösung: 41 Zellen, 10 Kurven; Dichte 41/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=187, maxDeductionDepth=14.
- Einstieg: Die rechte Nullspalte begrenzt drei volle aktive Achterbänder. Belegschritte: `[32, 33, 34, 35, 36, 37, 38, 39, 40]`.
- Eine tatsächliche maximale Prämissenkette: `[31, 66, 73, 81, 88, 97, 100, 115, 131, 145, 149, 155, 172, 182]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'COMBINATION']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die beiden oberen kürzeren Bänder müssen aus Sechser-/Siebenercounts und einer verlagerten Rückkehrspalte folgen. Die ähnliche Maskennähe zu 9 wird als bewusst horizontaler/vertikaler Dichtevergleich behalten: ein anderer Nullanker, veränderte Bandbreiten und Nord-/Südports ändern die Zugänge.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .└────┐..
01 ......│..
02 ┌─────┘..
03 │........
04 └──────┐.
05 .......│.
06 ┌──────┘.
07 │........
08 └──────┐.
```

## S1-05-02-11

- Input: 9×9; A S/7, B N/3. Zeilen `[1, 2, 1, 1, 3, 1, 4, 5, 3]`, Spalten `[3, 3, 6, 4, 1, 2, 1, 1, 0]`.
- Lösung: 21 Zellen, 14 Kurven; Dichte 21/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=180, maxDeductionDepth=32.
- Einstieg: B an N/3 belegt die Einserzeile oben. Belegschritte: `[3, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[30, 46, 69, 70, 77, 85, 86, 91, 97, 98, 104, 111, 112, 118, 125, 126, 131, 135, 142, 146, 150, 153, 155, 156, 159, 160, 163, 166, 168, 171, 177, 179]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 10 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die obere und mittlere Einzelstrecke öffnet erst links einen unteren Bogen; der Südport rechts muss über Fünfer-/Viererreste erreicht werden. Eine rechte Nullspalte begrenzt die Breite, beseitigt aber die lange zentrale Kurvenfolge nicht; 32 tiefe Folge bleibt Spieltestfrage.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...│.....
01 ..┌┘.....
02 ..│......
03 ..│......
04 ┌─┘......
05 │........
06 └┐┌┐.....
07 .└┘└─┐...
08 .....└─┐.
```

## S1-05-02-12

- Input: 9×9; A S/8, B W/1. Zeilen `[3, 3, 2, 1, 1, 3, 1, 1, 7]`, Spalten `[6, 4, 6, 1, 1, 1, 1, 1, 1]`.
- Lösung: 22 Zellen, 11 Kurven; Dichte 22/81. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=186, maxDeductionDepth=30.
- Einstieg: A an S/8 verbraucht Count 1 in Spalte 8. Belegschritte: `[9, 31]`.
- Eine tatsächliche maximale Prämissenkette: `[31, 39, 48, 49, 57, 66, 67, 75, 84, 85, 93, 102, 103, 111, 120, 121, 129, 138, 139, 149, 159, 160, 161, 173, 176, 178, 180, 182, 183, 184]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'EXCLUSION']`; erwartete gedankliche Folge 10 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Fünf weitere Einserspalten zwingen einen sehr langen Bodenquerarm, während die drei linken Sechser-/Vierercounts zwei obere Knicke und die mittlere Vertikale bestimmen. Die luftige 22-Zellen-Lösung ist räumlich klar, aber die obere Formfestlegung ergibt einen langen zweiten Meisterschaftsabschluss.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐......
01 ┘┌┘......
02 ┌┘.......
03 │........
04 │........
05 └─┐......
06 ..│......
07 ..│......
08 ..└─────┐
```
