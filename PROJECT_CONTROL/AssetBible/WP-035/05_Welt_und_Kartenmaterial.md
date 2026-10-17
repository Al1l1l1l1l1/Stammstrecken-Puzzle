# 05 – Welt- und Kartenmaterial

**WP-035 · Teil E · Entwurfsstand zur ausdrücklichen Ownerfreigabe**
**Zweck:** Wiederverwendbare Modul-, Kompositions-, Material- und Zustandsbriefe für die vorhandenen fünf Season-1-Netzabschnitte und ihre 20 vorhandenen Routen.

> Alle konkretisierten Maße, Materialwerte, Kamerawinkel und Modulnamen in diesem Kapitel sind **Empfehlungen**, keine Produktregel, Rechtsfreigabe, Production-Freigabe oder Importanweisung. Sie ändern weder Umfang, Rätselkern noch die bestehende URP-2D/2.5D-Richtung.

## 1. Bindung an Bestand und Leseschlüssel

- Die unveränderte Season umfasst **fünf Abschnitte × vier Routen × zwölf Meldungen**; diese Ausarbeitung erfindet keine Welt, Route, Station oder Meldung hinzu.
- Jede Route erhält nur ihr vorhandenes Karten-Ziel aus der Content-Bible und eine empfohlene Zusammenstellung vorhandener Weltbaustein-Kategorien.
- Der Puzzlepfad bleibt eine einzelne, schleifenfreie A–B-Verbindung aus sechs Formen. Eine sichtbare Karten-Schleife ist ausschließlich Übersichtssymbolik und niemals eine Puzzleform.
- Der gewählte Zug fährt nach der gültigen Lösung **den tatsächlich gebauten A–B-Pfad**, nicht eine dekorative Kartenkurve oder einen voranimierten Ersatzweg.
- Die Karte bleibt eine reduzierte fiktive Betriebslage, keine Netzkarte, Fahrplanauskunft, Ticketansicht, Stationslogo-Sammlung oder Live-Daten-Darstellung.
- Die Begriffe Material, Licht und Modul beschreiben statische Referenzbilder und spätere Produktionsvorbereitung; sie behaupten keine bereits angelegten Runtimeassets.

**Fachgrundlage:** [Betriebslage-Karte und Betriebsmittel](../../../Stammstrecken_Puzzle_Konzept_00-15/07_Zuege_Welten_und_Reisekarte.md), [Visuelle Identität](../../../Stammstrecken_Puzzle_Konzept_00-15/10_Visuelle_Identitaet_und_Stammstrecke.md), [Season-1-Content-Bible](../../../Stammstrecken_Puzzle_Konzept_00-15/14_Season_1_Content_Bible.md) und [Content Pipeline](../../../ARCHITECTURE/CONTENT_PIPELINE.md).

## 2. Gemeinsame visuelle Konstruktionsregel

