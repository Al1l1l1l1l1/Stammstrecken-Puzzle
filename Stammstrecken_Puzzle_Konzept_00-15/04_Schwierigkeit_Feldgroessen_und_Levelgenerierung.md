# Train Track Spiel – Schwierigkeit, Feldgrößen und Levelgenerierung

**Status:** Aktuelle, beschlossene Arbeitsfassung  
**Einordnung:** Konzeptbaustein 04  
**Zweck:** Dieses Dokument definiert, wie aus dem bekannten Raster-Schienenpuzzle eine lange, nachvollziehbare Lernkurve entsteht. Es trennt verbindlich die objektive Schwierigkeit eines Rätsels von der persönlichen Meisterschaft, mit der ein Spieler es löst.

## 1. Verbindliche Trennung: Rätsel und Spieler

Ein Level hat eine eigene, objektive Logikschwierigkeit. Diese wird durch seine Struktur bestimmt, nicht durch die Geschwindigkeit oder den persönlichen Lösungsstil eines Spielers. Ein geübter Spieler darf ein schwieriges Level direkt lösen; ein anderer darf dieselbe Aufgabe mit X- und grauen Belegungsmarkierungen ruhig durchdenken. Beide lösen dasselbe gültige Rätsel.

| Ebene | Woran sie gemessen wird | Darf nicht davon abhängen |
|---|---|---|
| Rätselschwierigkeit | Feldgröße, Zahlenverteilung, A/B-Lage, Streckendichte, logische Schlussketten und notwendige Geometrie. | Zeit, Zahl der X-Markierungen, Zahl grauer Belegungsmarkierungen oder persönlicher Stil. |
| Persönliche Meisterschaft | Eigene Lösungszeit, persönliche Bestzeit, Korrekturen und optional direkte Lösung ohne Hilfsmarkierungen. | Dem Zugang zu normalen Leveln, der Gültigkeit der Lösung oder der grundsätzlichen Anerkennung des Erfolgs. |

> **Der Kampagnenfortschritt entsteht immer durch das Lösen. Meisterschaft macht sichtbar, wie sich der Spieler im Lösen verbessert.**

Diese Trennung verhindert zwei Fehler: Schnelles Spielen wird nicht mit besserem Rätseldesign verwechselt, und sorgfältiges Planen wird nicht als minderwertiger Spielstil behandelt.

## 2. Grundsätze der Rätselschwierigkeit

Ein größeres Raster macht ein Rätsel nicht automatisch besser oder schwieriger. Die Schwierigkeit entsteht aus der **Denkstruktur**: aus dem Zusammenspiel von Randzahlen, Endpunktpositionen, Streckendichte und den Schlussketten, die der Spieler über mehrere Zeilen und Spalten hinweg erkennen muss.

Die Feldgröße ist deshalb eine von mehreren Stellschrauben. Sie steuert, wie viel räumliche Information der Spieler gleichzeitig verwalten muss. Sie darf nie als alleiniger Ersatz für gutes Leveldesign dienen.

Die Kampagne besteht aus **handverlesenen, sorgfältig geprüften Leveln**. **Season 1 enthält keine Sonderfelder und keine zusätzlichen Rätselregeln.** Jede der 240 Störungsmeldungen verwendet ausschließlich Raster, Randzahlen, A/B-Anschlüsse, die sechs Gleisformen sowie die optionalen X- und grauen Belegungsmarkierungen. Schwierigkeit entsteht vollständig aus deren Anordnung und logischer Verzahnung, nicht aus künstlicher Regelvielfalt.

Zufallsgenerierte Rätsel ergänzen später als Endlosmodus die Wiederspielbarkeit. Sie dürfen nur angeboten werden, wenn sie eindeutig lösbar, vollständig prüfbar und einer nachvollziehbaren Rätselschwierigkeit zugeordnet sind.

## 3. Die fünf Schwierigkeitsstufen

| Stufe | Typische Raster | Zentrale Denkaufgabe | Qualitätsziel |
|---|---:|---|---|
| Einstieg | 4×4 bis 4×5 | Gerade, Kurve, A–B-Verbindung und Randzahlen verstehen. | Der Spieler versteht nach wenigen Leveln das Grundprinzip ohne Tutorial-Überladung. |
| Grundlagen | 5×5 bis 6×5 | Sichere leere und belegte Felder erkennen; optionale Notizen bei Bedarf nutzen. | Planung ist verfügbar, aber niemals Voraussetzung. |
| Planung | 6×6 bis 7×6 | Mehrere Hinweise kombinieren; lokale Gewissheit auf entfernte Felder übertragen. | Der Spieler baut selbstständig kleine Schlussketten auf. |
| Strategie | 7×7 bis 8×8 | Unscheinbare Abhängigkeiten, engere Vorgaben und langfristigere Vorausplanung. | Jede konkrete Platzierung fühlt sich begründet statt geraten an. |
| Meisterschaft | 9×9 bis 10×10 und darüber | Große, miteinander verflochtene Schlussketten über das gesamte Raster. | Der Spieler löst komplexe Felder im eigenen Tempo und behält den Überblick. |

