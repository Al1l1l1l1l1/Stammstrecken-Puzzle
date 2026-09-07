# Train Track Spiel – Referenzspiel und bestätigte Baseline

**Status:** Aktuelle, bestätigte Referenzbeschreibung  
**Zweck:** Diese Datei hält ausschließlich fest, wie das Vorbildspiel tatsächlich funktioniert. Sie ist die verbindliche Ausgangsbasis für alle späteren Konzeptentscheidungen und enthält bewusst keine Produktannahmen für unser eigenes Spiel.

## 1. Der tatsächliche Rätselkern

Das Vorbild ist ein Raster-Logikpuzzle mit Gleiskacheln. Ein Level besteht aus einem rechteckigen Raster, etwa in kleineren Formaten wie 4×5 und später deutlich größeren Formaten bis ungefähr 10×10 oder darüber. An zwei äußeren Rasterkanten liegen die Anschlusspunkte A und B.

Der Spieler füllt das Raster so, dass eine einzige zusammenhängende Schienenstrecke von A nach B entsteht. Jedes Feld enthält entweder keine Schiene oder genau einen Abschnitt. Die Strecke darf sich nicht kreuzen und keine Schleife bilden.

## 2. Zahlenhinweise

An der oberen Kante des Rasters steht für jede Spalte eine Zahl. An einer Seitenkante steht für jede Zeile eine Zahl. Diese Zahlen legen fest, wie viele Felder der jeweiligen Spalte beziehungsweise Zeile Schienen enthalten müssen.

Die Randzahlen, die Verbindung zwischen A und B sowie die Regeln gegen Kreuzungen und Schleifen greifen ineinander. Das Denkspiel besteht darin, schrittweise herauszufinden, welche Felder belegt und welche leer sein müssen – und erst danach beziehungsweise parallel, welche konkrete Gleisform jeweils passt.

## 3. Sechs konkrete Gleisformen

| Gleisform | Verbindung im Feld |
|---|---|
| Senkrechte Gerade | Oben ↔ unten |
| Waagerechte Gerade | Links ↔ rechts |
| Kurve oben–rechts | Oben ↔ rechts |
| Kurve rechts–unten | Rechts ↔ unten |
| Kurve unten–links | Unten ↔ links |
| Kurve links–oben | Links ↔ oben |

Die tatsächlich gesetzten Gleise sind braun dargestellt. Sie bilden die aktuell vom Spieler gewählte Streckenform auf dem Raster.

## 4. Spielerische Hilfsmarkierungen

Die roten X und das kleine ausgegraute Standardgleis sind keine festen Vorgaben eines Levels. Sie sind optionale, vom Spieler gesetzte Denkwerkzeuge.

| Feldinhalt | Bedeutung |
|---|---|
| Leeres/unberührtes Feld | Der Spieler hat noch keine Aussage getroffen oder hat einen Inhalt vollständig entfernt. |
| Rotes X | Der Spieler vermutet beziehungsweise markiert: Dieses Feld bleibt leer. |
| Ausgegrautes kleines Standardgleis | Der Spieler vermutet beziehungsweise weiß: Dieses Feld muss irgendeine Schiene enthalten. Die konkrete Richtung und Form ist noch offen. |
| Braune konkrete Gleisform | Der Spieler hat eine der sechs Gleisformen auf das Feld gesetzt. |

Die Hilfsmarkierungen sind kein Pflichtablauf. Ein Spieler kann sie für strukturiertes Planen verwenden, punktuell einsetzen oder vollständig überspringen und konkrete braune Gleise direkt setzen.

## 5. Gelb ist nur ein Auswahlzustand

Gelb beschreibt keinen Feldinhalt und keine besondere Gleisform. Es ist ausschließlich die Benutzeroberflächen-Hervorhebung für das aktuell ausgewählte Werkzeug oder die aktuell ausgewählten Felder. Diese Hervorhebung ist unabhängig davon, ob ein Feld leer, mit X markiert, grau als belegt notiert oder mit einer braunen Schiene gefüllt ist.

