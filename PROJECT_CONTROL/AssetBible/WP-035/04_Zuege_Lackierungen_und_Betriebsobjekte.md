# 04 – Züge, Lackierungen und Betriebsobjekte

**Teil D · Entwurfsstand zur ausdrücklichen Owner-/Produkt- und Rechtsfreigabe.**

Dieses Kapitel empfiehlt eine eigene, tatsächlich konstruierbare Formensprache für die kosmetische Miniaturbahnwelt.

Es bestätigt weder einen Produktionskatalog noch Shop-SKUs, Preise, Freischaltungen, Assetadressen oder Runtimeimporte.

Die Fahrzeuge bleiben rein kosmetisch: Nach einer gültigen Lösung fahren sie die selbst gebaute A–B-Verbindung ab und ändern weder Hinweis, Stern, Zeit noch Rätselschwierigkeit.

Alle Maße sind relationale Konstruktionsmaße für Referenzzeichnungen, kein Geräte-, Physik- oder Importvertrag.

## 1. Gemeinsamer Formvertrag

| Baustein | Empfohlene, freizugebende Festlegung | Abgrenzung |
|---|---|---|
| Grundcharakter | Präziser Nahverkehrstriebzug als wertiges Miniaturmodell: dienstlich, kompakt, leicht überbeansprucht, niemals vermenschlicht. | Keine reale Baureihe nachzeichnen; keine Zuggesichter, Augen oder Slapstick-Mimik. |
| Stammeinheit `L` | Eine Wagenkastenlänge ohne Kupplung. Alle folgenden Längen werden in `L` beschrieben. | `L` ist kein Metermaß und keine Kollisionsbox. |
| Stammeinheit `H` | Normierte Wagenkastenhöhe von Kastenunterkante bis Dachoberkante; der Abstand zur Schienenoberkante ist separat 0,21 H; Breite `W = 0,72 H`; Standardkastenlänge `L = 3,18 H`. | Gleiche Grundproportion in allen Familien sichert Wiedererkennung. |
| Silhouette | Flach abgeschrägte Front, hoher gerader Seitenkasten, leicht nach innen gezogene Dachkante und sichtbare Drehgestelle. | Kein stromlinienförmiger Fernverkehrszug und keine kantige Kopie einer bekannten S-Bahn-Front. |
| Wiederholung | Front, Fensterband, Türmodul, Dach- und Seitenband sind gemeinsame Bausteine; Wagenzahl, Reihenfolge und Bandrhythmus differenzieren die Familien. | Variation kommt nicht durch neue Markenfarben oder beliebige Anbauten. |
| Darstellung | Fahrtrichtung ist über die Frontform, nicht über Logos, Baureihenanschriften oder Zieltexte lesbar. | Keine echten Betreiberzeichen, Liniennummern, Ortsnamen oder Fahrplandaten. |

> **Kernidee:** Die unmittelbare Pendler-Wiedererkennung entsteht aus dienstlicher Rot-/Weiß-/Anthrazit-Hierarchie, rationaler Fahrzeuggeometrie und der Lage auf der Miniaturkarte – nicht aus einer Kopie eines einzelnen existierenden Zuges.

## 2. Wagenkasten, Front und Laufwerk

### 2.1 Normierter Wagenkasten

