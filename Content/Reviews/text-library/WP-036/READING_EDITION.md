# Lesefassung der vollständigen Textbibliothek

Automatisch aus der kanonischen de-DE.json abgeleitet. Alle Texte sind DRAFT. Die als INTERN ausgewiesenen Einstiege sind keine Spielerhilfe und dürfen nicht automatisch angezeigt werden.

## Die 240 Meldungen

### S1-01-01-01 · Abfahrt laut geänderter Tafel

Status: Die Tafel zeigt fünf Minuten mehr. Damit ist der Zug wieder pünktlich.

Ergebnis: Die Tafel meldet Abfahrt. Diesmal bewegt sich auch etwas außerhalb der Anzeige.

**INTERN · Einstieg:** Nullbasiert: Zeile 0 fordert vier Gleisfelder bei vier Spalten und ist daher vollständig belegt. B an N/0 legt zusätzlich den Nordanschluss der Zelle (0,0) fest.

### S1-01-01-02 · Kurzzug zur Hauptverkehrszeit

Status: Heute halten nur drei Wagen. Die übrigen stehen im Fahrplan.

Ergebnis: Die Durchsage meldet einen vollständigen Zug. Alle drei Wagen fahren tatsächlich.

**INTERN · Einstieg:** Nullbasiert: Zeile 2 hat Count 4 und ist vollständig belegt. Eine bloße Gerade durch diese Zeile reicht nicht, da Spalten 2 und 3 jeweils zwei Gleisfelder verlangen.

### S1-01-01-03 · Erste Klasse am anderen Zugende

Status: Die erste Klasse hält hinten. Der Aushang bleibt seiner Richtung treu.

Ergebnis: Die Wagen stehen richtig. Der Aushang wurde umgedreht, nicht der Zug.

**INTERN · Einstieg:** Nullbasiert: Spalte 3 hat Count 0; alle vier Zellen dieser Spalte bleiben leer. Die nutzbare Fläche endet damit vor der rechten Rasterkante.

### S1-01-01-04 · Gleiswechsel nach Türschluss

Status: Ihr Zug steht gegenüber. Die neue Gleisangabe kommt mit dem Türschluss.

Ergebnis: Die Türen öffnen noch einmal. Der Weg über die Treppe zählt diesmal als Umstieg.

**INTERN · Einstieg:** Nullbasiert: Spalten 0 und 1 haben beide Count 0. Die gesamte linke Hälfte des 4×4-Felds ist damit ausgeschlossen.

### S1-01-01-05 · Aufzug mit planmäßiger Pause

Status: Der Aufzug zum Bahnsteig steht. Die Treppe übernimmt ohne Zuschlag.

Ergebnis: Aufzug und Zug fahren. Laut Aushang besteht darauf kein Anspruch.

**INTERN · Einstieg:** Nullbasiert: Zeile 0 hat Count 0 und bleibt leer. Deshalb kann keine Zelle in Zeile 1 nach Norden anschließen.

### S1-01-01-06 · Anschluss mit verkürzter Wartezeit

Status: Ihr Anschluss fährt schon. So entfällt die Wartezeit beim Umsteigen.

Ergebnis: Der Anschluss steht noch da. Die Durchsage muss ihre Entschuldigung abbrechen.

**INTERN · Einstieg:** Nullbasiert: Die unterste Zeile 3 hat Count 4 bei vier Feldern. Ihre vollständige Belegung ist sicher; die Gleisformen sind damit noch nicht bestimmt.

### S1-01-01-07 · Ersatzbus am gesperrten Ausgang

Status: Der Bus hält hinter dem Bauzaun. Die Abfahrt gilt als angeboten.

Ergebnis: Die Strecke ist frei. Der Ersatzbus darf wieder eine erreichbare Haltestelle bedienen.

**INTERN · Einstieg:** Nullbasiert: Zeile 1 ist mit Count 4 vollständig belegt. Sie liefert einen waagerechten Belegungsanker zwischen beiden Rasterseiten.

### S1-01-01-08 · Endstation durch neuen Linienplan

Status: Der Zug endet hier. Ihr Ziel wurde auf dem Linienplan näher herangerückt.

Ergebnis: Die Ansage kündigt Ihr Ziel an. Diesmal fährt der Zug auch hin.

**INTERN · Einstieg:** Nullbasiert: Spalte 0 ist bei Höhe 4 mit Count 4 voll. Die Zeilen 1, 2 und 3 haben je Count 1, weshalb ihre übrigen Zellen leer bleiben.

### S1-01-01-09 · Tunnelbaustelle mit neuem Enddatum

Status: Im Tunnel wird weiter gebaut. Der Aushang nennt jetzt den 32. Mai.

Ergebnis: Die Pressestelle meldet einen offenen Tunnel. Der 32. Mai wird als Bautag gestrichen.

**INTERN · Einstieg:** Nullbasiert: A an W/0 erzwingt die Belegung von (0,0). Da Zeile 0 Count 1 hat, bleiben (1,0), (2,0) und (3,0) leer.

### S1-01-01-10 · Freie Fahrt für die Durchsage

Status: Die Ansage meldet freie Fahrt. Sie gilt nur für den Lautsprecher.

Ergebnis: Laut Durchsage fahren wir jetzt. Die Räder übernehmen ab hier den Transport.

**INTERN · Einstieg:** Nullbasiert: Spalte 0 bleibt wegen Count 0 leer. Spalte 3 ist mit Count 4 vollständig belegt; die Ports N/1 und S/1 ersetzen diese Pflichtbelegung nicht.

### S1-01-01-11 · Abfahrt rückwirkend genehmigt

Status: Am Bahnsteig wartet Ihr Zug. Seine Abfahrt ist für gestern freigegeben.

Ergebnis: Die Türen schließen heute. Die Tafel führt das als einen Tag Verspätung.

**INTERN · Einstieg:** Nullbasiert: Zeile 3 hat Count 0 und bleibt leer; Zeile 0 ist bei Count 4 vollständig belegt. A an N/3 legt dort den Nordport fest.

### S1-01-01-12 · Warnung vor fahrplanmäßiger Abfahrt

Status: Alle Züge fahren pünktlich. Der Aushang warnt vor diesem ungetesteten Zustand.

Ergebnis: Ihr Zug fährt zur angegebenen Zeit. Die Pressestelle spricht von einem Einzelfall.

**INTERN · Einstieg:** Nullbasiert: Spalte 3 hat Count 0 und ist leer; Spalte 1 hat Count 4 und ist voll. Ihr oberes Feld (1,0) trägt A an N/1.

### S1-01-02-01 · Rotes Signal am Tunnel

Status: Der Zug wartet vor dem Tunnel. Feierabend gilt nur für die Büros.

Ergebnis: Sie können „Bin gleich da“ jetzt senden. Hinter dem grünen Signal liegt wieder Feierabend.

**INTERN · Einstieg:** Nullbasiert: Zeile 2 hat bei Breite 4 Count 4. Alle vier Zellen sind belegt; die größere Höhe ändert diesen sicheren Schluss nicht.

### S1-01-02-02 · Zwei Minuten bis zur Einfahrt

Status: Die Tafel meldet zwei Minuten. Die zweite dauert seit halb sechs.

Ergebnis: Die Tafel zeigt null Minuten. Der Zug kommt dazu. Beide Angaben stimmen.

**INTERN · Einstieg:** Nullbasiert: Spalte 1 hat Count 5 bei fünf Zeilen. Alle ihre Zellen sind belegt, die jeweiligen Formen bleiben offen.

### S1-01-02-03 · Halt bei freiem Nachbargleis

Status: Der Zug hält vor Rot. Auf dem Nachbargleis fährt Ihr Anschluss vorbei.

Ergebnis: Ihr Zug fährt weiter. Am nächsten Bahnsteig steht sogar noch der Anschluss.

**INTERN · Einstieg:** Nullbasiert: Spalten 2 und 3 haben beide Count 5 und sind vollständig belegt. A an N/2 fixiert den Nordport der Zelle (2,0).

### S1-01-02-04 · Durchsage ohne Abfahrtszeit

Status: Die Durchsage bittet um Geduld. Eine Abfahrtszeit wäre zu verbindlich.

Ergebnis: Die Durchsage nennt eine Abfahrt. Der Zug hält sich an das gesprochene Wort.

**INTERN · Einstieg:** Nullbasiert: Zeilen 0 und 1 sind mit Count 4 voll; Zeile 4 hat Count 0 und bleibt leer. Diese Belegungen sind unabhängig von den Formen sicher.

### S1-01-02-05 · Gleiswechsel im Feierabend

Status: Der Zug kommt auf Gleis 2. Das rote Signal zieht mit.

Ergebnis: Die Türen öffnen auf Gleis 2. Sie und der Zug haben denselben Bahnsteig erreicht.

**INTERN · Einstieg:** Nullbasiert: Zeile 4 ist mit Count 4 vollständig belegt. Ihre vier Belegungen lassen sich unabhängig von den noch offenen Kurven festhalten.

### S1-01-02-06 · Tunnelhalt neben der Werbung

Status: Der Zug hält neben einem Reiseplakat. Darauf kommt man weiter.

Ergebnis: Die Werbung zieht am Fenster vorbei. Diesmal liegt es an Ihrem Zug.

**INTERN · Einstieg:** Nullbasiert: Spalte 1 verlangt Count 5 und ist damit über die gesamte Höhe belegt. Ihre Belegung ist ein sicherer Anker für die kreuzenden Zeilen.

### S1-01-02-07 · Feierabend am geschlossenen Kiosk

Status: Der Kiosk schließt vor der Abfahrt. Nur Ihr Zug bleibt im Dienst.

Ergebnis: Der geschlossene Kiosk bleibt zurück. Ihr Abendessen muss nicht mehr bis morgen warten.

**INTERN · Einstieg:** Nullbasiert: Zeilen 0 und 1 haben je Count 4 bei vier Spalten und sind voll. Das bestimmt die Belegung, noch nicht die Gleisformen.

### S1-01-02-08 · Freie Fahrt nur auf der Tafel

Status: Die Tafel meldet Abfahrt. Der Zug nimmt die Meldung zur Kenntnis.

Ergebnis: Die Tafel und der Zug melden Abfahrt. Der Zug tut es mit Rädern.

**INTERN · Einstieg:** Nullbasiert: Zeile 0 hat Count 0 und bleibt leer. So sind alle Fortsetzungen aus Zeile 1 nach Norden ausgeschlossen.

### S1-01-02-09 · Ersatzbus hinter der Ampel

Status: Der Ersatzbus steht an der Ampel. Rot bleibt Teil des Angebots.

Ergebnis: Sie steigen aus dem Bus in den Zug. Der Ersatz ersetzt diesmal tatsächlich etwas.

**INTERN · Einstieg:** Nullbasiert: Zeilen 0, 1 und 2 haben Count 0 und sind leer. Übrig bleiben nur Zeilen 3 und 4 für die geforderten Gleisfelder.

### S1-01-02-10 · Grünes Signal mit Wartezeit

Status: Das Signal ist grün. Die Freigabe wartet noch auf Erlaubnis.

Ergebnis: Grün reicht zur Abfahrt. Ein zweites Grün wurde nicht benötigt.

**INTERN · Einstieg:** Nullbasiert: Zeile 1 hat Count 4 und ist voll. B an E/1 fixiert den Ostport der rechten Zelle (3,1) innerhalb dieser Zeile.

### S1-01-02-11 · Abfahrt auf dem neuen Fahrplan

Status: Am Bahnsteig hängt der neue Fahrplan. Ihr Zug steht jetzt unter gestern.

Ergebnis: Ihr Zugbegleiter begrüßt Sie zur heutigen Fahrt. Gestern bleibt im Fahrplan zurück.

**INTERN · Einstieg:** Nullbasiert: Spalte 3 hat Count 0 und ist vollständig leer. Keine Zelle der benachbarten Spalte 2 kann dorthin nach Osten fortsetzen.

### S1-01-02-12 · Rotes Signal bis zum Ruhestand

Status: Das Signal bleibt rot. Laut Aushang endet die Störung mit dem Berufsleben.

Ergebnis: Die Strecke ist frei. Sie erreichen Ihren Feierabend noch ohne Rentenausweis.

**INTERN · Einstieg:** Nullbasiert: Zeilen 1, 2 und 3 haben jeweils Count 4 und sind vollständig belegt. Die große sichere Fläche enthält weiterhin offene Formentscheidungen.

### S1-01-03-01 · Anschluss fährt bei Ihrer Ankunft

Status: Ihr Zug fährt ein. Der Anschluss fährt aus.

Ergebnis: Sie steigen um. Beide Züge waren kurz am selben Ort.

**INTERN · Einstieg:** Nullbasiert: Spalte 2 hat Count 5 und ist vollständig belegt. A an N/2 bestimmt zusätzlich den Nordport von (2,0).

### S1-01-03-02 · Gleiswechsel kurz vor dem Umstieg

Status: Der Anschluss hält gegenüber. Gegenüber liegt jetzt hinter zwei Treppen.

Ergebnis: Die Durchsage nennt das richtige Gleis. Sie erreichen es vor dem Zug.

**INTERN · Einstieg:** Nullbasiert: Zeilen 0 und 1 sind mit Count 4 voll. Zeilen 3 und 4 haben Count 0 und begrenzen die nutzbare Fläche nach unten.

### S1-01-03-03 · Rolltreppe zum Anschluss steht

Status: Die Rolltreppe steht. Ihre Umstiegszeit läuft weiter.

Ergebnis: Sie erreichen den Bahnsteig. Die offenen Türen sind tatsächlich für Sie.

**INTERN · Einstieg:** Nullbasiert: Spalten 2 und 3 haben Count 0 und bleiben leer. N/0 und W/3 liegen an verschiedenen Kanten des verbleibenden linken Bereichs.

### S1-01-03-04 · Anschlusstür schließt vor Ihnen

Status: Die Tür schließt vor Ihnen. Ihre Ankunft gilt nicht als Einstieg.

Ergebnis: Die Tür öffnet sich. Diesmal ist das keine Verabschiedung.

**INTERN · Einstieg:** Nullbasiert: A an E/1 belegt (3,1). Zeile 1 fordert nur ein Gleisfeld, daher bleiben ihre anderen drei Zellen leer.

### S1-01-03-05 · Anschluss laut Tafel erreicht

Status: Die Tafel meldet Ihren Umstieg als erfolgt. Sie stehen noch davor.

Ergebnis: Sie sitzen im Anschlusszug. Die Anzeige muss nichts mehr erfinden.

**INTERN · Einstieg:** Nullbasiert: Zeile 1 ist mit Count 4 vollständig belegt. Ihr linkes Feld (0,1) trägt den Westanschluss A.

### S1-01-03-06 · Kaffee bis zum nächsten Anschluss

Status: Ihr Anschluss ist weg. Am Kiosk kostet die Wartezeit jetzt pro Becher.

Ergebnis: Bitte halten Sie den Kaffee fest, wir fahren. Weitere Wartezeit ist nicht im Becherpreis enthalten.

**INTERN · Einstieg:** Nullbasiert: Zeile 0 hat Count 4 und ist vollständig belegt. Dieser Queranker legt noch nicht fest, wie die Kurven verbunden werden.

### S1-01-03-07 · Anschluss hinter der Baustellenwand

Status: Der Pfeil zum Anschluss zeigt durch die Wand. Dieser Weg spart Zeit.

Ergebnis: Der Bauzaun hat eine Lücke. Ihr Anschluss wartet auf der richtigen Seite.

**INTERN · Einstieg:** Nullbasiert: A an S/2 erzwingt (2,4). Weil Zeile 4 Count 1 hat, bleiben alle übrigen Zellen dieser Zeile leer.

### S1-01-03-08 · Anschlussdurchsage nach der Abfahrt

Status: Die Durchsage nennt Ihren Anschluss. Sie erreicht als Einzige den Zug.

Ergebnis: Die Ansage kommt vor der Abfahrt. Sie dürfen ihr diesmal folgen.

**INTERN · Einstieg:** Nullbasiert: Spalte 1 hat Count 5 und ist voll; Spalte 0 hat Count 0 und ist leer. Untere Einserzeilen können deshalb nur in Spalte 1 belegt sein.

### S1-01-03-09 · Wartebank für den nächsten Anschluss

Status: Der nächste Anschluss kommt morgen. Die Bank zählt als Übernachtung.

Ergebnis: Der Anschluss fährt noch heute. Die Bank muss kein Bett mehr darstellen.

**INTERN · Einstieg:** Nullbasiert: Zeilen 3 und 4 haben jeweils Count 4 und sind vollständig belegt. Der dichte untere Bereich ist damit sicher, seine Geometrie bleibt offen.

### S1-01-03-10 · Aushang empfiehlt den gestrigen Anschluss

Status: Der Aushang empfiehlt den Zug von gestern. Heute ist er pünktlich.

Ergebnis: Sie steigen in den Zug von heute. Der Aushang wird als Zukunftsprojekt gefeiert.

**INTERN · Einstieg:** Nullbasiert: B an S/2 belegt (2,4), das einzige geforderte Gleisfeld der untersten Zeile. Ihre anderen Felder bleiben leer.

### S1-01-03-11 · Anschluss wartet mit geschlossenen Türen

Status: Der Anschluss wartet jetzt auf Sie. Dafür bleiben seine Türen geschlossen.

Ergebnis: Ihr Rollkoffer fährt mit. Die offenen Türen waren diesmal auch zum Einsteigen da.

**INTERN · Einstieg:** Nullbasiert: Zeile 1 hat Count 4 und ist voll. Vier sichere Belegungen bilden den Einstieg; gleiche Spaltenwerte bedeuten noch keine gleichen Gleisformen.

### S1-01-03-12 · Pünktliche Abfahrt vor Ihrer Ankunft

Status: Der Anschluss fährt vor Ihrer Ankunft. So stört kein Umstieg die Pünktlichkeit.

Ergebnis: Sie sitzen im Anschluss. Die Statistik bittet Sie, das nicht weiterzuerzählen.

**INTERN · Einstieg:** Nullbasiert: B an N/1 belegt (1,0). A an S/3 belegt (3,4) und verbraucht damit den gesamten Count 1 von Spalte 3; deren übrige Felder sind leer.

### S1-01-04-01 · Abfahrt bleibt bei fünf Minuten

Status: Der Zug kommt fünf Minuten später. Die fünf Minuten bleiben stehen.

Ergebnis: Eine Taube sieht der Abfahrt nach. Sie hatte sich auf weitere fünf Minuten eingerichtet.

**INTERN · Einstieg:** Nullbasiert: Zeilen 0 und 1 haben Count 0 und bleiben leer. Die Strecke muss innerhalb der drei unteren Zeilen liegen.

### S1-01-04-02 · Neuer Fahrplan am Bahnsteig

Status: Am Bahnsteig hängt ein neuer Plan. Er ist älter als die Durchsage.

Ergebnis: Zug und Aushang nennen dieselbe Abfahrt. Das Papier darf diesmal hängen bleiben.

**INTERN · Einstieg:** Nullbasiert: Spalte 0 sowie Zeilen 3 und 4 haben jeweils Count 0. A an N/3 liegt im verbleibenden oberen rechten Gebiet.

### S1-01-04-03 · Abfahrt von der Tafel entfernt

Status: Ihr Zug steht nicht mehr auf der Tafel. Sie stehen weiter am Gleis.

Ergebnis: Die Türen öffnen sich. Diesmal ist der Zug nicht nur eine Zeile auf der Tafel.

**INTERN · Einstieg:** Nullbasiert: Zeile 0 ist bei Breite 4 und Count 4 vollständig belegt. A an N/0 fixiert den Nordport von (0,0).

### S1-01-04-04 · Neue Abfahrt auf Gleis 2

Status: Die Abfahrt wurde auf Gleis 2 verlegt. Ihre Wartezeit fährt mit.

Ergebnis: Sie sitzen im Zug. Die Treppe wird für diese Abfahrt nicht mehr benötigt.

**INTERN · Einstieg:** Nullbasiert: Spalte 1 ist mit Count 5 voll; Spalten 2 und 3 bleiben wegen Count 0 leer. Die drei unteren Einserzeilen sind damit auf Spalte 1 begrenzt.

### S1-01-04-05 · Neue Uhrzeit per Durchsage

Status: Die Durchsage nennt die neue Abfahrt. Der Satz beginnt mit „Vorhin“.

Ergebnis: Noch während der Ansage rollen wir los. Die neue Abfahrtszeit wird unterwegs nachgereicht.

**INTERN · Einstieg:** Nullbasiert: Zeilen 0, 1 und 2 haben Count 4 und sind voll. Zeile 3 fordert nur eine Zelle und kann diese Fläche nicht einfach fortsetzen.

### S1-01-04-06 · Fahrt endet vor Ihrem Ziel

Status: Der Zug endet am nächsten Halt. Ihr Ziel wurde aus dem Plan entfernt.

Ergebnis: Die Durchsage nennt Ihr Ziel. Diesmal dürfen Sie bis dorthin sitzen bleiben.

**INTERN · Einstieg:** Nullbasiert: Spalte 3 hat Count 0 und bleibt vollständig leer. Der verfügbare Bereich ist auf die drei linken Spalten begrenzt.

### S1-01-04-07 · Anschluss fährt jetzt früher

Status: Ihr Anschluss fährt vor Ihrer Ankunft. Im Plan gilt das als Umstieg.

Ergebnis: Ihr Anschluss steht noch am Gleis. Die Verbindung besteht heute auch außerhalb des Plans.

**INTERN · Einstieg:** Nullbasiert: Zeile 0 ist leer. A an S/0 belegt (0,4); die beiden vollen Zeilen 2 und 3 liefern zusätzlich sichere Queranker.

### S1-01-04-08 · Ihr Halt wurde gestrichen

Status: Der Zug fährt an Ihrem Bahnsteig vorbei. Im Plan entfällt nur Ihr Einstieg.

Ergebnis: An Ihrem Bahnsteig öffnen sich die Türen. Der Fahrplan gestattet wieder Ihre Anwesenheit.

**INTERN · Einstieg:** Nullbasiert: Zeile 2 hat Count 4 und ist voll. B an N/2 belegt (2,0), zugleich die einzige geforderte Zelle der oberen Zeile.

### S1-01-04-09 · Sonntagsfahrplan gilt ab Montag

Status: Am Bahnsteig gilt jetzt der Sonntagsplan. Der Montag wurde mit gestrichen.

Ergebnis: Sie fahren am Montag nach Montagsplan. Die Pressestelle meldet eine ganze Woche gewonnen.

**INTERN · Einstieg:** Nullbasiert: Zeilen 0 und 1 sind mit Count 4 vollständig belegt. Zeile 4 hat Count 0 und bleibt leer.

### S1-01-04-10 · Ersatzbus fährt einmal pro Stunde

Status: Der Ersatzbus fährt laut Aushang stündlich. Gemeint ist eine Stunde im Jahr.

Ergebnis: Der Bus bringt Sie zum Zug. Der Aushang hat für diesen Fall keine weitere Fußnote.

**INTERN · Einstieg:** Nullbasiert: A an E/0 belegt (3,0). Zeile 0 hat Count 1, weshalb ihre drei anderen Zellen leer bleiben.

### S1-01-04-11 · Abfahrt auf gestern vorverlegt

Status: Die Tafel meldet Ihre Abfahrt von gestern. Der Zug steht zur Bestätigung noch hier.

Ergebnis: Die Räder bewegen sich. Damit ist Ihre Abfahrt nun auch außerhalb der Akten erfolgt.

**INTERN · Einstieg:** Nullbasiert: Spalte 0 hat Count 5 und ist vollständig belegt. A an W/4 fixiert dort den Westanschluss des unteren Felds (0,4).

### S1-01-04-12 · Bahnhofsuhr dem Fahrplan angepasst

Status: Die Bahnhofsuhr bekommt 90 Minuten pro Stunde. Der Zug ist jetzt pünktlich.

Ergebnis: Die Durchsage meldet pünktliche Abfahrt. Bitte stellen Sie Ihre Uhr auf Bahnzeit.

**INTERN · Einstieg:** Nullbasiert: Zeilen 1 und 2 haben beide Count 4 und sind voll. B an W/4 belegt (0,4), zugleich die einzige Zelle der unteren Zeile.

### S1-02-01-01 · Bahnsteig wegen Bauarbeiten verkürzt

Status: Bitte gehen Sie bis zur Markierung. Der Zug hält noch am alten Ende.

Ergebnis: Die Türen öffnen sich am Bahnsteig. Ihr Rollkoffer benötigt keinen Bauhelm mehr.

**INTERN · Einstieg:** Nullbasiert: Spalte 0 hat Count 5 und ist voll. Zeilen 2, 3 und 4 haben je Count 1, weshalb sie außerhalb dieser Spalte leer bleiben.

### S1-02-01-02 · Zugang nur durch den Ausgang

Status: Der Eingang ist gesperrt. Bitte betreten Sie den Bahnhof durch den Ausgang.

Ergebnis: Sie sitzen im Zug. Der Ausgang hat seine Aufgabe als Eingang erfüllt.

**INTERN · Einstieg:** Nullbasiert: Zeile 4 hat Count 5 bei fünf Spalten. Alle fünf unteren Felder sind sicher belegt.

### S1-02-01-03 · Durchsagen wegen Baulärm nicht hörbar

Status: Die Durchsage erklärt den Baulärm. Verständlich bleibt nur der Presslufthammer.

Ergebnis: Die Durchsage nennt Ihr Ziel. Der Zug fährt sogar dorthin.

**INTERN · Einstieg:** Nullbasiert: Spalten 0 und 1 haben Count 0 und bleiben leer. Spalte 3 ist mit Count 5 vollständig belegt.

### S1-02-01-04 · Ersatzbus im Baustellenverkehr

Status: Der Ersatzbus steht im Stau. Der Fahrplan setzt eine freie Straße voraus.

Ergebnis: Im Ersatzbus bleiben die Sitze leer: Die Schienen sind wieder schneller als der Stau.

**INTERN · Einstieg:** Nullbasiert: Zeile 4 hat Count 0 und ist leer; Zeile 1 hat Count 5 und ist voll. Beide Grenzen sind unmittelbar aus den Randzahlen sicher.

### S1-02-01-05 · Bauende nur noch mit Bleistift