| Feld | Empfohlene, wiederverwendbare Vorgabe |
|---|---|
| Perspektive | Leicht räumliche 2.5D-Miniatur, orthografisch wirkend; Draufsicht um **35°** geneigt, Längsachsen bevorzugt in 30°/150° oder 0°/90°. Keine fotorealistische Kamerafahrt und keine neue Rendererwahl. |
| Referenzmaßstab | Für Referenzzeichnungen gilt **1 Weltmodul = H**, die gemeinsame Fahrzeughöhe aus Teil D (kein reales Metermaß). Ein Standardbahnsteigsegment ist 12 × 3 Module, ein gerades Gleissegment 6 Module lang; alle fünf Abschnitte teilen diese Rasterung. |
| Schienenquerschnitt | Zwei braune Schienenstränge, je 0,04 Module breit, Achsabstand 0,50 Module; Schwellen alle 0,20 Module, 0,90 × 0,08 Module. Kurven verwenden einen Innenradius von mindestens 4,0 Modulen. |
| Raumtiefe | Gleisoberkante liegt bei Z=0,18; Bahnsteigoberkante bei Z=0,52; Bahnsteigkante mindestens 0,45 Module seitlich von der Gleisachse. Diese drei Höhen bleiben abschnittsübergreifend identisch. |
| Zughülle | Standard-Endwagen: 3,18 × 0,72 × 1,00 Module; Mittelwagen 0,92 dieser Länge, identisch zu Teil D; Radlauf berührt die Gleisoberkante ohne sichtbaren Spalt. Parkende Zugteile verwenden dieselbe Hülle wie fahrende Züge. |
| Kartenmaßstab | Jeder Routenstrang belegt eine 4:1-Langform. Ein freigegebener Teilstrang ist in der statischen Referenz 8 logische Pixel breit; ein Störungsknoten 24 logische Pixel im Durchmesser. Dies sind keine Geräte-Mindestgrößen. |
| Detaildichte | Pro 12 × 12-Modul-Kachel höchstens ein Hero-Objekt, zwei Funktionsobjekte und sechs Kleinobjekte. Freie Bodenflächen bleiben lesbar; keine Kleindetailtapete. |
| Text/Zeichen | Keine realen Stationsnamen, Logos, Liniennummern, Fahrplanköpfe oder kopierten Piktogramme. Wegweiser tragen nur neue, kurze fiktive Layoutplatzhalter. |

## 3. Gemeinsame Material- und Tokenbindung

| Rolle | Ausschließlich zu verwendender gemeinsamer Token | Materielle Anwendung |
|---|---|---|
| Dienstlicher Akzent / Störung | `brand-red #C83D45` | Problemknoten, Bauzaun-Kappe, Warnbake und klar abgegrenzte Kartenstatusfläche; nie flächig als Bodenmaterial. |
| Informations- und Bahnsteigfläche | `surface #F4F2EE` / `paper #FFFFFF` | Saubere Bahnsteigplatten, Kartenhintergrund, helle Beschilderungsfelder und Rasterruhe. |
| Feste, freigegebene Infrastruktur | `ink #20272C` | Kartenstrang, Tunnelinnenkante, Pfosten, Brückenträger und zurückhaltende Kontur. |
| Konkrete Puzzle- und Abschlussgleise | `track-brown #805535` | Ausschließlich die sechs gesetzten Gleisformen und deren hervorgehobener echter Fahrpfad. |
| Offene Planungsannahme | `assumption-gray #727B82` | Nur graues Standardgleis als offene Belegungsnotiz; keine graue konkrete Kurve, kein Weltpfad. |
| Leerannahme | `empty-red #B42E39` | Ausschließlich optionales X im Raster, nie Warnschild- oder Objektfarbersatz. |
| Aktive Auswahl | `selection-yellow #F1C84B` mit `selection-edge #6C5717` | Nur Werkzeug- oder Feldselektion; nie Stern, Gewinn, freigegebene Strecke oder Kartenroute. |
| Trennlinie | `grid-line #CCD0D1` | Raster, Kartentrenner und feine technische Fugen. |

- Die gemeinsame Schriftfamilie bleibt **IBM Plex Sans** für UI/Zahlen und **IBM Plex Sans Condensed** für straffe Statusüberschriften; keine Alternativfamilie wird vorgeschlagen.
- Materialgefühl entsteht über Form, Kanten und kontrollierte Rauheit: Betonplatte 0,72, lackierte Bake 0,38, Schienenkopf 0,26, feiner Kies 0,84 als Referenzrauheit auf einer 0–1-Skala.
- Diese Werte sind Bild-/Materialreferenzwerte, keine beschlossenen Shader-, Importpreset- oder Performanceparameter.
- `track-brown` bleibt visuell der konkrete Gleisbeweis. Weltgleis und Kartenstrang dürfen daher nicht die Semantik von Grau, Rot-X oder Gelb übernehmen.

## 4. Kontakt-, Licht- und Maßstabsregeln

