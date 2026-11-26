# Stammstrecken-Puzzle – Konzept für das Rundum-Rework

**Stand: 25.11.2026, Fassung 2.1 · Verfasser: Manus · Status: Entscheidungen von Alex eingearbeitet; keine zusätzliche Logoentscheidung oder Anwaltsprüfung als Voraussetzung**

Dieses Konzept beschreibt, wie das Spiel als Ganzes neu gebaut wird. Die Rätselregeln bleiben unverändert: Raster, Randzahlen, A und B, sechs Gleisformen, X und Grau als freiwillige Notizen und direkte Taps. Neu gedacht werden das Aussehen, die Satire, die Bedienung, der Ablauf, die Inhalte und die Sammlung.

---

## 1. Der Existenzgrund: die Bahn durch den Kakao ziehen

Stammstrecken-Puzzle gibt es aus einem einzigen Grund: **Jeder, der in Deutschland regelmäßig Bahn fährt, soll das Spiel sehen und sofort wissen, worauf es anspielt. Und er soll sich nicht entscheiden können, ob er lachen oder heulen soll.** Das Puzzle ist das Handwerk, die Satire ist das Produkt. Ohne sie wäre es ein weiterer Gleis-Puzzle-Klon, mit ihr ist es ein Meme, das Pendler sich gegenseitig schicken und Influencer aufgreifen.

Die Grundpointe ist seit dem Pivot festgelegt:

> **Ein perfektes Logikspiel über ein System, das sich jeder Logik verweigert.** Der Spieler schafft im Rätsel das, was das echte System notorisch nicht schafft: eine funktionierende Verbindung von A nach B.

Daraus folgt die Humorformel: **Offiziell formulierter Optimismus trifft auf konkrete Unplanbarkeit.** Jeder Witz besteht aus einer echten Störung, die man kennt (Signal, Weiche, SEV, Wagenreihung, Gleiswechsel, Aufzug, verpasster Anschluss), aus ruhiger, beschwichtigender Betriebssprache und aus der Wahrheit, die jeder Pendler sofort versteht.

### Was der bisherige Build falsch gemacht hat

Im aktuellen Build existiert die Satire fast nur als Textzeile über dem Raster, und dort ist sie zu oft verkopft: aneinandergereihte Amtsbegriffe statt einer Situation, die man vor sich sieht. Die Welt selbst ist nicht komisch, die Systeme sind nicht komisch, nichts bewegt sich komisch. Ein Bahnfahrer sieht ein weißes Formular mit einem roten Zug und erkennt nichts wieder.

**Die Korrektur lautet: Die Satire wandert aus dem Text in alles hinein,** also in Aufmachung, Welt, Spielsysteme, Figuren, Klang und die großen Momente. Text bleibt wichtig, wird aber kürzer, schärfer und situativ.

### Die fünf Satire-Schichten

| Schicht | Was dort satirisch ist | Beispiel |
|---|---|---|
| **1. Aufmachung** | Das Spiel sieht aus, als hätte der Bahn-Designer ein Spiel gebaut. | Blaue Abfahrtstafel, weiß-oranges Störungsbanner, Verkehrsrot, rote Nahverkehrszüge, Durchsage-Gong, Fallblatt. |
| **2. Welt** | Die Spielwelt ist ein Wimmelbild voller wiedererkennbarer Alltagsgags. | Der Aufzug ist außer Betrieb, das Baustellenschild hat fünf durchgestrichene Fertigstellungsjahre, der SEV-Bus steht im Stau. |
| **3. Systeme** | Spielmechaniken folgen der Bahnlogik. | Bauzeit als Verspätung „+0:08“, Werbefrei-Kauf als „Ruhebereich“, Hinweis als knisternde Durchsage, Zurücksetzen verschiebt die Fertigstellung. |
| **4. Stimmen** | Ein kleines festes Ensemble spricht in Mails und Meldungen. | Abteilungsleitung, der Betriebshof (eine einzige Person), die Pressestelle, die alles schönredet. |
| **5. Momente** | Große, inszenierte Pointen an Schlüsselstellen. | Der erste pünktliche Zug, bei dem die Pendler applaudieren; das Saisonfinale; die erste Reset-Rückfrage. |

### Der Pendler-Test

Jeder Witz, jedes Bild und jede Meldung wird an einer einzigen Frage gemessen: **Erkennt ein regelmäßiger Bahnfahrer innerhalb von drei Sekunden, worum es geht?** Wenn man erst überlegen muss, welcher Begriff gemeint ist, fliegt der Text raus. Wenn man sofort an seinen eigenen Morgen auf dem Bahnsteig denkt, bleibt er.

Dazu kommen drei harte Regeln. **Erstens zielt der Witz auf das System, nie auf Mitarbeitende oder Fahrgäste;** der Zugbegleiter ist selbst Leidtragender. **Zweitens gibt es keine Witze über echte Unfälle oder Notlagen.** **Drittens verdeckt die Satire nie das Rätsel:** Während des Denkens bleibt das Baufeld frei, die Gags passieren am Rand und in den Pausen.

---