Status: Am Bauzaun hängt der neue Termin. Ein Radiergummi hängt daneben.

Ergebnis: Auf der Tafel steht eine Abfahrt statt eines Bautermins. Radieren ist nicht mehr nötig.

**INTERN · Einstieg:** Nullbasiert: Zeile 3 ist mit Count 5 voll; Zeilen 0 und 1 sind bei Count 0 leer. Die sichere Querbelegung liegt somit unter den leeren Reihen.

### S1-02-01-06 · Aufzug hinter dem Bauzaun

Status: Der Aufzug ist in Betrieb. Sie erreichen ihn ausschließlich über die Treppe.

Ergebnis: Sie erreichen den Zug ohne Treppe. Der Aufzug zählt wieder als Aufzug.

**INTERN · Einstieg:** Nullbasiert: Zeile 2 hat Count 5 und ist voll. Sie kreuzt die ebenfalls vollen Spalten 3 und 4; die Formen an den Schnittstellen bleiben offen.

### S1-02-01-07 · Nachtarbeiten bis Montagmittag

Status: Die Arbeiten finden nur nachts statt. Montag bleibt deshalb bis 14 Uhr dunkel.

Ergebnis: Tageslicht fällt ins fahrende Abteil. Damit ist auch die dreitägige Nacht abgeschlossen.

**INTERN · Einstieg:** Nullbasiert: B an N/1 belegt (1,0). Zeile 0 fordert Count 1, ihre übrigen vier Zellen sind deshalb leer.

### S1-02-01-08 · Abfahrt am Nachbarbahnhof

Status: Ihr Zug hält am Ersatzbahnsteig. Die Anfahrt dorthin erfordert einen Zug.

Ergebnis: Ihr Zug hält wieder hier. Für die Anreise zum Zug genügt nun ein Bahnhof.

**INTERN · Einstieg:** Nullbasiert: Spalte 0 ist bei Count 5 voll, Zeile 1 bei Count 5 ebenfalls. Die beiden sicheren Belegungsachsen kreuzen sich in (0,1).

### S1-02-01-09 · Bahnsteig hinter neuer Sichtschutzwand

Status: Die Sichtschutzwand ist fertig. Der Zug dahinter gilt als ausgefallen.

Ergebnis: Die Wand ist weg. Der Zug fährt und wird wieder als vorhanden geführt.

**INTERN · Einstieg:** Nullbasiert: Spalte 1 ist mit Count 5 vollständig belegt. Die sichere senkrechte Achse bleibt trotz A an N/3 und B an W/3 erforderlich.

### S1-02-01-10 · Umleitung zurück zum Start

Status: Bitte folgen Sie den gelben Pfeilen. Ihr Rundweg ersetzt heute die Abfahrt.

Ergebnis: Bitte steigen Sie ein; wir fahren weiter. Der Rundweg ist heute nicht im Preis enthalten.

**INTERN · Einstieg:** Nullbasiert: B an W/0 erzwingt (0,0). Zeile 0 hat Count 1, daher bleiben ihre anderen vier Felder leer.

### S1-02-01-11 · Wiedereröffnung am 31. Februar

Status: Der Termin steht am Bauzaun. Der Kalender muss sich dem Baufortschritt anpassen.

Ergebnis: Die Pressestelle bestätigt die Abfahrt am 1. März. Der 31. Februar gilt als eingehalten.

**INTERN · Einstieg:** Nullbasiert: Zeile 0 ist bei Count 5 voll. A an N/0 fixiert den Nordport von (0,0) innerhalb dieser sicheren Querbelegung.

### S1-02-01-12 · 100 Jahre am provisorischen Bahnsteig

Status: Der Holzbahnsteig wird 100. Die Festrede kündigt den baldigen Baubeginn an.

Ergebnis: Mit der ersten Abfahrt endet auch der Mietvertrag der Taube. Er lief seit 100 Jahren.

**INTERN · Einstieg:** Nullbasiert: B an N/3 belegt (3,0), die einzige Zelle von Zeile 0. Zeile 1 fordert ebenfalls nur eine Belegung; deren Position folgt erst aus weiteren Schlüssen.

### S1-02-02-01 · Warten am roten Signal

Status: Ihr Zug wartet am Signal. Die grüne Welle gilt für die Gegenrichtung.

Ergebnis: Die Pendler stehen zur Abfahrt auf. Diesmal nicht, um sich die Beine zu vertreten.

**INTERN · Einstieg:** Nullbasiert: Zeile 0 hat Count 5 und ist vollständig belegt. Die Formen lassen sich aus dieser Belegung allein noch nicht festlegen.

### S1-02-02-02 · Gleis 2 endet am Bauzaun

Status: Gleis 2 endet am Bauzaun. Nur der Fahrplan führt weiter.

Ergebnis: Die Strecke ist frei. Der Fahrplan hat wieder Kontakt zu den Schienen.

**INTERN · Einstieg:** Nullbasiert: Zeile 1 hat Count 5 und ist voll; Zeile 4 hat Count 0 und ist leer. Beide Aussagen liefern unabhängige sichere Belegungen.

### S1-02-02-03 · Kaffeepause vor dem Gegenzug

Status: Der Gegenzug wird abgewartet. Ihr Kaffee erreicht zuerst Raumtemperatur.

Ergebnis: Ihr Kaffee schwappt beim Anfahren. Diese Form der Unruhe ist ausdrücklich erwünscht.

**INTERN · Einstieg:** Nullbasiert: Zeile 0 hat Count 0; alle fünf oberen Felder bleiben leer. Die Belegung muss in den unteren vier Zeilen liegen.

### S1-02-02-04 · Gleiswechsel über die Treppe

Status: Ihr Zug fährt gegenüber. Der direkte Weg führt über 48 Stufen.

Ergebnis: Sie sitzen im Zug. Die Treppe zählt nicht mehr zur heutigen Strecke.

**INTERN · Einstieg:** Nullbasiert: Spalte 2 ist mit Count 5 voll. Sie belegt bereits die einzige geforderte Zelle von Zeile 2; deren übrige Felder sind leer.

### S1-02-02-05 · Bus statt zweitem Gleis

Status: Der Ersatzbus steht im Stau. Auch auf der Straße reicht eine Spur.

Ergebnis: Der Zugbegleiter meldet freie Fahrt. Den Busanschluss dürfen Sie diesmal verpassen.

**INTERN · Einstieg:** Nullbasiert: Spalten 3 und 4 haben Count 5 und sind voll. A an N/4 fixiert zunächst den Nordport der Zelle (4,0), nicht ohne weitere Schlüsse deren gesamte Form.

### S1-02-02-06 · Kurzzug am vollen Bahnsteig

Status: Heute fahren nur drei Wagen. Der Bahnsteig bleibt in voller Länge besetzt.

Ergebnis: Der Kurzzug fährt. Die Durchsage bedankt sich für das platzsparende Reisen.

**INTERN · Einstieg:** Nullbasiert: Spalte 3 und Zeile 0 sind bei Count 5 voll. Die gekreuzten Belegungsanker legen noch keine vollständigen Gleisformen fest.

### S1-02-02-07 · Anschluss fährt ohne Zubringer

Status: Ihr Anschluss fährt pünktlich ab. Ihr Zug wartet noch vor derselben Weiche.

Ergebnis: Beide Züge stehen am Bahnsteig. Der Anschluss ist diesmal mehr als ein Wort.

**INTERN · Einstieg:** Nullbasiert: Spalte 1 ist mit Count 5 voll. Spalten 3 und 4 haben Count 0 und bleiben leer; die untere Einserzeile ist damit auf Spalte 1 festgelegt.

### S1-02-02-08 · Rückfahrt nur auf der Anzeige

Status: Am Endbahnhof bleibt der Zug stehen. Zurück fährt nur die Anzeige.

Ergebnis: Die Rückfahrt beginnt. Auch die Räder nehmen daran teil.

**INTERN · Einstieg:** Nullbasiert: Zeilen 0 und 1 haben jeweils Count 5. Beide sind vollständig belegt, obwohl ihre konkreten Formen noch bestimmt werden müssen.

### S1-02-02-09 · Wartezeit ohne Bahnhofsuhr

Status: Am Engpass gibt es keine Verspätung mehr. Die Bahnhofsuhr wurde abgedeckt.

Ergebnis: Zur Abfahrt fällt das Tuch von der Uhr. Die Verspätung bleibt darunter.

**INTERN · Einstieg:** Nullbasiert: Zeilen 0 und 4 haben Count 0 und bleiben leer. Die drei mittleren Zeilen enthalten sämtliche Gleisfelder.

### S1-02-02-10 · Zweites Gleis laut Lautsprecher

Status: Die Durchsage meldet zwei freie Gleise. Eines davon liegt nur auf dem Bauplan.

Ergebnis: Laut Durchsage fahren Sie jetzt auf echten Schienen. Der Bauplan bleibt unbefahren.

**INTERN · Einstieg:** Nullbasiert: Zeile 0 hat Count 5 und ist voll. A an W/0 legt am linken Feld (0,0) den Westport fest.

### S1-02-02-11 · Bauende laut Schild morgen

Status: Der Bauzaun nennt morgen als Bauende. Der Termin wird täglich erneuert.

Ergebnis: Der Bauzaun ist weg. Das Schild mit dem Bauende hat seinen ersten Arbeitstag.

**INTERN · Einstieg:** Nullbasiert: A an S/4 belegt (4,4). Zeile 4 hat Count 1 und ist damit vollständig erfüllt; die anderen vier unteren Felder sind leer.

### S1-02-02-12 · Pünktlichkeit durch Gleisabbau

Status: Das zweite Gleis wurde entfernt. So kann dort kein Zug mehr zu spät kommen.

Ergebnis: Die Pressestelle meldet pünktliche Abfahrt. Das entfernte Gleis bleibt Spitzenreiter.

**INTERN · Einstieg:** Nullbasiert: Spalte 4 hat Count 0 und bleibt leer. Der Nordanschluss B an N/3 liegt direkt links davon; östliche Fortsetzungen aus Spalte 3 sind ausgeschlossen.

### S1-02-03-01 · Halt vor dem Baustellensignal

Status: Der Zug wartet vor der Baustelle. Die Freigabe hat noch keinen Termin.

Ergebnis: Am Fenster sehen Sie die Baustelle vorbeiziehen. Sie hat nun Verspätung, nicht Sie.

**INTERN · Einstieg:** Nullbasiert: Zeile 4 hat Count 0 und bleibt leer. Die vier oberen Zeilen enthalten sämtliche Gleisfelder.

### S1-02-03-02 · Türen zu bei stehendem Zug

Status: Die Türen bleiben geschlossen. Damit gilt die Abfahrt als begonnen.

Ergebnis: Wir rollen tatsächlich. Das Schließen der Türen hatte diesmal einen Zweck.

**INTERN · Einstieg:** Nullbasiert: Zeile 4 hat Count 5 und ist vollständig belegt. Die Zeile darüber fordert zwei weitere Zellen, deren Lage noch offen ist.

### S1-02-03-03 · Rückruf aus dem Stellwerk ausstehend

Status: Das Stellwerk bittet um Geduld. Ihr Anschluss bittet nicht und fährt.

Ergebnis: Die Fahrt geht weiter. Am nächsten Bahnsteig wartet tatsächlich noch ein Zug.

**INTERN · Einstieg:** Nullbasiert: Spalte 4 hat Count 5 und ist voll. Sie ist der einzige öffentliche Vollanker; die übrigen Randzahlen müssen miteinander kombiniert werden.

### S1-02-03-04 · Abfahrt in einer dauerhaften Minute

Status: Die Tafel zeigt noch eine Minute. Diese Angabe gilt seit einer Stunde.

Ergebnis: Die Anzeige springt auf null. Der Zug bewegt sich mit. Bitte nicht erschrecken.

**INTERN · Einstieg:** Nullbasiert: Spalte 4 hat Count 0 und ist leer. B an N/3 belegt (3,0) und erfüllt Count 1 von Spalte 3; deren übrige Felder bleiben leer.

### S1-02-03-05 · Neue Durchsage statt Weiterfahrt

Status: Die nächste Durchsage folgt in Kürze. Sie ersetzt die nächste Fahrt.

Ergebnis: Aus dem Lautsprecher kommt nichts mehr. Dafür fährt der Zug. Ein fairer Tausch.

**INTERN · Einstieg:** Nullbasiert: B an N/1 belegt (1,0). Da Zeile 0 nur Count 1 hat, sind ihre anderen vier Felder leer.

### S1-02-03-06 · Freigabe am falschen Bauzaun

Status: Die Freigabe hängt am Bauzaun. Sie gilt für das abgerissene Gleis.

Ergebnis: Ihr Zugbegleiter bestätigt die Abfahrt. Die Erlaubnis gilt erstmals für vorhandene Schienen.

**INTERN · Einstieg:** Nullbasiert: Zeile 0 hat Count 0 und bleibt auch im nun sechs Spalten breiten Raster leer. Die drei Fünferzeilen darunter sind nicht vollständig belegt.

### S1-02-03-07 · Kaffee kalt vor dem roten Signal

Status: Der Kaffee im Zug ist kalt. Als Ursache gilt die laufende Warmhaltephase.

Ergebnis: Ihr Kaffee ist noch kalt, aber endlich unterwegs. Dies gilt als Ende der Warmhaltephase.

**INTERN · Einstieg:** Nullbasiert: B an N/0 erzwingt (0,0). Spalte 0 hat Count 1, weshalb ihre anderen vier Zellen leer bleiben.

### S1-02-03-08 · Ersatzbus wartet auf das Stellwerk

Status: Der Bus steht vor dem Bahnhof. Er übernimmt auch die Wartezeit des Zuges.

Ergebnis: Die Verbindung steht. Sie fahren wieder auf Schienen. Der Bus hat Feierabend.

**INTERN · Einstieg:** Nullbasiert: B an N/4 belegt (4,0), die einzige Zelle der oberen Zeile. Spalte 5 hat Count 0 und bleibt vollständig leer.

### S1-02-03-09 · Grüner Zettel am roten Signal

Status: Am Signal hängt ein grüner Zettel. Das Rot ist damit schriftlich behoben.

Ergebnis: Auf der Tafel steht freie Fahrt. Die Lampe teilt erstmals die Meinung des Zettels.

**INTERN · Einstieg:** Nullbasiert: Zeilen 0 und 1 sind mit Count 6 vollständig belegt. Spalte 5 ist bei Höhe 5 und Count 5 ebenfalls voll.

### S1-02-03-10 · Antwort für einen ausgemusterten Zug

Status: Die Antwort liegt am Bahnsteig. Der zugehörige Zug steht jetzt im Museum.

Ergebnis: Ein heutiger Zug fährt los. Die Pressestelle spricht von beschleunigter Bearbeitung.

**INTERN · Einstieg:** Nullbasiert: Zeile 4 ist mit Count 6 vollständig belegt. Dieser Bodenanker ist sicher, obwohl Zeile 3 mit Count 5 eine Lücke enthalten muss.

### S1-02-03-11 · Sofortige Abfahrt ab 2041

Status: Die Freigabe gilt ab 2041. Der Aushang nennt das eine Sofortmaßnahme.

Ergebnis: Die Fahrt beginnt noch in diesem Jahr. Am Bahnsteig werden die Kalender geprüft.

**INTERN · Einstieg:** Nullbasiert: Spalten 1 und 2 haben bei Höhe 5 jeweils Count 5. Beide sind vollständig belegt und liefern sichere senkrechte Anker.

### S1-02-03-12 · Freie Fahrt nur bei Stillstand

Status: Der Zug darf weiterfahren. Die Erlaubnis erlischt, sobald er sich bewegt.

Ergebnis: Wir fahren trotzdem. Das Stellwerk bestätigt einen ordnungsgemäßen Stillstand.

**INTERN · Einstieg:** Nullbasiert: Spalte 0 ist mit Count 5 voll. A an W/4 fixiert den Westport im unteren Feld (0,4); die weiteren Formen sind noch offen.

### S1-02-04-01 · Zugang zu Gleis 2 hinter dem Bauzaun

Status: Gleis 2 liegt hinter dem Bauzaun. Der Fahrplan steht noch davor.

Ergebnis: Sie sind am richtigen Bahnsteig. Der Zug hält tatsächlich dort.

**INTERN · Einstieg:** Nullbasiert: Zeile 4 hat bei Breite 6 Count 6 und ist vollständig belegt. Die zwei Fünferzeilen weiter oben benötigen dagegen je eine freie Zelle.

### S1-02-04-02 · Umleitung zum Anschluss ohne Pfeil

Status: Bitte folgen Sie der Umleitung. Der erste Pfeil steht am Ziel.

Ergebnis: Der Anschluss ist erreicht. Das Schild bestätigt Ihre Ankunft im Nachhinein.

**INTERN · Einstieg:** Nullbasiert: Spalte 5 ist mit Count 5 voll. Die Zeilen 2, 3 und 4 haben Count 1 und bleiben deshalb außerhalb dieser Spalte leer.

### S1-02-04-03 · Umstieg mit Rollkoffer über die Treppe

Status: Zum Anschluss nehmen Sie die Treppe. Stufenlos ist nur der Lageplan.

Ergebnis: Koffer und Fahrgast sind im Zug. Der Lageplan meldet einen barrierefreien Umstieg.

**INTERN · Einstieg:** Nullbasiert: Zeilen 0, 1 und 2 haben Count 0 und bleiben leer. Sämtliche Gleisfelder müssen in den beiden unteren Zeilen liegen.

### S1-02-04-04 · Gleiswechsel während der Durchsage

Status: Ihr Zug hält jetzt an Gleis 4. Die Durchsage ist noch auf Gleis 3.

Ergebnis: Die Türen schließen hinter Ihnen. Die Durchsage darf nun in Ruhe nachkommen.

**INTERN · Einstieg:** Nullbasiert: Zeile 3 hat Count 6 und ist vollständig belegt. Sie liefert einen sicheren Queranker über die gesamte Rasterbreite.

### S1-02-04-05 · Anschluss nach verlängertem Fußweg

Status: Der Fußweg dauert zwölf Minuten. Zum Umstieg bleiben Ihnen drei.

Ergebnis: Der Anschluss wurde erreicht. Neun Minuten gelten damit als eingespart.

**INTERN · Einstieg:** Nullbasiert: Zeilen 3 und 4 haben Count 6 und sind beide voll. A an W/4 und B an E/1 liegen über verschiedene Höhen verteilt.

### S1-02-04-06 · Umstieg mit Umweg durch die Bahnhofshalle

Status: Der Weg führt erneut am Kiosk vorbei. Ihr Kaffee gilt noch als derselbe.

Ergebnis: Sie sitzen im Anschlusszug. Der Kaffee hat die Hallenrundfahrt ebenfalls beendet.

**INTERN · Einstieg:** Nullbasiert: Zeile 0 hat Count 6 und ist voll. Spalte 4 ist bei Höhe 5 mit Count 5 voll; beide sicheren Achsen kreuzen sich in (4,0).

### S1-02-04-07 · Neue Gleisnummer mit Kreide

Status: Die neue Gleisnummer steht auf dem Boden. Bei Regen gilt wieder die alte.

Ergebnis: Am richtigen Gleis fahren Sie ab. Eine Taube übernimmt den Regenschutz für die Kreide.

**INTERN · Einstieg:** Nullbasiert: Spalte 2 hat Count 5 und ist voll. Die sichere senkrechte Belegung umfasst alle fünf Zeilen; die kurvenreiche Verbindung folgt erst aus weiteren Schlüssen.

### S1-02-04-08 · Lageplan für den fertig gebauten Bahnhof

Status: Der Aushang zeigt den Weg zum Anschluss. Gültig ist er ab 2042.

Ergebnis: Sie haben den Zug erreicht. Laut Aushang liegen Sie sechzehn Jahre vor dem Plan.

**INTERN · Einstieg:** Nullbasiert: Spalten 0 und 5 haben Count 0 und bleiben leer. Spalten 3 und 4 haben Count 5 und sind vollständig belegt.

### S1-02-04-09 · Abkürzung durch den gesperrten Gang

Status: Der Gang ist nur auf halber Länge gesperrt. Diese Hälfte liegt in der Mitte.

Ergebnis: Ihr Rollkoffer rollt in den Wagen. Die Abfahrt erfolgt noch im Jahr des Umwegs.

**INTERN · Einstieg:** Nullbasiert: B an S/2 belegt (2,4). Zeile 4 hat Count 1, deshalb bleiben ihre anderen fünf Felder leer.

### S1-02-04-10 · Ersatzbus an Gleis 6 angekündigt

Status: Die Tafel kündigt den Bus an Gleis 6 an. Die Straße ist noch nicht bestellt.

Ergebnis: Der Anschluss fährt als Zug. Damit konnte auf die Straße verzichtet werden.

**INTERN · Einstieg:** Nullbasiert: Spalten 0 und 1 sind bei Count 5 voll; Spalte 5 hat Count 0 und bleibt leer. Das begrenzt die große sichere Belegungsfläche.

### S1-02-04-11 · Umstieg über den provisorischen Steg

Status: Bitte nutzen Sie den provisorischen Steg. Er wird nächstes Jahr hundert.

Ergebnis: Der Steg hat Sie zum Zug gebracht. Seine befristete Nutzung geht ins zweite Jahrhundert.

**INTERN · Einstieg:** Nullbasiert: Zeile 2 hat Count 6 und ist vollständig belegt. Der sichere Queranker verbindet in der weiteren Herleitung die getrennten Randgebiete.

### S1-02-04-12 · Anschlussbahnsteig hinter neuer Wand

Status: Der Anschluss wartet hinter der Wand. Die Tür folgt im nächsten Bauabschnitt.

Ergebnis: Die Tür ist eingebaut, der Zug fährt. Der Umstieg gilt nun auch baulich als vorgesehen.

**INTERN · Einstieg:** Nullbasiert: Zeilen 3 und 4 haben jeweils Count 6 und sind voll. Die breite sichere Fläche bestimmt noch nicht die 19 Kurven des vollständigen Wegs.

### S1-03-01-01 · Halt am Ortsbahnhof entfällt

Status: Der Zug hält erst im Nachbarort. Ihr Ziel bleibt hier.

Ergebnis: Sie steigen am richtigen Bahnhof aus. Der Rückweg vom Nachbarort entfällt.

**INTERN · Einstieg:** Nullbasiert: Zeilen 0 und 5 haben Count 0 und bleiben leer. Die vier mittleren Zeilen enthalten alle geforderten Gleisfelder.

### S1-03-01-02 · Abfahrt ohne Einstieg

Status: Die Tafel meldet eine Abfahrt. Eine Ankunft war nicht vorgesehen.

Ergebnis: Die Tafel zeigt denselben Zug nun mit Halt. Sie dürfen tatsächlich mitfahren.

**INTERN · Einstieg:** Nullbasiert: Zeile 0 ist bei Breite 6 und Count 6 voll. Zeilen 3, 4 und 5 bleiben bei Count 0 leer.

### S1-03-01-03 · Durchsage nach dem ausgelassenen Halt

Status: Der Lautsprecher kündigt den entfallenen Halt an. Ihr Ort ist schon hinter uns.

Ergebnis: Die Ansage nennt Ihren Bahnhof vor der Ankunft. Diesmal bleibt Zeit zum Aussteigen.

**INTERN · Einstieg:** Nullbasiert: Spalte 0 hat bei Höhe 6 Count 6 und ist voll. A an N/4 liegt rechts davon; seine Lage hebt den linken Vollanker nicht auf.

### S1-03-01-04 · Türen bleiben am Bahnsteig geschlossen

Status: Der Zug steht am Bahnsteig. Er hält hier nur für das rote Signal.

Ergebnis: Die Türen gehen auf. Der Aufenthalt gilt heute auch für Sie.

**INTERN · Einstieg:** Nullbasiert: Zeile 5 hat Count 6 und ist vollständig belegt. Der sichere Queranker liegt unten, während B an N/4 die obere Eintrittszelle (4,0) erzwingt.

### S1-03-01-05 · Ersatzbus ab dem nächsten Bahnhof

Status: Der Aushang nennt einen Ersatzbus. Er startet dort, wo Sie nicht aussteigen konnten.

Ergebnis: Der Ersatzbus darf leer bleiben. Sie können wieder am eigenen Bahnhof aussteigen.

**INTERN · Einstieg:** Nullbasiert: Zeile 5 hat Count 6 und ist voll. Alle sechs Belegungen des unteren Querbands sind sicher, ihre Formen bleiben zunächst offen.

### S1-03-01-06 · Bahnhof wegen Baustelle ausgelassen

Status: Am Zaun steht „Fertig im Mai“. Der Mai hat keine Jahreszahl.

Ergebnis: Der Bauzaun ist weg, der Zug hält. Die Pressestelle erklärt den Mai für eingetreten.

**INTERN · Einstieg:** Nullbasiert: B an S/3 belegt (3,5). Zeile 5 fordert nur ein Gleisfeld; ihre übrigen fünf Felder bleiben leer.

### S1-03-01-07 · Haltwunsch ohne Halt

Status: Die Taste bestätigt Ihren Haltwunsch. Er wird bis zur Endstation gespeichert.

Ergebnis: Auf Ihren Tastendruck folgt ein Halt. Die Bestätigung besteht diesmal aus offenen Türen.

**INTERN · Einstieg:** Nullbasiert: Zeile 3 hat Count 6 und ist vollständig belegt. Der sichere Queranker reicht über das gesamte Raster und muss mit den Endpunktgebieten zusammenpassen.

### S1-03-01-08 · Bahnhof von der Tafel gestrichen

Status: Ihr Bahnhof fehlt auf der Tafel. Damit sind wir an keinem Halt vorbeigefahren.

Ergebnis: Ihr Bahnhof steht wieder auf der Tafel. Der Zug hält sogar am dazugehörigen Bahnsteig.

**INTERN · Einstieg:** Nullbasiert: A an S/5 erzwingt (5,5). Zeile 5 hat Count 1, daher bleiben alle anderen unteren Felder leer.

### S1-03-01-09 · Durchfahrt wegen leerem Bahnsteig

Status: Der Zug hält wegen fehlender Fahrgäste nicht. Gezählt wurde nachts.

Ergebnis: Auf dem Bahnsteig werden Sie endlich mitgezählt. Zum Beweis öffnet sich die Tür.