| Merkmal | Relative Konstruktion | Sichtbare Folge |
|---|---|---|
| Kasten | Standardwagen `1,00 L × 1,00 H × 0,72 H`; Mittelwagen `0,92 L` bei gleicher Höhe und Breite. | Mittelwagen lesen sich als modulare Ergänzung, nicht als zweites Fahrzeugmodell. |
| Dach | Dachhaut `0,10 H` hoch; von der Seitenwand um `0,035 H` eingezogen; oben mit einem geraden Feld von `0,46 W`. | Dunkles Dach sitzt wie eine technische Kappe über dem hellen Kasten. |
| Unterkante | Untergestell `0,16 H` hoch; Wagenkasten beginnt bei `0,21 H` über Schienenoberkante. | Genug Tiefe für glaubhafte Drehgestelle, ohne Simulationshärte. |
| Ecken | Kastenradien: Dachschulter `0,045 H`, untere Seitenkante `0,025 H`, Stirnseitenecken `0,055 H`. | Weiche, modellhafte Reflexe statt einer glatten Spielzeugkapsel. |
| Stoßfuge | Zwischen Wagen `0,075 L` Luft; Faltenbalg/Übergang `0,035 L` breit und anthrazit. | Wagenreihung bleibt auch aus Kartenperspektive lesbar. |
| Kupplung | Zentral, `0,055 L` vorstehend, Schaft `0,018 H` dick, rechteckiger Kopf mit zwei abgerundeten Ecken. | Eigene technische Signatur, keine Nachbildung einer konkreten Kupplung. |
| Drehgestelle | Zwei pro Standardwagen, Achsmitten bei `0,19 L` und `0,81 L`; beim Mittelwagen bei `0,18 L` und `0,74 L`. | Kurze Überhänge und eine kompakte Nahverkehrshaltung. |
| Räder | Raddurchmesser `0,17 H`; Achse `0,11 H` über Schienenoberkante; vereinfachte volle Scheiben statt Speichen. | Auf kleinen Ansichten ruhig, in der Betriebswerkansicht materiell genug. |

### 2.2 Eigenständige Stirnfront

| Element | Präzise Empfehlung | Warum die Form eigenständig bleibt |
|---|---|---|
| Frontfläche | Senkrechte Hauptfläche `0,62 H` hoch; darunter eine `0,18 H` hohe, nach hinten laufende Schürze mit 14° Neigung. | Die Front ist bewusst flach abgeschrägt statt rund, spitz oder voll verglast. |
| Seitenfacetten | Je Seite eine `0,085 W` breite Facette, um 12° nach hinten gesetzt; mittlere Frontfläche bleibt `0,83 W`. | Erzeugt eine präzise dreiteilige Stirn ohne bekannte Monogramm- oder Maskengeometrie. |
| Windschutzscheibe | Zwei getrennte schmale Rechtecke, jeweils `0,29 W × 0,205 H`; Oberkante `0,115 H` unter Dachkante, Zwischensteg `0,075 W`. | Der vertikale Steg und die schmalen Gläser machen die Front sofort als eigene Familie lesbar. |
| Glasneigung | Beide Scheiben folgen der Hauptfläche; nur die äußeren oberen Ecken erhalten einen Radius `0,024 H`. | Keine geschwungene Panorama- oder durchgehende schwarze Maske. |
| Leuchten | Zwei kleine, weit außen liegende Rechtecke `0,095 W × 0,045 H`; Mittelpunkte bei `0,17 W` und `0,83 W`, `0,58 H` über Schienenoberkante. | Ihre Distanz und rechteckige Ruhe unterscheiden sich von paarigen Rundaugen oder einem Lichtbalken. |
| Leuchtenfassung | Anthrazit gerahmt, Innenfläche `paper`; im Stillbild kein blinkender Effekt und keine reale Signalbedeutung. | Lesbar als Fahrzeugdetail, ohne eine reale Lichtsignatur zu imitieren. |
| Untere Signatur | Mittig eine eingelassene, vertikale Servicefuge `0,022 W` breit und `0,11 H` hoch; keine Beschriftung. | Wiederkehrendes eigenes Erkennungsdetail statt Betreiberlogo. |
| Stirnabschluss | Kupplung tritt durch eine `0,13 W × 0,065 H` große anthrazitfarbene Öffnung aus; Schürzenkante endet `0,07 H` über Schiene. | Konstruktion bleibt glaubhaft, aber nicht baureihenspezifisch. |

### 2.3 Seitenfenster und Türen

