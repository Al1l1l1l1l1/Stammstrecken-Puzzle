# Stammstrecken-Puzzle – Projektübergabe und Gesamtstatus

**Status:** Versandfähige Übergabe an koordinierende Agenten und weitere KI-Systeme  
**Zweck:** Dieses Dokument beschreibt den verbindlichen Projektstand. Es listet alle bislang erledigten Konzeptbausteine, deren Inhalte, die verfügbaren Shared Files, nicht verhandelbare Regeln und die verbleibenden Arbeitspakete auf.

> **Lesereihenfolge für neue Agenten:** Zuerst dieses Dokument lesen, danach `00_Train_Track_Konzeptindex.md`, dann `09_Train_Track_Master_Spezifikation.md`. Für Detailarbeit nur die jeweils relevanten Fachdateien lesen.

---

## 1. Projekt in einem Absatz

**Stammstrecken-Puzzle** ist ein deutschsprachiges Mobile-Logikspiel mit einer satirischen deutschen Bahn-Alltags-Verpackung. Der Spieler baut in einem Raster eine einzelne, korrekte Schienenverbindung von A nach B. Das Spiel basiert auf der Referenzmechanik von *Train Tracks: Puzzle*, wird aber wesentlich hochwertiger inszeniert: durch klare Bedienung, sichtbare Meisterschaft, Betriebslage-Karte, Zugfahrten nach selbst gelösten Rätseln, kosmetische Sammlung, faire Werbung und trocken-bürokratische Bahnfahrer-Lore.

Die Kernfantasie lautet:

> **Der Spieler löst zuverlässig das, woran das fiktive Bahnsystem scheitert: eine funktionierende Verbindung von A nach B.**

---

## 2. Erledigte und verbindlich entschiedene Konzeptblöcke

| Block | Status | Verbindliches Ergebnis |
|---|---|---|
| Zielgruppen- und Motivationsgrundlage | Erledigt | Forschung zu Puzzle-Motivation, Kompetenz, Fortschritt, Sammlung, Werbung und fairer Langzeitmotivation liegt vor. |
| Referenzspiel verstanden | Erledigt | Bedienung und zwei Phasen des Vorbildrätsels wurden anhand von Screenshots und Nutzererklärung präzise erfasst. |
| Rätselkern | Erledigt | A/B-Verbindung, Randzahlen, sechs Gleisformen, keine Kreuzungen, keine Schleifen, passende Anschlüsse. |
| Feld- und Werkzeugsemantik | Erledigt | Leeres Feld, rotes X, graue Belegungsannahme, braune konkrete Schienenform, gelbe reine Auswahlmarkierung; Entfernen, Mehrfachauswahl, Undo und Hinweis. |
| Spielstile | Erledigt | Planend mit Hilfsmarkierungen, gemischt oder direkt/schnell. Hilfsmarkierungen sind optional und vollwertig. |
| Season-1-Regelumfang | Erledigt | Keine Sonderfelder und keine Zusatzregeln. Insbesondere keine Weichen, Brücken, Kreuzungen oder T-Knoten. |
| Schwierigkeit | Erledigt | Schwierigkeit wächst über Feldgröße, A/B-Lage, Dichte, Zahlenverteilung, Ausschlussstruktur, Schlusskettentiefe und Geometrie – nicht allein über Rastergröße. |
| Launchkampagne | Erledigt | Season 1 umfasst 240 kuratierte Rätsel: fünf Netzabschnitte, 20 Routen, zwölf Meldungen pro Route. |
| Contentdramaturgie | Erledigt | Jede Route hat eine 12-Meldungen-Struktur aus Wiedereinstieg, neuem Schwerpunkt, Verdichtung, räumlicher Variation und zwei Höhepunkten. |
| Fortschritt und Sterne | Erledigt | 1 Stern = **Angekommen**, 2 Sterne = **Anschluss erwischt**, 3 Sterne = **Tatsächlich pünktlich**. |
| Zeit- und Wiederholungslogik | Erledigt | Zeitziele werden später pro konkretem Level gesetzt. Hilfsmarkierungen erlauben weiterhin drei Sterne. Erst nach Abschluss einer Season öffnet die bewertete Betriebsrevision für alte Level. |
| Meisterschaft | Erledigt | Bestzeit, direkte Lösung ohne Hilfsmarkierungen und saubere Ausführung sind Profilwerte, aber nie Voraussetzung für Fortschritt oder Sterne. |
| Währung und Sammlung | Erledigt | Geduldspunkte; nur für kosmetische Züge, Lackierungen und Betriebsobjekte im Betriebswerk. Keine Rätselvorteile und keine Lootboxen. |
| Level-Erlebnis | Erledigt | Während des Denkens ruhig und ungestört; nach der Lösung Zugfahrt, Strecke freigegeben, Sterne, Währung und Kartenfortschritt. |
| Bahn-Alltags-Satire | Erledigt | Deutsche Bahnfahrer-Lore, trockene Verwaltungssprache, allgemeine Begriffe wie Schienenersatzverkehr, Signalstörung, Haltausfall, Umleitung, Wagenreihung und Anschluss. |
| Marken- und Weltkonzept | Erledigt | Premium-Miniaturbahnwelt mit bewusst naher deutscher Betriebslagen-Anmutung, aber eigenständigem Puzzle-Produkt. |
| UI-Architektur | Erledigt | Startscreen, Betriebslage-Karte, Rätseloberfläche, Abschluss, Betriebswerk und tägliche Betriebslage sind konzeptionell spezifiziert. |
| Monetarisierung | Erledigt | Früher Meme-/Hype-Funnel für Kurzbesucher, danach begrenzte Frequenz; keine Werbung im Rätsel, bei der Lösung oder während der Zugfahrt. |