## 2. Warum das Spiel wie ein Dokument aussieht

Ein Teil der Ursache liegt in unseren eigenen Konzeptdateien. Dort steht „Die Oberfläche wirkt dienstlich“, „helle, klare Informationsoberfläche“, und jeder Bildschirm ist als Tabelle aus Überschrift, Statuszeile, Text und Knopf beschrieben. Die Code-Agenten haben das wörtlich umgesetzt: Überschriften, Unterüberschriften, Textblöcke und Knopflisten auf weißem Grund, vom Hauptmenü bis zum Betriebshof.

Wir ändern deshalb das Leitbild, nicht nur die Farben:

> **Bisher:** Eine dienstliche Informationsoberfläche, auf der ein Puzzle liegt.
> **Neu:** Eine knubbelige, lebendige Comic-Bahnwelt im Ausnahmezustand, in der man selbst Gleise baut. Das Dienstliche steckt in den Gegenständen dieser Welt, also in Anzeigetafel, Durchsage, Dienst-Mail, Stellwerk und Werkhalle, und genau dort ist es komisch.

---

## 3. Der Stil: knubbelige Comic-Miniaturwelt in echtem 3D

**Entschieden (Alex):** Echte 3D-Spielwelt mit klarem eigenem Stil. Leicht comichaft im „bubbly look“, wie man ihn von Clash of Clans kennt. **Kein Fotorealismus und kein KI-generierter Fotorealismus.**

### Was „bubbly“ konkret heißt

- **Formen:** Alles ist leicht übertrieben, rund und weich. Züge haben dicke, abgerundete Fronten und große Fenster, Bahnsteige haben gerundete Kanten, Signale haben große Köpfe. Proportionen sind gestaucht und knuffig, aber nie kindisch: keine Gesichter auf Zügen.
- **Oberflächen:** Es gibt glatte, satte Farbflächen mit weichem Verlauf und wenig Textur. Material wird angedeutet und nicht simuliert. Schotter ist eine knubbelige Kieselfläche, Holz hat zwei, drei gemalte Maserungsstriche.
- **Licht:** Warmes, freundliches Hauptlicht von oben links, weiche Schatten, leichte Randaufhellung an den Silhouetten. Dadurch wirkt alles plastisch und greifbar.
- **Bewegung:** Alles federt: Knöpfe quetschen sich beim Drücken und springen zurück, Gleisstücke ploppen mit kleinem Staubwölkchen ins Feld, Züge wippen beim Anfahren. Die Reaktionen sind kurz (0,1–0,3 Sekunden), damit das Denken nicht gebremst wird.
- **Figuren:** Kleine, rundliche Pendler-Figuren ohne Detailgesichter (Punktaugen), in Gruppen auf Bahnsteigen. Sie gucken auf Handys, seufzen, schauen auf die Tafel und jubeln, wenn tatsächlich ein Zug kommt.

### Wie wir KI-Slop strukturell verhindern

Es gibt **keine KI-generierten Bilder oder Szenen im fertigen Spiel.** Die Welt wird aus einem eigenen, maßstabsgleichen 3D-Baukasten zusammengesetzt: Gleissegmente, Bahnsteigmodule, Gebäudeblöcke, Fahrzeugteile, Requisiten und Figuren. Alle Teile haben ein gemeinsames Raster, dieselbe Lichtsetzung und dieselbe Farbpalette. Überlappungen und falsche Maßstäbe, wie beim jetzigen Betriebshof, können so gar nicht mehr entstehen. KI darf höchstens für interne Ideenskizzen genutzt werden, die nie ins Spiel gelangen.

### Technischer Rahmen

Rätsellogik, Solver, Speicherstände, Fortschritt, Belohnungen, Werbung und Käufe bleiben erhalten. Nur die Darstellung darüber wird ausgetauscht.

- **Im Rätsel** schaut die Kamera fast senkrecht von oben, um etwa 10 bis 15 Grad geneigt, auf das Baufeld. Das Raster bleibt gut lesbar, trotzdem sind Gleishöhe, Schwellen und Schatten sichtbar.
- **Nach dem Lösen** kippt die Kamera auf etwa 40 Grad in die Szene, und der Zug fährt.
- **Gleise** werden exakt entlang der sechs Formen erzeugt und nicht gezeichnet. Sie bestehen aus knubbeligen Stahlschienen auf braunen Holzschwellen, sind immer sauber und nie verzerrt.
- **Leistung:** Ziel sind flüssige 60 Bilder pro Sekunde auf Mittelklasse-Handys, mit einfacher Geometrie, vorberechnetem Licht und wenigen Materialien. Bei 10×10 werden Gleisdetails reduziert, damit alles lesbar bleibt.

### Harte Regeln gegen den Dokument-Look

1. Kein Bildschirm hat eine Überschriften-Hierarchie aus Titel, Untertitel und Fließtext.
2. Außerhalb von Mails steht höchstens eine Textzeile pro Gegenstand.
3. Es gibt keine Listen aus Textknöpfen. Auswahl geschieht über Gegenstände.
4. Es gibt keinen Bildschirm ohne Szene. Weißer Leerraum als Hintergrund ist verboten.
5. Jeder Knopf ist ein Objekt mit Volumen, Schatten und Drückzustand.
6. Jeder Bildschirm hat genau eine Hauptaktion, und sie ist das auffälligste Objekt.

