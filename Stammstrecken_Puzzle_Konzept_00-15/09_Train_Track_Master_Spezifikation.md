# Stammstrecken-Puzzle – Master-Spezifikation

**Status:** Maßgebliche strategische Gesamtfassung nach dem Bahn-Alltags-Pivot  
**Geltung:** Bei Widerspruch hat eine spätere, ausdrücklich bestätigte Fachdatei Vorrang.  
**Zweck:** Ein späterer Chat, Designer oder Entwickler soll ohne Kenntnis des Gesprächs verstehen, was gebaut wird, welche Entscheidungen verbindlich sind und welche Details noch ausstehen.

## 1. Produktversprechen

**Stammstrecken-Puzzle** ist ein deutschsprachiges Mobile-Logikspiel über ein fiktives, bösartig vertrautes deutsches Bahnnetz. Der Spieler löst klare Rasterrätsel und schafft damit etwas, das im satirischen Kontext außergewöhnlich wirkt: eine funktionierende Verbindung von A nach B.

> **Ein perfektes Logikspiel über ein System, das sich jeder Logik verweigert.**

Das Produkt verbindet drei Ebenen:

| Ebene | Funktion |
|---|---|
| Fairer Rätselkern | Ein zugängliches, tiefes Raster-Schienenpuzzle mit eindeutiger Lösung. |
| Persönliche Meisterschaft | Spieler werden sichtbar schneller, sicherer und direkter, ohne zum Einsatz von Hilfsmarkierungen gezwungen zu sein. |
| Bahn-Alltags-Satire | Betriebslage, Störungsmeldungen, Schienenersatzverkehr und trockene Verwaltungssprache geben den Erfolgen einen unverwechselbaren deutschen Kontext. |

Die Satire soll für regelmäßige Bahnfahrende auf den ersten Blick verständlich sein. Sie nimmt Systemfrust, Pendleralltag und den Kontrast aus offizieller Sprache und tatsächlicher Lage aufs Korn. Sie macht keine realen Unfälle, Betroffenen oder Mitarbeitenden zur Pointe.

## 2. Nicht verhandelbare Produktprinzipien

| Prinzip | Konsequenz |
|---|---|
| Rätselkern vor Metagame | Jede Welt, Belohnung, Werbung und Sammlung verstärkt das Lösen, ersetzt es aber nie. |
| Faire Logik | Keine Rätselvorteile durch Kauf, Zufall oder Werbung. |
| Freier Spielstil | Hilfsmarkierungen sind verfügbar, aber niemals vorgeschrieben. |
| Konzentration schützen | Während eines Rätsels keine Werbung und kein visueller Ballast. |
| Leistung sichtbar machen | Jede gelöste Strecke führt zu einer sichtbaren Zugfahrt und freigegebenen Verbindung. |
| Gezielte Sammlung | Fiktive Züge und Betriebsverbesserungen sind klar erspiel- oder auswählbar, nie in Lootboxen versteckt. |
| Keine künstlichen Bremsen | Keine Leben, keine Wartezeiten, keine Strafe für Fehler und kein Verlust erzielter Fortschritte. |
| Satire statt Täuschung | Das Spiel ist klar ein unabhängiges Puzzle-Spiel, keine Reise-, Ticket- oder Echtzeit-App. |

## 3. Der bestätigte Rätselkern

### 3.1 Ziel eines Levels

Eine **Störungsmeldung** ist das einzelne Puzzle-Level. Sie besteht aus einem rechteckigen Raster mit zwei Außenanschlüssen, A und B. Zahlen am Rand geben vor, wie viele Felder jeder Zeile und Spalte eine Schiene enthalten müssen.

Der Spieler baut eine einzige zusammenhängende Strecke zwischen A und B.

| Gültigkeitsbedingung | Bedeutung |
|---|---|
| Verbindung | A und B sind durch genau eine zusammenhängende Strecke verbunden. |
| Randzahlen | Jede Zeile und Spalte enthält exakt die geforderte Zahl an Gleisfeldern. |
| Keine Kreuzung | Kein Feld enthält mehr als einen Gleisabschnitt. |
| Keine Schleife | Es entsteht keine geschlossene Teilrunde. |
| Passende Anschlüsse | Eine konkrete Gleisform endet nicht im Raster oder an einer falschen Außenkante. |

