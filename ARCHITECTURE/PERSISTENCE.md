# Persistence v0.1

## 1. Ziel und Wahrheiten

Der lokale, versionierte Save-Snapshot ist die autoritative Quelle für Gameplay-Fortschritt, Entwürfe, Geduldspunkte, kosmetische Freischaltungen und Einstellungen. Das Spiel ist vollständig offline spielbar. Analytics, Crashlytics und Werbung speichern oder rekonstruieren keine Gameplay-Wahrheit.

Der Apple App Store beziehungsweise Google Play Store ist die externe Wahrheit für den nicht konsumierbaren Anspruch `remove_ads`. Der lokale Save enthält einen ausfallsicheren Cache und verarbeitete Transaktionsreferenzen.

## 2. Speicherorte und Dateien

Alle Dateien liegen in Unitys app-privatem `persistentDataPath`. Das Save-System besitzt allein den Pfad; andere Module erhalten keinen Dateisystemzugriff.

| Datei | Zweck |
|---|---|
| `save-v1.json` | letzter erfolgreich atomar bestätigter Snapshot. |
| `save-v1.backup.json` | unmittelbar vorheriger gültiger Snapshot. |
| `save-v1.pending.json` | vollständig zu schreibender Kandidat; nur mit gültigem Envelope wiederherstellbar. |
| `save-v1.migration-source.json` | unveränderte Sicherung vor einer Schema-Migration. |
| `diagnostics-ring.jsonl` | begrenztes lokales, redigiertes Diagnosejournal. |

PlayerPrefs wird nicht für Spielstände, Währung, Entitlements oder Entwürfe verwendet. Falls Unity-/SDK-interne Einstellungen PlayerPrefs erzwingen, liegen sie außerhalb der autoritativen Persistenz und werden im SDK-Inventar dokumentiert.

## 3. Save-Envelope

Der JSON-Snapshot besitzt einen stabilen Envelope:

| Feld | Vertrag |
|---|---|
| `saveSchemaVersion` | positive Ganzzahl; initial `1`. |
| `generation` | bei jedem erfolgreichen Commit streng erhöhtes 64-Bit-Integer. |
| `createdAtUtc` | Erzeugungszeit dieses Snapshots; keine Konfliktwahrheit. |
| `appVersion`, `buildId` | Diagnose und Migrationsnachweis. |
| `contentCatalogHash` | Katalogstand, gegen den IDs validiert wurden. |
| `payload` | vollständiger `GameProfileState` plus aktive Entwürfe. |
| `payloadSha256` | SHA-256 über kanonischen Payload. |

Die Prüfsumme erkennt Korruption und unvollständige Writes, ist aber kein Manipulationsschutz. Das Spiel verspricht keine Anti-Cheat-Sicherheit; es verkauft keine Währung oder Rätselvorteile.

## 4. Payloadstruktur

| Bereich | Inhalt |
|---|---|
| `profile` | lokale Profil-ID, erste Nutzung, höchste freigegebene Inhalte, Meisterschaft. |
| `levels` | Records pro stabiler Level-ID plus `puzzleHashAtFirstCompletion`: Abschluss, Sterne, Zeiten, direkte Lösung und einmalige Rewards. |
| `economyLedger` | idempotente Gutschriften/Belastungen; Saldo wird geprüft abgeleitet. |
| `inventory` | gezielt freigeschaltete und ausgewählte kosmetische IDs. |
| `seasonProgressCache` | abgeleitete Route-/Abschnitt-/Seasonstände plus Quellhash. |
| `drafts` | höchstens ein aktiver Entwurf je Level, begrenzt auf die jüngsten 20, inklusive Undo-Diffs. |
| `dailyStatus` | reservierter, versionierter `DailyEntitlementRecord` mit bestätigten Claim-IDs; keine Tagesgrenzenpolicy vor Schließung von `BLOCKER-PROD-002`. |
| `entitlements` | lokaler Status von `remove_ads`, Store, Produkt-ID, letzte verifizierte Referenz und Revalidierungsstatus. |
| `pendingOperations` | idempotente Reward-/Kaufoperationen ohne geheimen Receiptinhalt. |
| `settings` | Locale, Lautstärkegruppen, Haptik, Accessibility- und Privacypräferenzen. |
| `dataLifecycle` | Consent-/Datenerfassungsrevision und letzter lokaler Reset. |

Unbekannte Level- oder Asset-IDs werden nicht gelöscht. Sie werden als `orphaned` erhalten, diagnostiziert und bei erneut verfügbarem Content reaktiviert. Dadurch gehen Fortschritte bei vorübergehenden Katalogfehlern nicht verloren.

## 5. Atomarer Schreibalgorithmus

