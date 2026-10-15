# Author-Leveldesignreview: S1-02-01 (globale Route 05)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Größenübergang | S1-02-01-01: 5, S1-02-01-02: 6 |
| 3–5 | Volle und leere Linien kreuzen | S1-02-01-03: 5, S1-02-01-04: 6, S1-02-01-05: 6 |
| 6–8 | Mehrere Achsen verbinden | S1-02-01-06: 7, S1-02-01-07: 8, S1-02-01-08: 8 |
| 9–10 | Andere Eckzwänge | S1-02-01-09: 7, S1-02-01-10: 8 |
| 11–12 | Mittlere Dichte | S1-02-01-11: 10, S1-02-01-12: 14 |

**Individuelle Routenbewertung:** Der Größenwechsel wird durch ein schmales Kopfstück und einen breiten Bodenanker abgefedert. 3–5 koppeln Nullen und Vollanker an verschiedene Endpunktfolgen; 6–8 kreuzen vertikale und horizontale Verpflichtungen. 9/10 wechseln von klarer Vollspalte zu einem dünnen Feld ohne Extremzahl. Die Abschlüsse haben Dichten 16/25 und 12/25 und verschiedene Geometrie. „Mittlere Dichte“ ist nicht als Zahlenintervall spezifiziert; die erste Variante ist deutlich dicht und bleibt gezielt zur Contentabnahme markiert. Keine automatische Tiefe-14-Freigabe.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-02-01-01

- Input: 5×5; A S/0, B N/2. Zeilen `[3, 3, 1, 1, 1]`, Spalten `[5, 2, 2, 0, 0]`.
- Lösung: 9 Zellen, 4 Kurven; Dichte 9/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=55, maxDeductionDepth=5.
- Einstieg: Spalte 0 ist voll und die drei unteren Einserzeilen lassen daneben nichts zu. Belegschritte: `[20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[22, 43, 49, 52, 53]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY']`; erwartete gedankliche Folge 3 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Das neue 5×5-Feld wird zunächst nur im oberen Dreispaltengebiet differenziert. Der lange Schenkel reduziert die neue Informationsmenge bewusst; der kleine obere Umweg übt die Nord-/Südportlage, ohne einen neuen Kurventyp einzuführen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐│..
01 │└┘..
02 │....
03 │....
04 │....
```

## S1-02-01-02

- Input: 5×5; A E/1, B W/3. Zeilen `[0, 1, 2, 3, 5]`, Spalten `[2, 1, 1, 3, 4]`.
- Lösung: 11 Zellen, 8 Kurven; Dichte 11/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=49, maxDeductionDepth=6.
- Einstieg: Die volle untere Zeile gibt einen breiten Anker. Belegschritte: `[25, 26, 27, 28, 29]`.
- Eine tatsächliche maximale Prämissenkette: `[6, 24, 35, 41, 44, 48]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die Einserzeile am Ostanschluss und die linke Zweierspalte begrenzen zwei entfernte Endgebiete. Der rechte Rückknick führt auf den Boden, der linke auf den Westport; mehr Breite als im Vorgänger, bei weiterhin klarer Belegungsinformation.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .....
01 ....┌
02 ...┌┘
03 ┐..└┐
04 └───┘
```

## S1-02-01-03

- Input: 5×5; A N/2, B S/3. Zeilen `[3, 2, 1, 1, 1]`, Spalten `[0, 0, 1, 5, 2]`.
- Lösung: 8 Zellen, 4 Kurven; Dichte 8/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=52, maxDeductionDepth=5.
- Einstieg: Die beiden linken Nullspalten sperren die westliche Hälfte; Spalte 3 ist voll. Belegschritte: `[20, 21, 22, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[2, 28, 35, 36, 48]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'COMBINATION']`; erwartete gedankliche Folge 3 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die drei unteren Einserzeilen verknüpfen Ausschluss und volle Belegung entlang derselben Achse. Nähe zu S1-02-02-02 ist vorhanden; dort liegt die Vollachse quer und endet am Westport. Behalten als explizite Einführung gekreuzter Extremwerte, die geringe Breite allein ist kein Ausschlusskriterium.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..└─┐
01 ...┌┘
02 ...│.
03 ...│.
04 ...│.
```

## S1-02-01-04