---

## 3. Produktkern und verbindliche Rätselregeln

### 3.1 Ziel jedes Rätsels

Eine **Störungsmeldung** ist ein einzelnes Puzzle-Level. Sie besteht aus einem rechteckigen Raster, zwei Außenanschlüssen A und B sowie Randzahlen. Die Randzahlen bestimmen die Anzahl von Gleisfeldern in jeder Zeile und jeder Spalte.

Der Spieler baut genau eine zusammenhängende Strecke von A nach B.

| Gültigkeitsregel | Bedeutung |
|---|---|
| Verbindung | A und B sind durch eine einzige zusammenhängende Strecke verbunden. |
| Randzahlen | Jede Zeile und Spalte enthält exakt die geforderte Zahl an Gleisfeldern. |
| Keine Kreuzung | In einem Feld liegt höchstens ein Gleisabschnitt. |
| Keine Schleife | Es entsteht keine geschlossene Teilrunde. |
| Passende Anschlüsse | Eine konkrete Gleisform endet nicht im Raster oder an einer falschen Außenkante. |

### 3.2 Feldzustände und Werkzeuge

| Element | Bedeutung | Spieleraktion |
|---|---|---|
| Leeres Feld | Unberührt oder vollständig entfernt. | Kann mit jedem Werkzeug belegt werden. |
| Rotes X | Optionale Annahme: Das Feld bleibt leer. | Reine Planungsnotiz; nicht vom System vorgegeben. |
| Graues kleines Standardgleis | Optionale Annahme: In diesem Feld liegt irgendeine Schiene; die Form ist noch offen. | Reine Planungsnotiz; nicht vom System vorgegeben. |
| Braune Gleisform | Konkret gesetzte Strecke. | Eine von sechs möglichen Formen. |
| Gelb | Nur momentane Auswahlhervorhebung. | Unabhängig vom Feldinhalt; niemals Gleisfarbe oder Wahrheitsstatus. |

Die sechs konkreten braunen Gleisformen sind: eine senkrechte Gerade, eine waagerechte Gerade und vier 90°-Kurven.

Die Werkzeuge umfassen: alle sechs Gleisformen, rotes X, graues Standardgleis, vollständiges Leeren, Mehrfachauswahl, Undo und Hinweise.

### 3.3 Gleichwertige Spielstile

Es gibt keine getrennten Anfänger- oder Expertenmodi. Hilfsmarkierungen dürfen genutzt, teilweise genutzt oder vollständig übersprungen werden.

| Stil | Verhalten |
|---|---|
| Strukturiert | X- und graue Belegungsmarkierungen als sichtbare Logiknotizen verwenden. |
| Gemischt | Sichere konkrete Schienen sofort setzen und unsichere Bereiche markieren. |
| Direkt | Braune Schienen unmittelbar setzen, weil die Lösung schnell erkannt wird. |

