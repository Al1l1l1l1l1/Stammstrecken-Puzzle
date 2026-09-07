# Definition of Done für technische Aufgaben

Eine technische Aufgabe ist erst abgeschlossen, wenn **alle** nachstehenden Bedingungen erfüllt und im zugehörigen Work Package nachweisbar dokumentiert sind. Eine teilweise Implementierung, ein ungeprüfter Prototyp oder ein nur im Chat bestätigter Stand gilt nicht als abgeschlossen.

| Abschlussbedingung | Verbindlicher Nachweis |
|---|---|
| Implementierung vollständig | Der im Work Package definierte Scope ist vollständig umgesetzt. Es bestehen keine bekannten, nicht als Blocker ausgewiesenen Teilergebnisse innerhalb des Scopes. |
| Vorgeschriebene Tests vorhanden | Alle im Work Package festgelegten Tests, Prüfschritte und Testdaten sind vorhanden oder eine Nichtanwendbarkeit ist vorab begründet und akzeptiert. |
| Tests erfolgreich | Die vorgeschriebenen Tests sind erfolgreich ausgeführt. Ergebnis, Ausführungsweg und verbleibende Einschränkungen sind nachvollziehbar dokumentiert. |
| Projekt baut | Der im Work Package definierte Build- oder Ausführbarkeitsnachweis ist erfolgreich. Ist in der aktuellen Projektphase noch kein Buildsystem festgelegt, wird dies ausdrücklich als nicht anwendbar dokumentiert; daraus folgt kein Recht, ein Buildsystem eigenmächtig zu entscheiden. |
| Akzeptanzkriterien erfüllt | Jedes Akzeptanzkriterium des Work Packages ist einzeln geprüft und erfüllt. |
| Dokumentation aktualisiert | Alle vom Work Package vorgegebenen technischen, fachlichen und Bedienungsdokumente entsprechen dem Ergebnis. |
| CURRENT_STATE aktualisiert | `PROJECT_CONTROL/CURRENT_STATE.md` benennt den abgeschlossenen Schritt, den nächsten vorgesehenen Schritt und eventuelle Blocker. |
| Keine undokumentierte Architekturänderung | Jede Architekturentscheidung ist entweder unverändert geblieben oder vor der Umsetzung per akzeptiertem ADR dokumentiert. |

## Zusätzliche Abschlussregeln

1. Eine Abweichung vom Scope, von bestätigten Produktentscheidungen oder von einer akzeptierten Architekturentscheidung verhindert den Abschluss, bis sie ausdrücklich entschieden und dokumentiert wurde.
2. Fehlende Zugänge, ungeklärte Anforderungen oder nicht ausführbare Tests sind als Blocker zu dokumentieren. Sie dürfen nicht durch angenommene Werte, stillschweigende Ausnahmen oder nicht belegte Erfolgsmeldungen ersetzt werden.
3. Ein Work Package ist erst dann übergabefähig, wenn ein neuer Agent das Ergebnis, die Nachweise und die offenen Restpunkte allein aus Repository und Dokumentation nachvollziehen kann.
4. Der Abschlussstatus wird im Work Package festgehalten. Der projektweite Kurzstand wird zusätzlich in `PROJECT_CONTROL/CURRENT_STATE.md` aktualisiert.
