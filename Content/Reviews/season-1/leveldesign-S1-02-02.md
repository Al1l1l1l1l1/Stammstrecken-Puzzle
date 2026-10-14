# Author-Leveldesignreview: S1-02-02 (globale Route 06)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Sichere Belegung | S1-02-02-01: 5, S1-02-02-02: 6 |
| 3–5 | Dichtere Struktur | S1-02-02-03: 8, S1-02-02-04: 9, S1-02-02-05: 9 |
| 6–8 | Mehrere Achsen | S1-02-02-06: 7, S1-02-02-07: 8, S1-02-02-08: 8 |
| 9–10 | Lange Verbindung bei begrenzter Dichte | S1-02-02-09: 7, S1-02-02-10: 8 |
| 11–12 | Kompakte Verdichtung | S1-02-02-11: 10, S1-02-02-12: 14 |

**Individuelle Routenbewertung:** 1/2 wiederholen Vollbelegung in entgegengesetzten Achsen, 3–5 verteilen jeweils 14 Zellen verschieden. Der Vollstreifen in 6 wiederholt ein altes Motiv ohne ausreichende Mehrlast und wurde ersetzt; 7 kann dann als einzelner enger Gegenpol bleiben. 8 verdichtet wieder breite Belegung. Die zwei niedrigen Dichtevarianten 9/10 sind ausdrücklich gewollt; gleiche rasterweite Leerränder wären an dieser Stelle kein automatischer Mangel. 11/12 schließen mit engpassgeführten Bögen statt Vollflächen ab, was die lange Schlussfolge erklärt und zum Spieltest übergeben wird.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-02-02-01

- Input: 5×5; A N/0, B S/4. Zeilen `[5, 1, 4, 1, 4]`, Spalten `[1, 4, 3, 3, 4]`.
- Lösung: 15 Zellen, 6 Kurven; Dichte 15/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=58, maxDeductionDepth=5.
- Einstieg: Die obere volle Zeile gibt einen bekannten Belegungsanker. Belegschritte: `[16, 17, 18, 19]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 21, 36, 43, 51]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die zwei Einserzeilen legen abwechselnd den rechten und linken Übergang zwischen den Querbändern fest. Nähe zu S1-02-03-01 ist ein Lernrückgriff, aber hier setzt die volle obere Zeile früher an und der Südport liegt ganz rechts; noch kein neuer Schwerpunkt.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └───┐
01 ....│
02 .┌──┘
03 .│...
04 .└──┐
```

## S1-02-02-02