Der direkte Stil wird als mögliche persönliche Meisterschaft sichtbar, macht planendes Lösen jedoch nicht schlechter.

---

## 4. Season 1: vollständig festgelegter Contentrahmen

### 4.1 Umfang

| Kennzahl | Festlegung |
|---|---:|
| Standardlevel | 240 |
| Netzabschnitte | 5 |
| Routen je Abschnitt | 4 |
| Meldungen je Route | 12 |
| Zusatzregeln / Sonderfelder | 0 |
| Spätere Inhalte | Betriebsrevision und Dauerbaustelle; spätere Seasons möglich |

### 4.2 Netzabschnitte

| Reihenfolge | Netzabschnitt | Satirische Lage | Typische Raster |
|---:|---|---|---|
| 1 | Stammstrecke | Angeblich normaler Betrieb kippt früh in kontrollierte Unklarheit. | 4×4 bis 4×5 |
| 2 | Bauabschnitt Mitte | Kleine Maßnahme wird zur dauerhaften Betriebslage. | 5×5 bis 6×5 |
| 3 | Außenast ohne Halt | Das Netz fährt weiter, hält aber nicht dort, wo es soll. | 6×6 bis 7×6 |
| 4 | Umleitung über Irgendwo | Die Verbindung existiert, aber nicht mehr auf nachvollziehbarem Weg. | 7×7 bis 8×8 |
| 5 | Schienenersatzverkehr | Finaler Ausnahmezustand mit den größten, verzahnten Rätseln. | 8×8 bis 10×10 |

### 4.3 Wiederkehrende Routenform

| Position in einer Route | Zweck |
|---:|---|
| Meldungen 1–2 | Bekannte Muster reaktivieren und schnell zugänglich machen. |
| Meldungen 3–5 | Neue Anordnung oder höhere Denkbelastung klar einführen. |
| Meldungen 6–8 | Neue und alte Logik verbinden. |
| Meldungen 9–10 | Räumliche Variation mit anderer A/B-Lage, Dichte oder Zahlenverteilung. |
| Meldungen 11–12 | Zwei faire Höhepunkte und sichtbare Freigabe des Kartenabschnitts. |

Die detaillierten Routennamen, Lernschwerpunkte und Meldungsgruppen liegen in `14_Season_1_Content_Bible.md`.

### 4.4 Endgame und spätere Seasons

Nach Abschluss aller 240 Meldungen von Season 1 öffnen sich zwei Wege:

| Modus | Inhalt |
|---|---|
| Betriebsrevision | Alte Season-1-Level erneut und bewertet spielen, um Zeit und Sterne zu verbessern. Fehlende Sternboni werden einmalig nachgezahlt. |
| Dauerbaustelle | Neue, automatisiert geprüfte Rätsel mit unbekannter Lösung und wählbarer Schwierigkeit. |

Spätere Seasons haben denselben Grundvertrag: Sie bringen eine vollständige neue Kampagne. Ihre Betriebsrevision öffnet sich erst nach ihrem jeweiligen Season-Abschluss.

---

## 5. Fortschritt, Belohnung und Ökonomie

### 5.1 Sterne

| Ergebnis | Stern | Bedeutung |
|---|---:|---|
| Rätsel korrekt lösen | 1 | **Angekommen.** |
| Gute levelbezogene Zeit | 2 | **Anschluss erwischt.** |
| Sehr gute levelbezogene Zeit | 3 | **Tatsächlich pünktlich.** |

Die Zeitgrenzen sind pro konkretem Rätsel zu bestimmen. Sie dürfen nicht pauschal pro Feldgröße identisch sein. X- und graue Hilfsmarkierungen verhindern niemals drei Sterne.

### 5.2 Geduldspunkte

| Quelle | Wert |
|---|---:|
| Erste korrekte Lösung | 15 |
| Zweiter Stern | +5 |
| Dritter Stern | +10 |
| Vollständige Route mit zwölf Meldungen | +60 |
| Vollständiger Netzabschnitt mit 48 Meldungen | +150 |

Eine perfekte Season-1-Kampagne liefert insgesamt 9.150 Geduldspunkte.

| Objektart | Preisrahmen |
|---|---:|
| Kleines Betriebsobjekt | 120 |
| Mittleres Objekt oder Lackierung | 300 |
| Großes Streckenobjekt | 650 |
| Erster zusätzlicher Zug | 1.200 |
| Besondere Zugvariante | 2.400–2.600 |