**INTERN · Einstieg:** Nullbasiert: B an S/2 belegt (2,5) und erfüllt den gesamten Count 1 der unteren Zeile. Die anderen fünf Felder dieser Zeile bleiben leer.

### S1-03-01-10 · Einstieg nur bei nachgewiesenem Bedarf

Status: Der Zettel verlangt einen Einstieg als Nachweis. Dafür müsste der Zug halten.

Ergebnis: Sie sind eingestiegen. Der Halt war damit rückwirkend gerechtfertigt.

**INTERN · Einstieg:** Nullbasiert: Spalte 0 hat Count 0 und bleibt leer. Zeilen 0 und 2 mit Count 5 müssen deshalb alle fünf übrigen Spalten belegen.

### S1-03-01-11 · Halt auf die Sitzbank verlegt

Status: Der Aushang bestätigt den Halt. Gemeint ist die Bank am Bahnsteig.

Ergebnis: Nun steht auch der Zug am Bahnsteig. Die Bank ist nicht mehr das einzige Reiseangebot.

**INTERN · Einstieg:** Nullbasiert: B an E/4 belegt (5,4). Spalte 5 hat Count 1 und ist damit erfüllt; ihre übrigen fünf Felder bleiben leer.

### S1-03-01-12 · Halt durch Blick aus dem Fenster

Status: Ihr Bahnhof gilt als bedient. Das Zugfenster war auf Ihrer Seite.

Ergebnis: Die Türen öffnen sich in Ihrem Ort. Die Aussicht lässt sich jetzt betreten.

**INTERN · Einstieg:** Nullbasiert: A an E/0 belegt (5,0). Das erfüllt den gesamten Count 1 von Spalte 5, deren übrige Zellen leer bleiben.

### S1-03-02-01 · Wagenreihung heute rückwärts

Status: Die Wagen stehen heute andersherum. Die Tafel kennt nur gestern.

Ergebnis: Alle sitzen. Der Zug fährt jetzt ohne Unterstützung der Tafel.

**INTERN · Einstieg:** Nullbasiert: Zeile 0 hat Count 0 und bleibt leer. Die übrigen fünf Zeilen bilden den verfügbaren Raum für die drei längeren Belegungsarme.

### S1-03-02-02 · Fahrradabteil laut Durchsage hinten

Status: Das Fahrradabteil finden Sie hinten. Gemeint ist der nächste Zug.

Ergebnis: Die Fahrräder sind an Bord. Die Durchsage darf nun denselben Zug meinen.

**INTERN · Einstieg:** Nullbasiert: Spalte 5 ist bei Höhe 6 mit Count 6 voll. Zeile 5 hat ebenfalls Count 6 und ist voll; beide sicheren Achsen kreuzen sich unten rechts.

### S1-03-02-03 · Kurzzug hält nicht bei Abschnitt D

Status: Bitte steigen Sie in Abschnitt A ein. Abschnitt D bedient heute die Wiese.

Ergebnis: Die Türen in Abschnitt A schließen. Auch ohne Besuch in D ist der Zug vollständig.

**INTERN · Einstieg:** Nullbasiert: Zeile 1 hat Count 6 und ist voll. Sie kreuzt die ebenfalls vollen Spalten 0 und 1; die Gleisformen bleiben trotz sicherer Fläche offen.

### S1-03-02-04 · Reservierung für den fehlenden Wagen 4

Status: Ihr Platz liegt in Wagen 4. Der Zug endet nach Wagen 3.

Ergebnis: Sie haben einen Sitzplatz. Diesmal ist auch ein Wagen darum herum.

**INTERN · Einstieg:** Nullbasiert: A an S/0 belegt (0,5). Zeile 5 hat Count 1, weshalb ihre anderen fünf Felder leer bleiben.

### S1-03-02-05 · Erste Klasse hinter dem Bahnsteigende

Status: Die erste Klasse befindet sich am Zugende. Der Bahnsteig endet früher.

Ergebnis: Die erste Klasse hält an einer Treppe. Der Aufpreis umfasst heute auch den Einstieg.

**INTERN · Einstieg:** Nullbasiert: Spalte 3 hat Count 6 und ist vollständig belegt. Sie liefert einen vertikalen Belegungsanker zwischen den unterschiedlich dichten oberen und unteren Gebieten.

### S1-03-02-06 · Hinterer Zugteil mit verschlossenen Türen

Status: Die hinteren Türen bleiben zu. Diese Wagen dienen der Längenangabe.

Ergebnis: Auch hinten steigen Sie ein. Der Zug befördert nun mehr als seine eigene Länge.

**INTERN · Einstieg:** Nullbasiert: A an N/1 belegt (1,0), die einzige Zelle von Zeile 0. Alle fünf anderen oberen Felder sind leer.

### S1-03-02-07 · Neuer Wagenstand nach jedem Rollkofferweg

Status: Bitte folgen Sie dem neuen Wagenplan. Ihr Koffer fährt heute weiter als der Zug.

Ergebnis: Ihr Koffer steht im Wagen. Die übrige Strecke übernimmt tatsächlich der Zug.

**INTERN · Einstieg:** Nullbasiert: Spalte 5 hat Count 6 und ist vollständig belegt. Der lange rechte Belegungsanker muss in der weiteren Herleitung mit beiden Endpunktgebieten verbunden werden.

### S1-03-02-08 · Wagen 2 zweimal am Bahnsteig

Status: Wagen 2 ist doppelt vorhanden. Damit gilt Wagen 3 als ersetzt.

Ergebnis: Jeder Wagen trägt eine andere Nummer. Die Tafel meldet einen Rückgang der Kapazität.

**INTERN · Einstieg:** Nullbasiert: Zeile 0 ist bei Count 0 leer. B an W/1 belegt (0,1), das einzige geforderte Feld von Zeile 1; ihr Rest bleibt leer.

### S1-03-02-09 · Erste Klasse nur auf der Tafel

Status: An den Türen steht nur zweite Klasse. Der Aufpreis richtet sich nach der Tafel.

Ergebnis: Die erste Klasse ist im Zug. Sie bezahlen erstmals für etwas, das mitfährt.

**INTERN · Einstieg:** Nullbasiert: Zeilen 0 und 1 sind mit Count 6 voll; Zeilen 4 und 5 haben Count 0 und bleiben leer. Die Belegung ist auf den oberen Bereich konzentriert.

### S1-03-02-10 · Zugteil zum Ziel bleibt am Bahnsteig

Status: Ihr Zugteil bleibt hier. Der Rest fährt zu Ihrem Ziel.

Ergebnis: Alle Wagen fahren zum Außenast. Ihr Ziel und Ihr Sitzplatz bleiben zusammen.

**INTERN · Einstieg:** Nullbasiert: Spalte 2 hat bei Höhe 6 Count 6 und ist vollständig belegt. Der sichere senkrechte Anker koppelt später den Kopfbereich mit dem unteren Bogen.

### S1-03-02-11 · Wagenplan für 2041 am Bahnsteig

Status: Der Aushang zeigt die Wagenfolge von 2041. Bis dahin läuft der Testbetrieb.

Ergebnis: Die Wagen stehen richtig, und Sie fahren mit. Für 2041 sind Sie damit deutlich zu früh.

**INTERN · Einstieg:** Nullbasiert: Zeile 5 hat Count 0 und bleibt leer. Die fünf oberen Zeilen enthalten alle Gleisfelder; ihre längeren Arme müssen über Restzahlen verbunden werden.

### S1-03-02-12 · Wagen richtig Bahnsteig falsch

Status: Die Wagen stehen genau wie auf der Tafel. Nun gilt der Bahnsteig als falsch.

Ergebnis: Die Pendler fahren im richtigen Wagen ab. Der Bahnsteig wurde per Durchsage berichtigt.

**INTERN · Einstieg:** Nullbasiert: Zeile 0 hat Count 6 und ist vollständig belegt. Der vertraute obere Belegungsanker bestimmt noch nicht die kurvenreiche Verbindung zum Westanschluss unten.

### S1-03-03-01 · Nur der vordere Zugteil fährt weiter

Status: Der hintere Zugteil bleibt am Bahnsteig. Ihr Sitzplatz fährt nicht mit.

Ergebnis: Beide Teile setzen sich in Bewegung. Sie dürfen tatsächlich sitzen bleiben.

**INTERN · Einstieg:** 0-basiert: Zeile 0 fordert 6 Gleisfelder bei Breite 6. Alle Felder (0,0) bis (5,0) sind deshalb belegt; die Gleisformen bleiben zunächst offen.

### S1-03-03-02 · Zwei Ziele an einem Zug

Status: Vorn steht Ihr Ziel, hinten das Depot. Zusammen fährt man nur bis hier.

Ergebnis: An beiden Wagen steht nun Ihr Ziel. Diesmal fahren sie auch dorthin.

**INTERN · Einstieg:** 0-basiert: A an N/1 erzwingt (1,0). Zeile 0 hat Count 1; ihre übrigen fünf Felder sind leer.

### S1-03-03-03 · Trennungsansage nach Türschluss

Status: Die Türen sind zu. Nun erfahren Sie, in welchem Zugteil Sie sitzen müssten.

Ergebnis: Die Türen öffnen noch einmal. Die Durchsage ist erstmals praktisch nutzbar.

**INTERN · Einstieg:** 0-basiert: Spalte 3 hat Count 6 bei Höhe 6. (3,0) bis (3,5) sind belegt, einschließlich des A-Eintritts an N/3.

### S1-03-03-04 · Weiterfahrt am anderen Bahnsteigende

Status: Bitte gehen Sie zum vorderen Wagen. Ihr Anschluss steht im selben Zug.

Ergebnis: Die Zugbegleitung meldet alle Türen geschlossen. Diesmal sind Sie auf der richtigen Seite.

**INTERN · Einstieg:** 0-basiert: Spalte 0 fordert bei Höhe 6 alle sechs Felder. (0,0) bis (0,5) sind sichere Belegung, ohne bereits ihre Formen festzulegen.

### S1-03-03-05 · Durchgang zum vorderen Zugteil fehlt

Status: Zwischen den Wagen gibt es keine Tür. Der kurze Weg führt durch den Regen.

Ergebnis: Sie sitzen wieder im Trockenen. Der Zug fährt, bevor ein weiterer Umzug nötig wird.

**INTERN · Einstieg:** 0-basiert: A an W/0 belegt (0,0). Damit ist Count 1 in Zeile 0 erfüllt; (1,0) bis (5,0) bleiben leer.

### S1-03-03-06 · Ihr Halt liegt hinter der Zugtrennung

Status: Ihr Bahnhof liegt am richtigen Ast. Ihr Wagen leider nicht.

Ergebnis: Vor dem Fenster erscheint Ihr Bahnhof. Der Zug hält sogar.

**INTERN · Einstieg:** 0-basiert: Zeile 0 hat Count 7 bei Breite 7. Sämtliche Felder (0,0) bis (6,0) sind belegt, darunter A an E/0.

### S1-03-03-07 · Hinterer Zugteil endet im Depot

Status: Am Fenster zieht Ihr Bahnhof vorbei. Der Wagen hat Feierabend, Sie nicht.

Ergebnis: Der Wagen erreicht den Bahnsteig statt der Halle. Feierabend ist nun auch für Sie möglich.

**INTERN · Einstieg:** 0-basiert: Spalte 0 hat Count 0 und bleibt leer. Zeile 5 fordert 6 der dann noch sechs verfügbaren Felder; (1,5) bis (6,5) sind belegt.

### S1-03-03-08 · Gleiswechsel nur für die vordere Hälfte

Status: Die Tafel schickt Sie zu Gleis 2. Ihr Wagen bleibt zur besseren Übersicht auf Gleis 1.

Ergebnis: Auf Gleis 2 wartet der ganze Zug. Die Übersicht hat ihren Widerstand aufgegeben.

**INTERN · Einstieg:** 0-basiert: A an W/0 erzwingt (0,0). Da Zeile 0 Count 1 hat, sind (1,0) bis (6,0) leer.

### S1-03-03-09 · Ersatzbus zwischen den Zugteilen

Status: Der Bus verbindet die beiden Wagen. Für einen ganzen Zug ist Umsteigen nötig.

Ergebnis: Die Wagen fahren gemeinsam weiter. Der Bus muss heute keinen Zug zusammenhalten.

**INTERN · Einstieg:** 0-basiert: Zeilen 0, 1 und 2 haben jeweils Count 0. Alle 21 Felder dieser drei Zeilen bleiben leer; der nutzbare Bereich liegt darunter.

### S1-03-03-10 · Wiedervereinigung laut Fahrplan gestern

Status: Am Bahnsteig hängt der Plan zur Weiterfahrt. Beide Teile trafen sich gestern.

Ergebnis: Die Verbindung steht, der Zug fährt. Heute wurde trotz abweichender Planung zugelassen.

**INTERN · Einstieg:** 0-basiert: B an N/0 erzwingt (0,0). Spalte 0 hat Count 1; (0,1) bis (0,5) sind daher leer.

### S1-03-03-11 · Ein Zug auf zwei Bahnsteigen

Status: Die Tafel zählt beide Hälften als einen Zug. Dazwischen liegen drei Stationen.

Ergebnis: Beide Hälften verlassen denselben Bahnsteig. Die Pressestelle nennt es räumlichen Fortschritt.

**INTERN · Einstieg:** 0-basiert: A an S/0 belegt (0,5). Zeile 5 fordert nur ein Gleisfeld; (1,5) bis (6,5) bleiben leer.

### S1-03-03-12 · Stehender Zugteil bereits auf Rückfahrt

Status: Ihr Wagen steht noch am Bahnsteig. Laut App bringt er Sie bereits zurück.

Ergebnis: Der ganze Zug erreicht Ihr Ziel. Die Rückfahrt muss erstmals auf die Hinfahrt warten.

**INTERN · Einstieg:** 0-basiert: Die sieben Felder der Zeile 0 müssen bei Count 7 sämtlich belegt sein: (0,0) bis (6,0). Ihre Formen sind noch nicht bestimmt.

### S1-03-04-01 · Anschluss vor Ihrer Ankunft abgefahren

Status: Ihr Anschluss hat den Bahnsteig verlassen. Der Fahrplan nennt das Umsteigen.

Ergebnis: Sie sitzen im Anschlusszug. Die Weiterfahrt findet diesmal mit Ihnen statt.

**INTERN · Einstieg:** 0-basiert: Zeile 0 hat Count 7 bei Breite 7. (0,0) bis (6,0) sind belegt; die Einserzeilen 1, 3 und 5 bestimmen davon unabhängig je nur ein Gleisfeld.

### S1-03-04-02 · Anschluss nur noch auf der Tafel

Status: Die Tafel zeigt Ihren Anschluss. Am Gleis steht nur seine Abfahrtszeit.

Ergebnis: Zur Anzeige ist ein Zug eingetroffen. Sie können jetzt auch körperlich weiterreisen.

**INTERN · Einstieg:** 0-basiert: Spalten 2 bis 6 haben jeweils Count 0. Ihre 30 Felder bleiben leer; die beiden aktiven Spalten 0 und 1 haben jeweils Count 5.

### S1-03-04-03 · Anschluss schließt die Türen

Status: Die Türen schließen bei Ihrer Ankunft. Ihr Zug war der Startschuss.

Ergebnis: Die Türen waren lange genug offen. Sie sind drin. Das zählt als gelungener Übergang.

**INTERN · Einstieg:** 0-basiert: Zeile 5 fordert bei Breite 7 sämtliche sieben Gleisfelder. (0,5) bis (6,5) sind deshalb belegt; ihre Formen folgen erst später.

### S1-03-04-04 · Anschluss nach Gleiswechsel weg

Status: Ihr Anschluss fährt am anderen Gleis. Die Treppe dauert länger als der Halt.

Ergebnis: Sie haben den Zug erreicht. Die Treppe wurde nicht als Zwischenaufenthalt berechnet.

**INTERN · Einstieg:** 0-basiert: Spalte 0 hat Count 0. In Zeile 1 bleiben damit sechs mögliche Felder für Count 6; (1,1) bis (6,1) sind belegt.

### S1-03-04-05 · Eine Stunde bis zum nächsten Zug

Status: Der nächste Zug kommt in einer Stunde. Ihr Kaffee ist schneller kalt.

Ergebnis: Ihr Kaffee bleibt kalt. Als Reiseergebnis kommt jetzt ein erreichter Bahnhof hinzu.

**INTERN · Einstieg:** 0-basiert: A an N/3 belegt (3,0). Count 1 in Zeile 0 schließt (0,0), (1,0), (2,0), (4,0), (5,0) und (6,0) aus.

### S1-03-04-06 · Anschluss fährt ohne Halt durch

Status: Ihr Anschluss fährt durch den Bahnhof. Der Halt besteht nur auf der Karte.

Ergebnis: Laut Durchsage dürfen Sie einsteigen. Der Halt findet diesmal außerhalb der Karte statt.

**INTERN · Einstieg:** 0-basiert: A an N/0 erzwingt (0,0). Spalte 0 fordert nur dieses eine Gleisfeld; (0,1) bis (0,5) bleiben leer.

### S1-03-04-07 · Ersatzbus endet vor dem Bahnhof

Status: Der Ersatzbus endet vor dem Bahnhof. Das letzte Stück ersetzt Ihr Fußweg.

Ergebnis: Vom Bus zum Zug hat es gereicht. Ihre Schuhe werden nicht mehr als Verkehrsmittel benötigt.

**INTERN · Einstieg:** 0-basiert: Spalte 2 hat Count 6 bei Höhe 6. Alle Felder (2,0) bis (2,5) sind belegt; dieser Vollanker verbindet die Zeileninformationen.

### S1-03-04-08 · Letzter Zug vor der Durchsage weg

Status: Der letzte Zug ist weg. Die Durchsage wünscht eine gute Weiterfahrt.

Ergebnis: Sie fahren im letzten Zug weiter. Die guten Wünsche durften am Bahnsteig bleiben.

**INTERN · Einstieg:** 0-basiert: Spalte 0 fordert alle sechs Felder (0,0) bis (0,5). Der A-Eintritt an W/0 gehört damit zum sicheren linken Belegungsarm.

### S1-03-04-09 · Anschluss zu Fuß empfohlen

Status: Der Aushang empfiehlt den Fußweg zum Anschluss. Es sind nur 42 Kilometer.

Ergebnis: Die Verbindung steht. Sie fahren weiter. Der Fußweg bleibt dem Marathon vorbehalten.

**INTERN · Einstieg:** 0-basiert: Zeilen 0 und 1 fordern jeweils 7 bei Breite 7 und sind vollständig belegt. Zeilen 4 und 5 haben Count 0 und bleiben vollständig leer.

### S1-03-04-10 · Anschluss wartet im nächsten Bahnhof

Status: Ihr Anschluss wartet im nächsten Bahnhof. Ihr Zug hält dort seit 1998 nicht mehr.

Ergebnis: Am Umstieg rollen wieder Koffer. Die Pause seit 1998 ist beendet.

**INTERN · Einstieg:** 0-basiert: Zeile 3 fordert 7 Gleisfelder bei Breite 7. (0,3) bis (6,3) sind sämtlich belegt, darunter der B-Eintritt an W/3.

### S1-03-04-11 · Anschluss am selben Gleis nächste Woche

Status: Die Tafel verspricht Anschluss am selben Gleis. Gemeint ist nächste Woche.

Ergebnis: Sie fahren noch heute weiter. Die Tafel bittet um Verständnis für den Zeitgewinn.

**INTERN · Einstieg:** 0-basiert: Spalte 4 hat Count 6 bei Höhe 6. (4,0) bis (4,5) sind belegt; B an N/4 liegt in diesem Vollanker.

### S1-03-04-12 · Umstieg in den Fahrplanaushang

Status: Ihr Anschluss ist im Fahrplan erreichbar. Bitte steigen Sie in den Aushang.

Ergebnis: Bitte nehmen Sie im Wagen Platz. Wir fahren weiter. Der Aushang bleibt Wandverkleidung.

**INTERN · Einstieg:** 0-basiert: A an W/0 belegt (0,0). Der Einsercount in Spalte 0 lässt (0,1) bis (0,5) leer; die Fortsetzung muss innerhalb der oberen Zeile liegen.

### S1-04-01-01 · Umleitung am Ziel vorbei

Status: Der Zug fährt am Ziel vorbei. Bitte bleiben Sie für den Umweg sitzen.

Ergebnis: Die Türen öffnen sich an Ihrem Ziel. Sie müssen diesmal nicht daran vorbeifahren.

**INTERN · Einstieg:** 0-basiert: B an E/0 belegt (6,0). Zeile 0 hat Count 1, also bleiben (0,0) bis (5,0) leer.

### S1-04-01-02 · Umweg über Gleis 8

Status: Die Tafel meldet Gleis 8. Der Weg dorthin zählt nicht zur Reisezeit.

Ergebnis: Sie sitzen im Zug. Der längste Abschnitt wurde zu Fuß erledigt.

**INTERN · Einstieg:** 0-basiert: Zeilen 0 bis 4 haben jeweils Count 0. Alle 35 Felder dieser Zeilen bleiben leer; nur Zeilen 5 und 6 können Gleisfelder enthalten.

### S1-04-01-03 · Ersatzhaltestelle hinter dem Parkhaus

Status: Der Ersatzbus hält hinter dem Parkhaus. Ab hier fährt nur Ihr Rollkoffer.

Ergebnis: Der Bus fährt mit Ihnen ab. Die Haltestelle war keine reine Suchaufgabe.

**INTERN · Einstieg:** 0-basiert: Spalten 0 und 6 haben Count 0. In Zeilen 0, 2, 4 und 6 verbleiben jeweils fünf mögliche Felder für Count 5; diese Felder in Spalten 1 bis 5 sind belegt.

### S1-04-01-04 · Ersatzbus über alle Ortsteile

Status: Der Bus fährt zum nächsten Bahnhof. Unterwegs bedient er den ganzen Landkreis.

Ergebnis: Sie steigen in den Zug. Die Rundfahrt war im Fahrpreis enthalten.

**INTERN · Einstieg:** 0-basiert: Spalten 0 und 1 haben jeweils Count 7 bei Höhe 7 und sind vollständig belegt. Spalten 5 und 6 haben Count 0 und bleiben leer.

### S1-04-01-05 · Zusätzlicher Halt am Startbahnhof

Status: Der Zug hält wieder am Start. Laut Tafel sind Sie jetzt weitergereist.

Ergebnis: Die Taube am Startbahnsteig sieht Sie abfahren. Eine vierte Begrüßung entfällt.

**INTERN · Einstieg:** 0-basiert: Spalte 6 hat Count 7. Alle Felder (6,0) bis (6,6) sind belegt; B an E/6 gehört zu dieser sicheren Belegung.

### S1-04-01-06 · Anschluss eine Minute vor Ankunft

Status: Der Bus kommt um 10.12 Uhr an. Der Anschluss wartet bis 10.11 Uhr.

Ergebnis: Sie erreichen den wartenden Zug. Die verlorene Minute wurde tatsächlich gefunden.

**INTERN · Einstieg:** 0-basiert: B an W/3 erzwingt (0,3). Spalte 0 hat Count 1; alle anderen Felder dieser Spalte bleiben leer.

### S1-04-01-07 · Weiterfahrt ab stillgelegtem Bahnhof

Status: Ihr Bus endet am Bahnhof. Der nächste Zug hält hier seit 2006 nicht mehr.

Ergebnis: Ein Zug hält und nimmt Sie mit. Der Bahnhof hat wieder einen Zweck.

**INTERN · Einstieg:** 0-basiert: Spalte 2 ist mit Count 7 über die Höhe 7 voll. (2,0) bis (2,6) sind belegt, darunter B an N/2.

### S1-04-01-08 · Ersatzhaltestelle laut Aushang

Status: Der Aushang führt Sie zur Ersatzhaltestelle. Dort hängt derselbe Aushang.

Ergebnis: Der letzte Pfeil endet an einer Bustür. Sie dürfen tatsächlich einsteigen.

**INTERN · Einstieg:** 0-basiert: Zeile 6 hat Count 7 bei Breite 7. Ihre Felder (0,6) bis (6,6) sind sicher belegt; die Form der Queranbindung bleibt offen.

### S1-04-01-09 · Ankunft mit zwei zusätzlichen Tagen

Status: Die Tafel meldet Ankunft um 08.03 Uhr. Gemeint ist übermorgen.

Ergebnis: Sie kommen noch heute an. Die Tafel verzichtet auf die zusätzlichen Tage.

**INTERN · Einstieg:** 0-basiert: Spalten 0, 2 und 4 haben jeweils Count 7 bei Höhe 7. Alle Felder dieser drei Spalten sind belegt; ihre Gleisformen sind noch offen.

### S1-04-01-10 · Umleitung auf der Rückseite des Globus

Status: Die Umleitung steht auf der Karte. Bitte drehen Sie den Globus weiter.

Ergebnis: Die Pressestelle meldet die Ankunft. Als Abkürzung wurde derselbe Kontinent genutzt.

**INTERN · Einstieg:** 0-basiert: Zeile 0 hat Count 0. Die Felder (0,0) bis (6,0) bleiben leer; sämtliche mögliche Belegung liegt in Zeilen 1 bis 6.

### S1-04-01-11 · Begrüßung aus dem entfernten Zielbahnhof

Status: Die Durchsage begrüßt Sie am Ziel. Der Zug ist noch zwei Bundesländer entfernt.

Ergebnis: Die Türen öffnen sich am richtigen Ort. Die Begrüßung passt erstmals dazu.

**INTERN · Einstieg:** 0-basiert: A an E/6 belegt (6,6). Spalte 6 hat Count 1; (6,0) bis (6,5) bleiben leer.

### S1-04-01-12 · Umweg über den gestrigen Fahrplan

Status: Der Aushang empfiehlt den Zug von gestern. Nur der war noch direkt.

Ergebnis: Sie kommen heute an. Das gilt hier bereits als Abkürzung.

**INTERN · Einstieg:** 0-basiert: A an E/0 erzwingt (6,0). Der Einsercount in Spalte 6 schließt (6,1) bis (6,6) aus.

### S1-04-02-01 · Umleitung mit zusätzlichem Halt

Status: Der Zug hält zusätzlich im Nachbarort. Ihr Ziel liegt vor dem Nachbarort.

