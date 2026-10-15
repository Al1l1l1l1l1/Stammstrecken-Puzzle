# Author-Leveldesignreview: S1-02-04 (globale Route 08)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Größere Informationsmenge | S1-02-04-01: 5, S1-02-04-02: 6 |
| 3–5 | Weit getrennte Bereiche | S1-02-04-03: 7, S1-02-04-04: 8, S1-02-04-05: 9 |
| 6–8 | Kreuzschlussketten | S1-02-04-06: 8, S1-02-04-07: 12, S1-02-04-08: 9 |
| 9–10 | Hohe Dichte | S1-02-04-09: 10, S1-02-04-10: 11 |
| 11–12 | Lange aufgebaute Verbindung | S1-02-04-11: 14, S1-02-04-12: 18 |

**Individuelle Routenbewertung:** 1/2 vergleichen einen breiten Querbandrückgriff mit einem schmalen ruhigen Randanker. 3/4 trennen sichere Gebiete entlang der Breite; der bisherige schmale Serpentinenkopf von 5 und Vollstreifen von 7 genügen ihren jeweiligen Briefen nicht und wurden einzeln ersetzt. 6/8 verbinden obere/untere Anker über Engpässe. 9/10 zeigen hohe Dichte mit klaren Endpunktzugängen. Die zwei langen Abschlüsse unterscheiden Einserengpass und breite Vollfläche, damit nicht nur die Kurvenzahl steigt. Das optisch dichte Finale bleibt besonders für den Spieltest markiert.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-02-04-01

- Input: 6×5; A N/0, B S/5. Zeilen `[5, 1, 5, 1, 6]`, Spalten `[4, 3, 3, 3, 4, 1]`.
- Lösung: 18 Zellen, 6 Kurven; Dichte 18/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=68, maxDeductionDepth=5.
- Einstieg: Die untere Vollzeile liefert im 6×5-Raster bekannte sichere Belegung. Belegschritte: `[18, 19, 20, 21, 22]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 25, 39, 48, 53]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'CHAIN']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Zwei Einserzeilen teilen drei lange Querbänder. Der breitere Serpentinenrückgriff ist hier als einfacher Übergang zur größeren Informationsmenge sinnvoll; die beiden oberen Bänder haben jeweils nur fünf statt sechs Zellen und müssen in den Spaltenwerten abgestimmt werden.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └───┐.
01 ....│.
02 ┌───┘.
03 │.....
04 └────┐
```

## S1-02-04-02