Geduldspunkte kaufen ausschließlich Kosmetik. Es gibt keine käuflichen Hilfen, Sterne, Rätsellösungen oder Fortschrittssperren.

---

## 6. Level-, Karten- und UI-Erlebnis

### 6.1 Emotionaler Ablauf einer Störungsmeldung

| Moment | Vorgabe |
|---|---|
| Start | Raster, Randzahlen, A/B und kurze Störungszeile ohne Ablenkung. |
| Denken | Ruhige, präzise Eingabe; keine Werbung und keine falsche Richtig-/Falsch-Bewertung von plausiblen Annahmen. |
| Verdichtung | Eigene Planung wird klarer. Objektive Mengenrückmeldung ist erlaubt, etwa erfüllte oder überschrittene Randzahlen. |
| Durchbruch | Gültige Verbindung wird erkannt, ohne Dialog, Werbung oder Unterbrechung. |
| Zugfahrt | Der gewählte Zug fährt exakt die selbst gebaute Strecke von A nach B ab. |
| Abschluss | Erst danach: Strecke freigegeben, Sterne, Geduldspunkte und sichtbarer Kartenfortschritt. |

### 6.2 Startscreen

Der Startscreen ist kein Menüfriedhof. Er zeigt eine große **Betriebslage** mit aktueller Störung und genau einer dominanten Handlung:

> **STRECKE FREIGEBEN**

Die untere Navigation besteht nur aus: **Betriebslage**, **Betriebswerk** und **Profil**.

### 6.3 Betriebslage-Karte

| Kartenstatus | Darstellung |
|---|---|
| Freigegeben | Zusammenhängender Streckenstrang, sichtbarer Zug und kleine Ergebnissterne. |
| Aktuelle Störung | Kräftiger roter Problemknoten. |
| Kommende Probleme | Gedämpfte sichtbare Knoten im weiteren Verlauf. |
| Noch nicht erreichbar | Zurückhaltende graue Verbindung hinter dem nächsten Engpass. |

Ein Knoten öffnet eine Störungsmeldungskarte mit Meldungsnummer, Kurzursache, Rastergröße, bisherigen Sternen und der Aktion **STRECKE FREIGEBEN** oder **BESTZEIT VERBESSERN**.

### 6.4 Rätseloberfläche

| Element | Vorgabe |
|---|---|
| Kopfzeile | Zurück, Netzabschnitt/Meldung, kurzer Status, Undo und Hinweis. |
| Zentrum | Raster maximal groß; keine Währung, Werbung oder Navigation im Rätsel. |
| Randzahlen | Dunkel, wenn offen; zurückhaltend grau, wenn exakt erfüllt; kurz rot, wenn objektiv überschritten. |
| Werkzeugleiste | Alle sechs Formen, X, graue Belegungsmarkierung, Leeren und Mehrfachauswahl direkt sichtbar. |
| Timer | Beginnt erst mit der ersten Eingabe und bleibt klein. |
| Mehrfachauswahl | Mehrere Felder wählen, Werkzeug wählen, dann gemeinsam anwenden. |

### 6.5 Abschluss, Betriebswerk und Tagesbonus

| Screen | Vorgabe |
|---|---|
| Abschluss | Erst Zugfahrt; dann Ergebnisfläche mit Sternen, Zeit, Punkten, Kartenfortschritt, **NÄCHSTE STÖRUNG** und **ZUR BETRIEBSLAGE**. |
| Betriebswerk | Ruhige, hochwertige Sammlung: aktuell gewählter Zug groß; Reiter **Zugpark** und **Betriebsverbesserungen**. |
| Betriebslage des Tages | Mechanische Meldetafel statt buntes Casino-Rad; einmal täglich kostenlos, maximal einmal zusätzlich nach freiwilligem Video. |

---

## 7. Bahn-Alltags-Satire und visuelle Identität

### 7.1 Satirischer Ton

Die Sprache ist trocken, präzise, frustriert-optimistisch und bewusst deutsch. Die Pointe entsteht aus dem Kontrast zwischen nüchterner Verwaltungssprache und absurd schlechter Betriebslage.

