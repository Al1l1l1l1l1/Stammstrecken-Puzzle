# Train Track Spiel – Rätselkern und Interaktionsmodell

**Status:** Aktuelle, bestätigte Baseline des Rätselspiels  
**Einordnung:** Konzeptbaustein 03  
**Zweck:** Diese Datei beschreibt den tatsächlichen Ingame-Lösungsprozess des Referenzspiels, einschließlich aller Werkzeuge, Feldinhalte und der davon getrennten Auswahlhervorhebung. Sie ersetzt frühere, unzutreffende Annahmen über Pflichtschritte, eine konkrete graue Gleisform oder eine Bestätigungslogik für Platzierungen.

## 1. Kern des Rätsels

Ein Level besteht aus einem rechteckigen Raster mit A und B als äußeren Anschlusspunkten. Die Zahlen oberhalb jeder Spalte und neben jeder Zeile geben an, wie viele Rasterfelder dieser Spalte beziehungsweise Zeile Schienen enthalten müssen.

Ziel ist eine einzige, durchgehende Schienenstrecke zwischen A und B. Schienen dürfen sich nicht kreuzen und keine Schleife bilden. Die Lösung wird aus zwei logisch zusammenhängenden Fragen aufgebaut:

| Denkebene | Frage |
|---|---|
| Belegung | Welche Felder sind leer und welche enthalten irgendeine Schiene? |
| Geometrie | Welche konkrete der sechs Schienenformen liegt in jedem belegten Feld? |

Diese Ebenen können ineinandergreifen, werden von erfahrenen Spielern aber nicht zwingend nacheinander bearbeitet.

## 2. Feldinhalte

Ein Rasterfeld kann die folgenden Inhalte haben. Die Darstellung eines Feldinhalts ist von der gelben Auswahlhervorhebung zu unterscheiden.

| Feldinhalt | Darstellung | Bedeutung |
|---|---|---|
| Unberührt / leer | Leeres Rasterfeld | Es wurde noch keine Notiz und keine Schiene gesetzt. |
| Leer-Markierung | Rotes X | Der Spieler hält dieses Feld für leer beziehungsweise schließt es nach seiner aktuellen Logik aus. |
| Belegungsmarkierung | Kleines ausgegrautes Standardgleis | Der Spieler hält es für wahrscheinlich oder sicher, dass dieses Feld eine Schiene enthalten muss. Die konkrete Richtung und Form ist offen. |
| Konkrete Schiene | Braune Gleisform | Der Spieler hat eine konkrete Schienenform in diesem Feld gesetzt. |

Die roten X und grauen Belegungsmarkierungen sind **optionale Hilfsmarkierungen des Spielers**. Sie sind keine festen Vorgaben des Levels und keine Pflichtphase. Ein Spieler kann sie umfangreich zur Planung einsetzen, punktuell verwenden oder vollständig überspringen.

## 3. Die sechs konkreten Gleisformen

Braune Gleise bilden die tatsächliche Strecke. Es gibt genau sechs Formen:

| Gleisform | Verbindungen |
|---|---|
| Senkrechte Gerade | Oben ↔ unten |
| Waagerechte Gerade | Links ↔ rechts |
| Kurve oben–rechts | Oben ↔ rechts |
| Kurve rechts–unten | Rechts ↔ unten |
| Kurve unten–links | Unten ↔ links |
| Kurve links–oben | Links ↔ oben |

Die Farbe Braun kennzeichnet den Gleisinhalt. Sie ist nicht an einen Auswahl- oder Bestätigungsstatus gekoppelt.

## 4. Werkzeuge des Spielers

| Werkzeug | Wirkung |
|---|---|
| Rotes X | Setzt eine Leer-Markierung auf das ausgewählte Feld oder die ausgewählten Felder. |
| Graues Standardgleis | Setzt eine Belegungsmarkierung: Hier soll irgendeine Schiene liegen, die Form bleibt offen. |
| Sechs braune Gleisformen | Setzt die gewählte konkrete Gerade oder Kurve auf das ausgewählte Feld oder die ausgewählten Felder. |
| Leeres Feld / Entfernen | Entfernt eine vorhandene X-Markierung, graue Belegungsmarkierung oder braune Schiene vollständig; das Feld wird wieder unberührt. |
| Mehrfachauswahl | Erlaubt, mehrere Felder zu wählen und dieselbe gewählte Markierung oder Gleisform effizient auf sie anzuwenden. |
| Rückgängig | Macht die letzte Spielerhandlung rückgängig. |
| Hinweis | Unterstützt den Spieler bei Bedarf; die genaue Wirkung wird erst im Fortschritts- und Hinweissystem festgelegt. |

## 5. Gelb ist kein Feldinhalt

Gelb ist eine reine **Auswahlhervorhebung der Benutzeroberfläche**. Es zeigt, welches Werkzeug oder welche Felder der Spieler aktuell angewählt hat. Die gelbe Markierung ist unabhängig davon, ob ein Feld leer, mit X markiert, grau als belegt notiert oder mit einer braunen Schiene gefüllt ist.

