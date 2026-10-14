# Author-Leveldesignreview: S1-01-04 (globale Route 04)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Einstiegsmuster | S1-01-04-01: 4, S1-01-04-02: 5 |
| 3–5 | Hohe und niedrige Randzahlen | S1-01-04-03: 6, S1-01-04-04: 6, S1-01-04-05: 6 |
| 6–8 | Belegungsbereiche verbinden | S1-01-04-06: 7, S1-01-04-07: 7, S1-01-04-08: 8 |
| 9–10 | Gegenüberliegende Endpunkte | S1-01-04-09: 7, S1-01-04-10: 7 |
| 11–12 | Abschnittskompetenz | S1-01-04-11: 9, S1-01-04-12: 12 |

**Individuelle Routenbewertung:** 1/2 reaktivieren bekannte Ausschlüsse; 3–5 stellen breite Vollflächen einer schmalen Einserfolge gegenüber. In 6/7 sind die zu verbindenden Gebiete über einen Engpass bzw. Queranker verteilt. Position 8 wurde gezielt korrigiert, weil ihre wiederholte Zweispaltenmaske diesen Schwerpunkt unterläuft. 9/10 halten gegenüberliegende Ports bei unterschiedlicher Dichte konstant. 11 gibt einen ruhigen Umriss, 12 eine dichte Kurvenfolge als Abschnittsabruf; diese konkrete Mehrlast ist der Anlass für den noch offenen Spieltest, nicht ein automatisch unzulässiger Tiefenquotient.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-01-04-01

