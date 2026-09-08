# Persistence v0.2

## 1. Ziel und Wahrheiten

Der lokale, versionierte Save-Snapshot ist die autoritative Quelle für Gameplay-Fortschritt, Entwürfe, Geduldspunkte, kosmetische Freischaltungen, Einstellungen und persistente Mobile-Operationen. Das Spiel ist vollständig offline spielbar. Analytics, Crashlytics und Werbung speichern oder rekonstruieren keine Gameplay-Wahrheit.

Der Apple App Store beziehungsweise Google Play Store ist die externe Wahrheit für den nicht konsumierbaren Anspruch `remove_ads`. Der lokale Save enthält einen ausfallsicheren Cache, den persistenten Operationszustand und verarbeitete Transaktionsreferenzen. Es gibt keine Cloud-Synchronisation und keinen `ISaveSyncPort`.

## 2. Speicherorte und Dateien

Alle Dateien liegen in Unitys app-privatem `persistentDataPath`. Nur `STP.Infrastructure.Persistence` besitzt den Pfad.

| Datei | Zweck |
|---|---|
| `save-v1.json` | letzter erfolgreich atomar bestätigter Snapshot. |
| `save-v1.backup.json` | unmittelbar vorheriger gültiger Snapshot. |
| `save-v1.pending.json` | vollständig zu schreibender Kandidat; nur mit gültigem Envelope wiederherstellbar. |
| `save-v1.migration-source.json` | unveränderte Sicherung vor Schema- oder Hashprofilmigration. |
| `diagnostics-ring.jsonl` | begrenztes lokales, redigiertes Diagnosejournal. |

PlayerPrefs wird nicht für Spielstände, Währung, Entitlements, Claimreservierungen oder Entwürfe verwendet.

## 3. Save-Envelope und Hashprofil

| Feld | Vertrag |
|---|---|
| `saveSchemaVersion` | positive Ganzzahl; initial `1`; versioniert die Payloadform. |
| `payloadHashProfile` | geschlossenes Profil; initial `STP-SAVE-JCS-1`. |
| `generation` | bei jedem erfolgreichen Commit streng erhöhtes 64-Bit-Integer im Save-Ganzzahlbereich. |
| `createdAtUtc` | RFC-3339-UTC-Diagnosezeit; keine Konfliktwahrheit. |
| `appVersion`, `buildId` | Diagnose und Migrationsnachweis. |
| `contentCatalogHash` | Katalogstand, gegen den IDs validiert wurden. |
| `payload` | vollständiger `GameProfileState` plus aktive Entwürfe und Operationen. |
| `payloadSha256` | kleingeschriebener SHA-256-Hexwert über die vom Profil definierten Bytes. |

Die Prüfsumme erkennt Korruption und unvollständige Writes, ist aber kein Manipulationsschutz.

### 3.1 `STP-SAVE-JCS-1`

Die Hashprojektion ist exakt das JSON-Objekt unter dem Envelopefeld `payload`; Envelopefelder werden nicht mitgehasht. Die Bytes sind die UTF-8-Ausgabe des **JSON Canonicalization Scheme nach RFC 8785** über diesen Payload.[3]

Der Vertrag ist vollständig:

- kein Byte Order Mark und kein abschließendes Zeilenende;
- Objektschlüssel rekursiv gemäß RFC 8785 nach UTF-16-Codeeinheiten sortiert;
- Arrayreihenfolgen unverändert;
- Strings gemäß JCS/ECMAScript escaped, ohne Unicode-Normalisierung;
- ungültige Surrogate und Nicht-I-JSON-Daten werden abgewiesen;
- keine Fließkommazahlen, NaN oder Infinity im Savevertrag;
- Ganzzahlen ausschließlich in `[-9007199254740991, 9007199254740991]`; größere technische Werte als validierte Dezimalstrings;
- Hashfunktion SHA-256; Ausgabe 64 kleingeschriebene Hexzeichen.

`STP-SAVE-JCS-1` ist eine Serializer-**Vertragsversion**, keine Bibliotheksversion. Ein Bibliothekswechsel ist nur zulässig, wenn alle Bytegoldens unverändert bleiben.

### 3.2 Alte Saves und Profilwechsel

