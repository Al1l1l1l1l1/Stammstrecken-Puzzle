# 03 – UI und Puzzlebausteine

**WP-035 · Teil C · Ausarbeitungsstand: Empfehlung zur ausdrücklichen Owner-/Produktfreigabe.**

> Dieses Kapitel konkretisiert ausschließlich die bereits bestätigte Puzzle- und Screenarchitektur als statische Designreferenz.
> Es ist **keine** Produktregel, Rechtsfreigabe, Runtime-Spezifikation, Levelnachweis oder Production-Importfreigabe.

## 1. Quellen, Rahmen und Referenzdateien

| Grundlage | Verwendung in diesem Kapitel |
|---|---|
| [Rätselkern und Interaktionsmodell](../../../Stammstrecken_Puzzle_Konzept_00-15/03_Raetselkern_und_Interaktionsmodell.md) | Semantik von sechs Formen, X, offener Belegungsnotiz, Leeren und Mehrfachauswahl. |
| [Level-Erlebnis und Qualitätsdifferenzierung](../../../Stammstrecken_Puzzle_Konzept_00-15/05_Level_Erlebnis_und_Qualitaetsdifferenzierung.md) | Konzentrationsphase sowie Reihenfolge aus Hervorhebung, A–B-Fahrt und Ergebnisfläche. |
| [UI- und Bedienungsspezifikation](../../../Stammstrecken_Puzzle_Konzept_00-15/12_UI_und_Bedienungsspezifikation.md) | Bestehende Screenarchitektur, Randzahlfeedback und Ausschlüsse im aktiven Rätsel. |
| [TECH_STACK](../../../ARCHITECTURE/TECH_STACK.md) | Dokumentarischer Anschluss an UI Toolkit/Unity ohne neue Architekturentscheidung. |
| [Owner-Kontext und Designbrief](evidence/OWNER_CONTEXT_AND_DESIGN_BRIEF.md) | Gemeinsame Tokens, Schriftpfade, sehr nahe aber eigenständige Bahn-App-Anmutung und Entwurfsgrenze. |

Die nachstehenden Werte sind **logische Konstruktionswerte**, keine Festlegung für Geräte-, Orientierungs-, Gesten- oder Accessibilitypolitik.

| Statische Quelle | Zweck | Einschränkung |
|---|---|---|
| [track-shapes.svg](references/track-shapes.svg) | Vektorursprung der sechs braunen Formen im 100×100-Zellkoordinatensystem. | Dokumentarische Referenz; kein Runtimeasset. |
| [03-puzzle.html](references/03-puzzle.html) | 4×4- und 10×10-Ansicht; Zelle, Werkzeug und Zählerzustände sind bewusst getrennt. | Kein JavaScript, keine versteckte Lösung, keine App. |
| [04-screens.html](references/04-screens.html) | Statische Ansichten der vorhandenen Start-, Karten-, Ergebnis-, Werk-, Profil- und Tagesbereiche. | Nur Layoutfixtures mit frei erfundenen Beispielwerten. |

Alle drei Quellen sind ohne Netzwerkanfrage statisch zu öffnen.
Die HTML-Ansichten laden ausschließlich die erwarteten, mitgelieferten lokalen Schriften `Fonts/IBMPlexSans-Regular.ttf` und `Fonts/IBMPlexSans-SemiBold.ttf`.
Sie verwenden keine Webfont-URL, kein Skript, keine Analytik und keine Produktnavigation.

## 2. Gemeinsames visuelles Vokabular

Diese Ausarbeitung verwendet ausschließlich das gemeinsame Token-System aus dem Designbrief.
Farbtonvarianten, Ersatzpaletten oder zusätzliche Markenfarben sind hier bewusst nicht eingeführt.

