# Author-Leveldesignreview: S1-05-01 (globale Route 17)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Zugängliche Extremwerte | S1-05-01-01: 7, S1-05-01-02: 7 |
| 3–5 | Entfernte Belegungsbereiche | S1-05-01-03: 10, S1-05-01-04: 11, S1-05-01-05: 10 |
| 6–8 | Ausschluss über das Raster | S1-05-01-06: 13, S1-05-01-07: 13, S1-05-01-08: 15 |
| 9–10 | Lange Pfade und Kurven | S1-05-01-09: 12, S1-05-01-10: 14 |
| 11–12 | Faire große Abschlüsse | S1-05-01-11: 32, S1-05-01-12: 30 |

**Individuelle Routenbewertung:** Die extremen Wiedereinstiege sind ausdrücklich vorgesehen. Position 3 verwendet ein nahes Langlaufmotiv, verändert aber die Bandbreiten und die Restzahlaufgabe gegenüber 2; 4/5 verbinden zwei räumlich getrennte sichere Gebiete über andere Queranker. 6–8 übertragen lokale Einser-/Nullausschlüsse bis zu entfernten Bögen. 9/10 erlauben schmalere aktive Fläche für lange Pfade und späte Kurven. Die beiden 8×8-Abschlüsse verlagern Formlast in Kopf bzw. Boden und bleiben auf hohe serielle Schlusslast im Spieltest zu prüfen.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-05-01-01

- Input: 8×8; A E/7, B S/0. Zeilen `[0, 0, 0, 0, 0, 0, 5, 7]`, Spalten `[2, 2, 1, 1, 2, 1, 2, 1]`.
- Lösung: 12 Zellen, 7 Kurven; Dichte 12/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=110, maxDeductionDepth=7.
- Einstieg: Sechs obere Nullzeilen begrenzen einen sehr klaren Bodenbereich. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 76, 91, 92, 96, 97, 107]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die Einsercounts in Spalten 2/3/5/7 verteilen lange Querarme und zwei kurze Knickpaare. Als zugänglicher großer Wiedereinstieg geeignet; die optische 8×8-Größe wird bewusst durch die extreme Leere entlastet.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ........
01 ........
02 ........
03 ........
04 ........
05 ........
06 ┌┐..┌─┐.
07 │└──┘.└─
```

## S1-05-01-02

- Input: 8×8; A W/0, B E/0. Zeilen `[6, 4, 4, 4, 4, 5, 2, 3]`, Spalten `[8, 1, 8, 1, 6, 1, 6, 1]`.
- Lösung: 32 Zellen, 8 Kurven; Dichte 32/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=139, maxDeductionDepth=7.
- Einstieg: Die vollen Spalten 0 und 2 geben sichere lange Anfangsarme. Belegschritte: `[28, 29, 30, 31, 32, 33, 34]`.
- Eine tatsächliche maximale Prämissenkette: `[20, 58, 70, 88, 89, 111, 122]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'COMBINATION']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die beiden rechten Sechserarme müssen eine Zeile früher zurückkehren als der linke Vollarm. Bekanntes Muster mit asymmetrischen Resten bleibt als Wiedereinstieg; die nachfolgende ähnliche Querbandbelegung verwendet weniger direkte Vollanker und neue Bandbreiten, weshalb ihre höhere Schlusslast gesondert begründbar ist.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐.┌─┐.┌─
01 │.│.│.│.
02 │.│.│.│.
03 │.│.│.│.
04 │.│.│.│.
05 │.│.└─┘.
06 │.│.....
07 └─┘.....
```

## S1-05-01-03

- Input: 8×8; A N/0, B S/0. Zeilen `[8, 1, 7, 1, 5, 1, 6, 1]`, Spalten `[3, 5, 4, 4, 4, 5, 2, 3]`.
- Lösung: 30 Zellen, 8 Kurven; Dichte 30/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=140, maxDeductionDepth=10.
- Einstieg: Die obere Vollzeile verbindet den Nordport mit einem entfernten rechten Arm. Belegschritte: `[28, 29, 30, 31, 32, 33, 34]`.
- Eine tatsächliche maximale Prämissenkette: `[20, 35, 51, 52, 60, 89, 91, 111, 120, 130]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die Bänder darunter haben unterschiedliche Längen 7/5/6; Einserzeilen legen nur ihre Übergänge fest. Obwohl die Maske nahe bei Position 2 liegt, wirken hier die mittleren Spaltenreste auf mehrere unterschiedlich weit reichende Bereiche; behalten als gezielte Einführung entfernter Belegungsgebiete.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 └──────┐
01 .......│
02 .┌─────┘
03 .│......
04 .└───┐..
05 .....│..
06 ┌────┘..
07 │.......
```

## S1-05-01-04

- Input: 8×8; A E/0, B S/2. Zeilen `[1, 1, 3, 2, 2, 6, 1, 1]`, Spalten `[0, 0, 3, 1, 1, 3, 4, 5]`.
- Lösung: 17 Zellen, 9 Kurven; Dichte 17/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=131, maxDeductionDepth=11.
- Einstieg: A an E/0 belegt die obere Einserzeile allein. Belegschritte: `[7, 22]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 34, 54, 60, 77, 101, 104, 119, 121, 128, 129]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'EXCLUSION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der rechte Eingang beginnt weit vom unteren Südport links; zwei linke Nullspalten begrenzen den Bodenarm. Der Sechserquerweg in Zeile 5 verbindet zwei getrennte Kurvenfolgen und kann erst mit den mittleren Dreier-/Vierercounts festgelegt werden.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .......┌
01 .......│
02 .....┌─┘
03 .....└┐.
04 ......└┐
05 ..┌────┘
06 ..│.....
07 ..│.....
```

## S1-05-01-05

- Input: 8×8; A N/6, B W/5. Zeilen `[6, 8, 1, 1, 1, 1, 0, 0]`, Spalten `[6, 2, 1, 1, 2, 2, 2, 2]`.
- Lösung: 18 Zellen, 11 Kurven; Dichte 18/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=125, maxDeductionDepth=10.
- Einstieg: Die volle Zeile 1 liefert sichere Belegung quer über das Raster. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[16, 42, 93, 97, 101, 102, 103, 105, 106, 108]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die unteren Einserzeilen zwingen einen linken Schenkel, während der Nordport rechts eine obere Doppelkurve einbindet. Die klar getrennten Kopf- und linke Endgebiete werden über vier Zweiercounts gekoppelt; weniger räumliche Verzahnung als 4, aber ein nachvollziehbarer zweiter Ankertyp.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌┐..┌┐└┐
01 │└──┘└─┘
02 │.......
03 │.......
04 │.......
05 ┘.......
06 ........
07 ........
```