Diese Stufen sind keine starren Größenklassen. Ein 6×6-Level kann strategisch sein, wenn seine Hinweise dicht verzahnt sind; ein 8×8-Level kann erholsam sein, wenn viele Aussagen eindeutig sind. Die Einstufung richtet sich deshalb nach dem gesamten Denkaufwand.

## 4. Die sechs Parameter eines guten Levels

| Parameter | Wirkung auf die Schwierigkeit | Sinnvolle Variation |
|---|---|---|
| Feldgröße | Erhöht die Menge gleichzeitig zu verwaltender Information. | Schrittweise von 4×4 zu 10×10 statt abrupter Sprünge. |
| Lage von A und B | Bestimmt Richtung, Länge und räumliche Spannung der möglichen Strecke. | Gegenüberliegende Kanten, angrenzende Kanten oder asymmetrische Positionen. |
| Streckendichte | Bestimmt, wie viele Felder tatsächlich belegt werden müssen. | Luftige Felder fördern Ausschlusslogik; dichte Felder fordern genaue Kurvenplanung. |
| Zahlenverteilung | Steuert, wo sichere Aussagen und wo Mehrdeutigkeit entstehen. | Einzelne sehr niedrige oder sehr hohe Zahlen schaffen starke Anker. |
| Ausschlussstruktur | Bestimmt, wie gut leere Felder aus den Randzahlen abgeleitet werden können. | Klarer Anfang in frühen Leveln, subtilere Ausschlüsse in höheren Stufen. |
| Schlusskettentiefe | Bestimmt, wie viele korrekte Folgerungen aufeinander aufbauen müssen. | Von einem direkten Schluss zu mehrstufigen Abhängigkeiten über mehrere Linien. |

Ein hochwertiges Level kombiniert diese Parameter gezielt. Es beginnt mit einer lesbaren Erkenntnis, eröffnet daraus eine Folge weiterer Ableitungen und endet in einer klaren, unvermeidbaren Lösung. Es verlangt niemals willkürliches Raten.

## 5. Kapitelarchitektur der Kampagne

Die Kampagne besteht aus kurzen, klaren Lern- und Meisterschaftsbögen. Ein Kapitel führt genau eine neue Form der Denkbelastung ein, lässt sie in unterschiedlichen Situationen üben und schließt mit wenigen herausfordernden Leveln ab. Eine Welt- oder Theme-Entscheidung wird später darübergelegt; die fachliche Kapitelstruktur bleibt davon unabhängig.

| Kapiteltyp | Funktion | Typische Zusammensetzung |
|---|---|---|
| Einführungskapitel | Eine neue Denkform verständlich machen. | Sehr kleine Raster, deutliche Zahlen, schnelle Abschlüsse. |
| Übungskapitel | Die Denkform in mehreren Konstellationen festigen. | Variierte A/B-Lage, andere Dichte, mehrere kurze Schlussketten. |
| Kombinationskapitel | Zwei bekannte Denkformen zusammenführen. | Mittlere Raster und mindestens zwei voneinander abhängige Teilbereiche. |
| Meisterschaftskapitel | Die erlernte Logik sicher abrufen. | Größere Raster, längere Schlussketten und vollständig freie Wahl der Denkwerkzeuge. |

Kein Kapitel schaltet die nächste Komplexität frei, bevor es mehrere gute, nicht redundante Anwendungen der aktuellen Komplexität geliefert hat.

## 6. Kampagnenlevel versus Endlosrätsel

| Modus | Inhalt | Anspruch | Rolle im Produkt |
|---|---|---|---|
| Kampagne | Handverlesene, sorgfältig geprüfte Level. | Jede Aufgabe hat einen didaktischen oder dramaturgischen Zweck. | Ersterfahrung, Kompetenzaufbau und sichtbarer Fortschritt. |
| Endlosmodus | Zufallsgenerierte, automatisiert geprüfte Rätsel. | Jedes Rätsel hat eine eindeutige Lösung und eine passende Schwierigkeitszuordnung. | Langfristige Wiederspielbarkeit nach oder neben der Kampagne. |

Die Kampagne ist immer das Qualitätsversprechen. Der Endlosmodus ist keine Notlösung für zu wenig Content, sondern ein zusätzlicher Raum für Spieler, die den Kern bereits beherrschen und neue Herausforderungen wünschen.

## 7. Anforderungen an den Levelgenerator

Ein Zufallsgenerator darf nicht bloß eine Route würfeln und Randzahlen ausgeben. Er benötigt eine vollständige Prüfstrecke.