| Token | Exakter Wert | Einsatz in Teil C |
|---|---:|---|
| `brand-red` | `#C83D45` | Dominante Betriebslage, Primäraktion und A/B-Portfläche. |
| `surface` | `#F4F2EE` | Warme äußere Informations- und Kartenumgebung. |
| `paper` | `#FFFFFF` | Rasterzelle, Karte, Kontrollfläche und Ergebnisblatt. |
| `ink` | `#20272C` | Text, freigegebener Kartenstrang und neutrale Werkzeugzeichen. |
| `track-brown` | `#805535` | Ausschließlich eine konkrete der sechs Schienenformen. |
| `assumption-gray` | `#727B82` | Richtungsoffene, optionale Belegungsnotiz und exakt erfüllte Randzahl. |
| `empty-red` | `#B42E39` | Optionales rotes X und überschrittene Randzahl. |
| `selection-yellow` | `#F1C84B` | Ausschließlich aktive Werkzeug-/Feldauswahl. |
| `selection-edge` | `#6C5717` | Geometrisch erkennbare Kontur der Auswahl. |
| `grid-line` | `#CCD0D1` | 1-logische-Einheit-Rasterlinie und ruhige Trennung. |

Die statischen Ansichten verwenden **IBM Plex Sans** mit den gelieferten Schnitten Regular 400 und SemiBold 600.
Zahlen nutzen tabellarische Ziffern, wenn eine Zähl- oder Zeitreihe ausgerichtet werden muss.
Die Wortmarke in den Referenzen ist Satz aus SemiBold, keine behauptete finale Wort-/Bildmarke.
Die im Brief genannte IBM Plex Sans Condensed wird nicht durch eine andere Schrift substituiert und ist in diesen drei Quellen nicht eingebettet.

## 3. Semantische Trennung innerhalb einer Rasterzelle

Ein Feld erhält maximal **einen Feldinhalt** und optional eine davon unabhängige Auswahlhülle.
Diese Schichten sind nicht miteinander zu verschmelzen.

| Schicht | Konstruktionsempfehlung | Semantik / Ausschluss |
|---|---|---|
| Leer | `paper`-Fläche mit `grid-line`-Grenze, ohne Zentrumssymbol. | Unberührt oder vollständig geleert; kein implizites Nein. |
| Konkrete Schiene | Durchgehende `track-brown`-Mittellinie gemäß Abschnitt 4. | Eine bestimmte NS/EW/NE/ES/SW/WN-Form; nie Auswahl- oder Wahrheitsfarbe. |
| Graue Notiz | Zentrierter, kantenfreier Standardgleis-Glyph: zwei kurze parallele Striche mit drei Querschwellen innerhalb 0,36C×0,20C, jeweils 0,04C Strich in `assumption-gray`. | Belegung wird angenommen; **keine Richtung, kein Anschluss und keine konkrete Form**. |
| Rotes X | Zwei diagonale, gleich starke Striche in `empty-red`, um ein 0,56C×0,56C-Quadrat. | Optionale Leerannahme des Spielers; keine Systemwahrheit. |
| Auswahl | `selection-yellow`-Flächenüberzug mit 38 % Deckung über dem vorhandenen Inhalt; innenliegende 0,028C-Kontur in `selection-edge`. | Nur aktuelle Auswahl; überlagert, ersetzt oder färbt den Inhalt nicht um. |
| Mehrfachauswahl | Zusätzlich zur Auswahl ein 0,014C gestrichelter Innenrahmen, Abstand 0,10C zur Zellkante. | Kennzeichnet nur die Auswahlmenge, nicht ein neues Feldformat. |

`C` bezeichnet die sichtbare Seitenlänge einer Rasterzelle nach Skalierung.
Die graue Notiz berührt keine Zellkante und kann deshalb nicht als Richtungsport gelesen werden.
Sie ist weder eine graue Gerade noch eine graue Kurve und darf nie als teilweise konkrete Schiene gerendert werden.
Ein rotes X, die graue Notiz und eine braune Form sind austauschbare Feldinhalte; die Auswahl kann über jedem davon liegen.

## 4. Exakte Gleisgeometrie

### 4.1 Zellkoordinaten und Verbindungsregel

Die Vektorquelle arbeitet pro Zelle im normierten Quadrat `0…100 × 0…100`.
Für eine tatsächliche Zelle der Seitenlänge `C` wird jeder Vektorwert mit `C / 100` skaliert.

| Bezugspunkt | Normierte Koordinate | In einer Zelle der Größe C |
|---|---:|---:|
| Nord-Port `N` | `(50, 0)` | `(C/2, 0)` |
| Ost-Port `E` | `(100, 50)` | `(C, C/2)` |
| Süd-Port `S` | `(50, 100)` | `(C/2, C)` |
| West-Port `W` | `(0, 50)` | `(0, C/2)` |
| Schienenstrich | `18` Einheiten | `0,18C` breit |
| Kurvenradius | `50` Einheiten | `0,50C` |