- Input: 5×5; A N/1, B E/3. Zeilen `[4, 5, 3, 1, 0]`, Spalten `[3, 3, 1, 2, 4]`.
- Lösung: 13 Zellen, 9 Kurven; Dichte 13/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=51, maxDeductionDepth=6.
- Einstieg: Die untere Nullzeile und die volle Zeile 1 begrenzen den durchgehenden Belegungsbereich. Belegschritte: `[25, 26, 27, 28, 29]`.
- Eine tatsächliche maximale Prämissenkette: `[18, 33, 38, 42, 46, 47]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Nordport in Spalte 1 und der Einserausgang rechts liegen an unterschiedlichen Seiten des Vollankers. Spalte 2 mit Count 1 schneidet die breitere Verbindung in der Mitte; diese Restzahl koppelt die beiden Extremzeilen statt sie nur unabhängig abzuhaken.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┘.┌┐
01 │┌─┘│
02 └┘..│
03 ....└
04 .....
```

## S1-02-01-05

- Input: 5×5; A E/3, B W/4. Zeilen `[0, 0, 3, 5, 2]`, Spalten `[3, 3, 2, 1, 1]`.
- Lösung: 10 Zellen, 6 Kurven; Dichte 10/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=54, maxDeductionDepth=6.
- Einstieg: Zeile 3 ist voll, die beiden oberen Zeilen sind leer. Belegschritte: `[26, 27, 28, 29]`.
- Eine tatsächliche maximale Prämissenkette: `[29, 33, 39, 47, 48, 53]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'EXCLUSION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Links müssen zwei Dreierspalten den oberen Bogen und den unteren Westausgang aufnehmen, rechts sind nur Einserreste erlaubt. Damit wirkt die aus den Nullzeilen folgende Raumgrenze bis in die Form des mittleren Vollankers.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .....
01 .....
02 ┌─┐..
03 └┐└──
04 ─┘...
```

## S1-02-01-06

- Input: 5×5; A E/1, B W/3. Zeilen `[3, 3, 5, 4, 2]`, Spalten `[2, 1, 4, 5, 5]`.
- Lösung: 17 Zellen, 14 Kurven; Dichte 17/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=64, maxDeductionDepth=7.
- Einstieg: Die volle mittlere Zeile kreuzt die beiden vollen rechten Spalten. Belegschritte: `[16, 17, 18, 19, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 24, 35, 55, 57, 58, 60]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Im dichten rechten Gebiet liegen mehrere mögliche Kurvenformen; die schmale Spalte 1 koppelt es mit dem linken Westanschluss. Die obere und untere Rückkehr liegen auf verschiedenen Zeilen, sodass mindestens zwei Achsen zusammen gelesen werden müssen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..┌─┐
01 ..└┐└
02 ┌─┐└┐
03 ┘.└┐│
04 ...└┘
```

## S1-02-01-07

- Input: 5×5; A W/4, B N/1. Zeilen `[1, 3, 3, 1, 4]`, Spalten `[1, 4, 3, 4, 0]`.
- Lösung: 12 Zellen, 5 Kurven; Dichte 12/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=58, maxDeductionDepth=8.
- Einstieg: B an N/1 belegt allein die obere Zeile. Belegschritte: `[1, 11]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 18, 25, 47, 49, 54, 55, 56]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die rechte Nullspalte und Count 1 in Spalte 0 lassen einen unteren Querweg entstehen, während die Dreierzeilen in der Mitte den oberen Umweg eingrenzen. Wenige Kurven, aber zwei entfernte Restzahlfolgen: eine ruhigere Formlast innerhalb der Verdichtung.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .│...
01 .│┌┐.
02 .└┘│.
03 ...│.
04 ───┘.
```

## S1-02-01-08

- Input: 5×5; A S/3, B E/2. Zeilen `[3, 5, 3, 3, 4]`, Spalten `[5, 5, 4, 2, 2]`.
- Lösung: 18 Zellen, 15 Kurven; Dichte 18/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=66, maxDeductionDepth=8.
- Einstieg: Spalte 0 ist voll; die volle Zeile 1 liefert eine quer dazu liegende Sicherheit. Belegschritte: `[16, 21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 29, 36, 51, 52, 61, 62, 63]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'DENSITY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Ostport liegt direkt unter dem dichten Querband, der Südport jedoch weiter links unten. 15 Kurven verknüpfen den oberen und unteren Bereich über die ebenfalls volle Spalte 1; die Verschränkung erweitert Position 6 statt denselben Rechtsbogen zu wiederholen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐..
01 └┐└─┐
02 ┌┘..└
03 │┌┐..
04 └┘└┐.
```