- Input: 5×5; A E/1, B W/3. Zeilen `[2, 5, 1, 1, 0]`, Spalten `[4, 2, 1, 1, 1]`.
- Lösung: 9 Zellen, 4 Kurven; Dichte 9/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=50, maxDeductionDepth=6.
- Einstieg: Die volle Zeile 1 und die untere Nullzeile sind unabhängig sicher. Belegschritte: `[16, 17, 18, 19]`.
- Eine tatsächliche maximale Prämissenkette: `[11, 29, 37, 39, 47, 49]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'EXCLUSION']`; erwartete gedankliche Folge 3 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der fast volle linke Schenkel trägt zwei Einserzeilen; oben erlaubt die Zweierzeile den kurzen Rückknick. Die Nähe zu S1-02-01-03 ist als Achsenwechsel einer Wiederholung plausibel: jetzt sind die Ports Ost/West statt Nord/Süd.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐...
01 │└───
02 │....
03 ┘....
04 .....
```

## S1-02-02-03

- Input: 5×5; A W/2, B S/3. Zeilen `[0, 3, 3, 4, 4]`, Spalten `[4, 4, 4, 2, 0]`.
- Lösung: 14 Zellen, 11 Kurven; Dichte 14/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=62, maxDeductionDepth=8.
- Einstieg: Die obere Nullzeile begrenzt die nutzbare Fläche auf die unteren vier Zeilen. Belegschritte: `[16, 17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 21, 48, 51, 55, 56, 57, 59]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: 14 Zellen füllen diesen Bereich stark; die rechte Nullspalte macht die zwei unteren Viererzeilen zu vollen aktiven Reihen, aber sie bestimmt ihre Kurven nicht. Der Westport sitzt in der Mitte und zwingt den Rücklauf zwischen den beiden dichten Paaren.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .....
01 ┌─┐..
02 ┘┌┘..
03 ┌┘┌┐.
04 └─┘│.
```

## S1-02-02-04

- Input: 5×5; A W/1, B E/4. Zeilen `[4, 3, 1, 3, 3]`, Spalten `[2, 1, 5, 4, 2]`.
- Lösung: 14 Zellen, 10 Kurven; Dichte 14/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=65, maxDeductionDepth=9.
- Einstieg: Spalte 2 ist voll und kreuzt den Einserengpass in Zeile 2. Belegschritte: `[16, 17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[18, 23, 28, 43, 52, 59, 60, 61, 63]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'CHAIN']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der obere weite Bogen und die kurze untere Doppelkurve sind über denselben senkrechten Strang verbunden. Im Gegensatz zu Position 3 sind alle Achsen aktiv; ein zusätzlicher Leerbereich trennt die Formlast in zwei gut lesbare Gebiete.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──┐.
01 ┘.┌┘.
02 ..│..
03 ..│┌┐
04 ..└┘└
```

## S1-02-02-05

- Input: 5×5; A N/4, B W/3. Zeilen `[2, 2, 2, 4, 4]`, Spalten `[1, 2, 1, 5, 5]`.
- Lösung: 14 Zellen, 11 Kurven; Dichte 14/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=61, maxDeductionDepth=9.
- Einstieg: Die beiden rechten Spalten sind voll, der Nordanschluss fixiert die erste Form. Belegschritte: `[25, 26, 27, 28]`.
- Eine tatsächliche maximale Prämissenkette: `[4, 38, 46, 47, 49, 56, 57, 58, 60]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'COMBINATION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die oberen Zweierzeilen erzwingen kleine Wendungen, die unteren Viererzeilen einen größeren linken Querweg. Hohe Dichte konzentriert sich rechts; damit verändert das Feld die Verteilung der ebenfalls 14 Zellen gegenüber Position 4.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...┌┘
01 ...└┐
02 ...┌┘
03 ─┐.└┐
04 .└──┘
```

## S1-02-02-06

- Input: 5×5; A W/1, B S/1. Zeilen `[5, 3, 1, 2, 4]`, Spalten `[2, 2, 2, 5, 4]`.
- Lösung: 15 Zellen, 9 Kurven; Dichte 15/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=57, maxDeductionDepth=7.
- Einstieg: Die volle Spalte 3 kreuzt die volle obere Zeile. Belegschritte: `[19, 27, 28, 29, 30]`.
- Eine tatsächliche maximale Prämissenkette: `[5, 23, 39, 40, 45, 54, 55]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Gezielt ersetzt durch ein 15-Zellen-Feld mit zwei entfernten Querarmen und einem mittleren Einserengpass. Der Westport in Zeile 1 und Südport in Spalte 1 liegen abseits der Vollachse; mehrere Zweiercounts und der rechte Viererrest müssen die oberen und unteren Bögen gemeinsam bestimmen. Das erfüllt die geforderte Prüfung über mehrere Achsen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌───┐
01 ┘..┌┘
02 ...│.
03 ...└┐
04 .┌──┘
```

## S1-02-02-07

- Input: 5×5; A N/0, B S/1. Zeilen `[3, 2, 2, 2, 1]`, Spalten `[1, 5, 4, 0, 0]`.
- Lösung: 10 Zellen, 8 Kurven; Dichte 10/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=56, maxDeductionDepth=8.
- Einstieg: Spalte 1 ist voll, die beiden rechten Spalten sind leer. Belegschritte: `[23, 24, 25, 26]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 20, 39, 40, 50, 51, 53, 55]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'EXCLUSION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die Einserzeile unten und der Dreierkopf begrenzen die Kurvenserie von zwei Seiten. Der schmale vertikale Rückgriff nach der dichteren Einführung ist allein kein Briefverstoß: Zeilenreste und Spaltenbelegung bestimmen tatsächlich verschiedene Teile der Formenfolge; wegen Nähe zum Streifenmotiv von 6 nun neben einer anderen Ersatzgeometrie zu beurteilen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └─┐..
01 .┌┘..
02 .└┐..
03 .┌┘..
04 .│...
```

## S1-02-02-08

- Input: 5×5; A S/0, B N/1. Zeilen `[5, 5, 4, 2, 2]`, Spalten `[4, 5, 4, 3, 2]`.
- Lösung: 18 Zellen, 16 Kurven; Dichte 18/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=65, maxDeductionDepth=8.
- Einstieg: Die oberen beiden Vollzeilen sichern eine breite Belegungsfläche. Belegschritte: `[16, 17, 18, 19]`.
- Eine tatsächliche maximale Prämissenkette: `[19, 28, 43, 48, 51, 58, 59, 60]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'DENSITY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die Belegung fällt danach auf 4/2/2 ab und muss zum Südport links führen. Die fast diagonale Kurvenfolge ist anders als die vertikale Wechselserie in Position 7; viele Formzwänge kreuzen die volle Spalte 1 und binden den unteren Bereich ein.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘┌─┐
01 │┌┘┌┘
02 └┘┌┘.
03 .┌┘..
04 ┌┘...
```

## S1-02-02-09

- Input: 5×5; A E/1, B W/2. Zeilen `[0, 2, 5, 3, 0]`, Spalten `[1, 1, 2, 3, 3]`.
- Lösung: 10 Zellen, 6 Kurven; Dichte 10/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=57, maxDeductionDepth=7.
- Einstieg: Die obere und untere Nullzeile begrenzen den Raum auf drei mittlere Zeilen. Belegschritte: `[16, 17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 32, 37, 50, 51, 54, 56]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die nahe Seiteneintrittslage E/1–W/2 erzwingt einen weiten Querweg mit einem rechten Umweg. 10 Zellen und sechs Kurven erfüllen die begrenzte Dichte; die leere Außenfläche ist hier eine sachlich passende räumliche Variation.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .....
01 ...┌─
02 ──┐└┐
03 ..└─┘
04 .....
```

## S1-02-02-10

- Input: 5×5; A W/0, B E/2. Zeilen `[5, 4, 2, 0, 0]`, Spalten `[1, 2, 2, 3, 3]`.
- Lösung: 11 Zellen, 8 Kurven; Dichte 11/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=58, maxDeductionDepth=8.
- Einstieg: Die obere Vollzeile fixiert die Belegung am Westanschluss. Belegschritte: `[16, 17, 18, 19]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 31, 40, 48, 52, 53, 54, 56]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Zwei untere Nullzeilen halten die geringe Dichte lesbar; in der zweiten Zeile bleibt ein freies Feld zwischen zwei Wendungen. Trotz ähnlichem Countverlauf zu Position 9 wechseln Vollanker und Ausgänge ihre Lage, sodass kein einfacher gespiegelt gelöster Streifen entsteht.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ─┐┌─┐
01 .└┘┌┘
02 ...└─
03 .....
04 .....
```

## S1-02-02-11

- Input: 5×5; A S/4, B N/1. Zeilen `[2, 1, 4, 4, 1]`, Spalten `[4, 3, 1, 2, 2]`.
- Lösung: 12 Zellen, 8 Kurven; Dichte 12/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=58, maxDeductionDepth=10.
- Einstieg: A an S/4 belegt die Einserzeile unten vollständig. Belegschritte: `[1, 15]`.
- Eine tatsächliche maximale Prämissenkette: `[15, 16, 20, 24, 27, 46, 47, 48, 51, 52]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die obere Einserzeile 1 und der Vierercount links erzeugen eine zweite entfernte Sicherheit. In der Mitte liegt eine breite Rückkurve; die ähnlichen Ports von Position 4 verdecken nicht die Änderung von Vollspalten zu Einserengpässen, weshalb der nahe Jaccardwert sachlich begründet behalten wird.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘...
01 │....
02 │┌─┐.
03 └┘.└┐
04 ....│
```

## S1-02-02-12

- Input: 5×5; A S/1, B N/3. Zeilen `[1, 2, 1, 3, 2]`, Spalten `[2, 2, 3, 2, 0]`.
- Lösung: 9 Zellen, 6 Kurven; Dichte 9/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=57, maxDeductionDepth=14.
- Einstieg: Die rechte Nullspalte verbietet Fortsetzungen neben dem Nordanschluss. Belegschritte: `[20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 18, 24, 28, 32, 35, 39, 40, 47, 49, 52, 53, 54, 56]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'EXCLUSION']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Vier aktive Spalten tragen nur neun Zellen; die Einserzeilen 0 und 2 zwingen den versetzten Übergang zum unteren Rückknick. Kompakter und luftiger als 11, aber mit längerer serieller Schlussfolge; als zweiter Abschluss geeignet, wahrgenommene Last nicht gemessen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...│.
01 ..┌┘.
02 ..│..
03 ┌─┘..
04 └┐...
```
