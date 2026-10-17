# WP-035 – Konsistenz und Eigenprüfung nach Owner-Stilkorrektur

## 1. Kein technischer PASS ersetzt die gestalterische Anforderung

Der Owner hat am 17.10.2026 die erste Fahrzeug-/Weltbildlieferung ausdrücklich zurückgewiesen. Die Bilder waren pseudo-realistische Modellbahnansichten statt der gewünschten Cartoon-/Comic-Spielgrafik. Dies war ein Auslegungs- und Sichtprüfungsfehler des ausführenden Agenten. Bereits Konzept 10 schloss Fotorealismus aus; der Fehler wird nicht als Quellenwiderspruch entschuldigt.

Die alten Datei-/Hash-/Scope-/Architekturbelege sind technische Historie, **keine** Stilfreigabe. Die [unmittelbare Ownerkorrektur](OWNER_STYLE_CORRECTION_2026-10-17.md) legt Cartoon-/Comic-Stil, plump-staubig-behördenhafte Anpassung und glasklare Deutsche-Bahn-Anspielung als Kern fest. Die neue konkrete Ausführung ist nicht automatisch angenommen.

## 2. Durchgängige Korrektur statt Bildaustausch

| Gegenstand | Korrigierte gemeinsame Richtung |
|---|---|
| Fahrzeug | Stämmiger roter Nahverkehrswagen: L=2,60H, W=0,90H; breite getrennte Scheiben, helle Türen/Bauchzone, dunkle Kappe/Laufwerkgruppen. |
| Grafischer Aufbau | Kräftige Außenkontur 0,025H, innere Teilung halb so stark; wenige große gezeichnete Flächen. |
| Material | Zwei bis drei flache Schattentöne, grafische Staubpatches; PBR-Rauheitsziele und photorealistische Detailtextur entfernt. |
| Welt | Blockige Tunnel/Brücken/Plattformen, übergroße Amtsobjekte, klare Bahnsteig-/Baustellen-/SEV-Situationen; dieselben fünf Abschnitte und 20 Routenziele. |
| Illustrationsfarbe | Boden, Schatten, Vegetation und Weltbeschilderung dürfen eigene gedämpfte Zeichnungsfarben haben; daraus entstehen keine neuen UI-/Puzzlezustände. |
| UI/Marke | Dienstliche Bahn-App-Anmutung und Informationsdisziplin bleiben der bewusste Kontrast zur Cartoonwelt. Keine Fantasy-/Candy-Identität und keine offizielle Kennzeichnung. |
| Rätsel | Sechs Halbkantenzentrum-Formen, 18/100-Strich, Viertelkreisradius 50/100 unverändert; Grau kantenfreie offene Note, X optionale Leerannahme, Gelb separate Auswahl. |
| Counts | Ausschließlich dargestellte Belegung; keine versteckte Zielgeometrie oder Lösungsbewertung. |
| Puzzlefahrt | Grafischer Körpervorschlag 0,78C lang, 0,27C breit; unverändert echter A–B-Pfad. |
| Abschluss | Hilfen zurücknehmen → tatsächlicher Pfad → Zug A–B → Ergebnis. |
| Sound | Selbst erzeugter prägnanter Bahn-/Amtsmaterialcharakter als Brief, keine reale Ansagensimulation, keine bereits gelieferten Clips. |

Kapitel A–G, Hauptreport, Designbrief, Fahrzeug-/Weltkonstruktionsquellen, Screenfahrzeuge, Inventarverweise und Bildbriefe werden entsprechend nachgeführt. Die bestehenden Quellen unter Konzept/Architektur/Assets/Content/Workflows sind unverändert; das eigene Manifest bleibt unverändert.

## 3. Neue Bildherkunft und tatsächliche Sichtprüfung

`05-vehicles-props-cartoon.png` und `06-worlds-cartoon.png` wurden am 17.10.2026 gemeinsam über Manus `generate_image` neu erzeugt, jeweils 2560×1440 PNG. **Keine Eingabereferenzen:** die abgelehnten Bilder wurden weder neu geöffnet noch als Stil-/Kompositionsinput verwendet. Sie sind aus dem aktuellen Dateibaum entfernt, bleiben nur Git-Historie.

