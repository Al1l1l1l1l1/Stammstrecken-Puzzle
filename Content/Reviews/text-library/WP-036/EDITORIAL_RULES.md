# Redaktionelle Regeln – WP-036

Die Oberfläche wirkt dienstlich. Die Lage gerät außer Kontrolle. Die Verbindung funktioniert durch die Leistung des Spielers. Maßgeblich sind Konzept 11, die Screenverträge aus 12 und die festen Routenbögen aus 14. Die ausdrückliche Ownerfestlegung vom 17.10.2026 verlangt eine glasklare Deutsche-Bahn-Anspielung im plump-staubig-behördenhaften Cartoon-/Comic-Ton. Die Sprache trägt diese Anspielung über Pendlerfolgen, Melderituale und absurde Zuständigkeit. Die konkrete grafische WP-035-Ausführung ist weiterhin offen.

| Regel | Positivbeispiel aus der eigenen Bibliothek | Negativbeispiel, ausschließlich redaktionelles Gegenbeispiel |
|---|---|---|
| Konkreten Vorgang benennen | „Der provisorische Bahnsteig ist eingerichtet. Die Verbindung wurde als Folgemaßnahme erfasst.“ | „Erlebe ein einzigartiges Abenteuer auf den Schienen!“ |
| Kurze dienstliche Hauptsätze | „Der Zugang zum nächsten Netzabschnitt steht.“ | „Aufgrund einer gegebenenfalls sich verändernden Lageentwicklung wird die Zugangsermöglichung in Aussicht gestellt.“ |
| Absurde Zuversicht aus dem Zustand | „Der Halt wird gesondert behandelt.“ | „Haha, schon wieder Chaos!“ |
| Fahrgastfolge statt bloßer Verspätungswitz | „Der Anschluss ist bereits unterwegs.“ | „Alle Züge sind immer zu spät.“ |
| Verwaltungslogik sichtbar machen | „Die kleine Maßnahme hat einen eigenen Netzabschnitt erhalten.“ | „Der lustige Schienenkobold hat die Gleise versteckt.“ |
| Leistung konkret anerkennen | „Jede Verbindung haben Sie selbst hergestellt.“ | „Du bist der ultimative Bahnheld!!!“ |
| Keine Lösungsbewertung vorwegnehmen | „Notiz gesetzt. Ihre Richtigkeit wird damit nicht bewertet.“ | „Gute Wahl! Dieses graue Feld gehört zur Lösung.“ |
| Klare Fehlerrückmeldung | „Diese benachbarten Gleisenden passen nicht zusammen.“ | „Ihre Logik ist verspätet.“ |
| Freiwilliges Angebot transparent | „Freiwilliges Video ansehen: +10 Geduldspunkte. Einmal je Meldung.“ | „Jetzt Bonus sichern, sonst verpasst du etwas!“ |
| Objektfunktion sauber halten | „Ein anderer Zug für dieselbe selbst gebaute Strecke.“ | „Dieser Zug bringt Sie schneller zu drei Sternen.“ |

Erst die Sachinformation, dann gegebenenfalls eine kleine Verschiebung. Nicht jede Statuszeile braucht eine Pointe. Buttons, Eingabe-, Speicher- und Kaufzustände bleiben eindeutig. Satire darf weder Fehlerfolgen verschleiern noch ein unsicheres Ergebnis als gesichert ausgeben.

## Kontinuität statt Spruchsammlung

Die Season verfolgt einen einzigen Betriebsvorgang: Normalität wird behauptet, eine Maßnahme wird dauerhaft, Halte und Zugteile verlieren ihren Bezug, Umleitungen brauchen weitere Umleitungen, schließlich muss der Ersatzverkehr selbst wieder durch eine Schienenverbindung ersetzt werden. Motive kehren verändert wieder: Die vorläufige Angabe wird zum dauerhaften Provisorium; die Rückmeldung wird selbst zum Gegenstand einer Prüfung; der Wegweiser ersetzt zunehmend die Orientierung. Erst am Ende stimmen Angaben und tatsächliche Verbindung überein.

