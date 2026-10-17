# Bildbrief – `references/06-worlds.png`

**Status:** Gestaltungsbrief für eine separat zu erzeugende, originale statische Referenztafel.
**Nicht enthalten:** Kein Spiel-Screenshot, keine reale Netzkarte, keine Station, kein Logo, keine Live-Daten und keine Produktfreigabe.

## Zweck und Ausgabe

- **Ausgabeziel:** `../references/06-worlds.png`.
- **Leinwand:** 2.400 × 1.350 px, RGB, sRGB, ohne Transparenz; vier gleich breite Außenränder à 72 px.
- **Aufbau:** Eine obere Informationsleiste (144 px) plus fünf gleich breite, vertikale Material-/Raumfelder à 422 px Breite mit 18 px Zwischenraum.
- **Typografie:** IBM Plex Sans für Beschriftungen, IBM Plex Sans Condensed für Abschnittstitel. Kein Ersatzfont und kein fremdes Bahn-Schriftbild.
- **Zielbild:** Präzise 2.5D-Miniatur-Materialtafel, keine spielbare Oberfläche und keine imitierte App-Ansicht.

## Gemeinsame Bildregeln

| Parameter | Vorgabe |
|---|---|
| Perspektive | Orthografisch wirkende 2.5D-Draufsicht: 35° Neigung, Gleisachsen nur 0°/90° oder 30°/150°. |
| Licht | Weiches Oberlicht von links oben, 35° Elevation; Schatten rechts unten, maximal 0,60 Modul lang. |
| Maßstab | 1 Weltmodul = 1,0 Miniatur-Meter; ein sichtbares Zugfenster, Bahnsteigrand oder eine Tür verankert jeden Bildausschnitt. |
| Gleis | `track-brown #805535`, zwei 0,10-Module-Schienen mit 1,30-Modul-Achsabstand; Schwellen im 0,70-Module-Takt. |
| Kontakt | Räder berühren Gleisoberkante, Zaunfüße und Baken stehen sichtbar auf dem Boden; keine Schwebeabstände über 0,02 Module. |
| Palette | Ausschließlich `brand-red #C83D45`, `surface #F4F2EE`, `paper #FFFFFF`, `ink #20272C`, `track-brown #805535`, `grid-line #CCD0D1` sowie die reservierten Puzzle-Tokens nur in der Legende. |
| Reservierte Semantik | `assumption-gray`, `empty-red`, `selection-yellow` und `selection-edge` stehen in der Leiste als Semantikhinweis, erscheinen aber nicht als Weltgleis oder Kartenpfad. |

## Leiste oben

Links: Titel **„SEASON 1 · MATERIAL- UND RAUMMODULE“**.
Rechts: sechs 20×20-px-Farbchips mit den exakten gemeinsamen Tokenwerten und kurzer Legende **„Braun = konkrete Schiene · Gelb = Auswahl“**.
Darunter eine 1-px-`grid-line`-Trennlinie; keine Marke, kein Monogramm und keine realen Stationsnamen.

## Fünf Felder

| Feld | Titel / Anteil | Erforderliche räumliche Elemente | Materialfokus |
|---|---|---|---|
| 1 | **STAMMSTRECKE** · 0–20 % | Zentraler Tunnelanschnitt, drei parallele 12×3-Bahnsteige, enger 18×18-Knoten, ein Signal-/Statusmast und zwei Warnbaken. | `ink`-Tunnelinneres, helle Bahnsteigplatten, punktuelles `brand-red`; der Hauptstrang bleibt fast vollständig, aber nicht beschriftet. |
| 2 | **BAUABSCHNITT MITTE** · 20–40 % | 12×12-Baugrube, zwei 2-Module-provisorische Laufwege, Zaun mit roten Kappen und enger Bahnsteigrand. | Kies, Betonplatte und kontrollierte Zaunsicherung; der Kartenring wird nicht als physische Gleisschleife gezeichnet. |
| 3 | **AUSSENAST OHNE HALT** · 40–60 % | 36-Module-Fernachse, Randbahnsteig, 6×6-Gewerbevolumen und ein abgestellter 22-Module-Zugteil auf Depotgleis. | Viel freie Bodenfläche und lange ruhige Schatten; Distanz entsteht durch Leerraum, nicht durch neue Karte. |
| 4 | **UMLEITUNG ÜBER IRGENDWO** · 60–80 % | Zwei versetzte Brückenfelder über einer Nebenachse, drei fiktive Richtungswegweiser und eine lange Rückführung. | `ink`-Träger, `surface`-Deckränder; kein Brückenfeld im Puzzle und keine Kreuzung als Spielregel. |
| 5 | **SCHIENENERSATZVERKEHR** · 80–100 % | Ersatzhaltestelle, Warteplatte, Wegweiser, feste Schiene bis zum Rand und grafisch getrennte gestrichelte Kartenfortsetzung. | Materielle Warte-/Wegstruktur gegen klar flache Kartenstriche; keine Busroute als fahrbarer A–B-Ersatz. |

## Kompositionsgrenzen

1. Die fünf Felder teilen eine durchgehende Grundlinie, dürfen aber keine zusammenhängende reale Netzkarte bilden.
2. Jedes Feld zeigt höchstens ein Hero-Objekt, zwei Funktionsobjekte und sechs Kleinobjekte pro 12×12-Module-Kachel.
3. Züge haben keine Gesichter, keine fremden Lackierungen, keine Logos und keine lesbaren realen Zielanzeigen.
4. Die Tafel zeigt keine Rasterzellen, Randzahlen, A/B-Punkte, X-Markierungen oder grauen Belegungsnotizen.
5. Eine Karte darf bei Feld 4 einen geschlossenen Überblicksring andeuten; ein Puzzlepfad darf nirgends als Schleife gezeigt werden.
6. Alle Beschriftungen sind originale generische Abschnittstitel; sie sind Layoutreferenz, keine finale Textbibliothek.

## Abnahmegrenzen für die Referenzdatei

- Die Tafel ist eine **dokumentarische Referenz**, kein Screenshot eines implementierten Spiels.
- Sie wird erst mit dem Originalbild als tatsächlich geliefert markiert; dieser Brief selbst liefert keine PNG-Datei.
- Material, Maß, Licht und Komposition bleiben Empfehlungen bis zur ausdrücklichen Ownerfreigabe.
- Die Bilddatei erteilt weder Lizenz-, Rechts-, Produkt- noch Productionfreigabe.

## Tatsächliche Lieferung – 17.10.2026

Die hier beauftragte AI-Materialreferenz liegt als PNG mit 2560×1440 Pixeln vor. Der Brief dokumentiert die originale Gestaltungsabsicht, keine Behauptung, dass die Generierung jeden Maß-, Font-, Prop- oder Paletteparameter exakt erfüllt. Der Hauptreport und DESIGN_CONSISTENCY_AND_VALIDATION.md benennen die Illustrationsgrenze; die separat gerenderten Fahrzeug-/Weltkonstruktionsblätter und Fachkapitel bleiben die exakten Quellen. Kein Suchbild wurde als Produktionsasset übernommen.
