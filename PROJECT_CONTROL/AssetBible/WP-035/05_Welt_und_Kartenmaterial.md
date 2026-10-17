# 05 – Cartoon-Welt und Kartenmaterial

**Teil E · korrigierte konkrete Empfehlung. Cartoon-/Comic-Stil und glasklare Deutsche-Bahn-Anspielung sind direkte Ownerfestlegungen.**

## 1. Bestand und Maßgeblichkeit

Die [Owner-Stilkorrektur](evidence/OWNER_STYLE_CORRECTION_2026-10-17.md) gilt ohne Realismus-Ausweichinterpretation. Die neue [Cartoon-Weltentafel](references/06-worlds-cartoon.png) zeigt die fünf bestehenden Räume. Keine Modellbahnfotografie, PBR-Materialtafel, realistische Schotter-/Beton-/Blatttextur oder glänzende Plastikproduktaufnahme.

Fünf Abschnitte × vier Routen × zwölf Rätsel bleiben unverändert. Kartenring, Weltweiche, Brücke und Bus führen keine Puzzlemechanik ein. Die Zugfahrt folgt ausschließlich dem tatsächlich gültigen A–B-Pfad. Alle nachstehenden Maße/Kompositionen sind grafische Vorschläge, keine Rendererwahl, Laufzeit-/Importregel oder Freigabe des konkreten Entwurfs.

## 2. Gemeinsame grafische Konstruktion

| Feld | Konkrete Empfehlung |
|---|---|
| Perspektive | Klare gezeichnete 2.5D-/isometrisch wirkende Spielwelt. Aufsicht als Zeichnungshilfe, keine fotografische Kamerainszenierung und kein neuer technischer Renderer. |
| Formensprache | Stämmige Quader, stumpfe Facetten, große einfache Bogen-/Trägerformen; bewusst verkürzt und überdimensioniert, nicht technisch maßstabsgetreu. |
| Kontur | Kräftige Anthrazit-Außenlinie; innere Fuge etwa halb so stark. Kleindetails entfallen vor der Silhouette. |
| Schattierung | Zwei bis drei klar abgegrenzte Farbflächen pro Material, breite grafische Kontaktschatten. Kein weiches fotorealistisches Ambient Occlusion. |
| Staub/Patina | Wenige breite entsättigte Farbpatches und große Platten-/Steinfugen; keine Noise- oder Mikromaterialtapete. |
| Fahrzeugmodul | H = Wagenkastenhöhe; L=2,60H und W=0,90H aus Teil D. Rote Hauptfläche, helle Türen/Bauchzone, dunkle Dach-/Glas-/Laufwerkgruppen. |
| Raumanker | Zug und Bahnsteig als lesbare Größenanker. Schilder und Amtsobjekte dürfen bewusst zu groß sein; die Übertreibung ist die trockene Pointe. |
| Detailhierarchie | Ein klares Hauptmotiv je Raum, wenige funktionale Sekundärobjekte. Der aktive Puzzlerasterraum bleibt davon frei. |

## 3. Farbe und Bahn-Wiedererkennung

**Die Deutsche-Bahn-Anspielung ist nicht optional:** rote Nahverkehrszüge, helle Tür-/Bauchzonen, anthrazitfarbene Dach-/Fenstergruppen, deutsche Bahnsteigtafeln, verdichtete Betriebslageninformation, rot-weiße Absperrungen, umständliche Wegweiser und SEV bilden zusammen die Identität. Eigenständige Fronten, eigene Beschilderung, fiktive Meldungen und eigene Karte; keine offiziellen DB-Kennzeichen oder übernommenen Screens.

Das UI-/Puzzle-Tokensystem bleibt unverändert. Im Raster: Braun konkrete Gleisform, Grau offene Belegung, Rot-X Leerannahme und Gelb ausschließlich Auswahl. **Illustrationsfarben sind nicht automatisch UI-Token:** gedämpftes Ocker auf einem gezeichneten Umwegschild, Schieferblau einer Bahnsteigtafel oder entsättigte Vegetation erzeugen keinen Auswahl-, Erfolgs- oder Fehlercode. Dieser Unterschied hebt die frühere eigene pauschale Beschränkung sämtlicher Weltfarben auf zehn UI-Token auf; die Puzzle-Semantik wird dadurch nicht verändert.

Material wird zeichnerisch vermittelt: ein Betonblock durch große Fuge und Schattenfläche, Holz durch wenige breite Linien, Metall durch Kontur und eine flache Lichtkante. Die alten numerischen PBR-Rauheitsziele entfallen vollständig.

## 4. Kontakt und Lesbarkeit

