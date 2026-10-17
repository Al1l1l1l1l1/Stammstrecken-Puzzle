# Owner-Kontext und gemeinsame Ausarbeitungsbasis

**Stand:** 17.10.2026. Nachweis-/Arbeitsbrief für WP-035, kein neues Konzept, ADR oder Productionkatalog.

## Vorrang und belegte Herkunft

Der Owner hat am 17.10.2026 während dieser Bearbeitung ausdrücklich klargestellt: **„dieser Kontext steht ÜBER dem Repo und den vorgaben“**. Der bestätigte ursprüngliche Kontext wird deshalb nicht durch später eingeführte Einschränkungen oder vorsichtige Umdeutungen abgeschwächt. Die aktuelle beauftragte Ausarbeitung bleibt ein Entwurf ohne Merge, Produktimport oder automatische Produktfreigabe.

Die ursprüngliche Konversation wurde am 30.09.2026 über Manus Tasks aus Task `jUgY7XWQvr7CEGpH3aArN3` rekonstruiert. Der lokale Auszug `original_page2_history.md` enthält folgende Aussagen; die Nummern sind die dort erhaltenen Nachrichtenpositionen, keine erfundenen Repoentscheide:

- Nutzer 76: großer Pivot zu **Stammstrecken-Puzzle**, „richtig grenzwertig nah“ an die deutsche Bahn-Aufmachung; das hochwertige, befriedigende Spiel bleibt erhalten.
- Nutzer 83/87: regelmäßige Bahnfahrer müssen die Referenz sofort erkennen; Aufmachung, Logo und Züge ausreizen, ohne Urheberrechte zu verletzen.
- Nutzer 93: die Bahnfahrer-Lore wirklich verstehen; **Schienenersatzverkehr** muss hinein, das konkrete Drohungsbeispiel ist ausdrücklich nicht direkt zu implementieren.
- Nutzer 100: **„als wäre es der gleiche Designer gewesen“**, **„so nah wie möglich annähern / Inspirieren lassen, aber nicht kopieren“**; allgemeine Betriebsbegriffe müssen nicht künstlich ersetzt werden.
- Nutzer 105 bestätigt die ausgearbeitete unmittelbare Bahnfahrer-Wiedererkennung: „Absolut on point“.
- Nutzer 110/117/122 bestätigen Produktwelt, visuelle Kernidentität und Systemsprache.

Die dazu korrelierten persistenten Fachfassungen sind insbesondere Konzept 10 und 11. Die frühere technische, separate WP-013-Metrikkonversation ist keine Quelle dieser Gestaltungsentscheidung. Die historischen Zitate dienen der Herkunftsklärung, ersetzen keine später ausdrücklich bestätigte Ownerentscheidung.

## Gemeinsame Empfehlung für die Ausarbeitung (noch nicht angenommen)

Alle Kapitel entwickeln **eine** zusammenhängende Designempfehlung. Kein Alternativenkatalog, keine neue Markenpositionierung. Strenge, vertraute deutsche Informationshierarchie, starke dienstliche Rot-/Weiß-/Anthrazitdramaturgie, sachlicher Verwaltungsduktus, fiktive eigenständige Nahverkehrsfronten und hochwertig materielle Miniaturwelt. Nicht Kindercomic, nicht Dystopie, keine niedlichen Zuggesichter, kein generischer Cozy-Fantasy-Pivot.

Vorgeschlagene gemeinsame Starttokens:

| Token | Wert | Rolle |
|---|---|---|
| brand-red | `#C83D45` | sehr präsenter dienstlicher Marken-/Betriebslagenakzent |
| surface | `#F4F2EE` | warme helle Informationsfläche |
| paper | `#FFFFFF` | Raster-/Kartenoberfläche |
| ink | `#20272C` | Primärtext und freigegebener Kartenstrang |
| track-brown | `#805535` | konkrete sechs Gleisformen |
| assumption-gray | `#727B82` | offene Belegungsnotiz, niemals eine konkrete graue Kurve |
| empty-red | `#B42E39` | optionale Leerannahme X |
| selection-yellow | `#F1C84B` | nur Werkzeug-/Feldauswahl, niemals Lösung/Belohnung/Spielzustand |
| selection-edge | `#6C5717` | geometrische Auswahlkontur |
| grid-line | `#CCD0D1` | ruhige Rasterlinien |

