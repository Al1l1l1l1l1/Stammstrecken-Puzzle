# ADR-027 – Providerfreier Endless-No-Reward-Terminalpfad

## Status

**Angenommen**

## Datum

2026-09-13

## Kontext

ADR-025 trennt Puzzleabschluss und nachgelagerten `POST_CLEAR_PATIENCE`-Claim. Der dort definierte No-Reward-Abschluss setzt jedoch ein bestätigtes Providerergebnis und eine Provideroperation voraus. Ein Spieler, der nach dem Endless-Abschluss bewusst kein Rewarded Ad startet, kann den offenen Claimrecord daher nicht lokal freigeben. Wiederholte nicht beanspruchte Abschlüsse würden die begrenzte `openEndless`-Kapazität blockieren.

Der neue Pfad darf keine Belohnung erzeugen, keine Provideroperation simulieren und keinen laufenden oder bereits bestätigten Providerpfad überschreiben.

## Entscheidung

Nach `CompleteEndless` besitzt `COMPLETION_CLAIM_OPEN` zunächst `claimStatus: LOCAL_DECISION_PENDING`, keine `providerOperationId` und keine gestartete Provideroperation. Zwei exklusive Commands konkurrieren über dieselbe `expectedSaveGeneration`:

1. `SkipEndlessReward` entfernt den exakt gebundenen offenen Record in einem atomaren Savecommit. Es schreibt keinen Ledger-Eintrag, erzeugt keine Provider-ID und startet keinen Provideraufruf.
2. `ReserveEndlessRewardProvider` bindet vor jedem SDK-Aufruf eine eindeutige `providerOperationId`, setzt `claimStatus: PROVIDER_RESERVED` und commitet diese Reservation atomar. Erst nach erfolgreichem Commit darf der Adapter angesprochen werden.

Gewinnt der lokale Skip, muss ein konkurrierender Providerstart nach erneutem Lesen wegen des fehlenden offenen Records vor jedem externen Aufruf abbrechen. Gewinnt die Providerreservation, wird der Skip ohne Mutation abgewiesen. `RECONCILIATION_REQUIRED`, `REWARD_CONFIRMED` und `NO_REWARD_CONFIRMED` sind ebenfalls nicht skipfähig.

Ein Crash vor dem Skip-Commit lässt `LOCAL_DECISION_PENDING` wiederaufnehmbar. Ein Crash nach dem Commit lädt den Ordinal als kompakt terminal. Ein erneuter Skip desselben bereits terminalen Ordinals ist ein idempotenter No-op. Ein verspäteter Callback nach gewonnenem Skip bleibt redigierte Quarantäne und darf weder offenen Record noch Reward rekonstruieren.

`CLOSED_NO_REWARD` bleibt ausschließlich der providerbestätigte Pfad nach `NO_REWARD_CONFIRMED`. Der lokale Skip verwendet weder diesen Status noch eine synthetische Provider-ID. Idempotenz beruht weiterhin auf Watermark plus begrenztem offenen Komplement; es entsteht keine Skip- oder Terminalhistorie.

## Begründung

Der lokale Skip beseitigt die Kapazitätssperre ohne neue monetäre Wahrheit. Die persistierte Providerreservation vor jedem SDK-Aufruf und der gemeinsame Compare-and-swap-Konflikt verhindern, dass Skip und Providerpfad gleichzeitig gewinnen. Der vorhandene bounded-state-Vertrag bleibt unverändert.

## Betrachtete Alternativen

| Alternative | Bewertung |
|---|---|
| Synthetisches `NO_REWARD_CONFIRMED` | Vermischt lokale Entscheidung mit Providerbeleg und verletzt dessen Bindungssemantik. |
| Offenen Claim unbegrenzt behalten | Blockiert nach höchstens 20 nicht beanspruchten Abschlüssen die Endless-Nutzung. |
| Terminale Skip-ID-Liste speichern | Verletzt die konstante Save-Grenze. |
| Provider erst aufrufen und danach reservieren | Öffnet ein Crash-/Racefenster mit möglicher Anzeige nach lokalem Skip. |

## Konsequenzen

`COMPLETION_CLAIM_OPEN` erhält den initialen Status `LOCAL_DECISION_PENDING` und den zusätzlichen Status `PROVIDER_RESERVED`. Bestehende providergebundene Reward-/No-Reward-/Reconciliation-Pfade bleiben erhalten. Die Application muss Providerreservation und lokalen Skip als konkurrierende Savecommands implementieren.

## Betroffene Artefakte

`ARCHITECTURE/PERSISTENCE.md`, `ARCHITECTURE/GAME_STATE_MODEL.md`, `ARCHITECTURE/TEST_STRATEGY.md`, `tools/architecture-validation/fixtures/endless-save-v2.example.json`, `tools/architecture-validation/validate.py` und `WP-005`.

## Validierung

Ein ausführbarer Reducer prüft lokalen Skip ohne Provider-ID, Crash vor/nach Commit, Restart, Duplicate Skip, Skip nach Rewardbestätigung, Skip während Providerreservation, den CAS-Konflikt und verspätete Providercallbacks. Ein 100-Zyklen-Test bestätigt `openCount == 0`, unveränderten Economy-Saldo und fehlende Capacity-Sperre.

## Ersetzt / ersetzt durch

Präzisiert und ersetzt ausschließlich den lokalen Nichtbeanspruchungs- und Providerstartanteil aus [ADR-025](./ADR-025-endless-open-lifecycle-und-claims.md). Watermark, Open-Lifecycle, Rewardcommit und providerbestätigtes `CLOSED_NO_REWARD` aus ADR-025 bleiben gültig. Wird derzeit durch keinen ADR ersetzt.

## Referenzen

[1]: ../ARCHITECTURE/PERSISTENCE.md "Persistence v0.5"
[2]: ../ARCHITECTURE/GAME_STATE_MODEL.md "Game State Model v0.5"
[3]: ../WORK_PACKAGES/WP-005_Architecture-v0.5-letzte-High-Korrekturen.md "WP-005"