> **Braun = konkrete Schiene. Grau = offene Belegungsannahme. Rot = Leerannahme. Gelb = aktuelle Auswahl.**

## 6. Werkzeugleiste und Bedienoptionen

Die Werkzeugleiste enthält mindestens die sechs konkreten Gleisformen, die rote Leer-Markierung, das ausgegraute Standardgleis, ein leeres Feld zum vollständigen Entfernen bestehender Inhalte, eine Mehrfachauswahl sowie Undo und Hinweise.

| Werkzeug | Zweck |
|---|---|
| Rotes X | Ein oder mehrere Felder als voraussichtlich leer markieren. |
| Graues Standardgleis | Ein oder mehrere Felder als belegt markieren, ohne konkrete Form festzulegen. |
| Sechs braune Gleisformen | Gewählte konkrete Gleisform auf einem oder mehreren Feldern setzen. |
| Leeres Feld / Entfernen | Vorhandenes X, graues Standardgleis oder braune Schiene vollständig entfernen. |
| Mehrfachauswahl | Dieselbe gewählte Markierung oder Gleisform effizient auf mehrere Felder anwenden. |
| Undo | Die letzte Spielerhandlung zurücknehmen. |
| Hinweis | Bei Bedarf zusätzliche Unterstützung anfordern. |

## 7. Typischer menschlicher Lösungsweg

Ein systematischer Spieler kann zunächst alle Zeilen oder Spalten mit `0` vollständig mit X markieren. Danach werden Felder als belegt markiert, wenn eine Randzahl exakt der Zahl der verfügbaren Felder entspricht oder wenn sie direkt an A beziehungsweise B liegen.

Sobald eine Zeile oder Spalte ihre erforderliche Zahl an belegten Feldern erreicht hat, werden die übrigen verfügbaren Felder dieser Linie mit X ausgeschlossen. Diese Erkenntnisse übertragen sich auf kreuzende Zeilen und Spalten und erzeugen neue sichere Schlüsse. Erst in der geometrischen Verdichtung werden aus grauen Belegungsmarkierungen konkrete braune Geraden und Kurven.

Eckfelder, die belegt sind und weder A noch B enthalten, müssen beispielsweise zwangsläufig eine passende Kurve sein, weil die Strecke nicht aus dem Raster herauslaufen darf. Geübte Spieler können solche Schlüsse direkt nutzen und die Hilfsmarkierungen überspringen.

## 8. Content-Struktur des Vorbilds

Das Vorbild bietet eine große Folge vorgefertigter Level über mehrere Schwierigkeitsstufen. Feldgröße und Komplexität wachsen. Nach Abschluss der vorgefertigten Inhalte sind zufallsgenerierte Level verfügbar, offenbar im Zusammenhang mit Werbung.

| Content-Baustein | Im Vorbild vorhanden |
|---|---|
| Vorgefertigte Kampagnenlevel | Ja |
| Mehrere Feldgrößen | Ja |
| Mehrere Schwierigkeitsstufen | Ja |
| Undo und Hinweise | Ja |
| Zufallsgenerierte Level nach Kampagnenabschluss | Ja |
| Werbezugang für zusätzliche Inhalte | Ja |
| Sichtbar fahrende Zugfahrzeuge | Nein |

## 9. Bestätigte Abgrenzung für unser Spiel

Das Vorbild zeigt abstrakte Gleislogik auf weißem Raster. Es zeigt keine sichtbaren Zugfahrzeuge, keine ausgeprägte Weltinszenierung und keine emotionale Auswertung der aufgebauten Strecke.

Der Mehrwert unseres Spiels liegt nicht darin, die Grundlogik willkürlich auszutauschen. Er liegt darin, die freie Denkhandlung klarer und hochwertiger zu gestalten und aus einer korrekt geplanten Strecke ein stärkeres Erlebnis zu machen: sichtbares Leben auf der fertig gebauten Strecke, nachvollziehbarer Fortschritt, persönliche Sammlung und faire Langzeitmotivation.