## S1-05-01-06

- Input: 8×8; A S/4, B N/1. Zeilen `[1, 5, 5, 1, 1, 1, 4, 5]`, Spalten `[0, 2, 1, 4, 3, 3, 3, 7]`.
- Lösung: 23 Zellen, 12 Kurven; Dichte 23/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=138, maxDeductionDepth=13.
- Einstieg: B an N/1 belegt allein die obere Zeile. Belegschritte: `[1, 24]`.
- Eine tatsächliche maximale Prämissenkette: `[1, 28, 49, 50, 85, 92, 93, 116, 131, 132, 133, 134, 137]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'CHAIN']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei mittlere Einserzeilen lenken die Strecke nach rechts, der Südport liegt im unteren Quergebiet. Die Einzerspalte 2 koppelt obere und untere Belegungsgruppe nur an einer Stelle; Ausschlüsse aus dem Nordbereich wirken dadurch bis zur unteren Doppelkurve.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .│......
01 .└─┐..┌┐
02 ...└──┘│
03 .......│
04 .......│
05 .......│
06 ...┌─┐.│
07 ...└┐└─┘
```

## S1-05-01-07

- Input: 8×8; A E/3, B S/1. Zeilen `[0, 0, 2, 8, 3, 2, 1, 1]`, Spalten `[4, 5, 1, 1, 1, 2, 2, 1]`.
- Lösung: 17 Zellen, 9 Kurven; Dichte 17/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=128, maxDeductionDepth=13.
- Einstieg: Die volle Zeile 3 und zwei obere Nullzeilen geben eine sichere Querfläche. Belegschritte: `[44, 45, 46, 47, 48, 49, 50]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 51, 93, 96, 97, 100, 103, 105, 112, 113, 114, 118, 126]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'COMBINATION']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Vier Einsercounts im Zentrum begrenzen deren Formen; der linke Kopfknick und der rechte kurze Umweg liegen auseinander. Die untersten Einserzeilen schließen dagegen den linken Südstrang fest ein; Folgen aus drei Teilgebieten treffen im Vollband zusammen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ........
01 ........
02 ┌┐......
03 │└───┐┌─
04 │....└┘.
05 └┐......
06 .│......
07 .│......
```

## S1-05-01-08