| Modul pro Standardwagen | Relative Lage auf einer Längsseite | Konstruktion |
|---|---|---|
| Führerraumfenster | Vorderes `0,16 L`; Fenster `0,10 L × 0,19 H`, Brüstung bei `0,42 H`. | Ein schmales Seitenfenster hinter der Frontfacette, nicht bis zur Dachkante gezogen. |
| Tür 1 | Achse bei `0,30 L`; Öffnung `0,115 L × 0,36 H`. | Zweiflügelig durch eine mittige `0,012 L`-Fuge, beide Blätter offwhite. |
| Fensterfeld A | `0,14 L` zwischen Führerraum und Tür. | Zwei getrennte Hochrechtecke mit `0,018 L` Pfosten. |
| Fensterfeld B | `0,20 L` zwischen den Türen. | Drei Hochrechtecke; mittleres Fenster genau `0,82` der Breite der äußeren. |
| Tür 2 | Achse bei `0,70 L`; gleiche Geometrie wie Tür 1. | Symmetrische Einstiegslogik ohne reale Türanordnung zu kopieren. |
| Fensterfeld C | `0,15 L` zwischen Tür 2 und Wagenende. | Zwei Hochrechtecke; Endpfosten mindestens `0,045 L` breit. |
| Mittelwagen | Kein Führerraumfenster; Türachsen bei `0,25 L` und `0,67 L`; mittiges Dreifensterfeld. | Das Modul wird als Anhänger erkannt, ohne eine andere Formensprache einzuführen. |
| Fensterband | Durchgehende Höhe `0,23 H`, Unterkante `0,40 H`, oberer Rand `0,08 H` unter Dachschulter. | Dunkles, gerades Band fasst Einzelgläser zusammen; Türblätter unterbrechen es sichtbar. |

## 3. Material- und Lackvertrag

Es werden ausschließlich die gemeinsamen Vorschlagstokens aus dem [Owner- und Designbrief](evidence/OWNER_CONTEXT_AND_DESIGN_BRIEF.md) verwendet; keine Zusatzpalette und keine Ersatzschrift.

| Ebene | Token / Wert | Auftrag am Fahrzeug | Materialwirkung |
|---|---|---|---|
| Wagenkörper | `surface` · `#F4F2EE` | Ganze Seitenwand, Türen und Stirnflächen unterhalb des Fensters. | Seidenmatt; breite, ruhige helle Flächen. |
| Spitzlichter / kleine Flächen | `paper` · `#FFFFFF` | Leuchteninnenfläche, kleine Dachtechnikfläche, neutrale Reflexkante. | Mattes, sauberes Weiß, nie als Betreiberzeichen. |
| Dach und Fensterband | `ink` · `#20272C` | Dachhaut, Fensterrahmen, Untergestell, Drehgestell, Kupplung und Fensterband. | Fein satiniert; stärkerer Glanz nur an vertikalen Glaskanten. |
| Seitenband | `brand-red` · `#C83D45` | Unterbrochene, eigenständige Seitenbandsegmente `0,038 H` hoch. | Satiniert und flächig; kein durchlaufender roter Streifen. |
| Gleisnähe | `track-brown` · `#805535` | Schienen- und Holzschwellenmaterial in Fahr- und Weltansicht, nie Fahrzeuglack. | Warm, leicht stumpf; hält den Zug von der Puzzle-Semantik getrennt. |
| Optionale Zustandsmarke | `brand-red` · `#C83D45` | Nur kleine abnehmbare Störungsabdeckung an Objektdeko, nicht am Zugband. | Flach und eindeutig situativ, niemals Rätselfehlermeldung. |

Das rote Seitenband beginnt `0,165 H` über der Unterkante und wird an jeder Türöffnung um mindestens `0,035 L` unterbrochen.

Ein Bandsegment ist maximal `0,23 L` lang; zwischen zwei Segmenten liegen mindestens `0,075 L` helle Wagenfläche.

Die Anordnung darf nie eine durchgehende horizontale Markenbinde ergeben, auch nicht über zwei gekuppelte Wagen.

Dach, Fensterband und Untergestell bleiben `ink`; keine schwarze oder rote Vollfront und keine fremde Wort-/Bildmarke.

