# Author-Leveldesignreview: S1-04-04 (globale Route 16)

Eigene fachliche Bewertung des Levelautors auf konkreten Inputs/Rootspuren. Keine unabhängige Abnahme oder menschlicher Spieltest.
Koordinaten und Linienindizes sind nullbasiert. Textgitter ist interne sachliche Prüfvorschau, keine Gestaltung. Legende: `.` leer, `│` NS, `─` EW, `└` NE, `┌` ES, `┐` SW, `┘` WN.

## Routenbogen

| Gruppe | Bestehender Schwerpunkt | Konkrete Positionen und Root-Tiefen |
|---|---|---|
| 1–2 | Klare Startanker | S1-04-04-01: 7, S1-04-04-02: 9 |
| 3–5 | Alternativen schrittweise ausschließen | S1-04-04-03: 10, S1-04-04-04: 11, S1-04-04-05: 12 |
| 6–8 | Hohe Verzahnung | S1-04-04-06: 12, S1-04-04-07: 13, S1-04-04-08: 14 |
| 9–10 | Knappe Counts und andere Endpunkte | S1-04-04-09: 11, S1-04-04-10: 15 |
| 11–12 | Strategischer Abschnittsabschluss | S1-04-04-11: 33, S1-04-04-12: 30 |

**Individuelle Routenbewertung:** Ein klarer Voll-/Nullstreifen in 1 bleibt, der wiederholte Serpentinenstart in 2 wurde für eigenständige Wiederaktivierung ersetzt. 3–5 schließen direkte oder voreilige Querwege durch ganz konkrete Null-, Einser- und Restcounts aus. 6–8 verzahnen Kopf, Mitte und Boden mit jeweils anderem Engpass. 9/10 variieren schwierige Seiteneintritte und knappe Counts bei stark anderer Kurvenlast. Die zwei strategischen Abschlüsse tragen dünne serielle bzw. breite flächige Last; ihre globale Nichtdirektheit lässt sich bereits aus Counts erklären, ohne dass eine bestimmte globale Solverregel zwingend auftreten muss.

Alle nachfolgenden Urteile und Einstiege sind wörtlich aus dem an öffentliche Inputs und Lösungen gebundenen Autorensatz übernommen. Erwartete gedankliche Folge und tatsächliche Root-Mikroschritttiefe sind unterschiedliche Zählgranularitäten. Keine unabhängige Abnahme oder menschlicher Spieltest.

## S1-04-04-01