Die beiden **neuen** Tafeln wurden geöffnet und einmal auf die ausdrückliche Stilvorgabe geprüft: klar gezeichnete Konturen, vereinfachte Volumen und Farbflächen, stämmige rote Nahverkehrszüge, erkennbare deutsche Bahn-Alltags-/SEV-Anspielung; keine Fotorealismus- oder Modellbahnfotografie. Das ist eine Eigenprüfung und keine behauptete Ownerannahme oder unabhängige QC.

Generierte Titel sind keine exakte Produktwortmarke/Originalfontsetzung. Das gezeichnete Fahrzeugemblem ist keine Produktionsfreigabe: für Ableitung gilt der eigene Gleisknoten aus Kapitel A. Texte, Zahlen, Figuren und Zusatzdetails sind illustrative Ton-/Raumbeispiele, keine finale Textbibliothek, Fahrplandaten, neuen Produktionsmengen oder Produktregeln. Diese Abgrenzung rechtfertigt keine Rückkehr zum Realismus.

Die Konstruktionsblätter sind reproduzierbare flache Quellen für Relation und gemeinsame Formen. Sie ersetzen die neue Cartoon-Art-Direction nicht durch technische Modelltreue. Alle betroffenen HTML-Ansichten werden mit den unveränderten Original-TTFs neu gerendert.

## 4. Belege und Prüfgrenzen

Die konkrete Lieferprüfung berechnet nach der Korrektur lokale Markdown-/HTML-/Font-/CSV-Verweise, Dateilesbarkeit, 40 eindeutige Inventarzeilen, drei Originalfont-Hashes/deutsche Glyphen, Original-Lizenzbytes im ZIP, 28 Countzustände und 17 Kontrastpaare neu. `delivery-validation.json`, `render-results.json` und `file-manifest.json` beziehen sich auf die korrigierten Dateien. Die Datei-/Link-/Inventarprüfung ist bestanden (40 Anforderungen, 28 Countzustände, 17 Kontrastpaare); die vier geänderten statischen PNGs wurden gesichtet. [Lokaler Korrektur-PASS](architecture-cartoon-local-pass.txt) liegt vor. Der bestehende Architekturvalidator wird separat mit dem eigenen unveränderten Scope und `--self-test` in der gepinnten Umgebung erneut ausgeführt. Ein solcher PASS behauptet keine Designfreigabe.

Draft-PR #30 bleibt offen, kein Merge. Der vorherige Head `64b1cadd260958bcc34e3b2af366b2ebb5d4bbfb` hatte zwei reguläre `validate`-FAILs; mehrere Unityjobs waren bereits erfolgreich, andere noch laufend. Dieser Stand wird nicht zum PASS umgedeutet und nicht auf den Korrekturhead übertragen. Die CI-Abschlussstatusabfrage wurde durch den Nutzerstopp beendet; daraus folgt kein Abschlussnachweis. Neue reguläre CI startet durch den normalen Push; deren Ergebnis wird am tatsächlichen Head ausgewiesen, kein Ersatzlauf oder Workflowänderung.

Kein menschlicher Spiel-/Hörtest, Zielgeräte-/Accessibilitynachweis, rechtliche Endprüfung, Unityimport, Audio, Animation oder funktionsfähige neue Gestaltung wurde geliefert oder als bestanden behauptet. WP-035 bleibt offen.

## 5. Unveränderter Anker

WP/Manifest-Erst-Add `ec7ed3aa35c40363b0ccb0842bdb14c3fee28039` auf Mainbasis `d90e798dd9b26d3f152f3b0c2d1d52cae641eb16`. Der zuvor noch unveröffentlichte eigene ID-Formfehler wurde vor Erstveröffentlichung korrigiert; fremde Anker wurden nicht verändert. Der Stilkorrekturauftrag liegt innerhalb desselben Dokumentationsscopes; keine neue Governance-/Abnahmeserie oder Produktionsplanung.