Gleise und Fahrzeuge müssen sichtbar zusammenpassen; kein schwebender Zug, keine abgeschnittene Verbindung. Stämmige Radgruppen liegen auf der dargestellten Schiene. Schatten sind grafische Bodenanker, keine physikalische Simulation. Figuren in der Illustration sind Hintergrundsituationen und kein beauftragter neuer Spielfigurenkatalog. Die klare A–B-Strecke darf niemals von Weltobjekten verdeckt oder durch eine attraktivere dekorative Kurve ersetzt werden.

## 5. Zustände und Dramaturgie

| Zustand | Grafische Regel |
|---|---|
| Aktives Rätsel | Ruhige dienstliche Arbeitsfläche: Kopf, Randzahlen, sechs Formen und optionale Notizen. Keine Weltkulisse im Raster. |
| Gültiger Abschluss | Hilfen zurücknehmen, den tatsächlichen braunen Pfad hervorheben; kein Richtigkeitsfeedback für offene Einzelannahmen. |
| Zugfahrt | Die klare Cartoon-Zugfamilie fährt dieselbe A–B-Verbindung. Volumen/Kulisse rahmen die Leistung, keine neue Strecke. |
| Ergebnis | Erst nach Ankunft an B; dienstliche Typohierarchie und vorhandene Daten, keine neue Wertung. |
| Betriebslage-Karte | Reduzierte fiktive Netzstabilisierung mit gezeichneten Bau-/SEV-Kontexten; keine reale Netz- oder Liveauskunft. |
| Betriebswerk | Lesbare cartoonhafte Züge/Objekte auf ruhigem Grund, keine photorealistische Produktvitrine. |

## 6. Wiederverwendbare Modulfamilien

Die vorhandenen Modulnamen werden als Zeichnungsnotation weiterverwendet; keine Runtimeadressen oder neue Inhaltsmengen.

| ID | Eigene Cartoon-Konstruktion |
|---|---|
| MOD-TRK | Dicke braune Schienen mit wenigen breiten Schwellen; große einfache Kurve statt Ballastmikrostruktur. |
| MOD-PLT | Breiter heller Bahnsteigblock, wenige große Platten und kräftige dunkle Unterkante. |
| MOD-TUN | Stumpfer großer Portalbogen, wenige Stein-/Betonblöcke, flache dunkle Öffnung. |
| MOD-SIG | Schwerfälliger Mast mit übergroßem rechteckigem Kopf und kräftigem Sockel. |
| MOD-BAU | Eine klar abgegrenzte kantige Grube, dicke rot-weiße Baken/Zäune und wenige gezeichnete Baustoffblöcke. |
| MOD-WEG | Breiter kantiger Fußweg mit wenigen großen Richtungswechseln und überdimensionierter Beschilderung. |
| MOD-BRG | Übergewichtige Träger, wenige breite Streben, dicke Pfeiler; keine filigrane Stahlrekonstruktion. |
| MOD-DEP | Kurzes geordnetes Abstellgleis mit rotem Zugteil und ein bis zwei einfachen Amtsschränken. |
| MOD-ERS | Winziges Wartehäuschen, riesiges SEV-Schild, blockiger roter/heller Bus und deutlich umständlicher Fußweg. |
| MOD-KAR | Ruhiger grafischer Spielkartenknoten; feste/spätere Verbindung, Störung und Fortschritt bleiben vorhandene Kartenbedeutungen. |

## 7. Fünf Abschnittsbriefe

| Abschnitt | Raumkomposition | Sichtbare deutsche Bahn-Alltagspointe |
|---|---|---|
| Stammstrecke | Großer stumpfer Tunnel, drei Bahnsteige, dichtes Gleis-/Meldetafelgefüge, rote Nahverkehrszüge. | Zu viel korrekt angeordnete Information in zu wenig Bahnhof; vertrauter enger Betriebsknoten. |
| Bauabschnitt Mitte | Eine große kantige Grube, zwei provisorische Umwege, dicke rot-weiße Zäune und übergroßes Vorübergehend-Schild. | Die Provisorik ist sorgfältiger verwaltet als das Weiterkommen. |
| Außenast ohne Halt | Viel flache freie Bodenfläche, kleiner entfernter Bahnsteig, stämmiger roter Zugteil, wenige Gewerbe-/Buschblöcke. | Alles sieht ordnungsgemäß aus, der Anschluss ist trotzdem weit weg. |
| Umleitung über Irgendwo | Zwei versetzte überbaute Brücken, lange lesbare Rückführung und übergroße Richtungswegweiser. | Behördenhafte Präzision organisiert den unvernünftig langen Weg. |
| Schienenersatzverkehr | Riesiges SEV-/Ersatzhalt-Schild, winzige Wartefläche, blockiger Bus, rot-weiße Absperrung, langer Fußweg. | Unverwechselbar der zentrale deutsche Pendlerfrust; nicht nur ein beliebiges Fahrzeug am Rand. |