- Input: 8×8; A W/0, B E/0. Zeilen `[8, 6, 0, 0, 0, 0, 0, 0]`, Spalten `[1, 2, 2, 1, 2, 2, 2, 2]`.
- Lösung: 14 Zellen, 12 Kurven; Dichte 14/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=109, maxDeductionDepth=7.
- Einstieg: Die volle obere Zeile und die sechs unteren Nullzeilen ergeben einen sehr klaren Start. Belegschritte: `[28, 29, 30, 31, 32, 33]`.
- Eine tatsächliche maximale Prämissenkette: `[30, 85, 92, 93, 103, 106, 107]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'EXCLUSION']`; erwartete gedankliche Folge 4 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Zwei Einsercounts markieren gerade Kopfstücke, die Zweiercounts die kleinen Rückknicke. Als einzelne ruhige Aktivierung im 8×8-Feld behalten; der nachfolgende breite Einstieg muss dann eine andere Geometrie tragen, um kein allgemeines Streifenpaar zu wiederholen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ─┐┌─┐┌┐┌
01 .└┘.└┘└┘
02 ........
03 ........
04 ........
05 ........
06 ........
07 ........
```

## S1-04-04-02

- Input: 8×8; A W/0, B S/7. Zeilen `[8, 4, 1, 2, 2, 1, 1, 2]`, Spalten `[1, 1, 2, 2, 1, 1, 8, 5]`.
- Lösung: 21 Zellen, 13 Kurven; Dichte 21/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=134, maxDeductionDepth=9.
- Einstieg: Die volle obere Zeile kreuzt die volle Spalte 6. Belegschritte: `[28, 29, 30, 31, 32, 33, 34]`.
- Eine tatsächliche maximale Prämissenkette: `[32, 56, 74, 75, 78, 79, 83, 124, 125]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['OCCUPANCY', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 5 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Gezielt ersetzt durch einen breiten Kopfarm mit einem kleinen Rückknick und zwei rechts getrennte Kurvengebiete. W/0 verbraucht Count 1 in Spalte 0, S/7 erreicht dagegen den unteren rechten Knick; die vielen Einsercounts begrenzen schnell die Raumalternativen. Ein klarer Start ohne den wiederholten Vierarmrhythmus der vorherigen Positions-2-Felder.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ──┐┌───┐
01 ..└┘..┌┘
02 ......│.
03 ......└┐
04 ......┌┘
05 ......│.
06 ......│.
07 ......└┐
```

## S1-04-04-03

- Input: 8×8; A N/1, B S/1. Zeilen `[7, 1, 7, 1, 7, 1, 7, 1]`, Spalten `[0, 6, 4, 4, 4, 4, 4, 6]`.
- Lösung: 32 Zellen, 8 Kurven; Dichte 32/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=133, maxDeductionDepth=10.
- Einstieg: Die linke Nullspalte macht die Siebenerreihen innerhalb der aktiven Breite voll. Belegschritte: `[35, 36, 37, 38, 39, 40, 41]`.
- Eine tatsächliche maximale Prämissenkette: `[0, 57, 83, 98, 126, 127, 128, 129, 130, 131]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'COMBINATION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die vier langen Querbänder brauchen abwechselnd nur einen Einserübergang. Der gleiche Nord-/Südport in Spalte 1 könnte einen direkten Weg nahelegen, wird aber durch mittlere Spaltenwerte ausgeschlossen; ein konkreter Ausschluss verschiedener Verbindungen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 .└─────┐
01 .......│
02 .┌─────┘
03 .│......
04 .└─────┐
05 .......│
06 .┌─────┘
07 .│......
```

## S1-04-04-04

- Input: 8×8; A E/2, B W/7. Zeilen `[0, 0, 1, 1, 2, 1, 6, 8]`, Spalten `[2, 2, 1, 1, 2, 2, 4, 5]`.
- Lösung: 19 Zellen, 14 Kurven; Dichte 19/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=124, maxDeductionDepth=11.
- Einstieg: Die volle Unterzeile und die zwei oberen Nullzeilen begrenzen den Verbindungskorridor. Belegschritte: `[51, 52, 53, 54, 55, 56, 57]`.
- Eine tatsächliche maximale Prämissenkette: `[52, 61, 92, 100, 112, 116, 117, 118, 120, 121, 123]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'CHAIN']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die Einserzeilen 2/3/5 legen einen versetzten rechten Schenkel fest; die Sechserreihe unten verteilt dann zwei getrennte Knickpaare. Die Formalternativen des Bodenwegs werden schrittweise durch niedrige Spaltencounts und den Westport eliminiert.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ........
01 ........
02 .......┌
03 .......│
04 ......┌┘
05 ......│.
06 ┌┐..┌┐└┐
07 ┘└──┘└─┘
```

## S1-04-04-05

- Input: 8×8; A W/0, B E/5. Zeilen `[2, 1, 1, 1, 2, 7, 2, 3]`, Spalten `[1, 8, 3, 3, 1, 1, 1, 1]`.
- Lösung: 19 Zellen, 8 Kurven; Dichte 19/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=132, maxDeductionDepth=12.
- Einstieg: Spalte 1 ist über alle acht Zeilen belegt. Belegschritte: `[35, 36, 37, 38, 39, 40, 41, 42]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 47, 62, 68, 102, 116, 118, 119, 121, 128, 129, 130]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['EXCLUSION', 'COMBINATION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei mittlere Einserzeilen sichern den linken Schenkel; fünf Einserspalten bestimmen die lange untere Ostzufahrt. Die beiden Dreiercounts im unteren Innenbogen müssen mit dem Vollanker zusammenpassen und verhindern eine direkte Querabkürzung.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ─┐......
01 .│......
02 .│......
03 .│......
04 .└┐.....
05 .┌┘┌────
06 .│.│....
07 .└─┘....
```

## S1-04-04-06

- Input: 8×8; A E/5, B W/2. Zeilen `[7, 2, 2, 3, 1, 4, 2, 4]`, Spalten `[3, 1, 1, 4, 5, 3, 7, 1]`.
- Lösung: 25 Zellen, 10 Kurven; Dichte 25/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=138, maxDeductionDepth=12.
- Einstieg: A an E/5 verbraucht Count 1 in Spalte 7. Belegschritte: `[10, 17]`.
- Eine tatsächliche maximale Prämissenkette: `[17, 28, 45, 57, 82, 87, 99, 107, 119, 123, 125, 131]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 7 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Obere fast volle Belegung, linker Westarm und unterer U-Bogen werden über die mittleren Vierer-/Fünfercounts verbunden. Die Einserzeile 4 trennt diese Gebiete; hohe Verzahnung ist hier als Austausch von Restzahlen zwischen mindestens drei Bereichen nachvollziehbar.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌─────┐.
01 │.....│.
02 ┘.....│.
03 ....┌─┘.
04 ....│...
05 ...┌┘.┌─
06 ...│..│.
07 ...└──┘.
```

## S1-04-04-07

- Input: 8×8; A N/4, B S/2. Zeilen `[2, 1, 4, 5, 5, 1, 1, 3]`, Spalten `[0, 0, 1, 4, 7, 5, 3, 2]`.
- Lösung: 22 Zellen, 16 Kurven; Dichte 22/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=145, maxDeductionDepth=13.
- Einstieg: Die zwei linken Nullspalten begrenzen die mittlere Kurvenfläche. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[22, 47, 64, 72, 89, 94, 100, 125, 126, 135, 137, 138, 139]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'CHAIN']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Nordport sitzt über drei dichten Reihen, der Südport links unter zwei Einserzeilen. Die Siebenerspalte 4 trägt den langen Anschluss, andere Spalten verteilen mehrere obere Knicke; der kurze Bodenquerarm hängt an den Ausschlüssen aus dem Zentrum.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ....└┐..
01 .....│..
02 ...┌┐└┐.
03 ...│└┐└┐
04 ...└┐└─┘
05 ....│...
06 ....│...
07 ..┌─┘...
```

## S1-04-04-08

- Input: 8×8; A S/2, B E/2. Zeilen `[6, 3, 5, 4, 2, 1, 1, 3]`, Spalten `[8, 6, 2, 1, 1, 4, 2, 1]`.
- Lösung: 25 Zellen, 15 Kurven; Dichte 25/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=141, maxDeductionDepth=14.
- Einstieg: Die volle Spalte 0 liefert einen langen linken Anker. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[33, 56, 75, 87, 101, 103, 104, 105, 106, 126, 127, 128, 129, 134]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['COMBINATION', 'ENDPOINT_GEOMETRY']`; erwartete gedankliche Folge 8 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Nordbogen reicht bis Spalte 5, der Ostport liegt mittig rechts; Count 1 in Spalten 3/4/7 begrenzt deren Übergänge. Der untere Südport ist durch die linke Wechselkurve vom oberen Querbogen getrennt, also wirken die Teilgebiete gegenseitig über mehrere Counts.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌────┐..
01 └┐...│..
02 ┌┘...│┌─
03 └┐...└┘.
04 ┌┘......
05 │.......
06 │.......
07 └─┐.....
```

## S1-04-04-09

- Input: 8×8; A E/6, B W/3. Zeilen `[0, 0, 2, 5, 4, 2, 5, 7]`, Spalten `[1, 4, 6, 6, 4, 1, 1, 2]`.
- Lösung: 25 Zellen, 20 Kurven; Dichte 25/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=136, maxDeductionDepth=11.
- Einstieg: B an W/3 verbraucht Count 1 in Spalte 0. Belegschritte: `[12, 19]`.
- Eine tatsächliche maximale Prämissenkette: `[12, 48, 86, 94, 107, 123, 127, 128, 129, 131, 133]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'EXCLUSION']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Die zwei oberen Nullzeilen begrenzen eine kurvenreiche untere Fläche; Einsercounts rechts erlauben nur einen langen Bodenübergang. Die Ports auf versetzten Seiteneintritten erzwingen 20 Kurven trotz weniger als halber Belegung, eine sachlich andere Endpunktvariation.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ........
01 ........
02 ..┌┐....
03 ─┐│└┐...
04 .└┘┌┘...
05 ..┌┘....
06 .┌┘┌┐..┌
07 .└─┘└──┘
```

## S1-04-04-10

- Input: 8×8; A S/7, B W/4. Zeilen `[0, 2, 6, 4, 4, 1, 1, 1]`, Spalten `[1, 1, 4, 2, 2, 2, 1, 6]`.
- Lösung: 19 Zellen, 9 Kurven; Dichte 19/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=141, maxDeductionDepth=15.
- Einstieg: Die obere Nullzeile entfernt den Kopf; drei untere Einserzeilen legen einen rechten Schenkel fest. Belegschritte: `[28, 29, 30, 31, 32, 33, 34, 35]`.
- Eine tatsächliche maximale Prämissenkette: `[27, 42, 66, 67, 73, 88, 89, 96, 101, 106, 126, 133, 137, 138, 139]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['ENDPOINT_GEOMETRY', 'CHAIN']`; erwartete gedankliche Folge 6 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Westport links verbraucht Einsercounts in Spalten 0/1, die mittigen Zweiercounts bilden einen oberen Doppelknick. Die Verbindung zum Südport ganz rechts ist lang und klar asymmetrisch; geringere Kurvenlast als in 9 entlastet bewusst vor den Abschlüssen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ........
01 ..┌┐....
02 ..│└┐┌─┐
03 ..│.└┘.│
04 ──┘....│
05 .......│
06 .......│
07 .......│
```

## S1-04-04-11

- Input: 8×8; A W/7, B N/7. Zeilen `[2, 2, 3, 3, 4, 1, 1, 1]`, Spalten `[4, 1, 1, 2, 1, 2, 3, 3]`.
- Lösung: 17 Zellen, 11 Kurven; Dichte 17/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=152, maxDeductionDepth=33.
- Einstieg: A an W/7 belegt die Einserzeile unten allein. Belegschritte: `[7, 20]`.
- Eine tatsächliche maximale Prämissenkette: `[20, 28, 42, 43, 44, 58, 59, 60, 74, 75, 79, 84, 85, 89, 94, 95, 99, 104, 105, 106, 114, 115, 118, 122, 123, 126, 130, 131, 135, 139, 143, 145, 150]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'COMBINATION']`; erwartete gedankliche Folge 10 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Drei untere Einserzeilen begrenzen den linken Schenkel, mehrere Einserspalten den mittleren Querweg. Der rechte Kopf enthält dagegen drei kleine Bögen; die lange Schlussfolge wechselt hier zwischen weit auseinanderliegenden Verpflichtungen statt nur eine dichte Fläche zu lesen.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ......┌┘
01 ......└┐
02 .....┌─┘
03 ...┌─┘..
04 ┌──┘....
05 │.......
06 │.......
07 ┘.......
```

