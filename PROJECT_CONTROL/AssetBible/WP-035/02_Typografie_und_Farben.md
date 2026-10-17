# 02 – Typografie und Farben

**Stilkorrektur 17.10.2026:** [Direkte Ownerfestlegung](evidence/OWNER_STYLE_CORRECTION_2026-10-17.md): Cartoon-/Comic-Spielwelt, plump-staubig-behördenhaft, mit **glasklarer Deutsche-Bahn-Anspielung**. Die klare dienstliche Informationshierarchie bleibt ihr bewusstes Gegenstück, nicht ein generischer Fantasy-/Candy-Look. Konkrete Typo-/Zeichen-/Layoutwerte bleiben Vorschläge; keine Annahme der abgelehnten Realismusbilder.


**Arbeitsstand:** konkrete Gestaltungsempfehlung für WP-035; **ausdrückliche Owner-, Produkt-, Accessibility- und Rechtsfreigabe steht aus**.
**Geltung:** statische Asset-Bible-Referenz, kein Runtimeimport und keine Geräte- oder Accessibilitypolitik.
**Zusammenhang:** Die gemeinsame Ausgangsbasis ist der [Owner-Kontext](evidence/OWNER_CONTEXT_AND_DESIGN_BRIEF.md). Visuelle Richtung und Puzzle-Semantik folgen der [Visuellen Identität](../../../Stammstrecken_Puzzle_Konzept_00-15/10_Visuelle_Identitaet_und_Stammstrecke.md) und der [UI-Spezifikation](../../../Stammstrecken_Puzzle_Konzept_00-15/12_UI_und_Bedienungsspezifikation.md). Die spätere Assetablage bleibt dem [Content-Pipeline-Vertrag](../../../ARCHITECTURE/CONTENT_PIPELINE.md) vorbehalten.

## 1. Lesbare dienstliche Präzision

Die Empfehlung kombiniert eine enge Statusschrift mit einer ruhigen UI-Schrift: **IBM Plex Sans Condensed SemiBold** trägt Wortmarke, große Statuszeilen und kurze Versal-Hinweise. **IBM Plex Sans Regular/SemiBold** trägt Informationen, Bedienlabels und Zahlen. Das erzeugt eine klare, straffe Betriebslagen-Hierarchie, ohne eine offizielle Bahn-Schrift oder deren Oberflächenkopie zu verwenden.

Die konkrete Zuordnung ist in [`references/design-tokens.json`](references/design-tokens.json) maschinenlesbar abgelegt und in [`references/02-type-color.html`](references/02-type-color.html) als lokale statische Referenz sichtbar. Die HTML-Seite lädt nur die mitgelieferten Dokumentationskopien der Original-TTFs; sie ist keine Screen-Demo und keine Web- oder Spiel-Laufzeit.

> **Entwurfsregel:** Die Nähe entsteht aus Rot/Weiß/Anthrazit, Informationsdichte und trockener Typohierarchie. Sie entsteht nicht aus einem fremden Logo, einer fremden Schrift, einer Ticketansicht oder einer Reproduktion eines bestehenden Screens.

## 2. Unveränderte Farbtoken und Rollen

Diese zehn **UI-/Puzzlewerte** sind aus dem gemeinsamen Brief übernommen. Sie beschränken nicht sämtliche Schatten-/Boden-/Beschilderungs-/Vegetationsfarben der Cartoonillustration; solche Weltfarben erzeugen keine UI-Zustände. Neue Farben für Erfolg, Fehler oder Belohnung werden nicht vorgeschlagen: Ein freigegebener Kartenstrang bleibt `ink`, nicht Grün; eine Überschreitung kann die bestehende Rotrolle nutzen, ohne die Bedeutung des roten X umzudefinieren.

