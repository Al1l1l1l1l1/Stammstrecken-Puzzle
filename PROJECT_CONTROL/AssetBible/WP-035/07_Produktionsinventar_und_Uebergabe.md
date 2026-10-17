# WP-035 Teil G – Produktionsinventar und Übergabe

**Stilkorrektur 17.10.2026:** [Direkte Ownerfestlegung](evidence/OWNER_STYLE_CORRECTION_2026-10-17.md): Cartoon-/Comic-Spielwelt, plump-staubig-behördenhaft, mit **glasklarer Deutsche-Bahn-Anspielung**. Die klare dienstliche Informationshierarchie bleibt ihr bewusstes Gegenstück, nicht ein generischer Fantasy-/Candy-Look. Konkrete Typo-/Zeichen-/Layoutwerte bleiben Vorschläge; keine Annahme der abgelehnten Realismusbilder.


**Arbeitsrolle:** Teil-Ausarbeitung G innerhalb WP-035, keine Abnahme, keine Geschäftsführung und keine unabhängige QC.

**Arbeitsstand:** Dieses Kapitel ist eine **Designempfehlung zur ausdrücklichen Ownerfreigabe**. Es importiert kein Asset, setzt keine Produktregel und erklärt keine Rechts-, Produktions- oder Releasefreigabe.

## 1. Zweck und belastbare Grenzen

Dieses Inventar zerlegt die Anforderungen der Teilkapitel A–F in einzeln nachverfolgbare künftige Runtime-Bausteine und getrennte dokumentarische Referenzen.

Es enthält bewusst **keine** Shopliste, Mengenplanung, Preise, Freischaltungen, Runtimeimplementierung, neue Pipeline, Performancewerte oder zusätzliche Test-/Abnahmerollen.

Die Referenzspalte führt die tatsächlich gelieferten Quellen. Die Eigenprüfung des Hauptagenten prüft ihre Dateiexistenz, Öffnbarkeit und Inventarzuordnung; das ist keine unabhängige QC und keine Produktionsfreigabe.

| Kategorie | Bedeutung | Tatsächlicher Lieferstatus |
|---|---|---|
| Dokumentarische Quelle | Hauptreport, sieben Kapitel, SVG-/HTML-/PNG-Referenzen, Token, Nachweise | `DELIVERED_REFERENCE—OWN_CHECK`; nicht Runtime |
| Künftiges Runtimeasset | Späterer Import unter Assets mit .meta und Zielklasse | `FUTURE_RUNTIME—NOT_PRODUCED` |
| Originalschrift | Drei unveränderte TTFs, Herkunft, Hashes und OFL-Originaltext | Lizenzbeleg geliefert; kein Runtimeimport autorisiert |

Die tatsächliche Gesamtliste samt SHA-256 steht im [Dateimanifest](evidence/file-manifest.json); der [Hauptreport](README.md) zeigt alle zehn PNG-Ansichten. Die beiden neuen Cartoon-Stiltafeln sind keine Maßautorität; genaue Konstruktionen liegen separat vor.

## 2. Gemeinsame unveränderte Design- und Semantikbasis

Es gilt genau das gemeinsame Token- und Schriftsystem aus dem [Owner-Brief](evidence/OWNER_CONTEXT_AND_DESIGN_BRIEF.md): kein Alternativsystem und keine offizielle Bahn-Schrift.

| Token | Fester Empfehlungswert | Semantische UI-/Puzzleverwendung im Inventar |
|---|---:|---|
| `brand-red` | `#C83D45` | Marke, dienstlicher Akzent und aktuelle Störung |
| `surface` | `#F4F2EE` | warme Informationsfläche |
| `paper` | `#FFFFFF` | Raster- und Kartenfläche |
| `ink` | `#20272C` | Primärtext und freigegebener Kartenstrang |
| `track-brown` | `#805535` | ausschließlich konkrete sechs Gleisformen |
| `assumption-gray` | `#727B82` | ausschließlich offene Belegungsannahme |
| `empty-red` | `#B42E39` | ausschließlich optionales Leer-X |
| `selection-yellow` | `#F1C84B` | ausschließlich Werkzeug- oder Feldauswahl |
| `selection-edge` | `#6C5717` | geometrische Auswahlkontur |
| `grid-line` | `#CCD0D1` | ruhige Rasterlinien |

