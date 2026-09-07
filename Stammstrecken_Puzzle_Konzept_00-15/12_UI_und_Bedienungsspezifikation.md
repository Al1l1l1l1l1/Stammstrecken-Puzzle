# Stammstrecken-Puzzle – UI- und Bedienungsspezifikation

**Status:** Aktuelle, beschlossene Arbeitsfassung  
**Einordnung:** Konzeptbaustein 12  
**Zweck:** Diese Spezifikation definiert die Informationshierarchie, zentralen Screens, Bedienlogik und Nicht-Unterbrechungsregeln. Sie ist für spätere UI-Entwürfe und die Umsetzung verbindlich, soweit keine spätere ausdrücklich bestätigte Detailentscheidung sie ersetzt.

> **Die Oberfläche wirkt dienstlich. Die Lage ist absurd. Das Puzzle funktioniert.**

## 1. Globale Informationsarchitektur

Das Produkt besitzt nur drei dauerhafte Hauptziele. Die App darf nicht zu einem Menüfriedhof ausarten.

| Ziel | Funktion | Sichtbarkeit |
|---|---|---|
| **Betriebslage** | Startscreen, aktuelles nächstes Rätsel und vollständige Betriebslage-Karte. | Standardziel beim App-Start. |
| **Betriebswerk** | Sammlung von Zügen und kosmetischen Betriebsverbesserungen. | Dauerhaft über untere Navigation erreichbar. |
| **Profil** | Sterne, persönliche Bestzeiten, Meisterschaft und Einstellungen. | Dauerhaft über untere Navigation erreichbar. |

Die untere Navigation ist nur außerhalb eines aktiven Rätsels sichtbar. Im Rätsel existiert keine dauerhafte Navigation, keine Geduldspunkteanzeige und keine Werbung.

## 2. Einstieg und Startscreen

### 2.1 Erster App-Start

Der erste Start ist keine lange Story. Eine einzige satirische Einleitung führt direkt zum ersten 4×5-Tutorialrätsel:

> **„Auf der Stammstrecke läuft heute alles nach Plan. Vermutlich.“**

Danach wird das erste Level geöffnet. Die Einführung erklärt die Rasterregeln direkt durch Handlung, nicht durch lange Texttafeln.

### 2.2 Wiederkehrender Startscreen

Bei jeder späteren Öffnung landet der Spieler auf der Betriebslage mit genau einer zentralen nächsten Aktion.

| Bereich | Inhalt und Funktion |
|---|---|
| Kopfbereich | Eigene Wortmarke **STAMMSTRECKEN-PUZZLE**, darunter klein: „Unabhängiges satirisches Logikspiel“. Rechts: Geduldspunkte und Einstellungen. |
| Hauptstatus | Große Statuskarte: **BETRIEBSLAGE: BEEINTRÄCHTIGT** mit dem aktuellen Netzabschnitt und einer kurzen fiktiven Ursache. |
| Hauptaktion | Dominante Taste **STRECKE FREIGEBEN**. Sie öffnet immer die nächste sinnvolle Störungsmeldung. |
| Fortschrittsvorschau | Kurzer Blick auf die aktuelle Route mit freigegebenen Strecken und dem nächsten roten Problemknoten. |
| Zweite Aktion | **BETRIEBSLAGE ANZEIGEN** führt in die vollständige Karte. |
| Untere Navigation | Betriebslage, Betriebswerk, Profil. |

Der Startscreen zeigt keine unterbrechende Werbung, keinen Tagesbonus-Pop-up und keine Vielzahl gleich lauter Angebote. Die nächste Rätselhandlung ist immer dominant.

## 3. Betriebslage-Karte

Die Betriebslage-Karte ist eine eigenständige, stilisierte Spielkarte. Sie erinnert in Hierarchie und Ton an Bahnkommunikation, ist aber weder eine echte Netzkarte noch eine Reise-, Ticket- oder Echtzeitauskunft.

### 3.1 Status eines Kartenknotens

| Zustand | Darstellung | Spielerbedeutung |
|---|---|---|
| **Freigegeben** | Zusammenhängender dunkler Streckenstrang, eigener Zug sichtbar; Sterne klein am Abschnitt. | Das Rätsel ist gelöst und die Verbindung funktioniert. |
| **Aktuelle Störung** | Kräftiger roter Problemknoten mit kurzer Bezeichnung. | Dies ist die nächste empfohlene Aufgabe. |
| **Bekanntes Folgeproblem** | Sichtbarer, aber gedämpfter Knoten im weiteren Verlauf. | Der Spieler sieht die nächste Route, ohne mit vielen Leveln überfrachtet zu werden. |
| **Noch nicht erreichbar** | Zurückhaltender grauer Abschnitt hinter einem klaren Engpass. | Erst die vorherige Verbindung muss freigegeben werden. |

Die Karte macht Fortschritt räumlich lesbar: Ein gelöstes Puzzle ist nicht nur ein Häkchen, sondern ein wieder befahrbarer Teil des fiktiven Netzes.

### 3.2 Störungsmeldungskarte