---

## 4. Die Aufmachung: so nah an der Bahn wie erlaubt

Ziel ist, dass das Spiel wirkt, **als hätte derselbe Designer es gebaut.** Die Nähe entsteht über die bekannten Bausteine der Bahnwelt, die nicht geschützt sind, und über eigene Gestaltung, wo es geschützt ist.

| Element | Wie wir es machen |
|---|---|
| **Abfahrtstafel** | Tiefes Anzeigeblau mit weißer Schrift, Zeilen wie Abfahrten: Linie, Ziel, Zeit und Verspätung in Orange-Rot. Ein Fallblatt-Effekt beim Umschalten. |
| **Störungsbanner** | Weißes Feld mit orangem Balken für Ereignisse: Störung, Rückfrage, neue Meldung. |
| **Verkehrsrot** | Markenfarbe für Wortmarke, Hauptaktion und die eigenen Züge. |
| **Züge** | Rot-weiße Nahverkehrsanmutung, in der knubbeligen Comic-Fassung. Eigene Fronten, keine Kopie realer Baureihen. |
| **Klang** | Ein eigener Durchsage-Gong (nicht der Original-Dreiklang), Fallblatt-Klappern, Türpiepen, quietschende Bremsen, Bahnhofs-Hintergrundrauschen. |
| **Schrift** | Eine eigene, klare Verkehrs-Grotesk (derzeit IBM Plex), keine Original-Hausschrift. |
| **Begriffe** | Alle echten Bahnbegriffe sind frei verwendbar: Schienenersatzverkehr, Signalstörung, Weichenstörung, Wagenreihung, Gleiswechsel, Betriebslage. |

**Gestaltungsleitplanke:** erkennbare, bewusst nahe Bahn-Satire mit eigenen Assets statt Kopien. Kein übernommenes DB-Logo, keine kopierten App-Screens, Originalaufnahmen oder fremden Ansagetexte und keine Behauptung einer offiziellen Kooperation. Eine Anwaltsprüfung ist weder beauftragt noch Voraussetzung für das Rework oder den Store-Start. Die Gestaltung wird damit nicht als rechtlich risikofrei behauptet.

---

## 5. Die Welt als Wimmelbild voller Gags

Die Diorama-Szenen in Hauptmenü, Rätselrand, Abschlussfahrt und Betriebshof sind voll mit kleinen, wiedererkennbaren Gags. Viele davon sind leicht animiert und laufen in Schleife, manche passieren selten und überraschend. Das sorgt dafür, dass man die Welt gern anschaut und immer wieder etwas Neues entdeckt.

### Beispiele für dauerhafte Welt-Gags

- **Das Baustellenschild der zweiten Stammstrecke** mit durchgestrichenen Fertigstellungsjahren: „2026“, „2028“, „2035“, „2037“ … und daneben ein frisch geklebter Zettel mit Fragezeichen. Jedes Zurücksetzen eines Rätsels verschiebt die Jahreszahl um zwei Jahre (siehe Abschnitt 6).
- **Der Aufzug zu Gleis 2**: „Außer Betrieb“ und eine Plakette „Instandsetzung geplant“, die ganz langsam verstaubt.
- **Die Rolltreppe** fährt nur nach unten, auch im Bahnhof, der nach oben führt.
- **Die Bahnsteiguhr** geht mit Absicht drei Minuten nach. Niemand bemerkt es.
- **Eine Pendlergruppe**, die gemeinsam von einem Ende des Bahnsteigs zum anderen läuft, weil die Wagenreihung geändert wurde, und dann wieder zurück.
- **Der Schienenersatzverkehr-Bus** steht auf einem Parkplatz. Ein Schild sagt „Ersatzhaltestelle“ mit einem Pfeil ins Nichts.
- **Ein Kaffeebecher-Stapel** auf der Bahnsteigbank wächst mit der Zahl gespielter Rätsel.
- **Tauben** sitzen auf der Oberleitung. Wenn tatsächlich ein Zug kommt, fliegen sie erschrocken auf.

### Seltene Überraschungsgags

Etwa bei jedem zehnten Besuch passiert in der Szene etwas Besonderes, und zwar nur auf Menübildschirmen, nie im Rätsel:

- Ein Zug fährt ein, die Pendler machen sich bereit, der Zug fährt ohne Halt durch. Auf der Tafel steht: „Fährt heute ohne Halt“.
- Die Tafel springt auf „+5“, „+10“, „+20“ und schließlich auf „Zug fällt aus“. Die Pendler seufzen synchron.
- Ein Zugbegleiter rennt mit Kaffee dem abfahrenden Zug hinterher.
- Eine Durchsage beginnt, besteht aber nur aus Knistern. Danach heißt es: „Wir bitten um Verständnis.“

### Welten mit eigenem Gag-Charakter

Die fünf bestehenden Netzabschnitte bekommen jeweils einen sichtbaren Grundwitz:

| Netzabschnitt | Grundwitz der Welt | Typische Bildgags |
|---|---|---|
| **1. Stammstrecke** | Der Kern der Stadt, alles läuft angeblich normal. | Überfüllter Tunnelbahnhof, Baustellenschild, Aufzug außer Betrieb. |
| **2. Bauabschnitt Mitte** | Eine kleine Maßnahme ist dauerhaft geworden. | Riesige Baugrube, Bauzäune, die von Besuch zu Besuch wandern, ein Bagger, an dem nie jemand sitzt, provisorische Holzstege. |
| **3. Außenast ohne Halt** | Das Netz funktioniert, hält aber nicht, wo man hin will. | Einsamer Bahnsteig auf dem Feld, eine Bank mit Spinnweben, ein Schild „Bedarfshalt“, abgestellte Zugteile. |
| **4. Umleitung über Irgendwo** | Alle Wege führen irgendwohin, nur nicht direkt. | Wegweiser, die in alle Richtungen zeigen, Gleise, die Schleifen fahren, eine Brücke mit „vorläufig tragfähig“. |
| **5. Schienenersatzverkehr** | Das Finale: Alles ist erreichbar, nur anders. | Busse in Kolonne, gestrichelte Linien statt Gleise, Pendler mit Rollkoffern auf Feldwegen, am Ende echte Gleise, die zurückkommen. |

---

## 6. Satire in den Spielsystemen

Die Spielsysteme selbst folgen der Bahnlogik. Das ist der stärkste Hebel, denn man erlebt den Witz, statt ihn nur zu lesen. Wichtig: **Keiner dieser Witze verändert eine Spielregel, verhängt eine Strafe oder erzwingt eine Wartezeit.**

| System | Bahnlogik im Spiel |
|---|---|
| **Uhr** | Heißt „Bauzeit“. Die Sternziele stehen auf der Tafel wie Abfahrtszeiten. Liegt man über dem Ziel, erscheint daneben die Verspätung in Orange: „+0:08“. Genau wie am Bahnsteig. |
| **Sterne** | ★ „Angekommen“, ★★ „Anschluss erwischt“, ★★★ „Tatsächlich pünktlich“. Drei Sterne bleiben auch mit Notizen erreichbar. |
| **Zurücksetzen** | Orange Rückfrage: „Strecke wirklich neu planen? Die Fertigstellung verschiebt sich dadurch um zwei Jahre.“ Danach zählt das Baustellenschild in der Welt tatsächlich zwei Jahre hoch, als dauerhafter Zähler über das ganze Spiel. **Die Bauzeit läuft dabei weiter (entschieden).** |
| **Hinweis** | Kommt als Lautsprecher-Durchsage mit Gong: „Aufgrund von … *krrrz* … gehört in dieses Feld eine Kurve. Wir bitten um Verständnis.“ Der Hinweis selbst ist immer eindeutig und vollständig. |
| **Währung** | „Geduldspunkte“, angezeigt in einem Messing-Zählwerk. |
| **Werbefrei-Kauf** | Heißt **„Ruhebereich buchen“** (3,99 €): „Garantiert ohne Durchsagen. Ausnahmsweise.“ |
| **Freiwilliges Video** | „Zusätzliche Geduld beantragen“, gerahmt als kurze Betriebsunterbrechung. |
| **Tagesbonus** | Die Fallblatt-Tafel „Betriebslage des Tages“ klappert durch Meldungen und bleibt auf einer Belohnung stehen. |
| **Laden** | „Verbindung wird gesucht …“, darunter wechselnde Sätze. |
| **Profil** | Ein Dienstausweis mit Dienstgrad, der mit Sternen steigt: vom Praktikanten zum Oberbaurat für Ausnahmezustände. |
| **Erfolge** | „Dienstjubiläen“, etwa „100 Strecken gebaut. Im gleichen Zeitraum wurde ein Planfeststellungsverfahren eröffnet.“ |
| **Wagenreihung** | Selten fährt der eigene Zug nach dem Lösen in umgekehrter Wagenreihung. Die Tafel meldet: „Heute in umgekehrter Wagenreihung.“ Das ist rein optisch. |
| **Teilen** | Nach dem Lösen lässt sich ein **„Pünktlichkeitsnachweis“** als Bild teilen, gestaltet wie ein Amtsformular mit Stempel: „Hiermit wird bestätigt, dass die Verbindung von A nach B pünktlich war. Der Vorfall wird untersucht.“ Das ist gezielt für Social Media und Influencer gedacht. |

---

## 7. Die Stimmen: ein kleines festes Ensemble

**Bewusste Änderung gegenüber der Content-Bible:** Dort steht „keine Handlung mit Figuren“. Für die Dienst-Mail und für mehr Tiefe führen wir ein kleines Ensemble ein. Es bleibt bei drei bis vier wiederkehrenden Absendern. Sie haben keine Gesichter, sondern eine Signatur, einen Tonfall und Running Gags. Alle Figuren sind fiktiv und keine echten Personen oder Ämter.

