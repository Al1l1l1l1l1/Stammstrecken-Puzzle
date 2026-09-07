# ADR-005 – Deterministisches Command/State-Modell für Puzzle-Sessions

## Status

**Angenommen**

## Datum

2026-09-07

## Kontext

Feldinhalte, gelbe Auswahl, Werkzeuge, Mehrfachauswahl, Undo, Timer und Lösungserkennung besitzen unterschiedliche Semantik. Versteckter Zustand in UI-Komponenten würde Tests, Savegames und agentenübergreifende Wartung gefährden.

## Entscheidung

`PuzzleSessionState` ist ein unveränderlicher Snapshot. Jede Spielerhandlung wird als expliziter Domainbefehl verarbeitet und ergibt entweder einen neuen Snapshot plus Domainereignisse oder eine unveränderte Ablehnung mit Diagnosecode.

Persistente Feldinhalte sind ausschließlich `Unset`, `EmptyAssumption`, `OccupiedAssumption` oder eine der sechs konkreten Gleisformen. Auswahl und aktives Werkzeug sind getrennte, flüchtige Präsentationszustände und niemals Teil des Feldinhalts. Eine Mehrfachanwendung ist ein einziger atomarer Befehl und ein einziger Undo-Schritt.

Der Timer beginnt mit dem ersten zustandsändernden Spielerbefehl. Er verwendet eine monotone Laufzeitquelle, zählt keine Hintergrundzeit und speichert nur bereits verstrichene aktive Millisekunden. `Undo` stellt den vorherigen Domainzustand wieder her, lässt den einmal gestarteten Timer jedoch laufen und nimmt keine bereits extern bestätigte Belohnung zurück.

Lösungserkennung läuft nach einem erfolgreichen Befehl und emittiert `PuzzleSolved` höchstens einmal je Versuch. Fortschritt und Belohnungen werden in der Application-Schicht idempotent aus diesem Ereignis abgeleitet; die Domain schreibt keine Dateien und ruft keine SDKs auf.

## Begründung

Commands und unveränderliche Snapshots machen jeden Übergang reproduzierbar. UI-Auswahl kann die Wahrheit des Rätsels nicht verfälschen. Atomare Batch-Befehle entsprechen der bestätigten Mehrfachauswahl und erzeugen verständliches Undo-Verhalten.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Mutable MonoBehaviours pro Zelle | Versteckter Zustand, schwer zu speichern und ohne Szene kaum testbar. |
| Event Sourcing als einzige Persistenz | Vollständig nachvollziehbar, aber unnötig komplex; begrenzte Undo-Diffs genügen. |
| Zustandsautomat nur in der UI | Vermischt Darstellung, Regeln und Persistenz. |
| Timer aus der Wanduhrdifferenz | Zählt Hintergrundzeit und reagiert falsch auf Uhränderungen. |

## Konsequenzen

Alle Eingaben müssen über den Command Handler laufen. Präsentation darf Zellen nicht direkt mutieren. Snapshots und Befehlsergebnisse benötigen stabile, serialisierbare Contracts.

## Betroffene Artefakte

`ARCHITECTURE/GAME_STATE_MODEL.md`, `ARCHITECTURE/PUZZLE_ENGINE.md`, `ARCHITECTURE/PERSISTENCE.md` und `ARCHITECTURE/TEST_STRATEGY.md`.

## Validierung

Property- und Replay-Tests wenden identische Befehlsfolgen auf identische Startzustände an und verlangen bytegleiche kanonische Snapshots sowie identische Ereignisse.

## Ersetzt / ersetzt durch

Ersetzt keinen früheren ADR. Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: ../Stammstrecken_Puzzle_Konzept_00-15/03_Raetselkern_und_Interaktionsmodell.md "Train Track Spiel – Rätselkern und Interaktionsmodell"
[2]: ../Stammstrecken_Puzzle_Konzept_00-15/12_UI_und_Bedienungsspezifikation.md "Stammstrecken-Puzzle – UI- und Bedienungsspezifikation"
