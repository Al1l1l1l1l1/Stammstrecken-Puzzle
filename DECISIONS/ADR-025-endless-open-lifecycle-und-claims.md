# ADR-025 – Endless-Open-Lifecycle und nachgelagerte Claims

## Status

**Angenommen**

## Datum

2026-09-12

## Kontext

ADR-019 ersetzt eine wachsende Terminalhistorie durch einen UInt64-Watermark und höchstens 20 aktive Detailrecords. Zwei Übergänge blieben unvollständig. Erstens soll `ReserveEndless` vor der Generierung persistieren, während der bestehende Draftvertrag bereits generierten Puzzleinput voraussetzt. Zweitens entfernt `CompleteEndless` den Draft, obwohl der optionale `POST_CLEAR_PATIENCE`-Claim erst danach stattfinden kann.

Das bisherige Prädikat „reserviert und nicht aktiv ist terminal“ kann weder eine noch nicht generierte Reservation noch eine offene Completion-/Claim-Fortsetzung darstellen. Eine unbegrenzte Liste abgeschlossener Instanzen ist dennoch ausgeschlossen.

## Entscheidung

Save v2 wird kompatibel um ein einziges, auf **höchstens 20 Einträge** begrenztes `openEndless`-Set erweitert. Jeder Eintrag ist durch dieselbe Ordinal- und E1-Identität gebunden und befindet sich in genau einem Zustand:

| Zustand | Persistierte Pflichtdaten | Bedeutung |
|---|---|---|
| `RESERVED_NOT_GENERATED` | Ordinal, E1-ID, vollständiger kanonischer Descriptor | Reservation ist committed; Puzzleinput existiert noch nicht. |
| `ACTIVE_DRAFT` | Reservation plus öffentlicher Puzzleinput, profilierte Hashes und vollständiger Resumezustand | Generierung und Verifikation wurden atomar promotet. |
| `COMPLETION_CLAIM_OPEN` | Ordinal, E1-ID, Puzzle-ID, Completion-Commit-ID, Reason-Code, Claim-ID, persistierte lokale Operation, Claimstatus und gegebenenfalls Provideroperation | Puzzle ist abgeschlossen; Resume ist entfernt, ein nachgelagerter Claim ist noch offen oder in Reconciliation. |

`highestReservedOrdinal` bleibt der kanonische UInt64-Dezimalstring und wird nur um eins erhöht. `ReserveEndless` commitet Watermark und `RESERVED_NOT_GENERATED` gemeinsam. Die Generierung verwendet ausschließlich den persistierten Descriptor. Erfolgreich schema-/solver-/proofvalidierter Output wird atomar zu `ACTIVE_DRAFT` promotet. Ein Crash vor Promotion wiederholt dieselbe Generierung mit derselben Ordinal-, Descriptor- und E1-Bindung. Fehlt die gebundene Generatorversion, bleibt `ENDLESS_GENERATOR_RECOVERY_REQUIRED`; es wird weder eine neue Reservation angelegt noch still terminalisiert.

`CompleteEndless` persistiert Completioneffekte und ersetzt `ACTIVE_DRAFT` atomar durch `COMPLETION_CLAIM_OPEN`. Der Resume-Payload wird entfernt. Nur dieser offene Record berechtigt `POST_CLEAR_PATIENCE`. Dessen Claim-ID lautet weiterhin `reward-claim:POST_CLEAR_PATIENCE:<puzzleId>`. `RESERVED`, `RECONCILIATION_REQUIRED`, `REWARD_CONFIRMED` beziehungsweise `NO_REWARD_CONFIRMED` und der zugehörige Provider-/Operationstatus liegen innerhalb derselben begrenzten Fortsetzung.

Die lokale Operation wird mit `COMPLETION_CLAIM_OPEN` vor jedem Providercallback persistiert. Ein Rewardcallback darf den offenen Record nur verändern, wenn Claim-ID, Puzzle-ID und lokale Operation exakt zur Reservation passen; eine bereits gespeicherte Provideroperation darf nicht durch eine fremde ersetzt werden. `COMMIT_CLAIM` ist ausschließlich nach `REWARD_CONFIRMED` und mit denselben vier Bindungen zulässig. Der Geduldspunktebetrag wird dabei aus der exakt geladenen und release-gelockten `POST_CLEAR_PATIENCE`-Definition des Completionkatalogs gelesen; ein Callback oder Command darf keinen Betrag liefern oder überschreiben. `CLOSED_NO_REWARD` ist ausschließlich nach `NO_REWARD_CONFIRMED` zulässig. Unklare Ergebnisse bleiben `RECONCILIATION_REQUIRED`; ein später Callback nach terminalem Commit oder Close erzeugt nur ein redigiertes Auditereignis und niemals einen neuen Reward.