Ergebnis: Sie sind am Ziel. Der Nachbarort muss heute ohne Sie auskommen.

**INTERN · Einstieg:** 0-basiert: Spalten 4, 5 und 6 sowie Zeilen 5 und 6 haben Count 0. Alle ihre Felder sind leer; die mögliche Belegung liegt in Spalten 0 bis 3 und Zeilen 0 bis 4.

### S1-04-02-02 · Abfahrt am anderen Bahnsteig

Status: Ihr Zug fährt von Gleis 8. Die Treppe dorthin zählt bereits als Reisezeit.

Ergebnis: Die Durchsage meldet Ihre Abfahrt. Sie stehen tatsächlich am genannten Gleis.

**INTERN · Einstieg:** 0-basiert: Spalten 0 und 2 haben jeweils Count 7 bei Höhe 7. (0,0) bis (0,6) und (2,0) bis (2,6) sind belegt.

### S1-04-02-03 · Ankunft eine Minute hinter dem Anschluss

Status: Der Anschluss fährt um 10 Uhr. Ihre Ankunft wurde auf 10.01 Uhr verbessert.

Ergebnis: Um 10.01 Uhr steigen Sie um. Der Anschluss wartet ohne Eingriff in die Bahnhofsuhr.

**INTERN · Einstieg:** 0-basiert: A an N/0 belegt (0,0). Spalte 0 hat Count 1 und schließt deshalb (0,1) bis (0,6) aus.

### S1-04-02-04 · Ersatzbus am Ortsrand

Status: Der Ersatzbus hält am Ortsrand. Der Fußweg ersetzt die erste Zugfahrt.

Ergebnis: Der Bus erreicht den Bahnhof. Sie müssen nur noch einsteigen, nicht hinwandern.

**INTERN · Einstieg:** 0-basiert: B an S/3 erzwingt (3,6). Zeile 6 fordert ein einziges Gleisfeld; ihre anderen sechs Felder bleiben leer.

### S1-04-02-05 · Zwischenhalt neben der Wiese

Status: Der Zug wartet neben einer Wiese. Der Fahrplan nennt das Durchfahrt.

Ergebnis: Es geht weiter. Die Wiese bleibt stehen, wie es für Wiesen vorgesehen ist.

**INTERN · Einstieg:** 0-basiert: A an W/0 belegt (0,0). In Spalte 0 ist Count 1 damit erfüllt; (0,1) bis (0,6) sind leer.

### S1-04-02-06 · Umweg zurück zum Startbahnhof

Status: Der Zug erreicht wieder Ihren Startbahnhof. Die Rückkehr gilt als Fortschritt.

Ergebnis: Die Türen öffnen sich am Ziel. Diesmal ist es nicht Ihr Startbahnhof.

**INTERN · Einstieg:** 0-basiert: A an E/3 belegt (6,3). Spalte 6 hat Count 1; die übrigen sechs Felder dieser Spalte sind leer.

### S1-04-02-07 · Ankunft nach Ladenschluss

Status: Der Zug kommt nach Ladenschluss an. Der Umweg war länger als der Einkauf.

Ergebnis: Bei Ihrer Ankunft ist die Ladentür noch offen. Der Einkauf muss nicht online nachreisen.

**INTERN · Einstieg:** 0-basiert: A an S/0 erzwingt (0,6). Zeile 6 hat Count 1, daher bleiben (1,6) bis (6,6) leer.

### S1-04-02-08 · Ankunftsdatum fehlt auf der Tafel

Status: Die Tafel zeigt Ihre Ankunft um 00.10 Uhr. Ein Datum würde Platz kosten.

Ergebnis: Sie steigen am Ziel aus. Auf Ihrem Handy steht noch derselbe Tag.

**INTERN · Einstieg:** 0-basiert: A an N/4 belegt (4,0). Count 1 der Zeile 0 schließt (0,0) bis (3,0) und (5,0) bis (6,0) aus.

### S1-04-02-09 · Dritte Runde um den Zielbahnhof

Status: Der Zug umrundet den Zielbahnhof. Der Halt ist erst nach Runde drei vorgesehen.

Ergebnis: Am Zielbahnsteig dürfen Sie aussteigen. Für die eingesparte Runde gibt es keine Erstattung.

**INTERN · Einstieg:** 0-basiert: Zeilen 0 bis 3 sind mit Count 0 leer. Zeile 5 fordert bei Breite 7 alle Felder (0,5) bis (6,5) als belegt.

### S1-04-02-10 · Monatskarte endet vor der Umleitung

Status: Ihre Monatskarte läuft im Zug ab. Für den restlichen Umweg beginnt ein neuer Monat.

Ergebnis: Sie sind da, Ihre Karte gilt noch. Die Pressestelle meldet eine verkürzte Monatsreise.

**INTERN · Einstieg:** 0-basiert: B an N/2 erzwingt (2,0). Zeile 0 hat Count 1, alle anderen Felder dieser Zeile sind leer.

### S1-04-02-11 · Weihnachtsmarkt erst im Januar erreichbar

Status: Die Tafel zeigt den 2. Januar. Sie sind für den Weihnachtsmarkt eingestiegen.

Ergebnis: Am Weihnachtsmarkt empfängt Sie Glühweinduft statt einer Abbaukolonne.

**INTERN · Einstieg:** 0-basiert: B an N/0 belegt (0,0), allein in Zeile 0 mit Count 1. Da (1,0) leer ist und B nach Norden anschließt, muss die Strecke nach (0,1) fortsetzen; Zeile 1 mit Count 1 ist damit ebenfalls bestimmt.

### S1-04-02-12 · Rückfahrt gestern Hinfahrt morgen

Status: Die Tafel zeigt Ihre Rückfahrt gestern. Die Hinfahrt dauert noch bis morgen.

Ergebnis: Ihre Ankunft steht jetzt auf der Tafel. Die Rückfahrt darf zeitlich dahinter bleiben.

**INTERN · Einstieg:** 0-basiert: A an W/0 belegt (0,0). Zeile 0 fordert ein Gleisfeld; (1,0) bis (6,0) sind deshalb leer.

### S1-04-03-01 · Abfahrt am anderen Gleis

Status: Ihr Zug fährt von Gleis 4. Die Änderung steht nur an Gleis 2.

Ergebnis: Sie fahren von Gleis 4 ab. Die exklusive Meldung an Gleis 2 ist nicht mitgekommen.

**INTERN · Einstieg:** 0-basiert: Spalte 6 fordert bei Höhe 7 alle sieben Felder (6,0) bis (6,6). Spalten 0 bis 4 haben Count 0 und bleiben leer.

### S1-04-03-02 · Zum Bahnsteig bitte durch den Ausgang

Status: Der Weg zum Zug führt durch den Ausgang. Bitte kommen Sie wieder herein.

Ergebnis: Sie sind zurück im Bahnhof. Diesmal dürfen Sie mit dem Zug hinaus.

**INTERN · Einstieg:** 0-basiert: Spalten 0 und 2 haben Count 7 bei Höhe 7. Beide Spalten sind vollständig belegt; über die Formen ist damit noch nichts entschieden.

### S1-04-03-03 · Ersatzbus außerhalb des Lageplans

Status: Der Ersatzbus hält hinter dem Bahnhof. Der Lageplan endet davor.

Ergebnis: Der Bus hat Sie zum Zug gebracht. Die Durchsage bedankt sich für Ihre Ortskenntnis.

**INTERN · Einstieg:** 0-basiert: A an S/1 belegt (1,6). Zeile 6 fordert nur ein Gleisfeld; (0,6) und (2,6) bis (6,6) bleiben leer.

### S1-04-03-04 · Gleiswechsel während des Treppenaufstiegs

Status: Ihr Gleis wurde erneut geändert. Sie können auf der Treppe wenden.

Ergebnis: Die Tafel bleibt bei Gleis 3. Der Zug fährt, bevor sie es sich anders überlegt.

**INTERN · Einstieg:** 0-basiert: Spalte 0 hat Count 0. In den Zeilen 0, 2, 4 und 6 verbleiben je sechs verfügbare Felder für Count 6; Spalten 1 bis 6 sind in diesen Zeilen belegt.

### S1-04-03-05 · Bahnsteigzugang hinter der Wand

Status: Der Durchgang ist zugemauert. Der Pfeil gilt weiterhin.

Ergebnis: Die Tür zum Bahnsteig ist offen. Ihr Zug fährt. Die Wand erhält keine Entschuldigung.

**INTERN · Einstieg:** 0-basiert: Zeilen 0 und 1 haben Count 0. Die Felder (0,0) bis (6,0) und (0,1) bis (6,1) bleiben leer.

### S1-04-03-06 · Zielbahnhof nur zur Durchfahrt

Status: Ihr Zug fährt ohne Halt durch Ihr Ziel. Bitte steigen Sie danach um.

Ergebnis: Am Ziel steigen die Pendler aus. Für die Rückfahrt fehlt nun der Grund.

**INTERN · Einstieg:** 0-basiert: Zeile 7 hat Count 0. Alle Felder (0,7) bis (7,7) bleiben leer; das begrenzt die mögliche Belegung auf Zeilen 0 bis 6.

### S1-04-03-07 · Drei Tafeln drei Abfahrtsgleise

Status: Die Tafeln nennen drei verschiedene Gleise. Ihr Zug hat nur eines gebucht.

Ergebnis: Alle Tafeln zeigen denselben Bahnsteig. Die Pressestelle meldet zwei Gleise eingespart.

**INTERN · Einstieg:** 0-basiert: Spalten 0, 2 und 4 haben jeweils Count 8. Sämtliche Felder dieser drei Spalten sind bei Höhe 8 belegt.

### S1-04-03-08 · Umleitung zurück zum Startbahnhof

Status: Ihre Fahrt führt über den Startbahnhof. Die Rückkehr zählt als Fortschritt.

Ergebnis: Vor dem Fenster erscheint ein neuer Bahnhof. Die Fahrt geht diesmal wirklich weiter.

**INTERN · Einstieg:** 0-basiert: Spalte 6 hat Count 8 und ist vollständig belegt. In den Zeilen 4 und 5 mit Count 1 liegen damit nur (6,4) und (6,5); ihre anderen Felder sind leer.

### S1-04-03-09 · Anschluss laut Tafel im Vorjahr

Status: Die Tafel nennt den Anschluss um 8 Uhr. Das Datum gehört zum Vorjahr.

Ergebnis: Der Anschluss fährt heute. Das Datum auf der Tafel hält mit.

**INTERN · Einstieg:** 0-basiert: Zeile 3 hat Count 8 bei Breite 8 und ist vollständig belegt. Zeilen 0, 1 und 2 haben Count 0 und bleiben leer.

### S1-04-03-10 · Wegweiser im geschlossenen Kreis

Status: Ein Pfeil führt zum nächsten. Der letzte zeigt zurück zum ersten.

Ergebnis: Sie erreichen den Zug ohne zweite Runde. Er fährt sogar weiter.

**INTERN · Einstieg:** 0-basiert: A an E/1 erzwingt (7,1). Zeile 1 hat Count 1; (0,1) bis (6,1) bleiben leer.

### S1-04-03-11 · Weg zum Zug nur per QR-Code im Zug

Status: Der QR-Code zeigt den Weg zum Zug. Zum Scannen müssen Sie im Zug sitzen.

Ergebnis: Der Weg ist ohne QR-Code begehbar. Ihr Zug fährt auch ohne Anmeldung.

**INTERN · Einstieg:** 0-basiert: B an S/0 belegt (0,7). Zeile 7 hat Count 1; (1,7) bis (7,7) bleiben leer.

### S1-04-03-12 · Weiterfahrt über die Brücke auf dem Plan

Status: Laut Plan fährt Ihr Zug über die Brücke. Die Brücke steht nur im Plan.

Ergebnis: Die Verbindung steht jetzt auch außerhalb der Karte. Der Zug fährt.

**INTERN · Einstieg:** 0-basiert: A an E/0 belegt (7,0). Spalte 7 hat Count 1; (7,1) bis (7,7) sind leer.

### S1-04-04-01 · Abfahrt vom anderen Bahnsteig

Status: Ihr Zug fährt gegenüber. Der direkte Weg führt über die Straße.

Ergebnis: Sie sitzen im Zug. Die andere Straßenseite war tatsächlich der richtige Bahnsteig.

**INTERN · Einstieg:** 0-basiert: Zeile 0 hat Count 8 bei Breite 8 und ist vollständig belegt. Zeilen 2 bis 7 haben jeweils Count 0 und bleiben vollständig leer.

### S1-04-04-02 · Ersatzbus hinter dem Bahnhof

Status: Der Ersatzbus hält hinter dem Bahnhof. Die Abkürzung endet am Bauzaun.

Ergebnis: Der Bus hat Sie zum Zug gebracht. Ihr Rollkoffer darf ab hier wieder rollen.

**INTERN · Einstieg:** 0-basiert: Zeile 0 fordert alle acht Felder (0,0) bis (7,0). Spalte 6 fordert alle acht Felder (6,0) bis (6,7); beide Vollanker kreuzen sich in (6,0).

### S1-04-04-03 · Treppe zum Ersatzbus gesperrt

Status: Die Treppe zum Bus ist gesperrt. Der Umweg führt zum ausgefallenen Zug.

Ergebnis: Der Weg zum Bus ist frei. Ihr Zug wartet diesmal am anderen Ende der Busfahrt.

**INTERN · Einstieg:** 0-basiert: Spalte 0 hat Count 0. Zeilen 0, 2, 4 und 6 fordern jeweils 7 bei sieben verbleibenden Feldern; Spalten 1 bis 7 sind dort belegt.

### S1-04-04-04 · Umleitung durch die Bahnhofshalle

Status: Bitte folgen Sie den gelben Pfeilen. Der letzte zeigt auf den ersten.

Ergebnis: Die Pfeile führen zum Zug. Er fährt weiter, statt Sie zurückzuschicken.

**INTERN · Einstieg:** 0-basiert: Zeile 7 hat Count 8 und ist vollständig belegt. Zeilen 0 und 1 haben Count 0 und schließen ihre 16 Felder aus.

### S1-04-04-05 · Ersatzbus fährt am Ersatzhalt vorbei

Status: Der Bus fährt zum Ersatzhalt. Ein Halt ist dort nicht vorgesehen.

Ergebnis: Der Bus hat gehalten. Sie erreichen Ihren Zug ohne Absprung.

**INTERN · Einstieg:** 0-basiert: Spalte 1 hat Count 8 bei Höhe 8. Alle Felder (1,0) bis (1,7) sind belegt; die konkrete Gleisgeometrie ist noch offen.

### S1-04-04-06 · Gleiswechsel nach dem Fußweg

Status: Der Weg zu Gleis 4 ist frei. Ihr Zug fährt inzwischen von Gleis 1.

Ergebnis: Bitte bleiben Sie stehen. Ihr Gleis stimmt jetzt mit der Abfahrt überein.

**INTERN · Einstieg:** 0-basiert: A an E/5 belegt (7,5). Der Einsercount in Spalte 7 lässt alle anderen Felder dieser Spalte leer.

### S1-04-04-07 · Umstieg am Nachbarbahnhof

Status: Der Aushang empfiehlt den Nachbarbahnhof. Zu Fuß überholen Sie den Ersatzbus.

Ergebnis: Sie erreichen den Zug vor dem Bus. Die Pressestelle lobt die schnelle Verbindung.

**INTERN · Einstieg:** 0-basiert: Spalten 0 und 1 haben Count 0. Ihre 16 Felder bleiben leer; die mögliche Belegung liegt in Spalten 2 bis 7.

### S1-04-04-08 · Rückfahrt zum Anschluss

Status: Ihr Anschluss fährt am vorigen Bahnhof. Zurück fährt nur Ihr Anschluss.

Ergebnis: Der Anschluss ist erreicht. Der Zug fährt weiter, ohne Sie zurückzuverweisen.

**INTERN · Einstieg:** 0-basiert: Spalte 0 hat Count 8 und ist vollständig belegt: (0,0) bis (0,7). Dieser Anker legt noch keine Gleisform fest.

### S1-04-04-09 · Bushaltestelle außerhalb der Karte

Status: Die Karte zeigt den Weg zum Bus. Die Haltestelle liegt jenseits des Kartenrands.

Ergebnis: Die Tafel zeigt wieder Ihr Ziel. Eine zweite Landkarte ist für die Fahrt nicht nötig.

**INTERN · Einstieg:** 0-basiert: B an W/3 erzwingt (0,3). Count 1 der Spalte 0 schließt (0,0) bis (0,2) und (0,4) bis (0,7) aus.

### S1-04-04-10 · Ersatzbus ab alter Haltestelle

Status: Der Zettel nennt die alte Bushaltestelle. Sie wurde 1998 abgerissen.

Ergebnis: Die Strecke ist frei. Der Zug hält an einem Bahnhof, der noch steht.

**INTERN · Einstieg:** 0-basiert: Zeile 0 hat Count 0. Da A an S/7 (7,7) belegt und die Zeilen 7, 6 und 5 jeweils Count 1 haben, erzwingen die Anschlüsse nacheinander (7,6) und (7,5); die übrigen Felder dieser drei Zeilen bleiben leer.

### S1-04-04-11 · Gleisangabe erst im Zug

Status: Das Gleis wird im Zug angesagt. Zum Einsteigen benötigen Sie diese Angabe.

Ergebnis: Die Durchsage nennt das Gleis vor der Abfahrt. Der Zug fährt trotzdem.

**INTERN · Einstieg:** 0-basiert: A an W/7 belegt (0,7). Zeile 7 fordert ein Gleisfeld; (1,7) bis (7,7) bleiben leer.

### S1-04-04-12 · Ersatzbus zurück zum Startbahnsteig

Status: Der Bus bringt Sie zum Startbahnsteig zurück. Die Rundfahrt gilt als Anreise.

Ergebnis: Laut Pressestelle beginnt die Zugfahrt jetzt. Die Ehrenrunde war keine Reisezeit.

**INTERN · Einstieg:** 0-basiert: Spalte 0 fordert bei Höhe 8 alle acht Felder (0,0) bis (0,7). Die volle linke Belegung verbindet die Zeileninformationen, ohne die Formen vorwegzunehmen.

### S1-05-01-01 · Ersatzbus am Parkplatz

Status: Der Bus hält laut Tafel am Parkplatz. Ihr Ticket gilt auch für den Fußweg.

Ergebnis: Der Bus fährt. Ab hier übernimmt wieder ein Fahrzeug.

**INTERN · Einstieg:** 0-basiert: Zeilen 0 bis 5 haben jeweils Count 0. Alle 48 Felder dieser Zeilen bleiben leer; nur die beiden unteren Zeilen können Gleisfelder enthalten.

### S1-05-01-02 · Wegweiser erst an der Haltestelle

Status: Der Weg zum Bus ist ausgeschildert. Das erste Schild steht am Ziel.

Ergebnis: Sie haben den Bus gefunden. Das Schild bestätigt Ihre Ankunft.

**INTERN · Einstieg:** 0-basiert: Spalten 0 und 2 haben Count 8 bei Höhe 8. Beide Spalten sind vollständig belegt: (0,0) bis (0,7) und (2,0) bis (2,7).

### S1-05-01-03 · Fünf Minuten Fußweg laut Aushang

Status: Laut Aushang sind es fünf Minuten zum Bus. Gemessen wurde ohne Ampeln.

Ergebnis: Alle Ampeln sind passiert. Der Bus fährt, ohne dass Sie weiterlaufen müssen.

**INTERN · Einstieg:** 0-basiert: Zeile 0 hat Count 8 und ist vollständig belegt, einschließlich A an N/0. Ihre acht Felder sind (0,0) bis (7,0).

### S1-05-01-04 · Tür zur Ersatzhaltestelle verschlossen

Status: Der Bus hält direkt hinter der Glastür. Die Tür öffnet erst morgen.

Ergebnis: Der Bus ist über den Umweg erreicht. Die Glastür bleibt ihrem Zeitplan treu.

**INTERN · Einstieg:** 0-basiert: A an E/0 belegt (7,0). Zeile 0 hat Count 1; alle anderen Felder (0,0) bis (6,0) bleiben leer.

### S1-05-01-05 · Ersatzbus hinter dem Bauzaun

Status: Der Bus hält zehn Meter hinter dem Zaun. Der Durchgang liegt einen Kilometer weiter.

Ergebnis: Der Zaun ist umrundet, der Bus fährt. Die Pressestelle lobt die Nähe zum Bahnhof.

**INTERN · Einstieg:** 0-basiert: Zeile 1 fordert bei Breite 8 alle acht Felder. (0,1) bis (7,1) sind deshalb belegt; ihre Formen sind noch nicht bestimmt.

### S1-05-01-06 · Haltestelle verlegt Schild bleibt stehen

Status: Die Haltestelle wurde verlegt. Am alten Schild steht: Sie sind hier.

Ergebnis: Der Bus fährt vom neuen Standort ab. Der alte ist weiterhin gut beschildert.

**INTERN · Einstieg:** 0-basiert: B an N/1 erzwingt (1,0). Der Einsercount der Zeile 0 schließt (0,0) und (2,0) bis (7,0) aus.

### S1-05-01-07 · Ersatzbus gleich nebenan auf dem Plan

Status: Auf dem Plan steht der Bus gleich nebenan. Ein Zentimeter sind zwei Kilometer.

Ergebnis: Der Bus ist erreicht und fährt. Auf dem Plan war das ein sehr kleiner Umweg.

**INTERN · Einstieg:** 0-basiert: Zeile 3 hat Count 8 und ist vollständig belegt. Zeilen 0 und 1 haben Count 0; ihre 16 Felder bleiben leer.

### S1-05-01-08 · Gelbe Pfeile zurück zum Bahnsteig

Status: Bitte folgen Sie den gelben Pfeilen. Sie führen zum ausgefallenen Zug.

Ergebnis: Die Durchsage meldet Abfahrt. Diesmal fährt tatsächlich etwas: der Bus.

**INTERN · Einstieg:** 0-basiert: Spalten 6 und 7 haben jeweils Count 0. (6,0) bis (6,7) und (7,0) bis (7,7) bleiben leer.

### S1-05-01-09 · Busabfahrt vor Ende des Fußwegs

Status: Laut Ansage dauert der Fußweg zwölf Minuten. Der Bus wartet drei.

Ergebnis: Sie sitzen im fahrenden Bus. Der Fahrplan hatte diesen Fall nicht vorgesehen.

**INTERN · Einstieg:** 0-basiert: B an S/1 belegt (1,7). Zeile 7 hat Count 1; (0,7) und (2,7) bis (7,7) bleiben leer.

### S1-05-01-10 · Ersatzhaltestelle am Bahnhof im Nachbarort

Status: Der Zettel nennt den Bahnhof als Bushalt. Gemeint ist der im nächsten Ort.

Ergebnis: Der Bus fährt. Der Weg zur Haltestelle war bereits die halbe Reise.

**INTERN · Einstieg:** 0-basiert: Zeilen 6 und 7 haben Count 0. Ihre 16 Felder bleiben leer; Kopf- und Mittelgebiet liegen oberhalb dieser Grenze.

### S1-05-01-11 · Fußweg zählt als erste Busfahrt

Status: Der Aushang zählt den Fußweg als Fahrt. Der erste Bus sind Ihre Schuhe.

Ergebnis: Sie sitzen, der Bus fährt. Die Statistik meldet einen nahtlosen Fahrzeugwechsel.

**INTERN · Einstieg:** 0-basiert: A an S/7 erzwingt (7,7). Zeile 7 fordert nur ein Gleisfeld; (0,7) bis (6,7) bleiben leer.

### S1-05-01-12 · Haltestellenschild fährt im Bus

Status: Das Haltestellenschild fährt im Bus mit. Bitte folgen Sie ihm zu Fuß.

Ergebnis: Der Bus hält und nimmt Sie mit. Die Haltestelle fährt diesmal nicht allein.

**INTERN · Einstieg:** 0-basiert: A an E/0 belegt (7,0). Der Einsercount der Zeile 0 lässt alle Felder (0,0) bis (6,0) leer.

### S1-05-02-01 · Am Bussteig fehlt der Ersatzbus

Status: Am Bussteig fehlt der Bus. Der Fahrplan ist schon da.

Ergebnis: Ihr Kaffee ist bei der Abfahrt noch warm. Bitte sichern Sie den Becher als Nachweis.

**INTERN · Einstieg:** 0-basiert: Zeile 4 hat Count 8 und ist vollständig belegt. Zeilen 0, 6 und 7 haben Count 0 und bleiben leer.

### S1-05-02-02 · Ersatzhalt hinter dem Bahnhof

Status: Der Aushang schickt Sie hinter den Bahnhof. Dort hängt derselbe Aushang.

Ergebnis: Ihr Rollkoffer rollt zum abfahrbereiten Zug. Die Ortsbesichtigung ist abgeschlossen.

**INTERN · Einstieg:** 0-basiert: Zeile 0 hat Count 0. Alle acht Felder (0,0) bis (7,0) bleiben leer; der obere Rand ist damit für weitere Belegung ausgeschlossen.

### S1-05-02-03 · Busankunft nur per Durchsage

Status: Die Durchsage kündigt den Bus an. Lautsprecher können nicht fahren.

Ergebnis: Die Ansage stimmt: Der Zug fährt ab. Das Mikrofon meldet keinen Widerspruch.

**INTERN · Einstieg:** 0-basiert: A an E/7 belegt (7,7). Zeile 7 hat Count 1 und schließt (0,7) bis (6,7) aus; die Fortsetzung nach (7,6) ist erzwungen. Die weiteren Schritte benötigen die übrigen Zeilen- und Spaltenreste.

### S1-05-02-04 · Kleinbus ersetzt vier Wagen

Status: Ein Kleinbus steht für den Zug bereit. Vier Wagen passen laut Plan hinein.

Ergebnis: Die Zugbegleitung meldet Platz für alle. Vier Wagen wurden durch vier Wagen ersetzt.

**INTERN · Einstieg:** 0-basiert: B an N/7 belegt (7,0). Spalte 7 hat Count 1; (7,1) bis (7,7) bleiben leer. Dieser lokale Anker klärt nur den Eintritt, danach sind mehrere mittlere Restcounts zu kombinieren.

### S1-05-02-05 · Anschluss fährt vor dem Ersatzbus