Der Leser wählt den Verifier zuerst aus `payloadHashProfile`. Erst nach erfolgreicher Hashprüfung wird die DTO-/Fachvalidierung ausgeführt. Danach folgen sequenzielle `saveSchemaVersion`- und gegebenenfalls Hashprofilmigrationen. Das Ergebnis wird als neue Generation mit dem Zielprofil geschrieben.

Ein unbekanntes Profil liefert `SAVE_HASH_PROFILE_UNSUPPORTED`, nicht `SAVE_CORRUPT`. Der Kandidat bleibt unverändert erhalten und löst keinen automatischen leeren Save aus. Ein neues Profil benötigt einen Reader für jedes noch unterstützte Profil, Golden Input/Bytes/Hash, Migration und explizite Supportgrenze. Ein fehlendes Profil in einer künftig tatsächlich veröffentlichten Legacyversion darf nur über eine versionsgebundene, dokumentierte Legacyregel interpretiert werden; es gibt keinen heuristischen Serializer-Fallback. Vor v0.2 wurde kein Production-Save veröffentlicht, daher beginnt der erste Scaffold direkt mit `STP-SAVE-JCS-1`.

## 4. Payloadstruktur

| Bereich | Inhalt |
|---|---|
| `profile` | lokale Profil-ID, erste Nutzung, höchste freigegebene Inhalte, Meisterschaft. |
| `levels` | Records pro stabiler Level-ID plus `puzzleHashAtFirstCompletion`, Abschluss, Sterne, Zeiten und endliche Claims. |
| `endless` | nächster Generationsordinal, höchstens 20 aktive Endless-Deskriptoren, höchstens 64 terminale Ordinalintervalle und die jüngsten 64 terminalen Detailrecords. |
| `ledgerCheckpoint` | kompaktierter Saldo-/Hashkettenstand. |
| `economyJournal` | begrenzte idempotente Gutschriften/Belastungen nach dem Checkpoint. |
| `inventory` | gezielt freigeschaltete und ausgewählte kosmetische IDs samt Kaufreferenz. |
| `seasonProgressCache` | abgeleitete Route-/Abschnitt-/Seasonstände plus Quellhash. |
| `drafts` | höchstens ein aktiver Entwurf je Level, begrenzt auf die jüngsten 20, inklusive Undo-Diffs. |
| `rewardClaims` | `POST_CLEAR_PATIENCE`-Reservationen/-Terminals und künftig freigegebene Claimarten. |
| `dailyStatus` | reservierter `DailyEntitlementRecord`; keine Tagespolicy vor `BLOCKER-PROD-002`. |
| `entitlements` | `remove_ads`, Store, logischer Produktkey, letzte bestätigte Transaktionsreferenz und Reconciliationstatus. |
| `pendingOperations` | persistente Reward-/Kauf-/Restore-/Finalisierungsoperationen ohne Receipt-/Tokenrohwerte. |
| `settings` | Locale, Lautstärkegruppen, Haptik, Accessibility- und Privacypräferenzen. |
| `dataLifecycle` | Consent-/Datenerfassungsrevision und letzter lokaler Reset. |

Unbekannte Level- oder Asset-IDs werden als `orphaned` erhalten, diagnostiziert und bei erneut verfügbarem Content reaktiviert.

## 5. Atomarer Schreibalgorithmus

Jeder Commit erhält eine fortlaufende `generation`:

1. neuen unveränderlichen Snapshot erzeugen und fachlich validieren;
2. Payload mit dem benannten `payloadHashProfile` kanonisieren und SHA-256 berechnen;
3. vollständigen Envelope nach `pending` schreiben;
4. Stream flushen und bestmöglichen persistenten Plattformflush anfordern;
5. `pending` erneut öffnen, Profil dispatchen, parsen und Hash prüfen;
6. gültige Hauptdatei nach `backup` rotieren;
7. `pending` über eine plattformgetestete Replace/Rename-Operation zur Hauptdatei machen;
8. Hauptdatei erneut auf Profil, Generation und Hash prüfen;
9. erst danach `SaveCommitted(generation)` melden.

Kann eine einzelne Dateisystemoperation nicht atomar garantiert werden, verwendet der Adapter Copy-on-Write über die drei Kandidaten und lädt ausschließlich vollständig validierte Generationen. Es gibt höchstens einen Writer; weitere Mutationen werden seriell zusammengeführt.