| Token | Exakter sRGB-Wert | Ausschließliche empfohlene Rolle | Nicht dafür verwenden |
|---|---|---|---|
| `brand-red` | `#C83D45` | Prägnanter Marken- und Betriebslagenakzent, dominante Statusfläche | konkretes Gleis, X, Auswahl oder Erfolgscode |
| `surface` | `#F4F2EE` | Warme Informationsfläche außerhalb der technischen Rasterfläche | Gleis-, X- oder Auswahlbedeutung |
| `paper` | `#FFFFFF` | Raster- und Kartenoberfläche, helle Gegenfläche | Statussemantik |
| `ink` | `#20272C` | Primärtext, technische Rahmen, freigegebener Kartenstrang | konkrete Gleisform oder aktive Auswahl |
| `track-brown` | `#805535` | Ausschließlich die sechs konkreten braunen Gleisformen | Textstatus, offene Annahme oder Auswahl |
| `assumption-gray` | `#727B82` | Offene Belegungsannahme als kleines Standardgleis | konkrete graue Kurve, Wahrheits- oder Lösungsurteil |
| `empty-red` | `#B42E39` | Optionale Leerannahme als X | Markenfläche, allgemeine Warnung oder Erfolg |
| `selection-yellow` | `#F1C84B` | Aktives Werkzeug und ausgewählte Zelle | Gleis, Lösung, Stern, Reward oder Zustandswahrheit |
| `selection-edge` | `#6C5717` | Geometrische Auswahlkontur | eigenständige Textfarbe auf gelbem Grund |
| `grid-line` | `#CCD0D1` | Ruhige 1-Einheit-Rasterlinie | Text, Ziffer oder semantischer Status |

Die semantische Trennung bleibt auch bei Überlagerung erhalten:

1. **Braun** bleibt eine konkrete NS-, EW-, NE-, ES-, SW- oder WN-Schiene.
2. **Grau** bleibt eine offene Belegungsnotiz; es darf keine konkrete graue Kurve darstellen.
3. **Rot als X** bleibt eine freiwillige Leerannahme und kein Fehlermarker des Systems.
4. **Gelb** liegt als Auswahlhülle über Feldinhalt oder Werkzeug; es bewertet diesen Inhalt nicht.
5. `brand-red` und `empty-red` sind getrennte Token. Ein roter Betriebslagenkopf soll deshalb nie ein X imitieren.

## 3. Auswahl- und Rastergeometrie als Referenz

Die folgenden Maße sind **Entwurfsmaße in logischen Einheiten**, nicht eine beschlossene Touch-, Orientierung- oder Gerätepolitik:

| Referenzelement | Konkrete Konstruktion | Zweck der Konstruktion |
|---|---|---|
| Auswahlzelle | Zellgrund: 38 % `selection-yellow` auf `paper`, ohne Umfärbung des Inhalts; innenliegende `selection-edge`-Kontur 0,028C; Werkzeugfläche dagegen Vollton; Radius `0` | Technische, rechtwinklige Auswahl ohne Verwechslung mit einem Gleis |
| Rasterlinie | `grid-line`, 1 Einheit, ohne Schatten oder Farbverlauf | Zellen gliedern, ohne die eigene Lösung zu übertönen |
| Konkrete Gerade | `track-brown`, 0,18C breite Mittellinie mit bündigen Halbkantenzentrum-Ports, identisch zu Teil C | materielle, aber eindeutig lesbare konkrete Schiene |
| Graue Belegungsnotiz | `assumption-gray`, kantenfreies Miniatur-Standardgleis: zwei kurze parallele Striche plus drei Querschwellen innerhalb 0,36C×0,20C; keine Anschlussrichtung | sichtbar leiser als eine konkrete Schiene, bewusst formoffen |
| Leerannahme | zwei `empty-red`-Diagonalen im 0,56C-Quadrat, je 0,04C breit | X ist eindeutig eine Notiz, nicht ein Statusbadge |
| 4×4-Referenz | Zellpitch 72, Zähllane 28, Randzahl 16/20 zentriert | großzügige Frühlevel-Illustration, keine Mindestgröße |
| 10×10-Referenz | Zellpitch 32, Zähllane 27, Randzahl 11 zentriert | dichte Referenz für große Raster, keine Laufzeitvorgabe |

Teil C ist die maßgebliche Konstruktionsquelle für Rasterdarstellung; die kleine B-Legende zeigt nur einen Ausschnitt, kein weiteres 4×4-Level. Die Zähllane liegt außerhalb der Zellen; eine Randzahl wird nicht in eine Schiene oder Auswahlfläche geschrieben. `grid-line` bleibt an einer gelb ausgewählten Zelle sichtbar, während die 2-Einheiten-Kontur darüberliegt. Dadurch sind Auswahl und Zellgeometrie zwei getrennte visuelle Ebenen.

## 4. Kontrastwerte – nachvollziehbare Auswahl

Die Werte unten wurden aus den zehn sRGB-Token per Code als Verhältnis relativer Luminanz berechnet. Die Berechnung linearisert jeden sRGB-Kanal und verwendet `(Lhell + 0.05) / (Ldunkel + 0.05)`; die Reihenfolge der Farben ändert das Verhältnis nicht.

