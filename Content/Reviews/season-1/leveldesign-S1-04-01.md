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

**Individuelle Routenbewertung:** Der Wechsel auf 7×7 ist mit klaren Einser-/Nullankern kontrolliert. 3–5 ordnen sichere Querbänder, einen langen Kopfarm und ein dichtes Kopfstück jeweils anders an; 6–8 verbinden konkrete obere, mittlere und untere Randgebiete. 9/10 kontrastieren lange sicher belegte Arme mit kurvenreicher Dichte. 11/12 sind dünne lange Folgen mit verschiedenen Unterbögen. Dass die Produktionsspur kein NO_LOOP/CONNECTIVITY verwendet, widerlegt den Lernbogen nicht automatisch: direkte Abkürzungen scheitern bereits an entfernten Counts. Die globale Verständlichkeit dieser Zusammenhänge bleibt ein Contentreview-/Spieltestpunkt.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-04-01-01

- Input: 7×7; A W/1, B E/0. Zeilen `[1, 5, 4, 4, 6, 0, 0]`, Spalten `[4, 1, 4, 1, 4, 1, 5]`.
- Lösung: 20 Zellen, 8 Kurven; Dichte 20/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=104, maxDeductionDepth=6.
- Einstieg: B an E/0 belegt allein die obere Zeile. Belegschritte: `[6, 7]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 24, 46, 68, 83, 85]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'EXCLUSION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die unteren zwei Nullzeilen und drei Einserspalten machen vier vertikale Arme im neuen 7×7-Feld ruhig lesbar. Der Eintritt links oben liegt unter dem Ausgang; kontrollierter Größenwechsel mit einem vertrauten Querübergangsmotiv.
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
- Einstieg: Fünf obere Nullzeilen begrenzen einen dünnen Bodenbereich. Belegschritte: `[24, 25, 26, 27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 66, 76, 78, 80, 83, 84]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die Spaltencounts 1/2 wechseln, sodass die zwei kleinen Bögen und der mittlere Querarm getrennt werden müssen. Ein solcher kurzer Wiedereinstieg entlastet neben dem breiten Vorgänger; er ist nicht als neue strategische Flächenlast ausgewiesen.
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
- Einstieg: Die beiden äußeren Nullspalten machen die vier Fünferzeilen innerhalb der aktiven Fläche voll. Belegschritte: `[24, 25, 26, 27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 52, 70, 79, 99, 100, 101, 102, 103, 104]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die dazwischenliegenden Einserzeilen setzen die Querbänder getrennt, obwohl beide Endpunkte in Spalte 1 liegen. Eine direkte Verbindung lässt die mittleren Spaltencounts unerfüllt; der bekannte Serpentinenzug dient hier der Verteilung mehrerer sicherer Quergebiete über die gesamte Höhe.
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
- Einstieg: Die beiden linken Spalten sind voll, die beiden rechten leer. Belegschritte: `[24, 25, 26, 27, 28, 29]`.
- Eine tatsächliche maximale Prämissenkette: `[25, 68, 80, 88, 89, 93, 94, 95, 97, 101, 102]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'COMBINATION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der obere Fünferarm muss drei Einsercounts auf entfernten Spalten abdecken, die unteren Zweierzeilen bestimmen dagegen die kleine linke Formenfolge. Zwei sehr verschiedene Teilgebiete werden durch Spaltenreste gekoppelt; bewusst keine Behauptung, dass sämtliche 7×7-Zellen komplex beteiligt seien.
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
- Einstieg: Die volle rechte Spalte bindet den unteren Ostanschluss an das obere Gebiet. Belegschritte: `[30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 24, 47, 60, 72, 73, 97, 106, 107, 108, 109]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Nordport links verbraucht den Einsercount von Spalte 0; zwei fast volle Kopfzeilen und mehrere niedrige Seitenzeilen teilen unterschiedliche Belegungsinseln. Die obere Doppelkurve und zwei getrennte rechte Rückknicke sind räumlich verteilt, statt ein gleichförmiger Vollstreifen zu sein.
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
- Einstieg: B an W/3 verbraucht Count 1 in Spalte 0. Belegschritte: `[11, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[11, 25, 53, 58, 80, 81, 95, 100, 102, 103, 105, 111]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die obere Zweiergruppe, der linke mittige Arm und die untere Rückkurve müssen über die beiden Sechsercounts im Zentrum verbunden werden. Die Einserzeile 5 ist ein später Engpass; drei unterschiedliche Randgebiete sind im Lösungsgitter tatsächlich sichtbar.
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
- Einstieg: Spalte 2 ist voll und trägt den Nordanschluss. Belegschritte: `[42, 43, 44, 45, 46, 47]`.
- Eine tatsächliche maximale Prämissenkette: `[2, 26, 48, 77, 88, 97, 98, 99, 101, 106, 107, 108, 109]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'EXCLUSION']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die beiden linken Nullspalten begrenzen den oberen Kurvenzug, unten führen Zweier-/Dreierspalten zu zwei weiteren Bögen. Der Südport rechts verbraucht einen Einsercount, also kann die lange volle Spalte nicht direkt bis zum Ausgang genügen; mehrere Gebiete werden gekoppelt.
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
- Einstieg: Die volle Unterzeile verknüpft beide Randarme und die mittleren Restgebiete. Belegschritte: `[24, 25, 26, 27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 69, 75, 84, 85, 86, 90, 98, 101, 106, 111, 112, 113, 114]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'DENSITY']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Nordport links und Ostport rechts sitzen weit oberhalb dieses Querankers; Count 1 in Spalte 4 erlaubt nur einen Übergang unten. Der linke Rückknick, mittlere Doppelbogen und rechte Ausgangsarm haben verschiedene Restzahlen und bilden die geforderte Verbindung entfernter Bereiche.
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
- Einstieg: Die vollen Spalten 0/2/4 geben drei sichere lange Belegungsarme. Belegschritte: `[24, 25, 26, 27, 28, 29]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 45, 53, 58, 59, 88, 103, 104, 106, 111]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'COMBINATION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: 29 Zellen füllen die ganze Höhe, ihre Querübergänge dürfen jeweils nur einen der Einsercounts in Spalten 1/3/5 verwenden. Die wiedererkennbare Armstruktur ist als hohe Dichtevariante gut zugänglich; wenig Kurven entlasten trotz vieler Belegungen.
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
- Einstieg: Die obere Nullzeile begrenzt eine dicht belegte Fläche darunter. Belegschritte: `[24, 25, 26, 27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 38, 49, 53, 59, 76, 77, 97, 101, 102, 103, 110, 111]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: 25 Zellen liegen über sechs aktive Reihen, aber 19 Kurven ersetzen die langen geraden Arme des Vorgängers. Der Südanschluss innen und Ostport mittig teilen die untere und obere Geometrie; dies ist ein sinnvoller Dichtevergleich bei deutlich anderer Formlast.
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
- Einstieg: A an E/6 verbraucht den Einsercount von Spalte 6. Belegschritte: `[7, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[23, 29, 36, 37, 42, 48, 49, 54, 60, 61, 65, 70, 71, 75, 80, 81, 84, 88, 89, 92, 96, 97, 99, 102, 103, 106, 108, 112, 113]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'EXCLUSION']`; erwartete gedankliche Folge 10 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Eine kleine Kopfkurve läuft in eine fast diagonale Treppe, die allen Zweiercounts genügen muss. Der dünne Umriss macht die 29 tiefe serielle Rootfolge orientierbar; die Schlusslast ist dennoch eine konkrete Spieltestfrage, keine aus der Tiefe gewonnene Fairnessfreigabe.
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
- Einstieg: A an E/0 belegt allein Count 1 der rechten Randspalte. Belegschritte: `[6, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 24, 30, 37, 38, 44, 51, 55, 56, 61, 62, 67, 73, 76, 84, 88, 92, 95, 104, 107, 109, 110, 111, 113, 115, 117]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 10 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der obere Arm und zwei untere Bögen müssen durch die Fünferspalte 3 sowie die Viererspalten links verbunden werden. Gegenüber 11 verteilen sich mehr Formentscheidungen in der unteren Hälfte, die technische Tiefe ist kleiner; zwei unterschiedliche lange deduktive Abschlüsse.
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
