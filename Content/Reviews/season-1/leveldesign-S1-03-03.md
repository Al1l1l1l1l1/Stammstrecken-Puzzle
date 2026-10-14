# Author-Leveldesignreview: S1-03-03 (globale Route 11)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Vertraute Linien | S1-03-03-01: 6, S1-03-03-02: 6 |
| 3–5 | Teilpfade durch Counts trennen | S1-03-03-03: 9, S1-03-03-04: 10, S1-03-03-05: 10 |
| 6–8 | Belegung, Ausschluss und Geometrie | S1-03-03-06: 12, S1-03-03-07: 12, S1-03-03-08: 13 |
| 9–10 | Entfernte A/B und Kurven | S1-03-03-09: 11, S1-03-03-10: 12 |
| 11–12 | Große 7×6-Abschlüsse | S1-03-03-11: 25, S1-03-03-12: 23 |

**Individuelle Routenbewertung:** 1/2 reaktivieren Querbänder und einen dünnen Randzug, 3–5 trennen unterschiedlich dichte Teilwege durch Einser- und hohe Restcounts. Bei 6 folgt der erlaubte Wechsel auf 7×6 mit einem Vollanker; 7/8 wirken räumlich breit, auch wenn die Lösung lange Arme enthält. 9/10 nutzen engere aktive Gebiete ausdrücklich für entfernte Port-/Kurvenvariation. Die zwei Abschlüsse 13/42 und 28/42 kontrastieren serielle und flächige Last. Die hohe Tiefe 25 in 11 wird fachlich als Schlussfolge beschrieben, nicht als alleiniger Qualitätsbeleg.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-03-03-01

- Input: 6×6; A N/0, B S/5. Zeilen `[6, 1, 4, 1, 4, 1]`, Spalten `[1, 1, 4, 3, 3, 5]`.
- Lösung: 17 Zellen, 6 Kurven; Dichte 17/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=77, maxDeductionDepth=6.
- Einstieg: Die obere Vollzeile gibt einen vertrauten Queranker. Belegschritte: `[20, 21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[19, 29, 49, 50, 64, 72]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'CHAIN']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die zwei unteren langen Bänder beginnen erst in Spalte 2, ihre Verbindungsspalten haben deshalb andere Restzahlen als im älteren 6×6-Serpentinenrückgriff. Als einzelner ruhiger Wiedereinstieg behalten; Position 2 bringt anschließend eine andere Form und deutlich geringere Dichte.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └────┐
01 .....│
02 ..┌──┘
03 ..│...
04 ..└──┐
05 .....│
```

## S1-03-03-02

- Input: 6×6; A N/1, B S/2. Zeilen `[1, 2, 1, 1, 1, 3]`, Spalten `[5, 3, 1, 0, 0, 0]`.
- Lösung: 9 Zellen, 4 Kurven; Dichte 9/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=67, maxDeductionDepth=6.
- Einstieg: A an N/1 belegt die Einserzeile oben allein. Belegschritte: `[1, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 20, 25, 51, 53, 64]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 3 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei Nullspalten rechts und drei mittlere Einserzeilen begrenzen einen langen linken Schenkel. Der kurze obere Versatz und der untere Dreierrückweg reaktivieren bekannte sichere Linien vor dem Größenwechsel; die geringe aktive Breite hat hier einen benannten Entlastungszweck.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .│....
01 ┌┘....
02 │.....
03 │.....
04 │.....
05 └─┐...
```

## S1-03-03-03

- Input: 6×6; A N/3, B S/4. Zeilen `[1, 2, 2, 4, 5, 5]`, Spalten `[0, 2, 3, 6, 5, 3]`.
- Lösung: 19 Zellen, 16 Kurven; Dichte 19/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=86, maxDeductionDepth=9.
- Einstieg: Spalte 3 ist voll und kreuzt den Nordanschluss. Belegschritte: `[30, 31, 32, 33, 34]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 23, 35, 51, 65, 78, 79, 80, 82]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'EXCLUSION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Oben bleiben Zweierreihen schmal, unten verteilen Fünferreihen verschiedene Einzelücken. Die Nullspalte links und der Fünfercount daneben trennen den rechten unteren Teilweg von der oberen Kurvenfolge; plausible Teilwege werden durch dieselben Counts unterschiedlich eingegrenzt.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...│..
01 ...└┐.
02 ...┌┘.
03 ..┌┘┌┐
04 .┌┘┌┘│
05 .└─┘┌┘
```

## S1-03-03-04