- Input: 8×8; A N/2, B S/0. Zeilen `[3, 2, 2, 2, 2, 5, 2, 6]`, Spalten `[7, 7, 3, 2, 3, 2, 0, 0]`.
- Lösung: 24 Zellen, 16 Kurven; Dichte 24/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=137, maxDeductionDepth=15.
- Einstieg: Die beiden rechten Nullspalten begrenzen die lange linke Kurvenfolge. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[26, 51, 57, 64, 71, 72, 78, 82, 106, 125, 129, 130, 131, 133, 135]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'CHAIN']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die zwei linken Siebenercounts verlangen fast durchgehende Belegung, erst die Fünfer-/Sechserreihen unten erlauben Querarme nach rechts. Nord und Süd liegen nahe, aber die hohen entfernten Spaltenwerte verhindern die Abkürzung; der Ausschlussbereich beeinflusst die gesamte Formfolge.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─┘.....
01 └┐......
02 ┌┘......
03 └┐......
04 ┌┘......
05 └───┐...
06 ....└┐..
07 ┌────┘..
```

## S1-05-01-09

- Input: 8×8; A W/0, B S/1. Zeilen `[5, 4, 1, 3, 3, 1, 1, 1]`, Spalten `[1, 7, 4, 5, 2, 0, 0, 0]`.
- Lösung: 19 Zellen, 11 Kurven; Dichte 19/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=135, maxDeductionDepth=12.
- Einstieg: B an S/1 belegt die Einserzeile unten. Belegschritte: `[0, 21]`.
- Eine tatsächliche maximale Prämissenkette: `[21, 28, 79, 80, 88, 102, 104, 113, 116, 126, 127, 128]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'CHAIN']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der dichte obere Doppelbogen und der lange untere Schenkel sind über einen mittleren Rückknick verbunden. Die drei rechten Nullspalten machen den Umriss ruhig, 11 Kurven bleiben dennoch erst aus Restzahlen festzulegen; lange Verbindung bei geringer aktiver Breite bewusst als Variation behalten.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ─┐┌─┐...
01 .└┘┌┘...
02 ...│....
03 .┌┐│....
04 .│└┘....
05 .│......
06 .│......
07 .│......
```

## S1-05-01-10

- Input: 8×8; A N/7, B E/5. Zeilen `[5, 3, 1, 4, 4, 3, 0, 0]`, Spalten `[0, 0, 0, 5, 4, 5, 4, 2]`.
- Lösung: 20 Zellen, 13 Kurven; Dichte 20/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=134, maxDeductionDepth=14.
- Einstieg: Die beiden unteren Nullzeilen grenzen ein Kopf- und Mittelgebiet ein. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 62, 67, 81, 94, 105, 108, 114, 116, 118, 126, 127, 129, 130]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'DENSITY']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Nordport ganz rechts verbraucht einen der zwei Counts in Spalte 7; links begrenzen drei Nullspalten den langen Gegenschenkel. Die kleine rechte Rückkurve liegt an einem anderen Teilraum als der obere Bogen und wird aus mittleren Vierer-/Fünfercounts spät geometrisch bestimmt.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ...┌┐┌─┘
01 ...│└┘..
02 ...│....
03 ...│┌─┐.
04 ...└┘┌┘.
05 .....└──
06 ........
07 ........
```

## S1-05-01-11

- Input: 8×8; A S/7, B W/0. Zeilen `[4, 4, 3, 2, 1, 1, 1, 1]`, Spalten `[2, 2, 1, 2, 2, 1, 2, 5]`.
- Lösung: 17 Zellen, 11 Kurven; Dichte 17/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=152, maxDeductionDepth=32.
- Einstieg: A an S/7 belegt die unterste Einserzeile allein. Belegschritte: `[0, 27]`.
- Eine tatsächliche maximale Prämissenkette: `[27, 34, 42, 43, 50, 58, 59, 66, 74, 75, 82, 90, 91, 94, 98, 99, 105, 112, 113, 115, 118, 119, 121, 124, 125, 126, 130, 140, 141, 143, 144, 148]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 10 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Vier untere Einserzeilen erzeugen den langen rechten Schenkel, die oberen vier Reihen dagegen elf unterschiedliche Kurven. Der kleine linke Anfangsbogen ist über einen diagonalen Querzug mit dem Schenkel verbunden; die lange technische Folge 32 betrifft vor allem diese obere Formlast.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┐┌─┐....
01 └┘.└┐...
02 ....└─┐.
03 ......└┐
04 .......│
05 .......│
06 .......│
07 .......│
```

## S1-05-01-12

- Input: 8×8; A E/0, B W/5. Zeilen `[1, 3, 1, 1, 1, 2, 5, 5]`, Spalten `[3, 1, 2, 2, 2, 6, 1, 2]`.
- Lösung: 19 Zellen, 12 Kurven; Dichte 19/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=151, maxDeductionDepth=30.
- Einstieg: A an E/0 belegt die Einserzeile oben und begrenzt den rechten Kopf. Belegschritte: `[7, 16]`.
- Eine tatsächliche maximale Prämissenkette: `[7, 34, 35, 43, 44, 50, 57, 58, 64, 71, 76, 77, 83, 88, 89, 95, 100, 101, 107, 112, 115, 121, 125, 129, 143, 144, 146, 147, 148, 150]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 10 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei weitere Einserzeilen machen den langen mittleren Arm sicher, unten tragen zwei Fünferreihen verschiedene Doppelknicke. Der Westport innen liegt am anderen Ende dieses Bodenbereichs; die dünne vertikale und kurvenreiche untere Folge kontrastiert den oberen Abschlusstyp 11.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .......┌
01 .....┌─┘
02 .....│..
03 .....│..
04 .....│..
05 ┐....│..
06 │.┌┐┌┘..
07 └─┘└┘...
```
