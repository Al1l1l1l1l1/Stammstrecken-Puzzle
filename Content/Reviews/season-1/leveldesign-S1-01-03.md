# Author-Leveldesignreview: S1-01-03 (globale Route 03)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Randzahlenanker | S1-01-03-01: 4, S1-01-03-02: 5 |
| 3–5 | Angrenzende Endpunktkanten | S1-01-03-03: 6, S1-01-03-04: 6, S1-01-03-05: 6 |
| 6–8 | Belegung und Geometrie | S1-01-03-06: 7, S1-01-03-07: 7, S1-01-03-08: 7 |
| 9–10 | Dichtekontrast | S1-01-03-09: 7, S1-01-03-10: 7 |
| 11–12 | Sichere Ecken und Kette | S1-01-03-11: 9, S1-01-03-12: 12 |

**Individuelle Routenbewertung:** Die Wiedereinstiege kontrastieren vertikale Einserzeilen und einen dichten Kopfbereich. 3–5 halten angrenzende Kanten konstant und verändern die Kurvenlast. 6–8 verbinden breit belegte Flächen und enge Einzelgänge; 9/10 drehen ausdrücklich die räumliche Dichteverteilung bei gleicher Pfadlänge um. Die Abschlüsse vergleichen gleich hohe Spaltencounts mit einem fast diagonalen Feld ohne Extremwerte. Der Tiefensprung von 7 auf 12 ist vor allem eine längere seriell aufgebaute Formfolge; die kleine sichtbare Lösung vereinfacht Orientierung, beweist jedoch keinen Spieltest-PASS.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-01-03-01

