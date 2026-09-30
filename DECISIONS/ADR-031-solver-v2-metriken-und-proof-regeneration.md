# ADR-031 – Solver-v2-Metriken und Proofregeneration

## Status

**Angenommen**

## Datum

2026-09-30

## Kontext

Die verbindlichen Architekturquellen verlangen einen deterministischen Solver, erklärbare sichere Deduktionen, reproduzierbare Metriken und regenerierbare, solverversionierte Proofartefakte. Sie legen jedoch die Zählgrenzen einzelner Metriken noch nicht präzise genug fest. Der nicht integrierte WP-013-Prototyp offenbart dadurch drei Inkonsistenzen:

1. Ein rein deduktiver Lauf zählt korrekt keine DFS-Wertannahmen, während das Proof-v1-Schema bisher mindestens einen `searchNodes`-Wert verlangt.
2. Ein gespeicherter Proof wird nicht mit einer frischen Solverregeneration verglichen.
3. Der Prototyp vermischt Root-Propagation und Suchzweige bei der Deduktionstiefe, obwohl die Kampagnenschwierigkeit aus nachvollziehbaren Schlussketten des öffentlichen Puzzleinputs entstehen muss.

Die Festlegung erfolgt **nicht** auf Grundlage eines späteren separaten Manus-Chats. Die Quellenherkunft und die ausgeschlossene Quelle sind in [`WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md`](../PROJECT_CONTROL/WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md) dokumentiert.

## Entscheidung

### 1. Versionierung

1. `solver-v1` bleibt als historische Solverkennung lesbar, wird aber nicht für neue Proofs oder neue Produktion verwendet.
2. Alle neuen Proofgenerierungen nach dieser Entscheidung verwenden **`solver-v2`**.
3. `proofFormatVersion` bleibt **1**, weil die Objektform, die Hashprojektion `STP-PROOF-JCS-1` und die Leserform unverändert bleiben. Ein neues Proofformat ist erst bei einer strukturellen Änderung nötig.
4. `solver-v2`-Proofs dürfen bei identischem `puzzleId` und identischem semantischem Puzzlehash neue Proofhashes haben. Das ist eine neutrale Proofregeneration gemäß ADR-021, keine Änderung der Puzzleidentität.

### 2. Metriken von `solver-v2`

Alle vier gespeicherten Metriken sind nichtnegative Ganzzahlen. Das Proof-v1-Schema akzeptiert deshalb für `searchNodes`, `deductionSteps`, `maxDeductionDepth` und `requiredGuessDepth` jeweils `minimum: 0`.

| Metrik | Verbindliche Definition | Zählgrenze |
|---|---|---|
| `searchNodes` | Zahl der tatsächlich begonnenen DFS-Wertannahmen zur Klassifikation bis zum Lösungslimit zwei. | Jeder angefangene DFS-Kindzweig zählt genau einmal; Root-Propagation zählt nicht; erfolgreiche und verworfene Zweige zählen. |
| `deductionSteps` | Zahl der deduplizierten, erklärbaren und sicheren Domänenreduktionen aus dem unveränderten öffentlichen Puzzleinput. | Ausschließlich Root-Propagation vor der ersten DFS-Annahme; alle Suchzweige sind ausgeschlossen. |
| `maxDeductionDepth` | Größte Prämissenabhängigkeit einer Root-Deduktion. | Öffentliche Startfakten haben Tiefe 0. Eine Reduktion ohne abgeleitete Prämisse hat Tiefe 1; sonst `1 + max(Tiefe der abgeleiteten Prämissen)`. Suchannahmen und ihre Folgen sind ausgeschlossen. Kein Root-Deduktionsschritt ergibt 0. |
| `requiredGuessDepth` | Zahl gleichzeitig aktiver DFS-Annahmen auf dem deterministischen Pfad zur einzigen gefundenen Lösung. | Root-only-Lösung ergibt 0. Vorher untersuchte, verworfene Zweige zählen nicht. Die feste Solver-v2-Tiebreakerordnung macht den Wert reproduzierbar. |

