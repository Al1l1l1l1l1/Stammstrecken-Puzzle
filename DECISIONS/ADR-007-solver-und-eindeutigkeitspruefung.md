# ADR-007 – Deterministischer Constraint-Solver mit Lösungslimit zwei

## Status

**Angenommen**

## Datum

2026-09-07

## Kontext

Jedes kuratierte und generierte Level muss gültig und exakt eindeutig sein. Der Solver soll außerdem reproduzierbare Schwierigkeitssignale und erklärbare Deduktionsspuren liefern. Laufzeit-UI und Unity-Physik dürfen den Nachweis nicht beeinflussen.

## Entscheidung

`STP.Puzzle.Solver` implementiert einen deterministischen Constraint-Satisfaction-Solver über der reinen Domain. Jede Zelle besitzt zunächst eine Teilmenge aus `EMPTY` und sechs Gleisformen. Propagation verarbeitet Randzahlen, Rastergrenzen, Endpunkte, Anschlusskonsistenz, Zellgrad, Zyklusverbot und mögliche globale A-B-Verbindung.

Wenn Propagation nicht entscheidet, verwendet der Solver Depth-First Search mit Minimum-Remaining-Values. Gleichstände werden zeilenweise und Werte in einer festgelegten Enum-Reihenfolge aufgelöst. Für Eindeutigkeit zählt er Lösungen nur bis **zwei**: null ist ungültig, eins eindeutig und zwei bedeutet „mehrdeutig oder mehr“.

Der Produktionsnachweis wird durch eine zweite Prüfpassage aus dem unveränderten Levelinput erzeugt. Er enthält Solver-Version, Puzzle-Hash, Lösungshash, Ergebniszahl, Suchknoten und Deduktionsmetriken. Ein Level darf nicht allein deshalb als eindeutig gelten, weil seine Authoring-Lösung gültig ist.

Ein regelbasierter Deduction Tracer protokolliert nur erklärbare, sichere Schlüsse. Die konkrete Auswahl und Darstellung eines Spielerhinweises bleibt hinter `IHintPolicy`; der Solver liefert keine werbliche oder UI-bezogene Entscheidung.

## Begründung

Ein Constraint-Solver bildet Regeln direkt ab, ist bei maximal 10×10 für Season 1 gut kontrollierbar und liefert sowohl Lösungen als auch Qualitätsmetriken. Das Limit zwei verhindert unnötige vollständige Enumeration. Deterministische Tiebreaker machen Proofs reproduzierbar.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Brute-Force über Pfade | Einfach, skaliert aber schlecht und liefert wenig Deduktionsinformation. |
| SAT-/SMT-Solver als Pflichtabhängigkeit | Leistungsfähig, aber zusätzliche native Toolchain und geringere Portabilität für Runtime-Hinweise. |
| Nur mitgespeicherte Lösung prüfen | Beweist keine Eindeutigkeit. |
| Zufällige Suchreihenfolge | Erschwert reproduzierbare Metriken und Tests. |

## Konsequenzen

Solver-Version und Heuristiken beeinflussen Metriken und müssen versioniert werden. Ein unabhängiger zweiter Algorithmus ist für v0.1 nicht Pflicht, aber kuratierte Releaselevel benötigen zusätzlich Golden- und Mutationsprüfungen. Laufzeitaufrufe erhalten harte Zeit- und Abbruchgrenzen.

## Betroffene Artefakte

`ARCHITECTURE/SOLVER_ARCHITECTURE.md`, `ARCHITECTURE/PUZZLE_ENGINE.md`, `ARCHITECTURE/LEVEL_DATA_FORMAT.md` und `ARCHITECTURE/TEST_STRATEGY.md`.

## Validierung

CI prüft bekannte 0-/1-/Mehrfachlösungsfälle, gespiegelte und rotierte Metamorphosen, deterministische Proof-Hashes sowie ein Performancebudget auf den größten vorgesehenen Rastern.

## Ersetzt / ersetzt durch

Ersetzt keinen früheren ADR. Wird durch [ADR-021](./ADR-021-puzzleidentitaet-und-proofartefakte.md) um die versionierte, an Puzzle und Lösung gebundene Proofartefaktform ergänzt; die Solverentscheidung bleibt angenommen.

## Referenzen

[1]: ../Stammstrecken_Puzzle_Konzept_00-15/04_Schwierigkeit_Feldgroessen_und_Levelgenerierung.md "Train Track Spiel – Schwierigkeit, Feldgrößen und Levelgenerierung"
[2]: ../Stammstrecken_Puzzle_Konzept_00-15/14_Season_1_Content_Bible.md "Stammstrecken-Puzzle – Season-1-Content-Bible"
