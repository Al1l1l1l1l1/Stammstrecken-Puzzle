# Lieferumfang und Abdeckungsgrenze

Die kanonische Quelle liefert 1.408 vollständige deutsche Einträge: 960 Leveltexte, 85 Kampagnentexte, 308 UI-Texte und 55 Ereignisvarianten. Darin sind 240 interne Einstiege ausdrücklich als INTERNAL gekennzeichnet; 1.168 Einträge sind für Spieler vorgesehen, bleiben aber sämtlich DRAFT. Die aktuelle maschinelle Zählung, Kurzfassungen und Sperren stehen in technical-validation.json.

## Season 1

| Netzabschnitt | Globale Routen | IDs | Meldungen | Textreferenzen |
|---|---|---|---:|---:|
| Stammstrecke | 01–04 | S1-01-01-01 bis S1-01-04-12 | 48 | 192 |
| Bauabschnitt Mitte | 05–08 | S1-02-01-01 bis S1-02-04-12 | 48 | 192 |
| Außenast ohne Halt | 09–12 | S1-03-01-01 bis S1-03-04-12 | 48 | 192 |
| Umleitung über Irgendwo | 13–16 | S1-04-01-01 bis S1-04-04-12 | 48 | 192 |
| Schienenersatzverkehr | 17–20 | S1-05-01-01 bis S1-05-04-12 | 48 | 192 |
| Gesamt | 20 | unveränderte tatsächliche WP-026-IDs | **240** | **960** |

Je Meldung: titleKey → kurzer Titel; statusKey → fiktiver Kontext ohne Lösungsverrat; entryDeductionKey → interner individueller Einstieg; resultTextKey → individuelle Ergebniszeile nach bestätigtem Abschluss. Die Zählung 960 umfasst ausdrücklich 240 interne Referenzen und 720 vorgesehene Spielertexte. Sie ist keine Behauptung, dass alle 960 Strings zur Laufzeit erscheinen.

Quelle ist der frisch abgefragte WP-026-HEAD dc1652befd7545b4c2a2dd0580f35a4e8e05d72d. Der Checker liest genau dessen Gitblobs, die vollständigen zwanzig Leveldesignreviews und das ursprüngliche 1.440-Ziele-Inventar. Keine fremde Datei wurde im Lieferbaum verändert oder integriert. Die 480 mapSegmentId-/trainMomentId-Ziele bleiben offen. Auch die 960 jetzt vorhandenen DRAFT-Quellen benötigen weiterhin freigegebene Import-/Cross-Reference-Zuordnung; das fremde ursprüngliche FAIL-Inventar bleibt als historischer Bericht unverändert.

## Kampagne und wiederkehrende Ereignisse

Fünf Netzabschnitte besitzen je name, intro, complete und available (20 Einträge). Zwanzig Routen besitzen je name, intro und complete (60). Hinzu kommen Seasonname, finale Meldung, konkrete Leistungsanerkennung und die beiden bereits geplanten Endgame-Wege (5). Die Dauerbaustellenzeile sagt ihre ausstehende Qualitätsfreigabe ausdrücklich an. Diese Texte eröffnen keine zusätzliche Story mit Figuren, keine neuen Orte und keine neue Ereignis-/Popupsequenz.

Elf Variantenfamilien mit je fünf Texten: eröffnete Störung, Strecke freigegeben, zwölf Meldungen einer Route abgeschlossen, nächster Netzabschnitt tatsächlich zugänglich, Zugfahrt beendet, bestätigte Punktgutschrift, bestätigter kosmetischer Fortschritt, Tagesrahmung, Wiederaufnahme am gesicherten Stand, freiwilliges +10-Angebot und Rückkehr aus einer Unterbrechung. Punktgewinn und kosmetischer Fortschritt sind getrennte Sachverhalte; ebenso Videoangebot und Rückkehr. Kein Zufall, Rotationssystem, Cooldown oder anderer Auswahlalgorithmus implementiert.

## Oberflächen- und Zustandsinventar

