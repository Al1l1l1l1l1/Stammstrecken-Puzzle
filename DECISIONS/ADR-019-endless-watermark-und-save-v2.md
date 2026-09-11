# ADR-019 – Endless-Watermark und Save v2

## Status

**Angenommen**

## Datum

2026-09-12

## Kontext

Die v0.2-Regel aus maximal 20 aktiven Deskriptoren, 64 Terminalintervallen und 64 Terminaldetails begrenzte nicht nur Diagnostik, sondern nach genügend alternierenden Abschlüssen die weitere Nutzung. Eine unbeschränkte Liste terminaler Identitäten wäre zugleich kein begrenzter lokaler Save.

## Entscheidung

Save v2 ersetzt die terminale Endless-Historie durch genau zwei autoritative Bestandteile:

1. `highestReservedOrdinal`: kanonischer UInt64-Dezimalstring ohne führende Nullen, initial `0`;
2. höchstens 20 aktive Endless-Drafts als einzige detaillierte Resume-Datensätze.

Für einen Ordinal `o` gilt:

- **aktiv**, wenn genau ein aktiver Draft mit `generationOrdinal == o` existiert;
- **terminal**, wenn `1 <= o <= highestReservedOrdinal` und kein aktiver Draft existiert;
- **unreserviert**, wenn `o > highestReservedOrdinal`.

`ReserveEndless` reserviert nur `highestReservedOrdinal + 1` und schreibt Watermark plus Draft atomar vor der Generierung. `CompleteEndless` schreibt alle zulässigen Progress-/Economy-Effekte und entfernt den Draft in demselben atomaren Savecommit. `AbandonEndless` entfernt den Draft atomar ohne Reward. Ein erneuter terminaler Command ist `ENDLESS_TERMINAL_DUPLICATE` beziehungsweise ein effektfreier No-op.

Terminalintervalle, Terminalcheckpoint und Terminaldetailtail sind in Save v2 keine fachliche Wahrheit. Der begrenzte, redigierte Diagnose-Ring bleibt optional und darf nie für Resume, Eligibility, Deduplikation, Rewards oder Ledgerkompaktierung gelesen werden. Die `endless-v1`-ID-Projektion bleibt unverändert.

Bei 20 offenen Drafts blockiert ausschließlich `ENDLESS_ACTIVE_DRAFT_CAPACITY`; terminale Folgen besitzen keine kleine Retentionsgrenze. Erst die Erschöpfung des technisch festen UInt64-Raums endet fail-closed mit `ENDLESS_ORDINAL_SPACE_EXHAUSTED`.

## Begründung

Das terminale Präfix ist das Komplement der begrenzten Aktivmenge. Dadurch wächst der autoritative Endless-Savezustand nicht mit der Zahl vergangener Abschlüsse und bleibt dennoch exakt deduplizierbar.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Statusintervalle mit fester Obergrenze | Erzeugt nach genügend Statuswechseln ein Produktlimit. |
| Unbegrenzte Liste terminaler IDs | Nicht begrenzter Offline-Save. |
| Nur Hashkette oder Bloomfilter | Kein exakter Membership-Nachweis beziehungsweise False Positives. |
| Beliebig große Ganzzahl | Mathematisch weiter, aber nicht konstant begrenzt. |

## Konsequenzen

`saveSchemaVersion` wird `2`; das JCS-Profil bleibt `STP-SAVE-JCS-1`. Die v1→v2-Migration validiert die vollständige reservierte Präfixabdeckung, setzt `highestReservedOrdinal = nextGenerationOrdinal - 1`, übernimmt aktive Drafts und bewahrt Ledger, Claims und Saldo. Bei Lücken oder Mehrdeutigkeit stoppt sie mit `SAVE_MIGRATION_NEEDS_DECISION` und erhält die Quelle unverändert.

Die Unterscheidung alter terminaler Endless-Instanzen in `COMPLETED` oder `ABANDONED` ist bewusst keine persistente Produktfunktion. Sollte sie benötigt werden, wäre eine neue Produkt- und Speicherentscheidung erforderlich.

## Betroffene Artefakte

`ARCHITECTURE/PERSISTENCE.md`, `ARCHITECTURE/SOLVER_ARCHITECTURE.md`, `ARCHITECTURE/GAME_STATE_MODEL.md`, Teststrategie, Save-Goldens und Architekturvalidator.

## Validierung

Der Referenzreducer führt mindestens 10.000 alternierende Complete-/Abandon-Transitionen, Resume-Lücken, Duplicate nach Ledgerkompaktierung, Crashfenster, aktive Kapazität und v1→v2-Goldenmigration aus. Reintroduzierte Terminalgrenzen sowie inkonsistente Watermarks/Drafts müssen scheitern.

## Ersetzt / ersetzt durch

Ersetzt die Retentions- und Saveanteile aus [ADR-014](./ADR-014-save-kanonisierung-und-ledgerkompaktierung.md) und [ADR-016](./ADR-016-katalogvertraege-und-endless-identitaet.md); deren nicht widersprechende JCS-, Ledger-, Katalog- und Identitätsentscheidungen bleiben durch diesen ADR ausdrücklich restatiert. Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: ../ARCHITECTURE/PERSISTENCE.md "Persistence v0.3"
[2]: ../ARCHITECTURE/SOLVER_ARCHITECTURE.md "Solver Architecture v0.3"
[3]: ../WORK_PACKAGES/WP-003_Architecture-v0.3-Finalkorrekturen.md "WP-003"