## S1-02-01-09

- Input: 5×5; A N/3, B W/3. Zeilen `[3, 1, 2, 3, 2]`, Spalten `[1, 5, 4, 1, 0]`.
- Lösung: 11 Zellen, 7 Kurven; Dichte 11/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=54, maxDeductionDepth=7.
- Einstieg: Die volle Spalte 1 verbindet den Nordbogen mit der unteren Rückkehr. Belegschritte: `[20, 21, 22, 23, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[9, 18, 41, 42, 49, 51, 53]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'EXCLUSION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: W/3 liegt nahe am unteren Rückknick und verbraucht die einzige Belegung von Spalte 0. Dadurch entsteht eine andere sichere Ecke als am äußeren Bodenweg in Position 7; die freie rechte Spalte erleichtert den räumlichen Wechsel.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌─┘.
01 .│...
02 .└┐..
03 ─┐│..
04 .└┘..
```

## S1-02-01-10

- Input: 5×5; A E/3, B W/0. Zeilen `[1, 3, 1, 2, 3]`, Spalten `[2, 1, 4, 1, 2]`.
- Lösung: 10 Zellen, 6 Kurven; Dichte 10/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=52, maxDeductionDepth=8.
- Einstieg: B an W/0 belegt die Einserzeile oben vollständig. Belegschritte: `[0, 10]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 16, 24, 25, 37, 46, 47, 48]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Ohne Null-/Volllinie müssen der obere Arm, die fast volle Mittelsäule und die untere Rückkehr aus Restzahlen kombiniert werden. Beide Endpunkte liegen höhenversetzt an gegenüberliegenden Seiten; der klare dünne Umriss rechtfertigt die geringere Dichte als räumliche Variante.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐....
01 └─┐..
02 ..│..
03 ..│.┌
04 ..└─┘
```

## S1-02-01-11

- Input: 5×5; A N/0, B S/3. Zeilen `[5, 1, 5, 1, 4]`, Spalten `[4, 3, 3, 3, 3]`.
- Lösung: 16 Zellen, 6 Kurven; Dichte 16/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=57, maxDeductionDepth=10.
- Einstieg: Die volle obere Zeile und A an N/0 geben eine gut lesbare Querachse. Belegschritte: `[16, 17, 18, 19]`.
- Eine tatsächliche maximale Prämissenkette: `[14, 25, 29, 31, 33, 35, 38, 39, 47, 53]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'CHAIN']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Zwei Einserzeilen wechseln mit langen Querbändern; die Folge ist ein früher Serpentinenvergleich über das ganze 5×5-Feld. 16/25 liegt deutlich über dem dünnen zweiten Abschluss, aber die sieben freien Zellen bleiben gut lokalisierbar; der Bedeutungsumfang „mittlere Dichte“ braucht die externe fachliche Abnahme, keinen erfundenen Prozentbereich.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └───┐
01 ....│
02 ┌───┘
03 │....
04 └──┐.
```

## S1-02-01-12

- Input: 5×5; A W/2, B N/3. Zeilen `[1, 1, 3, 4, 3]`, Spalten `[3, 1, 2, 4, 2]`.
- Lösung: 12 Zellen, 7 Kurven; Dichte 12/25. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=60, maxDeductionDepth=14.
- Einstieg: B an N/3 belegt die Einserzeile 0; Zeile 1 darf ebenfalls nur eine Zelle tragen. Belegschritte: `[3, 7]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 18, 20, 24, 26, 31, 39, 41, 46, 47, 52, 54, 55, 57]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der untere breite Bogen wird über den linken Westanschluss und die fast volle Spalte 3 begrenzt. Die 12 Zellen sind räumlich klar, doch das Aufeinanderfolgen der Restzahlen ist lang; guter Kontrast zum dichten Querbandfinale, mit offenem Spieltest zur wahrgenommenen Schlusslast.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...│.
01 ...│.
02 ┐..└┐
03 │.┌─┘
04 └─┘..
```