- Input: 6×6; A N/2, B E/1. Zeilen `[3, 5, 2, 3, 5, 3]`, Spalten `[6, 5, 4, 1, 4, 1]`.
- Lösung: 21 Zellen, 13 Kurven; Dichte 21/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=86, maxDeductionDepth=10.
- Einstieg: Die volle Spalte 0 sichert den linken Arm. Belegschritte: `[20, 21, 22, 23, 24, 25]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 29, 46, 48, 54, 56, 66, 73, 80, 81]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Zwei Einsercounts in den rechten Zwischenachsen erlauben nur Querübergänge; der Nordport liegt links davon, der Ostport rechts. Die unteren Fünfer-/Dreierreste bestimmen, welcher breite Bogen diese Teilwege verbinden kann; mehr räumliche Trennung als in 3.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐│...
01 │└┘.┌─
02 │...│.
03 └┐..│.
04 ┌┘┌─┘.
05 └─┘...
```

## S1-03-03-05

- Input: 6×6; A W/0, B S/4. Zeilen `[1, 3, 4, 2, 2, 2]`, Spalten `[3, 1, 4, 5, 1, 0]`.
- Lösung: 14 Zellen, 11 Kurven; Dichte 14/36. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=74, maxDeductionDepth=10.
- Einstieg: A an W/0 belegt die Einserzeile oben. Belegschritte: `[0, 18]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 20, 39, 40, 50, 64, 67, 68, 70, 72]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der lange linke Eingang muss erst den oberen kleinen Rechtsbogen erreichen und dann zur Südspalte 4 zurückkehren. Die Einsercounts in Spalten 1/4 und die rechte Nullspalte beschränken die möglichen Übergänge; gute dünne Gegenvariante zu den dichten Teilwegfeldern 3/4.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.....
01 │.┌┐..
02 └─┘│..
03 ..┌┘..
04 ..└┐..
05 ...└┐.
```

## S1-03-03-06

- Input: 7×6; A E/0, B W/4. Zeilen `[7, 6, 2, 2, 3, 3]`, Spalten `[6, 6, 4, 2, 2, 2, 1]`.
- Lösung: 23 Zellen, 20 Kurven; Dichte 23/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=105, maxDeductionDepth=12.
- Einstieg: Die volle obere Zeile gibt beim Wechsel auf 7×6 einen sicheren breiten Anker. Belegschritte: `[22, 23, 24, 25, 26, 27]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 38, 46, 80, 85, 89, 90, 93, 95, 98, 100, 104]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'DENSITY']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die beiden vollen linken Spalten tragen die untere Kurvenserie, die Einzerspalte 6 nur den Ostport oben. Hohe Belegung oben und kleine Zweier-/Dreierzeilen unten müssen über Restzahlen geometrisch verzahnt werden; 20 Kurven sind eine konkrete Lesbarkeitslast.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐┌┐┌─
01 └┐└┘└┘.
02 ┌┘.....
03 └┐.....
04 ┐└┐....
05 └─┘....
```

## S1-03-03-07

- Input: 7×6; A E/2, B S/1. Zeilen `[3, 3, 4, 2, 1, 6]`, Spalten `[0, 1, 1, 4, 3, 5, 5]`.
- Lösung: 19 Zellen, 11 Kurven; Dichte 19/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=100, maxDeductionDepth=12.
- Einstieg: Die linke Nullspalte begrenzt den unteren Queranker. Belegschritte: `[22, 23, 24, 25, 26, 27]`.
- Eine tatsächliche maximale Prämissenkette: `[16, 32, 38, 39, 48, 53, 67, 77, 79, 89, 93, 94]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die Einserzeile 4 liegt am rechten Rand, während der Südport links den Einsercount in Spalte 1 verbraucht. Der obere Kurvenblock wird über den langen rechten Weg an die fast volle untere Zeile angeschlossen; räumlich getrennte Schlussansätze verknüpfen sich.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...┌─┐.
01 ...│.└┐
02 ...└─┐└
03 .....└┐
04 ......│
05 .┌────┘
```

## S1-03-03-08

- Input: 7×6; A W/0, B E/2. Zeilen `[1, 4, 4, 4, 6, 0]`, Spalten `[5, 1, 4, 1, 4, 1, 3]`.
- Lösung: 19 Zellen, 8 Kurven; Dichte 19/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=100, maxDeductionDepth=13.
- Einstieg: A an W/0 belegt die einzige Zelle der oberen Zeile. Belegschritte: `[0, 10]`.
- Eine tatsächliche maximale Prämissenkette: `[15, 38, 56, 66, 69, 70, 71, 85, 86, 87, 88, 90, 92]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei Einserspalten verteilen die Querübergänge zwischen vier senkrechten Armen. Der Einserkopf und die leere Unterzeile machen die Belegung zugänglich, aber die Endpunktlage E/2 verhindert die Symmetrie eines fertigen Serpentinenstreifens; behalten als Kombination über mehrere Achsen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐......
01 │.┌─┐..
02 │.│.│.┌
03 │.│.│.│
04 └─┘.└─┘
05 .......
```