```python
# Eingabe: ein #RRGGBB-Tokenpaar
channel = c / 12.92 if c <= 0.04045 else ((c + 0.055) / 1.055) ** 2.4
luminance = 0.2126 * r + 0.7152 * g + 0.0722 * b
ratio = (max(luminance_a, luminance_b) + 0.05) / (min(luminance_a, luminance_b) + 0.05)
```

| Vordergrund auf Hintergrund | Verhältnis | Empfehlung für diese Referenz |
|---|---:|---|
| `ink` auf `paper` | 15.13:1 | Primärtext, Überschriften, Randzahlen und freigegebener Kartenstrang |
| `ink` auf `surface` | 13.53:1 | Fließtext und Informationszeilen auf warmer Fläche |
| `paper` auf `ink` | 15.13:1 | Invertierte kurze Beschriftung, wenn eine dunkle technische Fläche nötig ist |
| `paper` auf `brand-red` | 4.99:1 | Kurze weiße Statuszeile auf roter Betriebslagenfläche |
| `brand-red` auf `paper` | 4.99:1 | Roter Statusakzent oder kurze Überschrift auf Weiß |
| `paper` auf `empty-red` | 6.19:1 | Legende zur X-Notiz; das X selbst bleibt ohne Textfüllung |
| `paper` auf `track-brown` | 6.42:1 | Legende einer konkreten Gleisform, nicht der Gleiskern im Raster |
| `ink` auf `selection-yellow` | 9.44:1 | Werkzeugname oder kurze Auswahlbeschreibung innerhalb der gelben Markierung |
| `selection-edge` auf `paper` | 6.97:1 | Rein geometrische Auswahlkontur auf Papier |
| `empty-red` auf `surface` | 5.54:1 | X-Zeichen auf warmer Legendenfläche |
| `track-brown` auf `surface` | 5.75:1 | Gleisform auf warmer Muster-/Legendenfläche |

| Nicht geeignete Lesepaarung | Verhältnis | Konsequenz in dieser Empfehlung |
|---|---:|---|
| `assumption-gray` auf `paper` | 4.31:1 | Keine kritische Fließtext- oder Randzahlfarbe; nur die kurze Form-Note oder sekundäre, nicht allein tragende Darstellung |
| `assumption-gray` auf `surface` | 3.86:1 | Nicht für Informationstext einsetzen |
| `selection-edge` auf `selection-yellow` | 4.35:1 | Kontur auf Gelb, aber kein kleiner Auswahltext; Text bleibt `ink` |
| `brand-red` auf `surface` | 4.46:1 | Nicht für neue kleine entscheidende Beschriftung vorsehen; auf `paper` wechseln |
| `selection-yellow` auf `paper` | 1.60:1 | Nie als Text, Randzahl oder alleinige Feldsemantik |
| `grid-line` auf `paper` | 1.55:1 | Ausschließlich als unbeschriftete Gliederungslinie |

Diese Zahlen sind ein transparenter Gestaltungsnachweis, **keine** Erklärung einer barrierefreien, rechtlichen oder produktionsreifen Freigabe. Die noch offene Produktentscheidung zur Accessibility bleibt unberührt; reale Zielgeräte, Skalierung, Kontrastmodi und Nutzertests sind später separat zu bestimmen.

## 5. Typohierarchie und Laufweiten

Alle Größen sind Referenzgrößen in logischen Einheiten. Sie beschreiben keine fixe Geräteskalierung und setzen weder Mindest- noch Höchstwerte für eine künftige Accessibilitypolitik fest. Die Spationierung wird relativ zur jeweiligen Schriftgröße angegeben, damit sie in statischen Konstruktionen reproduzierbar bleibt.

