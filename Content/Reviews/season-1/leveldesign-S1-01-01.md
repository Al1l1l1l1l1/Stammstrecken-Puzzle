# Author-Leveldesignreview: S1-01-01 (globale Route 01)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | A/B und Randzahlen | S1-01-01-01: 4, S1-01-01-02: 4 |
| 3–5 | Nullen und Ausschluss | S1-01-01-03: 5, S1-01-01-04: 5, S1-01-01-05: 5 |
| 6–8 | Volle Linien | S1-01-01-06: 6, S1-01-01-07: 6, S1-01-01-08: 6 |
| 9–10 | Verschiedene A/B-Kanten | S1-01-01-09: 6, S1-01-01-10: 6 |
| 11–12 | Einstiegsmuster kombinieren | S1-01-01-11: 7, S1-01-01-12: 7 |

**Individuelle Routenbewertung:** Die ersten zwei Felder aktivieren Ports und volle Linien, 3–5 isolieren anschließend Nullen, 6–8 kontrastieren sichere Belegung mit offener Form. 9–10 zeigen ausdrücklich gegenüberliegende und versetzte Kanten; angrenzende Kanten waren bereits vorher vorhanden. Das Wort „erste“ in der Bible ist daher ein konkreter Auslegungspunkt, keine behauptete Erfüllung eines Verbots früherer verschiedener Seiten. Eine Eigentümerentscheidung bleibt dafür offen. Die zwei gezielt korrigierten Abschlüsse enthalten jetzt jeweils Null- und Volllinien; ihre tatsächlichen frischen Ketten werden neu nachgewiesen. Position 8 ist als kurze Entlastung sinnvoll. Die frühe Kurvendichte in 1/3 und die Abschlusslesbarkeit bleiben Spieltestfragen.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-01-01-01

- Input: 4×4; A E/2, B N/0. Zeilen `[4, 3, 1, 0]`, Spalten `[2, 2, 1, 3]`.
- Lösung: 8 Zellen, 5 Kurven; Dichte 8/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=33, maxDeductionDepth=4.
- Einstieg: Die volle Zeile 0 gibt vier Belegungen; am Nordanschluss bleibt der Außenport fest. Belegschritte: `[12, 13, 14]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 15, 22, 31]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 3 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der kleine obere Bereich macht den Übergang von Belegung zu Form sichtbar: in Zeile 2 verbraucht A bereits die einzige Zelle, also wird ihr Rest leer. Fünf Kurven auf acht Zellen sind für den ersten Kontakt dicht; behalten wegen der zwei klaren unabhängigen Anker, im Spieltest besonders die Lesbarkeit prüfen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 │┌─┐
01 └┘.│
02 ...└
03 ....
```

## S1-01-01-02