**Season 1 verwendet keine Sonderfelder und keine zusätzlichen Rätselregeln.** Sie enthält insbesondere keine Weichen, Brücken, Kreuzungen oder T-Knoten. Ihre gesamte Tiefe entsteht aus Randzahlen, A/B-Anschlüssen, der Verteilung der sechs Gleisformen, Feldgrößen und der logisch notwendigen Schlussketten.

### 3.2 Sechs konkrete Gleisformen

Es gibt sechs braune konkrete Gleisformen: eine senkrechte Gerade, eine waagerechte Gerade und vier 90°-Kurven.

### 3.3 Feldinhalt und Auswahlzustand

| Sichtbares Element | Bedeutung |
|---|---|
| Leeres Feld | Unberührt oder vollständig geleert. |
| Rotes X | Optionale Spielermarkierung: Dieses Feld wird voraussichtlich leer bleiben. |
| Graues kleines Standardgleis | Optionale Spielermarkierung: Hier muss oder sollte irgendeine Schiene liegen; die konkrete Form ist offen. |
| Braune Gleisform | Konkret gesetzte Gerade oder Kurve. |
| Gelb | Ausschließlich momentane UI-Auswahl, unabhängig vom Feldinhalt. |

Die Semantik ist verbindlich: **Braun = konkrete Schiene; Grau = offene Belegungsannahme; Rot = Leerannahme; Gelb = aktuelle Auswahl.** Gelb ist niemals eine Gleisfarbe und niemals eine Wahrheitsaussage des Systems.

### 3.4 Werkzeuge

Die Werkzeugleiste umfasst die sechs Gleisformen, rotes X, graues Standardgleis, vollständiges Entfernen, Mehrfachauswahl, Undo und Hinweise. Mehrfachauswahl erlaubt die effiziente Anwendung derselben gewählten Markierung oder Form auf mehrere Felder.

## 4. Unterschiedliche, gleichwertige Spielstile

Es gibt keine getrennten Anfänger- oder Expertenmodi. Jedes Rätsel besitzt dieselben Regeln und dieselbe Werkzeugpalette.

| Spielstil | Verhalten |
|---|---|
| Strukturiert | Spieler nutzt X und graue Belegungsmarkierungen als sichtbare Logiknotizen. |
| Gemischt | Offensichtliche konkrete Gleise werden direkt gesetzt; unsichere Bereiche erhalten optional Hilfsmarkierungen. |
| Direkt | Spieler erkennt den Lösungsweg schnell und setzt braune Gleise ohne Hilfsmarkierungen. |

Sorgfältiges Planen ist vollständig gültig. Direkte Lösungen sind später ein Meisterschaftsmerkmal, aber keine Voraussetzung für Sterne oder Kampagnenfortschritt.

## 5. Schwierigkeit und Content

### 5.1 Schwierigkeit versus Meisterschaft

Die objektive Rätselschwierigkeit entsteht durch Feldgröße, Randzahlen, Lage von A/B, Streckendichte, Ausschlussstruktur, Tiefe der notwendigen Schlussketten und konkrete Gleisgeometrie. Sie hängt nicht von Zeit, Hilfsmarkierungen oder dem individuellen Stil ab.

Meisterschaft beschreibt dagegen die persönliche Entwicklung beim selben Rätsel: Bestzeit, direkte Lösung ohne Hilfsmarkierungen, weniger Korrekturen und sichere Ausführung.

### 5.2 Feldgrößen und Lernkurve