| ID / Einsatz | Familie und Schnitt | Größe / Zeilenhöhe | Laufweite / Schreibweise | Konkrete Regel |
|---|---|---:|---|---|
| T1 Wortmarke `STAMMSTRECKEN-PUZZLE` | IBM Plex Sans Condensed SemiBold | 28 / 30 | `+0.026em`, Versalien | Eine Zeile; keine Buchstabenform, kein Monogramm und kein rotes Quadrat als Ersatzzeichen |
| T2 Ergebnis- und Screentitel `STRECKE FREIGEGEBEN` | IBM Plex Sans Condensed SemiBold | 20 / 24 | `+0.015em`, Versalien | Maximal zwei kurze Zeilen; Ausrichtung am Informationsblock, nicht am Rasterinhalt |
| T3 Betriebslagenstatus `BETRIEBSLAGE: BEEINTRÄCHTIGT` | IBM Plex Sans Condensed SemiBold | 14 / 18 | `+0.075em`, Versalien | Ein Statuslabel; erhöhtes Spatium signalisiert Verwaltungston, nicht Alarm |
| T4 Abschnitt/Kontext `Bauabschnitt Mitte · Meldung 07` | IBM Plex Sans SemiBold | 18 / 22 | `0em`, Satzschreibung | Titel der Meldung oder Abschnitt; kein Ersatz für die T3-Statuszeile |
| T5 Lesetext / Kurzfolge | IBM Plex Sans Regular | 15 / 22 | `0em`, Satzschreibung | Ein bis zwei ruhige Zeilen, kein vollständiger Fließroman im Rätselkopf |
| T6 Bedienlabel `NÄCHSTE STÖRUNG` | IBM Plex Sans SemiBold | 12 / 16 | `+0.040em`, Versalien | Kurze Handlung; Beschriftung bleibt innerhalb einer Zeile der Taste |
| T7 Randzahl / Zeit / Zähler | IBM Plex Sans SemiBold | 16 / 20 | `0em`, linierende tabellarische Ziffern | Präzise Zahlenmetrik; in der Raster-Zähllane zentriert |
| T8 Meta `Raster: 6×6 · Bisher: 1 Stern` | IBM Plex Sans Regular | 12 / 16 | `+0.010em`, Satzschreibung | Untergeordnet; Ziffern weiterhin tabellarisch und linierend |

**Hierarchieregeln:** T1–T3 sind die Versal-Auszeichnungsstufen der Empfehlung. T4–T5 bleiben in normaler deutscher Satzschreibung, damit die Status- und Verwaltungsebene nicht den gesamten Text in Anschlag verwandelt. T6 darf die Hauptaktion verdichten, T8 darf sie nie überstrahlen. Zwischen Statuslabel und Titel liegen 4 Einheiten; zwischen Titel und Fließtext 8 Einheiten; zwischen zwei unabhängigen Informationsblöcken 16 Einheiten.

Die vorgesehenen Schnitte sind absichtlich auf die drei gelieferten Dateien begrenzt: UI-Regular 400, UI-SemiBold 600 und Condensed-SemiBold 600. Es wird weder ein fehlender Bold-/Italic-Schnitt simuliert noch eine Ersatz- oder offizielle Bahn-Schrift vorgeschlagen.

## 6. Zahlen, Maße und Informationsrhythmus

Ziffern erhalten `tnum` und `lnum`: Alle Ziffern stehen gleich breit und auf derselben Grundlinie. Diese Empfehlung hält Randzahlen, Zeit und Fortschrittswerte beim Wechsel von `09` auf `10` ohne sichtbares Springen.

| Zahlenkontext | Darstellung | Beispiel | Nicht verwenden |
|---|---|---|---|
| Timer nach erster Eingabe | T7, tabellarisch/linierend, `MM:SS` mit führender Null | `04:27` | proportionale oder altstilartige Ziffern |
| Randzahl | T7, zentriert in eigener Zähllane | `2` | Farbe als Lösungsurteil; sie bleibt offen/dunkel, erfüllt/grau oder überschritten/kurz rot gemäß UI-Spezifikation |
| Rastermaß | T8 mit Multiplikationszeichen `×` | `6×6`, `10×10` | ASCII-`x` als Ersatzzeichen |
| Meldungsindex | T4/T8, zweistellig, wenn der Kontext eine Folge zeigt | `Meldung 07` | das Level als technische ID vor Nutzenden ausgeben |
| Stern-/Punktestand | T8, tabellarisch, Wert und Bedeutung getrennt | `1 Stern – Angekommen` | Gelb als automatische Belohnungsfarbe |

Randzahlen nutzen die bestehende Zustandslogik: offen `ink`, exakt erfüllt `assumption-gray`, objektiv überschritten kurz `empty-red`. Diese Farbe zeigt ausschließlich zählbare Belegung, keine korrekte Form, keinen Lösungsweg und keinen Wert der Spielweise. Die Empfehlung bewahrt damit die Gleichwertigkeit von direkten Gleisen und freiwilligen Planungsnotizen.