Status: Der Anschluss fährt vor dem Bus ab. So bleibt er pünktlich.

Ergebnis: Ihr Zug fährt erst nach Ihrer Ankunft ab. Wir melden keinen Rechenfehler.

**INTERN · Einstieg:** 0-basiert: A an W/0 belegt (0,0). Spalte 0 hat Count 1 und schließt (0,1) bis (0,7) aus. Der Eintritt ist damit lokal geklärt; die Belegung der übrigen Gebiete folgt erst aus kombinierten Restzahlen.

### S1-05-02-06 · Ersatzbus steht neben freien Schienen

Status: Ihr Ersatzbus steht vor der roten Ampel. Die Schienen daneben sind frei.

Ergebnis: Sie fahren wieder auf Schienen. Die rote Ampel darf ohne Sie weiterleuchten.

**INTERN · Einstieg:** 0-basiert: A an W/0 belegt (0,0). Count 1 in Zeile 0 macht (1,0) bis (8,0) leer. Dies ist ein sicherer lokaler Anker des 9×9-Felds.

### S1-05-02-07 · Zwei Minuten bis zum Ersatzbus

Status: Die Tafel meldet zwei Minuten Wartezeit. Die Stunde zählt nicht mit.

Ergebnis: Die Tafel zeigt die Abfahrt an. Diesmal bewegt sich auch der Zug.

**INTERN · Einstieg:** 0-basiert: Zeile 0 hat Count 9 bei Breite 9. Alle neun Felder (0,0) bis (8,0) sind belegt; dazu gehört A an N/0.

### S1-05-02-08 · Bus hält am alten Ersatzhalt

Status: Der Ersatzhalt wurde verlegt. Der Bus hält noch am alten Pfeil.

Ergebnis: Die Taube bleibt auf dem alten Pfeil sitzen. Alle anderen fahren vom richtigen Gleis ab.

**INTERN · Einstieg:** 0-basiert: A an S/8 belegt (8,8). Zeile 8 hat Count 1, daher sind (0,8) bis (7,8) leer.

### S1-05-02-09 · Sonntagsfahrplan am Montag

Status: Am Ersatzhalt hängt der Sonntagsfahrplan. Montag ist nicht vorgesehen.

Ergebnis: Für Montag steht eine Abfahrt auf der Tafel. Der Tag wurde in die Woche aufgenommen.

**INTERN · Einstieg:** 0-basiert: Spalten 0, 2 und 4 haben jeweils Count 9 bei Höhe 9. Alle Felder dieser drei Spalten sind sicher belegt.

### S1-05-02-10 · Ersatzbus umrundet den Bahnhof

Status: Der Bus fährt im Kreis um den Bahnhof. Jede Runde gilt als neue Verbindung.

Ergebnis: Vom nächsten Bahnsteig ist der Startbahnhof nicht zu sehen. Das gilt als Fortschritt.

**INTERN · Einstieg:** 0-basiert: Spalte 8 hat Count 0. Zeilen 4, 6 und 8 fordern jeweils 8 Gleisfelder; die verbleibenden acht Felder in Spalten 0 bis 7 sind dort sämtlich belegt.

### S1-05-02-11 · Bus laut Anzeige bereits angekommen

Status: Ihr Bus ist laut Tafel angekommen. Bitte steigen Sie in die Anzeige ein.

Ergebnis: Ein echter Zug fährt ab. Die Tafel verzichtet auf eine Gegendarstellung.

**INTERN · Einstieg:** 0-basiert: B an N/3 belegt (3,0). Zeile 0 fordert nur ein Gleisfeld; (0,0) bis (2,0) und (4,0) bis (8,0) bleiben leer.

### S1-05-02-12 · Ersatzbus bekommt einen Ersatzbus

Status: Am Bus hängt ein Zettel: Ersatzbus folgt. Der hat denselben Zettel.

Ergebnis: Ein Bus bleibt übrig, die Bahn fährt. Sein Zettel wird nicht mehr als Fahrzeug gezählt.

**INTERN · Einstieg:** 0-basiert: A an S/8 belegt (8,8). Der Einsercount in Spalte 8 lässt (8,0) bis (8,7) leer.

### S1-05-03-01 · Ersatzbus fährt vor der Zugankunft

Status: Ihr Zug fährt ein. Der Bus ist pünktlich ohne Sie abgefahren.

Ergebnis: Der Bus wartet bis zur Zugankunft. Die Weiterfahrt gelingt ohne Dauerlauf.

**INTERN · Einstieg:** 0-basiert: Zeile 0 fordert 9 bei Breite 9 und ist vollständig belegt. Spalte 8 fordert ebenfalls 9 bei Höhe 9 und ist vollständig belegt; beide treffen sich in (8,0).

### S1-05-03-02 · Ersatzhalt am falschen Ausgang

Status: Der Bus hält hinter dem Bahnhof. Der einzige Ausgang liegt vorne.

Ergebnis: Der Bus steht jetzt am Ausgang. Der Bahnhof muss nicht mehr umrundet werden.

**INTERN · Einstieg:** 0-basiert: Spalte 0 hat Count 0. Zeilen 0, 6 und 8 haben jeweils Count 8; ihre acht Felder in Spalten 1 bis 8 sind belegt.

### S1-05-03-03 · Zum Ersatzbus nur über die Treppe

Status: Der Aufzug zum Bus ist gesperrt. Die Treppe gilt als gleichwertiger Ersatz.

Ergebnis: Der Aufzug fährt wieder. Ihr Rollkoffer bleibt heute ein Rollkoffer.

**INTERN · Einstieg:** 0-basiert: A an W/0 belegt (0,0). Count 1 der Zeile 0 schließt (1,0) bis (8,0) aus.

### S1-05-03-04 · Anschlussbus bleibt verschlossen

Status: Der Bus steht mit geschlossenen Türen da. Laut Anzeige sind Sie angekommen.

Ergebnis: Die Türen öffnen sich. Der Anschluss besteht jetzt auch für Personen.

**INTERN · Einstieg:** 0-basiert: Zeile 8 hat Count 9 bei Breite 9. (0,8) bis (8,8) sind sämtlich belegt, darunter A an W/8.

### S1-05-03-05 · Anschlusszug auf ein anderes Gleis verlegt

Status: Ihr Zug fährt jetzt an Gleis 8. Der Wegweiser vom Bus kennt nur Gleis 1.

Ergebnis: Die Durchsage nennt das richtige Gleis. Ihr Zug steht freundlicherweise dort.

**INTERN · Einstieg:** 0-basiert: Spalte 0 hat Count 0. Alle neun Felder (0,0) bis (0,8) bleiben leer; die Querarme liegen in den verbleibenden Spalten.

### S1-05-03-06 · Keine Gehzeit zwischen Bus und Zug

Status: Der Bus kommt um 8.10 Uhr. Ihr Zug fährt um 8.10 Uhr hinter dem Parkhaus.

Ergebnis: Drei Minuten bis zur Abfahrt bleiben frei. Gehen ist wieder Teil des Fahrplans.

**INTERN · Einstieg:** 0-basiert: B an E/7 belegt (8,7). Zeile 7 hat Count 1; (0,7) bis (7,7) bleiben leer.

### S1-05-03-07 · Direktbus mit zwölf Zwischenhalten

Status: Der Bus fährt direkt zum Anschlusszug. Die zwölf Zwischenhalte zählen nicht.

Ergebnis: Ihr Anschluss fährt mit Ihnen ab. Für die entfallene Stadtrundfahrt gibt es keinen Ersatz.

**INTERN · Einstieg:** 0-basiert: Zeile 8 hat Count 9 bei Breite 9. Alle Felder (0,8) bis (8,8) sind belegt, einschließlich B an S/8.

### S1-05-03-08 · Bahnhofsuhr meldet Anschluss verpasst

Status: Die Bahnhofsuhr geht zehn Minuten vor. Laut Tafel sind Sie schon zu spät.

Ergebnis: Die Uhren stimmen überein. Der Zug ist noch da. Beides wird dokumentiert.

**INTERN · Einstieg:** 0-basiert: Spalte 8 hat Count 0. Zeilen 2 und 4 fordern jeweils 8 bei acht verbleibenden Feldern; (0,2) bis (7,2) und (0,4) bis (7,4) sind belegt.

### S1-05-03-09 · Wegweiser zum Ersatzbus führen im Kreis

Status: Die Pfeile führen zum Ersatzbus. Am Ende steht wieder der erste Pfeil.

Ergebnis: Der letzte Pfeil zeigt auf einen Bus. Dieser fährt sogar zum Anschlusszug.

**INTERN · Einstieg:** 0-basiert: Zeile 1 hat Count 9 bei Breite 9. Alle neun Felder (0,1) bis (8,1) sind belegt; ihre Formen sind noch offen.

### S1-05-03-10 · Anschlussbus wartet nur auf der Tafel

Status: Die Tafel meldet einen wartenden Bus. Er wartet jetzt am nächsten Bahnhof.

Ergebnis: Bus und Tafel befinden sich wieder am selben Ort. Sie können dort einsteigen.

**INTERN · Einstieg:** 0-basiert: Zeilen 0 bis 2 und Spalten 6 bis 8 haben jeweils Count 0. Alle betroffenen Felder bleiben leer; aktiv ist der Bereich Spalten 0 bis 5, Zeilen 3 bis 8.

### S1-05-03-11 · Busanschluss laut historischem Aushang

Status: Der Aushang nennt den Bus um 9 Uhr. Gemeint ist der 4. Mai 1987.

Ergebnis: Der Bus fährt heute. Die Pressestelle meldet einen Anschluss an die Gegenwart.

**INTERN · Einstieg:** 0-basiert: B an W/1 belegt (0,1). Spalte 0 hat Count 1 und schließt ihre übrigen acht Felder aus.

### S1-05-03-12 · Zug und Ersatzbus warten aufeinander

Status: Der Bus wartet auf den Zug. Der Zug kommt erst, wenn der Bus da ist.

Ergebnis: Bus und Zug sind da. Die Durchsage bittet darum, dieses Verfahren nicht zu erwarten.

**INTERN · Einstieg:** 0-basiert: A an E/8 belegt (8,8). Spalte 8 fordert nur dieses Gleisfeld; (8,0) bis (8,7) bleiben leer.

### S1-05-04-01 · Ersatzbus am anderen Ausgang

Status: Der Bus hält am anderen Ausgang. Ihr Zugticket enthält keinen Kompass.

Ergebnis: Die Bahn fährt wieder. Zur Abfahrt genügt jetzt der Weg bis zur Zugtür.

**INTERN · Einstieg:** 0-basiert: Zeilen 0 bis 2 und Spalten 4 bis 8 haben Count 0. Alle betroffenen Felder bleiben leer; mögliche Gleisfelder liegen in Spalten 0 bis 3 und Zeilen 3 bis 8.

### S1-05-04-02 · Abfahrtszeit fehlt am Bussteig

Status: Am Bussteig hängt der Zugfahrplan. Die Buszeiten bleiben Privatsache.

Ergebnis: Auf der Tafel steht eine Abfahrt. Am Gleis steht sogar das passende Fahrzeug.

**INTERN · Einstieg:** 0-basiert: Spalte 5 hat Count 9 bei Höhe 9 und ist vollständig belegt. In den Zeilen 2, 3 und 4 mit Count 1 sind deshalb nur (5,2), (5,3) und (5,4) belegt; die anderen Felder dieser Zeilen sind leer.

### S1-05-04-03 · Ersatzbus mit begrenztem Platz

Status: Der Bus ist voll. Acht Wagen wurden durch einen Mittelgang ersetzt.

Ergebnis: Alle Wagen rollen. Die versprochene Kapazität hat wieder Türen.

**INTERN · Einstieg:** 0-basiert: Zeile 8 hat Count 0 und bleibt leer. Spalten 0, 2 und 4 fordern jeweils 8; bei nur acht verbleibenden Zeilen sind ihre Felder in Zeilen 0 bis 7 vollständig belegt.

### S1-05-04-04 · Fahrkarten am gesperrten Gleis

Status: Der Automat steht am gesperrten Gleis. Der Fahrkartenverkauf läuft stabil.

Ergebnis: Ihre Fahrkarte gilt, der Zug fährt. Der Weg zum Automaten ist kein Reiseabschnitt mehr.

**INTERN · Einstieg:** 0-basiert: A an E/2 erzwingt (8,2). Spalte 8 hat Count 2; nach diesem sicheren Eintritt bleibt dort genau ein weiteres Gleisfeld zu bestimmen.

### S1-05-04-05 · Ersatzbus nur über Stufen erreichbar

Status: Zum Bus führt nur die Treppe. Der Aufzug ersetzt heute die Wand.

Ergebnis: Ihr Rollkoffer rollt in die Bahn. Für den Einstieg entfällt die Flugphase.

**INTERN · Einstieg:** 0-basiert: B an S/5 belegt (5,8). Zeile 8 hat Count 1 und schließt (0,8) bis (4,8) sowie (6,8) bis (8,8) aus.

### S1-05-04-06 · Anschlussbus bereits abgefahren

Status: Der Anschlussbus ist weg. Ihr Umstieg bleibt im Fahrplan möglich.

Ergebnis: Die Durchsage nennt den Anschluss. Diesmal steht er noch am Bahnsteig.

**INTERN · Einstieg:** 0-basiert: B an S/6 belegt (6,9). Zeile 9 hat Count 1; alle anderen Felder dieser untersten Zeile im 10×10-Raster bleiben leer.

### S1-05-04-07 · Ersatzhalt ohne Halt

Status: Der Bus passiert die Ersatzhaltestelle. Das Schild ersetzt nur das Schild.

Ergebnis: Die Zugbegleitung bestätigt: Sie können hier aussteigen. Das Schild ist wieder ein Hinweis, kein Denkmal.

**INTERN · Einstieg:** 0-basiert: Zeilen 8 und 9 haben jeweils Count 0. Die 20 Felder (0,8) bis (9,8) und (0,9) bis (9,9) bleiben leer.

### S1-05-04-08 · Umleitung zurück zum Bahnhof

Status: Der Bus fährt im Kreis. Die Verbindung zum Ausgangspunkt ist gesichert.

Ergebnis: Laut Pressestelle wurde ein weiterer Bahnhof erreicht. Es ist nicht wieder derselbe.

**INTERN · Einstieg:** 0-basiert: Spalten 8 und 9 haben Count 0. Ihre zwanzig Felder bleiben leer; die mögliche Kopfbelegung liegt in Spalten 0 bis 7.

### S1-05-04-09 · Keine Verspätung auf leerer Tafel

Status: Die Tafel meldet keine Verspätung. Busse sind dort nicht eingetragen.

Ergebnis: Auf der Tafel steht die Abfahrt, am Gleis fährt die Bahn. Beide meinen heute dasselbe.

**INTERN · Einstieg:** 0-basiert: Zeilen 0, 1 und 2 haben Count 0. Alle dreißig Felder dieser oberen Zeilen bleiben leer; die mögliche Belegung beginnt in Zeile 3.

### S1-05-04-10 · Ersatzhalt für die Ersatzhaltestelle

Status: Der Ersatzhalt wurde verlegt. Am neuen Standort hängt der Fahrplan von 1998.

Ergebnis: Abfahrt am Gleis. Die drei Ersatzhaltestellen dürfen unter sich bleiben.

**INTERN · Einstieg:** 0-basiert: Spalte 0 hat Count 10 bei Höhe 10 und ist vollständig belegt. Zeilen 0 bis 5 haben Count 1; ihre einzigen Gleisfelder liegen damit in Spalte 0, alle übrigen Felder dieser sechs Zeilen sind leer.

### S1-05-04-11 · Stabiler Betrieb ohne Abfahrt

Status: Der Bus bleibt stehen. Die Lage ist seit Dienstag stabil.

Ergebnis: Beim Anfahren schwappt der Kaffee über. Die Bewegung ist damit amtlich bestätigt.

**INTERN · Einstieg:** 0-basiert: A an S/0 erzwingt (0,9). Die unterste Zeile 9 hat Count 1; (1,9) bis (9,9) sind leer.

### S1-05-04-12 · Ersatzbus durch Warten ersetzt

Status: Der letzte Bus entfällt. Das Warten gilt jetzt als durchgehende Verbindung.

Ergebnis: Durchsage: Die Bahn fährt pünktlich. Bitte leiten Sie daraus keinen Anspruch ab.

**INTERN · Einstieg:** 0-basiert: A an E/0 belegt (9,0). Zeile 0 hat Count 1; (0,0) bis (8,0) bleiben leer. Von (9,0) kann die Strecke wegen dieses Ausschlusses nur nach (9,1) fortsetzen.

## Kampagne, Oberflächen und Varianten

### ui.app

**ui.app.title** · STAMMSTRECKEN-PUZZLE

Einsatz: Konzept 11/12; Einstieg, keine Fahrplanauskunft · title

**ui.app.role** · Unabhängiges satirisches Logikspiel

Einsatz: Konzept 11/12; Einstieg, keine Fahrplanauskunft · role

**ui.app.role.detail** · Ein fiktives Bahnnetz. Echte Logik. Keine Fahrplan-, Ticket- oder Reiseauskunft.

Einsatz: Konzept 11/12; Einstieg, keine Fahrplanauskunft · role.detail

**ui.app.first-start** · Auf der Stammstrecke läuft heute alles nach Plan. Vermutlich.

Einsatz: Konzept 11/12; Einstieg, keine Fahrplanauskunft · firstStart

**ui.app.loading** · Die Betriebslage wird geladen.

Einsatz: Konzept 11/12; Einstieg, keine Fahrplanauskunft · loading

**ui.app.ready** · Ihre nächste Verbindung steht zur Bearbeitung bereit.

Einsatz: Konzept 11/12; Einstieg, keine Fahrplanauskunft · ready

### ui.nav

**ui.nav.operations** · Betriebslage

Einsatz: Konzept 12; außerhalb des aktiven Rätsels · operations

**ui.nav.depot** · Betriebswerk

Einsatz: Konzept 12; außerhalb des aktiven Rätsels · depot

**ui.nav.profile** · Profil

Einsatz: Konzept 12; außerhalb des aktiven Rätsels · profile

**ui.nav.settings** · Einstellungen

Einsatz: Konzept 12; außerhalb des aktiven Rätsels · settings

**ui.nav.back** · Zurück

Einsatz: Konzept 12; außerhalb des aktiven Rätsels · back

**ui.nav.close** · Schließen

Einsatz: Konzept 12; außerhalb des aktiven Rätsels · close

**ui.nav.continue** · WEITER

Einsatz: Konzept 12; außerhalb des aktiven Rätsels · continue

**ui.nav.cancel** · ABBRECHEN

Einsatz: Konzept 12; außerhalb des aktiven Rätsels · cancel

**ui.nav.confirm** · BESTÄTIGEN

Einsatz: Konzept 12; außerhalb des aktiven Rätsels · confirm

**ui.nav.retry** · ERNEUT VERSUCHEN

Einsatz: Konzept 12; außerhalb des aktiven Rätsels · retry

### ui.operations

**ui.operations.title** · BETRIEBSLAGE: BEEINTRÄCHTIGT

Einsatz: Konzept 12; Startscreen und aktuelle Empfehlung · title

**ui.operations.clear** · STRECKE FREIGEBEN

Einsatz: Konzept 12; Startscreen und aktuelle Empfehlung · clear

**ui.operations.show-map** · BETRIEBSLAGE ANZEIGEN

Einsatz: Konzept 12; Startscreen und aktuelle Empfehlung · showMap

**ui.operations.current** · Aktuelle Störung

Einsatz: Konzept 12; Startscreen und aktuelle Empfehlung · current

**ui.operations.next** · Nächste Meldung: {title}

Einsatz: Konzept 12; Startscreen und aktuelle Empfehlung · next

**ui.operations.route** · Route: {route}

Einsatz: Konzept 12; Startscreen und aktuelle Empfehlung · route

**ui.operations.section** · Netzabschnitt: {section}

Einsatz: Konzept 12; Startscreen und aktuelle Empfehlung · section

**ui.operations.progress** · {count} von {total} Meldungen freigegeben

Einsatz: Konzept 12; Startscreen und aktuelle Empfehlung · progress

**ui.operations.quiet** · Eine funktionierende Verbindung ist weiterhin vorgesehen.

Einsatz: Konzept 12; Startscreen und aktuelle Empfehlung · quiet

**ui.operations.season-complete** · Alle 240 Meldungen sind freigegeben. Der Betrieb darf sich kurz daran gewöhnen.

Einsatz: Konzept 12; Startscreen und aktuelle Empfehlung · seasonComplete

**ui.operations.no-recommendation** · Keine weitere Kampagnenmeldung offen. Ihre freigegebenen Strecken finden Sie auf der Karte.

Einsatz: Konzept 12; Startscreen und aktuelle Empfehlung · noRecommendation

### ui.map

**ui.map.title** · BETRIEBSLAGE

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · title

**ui.map.released** · Freigegeben

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · released

**ui.map.current** · Aktuelle Störung

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · current

**ui.map.known** · Bekanntes Folgeproblem

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · known

**ui.map.locked** · Noch nicht erreichbar

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · locked

**ui.map.locked-reason** · Geben Sie zuerst die vorherige Verbindung frei.

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · lockedReason

**ui.map.message** · Meldung {position}

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · message

**ui.map.grid** · Raster: {width} × {height}

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · grid

**ui.map.no-stars** · Bisher ohne Sternwertung

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · noStars

**ui.map.one-star** · 1 Stern · Angekommen

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · oneStar

**ui.map.two-stars** · 2 Sterne · Anschluss erwischt

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · twoStars

**ui.map.three-stars** · 3 Sterne · Tatsächlich pünktlich

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · threeStars

**ui.map.practice** · ÜBUNGSFAHRT STARTEN

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · practice

**ui.map.revision** · BESTZEIT VERBESSERN

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · revision

**ui.map.practice-note** · Übungsfahrt: ohne neue Sterne, offizielle Bestzeit oder Geduldspunkte.

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · practiceNote

**ui.map.revision-note** · Betriebsrevision: Diese Wiederholung wird bewertet.

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · revisionNote

**ui.map.node-label** · {section} · {route} · Meldung {position} · {state}

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · nodeLabel

**ui.map.empty** · Die Betriebslage-Karte ist noch nicht verfügbar.

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · empty

**ui.map.loading** · Die Streckenübersicht wird geladen.

Einsatz: Konzept 12; Statuslabels beschreiben bestehenden Knoten, keine neue Unlockpolicy · loading

### ui.puzzle

**ui.puzzle.context** · {section} · Meldung {position}

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · context

**ui.puzzle.undo** · Rückgängig

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · undo

**ui.puzzle.undo-empty** · Keine Handlung zum Rückgängigmachen vorhanden.

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · undoEmpty

**ui.puzzle.hint** · Hinweis

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · hint

**ui.puzzle.back** · ZUR BETRIEBSLAGE

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · back

**ui.puzzle.resume** · RÄTSEL FORTSETZEN

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · resume

**ui.puzzle.timer** · Zeit

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · timer

**ui.puzzle.timer-value** · Zeit: {time}

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · timerValue

**ui.puzzle.timer-not-started** · Die Zeit beginnt mit Ihrer ersten Eingabe.

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · timerNotStarted

**ui.puzzle.paused** · Rätsel pausiert

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · paused

**ui.puzzle.paused-detail** · Setzen Sie Ihre Verbindung fort, wenn Sie bereit sind.

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · pausedDetail

**ui.puzzle.restoring** · Die letzte gesicherte Eingabe wird wiederhergestellt.

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · restoring

**ui.puzzle.restored** · Gesicherten Rätselstand geladen.

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · restored

**ui.puzzle.input-unavailable** · Eingabe momentan nicht möglich. Bitte versuchen Sie es erneut.

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · inputUnavailable

**ui.puzzle.definition-unavailable** · Diese Meldung kann derzeit nicht geöffnet werden.

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · definitionUnavailable

**ui.puzzle.selected-count** · Ausgewählte Felder: {count}

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · selectedCount

**ui.puzzle.selection-empty** · Wählen Sie zuerst mindestens ein Feld aus.

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · selectionEmpty

**ui.puzzle.apply-selection** · AUF AUSWAHL ANWENDEN

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · applySelection

**ui.puzzle.clear-selection** · AUSWAHL AUFHEBEN

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · clearSelection

**ui.puzzle.applied** · Werkzeug auf die Auswahl angewendet.

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · applied

**ui.puzzle.undone** · Letzte Handlung rückgängig gemacht.

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · undone

**ui.puzzle.unchanged** · Feldinhalt unverändert.

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · unchanged

**ui.puzzle.cleared** · Feldinhalt entfernt.

Einsatz: Konzept 03/12; ruhiger Rätselkopf, keine Statusbewertung plausibler Annahmen · cleared

### ui.tool

**ui.tool.vertical** · Gerade oben–unten

Einsatz: Konzept 03; Werkzeuglabels, Richtungen aus Sicht des Rasters · vertical

**ui.tool.horizontal** · Gerade links–rechts

Einsatz: Konzept 03; Werkzeuglabels, Richtungen aus Sicht des Rasters · horizontal

**ui.tool.north-east** · Kurve oben–rechts

Einsatz: Konzept 03; Werkzeuglabels, Richtungen aus Sicht des Rasters · northEast

**ui.tool.east-south** · Kurve rechts–unten

Einsatz: Konzept 03; Werkzeuglabels, Richtungen aus Sicht des Rasters · eastSouth

**ui.tool.south-west** · Kurve unten–links

Einsatz: Konzept 03; Werkzeuglabels, Richtungen aus Sicht des Rasters · southWest

**ui.tool.west-north** · Kurve links–oben

Einsatz: Konzept 03; Werkzeuglabels, Richtungen aus Sicht des Rasters · westNorth

**ui.tool.empty-mark** · Leerannahme · X

Einsatz: Konzept 03; Werkzeuglabels, Richtungen aus Sicht des Rasters · emptyMark

**ui.tool.occupied-mark** · Belegungsannahme · Form offen

Einsatz: Konzept 03; Werkzeuglabels, Richtungen aus Sicht des Rasters · occupiedMark

**ui.tool.erase** · Leeren

Einsatz: Konzept 03; Werkzeuglabels, Richtungen aus Sicht des Rasters · erase

**ui.tool.multi-select** · Mehrfachauswahl