## 6. Lade- und Recoveryalgorithmus

`main`, `backup` und `pending` werden unabhängig gelesen. Die Pipeline je Kandidat lautet: Envelopegrenzen → Profilverifier → Payloadhash → DTO/Schema → Fachinvarianten → Migration auf Kopie. Unter allen gültigen unterstützten Kandidaten gewinnt die höchste Generation. Gleichstand mit unterschiedlichem Hash ist `SAVE_SPLIT_BRAIN`; `main` wird konservativ bevorzugt und beide Dateien bleiben erhalten.

Falls kein Kandidat gültig ist, darf ein leerer Save nur nach sichtbarer Recoveryinformation und bewusster Nutzerentscheidung entstehen. „Erneut versuchen“, „lokales Backup verwenden“ und erst zuletzt „neuen Spielstand beginnen“ bleiben getrennt. Destruktives Zurücksetzen verlangt Bestätigung.

## 7. Autosave-Policy

| Auslöser | Verhalten |
|---|---|
| Zell-/Batchcommand oder Undo | 500-ms-Debounce, spätestens 2 Sekunden nach erster ungeschriebener Mutation. |
| App geht in Hintergrund / verliert Fokus | sofortiger Snapshot ohne Netzwerk. |
| Level gelöst | logischer Commit vor Zugfahrt und Ergebnisreward. |
| Economy-/Inventaraktion | atomarer Commit vor Erfolgsmeldung. |
| Reward-/Kaufcallback | idempotent verarbeiten und sofort committen. |
| Änderung einer persistenten Operation | sofortiger Commit vor externem Folgeschritt. |
| Einstellung | 500-ms-Debounce; lokale Wirkung sofort. |

Datei-I/O geschieht außerhalb des Renderframes. Quit ist nur Best Effort; Korrektheit hängt nicht davon ab.

## 8. Saveversionen und Migrationen

Jede Version besitzt einen expliziten DTO-Typ und genau einen Migrator `vN -> vN+1`. Migrationen sind rein, deterministisch und greifen nicht auf Netzwerk, Locale oder aktuelle Wanduhr für fachliche Defaults zu.

Vor einer Migration wird die unveränderte Quelle gesichert. Jede Zwischenversion wird einzeln migriert, validiert und roundtrip-serialisiert. Sterne, Ownership, Entitlements, Claimterminals und Ledgerbalance dürfen nicht sinken. Kann ein Pflichtwert nicht neutral hergeleitet werden, stoppt die Migration mit `SAVE_MIGRATION_NEEDS_DECISION`. Downgrade-Schreiben ist verboten.

Golden Tests umfassen immer ursprünglichen Envelope, Hashprofil, kanonische Payloadbytes, ursprünglichen Hash, migriertes Objekt, Zielbytes und Zielhash. Dadurch kann ein fachlich gültiger alter Save nicht allein wegen eines Serializerwechsels als korrupt gelten.

## 9. Economycheckpoint, Journal und Kompaktierung

`LedgerCheckpoint` enthält `ledgerContractVersion`, `throughSequence`, `balancePatience`, `entryChainHashSha256` und `sourceSaveGeneration`. `economyJournal` enthält streng sequenzielle Einträge mit `transactionId`, `reasonCode`, `amount`, `subjectId`, Katalog-/Claimbezug und UTC-Diagnosezeit.

Der aktuelle Saldo ist `checkpoint.balancePatience + sum(journal.amount)`. Jeder Command prüft zusätzlich, dass die Hashkette und Sequenzen lückenlos sind.

| Grenze | Vertrag |
|---|---|
| Harte Journalgrenze | höchstens 512 Einträge oder 256 KiB kanonische JSON-Bytes. |
| Kompaktierungsschwelle | ab 384 Einträgen oder 192 KiB beim nächsten erfolgreichen Savecommit. |
| Diagnosefenster | jüngste 64 terminale Einträge bleiben nach Kompaktierung erhalten. |
| Transaktionskollision | gleiche ID/gleicher Inhalt ist No-op; gleiche ID/anderer Inhalt blockiert. |

Kompaktierung ist deterministisch nach `sequence`. Ein entfernter Eintrag wird in Saldo, `throughSequence` und der Hashkette des Checkpoints gebunden. Er darf nur entfernt werden, wenn seine Deduplikationswahrheit in einem terminalen Fachrecord verbleibt:

- Level-, Stern-, Routen- und Rewardclaims in Level-/Progress-/Claimrecords;
- kosmetische Käufe im Inventory;
- IAP im Entitlement-/Operationsrecord;
- Endless-Claims in terminalen Ordinalintervallen.

Ein Eintrag ohne terminale Fachwahrheit blockiert Kompaktierung mit `ECO-COMPACTION-NONTERMINAL`. Checkpointbildung und Journalkürzung erfolgen im selben atomaren Savecommit. Endlosnutzung lässt dadurch weder Journal noch Claim-ID-Liste unbegrenzt wachsen.

### 9.1 Bounded Endless State

Jeder reservierte `generationOrdinal` befindet sich lückenlos entweder in einem aktiven Record oder in genau einem terminalen Statusintervall. Es gibt höchstens 20 aktive Records; sie sind dieselben höchstens 20 persistierten Entwürfe aus Abschnitt 4. Wird ein Entwurf nach der bestehenden Jüngsten-20-Regel verdrängt, wird sein Ordinal im selben Commit terminal `ABANDONED` und darf nie wieder erzeugt oder belohnt werden.

Terminale Status-/Claimwahrheit wird als sortierte, disjunkte Run-Length-Intervalle gespeichert und bei jedem Commit maximal zusammengeführt. Über alle Statusklassen zusammen sind höchstens 64 Intervalle zulässig. Die jüngsten 64 terminalen Instanzen behalten zusätzlich den vollständigen Reproduktionsdeskriptor als Diagnosefenster; ältere Details werden in eine fortlaufende SHA-256-Hashkette des Endless-Checkpoints aufgenommen und entfernt. Der Checkpoint enthält `throughOrdinal`, `terminalChainHashSha256` und die kompakten Intervalle.

Würde eine neue Reservation die Grenze von 20 aktiven Records oder ein Terminalcommit die Grenze von 64 Intervallen überschreiten, wird **keine** neue Instanz erzeugt. Application liefert `ENDLESS_RETENTION_LIMIT` und bietet an, einen alten Entwurf bewusst als `ABANDONED` zu schließen; es gibt keine stille Löschung. Da jeder aktive Record eine Intervalllücke erklärt und maximal 20 existieren, kann korrekte Zusammenführung die 64er-Grenze regulär einhalten. Migrationen prüfen lückenlose Ordinale, Intervallordnung, Detailfenster und Hashkette.

## 10. Einmaliger `POST_CLEAR_PATIENCE`-Claim

Die fachliche Claim-ID lautet `reward-claim:POST_CLEAR_PATIENCE:<levelId>`. Provider-Reward-ID und lokale Operation-ID sind ausschließlich Audit-/Callbackfelder.

| Zustand | Bedeutung |
|---|---|
| `AVAILABLE` | kein Record; Claim kann reserviert werden. |
| `RESERVED` | genau eine persistierte Operation darf eine Anzeige ausführen. |
| `RECONCILIATION_REQUIRED` | Ausgang nach Crash/Timeout unbekannt; keine zweite Anzeige zulässig. |
| `COMMITTED` | +10 und terminaler Claim wurden atomar persistiert. |

Eligibilityprüfung und `RESERVED` werden in einem Savecommit durchgeführt. Parallele Operationen verlieren auf `expectedSaveGeneration`. Rewardcallback, Claimterminal und Ledgergutschrift werden in einem gemeinsamen Savecommit geschrieben. Ein bestätigtes `CLOSED_NO_REWARD` beziehungsweise `FAILED` gibt die Reservation atomar frei. Ein unbekannter Ausgang bleibt gesperrt, bis Adapterreplay oder eine eindeutige terminale Antwort vorliegt. Späte Callbacks derselben Reservation sind idempotent; Callbacks einer freigegebenen älteren Operation werden quarantänisiert und erzeugen keinen zweiten Grant.

## 11. IAP-Verifikation, Grant und Storefinalisierung

Persistente Operationszustände sind:

```text
STARTED -> EVIDENCE_RECEIVED -> VERIFIED -> GRANTED_NOT_FINALIZED -> FINALIZED
                               \-> REJECTED
any non-terminal ambiguity -> RECONCILIATION_REQUIRED
```

