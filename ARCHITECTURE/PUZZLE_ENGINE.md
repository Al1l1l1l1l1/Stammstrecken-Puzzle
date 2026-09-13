# Puzzle Engine v0.3

## 1. Rolle

Die Puzzle Engine ist eine reine, deterministische C#-Domainbibliothek. Sie kennt keine Unity-Szene, kein Rendering, keine Datei, keine Werbung, keinen Store und keine Authoringlösung. Ihre öffentlichen Operationen sind total: Für jeden typgültigen Input liefern sie ein Ergebnis oder einen stabilen Diagnosecode, keine absichtlich ungefangene Ausnahme.

## 2. Domainwerte

| Typ | Vertrag |
|---|---|
| `GridSize` | Breite und Höhe mindestens 2; Runtime-Sicherheitsmaximum 32. |
| `CellCoordinate` | Ganzzahlige `x/y`, Existenz nur relativ zu `GridSize`. |
| `Direction` | geschlossenes Enum `N/E/S/W` mit deterministischer Gegenrichtung. |
| `TrackShape` | genau sechs Formen mit je exakt zwei unterschiedlichen Anschlüssen. |
| `Endpoint` | Seite plus Index; liefert genau eine angrenzende Innenzelle und Außenrichtung. |
| `PuzzleDefinition` | Raster, A/B und Zielzahlen; bei Konstruktion vollständig validiert. |
| `CellContent` | `UNSET`, zwei Markierungen oder genau eine konkrete Form. |
| `PuzzleSessionState` | immutable Snapshot mit monotoner Revision. |
| `PuzzleCommand` | explizite fachliche Handlung mit Command-ID und erwarteter Revision. |
| `CommandResult` | neuer/alter State, Ereignisse und sortierte Diagnosen. |

Primitive Strings und Integer werden an der Adaptergrenze in diese Typen übersetzt. Ungültige Domainobjekte können nicht konstruiert werden.

## 3. Invarianten der Definition

Eine `PuzzleDefinition` ist nur gültig, wenn:

1. A und B unterschiedliche Außenanschlüsse sind.
2. Jeder Endpointindex zur betreffenden Rasterachse passt.
3. `rowCounts.Length == height` und jeder Wert zwischen 0 und `width` liegt.
4. `columnCounts.Length == width` und jeder Wert zwischen 0 und `height` liegt.
5. Summe aller Zeilenwerte gleich Summe aller Spaltenwerte und mindestens 1 ist.
6. Die angrenzenden Zellen beider Endpoints laut Zahlen grundsätzlich belegt sein können.
7. die Ruleset-Version registriert ist.

A und B bleiben unterschiedliche Außenanschlüsse, können aber an dieselbe Rasterzelle grenzen. Diese Definition ist gültig, wenn mindestens eine der sechs Trackformen beide Außenrichtungen verbindet. Dadurch ist ein technisch gültiger Pfad aus genau einer Trackzelle möglich, ohne neue Gleisform oder Rätselregel.

Eindeutigkeit gehört nicht zur Konstruktion des Runtimeobjekts, sondern zum Content-Gate. Productionkataloge enthalten ausschließlich vorher eindeutig validierte Definitionen.

## 4. Command-Verarbeitung

```text
Handle(currentState, command, monotonicTime)
  -> validate command envelope
  -> compute proposed atomic cell diff
  -> if no-op: return unchanged state, no events
  -> apply diff to immutable cells
  -> update sticky usage/correction flags
  -> start/accumulate timer if applicable
  -> evaluate objective diagnostics from visible state
  -> evaluate exact completion from concrete tracks
  -> increment revision
  -> emit ordered domain events
```

Befehle werden nicht teilweise angewendet. Bei einem Batch mit einer ungültigen Koordinate bleibt der gesamte Zustand unverändert. Diagnosen sind nach Koordinate, dann Code sortiert.

## 5. Sichtbare objektive Diagnosen

Die Engine darf unmittelbar Fakten melden, die allein aus dem aktuellen sichtbaren Zustand folgen.