> **Braun beschreibt eine konkrete Schiene. Grau beschreibt eine offene Belegungsannahme. Rot beschreibt eine Leerannahme. Gelb beschreibt ausschließlich die aktuelle Auswahl.**

Diese Trennung ist für jede spätere Gestaltung verbindlich. Eine neue Oberfläche darf die Farben und Formen hochwertiger ausführen, darf ihre unterschiedlichen Bedeutungen aber nicht vermischen.

## 6. Der praktische Lösungsweg

Der Spieler kann das Puzzle auf unterschiedliche Weise lösen. Ein typischer systematischer Ablauf nutzt die Hilfen in der folgenden Reihenfolge:

| Schritt | Logischer Schluss | Mögliche Aktion |
|---:|---|---|
| 1 | Eine Zeile oder Spalte mit `0` enthält keine Schiene. | Alle Felder dieser Linie mit X markieren. |
| 2 | Eine Zahl entspricht genau der Menge aller verfügbaren Felder einer Zeile oder Spalte. | Alle diese Felder mit grauen Belegungsmarkierungen versehen. |
| 3 | A und B sind Ein- und Ausgänge. | Die angrenzenden Rasterfelder müssen belegt sein und können grau markiert oder direkt mit einer konkreten Schiene versehen werden. |
| 4 | Eine Zeile oder Spalte hat ihre erforderliche Zahl an belegten Feldern erreicht. | Alle übrigen Felder dieser Linie mit X markieren. |
| 5 | Neue X und graue Belegungsmarkierungen verändern die verfügbaren Felder kreuzender Linien. | Schritte 2 und 4 wiederholen, bis weitere sichere Schlüsse entstehen. |
| 6 | Ein belegtes Eckfeld ohne A oder B kann nicht nach außen weiterführen. | Die konkrete Form ist dort zwangsläufig eine passende Kurve. |
| 7 | Nachbarn, Randlage und die Verbindung zu A/B begrenzen die Form der belegten Felder. | Graue Belegungsmarkierungen durch passende braune Geraden oder Kurven ersetzen. |

Dieser Ablauf ist ein hilfreiches Verfahren, aber keine Nutzervorgabe. Geübte Spieler können sichere konkrete Gleise direkt setzen, mehrere Schlussketten parallel erkennen und X- oder graue Markierungen ganz oder teilweise weglassen.

## 7. Gleichwertige Spielstile

Es gibt keine getrennten Modi für Anfänger und Fortgeschrittene. Jedes Level erlaubt dieselbe vollständige Werkzeugpalette und dieselbe Ziellösung.

| Spielstil | Verhalten |
|---|---|
| Strukturiert | Nutzt X und graue Belegungsmarkierungen als externe Notizen, um die Belegung vor der Geometrie einzugrenzen. |
| Gemischt | Setzt offensichtliche braune Gleise direkt und nutzt Hilfsmarkierungen nur bei unsicheren Bereichen. |
| Direkt / schnell | Erkennt Belegung und Geometrie so schnell, dass konkrete braune Gleise ohne Hilfsmarkierungen gesetzt werden können. |

Der direkte Spielstil ist keine Regelverletzung, sondern ein sichtbares Zeichen wachsender Meisterschaft. Künftige Leistungs- und Belohnungssysteme dürfen dies würdigen, dürfen aber die Nutzung von Hilfsmarkierungen niemals als minderwertiges Spielen bestrafen.

## 8. Zielzustand

Ein Level ist gelöst, wenn A und B durch eine zusammenhängende, kreuzungsfreie Strecke verbunden sind, jede Randzahl erfüllt ist und keine Schleife vorliegt. Ob der Spieler dabei viele Hilfsmarkierungen genutzt oder direkt Gleise gesetzt hat, verändert die Gültigkeit der Lösung nicht.

## 9. Noch nicht entschiedene Produktfragen

Die folgenden Punkte werden bewusst erst in späteren Konzeptbausteinen festgelegt:

| Offen | Grund |
|---|---|
| Exakte visuelle Gestaltung von Raster, X, grauen Markierungen und Schienen | Gehört zum Erlebnis- und Markenbild. |
| Konkrete Touch-Gesten und Werkzeuganordnung | Gehört zur UI- und Bedienausarbeitung. |
| Art und Menge der Hinweise | Hängt von Fortschritt, Währung und fairer Monetarisierung ab. |
| Sterne, Zeitwertung und Anerkennung direkter Lösungen | Gehört zum Fortschritts- und Belohnungssystem. |
| Zugfahrt, Weltreaktion und Abschlussinszenierung | Gehört zum visuellen Erlebnisdesign. |
| Zusätzliche Sonderfelder oder neue Rätselregeln | Werden nur eingeführt, wenn sie den etablierten Kern nachweisbar verbessern. |

## 10. Nächster Konzeptschritt

Als Nächstes wird festgelegt, welche Elemente des Referenzspiels unverändert bleiben und welche **gezielten Qualitätsverbesserungen** unser Spiel erhält, ohne die optionale, frei wählbare Denkweise des Spielers zu stören.