`searchNodes` ist damit eine technische Nachweislast. `deductionSteps` und `maxDeductionDepth` sind Root-Logiksignale für die Contentqualität. `requiredGuessDepth` trennt den notwendigen Hypothesenpfad von der globalen Sucharbeit.

### 3. Deduktionsspur

`solver-v2` muss für jede gezählte Root-Deduktion mindestens `ruleCode`, konkrete Konklusion und die minimalen referenzierten Prämissen bereitstellen. Die Metrikwerte müssen aus dieser Spur rekonstruierbar sein. Eine bloße numerische Zell- oder globale Tiefenbuchführung genügt nicht.

### 4. Kampagnen- und Hint-Grenze

Normale kuratierte Kampagnenlevel benötigen `requiredGuessDepth = 0`. Ein Wert größer null ist ein harter Contentfehler für den normalen Kampagnenimport. Suchzweige dürfen zur Eindeutigkeitsprüfung stattfinden; sie dürfen jedoch nicht als notwendige Spielerlogik in die Kampagne gelangen. Spätere ausdrücklich als Expertenmodus entschiedene Inhalte benötigen einen separaten Produkt- und Architekturentscheid.

### 5. Proofregeneration und Strict-Modus

1. Die Validierung erzeugt aus dem öffentlichen Puzzleinput stets einen frischen kanonischen Proof mit dem Produktionssolver.
2. Der frische Proof muss zunächst selbst schema- und hashgültig sein.
3. Bei gleichem Puzzlehash und gleicher Solverversion müssen frischer und gespeicherter Proof bytegleich sein. Jede Abweichung ist `PRF-NONDETERMINISTIC` und blockiert Import, CI und Release.
4. `--strict` eskaliert jede Warnung innerhalb **jedes** `LevelValidationResult` zu einer Fehlerdiagnose mit unverändertem Diagnosecode. Batchreport, Einzelresultat, `Succeeded`, `FailedStage` und Zähler müssen denselben Fehlstatus zeigen.

## Begründung

Die Produktquellen definieren Schwierigkeit durch nachvollziehbare Schlussketten und verbieten blindes Raten in normalen Kampagnenleveln. Die Trennung der Metriken verhindert, dass technische Prüfsuche als Spielerlogik ausgegeben wird. Sie macht die Contentdaten didaktisch prüfbar, während `searchNodes` weiterhin den Aufwand der Eindeutigkeitsprüfung sichtbar hält.

ADR-007 verlangt sowohl erklärbare Deduktionen als auch Versionswechsel bei Metrikänderungen. ADR-021 erlaubt Proofregeneration nach Solverwechsel ohne Änderung der Puzzleidentität. Die Entscheidung erfüllt daher die bestehende Architektur, statt sie zu ersetzen.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| `searchNodes` künstlich bei 1 beginnen lassen | Verfälscht die Metrik und zählt den Root entgegen der DFS-Definition. Abgelehnt. |
| `searchNodes` bei 0 erlauben, alle übrigen Metriken aber unpräzise lassen | Löst nur die Schemainkonsistenz, nicht die Content- und Proofaussage. Abgelehnt. |
| Suchzweige in `maxDeductionDepth` einrechnen | Vermischt Hypothesen mit Spielerlogik und widerspricht der Contentabsicht. Abgelehnt. |
| `proof-v2` nur wegen neuer Metrikwerte einführen | Objektform und Hashprojektion bleiben gleich; `solverVersion` ist der vorgesehene Kompatibilitätsanker. Abgelehnt. |
| Gespeicherten Proof als ausreichenden Nachweis akzeptieren | Widerspricht regenerierbarem Cache, Determinismus und CI-Gate. Abgelehnt. |

## Konsequenzen