Jeder Commit erhält eine fortlaufende `generation` und folgt exakt diesem Ablauf:

1. neuen unveränderlichen Snapshot erzeugen und intern validieren;
2. Payload kanonisieren und SHA-256 berechnen;
3. vollständigen Envelope nach `pending` schreiben;
4. Stream flushen; Plattformadapter fordert bestmöglichen persistenten Flush an;
5. `pending` erneut öffnen, parsen und Hash prüfen;
6. gültige Hauptdatei nach `backup` rotieren;
7. `pending` über eine plattformgetestete Replace/Rename-Operation zur Hauptdatei machen;
8. Hauptdatei erneut auf Generation und Hash prüfen;
9. erst danach `SaveCommitted(generation)` melden.

Kann ein Schritt nicht garantiert atomar erfolgen, verwendet der Adapter Copy-on-Write mit den drei Generationen und wählt beim Lesen ausschließlich vollständig validierte Dateien. Der Savevertrag hängt nicht von einer einzelnen Dateisystem-API ab.

Es existiert zu jeder Zeit höchstens ein Writer. Weitere Mutationen werden seriell in einer Save Queue zusammengeführt. Kein Callsite schreibt den Snapshot selbst.

## 6. Lade- und Recoveryalgorithmus

Beim Start werden `main`, `backup` und `pending` unabhängig gelesen. Kandidaten mit Parsefehler, unbekannter Version, ungültigem Hash oder ungültiger Payload werden verworfen, aber nicht sofort gelöscht. Unter allen gültigen, unterstützten Kandidaten gewinnt die höchste Generation. Gleichstand mit unterschiedlichem Hash ist `SAVE_SPLIT_BRAIN` und bevorzugt `main`, während beide Dateien für Diagnose erhalten bleiben.

Recoveryreihenfolge:

1. höchste gültige unterstützte Generation;
2. falls nur ältere Schema-Version gültig: Migration auf Kopie;
3. falls kein Kandidat gültig: neuer leerer Save **nur nach** sichtbarer Recoveryinformation; korrupte Dateien bleiben exportierbar;
4. kein Fortschritt wird aus Telemetrie oder Contentnamen geraten.

Ein Production-Fehlerdialog bietet „Erneut versuchen“, „lokales Backup verwenden“ und erst als bewusste letzte Option „neuen Spielstand beginnen“. Destruktives Zurücksetzen verlangt eine separate Bestätigung.

## 7. Autosave-Policy

| Auslöser | Verhalten |
|---|---|
| Zell-/Batchcommand oder Undo | 500-ms-Debounce, spätestens 2 Sekunden nach erster noch ungeschriebener Mutation. |
| App geht in Hintergrund / verliert Fokus | sofortiger Snapshot ohne auf Netzwerk zu warten. |
| Level wird gelöst | synchroner logischer Commit vor Freigabe von Zugfahrt und Ergebnisreward. |
| Economy-/Inventaraktion | Commit vor Erfolgsmeldung an UI. |
| Kauf-/Rewardcallback | idempotent verarbeiten und sofort committen. |
| Einstellung | 500-ms-Debounce; Audio wirkt sofort. |
| Quit | Best effort; Korrektheit darf nicht vom Quit-Callback abhängen. |

Datei-I/O geschieht außerhalb des Renderframes. Snapshotbildung ist kurz und immutable. Ein fehlgeschlagener Autosave bleibt in einer exponentiell begrenzten lokalen Retryqueue; Abschluss- und Economycommits zeigen einen nicht irreführenden Fehlerzustand.

## 8. Saveversionen und Migrationen

Jede Version besitzt einen expliziten DTO-Typ und genau einen Migrator `vN -> vN+1`. Migrationen sind rein, deterministisch und greifen nicht auf Netzwerk, Locale oder aktuelle Wanduhr für fachliche Defaults zu.

Ablauf:

1. unveränderte Quelldatei als `migration-source` sichern;
2. jede Zwischenversion einzeln migrieren und validieren;
3. Endzustand roundtrip-serialisieren und Payloadhash prüfen;
4. fachliche Invarianten wie nicht sinkende Sterne und Ledgerbalance prüfen;
5. als neue Generation atomar committen;
6. alte Sicherung bis zum nächsten erfolgreichen Appstart behalten.

Kann ein Pflichtwert nicht neutral hergeleitet werden, stoppt die Migration mit `SAVE_MIGRATION_NEEDS_DECISION`. Ein Agent darf keinen Wert erfinden. Downgrade-Schreiben ist verboten; eine ältere App darf einen neueren Save nicht überschreiben.

## 9. Fortschritt und Idempotenz

