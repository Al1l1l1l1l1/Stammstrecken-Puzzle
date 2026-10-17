# 04 – Cartoon-Züge, Lackierungen und Betriebsobjekte

**Teil D · korrigierter Entwurf zur konkreten Annahme. Der allgemeine Stil und die glasklare Deutsche-Bahn-Anspielung sind ausdrücklich vom Owner festgelegt.**

## 1. Maßgebliche Richtung und Fehlerkorrektur

Die [Owner-Stilkorrektur vom 17.10.2026](evidence/OWNER_STYLE_CORRECTION_2026-10-17.md) ist maßgeblich: **stilisierter Cartoon-/Comic-Spielstil, plump, staubig, behördenhaft; glasklare Deutsche-Bahn-Anspielung als Kern.** Die alte Auslegung als präzises, pseudo-/fotorealistisches Modellbahnprodukt ist verworfen. Die neue [Fahrzeug-/Objekttafel](references/05-vehicles-props-cartoon.png) zeigt die korrigierte Richtung.

Die Vorschläge dieses Kapitels sind keine finalen Katalog-IDs, Preise, Unlocks, Assetadressen, Produktionsimporte oder Rechtsfreigaben. Kosmetik verändert keine Rätselschwierigkeit, Hinweise, Sterne oder Zeitwertung.

## 2. Ein gemeinsamer Cartoon-Formvertrag

| Baustein | Konkreter Vorschlag | Zweck |
|---|---|---|
| Primäre Silhouette | Verkürzter, stämmiger Nahverkehrswagen; große einfache Flächen, stumpfe Facetten und kräftiges dunkles Dach. | Sofort als deutscher S-Bahn-/Regionalzug lesbar, nicht als Fantasy-, Dampf- oder amerikanischer Zug. |
| Relation | Wagenkastenhöhe H; Standardlänge L=2,60H, Breite W=0,90H. Zwischenwagen 0,92L. | Bewusst plumpe, nicht maßstäblich-realistische Familie. H ist Zeichnungseinheit, kein Metermaß oder Collider. |
| Kanten | Außenkontur 0,025H; innere Tür-/Glas-/Materialtrennung 0,0125H. Auf kleinen Darstellungen werden innere Details zuerst reduziert. | Klare mobile-game-art-Lesbarkeit; kein Gravur- oder Nietengewimmel. |
| Volumen | Zwei bis drei klar begrenzte Farbflächen pro Hauptmaterial: Grundfläche, Schattenseite, sparsame Lichtkante. | Comic-/Cel-Shading-Eindruck, keine weichen fotorealistischen Reflexe. |
| Ecken | Dach-/Kastenfacette 0,08H; Tür-/Fensterecken 0,035H. | Stumpf und robust, nicht filigran und nicht kugelig-kleinkindlich. |
| Laufwerk | Große volle Radscheiben, Durchmesser 0,26H; sichtbare Untergestellgruppe als zwei bis drei dunkle Blöcke. | Zeichnerische Masse und Rhythmus statt mechanischer Modelltreue. |
| Wagenübergang | Dunkler Block/Faltenbalg mit 0,08L Zwischenraum; höchstens drei gezeichnete Falten. | Modulare Wagenreihung, sichtbar robust und etwas unbeholfen. |
| Blickrichtung | Frontform, Fenstergruppe und Lichtpaar geben die Richtung. | Keine Logos nötig; keine Augen, Münder oder vermenschlichten Zuggesichter. |

**Primär gelesen wird:** roter Nahverkehrszug → Bahn-Alltag → hochwertige Cartoon-Spielwelt. Eigenständigkeit darf nicht mehr über eine überwiegend neutrale Offwhite-Lackierung mit kaum sichtbaren roten Sprenkeln erkauft werden.

## 3. Eigenständige Front und Seitenkonstruktion

