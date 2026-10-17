# 01 – Marke und App-Icon

**Stilkorrektur 17.10.2026:** [Direkte Ownerfestlegung](evidence/OWNER_STYLE_CORRECTION_2026-10-17.md): Cartoon-/Comic-Spielwelt, plump-staubig-behördenhaft, mit **glasklarer Deutsche-Bahn-Anspielung**. Die klare dienstliche Informationshierarchie bleibt ihr bewusstes Gegenstück, nicht ein generischer Fantasy-/Candy-Look. Konkrete Typo-/Zeichen-/Layoutwerte bleiben Vorschläge; keine Annahme der abgelehnten Realismusbilder.


> **Entwurfsstatus / Empfehlung zur ausdrücklichen Freigabe:** Dieses Kapitel konkretisiert nur eine dokumentarische Gestaltungsrichtung.
> Es begründet keine Produktregel, keine Rechtefreigabe, keinen Produktionsimport und keine Aussage über Store-Zulassung.

## 1. Zweck, Ausgangspunkt und verbindliche Grenzen

- Gegenstand ist eine einzige Wort-/Bildmarke für **STAMMSTRECKEN-PUZZLE**, ein eigenständiges Gleisknoten-Zeichen und dessen App-Icon.
- Die Ausarbeitung nutzt die gemeinsame Entwurfspalette und die gemeinsame Schriftfamilie aus dem [Owner- und Designbrief](evidence/OWNER_CONTEXT_AND_DESIGN_BRIEF.md).
- Die empfohlenen Werte sind absichtlich konkret, bleiben aber bis zur ausdrücklichen Owner-, Produkt- und Rechtsfreigabe veränderbare Vorschläge.
- Die gewünschte Nähe entsteht über dienstliche Rot-/Weiß-/Anthrazit-Hierarchie, straffe Schrift und Störungslogik, nicht durch ein offizielles Kennzeichen.
- Zeichen, Wortmarke und App-Icon tragen weder `DB` noch einen Buchstaben, ein Ticketmotiv, eine Fahrplanauskunft oder Echtzeitbehauptung.
- Das Zeichen ist kein Monogramm, keine Umzeichnung einer bestehenden Wort-Bild-Marke und keine nachgezeichnete App-Oberfläche.
- Die Produktrolle bleibt ein unabhängiges satirisches Logikspiel; nicht eine Reise-, Ticket- oder Verkehrs-App.
- Die Verwendung der Begriffe Stammstrecke, Betriebslage und Schienenersatzverkehr folgt der sprachlichen Fachgrundlage, nicht einer behaupteten Kooperation.
- Die konkrete Bahnfahrer-Wiedererkennung und die Eigenständigkeitsgrenzen folgen [Konzept 10: Visuelle Identität](../../../Stammstrecken_Puzzle_Konzept_00-15/10_Visuelle_Identitaet_und_Stammstrecke.md#3-marken--und-app-identität) und [Konzept 11: Produktästhetik und Grenzen](../../../Stammstrecken_Puzzle_Konzept_00-15/11_Bahnfahrer_Lore_und_Satire_Leitplanken.md#6-produktästhetik-nah-inspiriert-aber-eigenständig).

## 2. Benannte Empfehlung

| Baustein | Empfohlene Festlegung zur Freigabe |
|---|---|
| Produktname | `STAMMSTRECKEN-PUZZLE`; Großschreibung nur innerhalb der Marke, Fließtext bleibt normal gesetzt. |
| Wort-/Bildmarke | Links der unterbrochene Gleisknoten, rechts eine einzeilige tatsächliche Textsetzung `STAMMSTRECKEN` + roter Unterbrechungsbalken + `PUZZLE`. |
| Zeichenname | **Unterbrochener Gleisknoten**; zwei rechtwinklig geführte Strecken verfehlen sich diagonal im Zentrum. |
| Primärfarbe des Zeichens | `ink #20272C` auf `paper #FFFFFF` oder `surface #F4F2EE`. |
| Wiedererkennungsakzent | Nur der Unterbrechungsbalken der Primärmarke setzt `brand-red #C83D45`. |
| Anmutungsträger | Proportion, Kontrast und Informationsdisziplin; keine Übernahme fremder Buchstabenformen, Markenrahmen oder Piktogramme. |
| Referenzstatus | Die drei Dateien unter `references/` sind statische, reproduzierbare Dokumentationsquellen und keine App-Screens oder Runtimeassets. |

## 3. Zeichenkonstruktion: unterbrochener Gleisknoten

- Masterfläche: quadratisches Koordinatensystem `96 × 96` ohne Hintergrund.
- Die sichtbare Zeichenzone liegt einschließlich 8-Einheiten-Strichstärke in `x=8…88` und `y=8…88`.
- Beide Routen haben `stroke-width=8`, `stroke-linecap=square`, `stroke-linejoin=round`, keine Füllung und keinen Schatten.
- Die empfohlene Primärfarbe jedes Strichs ist `#20272C`; die SVG enthält keine Verlauf-, Filter- oder Maskeneffekte.
- Route Südwest beginnt bei `(12,84)`, verläuft horizontal bis `(32,84)`, biegt mit Radius `16` nach oben und endet bei `(48,60)`.
- Route Nordost beginnt bei `(84,12)`, verläuft vertikal bis `(84,32)`, biegt mit Radius `16` nach links und endet bei `(60,48)`.
- Die exakten Pfaddaten lauten `M 12,84 H 32 C 40.837,84 48,76.837 48,68 V 60`.
- Die zweiten exakten Pfaddaten lauten `M 84,12 V 32 C 84,40.837 76.837,48 68,48 H 60`.
- Die beiden offenen Endmittelpunkte liegen `sqrt(288) = 16.971` Konstruktionseinheiten auseinander.
- Mit den quadratischen 8-Einheiten-Enden beträgt die engste sichtbare diagonale Leerstelle `sqrt(32) = 5.657` Einheiten.
- Die Leerstelle ist die semantische Aussage des Zeichens: Verbindung wird fast erreicht, aber nicht geschlossen.
- Die Lenkung jeder Route dreht genau `90°`; es gibt keinen Kreuzungspunkt, keine dritte Strecke und kein Buchstabenfragment.
- Die zwei Ausfahrten liegen bewusst an diagonal gegenüberliegenden Seiten, nicht als symmetrische horizontale oder vertikale Doppelspur.
- Der Knoten darf nicht geschlossen, gespiegelt, gedreht, gestaucht oder um einen Anschluss ergänzt werden.
- Die Striche dürfen nicht als braune konkrete Puzzle-Gleise eingefärbt werden; die Zeichenfarbe beschreibt Marke, nicht Feldsemantik.
- Die Zeichenfläche bleibt ohne Schwellen, Schotter, Räder, Bahnhofspiktogramm, Ziffern oder Dekoration.
- Die normierte Vektorquelle ist [references/brand-symbol.svg](references/brand-symbol.svg).

| Konstruktionswert | Wert | Messregel |
|---|---:|---|
| Masterkante | 96 | Eine Einheit entspricht `1/96` der Zeichenbreite. |
| äußerer Sicherheitsrand | 8 | Von jeder Artboardkante bis zur äußeren Strichkante. |
| Strichbreite | 8 | Konstant auf beiden Routen, auch an den Bögen. |
| Biegeradius | 16 | Radius der Mittellinie in beiden 90°-Kurven. |
| Innere offene Endpunkte | `(48,60)` / `(60,48)` | Nie durch Kappe, Punkt oder Füllform verbinden. |
| Diagonalabstand Endmittelpunkte | 16.971 | `sqrt(12² + 12²)`. |
| sichtbare Engstelle | 5.657 | Mindestabstand der quadratischen Endkappen. |
| Außenendpunkte | `(12,84)` / `(84,12)` | Bei jeder Skalierung erhalten. |

## 4. Wortmarke und Bild-/Wort-Satz

- Die Wortmarke besteht aus echtem Text, nicht aus in Pfade umgewandelten Buchstaben; die Referenz lädt `IBM Plex Sans Condensed SemiBold` per `@font-face`.
- Schriftfamilie für die Wortmarke: `IBM Plex Sans Condensed`, Gewicht `600`, Stil `normal`.
- Die HTML-Referenz erwartet die separat gelieferte Datei `Fonts/IBMPlexSansCondensed-SemiBold.ttf` relativ zu `references/01-brand.html`.
- Der Satz ist ausschließlich in Versalien und lautet sichtbar `STAMMSTRECKEN` — Unterbrechungsbalken — `PUZZLE`.
- Der Balken ersetzt in der Marke einen einfachen Bindestrich nicht als Textzeichen, sondern als eigene geometrische Verbindungsunterbrechung.
- Im Standardmuster beträgt die Schriftgröße `52 px`, die Zeilenhöhe `0.92` und das Letterspacing `0.026em`.
- Der Unterbrechungsbalken misst `0.64em × 0.102em`, hat links und rechts jeweils `0.17em` Abstand und wird um `0.045em` nach unten versetzt.
- Die Wortmarke ist einzeilig; keine Trennung nach `STAMM-`, kein Umbruch vor `PUZZLE` und keine gestapelte Wortmarke.
- Das Zeichen steht links auf optischer Mittellinie der Wortmarke; im Standardmuster misst es `64 × 64` logische Einheiten.
- Der feste Zwischenraum von Zeichenkante bis erstem `S` beträgt bei der Standarddarstellung `20` logische Einheiten.
- Die Standardrelation ist eine Zeichen-Artboardhöhe von 64 zu einer Schriftgröße von 52 logischen Pixeln; die tatsächliche Versalhöhe der mitgelieferten Schrift ist kleiner und wird nicht mit der Schriftgröße gleichgesetzt.
- Die Schrift und das Zeichen erhalten keine Kontur, Kursivschnitt, Textschatten, Glanzkante oder perspektivische Verformung.
- Der rote Balken ist der einzige farbige Buchstaben-Begleiter; die Buchstaben bleiben in der Primärmarke `ink`.
- Die statische Satzansicht ist [references/01-brand.html](references/01-brand.html); sie verwendet dieselben Tokens wie die übrigen Kapitel dieses Work Packages.

| Satzvariante | Zeichen | Text | Unterbrechungsbalken | Vorgesehener Untergrund |
|---|---|---|---|---|
| Primär | `ink #20272C` | `ink #20272C` | `brand-red #C83D45` | `paper` oder `surface` |
| Negativ | `paper #FFFFFF` | `paper #FFFFFF` | `brand-red #C83D45` | `ink #20272C` |
| Monochrom hell | `paper #FFFFFF` | `paper #FFFFFF` | `paper #FFFFFF` | Vollton `ink` oder Schwarz |
| Monochrom dunkel | `ink #20272C` | `ink #20272C` | `ink #20272C` | Weiß oder unbedruckter heller Träger |

## 5. Freiraum, Ausrichtung und Größenvarianten

- Der Schutzraum der Einzelmarke beträgt rundum mindestens `12` Mastereinheiten des Zeichens, also `12.5 %` der Zeichenbreite.
- Für die Wort-/Bildmarke ist der Mindestfreiraum rundum mindestens die sichtbare Breite des roten Unterbrechungsbalkens; bei der Standardgröße sind das `33.28 px`.
- An UI-Kanten und Anschnittflächen gilt der größere Wert aus Markenfreiraum und vorhandener Layout-Sicherheitszone; dieses Kapitel setzt keine Geräte-Safe-Area.
- Im horizontalen Lock-up darf die Wortmarke weder höher noch näher als die oben definierten Maße zum Zeichen gesetzt werden.
- Die kompakte Variante reduziert nur Zeichenmaß, Textgröße und Zwischenraum proportional; Pfade, Strichstärke-Verhältnis und Farbe ändern sich nicht.
- Die Zeichen-allein-Variante nutzt exakt denselben 96er Vektor, nicht einen neuen Piktogramm- oder Monogrammentwurf.
- Die App-Icon-Variante nutzt denselben Knoten, jedoch als zentrierten Bestandteil der unten beschriebenen 1024er Fläche.
- Unterhalb der Mindestgröße wird nicht nachverdichtet, die Leerstelle nicht gefüllt und keine dünnere Ersatzgeometrie geschaffen.

| Ausgabeform | Empfohlene Anwendung | Mindestgröße | Feste Parameter |
|---|---|---:|---|
| Primär, horizontal | Store-Feature, Startfläche, Titelbild, Abschlussreferenz | 640 logische px Gesamtbreite | Zeichen 64; Abstand 20; Wortmarke 52 px. |
| Kompakt, horizontal | schmale statische Dokumentkopfleiste | 320 logische px Gesamtbreite | Zeichen 40; Abstand 12; Wortmarke 28/30 px mit +0.026em. |
| Zeichen allein | quadratische Kennzeichnung ohne Produktname | 24 × 24 logische px | 96er Quelle vollständig, keine Beschriftung. |
| App-Icon | Launcher- und Store-Icon-Master | 1024 × 1024 Exportmaster | Geometrie ausschließlich aus Abschnitt 7. |
| Einfarbig | Prägung, Einfarbdruck, technisch begrenzte Ausgabe | Zeichen mindestens 24 × 24 | Alle Bestandteile exakt eine Volltonfarbe. |

## 6. Hell-, Dunkel- und Monoregeln

- Auf `paper #FFFFFF` und `surface #F4F2EE` wird die Primärvariante mit anthrazitfarbenem Zeichen und Text verwendet.
- Auf `ink #20272C` wird die Negativvariante mit weißem Zeichen und Text sowie rotem Unterbrechungsbalken verwendet.
- Die Monovariante setzt *alle* sichtbaren Bestandteile in genau einer Farbe; der Unterbrechungsbalken bleibt deshalb nicht rot.
- Eine rote Zeichenvariante ist nur als Einzelzeichen auf hellem Untergrund zulässig, nicht als zusätzliche Primärmarke und nicht auf rotem Grund.
- `track-brown #805535`, `assumption-gray #727B82`, `empty-red #B42E39`, `selection-yellow #F1C84B` und `selection-edge #6C5717` gehören zur Puzzle-Semantik und ersetzen keine Markenfarben.
- Gelb darf nie als Marken-Highlight, Markenbalken, Iconhintergrund oder „erfolgreiche Verbindung“ eingesetzt werden.
- Graue oder braune Einzelzeichen werden nicht als Logo-Variante verwendet, weil sie die dokumentierte Rätselbedeutung verfälschen könnten.
- Farbflächen hinter der Marke bleiben glatt; kein Gleistextur-, Schotter-, Metall-, Flaggen- oder Verlaufsmuster wird in den Schutzraum gelegt.

## 7. App-Icon-Geometrie

- Der Referenzmaster misst exakt `1024 × 1024` Einheiten und verwendet keinen Text, keine Initialen und keine Zahl.
- Das äußere Feld liegt bei `x=48`, `y=48`, hat `928 × 928` Einheiten und einen Eckradius von `216`.
- Das äußere Feld nutzt allein `brand-red #C83D45`; bis zu jeder Masterkante bleibt ein Rand von `48` Einheiten frei.
- Die zentrale Informationsfläche liegt bei `x=128`, `y=128`, hat `768 × 768` Einheiten und einen Eckradius von `156`.
- Die dadurch entstehende rote Rahmenbreite beträgt an den geraden Seiten exakt `80` Einheiten.
- Die Informationsfläche nutzt `surface #F4F2EE`, nicht reines Weiß und nicht ein rotes Quadrat mit weißem Monogramm.
- Der Gleisknoten wird mit `translate(224 224) scale(6)` aus dem unveränderten 96er Zeichen abgeleitet.
- Die transformierte Zeichenbox misst `576 × 576`; die sichtbaren äußeren Strichkanten liegen dadurch `168` Einheiten von der Innenfläche entfernt.
- Das Zeichen bleibt `ink #20272C`; die offene diagonale Unterbrechung bleibt im Icon sichtbar und wird nicht durch einen roten Punkt markiert.
- Es gibt keinen Buchstaben, keine Bahnabkürzung, keinen Fahrzeugkopf und keine Pfeil-, Ticket- oder Standortmarkierung.
- Die Vektorquelle ist [references/app-icon.svg](references/app-icon.svg).

| Iconteil | Koordinaten / Maß | Farbe | Konstruktionsregel |
|---|---:|---|---|
| Exportfläche | `1024 × 1024` | transparent außerhalb | Master, keine gerätespezifische Zuschnittbehauptung. |
| Außenfeld | `48,48,928,928; r=216` | `#C83D45` | Rundquadrat, keine Quadrat-Ecken. |
| Innenfläche | `128,128,768,768; r=156` | `#F4F2EE` | genau 80 Einheiten rote Seitenrahmung. |
| Knotentransformation | `translate(224,224) scale(6)` | `#20272C` | dieselben zwei Pfade wie das Zeichen. |
| Textinhalt | keiner | — | Icon bleibt bildhaft und eigenständig. |

## 8. Darstellungsgrenzen und Ausschlüsse

- Das Zeichen darf nicht in einem roten Quadrat ohne die helle Innenfläche platziert werden, wenn dadurch eine monogrammartige Verwechslungswirkung entsteht.
- Das Zeichen darf nicht zusammen mit `DB`, „Deutsche Bahn“, offiziellen Linienkennungen, echten Stationsnamen als Absender oder Kooperationsaussagen erscheinen.
- Die Wortmarke darf nicht in eine fremde Logo-Rahmenform, ein langes rotes Schild oder eine nachgezeichnete Wort-Bild-Komposition gesetzt werden.
- Das rote Rechteck darf nicht auf die Buchstaben gelegt werden; es bleibt der getrennte Unterbrechungsbalken zwischen beiden Wortteilen.
- Kein Bestandteil wird gedreht, gespiegelt, mit Outline versehen, gedehnt, animiert geschlossen oder mit einer dritten Route ergänzt.
- Keine Puzzle-Zellen, Randzahlen, A/B-Anschlüsse oder sechs konkreten Gleisformen werden in die Marke integriert.
- Das Zeichen ist kein Spielzustand: Es zeigt weder eine gültige Verbindung noch „Fehler“, „Auswahl“, Erfolg oder eine konkrete Puzzle-Lösung.
- Es wird nicht auf kleineren Ausgaben als 24 logischen Pixeln eingesetzt; darunter wird die Marke weggelassen, nicht neu gezeichnet.
- Eine technische Monochromproduktion muss die freie Mitte und die 5.657-Einheiten-Engstelle erhalten; füllende Druckzunahme ist vorab zu prüfen.
- Der Entwurf behauptet keine Markenverfügbarkeit, Schrifteinbettungsfreigabe, Plattformfreigabe oder rechtliche Risikobewertung.

## 9. Statische Referenzen und Prüfhinweise für die spätere Freigabe

| Datei | Inhalt | Dokumentarische Rolle |
|---|---|---|
| [references/brand-symbol.svg](references/brand-symbol.svg) | 96er Pfade des Knotens, mit zugänglichem Titel und Beschreibung. | Quelle für Geometrie, nicht für Runtimeimport. |
| [references/app-icon.svg](references/app-icon.svg) | 1024er Icon aus Außenfeld, Innenfläche und sechsfach skaliertem Knoten. | Quelle für Iconproportionen, nicht für Store-Upload. |
| [references/01-brand.html](references/01-brand.html) | Statische Ansicht von Primärmarke, Icon, Negativ- und Monovariante. | Sichtprüfung der Satz- und Kontrastbeziehung. |
| `Fonts/IBMPlexSansCondensed-SemiBold.ttf` | Mitgelieferte unveränderte Originalschrift; Provenienz und Lizenz siehe Kapitel 02. | Keine Lizenzbehauptung in diesem Kapitel. |

- Vor einer späteren Entscheidung sollte die zuständige Rolle die HTML-Ansicht mit der tatsächlich bereitgestellten TTF öffnen und die Zeilenfreiheit der Gesamtmarke bei 640 und 320 logischen Pixeln sowie des Einzelzeichens bei 24 Pixeln prüfen.
- Vor Produktion sollte die zuständige Rechtsprüfung Namen, Icon, Storegrafiken, Gesamtaufmachung und die konkrete Vertriebsverwendung gesondert beurteilen.
- Vor einem Runtimeimport bleiben Adressierung, Importpreset, gerätespezifischer Maskenbeschnitt, Kontrastnachweis und Plattform-Metadaten außerhalb dieser Empfehlung.
- Die in [Konzept 11](../../../Stammstrecken_Puzzle_Konzept_00-15/11_Bahnfahrer_Lore_und_Satire_Leitplanken.md#7-rechtliche-arbeitsleitplanken) genannten Grenzen gelten weiter; diese Konstruktion ersetzt keine Einzelfallprüfung.
- Diese Markenempfehlung verändert keine Puzzle-Regel: Braun bleibt konkrete Schiene, Grau offene Annahme, Rot X und Gelb ausschließlich Auswahl.