| Diagnose | Bedingung |
|---|---|
| `ROW_COUNT_EXCEEDED` | konkrete Tracks plus graue Belegungsannahmen übersteigen eine Zeilenzahl. |
| `COLUMN_COUNT_EXCEEDED` | entsprechende Spaltenbedingung. |
| `TRACK_EXITS_GRID` | konkrete Form hat einen Außenanschluss ohne A/B an exakt dieser Kante. |
| `TRACK_CONNECTION_MISMATCH` | zwei benachbarte konkrete Gleise stimmen an ihrer gemeinsamen Kante nicht überein. |
| `PREMATURE_CONCRETE_LOOP` | konkrete Gleise bilden bereits eine geschlossene Komponente. |
| `ENDPOINT_MISMATCH` | konkrete Endpointzelle kann den Außenanschluss nicht bedienen. |

Die Anzeige „exakt erfüllt“ für eine Linie bedeutet nur, dass die **vom Spieler aktuell als belegt dargestellte Anzahl** der Zielzahl entspricht. Sie ist kein Beweis, dass die Positionen korrekt sind.

Ein einzelnes konkretes Gleis mit Anschluss zu einer noch unbestimmten Nachbarzelle ist nicht automatisch falsch. Nur ein Widerspruch zu bereits konkretem Nachbar, Rastergrenze oder Endpoint wird gemeldet.

## 6. Exakte Completion-Prüfung

Ein Puzzle ist genau dann gelöst, wenn alle folgenden Schritte wahr sind:

1. Es existieren keine `MARK_OCCUPIED`-Zellen auf dem finalen Pfad und keine unaufgelösten konkreten Anschlussenden.
2. Die Zahl konkreter `TRACK_*`-Zellen je Zeile und Spalte entspricht exakt den Zielwerten.
3. Jede konkrete Trackzelle hat Grad zwei im Graph aus passenden Zell-/Endpointanschlüssen.
4. A und B haben jeweils Grad eins und verbinden mit ihrer angrenzenden Zelle.
5. Eine Traversierung ab A erreicht B.
6. Die Traversierung besucht jede konkrete Trackzelle genau einmal; bei identischer A-/B-Nachbarzelle ist dies genau ein Knoten.
7. Es gibt keine zweite konkrete Komponente, Schleife, Kreuzung oder Verbindung zu einer falschen Außenkante.

`MARK_EMPTY`, `MARK_OCCUPIED` und `UNSET` außerhalb der konkreten Strecke beeinflussen die Gültigkeit nicht. Hilfsmarkierungen dürfen nach Lösung visuell zurücktreten; ihr Vorhandensein entwertet den Abschluss nicht.

Die Completion-Prüfung vergleicht niemals mit `solution.path` aus Authoringdaten.

## 7. Graphmodell

Jede konkrete Trackzelle ist ein Knoten. Eine Kante existiert nur, wenn zwei orthogonal benachbarte Formen wechselseitig aufeinander zeigen. A und B sind externe Knoten. Da jede Trackform zwei Ports besitzt und A/B je einen Port haben, ist der gültige Gesamtgraph ein einfacher Pfad. Der kleinste gültige Graph ist `A -> eine Trackzelle -> B`; beide externen Kanten dürfen dieselbe Rasterzelle treffen.

Crossings und T-Knoten sind konstruktiv ausgeschlossen, weil keine erlaubte Form mehr als zwei Ports hat. Eine Schleife wird über Union-Find während inkrementeller Diagnostik oder über die abschließende Traversierung erkannt. Die abschließende Prüfung bleibt autoritativ.

## 8. Undo und Revisionen

Ein `CellDiff` speichert sortierte Tupel `(coordinate, before, after)`. Der Stack enthält maximal 256 atomare Nutzerhandlungen. Beim Überschreiten wird der älteste Diff verworfen; der aktuelle Zustand bleibt vollständig.

Undo erzeugt einen neuen Snapshot mit höherer Revision. Es setzt Zellen zurück, aber nicht `timerStarted`, `usedEmptyMarker`, `usedOccupiedMarker`, `hintCount` oder bereits verbuchte externe Ergebnisse. Redo ist für Architecture v0.3 nicht Teil des bestätigten Produkts und wird nicht implizit eingeführt.

## 9. Hintschnittstelle