## 7. Einsatz nach bestehenden Oberflächen

| Bestehender Kontext | Typo- und Farbzuordnung | Abgrenzung |
|---|---|---|
| Betriebslage / Start | T1 Wortmarke; T3 Status auf `brand-red`; T4 Abschnitt auf `paper` oder `surface`; T6 dominante Aktion | Keine Reise-, Ticket- oder Echtzeitinformation suggerieren |
| Betriebslage-Karte | `ink` für freigegebenen Strang; `brand-red` für aktuellen Problemknoten; `assumption-gray` für noch nicht erreichbaren Abschnitt | Karte bleibt Spielkarte, nicht Verkehrsauskunft |
| Aktives Rätsel | T4/T5 in Kopfzeile; T7 für Randzahlen; `paper` plus `grid-line` für Raster | Keine Währung, Werbung oder untere Navigation hinzufügen |
| Werkzeugleiste | T6 für Label; `selection-yellow` plus 2-Einheiten-`selection-edge` nur für aktive Auswahl | Gelb ist keine Gleis- oder Richtigkeitsfarbe |
| Abschluss nach Zugfahrt | T2 Überschrift; T7 Zeit; T8 Sterne/Punkte; `brand-red` nur als Betriebsakzent | Ergebnisfläche erscheint erst nach Path-Hervorhebung und A–B-Fahrt |
| Betriebswerk | T2 Abschnittstitel; T4 Objektname; T5 trockene Beschreibung | Kosmetik bleibt rein kosmetisch; keine Preis- oder Katalogentscheidung wird hier festgelegt |

## 8. Lizenz- und Referenzgrenze

Die Originalquellen für IBM Plex wurden an IBM-Originalstellen überprüft. [`evidence/font-provenance.md`](evidence/font-provenance.md) pinnt Repositorycommit, Original-URLs, Bytegrößen, eingebettete Namen und SHA-256 aller drei TTFs sowie der Lizenz. [`evidence/font-license-original.zip`](evidence/font-license-original.zip) enthält den bytegleichen Originaltext der SIL Open Font License 1.1; die TXT-Datei ist die lesbare, zeilenendnormalisierte Kopie.

Die OFL erlaubt nach ihrem Text unter Bedingungen unter anderem Einbettung und Weitergabe; sie ersetzt keine Marken-, Produkt-, Store- oder Rechtsfreigabe. Die TTFs bleiben hier dokumentarische Referenzen. Eine spätere Subsetbildung, Konvertierung, Umbenennung oder tatsächliche Runtimeeinbindung muss den dort dokumentierten Änderungs- und Weitergabegrenzen folgen und separat freigegeben werden.

## 9. Offene Freigaben vor Produktionsverwendung

1. Der Owner muss diese konkreten Größen, Laufweiten, Rastermaße und Farbverwendungsregeln ausdrücklich annehmen oder ändern.
2. Die offene Accessibilityentscheidung muss künftige Skalierung, Kontrastmodi, Haptik und reale Zielgeräte separat festlegen; die obigen Verhältnisse treffen diese Entscheidung nicht vorweg.
3. Vor Produktimport bleiben Lizenzhinweisplatzierung, endgültige Einbettungsform, Markenprüfung und Runtime-/Zielgeräteprüfung externe Folgearbeiten.
4. Keine Referenzseite, kein Token und keine beschaffte Schriftdatei autorisiert eine Unity- oder Addressables-Übernahme. Der nachgelagerte Pfad und seine Lizenzinventur bleiben in der Content-Pipeline geregelt.

## Referenzen

- [Owner-Kontext und gemeinsame Token](evidence/OWNER_CONTEXT_AND_DESIGN_BRIEF.md)
- [Konzept 10 – Visuelle Identität und Stammstrecke](../../../Stammstrecken_Puzzle_Konzept_00-15/10_Visuelle_Identitaet_und_Stammstrecke.md)
- [Konzept 12 – UI- und Bedienungsspezifikation](../../../Stammstrecken_Puzzle_Konzept_00-15/12_UI_und_Bedienungsspezifikation.md)
- [Architektur – Content Pipeline](../../../ARCHITECTURE/CONTENT_PIPELINE.md)
- [IBM-Plex-Provenienz, Hashes und Lizenzgrenzen](evidence/font-provenance.md)
- [Statische Farb- und Typoreferenz](references/02-type-color.html)
- [Maschinenlesbare Token](references/design-tokens.json)