| Element | Konstruktionsvorschlag | Grafische Behandlung |
|---|---|---|
| Front | Stumpfe rote Stirnfläche mit zwei getrennten breiten Rechteckscheiben. Scheiben je 0,30W × 0,25H, Zwischensteg 0,08W. | Große dunkle Flächen mit höchstens einer gezeichneten Lichtkante; keine echten Glasreflexe. |
| Dach | Anthrazitfarbene Kappe, 0,14H; eine große Technikbox plus höchstens zwei Nebenblöcke. | Breite Flächen, keine detaillierten Dachaggregate. |
| Leuchten | Kleine voneinander getrennte Rechtecke 0,12W × 0,075H; breit außen angeordnet. | Helle Fläche mit dunkler Fassung; keine emotionalen Augenbrauen oder reale Baureihen-Lichtsignatur. |
| Frontunterkante | Stämmiger anthrazitfarbener Sockel, 0,18H; große einfache Kupplungsgruppe. | Zwei bis drei Blöcke statt Schrauben, Leitungen und filigraner Mechanik. |
| Türen | Zwei helle Doppeltürgruppen, Achsen bei 0,27L und 0,73L; je 0,14L × 0,60H. | Große, eindeutig lesbare Fenster und mittlere Türfuge. |
| Seitenfenster | Vier große geordnete Rechteckfelder zwischen/neben den Türgruppen; keine Vielzahl dünner Hochfenster. | Innenkontur schwächer als Außenkontur; dunkel, ohne realistischen Innenraum. |
| Hauptlack | **Verkehrsroter Wagenkörper**, helle Türen, breite eigene Offwhite-Bauchzone, anthrazitfarbene Dach-/Glas-/Laufwerkgruppen. | Bewusst sofort erkennbare Deutsche-Bahn-/S-Bahn-/Regionalbahn-Anspielung. |
| Eigenes Lackdetail | Eine stumpfe diagonale Ausklinkung am eigenen hellen Bauchband und abweichende Tür-/Fensterproportionen. | Keine nachgezeichnete konkrete Baureihe oder offizielle Livery-Datei. |
| Eigenes Zeichen | Falls ein sichtbares Fahrzeugzeichen benötigt wird: der eigene unterbrochene Gleisknoten aus Kapitel A, nicht ein zufällig erzeugtes Emblem. | Keine DB-Buchstaben, Betreiberkennzeichnung oder Kooperationsbehauptung. |

Die [Fahrzeugkonstruktionsquelle](references/05-vehicles-construction.html) und ihre [PNG-Ansicht](references/05-vehicles-construction.png) setzen die vorgeschlagene Relation L/H/W und die reduzierte Front-/Tür-/Lackkonstruktion reproduzierbar um. Die generierte Cartoon-Tafel ist die Stil-/Volumenreferenz, keine technische Orthogonalzeichnung.

## 4. Farbe, Schatten und grafischer Staub

| Rolle | Basis | Zeichnerische Umsetzung |
|---|---|---|
| Wagenkörper | `brand-red #C83D45` | Große rote Hauptfläche; eine klar begrenzte dunklere Schattenseite. |
| Türen / Bauchzone / Beschilderung | `surface #F4F2EE`, Lichtkante `paper #FFFFFF` | Warmes Offwhite statt sterilem Glanzlack. |
| Kontur / Dach / Glas / Laufwerk | `ink #20272C` | Dunkle, ruhige Masse; kleine Lichtflächen nur als gezeichnete Formen. |
| Gleis / Bank / Bodenstaub | Warmes Braun und entsättigte Sand-/Steinableitungen | Große flache Flecken und wenige Fugen; kein fotografisches Kies-, Rost- oder Schmutzrauschen. |
| Weltbeschilderung | Bahnsteigtafeln dunkel mit heller Schrift; Baustellen-/Umwegtafeln dürfen eigenes gedämpftes Ockergelb haben | **Nicht** der semantische Auswahl-Token. Dekorative Farbe verändert keine Puzzle-/UI-Bedeutung. |

Die vier Puzzlefarben behalten ausschließlich im aktiven Raster ihre festgelegte Bedeutung. Hintergrundvegetation, Tafeln oder Baustellenfahrzeuge sind illustrative Weltfarben, keine neuen Spielzustände und keine Änderung des UI-Tokensystems. Das frühere pauschale Verbot zusätzlicher Illustrationsfarben war eine eigene Überbeschränkung und ist aufgehoben.