## 8. Routenbriefe 01–04 – Stammstrecke

| Route | Exakt vorhandene Zieländerung | Benötigte Module und finale Kartenkomposition |
|---|---|---|
| 01 · Planmäßiger Ausnahmebetrieb | „Der erste Tunnelabschnitt ist wieder offen.“ | `MOD-TUN` + 1×`MOD-TRK` + erster `MOD-KAR`; kurzer `ink`-Kartenteil endet an rotem Engpass, Abschluss färbt nur dieses Segment freigegeben. |
| 02 · Signalstörung im Feierabendverkehr | „Ein Signalabschnitt leuchtet wieder, was niemand erwartet hat.“ | `MOD-SIG` + 1×`MOD-TRK` + `MOD-PLT`; ein Statusmast steht zwischen erstem und zweitem Bahnsteig, ein neuer gerader Kartenabschnitt verbindet ihn. |
| 03 · Anschluss fährt planmäßig ohne Sie | „Ein Seitenbahnsteig wird wieder verbunden.“ | 1×`MOD-PLT` seitlich + 90°-Lesekurve aus `MOD-TRK`; die Karte ergänzt einen kurzen Abzweig zum Seitenbahnsteig, ohne eine neue Route zu öffnen. |
| 04 · Fahrplan wird fortlaufend angepasst | „Der erste Hauptstrang ist vollständig freigegeben.“ | `MOD-PLT` ×3 + `MOD-TUN` + `MOD-SIG`; alle vier Routenanteile bilden einen zusammenhängenden horizontalen Hauptstrang durch den engen Knoten. |

## 9. Routenbriefe 05–08 – Bauabschnitt Mitte

| Route | Exakt vorhandene Zieländerung | Benötigte Module und finale Kartenkomposition |
|---|---|---|
| 05 · Baumaßnahme von überschaubarer Dauer | „Ein provisorischer Bahnsteig erhält eine funktionierende Verbindung.“ | `MOD-BAU` + 1×temporäres `MOD-PLT` + `MOD-WEG`; ein kurzer Umfahrungsstrang landet sichtbar an der provisorischen Kante. |
| 06 · Eingleisiger Betrieb aus Überzeugung | „Ein Engpass kann wieder befahren werden.“ | `MOD-BAU`-Rand + 1×`MOD-TRK` + `MOD-SIG`; nur ein klarer enger Durchlass wird freigegeben, keine Weiche oder eingleisige Puzzle-Sonderregel. |
| 07 · Stellwerk meldet sich später | „Ein Signal- und Stellwerksabschnitt wird wieder verständlich.“ | `MOD-SIG` ×2 + `MOD-WEG` + `MOD-TRK`; zwei Masten rahmen den freigegebenen Abschnitt, der Kartenstrang bleibt gerade und lesbar. |
| 08 · Umstieg am Bahnsteig der Unklarheit | „Der Bauabschnitt Mitte ist als Kartenring stabilisiert.“ | `MOD-BAU` + `MOD-PLT` ×2 + `MOD-WEG` ×2; vier bestehende Routenanteile schließen einen Kartenring um die Grube. Der Ring ist Kartenbild, nicht Puzzle-Schleife. |

## 10. Routenbriefe 09–12 – Außenast ohne Halt

| Route | Exakt vorhandene Zieländerung | Benötigte Module und finale Kartenkomposition |
|---|---|---|
| 09 · Halt entfällt aus betrieblichen Gründen | „Der erste Außenbahnsteig wird wieder anfahrbar.“ | 1×`MOD-PLT` am Beginn einer 36-Module-Außenachse + `MOD-KAR`; der Kartenstrang verlässt das Zentrum sichtbar. |
| 10 · Wagenreihung mit Überraschungseffekt | „Ein abgestellter Zugteil wird wieder sinnvoll eingeordnet.“ | `MOD-DEP` + seitliches `MOD-TRK`; das parkende, nach Teil D proportionierte Zugteil bleibt Maßstabsobjekt neben, nie auf dem freigegebenen Kartenstrang. |
| 11 · Zugteil fährt heute eigenständig | „Eine Abzweigung zum Depot wird stabilisiert.“ | `MOD-DEP` + 90°-`MOD-TRK`-Lesekurve + `MOD-SIG`; ein einzelner kurzer Kartenabzweig endet im Depotknoten. |
| 12 · Anschluss ist leider weitergereist | „Der Außenast ohne Halt ist als eigenständiger Kartenstrang offen.“ | `MOD-PLT` + `MOD-DEP` + lange `MOD-TRK`-Achse; alle vier Anteile erscheinen als separater, nicht geschlossener Außenstrang. |

## 11. Routenbriefe 13–16 – Umleitung über Irgendwo