## 4. Fünf gestalterische Familien

Die folgenden Bezeichnungen sind **Arbeitsnamen für Formen**, keine finalen Katalog-IDs, Shop-SKUs, Preise, Freischaltungen oder Verkaufsversprechen.

| Familienform | Empfohlene Reihung | Abweichung innerhalb des Formvertrags | Lackrhythmus und situative Aussage |
|---|---|---|---|
| **Kompakter Starttriebzug** | `A–M–A`; Endwagen je `1,00 L`, Mittelwagen `0,92 L`; Gesamtlänge ohne Kupplungsüberstände `2,92 L`. | Beide Endwagen identische flach abgeschrägte Front; mittlerer Wagen erhält eine schmale Dachklappe `0,16 L × 0,035 H`. | Je Endwagen zwei rote Segmente unter den Fensterfeldern B/C; Mittelwagen drei kürzere Segmente. Der ruhigste, dienstlichste Ausgangspunkt. |
| **Modulare Ersatzgarnitur** | Grundbild `A–T`; Erweiterung stets um einen `T`-Mittel-/Beiwagen von `0,92 L`, maximal vier sichtbare Wagen in der Referenz. | Jeder Zusatzwagen hat dieselbe Dachhöhe und Kupplung, aber eine um `0,03 L` versetzte Türachse; keine improvisierten Fremdwaggons. | Rote Segmente beginnen an jedem Modul neu und enden vor dem Übergang. Die Satire liegt in der akkurat zusammengestellten Provisorik, nicht in Schaden oder Chaos. |
| **Funktionierender Zugteil** | `A–A`, zwei Endwagen Rücken an Rücken, `2,075 L` einschließlich Übergang. | Innenenden tragen keinen zweiten Führerraum: stattdessen ein `0,10 L` breiter, anthrazit gerahmter Übergang. | Ein einziges kurzes Bandsegment pro Wagen, exakt unter Fensterfeld B. Die kleine Einheit wirkt entschlossen und erstaunlich betriebsfähig. |
| **Wagenreihung** | `A–M–T–A`; `3,84 L` ohne Kupplungen. Der `T`-Wagen hat nur eine mittige Türgruppe. | Fensterteilung ist konsistent, aber die Reihenfolge von Tür- und Dreifensterfeld kippt am `T`-Wagen spiegelbildlich. | Das Bandsegment des `T`-Wagens ist um `0,07 L` gegenüber dem Mittelwagen versetzt. Die trockene Pointe ist eine sichtbar korrekte, aber ungewohnt geordnete Reihung. |
| **Anschlusswunder-Express** | `A–M–A`, aber mit `0,94 L` langen Endwagen und `0,98 L` langem Mittelwagen; kompakt statt hochgeschwind. | Schürze um `0,025 H` höher; Fensterpfosten um `0,008 L` schmaler. Front, Leuchtenlage und Kupplung bleiben unverändert. | Vier besonders kurze rote Fragmente pro Wagen, jeweils zwischen Tür und Fensterfeld. „Express“ beschreibt die seltene gelungene Verbindung, nicht eine andere Zugklasse. |

`A` bezeichnet einen Wagen mit der definierten Front; `M` einen vollwertigen Mittelwagen; `T` einen stillen Zwischenwagen ohne neue Mechanik.

Diese Buchstaben sind Zeichnungsnotation und dürfen nicht als sichtbare Fahrzeugbeschriftung übernommen werden.

Kein Familienunterschied darf eine spielmechanische Wirkung, einen Fahrtbonus oder eine Informationshierarchie auf dem Puzzle erzeugen.

## 5. Ausprägung je Darstellungsraum