**Verboten als Stilmittel:** PBR-Rauheitswerte als Bildziel, ray-traced Metall, fotorealistische Glas-/Beton-/Schottertexturen, Schärfentiefe und Kamerafotografie. „Staubig“ bedeutet grafische Patina und stumpfe Amtlichkeit, nicht Schmutzsimulation oder düsteren Verfall.

## 5. Fünf Familien innerhalb derselben Formensprache

Die Namen bleiben Arbeitsnamen, keine finalen Shop-SKUs oder Freischaltungen.

| Familie | Reihung | Sichtbare kosmetische Variation |
|---|---|---|
| Kompakter Starttriebzug | A–M–A | Klarste rote Grundlackierung und ruhiger heller Tür-/Bauchbandrhythmus. |
| Modulare Ersatzgarnitur | A–T, A–T–A oder A–T–T–A | Ein etwas anders angeordneter heller Tür-/Seitenblock; stämmige, akkurat zusammengestellte Provisorik. |
| Funktionierender Zugteil | A–A | Zwei gedrungene Endwagen, auffällig kurzer Zugverband, dieselbe rote Identität. |
| Wagenreihung | A–M–T–A | Eine geordnet, aber ungewohnt versetzte Tür-/Fenstergruppe. Kein physischer Schaden. |
| Anschlusswunder-Express | A–M–A | Sauberere helle Bauchzone und kompaktere Dachtechnik; keine Schnellzugnase und kein Geschwindigkeitsspielvorteil. |

A/M/T sind Zeichnungsnotation, nicht sichtbare Bahnbeschriftung. Der Humor liegt in ungewöhnlich korrekt verwalteter Provisorik und der seltenen funktionierenden Verbindung, nicht im Leid von Menschen oder alberner Fahrzeugmimik.

## 6. Reduktion für Karte, Puzzlefahrt und Betriebswerk

| Ort | Konkrete grafische Reduktion | Unverändert |
|---|---|---|
| Betriebslage-Karte | Ein kurzer roter Wagen, helle Türen/Bauchband, dunkle Kappe, zwei große Rad-/Schattenblöcke; kleinste Beschriftungen entfallen. | Dieselbe eigene Familie, keine zweite Lackwelt. |
| Puzzlefahrt | Körperlänge 0,78C und Breite 0,27C als passender Reduktionsvorschlag; kräftige Silhouette, vereinfachte Tür-/Glasflächen. | Ausschließlich der tatsächlich gültige A–B-Pfad; keine alternative Route. |
| Betriebswerk | Klare Drei-Viertel-Zeichnung mit zwei bis drei Schattentönen, kompletter Tür-/Fenstergruppe und etwas übergewichtiger Kupplung. | Kosmetik, keine automatische Preis-/Kauf-/Unlock-Bestätigung. |

Diese Reduktionen sind grafische Vorschläge, keine Rendererwahl und keine neuen Laufzeit-/Touch-/Performanceverträge.

## 7. Betriebsobjekte als Cartoon-Baukasten