| Stufe | Typische Raster | Lernfokus |
|---|---:|---|
| Einstieg | 4×4 bis 4×5 | Randzahlen, A/B-Anschlüsse und Grundformen verstehen. |
| Grundlagen | 5×5 bis 6×5 | Belegung logisch eingrenzen; optionale Notizen verstehen. |
| Planung | 6×6 bis 7×6 | Mehrere Hinweise und lokale Schlussketten verbinden. |
| Strategie | 7×7 bis 8×8 | Längere Abhängigkeiten und engere Vorgaben überblicken. |
| Meisterschaft | 9×9 bis 10×10 und darüber | Große, verzahnte Schlussketten geduldig lösen. |

Die Feldgröße allein ist keine Schwierigkeitsmetrik. Gute Level variieren zusätzlich A/B-Positionen, Dichte, Zahlverteilung, zwangsläufig leere Felder und die Zahl der echten logischen Verzweigungen.

### 5.3 Kampagne und Dauerbaustelle

Die Kampagne enthält handverlesene, eindeutig lösbare und didaktisch sinnvoll angeordnete Störungsmeldungen. Sie ist das Qualitätsversprechen des Spiels. **Season 1 umfasst 240 Meldungen in fünf Netzabschnitten mit je vier Routen à zwölf Meldungen.** Jede Route besitzt einen satirischen Mini-Handlungsbogen, einen klaren Lernschwerpunkt und ein sichtbares Ziel auf der Betriebslage-Karte. Die vollständige Reihenfolge und Produktionsstruktur stehen verbindlich in `14_Season_1_Content_Bible.md`.

Ein späterer Endlosmodus heißt **Dauerbaustelle**. Er liefert zufallsgenerierte Rätsel erst nach der Kampagne. Jeder Generator-Kandidat muss eine gültige Strecke erzeugen, Randzahlen daraus ableiten, exakt eine Lösung nachweisen und gegen Qualitätsregeln gefiltert werden.

## 6. Level-Erlebnis

Jede Störungsmeldung folgt derselben emotionalen Kurve.

| Moment | Erfahrung |
|---|---|
| Start | Raster, A/B und Randzahlen stehen ohne Ablenkung im Mittelpunkt; eine kurze Störungszeile rahmt die Lage. |
| Denken | Ruhige, präzise Eingabe; alle Hilfsmittel sind optional und schnell korrigierbar. |
| Verdichtung | Die eigene Lösung wird geordneter und lesbarer; das System verrät keine weiteren Schlüsse. |
| Durchbruch | Die gültige Verbindung wird erkannt, ohne Werbung, Dialog oder Unterbrechung. |
| Auflösung | „Strecke freigegeben“: Der gewählte Zug fährt exakt die selbst gebaute Verbindung von A nach B. |
| Abschluss | Erst Anerkennung und Belohnung, danach erst freiwillige nächste Optionen. |

Während des Denkens bestätigt das Spiel nur die Eingabehandlung, nicht die spätere Richtigkeit einer plausiblen Annahme. Die starke emotionale Energie gehört an den Abschluss, weil sie dann unmittelbar aus der eigenen Leistung entsteht.

## 7. Betriebslage, Netzabschnitte und Sammlung

Die Kampagne erscheint als **Betriebslage-Karte**, nicht als Nummernliste. Der Spieler stabilisiert ein fiktives Netz abschnittsweise.

| Bisherige Spielfunktion | Stammstrecken-Puzzle-Fassung |
|---|---|
| Kampagnenkarte | Betriebslage-Karte. |
| Welt / Kapitel | Netzabschnitt. |
| Einzelnes Level | Störungsmeldung. |
| Lösung | Strecke freigegeben. |
| Shop / Sammlung | Betriebswerk. |
| Endlosmodus | Dauerbaustelle. |

Verbindliche Netzabschnitte als thematischer Rahmen sind **Stammstrecke**, **Bauabschnitt Mitte**, **Außenast ohne Halt**, **Umleitung über Irgendwo**, **Schienenersatzverkehr** und **Dauerbaustelle**. Sie geben Kulisse, Textton und Kartenfortschritt, verändern aber nicht wahllos die Rasterregeln.

Züge sind rein kosmetische Betriebsmittel. Sie beeinflussen keine Hinweise, Sterne, Zeitziele oder Rätselschwierigkeit. Sie fahren nach der Lösung über die selbst gebaute Strecke.