- Input: 4×5; A W/2, B E/4. Zeilen `[0, 0, 1, 3, 4]`, Spalten `[3, 1, 2, 2]`.
- Lösung: 8 Zellen, 6 Kurven; Dichte 8/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=39, maxDeductionDepth=4.
- Einstieg: Die zwei oberen Nullzeilen begrenzen den Einstieg auf drei untere Reihen. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 23, 30, 37]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'OCCUPANCY']`; erwartete gedankliche Folge 3 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der volle Boden und A an W/2 verknüpfen den Einseranschluss mit einem kurzen Rückknick rechts. Gegenüber dem vorherigen diagonalen Finale kehren klare Flächenanker zurück; die reduzierte Höhe dient dem geforderten schnellen Wiedereinstieg.
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
- Einstieg: Spalte 0 und die beiden unteren Reihen sind leer; der Nordanschluss liegt in der verbleibenden Ecke. Belegschritte: `[14, 18, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[9, 28, 34, 36, 38]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 3 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Sieben Kurven in einem 3×3-Bereich verlangen genaue Anschlussformen trotz kleiner Fläche. Das Muster ist horizontal zum Vorgänger verlagert und hat keinen Vollcount; es aktiviert dadurch eine andere bekannte Sicherheit vor den hohen/niedrigen Randzahlen.
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
- Einstieg: Die volle Zeile 0 und der Nordport an Spalte 0 geben sichere Belegung. Belegschritte: `[14, 15, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[15, 25, 32, 35, 40, 41]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'DENSITY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die einzelne Zelle in Zeile 2 zwingt eine schmale Verbindung zwischen zwei dichten Bereichen. Nähe zu S1-01-02-07 ist sachlich vorhanden, aber der Engpass wandert in die Mitte und der Südanschluss fordert den unteren Rückknick; behalten als Abschnittsvergleich hoher und niedriger Zahlen.
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
- Einstieg: Spalte 1 ist voll und die zwei rechten Spalten sind leer. Belegschritte: `[17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[2, 29, 35, 36, 37, 39]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'EXCLUSION']`; erwartete gedankliche Folge 3 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die drei unteren Einserzeilen setzen den langen Schenkel fest, während der Westanschluss oben eine kleine Ausbuchtung verlangt. Der starke Dichtekontrast zu Position 3 zeigt denselben hohen Spaltenanker unter niedrigen Zeilenwerten; daher früher schmaler Wiederholungsraum gerechtfertigt.
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
- Einstieg: Die oberen drei Vollzeilen geben sichere Belegung über die ganze Breite. Belegschritte: `[14, 15, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 25, 38, 44, 45, 46]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Einserengpass in Zeile 3 verhindert das einfache Fortsetzen der vollen Fläche bis unten. Die Endpunkte liegen in derselben Spalte, aber der volle Count 5 sitzt daneben; diese Konstellation verbindet hohe Werte mit einer nicht direkten Endgeometrie.
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
- Einstieg: Die rechte Nullspalte sperrt eine ganze Seite des Rasters. Belegschritte: `[23, 24, 25, 26, 27]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 29, 45, 46, 48, 49, 51]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Zwei volle linke Spalten und wechselnde Zweier-/Dreierzeilen schaffen oben und unten verschiedene Belegungsformen. Der mittlere Zweierengpass vermittelt zwischen ihnen; sinnvoller erster Vergleich getrennter Gebiete im Dreispaltenraum.
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
- Einstieg: Die obere Nullzeile und der Südanschluss links begrenzen zwei Randgebiete. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[19, 27, 32, 33, 37, 40, 46]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Zwei volle Querzeilen verbinden den Eingang links mit der rechten kurvenreichen Hälfte; der Zweiercount in Zeile 1 bestimmt erst den Ausgangsbogen. Es sind deutlich getrennte linke und rechte Verpflichtungen sichtbar, weshalb das Feld den Verbindungsbrief trägt.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....
01 ..┌─
02 ┌┐└┐
03 │└┐│
04 │.└┘
```

## S1-01-04-08

- Input: 4×5; A E/1, B N/2. Zeilen `[1, 3, 4, 3, 2]`, Spalten `[3, 4, 4, 2]`.
- Lösung: 13 Zellen, 11 Kurven; Dichte 13/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=51, maxDeductionDepth=8.
- Einstieg: Die volle Zeile 2 verknüpft den einzelnen Nordanschluss mit dem unteren linken Bereich. Belegschritte: `[17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[2, 15, 26, 29, 37, 42, 43, 46]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Gezielt ersetzt durch 13 Zellen auf allen vier Spalten und fünf Zeilen. Die oberen Einser-/Dreierhinweise und unteren Zweierreihen müssen durch die fast vollen mittleren Spalten 1/2 verbunden werden; die versetzte Treppe unterscheidet obere, mittlere und untere Belegungsgebiete statt eines gleichförmigen Zweispaltenstreifens.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..│.
01 .┌┘┌
02 ┌┘┌┘
03 │┌┘.
04 └┘..
```

## S1-01-04-09

- Input: 4×5; A W/1, B E/3. Zeilen `[4, 4, 2, 2, 0]`, Spalten `[2, 2, 4, 4]`.
- Lösung: 12 Zellen, 10 Kurven; Dichte 12/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=47, maxDeductionDepth=7.
- Einstieg: Die zwei oberen Vollzeilen bilden sichere Belegung vor der unteren Nullzeile. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[15, 27, 35, 41, 42, 43, 44]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'DENSITY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: W/1 und E/3 liegen auf gegenüberliegenden Kanten; der Ausgang sitzt unter dem dichten Kopfbereich. Der rechte doppelte Knick verknüpft diesen Bereich mit der niedrigeren Zeilendichte und verändert den oberen Kurvenzug von S1-01-02-04.
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
- Einstieg: A an E/0 verbraucht den einzigen Count der oberen Zeile. Belegschritte: `[3, 4]`.
- Eine tatsächliche maximale Prämissenkette: `[4, 18, 36, 39, 43, 46, 48]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'CHAIN']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der volle rechte Schenkel ist leicht sichtbar, der Westport sitzt jedoch eine Zeile darunter und zwingt den langen unteren Umweg. Die asymmetrische Zweier-/Dreierfolge unterscheidet die Variante vom dichten Vorgänger trotz gleichartiger gegenüberliegender Kanten.
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
- Einstieg: Die volle Spalte 0 bindet den Westanschluss unten an den oberen Bereich. Belegschritte: `[16, 17, 18, 19]`.
- Eine tatsächliche maximale Prämissenkette: `[10, 15, 25, 27, 29, 34, 41, 46, 50]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Count 1 in Spalte 1 erlaubt nur den Übergang im Kopf, während die beiden Dreierspalten rechts einen versetzten Rückweg erzwingen. Der übersichtliche Umriss ist für den Abschnittsabschluss fair zugänglich; die Formbestimmung ist länger als die bloße Belegung.
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
- Einstieg: Die vollen Zeilen 1 und 2 sichern das Zentrum. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[10, 24, 27, 28, 35, 37, 38, 41, 42, 46, 50, 53]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'DENSITY']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: 14 Kurven verbinden drei dichte Reihen mit dem einzigen besetzten Feld der unteren Zeile. Der mittige Ostanschluss trennt zwei nahe Bögen; dieses Finale verlangt präzises Wechseln zwischen Belegung, Restzahl und Portgeometrie, seine Lesbarkeit bleibt im Spieltest zu prüfen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌─┐
01 ┌┘┌┘
02 └┐└┐
03 ┌┘.└
04 ┘...
```