- Input: 4×4; A E/2, B W/2. Zeilen `[0, 0, 4, 2]`, Spalten `[1, 1, 2, 2]`.
- Lösung: 6 Zellen, 4 Kurven; Dichte 6/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=32, maxDeductionDepth=4.
- Einstieg: Zeile 2 ist voll, obwohl die direkte Gerade nicht alle Spaltenwerte erfüllen kann. Belegschritte: `[20, 21]`.
- Eine tatsächliche maximale Prämissenkette: `[20, 23, 27, 28]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY']`; erwartete gedankliche Folge 3 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die beiden zusätzlichen Belegungen in Spalten 2 und 3 verlangen den unteren Umweg. Gegenüber Position 1 reduziert das Feld die aktive Höhe auf zwei Zeilen; die ähnliche gedrehte Belegung von Position 4 hat dort andere Ports und eine andere Restzahlentscheidung. Hier dient der enge Bereich dem Lesen der Randzahlen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....
01 ....
02 ──┐┌
03 ..└┘
```

## S1-01-01-03

- Input: 4×4; A W/2, B N/2. Zeilen `[3, 2, 3, 3]`, Spalten `[4, 4, 3, 0]`.
- Lösung: 11 Zellen, 9 Kurven; Dichte 11/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=44, maxDeductionDepth=5.
- Einstieg: Spalte 3 ist vollständig leer und begrenzt die nutzbare Fläche. Belegschritte: `[19, 20, 21, 22]`.
- Eine tatsächliche maximale Prämissenkette: `[5, 26, 37, 41, 42]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die zwei vollen Spalten 0 und 1 liefern sichere Belegung, aber ihre neun Kurven bleiben zu bestimmen. Die westliche Eintrittszelle liegt mitten im Bereich und verhindert bloßes Abfahren eines Randes; sinnvoller erster Wechsel von Ausschluss zu Anschlussform.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┘.
01 └┐..
02 ┐└┐.
03 └─┘.
```

## S1-01-01-04

- Input: 4×4; A S/2, B E/0. Zeilen `[2, 2, 2, 1]`, Spalten `[0, 0, 4, 3]`.
- Lösung: 7 Zellen, 5 Kurven; Dichte 7/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=35, maxDeductionDepth=5.
- Einstieg: Die leeren Spalten 0 und 1 schließen die linke Hälfte aus. Belegschritte: `[15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 26, 27, 29, 33]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 3 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: S/2 und E/0 machen den schmalen rechten Bereich lesbar; die einzelnen Zweierzeilen erzwingen versetzte Kurven. Nähe zu Position 2 ist bewusst auf frühe Wiederholung begrenzt: ein zusätzlicher besetzter Schritt und angrenzende statt gegenüberliegende Endpunkte ändern die Anschlussentscheidung.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌─
01 ..└┐
02 ..┌┘
03 ..│.
```

## S1-01-01-05

- Input: 4×4; A S/0, B E/1. Zeilen `[0, 4, 3, 1]`, Spalten `[3, 2, 2, 1]`.
- Lösung: 8 Zellen, 5 Kurven; Dichte 8/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=38, maxDeductionDepth=5.
- Einstieg: Die obere Nullzeile sperrt die Nordrichtung aller Zellen in Zeile 1. Belegschritte: `[12, 13, 14, 15]`.
- Eine tatsächliche maximale Prämissenkette: `[8, 19, 33, 35, 36]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'COMBINATION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die volle zweite Zeile verzahnt sich mit dem Südanschluss in Spalte 0 und den beiden mittleren Zweiercounts. Drei aktive Zeilen erweitern Position 4; die beiden inneren Kurvenpaare verlangen Überkreuzlesen statt Wiederholen des Zweispaltenkorridors.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....
01 ┌┐┌─
02 │└┘.
03 │...
```

## S1-01-01-06

- Input: 4×4; A N/2, B W/1. Zeilen `[2, 3, 3, 4]`, Spalten `[3, 1, 4, 4]`.
- Lösung: 12 Zellen, 9 Kurven; Dichte 12/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=35, maxDeductionDepth=6.
- Einstieg: Die untere Zeile mit Count 4 muss vollständig belegt sein. Belegschritte: `[12, 13, 14, 15]`.
- Eine tatsächliche maximale Prämissenkette: `[13, 17, 27, 28, 31, 34]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'COMBINATION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Zwei volle rechte Spalten kreuzen den unteren Vollanker, während Spalte 1 nur eine Zelle zulässt. Die zunächst sichere Belegung bestimmt deshalb nicht sofort die neun Kurven; guter Verdichtungsbeginn mit sichtbar breitem unteren Weg.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└┐
01 ┐.┌┘
02 │.└┐
03 └──┘
```

## S1-01-01-07

- Input: 4×4; A W/2, B S/1. Zeilen `[3, 4, 2, 2]`, Spalten `[2, 3, 4, 2]`.
- Lösung: 11 Zellen, 9 Kurven; Dichte 11/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=39, maxDeductionDepth=6.
- Einstieg: Die volle Zeile 1 verknüpft die voneinander entfernten Randbereiche. Belegschritte: `[12, 13, 14, 15]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 16, 22, 24, 25, 36]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'CHAIN']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der mittige Westport und der Südport an Spalte 1 zwingen unterschiedliche Anschlussrichtungen. Die volle Spalte 2 verbindet oberen und unteren Knick; gegenüber Position 6 wandert der Engpass nach links, statt denselben Weg zu spiegeln.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌─┐
01 ┌┘┌┘
02 ┘.│.
03 .┌┘.
```

## S1-01-01-08

- Input: 4×4; A W/3, B N/2. Zeilen `[3, 1, 1, 1]`, Spalten `[4, 1, 1, 0]`.
- Lösung: 6 Zellen, 3 Kurven; Dichte 6/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=36, maxDeductionDepth=6.
- Einstieg: Spalte 0 ist voll; der Westanschluss unten und Count 1 der unteren Zeilen begrenzen die Fortsetzung. Belegschritte: `[15, 16, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[2, 18, 24, 29, 30, 34]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'EXCLUSION']`; erwartete gedankliche Folge 3 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Bewusst luftiger Rückgriff nach zwei dichten Kurvenfeldern: drei einzelne Einserzeilen und die Nullspalte rechts lassen einen langen vertikalen Schenkel entstehen. Die geringere räumliche Last entlastet vor der Endpunktvariation, die Form am oberen Übergang bleibt ein echter Schluss.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┘.
01 │...
02 │...
03 ┘...
```

