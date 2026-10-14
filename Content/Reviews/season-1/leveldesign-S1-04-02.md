# Author-Leveldesignreview: S1-04-02 (globale Route 14)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Deutlicher Anfang | S1-04-02-01: 6, S1-04-02-02: 7 |
| 3–5 | Mittlere Randzahlen | S1-04-02-03: 13, S1-04-02-04: 17, S1-04-02-05: 18 |
| 6–8 | Kombination statt einzelner Voll-/Nullzahl | S1-04-02-06: 19, S1-04-02-07: 20, S1-04-02-08: 22 |
| 9–10 | Ungleichmäßige Dichte und Entfernung | S1-04-02-09: 12, S1-04-02-10: 13 |
| 11–12 | Lange Schlusskette | S1-04-02-11: 24, S1-04-02-12: 26 |

**Individuelle Routenbewertung:** 1/2 kontrastieren einen kleinen klaren Bereich mit asymmetrischen langen Armen. 3–8 besitzen keine öffentliche Null-/Vollzahl und führen den Kombinationsbrief konsequent über Ports, mittlere Counts und Restzahlen aus; Gleichheit einzelner Werte ist nicht gleichbedeutend mit derselben Belegung. 9/10 verteilen sehr geringe aktive Flächen einmal horizontal, einmal vertikal und passen dadurch zur Dichte-/Entfernungsvariation. 11/12 sind bewusst dichter Flächen- und dünner Treppenabschluss. Der Tiefensprung zur Schlussgruppe ist offen im Spieltest, aber kein Anlass für eine mathematisch gesetzte neue Grenze.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-04-02-01

- Input: 7×7; A N/3, B W/3. Zeilen `[3, 1, 1, 3, 3, 0, 0]`, Spalten `[2, 5, 3, 1, 0, 0, 0]`.
- Lösung: 11 Zellen, 7 Kurven; Dichte 11/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=87, maxDeductionDepth=6.
- Einstieg: Die drei rechten Nullspalten und zwei unteren Nullzeilen begrenzen die Anfangsfläche. Belegschritte: `[28, 35, 47, 48, 49, 50, 51]`.
- Eine tatsächliche maximale Prämissenkette: `[3, 43, 62, 69, 70, 78]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Nordport verbraucht Count 1 in Spalte 3; die Einserzeilen 1/2 lenken den Arm nach links. Der untere kurze Bogen um W/3 ist als deutliche Anfangsentscheidung zugänglich, kein behaupteter voller 7×7-Komplexitätsabruf.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .┌─┘...
01 .│.....
02 .│.....
03 ┐└┐....
04 └─┘....
05 .......
06 .......
```

## S1-04-02-02

- Input: 7×7; A W/0, B E/0. Zeilen `[5, 4, 4, 4, 4, 5, 3]`, Spalten `[7, 1, 7, 1, 6, 1, 6]`.
- Lösung: 29 Zellen, 8 Kurven; Dichte 29/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=110, maxDeductionDepth=7.
- Einstieg: Die vollen Spalten 0 und 2 liefern zwei deutliche Anfangsanker. Belegschritte: `[24, 25, 26, 27, 28, 29]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 38, 43, 61, 62, 82, 93]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'COMBINATION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Beide Endpunkte sitzen an der oberen Zeile. Die rechten Arme in Spalten 4/6 kehren gemeinsam in Zeile 5 zurück, die linken Vollarme in Spalten 0/2 erst in Zeile 6; Counts 6 statt 7 bestimmen diese unterschiedliche Höhe der Armpaare. Das Langarmmotiv dient einem klaren Start mit asymmetrischen Resten; die Nähe zum späteren S1-04-03-02 bleibt im Paarbericht sichtbar.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.┌─┐.┌
01 │.│.│.│
02 │.│.│.│
03 │.│.│.│
04 │.│.│.│
05 │.│.└─┘
06 └─┘....
```

## S1-04-02-03

- Input: 7×7; A N/0, B S/1. Zeilen `[5, 1, 4, 1, 6, 1, 6]`, Spalten `[1, 5, 4, 4, 5, 2, 3]`.
- Lösung: 24 Zellen, 8 Kurven; Dichte 24/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=105, maxDeductionDepth=13.
- Einstieg: A an N/0 verbraucht Count 1 in Spalte 0. Belegschritte: `[0, 18]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 27, 42, 49, 55, 57, 59, 74, 82, 87, 90, 92, 96]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Keine Linie ist öffentlich Null oder voll; Einserzeilen legen nur die Übergänge zwischen verschieden langen Querbändern fest. Die obere und untere Breite 5/6 unterscheiden sich, sodass mittlere Spaltenreste statt Wiedererkennen einer festen Streifenbreite nötig sind.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └───┐..
01 ....│..
02 .┌──┘..
03 .│.....
04 .└────┐
05 ......│
06 .┌────┘
```

## S1-04-02-04