| Absender | Rolle | Ton | Running Gag |
|---|---|---|---|
| **Abteilungsleitung Stammstrecke** | Ihre Chefin, schickt Aufträge. | Überkorrekt, freundlich, leicht überfordert. | Jede Mail endet mit einem „P.S.“, das eine neue Zusatzaufgabe enthält. |
| **Betriebshof** | Ein Kollege namens Gerd, der Einzige, der noch da ist. | Erschöpft, trocken, herzlich. | „Ich bin der Betriebshof. Ganz wörtlich.“ Er schreibt über Lackierungen, Waggons und seinen Rückenschmerz. |
| **Pressestelle** | Kommuniziert alles nach außen. | Strahlend positiv, egal was passiert. | Formuliert jede Katastrophe als Erfolg. „Die Pünktlichkeit hat sich gegenüber einem besonders ungünstigen Vergleichszeitraum deutlich verbessert.“ |
| **Rundschreiben „Konzernleitung“** | Selten, ganz oben. | Visionär, völlig realitätsfern. | Kündigt Großprojekte an, die nie fertig werden. „Pünktlichkeitsziel 2041 bestätigt.“ |

### Wo das Ensemble auftritt

- **Dienstpostfach:** Ein Briefumschlag-Symbol im Hauptmenü. Mails kommen zu festen Anlässen: Einstieg, neue Regeln im Tutorial, erster Besuch im Betriebshof, jede neue Welt, Saisonende, sowie gelegentliche absurde Rundschreiben. Jede Mail hat höchstens vier kurze Sätze.
- **Tafelmeldungen** in der Kopftafel jedes Rätsels.
- **Ergebnismeldung** nach jedem Rätsel.

### Die Einstiegsmail

> **Von:** Abteilungsleitung Stammstrecke
> **Betreff:** Willkommen im Team!
>
> Hallo und herzlich willkommen, neuer Ingenieur! Wir freuen uns sehr, dass Sie da sind – Sie sind die erste Neueinstellung seit 2017. Ihr erster Auftrag: Zwischen A und B fehlt eine Verbindung. Die Zahlen am Rand zeigen Ihnen, wie viele Gleisfelder in jeder Reihe liegen. Das sollte also schnell gehen.
>
> P.S.: Der Betriebshof hat aktuell Personalmangel. Könnten Sie den nebenbei auch übernehmen?

---

## 8. Texte: Situation statt Amtsbegriff-Salat

Alle Rätseltexte werden neu redigiert. Gelungene Texte bleiben, aneinandergereihte Begriffe fliegen raus. Jeder Text folgt einem einfachen Bauplan: **Situation, die man kennt → ruhige Betriebssprache → die Wahrheit.** Pro Satz gibt es höchstens einen Fachbegriff, und die Pointe muss beim ersten Lesen sitzen. Die Ansprache ist **durchgehend „Sie“ (entschieden)**, im leicht überkorrekten Ton der Betriebssprache.

### Tonproben, wie es künftig klingt

**Tafelmeldungen im Rätsel:**
- „Weichenstörung. Ursache: Weiche.“
- „Ihr Anschluss wartet. Behauptet er.“
- „Der Aufzug zu Gleis 2 ist außer Betrieb. Gleis 2 ebenfalls.“
- „Wegen Verspätung aus vorheriger Fahrt verzögert sich auch diese Fahrt. Die vorherige Fahrt war 2019.“
- „Heute in umgekehrter Wagenreihung. Die erste Klasse hält da, wo Sie nicht stehen.“
- „Schienenersatzverkehr. Der Bus ersetzt den Zug. Wer den Bus ersetzt, wird noch geklärt.“
- „Signalstörung. Das Signal zeigt Grün. Niemand traut sich.“

**Ergebnisse:**
- ★ „Angekommen. Mehr war nie versprochen.“
- ★★ „Anschluss erwischt. Bitte erzählen Sie es niemandem, sonst will das jeder.“
- ★★★ „Tatsächlich pünktlich. Der Vorfall wird untersucht.“

**Ladesätze:**
- „Verbindung wird gesucht … Die Verbindung wurde zuletzt 2014 gesehen.“
- „Bitte haben Sie Geduld. Sie haben ja Geduldspunkte.“

### Wie viele Texte es braucht

Es gibt drei Texte pro Rätsel (Titel, Tafelmeldung, Ergebnis), also 720 Texte für 240 Rätsel. Dazu kommen etwa 40 Mails, Weltintros, Ladesätze, Erfolge und Systemtexte. Ich schreibe sie selbst und redigiere sie nach dem Pendler-Test. Das ist eine eigene Lieferung (siehe Abschnitt 12).

---

## 9. Die Bildschirme

### 9.1 Erster Start

Ein Diensttablet liegt auf einem Schreibtisch im Baubüro. Durch das Fenster sieht man die Baustelle der Stammstrecke, davor das Baustellenschild mit den Jahreszahlen. Die Einstiegsmail kommt an. Ein großer Knopf **„Auftrag annehmen“** führt per Kamerafahrt durch das Fenster hinunter auf die erste Baustelle.

### 9.2 Geführter Einstieg