| Prüfung | Freigabepflichtige Bildregel |
|---|---|
| Bodenkontakt | Sockel, Zaunfüße, Bahnsteigkanten und Fahrzeuge sitzen mit höchstens 0,02 Modulen sichtbarem Abstand auf ihrer Trägerfläche; keine schwebenden Baken oder halb eingesunkenen Räder. |
| Gleiskontakt | Radlaufebene berührt Z=0,18; Schwellen liegen vollständig auf dem Schotterbett. Ein Zug deckt nie die Schienenverbindung so ab, dass der A–B-Pfad unlesbar wird. |
| Schichtabstand | Parallel liegende Spiel-, Karten- und Weltlinien erhalten mindestens 0,25 Module beziehungsweise 4 Bildschirm-Pixel Luft. Der sichere Leseraum um A/B misst mindestens 0,50 Module. |
| Hauptlicht | Ein weiches gerichtetes Oberlicht von links oben bei 35° Elevation; Schatten fallen nach rechts unten und bleiben höchstens 0,60 Module lang. |
| Füllung | Helle Flächen bleiben mit `surface`/`paper` lesbar; `ink`-Flächen dürfen höchstens 70 % der Bildfläche eines Ausschnitts belegen. Keine farbige Gegenlichtdramaturgie außerhalb der Tokens. |
| Kontakt- und AO-Eindruck | Kontaktzonen erhalten eine subtile, maximal 0,10 Module breite Abdunklung. Sie trennt Materiallagen, darf aber keine versteckte Spiellogik oder Richtung kodieren. |
| Maßstabsanker | Pro Kulissenausschnitt sind ein Zugfenster, eine Bahnsteigkante oder eine Tür als menschlicher Maßstabsanker sichtbar; Figuren sind nicht erforderlich. |
| Wiederholbarkeit | Wiederholte Objekte rotieren nur in 90°-Schritten; Bahnsteigsegmente, Zäune und Wegweiser rasten auf dem 1,0-Modul-Raster ein. |

## 5. Zustandsmodell: Puzzle, Fahrt, Karte und Betriebswerk

| Zustand | Sichtbare Regeln |
|---|---|
| Ruhiges Puzzle | Nur helle technische Arbeitsfläche, Randzahlen, A/B, sechs Formen und optionale Notizen. **Keine** Kulissenobjekte, Kartenausschnitte, Geld, Werbung oder Betriebswerk-Objekte im Raster. |
| Gültige Lösung | Graue und X-Notizen sowie technische Nebeninformation treten zurück. Der gebaute braune Pfad bleibt vollständig lesbar; Gelb endet mit der Auswahl und bewertet nicht die Lösung. |
| Zugfahrt | Rätselgeometrie wird als einzige befahrbare Schienenlinie räumlich lesbar. Der Zug startet am tatsächlichen A, fährt jede konkrete Gerade/Kurve in Reihenfolge ab und hält erst am tatsächlichen B. |
| Ergebnis | Erst nach erreichter B-Position erscheint Ergebnisfläche; die Welt darf einen ruhigen Freigabezustand zeigen, ersetzt aber keinen Teil der Fahrt. |
| Betriebslage-Karte | Abstrahierte Achsen, Knoten und Abschnittsmaterial; nur Routenfortschritt, Störung, gesperrte Verbindung und Freigabe. Sie zeigt nie Zellen, Randzahlen oder eine vermeintliche Puzzlelösung. |
| Betriebswerk | Ruhige materielle Präsentation auf `surface`/`paper`: Zug auf kurzem geradem Gleis, Betriebsobjekt auf 12 × 12-Modul-Präsentationsplatte, kein Kauf-/Freischaltversprechen in diesem Brief. |

## 6. Wiederverwendbare Modulfamilien