| Route | Exakt vorhandene Zieländerung | Benötigte Module und finale Kartenkomposition |
|---|---|---|
| 13 · Kurzfristig über einen Umweg | „Die erste Umfahrungsbrücke ist freigegeben.“ | 1×`MOD-BRG` + `MOD-TRK`; erster hoher Träger überspannt die direkte Achse, der Kartenstrang macht seinen ersten langen Bogen. |
| 14 · Mehr Reisezeit als geplant vorgesehen | „Ein längerer Weg wird auf der Karte wieder zusammenhängend.“ | 1×`MOD-BRG` + 2×`MOD-TRK` + Wegweiser; zwei entfernte Teilstücke erhalten eine einzige sichtbare Verbindung. |
| 15 · Bitte prüfen Sie die Route erneut | „Eine Kartenverbindung gewinnt wieder Orientierungspfeile.“ | `MOD-WEG`-Wegweiser ×3 + `MOD-SIG`; drei Pfeile folgen derselben fiktiven Kartenrichtung, keine echten Orts-/Linienbezeichnungen. |
| 16 · Die Alternative zur Alternative | „Der Abschnitt Umleitung über Irgendwo ist als Schleife auf der Karte sinnvoll geschlossen.“ | `MOD-BRG` ×2 + 4×`MOD-TRK` + Wegweiser; nur die Karte zeigt einen geschlossenen Rückführungsring. Kein Puzzle enthält diese Schleife oder eine Kreuzung. |

## 12. Routenbriefe 17–20 – Schienenersatzverkehr

| Route | Exakt vorhandene Zieländerung | Benötigte Module und finale Kartenkomposition |
|---|---|---|
| 17 · Ersatzhaltstelle in planbarer Entfernung | „Eine provisorische Haltestelle erhält erstmals einen echten Gleisanschluss.“ | `MOD-ERS` + kurzer `MOD-TRK`-Ansatz + `MOD-WEG`; der feste Strang endet erstmals materiell an der Ersatzhaltestelle. |
| 18 · Bus kommt, sobald er kann | „Eine gestrichelte Kartenzone wird wieder zu einer festen Strecke.“ | `MOD-ERS` + gestrichelter `MOD-KAR`-Zwischenstatus + `MOD-TRK`; Abschluss ersetzt nur die Kartenstriche durch festen Strang, ohne Busmechanik. |
| 19 · Anschluss an den Ersatzverkehr | „Der letzte große Umsteigeknoten wird verbunden.“ | `MOD-ERS` + 2×`MOD-PLT` + `MOD-WEG` ×2 + `MOD-SIG`; zwei materialisierte Wechselpunkte werden durch einen klaren Kartenknoten verbunden. |
| 20 · Betrieb bis auf Weiteres stabil | „Die gesamte Season-1-Karte wird befahrbar; Betriebsrevision und Dauerbaustelle erscheinen.“ | Reprise aller Abschnitts-Hero-Module: `MOD-TUN`, `MOD-BAU`, `MOD-DEP`, `MOD-BRG`, `MOD-ERS`; ein durchgehender, abschnittsweise lesbarer Kartenstrang trägt die symbolische Abschlussfahrt. |

## 13. Tatsächliche Referenzen und technische Grenze

- [Neue Cartoon-Weltentafel](references/06-worlds-cartoon.png), neu erzeugt aus [world-image-brief.md](evidence/world-image-brief.md), ohne die abgelehnten Bilder als Eingabe.
- [Weltkonstruktionsquelle](references/06-world-construction.html) und [PNG](references/06-world-construction.png): reduzierte reproduzierbare Form-/Kontakt-/Farbzuordnung passend zu Teil D.
- [20-Routenübersicht](references/07-route-map.html): vorhandene Routenziele, keine Änderung der Karte oder Content-Bible.

Die generierte Tafel ist eine **Cartoon-Stil-/Kompositionsreferenz**, keine orthografisch exakte Modellzeichnung. Gezeichnete Beispieltexte, Figuren und Zahlen sind illustrative Ton-/Kontextbeispiele; keine finale Textbibliothek, realen Fahrplandaten oder neuen Produktionsaufträge. Der Stil darf daraus nicht wieder in Pseudorealismus umgedeutet werden.

Unity-/Addressables-Gruppen, Adressmuster, .meta, Importpresets und spätere Lizenz-/Localization-Arbeit folgen unverändert der vorhandenen Architektur. Keine Rendererwahl, Performancegrenze, neue Pipeline, Produktionsdatei, Audio oder Animation werden in diesem Entwurf geliefert. Der allgemeine Stil und die zentrale DB-Anspielung sind bestätigt; konkrete Konstruktionen und neue Ausführung sind noch nicht automatisch angenommen.