Die Domain stellt keine „zeige richtige Lösung“-Methode bereit. Im normalen Hintmodus berechnet `STP.Puzzle.Solver` sichere Deduktionen ausschließlich aus `PuzzleDefinition`, also dem öffentlichen Puzzle ohne X-, Grau- oder konkrete Spielerannahmen. Der sichtbare Zustand dient danach nur als Auswahlfilter für einen nicht enttarnenden Schritt auf einer geeigneten unberührten Zelle. Application wählt nach der noch blockierten Produktpolicy genau einen Schritt. Erst nach erfolgreicher Darstellung werden Creditverbrauch und `HintPresented` idempotent verbucht.

Ein normaler Hint darf weder einen aus Spielerannahmen abgeleiteten Widerspruch erklären noch die Falschheit einer plausiblen Markierung oder Schiene offenlegen. Ist kein aus dem öffentlichen Puzzle beweisbarer, nicht enttarnender Schritt verfügbar, lautet das Ergebnis `NO_NON_REVEALING_HINT_AVAILABLE`. Objektiv sichtbare Verstöße meldet ausschließlich die Domain-Diagnostik. Ein Hint darf keine Gesamtlösung offenlegen. Credit-Lebensdauer und Modusbezug bleiben wegen `BLOCKER-PROD-001` fail-closed; die Portgrenze verhindert eine Vorentscheidung.

## 10. Determinismusvertrag

Für identische Definition, identischen Snapshot, identischen Command und identischen monotonen Zeitwert entstehen identischer nächster Snapshot, identische Diagnosen und identische Ereignisreihenfolge. Es gibt keine Verwendung von Wanduhr, Zufall, HashMap-Iterationsreihenfolge, Unity Frame Time oder Locale im Domainkern.

Collections werden vor Ausgabe explizit sortiert. Hashing verwendet den in `LEVEL_DATA_FORMAT.md` beschriebenen Vertrag. Zufall existiert nur in einem Generatoradapter mit explizitem Seed und nie in Completion oder Solverproof.

## 11. Fehlerbehandlung

Programmierfehler in intern bewiesenen Invarianten dürfen in Development mit Assertions stoppen. Nutzer-, Content- und Adapterinput liefert typisierte Fehler. Production zeigt eine lokalisierte, sichere Oberfläche und schreibt nur den Diagnosecode plus nicht sensible Dimensionen.

Wichtige Codes sind `INVALID_DEFINITION`, `OUT_OF_BOUNDS`, `STALE_COMMAND`, `SESSION_CLOSED`, `UNSUPPORTED_RULESET`, `CONTENT_REVISION_MISMATCH` und `INTERNAL_INVARIANT_BROKEN`.

## 12. Performancebudgets

Für Season-1-Raster bis 10×10 gilt als Ziel auf dem niedrigsten unterstützten Referenzgerät:

| Operation | Budget |
|---|---:|
| Einzel-/Batchcommand einschließlich Diagnostik | p95 unter 4 ms auf Hauptthread |
| Completion-Prüfung | p95 unter 2 ms |
| Snapshotkanonisierung für Autosave | unter 5 ms CPU, Dateischreiben asynchron |
| GC-Allokation pro wiederholter Einzelaktion | nach Warm-up 0 B im Presentation-Hotpath; Domainbudget wird profiliert und begrenzt |

Budgets sind Qualitätsgates, keine Produktzeitwerte. Bei Überschreitung wird zuerst gemessen und vereinfacht; keine verdeckte Parallelität verändert die Zustandsreihenfolge.

## 13. Tests

Pflicht sind Beispiel-, Property- und Metamorphic-Tests für alle Formen, Rotationen/Spiegelungen, Randanschlüsse, Zahlen, Ein-Zellen-A-B-Pfade, Schleifen, getrennte Komponenten, Markierungssemantik, Batchatomarität, No-op, Undo und Completion. Mutationen jeder einzelnen Regel müssen mindestens einen Test brechen.

## Referenzen

[1]: ../Stammstrecken_Puzzle_Konzept_00-15/03_Raetselkern_und_Interaktionsmodell.md "Train Track Spiel – Rätselkern und Interaktionsmodell"
[2]: ./GAME_STATE_MODEL.md "Game State Model v0.3"
[3]: ./LEVEL_DATA_FORMAT.md "Level Data Format v0.3"
[4]: ../DECISIONS/ADR-005-deterministisches-command-state-modell.md "ADR-005 – Deterministisches Command/State-Modell"