| Generatorschritt | Anforderung |
|---|---|
| 1. Parameterwahl | Rastergröße, Zielschwierigkeit, A/B-Position, Streckendichte und Zahlencharakter werden gezielt gewählt. |
| 2. Gültige Strecke erzeugen | Es wird eine einfache, zusammenhängende Strecke ohne Kreuzung und Schleife konstruiert. |
| 3. Rätsel ableiten | Aus der Strecke werden die korrekten Zeilen- und Spaltenzahlen erzeugt. |
| 4. Lösung entfernen | Der Spieler erhält nur Raster, A, B und Randzahlen; nicht die erzeugte Strecke. |
| 5. Eindeutigkeit prüfen | Ein unabhängiger Solver muss exakt eine gültige Lösung finden. Mehrdeutige Rätsel werden verworfen. |
| 6. Schwierigkeit messen | Der Solver ermittelt die logischen Eigenschaften des Rätsels, nicht eine menschliche Zielzeit. |
| 7. Qualitätsfilter | Triviale, zufällig wirkende, unlesbare oder zu frustrierende Rätsel werden verworfen. |
| 8. Ausgabe und Speicherung | Nur geprüfte Rätsel werden dem gewählten Schwierigkeitsbereich zugeordnet und gespeichert. |

Die Generatorqualität wird später nicht nach der Menge erzeugter Rätsel bewertet, sondern nach der Quote von Rätseln, die den vollständigen Filter bestehen und sich für Menschen logisch, klar und interessant anfühlen.

## 8. Schwierigkeit messbar machen

| Messgröße | Aussage |
|---|---|
| Rastergröße | Wie viel Information ist gleichzeitig sichtbar? |
| Pfadlänge und Dichte | Wie viele Felder müssen tatsächlich verknüpft werden? |
| Zahl eindeutiger Anfangsschlüsse | Wie zugänglich ist der Einstieg? |
| Längste Schlusskette | Wie lange muss eine Argumentation im Kopf gehalten werden? |
| Zahl plausibler Alternativen | Wie viele Hypothesen wirken zunächst möglich? |
| Benötigte Rückverfolgung | Reicht direkte Deduktion, oder ist kontrolliertes Testen nötig? |
| Symmetrie und Lesbarkeit | Wirkt das Rätsel natürlich strukturiert statt zufällig chaotisch? |

Für den ersten Endlosmodus ist die Zielvorgabe klar: Die meisten Rätsel müssen ohne blindes Raten lösbar sein. Rätsel, die echte Hypothesentests verlangen, werden später ausdrücklich als Expertenmodus gekennzeichnet statt unerwartet in normale Stufen gemischt.

## 9. Persönliche Meisterschaft als eigene Ebene

Das System kann nach einer Lösung persönliche Werte erfassen, etwa Lösungszeit, persönliche Bestzeit, Korrekturen oder einen direkten Lösungsabschluss ohne Hilfsmarkierungen. Diese Werte beschreiben die Entwicklung des Spielers, verändern aber niemals nachträglich die Rätselschwierigkeit.

| Persönliches Ziel | Bedeutung |
|---|---|
| Bestzeit verbessern | Derselbe Inhalt wird schneller und sicherer gelöst. |
| Direkt lösen | Der Spieler erkennt Belegung und Geometrie ohne externe Notizen. |
| Weniger Korrekturen | Die eigene mentale Planung wird präziser. |
| Schwierigeres Feld meistern | Der Spieler wendet bekannte Logik auf komplexere Strukturen an. |

Welche dieser Werte sichtbar sind, ob sie Sterne auslösen und wie sie belohnt werden, gehört zum späteren Fortschrittssystem. Bereits festgelegt ist: Sie blockieren keine Kampagnenlevel und entwerten keine sorgfältige Nutzung der Hilfsmarkierungen.

## 10. Bewusste Abgrenzung

Die zufallsgenerierten Level werden hier nur als Content- und Qualitätsarchitektur definiert. Zugangsbedingungen, freiwillige Werbeangebote, Währung oder Belohnungen werden erst im Monetarisierungs- und Fortschrittsbaustein entschieden. Der Generator darf niemals so eingesetzt werden, dass Kampagnencontent künstlich knapp oder die normale Spielfortsetzung erzwungen eingeschränkt wird.

Sonderfelder, Weichen, Brücken, Kreuzungen, T-Knoten oder vergleichbare Regelergänzungen sind für Season 1 ausdrücklich ausgeschlossen. Sie können erst für eine spätere Season geprüft werden, wenn sie nachweislich neue Logiktiefe liefern und den klaren Kern nicht verwässern.

## 11. Nächster Konzeptschritt

Als Nächstes wird das **Fortschritts-, Belohnungs- und Inhaltssystem** definiert. Dabei wird entschieden, wie eine gelöste Strecke, persönliche Meisterschaft, Sterne, Freischaltungen, Währung und Sammlung zusammenwirken, ohne die faire Kernlogik zu beschädigen.
