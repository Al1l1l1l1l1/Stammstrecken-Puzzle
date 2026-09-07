# Verbindliche Agentenleitlinie

Dieses Dokument ist die **verbindliche Startanweisung** für jeden KI-Agenten, der am Projekt **Stammstrecken-Puzzle** arbeitet. Es stellt sicher, dass Manus, ChatGPT/Astra, Kimi und künftige Systeme denselben dokumentierten Projektstand verwenden und nicht auf einen früheren Chat angewiesen sind.

## Verbindliche Lesereihenfolge vor jeder Arbeit

1. Zuerst diese Datei, `AGENTS.md`, vollständig lesen.
2. Danach `Stammstrecken_Puzzle_Konzept_00-15/15_Projektuebergabe_und_Gesamtstatus.md` vollständig lesen.
3. Danach `Stammstrecken_Puzzle_Konzept_00-15/00_Train_Track_Konzeptindex.md` vollständig lesen.
4. Danach `Stammstrecken_Puzzle_Konzept_00-15/09_Train_Track_Master_Spezifikation.md` vollständig lesen.
5. Danach `PROJECT_CONTROL/CURRENT_STATE.md` und das konkrete Work Package in `WORK_PACKAGES/` vollständig lesen.
6. Weitere Fachdateien nur lesen, soweit sie für das konkrete Work Package erforderlich sind. Der Konzeptindex und die Projektübergabe benennen die jeweils passenden Fachdateien.

Mit der Bearbeitung darf erst begonnen werden, wenn diese Lesereihenfolge abgeschlossen ist. Existiert noch kein konkretes Work Package, darf kein Agent eigenständig einen Umsetzungsauftrag ableiten oder außerhalb eines ausdrücklich freigegebenen Scopes arbeiten.

## Verbindliche Arbeitsregeln

| Regel | Verbindliche Anwendung |
|---|---|
| Dokumentierter Stand | Repository und Projektdateien sind der persistente Projektstand. Informationen, die nur in einem Chat vorkommen, gelten nicht als persistenter Projektstand. |
| Produktentscheidungen | Bereits bestätigte Produktentscheidungen dürfen nicht stillschweigend verändert werden. Eine Änderung erfordert eine neue ausdrückliche Produktentscheidung und die dokumentierte Fortschreibung der zuständigen Fachdateien. |
| Architekturentscheidungen | Architekturentscheidungen dürfen nach ihrer Annahme nicht eigenmächtig verändert werden. Sie sind nach dem ADR-Verfahren in `DECISIONS/README.md` zu dokumentieren und dürfen nur durch eine ausdrücklich dokumentierte, nachfolgende Entscheidung ersetzt werden. |
| Scope-Treue | Jeder Agent arbeitet ausschließlich innerhalb seines konkreten Work-Package-Scopes. Beiläufige „Verbesserungen“, Refactorings oder Inhaltsänderungen außerhalb dieses Scopes sind nicht erlaubt. |
| Unklare Informationen | Was nicht dokumentiert oder ausdrücklich entschieden ist, wird nicht geraten und nicht entschieden. Der Agent dokumentiert die Lücke als Blocker oder Rückfrage im dafür vorgesehenen Projektstand. |
| Abschlussdokumentation | Jede abgeschlossene Arbeit wird in den im Work Package vorgegebenen Projektdateien dokumentiert. Mindestens der Work-Package-Status und `PROJECT_CONTROL/CURRENT_STATE.md` sind bei einem Abschluss zu aktualisieren. |
| Qualitätsnachweis | Eine technische Aufgabe gilt nur nach `PROJECT_CONTROL/DEFINITION_OF_DONE.md` als abgeschlossen. |

## Verbindliche Quellenhierarchie

Bei einem Widerspruch gilt die folgende Reihenfolge, soweit eine höherstehende Quelle ausdrücklich zur konkreten Frage Stellung nimmt:

1. Eine neue, ausdrücklich bestätigte Produktentscheidung.
2. Eine angenommene, dokumentierte Architekturentscheidung im ADR-System für technische Fragen.
3. Die maßgebliche Master-Spezifikation und die dort nachgewiesenen, später bestätigten Fachdateien.
4. Die Projektübergabe und der Konzeptindex als Steuerungs- und Navigationsdokumente.
5. Der aktuelle Status und das konkrete Work Package für Ausführungsstand und Auftragsgrenzen.

Ein vermuteter Widerspruch ist vor der Umsetzung als Blocker zu dokumentieren. Er darf nicht durch eine eigene Interpretation aufgelöst werden.

## Abschluss- und Übergabepflicht

Vor dem Ende eines Arbeitsschritts prüft jeder Agent die Akzeptanzkriterien und die Definition of Done seines Work Packages. Der Agent dokumentiert Ergebnis, Validierung, offene Punkte und den nächsten vorgesehenen Schritt so, dass ein neuer Agent ohne Chat-Verlauf übernehmen kann. Der verbindliche Ablauf steht in `PROJECT_CONTROL/AI_HANDOVER_RULES.md`.

## Projektquellen

| Datei | Rolle |
|---|---|
| `Stammstrecken_Puzzle_Konzept_00-15/15_Projektuebergabe_und_Gesamtstatus.md` | Bestätigter Gesamtstand und bekannte Produktionsblöcke. |
| `Stammstrecken_Puzzle_Konzept_00-15/00_Train_Track_Konzeptindex.md` | Verzeichnis der maßgeblichen Fachfassungen. |
| `Stammstrecken_Puzzle_Konzept_00-15/09_Train_Track_Master_Spezifikation.md` | Strategische Gesamtfassung und bestätigte Leitplanken. |
| `PROJECT_CONTROL/` | Persistente Steuerung des laufenden Arbeitsstands. |
| `DECISIONS/` | Künftige dokumentierte Architekturentscheidungen. |