**Schriftvorschlag ohne Ersatzfamilie:** IBM Plex Sans für Interface/Zahlen und IBM Plex Sans Condensed für straffe Wortmarke/Statusüberschriften. Vor jedem späteren TTF-Import bleiben IBM-Originalquelle, konkrete Version, Lizenztext, Embedding und Weitergabe zu erfassen und ausdrücklich freizugeben.

Die vorhandene Rätselsemantik bleibt unverändert: sechs konkrete Formen `NS`, `EW`, `NE`, `ES`, `SW`, `WN`; Braun bedeutet konkret, Grau bedeutet offene Annahme, Rot bedeutet Leerannahme und Gelb bedeutet nur Auswahl.

Keine Inventarzeile führt Kreuzung, Weiche, Schleife, Sonderfeld, versteckte Lösungsbewertung oder eine Pflichtphase für Notizen ein.

## 3. Reproduzierbare Konstruktionsgrenzen für spätere Referenzen

Die folgenden Parameter sind Empfehlungen für die statische Referenzkonstruktion; sie werden erst nach Ownerfreigabe zu Produktionsvorgaben.

| Baustein | Empfohlene Referenzgeometrie | Invariante |
|---|---|---|
| Gleisfeld | Normalisierte Quadratfläche `100 × 100`; Portmitten bei `(50,0)`, `(100,50)`, `(50,100)`, `(0,50)` | Jede konkrete Form hat genau zwei dieser Ports. |
| Sechs Formen | `NS=(N,S)`, `EW=(E,W)`, `NE=(N,E)`, `ES=(E,S)`, `SW=(S,W)`, `WN=(W,N)` | Portlage ist unabhängig von Farbe und Auswahl. |
| X/Grau/Track | Drei getrennte Zeichenebenen mit gemeinsamer Zellmitte | Grau wird nie als konkrete graue Kurve gezeichnet; Gelb wird nie in Track-Zeichen eingebrannt. |
| Auswahl | separate Overlayebene über Zellinhalt und Werkzeug | `selection-yellow`/`selection-edge` dürfen keine Wahrheits- oder Ergebnisfarbe signalisieren. |
| A/B-Ports | Außenport liegt mittig auf einer äußeren Zellkante und verweist auf eine konkrete Randrichtung | A und B werden nicht als zusätzliche Schienenform modelliert. |
| Zeilen-/Spaltenzählung | Zustand `offen`, `exakt`, `überschritten` neben der betroffenen Linie | Nur sichtbare Belegung bestimmt den Zustand; keine Lösungsvorschau. |
| App-Icon | quadratischer Master mit eigener unterbrochener Gleisknoten-Geometrie | Kein offizielles Monogramm, kein rotes Quadrat als Nachzeichnung und keine Herkunftstäuschung. |

Diese Parameter beschreiben statische Designreferenzen, keine Screenshotvorgabe und keine Runtime- oder Eingabelogik.

## 4. Inventarstruktur und Referenzklassen

Die maschinenlesbare vollständige Zeilenliste liegt in [`evidence/asset-requirements.csv`](evidence/asset-requirements.csv). Jeder Eintrag führt ID, Zweck, Varianten/Zustände, Referenzdatei, spätere Zielklasse/Format, Label/Adresse, Import-/Meta-, Lizenz-/Versions- und Lokalisierungsbedarf sowie Lieferstatus.