Jede einmalige Belohnung besitzt eine deterministische Transaktions-ID. `ApplyLedgerEntry` ist idempotent: dieselbe ID mit identischem Inhalt ist ein No-op; dieselbe ID mit anderem Inhalt ist `LEDGER_ID_COLLISION` und blockiert den Commit.

Beispiele:

- `level:S1-01-01-01:first-clear`;
- `level:S1-01-01-01:star-2`;
- `level:S1-01-01-01:star-3`;
- `route:S1-01-01:complete`;
- `section:S1-01:complete`;
- `rewarded:S1-01-01-01:post-clear:{providerRewardId}`.

Bereits verdiente Sterne, Punkte und Kosmetik werden durch Revision, Migration oder Wiederholung nie entfernt. Betriebsrevision kann nur verbessern und fehlende Sternboni einmalig ergänzen.

## 10. IAP-Entitlement und Wiederherstellung

`remove_ads` ist ein nicht konsumierbares Produkt. Zustände sind `UNKNOWN`, `NOT_OWNED`, `OWNED_LOCAL_VERIFIED`, `OWNED_STORE_VERIFIED`, `PENDING` und `REVOCATION_CONFIRMED`.

- Ein bestätigter Kauf wird über Transaktions-ID idempotent verbucht und entfernt unterbrechende Anzeigen sofort.
- Beim Offline-Start genügt ein zuvor storeverifizierter lokaler Cache, um Anzeigen weiter auszuschalten.
- Eine vorübergehend fehlgeschlagene Storeabfrage setzt Eigentum nicht auf `NOT_OWNED` zurück.
- Nur eine eindeutige Storeantwort kann eine Revokation markieren; der Übergang wird diagnostiziert und darf nicht durch SDK-Timeout entstehen.
- Einstellungen bieten jederzeit „Käufe wiederherstellen“. Auf Apple wird der explizite Restoreflow verwendet; auf Google wird die Produktabfrage erneut synchronisiert.
- Vollständige Receipts und Storecredentials werden nicht in Logs oder Analytics geschrieben.

Ein neues Gerät ohne Cloudsave stellt nur Storeentitlements wieder her, nicht Gameplay-Fortschritt. Eine spätere Cloudfunktion benötigt ein neues ADR und Produktentscheidungen.

## 11. Tagesstatus und Uhr

Die Produktvorgabe „einmal pro Kalendertag“ legt noch nicht fest, welche Zeitzone gilt und wie Offlinebetrieb, Reisen, Zeitzonenwechsel oder manuelle Uhränderungen behandelt werden. **BLOCKER-PROD-002** verhindert deshalb die Implementierung und Veröffentlichung der Betriebslage-des-Tages-Anspruchslogik, bis diese Produktpolicy ausdrücklich bestätigt ist.

Die Persistenz reserviert bis dahin ausschließlich einen versionierbaren `DailyEntitlementRecord` mit Policyversion und bereits bestätigten Claim-IDs. Sie führt keine lokale/UTC-Rollbackregel als angenommene Wahrheit ein. Nach Produktfreigabe erhält die technische Umsetzung ein eigenes oder ersetzendes ADR, einen Zustandsautomaten, Nutzerkommunikation und Clock-/Zeitzonentests. Der Blocker betrifft weder lokale Rätsel noch anderen Fortschritt.

## 12. Datenschutz und Datenlöschung

Save und lokale Logs liegen im App-Sandboxspeicher und werden nicht automatisch übertragen. „Lokale Daten löschen“ entfernt Save, Backup, Pending, Logs und SDK-lokale Analyticsdaten soweit APIs dies erlauben. Storekäufe bleiben beim Store und können wiederhergestellt werden. Vor Löschung zeigt die UI exakt, dass Gameplay-Fortschritt nicht wiederherstellbar ist; die Aktion benötigt ausdrückliche Bestätigung.

## 13. Tests

Pflicht sind Roundtrip-, unbekannte-Felder-, unbekannte-Version-, truncation-, bad-hash-, pending-write-, backup-, split-brain-, low-disk-, permission-, migration-, idempotency-, duplicate-callback-, clock-rollback- und Restoretests. Gerätetests unterbrechen die App während Writes und prüfen Wiederherstellung auf Android und iOS.

## Referenzen

[1]: ../DECISIONS/ADR-006-lokale-persistenz-und-offline-first.md "ADR-006 – Lokaler atomarer Save als Offline-First-Wahrheit"
[2]: ./GAME_STATE_MODEL.md "Game State Model v0.1"
[3]: ../Stammstrecken_Puzzle_Konzept_00-15/06_Fortschritt_Belohnungen_und_Meisterschaft.md "Stammstrecken-Puzzle – Fortschritt, Belohnungen und Meisterschaft"