Nach atomarem Rewardcommit oder explizitem `CLOSED_NO_REWARD` wird die offene Fortsetzung entfernt. Erst dann ist der Ordinal kompakt terminal. `AbandonEndless` entfernt einen Reserved- oder Active-Eintrag ohne Completion- oder Claimberechtigung. Ein fehlender allgemeiner Rewardrecord bedeutet für Endless **nicht** automatisch `AVAILABLE`; berechtigt ist ausschließlich eine passende `COMPLETION_CLAIM_OPEN`-Instanz.

Für Ordinal `o` gilt:

- **offen**, wenn genau ein `openEndless`-Eintrag mit `generationOrdinal == o` existiert;
- **terminal**, wenn `1 <= o <= highestReservedOrdinal` und kein offener Eintrag existiert;
- **unreserviert**, wenn `o > highestReservedOrdinal`.

Terminalität enthält keinen Completed-/Abandoned-Unterschied. Die genaue Deduplikation ergibt sich aus Watermark plus begrenztem offenen Komplement. Verspätete Callbacks ohne passende offene Claimfortsetzung sind Quarantäne/No-op. Es entsteht keine terminale ID-Liste.

Die bisherige Save-v2-Form mit `activeDrafts` wird sequenziell und deterministisch in die erweiterte Save-v2-Leseform überführt: Jeder gültige aktive Draft wird `ACTIVE_DRAFT`. Eine historische v1-Quelle mit explizit belegter offener Claimberechtigung wird `COMPLETION_CLAIM_OPEN`. Ein v1-`COMPLETED` ohne beweisbare Claimauflösung oder Puzzlebindung stoppt mit `SAVE_MIGRATION_NEEDS_DECISION`; die Quelle bleibt unverändert. Der Writer emittiert ausschließlich `openEndless`.

## Begründung

Das offene Komplement des Watermarks bleibt konstant begrenzt und kann alle crashrelevanten Zwischenzustände tragen. Reservation wird vor Generierung wirklich dauerhaft, ohne unvollständigen Draft vorzutäuschen. Completion und Reward sind getrennte Commits, ohne Eligibility zu verlieren oder terminale Historie wachsen zu lassen.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Reservation sofort als vollständigen Draft speichern | Puzzleinput existiert vor Generierung noch nicht. |
| Nach Completion nur aus Watermark ableiten | Unterscheidet berechtigten Abschluss nicht von Abbruch oder nie generierter Reservation. |
| Unbegrenzte Completed-/Claim-ID-Liste | Verletzt den begrenzten Offline-Save. |
| Claim zwingend im Completioncommit abschließen | Externer Rewardausgang kann nachgelagert und unklar sein. |
| Neue Nutzungsgrenze | Ist keine bestätigte Produktentscheidung und nicht nötig. |

## Konsequenzen

Alle offenen Endless-Instanzen teilen die Kapazität 20. Terminale Nutzung bleibt bis zur UInt64-Erschöpfung unbegrenzt. Vollständiger Resume gilt nur für `ACTIVE_DRAFT`; deterministische Regeneration für `RESERVED_NOT_GENERATED`; begrenzte Claimrecovery für `COMPLETION_CLAIM_OPEN`.

Die Migration muss alte offene Claims beweisen oder fail-closed stoppen. Sie darf keinen Claim erfinden oder verlieren.

## Betroffene Artefakte

`ARCHITECTURE/PERSISTENCE.md`, `ARCHITECTURE/GAME_STATE_MODEL.md`, `ARCHITECTURE/SOLVER_ARCHITECTURE.md`, Save-/Migrationsfixtures, Teststrategie und Architekturvalidator.

## Validierung

Ein typisierter Reducer prüft Reserve, Crash, deterministische Regeneration, Promotion, Resume, Completion, Abandon, Rewardreservation, Reconciliation, Claim-/Providerbindung, kataloggebundenen Betrag, Commit, `CLOSED_NO_REWARD`, verspäteten Callback und Kompaktierung. Er erzwingt kanonische UInt64-Werte, E1-/Descriptorbindung, zulässige Pflichtfelder, Kapazität 20 und mindestens 10.000 gemischte Zyklen ohne wachsende Terminalhistorie. Negativmutationen decken falschen Betrag, fehlende oder falsche Claim-ID, unzulässigen Claimstatus, Commit ohne bestätigtes Rewardergebnis und späten Callback ab.

## Ersetzt / ersetzt durch

Ersetzt die Open-Lifecycle-, Terminalprädikat- und Claimfortsetzungsanteile aus [ADR-019](./ADR-019-endless-watermark-und-save-v2.md). UInt64-Watermark, fehlende terminale Nutzungsgrenze und `endless-v1`-Identität bleiben angenommen. Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: ../ARCHITECTURE/PERSISTENCE.md "Persistence v0.4"
[2]: ../ARCHITECTURE/SOLVER_ARCHITECTURE.md "Solver Architecture v0.4"
[3]: ../WORK_PACKAGES/WP-004_Architecture-v0.4-Abschlusskorrekturen.md "WP-004"