| Kapitel | Inventarpräfix | Tatsächlich gelieferte Referenzquelle | Künftiger Runtimebereich |
|---|---|---|---|
| A Marke/Zeichen/Icon | `A-`, `DOC-A-` | `01_Marke_und_App-Icon.md`; statische Marke/Icon-SVG und PNG unter `references/` | Kernmarke und App-Icon |
| B Typografie/Farben | `B-`, `DOC-B-` | `02_Typografie_und_Farben.md`; Type-/Token-Referenz unter `references/` | UI-Schrift, UI-Styles und Tokenabbildung |
| C UI/Puzzlebausteine | `C-`, `DOC-C-` | `03_UI_und_Puzzlebausteine.md`; Raster-/Komponentenansichten unter `references/` | UI Toolkit, Vektoren und Spriteatlas |
| D Züge/Lackierungen/Objekte | `D-`, `DOC-D-` | `04_Zuege_Lackierungen_und_Betriebsobjekte.md`; Fahrzeug-/Objektformblätter unter `references/` | World-Prefabs, Sprites/Texturen/Materialien |
| E Welt/Kartenmaterial | `E-`, `DOC-E-` | `05_Welt_und_Kartenmaterial.md`; Karten-/Weltformblätter unter `references/` | Karten- und Umgebungsbausteine |
| F Motion/Sound | `F-`, `DOC-F-` | `06_Motion_und_Sound.md`; Ablauf-/Cue-Sheets unter `references/` | Animationen und Unity-Audioquellen |

Die CSV-IDs sind Identifikatoren für Design- und Übergabeverfolgung. Sie sind **keine** Produkt-, Catalog-, Preis-, Unlock- oder Localization-Key-IDs.

## 5. Vollständigkeit nach funktionalen Bausteinen

| Bedarfskomplex | Abgedeckte Inventar-IDs | Konkrete Varianten oder Zustände |
|---|---|---|
| Brand/Icon | `A-01` bis `A-03` | Wortmarke, eigenständiges Gleisknoten-Zeichen, quadratischer Icon-Master |
| Schrift/Token | `B-01` bis `B-03` | Plex Sans, Plex Sans Condensed, zehn festgelegte Farbrollen |
| Konkrete Gleise | `C-01` | `NS`, `EW`, `NE`, `ES`, `SW`, `WN`, jeweils genau zwei Ports |
| Notizen/Auswahl | `C-02` bis `C-04` | Leer-X, offene graue Belegung, gelbe unabhängige Auswahl |
| Ports/Counts/Tools | `C-05` bis `C-07` | A/B-Randport, offen/exakt/überschritten, sechs Formen plus X/Grau/Leeren/Mehrfachauswahl/Undo/Hinweis |
| Status/Navigation/Ergebnis/Karte | `C-08` bis `C-11` | Kopfstatus, drei Hauptziele außerhalb des Rätsels, Zugfahrt-vor-Ergebnis, Kartenknotenstatus |
| Fahrzeug-/Objektmodule | `D-01` bis `D-04` | plumpe eigene rote Nahverkehrsfront, helle Türen/Bauchzone und Cartoon-Lackierungsebene, Bahnsteig/Signal/Wegweiser/Tunnel/Bake |
| Welt-/Kartenmodule | `E-01` bis `E-06` | fünf bestehende Netzabschnitte, vier Routen je Abschnitt, 20 sichtbare Routenkontexte ohne neuen Contentumfang |
| Semantischer Audio/Motion | `F-01` bis `F-07` | Eingabe statt Richtigkeitsurteil, Rücknahme, Freigabe, A–B-Fahrt, Ergebnis, Karte, Kosmetik |

Die Formulierung „20 sichtbare Routenkontexte“ bildet nur die bereits bestätigte Hierarchie `5 × 4` ab. Sie erzeugt weder eine neue Assetmenge noch einen Katalog, eine Freischaltung oder eine Preisentscheidung.

## 6. Vorhandene technische Anschlussregeln