| Bereich | Schlüsselpräfixe | Inhalt und Grenze |
|---|---|---|
| Einstieg, erster Start | ui.app, ui.nav | Originaleinleitung, Spielrolle und feste Navigation. Kein Rastergrößenversprechen angesichts des tatsächlichen 4×4-Auftakts. |
| Betriebslage/Knoten | ui.operations, ui.map | Nächste Handlung, Empfehlung, vier Kartenstatus, Raster-/Stern-/Moduslabels. BESTZEIT VERBESSERN nur in bewerteter Revision; zuvor ÜBUNGSFAHRT STARTEN gemäß späterer Fortschrittsregel. |
| Rätselkopf/Werkzeuge | ui.puzzle, ui.tool | Sechs Formen, X, graue Annahme, Leeren, Mehrfachauswahl, Undo und neutrale Eingaberückmeldung. Keine neue Touchgeste. |
| Regeln/Hinweise | ui.help, ui.feedback, ui.hint | Regelhilfe und objektive Mengen-/Anschlussfehler. Hinweise aktiv angefordert und wegen PROD-001/Darstellung gesperrt; acht konkrete geprüfte Payloadtypen als Textentwürfe, kein geladener Hintgenerator. |
| Zugfahrt/Abschluss | ui.ride, ui.results | Aufbau, Ankunft, gesicherter Abschluss, Übung/Revision, Sterne, Punkte, nächste Handlung und gesonderte Fehler-/Wiederaufnahmetexte. |
| Sterne/Geduld/Fortschritt | ui.star, ui.points | Drei feste Sternbezeichnungen, legitime Notizen, Saldo und bestätigte Ledgergutschrift. Keine erfundenen Zeitgrenzen. |
| Betriebswerk/Objekte | ui.depot, ui.object-category | Sammlung, Erwerb-/Besitz-/Auswahlzustände und bestehende allgemeine Objektkategorien. Individuelle Itemnamen/-beschreibungen/-preise warten auf den finalen Katalog; keine fiktiven Produktionsitems angelegt. |
| Profil/Meisterschaft | ui.profile | Bestehende Bestzeit-/Direktlösungs-/Fortschrittsanzeigen. Saubere Ausführung und Routenmeisterschaft ausdrücklich gesperrt, weil konkrete Auswertung offen. |
| Betriebsrevision | ui.revision | Zugang nach allen Erstabschlüssen, bewertete Wiederholung, einmalige fehlende Sternboni und Schutz vorhandener Werte. |
| Dauerbaustelle | ui.endless | Entwürfe für Einstieg, Schwierigkeitswahl und neutrale Prüfung/Fehler. PROD-003 bleibt offen; keine Schwierigkeitsstufen oder Rewards erfunden. |
| Betriebslage des Tages | ui.daily, variant.daily | Entwürfe für kostenlose/optionale Teilnahme, vorhandene Belohnungsarten und neutrale ausstehende Gutschrift. PROD-002 und Rewardkatalog bleiben gesperrt. Keine lokale Zeitzone oder Uhrzeit erfunden. |
| Anzeigen/freiwillige Angebote | ui.ad, variant.voluntary-offer, variant.interruption-return | Klare Kennzeichnung, +10-Bonus einmal je Meldung nach vollständigem Erstabschlussergebnis; Laden, Ablehnen, Unterbrechung, Verifikation und tatsächliche Gutschrift getrennt. |
| Werbefrei/Datenschutz | ui.store, ui.privacy | Konkrete Entwürfe, Preis als Formatparameter; Storeprodukt, Consent-/SDK- und Rechtscopy bleiben separat freigabepflichtig. Keine finalen Rechtstexte oder Plattformpreise erfunden. |
| Fehler/Leer/Laden/Wiederaufnahme | ui.state und bereichseigene Keys | Erfolg nur nach bestätigter Sicherung/Transaktion. Rückfall auf tatsächlich gesicherten Stand ausdrücklich benannt. Keine Cloud-/Offlineclaim-Garantien. |

Technische Existenz bedeutet weder implementierte Funktion noch freigegebene Oberfläche. DECIDED kennzeichnet nur die konzeptionelle Aussage. Für alle fehlenden Katalog-/Produktentscheidungen liefert die Bibliothek eine konkrete anschließbare Beschriftung oder klar abgegrenzte Entscheidungslücke; sie darf keine noch fehlende Funktion aktivieren.

## Feste Begriffe und bestehende Schlüssel

STRECKE FREIGEBEN, NÄCHSTE STÖRUNG, BETRIEBSWERK, Angekommen, Anschluss erwischt und Tatsächlich pünktlich werden exakt geprüft. Die 24 zusätzlich auf der Main-Basis gefundenen Schlüssel sind einzeln inventarisiert. Die nicht bedienten Schlüssel gehören ausschließlich zu Fixtures oder Tests, etwa bewusst verfälschten Readerinputs. Sie werden nicht durch erfundene Produktionsübersetzungen nachträglich gültig gemacht. Fehlende echte Produktionsreferenzen würden den Checker abbrechen lassen.