- Alle bisherigen `solver-v1`-Prooffixtures werden als historische Prototypfixtures behandelt und in WP-017 bewusst durch `solver-v2`-Goldens ersetzt.
- Der historische WP-013-Branch wird nicht direkt integriert.
- Die Umsetzung ist auf die getrennten Work Packages WP-015, WP-016 und WP-017 aufgeteilt. Kein Paket darf die Grenzen des anderen still erweitern.
- Neue normale Kampagnenlevel dürfen erst nach WP-017 in einen importierbaren Katalog gelangen.
- Jeder spätere Eingriff in Solver-Tiebreaker, Traceform oder Metriksemantik erfordert `solver-v3` oder höher, regenerierte Proofs und eine neue ADR-Prüfung.

## Betroffene Artefakte

- [`ARCHITECTURE/SOLVER_ARCHITECTURE.md`](../ARCHITECTURE/SOLVER_ARCHITECTURE.md)
- [`ARCHITECTURE/LEVEL_DATA_FORMAT.md`](../ARCHITECTURE/LEVEL_DATA_FORMAT.md)
- [`ARCHITECTURE/CONTENT_PIPELINE.md`](../ARCHITECTURE/CONTENT_PIPELINE.md)
- `ARCHITECTURE/schemas/proof-v1.schema.json`
- `STP.Puzzle.Solver`, `STP.Infrastructure.Content`, `STP.Editor.Content` und die zugehörigen EditMode-Tests
- [`WORK_PACKAGES/WP-015_Level-v2-Foundation-und-Hashvertraege.md`](../WORK_PACKAGES/WP-015_Level-v2-Foundation-und-Hashvertraege.md)
- [`WORK_PACKAGES/WP-016_Solver-v2-Metriken-und-Deduktionsspur.md`](../WORK_PACKAGES/WP-016_Solver-v2-Metriken-und-Deduktionsspur.md)
- [`WORK_PACKAGES/WP-017_Proof-v1-Regeneration-und-Strict-Validation.md`](../WORK_PACKAGES/WP-017_Proof-v1-Regeneration-und-Strict-Validation.md)

## Validierung

Die Umsetzung ist erst vollständig, wenn:

1. Referenzfälle die exakten vier `solver-v2`-Metriken einschließlich eines deduktiven Nullfalls und eines Suchfalls prüfen;
2. ein frisch generierter Root-only-Proof gegen ein gespeichertes Golden bytegleich ist;
3. ein selbstkonsistenter, aber fachlich abweichender gespeicherter Proof mit `PRF-NONDETERMINISTIC` fehlschlägt;
4. Strict- und Non-strict-Einzel- wie Batchresultate konsistent geprüft sind;
5. Astra und Sol die abgeschlossenen Implementierungs-Work-Packages unabhängig ohne Blocker oder High-Befund freigeben.

## Ersetzt / ersetzt durch

Ersetzt keinen ADR vollständig. Ergänzt [`ADR-007`](./ADR-007-solver-und-eindeutigkeitspruefung.md) in dessen Metrik- und Versionsanteil sowie [`ADR-021`](./ADR-021-puzzleidentitaet-und-proofartefakte.md) in dessen Proofregenerationsanteil. Die übrigen Entscheidungen beider ADRs bleiben unverändert gültig.

## Referenzen

[1]: ./ADR-007-solver-und-eindeutigkeitspruefung.md "ADR-007 – Deterministischer Constraint-Solver"
[2]: ./ADR-021-puzzleidentitaet-und-proofartefakte.md "ADR-021 – Puzzleidentität und versionierte Proofartefakte"
[3]: ../ARCHITECTURE/SOLVER_ARCHITECTURE.md "Solver Architecture"
[4]: ../ARCHITECTURE/LEVEL_DATA_FORMAT.md "Level Data Format"
[5]: ../ARCHITECTURE/CONTENT_PIPELINE.md "Content Pipeline"
[6]: ../Stammstrecken_Puzzle_Konzept_00-15/04_Schwierigkeit_Feldgroessen_und_Levelgenerierung.md "Schwierigkeit, Feldgrößen und Levelgenerierung"
[7]: ../PROJECT_CONTROL/WP-013_SOURCE_AUTHORITY_AND_RECOVERY.md "WP-013 – Quellenautorität, Auditkorrektur und Wiederanlauf"