Einsatz: Konzept 03; Werkzeuglabels, Richtungen aus Sicht des Rasters · multiSelect

**ui.tool.active** · Aktives Werkzeug: {tool}

Einsatz: Konzept 03; Werkzeuglabels, Richtungen aus Sicht des Rasters · active

### ui.help

**ui.help.goal** · Verbinden Sie A und B mit genau einer durchgehenden Strecke.

Einsatz: Konzept 03/06/12; Regelhilfe nur bei Bedarf, kein automatischer Lösungsverrat · goal

**ui.help.counts** · Jede Randzahl nennt die Anzahl der Gleisfelder in ihrer Zeile oder Spalte.

Einsatz: Konzept 03/06/12; Regelhilfe nur bei Bedarf, kein automatischer Lösungsverrat · counts

**ui.help.shapes** · Verwenden Sie Geraden und Kurven. Ihre Anschlüsse müssen zueinander passen.

Einsatz: Konzept 03/06/12; Regelhilfe nur bei Bedarf, kein automatischer Lösungsverrat · shapes

**ui.help.no-loops** · Die Strecke darf sich weder kreuzen noch eine Schleife bilden.

Einsatz: Konzept 03/06/12; Regelhilfe nur bei Bedarf, kein automatischer Lösungsverrat · noLoops

**ui.help.empty-mark** · Das rote X ist Ihre Notiz: Hier soll kein Gleis liegen.

Einsatz: Konzept 03/06/12; Regelhilfe nur bei Bedarf, kein automatischer Lösungsverrat · emptyMark

**ui.help.occupied-mark** · Die graue Markierung ist Ihre Notiz: Hier soll ein Gleis liegen. Die Form bleibt offen.

Einsatz: Konzept 03/06/12; Regelhilfe nur bei Bedarf, kein automatischer Lösungsverrat · occupiedMark

**ui.help.track** · Braune Gleise bilden Ihre konkrete Strecke.

Einsatz: Konzept 03/06/12; Regelhilfe nur bei Bedarf, kein automatischer Lösungsverrat · track

**ui.help.selection** · Gelb zeigt nur Ihre aktuelle Auswahl.

Einsatz: Konzept 03/06/12; Regelhilfe nur bei Bedarf, kein automatischer Lösungsverrat · selection

**ui.help.erase** · Mit Leeren entfernen Sie den gesamten Inhalt eines Feldes.

Einsatz: Konzept 03/06/12; Regelhilfe nur bei Bedarf, kein automatischer Lösungsverrat · erase

**ui.help.multi-select** · Wählen Sie mehrere Felder und wenden Sie ein Werkzeug gemeinsam darauf an.

Einsatz: Konzept 03/06/12; Regelhilfe nur bei Bedarf, kein automatischer Lösungsverrat · multiSelect

**ui.help.undo** · Rückgängig hebt Ihre letzte Handlung auf.

Einsatz: Konzept 03/06/12; Regelhilfe nur bei Bedarf, kein automatischer Lösungsverrat · undo

**ui.help.notes-fair** · X und graue Notizen sind freiwillig. Sie verhindern keine drei Sterne.

Einsatz: Konzept 03/06/12; Regelhilfe nur bei Bedarf, kein automatischer Lösungsverrat · notesFair

**ui.help.hints-fair** · Hinweise verhindern keine Sterne und keinen Kampagnenfortschritt.

Einsatz: Konzept 03/06/12; Regelhilfe nur bei Bedarf, kein automatischer Lösungsverrat · hintsFair

**ui.help.no-penalty** · Sie können Ihre Eingaben korrigieren. Dafür gibt es keine Strafe.

Einsatz: Konzept 03/06/12; Regelhilfe nur bei Bedarf, kein automatischer Lösungsverrat · noPenalty

**ui.help.completion** · Sobald Ihre Verbindung alle Regeln erfüllt, fährt der Zug Ihre Strecke ab.

Einsatz: Konzept 03/06/12; Regelhilfe nur bei Bedarf, kein automatischer Lösungsverrat · completion

### ui.feedback

**ui.feedback.row-count** · Zeile {row}: {actual} von {target} Gleisfeldern

Einsatz: Konzept 03/12; nur objektiv sichtbare Mengen und Regelverstöße, keine Lösungsgültigkeitsvorhersage · rowCount

**ui.feedback.column-count** · Spalte {column}: {actual} von {target} Gleisfeldern

Einsatz: Konzept 03/12; nur objektiv sichtbare Mengen und Regelverstöße, keine Lösungsgültigkeitsvorhersage · columnCount

**ui.feedback.count-met** · Geforderte Anzahl erreicht. Die Gleisformen bleiben zu prüfen.

Einsatz: Konzept 03/12; nur objektiv sichtbare Mengen und Regelverstöße, keine Lösungsgültigkeitsvorhersage · countMet

**ui.feedback.count-exceeded** · Mehr Gleisfelder als vorgegeben.

Einsatz: Konzept 03/12; nur objektiv sichtbare Mengen und Regelverstöße, keine Lösungsgültigkeitsvorhersage · countExceeded

**ui.feedback.count-open** · Die geforderte Anzahl ist noch offen.

Einsatz: Konzept 03/12; nur objektiv sichtbare Mengen und Regelverstöße, keine Lösungsgültigkeitsvorhersage · countOpen

**ui.feedback.outside** · Dieser Anschluss führt über eine unzulässige Außenkante.

Einsatz: Konzept 03/12; nur objektiv sichtbare Mengen und Regelverstöße, keine Lösungsgültigkeitsvorhersage · outside

**ui.feedback.mismatch** · Diese benachbarten Gleisenden passen nicht zusammen.

Einsatz: Konzept 03/12; nur objektiv sichtbare Mengen und Regelverstöße, keine Lösungsgültigkeitsvorhersage · mismatch

**ui.feedback.loose-end** · Hier endet ein konkretes Gleis ohne Anschluss.

Einsatz: Konzept 03/12; nur objektiv sichtbare Mengen und Regelverstöße, keine Lösungsgültigkeitsvorhersage · looseEnd

**ui.feedback.loop** · Diese konkreten Gleise bilden eine geschlossene Schleife.

Einsatz: Konzept 03/12; nur objektiv sichtbare Mengen und Regelverstöße, keine Lösungsgültigkeitsvorhersage · loop

**ui.feedback.disconnected** · Die konkreten Gleise bilden noch keine einzige Verbindung von A nach B.

Einsatz: Konzept 03/12; nur objektiv sichtbare Mengen und Regelverstöße, keine Lösungsgültigkeitsvorhersage · disconnected

**ui.feedback.assumption** · Notiz gesetzt. Ihre Richtigkeit wird damit nicht bewertet.

Einsatz: Konzept 03/12; nur objektiv sichtbare Mengen und Regelverstöße, keine Lösungsgültigkeitsvorhersage · assumption

### ui.hint

**ui.hint.request** · HINWEIS ANFRAGEN

Einsatz: Konzept 06 und BLOCKER-PROD-001; nur aktiv angeforderte Hilfe, keine automatische Ausgabe · request

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, HINT_PRESENTATION_NOT_DECIDED

**ui.hint.scope** · Ein Hinweis erklärt genau einen sicheren nächsten Schluss.

Einsatz: Konzept 06 und BLOCKER-PROD-001; nur aktiv angeforderte Hilfe, keine automatische Ausgabe · scope

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, HINT_PRESENTATION_NOT_DECIDED

**ui.hint.show** · HINWEIS ANZEIGEN

Einsatz: Konzept 06 und BLOCKER-PROD-001; nur aktiv angeforderte Hilfe, keine automatische Ausgabe · show

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, HINT_PRESENTATION_NOT_DECIDED

**ui.hint.cancel** · OHNE HINWEIS WEITER

Einsatz: Konzept 06 und BLOCKER-PROD-001; nur aktiv angeforderte Hilfe, keine automatische Ausgabe · cancel

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, HINT_PRESENTATION_NOT_DECIDED

**ui.hint.unavailable** · Ein Hinweis ist derzeit nicht verfügbar.

Einsatz: Konzept 06 und BLOCKER-PROD-001; nur aktiv angeforderte Hilfe, keine automatische Ausgabe · unavailable

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, HINT_PRESENTATION_NOT_DECIDED

**ui.hint.first-free** · Für dieses neue Rätsel steht der erste Hinweis kostenlos bereit.

Einsatz: Konzept 06 und BLOCKER-PROD-001; nur aktiv angeforderte Hilfe, keine automatische Ausgabe · firstFree

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, HINT_PRESENTATION_NOT_DECIDED

**ui.hint.video-offer** · Freiwilliges Video ansehen und einen zusätzlichen Logikhinweis erhalten.

Einsatz: Konzept 06 und BLOCKER-PROD-001; nur aktiv angeforderte Hilfe, keine automatische Ausgabe · videoOffer

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, HINT_PRESENTATION_NOT_DECIDED

**ui.hint.video-action** · VIDEO FÜR EINEN HINWEIS ANSEHEN

Einsatz: Konzept 06 und BLOCKER-PROD-001; nur aktiv angeforderte Hilfe, keine automatische Ausgabe · videoAction

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, HINT_PRESENTATION_NOT_DECIDED

**ui.hint.regular-access** · Regulär verfügbare Hilfe nutzen

Einsatz: Konzept 06 und BLOCKER-PROD-001; nur aktiv angeforderte Hilfe, keine automatische Ausgabe · regularAccess

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, HINT_PRESENTATION_NOT_DECIDED

**ui.hint.no-safe-step** · Für diesen Stand liegt kein geprüfter nächster Hinweis vor.

Einsatz: Konzept 06 und BLOCKER-PROD-001; nur aktiv angeforderte Hilfe, keine automatische Ausgabe · noSafeStep

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, HINT_PRESENTATION_NOT_DECIDED

**ui.hint.template.row-empty** · Zeile {row} verlangt kein Gleis. Ihre Felder bleiben leer.

Einsatz: Nur bei verifiziertem Hintpayload, Zeilen-/Spaltennummern für Spieler 1-basiert. Keine Verbindung zu .entry-Schlüsseln. · rowEmpty

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, VERIFIED_HINT_PAYLOAD_REQUIRED

**ui.hint.template.column-empty** · Spalte {column} verlangt kein Gleis. Ihre Felder bleiben leer.

Einsatz: Nur bei verifiziertem Hintpayload, Zeilen-/Spaltennummern für Spieler 1-basiert. Keine Verbindung zu .entry-Schlüsseln. · columnEmpty

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, VERIFIED_HINT_PAYLOAD_REQUIRED

**ui.hint.template.row-full** · In Zeile {row} werden alle noch verfügbaren Felder als Gleisfelder benötigt.

Einsatz: Nur bei verifiziertem Hintpayload, Zeilen-/Spaltennummern für Spieler 1-basiert. Keine Verbindung zu .entry-Schlüsseln. · rowFull

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, VERIFIED_HINT_PAYLOAD_REQUIRED

**ui.hint.template.column-full** · In Spalte {column} werden alle noch verfügbaren Felder als Gleisfelder benötigt.

Einsatz: Nur bei verifiziertem Hintpayload, Zeilen-/Spaltennummern für Spieler 1-basiert. Keine Verbindung zu .entry-Schlüsseln. · columnFull

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, VERIFIED_HINT_PAYLOAD_REQUIRED

**ui.hint.template.row-met** · Die geforderte Belegung in Zeile {row} steht fest. Die übrigen Felder bleiben leer.

Einsatz: Nur bei verifiziertem Hintpayload, Zeilen-/Spaltennummern für Spieler 1-basiert. Keine Verbindung zu .entry-Schlüsseln. · rowMet

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, VERIFIED_HINT_PAYLOAD_REQUIRED

**ui.hint.template.column-met** · Die geforderte Belegung in Spalte {column} steht fest. Die übrigen Felder bleiben leer.

Einsatz: Nur bei verifiziertem Hintpayload, Zeilen-/Spaltennummern für Spieler 1-basiert. Keine Verbindung zu .entry-Schlüsseln. · columnMet

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, VERIFIED_HINT_PAYLOAD_REQUIRED

**ui.hint.template.endpoint** · Das Feld direkt am Anschluss {endpoint} muss ein passendes Gleis enthalten.

Einsatz: Nur bei verifiziertem Hintpayload, Zeilen-/Spaltennummern für Spieler 1-basiert. Keine Verbindung zu .entry-Schlüsseln. · endpoint

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, VERIFIED_HINT_PAYLOAD_REQUIRED

**ui.hint.template.corner** · Dieses belegte Eckfeld hat nur zwei zulässige Anschlüsse. Hier ist die passende Kurve nötig.

Einsatz: Nur bei verifiziertem Hintpayload, Zeilen-/Spaltennummern für Spieler 1-basiert. Keine Verbindung zu .entry-Schlüsseln. · corner

**GESPERRTER ENTWURF:** BLOCKER-PROD-001, VERIFIED_HINT_PAYLOAD_REQUIRED

### ui.ride

**ui.ride.start** · Die Verbindung steht. Der Zug fährt.

Einsatz: Konzept 05/12; nur nach persistierter gültiger Lösung, ohne Werbung/Angebote · start

**ui.ride.running** · Ihre Strecke wird befahren.

Einsatz: Konzept 05/12; nur nach persistierter gültiger Lösung, ohne Werbung/Angebote · running

**ui.ride.arrived** · Zug angekommen

Einsatz: Konzept 05/12; nur nach persistierter gültiger Lösung, ohne Werbung/Angebote · arrived

**ui.ride.loading** · Die Fahrt wird vorbereitet.

Einsatz: Konzept 05/12; nur nach persistierter gültiger Lösung, ohne Werbung/Angebote · loading

**ui.ride.resume** · Die Abschlussfahrt wird fortgesetzt.

Einsatz: Konzept 05/12; nur nach persistierter gültiger Lösung, ohne Werbung/Angebote · resume

**ui.ride.unavailable** · Die Fahrt konnte nicht dargestellt werden.

Einsatz: Konzept 05/12; nur nach persistierter gültiger Lösung, ohne Werbung/Angebote · unavailable

**ui.ride.recorded** · Der Abschluss ist gespeichert. Das Ergebnis steht bereit.

Einsatz: Konzept 05/12; nur nach persistierter gültiger Lösung, ohne Werbung/Angebote · recorded

### ui.results

**ui.results.title** · STRECKE FREIGEGEBEN

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · title

**ui.results.next** · NÄCHSTE STÖRUNG

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · next

**ui.results.map** · ZUR BETRIEBSLAGE

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · map

**ui.results.time** · Lösungszeit: {time}

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · time

**ui.results.best-time** · Bestzeit: {bestTime}

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · bestTime

**ui.results.new-best** · Neue Bestzeit

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · newBest

**ui.results.points** · Geduldspunkte: +{points}

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · points

**ui.results.route-bonus** · Routenabschluss: +{points} Geduldspunkte

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · routeBonus

**ui.results.section-bonus** · Netzabschnitt abgeschlossen: +{points} Geduldspunkte

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · sectionBonus

**ui.results.stars-missing** · Eine Sternwertung ist derzeit nicht verfügbar.

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · starsMissing

**ui.results.practice** · Übungsfahrt beendet

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · practice

**ui.results.practice-detail** · Übung abgeschlossen. Sterne, offizielle Bestzeit und Geduldspunkte bleiben unverändert.

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · practiceDetail

**ui.results.revision** · Betriebsrevision beendet

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · revision

**ui.results.revision-unchanged** · Ihre bisherige Wertung bleibt bestehen.

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · revisionUnchanged

**ui.results.star-bonus** · Neu erreichter Sternbonus: +{points} Geduldspunkte

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · starBonus

**ui.results.end-of-season** · SEASONABSCHLUSS ANZEIGEN

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · endOfSeason

**ui.results.saving** · Ihr Abschluss wird gesichert.

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · saving

**ui.results.save-pending** · Der Abschluss ist noch nicht bestätigt. Bitte warten Sie auf die Sicherung.

Einsatz: Konzept 05/06/12/13; nach Zugfahrt und bestätigtem Ergebnis · savePending

### ui.star

**ui.star.one** · Angekommen

Einsatz: Bestätigte feste Bezeichnungen Konzept 06/13, keine Bonusberechnung im Text · one

**ui.star.two** · Anschluss erwischt

Einsatz: Bestätigte feste Bezeichnungen Konzept 06/13, keine Bonusberechnung im Text · two

**ui.star.three** · Tatsächlich pünktlich

Einsatz: Bestätigte feste Bezeichnungen Konzept 06/13, keine Bonusberechnung im Text · three

**ui.star.one.detail** · Die Verbindung ist korrekt. Ihr Netz kommt weiter.

Einsatz: Bestätigte feste Bezeichnungen Konzept 06/13, keine Bonusberechnung im Text · one.detail

**ui.star.two.detail** · Sie haben das Zeit-Ziel für zwei Sterne erreicht.

Einsatz: Bestätigte feste Bezeichnungen Konzept 06/13, keine Bonusberechnung im Text · two.detail

**ui.star.three.detail** · Sie haben das Zeit-Ziel für drei Sterne erreicht.

Einsatz: Bestätigte feste Bezeichnungen Konzept 06/13, keine Bonusberechnung im Text · three.detail

**ui.star.target-unavailable** · Die Zeitziele sind noch nicht freigegeben.

Einsatz: Bestätigte feste Bezeichnungen Konzept 06/13, keine Bonusberechnung im Text · targetUnavailable

**ui.star.notes** · Notizen und Hinweise verhindern keine Sterne.

Einsatz: Bestätigte feste Bezeichnungen Konzept 06/13, keine Bonusberechnung im Text · notes

### ui.points

**ui.points.name** · Geduldspunkte

Einsatz: Konzept 06/13; Werte werden ausschließlich aus bestätigtem Reward-/Ledgersnapshot geliefert · name

**ui.points.balance** · Geduldspunkte: {points}

Einsatz: Konzept 06/13; Werte werden ausschließlich aus bestätigtem Reward-/Ledgersnapshot geliefert · balance

**ui.points.use** · Für kosmetische Züge, Lackierungen und Betriebsobjekte.

Einsatz: Konzept 06/13; Werte werden ausschließlich aus bestätigtem Reward-/Ledgersnapshot geliefert · use

**ui.points.insufficient** · Noch benötigte Geduldspunkte: {points}.

Einsatz: Konzept 06/13; Werte werden ausschließlich aus bestätigtem Reward-/Ledgersnapshot geliefert · insufficient

**ui.points.recording** · Die Gutschrift wird geprüft.

Einsatz: Konzept 06/13; Werte werden ausschließlich aus bestätigtem Reward-/Ledgersnapshot geliefert · recording

**ui.points.recorded** · Geduldspunkte gutgeschrieben: {points}.

Einsatz: Konzept 06/13; Werte werden ausschließlich aus bestätigtem Reward-/Ledgersnapshot geliefert · recorded

**ui.points.already-recorded** · Diese Gutschrift wurde bereits berücksichtigt.

Einsatz: Konzept 06/13; Werte werden ausschließlich aus bestätigtem Reward-/Ledgersnapshot geliefert · alreadyRecorded

**ui.points.pending** · Die Gutschrift ist noch nicht bestätigt.

Einsatz: Konzept 06/13; Werte werden ausschließlich aus bestätigtem Reward-/Ledgersnapshot geliefert · pending

### ui.depot

**ui.depot.title** · BETRIEBSWERK

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · title

**ui.depot.trains** · ZUGPARK

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · trains

**ui.depot.objects** · BETRIEBSVERBESSERUNGEN

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · objects

**ui.depot.status** · Der Fuhrpark wird nach tatsächlicher Verfügbarkeit geführt.

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · status

**ui.depot.cosmetic** · Alle Betriebsmittel sind rein kosmetisch.

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · cosmetic

**ui.depot.selected** · Ausgewählt

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · selected

**ui.depot.select** · AUSWÄHLEN

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · select

**ui.depot.owned** · In Ihrem Bestand

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · owned

**ui.depot.already-owned** · Dieses Betriebsmittel gehört bereits Ihnen.

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · alreadyOwned

**ui.depot.purchase** · FÜR {pricePoints} GEDULDSPUNKTE FREISCHALTEN

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · purchase

**ui.depot.item** · {item}

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · item

**ui.depot.description** · {description}

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · description

**ui.depot.milestone** · Voraussetzung: {requirement}

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · milestone

**ui.depot.claim** · BETRIEBSMITTEL ABHOLEN

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · claim

**ui.depot.claim-not-ready** · Die Voraussetzung für dieses Betriebsmittel ist noch nicht erfüllt.

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · claimNotReady

**ui.depot.purchase-confirmed** · {item} ist jetzt in Ihrem Bestand.

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · purchaseConfirmed

**ui.depot.selection-confirmed** · {item} ausgewählt.

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · selectionConfirmed

**ui.depot.catalog-changed** · Das Angebot hat sich geändert. Bitte öffnen Sie es erneut.

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · catalogChanged

**ui.depot.not-available** · Dieses Betriebsmittel ist derzeit nicht verfügbar.

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · notAvailable

**ui.depot.empty** · Hier sind noch keine verfügbaren Betriebsmittel eingetragen.

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · empty

**ui.depot.loading** · Der Bestand wird geladen.

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · loading

**ui.depot.purchase-pending** · Die Freischaltung wird gesichert.

Einsatz: Konzept 07/12; konkrete Itemnamen/Preise/Freischaltungen bleiben fremder Katalogscope · purchasePending

### ui.object-category

**ui.object-category.train** · Zugvarianten

Einsatz: Konzept 07; allgemeine Kategorien, keine finalen Katalogitems · train

**ui.object-category.paint** · Lackierungen

Einsatz: Konzept 07; allgemeine Kategorien, keine finalen Katalogitems · paint

**ui.object-category.platform** · Bahnsteigobjekte

Einsatz: Konzept 07; allgemeine Kategorien, keine finalen Katalogitems · platform

**ui.object-category.trackside** · Streckenobjekte

Einsatz: Konzept 07; allgemeine Kategorien, keine finalen Katalogitems · trackside

**ui.object-category.replacement** · Ersatzobjekte

Einsatz: Konzept 07; allgemeine Kategorien, keine finalen Katalogitems · replacement

**ui.object-category.train.description** · Ein anderer Zug für dieselbe selbst gebaute Strecke.

Einsatz: Konzept 07; allgemeine Kategorien, keine finalen Katalogitems · train.description

**ui.object-category.paint.description** · Eine andere Lackierung. Der Anschluss bleibt Ihre Aufgabe.

Einsatz: Konzept 07; allgemeine Kategorien, keine finalen Katalogitems · paint.description

**ui.object-category.platform.description** · Mehr Ausstattung am Bahnsteig. Gleiche Regeln im Raster.

Einsatz: Konzept 07; allgemeine Kategorien, keine finalen Katalogitems · platform.description

**ui.object-category.trackside.description** · Für den sichtbaren Streckenrand. Ohne Eingriff in Ihre Logik.

Einsatz: Konzept 07; allgemeine Kategorien, keine finalen Katalogitems · trackside.description

**ui.object-category.replacement.description** · Für die geordnete Darstellung des Provisoriums.

Einsatz: Konzept 07; allgemeine Kategorien, keine finalen Katalogitems · replacement.description

### ui.profile

**ui.profile.title** · PROFIL

Einsatz: Konzept 06/12; Werte nur aus tatsächlich erfassten Profilrecords · title

**ui.profile.progress** · Ihr Netzfortschritt

Einsatz: Konzept 06/12; Werte nur aus tatsächlich erfassten Profilrecords · progress

**ui.profile.stars** · Gesammelte Sterne

Einsatz: Konzept 06/12; Werte nur aus tatsächlich erfassten Profilrecords · stars

**ui.profile.best-times** · Persönliche Bestzeiten

Einsatz: Konzept 06/12; Werte nur aus tatsächlich erfassten Profilrecords · bestTimes

**ui.profile.mastery** · Meisterschaft

Einsatz: Konzept 06/12; Werte nur aus tatsächlich erfassten Profilrecords · mastery

**ui.profile.direct** · Direkt gelöst

Einsatz: Konzept 06/12; Werte nur aus tatsächlich erfassten Profilrecords · direct

**ui.profile.direct.detail** · Ohne X, graue Belegungsmarkierung und Hinweis gelöst.

Einsatz: Konzept 06/12; Werte nur aus tatsächlich erfassten Profilrecords · direct.detail

**ui.profile.styles-equal** · Planend, gemischt oder direkt: Jeder gültige Lösungsweg zählt.

Einsatz: Konzept 06/12; Werte nur aus tatsächlich erfassten Profilrecords · stylesEqual

**ui.profile.direct-optional** · Direkte Lösungen sind ein Profilmerkmal. Sie sind keine Voraussetzung für Sterne oder Fortschritt.

Einsatz: Konzept 06/12; Werte nur aus tatsächlich erfassten Profilrecords · directOptional

**ui.profile.no-records** · Noch keine abgeschlossene Meldung eingetragen.

Einsatz: Konzept 06/12; Werte nur aus tatsächlich erfassten Profilrecords · noRecords

**ui.profile.no-best** · Noch keine offizielle Bestzeit vorhanden.

Einsatz: Konzept 06/12; Werte nur aus tatsächlich erfassten Profilrecords · noBest

**ui.profile.no-direct** · Noch keine direkte Lösung eingetragen.

Einsatz: Konzept 06/12; Werte nur aus tatsächlich erfassten Profilrecords · noDirect

**ui.profile.section-count** · {count} von {total} Netzabschnitten abgeschlossen

Einsatz: Konzept 06/12; Werte nur aus tatsächlich erfassten Profilrecords · sectionCount

**ui.profile.route-count** · {count} von {total} Routen abgeschlossen

Einsatz: Konzept 06/12; Werte nur aus tatsächlich erfassten Profilrecords · routeCount

**ui.profile.level-count** · {count} von {total} Meldungen abgeschlossen

Einsatz: Konzept 06/12; Werte nur aus tatsächlich erfassten Profilrecords · levelCount

**ui.profile.loading** · Ihre Leistungsübersicht wird geladen.

Einsatz: Konzept 06/12; Werte nur aus tatsächlich erfassten Profilrecords · loading

**ui.profile.pending.clean** · Saubere Ausführung