Erlaubte allgemeine Begriffe: **Schienenersatzverkehr**, Signalstörung, Weichenstörung, Stellwerkstörung, Haltausfall, Umleitung, Wagenreihung, Anschluss, mehr Reisezeit und Betriebslage.

Beispielhafte Tonproben:

- „Stammstrecke: derzeit uneingeschränkt eingeschränkt.“
- „Schienenersatzverkehr wird vorbereitet. Bitte lösen Sie schneller.“
- „Ihre Verbindung ist logisch. Der Fahrplan leider nicht.“
- „Wagenreihung erfolgreich geändert. Anlass wird nachgereicht.“
- „Betrieb bis auf Weiteres stabil. Bitte bewahren Sie diese Information gut auf.“

Die Satire zielt auf Systemfrust, Sprachrituale und Pendleralltag. Sie macht keine realen Unfälle, Mitarbeitenden oder Betroffenen zur Pointe.

### 7.2 Visuelle Richtung

Die visuelle Richtung lautet:

> **Premium-Miniaturbahnwelt in deutscher Betriebslagen-Anmutung.**

| Bereich | Verbindliche Richtung |
|---|---|
| Puzzle-Raster | Helle, präzise technische Arbeitsfläche; sehr klare Farbsemantik. |
| Gleise | Warmes, dunkles braunes Materialgefühl mit subtiler Tiefe. |
| UI | Verkehrsrote Akzente, helle Informationsflächen und Anthrazit; straffe eigene Groteskschrift. |
| Auswahl | Eigenes, freundliches Gelb, ausschließlich für aktive Bedienauswahl. |
| Abschluss | Das Raster gewinnt Tiefe und Umgebung; die eigene Strecke bleibt eindeutig erkennbar. |
| Startkulisse | Stammstrecke: fiktiver urbaner Knoten mit Tunnel, Bahnsteigen, Warnbaken, Baustellen und Umleitungswegweisern. |
| Fahrzeuge | Eigenständige stilisierte Nahverkehrs-/S-Bahn-Anmutung mit eigener Front und Lackierung. |
| Eigenes Zeichen | Unterbrochener Gleisknoten; zwei Schienen verfehlen die Verbindung knapp. |

Die gewünschte Wiedererkennung entsteht über Titel, Betriebslage-Duktus, Farbdramaturgie, Informationshierarchie und Bahn-Alltagslogik. Das Spiel darf nicht wie eine offizielle Reise-, Ticket- oder Echtzeit-App erscheinen.

---

## 8. Monetarisierung und Hype-Besucher-Logik

Das Spiel hat zwei relevante Nutzergruppen: Meme-/Gag-Besucher, die es kurz ausprobieren, und langfristige Puzzle-Spieler. Die Monetarisierung erfasst beide ohne den Rätselkern zu beschädigen.

| Werbemoment | Verbindliche Regel |
|---|---|
| App-Start | Nie eine Anzeige. Der Witz und das erste Rätsel müssen sofort funktionieren. |
| Erste gelöste Meldung | Zugfahrt und Ergebnis vollständig werbefrei. Freiwilliger Bonus möglich. |
| Zweite gelöste Meldung | Erste mögliche unterbrechende Anzeige – erst nach Zugfahrt und Ergebnis. |
| Meldungen 3–8 | Höchstens eine Anzeige nach jeder zweiten abgeschlossenen Meldung. |
| Ab Meldung 9 | Höchstens eine Anzeige nach jeder dritten Meldung. |
| Pro Spielsitzung | Höchstens drei unterbrechende Anzeigen. |
| Betriebsrevision | Höchstens nach jeder fünften Wiederholung. |
| Freiwilliges Video nach erster Lösung | Einmalig +10 Geduldspunkte. |
| Freiwilliges Hinweisvideo | Nur nach aktivem Hinweiswunsch; genau ein zusätzlicher Logikhinweis. |
| Tagesbonus | Ein kostenloser und maximal ein weiterer Durchlauf nach Video. |
| Werbefrei-Option | Dauerhafte Entfernung unterbrechender Anzeigen; finaler Preis noch offen. |

**Absolut ausgeschlossen:** Werbung während des Rätsels, zwischen Eingabe und Wirkung, bei der Lösungserkennung, vor/während Zugfahrt, direkt vor dem ersten Level, nach Fehlern oder nach Abbruch.

---

## 9. Erstellte Shared Files