Zwölf Meldungen einer Route bilden eine kleine Entwicklung. 1–2 eröffnen den bekannten Vorgang, 3–5 präzisieren seine absurde Begründung, 6–8 zeigen seine Folgen, 9–10 verändern Perspektive oder Zuständigkeit, 11–12 schließen den Vorgang. Diese Rahmung behauptet keine zeitgesteuerten Ereignisse im Rätsel.

Titel dürfen die in Bible 14 bereits festgelegten Erfolgsmotive tragen. Ein Titel wie „Tatsächlich pünktlich“ ist kein Drei-Sterne-Zuspruch. Die Sternanzeige darf nur den tatsächlich erreichten Ergebnisrecord beschriften. Ergebniszeilen behaupten grundsätzlich Verbindung und bearbeitete Meldung; sie behaupten keine unkalibrierte Zeit, garantierten Anschluss oder neuen Kosmetikgewinn.

## Sprache und Darstellung

- Einheitliche direkte Ansprache: Sie/Ihre, wo eine Ansprache nötig ist. Viele Meldungen kommen ohne Anrede aus.
- Feste Produktbegriffe: Störungsmeldung, Netzabschnitt, Route, Betriebslage, Betriebswerk, Geduldspunkte, Übungsfahrt, Betriebsrevision, Dauerbaustelle. Die drei Sternnamen bleiben unverändert.
- Schienenersatzverkehr bleibt ein markanter finaler Schwerpunkt. Allgemeine Begriffe wie Signalstörung, Haltausfall, Wagenreihung und Bahnsteigwechsel sind erwünscht. Daraus entstehen keine Sonderfelder.
- Interne Koordinaten sind nullbasiert. Spielertexte mit Zeilen-/Spaltenparametern verlangen ausdrücklich 1-basierte Darstellung durch einen späteren geprüften Presenter. Keine verdeckte Indexkonvertierung im Textchecker.
- Zahlen in Erläuterungen sind deutsche Ganzzahlen. Geduldspunkte sind keine Echtgeldwährung. Zahlen, Dauer und Preise in Platzhaltern werden als ganze Nachrichten übergeben; keine Stringverkettung und keine hier erfundene Währungs-/Zeitformatierung.
- UTF-8 ohne BOM, NFC, echte Umlaute, × für Rasterdimensionen und … als Auslassungszeichen. Gedankenstriche dienen nur einer klaren Labelzuordnung.
- Alle Zeichenlimits sind redaktionelle Vorschläge in Unicode-Codepoints einschließlich Platzhaltersyntax. Echte Platzhalterexpansion, Schriftgröße, Umbruch, dynamische Schrift und Screenreader benötigen spätere UI-Prüfung. Kurztexte sind Vorschläge, kein automatischer Kürzungsalgorithmus.

## Kritische Prüfung

Jede Meldung muss eine konkrete Rolle im Routenbogen haben. Ein ausgetauschtes Synonym allein schafft keine zweite Meldung. Identische längere Spielertexte und ähnliche Satzanfänge werden technisch aufgelistet und fachlich beurteilt. Allgemeine dienstliche Wörter dürfen wiederkehren; dieselbe Pointe sollte keine Serie tragen. Abweichungen vom Bible-Namen werden begründet, bestätigte Routennamen nicht umbenannt. Die historische Schreibweise „Ersatzhaltstelle in planbarer Entfernung“ bleibt als Routenname erhalten; allgemeine Fließtexte verwenden „Ersatzhaltestelle“ beziehungsweise „Ersatzhalt“.

Alle 240 Einstiegstexte sind interne redaktionelle Informationen. Ihre Bindung an reale Level-/Reviewquellen ist getrennt von den Spielertexten nachgewiesen. Sie dürfen nicht in einen zufälligen Hinweis- oder Statuspool gelangen. Eine aktiv angeforderte spätere Hilfe braucht geprüfte Prämissen und einen konkreten aktuellen Rätselstand.

Die Texte sind neu für das Spiel verfasst. Aus den bereitgestellten Konzepten übernommene feste Bezeichnungen und eigene Tonreferenzen sind interne Projektquellen. Keine realen Ansagen, offiziellen Statusmeldungen, fremden Witze, Logos oder Medien übernommen. Keine Kennzeichnung als offizieller Verkehrsservice, keine Kooperation behauptet. Das ist ein Herkunftsnachweis der Erstellung, keine rechtliche Endfreigabe oder externe Plagiatsrecherche.