- Input: 4×5; A N/2, B S/3. Zeilen `[1, 1, 1, 2, 3]`, Spalten `[0, 2, 5, 1]`.
- Lösung: 8 Zellen, 4 Kurven; Dichte 8/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=47, maxDeductionDepth=4.
- Einstieg: Die volle Spalte 2 und der Nordanschluss geben einen bekannten vertikalen Anker. Belegschritte: `[21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[2, 15, 28, 30]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY']`; erwartete gedankliche Folge 3 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei Einserzeilen oben sind sofort begrenzt; erst die unteren Zweier-/Dreierzeilen müssen um den Südport herum aufgelöst werden. Die große leere Fläche links dient einem ruhigen Rückgriff nach dem dichten vorherigen Abschluss, nicht einer neuen Denkbelastung.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..│.
01 ..│.
02 ..│.
03 .┌┘.
04 .└─┐
```

## S1-01-03-02

- Input: 4×5; A N/1, B E/2. Zeilen `[4, 4, 3, 0, 0]`, Spalten `[3, 3, 2, 3]`.
- Lösung: 11 Zellen, 9 Kurven; Dichte 11/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=45, maxDeductionDepth=5.
- Einstieg: Die zwei oberen Vollzeilen geben sichere Belegung, die beiden unteren Nullzeilen begrenzen die Fläche. Belegschritte: `[14, 15, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[8, 29, 41, 42, 43]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'DENSITY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Neun Kurven verteilen sich auf einen dichten Kopfbereich mit einem einzelnen freien Feld in Zeile 2. Der Nordport liegt innen statt an der Ecke; gegenüber Position 1 kommt die Formlast zurück, ohne mehr aktive Höhe zu verlangen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘┌┐
01 │┌┘│
02 └┘.└
03 ....
04 ....
```

## S1-01-03-03

- Input: 4×5; A N/0, B W/3. Zeilen `[2, 2, 2, 2, 2]`, Spalten `[5, 5, 0, 0]`.
- Lösung: 10 Zellen, 9 Kurven; Dichte 10/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=43, maxDeductionDepth=6.
- Einstieg: Die beiden rechten Nullspalten begrenzen das Feld; N/0 und W/3 öffnen den verbleibenden Korridor an verschiedenen Kanten. Belegschritte: `[22, 23, 24, 25, 26]`.
- Eine tatsächliche maximale Prämissenkette: `[2, 32, 38, 39, 40, 42]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die vollständig belegten linken Spalten lassen jede Formentscheidung durch Ports weiterlaufen. Für die erste explizite Kurvenlektion ist das schmale Feld sinnvoll; die identische Belegungsmaske des späteren S1-01-04-08 ist dort dagegen keine passende Verdichtung und wurde ersetzt.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └┐..
01 ┌┘..
02 └┐..
03 ┐│..
04 └┘..
```

## S1-01-03-04

- Input: 4×5; A E/1, B S/0. Zeilen `[0, 1, 4, 4, 2]`, Spalten `[3, 3, 2, 3]`.
- Lösung: 11 Zellen, 9 Kurven; Dichte 11/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=50, maxDeductionDepth=6.
- Einstieg: A an E/1 verbraucht den Einsercount dieser Zeile, wodurch ihr übriger Teil leer ist. Belegschritte: `[5, 10]`.
- Eine tatsächliche maximale Prämissenkette: `[5, 19, 29, 44, 46, 47]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'COMBINATION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Ausgang an S/0 liegt jenseits zweier voller Querzeilen. Der Einzelanschluss oben und die untere Zweierreihe zwingen sichere Ecken an beiden Seiten; damit erweitert dieses Feld die angrenzende Kantensituation von Position 3 in zwei Achsen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....
01 ...┌
02 ┌─┐│
03 └┐└┘
04 ┌┘..
```

## S1-01-03-05

- Input: 4×5; A W/1, B S/2. Zeilen `[2, 4, 1, 1, 2]`, Spalten `[1, 1, 3, 5]`.
- Lösung: 10 Zellen, 5 Kurven; Dichte 10/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=44, maxDeductionDepth=6.
- Einstieg: Die volle Zeile 1 trifft den Westanschluss; ihre vier Belegungen sind sofort sicher. Belegschritte: `[14, 15, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 21, 29, 32, 33, 35]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Spalte 3 ist voll, während zwei mittlere Einserzeilen den langen rechten Schenkel festlegen. Der Nordbogen muss über die Dreierspalte 2 zurückkehren; fünf statt neun Kurven entlasten bewusst vor der anschließenden Verdichtung.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌┐
01 ──┘│
02 ...│
03 ...│
04 ..┌┘
```

## S1-01-03-06

- Input: 4×5; A E/2, B W/3. Zeilen `[4, 3, 3, 4, 3]`, Spalten `[3, 5, 4, 5]`.
- Lösung: 17 Zellen, 12 Kurven; Dichte 17/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=55, maxDeductionDepth=7.
- Einstieg: Die obere volle Zeile liefert einen breiten sicheren Start. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 28, 32, 36, 48, 49, 50]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'DENSITY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Nur drei Zellen sind leer, aber die Ports E/2 und W/3 liegen mitten in der dicht belegten Fläche. Zwei volle Spalten koppeln oberen und unteren Bogen; die Formlage an der mittleren Einbuchtung unterscheidet dies vom kompakten Kopfstück in Position 2.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──┐
01 └┐.│
02 .└┐└
03 ─┐└┐
04 .└─┘
```

## S1-01-03-07

- Input: 4×5; A S/2, B E/0. Zeilen `[4, 3, 1, 3, 1]`, Spalten `[4, 2, 4, 2]`.
- Lösung: 12 Zellen, 7 Kurven; Dichte 12/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=48, maxDeductionDepth=7.
- Einstieg: Der Südanschluss in Spalte 2 belegt die einzige Zelle von Zeile 4. Belegschritte: `[3, 12]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 17, 22, 26, 35, 38, 39]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die volle obere Zeile ist ein zweiter, weit entfernter Anker; dazwischen verlangt Zeile 2 eine einzelne Zelle links. So entsteht ein Umweg über den Rand, bevor die beiden rechten Endpunktgebiete geometrisch zusammenfinden.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐┌
01 │.└┘
02 │...
03 └─┐.
04 ..│.
```

## S1-01-03-08

- Input: 4×5; A E/0, B S/1. Zeilen `[3, 2, 2, 1, 1]`, Spalten `[0, 5, 3, 1]`.
- Lösung: 9 Zellen, 5 Kurven; Dichte 9/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=48, maxDeductionDepth=7.
- Einstieg: Spalte 1 ist vollständig belegt, Spalte 0 leer. Belegschritte: `[21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 25, 30, 38, 39, 44, 45]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'EXCLUSION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die unteren Einserzeilen sichern einen langen Schenkel, oben muss ein kurzer Rückknick aus den Counts 3/2/2 folgen. Der Wechsel zur schmalen, luftigen Struktur gibt einen kleinen Zwischenabschluss; die Tiefe 7 stammt hier aus der Anschlussfolge und entspricht keiner dichten Flächenlast.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌──
01 .└┐.
02 .┌┘.
03 .│..
04 .│..
```

## S1-01-03-09

- Input: 4×5; A S/1, B N/3. Zeilen `[2, 2, 1, 4, 4]`, Spalten `[2, 2, 4, 5]`.
- Lösung: 13 Zellen, 10 Kurven; Dichte 13/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=49, maxDeductionDepth=7.
- Einstieg: Die beiden unteren vollen Zeilen sichern ein dichtes unteres Gebiet. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 23, 32, 33, 38, 39, 41]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'COMBINATION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Einserengpass in Zeile 2 trennt den unteren Bereich von den oberen Zweierreihen. Dichte wechselt dadurch tatsächlich innerhalb des Feldes; der Südanschluss liegt weit links vom Nordanschluss und verhindert einen rein vertikalen Weg.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌┘
01 ..└┐
02 ...│
03 ┌─┐│
04 └┐└┘
```

## S1-01-03-10

- Input: 4×5; A W/2, B S/2. Zeilen `[3, 4, 3, 2, 1]`, Spalten `[2, 4, 5, 2]`.
- Lösung: 13 Zellen, 11 Kurven; Dichte 13/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=51, maxDeductionDepth=7.
- Einstieg: B an S/2 belegt allein die unterste Zeile. Belegschritte: `[6, 12]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 19, 25, 38, 42, 44, 49]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der volle Anker liegt nun in Zeile 1, darunter nimmt die Belegung schrittweise ab. Obwohl beide Varianten 13 Zellen haben, liegen der dichte Teil und die Eintrittsrichtung anders als in Position 9; der Dichtebrief ist durch diese räumliche Umkehr nachvollziehbar.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌─┐
01 ┌┘┌┘
02 ┘┌┘.
03 .└┐.
04 ..│.
```

## S1-01-03-11

- Input: 4×5; A E/4, B N/2. Zeilen `[2, 4, 4, 2, 4]`, Spalten `[4, 4, 4, 4]`.
- Lösung: 16 Zellen, 11 Kurven; Dichte 16/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=56, maxDeductionDepth=9.
- Einstieg: Die volle Zeile 1 liefert vier sichere Belegungen. Belegschritte: `[14, 15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[2, 25, 31, 32, 33, 39, 50, 52, 53]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Alle Spalten haben Count 4, aber die Zeilen verteilen diese Belegung ungleich in drei Vollanker und zwei Zweierengpässe. Der Wechsel zwischen den Engpässen erzwingt mehrere getrennte Ecken; dies übt Zählen über Kreuz statt nur das Wiedererkennen einer Randlinie.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└┐
01 ┌─┐│
02 └┐└┘
03 ┌┘..
04 └───
```

## S1-01-03-12

- Input: 4×5; A S/3, B N/1. Zeilen `[2, 1, 2, 2, 2]`, Spalten `[3, 3, 2, 1]`.
- Lösung: 9 Zellen, 8 Kurven; Dichte 9/20. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=49, maxDeductionDepth=12.
- Einstieg: B an N/1 und Count 1 in Spalte 3 bzw. Count 1 in Zeile 1 begrenzen die beiden Endgebiete. Belegschritte: `[1, 13]`.
- Eine tatsächliche maximale Prämissenkette: `[13, 17, 22, 23, 25, 28, 29, 32, 36, 37, 41, 42]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Kein öffentlicher Extremwert liefert eine ganze Linie, die fast diagonale kurze Treppe muss aus Endpunkten und Restzahlen aufgebaut werden. Der dünne Weg ist optisch übersichtlich, hat aber eine lange technische Anschlusskette; passend als anderer Höhepunkt neben dem dichten Abschluss 11, menschliche Tiefe offen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘..
01 │...
02 └┐..
03 .└┐.
04 ..└┐
```