| ID | Modul | Konstruktion und Material |
|---|---|---|
| `MOD-TRK` | Gleisbasis | 6 × 3 Module, braune Schienen nach Abschnitt 2, Kiesbett 0,18 hoch; gerade, 90°-Kurve und Endstück als räumliche Präsentationsbausteine. |
| `MOD-PLT` | Bahnsteig | 12 × 3 × 0,52 Module, `surface`-Platten in 3 × 3-Modul-Fugen, `ink`-Kante; wahlweise mit 1,5-Modul-Wegweiserpfosten. |
| `MOD-TUN` | Tunnelportal | Öffnung 5,6 breit × 4,8 hoch × 2,0 tief, `ink`-Innenlaibung, `surface`-Außenkante; kein Schriftzug oder Monogramm. |
| `MOD-SIG` | Signal-/Statusmast | 0,35 × 0,35 × 3,6 Module, `ink`-Mast und ein 0,55-Modul-Statuskopf; der Kopf ist reine Kulisse, keine Rätselregel. |
| `MOD-BAU` | Baugrube und Sicherung | 12 × 12 Module, abgesenkte Mitte Z=−1,20, `brand-red`-Kappen an 1,0-Modul-Zaunsegmenten, zwei 1,5-Modul-Baken. |
| `MOD-WEG` | Provisorischer Weg | 2,0 Module breit, `surface`-Platten, `grid-line`-Fuge alle 1,0 Module, maximal eine rechtwinklige Richtungsänderung je 12-Module-Segment. |
| `MOD-BRG` | Brückenfeld | 12 × 4 × 1,20 Module, `ink`-Längsträger, `surface`-Deckrand, offene Unterseite für lesbare Überführung. |
| `MOD-DEP` | Depot-/Abstellgleis | 18 × 6 Module mit einem 3,18-Module-Standardwagen oder einer passend kurzen Garnitur, 1,0-Modul-Sicherheitsstreifen und maximal zwei Funktionsschränken. |
| `MOD-ERS` | Ersatzhaltestelle | 12 × 6 Module: 6 × 2-Modul-Dach, 2 × 4-Modul-Warteplatte, 1,5-Modul-Wegweiser und klar getrennte gestrichelte Kartenfortsetzung. |
| `MOD-KAR` | Kartenknoten | 8-mm-Kreis mit `brand-red` für aktuelle Störung, `ink` für freigegebenen Zielknoten, gedämpfte `grid-line`-Kontur für noch nicht erreichbare Knoten. |

## 7. Abschnittsbriefe – Raum, Komposition und Material