Die ersten fünf Rätsel bringen die Regeln durch Handlung bei. Pro Rätsel kommt ein neues Element dazu: A und B mit den ersten Gleisen, dann die Randzahlen, dann das X („Hier kommt sicher nichts hin“), dann die grauen Schwellen („Notizen kosten keine Sterne“), dann Uhr und Sterne. Jeder Hinweis ist ein Satz auf dem orangen Banner und verschwindet, sobald die Handlung ausgeführt ist. Danach greift das Spiel nicht mehr ein.

Nach Rätsel 3 kommt die Mail vom Betriebshof („Hallo, ich bin Gerd. Ich bin der Betriebshof.“) mit einer geschenkten Lackierung. So versteht man sofort, wozu Geduldspunkte da sind.

### 9.3 Hauptmenü: der Bahnsteig

Man steht leicht schräg über dem lebendigen Bahnsteig der aktuellen Welt, mit Pendlern, Tafel, Baustellenschild, Tauben und Welt-Gags.

- **Oben hängt die große blaue Abfahrtstafel.** Sie zeigt den nächsten Auftrag wie eine Abfahrt: „S1 · Meldung 07 · Weichenstörung Ostkreuz · 6×6“ mit Sternstand.
- **Die Hauptaktion ist ein großer roter Stellwerksknopf: „Strecke freigeben“.**
- **Weitere Ziele sind Orte in der Szene:** das Tor zum Betriebshof, die Tür zum Stellwerk für die Karte und der Briefkasten für das Dienstpostfach. Unten gibt es eine schmale Symbolleiste mit Bahnsteig, Betriebshof und Dienstausweis.
- **Oben rechts** sitzt das Messing-Zählwerk mit den Geduldspunkten.

### 9.4 Karte: das Stellwerkspult

Ein beleuchtetes Gleisbildpult in knubbeliger Comic-Form. Gelöste Abschnitte leuchten, die aktuelle Störung blinkt rot, kommende Abschnitte sind dunkel. Die fünf Welten sind Bereiche des Pults. Tippt man einen Abschnitt an, klappt eine kleine Meldekarte mit Name, Feldgröße, Sternen und Knopf auf. **Mit jedem gelösten Rätsel leuchtet ein Stück mehr, und das Netz wird sichtbar heil.** Ganz am Rand hängt ein Zettel des Stellwerkers: „Bitte nichts anfassen, was funktioniert.“

### 9.5 Das Rätsel: die Baustelle

**Die Kopftafel (oben, blau):**

- **links:** Linie und Meldung, etwa „S1 · 07“
- **Mitte:** die Tafelmeldung in einer Zeile und darunter groß die **Feldgröße „6×6“**
- **rechts:** die Bauzeit und die drei Sternziele als Abfahrtszeiten, bei Überschreitung mit orangem „+0:08“

**Das Baufeld (Mitte):** eine knubbelige Schotterplatte mit erhöhtem Rand und feinen Rasterfugen. A und B sind kleine Tunnelportale oder Bahnsteigenden, die Randzahlen stehen auf kleinen Schildern. Rundherum, außerhalb des Rasters, liegt dezente Baustellenumgebung mit Bauzaun, Baubude und Bagger. Wenn eine Reihe erfüllt ist, wird ihr Schild grau, bei Überschreitung kurz rot.

**Die Gleiskiste (unten):** eine Holzkiste mit den sechs Gleisstücken als kleine 3D-Stücke, dazu die Sprühdose für das rote X und das Bündel grauer Schwellen. Das aktive Werkzeug ist gelb umrandet und leicht angehoben. Seitlich liegen drei Knöpfe: Rückgängig, Durchsage (Hinweis, mit sichtbarer Zahl der Gratis-Hinweise) und Neu planen (Zurücksetzen).

**Notizen bleiben eindeutig:** Echte Gleise sind Stahl auf braunen Holzschwellen. Grau bedeutet nur graue Schwellen ohne Schienen, also „hier kommt ein Gleis hin, die Form ist offen“. Das rote X ist ein gesprühtes Kreuz auf dem Schotter. Gelb bedeutet ausschließlich das aktive Werkzeug. Die Bedienung bleibt wie festgelegt, und das Raster verschiebt sich nie.

### 9.6 Lösungsmoment und Fahrt

1. Die Lösung rastet ein, Notizen verschwinden, und die Gleise blitzen nacheinander von A nach B auf.
2. Die Kamera kippt in die Szene, und an A und B öffnen sich Tunnel oder Bahnsteige mit wartenden Pendlern.
3. Durchsage-Gong: „Ihr Zug fährt ein. Ja, wirklich.“ Der eigene Zug mit Waggons fährt die selbst gebaute Strecke ab. Die Tauben fliegen auf, die Pendler an B **applaudieren ungläubig**. Die Fahrt dauert 3 bis 5 Sekunden und lässt sich per Tap überspringen.
4. Die Tafel klappt im Fallblatt auf „Strecke freigegeben“.

### 9.7 Ergebnis

Ein Pünktlichkeitsnachweis fährt von unten herein:

- groß die Bauzeit, dazu „Neue Bestzeit“, falls zutreffend
- die Sterne als Stempel nacheinander, mit Ergebnistext
- bei verfehlten Sternen der Abstand: „Noch 0:08 bis ★★★“
- die verdienten Geduldspunkte fliegen ins Zählwerk
- Hauptknopf **„Nächste Störung“**, der sofort das nächste Rätsel per Kamerafahrt öffnet
- Zweitknopf **„Zum Stellwerk“**, der tatsächlich zur Karte führt
- ein kleiner Knopf „Nachweis teilen“
- ganz unten, klar getrennt, „Zusätzliche Geduld beantragen“ (freiwilliges Video)

### 9.8 Saisonende

Nach dem letzten Rätsel gibt es keine Wiederholung, sondern das große Finale:

- Der Zug fährt über das ganze heile Netz. Alle Pendler aller Welten stehen an den Bahnsteigen und jubeln, Konfetti, und die Tafel zeigt überall „pünktlich“.
- Mail der Pressestelle: „Betrieb bis auf Weiteres stabil. Bitte bewahren Sie diese Information gut auf.“
- Direkt danach eine Mail der Abteilungsleitung: „Herzlichen Glückwunsch! P.S.: Ab morgen beginnen die Bauarbeiten für Season 2.“
- „Nächste Störung“ ist ausgegraut: „Alle Störungen behoben. Vorerst.“
- Zwei neue Wege öffnen sich: **„Dauerbaustelle“** (freier Modus) und **„Betriebsrevision“** (Sterne verbessern).

### 9.9 Freier Modus: Dauerbaustelle

Vier Bauschilder vor einer endlosen Baustelle stehen für die vier Schwierigkeitsprofile. Nach jeder Runde startet **„Weiter“ sofort die nächste Runde** im gleichen Profil. Ein kleiner Knopf erlaubt jederzeit einen Profilwechsel. Das Baustellenschild hier zeigt „Fertigstellung: nie“.

### 9.10 Betriebshof: die Werkhalle

Eine knubbelige Werkhalle mit Drehscheibe. Darauf steht groß der aktuelle Zugverband aus Lok und Waggons, schön ausgeleuchtet. Gerd steht daneben, mit Schraubenschlüssel und Kaffee.

- **Wischen** dreht die Drehscheibe zum nächsten Fahrzeug. Gesperrte Fahrzeuge stehen unter einer Plane, ein Schild nennt den Preis.
- **Zwei Bereiche:** „Zugpark“ mit Loks, Triebzügen, Waggons und Lackierungen sowie „Betriebsverbesserungen“ mit Signalen, Bahnsteigen und Schildern, die dann tatsächlich in der Welt auftauchen.
- **Zusammenstellen:** Waggons werden per Antippen angekuppelt, mit sichtbarer Kupplungsanimation.
- **Jedes Objekt hat einen trockenen Satz von Gerd,** zum Beispiel: „Ersatzgarnitur. Ersetzt seit 2009 eine andere Ersatzgarnitur.“

### 9.11 Profil: der Dienstausweis

Ein Dienstausweis mit Platzhalterfoto, Dienstgrad, Statistik und Dienstjubiläen, dazu der Zähler „Fertigstellung durch Sie verschoben: 14 Jahre“. Die Einstellungen liegen hinter einem kleinen Zahnrad.

---

## 10. Entscheidungen

### Von Alex entschieden (25.11.2026)

1. **Stil:** Echte 3D-Spielwelt mit eigenem, leicht comichaftem „bubbly look“ wie Clash of Clans. Kein Fotorealismus, kein KI-generierter Fotorealismus.
2. **Waggons:** nur mit Spielwährung (Geduldspunkte).
3. **Münchner U-Bahn:** im Betriebshof mit Geduldspunkten kaufbar.
4. **Zurücksetzen:** Die Bauzeit läuft weiter.
5. **Ansprache:** durchgehend „Sie“.

### Klarstellung von Alex: kein Logo-Nebenschauplatz, keine Anwaltsvoraussetzung

Die bereits beschlossene Richtung bleibt: so nah und sofort wiedererkennbar wie möglich, ohne zu kopieren. Das Logo ist kein eigener Entscheidungsblock dieses Reworks. Die von Manus nachträglich eingeführte A/B-Auswahl, die vorläufige Festlegung auf einen Gleisknoten und die verpflichtende Anwaltsprüfung sind zurückgenommen. Alex muss dazu keine neue Entscheidung treffen oder Prüfung organisieren.

Der Fokus liegt auf dem Spiel: Satire, 3D-Stil, Bedienung, Rätselqualität und funktionierende Abläufe.

---

## 11. Rätselqualität, Fahrzeuge und Sammlung

### Rätsel: mehr Denken, weniger Abhaken

**Mehr Formenvielfalt.** Lösungspfade sollen keine wiederkehrenden Grundmuster mehr haben, wie etwa die regelmäßige Serpentine mit fünf parallelen Armen. Eine Mustererkennung vergleicht neue Rätsel über die Form des Lösungswegs mit allen bisherigen und verwirft zu ähnliche. Gezielt hinzu kommen neue Pfadarten: Haken, Rückläufe Richtung A, Spiralen, enge Zickzacks, asymmetrische Schleifen.