Im Betriebswerk werden gezielt fiktive Zugvarianten, Lackierungen, Signale, Bahnsteige, Wegweiser, Tunnelportale und Ersatzobjekte gesammelt. Mögliche Namen sind **Ersatzgarnitur**, **Funktionierender Zugteil**, **Wagenreihung nach Plan** oder **Anschlusswunder-Express**. Sie sind nicht endgültig und müssen später als Namensfamilie geprüft werden.

## 8. Sterne, Fortschritt und Geduldspunkte

Jede Störungsmeldung besitzt drei Sterne.

| Ergebnis | Stern | Bedeutung |
|---|---:|---|
| Korrekt gelöst | 1 | **Angekommen.** Die Verbindung steht und der Kampagnenweg geht weiter. |
| Gute levelbezogene Zeit | 2 | **Anschluss erwischt.** Die Verbindung war rechtzeitig genug. |
| Sehr gute levelbezogene Zeit | 3 | **Tatsächlich pünktlich.** Der seltene Ausnahmezustand ist erreicht. |

Zeitziele werden pro Level oder verlässlicher Schwierigkeitsklasse festgelegt; sie sind nicht pauschal. X- und graue Belegungsmarkierungen verhindern keine Sterne. Bereits verdiente Sterne gehen nicht verloren.

Die 240 kuratierten Standardlevel des Launches bilden **Season 1**. Vor Abschluss aller 240 Meldungen sind Wiederholungen reine Übungsfahrten ohne neue Sterne, offizielle Bestzeit oder Währung. Erst nach Abschluss der jeweiligen Season öffnet die **Betriebsrevision**: Dann dürfen deren alte Meldungen wieder bewertet werden. Eine bessere Zeit ersetzt die Sternwertung; fehlende Sternboni werden genau einmal nachgezahlt. Dies gilt entsprechend für spätere Seasons.

**Geduldspunkte** sind die Spielwährung. Die erste korrekte Lösung bringt 15 Punkte, Stern zwei zusätzlich 5 und Stern drei zusätzlich 10. Eine vollständige 12-Meldungen-Route bringt 60, ein kompletter 48-Meldungen-Netzabschnitt 150 Punkte. Freiwillige Werbeangebote und die Betriebslage des Tages ergänzen diese klaren Grundquellen. Geduldspunkte kaufen ausschließlich kosmetische Inhalte im Betriebswerk, keine Rätselvorteile, Sterne oder normalen Season-Zugang.

## 9. Betriebslage des Tages und Monetarisierung

Die frühere Glücksrad-Idee heißt **Betriebslage des Tages**. Sie ist ein kleines freiwilliges Tagesextra außerhalb eines Rätsels.

| Regel | Festlegung |
|---|---|
| Kostenlose Teilnahme | Einmal pro Kalendertag. |
| Zusätzliche Teilnahme | Höchstens einmal zusätzlich nach freiwilligem Video. |
| Belohnung | Geduldspunkte oder kosmetischer Fortschritt. |
| Ton | Trockene Meldung statt Casino-Inszenierung. |
| Grenzen | Keine kaufbaren Teilnahmen, keine Rätselvorteile und kein Verlust beim Auslassen. |

Zulässige Werbeformen sind freiwillige Videos für +10 Geduldspunkte nach der vollständigen Abschlussinszenierung, freiwillige Videos auf aktive Anfrage für einen Hinweis, begrenzte unterbrechende Anzeigen sowie eine dauerhafte Werbefrei-Option.

Die erste mögliche unterbrechende Anzeige erscheint erst nach der **zweiten** vollständig gelösten Meldung. Von Meldung 3 bis 8 erscheint sie höchstens nach jeder zweiten abgeschlossenen Meldung; ab Meldung 9 höchstens nach jeder dritten, begrenzt auf drei Unterbrechungen pro Spielsitzung. Die Betriebsrevision ist komfortabler (höchstens nach jeder fünften Wiederholung). Werbung ist strikt ausgeschlossen während des aktiven Rätsels, zwischen Eingabe und Wirkung, bei der Lösungserkennung, vor oder während der Zugfahrt, direkt vor dem ersten Level und nach Fehlern oder Abbruch.