Einsatz: Konzept 06: konkrete Ausführungs-/Routenmeisterschaftsdarstellung offen, keine berechnete Leistung behaupten · clean

**GESPERRTER ENTWURF:** MASTERY_DETAIL_NOT_DECIDED

**ui.profile.pending.route-mastery** · Routenmeisterschaft

Einsatz: Konzept 06: konkrete Ausführungs-/Routenmeisterschaftsdarstellung offen, keine berechnete Leistung behaupten · routeMastery

**GESPERRTER ENTWURF:** MASTERY_DETAIL_NOT_DECIDED

**ui.profile.pending.not-defined** · Die genaue Auswertung ist noch nicht festgelegt.

Einsatz: Konzept 06: konkrete Ausführungs-/Routenmeisterschaftsdarstellung offen, keine berechnete Leistung behaupten · notDefined

**GESPERRTER ENTWURF:** MASTERY_DETAIL_NOT_DECIDED

### ui.revision

**ui.revision.title** · BETRIEBSREVISION

Einsatz: Konzept 06/13; nur nach vollständigem Seasonabschluss, bewertete Wiederholung · title

**ui.revision.intro** · Das Netz steht. Jetzt dürfen die Zeiten besser werden.

Einsatz: Konzept 06/13; nur nach vollständigem Seasonabschluss, bewertete Wiederholung · intro

**ui.revision.locked** · Nach allen 240 Erstabschlüssen von Season 1 verfügbar.

Einsatz: Konzept 06/13; nur nach vollständigem Seasonabschluss, bewertete Wiederholung · locked

**ui.revision.start** · BEWERTETE WIEDERHOLUNG STARTEN

Einsatz: Konzept 06/13; nur nach vollständigem Seasonabschluss, bewertete Wiederholung · start

**ui.revision.rule** · Verbessern Sie Ihre Wertung. Fehlende Sternboni werden jeweils einmal nachgezahlt.

Einsatz: Konzept 06/13; nur nach vollständigem Seasonabschluss, bewertete Wiederholung · rule

**ui.revision.safe** · Bereits erreichte Sterne und Geduldspunkte bleiben erhalten.

Einsatz: Konzept 06/13; nur nach vollständigem Seasonabschluss, bewertete Wiederholung · safe

**ui.revision.practice** · Vor Seasonabschluss können Sie gelöste Meldungen als Übungsfahrt wiederholen.

Einsatz: Konzept 06/13; nur nach vollständigem Seasonabschluss, bewertete Wiederholung · practice

### ui.endless

**ui.endless.title** · DAUERBAUSTELLE

Einsatz: Konzept 04/06; neue unbekannte Rätsel, Freigabe von Qualitätsprofil abhängig · title

**GESPERRTER ENTWURF:** BLOCKER-PROD-003

**ui.endless.intro** · Neue Meldungen. Bewährte Zuständigkeit.

Einsatz: Konzept 04/06; neue unbekannte Rätsel, Freigabe von Qualitätsprofil abhängig · intro

**GESPERRTER ENTWURF:** BLOCKER-PROD-003

**ui.endless.locked** · Nach allen 240 Erstabschlüssen von Season 1 vorgesehen.

Einsatz: Konzept 04/06; neue unbekannte Rätsel, Freigabe von Qualitätsprofil abhängig · locked

**GESPERRTER ENTWURF:** BLOCKER-PROD-003

**ui.endless.difficulty** · Schwierigkeit wählen

Einsatz: Konzept 04/06; neue unbekannte Rätsel, Freigabe von Qualitätsprofil abhängig · difficulty

**GESPERRTER ENTWURF:** BLOCKER-PROD-003

**ui.endless.start** · NEUE MELDUNG ÖFFNEN

Einsatz: Konzept 04/06; neue unbekannte Rätsel, Freigabe von Qualitätsprofil abhängig · start

**GESPERRTER ENTWURF:** BLOCKER-PROD-003

**ui.endless.unavailable** · Derzeit steht keine freigegebene neue Meldung bereit.

Einsatz: Konzept 04/06; neue unbekannte Rätsel, Freigabe von Qualitätsprofil abhängig · unavailable

**GESPERRTER ENTWURF:** BLOCKER-PROD-003

**ui.endless.preparing** · Eine neue Meldung wird geprüft.

Einsatz: Konzept 04/06; neue unbekannte Rätsel, Freigabe von Qualitätsprofil abhängig · preparing

**GESPERRTER ENTWURF:** BLOCKER-PROD-003

**ui.endless.failed** · Diese neue Meldung konnte nicht freigegeben werden. Bitte versuchen Sie es später erneut.

Einsatz: Konzept 04/06; neue unbekannte Rätsel, Freigabe von Qualitätsprofil abhängig · failed

**GESPERRTER ENTWURF:** BLOCKER-PROD-003

### ui.daily

**ui.daily.title** · BETRIEBSLAGE DES TAGES

Einsatz: Konzept 08/12/13, Tagesanspruch bis Kalendertag-/Offlineentscheidung gesperrt · title

**GESPERRTER ENTWURF:** BLOCKER-PROD-002, DAILY_REWARD_CATALOG_REQUIRED

**ui.daily.intro** · Die Meldetafel hat eine zusätzliche Mitteilung.

Einsatz: Konzept 08/12/13, Tagesanspruch bis Kalendertag-/Offlineentscheidung gesperrt · intro

**GESPERRTER ENTWURF:** BLOCKER-PROD-002, DAILY_REWARD_CATALOG_REQUIRED

**ui.daily.free** · KOSTENLOSE MELDUNG ABRUFEN

Einsatz: Konzept 08/12/13, Tagesanspruch bis Kalendertag-/Offlineentscheidung gesperrt · free

**GESPERRTER ENTWURF:** BLOCKER-PROD-002, DAILY_REWARD_CATALOG_REQUIRED

**ui.daily.free-detail** · Eine kostenlose Teilnahme pro Kalendertag.

Einsatz: Konzept 08/12/13, Tagesanspruch bis Kalendertag-/Offlineentscheidung gesperrt · freeDetail

**GESPERRTER ENTWURF:** BLOCKER-PROD-002, DAILY_REWARD_CATALOG_REQUIRED

**ui.daily.optional** · Freiwilliges Extra außerhalb des Rätsels. Auslassen kostet Sie nichts.

Einsatz: Konzept 08/12/13, Tagesanspruch bis Kalendertag-/Offlineentscheidung gesperrt · optional

**GESPERRTER ENTWURF:** BLOCKER-PROD-002, DAILY_REWARD_CATALOG_REQUIRED

**ui.daily.video** · VIDEO FÜR EINE WEITERE TEILNAHME ANSEHEN

Einsatz: Konzept 08/12/13, Tagesanspruch bis Kalendertag-/Offlineentscheidung gesperrt · video

**GESPERRTER ENTWURF:** BLOCKER-PROD-002, DAILY_REWARD_CATALOG_REQUIRED

**ui.daily.video-detail** · Nach der kostenlosen Teilnahme ist höchstens eine weitere Teilnahme per freiwilligem Video möglich.

Einsatz: Konzept 08/12/13, Tagesanspruch bis Kalendertag-/Offlineentscheidung gesperrt · videoDetail

**GESPERRTER ENTWURF:** BLOCKER-PROD-002, DAILY_REWARD_CATALOG_REQUIRED

**ui.daily.used** · Die verfügbaren Teilnahmen für diesen Kalendertag sind aufgebraucht.

Einsatz: Konzept 08/12/13, Tagesanspruch bis Kalendertag-/Offlineentscheidung gesperrt · used

**GESPERRTER ENTWURF:** BLOCKER-PROD-002, DAILY_REWARD_CATALOG_REQUIRED

**ui.daily.checking** · Die Teilnahmeberechtigung wird geprüft.

Einsatz: Konzept 08/12/13, Tagesanspruch bis Kalendertag-/Offlineentscheidung gesperrt · checking

**GESPERRTER ENTWURF:** BLOCKER-PROD-002, DAILY_REWARD_CATALOG_REQUIRED

**ui.daily.unavailable** · Die Tagesmeldung ist derzeit nicht verfügbar.

Einsatz: Konzept 08/12/13, Tagesanspruch bis Kalendertag-/Offlineentscheidung gesperrt · unavailable

**GESPERRTER ENTWURF:** BLOCKER-PROD-002, DAILY_REWARD_CATALOG_REQUIRED

**ui.daily.points** · Tagesmeldung verbucht: +{points} Geduldspunkte

Einsatz: Konzept 08/12/13, Tagesanspruch bis Kalendertag-/Offlineentscheidung gesperrt · points

**GESPERRTER ENTWURF:** BLOCKER-PROD-002, DAILY_REWARD_CATALOG_REQUIRED

**ui.daily.cosmetic** · Kosmetischer Fortschritt: {progress}

Einsatz: Konzept 08/12/13, Tagesanspruch bis Kalendertag-/Offlineentscheidung gesperrt · cosmetic

**GESPERRTER ENTWURF:** BLOCKER-PROD-002, DAILY_REWARD_CATALOG_REQUIRED

**ui.daily.pending** · Die Tagesgutschrift ist noch nicht bestätigt.

Einsatz: Konzept 08/12/13, Tagesanspruch bis Kalendertag-/Offlineentscheidung gesperrt · pending

**GESPERRTER ENTWURF:** BLOCKER-PROD-002, DAILY_REWARD_CATALOG_REQUIRED

**ui.daily.done** · Tagesmeldung abgeschlossen.

Einsatz: Konzept 08/12/13, Tagesanspruch bis Kalendertag-/Offlineentscheidung gesperrt · done

**GESPERRTER ENTWURF:** BLOCKER-PROD-002, DAILY_REWARD_CATALOG_REQUIRED

### ui.ad

**ui.ad.label** · Freiwilliges Werbevideo

Einsatz: Konzept 08/13; freiwilliger Punktbonus nur nach erster Lösung und vollständigem Ergebnis · label

**ui.ad.bonus** · Zusätzliche Geduld beantragen

Einsatz: Konzept 08/13; freiwilliger Punktbonus nur nach erster Lösung und vollständigem Ergebnis · bonus

**ui.ad.bonus-detail** · Freiwilliges Video ansehen: +10 Geduldspunkte. Einmal je Meldung.

Einsatz: Konzept 08/13; freiwilliger Punktbonus nur nach erster Lösung und vollständigem Ergebnis · bonusDetail

**ui.ad.bonus-action** · VIDEO FÜR +10 GEDULDSPUNKTE ANSEHEN

Einsatz: Konzept 08/13; freiwilliger Punktbonus nur nach erster Lösung und vollständigem Ergebnis · bonusAction

**ui.ad.skip** · OHNE VIDEO WEITER

Einsatz: Konzept 08/13; freiwilliger Punktbonus nur nach erster Lösung und vollständigem Ergebnis · skip

**ui.ad.loading** · Das Werbevideo wird geladen.

Einsatz: Konzept 08/13; freiwilliger Punktbonus nur nach erster Lösung und vollständigem Ergebnis · loading

**ui.ad.unavailable** · Derzeit ist kein Video verfügbar.

Einsatz: Konzept 08/13; freiwilliger Punktbonus nur nach erster Lösung und vollständigem Ergebnis · unavailable

**ui.ad.declined** · Ohne Video fortfahren.

Einsatz: Konzept 08/13; freiwilliger Punktbonus nur nach erster Lösung und vollständigem Ergebnis · declined

**ui.ad.interrupted** · Video unterbrochen. Eine Belohnung ist noch nicht bestätigt.

Einsatz: Konzept 08/13; freiwilliger Punktbonus nur nach erster Lösung und vollständigem Ergebnis · interrupted

**ui.ad.verifying** · Der Videoabschluss wird geprüft.

Einsatz: Konzept 08/13; freiwilliger Punktbonus nur nach erster Lösung und vollständigem Ergebnis · verifying

**ui.ad.reward-pending** · Die Videobelohnung ist noch nicht bestätigt.

Einsatz: Konzept 08/13; freiwilliger Punktbonus nur nach erster Lösung und vollständigem Ergebnis · rewardPending

**ui.ad.reward-confirmed** · Videobelohnung bestätigt: +10 Geduldspunkte.

Einsatz: Konzept 08/13; freiwilliger Punktbonus nur nach erster Lösung und vollständigem Ergebnis · rewardConfirmed

**ui.ad.already-claimed** · Der Videobonus für diese Meldung wurde bereits berücksichtigt.

Einsatz: Konzept 08/13; freiwilliger Punktbonus nur nach erster Lösung und vollständigem Ergebnis · alreadyClaimed

**ui.ad.return** · Unterbrechung beendet. Sie können fortfahren.

Einsatz: Konzept 08/13; freiwilliger Punktbonus nur nach erster Lösung und vollständigem Ergebnis · return

**ui.ad.no-penalty** · Ein ausgelassenes Video ändert nichts an Sternen oder Kampagnenfortschritt.

Einsatz: Konzept 08/13; freiwilliger Punktbonus nur nach erster Lösung und vollständigem Ergebnis · noPenalty

### ui.store

**ui.store.title** · OHNE UNTERBRECHENDE WERBUNG

Einsatz: Konzept 08; dauerhafte Entfernung unterbrechender Anzeigen, Preis/Store-/Privacyflows noch offen · title

**GESPERRTER ENTWURF:** AD_FREE_PRODUCT_NOT_FINAL, STORE_PRIVACY_FLOW_REQUIRED

**ui.store.description** · Entfernt unterbrechende Anzeigen dauerhaft. Freiwillige Videos bleiben eine optionale Wahl.

Einsatz: Konzept 08; dauerhafte Entfernung unterbrechender Anzeigen, Preis/Store-/Privacyflows noch offen · description

**GESPERRTER ENTWURF:** AD_FREE_PRODUCT_NOT_FINAL, STORE_PRIVACY_FLOW_REQUIRED

**ui.store.buy** · FÜR {price} KAUFEN

Einsatz: Konzept 08; dauerhafte Entfernung unterbrechender Anzeigen, Preis/Store-/Privacyflows noch offen · buy

**GESPERRTER ENTWURF:** AD_FREE_PRODUCT_NOT_FINAL, STORE_PRIVACY_FLOW_REQUIRED

**ui.store.restore** · KÄUFE WIEDERHERSTELLEN

Einsatz: Konzept 08; dauerhafte Entfernung unterbrechender Anzeigen, Preis/Store-/Privacyflows noch offen · restore

**GESPERRTER ENTWURF:** AD_FREE_PRODUCT_NOT_FINAL, STORE_PRIVACY_FLOW_REQUIRED

**ui.store.active** · Unterbrechende Anzeigen deaktiviert

Einsatz: Konzept 08; dauerhafte Entfernung unterbrechender Anzeigen, Preis/Store-/Privacyflows noch offen · active

**GESPERRTER ENTWURF:** AD_FREE_PRODUCT_NOT_FINAL, STORE_PRIVACY_FLOW_REQUIRED

**ui.store.pending** · Der Kauf wird geprüft.

Einsatz: Konzept 08; dauerhafte Entfernung unterbrechender Anzeigen, Preis/Store-/Privacyflows noch offen · pending

**GESPERRTER ENTWURF:** AD_FREE_PRODUCT_NOT_FINAL, STORE_PRIVACY_FLOW_REQUIRED

**ui.store.confirmed** · Der Kauf ist bestätigt. Unterbrechende Anzeigen sind deaktiviert.

Einsatz: Konzept 08; dauerhafte Entfernung unterbrechender Anzeigen, Preis/Store-/Privacyflows noch offen · confirmed

**GESPERRTER ENTWURF:** AD_FREE_PRODUCT_NOT_FINAL, STORE_PRIVACY_FLOW_REQUIRED

**ui.store.cancelled** · Kauf abgebrochen.

Einsatz: Konzept 08; dauerhafte Entfernung unterbrechender Anzeigen, Preis/Store-/Privacyflows noch offen · cancelled

**GESPERRTER ENTWURF:** AD_FREE_PRODUCT_NOT_FINAL, STORE_PRIVACY_FLOW_REQUIRED

**ui.store.failed** · Der Kauf konnte nicht bestätigt werden.

Einsatz: Konzept 08; dauerhafte Entfernung unterbrechender Anzeigen, Preis/Store-/Privacyflows noch offen · failed

**GESPERRTER ENTWURF:** AD_FREE_PRODUCT_NOT_FINAL, STORE_PRIVACY_FLOW_REQUIRED

**ui.store.restoring** · Vorhandene Käufe werden geprüft.

Einsatz: Konzept 08; dauerhafte Entfernung unterbrechender Anzeigen, Preis/Store-/Privacyflows noch offen · restoring

**GESPERRTER ENTWURF:** AD_FREE_PRODUCT_NOT_FINAL, STORE_PRIVACY_FLOW_REQUIRED

**ui.store.restored** · Der vorhandene Kauf wurde wiederhergestellt.

Einsatz: Konzept 08; dauerhafte Entfernung unterbrechender Anzeigen, Preis/Store-/Privacyflows noch offen · restored

**GESPERRTER ENTWURF:** AD_FREE_PRODUCT_NOT_FINAL, STORE_PRIVACY_FLOW_REQUIRED

**ui.store.none** · Kein wiederherstellbarer Kauf gefunden.

Einsatz: Konzept 08; dauerhafte Entfernung unterbrechender Anzeigen, Preis/Store-/Privacyflows noch offen · none

**GESPERRTER ENTWURF:** AD_FREE_PRODUCT_NOT_FINAL, STORE_PRIVACY_FLOW_REQUIRED

**ui.store.unavailable** · Das Store-Angebot ist derzeit nicht verfügbar.

Einsatz: Konzept 08; dauerhafte Entfernung unterbrechender Anzeigen, Preis/Store-/Privacyflows noch offen · unavailable

**GESPERRTER ENTWURF:** AD_FREE_PRODUCT_NOT_FINAL, STORE_PRIVACY_FLOW_REQUIRED

### ui.settings

**ui.settings.title** · EINSTELLUNGEN

Einsatz: ADR011; Labels für bestätigte lokale Einstellungen, Gesten/Accessibilitydetails bleiben offen · title

**ui.settings.music** · Musik

Einsatz: ADR011; Labels für bestätigte lokale Einstellungen, Gesten/Accessibilitydetails bleiben offen · music

**ui.settings.effects** · Effekte

Einsatz: ADR011; Labels für bestätigte lokale Einstellungen, Gesten/Accessibilitydetails bleiben offen · effects

**ui.settings.haptics** · Haptik

Einsatz: ADR011; Labels für bestätigte lokale Einstellungen, Gesten/Accessibilitydetails bleiben offen · haptics

**ui.settings.master** · Gesamtlautstärke

Einsatz: ADR011; Labels für bestätigte lokale Einstellungen, Gesten/Accessibilitydetails bleiben offen · master

**ui.settings.on** · Ein

Einsatz: ADR011; Labels für bestätigte lokale Einstellungen, Gesten/Accessibilitydetails bleiben offen · on

**ui.settings.off** · Aus

Einsatz: ADR011; Labels für bestätigte lokale Einstellungen, Gesten/Accessibilitydetails bleiben offen · off

**ui.settings.privacy** · Datenschutz

Einsatz: ADR011; Labels für bestätigte lokale Einstellungen, Gesten/Accessibilitydetails bleiben offen · privacy

**ui.settings.about** · Über das Spiel

Einsatz: ADR011; Labels für bestätigte lokale Einstellungen, Gesten/Accessibilitydetails bleiben offen · about

### ui.privacy

**ui.privacy.unavailable** · Datenschutzeinstellungen sind derzeit nicht verfügbar.

Einsatz: Keine Rechts- oder SDK-Texte durch Redaktion ersetzen; finaler Consentflow separat freigeben · unavailable

**GESPERRTER ENTWURF:** PRIVACY_COPY_AND_SDK_FLOW_REQUIRED

**ui.privacy.open** · DATENSCHUTZEINSTELLUNGEN ÖFFNEN

Einsatz: Keine Rechts- oder SDK-Texte durch Redaktion ersetzen; finaler Consentflow separat freigeben · open

**GESPERRTER ENTWURF:** PRIVACY_COPY_AND_SDK_FLOW_REQUIRED

**ui.privacy.saved** · Ihre Auswahl wurde gespeichert.

Einsatz: Keine Rechts- oder SDK-Texte durch Redaktion ersetzen; finaler Consentflow separat freigeben · saved

**GESPERRTER ENTWURF:** PRIVACY_COPY_AND_SDK_FLOW_REQUIRED

**ui.privacy.not-saved** · Ihre Auswahl konnte noch nicht gespeichert werden.

Einsatz: Keine Rechts- oder SDK-Texte durch Redaktion ersetzen; finaler Consentflow separat freigeben · notSaved

**GESPERRTER ENTWURF:** PRIVACY_COPY_AND_SDK_FLOW_REQUIRED

### ui.state

**ui.state.loading** · Wird geladen …

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · loading

**ui.state.empty** · Derzeit keine Einträge vorhanden.

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · empty

**ui.state.failed** · Dieser Bereich konnte nicht geladen werden.

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · failed

**ui.state.try-again** · Bitte versuchen Sie es erneut.

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · tryAgain

**ui.state.offline** · Keine Verbindung verfügbar.

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · offline

**ui.state.offline-play** · Bereits verfügbare lokale Rätsel können ohne Internet gespielt werden.

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · offlinePlay

**ui.state.saving** · Wird gespeichert …

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · saving

**ui.state.saved** · Gespeichert

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · saved

**ui.state.save-failed** · Die letzte Änderung konnte nicht gesichert werden.

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · saveFailed

**ui.state.save-retry** · SICHERUNG ERNEUT VERSUCHEN

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · saveRetry

**ui.state.save-recovery** · Ein älterer gesicherter Stand wurde geladen. Prüfen Sie Ihre letzte Eingabe.

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · saveRecovery

**ui.state.save-unavailable** · Es konnte kein gesicherter Spielstand geladen werden.

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · saveUnavailable

**ui.state.save-conflict** · Der Spielstand hat sich geändert. Bitte laden Sie den aktuellen Stand.

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · saveConflict

**ui.state.content-missing** · Benötigte Spieldaten fehlen. Dieser Bereich bleibt geschlossen.

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · contentMissing

**ui.state.content-invalid** · Die Spieldaten konnten nicht geprüft werden. Dieser Bereich bleibt geschlossen.

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · contentInvalid

**ui.state.resume** · Letzten gesicherten Stand fortsetzen

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · resume

**ui.state.resume-failed** · Die Wiederaufnahme ist derzeit nicht möglich.

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · resumeFailed

**ui.state.purchase-busy** · Eine andere Freischaltung wird noch verarbeitet.

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · purchaseBusy

**ui.state.reward-failed** · Die Gutschrift konnte noch nicht abgeschlossen werden.

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · rewardFailed

**ui.state.retry-reward** · GUTSCHRIFT ERNEUT PRÜFEN

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · retryReward

**ui.state.session-ended** · Diese Spielsitzung ist nicht mehr aktiv.

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · sessionEnded

**ui.state.return-to-map** · ZUR BETRIEBSLAGE

Einsatz: Neutrale technische Zustände; Erfolgsaussagen nur nach bestätigtem zugehörigem Read-/Commitresultat · returnToMap

### campaign.s1

**campaign.s1.section.01.name** · Stammstrecke

Einsatz: Bible14 §4; bestehender Abschnitt, keine zusätzliche Meldung · name

**campaign.s1.section.01.intro** · Der Betrieb beginnt regulär. Die erste Abweichung ist bereits eingetragen.

Einsatz: Bible14 §4; bestehender Abschnitt, keine zusätzliche Meldung · intro

**campaign.s1.section.01.complete** · Der Hauptstrang ist offen. Die anfängliche Zuversicht hat jetzt eine Strecke.

Einsatz: Bible14 §4; bestehender Abschnitt, keine zusätzliche Meldung · complete

**campaign.s1.section.01.available** · Die Stammstrecke steht zur Bearbeitung bereit.

Einsatz: Bible14 §4; bestehender Abschnitt, keine zusätzliche Meldung · available

**campaign.s1.section.02.name** · Bauabschnitt Mitte

Einsatz: Bible14 §5; bestehender Abschnitt, keine zusätzliche Meldung · name

**campaign.s1.section.02.intro** · Die kleine Maßnahme hat einen eigenen Netzabschnitt erhalten.

Einsatz: Bible14 §5; bestehender Abschnitt, keine zusätzliche Meldung · intro

**campaign.s1.section.02.complete** · Das Provisorium funktioniert. Der Bauabschnitt hat wieder einen durchgehenden Weg.

Einsatz: Bible14 §5; bestehender Abschnitt, keine zusätzliche Meldung · complete

**campaign.s1.section.02.available** · Bauabschnitt Mitte ist erreichbar. Der Bauzaun bleibt zur Orientierung stehen.

Einsatz: Bible14 §5; bestehender Abschnitt, keine zusätzliche Meldung · available

**campaign.s1.section.03.name** · Außenast ohne Halt

Einsatz: Bible14 §6; bestehender Abschnitt, keine zusätzliche Meldung · name

**campaign.s1.section.03.intro** · Die Züge fahren weiter. Der Halt wird gesondert behandelt.

Einsatz: Bible14 §6; bestehender Abschnitt, keine zusätzliche Meldung · intro

**campaign.s1.section.03.complete** · Der Außenast ist angeschlossen. Er liegt wieder auf dem Weg und nicht nur daneben.

Einsatz: Bible14 §6; bestehender Abschnitt, keine zusätzliche Meldung · complete

**campaign.s1.section.03.available** · Der Außenast ist erreichbar. Bitte beachten Sie die bisher ausgelassenen Halte.

Einsatz: Bible14 §6; bestehender Abschnitt, keine zusätzliche Meldung · available

**campaign.s1.section.04.name** · Umleitung über Irgendwo

Einsatz: Bible14 §7; bestehender Abschnitt, keine zusätzliche Meldung · name

**campaign.s1.section.04.intro** · Die direkte Verbindung wird durch mehrere indirekte Angaben ersetzt.

Einsatz: Bible14 §7; bestehender Abschnitt, keine zusätzliche Meldung · intro

**campaign.s1.section.04.complete** · Die Umleitung ist verbunden. Der Weg lässt sich jetzt ohne weiteren Wegweiser nachvollziehen.

Einsatz: Bible14 §7; bestehender Abschnitt, keine zusätzliche Meldung · complete