- Input: 7×7; A E/1, B S/3. Zeilen `[3, 5, 6, 3, 1, 2, 1]`, Spalten `[4, 2, 6, 4, 2, 1, 2]`.
- Lösung: 21 Zellen, 13 Kurven; Dichte 21/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=116, maxDeductionDepth=17.
- Einstieg: B an S/3 belegt allein die unterste Zeile. Belegschritte: `[8, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[20, 26, 42, 43, 50, 55, 59, 81, 101, 102, 104, 107, 108, 111, 112, 113, 115]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'EXCLUSION']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Einsercount in Spalte 5 begrenzt den Querarm zum Ostport, während der Sechsercount in Spalte 2 den linken Bogen und unteren Schenkel verbindet. Der große untere Leerbereich kontrastiert den verzahnten Kopf; ohne Voll-/Nullzahl beginnt die Klärung an Ports und Restzahlen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┐....
01 │.│┌┐.┌
02 │.└┘└─┘
03 └─┐....
04 ..│....
05 ..└┐...
06 ...│...
```

## S1-04-02-05

- Input: 7×7; A W/0, B E/6. Zeilen `[5, 3, 1, 1, 1, 2, 2]`, Spalten `[1, 1, 1, 1, 2, 3, 6]`.
- Lösung: 15 Zellen, 6 Kurven; Dichte 15/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=117, maxDeductionDepth=18.
- Einstieg: A an W/0 verbraucht Count 1 in Spalte 0. Belegschritte: `[0, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 24, 30, 31, 38, 44, 45, 52, 58, 59, 66, 72, 73, 85, 95, 96, 105, 111]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Vier Einsercounts erzwingen den langen oberen Querarm; der rechte Sechsercount trägt danach die vertikale Strecke mit unterem Knick. Das dünne Muster ist räumlich klar, aber keine Zahl fixiert allein die Form; behalten als mittlere Hinweise mit verschiedener Restzahlbedeutung.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ────┐..
01 ....└─┐
02 ......│
03 ......│
04 ......│
05 .....┌┘
06 .....└─
```

## S1-04-02-06

- Input: 7×7; A E/3, B S/1. Zeilen `[5, 2, 3, 6, 2, 2, 1]`, Spalten `[6, 5, 1, 3, 4, 1, 1]`.
- Lösung: 21 Zellen, 11 Kurven; Dichte 21/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=120, maxDeductionDepth=19.
- Einstieg: A an E/3 verbraucht Count 1 in Spalte 6. Belegschritte: `[12, 18]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 38, 46, 55, 64, 76, 79, 83, 84, 86, 101, 105, 106, 108, 109, 111, 113, 115, 116]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der obere breite Bogen, der mittige Ostarm und die linke untere Wechselkurve werden über zwei fast volle Spalten verbunden. Count 1 in Spalte 2 lässt nur einen Querübergang zu; das früh erkennbare Feld verlangt danach Kombination statt eines zweiten Extremankers.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌───┐..
01 │...│..
02 │..┌┘..
03 └┐.└───
04 ┌┘.....
05 └┐.....
06 .│.....
```

## S1-04-02-07

- Input: 7×7; A S/0, B N/6. Zeilen `[2, 4, 6, 2, 3, 1, 1]`, Spalten `[3, 4, 4, 1, 1, 3, 3]`.
- Lösung: 19 Zellen, 12 Kurven; Dichte 19/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=117, maxDeductionDepth=20.
- Einstieg: A an S/0 belegt die Einserzeile unten. Belegschritte: `[6, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 24, 36, 37, 38, 50, 51, 54, 64, 69, 91, 92, 94, 98, 100, 102, 105, 110, 112, 113]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'EXCLUSION']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Nordport rechts und ein linker mittiger Doppelbogen liegen auf beiden Seiten zweier Einserspalten. Die Sechserzeile 2 ist fast voll, doch ihre Lücke hängt von den übrigen Spaltenresten ab; lokale Belegung eröffnet hier eine entfernte obere Rückkehr.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .....┌┘
01 .┌┐..└┐
02 .│└───┘
03 .└┐....
04 ┌─┘....
05 │......
06 │......
```

## S1-04-02-08

- Input: 7×7; A N/4, B W/1. Zeilen `[1, 4, 4, 4, 4, 3, 2]`, Spalten `[1, 1, 4, 1, 6, 4, 5]`.
- Lösung: 22 Zellen, 11 Kurven; Dichte 22/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=122, maxDeductionDepth=22.
- Einstieg: A an N/4 belegt allein die obere Zeile. Belegschritte: `[4, 7]`.
- Eine tatsächliche maximale Prämissenkette: `[4, 24, 36, 37, 48, 53, 54, 62, 67, 77, 82, 93, 94, 95, 97, 103, 104, 113, 114, 116, 119, 120]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 9 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Westport verbraucht einen der linken Einsercounts, während der lange rechte Umweg um ein freies Innengebiet führt. Gleich große Viererzeilen bedeuten verschiedene Zellen; Restzahlen und Gegenports müssen zusammen gelesen werden, eine direkte Mittelverbindung erfüllt sie nicht.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....│..
01 ──┐.│..
02 ..│.│┌┐
03 ..│.└┘│
04 ..└─┐.│
05 ....└┐│
06 .....└┘
```