| Prüffeld | Dokumentierter/erhobener Stand | Konsequenz für spätere Assets |
|---|---|---|
| Unity-/UI-Stack | Unity `6000.3.23f1`, URP 2D/2.5D und UI Toolkit sind verbindlich ([`TECH_STACK.md`](../../../ARCHITECTURE/TECH_STACK.md)). | Zielklassen bleiben Unity-/URP-/UI-Toolkit-kompatibel; Teil G führt keinen Renderer oder UI-Stack ein. |
| Addressables | Lokaler Launchkatalog und Addressables `2.10.3` sind Architekturvertrag; Gruppen `core`, `season-s1`, `cosmetics`, `audio`, `localization` und die Form `stp/<domain>/<stable-id>` sind vorgegeben ([`CONTENT_PIPELINE.md`](../../../ARCHITECTURE/CONTENT_PIPELINE.md)). | CSV-Adressen sind klein geschrieben und nur als spätere Zieladressen vorgeschlagen. |
| Lokale Settings | Bei Teil-G-Prüfung liegt kein `AddressableAssetSettings.asset` unter `Assets/` vor; das Paket ist gelockt. | Keine Gruppe oder Adresse wurde hier angelegt; spätere Konfiguration bleibt Produktionsarbeit. |
| Meta/Source Control | `m_SerializationMode: 2` (Force Text) ist vorhanden; 178 `.meta`-Dateien unter `Assets/` wurden gezählt. Architektur verlangt Visible Meta Files und eingecheckte GUIDs ([`TECH_STACK.md`](../../../ARCHITECTURE/TECH_STACK.md), [`CONTENT_PIPELINE.md`](../../../ARCHITECTURE/CONTENT_PIPELINE.md)). | Jedes spätere Runtimeasset erhält seine eingecheckte `.meta`; keine GUID- oder Pfaderfindung im Inventar. |
| Presets | `ProjectSettings/PresetManager.asset` existiert, aber kein `*.preset` wurde unter `Assets/` gefunden. | Zielklassen verlangen später ein versioniertes Importpreset; Namen und Parameter bleiben ausdrücklich offen. |
| Git/LFS | `.gitattributes` enthält nur Textnormalisierung; keine LFS-Attributregel und `git lfs track` meldet keine Muster. | Die Architektur fordert LFS oberhalb einer **noch festzulegenden** Schwelle; Teil G bestimmt weder Schwelle noch neue Regel. |
| Import/Generated | Produktionsassets liegen später unter `Assets/StammstreckenPuzzle/...`; `Generated/` ist kein handgepflegter Assetursprung ([`MODULE_BOUNDARIES.md`](../../../ARCHITECTURE/MODULE_BOUNDARIES.md)). | Referenzquellen bleiben außerhalb von `Assets/`; Import kann erst aus freigegebenen Quellen erfolgen. |
| Lokalisierung | Startlocale ist `de-DE`; sichtbarer Text benötigt stabile Schlüssel, Metadaten und Pseudolocale-Prüfung ([`CONTENT_PIPELINE.md`](../../../ARCHITECTURE/CONTENT_PIPELINE.md)). | Assetnamen bleiben unlokalisiert; Wörter auf Icons oder Fahrzeugen sind zu vermeiden oder als Asset-Table zu führen. |
| Audio | Code adressiert semantische Cue-IDs, nicht Clipnamen; Quellen benötigen Cue-ID, Urheber/Lizenz, Rohformat, Loopdaten, Normalisierungsziel, Mixergruppe und Preset ([`CONTENT_PIPELINE.md`](../../../ARCHITECTURE/CONTENT_PIPELINE.md)). | F-Audiozeilen sind Cue-Anforderungen, keine gelieferten Clips und keine Lautheitsvorgabe. |

Die lokale Bestandsaufnahme ist eine vorbereitende Beobachtung, kein Importtest und kein unabhängiger Abschlussnachweis.

## 7. Import-, Lizenz-, Versions- und Lokalisierungsminimum