**Weniger Autopilot.** Null-Reihen und volle Reihen bleiben als Lernanker im Einstieg erlaubt. Ab dem mittleren Teil wird gemessen, wie viel vom Raster allein durch solche Reihen entschieden ist, und Rätsel mit zu hohem Anteil werden verworfen. „Unlogischer“ heißt dabei überraschender, nie mehrdeutig: Jedes Rätsel bleibt eindeutig und ohne Raten lösbar.

**Echte Sternzeiten.** Die Sternzeiten werden zuerst aus Feldgröße und gemessener Denktiefe geschätzt, damit die Sterne sofort funktionieren. Später werden sie mit echten Spielzeiten nachjustiert.

### Fahrzeuge

Alle Fahrzeuge kommen aus dem knubbeligen 3D-Baukasten. Sie unterscheiden sich durch Form und nicht nur durch Farbe:

- S-Bahn (rot-weiß, der Klassiker)
- Regionalzug (Doppelstock-Anmutung)
- Fernzug (weiß mit rotem Band, lange Schnauze)
- Nachtzug (dunkelblau)
- Güterlok (orange)
- Oldtimer-Dampflok
- **Münchner U-Bahn, neue Generation** (hellblau-weiß, runde moderne Front, ohne Betreiberlogo), kaufbar im Betriebshof

Dazu kommen mindestens zehn klar verschiedene Lackierungen statt mehrerer Rottöne, darunter Spaßlackierungen wie „Baustellengelb“, „Graffiti-Edition“ und „Frisch gewaschen (selten)“.

### Waggons

Waggons werden einzeln mit Geduldspunkten gekauft und an die Lok gehängt. Es sind höchstens drei Wagen, damit der Zug auch auf kleinen Feldern sauber durch die Kurven passt.

Beispiele: Doppelstockwagen, Fahrradwagen, Speisewagen („Speisewagen. Heute ohne Speisen.“), Güterwagen, Bauzugwagen und als seltener Sonderwagen der „Wagen mit funktionierender Toilette“.

Der ganze Zugverband ist im Betriebshof und bei jeder Fahrt zu sehen. **Dadurch zeigt sich die Sammlung bei jeder gelösten Strecke.** Bestehende Käufe und Spielstände bleiben gültig, vorhandene Fahrzeuge werden im neuen Stil neu gebaut.

---

## 12. Umsetzung: der vollständige Rundum-Rework

Ich übernehme Produktführung und Umsetzung selbst. Die bestehende technische Grundlage bleibt erhalten, aber Präsentation, Inhalte, Abläufe, Rätselqualität, Fahrzeuge, Ökonomie und Satire werden als **ein zusammenhängendes Spiel** neu gebaut.

Es gibt keinen Vier-Stufen-Plan, keine künstlichen Produktionsphasen und keine isolierte Probestrecke. Ein einzelnes Level kann niemals das Spiel repräsentieren. Der Maßstab ist deshalb der vollständige Zusammenhang: vom ersten Öffnen und der Dienst-Mail über Hauptmenü, Rätsel, Ergebnis, Karte, Welten, Betriebshof, Fahrzeugkauf, Saisonfortschritt, Betriebsrevision und Dauerbaustelle bis zum Saisonende.

Die APK wird erst als Gesamtspiel betrachtet, wenn dieser Zusammenhang vorhanden ist. Vorläufige Einzelassets sind während der Umsetzung zulässig; vorläufige Produktlogik, fehlende Hauptbereiche oder nicht verbundene Abläufe sind es nicht.

Der Rework wird in der tatsächlichen Umsetzung laufend durchgespielt und korrigiert. Dabei prüfe ich insbesondere:

- ob sich das Spiel von der ersten Mail bis zum Saisonende wie **eine** Bahnwelt anfühlt;
- ob die Satire in Bildschirm, Objekt, Ablauf, Klang und Text sichtbar ist, ohne das Rätsel zu stören;
- ob kein Menü wie ein PDF, keine Szene wie KI-Slop und kein Fahrzeug wie ein roter Platzhalter wirkt;
- ob ein Bahnfahrer die Anspielung sofort erkennt und ein Nicht-Bahnfahrer die Bedienung versteht;
- ob Kampagne, Betriebsrevision und Dauerbaustelle unterschiedliche Gründe für die Rückkehr liefern;
- ob alle Weiter-, Zurück-, Ergebnis- und Freimoduspfade zuverlässig dorthin führen, wohin ihr Text es verspricht;
- ob Rätsel, Sternzeiten, Geduldspunkte, Fahrzeuge, Waggons und die Münchner U-Bahn im echten Spielfluss funktionieren.

Vor der Implementierung aktualisiere ich die betroffenen Konzeptdateien im Projekt: 07 Züge, 10 Visuelle Identität, 11 Satire, 12 UI, 14 Content-Bible und 04 Schwierigkeit. Dort steht dieses Leitbild statt „Informationsoberfläche“, damit kein späterer Agent wieder ein Dokument daraus baut.