## S1-04-02-09

- Input: 7×7; A S/6, B S/0. Zeilen `[0, 0, 0, 0, 2, 7, 6]`, Spalten `[2, 2, 2, 2, 2, 2, 3]`.
- Lösung: 15 Zellen, 12 Kurven; Dichte 15/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=96, maxDeductionDepth=12.
- Einstieg: Die volle Zeile 5 begrenzt mit den vier oberen Nullzeilen einen unteren Kurvenbereich. Belegschritte: `[52, 53, 54, 55, 56, 57, 58]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 59, 67, 70, 71, 83, 84, 88, 89, 92, 93, 94]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Beide Südports liegen weit auseinander; rechts reicht ein kleiner Kopf über das Vollband hinaus, links nicht. Die starke Dichtekonzentration im Boden ist die gewollte räumliche Variation, eine geringe aktive Höhe hier kein allgemeiner Qualitätsfehler.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .......
01 .......
02 .......
03 .......
04 .....┌┐
05 ┌┐┌┐┌┘│
06 │└┘└┘.│
```

## S1-04-02-10

- Input: 7×7; A S/2, B N/2. Zeilen `[1, 2, 1, 2, 1, 3, 3]`, Spalten `[0, 0, 4, 5, 4, 0, 0]`.
- Lösung: 13 Zellen, 8 Kurven; Dichte 13/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=101, maxDeductionDepth=13.
- Einstieg: B an N/2 belegt die Einserzeile oben. Belegschritte: `[2, 19]`.
- Eine tatsächliche maximale Prämissenkette: `[2, 25, 54, 55, 75, 81, 84, 85, 86, 87, 89, 93, 94]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['DENSITY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Vier Nullspalten lassen einen Dreiarmbereich über die ganze Höhe übrig; Nord und Süd liegen in derselben Spalte, aber die mittlere Fünferspalte erzwingt den rechten Umweg. Gegenüber dem horizontalen Bodenbild in 9 wirkt die Dichte nun vertikal verteilt und die Portentfernung groß.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ..│....
01 ..└┐...
02 ...│...
03 ...└┐..
04 ....│..
05 ..┌┐│..
06 ..│└┘..
```

## S1-04-02-11

- Input: 7×7; A S/2, B N/0. Zeilen `[1, 1, 5, 6, 5, 6, 7]`, Spalten `[6, 5, 5, 4, 3, 3, 5]`.
- Lösung: 31 Zellen, 22 Kurven; Dichte 31/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=122, maxDeductionDepth=24.
- Einstieg: B an N/0 belegt die oberen Einserzeilen zunächst am linken Rand. Belegschritte: `[0, 19]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 24, 36, 37, 48, 65, 66, 76, 81, 85, 87, 91, 95, 97, 98, 99, 103, 104, 105, 106, 107, 112, 116, 120]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'DENSITY']`; erwartete gedankliche Folge 9 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die untere Vollzeile und mehrere Fünfer-/Sechserreihen bieten breite Sicherheiten, die 22 Kurven bleiben trotzdem aufzulösen. Zwei fast volle linke Spalten verknüpfen verschachtelte Bögen mit dem Südport innen; dieser dichte Höhepunkt erklärt die Mehrlast sachlich.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 │......
01 │......
02 └─┐..┌┐
03 .┌┘┌─┘│
04 ┌┘┌┘..│
05 │┌┘┌┐.│
06 └┘┌┘└─┘
```

## S1-04-02-12

- Input: 7×7; A W/0, B S/6. Zeilen `[1, 2, 2, 1, 3, 3, 3]`, Spalten `[2, 2, 3, 1, 3, 2, 2]`.
- Lösung: 15 Zellen, 11 Kurven; Dichte 15/49. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=119, maxDeductionDepth=26.
- Einstieg: A an W/0 belegt die obere Einserzeile allein. Belegschritte: `[0, 23]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 24, 30, 31, 38, 43, 44, 50, 55, 56, 62, 66, 67, 72, 76, 77, 82, 89, 90, 95, 97, 98, 101, 109, 110, 117]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 10 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Alle Achsen sind aktiv und enthalten nur kleine Zahlen; die diagonale Treppe muss unten noch einen rechten Doppelknick aufnehmen. Gegenüber 11 ist die Orientierung luftiger, die Rootfolge aber länger (26 statt 24); die zwei Höhepunkte belasten unterschiedliche Aspekte.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐......
01 └┐.....
02 .└┐....
03 ..│....
04 ..└─┐..
05 ....│┌┐
06 ....└┘│
```