„Verifiziert“ bedeutet im client-only Vertrag: Der Storeadapter hat die plattformspezifische signierte Transaktion beziehungsweise Receipt-/Tokenantwort mit den offiziellen Store-/Unity-IAP-Mechanismen lokal geprüft. Logischer Produktkey, Plattformprodukt-ID, Bundle-/Package-ID, Environment, Kaufstatus und Transaktionsreferenz müssen zur Buildkonfiguration passen. Rohreceipt und Token werden nicht geloggt, in Analytics geschrieben oder dauerhaft im Klartext in generischen Diagnosen gespeichert.

Die Reihenfolge ist verbindlich:

1. Belegreferenz als `EVIDENCE_RECEIVED` persistieren.
2. Lokal prüfen; gültig wird `VERIFIED`, ungültig `REJECTED`, unklar `RECONCILIATION_REQUIRED`.
3. `remove_ads` und `GRANTED_NOT_FINALIZED` **atomar** persistieren.
4. Erst danach Google Purchase acknowledge beziehungsweise Apple Transaction finish anfordern.
5. Erfolg als `FINALIZED` persistieren.

Retry für Schritt 4 verwendet denselben Operations-/Transaktionsschlüssel, exponentielle Abstände von 5 Sekunden, 30 Sekunden, 5 Minuten, 30 Minuten und danach höchstens alle 6 Stunden bei Appaktivität. Es gibt keinen aggressiven Hintergrundloop. Timeout oder Offlinezustand entziehen das bereits persistierte Entitlement nicht.

Crash vor Schritt 3 wird durch Store-Replay/Restore erneut validiert. Crash zwischen 3 und 5 dupliziert den Grant nicht und wiederholt nur Finalisierung. Crash nach Storeerfolg, aber vor lokalem `FINALIZED`, wird bei der nächsten Produkt-/Ownershipabfrage abgeglichen. Restore verwendet dieselbe Pipeline. Eine leere oder fehlerhafte Abfrage ist keine Revocation. Nur eine eindeutige verifizierte Refund-/Revocationaussage darf `REVOCATION_CONFIRMED` setzen. Widersprüchliche Antworten werden `RECONCILIATION_REQUIRED`; ein zuvor bestätigtes Werbefrei-Entitlement bleibt konservativ aktiv.

## 12. Tagesstatus und Uhr

`BLOCKER-PROD-002` verhindert weiterhin die Implementierung und Veröffentlichung der Betriebslage-des-Tages-Anspruchslogik. Persistenz reserviert nur einen versionierbaren Record mit bestätigten Claim-IDs und führt keine lokale-/UTC-Regel als Wahrheit ein.

## 13. Datenschutz und Datenlöschung

Save und lokale Logs liegen im App-Sandboxspeicher und werden nicht automatisch übertragen. „Lokale Daten löschen“ entfernt Save, Backup, Pending, Logs und SDK-lokale optionale Telemetriedaten soweit APIs dies erlauben. Storekäufe bleiben beim Store und können wiederhergestellt werden. Die UI erklärt vor bestätigter Löschung, dass Gameplay-Fortschritt ohne Cloudsave nicht wiederherstellbar ist.

## 14. Tests

Pflicht sind Roundtrip-, Hashprofil-, JCS-Cross-Tool-, unbekanntes-Profil-, Schema-/Profilmigrations-, Truncation-, bad-hash-, pending-write-, backup-, split-brain-, low-disk-, permission-, idempotency-, Claimparallelitäts-, Spätcallback-, Rewardcrash-, IAP-Phasencrash-, Acknowledge-/Finish-Retry-, Restore-, Revocation-, Ledgergrenzen- und Kompaktierungstests. Gerätetests unterbrechen die App während Writes und Storeoperationen auf Android und iOS.

## Referenzen

[1]: ../DECISIONS/ADR-014-save-kanonisierung-und-ledgerkompaktierung.md "ADR-014 – Save-Kanonisierung und Ledgerkompaktierung"
[2]: ../DECISIONS/ADR-015-mobile-transaktionen-und-privacy-default-off.md "ADR-015 – Mobile Transaktionen und Privacy Default-Off"
[3]: https://www.rfc-editor.org/rfc/rfc8785 "RFC 8785 – JSON Canonicalization Scheme"
[4]: ./GAME_STATE_MODEL.md "Game State Model v0.2"