| Abschnitt | Raumkomposition | Pflichtmodule | Material- und Zustandsbild |
|---|---|---|---|
| Stammstrecke | Zentraler Tunnel mit **drei** 12×3-Bahnsteigen, zwei parallelen Gleisachsen und einem 18×18-Module engen Knoten. Tunnel belegt den linken oder oberen Bildrand, sodass die drei Plattformen sofort als Engstelle lesbar sind. | `MOD-TRK`, `MOD-PLT` ×3, `MOD-TUN`, `MOD-SIG`, einzelne `MOD-WEG`-Baken. | `ink`-Tunnelinnere, helle Bahnsteigplatten, punktuelles `brand-red` nur an Störung/Bake. Der fast vollständige Hauptstrang ist sichtbar, aber nie wie eine reale Station beschriftet. |
| Bauabschnitt Mitte | Eine offene 12×12-Baugrube versetzt die zuvor klare Mittelachse; zwei schmale 2,0-Module-Provisorien führen an ihr vorbei. Ein Kartenring entsteht nur als Überblicksstrang. | `MOD-BAU`, `MOD-WEG` ×2–3, `MOD-PLT`, `MOD-SIG`, `MOD-TRK`. | Raues Kies-/Beton-Verhältnis, kontrollierte `brand-red`-Zaunkappen, helle temporäre Gehplatten. Keine Baustellenmechanik im Puzzle. |
| Außenast ohne Halt | Oberirdische horizontale Distanz: ein Randbahnsteig links, Gewerbeblöcke als 6×6-Volumen rechts, dazwischen 36–48 Module offene Gleisachse. Ein abgestellter Zugteil liegt seitlich in `MOD-DEP`. | `MOD-TRK`, `MOD-PLT`, `MOD-DEP`, `MOD-SIG`, reduzierte Wegweiser. | Mehr offene Bodenfläche, lange Schatten, funktionierendes Zentrum räumlich fern. Die Distanz entsteht über Leerraum statt über neue Kartenkomplexität. |
| Umleitung über Irgendwo | Zwei `MOD-BRG` kreuzen versetzt über einer langen Nebenachse; Wegweiser zeigen bewusst gegen die intuitive Leserichtung. Die Kartenroute darf die Rückführung als Schleife darstellen. | `MOD-BRG` ×2, `MOD-TRK`, `MOD-WEG`, `MOD-SIG`, Richtungswegweiser. | `ink`-Träger, helle Deckränder, lange ruhige Horizontale. Brücken sind Kulisse; im Season-1-Puzzle entstehen weder Brückenzellen noch Kreuzungen. |
| Schienenersatzverkehr | Der feste Schienenstrang endet vor `MOD-ERS`; eine eindeutig grafische gestrichelte Kartenlinie überbrückt den Zwischenraum und wird später wieder als feste `ink`-/braune Schienenverbindung gezeigt. | `MOD-ERS`, `MOD-WEG`, `MOD-TRK`, `MOD-PLT`, provisorische Absperrung. | Warteplatte und Wegweiser sind materiell, die gestrichelte Linie bleibt ausschließlich Kartenstatus. Kein Bus als Puzzleobjekt und keine Busroute als Ersatz für die A–B-Zugfahrt. |

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

## 13. Statische Referenzen und Produktionsanschluss

- [Weltbild-Brief für `references/06-worlds.png`](evidence/world-image-brief.md) dokumentiert die eigene Herkunft der tatsächlich gelieferten [fünfteiligen Material-/Raumtafel](references/06-worlds.png). Die AI-Tafel ist illustrativ, keine Maß-/Font-/Objektmasterquelle; die [Weltkonstruktionsquelle](references/06-world-construction.html) und dieses Kapitel sind dafür maßgeblich.
- [Statische 20-Routenübersicht](references/07-route-map.html) ist eine dokumentarische HTML-Referenz ohne JavaScript, Eingaben, Runtimecode oder Spielinteraktion.
- Falls später nach ausdrücklicher Freigabe Produktionsassets entstehen, ordnet die bestehende Pipeline sie den Gruppen `core`, `season-s1`, `cosmetics`, `audio` oder `localization` und den Adressen `stp/<domain>/<stable-id>` zu; dieser Brief vergibt keine Runtimeadresse.
- Textur-, Modell- und Audiopresets, `.meta`-Dateien, tatsächliche Adressen, Katalogreferenzen, Lizenzmetadaten und Importparameter bleiben gemäß [Content Pipeline](../../../ARCHITECTURE/CONTENT_PIPELINE.md) spätere Produktionsarbeit.

## 14. Offene Freigaben und Abgrenzung

1. Ownerfreigabe der vorgeschlagenen Modulmaße, Perspektive, Rauheitsreferenzen, Lichtwinkel und Routenkompositionen steht aus.
2. Die originale Material-/Raumtafel `06-worlds.png` liegt als illustrative Materialreferenz vor; ihr Erscheinen ist keine Produkt-, Rechts- oder Productionfreigabe.
3. Alle fiktiven Beschilderungen, Fahrzeugoberflächen, finale Assetnamen und jede externe Rechtsprüfung bleiben außerhalb dieses Teilbriefs.
4. Es wurden keine neuen Level, keine neuen Puzzleformen, keine Routen, keine Kartenregeln, keine Rendererentscheidung und kein Runtimecode festgelegt.