## S1-03-03-09

- Input: 7×6; A W/3, B S/5. Zeilen `[0, 0, 0, 2, 6, 6]`, Spalten `[3, 3, 2, 2, 2, 2, 0]`.
- Lösung: 14 Zellen, 11 Kurven; Dichte 14/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=91, maxDeductionDepth=11.
- Einstieg: Die drei oberen Nullzeilen begrenzen einen unteren Kurvenbereich. Belegschritte: `[22, 23, 24, 25, 26, 27, 28]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 45, 61, 72, 79, 80, 81, 82, 86, 87, 89]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'DENSITY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: W/3 und S/5 liegen weit auseinander; die beiden Sechserreihen sind innerhalb der sechs aktiven Spalten voll, aber ihre Formen bleiben zu klären. Der kurze linke Vorlauf unterscheidet den Bodenstreifen von früheren Vollstreifen; hier ist die gezielte Endpunkt-/Kurvenvariation passend.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .......
01 .......
02 .......
03 ─┐.....
04 ┌┘┌┐┌┐.
05 └─┘└┘│.
```

## S1-03-03-10

- Input: 7×6; A S/3, B N/0. Zeilen `[5, 1, 1, 3, 3, 2]`, Spalten `[1, 1, 4, 4, 5, 0, 0]`.
- Lösung: 15 Zellen, 8 Kurven; Dichte 15/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=96, maxDeductionDepth=12.
- Einstieg: B an N/0 verbraucht Count 1 in Spalte 0. Belegschritte: `[0, 18]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 22, 39, 40, 57, 72, 77, 82, 88, 91, 93, 95]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die zwei rechten Nullspalten und oberen Einserzeilen lenken den langen Eingang erst an den rechten aktiven Rand. Der Südport an Spalte 3 liegt innerhalb eines unteren Doppelbogens; die Portentfernung ist sechs Zellschritte und fordert eine andere Ecke als Position 9.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └───┐..
01 ....│..
02 ....│..
03 ..┌┐│..
04 ..│└┘..
05 ..└┐...
```

## S1-03-03-11

- Input: 7×6; A S/0, B E/1. Zeilen `[2, 3, 1, 1, 5, 1]`, Spalten `[2, 1, 1, 1, 4, 2, 2]`.
- Lösung: 13 Zellen, 7 Kurven; Dichte 13/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=100, maxDeductionDepth=25.
- Einstieg: A an S/0 belegt allein die unterste Zeile. Belegschritte: `[8, 15]`.
- Eine tatsächliche maximale Prämissenkette: `[15, 22, 34, 35, 39, 44, 45, 49, 54, 55, 59, 64, 65, 69, 74, 75, 78, 82, 83, 84, 88, 89, 90, 93, 97]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 9 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei Einserspalten erzwingen einzelne Übergänge zum rechten Kopfstück, dessen Zweier-/Dreierzeilen erst spät die Form festlegen. Nur 13 Zellen durch alle Achsen ergeben eine übersichtliche, aber lange serielle Folge; die hohe Tiefe 25 darf nicht als menschlich bestätigte Meisterschaft gelten.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .....┌┐
01 ....┌┘└
02 ....│..
03 ....│..
04 ┌───┘..
05 │......
```

## S1-03-03-12

- Input: 7×6; A S/2, B W/4. Zeilen `[7, 6, 3, 2, 6, 4]`, Spalten `[3, 5, 2, 4, 3, 5, 6]`.
- Lösung: 28 Zellen, 19 Kurven; Dichte 28/42. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=107, maxDeductionDepth=23.
- Einstieg: Die volle obere Zeile gibt einen klaren ersten Belegungsanker. Belegschritte: `[22, 23, 24, 25, 26, 27, 28]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 29, 39, 55, 56, 73, 76, 80, 83, 84, 85, 87, 90, 93, 94, 96, 97, 98, 100, 101, 103, 104, 106]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'DENSITY']`; erwartete gedankliche Folge 9 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: 28 Zellen verteilen sich über obere breite Bögen und einen unteren rechteckigen Umweg, verbunden durch unterschiedliche Fünfer-/Sechsercounts. Die West-/Südports liegen unterhalb verschiedener Teilgebiete; dieser dichte Abschluss kontrastiert 11 räumlich und trägt bei geringerer technischer Tiefe mehr Formlast.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──┐┌─┐
01 └┐.└┘┌┘
02 .│...└┐
03 .│....│
04 ─┘.┌─┐│
05 ..┌┘.└┘
```