> **Verbindungsregel:** Jede braune Mittellinie beginnt und endet exakt in einem Halbkantenzentrum. Zwei benachbarte Zellen verbinden sich deshalb nur über dieselbe gemeinsame Halbkantenzentrum-Koordinate.

Die Strichenden sind bündig (`butt`), die Kurvenübergänge rund (`round`).
Eine Kurve erhält keine zusätzliche Gerade vor oder nach dem Bogen.
Der Radius von `0,50C` liegt in der Ecke, die beide verbundenen Kanten teilen; der Bogen bleibt vollständig innerhalb des Feldes.

### 4.2 Die sechs und nur die sechs Formen

| ID | Anschlüsse | Mittellinienpfad in `viewBox="0 0 100 100"` | Kontrollpunkt / Ausschluss |
|---|---|---|---|
| `NS` | N ↔ S | `M 50 0 L 50 100` | senkrechte Gerade; keine Seitenports. |
| `EW` | E ↔ W | `M 0 50 L 100 50` | waagerechte Gerade; keine Nord-/Südports. |
| `NE` | N ↔ E | `M 50 0 A 50 50 0 0 0 100 50` | Bogen um die Nordost-Ecke. |
| `ES` | E ↔ S | `M 100 50 A 50 50 0 0 0 50 100` | Bogen um die Südost-Ecke. |
| `SW` | S ↔ W | `M 50 100 A 50 50 0 0 0 0 50` | Bogen um die Südwest-Ecke. |
| `WN` | W ↔ N | `M 0 50 A 50 50 0 0 0 50 0` | Bogen um die Nordwest-Ecke. |

Es existieren keine gedrehten oder gespiegelten zusätzlichen Varianten außerhalb dieser sechs IDs.
Ein Feld enthält höchstens eine dieser Mittellinien.
Kreuz, Weiche, T-Knoten, Brücke, Schleife, Endstück und Sonderfeld sind in dieser Konstruktion nicht definiert und dürfen nicht ergänzt werden.

## 5. A/B-Ports und Rasterrahmen

A und B bleiben Außenanschlüsse und werden nicht als siebte oder achte Gleisform behandelt.

| Baustein | Konstruktionsempfehlung | Bedeutung |
|---|---|---|
| Anschlusskoordinate | Exakt das Halbkantenzentrum der betroffenen Außenkante. | Gleiche Anschlusskoordinate wie die folgende braune Mittellinie. |
| Portscheibe | Kreis mit Durchmesser `0,35C`, `brand-red`-Füllung, `paper`-Buchstabe A oder B in SemiBold. | Orientierungspunkt, kein Inhalt der Zelle. |
| Platzierung | Mittelpunkt der Scheibe auf der Außen-Normalen, `0,17C` außerhalb der Anschlusskoordinate. | Die tangentiale Scheibe berührt den Rahmen, verdeckt aber keine konkrete Schienenform. |
| Rasterrahmen | Jede innere Zellgrenze `0,014C` in `grid-line`; keine schwere Außenbox über dem Rätsel. | Technische Arbeitsfläche statt dekorativer Karte. |
| Randzahlenraum | Oberhalb und links der Zellen je mindestens `0,35C` Freiraum; Zahl am jeweiligen Zeilen-/Spaltenzentrum. | Zahlen bleiben räumlich der ganzen Zeile oder Spalte zugeordnet. |

Die Ports in `03-puzzle.html` sind sichtbare Darstellungsfixtures; ihre Position belegt keine konkrete A/B-Lage eines Levels.

## 6. Randzahlen: nur objektive Mengenrückmeldung

Die Zahl selbst bleibt gleich groß und gleich schwer lesbar; nur ihre Farbe folgt dem erkennbaren Belegungszähler.

| Zustand | Farbe | Formregel | Aussage |
|---|---|---|---|
| Offen | `ink` | SemiBold, tabellarische Ziffer; keine Fläche und kein Symbol. | Geforderte Zahl noch nicht exakt erreicht. |
| Exakt | `assumption-gray` | Gleiche Größe, Gewicht und Position wie offen. | Sichtbare Belegungsmenge stimmt genau. |
| Überschritten | `empty-red` | Gleiche Größe, Gewicht und Position; keine Fehlermeldungsbox. | Sichtbare Belegungsmenge liegt objektiv über der Randzahl. |

