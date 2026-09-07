# Verbindliche Regeln für die KI-Übergabe

> **Kernregel:** Ein neuer Agent muss das Projekt vollständig aus Repository und Dokumentation übernehmen können, ohne einen vorherigen Chat gelesen zu haben.

Diese Regeln gelten für Manus, ChatGPT/Astra, Kimi und alle zukünftigen KI-Agenten gleichermaßen.

## 1. Übernahme vor Arbeitsbeginn

Der übernehmende Agent muss die Lesereihenfolge aus `AGENTS.md` vollständig einhalten. Dazu gehören insbesondere die Projektübergabe, der Konzeptindex, die Master-Spezifikation, der aktuelle Projektstand und das konkrete Work Package. Fachdateien sind zusätzlich zu lesen, wenn sie für den Scope des Work Packages relevant sind.

Vor Beginn prüft der Agent, ob Ziel, Voraussetzungen, Scope, verbotene Änderungen, Akzeptanzkriterien, Tests und Risikoklasse des Work Packages vollständig sind. Fehlt eine dieser Angaben oder widerspricht sie dem dokumentierten Projektstand, wird nicht geraten. Der Mangel wird als Blocker dokumentiert.

## 2. Verbindliche Arbeitsführung

Während der Bearbeitung arbeitet der Agent ausschließlich innerhalb des Work-Package-Scopes. Bestätigte Produktentscheidungen werden nicht verändert. Architekturentscheidungen werden nicht eigenmächtig getroffen oder geändert. Chat-Aussagen, Erinnerungen und nicht in Projektdateien festgehaltene Ergebnisse gelten nicht als belastbare Grundlage.

Ein Agent dokumentiert jede relevante Erkenntnis dort, wo das Work Package sie verlangt. Eine Architekturentscheidung wird nur nach dem ADR-System dokumentiert. Eine Produktentscheidung benötigt eine ausdrückliche Bestätigung und die Fortschreibung der zuständigen Fachdokumentation nach den geltenden Projektregeln.

## 3. Pflichtinhalt einer Übergabe

Beim Anhalten, Abschließen oder Wechseln eines Work Packages muss der übergebende Agent in den vorgesehenen Projektdateien mindestens die folgenden Informationen dokumentieren.

| Pflichtinformation | Ablage |
|---|---|
| Work-Package-ID, Bearbeitungsstatus und Ergebnis | Zugehörige Datei in `WORK_PACKAGES/`. |
| Erstellte oder geänderte Dateien und ihre Funktion | Zugehörige Datei in `WORK_PACKAGES/`; ergänzend dort, wo das Work Package es verlangt. |
| Durchgeführte Tests, Ergebnisse und noch nicht ausführbare Tests | Zugehörige Datei in `WORK_PACKAGES/`. |
| Abweichungen, bekannte Einschränkungen und offene Blocker | Zugehörige Datei in `WORK_PACKAGES/` und `PROJECT_CONTROL/CURRENT_STATE.md`. |
| Letzter abgeschlossener Schritt und nächster vorgesehener Schritt | `PROJECT_CONTROL/CURRENT_STATE.md`. |
| Architekturentscheidungen oder ausstehende Architekturfragen | Passender ADR in `DECISIONS/` oder klarer Blocker im Work Package; niemals nur im Chat. |

## 4. Abschlussübergabe

Ein Work Package wird erst als abgeschlossen übergeben, wenn seine Akzeptanzkriterien erfüllt und seine Definition of Done nachweisbar geprüft sind. Andernfalls bleibt der Status offen, blockiert oder in Bearbeitung. Der übergebende Agent darf einen unvollständigen Stand nicht als abgeschlossen darstellen.

Die Abschlussdokumentation muss so konkret sein, dass der nächste Agent ohne Rückfragen erkennen kann, was fertig ist, was nicht fertig ist, was unverändert bleiben muss und welche Arbeit als Nächstes ansteht.

## 5. Umgang mit Widersprüchen und Lücken

Bei einem Widerspruch zwischen Quellen gilt die in `AGENTS.md` definierte Quellenhierarchie. Bei einer Dokumentationslücke, einem fehlenden Work Package oder einer nicht bestätigten Entscheidung stoppt der Agent den betroffenen Teilbereich und dokumentiert den Blocker. Der Agent darf keine Architektur, Produktregel, Technologie, Priorität oder Implementierungsannahme eigenständig ergänzen.

## 6. Mindeststandard der Nachvollziehbarkeit

Eine Übergabe ist nur dann gültig, wenn sie ohne individuelle Modellkenntnis, ohne Zugang zu einem vorangegangenen Chat und ohne mündliche Ergänzung verständlich ist. Persistente Projektdateien sind daher stets maßgeblich; Chat-Kommunikation kann sie erläutern, aber nicht ersetzen.