## S1-04-04-12

- Input: 8×8; A N/5, B W/6. Zeilen `[7, 4, 3, 5, 7, 2, 2, 2]`, Spalten `[8, 6, 4, 4, 1, 2, 2, 5]`.
- Lösung: 32 Zellen, 19 Kurven; Dichte 32/64. Der vollständige konkrete A→B-Pfad steht in der kanonischen Levelquelle.
- Nachweis: UNIQUE/1, searchNodes=0, requiredGuessDepth=0, deductionSteps=155, maxDeductionDepth=30.
- Einstieg: Die volle linke Spalte verbindet mehrere sichere Teilgebiete. Belegschritte: `[28, 29, 30, 31, 32, 33, 34]`.
- Eine tatsächliche maximale Prämissenkette: `[20, 37, 40, 41, 48, 67, 78, 83, 89, 97, 100, 104, 108, 109, 113, 118, 119, 127, 128, 132, 135, 136, 137, 139, 142, 143, 147, 150, 152, 154]` im zugehörigen Rootspurbericht.
- Individueller Fokus: `['CHAIN', 'DENSITY']`; erwartete gedankliche Folge 10 (eigene vorläufige Schätzung, nicht gemessen).
- Individuelle fachliche Entscheidung: Der Nordport innen und der Westport unten liegen auf unterschiedlichen Seiten zweier großer Querbögen. Count 1 in Spalte 4 bestimmt den einzigen mittleren Übergang, die unteren Zweierzeilen den kleinen Rückknick; 32 Zellen und 19 Kurven bilden den breiteren Gegenabschluss zu 11.
- Grenze: Lesbarkeit, erwartete menschliche Schlusskettentiefe und Lernwirkung noch nicht im Spieltest bestätigt. Nutzertexte und Referenzziele offen; Zeitklasse unbestimmt, Sternsekunden null.

```text
00 ┌──┐.└─┐
01 │.┌┘...│
02 │.│....│
03 └┐└┐...│
04 ┌┘.└───┘
05 └┐......
06 ┐│......
07 └┘......
```