„Exakt“ bedeutet nicht, dass die Geometrie, die A–B-Verbindung oder das gesamte Rätsel richtig ist.
„Überschritten“ darf eine nachvollziehbare Mengenabweichung zeigen, nicht aber eine plausible Planungsannahme bewerten.
Gelb ist für Randzahlen nicht vorgesehen, weil es keine Auswahl dieser Menge darstellt.

## 7. Werkzeugleiste und Kopfaktionen

Die Werkzeugleiste zeigt alle zehn direkten Feldwerkzeuge ohne Untermenü.
Die Referenz ordnet sie als nicht scrollende Zwei-Zeilen-Leiste mit fünf gleich breiten Feldern an; die Anordnung ist eine Layoutreferenz, keine neue Gesten- oder Orientierungsentscheidung.

| Position / Icon | Geometrie | Aktionssemantik |
|---|---|---|
| 1–6: NS, EW, NE, ES, SW, WN | Jeweils die identische 100er-Gleisgeometrie in einem `54×54`-logischen Feld, mit proportionaler Vorschau bei 0,69 Feldkante. | Setzt die gewählte konkrete braune Form. |
| 7: X | Zentrales rotes 0,55-Feld-× ohne Kreis, Schild oder Warnsymbol. | Setzt die optionale Leerannahme. |
| 8: Offen | Zentrierter kantenfreier grauer Notiz-Glyph, identisch zu Abschnitt 3. | Setzt eine richtungsoffene Belegungsannahme. |
| 9: Leeren | Neutrales `ink`-Radiererzeichen in einem 0,42-Feld-Rahmen. | Entfernt X, graue Notiz oder konkrete Schiene vollständig. |
| 10: Mehrfach | Zwei versetzte `selection-edge`-Quadratrahmen, je 0,42 Feldkante. | Kennzeichnet die Auswahlmenge vor gemeinsamer Anwendung. |

Ein aktives Werkzeug erhält `selection-yellow` als volle Feldfläche und eine `selection-edge`-Kontur von zwei logischen Einheiten.
Inaktive Werkzeuge haben `paper`-Fläche und `grid-line`-Rand von einer Einheit.
Die Formfarbe bleibt in einem aktiven Werkzeug braun; Gelb übernimmt nicht die Farbe einer Schiene.

Zurück und Hinweis bleiben eigenständige `30×30`-logische Kopfflächen mit `grid-line`-Umrandung.
Zurück zeigt einen einfachen linksgebundenen Winkel, Undo eine gegen den Uhrzeigersinn gebogene Einstrichlinie und Hinweis ein Fragezeichen innerhalb eines dünnen Kreises.
Diese drei Zeichen bleiben abstrakt-eigenständig und übernehmen keine fremden Verkehrspiktogramme.
Der Timer ist reiner tabellarischer Text in `assumption-gray`; er startet gemäß bestehender Regel erst mit der ersten Eingabe, ohne dass diese Quelle eine neue Zeitlogik festlegt.

## 8. Rätseloberfläche: Größe, Hierarchie und Ausschlüsse

| Bereich | Präzise Referenzregel |
|---|---|
| Kopf | Eine 57-logische-Einheiten-Zeile: Zurück, Abschnitt/Meldung, Störungsname, Undo, Hinweis und kleiner Timer. |
| Raster | Größte ruhige Fläche; erst Kopf, Randzahlen und Werkzeugleiste reservieren Raum, danach skaliert `C` einheitlich. |
| 4×4-Fixture | `C = 72` logische Einheiten, Randzahlen 16, A/B-Scheibe 25; absichtlich gemischte, nicht verbundene Zustände. |
| 10×10-Fixture | `C = 32` logische Einheiten, Randzahlen 11, A/B-Scheibe 12; einzelne Zustände, ausdrücklich keine Strecke. |
| Werkzeugfläche | `54` logische Einheiten pro Feld, 8 logische Einheiten Zwischenraum, dauerhaft sichtbar. |
| Auswahl | Zelle und Werkzeug dürfen beide gelb markiert sein, weil die eine Markierung Felder und die andere das aktive Werkzeug beschreibt. |