1. **Referenz vor Runtime:** Erst die tatsächlich gelieferte statische Quelle und der Ownerentscheid, danach der Produktionsimport mit eigener `.meta`; die Bible erzeugt keinen Assetpfad oder Addressables-Eintrag.
2. **Import:** Textur, Modell und Audio verwenden später jeweils ein versioniertes Zielklassen-Preset nach [`CONTENT_PIPELINE.md`](../../../ARCHITECTURE/CONTENT_PIPELINE.md). Konkrete Kompression, Mipmap-, Mesh-, Audio- und Größenparameter sind nicht vorgegeben.
3. **Lizenz:** Für Font, Audio und jede fremde Ausgangsquelle sind Herkunft, Rechteinhaber, Lizenztext, Version, Embedding, Weitergabe und Einschränkungen vor Import zu dokumentieren. Eigene Quellen erhalten Autor und Version; externe Suchbilder gelten nie automatisch als Assetquelle.
4. **Version:** Jede spätere Katalog-/Assetbindung folgt stabiler ID, Kataloghash und Release-Lock; `DRAFT`/`FIXTURE_ONLY` sind nicht runtimeimportierbar ([`CONTENT_CATALOGS.md`](../../../ARCHITECTURE/CONTENT_CATALOGS.md)).
5. **Localization:** Nur Leserichtung und sichtbare Sprache benötigen `de-DE`-Asset-/String-Tabellen und Schlüssel. Reine Geometrie, Farbtokens, Cue-IDs und technische Adressen bleiben sprachneutral.
6. **Semantik:** Audio, Motion, Farbe und Feedback dürfen eine Eingabe bestätigen, aber keine noch offene Annahme als richtig oder falsch bewerten.

## 8. Ausdrückliche offene Entscheidungen für den Owner

| ID | Entscheidung zur ausdrücklichen Freigabe | Warum Teil G sie nicht annimmt |
|---|---|---|
| `OD-G-01` | Gemeinsame Designempfehlung mit den zehn Tokens und beiden IBM-Plex-Familien | Der Owner-Brief bezeichnet sie als noch nicht angenommene Empfehlung. |
| `OD-G-02` | Eigenständige Wortmarken-/Zeichen-/Icon-Konstruktion und ihre Nähe ohne Kopie | Zentrale Herkunfts- und Rechtswirkung; keine Rechtsfreigabe durch diese Dokumentation. |
| `OD-G-03` | Konkrete Fahrzeugfronten, Lackierungen, Objekt- und Weltformblätter der Kapitel D/E | Kosmetik bleibt bindend spielneutral, aber Aussehen ist noch Entwurf. |
| `OD-G-04` | Motion-/Cue-Sheets, inklusive optionaler nichttextlicher Audioquellen | Ablaufreihenfolge ist vorgegeben; die einzelnen Medienquellen und Rechte sind noch nicht freigegeben. |
| `OD-G-05` | Spätere Presetnamen/-parameter und eine LFS-Schwelle | Im lokalen Stand nicht vorhanden; Architektur verbietet das Erfinden konkreter Schwellen. |

## 9. Übergabe ohne Abschlussbehauptung

**Nächster Schritt:** ausdrückliche Ownerfreigabe der zusammenhängenden Designempfehlung; anschließend Prüfung und Nutzung der vorhandenen Produktionsblöcke innerhalb ihrer jeweiligen Scopes, ohne Projektneuplanning.

Der Hauptagent hat die konkreten SVG-/HTML-/PNG-Dateien geprüft und die CSV auf tatsächliche Pfade nachgeführt. Nachweise stehen im Lieferprüfungs- und Renderprotokoll; diese Eigenprüfung ist keine unabhängige Abschlussfreigabe.

Teil G liefert dafür nur die Inventar- und Übergabestruktur. Es behauptet weder Runtimeassets noch Produktimport, Rechtsprüfung, Produktionsfreigabe, externe Abnahme oder unabhängige QC.

## Quellen

- [Owner-Kontext und gemeinsame Ausarbeitungsbasis](evidence/OWNER_CONTEXT_AND_DESIGN_BRIEF.md)
- [WP-035 – Finale Asset-Bible: vollständige Ausarbeitung](../../../WORK_PACKAGES/WP-035_Finale-Asset-Bible-Ausarbeitung.md)
- [Technology Stack v0.4](../../../ARCHITECTURE/TECH_STACK.md)
- [Module Boundaries v0.3](../../../ARCHITECTURE/MODULE_BOUNDARIES.md)
- [Content Pipeline v0.3](../../../ARCHITECTURE/CONTENT_PIPELINE.md)
- [Content Catalogs v0.5](../../../ARCHITECTURE/CONTENT_CATALOGS.md)
- [Test Strategy v0.5](../../../ARCHITECTURE/TEST_STRATEGY.md)
- [Master-Spezifikation](../../../Stammstrecken_Puzzle_Konzept_00-15/09_Train_Track_Master_Spezifikation.md)