## S1-01-01-09

- Input: 4×4; A W/0, B E/3. Zeilen `[1, 2, 1, 3]`, Spalten `[2, 3, 1, 1]`.
- Lösung: 7 Zellen, 4 Kurven; Dichte 7/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=36, maxDeductionDepth=6.
- Einstieg: A an W/0 belegt die einzige Zelle der oberen Zeile; alle anderen oberen Zellen sind leer. Belegschritte: `[0, 11]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 12, 20, 21, 27, 32]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'CHAIN']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die gegenüberliegenden Anschlüsse liegen maximal höhenversetzt und die Counts enthalten weder Null- noch Volllinie. Damit ersetzt die Endpoint-Information den gewohnten Extremwertanker; der diagonale Versatz ist eine neue räumliche Anwendung der Einserlogik.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐...
01 └┐..
02 .│..
03 .└──
```

## S1-01-01-10

- Input: 4×4; A N/1, B S/1. Zeilen `[3, 1, 1, 3]`, Spalten `[0, 2, 2, 4]`.
- Lösung: 8 Zellen, 4 Kurven; Dichte 8/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=35, maxDeductionDepth=6.
- Einstieg: Spalte 0 ist leer; die beiden Anschlüsse in Spalte 1 müssen trotzdem über die volle rechte Spalte verbunden werden. Belegschritte: `[12, 13, 14, 15]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 16, 22, 23, 24, 33]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'DENSITY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der große freie Innenbereich kontrastiert mit Position 9. Beide Endpunkte liegen auf gegenüberliegenden horizontalen Kanten; die volle Spalte 3 verhindert die naheliegende direkte Verbindung und erzeugt zwei räumlich getrennte Ecken.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .└─┐
01 ...│
02 ...│
03 .┌─┘
```

## S1-01-01-11

- Input: 4×4; A N/3, B E/2. Zeilen `[4, 2, 3, 0]`, Spalten `[2, 3, 2, 2]`.
- Lösung: 9 Zellen, 5 Kurven; Dichte 9/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=41, maxDeductionDepth=7.
- Einstieg: Die untere Nullzeile begrenzt den Raum; Zeile 0 ist voll und enthält A an N/3. Belegschritte: `[15, 16, 17, 18]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 21, 29, 31, 33, 35, 36]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Gezielt ersetzt: der bisherige Abschluss hatte keine öffentliche Nullzahl. Nun müssen die volle obere Zeile, die untere Leere, der Zweierrest in Zeile 1 und die Endpunkte zusammen gelesen werden. Der Übergang in Zeile 2 liegt links vom Ausgang, sodass der gelernte Vollanker nicht als fertige Gerade missverstanden werden kann.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──┘
01 └┐..
02 .└──
03 ....
```

## S1-01-01-12

- Input: 4×4; A N/1, B W/3. Zeilen `[1, 2, 3, 3]`, Spalten `[2, 4, 3, 0]`.
- Lösung: 9 Zellen, 7 Kurven; Dichte 9/16. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=40, maxDeductionDepth=7.
- Einstieg: Spalte 3 ist leer; Spalte 1 ist vollständig belegt und enthält den Nordanschluss. Belegschritte: `[21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 12, 24, 26, 31, 37, 39]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Gezielt ersetzt, um das Nullmuster ebenfalls im zweiten Abschluss anzuwenden. Der Nordport belegt die einzige obere Zelle; darunter müssen der mittlere Rechtsbogen und die untere Rückkehr zum Westport mit den Zweier-/Dreierrestzahlen zusammenpassen. Gegenüber dem horizontalen Abschluss 11 wechselt der Vollanker auf eine innere Spalte. Eine zunächst erwogene Variante wurde vor dem Einfrieren verworfen, weil sie fast die Maske von Position 1 wiederholte; diese kleine verschachtelte Anordnung hat einen eigenständigen Abschlusszugang.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .│..
01 .└┐.
02 ┌┐│.
03 ┘└┘.
```