| Nr. | Datei | Inhalt / Nutzen |
|---:|---|---|
| 00 | `00_Train_Track_Konzeptindex.md` | Index aller maßgeblichen Dateien; immer nach dieser Übergabe lesen. |
| 01 | `01_Zielgruppen_und_Motivationspsychologie.md` | Forschungsgrundlage zu Puzzle-Motivation, Kompetenz, Fortschritt, Werbung und Fairness. |
| 02 | `02_Referenzspiel_und_Baseline.md` | Korrekt bestätigte Referenzmechanik des Vorbildspiels. |
| 03 | `03_Raetselkern_und_Interaktionsmodell.md` | Vollständige Feld- und Werkzeugsemantik inklusive optionaler Hilfen und Spielstile. |
| 04 | `04_Schwierigkeit_Feldgroessen_und_Levelgenerierung.md` | Schwierigkeit, Rastergrößen, Generator-/Solver-Anforderungen und keine Sonderfelder in Season 1. |
| 05 | `05_Level_Erlebnis_und_Qualitaetsdifferenzierung.md` | Konzentration, Durchbruch, Zugfahrt und Abschlussinszenierung. |
| 06 | `06_Fortschritt_Belohnungen_und_Meisterschaft.md` | Sterne, Geduldspunkte, Meisterschaft, Zeitlogik und Betriebsrevision. |
| 07 | `07_Zuege_Welten_und_Reisekarte.md` | Betriebslage-Karte, Netzabschnitte, Betriebswerk und kosmetische Betriebsmittel. |
| 08 | `08_Faire_Monetarisierung_und_Werbeangebote.md` | Unterbrechende und freiwillige Werbeangebote, Tagesbonus und Werbefrei-Option. |
| 09 | `09_Train_Track_Master_Spezifikation.md` | Maßgebliche strategische Gesamtfassung; bei jeder Änderung gegenprüfen. |
| 10 | `10_Visuelle_Identitaet_und_Stammstrecke.md` | Visuelle Kernidentität, Stilregeln und Stammstrecke als erste Kulisse. |
| 11 | `11_Bahnfahrer_Lore_und_Satire_Leitplanken.md` | Recherchierte Lore, Textton und rechtlich-gestalterische Leitplanken. |
| 12 | `12_UI_und_Bedienungsspezifikation.md` | Startscreen, Karte, Rätsel, Abschluss, Betriebswerk und Tagesbonus. |
| 13 | `13_Oekonomie_und_Monetarisierungs_Balancing.md` | Season-1-Umfang, Währungswerte, Preise, Betriebsrevision und Hype-Besucher-Monetarisierung. |
| 14 | `14_Season_1_Content_Bible.md` | Fünf Netzabschnitte, 20 Routen, 240 Meldungen, Lernbögen und Datenschema je Rätsel. |
| 15 | `15_Projektuebergabe_und_Gesamtstatus.md` | Diese selbstständige Übergabe für Koordination und neue KI-Systeme. |

---

## 10. Nicht verhandelbare Guardrails für weitere KI-Systeme

1. **Keine Annahmen über das Vorbildspiel.** Bei Mechanikfragen Datei 02 und Datei 03 lesen.
2. **Season 1 bleibt ohne Sonderfelder.** Keine Weichen, Brücken, Kreuzungen, T-Knoten oder neue Regeln.
3. **Hilfsmarkierungen bleiben optional und vollwertig.** Sie verhindern niemals Fortschritt oder drei Sterne.
4. **Während des Rätsels gibt es keine Werbung.** Auch Zugfahrt und Lösungserkennung bleiben ununterbrochen.
5. **Züge und Betriebsobjekte sind nur kosmetisch.** Keine Rätselvorteile und keine Lootboxen.
6. **Die Satire ist trocken und eigenständig.** Allgemeine Bahnbegriffe sind erwünscht; offizielle Logos, Kennzeichen, Originaltexte, Screens und Herkunftstäuschung sind ausgeschlossen.
7. **Bereits bestätigte Entscheidungen nicht stillschweigend verändern.** Jede Änderung braucht eine neue ausdrückliche Produktentscheidung.
8. **Chat und Dokumente ergänzen sich.** Erst kurz im Chat erklären, dann bestätigte Entscheidungen in die passende Fachdatei, die Master-Spezifikation und den Konzeptindex eintragen.

---