- Input: 6×5; A E/4, B N/3. Zeilen `[3, 3, 1, 1, 1]`, Spalten `[0, 0, 0, 2, 2, 5]`.
- Lösung: 9 Zellen, 5 Kurven; Dichte 9/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=62, maxDeductionDepth=6.
- Einstieg: Die volle rechte Spalte und die drei unteren Einserzeilen sichern einen Randstrang. Belegschritte: `[35, 36, 37, 38]`.
- Eine tatsächliche maximale Prämissenkette: `[33, 43, 54, 58, 59, 60]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY']`; erwartete gedankliche Folge 3 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die drei Nullspalten links machen das größere Raster ruhig lesbar; oben bleibt nur ein kleines Kurvenpaar zu bestimmen. Diese reduzierte aktive Breite dient noch dem Wiedereinstieg und darf nicht als Erfüllung des nächsten Gebietsbriefs ausgegeben werden.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...│┌┐
01 ...└┘│
02 .....│
03 .....│
04 .....└
```

## S1-02-04-03

- Input: 6×5; A S/5, B W/3. Zeilen `[0, 0, 0, 4, 5]`, Spalten `[2, 2, 1, 2, 1, 1]`.
- Lösung: 9 Zellen, 7 Kurven; Dichte 9/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=57, maxDeductionDepth=7.
- Einstieg: Die oberen drei Nullzeilen begrenzen die beiden unteren Randgebiete. Belegschritte: `[18, 19, 20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 36, 42, 43, 47, 48, 53]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Westport links und Südport rechts sind über die gesamte Breite getrennt; Counts 1 in Spalten 2/4/5 erlauben nur schmale Übergänge. Trotz zwei aktiver Reihen gibt es einen linken Rückknick und einen rechten Ausgangsarm mit unterschiedlichen Restzahlen; behalten als räumlich entfernte horizontale Gebiete.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ......
01 ......
02 ......
03 ┐┌─┐..
04 └┘.└─┐
```

## S1-02-04-04

- Input: 6×5; A E/2, B N/1. Zeilen `[2, 4, 4, 6, 3]`, Spalten `[4, 2, 1, 4, 4, 4]`.
- Lösung: 19 Zellen, 13 Kurven; Dichte 19/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=70, maxDeductionDepth=8.
- Einstieg: Die volle Zeile 3 verknüpft sichere Belegung über die gesamte Breite. Belegschritte: `[18, 19, 20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 24, 31, 42, 48, 59, 61, 64]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Nordport in Spalte 1 und der Ostport in Zeile 2 liegen über verschiedenen Viererspaltenblöcken. Die Einzerspalte 2 koppelt diese beiden Gebiete nur unten; das Feld liefert deutlich auseinanderliegende Schlussansätze.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘....
01 │..┌─┐
02 │..└┐└
03 └──┐└┐
04 ...└─┘
```

## S1-02-04-05

- Input: 6×5; A W/4, B E/1. Zeilen `[0, 1, 2, 6, 6]`, Spalten `[2, 2, 2, 2, 3, 4]`.
- Lösung: 15 Zellen, 14 Kurven; Dichte 15/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=67, maxDeductionDepth=9.
- Einstieg: Die beiden unteren Vollzeilen sichern einen breiten Belegungsbereich. Belegschritte: `[29, 30, 31, 32, 33, 34]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 40, 49, 55, 56, 59, 60, 63, 64]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Gezielt ersetzt: W/4 und E/1 liegen mit Abstand 8 weit auseinander. Der einzelne obere Ostanschluss und Zweierrest in Zeile 2 bilden ein zweites entferntes sicheres Gebiet; die Vierer-/Dreiercounts rechts verknüpfen es mit dem dichten Boden. Die 14 Kurven bleiben offen, obwohl der Boden vollständig belegt ist.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ......
01 .....┌
02 ....┌┘
03 ┌┐┌┐└┐
04 ┘└┘└─┘
```

## S1-02-04-06

- Input: 6×5; A W/1, B S/4. Zeilen `[6, 5, 2, 2, 1]`, Spalten `[2, 2, 2, 1, 5, 4]`.
- Lösung: 16 Zellen, 13 Kurven; Dichte 16/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=70, maxDeductionDepth=8.
- Einstieg: Die obere Vollzeile und Spalte 4 mit Count 5 geben sich kreuzende Anker. Belegschritte: `[18, 19, 20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 31, 41, 42, 46, 47, 51, 58]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die dichte obere Doppelkurve und der untere Zweispaltenrücklauf müssen über den Einsercount von Spalte 3 abgestimmt werden. Der Wechsel von breiten oberen zu schmalen unteren Belegungen schafft Folgeausschlüsse über die beiden Achsen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐┌──┐
01 ┘└┘.┌┘
02 ....└┐
03 ....┌┘
04 ....│.
```

## S1-02-04-07

- Input: 6×5; A E/3, B W/1. Zeilen `[2, 4, 4, 3, 3]`, Spalten `[2, 2, 5, 4, 2, 1]`.
- Lösung: 16 Zellen, 12 Kurven; Dichte 16/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=70, maxDeductionDepth=12.
- Einstieg: Die volle Spalte 2 reicht vom oberen kleinen Bogen bis zum unteren Querarm. Belegschritte: `[18, 19, 20, 21, 22]`.
- Eine tatsächliche maximale Prämissenkette: `[11, 25, 30, 32, 35, 36, 53, 56, 63, 64, 65, 66]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Gezielt ersetzt durch 16 Zellen auf allen sechs Spalten und fünf Zeilen, zwölf Kurven und nur einen Vollanker. Die beiden Viererzeilen und Spalte 3 mit Count 4 koppeln linken Westport, oberen Rückknick und rechten Ostzugang; die Zweiercounts links bestimmen zuerst einen eng begrenzten Bereich, dessen Folgeausschlüsse im rechten Querarm wirken.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌┐..
01 ┐┌┘│..
02 └┘┌┘..
03 ..│.┌─
04 ..└─┘.
```

## S1-02-04-08

- Input: 6×5; A N/2, B S/2. Zeilen `[3, 2, 2, 4, 4]`, Spalten `[0, 2, 3, 5, 5, 0]`.
- Lösung: 15 Zellen, 12 Kurven; Dichte 15/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=73, maxDeductionDepth=9.
- Einstieg: Die beiden äußeren Nullspalten begrenzen den Raum; zwei volle innere Spalten bleiben sicher. Belegschritte: `[18, 19, 20, 21, 22]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 38, 59, 60, 61, 62, 64, 67, 72]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die unteren Viererzeilen müssen an den oben engeren Zweier-/Dreierbereich anschließen. Nord und Süd liegen in Spalte 2, die volle Achse jedoch rechts davon; die direkte Verbindung scheitert an den verteilten Randcounts und verlangt einen linken unteren Umweg.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└─┐.
01 ...┌┘.
02 ...└┐.
03 .┌─┐│.
04 .└┐└┘.
```

## S1-02-04-09

- Input: 6×5; A E/0, B S/2. Zeilen `[4, 5, 4, 2, 1]`, Spalten `[3, 4, 4, 2, 2, 1]`.
- Lösung: 16 Zellen, 13 Kurven; Dichte 16/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=75, maxDeductionDepth=10.
- Einstieg: B an S/2 belegt die Einserzeile unten allein. Belegschritte: `[5, 14]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 19, 37, 39, 44, 47, 51, 60, 67, 68]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die Belegungsdichte steigt nach oben, doch die Form am rechten Ostanschluss wird durch die Einzerspalte 5 getrennt. 16/30 Zellen erzeugen ein breites dichtes Kopfgebiet mit einer klaren unteren Spitze; ein natürlicher Zugang bleibt vorhanden.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐..┌─
01 │└┐┌┘.
02 └┐└┘..
03 .└┐...
04 ..│...
```

## S1-02-04-10

- Input: 6×5; A W/1, B S/1. Zeilen `[5, 5, 3, 4, 4]`, Spalten `[5, 5, 4, 5, 2, 0]`.
- Lösung: 21 Zellen, 17 Kurven; Dichte 21/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=74, maxDeductionDepth=11.
- Einstieg: Die vollen Spalten 0 und 1 liefern sichere Belegung, die rechte Nullspalte begrenzt die Fläche. Belegschritte: `[18, 19, 20, 21]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 38, 41, 44, 45, 52, 54, 56, 57, 66, 68]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'COMBINATION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: 21 Zellen im 5×5-aktiven Gebiet kontrastieren den spitz zulaufenden Vorgänger. Der Westport liegt innen, der Südport unten daneben; mehrere Kurven müssen durch die Dreierzeile 2 hindurch mit den fast vollen Querreihen zusammenpassen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐┌─┐.
01 ┘└┘┌┘.
02 ┌┐.│..
03 │└┐│..
04 └┐└┘..
```

## S1-02-04-11

- Input: 6×5; A E/1, B W/3. Zeilen `[4, 3, 6, 4, 3]`, Spalten `[2, 1, 5, 4, 4, 4]`.
- Lösung: 20 Zellen, 14 Kurven; Dichte 20/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=75, maxDeductionDepth=14.
- Einstieg: Die volle mittlere Zeile gibt einen klaren Queranker. Belegschritte: `[18, 19, 20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[10, 24, 47, 49, 50, 51, 54, 55, 57, 61, 62, 69, 70, 74]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die rechte breite Belegung und der linke Westport werden durch den Einsercount in Spalte 1 verbunden. Vierercounts rechts reichen nicht für eine fertige Form: der untere Rücklauf muss sowohl mit dem mittigen Anker als auch mit dem Ostport oben übereinstimmen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌──┐
01 ..└┐.└
02 ┌─┐└─┐
03 ┘.│.┌┘
04 ..└─┘.
```

## S1-02-04-12

- Input: 6×5; A E/0, B N/1. Zeilen `[4, 4, 3, 6, 6]`, Spalten `[4, 5, 2, 2, 5, 5]`.
- Lösung: 23 Zellen, 19 Kurven; Dichte 23/30. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=82, maxDeductionDepth=18.
- Einstieg: Die unteren beiden Vollzeilen sichern eine breite Abschlussfläche. Belegschritte: `[18, 19, 20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 35, 44, 45, 48, 58, 59, 60, 62, 63, 64, 66, 68, 69, 71, 73, 74, 79]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'DENSITY']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die oberen beiden Zweispaltengruppen liegen durch die niedrigen mittleren Counts auseinander. Erst der untere volle Bereich verbindet ihre Kurvenzüge; 19 Kurven auf 23 Zellen verlangen eine lange genaue Formfolge, deren Lesbarkeit extern zu prüfen bleibt.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘..┌─
01 └┐..└┐
02 .│..┌┘
03 ┌┘┌┐└┐
04 └─┘└─┘
```