Ein Antippen eines Kartenknotens öffnet eine kompakte Detailkarte vom unteren Rand.

| Element | Beispiel |
|---|---|
| Zugehörigkeit | **Bauabschnitt Mitte · Meldung 07** |
| Titel | **Weichenstörung am Knoten Süd** |
| Kurzfolge | „Verbindung Richtung Ostast beeinträchtigt.“ |
| Rätseldaten | „Raster: 6×6 · Bisher: 1 Stern – Angekommen“ |
| Primäre Aktion | **STRECKE FREIGEBEN** |
| Bereits gelöst | Primäre Aktion wird zu **BESTZEIT VERBESSERN**. |

Störungsnamen geben Ton und Ort. Sie fügen nicht automatisch neue Rätselregeln hinzu.

## 4. Rätseloberfläche

Das Raster ist die unangefochtene Hauptfläche. Die Oberfläche darf die Mechanik des Referenzspiels veredeln, aber nicht verändern oder durch Dekoration verdecken.

### 4.1 Kopfzeile

| Element | Regel |
|---|---|
| Zurück | Kehren zur Betriebslage zurück; eine offene Eingabe bleibt erhalten. |
| Kontext | Beispiel: **Bauabschnitt Mitte / Meldung 07**. |
| Störungsname | Beispiel: **Weichenstörung**; klein und kontextgebend. |
| Rückgängig | Ein klarer Schritt zurück. Langes Drücken öffnet die letzten Handlungen, nicht den gesamten Level-Reset. |
| Hinweis | Nur auf aktive Anfrage; nie automatisch aufdringlich. |

Während des Rätsels sind Geduldspunkte, untere Navigation, Werbung, Tagesbonus und Betriebswerk ausgeblendet.

### 4.2 Raster und Randzahlen

Das Raster skaliert so groß wie möglich und hält A, B sowie alle Randzahlen jederzeit lesbar. Bei großen Feldern wird klar skaliert, nicht mit Dekoration eingeengt.

| Element | Zustand | Bedeutung |
|---|---|---|
| Randzahl | Dunkel | Die geforderte Belegung ist noch nicht erreicht. |
| Randzahl | Zurückhaltend grau | Die geforderte Anzahl an Gleisfeldern ist exakt erfüllt. |
| Randzahl | Kurz rot | Die geforderte Anzahl ist überschritten. |

Diese Rückmeldung zeigt ausschließlich objektiv sichtbare Mengen. Sie verrät keine korrekte Lösung und bewertet keine plausible Annahme vorzeitig als richtig oder falsch.

### 4.3 Werkzeugleiste

Die Werkzeugleiste ist dauerhaft am unteren Rand sichtbar. Alle sechs konkreten Gleisformen bleiben direkt erreichbar; sie werden nicht hinter Untermenüs versteckt.

| Werkzeug | Funktion |
|---|---|
| Rotes X | Setzt eine optionale Leerannahme. |
| Graues Standardgleis | Setzt eine optionale Belegungsannahme; konkrete Form bleibt offen. |
| Sechs braune Gleisformen | Setzt eine konkrete senkrechte, waagerechte oder gekrümmte Schiene. |
| Leeren | Entfernt den bestehenden Feldinhalt vollständig. |
| Mehrfachauswahl | Markiert mehrere beliebige Felder für eine gemeinsame Anwendung des gewählten Werkzeugs. |

Die gewählte Option erhält eine gelbe Hervorhebung. **Gelb bedeutet ausschließlich: Dieses Werkzeug ist aktuell aktiv.** Es sagt nichts über die Richtigkeit oder den Inhalt eines Feldes aus.

### 4.4 Eingaben und Mehrfachauswahl

| Handlung | Verhalten |
|---|---|
| Einzelfeld antippen | Setzt das aktive Werkzeug auf genau dieses Feld. |
| Gleiches Werkzeug erneut anwenden | Löst keine unerwünschte zyklische Änderung aus. |
| Leeren anwenden | Entfernt X, graue Markierung oder konkrete Gleisform und stellt ein neutrales Feld her. |
| Mehrfachauswahl aktivieren | Spieler markiert zuerst mehrere Felder; danach wählt er Inhalt und bestätigt die Anwendung auf alle ausgewählten Felder. |
| Rückgängig | Hebt die letzte Handlung wieder auf; keine Strafe. |

Der Timer startet erst mit der **ersten Eingabe**, nicht beim Lesen der Störungsmeldung. Er bleibt klein und untergeordnet.

## 5. Abschlussbildschirm

Die Ergebnisinszenierung folgt einer nicht verhandelbaren Reihenfolge. Keine Werbung, kein Angebot und kein Pop-up darf sie unterbrechen.

1. Das Puzzle erkennt die gültige Verbindung.
2. Hilfsmarkierungen und technische Nebeninformationen treten zurück.
3. Die gebaute Strecke wird sichtbar hervorgehoben.
4. Der gewählte Zug fährt von A nach B.
5. Erst danach erscheint die Ergebnisfläche von unten.