## 11. Offene Arbeitspakete

Diese Punkte fehlen noch. Sie sind keine offene Produktidee, sondern die nächsten detaillierten Produktions- und Finalisierungsblöcke.

| Priorität | Arbeitspaket | Konkreter Inhalt |
|---:|---|---|
| 1 | Technische Produktionsspezifikation | Zielplattformen, Engine, Code-/Datenarchitektur, Zustandsmodell, Persistenz, Cloud-/Offline-Strategie, Analytics, Ads, IAP, Datenschutz, QA und Releasepipeline. |
| 2 | Puzzle-Solver und Levelauthoring | Implementierbarer Solver, Eindeutigkeitsprüfung, Generatorvalidierung, Leveldatenformat, Editor-Workflow und automatisierte Tests. |
| 3 | 240 konkrete Rätselinstanzen | Pro Meldung echte Lösung, Randzahlen, A/B-Positionen, Solver-Nachweis, Einstiegsschluss, Qualitätsnotiz, Zeitklasse und Abschlussinszenierung. |
| 4 | Finale Zeitwerte | Konkrete 2-/3-Sterne-Grenzen je gebautem und geprüften Rätsel. |
| 5 | Finale Asset-Bible | Exakte Wort-/Bildmarke, Icon, Schriftlizenz, Farbwerte, Zugfamilien, Lackierungen, Betriebsobjekte, Motion- und Soundregeln. |
| 6 | Vollständige Textbibliothek | Störungs-, Status-, Hinweis-, Abschluss-, Objekt- und Tagesmeldungen mit Längenregeln und Tonprüfung. |
| 7 | Finaler Zug- und Objektkatalog | Welche Züge und Betriebsobjekte es gibt, Preise, Freischaltreihenfolge, Aussehen, Sound und Kartenreaktion. |
| 8 | Werbefrei-Produkt | Plattformkonformer Preis, Produktbeschreibung, Kaufwiederherstellung, Consent- und Datenschutz-Flows. |
| 9 | Rechtliche Endprüfung | Wortmarke, Icon, Screenshots, Werbemittel, Zugdesigns und Herkunftseindruck durch qualifizierte Rechtsberatung prüfen. |
| 10 | Launchpaket | Store-Metadaten, Screenshots, Trailer, Meme-Clips, Influencer-Material, Community- und Messplan. |

---

## 12. Empfohlene Aufgabenteilung für mehrere KI-Systeme

| Rolle | Auftrag | Pflichtdateien vor Start |
|---|---|---|
| Koordinationsagent | Abhängigkeiten steuern, Entscheidungen gegen Master prüfen und Fachagenten beauftragen. | 15, 00, 09 |
| Technische Architektur-KI | Umsetzungsreife Architektur und Technologieentscheidung erarbeiten. | 03, 04, 12, 13, 14 |
| Puzzle-/Algorithmus-KI | Solver, Eindeutigkeitsnachweis, Generator, Datenformat und Editor spezifizieren/implementieren. | 02, 03, 04, 14 |
| UX-/UI-KI | High-fidelity-Screens, Komponenten, Gesten, States, Accessibility und Motion ausarbeiten. | 09, 10, 11, 12 |
| Marken-/Visual-KI | Eigenständiges, rechtlich prüfbares Icon, Logo, Zug- und Assetsystem entwickeln. | 09, 10, 11 |
| Text-/Lore-KI | Vollständige trockene Bahnfahrer-Textbibliothek erstellen. | 09, 11, 14 |
| Leveldesign-KI | 240 konkrete Rätsel produzieren und gegen Solverregeln validieren. | 03, 04, 14 |
| Launch-/Marketing-KI | Store-, Trailer-, Meme- und Influencerpaket im bestätigten Stil entwickeln. | 09, 10, 11, 13 |

---

## 13. Empfohlener nächster Schritt

Der nächste große Block ist die **technische Produktionsspezifikation**. Sie beginnt mit dem Puzzle-Fundament: Leveldatenformat, Zell- und Werkzeugzustandsmodell, Solver, Eindeutigkeitsprüfung, Generatorvalidierung, Leveleditor und Teststrategie.

Erst wenn dieses Fundament festgelegt ist, sollten 240 konkrete Rätselfelder produziert, Zeitgrenzen kalibriert oder die Dauerbaustelle implementiert werden.