## 10. Markenstimme und Satire

Die Markenstimme ist **trocken, präzise, frustriert-optimistisch und herrlich deutsch**. Sie nutzt allgemeine Bahnbegriffe wie Schienenersatzverkehr, Signalstörung, Weichenstörung, Stellwerkstörung, Haltausfall, Umleitung, Wagenreihung, Anschluss und mehr Reisezeit.

Die Sprache wird vollständig neu geschrieben. Tonproben sind:

- „Stammstrecke: derzeit uneingeschränkt eingeschränkt.“
- „Schienenersatzverkehr wird vorbereitet. Bitte lösen Sie schneller.“
- „Ihre Verbindung ist logisch. Der Fahrplan leider nicht.“
- „Wagenreihung erfolgreich geändert. Anlass wird nachgereicht.“
- „Anschluss nicht erreichbar. Bitte nehmen Sie den gedanklichen Umweg.“
- „Betrieb bis auf Weiteres stabil.“

Die Komik entsteht durch sachliche Form für absurden Zustand. Sie braucht keine Dauerwitze und keine kindlichen Figuren.

## 11. Visuelle Identität

Die visuelle Richtung lautet: **Premium-Miniaturbahnwelt in deutscher Betriebslagen-Anmutung**.

| Bereich | Verbindliche Richtung |
|---|---|
| Puzzle-Raster | Helle, präzise technische Arbeitsfläche mit klarer Farbsemantik. |
| Kopfzeile | Kompakte Betriebslage mit Netzabschnitt, Störungsname und Kurzstatus. |
| Marke | Verkehrsrote Akzente, helle Informationsflächen, Anthrazit und straffe eigene Groteskschrift. |
| Eigenes Zeichen | Unterbrochener Gleisknoten, bei dem zwei Schienen die Verbindung knapp verfehlen. |
| Erste Welt | Stammstrecke: fiktiver urbaner Knoten mit Tunnel, Bahnsteigen, Warnbaken, Baustellen und Umleitungswegweisern. |
| Züge | Eigenständige, stilisierte S-Bahn-/Regionalbahn-Anmutung mit eigener Front und eigener Lackierung. |
| Abschluss | Das Raster gewinnt Tiefe und Umgebung; die selbst gebaute Verbindung bleibt klar erkennbar. |

Die Nähe zu vertrauter deutscher Bahnkommunikation entsteht über Titel, Statushierarchie, Farbdramaturgie, Orte, Textton und Situationslogik. Nicht übernommen werden offizielle Kennzeichen, Logos, exakte Wort-Bild-Marken, Screens, Piktogramme, Schriften, Ansagen oder Echtzeitmeldungen.

## 12. Rechtliche Arbeitsleitplanken

Dies ist keine verbindliche Rechtsberatung. Titel, App-Icon, Store-Screenshots, Werbemittel, Zugdarstellungen und zentrale In-Game-Kennzeichen müssen vor Veröffentlichung marken- und urheberrechtlich geprüft werden.

Verbindlich zu vermeiden sind:

| Nicht zulässig | Konsequenz |
|---|---|
| Offizielle Unternehmens- oder Logo-Kennzeichnung | Keine Verwendung von „Deutsche Bahn“ oder „DB“ als Absender oder dominierendes Markenelement. |
| Logo-/Icon-Kopie | Kein nachgezeichnetes Monogramm im roten Quadrat und keine nahezu identische Wort-Bild-Marke. |
| Oberflächenkopie | Keine Nachbildung offizieller Navigations-, Ticket- oder Fahrplanaussichten. |
| Fremde Inhalte | Keine Originalansagen, Screenshots, Fotos, längeren Meldungstexte oder fremden Witze. |
| Herkunftstäuschung | Keine Ticket-, Reise- oder Echtzeitfunktion; Spielrolle und Unabhängigkeit bleiben klar. |

