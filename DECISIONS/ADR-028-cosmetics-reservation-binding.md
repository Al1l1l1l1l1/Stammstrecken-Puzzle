# ADR-028 – Bindende Cosmetics-Claim-Reservation

## Status

**Angenommen**

## Datum

2026-09-13

## Kontext

ADR-023 definiert einen atomaren Meilensteinclaim. Der ausführbare Architekturvertrag erlaubte `COMMIT_CLAIM` dennoch ohne nachweislich zuvor persistierte, eligibility-validierte Reservation. Eine bloße aktuelle Eligibility oder ein ungebundener `RESERVED`-String beweist weder den geprüften Katalogsnapshot noch die geprüfte Fortschrittsprojektion.

## Entscheidung

`ClaimMilestoneCosmetic` besteht aus zwei getrennten atomaren Savecommits.

`RESERVE_CLAIM` ist nur nach erfolgreicher Prüfung des gelockten runtime-zulässigen Katalogs, eines `ACTIVE`-Items mit `MILESTONE_GRANT`, der erwarteten Savegeneration und positiver Eligibility zulässig. Die persistierte Reservation enthält mindestens:

| Feld | Bindung |
|---|---|
| `claimId` | `cosmetic-milestone-claim:v1:<catalogId>:<itemId>` |
| `operationId` | lokal eindeutig für diese Reservation |
| `claimGeneration` | innerhalb dieser Claim-ID monoton und im Record fixiert |
| `itemId`, `catalogId` | exakte Item- und Katalogidentität |
| `catalogRevision`, `catalogHashSha256` | exakt geprüfter Katalogsnapshot |
| `acquisition` | `MILESTONE_GRANT` |
| `eligibilityContractVersion` | aktuell `1` |
| `eligibilityProjectionHashSha256` | Hash der deterministisch sortierten Eligibility-Projektion |
| `claimState` | `RESERVED` |

Die Eligibility-Projektion bindet Katalog, Item, Eligibilityvertrag, sortierte Campaign-Subject-IDs, deren expandierte Puzzle-IDs und die erforderlichen First-Clear-Records. Andere Inputs sind ausgeschlossen.

`COMMIT_CLAIM` lädt genau diese persistierte Reservation. Es ist nur zulässig, wenn Claim-ID, Operation-ID, Claimgeneration, Item, Erwerbsart, Katalog-ID, Revision, Hash, Eligibility-Version und erneut hergeleitete Eligibility-Projektion vollständig übereinstimmen und der Claim noch nicht verbucht ist. Der Commit schreibt in einem atomaren Snapshot den terminalen Ownershiprecord mit derselben Provenienz und entfernt die offene Reservation. Das Ledgerdelta ist null.

Ein identischer bereits terminaler Claim liefert `ALREADY_OWNED` ohne weiteren Effekt. Dieselbe Claim-ID mit abweichender Identität oder Projektion ist `COS_MILESTONE_CLAIM_COLLISION`. Fehlende, nachträglich eingefügte, nicht eligibility-validierte oder abweichende Reservationen werden abgewiesen.

## Begründung

Die persistierte Reservation ist der einzige belastbare Nachweis, dass Eligibility und Katalogsnapshot vor dem Commit geprüft wurden. Die vollständige Bindung verhindert Wiederverwendung für ein anderes Item, einen anderen Katalog oder einen veränderten Fortschrittsstand. Der atomare Ownershipcommit bewahrt Crashsicherheit und Idempotenz.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Eligibility erst in `COMMIT_CLAIM` prüfen | Beweist keine zuvor persistierte Reservation und lässt direkte Commitpfade zu. |
| Nur Claim-ID persistieren | Bindet Katalogrevision, Item und Eligibility-Projektion nicht. |
| Separate unbegrenzte Terminalclaimliste | Dupliziert Inventory-Wahrheit und wächst unbegrenzt. |
| Reservation ohne erneute Eligibilityprüfung committen | Akzeptiert nachträglich unpassende oder manipulierte Provenienz. |

## Konsequenzen

Cosmetics-Fixtures und Validator verwenden strukturierte Reservation- und Ownershiprecords statt eines einzelnen `claimState`-Strings. Positive Tests folgen `Evaluate → ReserveCommit → optional Crash/Restart → Commit`. Alle direkten oder inkonsistenten Commitpfade scheitern.

## Betroffene Artefakte

`ARCHITECTURE/CONTENT_CATALOGS.md`, `ARCHITECTURE/PERSISTENCE.md`, `ARCHITECTURE/GAME_STATE_MODEL.md`, `ARCHITECTURE/TEST_STRATEGY.md`, `tools/architecture-validation/fixtures/cosmetics-lifecycle-v1.json`, `tools/architecture-validation/validate.py` und `WP-005`.

## Validierung

Der Reducer prüft den positiven Reserve-/Restart-/Commitpfad und lehnt Commit ohne Reservation, gefälschte Reservation ohne Eligibility, abweichende Cosmetic-ID, andere Katalogrevision, geänderte Eligibility-Projektion und Claimkollision ab. Identisches Replay bleibt ohne Doppelownership idempotent.

## Ersetzt / ersetzt durch

Präzisiert und ersetzt ausschließlich die Reservation-/Commitbindung des kosmetischen Meilensteinclaims aus [ADR-023](./ADR-023-releasekandidat-und-kosmetikclaims.md). Releasekandidat-Identität, Erwerbsmodi und terminale Inventory-Wahrheit aus ADR-023 bleiben gültig. Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: ../ARCHITECTURE/CONTENT_CATALOGS.md "Content Catalogs v0.5"
[2]: ../ARCHITECTURE/PERSISTENCE.md "Persistence v0.5"
[3]: ../WORK_PACKAGES/WP-005_Architecture-v0.5-letzte-High-Korrekturen.md "WP-005"