Vorgeschlagene Schriftfamilie: **IBM Plex Sans** für Interface und Zahlen, **IBM Plex Sans Condensed** für straffe Wortmarke/Statusüberschriften; echte License-/Embedding-/Weitergabebedingungen an den IBM-Originalquellen prüfen. Keine offizielle Bahn-Schrift verwenden. Schriftgrößen sind Designreferenzen in logischen Einheiten, keine beschlossene Geräte-/Accessibilitypolitik.

Konstruktionen und Quellen müssen reproduzierbar sein. SVG-/HTML-/PNG-Dateien sind hier ausschließlich source-kontrollierte, statische Gestaltungsreferenzen, keine Web-App oder alternative Runtime. Brand und UI erhalten exakte Vektorkonstruktionen; Fahrzeug-/Weltmaterial kann zusätzlich original generierte Form-/Materialreferenzen nutzen. Keine fremden Suchbilder als gekaufte/lizenzierte Assets ausgeben.

## Unveränderte Semantik und Grenzen

Braun = konkrete Schiene; Grau = optionale Belegungsannahme; Rot X = optionale Leerannahme; Gelb = ausschließlich Auswahl. Sechs Formen NS/EW/NE/ES/SW/WN; keine Kreuzung, Weiche, Schleife, Sonderfelder. Notizen sind keine Pflichtphase und kein Richtigkeitsurteil. Anfang, Denkphase, gültige Lösung, Rücknahme technischer Hilfen, Hervorhebung des echten Pfads, exakte A–B-Zugfahrt, Ergebnisfläche, danach erst getrennte freiwillige Angebote. Bestehende drei Hauptziele; keine untere Navigation/Geld/Werbung im Rätsel. Keine neuen Gesten, Orientierung, Accessibilityproduktentscheidungen, Preise, Freischaltungen, Performancebudgets, Kataloge, Texte oder Leveltests.

Fünf unveränderte Seasonabschnitte: Stammstrecke, Bauabschnitt Mitte, Außenast ohne Halt, Umleitung über Irgendwo, Schienenersatzverkehr. Je vier vorhandene Routen, insgesamt 20 × 12 Rätsel; im Design keine neuen Inhaltsumfänge. Fahrzeuge und Objekte rein kosmetisch. Allgemeine Bahnbegriffe ausdrücklich erhalten; konkrete Sätze selbst schreiben und nur als Layout-/Tonbeispiele kennzeichnen.

## Technischer Anschluss ohne neue Architektur

Unity 6000.3.23f1, URP 2D/2.5D, UI Toolkit, lokale Addressables, Localization de-DE und Unity Audio sind vorhandene Verträge. Keine neue Pipeline oder Renderingwahl. Logische Adressen `stp/<domain>/<stable-id>` mit Kleinbuchstaben/Ziffern/Bindestrichen; vorhandene Gruppen core, season-s1, cosmetics, audio, localization. Runtimeassets später unter `Assets/StammstreckenPuzzle/` mit `.meta` und versionierten Importpresets. Diese Bible liegt dokumentarisch unter `PROJECT_CONTROL/AssetBible/WP-035/`; kein Runtimeasset wird hierdurch importiert. Endgültige Importpresetparameter oder noch nicht vorhandene LFS-Schwellen nicht als bestätigt erfinden.

## Ausarbeitungsrollen

Teilkapitel werden parallel als Ausarbeitung erstellt; keine zusätzliche Abnahmerolle und keine unabhängige QC-Freigabe werden damit eingeführt. Der ausführende Hauptagent vereinheitlicht und prüft die konkrete Lieferung. Work-Package- und Architekturentscheidungen werden nicht an Kimi delegiert.