| Darstellungsraum | Zugreduktion | Pflichtmerkmale | Auslassen / untersagt |
|---|---|---|---|
| **Betriebslage-Karte** | Ein Wagen als `28 × 9` Referenzeinheiten, Drehgestelle als zwei `4 × 2`-Schatten; maximal vier Wagen sichtbar. | Flache Stirnfläche, geteilte Scheibe als zwei helle Striche, dunkles Dach, zwei bis vier getrennte rote Bandpixel. | Keine Türen, Innenraum, Zielanzeige, Fahrplandaten oder Echtzeitstatus. |
| **Puzzlefahrt nach Lösung** | Ein Wagen `0,78` einer Rasterzellenbreite lang und `0,24` Zellenbreiten hoch; Übergang `0,06` Zellenbreiten. | Beide Leuchten, zweigeteilte Windschutzscheibe, Fensterband, getrennte rote Bandfragmente und zwei Drehgestelle bleiben sichtbar. | Fahrzeug bleibt auf der bestätigten Strecke; keine Weiche, Brücke oder Busfahrt als neues Rätselwerkzeug. |
| **Betriebswerk** | Drei-Viertel-Ansicht bei `32°` horizontaler Drehung und `18°` Aufsicht; ein Fokuszug zeigt mindestens `2,0 L` seiner Reihung. | Volle Türfugen, Fensterpfosten, Kupplung, Dachkante und Materialreflexe; Grundlage für die Form-/Materialtafel. | Keine Kauf- oder Preisbadge, keine Freischaltbehauptung, keine reale Baureihennummer. |

Die Kartenreduktion verwendet nur Silhouette, Bandrhythmus und Frontmarker; sie ist keine zweite Lackierung.

Die Puzzlefahrt verwendet die exakt gebaute A–B-Strecke und zeigt keine Abkürzung oder unabhängige Schienenlogik.

Das Betriebswerk darf die Familien als Sammelgegenstände inszenieren, nicht als Produktangebot oder Lucky-Draw.

## 6. Betriebsobjekte als wiederverwendbarer Miniaturbaukasten