Im aktiven Rätsel erscheinen **keine** untere Hauptnavigation, Geduldspunkte, Geld, Werbung, Tagesbonuskarte oder Betriebswerk-Zugänge.
Die HTML-Referenz enthält deshalb nur Puzzle-Kopf, Raster, Zählerzustände und Werkzeugleiste.
Sie enthält keine Geste, Drag-Variante, Fehlertoleranz, Orientierungs- oder Accessibilityentscheidung.

## 9. Statusflächen und bestehende Screenbausteine

`04-screens.html` baut keine neue Informationsarchitektur auf, sondern zeigt die sechs vorhandenen Bereiche als separate Blätter.

| Bestehender Bereich | Statische Konstruktionsempfehlung | Nicht daraus abzuleiten |
|---|---|---|
| Startscreen / Betriebslage | 57er Kopf, rote Statusfläche, eine rote Primärfläche **STRECKE FREIGEBEN**, danach ruhige Kartenminiatur und sekundäre Kartenaktion. | Keine neue Zielgruppe, Navigation oder Angebotsreihenfolge. |
| Betriebslage-Karte | `surface`-Karte mit `ink`-Freigabe, rotem aktuellen Knoten, gedämpftem Folgeproblem und `grid-line`-Sperrstrang; Detailblatt vom unteren Rand als statisches Layout. | Keine echte Netzkarte, Reise-, Ticket- oder Echtzeitfunktion. |
| Ergebnis | Erst brauner Streckenausschnitt mit fiktivem Zug, dann Status **STRECKE FREIGEGEBEN**, Sterne, Zeit, Punkte, Kartenchip und die zwei bestehenden Folgeaktionen. | Keine neue Belohnung, keine Werbung zwischen Lösung und Ergebnis. |
| Betriebswerk | Helle Werkhalle, fiktiver Zug als große materielle Form, bestehende Reiter **ZUGPARK** und **BETRIEBSVERBESSERUNGEN**. | Keine Kauf-, Preis-, Freischalt- oder Spielvorteilsregel. |
| Profil | Anthrazitfarbener Kopf, ruhige Rasterkarten für vorhandene Meisterschaftswerte, darunter Listenzeilen. | Keine neue Statistik-, Profil- oder Bewertungsregel. |
| Betriebslage des Tages | Anthrazite Meldetafel mit rotem Kopfstreifen und weiß lesbarer Fixture-Belohnung; getrennte, ruhige Erklärung. | Keine Tages-/Zeitzonenpolicy, kein Casino-Rad und keine Kaufmechanik. |

Außerhalb des Rätsels zeigt die untere Navigation **nur** Betriebslage, Betriebswerk und Profil.
Die Tagesmeldung ist eine Karte innerhalb der Betriebslage und kein viertes Hauptziel.
Alle Beispielzahlen, Sterne, Namen und Statussätze in `04-screens.html` sind Beschriftungsfixtures und weder Level-, Ökonomie- noch Runtimebelege.

## 10. Abgrenzung für spätere Produktion

| Geliefert in Teil C | Ausdrücklich nicht geliefert / nicht entschieden |
|---|---|
| Drei statische Konstruktionsquellen und deren messbare Geometrie. | Kein Unity-Asset, keine UI-Toolkit-UXML/USS-Datei und kein Runtimecode. |
| Farbliche und semantische Trennung von Braun, Grau, Rot und Gelb. | Keine neue Puzzle-, Solver-, Validierungs- oder Lösungsregel. |
| Layoutreferenzen für 4×4, 10×10 und die sechs vorhandenen Nicht-Puzzle-Screens. | Keine Levelinstanz, keine versteckte Ziellösung und keine Zeitkalibrierung. |
| Lokale Verweise auf zwei erwartete IBM-Plex-Sans-Dateien. | Keine Lizenzbehauptung, Einbettungsfreigabe oder Ersatzschriftentscheidung. |
| Eigenständige abstrakte Kontrollzeichen und fiktive Layoutinhalte. | Keine Rechtsfreigabe und keine Übernahme offizieller Kennzeichen, Screens oder Piktogramme. |

Für eine spätere Produktion sind die gezeigten Maße gegen die tatsächlich entschiedene Gerätegeometrie, Schriftlieferung, Kontrastprüfung und UI-Implementierung zu verifizieren.
Das ist eine nachgelagerte Freigabeaufgabe und wird durch diese Designreferenz nicht vorweggenommen.