| Objekt | Konkrete eigene Cartoonform | Trockene Bahn-Alltagsfunktion |
|---|---|---|
| Signal | Stämmiger Mast, übergroßer rechteckiger Kopf, eine klare Lichtöffnung und blockiger Sockel. | Betriebslage-Kulisse; kein auswählbares Puzzletool. |
| Weltweiche | Zwei dicke braune Schienen, stark vereinfachte Zunge und überdimensionierter Stellkasten. | Nur Weltdekoration; keine neue Gleisform oder Abzweigmechanik. |
| Bahnsteig | Dicke helle Kante, wenige große Platten, dunkle Unterkante. | Wiedererkennbarer deutscher Bahnhalt, ohne realen Stationsnachbau. |
| Bank | Drei breite Holzlatten, zwei kräftige dunkle Bügel, leicht schwerfällige Proportion. | Stilles Pendlerobjekt, keine eigene Spielhandlung. |
| Meldetafel | Großer dunkler Informationskasten, helle geordnete Zeilen, massive Pfosten. | Bahn-App-/Bahnsteig-Duktus; alle Beispieltexte selbst geschrieben. |
| Wegweiser | Für den kleinen Halt deutlich zu großes stumpfes Schild auf dickem Pfosten. | Überpräzise verwalteter Umweg; kein echter Ortsname. |
| Tunnelportal | Großer stumpfer Bogen aus wenigen groben Stein-/Betonblöcken, dicke Kontur, dunkle flache Innenöffnung. | Keine mikroskopischen Fugen und keine realistische Tunnelaufnahme. |
| Brücke | Überdimensionierte Balken, wenige große Streben und stämmige Pfeiler. | Absurd aufwendige Umleitung; keine Puzzlebrücke oder Kreuzung. |
| Ersatzhaltestelle | Winziges Wartehäuschen neben riesigem SEV-/Ersatzhalt-Schild. | Schienenersatzverkehr sofort als zentrale Pointe erkennbar. |
| Absperrung | Breite rot-weiße Felder, dicke Füße, wenige große Leuchten. | Präzise organisierte Provisorik, keine Kollisions- oder Sperrmechanik. |
| SEV-Bus | Gedrungener rot-/heller Quader, große dunkle Fenster, dicke Radscheiben. | Neben dem Gleis, niemals als Ersatzpfad im Rätsel. |

Die Illustration darf die Situation mit vereinfachten Hintergrundpersonen erklären; daraus entstehen keine beauftragten Spielfiguren, Produktionskataloge oder neuen Inhaltsumfänge. Kein eigenständiges Personen-Asset wurde in diesem Block geliefert.

## 8. Tatsächliche Referenzen und Nutzungsgrenzen

Die neue [Cartoon-Fahrzeug-/Objekttafel](references/05-vehicles-props-cartoon.png) ist aus dem [korrigierten Bildbrief](evidence/vehicle-image-brief.md) neu erzeugt; die abgelehnten Realismusbilder wurden **nicht** als Eingabe verwendet und sind aus der aktuellen Lieferung entfernt.

Die generierte Titeltypografie ist eine Illustrationsbeschriftung, nicht die exakte Produktwortmarke aus Kapitel A. Das generierte Frontemblem ist keine Produktionszeichenfreigabe; bei einer Ableitung wird ausschließlich der eigene Gleisknoten verwendet. Beispielmeldungen/Zahlen sind Layout- und Tonbeispiele, keine finale Textbibliothek, reale Fahrplandaten oder Preis-/Zeitvorgabe. Diese Abgrenzungen erlauben nicht, den jetzt klar cartoonhaften Stil wieder in Realismus umzudeuten.

Geliefert sind Quellen und statische Ansichten, **keine Modelle, Texturen, Animationen, Audiodateien oder Unityimports**. Die allgemeine Stilrichtung ist bestätigt; die konkrete neue Ausführung bleibt anhand der Zeichnungen zur Annahme vorgelegt.

## 9. Quellen und feste Grenzen

- [Owner-Stilkorrektur](evidence/OWNER_STYLE_CORRECTION_2026-10-17.md): Cartoon-/Comic-Stil und glasklare Deutsche-Bahn-Anspielung.
- [Konzept 07](../../../Stammstrecken_Puzzle_Konzept_00-15/07_Zuege_Welten_und_Reisekarte.md): rein kosmetische Züge und Betriebsobjekte.
- [Konzept 10](../../../Stammstrecken_Puzzle_Konzept_00-15/10_Visuelle_Identitaet_und_Stammstrecke.md) und [Konzept 11](../../../Stammstrecken_Puzzle_Konzept_00-15/11_Bahnfahrer_Lore_und_Satire_Leitplanken.md): sofort vertraute Bahn-Alltags-Satire, keine offiziellen Kennzeichen oder übernommenen Medien.
- [Konzept 13](../../../Stammstrecken_Puzzle_Konzept_00-15/13_Oekonomie_und_Monetarisierungs_Balancing.md): keine Preise, Unlocks oder Spielvorteile aus der Bible.
- [Content Catalogs](../../../ARCHITECTURE/CONTENT_CATALOGS.md): keine Produkt-/Import-/Releasefreigabe durch diese dokumentarische Lieferung.