**campaign.s1.section.04.available** · Die Umleitung ist erreichbar. Die bisherige Richtung gilt hier nur bedingt.

Einsatz: Bible14 §7; bestehender Abschnitt, keine zusätzliche Meldung · available

**campaign.s1.section.05.name** · Schienenersatzverkehr

Einsatz: Bible14 §8; bestehender Abschnitt, keine zusätzliche Meldung · name

**campaign.s1.section.05.intro** · Die Schiene endet am Ersatzhalt. Ihre Zuständigkeit endet dort noch nicht.

Einsatz: Bible14 §8; bestehender Abschnitt, keine zusätzliche Meldung · intro

**campaign.s1.section.05.complete** · Der letzte Ersatzweg hat wieder Gleise. Der Schienenersatzverkehr darf sich vertreten lassen.

Einsatz: Bible14 §8; bestehender Abschnitt, keine zusätzliche Meldung · complete

**campaign.s1.section.05.available** · Der Schienenersatzverkehr ist erreichbar. Die Haltestelle wird vorsorglich weiter ausgeschildert.

Einsatz: Bible14 §8; bestehender Abschnitt, keine zusätzliche Meldung · available

**campaign.s1.route.01.01.name** · Planmäßiger Ausnahmebetrieb

Einsatz: Bible14 Route01; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Planmäßiger Ausnahmebetrieb

**campaign.s1.route.01.01.intro** · Der erste Tunnelabschnitt wird geprüft. Der Fahrplan geht noch von Durchfahrt aus.

Einsatz: Bible14 Route01; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

**campaign.s1.route.01.01.complete** · Der Tunnelabschnitt ist offen. Die Ausnahme hat sich als befahrbar erwiesen.

Einsatz: Bible14 Route01; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.01.02.name** · Signalstörung im Feierabendverkehr

Einsatz: Bible14 Route02; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Signalstörung im Feierabend

**campaign.s1.route.01.02.intro** · Das Signal fällt zur üblichen Zeit auf. Der Feierabendverkehr ist bereits anwesend.

Einsatz: Bible14 Route02; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

**campaign.s1.route.01.02.complete** · Der Signalabschnitt ist verbunden. Die Anzeige und die Strecke haben denselben Stand.

Einsatz: Bible14 Route02; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.01.03.name** · Anschluss fährt planmäßig ohne Sie

Einsatz: Bible14 Route03; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Anschluss: planmäßig ohne Sie

**campaign.s1.route.01.03.intro** · Der Seitenbahnsteig liegt richtig. Seine Verbindung liegt noch zur Bearbeitung vor.

Einsatz: Bible14 Route03; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

**campaign.s1.route.01.03.complete** · Der Seitenbahnsteig ist verbunden. Der Anschluss muss nun wenigstens nicht an der Strecke scheitern.

Einsatz: Bible14 Route03; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.01.04.name** · Fahrplan wird fortlaufend angepasst

Einsatz: Bible14 Route04; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Fahrplan in Anpassung

**campaign.s1.route.01.04.intro** · Der Hauptstrang erhält die nächste Fahrplanfassung. Sie dürfen die Verbindung dazu herstellen.

Einsatz: Bible14 Route04; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

**campaign.s1.route.01.04.complete** · Der erste Hauptstrang steht. Die Fahrplanfassung hat erstmals eine passende Strecke.

Einsatz: Bible14 Route04; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.02.01.name** · Baumaßnahme von überschaubarer Dauer

Einsatz: Bible14 Route05; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Baumaßnahme mit Dauer

**campaign.s1.route.02.01.intro** · Der provisorische Bahnsteig ist eingerichtet. Die Verbindung wurde als Folgemaßnahme erfasst.

Einsatz: Bible14 Route05; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

**campaign.s1.route.02.01.complete** · Der provisorische Bahnsteig ist verbunden. Seine vorläufige Nutzung ist dauerhaft möglich.

Einsatz: Bible14 Route05; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.02.02.name** · Eingleisiger Betrieb aus Überzeugung

Einsatz: Bible14 Route06; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Eingleisiger Betrieb

**campaign.s1.route.02.02.intro** · Der Engpass erhält eine eigene Begründung. Für die Durchfahrt genügt weiterhin ein funktionierender Weg.

Einsatz: Bible14 Route06; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

Kurz: Ein Engpass mit eigener Begründung. Ein funktionierender Weg genügt.

**campaign.s1.route.02.02.complete** · Der Engpass ist befahrbar. Die begrenzte Kapazität wird nun tatsächlich genutzt.

Einsatz: Bible14 Route06; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.02.03.name** · Stellwerk meldet sich später

Einsatz: Bible14 Route07; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Stellwerk meldet sich später

**campaign.s1.route.02.03.intro** · Die Rückmeldung ist angekündigt. Im Signalabschnitt warten noch die Verbindungen.

Einsatz: Bible14 Route07; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

**campaign.s1.route.02.03.complete** · Der Stellwerksabschnitt ist verbunden. Die Rückmeldung beschreibt jetzt einen brauchbaren Zustand.

Einsatz: Bible14 Route07; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.02.04.name** · Umstieg am Bahnsteig der Unklarheit

Einsatz: Bible14 Route08; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Umstieg der Unklarheit

**campaign.s1.route.02.04.intro** · Der Umstieg wurde verlegt. Die Orientierung folgt nach Abschluss der Wegeführung.

Einsatz: Bible14 Route08; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

**campaign.s1.route.02.04.complete** · Der Bauabschnitt ist verbunden. Der Umstieg hat wieder einen durchgehenden Weg.

Einsatz: Bible14 Route08; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.03.01.name** · Halt entfällt aus betrieblichen Gründen

Einsatz: Bible14 Route09; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Halt entfällt

**campaign.s1.route.03.01.intro** · Der Außenbahnsteig ist vorhanden. In den betrieblichen Angaben wird er derzeit übergangen.

Einsatz: Bible14 Route09; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

**campaign.s1.route.03.01.complete** · Der erste Außenbahnsteig ist wieder angebunden. Der Halt hat erneut eine Strecke.

Einsatz: Bible14 Route09; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.03.02.name** · Wagenreihung mit Überraschungseffekt

Einsatz: Bible14 Route10; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Wagenreihung überrascht

**campaign.s1.route.03.02.intro** · Der Zugteil steht am Außenast. Seine Zuordnung ist weiterhin in Bewegung.

Einsatz: Bible14 Route10; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

**campaign.s1.route.03.02.complete** · Der Zugteil ist eingeordnet. Seine Position lässt sich nun auch durch eine Verbindung erklären.

Einsatz: Bible14 Route10; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.03.03.name** · Zugteil fährt heute eigenständig

Einsatz: Bible14 Route11; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Zugteil eigenständig

**campaign.s1.route.03.03.intro** · Die Abzweigung zum Depot wird gesondert geführt. Ein gemeinsamer Weg ist noch nicht hergestellt.

Einsatz: Bible14 Route11; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

**campaign.s1.route.03.03.complete** · Die Depotverbindung steht. Der Fahrtverband muss sich nicht mehr räumlich erklären.

Einsatz: Bible14 Route11; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.03.04.name** · Anschluss ist leider weitergereist

Einsatz: Bible14 Route12; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Anschluss weitergereist

**campaign.s1.route.03.04.intro** · Der Außenast erhält eine alternative Verbindung. Der ursprüngliche Anschluss ist bereits unterwegs.

Einsatz: Bible14 Route12; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

**campaign.s1.route.03.04.complete** · Der Außenast ist vollständig verbunden. Die Alternative hat einen erreichbaren Endpunkt.

Einsatz: Bible14 Route12; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.04.01.name** · Kurzfristig über einen Umweg

Einsatz: Bible14 Route13; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Kurzfristiger Umweg

**campaign.s1.route.04.01.intro** · Die Umfahrungsbrücke liegt auf der alternativen Strecke. Der direkte Weg bleibt in der Erläuterung.

Einsatz: Bible14 Route13; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

**campaign.s1.route.04.01.complete** · Die Kartenverbindung über die Brücke ist offen. Der Umweg kann jetzt tatsächlich befahren werden.

Einsatz: Bible14 Route13; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.04.02.name** · Mehr Reisezeit als geplant vorgesehen

Einsatz: Bible14 Route14; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Mehr Reisezeit vorgesehen

**campaign.s1.route.04.02.intro** · Der längere Weg wird als Verbindung geführt. Die zusätzliche Zeit gilt bereits als eingeplant.

Einsatz: Bible14 Route14; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

**campaign.s1.route.04.02.complete** · Der lange Kartenweg ist durchgehend verbunden. Die Entfernung ist wenigstens wieder eindeutig.

Einsatz: Bible14 Route14; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.04.03.name** · Bitte prüfen Sie die Route erneut

Einsatz: Bible14 Route15; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Route erneut prüfen

**campaign.s1.route.04.03.intro** · Die Wegweisung wird aktualisiert. Bitte stellen Sie währenddessen eine nachvollziehbare Verbindung her.

Einsatz: Bible14 Route15; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

Kurz: Die Wegweisung wird aktualisiert. Ihre Verbindung darf schon funktionieren.

**campaign.s1.route.04.03.complete** · Die Kartenverbindung hat wieder Orientierung. Die nächste Prüfung darf vom selben Weg ausgehen.

Einsatz: Bible14 Route15; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.04.04.name** · Die Alternative zur Alternative

Einsatz: Bible14 Route16; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Alternative zur Alternative

**campaign.s1.route.04.04.intro** · Die Ersatzroute hat eine eigene Umleitung erhalten. Die Übersicht wird entsprechend verlängert.

Einsatz: Bible14 Route16; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

**campaign.s1.route.04.04.complete** · Der Umleitungsabschnitt ist verbunden. Die Alternative braucht keine weitere Alternative.

Einsatz: Bible14 Route16; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.05.01.name** · Ersatzhaltstelle in planbarer Entfernung

Einsatz: Bible14 Route17; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Ersatzhalt in Entfernung

**campaign.s1.route.05.01.intro** · Der Ersatzhalt ist ausgeschildert. Der Fußweg nimmt inzwischen einen erheblichen Teil der Verbindung ein.

Einsatz: Bible14 Route17; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

Kurz: Der Ersatzhalt ist ausgeschildert. Der Fußweg beansprucht bereits die Verbindung.

**campaign.s1.route.05.01.complete** · Der provisorische Halt hat einen Gleisanschluss. Der längste Teil des Ersatzwegs ist erledigt.

Einsatz: Bible14 Route17; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.05.02.name** · Bus kommt, sobald er kann

Einsatz: Bible14 Route18; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Bus kommt bei Gelegenheit

**campaign.s1.route.05.02.intro** · Der Ersatzbus wird angekündigt. Die gestrichelte Verbindung wartet weiter auf eine feste Grundlage.

Einsatz: Bible14 Route18; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

**campaign.s1.route.05.02.complete** · Die gestrichelte Kartenzone ist wieder verbunden. Die Beförderung hat ihre Schienenbasis zurück.

Einsatz: Bible14 Route18; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.route.05.03.name** · Anschluss an den Ersatzverkehr

Einsatz: Bible14 Route19; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Anschluss an Ersatzverkehr

**campaign.s1.route.05.03.intro** · Am letzten Umsteigeknoten treffen mehrere Ersatzangaben ein. Ein Anschluss fehlt noch.

Einsatz: Bible14 Route19; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

**campaign.s1.route.05.03.complete** · Der letzte große Umsteigeknoten ist verbunden. Die Beschilderung darf aufhören, weitere Wege vorzuschlagen.

Einsatz: Bible14 Route19; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

Kurz: Der letzte Umsteigeknoten steht. Die Wegweisung braucht keine Ergänzung.

**campaign.s1.route.05.04.name** · Betrieb bis auf Weiteres stabil

Einsatz: Bible14 Route20; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · name

Kurz: Betrieb vorläufig stabil

**campaign.s1.route.05.04.intro** · Die letzte Route ist eröffnet. Nach den Ersatzwegen wird nun die Stabilität selbst geprüft.

Einsatz: Bible14 Route20; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · intro

**campaign.s1.route.05.04.complete** · Alle Routen von Season 1 sind verbunden. Die letzte Betriebslage ist tatsächlich bearbeitet.

Einsatz: Bible14 Route20; bestehender Routenbeginn/12er-Abschluss, keine neue Laufzeitsequenz · complete

**campaign.s1.name** · Season 1

Einsatz: Bible14 §11; bestehender Abschluss nach 240 Erstlösungen · name

**campaign.s1.finale** · Betrieb bis auf Weiteres stabil. Bitte bewahren Sie diese Information gut auf.

Einsatz: Bible14 §11; bestehender Abschluss nach 240 Erstlösungen · finale

**campaign.s1.finale.detail** · 240 Meldungen. Eine zusammenhängende Betriebslage. Jede Verbindung haben Sie selbst hergestellt.

Einsatz: Bible14 §11; bestehender Abschluss nach 240 Erstlösungen · finale.detail

**campaign.s1.revision** · Die Betriebsrevision ist freigegeben. Ihre alten Strecken dürfen jetzt bessere Zeiten bekommen.

Einsatz: Bible14 §11; bestehender Abschluss nach 240 Erstlösungen · revision

**campaign.s1.endless** · Die Dauerbaustelle ist nach Seasonabschluss vorgesehen. Neue Meldungen warten auf ihre Qualitätsfreigabe.

Einsatz: Bible14 §11; bestehender Abschluss nach 240 Erstlösungen · endless

Kurz: Dauerbaustelle vorgesehen. Neue Meldungen benötigen noch ihre Qualitätsfreigabe.

### variant.opened

**variant.opened.01** · Die Störung ist erfasst. Die Verbindung steht noch aus.

Einsatz: Eröffnete Störung, noch kein Ergebnis; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 01

**variant.opened.02** · Diese Meldung liegt jetzt zur Bearbeitung vor.

Einsatz: Eröffnete Störung, noch kein Ergebnis; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 02

**variant.opened.03** · Der nächste Streckenabschnitt wartet auf eine durchgehende Verbindung.

Einsatz: Eröffnete Störung, noch kein Ergebnis; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 03

**variant.opened.04** · Die Betriebslage ist eröffnet. A und B sind noch getrennt.

Einsatz: Eröffnete Störung, noch kein Ergebnis; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 04

**variant.opened.05** · Die Abweichung hat eine Meldungsnummer erhalten.

Einsatz: Eröffnete Störung, noch kein Ergebnis; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 05

### variant.released

**variant.released.01** · Die Strecke ist freigegeben. Die Verbindung hält der Prüfung stand.

Einsatz: Gültige Verbindung bereits bestätigt; keine Stern-/Zeitbehauptung; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 01

**variant.released.02** · A und B sind verbunden. Der Eintrag kann geschlossen werden.

Einsatz: Gültige Verbindung bereits bestätigt; keine Stern-/Zeitbehauptung; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 02

**variant.released.03** · Die Strecke funktioniert. Die Zustandsbeschreibung wurde angepasst.

Einsatz: Gültige Verbindung bereits bestätigt; keine Stern-/Zeitbehauptung; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 03

**variant.released.04** · Diese Verbindung ist hergestellt. Weitere Erklärungen sind nicht nötig.

Einsatz: Gültige Verbindung bereits bestätigt; keine Stern-/Zeitbehauptung; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 04

**variant.released.05** · Der Abschnitt ist frei. Die Gleise passen besser zusammen als die bisherigen Angaben.

Einsatz: Gültige Verbindung bereits bestätigt; keine Stern-/Zeitbehauptung; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 05

### variant.route-complete

**variant.route-complete.01** · Zwölf Meldungen bearbeitet. Die Route ist durchgehend verbunden.

Einsatz: Alle zwölf Erstabschlüsse dieser Route bestätigt; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 01

**variant.route-complete.02** · Die Route ist vollständig freigegeben. Die Einzelmeldungen sind erledigt.

Einsatz: Alle zwölf Erstabschlüsse dieser Route bestätigt; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 02

**variant.route-complete.03** · Der Routenstrang steht. Alle zwölf Verbindungen sind hergestellt.

Einsatz: Alle zwölf Erstabschlüsse dieser Route bestätigt; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 03

**variant.route-complete.04** · Diese Route hat keine offene Meldung mehr.

Einsatz: Alle zwölf Erstabschlüsse dieser Route bestätigt; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 04

**variant.route-complete.05** · Die Route ist abgeschlossen. Eine durchgehende Verbindung liegt vor.

Einsatz: Alle zwölf Erstabschlüsse dieser Route bestätigt; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 05

### variant.section-available

**variant.section-available.01** · Der nächste Netzabschnitt ist erreichbar. Seine Meldungen liegen bereit.

Einsatz: Nächster Netzabschnitt tatsächlich zugänglich, nicht automatisch erledigt; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 01

**variant.section-available.02** · Ein weiterer Netzabschnitt ist zur Bearbeitung freigegeben.

Einsatz: Nächster Netzabschnitt tatsächlich zugänglich, nicht automatisch erledigt; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 02

**variant.section-available.03** · Der nächste Bereich kann geöffnet werden. Die dortige Betriebslage besteht bereits.

Einsatz: Nächster Netzabschnitt tatsächlich zugänglich, nicht automatisch erledigt; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 03

**variant.section-available.04** · Der Zugang zum nächsten Netzabschnitt steht.

Einsatz: Nächster Netzabschnitt tatsächlich zugänglich, nicht automatisch erledigt; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 04

**variant.section-available.05** · Die Bearbeitung kann im nächsten Netzabschnitt fortgesetzt werden.

Einsatz: Nächster Netzabschnitt tatsächlich zugänglich, nicht automatisch erledigt; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 05

### variant.ride-ended

**variant.ride-ended.01** · Der Zug hat B erreicht. Die Verbindung ist befahren.

Einsatz: Zug hat B auf selbstgebauter Strecke erreicht, noch keine Zusatzbelohnung; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 01

**variant.ride-ended.02** · Fahrt beendet. Die gebaute Strecke hat ihren Zweck erfüllt.

Einsatz: Zug hat B auf selbstgebauter Strecke erreicht, noch keine Zusatzbelohnung; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 02

**variant.ride-ended.03** · Der Zug ist angekommen. Die Gleise haben ausgereicht.

Einsatz: Zug hat B auf selbstgebauter Strecke erreicht, noch keine Zusatzbelohnung; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 03

**variant.ride-ended.04** · Fahrt abgeschlossen. Ihre selbst gebaute Strecke hat den Zug bis B gebracht.

Einsatz: Zug hat B auf selbstgebauter Strecke erreicht, noch keine Zusatzbelohnung; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 04

**variant.ride-ended.05** · B erreicht. Die praktische Prüfung fällt knapp aus: Es funktioniert.

Einsatz: Zug hat B auf selbstgebauter Strecke erreicht, noch keine Zusatzbelohnung; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 05

### variant.points-granted

**variant.points-granted.01** · {points} Geduldspunkte sind gutgeschrieben.

Einsatz: Bestätigte Punktgutschrift, points aus Ledger, kein Bonuswurf; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 01

**variant.points-granted.02** · Die Gutschrift steht: {points} Geduldspunkte.

Einsatz: Bestätigte Punktgutschrift, points aus Ledger, kein Bonuswurf; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 02

**variant.points-granted.03** · Geduld verbucht: {points} Punkte.

Einsatz: Bestätigte Punktgutschrift, points aus Ledger, kein Bonuswurf; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 03

**variant.points-granted.04** · Ihr Bestand enthält jetzt die zusätzlichen {points} Geduldspunkte.

Einsatz: Bestätigte Punktgutschrift, points aus Ledger, kein Bonuswurf; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 04

**variant.points-granted.05** · Die Abrechnung ist erfolgt. Gutschrift: {points} Geduldspunkte.

Einsatz: Bestätigte Punktgutschrift, points aus Ledger, kein Bonuswurf; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 05

### variant.cosmetic-progress

**variant.cosmetic-progress.01** · Kosmetischer Fortschritt verbucht: {progress}.

Einsatz: Tatsächlich bestätigter kosmetischer Fortschritt, keine Itemvergabe suggerieren; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 01

**GESPERRTER ENTWURF:** COSMETIC_PROGRESS_CATALOG_REQUIRED

**variant.cosmetic-progress.02** · Der Sammlungsfortschritt wurde ergänzt: {progress}.

Einsatz: Tatsächlich bestätigter kosmetischer Fortschritt, keine Itemvergabe suggerieren; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 02

**GESPERRTER ENTWURF:** COSMETIC_PROGRESS_CATALOG_REQUIRED

**variant.cosmetic-progress.03** · Im Betriebswerk ist folgender Fortschritt erfasst: {progress}.

Einsatz: Tatsächlich bestätigter kosmetischer Fortschritt, keine Itemvergabe suggerieren; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 03

**GESPERRTER ENTWURF:** COSMETIC_PROGRESS_CATALOG_REQUIRED

**variant.cosmetic-progress.04** · Der kosmetische Fortschritt ist bestätigt: {progress}.

Einsatz: Tatsächlich bestätigter kosmetischer Fortschritt, keine Itemvergabe suggerieren; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 04

**GESPERRTER ENTWURF:** COSMETIC_PROGRESS_CATALOG_REQUIRED

**variant.cosmetic-progress.05** · Fortschrittsmeldung für das Betriebswerk: {progress}.

Einsatz: Tatsächlich bestätigter kosmetischer Fortschritt, keine Itemvergabe suggerieren; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 05

**GESPERRTER ENTWURF:** COSMETIC_PROGRESS_CATALOG_REQUIRED

### variant.daily

**variant.daily.01** · Die Tageslage ist erfasst. Der Aushang wurde entsprechend ergänzt.

Einsatz: Reine Tagesrahmung, keine Rechte/Beträge/Claimzusage; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 01

**GESPERRTER ENTWURF:** BLOCKER-PROD-002

**variant.daily.02** · Eine zusätzliche Meldung liegt vor. Sie betrifft ausnahmsweise keinen Gleiswechsel.

Einsatz: Reine Tagesrahmung, keine Rechte/Beträge/Claimzusage; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 02

**GESPERRTER ENTWURF:** BLOCKER-PROD-002

**variant.daily.03** · Die Meldetafel hat ihren vorgesehenen Stand erreicht.

Einsatz: Reine Tagesrahmung, keine Rechte/Beträge/Claimzusage; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 03

**GESPERRTER ENTWURF:** BLOCKER-PROD-002

**variant.daily.04** · Heute liegt eine Mitteilung außerhalb der regulären Störungsliste vor.

Einsatz: Reine Tagesrahmung, keine Rechte/Beträge/Claimzusage; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 04

**GESPERRTER ENTWURF:** BLOCKER-PROD-002

**variant.daily.05** · Die Tagesmeldung ist eingegangen. Die übliche Zurückhaltung bleibt angebracht.

Einsatz: Reine Tagesrahmung, keine Rechte/Beträge/Claimzusage; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 05

**GESPERRTER ENTWURF:** BLOCKER-PROD-002

### variant.resume

**variant.resume.01** · Ihr gesicherter Stand ist geladen. Die Verbindung kann weitergebaut werden.

Einsatz: Bestätigt geladene laufende Sitzung, keine Pausezeit-/Streakregel; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 01

**variant.resume.02** · Die Bearbeitung kann am gesicherten Stand fortgesetzt werden.

Einsatz: Bestätigt geladene laufende Sitzung, keine Pausezeit-/Streakregel; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 02

**variant.resume.03** · Rätselstand wiederhergestellt. A und B stehen weiterhin zur Verfügung.

Einsatz: Bestätigt geladene laufende Sitzung, keine Pausezeit-/Streakregel; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 03

**variant.resume.04** · Die letzte Sicherung liegt vor. Sie können weiterplanen.

Einsatz: Bestätigt geladene laufende Sitzung, keine Pausezeit-/Streakregel; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 04

**variant.resume.05** · Wiederaufnahme bereit. Die offenen Felder haben nichts überstürzt.

Einsatz: Bestätigt geladene laufende Sitzung, keine Pausezeit-/Streakregel; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 05

### variant.voluntary-offer

**variant.voluntary-offer.01** · Freiwilliges Video für +10 Geduldspunkte ansehen.

Einsatz: Gleiches freiwilliges +10 Punkte-Videoangebot nach vollständigem Erstabschlussergebnis; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 01

**variant.voluntary-offer.02** · Optional: Ein Werbevideo ansehen und +10 Geduldspunkte erhalten.

Einsatz: Gleiches freiwilliges +10 Punkte-Videoangebot nach vollständigem Erstabschlussergebnis; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 02

**variant.voluntary-offer.03** · +10 Geduldspunkte per freiwilligem Werbevideo beantragen.

Einsatz: Gleiches freiwilliges +10 Punkte-Videoangebot nach vollständigem Erstabschlussergebnis; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 03

**variant.voluntary-offer.04** · Zusätzliche Geduld: +10 Punkte nach einem freiwilligen Werbevideo.

Einsatz: Gleiches freiwilliges +10 Punkte-Videoangebot nach vollständigem Erstabschlussergebnis; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 04

**variant.voluntary-offer.05** · Freiwilliges Zusatzangebot: Werbevideo für +10 Geduldspunkte.

Einsatz: Gleiches freiwilliges +10 Punkte-Videoangebot nach vollständigem Erstabschlussergebnis; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 05

### variant.interruption-return

**variant.interruption-return.01** · Die Unterbrechung ist beendet. Sie können fortfahren.

Einsatz: Unterbrechung beendet, keine Gutschrift behaupten; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 01

**variant.interruption-return.02** · Der Spielbildschirm steht wieder bereit.

Einsatz: Unterbrechung beendet, keine Gutschrift behaupten; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 02

**variant.interruption-return.03** · Zurück im Betrieb. Die nächste Handlung liegt bei Ihnen.

Einsatz: Unterbrechung beendet, keine Gutschrift behaupten; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 03

**variant.interruption-return.04** · Die Unterbrechung ist vorbei. Der Betrieb kann weitergehen.

Einsatz: Unterbrechung beendet, keine Gutschrift behaupten; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 04

**variant.interruption-return.05** · Fortsetzung möglich. Die Zwischenmeldung ist erledigt.

Einsatz: Unterbrechung beendet, keine Gutschrift behaupten; redaktioneller Variantenpool, keine Auswahl-/Zufallsmechanik · 05