| Objekt | Konkrete Bauform | Farb- und Materialzuordnung | Zustandsdarstellung ohne neue Spielregel |
|---|---|---|---|
| **Signal** | Mast `0,78 H` hoch, Profil `0,035 H`; rechteckiger Kopf `0,18 H × 0,14 H`, nach vorne `0,055 H` tief; Sockel `0,12 H` breit. | Mast/Kopf `ink`, Lichtöffnung `paper`, Sockel `surface`. | „Betrieblich bereit“ = helle Papieröffnung mit sehr kleinem Glanz; „abgedeckt“ = `brand-red`-Abdeckplatte. Reine Kulisse, kein Signalspiel. |
| **Weiche** | Zwei parallel geführte `track-brown`-Schienen mit einer einzigen sichtbaren Abzweigzunge über `0,88 L`; niedriger Stellkasten `0,13 H × 0,16 L`. | Schiene `track-brown`, Stellkasten `ink`, Schwellen `surface` mit braunen Oberkanten. | Stets statisch als frisch gewartete Weltdeko; **niemals** ein Puzzletool, Feldtyp, auswählbares Element oder Pfadverzweiger. |
| **Bahnsteig** | Kante `1,35 L` lang, `0,11 H` hoch, Plattenraster `0,12 L`; Hinterkante `0,26 H` über Gleisniveau. | Grundkörper `surface`, Kante `paper`, Schatten/Unterkante `ink`. | Ein freier Bahnsteig bleibt ruhig; eine Absperrplatte in `brand-red` markiert nur Kulissenlage, nie einen gesperrten Rasterpfad. |
| **Bank** | Sitz `0,31 L` lang, `0,075 H` tief; Rücken `0,08 H` hoch; zwei Kufen `0,024 H` dick. | Sitz/Lehnen `surface`, Gestell `ink`. | Gerade, leer und sachlich; keine Figuren, Gepäckpantomime oder gesichtsartige Anordnung. |
| **Meldetafel** | Rahmen `0,31 L × 0,18 H`, auf zwei `0,022 H` hohen Stützen; innen vier neutrale Zeilenblöcke. | Rahmen `ink`, Fläche `paper`, eine `brand-red`-Statuszeile maximal `0,025 H` hoch. | Zeilen sind abstrakte Balken, keine echten Meldungstexte, Orte oder Fahrpläne. |
| **Wegweiser** | Pfosten `0,52 H`, zwei gegenläufige Rechteckpfeile `0,24 L × 0,075 H`, Abstand `0,035 H`. | Pfosten `ink`, Schilder `surface`, Rand `brand-red`. | Pfeile führen sichtbar um eine Baustellenkulisse, aber nicht in eine reale Richtung und nicht aus dem Puzzle heraus. |
| **Tunnelportal** | Öffnung `1,10 W` breit, `0,92 H` hoch; Wandring `0,12 H` stark; obere Ecken Radius `0,16 H`. | Portal `surface`, Innenraum `ink`, eine tiefe `track-brown`-Schwellenzone. | Optionaler roter Randstreifen nur als kurze Warnkante `0,04 H`; keine reale Stationsarchitektur nachbilden. |
| **Brücke** | Zwei seitliche Längsträger `1,55 L` lang, `0,10 H` hoch; drei Pfeilerachsen; Durchfahrtshöhe `0,52 H`. | Träger `ink`, Fahrbahn `surface`, Schienen `track-brown`. | Ganze Brücke ausschließlich auf Karte/Fahrtkulisse, niemals als Kreuzungs-, Überfahr- oder Sonderfeldregel. |
| **Ersatzhaltestelle** | Schildmast `0,72 H`; Schild `0,36 L × 0,19 H`; niedriger Bord `0,12 H`; daneben zwei modulare Sperrgitter à `0,42 L`. | Schild `surface` mit `brand-red`-Rand, Mast/Gitter `ink`, Störungskappe `brand-red`. | Satire durch überpräzise provisorische Beschilderung und den sichtbar langen Fußweg, nicht durch Textwitz oder reale Verkehrszeichen. |
| **Absperrung** | Drei Felder à `0,20 L` Breite, Gesamthöhe `0,22 H`; Füße `0,08 L` tief. | Rahmen `ink`, zwei getrennte `brand-red`-Flächen pro Feld, Zwischenräume `surface`. | Wiederverwendbar an Bahnsteig, Portal und Bauabschnitt; trennt nur Kulissenraum, nie Spielerinteraktion. |
| **Busmotiv** | Kompakter Quader `0,94 L × 0,29 H × 0,40 H`, flache `0,06 L` tiefe Stirn, sechs Seitenfenster. | Aufbau `surface`, Fenster/Dach `ink`, zwei kurze `brand-red`-Segmente unter Fenster 2 und 5. | Steht oder fährt ausschließlich **außerhalb** des Puzzlerasters als Schienenersatzverkehr-Motiv; keine Busroute, keine Spielhandlung. |

## 7. Wiederverwendung, Zustände und Satire

| Regel | Konkrete Anwendung |
|---|---|
| Ein Master pro Objekt | Signal, Bahnsteig, Bank, Tafel, Wegweiser, Portal, Brücke, Ersatzhaltestelle, Absperrung und Busmotiv werden je aus einer Grundgeometrie skaliert, gespiegelt oder als kurze Reihe wiederholt. |
| Lesbare Stufe | Karte zeigt Silhouette plus Rotakzent; Puzzlefahrt ergänzt Materialfuge; Betriebswerk ergänzt Volumen, Dach- und Kantenreflex. Die Bedeutung wechselt nicht zwischen diesen Stufen. |
| Zustände | „Betrieblich bereit“, „provisorisch“ und „baulich eingegrenzt“ entstehen ausschließlich durch Abdeckplatte, Absperrung, Wegweiser, Leerraum oder Reihenfolge der gleichen Objekte. |
| Keine Zerstörungskomik | Keine verbogenen Züge, Explosionen, anthropomorphe Signale, hilflosen Figuren oder Leiden als Pointe. Die Lage wirkt durch ihre unnötig präzise Verwaltung absurd. |
| Situative Formdetails | Die Ersatzhaltestelle erhält einen für die kleine Plattform zu großen, aber sauber gerahmten Wegweiser; die Wagenreihung bleibt ordentlicher als die Karte; der Bus wartet korrekt neben statt auf dem Gleis. |
| Schienenersatzverkehr | Bus, Ersatzschild, Absperrung und langer Fußweg bilden gemeinsam die sichtbare Erzählung. Sie bleiben Kulisse und verdrängen die Logikfläche nicht. |
| Wiedererkennung | Das wiederkehrende rote Fragmentband, die geteilte Frontscheibe und die stille anthrazitfarbene Dachkappe verbinden alle Fahrzeuge und Kartenobjekte ohne fremdes Kennzeichen. |