Die detaillierte Recherche und alle Quellen liegen in `11_Bahnfahrer_Lore_und_Satire_Leitplanken.md`.

## 13. Offene Produktentscheidungen

| Thema | Noch zu entscheiden |
|---|---|
| Finale Wort-/Bildmarke | Genaues App-Icon, Schriftlizenz, Farben und Logo-System nach professioneller Rechtsprüfung. |
| UI-Restdetails | Exakte Touch-Gesten für Mehrfachauswahl, Hinweisdarstellung, Zugänglichkeit und Motion-Dauer. Der verbindliche Screenaufbau steht in Konzeptbaustein 12. |
| Season-1-Detailcontent | Routen- und Lernarchitektur ist in Content-Bible 14 festgelegt. Als Nächstes folgen 240 konkrete Rätselinstanzen mit Solver-Nachweis, Leveldaten und finalen Zeitwerten. |
| Ökonomie | Punktwerte und Objektpreisrahmen sind beschlossen; finale Werbefrei-Preisgestaltung wird erst nach Plattform- und Marktprüfung festgelegt. |
| Zeitbalancing | Tatsächliche Zeitziele je Level beziehungsweise Schwierigkeitsklasse. |
| Zugkatalog | Finale Zugfamilien, Lackierungen, Sounds und Freischaltreihenfolge. |
| Sonderfelder | Für Season 1 ausdrücklich ausgeschlossen. Erst für spätere Seasons prüfen, wenn sie nachweislich neue Logiktiefe liefern und den klaren Kern nicht verwässern. |
| Produktionsplanung | Levelsolver, Generatorvalidierung, Asset-Pipeline, Audio, Analytics, QA und Veröffentlichung. |

## 14. Maßgebliche Fachdateien

| Datei | Inhalt |
|---|---|
| `01_Zielgruppen_und_Motivationspsychologie.md` | Evidenz- und Zielgruppenbasis für faire Langzeitmotivation. |
| `02_Referenzspiel_und_Baseline.md` | Präzise bestätigte Mechanik des Vorbilds. |
| `03_Raetselkern_und_Interaktionsmodell.md` | Feldzustände, Werkzeuglogik und gleichwertige Spielstile. |
| `04_Schwierigkeit_Feldgroessen_und_Levelgenerierung.md` | Rätselschwierigkeit, Feldgrößen, Kampagne und Generatorqualität. |
| `05_Level_Erlebnis_und_Qualitaetsdifferenzierung.md` | Konzentrationsphase, Störungsmeldung, Durchbruch und Abschlussinszenierung. |
| `06_Fortschritt_Belohnungen_und_Meisterschaft.md` | Sterne, Geduldspunkte, Netzfortschritt und persönliche Verbesserung. |
| `07_Zuege_Welten_und_Reisekarte.md` | Betriebslage-Karte, Netzabschnitte, Betriebswerk und kosmetische Betriebsmittel. |
| `08_Faire_Monetarisierung_und_Werbeangebote.md` | Werbung, Betriebslage des Tages und Werbefrei-Option. |
| `10_Visuelle_Identitaet_und_Stammstrecke.md` | Visuelle Kernidentität nach dem Pivot; Sonnental ist als späterer Außenast zurückgestellt. |
| `11_Bahnfahrer_Lore_und_Satire_Leitplanken.md` | Recherche, satirisches Vokabular sowie rechtliche und gestalterische Leitplanken. |
| `12_UI_und_Bedienungsspezifikation.md` | Startscreen, Betriebslage-Karte, Rätseloberfläche, Abschluss, Betriebswerk und Betriebslage des Tages. |
| `13_Oekonomie_und_Monetarisierungs_Balancing.md` | Season-1-Umfang, Punkte- und Preiswerte, Betriebsrevision, Hype-Besucher-Monetarisierung, Messrahmen und Quellenbasis. |
| `14_Season_1_Content_Bible.md` | Vollständige Season-1-Architektur: fünf Netzabschnitte, 20 Routen, 240 Meldungen, Lernbögen, satirische Rahmenhandlung und Produktionsschema je Rätsel. |