| Element der Ergebnisfläche | Inhalt |
|---|---|
| Überschrift | **STRECKE FREIGEGEBEN** |
| Sterne | Nacheinander: **Angekommen**, **Anschluss erwischt**, **Tatsächlich pünktlich**. |
| Leistung | Zeit, neue Bestzeit und eventuelle Meisterschaftsmarkierung. |
| Währung | Verdiente Geduldspunkte. |
| Kartenfortschritt | Kleiner sichtbarer neu freigegebener Abschnitt. |
| Haupttaste | **NÄCHSTE STÖRUNG** |
| Zweite Taste | **ZUR BETRIEBSLAGE** |
| Freiwilliger Bonus | Ganz unten und klar getrennt: „Zusätzliche Geduld beantragen“ mit eindeutigem Betrag und freiwilligem Video. |

Der freiwillige Bonus verdoppelt nicht die erarbeitete Levelbelohnung und erscheint erst nach der vollständigen Würdigung der Leistung.

## 6. Betriebswerk

Das Betriebswerk ist die Sammlung und der Ort, an dem Geduldspunkte sichtbar eingesetzt werden. Es ist keine laute Shop-Wand und keine Karten-/Lootbox-Ansicht.

| Bereich | Inhalt |
|---|---|
| Kopf | **BETRIEBSWERK**, aktueller Geduldspunkte-Stand und kurze satirische Statuszeile. |
| Hauptobjekt | Der aktuell gewählte Zug steht groß und materiell in einer stilisierten Werkhalle. |
| Reiter 1 | **ZUGPARK**: fiktive Zugvarianten, Lackierungen und Waggonkonfigurationen. |
| Reiter 2 | **BETRIEBSVERBESSERUNGEN**: Signale, Bahnsteige, Wegweiser, Brücken, Tunnelportale und Ersatzobjekte. |
| Objektkarte | Ein Objekt, sein Preis oder klarer Meilenstein sowie eine kurze trockene Beschreibung. |

Alle Inhalte sind kosmetisch. Sie verändern keine Rätsellogik, keine Sterne und keine Hinweise.

## 7. Betriebslage des Tages

Die Betriebslage des Tages ist das tägliche freiwillige Extra. Sie wird nicht als buntes Casino-Rad inszeniert, sondern als mechanische Abfahrtsanzeige oder Meldetafel.

| Regel | Festlegung |
|---|---|
| Auffindbarkeit | Als kleine Karte in der Betriebslage, nie als erzwungener Start-Pop-up. |
| Teilnahme | Einmal pro Kalendertag kostenlos. |
| Zusätzliche Teilnahme | Höchstens einmal nach freiwilligem Video. |
| Ablauf | Die Meldetafel klappt durch mögliche Betriebslagen und bleibt auf einer Belohnung stehen. |
| Belohnung | Geduldspunkte oder kosmetischer Fortschritt. |
| Grenzen | Keine kaufbaren Teilnahmen, keine Sternen, keine Lösungen und keine spielentscheidenden Vorteile. |

Beispielhafte eigene Meldungen sind: „Signal wieder funktionsfähig: +15 Geduldspunkte“, „Anschluss rein theoretisch erreicht: +25“ und „Schienenersatzverkehr ordnungsgemäß improvisiert: kosmetischer Fortschritt“.

## 8. Nicht verhandelbare UX-Regeln

| Regel | Konsequenz |
|---|---|
| Nächste Handlung ist klar | Der Startscreen hat genau eine dominante Puzzle-Aktion. |
| Rätsel bleibt konzentriert | Keine Werbung, Währung, Navigation oder Tagesbonus im aktiven Level. |
| Bedeutung bleibt eindeutig | Braun, Grau, Rot und Gelb behalten ihre festgelegten semantischen Rollen. |
| Meisterschaft bleibt freiwillig | Sterne und Bestzeit motivieren, sperren aber keine normale Kampagne. |
| Angebot folgt Leistung | Freiwillige Werbung erscheint erst nach Abschlussinszenierung. |
| Satire unterstützt das Spiel | Betriebssprache und Statuszeilen rahmen den Fortschritt, verdecken aber nie das Puzzle. |
| Produktrolle ist klar | Keine Ticket-, Fahrplan- oder Echtzeitfunktion; das Produkt ist ein unabhängiges Puzzle-Spiel. |

## 9. Offene UI-Details

| Thema | Spätere Entscheidung |
|---|---|
| Konkrete Touch-Gesten | Exakte Auslösung und visuelle Form der Mehrfachauswahl, Drag-Optionen und Fehlertoleranzen. |
| Hinweissystem | Arten von Hinweisen, Darstellung und reguläre Bezugswege. |
| Barrierefreiheit | Schriftgrößen, Kontrast, Haptikoptionen, Tonsteuerung und Einhandbedienung. |
| Motion Design | Exakte Dauer und Kameraführung der Zugfahrt und Statusanimationen. |
| Werbefrequenz | Konkrete Obergrenzen pro Sitzung und genaue Abstände. |
| UI-Assetproduktion | Finale Icons, Schriftlizenz, Illustrationsraster und Komponentenbibliothek. |