## 8. Statische Bildreferenz und Liefergrenzen

Die Form- und Materialtafel ist als **tatsächlich original generierte** Materialreferenz [references/05-vehicles-props.png](references/05-vehicles-props.png) geliefert.

Die AI-Tafel ist ausdrücklich nicht maßhaltig; einzelne Front-/Signal-/Propdetails sind keine freigegebenen Master. Die exakte Vorlage sind dieses Kapitel und [das Fahrzeugkonstruktionsblatt](references/05-vehicles-construction.html) samt [PNG](references/05-vehicles-construction.png). Der genaue Bildauftrag steht in [evidence/vehicle-image-brief.md](evidence/vehicle-image-brief.md).

Sie ist eine dokumentarische Referenzansicht, kein Screenshot, keine Demo-App, kein Runtimeasset und kein Nachweis eines importierten 3D-Modells.

In diesem Teilkapitel werden **kein Modell, keine Textur, keine Animation und keine Audiodatei** geliefert oder erfunden behauptet.

Die Familien- und Objektbezeichnungen, Maße, Materialzuordnung und Bildbrief bleiben Empfehlungen bis zur ausdrücklichen Freigabe.

## 9. Fachbezug und feste Grenzen

| Grundlage | Übernommene Grenze |
|---|---|
| [Konzept 07 – Züge, Welten und Reisekarte](../../../Stammstrecken_Puzzle_Konzept_00-15/07_Zuege_Welten_und_Reisekarte.md) | Züge und Betriebsobjekte sind gezielte, rein kosmetische Betriebsmittel; der Schienenersatzverkehr bleibt zentral sichtbar. |
| [Konzept 10 – Visuelle Identität und Stammstrecke](../../../Stammstrecken_Puzzle_Konzept_00-15/10_Visuelle_Identitaet_und_Stammstrecke.md) | Premium-Miniaturwelt, sofort vertraute deutsche Betriebslagen-Anmutung, eigene Front und keine offizielle Kennzeichnung. |
| [Konzept 13 – Ökonomie und Balancing](../../../Stammstrecken_Puzzle_Konzept_00-15/13_Oekonomie_und_Monetarisierungs_Balancing.md) | Keine Preis-, Freischalt- oder Verkaufsaussage in dieser Gestaltungsausarbeitung; Kosmetik liefert nie einen Rätselvorteil. |
| [Content Catalogs v0.5](../../../ARCHITECTURE/CONTENT_CATALOGS.md) | Kein Eintrag, keine ID und kein `PRODUCT_APPROVED`-Anspruch wird aus dieser Dokumentation abgeleitet; DRAFT- und Produktionsgrenzen bleiben bestehen. |
| [Gemeinsamer Owner- und Designbrief](evidence/OWNER_CONTEXT_AND_DESIGN_BRIEF.md) | Gemeinsame Tokens und IBM-Plex-Vorschlag werden geteilt; keine Ersatzpalette, keine neue Schriftentscheidung und keine Runtimepipeline. |

**Nicht Gegenstand dieses Kapitels:** Rechtsfreigabe, finale Zeichen-/Markenprüfung, Produktionsmodellierung, Audio, Preisgestaltung, Katalogauthoring, Freischaltlogik, Puzzle- oder Kartenmechanik.
